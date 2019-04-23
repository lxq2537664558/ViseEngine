using System;
using System.Collections.Generic;

namespace RPC
{
    public class RPCMethodInfo
    {
        public string ClassName;
        public UInt32 HashCode;
        public byte Index;
        public string FullName;
    }
    class RPCClassMethods
    {
        public string ClassName;

        public Dictionary<UInt32, RPCMethodInfo> mMethods = new Dictionary<UInt32, RPCMethodInfo>();

        bool HasIndex(byte index)
        {
            foreach (var i in mMethods)
            {
                if (i.Value.Index == index)
                    return true;
            }
            return false;
        }
        
        public RPCMethodInfo GetMethodIndex(UInt32 hash, out bool bAdd)
        {
            RPCMethodInfo index;
            if (mMethods.TryGetValue(hash, out index))
            {
                bAdd = false;
                return index;
            }

            for (byte i = 0; i < byte.MaxValue; i++)
            {
                if (HasIndex(i))
                    continue;
                else
                {
                    index = new RPCMethodInfo();
                    index.ClassName = ClassName;
                    index.HashCode = hash;
                    index.Index = i;
                    mMethods.Add(hash, index);
                    bAdd = true;
                    return index;
                }
            }

            var expInfo = string.Format("RPC类:{0}的RPC函数超过255个，请改变写法");
            //System.Windows.Forms.MessageBox.Show(expInfo);
            throw new System.Exception(expInfo);
        }

        public void RegMethodIndex(UInt32 hash, byte index, string name)
        {
            RPCMethodInfo oidx;
            if (mMethods.TryGetValue(hash, out oidx))
            {
                System.Diagnostics.Debugger.Break();
            }
            oidx = new RPCMethodInfo();
            oidx.ClassName = ClassName;
            oidx.HashCode = hash;
            oidx.Index = index;
            oidx.FullName = name;
            mMethods[hash] = oidx;
        }
    }
    public class RPCVersionManager
    {
        Dictionary<string, RPCClassMethods> mRpcClass = new Dictionary<string, RPCClassMethods>();

        public int RPCHashCode;

        public RPCMethodInfo GetMethodIndex(string cname, UInt32 methodHash, out bool bAdd)
        {
            RPCClassMethods rclass;
            if (mRpcClass.TryGetValue(cname, out rclass) == false)
            {
                rclass = new RPCClassMethods();
                rclass.ClassName = cname;
                mRpcClass.Add(cname, rclass);
            }

            return rclass.GetMethodIndex(methodHash, out bAdd);
        }

        public void RegMethodIndex(string cname, UInt32 methodHash, byte index, string fname)
        {
            RPCClassMethods rclass;
            if (mRpcClass.TryGetValue(cname, out rclass) == false)
            {
                rclass = new RPCClassMethods();
                rclass.ClassName = cname;
                mRpcClass.Add(cname, rclass);
            }

            rclass.RegMethodIndex(methodHash, index, fname);
        }

        public Dictionary<RPCMethodInfo,RPCMethodInfo> GetMethods()
        {
            var result = new Dictionary<RPCMethodInfo, RPCMethodInfo>();

            foreach (var i in mRpcClass)
            {
                foreach (var j in i.Value.mMethods)
                {
                    result.Add(j.Value, j.Value);
                }
            }

            return result;
        }
    }
}
