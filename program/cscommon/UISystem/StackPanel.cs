using System.Collections.Specialized;
using System.ComponentModel;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_Control("容器.StackPanel")]
    public class StackPanel : Container.Container
    {
        public enum OrientationType
        {
            LeftToRight,
            RightToLeft,
            TopToBottom,
            BottomToTop,
        }

        OrientationType mOrientation = OrientationType.TopToBottom;
        [Category("布局"), DisplayName("方向")]
        public OrientationType Orientation
        {
            get { return mOrientation; }
            set
            {
                mOrientation = value;

                switch(mOrientation)
                {
                    case OrientationType.LeftToRight:
                    case OrientationType.RightToLeft:
                        {
                            foreach (var child in ChildWindows)
                            {
                                child.EnableHorizontalArrangementLineShow = false;
                                child.EnableVerticalArrangementLineShow = true;
                            }
                        }
                        break;

                    case OrientationType.TopToBottom:
                    case OrientationType.BottomToTop:
                        {
                            foreach (var child in ChildWindows)
                            {
                                child.EnableHorizontalArrangementLineShow = true;
                                child.EnableVerticalArrangementLineShow = false;
                            }
                        }
                        break;
                }

                foreach (var item in LogicChildren)
                {
                    UpdateChildLockedAlignment(item);
                }

                //UpdateHorizontalArrangement();
                //UpdateVerticalArrangement();
                UpdateLayout();

                OnPropertyChanged("Orientation");
            }
        }

        public StackPanel()
        {
            ContainerType = enContainerType.Multi;
        }

        protected void UpdateChildLockedAlignment(WinBase item)
        {
            item.Margin = new CSUtility.Support.Thickness(0);
            item.LockedHorizontals.Clear();
            item.LockedVerticals.Clear();

            //            item.MouseMoveAbleInEditor = false;
            switch (Orientation)
            {
                case OrientationType.LeftToRight: //UI.Orientation.Horizontal:
                    //item.Height_Auto = true;
                    //item.Margin = new Thickness(0, item.Margin.Top, 0, item.Margin.Bottom);
                    item.HorizontalAlignment = UI.HorizontalAlignment.Left;
                    //item.VerticalAlignment = UI.VerticalAlignment.Stretch;
                    item.LockedHorizontals.Add(UI.HorizontalAlignment.Left);
                    item.EnableHorizontalArrangementLineShow = false;
                    item.EnableVerticalArrangementLineShow = true;
                    break;

                case OrientationType.RightToLeft:
                    //item.Height_Auto = true;
                    //item.Margin = new Thickness(0, item.Margin.Top, 0, item.Margin.Bottom);
                    item.HorizontalAlignment = UI.HorizontalAlignment.Left;
                    //item.VerticalAlignment = VerticalAlignment.Stretch;
                    item.LockedHorizontals.Add(UI.HorizontalAlignment.Left);
                    item.EnableHorizontalArrangementLineShow = false;
                    item.EnableVerticalArrangementLineShow = true;
                    break;

                case OrientationType.TopToBottom: //UI.Orientation.Vertical:
                    //item.Width_Auto = true;
                    //item.Margin = new Thickness(item.Margin.Left, 0, item.Margin.Right, 0);
                    //item.HorizontalAlignment = UI.HorizontalAlignment.Stretch;
                    item.VerticalAlignment = UI.VerticalAlignment.Top;
                    item.LockedVerticals.Add(UI.VerticalAlignment.Top);
                    item.EnableHorizontalArrangementLineShow = true;
                    item.EnableVerticalArrangementLineShow = false;
                    break;

                case OrientationType.BottomToTop:
                    //item.Width_Auto = true;
                    //item.Margin = new Thickness(item.Margin.Left, 0, item.Margin.Right, 0);
                    //item.HorizontalAlignment = HorizontalAlignment.Stretch;
                    item.VerticalAlignment = UI.VerticalAlignment.Top;
                    item.LockedVerticals.Add(UI.VerticalAlignment.Top);
                    item.EnableHorizontalArrangementLineShow = true;
                    item.EnableVerticalArrangementLineShow = false;
                    break;
            }
        }

        protected override void ChildWindows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (WinBase item in e.NewItems)
                    {
                        //item.EnableHorizontalArrangementLineShow = false;

                        UpdateChildLockedAlignment(item);
                        //item.Margin = new CSCommon.Support.Thickness(0);

                        if (!item.IsTemplateControl)
                            LogicChildren.Add(item);

                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (WinBase item in e.OldItems)
                        {
                            if (LogicChildren.Contains(item))
                            {
                                LogicChildren.Remove(item);
                            }
                        }
                    }
                    break;
            }

            UpdateLayout();

            //UpdateHorizontalArrangement();
            //UpdateVerticalArrangement();
        }

        public override CSUtility.Support.Point GetChildOffset(WinBase child)
        {
            if (child == null)
                return base.GetChildOffset(child);
            var index = ChildWindows.IndexOf(child);
            if (index < 0)
                return base.GetChildOffset(child);

            var offset = 0;
            switch (Orientation)
            {
                case OrientationType.LeftToRight:
                    {
                        for (int i = 0; i < index; i++)
                        {
                            offset += (int)(ChildWindows[i].Width + ChildWindows[i].Margin.Left + ChildWindows[i].Margin.Right);
                        }
                        return new CSUtility.Support.Point(offset, 0);
                    }
                case OrientationType.RightToLeft:
                    {
                        for (int i = 0; i < index; i++)
                        {
                            offset += (int)(ChildWindows[i].Width + ChildWindows[i].Margin.Left + ChildWindows[i].Margin.Right);
                        }
                        offset = (int)(Width - offset - (child.Width + child.Margin.Right));
                        return new CSUtility.Support.Point(offset, 0);
                    }
                case OrientationType.TopToBottom:
                    {
                        for (int i = 0; i < index; i++)
                        {
                            offset += (int)(ChildWindows[i].Height + ChildWindows[i].Margin.Top + ChildWindows[i].Margin.Bottom);
                        }
                        return new CSUtility.Support.Point(0, offset);
                    }
                case OrientationType.BottomToTop:
                    {
                        for (int i = 0; i < index; i++)
                        {
                            offset += (int)(ChildWindows[i].Height + ChildWindows[i].Margin.Top + ChildWindows[i].Margin.Bottom);
                        }
                        offset = (int)(Height - offset - (child.Height + child.Margin.Bottom));
                        return new CSUtility.Support.Point(0, offset);
                    }
            }

            return base.GetChildOffset(child);
        }

        public override CSUtility.Support.Size GetSizeByChild(WinBase child)
        {
            if (child == null)
                return base.GetSizeByChild(child);
            var index = ChildWindows.IndexOf(child);
            if (index < 0)
                return base.GetSizeByChild(child);

            switch (Orientation)
            {
                case OrientationType.LeftToRight:
                case OrientationType.RightToLeft:
                    return new CSUtility.Support.Size((int)(child.Width + child.Margin.Left + child.Margin.Top),
                                                   Height);
                case OrientationType.TopToBottom:
                case OrientationType.BottomToTop:
                    return new CSUtility.Support.Size(Width,
                                                   (int)(child.Height + child.Margin.Top + child.Margin.Bottom));
            }

            return base.GetSizeByChild(child);
        }

        //protected override void UpdateWidthFromChildren()
        //{
        //    base.UpdateWidthFromChildren();

        //    switch (Orientation)
        //    {
        //        case UI.Orientation.Horizontal:
        //            UpdateHorizontalArrangement();
        //            break;

        //        case UI.Orientation.Vertical:
        //            UpdateVerticalArrangement();
        //            break;
        //    }
        //}

        //protected override void UpdateHeightFromChildren()
        //{
        //    base.UpdateHeightFromChildren();

        //    switch (Orientation)
        //    {
        //        case UI.Orientation.Horizontal:
        //            UpdateHorizontalArrangement();
        //            break;
                    
        //        case UI.Orientation.Vertical:
        //            base.UpdateHeightFromChildren();
        //            UpdateVerticalArrangement();
        //            break;
        //    }
        //}

        //protected override void UpdateHorizontalArrangement()
        //{
        //    base.UpdateHorizontalArrangement();
        //}

        //protected override void UpdateVerticalArrangement()
        //{
        //    base.UpdateVerticalArrangement();
        //}

        protected override SlimDX.Size MeasureOverride(SlimDX.Size availableSize)
        {
            SlimDX.Size returnDesiredSize = new SlimDX.Size();

            foreach (var child in ChildWindows)
            {
                if (child.Visibility == Visibility.Collapsed)
                    continue;

                child.Measure(availableSize);

                switch (Orientation)
                {
                    case OrientationType.LeftToRight:
                    case OrientationType.RightToLeft:
                        {
                            returnDesiredSize.Width += child.DesiredSize.Width;
                            returnDesiredSize.Height = System.Math.Max(returnDesiredSize.Height, child.DesiredSize.Height);
                        }
                        break;

                    case OrientationType.TopToBottom:
                    case OrientationType.BottomToTop:
                        {
                            returnDesiredSize.Width = System.Math.Max(returnDesiredSize.Width, child.DesiredSize.Width);
                            returnDesiredSize.Height += child.DesiredSize.Height;
                        }
                        break;
                }
            }

            return returnDesiredSize;
        }

        protected override SlimDX.Size ArrangeOverride(SlimDX.Size finalSize)
        {
            int x = 0, y = 0;

            for (int i = 0; i < ChildWindows.Count; i++)
            {
                var child = ChildWindows[i];

                if (child.Visibility == Visibility.Collapsed)
                    continue;

                switch (Orientation)
                {
                    case OrientationType.LeftToRight:
                        {
                            child.Arrange(new SlimDX.Rect(x, 0, finalSize.Width, finalSize.Height));
                            x += child.TotalWidth;                        
                        }
                        break;

                    case OrientationType.RightToLeft:
                        {
                            x += child.TotalWidth;
                            child.Arrange(new SlimDX.Rect((int)(finalSize.Width - x), 0, finalSize.Width, finalSize.Height));
                        }
                        break;

                    case OrientationType.TopToBottom:
                        {
                            child.Arrange(new SlimDX.Rect(0, y, finalSize.Width, finalSize.Height));
                            y += child.TotalHeight;                       
                        }
                        break;

                    case OrientationType.BottomToTop:
                        {
                            y += child.TotalHeight;
                            child.Arrange(new SlimDX.Rect(0, (int)(finalSize.Height - y), finalSize.Width, finalSize.Height));
                        }
                        break;
                }
            }

            return finalSize;
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Orientation"))
                pXml.AddAttrib("Orientation", Orientation.ToString());

            base.OnSave(pXml, holder);
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            var attr = pXml.FindAttrib("Orientation");
            if (attr != null)
            {
                Orientation = CSUtility.Support.IHelper.EnumTryParse<OrientationType>(attr.Value);
            }

            base.OnLoad(pXml);
        }
    }
}
