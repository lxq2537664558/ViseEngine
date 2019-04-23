using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UIEditor.Panel.ControlsTreeView
{
    /// <summary>
    /// Interaction logic for UIControlsContainer.xaml
    /// </summary>
    public partial class UIControlsContainer : UserControl
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        //public DrawPanel mDrawPanel;
        //public DrawPanel DrawPanel
        //{
        //    get { return mDrawPanel; }
        //    set
        //    {
        //        mDrawPanel = value;
        //        SelectedItems = DrawPanel.SelectedWinControls;
        //    }
        //}

        //ObservableCollection<UISystem.WinBase> mSelectedItems = new ObservableCollection<UISystem.WinBase>();
        public ObservableCollection<UIEditor.WinBase> SelectedItems
        {
            get { return (ObservableCollection<UIEditor.WinBase>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<UIEditor.WinBase>), typeof(UIControlsContainer), new FrameworkPropertyMetadata(new ObservableCollection<UIEditor.WinBase>(), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSelectedItemsChanged)));

        public static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIControlsContainer control = d as UIControlsContainer;

            var oldItems = e.OldValue as ObservableCollection<UIEditor.WinBase>;
            if (oldItems != null)
                oldItems.CollectionChanged -= control.SelectedItems_CollectionChanged;
            control.SelectedItems.CollectionChanged += control.SelectedItems_CollectionChanged;

            control.UpdateSelectItems((ObservableCollection<UIEditor.WinBase>)e.NewValue, new ObservableCollection<WinBase>());
        }

        private void UnselectControlsWithChildren(UIEditor.WinBase win)
        {
            if (win == null)
                return;

            win.TreeViewItemForeground = this.FindResource("UnSelectedForeground") as Brush;
            win.TreeViewItemBackground = this.FindResource("UnSelectedBackground") as Brush;

            foreach (var ctrl in win.LogicChildren)
            {
                UnselectControlsWithChildren(ctrl);
            }
        }

        private void UnSelectedControl(UIEditor.WinBase win)
        {
            if (win == null)
                return;

            win.TreeViewItemForeground = this.FindResource("UnSelectedForeground") as Brush;
            win.TreeViewItemBackground = this.FindResource("UnSelectedBackground") as Brush;
        }

        public void UpdateSelectItems(ObservableCollection<UIEditor.WinBase> selectedItems, ObservableCollection<UIEditor.WinBase> unSelectedItems)//bool bUpdateDrawPanel)
        {
            //if (mDrawPanel != null && bUpdateDrawPanel)
            //{
            ////    mDrawPanel.SelectedWinControls = mSelectedItems;
            //    mDrawPanel.UpdateUIControlsSelection(false);
            //}

            if (unSelectedItems != null)
            {
                foreach (var ctrl in unSelectedItems)
                {
                    UnSelectedControl(ctrl);
                    foreach (var data in mSelectItemIndexDatas)
                    {
                        if (data.mControl == ctrl)
                        {
                            mSelectItemIndexDatas.Remove(data);
                            break;
                        }
                    }
                }

                //UnselectControlsWithChildren(mForm);
                //mSelectItemIndexDatas.Clear();
            }

            if (selectedItems != null)
            {
                foreach (var ctrl in selectedItems)
                {
                    ExpandAllTreeViewParent(ctrl);

                    ctrl.TreeViewItemBackground = this.FindResource("SelectedBackground") as Brush;
                    ctrl.TreeViewItemForeground = this.FindResource("SelectedForeground") as Brush;

                    stSelectItemIndexData data = new stSelectItemIndexData()
                    {
                        mControl = ctrl,
                        mTotalIndex = GetIndexInRoot(ctrl)
                    };
                    mSelectItemIndexDatas.Add(data);
                }

                mSelectItemIndexDatas.Sort(CompareSelectItemByIndex);
            }
        }

        #region 选中对象排序
        private static int CompareSelectItemByIndex(stSelectItemIndexData data1, stSelectItemIndexData data2)
        {
            if (data1.mTotalIndex < data2.mTotalIndex)
                return -1;
            else if (data1.mTotalIndex > data2.mTotalIndex)
                return 1;
            else
                return 0;
        }
        // 保证在拖动时选中对象的顺序不变
        struct stSelectItemIndexData
        {
            public UIEditor.WinBase mControl;
            public int mTotalIndex;
        }
        List<stSelectItemIndexData> mSelectItemIndexDatas = new List<stSelectItemIndexData>();
        private bool GetTotalChildIndex(UISystem.WinBase parent, UISystem.WinBase ctrl, ref int index)
        {
            foreach (var child in parent.LogicChildren)
            {
                index++;

                if (child == ctrl)
                {
                    return true;
                }
                else if(child != null)
                {
                    if (GetTotalChildIndex(child, ctrl, ref index))
                        return true;
                }
            }

            return false;
        }
        private int GetIndexInRoot(UIEditor.WinBase ctrl)
        {
            if(ctrl == null || ctrl == mForm)
                return 0;

            var rootParent = ctrl.UIWin;
            while (rootParent != mForm.UIWin)
            {
                rootParent = rootParent.Parent as UISystem.WinBase;
                if (rootParent == null)
                    return 0;
            }

            int retIndex = 0;
            GetTotalChildIndex(rootParent, ctrl.UIWin, ref retIndex);
            return retIndex;
        }

        #endregion

        enum enDragOperationType
        {
            None,
            Selection,
            InsertBefore,
            InsertAfter,
            InsertChild,
        }
        enDragOperationType mDragOperationType = enDragOperationType.None;

        UIEditor.WinBase mForm;

        //public System.Collections.IList SelectedTreeNodes
        //{
        //    get { return (System.Collections.IList)GetValue(SelectedTreeNodesProperty); }
        //    set { SetValue(SelectedTreeNodesProperty, value); }
        //}
        //public static readonly DependencyProperty SelectedTreeNodesProperty = DependencyProperty.Register("SelectedTreeNodes", typeof(System.Collections.IList), typeof(UIControlsContainer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSelectedTreeNodesChanged)));

        //public static void OnSelectedTreeNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //}

        public UIControlsContainer()
        {
            InitializeComponent();

            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
        }

        ObservableCollection<WinBase> mOldSelections = new ObservableCollection<WinBase>();
        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //ObservableCollection<UISystem.WinBase> collection = new ObservableCollection<UISystem.WinBase>();

            //if (e.NewItems != null)
            //{
            //    foreach (var item in e.NewItems)
            //    {
            //        collection.Add((UISystem.WinBase)item);
            //    }
            //}

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        UpdateSelectItems(null, mOldSelections);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        ObservableCollection<WinBase> selItems = new ObservableCollection<WinBase>();
                        foreach (WinBase win in e.NewItems)
                        {
                            selItems.Add(win);
                        }
                        UpdateSelectItems(selItems, null);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        ObservableCollection<WinBase> unSelItems = new ObservableCollection<WinBase>();
                        foreach (WinBase win in e.OldItems)
                        {
                            unSelItems.Add(win);
                        }
                        UpdateSelectItems(null, unSelItems);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    {
                        ObservableCollection<WinBase> selItems = new ObservableCollection<WinBase>();
                        ObservableCollection<WinBase> unSelItems = new ObservableCollection<WinBase>();
                        foreach (WinBase win in e.NewItems)
                        {
                            selItems.Add(win);
                        }
                        foreach (WinBase win in e.OldItems)
                        {
                            unSelItems.Add(win);
                        }
                        UpdateSelectItems(selItems, unSelItems);
                    }
                    break;
            }

            mOldSelections.Clear();
            foreach (var item in SelectedItems)
            {
                mOldSelections.Add(item);
            }
        }

        private void TreeView_UIControls_Item_Selected(object sender, RoutedEventArgs e)
        {

            //e.OriginalSource 就是TreeViewItem对象，你可以将其保存到窗体类的某个私有字段中，或者直接使用它，如下所示：
            //(e.OriginalSource as TreeViewItem).IsExpanded = true;
        }

        public void SetRootForm(UISystem.WinBase form)
        {
            mForm = new WinBase(form);
            var items = new ObservableCollection<UIEditor.WinBase>();
            items.Add(mForm);
            TreeView_UIControls.ItemsSource = items;

            //if (TreeView_UIControls.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            //{
            //    TreeView_UIControls.UpdateLayout();
            //}

            //var treeViewItem = TreeView_UIControls.ItemContainerGenerator.ContainerFromItem(mForm) as TreeViewItem;
            //if (treeViewItem != null)
            //{
            //    treeViewItem.Expanded -= TreeViewItem_Expanded;
            //    treeViewItem.Expanded += TreeViewItem_Expanded;
            //}

            //var treeViewItem = FindTreeViewItem(mForm);
            //treeViewItem.Expanded += TreeViewItem_Expanded;
            TreeView_UIControls.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
            TreeView_UIControls.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;

            ExpandAll();
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            var generator = sender as System.Windows.Controls.ItemContainerGenerator;
            if (generator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                int i = 0;
                var item = generator.ContainerFromIndex(i) as TreeViewItem;
                while (item != null)
                {
                    i++;
                    UISystem.WinBase control = generator.ItemFromContainer(item) as UISystem.WinBase;
                    item.SetBinding(TreeViewItem.VisibilityProperty, new Binding("VisibleInTreeView") { Source = control });
                    //item.Visibility = control.VisibleInTreeView;
                    item.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                    item.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
                    item = generator.ContainerFromIndex(i) as TreeViewItem;
                }
            }
        }
        
        //void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        //{
        //    //var treeViewItem = sender as TreeViewItem;

        //    //if(treeViewItem.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
        //    //{
        //    //    foreach (UISystem.WinBase control in treeViewItem.Items)
        //    //    {
        //    //        var item = treeViewItem.ItemContainerGenerator.ContainerFromItem(control) as TreeViewItem;
        //    //        if (item != null)
        //    //        {
        //    //            item.Visibility = control.VisibleInTreeView;

        //    //            item.Expanded -= TreeViewItem_Expanded;
        //    //            item.Expanded += TreeViewItem_Expanded;
        //    //        }
        //    //    }
        //    //}
        //}

        private static TreeViewItem FindTreeViewItem(object obj)
        {
            DependencyObject dpObj = obj as DependencyObject;
            if (dpObj == null)
                return null;
            if (dpObj is TreeViewItem)
                return (TreeViewItem)dpObj;
            return FindTreeViewItem(VisualTreeHelper.GetParent(dpObj));
        }

        private TreeViewItem FindTreeViewItem(TreeViewItem item, UISystem.WinBase control)
        {
            return item.ItemContainerGenerator.ContainerFromItem(control) as TreeViewItem;
        }

        private void ExpandAll()
        {
            //TreeViewItem item = VisualTreeHelper.GetChild(TreeView_UIControls, 0) as TreeViewItem;
            //ExpandAll(item);
        }
        private void ExpandAll(TreeViewItem tvItem)
        {
            if (tvItem != null)
            {
                tvItem.IsExpanded = true;
                tvItem.UpdateLayout();
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(tvItem); i++)
            {
                var item = VisualTreeHelper.GetChild(tvItem, i) as TreeViewItem;
                ExpandAll(item);
            }
        }

        private TreeViewItem FindTreeViewItem(ItemsControl item, object data)
        {
            TreeViewItem findItem = null;
            bool itemIsExpand = false;
            if (item is TreeViewItem)
            {
                TreeViewItem tviCurrent = item as TreeViewItem;
                itemIsExpand = tviCurrent.IsExpanded;
                if (!tviCurrent.IsExpanded)
                {
                    //如果这个TreeViewItem未展开过，则不能通过ItemContainerGenerator来获得TreeViewItem
                    tviCurrent.SetValue(TreeViewItem.IsExpandedProperty, true);
                    //必须使用UpdaeLayour才能获取到TreeViewItem
                    tviCurrent.UpdateLayout();
                }
            }
            for (int i = 0; i < item.Items.Count; i++)
            {
                TreeViewItem tvItem = (TreeViewItem)item.ItemContainerGenerator.ContainerFromIndex(i);
                if (tvItem == null)
                    continue;
                object itemData = item.Items[i];
                if (itemData == data)
                {
                    findItem = tvItem;
                    break;
                }
                else if (tvItem.Items.Count > 0)
                {
                    findItem = FindTreeViewItem(tvItem, data);
                    if (findItem != null)
                        break;
                }

            }
            if (findItem == null)
            {
                TreeViewItem tviCurrent = item as TreeViewItem;
                if (tviCurrent != null)
                {
                    tviCurrent.SetValue(TreeViewItem.IsExpandedProperty, itemIsExpand);
                    tviCurrent.UpdateLayout();
                }
            }
            return findItem;
        }

        private void ExpandAllTreeViewParent(UIEditor.WinBase winObj)
        {
            var item = FindTreeViewItem(TreeView_UIControls, winObj);
            if(item != null)
                item.BringIntoView();

            //if(winObj == null)
            //    return;

            //Queue<UISystem.WinBase> parentQueue = new Queue<UISystem.WinBase>();

            //while(winObj.Parent != null)
            //{
            //    var parent = winObj.Parent as UISystem.WinBase;
            //    parentQueue.Enqueue(parent);
            //    winObj = parent;
            //}

            //VisualTreeHelper.GetChild(TreeView_UIControls, 0);

            //for(int i=0; i<.Items.Count; i++)
            //{
                
            //}
            //foreach (TreeViewItem item in )
            //{
            //}
            
            //DependencyObject dpObj = obj as DependencyObject;
            //if (dpObj == null)
            //    return;
            //if (dpObj is TreeViewItem)
            //{
            //    TreeViewItem item = dpObj as TreeViewItem;
            //    if(item.HasItems && !item.IsExpanded)
            //        item.IsExpanded = true;
            //}
            //ExpandAllTreeViewParent(VisualTreeHelper.GetParent(obj));
        }

        private void TreeView_UIControls_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeViewItem = FindTreeViewItem(e.OriginalSource);
            if (treeViewItem == null)
                return;
        }

        private void SetSelection(UIEditor.WinBase win)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (SelectedItems.Contains(win))
                    SelectedItems.Remove(win);
                else
                    SelectedItems.Add(win);
            }
            else
            {
                UpdateSelectItems(null, SelectedItems);
                SelectedItems.Clear();
                SelectedItems.Add(win);
            }
        }

        bool mDragStart = true;
        Point mMouseLeftButtonDownPointInTreeView;
        private void TreeViewItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mMouseLeftButtonDownPointInTreeView = e.GetPosition(TreeView_UIControls);

                var treeViewItem = FindTreeViewItem(e.OriginalSource);
                if (treeViewItem == null)
                    return;

                var win = treeViewItem.Header as UIEditor.WinBase;

                //foreach (var ctrl in SelectedItems)
                //{
                //    ctrl.TreeViewItemForeground = this.FindResource("UnSelectedForeground") as Brush;
                //    ctrl.TreeViewItemBackground = this.FindResource("UnSelectedBackground") as Brush;
                //}

                if (SelectedItems.Contains(win))
                {
                    mDragOperationType = enDragOperationType.Selection;
                }
                else
                {
                    SetSelection(win);
                    //UpdateSelectItems(SelectedItems);
                }

                mDragStart = false;
            }
        }

        private void TreeViewItem_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(TreeView_UIControls);
                if ((pos - mMouseLeftButtonDownPointInTreeView).LengthSquared > 3)
                    mDragStart = true;

                if (mDragStart == true)
                {
                    var treeViewItem = FindTreeViewItem(e.OriginalSource);
                    if (treeViewItem == null)
                    {
                        return;
                    }

                    var win = treeViewItem.Header as UIEditor.WinBase;

                    if (win.UIWin.Parent != null && !SelectedItems.Contains(win) && SelectedItems.Count > 0)
                    {
                        if(TextBlock_MouseTips.Visibility != System.Windows.Visibility.Visible)
                            TextBlock_MouseTips.Visibility = System.Windows.Visibility.Visible;

                        var gridLoc = e.GetPosition(Grid_Main);
                        TextBlock_MouseTips.Margin = new Thickness(gridLoc.X + 20, gridLoc.Y, 0, 0);

                        string mPreText = "";
                        if (SelectedItems.Count > 1)
                        {
                            mPreText = SelectedItems.Count + "个对象";
                        }
                        else
                            mPreText = SelectedItems[0].NameInEditor;

                        var mousePoint = e.GetPosition(treeViewItem);
                        if (mousePoint.Y < win.TreeViewItemHeight * 0.3)
                        {
                            if (win.UIWin.Parent != null && ((UISystem.WinBase)win.UIWin.Parent).CanInsertChild())
                            {
                                win.UpInsertLineVisible = Visibility.Visible;
                                win.DownInsertLineVisible = Visibility.Collapsed;
                                win.ChildInsertLineVisible = Visibility.Collapsed;
                                mDragOperationType = enDragOperationType.InsertBefore;

                                TextBlock_MouseTips.Text = "将" + mPreText + "插入" + win.NameInEditor + "之前";
                            }
                            else
                            {
                                if (TextBlock_MouseTips.Visibility == System.Windows.Visibility.Visible)
                                    TextBlock_MouseTips.Visibility = System.Windows.Visibility.Hidden;
                            }
                        }
                        else if (mousePoint.Y > win.TreeViewItemHeight * 0.7)
                        {
                            if (win.UIWin.Parent != null && ((UISystem.WinBase)win.UIWin.Parent).CanInsertChild())
                            {
                                win.UpInsertLineVisible = Visibility.Collapsed;
                                win.DownInsertLineVisible = Visibility.Visible;
                                win.ChildInsertLineVisible = Visibility.Collapsed;
                                mDragOperationType = enDragOperationType.InsertAfter;

                                TextBlock_MouseTips.Text = "将" + mPreText + "插入" + win.NameInEditor + "之后";
                            }
                            else
                            {
                                if (TextBlock_MouseTips.Visibility == System.Windows.Visibility.Visible)
                                    TextBlock_MouseTips.Visibility = System.Windows.Visibility.Hidden;
                            }
                        }
                        else
                        {
                            //if (win.ContainerType != UISystem.WinBase.enContainerType.None &&
                            //    !(win.ContainerType == UISystem.WinBase.enContainerType.One && win.GetChildWinNumber() > 0))
                            if(win.UIWin.CanInsertChild())
                            {
                                win.UpInsertLineVisible = Visibility.Collapsed;
                                win.DownInsertLineVisible = Visibility.Collapsed;
                                win.ChildInsertLineVisible = Visibility.Visible;
                                mDragOperationType = enDragOperationType.InsertChild;

                                TextBlock_MouseTips.Text = "将" + mPreText + "插入" + win.NameInEditor + "之内";
                            }
                            else
                            {
                                if (TextBlock_MouseTips.Visibility == System.Windows.Visibility.Visible)
                                    TextBlock_MouseTips.Visibility = System.Windows.Visibility.Hidden;
                            }
                        }
                    }
                    else
                    {
                        if (TextBlock_MouseTips.Visibility == System.Windows.Visibility.Visible)
                            TextBlock_MouseTips.Visibility = System.Windows.Visibility.Hidden;
                    }
                }

            }
        }

        private void TreeViewItem_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeViewItem = FindTreeViewItem(e.OriginalSource);
            if (treeViewItem == null)
                return;

            var win = treeViewItem.Header as UIEditor.WinBase;
            TextBlock_MouseTips.Visibility = System.Windows.Visibility.Hidden;

            switch (mDragOperationType)
            {
                case enDragOperationType.Selection:
                    SetSelection(win);
                    break;

                case enDragOperationType.InsertAfter:
                    {
                        if (win.UIWin.Parent == null || win == mForm)
                            break;

                        if (!((UISystem.WinBase)win.UIWin.Parent).CanInsertChild())
                            break;

                        if (SelectedItems.Contains(win))
                            break;

                        //for (int i = SelectedItems.Count - 1; i >= 0; i--)
                        //foreach (var item in SelectedItems)
                        for (int i = mSelectItemIndexDatas.Count - 1; i >=0 ; i-- )
                        {
                            var item = mSelectItemIndexDatas[i].mControl;

                            if (item.UIWin.Parent == null)
                                continue;

                            item.UIWin.Parent = win.UIWin.Parent;

                            var winParent = win.UIWin.Parent as UISystem.WinBase;
                            var itemParent = item.UIWin.Parent as UISystem.WinBase;

                            var tempLeft = item.UIWin.AbsRect.Left - itemParent.AbsRect.Left;
                            var tempTop = item.UIWin.AbsRect.Top - itemParent.AbsRect.Top;
                            item.UIWin.Margin = item.UIWin.GetMargin(tempLeft, tempTop, item.UIWin.Width, item.UIWin.Height, itemParent);

                            var index = winParent.LogicChildren.IndexOf(win.UIWin);
                            var nowIndex = ((UISystem.WinBase)itemParent).LogicChildren.IndexOf(item.UIWin);
                            if (index + 1 >= winParent.LogicChildren.Count)
                                winParent.LogicChildren.Move(nowIndex, winParent.LogicChildren.Count - 1);
                            else if (nowIndex < index)
                                winParent.LogicChildren.Move(nowIndex, index);
                            else
                                winParent.LogicChildren.Move(nowIndex, index + 1);
                        }
                    }
                    break;

                case enDragOperationType.InsertBefore:
                    {
                        if (win.UIWin.Parent == null || win == mForm)
                            break;

                        var winParent = win.UIWin.Parent as UISystem.WinBase;

                        if (!winParent.CanInsertChild())
                            break;

                        if (SelectedItems.Contains(win))
                            break;

                        //for (int i = SelectedItems.Count - 1; i >= 0; i--)
                        //foreach (var item in SelectedItems)
                        for (int i = 0; i < mSelectItemIndexDatas.Count; i++)
                        {
                            var item = mSelectItemIndexDatas[i].mControl;

                            if (item.UIWin.Parent == null)
                                continue;

                            item.UIWin.Parent = win.UIWin.Parent;

                            var itemParent = item.UIWin.Parent as UISystem.WinBase;

                            var tempLeft = item.UIWin.AbsRect.Left - itemParent.AbsRect.Left;
                            var tempTop = item.UIWin.AbsRect.Top - itemParent.AbsRect.Top;
                            item.UIWin.Margin = item.UIWin.GetMargin(tempLeft, tempTop, item.UIWin.Width, item.UIWin.Height, itemParent);

                            var index = winParent.LogicChildren.IndexOf(win.UIWin);
                            var nowIndex = itemParent.LogicChildren.IndexOf(item.UIWin);
                            if (nowIndex > index)
                                winParent.LogicChildren.Move(nowIndex, index);
                            else
                                winParent.LogicChildren.Move(nowIndex, System.Math.Max(index - 1, 0));
                        }
                    }
                    break;

                case enDragOperationType.InsertChild:
                    {
                        //if (win.ContainerType == UISystem.WinBase.enContainerType.None)
                        //    break;
                        //if (win.ContainerType == UISystem.WinBase.enContainerType.One &&
                        //   win.GetChildWinNumber() > 0)
                        //    break;
                        if (!win.UIWin.CanInsertChild())
                            break;

                        if (SelectedItems.Contains(win))
                            break;

                        //for (int i = SelectedItems.Count - 1; i >= 0; i--)
                        //foreach (var item in SelectedItems)
                        for (int i = 0; i < mSelectItemIndexDatas.Count; i++)
                        {
                            var item = mSelectItemIndexDatas[i].mControl;

                            if (item.UIWin.Parent == null)
                                continue;

                            item.UIWin.Parent = win.UIWin;
                            var itemParent = item.UIWin.Parent as UISystem.WinBase;

                            //itemParent = win;
                            var tempLeft = item.UIWin.AbsRect.Left - itemParent.AbsRect.Left;
                            var tempTop = item.UIWin.AbsRect.Top - itemParent.AbsRect.Top;
                            item.UIWin.Margin = item.UIWin.GetMargin(tempLeft, tempTop, item.UIWin.Width, item.UIWin.Height, itemParent);
                        }

                        treeViewItem.IsExpanded = true;
                    }
                    break;
            }

            mDragStart = false;
            mDragOperationType = enDragOperationType.None;
        }

        private void TreeViewItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
        }

        private void TreeViewItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var treeViewItem = FindTreeViewItem(e.OriginalSource);
            if (treeViewItem == null)
                return;

            var win = treeViewItem.Header as UIEditor.WinBase;
            if (win != null)
            {
                win.UpInsertLineVisible = System.Windows.Visibility.Collapsed;
                win.DownInsertLineVisible = System.Windows.Visibility.Collapsed;
                win.ChildInsertLineVisible = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
