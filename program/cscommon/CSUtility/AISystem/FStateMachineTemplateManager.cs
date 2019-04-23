using System;
using System.Collections.Generic;

namespace CSUtility.AISystem
{
    /// <summary>
    /// 状态机模板管理器
    /// </summary>
    public class FStateMachineTemplateManager
    {
        /// <summary>
        /// 定义编译状态机代码时调用的委托事件
        /// </summary>
        public FStateMachineTemplate.Delegate_OnCompileFSMCode OnCompileFSMCode;
        /// <summary>
        /// 状态机模板管理器对象
        /// </summary>
        protected static FStateMachineTemplateManager mInstance = new FStateMachineTemplateManager();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static FStateMachineTemplateManager Instance
        {
            get { return mInstance; }
        }
        /// <summary>
        /// 状态机的根路径
        /// </summary>
        public string RootDirectory = CSUtility.Support.IFileManager.Instance.Root;

        //////// 记录状态机ID对应的目录
        //////Dictionary<Guid, string> mFStateMachineTemplateDirectoryDictionary = new Dictionary<Guid, string>();
        // 用于记录状态机绑定的Host，以便状态机更新时刷新Host的AI
        
        Dictionary<Guid, FStateMachineTemplate> mFSMs = new Dictionary<Guid, FStateMachineTemplate>();
        Dictionary<string, Type> mParamTypesDictionary = new Dictionary<string, Type>();

        private FStateMachineTemplateManager()
        {
            mParamTypesDictionary["BeAttack"] = typeof(States.IBeAttackParameter);
            mParamTypesDictionary["Death"] = typeof(States.IDeathParameter);
            mParamTypesDictionary["Idle"] = typeof(States.IIdleStateParameter);
            mParamTypesDictionary["LostControl"] = typeof(States.ILostControlParameter);
            mParamTypesDictionary["MoveAttack"] = typeof(States.IMoveAttackParameter);
            mParamTypesDictionary["SpecialAction"] = typeof(States.ISpecialActionParameter);
            mParamTypesDictionary["StayAttack"] = typeof(States.IStayAttackParameter);
            mParamTypesDictionary["StayChannel"] = typeof(States.IStayChannelParameter);
            mParamTypesDictionary["Walk"] = typeof(States.IWalkParameter);
        }
        /// <summary>
        /// 获取相应状态的参数类型
        /// </summary>
        /// <param name="strStateType">状态机的类型</param>
        /// <returns>返回相应状态的参数类型</returns>
        public Type GetParameterType(string strStateType)
        {
            Type outType;
            if (mParamTypesDictionary.TryGetValue(strStateType, out outType))
                return outType;

            return null;
        }
        /// <summary>
        /// 获取状态机模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="csType">客户端服务器类型</param>
        /// <param name="forceLoad">是否强制从磁盘加载</param>
        /// <returns>返回相应的状态机模板</returns>
        public FStateMachineTemplate GetFSMTemplate(Guid templateId, CSUtility.Helper.enCSType csType, bool forceLoad = false)
        {
            if (templateId == Guid.Empty)
                return null;
            FStateMachineTemplate result = null;
            if (mFSMs.TryGetValue(templateId, out result))
            {
                if (forceLoad==true)
                {
                    if (result.Initialize(true))
                        return result;
                    else
                        return null;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                // 判断此FSM是否存在
                if (!CSUtility.Program.FinalRelease)
                {
                    if (!FSMTemplateVersionManager.Instance.FSMTemplateVersionDictionary.ContainsKey(templateId))
                        return null;
                }
                else
                {
                    var dllFile = FStateMachineTemplate.GetAssemblyDir(csType) + "/" + FStateMachineTemplate.GetAssemblyFileName(templateId, csType);
                    if (!System.IO.File.Exists(dllFile))
                        return null;
                }

                result = new FStateMachineTemplate(templateId, csType);
                //result.OnCompileFSMCode = _OnCompileFSMCode;
                if (result.Initialize(false))
                {
                    mFSMs[templateId] = result;
                }
                else
                {
                    return null;
                }
            }

            return result;
        }

//         private bool _OnCompileFSMCode(FStateMachineTemplate fsl, CSUtility.Helper.enCSType csType, bool bForceCompile)
//         {
//             if (OnCompileFSMCode != null)
//                 return OnCompileFSMCode(fsl, csType, bForceCompile);
// 
//             return false;
//         }
        
        //////public string GetFSMTemplateDirectory(Guid id)
        //////{
        //////    string retValue = "";
        //////    if (mFStateMachineTemplateDirectoryDictionary.TryGetValue(id, out retValue))
        //////        return retValue;

        //////    return retValue;
        //////}

        //////public static Guid GetIdFromFolder(string folder)
        //////{
        //////    folder = folder.Replace("\\", "/");
        //////    var idStr = folder.Substring(folder.LastIndexOf("/") + 1);
        //////    Guid retGuid;
        //////    if (System.Guid.TryParse(idStr, out retGuid))
        //////        return retGuid;

        //////    return Guid.Empty;
        //////}

        //////// folder为绝对路径
        //////private FStateMachineTemplate FindFSMTemplate(Guid id, string folder)
        //////{
        //////    foreach (var dir in System.IO.Directory.GetDirectories(folder))
        //////    {
        //////        var dirId = GetIdFromFolder(dir);
        //////        if (dirId == Guid.Empty)
        //////        {
        //////            var fsm = FindFSMTemplate(id, dir);
        //////            if (fsm != null)
        //////                return fsm;
        //////        }
        //////        if (id == dirId)
        //////        {
        //////            var relativeDir = dir.Replace(CSUtility.Support.IFileManager.Instance.Root, "");
        //////            FStateMachineTemplate fsm = new FStateMachineTemplate(id);
        //////            fsm.LoadFSMTemplate(relativeDir);
        //////        }
        //////    }

        //////    return null;
        //////}

        //////// folder为绝对路径,路径全部为"\\"
        //////public FStateMachineTemplate CreateFSMTemplate(string folder)
        //////{
        //////    var id = Guid.NewGuid();
        //////    FStateMachineTemplate fsm = new FStateMachineTemplate(id);
        //////    mFSMs[id] = fsm;
        //////    folder = folder.Replace(CSUtility.Support.IFileManager.Instance.Root, "");
        //////    mFStateMachineTemplateDirectoryDictionary[id] = folder + "\\" + id.ToString();
        //////    return fsm;
        //////}

        //////// folder为相对Release的目录
        //////public void SetFSMTemplateFolder(Guid id, string folder)
        //////{
        //////    mFStateMachineTemplateDirectoryDictionary[id] = folder;
        //////}

        //////public void RemoveFSMTemplate(Guid id, bool bDeleteFile)
        //////{
        //////    FStateMachineTemplate fsm = null;
        //////    if (mFSMs.TryGetValue(id, out fsm) && bDeleteFile)
        //////    {
        //////        var dir = mFStateMachineTemplateDirectoryDictionary[id];
        //////        fsm.DeleteFiles(dir);
        //////    }

        //////    mFSMs.Remove(id);
        //////    mFStateMachineTemplateDirectoryDictionary.Remove(id);
        //////}

        //////public FStateMachineTemplate ForceReloadFSMTemplate(Guid id)
        //////{
        //////    FStateMachineTemplate fsm = GetFSMTemplate(id);
        //////    var dir = mFStateMachineTemplateDirectoryDictionary[id];
        //////    fsm.LoadFSMTemplate(dir);

        //////    return fsm;
        //////}

        //////public void SaveFSMTemplate(Guid id)
        //////{
        //////    FStateMachineTemplate fsm = GetFSMTemplate(id);
        //////    var dir = mFStateMachineTemplateDirectoryDictionary[id];
        //////    fsm.SaveFSMTemplate(dir);

        //////    // 保存时刷新AI
        //////    List<StateHost> mTempHosts = null;
        //////    if (mFSMHostsDictionary.TryGetValue(fsm, out mTempHosts))
        //////    {
        //////        foreach (var host in mTempHosts)
        //////        {
        //////            host.InitFSM(id);
        //////        }
        //////    }
        //////}
    }
}
