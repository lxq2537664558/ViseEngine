using System;

namespace CCore.Light
{
    /// <summary>
    /// 点光源的类
    /// </summary>
    [CCore.EditorAssist.PlantAbleAttribute("灯光.点光源", "", "")]
    [CCore.Socket.SocketComponentInfoAttribute("点光源")]
    public class PointLight : Light, EditorAssist.IPlantAbleObject
    {
        #region EditAssist
        /// <summary>
        /// 得到种植的点光源对象
        /// </summary>
        /// <param name="world">种植到的世界对象</param>
        /// <returns>返回种植的点光源</returns>
        public CCore.World.Actor GetPlantActor(CCore.World.World world)
        {
            var lightActor = new CCore.World.LightActor();
            var lightInit = new CCore.World.LightActorInit();
            lightInit.LightType = ELightType.Point;
            lightActor.Initialize(lightInit);
            lightActor.SetPlacement(new CSUtility.Component.StandardPlacement(lightActor));

            if (mPropertyObject != null)
            {
                lightActor.Light.CopyFrom(mPropertyObject);
            }

            lightActor.ActorName = "点光源" + Program.GetActorIndex();
            return lightActor;
        }
        /// <summary>
        /// 获取预览用点光源对象，在拖动对象进入场景时显示预览的点光源对象
        /// </summary>
        /// <param name="world">拖动进入的世界对象</param>
        /// <returns>返回拖动对象进入场景时的预览点光源对象</returns>
        public CCore.World.Actor GetPreviewActor(CCore.World.World world)
        {
            if (!CCore.Program.IsActorTypeShow(world, CCore.Program.LightAssistTypeName))
                CCore.Program.SetActorTypeShow(world, CCore.Program.LightAssistTypeName, true);

            var lightActor = new CCore.World.LightActor();
            var lightInit = new CCore.World.LightActorInit();
            lightInit.LightType = ELightType.Point;
            lightActor.Initialize(lightInit);
            lightActor.SetPlacement(new CSUtility.Component.StandardPlacement(lightActor));

            if (mPropertyObject != null)
            {
                lightActor.Light.CopyFrom(mPropertyObject);
            }

            return lightActor;
        }

        CCore.Light.PointLight mPropertyObject = null;
        /// <summary>
        /// 获取需要显示属性的点光源对象
        /// </summary>
        /// <returns>返回显示属性的点光源对象</returns>
        public object GetPropertyShowObject()
        {
            var lightActorInit = new CCore.World.LightActorInit();
            lightActorInit.LightType = ELightType.Point;
            var lightObject = new CCore.Light.PointLight();
            var lightInit = new CCore.Light.LightInit();
            lightInit.LightType = lightActorInit.LightType;
            lightObject.Initialize(lightInit, null);
            mPropertyObject = lightObject;
            return mPropertyObject;
        }

        #endregion
        /// <summary>
        /// 只读属性，光源类型为点光源
        /// </summary>
        public override ELightType LightType
        {
            get { return ELightType.Point; }
        }
        /// <summary>
        /// 点光源的构造函数，创建点光源
        /// </summary>
        public PointLight()
        {
            CreateLightProxy(LightType);
        }

        double mRadius = 10;
        /// <summary>
        /// 光源的半径，范围为1-50，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("Radius")]
        [System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Editor.Editor_ValueWithRange(1, 50)]
        public double Radius
        {
            get
            {
                mRadius = Scale.Y;
                return mRadius;
            }
            set
            {
                mRadius = value;
                Scale = new SlimDX.Vector3((float)mRadius);
                OnPropertyChanged("Radius");
            }
        }
        /// <summary>
        /// 点光源的半影角度，范围为0-1，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DataValueAttribute("InnerPercent")]
        //[System.ComponentModel.Category("光源属性")]
        [System.ComponentModel.Category("常用")]
        [System.ComponentModel.DisplayName("半影角度")]
        [CSUtility.Editor.Editor_ValueWithRange(0, 1)]
        public double InnerPercent
        {
            get
            {
                return base.PointInnerPercent;
            }
            set
            {
                base.PointInnerPercent = (float)value;
                OnPropertyChanged("InnerPercent");
            }
        }
        /// <summary>
        /// 计算点光源的光照范围
        /// </summary>
        /// <returns>返回点光源的光照范围</returns>
        public override float CalcLightRange()
        {
            Radius = base.CalcLightRange();
            return (float)Radius;
        }
        /// <summary>
        /// 点光源的AABB包围盒
        /// </summary>
        /// <param name="vMin">点光源的最小顶点</param>
        /// <param name="vMax">点光源的最大顶点</param>
        public override void GetAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
            vMin = -SlimDX.Vector3.UnitXYZ;
            vMax = SlimDX.Vector3.UnitXYZ;
        }
    }
}
