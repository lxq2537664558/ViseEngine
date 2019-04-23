using System;
using System.Collections.Generic;
using System.Text;

namespace CCore
{
    /// <summary>
    /// 通用客户端远程调用函数
    /// </summary>
    [RPC.RPCClassAttribute(typeof(CommonRPC))]
    public class CommonRPC : RPC.RPCObject
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion

        /// <summary>
        /// 服务器端设置客户端对象属性
        /// </summary>
        /// <param name="actorId">对象ID</param>
        /// <param name="propertyName">对象属性名称</param>
        /// <param name="value">属性值</param>
        /// <param name="fwd">远程调用信息</param>
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_SetSceneActorProperty(Guid actorId, string propertyName, string value, RPC.RPCForwardInfo fwd)
        {
            CCore.Client.MainWorldInstance.QueueSceneActorPropertyChanged(actorId, propertyName, value);
        }
    }
}
