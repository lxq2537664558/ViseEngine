using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace ScenePointEditor
{
    /// <summary>
    /// Interaction logic for ScenePointMgrPanel.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "ScenePointEditor")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/ScenePointEditor")]
    [Guid("354C866C-6BA0-4200-A55C-8FAD09665819")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class ScenePointMgrPanel : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
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
            get { return "ScenePointEditor"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "ScenePointEditor",
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

        //////////////////////////////////////////////////////////////

        public delegate void Delegate_OnSelectedActor(CCore.World.Actor actor, bool bMultiSelected);
        public Delegate_OnSelectedActor OnSelectedActor;
        public delegate void Delegate_OnPlantScenePoint();
        public Delegate_OnPlantScenePoint OnPlantScenePoint;

        public delegate void Delegate_OnTellServerUpdateScenePointGroup(Guid mapId, Guid groupId);
        public Delegate_OnTellServerUpdateScenePointGroup OnTellServerUpdateScenePointGroup;
        
        enum enOperationType
        { 
            Normal,
            Deleteing,
            SelectingFromWorld,
            SelectingFromMySelf,
        }
        enOperationType mOperationType = enOperationType.Normal;

        //protected CCore.World.MeshActor mScenePointActor;

        //static ScenePointMgrPanel smInstance = null;
        //public static ScenePointMgrPanel Instance
        //{
        //    get
        //    {
        //        if (smInstance == null)
        //            smInstance = new ScenePointMgrPanel();
        //        return smInstance;
        //    }
        //}
        EditorCommon.Hotkey.HotkeyGroup mCurrentActiveGroup = null;
        protected CCore.World.ScenePointActor mScenePointActor;
        private void AddActiveOperate()
        {
            mCurrentActiveGroup = EditorCommon.Hotkey.HotkeyManager.Instance.CurrentActiveGroup;
            EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup("场景点编辑操作");

            //mTempScenePointActor = new CCore.World.ScenePointActor(new CSUtility.Map.ScenePoint());
            //var mTempScenePointInit = new CCore.World.ScenePointActorInit();
            //mTempScenePointActor.Initialize(mTempScenePointInit);




            mScenePointActor = new CCore.World.ScenePointActor(new CSUtility.Map.ScenePoint());
            var scenePointActorInit = new CCore.World.ScenePointActorInit();
            mScenePointActor.Initialize(scenePointActorInit);

            //var mesh = new CCore.Mesh.Mesh();
            //var meshInit = new CCore.Mesh.MeshInit();
            //meshInit.MeshTemplateID = CSUtility.Support.IFileConfig.ScenePointMesh;
            //mesh.Initialize(meshInit, mScenePointActor);
            //mScenePointActor.Visual = mesh;
            //mScenePointActor.Visible = true;
            //mScenePointActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient);
            //mScenePointActor.SetPlacement(new CSUtility.Component.StandardPlacement(mScenePointActor));
            //mScenePointActor.Placement.SetLocation(new SlimDX.Vector3(0, 0, 0));
            CCore.Engine.Instance.Client.MainWorld.AddEditorActor(mScenePointActor);
        }

        private void RemoveActiveOperate()
        {
            if (mCurrentActiveGroup != null)
                EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup(mCurrentActiveGroup.GroupName);
            EditorCommon.WorldEditMode.Instance.RestoreEditMode();

            if (mScenePointActor != null)
                CCore.Engine.Instance.Client.MainWorld.RemoveEditorActor(mScenePointActor);
        }

        bool mAddPointMode = false;
        public bool AddPointMode
        {
            get { return mAddPointMode; }
            set
            {
                if (mAddPointMode == value)
                    return;

                mAddPointMode = value;

                if (mAddPointMode)
                {
                    RemoveActiveOperate();
                    mInsertPointMode = false;

                    if (OnPlantScenePoint != null)
                        OnPlantScenePoint();
                    AddActiveOperate();
                }
                else
                {
                    RemoveActiveOperate();
                }
                OnPropertyChanged("AddPointMode");
            }
        }

        bool mInsertPointMode = false;
        public bool InsertPointMode
        {
            get { return mInsertPointMode; }
            set
            {
                if (value && ListBox_Points.SelectedIndex < 0)
                {
                    EditorCommon.MessageBox.Show("请先选择一个点再做插入操作!");
                    return;
                }

                mInsertPointMode = value;

                if (mInsertPointMode)
                {
                    mAddPointMode = false;
                    RemoveActiveOperate();
                    
                    if (OnPlantScenePoint != null)
                        OnPlantScenePoint();

                    AddActiveOperate();
                }
                else
                {
                    RemoveActiveOperate();
                }
                OnPropertyChanged("InsertPointMode");
            }
        }

        string mCountString = "0个点";
        public string CountString
        {
            get { return mCountString; }
            set
            {
                mCountString = value;
                OnPropertyChanged("CountString");
            }
        }

        CSUtility.Map.ScenePointGroup mCurrentGroup;
        public CSUtility.Map.ScenePointGroup CurrentGroup
        {
            get { return mCurrentGroup; }
        }

        string mSearchString = "";
        public string SearchString
        {
            get { return mSearchString; }
            set
            {
                mSearchString = value;

                if (string.IsNullOrEmpty(mSearchString))
                {
                    foreach (ScenePointGroupItem item in ListBox_Groups.Items)
                    {
                        item.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    var tempTex = mSearchString.ToLower();
                    foreach (ScenePointGroupItem item in ListBox_Groups.Items)
                    {
                        if (item.GroupShowName.ToLower().Contains(tempTex))
                            item.Visibility = Visibility.Visible;
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                }

                OnPropertyChanged("SearchString");
            }
        }

        class ShowData
        {
            public CCore.World.ScenePointActor Actor;
            public TextBlock Ctrl;
            public CSUtility.Map.ScenePoint Pt;
        }

        Dictionary<Guid, ShowData> mShowDatas = new Dictionary<Guid, ShowData>();
        CCore.World.ScenePointGroupActor mGroupActor = null;
        Guid mCurrentMapId = Guid.Empty;

        public ScenePointMgrPanel()
        {
            InitializeComponent();
            CSUtility.Map.ScenePointGroupManager.Instance.OnCurrentMapChanged += _OnCurrentMapChanged;
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("场景点编辑操作", "场景点编辑_显示场景点", "刷新场景点的坐标", CCore.MsgProc.BehaviorParameter.enKeys.None, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, null, OnMouseMove);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("场景点编辑操作", "场景点编辑_增加场景点", "在地面上点击增加场景点", CCore.MsgProc.BehaviorParameter.enKeys.LButton, false, false, false, EditorCommon.Hotkey.enMouseType.None, null, OnLBMouseDown);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("场景点编辑操作", "转动摄像机", "转动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.LButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.RotateCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("场景点编辑操作", "按水平面移动摄像机", "按照水平面移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.MButton, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.HorizontalMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("场景点编辑操作", "按视平面移动摄像机", "按照视平面移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.MButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.ScreenMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("场景点编辑操作", "轴向移动摄像机", "沿摄像机方向移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.RButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.DirMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("场景点编辑操作", "轴向移动摄像机 ", "沿摄像机方向移动场景中的视口摄像机", CCore.MsgProc.BehaviorParameter.enKeys.None, false, false, false, EditorCommon.Hotkey.enMouseType.MouseWheel, null, EditorCommon.GameCameraOperation.DirMoveCamera2);

            CCore.Engine.Instance.Client.MainWorld.OnRemoveActor += MainWorld_OnRemoveActor;
        }
        public void MainWorld_OnRemoveActor(CCore.World.Actor act, CCore.World.World world)
        {
            if (!mShowDatas.ContainsKey(act.Id))
                return;

            ListBox_Points.SelectedItems.Add(mShowDatas[act.Id].Ctrl);
            RemoveSelectedPoints();
        }

        ~ScenePointMgrPanel()
        {
            if (mCurrentGroup != null)
                mCurrentGroup.OnPointsChanged -= _OnPointsChanged;
        }

        public void OnMouseMove(CCore.MsgProc.BehaviorParameter param, object obj)
        {
            CCore.MsgProc.Behavior.Mouse_Move parameter = param as CCore.MsgProc.Behavior.Mouse_Move;
            SlimDX.Vector3 end = EditorCommon.Assist.Assist.IntersectWithWorld(parameter.X, parameter.Y, CCore.Engine.Instance.Client.MainWorld, EditorCommon.Program.GameREnviroment.Camera, (UInt32)CSUtility.enHitFlag.HitMeshTriangle);
            mScenePointActor.SetPlacement(new CSUtility.Component.StandardPlacement(mScenePointActor));
            mScenePointActor.Placement.SetLocation(ref end);
        }

        public void OnLBMouseDown(CCore.MsgProc.BehaviorParameter param, object obj)
        {
            if (mCurrentGroup == null)
                return;
            CCore.MsgProc.Behavior.Mouse_Key parameter = param as CCore.MsgProc.Behavior.Mouse_Key;
            SlimDX.Vector3 end = EditorCommon.Assist.Assist.IntersectWithWorld(parameter.X, parameter.Y, CCore.Engine.Instance.Client.MainWorld, EditorCommon.Program.GameREnviroment.Camera, (UInt32)CSUtility.enHitFlag.HitMeshTriangle);
            var point = new CSUtility.Map.ScenePoint();
            AddPoint(end);
        }

        public void _OnCurrentMapChanged(Guid mapId)
        {
            mCurrentMapId = mapId;

            ListBox_Groups.Items.Clear();

            var groups = CSUtility.Map.ScenePointGroupManager.Instance.LoadAllGroups(mapId);
            foreach (var group in groups)
            {
                if (group is CSUtility.Map.ScenePointGroup_Camera)
                    continue;

                ScenePointGroupItem item = new ScenePointGroupItem(group);
                ListBox_Groups.Items.Add(item);
            }
        }

        private void Button_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var savedValues = CSUtility.Map.ScenePointGroupManager.Instance.SaveDirtyGroups();

            // 通知服务器更新场景点
            foreach (var value in savedValues)
            {
#warning 通知服务器更新场景点
                //SvnInterface.Commander.Instance.AddMethodToCommanderPipe(this, "_OnTellServerUpdateScenePointGroup", new object[] { value.Key, value.Value });
            }
        }

        public void _OnTellServerUpdateScenePointGroup(Guid mapId, Guid groupId)
        {
            if (OnTellServerUpdateScenePointGroup != null)
                OnTellServerUpdateScenePointGroup(mapId, groupId);
        }

        private void Button_AddGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var group = new CSUtility.Map.ScenePointGroup();
            CSUtility.Map.ScenePointGroupManager.Instance.AddGroup(group, mCurrentMapId);
            ScenePointGroupItem item = new ScenePointGroupItem(group);
            ListBox_Groups.Items.Add(item);
            ListBox_Groups.SelectedItem = item;
        }

        private void Button_DelGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mCurrentGroup == null)
                return;

            if (ListBox_Groups.SelectedItem == null)
                return;

            if (EditorCommon.MessageBox.Show("即将删除点组" + mCurrentGroup.Name + ", 是否确定？", "警告", EditorCommon.MessageBox.enMessageBoxButton.OKCancel) != EditorCommon.MessageBox.enMessageBoxResult.OK)
            {
                return;
            }

            if (mCurrentGroup != null)
            {
                mCurrentGroup.OnPointsChanged -= _OnPointsChanged;
                mCurrentGroup = null;
            }

            ProGrid_Group.Instance = null;

            // 显示连线
            if (mGroupActor != null)
            {
                CCore.Engine.Instance.Client.MainWorld.RemoveEditorActor(mGroupActor);
                mGroupActor = null;
            }

            var item = ListBox_Groups.SelectedItem as ScenePointGroupItem;
            CSUtility.Map.ScenePointGroupManager.Instance.DelGroup(mCurrentMapId, item.LinkedGroup.Id);
            ListBox_Groups.Items.Remove(item);
            ListBox_Points.Items.Clear();
        }
        public CCore.World.Actor AddPoint(SlimDX.Vector3 pos)
        {
            if (CurrentGroup == null)
                return null;

            if (AddPointMode)
            {
                var pt = CurrentGroup.AddScenePoint(pos);
            }
            else if (InsertPointMode)
            {
                if (ListBox_Points.SelectedIndex < 0)
                { 
                    EditorCommon.MessageBox.Show("请先选择一个点再做插入操作!");
                }
                CurrentGroup.InsertScenePoint(ListBox_Points.SelectedIndex, pos);
            }

            //var spActor = new CCore.World.ScenePointActor(pt);
            //var spaInit = new FrameSet.Scene.ScenePointActorInit();
            //spActor.Initialize(spaInit);

            //mShowDatas[spActor.Id] = spActor;

            //CCore.Engine.Instance.MainWorld.AddActor(spActor);

            return null;
        }

        private void Button_DelPoint_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveSelectedPoints();
        }

        private void panelBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _OnCurrentMapChanged(CCore.Engine.Instance.Client.MainWorld.WorldInit.WorldId);
        }

        private void ListBox_Groups_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = ListBox_Groups.SelectedItem as ScenePointGroupItem;
            if (item == null)
            {
                CCore.Program.SetActorTypeShow(CCore.Engine.Instance.Client.MainWorld, CCore.Program.ScenePointTypeName, false);
                //CCore.Client.MainWorldInstance.SetActorGameTypeShow((UInt16)(CSUtility.Component.EActorGameType.ScenePoint), false);
                return;
            }
            CCore.Program.SetActorTypeShow(CCore.Engine.Instance.Client.MainWorld, CCore.Program.ScenePointTypeName, true);
            //CCore.Client.MainWorldInstance.SetActorGameTypeShow((UInt16)(CSUtility.Component.EActorGameType.ScenePoint), true);

            if (mCurrentGroup != null)
            {
                mCurrentGroup.OnPointsChanged -= _OnPointsChanged;
            }

            mCurrentGroup = item.LinkedGroup;
            ProGrid_Group.Instance = mCurrentGroup;

            if (mGroupActor != null)
            {
                CCore.Engine.Instance.Client.MainWorld.RemoveEditorActor(mGroupActor);

                //CCore.Engine.Instance.Client.MainWorld.RemoveEditorActor(mGroupActor);
            }

            if (mCurrentGroup == null)
                return;

            mCurrentGroup.OnPointsChanged += _OnPointsChanged;

            // 显示连线
            WPG.Data.EditorContext.SelectedScenePointGroup = mCurrentGroup;

            mGroupActor = new CCore.World.ScenePointGroupActor(mCurrentGroup);
            var actInit = new CCore.World.ScenePointGroupActorInit();
            mGroupActor.Initialize(actInit);
            CCore.Engine.Instance.Client.MainWorld.AddEditorActor(mGroupActor);
            // 显示点列表
            _OnPointsChanged();
        }

        public void _OnPointsChanged()
        {
            if (mOperationType != enOperationType.Normal)
                return;

            if (mCurrentGroup == null)
                return;

            ListBox_Points.Items.Clear();
            CCore.Engine.Instance.Client.MainWorld.OnRemoveActor -= MainWorld_OnRemoveActor;
            foreach (var data in mShowDatas.Values)
            {
                CCore.Engine.Instance.Client.MainWorld.RemoveActor(data.Actor);
            }
            mShowDatas.Clear();

            for (int i = 0; i < mCurrentGroup.Points.Count; i++)
            {
                var textBlock = new TextBlock()
                {
                    Text = "Point" + i
                };
                ListBox_Points.Items.Add(textBlock);

                var spActor = new CCore.World.ScenePointActor(mCurrentGroup.Points[i]);
                var spaInit = new CCore.World.ScenePointActorInit();
                spActor.Initialize(spaInit);
                spActor.ActorName = mCurrentGroup.Name + "_点" + i;

                ShowData data = new ShowData()
                {
                    Actor = spActor,
                    Ctrl = textBlock,
                    Pt = mCurrentGroup.Points[i]
                };
                textBlock.Tag = data;

                mShowDatas[spActor.Id] = data;
                
                CCore.Engine.Instance.Client.MainWorld.AddActor(spActor);
            }
            CCore.Engine.Instance.Client.MainWorld.OnRemoveActor += MainWorld_OnRemoveActor;
            CountString = mCurrentGroup.Points.Count + "个点";
        }

        public void SelectedPoints(List<CCore.World.Actor> actors)
        {
            if (mOperationType != enOperationType.Normal)
                return;

            mOperationType = enOperationType.SelectingFromWorld;

            ListBox_Points.SelectedItems.Clear();

            foreach (var actor in actors)
            {
                if (mShowDatas.ContainsKey(actor.Id))
                {
                    ListBox_Points.SelectedItems.Add(mShowDatas[actor.Id].Ctrl);
                }
            }

            mOperationType = enOperationType.Normal;
        }

        public void RemoveSelectedPoints()
        {
            if (mOperationType != enOperationType.Normal)
                return;

            if (mCurrentGroup == null)
                return;
            
            mOperationType = enOperationType.Deleteing;

            foreach (var item in ListBox_Points.SelectedItems)
            {
                TextBlock text = item as TextBlock;
                var data = text.Tag as ShowData;
                mCurrentGroup.DeleteScenePoint(data.Pt);
            }
            mOperationType = enOperationType.Normal;
            _OnPointsChanged();
        }

        private void ListBox_Points_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (mOperationType != enOperationType.Normal)
                return;

            mOperationType = enOperationType.SelectingFromMySelf;
            CCore.Engine.Instance.Client.MainWorld.OnRemoveActor -= MainWorld_OnRemoveActor;
            if (ListBox_Points.SelectedItems.Count == 0)
            {
                if (OnSelectedActor != null)
                    OnSelectedActor(null, false);
            }
            else
            {
                if (OnSelectedActor != null)
                    OnSelectedActor(null, false);

                bool multiSelected = (ListBox_Points.SelectedItems.Count == 1) ? false : true;

                for (int i = 0; i < ListBox_Points.SelectedItems.Count; i++)
                {
                    var ctrl = ListBox_Points.SelectedItems[i] as TextBlock;
                    var data = ctrl.Tag as ShowData;
                    if (OnSelectedActor != null)
                        OnSelectedActor(data.Actor, multiSelected);
                    
                    CCore.Engine.Instance.Client.MainWorld.RemoveActor(data.Actor);
                    CCore.Engine.Instance.Client.MainWorld.AddActor(data.Actor);                    
                }
            }
            CCore.Engine.Instance.Client.MainWorld.OnRemoveActor += MainWorld_OnRemoveActor;
            mOperationType = enOperationType.Normal;
        }
    }
}
