using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
/// <summary>
/// 角色模板的命名空间
/// </summary>
namespace CCore.World.Role
{    
    /// <summary>
    /// NPC初始化类
    /// </summary>
    public class NPCInitializerActor : CCore.World.Actor
    {
        UInt32 mNPCTemplateVersion = 0;
        CSUtility.Map.Role.NPCInitializerActorInit mNPCInitializerActorInit;
        /// <summary>
        /// 圆数据
        /// </summary>
        class CircleData
        {
            float mRadius = -1;
            /// <summary>
            /// 圆的半径
            /// </summary>
            public float Radius
            {
                get { return mRadius; }
                set
                {
                    mRadius = value;

                    ScaleMat = SlimDX.Matrix.Scaling(mRadius, mRadius, mRadius);
                }
            }
            /// <summary>
            /// 缩放矩阵
            /// </summary>
            public SlimDX.Matrix ScaleMat = SlimDX.Matrix.Identity;
            /// <summary>
            /// 贴花对象
            /// </summary>
            public Component.Decal Decal = new Component.Decal();
            /// <summary>
            /// 圆数据的构造函数
            /// </summary>
            public CircleData()
            {
                Decal.CanHitProxy = false;
                Decal.ShowSignMesh = false;
            }
        }
        CircleData mWanderData = new CircleData();
        CircleData mLockonData = new CircleData();
        CircleData mCallHelpData = new CircleData();
        /// <summary>
        /// NPC初始化类的构造函数
        /// </summary>
        public NPCInitializerActor()
        {
            mPlacement = new CSUtility.Component.StandardPlacement(this);            

            this.Placement.OnLocationChanged += OnLocationChanged;
            this.Placement.OnRotationChanged += OnRotationChanged;
            this.Placement.OnScaleChanged += OnScaleChanged;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~NPCInitializerActor()
        {
            this.Placement.OnLocationChanged -= OnLocationChanged;
            this.Placement.OnRotationChanged -= OnRotationChanged;
            this.Placement.OnScaleChanged -= OnScaleChanged;
        }
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">Actor的初始化类对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            if (base.Initialize(_init) == false)
                return false;

            mNPCInitializerActorInit = _init as CSUtility.Map.Role.NPCInitializerActorInit;

            if (mNPCInitializerActorInit.NPCData.Template != null)
                mNPCTemplateVersion = mNPCInitializerActorInit.NPCData.Template.Version;

            var roleVisual = new Component.RoleActorVisual();
            var visInit = new Component.RoleActorVisualInit();
            if (mNPCInitializerActorInit.NPCData != null && mNPCInitializerActorInit.NPCData.Template != null)
            {
                visInit.MeshTemplateIds.Clear();
                visInit.MeshTemplateIds.AddRange(mNPCInitializerActorInit.NPCData.Template.DefaultMeshs);                
            }
            visInit.CanHitProxy = true;
            roleVisual.Initialize(visInit, this);
            roleVisual.SetHitProxyAll(Graphics.HitProxyMap.Instance.GenHitProxy(this.Id));

            roleVisual.EdgeDetect = true;

            Visual = roleVisual;

            if (mNPCInitializerActorInit.NPCData != null)
            {
                var pos = mNPCInitializerActorInit.NPCData.Position;
                this.Placement.SetLocation(ref pos);
                var scale = new SlimDX.Vector3(mNPCInitializerActorInit.NPCData.FinalScale);
                this.Placement.SetScale(ref scale);

                if (mNPCInitializerActorInit.NPCData.Template != null)
                    this.Placement.SetRotationY(mNPCInitializerActorInit.NPCData.Direction, mNPCInitializerActorInit.NPCData.Template.MeshFixAngle);

//                 mWanderData.Decal.SetMaterial(CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.CircleDecal_Green));
//                 mLockonData.Decal.SetMaterial(CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(CSUtility.Support.IFileConfig.CircleDecal_Red));
            }

            
//                 switch ((CSUtility.Component.EActorGameType)mNPCInitializerActorInit.GameType)
//                 {
//                     case CSUtility.Component.EActorGameType.Npc:
//                         {
//                             roleVisual.EdgeDetectColor = CSCommon.Data.RoleCommonTemplateManager.Instance.CommonRole.EdgeDetectNPCInitializer;
//                             var npcData = mNPCInitializerActorInit.NPCData as CSCommon.Data.NPCData;
//                         }
//                         break;
//                     case CSCommon.Data.NPCType.GatherNPC:
//                         {
//                             roleVisual.EdgeDetectColor = CSCommon.Data.RoleCommonTemplateManager.Instance.CommonRole.EdgeDetectGatherNPCInitializer;
//                         }
//                         break;
//                 }
            

            return true;
        }

        private void OnLocationChanged(ref SlimDX.Vector3 loc)
        {
            if (mNPCInitializerActorInit == null)
                return;

            if (mNPCInitializerActorInit.NPCData.OriPosition != loc)
            {
                mNPCInitializerActorInit.NPCData.OriPosition = loc;
                mNPCInitializerActorInit.NPCData.Position = loc;
            }
        }

        private void OnRotationChanged(ref SlimDX.Quaternion rot)
        {            
            float dir = 0, pitch = 0, roll = 0;
            rot.GetYawPitchRoll(out dir, out pitch, out roll);

            var temAg = dir - mNPCInitializerActorInit.NPCData.Template.MeshFixAngle;
            var angle = temAg % (System.Math.PI * 2);

            mNPCInitializerActorInit.NPCData.Direction = (float)angle;
        }

        private void OnScaleChanged(ref SlimDX.Vector3 scale)
        {
            // scale只取一个方向
            mNPCInitializerActorInit.NPCData.FinalScale = scale.Z;
        }
        /// <summary>
        /// 获取显示的NPC数据
        /// </summary>
        /// <returns>返回显示的NPC数据</returns>
        public override object GetShowPropertyObj()
        {
            return mNPCInitializerActorInit?.NPCData;
        }
        /// <summary>
        /// 保存场景数据到XND文件
        /// </summary>
        /// <param name="attribute">XND文件节点</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public override bool SaveSceneData(CSUtility.Support.XndAttrib attribute)
        {
            attribute.Write(Id);

            attribute.Write(CSUtility.Program.GetTypeSaveString(mActorInit.GetType()));
            mActorInit.Write(attribute);

            return true;
        }

        /// <summary>
        /// 加载场景数据
        /// </summary>
        /// <param name="attribute">XND数据</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadSceneData(CSUtility.Support.XndAttrib attribute)
        {
            Cleanup();

            Guid id;
            attribute.Read(out id);
            this.mId = id;

            System.String initTypeStr;
            attribute.Read(out initTypeStr);
            System.Type initType = CSUtility.Program.GetTypeFromSaveString(initTypeStr);
            if (initType == null)
            {
                initType = typeof(CSUtility.Map.Role.NPCInitializerActorInit);
            }
            var actInit = (CSUtility.Component.ActorInitBase)System.Activator.CreateInstance(initType);
            actInit.Read(attribute);
            Initialize(actInit);

            return true;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(long elapsedMillisecond)
        {
            if (Visual != null)
                Visual.Tick(this, elapsedMillisecond);

            if (mNPCInitializerActorInit.NPCData.Template != null)
            {
                if (mNPCTemplateVersion != mNPCInitializerActorInit.NPCData.Template.Version)
                {
                    mNPCTemplateVersion = mNPCInitializerActorInit.NPCData.Template.Version;

                    var scale = new SlimDX.Vector3(mNPCInitializerActorInit.NPCData.FinalScale);
                    this.Placement.SetScale(ref scale);
                }
            }
        }
        /// <summary>
        /// 获取层名称
        /// </summary>
        /// <returns>返回当前的层名称为NPCInitializer</returns>
        public override string GetLayerName()
        {
            return "NPCInitializer";
        }
    }
}
