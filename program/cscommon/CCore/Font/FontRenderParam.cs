using System;

namespace CCore.Font
{
    /// <summary>
    /// 字体轮廓类型的枚举值
    /// </summary>
    public enum enFontOutlineType : int
	{
		None = 0,
		Line = 1,
		Inner = 2,
		Outer = 3,
	};
    /// <summary>
    /// 字体四个顶点的颜色
    /// </summary>
	public class FourVertexColor
	{
        /// <summary>
        /// 字体左下顶点颜色，默认为白色
        /// </summary>
		public CSUtility.Support.Color BLColor = CSUtility.Support.Color.White;
        /// <summary>
        /// 字体右下顶点颜色，默认为白色
        /// </summary>
        public CSUtility.Support.Color BRColor = CSUtility.Support.Color.White;
        /// <summary>
        /// 字体左上顶点颜色，默认为白色
        /// </summary>
        public CSUtility.Support.Color TLColor = CSUtility.Support.Color.White;
        /// <summary>
        /// 字体右上顶点颜色，默认为白色
        /// </summary>
        public CSUtility.Support.Color TRColor = CSUtility.Support.Color.White;
        /// <summary>
        /// 构造函数
        /// </summary>
        public FourVertexColor()
        {

        }
        /// <summary>
        /// 四顶点的颜色设置
        /// </summary>
        /// <param name="bl">左下顶点的颜色</param>
        /// <param name="br">右下顶点的颜色</param>
        /// <param name="tl">左上顶点的颜色</param>
        /// <param name="tr">右上顶点的颜色</param>
		public FourVertexColor(CSUtility.Support.Color bl, CSUtility.Support.Color br, CSUtility.Support.Color tl, CSUtility.Support.Color tr)
		{
			BLColor = bl;
			BRColor = br;
			TLColor = tl;
			TRColor = tr;
		}
	};
    
    /// <summary>
    /// 字体的渲染参数
    /// </summary>
    public class FontRenderParam : CSUtility.Component.IComponent
    {
        /// <summary>
        /// 实例的指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 实例地址
        /// </summary>
        public IntPtr Inner
        {
            get { return mInner; }
            set
            {
                mInner = value;
            }
        }

        FontRenderParamList mHolder = null;
        /// <summary>
        /// 字体的左下顶点颜色
        /// </summary>
        public CSUtility.Support.Color BLColor
        {
            get
            {
                unsafe
                {
                    return CSUtility.Support.Color.FromArgb(DllImportAPI.V3DFontRenderParam_GetBLColor(mInner));
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DFontRenderParam_SetBLColor(mInner, value.ToArgb());
                }
            }
        }
        /// <summary>
        /// 字体的右下顶点颜色
        /// </summary>
        public CSUtility.Support.Color BRColor
        {
            get
            {
                unsafe
                {
                    return CSUtility.Support.Color.FromArgb(DllImportAPI.V3DFontRenderParam_GetBRColor(mInner));
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DFontRenderParam_SetBRColor(mInner, value.ToArgb());
                }
            }
        }
        /// <summary>
        /// 字体的左上顶点颜色
        /// </summary>
        public CSUtility.Support.Color TLColor
        {
            get
            {
                unsafe
                {
                    return CSUtility.Support.Color.FromArgb(DllImportAPI.V3DFontRenderParam_GetTLColor(mInner));
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DFontRenderParam_SetTLColor(mInner, value.ToArgb());
                }
            }
        }
        /// <summary>
        /// 字体的透明度
        /// </summary>
        public float Opacity
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DFontRenderParam_GetOpacity(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DFontRenderParam_SetOpacity(mInner, value);
                }
            }
        }
        /// <summary>
        /// 字体的右上顶点颜色
        /// </summary>
        public CSUtility.Support.Color TRColor
        {
            get
            {
                unsafe
                {
                    return CSUtility.Support.Color.FromArgb(DllImportAPI.V3DFontRenderParam_GetTRColor(mInner));
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DFontRenderParam_SetTRColor(mInner, value.ToArgb());
                }
            }
        }
        /// <summary>
        /// 字体的外轮廓类型
        /// </summary>
        public enFontOutlineType OutlineType
        {
            get
            {
                unsafe
                {
                    return (enFontOutlineType)DllImportAPI.V3DFontRenderParam_GetOutlineType(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DFontRenderParam_SetOutlineType(mInner, (int)value);
                }
            }
        }
        /// <summary>
        /// 字体外轮廓的厚度
        /// </summary>
        public float OutlineThickness
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.V3DFontRenderParam_GetOutlineThickness(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.V3DFontRenderParam_SetOutlineThickness(mInner, value);
                }
            }
        }

        FourVertexColor mFourVertexColor = new FourVertexColor();
        /// <summary>
        /// 字体顶点的颜色
        /// </summary>
		[System.ComponentModel.DisplayName("颜色")]
        public FourVertexColor FourVertexColor
        {
            get{return mFourVertexColor;}
            set
            {
                mFourVertexColor = value;

			    BLColor = value.BLColor;
			    BRColor = value.BRColor;
			    TLColor = value.TLColor;
			    TRColor = value.TRColor;
        
                OnPropertyChanged("FourVertexColor");
            }
        }
        /// <summary>
        /// 字体属性的构造函数，用于创建字体
        /// </summary>
        public FontRenderParam()
        {
            mInner = DllImportAPI.V3DFontRenderParam_New();
        }
        /// <summary>
        /// 析构函数，释放实例内存
        /// </summary>
        ~FontRenderParam()
        {
            Cleanup();
        }
        /// <summary>
        /// 释放字体实例内存并指针置空，在删除实例时调用
        /// </summary>
        void Cleanup()
        {
            if (mInner != IntPtr.Zero)
            {
                DllImportAPI.V3DFontRenderParam_Release(mInner);
                mInner = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 创建字体渲染参数
        /// </summary>
        /// <param name="holder">字体渲染列表</param>
        /// <param name="inner">实例指针</param>
        /// <returns>需要渲染的字体指针</returns>
        public static FontRenderParam CreateRenderParamInstance(FontRenderParamList holder, IntPtr inner)
		{
			var result = new FontRenderParam();
			result.Cleanup();
			result.mHolder = holder;
			result.Inner = inner;
            DllImportAPI.V3DFontRenderParam_AddRef(inner);
			return result;
		}
        /// <summary>
        /// 加载字体渲染节点的参数
        /// </summary>
        /// <param name="node">字体节点的参数</param>
        public void OnLoad(CSUtility.Support.XmlNode node)
	    {
		    CSUtility.Support.XmlAttrib attr = null;
		    int iARGB;
		    attr = node.FindAttrib("BLColor");
		    if (attr != null)
		    {
			    if (int.TryParse(attr.Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out iARGB) == true)
			    {
				    BLColor = CSUtility.Support.Color.FromArgb(iARGB);
			    }
		    }
		    attr = node.FindAttrib("BRColor");
		    if (attr != null)
		    {
                if (int.TryParse(attr.Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out iARGB) == true)
			    {
				    BRColor = CSUtility.Support.Color.FromArgb(iARGB);
			    }
		    }
		    attr = node.FindAttrib("TLColor");
		    if (attr != null)
		    {
                if (int.TryParse(attr.Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out iARGB) == true)
			    {
				    TLColor = CSUtility.Support.Color.FromArgb(iARGB);
			    }
		    }
		    attr = node.FindAttrib("TRColor");
		    if (attr != null)
		    {
                if (int.TryParse(attr.Value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out iARGB) == true)
			    {
				    TRColor = CSUtility.Support.Color.FromArgb(iARGB);
			    }
		    }
		    attr = node.FindAttrib("OutlineType");
		    if (attr != null)
		    {
                try
                {
                    OutlineType = (enFontOutlineType)System.Enum.Parse(typeof(enFontOutlineType), attr.Value);
                }
                catch (System.Exception)
                {
                    OutlineType = enFontOutlineType.None;
                }
                
		    }
		    attr = node.FindAttrib("OutlineThickness");
		    if (attr != null)
			    OutlineThickness = System.Convert.ToSingle(attr.Value);
	    }
        /// <summary>
        /// 保存渲染字体的各项参数
        /// </summary>
        /// <param name="node">保存字体参数的节点</param>
        public void OnSave(CSUtility.Support.XmlNode node)
	    {
		    node.AddAttrib("BLColor", BLColor.Name);
		    node.AddAttrib("BRColor", BRColor.Name);
		    node.AddAttrib("TLColor", TLColor.Name);
		    node.AddAttrib("TRColor", TRColor.Name);
		    node.AddAttrib("OutlineType", OutlineType.ToString());
		    node.AddAttrib("OutlineThickness", OutlineThickness.ToString());
	    }

    }
}
