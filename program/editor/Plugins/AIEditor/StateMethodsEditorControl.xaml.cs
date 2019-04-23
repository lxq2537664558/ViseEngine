using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;

namespace AIEditor
{
    /// <summary>
    /// Interaction logic for StateMethodsEditorControl.xaml
    /// </summary>
    public partial class StateMethodsEditorControl : UserControl
    {
        public delegate void Delegate_OnSaveMethodInfo();
        public Delegate_OnSaveMethodInfo OnSaveMethodInfo;

        public enum enMethodDelegateEditType
        {
            Default,
            CurrentState,
            TargetState,
            SelfChange,
        }
        enMethodDelegateEditType mMethodEditType = enMethodDelegateEditType.Default;
        public enMethodDelegateEditType MethodEditType
        {
            get { return mMethodEditType; }
        }

        CSUtility.Helper.enCSType mCSType = CSUtility.Helper.enCSType.Common;
        public CSUtility.Helper.enCSType CSType
        {
            get { return mCSType; }
        }

        string mCurStateType = null;
        public string CurStateType
        {
            get { return mCurStateType; }
        }

        string mTagStateType = null;
        public string TagStateType
        {
            get { return mTagStateType; }
        }

        string mChangeToStateType = null;
        public string ChangeToStateType
        {
            get { return mChangeToStateType; }
        }

        public Type MethodClassType
        {
            get
            {
                switch (MethodEditType)
                {
                    case StateMethodsEditorControl.enMethodDelegateEditType.Default:
                    case StateMethodsEditorControl.enMethodDelegateEditType.CurrentState:
                    case StateMethodsEditorControl.enMethodDelegateEditType.SelfChange:
                        {
                            if (mHostAIInstance != null)
                            {
                                return mHostAIInstance.GetStateBaseType(mCurStateType, mCSType);
                            }
                            //return mCurStateType;
                        }
                        break;

                    case StateMethodsEditorControl.enMethodDelegateEditType.TargetState:
                        {
                            if (mHostAIInstance != null)
                            {
                                return mHostAIInstance.GetStateBaseType(ChangeToStateType, mCSType);
                            }
                            //return ChangeToStateType;
                        }
                        break;
                }

                return null;
            }
        }

        bool mIsDirty = false;
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;
                if (mIsDirty && mHostAIInstance != null)
                    mHostAIInstance.IsDirty = true;
            }
        }

        private AIEditor.FSMTemplateInfo mHostAIInstance = null;
        Guid mHostAIInstanceInfoId = Guid.Empty;
        public Guid HostAIInstanceInfoId
        {
            get { return mHostAIInstanceInfoId; }
            set
            {
                mHostAIInstanceInfoId = value;
                //NodesControl.HostAIInstanceInfoId = mHostAIInstanceInfoId;
                if (mHostAIInstanceInfoId != Guid.Empty)
                    mHostAIInstance = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceInfoId, false);
            }
        }

        public StateMethodsEditorControl()
        {
            InitializeComponent();

            airViewer.TargetNodesContainer = NodesControl;
            NodesList.NodesContainer = NodesControl;
            UsefulNodesList.NodesContainer = NodesControl;
            NodesControl.OnDirtyChanged += NodesControl_OnDirtyChanged;
            NodesControl.OnContainLinkNodesChanged = NodesControl_OnContainLinkNodesChanged;
            NodesControl.OnLinkControlSelected += NodesControl_OnLinkControlSelected;
            NodesControl.OnSelectNodeControl += NodesControl_OnSelectNodeControl;
            NodesControl.OnUnSelectNodes += NodesControl_OnUnSelectNodes;
            NodesControl.ErrorListCtrl = ErrorList;
            //NodesControl.OnCodePreview = NodesControl_OnCodePreview;
            NodesControl.OnSave = NodesControl_OnSave;
        }

        //public delegate void Delegate_PreviewCode();
        //public Delegate_PreviewCode OnPreviewCode;
        //private void NodesControl_OnCodePreview(CodeGenerateSystem.Controls.NodesContainerControl ctrl)
        //{
        //    OnPreviewCode?.Invoke();
        //}

        public delegate void Delegate_Save();
        public Delegate_Save OnSave;
        private void NodesControl_OnSave(CodeGenerateSystem.Controls.NodesContainerControl ctrl)
        {
            this.Save();
        }

        private void NodesControl_OnUnSelectNodes(List<CodeGenerateSystem.Base.BaseNodeControl> nodes)
        {
            ProGrid.Instance = null;
        }

        private void NodesControl_OnSelectNodeControl(CodeGenerateSystem.Base.BaseNodeControl node)
        {
            if (node == null)
                ProGrid.Instance = null;
            else
                ProGrid.Instance = node.GetShowPropertyObject();
        }

        private void NodesControl_OnLinkControlSelected(CodeGenerateSystem.Base.LinkControl linkCtrl)
        {
            UsefulNodesList.ClearNodes();

            if (linkCtrl == null || linkCtrl.HostNodeControl == null)
            {
                //UsefulNodesList.Visibility = Visibility.Collapsed;
                return;
            }

            if (linkCtrl.HostNodeControl is CodeGenerateSystem.Base.UsefulMember)
            {
                var usefulMem = linkCtrl.HostNodeControl as CodeGenerateSystem.Base.UsefulMember;

                foreach (var um in usefulMem.GetUsefulMembers(linkCtrl))
                {
                    CodeDomNode.ClassMemberData data;
                    if (mClassMemberDataDictionary.TryGetValue(um.ClassTypeFullName, out data))
                    {
                        foreach (var property in data.PropertyInfos)
                        {
                            var setMethod = property.GetSetMethod(false);
                            if (setMethod != null && !setMethod.IsStatic)
                                AddPropertyNode(property, UsefulNodesList, true, um);

                            var getMethod = property.GetGetMethod(false);
                            if (getMethod != null && !getMethod.IsStatic)
                                AddPropertyNode(property, UsefulNodesList, false, um);
                        }

                        foreach (var method in data.MethodInfos)
                        {
                            if (method.IsStatic || !method.IsPublic)
                                continue;

                            AddMethodNode(method, UsefulNodesList, um);
                        }
                    }

                    // 继承类处理
                    var types = CSUtility.Program.GetInheritTypesFromType(um.ClassTypeFullName, mCSType);
                    foreach (var type in types)
                    {
                        if (mClassMemberDataDictionary.TryGetValue(type.FullName, out data))
                        {
                            foreach (var property in data.PropertyInfos)
                            {
                                var setMethod = property.GetSetMethod(false);
                                if (setMethod != null && !setMethod.IsStatic)
                                    AddPropertyNode(property, UsefulNodesList, true, um);

                                var getMethod = property.GetGetMethod(false);
                                if (getMethod != null && !getMethod.IsStatic)
                                    AddPropertyNode(property, UsefulNodesList, false, um);
                            }

                            foreach (var method in data.MethodInfos)
                            {
                                if (method.IsStatic || !method.IsPublic)
                                    continue;

                                AddMethodNode(method, UsefulNodesList, um);
                            }
                        }
                    }
                }
            }
        }

        private void NodesControl_OnDirtyChanged(bool dirty)
        {
            if(dirty)
                this.Save();
        }

        private void NodesControl_OnContainLinkNodesChanged(bool bContain, CSUtility.Helper.enCSType csType)
        {
        }

        public void Initialize(string curState, string tagState, string changeToState, enMethodDelegateEditType editType, CSUtility.Helper.enCSType csType)
        {
            mCurStateType = curState;
            mTagStateType = tagState;
            mChangeToStateType = changeToState;
            mMethodEditType = editType;
            mCSType = csType;

            InitMethodList();
            InitializeNodeList();
            InitializeAllowMembers();
        }

        // 根据类全名称记录类成员数据（包含名字空间但不包含assembly信息），如果类名相同但assembly不同也会认为是用一个类，防止共享工程中的类出现重复
        Dictionary<string, CodeDomNode.ClassMemberData> mClassMemberDataDictionary = new Dictionary<string, CodeDomNode.ClassMemberData>();
        private void InitializeNodeList()
        {
            NodesList.AddNodesFromAssembly(this.GetType().Assembly);

            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.数值", "Common,0", "标识所有类型的数值");
            //NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.Boolean", "System.Boolean,0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.SByte", "System.SByte,0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.Int16", "System.Int16,0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.Int32", "System.Int32,0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.Int64", "System.Int64,0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.Byte", "System.Byte,0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.UInt16", "System.UInt16,0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.UInt32", "System.UInt32,0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.UInt64", "System.UInt64,0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.Single", "System.Single,0.0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.Double", "System.Double,0.0", "");
            NodesList.AddNodesFromType(typeof(CodeDomNode.CommonValue), "数值.String", "System.String,", "");

            NodesList.AddNodesFromType(typeof(CodeDomNode.Arithmetic), "运算.＋（加）", "＋", "加法运算节点");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Arithmetic), "运算.－（减）", "－", "减法运算节点");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Arithmetic), "运算.×（乘）", "×", "乘法运算节点");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Arithmetic), "运算.÷（除）", "÷", "除法运算节点");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Arithmetic), "位操作.按位与(&)", "&", "按位与操作节点");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Arithmetic), "位操作.按位或(|)", "|", "按位或操作节点");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Arithmetic), "布尔操作.与(&&)", "&&", "布尔操作与运算节点");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Arithmetic), "布尔操作.或(||)", "||", "布尔操作或运算节点");

            NodesList.AddNodesFromType(typeof(CodeDomNode.Compare), "比较.＞（大于）", "＞", "比较运算节点（大于）");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Compare), "比较.==（等于）", "==", "比较运算节点（等于）");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Compare), "比较.＜（小于）", "＜", "比较运算节点（小于）");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Compare), "比较.≥（大于等于）", "≥", "比较运算节点（大于等于）");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Compare), "比较.≤（小于等于）", "≤", "比较运算节点（小于等于）");
            NodesList.AddNodesFromType(typeof(CodeDomNode.Compare), "比较.≠（不等于）", "≠", "比较运算节点（不等于）");

            NodesList.AddNodesFromType(typeof(CodeDomNode.ClassCastControl), "逻辑.继承类类型转换", mCSType.ToString(), "将类型转换为子类型");
            //NodesList.AddNodesFromType(typeof(CodeDomNode.RefClassMembers), "逻辑.类成员", mCSType.ToString(), "获取类的属性、成员函数");
            NodesList.AddNodesFromType(typeof(CodeDomNode.TypeCastControl), "逻辑.强制类型转换", mCSType.ToString(), "将类型转换为输入的目标类型");
        }

        private void InitializeAllowMembers()
        {
            var assemblys = new List<Assembly>();
            if (mCSType == CSUtility.Helper.enCSType.Common)
                assemblys.AddRange(CSUtility.Program.GetAnalyseAssemblys(CSUtility.Helper.enCSType.All));
            else
                assemblys = new List<Assembly>(CSUtility.Program.GetAnalyseAssemblys(mCSType));

            mClassMemberDataDictionary.Clear();

            foreach (var assembly in assemblys)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsEnum)
                    {
                        var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(type, "CSUtility.AISystem.Attribute.AllowEnum", false);
                        if (att != null)
                        {
                            var path = (string)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Path");
                            var description = (string)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Description");
                            var param = CodeDomNode.EnumValue.GetEnumParam(path, type);
                            NodesList.AddNodesFromType(typeof(CodeDomNode.EnumValue), path, param, description);
                        }
                    }
                    else if (type.IsClass)
                    {
                        var propertys = type.GetProperties();
                        var methods = type.GetMethods();
                        var data = new CodeDomNode.ClassMemberData(type, propertys, methods);
                        mClassMemberDataDictionary[type.FullName] = data;

                        var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(type, "CSUtility.AISystem.Attribute.AllowClass", false);
                        if (att != null)
                        {
                            var classType = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "ClassType");
                            if (classType != null)
                            {
                                if (classType.ToString().Equals("Instance"))
                                {
                                    var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
                                    {
                                        ClassTypeFullName = type.FullName,
                                        HostType = CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Instance,
                                    };

                                    foreach (var proInfo in propertys)
                                    {
                                        var setMethod = proInfo.GetSetMethod(false);
                                        if (setMethod != null && !setMethod.IsStatic)
                                            AddPropertyNode(proInfo, NodesList, true, memberData);

                                        var getMethod = proInfo.GetGetMethod(false);
                                        if (getMethod != null && !getMethod.IsStatic)
                                            AddPropertyNode(proInfo, NodesList, false, memberData);
                                    }
                                    foreach (var methodInfo in methods)
                                    {
                                        if (methodInfo.IsStatic || !methodInfo.IsPublic)
                                            continue;

                                        AddMethodNode(methodInfo, NodesList, memberData);
                                    }
                                }
                                else if (classType.ToString().Equals("New"))
                                {

                                }
                            }
                        }

                        // 将静态属性加入节点列表中
                        foreach (var proInfo in propertys)
                        {
                            var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
                            {
                                ClassTypeFullName = type.FullName,
                                HostType = CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Static,
                            };
                            var setMethod = proInfo.GetSetMethod(false);
                            if (setMethod != null && setMethod.IsStatic)
                                AddPropertyNode(proInfo, NodesList, true, memberData);

                            var getMethod = proInfo.GetGetMethod(false);
                            if (getMethod != null && getMethod.IsStatic)
                                AddPropertyNode(proInfo, NodesList, false, memberData);

                        }
                        // 将静态函数加入节点列表中
                        foreach (var methodInfo in methods)
                        {
                            if (!methodInfo.IsStatic || !methodInfo.IsPublic)
                                continue;

                            var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
                            {
                                ClassTypeFullName = type.FullName,
                                HostType = CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Static,
                            };
                            AddMethodNode(methodInfo, NodesList, memberData);
                        }
                    }
                }
            }
            
            // 计算类的所有子类
            foreach(var data in mClassMemberDataDictionary.Values)
            {
                CodeDomNode.ClassMemberData.CalculateInheritanceClasses(data.ClassType, assemblys, mClassMemberDataDictionary);
            }
        }

        // 添加属性节点
        private void AddPropertyNode(PropertyInfo propertyInfo, CodeGenerateSystem.Controls.NodeListControl hostNodesList, bool bSet, CodeGenerateSystem.Base.UsefulMemberHostData usefulMemberData = null)
        {
            var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(propertyInfo, "CSUtility.AISystem.Attribute.AllowMember", false);
            if (att == null)
                return;

            var csType = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "CSType").ToString();
            if (csType != "Common" && csType != mCSType.ToString())
                return;

            var path = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Path").ToString();
            var description = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Description").ToString();

            var refTypeStr = CSUtility.Program.GetAppTypeString(propertyInfo.ReflectedType);
            var infoString = CodeDomNode.PropertyNode.GetParamInPropertyInfo(propertyInfo, bSet, path);
            var tempStr = CodeDomNode.PropertyNode.GetParamPreInfo(bSet);
            if (!string.IsNullOrEmpty(infoString))
            {
                if (usefulMemberData != null)
                {
                    var usefulMemberStr = usefulMemberData.ToString().Replace(usefulMemberData.ClassTypeFullName, refTypeStr);
                    infoString += ";" + usefulMemberStr;
                }
                path += "(" + refTypeStr + ")";
                hostNodesList.AddNodesFromType(typeof(CodeDomNode.PropertyNode), path + "(" + tempStr + ")", infoString, description);
            }
        }

        // 添加函数节点
        private void AddMethodNode(MethodInfo methodInfo, CodeGenerateSystem.Controls.NodeListControl hostNodesList, CodeGenerateSystem.Base.UsefulMemberHostData usefulMemberData = null)
        {
            var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(methodInfo, "CSUtility.AISystem.Attribute.AllowMember", false);
            if (att == null)
                return;

            var csType = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "CSType").ToString();
            if (csType != "Common" && csType != mCSType.ToString())
                return;

            var path = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Path").ToString();
            var description = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Description").ToString();

            var refTypeStr = CSUtility.Program.GetAppTypeString(methodInfo.ReflectedType);
            var methodInfoString = CodeDomNode.MethodInvokeNode.GetParamFromMethodInfo(methodInfo, path);
            methodInfoString += "," + this.CSType.ToString();
            if (usefulMemberData != null)
            {
                var usefulMemberStr = usefulMemberData.ToString().Replace(usefulMemberData.ClassTypeFullName, refTypeStr);
                methodInfoString += ";" + usefulMemberStr;
            }
            path += "(" + refTypeStr + ")";
            hostNodesList.AddNodesFromType(typeof(CodeDomNode.MethodInvokeNode), path, methodInfoString, description);
        }

        public void InitStatePropertyNodesList(List<AIEditor.FSMTemplateInfo.StatePropertyInfo> propertyInfos)
        {
            foreach(var proInfo in propertyInfos)
            {
                NodesList.AddNodesFromType(typeof(LinkSystem.Value.Value_StateProperty), "参数.类成员变量." + proInfo.PropertyName, proInfo.PropertyType.ToString() + "," + proInfo.PropertyName, "");
            }
        }

        private void SetStatePropertyInNodesList(Type stateType, CSUtility.Helper.enCSType csType)
        {
            var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
            {
                ClassTypeFullName = stateType.FullName,
                HostType = CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.This,
            };

            foreach (var property in stateType.GetProperties())
            {
                var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(property, "CSUtility.AISystem.Attribute.AllowMember", false);
                if (att == null)
                    continue;

                var attCSType = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "CSType").ToString();
                switch (csType)
                {
                    case CSUtility.Helper.enCSType.Common:
                        if (!attCSType.Equals("Common"))
                            continue;
                        break;

                    case CSUtility.Helper.enCSType.Server:
                        if (attCSType.Equals("Client"))
                            continue;
                        break;

                    case CSUtility.Helper.enCSType.Client:
                        if (attCSType.Equals("Server"))
                            continue;
                        break;
                }
                
                var setMethod = property.GetSetMethod(false);
                if (setMethod != null && !setMethod.IsStatic)
                    AddPropertyNode(property, NodesList, true, memberData);

                var getMethod = property.GetGetMethod(false);
                if (getMethod != null && !getMethod.IsStatic)
                    AddPropertyNode(property, NodesList, false, memberData);
                //NodesList.AddNodesFromType(typeof(LinkSystem.Value.Value_StateProperty), "参数.类成员变量." + property.Name, property.PropertyType.ToString() + "," + property.Name, "");
            }
        }

        private void SetStateMethodInNodesList(Type stateType, CSUtility.Helper.enCSType csType)
        {
            var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
            {
                ClassTypeFullName = stateType.FullName,
                HostType = CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.This,
            };

            foreach (var method in stateType.GetMethods())
            {
                var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(method, "CSUtility.AISystem.Attribute.AllowMember", false);
                if (att == null)
                    continue;

                var attCSType = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "CSType").ToString();
                switch (csType)
                {
                    case CSUtility.Helper.enCSType.Common:
                        if (!attCSType.Equals("Common"))
                            continue;
                        break;

                    case CSUtility.Helper.enCSType.Server:
                        if (attCSType.Equals("Client"))
                            continue;
                        break;

                    case CSUtility.Helper.enCSType.Client:
                        if (attCSType.Equals("Server"))
                            continue;
                        break;
                }

                var methodName = method.Name + "(";
                foreach (var param in method.GetParameters())
                {
                    methodName += param.ParameterType.Name + " " + param.Name + ",";
                }
                methodName.Remove(methodName.Length - 1);
                methodName += ")";
                AddMethodNode(method, NodesList, memberData);
//                NodesList.AddNodesFromType(typeof(CodeDomNode.MethodInvokeNode), "函数.本状态." + methodName, CodeDomNode.MethodInvokeNode.GetParamFromMethodInfo(method)+";", "");
            }
        }

        void InitMethodList()
        {
            if (MethodClassType == null)
                return;

            string attributeName = null;

            switch (MethodEditType)
            {
                case enMethodDelegateEditType.Default:
                    attributeName = "CSUtility.AISystem.Attribute.OverrideInterface";
                    break;
                case enMethodDelegateEditType.CurrentState:
                    attributeName = "CSUtility.AISystem.Attribute.OverrideInterface";
                    break;
                case enMethodDelegateEditType.TargetState:
                    attributeName = "CSUtility.AISystem.Attribute.OverrideInterface";
                    break;
                case enMethodDelegateEditType.SelfChange:
                    attributeName = "CSUtility.AISystem.Attribute.OverrideInterface";
                    break;
            }

            foreach (var method in MethodClassType.GetMethods())
            {
                var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(method, attributeName, true);
                if (att == null)
                    continue;

                var attCSTypeStr = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "CSType").ToString();
                switch (mCSType)
                {
                    case CSUtility.Helper.enCSType.Common:
                        if (!attCSTypeStr.Equals("Common"))
                            continue;
                        break;

                    case CSUtility.Helper.enCSType.Server:
                        if (attCSTypeStr.Equals("Client"))
                            continue;
                        break;

                    case CSUtility.Helper.enCSType.Client:
                        if (attCSTypeStr.Equals("Server"))
                            continue;
                        break;
                }

                StateMethodEditorListItem item = new StateMethodEditorListItem(HostAIInstanceInfoId, CurStateType, TagStateType, ChangeToStateType, MethodEditType, method, mCSType);
                //item.OnEditBtnClick = new StateMethodEditorListItem.Delegate_OnEditBtnClick(OnStateMethodEditorListItemEditorBtnClick);
                item.OnDirtyChanged = new StateMethodEditorListItem.Delegate_OnDirtyChanged(OnListItemDirtyChanged);
                ListBox_Methods.Items.Add(item);
            }

            NodesControl.Initialize(mCSType);
            SetStateMethodInNodesList(MethodClassType, mCSType);
            SetStatePropertyInNodesList(MethodClassType, mCSType);
        }

        void OnListItemDirtyChanged(bool dirty)
        {
            foreach (StateMethodEditorListItem item in ListBox_Methods.Items)
            {
                if (item.IsDirty)
                {
                    IsDirty = true;
                    return;
                }
            }
        }
        
        public void Save()
        {
            if (ListBox_Methods.SelectedIndex < 0)
                return;

            StateMethodEditorListItem item = ListBox_Methods.SelectedItem as StateMethodEditorListItem;
            Save(item.MethodInfo);
        }

        private void Save(System.Reflection.MethodInfo methodInfo)
        {
            //if (!NSControl.IsDirty)
            //    return;

            //if (!NSControl.ContainLinkNodes)
            //    return;

            if (MethodClassType != null && methodInfo != null && mHostAIInstance != null)
            {
                var strTemp = AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(CurStateType, TagStateType, MethodClassType, methodInfo, CSType);
                CSUtility.Support.XmlHolder methodHolder = null;
                if (!mHostAIInstance.StateMethodDelegateXmlHolders.TryGetValue(strTemp, out methodHolder))
                {
                    if (!NodesControl.ContainLinkNodes)
                    {
                        if (OnSaveMethodInfo != null)
                            OnSaveMethodInfo();
                        return;
                    }
                }
                else
                {
                    if (!NodesControl.ContainLinkNodes)
                    {
                        mHostAIInstance.RemoveMethodDelegate(strTemp);
                        //mHostAIInstance.StateMethodDelegateXmlHolders.Remove(strTemp);

                        if (OnSaveMethodInfo != null)
                            OnSaveMethodInfo();

                        return;
                    }
                }

                methodHolder = CSUtility.Support.XmlHolder.NewXMLHolder("Method", "");
                mHostAIInstance.StateMethodDelegateXmlHolders[strTemp] = methodHolder;

                NodesControl.SaveXML(methodHolder);

            }

            if (OnSaveMethodInfo != null)
                OnSaveMethodInfo();
        }

        private bool Load(System.Reflection.MethodInfo methodInfo)
        {
            if (MethodClassType != null && methodInfo != null && mHostAIInstance != null)
            {
                var strTemp = AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(CurStateType, TagStateType, MethodClassType, methodInfo, mCSType);
                CSUtility.Support.XmlHolder methodHolder = null;
                if (mHostAIInstance.StateMethodDelegateXmlHolders.TryGetValue(strTemp, out methodHolder))
                {
                    NodesControl.LoadXML(methodHolder);
                    return true;
                }
            }

            return false;
        }

        private void ListBox_Methods_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // 将旧的函数连线保存到AIInstanceInfo中
            if (e.RemovedItems.Count > 0)
            {
                StateMethodEditorListItem oldItem = e.RemovedItems[0] as StateMethodEditorListItem;
                Save(oldItem.MethodInfo);
            }

            if (ListBox_Methods.SelectedIndex < 0)
                return;

            StateMethodEditorListItem item = ListBox_Methods.SelectedItem as StateMethodEditorListItem;
            //NSControl.SetMethodParam(item.MethodInfo);
            NodesControl.OnContainLinkNodesChanged = item.OnContainLinkNodesChanged;
            NodesControl.OnDirtyChanged += item.OnLinkControlDirtyChanged;

            if (!Load(item.MethodInfo))
            {
                NodesControl.ClearControlNodes();
                var methodNode = NodesControl.AddOrigionNode(typeof(AIEditor.LinkSystem.MethodNode), AIEditor.LinkSystem.MethodNode.GetParamInMethodInfo(item.MethodInfo) + ",true,false", 0, 0) as AIEditor.LinkSystem.MethodNode;
                methodNode.CurrentState = mCurStateType;
                methodNode.TargetState = mTagStateType;
                methodNode.ChangeToState = mChangeToStateType;
                methodNode.MethodEditType = mMethodEditType;
            }
        }
    }
}
