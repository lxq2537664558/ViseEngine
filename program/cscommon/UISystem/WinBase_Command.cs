using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UISystem
{
    public partial class WinBase
    {
        public List<string> CommandFromTemplateList = new List<string>();

        protected Dictionary<string, List<string>> mCommandBindingInfoDictionary = new Dictionary<string, List<string>>();
        [Browsable(false)]
        public Dictionary<string, List<string>> CommandBindingInfoDictionary
        {
            get { return mCommandBindingInfoDictionary; }
            set
            {
                if (value == null)
                    return;

                mCommandBindingInfoDictionary.Clear();
                foreach (var cmdBind in value)
                {
                    List<string> list = new List<string>(cmdBind.Value);
                    mCommandBindingInfoDictionary[cmdBind.Key] = list;
                }
            }
        }
        
        public void SetCommandBinding(string eventName, Guid targetId, string methodName)
        {
            List<string> refList;
            if (!mCommandBindingInfoDictionary.TryGetValue(eventName, out refList))
            {
                refList = new List<string>();
                mCommandBindingInfoDictionary[eventName] = refList;
            }

            var value = targetId.ToString() + "." + methodName;
            if(!refList.Contains(value))
                refList.Add(value);
        }

        public void RemoveCommandBinding(string eventName, int index)
        {
            List<string> opList;
            if (!mCommandBindingInfoDictionary.TryGetValue(eventName, out opList))
                return;

            if (index < 0 || index >= opList.Count)
                return;

            opList.RemoveAt(index);
            if (opList.Count == 0)
                mCommandBindingInfoDictionary.Remove(eventName);
        }

        public List<string> GetCommandBindingInfoFromEventName(string eventName)
        {
            List<string> outList;
            if (!mCommandBindingInfoDictionary.TryGetValue(eventName, out outList))
                outList = new List<string>();

            return outList;
        }

        public bool IsTemplateCommand(string eventName, string commandInfo)
        {
            if (CommandFromTemplateList.IndexOf(eventName + "_" + commandInfo) >= 0)
                return true;

            return false;
        }

        //public bool GetBindingCommandInfoFromEventName(string eventName, ref Guid targetId, ref string methodName)
        //{
        //    List<string> outList;
        //    if (!mCommandBindingInfoDictionary.TryGetValue(eventName, out outList))
        //        return false;

        //    return GetBindingCommandInfoFromString(outList, ref targetId, ref methodName);
        //}

        //protected bool GetBindingCommandInfoFromString(List<string> list, ref Guid targetId, ref string methodName)
        //{
        //    if (string.IsNullOrEmpty(strInfo))
        //        return false;

        //    var splits = strInfo.Split('.');
        //    if (splits.Length < 2)
        //        return false;

        //    targetId = Guid.Parse(splits[0]);
        //    methodName = splits[1];

        //    return true;
        //}

        protected virtual void SaveCommandBindInfo(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            var events = this.GetType().GetEvents();
            foreach (var evt in events)
            {
                var attributes = evt.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_CommandEventAttribute), true);
                if (attributes.Length <= 0)
                    continue;

                List<string> commandBindInfos;
                if (!mCommandBindingInfoDictionary.TryGetValue(evt.Name, out commandBindInfos))
                    continue;

                bool bHave = false;
                foreach (var cmdBind in commandBindInfos)
                {
                    //if (CommandFromTemplateList.IndexOf(evt.Name + "_" + cmdBind) >= 0)
                    //    continue;
                    if (IsTemplateCommand(evt.Name, cmdBind))
                        continue;

                    bHave = true;
                }

                if (bHave)
                {
                    var commandBindNode = pXml.AddNode("CommandBinds", "",holder);
                    commandBindNode.AddAttrib("Event", evt.Name);
                    foreach (var cmdBind in commandBindInfos)
                    {
                        var cmdNode = commandBindNode.AddNode("Command", "",holder);
                        cmdNode.AddAttrib("Bind", cmdBind);
                    }
                }
            }
        }

        protected virtual void LoadCommandBindInfo(CSUtility.Support.XmlNode pXml)
        {
            var commandBindNodes = pXml.FindNodes("CommandBinds");
            foreach (var commandBindNode in commandBindNodes)
            {
                var attr = commandBindNode.FindAttrib("Event");
                if(attr == null)
                    continue;

                var evt = this.GetType().GetEvent(attr.Value);
                if(evt == null)
                    continue;

                List<string> commandBindInfos = new List<string>();
                var cmdNodes = commandBindNode.FindNodes("Command");
                foreach (var cmdNode in cmdNodes)
                {
                    var cAttr = cmdNode.FindAttrib("Bind");
                    if (cAttr == null)
                        continue;

                    // 验证
                    var splits = cAttr.Value.Split('.');
                    if (splits.Length < 2)
                        continue;

                    commandBindInfos.Add(cAttr.Value);
                }

                mCommandBindingInfoDictionary[evt.Name] = commandBindInfos;
            }

        //    mCommandBindingInfoDictionary.Clear();
        //    var events = this.GetType().GetEvents();
        //    foreach (var evt in events)
        //    {
        //        var attributes = evt.GetCustomAttributes(typeof(CSUtility.Editor.UIEditor_CommandEventAttribute), true);
        //        if (attributes.Length <= 0)
        //            continue;

        //        var attr = pXml.FindAttrib(evt.Name + ".Command");
        //        if (attr == null)
        //            continue;


        //        string targetName = "", methodName = "";
        //        if (!GetBindingCommandInfoFromString(attr.Value, ref targetName, ref methodName))
        //            continue;

        //        var form = 
        //    }
        }

        // 需要在rootNode全部加载完成后进行操作
        // 绑定命令
        protected virtual void BuildCommandBinding(WinBase rootNode, bool bWithChild = true, bool bExceptTemplateControl = true)
        {
            //if (mCommandBindingInfoDictionary.Count > 0)
            //{
                //var form = GetRoot(typeof(WinForm));

                foreach (var commandBind in mCommandBindingInfoDictionary)
                {
                    var evt = this.GetType().GetEvent(commandBind.Key);
                    var invoke = evt.EventHandlerType.GetMethod("Invoke");
                    var delegateParams = invoke.GetParameters();
                    Type[] handerTypes = new Type[delegateParams.Length];
                    for (int i = 0; i < delegateParams.Length; i++)
                    //foreach (var paramInfo in delegateParams)
                    {
                        handerTypes[i] = delegateParams[i].ParameterType;
                    }

                    foreach (var cmdBind in commandBind.Value)
                    {
                        if (IsTemplateCommand(commandBind.Key, cmdBind))
                            continue;
                        
                        var splits = cmdBind.Split('.');
                        if (splits.Length < 2)
                            continue;

                        var targetId = CSUtility.Support.IHelper.GuidTryParse(splits[0]);
                        var targetControl = rootNode.FindControl(targetId);
                        if (targetControl == null)
                            continue;

                        var methodInfo = targetControl.GetType().GetMethod(splits[1]);
                        if (methodInfo == null)
                            continue;

                        Delegate d = Delegate.CreateDelegate(evt.EventHandlerType, targetControl, methodInfo);
                        evt.AddEventHandler(this, d);
                    }
                }
            //}


            if (bWithChild)
            {
                foreach (var child in ChildWindows)
                {
                    if (child.IsTemplateControl && bExceptTemplateControl)
                        continue;

                    child.BuildCommandBinding(rootNode, bWithChild, bExceptTemplateControl);
                }
            }
        }
    }
}
