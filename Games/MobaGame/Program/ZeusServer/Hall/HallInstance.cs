using Server.Hall.Map;
using Server.Hall.Role.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class HallInstance
    {
        public static HallInstance NullHallsInstance = new HallInstance(new GameData.HallsData());

        GameData.HallsData mHallsData;
        public GameData.HallsData HallsData
        {
            get { return mHallsData; }
        }

        public Guid HallsId
        {
            get { return mHallsData.HallsId; }
        }

        public HallInstance(GameData.HallsData hallsData)
        {
            mHallsData = hallsData;
        }

        Dictionary<Guid, MapInstance> mGlobalMaps = new Dictionary<Guid, MapInstance>();
        public Dictionary<Guid, MapInstance> GlobalMaps
        {
            get { return mGlobalMaps; }
        }
        public MapInstance GetGlobalMap(Guid mapSourceId)
        {
            MapInstance result;
            if (mGlobalMaps.TryGetValue(mapSourceId, out result))
                return result;
            return null;
        }

        public void AddGlobalMap(Guid mapSourceId, MapInstance map)
        {
            mGlobalMaps[mapSourceId] = map;
        }        

        Dictionary<Guid, PlayerInstance> mPlayers = new Dictionary<Guid, PlayerInstance>();
        public Dictionary<Guid, PlayerInstance> AllPlayers
        {//这里现在纯粹就是一个统计功能，不能做任何逻辑处理，因为同一个位面的不同地图，可能运行在多个物理服务器进程上
            get { return mPlayers; }
        }

        #region Enter@Leave
        public void EnterHalls(PlayerInstance player)
        {
            if (player.HallInstance == this)
                return;

            lock (this)
            {
                mPlayers[player.Id] = player;
                player.HallInstance = this;
                if (this.HallsId != Guid.Empty)
                    player.PlayerData.HallsId = this.HallsId;
            }

            {
//                 RPC.PackageWriter pkg = new RPC.PackageWriter();
//                 H_RPCRoot.smInstance.HGet_DataServer(pkg).HGet_PlayerManager(pkg).RoleEnterPlanesSuccessed(pkg, player.Id);
//                 pkg.DoCommand(IPlanesServer.Instance.DataConnect, RPC.CommandTargetType.DefaultType);
            }

            {
//                 RPC.PackageWriter pkg = new RPC.PackageWriter();
//                 H_RPCRoot.smInstance.HGet_ComServer(pkg).HGet_UserRoleManager(pkg).RPC_RoleEnterPlanes(pkg, player.Id);
//                 pkg.DoCommand(IPlanesServer.Instance.ComConnect, RPC.CommandTargetType.DefaultType);
            }
        }
        public void LeaveHalls(Guid roleId)
        {
            lock (this)
            {
                var player = mPlayers[roleId];
                if (player != null)
                {
                    player.HallInstance = null;
                }
                mPlayers.Remove(roleId);
            }

            {
//                 RPC.PackageWriter pkg = new RPC.PackageWriter();
//                 H_RPCRoot.smInstance.HGet_ComServer(pkg).HGet_UserRoleManager(pkg).RPC_RoleLogout(pkg, roleId);
//                 pkg.DoCommand(IPlanesServer.Instance.ComConnect, RPC.CommandTargetType.DefaultType);
            }
        }
        #endregion        
    }
}
