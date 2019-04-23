using System;

namespace CSUtility.Support
{
    public class IHelper
    {
        readonly static object _sync = new object();

        static UInt16 index;//16bit，一个服务器，一秒钟最多产生65535个 ObjId
        static UInt16 serverId = 1;//16bit 4096个服务器
        
        static System.DateTime StartTime = new System.DateTime(2015, 1, 1);

        //id是64位的（serverID：16位；时间戳：32位；序列号：16位）
        public static UInt64 GenerateObjID(UInt16 itemType)
        {
            lock (_sync)
            {
                UInt32 ts = (UInt32)(System.DateTime.Now - StartTime).TotalSeconds;
                //UInt64 objId = (UInt64)(((UInt64)(itemType) << 60) | ((UInt64)(serverId) << 48) | ((UInt64)(ts) << 16) | ((UInt64)(++index)));
                UInt64 objId = (UInt64)(((UInt64)(serverId) << 48) | ((UInt64)(ts) << 16) | ((UInt64)(++index)));
                return objId;
            }
        }

        public static UInt32 Obj_ID_SEQ(UInt64 id) { return (UInt32)((id) & 0xFFFF); }
        public static UInt32 Obj_ID_TIMESTAMP(UInt64 id) { return (UInt32)(((id) >> 16) & 0xFFFFFFFF); }
        public static UInt32 Obj_ID_SERVER(UInt64 id) { return (UInt32)(((id) >> 48) & 0xFFFF); }
        //public static UInt32 Obj_ID_TYPE(UInt64 id) { return (UInt32)(((id) >> 60) & 0xF); }

        static public Guid GuidParse(string str)
        {
            if(!str.Contains("-"))
                return Guid.Empty;

            return new Guid(str);
        }
        static public Guid GuidTryParse(string str)
        {
            try
            {
                if (!str.Contains("-"))
                    return Guid.Empty;

                return new Guid(str);
            }
            catch (System.Exception)
            {
                Log.FileLog.WriteLine(string.Format("Parse Guid Failed:{0}", str));
                return Guid.Empty;
            }
        }

        static public T EnumTryParse<T>(string str)
        {
            try
            {
                return (T)(System.Enum.Parse(typeof(T), str));
            }
            catch (System.Exception)
            {
                return default(T);
            }
        }

        static public object EnumTryParse(System.Type type, string str)
        {
            try
            {
                return System.Enum.Parse(type, str);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        static public bool StringContain(string str, System.Char chr)
        {
            return (str.IndexOf(chr) >= 0);
        }
        static public bool StringContain(string str, string chr)
        {
            return (str.IndexOf(chr) >= 0);
        }
    }
}
