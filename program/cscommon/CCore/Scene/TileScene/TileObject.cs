using System;
using System.Collections.Generic;

namespace CCore.Scene.TileScene
{
    /// <summary>
    /// 声明在块状物体上释放Actor时调用的委托事件
    /// </summary>
    /// <param name="actor">要释放的Actor指针</param>
    public delegate void Delegate_TileObject_ReleaseActor(IntPtr actor);
    /// <summary>
    /// 声明在块状物体上删除Actor时调用的委托事件
    /// </summary>
    /// <param name="actor">要删除的Actor指针</param>
    public delegate void Delegate_TileObject_Cleanup(IntPtr actor);
    /// <summary>
    /// 声明预览对象时调用的委托事件
    /// </summary>
    /// <param name="actor">要预览的Actor指针</param>
    /// <param name="bForce">是否强制从磁盘加载</param>
    /// <param name="time">预览的时间</param>
    public delegate void Delegate_TileObject_PreUse(IntPtr actor, bool bForce, UInt64 time);
    /// <summary>
    /// 声明获取物体的AABB包围盒时调用的委托事件
    /// </summary>
    /// <param name="actor">需要创建AABB包围盒的Actor指针</param>
    /// <param name="vMax">AABB包围盒的最大顶点指针</param>
    /// <param name="vMin">AABB包围盒的最小顶点指针</param>
    public delegate void Delegate_TileObject_GetAABB(IntPtr actor, IntPtr vMax, IntPtr vMin);
    /// <summary>
    /// 声明获取物体的当前坐标时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <param name="vec">vector对象指针</param>
    public delegate void Delegate_TileObject_GetLocation(IntPtr actor, IntPtr vec);
    /// <summary>
    /// 声明获取Actor本地坐标时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <param name="vec">vector对象的指针</param>
    public delegate void Delegate_TileObject_GetOriginLocation(IntPtr actor, IntPtr vec);
    /// <summary>
    /// 声明网格线检查时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <param name="startVec">线的起始点坐标指针</param>
    /// <param name="endVec">线结束点坐标的指针</param>
    /// <param name="hitResult">点击结果的代理指针</param>
    /// <param name="param">参数指针</param>
    /// <returns>没有问题返回true，否则返回false</returns>
    public delegate bool Delegate_TileObject_LineCheck(IntPtr actor, IntPtr startVec, IntPtr endVec, IntPtr hitResult, IntPtr param);
    /// <summary>
    /// 声明Actor对象是否包含某一标志的委托事件 
    /// </summary>
    /// <param name="actor">Actor对象的指针</param>
    /// <param name="flag">标志</param>
    /// <returns>如果有该标志返回true，否则返回false</returns>
    public delegate bool Delegate_TileObject_HasFlag(IntPtr actor, int flag);
    /// <summary>
    /// 声明保存Actor对象时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <param name="xndAttrib">XND数据指针</param>
    public delegate void Delegate_TileObject_SaveActor(IntPtr actor, IntPtr xndAttrib);
    /// <summary>
    /// 声明从XND数据加载Actor时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <param name="xndAttrib">XND数据指针</param>
    public delegate void Delegate_TileObject_LoadActor(IntPtr actor, IntPtr xndAttrib);
    /// <summary>
    /// 声明获取类型名称时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <returns>返回获取的类型对象指针</returns>
    public delegate IntPtr Delegate_TileObject_GetTypeName(IntPtr actor);
    /// <summary>
    /// 声明释放Actor类型名称时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <param name="str">名称的指针</param>
    public delegate void Delegate_TileObject_ReleaseTypeName(IntPtr actor, IntPtr str);
    /// <summary>
    /// 声明判断Actor是否为活动类型时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <returns>为活动的Actor时返回true，否则返回false</returns>
    public delegate bool Delegate_TileObject_IsDynamic(IntPtr actor);
    /// <summary>
    /// 声明获取Actor的ID时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象的指针</param>
    /// <param name="guid">Actor对象ID的指针</param>
    /// <returns>得到ID返回true，否则返回false</returns>
    public delegate bool Delegate_TileObject_GetID(IntPtr actor, IntPtr guid);
    /// <summary>
    /// 声明每帧刷新时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <param name="elapsedMillisecondTime">每帧之间的间隔时间</param>
    public delegate void Delegate_TileObject_Tick(IntPtr actor, Int64 elapsedMillisecondTime);
    /// <summary>
    /// 声明获取游戏类型时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <returns>返回Actor的游戏类型</returns>
    public delegate UInt16 Delegate_TileObject_GetGameType(IntPtr actor);
    /// <summary>
    /// 声明获取场景标志时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <returns>返回Actor的标志</returns>
    public delegate byte Delegate_TileObject_GetSceneFlag(IntPtr actor);
    /// <summary>
    /// 声明恢复Actor时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <returns>返回</returns>
    public delegate int Delegate_TileObject_RestoreObjects(IntPtr actor);
    /// <summary>
    /// 声明Actor作废时调用的委托事件
    /// </summary>
    /// <param name="actor">Actor对象指针</param>
    /// <returns>返回</returns>
    public delegate int Delegate_TileObject_InvalidateObjects(IntPtr actor);
    //public delegate int Delegate_TileObject_GetStreamingState(IntPtr actor);
    //public delegate uint Delegate_TileObject_GetResouceSize(IntPtr actor);
    //public delegate Int64 Delegate_TileObject_GetAccessTime(IntPtr actor);
    /// <summary>
    /// 声明创建对象时调用的委托事件
    /// </summary>
    /// <param name="id">对象ID指针</param>
    /// <param name="strName">对象名称指针</param>
    /// <param name="scene">场景指针</param>
    /// <param name="bIsNewObject">是否为新的对象的指针</param>
    /// <returns>返回创建的对象指针</returns>
    public delegate IntPtr Delegate_TileObject_CreateTileObject(IntPtr id, IntPtr strName, IntPtr scene, ref Int32 bIsNewObject);
    /// <summary>
    /// 声明加载对象完成时调用的委托事件
    /// </summary>
    /// <param name="id">对象的ID指针</param>
    /// <param name="actor">Actor对象指针</param>
    public delegate void Delegate_TileObject_LoadFinish(IntPtr id, IntPtr actor);

    /// <summary>
    /// 对象类
    /// </summary>
    public class TileObject
    {
        /// <summary>
        /// 绑定Actor到场景
        /// </summary>
        /// <param name="world">世界对象</param>
        /// <param name="actor">Actor对象</param>
        /// <param name="scene">场景指针</param>
        public static void Bind2SceneGraph(CCore.World.World world, CCore.World.Actor actor,IntPtr scene)
        {
            actor.World = world;
            System.Diagnostics.Debug.Assert(scene != IntPtr.Zero);
            IntPtr pinActor = (IntPtr)(System.Runtime.InteropServices.GCHandle.Alloc(actor));
            DllImportAPI.vTileObject_SetActor(actor.ActorPtr, pinActor);
            DllImportAPI.vTileObject_SetTileScene(actor.ActorPtr, scene);
        }
        /// <summary>
        /// 从场景中移除Actor
        /// </summary>
        /// <param name="actor">Actor对象</param>
        public static void UnBindFromSceneGraph(IntPtr actor)
        {
            unsafe
            {
                var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                if (handle == null)
                    return;
                handle.Target = null;
                handle.Free();
            }
        }
        /// <summary>
        /// 预览Actor对象
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <param name="Force">是否强制从磁盘加载</param>
        /// <param name="time">时间</param>
        public static void PreUse(IntPtr actor, bool Force, UInt64 time)
        {
            unsafe
            {
                var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                var act = handle.Target as CCore.World.Actor;
                if (act != null)
                    act.PreUse(Force, time);
            }
        }
        /// <summary>
        /// 获取AABB包围盒
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <param name="vMax">最大顶点指针</param>
        /// <param name="vMin">最小顶点指针</param>
        public static void GetAABB(IntPtr actor, IntPtr vMax, IntPtr vMin)
        {
            unsafe
            {
                SlimDX.Vector3 maxVec = new SlimDX.Vector3(1,1,1);//*(SlimDX.Vector3*)(vMax.ToPointer());
                SlimDX.Vector3 minVec = new SlimDX.Vector3(-1,-1,-1);//*(SlimDX.Vector3*)(vMin.ToPointer());

                if (actor != IntPtr.Zero)
                {
                    var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                    var act = handle.Target as CCore.World.Actor;
                    if (act != null)
                    {
                        act.GetAABB(ref minVec, ref maxVec);
                    }
                    //else
                    //{
                    //    maxVec.X = 1;
                    //    maxVec.Y = 1;
                    //    maxVec.Z = 1;
                    //    minVec.X = -1;
                    //    minVec.Y = -1;
                    //    minVec.Z = -1;
                    //}
                }

                *(SlimDX.Vector3*)(vMax.ToPointer()) = maxVec;
                *(SlimDX.Vector3*)(vMin.ToPointer()) = minVec;
            }
        }
        /// <summary>
        /// 获取Actor对象的位置
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <param name="vec">位置指针</param>
        public static void GetLocation(IntPtr actor, IntPtr vec)
        {
            unsafe
            {
                SlimDX.Vector3 outVec = new SlimDX.Vector3(0, 0, 0);

                if (actor != IntPtr.Zero)
                {
                    var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                    var act = handle.Target as CCore.World.Actor;
                    if (act != null && act.Placement != null)
                    {
                        outVec = act.Placement.GetLocation();
                    }
                }

                //else
                //{
                //    outVec.X = 0;
                //    outVec.Y = 0;
                //    outVec.Z = 0;
                //}

                *(SlimDX.Vector3*)(vec.ToPointer()) = outVec;
            }
        }
        /// <summary>
        /// 获取Actor的本地坐标
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <param name="vec">位置坐标指针</param>
        public static void GetOriginLocation(IntPtr actor, IntPtr vec)
        {
            unsafe
            {
                SlimDX.Vector3 outVec = new SlimDX.Vector3(0, 0, 0);
                //outVec.X = 0;
                //outVec.Y = 0;
                //outVec.Z = 0;
                *(SlimDX.Vector3*)(vec.ToPointer()) = outVec;
            }
        }
        static CSUtility.Performance.PerfCounter LineCheckTimer = new CSUtility.Performance.PerfCounter("Actor.LineCheck");
        /// <summary>
        /// 网格线检测
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <param name="startVec">起始点坐标指针</param>
        /// <param name="endVec">结束点坐标指针</param>
        /// <param name="hitResult">点击结果指针</param>
        /// <param name="param">消息参数指针</param>
        /// <returns>检测无问题返回true，否则返回false</returns>
        public static bool LineCheck(IntPtr actor, IntPtr startVec, IntPtr endVec, IntPtr hitResult, IntPtr param)
        {
            if (actor == IntPtr.Zero)
                return false;

            unsafe
            {
                LineCheckTimer.Begin();
                var handle = System.Runtime.InteropServices.GCHandle.FromIntPtr(actor);
                //var handle = (System.Runtime.InteropServices.GCHandle)(actor);

                var act = handle.Target as CCore.World.Actor;
                //var act = (CCore.World.Actor)handle.Target;
                if (act == null)
                {
                    LineCheckTimer.End();
                    return false;
                }

                if (param != IntPtr.Zero)
                {
                    var listHandle = (System.Runtime.InteropServices.GCHandle)(param);
                    var list = listHandle.Target as List<CCore.World.Actor>;
                    if (list != null)
                    {
                        if (list.Contains(act))
                        {
                            LineCheckTimer.End();
                            return false;
                        }
                    }
                }

                SlimDX.Vector3* vStart = (SlimDX.Vector3*)(startVec.ToPointer());
                SlimDX.Vector3* vEnd = (SlimDX.Vector3*)(endVec.ToPointer());
                CSUtility.Support.stHitResult* hResult = (CSUtility.Support.stHitResult*)(hitResult.ToPointer());
                if (!act.NeedLineCheck(hResult->mHitFlags))
                {
                    LineCheckTimer.End();
                    return false;
                }
                LineCheckTimer.End();

                var retValue = act.LineCheck(ref (*vStart), ref (*vEnd), ref (*hResult));
                return retValue;
            }
        }
        /// <summary>
        /// Actor对象是否含有指定标志
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <param name="flag">标志</param>
        /// <returns>Actor存在该标志返回true，否则返回false</returns>
        public static bool HasFlag(IntPtr actor, int flag)
        {
            unsafe
            {
                if (actor == IntPtr.Zero)
                    return false;

                var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                var act = handle.Target as CCore.World.Actor;
                if (act != null)
                    return act.HasFlag((CSUtility.Component.ActorInitBase.EActorFlag)flag);

                return false;
            }
        }
        /// <summary>
        /// 保存Actor对象到XND文件
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <param name="xndAttrib">XND数据指针</param>
        public static void SaveActor(IntPtr actor, IntPtr xndAttrib)
        {
            unsafe
            {
                var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                var act = handle.Target as CCore.World.Actor;
                if (act == null)
                    return;

                var att = new CSUtility.Support.XndAttrib(xndAttrib);
                att.BeginWrite();
                act.SaveSceneData(att);
                att.EndWrite();
            }
        }
        /// <summary>
        /// 加载Actor对象
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <param name="xndAttrib">XND数据对象指针</param>
        public static void LoadActor(IntPtr actor, IntPtr xndAttrib)
        {
            unsafe
            {
                var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                var act = handle.Target as CCore.World.Actor;
                if (act == null)
                    return;

                var att = new CSUtility.Support.XndAttrib(xndAttrib);
                att.BeginRead();
                act.LoadSceneData(att);
                att.EndRead();

                if (act.Visual != null)
                    act.Visual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(act.Id));
            }
        }
        /// <summary>
        /// 获取Actor的typename
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <returns>返回Actor的typename的指针</returns>
        public static IntPtr GetTypeName(IntPtr actor)
        {
            unsafe
            {
                string retString = "";

                var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                var act = handle.Target as CCore.World.Actor;
                if (act == null)
                {
                    return System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(retString);
                }

                return System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(CSUtility.Program.GetTypeSaveString(act.GetType()));
            }
        }
        /// <summary>
        /// 释放Actor的typename
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <param name="str">Actor名称指针</param>
        public static void ReleaseTypeName(IntPtr actor, IntPtr str)
        {
            unsafe
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(str);
            }
        }
        //public static bool IsDynamic(IntPtr actor)
        //{
        //    if (actor == IntPtr.Zero)
        //        return false;
        //    unsafe
        //    {
        //        var handle = (System.Runtime.InteropServices.GCHandle)(actor);
        //        var act = handle.Target as CCore.World.Actor;
        //        if (act != null)
        //            return act.IsDynamic;
        //        return false;
        //    }
        //}
        /// <summary>
        /// 获取Actor的ID
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <param name="guid">guid对象指针</param>
        /// <returns>得到Actor的ID返回true，否则返回false</returns>
        public static bool GetID(IntPtr actor, IntPtr guid)
        {
            if (actor == IntPtr.Zero)
                return false;
            unsafe
            {
                var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                var act = handle.Target as CCore.World.Actor;
                if (act == null)
                    return false;

                (*(System.Guid*)guid) = act.Id;

                return true;
            }
        }
        /// <summary>
        /// 获取Actor在游戏中的类型
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <returns>返回Actor在游戏中的类型</returns>
        public static UInt16 GetGameType(IntPtr actor)
        {
            if (actor==IntPtr.Zero)
                return (UInt16)(CSUtility.Component.EActorGameType.Unknow);
            unsafe
            {
                var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                var act = handle.Target as CCore.World.Actor;
                if (act == null)
                    return (UInt16)(CSUtility.Component.EActorGameType.Unknow);

                return (UInt16)act.GameType;
            }
        }
        /// <summary>
        /// 获取场景中Actor的标志
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <returns>返回Actor对象在场景中的标志</returns>
        public static byte GetSceneFlag(IntPtr actor)
        {
            if (actor == IntPtr.Zero)
                return (byte)(CSUtility.Component.enActorSceneFlag.Unknow);
            unsafe
            {
                var handle = (System.Runtime.InteropServices.GCHandle)(actor);
                var act = handle.Target as CCore.World.Actor;
                if (act == null)
                    return (byte)(CSUtility.Component.enActorSceneFlag.Unknow);

                return (byte)act.SceneFlag;
            }
        }
        /// <summary>
        /// 恢复Actor
        /// </summary>
        /// <param name="actor">进行恢复的Actor对象指针</param>
        /// <returns>返回值</returns>
        public static int RestoreObjects(IntPtr actor)
        {
            return 0;
        }
        /// <summary>
        /// 将该Actor作废
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <returns>返回值</returns>
        public static int InvalidateObjects(IntPtr actor)
        {
            return 0;
        }
        /// <summary>
        /// 获取Actor的流状态
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <returns>返回Actor的流状态</returns>
        public static int GetStreamingState(IntPtr actor)
        {
            return (int)(StreamingState.SS_Invalid);
        }
        /// <summary>
        /// 获取资源的大小
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <returns>返回Actor资源的大小</returns>
        public static uint GetResouceSize(IntPtr actor)
        {
            return 0;
        }
        /// <summary>
        /// 获取Access时间
        /// </summary>
        /// <param name="actor">Actor对象指针</param>
        /// <returns>返回Actor的Access时间</returns>
        public static Int64 GetAccessTime(IntPtr actor)
        {
            return 0;
        }

    }

}
