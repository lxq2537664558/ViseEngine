using System;
using System.ComponentModel;

namespace CSUtility.Map.Trigger
{
    public abstract class TriggerDataFactory
    {
        public static TriggerDataFactory Instance;
        // 根据类型来创建TriggerData
        public abstract TriggerData CreateTriggerData(Byte triggerType);
    }

    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Trigger")]
    public delegate void FOnCreate(TriggerProcessData processData,TriggerData trigger);
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Trigger")]
    public delegate void FOnEnter(TriggerProcessData processData, TriggerData trigger);
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Trigger")]
    public delegate void FOnLeave(TriggerProcessData processData, TriggerData trigger);
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Trigger")]
    public delegate void FOnTriggerTick(TriggerData trigger);
    public class TriggerData : RPC.IAutoSaveAndLoad, CSUtility.Support.IXndSaveLoadProxy, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        // 需同步的属性改变时调用，用于同步更新服务器数据
        public delegate void Delegate_OnPropertyUpdate(TriggerData data);
        public event Delegate_OnPropertyUpdate OnPropertyUpdate;

        Guid mId = Guid.NewGuid();
        [CSUtility.Support.AutoSaveLoadAttribute]
        [ReadOnly(true)]
        public Guid Id
        {
            get { return mId; }
            set { mId = value; }
        }

        string mName = "";
        [CSUtility.Support.AutoSaveLoadAttribute]
        public string Name
        {
            get { return mName; }
            set
            {
                mName = value;

                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);

                OnPropertyChanged("Name");
            }
        }

        UInt16 mFactionId = UInt16.MaxValue;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public UInt16 FactionId
        {
            get { return mFactionId; }
            set
            {
                mFactionId = value;

                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);

                OnPropertyChanged("FactionId");
            }
        }

        // Tick间隔
        public long TickIntervalMillisecond { get; set; } = 10;

        public enum enProcessType
        {
            Client,
            Server,
            ClientAndServer,
        }
        enProcessType mProcessType = enProcessType.Client;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public enProcessType ProcessType
        {
            get { return mProcessType; }
            set
            {
                mProcessType = value;
                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);

                OnPropertyChanged("ProcessType");
            }
        }

        SlimDX.Matrix mTransMatrix = SlimDX.Matrix.Identity;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [Browsable(false)]
        public SlimDX.Matrix TransMatrix
        {
            get { return mTransMatrix; }
            set
            {
                mTransMatrix = value;
                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);

                OnPropertyChanged("TransMatrix");
            }
        }

        bool mEnable = true;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool Enable
        {
            get { return mEnable; }
            set
            {
                mEnable = value;

                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);

                OnPropertyChanged("Enable");
            }
        }
        
        string mTriggerGroup = "";
        [CSUtility.Support.AutoSaveLoadAttribute]
        [System.ComponentModel.DisplayName("触发器组名")]
        public string TriggerGroup
        {
            get { return mTriggerGroup; }
            set { mTriggerGroup = value; }
        }

        UInt16 mMonsterCreateTemplateId = UInt16.MaxValue;
        [CSUtility.Event.Attribute.AllowMember("触发器.怪物创建模板ID", CSUtility.Helper.enCSType.Common, "怪物创建模板ID")]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [System.ComponentModel.DisplayName("怪物创建模板Id")]
        public UInt16 MonsterCreateTemplateId
        {
            get { return mMonsterCreateTemplateId; }
            set { mMonsterCreateTemplateId = value; }
        }

        // delta为检测范围调整阈值，服务器验证时使用
        public bool IsPositionIn(SlimDX.Vector3 pos, float delta = 1.0f)
        {
            var invMat = SlimDX.Matrix.Invert(ref mTransMatrix);
            var triggerSpacePos = SlimDX.Vector3.TransformCoordinate(pos, invMat);
            if ((Math.Abs(triggerSpacePos.X) < (0.5f * delta)) &&
                //(Math.Abs(triggerSpacePos.Y) < (0.5f * delta)) &&
                (Math.Abs(triggerSpacePos.Z) < (0.5f * delta)))
                return true;

            return false;
        }

        public void Tick(long elapsedMillisecond)
        {
            try
            {
                var callee = OnTickCB?.GetCallee() as CSUtility.Map.Trigger.FOnTriggerTick;
                callee?.Invoke(this);
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
            }

            TickOverride(elapsedMillisecond);
        }
        public virtual void TickOverride(long elapsedMillisecond)
        {

        }

        public void ProcessEnter(TriggerProcessData processData)
        {
            try
            {
                // 在连线中视情况调用服务器同步接口
                var callee = OnEnterCB?.GetCallee() as CSUtility.Map.Trigger.FOnEnter;
                callee?.Invoke(processData, this);
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
            }

            ProcessEnterOverride(processData);
        }
        public virtual void ProcessEnterOverride(TriggerProcessData processData)
        {

        }

        public void ProcessLeave(TriggerProcessData processData)
        {
            try
            {
                var callee = OnLeaveCB?.GetCallee() as CSUtility.Map.Trigger.FOnLeave;
                callee?.Invoke(processData, this);
            }
            catch (System.Exception ex)
            {
                Log.FileLog.WriteLine(ex.ToString());
            }

            ProcessLeaveOverride(processData);
        }
        public virtual void ProcessLeaveOverride(TriggerProcessData processData)
        {

        }
        
        #region IXndSaveLoadProxy

        public bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            return CSUtility.Support.XndSaveLoadProxy.Read(this, xndAtt);
        }
        public bool Write(CSUtility.Support.XndAttrib xndAtt)
        {
            return CSUtility.Support.XndSaveLoadProxy.Write(this, xndAtt);
        }
        public bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            return CSUtility.Support.Copyable.CopyFrom(srcData,this);
        }

        #endregion

        #region 回调函数

        CSUtility.Helper.EventCallBack mOnEnterCB;
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        [RPC.FieldDontAutoSaveLoadAttribute]
        public CSUtility.Helper.EventCallBack OnEnterCB
        {
            get { return mOnEnterCB; }
        }

        Guid mOnEnter = Guid.Empty;
        [CSUtility.Support.AutoSaveLoadAttribute]        
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet", new object[] { typeof(FOnEnter) })]
        [Category("触发器")]
        public Guid OnEnter
        {// 进入Trigger调用回调
            get { return mOnEnter; }
            set
            {
                mOnEnter = value;
                mOnEnterCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnEnter), value);
            }
        }

        CSUtility.Helper.EventCallBack mOnLeaveCB;
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        [RPC.FieldDontAutoSaveLoadAttribute]
        public CSUtility.Helper.EventCallBack OnLeaveCB
        {
            get { return mOnLeaveCB; }
        }

        Guid mOnLeave = Guid.Empty;
        [CSUtility.Support.AutoSaveLoadAttribute]       
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet", new object[] { typeof(FOnLeave) })]
        [Category("触发器")]
        public Guid OnLeave
        {// 离开Trigger时调用回调
            get { return mOnLeave; }
            set
            {
                mOnLeave = value;
                mOnLeaveCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnLeave), value);
            }
        }

        CSUtility.Helper.EventCallBack mOnTickCB;
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        [RPC.FieldDontAutoSaveLoadAttribute]
        public CSUtility.Helper.EventCallBack OnTickCB
        {
            get { return mOnTickCB; }
        }

        Guid mOnTick = Guid.Empty;
        [CSUtility.Support.AutoSaveLoadAttribute]        
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet", new object[] { typeof(FOnTick) })]
        [Category("触发器")]
        public Guid OnTick
        {// 进入Trigger调用回调
            get { return mOnTick; }
            set
            {
                mOnTick = value;
                mOnTickCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnTick), value);
            }
        }

        CSUtility.Helper.EventCallBack mProcessDataOnTimerCB;
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        [RPC.FieldDontAutoSaveLoadAttribute]
        public CSUtility.Helper.EventCallBack ProcessDataOnTimerCB
        {
            get { return mProcessDataOnTimerCB; }
        }
        Guid mProcessDataOnTimer = Guid.Empty;
        [CSUtility.Support.AutoSaveLoadAttribute]        
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet", new object[] { typeof(FProcessDataOnTimer) })]
        [Category("触发器")]
        public Guid ProcessDataOnTimer
        {
            get { return mProcessDataOnTimer; }
            set
            {
                mProcessDataOnTimer = value;
                mProcessDataOnTimerCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FProcessDataOnTimer), value);
            }
        }

        CSUtility.Helper.EventCallBack mOnCreateCB;
        [System.ComponentModel.Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        [RPC.FieldDontAutoSaveLoadAttribute]
        public CSUtility.Helper.EventCallBack OnCreateCB
        {
            get { return mOnCreateCB; }
        }
        Guid mOnCreate = Guid.Empty;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EventSet", new object[] { typeof(FOnCreate) })]
        [Category("触发器")]
        public Guid OnCreate
        {
            get { return mOnCreate; }
            set
            {
                mOnCreate = value;
                mOnCreateCB = CSUtility.Helper.EventCallBackManager.Instance.GetCallee(typeof(FOnCreate), value);
            }
        }
        #endregion

        #region LogicTimer

        [CSUtility.AISystem.Attribute.ToolTip("添加一个逻辑计时器")]
        [CSUtility.AISystem.Attribute.AllowMember("触发器.函数.添加计时器", CSUtility.Helper.enCSType.Common, "给触发器添加计时器")]
        [CSUtility.Event.Attribute.AllowMember("触发器.函数.添加计时器", CSUtility.Helper.enCSType.Common, "给触发器添加计时器")]
        public void AddProcessDataLogicTimer(string name, Int64 interval, TriggerProcessData processData)
        {
            processData.AddLogicTimer(name, interval, this);
        }
        [CSUtility.AISystem.Attribute.ToolTip("移除指定名字的逻辑计时器")]
        [CSUtility.AISystem.Attribute.AllowMember("触发器.函数.删除计时器", CSUtility.Helper.enCSType.Common, "从触发器删除计时器")]
        [CSUtility.Event.Attribute.AllowMember("触发器.函数.删除计时器", CSUtility.Helper.enCSType.Common, "从触发器删除计时器")]
        public void RemoveProcessDataLogicTimer(string name, TriggerProcessData processData)
        {
            processData.RemoveLogicTimer(name);
        }

        #endregion

    }

}
