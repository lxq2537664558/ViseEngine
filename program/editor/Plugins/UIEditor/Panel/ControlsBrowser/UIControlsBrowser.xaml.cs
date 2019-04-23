using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UIEditor
{
    /// <summary>
    /// Interaction logic for UIControlsBrowser.xaml
    /// </summary>
    public partial class UIControlsBrowser : UserControl
    {
        //public delegate void Delegate_OnSelectedChanged(UIControlsBrowser_Item item);
        //public Delegate_OnSelectedChanged OnSelectedChanged;

        //TreeViewItem mSelectedTreeViewTemplateItem = null;

        // 所有控件列表
        ObservableCollection<IUIControlVM> mAllUIControlViews = new ObservableCollection<IUIControlVM>();
        // 显示的控件列表
        ObservableCollection<IUIControlVM> mShowUIControlViews = new ObservableCollection<IUIControlVM>();

        //int mListBoxControlsSelectIndex = -1;
        //public int ListBoxControlsSelectIndex
        //{
        //    get { return mListBoxControlsSelectIndex; }
        //    set
        //    {
        //        if (value < 0 || value >= ListBox_Controls.Items.Count)
        //            value = -1;

        //        mListBoxControlsSelectIndex = value;
        //        ListBox_Controls.SelectedIndex = mListBoxControlsSelectIndex;
        //    }
        //}

        //int mListBoxTemplatesSelectIndex = -1;
        //public int ListBoxTemplatesSelectIndex
        //{
        //    get { return mListBoxControlsSelectIndex; }
        //    set
        //    {
        //        if (value < 0 || value >= ListBox_Templates.Items.Count)
        //            value = -1;

        //        mListBoxTemplatesSelectIndex = value;
        //        ListBox_Templates.SelectedIndex = mListBoxTemplatesSelectIndex;
        //    }
        //}        

        public UIControlsBrowser()
        {
            InitializeComponent();
            InitControlsShow();
        }

        public void UnselectAll()
        {
            //    ListBox_Controls.SelectedIndex = -1;
            //    //ListBox_Templates.SelectedIndex = -1;
            //    //if(TreeView_Templates.SelectedItem != null)
            //    //{
            //    //    TreeViewItem.ItemContainerGenerator.ContainerFromItem(TreeView_Templates.SelectedItem) as TreeViewItem;
            //    //}
            //    //TreeViewItem item;
            //    //item.sele
            //    if (mSelectedTreeViewTemplateItem != null)
            //        mSelectedTreeViewTemplateItem.IsSelected = false;
        }

        private void InitControlsShow()
        {
            foreach(var assembly in CSUtility.Program.GetAnalyseAssemblys(CSUtility.Helper.enCSType.Client))
            {
                foreach (var type in assembly.GetTypes())
                {
                    var atts = type.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_ControlAttribute), true);
                    if (atts.Length > 0)
                    {
                        CSUtility.Editor.UIEditor_ControlAttribute ctrlAtt = atts[0] as CSUtility.Editor.UIEditor_ControlAttribute;

                        //UIControlsBrowser_Item item = new UIControlsBrowser_Item();
                        //item.ControlName = ctrlAtt.Name;
                        //item.TargetType = type;
                        List<string> path = new List<string>(ctrlAtt.Name.Split('.'));

                        InitControlsShow(path, type, null);
                    }
                }
            }

            TreeView_Items.ItemsSource = mAllUIControlViews;
        }

        private void InitControlsShow(List<string> path, Type controlType, IUIControlVM parent)
        {
            ObservableCollection<IUIControlVM> items = mAllUIControlViews;
            if (parent != null)
                items = parent.Children;

            if (path.Count == 1)
            {
                // 叶子节点
                var uivm = new UIControlViewModel(controlType);
                items.Add(uivm);
            }
            else
            {
                var pt = path[0];
                path.RemoveAt(0);

                foreach(var item in items)
                {
                    if(item.ControlName.Equals(pt))
                    {
                        InitControlsShow(path, controlType, item);
                        return;
                    }
                }

                var uivm = new UIControlViewModelParent();
                uivm.ControlName = pt;
                items.Add(uivm);
                InitControlsShow(path, controlType, uivm);
            }
        }

        //private void ListBox_Controls_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    if (ListBox_Controls.SelectedIndex < 0)
        //        return;

        //    //ListBox_Templates.SelectedIndex = -1;
        //    //TreeView_Templates.Items.
        //    if (mSelectedTreeViewTemplateItem != null)
        //        mSelectedTreeViewTemplateItem.IsSelected = false;

        //    if (OnSelectedChanged != null)
        //        OnSelectedChanged((UIControlsBrowser_Item)ListBox_Controls.SelectedItem);
        //}

        public void SelectTemplateIndex(int index)
        {
            //if (index < 0 || index >= ListBox_Controls.Items.Count)
            //    return;

            //ListBox_Controls.SelectedIndex = index;
        }

        //private void ListBox_Templates_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    if (ListBox_Templates.SelectedIndex < 0)
        //        return;

        //    ListBox_Controls.SelectedIndex = -1;

        //    if (OnSelectedChanged != null)
        //        OnSelectedChanged((UIControlsBrowser_Item)ListBox_Templates.SelectedItem);
        //}
        private static TreeViewItem FindTreeViewItem(object obj)
        {
            try
            {
                DependencyObject dpObj = obj as DependencyObject;
                if (dpObj == null)
                    return null;
                if (dpObj is TreeViewItem)
                    return (TreeViewItem)dpObj;
                return FindTreeViewItem(VisualTreeHelper.GetParent(dpObj));
            }
            catch (Exception)
            {
                return null;
            }
        }

        Point mMouseLeftButtonDownPointInTreeView;
        bool mMouseDown = false;
        private void TreeViewItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var elm = sender as FrameworkElement;
                Mouse.Capture(elm);
                mMouseDown = true;
                mMouseLeftButtonDownPointInTreeView = e.GetPosition(elm);
            }
        }
        private void TreeViewItem_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && mMouseDown)
            {
                var elm = sender as FrameworkElement;
                var pt = e.GetPosition(elm);
                var length = System.Math.Sqrt((pt.X - mMouseLeftButtonDownPointInTreeView.X) * (pt.X - mMouseLeftButtonDownPointInTreeView.X) +
                                              (pt.Y - mMouseLeftButtonDownPointInTreeView.Y) * (pt.Y - mMouseLeftButtonDownPointInTreeView.Y));
                if(length > 5)
                {
                    var content = VisualTreeHelper.GetParent(elm) as System.Windows.Controls.ContentPresenter;
                    var uiCVM = content.Content as UIControlViewModel;
                    //if (uiCVM.DragVisual == null)
                    //{
                    //    var p = VisualTreeHelper.GetParent(content);
                    //    while(p != null)
                    //    {
                    //        if (p is TreeViewItem)
                    //            break;

                    //        p = VisualTreeHelper.GetParent(p);
                    //    }

                    //    if (p != null)
                    //        uiCVM.DragVisual = p as FrameworkElement;
                    //    else
                    //        uiCVM.DragVisual = elm;

                    //}

                    EditorCommon.DragDrop.DragDropManager.Instance.StartDrag(UIEditor.Program.ControlDragType, new EditorCommon.DragDrop.IDragAbleObject[] { uiCVM }, "创建" + uiCVM.ControlName);
                    mMouseDown = false;
                    Mouse.Capture(null);
                }
            }
        }
        private void TreeViewItem_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mMouseDown = false;
            Mouse.Capture(null);
            //if (mMouseDown)
            //{
            //    mMouseLeftButtonDownPointInTreeView = e.GetPosition(TreeView_Items);

            //    object itemObj = e.OriginalSource;
            //    if (e.OriginalSource is System.Windows.Documents.Run)
            //    {
            //        itemObj = ((System.Windows.Documents.Run)e.OriginalSource).Parent;
            //    }
            //    var treeViewItem = FindTreeViewItem(itemObj);
            //    if (treeViewItem == null)
            //        return;

            //    var actorVM = treeViewItem.Header as UIControlViewModel;
            //    SetSelection(actorVM);

            //    if (mMouseClickCount >= 2)
            //    {
            //        List<CCore.World.Actor> focusActors = new List<CCore.World.Actor>();
            //        foreach (var avm in SelectedItems)
            //        {
            //            focusActors.Add(avm.Actor);
            //        }
            //        // 双击定位选中的对象
            //        EditorCommon.GameActorOperation.FocusActors(focusActors);
            //    }
            //}

            //mMouseDown = false;
            //mMouseClickCount = 0;
        }
    }
}
