using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UIEditor
{
    public class WinBase : INotifyPropertyChanged
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

        public static WinBase GetHostWin(UISystem.WinBase uiWin)
        {
            if (uiWin.HostEditorWin != null)
                return uiWin.HostEditorWin as WinBase;

            return new WinBase(uiWin);
        }

        public static UISystem.WinBase GetAvailableUIWin(WinBase win)
        {
            return GetAvailableUIWin(win.UIWin);
        }

        public static UISystem.WinBase GetAvailableUIWin(UISystem.WinBase win)
        {
            var parent = win.Parent as UISystem.WinBase;
            if (parent == null)
                return win;

            if (parent.LogicChildren.Contains(win))
            {
                return win;
            }

            var pp = parent.Parent as UISystem.WinBase;
            while (pp != null)
            {
                if (pp.LogicChildren.Contains(parent))
                    return parent;

                parent = pp;
                pp = parent.Parent as UISystem.WinBase;
            }

            return win;
        }

        public WinBase(UISystem.WinBase win)
        {
            mUIWin = win;
            mUIWin.HostEditorWin = this;
            LogicChildren.Clear();

            if (mUIWin != null)
            {
                mUIWin.LogicChildren.CollectionChanged += LogicChildren_CollectionChanged;

                foreach (var child in mUIWin.LogicChildren)
                {
                    var edWin = new WinBase(child);
                    LogicChildren.Add(edWin);
                }

                try
                {
                    Icon = new BitmapImage(new Uri("pack://application:,,,/UIEditor;component/Source/ControlIcons/" + mUIWin.GetType().Name + ".png"));
                }
                catch (System.Exception)
                {
                    // 部分控件可能找不到图标，这里做一下异常捕获
                }
            }
        }

        ~WinBase()
        {
            mUIWin.LogicChildren.CollectionChanged -= LogicChildren_CollectionChanged;
        }

        protected UISystem.WinBase mUIWin = null;
        public UISystem.WinBase UIWin
        {
            get { return mUIWin; }
        }

        void LogicChildren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    {
                        LogicChildren.Clear();
                    }
                    break;

                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (UISystem.WinBase win in e.NewItems)
                        {
                            LogicChildren.Add(new WinBase(win));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (UISystem.WinBase win in e.OldItems)
                        {
                            var edWin = win.HostEditorWin as WinBase;
                            if(edWin != null)
                                LogicChildren.Remove(edWin);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.NewItems != null)
                        {
                            foreach (UISystem.WinBase win in e.NewItems)
                            {
                                LogicChildren.Add(new WinBase(win));
                            }
                        }
                        if (e.OldItems != null)
                        {
                            foreach (UISystem.WinBase win in e.OldItems)
                            {
                                var edWin = win.HostEditorWin as WinBase;
                                if (edWin != null)
                                    LogicChildren.Remove(edWin);
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    {
                        LogicChildren.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    break;
            }
        }

        [Browsable(false)]
        public double TreeViewItemHeight
        {
            get
            {
                if (UIWin != null)
                    return UIWin.TreeViewItemHeight;
                return 25;
            }
            set
            {
                if(UIWin != null)
                    UIWin.TreeViewItemHeight = value;
            }
        }

        [Browsable(false)]
        public bool IsVisibleInEditor
        {
            get
            {
                if (UIWin != null)
                    return UIWin.IsVisibleInEditor;
                return true;
            }
            set
            {
                if(UIWin != null)
                    UIWin.IsVisibleInEditor = value;

                OnPropertyChanged("IsVisibleInEditor");
            }
        }

        ImageSource mIcon = null;
        [Browsable(false)]
        public ImageSource Icon
        {
            get { return mIcon; }
            set
            {
                mIcon = value;
                OnPropertyChanged("Icon");
            }
        }

        [Browsable(false)]
        public string NameInEditor
        {
            get
            {
                if (UIWin != null)
                    return UIWin.NameInEditor;
                return "";
            }
            set
            {
                if (UIWin != null)
                    UIWin.NameInEditor = value;
                OnPropertyChanged("NameInEditor");
            }
        }

        System.Windows.Visibility mUpInsertLineVisible = System.Windows.Visibility.Collapsed;
        [Browsable(false)]
        public System.Windows.Visibility UpInsertLineVisible
        {
            get { return mUpInsertLineVisible; }
            set
            {
                mUpInsertLineVisible = value;
                OnPropertyChanged("UpInsertLineVisible");
            }
        }

        System.Windows.Visibility mDownInsertLineVisible = System.Windows.Visibility.Collapsed;
        [Browsable(false)]
        public System.Windows.Visibility DownInsertLineVisible
        {
            get { return mDownInsertLineVisible; }
            set
            {
                mDownInsertLineVisible = value;
                OnPropertyChanged("DownInsertLineVisible");
            }
        }

        System.Windows.Visibility mChildInsertLineVisible = System.Windows.Visibility.Collapsed;
        [Browsable(false)]
        public System.Windows.Visibility ChildInsertLineVisible
        {
            get { return mChildInsertLineVisible; }
            set
            {
                mChildInsertLineVisible = value;
                OnPropertyChanged("ChildInsertLineVisible");
            }
        }

        System.Windows.Media.Brush mTreeViewItemForeGround = System.Windows.Media.Brushes.White;
        [Browsable(false)]
        public System.Windows.Media.Brush TreeViewItemForeground
        {
            get
            {
                //if (UIWin != null)
                //{
                //    return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(UIWin.TreeViewItemFG_A, UIWin.TreeViewItemFG_R, UIWin.TreeViewItemFG_G, UIWin.TreeViewItemFG_B));
                //}
                //return System.Windows.Media.Brushes.White;
                return mTreeViewItemForeGround;
            }
            set
            {
                //if (UIWin != null)
                //{
                //    var brush = value as System.Windows.Media.SolidColorBrush;
                //    if(brush != null)
                //    {
                //        UIWin.TreeViewItemFG_A = brush.Color.A;
                //        UIWin.TreeViewItemFG_R = brush.Color.R;
                //        UIWin.TreeViewItemFG_G = brush.Color.G;
                //        UIWin.TreeViewItemFG_B = brush.Color.B;
                //    }
                //}
                mTreeViewItemForeGround = value;

                OnPropertyChanged("TreeViewItemForeground");
            }
        }

        System.Windows.Media.Brush mTreeViewItemBackground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(1, 0, 0, 0));
        [Browsable(false)]
        public System.Windows.Media.Brush TreeViewItemBackground
        {
            get
            {
                //if (UIWin != null)
                //{
                //    return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(UIWin.TreeViewItemBG_A, UIWin.TreeViewItemBG_R, UIWin.TreeViewItemBG_G, UIWin.TreeViewItemBG_B));
                //}
                //return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(1, 0, 0, 0));
                return mTreeViewItemBackground;
            }
            set
            {
                //if (UIWin != null)
                //{
                //    var brush = value as System.Windows.Media.SolidColorBrush;
                //    if (brush != null)
                //    {
                //        UIWin.TreeViewItemBG_A = brush.Color.A;
                //        UIWin.TreeViewItemBG_R = brush.Color.R;
                //        UIWin.TreeViewItemBG_G = brush.Color.G;
                //        UIWin.TreeViewItemBG_B = brush.Color.B;
                //    }
                //}
                mTreeViewItemBackground = value;

                OnPropertyChanged("TreeViewItemBackground");
            }
        }

        protected ObservableCollection<WinBase> mLogicChildren = new ObservableCollection<WinBase>();
        [Browsable(false)]
        public ObservableCollection<WinBase> LogicChildren
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

        [Browsable(false)]
        public bool IgnoreSaver
        {
            get
            {
                if(UIWin != null)
                    return UIWin.IgnoreSaver;
                return false;
            }
            set
            {
                if(UIWin != null)
                    UIWin.IgnoreSaver = value;

                OnPropertyChanged("IgnoreSaver");
            }
        }
    }
}
