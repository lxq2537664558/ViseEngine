using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WorldViewer
{
    /// <summary>
    /// Interaction logic for WorldPanelItemsTab.xaml
    /// </summary>
    public partial class WorldPanelItemsTab : UserControl, INotifyPropertyChanged
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

        public delegate void Delegate_OnSelectedActor(CCore.World.Actor actor, bool bMultiSelected);
        public Delegate_OnSelectedActor OnSelectedActor;

        bool mShowAll = false;
        public bool ShowAll
        {
            get { return mShowAll; }
            set
            {
                mShowAll = value;

                foreach (var item in ItemsList)
                {
                    item.ActorVisible = value;
                }
            }
        }

        public List<WorldItem> ItemsList
        {
            get { return (List<WorldItem>)GetValue(ItemsListProperty); }
            set { SetValue(ItemsListProperty, value); }
        }

        public static readonly DependencyProperty ItemsListProperty =
            DependencyProperty.Register("ItemsList", typeof(List<WorldItem>), typeof(WorldPanelItemsTab),
                                        new FrameworkPropertyMetadata(new List<WorldItem>(), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnItemsListChanged)));
        public static void OnItemsListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        string mSearchString = "";
        public string SearchString
        {
            get { return mSearchString; }
            set
            {
                mSearchString = value;

                if (string.IsNullOrEmpty(mSearchString))
                {
                    foreach (var item in ItemsList)
                    {
                        item.ItemVisibility = Visibility.Visible;
                    }
                }
                else
                {
                    var tempTex = mSearchString.ToLower();
                    foreach (var item in ItemsList)
                    {
                        if (item.DisplayName.ToLower().Contains(tempTex))
                            item.ItemVisibility = Visibility.Visible;
                        else
                            item.ItemVisibility = Visibility.Collapsed;
                    }
                }

                OnPropertyChanged("SearchString");
            }
        }

        public WorldPanelItemsTab()
        {
            InitializeComponent();
        }
        
        public bool SelectedFromWorld = false;
        private void ListBox_Items_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SelectedFromWorld)
                return;

            if (ListBox_Items.SelectedItems.Count == 0)
            {
                if (OnSelectedActor != null)
                    OnSelectedActor(null, false);
            }
            else
            {
                var worldItem = ListBox_Items.SelectedItems[0] as WorldItem;
                if (OnSelectedActor != null)
                    OnSelectedActor(worldItem.HostActor, false);

                CCore.Client.MainWorldInstance.RemoveEditorActor(worldItem.HostActor);
                CCore.Client.MainWorldInstance.AddActor(worldItem.HostActor);

                for (int i = 1; i < ListBox_Items.SelectedItems.Count; i++)
                {
                    worldItem = ListBox_Items.SelectedItems[i] as WorldItem;
                    if (OnSelectedActor != null)
                        OnSelectedActor(worldItem.HostActor, true);

                    CCore.Client.MainWorldInstance.RemoveEditorActor(worldItem.HostActor);
                    CCore.Client.MainWorldInstance.AddActor(worldItem.HostActor);
                }
            }

        }

        private void Button_ResetEffect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button == null)
                return;

            var actor = button.Tag as CCore.World.Actor;
            if (actor == null)
                return;

            var visual = actor.Visual as CCore.Component.EffectVisual;
            if (visual == null)
                return;

            if (visual.EffectInit.EffectTemplate == null)
                return;

            visual.EffectInit.EffectTemplate.Reset();
        }
    }
}
