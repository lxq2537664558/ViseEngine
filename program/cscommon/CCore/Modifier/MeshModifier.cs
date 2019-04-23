using System;
using System.ComponentModel;

namespace CCore.Modifier
{
    /// <summary>
    /// mesh模拟器的初始化类
    /// </summary>
    public struct MeshModifierInit
    {
    };
    /// <summary>
    /// mesh模拟器类
    /// </summary>
    public class MeshModifier : CSUtility.Support.XndSaveLoadProxy
    {
        /// <summary>
        /// mesh模拟器的对象指针
        /// </summary>
        protected IntPtr mModifier = IntPtr.Zero;
        /// <summary>
        /// 只读属性，mesh模拟器的对象指针
        /// </summary>
        [Browsable(false)]
        public IntPtr Modifier
        {
            get { return mModifier; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mod">mesh模拟器的对象</param>
        public MeshModifier(IntPtr mod)
        {
            mModifier = mod;
            if(mModifier != IntPtr.Zero)
                DllImportAPI.V3DModifier_AddRef(mModifier);
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~MeshModifier()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除对象，释放指针内存
        /// </summary>
        public virtual void Cleanup()
        {
            if (mModifier != IntPtr.Zero)
            {
                DllImportAPI.V3DModifier_Release(mModifier);
                mModifier = IntPtr.Zero;

            }
        }
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="_init">初始化类</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public virtual bool Initialize(MeshModifierInit _init)
        {
            Cleanup();
            return true;
        }
    }
}
