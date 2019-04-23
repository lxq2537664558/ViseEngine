using System;
using System.Collections.Generic;

namespace CCore.MsgProc
{
    /// <summary>
    /// 消息接收的初始化类
    /// </summary>
    public class MsgRecieverMgrInit
    {

    }
    /// <summary>
    /// 消息接收类
    /// </summary>
    public class MsgRecieverMgr
    {
        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //private static extern int GetKeyboardState(byte[] pbKeyState);


        //// 鼠标位置
        //static CSUtility.Support.Point mMousePoint;
        //public static CSUtility.Support.Point MousePoint
        //{
        //    get { return mMousePoint; }
        //}
        /// <summary>
        /// 行为命令结构
        /// </summary>
        protected struct BehaviorCmd
        {
            /// <summary>
            /// 消息接收对象
            /// </summary>
            public MsgReceiver Reciever;
            /// <summary>
            /// 行为类型
            /// </summary>
            public int BeType;
            /// <summary>
            /// 行为参数
            /// </summary>
            public BehaviorParameter BeInit;
        }
        /// <summary>
        /// 消息接收对象列表
        /// </summary>
        protected List<MsgReceiver> mReceivers;
        /// <summary>
        /// 行为命令队列
        /// </summary>
        protected Queue<BehaviorCmd> mBehaviors;
        /// <summary>
        /// 消息转换器
        /// </summary>
        protected IMsgTranslator mMsgTranslator;

        //protected byte[] keyStates;
        //protected byte[] oldKeyStates;
        //protected BehaviorParameter.KB_Char m_KB_Char;
        /// <summary>
        /// 构造函数，创建消息转换器
        /// </summary>
        public MsgRecieverMgr()
        {
            mMsgTranslator = new IMsgTranslator();

            //m_KB_Char = new BehaviorParameter.KB_Char();
            //keyStates = new byte[256];
            //oldKeyStates = new byte[256];
            //GetKeyboardState(keyStates);
            //GetKeyboardState(oldKeyStates);
        }
        /// <summary>
        /// 析构函数，删除对象，是否指针
        /// </summary>
        ~MsgRecieverMgr()
        {
            Cleanup();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_init">消息接收的初始化类</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool Initialize(MsgRecieverMgrInit _init)
        {
            return true;
        }
        /// <summary>
        /// 清空消息队列
        /// </summary>
        public void Cleanup()
        {
            UnHookWindow();

            if(mReceivers != null)
                mReceivers.Clear();

            //keyStates = null;
            //oldKeyStates = null;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        public void Tick()
        {
            foreach(var i in mReceivers)
            {
                i.TickAsyncBehaviors();
            }
        }
        /// <summary>
        /// 加密窗口消息
        /// </summary>
        public void HookWindow()
        {

        }
        /// <summary>
        /// 将消息解密
        /// </summary>
        public void UnHookWindow()
        {

        }
        /// <summary>
        /// 注册消息接收
        /// </summary>
        /// <param name="rcv">接收到的消息</param>
        public void RegReciever(MsgReceiver rcv)
        {
            if (rcv == null)
                return;

            rcv.mIsRemoved = false;
            if (mReceivers == null)
                mReceivers = new List<MsgReceiver>();

            foreach (var i in mReceivers)
            {
                if (i == rcv)
                    return;
            }

            mReceivers.Add(rcv);
        }
        /// <summary>
        /// 删除指定的注册消息
        /// </summary>
        /// <param name="rcv">接收消息的对象</param>
        public void UnRegReciever(MsgReceiver rcv)
        {
            if (rcv == null || mReceivers == null)
                return;

            rcv.mIsRemoved = true;
        }
        /// <summary>
        /// 系统消息处理
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数</param>
        public void OnSystemMsg(int msg, UIntPtr wParam, UIntPtr lParam)
        {
            if (mReceivers != null)
            {
                for (int i = 0; i < mReceivers.Count; i++)
                {
                    MsgReceiver rcv = mReceivers[i];
                    if (rcv.mIsRemoved == true)
                    {
                        mReceivers.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (rcv.OnSystemMsg(msg, wParam, lParam))
                    {
                        //System.Diagnostics.Debug.WriteLine("=====MSG: " + msg);
                    }
                }
            }

            if (mMsgTranslator == null)
                return;

            var tr = mMsgTranslator.Translate(msg, wParam, lParam);
            if (tr == null)
                return;

            Dispatch(tr.BeType, tr.BeInit);
        }

        //public void OnKeyboardStateCheck()
        //{
        //    //if (GetKeyboardState(keyStates) != 0)
        //    //{
        //    //    for (int i = 0; i < 256; ++i)
        //    //    {
        //    //        m_KB_Char.Key = i;
        //    //        if ((keyStates[i] & 0x80) == 0x80)
        //    //        {
        //    //            m_KB_Char.behavior = MidLayer.BehaviorType.BHT_KB_Char_Down;
        //    //            Dispatch((int)m_KB_Char.behavior, m_KB_Char);
        //    //        }

        //    //        if (keyStates[i] != oldKeyStates[i])
        //    //        {
        //    //            if ((keyStates[i] & 0x80) == 0)
        //    //            {
        //    //                m_KB_Char.behavior = MidLayer.BehaviorType.BHT_KB_Char_Up;
        //    //                Dispatch((int)m_KB_Char.behavior, m_KB_Char);
        //    //            }

        //    //            oldKeyStates[i] = keyStates[i];
        //    //        }
        //    //    }
        //    //}
        //}
        /// <summary>
        /// 消息分发
        /// </summary>
        /// <param name="bht">消息行为类型</param>
        /// <param name="bhInit">消息行为初始化对象</param>
        public void Dispatch(int bht, BehaviorParameter bhInit)
        {
            if (mReceivers == null)
                return;
            
            //foreach (var rcv in mReceivers)
            for (int i = 0; i < mReceivers.Count; i++ )
            {
                var rcv = mReceivers[i];

                bool bExclude = false;
                if (rcv.mExcludeFilters != null)
                {
                    foreach (var flt in rcv.mExcludeFilters)
                    {
                        if (bht >= flt.mMsgBegin && bht <= flt.mMsgEnd)
                        {
                            bExclude = true;
                            break;
                        }
                    }
                    if (bExclude == true)
                        continue;
                }

                if (rcv.mIncludeFilters != null)
                {
                    foreach (var flt in rcv.mIncludeFilters)
                    {
                        if (bht >= flt.mMsgBegin && bht <= flt.mMsgEnd)
                        {
                            rcv.OnBehavior(bht, bhInit);
                            break;
                        }
                    }
                }
                else
                {
                    rcv.OnBehavior(bht, bhInit);
                }
            }
        }
        /// <summary>
        /// 邮寄行为消息
        /// </summary>
        public void ProcessPostBehavior()
        {
            if (mBehaviors == null)
                return;

            foreach (var cmd in mBehaviors)
            {
                if (cmd.Reciever == null)
                    continue;
                cmd.Reciever.OnBehavior(cmd.BeType, cmd.BeInit);
            }
            mBehaviors.Clear();
        }
        /// <summary>
        /// 发送消息行为
        /// </summary>
        /// <param name="rcv">接收到的消息</param>
        /// <param name="bht">行为类型</param>
        /// <param name="bhInit">行为参数</param>
        /// <returns>返回消息响应值</returns>
        public int SendBehavior(MsgReceiver rcv, int bht, BehaviorParameter bhInit)
        {
            if (rcv == null)
                return 0;
            return rcv.OnBehavior(bht, bhInit);
        }
        /// <summary>
        /// 邮寄消息行为
        /// </summary>
        /// <param name="rcv">接收到的消息</param>
        /// <param name="bht">消息行为类型</param>
        /// <param name="bhInit">行为参数</param>
        public void PostBehavior(MsgReceiver rcv, int bht, BehaviorParameter bhInit)
        {
            if (mBehaviors == null)
                mBehaviors = new Queue<BehaviorCmd>();

            var cmd = new BehaviorCmd();
            cmd.BeInit = bhInit;
            cmd.BeType = bht;
            cmd.Reciever = rcv;
            mBehaviors.Enqueue(cmd);
        }
    }
}
