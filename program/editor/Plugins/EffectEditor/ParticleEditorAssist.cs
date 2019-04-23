namespace EffectEditor
{
    public interface ParticleEditorItemInterface
    {
        bool IsSelected
        {
            get;
            set;
        }

        bool IsCurrentEditModifier
        {
            get;
            set;
        }

        object GetPropertyShowObject();
    }
}
