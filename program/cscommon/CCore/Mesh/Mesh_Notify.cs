using System;
using System.Collections.Generic;

namespace CCore.Mesh
{
    public partial class Mesh
    {
        // 需要Tick的Notify类型
        private Type[] mNotifyUpdateTypes = new Type[] { typeof(CSUtility.ActionNotify.EffectActionNotifier) };

        List<CSUtility.ActionNotify.EffectActionNotifier> mEffectNotifiers = new List<CSUtility.ActionNotify.EffectActionNotifier>();
        /// <summary>
        /// 初始化mesh的关键帧
        /// </summary>
        /// <param name="animTree">动画树</param>
        protected void InitializeNotifys(CCore.AnimTree.AnimTreeNode animTree)
        {
            mEffectNotifiers.Clear();

            if (animTree == null)
                return;

            if (animTree.Action == null)
                return;

            var notifiers = animTree.Action.GetNotifiers(typeof(CSUtility.ActionNotify.EffectActionNotifier));
            foreach (CSUtility.ActionNotify.EffectActionNotifier ntf in notifiers)
            {
                var efNtf = new CSUtility.ActionNotify.EffectActionNotifier();
                efNtf.CopyFrom(ntf);
                efNtf.OnActiveEffect = ActiveNotifyEffect;
                mEffectNotifiers.Add(efNtf);               
            }
        }
        /// <summary>
        /// 激活关键帧特效
        /// </summary>
        /// <param name="data">关键帧的特效数据</param>
        protected void ActiveNotifyEffect(CSUtility.ActionNotify.EffectItemData data)
        {
            if (data == null)
                return;

            var socket = this.SocketTable?.GetSocket(data.SocketName);
            if (socket == null)
                return;

            var effVisInit = new CCore.Component.EffectVisualInit()
            {
                EffectTemplateID = data.EffectId,
                CanHitProxy = false
            };
            var effVis = new CCore.Component.EffectVisual();
            effVis.Initialize(effVisInit, null);

            var effActorInit = new CCore.World.EffectActorInit()
            {
                ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.WithOutSceneManager
            };
            var effActor = new CCore.World.EffectActor();
            effActor.Initialize(effActorInit);
            effActor.Visual = effVis;
            effActor.LoopPlay = false;

            if (this.HostActor != null)
            {
                effActor.SetPlacement(new CSUtility.Component.StandardPlacement(effActor));
                SlimDX.Matrix hostMatrix = SlimDX.Matrix.Identity;
                this.HostActor.Placement.GetAbsMatrix(out hostMatrix);
                var matrix = socket.AbsMatrix * hostMatrix;

                effActor.Placement.SetMatrix(ref matrix);

                var tempLoc = effActor.Placement.GetLocation() + data.Offset;
                effActor.Placement.SetLocation(ref tempLoc);
                effActor.Placement.SetScale(data.Scale);

                SlimDX.Quaternion quat = SlimDX.Quaternion.RotationAxis(data.RotationPos, (float)data.Angle * (float)System.Math.PI / 180f);
                effActor.Placement.SetRotation(ref quat);

            }
//             else
//             {
//                 effActor.SetPlacement(new CSUtility.Component.StandardPlacement(effActor));
//                 SlimDX.Vector3 tempLoc;
//                 if (this.HostActor != null)
//                     tempLoc = this.HostActor.Placement.GetLocation() + data.Offset;
//                 else
//                     tempLoc = data.Offset;
// 
//                 effActor.Placement.SetLocation(ref tempLoc);
//             }

            if (this.HostActor != null && this.HostActor.World != null)
                this.HostActor.World.AddActor(effActor);
            else
                CCore.Client.MainWorldInstance.AddActor(effActor);            
        }
    }
}
