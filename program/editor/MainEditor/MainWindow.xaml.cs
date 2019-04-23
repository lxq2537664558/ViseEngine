using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MainEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DockControl.Controls.DockAbleWindowBase, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        static MainWindow smInstance = null;
        public static MainWindow Instance
        {
            get
            {
                if (smInstance == null)
                    smInstance = new MainWindow();
                return smInstance;
            }
        }

        //public delegate void Delegate_OnTellServerUpdateActionNotify(string name);
        //public Delegate_OnTellServerUpdateActionNotify OnTellServerUpdateActionNotify;

        public delegate void Delegate_OnPlantLight(CCore.Light.ELightType lightType);
        public Delegate_OnPlantLight OnPlantLight;
        //public delegate void Delegate_OnPlantDecal(FrameSet.Decal.DecalActorType decalType);
        //public Delegate_OnPlantDecal OnPlantDecal;
        //public delegate void Delegate_RunGMCommand(string cmd, string[] args);
        //public Delegate_RunGMCommand OnRunGMCommand;

        //public void _RunGMCommand(string cmd, string[] args)
        //{
        //    if (OnRunGMCommand != null)
        //        OnRunGMCommand(cmd, args);
        //}

        //public bool DirectMoveWhenNotFindPath
        //{
        //    get { return CCore.Engine.Instance.DirectMoveWhenNotFindPath; }
        //    set
        //    {
        //        CCore.Engine.Instance.DirectMoveWhenNotFindPath = value;
        //        OnPropertyChanged("DirectMoveWhenNotFindPath");
        //    }
        //}

        bool mShowLineCheckAssist = false;
        public bool ShowLineCheckAssist
        {
            get { return mShowLineCheckAssist; }
            set
            {
                mShowLineCheckAssist = value;
                OnPropertyChanged("ShowLineCheckAssist");
            }
        }

        bool mShowGameAssistWindow = true;
        [CSUtility.Support.DataValueAttribute("ShowGameAssistWindow")]
        public bool ShowGameAssistWindow
        {
            get { return mShowGameAssistWindow; }
            set
            {
                mShowGameAssistWindow = value;

                if (mShowGameAssistWindow)
                    GameAssistWindow.Instance.Show();
                else
                    GameAssistWindow.Instance.Close();

                OnPropertyChanged("ShowGameAssistWindow");
            }
        }

        private CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> mSelectedActors = new CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor>();
        public CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> SelectedActors
        {
            get { return mSelectedActors; }
        }
//        public void OnSelectedSceneActorsUpdate(CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> actors)
//        {
//            mSelectedActors = actors;

//            if (mSelectedActors.Count > 0)
//            {
//#warning 场景对象操作
//                var actor = mSelectedActors[mSelectedActors.Count - 1];
//                //MainEditor.Panel.TransformPanel.Instance.UpdateSelectedActorTranslateShow();
//                OnShowProperty(actor);
//            }
//        }
        
        public void ResetGameCamera()
        {
            SlimDX.Vector3 pos = new SlimDX.Vector3(-30, 30, -30);
            SlimDX.Vector3 lookat = new SlimDX.Vector3(0, 0, 0);
            EditorCommon.Program.EditorCameraController.SetPosLookAtUp(ref pos, ref lookat, ref SlimDX.Vector3.UnitY);
        }

        public DockControl.DropSurface CurrentSurface
        {
            get
            {
                foreach (var surface in mSurfaces)
                {
                    if (surface.SurfaceRect == Rect.Empty)
                        continue;

                    return surface;
                }

                return null;
            }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            
            EditorCommon.Program.InitializeMainDispatcher();

            CanClose = false;

            var ctrl = new DockControl.Controls.DockAbleContainerControl(this);
            var tabCtrl = new DockControl.Controls.DockAbleTabControl();
            ctrl.Content = tabCtrl;
            MainGrid.Children.Add(ctrl);

            DockControl.DockManager.Instance.OnGetElementInstance = _OnGetElementInstance;
            DockControl.DockManager.Instance.OnGetWindowInstance = _OnGetWindowInstance;

            RegisterDataTemplate();
        }

        void RegisterDataTemplate()
        {
            var template = this.TryFindResource("ActorSetter") as DataTemplate;
            WPG.Program.RegisterDataTemplate("ActorSetter", template);
        }

        private FrameworkElement _OnGetElementInstance(Type elementType, string keyValue)
        {
            //    var ctrl = Program.GetControl(elementType, keyValue);
            //    if (ctrl is SourceViewControl)
            //    {
            //        var svCtrl = ctrl as SourceViewControl;
            //        var sourceType = (SourceControlInfo.enSourceType)System.Enum.Parse(typeof(SourceControlInfo.enSourceType), keyValue);
            //        if (sourceType != SourceControlInfo.enSourceType.None)
            //        {
            //            svCtrl.InitSourcesShow(sourceType);
            //            //svCtrl.OnShowProperty = new SourceViewControl.Delegate_ShowProperty(ShowProperty);
            //            svCtrl.OnPlantMeshTemplate = new SourceViewControl.Delegate_OnPlantMeshTemplate(_OnPlantMeshTemplate);
            //            svCtrl.OnPlantRoleTemplate = new SourceViewControl.Delegate_OnPlantRoleTemplate(_OnPlantRoleTemplate);
            //            svCtrl.OnPlantEffectTemplate = new SourceViewControl.Delegate_OnPlantEffectTemplate(_OnPlantEffectTemplate);
            //            svCtrl.OnPlantPrefabResource = new SourceViewControl.Delegate_OnPlantPrefabResource(_OnPlantPrefabResource);
            //            svCtrl.OnPlantAudioResource = new SourceViewControl.Delegate_OnPlantAudioResource(_OnPlantAudioResource);
            //        }
            //    }

            //    return ctrl;     
                   
            return System.Activator.CreateInstance(elementType) as FrameworkElement;
//             var ctrl = Program.GetPluginControl(pluginItem.PluginObject);
//             return null;
        }

        private DockControl.Controls.DockAbleWindowBase _OnGetWindowInstance(Type winType, string keyValue)
        {
            switch (keyValue)
            {
                case "MainWindow":
                    return this;
            }

            return null;
        }

        public static void FinalInstance()
        {
            smInstance = null;
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {            
            this.Hide();
            e.Cancel = true; 
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            //var d3dCtrl = Program.GetD3DControl();
            //if (d3dCtrl != null)
            //    d3dCtrl.EnableTick = true;
            this.mPriority = DockControl.DockManager.Instance.GetPriority();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            //var d3dCtrl = Program.GetD3DControl();
            //if (d3dCtrl != null)
            //    d3dCtrl.EnableTick = false;
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {



            // 重新生成一个新的ID，以便保存布局的时候不会覆盖原来的布局模板
            ArrangementId = Guid.NewGuid();
            
            EditorCommon.PluginAssist.PluginOperation.OnSetObjectToPluginForEdit -= PluginOperation_OnSetObjectToPluginForEdit;
            EditorCommon.PluginAssist.PluginOperation.OnSetObjectToPluginForEdit += PluginOperation_OnSetObjectToPluginForEdit;

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {            
        }

        //public SourceViewControl.Delegate_OnPlantMeshTemplate OnPlantMeshTemplate;
        //public SourceViewControl.Delegate_OnPlantRoleTemplate OnPlantRoleTemplate;
        //public SourceViewControl.Delegate_OnPlantEffectTemplate OnPlantEffectTemplate;
        //public SourceViewControl.Delegate_OnPlantPrefabResource OnPlantPrefabResource;
        //public SourceViewControl.Delegate_OnPlantAudioResource OnPlantAudioResource;
        //private void _OnPlantMeshTemplate(CCore.Mesh.MeshTemplate mesh)
        //{
        //    if (OnPlantMeshTemplate != null)
        //        OnPlantMeshTemplate(mesh);
        //}

        //private void _OnPlantRoleTemplate(CSUtility.Data.NPCType roleType, CSUtility.Data.RoleTemplateBase roleTemplate)
        //{
        //    if (OnPlantRoleTemplate != null)
        //        OnPlantRoleTemplate(roleType, roleTemplate);
        //}

        //private void _OnPlantEffectTemplate(CCore.Effect.EffectTemplate effect)
        //{
        //    if (OnPlantEffectTemplate != null)
        //        OnPlantEffectTemplate(effect);
        //}

        //private void _OnPlantPrefabResource(CCore.World.Prefab.PrefabResource res)
        //{
        //    if (OnPlantPrefabResource != null)
        //        OnPlantPrefabResource(res);
        //}

        //private void _OnPlantAudioResource(string audioFile)
        //{
        //    if (OnPlantAudioResource != null)
        //        OnPlantAudioResource(audioFile);
        //}


#region 菜单操作

        public void SetPluginMenu(string menuString, MainEditor.PluginAssist.PluginItem pluginItem)
        {
            if(string.IsNullOrEmpty(menuString))
                return;

            var menuStrings = menuString.Split('/');
            if (menuStrings.Length <= 0)
                return;

            bool bSet = false;
            foreach (var tempItem in Menu_Main.Items)
            {
                MenuItem menuItem = tempItem as MenuItem;
                if (menuItem == null)
                    continue;

                if (SetPluginMenu(menuStrings, pluginItem, menuItem))
                {
                    bSet = true;
                    break;
                }
            }

            if (!bSet)
            {
                MenuItem subMenuItem = new MenuItem()
                {
                    Header = menuStrings[0],
                };
                subMenuItem.Style = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "MenuItem_Default")) as System.Windows.Style;
                Menu_Main.Items.Add(subMenuItem);
                SetPluginMenu(menuStrings, pluginItem, subMenuItem);
            }
        }
        private bool SetPluginMenu(string[] menuStrings, MainEditor.PluginAssist.PluginItem pluginItem, MenuItem menuItem)
        {
            if(menuStrings.Length <= 0)
                return false;

            if (menuStrings.Length > 1)
            {
                if (menuItem.Header.ToString() == menuStrings[0])
                {
                    var tempStrings = new string[menuStrings.Length - 1];
                    for (int i = 0; i < tempStrings.Length; i++)
                    {
                        tempStrings[i] = menuStrings[i + 1];
                    }

                    foreach (var tempItem in menuItem.Items)
                    {
                        MenuItem subMenuItem = tempItem as MenuItem;
                        if (subMenuItem == null)
                            continue;

                        if (SetPluginMenu(tempStrings, pluginItem, subMenuItem))
                            return true;
                    }

                    MenuItem newSubMenuItem = new MenuItem()
                    {
                        Header = tempStrings[0]
                    };
                    newSubMenuItem.Style = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "MenuItem_Default")) as System.Windows.Style;
                    menuItem.Items.Add(newSubMenuItem);
                    if(tempStrings.Length > 1)
                        return SetPluginMenu(tempStrings, pluginItem, newSubMenuItem);
                    else
                    {
                        pluginItem.HostMeuItem = newSubMenuItem;
                        newSubMenuItem.Tag = pluginItem;
                        newSubMenuItem.Click += PluginMenuIem_Click;
                        return true;
                    }
                }
            }
            //else
            //{
            //    var subMenuItem = new MenuItem()
            //    {
            //        Header = menuStrings[0],
            //        Tag = pluginItem
            //    };
            //    pluginItem.HostMeuItem = subMenuItem;
            //    subMenuItem.Click += PluginMenuIem_Click;
            //    menuItem.Items.Add(subMenuItem);

            //    return true;
            //}

            return false;
        }

        // 插件生成菜单点击操作
        void PluginMenuIem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null)
                return;

            var pluginItem = menuItem.Tag as MainEditor.PluginAssist.PluginItem;
            if (pluginItem == null)
                return;

            if (pluginItem.PluginObject is System.Windows.FrameworkElement)
            {
                var ctrl = Program.GetPluginControl(pluginItem.PluginObject);
                //var ctrl = Program.GetControl(typeof(MainEditor.PluginAssist.PluginControlContainer), pluginItem.PluginName) as MainEditor.PluginAssist.PluginControlContainer;// .GetPluginControl(pluginItem.PluginObject);
                if (ctrl.IsShowing)
                {
                    var parentTabItem = Program.GetParent(ctrl, typeof(TabItem)) as TabItem;
                    var parentTabControl = Program.GetParent(ctrl, typeof(TabControl)) as TabControl;
                    if (parentTabControl != null && parentTabItem != null)
                    {
                        parentTabControl.SelectedItem = parentTabItem;
                    }

                    // 将包含该控件的窗体显示到最前
                    var parentWin = Program.GetParent(ctrl, typeof(Window)) as Window;
                    Program.BringWindowToTop(new System.Windows.Interop.WindowInteropHelper(parentWin).Handle);
                }
                else
                {
                    var tabItem = new DockControl.Controls.DockAbleTabItem()
                    {
                        Header = menuItem.Header,
                        Content = ctrl
                    };

                    CurrentSurface.AddChild(tabItem);
                }
            }
        }

        // 工具菜单生成
        private void GenerateToolMenu(double maxProgressValue)
        {
            var assem = System.Reflection.Assembly.GetExecutingAssembly();
            var types = assem.GetTypes();
            var progressStep = (types.Length > 0) ? (maxProgressValue / types.Length) : 0;
            foreach (var value in types)
            {
                var attrUC = (MainEditorAttribute.UserControlAttribute[])value.GetCustomAttributes(typeof(MainEditorAttribute.UserControlAttribute), false);
                if (attrUC.Length < 1)
                    continue;

                this.Dispatcher.Invoke(() =>
                {
                    var menuItem = new MenuItem()
                    {
                        Header = attrUC[0].ControlName,
                        Tag = value,
                    };
                    menuItem.Click += MenuItem_Tool_Click;
                    MenuItem_Tools.Items.Add(menuItem);
                });

                EditorLoading.Instance.UpdateProcess("初始化工具菜单...", progressStep);
            }


            this.Dispatcher.Invoke(() =>
            {
                MenuItem_Tools.Items.Add(new Separator()
                {
                    Style = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "MenuSeparatorStyle")) as System.Windows.Style
                });
            });

            //foreach (SourceControlInfo.enSourceType enValue in System.Enum.GetValues(typeof(SourceControlInfo.enSourceType)))
            //{
            //    if (enValue == SourceControlInfo.enSourceType.None)
            //        continue;

            //    var menuItem = new MenuItem()
            //    {
            //        Header = enValue.ToString(),
            //        Tag = typeof(SourceViewControl),
            //    };
            //    menuItem.Click += MenuItem_Tool_Click;
            //    MenuItem_Tools.Items.Add(menuItem);
            //}
        }

        void MenuItem_Tool_Click(object sender, RoutedEventArgs e)
        {
            /*var menuItem = sender as MenuItem;
            if (menuItem == null)
                return;

            if (CurrentSurface == null)
                return;

            var ctrl = Program.GetControl((Type)(menuItem.Tag), (string)menuItem.Header) as DockControl.IDockAbleControl;
            if (ctrl.IsShowing)
            {
                var parentTabItem = Program.GetParent(ctrl as FrameworkElement, typeof(TabItem)) as TabItem;
                var parentTabControl = Program.GetParent(ctrl as FrameworkElement, typeof(TabControl)) as TabControl;
                if (parentTabControl != null && parentTabItem != null)
                {
                    parentTabControl.SelectedItem = parentTabItem;
                }

                // 将包含该控件的窗体显示到最前
                var parentWin = Program.GetParent(ctrl as FrameworkElement, typeof(Window)) as Window;

                Program.BringWindowToTop(new System.Windows.Interop.WindowInteropHelper(parentWin).Handle);
            }
            else
            {
                //if (ctrl is SourceViewControl)
                //{
                //    var svCtrl = ctrl as SourceViewControl;
                //    var sourceType = (SourceControlInfo.enSourceType)System.Enum.Parse(typeof(SourceControlInfo.enSourceType), (string)menuItem.Header);
                //    if (sourceType != SourceControlInfo.enSourceType.None)
                //    {
                //        svCtrl.InitSourcesShow(sourceType);
                //        //svCtrl.OnShowProperty = new SourceViewControl.Delegate_ShowProperty(ShowProperty);
                //        svCtrl.OnPlantMeshTemplate = new SourceViewControl.Delegate_OnPlantMeshTemplate(_OnPlantMeshTemplate);
                //        svCtrl.OnPlantRoleTemplate = new SourceViewControl.Delegate_OnPlantRoleTemplate(_OnPlantRoleTemplate);
                //        svCtrl.OnPlantEffectTemplate = new SourceViewControl.Delegate_OnPlantEffectTemplate(_OnPlantEffectTemplate);
                //        svCtrl.OnPlantPrefabResource = new SourceViewControl.Delegate_OnPlantPrefabResource(_OnPlantPrefabResource);
                //        svCtrl.OnPlantAudioResource = new SourceViewControl.Delegate_OnPlantAudioResource(_OnPlantAudioResource);
                //    }
                //}

                var tabItem = new DockControl.Controls.DockAbleTabItem()
                {
                    Header = menuItem.Header,
                    Content = ctrl
                };

                CurrentSurface.AddChild(tabItem);
            }*/
        }
        
        // 文件 ----------------------------------------------

        /*
        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "地图文件(*.map)|*.map";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (OnUnselectedAllActor != null)
                    OnUnselectedAllActor();

                var mapFileName = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(sfd.FileName);
                mapFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(mapFileName, CSUtility.Support.IFileManager.Instance.Root);
                CCore.Client.MainWorldInstance.SaveWorld(mapFileName, true, CCore.World.World.enSaveWorldType.All);

                _OnSaveWorld(mapFileName, CCore.World.World.enSaveWorldType.All);
            }
        }*/

        public delegate void Delegate_OnLoadWorld(string mapPath);
        public Delegate_OnLoadWorld OnLoadWorld;
        public void _OnLoadWorld(string mapPath)
        {
            if (OnLoadWorld != null)
                OnLoadWorld(mapPath);
        }

        public delegate void Delegate_OnNewWorld();
        public Delegate_OnNewWorld OnNewWorld;
        public void _OnNewWorld()
        {
            if (OnNewWorld != null)
                OnNewWorld();
        }

        // strDir路径相对于CSUtility.Support.IFileManager.Instance.Root

        /*public delegate void Delegate_OnSaveWorld(string strDir, CCore.World.World.enSaveWorldType swType);
        public Delegate_OnSaveWorld OnSaveWorld;
        public void _OnSaveWorld(string strDir, CCore.World.World.enSaveWorldType swType)
        {
            if (OnSaveWorld != null)
                OnSaveWorld(strDir, swType);
        }*/

        #region 编辑

        private void MenuItem_Undo_Click(object sender, RoutedEventArgs e)
        {
            // 撤销
            EditorCommon.UndoRedoManager.Instance.Undo();
        }
        private void MenuItem_Redo_Click(object sender, RoutedEventArgs e)
        {
            // 重做
            EditorCommon.UndoRedoManager.Instance.Redo();
        }

        #endregion

        // 工具 ----------------------------------------------

        //private void MenuItem_DelegateMethodEditor_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DelegateMethodEditorControl.IsShowing)
        //    {
        //        var parentTabItem = Program.GetParent(DelegateMethodEditorControl, typeof(TabItem)) as TabItem;
        //        var parentTabControl = Program.GetParent(DelegateMethodEditorControl, typeof(TabControl)) as TabControl;
        //        if (parentTabControl != null && parentTabItem != null)
        //        {
        //            parentTabControl.SelectedItem = parentTabItem;
        //        }

        //        // 将包含该控件的窗体显示到最前
        //        var parentWin = Program.GetParent(DelegateMethodEditorControl, typeof(Window)) as Window;
        //        Program.BringWindowToTop(new System.Windows.Interop.WindowInteropHelper(parentWin).Handle);
        //    }
        //    else
        //    {
        //        var tabItem = new DockControl.Controls.DockAbleTabItem()
        //        {
        //            Header = DelegateMethodEditorControl.KeyValue,
        //            Content = DelegateMethodEditorControl
        //        };
        //        CurrentSurface.AddChild(tabItem);
        //    }
        //}

        public delegate void Delegate_OnGenerateMap(UInt32 picSize, UInt32 levelDelta, string folder);
        public Delegate_OnGenerateMap OnGenerateMap;
        private void MenuItem_GenerateMap_Click(object sender, RoutedEventArgs e)
        {
            MiniMapGeneratWindow win = new MiniMapGeneratWindow();
            if (win.ShowDialog() == true)
            {
                if (OnGenerateMap != null)
                    OnGenerateMap(win.PicSize, win.LevelCountDelta, win.Path);
            }
        }
        private void MenuItem_HotkeySet_Click(object sender, RoutedEventArgs e)
        {
            Panel.HotkeyConfigWindow.Instance.ShowDialog();
        }
        private void MenuItem_SVNAutoCommit_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_SVNAutoCommit.IsChecked = !MenuItem_SVNAutoCommit.IsChecked;
            EditorCommon.VersionControl.VersionControlManager.Instance.Enable = MenuItem_SVNAutoCommit.IsChecked;
        }

        // 调试 ----------------------------------------------
        
        public UISystem.WinForm m_Form = null;
        private void MenuItem_HideUI_Click(object sender, RoutedEventArgs e)
        {
#warning 隐藏UI
            //if (CCore.Support.ReflectionManager.Instance.GetUIForm("MainUI").Visibility == CSUtility.Visibility.Visible)
            //    CCore.Support.ReflectionManager.Instance.GetUIForm("MainUI").Visibility = CSUtility.Visibility.Collapsed;
            //else
            //    CCore.Support.ReflectionManager.Instance.GetUIForm("MainUI").Visibility = CSUtility.Visibility.Visible;
        }

        private void MenuItem_MultiRender_Click(object sender, RoutedEventArgs e)
        {
            CCore.Engine.IsMultiThreadRendering = !CCore.Engine.IsMultiThreadRendering;
            MenuItem_MultiRender.IsChecked = CCore.Engine.IsMultiThreadRendering;
        }

        //private void MenuItem_SetOutPut_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem_SetOutPut.IsChecked = !MenuItem_SetOutPut.IsChecked;
        //    if (MenuItem_SetOutPut.IsChecked)
        //        MidLayer.IDllImportAPI.vLoadPipe_SetOutputLoadInfo(1);
        //    else
        //        MidLayer.IDllImportAPI.vLoadPipe_SetOutputLoadInfo(0);

        //}

        private void MenuItem_CaptureMRT_Click(object sender, RoutedEventArgs e)
        {
            if (EditorCommon.Program.GameREnviroment != null)
                EditorCommon.Program.GameREnviroment.CaptureMRT();
        }

        private void MenuItem_ShowHurtBox_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_ShowHurtBox.IsChecked = !MenuItem_ShowHurtBox.IsChecked;
            if (MenuItem_ShowHurtBox.IsChecked)
                CCore.Client.MainWorldInstance.ShowHurtBox = true;
            else
                CCore.Client.MainWorldInstance.ShowHurtBox = false;
        }

        //private void MenuItem_HideFPS_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem_HideFPS.IsChecked = !MenuItem_HideFPS.IsChecked;
        //    if (MenuItem_HideFPS.IsChecked)
        //        FrameSet.AppConfig.Instance.ForceShowDebugInfo = true;
        //    else
        //        FrameSet.AppConfig.Instance.ForceShowDebugInfo = false;
        //}

        private void MenuItem_CheckMemoryState_Click(object sender, RoutedEventArgs e)
        {
            CCore.Engine.Instance.CheckNativeMemoryState("StartCheckMemory");
        }

        public delegate void Delegate_OnUpdateShaderCache();
        public Delegate_OnUpdateShaderCache OnUpdateShaderCache;
        private void MenuItem_UpdateShaderCache_Click(object sender, RoutedEventArgs e)
        {
            if (OnUpdateShaderCache != null)
                OnUpdateShaderCache();
        }

        // 性能 ----------------------------------------------
        private void MenuItem_NVPerf_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_NVPerf.IsChecked = !MenuItem_NVPerf.IsChecked;
            //if (MenuItem_NVPerf.IsChecked)
            //    MidLayer.IDllImportAPI.vNVPerf_SetEnable(true);
            //else
            //    MidLayer.IDllImportAPI.vNVPerf_SetEnable(false);
        }
        private void MenuItem_CaptureBottleneck_Click(object sender, RoutedEventArgs e)
        {
            //MidLayer.IDllImportAPI.vNVPerf_BottleneckUtilization(true);
            //MidLayer.IDllImportAPI.vNVPerf_CaptureBottleneckGraph();
        }
        private void MenuItem_CaptureUtilization_Click(object sender, RoutedEventArgs e)
        {
            //MidLayer.IDllImportAPI.vNVPerf_BottleneckUtilization(false);
            //MidLayer.IDllImportAPI.vNVPerf_CaptureBottleneckGraph();
        }

        private void MenuItem_PluginManager_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PluginAssist.PluginManagerWindow.Instance.ShowDialog();
        }
        

#endregion


#region 布局

        private Guid mArrangementId = Guid.NewGuid();
        public Guid ArrangementId
        {
            get { return mArrangementId; }
            set { mArrangementId = value; }
        }

        private void MenuItem_SaveEditorArrangement_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Arrangement.EditorArrangementManager.Instance.ContainArrangement(this.ArrangementId))
            {
                Arrangement.EditorArrangementManager.Instance.SaveArrangement(this.ArrangementId);
            }
            else
                MenuItem_SaveAsEditorArrangement_Clicked(sender, e);
        }

        private void MenuItem_SaveAsEditorArrangement_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Arrangement.SaveArrangementWindow win = new Arrangement.SaveArrangementWindow();
            if (win.ShowDialog() == true)
            {
                var id = Guid.NewGuid();

                Arrangement.EditorArrangementManager.Instance.AddArrangement(id, win.ArrangementName);
                Arrangement.EditorArrangementManager.Instance.SaveArrangement(id);

                UpdateArrangementConfigs();
            }

        }

        private void MenuItem_DeleteEditorArrangement_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Arrangement.DeleteArrangementWindow win = new Arrangement.DeleteArrangementWindow();
            win.ShowDialog();
        }

        string mResetLayoutFile = CSUtility.Support.IFileConfig.EditorResourcePath + "/EditorArrangements/ResetDefaultConfig.xml";
        private void MenuItem_ResetArrangement_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (EditorCommon.MessageBox.enMessageBoxResult.OK == EditorCommon.MessageBox.Show("确认要重置布局？","提示",EditorCommon.MessageBox.enMessageBoxButton.OKCancel))
            {
                LoadLayoutConfig(mResetLayoutFile);
            }            
        }

        private void SetArrangement(Arrangement.ArrangementData arrangement)
        {
            if (arrangement == null)
                return;

            LoadLayoutConfig(Arrangement.EditorArrangementManager.Instance.GetFileName(arrangement.Id));
        }

        void ArrangementRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            var arg = rb.Tag as Arrangement.ArrangementData;
            SetArrangement(arg);
            ArrangementId = arg.Id;
        }

        public void UpdateArrangementConfigs()
        {
            StackPanel_EditorArrangementShow.Children.Clear();
            StackPanel_EditorArrangement.Children.Clear();

            //int showCount = 5;
            //int i = 0;
            foreach (var arrangement in Arrangement.EditorArrangementManager.Instance.EditorArrangement)
            {
                RadioButton rb = new RadioButton()
                {
                    Content = arrangement.Name,
                    Tag = arrangement,
                    Margin = new System.Windows.Thickness(2),
                    GroupName = "EditorArrangement",
                    Foreground = Brushes.White,
                };
                rb.Checked += ArrangementRadioButton_Checked;


                if (arrangement.Id == this.ArrangementId)
                    rb.IsChecked = true;

                StackPanel_EditorArrangement.Children.Add(rb);

                //if (i < showCount)
                //{
                //    RadioButton rbShow = new RadioButton()
                //    {
                //        Content = arrangement.ArrangementName,
                //        Tag = arrangement,
                //        Margin = new Thickness(2),
                //        HorizontalContentAlignment = HorizontalAlignment.Center,
                //        VerticalContentAlignment = VerticalAlignment.Center,
                //        Style = this.FindResource("RadioButtonStyle_Toggle") as System.Windows.Style,
                //        GroupName = "EditorArrangementShow",
                //    };
                //    rbShow.SetBinding(RadioButton.IsCheckedProperty, new Binding("IsChecked") { Source = rbShow, Mode = BindingMode.TwoWay });

                //    StackPanel_EditorArrangementShow.Children.Add(rbShow);
                //}

                //i++;
            }
        }

#region SaveLoad

        private string mConfigFileName = CSUtility.Support.IFileConfig.EditorResourcePath + "/EditorArrangements/DefaultConfig.xml";

        public override void SaveLayout(CSUtility.Support.XmlNode node, CSUtility.Support.XmlHolder holder)
        {
            var screen = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle);

            node.AddAttrib("Type", this.GetType().Assembly.FullName + "|" + this.GetType().FullName);
            node.AddAttrib("KeyValue", "MainWindow");
            node.AddAttrib("WinState",this.WindowState.ToString());
            node.AddAttrib("LeftRate", ((float)(this.Left - screen.WorkingArea.Left) / (float)screen.WorkingArea.Width).ToString());
            node.AddAttrib("TopRate", ((float)(this.Top - screen.WorkingArea.Top) / (float)screen.WorkingArea.Height).ToString());
            node.AddAttrib("WidthRate", ((float)this.Width / (float)screen.WorkingArea.Width).ToString());
            node.AddAttrib("HeightRate", ((float)this.Height / (float)screen.WorkingArea.Height).ToString());

            for(int i = 0;i < System.Windows.Forms.Screen.AllScreens.Length;++i)
            {
                if (screen.Equals(System.Windows.Forms.Screen.AllScreens[i]))
                {
                    node.AddAttrib("ScreenIndex", i.ToString());
                    break;
                }                    
            }

            var contentNode = node.AddNode("WinContent", "", holder);
            if (MainGrid.Children.Count > 0)
                SaveElement(MainGrid.Children[0] as FrameworkElement, contentNode, holder);
        }

        public override void LoadLayout(CSUtility.Support.XmlNode node)
        {                        
            int screenIndex = 0;
            System.Windows.Forms.Screen screen;

            var att = node.FindAttrib("ScreenIndex");
            if (att != null)
                screenIndex = System.Convert.ToInt32(att.Value);
            if (System.Windows.Forms.Screen.AllScreens.Length <= screenIndex)
            {
                screen = System.Windows.Forms.Screen.PrimaryScreen;
            }
            else
            {
                screen = System.Windows.Forms.Screen.AllScreens[screenIndex];
            }
            if (screen == null)
                return;

            att = node.FindAttrib("LeftRate");
            if (att != null)
                this.Left = System.Convert.ToSingle(att.Value) * (float)screen.WorkingArea.Width + screen.WorkingArea.Left;
            att = node.FindAttrib("TopRate");
            if (att != null)
                this.Top = System.Convert.ToSingle(att.Value) * (float)screen.WorkingArea.Height + screen.WorkingArea.Top;
            att = node.FindAttrib("WidthRate");
            if (att != null)
                this.Width = System.Convert.ToSingle(att.Value) * (float)screen.WorkingArea.Width;
            att = node.FindAttrib("HeightRate");
            if (att != null)
                this.Height = System.Convert.ToSingle(att.Value) * (float)screen.WorkingArea.Height;
            att = node.FindAttrib("WinState");
            if (att != null)
            {
                WindowState state = WindowState.Normal;
                if (Enum.TryParse<WindowState>(att.Value, out state))
                    this.WindowState = state;
            }

            ReSetLocation(screen);

            var contentNode = node.FindNode("WinContent");
            MainGrid.Children.Clear();
            LoadElement(MainGrid, contentNode);
        }        

        public void LoadLayoutConfig(string fileName)
        {
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(fileName);
            if (xmlHolder == null || xmlHolder.RootNode == null)
            {
                DockControl.DockManager.Instance.IsLoading = false;
                return;
            }

            DockControl.DockManager.Instance.IsLoading = true;

            List<DockControl.Controls.DockAbleWindowBase> wins = new List<DockControl.Controls.DockAbleWindowBase>();
            foreach (var win in DockControl.DockManager.Instance.DockableWindows)
            {
                if (win != this)
                {
                    wins.Add(win);
                }
            }

            foreach (var win in wins)
            {
                win.Close();
            }

            foreach (var winNode in xmlHolder.RootNode.FindNodes("DockWindow"))
            {
                var win = DockControl.DockManager.Instance.GetWindowInstance(winNode);
                if (win == null)
                    continue;
                win.Show();
                win.LoadLayout(winNode);
                //DockControl.DockManager.Instance.AddDockableWin(win);
                
            }
            DockControl.DockManager.Instance.IsLoading = false;
        }

        #endregion

        #endregion

        #region 资源反查

        /*        DelegateMethodEditor.MainControl DelegateMethodEditorControl
                {
                    get
                    {
                        return Program.GetControl(typeof(DelegateMethodEditor.MainControl), DelegateMethodEditor.MainControl.StaticKeyValue) as DelegateMethodEditor.MainControl;
                    }
                }
        */

        public void ShowResource(string browserType, object[] resData)
        {
            /*var ctrl = Program.GetControl(typeof(MainEditor.PluginAssist.PluginControlContainer), resourceType) as MainEditor.PluginAssist.PluginControlContainer;
            if (ctrl != null)
            {
                ctrl.PluginObject.SetObjectToEdit(resData);

                if (ctrl.IsShowing)
                {
                    var parentTabItem = Program.GetParent(ctrl as FrameworkElement, typeof(TabItem)) as TabItem;
                    var parentTabControl = Program.GetParent(ctrl as FrameworkElement, typeof(TabControl)) as TabControl;
                    if (parentTabControl != null && parentTabItem != null)
                    {
                        parentTabControl.SelectedItem = parentTabItem;
                    }

                    // 将包含该控件的窗体显示到最前
                    var parentWin = Program.GetParent(ctrl as FrameworkElement, typeof(Window)) as Window;
                    Program.BringWindowToTop(new System.Windows.Interop.WindowInteropHelper(parentWin).Handle);
                }
                else
                {
                    var win = new DockControl.DockAbleWindow();
                    var tabItem = new DockControl.Controls.DockAbleTabItem()
                    {
                        Header = resourceType,
                        Content = ctrl
                    };
                    win.SetContent(tabItem);
                    win.Show();
                }
            }*/
        }

        private void PluginOperation_OnSetObjectToPluginForEdit(object[] data)
        {
            Program.OnOpenEditor(data);
        }


        ////////////        public void ShowResource(EditorCommon.ResourceSearch.enResourceType rType, object resData)
        ////////////        {
        ////////////            SourceControlInfo.enSourceType sType = SourceControlInfo.enSourceType.None;
        ////////////            object sResourceData = null;
        ////////////            DockControl.IDockAbleControl ctrl = null;
        ////////////            Type ctrlType = null;
        ////////////            string keyString = "";

        ////////////            switch (rType)
        ////////////            {
        ////////////                case EditorCommon.ResourceSearch.enResourceType.Texture:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.TextureBrowser;
        ////////////                        sResourceData = resData;
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.Mesh:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.MeshSourceBrowser;
        ////////////                        sResourceData = resData;
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.MeshTemplate:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.MeshTemplateBrowser;
        ////////////                        sResourceData = resData;
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.Material:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.MaterialBrowser;
        ////////////                        sResourceData = "Mat|" + resData.ToString();
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.Technique:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.MaterialBrowser;
        ////////////                        sResourceData = "Tech|" + resData.ToString();
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.Action:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.ActionBrowser;
        ////////////                        sResourceData = resData;
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.AI:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.AIBrowser;
        ////////////                        sResourceData = resData;
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.UVAnim:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.UVAnimBrowser;
        ////////////                        sResourceData = resData;
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.Event:
        ////////////                    {
        /////////////*                        var item = resData as DelegateMethodEditor.EventListItem;
        ////////////                        if (item != null)
        ////////////                            DelegateMethodEditorControl.SelectedEvent(item.EventCallBack.CBType, item.EventId);
        ////////////                        ctrlType = typeof(DelegateMethodEditor.MainControl);
        ////////////                        keyString = DelegateMethodEditor.MainControl.StaticKeyValue;
        ////////////*/
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.RoleTemplate:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.RoleTemplateBrowser;
        ////////////                        sResourceData = resData;
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.Effect:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.EffectBrowser;
        ////////////                        sResourceData = resData;
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////                case EditorCommon.ResourceSearch.enResourceType.Audio:
        ////////////                    {
        ////////////                        sType = SourceControlInfo.enSourceType.AudioBrowser;
        ////////////                        sResourceData = resData;
        ////////////                        ctrlType = typeof(SourceViewControl);
        ////////////                        keyString = sType.ToString();
        ////////////                    }
        ////////////                    break;
        ////////////            }

        ////////////            ctrl = Program.GetControl(ctrlType, keyString) as DockControl.IDockAbleControl;
        ////////////            if (ctrl != null)
        ////////////            {
        ////////////                if (ctrlType == typeof(SourceViewControl))
        ////////////                {
        ////////////                    ((SourceViewControl)ctrl).InitSourcesShow(sType);
        ////////////                    ((SourceViewControl)ctrl).ShowResource(sType, sResourceData);
        ////////////                }

        ////////////                if (ctrl.IsShowing)
        ////////////                {
        ////////////                    var parentTabItem = Program.GetParent(ctrl as FrameworkElement, typeof(TabItem)) as TabItem;
        ////////////                    var parentTabControl = Program.GetParent(ctrl as FrameworkElement, typeof(TabControl)) as TabControl;
        ////////////                    if (parentTabControl != null && parentTabItem != null)
        ////////////                    {
        ////////////                        parentTabControl.SelectedItem = parentTabItem;
        ////////////                    }

        ////////////                    // 将包含该控件的窗体显示到最前
        ////////////                    var parentWin = Program.GetParent(ctrl as FrameworkElement, typeof(Window)) as Window;
        ////////////                    Program.BringWindowToTop(new System.Windows.Interop.WindowInteropHelper(parentWin).Handle);
        ////////////                }
        ////////////                else
        ////////////                {
        ////////////                    var win = new DockControl.DockAbleWindow();
        ////////////                    var tabItem = new DockControl.Controls.DockAbleTabItem()
        ////////////                    {
        ////////////                        Header = keyString,
        ////////////                        Content = ctrl
        ////////////                    };
        ////////////                    win.SetContent(tabItem);
        ////////////                    win.Show();
        ////////////                }
        ////////////            }
        ////////////        }

        #endregion


    }
}
