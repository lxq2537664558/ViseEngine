using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Data
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class HallsMgr
    {
        DataServer mDataServer;
        Dictionary<Guid, GameData.HallsData> mHalls = new Dictionary<Guid, GameData.HallsData>();
        public Dictionary<Guid, GameData.HallsData> Halls
        {
            get { return mHalls; }
        }

        public HallsMgr(DataServer server)
        {
            mDataServer = server;
        }

        public void CreateHallsData()
        {
            GameData.HallsData hd = new GameData.HallsData();
            hd.CreateTime = DateTime.Now;
            hd.HallsId = Guid.NewGuid();

            mHalls.Add(hd.HallsId, hd);

            UpdateHallsInfo2RegServer();
        }

        public void UpdateHallsInfo2RegServer()
        {
            var pkg = new RPC.PackageWriter();
            var info = new RPC.DataWriter();
            UInt16 count = (UInt16)mHalls.Count;
            info.Write(count);
            foreach (var i in mHalls)
            {
                info.Write(i.Value, false);
            }
            //H_RPCRoot.smInstance.HGet_RegServer(pkg).UpdatePlanesInfo(pkg, info);
            pkg.DoCommand(mDataServer.RegisterConnect, RPC.CommandTargetType.DefaultType);
        }

        public GameData.HallsData GetHallsData(Guid hallsId)
        {
            GameData.HallsData hallsData;
            if (false == mHalls.TryGetValue(hallsId, out hallsData))
                return null;

            return hallsData;
        }

        public void OnHallsSeverDisconnect(SCore.TcpServer.TcpConnect connect)
        {
            
        }
    }
}
