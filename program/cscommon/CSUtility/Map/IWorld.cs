namespace CSUtility.Map
{
    public interface IWorldBase
    {
        bool LineCheck(ref SlimDX.Vector3 start, ref SlimDX.Vector3 end, ref CSUtility.Support.stHitResult result);
    }
}
