using System;

namespace CSUtility.Support
{
    public class CPUInfo
    {
        public static UInt64 GetProcessorPackageCount()
        {
            return DllImportAPI.CPU_GetProcessorPackageCount();
        }
        public static UInt64 GetLogicalProcessorCount()
        {
            return DllImportAPI.CPU_GetLogicalProcessorCount();
        }
        public static UInt64 GetProcessorCoreCount()
        {
            return DllImportAPI.CPU_GetProcessorCoreCount();
        }
        public static UInt64 GetProcessorL1CacheCount()
        {
            return DllImportAPI.CPU_GetProcessorL1CacheCount();
        }
        public static UInt64 GetProcessorL2CacheCount()
        {
            return DllImportAPI.CPU_GetProcessorL2CacheCount();
        }
        public static UInt64 GetProcessorL3CacheCount()
        {
            return DllImportAPI.CPU_GetProcessorL3CacheCount();
        }
    }
}
