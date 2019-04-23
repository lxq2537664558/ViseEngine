//Server Caller
namespace Server{
public class H_RPCRoot
{
    public static Server.H_RPCRoot smInstance = new Server.H_RPCRoot();
    public Server.H_RegisterServer HGet_RegServer(RPC.PackageWriter pkg)
    {
        pkg.PushStack(0);
        return Server.H_RegisterServer.smInstance;
    }
    public Server.H_GateServer HGet_GateServer(RPC.PackageWriter pkg)
    {
        pkg.PushStack(1);
        return Server.H_GateServer.smInstance;
    }
    public Server.H_DataServer HGet_DataServer(RPC.PackageWriter pkg)
    {
        pkg.PushStack(2);
        return Server.H_DataServer.smInstance;
    }
    public Server.H_HallServer HGet_HallServer(RPC.PackageWriter pkg)
    {
        pkg.PushStack(3);
        return Server.H_HallServer.smInstance;
    }
    public Server.H_PathFindServer HGet_PathFindServer(RPC.PackageWriter pkg)
    {
        pkg.PushStack(4);
        return Server.H_PathFindServer.smInstance;
    }
    public Server.H_ComServer HGet_ComServer(RPC.PackageWriter pkg)
    {
        pkg.PushStack(5);
        return Server.H_ComServer.smInstance;
    }
    public Server.H_LogServer HGet_LogServer(RPC.PackageWriter pkg)
    {
        pkg.PushStack(6);
        return Server.H_LogServer.smInstance;
    }
    public Server.H_MonitorServer HGet_MonitorServer(RPC.PackageWriter pkg)
    {
        pkg.PushStack(7);
        return Server.H_MonitorServer.smInstance;
    }
}
}
namespace Server{
public class H_ComServer
{
    public static Server.H_ComServer smInstance = new Server.H_ComServer();
}
}
namespace Server{
public class H_DataServer
{
    public static Server.H_DataServer smInstance = new Server.H_DataServer();
    static RPC.RPCCallerCounter sCCTer_RegGateServer = RPC.RPCEntrance.GetCallerCounter(557012418,"Server.DataServer.RegGateServer");
    public void RegGateServer(RPC.PackageWriter pkg,System.String ip,System.UInt16 port,System.Guid id)
    {
        pkg.Write(ip);
        pkg.Write(port);
        pkg.Write(id);
        pkg.SetMethod(0);
        pkg.CallerCounter = sCCTer_RegGateServer;
    }
    static RPC.RPCCallerCounter sCCTer_RegHallServer = RPC.RPCEntrance.GetCallerCounter(3802170108,"Server.DataServer.RegHallServer");
    public void RegHallServer(RPC.PackageWriter pkg,System.String ip,System.UInt16 port,System.Guid id,System.Single power)
    {
        pkg.Write(ip);
        pkg.Write(port);
        pkg.Write(id);
        pkg.Write(power);
        pkg.SetMethod(1);
        pkg.CallerCounter = sCCTer_RegHallServer;
    }
    static RPC.RPCCallerCounter sCCTer_LoginRole = RPC.RPCEntrance.GetCallerCounter(1095519736,"Server.DataServer.LoginRole");
    public void LoginRole(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(2);
        pkg.CallerCounter = sCCTer_LoginRole;
    }
}
}
namespace Server{
public class H_GateServer
{
    public static Server.H_GateServer smInstance = new Server.H_GateServer();
    static RPC.RPCCallerCounter sCCTer_TestRPCVersion = RPC.RPCEntrance.GetCallerCounter(2214271257,"Server.GateServer.TestRPCVersion");
    public void TestRPCVersion(RPC.PackageWriter pkg,System.Int32 clientHash,System.Int32 serverHash)
    {
        pkg.Write(clientHash);
        pkg.Write(serverHash);
        pkg.SetMethod(0);
        pkg.CallerCounter = sCCTer_TestRPCVersion;
    }
    static RPC.RPCCallerCounter sCCTer_TryEnterGame = RPC.RPCEntrance.GetCallerCounter(2186626402,"Server.GateServer.TryEnterGame");
    public void TryEnterGame(RPC.PackageWriter pkg,System.Guid mapSourceId,System.UInt16 roleTemplateId)
    {
        pkg.Write(mapSourceId);
        pkg.Write(roleTemplateId);
        pkg.SetMethod(1);
        pkg.CallerCounter = sCCTer_TryEnterGame;
    }
    static RPC.RPCCallerCounter sCCTer_GM_ReloadTemplate = RPC.RPCEntrance.GetCallerCounter(2854127094,"Server.GateServer.GM_ReloadTemplate");
    public void GM_ReloadTemplate(RPC.PackageWriter pkg,System.String templateType,System.UInt16 id,System.Byte opType)
    {
        pkg.Write(templateType);
        pkg.Write(id);
        pkg.Write(opType);
        pkg.SetMethod(3);
        pkg.CallerCounter = sCCTer_GM_ReloadTemplate;
    }
}
}
namespace Server{
public class H_HallServer
{
    public static Server.H_HallServer smInstance = new Server.H_HallServer();
    static RPC.RPCCallerCounter sCCTer_GetHallServerId = RPC.RPCEntrance.GetCallerCounter(4254261451,"Server.HallServer.GetHallServerId");
    public void GetHallServerId(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(0);
        pkg.CallerCounter = sCCTer_GetHallServerId;
    }
    static RPC.RPCCallerCounter sCCTer_RegGateServer = RPC.RPCEntrance.GetCallerCounter(876228768,"Server.HallServer.RegGateServer");
    public void RegGateServer(RPC.PackageWriter pkg,System.String ip,System.UInt16 port,System.Guid id)
    {
        pkg.Write(ip);
        pkg.Write(port);
        pkg.Write(id);
        pkg.SetMethod(1);
        pkg.CallerCounter = sCCTer_RegGateServer;
    }
    static RPC.RPCCallerCounter sCCTer_EnterMap = RPC.RPCEntrance.GetCallerCounter(693389729,"Server.HallServer.EnterMap");
    public void EnterMap(RPC.PackageWriter pkg,System.UInt16 cltHandle,System.String ip,System.Int32 port,System.Guid mapSourceId,System.UInt16 roleTemplateId)
    {
        pkg.Write(cltHandle);
        pkg.Write(ip);
        pkg.Write(port);
        pkg.Write(mapSourceId);
        pkg.Write(roleTemplateId);
        pkg.SetMethod(4);
        pkg.CallerCounter = sCCTer_EnterMap;
    }
    static RPC.RPCCallerCounter sCCTer_LeaveMap = RPC.RPCEntrance.GetCallerCounter(3191202514,"Server.HallServer.LeaveMap");
    public void LeaveMap(RPC.PackageWriter pkg,System.UInt16 cltHandle)
    {
        pkg.Write(cltHandle);
        pkg.SetMethod(2);
        pkg.CallerCounter = sCCTer_LeaveMap;
    }
    static RPC.RPCCallerCounter sCCTer_RemoveInstanceMap = RPC.RPCEntrance.GetCallerCounter(3754335694,"Server.HallServer.RemoveInstanceMap");
    public void RemoveInstanceMap(RPC.PackageWriter pkg,System.Guid mapInstanceId)
    {
        pkg.Write(mapInstanceId);
        pkg.SetMethod(3);
        pkg.CallerCounter = sCCTer_RemoveInstanceMap;
    }
    static RPC.RPCCallerCounter sCCTer_GM_ReloadTemplate = RPC.RPCEntrance.GetCallerCounter(3517965434,"Server.HallServer.GM_ReloadTemplate");
    public void GM_ReloadTemplate(RPC.PackageWriter pkg,System.String templateType,System.UInt16 id,System.Byte opType)
    {
        pkg.Write(templateType);
        pkg.Write(id);
        pkg.Write(opType);
        pkg.SetMethod(6);
        pkg.CallerCounter = sCCTer_GM_ReloadTemplate;
    }
}
}
namespace Server{
public class H_LogServer
{
    public static Server.H_LogServer smInstance = new Server.H_LogServer();
}
}
namespace Server{
public class H_MonitorServer
{
    public static Server.H_MonitorServer smInstance = new Server.H_MonitorServer();
    static RPC.RPCCallerCounter sCCTer_StartNewServer = RPC.RPCEntrance.GetCallerCounter(1858890362,"Server.MonitorServer.StartNewServer");
    public void StartNewServer(RPC.PackageWriter pkg,System.String name,System.String args)
    {
        pkg.Write(name);
        pkg.Write(args);
        pkg.SetMethod(0);
        pkg.CallerCounter = sCCTer_StartNewServer;
    }
    static RPC.RPCCallerCounter sCCTer_StopServer = RPC.RPCEntrance.GetCallerCounter(989949548,"Server.MonitorServer.StopServer");
    public void StopServer(RPC.PackageWriter pkg,System.Int32 processId)
    {
        pkg.Write(processId);
        pkg.SetMethod(14);
        pkg.CallerCounter = sCCTer_StopServer;
    }
}
}
namespace Server{
public class H_PathFindServer
{
    public static Server.H_PathFindServer smInstance = new Server.H_PathFindServer();
    static RPC.RPCCallerCounter sCCTer_GlobalMapFindPath = RPC.RPCEntrance.GetCallerCounter(1956455141,"Server.PathFindServer.GlobalMapFindPath");
    public void GlobalMapFindPath(RPC.PackageWriter pkg,System.Guid hallsId,System.Guid mapSourceId,System.Guid mapInstanceId,System.Guid roleId,SlimDX.Vector3 from,SlimDX.Vector3 to)
    {
        pkg.Write(hallsId);
        pkg.Write(mapSourceId);
        pkg.Write(mapInstanceId);
        pkg.Write(roleId);
        pkg.Write(from);
        pkg.Write(to);
        pkg.SetMethod(2);
        pkg.CallerCounter = sCCTer_GlobalMapFindPath;
        pkg.SetWeakPkg();
    }
    static RPC.RPCCallerCounter sCCTer_ReloadNavigation = RPC.RPCEntrance.GetCallerCounter(2820074252,"Server.PathFindServer.ReloadNavigation");
    public void ReloadNavigation(RPC.PackageWriter pkg,System.Guid planesId,System.Guid mapSourceId)
    {
        pkg.Write(planesId);
        pkg.Write(mapSourceId);
        pkg.SetMethod(1);
        pkg.CallerCounter = sCCTer_ReloadNavigation;
    }
    static RPC.RPCCallerCounter sCCTer_SetMapBlocks = RPC.RPCEntrance.GetCallerCounter(3925432124,"Server.PathFindServer.SetMapBlocks");
    public void SetMapBlocks(RPC.PackageWriter pkg,System.Guid planesId,System.Guid mapSourceId,System.Guid mapInstanceId,RPC.DataWriter modifyBlocks)
    {
        pkg.Write(planesId);
        pkg.Write(mapSourceId);
        pkg.Write(mapInstanceId);
        pkg.Write(modifyBlocks);
        pkg.SetMethod(0);
        pkg.CallerCounter = sCCTer_SetMapBlocks;
    }
}
}
namespace Server{
public class H_RegisterServer
{
    public static Server.H_RegisterServer smInstance = new Server.H_RegisterServer();
    static RPC.RPCCallerCounter sCCTer_RegGateServer = RPC.RPCEntrance.GetCallerCounter(3592285600,"Server.RegisterServer.RegGateServer");
    public void RegGateServer(RPC.PackageWriter pkg,System.Guid id,System.UInt16 port,System.String ip)
    {
        pkg.Write(id);
        pkg.Write(port);
        pkg.Write(ip);
        pkg.SetMethod(16);
        pkg.CallerCounter = sCCTer_RegGateServer;
    }
    static RPC.RPCCallerCounter sCCTer_GetGateServers = RPC.RPCEntrance.GetCallerCounter(404836513,"Server.RegisterServer.GetGateServers");
    public void GetGateServers(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(1);
        pkg.CallerCounter = sCCTer_GetGateServers;
    }
    static RPC.RPCCallerCounter sCCTer_RegHallServer = RPC.RPCEntrance.GetCallerCounter(1238154418,"Server.RegisterServer.RegHallServer");
    public void RegHallServer(RPC.PackageWriter pkg,System.Guid id,System.UInt16 port)
    {
        pkg.Write(id);
        pkg.Write(port);
        pkg.SetMethod(2);
        pkg.CallerCounter = sCCTer_RegHallServer;
    }
    static RPC.RPCCallerCounter sCCTer_RegDataServer = RPC.RPCEntrance.GetCallerCounter(2309804327,"Server.RegisterServer.RegDataServer");
    public void RegDataServer(RPC.PackageWriter pkg,System.Guid id,System.UInt16 port)
    {
        pkg.Write(id);
        pkg.Write(port);
        pkg.SetMethod(3);
        pkg.CallerCounter = sCCTer_RegDataServer;
    }
    static RPC.RPCCallerCounter sCCTer_RegPathFindServer = RPC.RPCEntrance.GetCallerCounter(2649410743,"Server.RegisterServer.RegPathFindServer");
    public void RegPathFindServer(RPC.PackageWriter pkg,System.Guid id,System.UInt16 port)
    {
        pkg.Write(id);
        pkg.Write(port);
        pkg.SetMethod(4);
        pkg.CallerCounter = sCCTer_RegPathFindServer;
    }
    static RPC.RPCCallerCounter sCCTer_GetPathFindServers = RPC.RPCEntrance.GetCallerCounter(1263959954,"Server.RegisterServer.GetPathFindServers");
    public void GetPathFindServers(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(5);
        pkg.CallerCounter = sCCTer_GetPathFindServers;
    }
    static RPC.RPCCallerCounter sCCTer_RegComServer = RPC.RPCEntrance.GetCallerCounter(1309958505,"Server.RegisterServer.RegComServer");
    public void RegComServer(RPC.PackageWriter pkg,System.Guid id,System.UInt16 port)
    {
        pkg.Write(id);
        pkg.Write(port);
        pkg.SetMethod(6);
        pkg.CallerCounter = sCCTer_RegComServer;
    }
    static RPC.RPCCallerCounter sCCTer_RegLogServer = RPC.RPCEntrance.GetCallerCounter(4086803099,"Server.RegisterServer.RegLogServer");
    public void RegLogServer(RPC.PackageWriter pkg,System.Guid id,System.UInt16 port)
    {
        pkg.Write(id);
        pkg.Write(port);
        pkg.SetMethod(7);
        pkg.CallerCounter = sCCTer_RegLogServer;
    }
    static RPC.RPCCallerCounter sCCTer_SetGateLinkNumber = RPC.RPCEntrance.GetCallerCounter(142723755,"Server.RegisterServer.SetGateLinkNumber");
    public void SetGateLinkNumber(RPC.PackageWriter pkg,System.Int32 num)
    {
        pkg.Write(num);
        pkg.SetMethod(8);
        pkg.CallerCounter = sCCTer_SetGateLinkNumber;
    }
    static RPC.RPCCallerCounter sCCTer_GetLowGateServer = RPC.RPCEntrance.GetCallerCounter(691955809,"Server.RegisterServer.GetLowGateServer");
    public void GetLowGateServer(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(9);
        pkg.CallerCounter = sCCTer_GetLowGateServer;
    }
    static RPC.RPCCallerCounter sCCTer_GetDataServer = RPC.RPCEntrance.GetCallerCounter(4084570710,"Server.RegisterServer.GetDataServer");
    public void GetDataServer(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(10);
        pkg.CallerCounter = sCCTer_GetDataServer;
    }
    static RPC.RPCCallerCounter sCCTer_GetComServer = RPC.RPCEntrance.GetCallerCounter(1456081368,"Server.RegisterServer.GetComServer");
    public void GetComServer(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(11);
        pkg.CallerCounter = sCCTer_GetComServer;
    }
    static RPC.RPCCallerCounter sCCTer_GetLogServer = RPC.RPCEntrance.GetCallerCounter(710675388,"Server.RegisterServer.GetLogServer");
    public void GetLogServer(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(12);
        pkg.CallerCounter = sCCTer_GetLogServer;
    }
    static RPC.RPCCallerCounter sCCTer_GetHallServers = RPC.RPCEntrance.GetCallerCounter(1469226417,"Server.RegisterServer.GetHallServers");
    public void GetHallServers(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(13);
        pkg.CallerCounter = sCCTer_GetHallServers;
    }
    static RPC.RPCCallerCounter sCCTer_GetAllHallsInfo = RPC.RPCEntrance.GetCallerCounter(3855681426,"Server.RegisterServer.GetAllHallsInfo");
    public void GetAllHallsInfo(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(14);
        pkg.CallerCounter = sCCTer_GetAllHallsInfo;
    }
    static RPC.RPCCallerCounter sCCTer_UpdatePlanesInfo = RPC.RPCEntrance.GetCallerCounter(3417921045,"Server.RegisterServer.UpdatePlanesInfo");
    public void UpdatePlanesInfo(RPC.PackageWriter pkg,RPC.DataWriter info)
    {
        pkg.Write(info);
        pkg.SetMethod(15);
        pkg.CallerCounter = sCCTer_UpdatePlanesInfo;
    }
}
}
namespace Server.Hall.Role{
public class H_RoleActor
{
    public static Server.Hall.Role.H_RoleActor smInstance = new Server.Hall.Role.H_RoleActor();
}
}
namespace Server.Hall.Role.Player{
public class H_PlayerInstance
{
    public static Server.Hall.Role.Player.H_PlayerInstance smInstance = new Server.Hall.Role.Player.H_PlayerInstance();
    static RPC.RPCCallerCounter sCCTer_RPC_FSMChangeState = RPC.RPCEntrance.GetCallerCounter(1432321066,"Server.Hall.Role.Player.PlayerInstance.RPC_FSMChangeState");
    public void RPC_FSMChangeState(RPC.PackageWriter pkg,System.UInt32 singleId,System.String curState,System.String newCurState,System.String newtarState,RPC.DataWriter parameter)
    {
        pkg.Write(singleId);
        pkg.Write(curState);
        pkg.Write(newCurState);
        pkg.Write(newtarState);
        pkg.Write(parameter);
        pkg.SetMethod(0);
        pkg.CallerCounter = sCCTer_RPC_FSMChangeState;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_UpDateRoleState = RPC.RPCEntrance.GetCallerCounter(923366459,"Server.Hall.Role.Player.PlayerInstance.RPC_UpDateRoleState");
    public void RPC_UpDateRoleState(RPC.PackageWriter pkg,System.UInt32 roleid)
    {
        pkg.Write(roleid);
        pkg.SetMethod(10);
        pkg.CallerCounter = sCCTer_RPC_UpDateRoleState;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_UpdatePosition = RPC.RPCEntrance.GetCallerCounter(4147218322,"Server.Hall.Role.Player.PlayerInstance.RPC_UpdatePosition");
    public void RPC_UpdatePosition(RPC.PackageWriter pkg,SlimDX.Vector3 pos,System.Single dir)
    {
        pkg.Write(pos);
        pkg.Write(dir);
        pkg.SetMethod(1);
        pkg.CallerCounter = sCCTer_RPC_UpdatePosition;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_UpdateDirection = RPC.RPCEntrance.GetCallerCounter(2453043333,"Server.Hall.Role.Player.PlayerInstance.RPC_UpdateDirection");
    public void RPC_UpdateDirection(RPC.PackageWriter pkg,System.UInt32 singeId,System.Single dir)
    {
        pkg.Write(singeId);
        pkg.Write(dir);
        pkg.SetMethod(2);
        pkg.CallerCounter = sCCTer_RPC_UpdateDirection;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_GetRoleCreateInfo = RPC.RPCEntrance.GetCallerCounter(2095998032,"Server.Hall.Role.Player.PlayerInstance.RPC_GetRoleCreateInfo");
    public void RPC_GetRoleCreateInfo(RPC.PackageWriter pkg,System.UInt32 singleId)
    {
        pkg.Write(singleId);
        pkg.SetMethod(3);
        pkg.CallerCounter = sCCTer_RPC_GetRoleCreateInfo;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_RelifeMe = RPC.RPCEntrance.GetCallerCounter(3084239928,"Server.Hall.Role.Player.PlayerInstance.RPC_RelifeMe");
    public void RPC_RelifeMe(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(12);
        pkg.CallerCounter = sCCTer_RPC_RelifeMe;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_GetRolePosition = RPC.RPCEntrance.GetCallerCounter(1246044490,"Server.Hall.Role.Player.PlayerInstance.RPC_GetRolePosition");
    public void RPC_GetRolePosition(RPC.PackageWriter pkg,System.UInt32 singleId)
    {
        pkg.Write(singleId);
        pkg.SetMethod(5);
        pkg.CallerCounter = sCCTer_RPC_GetRolePosition;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_GetRoleState = RPC.RPCEntrance.GetCallerCounter(1382946689,"Server.Hall.Role.Player.PlayerInstance.RPC_GetRoleState");
    public void RPC_GetRoleState(RPC.PackageWriter pkg,System.UInt32 singleId)
    {
        pkg.Write(singleId);
        pkg.SetMethod(6);
        pkg.CallerCounter = sCCTer_RPC_GetRoleState;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_GetCurrentZeusTime = RPC.RPCEntrance.GetCallerCounter(801428783,"Server.Hall.Role.Player.PlayerInstance.RPC_GetCurrentZeusTime");
    public void RPC_GetCurrentZeusTime(RPC.PackageWriter pkg,System.Int32 timeAccelerate)
    {
        pkg.Write(timeAccelerate);
        pkg.SetMethod(7);
        pkg.CallerCounter = sCCTer_RPC_GetCurrentZeusTime;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_GlobalMapFindPath = RPC.RPCEntrance.GetCallerCounter(3031747107,"Server.Hall.Role.Player.PlayerInstance.RPC_GlobalMapFindPath");
    public void RPC_GlobalMapFindPath(RPC.PackageWriter pkg,SlimDX.Vector3 from,SlimDX.Vector3 to)
    {
        pkg.Write(from);
        pkg.Write(to);
        pkg.SetMethod(8);
        pkg.CallerCounter = sCCTer_RPC_GlobalMapFindPath;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_GetDynamicBlockValue = RPC.RPCEntrance.GetCallerCounter(1183517947,"Server.Hall.Role.Player.PlayerInstance.RPC_GetDynamicBlockValue");
    public void RPC_GetDynamicBlockValue(RPC.PackageWriter pkg,System.Guid actorId)
    {
        pkg.Write(actorId);
        pkg.SetMethod(4);
        pkg.CallerCounter = sCCTer_RPC_GetDynamicBlockValue;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_SkillLevelUp = RPC.RPCEntrance.GetCallerCounter(1208258282,"Server.Hall.Role.Player.PlayerInstance.RPC_SkillLevelUp");
    public void RPC_SkillLevelUp(RPC.PackageWriter pkg,System.Int32 index)
    {
        pkg.Write(index);
        pkg.SetMethod(9);
        pkg.CallerCounter = sCCTer_RPC_SkillLevelUp;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_FireInitiativeSkill = RPC.RPCEntrance.GetCallerCounter(1643551847,"Server.Hall.Role.Player.PlayerInstance.RPC_FireInitiativeSkill");
    public void RPC_FireInitiativeSkill(RPC.PackageWriter pkg,System.UInt16 skillId,System.UInt32 lockroleid,SlimDX.Vector3 summonPos,SlimDX.Vector3 tarPos,SlimDX.Vector3 tarDir,System.Single tarAngle)
    {
        pkg.Write(skillId);
        pkg.Write(lockroleid);
        pkg.Write(summonPos);
        pkg.Write(tarPos);
        pkg.Write(tarDir);
        pkg.Write(tarAngle);
        pkg.SetMethod(11);
        pkg.CallerCounter = sCCTer_RPC_FireInitiativeSkill;
    }
}
}
namespace Server.Hall.Role.Summon{
public class H_SummonRole
{
    public static Server.Hall.Role.Summon.H_SummonRole smInstance = new Server.Hall.Role.Summon.H_SummonRole();
}
}
public class RPCServerVersion
{
	static RPC.RPCVersionManager smManager = null;
	public static RPC.RPCVersionManager GetManager()
	{
		if (smManager!=null)
			return smManager;
		smManager = new RPC.RPCVersionManager();
		smManager.RegMethodIndex("Server.DataServer", 557012418, 0, "Server.DataServer->RegGateServer");
		smManager.RegMethodIndex("Server.DataServer", 3802170108, 1, "Server.DataServer->RegHallServer");
		smManager.RegMethodIndex("Server.DataServer", 1095519736, 2, "Server.DataServer->LoginRole");
		smManager.RegMethodIndex("Server.GateServer", 2214271257, 0, "Server.GateServer->TestRPCVersion");
		smManager.RegMethodIndex("Server.GateServer", 2186626402, 1, "Server.GateServer->TryEnterGame");
		smManager.RegMethodIndex("Server.GateServer", 2854127094, 3, "Server.GateServer->GM_ReloadTemplate");
		smManager.RegMethodIndex("Server.HallServer", 4254261451, 0, "Server.HallServer->GetHallServerId");
		smManager.RegMethodIndex("Server.HallServer", 876228768, 1, "Server.HallServer->RegGateServer");
		smManager.RegMethodIndex("Server.HallServer", 693389729, 4, "Server.HallServer->EnterMap");
		smManager.RegMethodIndex("Server.HallServer", 3191202514, 2, "Server.HallServer->LeaveMap");
		smManager.RegMethodIndex("Server.HallServer", 3754335694, 3, "Server.HallServer->RemoveInstanceMap");
		smManager.RegMethodIndex("Server.HallServer", 3517965434, 6, "Server.HallServer->GM_ReloadTemplate");
		smManager.RegMethodIndex("Server.MonitorServer", 1858890362, 0, "Server.MonitorServer->StartNewServer");
		smManager.RegMethodIndex("Server.MonitorServer", 989949548, 14, "Server.MonitorServer->StopServer");
		smManager.RegMethodIndex("Server.PathFindServer", 1956455141, 2, "Server.PathFindServer->GlobalMapFindPath");
		smManager.RegMethodIndex("Server.PathFindServer", 2820074252, 1, "Server.PathFindServer->ReloadNavigation");
		smManager.RegMethodIndex("Server.PathFindServer", 3925432124, 0, "Server.PathFindServer->SetMapBlocks");
		smManager.RegMethodIndex("Server.RegisterServer", 3592285600, 16, "Server.RegisterServer->RegGateServer");
		smManager.RegMethodIndex("Server.RegisterServer", 404836513, 1, "Server.RegisterServer->GetGateServers");
		smManager.RegMethodIndex("Server.RegisterServer", 1238154418, 2, "Server.RegisterServer->RegHallServer");
		smManager.RegMethodIndex("Server.RegisterServer", 2309804327, 3, "Server.RegisterServer->RegDataServer");
		smManager.RegMethodIndex("Server.RegisterServer", 2649410743, 4, "Server.RegisterServer->RegPathFindServer");
		smManager.RegMethodIndex("Server.RegisterServer", 1263959954, 5, "Server.RegisterServer->GetPathFindServers");
		smManager.RegMethodIndex("Server.RegisterServer", 1309958505, 6, "Server.RegisterServer->RegComServer");
		smManager.RegMethodIndex("Server.RegisterServer", 4086803099, 7, "Server.RegisterServer->RegLogServer");
		smManager.RegMethodIndex("Server.RegisterServer", 142723755, 8, "Server.RegisterServer->SetGateLinkNumber");
		smManager.RegMethodIndex("Server.RegisterServer", 691955809, 9, "Server.RegisterServer->GetLowGateServer");
		smManager.RegMethodIndex("Server.RegisterServer", 4084570710, 10, "Server.RegisterServer->GetDataServer");
		smManager.RegMethodIndex("Server.RegisterServer", 1456081368, 11, "Server.RegisterServer->GetComServer");
		smManager.RegMethodIndex("Server.RegisterServer", 710675388, 12, "Server.RegisterServer->GetLogServer");
		smManager.RegMethodIndex("Server.RegisterServer", 1469226417, 13, "Server.RegisterServer->GetHallServers");
		smManager.RegMethodIndex("Server.RegisterServer", 3855681426, 14, "Server.RegisterServer->GetAllHallsInfo");
		smManager.RegMethodIndex("Server.RegisterServer", 3417921045, 15, "Server.RegisterServer->UpdatePlanesInfo");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 1432321066, 0, "Server.Hall.Role.Player.PlayerInstance->RPC_FSMChangeState");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 923366459, 10, "Server.Hall.Role.Player.PlayerInstance->RPC_UpDateRoleState");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 4147218322, 1, "Server.Hall.Role.Player.PlayerInstance->RPC_UpdatePosition");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 2453043333, 2, "Server.Hall.Role.Player.PlayerInstance->RPC_UpdateDirection");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 2095998032, 3, "Server.Hall.Role.Player.PlayerInstance->RPC_GetRoleCreateInfo");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 3084239928, 12, "Server.Hall.Role.Player.PlayerInstance->RPC_RelifeMe");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 1246044490, 5, "Server.Hall.Role.Player.PlayerInstance->RPC_GetRolePosition");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 1382946689, 6, "Server.Hall.Role.Player.PlayerInstance->RPC_GetRoleState");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 801428783, 7, "Server.Hall.Role.Player.PlayerInstance->RPC_GetCurrentZeusTime");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 3031747107, 8, "Server.Hall.Role.Player.PlayerInstance->RPC_GlobalMapFindPath");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 1183517947, 4, "Server.Hall.Role.Player.PlayerInstance->RPC_GetDynamicBlockValue");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 1208258282, 9, "Server.Hall.Role.Player.PlayerInstance->RPC_SkillLevelUp");
		smManager.RegMethodIndex("Server.Hall.Role.Player.PlayerInstance", 1643551847, 11, "Server.Hall.Role.Player.PlayerInstance->RPC_FireInitiativeSkill");
		smManager.RPCHashCode = 1535036532;
		return smManager;
	}
}
