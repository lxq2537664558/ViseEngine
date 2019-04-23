using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AIEditor.LinkSystem.Value
{
    /// <summary>
    /// Interaction logic for Value_RoleSkill.xaml
    /// </summary>
//    [AIEditor.ShowInAIEditorMenu("参数.RoleSkill")]
    public partial class Value_RoleSkill : CodeGenerateSystem.Base.BaseNodeControl
    {
        System.CodeDom.CodeVariableDeclarationStatement mSkillVarDec = new System.CodeDom.CodeVariableDeclarationStatement();

        Dictionary<FrameworkElement, System.Reflection.PropertyInfo> mPropertyInfoDic = new Dictionary<FrameworkElement, System.Reflection.PropertyInfo>();
        Type mRoleSkillType;

        public Value_RoleSkill(Canvas parentCanvas, string strParam)
            : base(parentCanvas, strParam)
        {
            InitializeComponent();

            mRoleSkillType = Program.GetType("FrameSet.Skill.RoleSkill");
            SetDragObject(RectangleTitle);

            AnalyseProperty();

            AddLinkObject(CodeGenerateSystem.Base.enLinkType.Class, ValueSet, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, null, false);
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            xmlNode.AddAttrib("SkillParamNode", SkillParamName.Text);
            base.Save(xmlNode, newGuid,holder);
        }

        public override void Load(CSUtility.Support.XmlNode xmlNode, double deltaX, double deltaY)
        {
            try
            {
                SkillParamName.Text = xmlNode.FindAttrib("SkillParamNode").Value;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            base.Load(xmlNode, deltaX, deltaY);
        }

        // 分析属性并添加节点
        private void AnalyseProperty()
        {
            if (mRoleSkillType == null)
                return;

            mPropertyInfoDic.Clear();

            var propertyInfos = mRoleSkillType.GetProperties();
            foreach (var info in propertyInfos)
            {
                AddPropertyHandle(info);
            }
        }

        private void AddPropertyHandle(System.Reflection.PropertyInfo info)
        {
            Grid grid = new Grid();
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            TextBlock label = new TextBlock()
            {
                Text = info.Name + "(" + info.PropertyType.Name + ")",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right
            };
            grid.Children.Add(label);

            var rect = new CodeGenerateSystem.Controls.LinkOutControl()
            {
                Margin = new Thickness(0, 0, -13, 0),
                Width = 10,
                Height = 10,
                BackBrush = new SolidColorBrush(Color.FromRgb(243, 146, 243)),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                Direction = CodeGenerateSystem.Base.enBezierType.Right,
            };
            grid.Children.Add(rect);
            AddLinkObject(info.PropertyType, rect, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, rect.BackBrush, true);

            SkillPropertyStack.Children.Add(grid);

            mPropertyInfoDic[rect] = info;
        }

        public override string GCode_GetValueName(FrameworkElement element)
        {
            if (String.IsNullOrEmpty(SkillParamName.Text))
            {
                return "skill_" + Program.GetValuedGUIDString(Id);
            }
            else
                return SkillParamName.Text;
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            var linkOI = GetLinkObjInfo(element);
            if (linkOI.HasLink)
            {
                return mPropertyInfoDic[element].PropertyType.ToString();
            }

            return base.GCode_GetValueType(element);
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            var linkOI = GetLinkObjInfo(element);
            if (linkOI.HasLink)
            {
                return new System.CodeDom.CodeFieldReferenceExpression(new System.CodeDom.CodeVariableReferenceExpression(GCode_GetValueName(null)), mPropertyInfoDic[element].Name);
            }

            return base.GCode_CodeDom_GetValue(element);
        }

        public override void GCode_CodeDom_GenerateCode(System.CodeDom.CodeTypeDeclaration codeClass, System.CodeDom.CodeStatementCollection codeStatementCollection, FrameworkElement element)
        {
            System.CodeDom.CodeExpression valInExp = null;
            var linkOI = GetLinkObjInfo(ValueSet);
            if (linkOI.HasLink)
            {
                // 该节点有链接，表示从外面赋值
                if (!linkOI.GetLinkObject(0, true).IsOnlyReturnValue)
                    linkOI.GetLinkObject(0, true).GCode_CodeDom_GenerateCode(codeClass, codeStatementCollection, linkOI.GetLinkElement(0, true));

                valInExp = linkOI.GetLinkObject(0, true).GCode_CodeDom_GetValue(linkOI.GetLinkElement(0, true));
            }

            if (codeStatementCollection.Contains(mSkillVarDec))
            {
                if (valInExp != null)
                {
                    // 如果已包含声明，则直接赋值
                    System.CodeDom.CodeAssignStatement skillAssign = new System.CodeDom.CodeAssignStatement();
                    skillAssign.Left = new System.CodeDom.CodeVariableReferenceExpression(GCode_GetValueName(null));
                    skillAssign.Right = valInExp;
                    codeStatementCollection.Add(skillAssign);
                }
            }
            else
            {
                // 未包含声明则先声明并创建对象
                mSkillVarDec.Type = new System.CodeDom.CodeTypeReference(mRoleSkillType);
                mSkillVarDec.Name = GCode_GetValueName(null);

                if (valInExp != null)
                    mSkillVarDec.InitExpression = valInExp;
                else
                    mSkillVarDec.InitExpression = new System.CodeDom.CodeObjectCreateExpression(mRoleSkillType.ToString(), new System.CodeDom.CodeExpression[] { });

                codeStatementCollection.Add(mSkillVarDec);
            }
        }
    }
}
