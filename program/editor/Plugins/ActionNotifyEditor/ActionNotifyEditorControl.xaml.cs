using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Threading;

namespace ActionNotifyEditor
{
    /// <summary>
    /// Interaction logic for ActionNotifyEditorControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "ActionNotifyEditor")]
    //[EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/ActionNotifyEditor")]
    [Guid("DA7F1A3C-8DB4-40DD-B074-11E406E51911")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class ActionNotifyEditorControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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
            get { return "动作事件编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }     

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "动作事件编辑器",
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

        public void SetObjectToEdit(object[] obj)
        {
            if (obj == null)
                return;
            if (obj.Length == 0)
                return;

            try
            {
                var actionResInfo = obj[0] as ActionNotifyResourceInfo;
                if (actionResInfo == null)
                    return;
                InitWithActionFullFileName(actionResInfo.AbsResourceFileName);
                OnCurrentFrameChanged(1);     
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
            D3DViewer.Tick();

            CCore.Engine.Instance.ScreenShake(D3DViewer.CameraController);
        }

        // 文件名相对于Release目录
        string mHostActionFileName = "";
        //string mActionNotifyFileName = "";

        CCore.AnimTree.AnimTreeNode_Action mCurrentAction;
        public CCore.AnimTree.AnimTreeNode_Action CurrentAction
        {
            get { return mCurrentAction; }
            set
            {
                mCurrentAction = value;

                if (mCurrentAction != null)
                {
                    mCurrentAction.SetPause(true);
                    UpdateCurrentActionData(mCurrentAction);
                }
            }
        }

        string mPreviewMeshString = "";
        public string PreviewMeshString
        {
            get { return mPreviewMeshString; }
            set
            {
                mPreviewMeshString = value;
                OnPropertyChanged("PreviewMeshString");
            }
        }

        CSUtility.Animation.ActionNotifier mCurrentActionNotifier = null;
        //List<CSUtility.Animation.ActionNotifier> mActionNotifierList = new List<CSUtility.Animation.ActionNotifier>();
        CSUtility.Animation.ActionSource mActionSource;

        public ActionNotifyEditorControl()
        {
            InitializeComponent();            

            TimeLineCtrl.OnSelectedTimeLineObject = new TimeLine.TimeLineControl.Delegate_OnSelectedTimeLineObject(OnSelectedActionNotifier);
            //TimeLineCtrl.OnAddKeyFrame = new TimeLine.TimeLineControl.Delegate_OnAddFrame(OnAddKeyFrame);
            TimeLineCtrl.OnCurrentFrameChanged = new TimeLine.TimeLineControl.Delegate_OnCurrentFrameChanged(OnCurrentFrameChanged);
            TimeLineCtrl.OnUpdateTimeLinkTrackItemActiveShow = new TimeLine.TimeLineTrackItem.Delegate_OnUpdateTimeLinkTrackItemActiveShow(OnUpdateTimeLinkTrackItemActiveShow);
            TimeLineCtrl.OnTimeLineTrackItemSelected = new TimeLine.TimeLineTrackItem.Delegate_OnSelected(OnTimeLineTrackItemSelected);
            TimeLineCtrl.OnTimeLineTrackItemUnSelected = new TimeLine.TimeLineTrackItem.Delegate_OnUnSelected(OnTimeLineTrackItemUnSelected);
            TimeLineCtrl.OnCreateTimeLineTrackItem = new TimeLine.TimeLineTrackItem.Delegate_OnCreateTimeLinkTrackItem(OnCreateTimeLineTrackItem);
            TimeLineCtrl.OnRemoveTimeLineTrackItem = new TimeLine.TimeLineTrackItem.Delegate_RemoveTimeLineTrackItem(OnRemoveTimeLineTrackItem);
            TimeLineCtrl.PlayLoop = true;

            InitializeActionTypesComboBox();

            mDropAdorner = new EditorCommon.DragDrop.DropAdorner(Retangle_AddMesh);
            NotifyPointProperty.HostControl = this;
        }

        private void InitializeActionTypesComboBox()
        {
            ComboBox_NotifyTypes.Items.Clear();

            var notifierType = typeof(CSUtility.Animation.ActionNotifier);
//             ComboBox_NotifyTypes.Items.Add(notifierType);

            foreach (var type in CSUtility.Program.GetTypes())
            {
                if (type.IsSubclassOf(notifierType))
                {
                    ComboBox_NotifyTypes.Items.Add(type);
                }
            }

            ComboBox_NotifyTypes.SelectedIndex = 0;

        }

        private void OnSelectedActionNotifier(CSUtility.Animation.TimeLineObjectInterface obj)
        {
            mCurrentActionNotifier = obj as CSUtility.Animation.ActionNotifier;
        }

        //private CSUtility.Animation.TimeLineKeyFrameObjectInterface OnAddKeyFrame(Int64 milliTime)
        //{
        //    if (mCurrentActionNotifier != null)
        //    {
        //        return mCurrentActionNotifier.AddNotifyPoint(milliTime, "NewNotify");
        //    }

        //    return null;
        //}

        private Dictionary<CSUtility.Animation.NotifyPoint, List<CCore.World.Actor>> mNotifyPointWorldActorsDictionary = new Dictionary<CSUtility.Animation.NotifyPoint, List<CCore.World.Actor>>();
        private List<CCore.World.Actor> mPreFrameWorldActors = new List<CCore.World.Actor>();

        private void OnCurrentFrameChanged(Int64 curFrame)
        {
            if (CurrentAction != null)
            {

                if (TimeLineCtrl.TotalFrame != 0)
                    CurrentAction.CurAnimTime = TimeLineCtrl.GetFrameMillisecondTime(curFrame);//curFrame * TimeLineCtrl.TotalMilliTime / TimeLineCtrl.TotalFrame;
                else
                    CurrentAction.CurAnimTime = 0;

                //System.Diagnostics.Debug.WriteLine("CurrentFrame=" + curFrame + ",CurAnimTime=" + CurrentAction.CurAnimTime);

                /*/ 将上一帧的对象从场景去除
                foreach (var actor in mPreFrameWorldActors)
                {
                    D3DEvrCtrl.World.RemoveActor(actor.Id);
                }
                mPreFrameWorldActors.Clear();

                foreach (var notify in mActionSource.NotifierList)
                {
                    var ntPts = notify.GetNotifyPoints(curFrame, curFrame + 1);

                    foreach (var pt in ntPts)
                    {
                        if (pt.GetType() == typeof(CSUtility.Animation.AttackNotifyPoint))
                        {
                            List<CCore.World.Actor> actorList = null;
                            if (mNotifyPointWorldActorsDictionary.TryGetValue(pt, out actorList))
                            {
                                foreach (var actor in actorList)
                                {
                                    D3DEvrCtrl.World.AddActor(actor);
                                }

                                mPreFrameWorldActors.AddRange(actorList);
                            }
                            else
                            {
                                var anPt = pt as CSUtility.Animation.AttackNotifyPoint;

                                foreach (var box in anPt.BoxList)
                                {
                                    CCore.Component.V3dBox3 boxVis = new CCore.Component.V3dBox3();
                                    MidLayer.ICommActorInit actInit = new MidLayer.ICommActorInit();
                                    CCore.World.Actor actor = new CCore.World.Actor();
                                    actor.Initialize(actInit);
                                    actor.Visual = boxVis;
                                    actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));
                                    var loc = box.GetCenter();
                                    actor.Placement.SetLocation(ref loc);
                                    var scale = new SlimDX.Vector3(box.Maximum.X - box.Minimum.X,
                                                                    box.Maximum.Y - box.Minimum.Y,
                                                                    box.Maximum.Z - box.Minimum.Z);
                                    actor.Placement.SetScale(ref scale);
                                    D3DEvrCtrl.World.AddActor(actor);

                                    actorList.Add(actor);
                                }

                                mNotifyPointWorldActorsDictionary[anPt] = actorList;
                                mPreFrameWorldActors.AddRange(actorList);
                            }
                        }
                    }
                }*/
            }
        }

        private void OnUpdateTimeLinkTrackItemActiveShow(TimeLine.TimeLineTrackItem item)
        {
            //             if (item.IsActive)
            //             {
            //                 if (item.TimeLineItemProCtrl is AttackNotifyPointControl)
            //                 {
            //                     if (D3DShowPlugin != null)
            //                     {
            //                         var atkNotifyPtCtrl = item.TimeLineItemProCtrl as AttackNotifyPointControl;
            //                         foreach (AttackNotifyPointListItem boxItem in atkNotifyPtCtrl.BoxItems)
            //                         {
            //                             ////////////if (D3DEvrCtrl.World.FindActor(boxItem.Actor.Id) == null)
            //                             ////////////{
            //                             ////////////    D3DEvrCtrl.World.AddActor(boxItem.Actor);
            //                             ////////////}
            //                             D3DShowPlugin.SetObjectToEdit(new object[] { 
            //                                                              new object[] { boxItem.Actor }});
            //                         }
            //                     }
            //                 }
            //                 else if (item.TimeLineItemProCtrl is EffectNotifyPointControl)
            //                 {
            // 
            //                 }
            //             }
            //             else
            //             {
            //                 if (item.TimeLineItemProCtrl is AttackNotifyPointControl)
            //                 {
            //                     if (D3DShowPlugin != null)
            //                     {
            //                         var atkNotifyPtCtrl = item.TimeLineItemProCtrl as AttackNotifyPointControl;
            //                         foreach (AttackNotifyPointListItem boxItem in atkNotifyPtCtrl.BoxItems)
            //                         {
            //                             D3DShowPlugin.RemoveObjects(new object[] { "Actor", boxItem.Actor });
            //                             ////////////D3DEvrCtrl.World.RemoveActor(boxItem.Actor);
            //                         }
            //                     }
            //                 }
            //             }
            var notifyPoint = item.KeyFrameItem as CSUtility.Animation.NotifyPoint;
            if (notifyPoint == null)
                return;
            
            AddActorToD3DViewer(notifyPoint, item.IsActive);            
        }

        public void AddActorToD3DViewer(CSUtility.Animation.NotifyPoint notifyPoint,bool add = true)
        {
            if (notifyPoint == null)
                return;
            foreach (var i in notifyPoint.PointDatas)
            {
                AddActorToD3DViewer(i, add);
            }
        }

        public void AddActorToD3DViewer(CSUtility.Animation.NotifyItemDataBase itemData, bool add = true)
        {
            if (itemData == null)
                return;

            CSUtility.Component.ActorBase actor = null;
            if (itemData is CSUtility.Animation.IGetVisualActor)
            {
                actor = (itemData as CSUtility.Animation.IGetVisualActor).GetVisualActor();
            }

            if (actor == null)
                return;

            if (add)
            {
                if (D3DViewer.World.FindActor(actor.Id) == null)
                {
                    D3DViewer.SetObjectToEdit(new object[] { new object[]{ "Actor", true },
                                                         new object[]{ actor }});
                }
            }
            else
            {
                D3DViewer.World.RemoveActor((CCore.World.Actor)actor);
            }
        }        

        TimeLine.TimeLineTrackItem curItem;
        private void OnTimeLineTrackItemSelected(TimeLine.TimeLineTrackItem item)
        {
            //             Grid_PropertyControlContainer.Children.Clear();
            //             Grid_PropertyControlContainer.Children.Add(item.TimeLineItemProCtrl);
            if (item.KeyFrameItem == null)
                return;
            if (item == curItem)
                return;

            curItem = item;            

            NotifyPointProperty.PropertyInstance = item.KeyFrameItem;


            var notifyPoint = item.KeyFrameItem as CSUtility.Animation.NotifyPoint;
            if (notifyPoint == null)
                return;

            AddActorToD3DViewer(notifyPoint, true);

        }

        private void OnTimeLineTrackItemUnSelected(TimeLine.TimeLineTrackItem item)
        {            
            if (item.KeyFrameItem == null)
                return;

            var notifyPoint = item.KeyFrameItem as CSUtility.Animation.NotifyPoint;
            if (notifyPoint == null)
                return;

            AddActorToD3DViewer(notifyPoint, false);
        }

        private void OnCreateTimeLineTrackItem(TimeLine.TimeLineTrackItem item)
        {
            var notifyPoint = item.KeyFrameItem as CSUtility.Animation.NotifyPoint;
            notifyPoint.AddItemDatas(notifyPoint);
            foreach (var i in notifyPoint.PointDatas)
            {
                NotifyPointProperty.InitPointItemData(i);                
            }
        }

        private void OnRemoveTimeLineTrackItem(TimeLine.TimeLineTrackItem item)
        {
            var notifyPoint = item.KeyFrameItem as CSUtility.Animation.NotifyPoint;            
            if (NotifyPointProperty.NotifyPoint == notifyPoint)
            {
                NotifyPointProperty.PropertyInstance = null;
                AddActorToD3DViewer(notifyPoint, false);
            }
        }

        public void InitWithActionFullFileName(string fileName)
        {
            //if (mActionSource!=null)
            //    mActionSource.NotifierList.Clear();

            mHostActionFileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(fileName, CSUtility.Support.IFileManager.Instance.Root);
            //mActionNotifyFileName = mHostActionFileName + CSUtility.Support.IFileConfig.ActionNotifyExtension;
            var action = new CCore.AnimTree.AnimTreeNode_Action();
            action.Initialize();
            action.ActionName = mHostActionFileName;            

            CurrentAction = action;
            ////////////CurrentAction = D3DEvrCtrl.SetAction(mHostActionFileName);

            TextBlock_CurrentActionName.Text = "当前编辑动作:" + CSUtility.Support.IFileManager.Instance.GetPureFileFromFullName(fileName);

            // 尝试读取ActionNotifys
            LoadActionNotify(mHostActionFileName);

            D3DViewer.SetObjectToEdit(new object[] { new object[] { "Action", true },
                                                              new object[] { 0, action }});
        }

        private void UpdateCurrentActionData(CCore.AnimTree.AnimTreeNode_Action action)
        {
            if (action == null)
                return;

            TimeLineCtrl.Initialize();
            TimeLineCtrl.TotalMilliTime = action.Duration;
        }

        private void Button_MeshSet_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("MeshTemplate");
            if (data == null)
                return;

            if (data.Length > 0)
            {                
                mActionSource.MeshId = CSUtility.Program.GetIdFromFile((string)data[0]);

                var meshTemplate = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(mActionSource.MeshId);
                PreviewMeshString = meshTemplate.NickName;

                var mshInit = new CCore.Mesh.MeshInit()
                {
                    MeshTemplateID = meshTemplate.MeshID,
                    CanHitProxy = false
                };
                var mesh = new CCore.Mesh.Mesh();
                mesh.Initialize(mshInit, null);

                var atInit = new CCore.World.MeshActorInit();
                var actor = new CCore.World.MeshActor();
                actor.Initialize(atInit);
                actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));
                actor.mUpdateAnimByDistance = false;
                actor.Visual = mesh;

                D3DViewer.SetObjectToEdit(new object[] { new object[]{ "Actor", false },
                                                         new object[]{ actor }});
            }
        }

        private void Button_MeshSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var meshFileName = CSUtility.Support.IFileManager.Instance.Root + CCore.Mesh.MeshTemplateMgr.Instance.GetMeshTemplateFile(mActionSource.MeshId);

            EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", meshFileName });
        }

        private void Button_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveActionNotify(mHostActionFileName);
            //CSUtility.Animation.
        }

        private void Button_AddNotify_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ComboBox_NotifyTypes.SelectedIndex < 0)
            {
                EditorCommon.MessageBox.Show("请先选择要创建的Notify类型");
                return;
            }

            var an = System.Activator.CreateInstance((System.Type)(ComboBox_NotifyTypes.SelectedItem)) as CSUtility.Animation.ActionNotifier;
            if (CurrentAction != null)
                an.ActionDurationMilliSecond = CurrentAction.Duration;
            TimeLineCtrl.AddTimeLineObject(an);
            if (mActionSource != null)
                mActionSource.AddNotifier(an);// .NotifierList.Add(an);
        }

        private void Button_RemoveNotify_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var an = TimeLineCtrl.RemoveSelectedTimeLineObject() as CSUtility.Animation.ActionNotifier;
            if (mActionSource != null)
                mActionSource.RemoveNotifier(an);// .NotifierList.Remove(an);
        }

        private bool LoadActionNotify(string fileName)
        {
            TimeLineCtrl.Cleanup();

            mActionSource = CSUtility.Animation.ActionNodeManager.Instance.GetActionSource(fileName, false, CSUtility.Helper.enCSType.All);
            if (mActionSource == null)
            {
                mActionSource = new CSUtility.Animation.ActionSource();
            }

            if (mActionSource.MeshId != Guid.Empty)
            {
                var meshTemplate = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(mActionSource.MeshId);
                if (meshTemplate != null)
                {
                    PreviewMeshString = meshTemplate.NickName;

                    var mshInit = new CCore.Mesh.MeshInit()
                    {
                        MeshTemplateID = meshTemplate.MeshID,
                        CanHitProxy = false
                    };
                    var mesh = new CCore.Mesh.Mesh();
                    mesh.Initialize(mshInit, null);

                    var atInit = new CCore.World.MeshActorInit();
                    var actor = new CCore.World.MeshActor();
                    actor.Initialize(atInit);
                    actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));
                    actor.mUpdateAnimByDistance = false;
                    actor.Visual = mesh;

                    D3DViewer.SetObjectToEdit(new object[] { new object[]{ "Actor", false },
                                                         new object[]{ actor }});

                    //                     var action = new CCore.AnimTree.AnimTreeNode_Action();
                    //                     action.Initialize();
                    //                     action.ActionName = mHostActionFileName;

                    //                     if (D3DShowPlugin != null)
                    //                     {
                    //                         D3DShowPlugin.SetObjectToEdit(new object[] { new object[] { "Action", true },
                    //                                                              new object[] { 0, action }});
                    // 
                    //                     }
                    /*CurrentAction = action;*/
                    ////////////CurrentAction = D3DEvrCtrl.SetAction(mHostActionFileName);
                }
            }
            else
            {
//                 if (D3DShowPlugin != null)
//                 {
//                     D3DShowPlugin.SetObjectToEdit(null);
//                 }
                ////////////D3DEvrCtrl.SetMesh("");
                PreviewMeshString = "";
                D3DViewer.SetObjectToEdit(null);
                //     CurrentAction = null;
            }

            foreach (var notify in mActionSource.NotifierList)
            {
                TimeLineCtrl.AddTimeLineObject(notify);
            }

            return true;
        }

        private void SaveActionNotify(string fileName)
        {
            if (mActionSource == null)
                return;

            mActionSource.Duration = 0;
            if (CurrentAction != null)
                mActionSource.Duration = CurrentAction.Duration;
            CSUtility.Animation.ActionNodeManager.Instance.SaveActionSource(mActionSource, fileName);
            CSUtility.Animation.ActionNodeManager.Instance.GetActionSource(fileName, true, CSUtility.Helper.enCSType.All);

            // 通知服务器更新ActionNotify
            TellServerUpdateActionNotify(fileName);
        }

        void TellServerUpdateActionNotify(string fileName)
        {
#warning RefreshActionNotify
            //RPC.PackageWriter pkg = new RPC.PackageWriter();
            //ServerCommon.H_RPCRoot.smInstance.HGet_PlanesServer(pkg).HGet_GameMaster(pkg).RefreshActionNotify(pkg, fileName);
            //pkg.DoClient2Planes(CCore.Engine.Instance.Client.GateSvrConnect);
        }

        enum enDropResult
        {
            Denial_UnknowFormat,
            Denial_NoDragAbleObject,
            Allow,
        }
        // 是否允许拖放
        enDropResult AllowResourceItemDrop(System.Windows.DragEventArgs e)
        {
            var formats = e.Data.GetFormats();
            if (formats == null || formats.Length == 0)
                return enDropResult.Denial_UnknowFormat;

            var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
            if (datas == null)
                return enDropResult.Denial_NoDragAbleObject;

            bool containMeshSource = false;
            foreach (var data in datas)
            {
                var resInfo = data as ResourcesBrowser.ResourceInfo;
                if (resInfo.ResourceType == "MeshTemplate")
                {
                    containMeshSource = true;
                    break;
                }
            }

            if (!containMeshSource)
                return enDropResult.Denial_NoDragAbleObject;

            return enDropResult.Allow;
        }

        EditorCommon.DragDrop.DropAdorner mDropAdorner;        

        private void Rectangle_AddMesh_DragEnter(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                mDropAdorner.IsAllowDrop = false;

                switch (AllowResourceItemDrop(e))
                {
                    case enDropResult.Allow:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "添加模型资源";

                            mDropAdorner.IsAllowDrop = true;
                            var pos = e.GetPosition(Retangle_AddMesh);
                            if (pos.X > 0 && pos.X < Retangle_AddMesh.ActualWidth &&
                               pos.Y > 0 && pos.Y < Retangle_AddMesh.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(Retangle_AddMesh);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;

                    case enDropResult.Denial_NoDragAbleObject:
                    case enDropResult.Denial_UnknowFormat:
                        {
                            EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "拖动内容不是模型资源";

                            mDropAdorner.IsAllowDrop = false;
                            var pos = e.GetPosition(Retangle_AddMesh);
                            if (pos.X > 0 && pos.X < Retangle_AddMesh.ActualWidth &&
                               pos.Y > 0 && pos.Y < Retangle_AddMesh.ActualHeight)
                            {
                                var layer = AdornerLayer.GetAdornerLayer(Retangle_AddMesh);
                                layer.Add(mDropAdorner);
                            }
                        }
                        break;
                }
            }
        }

        private void Rectangle_AddMesh_DragLeave(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                EditorCommon.DragDrop.DragDropManager.Instance.InfoString = "";
                var layer = AdornerLayer.GetAdornerLayer(Retangle_AddMesh);
                layer.Remove(mDropAdorner);
            }
        }

        private void Rectangle_AddMesh_DragOver(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                if (AllowResourceItemDrop(e) == enDropResult.Allow)
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void Rectangle_AddMesh_Drop(object sender, DragEventArgs e)
        {
            if (EditorCommon.DragDrop.DragDropManager.Instance.DragType.Equals("ResourceItem"))
            {
                e.Handled = true;
                var layer = AdornerLayer.GetAdornerLayer(Retangle_AddMesh);
                layer.Remove(mDropAdorner);

                if (AllowResourceItemDrop(e) == enDropResult.Allow)
                {
                    var formats = e.Data.GetFormats();
                    var datas = e.Data.GetData(formats[0]) as EditorCommon.DragDrop.IDragAbleObject[];
                    foreach (var data in datas)
                    {
                        var resInfo = data as ResourcesBrowser.ResourceInfo;
                        if (resInfo == null)
                            continue;
                        
                        mActionSource.MeshId = CSUtility.Program.GetIdFromFile(resInfo.RelativeResourceFileName);

                        var meshTemplate = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(mActionSource.MeshId);
                        PreviewMeshString = meshTemplate.NickName;

                        var mshInit = new CCore.Mesh.MeshInit()
                        {
                            MeshTemplateID = meshTemplate.MeshID,
                            CanHitProxy = false
                        };
                        var mesh = new CCore.Mesh.Mesh();
                        mesh.Initialize(mshInit, null);

                        var atInit = new CCore.World.MeshActorInit();
                        var actor = new CCore.World.MeshActor();
                        actor.Initialize(atInit);
                        actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));
                        actor.mUpdateAnimByDistance = false;
                        actor.Visual = mesh;

                        D3DViewer.SetObjectToEdit(new object[] { new object[]{ "Actor", false },
                                                         new object[]{ actor }});
                    }
                }
            }
        }
    }
}
