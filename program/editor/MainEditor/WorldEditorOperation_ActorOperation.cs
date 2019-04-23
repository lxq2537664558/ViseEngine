using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainEditor
{
    public partial class WorldEditorOperation
    {
        // 选中的对象列表
        CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> mSelectedActors = new CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor>();
        public CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> SelectedActors
        {
            get { return mSelectedActors; }
        }
        // 选中对象的外部包围盒对象列表
        Dictionary<CCore.World.Actor, CCore.Support.OBBShowActor> mSelectedBoundActors = new Dictionary<CCore.World.Actor, CCore.Support.OBBShowActor>();

        private void Initialize_ActorOperation()
        {
            EditorCommon.GameActorOperation.OnSelectActors += GameActorOperation_OnSelectActors;
            EditorCommon.GameActorOperation.OnUnSelectActors += GameActorOperation_OnUnSelectActors;
            EditorCommon.GameActorOperation.OnFocusActors += FocusActors;
            EditorCommon.GameActorOperation.OnRemoveActor += _RemoveActor;
        }

        private void GameActorOperation_OnStartCopyActors(CCore.MsgProc.BehaviorParameter parameter, object obj)
        {
            if (m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Null)
                return;

            var copyedActorList = new List<CCore.World.Actor>();
            foreach(var actor in mSelectedActors)
            {
                var copyedActor = actor.Duplicate();
                copyedActor.ActorName += CCore.Program.GetActorIndex();
                copyedActorList.Add(copyedActor);
                CCore.Client.MainWorldInstance.AddActor(copyedActor);
            }

            // 选中复制出来的对象
            EditorCommon.GameActorOperation.SelectActors(copyedActorList);

            StartTranslateAxis(parameter, obj);
        }

        private void GameActorOperation_OnSelectActors(List<CCore.World.Actor> actors)
        {
            int i = 0;
            bool multiSelected = true;
            foreach (var actor in actors)
            {
                if (i == 0)
                    multiSelected = false;
                else
                    multiSelected = true;

                SelectedActor(actor, multiSelected);

                i++;
            }
        }

        private void GameActorOperation_OnUnSelectActors(List<CCore.World.Actor> actors)
        {
            foreach(var actor in actors)
            {
                if(mSelectedActors.Contains(actor))
                {
                    mSelectedActors.Remove(actor);

                    CCore.Support.OBBShowActor boundActor;
                    if(mSelectedBoundActors.TryGetValue(actor, out boundActor))
                    {
                        CCore.Client.MainWorldInstance.RemoveEditorActor(boundActor);
                        mSelectedBoundActors.Remove(actor);
                    }

                    actor.Editor_UnSelected();
                }
            }

            if (mSelectedActors.Count == 0)
                SetAxisTargets(null);
            else
                SetAxisTargets(mSelectedActors);
        }

        // 通过HitProxy获取选中的Actor
        CCore.World.Actor GetActorByHitProxy(int x, int y, out UInt32 hitId)
        {
            hitId = mREnviroment.GetHitProxy(x, y);
            var actorId = CCore.Graphics.HitProxyMap.Instance.GetActorId(hitId);
            if (m3dAxis.Id == actorId)
                return m3dAxis;

            if (CCore.Client.MainWorldInstance == null || CCore.Client.MainWorldInstance.IsNullWorld)
                return null;

            var actor = CCore.Client.MainWorldInstance.FindActor(actorId);

            var hitIdList = new List<UInt32>();
            if (!mREnviroment.GetHitProxy(x, y, 1, hitIdList))
                return actor;

            foreach(var hitIdx in hitIdList)
            {
                if(m3dAxis.Id == CCore.Graphics.HitProxyMap.Instance.GetActorId(hitIdx))
                {
                    hitId = hitIdx;
                    return m3dAxis;
                }
            }

            return actor;
        }
        // 取消选择所有对象
        void UnSelectedAll()
        {
            EditorCommon.GameActorOperation.UnSelectActor(new List<CCore.World.Actor>(mSelectedActors));

            ////////////foreach(var selectedActor in mSelectedActors)
            ////////////{
            ////////////    selectedActor.Editor_UnSelected();

            ////////////    // 场景内的对象会自动跨格，不再需要放入EditorActor中
            ////////////    //CCore.Client.MainWorldInstance.AddActor(selectedActor);
            ////////////    //CCore.Client.MainWorldInstance.RemoveEditorActor(selectedActor);
            ////////////}
            ////////////mSelectedActors.Clear();
            ////////////foreach(var boundActor in mSelectedBoundActors.Values)
            ////////////{
            ////////////    CCore.Client.MainWorldInstance.RemoveEditorActor(boundActor);
            ////////////}
            ////////////mSelectedBoundActors.Clear();

            ////////////SetAxisTargets(null);
        }
        void _RemoveActor(CCore.World.Actor actor)
        {
            CCore.Support.OBBShowActor obbActor;
            if (mSelectedBoundActors.TryGetValue(actor, out obbActor))
            {
                CCore.Engine.Instance.Client.MainWorld.RemoveEditorActor(obbActor);
            }
        }
        // 选择对象
        void SelectedActor(CCore.World.Actor actor, bool multiSelected)
        {
            if(actor == null)
            {
                UnSelectedAll();
                return;
            }

            if (actor == m3dAxis)
                return;

            if(multiSelected)
            {
                if(mSelectedActors.Contains(actor))
                {
                    actor.Editor_UnSelected();

                    // 场景内的对象会自动跨格，不再需要放入EditorActor中
                    //CCore.Client.MainWorldInstance.AddActor(actor);
                    //CCore.Client.MainWorldInstance.RemoveEditorActor(actor);
                    mSelectedActors.Remove(actor);
                    CCore.Client.MainWorldInstance.RemoveEditorActor(mSelectedBoundActors[actor]);
                    mSelectedBoundActors.Remove(actor);
                }
                else
                {
                    mSelectedActors.Add(actor);
                    actor.Editor_Selected();

                    // 设置包围盒
                    var boundActor = new CCore.Support.OBBShowActor();
                    boundActor.Initialize(new CCore.World.ActorInit());
                    boundActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
                    boundActor.SetTarget(actor);
                    mSelectedBoundActors[actor] = boundActor;
                    CCore.Client.MainWorldInstance.AddEditorActor(boundActor);
                }

                // 场景内的对象会自动跨格，不再需要放入EditorActor中
                //// 将选中的对象移出场景管理，防止移动的时候被裁减掉
                //CCore.Client.MainWorldInstance.RemoveActor(actor);
                //CCore.Client.MainWorldInstance.AddEditorActor(actor);
            }
            else
            {
                UnSelectedAll();
                mSelectedActors.Add(actor);
                actor.Editor_Selected();

                // 设置包围盒
                var boundActor = new CCore.Support.OBBShowActor();
                boundActor.Initialize(new CCore.World.ActorInit());
                boundActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
                boundActor.SetTarget(actor);
                mSelectedBoundActors[actor] = boundActor;
                CCore.Client.MainWorldInstance.AddEditorActor(boundActor);
                
                // 场景内的对象会自动跨格，不再需要放入EditorActor中
                //// 将选中的对象移出场景管理，防止移动的时候被裁减掉
                //CCore.Client.MainWorldInstance.RemoveActor(actor);
                //CCore.Client.MainWorldInstance.AddEditorActor(actor);
            }

            SetAxisTargets(mSelectedActors);
            //MainEditor.MainWindow.Instance.OnSelectedSceneActorsUpdate(mSelectedActors);

            if (mSelectedActors.Count > 0)
            {
                EditorCommon.PluginAssist.PropertyGridAssist.SetSelectedObjectData("WorldActor", new object[] { mSelectedActors[0] });
            }
            else
            {
                EditorCommon.PluginAssist.PropertyGridAssist.SetSelectedObjectData("WorldActor", new object[] { null });
            }
        }

        // 选择Actor操作
        public void SelectActorBehavior(CCore.MsgProc.BehaviorParameter param, object obj)
        {
            var mouseKeyParam = param as CCore.MsgProc.Behavior.Mouse_Key;

            switch(EditorCommon.WorldEditMode.Instance.EditMode)
            {
                case EditorCommon.WorldEditMode.enEditMode.Edit_Camera:
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisMove:
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisRot:
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisScale:
                    {
                        var start = mREnviroment.Camera.GetLocation();
                        var end = start + mREnviroment.Camera.GetPickDirection(mouseKeyParam.X, mouseKeyParam.Y) * 1000.0f;

                        UInt32 hitId;
                        var actor = GetActorByHitProxy(mouseKeyParam.X, mouseKeyParam.Y, out hitId);
                        //SelectedActor(actor, false);
                        EditorCommon.GameActorOperation.SelectActors(new List<CCore.World.Actor>() { actor });
                    }
                    break;
            }
        }
        // 多选Actor操作
        public void MultiSelectActorBehavior(CCore.MsgProc.BehaviorParameter param, object obj)
        {
            var mouseKeyParam = param as CCore.MsgProc.Behavior.Mouse_Key;

            switch(EditorCommon.WorldEditMode.Instance.EditMode)
            {
                case EditorCommon.WorldEditMode.enEditMode.Edit_Camera:
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisMove:
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisRot:
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisScale:
                    {
                        var start = mREnviroment.Camera.GetLocation();
                        var end = start + mREnviroment.Camera.GetPickDirection(mouseKeyParam.X, mouseKeyParam.Y) * 1000.0f;

                        UInt32 hitId;
                        var actor = GetActorByHitProxy(mouseKeyParam.X, mouseKeyParam.Y, out hitId);
                        var actors = new List<CCore.World.Actor>(mSelectedActors);
                        actors.Add(actor);
                        EditorCommon.GameActorOperation.SelectActors(actors);
                        //SelectedActor(actor, true);
                    }
                    break;
            }
        }

        // 预选则场景对象
        public bool PreChooseEnable = false;
        CCore.World.Actor mMouseHoverActor = null;
        public void PreChooseActor(CCore.MsgProc.BehaviorParameter parameter)
        {
            var mouseMove = parameter as CCore.MsgProc.Behavior.Mouse_Move;
            switch (EditorCommon.WorldEditMode.Instance.EditMode)
            {
                case EditorCommon.WorldEditMode.enEditMode.Edit_Camera:
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisMove:
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisRot:
                case EditorCommon.WorldEditMode.enEditMode.Edit_AxisScale:
                    {
                        var start = mREnviroment.Camera.GetLocation();
                        var end = start + mREnviroment.Camera.GetPickDirection(mouseMove.X, mouseMove.Y) * 1000.0f;

                        UInt32 hitId;
                        var actor = GetActorByHitProxy(mouseMove.X, mouseMove.Y, out hitId);

                        if (mMouseHoverActor != actor && PreChooseEnable)
                        {
                            if (mMouseHoverActor != null)
                                mMouseHoverActor.OnMouseLeave();
                            if (actor != null)
                                actor.OnMouseEnter();

                            mMouseHoverActor = actor;
                        }

                        if(!IsTranslatingAxis)
                        {
                            if (actor == m3dAxis)
                                m3dAxis.OnMousePointAt(hitId);
                            else
                                m3dAxis.OnMousePointOut();
                        }
                    }
                    break;
            }
        }

        public void FocusActors(List<CCore.World.Actor> actors)
        {
            if (actors != null && actors.Count > 0)
            {
                SlimDX.Vector3 vMax = actors[0].Placement.GetLocation();
                SlimDX.Vector3 vMin = actors[0].Placement.GetLocation();

                foreach (var actor in actors)
                {
                    SlimDX.Vector3 v0, v1;

                    if (actor.Visual is CCore.Mesh.Mesh)
                    {
                        var mesh = actor.Visual as CCore.Mesh.Mesh;
                        SlimDX.Matrix transMatrix = actor.Placement.mMatrix;
                        v0 = SlimDX.Vector3.TransformCoordinate(mesh.vMax, transMatrix);
                        v1 = SlimDX.Vector3.TransformCoordinate(mesh.vMin, transMatrix);
                    }
                    else
                    {
                        var pos = actor.Placement.GetLocation();
                        v0 = pos + SlimDX.Vector3.UnitXYZ * 3;
                        v1 = pos - SlimDX.Vector3.UnitXYZ * 3;
                    }

                    if (vMax.X < v0.X)
                        vMax.X = v0.X;
                    if (vMax.Y < v0.Y)
                        vMax.Y = v0.Y;
                    if (vMax.Z < v0.Z)
                        vMax.Z = v0.Z;

                    if (vMax.X < v1.X)
                        vMax.X = v1.X;
                    if (vMax.Y < v1.Y)
                        vMax.Y = v1.Y;
                    if (vMax.Z < v1.Z)
                        vMax.Z = v1.Z;

                    if (vMin.X > v0.X)
                        vMin.X = v0.X;
                    if (vMin.Y > v0.X)
                        vMin.Y = v0.Y;
                    if (vMin.Z > v0.Z)
                        vMin.Z = v0.Z;

                    if (vMin.X > v1.X)
                        vMin.X = v1.X;
                    if (vMin.Y > v1.Y)
                        vMin.Y = v1.Y;
                    if (vMin.Z > v1.Z)
                        vMin.Z = v1.Z;
                }

                EditorCommon.Program.MaxZoomMeshShow(0, 0, REnviroment.View.Width, REnviroment.View.Height, vMax, vMin, FreeCameraController);
            }
        }
    }
}
