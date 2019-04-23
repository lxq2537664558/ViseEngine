using System.Collections.Specialized;
using System.ComponentModel;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_Control("容器.WrapPanel")]
    public class WrapPanel : Container.Container
    {
        public enum OrientationType
        {
            Left2Right_Top2Bottom,
            Left2Right_Bottom2Top,
            Right2Left_Top2Bottom,
            Right2Left_Bottom2Top,
            Top2Bottom_Left2Right,
            Top2Bottom_Right2Left,
            Bottom2Top_Left2Right,
            Bottom2Top_Right2Left,
        }

        OrientationType mOrientation = OrientationType.Left2Right_Top2Bottom;
        [Category("布局"), DisplayName("方向")]
        public OrientationType Orientation
        {
            get { return mOrientation; }
            set
            {
                mOrientation = value;
                UpdateLayout();
                OnPropertyChanged("Orientation");
            }
        }

        public WrapPanel()
        {
            ContainerType = enContainerType.Multi;
        }

        protected override void ChildWindows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (WinBase item in e.NewItems)
                        {
                            item.EnableEditorMouseMove = false;

                            item.Margin = new CSUtility.Support.Thickness(0);
                            item.LockedHorizontals.Clear();
                            item.LockedVerticals.Clear();

                            item.HorizontalAlignment = UI.HorizontalAlignment.Left;
                            item.LockedHorizontals.Add(UI.HorizontalAlignment.Left);
                            item.VerticalAlignment = UI.VerticalAlignment.Top;
                            item.LockedVerticals.Add(UI.VerticalAlignment.Top);

                            item.EnableHorizontalArrangementLineShow = false;
                            item.EnableVerticalArrangementLineShow = false;

                            if (!item.IsTemplateControl)
                                LogicChildren.Add(item);
                        }
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
        }

        public override CSUtility.Support.Point GetChildOffset(WinBase child)
        {
            var index = ChildWindows.IndexOf(child);
            if (index < 0)
                return base.GetChildOffset(child);

            int sumX = 0, sumY = 0;
            int maxYRow = 0;

            for (int i = 0; i < index; i++)
            {
                switch (Orientation)
                {
                    case OrientationType.Left2Right_Top2Bottom:
                        {
                            if (sumX + child.DesiredSize.Width >= Width)
                            {
                                sumY += maxYRow;
                                maxYRow = 0;
                                sumX = 0;
                            }
                            sumX += child.Width;
                            maxYRow = System.Math.Max(maxYRow, child.Height);
                        }
                        break;
                }
            }

            return new CSUtility.Support.Point(sumX, sumY);
        }

        protected override SlimDX.Size MeasureOverride(SlimDX.Size availableSize)
        {
            var returnDesiredSize = new SlimDX.Size();

            double widthToSet = 0;
            double heightToSet = 0;

            double childTotalWidth = 0;
            double childTotalHeight = 0;

            foreach (var child in ChildWindows)
            {
                if (child.Visibility == Visibility.Collapsed)
                    continue;

                child.Measure(availableSize);

                childTotalWidth += child.DesiredSize.Width;
                childTotalHeight += child.DesiredSize.Height;
            }
            childTotalWidth += 1;
            childTotalHeight += 1;

            double childDesireWidth = 0;
            double childDesireHeight = 0;
            double childMaxWidth = 0;
            double childMaxHeight = 0;
            double widthSum = 0;
            double heightSum = 0;
            switch (Orientation)
            {
                case OrientationType.Left2Right_Bottom2Top:
                case OrientationType.Left2Right_Top2Bottom:
                    {
                        if (Width_Auto)
                            widthToSet = System.Math.Min(childTotalWidth, availableSize.Width);
                        else
                            widthToSet = Width;

                        foreach (var child in ChildWindows)
                        {
                            if (widthSum + child.DesiredSize.Width - child.Margin.Right > widthToSet)
                            {
                                childDesireHeight += childMaxHeight;
                                childMaxHeight = 0;
                                widthSum = 0;
                            }
                            widthSum += child.DesiredSize.Width;
                            childMaxHeight = System.Math.Max(child.DesiredSize.Height, childMaxHeight);
                        }
                        childDesireHeight += childMaxHeight;

                        if (Height_Auto)
                            heightToSet = System.Math.Min(childDesireHeight, availableSize.Height);
                        else
                            heightToSet = Height;
                    }
                    break;
                case OrientationType.Right2Left_Bottom2Top:
                case OrientationType.Right2Left_Top2Bottom:
                    {
                        if (Width_Auto)
                            widthToSet = System.Math.Min(childTotalWidth, availableSize.Width);
                        else
                            widthToSet = Width;

                        foreach (var child in ChildWindows)
                        {
                            widthSum += child.DesiredSize.Width;
                            if (widthSum - child.Margin.Left > widthToSet)
                            {
                                childDesireHeight += childMaxHeight;
                                childMaxHeight = 0;
                                widthSum = child.DesiredSize.Width;
                            }
                            childMaxHeight = System.Math.Max(child.DesiredSize.Height, childMaxHeight);
                        }
                        childDesireHeight += childMaxHeight;

                        if (Height_Auto)
                            heightToSet = System.Math.Min(childDesireHeight, availableSize.Height);
                        else
                            heightToSet = Height;
                    }
                    break;

                case OrientationType.Top2Bottom_Left2Right:
                case OrientationType.Top2Bottom_Right2Left:
                    {
                        if (Height_Auto)
                            heightToSet = System.Math.Min(childTotalHeight, availableSize.Height);
                        else
                            heightToSet = Height;

                        foreach (var child in ChildWindows)
                        {
                            if (heightSum + child.DesiredSize.Height - child.Margin.Bottom > heightToSet)
                            {
                                childDesireWidth += childMaxWidth;
                                childMaxWidth = 0;
                                heightSum = 0;
                            }
                            heightSum += child.DesiredSize.Height;
                            childMaxWidth = System.Math.Max(child.DesiredSize.Width, childMaxWidth);
                        }
                        childDesireWidth += childMaxWidth;

                        if (Width_Auto)
                            widthToSet = System.Math.Min(childDesireWidth, availableSize.Width);
                        else
                            widthToSet = Width;
                    }
                    break;
                case OrientationType.Bottom2Top_Left2Right:
                case OrientationType.Bottom2Top_Right2Left:
                    {
                        if (Height_Auto)
                            heightToSet = System.Math.Min(childTotalHeight, availableSize.Height);
                        else
                            heightToSet = Height;

                        foreach (var child in ChildWindows)
                        {
                            heightSum += child.DesiredSize.Height;
                            if (heightSum - child.Margin.Top > heightToSet)
                            {
                                childDesireWidth += childMaxWidth;
                                childMaxWidth = 0;
                                heightSum = child.DesiredSize.Height;
                            }
                            childMaxWidth = System.Math.Max(child.DesiredSize.Width, childMaxWidth);
                        }
                        childDesireWidth += childMaxWidth;

                        if (Width_Auto)
                            widthToSet = System.Math.Min(childDesireWidth, availableSize.Width);
                        else
                            widthToSet = Width;
                    }
                    break;
            }



            //foreach (var child in ChildWindows)
            //{
            //    switch (Orientation)
            //    {
            //        case OrientationType.Left2Right_Bottom2Top:
            //        case OrientationType.Left2Right_Top2Bottom:
            //            {
            //                if (widthSum + child.DesiredSize.Width - child.Margin.Right > widthToSet)
            //                {
            //                    childDesireHeight += childMaxHeight;
            //                    widthSum = 0;
            //                }
            //                widthSum += child.DesiredSize.Width;
            //                childMaxHeight = System.Math.Max(child.DesiredSize.Height, childMaxHeight);
            //            }
            //            break;

            //        case OrientationType.Right2Left_Bottom2Top:
            //        case OrientationType.Right2Left_Top2Bottom:
            //            {
            //                widthSum += child.DesiredSize.Width;
            //                if (widthSum - child.Margin.Left > widthToSet)
            //                {
            //                    childDesireHeight += childMaxHeight;
            //                    widthSum = child.DesiredSize.Width;
            //                }
            //                childMaxHeight = System.Math.Max(child.DesiredSize.Height, childMaxHeight);
            //            }
            //            break;

            //        case OrientationType.Top2Bottom_Left2Right:
            //        case OrientationType.Top2Bottom_Right2Left:
            //            {

            //            }
            //            break;

            //        case OrientationType.Bottom2Top_Left2Right:
            //        case OrientationType.Bottom2Top_Right2Left:
            //            {
            //                if (Height_Auto)
            //                {
            //                    heightToSet += child.DesiredSize.Height;
            //                }
            //                else
            //                {
            //                    heightToSet = Height;
            //                }
            //            }
            //            break;
            //    }
            //}


            returnDesiredSize.Width = (float)widthToSet;
            returnDesiredSize.Height = (float)heightToSet;

            return returnDesiredSize;
        }

        protected override SlimDX.Size ArrangeOverride(SlimDX.Size finalSize)
        {
            double sumX = 0, sumY = 0;
            double maxYRow = 0, maxXColumn = 0;;

            foreach (var child in ChildWindows)
            {
                if (child.Visibility == Visibility.Collapsed)
                    continue;

                switch (Orientation)
                {
                    case OrientationType.Left2Right_Top2Bottom:
                        {
                            if (sumX + child.DesiredSize.Width - child.Margin.Right >= finalSize.Width)
                            {
                                sumY += maxYRow;
                                maxYRow = 0;
                                sumX = 0;
                            }
                            child.Arrange(new SlimDX.Rect(sumX, sumY, child.DesiredSize.Width, child.DesiredSize.Height));
                            sumX += child.DesiredSize.Width;
                            maxYRow = System.Math.Max(maxYRow, child.DesiredSize.Height);
                        }
                        break;
                    case OrientationType.Left2Right_Bottom2Top:
                        {
                            if (sumX + child.DesiredSize.Width - child.Margin.Right >= finalSize.Width)
                            {
                                sumY += maxYRow;
                                maxYRow = 0;
                                sumX = 0;
                            }
                            child.Arrange(new SlimDX.Rect(sumX, finalSize.Height - sumY - child.DesiredSize.Height, child.DesiredSize.Width, child.DesiredSize.Height));
                            sumX += child.DesiredSize.Width;
                            maxYRow = System.Math.Max(maxYRow, child.DesiredSize.Height);
                        }
                        break;
                    case OrientationType.Right2Left_Top2Bottom:
                        {
                            sumX += child.DesiredSize.Width;
                            if (sumX - child.Margin.Left >= finalSize.Width)
                            {
                                sumY += maxYRow;
                                maxYRow = 0;
                                sumX = child.DesiredSize.Width;
                            }
                            child.Arrange(new SlimDX.Rect(finalSize.Width - sumX, sumY, child.DesiredSize.Width, child.DesiredSize.Height));
                            maxYRow = System.Math.Max(maxYRow, child.DesiredSize.Height);
                        }
                        break;
                    case OrientationType.Right2Left_Bottom2Top:
                        {
                            sumX += child.DesiredSize.Width;
                            if (sumX - child.Margin.Left >= finalSize.Width)
                            {
                                sumY += maxYRow;
                                maxYRow = 0;
                                sumX = child.DesiredSize.Width;
                            }
                            child.Arrange(new SlimDX.Rect(finalSize.Width - sumX, finalSize.Height - sumY - child.DesiredSize.Height, child.DesiredSize.Width, child.DesiredSize.Height));
                            maxYRow = System.Math.Max(maxYRow, child.DesiredSize.Height);
                        }
                        break;
                    case OrientationType.Top2Bottom_Left2Right:
                        {
                            if (sumY + child.DesiredSize.Height - child.Margin.Bottom >= finalSize.Height)
                            {
                                sumX += maxXColumn;
                                maxXColumn = 0;
                                sumY = 0;
                            }
                            child.Arrange(new SlimDX.Rect(sumX, sumY, child.DesiredSize.Width, child.DesiredSize.Height));
                            sumY += child.DesiredSize.Height;
                            maxXColumn = System.Math.Max(maxXColumn, child.DesiredSize.Width);
                        }
                        break;
                    case OrientationType.Top2Bottom_Right2Left:
                        {
                            if (sumY + child.DesiredSize.Height - child.Margin.Bottom >= finalSize.Height)
                            {
                                sumX += maxXColumn;
                                maxXColumn = 0;
                                sumY = 0;
                            }
                            child.Arrange(new SlimDX.Rect(finalSize.Width - sumX - child.DesiredSize.Width, sumY, child.DesiredSize.Width, child.DesiredSize.Height));
                            sumY += child.DesiredSize.Height;
                            maxXColumn = System.Math.Max(maxXColumn, child.DesiredSize.Width);
                        }
                        break;
                    case OrientationType.Bottom2Top_Left2Right:
                        {
                            sumY += child.DesiredSize.Height;
                            if (sumY - child.Margin.Bottom >= finalSize.Height)
                            {
                                sumX += maxXColumn;
                                maxXColumn = 0;
                                sumY = child.DesiredSize.Height;
                            }
                            child.Arrange(new SlimDX.Rect(sumX, finalSize.Height - sumY, child.DesiredSize.Width, child.DesiredSize.Height));
                            maxXColumn = System.Math.Max(maxXColumn, child.DesiredSize.Width);
                        }
                        break;
                    case OrientationType.Bottom2Top_Right2Left:
                        {
                            sumY += child.DesiredSize.Height;
                            if (sumY - child.Margin.Bottom >= finalSize.Height)
                            {
                                sumX += maxXColumn;
                                maxXColumn = 0;
                                sumY = child.DesiredSize.Height;
                            }
                            child.Arrange(new SlimDX.Rect(finalSize.Width - sumX - child.DesiredSize.Width, finalSize.Height - sumY, child.DesiredSize.Width, child.DesiredSize.Height));
                            maxXColumn = System.Math.Max(maxXColumn, child.DesiredSize.Width);
                        }
                        break;
                }
            }

            return finalSize;
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml, holder);

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Orientation"))
                pXml.AddAttrib("Orientation", Orientation.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            CSUtility.Support.XmlAttrib attr = pXml.FindAttrib("Orientation");
            if (attr != null)
            {
                Orientation = CSUtility.Support.IHelper.EnumTryParse<OrientationType>(attr.Value);
            }
        }
    }
}
