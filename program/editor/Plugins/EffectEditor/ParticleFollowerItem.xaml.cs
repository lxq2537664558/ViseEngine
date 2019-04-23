using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace EffectEditor
{
    /// <summary>
    /// Interaction logic for ParticleFollowerItem.xaml
    /// </summary>
    public partial class ParticleFollowerItem : UserControl, INotifyPropertyChanged, ParticleEditorItemInterface
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

        //string mFollowerEmitterName = "Unknow";
        //public string FollowerEmitterName
        //{
        //    get { return mFollowerEmitterName; }
        //    set
        //    {
        //        mFollowerEmitterName = value;

        //        OnPropertyChanged("FollowerEmitterName");
        //    }
        //}

        public string FollowerEmitterName
        {
            get { return (string)GetValue(FollowerEmitterNameProperty); }
            set { SetValue(FollowerEmitterNameProperty, value); }
        }

        public static readonly DependencyProperty FollowerEmitterNameProperty =
            DependencyProperty.Register("FollowerEmitterName", typeof(string), typeof(ParticleFollowerItem),
                                    new FrameworkPropertyMetadata("Unknow", FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnFollowerEmitterNameChanged))
                                    );
        public static void OnFollowerEmitterNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

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

        CCore.Particle.ParticleEmitter mFollowerEmitter = null;
        public CCore.Particle.ParticleEmitter FollowerEmitter
        {
            get { return mFollowerEmitter; }
            set
            {
                mFollowerEmitter = value;
                BindingOperations.SetBinding(this, FollowerEmitterNameProperty, new Binding("Name") { Source = FollowerEmitter });
            }
        }

        public object GetPropertyShowObject()
        {
            return null;
        }

        public ParticleFollowerItem(ParticleEmitterItem hostItem)
        {
            InitializeComponent();

            mHostParticleEmitterItem = hostItem;
        }

        private void Button_Set_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var emitterItem = ParticleEditor.SelectedItem as ParticleEmitterItem;
            if (emitterItem == null)
                return;

            if (emitterItem == this.HostParticleEmitterItem)
            {
                EditorCommon.MessageBox.Show("不能设置发射器自己跟随自己");
                return;
            }

            FollowerEmitter = emitterItem.HostParticleEmitter;

            if (emitterItem != null)
            {
                //mHostParticleEmitterItem.HostParticleEmitter.SetFollowerEmitter(emitterItem.HostParticleEmitter);
                mHostParticleEmitterItem.SetParticleFollower(this);
            }
        }

        private void Button_Remove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            mHostParticleEmitterItem.RemoveParticleFollower(this);
        }
    }
}
