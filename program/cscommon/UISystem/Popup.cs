using System;
using System.ComponentModel;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_Control("组件.Popup")]
    public class Popup : WinBase
    {
        public enum enPlacementMode
        {
            Bottom,
            Left,
            Mouse,
            Right,
            Top,
        }

        [Category("外观")]
        [Browsable(false)]
        public WinState State
        {
            get { return mWinState; }
        }
        [Category("外观")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        [DisplayName("背景图元")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("UVAnimSet")]
        public Guid StateUVAnimId
        {
            get
            {
                if (mWinState == null)
                    return Guid.Empty;
                return mWinState.UVAnimId;
            }
            set
            {
                if (mWinState != null)
                    mWinState.UVAnimId = value;
                OnPropertyChanged("StateUVAnimId");
            }
        }

        private WinBase mPlacementTarget = null;
        private WinBase mChild = null;

        enPlacementMode mPlacementMode = enPlacementMode.Bottom;
        [Category("行为"), DisplayName("放置类型")]
        public enPlacementMode PlacementMode
        {
            get { return mPlacementMode; }
            set
            {
                if (mPlacementMode == value)
                    return;

                mPlacementMode = value;

                Reposition();

                OnPropertyChanged("PlacementMode");
            }
        }

        bool mIsOpen = false;
        [Category("行为"), DisplayName("是否打开")]
        public bool IsOpen
        {
            get { return mIsOpen; }
            set
            {
                if (mIsOpen == value)
                    return;

                mIsOpen = value;
                var root = this.GetRoot() as WinRoot;

                if (mIsOpen)
                {
                    //this.Visibility = Visibility.Visible;
                    if (root != null)
                    {
                        root.PopupedWins.Add(this);
                    }
                }
                else
                {
                    //this.Visibility = Visibility.Hidden;
                    if (root != null)
                    {
                        root.PopupedWins.Remove(this);
                    }
                }

                Reposition();

                OnPropertyChanged("IsOpen");
            }
        }

        bool mStaysOpen = false;
        [Category("行为"), DisplayName("保持打开")]
        public bool StaysOpen
        {
            get { return mStaysOpen; }
            set
            {
                mStaysOpen = value;

                OnPropertyChanged("StaysOpen");
            }
        }

        public Popup()
        {
            mWinState = new WinState(this);
            ContainerType = enContainerType.One;

            Width_Auto = true;
            Height_Auto = true;

            LockedHorizontals.Add(UI.HorizontalAlignment.Center);
            LockedHorizontals.Add(UI.HorizontalAlignment.Left);
            LockedHorizontals.Add(UI.HorizontalAlignment.Right);
            LockedHorizontals.Add(UI.HorizontalAlignment.Stretch);

            LockedVerticals.Add(UI.VerticalAlignment.Bottom);
            LockedVerticals.Add(UI.VerticalAlignment.Center);
            LockedVerticals.Add(UI.VerticalAlignment.Stretch);
            LockedVerticals.Add(UI.VerticalAlignment.Top);
        }

        protected override void OnSetParent(WinBase parent)
        {
            base.OnSetParent(parent);

            mPlacementTarget = parent;
        }

        protected override void OnAddChild(WinBase child)
        {
            base.OnAddChild(child);

            mChild = child;
        }

        public override void UpdateClipRect(bool bWithChildren)
        {
            if (mChild != null)
            {
                mClipRect = mChild.AbsRect;
            }
            //mClipRect = mAbsRect;

            //foreach (WinBase i in mChildWindows)
            //{
            //    i.UpdateClipRect();
            //}
        }

        protected override SlimDX.Size MeasureOverride(SlimDX.Size availableSize)
        {
            if (mChild != null)
            {
                mChild.Measure(availableSize);
            }
            return new SlimDX.Size();
        }

        private void Reposition()
        {
            if (IsOpen)
            {
                switch (PlacementMode)
                {
                    case enPlacementMode.Left:
                        {
                            // todo: 判断正常位置是否超过屏幕，超过则按照Right处理
                            var childWidth = 0;
                            if (mChild != null)
                                childWidth = mChild.Width;
                            this.Margin = new CSUtility.Support.Thickness(-childWidth, 0, 0, 0);
                            this.HorizontalAlignment = UI.HorizontalAlignment.Left;
                            this.VerticalAlignment = UI.VerticalAlignment.Top;
                        }
                        break;

                    case enPlacementMode.Right:
                        {
                            // todo: 判断正常位置是否超过屏幕，超过则按照Left处理
                            this.Margin = new CSUtility.Support.Thickness();
                            this.HorizontalAlignment = UI.HorizontalAlignment.Right;
                            this.VerticalAlignment = UI.VerticalAlignment.Top;
                        }
                        break;

                    case enPlacementMode.Top:
                        {
                            // todo: 判断正常位置是否超过屏幕，超过则按照Bottom处理
                            var childHeight = 0;
                            if (mChild != null)
                                childHeight = mChild.Height;
                            this.Margin = new CSUtility.Support.Thickness(0, -childHeight, 0, 0);
                            this.HorizontalAlignment = UI.HorizontalAlignment.Left;
                            this.VerticalAlignment = UI.VerticalAlignment.Top;
                        }
                        break;

                    case enPlacementMode.Bottom:
                        {
                            // todo: 判断正常位置是否超过屏幕，超过则按照Top处理
                            this.Margin = new CSUtility.Support.Thickness();
                            this.HorizontalAlignment = UI.HorizontalAlignment.Left;
                            this.VerticalAlignment = UI.VerticalAlignment.Bottom;
                        }
                        break;

                    case enPlacementMode.Mouse:
                        {
                            var targetPos = new CSUtility.Support.Point();
                            var mousePos = UISystem.Device.Mouse.Instance.Position;
                            if (mPlacementTarget != null)
                                targetPos = mPlacementTarget.AbsToLocal(ref mousePos);

                            this.Margin = new CSUtility.Support.Thickness(targetPos.X, targetPos.Y, 0, 0);
                            this.HorizontalAlignment = UI.HorizontalAlignment.Left;
                            this.VerticalAlignment = UI.VerticalAlignment.Top;
                        }
                        break;
                }
            }
        }

        public Popup PopupStayWindow(ref CSUtility.Support.Point pt)
        {
            if (mChild == null)
                return null;

            if(mChild.AbsRect.Contains(pt))
                return this;

            return null;
        }

        public override void Draw(UIRenderPipe pipe, int zOrder, ref SlimDX.Matrix parentMatrix)
        {
            //if(IsOpen)
            //    base.Draw();
        }

        public void PopupDraw(UIRenderPipe pipe, int zOrder)
        {
            var transMat = SlimDX.Matrix.Identity;

            if (IsOpen)
                base.Draw(pipe, zOrder, ref transMat);
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (mWinState != null)
            {
                CSUtility.Support.XmlNode stateNode = pXml.AddNode("WinState", "",holder);
                mWinState.OnSave(stateNode,holder);
            }

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "PlacementMode"))
                pXml.AddAttrib("PlacementMode", PlacementMode.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "IsOpen"))
                pXml.AddAttrib("IsOpen", IsOpen.ToString());
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "StaysOpen"))
                pXml.AddAttrib("StaysOpen", StaysOpen.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            var stateNode = pXml.FindNode("WinState");
            if (stateNode != null)
            {
                if (mWinState == null)
                    mWinState = new WinState(this);
                mWinState.OnLoad(stateNode);
            }

            var attr = pXml.FindAttrib("PlacementMode");
            if (attr != null)
                PlacementMode = (enPlacementMode)System.Enum.Parse(typeof(enPlacementMode), attr.Value);
            attr = pXml.FindAttrib("IsOpen");
            if (attr != null)
                IsOpen = System.Convert.ToBoolean(attr.Value);
            attr = pXml.FindAttrib("StaysOpen");
            if (attr != null)
                StaysOpen = System.Convert.ToBoolean(attr.Value);
        }
    }
}
