using System;
using System.Collections.Generic;
//using System.Linq;
using System.ComponentModel;
using CSUtility.Support;

namespace UISystem.RichText
{

    public class Document : BaseObj, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public Document(WinBase parent)
        {
            ParentCtrl = parent;
        }
        ~Document()
        {
            if (mFragmentObjs != null)
                mFragmentObjs.Clear();
            if (mOrginFragmentObjs != null)
                mOrginFragmentObjs.Clear();
            if (mLineObjs != null)
                mLineObjs.Clear();
        }

        //  <D>
        //      <F Type="Text" FontSize="42" Content="显示文本的内容">
        //          <P>
        //              <0 BLColor="-5888" BRColor="1367342848" TLColor="-65490" TRColor="-14614273" OutlineType="Line" OutlineThickness="1"/>
        //          </P>
        //      </F>
        //      <F Type="Img" Width="10" Height="20" MaxBottomLine="5" Content="UI/Test.tga">
        //      </F>
        //  </D>
        public bool OnLoad(XmlNode pXml)
        {
            XmlAttrib attr = null;

            attr = pXml.FindAttrib("FontName");
            if (attr != null)
                mFontName = attr.Value;

            attr = pXml.FindAttrib("FontSize");
            if (attr != null)
                mFontSize = Convert.ToInt32(attr.Value);

            attr = pXml.FindAttrib("TextAlignment");
            if (attr != null)
                mTextAlignment = (UI.TextAlignment)System.Enum.Parse(typeof(UI.TextAlignment), attr.Value);

            attr = pXml.FindAttrib("TextWrapping");
            if (attr != null)
                mTextWrapping = (UI.TextWrapping)System.Enum.Parse(typeof(UI.TextWrapping), attr.Value);

            attr = pXml.FindAttrib("LineHeight");
            if (attr != null)
                mLineHeight = Convert.ToInt32(attr.Value);

            CSUtility.Support.XmlNode pParamsNode = pXml.FindNode("FontRenderParams");
            if (pParamsNode != null)
            {
                mFontRenderParams.Cleanup();
                List<CSUtility.Support.XmlNode> lists = pParamsNode.GetNodes();
                foreach (CSUtility.Support.XmlNode i in lists)
                {
                    var newParam = mFontRenderParams.AddParam();
                    newParam.OnLoad(i);
                }
            }

            //attr = pXml.FindAttrib("Text");
            //if (attr != null)
            //    Text = attr.Value;

            var textNode = pXml.FindNode("Text");
            if (textNode != null)
            {
                var textNodes = textNode.GetNodes();
                foreach (var node in textNodes)
                {
                    FragmentObj fragment = FragmentObjFactoryMgr.GetInstance().CreateFragmentObj(node, this);
                    if (fragment != null)
                    {
                        fragment.Doc = this;
                        mFragmentObjs.AddLast(fragment);
                        mOrginFragmentObjs.AddLast(fragment);
                    }
                }

                Update();

                XmlHolder.GetXMLStringFromNode(ref mText, textNode);
                OnPropertyChanged("Text");
            }

            return true;
        }

        public bool OnSave(XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            pXml.AddAttrib("FontName", FontName);
            pXml.AddAttrib("FontSize", FontSize.ToString());
            pXml.AddAttrib("TextAlignment", System.Convert.ToString(TextAlignment));
            pXml.AddAttrib("TextWrapping", System.Convert.ToString(TextWrapping));
            pXml.AddAttrib("LineHeight", LineHeight.ToString());

            CSUtility.Support.XmlNode pParamsNode = pXml.AddNode("FontRenderParams", "FontRenderParams",holder);
            //int iIndex = 0;
            //foreach (var fontParam in FontRenderParams)
            for (int iIndex = 0; iIndex < FontRenderParams.GetParamCount();iIndex++)
            {
                CSUtility.Support.XmlNode pNode = pParamsNode.AddNode(System.Convert.ToString(iIndex), "", holder);
                var fontParam = FontRenderParams.GetParam(iIndex);
                fontParam.OnSave(pNode);
            }

            //pXml.AddAttrib("Text", Text);

            var textNode = pXml.AddNode("Text", "",holder);
            foreach (var obj in mFragmentObjs)
            {
                var objNode = textNode.AddNode("F", "",holder);
                obj.OnSave(objNode,holder);
            }          

            return true;
        }

        public String GetFormatString()
        {
            XmlHolder holder = XmlHolder.NewXMLHolder("D", "");

            foreach (var obj in mFragmentObjs)
            {
                var objNode = holder.RootNode.AddNode("F","",holder);
                obj.OnSave(objNode,holder);
            }

            string ret = "";
            XmlHolder.GetXMLString(ref ret, holder);

            return ret;
        }

        // 解析字符，将文本分成一段段
        public void UpdateFragmentsByText()
        {
            // 解析的XML文本示例:
            //<D>
            //    <F Type="Text" FontSize="42" Content="显示文本的内容">
            //        <P>
            //            <0 BLColor="-5888" BRColor="1367342848" TLColor="-65490" TRColor="-14614273" OutlineType="Line" OutlineThickness="1"/>
            //        </P>
            //    </F>
            //    <F Type="Img" Width="10" Height="20" MaxBottomLine="5" Content="UI/Test.tga">
            //    </F>
            //</D>    

            if (string.IsNullOrEmpty(Text))
                return;

            XmlHolder xmlHolder = XmlHolder.ParseXML(Text);
            if (xmlHolder == null)
                return;

            List<XmlNode> xmlNodes = xmlHolder.RootNode.FindNodes("F");
            if (xmlNodes == null)
                return;

            if (mFragmentObjs != null)
                mFragmentObjs.Clear();
            if (mOrginFragmentObjs != null)
                mOrginFragmentObjs.Clear();

            foreach (var node in xmlNodes)
            {
                FragmentObj fragment = FragmentObjFactoryMgr.GetInstance().CreateFragmentObj(node, this);
                if (fragment != null)
                {
                    fragment.Doc = this;
                    mFragmentObjs.AddLast(fragment);
                    mOrginFragmentObjs.AddLast(fragment);
                }
            }
        }

        // 从指定对象开始，拆分可以拆分的FragmentObj
        public LinkedListNode<FragmentObj> SplitFragmentsByWidth(FragmentObj startObj)
        {
            if (mWrapInSmallWidth == false && Width <= FontSize)
            {
                return mFragmentObjs.Find(startObj);
            }

            int iCurLineWidth = 0;  // 当前行的文本宽度
            LinkedList<FragmentObj> splitFragmentObjs = new LinkedList<FragmentObj>();
            LinkedList<FragmentObj> unchangeFragmentObjs = new LinkedList<FragmentObj>();
           
            var startNode = mFragmentObjs.First;
            if (startObj != null)
            {
                var t = mFragmentObjs.Find(startObj);
                if (t != null)
                {
                    startNode = t;
                    foreach (var n in mFragmentObjs)
                    {
                        if(n == startNode.Value)
                            break;

                        unchangeFragmentObjs.AddLast(n);
                    }
                }
            }

            do
            {
                var oldObj = startNode.Value;

                if (iCurLineWidth + oldObj.Width > Width) // 如果当前obj超过了行宽，则分割
                {
                    if (oldObj.Type.CompareTo("text") == 0 || oldObj.Type.CompareTo("Text") == 0)
                    {
                        TextObj pTextObj = oldObj as TextObj;
                        List<string> oTexts = new List<string>();

                        int iSplitWidth = Width - iCurLineWidth;
                        if (iSplitWidth == Width || iSplitWidth <= pTextObj.FontSize) // 如果本行空余宽度不够一个字符，则另起一行，将当前字串按行宽分割出多个字符串
                        {
                            pTextObj.SplitTextInWidth(Width, oTexts);
                        }
                        else // 按空余宽度分割为2个字符串
                        {
                            //pTextObj.SplitTextInHalf(iSplitWidth, oTexts);

                            List<string> oTwolineTexts = new List<string>();
                            List<string> oMultilineTexts = new List<string>();
                            pTextObj.SplitTextInHalf(iSplitWidth, oTwolineTexts);
                            if (oTwolineTexts.Count > 1)
                            {
                                IRender.GetInstance().SplitTextInWidth(pTextObj.AbsFontName, oTwolineTexts[1],
                                    pTextObj.FontSize, pTextObj.GetSuitableFontRenderParams(), Width, oMultilineTexts);
                                oTexts.Add(oTwolineTexts[0]);
                                oTexts.AddRange(oMultilineTexts);
                            }
                            else
                                oTexts.AddRange(oTwolineTexts);
                        }

                        for(int i = 0; i < oTexts.Count; ++i)
                        {
                            var newObj = pTextObj.Clone(oTexts[i]);
                            // 分割出的最后一个TextObj设置是否换行
                            if (i == oTexts.Count - 1)
                            {
                                newObj.Return = pTextObj.Return;
                            }
                            splitFragmentObjs.AddLast(newObj);
                        }

                        iCurLineWidth = splitFragmentObjs.Last.Value.Width;
                    }
                    else
                    {
                        splitFragmentObjs.AddLast(oldObj);
                        iCurLineWidth = oldObj.Width;
                    }
                }
                else
                {
                    splitFragmentObjs.AddLast(oldObj);
                    iCurLineWidth += oldObj.Width;
                }

                startNode = startNode.Next;
            }
            while (startNode != null);
                
            mFragmentObjs.Clear();
            mFragmentObjs = unchangeFragmentObjs;
            foreach (var obj in splitFragmentObjs)
            {
                mFragmentObjs.AddLast(obj);
            }

            return splitFragmentObjs.First;
        }        

        // 拆分可以拆分的FragmentObj
        public void SplitFragmentsByWidth()
        {          
            if (mWrapInSmallWidth == false && Width <= FontSize)
                return;

            if(mFragmentObjs.Count>0)
                SplitFragmentsByWidth(mFragmentObjs.First.Value);
        }

        // 创建并初始化LineObj
        public void UpdateLineObjs()
        {
            lock (mLineObjs)
            {
                mLineObjs.Clear();
                LineObj curLine = CreateLineObj();

                if (mWrapInSmallWidth == false && Width <= FontSize)
                {
                    foreach (var obj in mFragmentObjs)
                    {
                        curLine.AddFragmentObj(obj);
                    }
                }
                else
                {
                    int iCurLineWidth = 0;
                    foreach (var obj in mFragmentObjs)
                    {
                        if (mTextWrapping == UI.TextWrapping.NoWrap) // 如果NoWrap，则只有一行
                        {
                            curLine.AddFragmentObj(obj);
                        }
                        else if (iCurLineWidth + obj.Width > Width) // 如果当前obj超过了行宽，则另起一行
                        {
                            curLine = CreateLineObj();
                            curLine.AddFragmentObj(obj);
                            iCurLineWidth = obj.Width;
                        }
                        else
                        {
                            curLine.AddFragmentObj(obj);
                            iCurLineWidth += obj.Width;
                        }

                        // 强制换行
                        var textObj = obj as TextObj;
                        if (textObj != null)
                        {
                            if (textObj.Return == true)
                            {
                                curLine = CreateLineObj();
                                iCurLineWidth = 0;
                            }
                        }
                        //
                        var imagObj = obj as ImgObj;
                        if (imagObj != null)
                        {
                            if (imagObj.Return)
                            {
                                curLine = CreateLineObj();
                                iCurLineWidth = 0;
                            }
                        }
                    }
                }

                // 计算Line的宽高等属性
                TextHeight = 0;
                TextWidth = 0;
                foreach (var line in mLineObjs)
                {
                    line.UpdateSize();
                    // 如果指定了固定行高，并且不小于计算出来的文本行高，则使用固定行高
                    if (LineHeight > line.Height)
                    {
                        line.Height = LineHeight;
                    }
                    TextHeight += line.Height;
                    TextWidth = System.Math.Max(TextWidth, line.Width);
                }

                // 计算Line的画笔属性
                int curPenX = PenX;
                int curPenY = PenY;
                LineObj preLine = null;
                foreach (var line in mLineObjs)
                {
                    if (preLine != null)
                    {
                        curPenY += preLine.MaxBottomLine;
                    }
                    curPenY += line.Height - line.MaxBottomLine;
                    line.UpdatePen(curPenX, curPenY);

                    preLine = line;
                }
            }
        }

        public void UpdateCursorPos()
        {
            PointClick(mCursorX, mCursorY);
            //var obj = GetFragment(mCursorPos);
            //obj.CalcCursorPos(mCursorPos, ref mCursorX, ref mCursorY);
        }

	    public void Update()
	    {
            try
            {
                if (Width == 0)
                    return;

                if (mTextWrapping != UI.TextWrapping.NoWrap)
                {
                    SplitFragmentsByWidth();
                }
                UpdateLineObjs();
                UpdateCursorPos();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        public void Update(int penX, int penY, int W, int H)
        {
            //if (Height == H && Width == W)
            //    return;

            //if (Text.Length == 0)
            //    return;

            Height  = H;

            try
            {
                if (Width != W)
                {
                    Width = W;
                    PenX = penX;
                    PenY = penY;

                    if (mTextWrapping != UI.TextWrapping.NoWrap)
                    {
                        SplitFragmentsByWidth();
                    }
                    UpdateLineObjs();
                    UpdateCursorPos();
                }
                else if (PenX != penX || PenY != penY)
                {
                    PenX = penX;
                    PenY = penY;
                    UpdateLineObjs();
                    UpdateCursorPos();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        override public void Draw(UIRenderPipe pipe, int zOrder)
	    {
            lock (mLineObjs)
            {
                for (var i = 0; i < mLineObjs.Count; i++ )
                {
                    LineObj obj = null;
                    try
                    {
                        obj = mLineObjs[i];
                    }
                    catch (System.Exception)
                    {
                    	continue;
                    }
                    if(obj!=null)
                        obj.Draw(pipe, zOrder);
                }
            }
	    }

        override public bool PointCheck(int x, int y, ref int outCursorX, ref int outCursorY, ref int outCursorHeight, ref int outCurPos, ref FragmentObj pObj)
        {
            if (mLineObjs.Count == 0)
            {
                outCursorX = PenX;
                outCursorY = PenY;
                outCursorHeight = mFontSize;
                outCurPos = 0;
                pObj = null;
                return true;
            }

            int left = PenX;
            int right = PenX + TextWidth;
            int top = PenY;
            int bottom = PenY + TextHeight;

            if (x < left)
                x = left;
            if (x > right)
                x = right;
            if (y < top)
                y = top;
            if (y > bottom)
                y = bottom;

            for (var i = 0; i < mLineObjs.Count; i++ )
            {
                LineObj line = null;
                try
                {
                    line = mLineObjs[i];
                }
                catch (System.Exception)
                {
                	continue;
                }

                if (line.PointCheck(x, y, ref outCursorX, ref outCursorY, ref outCursorHeight, ref outCurPos, ref pObj))
                {
                    break;
                }
            }

            if (outCursorHeight == 0)
                outCursorHeight = mFontSize;

            return true;
        }

        public delegate void FCursorPosChange();
        public event FCursorPosChange CursorPosChange;

        public void PointClick(int x, int y)
        {
            if (FragmentObjs.Count == 0)
            {
                mCurObj = null;
                mCursorPos = 0;
                mCursorX = PenX;
                mCursorY = PenY;
                mCursorHeight = mFontSize;

                if (CursorPosChange != null)
                    CursorPosChange();

                return;
            }

            mCurObj = FragmentObjs.Last.Value;
            PointCheck(x, y, ref mCursorX, ref mCursorY, ref mCursorHeight, ref mCursorPos, ref mCurObj);
            if (mLineHeight > mCursorHeight)
                mCursorHeight = mLineHeight;

            if (CursorPosChange != null)
                CursorPosChange();
        }

        public void PointMouseClick(int x, int y)
        {
            foreach (var line in mLineObjs)
            {
                if (line.WinMouseClick(x, y))
                {
                    break;
                }
            }
        }

        public void PointMouseEnter(int x, int y)
        {
            foreach (var line in mLineObjs)
            {
                if (line.WinMouseEnter(x, y))
                {
                    break;
                }
            }
        }

        public void PointMouseLeave(int x, int y)
        {
            try
            {
                foreach (var obj in mFragmentObjs)
                {
                    var textObj = obj as UISystem.RichText.HyperLinkObj;
                    if (textObj != null)
                    {
                        //var textobj = obj as UISystem.RichText.TextObj;
                        if (!textObj.Hyperlink)
                            break;
                        if (textObj.IsMouseEnter)
                            textObj.HyperlinkWinMouseLeaveEvent();
                    }
                }
            }
            catch (System.Exception)
            {
            	
            }
            
        }

        public void PointMouseMove(int x, int y)
        {
            //foreach (var line in mLineObjs)
            for (var i = 0; i < mLineObjs.Count; i++ )
            {
                LineObj line = null;
                try
                {
                    line = mLineObjs[i];
                }
                catch (System.Exception)
                {
                	continue;
                }
                if (line.WinMouseMove(x, y))
                {
                    break;
                }
            }
        }

        void Update(FragmentObj startObj, int iNewPos)
        {
            if (startObj == null)
                return;

            var splitNode = SplitFragmentsByWidth(startObj);
            UpdateLineObjs();

            // 重新计算CurObj和光标位置 
            while (iNewPos > splitNode.Value.Length)
            {
                iNewPos -= splitNode.Value.Length;

                // 如果下一个obj是空（到了Doc的末尾）， 或不是textObj，则截止
                if (splitNode.Next == null || splitNode.Next.Value.Type != "text")
                {
                    iNewPos = splitNode.Value.Length;
                    break;
                }

                splitNode = splitNode.Next;
            }
            mCurObj = splitNode.Value;
            mCursorPos = (iNewPos < splitNode.Value.Length) ? iNewPos : splitNode.Value.Length;
            mCurObj.CalcCursorPos(mCursorPos, ref mCursorX, ref mCursorY);
        }

        public void InsertString(string s)
        {
            if (mCurObj != null && s.Length>0)
            {
                if (mCurObj.Type == "text")
                {
                    int iCurPos = mCursorPos;
                    if (iCurPos > mCurObj.Content.Length)
                        iCurPos = mCurObj.Content.Length;

                    mCurObj.Content = mCurObj.Content.Insert(iCurPos, s);
                    Update(mCurObj, iCurPos + s.Length);                    
                }
                else
                {
                    var newText = FragmentObjFactoryMgr.GetInstance().CreateFragmentObj("text", this);
                    newText.Content = s;
                    var curNode = mFragmentObjs.Find(mCurObj);
                    if (mCursorPos == 0)
                    {
                        mFragmentObjs.AddBefore(curNode, newText);
                    }
                    else
                    {
                        mFragmentObjs.AddAfter(curNode, newText);
                    }

                    var splitNode = SplitFragmentsByWidth(curNode.Value);
                    UpdateLineObjs();

                    Update(mCurObj, s.Length);
                }
            }
            else
            {
                var newText = FragmentObjFactoryMgr.GetInstance().CreateFragmentObj("text", this) as TextObj;
                newText.Content = s;
                newText.FontSize = mFontSize;
                newText.FontName = mFontName;

                mFragmentObjs.AddLast(newText);
                Update(newText, s.Length);
            }

            if (CursorPosChange != null)
                CursorPosChange();


            // Debug
            String temp = GetFormatString();
        }

        public void InsertObj(FragmentObj obj)
        {
            if (CursorPosChange != null)
                CursorPosChange();
        }

        void RemoveObj(LinkedListNode<FragmentObj> removeNode)
        {
            // 重新计算CurObj CursorPos
            if (removeNode.Next != null)
            {
                if (removeNode.Next.Value.Type == "text")
                {
                    mCursorPos = 0;
                    mCurObj = removeNode.Next.Value;
                }
                else
                {
                    if (removeNode.Previous != null && removeNode.Previous.Value.Type == "text")
                    {
                        mCurObj = removeNode.Previous.Value;
                        mCursorPos = mCurObj.Length;
                    }
                    else
                    {
                        mCursorPos = 0;
                        mCurObj = removeNode.Next.Value;
                    }
                }
            }
            else if (removeNode.Previous != null)
            {
                mCurObj = removeNode.Previous.Value;
                mCursorPos = mCurObj.Length;
            }
            else
            {
                mCurObj = null;
                mCursorPos = 0;
            }

            mFragmentObjs.Remove(removeNode);
        }

        public void Delete(int count)
        {
            if (mCurObj != null)
            {
                var curNode = mFragmentObjs.Find(mCurObj);
                // 如果光标在末尾，则操作下一个FragmentObj
                if (mCursorPos == mCurObj.Length)
                {
                    curNode = curNode.Next;
                    if (curNode == null)
                        return;
                    mCursorPos = 0;
                }


                if (curNode.Value.Type == "text")
                {
                    if (mCursorPos >= curNode.Value.Length)
                        mCursorPos = curNode.Value.Length - 1;
                    curNode.Value.Content = curNode.Value.Content.Remove(mCursorPos, count);

                    if (curNode.Value.Content == "")
                    {
                        RemoveObj(curNode);
                    }
                }
                else
                {
                    RemoveObj(curNode);
                }

                //curNode.Value.CalcCursorPos(mCursorPos, ref mCursorX, ref mCursorY);
                //Update(mCurObj, mCursorPos);
                ////Update();

                //if (CursorPosChange != null)
                //    CursorPosChange();
            }
        }

        public void DeleteBack(int count)
        {
            if (mCurObj != null)
            {
                var curNode = mFragmentObjs.Find(mCurObj);
                // 如果光标在开头，则操作上一个FragmentObj
                if (mCursorPos == 0)
                {
                    curNode = curNode.Previous;
                    if (curNode == null)
                        return;
                    mCurObj = curNode.Value;
                    mCursorPos = curNode.Value.Length;
                }

                if (curNode.Value.Type == "text")
                {
                    curNode.Value.Content = curNode.Value.Content.Remove(mCursorPos - 1, count);
                    mCursorPos--;

                    if (curNode.Value.Content == "")
                    {
                        RemoveObj(curNode);
                    }
                }
                else
                {
                    RemoveObj(curNode);
                }

                curNode.Value.CalcCursorPos(mCursorPos, ref mCursorX, ref mCursorY);
                //Update(mCurObj, mCursorPos);
                Update();

                if (CursorPosChange != null)
                    CursorPosChange();
            }
        }


        public void DeleteString(int pos, int count)
        {
            if (mCurObj != null)
            {
                if (mCurObj.Type == "text")
                {
                    var textObj = mCurObj as TextObj;
                    int iCurPos = mCursorPos + pos - 1;
                    if (iCurPos >= textObj.Content.Length)
                        iCurPos = textObj.Content.Length;
                    if (iCurPos <= 0)
                        iCurPos = 0;

                    textObj.Content = textObj.Content.Remove(iCurPos, count);
                    mCursorPos--;
                    if (textObj.Content == "")
                    {
                        mFragmentObjs.Remove(textObj);
                        mCursorPos = 0;
                        mCurObj = null;
                    }
                }
            }

            if (CursorPosChange != null)
                CursorPosChange();
        }

        //public FragmentObj GetFragment(int pos)
        //{
        //    if (pos > (int)mText.Length)
        //    {
        //        pos = (int)mText.Length;
        //    }

        //    foreach (var obj in mFragmentObjs)
        //    {
        //        if (pos <= obj.BeginPos + obj.Length)
        //            return obj;
        //    }

        //    return mFragmentObjs.Last();
        //}

        public void RemoveFragmentObj(FragmentObj obj)
        {
            // TODO
        }

        public LineObj CreateLineObj()
        {
            LineObj line = new LineObj();
            line.Doc = this;
            mLineObjs.Add(line);
            return line;
        }

        public int MeasureMaxWidth()
        {
            int maxWidth = 0;
            foreach (var obj in mFragmentObjs)
            {
                maxWidth += obj.Width;
            }
            return maxWidth;
        }

        LinkedList<FragmentObj> mFragmentObjs = new LinkedList<FragmentObj>();								// 片段列表，包括文字、图片、链接等       
        public LinkedList<FragmentObj> FragmentObjs
        {
            get { return mFragmentObjs; }
        }
        LinkedList<FragmentObj> mOrginFragmentObjs = new LinkedList<FragmentObj>();								// 片段列表，包括文字、图片、链接等
        public LinkedList<FragmentObj> OrginFragmentObjs
        {
            get { return mOrginFragmentObjs; }
        }
        List<LineObj> mLineObjs = new List<LineObj>();									// 行信息

        Int32 LineCount
        {
            get
            {
                return mLineObjs.Count;
            }
        }

        string              mText = "";
        public string Text
        {
            get { return mText; }
            set
            {
                if (mText == value)
                    return;

                mText = value;
                UpdateFragmentsByText(); 
                Update();

                OnPropertyChanged("Text");
            }
        }

        string              mFontName = "";
        public string FontName
        {
            get { return mFontName; }
            set
            {
                mFontName = value;
                mFontName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(mFontName);
                Update();
            }
        }

        int mFontSize = 20;
        public int FontSize
        {
            get { return mFontSize; }
            set
            {
                mFontSize = value;
                Update();
            }
        }

        CCore.Font.FontRenderParamList mFontRenderParams = new CCore.Font.FontRenderParamList(true);
        public CCore.Font.FontRenderParamList FontRenderParams
        {
            get { return mFontRenderParams; }
            set
            {
                mFontRenderParams = value;
                Update();
            }
        }

        int mTextHeight = 0;                            // 文本高度，行数*行间距+每行高度
        [Browsable(false)]
        public int TextHeight
        {
            get { return mTextHeight; }
            set
            {
                mTextHeight = value;
            }
        }

        int mTextWidth = 0;                            
        [Browsable(false)]
        public int TextWidth
        {
            get { return mTextWidth; }
            set
            {
                mTextWidth = value;
            }
        }

        int mLineHeight;
        public int LineHeight
        {
            get { return mLineHeight; }
            set
            {
                mLineHeight = value;
                Update();
            }
        }

        UI.TextAlignment mTextAlignment;
        public UI.TextAlignment TextAlignment
        {
            get { return mTextAlignment; }
            set
            {
                mTextAlignment = value;
                Update();
            }
        }

        UI.TextWrapping mTextWrapping = UI.TextWrapping.Wrap;
        public UI.TextWrapping TextWrapping
        {
            get { return mTextWrapping; }
            set
            {
                mTextWrapping = value;
                UpdateFragmentsByText();
                Update();
            }
        }

        bool mWrapInSmallWidth = false;                 // 避免宽度过小时一个字占一行的情况，节约效率
        public bool WrapInSmallWidth
        {
            get { return mWrapInSmallWidth; }
            set
            {
                mWrapInSmallWidth = value;
            }
        }

        public int GetCursorHeight()
        {
            int h = FontSize;
            if (mCurObj != null && mCurObj.Line!=null)
            {
                h = mCurObj.Line.TextHeight;
            }

            return h;
        }

        int mCursorPos = 0;								    // 光标在字符串中的位置
        public int CursorPos
        {
            get { return mCursorPos; }
            set
            {
                mCursorPos = value;
            }
        }
        int mCursorHeight = 0;								    // 光标在字符串中的位置
        public int CursorHeight
        {
            get { return mCursorHeight; }
            set
            {
                mCursorHeight = value;
            }
        }
        int mCursorX;									// 光标的显示位置
        public int CursorX
        {
            get { return mCursorX; }
            set
            {
                mCursorX = value;
            }
        }
        int mCursorY;									// 光标的显示位置
        public int CursorY
        {
            get { return mCursorY; }
            set
            {
                mCursorY = value;
            }
        }
        FragmentObj mCurObj = null;             // 光标所在的当前FragmentObj
        public FragmentObj CurObj
        {
            get { return mCurObj; }
            set
            {
                mCurObj = value;
            }
        }

        WinBase mParentCtrl;
        [Browsable(false)]
        public WinBase ParentCtrl
        {
            get { return mParentCtrl; }
            set
            {
                mParentCtrl = value;
            }
        }

    }
}
