using CCore.Camera;
using CCore.Component;
using CCore.Graphics;
using CCore.World;
using CSUtility.Component;
using SlimDX;
using System.ComponentModel;

namespace CCore.Light
{
    /// <summary>
    /// 方向光类型的光源
    /// </summary>
    [CCore.EditorAssist.PlantAbleAttribute("灯光.方向光", "", "")]
    [CCore.Socket.SocketComponentInfoAttribute("方向光")]
    public class DirLight : Light, EditorAssist.IPlantAbleObject
    {
        #region EditAssist
        /// <summary>
        /// 得到种植的方向光对象
        /// </summary>
        /// <param name="world">种植到的世界对象</param>
        /// <returns>返回种植的方向光</returns>
        public CCore.World.Actor GetPlantActor(CCore.World.World world)
        {
            var lightActor = new CCore.World.LightActor();
            var lightInit = new CCore.World.LightActorInit();
            lightInit.LightType = ELightType.Dir;
            lightActor.Initialize(lightInit);
            lightActor.SetPlacement(new CSUtility.Component.StandardPlacement(lightActor));

            if (mPropertyObject != null)
            {
                lightActor.Light.CopyFrom(mPropertyObject);
            }

            lightActor.ActorName = "方向光" + Program.GetActorIndex();
            return lightActor;
        }
        /// <summary>
        /// 获取预览用方向光对象，在拖动对象进入场景时显示预览的方向光对象
        /// </summary>
        /// <param name="world">拖动进入的世界对象</param>
        /// <returns>返回拖动对象进入场景时的预览方向光对象</returns>
        public CCore.World.Actor GetPreviewActor(CCore.World.World world)
        {
            if (!CCore.Program.IsActorTypeShow(world, CCore.Program.LightAssistTypeName))
                CCore.Program.SetActorTypeShow(world, CCore.Program.LightAssistTypeName, true);

            var lightActor = new CCore.World.LightActor();
            var lightInit = new CCore.World.LightActorInit();
            lightInit.LightType = ELightType.Dir;
            lightActor.Initialize(lightInit);
            lightActor.SetPlacement(new CSUtility.Component.StandardPlacement(lightActor));

            if (mPropertyObject != null)
            {
                lightActor.Light.CopyFrom(mPropertyObject);
            }

            return lightActor;
        }

        CCore.Light.DirLight mPropertyObject = null;
        /// <summary>
        /// 获取需要显示属性的方向光对象
        /// </summary>
        /// <returns>返回显示属性的方向光对象</returns>
        public object GetPropertyShowObject()
        {
            var lightActorInit = new CCore.World.LightActorInit();
            lightActorInit.LightType = ELightType.Dir;
            var lightObject = new CCore.Light.DirLight();
            var lightInit = new CCore.Light.LightInit();
            lightInit.LightType = lightActorInit.LightType;
            lightObject.Initialize(lightInit, null);
            mPropertyObject = lightObject;
            return mPropertyObject;
        }

        #endregion
        /// <summary>
        /// 只读属性，灯光类型为方向光
        /// </summary>
        public override ELightType LightType
        {
            get
            {
                return ELightType.Dir;
            }
        }

        CCore.Mesh.Mesh mIconMesh;
        CCore.Mesh.Mesh mDirMesh;
        /// <summary>
        /// 构造函数，设置方向光可进行鼠标点击操作
        /// </summary>
        public DirLight()
        {
            CreateLightProxy(LightType);
        }
        /// <summary>
        /// 删除实例对象，释放内存
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();

            if(mIconMesh != null)
            {
                mIconMesh.Cleanup();
                mIconMesh = null;
            }
            if(mDirMesh != null)
            {
                mDirMesh.Cleanup();
                mDirMesh = null;
            }
        }
        /// <summary>
        /// 方向光的Yaw值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Yaw")]
        [System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.DisplayName("Yaw")]
        [CSUtility.Editor.Editor_ValueWithRange(-System.Math.PI, System.Math.PI)]
        [Browsable(false)]
        public double Yaw
        {
            get
            {
                float yaw = 0, pitch = 0, roll = 0;
                Quat.GetYawPitchRoll(out yaw, out pitch, out roll);
                return yaw;
            }
            set
            {
                float yaw = 0, pitch = 0, roll = 0;
                Quat.GetYawPitchRoll(out yaw, out pitch, out roll);
                Quat = SlimDX.Quaternion.RotationYawPitchRoll((float)value, pitch, roll);

                OnPropertyChanged("Yaw");
            }
        }
        /// <summary>
        /// 方向光的Pitch值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Pitch")]
        [System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.DisplayName("Pitch")]
        [CSUtility.Editor.Editor_ValueWithRange(-System.Math.PI / 2, System.Math.PI / 2)]
        [Browsable(false)]
        public double Pitch
        {
            get
            {
                float yaw = 0, pitch = 0, roll = 0;
                Quat.GetYawPitchRoll(out yaw, out pitch, out roll);
                return pitch;
            }
            set
            {
                float yaw = 0, pitch = 0, roll = 0;
                Quat.GetYawPitchRoll(out yaw, out pitch, out roll);
                Quat = SlimDX.Quaternion.RotationYawPitchRoll(yaw, (float)value, roll);
                OnPropertyChanged("Pitch");
            }
        }
        /// <summary>
        /// 方向光的Roll值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Roll")]
        [System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.DisplayName("Roll")]
        [CSUtility.Editor.Editor_ValueWithRange(-System.Math.PI / 2, System.Math.PI / 2)]
        [Browsable(false)]
        public double Roll
        {
            get
            {
                float yaw = 0, pitch = 0, roll = 0;
                Quat.GetYawPitchRoll(out yaw, out pitch, out roll);
                return roll;
            }
            set
            {
                float yaw = 0, pitch = 0, roll = 0;
                Quat.GetYawPitchRoll(out yaw, out pitch, out roll);
                Quat = SlimDX.Quaternion.RotationYawPitchRoll(yaw, pitch, (float)value);
                OnPropertyChanged("Roll");
            }
        }
        /// <summary>
        /// 设置某一类型的光源可进行鼠标点选
        /// </summary>
        /// <param name="t">光源类型</param>
        public override void CreateLightProxy(ELightType t)
        {
            base.CreateLightProxy(t);

            var meshInit = new CCore.Mesh.MeshInit();
            meshInit.MeshTemplateID = CSUtility.Support.IFileConfig.DirLightMeshTemplate;
            mIconMesh = new Mesh.Mesh();
            mIconMesh.Initialize(meshInit, null);

            meshInit = new CCore.Mesh.MeshInit();
            var meshInitPart = new CCore.Mesh.MeshInitPart();
            meshInitPart.MeshName = CSUtility.Support.IFileConfig.ArrowMesh;
            meshInitPart.Techs.Add(CSUtility.Support.IFileConfig.DirLightArrowTech);
            meshInitPart.Techs.Add(CSUtility.Support.IFileConfig.DirLightArrowTech);
            meshInit.MeshInitParts.Add(meshInitPart);
            mDirMesh = new Mesh.Mesh();
            mDirMesh.Initialize(meshInit, null);
            mDirMesh.Layer = RLayer.RL_Translucent;

            if(HostActor != null)
            {
                var hitProxy = CCore.Graphics.HitProxyMap.Instance.GenHitProxy(HostActor.Id);
                mIconMesh.SetHitProxyAll(hitProxy);
                mDirMesh.SetHitProxyAll(hitProxy);
            }
        }
        /// <summary>
        /// 将该对象提交到渲染环境
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">该对象的位置矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(REnviroment renderEnv, ref Matrix matrix, CameraObject eye)
        {
            if(Visible)
            {
                unsafe
                {
                    fixed (SlimDX.Matrix* pinMatrix = &matrix)
                    {
                        DllImportAPI.vDSRenderEnv_CommitDSLighting(renderEnv.DSRenderEnv, (int)mGroup, mInner, pinMatrix, eye.CommitCamera);
                    }
                }

                if (mIconMesh != null)
                {
                    mIconMesh.Commit(renderEnv, ref matrix, eye);
                }
                if(mDirMesh != null)
                {
                    mDirMesh.Commit(renderEnv, ref matrix, eye);
                }
            }
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="host">所属Actor</param>
        /// <param name="elapsedMillisecond">每帧之间的时间间隔</param>
        public override void Tick(ActorBase host, long elapsedMillisecond)
        {
            base.Tick(host, elapsedMillisecond);

            if (mIconMesh != null)
            {
                mIconMesh.Tick(host, elapsedMillisecond);
            }
            if (mDirMesh != null)
            {
                mDirMesh.Tick(host, elapsedMillisecond);
            }
        }
    }
}
