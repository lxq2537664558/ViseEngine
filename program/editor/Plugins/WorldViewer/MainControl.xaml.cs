using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;

namespace WorldViewer
{
    /// <summary>
    /// MainControl.xaml 的交互逻辑
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "WorldViewer")]
    [EditorCommon.PluginAssist.PluginMenuItem("窗口/世界浏览器")]
    [Guid("340AEE3A-1002-4DFD-BC72-AF4220AEA17D")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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

        public string PluginName
        {
            get { return "世界浏览器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "提供对地图中对象的浏览和编辑功能",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void SetObjectToEdit(object[] obj)
        {

        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        public void Tick()
        {
        }

        ///////////////////////////////////////////////////////////

        #region Filter
            
        string mFilterString = "";
        public string FilterString
        {
            get { return mFilterString; }
            set
            {
                mFilterString = value;

                mFilterCount = 0;
                mSelectCount = 0;
                mShowActorViews.Clear();
                ShowItemWithFilter(mAllActorViews, ref mShowActorViews, mFilterString);
                UpdateCountInfo();
                TreeView_Items.ItemsSource = mShowActorViews;

                OnPropertyChanged("FilterString");
            }
        }

        bool ShowItemWithFilter(ObservableCollection<ActorViewModel> srcActorViews, ref ObservableCollection<ActorViewModel> tagActorViews, string filterString)
        {
            tagActorViews.Clear();

            if (string.IsNullOrEmpty(filterString))
            {
                tagActorViews = new ObservableCollection<ActorViewModel>(srcActorViews);
                foreach(var tav in tagActorViews)
                {
                    tav.HighLightString = "";
                }
                mFilterCount = 0;
                return true;
            }

            bool retValue = false;
            for(int i=0; i<srcActorViews.Count; i++)
            {
                var avm = srcActorViews[i];
                ActorViewModel tagAvm = null;
                if (avm.ActorName.IndexOf(filterString, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    tagAvm = new ActorViewModel(avm.Actor);
                    tagActorViews.Add(tagAvm);
                    tagAvm.HighLightString = filterString;
                    mFilterCount++;
                    retValue = true;
                }

                if(avm.Children.Count > 0)
                {
                    ObservableCollection<ActorViewModel> children = new ObservableCollection<ActorViewModel>();
                    var bFind = ShowItemWithFilter(avm.Children, ref children, filterString);
                    if(bFind)
                    {
                        if (tagAvm == null)
                        {
                            tagAvm = new ActorViewModel(avm.Actor);
                            tagActorViews.Add(tagAvm);
                            tagAvm.HighLightString = filterString;
                            mFilterCount++;
                        }
                        tagAvm.Children = children;
                        retValue = true;
                    }
                }
            }

            return retValue;
        }
        
        #endregion // Filter

        int mFilterCount = 0;
        int mSelectCount = 0;
        string mCountInfo = "";
        public string CountInfo
        {
            get { return mCountInfo; }
            private set
            {
                mCountInfo = value;
                OnPropertyChanged("CountInfo");
            }
        }
        private void UpdateCountInfo()
        {
            CountInfo = mAllActorDic.Count + "个对象";
            if (mFilterCount > 0 && mFilterCount != mAllActorDic.Count)
                CountInfo = "显示" + CountInfo + "中的" + mFilterCount + "个";
            if(mSelectCount > 0)
            {
                CountInfo += "选中" + mSelectCount + "个";
            }
        }


        // 包含所有Actor的字典表
        Dictionary<Guid, ActorViewModel> mAllActorDic = new Dictionary<Guid, ActorViewModel>();
        // 按照树形结构存储的所有Actor列表
        ObservableCollection<ActorViewModel> mAllActorViews = new ObservableCollection<ActorViewModel>();
        // 显示在Treeview上的Actor列表
        ObservableCollection<ActorViewModel> mShowActorViews = new ObservableCollection<ActorViewModel>();

        public MainControl()
        {
            InitializeComponent();

            EditorCommon.GameActorOperation.OnSelectActors += GameActorOperation_OnSelectActors;
            EditorCommon.GameActorOperation.OnUnSelectActors += GameActorOperation_OnUnSelectActors;
            
            mFilterCount = 0;
            mSelectCount = 0;
            TreeView_Items.ItemsSource = mShowActorViews;
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;

            mShowActorViews.CollectionChanged += MActorViews_CollectionChanged;

            OnWorldLoaded();
            CCore.Program.OnWorldLoaded += Program_OnWorldLoaded;
        }

        private void GameActorOperation_OnSelectActors(List<CCore.World.Actor> actors)
        {
            if (actors == null)
                return;

            foreach(var actor in actors)
            {
                if (actor == null)
                    continue;
                ActorViewModel avm;
                if(mAllActorDic.TryGetValue(actor.Id, out avm))
                {
                    SelectedControl(avm);
                }
            }
        }
        private void GameActorOperation_OnUnSelectActors(List<CCore.World.Actor> actors)
        {
            if (actors == null)
                return;

            foreach (var actor in actors)
            {
                if (actor == null)
                    continue;
                ActorViewModel avm;
                if (mAllActorDic.TryGetValue(actor.Id, out avm))
                {
                    UnSelectedControl(avm);
                }
            }
        }

        private void MActorViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //UpdateCountInfo();
        }

        private void Program_OnWorldLoaded(System.String strAbsFolder, string componentName, CCore.World.World world)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (componentName)
                {
                    case "场景":
                        OnWorldLoaded();
                        break;
                }
            });
        }
        private void OnWorldLoaded()
        {
            //if (CCore.Client.MainWorldInstance.IsNullWorld)
            //    return;

            //UpdateActorList();
        }

        private void UpdateActorList()
        {
            lock(this)
            {
                var actors = CCore.Client.MainWorldInstance.GetActors(0);
                mAllActorDic.Clear();
                mAllActorViews.Clear();
                foreach (var actor in actors)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        var actorV = new ActorViewModel(actor);
                        mAllActorDic[actor.Id] = actorV;
                        // todo: 这里要按照树形结构存储
                        mAllActorViews.Add(actorV);
                    });
                }

                /* 大列表性能测试
                for(int i=0; i<100000; i++)
                {
                    var actInit = new CCore.World.ActorInit();
                    CCore.World.Actor actor = new CCore.World.Actor();
                    actor.Initialize(actInit);
                    actor.ActorName = "Actor " + i;
                    var actorV = new ActorViewModel(actor);

                    mAllActorViews.Add(actorV);
                }*/
                mShowActorViews = new ObservableCollection<ActorViewModel>(mAllActorViews);
                this.Dispatcher.Invoke(new Action<ObservableCollection<ActorViewModel>>((actorViews) =>
                {
                    mFilterCount = 0;
                    mSelectCount = 0;
                    ShowItemWithFilter(actorViews, ref mShowActorViews, mFilterString);
                    TreeView_Items.ItemsSource = mShowActorViews;
                    UpdateCountInfo();
                }), new object[] { mAllActorViews });
            }

        }

        #region 对象刷新计时
        System.Timers.Timer mRefreshActorTimer;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mRefreshActorTimer = new System.Timers.Timer()
            {
                Interval = 1000,
                Enabled = false,
            };
            mRefreshActorTimer.Elapsed += RefreshActorTimer_Elapsed;
            mRefreshActorTimer.Enabled = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            mRefreshActorTimer.Stop();
            mRefreshActorTimer.Enabled = false;
            mRefreshActorTimer.Dispose();
            mRefreshActorTimer = null;
        }
        
        private void RefreshActorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var actors = CCore.Client.MainWorldInstance.GetActors(0);
            if(actors.Count != mAllActorDic.Count)
            {
                // 需要刷新树形列表
                UpdateActorList();
            }
        }
        #endregion

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
        private TreeViewItem FindTreeViewItem(TreeViewItem item, UISystem.WinBase control)
        {
            return item.ItemContainerGenerator.ContainerFromItem(control) as TreeViewItem;
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
                if (itemData.Equals(data))
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

        #region TreeView选择

        public ObservableCollection<ActorViewModel> SelectedItems
        {
            get { return (ObservableCollection<ActorViewModel>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<ActorViewModel>), typeof(MainControl), new FrameworkPropertyMetadata(new ObservableCollection<ActorViewModel>(), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSelectedItemsChanged)));

        public static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainControl control = d as MainControl;

            control.SelectedItems.CollectionChanged += control.SelectedItems_CollectionChanged;

            control.UpdateSelectItems((ObservableCollection<ActorViewModel>)e.NewValue, new ObservableCollection<ActorViewModel>());
        }

        ObservableCollection<ActorViewModel> mOldSelections = new ObservableCollection<ActorViewModel>();

        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        UpdateSelectItems(null, mOldSelections);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        ObservableCollection<ActorViewModel> selItems = new ObservableCollection<ActorViewModel>(mOldSelections);
                        foreach (ActorViewModel avm in e.NewItems)
                        {
                            selItems.Add(avm);
                        }
                        UpdateSelectItems(selItems, null);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        ObservableCollection<ActorViewModel> unSelItems = new ObservableCollection<ActorViewModel>();
                        foreach (ActorViewModel avm in e.OldItems)
                        {
                            unSelItems.Add(avm);
                        }
                        UpdateSelectItems(null, unSelItems);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    {
                        ObservableCollection<ActorViewModel> selItems = new ObservableCollection<ActorViewModel>();
                        ObservableCollection<ActorViewModel> unSelItems = new ObservableCollection<ActorViewModel>();
                        foreach (ActorViewModel avm in e.NewItems)
                        {
                            selItems.Add(avm);
                        }
                        foreach (ActorViewModel avm in e.OldItems)
                        {
                            unSelItems.Add(avm);
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

        ActorViewModel mLastSelectedActorViewModel = null;
        private void SelectedControl(ActorViewModel avm)
        {
            if (avm == null || avm.IsSelected)
                return;

            var item = FindTreeViewItem(TreeView_Items, avm);
            if (item != null)
                item.BringIntoView();

            avm.TreeViewItemBackground = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TreeViewItemSelectedBackground")) as Brush;
            avm.TreeViewItemForeground = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TreeViewItemSelectedForeground")) as Brush;
            avm.IsSelected = true;
            if (mLastSelectedActorViewModel != null)
            {
                mLastSelectedActorViewModel.Actor.Placement.OnLocationChanged -= LastSelectedActorPlacement_OnLocationChanged;
                mLastSelectedActorViewModel.Actor.Placement.OnRotationChanged -= LastSelectedActorPlacement_OnRotationChanged;
                mLastSelectedActorViewModel.Actor.Placement.OnScaleChanged -= LastSelectedActorPlacement_OnScaleChanged;
            }
            mLastSelectedActorViewModel = avm;
            mLastSelectedActorViewModel.Actor.Placement.OnLocationChanged += LastSelectedActorPlacement_OnLocationChanged;
            mLastSelectedActorViewModel.Actor.Placement.OnRotationChanged += LastSelectedActorPlacement_OnRotationChanged;
            mLastSelectedActorViewModel.Actor.Placement.OnScaleChanged += LastSelectedActorPlacement_OnScaleChanged;
            mSelectCount++;

            BindingOperations.ClearBinding(TextBox_Name, TextBox.TextProperty);
            BindingOperations.SetBinding(TextBox_Name, TextBox.TextProperty, new Binding("ActorName") { Source = avm, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            PropertyGrid_Actor.Instance = avm.Actor?.GetShowPropertyObj();

            UpdateTransShow();
            UpdateCountInfo();
        }
        
        private void UnSelectedControl(ActorViewModel avm)
        {
            if (avm == null || !avm.IsSelected)
                return;

            avm.TreeViewItemForeground = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TreeViewItemNormalForeground")) as Brush;
            avm.TreeViewItemBackground = this.TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "TreeViewItemNormalBackground")) as Brush;
            avm.IsSelected = false;
            mSelectCount--;
            if (mSelectCount <= 0)
            {
                mSelectCount = 0;
                if (mLastSelectedActorViewModel != null)
                {
                    mLastSelectedActorViewModel.Actor.Placement.OnLocationChanged -= LastSelectedActorPlacement_OnLocationChanged;
                    mLastSelectedActorViewModel.Actor.Placement.OnRotationChanged -= LastSelectedActorPlacement_OnRotationChanged;
                    mLastSelectedActorViewModel.Actor.Placement.OnScaleChanged -= LastSelectedActorPlacement_OnScaleChanged;
                }
                mLastSelectedActorViewModel = null;
            }

            UpdateTransShow();
            UpdateCountInfo();
        }

        private void UpdateTransShow()
        {
            if (EditorCommon.Program.Game3dAxis != null && mLastSelectedActorViewModel != null)
            {
                var loc = EditorCommon.Program.Game3dAxis.Placement.GetLocation();
                mPositionX = loc.X;
                OnPropertyChanged("PositionX");
                mPositionY = loc.Y;
                OnPropertyChanged("PositionY");
                mPositionZ = loc.Z;
                OnPropertyChanged("PositionZ");
                var rot = EditorCommon.Program.Game3dAxis.Placement.GetRotation();
                float yaw, pitch, roll;
                rot.GetYawPitchRoll(out yaw, out pitch, out roll);
                mRotationX = (float)(pitch / System.Math.PI * 180);
                OnPropertyChanged("RotationX");
                mRotationY = (float)(yaw / System.Math.PI * 180);
                OnPropertyChanged("RotationY");
                mRotationZ = (float)(roll / System.Math.PI * 180);
                OnPropertyChanged("RotationZ");
                var scale = mLastSelectedActorViewModel.Actor.Placement.GetScale();
                mScaleX = scale.X;
                OnPropertyChanged("ScaleX");
                mScaleY = scale.Y;
                OnPropertyChanged("ScaleY");
                mScaleZ = scale.Z;
                OnPropertyChanged("ScaleZ");
            }
            else
            {
                mPositionX = 0;
                OnPropertyChanged("PositionX");
                mPositionY = 0;
                OnPropertyChanged("PositionY");
                mPositionZ = 0;
                OnPropertyChanged("PositionZ");
                mRotationX = 0;
                OnPropertyChanged("RotationX");
                mRotationY = 0;
                OnPropertyChanged("RotationY");
                mRotationZ = 0;
                OnPropertyChanged("RotationZ");
                mScaleX = 1;
                OnPropertyChanged("ScaleX");
                mScaleY = 1;
                OnPropertyChanged("ScaleY");
                mScaleZ = 1;
                OnPropertyChanged("ScaleZ");
            }
        }

        public void UpdateSelectItems(ObservableCollection<ActorViewModel> selectedItems, ObservableCollection<ActorViewModel> unSelectedItems)//bool bUpdateDrawPanel)
        {
            if (unSelectedItems != null)
            {
                var actorList = new List<CCore.World.Actor>();
                foreach (var ctrl in unSelectedItems)
                {
                    actorList.Add(ctrl.Actor);
                }

                EditorCommon.GameActorOperation.UnSelectActor(actorList);
                foreach (var ctrl in unSelectedItems)
                {
                    UnSelectedControl(ctrl);
                }
            }

            if (selectedItems != null)
            {
                var actorList = new List<CCore.World.Actor>();
                foreach (var ctrl in selectedItems)
                {
                    actorList.Add(ctrl.Actor);
                }

                EditorCommon.GameActorOperation.SelectActors(actorList);
                foreach (var ctrl in selectedItems)
                {
                    SelectedControl(ctrl);
                }
            }
        }
        private void SetSelection(ActorViewModel avm)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (SelectedItems.Contains(avm))
                    SelectedItems.Remove(avm);
                else
                    SelectedItems.Add(avm);
            }
            else
            {
                UpdateSelectItems(null, SelectedItems);
                SelectedItems.Clear();
                SelectedItems.Add(avm);
            }
        }

        Point mMouseLeftButtonDownPointInTreeView;
        bool mMouseDown = false;
        int mMouseClickCount = 0;
        private void TreeViewItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                mMouseDown = true;
                mMouseClickCount = e.ClickCount;
            }
            e.Handled = true;
        }
        private void TreeViewItem_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }
        private void TreeViewItem_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(mMouseDown)
            {
                mMouseLeftButtonDownPointInTreeView = e.GetPosition(TreeView_Items);

                object itemObj = e.OriginalSource;
                if(e.OriginalSource is System.Windows.Documents.Run)
                {
                    itemObj = ((System.Windows.Documents.Run)e.OriginalSource).Parent;
                }
                var treeViewItem = FindTreeViewItem(itemObj);
                if (treeViewItem == null)
                    return;

                var actorVM = treeViewItem.Header as ActorViewModel;
                SetSelection(actorVM);

                if(mMouseClickCount >= 2)
                {
                    List<CCore.World.Actor> focusActors = new List<CCore.World.Actor>();
                    foreach(var avm in SelectedItems)
                    {
                        focusActors.Add(avm.Actor);
                    }
                    // 双击定位选中的对象
                    EditorCommon.GameActorOperation.FocusActors(focusActors);
                }
            }

            mMouseDown = false;
            mMouseClickCount = 0;
        }

        #endregion

        #region PlacementOperation

        float mPositionX = 0;
        public float PositionX
        {
            get { return mPositionX; }
            set
            {
                if (System.Math.Abs(mPositionX - value) < 0.0001f)
                    return;

                var oldPosition = mPositionX;
                mPositionX = value;

                var loc = new SlimDX.Vector3(mPositionX - oldPosition, 0, 0);
                EditorCommon.Program.Game3dAxis.SetLocationDeltaWithTargets(ref loc);

                OnPropertyChanged("PositionX");
            }
        }
        float mPositionY = 0;
        public float PositionY
        {
            get { return mPositionY; }
            set
            {
                if (System.Math.Abs(mPositionY - value) < 0.0001f)
                    return;

                var oldPosition = mPositionY;
                mPositionY = value;

                var loc = new SlimDX.Vector3(0, mPositionY - oldPosition, 0);
                EditorCommon.Program.Game3dAxis.SetLocationDeltaWithTargets(ref loc);

                OnPropertyChanged("PositionY");
            }
        }
        float mPositionZ = 0;
        public float PositionZ
        {
            get { return mPositionZ; }
            set
            {
                if (System.Math.Abs(mPositionZ - value) < 0.0001f)
                    return;

                var oldPosition = mPositionZ;
                mPositionZ = value;
                
                var loc = new SlimDX.Vector3(0, 0, mPositionZ - oldPosition);
                EditorCommon.Program.Game3dAxis.SetLocationDeltaWithTargets(ref loc);

                OnPropertyChanged("PositionZ");
            }
        }

        float mRotationX = 0;
        public float RotationX
        {
            get { return mRotationX; }
            set
            {
                if (System.Math.Abs(mRotationX - value) < 0.0001f)
                    return;

                var oldRotation = mRotationX;
                mRotationX = value;
                var rotX = (float)((mRotationX - oldRotation) / 180.0 * System.Math.PI);

                var rot = SlimDX.Quaternion.RotationYawPitchRoll(0, rotX, 0);
                var mat = SlimDX.Matrix.RotationQuaternion(rot);
                EditorCommon.Program.Game3dAxis.SetRotationDeltaWithTargets(ref mat);

                OnPropertyChanged("RotationX");
            }
        }
        float mRotationY = 0;
        public float RotationY
        {
            get { return mRotationY; }
            set
            {
                if (System.Math.Abs(mRotationY - value) < 0.0001f)
                    return;

                var oldRotation = mRotationY;
                mRotationY = value;
                var rotY = (float)((mRotationY - oldRotation) / 180.0 * System.Math.PI);

                var rot = SlimDX.Quaternion.RotationYawPitchRoll(rotY, 0, 0);
                var mat = SlimDX.Matrix.RotationQuaternion(rot);
                EditorCommon.Program.Game3dAxis.SetRotationDeltaWithTargets(ref mat);
                
                OnPropertyChanged("RotationY");
            }
        }
        float mRotationZ = 0;
        public float RotationZ
        {
            get { return mRotationZ; }
            set
            {
                if (System.Math.Abs(mRotationZ - value) < 0.0001f)
                    return;

                var oldRotation = mRotationZ;
                mRotationZ = value;
                var rotZ = (float)((mRotationZ - oldRotation) / 180.0 * System.Math.PI);

                var rot = SlimDX.Quaternion.RotationYawPitchRoll(0, 0, rotZ);
                var mat = SlimDX.Matrix.RotationQuaternion(rot);
                EditorCommon.Program.Game3dAxis.SetRotationDeltaWithTargets(ref mat);

                OnPropertyChanged("RotationZ");
            }
        }

        bool mLockXYZ = true;
        public bool LockXYZ
        {
            get { return mLockXYZ; }
            set
            {
                mLockXYZ = value;
                OnPropertyChanged("LockXYZ");
            }
        }

        float mScaleX = 0;
        public float ScaleX
        {
            get { return mScaleX; }
            set
            {
                if (System.Math.Abs(mScaleX - value) < 0.0001f)
                    return;

                var oldValue = mScaleX;
                mScaleX = value;
                var slDelta = (oldValue != 0) ? (mScaleX / oldValue) : 1;
                if(LockXYZ)
                {
                    mScaleY = mScaleX;
                    OnPropertyChanged("ScaleY");
                    mScaleZ = mScaleX;
                    OnPropertyChanged("ScaleZ");
                    var delta = new SlimDX.Vector3(slDelta, slDelta, slDelta);
                    EditorCommon.Program.Game3dAxis.SetScaleDeltaWithTargets(ref delta);
                }
                else
                {
                    var delta = new SlimDX.Vector3(slDelta, 1, 1);
                    EditorCommon.Program.Game3dAxis.SetScaleDeltaWithTargets(ref delta);
                }
            }
        }
        float mScaleY = 0;
        public float ScaleY
        {
            get { return mScaleY; }
            set
            {
                if (System.Math.Abs(mScaleY - value) < 0.0001f)
                    return;

                var oldValue = mScaleY;
                mScaleY = value;
                var slDelta = (oldValue != 0) ? (mScaleY / oldValue) : 1;
                if(LockXYZ)
                {
                    mScaleX = mScaleY;
                    OnPropertyChanged("ScaleX");
                    mScaleZ = mScaleY;
                    OnPropertyChanged("ScaleZ");
                    var delta = new SlimDX.Vector3(slDelta, slDelta, slDelta);
                    EditorCommon.Program.Game3dAxis.SetScaleDeltaWithTargets(ref delta);
                }
                else
                {
                    var delta = new SlimDX.Vector3(1, slDelta, 1);
                    EditorCommon.Program.Game3dAxis.SetScaleDeltaWithTargets(ref delta);
                }
            }
        }
        float mScaleZ = 0;
        public float ScaleZ
        {
            get { return mScaleZ; }
            set
            {
                if (System.Math.Abs(mScaleZ - value) < 0.0001f)
                    return;

                var oldValue = mScaleZ;
                mScaleZ = value;
                var slDelta = (oldValue != 0) ? (mScaleZ / oldValue) : 1;
                if (LockXYZ)
                {
                    mScaleX = mScaleZ;
                    OnPropertyChanged("ScaleX");
                    mScaleY = mScaleZ;
                    OnPropertyChanged("ScaleY");
                    var delta = new SlimDX.Vector3(slDelta, slDelta, slDelta);
                    EditorCommon.Program.Game3dAxis.SetScaleDeltaWithTargets(ref delta);
                }
                else
                {
                    var delta = new SlimDX.Vector3(1, 1, slDelta);
                    EditorCommon.Program.Game3dAxis.SetScaleDeltaWithTargets(ref delta);
                }
            }
        }

        private void LastSelectedActorPlacement_OnLocationChanged(ref SlimDX.Vector3 loc)
        {
            mPositionX = loc.X;
            OnPropertyChanged("PositionX");
            mPositionY = loc.Y;
            OnPropertyChanged("PositionY");
            mPositionZ = loc.Z;
            OnPropertyChanged("PositionZ");
        }
        private void LastSelectedActorPlacement_OnRotationChanged(ref SlimDX.Quaternion rot)
        {
            float yaw, pitch, roll;
            rot.GetYawPitchRoll(out yaw, out pitch, out roll);
            mRotationX = (float)(pitch / System.Math.PI * 180);
            OnPropertyChanged("RotationX");
            mRotationY = (float)(yaw / System.Math.PI * 180);
            OnPropertyChanged("RotationY");
            mRotationZ = (float)(roll / System.Math.PI * 180);
            OnPropertyChanged("RotationZ");
        }
        private void LastSelectedActorPlacement_OnScaleChanged(ref SlimDX.Vector3 scale)
        {
            mScaleX = scale.X;
            OnPropertyChanged("ScaleX");
            mScaleY = scale.Y;
            OnPropertyChanged("ScaleY");
            mScaleZ = scale.Z;
            OnPropertyChanged("ScaleZ");
        }

        #endregion
    }
}
