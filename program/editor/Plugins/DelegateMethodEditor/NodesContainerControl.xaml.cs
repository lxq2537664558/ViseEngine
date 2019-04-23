using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DelegateMethodEditor
{
    /// <summary>
    /// Interaction logic for NodesContainerControl.xaml
    /// </summary>
    public partial class NodesContainerControl : UserControl
    {
        public delegate void Delegate_OnDirtyChanged(bool dirty);
        public event Delegate_OnDirtyChanged OnDirtyChanged;
        public CodeGenerateSystem.Controls.NodesContainerControl.Delegate_OnContainLinkNodesChanged OnContainLinkNodesChanged;

        CSUtility.Helper.enCSType mCSType = CSUtility.Helper.enCSType.Common;

        public bool ContainLinkNodes
        {
            get
            {
                if (NodesControl != null)
                    return NodesControl.ContainLinkNodes;
                return false;
            }
            set
            {
                if (NodesControl != null)
                    NodesControl.ContainLinkNodes = value;
            }
        }

        public List<CodeGenerateSystem.Base.BaseNodeControl> OrigionNodeControls
        {
            get
            {
                if(NodesControl != null)
                    return NodesControl.OrigionNodeControls;

                return new List<CodeGenerateSystem.Base.BaseNodeControl>();
            }
        }

        public NodesContainerControl()
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
            NodesControl.OnCodePreview = NodesControl_OnCodePreview;
            NodesControl.OnSave = NodesControl_OnSave;
        }

        public delegate void Delegate_PreviewCode();
        public Delegate_PreviewCode OnPreviewCode;
        private void NodesControl_OnCodePreview(CodeGenerateSystem.Controls.NodesContainerControl ctrl)
        {
            OnPreviewCode?.Invoke();
        }
        public delegate void Delegate_Save();
        public Delegate_Save OnSave;
        private void NodesControl_OnSave(CodeGenerateSystem.Controls.NodesContainerControl ctrl)
        {
            OnSave?.Invoke();
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

        // 选中节点操作
        private void NodesControl_OnLinkControlSelected(CodeGenerateSystem.Base.LinkControl linkCtrl)
        {
            UsefulNodesList.ClearNodes();

            if (linkCtrl == null || linkCtrl.HostNodeControl == null)
            {
                //UsefulNodesList.Visibility = Visibility.Collapsed;
                return;
            }

            if(linkCtrl.HostNodeControl is CodeGenerateSystem.Base.UsefulMember)
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
                    foreach(var type in types)
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

        public bool SaveXML(CSUtility.Support.XmlHolder xmlHolder)
        {
            if (NodesControl != null)
                return NodesControl.SaveXML(xmlHolder);
            return false;
        }

        public void LoadXML(CSUtility.Support.XmlHolder xmlHolder)
        {
            NodesControl?.LoadXML(xmlHolder);
        }

        public CodeGenerateSystem.Base.BaseNodeControl AddOrigionNode(Type nodeType, string strParams, double x, double y)
        {
            if (NodesControl != null)
                return NodesControl.AddOrigionNode(nodeType, strParams, x, y);

            return null;
        }

        public void ClearControlNodes()
        {
            NodesControl?.ClearControlNodes();
        }

        private void NodesControl_OnDirtyChanged(bool dirty)
        {
            OnDirtyChanged?.Invoke(dirty);
        }

        private void NodesControl_OnContainLinkNodesChanged(bool bContain, CSUtility.Helper.enCSType csType)
        {
            OnContainLinkNodesChanged?.Invoke(bContain, csType);
        }

        public void Initialize(CSUtility.Helper.enCSType csType)
        {
            mCSType = csType;

            InitializeNodeList();
            NodesControl.Initialize(csType);
        }

        #region 节点列表


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

            InitializeAllowMembers();
        }

        private void InitializeAllowMembers()
        {
            var assemblys = new List<Assembly>();
            if (mCSType == CSUtility.Helper.enCSType.Common)
                assemblys.AddRange(CSUtility.Program.GetAnalyseAssemblys(CSUtility.Helper.enCSType.All));
            else
                assemblys = new List<Assembly>(CSUtility.Program.GetAnalyseAssemblys(mCSType));

            mClassMemberDataDictionary.Clear();

            foreach(var assembly in assemblys)
            {
                foreach(var type in assembly.GetTypes())
                {
                    if(type.IsEnum)
                    {
                        var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(type, "CSUtility.Event.Attribute.AllowEnum", false);
                        if(att != null)
                        {
                            var path = (string)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Path");
                            var description = (string)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Description");
                            var param = CodeDomNode.EnumValue.GetEnumParam(path, type);
                            NodesList.AddNodesFromType(typeof(CodeDomNode.EnumValue), path, param, description);
                        }
                    }
                    else if(type.IsClass)
                    {
                        var propertys = type.GetProperties();
                        var methods = type.GetMethods();
                        var data = new CodeDomNode.ClassMemberData(type, propertys, methods);
                        mClassMemberDataDictionary[type.FullName] = data;
                    
                        var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(type, "CSUtility.Event.Attribute.AllowClass", false);
                        if(att != null)
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

                                    foreach(var proInfo in propertys)
                                    {
                                        var setMethod = proInfo.GetSetMethod(false);
                                        if (setMethod != null && !setMethod.IsStatic)
                                            AddPropertyNode(proInfo, NodesList, true, memberData);

                                        var getMethod = proInfo.GetGetMethod(false);
                                        if(getMethod != null && !getMethod.IsStatic)
                                            AddPropertyNode(proInfo, NodesList, false, memberData);
                                    }
                                    foreach (var methodInfo in methods)
                                    {
                                        if (methodInfo.IsStatic || !methodInfo.IsPublic)
                                            continue;

                                        AddMethodNode(methodInfo, NodesList, memberData);
                                    }
                                }
                                else if(classType.ToString().Equals("New"))
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
                            if(setMethod != null && setMethod.IsStatic)
                                AddPropertyNode(proInfo, NodesList, true, memberData);

                            var getMethod = proInfo.GetGetMethod(false);
                            if(getMethod != null && getMethod.IsStatic)
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
            foreach (var data in mClassMemberDataDictionary.Values)
            {
                CodeDomNode.ClassMemberData.CalculateInheritanceClasses(data.ClassType, assemblys, mClassMemberDataDictionary);
            }
        }

        // 添加属性节点
        private void AddPropertyNode(PropertyInfo propertyInfo, CodeGenerateSystem.Controls.NodeListControl hostNodesList, bool bSet, CodeGenerateSystem.Base.UsefulMemberHostData usefulMemberData = null)
        {
            var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(propertyInfo, "CSUtility.Event.Attribute.AllowMember", false);
            if (att == null)
                return;

            var csType = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "CSType").ToString();
            if (csType != "Common" && csType != mCSType.ToString())
                return;

            var path = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Path").ToString();
            var description = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Description").ToString();

            var infoString = CodeDomNode.PropertyNode.GetParamInPropertyInfo(propertyInfo, bSet, path);
            var tempStr = CodeDomNode.PropertyNode.GetParamPreInfo(bSet);
            if(!string.IsNullOrEmpty(infoString))
            {
                var refTypeStr = CSUtility.Program.GetAppTypeString(propertyInfo.ReflectedType);
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
            var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(methodInfo, "CSUtility.Event.Attribute.AllowMember", false);
            if (att == null)
                return;

            var csType = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "CSType").ToString();
            if (csType != "Common" && csType != mCSType.ToString())
                return;

            var path = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Path").ToString();
            var description = CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Description").ToString();

            var refTypeStr = CSUtility.Program.GetAppTypeString(methodInfo.ReflectedType);
            var methodInfoString = CodeDomNode.MethodInvokeNode.GetParamFromMethodInfo(methodInfo, path);
            methodInfoString += "," + this.mCSType.ToString();
            if (usefulMemberData != null)
            {
                var usefulMemberStr = usefulMemberData.ToString().Replace(usefulMemberData.ClassTypeFullName, refTypeStr);
                methodInfoString += ";" + usefulMemberStr;
            }
            path += "(" + refTypeStr + ")";
            hostNodesList.AddNodesFromType(typeof(CodeDomNode.MethodInvokeNode), path, methodInfoString, description);
        }

        #endregion

        /*#region 菜单设置

        protected override string GetNodeInitParamFromType(Type type, Type nodeType)
        {
            if (type == typeof(LinkSystem.RefClassMembers) ||
                type == typeof(LinkSystem.ClassCastControl) ||
                type == typeof(LinkSystem.TypeCastControl))
            {
                return mCSType.ToString();
            }

            return "";
        }

        private void AddNodeInMenu(Type type, string menuName, string strParams)
        {
            DelegateMethodEditor.ShowInEditorMenu attr = new DelegateMethodEditor.ShowInEditorMenu(menuName);
            CreateMenu(attr.MenuList, 0, RightButtonMenu.Items);

            stMenuValue menuValue = new stMenuValue();
            menuValue.m_Type = type;
            menuValue.m_Params = strParams;
            m_menuNodeInfos[attr.MenuList[attr.MenuList.Count - 1]] = menuValue;
        }

        private void GetAllOtherMenuNodes()
        {
            //AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.Boolean", "System.Boolean,0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.SByte", "System.SByte,0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.Int16", "System.Int16,0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.Int32", "System.Int32,0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.Int64", "System.Int64,0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.Byte", "System.Byte,0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.UInt16", "System.UInt16,0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.UInt32", "System.UInt32,0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.UInt64", "System.UInt64,0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.Single", "System.Single,0.0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.Double", "System.Double,0.0");
            AddNodeInMenu(typeof(LinkSystem.Value.CommonValue), "参数.数值.String", "System.String,");

            AddNodeInMenu(typeof(LinkSystem.Operation.Arithmetic), "运算.四则运算.＋", "＋");
            AddNodeInMenu(typeof(LinkSystem.Operation.Arithmetic), "运算.四则运算.－", "－");
            AddNodeInMenu(typeof(LinkSystem.Operation.Arithmetic), "运算.四则运算.×", "×");
            AddNodeInMenu(typeof(LinkSystem.Operation.Arithmetic), "运算.四则运算.÷", "÷");

            AddNodeInMenu(typeof(LinkSystem.Operation.Arithmetic), "运算.位操作.按位与(&)", "&");
            AddNodeInMenu(typeof(LinkSystem.Operation.Arithmetic), "运算.位操作.按位或(|)", "|");

            AddNodeInMenu(typeof(LinkSystem.Operation.Arithmetic), "运算.布尔操作.与(&&)", "&&");
            AddNodeInMenu(typeof(LinkSystem.Operation.Arithmetic), "运算.布尔操作.或(||)", "||");

            AddNodeInMenu(typeof(LinkSystem.Operation.Compare), "运算.比较.＞", "＞");
            AddNodeInMenu(typeof(LinkSystem.Operation.Compare), "运算.比较.==", "==");
            AddNodeInMenu(typeof(LinkSystem.Operation.Compare), "运算.比较.＜", "＜");
            AddNodeInMenu(typeof(LinkSystem.Operation.Compare), "运算.比较.≥", "≥");
            AddNodeInMenu(typeof(LinkSystem.Operation.Compare), "运算.比较.≤", "≤");
            AddNodeInMenu(typeof(LinkSystem.Operation.Compare), "运算.比较.≠", "≠");

            switch (mCSType)
            {
                case CSUtility.Helper.enCSType.Client:
                case CSUtility.Helper.enCSType.Server:
                    break;
            }

        }

        #endregion*/

    }
}
