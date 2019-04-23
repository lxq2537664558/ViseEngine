using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace EffectEditor
{
    /// <summary>
    /// Interaction logic for ParticleShapeItem.xaml
    /// </summary>
    public partial class ParticleShapeItem : UserControl, INotifyPropertyChanged, ParticleEditorItemInterface
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

        bool mIsSelected = false;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                mIsSelected = value;

                if (mIsSelected)
                    Border_Shape.Background = this.FindResource("ShapeSelectedBackground") as Brush;
                else
                    Border_Shape.Background = this.FindResource("ShapeNormalBackground") as Brush;
            }
        }

        bool mIsCurrentEditModifier = false;
        public bool IsCurrentEditModifier
        {
            get { return mIsCurrentEditModifier; }
            set
            {
                mIsCurrentEditModifier = value;

                HostParticleEmitterItem.HostParticleModifierItem.IsCurrentEditModifier = value;
            }
        }

        public object GetPropertyShowObject()
        {
            return HostParticleShape;
        }

        ParticleEmitterItem mHostParticleEmitterItem;
        public ParticleEmitterItem HostParticleEmitterItem
        {
            get { return mHostParticleEmitterItem; }
            set
            {
                mHostParticleEmitterItem = value;

                if (mHostParticleEmitterItem != null)
                {
                    if (ShapeType != mHostParticleEmitterItem.HostParticleEmitter.ShapeType)
                        ShapeType = mHostParticleEmitterItem.HostParticleEmitter.ShapeType;
                }
            }
        }
        public CCore.Particle.ParticleEmitterShape HostParticleShape
        {
            get { return HostParticleEmitter.Shape; }
        }

        public CCore.Particle.ParticleEmitter HostParticleEmitter
        {
            get { return mHostParticleEmitterItem.HostParticleEmitter; }
        }

        public CCore.Particle.ParticleEmitterShapeCN ShapeType
        {
            get
            {
                if (HostParticleEmitter != null)
                    return HostParticleEmitter.ShapeType;
                return CCore.Particle.ParticleEmitterShapeCN.Point;
            }
            set
            {
                if (HostParticleEmitter != null)
                    HostParticleEmitter.ShapeType = value;

                if (HostParticleEmitterItem.HostParticleModifierItem.HostEditor.GetSelectedparticleEditorItem() == this)
                    HostParticleEmitterItem.HostParticleModifierItem.HostEditor.SetSelectedParticleEditorItem(this);

                OnPropertyChanged("ShapeType");
            }
        }

        public ParticleShapeItem(ParticleEmitterItem hostItem)
        {
            HostParticleEmitterItem = hostItem;

            InitializeComponent();
        }

        private void Border_Shape_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (OnSelectedParticleEmitterItem != null)
            //    OnSelectedParticleEmitterItem(this);

            //if (mSelectedParticleEffectorItem != null)
            //    mSelectedParticleEffectorItem.IsSelected = false;
            //mSelectedParticleEffectorItem = null;

            //IsShapeSelected = true;
            //HostParticleModifierItem.HostEditor.SetItemShowProperty(HostParticleEmitter);
            HostParticleEmitterItem.HostParticleModifierItem.HostEditor.SetSelectedParticleEditorItem(this);
        }
    }
}
