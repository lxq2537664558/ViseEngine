using System;
using System.Collections.Generic;

namespace SCore
{
    public class AsyncExecuter
    {
        public delegate void FOnExec();
        public FOnExec Exec;
    }

    public class AsyncExecuteThread
    {
        public AsyncExecuter AsyncExe(bool pushQueue)
        {
            AsyncExecuter holder = new AsyncExecuter();
            if (pushQueue)
            {
                lock (this)
                {
                    mLoginQueue.Add(holder);
                }
            }
            return holder;
        }

        List<AsyncExecuter> mLoginQueue = new List<AsyncExecuter>();
        public int GetNumber()
        {
            return mLoginQueue.Count;
        }
        void Tick()
        {
            if (mLoginQueue.Count > 0)
            {
                AsyncExecuter atom = null;
                lock (this)
                {
                    atom = mLoginQueue[0];
                    if (atom.Exec != null)
                        mLoginQueue.RemoveAt(0);
                    else
                        return;
                }
                try
                {
                    atom.Exec();
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }
            }
        }
        bool mRunning;
        private System.Threading.Thread mThread;
        public void ThreadLoop()
        {
            while (mRunning)
            {
                Tick();
                System.Threading.Thread.Sleep(5);
            }
            mThread = null;

            Log.FileLog.WriteLine("AsyncExecuteThread Thread exit!");
        }
        public void StartThread()
        {
            mRunning = true;
            mThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.ThreadLoop));
            mThread.Start();
        }
        public void StopThread()
        {
            mRunning = false;
        }
    }
    public class AsyncExecuteThreadManager
    {
        static AsyncExecuteThreadManager smInstance = new AsyncExecuteThreadManager();
        public static AsyncExecuteThreadManager Instance
        {
            get { return smInstance; }
        }

        List<AsyncExecuteThread> mExes = new List<AsyncExecuteThread>();

        public AsyncExecuteThread GetAsyncExecuteThread(int i)
        {
            return mExes[i];
        }

        public AsyncExecuter AsyncExe(bool pushQueue)
        {
            lock (this)
            {
                int MinNumber = Int32.MaxValue;
                AsyncExecuteThread exe = null;
                foreach (var i in mExes)
                {
                    if (i.GetNumber() < MinNumber)
                    {
                        MinNumber = i.GetNumber();
                        exe = i;
                    }
                }
                if (exe != null)
                {
                    return exe.AsyncExe(pushQueue);
                }
                return null;
            }
        }

        public void InitManager(int nThread)
        {
            mExes.Clear();
            for (int i = 0; i < nThread; i++)
            {
                mExes.Add(new AsyncExecuteThread());
            }
        }

        public void StartThread()
        {
            foreach (var i in mExes)
            {
                i.StartThread();
            }
        }
        public void StopThread()
        {
            foreach (var i in mExes)
            {
                i.StopThread();
            }
        }
    }
}
