using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore
{
    /// <summary>
    /// 3D坐标轴的类型枚举
    /// </summary>
    public enum V3D_AXIS_TYPE
    {
        V3D_AXIS_X,
        V3D_AXIS_Y,
        V3D_AXIS_Z,
    }
    /// <summary>
    /// 坐标轴枚举
    /// </summary>
    public enum CoordAxis : int
    {
        X,
        Y,
        Z,
    }
    /// <summary>
    /// 坐标平面枚举
    /// </summary>
    public enum CoordPlane : int
    {
        XY,
        YZ,
        ZX,
    }
    /// <summary>
    /// 纹理色彩空间枚举
    /// </summary>
    public enum TextureColorSpace
    {
        TCS_LINEAR = 0,
        TCS_SRGB
    };
    /// <summary>
    /// 缓冲格式
    /// </summary>
    public enum BufferFormat : uint
    {
        D3DFMT_UNKNOWN = 0,

        D3DFMT_R8G8B8 = 20,
        D3DFMT_A8R8G8B8 = 21,
        D3DFMT_X8R8G8B8 = 22,
        D3DFMT_R5G6B5 = 23,
        D3DFMT_X1R5G5B5 = 24,
        D3DFMT_A1R5G5B5 = 25,
        D3DFMT_A4R4G4B4 = 26,
        D3DFMT_R3G3B2 = 27,
        D3DFMT_A8 = 28,
        D3DFMT_A8R3G3B2 = 29,
        D3DFMT_X4R4G4B4 = 30,
        D3DFMT_A2B10G10R10 = 31,
        D3DFMT_A8B8G8R8 = 32,
        D3DFMT_X8B8G8R8 = 33,
        D3DFMT_G16R16 = 34,
        D3DFMT_A2R10G10B10 = 35,
        D3DFMT_A16B16G16R16 = 36,

        D3DFMT_A8P8 = 40,
        D3DFMT_P8 = 41,

        D3DFMT_L8 = 50,
        D3DFMT_A8L8 = 51,
        D3DFMT_A4L4 = 52,

        D3DFMT_V8U8 = 60,
        D3DFMT_L6V5U5 = 61,
        D3DFMT_X8L8V8U8 = 62,
        D3DFMT_Q8W8V8U8 = 63,
        D3DFMT_V16U16 = 64,
        D3DFMT_A2W10V10U10 = 67,

        D3DFMT_UYVY = ((UInt32)((Byte)('R'))) | ((UInt32)((Byte)('G')) << 8) | ((UInt32)((Byte)('B')) << 16) | ((UInt32)((Byte)('G')) << 24),
        D3DFMT_R8G8_B8G8 = ((UInt32)((Byte)('R'))) | ((UInt32)((Byte)('G')) << 8) | ((UInt32)((Byte)('B')) << 16) | ((UInt32)((Byte)('G')) << 24),
        D3DFMT_YUY2 = ((UInt32)((Byte)('Y'))) | ((UInt32)((Byte)('U')) << 8) | ((UInt32)((Byte)('Y')) << 16) | ((UInt32)((Byte)('2')) << 24),
        D3DFMT_G8R8_G8B8 = ((UInt32)((Byte)('G'))) | ((UInt32)((Byte)('R')) << 8) | ((UInt32)((Byte)('G')) << 16) | ((UInt32)((Byte)('B')) << 24),
        D3DFMT_DXT1 = ((UInt32)((Byte)('D'))) | ((UInt32)((Byte)('X')) << 8) | ((UInt32)((Byte)('T')) << 16) | ((UInt32)((Byte)('1')) << 24),
        D3DFMT_DXT2 = ((UInt32)((Byte)('D'))) | ((UInt32)((Byte)('X')) << 8) | ((UInt32)((Byte)('T')) << 16) | ((UInt32)((Byte)('2')) << 24),
        D3DFMT_DXT3 = ((UInt32)((Byte)('D'))) | ((UInt32)((Byte)('X')) << 8) | ((UInt32)((Byte)('T')) << 16) | ((UInt32)((Byte)('3')) << 24),
        D3DFMT_DXT4 = ((UInt32)((Byte)('D'))) | ((UInt32)((Byte)('X')) << 8) | ((UInt32)((Byte)('T')) << 16) | ((UInt32)((Byte)('4')) << 24),
        D3DFMT_DXT5 = ((UInt32)((Byte)('D'))) | ((UInt32)((Byte)('X')) << 8) | ((UInt32)((Byte)('T')) << 16) | ((UInt32)((Byte)('5')) << 24),

        D3DFMT_D16_LOCKABLE = 70,
        D3DFMT_D32 = 71,
        D3DFMT_D15S1 = 73,
        D3DFMT_D24S8 = 75,
        D3DFMT_D24X8 = 77,
        D3DFMT_D24X4S4 = 79,
        D3DFMT_D16 = 80,

        D3DFMT_D32F_LOCKABLE = 82,
        D3DFMT_D24FS8 = 83,

        /* D3D9Ex only -- */
#if D3D_DISABLE_9EX
#else
        /* Z-Stencil formats valid for CPU access */
        D3DFMT_D32_LOCKABLE = 84,
        D3DFMT_S8_LOCKABLE = 85,

#endif // !D3D_DISABLE_9EX
        /* -- D3D9Ex only */


        D3DFMT_L16 = 81,

        D3DFMT_VERTEXDATA = 100,
        D3DFMT_INDEX16 = 101,
        D3DFMT_INDEX32 = 102,

        D3DFMT_Q16W16V16U16 = 110,

        D3DFMT_MULTI2_ARGB8 = ((UInt32)((Byte)('M'))) | ((UInt32)((Byte)('E')) << 8) | ((UInt32)((Byte)('T')) << 16) | ((UInt32)((Byte)('1')) << 24),

        // Floating point surface formats

        // s10e5 formats (16-bits per channel)
        D3DFMT_R16F = 111,
        D3DFMT_G16R16F = 112,
        D3DFMT_A16B16G16R16F = 113,

        // IEEE s23e8 formats (32-bits per channel)
        D3DFMT_R32F = 114,
        D3DFMT_G32R32F = 115,
        D3DFMT_A32B32G32R32F = 116,

        D3DFMT_CxV8U8 = 117,

        /* D3D9Ex only -- */
#if D3D_DISABLE_9EX
#else

        // Monochrome 1 bit per pixel format
        D3DFMT_A1 = 118,

        // 2.8 biased fixed point
        D3DFMT_A2B10G10R10_XR_BIAS = 119,


        // Binary format indicating that the data has no inherent type
        D3DFMT_BINARYBUFFER = 199,

#endif // !D3D_DISABLE_9EX
        /* -- D3D9Ex only */

        D3DFMT_FORCE_DWORD = 0x7fffffff
    }
    /// <summary>
    /// D3DX的图片文件格式
    /// </summary>
    public enum enD3DXIMAGE_FILEFORMAT
    {
        D3DXIFF_BMP = 0,
        D3DXIFF_JPG = 1,
        D3DXIFF_TGA = 2,
        D3DXIFF_PNG = 3,
        D3DXIFF_DDS = 4,
        D3DXIFF_PPM = 5,
        D3DXIFF_DIB = 6,
        D3DXIFF_HDR = 7,       //high dynamic range formats
        D3DXIFF_PFM = 8,       //
        D3DXIFF_FORCE_DWORD = 0x7fffffff
    }
    /// <summary>
    /// 右手坐标系下的层枚举
    /// </summary>
    public enum RLayer : int
    {
        RL_DSBase = 0,
        RL_DSTranslucent = 1,
        RL_DSDecal = 2,
        RL_SystemDSLighting = 3,
        RL_CustomDSLighting = 4,
        RL_PreTranslucent = 5,
        RL_Translucent = 6,
        RL_PostTranslucent = 7,
        RL_SystemHelper = 8,
        RL_HitProxy = 9,
        RL_EdgeDetect = 10,
        RL_DSPost = 11,
        RL_None = 65535,
    }

    //这个以后要慢慢废除掉
    /// <summary>
    /// 分组枚举
    /// </summary>
    public enum RGroup : int
    {
        RL_EditorPreAll = 0,
        RL_PreWorld = 1,
        RL_World = 2,
        RL_PostWorld = 3,
        RL_EditorPostAll = 4,
        MaxGroup,
    };
    /// <summary>
    /// 碰撞类型枚举
    /// </summary>
    public enum CONTAIN_TYPE
    {
        CONTAIN_TEST_NOTIMPLEMENT = -2,
        CONTAIN_TEST_INNER = 1,
        CONTAIN_TEST_OUTER = -1,
        CONTAIN_TEST_REFER = 0,
    }
    /// <summary>
    /// 渲染模式枚举
    /// </summary>
    public enum EShadingEnv : int
    {
        SDE_Deffered,
        SDE_Forward,
    }
    /// <summary>
    /// 流状态枚举
    /// </summary>
    enum StreamingState
    {
        SS_Unknown, //未知
        SS_Invalid, // 非法
        SS_Pending, // 即将读取
        SS_Streaming,   // 读取中
        //SS_Streamed		,	// 已经读取，我们磁盘->内存，和构造没有分开，这个没有必要了
        SS_Valid,   // 可用
        SS_PendingKill, // 即将删除
        SS_Killing, // 销毁中
        SS_Killed,  // 删除完毕
    };
    /// <summary>
    /// 渲染API类
    /// </summary>
    public abstract class RenderAPI
    {
        /// <summary>
        /// 资源池枚举
        /// </summary>
        public enum V3DPOOL
        {
            V3DPOOL_DEFAULT = 0,
            V3DPOOL_MANAGED = 1,
            V3DPOOL_SYSTEMMEM = 2,
            V3DPOOL_SCRATCH = 3,

            V3DPOOL_FORCE_DWORD = 0x7fffffff
        };

        public readonly static UInt64 V3DUSAGE_RENDERTARGET = (0x00000001L);
        public readonly static UInt64 V3DUSAGE_DEPTHSTENCIL = (0x00000002L);
        public readonly static UInt64 V3DUSAGE_DYNAMIC = (0x00000200L);

        public readonly static UInt64 V3DUSAGE_NONSECURE = (0x00800000L);

        public readonly static UInt64 V3DUSAGE_AUTOGENMIPMAP = (0x00000400L);
        public readonly static UInt64 V3DUSAGE_DMAP = (0x00004000L);

        public readonly static UInt64 V3DUSAGE_QUERY_LEGACYBUMPMAP = (0x00008000L);
        public readonly static UInt64 V3DUSAGE_QUERY_SRGBREAD = (0x00010000L);
        public readonly static UInt64 V3DUSAGE_QUERY_FILTER = (0x00020000L);
        public readonly static UInt64 V3DUSAGE_QUERY_SRGBWRITE = (0x00040000L);
        public readonly static UInt64 V3DUSAGE_QUERY_POSTPIXELSHADER_BLENDING = (0x00080000L);
        public readonly static UInt64 V3DUSAGE_QUERY_VERTEXTEXTURE = (0x00100000L);
        public readonly static UInt64 V3DUSAGE_QUERY_WRAPANDMIP = (0x00200000L);

        public readonly static UInt64 V3DUSAGE_WRITEONLY = (0x00000008L);
        public readonly static UInt64 V3DUSAGE_SOFTWAREPROCESSING = (0x00000010L);
        public readonly static UInt64 V3DUSAGE_DONOTCLIP = (0x00000020L);
        public readonly static UInt64 V3DUSAGE_POINTS = (0x00000040L);
        public readonly static UInt64 V3DUSAGE_RTPATCHES = (0x00000080L);
        public readonly static UInt64 V3DUSAGE_NPATCHES = (0x00000100L);

        public readonly static UInt64 V3DUSAGE_TEXTAPI = (0x10000000L);
        public readonly static UInt64 V3DUSAGE_RESTRICTED_CONTENT = (0x00000800L);
        public readonly static UInt64 V3DUSAGE_RESTRICT_SHARED_RESOURCE = (0x00002000L);
        public readonly static UInt64 V3DUSAGE_RESTRICT_SHARED_RESOURCE_DRIVER = (0x00001000L);

        public readonly static UInt64 V3DLOCK_READONLY = 0x00000010L;
        public readonly static UInt64 V3DLOCK_DISCARD = 0x00002000L;
        public readonly static UInt64 V3DLOCK_NOOVERWRITE = 0x00001000L;
        public readonly static UInt64 V3DLOCK_NOSYSLOCK = 0x00000800L;
        public readonly static UInt64 V3DLOCK_DONOTWAIT = 0x00004000L;

        public readonly static UInt64 V3DLOCK_NO_DIRTY_UPDATE = 0x00008000L;

        public readonly static UInt64 V3DSTREAMSOURCE_INDEXEDDATA = (((UInt64)1) << 30);
        public readonly static UInt64 V3DSTREAMSOURCE_INSTANCEDATA = (((UInt64)2) << 30);

        public readonly static UInt64 V3DCLEAR_TARGET = 0x00000001L;  /* Clear target surface */
        public readonly static UInt64 V3DCLEAR_ZBUFFER = 0x00000002L;  /* Clear target z buffer */
        public readonly static UInt64 V3DCLEAR_STENCIL = 0x00000004L;  /* Clear stencil planes */
    }

    public class vIIDDefine
    {
        public static UInt64 vIID_IUnkown = 0x0000000000000000;
        public static UInt64 vIID_v3dTrailModifier = 0x56cb482853c39334;
        public static UInt64 vIID_v3dSkinModifier = 0x5fdefa164ffbc429;
        public static UInt64 vIID_v3dPNTModifier = 0x8a0b32e8456143b3;
        public static UInt64 vIID_VFile2Memory = 0x149386ee4ccfeb6a;
        public static UInt64 vIID_VRes2Memory = 0xe9ff69794c917a6c;
        public static UInt64 vIID_VResFactory = 0xe6588b7b4c917a83;
        public static UInt64 vIID_v3dResourceInterface = 0x6cfdc8104192e636;
        public static UInt64 vIID_vNavigationLevelResource = 0xedeb4244533177ad;
        public static UInt64 vIID_XNDAttrib = 0x838484794c918324;
        public static UInt64 vIID_XNDNode = 0xedf68bbc4c91830d;
        public static UInt64 vIID_v3dCulling = 0xc49ab8f743bcb3a3;
        public static UInt64 vIID_v3dShaderVarSetter = 0xad8ad4af48ad2432;
        public static UInt64 vIID_v3dBspSpace = 0xe88e3266530f0ed8;
        public static UInt64 vIID_v3dGroupGridObject = 0x7f6ac91c534f6b1d;
        public static UInt64 vIID_v3dGridObject = 0x0e0378114c84931f;
        public static UInt64 vIID_v3dLineObject = 0x3c59cfb34f8e30b9;
        public static UInt64 vIID_v3dText2DObject = 0xaffb000b539053c7;
        public static UInt64 vIID_v3dText3DObject = 0x510c37c74f9a0587;
        public static UInt64 vIID_v3dTipAxisObject = 0x5977064e4c8496ba;
        public static UInt64 vIID_v3dBox3Object = 0xb8fd55dd4c849859;
        public static UInt64 vIID_v3dParticleModifier = 0x30191957532fd2a1;
        public static UInt64 vIID_v3dRInstModifier = 0xcef7578e4cbe8f1f;
        public static UInt64 vIID_v3dGrassModifier = 0xfa15e45053d9fb8a;
        public static UInt64 vIID_v3dStandShaderEnvironment = 0x02beda2d4c2c0039;
        public static UInt64 vIID_v3dStagedMaterialBase = 0x6438d7dd4c2bfb74;
        public static UInt64 vIID_v3dSkeleton = 0x66e0a6cd45c2d57c;
        public static UInt64 vIID_v3dSubAction = 0xec8164a447d66d86;
        public static UInt64 vIID_v3dAnimTreeNode = 0x1849cb3847d66d57;
        public static UInt64 vIID_v3dAnimTreeNode_BlendWithPrev = 0xe6acf29347d66d69;
        public static UInt64 vIID_v3dAnimTreeNode_BlendPerBone = 0xc99a5ee252d66c47;
        public static UInt64 vIID_v3dAnimTreeNode_SubAction = 0x1868c8fe47d66d60;
        public static UInt64 vIID_v3dRootMotionNode = 0xeb8ed1ad4845fde0;
        public static UInt64 vIID_v3dIKAnimNode = 0x277a14e7482a4ed9;
        public static UInt64 vIID_v3dSocketTable = 0x7cd02452539bbd36;
        public static UInt64 vIID_v3dSkinPlayer = 0xe2f818e247d7a31b;
        public static UInt64 vIID_v3dTerrainObject = 0xc8ecb7934c8f1f8c;
        public static UInt64 vIID_v3dVIDTerrainModifier = 0x18fd4cc74c887044;
        public static UInt64 vIID_v3dLayerBasedMaterial = 0x3d17e1554c92dbd4;
        public static UInt64 vIID_v3dModifier = 0x1532a82b458112e3;
        public static UInt64 vIID_v3dStagedObject = 0x7817a2ca4c22f5c0;
        public static UInt64 vIID_v3dModelSource = 0xcfa62d704c2815c5;
        public static UInt64 vIID_v3dMesh = 0xd4ce85ae455181bb;
        public static UInt64 vIID_v3dRObject = 0x439533eb454aebb7;
        public static UInt64 vIID_v3dMeshRenderObject = 0x436a68ba4c2af469;
        public static UInt64 vIID_v3dPrimitiveSource = 0xc94870364c8725b6;
        public static UInt64 vIID_v3dCameraNotify = 0x4f7e917843bcb373;
        public static UInt64 vIID_v3dDevice = 0xbb17597e43a766e6;
        public static UInt64 vIID_v3dLight = 0x31c5fe2a48881f8b;
        public static UInt64 vIID_V3_ResourceMgr = 0x1e98d11643a76757;
        public static UInt64 vIID_V3_TextureMgr = 0x9c4b83a843a76696;
        public static UInt64 vIID_v3dTechnique = 0x4b96f50c488d6b17;
        public static UInt64 vIID_v3dFontA = 0x4bc15d8d3fc70aab;
        public static UInt64 vIID_v3dFontW = 0x4bc15d8d3fc70aab;
        public static UInt64 vIID_ITexture = 0x867671d243a76724;
    }
    /// <summary>
    /// 程序类
    /// </summary
    public class Program
    {
        /// <summary>
        /// 每帧释放资源
        /// </summary>
        /// <param name="freeResourceTime">释放资源的时间</param>
        /// <param name="texLimitSize">限制资源的大小</param>
        /// <param name="texTime">资源调用的时间</param>
        /// <param name="texReduce">资源缩减值</param>
        /// <param name="vmobjLimitSize">vm限定大小</param>
        /// <param name="vmobjTime">vm对象的时间</param>
        /// <param name="vmojbReduce">vm对象的缩减值</param>
        public static void TickTryFreeResource(ref Int64 freeResourceTime, UInt32 texLimitSize, UInt32 texTime, UInt32 texReduce, UInt32 vmobjLimitSize, UInt32 vmobjTime, UInt32 vmojbReduce)
        {
            freeResourceTime -= CCore.Engine.Instance.GetElapsedMillisecond();
            if (freeResourceTime < 0)
            {
                freeResourceTime = 30 * 1000;
                UInt32 freeSize = DllImportAPI.v3dDevice_TextureManager_TryFreeResource(CCore.Engine.Instance.Client.Graphics.Device, texLimitSize, texTime, texReduce);// 90 * 1000, 1 * 1024 * 1024);
                if (freeSize != 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Tex FreeSize = {0}", freeSize));
                    //Log.FileLog.WriteLine("FreeSize = {0}",freeSize);
                }
                freeSize = DllImportAPI.v3dDevice_VMObjectManager_TryFreeResource(CCore.Engine.Instance.Client.Graphics.Device, vmobjLimitSize, vmobjTime, vmojbReduce);// 90 * 1000, 1 * 1024 * 1024);
                if (freeSize != 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("VMO FreeSize = {0}", freeSize));
                }
                freeSize = DllImportAPI.v3dDevice_RAMObjectManager_TryFreeResource(CCore.Engine.Instance.Client.Graphics.Device, UInt32.MaxValue, UInt32.MaxValue, 0);// 90 * 1000, 1 * 1024 * 1024);
                if (freeSize != 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("RAM FreeSize = {0}", freeSize));
                }
            }

            DllImportAPI.ResourceReplacer_ReplaceAllResources();
            DllImportAPI.vFreePipe_OnFreeTick();
        }


        #region Editor
        /// <summary>
        /// 声明地图组件加载完成后调用的方法
        /// </summary>
        /// <param name="componentName">地图组件</param>
        /// <param name="world">世界对象</param>
        public delegate void Delegate_OnWorldLoaded(System.String strAbsFolder, string componentName, CCore.World.World world);
        /// <summary>
        /// 地图组件加载完成后调用的方法，主要供编辑器使用
        /// </summary>
        public static event Delegate_OnWorldLoaded OnWorldLoaded;
        /// <summary>
        /// 地图组件加载完成后调用的方法
        /// </summary>
        /// <param name="strAbsFolder">地图的绝对路径</param>
        /// <param name="componentName">地图组件</param>
        /// <param name="world">世界对象</param>
        public static void _OnWorldLoaded(System.String strAbsFolder, string componentName, CCore.World.World world)
        {
            OnWorldLoaded?.Invoke(strAbsFolder, componentName, world);
        }

        static UInt64 mCurrentActorIndex = 0;
        /// <summary>
        /// 获取Actor对象的索引
        /// </summary>
        /// <returns>返回Actor对象的索引</returns>
        public static UInt64 GetActorIndex()
        {
            if (mCurrentActorIndex == 0)
                mCurrentActorIndex = (UInt64)(CCore.Engine.Instance.Client.MainWorld.GetActors(0).Count);
            else
                mCurrentActorIndex++;

            return mCurrentActorIndex;
        }
        /// <summary>
        /// 只读属性，类型为声音
        /// </summary>
        public static string SoundTypeName { get { return "声音"; } }
        /// <summary>
        /// 只读属性，类型为路点
        /// </summary>
        public static string NavigationPointTypeName { get { return "路点"; } }
        /// <summary>
        /// 只读属性，类型为场景点
        /// </summary>
        public static string ScenePointTypeName { get { return "场景点"; } }
        /// <summary>
        /// 只读属性，类型为碰撞模型
        /// </summary>
        public static string SimplyMeshTypeName { get { return "碰撞模型"; } }
        /// <summary>
        /// 只读属性，类型为贴花辅助
        /// </summary>
        public static string DecalAssistTypeName { get { return "贴花辅助"; } }
        /// <summary>
        /// 只读属性，类型为动态碰撞体
        /// </summary>
        public static string DynamicBlockTypeName { get { return "动态碰撞体"; } }
        /// <summary>
        /// 只读属性，类型为灯光辅助
        /// </summary>
        public static string LightAssistTypeName { get { return "灯光辅助"; } }
        /// <summary>
        /// 只读属性，类型为触发器辅助
        /// </summary>
        public static string TriggerAssistTypeName { get { return "触发器辅助"; } }
        /// <summary>
        /// 只读属性，类型为种植NPC辅助
        /// </summary>
        public static string NPCInitializerAssistTypeName { get { return "种植NPC辅助"; } }
        /// <summary>
        /// 显示Actor类型的数据
        /// </summary>
        public class ActorTypeShowData : INotifyPropertyChanged
        {
            #region INotifyPropertyChangedMembers
            /// <summary>
            /// 定义对象属性改变时调用的委托事件
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion

            string mActorTypeName = "";
            /// <summary>
            /// Actor的类型名称
            /// </summary>
            public string ActorTypeName
            {
                get { return mActorTypeName; }
                set
                {
                    mActorTypeName = value;
                    OnPropertyChanged("ActorTypeName");
                }
            }

            UInt16 mActorType;
            /// <summary>
            /// Actor的类型
            /// </summary>
            public UInt16 ActorType
            {
                get { return mActorType; }
                set
                {
                    mActorType = value;
                    OnPropertyChanged("ActorType");
                }
            }

            string mEditorPath = "";
            /// <summary>
            /// 编辑路径
            /// </summary>
            public string EditorPath
            {
                get { return mEditorPath; }
                set
                {
                    mEditorPath = value;
                    OnPropertyChanged("EditorPath");
                }
            }

            bool mShowData = false;
            /// <summary>
            /// 是否显示数据
            /// </summary>
            public bool ShowData
            {
                get { return mShowData; }
                set
                {
                    mShowData = value;
                    ShowAction?.Invoke(HostWorld, ActorType, mShowData);
                    OnPropertyChanged("ShowData");
                }
            }
            /// <summary>
            /// 显示动作列表
            /// </summary>
            public Action<World.World, UInt16, bool> ShowAction;
            /// <summary>
            /// 主世界对象
            /// </summary>
            public World.World HostWorld;
            /// <summary>
            /// 复制对象数据
            /// </summary>
            /// <param name="srcData">源数据</param>
            public void CopyFrom(ActorTypeShowData srcData)
            {
                ActorTypeName = srcData.ActorTypeName;
                ActorType = srcData.ActorType;
                EditorPath = srcData.EditorPath;
                ShowData = srcData.ShowData;
                ShowAction = srcData.ShowAction;
            }
        }
        // 记录所有可调节显示对象的列表
        static CSUtility.Support.ConcurentObjManager<string, ActorTypeShowData> mAllDataList = new CSUtility.Support.ConcurentObjManager<string, ActorTypeShowData>();
        // 根据世界记录可调节对象的列表
        static CSUtility.Support.ConcurentObjManager<CCore.World.World, CSUtility.Support.ThreadSafeObservableCollection<ActorTypeShowData>> mActorTypeShowDic = new CSUtility.Support.ConcurentObjManager<World.World, CSUtility.Support.ThreadSafeObservableCollection<ActorTypeShowData>>();
        /// <summary>
        /// 只读属性，根据世界记录可调节对象的列表
        /// </summary>
        public static CSUtility.Support.ConcurentObjManager<CCore.World.World, CSUtility.Support.ThreadSafeObservableCollection<ActorTypeShowData>> ActorTypeShowDic
        {
            get { return mActorTypeShowDic; }
        }
        /// <summary>
        /// 是否显示Actor对象的类型
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="typeString">类型描述</param>
        /// <returns>如果显示对象返回true，否则返回false</returns>
        public static bool IsActorTypeShow(CCore.World.World world, string typeString)
        {
            if (world == null)
                return false;

            if (!mAllDataList.ContainsKey(typeString))
                return false;

            CSUtility.Support.ThreadSafeObservableCollection<ActorTypeShowData> datas;
            if (mActorTypeShowDic.TryGetValue(world, out datas))
            {
                foreach (var data in datas)
                {
                    if (data.ActorTypeName.Equals(typeString))
                        return data.ShowData;
                }
            }

            return false;
        }
        /// <summary>
        /// 设置Actor对象显示类型时调用的委托对象
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="typeString">类型信息描述</param>
        /// <param name="show">是否显示</param>
        public delegate void Delegate_OnSetActorTypeShow(CCore.World.World world, string typeString, bool show);
        /// <summary>
        /// 设置Actor对象显示类型时调用的委托对象
        /// </summary>
        public static event Delegate_OnSetActorTypeShow OnSetActorTypeShow;
        /// <summary>
        /// 初始化Actor对象显示类型
        /// </summary>
        /// <param name="world">当前世界</param>
        /// <param name="typeString">类型信息描述</param>
        /// <param name="show">是否显示</param>
        /// <param name="actorType">Actor类型</param>
        /// <param name="showAction">显示动作的列表</param>
        /// <param name="path">对象路径</param>
        public static void InitializeActorTypeShow(CCore.World.World world, string typeString, bool show, UInt16 actorType, Action<World.World, UInt16, bool> showAction = null, string path = "")
        {
            if (world == null)
                return;

            bool bCreate = false;
            if (!mAllDataList.ContainsKey(typeString))
            {
                if (!mActorTypeShowDic.ContainsKey(world))
                {
                    var datas = new CSUtility.Support.ThreadSafeObservableCollection<ActorTypeShowData>();
                    foreach (var srcData in mAllDataList.Values)
                    {
                        var newData = new ActorTypeShowData();
                        newData.CopyFrom(srcData);
                        newData.HostWorld = world;
                        datas.Add(newData);
                    }
                    mActorTypeShowDic[world] = datas;
                }

                mAllDataList[typeString] = new ActorTypeShowData()
                {
                    ActorTypeName = typeString,
                    ActorType = actorType,
                    EditorPath = path,
                    ShowData = false,
                    ShowAction = showAction,
                };
                ActorTypeShowDic.For_Each((CCore.World.World wd, CSUtility.Support.ThreadSafeObservableCollection<ActorTypeShowData> data, object obj) =>
                {
                    data.Add(new ActorTypeShowData()
                    {
                        ActorTypeName = typeString,
                        ActorType = actorType,
                        EditorPath = path,
                        ShowData = show,
                        ShowAction = showAction,
                        HostWorld = world,
                    });

                    return CSUtility.Support.EForEachResult.FER_Continue;
                }, null);

                bCreate = true;
            }

            if (!bCreate)
            {
                CSUtility.Support.ThreadSafeObservableCollection<ActorTypeShowData> datas;
                if (!mActorTypeShowDic.TryGetValue(world, out datas))
                {
                    datas = new CSUtility.Support.ThreadSafeObservableCollection<ActorTypeShowData>();

                    foreach (var data in mAllDataList.Values)
                    {
                        var newData = new ActorTypeShowData();
                        newData.CopyFrom(data);
                        newData.HostWorld = world;
                        datas.Add(newData);
                    }

                    mActorTypeShowDic[world] = datas;
                }

                foreach (var data in datas)
                {
                    if (data.ActorTypeName.Equals(typeString))
                    {
                        data.ShowData = show;
                        break;
                    }
                }
            }

        }
        /// <summary>
        /// 设置Actor类型的对象是否显示
        /// </summary>
        /// <param name="world">当前的世界对象</param>
        /// <param name="typeString">类型信息描述</param>
        /// <param name="show">是否显示</param>
        public static void SetActorTypeShow(CCore.World.World world, string typeString, bool show)
        {
            CSUtility.Support.ThreadSafeObservableCollection<ActorTypeShowData> datas;
            if (mActorTypeShowDic.TryGetValue(world, out datas))
            {
                foreach (var data in datas)
                {
                    if (data.ActorTypeName.Equals(typeString))
                    {
                        data.ShowData = show;
                        break;
                    }
                }

                OnSetActorTypeShow?.Invoke(world, typeString, show);
            }
        }

        #endregion
    }
}