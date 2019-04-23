using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Hall
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class LogicProcessor
    {
        //这个逻辑处理器负责的地图列表
        public CSUtility.Support.AsyncObjManager<Map.MapInstance, Map.MapInstance> Maps { get; } = new CSUtility.Support.AsyncObjManager<Map.MapInstance, Map.MapInstance>();
        
        public bool InProcessor(Map.MapInstance map)
        {
            if (Maps.FindObj(map) == null)
                return false;
            return true;
        }

        public bool PushMap(Map.MapInstance map)
        {
            if (map.LogicProcessor == this)
                return true;

            if (map.LogicProcessor != null)
            {
                return false;
            }

            map.LogicProcessor = this;
            Maps.Add(map, map);
            return true;
        }

        public Int64 TickTime = CSUtility.DllImportAPI.vfxGetTickCount();
        public Int64 ElapsedMiliSeccond;
        public void Tick()
        {
            var nowTime = CSUtility.DllImportAPI.vfxGetTickCount();
            ElapsedMiliSeccond = nowTime - TickTime;
            TickTime = nowTime;
        
            Maps.BeforeTick();

            bool bRemoveMap = false;
            CSUtility.Support.AsyncObjManager<Map.MapInstance, Map.MapInstance>.FOnVisitObject visitor = delegate(Map.MapInstance key, Map.MapInstance map, object arg)
            {
                if (map != null)
                {
                    try
                    {
                        map.Tick(ElapsedMiliSeccond);
                    }
                    catch (System.Exception ex)
                    {
                        Log.FileLog.WriteLine(ex.ToString());
                        Log.FileLog.WriteLine(ex.StackTrace.ToString());
                    }
                    
                    if (map.WaitDestory)
                    {
                        Maps.Remove(key);
                        bRemoveMap = true;
                    }
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            Maps.For_Each(visitor, null);

            Maps.AfterTick();
            if (bRemoveMap)
                System.GC.Collect();
        }
        bool bOutputInfo = false;
        bool mRunning;
        System.Threading.Thread mThread;

        int mTickCost = 0;
        public int TickCost
        {
            get { return mTickCost; }
        }
        public void ThreadLoop()
        {
            while (mRunning)
            {
                try
                {
                    var v1 = CSUtility.Helper.LogicTimer.GetHPTickCount();
                    Tick();
                    var v2 = CSUtility.Helper.LogicTimer.GetHPTickCount();
                    mTickCost = (int)(v2 - v1);
                    if (mTickCost < 40000)
                    {
                        CSUtility.Helper.LogicTimer.TimeBeginPeriod(1);
                        System.Threading.Thread.Sleep((40000 - mTickCost) / 1000);
                        CSUtility.Helper.LogicTimer.TimeEndPeriod(1);
                    }
                    else
                    {
                        if (bOutputInfo)
                            System.Diagnostics.Trace.WriteLine("Logic Processor TickTime " + mTickCost);
                    }
                }
                catch (Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }
            }
            mThread = null;
            Log.FileLog.WriteLine(System.String.Format("LogicProcessor{0} Thread exit!", mThreadIndex));
        }
        int mThreadIndex;
        public void StartThread(int threadIndex)
        {
            mThreadIndex = threadIndex;
            mRunning = true;
            mThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.ThreadLoop));
            mThread.Start();
        }
        public void StopThread()
        {
            mRunning = false;
        }
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class LogicProcessorManager
    {
        static LogicProcessorManager smInstance = new LogicProcessorManager();
        public static LogicProcessorManager Instance
        {
            get { return smInstance; }
        }
        LogicProcessor[] mProcessors;
        public LogicProcessor[] Processors
        {
            get { return mProcessors; }
        }
        public void StartProcessors(int count)
        {
            var CpuCoreNumber = Environment.ProcessorCount;
            //var CpuCoreNumber = (int)CSUtility.Support.CPUInfo.GetProcessorCoreCount();

            Log.FileLog.WriteLine("CPU内核数量：{0}", CpuCoreNumber);
            if (count == 0)
                count = CpuCoreNumber;

            Log.FileLog.WriteLine("地图逻辑处理器线程数量：{0}", count);

            mProcessors = new LogicProcessor[count];
            for (int i = 0; i < mProcessors.Length; i++)
            {
                mProcessors[i] = new LogicProcessor();
                mProcessors[i].StartThread(i);
            }
        }
        public void StopProcessors()
        {
            for (int i = 0; i < mProcessors.Length; i++)
            {
                mProcessors[i].StopThread();
            }
        }
        public void PushMap(Map.MapInstance map)
        {
            lock (this)
            {
                LogicProcessor processor = null;
                Int32 minMap = Int32.MaxValue;
                for (int i = 0; i < mProcessors.Length; i++)
                {
                    if (mProcessors[i].InProcessor(map))
                        return;
                    int count = mProcessors[i].Maps.Count;
                    if (minMap > count)
                    {
                        minMap = count;
                        processor = mProcessors[i];
                    }
                }
                if (processor == null)
                    return;
                processor.PushMap(map);
            }
        }
    }
}
