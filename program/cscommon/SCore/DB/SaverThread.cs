using System;
using System.Collections.Generic;

namespace SCore.DB
{
    public enum SqlExeType
    {
        Update,
        Insert,
        Select,
        Delete,
        Destroy,
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class DBOperator
    {
        public List<System.Data.SqlClient.SqlParameter> SqlParameters = new List<System.Data.SqlClient.SqlParameter>();
        public string SqlCode;
        public string Code
        {
            get { return SqlCode; }
        }
        public SqlExeType ExeType;

        public AsyncExecuter Executer = null;

        public System.Data.SqlClient.SqlCommand ToSqlCommand(System.Data.SqlClient.SqlConnection dbConnect)
        {
            var cmd = new System.Data.SqlClient.SqlCommand(SqlCode,dbConnect);
            //System.Data.SqlClient.SqlCommandBuilder myCB = new System.Data.SqlClient.SqlCommandBuilder(mDataAdapter);
            if (SqlParameters != null)
            {
                foreach (var i in SqlParameters)
                {
                    cmd.Parameters.Add(i);
                }                
            }
            
            return cmd;
        }
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class PlayerSaver
    {
        public Unit.PlayerDBUnit mPlayerDataEx;
        public bool mWaitRemove = false;
        System.Collections.Concurrent.ConcurrentQueue<DBOperator> mQueues = new System.Collections.Concurrent.ConcurrentQueue<DBOperator>();
        public System.Collections.Concurrent.ConcurrentQueue<DBOperator> Queues
        {
            get { return mQueues; }
        }
        public void Push(DBOperator saver)
        {
            if (saver == null)
            {
                if (mPlayerDataEx != null)
                {
                    Log.FileLog.WriteLine("不能压入null存盘" + mPlayerDataEx.Id);
                    return;
                }
            }
            mQueues.Enqueue(saver);
        }
        public void Tick(DBConnect dbConnect)
        {
            try
            {//这里之所以要try住，是因为存盘的过程中，如果发生了Clear，丢弃之前的存储指令，有可能触发迭代过程异常，但是这个异常不会影响到什么东西
                int ProcNumber = 0;
                DBOperator saver = null;
                ProcNumber = mQueues.Count;
                if (ProcNumber > 64)
                    ProcNumber = 64;
                if (ProcNumber == 0)
                {
                    return;
                }

                if (false == mQueues.TryDequeue(out saver))
                    return;

                if (saver == null)
                {
                    Log.FileLog.WriteLine("见鬼了，居然还有null存盘" + mPlayerDataEx.Id);
                    return;
                }

                while (saver != null )
                {
                    #region 真正数据库操作
                    try
                    {
                        if (saver.Executer != null)
                        {
                            if (saver.Executer.Exec != null)
                                saver.Executer.Exec();
                        }
                        else if (saver.ExeType == SqlExeType.Update)
                        {
                            var finished = dbConnect._ExecuteUpdate(saver);
                            if (finished==false)
                            {
                                continue;
                            }
                        }
                        else if (saver.ExeType == SqlExeType.Insert)
                        {
                            var finished = dbConnect._ExecuteInsert(saver);
                            if (finished == false)
                            {
                                continue;
                            }
                        }
                        else if (saver.ExeType == SqlExeType.Delete)
                        {
                            var finished = dbConnect._ExecuteDelete(saver);
                            if (finished == false)
                            {
                                continue;
                            }
                        }
                        else if (saver.ExeType == SqlExeType.Destroy)
                        {
                            var finished = dbConnect._ExecuteDestroy(saver);
                            if (finished == false)
                            {
                                continue;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.FileLog.WriteLine(ex.ToString());
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                    #endregion

                    ProcNumber--;
                    if (ProcNumber <= 0)
                        break;
                    if (mQueues.TryDequeue(out saver) == false)
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public bool IsEmpty()
        {
            return mQueues.Count==0;
        }
        public int GetNumber()
        {
            return mQueues.Count;
        }
        public void Clear()
        {
            DBOperator saver = null;
            while (mQueues.Count > 0)
            {
                mQueues.TryDequeue(out saver);
                Log.FileLog.WriteLine(string.Format("玩家[{0}]因为Clear丢失数据库操作:{1}",this.mPlayerDataEx.Id,saver.Code));
            }
        }
    }

    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class PlayerSaverThread
    {
        CSUtility.Support.AsyncObjManager<Unit.PlayerDBUnit, PlayerSaver> mPlayers = new CSUtility.Support.AsyncObjManager<Unit.PlayerDBUnit, PlayerSaver>();
        CSUtility.Support.AsyncObjManager<Guid, PlayerSaver> mIdPlayers = new CSUtility.Support.AsyncObjManager<Guid, PlayerSaver>();
        public CSUtility.Support.AsyncObjManager<Unit.PlayerDBUnit, PlayerSaver> Players
        {
            get { return mPlayers; }
        }
        public int GetPlayerCount()
        {
            return mPlayers.Count;
        }

        int mPlayerMaxSaverNumber;
        public int PlayerMaxSaverNumber
        {
            get { return mPlayerMaxSaverNumber; }
        }

        int mSaverNumber;
        public int SaverNumber
        {
            get { return mSaverNumber; }
        }
        public int GetSaverNumber()
        {
            return mSaverNumber;
        }
        public PlayerSaver AddPlayer(Unit.PlayerDBUnit player)
        {
            var saver = mPlayers.FindObj(player);
            if (saver != null)
                return saver;
            saver = new PlayerSaver();
            saver.mPlayerDataEx = player;
            if (player.SaverThread != null)
            {
                Log.FileLog.WriteLine("AddPlayer mSaverThread is not null");
            }
            player.SaverThread = this;
            mPlayers.Add(player, saver);
            mIdPlayers.Add(player.Id, saver);
            //PlayerSaver saver;
            //lock (this)
            //{
            //    saver = FindPlayerSaver(player);
            //    if (saver != null)
            //        return saver;
            //    saver = new PlayerSaver();
            //    saver.mPlayerDataEx = player;
            //    if (player.mSaverThread != null)
            //    {
            //        Log.FileLog.WriteLine("AddPlayer mSaverThread is not null");
            //    }
            //    player.mSaverThread = this;
            //    mPushPlayers.Add(player, saver);
            //}

            return saver;
        }
        public PlayerSaver FindPlayerSaver(Unit.PlayerDBUnit roleId)
        {
            return mPlayers.FindObj(roleId);
        }
        public PlayerSaver FindPlayerSaverById(Guid roleId)
        {
            return mIdPlayers.FindObj(roleId);
        }
        void Tick()
        {
            mPlayers.BeforeTick();
            mIdPlayers.BeforeTick();
            
            mSaverNumber = 0;
            mPlayerMaxSaverNumber = 0;

            CSUtility.Support.AsyncObjManager<Unit.PlayerDBUnit, PlayerSaver>.FOnVisitObject visitor = delegate(Unit.PlayerDBUnit key, PlayerSaver value, object arg)
            {
                var number = value.GetNumber();
                mSaverNumber += number;
                if (number > mPlayerMaxSaverNumber)
                    mPlayerMaxSaverNumber = number;

                value.Tick(mDBConnect);

                if (value.IsEmpty())
                {
                    if (value.mWaitRemove || mRunning == false)
                    {
                        value.mPlayerDataEx = null;
                        mPlayers.Remove(key);
                        mIdPlayers.Remove(key.Id);
                    }
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            };
            mPlayers.For_Each(visitor, null);

            mPlayers.AfterTick();
            mIdPlayers.AfterTick();
        }
        bool mRunning;
        DBConnect mDBConnect;
        public DBConnect DBConnect
        {
            get { return mDBConnect; }
        }
        public void ThreadLoop()
        {
            while (GetPlayerCount() > 0 || mRunning)
            {
                try
                {
                    Tick();
                }
                catch (System.Exception ex)
                {
                    Log.FileLog.WriteLine(ex.ToString());
                    Log.FileLog.WriteLine(ex.StackTrace.ToString());
                }

                System.Threading.Thread.Sleep(1);
            }
            mThread = null;

            Log.FileLog.WriteLine(System.String.Format("PlayerSaverThread{0} Thread exit!", mThreadIndex));
        }
        System.Threading.Thread mThread;
        int mThreadIndex;
        public void StartThread(int threadIndex, string dbIp, string dbName)
        {
            mDBConnect = new DBConnect();
            mDBConnect.OpenConnect(dbIp, dbName);

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
    public class DBConnectManager
    {
        static DBConnectManager smInstance = new DBConnectManager();
        public static DBConnectManager Instance
        {
            get { return smInstance; }
        }

        #region 多线程的数据库读取，主要用来做登陆，进入游戏队列
        AccountLoginThread mLoginLoader = new AccountLoginThread();
        public AccountLoginThread LoginLoader
        {
            get { return mLoginLoader; }
        }
        PlayerEnterThread mRoleLoadder = new PlayerEnterThread();
        public PlayerEnterThread RoleLoadder
        {
            get { return mRoleLoadder; }
        }
        public int GetLoginPipeNumber()
        {
            return mLoginLoader.GetNumber();
        }
        public void CancelLogin(string name)
        {
            mLoginLoader.CancelLogin(name);
        }
        #endregion

        #region 多线程数据库存储
        Dictionary<Guid, PlayerSaver> mPlayerSavers = new Dictionary<Guid, PlayerSaver>();
        List<PlayerSaverThread> mSaveThreads = new List<PlayerSaverThread>();
        public List<PlayerSaverThread> SaveThreads
        {
            get { return mSaveThreads; }
        }
        public void StartThreadPool(int num, string dbIp, string dbName)
        {
            for (int i = 0; i < num; i++)
            {
                PlayerSaverThread thread = new PlayerSaverThread();
                thread.StartThread(i,dbIp,dbName);
                mSaveThreads.Add(thread);
            }
        }
        public void StopThreadPool()
        {
            foreach (var i in mSaveThreads)
            {
                i.StopThread();
            }
        }
        public bool IsEmptyPool()
        {
            int count = 0;
            foreach (var i in mSaveThreads)
            {
                count += i.GetPlayerCount();
            }
            return count == 0;
        }
        public int GetPlayerSaverNumber()
        {
            int count = 0;
            foreach (var i in mSaveThreads)
            {
                count += i.GetSaverNumber();
            }
            return count;
        }
        public bool InPool(Unit.PlayerDBUnit roleId)
        {
            PlayerSaver saver = null;
            foreach (var i in mSaveThreads)
            {
                saver = i.FindPlayerSaver(roleId);
                if (saver != null)
                    return true;
            }
            return false;
        }
        public PlayerSaver FindPlayerSaverById(Guid roleId)
        {
            PlayerSaver saver = null;
            foreach (var i in mSaveThreads)
            {
                saver = i.FindPlayerSaverById(roleId);
                if (saver != null)
                    return saver;
            }
            return null;
        }
        public void _ClearSaver(Unit.PlayerDBUnit roleId)
        {
            PlayerSaver saver = null;
            foreach (var i in mSaveThreads)
            {
                saver = i.FindPlayerSaver(roleId);
                if (saver != null)
                {
                    saver.Clear();
                }
            }
        }
        public PlayerSaver AddPlayer(Unit.PlayerDBUnit roleId)
        {
            PlayerSaver saver = null;
            PlayerSaverThread thread = null;
            Int32 mincount = Int32.MaxValue;
            foreach (var i in mSaveThreads)
            {
                int count = i.GetPlayerCount();
                if (count < mincount)
                {
                    mincount = count;
                    thread = i;
                }
                saver = i.FindPlayerSaver(roleId);
                if (saver != null)
                    return saver;
            }

            if (thread==null)
            {
                Log.FileLog.WriteLine("AddPlayer thread is null");
            }

            saver = thread.AddPlayer(roleId);
            return saver;
        }
        public void RemovePlayer(Unit.PlayerDBUnit roleId)
        {
            PlayerSaver saver = null;
            foreach (var i in mSaveThreads)
            {
                saver = i.FindPlayerSaver(roleId);
                if (saver != null)
                    break;
            }
            if (saver != null)
                saver.mWaitRemove = true;
        }
        public void PushSave(Unit.PlayerDBUnit roleId, DBOperator dbOp)
        {
            if (dbOp == null)
                return;
            if (roleId == null)
            {
                //走到这里说明DataServer已经重启过了，之前的在线玩家需要存盘，放到一个特殊线程处理就好了
                switch (dbOp.ExeType)
                {
                    case SqlExeType.Update:
                        {
                            PlayerEnterThread.Instance.DBConnect._ExecuteUpdate(dbOp);
                        }
                        break;
                    case SqlExeType.Insert:
                        {
                            PlayerEnterThread.Instance.DBConnect._ExecuteInsert(dbOp);
                        }
                        break;
                    case SqlExeType.Delete:
                        {
                            PlayerEnterThread.Instance.DBConnect._ExecuteDelete(dbOp);
                        }
                        break;
                    case SqlExeType.Destroy:
                        {
                            PlayerEnterThread.Instance.DBConnect._ExecuteDestroy(dbOp);
                        }
                        break;
                }
                return;
            }
            PlayerSaver saver = AddPlayer(roleId);

            saver.Push(dbOp);
            saver.mWaitRemove = false;//只要有数据重新要存储，说明他又登陆进来了，不需要remove了
        }

        public void PushSave(Unit.PlayerDBUnit roleId, string SqlCode, SqlExeType ExeType)
        {
            PlayerSaver saver = AddPlayer(roleId);
            
            DBOperator sqlAtom = new DBOperator();
            sqlAtom.SqlCode = SqlCode;
            sqlAtom.ExeType = ExeType;
            saver.Push(sqlAtom);
            saver.mWaitRemove = false;//只要有数据重新要存储，说明他又登陆进来了，不需要remove了
        }

        public void PushExecuter(Unit.PlayerDBUnit roleId, AsyncExecuter exec)
        {
            PlayerSaver saver = null;
            PlayerSaverThread thread = null;
            Int32 mincount = Int32.MaxValue;
            foreach (var i in mSaveThreads)
            {
                int count = i.GetPlayerCount();
                if (count < mincount)
                {
                    mincount = count;
                    thread = i;
                }
                saver = i.FindPlayerSaver(roleId);
                if (saver != null)
                    break;
            }
            if (saver == null)
            {
                saver = thread.AddPlayer(roleId);
            }
            DBOperator sqlAtom = new DBOperator();
            sqlAtom.Executer = exec;
            saver.Push(sqlAtom);
            saver.mWaitRemove = false;//只要有数据重新要存储，说明他又登陆进来了，不需要remove了
        }
        #endregion
    }
}
