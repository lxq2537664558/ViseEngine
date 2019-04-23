using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EffectEditor
{
    public class ParticleModifierTreeNode : INotifyPropertyChanged
	{
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }
        public string EditIcon { get; set; }
        public object TargetObj { get; private set; }
        public bool IsExpanded { get; set; }

        public ObservableCollection<ParticleModifierTreeNode> Children { get; set; }
        public ParticleModifierTreeNode(string name, string caption)
		{
            Name = name;
            DisplayName = caption;
            IsExpanded = true;
            Children = new ObservableCollection<ParticleModifierTreeNode>();
		}
        public ParticleModifierTreeNode(CCore.Modifier.ParticleModifier e)
            : this("Modifier", "粒子修改器")
        {
            TargetObj = e;
        }
        public ParticleModifierTreeNode(CCore.Particle.ParticleEmitter e)
            : this("Emitter", e.Name)
        {
            TargetObj = e;
        }
        public ParticleModifierTreeNode(CCore.Particle.ParticleEffector e)
            : this("Effector", e.Slot.ToString())
        {
            TargetObj = e;
        }
        public ParticleModifierTreeNode(CCore.Particle.ParticleEmitterShape e)
            : this("Shape", "形体")
        {
            TargetObj = e;
        }

        #region INotifyPropertyChanged 成员
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}