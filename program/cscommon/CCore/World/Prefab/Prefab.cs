using System;
using System.Collections.Generic;
/// <summary>
/// 预制件的命名空间
/// </summary>
namespace CCore.World.Prefab
{
    /// <summary>
    /// 预制件对象的初始化类
    /// </summary>
    public class PrefabInit : CCore.World.ActorInit
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PrefabInit()
        {
            ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.SaveWithClient;
            GameType = (UInt16)CSUtility.Component.EActorGameType.Prefab;
        }
    }
    /// <summary>
    /// 预制件类
    /// </summary>
    public class Prefab : CCore.World.Actor
    {
        /// <summary>
        /// 预制件Actor的ID列表
        /// </summary>
        protected List<Guid> mActorIdList = new List<Guid>();
        /// <summary>
        /// 只读属性，预制件Actor的ID列表
        /// </summary>
        public List<Guid> ActorIdList
        {
            get { return mActorIdList; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Prefab()
        {
            mId = GenId();
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~Prefab()
        {

        }
        /// <summary>
        /// 清空预制件Actor的ID列表
        /// </summary>
        public override void Cleanup()
        {
            mActorIdList.Clear();
        }
        /// <summary>
        /// 是否包含相应Actor的ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>包含返回true，否则返回false</returns>
        public bool ContainActor(Guid id)
        {
            return mActorIdList.Contains(id);
        }
        /// <summary>
        /// 预制Actor列表组
        /// </summary>
        /// <param name="actors">Actor列表</param>
        /// <returns>返回预制的Actor列表组</returns>
        public static Prefab Group(List<CCore.World.Actor> actors)
        {
            if (actors == null)
                return null;

            if (actors.Count == 0)
                return null;

            var pfInit = new PrefabInit();
            var retValue = new Prefab();
            retValue.Initialize(pfInit);

            foreach (var act in actors)
            {
                retValue.mActorIdList.Add(act.Id);
            }

            return retValue;
        }
        /// <summary>
        /// 清空预制的Actor列表
        /// </summary>
        public void UnGroup()
        {
            Cleanup();
        }
        /// <summary>
        /// 保存场景数据到XND文件
        /// </summary>
        /// <param name="attribute">XND文件</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public override bool SaveSceneData(CSUtility.Support.XndAttrib attribute)
        {
            if (base.SaveSceneData(attribute) == false)
                return false;

            var count = mActorIdList.Count;
            attribute.Write(count);
            foreach (var actId in mActorIdList)
            {
                attribute.Write(actId);
            }

            return true;
        }
        /// <summary>
        /// 加载场景数据
        /// </summary>
        /// <param name="attribute">XND文件</param>
        /// <returns>加载成功返回true，否则返回false</returns>
        public override bool LoadSceneData(CSUtility.Support.XndAttrib attribute)
        {
            Cleanup();

            if (base.LoadSceneData(attribute) == false)
                return false;

            int count;
            attribute.Read(out count);
            for (int i = 0; i < count; i++)
            {
                Guid actId;
                attribute.Read(out actId);
                mActorIdList.Add(actId);
            }

            return true;
        }
        /// <summary>
        /// 获取原始的AABB包围盒
        /// </summary>
        /// <param name="vMin">最小顶点坐标</param>
        /// <param name="vMax">最大顶点坐标</param>
        public override void GetOrigionAABB(ref SlimDX.Vector3 vMin, ref SlimDX.Vector3 vMax)
        {
            base.GetOrigionAABB(ref vMin, ref vMax);
        }
        /// <summary>
        /// 获取层名称
        /// </summary>
        /// <returns>返回该层名称为Prefab</returns>
        public override string GetLayerName()
        {
            return "Prefab";
        }
    }
}
