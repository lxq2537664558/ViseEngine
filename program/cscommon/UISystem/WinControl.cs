using System.ComponentModel;

namespace UISystem
{
    public class WinControl : WinBase
    {
        protected int mTabIndex = 0;        
        [Category("行为")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public int TabIndex
        {
			get { return mTabIndex; }
			set { mTabIndex = value; }
		}
        //protected string mText = "";
        //[Category("外观")]
        //public virtual string Text
        //{
        //    get { return mText; }
        //    set { mText = value; }
        //}
        protected UI.ContentAlignment mTextAlign = UI.ContentAlignment.MiddleCenter;
        [Category("外观")]
		public virtual UI.ContentAlignment TextAlign
        {
			get { return mTextAlign; }
			set { mTextAlign = value; }
		}
        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
		    base.OnSave(pXml,holder);

            //if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Text"))
            //    pXml.AddAttrib("Text", Text);
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TextAlign"))
                pXml.AddAttrib("TextAlign", TextAlign.ToString());
        }
        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
		    base.OnLoad(pXml);

            CSUtility.Support.XmlAttrib attr = null;
            //CSUtility.Support.IXmlAttrib attr = pXml.FindAttrib("Text");
            //if(attr!=null)
            //    Text = attr.Value;
		    attr = pXml.FindAttrib( "TextAlign" );
		    if(attr!=null)
		    {
			    if(attr.Value=="TopLeft")
				    TextAlign = UI.ContentAlignment.TopLeft;
			    else if(attr.Value=="TopCenter")
				    TextAlign = UI.ContentAlignment.TopCenter;
			    else if(attr.Value=="TopRight")
				    TextAlign = UI.ContentAlignment.TopRight;
			    else if(attr.Value=="MiddleLeft")
				    TextAlign = UI.ContentAlignment.MiddleLeft;
			    else if(attr.Value=="MiddleCenter")
				    TextAlign = UI.ContentAlignment.MiddleCenter;
			    else if(attr.Value=="MiddleRight")
				    TextAlign = UI.ContentAlignment.MiddleRight;
			    else if(attr.Value=="BottomLeft")
				    TextAlign = UI.ContentAlignment.BottomLeft;
			    else if(attr.Value=="MiddleCenter")
				    TextAlign = UI.ContentAlignment.BottomCenter;
			    else if(attr.Value=="BottomRight")
				    TextAlign = UI.ContentAlignment.BottomRight;
		    }
        }
    }
}
