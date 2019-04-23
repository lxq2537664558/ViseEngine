using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EffectEditor
{
    public class SpawnEmitterItem : INotifyPropertyChanged
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

        string mDisplayName = "";
        public string DisplayName
        {
            get { return mDisplayName; }
            set
            {
                mDisplayName = value;

                OnPropertyChanged("DisplayName");
            }
        }

        CCore.Particle.ParticleEmitter mHostEmitter = null;
        public CCore.Particle.ParticleEmitter HostEmitter
        {
            get { return mHostEmitter; }
            set
            {
                mHostEmitter = value;
                if (mHostEmitter != null)
                {
                    DisplayName = mHostEmitter.Name;
                }
                else
                    DisplayName = "";

                OnPropertyChanged("HostEmitter");
            }
        }

        //ParticleEmitterItem mHostEmitterItem;
        //public ParticleEmitterItem HostEmitterItem
        //{
        //    get { return mHostEmitterItem; }
        //    set
        //    {
        //        mHostEmitterItem = value;

        //        HostEmitter = mHostEmitterItem.HostParticleEmitter;

        //        OnPropertyChanged("HostEmitterItem");
        //    }
        //}

        public SpawnEmitterItem MySelf
        {
            get { return this; }
        }
    }

    /// <summary>
    /// Interaction logic for ParticleSpawnEffectorItem.xaml
    /// </summary>
    public partial class ParticleSpawnEffectorItem : UserControl, INotifyPropertyChanged, ParticleEditorItemInterface
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

        ObservableCollection<SpawnEmitterItem> mEmittersList = new ObservableCollection<SpawnEmitterItem>();
        public ObservableCollection<SpawnEmitterItem> EmittersList
        {
            get { return mEmittersList; }
            set
            {
                mEmittersList = value;
                OnPropertyChanged("EmittersList");
            }
        }

        ParticleEmitterItem mHostParticleEmitterItem = null;
        public ParticleEmitterItem HostParticleEmitterItem
        {
            get { return mHostParticleEmitterItem; }
        }

        CCore.Particle.ParticleEffector_Spawn mHostParticleEffector;
        public CCore.Particle.ParticleEffector_Spawn HostParticleEffector
        {
            get { return mHostParticleEffector; }
        }

        string mEffectorName = "Effector";
        public string EffectorName
        {
            get { return mEffectorName; }
            set
            {
                mEffectorName = value;
                OnPropertyChanged("EffectorName");
            }
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
                    Border_Main.Background = this.FindResource("SelectedBackground") as Brush;
                }
                else
                    Border_Main.Background = this.FindResource("NormalBackground") as Brush;
            }
        }

        bool mIsCurrentEditModifier = false;
        public bool IsCurrentEditModifier
        {
            get { return mIsCurrentEditModifier; }
            set
            {
                mIsCurrentEditModifier = value;

                HostParticleEmitterItem.HostParticleModifierItem.IsCurrentEditModifier = mIsCurrentEditModifier;
            }
        }

        bool mIsActive = true;
        public bool IsActive
        {
            get { return mIsActive; }
            set
            {
                mIsActive = value;

                mHostParticleEffector.Enable = value;

                OnPropertyChanged("IsActive");
            }
        }

        public object GetPropertyShowObject()
        {
            return HostParticleEffector;
        }

        public ParticleSpawnEffectorItem(ParticleEmitterItem hostItem, CCore.Particle.ParticleEffector hostEffector)
        {
            InitializeComponent();

            EmittersList.Clear();
            mHostParticleEmitterItem = hostItem;
            mHostParticleEffector = hostEffector as CCore.Particle.ParticleEffector_Spawn;

            EffectorName = Program.GetDescription(mHostParticleEffector.Slot);

            UpdateSpawnEmitterShow();

            hostEffector.Enable = true;
        }

        private void UpdateSpawnEmitterShow()
        {
            if (mHostParticleEffector == null)
                return;

            foreach (var emitter in mHostParticleEffector.SpawnEmitters)
            {
                var item = new SpawnEmitterItem();
                item.HostEmitter = emitter;
                EmittersList.Add(item);
            }
        }

        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HostParticleEmitterItem.HostParticleModifierItem.HostEditor.SetSelectedParticleEditorItem(this);
        }

        private void Button_Remove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (HostParticleEffector != null)
            //    HostParticleEffector.Enable = false;

            //if (OnRemoveParticleEffector != null)
            //    OnRemoveParticleEffector(this);
            HostParticleEmitterItem.RemoveParticleEffector(HostParticleEffector);
        }

        private void Button_AddEmitter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SpawnEmitterItem item = new SpawnEmitterItem();
            EmittersList.Add(item);
        }

        private void Button_DelEmitter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ListBox_Emitters.SelectedIndex < 0)
            {
                EditorCommon.MessageBox.Show("请先选择一个列表中的发射器再进行删除!");
                return;
            }

            var selEmitter = EmittersList[ListBox_Emitters.SelectedIndex].HostEmitter;
            HostParticleEffector.RemoveSpawnEmitter(selEmitter);

            EmittersList.RemoveAt(ListBox_Emitters.SelectedIndex);
        }

        private void Button_SetEmitter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;

            var item = btn.Tag as SpawnEmitterItem;
            if (item == null)
                return;

            var emitterItem = ParticleEditor.SelectedItem as ParticleEmitterItem;
            if (emitterItem == null)
                return;

            if (emitterItem == this.HostParticleEmitterItem)
            {
                EditorCommon.MessageBox.Show("不能设置发射器自己繁衍自己");
                return;
            }

            if (HostParticleEffector.ProcessSetSpawnEmitter(emitterItem.HostParticleEmitter) == false)
            {
                EditorCommon.MessageBox.Show("不能重复设置发射器");
                return;
            }

            item.HostEmitter = emitterItem.HostParticleEmitter;
        }

        private void Button_FindEmitter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //var btn = sender as Button;
            //if (btn == null)
            //    return;

            //var item = btn.Tag as SpawnEmitterItem;
            //if (item == null)
            //    return;

            //if (item.HostEmitterItem == null)
            //    return;

            //HostParticleEmitterItem.HostParticleModifierItem.HostEditor.SetSelectedParticleEditorItem(item.HostEmitterItem);

        }
    }
}
