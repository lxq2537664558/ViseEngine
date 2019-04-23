using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace UISystem
{
    public partial class WinBase
    {
        public bool EnableEditorMouseMove = true;
        public bool EditDeleteAble = true;

        public bool CopyFrom(WinBase control, bool copyId = false)
        {
            if (this.GetType() != control.GetType())
                return false;

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(control))
            {
                //if (property.Name == "ChildWindows")
                //    continue;

                property.SetValue(this, property.GetValue(control));
            }

            if (copyId)
                mGuid = control.Id;

            return true;
        }

        public bool CopyFrom(WinBase control, List<string> ignoreProperty, bool copyId = false, bool withChildren = false)
        {
            if (this.GetType() != control.GetType())
                return false;

            if (control.TemplateId != Guid.Empty)
            {
                this.TemplateId = control.TemplateId;

                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(control))
                {
                    if (property.Name == "ChildWindows" ||
                       property.Name == "TemplateId" ||
                       property.Name == "Parent")
                        continue;

                    property.SetValue(this, property.GetValue(control));
                }

                if (copyId)
                    mGuid = control.Id;
                else
                    mCopyedFromId = control.Id;

                // 对象模型拷贝
                if (control is Content.ContentControl)
                {
                    var content = ((Content.ContentControl)control).Content;
                    if (content != null)
                    {
                        var copyedChild = content.GetType().Assembly.CreateInstance(content.GetType().ToString()) as WinBase;
                        copyedChild.CopyFrom(content, new List<string>() { "Parent" }, copyId, withChildren);
                        copyedChild.Parent = this;
                    }
                }
            }
            else
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(control))
                {
                    if (ignoreProperty.Contains(property.Name))
                        continue;

                    if (withChildren && property.Name == "ChildWindows")
                        continue;

                    property.SetValue(this, property.GetValue(control));
                }

                if (copyId)
                    mGuid = control.Id;
                else
                    mCopyedFromId = control.Id;

                if (withChildren)
                {
                    this.ClearChildWindows();
                    foreach (var child in control.ChildWindows)
                    {
                        //if (child.IsTemplateControl)
                        //    continue;

                        var copyedChild = child.GetType().Assembly.CreateInstance(child.GetType().ToString()) as WinBase;
                        copyedChild.CopyFrom(child, new List<string>() { "Parent" }, copyId, withChildren);
                        copyedChild.Parent = this;
                    }
                }
            }

            return true;
        }

        // 绘制辅助对象
        public virtual void RenderAssist(UIRenderPipe pipe, int zOrder, CSUtility.Support.Point parentLoc)
        {
            parentLoc.X += Left;
            parentLoc.Y += Top;

            foreach (var child in ChildWindows)
            {
                child.RenderAssist(pipe, zOrder+10, parentLoc);
            }
        }

        public WinBase GetNearestX(int inX, ref int outX, int range, WinBase[] exceptControls)
        {
            foreach(var ctrl in exceptControls)
            {
                if (ctrl == this)
                    return null;
            }

            var leftOffset = System.Math.Abs(inX - Left);
            var rightOffset = System.Math.Abs(inX - Right);

            int minOffset = 0;
            if (leftOffset < rightOffset)
            {
                minOffset = leftOffset;
                outX = Left;
            }
            else
            {
                minOffset = rightOffset;
                outX = Right;
            }

            inX -= Left;
            int childOutX = 0;
            WinBase retControl = this;
            foreach (var child in ChildWindows)
            {
                var nearestControl = child.GetNearestX(inX, ref childOutX, range, exceptControls);
                if (nearestControl != null)
                {
                    var offset = System.Math.Abs(inX - childOutX);
                    if (offset < minOffset)
                    {
                        minOffset = offset;
                        retControl = nearestControl;
                        outX = childOutX + Left;
                    }
                }
            }

            if (minOffset < range)
                return retControl;

            return null;
        }

        public WinBase GetNearestY(int inY, ref int outY, int range, WinBase[] exceptControls)
        {
            foreach(var ctrl in exceptControls)
            {
                if (ctrl == this)
                    return null;
            }

            var topOffset = System.Math.Abs(inY - Top);
            var bottomOffset = System.Math.Abs(inY - Bottom);

            int minOffset = 0;
            if (topOffset < bottomOffset)
            {
                minOffset = topOffset;
                outY = Top;
            }
            else
            {
                minOffset = bottomOffset;
                outY = Bottom;
            }

            inY -= Top;
            int childOutY = 0;
            WinBase retControl = this;
            foreach (var child in ChildWindows)
            {
                var nearestControl = child.GetNearestY(inY, ref childOutY, range, exceptControls);
                if (nearestControl != null)
                {
                    var offset = System.Math.Abs(inY - childOutY);
                    if (offset < minOffset)
                    {
                        minOffset = offset;
                        retControl = nearestControl;
                        outY = childOutY + Top;
                    }
                }
            }

            if (minOffset < range)
                return retControl;

            return null;
        }

        //public bool MouseMoveAbleInEditor = true;

        protected double mTreeViewItemHeight = 25;
        [Browsable(false)]
        public double TreeViewItemHeight
        {
            get { return mTreeViewItemHeight; }
            set
            {
                mTreeViewItemHeight = value;
            }
        }

        bool mIsVisibleInEditor = true;
        [Browsable(false)]
        public bool IsVisibleInEditor
        {
            get { return mIsVisibleInEditor; }
            set
            {
                mIsVisibleInEditor = value;
                OnPropertyChanged("IsVisibleInEditor");
            }
        }

        string mNameInEditor = "";
        [Browsable(false)]
        public string NameInEditor
        {
            get
            {
                var atts = this.GetType().GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_ControlAttribute), true);
                if (atts.Length > 0)
                {
                    var name = ((CSUtility.Editor.UIEditor_ControlAttribute)atts[0]).Name;
                    name = name.Substring(name.LastIndexOf('.') + 1);
                    return "[" + name + "]" + WinName;
                }
                //return "[" + this.GetType().Name + "]" + WinName;
                if (this is UISystem.Template.ControlTemplate)
                    return "Template(" + WinName + ")";

                atts = this.GetType().GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute), true);
                if (atts.Length > 0)
                {
                    var name = ((CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute)atts[0]).Name;
                    name = name.Substring(name.LastIndexOf('.') + 1);
                    return "[" + name + "]" + WinName;
                }

                return "Form(" + WinName + ")";
            }
            set
            {
                mNameInEditor = value;
                OnPropertyChanged("NameInEditor");
            }
        }

        //Visibility mUpInsertLineVisible = Visibility.Collapsed;
        //[Browsable(false)]
        //public Visibility UpInsertLineVisible
        //{
        //    get { return mUpInsertLineVisible; }
        //    set
        //    {
        //        mUpInsertLineVisible = value;
        //        OnPropertyChanged("UpInsertLineVisible");
        //    }
        //}

        //Visibility mDownInsertLineVisible = Visibility.Collapsed;
        //[Browsable(false)]
        //public Visibility DownInsertLineVisible
        //{
        //    get { return mDownInsertLineVisible; }
        //    set
        //    {
        //        mDownInsertLineVisible = value;
        //        OnPropertyChanged("DownInsertLineVisible");
        //    }
        //}

        //Visibility mChildInsertLineVisible = Visibility.Collapsed;
        //[Browsable(false)]
        //public Visibility ChildInsertLineVisible
        //{
        //    get { return mChildInsertLineVisible; }
        //    set
        //    {
        //        mChildInsertLineVisible = value;
        //        OnPropertyChanged("ChildInsertLineVisible");
        //    }
        //}

        //System.Windows.Media.Brush mTreeViewItemForeGround = System.Windows.Media.Brushes.White;
        //[Browsable(false)]
        //public System.Windows.Media.Brush TreeViewItemForeground
        //{
        //    get { return mTreeViewItemForeGround; }
        //    set
        //    {
        //        mTreeViewItemForeGround = value;
        //        OnPropertyChanged("TreeViewItemForeground");
        //    }
        //}

        //System.Windows.Media.Brush mTreeViewItemBackground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(1, 0, 0, 0));
        //[Browsable(false)]
        //public System.Windows.Media.Brush TreeViewItemBackground
        //{
        //    get { return mTreeViewItemBackground; }
        //    set
        //    {
        //        mTreeViewItemBackground = value;
        //        OnPropertyChanged("TreeViewItemBackground");
        //    }
        //}

        //public Byte TreeViewItemBG_A = 1;
        //public Byte TreeViewItemBG_R = 0;
        //public Byte TreeViewItemBG_G = 0;
        //public Byte TreeViewItemBG_B = 0;

        //public Byte TreeViewItemFG_A = 255;
        //public Byte TreeViewItemFG_R = 255;
        //public Byte TreeViewItemFG_G = 255;
        //public Byte TreeViewItemFG_B = 255;
        public Object HostEditorWin = null;

        // 编辑器中显示横向布局线
        public bool EnableHorizontalArrangementLineShow = true;
        // 编辑器中显示纵向布局线
        public bool EnableVerticalArrangementLineShow = true;

        // 逻辑子，在UI编辑器中树形控件列表中显示的是逻辑子
        protected CSUtility.Support.ThreadSafeObservableCollection<WinBase> mLogicChildren = new CSUtility.Support.ThreadSafeObservableCollection<WinBase>();
        [Browsable(false)]
        public CSUtility.Support.ThreadSafeObservableCollection<WinBase> LogicChildren
        {
            get { return mLogicChildren; }
            set
            {
                if (value == null)
                {
                    mLogicChildren.Clear();
                    return;
                }

                mLogicChildren.Clear();
                foreach (var child in value)
                {
                    mLogicChildren.Add(child);
                }
            }
        }

        protected virtual void LogicChildren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Move:
                    {
                        MoveChild(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }
    }
}
