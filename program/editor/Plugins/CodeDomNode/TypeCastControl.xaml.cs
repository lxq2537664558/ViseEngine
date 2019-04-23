using System.Windows;
using System.Windows.Controls;

namespace CodeDomNode
{
    /// <summary>
    /// Interaction logic for TypeCastControl.xaml
    /// </summary>
    public partial class TypeCastControl : CodeGenerateSystem.Base.BaseNodeControl
    {
        CSUtility.Helper.enCSType mCSType = CSUtility.Helper.enCSType.Common;

        string mTargetTypeName = "";
        public string TargetTypeName
        {
            get { return mTargetTypeName; }
            set
            {
                mTargetTypeName = value;
                OnPropertyChanged("TargetTypeName");
            }
        }

        public TypeCastControl(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            mCSType = (CSUtility.Helper.enCSType)System.Enum.Parse(typeof(CSUtility.Helper.enCSType), strParam);

            SetDragObject(RectangleTitle);
            NodeName = "强制类型转换";

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.All, ClassLinkHandle_In, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, ClassLinkHandle_In.BackBrush, false);
            AddLinkObject(CodeGenerateSystem.Base.enLinkType.All, ClassLinkHandle_Out, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ClassLinkHandle_Out.BackBrush, true);
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            base.Save(xmlNode, newGuid, holder);

            xmlNode.AddAttrib("TypeName", TargetTypeName);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            base.Load(xmlNode, deltaX, deltaY);

            var att = xmlNode.FindAttrib("TypeName");
            if (att != null)
                TargetTypeName = att.Value;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == ClassLinkHandle_Out)
            {
                return TargetTypeName;
            }

            return "";
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            if (element == ClassLinkHandle_Out)
            {
                var linkOI = GetLinkObjInfo(ClassLinkHandle_In);
                if (linkOI.HasLink)
                {
                    if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                        linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, true));
                }
            }
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
