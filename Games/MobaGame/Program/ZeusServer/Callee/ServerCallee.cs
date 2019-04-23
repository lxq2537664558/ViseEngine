//Server Callee
namespace RPC_ExecuterNamespace{
[RPC.RPCMethordExecuterTypeAttribute(557012418,"RegGateServer",typeof(HExe_557012418))]
public class HExe_557012418: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.DataServer host = obj as Server.DataServer;
        if (host == null) return null;
        System.String ip;
        pkg.Read(out ip);
        System.UInt16 port;
        pkg.Read(out port);
        System.Guid id;
        pkg.Read(out id);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.DataServer).GetMethod("RegGateServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.DataServer:RegGateServer");
            if (isValid==false)
                return null;
        }
        return host.RegGateServer(ip,port,id,connect);
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3802170108,"RegHallServer",typeof(HExe_3802170108))]
public class HExe_3802170108: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.DataServer host = obj as Server.DataServer;
        if (host == null) return null;
        System.String ip;
        pkg.Read(out ip);
        System.UInt16 port;
        pkg.Read(out port);
        System.Guid id;
        pkg.Read(out id);
        System.Single power;
        pkg.Read(out power);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.DataServer).GetMethod("RegHallServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.DataServer:RegHallServer");
            if (isValid==false)
                return null;
        }
        return host.RegHallServer(ip,port,id,power,connect);
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1095519736,"LoginRole",typeof(HExe_1095519736))]
public class HExe_1095519736: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.DataServer host = obj as Server.DataServer;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.DataServer).GetMethod("LoginRole");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.DataServer:LoginRole");
            if (isValid==false)
                return null;
        }
        host.LoginRole(connect,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(2214271257,"TestRPCVersion",typeof(HExe_2214271257))]
public class HExe_2214271257: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.GateServer host = obj as Server.GateServer;
        if (host == null) return null;
        System.Int32 clientHash;
        pkg.Read(out clientHash);
        System.Int32 serverHash;
        pkg.Read(out serverHash);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.GateServer).GetMethod("TestRPCVersion");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.GateServer:TestRPCVersion");
            if (isValid==false)
                return null;
        }
        return host.TestRPCVersion(clientHash,serverHash);
    }
}
[RPC.RPCMethordExecuterTypeAttribute(2186626402,"TryEnterGame",typeof(HExe_2186626402))]
public class HExe_2186626402: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.GateServer host = obj as Server.GateServer;
        if (host == null) return null;
        System.Guid mapSourceId;
        pkg.Read(out mapSourceId);
        System.UInt16 roleTemplateId;
        pkg.Read(out roleTemplateId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.GateServer).GetMethod("TryEnterGame");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.GateServer:TryEnterGame");
            if (isValid==false)
                return null;
        }
        host.TryEnterGame(mapSourceId,roleTemplateId,connect,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(2854127094,"GM_ReloadTemplate",typeof(HExe_2854127094))]
public class HExe_2854127094: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 300; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.GateServer host = obj as Server.GateServer;
        if (host == null) return null;
        System.String templateType;
        pkg.Read(out templateType);
        System.UInt16 id;
        pkg.Read(out id);
        System.Byte opType;
        pkg.Read(out opType);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.GateServer).GetMethod("GM_ReloadTemplate");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.GateServer:GM_ReloadTemplate");
            if (isValid==false)
                return null;
        }
        host.GM_ReloadTemplate(templateType,id,opType,connect,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(4254261451,"GetHallServerId",typeof(HExe_4254261451))]
public class HExe_4254261451: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.HallServer host = obj as Server.HallServer;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.HallServer).GetMethod("GetHallServerId");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.HallServer:GetHallServerId");
            if (isValid==false)
                return null;
        }
        return host.GetHallServerId();
    }
}
[RPC.RPCMethordExecuterTypeAttribute(876228768,"RegGateServer",typeof(HExe_876228768))]
public class HExe_876228768: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.HallServer host = obj as Server.HallServer;
        if (host == null) return null;
        System.String ip;
        pkg.Read(out ip);
        System.UInt16 port;
        pkg.Read(out port);
        System.Guid id;
        pkg.Read(out id);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.HallServer).GetMethod("RegGateServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.HallServer:RegGateServer");
            if (isValid==false)
                return null;
        }
        host.RegGateServer(ip,port,id,connect);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(693389729,"EnterMap",typeof(HExe_693389729))]
public class HExe_693389729: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.HallServer host = obj as Server.HallServer;
        if (host == null) return null;
        System.UInt16 cltHandle;
        pkg.Read(out cltHandle);
        System.String ip;
        pkg.Read(out ip);
        System.Int32 port;
        pkg.Read(out port);
        System.Guid mapSourceId;
        pkg.Read(out mapSourceId);
        System.UInt16 roleTemplateId;
        pkg.Read(out roleTemplateId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.HallServer).GetMethod("EnterMap");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.HallServer:EnterMap");
            if (isValid==false)
                return null;
        }
        host.EnterMap(cltHandle,ip,port,mapSourceId,roleTemplateId,connect,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3191202514,"LeaveMap",typeof(HExe_3191202514))]
public class HExe_3191202514: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.HallServer host = obj as Server.HallServer;
        if (host == null) return null;
        System.UInt16 cltHandle;
        pkg.Read(out cltHandle);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.HallServer).GetMethod("LeaveMap");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.HallServer:LeaveMap");
            if (isValid==false)
                return null;
        }
        host.LeaveMap(cltHandle);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3754335694,"RemoveInstanceMap",typeof(HExe_3754335694))]
public class HExe_3754335694: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.HallServer host = obj as Server.HallServer;
        if (host == null) return null;
        System.Guid mapInstanceId;
        pkg.Read(out mapInstanceId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.HallServer).GetMethod("RemoveInstanceMap");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.HallServer:RemoveInstanceMap");
            if (isValid==false)
                return null;
        }
        host.RemoveInstanceMap(mapInstanceId);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3517965434,"GM_ReloadTemplate",typeof(HExe_3517965434))]
public class HExe_3517965434: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.HallServer host = obj as Server.HallServer;
        if (host == null) return null;
        System.String templateType;
        pkg.Read(out templateType);
        System.UInt16 id;
        pkg.Read(out id);
        System.Byte opType;
        pkg.Read(out opType);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.HallServer).GetMethod("GM_ReloadTemplate");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.HallServer:GM_ReloadTemplate");
            if (isValid==false)
                return null;
        }
        host.GM_ReloadTemplate(templateType,id,opType,connect,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1858890362,"StartNewServer",typeof(HExe_1858890362))]
public class HExe_1858890362: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.MonitorServer host = obj as Server.MonitorServer;
        if (host == null) return null;
        System.String name;
        pkg.Read(out name);
        System.String args;
        pkg.Read(out args);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.MonitorServer).GetMethod("StartNewServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.MonitorServer:StartNewServer");
            if (isValid==false)
                return null;
        }
        host.StartNewServer(name,args,connect,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(989949548,"StopServer",typeof(HExe_989949548))]
public class HExe_989949548: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.MonitorServer host = obj as Server.MonitorServer;
        if (host == null) return null;
        System.Int32 processId;
        pkg.Read(out processId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.MonitorServer).GetMethod("StopServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.MonitorServer:StopServer");
            if (isValid==false)
                return null;
        }
        host.StopServer(processId);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1956455141,"GlobalMapFindPath",typeof(HExe_1956455141))]
public class HExe_1956455141: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.PathFindServer host = obj as Server.PathFindServer;
        if (host == null) return null;
        System.Guid hallsId;
        pkg.Read(out hallsId);
        System.Guid mapSourceId;
        pkg.Read(out mapSourceId);
        System.Guid mapInstanceId;
        pkg.Read(out mapInstanceId);
        System.Guid roleId;
        pkg.Read(out roleId);
        SlimDX.Vector3 from;
        pkg.Read(out from);
        SlimDX.Vector3 to;
        pkg.Read(out to);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.PathFindServer).GetMethod("GlobalMapFindPath");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.PathFindServer:GlobalMapFindPath");
            if (isValid==false)
                return null;
        }
        host.GlobalMapFindPath(hallsId,mapSourceId,mapInstanceId,roleId,from,to,connect,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(2820074252,"ReloadNavigation",typeof(HExe_2820074252))]
public class HExe_2820074252: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.PathFindServer host = obj as Server.PathFindServer;
        if (host == null) return null;
        System.Guid planesId;
        pkg.Read(out planesId);
        System.Guid mapSourceId;
        pkg.Read(out mapSourceId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.PathFindServer).GetMethod("ReloadNavigation");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.PathFindServer:ReloadNavigation");
            if (isValid==false)
                return null;
        }
        host.ReloadNavigation(planesId,mapSourceId,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3925432124,"SetMapBlocks",typeof(HExe_3925432124))]
public class HExe_3925432124: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.PathFindServer host = obj as Server.PathFindServer;
        if (host == null) return null;
        System.Guid planesId;
        pkg.Read(out planesId);
        System.Guid mapSourceId;
        pkg.Read(out mapSourceId);
        System.Guid mapInstanceId;
        pkg.Read(out mapInstanceId);
        RPC.DataReader modifyBlocks;
        pkg.Read(out modifyBlocks);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.PathFindServer).GetMethod("SetMapBlocks");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.PathFindServer:SetMapBlocks");
            if (isValid==false)
                return null;
        }
        host.SetMapBlocks(planesId,mapSourceId,mapInstanceId,modifyBlocks,connect,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3592285600,"RegGateServer",typeof(HExe_3592285600))]
public class HExe_3592285600: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        System.Guid id;
        pkg.Read(out id);
        System.UInt16 port;
        pkg.Read(out port);
        System.String ip;
        pkg.Read(out ip);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("RegGateServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:RegGateServer");
            if (isValid==false)
                return null;
        }
        return host.RegGateServer(id,port,ip,connect);
    }
}
[RPC.RPCMethordExecuterTypeAttribute(404836513,"GetGateServers",typeof(HExe_404836513))]
public class HExe_404836513: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("GetGateServers");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:GetGateServers");
            if (isValid==false)
                return null;
        }
        return host.GetGateServers();
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1238154418,"RegHallServer",typeof(HExe_1238154418))]
public class HExe_1238154418: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        System.Guid id;
        pkg.Read(out id);
        System.UInt16 port;
        pkg.Read(out port);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("RegHallServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:RegHallServer");
            if (isValid==false)
                return null;
        }
        return host.RegHallServer(id,port,connect);
    }
}
[RPC.RPCMethordExecuterTypeAttribute(2309804327,"RegDataServer",typeof(HExe_2309804327))]
public class HExe_2309804327: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        System.Guid id;
        pkg.Read(out id);
        System.UInt16 port;
        pkg.Read(out port);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("RegDataServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:RegDataServer");
            if (isValid==false)
                return null;
        }
        return host.RegDataServer(id,port,connect);
    }
}
[RPC.RPCMethordExecuterTypeAttribute(2649410743,"RegPathFindServer",typeof(HExe_2649410743))]
public class HExe_2649410743: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        System.Guid id;
        pkg.Read(out id);
        System.UInt16 port;
        pkg.Read(out port);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("RegPathFindServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:RegPathFindServer");
            if (isValid==false)
                return null;
        }
        return host.RegPathFindServer(id,port,connect);
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1263959954,"GetPathFindServers",typeof(HExe_1263959954))]
public class HExe_1263959954: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("GetPathFindServers");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:GetPathFindServers");
            if (isValid==false)
                return null;
        }
        host.GetPathFindServers(connect,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1309958505,"RegComServer",typeof(HExe_1309958505))]
public class HExe_1309958505: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        System.Guid id;
        pkg.Read(out id);
        System.UInt16 port;
        pkg.Read(out port);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("RegComServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:RegComServer");
            if (isValid==false)
                return null;
        }
        return host.RegComServer(id,port,connect);
    }
}
[RPC.RPCMethordExecuterTypeAttribute(4086803099,"RegLogServer",typeof(HExe_4086803099))]
public class HExe_4086803099: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        System.Guid id;
        pkg.Read(out id);
        System.UInt16 port;
        pkg.Read(out port);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("RegLogServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:RegLogServer");
            if (isValid==false)
                return null;
        }
        return host.RegLogServer(id,port,connect);
    }
}
[RPC.RPCMethordExecuterTypeAttribute(142723755,"SetGateLinkNumber",typeof(HExe_142723755))]
public class HExe_142723755: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        System.Int32 num;
        pkg.Read(out num);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("SetGateLinkNumber");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:SetGateLinkNumber");
            if (isValid==false)
                return null;
        }
        host.SetGateLinkNumber(connect,num);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(691955809,"GetLowGateServer",typeof(HExe_691955809))]
public class HExe_691955809: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("GetLowGateServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:GetLowGateServer");
            if (isValid==false)
                return null;
        }
        return host.GetLowGateServer(connect);
    }
}
[RPC.RPCMethordExecuterTypeAttribute(4084570710,"GetDataServer",typeof(HExe_4084570710))]
public class HExe_4084570710: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("GetDataServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:GetDataServer");
            if (isValid==false)
                return null;
        }
        return host.GetDataServer();
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1456081368,"GetComServer",typeof(HExe_1456081368))]
public class HExe_1456081368: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("GetComServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:GetComServer");
            if (isValid==false)
                return null;
        }
        return host.GetComServer();
    }
}
[RPC.RPCMethordExecuterTypeAttribute(710675388,"GetLogServer",typeof(HExe_710675388))]
public class HExe_710675388: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("GetLogServer");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:GetLogServer");
            if (isValid==false)
                return null;
        }
        return host.GetLogServer();
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1469226417,"GetHallServers",typeof(HExe_1469226417))]
public class HExe_1469226417: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("GetHallServers");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:GetHallServers");
            if (isValid==false)
                return null;
        }
        return host.GetHallServers();
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3855681426,"GetAllHallsInfo",typeof(HExe_3855681426))]
public class HExe_3855681426: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("GetAllHallsInfo");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:GetAllHallsInfo");
            if (isValid==false)
                return null;
        }
        return host.GetAllHallsInfo();
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3417921045,"UpdatePlanesInfo",typeof(HExe_3417921045))]
public class HExe_3417921045: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 400; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.RegisterServer host = obj as Server.RegisterServer;
        if (host == null) return null;
        RPC.DataReader info;
        pkg.Read(out info);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.RegisterServer).GetMethod("UpdatePlanesInfo");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.RegisterServer:UpdatePlanesInfo");
            if (isValid==false)
                return null;
        }
        host.UpdatePlanesInfo(info);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1432321066,"RPC_FSMChangeState",typeof(HExe_1432321066))]
public class HExe_1432321066: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        System.UInt32 singleId;
        pkg.Read(out singleId);
        System.String curState;
        pkg.Read(out curState);
        System.String newCurState;
        pkg.Read(out newCurState);
        System.String newtarState;
        pkg.Read(out newtarState);
        RPC.DataReader parameter;
        pkg.Read(out parameter);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_FSMChangeState");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_FSMChangeState");
            if (isValid==false)
                return null;
        }
        host.RPC_FSMChangeState(singleId,curState,newCurState,newtarState,parameter,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(923366459,"RPC_UpDateRoleState",typeof(HExe_923366459))]
public class HExe_923366459: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        System.UInt32 roleid;
        pkg.Read(out roleid);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_UpDateRoleState");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_UpDateRoleState");
            if (isValid==false)
                return null;
        }
        host.RPC_UpDateRoleState(roleid);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(4147218322,"RPC_UpdatePosition",typeof(HExe_4147218322))]
public class HExe_4147218322: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        SlimDX.Vector3 pos;
        pkg.Read(out pos);
        System.Single dir;
        pkg.Read(out dir);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_UpdatePosition");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_UpdatePosition");
            if (isValid==false)
                return null;
        }
        host.RPC_UpdatePosition(pos,dir,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(2453043333,"RPC_UpdateDirection",typeof(HExe_2453043333))]
public class HExe_2453043333: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        System.UInt32 singeId;
        pkg.Read(out singeId);
        System.Single dir;
        pkg.Read(out dir);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_UpdateDirection");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_UpdateDirection");
            if (isValid==false)
                return null;
        }
        host.RPC_UpdateDirection(singeId,dir,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(2095998032,"RPC_GetRoleCreateInfo",typeof(HExe_2095998032))]
public class HExe_2095998032: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        System.UInt32 singleId;
        pkg.Read(out singleId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_GetRoleCreateInfo");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_GetRoleCreateInfo");
            if (isValid==false)
                return null;
        }
        host.RPC_GetRoleCreateInfo(singleId,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3084239928,"RPC_RelifeMe",typeof(HExe_3084239928))]
public class HExe_3084239928: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_RelifeMe");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_RelifeMe");
            if (isValid==false)
                return null;
        }
        host.RPC_RelifeMe();
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1246044490,"RPC_GetRolePosition",typeof(HExe_1246044490))]
public class HExe_1246044490: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        System.UInt32 singleId;
        pkg.Read(out singleId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_GetRolePosition");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_GetRolePosition");
            if (isValid==false)
                return null;
        }
        host.RPC_GetRolePosition(singleId,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1382946689,"RPC_GetRoleState",typeof(HExe_1382946689))]
public class HExe_1382946689: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        System.UInt32 singleId;
        pkg.Read(out singleId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_GetRoleState");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_GetRoleState");
            if (isValid==false)
                return null;
        }
        host.RPC_GetRoleState(singleId,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(801428783,"RPC_GetCurrentZeusTime",typeof(HExe_801428783))]
public class HExe_801428783: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        System.Int32 timeAccelerate;
        pkg.Read(out timeAccelerate);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_GetCurrentZeusTime");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_GetCurrentZeusTime");
            if (isValid==false)
                return null;
        }
        host.RPC_GetCurrentZeusTime(timeAccelerate,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3031747107,"RPC_GlobalMapFindPath",typeof(HExe_3031747107))]
public class HExe_3031747107: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        SlimDX.Vector3 from;
        pkg.Read(out from);
        SlimDX.Vector3 to;
        pkg.Read(out to);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_GlobalMapFindPath");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_GlobalMapFindPath");
            if (isValid==false)
                return null;
        }
        host.RPC_GlobalMapFindPath(from,to,connect,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1183517947,"RPC_GetDynamicBlockValue",typeof(HExe_1183517947))]
public class HExe_1183517947: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        System.Guid actorId;
        pkg.Read(out actorId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_GetDynamicBlockValue");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_GetDynamicBlockValue");
            if (isValid==false)
                return null;
        }
        host.RPC_GetDynamicBlockValue(actorId,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1208258282,"RPC_SkillLevelUp",typeof(HExe_1208258282))]
public class HExe_1208258282: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        System.Int32 index;
        pkg.Read(out index);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_SkillLevelUp");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_SkillLevelUp");
            if (isValid==false)
                return null;
        }
        host.RPC_SkillLevelUp(index,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1643551847,"RPC_FireInitiativeSkill",typeof(HExe_1643551847))]
public class HExe_1643551847: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 100; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Server.Hall.Role.Player.PlayerInstance host = obj as Server.Hall.Role.Player.PlayerInstance;
        if (host == null) return null;
        System.UInt16 skillId;
        pkg.Read(out skillId);
        System.UInt32 lockroleid;
        pkg.Read(out lockroleid);
        SlimDX.Vector3 summonPos;
        pkg.Read(out summonPos);
        SlimDX.Vector3 tarPos;
        pkg.Read(out tarPos);
        SlimDX.Vector3 tarDir;
        pkg.Read(out tarDir);
        System.Single tarAngle;
        pkg.Read(out tarAngle);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Server.Hall.Role.Player.PlayerInstance).GetMethod("RPC_FireInitiativeSkill");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Server.Hall.Role.Player.PlayerInstance:RPC_FireInitiativeSkill");
            if (isValid==false)
                return null;
        }
        host.RPC_FireInitiativeSkill(skillId,lockroleid,summonPos,tarPos,tarDir,tarAngle,fwd);
return null;
    }
}
public class MappingHashCode2Index{
public static void BuildMapping(){
    RPC.RPCNetworkMgr.AddExecuterIndxer(557012418 , 0);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3802170108 , 1);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1095519736 , 2);
    RPC.RPCNetworkMgr.AddExecuterIndxer(2214271257 , 0);
    RPC.RPCNetworkMgr.AddExecuterIndxer(2186626402 , 1);
    RPC.RPCNetworkMgr.AddExecuterIndxer(2854127094 , 3);
    RPC.RPCNetworkMgr.AddExecuterIndxer(4254261451 , 0);
    RPC.RPCNetworkMgr.AddExecuterIndxer(876228768 , 1);
    RPC.RPCNetworkMgr.AddExecuterIndxer(693389729 , 4);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3191202514 , 2);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3754335694 , 3);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3517965434 , 6);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1858890362 , 0);
    RPC.RPCNetworkMgr.AddExecuterIndxer(989949548 , 14);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1956455141 , 2);
    RPC.RPCNetworkMgr.AddExecuterIndxer(2820074252 , 1);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3925432124 , 0);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3592285600 , 16);
    RPC.RPCNetworkMgr.AddExecuterIndxer(404836513 , 1);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1238154418 , 2);
    RPC.RPCNetworkMgr.AddExecuterIndxer(2309804327 , 3);
    RPC.RPCNetworkMgr.AddExecuterIndxer(2649410743 , 4);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1263959954 , 5);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1309958505 , 6);
    RPC.RPCNetworkMgr.AddExecuterIndxer(4086803099 , 7);
    RPC.RPCNetworkMgr.AddExecuterIndxer(142723755 , 8);
    RPC.RPCNetworkMgr.AddExecuterIndxer(691955809 , 9);
    RPC.RPCNetworkMgr.AddExecuterIndxer(4084570710 , 10);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1456081368 , 11);
    RPC.RPCNetworkMgr.AddExecuterIndxer(710675388 , 12);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1469226417 , 13);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3855681426 , 14);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3417921045 , 15);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1432321066 , 0);
    RPC.RPCNetworkMgr.AddExecuterIndxer(923366459 , 10);
    RPC.RPCNetworkMgr.AddExecuterIndxer(4147218322 , 1);
    RPC.RPCNetworkMgr.AddExecuterIndxer(2453043333 , 2);
    RPC.RPCNetworkMgr.AddExecuterIndxer(2095998032 , 3);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3084239928 , 12);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1246044490 , 5);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1382946689 , 6);
    RPC.RPCNetworkMgr.AddExecuterIndxer(801428783 , 7);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3031747107 , 8);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1183517947 , 4);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1208258282 , 9);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1643551847 , 11);
}
}

}
