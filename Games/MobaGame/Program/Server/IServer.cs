using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum EServerType
    {
        Register,
        Gate,
        Hall,
        Data,
        Path,
        Com,
    }

    public class IServer
    {
        static IServer smInstance = new IServer();
        public static IServer Instance
        {
            get { return smInstance; }
        }
        public IServer()
        {
            mExePath = AppDomain.CurrentDomain.BaseDirectory;

            var serverWindowsAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, "ServerCommon.dll");
            CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, "cscommon", serverWindowsAssembly);
            
            var serverAssembly = CSUtility.Program.GetAssemblyFromDllFileName(CSUtility.Helper.enCSType.Server, "Server.dll");
            CSUtility.Program.RegisterAnalyseAssembly(CSUtility.Helper.enCSType.Server, CSUtility.enPlatform.Windows, "game", serverAssembly);            
        }

        string mExePath;
        public string ExePath
        {
            get { return mExePath; }
        }
        public void LoadRPCModule(string dllname)
        {
            System.Reflection.Assembly assembly;
            assembly = System.Reflection.Assembly.LoadFrom(mExePath + "/Server.dll");
            RPC.RPCEntrance.BuildRPCMethordExecuter(assembly);
        }

        Int64 mLogicNowTick = 0;
        System.DateTime mLogicDateTime;
        Int64 mElapseMilliSecondTime;

        public void Tick()
        {
            Int64 nowTick = CSUtility.Helper.LogicTimer.GetTickCount();
            if (mLogicNowTick < 0)
            {
                mLogicNowTick = nowTick;
            }
            mElapseMilliSecondTime = nowTick - mLogicNowTick;

            mLogicNowTick = nowTick;
            mLogicDateTime = System.DateTime.Now;
        }

        public System.DateTime GetNowDateTime()
        {
            return mLogicDateTime;
        }
        public Int64 GetElapseMilliSecondTime()
        {
            return mElapseMilliSecondTime;
        }
        static System.DateTime mBeginTime = new System.DateTime(1970, 1, 1, 8, 0, 0);
        public static Double TotalSecond1970Time
        {
            get { return (System.DateTime.Now - mBeginTime).TotalSeconds; }
        }

        public static DateTime GetDateTimeFromTotalSecond(Double seconds)
        {
            DateTime date = new DateTime(1970, 1, 1, 8, 0, 0);

            return date.AddSeconds(seconds);
        }
    }
}
