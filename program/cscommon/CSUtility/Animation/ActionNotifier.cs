using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CSUtility.Animation
{
    /// <summary>
    /// 动作播放模式枚举
    /// </summary>
    public enum EActionPlayerMode
    {
        Default,
        Loop,
        Pause,
    }
    /// <summary>
    /// 轴向根动作模式
    /// </summary>
    public enum AxisRootmotionType
	{
		ART_Default		= 0,
		ART_Discard,
		ART_Translate
	};
    /// <summary>
    /// 动画树结束时调用
    /// </summary>
    public delegate void Delegate_OnAnimTreeFinish();
    /// <summary>
    /// 动作结束时调用
    /// </summary>
    public delegate void Delegate_OnActionFinish();
    /// <summary>
    /// 动画树接口
    /// </summary>
    public interface AnimationTree
    {
        /// <summary>
        /// 动作
        /// </summary>
        BaseAction Action { get; set; }
        /// <summary>
        /// 当前监听时间
        /// </summary>
        Int64 CurNotifyTime { get; set; }
        /// <summary>
        /// 获取动画
        /// </summary>
        /// <returns>返回动画树列表</returns>
        List<AnimationTree> GetAnimations();
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">动画树节点</param>
        //void SetAnimations(List<AnimationTree> anims);        
        void AddNode( AnimationTree node );
        /// <summary>
        /// 动作是否完成
        /// </summary>
        /// <returns>完成返回true，否则返回false</returns>
        bool IsActionFinished();
        /// <summary>
        /// 设置动作是否循环
        /// </summary>
        /// <param name="bLoop">该动作是否循环</param>
        void SetLoop(bool bLoop);
        /// <summary>
        /// 获取动作是否循环
        /// </summary>
        /// <returns>该动作循环返回true，否则返回false</returns>
        bool GetLoop();
        /// <summary>
        /// 设置动作的播放速度
        /// </summary>
        /// <param name="playRate">动作的播放速度</param>
        void SetPlayRate(float playRate);
        /// <summary>
        /// 获取动作的播放速度
        /// </summary>
        /// <returns>返回动作的播放速度</returns>
        float GetPlayRate();
        /// <summary>
        /// 动画树结束时调用的方法
        /// </summary>
        Delegate_OnAnimTreeFinish DelegateOnAnimTreeFinish { get; set; }
        /// <summary>
        /// 动作结束时调用的方法
        /// </summary>
		Delegate_OnActionFinish DelegateOnActionFinish { get; set; }
    }
    /// <summary>
    /// 基类动作类
    /// </summary>
    public interface BaseAction : AnimationTree
    {
        /// <summary>
        /// 动作名称
        /// </summary>
        string ActionName { get; set; }
        /// <summary>
        /// X轴的根动作类型
        /// </summary>
        AxisRootmotionType XRootmotionType { get; set; }
        /// <summary>
        /// Y轴的根动作类型
        /// </summary>
        AxisRootmotionType YRootmotionType { get; set; }
        /// <summary>
        /// Z轴的根动作类型
        /// </summary>
        AxisRootmotionType ZRootmotionType { get; set; }
        /// <summary>
        /// 播放模式
        /// </summary>
        EActionPlayerMode PlayerMode { get; set; }
        /// <summary>
        /// 只读属性，动作持续时间
        /// </summary>
        Int64 Duration { get; }
        /// <summary>
        /// 播放速度
        /// </summary>
        double PlayRate { get; set; }
        /// <summary>
        /// 获取相应名称的监听器
        /// </summary>
        /// <param name="name">监听器名称</param>
        /// <returns>返回相应名称的监听器</returns>
        ActionNotifier GetNotifier(string name);
        /// <summary>
        /// 获取相应的监听器
        /// </summary>
        /// <param name="index">监听器索引值</param>
        /// <returns>返回相应的监听器</returns>
        ActionNotifier GetNotifier(UInt32 index);
        /// <summary>
        /// 获取相应类型的监听器
        /// </summary>
        /// <param name="type">监听器的类型</param>
        /// <returns>返回相应类型的监听器</returns>
        List<ActionNotifier> GetNotifiers(Type type);
    }
    /// <summary>
    /// 访问监听器时调用的委托对象
    /// </summary>
    /// <param name="ntf">动作监听器</param>
    /// <param name="np">监听点</param>
    /// <returns>访问成功返回true，否则返回false</returns>
    public delegate bool FOnVisitNotifier(ActionNotifier ntf , NotifyPoint np );
    /// <summary>
    /// 动作监听器类
    /// </summary>
    public class ActionNotifier : CSUtility.Support.XndSaveLoadProxy, Animation.TimeLineObjectInterface
    {
        #region ForEditor
        /// <summary>
        /// 监听器名称
        /// </summary>
        public string TimeLineObjectName
        {
            get { return NotifyName; }
            set { NotifyName = value; }
        }
        /// <summary>
        /// 添加关键帧
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="name">监听点名称</param>
        /// <returns>返回该关键帧</returns>
        public Animation.TimeLineKeyFrameObjectInterface AddKeyFrameObject(Int64 startTime, Int64 endTime, string name)
        {
            return AddNotifyPoint(startTime, name);
        }
        /// <summary>
        /// 删除关键帧
        /// </summary>
        /// <param name="frame">关键帧对象</param>
        public void RemoveKeyFrameObject(Animation.TimeLineKeyFrameObjectInterface frame)
        {
            var np = frame as NotifyPoint;
            if (np != null)
                mNotifyPoints.Remove(np);
        }
        /// <summary>
        /// 获取关键帧
        /// </summary>
        /// <returns>返回所有的关键帧</returns>
        public List<Animation.TimeLineKeyFrameObjectInterface> GetKeyFrames()
        {
            var retList = new List<Animation.TimeLineKeyFrameObjectInterface>();
            foreach (var kf in NotifyPoints)
            {
                retList.Add(kf);
            }

            return retList;
        }

        #endregion
        /// <summary>
        /// 获取监听器的服务器客户端类型
        /// </summary>
        /// <returns>返回Common类型</returns>
        public virtual CSUtility.Helper.enCSType GetNotifyCSType()
        {
            return CSUtility.Helper.enCSType.Common;
        }
        /// <summary>
        /// 获取关键帧类型
        /// </summary>
        /// <returns>返回关键帧类型</returns>
        public virtual Type GetKeyFrameType()
        {
            return typeof(NotifyPoint);
        }

        //这个Action的名字
        /// <summary>
        /// 监听名称
        /// </summary>
        protected string mNotifyName = "未知Notifier";
        /// <summary>
        /// 监听名称
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public string NotifyName
        {
            get { return mNotifyName; }
            set { mNotifyName = value; }
        }
        /// <summary>
        /// 前一帧的时间
        /// </summary>
        protected Int64 mPreFrameMilliSecond = 0;
        /// <summary>
        /// 当前帧的时间
        /// </summary>
        protected Int64 mCurFrameMilliSecond = 0;
        /// <summary>
        /// 动作持续的时间
        /// </summary>
        protected Int64 mActionDurationMilliSecond = 1;
        /// <summary>
        /// 动作持续的时间
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public Int64 ActionDurationMilliSecond
        {
            get { return mActionDurationMilliSecond; }
            set { mActionDurationMilliSecond = value; }
        }
        //这个值除开第一次Load有序数组，否则不允许直接操作，必须用AddNotifier来添加
        /// <summary>
        /// 监听点列表
        /// </summary>
        protected List<NotifyPoint> mNotifyPoints = new List<NotifyPoint>();
        /// <summary>
        /// 监听点列表
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public List<NotifyPoint> NotifyPoints
        {
            get { return mNotifyPoints; }
            set { mNotifyPoints = value; }
        }
        /// <summary>
        /// 添加监听点
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="name">监听名称</param>
        /// <returns>返回监听点</returns>
        public virtual NotifyPoint AddNotifyPoint(Int64 time, string name)
        {
            for (int i = 0; i < mNotifyPoints.Count; i++)
            {
                if (mNotifyPoints[i].NotifyTime >= time)
                {
                    var np = new NotifyPoint(time, name);
                    mNotifyPoints.Insert(i, np);
                    return np;
                }
            }

            var ntP = new NotifyPoint(time, name);
            mNotifyPoints.Add(ntP);
            return ntP;
        }

        //endTime必须大于startTime
        List<NotifyPoint> _GetNotifyPoints(Int64 startTime, Int64 endTime)
        {
            List<NotifyPoint> result = new List<NotifyPoint>();

            if (mNotifyPoints.Count == 0)
                return result;

            if (endTime < startTime)
                return result;

            int startIndex;
            if (mNotifyPoints.Count == 1)
            {   
                if (mNotifyPoints[0].KeyFrameMilliTimeStart >= startTime && mNotifyPoints[0].KeyFrameMilliTimeEnd <= endTime)
                {
                    result.Add(mNotifyPoints[0]);
                    return result;
                }
                return result;
            }
            else
                startIndex = FindEqualGreaterNotifyPoint(startTime, 0, mNotifyPoints.Count - 1);

            if (mNotifyPoints[startIndex].KeyFrameMilliTimeStart > endTime)
                return result;

            for (int i = startIndex; i < mNotifyPoints.Count; i++)
            {
                if (mNotifyPoints[0].KeyFrameMilliTimeStart >= startTime && mNotifyPoints[0].KeyFrameMilliTimeEnd <= endTime)
                {
                    result.Add(mNotifyPoints[i]);
                }
                else
                {
                    continue;
                }
            }
            return result;
        }

        bool mIsNotifyFinished = false;
        /// <summary>
        /// 监听是否完成
        /// </summary>
        [CSUtility.Support.DoNotCopy]
        public bool IsNotifyFinished
        {
            get { return mIsNotifyFinished; }
            set { mIsNotifyFinished = value; }
        }
        /// <summary>
        /// 获取监听点
        /// </summary>
        /// <param name="prevTime">前一个监听点的时间</param>
        /// <param name="nowTime">当前的时间</param>
        /// <returns>返回监听点列表</returns>
        public List<NotifyPoint> GetNotifyPoints(Int64 prevTime, Int64 nowTime)
        {
            var result = _GetNotifyPoints(prevTime, nowTime);

            return result;
        }
        /// <summary>
        /// 查找相同的监听点
        /// </summary>
        /// <param name="time">监听时间</param>
        /// <param name="start">起始值</param>
        /// <param name="end">结束值</param>
        /// <returns>返回监听点</returns>
        int FindEqualGreaterNotifyPoint(Int64 time, int start, int end)
        {
            if (end - start == 1)
            {
                return start;
            }
            else
            {
                int mid = (start + end) / 2;
                if (mNotifyPoints[mid].NotifyTime >= time)
                {
                    return FindEqualGreaterNotifyPoint(time, start, mid);
                }
                else// if (mNotifyPoints[mid].NotifyTime < time)
                {
                    return FindEqualGreaterNotifyPoint(time, mid, end);
                }
            }
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="time">每帧之间的间隔时间</param>
        public virtual void Tick(Int64 time)
        {
            mPreFrameMilliSecond = mCurFrameMilliSecond;
            mCurFrameMilliSecond += time;

            if (mCurFrameMilliSecond >= ActionDurationMilliSecond)
                mCurFrameMilliSecond = mCurFrameMilliSecond - ActionDurationMilliSecond;
        }
        /// <summary>
        /// 更新动作
        /// </summary>
        /// <param name="host">主Actor对象</param>
        /// <param name="preTime">上次时间</param>
        /// <param name="nowTime">当前时间</param>
        public virtual void UpdateActive(CSUtility.Component.ActorBase host, long preTime, long nowTime)
        {
            // 当前时间和上次时间一样，说明动作暂停中
            if (preTime == nowTime)
                return;

            var notifys = GetNotifyPoints(preTime, nowTime);
            foreach (var ntf in notifys)
            {
                ntf.ActiveNotifyPoint(host);
            }
        }
    }
    /// <summary>
    /// 动作资源类
    /// </summary>
    public class ActionSource : CSUtility.Helper.IVersionInterface, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 属性改变时调用的方法
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        UInt32 mVer = 1;
        /// <summary>
        /// 版本号
        /// </summary>
        public UInt32 Ver
        {
            get { return mVer; }
            set
            {
                mVer = value;
                OnPropertyChanged("Ver");
            }
        }
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns>返回版本号</returns>
        public UInt32 GetVersion()
        {
            return mVer;
        }

        Int64 mDuration = 0;
        /// <summary>
        /// 持续时间
        /// </summary>
        public Int64 Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }

        float mPlayRate = 1.0f;
        /// <summary>
        /// 播放速度
        /// </summary>
        public float PlayRate
        {
            get { return mPlayRate; }
            set
            {
                mPlayRate = value;
            }
        }

        Dictionary<Type, List<CSUtility.Animation.ActionNotifier>> mActionNotifierTypeDic = new Dictionary<Type, List<CSUtility.Animation.ActionNotifier>>();
        Dictionary<string, List<CSUtility.Animation.ActionNotifier>> mActionNotifierNameDic = new Dictionary<string, List<CSUtility.Animation.ActionNotifier>>();
        List<CSUtility.Animation.ActionNotifier> mNotifierList = new List<CSUtility.Animation.ActionNotifier>();
        /// <summary>
        /// 只读属性，监听器列表
        /// </summary>
        public CSUtility.Animation.ActionNotifier[] NotifierList
        {
            get { return mNotifierList.ToArray(); }
        }

        Guid mMeshId; // for editor
        /// <summary>
        /// 使用的meshID
        /// </summary>
        public Guid MeshId
        {
            get { return mMeshId; }
            set { mMeshId = value; }
        }
        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="notifier">动作监听器名称</param>
        public void AddNotifier(CSUtility.Animation.ActionNotifier notifier)
        {
            if (mNotifierList.Contains(notifier))
                return;

            mNotifierList.Add(notifier);

            List<CSUtility.Animation.ActionNotifier> tempNts;
            if (!mActionNotifierNameDic.TryGetValue(notifier.NotifyName, out tempNts))
            {
                tempNts = new List<ActionNotifier>();
                mActionNotifierNameDic[notifier.NotifyName] = tempNts;
            }
            tempNts.Add(notifier);

            if (!mActionNotifierTypeDic.TryGetValue(notifier.GetType(), out tempNts))
            {
                tempNts = new List<ActionNotifier>();
                mActionNotifierTypeDic[notifier.GetType()] = tempNts;
            }
            tempNts.Add(notifier);
        }
        /// <summary>
        /// 删除监听器
        /// </summary>
        /// <param name="notifier">监听器名称</param>
        public void RemoveNotifier(CSUtility.Animation.ActionNotifier notifier)
        {
            if (mNotifierList.Contains(notifier))
            {
                mNotifierList.Remove(notifier);

                List<CSUtility.Animation.ActionNotifier> tempNts;
                if (mActionNotifierNameDic.TryGetValue(notifier.NotifyName, out tempNts))
                {
                    tempNts.Remove(notifier);
                }

                if (mActionNotifierTypeDic.TryGetValue(notifier.GetType(), out tempNts))
                {
                    tempNts.Remove(notifier);
                }
            }
        }
        /// <summary>
        /// 获取监听器
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回相应的监听器</returns>
        public ActionNotifier GetNotifier(int index)
        {
            if (index >= 0 && index < mNotifierList.Count)
                return mNotifierList[index];

            return null;
        }
        /// <summary>
        /// 获取第一个监听器
        /// </summary>
        /// <param name="name">监听器的名称</param>
        /// <returns>返回相应的监听器</returns>
        public ActionNotifier GetFirstNotifier(string name)
        {
            var nts = GetNotifier(name);
            if (nts.Count > 0)
                return nts[0];

            return null;
        }
        /// <summary>
        /// 获取相应名称的监听器
        /// </summary>
        /// <param name="name">监听器名称</param>
        /// <returns>返回相应名称的监听器列表</returns>
        public List<ActionNotifier> GetNotifier(string name)
        {
            List<ActionNotifier> nts;
            if (!mActionNotifierNameDic.TryGetValue(name, out nts))
            {
                return new List<ActionNotifier>();
            }

            return nts;
        }
        /// <summary>
        /// 获取相应类型的监听器列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>返回相应类型的监听器列表</returns>
        public List<ActionNotifier> GetNotifier(Type type)
        {
            List<ActionNotifier> nts;
            if (!mActionNotifierTypeDic.TryGetValue(type, out nts))
            {
                return new List<ActionNotifier>();
            }

            return nts;
        }
        /// <summary>
        /// 保存动作监听器
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="verUpdate">是否更新版本，缺省为true</param>
        public void SaveActionNotifier(string fileName, bool verUpdate = true)
        {
            var holder = CSUtility.Support.XndHolder.NewXNDHolder();

            if(verUpdate)
                Ver++;

            var verAttr = holder.Node.AddAttrib("Ver");
            verAttr.BeginWrite();
            verAttr.Write(Ver);
            verAttr.EndWrite();

            var attr = holder.Node.AddAttrib("Header");
            // 存储预览用模型
            attr.BeginWrite();

            attr.Write(mMeshId);
            attr.Write(mDuration);
            attr.Write(mPlayRate);

            attr.EndWrite();

            var notifyNode = holder.Node.AddNode("Notifys", 0, 0);
            foreach (var notify in mNotifierList)
            {
                var notifyTypeName = CSUtility.Program.GetTypeSaveString(notify.GetType());
                var notifyAtt = notifyNode.AddAttrib(notifyTypeName);
                notifyAtt.BeginWrite();

                notify.Write(notifyAtt);

                notifyAtt.EndWrite();
            }

            fileName = fileName + CSUtility.Support.IFileConfig.ActionNotifyExtension;
            CSUtility.Support.XndHolder.SaveXND(fileName, holder);
        }
        /// <summary>
        /// 加载动作监听器
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="csType">服务器客户端类型</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public bool LoadActionNotifier(string fileName, CSUtility.Helper.enCSType csType)
        {
            mNotifierList.Clear();
            mActionNotifierTypeDic.Clear();
            mActionNotifierNameDic.Clear();

            fileName = fileName + CSUtility.Support.IFileConfig.ActionNotifyExtension;
            var holder = CSUtility.Support.XndHolder.LoadXND(fileName);
            if (holder == null)
                return false;

            var verAttr = holder.Node.FindAttrib("Ver");
            if (verAttr != null)
            {
                verAttr.BeginRead();
                UInt32 ver = 0;
                verAttr.Read(out ver);
                Ver = ver;
                verAttr.EndRead();
            }

            var attr = holder.Node.FindAttrib("Header");
            if (attr != null)
            {
                attr.BeginRead();

                attr.Read(out mMeshId);
                attr.Read(out mDuration);
                if(Ver==2)
                {
                    attr.Read(out mPlayRate);
                }

                attr.EndRead();
            }

            var notifyNode = holder.Node.FindNode("Notifys");
            if (notifyNode != null)
            {
                var notifyAtts = notifyNode.GetAttribs();
                foreach (var ntfAtt in notifyAtts)
                {
                    var notifyTypeStr = ntfAtt.GetName();
                    var notifyType = CSUtility.Program.GetTypeFromSaveString(notifyTypeStr, csType);
                    if (notifyType == null)
                        continue;

                    var notify = System.Activator.CreateInstance(notifyType) as CSUtility.Animation.ActionNotifier;
                    if (notify == null)
                        continue;

                    switch (csType)
                    {
                        case Helper.enCSType.Client:
                            {
                                if (notify.GetNotifyCSType() == CSUtility.Helper.enCSType.Server)
                                    continue;
                            }
                            break;

                        case Helper.enCSType.Server:
                            {
                                if (notify.GetNotifyCSType() == CSUtility.Helper.enCSType.Client)
                                    continue;
                            }
                            break;
                    }

                    ntfAtt.BeginRead();
                    notify.Read(ntfAtt);
                    ntfAtt.EndRead();

                    notify.ActionDurationMilliSecond = mDuration;

                    AddNotifier(notify);
                    //mNotifierList.Add(notify);
                }
            }

            holder.Node.TryReleaseHolder();

            return true;
        }
    }
    /// <summary>
    /// 动作节点管理器类
    /// </summary>
    public class ActionNodeManager
    {
        static ActionNodeManager smInstance = new ActionNodeManager();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static ActionNodeManager Instance
        {
            get { return smInstance; }
        }

        Dictionary<string, ActionSource> mActions = new Dictionary<string, ActionSource>();
        /// <summary>
        /// 获取动作的资源
        /// </summary>
        /// <param name="name">动作名称</param>
        /// <param name="bForceLoad">是否强制从磁盘加载</param>
        /// <param name="csType">服务器客户端类型</param>
        /// <returns>返回该动作的资源</returns>
        public ActionSource GetActionSource(string name, bool bForceLoad, CSUtility.Helper.enCSType csType)
        {
            lock (mActions)
            {
                ActionSource outAction;
                if (mActions.TryGetValue(name, out outAction))
                {
                    if (bForceLoad == true)
                    {
                        outAction.LoadActionNotifier(name, csType);
                    }
                    return outAction;
                }

                outAction = new ActionSource();
                if (outAction.LoadActionNotifier(name, csType) == false)
                    return null;
                mActions.Add(name, outAction);
            
                return outAction;
            }
        }
        /// <summary>
        /// 保存动作资源
        /// </summary>
        /// <param name="source">动作资源</param>
        /// <param name="fileName">文件名称</param>
        public void SaveActionSource(ActionSource source, string fileName)
        {
            source.SaveActionNotifier(fileName);
        }
    }
}
