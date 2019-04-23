using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class GMCommand
    {
        [CCore.Support.GMCommand("ReloadTemplate", "重读数据模板")]
        public static void GM_ReloadTemplate(string templateType, UInt16 id, Byte opType)
        {
            var pkg = new RPC.PackageWriter();
            Server.H_RPCRoot.smInstance.HGet_GateServer(pkg).GM_ReloadTemplate(pkg, templateType, id, opType);
            pkg.DoCommand(CCore.Engine.Instance.Client.GateSvrConnect, RPC.CommandTargetType.DefaultType);
        }
    }
}
