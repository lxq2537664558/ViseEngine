using System;

namespace CCore.Camera
{
    /// <summary>
    /// 用来初始化Camera的类
    /// </summary>
    public class CameraInit
    {
        /// <summary>
        /// 摄像机的FOV参数
        /// </summary>
		public float	Fov;
        /// <summary>
        /// 摄像机的宽
        /// </summary>
		public int		Width;
        /// <summary>
        /// 摄像机的高
        /// </summary>
		public int		Height;
        /// <summary>
        /// 摄像机所在的Z轴最小值
        /// </summary>
		public float	ZNear;
        /// <summary>
        /// 摄像机所在的Z轴最大值
        /// </summary>
		public float	ZFar;
        /// <summary>
        /// 摄像机的位置
        /// </summary>
        public SlimDX.Vector3 Pos;
        /// <summary>
        /// 摄像机的LOOK
        /// </summary>
        public SlimDX.Vector3 Look;
        /// <summary>
        /// 摄像机的UP参数
        /// </summary>
        public SlimDX.Vector3 Up;
        /// <summary>
        /// 构造函数，设置camera的一些默认属性值
        /// </summary>
        public CameraInit()
        {
			Fov = (float)(28 * System.Math.PI/180.0f);
			Width = 1024;
			Height = 768;
			ZNear = 0.1f;
			ZFar = 1000.0f;
            Pos = new SlimDX.Vector3(0, 0, 0);
            Look = new SlimDX.Vector3(0, 0, 1);
            Up = new SlimDX.Vector3(0, 1, 0);
        }
    }
    /// <summary>
    /// 摄像机类
    /// </summary>
	[System.ComponentModel.TypeConverterAttribute( "System.ComponentModel.ExpandableObjectConverter" )]
    public class CameraObject
    {
        IntPtr mCameraPtr;
        IntPtr mCommitCamera;
        /// <summary>
        /// 摄像机的内存地址
        /// </summary>
        public IntPtr CameraPtr
        {
            get { return mCameraPtr; }
        }
        /// <summary>
        /// 提交的摄像机地址
        /// </summary>
        public IntPtr CommitCamera
        {
            get { return mCommitCamera; }
        }
        /// <summary>
        /// 复制提交的摄像机
        /// </summary>
        public void CopyCommitCamera()
        {
            DllImportAPI.v3dCamera_CopyData(mCommitCamera, mCameraPtr);
        }
        /// <summary>
        /// FOV属性，编辑器中可调
        /// </summary>
        public float FOV
        {
            get { return GetFOV(); }
            set { SetFOV(value); }
        }
        /// <summary>
        /// 只读属性，摄像机的位置
        /// </summary>
        public SlimDX.Vector3 Location
        {
            get
            {
                unsafe
                {
                    SlimDX.Vector3 pos;
                    DllImportAPI.v3dCamera_GetPosition(mCameraPtr, &pos);
                    return pos;
                }
            }
        }
        /// <summary>
        /// 只读属性，摄像机的方向
        /// </summary>
        public SlimDX.Vector3 Direction
        {
            get
            {
                unsafe
                {
                    SlimDX.Vector3 dir;
                    DllImportAPI.v3dCamera_GetDirVec(mCameraPtr, &dir);
                    return dir;
                }
            }
        }
        /// <summary>
        /// 摄像机Z轴的最近距离
        /// </summary>
        public float ZNear
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dCamera_GetNear(mCameraPtr);
                }
            }
        }
        /// <summary>
        /// 摄像机Z轴的最远距离
        /// </summary>
        public float ZFar
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dCamera_GetFar(mCameraPtr);
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CameraObject()
        {

        }
        /// <summary>
        /// 析构函数，释放实例
        /// </summary>
        ~CameraObject()
        {
            Cleanup();
        }
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">摄像机的初始化类</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool Initialize(CameraInit _init)
        {
            if (Engine.Instance.Client == null)
                return false;
            if (Engine.Instance.Client.Graphics == null)
                return false;

            if (mCameraPtr == IntPtr.Zero)
            {
                unsafe
                {
                    mCameraPtr = DllImportAPI.v3dCamera_New(Engine.Instance.Client.Graphics.Device);
                    DllImportAPI.v3dCamera_MakePerspective(mCameraPtr, _init.Fov, _init.Width, _init.Height, _init.ZNear, _init.ZFar);
                    SlimDX.Vector3 pos = _init.Pos;
                    SlimDX.Vector3 look = _init.Look;
                    SlimDX.Vector3 up = _init.Up;
                    DllImportAPI.v3dCamera_SetPosLookAtUp(mCameraPtr, &pos, &look, &up);

                    mCommitCamera = DllImportAPI.v3dCamera_New(Engine.Instance.Client.Graphics.Device);
                    CopyCommitCamera();
                }
            }

            return true;
        }
        /// <summary>
        /// 清除摄像机，并释放实例内存
        /// </summary>
        public void Cleanup()
        {
            if(mCameraPtr != IntPtr.Zero)
            {
                DllImportAPI.v3dCamera_Release(mCameraPtr);
                mCameraPtr = IntPtr.Zero;
            }

            if (mCommitCamera != IntPtr.Zero)
            {
                DllImportAPI.v3dCamera_Release(mCommitCamera);
                mCommitCamera = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 得到摄像机的当前位置
        /// </summary>
        /// <returns>返回摄像机对象的当前位置</returns>
        public SlimDX.Vector3 GetLocation()
        {
            unsafe
            {
                SlimDX.Vector3 vec = new SlimDX.Vector3();
                DllImportAPI.v3dCamera_GetPosition(mCameraPtr, &vec);
                return vec;
            }
        }
        /// <summary>
        /// 摄像机的点击方向
        /// </summary>
        /// <param name="x">点击的X值</param>
        /// <param name="y">点击的Y值</param>
        /// <returns>返回点击后摄像机的方向</returns>
        public SlimDX.Vector3 GetPickDirection(int x, int y)
        {
            unsafe
            {
                SlimDX.Vector3 vec = new SlimDX.Vector3();
                DllImportAPI.v3dCamera_GetPickRay(mCameraPtr, &vec, x, y);
                return vec;
            }
        }
        /// <summary>
        /// 摄像机的移动
        /// </summary>
        /// <param name="vec">移动的vector3</param>
        public void MoveByCamera(SlimDX.Vector3 vec)
        {
            unsafe
            {
                DllImportAPI.v3dCamera_MoveByCameraAxis(mCameraPtr, &vec);
            }
        }
        /// <summary>
        /// 世界中的屏幕尺寸
        /// </summary>
        /// <param name="worldPos">世界坐标</param>
        /// <param name="screenSize">屏幕尺寸</param>
        /// <returns>返回世界中的屏幕尺寸</returns>
        public float GetScreenSizeInWorld(SlimDX.Vector3 worldPos, float screenSize)
        {
            unsafe
            {
                return DllImportAPI.v3dCamera_GetScreenSizeInWorld(mCameraPtr, &worldPos, screenSize);
            }
        }
        /// <summary>
        /// 屏幕中的世界尺寸
        /// </summary>
        /// <param name="worldPos">世界坐标</param>
        /// <param name="worldSize">世界的大小</param>
        /// <returns>返回屏幕中世界的尺寸</returns>
        public float GetWorldSizeInScreen(SlimDX.Vector3 worldPos, float worldSize)
        {
            unsafe
            {
                return DllImportAPI.v3dCamera_GetWorldSizeInScreen(mCameraPtr, &worldPos, worldSize);
            }
        }
        /// <summary>
        /// 屏幕的坐标
        /// </summary>
        /// <param name="pt">坐标值</param>
        /// <param name="fMRTScale">缩放值</param>
        /// <returns>返回屏幕的坐标</returns>
		public SlimDX.Vector3 GetScreenCoord(ref SlimDX.Vector3 pt, float fMRTScale)
        {
            unsafe
            {
                SlimDX.Vector3 result;
                fixed (SlimDX.Vector3* pinPt = &pt)
                {
                    if (DllImportAPI.v3dCamera_Trans2Screen(mCameraPtr, &result, pinPt) == IntPtr.Zero)
                        return SlimDX.Vector3.Zero;
                }

                result.X /= fMRTScale;
                result.Y /= fMRTScale;
                return result;
            }
        }
        /// <summary>
        /// 视口矩阵
        /// </summary>
        /// <param name="matrix">转换矩阵</param>
		public void GetViewMatrix( ref SlimDX.Matrix matrix )
        {
            unsafe
            {
                fixed (SlimDX.Matrix* pinMatrix = &matrix)
                {
                    DllImportAPI.v3dCamera_GetViewTM(mCameraPtr, pinMatrix);
                }
            }
        }
        /// <summary>
        /// 项目矩阵
        /// </summary>
        /// <param name="matrix">转换矩阵</param>
		public void GetProjMatrix( ref SlimDX.Matrix matrix )
        {
            unsafe
            {
                fixed (SlimDX.Matrix* pinMatrix = &matrix)
                {
                    DllImportAPI.v3dCamera_GetProjTM(mCameraPtr, pinMatrix);
                }
            }
        }
        /// <summary>
        /// 世界矩阵
        /// </summary>
        /// <param name="matrix">转换矩阵</param>
		public void GetWorldMatrix( ref SlimDX.Matrix matrix )
        {

        }
        /// <summary>
        /// 广告牌矩阵
        /// </summary>
        /// <param name="matrix">转换矩阵</param>
        public void GetBillboardMatrix(ref SlimDX.Matrix matrix)
        {
            unsafe
            {
                fixed (SlimDX.Matrix* pinMatrix = &matrix)
                {
                    DllImportAPI.v3dCamera_GetBillboardTM(mCameraPtr, pinMatrix);
                }
            }
        }

        /// <summary>
        /// 摄像机的FOV值
        /// </summary>
        /// <returns>返回摄像机的FOV</returns>
		public float GetFOV()
        {
            unsafe
            {
                return DllImportAPI.v3dCamera_GetFOV(mCameraPtr);
            }
        }
        /// <summary>
        /// 设置摄像机的FOV
        /// </summary>
        /// <param name="value">FOV的值</param>
		public void SetFOV(float value)
        {
            unsafe
            {
                var viewPortWidth = DllImportAPI.v3dCamera_GetViewPortWidth(mCameraPtr);
                var viewPortHeight = DllImportAPI.v3dCamera_GetViewPortHeight(mCameraPtr);
                var near = DllImportAPI.v3dCamera_GetNear(mCameraPtr);
                var far = DllImportAPI.v3dCamera_GetFar(mCameraPtr);
                DllImportAPI.v3dCamera_MakePerspective(mCameraPtr, value, viewPortWidth, viewPortHeight, near, far);
            }
        }
        /// <summary>
        /// 计算X轴方向的vector
        /// </summary>
        /// <param name="vec">转换矩阵</param>
		public void GetXVector( ref SlimDX.Vector3 vec )
        {
            unsafe
            {
                fixed (SlimDX.Vector3* pinVec = &vec)
                {
                    DllImportAPI.v3dCamera_GetRightVec(mCameraPtr, pinVec);
                }
            }
        }
        /// <summary>
        /// 计算Y轴方向的vector
        /// </summary>
        /// <param name="vec">转换矩阵</param>
		public void GetYVector( ref SlimDX.Vector3 vec )
        {
            unsafe
            {
                fixed (SlimDX.Vector3* pinVec = &vec)
                {
                    DllImportAPI.v3dCamera_GetUpVec(mCameraPtr, pinVec);
                }
            }
        }
        /// <summary>
        /// 计算Z轴方向的vector
        /// </summary>
        /// <param name="vec">转换矩阵</param>
		public void GetZVector( ref SlimDX.Vector3 vec )
        {
            unsafe
            {
                fixed (SlimDX.Vector3* pinVec = &vec)
                {
                    DllImportAPI.v3dCamera_GetDirVec(mCameraPtr, pinVec);
                }
            }
        }
        /// <summary>
        /// 当前的right值
        /// </summary>
        /// <param name="vec">转换矩阵</param>
		public void GetLocalRight( ref SlimDX.Vector3 vec )
        {
            unsafe
            {
                fixed (SlimDX.Vector3* pinVec = &vec)
                {
                    DllImportAPI.v3dCamera_GetLocalRightVec(mCameraPtr, pinVec);
                }
            }
        }
        /// <summary>
        /// 当前的UP值
        /// </summary>
        /// <param name="vec">转换矩阵</param>
        public void GetLocalUp(ref SlimDX.Vector3 vec)
        {
            unsafe
            {
                fixed (SlimDX.Vector3* pinVec = &vec)
                {
                    DllImportAPI.v3dCamera_GetLocalUpVec(mCameraPtr, pinVec);
                }
            }
        }
        
        /// <summary>
        /// 获取欧拉角度
        /// </summary>
        /// <param name="vec">位置坐标</param>
		public void GetEular( ref SlimDX.Vector3 vec )
        {

        }
        /// <summary>
        /// 摄像机X轴向的Vector3
        /// </summary>
        /// <returns>返回摄像机X轴向的Vector3</returns>
		public SlimDX.Vector3 GetXVector()
        {
            SlimDX.Vector3 vec = new SlimDX.Vector3();
            GetXVector(ref vec);
            return vec;
        }
        /// <summary>
        /// 摄像机Y轴向的Vector3
        /// </summary>
        /// <returns>返回摄像机X轴向的Vector3</returns>
		public SlimDX.Vector3 GetYVector()
        {
            SlimDX.Vector3 vec = new SlimDX.Vector3();
            GetYVector(ref vec);
            return vec;
        }
        /// <summary>
        /// 摄像机Z轴向的Vector3
        /// </summary>
        /// <returns>返回摄像机X轴向的Vector3</returns>
        public SlimDX.Vector3 GetZVector()
        {
            SlimDX.Vector3 vec = new SlimDX.Vector3();
            GetZVector(ref vec);
            return vec;
        }
        /// <summary>
        /// 标准的vector3
        /// </summary>
        /// <returns>返回标准的vector3</returns>
		public SlimDX.Vector3 GetEular()
        {
            return new SlimDX.Vector3();
        }
        /// <summary>
        /// 透视投影
        /// </summary>
        /// <param name="Fov">摄像机的FOV值</param>
        /// <param name="w">摄像机的宽</param>
        /// <param name="h">摄像机的高</param>
        /// <param name="zn">摄像机的Z轴最近值</param>
        /// <param name="zf">摄像机的Z轴最远值</param>
		public void MakePerspective(float Fov , float w , float h , float zn , float zf)
        {
            unsafe
            {
                DllImportAPI.v3dCamera_MakePerspective(mCameraPtr, Fov, w, h, zn, zf);
            }
        }
        /// <summary>
        /// 平行投影
        /// </summary>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <param name="pvw">摄像机的宽</param>
        /// <param name="pvh">摄像机的高</param>
		public void MakeOrtho(float w , float h , float pvw , float pvh)
        {
            unsafe
            {
                var fNear = DllImportAPI.v3dCamera_GetNear(mCameraPtr);
                var fFar = DllImportAPI.v3dCamera_GetFar(mCameraPtr);
                DllImportAPI.v3dCamera_MakeOrtho(mCameraPtr, w, h, pvw, pvh, fNear, fFar);
            }
        }
        /// <summary>
        /// 平行投影
        /// </summary>
        /// <param name="w">视野的宽</param>
        /// <param name="pvw">摄像机的宽</param>
        /// <param name="pvh">摄像机的高</param>
        /// <param name="zn">对象右侧的Z值</param>
        /// <param name="zf">对象左侧的Z值</param>
		public void MakeOrthoAutoAspect(float w , float pvw , float pvh , float zn , float zf)
        {
            unsafe
            {
                DllImportAPI.v3dCamera_MakeOrthoAutoAspect(mCameraPtr, w, pvw, pvh, zn, zf);
            }
        }
        /// <summary>
        /// 点是否在视截体中
        /// </summary>
        /// <param name="vec">点坐标</param>
        /// <returns>在视截体中返回true，否则返回false</returns>
        public bool IsFrustumContainVector(ref SlimDX.Vector3 vec)
        {
            unsafe
            {
                fixed (SlimDX.Vector3* pinVec = &vec)
                {
                    return (DllImportAPI.v3dCamera_IsFrustumContainVector(CameraPtr, pinVec)) != 0 ? true : false;
                }
            }
        }

        /// <summary>
        /// 包围盒是否在视截体中
        /// </summary>
        /// <param name="box">外接盒</param>
        /// <returns>在视截体中返回true，否则返回false</returns>
        public bool IsFrustumContainBox(ref SlimDX.BoundingBox box)
        {
            unsafe
            {
                fixed (SlimDX.BoundingBox* pinBox = &box)
                {
                    return (DllImportAPI.v3dCamera_IsFrustumContainBox(CameraPtr, pinBox)) != 0 ? true : false;
                }
            }
        }
        /// <summary>
        /// OBB包围盒是否在视截体中
        /// </summary>
        /// <param name="box">外接盒</param>
        /// <param name="matrix">转换矩阵</param>
        /// <returns>在视截体中返回true，否则返回false</returns>
        public bool IsFrustumContainOBB(ref SlimDX.BoundingBox box, ref SlimDX.Matrix matrix)
        {
            unsafe
            {
                fixed (SlimDX.BoundingBox* pinBox = &box)
                {
                    fixed (SlimDX.Matrix* pinMatrix = &matrix)
                    {
                        return (DllImportAPI.v3dCamera_IsFrustumContainOBB(CameraPtr, pinBox, pinMatrix)) != 0 ? true : false;
                    }
                }
            }
        }
    }
}
