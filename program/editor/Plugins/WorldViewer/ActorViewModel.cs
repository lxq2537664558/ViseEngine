using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WorldViewer
{
    public class ActorViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public override bool Equals(object obj)
        {
            var avm = obj as ActorViewModel;
            if (avm == null)
                return false;

            if (avm.Id == this.Id)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        readonly CCore.World.Actor mActor;
        public CCore.World.Actor Actor
        {
            get { return mActor; }
        }

        readonly ActorViewModel mParent;
        public ActorViewModel Parent
        {
            get { return mParent; }
        }

        ObservableCollection<ActorViewModel> mChildren = new ObservableCollection<ActorViewModel>();
        public ObservableCollection<ActorViewModel> Children
        {
            get { return mChildren; }
            set { mChildren = value; }
        }

        public Guid Id
        {
            get
            {
                if (mActor == null)
                    return Guid.Empty;

                return mActor.Id;
            }
        }

        string mActorName = "";
        public string ActorName
        {
            get { return mActorName; }
            set
            {
                mActorName = value;
                if(mActor != null)
                    mActor.ActorName = value;
                OnPropertyChanged("ActorName");
            }
        }

        bool mVisible = true;
        public bool Visible
        {
            get { return mVisible; }
            set
            {
                mVisible = value;
                OnPropertyChanged("Visible");
            }
        }

        bool mIsExpanded;
        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set
            {
                if(mIsExpanded != value)
                {
                    mIsExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }

                if (mIsExpanded && mParent != null)
                    mParent.IsExpanded = true;
            }
        }

        bool mIsSelected;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                if(IsSelected != value)
                {
                    mIsSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        Visibility mVisibility = Visibility.Visible;
        public Visibility Visibility
        {
            get { return mVisibility; }
            set
            {
                if(mVisibility != value)
                {
                    mVisibility = value;
                    OnPropertyChanged("Visibility");
                }
            }
        }

        System.Windows.Media.Brush mTreeViewItemBackground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(1, 0, 0, 0));
        [Browsable(false)]
        public System.Windows.Media.Brush TreeViewItemBackground
        {
            get { return mTreeViewItemBackground; }
            set
            {
                mTreeViewItemBackground = value;
                OnPropertyChanged("TreeViewItemBackground");
            }
        }

        System.Windows.Media.Brush mTreeViewItemForeGround = System.Windows.Media.Brushes.White;
        [Browsable(false)]
        public System.Windows.Media.Brush TreeViewItemForeground
        {
            get { return mTreeViewItemForeGround; }
            set
            {
                mTreeViewItemForeGround = value;
                OnPropertyChanged("TreeViewItemForeground");
            }
        }

        string mHighLightString = "";
        [Browsable(false)]
        public string HighLightString
        {
            get { return mHighLightString; }
            set
            {
                mHighLightString = value;
                OnPropertyChanged("HighLightString");
            }
        }

        public ActorViewModel(CCore.World.Actor actor)
            : this(actor, null)
        {
        }

        public ActorViewModel(CCore.World.Actor actor, ActorViewModel parent)
        {
            mActor = actor;
            mParent = parent;

            if(mActor != null)
                mActorName = mActor.ActorName;

            //mChildren = new ReadOnlyCollection<ActorViewModel>(
            //                (from child in mActor.Children
            //                 select new PersonViewModel(child, this))
            //                 .ToList<ActorViewModel>());
        }

        private bool NameContainsText(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(ActorName))
                return false;

            return this.ActorName.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }
    }
}
