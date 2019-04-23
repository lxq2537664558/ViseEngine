using System;

namespace CCore.Light
{
    /// <summary>
    /// 聚光灯类型的光源
    /// </summary>
    [CCore.EditorAssist.PlantAbleAttribute("灯光.聚光灯", "", "")]
    [CCore.Socket.SocketComponentInfoAttribute("聚光灯")]
    public class SpotLight : Light, EditorAssist.IPlantAbleObject
    {
        #region EditAssist
        /// <summary>
        /// 得到种植的聚光灯对象
        /// </summary>
        /// <param name="world">种植到的世界对象</param>
        /// <returns>返回种植的聚光灯</returns>
        public CCore.World.Actor GetPlantActor(CCore.World.World world)
        {
            var lightActor = new CCore.World.LightActor();
            var lightInit = new CCore.World.LightActorInit();
            lightInit.LightType = ELightType.Spot;
            lightActor.Initialize(lightInit);
            lightActor.SetPlacement(new CSUtility.Component.StandardPlacement(lightActor));

            if (mPropertyObject != null)
            {
                lightActor.Light.CopyFrom(mPropertyObject);
            }

            lightActor.ActorName = "聚光灯" + Program.GetActorIndex();
            return lightActor;
        }
        /// <summary>
        /// 获取预览用聚光灯对象，在拖动对象进入场景时显示预览的聚光灯对象
        /// </summary>
        /// <param name="world">拖动进入的世界对象</param>
        /// <returns>返回拖动对象进入场景时的预览聚光灯对象</returns>
        public CCore.World.Actor GetPreviewActor(CCore.World.World world)
        {
            if (!CCore.Program.IsActorTypeShow(world, CCore.Program.LightAssistTypeName))
                CCore.Program.SetActorTypeShow(world, CCore.Program.LightAssistTypeName, true);

            var lightActor = new CCore.World.LightActor();
            var lightInit = new CCore.World.LightActorInit();
            lightInit.LightType = ELightType.Spot;
            lightActor.Initialize(lightInit);
            lightActor.SetPlacement(new CSUtility.Component.StandardPlacement(lightActor));

            if (mPropertyObject != null)
            {
                lightActor.Light.CopyFrom(mPropertyObject);
            }

            return lightActor;
        }

        CCore.Light.SpotLight mPropertyObject = null;
        /// <summary>
        /// 获取需要显示属性的聚光灯对象
        /// </summary>
        /// <returns>返回显示属性的聚光灯对象</returns>
        public object GetPropertyShowObject()
        {
            var lightActorInit = new CCore.World.LightActorInit();
            lightActorInit.LightType = ELightType.Spot;
            var lightObject = new CCore.Light.SpotLight();
            var lightInit = new CCore.Light.LightInit();
            lightInit.LightType = lightActorInit.LightType;
            lightObject.Initialize(lightInit, null);
            mPropertyObject = lightObject;
            return mPropertyObject;
        }

        #endregion

        /// <summary>
        /// 只读属性，光源类型为聚光灯
        /// </summary>
        public override ELightType LightType
        {
            get
            {
                return ELightType.Spot;
            }
        }
        /// <summary>
        /// 聚光灯的构造函数
        /// </summary>
        public SpotLight()
        {
            CreateLightProxy(LightType);
        }

        double mHeight = 10;
        /// <summary>
        /// 聚光灯的高度，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Height")]
        [System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Editor.Editor_ValueWithRange(1, 50)]
        public double Height
        {
            get
            {
                mHeight = Scale.Y;
                return mHeight;
            }
            set
            {
                mHeight = value;

                SlimDX.Vector3 scale = Scale;
                scale.Y = (float)mHeight;
                Scale = scale;
                OutterFOV = OutterFOV;
                OnPropertyChanged("Height");
            }
        }
        /// <summary>
        /// 聚光灯的半影角度，范围为0-1，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("InnerFOVPercent")]
        //[System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("半影角度")]
        [CSUtility.Editor.Editor_ValueWithRange(0, 1)]
        public double InnerFOVPercent
        {
            get
            {
                return base.SpotInnerFOVPercent;
            }
            set
            {
                base.SpotInnerFOVPercent = (float)value;
                OnPropertyChanged("InnerFOVPercent");
            }
        }
        double mOutterFOV = 1;
        /// <summary>
        /// 聚光灯的圆锥角度，范围为10-179，可在编辑内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("OutterFOV")]
        //[System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("圆锥角度")]
        [CSUtility.Editor.Editor_ValueWithRange(10, 179)]
        public double OutterFOV
        {
            get
            {
                return mOutterFOV * 180.0 / System.Math.PI;
            }
            set
            {
                mOutterFOV = (float)(value * System.Math.PI / 180.0);

                SlimDX.Vector3 scale = Scale;
                double hafFOV = mOutterFOV / 2.0;
                scale.X = scale.Z = scale.Y * (float)Math.Tan(hafFOV) * 2.0f;
                Scale = scale;
                OnPropertyChanged("OutterFOV");
            }
        }
        /// <summary>
        /// 聚光灯的Pitch值，可在编辑期内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Pitch")]
        [System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.DisplayName("Pitch")]
        [CSUtility.Editor.Editor_ValueWithRange(-System.Math.PI / 2, System.Math.PI / 2)]
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
        /// 聚光灯的Roll值，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Roll")]
        [System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.DisplayName("Roll")]
        [CSUtility.Editor.Editor_ValueWithRange(-System.Math.PI / 2, System.Math.PI / 2)]
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
        /// 计算聚光灯的光照范围
        /// </summary>
        /// <returns>返回聚光灯的光照范围</returns>
        public override float CalcLightRange()
        {
            Height = base.CalcLightRange();
            return (float)Height;
        }
        /// <summary>
        /// 聚光灯的AABB包围盒
        /// </summary>
        /// <param name="vMin">聚光灯的最小顶点</param>
        /// <param name="vMax">聚光灯的最大顶点</param>
        public override void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
            vMin.Y = -1;
            vMin.X = -1;
            vMin.Z = -1;

            vMax.Y = 0;
            vMax.X = 1;
            vMax.Z = 1;
        }
    }
}
