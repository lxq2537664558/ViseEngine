using System;

namespace CSUtility.Map.Trigger
{
    public abstract class TriggerProcessDataFactory
    {
        public static TriggerProcessDataFactory Instance;
        public abstract TriggerProcessData CreateTriggerProcessData(Byte processType);
    }
    
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("Trigger")]
    public delegate bool FProcessDataOnTimer(string name, TriggerProcessData processData, TriggerData trigger);
    public abstract class TriggerProcessData
    {
        public Int64 EnterTime;
        public Int64 LeaveTime;

        public abstract Guid Id
        {
            get;
            set;
        }

        CSUtility.Component.ActorBase mActor;
        [CSUtility.Event.Attribute.AllowMember("触发器数据.属性.绑定对象", CSUtility.Helper.enCSType.Common, "")]
        public CSUtility.Component.ActorBase Actor
        {
            get { return mActor; }
            set { mActor = value; }
        }

        public virtual bool CanProcessEnter(TriggerData data)
        {
            if (data == null || Actor == null)
                return false;

            return data.IsPositionIn(Actor.Placement.GetLocation());
        }

        public virtual bool CanProcessLeave(TriggerData data)
        {
            if (data == null || Actor == null)
                return false;

            return !data.IsPositionIn(Actor.Placement.GetLocation());
        }
        
        public void Tick(long elapsedMillisecond)
        {
            mTimerManager.Tick(elapsedMillisecond);
            TickOverride(elapsedMillisecond);
        }
        public virtual void TickOverride(long elapsedMillisecond)
        {

        }

        #region LogicTimer


        CSUtility.Support.ConcurentObjManager<string, TriggerData> mProcessDataMap = new Support.ConcurentObjManager<string, TriggerData>();
        Helper.LogicTimerManager mTimerManager { get; } = new Helper.LogicTimerManager();
        public void AddLogicTimer(string name, Int64 interval, TriggerData data)
        {
            mProcessDataMap.Add(name, data);
            mTimerManager.AddLogicTimer(name, interval, ProcessData_OnTimer, CSUtility.Helper.enCSType.Common);
        }
        public void RemoveLogicTimer(string name)
        {
            mProcessDataMap.Remove(name);
            mTimerManager.RemoveLogicTimer(name);
        }

        private bool ProcessData_OnTimer(string name)
        {
            var triggerData = mProcessDataMap.FindObj(name);
            var callee = triggerData?.ProcessDataOnTimerCB?.GetCallee() as CSUtility.Map.Trigger.FProcessDataOnTimer;
            return (bool)callee?.Invoke(name, this, triggerData);            
        }

        #endregion
    }
}
