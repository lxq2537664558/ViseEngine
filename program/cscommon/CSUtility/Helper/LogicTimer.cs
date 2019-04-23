using System;
using System.Collections.Generic;

namespace CSUtility.Helper
{
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Timer")]
    public delegate bool FOnTimer(string name);
    public class LogicTimer
    {
        public static long GetTickCount()
        {
            return DllImportAPI.vfxGetTickCount();
        }
        public static long GetHPTickCount()
        {
            return DllImportAPI.HighPrecision_GetTickCount();
        }
        public static UInt32 TimeBeginPeriod(UInt32 uPeriod)
        {
            return DllImportAPI.timeBeginPeriod(uPeriod);
        }
        public static UInt32 TimeEndPeriod(UInt32 uPeriod)
        {
            return DllImportAPI.timeEndPeriod(uPeriod);
        }

        public bool mIsRemoved = false;
        string mName = "";
        public string Name
        {
            get { return mName; }
        }
        Int64 mCurrTime;
        Int64 mInterval;
        public Int64 Interval
        {
            get { return mInterval; }
            set { mInterval = value; }
        }
        //FOnTimer OnTimer;
        FOnTimer CallBack = null;

        public void ResetCurTime()
        {
            mCurrTime = 0;
        }

        public void ResetTimer(string name, Int64 interval, CSUtility.Helper.EventCallBack cb)
        {
            mName = name;
            mInterval = interval;
            if (cb != null)
            {
                CallBack = cb.GetCallee() as FOnTimer;
            }
            
            mIsRemoved = false;
        }

        public void Tick(Int64 elapseMillisecond)
        {
            if (CallBack == null)
                return;

            mCurrTime += elapseMillisecond;
            if (mCurrTime > mInterval)
            {
                if (CallBack == null)
                    return;

                if (CallBack(mName) == true)
                {
                    mCurrTime = 0;
                }
                else
                {
                    this.mIsRemoved = true;
                }
            }
        }
    }

    public class LogicTimerManager
    {
        static LogicTimerManager smInstance = new LogicTimerManager();
        public static LogicTimerManager Instance
        {
            get { return smInstance; }
        }

        List<LogicTimer> mTimers = new List<LogicTimer>();
        List<LogicTimer> mAddingTimers = new List<LogicTimer>();

        public void ResetCurLogicTimer(string name)
        {
            lock (this)
            {
                foreach (var i in mAddingTimers)
                {
                    if (i.Name == name)
                    {
                        i.ResetCurTime();
                        return;
                    }
                }

                foreach (var i in mTimers)
                {
                    if (i.Name == name)
                    {
                        i.ResetCurTime();
                        return;
                    }
                }
            }
        }

        public void AddLogicTimer(string name, Int64 interval, FOnTimer cb, CSUtility.Helper.enCSType csType)
        {
            var ecb = new CSUtility.Helper.EventCallBack(csType)
            {
                CBType = typeof(CSUtility.Helper.FOnTimer),
                Id = Guid.NewGuid(),
                Callee = (CSUtility.Helper.FOnTimer)(cb)
            };
            AddLogicTimer(name, interval, ecb);
        }
        public void AddLogicTimer(string name, Int64 interval, CSUtility.Helper.EventCallBack cb)
        {
            lock (this)
            {
                foreach (var i in mAddingTimers)
                {
                    if (i.Name == name)
                    {
                        i.ResetTimer(name, interval, cb);
                        return;
                    }
                }

                foreach (var i in mTimers)
                {
                    if (i.Name == name)
                    {
                        i.ResetTimer(name, interval, cb);
                        return;
                    }
                }

                var timer = new LogicTimer();
                timer.ResetTimer(name, interval, cb);
                mAddingTimers.Add(timer);
            }
        }
        public void RemoveLogicTimer(string name)
        {
            lock (this)
            {
                foreach (var i in mAddingTimers)
                {
                    if (i.Name == name)
                    {
                        mAddingTimers.Remove(i);
                        return;
                    }
                }

                foreach (var i in mTimers)
                {
                    if (i.Name == name)
                    {
                        i.mIsRemoved = true;
                        return;
                    }
                }
            }
        }

        public void Tick(Int64 elapseMillisecond)
        {
            try
            {
                lock (this)
                {
                    if (mAddingTimers.Count > 0)
                    {
                        mTimers.AddRange(mAddingTimers);
                        mAddingTimers.Clear();
                    }
                }

                var rmvList = new List<LogicTimer>();
                foreach (var i in mTimers)
                {
                    if (i.mIsRemoved)
                    {
                        rmvList.Add(i);
                        continue;
                    }
                    i.Tick(elapseMillisecond);
                }

                foreach (var i in rmvList)
                {
                    mTimers.Remove(i);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.StackTrace.ToString());
            }
        }
    }
}
