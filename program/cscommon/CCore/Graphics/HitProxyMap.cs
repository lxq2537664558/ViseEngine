using System;
using System.Collections.Generic;

namespace CCore.Graphics
{
    /// <summary>
    /// 鼠标点击处理
    /// </summary>
    public class HitProxyMap
    {
        private static HitProxyMap mInstance = new HitProxyMap();
        /// <summary>
        /// 点击代理单例
        /// </summary>
        public static HitProxyMap Instance
        {
            get { return mInstance; }
        }
        /// <summary>
        /// 保存当前的地图
        /// </summary>
        protected Dictionary<UInt32, Guid> mMaps;
        /// <summary>
        /// 点击的代理值
        /// </summary>
        protected UInt32 mHitProxy;
        /// <summary>
        /// 点击代理的构造函数，将代理值设置为1
        /// </summary>
        public HitProxyMap()
        {
            mMaps = new Dictionary<uint, Guid>();
            mHitProxy = 1;
        }
        /// <summary>
        /// 根据Actor的ID创建Actor的点击代理值
        /// </summary>
        /// <param name="id">需要创建点击代理的ActorID</param>
        /// <returns>返回设置好的点击代理值</returns>
        public UInt32 GenHitProxy(Guid id)
        {
            lock (this)
            {
                mHitProxy++;

                if (mHitProxy == UInt32.MaxValue)
                    System.Diagnostics.Debug.WriteLine("HitProxyMap.GenHitProxy---Warning: HitProxy out of UInt32 range");

                mMaps.Add(mHitProxy, id);
                return mHitProxy;
            }
        }
        /// <summary>
        /// 根据点击代理值得到Actor的ID
        /// </summary>
        /// <param name="hitProxy">点击代理值</param>
        /// <returns>返回Actor的ID</returns>
        public Guid GetActorId(UInt32 hitProxy)
        {
            Guid id;
            if (mMaps.TryGetValue(hitProxy, out id))
                return id;
            return Guid.Empty;
        }
    }
}
