using System;
using System.Collections.Generic;

namespace CSUtility.Performance
{
    /// <summary>
    /// 性能计数器
    /// </summary>
    public class PerfCounter
    {
        /// <summary>
        /// 性能计数器对象指针
        /// </summary>
        protected IntPtr mInner = IntPtr.Zero;
        /// <summary>
        /// 性能计数器对象指针
        /// </summary>
        public IntPtr Inner
        {
            get { return mInner; }
            set
            {
                mInner = value;
            }
        }
        Int64 mBeginTime;
        /// <summary>
        /// 根据名称查找性能计数器
        /// </summary>
        /// <param name="name">名称</param>
        public PerfCounter(System.String name)
	    {
            mInner = DllImportAPI.v3dSampMgr_FindSamp(name);
        }
	    PerfCounter(IntPtr inner)
	    { 
            mInner = inner;
	    }
        /// <summary>
        /// 开启性能计数器
        /// </summary>
        public void Begin()
	    {
            mBeginTime = DllImportAPI.v3dSampMgr_Begin(mInner);
	    }
        /// <summary>
        /// 终止性能计数器
        /// </summary>
        public void End()
	    {
		    DllImportAPI.v3dSampMgr_End(mBeginTime,mInner);
	    }
        /// <summary>
        /// 只读属性，性能信息
        /// </summary>
        public string Infomation
        {
            get
            {
                return string.Format("{0}:Counter[{1}]Hit[{2}]Time[{3}]",
                    "",
                    AvgCounter.ToString("D4"),
                    AvgHit.ToString("D4"),
                    AvgTime.ToString("D4")
                    );
            }
        }
        /// <summary>
        /// 是否开启性能计数器
        /// </summary>
        public bool Enable
        {
            get
            {
                unsafe
                {
                    return CSUtility.DllImportAPI.v3dSamp_GetEnable(mInner)==0?false:true;
                }
            }
            set
            {
                unsafe
                {
                    CSUtility.DllImportAPI.v3dSamp_SetEnable(mInner, value ? 1 : 0);
                }
            }
        }
        /// <summary>
        /// 性能均值
        /// </summary>
        public int AvgCounter
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dSamp_GetAvgCounter(mInner);
                }
            }
            set
            {
                unsafe
                {
                    DllImportAPI.v3dSamp_SetAvgCounter(mInner, value);
                }
            }
        }
        /// <summary>
        /// 平均点击次数
        /// </summary>
        public int AvgHit
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dSamp_GetAvgHit(mInner);
                }
            }
        }
        /// <summary>
        /// 平均占用时间
        /// </summary>
        public Int64 AvgTime
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dSamp_GetAvgTime(mInner);
                }
            }
        }
        /// <summary>
        /// 占用的最大时间
        /// </summary>
        public Int64 MaxTimeInCounter
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dSamp_GetMaxTimeInCounter(mInner);
                }
            }
        }
        /// <summary>
        /// 只读属性，性能计数器的名称
        /// </summary>
        public string Name
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dSamp_GetName(mInner);
                }
            }
        }
        /// <summary>
        /// 只读属性，父类名称
        /// </summary>
        public string Parent
        {
            get
            {
                unsafe
                {
                    return DllImportAPI.v3dSamp_GetParentName(mInner);
                }
            }
        }
        /// <summary>
        /// 获取父类信息
        /// </summary>
        /// <returns>返回父类信息列表</returns>
        public List<System.String> GetParentsInfo()
	    {
            List<System.String> result = new List<System.String>();
            unsafe
            {
                int strCount = 0;
                void** strArray = DllImportAPI.v3dSamp_GetParentsInfo(mInner, &strCount);

                for (int i = 0; i < strCount; ++i)
                {
                    var str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi((IntPtr)(strArray[i]));
                    result.Add(str);
                }
                DllImportAPI.v3dSamp_DeleteStrings(strArray, strCount);
            }

            return result;
        }
        /// <summary>
        /// 获取所有的父类名称
        /// </summary>
        /// <returns>返回所有的父类名称列表</returns>
        public List<System.String> GetParentsName()
        {
            List<System.String> result = new List<System.String>();
            unsafe
            {
                int strCount = 0;
                void** strArray = DllImportAPI.v3dSamp_GetParentsName(mInner, &strCount);

                for (int i = 0; i < strCount; ++i)
                {
                    var str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi((IntPtr)(strArray[i]));
                    result.Add(str);
                }
                DllImportAPI.v3dSamp_DeleteStrings(strArray, strCount);
            }

            return result;
        }
        /// <summary>
        /// 获取父类引用
        /// </summary>
        /// <param name="name">子类名称</param>
        /// <returns>返回引用次数</returns>
        public float GetParentRate(string name)
        {
            return DllImportAPI.v3dSamp_GetParentRate(mInner, name);
        }
        /// <summary>
        /// 获取所有的引用计数次数
        /// </summary>
        /// <returns>返回所有的引用计数</returns>
        public static int GetAllPerfCounterNumber()
	    {
            return DllImportAPI.v3dSampMgr_GetSampSize();
	    }
        /// <summary>
        /// 获取所有的引用计数器
        /// </summary>
        /// <returns>返回所有的引用计数器列表</returns>
	    public static List<PerfCounter> GetAllPerfCounter()
	    {
            List<PerfCounter> result = new List<PerfCounter>();
            unsafe
            {
                int sampCount = 0;
                void** ptrArray = DllImportAPI.v3dSampMgr_GetSamps(&sampCount);

                if (sampCount == 0)
                    return null;


                for (int i = 0; i < sampCount; ++i)
                {
                    result.Add(new PerfCounter((IntPtr)(ptrArray[i])));
                }
                DllImportAPI.v3dSampMgr_DeleteSampsPtr(ptrArray);
            }

            return result;
	    }

    }
}
