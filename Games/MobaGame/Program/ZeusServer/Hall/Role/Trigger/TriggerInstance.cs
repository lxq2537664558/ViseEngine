using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall.Role.Trigger
{
    public class TriggerProcessData_Server : CSUtility.Map.Trigger.TriggerProcessData
    {
        public TriggerProcessData_Server(RoleActor role)
        {            
            Actor = role;
            Id = role.Id;
        }
        public override Guid Id
        {
            get;
            set;
        } = Guid.NewGuid();

        [CSUtility.Event.Attribute.AllowMember("触发器.Timer出发次数", CSUtility.Helper.enCSType.Server, "Timer出发次数")]
        public int OnTimerTimes { get; set; }

        [CSUtility.Event.Attribute.AllowMember("触发器.所属地图", CSUtility.Helper.enCSType.Server, "触发器所属地图")]
        public Map.MapInstance HostMap
        {
            get;
            set;
        }
    }

    public class TriggerInstance : Role.RoleActor
    {
        private CSUtility.Map.Trigger.TriggerData mTriggerData;
        public CSUtility.Map.Trigger.TriggerData TriggerData
        {
            get { return mTriggerData; }
        }

        public override Guid Id
        {
            get
            {
                if (TriggerData != null)
                    return TriggerData.Id;

                return Guid.Empty;
            }
        }

        public override string RoleName
        {
            get
            {
                return TriggerData.Name;
            }
        }

        public bool InitializeTrigger(CSUtility.Map.Trigger.TriggerData td)
        {
            if (td == null)
                return false;
            mTriggerData = td;
            mPlacement = new Role.RolePlacement(this);
            mGravity = null;

            var transMat = td.TransMatrix;
            mPlacement.SetMatrix(ref transMat);

            return true;
        }

        [CSUtility.Event.Attribute.AllowMember("触发器.位置", CSUtility.Helper.enCSType.Server, "触发器位置")]
        public override SlimDX.Vector3 Location
        {
            get { return this.Placement.GetLocation(); }
        }

        TriggerProcessData_Server mTriggerProcessData;
        #region 创建
        public static TriggerInstance CreateTriggerInstance(CSUtility.Map.Trigger.TriggerData td, HallInstance hall, Map.MapInstance map)
        {
            if (td == null)
                return null;

            TriggerInstance ret = new TriggerInstance();
            ret.RoleCreateType = GameData.Role.ERoleType.Trigger;
            ret.HallInstance = hall;
            ret.InitializeTrigger(td);

            CSUtility.Component.ActorInitBase actInit = new CSUtility.Component.ActorInitBase();
            actInit.GameType = (UInt16)CSUtility.Component.EActorGameType.Trigger;
            ret.Initialize(actInit);

            var matrix = td.TransMatrix;
            ret.Placement.SetMatrix(ref matrix);

            ret.OnEnterMap(map);

            ret.mTriggerProcessData = new TriggerProcessData_Server(ret);
            ret.mTriggerProcessData.HostMap = map;

            ret.mProcessDatas.Add(ret.mTriggerProcessData.Id, ret.mTriggerProcessData);

            var loc = ret.Placement.GetLocation();
            loc.Y = 0;
            ret.Placement.SetLocation(loc);

            try
            {
                if (td.OnCreateCB != null)
                {
                    var callee = td.OnCreateCB.GetCallee() as CSUtility.Map.Trigger.FOnCreate;
                    callee?.Invoke(ret.mTriggerProcessData, td);
                }
            }
            catch (Exception)
            {
            }
            return ret;
        }

        #endregion

        #region 重载
        public override void OnEnterMap(Map.MapInstance map)
        {
            HostMap = map;
            var scale = this.Placement.GetScale();
            SlimDX.Vector3 vOriMin = new SlimDX.Vector3(-scale.X * 0.5f, -scale.Y * 0.5f, -scale.Z * 0.5f);
            SlimDX.Vector3 vOriMax = new SlimDX.Vector3(scale.X * 0.5f, scale.Y * 0.5f, scale.Z * 0.5f);

            SlimDX.Vector3[] mTempVecArray = new SlimDX.Vector3[8];
            mTempVecArray[0] = new SlimDX.Vector3(vOriMin.X, vOriMax.Y, vOriMax.Z);
            mTempVecArray[1] = vOriMax;
            mTempVecArray[2] = new SlimDX.Vector3(vOriMax.X, vOriMax.Y, vOriMin.Z);
            mTempVecArray[3] = new SlimDX.Vector3(vOriMin.X, vOriMax.Y, vOriMin.Z);
            mTempVecArray[4] = new SlimDX.Vector3(vOriMin.X, vOriMin.Y, vOriMax.Z);
            mTempVecArray[5] = new SlimDX.Vector3(vOriMax.X, vOriMin.Y, vOriMax.Z);
            mTempVecArray[6] = new SlimDX.Vector3(vOriMax.X, vOriMin.Y, vOriMin.Z);
            mTempVecArray[7] = vOriMin;

            SlimDX.Matrix absMatrix;
            Placement.GetAbsMatrix(out absMatrix);

            for (int i = 0; i < 8; i++)
            {
                mTempVecArray[i] = SlimDX.Vector3.TransformCoordinate(mTempVecArray[i], absMatrix);
            }

            var vMin = mTempVecArray[0];
            var vMax = mTempVecArray[0];

            for (int i = 0; i < 8; i++)
            {
                if (vMin.X > mTempVecArray[i].X)
                    vMin.X = mTempVecArray[i].X;
                if (vMin.Y > mTempVecArray[i].Y)
                    vMin.Y = mTempVecArray[i].Y;
                if (vMin.Z > mTempVecArray[i].Z)
                    vMin.Z = mTempVecArray[i].Z;

                if (vMax.X < mTempVecArray[i].X)
                    vMax.X = mTempVecArray[i].X;
                if (vMax.Y < mTempVecArray[i].Y)
                    vMax.Y = mTempVecArray[i].Y;
                if (vMax.Z < mTempVecArray[i].Z)
                    vMax.Z = mTempVecArray[i].Z;
            }

            var mapCells = HostMap.GetMapCell(vMin.X, vMin.Z, vMax.X, vMax.Z);

            foreach (var cell in mapCells)
            {
                cell.Enter(this);
            }

            map.AddTrigger(this);
        }

        public override void DangrouseOnLeaveMap()
        {
            if (!HostMap.IsNullMap)
            {
                HostMap.RemoveTrigger(this);
            }

            base.DangrouseOnLeaveMap();
        }
        
        // delta为检测范围调整阈值
        public void ProcessActorEnter(Role.RoleActor actor, float delta = 1.0f)
        {
            if (TriggerData == null)
                return;

            if (!TriggerData.Enable)
                return;

            if (TriggerData.ProcessType == CSUtility.Map.Trigger.TriggerData.enProcessType.Client)
                return;

            if (actor == null)
                return;

            TriggerProcessData_Server tData = new TriggerProcessData_Server(actor);
            tData.HostMap = this.HostMap;

            if (mProcessDatas.ContainsKey(tData.Id))
                return;

            if (!tData.CanProcessEnter(TriggerData))
                return;

            tData.EnterTime = CSUtility.DllImportAPI.vfxGetTickCount();

            mProcessDatas.Add(tData.Id, tData);
                      
            // 调用进入回调
            TriggerData.ProcessEnter(tData);
        }

        CSUtility.Support.AsyncObjManager<Guid, CSUtility.Map.Trigger.TriggerProcessData> mProcessDatas = new CSUtility.Support.AsyncObjManager<Guid, CSUtility.Map.Trigger.TriggerProcessData>();

        //long mElapsedMillisecond = 0;
        public override void Tick(long elapsedMillisecond)
        {            
            base.Tick(elapsedMillisecond);
            if (TriggerData == null)
                return;

            if (!TriggerData.Enable)
                return;

            if (TriggerData.ProcessType == CSUtility.Map.Trigger.TriggerData.enProcessType.Client)
                return;

            //             mElapsedMillisecond += elapsedMillisecond;
            //             if (mElapsedMillisecond > TriggerData.TickIntervalMillisecond)
            //             {
            //                 mElapsedMillisecond = 0;                
            //             }
            try
            {
                mProcessDatas.BeforeTick();
                mProcessDatas.For_Each((Guid id, CSUtility.Map.Trigger.TriggerProcessData processData, object arg) =>
                {
                    processData.Tick(elapsedMillisecond);

                    if (processData.CanProcessLeave(TriggerData))
                    {
                        processData.LeaveTime = CSUtility.DllImportAPI.vfxGetTickCount();
                        TriggerData.ProcessLeave(processData);
                        return CSUtility.Support.EForEachResult.FER_Erase;
                    }

                    return CSUtility.Support.EForEachResult.FER_Continue;
                }, null);

                mProcessDatas.AfterTick();

                TriggerData.Tick(elapsedMillisecond);
            }
            catch (Exception)
            {
                
            }                        
        }
        #endregion       
    }
}
