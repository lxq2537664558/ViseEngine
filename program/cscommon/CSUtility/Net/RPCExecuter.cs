using System;
using System.Collections.Generic;

namespace RPC
{
    public enum ERPCHeader
    {
		MaxStack		= 4,
		MaxStackSize	= (MaxStack%2)==0 ? (MaxStack/2) : (MaxStack/2+1),
		StackUnused		= 0x0F,//没用到的
		StackIndexBegin	= 11,//大于11，小于16的属于索引器
		SinglePkg		= (1<<7),
		PackageSendMask	= 0x0F,
	};

    public class RPCMethodExecuter
	{
        public int TotalCallCount = 0;
        public int TotalReadSize = 0;
	    public System.String mMethodFullName;
        public System.String mMethodName;
		public virtual int LimitLevel
		{
			get{return 0;}
		}
		public virtual System.Object Execute(CSUtility.Net.NetConnection connect,RPCObject obj,PackageProxy pkg,RPCForwardInfo fwd)
        {
            return null;
        }
	}

	public class RPCIndexerExecuter
	{
	    public System.String mMethodName;
		public virtual int LimitLevel
		{
			get{return 0;}
		}
		public virtual RPCObject Execute(RPCObject obj,PackageProxy pkg)
        {
            return null;
        }
	}

	[System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
	public interface RPCObject
	{
        RPCClassInfo GetRPCClassInfo();
		//virtual RPCObject RPCGetChildObject(int limitLevel,int Index);
		//virtual RPCObject RPCGetIndexObject(int limitLevel,int Index,PackageProxy pkg);
		//virtual System.Object RPCExecuteMethod(int limitLevel,Iocp.NetConnection connect,int Index,PackageProxy pkg,RPCForwardInfo fwd);
	};

	public delegate System.Object FEntranceExecute(int limitLevel, CSUtility.Net.NetConnection connect,RPCObject rootObj , PackageProxy pkg,RPCForwardInfo fwd);

	public class RPCEntrance
	{
		public static FEntranceExecute EntranceExecute;
		public static System.Object Execute(int limitLevel, CSUtility.Net.NetConnection connect,RPCObject rootObj , PackageProxy pkg,RPCForwardInfo fwd)
	    {
		    if(rootObj==null)
			    return null;
		    try
		    {
			    if(EntranceExecute!=null)
			    {
				    return EntranceExecute(limitLevel,connect,rootObj , pkg,fwd);
			    }
			    else
			    {
				    RPCObject curExe = rootObj;
				    for( Int16 i=0 ; i<pkg.GetMaxStack() ; i++ )
				    {
					    if(curExe==null)
						    return null;
					    System.Byte stk = pkg.GetStack(i);
                        if (stk == (Byte)ERPCHeader.StackUnused)
						    break;
                        else if (stk < (Byte)ERPCHeader.StackIndexBegin)
					    {
						    //curExe = curExe.RPCGetChildObject(limitLevel,stk);
						    curExe = curExe.GetRPCClassInfo().RPCGetChildObject(limitLevel,stk,curExe);
						    //curExe = (RPCObject)curExe.GetRPCClassInfo().mChildObjects[stk].GetValue(curExe,null);
					    }
					    else
					    {
						    //curExe = curExe.RPCGetIndexObject(limitLevel,stk-RPCHeader.StackIndexBegin,pkg);
                            curExe = curExe.GetRPCClassInfo().RPCGetIndexerExecuter(limitLevel, stk - (Byte)ERPCHeader.StackIndexBegin, curExe, pkg);
					    }
				    }

				    if(curExe==null)
					    return null;
				    //return curExe.GetRPCClassInfo().RPCExecuteMethod(limitLevel,connect,pkg.GetMethod(),pkg,fwd);
				    return curExe.GetRPCClassInfo().RPCGetMethordExecuter(limitLevel,connect,pkg.GetMethod(),curExe,pkg,fwd);
			    }
		    }
		    catch (System.Exception e)
		    {   
			    System.Diagnostics.Debug.WriteLine( e.Message + "==>" );
			    System.Diagnostics.Debug.WriteLine( e.StackTrace.ToString());
			    Log.FileLog.WriteLine( e.Message + "==>" );
			    Log.FileLog.WriteLine( e.StackTrace.ToString() );
			
			    //如果发生RPC函数内部异常，尽量返回一个-128的值，免得服务器之间的调用因为没有返回WaitHandle没有机会清空
			    PackageWriter retPkg = new PackageWriter();
			    retPkg.Write((System.SByte)(-128));
			    retPkg.DoReturnCommand2(connect, fwd.ReturnSerialId);

			    return null;
		    }
	    }

        public static System.UInt32 GetMethodHashCode(System.Reflection.MethodInfo mi, System.String cname)
	    {
		    System.String hashString = mi.ReturnType.FullName + " " + cname + "." + mi.Name + "(";
		    System.Reflection.ParameterInfo[] parameters = mi.GetParameters();
		    for(int i=0;i<parameters.Length;i++)
		    {
			    hashString += parameters[i].ParameterType.FullName + " " + parameters[i].Name + ",";
		    }
		    hashString += ")";
            return CSUtility.Support.UniHash.DefaultHash(hashString);
		    //return (System.UInt32)hashString.GetHashCode();
	    }

        public static System.UInt32 GetIndexerHashCode(System.Reflection.PropertyInfo pi, System.String cname)
	    {
		    System.String hashString = cname + "$" + pi.PropertyType.FullName + " " + pi.Name + "(";
		    System.Reflection.ParameterInfo[] parameters =  pi.GetIndexParameters();
		    for(int i=0;i<parameters.Length;i++)
		    {
			    hashString += parameters[i].ParameterType.FullName + " " + parameters[i].Name + ",";
		    }
		    hashString += ")";
            return CSUtility.Support.UniHash.DefaultHash(hashString);
		    //return (System.UInt32)hashString.GetHashCode();
	    }

        public static List<RPCClassInfo> RpcAllClassInfo = new List<RPCClassInfo>();
        public static Dictionary<UInt32, RPCCallerCounter> RpcAllCaller = new Dictionary<UInt32, RPCCallerCounter>();

        public static RPCCallerCounter GetCallerCounter(UInt32 hashCode, string name)
        {
            RPCCallerCounter result;
            if (RpcAllCaller.TryGetValue(hashCode, out result))
            {
                return result;
            }
            result = new RPCCallerCounter();
            result.Name = name;
            RpcAllCaller.Add(hashCode, result);
            return result;
        }

        public static void BuildRPCClassInfo(System.Type type, RPCClassInfo cInfos)
	    {
            cInfos.ObjType = type;
		    System.Reflection.PropertyInfo[] props = type.GetProperties();
		    foreach(System.Reflection.PropertyInfo p in props)
		    {
			    if(p.Name=="Item")
			    {
				    System.Object[] attrs = p.GetCustomAttributes(typeof(RPCIndexObjectAttribute),true);
				    if(attrs==null||attrs.Length==0)
					    continue;
				    RPCIndexObjectAttribute att = (RPCIndexObjectAttribute)(attrs[0]);
				    if(att==null)
					    continue;
				    System.Type exeType = RPCNetworkMgr.FindIndexExecuterType( GetIndexerHashCode(p,type.FullName) );
				    if(exeType==null)
					    continue;
				    RPCIndexerExecuter executer = (RPCIndexerExecuter)System.Activator.CreateInstance( exeType );
				    executer.mMethodName = type.FullName + "." + p.ToString();
				    cInfos.mIndexers[att.ChildIndex] = executer;				

				    continue;
			    }
			    else
			    {
				    System.Object[] attrs = p.GetCustomAttributes(typeof(RPCChildObjectAttribute),true);
				    if(attrs!=null && attrs.Length==1)
				    {
					    RPCChildObjectAttribute att = (RPCChildObjectAttribute)(attrs[0]);
					    if(att==null)
						    continue;
					    cInfos.mChildObjects[att.ChildIndex] = p;
				    }
			    }
		    }
		    System.Reflection.MethodInfo[] methords = type.GetMethods();
		    foreach(System.Reflection.MethodInfo m in methords)
		    {
			    System.Object[] attrs = m.GetCustomAttributes(typeof(RPCMethodAttribute),true);
			    if(attrs!=null && attrs.Length==1)
			    {
				    RPCMethodAttribute att = (RPCMethodAttribute)(attrs[0]);
				    if(att==null)
					    continue;
				    System.UInt32 hashCode = GetMethodHashCode(m,type.FullName);
				    System.Type exeType = RPCNetworkMgr.FindExecuterType( hashCode );
				    if(exeType==null)
					    continue;
				    System.Byte methodIndexer = RPCNetworkMgr.FindExecuterIndexer(hashCode);
				    RPCMethodExecuter executer = (RPCMethodExecuter)System.Activator.CreateInstance( exeType );
                    executer.mMethodFullName = type.FullName + "." + m.ToString();
                    executer.mMethodName = m.Name;
				    cInfos.mMethods[methodIndexer] = executer;
			    }
		    }

            RpcAllClassInfo.Add(cInfos);
	    }

        public static void BuildRPCMethordExecuter(System.Reflection.Assembly assembly)
	    {
		    System.Type[] ctypes = assembly.GetTypes();
		    foreach(System.Type t in  ctypes)
		    {
			    if( t.FullName=="RPC_ExecuterNamespace.MappingHashCode2Index" )
			    {
				    System.Reflection.MethodInfo buildMI = t.GetMethod("BuildMapping");
				    if(buildMI!=null)
				    {
					    buildMI.Invoke(System.Activator.CreateInstance(t),new System.Object[0]);
				    }
			    }
			    {
				    System.Object[] attrs = t.GetCustomAttributes(typeof(RPCMethordExecuterTypeAttribute),true);
				    if(attrs!=null && attrs.Length==1)
				    {
					    RPCMethordExecuterTypeAttribute att = (RPCMethordExecuterTypeAttribute)(attrs[0]);
					    if(att==null)
						    continue;
					    RPCNetworkMgr.AddExecuterType(att.KeyCode,att.ExecuterType);
				    }
			    }			

			    {
				    System.Object[] attrs = t.GetCustomAttributes(typeof(RPCIndexExecuterTypeAttribute),true);
				    if(attrs!=null && attrs.Length==1)
				    {
					    RPCIndexExecuterTypeAttribute att = (RPCIndexExecuterTypeAttribute)(attrs[0]);
					    if(att==null)
						    continue;
					    RPCNetworkMgr.AddIndexExecuterType(att.KeyCode,att.ExecuterType);
				    }
			    }
		    }
            
            foreach (System.Type t in  ctypes)
		    {                
                System.Object[] attrs = t.GetCustomAttributes(typeof(RPCClassAttribute),false);
			    if(attrs!=null && attrs.Length==1)
			    {
                    RPCClassAttribute att = (RPCClassAttribute)(attrs[0]);
				    if(att==null)
					    continue;
				    System.Reflection.MethodInfo mtd_GetRPCClassInfo = t.GetMethod("GetRPCClassInfo");
				    if(mtd_GetRPCClassInfo==null)
					    continue;

                    try
                    {
                        System.Object obj = System.Activator.CreateInstance(t);
                        RPCClassInfo cInfos = (RPCClassInfo)mtd_GetRPCClassInfo.Invoke(obj, null);
                        if (cInfos == null)
                            continue;
                        BuildRPCClassInfo(t, cInfos);
                    }
                    catch (Exception e)
                    {
                        CSUtility.Program.LogInfo("RPC e " + e.ToString());
                    }                                                     
			    }
            }
        }
	}

	public class RPCClassInfo
	{
        public System.Type ObjType;
		public System.Reflection.PropertyInfo[] mChildObjects = new System.Reflection.PropertyInfo[11];//4位的堆栈，最多11个子对象属性
		public RPCMethodExecuter[]	mMethods = new RPCMethodExecuter[256];
		public RPCIndexerExecuter[]	mIndexers= new RPCIndexerExecuter[4];

		public RPCObject RPCGetChildObject(int limitLevel,int Index,RPCObject obj)
	    {
		    if(mChildObjects[Index]==null)
		    {
			    Log.FileLog.WriteLine("RPCGetChildObject Indexer Over Ranged:"+Index);
			    return null;
		    }

		    return (RPCObject)(mChildObjects[Index].GetValue(obj,null));
	    }
        public System.Object RPCGetMethordExecuter(int limitLevel, CSUtility.Net.NetConnection connect, int Index, RPCObject obj, PackageProxy pkg, RPCForwardInfo fwd)
	    {
		    if(mMethods[Index]==null)
		    {
                //Log.FileLog.WriteLine("RPCGetMethordExecuter Indexer Over Ranged:" + Index);
			    return null;
		    }
		    if( limitLevel < mMethods[Index].LimitLevel )
		    {
			    //此处应该记录有人越权调用函数
			    RPCHallServerSpecialRoot cb = RPCNetworkMgr.Instance.HallServerSpecialRoot;
			    if(cb==null)
			    {
				    Log.FileLog.WriteLine("发现有超越权限的函数调用({0})",mMethods[Index].mMethodName);
			    }
			    else
			    {
				    System.String str = cb.GetPlayerInfoString(fwd);
				    Log.FileLog.WriteLine("发现有超越权限的函数调用({0}:{1})",mMethods[Index].mMethodName,str);
			    }
			    return null;
		    }
		    return mMethods[Index].Execute(connect,obj,pkg,fwd);
	    }
        public RPCObject RPCGetIndexerExecuter(int limitLevel, int Index, RPCObject obj, PackageProxy pkg)
	    {
		    if(mIndexers[Index]==null)
		    {
			    //Log.FileLog.WriteLine("RPCGetIndexerExecuter Indexer Over Ranged:"+Index);
			    return null;
		    }
		    if( limitLevel < mIndexers[Index].LimitLevel )
		    {
			    //此处应该记录有人越权调用函数
			    Log.FileLog.WriteLine("发现有超越权限的索引器调用");
			    return null;
		    }
		    return mIndexers[Index].Execute(obj,pkg);
	    }
	}

    public class RPCCallerCounter
    {
        public string Name;
        public int CallCounter;
        public int WriteSize;
    }

#region 标志RPC的(类，子对象，索引器，函数的属性)
	public enum RPCExecuteLimitLevel
	{
		All			= 0,
		Player		= 100,
		Lord		= 200,
		GM			= 300,
		Developer	= 400,
		God			= 500,
		TheOne		= 600,
	}
	public class RPCClassAttribute : System.Attribute
	{
	    public System.Type RPCType { get; set; }
	
		public RPCClassAttribute(System.Type t) 
		{
            RPCType = t;
		}
	}

	public class RPCChildObjectAttribute : System.Attribute
	{
	    public int ChildIndex { get; set; }
		public int LimitLevel { get; set; }
		public bool NoClientCall { get; set; }
	
		public RPCChildObjectAttribute(int Index,int Level,bool noClient)
		{
            ChildIndex = Index;
			LimitLevel = Level;
			NoClientCall = noClient;
		}
	}

	public class RPCIndexObjectAttribute : System.Attribute
	{
		public int ChildIndex { get; set; }
        public System.Type IndexType { get; set; }
		public int LimitLevel { get; set; }
		public bool NoClientCall { get; set; }
	
		public RPCIndexObjectAttribute(int Index,System.Type type,int Level,bool noClient)
		{
            ChildIndex = Index;
			IndexType = type;
			LimitLevel = Level;
			NoClientCall = noClient;
		}
	}

	public class RPCMethodAttribute : System.Attribute
	{
		public int LimitLevel { get; set; }
		public bool NoClientCall { get; set; }
        public bool IsWeakPkg { get; set; }
        public Int64 MinCallInterval { get; set; } = -1;
	
		public RPCMethodAttribute(int Level,bool noClient,bool isWeak=false, Int64 callInterval = -1)
		{
            LimitLevel = Level;
			NoClientCall = noClient;
            IsWeakPkg = isWeak;
            MinCallInterval = callInterval;
		}
	}
#endregion	

#region 只有编辑器生成的代码才会用到的Attribute
	public class RPCMethordExecuterTypeAttribute : System.Attribute
	{
		public System.UInt32	KeyCode;
		public System.String	FullName;
		public System.Type	ExecuterType;
	
		public RPCMethordExecuterTypeAttribute(System.UInt32 hashCode,System.String name,System.Type exeType)
		{
            KeyCode = hashCode;//System.Reflection.MethodInfo.GetHashCode()
			FullName = name;//System.Reflection.MethodInfo.ToString()
			ExecuterType = exeType;//typeof(KeyCode)
		}
	}

	public class RPCIndexExecuterTypeAttribute : System.Attribute
	{
		public System.UInt32	KeyCode;
		public System.String	FullName;
		public System.Type	ExecuterType;
	
		public RPCIndexExecuterTypeAttribute(System.UInt32 hashCode,System.String name,System.Type exeType)
		{
            KeyCode = hashCode;//System.Reflection.MethodInfo.GetHashCode()
			FullName = name;//System.Reflection.MethodInfo.ToString()
            ExecuterType = exeType;//typeof(KeyCode)
		}
	}
#endregion
}
