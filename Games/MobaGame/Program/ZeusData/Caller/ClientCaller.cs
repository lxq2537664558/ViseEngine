//Server Caller
namespace Client{
public class H_GameRPC
{
    public static Client.H_GameRPC smInstance = new Client.H_GameRPC();
    public Client.Role.H_RoleActor HIndex(RPC.PackageWriter pkg,System.UInt32 i)
    {
        pkg.PushStack(11+0);
        pkg.Write(i);
        return Client.Role.H_RoleActor.smInstance;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_EnterMap = RPC.RPCEntrance.GetCallerCounter(649012910,"Client.GameRPC.RPC_EnterMap");
    public void RPC_EnterMap(RPC.PackageWriter pkg,System.UInt32 singleId,System.Guid mapSourceId,SlimDX.Vector3 pos,RPC.DataWriter sceneParameter)
    {
        pkg.Write(singleId);
        pkg.Write(mapSourceId);
        pkg.Write(pos);
        pkg.Write(sceneParameter);
        pkg.SetMethod(0);
        pkg.CallerCounter = sCCTer_RPC_EnterMap;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_RoleLeave = RPC.RPCEntrance.GetCallerCounter(483678,"Client.GameRPC.RPC_RoleLeave");
    public void RPC_RoleLeave(RPC.PackageWriter pkg,System.UInt32 singleId)
    {
        pkg.Write(singleId);
        pkg.SetMethod(1);
        pkg.CallerCounter = sCCTer_RPC_RoleLeave;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_CreateSummon = RPC.RPCEntrance.GetCallerCounter(3247434719,"Client.GameRPC.RPC_CreateSummon");
    public void RPC_CreateSummon(RPC.PackageWriter pkg,RPC.DataWriter parameter)
    {
        pkg.Write(parameter);
        pkg.SetMethod(2);
        pkg.CallerCounter = sCCTer_RPC_CreateSummon;
    }
}
}
namespace Client.Role{
public class H_RoleActor
{
    public static Client.Role.H_RoleActor smInstance = new Client.Role.H_RoleActor();
    static RPC.RPCCallerCounter sCCTer_RPC_FSMChangeState = RPC.RPCEntrance.GetCallerCounter(2483224510,"Client.Role.RoleActor.RPC_FSMChangeState");
    public void RPC_FSMChangeState(RPC.PackageWriter pkg,System.String curState,System.String newCurState,System.String newtarState,RPC.DataWriter parameter)
    {
        pkg.Write(curState);
        pkg.Write(newCurState);
        pkg.Write(newtarState);
        pkg.Write(parameter);
        pkg.SetMethod(0);
        pkg.CallerCounter = sCCTer_RPC_FSMChangeState;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_UpdatePosition = RPC.RPCEntrance.GetCallerCounter(1014639420,"Client.Role.RoleActor.RPC_UpdatePosition");
    public void RPC_UpdatePosition(RPC.PackageWriter pkg,System.Int16 x,System.Int16 z)
    {
        pkg.Write(x);
        pkg.Write(z);
        pkg.SetMethod(1);
        pkg.CallerCounter = sCCTer_RPC_UpdatePosition;
        pkg.SetWeakPkg();
    }
    static RPC.RPCCallerCounter sCCTer_RPC_UpdateCriticalValue = RPC.RPCEntrance.GetCallerCounter(2325886331,"Client.Role.RoleActor.RPC_UpdateCriticalValue");
    public void RPC_UpdateCriticalValue(RPC.PackageWriter pkg,System.UInt32 attackerid,System.UInt32 targetId,System.UInt16 skillid,System.Int32 value,System.Byte hitType)
    {
        pkg.Write(attackerid);
        pkg.Write(targetId);
        pkg.Write(skillid);
        pkg.Write(value);
        pkg.Write(hitType);
        pkg.SetMethod(7);
        pkg.CallerCounter = sCCTer_RPC_UpdateCriticalValue;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_AddBuff = RPC.RPCEntrance.GetCallerCounter(3100448924,"Client.Role.RoleActor.RPC_AddBuff");
    public void RPC_AddBuff(RPC.PackageWriter pkg,GameData.Skill.BuffData data)
    {
        pkg.Write(data);
        pkg.SetMethod(4);
        pkg.CallerCounter = sCCTer_RPC_AddBuff;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_RemoveBuff = RPC.RPCEntrance.GetCallerCounter(4071610659,"Client.Role.RoleActor.RPC_RemoveBuff");
    public void RPC_RemoveBuff(RPC.PackageWriter pkg,GameData.Skill.BuffData data)
    {
        pkg.Write(data);
        pkg.SetMethod(5);
        pkg.CallerCounter = sCCTer_RPC_RemoveBuff;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_FreshBuff = RPC.RPCEntrance.GetCallerCounter(3447275784,"Client.Role.RoleActor.RPC_FreshBuff");
    public void RPC_FreshBuff(RPC.PackageWriter pkg,System.Guid id,System.Int64 time)
    {
        pkg.Write(id);
        pkg.Write(time);
        pkg.SetMethod(6);
        pkg.CallerCounter = sCCTer_RPC_FreshBuff;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_UpdateRoleValue = RPC.RPCEntrance.GetCallerCounter(473901565,"Client.Role.RoleActor.RPC_UpdateRoleValue");
    public void RPC_UpdateRoleValue(RPC.PackageWriter pkg,System.String name,RPC.DataWriter value)
    {
        pkg.Write(name);
        pkg.Write(value);
        pkg.SetMethod(3);
        pkg.CallerCounter = sCCTer_RPC_UpdateRoleValue;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_LevelUpEffect = RPC.RPCEntrance.GetCallerCounter(3746655369,"Client.Role.RoleActor.RPC_LevelUpEffect");
    public void RPC_LevelUpEffect(RPC.PackageWriter pkg)
    {
        pkg.SetMethod(8);
        pkg.CallerCounter = sCCTer_RPC_LevelUpEffect;
    }
    static RPC.RPCCallerCounter sCCTer_RPC_RoleCollision = RPC.RPCEntrance.GetCallerCounter(3492219995,"Client.Role.RoleActor.RPC_RoleCollision");
    public void RPC_RoleCollision(RPC.PackageWriter pkg,System.UInt32 targetSingleId)
    {
        pkg.Write(targetSingleId);
        pkg.SetMethod(2);
        pkg.CallerCounter = sCCTer_RPC_RoleCollision;
    }
}
}
namespace CCore{
public class H_CommonRPC
{
    public static CCore.H_CommonRPC smInstance = new CCore.H_CommonRPC();
    static RPC.RPCCallerCounter sCCTer_RPC_SetSceneActorProperty = RPC.RPCEntrance.GetCallerCounter(3346036833,"CCore.CommonRPC.RPC_SetSceneActorProperty");
    public void RPC_SetSceneActorProperty(RPC.PackageWriter pkg,System.Guid actorId,System.String propertyName,System.String value)
    {
        pkg.Write(actorId);
        pkg.Write(propertyName);
        pkg.Write(value);
        pkg.SetMethod(0);
        pkg.CallerCounter = sCCTer_RPC_SetSceneActorProperty;
    }
}
}
public class RPCClientVersion
{
	static RPC.RPCVersionManager smManager = null;
	public static RPC.RPCVersionManager GetManager()
	{
		if (smManager!=null)
			return smManager;
		smManager = new RPC.RPCVersionManager();
		smManager.RegMethodIndex("Client.GameRPC", 649012910, 0, "Client.GameRPC->RPC_EnterMap");
		smManager.RegMethodIndex("Client.GameRPC", 483678, 1, "Client.GameRPC->RPC_RoleLeave");
		smManager.RegMethodIndex("Client.GameRPC", 3247434719, 2, "Client.GameRPC->RPC_CreateSummon");
		smManager.RegMethodIndex("Client.Role.RoleActor", 2483224510, 0, "Client.Role.RoleActor->RPC_FSMChangeState");
		smManager.RegMethodIndex("Client.Role.RoleActor", 1014639420, 1, "Client.Role.RoleActor->RPC_UpdatePosition");
		smManager.RegMethodIndex("Client.Role.RoleActor", 2325886331, 7, "Client.Role.RoleActor->RPC_UpdateCriticalValue");
		smManager.RegMethodIndex("Client.Role.RoleActor", 3100448924, 4, "Client.Role.RoleActor->RPC_AddBuff");
		smManager.RegMethodIndex("Client.Role.RoleActor", 4071610659, 5, "Client.Role.RoleActor->RPC_RemoveBuff");
		smManager.RegMethodIndex("Client.Role.RoleActor", 3447275784, 6, "Client.Role.RoleActor->RPC_FreshBuff");
		smManager.RegMethodIndex("Client.Role.RoleActor", 473901565, 3, "Client.Role.RoleActor->RPC_UpdateRoleValue");
		smManager.RegMethodIndex("Client.Role.RoleActor", 3746655369, 8, "Client.Role.RoleActor->RPC_LevelUpEffect");
		smManager.RegMethodIndex("Client.Role.RoleActor", 3492219995, 2, "Client.Role.RoleActor->RPC_RoleCollision");
		smManager.RegMethodIndex("CCore.CommonRPC", 3346036833, 0, "CCore.CommonRPC->RPC_SetSceneActorProperty");
		smManager.RPCHashCode = 1684971488;
		return smManager;
	}
}
