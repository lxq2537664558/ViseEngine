using System;
using System.Runtime.InteropServices;

namespace CCore
{
    /// <summary>
    /// 动态库连接的API
    /// </summary>
    public class DllImportAPI
    {
        /// <summary>
        /// 声明绘制子集时调用的委托事件
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="material">材质名称</param>
        /// <param name="sourceFile">源文件</param>
        /// <param name="shader">shader名称</param>
        /// <param name="triCount">触发次数</param>
        /// <param name="instance">实例</param>
        public delegate void Delegate_FOnDrawSubset(int time, string material, string sourceFile, string shader, int triCount, int instance);
        /// <summary>
        /// 声明异步加载对象时调用的委托事件
        /// </summary>
        /// <param name="count">次数</param>
        /// <param name="classType">类的类型</param>
        /// <param name="sourceFile">源文件</param>
        public delegate void Delegate_FOnAsyncLoadObject(int count, string classType, string sourceFile);
        /// <summary>
        /// 声明OpenGL报错时调用的委托事件
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <param name="line">线路</param>
        /// <param name="e">出错信息代码</param>
        /// <param name="info">出错信息描述</param>
        public delegate void Delegate_FOnGLError(string file, int line, int e, string info);
        /// <summary>
        /// 声明句柄被释放时调用的委托事件
        /// </summary>
        /// <param name="handle">句柄指针</param>
        public delegate void Delegate_FreeGCHandle(IntPtr handle);
#if WIN
        const string ModuleNC = "core.Windows.dll";
#elif IOS
        const string ModuleNC = "__Internal";
#else
        const string ModuleNC = "libcore.so";
#endif

#region Android
        /// <summary>
        /// 由windows转换到安卓设备
        /// </summary>
        /// <param name="env">渲染环境指针</param>
        /// <param name="surface">绘制对象</param>
        /// <returns>返回设备指针</returns>
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr Android_ANWinFromSurface(IntPtr env, IntPtr surface);
#endregion

#region Particle

        // ParticleEffector
        /// <summary>
        /// 粒子特效增加计数
        /// </summary>
        /// <param name="ptr">特效指针</param>
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEffector_AddRef(IntPtr ptr);
        /// <summary>
        /// 粒子特效减少计数
        /// </summary>
        /// <param name="ptr">特效指针</param>
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEffector_Release(IntPtr ptr);
        /// <summary>
        /// 获取可以发射的粒子特效
        /// </summary>
        /// <param name="ptr">特效指针</param>
        /// <returns>返回可以发射的粒子特效</returns>
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEffector_GetEnable(IntPtr ptr);
        /// <summary>
        /// 设置粒子特效是否可发射
        /// </summary>
        /// <param name="ptr">特效指针</param>
        /// <param name="enable">是否可以发射</param>
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEffector_SetEnable(IntPtr ptr, bool enable);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleEffector_GetEffectMode(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEffector_SetEffectMode(IntPtr ptr, int mode);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEffector_New_Velocity();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEffector_New_Force();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEffector_New_Color();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEffector_New_Scale();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEffector_New_Rotation();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEffector_New_Spawn();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEffector_New_Orbit();

        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleVelocityEffector_GetVelocity(IntPtr ptr, SlimDX.Vector3* vec);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleVelocityEffector_SetVelocity(IntPtr ptr, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleVelocityEffector_GetVelocityXPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleVelocityEffector_GetVelocityYPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleVelocityEffector_GetVelocityZPtr(IntPtr ptr);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleForceEffector_GetAccelerationDir(IntPtr ptr, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleForceEffector_SetAccelerationDir(IntPtr ptr, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleForceEffector_GetAccelerationPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleForceEffector_SetAcceleration(IntPtr ptr, float acc);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleForceEffector_GetIsRadialDirection(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleForceEffector_SetIsRadialDirection(IntPtr ptr, bool value);

        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleColorEffector_GetValueBegin(IntPtr ptr, SlimDX.Vector4* vec);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleColorEffector_SetValueBegin(IntPtr ptr, SlimDX.Vector4* vec);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleColorEffector_GetValueEnd(IntPtr ptr, SlimDX.Vector4* vec);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleColorEffector_SetValueEnd(IntPtr ptr, SlimDX.Vector4* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleColorEffector_GetInSpeedPtr(IntPtr ptr);

        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static float ParticleScaleEffector_GetValueBegin(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleScaleEffector_SetValueBegin(IntPtr ptr, float value);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static float ParticleScaleEffector_GetValueEnd(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleScaleEffector_SetValueEnd(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleScaleEffector_GetScaleAll(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleScaleEffector_SetScaleAll(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleScaleEffector_GetScaleXPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleScaleEffector_GetScaleYPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleScaleEffector_GetScaleZPtr(IntPtr ptr);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleColorEffector_GetColorRPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleColorEffector_GetColorGPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleColorEffector_GetColorBPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleColorEffector_GetColorAPtr(IntPtr ptr);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleRotationEffector_GetIsAxisOnDirection(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleRotationEffector_SetIsAxisOnDirection(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleRotationEffector_GetAxis(IntPtr ptr, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleRotationEffector_SetAxis(IntPtr ptr, SlimDX.Vector3* vec);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static float ParticleRotationEffector_GetVelocity(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleRotationEffector_SetVelocity(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleRotationEffector_GetVelocityPtr(IntPtr ptr);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleSpawnEffector_RemoveEmitter(IntPtr ptr, int idx);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleSpawnEffector_SetEmitter(IntPtr ptr, int idx, IntPtr emitter, IntPtr modifier);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleSpawnEffector_GetSpawnMode(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleSpawnEffector_SetSpawnMode(IntPtr ptr, int mode);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleSpawnEffector_SetLinkCheckFunction(IntPtr ptr, CCore.Particle.ParticleEffector_Spawn.Delegate_OnParticleLineCheck onCheck);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleOrbitEffector_GetOffsetXPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleOrbitEffector_GetOffsetYPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleOrbitEffector_GetOffsetZPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleOrbitEffector_GetRotVXPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleOrbitEffector_GetRotVYPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleOrbitEffector_GetRotVZPtr(IntPtr ptr);

        // ParticleEmitterShape
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitterShape_NewEmitterShape(IntPtr ptr, int shapeType);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_ReleaseEmitterShape(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_AddRef(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetIsEmitFromShell(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetIsEmitFromShell(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetIsRandomDirection(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetIsRandomDirection(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomDirAvailableX(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomDirAvailableX(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomDirAvailableY(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomDirAvailableY(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomDirAvailableZ(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomDirAvailableZ(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomDirAvailableInvX(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomDirAvailableInvX(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomDirAvailableInvY(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomDirAvailableInvY(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomDirAvailableInvZ(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomDirAvailableInvZ(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomPosAvailableX(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomPosAvailableX(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomPosAvailableY(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomPosAvailableY(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomPosAvailableZ(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomPosAvailableZ(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomPosAvailableInvX(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomPosAvailableInvX(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomPosAvailableInvY(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomPosAvailableInvY(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShape_GetRandomPosAvailableInvZ(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShape_SetRandomPosAvailableInvZ(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleEmitterShape_GetType(IntPtr ptr);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitterShapeBox_GetSizeX(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeBox_SetSizeX(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitterShapeBox_GetSizeY(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeBox_SetSizeY(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitterShapeBox_GetSizeZ(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeBox_SetSizeZ(IntPtr ptr, float value);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitterShapeCone_GetAngle(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeCone_SetAngle(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitterShapeCone_GetRadius(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeCone_SetRadius(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitterShapeCone_GetLength(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeCone_SetLength(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShapeCone_GetIsEmitFromBase(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeCone_SetIsEmitFromBase(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShapeCone_GetDirType(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeCone_SetDirType(IntPtr ptr, int value);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitterShapeSphere_GetRadius(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeSphere_SetRadius(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShapeSphere_GetIsHemiSphere(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeSphere_SetIsHemiSphere(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShapeSphere_GetIsRadialOutDirection(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeSphere_SetIsRadialOutDirection(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitterShapeSphere_GetIsRadialInDirection(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitterShapeSphere_SetIsRadialInDirection(IntPtr ptr, bool value);

        // ParticleEmitter
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_AddRef(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_Release(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static IntPtr ParticleEmitter_GetEffector(IntPtr ptr, int idx);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetShapePtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_SetShapePtr(IntPtr ptr, IntPtr shape);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetName(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void ParticleEmitter_SetName(IntPtr ptr, string name);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitter_GetEnable(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetEnable(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitter_GetMovingTime(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetMovingTime(IntPtr ptr, float time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetPositionXPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetPositionYPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetPositionZPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_GetDirection(IntPtr ptr, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetDirection(IntPtr ptr, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitter_GetDuration(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetDuration(IntPtr ptr, float value);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static float ParticleEmitter_GetEmissionRate(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleEmitter_SetEmissionRate(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitter_GetIsLooping(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetIsLooping(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitter_GetStartDelay(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetStartDelay(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float ParticleEmitter_GetEmitterDelay(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetEmitterDelay(IntPtr ptr, float value);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static float ParticleEmitter_GetGravity(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleEmitter_SetGravity(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleEmitter_GetCoordinateSpace(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetCoordinateSpace(IntPtr ptr, int value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 ParticleEmitter_GetInheritVelocity(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetInheritVelocity(IntPtr ptr, bool value);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static float ParticleEmitter_GetEmitLife(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleEmitter_SetEmitLife(IntPtr ptr, float value);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static float ParticleEmitter_GetEmitVelocity(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleEmitter_SetEmitVelocity(IntPtr ptr, float value);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static float ParticleEmitter_GetEmitScale(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleEmitter_SetEmitScale(IntPtr ptr, float value);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static float ParticleEmitter_GetEmitRotation(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleEmitter_SetEmitRotation(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetEmitRotationAnglePtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_GetEmitRotationAxisMin(IntPtr ptr, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetEmitRotationAxisMin(IntPtr ptr, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_GetEmitRotationAxisMax(IntPtr ptr, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetEmitRotationAxisMax(IntPtr ptr, SlimDX.Vector3* vec);        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_GetEmitColor(IntPtr ptr, SlimDX.Vector4* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetEmitColor(IntPtr ptr, SlimDX.Vector4* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleEmitter_GetShapeType(IntPtr ptr);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void ParticleEmitter_SetShapeType(IntPtr ptr, int value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_Reset(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_AddEffector(IntPtr ptr, IntPtr effector);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static bool ParticleEmitter_RemoveEffector(IntPtr ptr, IntPtr effector);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetEmissionRatePtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetEmitLifePtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetEmitVelocityPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleEmitter_GetEmitScaleAll(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetEmitScaleAll(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetEmitScaleXPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetEmitScaleYPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetEmitScaleZPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetGravityPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetEmissionType(IntPtr ptr, int emType);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleEmitter_GetEmissionType(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetEmissionCount(IntPtr ptr, int count);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetEmissionCountPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ParticleEmitter_GetParticleFrame(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetFaceToDirection(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleEmitter_GetFaceToDirection(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetFollowerEmitter(IntPtr ptr, int idx, IntPtr fEmitter, IntPtr fModifier);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_RemoveFollowerEmitter(IntPtr ptr, int idx);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleEmitter_IsFinished(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ParticleEmitter_GetEnableEmitter(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ParticleEmitter_SetEnableEmitter(IntPtr ptr, bool enable);

        // v3dScalarVariable

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dScalarVariable_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dScalarVariable_Delete(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dScalarVariable_GetVariableType(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dScalarVariable_SetVariableType(IntPtr ptr, int type);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dScalarVariable_GetValue(IntPtr ptr, float slider);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dScalarVariable_SetValue(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dScalarVariable_GetRandomValue(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dScalarVariable_GetValueBegin(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dScalarVariable_SetValueBegin(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dScalarVariable_GetValueEnd(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dScalarVariable_SetValueEnd(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dScalarVariable_SetValues(IntPtr ptr, int varType, float constantValue, float valueBegin, float valueEnd);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dScalarVariable_GetBezierPtr(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dScalarVariable_CanChangeToType(IntPtr ptr, int type);

#endregion

#region MeshModifier

        // MeshModifier
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DModifier_AddRef(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DModifier_Release(IntPtr ptr);

        // ParticleModifier
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DParticleModifier_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 V3DParticleModifier_InitObjects(IntPtr ptr, IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DParticleModifier_AddEmitter(IntPtr ptr, IntPtr emitter);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DParticleModifier_RemoveEmitter(IntPtr ptr, int index);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DParticleModifier_GetParticlePoolSize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DParticleModifier_SetParticlePoolSize(IntPtr ptr, int maxCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DParticleModifier_GetDirectionMode(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DParticleModifier_SetDirectionMode(IntPtr ptr, int mode);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DParticleModifier_SetDirectionCamera(IntPtr ptr, IntPtr eye);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DParticleModifier_SetWorldTransMat(IntPtr ptr, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DParticleModifier_GetParticlesCount(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DParticleModifier_GetCoordinateSpace(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DParticleModifier_SetCoordinateSpace(IntPtr ptr, int space);

        // TrailModifier
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DTrailModifier_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int32 V3DTrailModifier_InitObjects(IntPtr ptr, IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DTrailModifier_GetTrailPoolSize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_SetTrailPoolSize(IntPtr ptr, int poolSize);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_ClearTrail(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_SetEnableTrail(IntPtr ptr, bool enable);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_SetAutoSpawn(IntPtr ptr, bool enable);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_SetmTrailPos1(IntPtr ptr, SlimDX.Vector3* pos);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_SetmTrailPos2(IntPtr ptr, SlimDX.Vector3* pos);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_SetTrailLife(IntPtr ptr, float life);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_SetEmitInterval(IntPtr ptr, float interval);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_SetMinDistance(IntPtr ptr, float minDist);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_AllocSegment(IntPtr ptr, SlimDX.Vector3* pos1, SlimDX.Vector3* pos2);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DTrailModifier_GetMaxSegmentPerSpawnPoint(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_SetMaxSegmentPerSpawnPoint(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DTrailModifier_SetUseSpline(IntPtr ptr, bool p);



        // GrassData
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DGrassData_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetGrassObject(IntPtr ptr, IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DGrassData_GetGrassObject(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetTextureFrameCount(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DGrassData_GetTextureFrameCount(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetVertexWeightOffset(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DGrassData_GetVertexWeightOffset(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetPlantRate(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DGrassData_GetPlantRate(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetCoverPercent(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DGrassData_GetCoverPercent(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetPlantOffset(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DGrassData_GetPlantOffset(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetScale(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DGrassData_GetScale(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetScaleY(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DGrassData_GetScaleY(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetMaxFalldownRightOffset(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DGrassData_GetMaxFalldownRightOffset(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetMaxFalldownBackOffset(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DGrassData_GetMaxFalldownBackOffset(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DGrassData_SetMeshTemplateId(IntPtr ptr, System.Guid* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Guid* V3DGrassData_GetMeshTemplateId(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void V3DGrassData_InitLoadMeshTemplateCallback(CCore.Grass.Delegate_GrassData_LoadMeshTemplate loadMeshTemplateEvent);

        // Light
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vDirLightProxy_New(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vPointLightProxy_New(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vSpotLightProxy_New(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_CalcLightRange(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetIntensityAlpha(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetIntensityAlpha(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetIntensity(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetIntensity(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetSpecularIntensity(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetSpecularIntensity(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetDiffuse(IntPtr ptr, SlimDX.Vector4* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_GetDiffuse(IntPtr ptr, SlimDX.Vector4* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetAmbient(IntPtr ptr, SlimDX.Vector4* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_GetAmbient(IntPtr ptr, SlimDX.Vector4* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetSpecular(IntPtr ptr, SlimDX.Vector4* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_GetSpecular(IntPtr ptr, SlimDX.Vector4* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetAttenuation(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_GetAttenuation(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetBlackPoint(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetBlackPoint(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetShadowType(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vLightProxy_GetShadowType(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetShadowMapSize(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetShadowMapSize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetShadowCoverSize(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetShadowCoverSize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetDoVSMBlur(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vLightProxy_GetDoVSMBlur(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetBlurAmount(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetBlurAmount(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetLBRAmount(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetLBRAmount(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetIsRenderStaticShadow(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vLightProxy_GetIsRenderStaticShadow(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetRealtimeStaticShadow(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vLightProxy_GetRealtimeStaticShadow(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetShadowAlpha(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetShadowAlpha(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetShadowDarkScale(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetShadowDarkScale(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetCanHitProxy(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vLightProxy_GetCanHitProxy(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetZNear(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetZNear(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetZFar(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetZFar(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetPointInnerPercent(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetPointInnerPercent(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetSpotInnerPercent(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_GetSpotInnerPercent(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vLightProxy_CommitEditorMesh(IntPtr ptr,
            int ShowRangeMesh, int ShowSignMesh, float SignMeshSize,
            IntPtr env, int inGroup, SlimDX.Matrix* mat
            );
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vLightProxy_GetEditorMesh(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_CommitShadowMesh(IntPtr ptr,
            Int64 time, IntPtr pStagedObj, SlimDX.Matrix* mat, bool dynamic);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SwapPipes(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_RenderShadow(IntPtr ptr, IntPtr device, IntPtr lookCamera);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_SetHitProxy(IntPtr ptr, int hitProxy);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vLightProxy_GetShadowMapRenderEnv(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLightProxy_EditorMeshSetMaterial(IntPtr ptr, int index, IntPtr material);



        // Socket
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DSocket_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DSocket_CloneSocket(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void V3DSocket_SetName(IntPtr ptr, string p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr V3DSocket_GetName(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void V3DSocket_SetParentBoneName(IntPtr ptr, string p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr V3DSocket_GetParentBoneName(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_SetPos(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_GetPos(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_SetScale(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_GetScale(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_SetQuat(IntPtr ptr, SlimDX.Quaternion* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_GetQuat(IntPtr ptr, SlimDX.Quaternion* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_SetAbsPos(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_GetAbsPos(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_SetAbsScale(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_GetAbsScale(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_SetAbsQuat(IntPtr ptr, SlimDX.Quaternion* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_GetAbsQuat(IntPtr ptr, SlimDX.Quaternion* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_GetAbsMatrix(IntPtr ptr, SlimDX.Matrix* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_SetParentIndexInFullSocketTable(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DSocket_GetParentIndexInFullSocketTable(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocket_SetInheritRotate(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DSocket_GetInheritRotate(IntPtr ptr);

        // SocketTable
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DSocketTable_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocketTable_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocketTable_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocketTable_AddSocket(IntPtr ptr, IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DSocketTable_GetSocket(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DSocketTable_GetSocketCount(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DSocketTable_RemoveSocket(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocketTable_Merge(IntPtr ptr, IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocketTable_Build(IntPtr ptr, IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSocketTable_Update(IntPtr ptr, IntPtr p);

        // Bone
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DBone_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr V3DBone_GetName(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DBone_SetPos(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DBone_GetPos(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DBone_SetScale(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DBone_GetScale(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DBone_SetQuat(IntPtr ptr, SlimDX.Quaternion* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DBone_GetQuat(IntPtr ptr, SlimDX.Quaternion* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DBone_GetAbsMatrix(IntPtr ptr, SlimDX.Matrix* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DBone_GetChildSize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DBone_GetChildIndex(IntPtr ptr, int index);

        // Skeleton
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DSkeleton_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSkeleton_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSkeleton_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSkeleton_Merge(IntPtr ptr, IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSkeleton_BuildHiberarchys(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSkeleton_CalcBoundingBox(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSkeleton_GetBoundingBoxMin(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSkeleton_GetBoundingBoxMax(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DSkeleton_GetBoneCount(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DSkeleton_GetBone(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DSkeleton_GetRootBoneCount(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DSkeleton_GetRootBoneIndex(IntPtr ptr, int p);


        // Mesh

        // AnimTreeNode
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DAnimTreeNode_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_LoadFromXnd(IntPtr p, IntPtr device, IntPtr node);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_SaveToXnd(IntPtr p, IntPtr node);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DAnimTreeNode_IsActionFinished(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_UpdateTick(IntPtr ptr, Int64 tm);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_UpdateNode(IntPtr ptr, Int64 tm);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_AddNode(IntPtr ptr, IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_RemoveNode(IntPtr ptr, IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_ClearNode(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_SetSkeleton(IntPtr ptr, IntPtr device, IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DAnimTreeNode_GetSkeleton(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_SetXRootMotionType(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DAnimTreeNode_GetXRootMotionType(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_SetYRootMotionType(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DAnimTreeNode_GetYRootMotionType(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_SetZRootMotionType(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DAnimTreeNode_GetZRootMotionType(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_SetBlendFactor(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DAnimTreeNode_GetBlendFactor(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_SetATFinished(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DAnimTreeNode_GetATFinished(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_SetPause(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DAnimTreeNode_GetPause(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_GetDeltaRootmotionPos(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_GetDeltaRootmotionQuat(IntPtr ptr, SlimDX.Quaternion* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_SetBlendElapse(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DAnimTreeNode_GetBlendElapse(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_SetBlendDuration(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DAnimTreeNode_GetBlendDuration(IntPtr p);

        // AnimTreeNode_Action
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dAnimTreeNode_SubAction_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dAnimTreeNode_SubAction_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dAnimTreeNode_SubAction_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static string v3dAnimTreeNode_SubAction_GetActionName(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int64 v3dAnimTreeNode_SubAction_GetDuration(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dAnimTreeNode_SubAction_SetPlayRate(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dAnimTreeNode_SubAction_GetPlayRate(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dAnimTreeNode_SubAction_SetCurAnimTime(IntPtr ptr, Int64 p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int64 v3dAnimTreeNode_SubAction_GetCurAnimTime(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dAnimTreeNode_SubAction_SetLoop(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dAnimTreeNode_SubAction_GetLoop(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dAnimTreeNode_SubAction_SetSubAction(IntPtr ptr, IntPtr device, string name, bool bClearLink);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dAnimTreeNode_SubAction_ClearLink(IntPtr ptr);

        // AnimTreeNode_BlendWithPrev
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DAnimTreeNode_BlendWithPrev_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_BlendWithPrev_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_BlendWithPrev_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_BlendWithPrev_SetBlendDurationPercient(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DAnimTreeNode_BlendWithPrev_GetBlendDurationPercient(IntPtr p);

        // IAnimTreeNode_BlendPerBone
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DAnimTreeNode_BlendPerBone_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_BlendPerBone_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_BlendPerBone_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_BlendPerBone_ClearBranchStartBone(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DAnimTreeNode_BlendPerBone_AddBranchStartBone(IntPtr ptr, string p);

#endregion

#region Navigation

        // NavigationRenderEnv
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr NavigationRenderEnv_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void NavigationRenderEnv_Release(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void NavigationRenderEnv_SetNavigationTexSize(IntPtr env, UInt32 textureWidth, UInt32 textureHeight, float meterPerPixelX, float meterPerPixelZ);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void NavigationRenderEnv_SetCameraPos(IntPtr env, float x, float y, float z);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void NavigationRenderEnv_ClearAllDrawingCommits(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void NavigationRenderEnv_CommitMesh(IntPtr env, Int64 time, IntPtr mesh, SlimDX.Matrix* mat, int meshType);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void NavigationRenderEnv_CommitTerrain(IntPtr env, Int64 time, IntPtr terrainPatch, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void NavigationRenderEnv_Draw(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void NavigationRenderEnv_BuildNavigationData(IntPtr env, IntPtr navData, UInt32 tX, UInt32 tZ, UInt32 textureWidth, UInt32 textureHeight, void* texture, float meterPerPixel, float blockAngleDelta, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void BuildNavigationFromData(IntPtr navData, UInt32 lx, UInt32 lz, UInt32 textureWidth, UInt32 textureHeight, IntPtr texture, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void DrawNavigation(IntPtr texture, float radius, UInt32 tX, UInt32 tZ, float centerX, float centerZ, UInt32 textureWidth, UInt32 textureHeight, float meterPerPixel, bool isBlock, bool isErase, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void DrawNavigationToData(IntPtr navData, IntPtr texture, UInt32 tX, UInt32 tZ, UInt32 textureWidth, UInt32 textureHeight, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void DrawNavigationPath(IntPtr texture, UInt32 x, UInt32 z, UInt32 textureWidth, UInt32 textureHeight, byte value);


#endregion

#region v3dLineObject

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dLineObject_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dLineObject_Release(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dLineObject_SetColor(IntPtr ptr, UInt32 color);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dLineObject_GetColor(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dLineObject_SetStart(IntPtr ptr, SlimDX.Vector3* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dLineObject_GetStart(IntPtr ptr, SlimDX.Vector3* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dLineObject_SetEnd(IntPtr ptr, SlimDX.Vector3* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dLineObject_GetEnd(IntPtr ptr, SlimDX.Vector3* v);


#endregion

#region vDSRenderEnv

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vDSRenderEnv_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_Release(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_SetMainSceneRenderEnv(IntPtr env, bool bMainScene);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitHelperLine(IntPtr env, int inGroup, IntPtr lineObj, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitDSMesh(IntPtr env, Int64 time, int inGroup, IntPtr pMesh, SlimDX.Matrix* mat, IntPtr pCamera, int customTime, float fadePercent, int IsFadeIn, int IsFadeOut);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitDSPostMesh(IntPtr env, Int64 time, int inGroup, IntPtr pMesh, SlimDX.Matrix* mat, IntPtr pCamera, int customTime, float fadePercent, int IsFadeIn, int IsFadeOut);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitDSLighting(IntPtr env, int inGroup, IntPtr pLight, SlimDX.Matrix* mat, IntPtr pCamera);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitHitProxy(IntPtr env, int inGroup, IntPtr pMesh, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitDSDecal(IntPtr env, int inGroup, IntPtr decal, SlimDX.Matrix* mat, IntPtr eye);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitHelperMesh(IntPtr env, int inGroup, IntPtr pMesh, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitText2D(IntPtr env, int inGroup, int layer, IntPtr pText2DObj, SlimDX.Matrix* mat, bool immediate);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitHelperText3D(IntPtr env, int inGroup, IntPtr pText3DObj, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_CommitTerrain(IntPtr env, Int64 time, int inGroup, IntPtr patch, IntPtr pCamera);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_CommitGrass(IntPtr env, IntPtr patch, Int64 time, int inGroup, IntPtr pCamera);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitDSTranslucentMesh(IntPtr env, Int64 time, int inGroup, IntPtr pMesh, SlimDX.Matrix* matrix, IntPtr eye, int customTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitFSMesh(IntPtr env, Int64 time, int inGroup, int layer, IntPtr pMesh, SlimDX.Matrix* matrix, int customTime, float FadePercent, int IsFadeIn, int IsFadeOut);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitEdgeDetectMesh(IntPtr env, Int64 time, int inGroup, int layer, IntPtr pMesh, SlimDX.Matrix* matrix, int customTime, SlimDX.Vector4* edgeColor, int edgeDetectMode, float edgeDetectHightlight);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitShadow_Terrain(IntPtr light, IntPtr patch, Int64 time, SlimDX.Matrix* matrix);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_CommitHelperGroupGrid(IntPtr env, int inGroup, IntPtr pObj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_CommitHelperGrid(IntPtr env, int inGroup, IntPtr pObj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_CommitHelperTipAxis(IntPtr env, int inGroup, IntPtr pObj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_CommitHelperBox(IntPtr env, int inGroup, IntPtr pObj, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int vDSRenderEnv_GetRenderMode(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_SetRenderMode(IntPtr env, int mode);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int vDSRenderEnv_Initialize(IntPtr env, int width, int height, bool bUseIntZ);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_ClearAllCommits(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_SwapPipe(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_ClearAllDrawingCommits(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_DrawAll(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_DrawPostProcess(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_UpdateDebugTextures(IntPtr env, IntPtr pDevice);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int vDSRenderEnv_MRT_BeginFinalDrawing(IntPtr env, IntPtr pDevice);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_MRT_EndFinalDrawing(IntPtr env, IntPtr pDevice);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr vDSRenderEnv_GetFinalTexture(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vDSRenderEnv_GetFinalTexturePixelData(IntPtr env, byte* datas, IntPtr tagTex);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr vDSRenderEnv_GetAlbedoTexture(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr vDSRenderEnv_SaveFinalTexture(IntPtr env, string file);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static IntPtr vDSRenderEnv_SaveAlbedoTexture(IntPtr env, string file);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_SetClearColorMRT(IntPtr env, UInt32 color);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_SetEdgeDetectMode(IntPtr env, int mode);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_DrawHitProxy(IntPtr env, IntPtr device, IntPtr view);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_UpdateHitProxyResult(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt32 vDSRenderEnv_GetHitProxy(IntPtr env, int x, int y);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_GetHitProxyArea(IntPtr env, int x, int y, int w, int h, int step, int* hitCount, int* hitId1, int* hitId2, int* hitId3);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int vDSRenderEnv_ResizeInternalRT(IntPtr env, UInt32 inSizeX, UInt32 inSizeY, bool bUseIntZ, bool bUseHDR);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void vDSRenderEnv_FinalTexture_Save2File(IntPtr env, string fileName, UInt32 flag);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void vDSRenderEnv_DepthTexture_Save2File(IntPtr env, string fileName, UInt32 flag);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void vDSRenderEnv_PostProcessPipe_Clear(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void vDSRenderEnv_SetPostProcess_SSAO(IntPtr env, IntPtr ssao);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void vDSRenderEnv_SetPostProcess_ToneMapping(IntPtr env, IntPtr toneMapping);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void vDSRenderEnv_PostProcessPipe_Push_back(IntPtr env, IntPtr postProcess);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //public unsafe extern static int vDSRenderEnv_CheckDeviceLost(IntPtr env, IntPtr device, int viewPresentResult, UInt32 width, UInt32 height, bool useIntZ);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt32 vDSRenderEnv_SaveMRT(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_DoFSBlur(IntPtr env, int doBlur);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_DoCopyPreFinal(IntPtr env, int doCopy); 
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void vDSRenderEnv_DoFSPostBlur(IntPtr env, int doPost);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int vDSRenderEnv_GetDSDrawCall(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int vDSRenderEnv_GetDSDrawTri(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int vDSRenderEnv_GetFSDrawCall(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int vDSRenderEnv_GetFSDrawTri(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt32 vDSRenderEnv_SubmitFSPipe(IntPtr env, int inGroup);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void vDSRenderEnv_SetEdgeDetectParams(IntPtr env, float rate, float strength, float scale);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static void vDSRenderEnv_SetFSBlurParams(IntPtr env, float rate, float strength, float scale);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static UInt32 vDSRenderEnv_Editor_UpdateShaderCache(IntPtr env, IntPtr material);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr vDSRenderEnv_Editor_BeginPackShaderCache(IntPtr pDevice);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vDSRenderEnv_Editor_EndPackShaderCache(IntPtr pDevice, IntPtr packedNode);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vDSRenderEnv_Editor_PackShaderCache(IntPtr pDevice, IntPtr packedNode, string file);


        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vDSRenderEnv_InvalidateObjects(IntPtr env);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vDSRenderEnv_RestoreObjects(IntPtr env);

#endregion

#region v3dDDSConvert

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr v3dDDSConvert_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dDDSConvert_Delete(IntPtr ddsc);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static int v3dDDSConvert_CreateD3DDevice(IntPtr ddsc, IntPtr hwnd);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public extern static int v3dDDSConvert_ConvertDDS(IntPtr ddsc, string szSrcFile, string szDstFile, bool generateMipmap);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //public unsafe extern static byte* v3dDDSConvert_LoadDDSData(IntPtr d3dDevice, string fileName, int* pitch, UInt32* imgWidth, UInt32* imgHeight, UInt32* textureSize);
        public extern static IntPtr v3dDDSConvert_LoadDDSData(IntPtr d3dDevice, string fileName, IntPtr pitch, IntPtr imgWidth, IntPtr imgHeight, IntPtr textureSize);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public extern static void v3dDDSConvert_DeleteImgData(IntPtr data);


#endregion

#region vfxConvexDecomposition

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vfxConvexDecomposition_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vfxConvexDecomposition_Delete(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vfxConvexDecomposition_SetConvexData(IntPtr ptr, uint depth, double cpercent, double ppercent, uint maxVertices, double skinWidth);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vfxConvexDecomposition_performConvexDecomposition(IntPtr ptr, IntPtr device, IntPtr outMesh, IntPtr mesh, IntPtr material);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vfxConvexDecomposition_performBoxDecomposition(IntPtr ptr, IntPtr device, IntPtr outMesh, IntPtr mesh, IntPtr material);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vfxConvexDecomposition_performSphereDecomposition(IntPtr ptr, IntPtr device, IntPtr outMesh, IntPtr mesh, IntPtr material);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vfxConvexDecomposition_performCylinderDecomposition(IntPtr ptr, IntPtr device, IntPtr outMesh, IntPtr mesh, IntPtr material);


#endregion

#region Decal

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void DecalProxy_SwapPipes(IntPtr decal);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr BoxDecalProxy_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void BoxDecalProxy_Release(IntPtr decal);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void BoxDecalProxy_DSDecalMeshSetMaterial(IntPtr decal, IntPtr mtl);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void BoxDecalProxy_EditorDecalMeshSetMaterial(IntPtr decal, int idx, IntPtr mtl);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void BoxDecalProxy_CommitRangeMesh(IntPtr decal, IntPtr renderEnv, int renderGroup, SlimDX.Matrix* matrix);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void BoxDecalProxy_CommitSignMesh(IntPtr decal, IntPtr renderEnv, int renderGroup, float signMeshSize, bool canHitProxy, SlimDX.Matrix* matrix);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void BoxDecalProxy_SetHitProxy(IntPtr decal, UInt32 idx, UInt32 hitProxy);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int BoxDecalProxy_GetCanHitProxy(IntPtr decal);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void BoxDecalProxy_SetCanHitProxy(IntPtr decal, bool canHitProxy);

#endregion

#region PostProcess

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_GetEnable(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_SetEnable(IntPtr ptr, bool enable);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Release(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_SetLerpValue(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_GetDoLerp(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_SetDoLerp(IntPtr ptr, int value);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr PostProcess_SSAO_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_SSAO_Initialize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void PostProcess_SSAO_SetRandomNormalTexture(IntPtr ptr, string texName);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_SSAO_SetSampleRad(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_SSAO_GetSampleRad(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_SSAO_SetIntensity(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_SSAO_GetIntensity(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_SSAO_SetScale(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_SSAO_GetScale(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_SSAO_SetBias(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_SSAO_GetBias(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_SSAO_SetDoBlur(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_SSAO_GetDoBlur(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_SSAO_GetFXAAQuality(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_SSAO_SetFXAAQuality(IntPtr ptr, int value);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr PostProcess_Bloom_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Bloom_Initialize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Bloom_SetDoBlur(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_Bloom_GetDoBlur(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Bloom_SetBloomImageScale(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_Bloom_GetBloomImageScale(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Bloom_SetBlurType(IntPtr ptr, int value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_Bloom_GetBlurType(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Bloom_SetBlurScale(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_Bloom_GetBlurScale(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Bloom_SetBlurStrength(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_Bloom_GetBlurStrength(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Bloom_SetBlurAmount(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_Bloom_GetBlurAmount(IntPtr ptr);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr PostProcess_ToneMapping_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_Initialize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_ToneMapping_GetA(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetA(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_ToneMapping_GetB(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetB(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_ToneMapping_GetC(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetC(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_ToneMapping_GetD(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetD(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_ToneMapping_GetE(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetE(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_ToneMapping_GetF(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetF(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_ToneMapping_GetW(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetW(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_ToneMapping_GetBrightFactor(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetBrightFactor(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetPreA(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetPreB(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetPreC(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetPreD(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetPreE(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetPreF(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetPreW(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_SetLensEffectsEnable(IntPtr ptr, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_ToneMapping_GetLensEffectsEnable(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ToneMapping_UpdateDownsamplerParams(IntPtr ptr, int count, float rate1, float rate2, float rate3, float strength1, float strength2, float strength3);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr PostProcess_ColorGrading_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ColorGrading_Initialize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr PostProcess_ColorGrading_GetColorGrading(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void PostProcess_ColorGrading_SetColorGrading(IntPtr ptr, string texName);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_ColorGrading_GetEnableFXAA(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ColorGrading_SetEnableFXAA(IntPtr ptr, int value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_ColorGrading_GetFXAAQuality(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ColorGrading_SetFXAAQuality(IntPtr ptr, int value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_ColorGrading_GetEnableColorGrading(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ColorGrading_SetEnableColorGrading(IntPtr ptr, int value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ColorGrading_SetSrcColorGradingTexture(IntPtr ptr, IntPtr pTex);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr PostProcess_Sharpen_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Sharpen_Initialize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_Sharpen_GetHighQualitySharpen(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Sharpen_SetHighQualitySharpen(IntPtr ptr, int value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_Sharpen_GetAverage(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Sharpen_SetAverage(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_Sharpen_GetCoefBlur(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Sharpen_SetCoefBlur(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_Sharpen_GetSharpenEdge(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Sharpen_SetSharpenEdge(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float PostProcess_Sharpen_GetSharpenValue(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_Sharpen_SetSharpenValue(IntPtr ptr, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int PostProcess_ColorGrading_GetSharpenQuality(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void PostProcess_ColorGrading_SetSharpenQuality(IntPtr ptr, int value);

#endregion

#region v3dVertexAssembly

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dVertexAssembly_AddSemantic(IntPtr ptr, byte semhost, byte sem, byte dtype);


#endregion

#region v3dModelCooking

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dModelCooking_CookModelRect(IntPtr device, SlimDX.Matrix* mat, float x, float y, float w, float h, float z, UInt32 dwUsage, int pool);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dModelCooking_CookBox(IntPtr device, SlimDX.Matrix* mat, float x, float y, float z, UInt32 dwUsage, int pool);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dModelCooking_CookSphere(IntPtr device, SlimDX.Matrix* mat, float fRadius, uint uSlices, uint uStacks, UInt32 dwUsage, int pool);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dModelCooking_CookModelTrail(IntPtr device, SlimDX.Matrix* mat, UInt32 dwUsage, int pool);


#endregion

#region v3dModelSource

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dModelSource_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dModelSource_Release(IntPtr ptr);


#endregion

#region v3dStagedObject

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dStagedObject_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_Release(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dStagedObject_CreateModel(IntPtr ptr, IntPtr device, IntPtr modelSource);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dStagedObject_GetDataStream(IntPtr ptr, int idx);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_SetMaterial(IntPtr ptr, uint idx, IntPtr material, IntPtr shadingEnv);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dStagedObject_RenderWithoutTransform(IntPtr ptr, IntPtr device, bool bImmediate, bool bCulling, IntPtr shadingEnv, UInt32 dwDiscardWriteStream);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dStagedObject_GetModelSourceRenderAtomNumber(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_SetHitProxy(IntPtr ptr, UInt32 index, UInt32 hitProxy);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_ModStacks_UpdateTick(IntPtr ptr, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dStagedObject_QueryModifier(IntPtr ptr, UInt64 viid);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dStagedObject_ModelSource_IsCreateFrom(IntPtr ptr, int type);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_ModelSource_SetCreateFrom(IntPtr ptr, int type);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static uint v3dStagedObject_ModelSource_GetVertexNumber(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static uint v3dStagedObject_ModelSource_GetPolyNumber(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_PreUse(IntPtr ptr, bool bForce, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_ModelSource_GetSelfBox(IntPtr ptr, SlimDX.Vector3* vMax, SlimDX.Vector3* vMin);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_ModelSource_GetSelfOBB(IntPtr ptr, SlimDX.Vector3* vMax, SlimDX.Vector3* vMin, SlimDX.Matrix* fixObbMatrix);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt64 v3dStagedObject_GetMaxMaterialNum(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_MaterialTechnique_PreUse(IntPtr ptr, uint idx, bool bForce, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dStagedObject_GetMaterial(IntPtr ptr, uint idx);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dStagedObject_SaveMeshAndBSP(IntPtr mesh, IntPtr bsp, string fileName);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dStagedObject_LineCheck(int bOBBCheck, IntPtr mesh, SlimDX.Vector3* start, SlimDX.Vector3* length, SlimDX.Matrix* matrix, CSUtility.Support.stHitResult* hitResult);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dStageObject_LineIntersect(SlimDX.Matrix* matrix, SlimDX.Vector3* start, SlimDX.Vector3* end, SlimDX.BoundingBox* box);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dStagedObject_GetNearestVertexPos(IntPtr mesh, SlimDX.Vector3* srcPos, SlimDX.Vector3* tagPos, SlimDX.Matrix* matrix, float* fMinLength);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_ModStacks_InsertModifier(IntPtr mesh, IntPtr modifier, UInt64 viid, bool bModifierEffect);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedObject_OnModelSourceReplaced(IntPtr mesh);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dStagedObject_GetReplaceVersion(IntPtr mesh);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dStagedObject_GetModelDesc(IntPtr mesh, CCore.Mesh.MeshDesc* desc);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dStagedObject_SetOnDrawSubsetCallBack(Delegate_FOnDrawSubset cb);

#endregion

#region v3dDataStream

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dDataStream_GetFrameBuffer(IntPtr ptr, uint idx);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDataStream_Flush2VB(IntPtr ptr, IntPtr device);


#endregion

#region v3dUIRender

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dUIRender_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dUIRender_Release(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dUIRender_DrawImage(IntPtr mgr,int zOrder, IntPtr ptr, IntPtr pDiffuseTex, SlimDX.Vector4* backColor, float opacity, int* rectDatas, float* srcOriData, float* floatData, SlimDX.Matrix* transMat, IntPtr material);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dUIRender_FillRectangle(IntPtr mgr, int zOrder, IntPtr ptr, SlimDX.Vector4* backColor, int left, int top, int width, int height, float screenWidth, float screenHeight, SlimDX.Matrix* transMat, IntPtr material);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dUIRender_DrawLine(IntPtr mgr, int zOrder, IntPtr ptr, int* intData, UInt32 clr, float scaleX, float scaleY);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dUIRender_DrawWidthLine(IntPtr mgr, int zOrder, IntPtr ptr, int* intData, Int32 width, UInt32 clr, float scaleX, float scaleY);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dUIRender_DrawLineWithMaterial(IntPtr mgr, int zOrder, IntPtr ptr, int* intData, Int32 width, float* srcOriData, float* floatData, SlimDX.Matrix* transMat, IntPtr material);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dUIRender_DrawString(IntPtr mgr, int zOrder, IntPtr ptr, int x, int y, string fontName, int fontSize, string text, UInt32 clr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dUIRender_DrawStringWithParams(IntPtr mgr, int zOrder, IntPtr ptr, int x, int y, IntPtr paramVector);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dUIRender_DeleteStrings(void** str, int strVectorSize);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void** v3dUIRender_MeasureStringInWidth(IntPtr ptr, string fontName, int fontSize, string text, int textLength, IntPtr paramVector, int limitWidth, CSUtility.Support.Size* oTextSize, int* oneLineHeight, int* maxTopLine, int* maxBottomLine, int* realSize, int* strVectorCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dUIRender_MeasureStringInLine(IntPtr ptr, string fontName, int fontSize, string text, int textLength, IntPtr paramVector, CSUtility.Support.Size* oTextSize, int* maxTopLine, int* maxBottomLine, int* realSize);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void** v3dUIRender_SplitTextInWidth(IntPtr ptr, string fontName, int fontSize, string text, int textLength, IntPtr paramVector, int limitWidth, int* strVectorCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void** v3dUIRender_SplitTextInHalf(IntPtr ptr, string fontName, int fontSize, string text, int textLength, IntPtr paramVector, int limitWidth, int* strVectorCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dUIRender_PointCheck(IntPtr ptr, string fontName, int fontSize, string text, int textLength, IntPtr paramVector, int x, int y, int* outX, int* outY, int* outPos);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dUIRender_MeasureTextToPos(IntPtr ptr, string fontName, int fontSize, string text, int textLength, IntPtr paramVector, int pos, int* outWidth);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dUIRender_SetClipRect(IntPtr mgr, IntPtr ptr, SlimDX.Vector4* rect);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dUIRender_CommitDrawCall(IntPtr ptr, IntPtr mgr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dUIRender_SwapQueue(IntPtr ptr, IntPtr mgr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dUIRender_ClearAllCommit(IntPtr mgr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr UIDrawCallManager_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void UIDrawCallManager_Delete(IntPtr mgr);

#endregion


#region Font
        // IFont
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DFontW_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DFontW_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontW_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontW_InitObjects(IntPtr p, IntPtr pDevice, IntPtr hFont, ulong dwFlags, int nCatchSzie);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DFontW_GetFont(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DFontW_CreateDefaultFont();


        // FontRenderParam
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DFontRenderParam_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DFontRenderParam_Clone(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParam_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParam_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParam_SetBLColor(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DFontRenderParam_GetBLColor(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParam_SetBRColor(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DFontRenderParam_GetBRColor(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParam_SetTLColor(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DFontRenderParam_GetTLColor(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParam_SetTRColor(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DFontRenderParam_GetTRColor(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParam_SetOpacity(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DFontRenderParam_GetOpacity(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParam_SetOutlineType(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DFontRenderParam_GetOutlineType(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParam_SetOutlineThickness(IntPtr ptr, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DFontRenderParam_GetOutlineThickness(IntPtr ptr);



        // FontRenderParamList
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DFontRenderParamList_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DFontRenderParamList_New2();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParamList_New3();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParamList_Release(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParamList_AddRef(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParamList_Clear(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DFontRenderParamList_GetSize(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DFontRenderParamList_GetParam(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParamList_PushBack(IntPtr ptr, IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParamList_Erase(IntPtr ptr, int p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DFontRenderParamList_SetFontAttribs(IntPtr ptr, string fontName, int fontSize, string text);
        
#endregion

        //#region FTFontManager

        //        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //        public unsafe extern static void FTFontManager_DrawText2D(IntPtr mgr, string fontName, int fontSize, string text, int penX, int penY, UInt32 color);
        //        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //        public unsafe extern static void FTFontManager_DrawText2DWithParams(IntPtr mgr, string fontName, int fontSize, string text, int penX, int penY, IntPtr paramVector);

#region SimpleSpline
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DSimpleSpline_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DSimpleSpline_Delete(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DSimpleSpline_Clear(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int V3DSimpleSpline_GetPointCount(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSimpleSpline_AddPoint(IntPtr ptr, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSimpleSpline_GetTangent(IntPtr ptr, int index, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSimpleSpline_RecalcTangents(IntPtr p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DSimpleSpline_Interpolate(IntPtr ptr, float t, SlimDX.Vector3* p);

#endregion
        //#endregion

#region v3dDevice
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_StartIOThread();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_EndIOThread();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dDevice_GetFontManager(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetCamera(IntPtr device, IntPtr pCamera);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetPlayerPos(IntPtr device, SlimDX.Vector3* p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetPlayerRadius(IntPtr device, float p);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_RenderDevice_Clear(IntPtr device, uint index, UInt32 flag, UInt32 color, float depth, UInt32 stencil);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetViewPort(IntPtr device, IntPtr pView);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetViewPortPtr(IntPtr device, IntPtr pVP);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetSceneCapture1(IntPtr device, IntPtr pTexture);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetSceneCapture2(IntPtr device, IntPtr pTexture);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetSceneCapture3(IntPtr device, IntPtr pTexture);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetSceneCapture4(IntPtr device, IntPtr pTexture);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetSceneCapture5(IntPtr device, IntPtr pTexture);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_BeginDraw(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_EndDraw(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dDevice_GetGammaCorrect(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_SetGammaCorrect(IntPtr device, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dDevice_GetUseSRGBSpace(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_SetUseSRGBSpace(IntPtr device, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dDevice_RenderDevice_GetDrawTriangleCount(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dDevice_RenderDevice_GetDPCount(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dDevice_RenderDevice_GetClearCount(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dDevice_GetCamera(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dDevice_GetD3DDevice(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_AddCustomShaderTime(IntPtr device, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dDevice_CreateRenderView(IntPtr device, IntPtr win, UInt32 w, UInt32 h, CCore.BufferFormat format, CCore.BufferFormat dsFormat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dDevice_VMObjMgr_LoadModelSource(IntPtr device, string fileNameOri, UInt32 dwUsage, int pool, bool forceLoad);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dDevice_VMObjMgr_ForceReloadModelSource(IntPtr device, string fileName);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dDevice_RAMObjMgr_LoadSubAction(IntPtr device, string fileNameOri);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dDevice_RAMObjMgr_LoadBSPSource(IntPtr device, string fileNameOri, UInt32 dwUsage, int pool, bool forceLoad);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dDevice_RAMObjMgr_LoadSocketTable(IntPtr device, string fileName);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_TextureMgr_ForceReloadTexture(IntPtr device, string fileName);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dDevice_TextureMgr_LoadTexture(IntPtr device, string fileName);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dDevice_TextureMgr_CreateTexture(IntPtr device, uint width, uint height, int format, uint mipLevels, UInt32 usage, int pool);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dDevice_SetSunLightShadowMapRenderMaterial(IntPtr device, IntPtr pM);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dDevice_SetHeadLightShadowMapRenderMaterial(IntPtr device, IntPtr pM);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dDevice_IsDeviceLost(IntPtr device, int viewPresentResult);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_BeforeDeviceReset(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dDevice_TryDeviceReset(IntPtr device, IntPtr hWindow);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_InvalidateObjects(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_RestoreObjects(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_ResetRAMObjectManager(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_ResetVMObjectManager(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_ResetTextureManager(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_SetIsEditorMode(IntPtr device, bool value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dDevice_GetDisplayModeCount(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_GetDisplayMode(IntPtr device, int index, int* w, int* h);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dDevice_GetCurrMaterialFilter(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_SetCurrMaterialFilter(IntPtr device, int materialFilter);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dDevice_GetCurrMaterialLOD(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_SetCurrMaterialLOD(IntPtr device, int materialLOD);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetDefaultFontName(IntPtr device, string fontName);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetDefaultShadowSmoothTextureName(IntPtr device, string name);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dDevice_SetDefaultEmptyTextureName(IntPtr device, string name);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dDevice_GetDebugTextureCount(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dDevice_GetDebugTexturePtr(IntPtr device, int index);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dDevice_GetDebugTextureName(IntPtr device, int index);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dDevice_GetDebugTextureWidth(IntPtr device, int index);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dDevice_GetDebugTextureHeight(IntPtr device, int index);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dDevice_GetDebugTextureGray(IntPtr device, int index);

#endregion

#region v3dCamera

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dCamera_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_Release(IntPtr eye);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_MakePerspective(IntPtr eye, float fov, float width, float height, float zNear, float zFar);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_SetPosLookAtUp(IntPtr eye, SlimDX.Vector3* pPos, SlimDX.Vector3* pLook, SlimDX.Vector3* pUp);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_MoveByCamera(IntPtr eye, int axisType, float value, bool bUpdate);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_MoveByAxis(IntPtr eye, SlimDX.Vector3* axis, bool bUpdate);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_SetPosDir(IntPtr eye, SlimDX.Vector3* pos, SlimDX.Vector3* dir);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_RotateCameraAtByCamera(IntPtr eye, int axis, float value, bool bUpdate);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_SetPosDirUp(IntPtr eye, SlimDX.Vector3* pos, SlimDX.Vector3* dir, SlimDX.Vector3* up);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_GetPosition(IntPtr eye, SlimDX.Vector3* pos);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_GetDirVec(IntPtr eye, SlimDX.Vector3* dir);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_GetUpVec(IntPtr eye, SlimDX.Vector3* up);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static float v3dCamera_GetViewLength(IntPtr eye);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_SetPosByLength(IntPtr eye, float length);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_RotateLookAtByCamera(IntPtr eye, int axisType, float step, bool bUpdate);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dCamera_RotateLookAtByAxis(IntPtr eye, int axisType, float step, bool bUpdate);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_GetViewTM(IntPtr eye, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_GetProjTM(IntPtr eye, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_GetBillboardTM(IntPtr eye, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dCamera_GetFOV(IntPtr eye);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dCamera_GetViewPort(IntPtr eye);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dCamera_GetViewPortWidth(IntPtr eye);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dCamera_GetViewPortHeight(IntPtr eye);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dCamera_GetNear(IntPtr eye);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dCamera_GetFar(IntPtr eye);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_GetRightVec(IntPtr eye, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_GetPickRay(IntPtr eye, SlimDX.Vector3* vec, int x, int y);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_MoveByCameraAxis(IntPtr eye, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dCamera_GetScreenSizeInWorld(IntPtr eye, SlimDX.Vector3* worldPos, float screenSize);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dCamera_GetWorldSizeInScreen(IntPtr eye, SlimDX.Vector3* worldPos, float worldSize);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dCamera_Trans2Screen(IntPtr eye, SlimDX.Vector3* vSrc, SlimDX.Vector3* vPos);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_MakeOrtho(IntPtr eye, float w, float h, float pvw, float pvh, float fNear, float fFar);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_MakeOrthoAutoAspect(IntPtr eye, float w, float pvw, float pvh, float fNear, float fFar);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dCamera_IsFrustumContainVector(IntPtr eye, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dCamera_IsFrustumContainBox(IntPtr eye, SlimDX.BoundingBox* box);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dCamera_IsFrustumContainOBB(IntPtr eye, SlimDX.BoundingBox* box, SlimDX.Matrix* pTM);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_CopyData(IntPtr eye, IntPtr srcCamera);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_GetLocalRightVec(IntPtr eye, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_GetLocalUpVec(IntPtr eye, SlimDX.Vector3* vec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dCamera_DrawCameraFrustum(IntPtr eye);


#endregion

#region v3dGraphics

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ResourceReplacer_ReplaceAllResources();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGraphics_SetFreeGCHandleEvent(Delegate_FreeGCHandle fun);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dGraphics_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGraphics_Delete(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dGraphics_Initialize(IntPtr ptr, int deviceType, string fvfPath, string effectPath, string binaryEffectPath, string defaultResourcePath, UInt32 adapter, void* deviceWindow, bool windowed, bool immediate, Int64 frameMillionSecondTime,
                                                               CCore.Graphics.Delegate_RealMaterialLoaderLoadMaterial RlMEvent,
                                                               CCore.Graphics.Delegate_RealMaterialLoaderLoadTechnique RlTEvent,
                                                               CCore.Graphics.Delegate_RealMaterialLoaderLoadMaterialWithGuid RlMWGEvent,
                                                               CCore.Graphics.Delegate_MeshResFactoryCreateRes MRC,
                                                               CCore.Graphics.Delegate_TextureResFactoryCreateRes TRC,
                                                               CCore.Graphics.Delegate_XndResFactoryCreateRes XRC);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dGraphics_InitShadingEnv();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dGraphics_InitializeTextureResDownloadEvent(IntPtr ptr,
                                                               CCore.Graphics.Delegate_TextureResIsDownloadingEvent trid,
                                                               CCore.Graphics.Delegate_TextureResGetDefaultResource trgd,
                                                               CCore.Graphics.Delegate_TextureResRegWaitDownloadResource trrwd);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dGraphics_TextureRes_OnDownloadFinished(IntPtr ptr, string pszFile);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dGraphics_InitializeMeshResDownloadEvent(IntPtr ptr,
                                                               CCore.Graphics.Delegate_MeshResIsDownloadingEvent rid,
                                                               CCore.Graphics.Delegate_MeshResGetDefaultResource rgd,
                                                               CCore.Graphics.Delegate_MeshResRegWaitDownloadResource rrwd);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dGraphics_MeshRes_OnDownloadFinished(IntPtr ptr, string pszFile);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dGraphics_InitializeXndResDownloadEvent(IntPtr ptr,
                                                               CCore.Graphics.Delegate_XndResIsDownloadingEvent rid,
                                                               CCore.Graphics.Delegate_XndResGetDefaultResource rgd,
                                                               CCore.Graphics.Delegate_XndResRegWaitDownloadResource rrwd);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dGraphics_XndRes_OnDownloadFinished(IntPtr ptr, string pszFile);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGraphics_Cleanup(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGraphics_CopyFrom(IntPtr ptr, IntPtr pTexture, SlimDX.Rect *pRect);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dGraphics_GetDevice(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGraphics_DumpAllShader(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGraphics_SetResourceForcePreUseCount(int count);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dGraphics_GetResourceForcePreUseCount();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dGraphics_GetResourceAsyncCount();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dGraphics_GetResourceCountByType(IntPtr ptr, int resType);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGraphics_HelloTriangle(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dGraphics_SetOnGLError(Delegate_FOnGLError cb);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGraphics_SetViewPort(IntPtr ptr, int x, int y, int w, int h, float minz, float maxz);

#endregion

#region Material

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedMaterialBase_AddRef(IntPtr mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedMaterialBase_Release(IntPtr mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dStagedMaterialBase_GetMaterialTag(IntPtr mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dStagedMaterialBase_GetMain(IntPtr mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedMaterialBase_DeleteStrings(void** str, int strCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static UInt64 v3dStagedMaterialBase_AssignUniqueIDWithString(string strKey);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static UInt64 v3dStagedMaterialBase_AssignUniqueID(IntPtr mat, string strKey);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static UInt64 v3dStagedMaterialBase_GetUniqueID(IntPtr mat);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void** vStandMaterial_GetTechNames(IntPtr str, int* strCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vStandMaterial_GetTechnique(IntPtr str, string strName);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vStandMaterial_SetTechnique(IntPtr mat, IntPtr tech);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vStandMaterial_GetDefaultTechnique(IntPtr mat);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedMaterialInstance_AddRef(IntPtr mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedMaterialInstance_Release(IntPtr mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedMaterialInstance_SetMaterial(IntPtr mat, IntPtr m, IntPtr t);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetInt(IntPtr mat, string name, int v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetFloat(IntPtr mat, string name, float v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetFloat2(IntPtr mat, string name, SlimDX.Vector2* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetFloat3(IntPtr mat, string name, SlimDX.Vector3* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetFloat4(IntPtr mat, string name, SlimDX.Vector4* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetFloat4x4(IntPtr mat, string name, SlimDX.Matrix* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetTexture(IntPtr mat, string name, IntPtr v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetFloatByShaderVar(IntPtr var, float v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetFloat2ByShaderVar(IntPtr var, SlimDX.Vector2* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetFloat3ByShaderVar(IntPtr var, SlimDX.Vector3* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetFloat4ByShaderVar(IntPtr var, SlimDX.Vector4* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetFloat4x4ByShaderVar(IntPtr var, SlimDX.Matrix* v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_SetTextureByShaderVar(IntPtr var, IntPtr v);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dStagedMaterialInstance_ForceGetShaderVarIndex(IntPtr mat, string name);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dStagedMaterialInstance_SetTextureByIndex(IntPtr mat, int index, IntPtr v);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedMaterialInstance_PreUse(IntPtr mat, bool bForce, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static uint v3dStagedMaterialInstance_GetVer(IntPtr mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dStagedMaterialInstance_GetTechnique(IntPtr mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static uint v3dStagedMaterialInstance_GetTechniqueVer(IntPtr mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dStagedMaterialInstance_LoadRefTexture(IntPtr tech);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vStandMaterialMgr_Get(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vStandMaterialMgr_RemoveMaterial(IntPtr mgr, string str);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vStandMaterialMgr_LoadMaterial(IntPtr mgr, string mtlFile, IntPtr r2m, bool bForceLoad);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vStandMaterialMgr_LoadTechnique(IntPtr mgr, string techFile, IntPtr r2m, bool bForceLoad);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vStandMaterialMgr_RemoveTechnique(IntPtr mgr, string techFile);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vStandMaterialMgr_FindTechnique(IntPtr mgr, string techFile);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vStandMaterialMgr_FindMaterial(IntPtr mgr, string mtlFile);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vStandMaterialMgr_RefreshEffect(IntPtr device, IntPtr material);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vStandMaterialMgr_RefreshAllEffect(IntPtr mgr, IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void** vStandMaterialMgr_GetShaderAutoDatas(IntPtr device, int* strCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vStandMaterialMgr_DeleteShaderAutoDatasString(void** str, int strCount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vStandMaterialMgr_InitEdgeDetectParams(IntPtr edgeMtl, IntPtr originMtl);

#endregion

#region v3dTechnique

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dTechnique_Release(IntPtr tech);

#endregion

#region IEngine

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int IEngine_PeekMessage(IntPtr msg, IntPtr hwnd, UInt32 msgFilterMin, UInt32 msgFilterMax, UInt32 removeMsg);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IEngine_Sleep(UInt64 milliSecond);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IEngine_SetEngineTick(Int64 time);

#endregion


#region v3dSampMgr

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vfxMemory_DumpMemoryState(string str);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vfxMemory_CheckMemoryState(string str);


#endregion

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLoadPipe_Pause();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLoadPipe_Resume();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLoadPipe_SetPreUseForceMode(int force);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vLoadPipe_GetAsyncLoadNumber();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLoadPipe_SetOutputLoadInfo(int bOutput);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vLoadPipe_SetAsyncLoadObjectCallBack(Delegate_FOnAsyncLoadObject cb);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vFreePipe_OnFreeTick();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dDevice_TextureManager_TryFreeResource(IntPtr device, UInt32 limitSize, UInt32 livetime, UInt32 reduceSize);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dDevice_VMObjectManager_TryFreeResource(IntPtr device, UInt32 limitSize, UInt32 livetime, UInt32 reduceSize);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dDevice_RAMObjectManager_TryFreeResource(IntPtr device, UInt32 limitSize, UInt32 livetime, UInt32 reduceSize);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dDevice_TextureManager_GetTotalSize(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dDevice_VMObjectManager_GetTotalSize(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dDevice_RAMObjectManager_GetTotalSize(IntPtr device);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dDevice_SetDefaultRS(IntPtr device, int mode);

#region View

        // ViewPort
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr V3DVIEWPORT9_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DVIEWPORT9_Delete(IntPtr viewPort);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DVIEWPORT9_SetX(IntPtr viewPort, UInt32 x);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 V3DVIEWPORT9_GetX(IntPtr viewPort);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DVIEWPORT9_SetY(IntPtr viewPort, UInt32 y);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 V3DVIEWPORT9_GetY(IntPtr viewPort);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DVIEWPORT9_SetWidth(IntPtr viewPort, UInt32 width);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 V3DVIEWPORT9_GetWidth(IntPtr viewPort);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DVIEWPORT9_SetHeight(IntPtr viewPort, UInt32 height);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 V3DVIEWPORT9_GetHeight(IntPtr viewPort);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DVIEWPORT9_SetMinZ(IntPtr viewPort, float minZ);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DVIEWPORT9_GetMinZ(IntPtr viewPort);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void V3DVIEWPORT9_SetMaxZ(IntPtr viewPort, float maxZ);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float V3DVIEWPORT9_GetMaxZ(IntPtr viewPort);

        // RenderView
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IRenderView_Release(IntPtr view);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IRenderView_SetViewPortX(IntPtr view, UInt32 value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 IRenderView_GetViewPortX(IntPtr view);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IRenderView_SetViewPortY(IntPtr view, UInt32 value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 IRenderView_GetViewPortY(IntPtr view);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 IRenderView_GetViewPortWidth(IntPtr view);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IRenderView_SetViewPortWidth(IntPtr view, UInt32 value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 IRenderView_GetViewPortHeight(IntPtr view);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IRenderView_SetViewPortHeight(IntPtr view, UInt32 value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IRenderView_Resize(IntPtr view, UInt32 w, UInt32 h);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IRenderView_SetViewPort(IntPtr view, IntPtr viewPort);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IRenderView_Begin(IntPtr view);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IRenderView_End(IntPtr view);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static Int64 IRenderView_GetPresent(IntPtr view);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IRenderView_Clear(IntPtr view, UInt32 flag, UInt32 color, float fDepth, UInt32 stencil);


#endregion

#region TileScene

        // TileObject
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vTileObject_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vTileObject_AddRef(IntPtr tileObject);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileObject_Release(IntPtr tileObject);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vTileObject_GetRefCount(IntPtr tileObject);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vTileObject_IsOutofCurrentLevels(IntPtr tileObj);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileObject_InitializeEvent(IntPtr tileObject,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_Cleanup cuEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_PreUse puEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_GetAABB gAABBEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_GetLocation glEvnet,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_GetOriginLocation glEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_LineCheck lcEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_HasFlag hfEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_SaveActor saEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_LoadActor laEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_GetTypeName gtnEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_ReleaseTypeName rtnEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_GetID giEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_GetGameType ggtEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_GetSceneFlag gsfEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_RestoreObjects roEvent,
                                                                     CCore.Scene.TileScene.Delegate_TileObject_InvalidateObjects ioEvent);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileObject_SetTileScene(IntPtr tileObject, IntPtr scene);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vTileObject_GetTileScene(IntPtr tileObj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileObject_RemoveFromAllReferPatches(IntPtr tileObj);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileObject_SetActor(IntPtr tileObject, IntPtr actor);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileObject_SetCreateTileObjectEvent(IntPtr scene, CCore.Scene.TileScene.Delegate_TileObject_CreateTileObject createEvent);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileObject_SetTileObjectLoadFinishEvent(IntPtr scene, CCore.Scene.TileScene.Delegate_TileObject_LoadFinish addEvent);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileObject_GetLocation(IntPtr tileObject, SlimDX.Vector3* outVec);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vTileObject_GetCSActor(IntPtr tileObject);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileObject_Tick(IntPtr tileObject);

        // TilePatch
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vTilePatch_TourTileAllObjects(IntPtr tilePatch, CCore.Scene.TileScene.TileScene.Delegate_OnVisitTileObject visit, UInt16 actorType, uint serialId, IntPtr arg);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vTilePatch_IsDeleted(IntPtr tilePatch);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTilePatch_DrawInheritBoundingBox(IntPtr tilePatch, IntPtr device);

        // TileLevel
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vTileLevel_GetPatch(IntPtr level, uint u, uint v);

        // TileScene
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vTileScene_New(IntPtr handle);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileScene_AddRef(IntPtr scene);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTileScene_Release(IntPtr scene);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTileScene_ConstructTileScene(IntPtr scene, string name, string path, CCore.Scene.TileScene.vTileSceneInfo* sceneInfo);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_InitializeEvent(IntPtr scene, CCore.Scene.TileScene.Delegate_GetGameTypePathName ggtpEvent);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTileScene_LoadTileScene(IntPtr scene, string name, string path, string mapPath);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTileScene_LoadServerTileScene(IntPtr scene, string name, string serverPath);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_SaveDirtyLevel(IntPtr scene, string name, string path, bool bForceSave);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_SaveSpecialDirtyLevel(IntPtr scene, string path, UInt16 actorType, bool bForceSave);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_SetShowServerObject(IntPtr scene, UInt16 actorType, bool bShow);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_NewLevel(IntPtr scene, UInt16 iCol, UInt16 iRow);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTileScene_TravelTo(IntPtr scene, float x, float z, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_GetTileInfo(IntPtr scene, CCore.Scene.TileScene.vTileSceneInfo* info);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vTileScene_GetLevel(IntPtr scene, UInt16 iCol, UInt16 iRow);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTileScene_LineCheck(IntPtr scene, SlimDX.Vector3* start, SlimDX.Vector3* end, CSUtility.Support.stHitResult* hitResut, bool bWithDeletePatch, IntPtr param);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_Tick(IntPtr scene, Int64 elapsedMillisecond, Int64 millisecond);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_KickOffCache(IntPtr scene, Int64 nowTime, uint lifeTime);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //public unsafe extern static int vTileScene_AddActor(IntPtr scene, IntPtr tileObj, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static UInt16 vTileScene_GetLevelIndexX(IntPtr scene, float x);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static UInt16 vTileScene_GetLevelIndexZ(IntPtr scene, float z);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vTileScene_FindActor(IntPtr scene, Guid* id, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_TourActors(IntPtr scene, CCore.Scene.TileScene.TileScene.Delegate_OnVisitTileObject_GetAllActors visit, Int64 millisecondTime, UInt16 actorType, uint serialId, IntPtr arg);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_TourActorsWithRange(IntPtr scene, CCore.Scene.TileScene.TileScene.Delegate_OnVisitTileObject_GetAllActors visit, SlimDX.Vector3* vStart, SlimDX.Vector3* vEnd, Int64 millisecondTime, UInt16 actorType, uint serialId, IntPtr arg);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_CheckVisible(IntPtr scene, CCore.Scene.TileScene.TileScene.Delegate_TileVisibleVisitor_Visit visit, IntPtr visitorArg, IntPtr pCamera, Int64 millisecondTime, int stopAtPatch, int ignoreChild);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_CheckVisibleSquare(IntPtr scene, CCore.Scene.TileScene.TileScene.Delegate_TileVisibleVisitor_Visit visit, IntPtr visitorArg, SlimDX.Vector3* start, SlimDX.Vector3* end, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vTileScene_GetRealLevel(IntPtr scene, UInt16 lvlX, UInt16 lvlZ, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTileScene_AddPatch(IntPtr scene, float x, float z, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTileScene_DelPatch(IntPtr scene, float x, float z, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTileScene_AddLevel(IntPtr scene, uint idu, uint idv, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTileScene_DelLevel(IntPtr scene, uint idu, uint idv, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vTileScene_GetPatch(IntPtr scene, float x, float z, Int64 millisecondTime, bool preUse);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTileScene_SetNeighborSide(IntPtr scene, uint value, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTileScene_SetLevelDirty(IntPtr scene, IntPtr actor, Int64 frameMillisecondTime, UInt16* minX, UInt16* maxX, UInt16* minZ, UInt16* maxZ);

#endregion

#region v3dBspSpace

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dBspSpace_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dBspSpace_Release(IntPtr bsp);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dBspSpace_AddRef(IntPtr bsp);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dBspSpace_Split(IntPtr bsp, IntPtr mesh, float epsilon);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dBspSpace_QueryRayIntersect(IntPtr bsp, SlimDX.Vector3* start, SlimDX.Vector3* end, SlimDX.Matrix* matrix, SlimDX.Matrix* invMatrix, CSUtility.Support.stHitResult* hitResult);
        //[DllImport("Victory3D_Win32_ReleaseU.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "v3dBspSpace_QueryRayIntersect")]
        //public unsafe extern static int v3dBspSpace_QueryRayIntersect_Release(IntPtr bsp, SlimDX.Vector3* start, SlimDX.Vector3* end, SlimDX.Matrix* matrix, SlimDX.Matrix* invMatrix, SlimDX.Vector3* hitPosition, SlimDX.Vector3* hitNormal);
        //[DllImport("Victory3D_Win32_DebugU.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "v3dBspSpace_QueryRayIntersect")]
        //public unsafe extern static int v3dBspSpace_QueryRayIntersect_Debug(IntPtr bsp, SlimDX.Vector3* start, SlimDX.Vector3* end, SlimDX.Matrix* matrix, SlimDX.Matrix* invMatrix, SlimDX.Vector3* hitPosition, SlimDX.Vector3* hitNormal);
        //public static bool RayIntersectDebug = true;
        //public unsafe static int v3dBspSpace_QueryRayIntersect(IntPtr bsp, SlimDX.Vector3* start, SlimDX.Vector3* end, SlimDX.Matrix* matrix, SlimDX.Matrix* invMatrix, SlimDX.Vector3* hitPosition, SlimDX.Vector3* hitNormal)
        //{
        //    unsafe
        //    {
        //        if (RayIntersectDebug)
        //            return v3dBspSpace_QueryRayIntersect_Debug(bsp, start, end, matrix, invMatrix, hitPosition, hitNormal);
        //        else
        //            return v3dBspSpace_QueryRayIntersect_Release(bsp, start, end, matrix, invMatrix, hitPosition, hitNormal);
        //    }
        //}

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dBspSpace_Save(IntPtr bsp, IntPtr xndNode);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dBspSpace_Load(IntPtr bsp, IntPtr xndNode);


#endregion

#region SkinModifier

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dSkinModifier_Update(IntPtr modifier, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dSkinModifier_SetFullSkeleton(IntPtr modifier, IntPtr skeleton);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dSkinModifier_GetSubSkeleton(IntPtr modifier);


#endregion

#region ServerAltitudeRenderEnv

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ServerAltitudeRenderEnv_Delete(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ServerAltitudeRenderEnv_New(IntPtr device);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ServerAltitudeRenderEnv_SetTexSize(IntPtr env, UInt32 w, UInt32 h, float fXPerPixel, float fZPerPixel);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ServerAltitudeRenderEnv_SetCameraPos(IntPtr env, SlimDX.Vector3* eyePos);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ServerAltitudeRenderEnv_ClearAllDrawingCommits(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ServerAltitudeRenderEnv_CommitMesh(IntPtr env, Int64 time, IntPtr mesh, SlimDX.Matrix* matrix);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ServerAltitudeRenderEnv_CommitTerrain(IntPtr env, Int64 time, IntPtr terrainPatch, SlimDX.Matrix* matrix);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ServerAltitudeRenderEnv_Draw(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr ServerAltitudeRenderEnv_CreatePixelBuffer(IntPtr env);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void IG32R32FPixelBuffer_Release(IntPtr buffer);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float* IG32R32FPixelBuffer_GetPixel(IntPtr buffer, int x, int y);


#endregion

#region vTerrain

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vTerrain_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTerrain_Release(IntPtr terrain);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_ConstructTerrain(IntPtr terrain, IntPtr device, string name, string path, CCore.Terrain.TerrainInfo* terInfo);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_LoadTerrain(IntPtr terrain, IntPtr device, string name, string path);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_SaveDirtyLevel(IntPtr terrain, string name, string path, bool forceSave);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vTerrain_NewLevel(IntPtr terrain, UInt32 iCol, UInt32 iRow);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_TravelTo(IntPtr terrain, float x, float z, Int64 milliSecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_GetHeight(IntPtr terrain, UInt32 x, UInt32 z, short* alt, Int64 milliSecondTime, bool preUse);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_GetHeightF(IntPtr terrain, float x, float z, float* alt, Int64 milliSecondTime, bool preUse);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_SetHeight(IntPtr terrain, uint idu, uint idv, short alt, Int64 milliSecondTime, bool preUse);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static CCore.Terrain.TerrainInfo* vTerrain_GetTerrainInfo(IntPtr terrain);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_GetPatchLocation(IntPtr terrain, uint idu, uint idv, out float outX, out float outY, out float outZ);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vTerrain_GetLevel(IntPtr terrain, UInt16 iCol, UInt16 iRow);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_LineCheck(IntPtr terrain, SlimDX.Vector3* start, SlimDX.Vector3* end, CSUtility.Support.stHitResult* result, bool withDeletePatch);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_KickOffCache(IntPtr terrain, Int64 nowMilliSecondTime, uint lifeMilliSecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_SetBaseLayerMaterial(IntPtr terrain, Guid* matId, Int64 milliSecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_GetBaseLayerMaterial(IntPtr terrain, Guid* matId);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_PaintLayerData(IntPtr terrain, IntPtr mtl, IntPtr grassData, int value, uint idu, uint idv, Int64 milliSecondTime, bool preUse);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static Guid* vTerrain_GetLayerMaterials_Alloc(IntPtr terrain, int* count);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_GetLayerMaterials_Free(Guid* guidArray);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void** vTerrain_GetGrassData_Alloc(IntPtr terrain, int* count, Int64 milliSecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_GetGrassData_Free(void** grassData);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void** vTerrain_GetRemarks_Alloc(IntPtr terrain, int* count);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_GetRemarks_Free(void** remarks, int count);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_AddLayerMaterial(IntPtr terrain, Guid* id);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_RemoveLayerMaterial(IntPtr terrain, Guid* id, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_ResetLayerMaterial(IntPtr terrain, Guid* oldMatId, IntPtr mtl, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vTerrain_ResetLayerGrass(IntPtr terrain, Guid* matId, IntPtr grassData, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_ResetLayerRemarks(IntPtr terrain, Guid* matId, string remark);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_PickMaterial(IntPtr terrain, Guid* outMatId, uint idu, uint idv, Int64 millisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_RefreshEffect(IntPtr terrain, IntPtr device, IntPtr mtl);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_AddPatch(IntPtr terrain, uint idu, uint idv, Int64 milliSecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_DelPatch(IntPtr terrain, uint idu, uint idv, Int64 milliSecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_AddLevel(IntPtr terrain, uint idu, uint idv, Int64 milliSecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrain_DelLevel(IntPtr terrain, uint idu, uint idv, Int64 milliSecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_CheckVisible(IntPtr terrain, CCore.Terrain.Terrain.Delegate_TerrainVisibleVisitor_Visit visitorEvent, IntPtr visitorArg, IntPtr pCamera, Int64 millisecondTime, bool stopAtPatch, bool ignoreChild);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vTerrain_GetRealLevel(IntPtr terrain, uint lvlX, uint lvlZ, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void vTerrain_SetNeighborSide(IntPtr terrain, uint value, Int64 time);

        // vTerrainPatch
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vTerrainPatch_IsDeleted(IntPtr patch);

        // vTerrainLevel
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr vTerrainLevel_GetPatch(IntPtr level, uint u, uint v, Int64 time);


#endregion

#region v3dGroupGridObject

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dGroupGridObject_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_Release(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetXLength(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetZLength(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetDeltaX(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetDeltaZ(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetLocX(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetLocY(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetLocZ(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetInterval(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetlineColor(IntPtr obj, UInt32 value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetGroupColor(IntPtr obj, UInt32 value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGroupGridObject_SetRelativeMatrix(IntPtr obj, SlimDX.Matrix* value);


#endregion

#region v3dGridObject

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dGridObject_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGridObject_Release(IntPtr grid);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dGridObject_GetCol(IntPtr grid);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGridObject_SetCol(IntPtr grid, int value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int v3dGridObject_GetRow(IntPtr grid);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGridObject_SetRow(IntPtr grid, int value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dGridObject_GetDeltaX(IntPtr grid);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGridObject_SetDeltaX(IntPtr grid, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dGridObject_GetDeltaZ(IntPtr grid);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGridObject_SetDeltaZ(IntPtr grid, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dGridObject_GetHeight(IntPtr grid);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGridObject_SetHeight(IntPtr grid, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dGridObject_GetColor(IntPtr grid);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGridObject_SetColor(IntPtr grid, UInt32 value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dGridObject_GetColorAxis(IntPtr grid);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGridObject_SetColorAxis(IntPtr grid, UInt32 value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGridObject_GetRelativeMatrix(IntPtr grid, SlimDX.Matrix* mat);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dGridObject_SetRelativeMatrix(IntPtr grid, SlimDX.Matrix* mat);


#endregion

#region v3dText2DObject

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dText2DObject_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dText2DObject_Release(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dText2DObject_SetParams(IntPtr obj, IntPtr param);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dText2DObject_Params_AddRef(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dText2DObject_GetText(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dText2DObject_SetText(IntPtr obj, string text);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static IntPtr v3dText2DObject_GetFontName(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dText2DObject_SetFontName(IntPtr obj, string fontName);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int v3dText2DObject_GetFontSize(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dText2DObject_SetFontSize(IntPtr obj, int size);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dText2DObject_GetX(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dText2DObject_SetX(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dText2DObject_GetY(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dText2DObject_SetY(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dText2DObject_GetZ(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dText2DObject_SetZ(IntPtr obj, float value);

#endregion

#region v3dText3DObject

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dText3DObject_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dText3DObject_Release(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dText3DObject_GetColor(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dText3DObject_SetColor(IntPtr obj, UInt32 value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dText3DObject_GetText(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static void v3dText3DObject_SetText(IntPtr obj, string value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dText3DObject_GetFont(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dText3DObject_SetFont(IntPtr obj, IntPtr value);

#endregion

#region v3dTipAxisObject

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dTipAxisObject_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dTipAxisObject_Release(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dTipAxisObject_GetAxisLen(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dTipAxisObject_SetAxisLen(IntPtr obj, float value);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float v3dTipAxisObject_GetDrawOffset(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dTipAxisObject_SetDrawOffset(IntPtr obj, float value);


#endregion

#region v3dBox3Object

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr v3dBox3Object_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dBox3Object_Release(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dBox3Object_SetSize(IntPtr obj, SlimDX.Vector3* size);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static UInt32 v3dBox3Object_GetColor(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void v3dBox3Object_SetColor(IntPtr obj, UInt32 value);


#endregion

#region ITexture

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ITexture_Release(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static long ITexture_AddRef(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ITexture_PreUse(IntPtr obj, bool bForce, Int64 time);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static uint ITexture_GetResouceSize(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static uint ITexture_GetImageInfoWidth(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static uint ITexture_GetImageInfoHeight(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static uint ITexture_GetImageInfoMipLevels(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ITexture_GetImageInfoFormat(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int ITexture_GetColorSpace(IntPtr obj);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void ITexture_SetColorSpace(IntPtr obj, int tcs);

#endregion

#region NVPerf
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vNVPerf_SetEnable(bool bEnable);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vNVPerf_CaptureBottleneckGraph();
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vNVPerf_BottleneckUtilization(bool bBottleneck);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vNVPerf_BeginFrame(int numDrawCalls);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vNVPerf_EndFrame();
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vNVPerf_Render();


#endregion

#region Physics

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vPhysics_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vPhysics_Delete(IntPtr physics);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vPhysics_InitPhysX(IntPtr physics);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vPhysXShape_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vPhysXShape_Release(IntPtr phyShape);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vPhysXShape_SetShapeInContactTestsEnable(IntPtr phyShape, bool enable);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vPhysXShape_SetShapeInSceneQueryTestsEnable(IntPtr phyShape, bool enable);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vPhysXMaterial_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vPhysXMaterial_Release(IntPtr phyShape);

#endregion

#region Audio

        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static IntPtr vOpenAL_New();
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vOpenAL_Delete(IntPtr audioDevice);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vOpenAL_InitializeDevice(IntPtr audioDevice);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vOpenAL_KickOffBuffer(IntPtr audioDevice, Int64 nowMillisecondTime, UInt32 lifeMillisecondTime);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //public unsafe extern static UInt32 vOpenAL_PlayWAV(IntPtr audioDevice, string audioFileName, bool loop, Int64 nowMillisecondTime);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //public unsafe extern static UInt32 vOpenAL_PlayOGG(IntPtr audioDevice, string audioFileName, bool loop, Int64 nowMillisecondTime);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //public unsafe extern static UInt32 vOpenAL_PlayMP3(IntPtr audioDevice, string audioFileName, bool loop, Int64 nowMillisecondTime);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vOpenAL_Play(IntPtr audioDevice, UInt32 sourceId, bool loop);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vOpenAL_Stop(IntPtr audioDevice, UInt32 sourceId);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vOpenAL_Pause(IntPtr audioDevice, UInt32 sourceId);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static int vOpenAL_IsPlaying(IntPtr audioDevice, UInt32 sourceId);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vOpenAL_Tick(IntPtr audioDevice);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static void vOpenAL_SetOnPlayFinishedEvent(IntPtr audioDevice, MidLayer.Audio.AudioManager.Delegate_AudioManager_OnPlayFinished _event);

        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static int vOpenAL_SetListenerf(IntPtr audioDevice, int enumType, float val);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static int vOpenAL_SetListener3f(IntPtr audioDevice, int enumType, float val1, float val2, float val3);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static int vOpenAL_SetListenerOrientation(IntPtr audioDevice, float val1, float val2, float val3, float val4, float val5, float val6);

        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static int vOpenAL_SetSourcef(IntPtr audioDevice, UInt32 sourceId, int enumType, float val);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static int vOpenAL_SetSource3f(IntPtr audioDevice, UInt32 sourceId, int enumType, float val1, float val2, float val3);
        //[DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        //public unsafe extern static int vOpenAL_SetDistanceMode(IntPtr audioDevice, int model);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vFMod_SetOnPlayFinishedEvent(IntPtr audioDevice, CCore.Audio.AudioManager.Delegate_AudioManager_OnPlayFinished _event);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static IntPtr vFMod_New();
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vFMod_Delete(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vFMod_Initialize(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vFMod_KickOffSource(IntPtr ptr, Int64 nowMillisecondTime, UInt32 lifeMillisecondTime);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static int vFMod_Play(IntPtr ptr, string audioFileName, System.Guid* channelId, UInt32 soundType, CCore.Audio.enLoopType loop, Int64 nowMillisecondTime, bool rePlay, bool isStream, bool pause);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_Stop(IntPtr ptr, System.Guid* channelId);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_Pause(IntPtr ptr, System.Guid* channelId, bool pause);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_IsPlaying(IntPtr ptr, System.Guid* channelId);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vFMod_Tick(IntPtr ptr);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe extern static UInt32 vFMod_GetLength(IntPtr ptr, string audioFileName, CCore.Audio.AudioManager.enAudioTimeUnit unit);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetListenerNumbers(IntPtr ptr, int num);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetListenerAttributes(IntPtr ptr, int listenerId, SlimDX.Vector3* pos, SlimDX.Vector3* vel, SlimDX.Vector3* forward, SlimDX.Vector3* up);

        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSound3DAttributes(IntPtr ptr, Guid* channelId, SlimDX.Vector3* pos, SlimDX.Vector3* vel);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSound3DConeOrientation(IntPtr ptr, Guid* channelId, SlimDX.Vector3* orientation);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSound3DConeSettings(IntPtr ptr, Guid* channelId, float insideconeangle, float outsideconeangle, float outsidevolume);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSound3DCustomRolloff(IntPtr ptr, Guid* channelId, SlimDX.Vector3* points, int numpoints);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSound3DDistanceFilter(IntPtr ptr, Guid* channelId, bool custom, float customLevel, float centerFreq);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSound3DDopplerLevel(IntPtr ptr, Guid* channelId, float level);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSound3DLevel(IntPtr ptr, Guid* channelId, float level);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSound3DMinMaxDistance(IntPtr ptr, Guid* channelId, float mindistance, float maxdistance);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSound3DOcclusion(IntPtr ptr, Guid* channelId, float directocclusion, float reverbocclusion);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSound3DSpread(IntPtr ptr, Guid* channelId, float angle);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSoundVolumn(IntPtr ptr, Guid* channelId, float volumn);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSoundPosition(IntPtr ptr, Guid* channelId, UInt32 position, CCore.Audio.AudioManager.enAudioTimeUnit postype);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSoundMode(IntPtr ptr, Guid* channelId, CCore.Audio.AudioManager.enAudioMode mode);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSoundFrequency(IntPtr ptr, Guid* channelId, float frequency);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSoundLoopCount(IntPtr ptr, Guid* channelId, int loopcount);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSoundLoopPoints(IntPtr ptr, Guid* channelId, UInt32 loopstart, CCore.Audio.AudioManager.enAudioTimeUnit loopstarttype, UInt32 loopend, CCore.Audio.AudioManager.enAudioTimeUnit loopendtype);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSoundMute(IntPtr ptr, Guid* channelId, bool mute);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSoundPan(IntPtr ptr, Guid* channelId, float pan);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSoundPitch(IntPtr ptr, Guid* channelId, float pitch);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static int vFMod_SetSoundPriority(IntPtr ptr, Guid* channelId, int priority);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vFMod_SetSoundTypeVolume(IntPtr ptr, UInt32 soundType, float volumn);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vFMod_GetSoundTypeVolume(IntPtr ptr, UInt32 soundType);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static void vFMod_SetMainVolume(IntPtr ptr, float volumn);
        [DllImport(ModuleNC, CallingConvention = CallingConvention.Cdecl)]
        public unsafe extern static float vFMod_GetMainVolume(IntPtr ptr);

#endregion
    }
}
