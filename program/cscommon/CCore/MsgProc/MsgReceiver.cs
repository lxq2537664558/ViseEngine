using System;
using System.Collections.Generic;

namespace CCore.MsgProc
{
    /// <summary>
    /// 消息过滤
    /// </summary>
    public class MsgFilter
    {
        /// <summary>
        /// 消息起点
        /// </summary>
        public int mMsgBegin;
        /// <summary>
        /// 消息终点
        /// </summary>
        public int mMsgEnd;
    }
    /// <summary>
    /// 声明行为动作的委托事件
    /// </summary>
    /// <param name="init">行为参数对象</param>
    /// <returns>返回行为值</returns>
    public delegate int FBehaviorProcess(BehaviorParameter init);
    /// <summary>
    /// 消息接收
    /// </summary>
    public class MsgReceiver : CSUtility.Support.XndSaveLoadProxy
    {
        /// <summary>
        /// 是否删除消息
        /// </summary>
        public bool mIsRemoved;
        /// <summary>
        /// 不包含的过滤列表
        /// </summary>
        public List<MsgFilter> mExcludeFilters;
        /// <summary>
        /// 包含的过滤器列表
        /// </summary>
        public List<MsgFilter> mIncludeFilters;
        /// <summary>
        /// 是否强制为异步行为消息，默认为false
        /// </summary>
        static public bool ForceAsyncBehavior = false;
        /// <summary>
        /// 程序的主线程
        /// </summary>
        static public System.Threading.Thread MainThread = null;
        /// <summary>
        /// 是否为系统消息
        /// </summary>
        /// <param name="msg">发送的消息</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数</param>
        /// <returns>为系统消息返回true</returns>
        public virtual bool OnSystemMsg(int msg, UIntPtr wParam, UIntPtr lParam)
        {
            return true;
        }
        /// <summary>
        /// 消息对
        /// </summary>
        class BHVPair
        {
            /// <summary>
            /// 定义消息行为的委托事件
            /// </summary>
            public FBehaviorProcess proc;
            /// <summary>
            /// 行为参数
            /// </summary>
            public BehaviorParameter parameter;
            /// <summary>
            /// 是否成对，默认为false
            /// </summary>
            public bool isok = false;
        }
        private List<BHVPair> mAsyncBehaviors = new List<BHVPair>();
        /// <summary>
        /// 处理消息行为
        /// </summary>
        /// <param name="bhType">消息行为的类型</param>
        /// <param name="bhInit">消息行为的参数</param>
        /// <param name="bAsync">是否异步处理，默认为false</param>
        /// <returns>返回行为值</returns>
        public virtual int OnBehavior(int bhType, BehaviorParameter bhInit, bool bAsync = false)
        {
            FBehaviorProcess bhvr = FindBehavior(bhInit);
            if (bhvr != null)
            {
                if(bAsync|| (ForceAsyncBehavior&& System.Threading.Thread.CurrentThread != MainThread))
                {   
                    lock (mAsyncBehaviors)
                    {
                        var sv = new BHVPair();
                        sv.proc = bhvr;
                        sv.parameter = bhInit;
                        mAsyncBehaviors.Add(sv);
                    }
                }
                else
                {
                    return bhvr(bhInit);
                }
            }   
            return 0;
        }
        /// <summary>
        /// 根据行为参数查找消息行为
        /// </summary>
        /// <param name="bhInit">行为参数对象</param>
        /// <returns>返回消息行为对象</returns>
        public virtual FBehaviorProcess FindBehavior(BehaviorParameter bhInit)
        {
            return null;
        }
        /// <summary>
        /// 异步刷新消息行为
        /// </summary>
        public void TickAsyncBehaviors()
        {
            lock(mAsyncBehaviors)
            {
                for (int i = 0; i < mAsyncBehaviors.Count; i++)
                {
                    var sv = mAsyncBehaviors[i];
                    if(sv.isok)
                    {
                        mAsyncBehaviors.RemoveAt(i);
                        i--;
                    }
                }
            }
            
            for (int i=0 ; i<mAsyncBehaviors.Count;i++)
            {
                var sv = mAsyncBehaviors[i];
                sv.proc(sv.parameter);
                sv.isok = true;
            }
        }
    }
}
