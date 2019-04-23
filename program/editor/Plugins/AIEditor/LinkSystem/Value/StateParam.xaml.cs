using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AIEditor.LinkSystem.Value
{
    /// <summary>
    /// Interaction logic for StateParam.xaml
    /// </summary>
    public partial class StateParam : CodeGenerateSystem.Base.BaseNodeControl
    {
        Type mStateParameterType = null;
        public Type StateParameterType
        {
            get { return mStateParameterType; }
            protected set
            {
                mStateParameterType = value;

                if(mStateParameterType != null)
                    MoveHandle.Text = mStateParameterType.ToString();
            }
        }

        string mParamName = "";
        public string ParamName
        {
            get { return mParamName; }
            set
            {
                mParamName = value;
                OnPropertyChanged("ParamName");
            }
        }

        bool mParamNameReadOnly = false;
        public bool ParamNameReadOnly
        {
            get { return mParamNameReadOnly; }
            set
            {
                mParamNameReadOnly = value;
                OnPropertyChanged("ParamNameReadOnly");
            }
        }

        List<FrameworkElement> mParamRectList = new List<FrameworkElement>();      // 记录状态参数右侧的方框
        //List<FrameworkElement> mParamEllipseList = new List<FrameworkElement>();   // 记录状态参数左侧的圆

        public struct stInLinkData
        {
            public System.Reflection.FieldInfo filedInfo;
            public FrameworkElement linkElement;
        }
        Dictionary<string, stInLinkData> m_InLinkElements = new Dictionary<string, stInLinkData>();

        Dictionary<FrameworkElement, System.Reflection.FieldInfo> mValueInfoDic = new Dictionary<FrameworkElement, System.Reflection.FieldInfo>();

        public StateParam(Canvas parentCanvas, string strParam)
                    : base(parentCanvas, strParam)
        {
            InitializeComponent();

            IsOnlyReturnValue = true;

            SetDragObject(RectangleTitle);
            if (!String.IsNullOrEmpty(strParam))
            {
                var splits = strParam.Split(',');
                StateParameterType = Program.GetType(splits[0]);
                if (StateParameterType == null)
                    StateParameterType = typeof(CSUtility.AISystem.StateParameter);
                SetStateParam(StateParameterType);

                ParamName = splits[1];
            }

            var linkObj = AddLinkObject(CodeGenerateSystem.Base.enLinkType.Class, ParamGetValue, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ParamGetValue.BackBrush, true);
            if (StateParameterType != null)
                linkObj.ClassType = StateParameterType;
        }

        public override void Save(CSUtility.Support.XmlNode xmlNode, bool newGuid, CSUtility.Support.XmlHolder holder)
        {
            if (StateParameterType != null)
            {
                StrParams = StateParameterType.ToString() + "," + ParamName;
            }

            base.Save(xmlNode, newGuid,holder);
        }

        public void SetStateParam(Type paramType)
        {
            ClearStateParamLink();

            var pLObj = GetLinkObjInfo(ParamGetValue);
            if (pLObj != null)
                pLObj.ClassType = paramType;

            StateParameterType = paramType;

            var fieldInfos = paramType.GetFields();
            foreach (var info in fieldInfos)
            {
                AddParamValue(info);
            }
        }

        private void AddParamValue(System.Reflection.FieldInfo info)
        {
            Grid grid = new Grid();
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            TextBlock textBlock = new TextBlock()
            {
                Text = info.Name + "(" + info.FieldType.Name + ")",
                HorizontalAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(textBlock);

            var rect = new CodeGenerateSystem.Controls.LinkOutControl()
            {
                Margin = new Thickness(0, 0, -13, 0),
                Width = 10,
                Height = 10,
                BackBrush = this.TryFindResource("Link_ValueBrush") as Brush,
                HorizontalAlignment = HorizontalAlignment.Right,
                Direction = CodeGenerateSystem.Base.enBezierType.Right,
            };
            grid.Children.Add(rect);
            AddLinkObject(info.FieldType, rect, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, rect.BackBrush, true);
            mParamRectList.Add(rect);

            ParamStack.Children.Add(grid);

            //stInLinkData data = new stInLinkData();
            //data.filedInfo = info;
            //data.linkElement = Ellipse;
            //m_InLinkElements[info.Name] = data;

            mValueInfoDic[rect] = info;
        }

        private void ClearStateParamLink()
        {
            foreach (var element in mParamRectList)
            {
                var linkObj = GetLinkObjInfo(element);
                linkObj.Clear();
            }
            mParamRectList.Clear();

            ParamStack.Children.Clear();
            mValueInfoDic.Clear();
        }

#region 代码生成

        public override string GCode_GetValueName(FrameworkElement element)
        {
            string strValueName = "";

            if (String.IsNullOrEmpty(ParamName))
            {
                strValueName = "stateParam_" + Program.GetValuedGUIDString(Id);
            }
            else
                strValueName = ParamName;

            if (element == ParamGetValue)
            {
                return strValueName;
            }
            else
            {
                System.Reflection.FieldInfo info = null;
                if (mValueInfoDic.TryGetValue(element, out info))
                {
                    return strValueName + "." + info.Name;
                }
            }

            return base.GCode_GetValueName(element);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            if (element == null || element == ParamGetValue)
                return StateParameterType.ToString();
            else
            {
                var linkOI = GetLinkObjInfo(element);
                if (linkOI.HasLink)
                {
                    System.Reflection.FieldInfo paramInfo;
                    if (mValueInfoDic.TryGetValue(element, out paramInfo))
                        return paramInfo.FieldType.ToString();
                }
            }

            return base.GCode_GetValueType(element);
        }

        public override System.CodeDom.CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            if (StateParameterType != null)
            {
                var linkOI = GetLinkObjInfo(element);
                if (linkOI.HasLink)
                {
                    string stateParamName = ParamName;
                    if(string.IsNullOrEmpty(ParamName))
                       stateParamName = StateParameterType.Name.Remove(0, 1);

                    if (element == ParamGetValue)
                        return new System.CodeDom.CodeVariableReferenceExpression(stateParamName);
                    else
                    {
                        System.Reflection.FieldInfo paramInfo;
                        if (mValueInfoDic.TryGetValue(element, out paramInfo))
                        {
                            System.CodeDom.CodeFieldReferenceExpression paramField = new System.CodeDom.CodeFieldReferenceExpression();
                            paramField.TargetObject = new System.CodeDom.CodeVariableReferenceExpression(stateParamName);
                            paramField.FieldName = paramInfo.Name;

                            return paramField;
                        }
                    }
                }
            }

            return base.GCode_CodeDom_GetValue(element);
        }

#endregion
    }
}
