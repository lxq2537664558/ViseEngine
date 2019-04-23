using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace CameraAnimationEditor
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "CameraAnimationEditor")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/CameraAnimationEditor")]
    [Guid("76884699-57CC-4C61-B944-2FAE54A83EC9")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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
            get { return "CameraAnimationEditor"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "CameraAnimationEditor",
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

        public delegate void Delegate_OnSelectedActor(CCore.World.Actor actor, bool bMultiSelected);
        public Delegate_OnSelectedActor OnSelectedActor;
        public delegate void Delegate_OnPlantScenePoint();
        public Delegate_OnPlantScenePoint OnPlantScenePoint;
        public CSUtility.Map.ScenePointGroup_Camera.Delegate_GetCameraTrans OnGetCameraTrans;

        enum enOperationType
        {
            Normal,
            Deleteing,
            SelectingFromWorld,
            SelectingFromMySelf,
        }
        enOperationType mOperationType = enOperationType.Normal;

        CSUtility.Map.ScenePointGroup_Camera mCurrentGroup = null;
        public CSUtility.Map.ScenePointGroup_Camera CurrentGroup
        {
            get { return mCurrentGroup; }
            protected set
            {
                TimeLineCtrl.RemoveAllTimeLineObject();

                if (mCurrentGroup != null)
                {
                    mCurrentGroup.OnGetCameraTrans = null;
                }

                mCurrentGroup = value;
                mCurrentGroup.OnGetCameraTrans = _OnGetCameraTrans;
                TimeLineCtrl.AddTimeLineObject(mCurrentGroup);
                TimeLineCtrl.SelectedTimeLineObject(0);
            }
        }

        class ShowData
        {
            public CCore.World.ScenePointActor Actor;
            public CSUtility.Map.ScenePoint Pt;
        }
        Dictionary<Guid, ShowData> mShowDatas = new Dictionary<Guid, ShowData>();
        CCore.World.ScenePointGroupActor mGroupActor = null;
        Guid mCurrentMapId = Guid.Empty;

        CCore.Camera.CameraAnimation mCameraAnimation;

        CCore.Camera.CameraController mCamAnimCameraController = new CCore.Camera.MayaPosCameraController();
        public void SetCamera(CCore.Camera.CameraObject eye)
        {
            mCamAnimCameraController.Camera = eye;
        }
        CCore.Camera.CameraController mOldCamAnimCameraControllerStore;
        bool mPreView = false;
        public bool PreView
        {
            get { return mPreView; }
            set
            {
                mPreView = value;

                if (mPreView)
                {
                    mOldCamAnimCameraControllerStore = CCore.Camera.CameraController.CurrentCameraController;
                    CCore.Camera.CameraController.CurrentCameraController = mCamAnimCameraController;
                }
                else
                {
                    CCore.Camera.CameraController.CurrentCameraController = mOldCamAnimCameraControllerStore;
                }

                OnPropertyChanged("PreView");
            }
        }

        bool mCameraObjectVisible = true;
        public bool CameraObjectVisible
        {
            get { return mCameraObjectVisible; }
            set
            {
                mCameraObjectVisible = value;

                if (mGroupActor != null)
                    mGroupActor.Visible = mCameraObjectVisible;

                foreach (var data in mShowDatas.Values)
                {
                    data.Actor.Visible = mCameraObjectVisible;
                }

                OnPropertyChanged("CameraObjectVisible");
            }
        }

        string mSearchText = "";
        public string SearchText
        {
            get { return mSearchText; }
            set
            {
                mSearchText = value;

                if (string.IsNullOrEmpty(mSearchText))
                {
                    foreach (ScenePointGroupItem item in ListBox_Groups.Items)
                    {
                        item.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    var tempTex = mSearchText.ToLower();
                    foreach (ScenePointGroupItem item in ListBox_Groups.Items)
                    {
                        if (item.GroupShowName.ToLower().Contains(tempTex))
                            item.Visibility = Visibility.Visible;
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                }

                OnPropertyChanged("SearchText");
            }
        }
        
        public MainControl()
        {
            InitializeComponent();

            CSUtility.Map.ScenePointGroupManager.Instance.OnCurrentMapChanged += _OnCurrentMapChanged;

            TimeLineCtrl.OnAddFrame = _OnAddFrame;
            TimeLineCtrl.OnCurrentFrameChanged = _OnCurrentFrameChanged;
            TimeLineCtrl.OnUpdateTimeLinkTrackItemActiveShow = _OnUpdateTimeLinkTrackItemActiveShow;
            TimeLineCtrl.OnTimeLineTrackItemSelected = _OnTimeLineTrackItemSelected;
            TimeLineCtrl.PlayLoop = false;
        }

        ~MainControl()
        {
            if (mCurrentGroup != null)
                mCurrentGroup.OnPointsChanged -= _OnPointsChanged;
        }

        private void _OnAddFrame(CSUtility.Animation.TimeLineKeyFrameObjectInterface keyObj)
        {
            _OnPointsChanged();
        }

        private void _OnCurrentFrameChanged(Int64 curFrame)
        {
            if (mCameraAnimation == null)
                return;

            if(TimeLineCtrl.TotalFrame != 0)
                mCameraAnimation.CurAnimTime = TimeLineCtrl.GetFrameMillisecondTime(curFrame);// curFrame * TimeLineCtrl.TotalMilliTime / TimeLineCtrl.TotalFrame;
            else
                mCameraAnimation.CurAnimTime = 0;
            var pos = mCameraAnimation.GetCurrentCameraPosition();
            var dir = mCameraAnimation.GetCurrentCameraDirection();

            mCamAnimCameraController.SetPosDir(ref pos, ref dir);
        }

        private void _OnUpdateTimeLinkTrackItemActiveShow(TimeLine.TimeLineTrackItem item)
        {
        }

        private void _OnTimeLineTrackItemSelected(TimeLine.TimeLineTrackItem item)
        {
            if (mOperationType != enOperationType.Normal)
                return;

            mOperationType = enOperationType.SelectingFromMySelf;

            if (OnSelectedActor != null)
            {
                OnSelectedActor(null, false);

                var camSP = item.KeyFrameItem as CSUtility.Map.ScenePoint_Camera;
                var data = camSP.EditorObject as ShowData;
                if (data != null)
                {
                    OnSelectedActor(data.Actor, false);
                }
            }

            mOperationType = enOperationType.Normal;
        }

        private SlimDX.Matrix _OnGetCameraTrans()
        {
            if (OnGetCameraTrans != null)
                return OnGetCameraTrans();

            return SlimDX.Matrix.Identity;
        }

        public void _OnCurrentMapChanged(Guid mapId)
        {
            mCurrentMapId = mapId;

            ListBox_Groups.Items.Clear();

            var groups = CSUtility.Map.ScenePointGroupManager.Instance.LoadAllGroups(mapId);
            foreach (var group in groups)
            {
                if (!(group is CSUtility.Map.ScenePointGroup_Camera))
                    continue;

                ScenePointGroupItem item = new ScenePointGroupItem(group);
                ListBox_Groups.Items.Add(item);
            }
        }

        private void Button_AddGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var group = new CSUtility.Map.ScenePointGroup_Camera();
            CSUtility.Map.ScenePointGroupManager.Instance.AddGroup(group, mCurrentMapId);
            group.SPGType = CSUtility.Map.ScenePointGroup.enScenePointGroupType.CameraPoint;
            group.LineType = CSUtility.Map.ScenePointGroup.enLineType.Spline;

            if (group == null)
                return;

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

            if (EditorCommon.MessageBox.Show("即将删除点组" + mCurrentGroup.Name + ", 是否确定？", "警告", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
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
                CCore.Client.MainWorldInstance.RemoveActor(mGroupActor);
                mGroupActor = null;
            }

            var item = ListBox_Groups.SelectedItem as ScenePointGroupItem;
            CSUtility.Map.ScenePointGroupManager.Instance.DelGroup(mCurrentMapId, item.LinkedGroup.Id);
            ListBox_Groups.Items.Remove(item);

            TimeLineCtrl.RemoveAllTimeLineObject();
        }

        private void Button_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var savedValues = CSUtility.Map.ScenePointGroupManager.Instance.SaveDirtyGroups();

            // 通知服务器更新场景点
            foreach (var value in savedValues)
            {
#warning SVN上传及通知服务器更新
                //EditorCommon.VersionControl.VersionControlManager.Instance.MethodInvoke()

                //SvnInterface.Commander.Instance.AddMethodToCommanderPipe(this, "_OnTellServerUpdateScenePointGroup", new object[] { value.Key, value.Value });
            }
        }

        private void ListBox_Groups_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = ListBox_Groups.SelectedItem as ScenePointGroupItem;
            if (item == null)
            {
                CCore.Client.MainWorldInstance.SetActorGameTypeShow((UInt16)(CSUtility.Component.EActorGameType.ScenePoint), false);
                return;
            }
            CCore.Client.MainWorldInstance.SetActorGameTypeShow((UInt16)(CSUtility.Component.EActorGameType.ScenePoint), true);

            //if (mCurrentGroup != null)
            //{
            //    mCurrentGroup.OnPointsChanged -= _OnPointsChanged;
            //}

            CurrentGroup = item.LinkedGroup as CSUtility.Map.ScenePointGroup_Camera;
            ProGrid_Group.Instance = mCurrentGroup;

            if (mGroupActor != null)
            {
                CCore.Client.MainWorldInstance.RemoveActor(mGroupActor);
            }

            if (mCurrentGroup == null)
                return;

            //mCurrentGroup.OnPointsChanged += _OnPointsChanged;

            // 显示连线
            WPG.Data.EditorContext.SelectedScenePointGroup = mCurrentGroup;

            mGroupActor = new CCore.World.ScenePointGroupActor(mCurrentGroup);
            var actInit = new CCore.World.ScenePointGroupActorInit();
            mGroupActor.Initialize(actInit);
            CCore.Client.MainWorldInstance.AddActor(mGroupActor);
            mGroupActor.Visible = mCameraObjectVisible;

            mCameraAnimation = new CCore.Camera.CameraAnimation(mCurrentGroup);

            // 显示点列表
            _OnPointsChanged();
        }

        public void _OnPointsChanged()
        {
            if (mOperationType != enOperationType.Normal)
                return;

            if (mCurrentGroup == null)
                return;

            foreach (var data in mShowDatas.Values)
            {
                CCore.Client.MainWorldInstance.RemoveActor(data.Actor);
            }
            mShowDatas.Clear();

            for (int i = 0; i < mCurrentGroup.Points.Count; i++)
            {
                var camSP = mCurrentGroup.Points[i] as CSUtility.Map.ScenePoint_Camera;
                if (camSP == null)
                    continue;

                var spActor = new CCore.World.ScenePointActor(camSP);
                var spaInit = new CCore.World.ScenePointActorInit();
                spActor.Initialize(spaInit);

                ShowData data = new ShowData()
                {
                    Actor = spActor,
                    Pt = camSP
                };

                mShowDatas[spActor.Id] = data;
                camSP.EditorObject = data;

                CCore.Client.MainWorldInstance.AddActor(spActor);
                spActor.Visible = mCameraObjectVisible;
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _OnCurrentMapChanged(CCore.Client.MainWorldInstance.WorldInit.WorldId);
        }
    }
}
