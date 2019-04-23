using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CSUtility.Map.Role
{
    public abstract class NPCDataInitFactory
    {
        public static NPCDataInitFactory Instance;
        public abstract CSUtility.Data.RoleDataBase CreateNPCData();
    }

    public class NPCInitializerActorInit : CSUtility.Component.ActorInitBase
    {
        CSUtility.Data.RoleDataBase mNPCData;
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public CSUtility.Data.RoleDataBase NPCData
        {
            get { return mNPCData; }
            set { mNPCData = value; }
        }

        public NPCInitializerActorInit()
        {
            mNPCData = NPCDataInitFactory.Instance?.CreateNPCData();            
            GameType = (UInt16)CSUtility.Component.EActorGameType.NpcInitializer;
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.SaveWithServer;
            SceneFlag = Component.enActorSceneFlag.Dynamic_Origion;
        }
    }
}
