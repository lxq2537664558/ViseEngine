using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace PostProcessEditor
{
    /// <summary>
    /// Interaction logic for PostProcessPanel.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "PostProcessEditor")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/PostProcessEditor")]
    [Guid("5A6F3322-023C-4771-88BF-8CD5C2C2B747")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class PostProcessPanel : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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
            get { return "PostProcessEditor"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "PostProcessEditor",
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
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

        public void Tick()
        {

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

        ////////////////////////////////////////////////////////////////

        public PostProcessPanel()
        {
            this.InitializeComponent();

            UpdatePostProcess();
        }

        public void UpdatePostProcess()
        {
            int i = 0;
            foreach (var p in CCore.Client.MainWorldInstance.PostProceses)
            {
                switch (p.m_Type)
                {
                    case CCore.Graphics.enPostProcessType.HDR:
                        {
                            PostProcessPropertyGrid_HDR.Visibility = System.Windows.Visibility.Visible;
                            PostProcessPropertyGrid_HDR.Headline = p.Name;
                            PostProcessPropertyGrid_HDR.Instance = p;
                        }
                        break;

                    case CCore.Graphics.enPostProcessType.Sharpen:
                        {
                            PostProcessPropertyGrid_Sharpen.Visibility = System.Windows.Visibility.Visible;
                            PostProcessPropertyGrid_Sharpen.Headline = p.Name;
                            PostProcessPropertyGrid_Sharpen.Instance = p;
                        }
                        break;

                    default:
                        {
                            var tabItem = PPS.Items[i] as TabItem;
                            tabItem.Header = p.Name;
                            WPG.PropertyGrid pg = tabItem.Content as WPG.PropertyGrid;
                            if (pg != null)
                            {
                                pg.Visibility = System.Windows.Visibility.Visible;
                                pg.Headline = p.Name;
                                pg.Instance = p;
                            }

                            i++;
                        }
                        break;
                }

            }


        }

        //private void OnPostProcessPropertyExpanded(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    UpdatePostProcess();
        //}
    }
}
