using System;
using System.ComponentModel;

namespace CCore.Particle
{
    /// <summary>
    /// 粒子发射器的形状
    /// </summary>
    [TypeConverter(typeof(ParticleObjConverter))]
    public class ParticleEmitterShape : CSUtility.Support.XndSaveLoadProxy
    {
        /// <summary>
        /// 发射器形状的实例指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 只读属性，发射器形状的实例地址
        /// </summary>
        [Browsable(false)] 
        [CSUtility.Support.DoNotCopy]
        public IntPtr Inner
        {
            get { return mInner; }
        } 
        /// <summary>
        /// 是否在壳表面发射，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("壳表面发射")]
        public bool IsEmitFromShell
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        return (DllImportAPI.ParticleEmitterShape_GetIsEmitFromShell(Inner) == 0) ? false : true;
                    }
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetIsEmitFromShell(Inner, value);
                }
            }
        }
        /// <summary>
        /// 发射粒子形状的随机方向是否完全随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机方向"), DisplayName("方向完全随机")]
        public bool IsRandomDirection
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetIsRandomDirection(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetIsRandomDirection(Inner, value);
                }
            }
        } 
        /// <summary>
        /// 发射粒子形状的随机方向是否为X轴正向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机方向"), DisplayName("X轴正向随机")]
        public bool RandomDirAvailableX
        {
            get
            {
                unsafe
                {
                    if(Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomDirAvailableX(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomDirAvailableX(Inner, value);
                }
            }
        }  
        /// <summary>
        /// 发射粒子形状的随机方向是否为Y轴正向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机方向"), DisplayName("Y轴正向随机")]
        public bool RandomDirAvailableY
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomDirAvailableY(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomDirAvailableY(Inner, value);
                }
            }
        } 
        /// <summary>
        /// 发射粒子形状的随机方向是否为Z轴正向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机方向"), DisplayName("Z轴正向随机")]
        public bool RandomDirAvailableZ
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomDirAvailableZ(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomDirAvailableZ(Inner, value);
                }
            }
        } 
        /// <summary>
        /// 发射粒子形状的随机方向是否为X轴反向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机方向"), DisplayName("X轴反向随机")]
        public bool RandomDirAvailableInvX
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomDirAvailableInvX(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomDirAvailableInvX(Inner, value);
                }
            }
        }
        /// <summary>
        /// 发射粒子形状的随机方向是否为Y轴反向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机方向"), DisplayName("Y轴反向随机")]
        public bool RandomDirAvailableInvY
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomDirAvailableInvY(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomDirAvailableInvY(Inner, value);
                }
            }
        }
        /// <summary>
        /// 发射粒子形状的随机方向是否为Z轴反向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机方向"), DisplayName("Z轴反向随机")]
        public bool RandomDirAvailableInvZ
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomDirAvailableInvZ(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomDirAvailableInvZ(Inner, value);
                }
            }
        }
        /// <summary>
        /// 发射粒子形状的随机位置是否为X轴正向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机位置"), DisplayName("X轴正向随机")]
        public bool RandomPosAvailableX
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomPosAvailableX(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomPosAvailableX(Inner, value);
                }
            }
        }
        /// <summary>
        /// 发射粒子形状的随机位置是否为Y轴正向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机位置"), DisplayName("Y轴正向随机")]
        public bool RandomPosAvailableY
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomPosAvailableY(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomPosAvailableY(Inner, value);
                }
            }
        }
        /// <summary>
        /// 发射粒子形状的随机位置是否为Z轴正向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机位置"), DisplayName("Z轴正向随机")]
        public bool RandomPosAvailableZ
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomPosAvailableZ(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomPosAvailableZ(Inner, value);
                }
            }
        }
        /// <summary>
        /// 发射粒子形状的随机位置是否为X轴反向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机位置"), DisplayName("X轴反向随机")]
        public bool RandomPosAvailableInvX
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomPosAvailableInvX(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomPosAvailableInvX(Inner, value);
                }
            }
        }  
        /// <summary>
        /// 发射粒子形状的随机位置是否为Y轴反向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机位置"), DisplayName("Y轴反向随机")]
        public bool RandomPosAvailableInvY
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomPosAvailableInvY(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomPosAvailableInvY(Inner, value);
                }
            }
        } 
        /// <summary>
        /// 发射粒子形状的随机位置是否为Z轴反向随机，在编辑器内可更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("随机位置"), DisplayName("Z轴反向随机")]
        public bool RandomPosAvailableInvZ
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShape_GetRandomPosAvailableInvZ(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    DllImportAPI.ParticleEmitterShape_SetRandomPosAvailableInvZ(Inner, value);
                }
            }
        }
        /// <summary>
        /// AABB包围盒的最大顶点
        /// </summary>
        protected SlimDX.Vector3 mAABBMax = SlimDX.Vector3.UnitXYZ;
        /// <summary>
        /// 只读属性，AABB包围盒的最大顶点
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public SlimDX.Vector3 AABBMax
        {
            get { return mAABBMax; }
            //set { mAABBMax = value; }
        }
        /// <summary>
        /// AABB包围盒的最小顶点
        /// </summary>
        protected SlimDX.Vector3 mAABBMin = -SlimDX.Vector3.UnitXYZ;
        /// <summary>
        /// 只读属性，AABB包围盒的最小顶点
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public SlimDX.Vector3 AABBMin
        {
            get { return mAABBMin; }
            //set { mAABBMin = value; }
        }
        /// <summary>
        /// 粒子发射器形状的构造函数
        /// </summary>
        public ParticleEmitterShape()
        {
            //if(mInner == IntPtr.Zero)
                //mInner = IDllImportAPI.ParticleEmitterShape_NewEmitterShape(IntPtr.Zero, (int)(ParticleEmitterShapeCN.Point));
        }
        /// <summary>
        /// 析构函数，将对象的指针置空
        /// </summary>
        ~ParticleEmitterShape()
        {
            if (mInner != IntPtr.Zero)
            {
                DllImportAPI.ParticleEmitterShape_ReleaseEmitterShape(mInner);
                mInner = IntPtr.Zero;
            }
        }
        //public IParticleEmitterShape(IntPtr inner)
        //{
        //    mInner = inner;
        //}
        /// <summary>
        /// 克隆粒子发射器的形状
        /// </summary>
        /// <param name="src">粒子发射器的形状的源对象</param>
        public virtual void Clone(ParticleEmitterShape src)
        {
            IsEmitFromShell = src.IsEmitFromShell;
            IsRandomDirection = src.IsRandomDirection;
            RandomDirAvailableX = src.RandomDirAvailableX;
            RandomDirAvailableY = src.RandomDirAvailableY;
            RandomDirAvailableZ = src.RandomDirAvailableZ;
            RandomDirAvailableInvX = src.RandomDirAvailableInvX;
            RandomDirAvailableInvY = src.RandomDirAvailableInvY;
            RandomDirAvailableInvZ = src.RandomDirAvailableInvZ;
            RandomPosAvailableX = src.RandomPosAvailableX;
            RandomPosAvailableY = src.RandomPosAvailableY;
            RandomPosAvailableZ = src.RandomPosAvailableZ;
            RandomPosAvailableInvX = src.RandomPosAvailableInvX;
            RandomPosAvailableInvY = src.RandomPosAvailableInvY;
            RandomPosAvailableInvZ = src.RandomPosAvailableInvZ;
        }
    }
    /// <summary>
    /// 点状的粒子发射器
    /// </summary>
    public class ParticleEmitterShapePoint : ParticleEmitterShape
    {
        /// <summary>
        /// 创建粒子发射器的形状为点状，编辑器内可更改
        /// </summary>
        public ParticleEmitterShapePoint()
        {
            mInner = DllImportAPI.ParticleEmitterShape_NewEmitterShape(IntPtr.Zero, (int)(ParticleEmitterShapeCN.Point));
        }
    }
    /// <summary>
    /// 盒子形状的粒子发射器
    /// </summary>
    public class ParticleEmitterShapeBox : ParticleEmitterShape
    {
        /// <summary>
        /// 盒子形状的发射器X轴大小
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("SizeX")]
        public float SizeX
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return DllImportAPI.ParticleEmitterShapeBox_GetSizeX(Inner);
                }
                return 0;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        if(value < 0)
                            value = 0;
                        DllImportAPI.ParticleEmitterShapeBox_SetSizeX(Inner, value);

                        mAABBMax.X = SizeX * 0.5f;
                        mAABBMin.X = -SizeX * 0.5f;
                    }
                }
            }
        }
        /// <summary>
        /// 盒子形状的发射器Y轴大小
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("SizeY")]
        public float SizeY
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return DllImportAPI.ParticleEmitterShapeBox_GetSizeY(Inner);
                }
                return 0;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        if (value < 0)
                            value = 0;
                        DllImportAPI.ParticleEmitterShapeBox_SetSizeY(Inner, value);

                        mAABBMax.Y = SizeY * 0.5f;
                        mAABBMin.Y = -SizeY * 0.5f;
                    }
                }
            }
        }
        /// <summary>
        /// 盒子形状的发射器Z轴大小
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("SizeZ")]
        public float SizeZ
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return DllImportAPI.ParticleEmitterShapeBox_GetSizeZ(Inner);
                }
                return 0;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                    {
                        if (value < 0)
                            value = 0;
                        DllImportAPI.ParticleEmitterShapeBox_SetSizeZ(Inner, value);

                        mAABBMax.Z = SizeZ * 0.5f;
                        mAABBMin.Z = -SizeZ * 0.5f;
                    }
                }
            }
        }
        /// <summary>
        /// 克隆box形状的粒子发射器
        /// </summary>
        /// <param name="src">粒子发射器的形状的源对象</param>
        public override void Clone(ParticleEmitterShape src)
        {
            base.Clone(src);

            var srcT = src as ParticleEmitterShapeBox;
            if(srcT !=null)
            {
                SizeX = srcT.SizeX;
                SizeY = srcT.SizeY;
                SizeZ = srcT.SizeZ;
            }
        }
        /// <summary>
        /// box形状的发射器的构造函数
        /// </summary>
        public ParticleEmitterShapeBox()
        {
            mInner = DllImportAPI.ParticleEmitterShape_NewEmitterShape(IntPtr.Zero, (int)(ParticleEmitterShapeCN.Box));
        }
        //public IParticleEmitterShapeBox(IntPtr inner)
        //    : base(inner)
        //{
        //}
        /// <summary>
        /// 析构函数，释放实例内存
        /// </summary>
        ~ParticleEmitterShapeBox()
        {
            if (mInner != IntPtr.Zero)
            {
                DllImportAPI.ParticleEmitterShape_ReleaseEmitterShape(mInner);
                mInner = IntPtr.Zero;
            }            
        }
    }
    /// <summary>
    /// 胶囊形状的粒子发射器
    /// </summary>
    public class ParticleEmitterShapeCone : ParticleEmitterShape
    {
        /// <summary>
        /// cone形状的发射器发射粒子的方向枚举值
        /// </summary>
        public enum enDirectionType
        {
            ConeDirUp,
            ConeDirDown,
            EmitterDir,
            NormalOutDir,
            NormalInDir,
            OutDir,
            InDir,
        };
        /// <summary>
        /// cone形状的发射器发射粒子的方向，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("方向")]
        public enDirectionType ParticleDirectionType
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (enDirectionType)(DllImportAPI.ParticleEmitterShapeCone_GetDirType(Inner));

                    return enDirectionType.ConeDirUp;
                }
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitterShapeCone_SetDirType(Inner, (int)value);
                }
            }
        }
        /// <summary>
        /// cone形状的发射器发射粒子的角度，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("角度")]
        public float Angle
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return DllImportAPI.ParticleEmitterShapeCone_GetAngle(Inner);
                }
                return 0;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitterShapeCone_SetAngle(Inner, value);
                }
            }
        }
        /// <summary>
        /// cone形状的发射器发射粒子的半径，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("半径")]
        public float Radius
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return DllImportAPI.ParticleEmitterShapeCone_GetRadius(Inner);
                }
                return 0;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitterShapeCone_SetRadius(Inner, value);

                    mAABBMax.X = Radius;
                    mAABBMax.Z = Radius;
                    mAABBMin.X = -Radius;
                    mAABBMin.Z = -Radius;
                }
            }
        }
        /// <summary>
        /// cone形状的发射器发射粒子的长度，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("长度")]
        public float Length
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return DllImportAPI.ParticleEmitterShapeCone_GetLength(Inner);
                }
                return 0;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitterShapeCone_SetLength(Inner, value);

                    mAABBMax.Y = Length;
                    mAABBMin.Y = 0;
                }
            }
        }
        /// <summary>
        /// cone形状的发射器是否仅根截面发射粒子，可在编辑器内更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("仅根截面发射")]
        public bool IsEmitFromBase
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShapeCone_GetIsEmitFromBase(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitterShapeCone_SetIsEmitFromBase(Inner, value);
                }
            }
        }
        /// <summary>
        /// 克隆cone形状的发射器
        /// </summary>
        /// <param name="src">要克隆的cone形状的发射器</param>
        public override void Clone(ParticleEmitterShape src)
        {
            base.Clone(src);

            var srcT = src as ParticleEmitterShapeCone;
            if (srcT != null)
            {
                ParticleDirectionType = srcT.ParticleDirectionType;
                Angle = srcT.Angle;
                Radius = srcT.Radius;
                Length = srcT.Length;
                IsEmitFromBase = srcT.IsEmitFromBase;
            }
        }
        /// <summary>
        /// cone形状的发射器的构造函数，用于创建cone形状的发射器
        /// </summary>
        public ParticleEmitterShapeCone()
        {
            mInner = DllImportAPI.ParticleEmitterShape_NewEmitterShape(IntPtr.Zero, (int)(ParticleEmitterShapeCN.Cone));
        }
        //public IParticleEmitterShapeCone(IntPtr inner)
        //    : base(inner)
        //{
        //}
        /// <summary>
        /// 析构函数，释放对象内存
        /// </summary>
        ~ParticleEmitterShapeCone()
        {
            if (mInner != IntPtr.Zero)
            {
                DllImportAPI.ParticleEmitterShape_ReleaseEmitterShape(mInner);
                mInner = IntPtr.Zero;
            }            
        }
    }
    /// <summary>
    /// 球体形状的粒子发射器
    /// </summary>
    public class ParticleEmitterShapeSphere : ParticleEmitterShape
    {
        /// <summary>
        /// 球体形状的发射器的球体半径
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("半径")]
        public float Radius
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return DllImportAPI.ParticleEmitterShapeSphere_GetRadius(Inner);
                }
                return 0;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitterShapeSphere_SetRadius(Inner, value);

                    mAABBMax.X = Radius;
                    mAABBMax.Y = Radius;
                    mAABBMax.Z = Radius;
                    mAABBMin.X = -Radius;
                    mAABBMin.Y = -Radius;
                    mAABBMin.Z = -Radius;
                }
            }
        }
        /// <summary>
        /// 球体形状的发射器的半球体
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Browsable(false), Category("基本"), DisplayName("半球体")]
        public bool IsHemiSphere
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShapeSphere_GetIsHemiSphere(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitterShapeSphere_SetIsHemiSphere(Inner, value);
                }
            }
        }
        /// <summary>
        /// 球体形状的发射器径向向外发射
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("径向向外发射")]
        public bool IsRadialOutDirection
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShapeSphere_GetIsRadialOutDirection(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    IsRadialInDirection = false;
                }

                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitterShapeSphere_SetIsRadialOutDirection(Inner, value);
                }
            }
        }
        /// <summary>
        /// 球体形状的发射器径向向外发射
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("径向向内发射")]
        public bool IsRadialInDirection
        {
            get
            {
                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        return (DllImportAPI.ParticleEmitterShapeSphere_GetIsRadialInDirection(Inner) == 0) ? false : true;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    IsRadialOutDirection = false;
                }

                unsafe
                {
                    if (Inner != IntPtr.Zero)
                        DllImportAPI.ParticleEmitterShapeSphere_SetIsRadialInDirection(Inner, value);
                }
            }
        }
        /// <summary>
        /// 克隆球体形状的发射器
        /// </summary>
        /// <param name="src">要克隆的球体发射器</param>
        public override void Clone(ParticleEmitterShape src)
        {
            base.Clone(src);

            var srcT = src as ParticleEmitterShapeSphere;
            if (srcT != null)
            {
                Radius = srcT.Radius;
                IsHemiSphere = srcT.IsHemiSphere;
                IsRadialOutDirection = srcT.IsRadialOutDirection;
                IsRadialInDirection = srcT.IsRadialInDirection;
            }
        }
        /// <summary>
        /// 球体形状的发射器的构造函数，用于创建球体的发射器形状
        /// </summary>
        public ParticleEmitterShapeSphere()
        {
            mInner = DllImportAPI.ParticleEmitterShape_NewEmitterShape(IntPtr.Zero, (int)(ParticleEmitterShapeCN.Sphere));
        }
        //public IParticleEmitterShapeSphere(IntPtr inner)
        //    : base(inner)
        //{
        //}
        /// <summary>
        /// 析构函数，释放对象内存
        /// </summary>
        ~ParticleEmitterShapeSphere()
        {
            if (mInner != IntPtr.Zero)
            {
                DllImportAPI.ParticleEmitterShape_ReleaseEmitterShape(mInner);
                mInner = IntPtr.Zero;
            }            
        }
    }
    /// <summary>
    /// mesh形状的粒子发射器
    /// </summary>
    public class ParticleEmitterShapeMesh : ParticleEmitterShape
    {
        System.String mMeshFile = "";
        /// <summary>
        /// mesh形状的发射器模型文件
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("模型")]
        public System.String MeshFile
        {
            get { return mMeshFile; }
            set { mMeshFile = value; }
        }
        /// <summary>
        /// 克隆mesh形状的发射器
        /// </summary>
        /// <param name="src">要克隆的mesh形状的发射器</param>
        public override void Clone(ParticleEmitterShape src)
        {
            base.Clone(src);

            var srcT = src as ParticleEmitterShapeMesh;
            if (srcT != null)
            {
                MeshFile = srcT.MeshFile;
            }
        }
        /// <summary>
        /// mesh形状的发射器的构造函数，用于创建发射器的mesh形状模型
        /// </summary>
        public ParticleEmitterShapeMesh()
        {
            mInner = DllImportAPI.ParticleEmitterShape_NewEmitterShape(IntPtr.Zero, (int)(ParticleEmitterShapeCN.Mesh));
        }
        //public IParticleEmitterShapeMesh(IntPtr inner)
        //    : base(inner)
        //{

        //}
        /// <summary>
        /// 析构函数，释放实例内存
        /// </summary>
        ~ParticleEmitterShapeMesh()
        {
            if (mInner != IntPtr.Zero)
            {
                DllImportAPI.ParticleEmitterShape_ReleaseEmitterShape(mInner);
                mInner = IntPtr.Zero;
            }            
        }
    }
}
