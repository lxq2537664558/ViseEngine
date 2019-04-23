using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace NavigationEditor
{
    /// <summary>
    /// Interaction logic for NavigationPanel.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "NavigationEditor")]
    [EditorCommon.PluginAssist.PluginMenuItem("窗口/寻路")]
    [Guid("4132CF91-CA66-4EB5-8EF4-93F004F0E320")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class NavigationPanel : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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
            get { return "寻路编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "寻路编辑器",
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
                
        //static NavigationPanel smInstance = null;
        //public static NavigationPanel Instance
        //{
        //    get
        //    {
        //        if (smInstance == null)
        //            smInstance = new NavigationPanel();
        //        return smInstance;
        //    }
        //}

        CCore.Navigation.NavigationGenerateInfo mGenInfo = new CCore.Navigation.NavigationGenerateInfo();
        CCore.Support.ITerrainBrushActor mNavigationBrushActor;

        public NavigationPanel()
        {
            InitializeComponent();

            PG_CreateInfo.Instance = mCreateInfo;

            EditorCommon.WorldEditMode.Instance.OnEditModeChangeFrom += WorldEditMode_OnEditModeChangeFrom;
            EditorCommon.WorldEditMode.Instance.OnEditModeChangeTo += WorldEditMode_OnEditModeChangeTo;

            EditorCommon.GameActorOperation.OnSelectActors += SelectedActors;
            EditorCommon.GameActorOperation.OnUnSelectActors += UnSelectActors;
            EditorCommon.GameActorOperation.OnRemoveActor += GameActorOperation_OnRemoveActor;

            OnWorldLoaded();
            CCore.Program.OnWorldLoaded += Program_OnWorldLoaded;

            // 网格寻路编辑
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("寻路编辑操作", "移动笔刷", "移动寻路绘制笔刷", CCore.MsgProc.BehaviorParameter.enKeys.None, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, null, OnMouseMove);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("寻路编辑操作", "绘制寻路", "绘制寻路", CCore.MsgProc.BehaviorParameter.enKeys.LButton, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, null, OnNavDraw);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("寻路编辑操作", "擦除寻路", "绘制寻路", CCore.MsgProc.BehaviorParameter.enKeys.LButton, true, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, null, OnEraseNavDraw);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("寻路编辑操作", "转动摄像机", "转动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.LButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.RotateCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("寻路编辑操作", "按水平面移动摄像机", "按照水平面移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.MButton, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.HorizontalMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("寻路编辑操作", "按视平面移动摄像机", "按照视平面移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.MButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.ScreenMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("寻路编辑操作", "轴向移动摄像机", "沿摄像机方向移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.RButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.DirMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("寻路编辑操作", "轴向移动摄像机 ", "沿摄像机方向移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.None, false, false, false, EditorCommon.Hotkey.enMouseType.MouseWheel, null, EditorCommon.GameCameraOperation.DirMoveCamera2);

            // 路点编辑
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("路点编辑操作", "预览路点种植", "预览路点种植", CCore.MsgProc.BehaviorParameter.enKeys.None, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, null, OnPlantNavPtMouseMove);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("路点编辑操作", "种植路点", "种植路点", CCore.MsgProc.BehaviorParameter.enKeys.LButton, false, false, false, EditorCommon.Hotkey.enMouseType.None, null, OnPlantNavPt);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("路点编辑操作", "转动摄像机", "转动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.LButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.RotateCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("路点编辑操作", "按水平面移动摄像机", "按照水平面移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.MButton, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.HorizontalMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("路点编辑操作", "按视平面移动摄像机", "按照视平面移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.MButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.ScreenMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("路点编辑操作", "轴向移动摄像机", "沿摄像机方向移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.RButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.DirMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("路点编辑操作", "轴向移动摄像机 ", "沿摄像机方向移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.None, false, false, false, EditorCommon.Hotkey.enMouseType.MouseWheel, null, EditorCommon.GameCameraOperation.DirMoveCamera2);
        }

        private void GameActorOperation_OnRemoveActor(CCore.World.Actor actor)
        {
            if (actor == null)
                return;

            var navActor = actor as CCore.World.NavigationPointActor;
            if (navActor == null)
                return;

            navActor.RemoveAllLinks();
            mNavigationPointActors.Remove(actor.Id);
            CCore.Navigation.Navigation.Instance.NavigationPointData.RemoveNavigationPoint(actor.Id);
        }

        private void OnMouseMove(CCore.MsgProc.BehaviorParameter param, object obj)
        {
            if (mNavigationBrushActor == null)
                return;

            CCore.MsgProc.Behavior.Mouse_Move parameter = param as CCore.MsgProc.Behavior.Mouse_Move;
            var end = EditorCommon.Assist.Assist.IntersectWithWorld(parameter.X, parameter.Y, CCore.Client.MainWorldInstance, EditorCommon.Program.GameREnviroment?.Camera, 0);
            mNavigationBrushActor.Placement.SetLocation(ref end);
            mNavigationBrushActor.UpdateBrush(CCore.Client.MainWorldInstance.Terrain);
        }

        private void OnNavDraw(CCore.MsgProc.BehaviorParameter param, object obj)
        {
            if (mNavigationBrushActor == null)
                return;

            CCore.MsgProc.Behavior.Mouse_Move parameter = param as CCore.MsgProc.Behavior.Mouse_Move;
            var end = EditorCommon.Assist.Assist.IntersectWithWorld(parameter.X, parameter.Y, CCore.Client.MainWorldInstance, EditorCommon.Program.GameREnviroment?.Camera, 0);
            mNavigationBrushActor.Placement.SetLocation(ref end);
            mNavigationBrushActor.UpdateBrush(CCore.Client.MainWorldInstance.Terrain);

            var navData = CCore.Navigation.Navigation.Instance.NavigationData;

            switch(NavBrushType)
            {
                case enBrushType.Block:
                    CCore.Navigation.NavigationAssist.Instance.DrawNavigation(end.X, end.Z, BrushSize, true, false, ref navData);
                    break;
                case enBrushType.Throught:
                    CCore.Navigation.NavigationAssist.Instance.DrawNavigation(end.X, end.Z, BrushSize, false, false, ref navData);
                    break;
            }
            CCore.Navigation.NavigationAssist.Instance.DrawNavigationToData(ref navData);
        }
        private void OnEraseNavDraw(CCore.MsgProc.BehaviorParameter param, object obj)
        {
            if (mNavigationBrushActor == null)
                return;

            CCore.MsgProc.Behavior.Mouse_Move parameter = param as CCore.MsgProc.Behavior.Mouse_Move;
            var end = EditorCommon.Assist.Assist.IntersectWithWorld(parameter.X, parameter.Y, CCore.Client.MainWorldInstance, EditorCommon.Program.GameREnviroment?.Camera, 0);
            mNavigationBrushActor.Placement.SetLocation(ref end);
            mNavigationBrushActor.UpdateBrush(CCore.Client.MainWorldInstance.Terrain);

            var navData = CCore.Navigation.Navigation.Instance.NavigationData;

            switch (NavBrushType)
            {
                case enBrushType.Block:
                    CCore.Navigation.NavigationAssist.Instance.DrawNavigation(end.X, end.Z, BrushSize, true, true, ref navData);
                    break;
                case enBrushType.Throught:
                    CCore.Navigation.NavigationAssist.Instance.DrawNavigation(end.X, end.Z, BrushSize, false, true, ref navData);
                    break;
            }
        }

        private void OnPlantNavPtMouseMove(CCore.MsgProc.BehaviorParameter param, object obj)
        {
            if (mNavigationPointPreviewActor == null)
                return;

            CCore.MsgProc.Behavior.Mouse_Move parameter = param as CCore.MsgProc.Behavior.Mouse_Move;
            var end = EditorCommon.Assist.Assist.IntersectWithWorld(parameter.X, parameter.Y, CCore.Client.MainWorldInstance, EditorCommon.Program.GameREnviroment?.Camera, 0);
            mNavigationPointPreviewActor.Placement.SetLocation(ref end);
        }
        private void OnPlantNavPt(CCore.MsgProc.BehaviorParameter param, object obj)
        {
            var parameter = param as CCore.MsgProc.Behavior.Mouse_Key;
            var end = EditorCommon.Assist.Assist.IntersectWithWorld(parameter.X, parameter.Y, CCore.Client.MainWorldInstance, EditorCommon.Program.GameREnviroment?.Camera, 0);

            AddNavigationPoint(end);
        }

        private void Program_OnWorldLoaded(System.String strAbsFolder, string componentName, CCore.World.World world)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch(componentName)
                {
                    case "场景":
                        OnWorldLoaded();
                        break;
                    case "寻路":
                        InitializeNavigationPointsFromData();
                        break;
                }
            });
        }
        private void OnWorldLoaded()
        {
            if(mNavigationBrushActor != null)
            {
                CCore.Client.MainWorldInstance.RemoveEditorActor(mNavigationBrushActor);
                mNavigationBrushActor.Cleanup();
            }

            // 设置寻路刷
            mNavigationBrushActor = new CCore.Support.ITerrainBrushActor();
            var nbActorInit = new CCore.Support.ITerrainBrushInit();
            mNavigationBrushActor.Initialize(nbActorInit);
            mNavigationBrushActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
            mNavigationBrushActor.ActorName = "寻路刷";
            CCore.Client.MainWorldInstance.AddEditorActor(mNavigationBrushActor);
            mNavigationBrushActor.SetBrushImageFile(CSUtility.Support.IFileConfig.NavigationBrushDecalTextureFile);
            mNavigationBrushActor.Visible = EnableEdit;
            BrushSize = 1;

            // 设置寻路网格显示对象
            var navActor = new CCore.World.Actor();
            var navActorInit = new CCore.World.ActorInit();
            navActor.Initialize(navActorInit);
            navActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
            navActor.ActorName = "寻路辅助对象";
            navActor.Visual = CCore.Navigation.NavigationAssist.Instance;
            navActor.SetPlacement(new CSUtility.Component.StandardPlacement(navActor));
            CCore.Client.MainWorldInstance.AddEditorActor(navActor);
        }

        void WorldEditMode_OnEditModeChangeTo(EditorCommon.WorldEditMode.enEditMode editMode)
        {
            if (editMode == EditorCommon.WorldEditMode.enEditMode.Edit_Navigation)
                EnableEdit = true;
        }

        void WorldEditMode_OnEditModeChangeFrom(EditorCommon.WorldEditMode.enEditMode editMode)
        {
            if (editMode == EditorCommon.WorldEditMode.enEditMode.Edit_Navigation)
                EnableEdit = false;
        }

#region 显示

        bool mShowAutoGenerateNavigation = false;
        public bool ShowAutoGenerateNavigation
        {
            get { return mShowAutoGenerateNavigation; }
            set
            {
                if (value && !CheckNavigationValid())
                    return;

                mShowAutoGenerateNavigation = value;

                if (mShowAutoGenerateNavigation == true &&
                    CCore.Navigation.NavigationAssist.Instance.Visible == false)
                {
                    CCore.Navigation.NavigationAssist.Instance.Visible = true;
                }
                else if (ShowManualNavigation == false && ShowFindedPathNavigation == false)
                {
                    CCore.Navigation.NavigationAssist.Instance.Visible = false;
                }

                CCore.Navigation.NavigationAssist.Instance.ShowAutoNavigation = mShowAutoGenerateNavigation;

                OnPropertyChanged("ShowAutoGenerateNavigation");
            }
        }

        bool mShowManualNavigation = false;
        public bool ShowManualNavigation
        {
            get { return mShowManualNavigation; }
            set
            {
                if (value && !CheckNavigationValid())
                    return;

                mShowManualNavigation = value;

                if (mShowManualNavigation == true &&
                   CCore.Navigation.NavigationAssist.Instance.Visible == false)
                {
                    CCore.Navigation.NavigationAssist.Instance.Visible = true;
                }
                else if (ShowAutoGenerateNavigation == false && ShowFindedPathNavigation == false)
                {
                    CCore.Navigation.NavigationAssist.Instance.Visible = false;
                }

                CCore.Navigation.NavigationAssist.Instance.ShowManualNavigation = mShowManualNavigation;

                OnPropertyChanged("ShowManualNavigation");
            }
        }

        bool mShowDynamicBlock = false;
        public bool ShowDynamicBlock
        {
            get { return mShowDynamicBlock; }
            set
            {
                if (value && !CheckNavigationValid())
                    return;

                mShowDynamicBlock = value;

                if (mShowDynamicBlock == true &&
                   CCore.Navigation.NavigationAssist.Instance.Visible == false)
                {
                    CCore.Navigation.NavigationAssist.Instance.Visible = true;
                }

                CCore.Navigation.NavigationAssist.Instance.ShowDynamicBlock = mShowDynamicBlock;

                OnPropertyChanged("ShowDynamicBlock");
            }
        }

        bool mShowFindedPathNavigation = false;
        public bool ShowFindedPathNavigation
        {
            get { return mShowFindedPathNavigation; }
            set
            {
                if (value && !CheckNavigationValid())
                    return;

                mShowFindedPathNavigation = value;

                if (mShowFindedPathNavigation == true &&
                   CCore.Navigation.NavigationAssist.Instance.Visible == false)
                {
                    CCore.Navigation.NavigationAssist.Instance.Visible = true;
                }
                else if (ShowAutoGenerateNavigation == false && ShowManualNavigation == false)
                {
                    CCore.Navigation.NavigationAssist.Instance.Visible = false;
                }

                CCore.Navigation.NavigationAssist.Instance.ShowFindedPath = mShowFindedPathNavigation;

                OnPropertyChanged("ShowFindedPathNavigation");
            }
        }

        public bool ShowAll
        {
            get { return CCore.Navigation.NavigationAssist.Instance.Visible; }
            set
            {
                if (value && !CheckNavigationValid())
                    return;

                CCore.Navigation.NavigationAssist.Instance.Visible = value;

                ShowAutoGenerateNavigation = value;
                ShowManualNavigation = value;
                ShowDynamicBlock = value;
                ShowFindedPathNavigation = value;
                ShowNavigationPoint = value;
            }
        }

        private void Button_ShowAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ShowAll = true;
        }

        private void Button_HideAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ShowAll = false;
        }

#endregion

#region 生成

        // 地形阻挡倾斜角度值
        public float TerrainBlockAngleDelta
        {
            get { return mGenInfo.mTerrainBlockAngleDelta; }
            set
            {
                mGenInfo.mTerrainBlockAngleDelta = value;

                OnPropertyChanged("TerrainBlockAngleDelta");
            }
        }

        public bool ClearManualData
        {
            get { return mGenInfo.mClearManualData; }
            set
            {
                mGenInfo.mClearManualData = value;
                OnPropertyChanged("ClearManualData");
            }
        }

        private void Button_GenerateNavigation_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ClearManualData)
            {
                if (EditorCommon.MessageBox.Show("此次生成会清除手绘寻路数据，是否确认？\r\n(不想清除请取消勾选 清除手绘数据 选项)", "警告", EditorCommon.MessageBox.enMessageBoxButton.YesNo) != EditorCommon.MessageBox.enMessageBoxResult.Yes)
                    return;
            }

            //if (!CheckNavigationValid(false))
            //{
            //    if (CCore.Navigation.Navigation.Instance.NavigationData == null)
            //    {
            //        CCore.Engine.Instance.MainWorld.InitializeNavigation();
            //    }
            //    CSUtility.Navigation.INavigationInfo info = new CSUtility.Navigation.INavigationInfo();
            //    CCore.Navigation.Navigation.Instance.NavigationData.GetNavigationInfo(ref info);
            //    CCore.Navigation.NavigationAssist.Instance.InitializeNavigationProxy(ref info);
            //}
            CheckNavigationValid(false);
            CCore.Navigation.NavigationAssist.Instance.GenerateNavigation(CCore.Client.MainWorldInstance, mGenInfo);
            ShowAll = true;
        }

        private bool CheckNavigationValid(bool showMessage = true)
        {
            // 检查寻路数据有效性
            //if (CCore.Navigation.NavigationAssist.Instance.HasNavigationData)
            //    return true;



            if (CCore.Navigation.Navigation.Instance.NavigationData != null && !CCore.Navigation.NavigationAssist.Instance.Initialized)
            {
                // 通过寻路数据生成寻路图
                CSUtility.Navigation.NavigationInfo info;// = new CSUtility.Navigation.INavigationInfo();
                CCore.Navigation.Navigation.Instance.NavigationData.GetNavigationInfo(out info);
                CCore.Navigation.NavigationAssist.Instance.InitializeNavigationProxy(ref info);
                CCore.Navigation.NavigationAssist.Instance.BuildNavigationFromData(CCore.Navigation.Navigation.Instance.NavigationData);
                CCore.Navigation.NavigationAssist.Instance.Initialized = true;
                return true;
            }

            //if(showMessage)
            //    EditorCommon.MessageBox.Show("没有寻路数据，请先生成数据!");
            return true;
        }

#endregion

#region 编辑

        int mBrushSize = 1;
        public int BrushSize
        {
            get { return mBrushSize; }
            set
            {
                mBrushSize = value;

                if(mNavigationBrushActor != null)
                {
                    var REnviroment = EditorCommon.Program.GameREnviroment;
                    if (REnviroment != null)
                    {
                        var end = EditorCommon.Assist.Assist.IntersectWithWorld(REnviroment.View.Width / 2, REnviroment.View.Height / 2, CCore.Client.MainWorldInstance, REnviroment.Camera, 0);
                        mNavigationBrushActor.Placement.SetLocation(ref end);

                        float fRad = CCore.Navigation.NavigationAssist.Instance.MeterPerPixelX * 0.5f + mBrushSize * CCore.Navigation.NavigationAssist.Instance.MeterPerPixelX;
                        mNavigationBrushActor.SetRadius(fRad, fRad);
                    }
                }

                OnPropertyChanged("BrushSize");
            }
        }

        public enum enBrushType
        {
            Throught,   // 通过
            Block,      // 阻挡
        }
        public enBrushType NavBrushType = enBrushType.Throught;

        bool mEnableEdit = false;
        EditorCommon.Hotkey.HotkeyGroup mHotKeyGroupStore;
        public bool EnableEdit
        {
            get { return mEnableEdit; }
            set
            {
                if (mEnableEdit == value)
                    return;

                mEnableEdit = value;

                if (EnableEdit)
                {
                    NavigationPointEditMode = enNavigationPointEditMode.None;

                    mHotKeyGroupStore = EditorCommon.Hotkey.HotkeyManager.Instance.CurrentActiveGroup;
                    EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup("寻路编辑操作");
                    EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_Navigation);
                }
                else
                {
                    if(mHotKeyGroupStore != null)
                    {
                        EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup(mHotKeyGroupStore.GroupName);
                    }
                    EditorCommon.WorldEditMode.Instance.RestoreEditMode();
                }
                mNavigationBrushActor.Visible = mEnableEdit;


                OnPropertyChanged("EnableEdit");
            }
        }

        private void Brush_Throught_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            NavBrushType = enBrushType.Throught;
        }

        private void Brush_Block_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            NavBrushType = enBrushType.Block;
        }



#endregion

#region 路点

        public enum enNavigationPointEditMode
        {
            None,
            Add,
            Remove,
            AddLink,
            RemoveLink,
        }
        enNavigationPointEditMode mNavigationPointEditMode = enNavigationPointEditMode.None;
        public enNavigationPointEditMode NavigationPointEditMode
        {
            get { return mNavigationPointEditMode; }
            set
            {
                if (mNavigationPointEditMode == value)
                    return;

                mNavigationPointEditMode = value;

                switch (mNavigationPointEditMode)
                {
                    case enNavigationPointEditMode.Add:
                        {
                            EnableEdit = false;
                            IsAddNavigationPoint = true;
                            IsAddNavigationPointLink = false;
                            IsRemoveNavigationPointLink = false;
                        }
                        break;

                    case enNavigationPointEditMode.AddLink:
                        {
                            EnableEdit = false;
                            IsAddNavigationPoint = false;
                            IsAddNavigationPointLink = true;
                            IsRemoveNavigationPointLink = false;
                        }
                        break;

                    case enNavigationPointEditMode.RemoveLink:
                        {
                            EnableEdit = false;
                            IsAddNavigationPoint = false;
                            IsAddNavigationPointLink = false;
                            IsRemoveNavigationPointLink = true;
                        }
                        break;

                    case enNavigationPointEditMode.None:
                        {
                            IsAddNavigationPoint = false;
                            IsAddNavigationPointLink = false;
                            IsRemoveNavigationPointLink = false;
                        }
                        break;
                }
            }
        }

        protected CCore.World.NavigationPointActor mNavigationPointPreviewActor;

        bool mIsAddNavigationPoint = false;
        public bool IsAddNavigationPoint
        {
            get { return mIsAddNavigationPoint; }
            set
            {
                if (mIsAddNavigationPoint == value)
                    return;

                mIsAddNavigationPoint = value;

                if (mIsAddNavigationPoint)
                {
                    ShowNavigationPoint = true;

                    NavigationPointEditMode = enNavigationPointEditMode.Add;

                    if(mNavigationPointPreviewActor != null)
                    {
                        CCore.Client.MainWorldInstance.RemoveEditorActor(mNavigationPointPreviewActor);
                        mNavigationPointPreviewActor.Cleanup();
                        mNavigationPointPreviewActor = null;
                    }
                    mNavigationPointPreviewActor = new CCore.World.NavigationPointActor();
                    var actorInit = new CCore.World.NavigationPointActorInit();
                    mNavigationPointPreviewActor.Initialize(actorInit);
                    mNavigationPointPreviewActor.ActorName = "路点种植预览";
                    CCore.Engine.Instance.Client.MainWorld.AddEditorActor(mNavigationPointPreviewActor);
                    mHotKeyGroupStore = EditorCommon.Hotkey.HotkeyManager.Instance.CurrentActiveGroup;
                    EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup("路点编辑操作");
                }
                else
                {
                    if (mNavigationPointPreviewActor != null)
                    {
                        CCore.Client.MainWorldInstance.RemoveEditorActor(mNavigationPointPreviewActor);
                        mNavigationPointPreviewActor.Cleanup();
                        mNavigationPointPreviewActor = null;
                    }

                    if (mHotKeyGroupStore != null)
                    {
                        EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup(mHotKeyGroupStore.GroupName);
                    }
                }
                OnPropertyChanged("IsAddNavigationPoint");
            }
        }

        bool mIsAddNavigationPointLink = false;
        public bool IsAddNavigationPointLink
        {
            get { return mIsAddNavigationPointLink; }
            set
            {
                if (mIsAddNavigationPointLink == value)
                    return;

                mIsAddNavigationPointLink = value;

                if (mIsAddNavigationPointLink)
                {
                    ShowNavigationPoint = true;
                    NavigationPointEditMode = enNavigationPointEditMode.AddLink;
                }
                OnPropertyChanged("IsAddNavigationPointLink");
            }
        }

        bool mIsRemoveNavigationPointLink = false;
        public bool IsRemoveNavigationPointLink
        {
            get { return mIsRemoveNavigationPointLink; }
            set
            {
                if (mIsRemoveNavigationPointLink == value)
                    return;

                mIsRemoveNavigationPointLink = value;

                if (mIsRemoveNavigationPointLink)
                {
                    ShowNavigationPoint = true;
                    NavigationPointEditMode = enNavigationPointEditMode.RemoveLink;
                }
                OnPropertyChanged("IsRemoveNavigationPointLink");
            }
        }

        bool mShowNavigationPoint = false;
        public bool ShowNavigationPoint
        {
            get { return mShowNavigationPoint; }
            set
            {
                mShowNavigationPoint = value;

                foreach (var actor in mNavigationPointActors.Values)
                {
                    actor.Visible = value;
                }

                CCore.Program.SetActorTypeShow(CCore.Client.MainWorldInstance, CCore.Program.NavigationPointTypeName, value);

                OnPropertyChanged("ShowNavigationPoint");
            }
        }

        bool mAutoLinkLastSelectedPoint = true;
        public bool AutoLinkLastSelectedPoint
        {
            get { return mAutoLinkLastSelectedPoint; }
            set
            {
                mAutoLinkLastSelectedPoint = value;

                OnPropertyChanged("AutoLinkLastSelectedPoint");
            }
        }

        Dictionary<Guid, CCore.World.NavigationPointActor> mNavigationPointActors = new Dictionary<Guid, CCore.World.NavigationPointActor>();

        private void Button_RemoveNavigationPoint_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveSelectedPoints();
        }

        public void InitializeNavigationPointsFromData()
        {
            if (CCore.Navigation.Navigation.Instance.NavigationPointData == null)
                return;

            foreach (var actor in mNavigationPointActors)
            {
                CCore.Client.MainWorldInstance.RemoveEditorActor(actor.Value);
            }
            mNavigationPointActors.Clear();

            List<CSUtility.Navigation.NavigationPoint> ptList = new List<CSUtility.Navigation.NavigationPoint>();
            if (CCore.Navigation.Navigation.Instance.NavigationPointData.GetNavigationDatasFromPtr(ref ptList))
            {
                // 创建路点
                int i = 0;
                foreach (var pt in ptList)
                {
                    var navActor = new CCore.World.NavigationPointActor(pt.Id);
                    navActor.ParticipationLineCheck = false;
                    var actorInit = new CCore.World.NavigationPointActorInit();
                    navActor.Initialize(actorInit);
                    navActor.ActorName = "路点" + i;
                    SlimDX.Vector3 vec = new SlimDX.Vector3(pt.PosX, pt.PosY, pt.PosZ);
                    navActor.Placement.SetLocation(ref vec);

                    mNavigationPointActors[navActor.Id] = navActor;
                    CCore.Client.MainWorldInstance.AddEditorActor(navActor);
                    i++;
                }

                // 设置连线
                foreach (var pt in ptList)
                {
                    var actor = mNavigationPointActors[pt.Id];

                    foreach (var linkId in pt.LinkIds)
                    {
                        actor.AddLink(mNavigationPointActors[linkId]);
                    }
                }
            }
        }

        public void RemoveSelectedPoints()
        {
            mLastSelectedActor = null;

            foreach (var actor in mSelectedActors)
            {
                var navActor = actor as CCore.World.NavigationPointActor;
                if (navActor == null)
                    continue;

                CCore.Client.MainWorldInstance.RemoveEditorActor(actor);
                EditorCommon.GameActorOperation.RemoveActor(actor);
            }
        }

        private void Button_AddNavigationLink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            for (int i = 0; i < mSelectedActors.Count - 1; i++)
            {
                var actor1 = mSelectedActors[i] as CCore.World.NavigationPointActor;
                var pos1 = mSelectedActors[i].Placement.GetLocation();
                for (int j = i + 1; j < mSelectedActors.Count; j++)
                {
                    var actor2 = mSelectedActors[j] as CCore.World.NavigationPointActor;
                    if (actor1 == null || actor2 == null)
                        continue;

                    var pos2 = mSelectedActors[j].Placement.GetLocation();

                    actor1.AddLink(actor2);
                    actor2.AddLink(actor1);

                    CCore.Navigation.Navigation.Instance.NavigationPointData.AddNavigationLink(actor1.Id, actor2.Id);
                }
            }
        }
        private void Button_RemoveNavigationLink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            for (int i = 0; i < mSelectedActors.Count - 1; i++)
            {
                var actor1 = mSelectedActors[i] as CCore.World.NavigationPointActor;
                var pos1 = mSelectedActors[i].Placement.GetLocation();
                for (int j = i + 1; j < mSelectedActors.Count; j++)
                {
                    var actor2 = mSelectedActors[j] as CCore.World.NavigationPointActor;
                    if (actor1 == null || actor2 == null)
                        continue;

                    var pos2 = mSelectedActors[j].Placement.GetLocation();

                    actor1.RemoveLink(actor2);
                    actor2.RemoveLink(actor1);

                    CCore.Navigation.Navigation.Instance.NavigationPointData.RemoveNavigationLink(actor1.Id, actor2.Id);
                }
            }
        }

        private void Button_AutoGenerateNavigationPointLink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // 遍历所有路点，连接中间能够连接的路点
            var actors = mNavigationPointActors.Values.ToArray();
            for (int i = 0; i < actors.Length - 1; i++)
            {
                var pos1 = actors[i].Placement.GetLocation();
                for (int j = i + 1; j < actors.Length; j++)
                {
                    var pos2 = actors[j].Placement.GetLocation();
                    if (CCore.Navigation.Navigation.Instance.HasBarrier(Guid.Empty, pos1.X, pos1.Z, pos2.X, pos2.Z))
                    {
                        actors[i].RemoveLink(actors[j]);
                        actors[j].RemoveLink(actors[i]);

                        CCore.Navigation.Navigation.Instance.NavigationPointData.RemoveNavigationLink(actors[i].Id, actors[j].Id);
                    }
                    else
                    {
                        actors[i].AddLink(actors[j]);
                        actors[j].AddLink(actors[i]);

                        CCore.Navigation.Navigation.Instance.NavigationPointData.AddNavigationLink(actors[i].Id, actors[j].Id);
                    }
                }
            }
        }

        CCore.World.NavigationPointActor mLastSelectedActor = null;
        List<CCore.World.Actor> mSelectedActors = new List<CCore.World.Actor>();
        public void SelectedActors(List<CCore.World.Actor> actors)
        {
            mSelectedActors.Clear();
            mSelectedActors.AddRange(actors);

            if (actors == null || actors.Count == 0)
            {
                mLastSelectedActor = null;
                return;
            }

            mLastSelectedActor = actors[actors.Count - 1] as CCore.World.NavigationPointActor;
        }

        public void UnSelectActors(List<CCore.World.Actor> actors)
        {
            foreach(var actor in actors)
            {
                mSelectedActors.Remove(actor);
            }

            if (mSelectedActors.Count > 0)
                mLastSelectedActor = mSelectedActors[mSelectedActors.Count - 1] as CCore.World.NavigationPointActor;
            else
                mLastSelectedActor = null;
        }

        public CCore.World.Actor AddNavigationPoint(SlimDX.Vector3 pos)
        {
            var navActor = new CCore.World.NavigationPointActor();
            navActor.ParticipationLineCheck = false;
            var actorInit = new CCore.World.NavigationPointActorInit();
            navActor.Initialize(actorInit);
            navActor.ActorName = "路点" + mNavigationPointActors.Count;
            navActor.Placement.SetLocation(ref pos);

            mNavigationPointActors[navActor.Id] = navActor;
            CCore.Navigation.Navigation.Instance.NavigationPointData.AddNavigationPoint(navActor.Id, pos.X, pos.Y, pos.Z);

            if (AutoLinkLastSelectedPoint && mLastSelectedActor != null && mLastSelectedActor.World != null)
            {
                navActor.AddLink(mLastSelectedActor);
                mLastSelectedActor.AddLink(navActor);

                CCore.Navigation.Navigation.Instance.NavigationPointData.AddNavigationLink(mLastSelectedActor.Id, navActor.Id);
            }

            CCore.Client.MainWorldInstance.AddEditorActor(navActor);
            EditorCommon.GameActorOperation.SelectActors(new List<CCore.World.Actor>() { navActor });

            return navActor;
        }

        private void NavigationPanel_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //InitializeNavigationPointsFromData();
        }

        #endregion

#region CreateNavigation

        CSUtility.Navigation.NavigationInfoOperator mCreateInfo = new CSUtility.Navigation.NavigationInfoOperator();
        private void Button_CreateNavigation_Click(object sender, RoutedEventArgs e)
        {
            if (EditorCommon.MessageBox.Show("创建寻路会清除原有的寻路数据，是否继续？", "警告", EditorCommon.MessageBox.enMessageBoxButton.YesNo) != EditorCommon.MessageBox.enMessageBoxResult.Yes)
                return;

            CCore.Navigation.Navigation.Instance.Info = mCreateInfo.OpInfo;
            var absFolder = CCore.Client.MainWorldInstance.GetWorldLastLoadedAbsFolder("场景");
            CCore.Client.MainWorldInstance.Initialize(absFolder, "寻路");

            //CSUtility.Navigation.NavigationInfo info;// = new CSUtility.Navigation.INavigationInfo();
            //CCore.Navigation.Navigation.Instance.NavigationData.GetNavigationInfo(out info);
            var info = CCore.Navigation.Navigation.Instance.Info;
            CCore.Navigation.NavigationAssist.Instance.InitializeNavigationProxy(ref info);
            CCore.Navigation.NavigationAssist.Instance.BuildNavigationFromData(CCore.Navigation.Navigation.Instance.NavigationData);
            CCore.Navigation.NavigationAssist.Instance.Initialized = true;
        }

#endregion
    }
}
