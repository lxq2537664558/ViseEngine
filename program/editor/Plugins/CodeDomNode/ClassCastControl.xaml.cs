using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for ClassCastControl.xaml
    /// </summary>
    public partial class ClassCastControl : CodeGenerateSystem.Base.BaseNodeControl
    {
        CSUtility.Helper.enCSType mCSType = CSUtility.Helper.enCSType.Common;

        ObservableCollection<string> mSubClasses = new ObservableCollection<string>();
        public ObservableCollection<string> SubClasses
        {
            get { return mSubClasses; }
            set
            {
                mSubClasses = value;
                OnPropertyChanged("SubClasses");
            }
        }

        public ClassCastControl(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            IsOnlyReturnValue = true;
            mCSType = (CSUtility.Helper.enCSType)System.Enum.Parse(typeof(CSUtility.Helper.enCSType), strParam);
            NodeName = "继承类类型转换";

            SetDragObject(RectangleTitle);

            var linkObjInfo = AddLinkObject(CodeGenerateSystem.Base.enLinkType.Class, ClassLinkHandle_In, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, ClassLinkHandle_In.BackBrush, false);
            linkObjInfo.OnAddLinkInfo += new CodeGenerateSystem.Base.LinkObjInfo.Delegate_OnOperateLinkInfo(ClassLink_In_OnAddLinkInfo);

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Class, ClassLinkHandle_Out, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ClassLinkHandle_Out.BackBrush, true);
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            base.Save(xmlNode, newGuid, holder);

            var subClassesNode = xmlNode.AddNode("SubClasses", "", holder);
            foreach (var sctName in SubClasses)
            {
                var node = subClassesNode.AddNode("SubClass", "", holder);
                node.AddAttrib("Type", sctName);
            }

            subClassesNode.AddAttrib("SelectedIndex", ComboBox_Types.SelectedIndex.ToString());
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            base.Load(xmlNode, deltaX, deltaY);

            SubClasses.Clear();
            var subClassesNode = xmlNode.FindNode("SubClasses");
            if (subClassesNode != null)
            {
                var nodes = subClassesNode.FindNodes("SubClass");
                foreach (var node in nodes)
                {
                    var att = node.FindAttrib("Type");
                    if (att != null)
                    {
                        SubClasses.Add(att.Value);
                    }
                }
            }

            var sAtt = subClassesNode.FindAttrib("SelectedIndex");
            if (sAtt != null)
            {
                ComboBox_Types.SelectedIndex = System.Convert.ToInt32(sAtt.Value);
            }
        }

        public static bool ClassInterfaceFilter(Type typeObj, Object criteriaObj)
        {
            if (typeObj == null || criteriaObj == null)
                return false;

            if (typeObj.ToString() == criteriaObj.ToString())
                return true;
            else
                return false;
        }

        void ClassLink_In_OnAddLinkInfo(CodeGenerateSystem.Base.LinkInfo linkInfo)
        {
            if (!linkInfo.m_linkFromObjectInfo.mIsLoadingLinks && !linkInfo.m_linkToObjectInfo.mIsLoadingLinks)
            {
                SubClasses.Clear();

                string classTypeName = linkInfo.m_linkFromObjectInfo.m_linkObj.GCode_GetValueType(linkInfo.m_linkFromObjectInfo.LinkElement);
                Type classType = CSUtility.Program.GetTypeFromTypeFullName(classTypeName);
                if (classType == null)
                    return;

                // 取得该类的子类
                if (classType.IsInterface)
                {
                    var types = CSUtility.Program.GetTypes(mCSType);// Program.GetTypes(mCSType);
                    foreach (var type in types)
                    {
                        var interFaces = type.GetInterfaces();
                        foreach (var ife in interFaces)
                        {
                            if (ife == classType)
                            {
                                SubClasses.Add(type.FullName);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    var types = CSUtility.Program.GetTypes(mCSType);// Program.GetTypes(mCSType);
                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(classType))
                        {
                            SubClasses.Add(type.FullName);
                        }
                    }
                }

            }
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == ClassLinkHandle_Out)
            {
                if (ComboBox_Types.SelectedIndex < 0)
                {
                    var linkOI = GetLinkObjInfo(ClassLinkHandle_In);
                    if (linkOI.HasLink)
                    {
                        return linkOI.GetLinkObject(0, true).GCode_GetValueType(linkOI.GetLinkElement(0, true));
                    }
                }
                else
                {
                    return SubClasses[ComboBox_Types.SelectedIndex];
                }
            }

            return "";
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            if (element == ClassLinkHandle_Out)
            {
                var linkOI = GetLinkObjInfo(ClassLinkHandle_In);
                if (linkOI.HasLink)
                {
                    return new System.CodeDom.CodeCastExpression(GCode_GetValueType(element),
                                                                 linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true)));
                }


            }

            return null;
        }
    }
}
