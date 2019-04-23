using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace WeatherEditor
{
    /// <summary>
    /// Interaction logic for IlluminationPanel.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "WeatherEditor")]
    [EditorCommon.PluginAssist.PluginMenuItem("窗口/天气系统")]
    [Guid("C98D82C8-3EB9-42D0-9E12-1F5CBCB19617")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class IlluminationPanel : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public string PluginName
        {
            get { return "天气系统"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "天气系统的编辑",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void SetObjectToEdit(object[] obj)
        {

        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        public void Tick()
        {
        }

        ///////////////////////////////////////////////////////////

        public IlluminationPanel()
        {
            InitializeComponent();

            UpdateIllumination();
            InitializeComboBox();
            CCore.Program.OnWorldLoaded += Program_MainWorld_OnWorldLoaded;
        }

        private void Program_MainWorld_OnWorldLoaded(System.String strAbsFolder, string componentName, CCore.World.World world)
        {
            this.Dispatcher.Invoke(() =>
            {
                InitializeComboBox();
            });
        }

        //ObservableCollection<SunProperty> SunDatas = new ObservableCollection<SunProperty>();
        List<CCore.WeatherSystem.SunProperty> SunDatas = new List<CCore.WeatherSystem.SunProperty>();

        public void UpdateIllumination()
        {
            if (CCore.Client.MainWorldInstance == null)
                return;

            SystemPropertyGrid.Instance = CCore.WeatherSystem.IlluminationManager.Instance;
            GlobalPropertyGrid.Instance = CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination;

            TFHoursControl.OnKeyPointSelectionChanged = KeyPointSelectionChanged;
        }

        private void KeyPointSelectionChanged(CCore.WeatherSystem.SunProperty sunProperty)
        {
            SkyLightPropertyGrid.Instance = sunProperty;
        }

        private void InitializeComboBox()
        {
            ComboBox_Illumination.Items.Clear();

            CCore.WeatherSystem.IlluminationManager.Instance.LoadAllIllumination(CCore.Client.MainWorldInstance);

            Dictionary<Guid, CCore.WeatherSystem.Illumination> dic;
            if (!CCore.WeatherSystem.IlluminationManager.Instance.IlluminationDic.TryGetValue(CCore.Client.MainWorldInstance.Id, out dic))
                return;

            int i = 0, selectIdx = -1;
            foreach (var ilu in dic.Values)
            {
                TextBlock text = new TextBlock()
                {
                    Style = TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TextBlockStyle_Default")) as Style
                };
                BindingOperations.SetBinding(text, TextBlock.TextProperty, new Binding("Name") { Source = ilu });
                text.Tag = ilu;
                ComboBox_Illumination.Items.Add(text);

                if (CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination != null && ilu.Id == CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination.Id)
                    selectIdx = i;

                i++;
            }

            ComboBox_Illumination.SelectedIndex = selectIdx;
        }

        private void ComboBox_Illumination_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_Illumination.SelectedIndex < 0)
                return;

            var text = ComboBox_Illumination.SelectedItem as TextBlock;
            var ilu = text.Tag as CCore.WeatherSystem.Illumination;
            CCore.WeatherSystem.IlluminationManager.Instance.ChangedToIllumination(CCore.Client.MainWorldInstance, ilu.Id);
            GlobalPropertyGrid.Instance = CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination;
            TFHoursControl.UpdateColorLineShow();
            TFHoursControl.UpdateSunRiseSetTimeShow();
            SkyLightPropertyGrid.Instance = null;
        }

        private void Button_Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var win = new InputWindow.InputWindow();
            win.Description = "天气系统名称: ";
            if (win.ShowDialog() == true)
            {
                var name = System.Convert.ToString(win.Value);
                var ilu = CCore.WeatherSystem.IlluminationManager.Instance.AddIllumination(CCore.Client.MainWorldInstance, name);
                TextBlock text = new TextBlock();
                BindingOperations.SetBinding(text, TextBlock.TextProperty, new Binding("Name") { Source = ilu });
                text.Tag = ilu;
                ComboBox_Illumination.Items.Add(text);
                ComboBox_Illumination.SelectedIndex = ComboBox_Illumination.Items.Count - 1;
            }
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ComboBox_Illumination.SelectedIndex < 0)
                return;

            if (CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination == null)
                return;

            var id = CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination.Id;
            var name = CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination.Name;
            if (CCore.WeatherSystem.IlluminationManager.Instance.DelIllumination(CCore.Client.MainWorldInstance, id))
            {
                var absFile = CCore.WeatherSystem.Illumination.GetIlluminationAbsFile(CCore.Client.MainWorldInstance, id);
                if(!string.IsNullOrEmpty(absFile))
                {
                    if (EditorCommon.VersionControl.VersionControlManager.Instance.Enable)
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                        {
                            if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"天气系统: {name} {absFile}删除失败!");
                            }
                            else
                            {
                                EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                                {
                                    if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                                    {
                                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"天气系统: {name} {absFile}删除失败!");
                                        return;
                                    }
                                }, absFile, $"AutoCommit 删除天气系统{name}");
                            }
                        }, absFile);
                    }
                    else
                    {
                        try
                        {
                            if (System.IO.File.Exists(absFile))
                                System.IO.File.Delete(absFile);
                        }
                        catch(System.Exception ex)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"天气系统: {name}删除失败!\r\n{ex.ToString()}");
                        }
                    }
                }

                ComboBox_Illumination.Items.RemoveAt(ComboBox_Illumination.SelectedIndex);
                if (ComboBox_Illumination.Items.Count > 0)
                    ComboBox_Illumination.SelectedIndex = 0;
            }
        }

        private void Button_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ComboBox_Illumination.SelectedIndex < 0)
                return;

            if (CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination != null)
            {
                CCore.WeatherSystem.IlluminationManager.Instance.SaveIllumination(CCore.Client.MainWorldInstance, CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination.Id);
                CCore.Client.MainWorldInstance.WorldInit.WeatherSystemId = CCore.WeatherSystem.IlluminationManager.Instance.CurrentIllumination.Id;
                var absDir = CCore.Client.MainWorldInstance.GetWorldLastLoadedAbsFolder("");
                CCore.Client.MainWorldInstance.WorldInit.Save(absDir);
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }
    }
}
