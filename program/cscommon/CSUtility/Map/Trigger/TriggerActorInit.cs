using System;
using System.ComponentModel;

namespace CSUtility.Map.Trigger
{
    public class TriggerActorInit : CSUtility.Component.ActorInitBase
    {
        TriggerData mTriggerData;
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public TriggerData TriggerData
        {
            get { return mTriggerData; }
            set { mTriggerData = value; }
        }

        bool mForClient = true;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool ForClient
        {
            get { return mForClient; }
            set { mForClient = value; }
        }

        bool mForServer = true;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool ForServer
        {
            get { return mForServer; }
            set { mForServer = value; }
        }

        public TriggerActorInit()
        {
            GameType = (UInt16)CSUtility.Component.EActorGameType.Trigger;
            SceneFlag = CSUtility.Component.enActorSceneFlag.Trigger;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.SaveWithServer;

            TriggerData = new TriggerData();            
        }
    }
}
