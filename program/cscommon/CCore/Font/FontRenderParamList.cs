using System;
using System.Collections.Generic;

namespace CCore.Font
{
    /// <summary>
    /// 字体渲染列表
    /// </summary>
    public class FontRenderParamList
    {
        /// <summary>
        /// 对象的指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 字体渲染列表的实例地址，用于新建字体渲染参数列表时保存其内存地址
        /// </summary>
        public IntPtr Inner
        {
            get { return mInner; }
            set
            {
                mInner = value;
            }
        }

		List<FontRenderParam> mRenderParams = new List<FontRenderParam>();
        /// <summary>
        /// 得到所有的字体渲染参数
        /// </summary>
        /// <returns>返回字体渲染参数列表</returns>
        public List<FontRenderParam> GetAllRenderParams()
        {
            return mRenderParams;
        }
        /// <summary>
        /// 字体渲染列表的构造函数，并保存其指针地址
        /// </summary>
	    public FontRenderParamList()
	    {
            //DllImportAPI.V3DFontRenderParamList_New3();
            //mInner = DllImportAPI.V3DFontRenderParamList_New2();
            mInner = DllImportAPI.V3DFontRenderParamList_New();
        }
        /// <summary>
        /// 带参的字体渲染列表的构造函数，设置是否添加默认参数，并保存其指针地址
        /// </summary>
        /// <param name="addDefautParam">是否添加默认参数</param>
	    public FontRenderParamList(bool addDefautParam)
	    {
            mInner = DllImportAPI.V3DFontRenderParamList_New();

		    if(addDefautParam)
		    {
			    var param = AddParam();
			    param = null;
		    }
	    }
        /// <summary>
        /// 带参的字体渲染列表的构造函数，根据指定的字体渲染列表创建，并保存其指针地址
        /// </summary>
        /// <param name="src">字体渲染列表源数据</param>
	    public FontRenderParamList(FontRenderParamList src)
	    {
            mInner = DllImportAPI.V3DFontRenderParamList_New();

		    for( int i=0 ; i<DllImportAPI.V3DFontRenderParamList_GetSize(src.Inner); i++ )
		    {
                var paramInner = DllImportAPI.V3DFontRenderParamList_GetParam(src.Inner, i);
                var newParamInner = DllImportAPI.V3DFontRenderParam_Clone(paramInner);
                DllImportAPI.V3DFontRenderParamList_PushBack(mInner, newParamInner);
                FontRenderParam newParam = FontRenderParam.CreateRenderParamInstance(this, newParamInner);
                mRenderParams.Add(newParam);
                DllImportAPI.V3DFontRenderParam_Release(newParamInner);
		    }
	    }
        /// <summary>
        /// 析构函数，释放对象内存
        /// </summary>
	    ~FontRenderParamList()
	    {
		    Cleanup();
            if(mInner != IntPtr.Zero)
            {
                DllImportAPI.V3DFontRenderParamList_Release(mInner);
                mInner = IntPtr.Zero;
            }
	    }
        /// <summary>
        /// 清空字体渲染列表并释放实例内存
        /// </summary>
	    public void Cleanup()
	    {
            lock (this)
            {
                if (mInner != IntPtr.Zero)
                {
                    DllImportAPI.V3DFontRenderParamList_Clear(mInner);
                }
                mRenderParams.Clear();
            }
	    }
        /// <summary>
        /// 添加字体渲染参数
        /// </summary>
        /// <returns>返回添加的渲染参数</returns>
        public FontRenderParam AddParam()
	    {
            lock (this)
            {
                var frp = DllImportAPI.V3DFontRenderParam_New();
                DllImportAPI.V3DFontRenderParamList_PushBack(mInner, frp);
                FontRenderParam result = FontRenderParam.CreateRenderParamInstance(this, frp);
                mRenderParams.Add(result);
                DllImportAPI.V3DFontRenderParam_Release(frp);
                return result;
            }
	    }
        /// <summary>
        /// 根据索引删除相应的渲染参数
        /// </summary>
        /// <param name="index">要删除的渲染参数索引值</param>
        public void RemoveParam(int index)
        {
            lock (this)
            {
                if (index < 0 || index >= DllImportAPI.V3DFontRenderParamList_GetSize(mInner))
                    return;
                DllImportAPI.V3DFontRenderParamList_Erase(mInner, index);
                mRenderParams.RemoveAt(index);
            }
        }
        /// <summary>
        /// 得到对应索引的渲染参数
        /// </summary>
        /// <param name="index">渲染参数列表的索引值</param>
        /// <returns>返回对应索引的字体渲染参数</returns>
	    public FontRenderParam GetParam(int index)
	    {
            lock (this)
            {
                if (mRenderParams.Count <= index)
                    return null;
                return mRenderParams[index];
            }
	    }
        /// <summary>
        /// 得到渲染参数数量
        /// </summary>
        /// <returns>返回字体渲染参数的个数</returns>
	    public int GetParamCount()
	    {
		    return mRenderParams.Count;
	    }


    }
}
