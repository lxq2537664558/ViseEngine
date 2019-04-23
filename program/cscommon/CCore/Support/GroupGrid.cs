using System;

namespace CCore.Support
{
    /// <summary>
    /// 分组表格
    /// </summary>
    public class GroupGrid : CCore.Component.Visual
    {
        /// <summary>
        /// 分组表格对象指针
        /// </summary>
        protected IntPtr mGroupGridObject;  //model3.v3dGroupGridObject*
        /// <summary>
        /// 只读属性，分组表格对象指针
        /// </summary>
        public IntPtr GroupGridObject
        {
            get { return mGroupGridObject; }
        }
        /// <summary>
        /// 分组表格X轴的长度，默认为512
        /// </summary>
        protected float mXLength = 512.0f;
        /// <summary>
        /// 分组表格中X轴的长度
        /// </summary>
        [CSUtility.Support.DataValueAttribute("XLength")]
        public float XLength
        {
            get { return mXLength; }
            set
            {
                mXLength = value;
                DllImportAPI.v3dGroupGridObject_SetXLength(mGroupGridObject, value);
            }
        }
        /// <summary>
        /// 分组表格Z轴的长度，默认为512
        /// </summary>
		protected float mZLength = 512.0f;
        /// <summary>
        /// 分组表格Z轴的长度，默认为512
        /// </summary>
        [CSUtility.Support.DataValueAttribute("ZLength")]
        public float ZLength
        {
            get{return mZLength;}
            set
            {
                mZLength = value;
                CCore.DllImportAPI.v3dGroupGridObject_SetZLength(mGroupGridObject, value);
            }
        }
        /// <summary>
        /// 网格中X轴方向上的两条线之间的间隔
        /// </summary>
		protected float mDeltaX = 1.0f;
        /// <summary>
        /// 网格中X轴方向上的两条线之间的间隔，默认为1
        /// </summary>
        [CSUtility.Support.DataValueAttribute("DeltaX")]
        public float DeltaX
        {
            get{return mDeltaX;}
            set
            {
                mDeltaX = value;
                CCore.DllImportAPI.v3dGroupGridObject_SetDeltaX(mGroupGridObject, value);
            }
        }
        /// <summary>
        /// 网格中Z轴方向上的两条线之间的间隔
        /// </summary>
		protected float mDeltaZ = 1.0f;
        /// <summary>
        /// 网格中Z轴方向上的两条线之间的间隔，默认为1
        /// </summary>
        [CSUtility.Support.DataValueAttribute("DeltaZ")]
        public float DeltaZ
        {
            get{return mDeltaZ;}
            set
            {
                mDeltaZ = value;
                CCore.DllImportAPI.v3dGroupGridObject_SetDeltaZ(mGroupGridObject, value);
            }
        }
        /// <summary>
        /// 本地坐标X的值
        /// </summary>
		protected float mLocX = 0.0f;
        /// <summary>
        /// 本地坐标X的值，默认为0
        /// </summary>
        [CSUtility.Support.DataValueAttribute("LocX")]
        public float LocX
        {
            get{return mLocX;}
            set
            {
                mLocX = value;
                CCore.DllImportAPI.v3dGroupGridObject_SetLocX(mGroupGridObject, value);
            }
        }
        /// <summary>
        /// 本地坐标Y的值
        /// </summary>
		protected float mLocY = 0.0f;
        /// <summary>
        /// 本地坐标Y的值，默认为0
        /// </summary>
        [CSUtility.Support.DataValueAttribute("LocY")]
        public float LocY
        {
            get{return mLocY;}
            set
            {
                mLocY = value;
                CCore.DllImportAPI.v3dGroupGridObject_SetLocY(mGroupGridObject, value);
            }
        }
        /// <summary>
        /// 本地坐标Z的值
        /// </summary>
		protected float mLocZ = 0.0f;
        /// <summary>
        /// 本地坐标Z的值，默认为0
        /// </summary>
        [CSUtility.Support.DataValueAttribute("LocZ")]
        public float LocZ
        {
            get{return mLocZ;}
            set
            {
                mLocZ = value;
                CCore.DllImportAPI.v3dGroupGridObject_SetLocZ(mGroupGridObject, value);
            }
        }
        /// <summary>
        /// 间隔值
        /// </summary>
		protected int mInterval = 32;
        /// <summary>
        /// 间隔值大小，默认为32
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Interval")]
        public int Interval
        {
            get{return mInterval;}
            set
            {
                mInterval = value;
                CCore.DllImportAPI.v3dGroupGridObject_SetInterval(mGroupGridObject, value);
            }
        }
        /// <summary>
        /// 网格线的颜色
        /// </summary>
        protected CSUtility.Support.Color mLineColor = CSUtility.Support.Color.FromArgb(115, 115, 115);
        /// <summary>
        /// 网格线的颜色
        /// </summary>
        [CSUtility.Support.DataValueAttribute("LineColor")]
        public CSUtility.Support.Color LineColor
        {
            get { return mLineColor; }
            set
            {
                mLineColor = value;
                CCore.DllImportAPI.v3dGroupGridObject_SetlineColor(mGroupGridObject, (UInt32)value.ToArgb());
            }
        }
        /// <summary>
        /// 组颜色
        /// </summary>
		protected CSUtility.Support.Color mGroupColor = CSUtility.Support.Color.FromArgb(255, 126, 0);
        /// <summary>
        /// 组颜色
        /// </summary>
        [CSUtility.Support.DataValueAttribute("GroupColor")]
        public CSUtility.Support.Color GroupColor
        {
            get { return mGroupColor; }
            set
            {
                mGroupColor = value;
                CCore.DllImportAPI.v3dGroupGridObject_SetGroupColor(mGroupGridObject, (UInt32)value.ToArgb());
            }
        }
        /// <summary>
        /// 网格矩阵
        /// </summary>
		protected SlimDX.Matrix mTransform = SlimDX.Matrix.Identity;
        /// <summary>
        /// 网格的转换矩阵
        /// </summary>
        public SlimDX.Matrix Transform
        {
            get { return mTransform; }
            set
            {
                unsafe
                {
                    fixed (SlimDX.Matrix* pinTransform = &mTransform)
                    {
                        mTransform = value;
                        CCore.DllImportAPI.v3dGroupGridObject_SetRelativeMatrix(mGroupGridObject, pinTransform);
                    }
                }
            }
        }
        /// <summary>
        /// 构造函数，创建网格对象
        /// </summary>
        public GroupGrid()
        {
            mGroupGridObject = CCore.DllImportAPI.v3dGroupGridObject_New();
            mLayer = RLayer.RL_SystemHelper;
        }
        /// <summary>
        /// 析构函数，删除对象
        /// </summary>
        ~GroupGrid()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除对象，释放指针
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
            if (mGroupGridObject != IntPtr.Zero)
            {
                CCore.DllImportAPI.v3dGroupGridObject_Release(mGroupGridObject);
                mGroupGridObject = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 提交对象到渲染环境
        /// </summary>
        /// <param name="renderEnv">渲染环境</param>
        /// <param name="matrix">当前对象的位置矩阵</param>
        /// <param name="eye">视野</param>
        public override void Commit(CCore.Graphics.REnviroment renderEnv, ref SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            if(mGroupGridObject != IntPtr.Zero)
                CCore.DllImportAPI.vDSRenderEnv_CommitHelperGroupGrid(renderEnv.DSRenderEnv, (int)mGroup, mGroupGridObject);
        }
        /// <summary>
        /// 保存对象数据
        /// </summary>
        /// <param name="absPath">据对路径</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public bool Save(string absPath)
        {
            return CSUtility.Support.IConfigurator.SaveProperty(this, "Grid", absPath + "Grid.cfg");
        }
        /// <summary>
        /// 加载网格数据
        /// </summary>
        /// <param name="absPath">网格对象的绝对路径</param>
        /// <returns>成功加载返回true，否则返回false</returns>
        public bool Load(string absPath)
        {
            return CSUtility.Support.IConfigurator.FillProperty(this, absPath + "Grid.cfg");
        }
    }
}
