using System;
using System.Collections.Generic;
using CSUtility.Support;

namespace UISystem.RichText
{
    public class FragmentObj : BaseObj
    {
        virtual public bool OnLoad(XmlNode pXml)
        {
            return true;
        }
        virtual public bool OnSave(XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            return true;
        }
        override public void Draw(UIRenderPipe pipe, int zOrder)
        {

        }

		// 根据x,y坐标，计算出光标的显示位置，和光标在字符串中的位置
        override public bool PointCheck(int x, int y, ref int outCursorX, ref int outCursorY, ref int outCursorHeight, ref int outCurPos, ref FragmentObj pObj)
        {
            int l = PenX;
            int r = l + Width;
            int t = PenY - Height;
            int b = PenY;
            outCursorX = r;
            outCurPos = 1;

            if (x >= l && x <= r && y >= t && y <= b)
            {
                int iRight = x - l;
                if (iRight > Width / 2)
                {
                    outCursorX = r;
                    outCurPos = 1;
                }
                else
                {
                    outCursorX = l;
                    outCurPos = 0;
                }

                pObj = this;
                return true;
            }

            return false;
        }

		// 根据光标在字符中的位置，计算出光标的显示位置
        virtual public void CalcCursorPos(int pos, ref int outCursorX, ref int outCursorY)
        {
            int iLocalPos = pos;
            if (iLocalPos < 0)
                return;

            if (iLocalPos > 0)
                outCursorX = PenX + Width;
            else
                outCursorX = PenX;
            outCursorY = mLine.PenY + Line.MaxBottomLine - Line.Height;
        }

        virtual public bool PointWinEnterObj(int x, int y)
        {
            int l = PenX;
            int r = l + Width;
            int t = PenY - Height;
            int b = PenY;
            if (x >= l && x <= r && y >= t && y <= b)
            {
                return true;
            }
            return false;
        }

//         virtual public bool PointWinLeaveObj(int x, int y)
//         {
//             int l = PenX;
//             int r = l + Width;
//             int t = PenY - Height;
//             int b = PenY;
//             if (x >= l && x <= r && y >= t && y <= b)
//             {
//                 return false;
//             }
//             return true;
//         }

        string mType = "";					// 片段的类型
        public string Type
        {
            get { return mType; }
            set
            {
                mType = value;
            }
        }

		string mContent = "";					// 片段的描述文本
        public virtual string Content
        {
            get { return mContent; }
            set
            {
                mContent = value;
            }
        }
        
        Document    mDoc;				
        public Document Doc
        {
            get {return mDoc;}
            set
            {
                mDoc = value;
            }
        }

		LineObj		mLine;					
        public LineObj Line
        {
            get { return mLine; }
            set
            {
                mLine = value;
            }
        }

        int mIndex = 0;
        public int Index
        {
            get { return mIndex; }
            set
            {
                mIndex = value;
            }
        }

        Object mTag = null;
        public Object Tag
        {
            get { return mTag; }
            set
            {
                mTag = value;
            }
        }
    }


    public abstract class FragmentObjFactory
    {
        ~FragmentObjFactory()
        {
        }

        public abstract string GetName();
        public abstract FragmentObj CreateFragmentObj(Document parent);


    }

    public class FragmentObjFactoryMgr
    {
        FragmentObjFactoryMgr()
        {
            AddFragmentFactory(new TextObjFactory());
            AddFragmentFactory(new ImgObjFactory());
            AddFragmentFactory(new HyperLinkObjFactory());
        }
        ~FragmentObjFactoryMgr()
        {
            mFragmentObjFactories.Clear();
        }

        static FragmentObjFactoryMgr smMgr = new FragmentObjFactoryMgr();
        public static FragmentObjFactoryMgr GetInstance()
        {
            return smMgr;
        }

        Dictionary<string, FragmentObjFactory> mFragmentObjFactories = new Dictionary<string, FragmentObjFactory>();

        public void AddFragmentFactory(FragmentObjFactory f)
        {
            mFragmentObjFactories.Add(f.GetName(), f);
        }

        public FragmentObj CreateFragmentObj(string typeName, Document parent)
        {           
            FragmentObjFactory f;
            if (mFragmentObjFactories.TryGetValue(typeName.ToLower(), out f) == true)
            {
                if (f != null)
                {
                    var obj = f.CreateFragmentObj(parent);
                    obj.Doc = parent;
                    return obj;
                }
            }
            return null;
        }

        public FragmentObj CreateFragmentObj(XmlNode node, Document parent)
        {
            XmlAttrib attr = node.FindAttrib("Type");
            if (attr!=null)
            {
                FragmentObj obj = CreateFragmentObj(attr.Value, parent);
                obj.OnLoad(node);
                return obj;
            }

            return null;
        }

    }

}
