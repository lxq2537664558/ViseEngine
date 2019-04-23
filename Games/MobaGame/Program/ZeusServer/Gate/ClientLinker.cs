using System;
using System.Collections.Generic;
using System.Text;
using GameData.Role;

namespace Server.Gate
{
    public enum LandStep
    {
        TryLogin,
        SelectRole,
        EnterGame,
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class ClientLinker
    {
        public ClientLinker()
        {
            LinkerSerialId = Guid.NewGuid();
        }

        public SCore.TcpServer.TcpConnectHP Connect;

        public Guid LinkerSerialId;

        public System.DateTime ConnectedTime = System.DateTime.Now;

        public bool OffLineKeepLinker = false;
        public System.DateTime OffLineKeepTime;

        AccountInfo mAccountInfo = new AccountInfo();
        public AccountInfo AccountInfo
        {
            get { return mAccountInfo; } 
        }

        PlayerDataEx mPlayerData;
        public PlayerDataEx PlayerData
        {
            get { return mPlayerData; }
        }

        LandStep mLandStep = LandStep.TryLogin;
        public LandStep LandStep
        {
            get { return mLandStep; }
            set { mLandStep = value; }
        }

        public RPC.RPCForwardInfo mForwardInfo = new RPC.RPCForwardInfo();

        public void InvalidLinkerWhenRoleExit()
        {
            mPlayerData = null;
            mForwardInfo.RoleId = Guid.Empty;
            mForwardInfo.MapIndexInServer = UInt16.MaxValue;
            mForwardInfo.PlayerIndexInMap = UInt16.MaxValue;
            mForwardInfo.Gate2PlanesConnect = null;
            mLandStep = Gate.LandStep.SelectRole;
        }
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class KeepLinkerManager
    {
        static KeepLinkerManager smInstance = new KeepLinkerManager();
        public static KeepLinkerManager Instance
        {
            get { return smInstance; }
        }
        public int KeepLinkerNumber
        {
            get { return mLinkers.Count; }
        }
        Dictionary<Guid, ClientLinker> mLinkers = new Dictionary<Guid, ClientLinker>();
        public Dictionary<Guid, ClientLinker> Linkers
        {
            get { return mLinkers; }
        }
        public void KeepLinker(GateServer gateSever, ClientLinker linker)
        {
            lock (this)
            {
                linker.OffLineKeepTime = System.DateTime.Now;
                linker.OffLineKeepLinker = true;
                foreach (var i in mLinkers)
                {
                    if (i.Value.AccountInfo.Id == linker.AccountInfo.Id)
                    {//同一个账号再次请求登陆，之前的肯定不会再用了
                        mLinkers.Remove(i.Key);
                        TimeOutDisconnect(gateSever, i.Value);
                        break;
                    }
                }
                mLinkers[linker.LinkerSerialId] = linker;
            }
        }
        public ClientLinker PeekLinker(Guid linkId)
        {
            lock (this)
            {
                ClientLinker linker;
                if (mLinkers.TryGetValue(linkId, out linker))
                {
                    mLinkers.Remove(linkId);
                    linker.OffLineKeepLinker = false;
                    return linker;
                }
                return null;
            }
        }

        public bool TryDisconnectKeepLinker(GateServer gateSever, Guid accountId)
        {
            lock (this)
            {
                foreach (var i in mLinkers)
                {
                    if (i.Value.AccountInfo.Id == accountId)
                    {
                        mLinkers.Remove(i.Key);
                        TimeOutDisconnect(gateSever, i.Value);
                        return true;
                    }
                }
                return false;
            }
        }

        const int KeepSecond = 90;//int.MaxValue;//
        public void Tick(GateServer gateSever)
        {
            //这里应该有一个倒计时，发现超过时间的就要让他断线啦
            lock (this)
            {
                var lst = new List<Guid>();
                System.DateTime nowTime = System.DateTime.Now;
                foreach (var i in mLinkers)
                {
                    if ((nowTime - i.Value.OffLineKeepTime).Seconds > KeepSecond)
                    {
                        TimeOutDisconnect(gateSever, i.Value);
                        lst.Add(i.Key);
                    }
                }

                foreach (var i in lst)
                {
                    mLinkers.Remove(i);
                }
            }
        }

        void TimeOutDisconnect(GateServer gateSever, ClientLinker linker)
        {
            linker.OffLineKeepLinker = false;
            gateSever.NotifyOtherServers_ClientDisconnect(linker);
        }
    }
}
