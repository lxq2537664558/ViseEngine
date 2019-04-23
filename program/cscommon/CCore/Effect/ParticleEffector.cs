using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.Particle
{
    /// <summary>
    /// 效果器枚举类型
    /// </summary>
    public enum EffectorSlot
    {
        [Description("速度")]
        Velocity,
        [Description("外力")]
        Force,
        [Description("颜色")]
        Color,
        [Description("缩放")]
        Scale,
        [Description("旋转")]
        Rotation,
        [Description("繁衍")]
        Spawn,
        [Description("盘旋")]
        Orbit,
        COUNT
    }
    /// <summary>
    /// 粒子系统坐标系类型枚举
    /// </summary>
    public enum CoordinateSpaceCN
	{
        [Description("世界")]
		World,
        [Description("本地")]
		Local,
        [Description("本地带方向")]
        LocalWithDirection,
	}
    /// <summary>
    /// 粒子模型形状
    /// </summary>
	public enum ParticleEmitterShapeCN
	{
        [Description("默认")]
        Point,      //默认,
        [Description("喇叭")]
        Cone,       //喇叭,
        [Description("盒子")]
        Box,        //盒子,
        [Description("圆球")]
        Sphere,     //圆球,
        [Description("模型")]
        Mesh,       //模型,
	}
    /// <summary>
    /// 粒子系统转换的类
    /// </summary>
    public class ParticleObjConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// 返回此转换器是否可以使用指定的上下文将该对象转换为指定的类型。
        /// </summary>
        /// <param name="context">一个 System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
        /// <param name="destinationType">一个 System.Type，表示要转换到的类型。</param>
        /// <returns>转换成功返回true，否则返回false</returns>

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }
        /// <summary>
        /// 返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型。
        /// </summary>
        /// <param name="context">提供格式上下文。</param>
        /// <param name="sourceType">要转换的类型。</param>
        /// <returns>转换成功返回true，否则返回false</returns>
 
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return base.CanConvertFrom(context, sourceType);
        }
        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型。
        /// </summary>
        /// <param name="context">提供格式上下文。</param>
        /// <param name="culture">如果传递 null，则采用当前区域性。</param>
        /// <param name="value">要转换的 System.Object。</param>
        /// <param name="destinationType"> value 参数要转换成的 System.Type。</param>
        /// <returns>表示转换的 value 的 System.Object。</returns>   
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的对象转换为此转换器的类型。
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <param name="culture">用作当前区域性的 System.Globalization.CultureInfo</param>
        /// <param name="value">要转换的 System.Object。</param>
        /// <returns>表示转换的 value 的 System.Object。</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }
    }
    /// <summary>
    /// 粒子特效效果器
    /// </summary>
    [TypeConverter(typeof(ParticleObjConverter))]
    public class ParticleEffector : CSUtility.Support.XndSaveLoadProxy
    {
        //protected int mId;
        //[Browsable(false)]
        //public int Id
        //{
        //    get { return mId; }
        //}
        /// <summary>
        /// 粒子特效的地址
        /// </summary>
        protected IntPtr mInner;
        /// <summary>
        /// 只读属性，粒子特效的地址
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public IntPtr Inner
        {
            get { return mInner; }
        }
        /// <summary>
        /// 只读属性，效果器的数量
        /// </summary>
        [Browsable(false)]
        public virtual EffectorSlot Slot
        {
            get { return EffectorSlot.COUNT; }
        }
        /// <summary>
        /// 粒子表现模式
        /// </summary>
        public enum EffectingMode
        {
            EffectByLiftTime,
            EffectBySpeed,
            EffectByLifeTimeAndSpeed,
        }
        /// <summary>
        /// 当前粒子系统效果器的模式，默认为EffectByLiftTime，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Browsable(true), Category("基本"), DisplayName("模式")]
        public EffectingMode EffectMode
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (EffectingMode)DllImportAPI.ParticleEffector_GetEffectMode(Inner);
                }

                return EffectingMode.EffectByLiftTime;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEffector_SetEffectMode(Inner, (int)value);
                }
            }
        }
        /// <summary>
        /// 是否启用效果器
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Browsable(false), Category("基本"), DisplayName("启用")]
        public bool Enable
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEffector_GetEnable(Inner) == 0) ? false : true;
                }

                return false;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEffector_SetEnable(Inner, value);
                }
            }
        }
        /// <summary>
        /// 复制指定的粒子到该实例
        /// </summary>
        /// <param name="src">需要复制的源粒子</param>
        public virtual void Clone(ParticleEffector src)
        {
            EffectMode = src.EffectMode;
            Enable = src.Enable;
        }

        //public IParticleEffector()
        //{

        //}
        //public IParticleEffector(IntPtr inner)
        //{
        //    unsafe
        //    {
        //        IDllImportAPI.ParticleEffector_AddRef(inner);
        //        mInner = inner;
        //    }
        //}
        /// <summary>
        /// 析构函数，释放内存
        /// </summary>
        ~ParticleEffector()
        {
            Cleanup();
        }
        /// <summary>
        /// 清除该实例
        /// </summary>
        public void Cleanup()
        {
            unsafe
            {
                if (mInner != IntPtr.Zero)
                {
                    //System.Diagnostics.Debug.WriteLine("---ParticleEffector Cleanup(" + mInner.ToString() + ")---");
                    DllImportAPI.ParticleEffector_Release(mInner);
                    mInner = IntPtr.Zero;
                }
            }
        }
    }
    /// <summary>
    /// 粒子特效的速度模拟器
    /// </summary>
    [TypeConverter(typeof(ParticleObjConverter))]
    public class ParticleEffector_Velocity : ParticleEffector
    {
        /// <summary>
        /// 只读属性，粒子速度效果器
        /// </summary>
        [Category("基本"), DisplayName("类型"), ReadOnly(true)]
        public override EffectorSlot Slot
        {
            get { return EffectorSlot.Velocity; }
        }

        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[CSUtility.Editor.Editor_VectorEditor]
        //public SlimDX.Vector3 Velocity
        //{
        //    get
        //    {
        //        unsafe
        //        {
        //            if (Inner != IntPtr.Zero)
        //            {
        //                SlimDX.Vector3 data = new SlimDX.Vector3();
        //                IDllImportAPI.ParticleVelocityEffector_GetVelocity(Inner, &data);
        //                return data;
        //            }
        //        }

        //        return SlimDX.Vector3.Zero;
        //    }
        //    set
        //    {
        //        unsafe
        //        {
        //            if (Inner != IntPtr.Zero)
        //                IDllImportAPI.ParticleVelocityEffector_SetVelocity(Inner, &value);
        //        }
        //    }
        //}

        CCore.Support.ScalarVariable mVelocityX = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// X方向的速度，可在编辑器内调整，默认为0
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("速度X")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable VelocityX
        {
            get { return mVelocityX; }
            set { mVelocityX = value; }
        }

        CCore.Support.ScalarVariable mVelocityY = new CCore.Support.ScalarVariable(10);
        /// <summary>
        /// Y方向的速度，可在编辑器内调整，默认为10
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("速度Y")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable VelocityY
        {
            get { return mVelocityY; }
            set { mVelocityY = value; }
        }

        CCore.Support.ScalarVariable mVelocityZ = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// Z方向的速度，可在编辑器内调整，默认为0
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("速度Z")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable VelocityZ
        {
            get { return mVelocityZ; }
            set { mVelocityZ = value; }
        }
        /// <summary>
        /// 加添加粒子效果器时调用，创建粒子的速度效果器
        /// </summary>
        public ParticleEffector_Velocity()
        {
            unsafe
            {
                mInner = DllImportAPI.ParticleEffector_New_Velocity();
                //System.Diagnostics.Debug.WriteLine("---IParticleEffector_Velocity New(" + mInner.ToString() + ")---");

                VelocityX.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityXPtr(mInner);
                VelocityY.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityYPtr(mInner);
                VelocityZ.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityZPtr(mInner);
            }
        }
        //public IParticleEffector_Velocity(IntPtr inner)
        //    : base(inner)
        //{
        //}
        /// <summary>
        /// 克隆指定粒子的速度效果器
        /// </summary>
        /// <param name="src">要复制的源数据</param>
        public override void Clone(ParticleEffector src)
        {
            base.Clone(src);

            var srcT = src as ParticleEffector_Velocity;
            if (srcT != null)
            {
                VelocityX.Clone(srcT.VelocityX);
                VelocityY.Clone(srcT.VelocityY);
                VelocityZ.Clone(srcT.VelocityZ);

                VelocityX.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityXPtr(mInner);
                VelocityX.SetValueToIntptr(VelocityX.ScalarVariablePtr);
                VelocityY.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityYPtr(mInner);
                VelocityY.SetValueToIntptr(VelocityY.ScalarVariablePtr);
                VelocityZ.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityZPtr(mInner);
                VelocityZ.SetValueToIntptr(VelocityZ.ScalarVariablePtr);
            }
        }
        /// <summary>
        /// 读取粒子速度效果器的文件
        /// </summary>
        /// <param name="xndAtt">粒子的速度效果器文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (base.Read(xndAtt) == false)
                return false;

            VelocityX.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityXPtr(mInner);
            VelocityX.SetValueToIntptr(VelocityX.ScalarVariablePtr);
            VelocityY.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityYPtr(mInner);
            VelocityY.SetValueToIntptr(VelocityY.ScalarVariablePtr);
            VelocityZ.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityZPtr(mInner);
            VelocityZ.SetValueToIntptr(VelocityZ.ScalarVariablePtr);

            return true;
        }
        /// <summary>
        /// 复制源数据到实例
        /// </summary>
        /// <param name="srcData">需要复制的源数据</param>
        /// <returns>复制成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            if (!base.CopyFrom(srcData))
                return false;

            VelocityX.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityXPtr(mInner);
            VelocityX.SetValueToIntptr(VelocityX.ScalarVariablePtr);
            VelocityY.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityYPtr(mInner);
            VelocityY.SetValueToIntptr(VelocityY.ScalarVariablePtr);
            VelocityZ.ScalarVariablePtr = DllImportAPI.ParticleVelocityEffector_GetVelocityZPtr(mInner);
            VelocityZ.SetValueToIntptr(VelocityZ.ScalarVariablePtr);

            return true;
        }
    }
    /// <summary>
    /// 粒子特效的外力效果器
    /// </summary>
    [TypeConverter(typeof(ParticleObjConverter))]
    public class ParticleEffector_Force : ParticleEffector
    {
        /// <summary>
        /// 只读属性，粒子外力效果器
        /// </summary>
        [Browsable(true), Category("基本"), DisplayName("类型"), ReadOnly(true)]
        public override EffectorSlot Slot
        {
            get { return EffectorSlot.Force; }
        }
        /// <summary>
        /// 粒子受力方向
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_VectorEditor]
        [Category("基本"), DisplayName("方向")]
        public SlimDX.Vector3 AccelerationDir
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        SlimDX.Vector3 data = new SlimDX.Vector3();
                        DllImportAPI.ParticleForceEffector_GetAccelerationDir(Inner, &data);
                        return data;
                    }
                }

                return SlimDX.Vector3.Zero;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleForceEffector_SetAccelerationDir(Inner, &value);
                }
            }
        }

        CCore.Support.ScalarVariable mAcceleration = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 粒子效果器外力类型和大小
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("力")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable Acceleration
        {
            get { return mAcceleration; }
            set { mAcceleration = value; }
        }
        //public float Acceleration
        //{
        //    get
        //    {
        //        unsafe
        //        {
        //            if (Inner != IntPtr.Zero)
        //            {
        //                return IDllImportAPI.ParticleForceEffector_GetAcceleration(Inner);
        //            }
        //        }

        //        return 0;
        //    }
        //    set
        //    {
        //        unsafe
        //        {
        //            if(Inner != IntPtr.Zero)
        //                IDllImportAPI.ParticleForceEffector_SetAcceleration(Inner, value);
        //        }
        //    }
        //}
        /// <summary>
        /// 粒子外力效果器的延速度方向
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("延速度方向")]
        public bool IsRadialDirection
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        return DllImportAPI.ParticleForceEffector_GetIsRadialDirection(Inner) != 0 ? true : false;
                    }
                }

                return false;
            }
            set
            {
                unsafe
                {
                    if(Inner != IntPtr.Zero)
                        DllImportAPI.ParticleForceEffector_SetIsRadialDirection(Inner, value);
                }
            }
        }
        /// <summary>
        /// 粒子外力效果器的构造函数，创建外力效果器
        /// </summary>
        public ParticleEffector_Force()
        {
            mInner = DllImportAPI.ParticleEffector_New_Force();
            //System.Diagnostics.Debug.WriteLine("---IParticleEffector_Force New(" + mInner.ToString() + ")---");

            Acceleration.ScalarVariablePtr = DllImportAPI.ParticleForceEffector_GetAccelerationPtr(mInner);
        }
        //public IParticleEffector_Force(IntPtr inner)
        //    : base(inner)
        //{
        //}
        /// <summary>
        /// 克隆粒子外力效果器
        /// </summary>
        /// <param name="src">克隆的外力效果器源数据</param>
        public override void Clone(ParticleEffector src)
        {
            base.Clone(src);

            var srcT = src as ParticleEffector_Force;
            if (srcT != null)
            {
                Acceleration.Clone(srcT.Acceleration);
                AccelerationDir = srcT.AccelerationDir;
                IsRadialDirection = srcT.IsRadialDirection;

                Acceleration.ScalarVariablePtr = DllImportAPI.ParticleForceEffector_GetAccelerationPtr(mInner);
                Acceleration.SetValueToIntptr(Acceleration.ScalarVariablePtr);
            }
        }
        /// <summary>
        /// 读取粒子外力效果器数据
        /// </summary>
        /// <param name="xndAtt">粒子外力效果器的数据文件</param>
        /// <returns>成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (base.Read(xndAtt) == false)
                return false;

            Acceleration.ScalarVariablePtr = DllImportAPI.ParticleForceEffector_GetAccelerationPtr(mInner);
            Acceleration.SetValueToIntptr(Acceleration.ScalarVariablePtr);

            return true;
        }
        /// <summary>
        /// 从源数据复制粒子外力效果器
        /// </summary>
        /// <param name="srcData">需要复制的源数据</param>
        /// <returns>成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            if (!base.CopyFrom(srcData))
                return false;

            Acceleration.ScalarVariablePtr = DllImportAPI.ParticleForceEffector_GetAccelerationPtr(mInner);
            Acceleration.SetValueToIntptr(Acceleration.ScalarVariablePtr);
            return true;
        }
    }
    /// <summary>
    /// 粒子特效的颜色效果器
    /// </summary>
    [TypeConverter(typeof(ParticleObjConverter))]
    public class ParticleEffector_Color : ParticleEffector
    {
        /// <summary>
        /// 只读属性，粒子颜色效果器
        /// </summary>
        [Category("基本"), DisplayName("类型"), ReadOnly(true)]
        public override EffectorSlot Slot
        {
            get { return EffectorSlot.Color; }
        }
        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[Category("基本"), DisplayName("颜色起始")]
        //[CSUtility.Editor.Editor_VectorEditor]
        //public SlimDX.Vector4 ColorBgn
        //{
        //    get
        //    {
        //        unsafe
        //        {
        //            if(Inner != IntPtr.Zero)
        //            {
        //                SlimDX.Vector4 data = new SlimDX.Vector4();
        //                IDllImportAPI.ParticleColorEffector_GetValueBegin(Inner, &data);
        //                return data;
        //            }
        //        }

        //        return new SlimDX.Vector4();
        //    }
        //    set
        //    {
        //        unsafe
        //        {
        //            if (Inner != IntPtr.Zero)
        //            {
        //                IDllImportAPI.ParticleColorEffector_SetValueBegin(Inner, &value);
        //            }
        //        }
        //    }
        //}
        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[Category("基本"), DisplayName("颜色结束")]
        //[CSUtility.Editor.Editor_VectorEditor]
        //public SlimDX.Vector4 ColorEnd
        //{
        //    get
        //    {
        //        unsafe
        //        {
        //            if (Inner != IntPtr.Zero)
        //            {
        //                SlimDX.Vector4 data = new SlimDX.Vector4();
        //                IDllImportAPI.ParticleColorEffector_GetValueEnd(Inner, &data);
        //                return data;
        //            }
        //        }
        //        return new SlimDX.Vector4();
        //    }
        //    set
        //    {
        //        unsafe
        //        {
        //            if (Inner != IntPtr.Zero)
        //                IDllImportAPI.ParticleColorEffector_SetValueEnd(Inner, &value);
        //        }
        //    }
        //}

        CCore.Support.ScalarVariable mSpeedRange = new CCore.Support.ScalarVariable(0, 0);
        /// <summary>
        /// 粒子颜色效果器的速度范围，可在编辑器中更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("速度范围")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable SpeedRange
        {
            get { return mSpeedRange; }
            set { mSpeedRange = value; }
        }

        CCore.Support.ScalarVariable mColorR = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子颜色效果器的R值，默认为1，可在编辑器内修改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("颜色R")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable ColorR
        {
            get { return mColorR; }
            set { mColorR = value; }
        }
        CCore.Support.ScalarVariable mColorG = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子颜色效果器的G值，默认为1，可在编辑器内修改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("颜色G")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable ColorG
        {
            get { return mColorG; }
            set { mColorG = value; }
        }
        CCore.Support.ScalarVariable mColorB = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子颜色效果器的B值，默认为1，可在编辑器内修改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("颜色B")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable ColorB
        {
            get { return mColorB; }
            set { mColorB = value; }
        }
        CCore.Support.ScalarVariable mColorA = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子颜色效果器的A值（透明度），默认为1，可在编辑器内修改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("颜色A")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable ColorA
        {
            get { return mColorA; }
            set { mColorA = value; }
        }
        /// <summary>
        /// 粒子颜色效果器的构造函数，用于创建颜色效果器
        /// </summary>
        public ParticleEffector_Color()
        {
            unsafe
            {
                mInner = DllImportAPI.ParticleEffector_New_Color();
                //System.Diagnostics.Debug.WriteLine("---IParticleEffector_Color New(" + mInner.ToString() + ")---");

                SpeedRange.VariableType = CCore.Support.enVariableType.ConstantRange;
                SpeedRange.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetInSpeedPtr(mInner);

                //ColorR.VariableType = CSharp.enVariableType.Constant;
                ColorR.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorRPtr(mInner);
                //ColorG.VariableType = CSharp.enVariableType.Constant;
                ColorG.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorGPtr(mInner);
                //ColorB.VariableType = CSharp.enVariableType.Constant;
                ColorB.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorBPtr(mInner);
                //ColorA.VariableType = CSharp.enVariableType.Constant;
                ColorA.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorAPtr(mInner);
            }
        }
        //public IParticleEffector_Color(IntPtr inner)
        //    : base(inner)
        //{
        //}
        /// <summary>
        /// 克隆粒子颜色效果器
        /// </summary>
        /// <param name="src">需要克隆的颜色效果器源数据</param>
        public override void Clone(ParticleEffector src)
        {
            base.Clone(src);

            var srcT = src as ParticleEffector_Color;
            if (srcT != null)
            {
                SpeedRange.Clone(srcT.SpeedRange);
                ColorR.Clone(srcT.ColorR);
                ColorG.Clone(srcT.ColorG);
                ColorB.Clone(srcT.ColorB);
                ColorA.Clone(srcT.ColorA);

                SpeedRange.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetInSpeedPtr(mInner);
                SpeedRange.SetValueToIntptr(SpeedRange.ScalarVariablePtr);

                ColorR.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorRPtr(mInner);
                ColorR.SetValueToIntptr(ColorR.ScalarVariablePtr);
                ColorG.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorGPtr(mInner);
                ColorG.SetValueToIntptr(ColorG.ScalarVariablePtr);
                ColorB.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorBPtr(mInner);
                ColorB.SetValueToIntptr(ColorB.ScalarVariablePtr);
                ColorA.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorAPtr(mInner);
                ColorA.SetValueToIntptr(ColorA.ScalarVariablePtr);
            }
        }
        /// <summary>
        /// 读取粒子的颜色效果器文件
        /// </summary>
        /// <param name="xndAtt">粒子的颜色效果器文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (base.Read(xndAtt) == false)
                return false;

            SpeedRange.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetInSpeedPtr(mInner);
            SpeedRange.SetValueToIntptr(SpeedRange.ScalarVariablePtr);

            ColorR.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorRPtr(mInner);
            ColorR.SetValueToIntptr(ColorR.ScalarVariablePtr);
            ColorG.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorGPtr(mInner);
            ColorG.SetValueToIntptr(ColorG.ScalarVariablePtr);
            ColorB.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorBPtr(mInner);
            ColorB.SetValueToIntptr(ColorB.ScalarVariablePtr);
            ColorA.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorAPtr(mInner);
            ColorA.SetValueToIntptr(ColorA.ScalarVariablePtr);

            return true;
        }
        /// <summary>
        /// 复制颜色效果器
        /// </summary>
        /// <param name="srcData">需要复制的颜色效果器</param>
        /// <returns>复制成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            if (!base.CopyFrom(srcData))
                return false;

            SpeedRange.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetInSpeedPtr(mInner);
            SpeedRange.SetValueToIntptr(SpeedRange.ScalarVariablePtr);

            ColorR.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorRPtr(mInner);
            ColorR.SetValueToIntptr(ColorR.ScalarVariablePtr);
            ColorG.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorGPtr(mInner);
            ColorG.SetValueToIntptr(ColorG.ScalarVariablePtr);
            ColorB.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorBPtr(mInner);
            ColorB.SetValueToIntptr(ColorB.ScalarVariablePtr);
            ColorA.ScalarVariablePtr = DllImportAPI.ParticleColorEffector_GetColorAPtr(mInner);
            ColorA.SetValueToIntptr(ColorA.ScalarVariablePtr);
            return true;
        }
    }
    /// <summary>
    /// 粒子特效的缩放效果器
    /// </summary>
    [TypeConverter(typeof(ParticleObjConverter))]
    public class ParticleEffector_Scale : ParticleEffector
    {
        /// <summary>
        /// 只读属性，粒子的缩放效果器类型
        /// </summary>
        [Category("基本"), DisplayName("类型"), ReadOnly(true)]
        public override EffectorSlot Slot
        {
            get { return EffectorSlot.Scale; }
        }
        /// <summary>
        /// 粒子缩放效果器的整体缩放，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("整体缩放")]
        public bool ScaleAll
        {
            get
            {
                if(Inner != null)
                    return DllImportAPI.ParticleScaleEffector_GetScaleAll(Inner) != 0 ? true : false;
                return false;
            }
            set
            {
                if (Inner != null)
                    DllImportAPI.ParticleScaleEffector_SetScaleAll(Inner, value);
            }
        }

        CCore.Support.ScalarVariable mEffectorScaleX = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子缩放效果器的X轴缩放，默认为1，可在编辑器内修改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("缩放X")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EffectorScaleX
        {
            get { return mEffectorScaleX; }
            set { mEffectorScaleX = value; }
        }
        CCore.Support.ScalarVariable mEffectorScaleY = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子缩放效果器的Y轴缩放，默认为1，可在编辑器内修改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("缩放Y")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EffectorScaleY
        {
            get { return mEffectorScaleY; }
            set { mEffectorScaleY = value; }
        }
        CCore.Support.ScalarVariable mEffectorScaleZ = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子缩放效果器的Z轴缩放，默认为1，可在编辑器内修改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("缩放Z")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EffectorScaleZ
        {
            get { return mEffectorScaleZ; }
            set { mEffectorScaleZ = value; }
        }
        /// <summary>
        /// 粒子缩放效果器的构造函数，用于创建粒子的缩放效果器
        /// </summary>
        public ParticleEffector_Scale()
        {
            unsafe
            {
                mInner = DllImportAPI.ParticleEffector_New_Scale();
                //System.Diagnostics.Debug.WriteLine("---IParticleEffector_Scale New(" + mInner.ToString() + ")---");

                EffectorScaleX.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleXPtr(mInner);
                EffectorScaleY.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleYPtr(mInner);
                EffectorScaleZ.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleZPtr(mInner);
            }
        }
        //public IParticleEffector_Scale(IntPtr inner)
        //    : base(inner)
        //{
        //}
        /// <summary>
        /// 克隆粒子的缩放效果器
        /// </summary>
        /// <param name="src">克隆的粒子缩放效果器</param>
        public override void Clone(ParticleEffector src)
        {
            base.Clone(src);

            var srcT = src as ParticleEffector_Scale;
            if (srcT != null)
            {
                ScaleAll = srcT.ScaleAll;
                EffectorScaleX.Clone(srcT.EffectorScaleX);
                EffectorScaleY.Clone(srcT.EffectorScaleY);
                EffectorScaleZ.Clone(srcT.EffectorScaleZ);

                EffectorScaleX.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleXPtr(mInner);
                EffectorScaleX.SetValueToIntptr(EffectorScaleX.ScalarVariablePtr);
                EffectorScaleY.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleYPtr(mInner);
                EffectorScaleY.SetValueToIntptr(EffectorScaleY.ScalarVariablePtr);
                EffectorScaleZ.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleZPtr(mInner);
                EffectorScaleZ.SetValueToIntptr(EffectorScaleZ.ScalarVariablePtr);

            }
        }
        /// <summary>
        /// 读取粒子的缩放效果器文件
        /// </summary>
        /// <param name="xndAtt">粒子的缩放效果器文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (base.Read(xndAtt) == false)
                return false;

            EffectorScaleX.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleXPtr(mInner);
            EffectorScaleX.SetValueToIntptr(EffectorScaleX.ScalarVariablePtr);
            EffectorScaleY.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleYPtr(mInner);
            EffectorScaleY.SetValueToIntptr(EffectorScaleY.ScalarVariablePtr);
            EffectorScaleZ.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleZPtr(mInner);
            EffectorScaleZ.SetValueToIntptr(EffectorScaleZ.ScalarVariablePtr);

            return true;
        }
        /// <summary>
        /// 复制粒子的缩放效果器
        /// </summary>
        /// <param name="srcData">需要复制的粒子缩放效果器数据</param>
        /// <returns>复制成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            if (!base.CopyFrom(srcData))
                return false;

            EffectorScaleX.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleXPtr(mInner);
            EffectorScaleX.SetValueToIntptr(EffectorScaleX.ScalarVariablePtr);
            EffectorScaleY.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleYPtr(mInner);
            EffectorScaleY.SetValueToIntptr(EffectorScaleY.ScalarVariablePtr);
            EffectorScaleZ.ScalarVariablePtr = DllImportAPI.ParticleScaleEffector_GetScaleZPtr(mInner);
            EffectorScaleZ.SetValueToIntptr(EffectorScaleZ.ScalarVariablePtr);
            return true;
        }
    }
    /// <summary>
    /// 粒子特效的旋转效果器
    /// </summary>
    [TypeConverter(typeof(ParticleObjConverter))]
    public class ParticleEffector_Rotation : ParticleEffector
    {
        /// <summary>
        /// 只读属性，粒子的旋转效果器
        /// </summary>
        [Category("基本"), DisplayName("类型"), ReadOnly(true)]
        public override EffectorSlot Slot
        {
            get { return EffectorSlot.Rotation; }
        }
        /// <summary>
        /// 是否使用旋转轴进行旋转，编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("使用旋转轴")]
        public bool UseAxis
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleRotationEffector_GetIsAxisOnDirection(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleRotationEffector_SetIsAxisOnDirection(Inner, value);
                }
            }
        }
        /// <summary>
        /// 粒子旋转效果器的旋转轴
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("旋转轴")]
		[CSUtility.Editor.Editor_VectorEditor]
        public SlimDX.Vector3 Axis
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        SlimDX.Vector3 data = new SlimDX.Vector3();
                        DllImportAPI.ParticleRotationEffector_GetAxis(Inner, &data);
                        return data;
                    }
                }
                return new SlimDX.Vector3();
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        var data = value;
                        data.Normalize();
                        DllImportAPI.ParticleRotationEffector_SetAxis(Inner, &data);
                    }
                }
            }
        }
        CCore.Support.ScalarVariable mVelocity = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 粒子旋转效果器的旋转速度
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("旋转速率")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable Velocity
        {
            get { return mVelocity; }
            set { mVelocity = value; }
        }
        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[Category("基本"), DisplayName("旋转速率")]
        //public float Velocity
        //{
        //    get
        //    { 
        //        unsafe
        //        {
        //            if (Inner != IntPtr.Zero)
        //            {
        //                return IDllImportAPI.ParticleRotationEffector_GetVelocity(Inner);
        //            }
        //        }
        //        return 0;
        //    }
        //    set
        //    {
        //        unsafe
        //        {
        //            if (Inner != IntPtr.Zero)
        //            {
        //                IDllImportAPI.ParticleRotationEffector_SetVelocity(Inner, value);
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 粒子旋转效果器的构造函数，用于创建粒子旋转效果器
        /// </summary>
        public ParticleEffector_Rotation()
        {
            unsafe
            {
                mInner = DllImportAPI.ParticleEffector_New_Rotation();
                //System.Diagnostics.Debug.WriteLine("---IParticleEffector_Rotation New(" + mInner.ToString() + ")---");

                Velocity.ScalarVariablePtr = DllImportAPI.ParticleRotationEffector_GetVelocityPtr(mInner);
            }
        }
        //public IParticleEffector_Rotation(IntPtr inner)
        //    : base(inner)
        //{
        //}
        /// <summary>
        /// 克隆粒子的旋转效果器
        /// </summary>
        /// <param name="src">需要克隆的粒子旋转效果器</param>
        public override void Clone(ParticleEffector src)
        {
            base.Clone(src);

            var srcT = src as ParticleEffector_Rotation;
            if (srcT != null)
            {
                UseAxis = srcT.UseAxis;
                Axis = srcT.Axis;
                Velocity.Clone(srcT.Velocity);

                Velocity.ScalarVariablePtr = DllImportAPI.ParticleRotationEffector_GetVelocityPtr(mInner);
                Velocity.SetValueToIntptr(Velocity.ScalarVariablePtr);
            }
        }
        /// <summary>
        /// 读取粒子的旋转效果器文件
        /// </summary>
        /// <param name="xndAtt">粒子的旋转效果器文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (base.Read(xndAtt) == false)
                return false;

            Velocity.ScalarVariablePtr = DllImportAPI.ParticleRotationEffector_GetVelocityPtr(mInner);
            Velocity.SetValueToIntptr(Velocity.ScalarVariablePtr);

            return true;
        }
        /// <summary>
        /// 复制粒子的旋转效果器
        /// </summary>
        /// <param name="srcData">需要进行复制的粒子旋转效果器</param>
        /// <returns>复制成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            if (!base.CopyFrom(srcData))
                return false;

            Velocity.ScalarVariablePtr = DllImportAPI.ParticleRotationEffector_GetVelocityPtr(mInner);
            Velocity.SetValueToIntptr(Velocity.ScalarVariablePtr);
            return true;
        }
    }
    /// <summary>
    /// 粒子特效的繁衍效果器
    /// </summary>
    [TypeConverter(typeof(ParticleObjConverter))]
    public class ParticleEffector_Spawn : ParticleEffector
    {
        /// <summary>
        /// 声明粒子系统进行检查时调用的委托事件
        /// </summary>
        /// <param name="vStart">起点坐标指针</param>
        /// <param name="vEnd">终点坐标指针</param>
        /// <param name="vCollisionPos">碰撞的位置指针</param>
        /// <param name="arg">参数地址</param>
        /// <returns>检查无问题返回true，否则返回false</returns>
        public delegate bool Delegate_OnParticleLineCheck(IntPtr vStart, IntPtr vEnd, IntPtr vCollisionPos, IntPtr arg);

        static Delegate_OnParticleLineCheck onLinkCheckEvent = ParticleEffector_Spawn.OnParticleLineCheck;
        /// <summary>
        /// 粒子的繁衍效果器的繁衍模式
        /// </summary>
        public enum SpawnMode
        {
            [Description("出生时发射")]
            SpawnOnBirth,
            [Description("碰撞时发射")]
            SpawnOnCollision,
            [Description("死亡时发射")]
            SpawnOnDeath,
        }
        /// <summary>
        /// 繁衍效果器包含的发射器ID列表
        /// </summary>
        protected List<Guid> mSpawnEmitterIds = new List<Guid>();
        /// <summary>
        /// 繁衍效果器包含的发射器ID列表
        /// </summary>
        [Browsable(false)] 
        [CSUtility.Support.AutoSaveLoad]
        public List<Guid> SpawnEmitterIds
        {
            get { return mSpawnEmitterIds; }
            set { mSpawnEmitterIds = value; }
        }
        /// <summary>
        /// 繁衍效果器列表
        /// </summary>
        protected List<ParticleEmitter> mSpawnEmitters = new List<ParticleEmitter>();
        /// <summary>
        /// 只读属性，繁衍的发射器名字
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public ParticleEmitter[] SpawnEmitters
        {
            get { return mSpawnEmitters.ToArray(); }
        }
        /// <summary>
        /// 只读属性，繁衍类型
        /// </summary>
        [Category("基本"), DisplayName("类型"), ReadOnly(true)]
        public override EffectorSlot Slot
        {
            get { return EffectorSlot.Spawn; }
        }
        /// <summary>
        /// 繁衍效果器的繁衍类型
        /// </summary>
        [Category("繁衍"), DisplayName("繁衍类型")]
        [CSUtility.Support.AutoSaveLoad]
        public SpawnMode Mode
        {
            get
            {
                if(Inner != null)
                    return (SpawnMode)(DllImportAPI.ParticleSpawnEffector_GetSpawnMode(Inner));
                return SpawnMode.SpawnOnDeath;
            }
            set
            {
                if (Inner != null)
                    DllImportAPI.ParticleSpawnEffector_SetSpawnMode(Inner, (int)value);
            }
        }
        /// <summary>
        /// 繁衍效果器的构造函数，用于创建粒子的繁衍效果器
        /// </summary>
        public ParticleEffector_Spawn()
        {
            unsafe
            {
                mInner = DllImportAPI.ParticleEffector_New_Spawn();
                DllImportAPI.ParticleSpawnEffector_SetLinkCheckFunction(mInner, onLinkCheckEvent);
            }
        }
        /// <summary>
        /// 克隆粒子的繁衍效果器
        /// </summary>
        /// <param name="src">需要进行克隆的粒子效果器</param>
        public override void Clone(ParticleEffector src)
        {
            base.Clone(src);

            var srcT = src as ParticleEffector_Spawn;
            if (srcT != null)
            {
                SpawnEmitterIds.Clear();
                SpawnEmitterIds.AddRange(srcT.SpawnEmitterIds);
                Mode = srcT.Mode;
            }
        }
        /// <summary>
        /// 读取粒子的繁衍效果器文件
        /// </summary>
        /// <param name="xndAtt">繁衍效果器的文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            return base.Read(xndAtt);
        }
        /// <summary>
        /// 复制粒子的繁衍效果器
        /// </summary>
        /// <param name="srcData">需要进行复制的繁衍效果器文件</param>
        /// <returns>复制成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            return base.CopyFrom(srcData);
        }
        /// <summary>
        /// 设置繁衍出来的发射器
        /// </summary>
        /// <param name="emitter">粒子发射器，根据繁衍模式进行显示</param>
        /// <returns>设置成功返回true，否则返回false</returns>
        public bool ProcessSetSpawnEmitter(CCore.Particle.ParticleEmitter emitter)
        {
            if (SpawnEmitterIds.Contains(emitter.Id))
                return false;

            SpawnEmitterIds.Add(emitter.Id);
            mSpawnEmitters.Add(emitter);
            DllImportAPI.ParticleSpawnEffector_SetEmitter(mInner, SpawnEmitterIds.Count - 1, emitter.Inner, emitter.HostModifier.Modifier);

            return true;
        }
        /// <summary>
        /// 删除繁衍效果器设置的发射器
        /// </summary>
        /// <param name="emitter">繁衍效果器对象</param>
        public void RemoveSpawnEmitter(CCore.Particle.ParticleEmitter emitter)
        {
            if (emitter == null)
                return;

            if (!SpawnEmitterIds.Contains(emitter.Id))
                return;

            var idx = SpawnEmitterIds.IndexOf(emitter.Id);
            if(mInner != null)
                DllImportAPI.ParticleSpawnEffector_RemoveEmitter(mInner, idx);

            SpawnEmitterIds.RemoveAt(idx);
            mSpawnEmitters.Remove(emitter);
        }
        /// <summary>
        /// 加载后更新繁衍效果器
        /// </summary>
        /// <param name="hostEmitter">所属发射器</param>
        public void UpdateAfterLoaded(ParticleEmitter hostEmitter)
        {
            unsafe
            {
                foreach (var emitterId in mSpawnEmitterIds)
                {
                    var emt = hostEmitter.HostModifier.HostEffector.FindEmitter(emitterId);
                    if (emt == null)
                        continue;

                    mSpawnEmitters.Add(emt);
                    if(mInner != null)
                        DllImportAPI.ParticleSpawnEffector_SetEmitter(mInner, SpawnEmitterIds.Count - 1, emt.Inner, emt.HostModifier.Modifier);
                }
            }
        }
        /// <summary>
        /// 粒子碰撞检测，在碰撞繁衍时使用
        /// </summary>
        /// <param name="vStart">射线检测起始位置</param>
        /// <param name="vEnd">射线检测结束位置</param>
        /// <param name="vCollisionPos">碰撞位置</param>
        /// <param name="arg"></param>
        /// <returns>检测到碰撞返回true，否则返回false</returns>
        static bool OnParticleLineCheck(IntPtr vStart, IntPtr vEnd, IntPtr vCollisionPos, IntPtr arg)
        {
            unsafe
            {
                SlimDX.Vector3 start = *(SlimDX.Vector3*)(vStart.ToPointer());
                SlimDX.Vector3 end = *(SlimDX.Vector3*)(vEnd.ToPointer());

                CSUtility.Support.stHitResult result = new CSUtility.Support.stHitResult();
                if(CCore.Engine.Instance.Client.MainWorld.LineCheck(ref start, ref end, ref result))
                {
                    *(SlimDX.Vector3*)(vCollisionPos.ToPointer()) = result.mHitPosition;
                    return true;
                }

                return false;
            }
        }
    }
    /// <summary>
    /// 粒子特效的盘旋效果器
    /// </summary>
    [TypeConverter(typeof(ParticleObjConverter))]
    public class ParticleEffector_Orbit : ParticleEffector
    {
        /// <summary>
        /// 只读属性，效果器的类型为盘旋
        /// </summary>
        [Category("基本"), DisplayName("类型"), ReadOnly(true)]
        public override EffectorSlot Slot
        {
            get { return EffectorSlot.Orbit; }
        }

        CCore.Support.ScalarVariable mOffsetX = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 盘旋效果X轴的偏移，默认为0，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("偏移X")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable OffsetX
        {
            get { return mOffsetX; }
            set { mOffsetX = value; }
        }

        CCore.Support.ScalarVariable mOffsetY = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 盘旋效果Y轴的偏移，默认为0，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("偏移Y")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable OffsetY
        {
            get { return mOffsetY; }
            set { mOffsetY = value; }
        }

        CCore.Support.ScalarVariable mOffsetZ = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 盘旋效果Z轴的偏移，默认为0，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("偏移Z")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable OffsetZ
        {
            get { return mOffsetZ; }
            set { mOffsetZ = value; }
        }

        CCore.Support.ScalarVariable mRotVX = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 盘旋效果X轴的转速，默认为0，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("转速X")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable RotVX
        {
            get { return mRotVX; }
            set { mRotVX = value; }
        }

        CCore.Support.ScalarVariable mRotVY = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 盘旋效果Y轴的转速，默认为0，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("转速Y")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable RotVY
        {
            get { return mRotVY; }
            set { mRotVY = value; }
        }

        CCore.Support.ScalarVariable mRotVZ = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 盘旋效果Z轴的转速，默认为0，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("转速Z")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable RotVZ
        {
            get { return mRotVZ; }
            set { mRotVZ = value; }
        }
        /// <summary>
        /// 盘旋效果器的构造函数，用于创建盘旋效果器
        /// </summary>
        public ParticleEffector_Orbit()
        {
            unsafe
            {
                mInner = DllImportAPI.ParticleEffector_New_Orbit();

                OffsetX.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetXPtr(mInner);
                OffsetY.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetYPtr(mInner);
                OffsetZ.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetZPtr(mInner);
                RotVX.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVXPtr(mInner);
                RotVY.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVYPtr(mInner);
                RotVZ.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVZPtr(mInner);
            }
        }
        /// <summary>
        /// 克隆粒子的盘旋效果器
        /// </summary>
        /// <param name="src">需要克隆的盘旋效果器源数据</param>
        public override void Clone(ParticleEffector src)
        {
            base.Clone(src);

            var srcT = src as ParticleEffector_Orbit;
            if (srcT != null)
            {
                OffsetX.Clone(srcT.OffsetX);
                OffsetY.Clone(srcT.OffsetY);
                OffsetZ.Clone(srcT.OffsetZ);
                RotVX.Clone(srcT.RotVX);
                RotVY.Clone(srcT.RotVY);
                RotVZ.Clone(srcT.RotVZ);

                OffsetX.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetXPtr(mInner);
                OffsetX.SetValueToIntptr(OffsetX.ScalarVariablePtr);
                OffsetY.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetYPtr(mInner);
                OffsetY.SetValueToIntptr(OffsetY.ScalarVariablePtr);
                OffsetZ.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetZPtr(mInner);
                OffsetZ.SetValueToIntptr(OffsetZ.ScalarVariablePtr);
                RotVX.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVXPtr(mInner);
                RotVX.SetValueToIntptr(RotVX.ScalarVariablePtr);
                RotVY.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVYPtr(mInner);
                RotVY.SetValueToIntptr(RotVY.ScalarVariablePtr);
                RotVZ.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVZPtr(mInner);
                RotVZ.SetValueToIntptr(RotVZ.ScalarVariablePtr);
            }
        }
        /// <summary>
        /// 读取粒子的盘旋效果器文件
        /// </summary>
        /// <param name="xndAtt">粒子的盘旋效果器文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (base.Read(xndAtt) == false)
                return false;

            OffsetX.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetXPtr(mInner);
            OffsetX.SetValueToIntptr(OffsetX.ScalarVariablePtr);
            OffsetY.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetYPtr(mInner);
            OffsetY.SetValueToIntptr(OffsetY.ScalarVariablePtr);
            OffsetZ.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetZPtr(mInner);
            OffsetZ.SetValueToIntptr(OffsetZ.ScalarVariablePtr);
            RotVX.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVXPtr(mInner);
            RotVX.SetValueToIntptr(RotVX.ScalarVariablePtr);
            RotVY.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVYPtr(mInner);
            RotVY.SetValueToIntptr(RotVY.ScalarVariablePtr);
            RotVZ.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVZPtr(mInner);
            RotVZ.SetValueToIntptr(RotVZ.ScalarVariablePtr);

            return true;
        }
        /// <summary>
        /// 复制粒子的盘旋效果器
        /// </summary>
        /// <param name="srcData">需要复制的源数据</param>
        /// <returns>复制成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            if (!base.CopyFrom(srcData))
                return false;

            OffsetX.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetXPtr(mInner);
            OffsetX.SetValueToIntptr(OffsetX.ScalarVariablePtr);
            OffsetY.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetYPtr(mInner);
            OffsetY.SetValueToIntptr(OffsetY.ScalarVariablePtr);
            OffsetZ.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetOffsetZPtr(mInner);
            OffsetZ.SetValueToIntptr(OffsetZ.ScalarVariablePtr);
            RotVX.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVXPtr(mInner);
            RotVX.SetValueToIntptr(RotVX.ScalarVariablePtr);
            RotVY.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVYPtr(mInner);
            RotVY.SetValueToIntptr(RotVY.ScalarVariablePtr);
            RotVZ.ScalarVariablePtr = DllImportAPI.ParticleOrbitEffector_GetRotVZPtr(mInner);
            RotVZ.SetValueToIntptr(RotVZ.ScalarVariablePtr);
            return true;
        }
    }
}
