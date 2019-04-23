using System;
using System.Collections.Generic;

namespace CSUtility.AISystem.States
{
    /// <summary>
    /// 行走状态参数类
    /// </summary>
    public sealed  class IWalkParameter : StateParameter
    {
        #region 包读写重载
        /// <summary>
        /// 写入包数据
        /// </summary>
        /// <param name="pkg">包</param>
        /// <param name="sType">类型</param>
        public override void PackageWriteFull(RPC.PackageWriter pkg, System.Type sType)
        {
            base.PackageWriteFull(pkg, sType);
            pkg.Write(TargetPositions.Count);
            foreach (var i in TargetPositions)
            {
                pkg.Write(i);
            }
        }
        /// <summary>
        /// 读取包数据
        /// </summary>
        /// <param name="pkg">包</param>
        /// <param name="sType">类型</param>
        public override void PackageReadFull(RPC.PackageProxy pkg, System.Type sType)
        {
            base.PackageReadFull(pkg, sType);
            int Count;
            pkg.Read(out Count);
            TargetPositions.Clear();
            for (int i = 0; i < Count; i++)
            {
                SlimDX.Vector3 pos;
                pkg.Read(out pos);
                TargetPositions.Enqueue(pos);
            }
        }
        /// <summary>
        /// 只写属性的包
        /// </summary>
        /// <param name="pkg">包</param>
        /// <param name="sType">类型</param>
        public override void PackageWriteSingle(RPC.PackageWriter pkg, System.Type sType)
        {
            base.PackageWriteSingle(pkg,sType);
            pkg.Write(TargetPositions.Count);
            foreach (var i in TargetPositions)
            {
                pkg.Write(i);
            }
        }
        /// <summary>
        /// 只读属性的包
        /// </summary>
        /// <param name="pkg">包</param>
        /// <param name="sType">类型</param>
        public override void PackageReadSingle(RPC.PackageProxy pkg, System.Type sType)
        {
            base.PackageReadSingle(pkg,sType);
            int Count;
            pkg.Read(out Count);
            TargetPositions.Clear();
            for (int i = 0; i < Count; i++)
            {
                SlimDX.Vector3 pos;
                pkg.Read(out pos);
                TargetPositions.Enqueue(pos);
            }
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="pkg">包</param>
        /// <param name="sType">类型</param>
        public override void DataWriteFull(RPC.DataWriter pkg, System.Type sType)
        {
            base.DataWriteFull(pkg, sType);
            pkg.Write(TargetPositions.Count);
            foreach (var i in TargetPositions)
            {
                pkg.Write(i);
            }
        }
        /// <summary>
        /// 读取包数据
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="sType"></param>
        public override void DataReadFull(RPC.DataReader pkg, System.Type sType)
        {
            base.DataReadFull(pkg, sType);
            int Count;
            pkg.Read(out Count);
            TargetPositions.Clear();
            for (int i = 0; i < Count; i++)
            {
                SlimDX.Vector3 pos;
                pkg.Read(out pos);
                TargetPositions.Enqueue(pos);
            }
        }
        /// <summary>
        /// 只写数据包
        /// </summary>
        /// <param name="pkg">吸入的数据包</param>
        /// <param name="sType">类型</param>
        public override void DataWriteSingle(RPC.DataWriter pkg, System.Type sType)
        {
            base.DataWriteSingle(pkg, sType);
            pkg.Write(TargetPositions.Count);
            foreach (var i in TargetPositions)
            {
                pkg.Write(i);
            }
        }
        /// <summary>
        /// 只读数据包
        /// </summary>
        /// <param name="pkg">包</param>
        /// <param name="sType">类型</param>
        public override void DataReadSingle(RPC.DataReader pkg, System.Type sType)
        {
            base.DataReadSingle(pkg, sType);
            int Count;
            pkg.Read(out Count);
            TargetPositions.Clear();
            for (int i = 0; i < Count; i++)
            {
                SlimDX.Vector3 pos;
                pkg.Read(out pos);
                TargetPositions.Enqueue(pos);
            }
        }
        #endregion

        Queue<SlimDX.Vector3> mTargetPositions = new Queue<SlimDX.Vector3>();
        /// <summary>
        /// 目标位置
        /// </summary>
        public Queue<SlimDX.Vector3> TargetPositions
        {
            get { return mTargetPositions; }
            set { mTargetPositions = value; }
        }
        /// <summary>
        /// 最终位置
        /// </summary>
        public SlimDX.Vector3 mFinalPosition = SlimDX.Vector3.Zero;
        /// <summary>
        /// 最终位置
        /// </summary>
        public SlimDX.Vector3 FinalPosition
        {
            get { return mFinalPosition; }
            set { mFinalPosition = value; }
        }
        /// <summary>
        /// 目标位置
        /// </summary>
        public SlimDX.Vector3 mTargetPosition = SlimDX.Vector3.Zero;
        /// <summary>
        /// 目标位置
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.行走状态参数.目标位置", CSUtility.Helper.enCSType.Common, "行走状态参数目标位置")]
        [CSUtility.Event.Attribute.AllowMember("AI.行走状态参数.目标位置", CSUtility.Helper.enCSType.Common, "行走状态参数目标位置")]
        public SlimDX.Vector3 TargetPosition
        {
            get { return mTargetPosition; }
            set { mTargetPosition = value; }
        }
        float mMaxCloseDistance = 0.5F;
        /// <summary>
        /// 最近距离
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.行走状态参数.最近距离", CSUtility.Helper.enCSType.Common, "最近距离")]
        [CSUtility.Event.Attribute.AllowMember("AI.行走状态参数.最近距离", CSUtility.Helper.enCSType.Common, "最近距离")]
        public float MaxCloseDistance
        {
            get { return mMaxCloseDistance; }
            set { mMaxCloseDistance = value; }
        }
        //[RPC.FieldDontAutoSaveLoadAttribute()]
        /// <summary>
        /// 目标状态
        /// </summary>
        public IStateHost TargetHost;
        float mMoveSpeed = 1.0f;//缺省速度1米每秒
        /// <summary>
        /// 移动速度
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.行走状态参数.移动速度", CSUtility.Helper.enCSType.Common, "移动速度")]
        [CSUtility.Event.Attribute.AllowMember("AI.行走状态参数.移动速度", CSUtility.Helper.enCSType.Common, "移动速度")]
        public float MoveSpeed
        {
            get { return mMoveSpeed; }
            set { mMoveSpeed = value; }
        }
        float mAccelerate = 0;//默认没有加速度
        /// <summary>
        /// 移动加速度值
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.行走状态参数.移动加速度", CSUtility.Helper.enCSType.Common, "移动加速度")]
        [CSUtility.Event.Attribute.AllowMember("AI.行走状态参数.移动加速度", CSUtility.Helper.enCSType.Common, "移动加速度")]
        public float Accelerate
        {
            get { return mAccelerate; }
            set { mAccelerate = value; }
        }

        SByte mRun = 0;
        /// <summary>
        /// 是否跑步，跑步为1，否则为0
        /// </summary>
        [CSUtility.AISystem.Attribute.AllowMember("AI.行走状态参数.跑步", CSUtility.Helper.enCSType.Common, "")]
        [CSUtility.Event.Attribute.AllowMember("AI.行走状态参数.跑步", CSUtility.Helper.enCSType.Common, "")]
        public SByte Run
        {
            get { return mRun; }
            set { mRun = value; }
        }
    }
    /// <summary>
    /// 行走状态类
    /// </summary>
    [CSUtility.AISystem.Attribute.StatementClass("行走状态", Helper.enCSType.Common)]
    [CSUtility.AISystem.Attribute.ToolTip("行走状态")]
    public class Walk : State
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Walk()
        {
            mStateName = "Walk";
            Parameter = new IWalkParameter();
        }
        /// <summary>
        /// 只读属性，行走状态参数
        /// </summary>
        public IWalkParameter WalkParameter
        {
            get { return (IWalkParameter)Parameter; }
        }

        /// <summary>
        /// 每帧调用
        /// </summary>
        /// <param name="elapsedMillisecond">每帧之间的间隔时间</param>
        public override void Tick(Int64 elapsedMillisecond)
        {
            base.Tick(elapsedMillisecond);
            HP_Tick(elapsedMillisecond,60);
            //             if (WalkParameter.Run == 1 || WalkParameter.ActionPlayRate == 0)
            //             {
            //                 Host.FSMSetAction("Run", true, WalkParameter.ActionPlayRate, 100);
            //             }
            //             else
            //             {
            //                 Host.FSMSetAction("Walk", true, WalkParameter.ActionPlayRate,100);
            //             }

//             var hostPos = Host.Actor.Placement.GetLocation();
//             hostPos.Y = 0;
//             var tagPos = WalkParameter.TargetPosition;
//             tagPos.Y = 0;
//             float dist = SlimDX.Vector3.Distance(hostPos, tagPos);
//             var limitdist = WalkParameter.MaxCloseDistance;
//             if (limitdist == 0)
//                 limitdist = 0.03f;
//             if (limitdist > dist)
//             {
//                 OnArrived();
//             }
//             else
//             {
//                 TickMovement(elapsedMillisecond,60);
//             }
        }
        /// <summary>
        /// HP的每帧调用
        /// </summary>
        /// <param name="elapsedHighPrecision">每帧之间的高精度间隔时间</param>
        /// <param name="frameMillisecond">每个动画帧之间的间隔时间</param>
        public void HP_Tick(Int64 elapsedHighPrecision, Int64 frameMillisecond)
        {
            if (TickMovement(elapsedHighPrecision, frameMillisecond) == false)
            {
                OnArrived();
            }
        }
        SlimDX.Vector3 PreMoveDir = new SlimDX.Vector3(0, 0, 1);
        /// <summary>
        /// 移动时每帧刷新
        /// </summary>
        /// <param name="elapsedHighPrecision">每帧之间的高精度间隔时间</param>
        /// <param name="frameMillisecond">每个动画帧之间的间隔时间</param>
        /// <returns>刷新成功返回true，否则返回false</returns>
        protected virtual bool TickMovement(Int64 elapsedHighPrecision, Int64 frameMillisecond)
        {
            if (Host.Actor == null || Host.Actor.Placement == null)
                return false;
            var curLoc = Host.Actor.Placement.GetLocation();
            var MoveDir = WalkParameter.TargetPosition - curLoc;
            MoveDir.Y = 0;
            MoveDir.Normalize();
            var moveDist = ((float)elapsedHighPrecision) * 0.001f * WalkParameter.MoveSpeed;
            curLoc.Y = WalkParameter.TargetPosition.Y;
            float dist = SlimDX.Vector3.Distance(WalkParameter.TargetPosition, curLoc);
            if (moveDist > dist)
            {
                if (WalkParameter.TargetPositions.Count > 0)
                {
                    WalkParameter.TargetPosition = WalkParameter.TargetPositions.Dequeue();
                    var newPos = curLoc + moveDist * MoveDir;
                    if (Host.Actor.Gravity != null)
                    {
                        newPos.Y = Host.GetAltitude(newPos.X, newPos.Z);
                    }
                    if (IsBlock(newPos))
                        return true;
                    Host.Actor.Placement.SetLocation(ref newPos);
                    Host.Actor.Placement.SetRotationY(MoveDir.Z, MoveDir.X, false);
                    return true;
                }
                else
                {
                    var newPos1 = WalkParameter.TargetPosition;
                    if (Host.Actor.Gravity != null)
                        newPos1.Y = Host.GetAltitude(newPos1.X, newPos1.Z);
                    if (IsBlock(newPos1))
                        return false;
                    Host.Actor.Placement.SetLocation(ref newPos1);
                    return false;
                }
            }
            else
            {
                
                var tarLoc = curLoc + MoveDir * moveDist;
                if (Host.Actor.Gravity != null)
                {
                    tarLoc.Y = Host.GetAltitude(tarLoc.X, tarLoc.Z);
                }
                if (IsBlock(tarLoc))
                    return true;
                Host.Actor.Placement.SetLocation(ref tarLoc);
                Host.Actor.Placement.SetRotationY(MoveDir.Z, MoveDir.X, false);
                return true;
            }
        }
        /// <summary>
        /// 到达指定位置
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnArrived()
        {
            // 判断目标点队列中是否还有值，有的话运动到下一个点
            if (WalkParameter.TargetPositions.Count > 0)
            {
                WalkParameter.TargetPosition = WalkParameter.TargetPositions.Dequeue();

                var curLoc = Host.Actor.Placement.GetLocation();
                var MoveDir = WalkParameter.TargetPosition - curLoc;
                MoveDir.Y = 0;
                MoveDir.Normalize();
                Host.Actor.Placement.SetRotationY(MoveDir.Z, MoveDir.X,  false);
            }
            else
            {
                //var rot = Host.Actor.Placement.GetRotation();
                this.ToState(Host.AIStates.DefaultState.StateName, null);
                //Host.Actor.Placement.SetRotation(ref rot);
            }
        }
        /// <summary>
        /// 召唤物移动
        /// </summary>
        /// <param name="dir">移动方向</param>
        /// <param name="delta">移动距离</param>
        /// <returns>移动成功返回true，否则返回false</returns>
        public virtual bool SommonMove(ref SlimDX.Vector3 dir, float delta)
        {
            var curLoc = Host.Actor.Placement.GetLocation();
            var tarLoc = curLoc + dir * delta;
            if (Host.Actor.Gravity != null)
            {
                tarLoc.Y = Host.GetAltitude(tarLoc.X, tarLoc.Z);
            }
            Host.Actor.Placement.SetLocation(ref tarLoc);
            Host.Actor.Placement.SetRotationY(dir.Z, dir.X, false);
            return true;
        }
        /// <summary>
        /// 尝试到达合适的位置
        /// </summary>
        /// <param name="tar">目标位置</param>
        public virtual void TryFixPosition(ref SlimDX.Vector3 tar)
        {
            float moveDist = SlimDX.Vector3.Distance(tar, Host.Actor.Placement.GetLocation());
            var MoveDir = tar - Host.Actor.Placement.GetLocation();
            Host.Actor.Placement.Move(ref MoveDir, moveDist);
        }
        /// <summary>
        /// 碰撞画面
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnCollideScene()
        {

        }
        /// <summary>
        /// 刷新新的位置点
        /// </summary>
        /// <param name="newPos">新的目标位置</param>
        /// <param name="newDir">新的方向</param>
        /// <param name="moveDist">移动距离</param>
        public void FreshNewPos(ref SlimDX.Vector3 newPos,ref SlimDX.Vector3 newDir,float moveDist)
        {
            if (IsBlock(newPos))
            {
                var curLoc = Host.Actor.Placement.GetLocation();
                var save = ProBlockOther(newDir, moveDist);
                if (IsBlock(save))
                {
                    newPos = curLoc;
                    return;
                }
                newPos = save;
                newDir = save - curLoc;
                newDir.Y = 0;
                newDir.Normalize();
            }
        }
        /// <summary>
        /// 指定位置是否为障碍物
        /// </summary>
        /// <param name="pos">位置坐标</param>
        /// <returns>返回false</returns>
        public virtual bool IsBlock(SlimDX.Vector3 pos)
        {
   
            return false;
        }
        /// <summary>
        /// 让开障碍物
        /// </summary>
        /// <param name="moveDir">移动方向</param>
        /// <param name="length">长度，缺省为1</param>
        /// <returns>返回最终的目标位置</returns>
        public virtual SlimDX.Vector3  ProBlockOther(SlimDX.Vector3 moveDir, float length = 1)
        {
            var curpos = Host.Actor.Placement.GetLocation();
            var dir = SlimDX.Vector3.Transform(moveDir, SlimDX.Matrix.RotationY(0.56f));
            SlimDX.Vector3 newDir = new SlimDX.Vector3(dir.X,0,dir.Z);
            dir.Y = 0;
            newDir.Normalize();
            int i = 1;
            var tarpos = newDir * length + curpos;
            while (IsBlock(tarpos) && i < 12)
            {
                dir = SlimDX.Vector3.Transform(moveDir, SlimDX.Matrix.RotationY(0.56f));
                newDir = new SlimDX.Vector3(dir.X, 0, dir.Z);
                dir.Y = 0;
                newDir.Normalize();
                tarpos = newDir * length + curpos;
                i++;
            }
            moveDir = newDir;
            return tarpos;
        }
        /// <summary>
        /// 碰撞角色
        /// </summary>
        [AISystem.Attribute.OverrideInterface(CSUtility.Helper.enCSType.Common)]
        public virtual void OnCollideRoles()
        {

        }
        /// <summary>
        /// 进入该状态之前
        /// </summary>
        public override void OnPreEnterState()
        {
            base.OnPreEnterState();
          
            if (WalkParameter.Run == 1)
            {
                Host.FSMSetAction("Run", true, Parameter.ActionPlayRate, 100);
            }
            else
            {
                Host.FSMSetAction("Walk", true, Parameter.ActionPlayRate, 100);
            }
            
        }
        /// <summary>
        /// 进入该状态
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();

//             var curLoc = Host.Actor.Placement.GetLocation();
//             var MoveDir = WalkParameter.TargetPosition - curLoc;
//             Host.Actor.Placement.SetRotationY(MoveDir.Z, MoveDir.X,false);
        }
        /// <summary>
        /// 获取目标位置的数量
        /// </summary>
        /// <returns>返回目标位置数量</returns>
        [CSUtility.AISystem.Attribute.AllowMember("AI.行走状态.GetTargetPositionsNum", CSUtility.Helper.enCSType.Common, "")]
        [CSUtility.Event.Attribute.AllowMember("AI.行走状态.GetTargetPositionsNum", CSUtility.Helper.enCSType.Common, "")]
        public int GetTargetPositionsNum()
        {
            if (WalkParameter.TargetPositions == null)
                return 0;
            return WalkParameter.TargetPositions.Count;
        }
        /// <summary>
        /// 重置该状态
        /// </summary>
        public override void OnReEnterState()
        {
            base.OnReEnterState();

            var curLoc = Host.Actor.Placement.GetLocation();
            var MoveDir = WalkParameter.TargetPosition - curLoc;
            MoveDir.Y = 0;
            var len = MoveDir.Length();
            if (len < 0.02F)
                return;
            MoveDir /= len;
            Host.Actor.Placement.SetRotationY(MoveDir.Z, MoveDir.X, false);
        }
        /// <summary>
        /// 离开该状态
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            //System.Diagnostics.Debug.WriteLine(string.Format("TotalMoveDistance:{0}", mTotalMoveDistance));
        }
    }
}
