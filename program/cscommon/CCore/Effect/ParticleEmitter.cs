using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.Particle
{
    /// <summary>
    /// 粒子系统的发射器类
    /// </summary>
    public class ParticleEmitter : CSUtility.Support.XndSaveLoadProxy
    {
        /// <summary>
        /// 声明发射器形状改变的委托事件
        /// </summary>
        /// <param name="emitter">改变形状的粒子发射器</param>
        public delegate void Delegate_ChangeShape(ParticleEmitter emitter);
        /// <summary>
        /// 定义粒子发射器改变形状的委托事件
        /// </summary>
        public event Delegate_ChangeShape OnShapeChanged;
        /// <summary>
        /// 发射器的指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 只读属性，粒子发射器的内存地址
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public IntPtr Inner
        {
            get { return mInner; }
        }

        /// <summary>
        /// 发射器运行总时间, 循环播放的发射器运行时间为无限
        /// </summary>
        [Browsable(false)]
        public float TotalTime
        {
            get
            {
                if (IsLooping)
                    return float.PositiveInfinity;
                else
                    return StartDelay + Duration + EmitLife.MaxValue;
            }
        }
        /// <summary>
        /// 发射是否完成，若完成那么就释放该发射器的内存
        /// </summary>
        [Browsable(false)]
        public bool IsFinished
        {
            get
            {
                if (IsLooping)
                    return false;

                if (mInner != IntPtr.Zero)
                    return (DllImportAPI.ParticleEmitter_IsFinished(mInner) != 0) ? true : false;

                return false;
            }
        }
        /// <summary>
        /// 是否发射，可在编辑器中更改
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public bool EnableEmitter
        {
            get
            {
                if(mInner != IntPtr.Zero)
                    return (DllImportAPI.ParticleEmitter_GetEnableEmitter(mInner) != 0) ? true : false;
                return false;
            }
            set
            {
                if (mInner != IntPtr.Zero)
                    DllImportAPI.ParticleEmitter_SetEnableEmitter(mInner, value);
            }
        }
        /// <summary>
        /// 粒子发射器的ID
        /// </summary>
        protected Guid mId = Guid.NewGuid();
        /// <summary>
        /// 粒子发射器的ID
        /// </summary>
        [ReadOnly(true)]
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DoNotCopy]
        [Category("基本"), DisplayName("Id")]
        public Guid Id
        {
            get { return mId; }
            set { mId = value; }
        }
        /// <summary>
        /// 粒子发射器的形状
        /// </summary>
        protected ParticleEmitterShape mShape;
        /// <summary>
        /// 粒子发射器的形状，可在编辑器内更改
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DoNotCopy]
        public ParticleEmitterShape Shape
        {
            get { return mShape; }
            set { mShape = value; }
        }

        List<ParticleEffector> mEffectors = new List<ParticleEffector>();
        //[CSUtility.Support.AutoCopy]
        /// <summary>
        /// 粒子发射器的所有粒子效果器，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DoNotCopy]
        [Browsable(false)]
        public List<ParticleEffector> Effectors
        {
            get { return mEffectors; }
            set { mEffectors = value; }
        }
        //public IParticleEffector[] Effectors
        //{
        //    get
        //    {
        //        List<IParticleEffector> lst = new List<IParticleEffector>();
        //        lst.Add(mEffectorVelocity);
        //        lst.Add(mEffectorForce);
        //        lst.Add(mEffectorColor);
        //        lst.Add(mEffectorScale);
        //        lst.Add(mEffectorRotation);
        //        lst.Add(mEffectorSpawn);
        //        return lst.ToArray();
        //    }
        //}

        List<Guid> mFollowers = new List<Guid>();
        /// <summary>
        /// 粒子跟随的发射器ID列表
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Browsable(false)]
        public List<Guid> Followers
        {
            get { return mFollowers; }
            set
            {
                mFollowers = value;
            }
        }
        /// <summary>
        /// 粒子发射器名字，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("名称")]
        public string Name
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImportAPI.ParticleEmitter_GetName(Inner));
                    }
                }
                return "Emitter";
            }
            set
            {
                unsafe
                {
                    if(Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetName(Inner, value);
                }
            }
        }
        /// <summary>
        /// 是否启用粒子发射器，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Browsable(false), Category("基本"), DisplayName("启用")]
        public bool Enabled
        {
            get
            {
                unsafe
                {
                    if(Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitter_GetEnable(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    if(Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetEnable(Inner, value);
                }
            }
        }

        CCore.Support.ScalarVariable mPositionX = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 粒子发射器的X轴坐标，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("位置"), DisplayName("位置X")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable PositionX
        {
            get { return mPositionX; }
            set { mPositionX = value; }
        }
        CCore.Support.ScalarVariable mPositionY = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 粒子发射器的Y轴坐标，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("位置"), DisplayName("位置Y")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable PositionY
        {
            get { return mPositionY; }
            set { mPositionY = value; }
        }
        CCore.Support.ScalarVariable mPositionZ = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 粒子发射器的Z轴坐标，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("位置"), DisplayName("位置Z")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable PositionZ
        {
            get { return mPositionZ; }
            set { mPositionZ = value; }
        }

        CCore.Support.ScalarVariable mParticleFrame = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 粒子动画帧
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("动画"), DisplayName("粒子动画帧")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable ParticleFrame
        {
            get { return mParticleFrame; }
            set { mParticleFrame = value; }
        }
        /// <summary>
        /// 粒子运动时间，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("位置"), DisplayName("运动时间")]
        public float MovingTime
        {
            get
            {
                unsafe
                {
                    if(mInner != IntPtr.Zero)
                        return DllImportAPI.ParticleEmitter_GetMovingTime(mInner);
                    return 0;
                }
            }
            set
            {
                unsafe
                {
                    if (mInner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetMovingTime(mInner, value);
                }
            }
        }
        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[CSUtility.Editor.Editor_VectorEditor]
        //[Category("基本"), DisplayName("位置")]
        //public SlimDX.Vector3 Position
        //{
        //    get
        //    {
        //        unsafe
        //        {
        //            if (Inner != IntPtr.Zero)
        //            {
        //                SlimDX.Vector3 data = new SlimDX.Vector3();
        //                IDllImportAPI.ParticleEmitter_GetPosition(Inner, &data);
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
        //                IDllImportAPI.ParticleEmitter_SetPosition(Inner, &value);
        //        }
        //    }
        //}
        /// <summary>
        /// 粒子方向，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_VectorEditor]
        [Category("基本"), DisplayName("朝向")]
        public SlimDX.Vector3 Direction
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        SlimDX.Vector3 data = new SlimDX.Vector3();
                        DllImportAPI.ParticleEmitter_GetDirection(Inner, &data);
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
                        DllImportAPI.ParticleEmitter_SetDirection(Inner, &value);
                }
            }
        }
        /// <summary>
        /// 粒子的持续时间，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("发射数量控制"), DisplayName("持续时间")]
        public float Duration
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        return DllImportAPI.ParticleEmitter_GetDuration(Inner);
                    }
                    return 0;
                }
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetDuration(Inner, value);
                }
            }
        }

        CCore.Support.ScalarVariable mEmissionRate = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子的发射频率，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("发射数量控制"), DisplayName("发射频率")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EmissionRate
        {
            get { return mEmissionRate; }
            set
            {
                mEmissionRate = value;
            }
        }
        /// <summary>
        /// 发射类型的枚举
        /// </summary>
        public enum enEmissionType
        {
            EmissionByRate,
            EmissionByCount,
        }
        /// <summary>
        /// 粒子的发射类型，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("发射数量控制"), DisplayName("发射类型")]
        public enEmissionType EmissionType
        {
            get
            {
                if(mInner != IntPtr.Zero)
                    return (enEmissionType)DllImportAPI.ParticleEmitter_GetEmissionType(mInner);
                return enEmissionType.EmissionByRate;
            }
            set
            {
                if(mInner != IntPtr.Zero)
                    DllImportAPI.ParticleEmitter_SetEmissionType(mInner, (int)value);
            }
        }

        CCore.Support.ScalarVariable mEmissionCount = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 发射器一次发射粒子数量，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("发射数量控制"), DisplayName("一次发射数量")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EmissionCount
        {
            get { return mEmissionCount; }
            set { mEmissionCount = value; }
        }
        /// <summary>
        /// 是否循环发射，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Browsable(false), Category("基本"), DisplayName("循环")]
        public bool IsLooping
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitter_GetIsLooping(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetIsLooping(Inner, value);
                }
            }
        }
        /// <summary>
        /// 发射器发射的起始延迟，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("发射数量控制"), DisplayName("起始延迟")]
        public float StartDelay
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return DllImportAPI.ParticleEmitter_GetStartDelay(Inner);
                }
                return 0;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetStartDelay(Inner, value);
                }
            }
        }
        /// <summary>
        /// 发射器的发射延迟，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("发射数量控制"), DisplayName("发射延迟")]
        public float EmitterDelay
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return DllImportAPI.ParticleEmitter_GetEmitterDelay(Inner);
                }
                return 0;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetEmitterDelay(Inner, value);
                }
            }
        }
        CCore.Support.ScalarVariable mGravity = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 发射器发射的粒子所受重力，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("重力")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable Gravity
        {
            get { return mGravity; }
            set { mGravity = value; }
        }
        /// <summary>
        /// 粒子发射器的坐标系，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Browsable(false), Category("基本"), DisplayName("坐标系")]
        public CoordinateSpaceCN Space
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (CoordinateSpaceCN)DllImportAPI.ParticleEmitter_GetCoordinateSpace(Inner);
                }
                return CoordinateSpaceCN.World;
            }
            set
            {
                unsafe
                {
                    if(Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetCoordinateSpace(Inner, (int)value);
                }
            }
        }
        /// <summary>
        /// 粒子发射器发射的粒子是否具有惯性
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("惯性")]
        public bool InheritVelocity
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitter_GetInheritVelocity(Inner) == 0) ? false : true;
                }

                return false;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetInheritVelocity(Inner, value);
                }
            }
        }

        CCore.Support.ScalarVariable mEmitLife = new CCore.Support.ScalarVariable(5);
        /// <summary>
        /// 粒子发射器的粒子生存期，默认为5s，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("粒子"), DisplayName("生存期")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EmitLife
        {
            get { return mEmitLife; }
            set { mEmitLife = value; }
        }

        CCore.Support.ScalarVariable mEmitVelocity = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 粒子发射器发射粒子的初始速率，默认为0，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("粒子"), DisplayName("初始速率")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EmitVelocity
        {
            get { return mEmitVelocity; }
            set { mEmitVelocity = value; }
        }
        /// <summary>
        /// 粒子发射器发射粒子的整体缩放，可在编辑器内修改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("缩放"), DisplayName("整体缩放")]
        public bool EmitScaleAll
        {
            get
            {
                if (mInner != IntPtr.Zero)
                    return DllImportAPI.ParticleEmitter_GetEmitScaleAll(mInner) != 0 ? true : false;
                return false;
            }
            set
            {
                if (mInner != IntPtr.Zero)
                    DllImportAPI.ParticleEmitter_SetEmitScaleAll(mInner, value);
            }
        }
        CCore.Support.ScalarVariable mEmitScaleX = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子发射器发射粒子X轴方向的初始缩放，默认为1，编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("缩放"), DisplayName("初始缩放X")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EmitScaleX
        {
            get { return mEmitScaleX; }
            set { mEmitScaleX = value; }
        }
        CCore.Support.ScalarVariable mEmitScaleY = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子发射器发射粒子Y轴方向的初始缩放，默认为1，编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("缩放"), DisplayName("初始缩放Y")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EmitScaleY
        {
            get { return mEmitScaleY; }
            set { mEmitScaleY = value; }
        }
        CCore.Support.ScalarVariable mEmitScaleZ = new CCore.Support.ScalarVariable(1);
        /// <summary>
        /// 粒子发射器发射粒子Z轴方向的初始缩放，默认为1，编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("缩放"), DisplayName("初始缩放Z")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EmitScaleZ
        {
            get { return mEmitScaleZ; }
            set { mEmitScaleZ = value; }
        }

        CCore.Support.ScalarVariable mEmitRotation = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// 粒子发射器发射粒子的初始旋转，编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("旋转"), DisplayName("初始旋转")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable EmitRotation
        {
            get { return mEmitRotation; }
            set { mEmitRotation = value; }
        }
        /// <summary>
        /// 粒子发射器发射粒子的初始旋转轴的最小值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_VectorEditor]
        [Category("旋转"), DisplayName("初始旋转轴_最小值")]
        public SlimDX.Vector3 EmitRotationAxisMin
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        SlimDX.Vector3 data = new SlimDX.Vector3();
                        DllImportAPI.ParticleEmitter_GetEmitRotationAxisMin(Inner, &data);
                        return data;
                    }
                    return new SlimDX.Vector3();
                }
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetEmitRotationAxisMin(Inner, &value);
                }
            }
        }
        /// <summary>
        /// 粒子发射器发射粒子的初始旋转轴的最大值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_VectorEditor]
        [Category("旋转"), DisplayName("初始旋转轴_最大值")]
        public SlimDX.Vector3 EmitRotationAxisMax
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        SlimDX.Vector3 data = new SlimDX.Vector3();
                        DllImportAPI.ParticleEmitter_GetEmitRotationAxisMax(Inner, &data);
                        return data;
                    }
                    return new SlimDX.Vector3();
                }
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetEmitRotationAxisMax(Inner, &value);
                }
            }
        }
        /// <summary>
        /// 粒子发射器发射的粒子在旋转时是否面对速度方向，编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("旋转"), DisplayName("面向速度方向")]
        public bool FaceWithDirection
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitter_GetFaceToDirection(Inner) != 0)? true : false;
                    return false;
                }
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetFaceToDirection(Inner, value);
                }
            }
        }
        /// <summary>
        /// 粒子发射器发射的粒子的初始颜色
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_VectorEditor]
        [Category("粒子"), DisplayName("初始颜色")]
        public SlimDX.Vector4 EmitColor
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        SlimDX.Vector4 data = new SlimDX.Vector4();
                        DllImportAPI.ParticleEmitter_GetEmitColor(Inner, &data);
                        return data;
                    }
                    return new SlimDX.Vector4();
                }
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitter_SetEmitColor(Inner, &value);
                }
            }
        }
        /// <summary>
        /// 粒子发射器发射的粒子的形体类型
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Browsable(false), Category("基本"), DisplayName("形体类型")]
        public ParticleEmitterShapeCN ShapeType
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        return (ParticleEmitterShapeCN)DllImportAPI.ParticleEmitter_GetShapeType(Inner);
                    }
                    return ParticleEmitterShapeCN.Point;
                }
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero && !mIsLoading)
                    {
                        //IDllImportAPI.ParticleEmitter_SetShapeType(Inner, (int)value);
                        mShape = null;
                        OnChangeShape(value);
                    }
                }
            }
        }
        /// <summary>
        /// 粒子发射器的所属模拟器，默认为null
        /// </summary>
        public CCore.Modifier.ParticleModifier HostModifier = null;

        //protected IParticleEffector_Velocity mEffectorVelocity;
        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[Browsable(false), Category("效果器"), DisplayName("变速")]
        //[System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        //public IParticleEffector_Velocity VelocityEffector
        //{
        //    get { return mEffectorVelocity; }
        //    set { mEffectorVelocity = value; }
        //}
        //protected IParticleEffector_Force mEffectorForce;
        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[Browsable(false), Category("效果器"), DisplayName("力场")]
        //[System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        //public IParticleEffector_Force ForceEffector
        //{
        //    get { return mEffectorForce; }
        //    set { mEffectorForce = value; }
        //}
        //protected IParticleEffector_Color mEffectorColor;
        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[Browsable(false), Category("效果器"), DisplayName("变色")]
        //[System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        //public IParticleEffector_Color ColorEffector
        //{
        //    get { return mEffectorColor; }
        //    set { mEffectorColor = value; }
        //}
        //protected IParticleEffector_Scale mEffectorScale;
        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[Browsable(false), Category("效果器"), DisplayName("放缩")]
        //[System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        //public IParticleEffector_Scale ScaleEffector
        //{
        //    get { return mEffectorScale; }
        //    set { mEffectorScale = value; }
        //}
        //protected IParticleEffector_Rotation mEffectorRotation;
        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[Browsable(false), Category("效果器"), DisplayName("旋转")]
        //[System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        //public IParticleEffector_Rotation RotationEffector
        //{
        //    get { return mEffectorRotation; }
        //    set { mEffectorRotation = value; }
        //}
        //protected IParticleEffector_Spawn mEffectorSpawn;
        //[CSUtility.Support.AutoCopy]
        //[CSUtility.Support.AutoSaveLoad]
        //[Browsable(false), Category("效果器"), DisplayName("繁衍")]
        //[System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
        //public IParticleEffector_Spawn SpawnEffector
        //{
        //    get { return mEffectorSpawn; }
        //    set { mEffectorSpawn = value; }
        //}
        /// <summary>
        /// 粒子发射器的构造函数，用于创建粒子发射器
        /// </summary>
        public ParticleEmitter()
        {
            unsafe
            {
                mInner = DllImportAPI.ParticleEmitter_New();

                //mEffectorVelocity = new IParticleEffector_Velocity(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Velocity)));
                //mEffectorForce = new IParticleEffector_Force(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Force)));
                //mEffectorColor = new IParticleEffector_Color(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Color)));
                //mEffectorScale = new IParticleEffector_Scale(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Scale)));
                //mEffectorRotation = new IParticleEffector_Rotation(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Rotation)));
                //mEffectorSpawn = new IParticleEffector_Spawn(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Spawn)));
                //mEffectors.Add(mEffectorVelocity);
                //mEffectors.Add(mEffectorForce);
                //mEffectors.Add(mEffectorColor);
                //mEffectors.Add(mEffectorScale);
                //mEffectors.Add(mEffectorRotation);
                //mEffectors.Add(mEffectorSpawn);

                EmissionRate.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmissionRatePtr(mInner);
                EmitLife.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitLifePtr(mInner);
                EmitVelocity.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitVelocityPtr(mInner);
                EmitScaleX.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleXPtr(mInner);
                EmitScaleY.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleYPtr(mInner);
                EmitScaleZ.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleZPtr(mInner);
                Gravity.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetGravityPtr(mInner);
                EmitRotation.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitRotationAnglePtr(mInner);
                EmissionCount.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmissionCountPtr(mInner);
                PositionX.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionXPtr(mInner);
                PositionY.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionYPtr(mInner);
                PositionZ.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionZPtr(mInner);

                ParticleFrame.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetParticleFrame(mInner);
                ParticleFrame.VariableType = CCore.Support.enVariableType.ConstantRange;

                Name = "Emitter";
                OnChangeShape(ParticleEmitterShapeCN.Cone);
            }
        }
        //public IParticleEmitter(IntPtr inner)
        //{
        //    unsafe
        //    {
        //        if (inner != IntPtr.Zero)
        //        {
        //            IDllImportAPI.ParticleEmitter_AddRef(inner);
        //            mInner = inner;

        //            //mEffectorVelocity = new IParticleEffector_Velocity(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Velocity)));
        //            //mEffectorForce = new IParticleEffector_Force(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Force)));
        //            //mEffectorColor = new IParticleEffector_Color(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Color)));
        //            //mEffectorScale = new IParticleEffector_Scale(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Scale)));
        //            //mEffectorRotation = new IParticleEffector_Rotation(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Rotation)));
        //            //mEffectorSpawn = new IParticleEffector_Spawn(IDllImportAPI.ParticleEmitter_GetEffector(Inner, (int)(EffectorSlot.Spawn)));
        //            //mEffectors.Add(mEffectorVelocity);
        //            //mEffectors.Add(mEffectorForce);
        //            //mEffectors.Add(mEffectorColor);
        //            //mEffectors.Add(mEffectorScale);
        //            //mEffectors.Add(mEffectorRotation);
        //            //mEffectors.Add(mEffectorSpawn);

        //            EmissionRate.ScalarVariablePtr = IDllImportAPI.ParticleEmitter_GetEmissionRatePtr(mInner);
        //            EmitLife.ScalarVariablePtr = IDllImportAPI.ParticleEmitter_GetEmitLifePtr(mInner);
        //            EmitVelocity.ScalarVariablePtr = IDllImportAPI.ParticleEmitter_GetEmitVelocityPtr(mInner);
        //            EmitScale.ScalarVariablePtr = IDllImportAPI.ParticleEmitter_GetEmitScalePtr(mInner);
        //            Gravity.ScalarVariablePtr = IDllImportAPI.ParticleEmitter_GetGravityPtr(mInner);

        //            OnChangeShape();
        //        }
        //    }
        //}
        /// <summary>
        /// 析构函数，释放发射器实例的内存
        /// </summary>
        ~ParticleEmitter()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放该发射器实例的内存
        /// </summary>
        public void Cleanup()
        {
            unsafe
            {
                if (mInner != IntPtr.Zero)
                {
                    DllImportAPI.ParticleEmitter_Release(mInner);
                    mInner = IntPtr.Zero;
                }
            }

        }
        /// <summary>
        /// 当发射器形状改变时调用
        /// </summary>
        /// <param name="shapeType">发射器的形状</param>
        public void OnChangeShape(ParticleEmitterShapeCN shapeType)
        {
            unsafe
            {
                //var shapePtr = IDllImportAPI.ParticleEmitter_GetShape(Inner);
                //var shapeType = (ParticleEmitterShapeCN)IDllImportAPI.ParticleEmitterShape_GetType(shapePtr);
                if (mShape != null && ((ParticleEmitterShapeCN)DllImportAPI.ParticleEmitterShape_GetType(mShape.Inner) == shapeType))
                    return;

                switch (shapeType)
                {
                    case ParticleEmitterShapeCN.Box:
                        mShape = new ParticleEmitterShapeBox();
                        break;
                    case ParticleEmitterShapeCN.Cone:
                        mShape = new ParticleEmitterShapeCone();
                        break;
                    case ParticleEmitterShapeCN.Sphere:
                        mShape = new ParticleEmitterShapeSphere();
                        break;
                    case ParticleEmitterShapeCN.Mesh:
                        mShape = new ParticleEmitterShapeMesh();
                        break;
                    case ParticleEmitterShapeCN.Point:
                    default:
                        mShape = new ParticleEmitterShapePoint();
                        break;
                }

                DllImportAPI.ParticleEmitter_SetShapePtr(Inner, mShape.Inner);

                if (OnShapeChanged != null)
                    OnShapeChanged(this);
            }
        }

        //public void Save(CSUtility.Support.IXndNode node)
        //{
        //    // 存基本属性
        //    // 存效果器
        //    // 存Shape
        //}
        //public void Load(CSUtility.Support.IXndNode node)
        //{

        //}
        //public void CopyFrom(IParticleEmitter srcEmitter)
        //{

        //}

        //public void Load(CSUtility.Support.IXndNode xndNode)
        //{

        //}
        //public void Save(CSUtility.Support.IXndNode xndNode)
        //{

        //}
        /// <summary>
        /// 克隆粒子发射器
        /// </summary>
        /// <param name="src">需要克隆的粒子发射器</param>
        public virtual void Clone(ParticleEmitter src)
        {
            Id = src.Id;
            mFollowers.Clear();
            mFollowers.AddRange(src.mFollowers);
            Name = src.Name;
            Enabled = src.Enabled;
            PositionX.Clone(src.PositionX);
            PositionY.Clone(src.PositionY);
            PositionZ.Clone(src.PositionZ);
            ParticleFrame.Clone(src.ParticleFrame);
            MovingTime = src.MovingTime;
            Direction = src.Direction;
            Duration = src.Duration;
            EmissionRate.Clone(src.EmissionRate);
            EmissionType = src.EmissionType;
            EmissionCount.Clone(src.EmissionCount);
            IsLooping = src.IsLooping;
            StartDelay = src.StartDelay;
            EmitterDelay = src.EmitterDelay;
            Gravity.Clone(src.Gravity);
            Space = src.Space;
            InheritVelocity = src.InheritVelocity;
            EmitLife.Clone(src.EmitLife);
            EmitVelocity.Clone(src.EmitVelocity);
            EmitScaleAll = src.EmitScaleAll;
            EmitScaleX.Clone(src.EmitScaleX);
            EmitScaleY.Clone(src.EmitScaleY);
            EmitScaleZ.Clone(src.EmitScaleZ);
            EmitRotation.Clone(src.EmitRotation);
            EmitRotationAxisMin = src.EmitRotationAxisMin;
            FaceWithDirection = src.FaceWithDirection;
            EmitColor = src.EmitColor;
            ShapeType = src.ShapeType;

            if (Shape == null)
                Shape = new ParticleEmitterShape();
            Shape.Clone(src.Shape);

            foreach (var effector in src.Effectors)
            {
                var newEffector = AddEffector(effector.Slot);
                if (newEffector != null)
                    newEffector.Clone(effector);
            }


            EmissionRate.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmissionRatePtr(mInner);
            EmissionRate.SetValueToIntptr(EmissionRate.ScalarVariablePtr);
            EmitLife.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitLifePtr(mInner);
            EmitLife.SetValueToIntptr(EmitLife.ScalarVariablePtr);
            EmitVelocity.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitVelocityPtr(mInner);
            EmitVelocity.SetValueToIntptr(EmitVelocity.ScalarVariablePtr);
            EmitScaleX.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleXPtr(mInner);
            EmitScaleX.SetValueToIntptr(EmitScaleX.ScalarVariablePtr);
            EmitScaleY.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleYPtr(mInner);
            EmitScaleY.SetValueToIntptr(EmitScaleY.ScalarVariablePtr);
            EmitScaleZ.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleZPtr(mInner);
            EmitScaleZ.SetValueToIntptr(EmitScaleZ.ScalarVariablePtr);
            Gravity.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetGravityPtr(mInner);
            Gravity.SetValueToIntptr(Gravity.ScalarVariablePtr);
            EmitRotation.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitRotationAnglePtr(mInner);
            EmitRotation.SetValueToIntptr(EmitRotation.ScalarVariablePtr);
            EmissionCount.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmissionCountPtr(mInner);
            EmissionCount.SetValueToIntptr(EmissionCount.ScalarVariablePtr);
            PositionX.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionXPtr(mInner);
            PositionX.SetValueToIntptr(PositionX.ScalarVariablePtr);
            PositionY.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionYPtr(mInner);
            PositionY.SetValueToIntptr(PositionY.ScalarVariablePtr);
            PositionZ.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionZPtr(mInner);
            PositionZ.SetValueToIntptr(PositionZ.ScalarVariablePtr);
            ParticleFrame.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetParticleFrame(mInner);
            ParticleFrame.SetValueToIntptr(ParticleFrame.ScalarVariablePtr);
        }


        bool mIsLoading = false;
        /// <summary>
        /// 读取粒子发射器的文件
        /// </summary>
        /// <param name="xndAtt">粒子发射器的文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            mIsLoading = true;

            if (base.Read(xndAtt) == false)
                return false;

            if (mShape == null || mShape.Inner == IntPtr.Zero)
                mShape = new ParticleEmitterShapePoint();

            //OnChangeShape(ShapeType);
            DllImportAPI.ParticleEmitter_SetShapePtr(Inner, mShape.Inner);

            EmissionRate.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmissionRatePtr(mInner);
            EmissionRate.SetValueToIntptr(EmissionRate.ScalarVariablePtr);
            EmitLife.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitLifePtr(mInner);
            EmitLife.SetValueToIntptr(EmitLife.ScalarVariablePtr);
            EmitVelocity.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitVelocityPtr(mInner);
            EmitVelocity.SetValueToIntptr(EmitVelocity.ScalarVariablePtr);
            EmitScaleX.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleXPtr(mInner);
            EmitScaleX.SetValueToIntptr(EmitScaleX.ScalarVariablePtr);
            EmitScaleY.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleYPtr(mInner);
            EmitScaleY.SetValueToIntptr(EmitScaleY.ScalarVariablePtr);
            EmitScaleZ.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleZPtr(mInner);
            EmitScaleZ.SetValueToIntptr(EmitScaleZ.ScalarVariablePtr);
            Gravity.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetGravityPtr(mInner);
            Gravity.SetValueToIntptr(Gravity.ScalarVariablePtr);
            EmitRotation.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitRotationAnglePtr(mInner);
            EmitRotation.SetValueToIntptr(EmitRotation.ScalarVariablePtr);
            EmissionCount.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmissionCountPtr(mInner);
            EmissionCount.SetValueToIntptr(EmissionCount.ScalarVariablePtr);
            PositionX.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionXPtr(mInner);
            PositionX.SetValueToIntptr(PositionX.ScalarVariablePtr);
            PositionY.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionYPtr(mInner);
            PositionY.SetValueToIntptr(PositionY.ScalarVariablePtr);
            PositionZ.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionZPtr(mInner);
            PositionZ.SetValueToIntptr(PositionZ.ScalarVariablePtr);
            ParticleFrame.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetParticleFrame(mInner);
            ParticleFrame.SetValueToIntptr(ParticleFrame.ScalarVariablePtr);

            //if (mEffectorVelocity.Enable)
            //    mEffectors.Add(mEffectorVelocity);
            //if (mEffectorForce.Enable)
            //    mEffectors.Add(mEffectorForce);
            //if (mEffectorColor.Enable)
            //    mEffectors.Add(mEffectorColor);
            //if (mEffectorScale.Enable)
            //    mEffectors.Add(mEffectorScale);
            //if (mEffectorRotation.Enable)
            //    mEffectors.Add(mEffectorRotation);
            //if (mEffectorSpawn.Enable)
            //    mEffectors.Add(mEffectorSpawn);

            unsafe
            {
                foreach (var effector in mEffectors)
                {
                    DllImportAPI.ParticleEmitter_AddEffector(Inner, effector.Inner);
                }
            }

            mIsLoading = false;

            return true;
        }
        /// <summary>
        /// 复制粒子发射器
        /// </summary>
        /// <param name="srcData">复制的粒子发射器源数据</param>
        /// <returns>复制成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            if (!base.CopyFrom(srcData))
                return false;

            EmissionRate.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmissionRatePtr(mInner);
            EmissionRate.SetValueToIntptr(EmissionRate.ScalarVariablePtr);
            EmitLife.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitLifePtr(mInner);
            EmitLife.SetValueToIntptr(EmitLife.ScalarVariablePtr);
            EmitVelocity.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitVelocityPtr(mInner);
            EmitVelocity.SetValueToIntptr(EmitVelocity.ScalarVariablePtr);
            EmitScaleX.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleXPtr(mInner);
            EmitScaleX.SetValueToIntptr(EmitScaleX.ScalarVariablePtr);
            EmitScaleY.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleYPtr(mInner);
            EmitScaleY.SetValueToIntptr(EmitScaleY.ScalarVariablePtr);
            EmitScaleZ.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitScaleZPtr(mInner);
            EmitScaleZ.SetValueToIntptr(EmitScaleZ.ScalarVariablePtr);
            Gravity.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetGravityPtr(mInner);
            Gravity.SetValueToIntptr(Gravity.ScalarVariablePtr);
            EmitRotation.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmitRotationAnglePtr(mInner);
            EmitRotation.SetValueToIntptr(EmitRotation.ScalarVariablePtr);
            EmissionCount.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetEmissionCountPtr(mInner);
            EmissionCount.SetValueToIntptr(EmissionCount.ScalarVariablePtr);
            PositionX.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionXPtr(mInner);
            PositionX.SetValueToIntptr(PositionX.ScalarVariablePtr);
            PositionY.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionYPtr(mInner);
            PositionY.SetValueToIntptr(PositionY.ScalarVariablePtr);
            PositionZ.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetPositionZPtr(mInner);
            PositionZ.SetValueToIntptr(PositionZ.ScalarVariablePtr);
            ParticleFrame.ScalarVariablePtr = DllImportAPI.ParticleEmitter_GetParticleFrame(mInner);
            ParticleFrame.SetValueToIntptr(ParticleFrame.ScalarVariablePtr);

            var srcEmitter = srcData as ParticleEmitter;

            //this.Id = srcEmitter.Id;

            if (Shape == null)
                Shape = new ParticleEmitterShapePoint();
            Shape.CopyFrom(srcEmitter.Shape);

            foreach (var effector in srcEmitter.Effectors)
            {
                var newEffector = AddEffector(effector.Slot);
                if (newEffector != null)
                    newEffector.CopyFrom(effector);
            }
            //unsafe
            //{
            //    foreach (var effector in mEffectors)
            //    {
            //        IDllImportAPI.ParticleEmitter_AddEffector(Inner, effector.Inner);
            //    }
            //}
            return true;
        }
        /// <summary>
        /// 重置粒子发射器
        /// </summary>
        public void Reset()
        {
            unsafe{
                if (Inner != IntPtr.Zero)
                    DllImportAPI.ParticleEmitter_Reset(Inner);
            }
        }
        /// <summary>
        /// 给当前的粒子发射器实例添加效果器
        /// </summary>
        /// <param name="slot">要添加的效果器</param>
        /// <returns>返回添加的粒子效果器</returns>
        public ParticleEffector AddEffector(EffectorSlot slot)
        {
            // 判断是否添加重复
            //foreach (var effector in mEffectors)
            //{
            //    if (effector.Slot == slot)
            //        return null;
            //}

            ParticleEffector addEffector = null;
            switch (slot)
            {
                case EffectorSlot.Velocity:
                    {
                        addEffector = new ParticleEffector_Velocity();
                    }
                    break;

                case EffectorSlot.Force:
                    {
                        addEffector = new ParticleEffector_Force();
                    }
                    break;

                case EffectorSlot.Color:
                    {
                        addEffector = new ParticleEffector_Color();
                    }
                    break;

                case EffectorSlot.Scale:
                    {
                        addEffector = new ParticleEffector_Scale();
                    }
                    break;

                case EffectorSlot.Rotation:
                    {
                        addEffector = new ParticleEffector_Rotation();
                    }
                    break;

                case EffectorSlot.Spawn:
                    {
                        addEffector = new ParticleEffector_Spawn();
                    }
                    break;

                case EffectorSlot.Orbit:
                    {
                        addEffector = new ParticleEffector_Orbit();
                    }
                    break;
            }

            if (addEffector != null)
            {
                mEffectors.Add(addEffector);
                DllImportAPI.ParticleEmitter_AddEffector(Inner, addEffector.Inner);
            }


            return addEffector;
        }
        /// <summary>
        /// 删除粒子的效果器
        /// </summary>
        /// <param name="effect">将要删除的粒子效果器</param>
        public void RemoveEffector(CCore.Particle.ParticleEffector effect)
        {
            mEffectors.Remove(effect);
            DllImportAPI.ParticleEmitter_RemoveEffector(Inner, effect.Inner);
        }

        #region 跟随处理
        /// <summary>
        /// 删除跟随的粒子发射器，编辑器内可操作
        /// </summary>
        /// <param name="idx">跟随的粒子发射器组的索引</param>
        public void RemoveFollowerEmitter(int idx)
        {
            if (idx >= mFollowers.Count || idx < 0)
                return;

            mFollowers.RemoveAt(idx);
            unsafe
            {
                if (mInner != null)
                    DllImportAPI.ParticleEmitter_RemoveFollowerEmitter(mInner, idx);
            }
        }
        /// <summary>
        /// 设置跟随的粒子发射器
        /// </summary>
        /// <param name="idx">跟随的粒子发射器索引</param>
        /// <param name="emit">跟随的粒子发射器实例</param>
        public void SetFollowerEmitter(int idx, ParticleEmitter emit)
        {
            if (idx < 0 || idx >= mFollowers.Count)
                return;

            mFollowers[idx] = emit.Id;

            unsafe
            {
                if (mInner != null)
                    DllImportAPI.ParticleEmitter_SetFollowerEmitter(mInner, idx, emit.Inner, emit.HostModifier.Modifier);
            }
        }
        /// <summary>
        /// 更新跟随的粒子发射器
        /// </summary>
        public void UpdateFollowerEmitters()
        {
            int idx = 0;
            foreach (var id in mFollowers)
            {
                var emt = this.HostModifier.HostEffector.FindEmitter(id);
                if (emt == null)
                    continue;

                unsafe
                {
                    if (mInner != null)
                        DllImportAPI.ParticleEmitter_SetFollowerEmitter(mInner, idx, emt.Inner, emt.HostModifier.Modifier);
                }

                idx++;
            }
        }
        /// <summary>
        /// 更新发射器的繁衍效果器
        /// </summary>
        public void UpdateSpawnEffector()
        {
            foreach (var effect in mEffectors)
            {
                if (effect is CCore.Particle.ParticleEffector_Spawn)
                {
                    var spawnEffect = effect as CCore.Particle.ParticleEffector_Spawn;
                    spawnEffect.UpdateAfterLoaded(this);
                }
            }
        }

#endregion

    }
}
