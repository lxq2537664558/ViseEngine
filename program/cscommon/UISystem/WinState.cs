
/* 项目“Client.Windows”的未合并的更改
在此之前:
using System;
using System.Collections.Generic;
在此之后:
using System;
*/
using System;
using System.
/* 项目“Client.Windows”的未合并的更改
在此之前:
using System.Text;

using System.Drawing.Design;
在此之后:
using System.Drawing.Design;
*/
ComponentModel;
/* 项目“Client.Windows”的未合并的更改
在此之前:
using System.Windows.Forms;
using System.Windows.Forms.Design;
在此之后:
using System.Windows.Forms.Design;
*/


namespace UISystem
{

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class WinState
    {
        WinBase mHostWin;
        [Browsable(false)]
        public WinBase HostWin
        {
            get { return mHostWin; }
        }

        public WinState()
        {

        }

        public WinState(WinBase win)
        {
            mHostWin = win;

            RegisterDefaultValue();
        }

        protected DefaultValueTemplate mDefaultValueTemplate = new DefaultValueTemplate();
        protected virtual void RegisterDefaultValue()
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
            {
                mDefaultValueTemplate.RegisterDefaultValue(property.Name, property.GetValue(this));
            }
        }

        //string mText = "";
        //[Category("属性")]
        //public string Text
        //{
        //    get { return mText; }
        //    set
        //    {
        //        mText = value;
        //    }
        //}

        [Browsable(false)]
        public CCore.Graphics.Texture Texture
        {
            get 
            {
                if (mUVAnim == null)
                    return null;
                return mUVAnim.TextureObject; 
            }
            set
            {
                if (mUVAnim != null)
                    mUVAnim.TextureObject = value;
            }
        }
        [Browsable(false)]
        public CCore.Material.Material Material
        {
            get 
            {
                if (mUVAnim == null)
                    return null;
                return mUVAnim.MaterialObject; 
            }
        }
        //[System.ComponentModel.EditorAttribute(typeof(UVAnimEditor), typeof(System.Drawing.Design.UITypeEditor))]
        //[Browsable(false)]
        //public Guid UIAnimId
        //{
        //    get 
        //    {
        //        if (mUVAnim == null)
        //            return new Guid();
        //        return mUVAnim.Id; 
        //    }
        //    set 
        //    {
        //        this.UVAnim = UVAnimMgr.Instance.Find(value, true);
        //    }
        //}
        UVAnim mUVAnim;
        //[ReadOnly(false)]
        [Browsable(false)]
        public UVAnim UVAnim
        {
            get { return mUVAnim; }
            set
            {
                mUVAnim = value;
                if (mHostWin != null)
                {
                    //WinMSG msg = new WinMSG();
                    //msg.message = (UInt32)MidLayer.SysMessage.VWM_USER + (UInt32)MsgDefineUI.OnSetUVAnim;
                    //mHostWin.Send2Me(ref msg);

                    var uvMsg = new CCore.MsgProc.Behavior.UVAnimSetted();
                    UISystem.Message.RoutedEventArgs args = new UISystem.Message.RoutedEventArgs();
                    args.Origion = mHostWin;
                    args.Source = mHostWin;
                    mHostWin.Send2Me(uvMsg, args);
                }

                if (mUVAnim != null && mUVAnim.Id != UVAnimId)
                {
                    mUVAnimId = mUVAnim.Id;
                }
                else if (mUVAnim == null)
                {
                    mUVAnimId = Guid.Empty;
                }
            }
        }
        Guid mUVAnimId = Guid.Empty;
        [Category("属性")]
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
        UI.ContentAlignment mTextAlign = UI.ContentAlignment.MiddleCenter;
        [Category("属性")]
        [Browsable(false)]
        public UI.ContentAlignment TextAlign
        {
            get { return mTextAlign; }
            set { mTextAlign = value; }
        }
        CSUtility.Support.Point mTextOffset;
        [Category("属性")]
        [Browsable(false)]
        public CSUtility.Support.Point TextOffset
        {
            get { return mTextOffset; }
            set { mTextOffset = value; }
        }

        public void ResetUVAnimFramePlay()
        {
            if(UVAnim != null)
                UVAnim.ResetUVAnimFramePlay();
        }
        
        static public CSUtility.Performance.PerfCounter CounterLayout = new CSUtility.Performance.PerfCounter("UI.Layout");
        public virtual void Draw(UIRenderPipe pipe, int zOrder, WinBase pWin, ref SlimDX.Vector4 backColor, ref SlimDX.Matrix parentMatrix )
        {
            IRender pRender = IRender.GetInstance();
		    pRender.SetClipRect( pipe, pWin.ClipRect );
            //pRender.SetClipRect( pWin.AbsRect );

            WinRoot root = pWin.GetRoot() as WinRoot;
            if (root != null && UVAnim != null && UVAnim.Frames.Count > 0)
            {
                UVAnim.CheckAndAutoRefreshFromTemplateUVAnim();

                UVFrame frame = UVAnim.GetUVFrame(CCore.Engine.Instance.GetFrameSecondTimeFloatByUVAnim());
                CSUtility.Support.Rectangle dst = pWin.AbsRect;
                if (Material != null && Material.IsMaterialValid())
                {
                    if (frame.Scale9DrawRectangles.Count > 1)
                    {
                        foreach (var scaleInfo in frame.Scale9DrawRectangles)
                        {
                            var refRect = scaleInfo.GetDrawRect(dst);
                            pRender.DrawImage(pipe, zOrder, root.Width, root.Height, 
                                              Texture,
                                              ref backColor,
                                              ref refRect,
                                              ref scaleInfo.mDrawUVRect, ref parentMatrix, pWin.Opacity * UVAnim.Opacity, Material);
                        }
                    }
                    else
                    {
                        pRender.DrawImage(pipe, zOrder, root.Width, root.Height, Texture, ref backColor,
                        ref dst,
                        ref frame.mUVRect, ref parentMatrix, pWin.Opacity * UVAnim.Opacity, Material);
                    }
                }

                if (UVAnim.CurrentState == UVAnim.enState.FadeOut)
                {
                    // 渐隐处理
                    UVAnim.FadeOutProcess();
                }
            }
            else
            {
                if (backColor.W != 0)
                {
                    pRender.FillRectangle(pipe, zOrder, root.Width, root.Height, pWin.AbsRect, ref backColor, ref parentMatrix);
                }
            }

        }

        public void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            if (mUVAnim != null)
            {
                pXml.AddAttrib("UVAnim", mUVAnim.Id.ToString());
            }
            //if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Text"))
            //    pXml.AddAttrib("Text", Text);
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TextAlign"))
                pXml.AddAttrib("TextAlign", ((int)TextAlign).ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TextOffset"))
                pXml.AddAttrib("TextOffset", System.String.Format("{0},{1}", TextOffset.X, TextOffset.Y));
        }
        public void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            //using namespace MidLayer::Support;
            CSUtility.Support.XmlAttrib attr = pXml.FindAttrib("UVAnim");
            if (attr != null)
            {
                Guid id = CSUtility.Support.IHelper.GuidTryParse(attr.Value);
                UVAnim = UVAnimMgr.Instance.Find(id, true);
            }
            //attr = pXml.FindAttrib("Text");
            //if(attr!=null)
            //    Text = attr.Value;
            attr = pXml.FindAttrib("TextAlign");
            if (attr != null)
                TextAlign = (UI.ContentAlignment)System.Convert.ToInt32(attr.Value);
            attr = pXml.FindAttrib("TextOffset");
            if(attr!=null)
            {
                string[] substrs = attr.Value.Split( ',' );//可能哪天俄罗斯系统支持的时候，要用Culture::Textinfo->ListSeparator[0]来做分隔了。
                if(substrs!=null&&substrs.Length==2)
                {
                    mTextOffset.X = System.Convert.ToInt32(substrs[0]);
                    mTextOffset.Y = System.Convert.ToInt32(substrs[1]);
                }
            }
        }
        public CSUtility.Support.Size GetImageSize()
        {
            if (UVAnim == null)
                return CSUtility.Support.Size.Empty;

            if (UVAnim.Frames.Count <= 0)
                return CSUtility.Support.Size.Empty;

            if(UVAnim.TextureObject == null)
                return CSUtility.Support.Size.Empty;

            var width = (int)(UVAnim.Frames[0].SizeU * UVAnim.TextureObject.Width);
            var height = (int)(UVAnim.Frames[0].SizeV * UVAnim.TextureObject.Height);

            return new CSUtility.Support.Size(width, height);
        }

        public void CopyFrom(WinState state)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
            {
                property.SetValue(this, property.GetValue(state));
            }
        }
    }
}
