using System;
using System.Collections.Generic;

namespace SCore.DB
{
    public class AccountLoginHolder
    {
        public UInt16 LinkHandle;
        public Guid LinkSerialId;
        public string Name;
        public string Password;
        public string Serialid;
        public Guid PlanesId;
        public CSUtility.Net.NetConnection Connect;
        public UInt16 ReturnSerialId;
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class AccountLoginThread
    {
        static AccountLoginThread smInstance = new AccountLoginThread();
        public static AccountLoginThread Instance
        {
            get { return smInstance; }
        }

        public Unit.PlayerDBUnitManager PlayerManager = null;

        public void PushLogin(AccountLoginHolder loginHolder)
        {
            int queueCount = 0;
            lock (this)
            {
                foreach (var i in mLoginQueue)
                {
                    queueCount++;
                    if (i.Name == loginHolder.Name)
                    {
                        RPC.PackageWriter retPkg = new RPC.PackageWriter();
                        retPkg.Write((sbyte)-100);//已经在登录队列中了，请等待
                        retPkg.Write(queueCount);//告诉你前面还有多少人在等待
                        retPkg.DoReturnCommand2(loginHolder.Connect, loginHolder.ReturnSerialId);
                        return;
                    }
                }
                mLoginQueue.Add(loginHolder);
                queueCount = mLoginQueue.Count;
            }
        }
        public void CancelLogin(string name)
        {
            lock (this)
            {
                for (int i = 0; i < mLoginQueue.Count; i++)
                {
                    if (mLoginQueue[i].Name == name)
                    {
                        mLoginQueue.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        List<AccountLoginHolder> mLoginQueue = new List<AccountLoginHolder>();
        public List<AccountLoginHolder> LoginQueue
        {
            get { return mLoginQueue; }
        }
        public int GetNumber()
        {
            return mLoginQueue.Count;
        }
        //Queue<AtomHolder> mEnterGameQueue;
        void Tick()
        {
            if (mLoginQueue.Count > 0)
            {
                AccountLoginHolder atom = null; 
                lock (this)
                {
                    atom = mLoginQueue[0];
                    mLoginQueue.RemoveAt(0);
                }
                try
                {
                    PlayerManager?.OnAccountEnter(atom);
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }
            }
        }
        bool mRunning;
        private DBConnect mDBConnect;
        public DBConnect DBConnect
        {
            get { return mDBConnect; }
        }
        private System.Threading.Thread mThread;
        public void ThreadLoop()
        {
            while (mRunning)
            {
                Tick();
                System.Threading.Thread.Sleep(5);
            }
            mThread = null;
            Log.FileLog.WriteLine("PlayerLoginThread Thread exit!");
        }
        public void StartThread(string dbIp,string dbName)
        {
            mDBConnect = new DBConnect();
            mDBConnect.OpenConnect(dbIp, dbName);
            mRunning = true;
            mThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.ThreadLoop));
            mThread.Start();
        }
        public void StopThread()
        {
            mRunning = false;
        }
    }

    public class RoleEnterHolder
    {
        public Guid linkSerialId;
        public UInt16 cltIndex;
        public Guid accountId;
        public Guid roleId;
        public CSUtility.Net.NetConnection connect;
        public UInt16 returnSerialId;

        public AsyncExecuter RoleCreator;
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class PlayerEnterThread
    {
        static PlayerEnterThread smInstance = new PlayerEnterThread();
        public static PlayerEnterThread Instance
        {
            get { return smInstance; }
        }

        public Unit.PlayerDBUnitManager PlayerManager;

        public void PushRoleCreator(AsyncExecuter exec)
        {
            var sqlAtom = new RoleEnterHolder();
            sqlAtom.RoleCreator = exec;
            //this.Push(sqlAtom);

            int queueCount = 0;
            lock (this)
            {
                mLoginQueue.Add(sqlAtom);
                queueCount = mLoginQueue.Count;
            }
        }

        public void Push(RoleEnterHolder holder)
        {
            int queueCount = 0;
            lock (this)
            {
                foreach (var i in mLoginQueue)
                {
                    queueCount++;
                    if (i.roleId == holder.roleId)
                    {
                        RPC.PackageWriter retPkg = new RPC.PackageWriter();
                        retPkg.Write((sbyte)-100);//已经在登录队列中了，请等待
                        retPkg.Write(queueCount);//告诉你前面还有多少人在等待
                        retPkg.DoReturnCommand2(holder.connect, holder.returnSerialId);

                        return;
                    }
                }
                mLoginQueue.Add(holder);
                queueCount = mLoginQueue.Count;
            }
        }
        public void Remove(Guid roldId)
        {
            lock (this)
            {
                for (int i = 0; i < mLoginQueue.Count; i++)
                {
                    if (mLoginQueue[i].roleId == roldId)
                    {
                        mLoginQueue.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        List<RoleEnterHolder> mLoginQueue = new List<RoleEnterHolder>();
        public List<RoleEnterHolder> LoginQueue
        {
            get { return mLoginQueue; }
        }
        public int GetNumber()
        {
            return mLoginQueue.Count;
        }
        void Tick(DBConnect dbConnect)
        {
            if (mLoginQueue.Count > 0)
            {
                RoleEnterHolder atom = null; 
                lock (this)
                {
                    atom = mLoginQueue[0];
                    mLoginQueue.RemoveAt(0);
                }
                try
                {
                    if (atom.RoleCreator != null)
                    {
                        if (atom.RoleCreator.Exec != null)
                            atom.RoleCreator.Exec();
                    }
                    else
                    {
                        if(PlayerManager!=null)
                            PlayerManager.OnRoleEnter(atom);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }
            }
        }
        bool mRunning;
        private DBConnect mDBConnect;
        public DBConnect DBConnect
        {
            get { return mDBConnect; }
        }
        private System.Threading.Thread mThread;
        public void ThreadLoop()
        {
            while (mRunning)
            {
                Tick(mDBConnect);
                System.Threading.Thread.Sleep(5);
            }
            mThread = null;
            Log.FileLog.WriteLine("PlayerEnterThread Thread exit!");
        }
        public void StartThread(string dbIp,string dbName)
        {
            mDBConnect = new DBConnect();
            mDBConnect.OpenConnect(dbIp, dbName);
            mRunning = true;
            mThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.ThreadLoop));
            mThread.Start();
        }
        public void StopThread()
        {
            mRunning = false;
        }
    }

    
    
}
