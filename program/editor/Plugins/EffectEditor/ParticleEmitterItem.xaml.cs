using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace EffectEditor
{
    /// <summary>
    /// Interaction logic for ParticleEmitterItem.xaml
    /// </summary>
    public partial class ParticleEmitterItem : UserControl, INotifyPropertyChanged, ParticleEditorItemInterface
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

        ParticleModifierItem mHostParticleModifierItem = null;
        public ParticleModifierItem HostParticleModifierItem
        {
            get { return mHostParticleModifierItem; }
        }

        CCore.Particle.ParticleEmitter mHostParticleEmitter;
        public CCore.Particle.ParticleEmitter HostParticleEmitter
        {
            get { return mHostParticleEmitter; }
        }

        bool mIsSelected = false;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                mIsSelected = value;

                if (mIsSelected)
                {
                    Border_Title.Background = this.FindResource("SelectedBackground") as Brush;
                }
                else
                {
                    Border_Title.Background = this.FindResource("NormalBackground") as Brush;
                }
            }
        }

        bool mIsCurrentEditModifier = false;
        public bool IsCurrentEditModifier
        {
            get { return mIsCurrentEditModifier; }
            set
            {
                mIsCurrentEditModifier = value;

                if (HostParticleModifierItem != null)
                    HostParticleModifierItem.IsCurrentEditModifier = mIsCurrentEditModifier;
            }
        }

        bool mIsActive = true;
        public bool IsActive
        {
            get
            {
                return mIsActive;
            }
            set
            {
                mIsActive = value;

                if (HostParticleEmitter != null)
                    HostParticleEmitter.Enabled = value;

                OnPropertyChanged("IsActive");
            }
        }

        bool mIsLoop = true;
        public bool IsLoop
        {
            get { return mIsLoop; }
            set
            {
                mIsLoop = value;

                mHostParticleEmitter.IsLooping = value;

                OnPropertyChanged("IsLoop");
            }
        }

        public object GetPropertyShowObject()
        {
            return HostParticleEmitter;
        }

        //bool mIsShapeSelected = false;
        //public bool IsShapeSelected
        //{
        //    get { return mIsShapeSelected; }
        //    set
        //    {
        //        mIsShapeSelected = value;

        //        if (mIsShapeSelected)
        //        {
        //            Border_Shape.Background = this.FindResource("ShapeNormalBackground") as Brush;
        //        }
        //        else
        //        {
        //            Border_Shape.Background = this.FindResource("ShapeSelectedBackground") as Brush;
        //        }
        //    }
        //}

        //string mShapeName = "未知";
        //public string ShapeName
        //{
        //    get { return mShapeName; }
        //    set
        //    {
        //        mShapeName = value;
        //        OnPropertyChanged("ShapeName");
        //    }
        //}

        public ParticleEmitterItem(ParticleModifierItem hostItem, CCore.Particle.ParticleEmitter hostParticleEmitter)
        {
            InitializeComponent();

            mHostParticleModifierItem = hostItem;
            mHostParticleEmitter = hostParticleEmitter;
            if (mHostParticleEmitter != null)
            {
                mHostParticleEmitter.Reset();
                IsLoop = mHostParticleEmitter.IsLooping;
            }

            BindingOperations.SetBinding(TextBlock_EmitterName, TextBlock.TextProperty, new Binding("Name") { Source = mHostParticleEmitter });

            var shapeItem = new ParticleShapeItem(this);
            Grid_Shape.Children.Add(shapeItem);

            if (HostParticleModifierItem.HostEditor.GetSelectedparticleEditorItem() != null && HostParticleModifierItem.HostEditor.GetSelectedparticleEditorItem().GetPropertyShowObject() == shapeItem.HostParticleShape)
                HostParticleModifierItem.HostEditor.SetSelectedParticleEditorItem(shapeItem);

            UpdateEffectors();
            UpdateEffectorMenu();

            UpdateFollowers();
        }

        private void UpdateEffectors()
        {
            if (HostParticleEmitter == null)
                return;

            StackPanel_Effectors.Children.Clear();
            foreach (var effector in HostParticleEmitter.Effectors)
            {
                if (effector is CCore.Particle.ParticleEffector_Spawn)
                {
                    var item = new ParticleSpawnEffectorItem(this, effector);
                    //item.OnRemoveParticleEffector = OnRemoveParticleEffector;
                    //item.OnSelectedParticleEffector = OnSelectedParticleEffector;
                    if (HostParticleModifierItem.HostEditor.GetSelectedparticleEditorItem() != null && HostParticleModifierItem.HostEditor.GetSelectedparticleEditorItem().GetPropertyShowObject() == effector)
                        HostParticleModifierItem.HostEditor.SetSelectedParticleEditorItem(item);

                    StackPanel_Effectors.Children.Add(item);
                }
                else
                {
                    var item = new ParticleEffectorItem(this, effector);
                    //item.OnRemoveParticleEffector = OnRemoveParticleEffector;
                    //item.OnSelectedParticleEffector = OnSelectedParticleEffector;
                    if (HostParticleModifierItem.HostEditor.GetSelectedparticleEditorItem() != null && HostParticleModifierItem.HostEditor.GetSelectedparticleEditorItem().GetPropertyShowObject() == effector)
                        HostParticleModifierItem.HostEditor.SetSelectedParticleEditorItem(item);

                    StackPanel_Effectors.Children.Add(item);
                }
            }

            //if (StackPanel_Effectors.Children.Count == 0)
            //    Border_Effectors.Visibility = System.Windows.Visibility.Collapsed;
            //else
            //    Border_Effectors.Visibility = System.Windows.Visibility.Visible;
        }

        private void UpdateEffectorMenu()
        {
            EffectorMenu.Items.Clear();

            List<CCore.Particle.EffectorSlot> slotValues = new List<CCore.Particle.EffectorSlot>((CCore.Particle.EffectorSlot[])(System.Enum.GetValues(typeof(CCore.Particle.EffectorSlot))));
            //foreach (var effector in HostParticleEmitter.Effectors)
            //{
            //    slotValues.Remove(effector.Slot);                
            //}
            slotValues.Remove(CCore.Particle.EffectorSlot.COUNT);

            foreach (var slot in slotValues)
            {
                var menuItem = new MenuItem()
                {
                    Header = Program.GetDescription(slot),
                    Tag = slot,
                    Foreground = Brushes.White
                };
                menuItem.Click += EffectorMenuItem_Click;

                //menuItem.Style = (Style)FindResource(ResourceLibrary.CustomResources.MenuItemStyle);

                EffectorMenu.Items.Add(menuItem);                
            }
        }

        void EffectorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (HostParticleEmitter == null)
                return;

            var menuItem = sender as MenuItem;
            HostParticleEmitter.AddEffector((CCore.Particle.EffectorSlot)menuItem.Tag);

            UpdateEffectors();

            UpdateEffectorMenu();
        }

        void MenuBtn_AddFollower_Click(object sender, RoutedEventArgs e)
        {
            if (HostParticleEmitter == null)
                return;

            ParticleFollowerItem item = new ParticleFollowerItem(this);
            StackPanel_Followers.Children.Add(item);

            HostParticleEmitter.Followers.Add(Guid.Empty);
        }

        public void RemoveParticleEffector(CCore.Particle.ParticleEffector effect)
        {
            if (HostParticleEmitter == null)
                return;

            HostParticleEmitter.RemoveEffector(effect);

            UpdateEffectors();

            UpdateEffectorMenu();
        }

        public void SetParticleFollower(ParticleFollowerItem item)
        {
            if (HostParticleEmitter == null)
                return;

            var idx = StackPanel_Followers.Children.IndexOf(item);
            HostParticleEmitter.SetFollowerEmitter(idx, item.FollowerEmitter);
        }
        public void RemoveParticleFollower(ParticleFollowerItem item)
        {
            if (HostParticleEmitter == null)
                return;

            var idx = StackPanel_Followers.Children.IndexOf(item);
            HostParticleEmitter.RemoveFollowerEmitter(idx);

            UpdateFollowers();
        }

        private void UpdateFollowers()
        {
            if (HostParticleEmitter == null)
                return;

            StackPanel_Followers.Children.Clear();
            foreach (var id in HostParticleEmitter.Followers)
            {
                var item = new ParticleFollowerItem(this);

                var emt = this.HostParticleEmitter.HostModifier.HostEffector.FindEmitter(id);
                item.FollowerEmitter = emt;

                StackPanel_Followers.Children.Add(item);
            }

            //if (StackPanel_Effectors.Children.Count == 0)
            //    Border_Effectors.Visibility = System.Windows.Visibility.Collapsed;
            //else
            //    Border_Effectors.Visibility = System.Windows.Visibility.Visible;
        }

        private void Border_Title_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HostParticleModifierItem.HostEditor.SetSelectedParticleEditorItem(this);
        }

        private void Button_DelEmitter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            HostParticleModifierItem.RemoveEmitter(this);
        }
    }
}
