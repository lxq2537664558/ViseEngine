using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace MeshTemplateEditor
{
    public class SocketShowData : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public CCore.World.Actor ShowActor;
        string mSocketName = "";
        public string SocketName
        {
            get { return mSocketName; }
            set
            {
                mSocketName = value;
                OnPropertyChanged("SocketName");
            }
        }

        string mHighLightString = "";
        public string HighLightString
        {
            get { return mHighLightString; }
            set
            {
                mHighLightString = value;
                OnPropertyChanged("HighLightString");
            }
        }


        //public MidLayer.ISocket Socket;
        public override string ToString()
        {
            //if(Socket != null)
            //    return Socket.Name;

            //return "Unknow";
            return SocketName;
        }
    }

    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "MeshTemplateEditor")]
    //[EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/MeshTemplateEditor")]
    [Guid("975A8C56-40B8-4EA0-8052-1C8C7D657FE8")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public string PluginName
        {
            get { return "模型模板编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "MeshTemplateEditor",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            D3DViewer.Activated = true;
            return true;
        }
        public bool OnDeactive()
        {
            D3DViewer.Activated = false;
            return true;
        }

        // 判断当前控件是否完全加载完毕
        MeshTemplateResourceInfo mCurrentMeshTemplateResInfo;
        public void SetObjectToEdit(object[] obj)
        {
            if (obj == null)
                return;
            if (obj.Length == 0)
                return;

            try
            {
                mCurrentMeshTemplateResInfo = obj[0] as MeshTemplateResourceInfo;
                SetMeshTemplate(mCurrentMeshTemplateResInfo.MeshTemplate);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
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
            if (D3DViewer.IsInitialized && D3DViewer.CameraController != null)
            {
                int idx = 0;
                foreach (var data in mSocketShowDataList)
                {
                    var socket = mCurMesh.SocketTable.GetSocket(idx);
                    if (socket == null)
                        continue;

                    var mat = socket.AbsMatrix;

                    SlimDX.Vector3 pos, scale;
                    SlimDX.Quaternion quat;
                    mat.Decompose(out scale, out quat, out pos);
                    SlimDX.Matrix actMat = mCurActor.Placement.mMatrix;
                    pos = SlimDX.Vector3.TransformCoordinate(pos, actMat);
                    var sinSize = D3DViewer.CameraController.Camera.GetScreenSizeInWorld(pos, mSocketViewSize);
                    if (data == CurSocketShowData)
                    {
                        sinSize *= 1.5f;
                    }
                    var unitMat = SlimDX.Matrix.Transformation(SlimDX.Vector3.Zero, SlimDX.Quaternion.Identity,
                                                           SlimDX.Vector3.UnitXYZ * sinSize,
                                                           SlimDX.Vector3.Zero, quat,
                                                           pos);

                    data.ShowActor.Placement.SetMatrix(ref unitMat);

                    idx++;
                }
            }

            /*InitializeD3DShow();

            if(mD3DShowPlugin != null)
                mD3DShowPlugin.Tick();*/

            D3DViewer.Tick();
        }

#region D3DPreView

        //[Import("D3DViewer_RemoveActor", typeof(Action<CCore.World.Actor>))]
        //public Action<CCore.World.Actor> D3DViewer_RemoveActor_Delegate { get; set; }

        ////[Import("D3DViewer_GetCameraController", typeof(Func<CCore.Camera.CameraController>))]
        ////public Func<CCore.Camera.CameraController> D3DViewer_GetCameraController_Delegate { get; set; }

        //[Import("D3DShow", AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]
        //EditorCommon.PluginAssist.IEditorPlugin mD3DShowPlugin = null;
        //public EditorCommon.PluginAssist.IEditorPlugin D3DShowPlugin
        //{
        //    get { return mD3DShowPlugin; }
        //}

        //bool d3dShowInited = false;
        //void InitializeD3DShow(bool force = false)
        //{
        //    if (d3dShowInited && !force)
        //        return;

        //    if (mD3DShowPlugin == null)
        //        return;

        //    /*if (mD3DShowPlugin is FrameworkElement)
        //    {
        //        Grid_Preview.Children.Clear();
        //        Grid_Preview.Children.Add(mD3DShowPlugin as FrameworkElement);
        //    }*/

        //    d3dShowInited = true;
        //}

#endregion

        Guid mCurMeshTemplateId = Guid.Empty;
//        SourceControl mCurEditSourceControl = null;
        CCore.Mesh.MeshTemplate mCurMeshTemplate = null;
        public CCore.Mesh.MeshTemplate CurMeshTemplate
        {
            get { return mCurMeshTemplate; }
        }
        CCore.Mesh.Mesh mCurMesh = null;
        public CCore.Mesh.Mesh CurMesh
        {
            get { return mCurMesh; }
        }
        CCore.World.Actor mCurActor = null;
        List<SocketShowData> mSocketShowDataList = new List<SocketShowData>();
        public SocketShowData CurSocketShowData
        {
            get
            {
                return ListBox_SocketList.SelectedItem as SocketShowData;
            }
        }
        Guid mNormalSocketTechId = CSUtility.Support.IFileConfig.SocketHandleTechnique_Normal;
        Guid mSelectedSocketTechId = CSUtility.Support.IFileConfig.SocketHandleTechnique_Selected;
        float mSocketViewSize = 0.003f;
        
        bool mAnimationPlaying = true;
        public bool AnimationPlaying
        {
            get { return mAnimationPlaying; }
            set
            {
                mAnimationPlaying = value;

                if (mCurMesh != null)
                {
                    var anim = mCurMesh.GetAnimTree();
                    if (anim != null)
                    {
                        if (anim.GetPause() == mAnimationPlaying)
                            anim.SetPause(!mAnimationPlaying);
                    }
                }
            }
        }

        bool mMeshVisible = true;
        public bool MeshVisible
        {
            get { return mMeshVisible; }
            set
            {
                mMeshVisible = value;

                if (mCurMesh != null)
                {
                    mCurMesh.Visible = mMeshVisible;
                }
            }
        }

        bool mSocketVisible = true;
        public bool SocketVisible
        {
            get { return mSocketVisible; }
            set
            {
                mSocketVisible = value;

                foreach (var data in mSocketShowDataList)
                {
                    data.ShowActor.Visible = mSocketVisible;
                }
            }
        }

        string mSocketFilterString = "";
        public string SocketFilterString
        {
            get { return mSocketFilterString; }
            set
            {
                mSocketFilterString = value;

                if(string.IsNullOrEmpty(mSocketFilterString))
                {
                    ListBox_SocketList.ItemsSource = mSocketShowDataList;
                    foreach (var data in mSocketShowDataList)
                    {
                        data.HighLightString = "";
                    }
                }
                else
                {
                    var showList = new List<SocketShowData>();
                    foreach(var data in mSocketShowDataList)
                    {
                        if (data.SocketName.IndexOf(mSocketFilterString, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            data.HighLightString = "";
                            continue;
                        }

                        data.HighLightString = mSocketFilterString;
                        showList.Add(data);
                    }

                    ListBox_SocketList.ItemsSource = showList;
                }

                OnPropertyChanged("SocketFilterString");
            }
        }

        MeshInfo mInfoControl = new MeshInfo();

        public MainControl()
        {
            InitializeComponent();

            Grid_Main.Children.Remove(ToolBar_Main);
            D3DViewer.AddToolBar(ToolBar_Main);

            SocketMgrCtrl.OnSelectedChanged = OnSelectedEditSocketChanged;

            var template = this.TryFindResource("MeshInitControl") as DataTemplate;
            WPG.Program.RegisterDataTemplate("MeshInit", template);
            template = this.TryFindResource("TechniqueSetControl") as DataTemplate;
            WPG.Program.RegisterDataTemplate("TechniqueSet", template);
            template = this.TryFindResource("MeshSocketSetter") as DataTemplate;
            WPG.Program.RegisterDataTemplate("MeshSocketSetter", template);

            D3DViewer.AddInfoControl(mInfoControl);
        }

        private void SetMeshTemplate(CCore.Mesh.MeshTemplate meshTemplate)
        {
            var isDirty = meshTemplate.IsDirty;

            if (meshTemplate == null)
                return;

            if (mCurMeshTemplate != null)
                mCurMeshTemplate._OnPropertyChanged -= OnMeshTemplatePropertyChanged;
            mCurMeshTemplate = meshTemplate;
            mCurMeshTemplate._OnPropertyChanged += OnMeshTemplatePropertyChanged;

            ProGrid.Instance = meshTemplate;

            foreach (var data in mSocketShowDataList)
            {
                D3DViewer.RemoveActorFromWorld(data.ShowActor);
                //if (D3DViewer_RemoveActor_Delegate != null)
                //    D3DViewer_RemoveActor_Delegate(data.ShowActor);
            }
            mSocketShowDataList.Clear();
            ListBox_SocketList.ItemsSource = null;

            var mshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = meshTemplate.MeshID,
                CanHitProxy = false
            };
            mCurMesh = new CCore.Mesh.Mesh();
            mCurMesh.Initialize(mshInit, null);

            var atInit = new CCore.World.MeshActorInit();
            mCurActor = new CCore.World.MeshActor();
            mCurActor.Initialize(atInit);
            mCurActor.SetPlacement(new CSUtility.Component.StandardPlacement(mCurActor));
            mCurActor.mUpdateAnimByDistance = false;
            mCurActor.Visual = mCurMesh;

            D3DViewer.SetObjectToEdit(new object[] { new object[]{ "Actor", false }, 
                                                         new object[]{ mCurActor }});

            // 添加socket
            if (mCurMesh != null && mCurMesh.SocketTable != null)
            {
                for (int i = 0; i < mCurMesh.SocketTable.GetSocketCount(); i++)
                {
                    var socket = mCurMesh.SocketTable.GetSocket(i);

                    var actInit = new CCore.World.ActorInit();
                    var showActor = new CCore.World.Actor();
                    showActor.Initialize(actInit);
                    var meshInit = new CCore.Mesh.MeshInit();
                    var meshInitPart = new CCore.Mesh.MeshInitPart();
                    meshInitPart.MeshName = CSUtility.Support.IFileConfig.SocketHandleMesh;
                    meshInitPart.Techs.Add(mNormalSocketTechId);
                    meshInit.MeshInitParts.Add(meshInitPart);
                    var mesh = new CCore.Mesh.Mesh();
                    mesh.Initialize(meshInit, showActor);
                    mesh.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(showActor.Id));
                    mesh.Layer = CCore.RLayer.RL_PostTranslucent;
                    showActor.Visual = mesh;
                    showActor.SetPlacement(new CSUtility.Component.StandardPlacement(showActor));
                    var mat = socket.AbsMatrix;
                    showActor.Placement.SetMatrix(ref mat);

                    showActor.OnSelected += OnSelectedSocketActor;

                    D3DViewer.SetObjectToEdit(new object[] { new object[]{ "Actor", true }, 
                                                                 new object[]{ showActor }});

                    SocketShowData data = new SocketShowData();
                    data.ShowActor = showActor;
                    //data.Socket = socket;
                    data.SocketName = socket.Name;

                    mSocketShowDataList.Add(data);

                    //////D3DViewer.AddFreezeSizeInScreenActor(
                    //////    new D3DViewer.D3DViewerControl.FreezeSizeInScreenData()
                    //////    {
                    //////        Actor = showActor,
                    //////        ScreenSize = 0.003f
                    //////    });
                }
                ListBox_SocketList.ItemsSource = mSocketShowDataList;

                var anim = mCurMesh.GetAnimTree();
                if (anim != null)
                {
                    AnimationPlaying = !anim.GetPause();
                }
            }

            SocketMgrCtrl.Initialize(this);

            ListBox_SocketList.SelectedIndex = -1;

            meshTemplate.IsDirty = isDirty;

            mInfoControl.UpdateMeshInfo(CurMesh);
        }

        //public void OnSelectedMeshTemplateControl(SourceControl sCtrl)
        //{
        //    if (sCtrl.SourceInfo.MeshTemplateGUID == mCurMeshTemplateId)
        //        return;

        //    mCurEditSourceControl = sCtrl;

        //    var meshTemplate = sCtrl.SourceObj as CCore.Mesh.MeshTemplate;
        //    if (meshTemplate == null)
        //        return;

        //    mCurMeshTemplateId = sCtrl.SourceInfo.MeshTemplateGUID;

        //    SetMeshTemplate(meshTemplate);
        //}

        private void Menu_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mCurrentMeshTemplateResInfo == null)
                return;

            Program.SaveMeshTemplate(mCurrentMeshTemplateResInfo);
        }

        private void ListBox_SocketList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {            
            foreach (SocketShowData item in e.RemovedItems)
            {
                var mesh = item.ShowActor.Visual as CCore.Mesh.Mesh;
                var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(mNormalSocketTechId);
                mesh.SetMaterial(0, 0, mtl);
            }
            foreach (SocketShowData item in e.AddedItems)
            {
                var mesh = item.ShowActor.Visual as CCore.Mesh.Mesh;
                var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(mSelectedSocketTechId);
                mesh.SetMaterial(0, 0, mtl);
            }

            if (e.AddedItems.Count != 0)
            {
                var data = e.AddedItems[e.AddedItems.Count - 1] as SocketShowData;
                EditorCommon.PluginAssist.PropertyGridAssist.SetSelectedObjectData("MeshSocket", new object[] { data.SocketName });
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //D3DEvrCtrl.World.AddActor(mSocketShowActor);
            //D3DEvrCtrl.OnMouseSelectedActor = OnMouseSelectedActor;
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void OnSelectedSocketActor(CCore.World.Actor actor, bool selected)
        {
            if(selected)
            {
                foreach (var data in mSocketShowDataList)
                {
                    if (data.ShowActor == actor)
                    {
                        ListBox_SocketList.SelectedItem = data;
                    }
                }
            }
        }

        // 选中创建的Socket挂接对象
        private void OnSelectedEditSocketChanged(string socketName)
        {
            foreach (var data in mSocketShowDataList)
            {
                if (data.SocketName == socketName)
                {
                    ListBox_SocketList.SelectedItem = data;
                    return;
                }
            }

            ListBox_SocketList.SelectedIndex = -1;
        }

        private void OnMeshTemplatePropertyChanged(string propertyName)
        {
            if (mCurMeshTemplate == null)
                return;

            var animNode = mCurMesh.GetAnimTree() as CCore.AnimTree.AnimTreeNode_Action;

            switch (propertyName)
            {
                case "ActionName":
                    {
                        if (mCurMesh != null)
                        {
                            var nAnimNode = new CCore.AnimTree.AnimTreeNode_Action();
                            nAnimNode.Initialize();
                            nAnimNode.ActionName = mCurMeshTemplate.ActionName;
                            //if (mVisual is CCore.Mesh.Mesh)
                            mCurMesh.SetAnimTree(nAnimNode);
                            //else if (mVisual is FrameSet.Role.RoleActorVisual)
                            //    ((FrameSet.Role.RoleActorVisual)mVisual).SetAnimTree(retAnimNode);
                            nAnimNode.SetPause(!AnimationPlaying);
                        }
                    }
                    break;

                case "PlayRate":
                    {
                        if (animNode != null)
                        {
                            animNode.PlayRate = mCurMeshTemplate.PlayRate;
                        }
                    }
                    break;
                //case "Pause":
                //    {
                //        if(animNode != null)
                //        {
                //            animNode.SetPause(mCurMeshTemplate.Pause);
                //        }
                //    }
                //    break;
                case "ActionPercent":
                    {
                        if (animNode != null)
                        {
                            animNode.CurAnimTime = (long)(animNode.Duration * mCurMeshTemplate.ActionPercent);
                        }
                    }
                    break;
                //case "Loop":
                //    {
                //        if(animNode != null)
                //        {
                //            animNode.SetLoop(mCurMeshTemplate.Loop);
                //        }
                //    }
                //    break;
                case "MeshInitList":
                    {
                        SetMeshTemplate(mCurMeshTemplate);
                    }
                    break;
                case "Scale":
                    {
                        if (mCurActor != null)
                        {
                            SlimDX.Vector3 vScale = new SlimDX.Vector3(mCurMeshTemplate.Scale);
                            mCurActor.Placement.SetScale(ref vScale);
                        }
                    }
                    break;
            }
        }

        #region SocketListDrag

        bool mMouseDown = false;
        Point mMouseDownPt;
        private void SocketListItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                mMouseDown = true;
                mMouseDownPt = e.GetPosition(this);
            }
        }

        private void SocketListItem_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && mMouseDown)
            {
                var pt = e.GetPosition(this);
                var length = (pt.X - mMouseDownPt.X) * (pt.X - mMouseDownPt.X) + (pt.Y - mMouseDownPt.Y) * (pt.Y - mMouseDownPt.Y);
                if(length > 10)
                {
                    EditorCommon.DragDrop.DragDropManager.Instance.StartDrag(Program.SocketDragType, new EditorCommon.DragDrop.IDragAbleObject[] { sender as EditorCommon.DragDrop.IDragAbleObject });
                    mMouseDown = false;
                }
            }
        }

        #endregion
    }
}
