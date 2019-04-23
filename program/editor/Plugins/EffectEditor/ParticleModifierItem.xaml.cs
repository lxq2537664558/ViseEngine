using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace EffectEditor
{
    /// <summary>
    /// Interaction logic for ParticleModifierItem.xaml
    /// </summary>
    public partial class ParticleModifierItem : UserControl, INotifyPropertyChanged, ParticleEditorItemInterface
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

        private ParticleEditor mHostEditor = null;
        public ParticleEditor HostEditor
        {
            get { return mHostEditor; }
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
                    Rect_Title.Fill = this.FindResource("SelectedBackground") as Brush;
                }
                else
                {
                    Rect_Title.Fill = this.FindResource("NormalBackground") as Brush;
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

                if (mIsCurrentEditModifier)
                {
                    Border_Main.BorderBrush = this.FindResource("SelectedBorderBrush") as Brush;
                }
                else
                {
                    Border_Main.BorderBrush = this.FindResource("NormalBorderBrush") as Brush;
                }
            }
        }

        bool mIsActive = true;
        public bool IsActive
        {
            get { return mIsActive; }
            set
            {
                mIsActive = value;

                if (HostParticleModifier != null)
                {
                    foreach (var emitter in StackPanel_Emitters.Children)
                    {
                        if (emitter is ParticleEmitterItem)
                            ((ParticleEmitterItem)emitter).IsActive = value;
                    }
                }

                OnPropertyChanged("IsActive");
            }
        }

        public object GetPropertyShowObject()
        {
            return HostParticleModifier;
        }

        CCore.Modifier.ParticleModifier mHostParticleModifier = null;
        public CCore.Modifier.ParticleModifier HostParticleModifier
        {
            get { return mHostParticleModifier; }
        }

        public ParticleModifierItem(ParticleEditor hostEditor, CCore.Modifier.ParticleModifier modifier)
        {
            InitializeComponent();

            mHostEditor = hostEditor;
            mHostParticleModifier = modifier;
            //mHostParticleModifier.OnSetMeshTemplateId = OnModifierSetMeshTemplateId;
            mHostParticleModifier.OnPropertyUpdate = OnModifierPropertyChanged;

            BindingOperations.SetBinding(TextBlock_ParticleName, TextBlock.TextProperty, new Binding("ParticleModifierName") { Source = mHostParticleModifier });

            UpdateModifierShow();
        }

        private void UpdateModifierShow()
        {
            if (HostParticleModifier == null)
                return;

            StackPanel_Emitters.Children.Clear();
            foreach (var emitter in HostParticleModifier.Emitters)
            {
                var item = new ParticleEmitterItem(this, emitter);

                if (HostEditor.GetSelectedparticleEditorItem() != null && HostEditor.GetSelectedparticleEditorItem().GetPropertyShowObject() == emitter)
                    HostEditor.SetSelectedParticleEditorItem(item);

                StackPanel_Emitters.Children.Add(item);
            }
        }

        private void OnModifierPropertyChanged(CCore.Modifier.ParticleModifier modifier, string proName)
        {
            switch (proName)
            {
                case "MeshTemplateId":
                case "Space":
                    this.HostEditor.RefreshEffectTemplate();
                    break;
            }
        }
        //private void OnModifierSetMeshTemplateId(Guid meshTemplateId, CCore.Modifier.ParticleModifier modifier)
        //{
        //    this.HostEditor.RefreshEffectTemplate();
        //}

        ParticleEmitterItem mSelectedEmitterItem = null;
        private void OnSelectedParticleEmitterItem(ParticleEmitterItem item)
        {
            if (mSelectedEmitterItem != null)
                mSelectedEmitterItem.IsSelected = false;

            mSelectedEmitterItem = item;
            mSelectedEmitterItem.IsSelected = true;
        }

        private void Rect_Title_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (OnSelectedParticleModifierItem != null)
            //    OnSelectedParticleModifierItem(this);

            //HostEditor.SetItemShowProperty(HostParticleModifier);
            //HostEditor.SetCurrentEditModifier(this);
            HostEditor.SetSelectedParticleEditorItem(this);
        }

        private void MenuItem_AddEmitter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HostParticleModifier == null)
                return;

            var emitter = HostParticleModifier.AddEmitter();
            UpdateModifierShow();
        }

        public void RemoveEmitter(ParticleEmitterItem item)
        {
            HostParticleModifier.RemoveEmitterById(item.HostParticleEmitter.Id);
            UpdateModifierShow();
        }

        private void Button_Remove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (EditorCommon.MessageBox.Show("即将删除 " + mHostParticleModifier.ParticleModifierName + " , 删除后不可恢复, 是否确定?", "警告", EditorCommon.MessageBox.enMessageBoxButton.YesNo) == EditorCommon.MessageBox.enMessageBoxResult.Yes)
                HostEditor.RemoveParticle(this);
        }

        private void Button_MoveRight_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var idx = HostEditor.StackPanel_Particles.Children.IndexOf(this);
            HostEditor.StackPanel_Particles.Children.Remove(this);
            HostEditor.StackPanel_Particles.Children.Insert(idx + 1, this);

            HostEditor.HostEffect.RemoveParticleModifier(mHostParticleModifier);
            HostEditor.HostEffect.InsertParticleModifier(idx + 1, mHostParticleModifier);
        }

        private void Button_MoveLeft_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var idx = HostEditor.StackPanel_Particles.Children.IndexOf(this);
            if (idx <= 0)
                return;

            HostEditor.StackPanel_Particles.Children.Remove(this);
            HostEditor.StackPanel_Particles.Children.Insert(idx - 1, this);

            HostEditor.HostEffect.RemoveParticleModifier(mHostParticleModifier);
            HostEditor.HostEffect.InsertParticleModifier(idx - 1, mHostParticleModifier);
        }
    }
}
