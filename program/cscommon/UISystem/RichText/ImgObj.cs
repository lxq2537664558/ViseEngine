using System;
using CSUtility.Support;

namespace UISystem.RichText
{

    public class ImgObj : FragmentObj
    {
        protected SlimDX.Matrix mTransMatrix = SlimDX.Matrix.Identity;
        protected SlimDX.Vector4 mBackColor = new SlimDX.Vector4(0, 0, 0, 0);

        public ImgObj()
        {
            Type = "img";
        }

        override public bool OnLoad(XmlNode pXml)
        {
            // 解析的XML文本示例:
            //<F Type="img" W="64" H="64" Content="animUV guid">
            //</F>

            XmlAttrib attr = null;

            attr = pXml.FindAttrib("W");
            if (attr != null)
                Width = Convert.ToInt32(attr.Value);

            attr = pXml.FindAttrib("H");
            if (attr != null)
                Height = Convert.ToInt32(attr.Value);

            attr = pXml.FindAttrib("Return");
            if (attr != null)
                mReturn = Convert.ToInt32(attr.Value) > 0 ? true : false;

            attr = pXml.FindAttrib("Content");
            if (attr != null)
                Content = attr.Value;

            return true;
        }

        override public bool OnSave(XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            pXml.AddAttrib("W", Width.ToString());
            pXml.AddAttrib("H", Height.ToString());
            pXml.AddAttrib("Content", Content);
            int iReturn = mReturn ? 1 : 0;
            pXml.AddAttrib("Return", iReturn.ToString());

            return true;
        }

        override public void Draw(UIRenderPipe pipe, int zOrder)
        {
            if (UVAnim != null)
            {
                UVFrame frame = UVAnim.GetUVFrame(CCore.Engine.Instance.GetFrameSecondTimeFloatByUVAnim());
                int imageBottomLine = MaxBottomLine;
                if (MaxBottomLine == 0)
                {
                    imageBottomLine = Line.MaxBottomLine;
                }
                CSUtility.Support.Rectangle dst = new CSUtility.Support.Rectangle(
                    PenX, 
                    PenY + imageBottomLine - Height, 
                    (int)(Width ), 
                    (int)(Height ));
                    //(int)(Width * ScaleX), 
                    //(int)(Height * ScaleY));
                if (Material != null && Doc!=null && Doc.ParentCtrl!=null)
                {
                    WinRoot root = (WinRoot)Doc.ParentCtrl.GetRoot();

                    if (frame.Scale9DrawRectangles.Count > 0)
                    {
                        foreach (var scaleInfo in frame.Scale9DrawRectangles)
                        {
                            var refRect = scaleInfo.GetDrawRect(dst);
                            IRender.GetInstance().DrawImage(pipe, zOrder, root.Width, root.Height,
                                              Texture,
                                              ref mBackColor,
                                              ref refRect,
                                              ref scaleInfo.mDrawUVRect, ref mTransMatrix, 1, Material);
                        }
                    }
                    else
                    {
                        IRender.GetInstance().DrawImage(pipe, zOrder, root.Width, root.Height, Texture, ref mBackColor,
                        ref dst,
                        ref frame.mUVRect, ref mTransMatrix, 1, Material);
                    }
                }
            }
        }

        public override string Content
        {
            get { return base.Content; }
            set
            {
                base.Content = value;
                
                UVAnim = UVAnimMgr.Instance.Find(CSUtility.Support.IHelper.GuidTryParse(value), true);
            }
        }

        public CCore.Graphics.Texture Texture
        {
            get
            {
                if (mUVAnim == null)
                    return null;
                return mUVAnim.TextureObject;
            }
        }
        public CCore.Material.Material Material
        {
            get
            {
                if (mUVAnim == null)
                    return null;
                return mUVAnim.MaterialObject;
            }
        }

        UVAnim mUVAnim;
        public UVAnim UVAnim
        {
            get { return mUVAnim; }
            set
            {
                mUVAnim = value;
            }
        }

        Guid mUVAnimId = Guid.Empty;
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid UVAnimId
        {
            get { return mUVAnimId; }
            set
            {
                if (mUVAnimId == value)
                    return;

                mUVAnimId = value;
                UVAnim anim = null;
                if (value != Guid.Empty)
                {
                    anim = UISystem.UVAnimMgr.Instance.Find(mUVAnimId, true);
                }

                if (anim != UVAnim)
                    UVAnim = anim;
            }
        }

        bool mReturn = false;
        public bool Return
        {
            get { return mReturn; }
            set
            {
                mReturn = value;
            }
        }

        public float ScaleX
        {
            get
            {
                if (UVAnim == null)
                    return 1;
                if (UVAnim.FirstFrameWidth == 0 || Width == 0)
                    return 1;
                return (float)Width / (float)UVAnim.FirstFrameWidth;
            }
        }

        public float ScaleY
        {
            get
            {
                if (UVAnim == null)
                    return 1;
                if (UVAnim.FirstFrameHeight == 0 || Height == 0)
                    return 1;
                return (float)Height / (float)UVAnim.FirstFrameHeight;
            }
        }

        public delegate void FImageClick(ImgObj imgObj);
        public event FImageClick ImageClick;
        public void ImageClickEvent()
        {
            if (ImageClick != null)
                ImageClick(this);
        }

    }

    public class ImgObjFactory : FragmentObjFactory
    {
        override public string GetName() { return "img"; }
        override public FragmentObj CreateFragmentObj(Document parent)
        {
            var f = new ImgObj();
            f.Doc = parent;
            if (parent != null)
            {
            }
            return f;
        }
    };
}
