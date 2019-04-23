using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace EffectEditor
{
    /// <summary>
    /// Interaction logic for ParticleEffectorItem.xaml
    /// </summary>
    public partial class ParticleEffectorItem : UserControl, INotifyPropertyChanged, ParticleEditorItemInterface
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

        ParticleEmitterItem mHostParticleEmitterItem = null;
        public ParticleEmitterItem HostParticleEmitterItem
        {
            get { return mHostParticleEmitterItem; }
        }

        CCore.Particle.ParticleEffector mHostParticleEffector;
        public CCore.Particle.ParticleEffector HostParticleEffector
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

        public ParticleEffectorItem(ParticleEmitterItem hostItem, CCore.Particle.ParticleEffector hostEffector)
        {
            InitializeComponent();

            mHostParticleEmitterItem = hostItem;
            mHostParticleEffector = hostEffector;

            EffectorName = Program.GetDescription(mHostParticleEffector.Slot);

            hostEffector.Enable = true;
        }

        private void Button_Remove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (HostParticleEffector != null)
            //    HostParticleEffector.Enable = false;

            //if (OnRemoveParticleEffector != null)
            //    OnRemoveParticleEffector(this);
            HostParticleEmitterItem.RemoveParticleEffector(HostParticleEffector);
        }

        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (OnSelectedParticleEffector != null)
            //    OnSelectedParticleEffector(this);

            //HostParticleEmitterItem.HostParticleModifierItem.HostEditor.SetItemShowProperty(HostParticleEffector);
            HostParticleEmitterItem.HostParticleModifierItem.HostEditor.SetSelectedParticleEditorItem(this);
        }
    }
}
