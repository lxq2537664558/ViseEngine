using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Role
{
    public class PlayerData : PlayerDetail
    {
      //  public PlayerDetail PlayerDetail { get; } = new PlayerDetail();
        public int PlayerFaction { get; set; }

        public List<GameData.Skill.SkillData> Skills { get; set; } = new List<GameData.Skill.SkillData>();

        public Guid AIGuid { get; set; }
        public  string IpAddress { get; set; }
        public  int Port { get; set; }
    }

    public class PlayerDataEx : PlayerData
    {
        public Guid mLinkSerialId;
        public AccountInfo mAccountInfo;
        public CSUtility.Net.NetConnection mGateConnect;
        public CSUtility.Net.NetConnection mPlanesConnect;
        public UInt16 mGateConnectIndex;//客户端在gate上的tcpconnect编号

        public object mSaverThread;
        public object CurPlanes = null;
        public object CurMap = null;
    }
}
