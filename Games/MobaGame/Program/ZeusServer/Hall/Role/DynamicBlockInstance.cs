using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Hall.Role
{
    /// <summary>
    /// 服务器端动态碰撞体，用于处理寻路中动态阻挡的部分（比如门，桥等）
    /// </summary>
    public class DynamicBlockInstance : Role.RoleActor
    {
        ~DynamicBlockInstance()
        {
            DynamicBlockInstanceNumber--;
        }
        #region 重载

        public override void OnEnterMap(Map.MapInstance map)
        {
            base.OnEnterMap(map);
            map.AddDynamicBlock(this);
        }

        #endregion

        private CSUtility.Map.DynamicBlock.DynamicBlockData mDynamicBlockData;
        // 防止直接修改BlockData中的数据，这里不对外暴露DynamicBlockData
        public CSUtility.Map.DynamicBlock.DynamicBlockData DynamicBlockData
        {
            get { return mDynamicBlockData; }
        }

        List<CSUtility.SceneActorPropertyChangeData> mClientActorProperty = new List<CSUtility.SceneActorPropertyChangeData>();

        Role.RoleActor mClientActor = null;
        public Role.RoleActor ClientActor
        {
            get { return mClientActor; }
            set { mClientActor = value; }
        }

        public override Guid Id
        {
            get { return mDynamicBlockData.Id; }
        }
        public void SetBlock(bool block)
        {
            if (mDynamicBlockData == null)
                return;

            if (mDynamicBlockData.IsBlock == block)
                return;

            mDynamicBlockData.IsBlock = block;

            RPC.DataWriter dw = new RPC.DataWriter();
            dw.Write((UInt16)1);
            dw.Write(mDynamicBlockData.Id);
            dw.Write(mDynamicBlockData.IsBlock);

            // 通知寻路服务器动态碰撞体变更
            var pkg = new RPC.PackageWriter();
            H_RPCRoot.smInstance.HGet_PathFindServer(pkg).SetMapBlocks(pkg, this.HallInstance.HallsId, HostMap.MapSourceId, Guid.Empty, dw);
            pkg.DoCommand(HallServer.Instance.PathFindConnect, RPC.CommandTargetType.DefaultType);

            // 广播通知客户端动态碰撞体变更
            foreach (var player in this.HostMap.PlayerPool)
            {
                if (player == null)
                    continue;
                if (block == false)
                {
                    foreach (var pro in mClientActorProperty)
                    {
                        if (pro.ActorId != Guid.Empty)
                            player.SetClientSceneActorProperty(pro.ActorId, pro.PropertyName, pro.TargetValue);//关闭动画
                    }
                    mClientActorProperty.Clear();
                }

                var clPkg = new RPC.PackageWriter();
                CCore.H_CommonRPC.smInstance.RPC_SetSceneActorProperty(clPkg, mDynamicBlockData.Id, "IsBlock", block.ToString());
                clPkg.DoCommandPlanes2Client(player.Planes2GateConnect, player.ClientLinkId);
            }
        }

        public void AddCloseClientActorProperty(Guid id, string propertyName, string targetValue)
        {
            var property = new CSUtility.SceneActorPropertyChangeData();
            property.ActorId = id;
            property.PropertyName = propertyName;
            property.TargetValue = targetValue;
            mClientActorProperty.Add(property);
        }

        public void AddLogicTimerClock(Int64 time)
        {
            TimerManager.AddLogicTimer("DynamicBlockClose", time, this.SetDynamicBlockClose, CSUtility.Helper.enCSType.Server);//添加一个定时器
        }

        bool SetDynamicBlockClose(string time)
        {
            SetBlock(true);
            return false;
        }

        public bool InitializeDynamicBlock(CSUtility.Map.DynamicBlock.DynamicBlockData data)
        {
            if (data == null)
                return false;

            mDynamicBlockData = data;
            mPlacement = new Role.RolePlacement(this);
            mGravity = null;

            return true;
        }

        public static int DynamicBlockInstanceNumber = 0;
        public static DynamicBlockInstance CreateDynamicBlockInstance(CSUtility.Map.DynamicBlock.DynamicBlockData data, HallInstance hall, Map.MapInstance map)
        {
            if (data == null)
                return null;

            DynamicBlockInstance ret = new DynamicBlockInstance();
            ret.RoleCreateType = GameData.Role.ERoleType.DynamicBlock;
            ret.HallInstance = hall;
            ret.InitializeDynamicBlock(data);


            map.RoleManager.DynamicBlockManager.MapRole(ret);
            ret.OnEnterMap(map);

            var transMat = data.TransMatrix;
            ret.Placement.SetMatrix(ref transMat);

            DynamicBlockInstanceNumber++;
            return ret;
        }
    }
}
