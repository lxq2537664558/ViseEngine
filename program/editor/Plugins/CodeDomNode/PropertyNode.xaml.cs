using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CodeDomNode
{
    public sealed partial class PropertyNode : CodeGenerateSystem.Base.BaseNodeControl, CodeGenerateSystem.Base.UsefulMember
    {
        public List<CodeGenerateSystem.Base.UsefulMemberHostData> GetUsefulMembers()
        {
            List<CodeGenerateSystem.Base.UsefulMemberHostData> retValue = new List<CodeGenerateSystem.Base.UsefulMemberHostData>();
            if (!IsOnlyReturnValue)
                return retValue;

            var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
            {
                ClassTypeFullName = PropertyType,
                HostControl = this,
                LinkObject = mOutLinkInfo,
            };

            retValue.Add(memberData);

            return retValue;
        }

        public List<CodeGenerateSystem.Base.UsefulMemberHostData> GetUsefulMembers(CodeGenerateSystem.Base.LinkControl linkCtrl)
        {
            List<CodeGenerateSystem.Base.UsefulMemberHostData> retValue = new List<CodeGenerateSystem.Base.UsefulMemberHostData>();
            if (!IsOnlyReturnValue)
                return retValue;

            if (linkCtrl == ValueOutHandle)
            {
                var memberData = new CodeGenerateSystem.Base.UsefulMemberHostData()
                {
                    ClassTypeFullName = PropertyType,
                    HostControl = this,
                    LinkObject = mOutLinkInfo,
                };

                retValue.Add(memberData);
            }

            return retValue;
        }

        string mPropertyName = "";
        string mPropertyType = "";
        public string PropertyType
        {
            get { return mPropertyType; }
            set
            {
                mPropertyType = value;
                OnPropertyChanged("PropertyType");
            }
        }
        CodeGenerateSystem.Base.UsefulMemberHostData mHostUsefulMemberData = new CodeGenerateSystem.Base.UsefulMemberHostData();
        CodeGenerateSystem.Base.LinkObjInfo mOutLinkInfo;

        // 类实例名称
        string mClassInstanceName;
        public string ClassInstanceName
        {
            get { return mClassInstanceName; }
            set
            {
                mClassInstanceName = value;
                OnPropertyChanged("ClassInstanceName");
            }
        }

        public PropertyNode(Canvas parentCanvas, string param)
            : base(parentCanvas, param)
        {
            this.InitializeComponent();

            SetDragObject(RectangleTitle);

            var splits = param.Split(';');
            if (splits.Length < 2)
                return;

            var propertyParams = splits[0].Split(',');

            PropertyName.Text = propertyParams[0];
            NodeName = PropertyName.Text;
            mPropertyName = propertyParams[1];
            PropertyType = propertyParams[2];

            switch (propertyParams[3])
            {
                case "set":
                    {
                        ValueOutHandle.Visibility = System.Windows.Visibility.Collapsed;
                        AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromTypeString(PropertyType), ValueInHandle, CodeGenerateSystem.Base.enBezierType.Left, CodeGenerateSystem.Base.enLinkOpType.End, ValueInHandle.BackBrush, false);
                    }
                    break;
                case "get":
                    {
                        ValueInHandle.Visibility = System.Windows.Visibility.Collapsed;
                        IsOnlyReturnValue = true;
                        mOutLinkInfo = AddLinkObject(CodeGenerateSystem.Base.LinkObjInfo.GetLinkTypeFromTypeString(PropertyType), ValueOutHandle, CodeGenerateSystem.Base.enBezierType.Right, CodeGenerateSystem.Base.enLinkOpType.Start, ValueOutHandle.BackBrush, true);
                    }
                    break;
            }

            OnDeleteNode += (node) =>
            {
                parentCanvas.Children.Remove(mParentLinkPath);
                if (mHostUsefulMemberData.LinkObject != null)
                    mHostUsefulMemberData.LinkObject.m_virtualNodes.Remove(this);
            };
        }

        public static string GetParamInPropertyInfo(PropertyInfo proInfo, bool set, string path)
        {
            if (proInfo == null)
                return "";
            if (set && !proInfo.CanWrite)
                return "";
            if (!set && !proInfo.CanRead)
                return "";

            string pre = GetParamPreInfo(set);
            string end = "";
            if (set)
            {
                end = "set";
            }
            else
            {
                end = "get";
            }
            string strRet = path + "(" + pre + ")," + proInfo.Name + "," + proInfo.PropertyType.FullName + "," + end;

            return strRet;
        }
        public static string GetParamPreInfo(bool bSet)
        {
            if(bSet)
                return "设置";
            else
                return "读取";
        }

        private void InitializeLinkLine()
        {
            if (ParentDrawCanvas == null)
                return;

            if (mHostUsefulMemberData.LinkObject != null)
                mHostUsefulMemberData.LinkObject.m_virtualNodes.Add(this);

            BindingOperations.ClearBinding(this.mParentLinkPath, Path.VisibilityProperty);
            BindingOperations.SetBinding(this.mParentLinkPath, Path.VisibilityProperty, new Binding("Visibility") { Source = this });
            mParentLinkPath.Stroke = Brushes.LightGray;
            mParentLinkPath.StrokeDashArray = new DoubleCollection(new double[] { 2, 4 });
            //m_ParentLinkPath.StrokeThickness = 3;
            mParentLinkPathFig.Segments.Add(mParentLinkBezierSeg);
            PathFigureCollection pfc = new PathFigureCollection();
            pfc.Add(mParentLinkPathFig);
            PathGeometry pg = new PathGeometry();
            pg.Figures = pfc;
            mParentLinkPath.Data = pg;
            ParentDrawCanvas.Children.Add(mParentLinkPath);
        }

        public override void UpdateLink()
        {
            base.UpdateLink();

            if (mHostUsefulMemberData == null || mHostUsefulMemberData.LinkObject == null)
                return;

            mParentLinkPathFig.StartPoint = mHostUsefulMemberData.LinkObject.LinkElement.TranslatePoint(mHostUsefulMemberData.LinkObject.LinkElementOffset, ParentDrawCanvas);
            mParentLinkBezierSeg.Point3 = TranslatePoint(new Point(0, 0), ParentDrawCanvas);

            double delta = Math.Max(Math.Abs(mParentLinkBezierSeg.Point3.X - mParentLinkPathFig.StartPoint.X) / 2, 25);
            delta = Math.Min(150, delta);

            switch (mHostUsefulMemberData.LinkObject.m_bezierType)
            {
                case CodeGenerateSystem.Base.enBezierType.Left:
                    mParentLinkBezierSeg.Point1 = new Point(mParentLinkPathFig.StartPoint.X - delta, mParentLinkPathFig.StartPoint.Y);
                    break;
                case CodeGenerateSystem.Base.enBezierType.Right:
                    mParentLinkBezierSeg.Point1 = new Point(mParentLinkPathFig.StartPoint.X + delta, mParentLinkPathFig.StartPoint.Y);
                    break;
                case CodeGenerateSystem.Base.enBezierType.Top:
                    mParentLinkBezierSeg.Point1 = new Point(mParentLinkPathFig.StartPoint.X, mParentLinkPathFig.StartPoint.Y - delta);
                    break;
                case CodeGenerateSystem.Base.enBezierType.Bottom:
                    mParentLinkBezierSeg.Point1 = new Point(mParentLinkPathFig.StartPoint.X, mParentLinkPathFig.StartPoint.Y + delta);
                    break;
            }

            mParentLinkBezierSeg.Point2 = new Point(mParentLinkBezierSeg.Point3.X, mParentLinkBezierSeg.Point3.Y - delta);

        }

        public override void InitializeUsefulLinkDatas()
        {
            var methodParamSplits = this.StrParams.Split(';');
            if (methodParamSplits.Length < 2)
                return;

            mHostUsefulMemberData.ParseString(methodParamSplits[1], HostNodesContainer);            

            switch (mHostUsefulMemberData.HostType)
            {
                case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Static:
                    ClassInstanceName = mHostUsefulMemberData.ClassTypeFullName;
                    break;
                case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Instance:
                    ClassInstanceName = mHostUsefulMemberData.ClassTypeFullName + ".Instance";
                    break;
                case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Normal:
                    {
                        var name = mHostUsefulMemberData.HostControl.GCode_GetValueName(mHostUsefulMemberData.LinkObject.LinkElement);
                        ClassInstanceName = name + "(" + mHostUsefulMemberData.ClassTypeFullName + ")";
                    }
                    break;
            }

            InitializeLinkLine();
        }

        #region 代码生成

        public override string GCode_GetValueName(FrameworkElement element)
        {
            return "ClassPropertyValue_" + CodeGenerateSystem.Program.GetValuedGUIDString(Id);
        }

        public override string GCode_GetValueType(FrameworkElement element)
        {
            return mPropertyType;
        }

        public override CodeExpression GCode_CodeDom_GetValue(FrameworkElement element)
        {
            var proExp = new System.CodeDom.CodePropertyReferenceExpression();
            switch (mHostUsefulMemberData.HostType)
            {
                case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Static:
                    proExp.TargetObject = new System.CodeDom.CodeSnippetExpression(mHostUsefulMemberData.ClassTypeFullName);
                    break;
                case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Instance:
                    proExp.TargetObject = new System.CodeDom.CodeSnippetExpression(mHostUsefulMemberData.ClassTypeFullName + ".Instance");
                    break;
                case CodeGenerateSystem.Base.UsefulMemberHostData.enHostType.Normal:
                    proExp.TargetObject = new System.CodeDom.CodeCastExpression(new System.CodeDom.CodeTypeReference(mHostUsefulMemberData.ClassTypeFullName),
                        mHostUsefulMemberData.HostControl.GCode_CodeDom_GetValue(mHostUsefulMemberData.LinkObject.LinkElement));
                        //new System.CodeDom.CodeSnippetExpression(mHostUsefulMemberData.HostControl.GCode_GetValueName(mHostUsefulMemberData.LinkObject.LinkElement)));
                    break;
            }
            proExp.PropertyName = mPropertyName;
            return proExp;
        }

        #endregion
    }
}
