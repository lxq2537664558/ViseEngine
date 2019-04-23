using System;
using System.ComponentModel;

namespace CSUtility.Map.DynamicBlock
{
    public class DynamicBlockActorInit : CSUtility.Component.ActorInitBase
    {
        DynamicBlockData mDynamicBlockData = new DynamicBlockData();
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public DynamicBlockData DynamicBlockData
        {
            get { return mDynamicBlockData; }
            set { mDynamicBlockData = value; }
        }

        public DynamicBlockActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.DynamicBlock;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient | CSUtility.Component.ActorInitBase.EActorFlag.SaveWithServer;
        }

        public virtual DynamicBlockData CreateDynamicBlockData()
        {
            return new DynamicBlockData();
        }

        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            base.CopyFrom(srcData);

            var srcInit = srcData as DynamicBlockActorInit;

            DynamicBlockData = CreateDynamicBlockData();
            DynamicBlockData.Id = srcInit.DynamicBlockData.Id;
            DynamicBlockData.CopyFrom(srcInit.DynamicBlockData);

            return true;
        }

        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (!base.Read(xndAtt))
                return false;

            SByte hasData;
            xndAtt.Read(out hasData);

            if (hasData > 0)
            {
                if (DynamicBlockData == null)
                    DynamicBlockData = CreateDynamicBlockData();
                DynamicBlockData.Read(xndAtt);
            }

            return true;
        }

        public override bool Write(CSUtility.Support.XndAttrib xndAtt)
        {
            if (!base.Write(xndAtt))
                return false;

            SByte hasData = (SByte)((DynamicBlockData != null) ? 1 : -1);
            xndAtt.Write(hasData);

            if (DynamicBlockData != null)
            {
                DynamicBlockData.Write(xndAtt);
            }

            return true;
        }
    }
}
