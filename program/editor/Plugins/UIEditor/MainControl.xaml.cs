using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace UIEditor
{
    /// <summary>
    /// MainControl.xaml 的交互逻辑
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "UIEditor")]
    //[EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/UIEditor")]
    [Guid("159BB080-5662-45CA-AC20-6F742AA9BA7C")]
    [PartCreationPolicy(CreationPolicy.Shared)]
	public partial class MainControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin, EditorCommon.PluginAssist.IEditorDockManagerDragDropOperation
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

        //bool mInitializeFinish = false;

        bool mSnapEnable = true;
        public bool SnapEnable
        {
            get { return mSnapEnable; }
            set
            {
                mSnapEnable = value;

                UIDrawPanel.SnapEnable = value;

                OnPropertyChanged("SnapEnable");
            }
        }

        public string PluginName
        {
            get { return "UIEditor"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "UIEditor",
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

        UIResourceInfo mUIResInfo;
        public void SetObjectToEdit(object[] obj)
        {
            var uiResInfo = obj[0] as UIResourceInfo;
            if (uiResInfo == null)
                return;
            mUIResInfo = uiResInfo;
            uiResInfo.HostControl = this;

            OpenFile(uiResInfo.AbsResourceFileName);
        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        public void StartDrag()
        {

        }
        public void EndDrag()
        {

        }

        public MainControl()
		{
            this.InitializeComponent();

            RegisterDataTemplate();

            Program.mDrawPanel = UIDrawPanel;
            Program.mMainControl = this;

            //mInitializeFinish = true;

            foreach (var info in EditorCommon.Assist.ScreenSizeManager.Instance.ScreenSizeList)
            {
                ComboBox_ScreenSize.Items.Add(info);
            }
            ComboBox_ScreenSize.SelectedIndex = 0;  
            //ComboBox_ScreenSize.SelectedItem = Assist.ScreenSizeManager.Instance.CurrentScreenSizeInfo;
            //Assist.ScreenSizeManager.Instance.CurrentScreenSizeInfo

            UIDrawPanel.UIControlsContainerHolder = ControlsContainer;
            UIDrawPanel.ProAndBindPanel = Pro_Bind_Panel;
            UIDrawPanel.OnUpdateIndex = ControlsBrowser.SelectTemplateIndex;
            UIDrawPanel.OnRootFormChanged += ControlsContainer.SetRootForm;
            UIDrawPanel.OnRootFormChanged += TriggerPanel.SetRootForm;
            UIDrawPanel.OnWinControlPropertyChangedEvent += TriggerPanel.OnWinControlPropertyChanged;
            UIDrawPanel.OnDeleteControls = TriggerPanel.OnDeleteControls;
            //UIDrawPanel.OnSelectedWinControlsCollectionChanged += TriggerPanel.OnSelectedWinControlsCollectionChanged;
            //ControlsContainer.DrawPanel = UIDrawPanel;

            Pro_Bind_Panel.OnPropertyChangedEvent += OnDirtyPropertyChange;

        }
        
        void RegisterDataTemplate()
        {
            var template = this.TryFindResource("UITypesSelect") as DataTemplate;
            WPG.Program.RegisterDataTemplate("UITypesSelect", template);

            template = this.TryFindResource("UITemplateSetter") as DataTemplate;
            WPG.Program.RegisterDataTemplate("UITemplateSetter", template);
        }

        public void OnDirtyPropertyChange()
        {
            if (mUIResInfo != null)
                mUIResInfo.UnsaveVisibility = Visibility.Visible;
        }

        public void Tick()
        {
            UIDrawPanel.Tick();
        }
                
        private void ComboBox_ScreenSize_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            EditorCommon.Assist.ScreenSizeManager.Instance.CurrentScreenSizeInfo = ComboBox_ScreenSize.SelectedItem as EditorCommon.Assist.ScreenSizeInfo;
            UIDrawPanel.SetScreenSize(EditorCommon.Assist.ScreenSizeManager.Instance.CurrentScreenSizeInfo);
        }

        private void Button_Copy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UIDrawPanel.CopySelectedControls();
        }

        private void Button_Paste_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UIDrawPanel.PasteControls();
        }

		private void Button_Fill_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UIDrawPanel.FillParent();
        }

        private void Button_ImageSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UIDrawPanel.SetToImageSize();
        }

        private void Button_HCenter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UIDrawPanel.SetToHorizontalCenter();
        }

        private void Button_VCenter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UIDrawPanel.SetToVerticalCenter();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(OpenedFileName);
            if (mUIResInfo != null)
            {
                mUIResInfo.ParentBrowser.ReCreateSnapshot(mUIResInfo);
            }

            // 重置大小
            //EditorCommon.Assist.ScreenSizeManager.Instance.CurrentScreenSizeInfo = ComboBox_ScreenSize.SelectedItem as EditorCommon.Assist.ScreenSizeInfo;
            //UIDrawPanel.SetScreenSize(EditorCommon.Assist.ScreenSizeManager.Instance.CurrentScreenSizeInfo);
            UIDrawPanel.WinRoot.UpdateLayout(true);

            if (mUIResInfo != null)
                mUIResInfo.UnsaveVisibility = Visibility.Collapsed;
            //EditorCommon.Assist.ScreenSizeManager.Instance.Save();
        }

        #region Menu

        string mStrOpenedFileName = "";
        public string OpenedFileName
        {
            get { return mStrOpenedFileName; }
            set
            {
                mStrOpenedFileName = value;
                TextBlock_OpendFileName.Text = mStrOpenedFileName;
                OnPropertyChanged("OpenedFileName");
            }
        }

        public void CreateNewUI()
        {
            OpenedFileName = null;
            UIDrawPanel.CreateNew();
            Pro_Bind_Panel.PropertyInstanceObject = null;
        }

        public void CreateNewTemplate()
        {
            OpenedFileName = null;
            UIDrawPanel.CreateNewTemplate();
            Pro_Bind_Panel.PropertyInstanceObject = null;
        }

        public void OpenFile(string file)
        {
            OpenedFileName = file;

            Pro_Bind_Panel.PropertyInstanceObject = null;

            file = file.Replace("\\", "/");
            file = file.Replace(CSUtility.Support.IFileManager.Instance.Root, "");
            UIDrawPanel.Clear();
            UIDrawPanel.WinRootForm = UISystem.WinBase.CreateFromXml(file);
        }

        public void SaveFile(string file)
        {
            OpenedFileName = file;

            file = file.Replace("\\", "/");
            file = file.Replace(CSUtility.Support.IFileManager.Instance.Root, "");
            UISystem.WinBase.SaveToXml(file, UIDrawPanel.WinRootForm);

            if (UIDrawPanel.WinRootForm is UISystem.WinForm)
            {
                //var startIndex = file.LastIndexOf('/') + 1;
                //var endIndex = file.IndexOf('.');
                //Common.ReflectionManager.Instance.ReloadUI(file.Substring(startIndex, endIndex - startIndex));
                CCore.Support.ReflectionManager.Instance.ReloadAll();
            }
            else if (UIDrawPanel.WinRootForm is UISystem.Template.ControlTemplate)
            {
//                ControlsBrowser.UpdateTemplateShow();
                CCore.Support.ReflectionManager.Instance.ReloadAll();
            }
        }

        private void MenuItem_New_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CreateNewUI();
        }

        private void MenuItem_NewTemplate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CreateNewTemplate();
        }

        private void MenuItem_Open_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "xml files(*.xml)|*.xml|All files(*.*)|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OpenFile(ofd.FileName);
            }
        }

        private void MenuItem_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(OpenedFileName))
                MenuItem_SaveAs_Click(sender, e);
            else
                SaveFile(OpenedFileName);
        }

        private bool SearchForSaveFileName(string dir, string fileName)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(dir))
            {
                var chkFileName = file.Replace("\\", "/");
                chkFileName = chkFileName.Substring(chkFileName.LastIndexOf('/') + 1);

                if (chkFileName == fileName)
                    return true;
            }

            foreach (var subDir in System.IO.Directory.EnumerateDirectories(dir))
            {
                if (SearchForSaveFileName(subDir, fileName))
                    return true;
            }

            return false;
        }

        private void MenuItem_SaveAs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "xml files(*.xml)|*.xml|All files(*.*)|*.*";

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //var fileName = sfd.FileName.Replace("\\", "/");
                //fileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
                //if(SearchForSaveFileName(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultUIDirectory, fileName))
                //{
                //    MessageBox.Show("UI文件重名，请保证所有UI文件都不重名", "保存失败");
                //    goto gotoOpenToSave;
                //}

                // 重置所有WinBase的ID
                ChangedWinIdWithChildrens(UIDrawPanel.WinRootForm);

                SaveFile(sfd.FileName);
            }
        }

        private void ChangedWinIdWithChildrens(UISystem.WinBase win)
        {
            win.ResetId();

            foreach (var child in win.GetChildWindows())
            {
                ChangedWinIdWithChildrens(child);
            }
        }

        #endregion

    }
}