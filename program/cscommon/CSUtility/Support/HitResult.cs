using System;

namespace CSUtility.Support
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 4)]
    public struct stHitResult
    {
        public SlimDX.Vector3 mHitPosition;
        public SlimDX.Vector3 mHitNormal;
        public float mHitLength;
        public UInt32 mHitFlags;
        public Guid mHitActorId;

        public int BoxTest;
        public int TriTest;

        public SlimDX.Vector3 mClosedPos;
        public float mClosedDistSq;
        public void InitObject()
        {
            mClosedDistSq = -1.0F;
        }
        public void InitObject(stHitResult rh)
        {
            mClosedPos = rh.mClosedPos;
            mClosedDistSq = rh.mClosedDistSq;
            mHitFlags = rh.mHitFlags;
        }
        public bool HasFlag(UInt32 flag)
        {
            return ((mHitFlags & flag) == flag);
        }
    };
}
