//Server Callee
namespace RPC_ExecuterNamespace{
[RPC.RPCIndexExecuterTypeAttribute(3608465067,"Item",typeof(HIndex_3608465067))]
public class HIndex_3608465067: RPC.RPCIndexerExecuter
{
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override RPC.RPCObject Execute(RPC.RPCObject obj,RPC.PackageProxy pkg)
    {
        Client.GameRPC host = obj as Client.GameRPC;
        if (host == null) return null;
        System.UInt32 i;
        pkg.Read(out i);
        return host[i];
    }
}
[RPC.RPCMethordExecuterTypeAttribute(649012910,"RPC_EnterMap",typeof(HExe_649012910))]
public class HExe_649012910: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.GameRPC host = obj as Client.GameRPC;
        if (host == null) return null;
        System.UInt32 singleId;
        pkg.Read(out singleId);
        System.Guid mapSourceId;
        pkg.Read(out mapSourceId);
        SlimDX.Vector3 pos;
        pkg.Read(out pos);
        RPC.DataReader sceneParameter;
        pkg.Read(out sceneParameter);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.GameRPC).GetMethod("RPC_EnterMap");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.GameRPC:RPC_EnterMap");
            if (isValid==false)
                return null;
        }
        host.RPC_EnterMap(singleId,mapSourceId,pos,sceneParameter);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(483678,"RPC_RoleLeave",typeof(HExe_483678))]
public class HExe_483678: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.GameRPC host = obj as Client.GameRPC;
        if (host == null) return null;
        System.UInt32 singleId;
        pkg.Read(out singleId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.GameRPC).GetMethod("RPC_RoleLeave");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.GameRPC:RPC_RoleLeave");
            if (isValid==false)
                return null;
        }
        host.RPC_RoleLeave(singleId);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3247434719,"RPC_CreateSummon",typeof(HExe_3247434719))]
public class HExe_3247434719: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.GameRPC host = obj as Client.GameRPC;
        if (host == null) return null;
        RPC.DataReader parameter;
        pkg.Read(out parameter);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.GameRPC).GetMethod("RPC_CreateSummon");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.GameRPC:RPC_CreateSummon");
            if (isValid==false)
                return null;
        }
        host.RPC_CreateSummon(parameter);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(2483224510,"RPC_FSMChangeState",typeof(HExe_2483224510))]
public class HExe_2483224510: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.Role.RoleActor host = obj as Client.Role.RoleActor;
        if (host == null) return null;
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
            Method = typeof(Client.Role.RoleActor).GetMethod("RPC_FSMChangeState");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.Role.RoleActor:RPC_FSMChangeState");
            if (isValid==false)
                return null;
        }
        host.RPC_FSMChangeState(curState,newCurState,newtarState,parameter,fwd);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(1014639420,"RPC_UpdatePosition",typeof(HExe_1014639420))]
public class HExe_1014639420: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.Role.RoleActor host = obj as Client.Role.RoleActor;
        if (host == null) return null;
        System.Int16 x;
        pkg.Read(out x);
        System.Int16 z;
        pkg.Read(out z);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.Role.RoleActor).GetMethod("RPC_UpdatePosition");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.Role.RoleActor:RPC_UpdatePosition");
            if (isValid==false)
                return null;
        }
        host.RPC_UpdatePosition(x,z);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(2325886331,"RPC_UpdateCriticalValue",typeof(HExe_2325886331))]
public class HExe_2325886331: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.Role.RoleActor host = obj as Client.Role.RoleActor;
        if (host == null) return null;
        System.UInt32 attackerid;
        pkg.Read(out attackerid);
        System.UInt32 targetId;
        pkg.Read(out targetId);
        System.UInt16 skillid;
        pkg.Read(out skillid);
        System.Int32 value;
        pkg.Read(out value);
        System.Byte hitType;
        pkg.Read(out hitType);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.Role.RoleActor).GetMethod("RPC_UpdateCriticalValue");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.Role.RoleActor:RPC_UpdateCriticalValue");
            if (isValid==false)
                return null;
        }
        host.RPC_UpdateCriticalValue(attackerid,targetId,skillid,value,hitType);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3100448924,"RPC_AddBuff",typeof(HExe_3100448924))]
public class HExe_3100448924: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.Role.RoleActor host = obj as Client.Role.RoleActor;
        if (host == null) return null;
        GameData.Skill.BuffData data = new GameData.Skill.BuffData();
        pkg.Read( data);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.Role.RoleActor).GetMethod("RPC_AddBuff");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.Role.RoleActor:RPC_AddBuff");
            if (isValid==false)
                return null;
        }
        host.RPC_AddBuff(data);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(4071610659,"RPC_RemoveBuff",typeof(HExe_4071610659))]
public class HExe_4071610659: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.Role.RoleActor host = obj as Client.Role.RoleActor;
        if (host == null) return null;
        GameData.Skill.BuffData data = new GameData.Skill.BuffData();
        pkg.Read( data);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.Role.RoleActor).GetMethod("RPC_RemoveBuff");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.Role.RoleActor:RPC_RemoveBuff");
            if (isValid==false)
                return null;
        }
        host.RPC_RemoveBuff(data);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3447275784,"RPC_FreshBuff",typeof(HExe_3447275784))]
public class HExe_3447275784: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.Role.RoleActor host = obj as Client.Role.RoleActor;
        if (host == null) return null;
        System.Guid id;
        pkg.Read(out id);
        System.Int64 time;
        pkg.Read(out time);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.Role.RoleActor).GetMethod("RPC_FreshBuff");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.Role.RoleActor:RPC_FreshBuff");
            if (isValid==false)
                return null;
        }
        host.RPC_FreshBuff(id,time);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(473901565,"RPC_UpdateRoleValue",typeof(HExe_473901565))]
public class HExe_473901565: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.Role.RoleActor host = obj as Client.Role.RoleActor;
        if (host == null) return null;
        System.String name;
        pkg.Read(out name);
        RPC.DataReader value;
        pkg.Read(out value);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.Role.RoleActor).GetMethod("RPC_UpdateRoleValue");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.Role.RoleActor:RPC_UpdateRoleValue");
            if (isValid==false)
                return null;
        }
        host.RPC_UpdateRoleValue(name,value);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3746655369,"RPC_LevelUpEffect",typeof(HExe_3746655369))]
public class HExe_3746655369: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.Role.RoleActor host = obj as Client.Role.RoleActor;
        if (host == null) return null;
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.Role.RoleActor).GetMethod("RPC_LevelUpEffect");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.Role.RoleActor:RPC_LevelUpEffect");
            if (isValid==false)
                return null;
        }
        host.RPC_LevelUpEffect();
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3492219995,"RPC_RoleCollision",typeof(HExe_3492219995))]
public class HExe_3492219995: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        Client.Role.RoleActor host = obj as Client.Role.RoleActor;
        if (host == null) return null;
        System.UInt32 targetSingleId;
        pkg.Read(out targetSingleId);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(Client.Role.RoleActor).GetMethod("RPC_RoleCollision");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "Client.Role.RoleActor:RPC_RoleCollision");
            if (isValid==false)
                return null;
        }
        host.RPC_RoleCollision(targetSingleId);
return null;
    }
}
[RPC.RPCMethordExecuterTypeAttribute(3346036833,"RPC_SetSceneActorProperty",typeof(HExe_3346036833))]
public class HExe_3346036833: RPC.RPCMethodExecuter
{
    static RPC.RPCMethodAttribute RPCMethodAttr = null;
    static System.Reflection.MethodInfo Method = null;
    public override int LimitLevel
    {
        get { return 0; }
    }
    public override object Execute(CSUtility.Net.NetConnection connect,RPC.RPCObject obj,RPC.PackageProxy pkg,RPC.RPCForwardInfo fwd)
    {
        CCore.CommonRPC host = obj as CCore.CommonRPC;
        if (host == null) return null;
        System.Guid actorId;
        pkg.Read(out actorId);
        System.String propertyName;
        pkg.Read(out propertyName);
        System.String value;
        pkg.Read(out value);
        TotalReadSize += pkg.CurPtr();
        TotalCallCount++;
        if (RPCMethodAttr == null)
        {
            Method = typeof(CCore.CommonRPC).GetMethod("RPC_SetSceneActorProperty");
            var attrs = Method.GetCustomAttributes(typeof(RPC.RPCMethodAttribute), true);
            RPCMethodAttr = attrs[0] as RPC.RPCMethodAttribute;
        }
        if (RPCMethodAttr != null && RPCMethodAttr.MinCallInterval > 0)
        {
            var isValid = connect.IsValidCallTime(Method, RPCMethodAttr, pkg.RecvTime, "CCore.CommonRPC:RPC_SetSceneActorProperty");
            if (isValid==false)
                return null;
        }
        host.RPC_SetSceneActorProperty(actorId,propertyName,value,fwd);
return null;
    }
}
public class MappingHashCode2Index{
public static void BuildMapping(){
    RPC.RPCNetworkMgr.AddExecuterIndxer(649012910 , 0);
    RPC.RPCNetworkMgr.AddExecuterIndxer(483678 , 1);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3247434719 , 2);
    RPC.RPCNetworkMgr.AddExecuterIndxer(2483224510 , 0);
    RPC.RPCNetworkMgr.AddExecuterIndxer(1014639420 , 1);
    RPC.RPCNetworkMgr.AddExecuterIndxer(2325886331 , 7);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3100448924 , 4);
    RPC.RPCNetworkMgr.AddExecuterIndxer(4071610659 , 5);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3447275784 , 6);
    RPC.RPCNetworkMgr.AddExecuterIndxer(473901565 , 3);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3746655369 , 8);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3492219995 , 2);
    RPC.RPCNetworkMgr.AddExecuterIndxer(3346036833 , 0);
}
}

}
