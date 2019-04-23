using System.Windows.Controls;
using CSUtility.Support;

namespace AIEditor.LinkSystem
{
    public class MethodNode : CodeDomNode.MethodNode
    {
        string mCurrentState = "";
        public string CurrentState
        {
            get { return mCurrentState; }
            set
            {
                mCurrentState = value;
            }
        }

        string mTargetState = "";
        public string TargetState
        {
            get { return mTargetState; }
            set
            {
                mTargetState = value;
            }
        }

        string mChangeToState = "";
        public string ChangeToState
        {
            get { return mChangeToState; }
            set
            {
                mChangeToState = value;
            }
        }

        StateMethodsEditorControl.enMethodDelegateEditType mMethodEditType = StateMethodsEditorControl.enMethodDelegateEditType.Default;
        public StateMethodsEditorControl.enMethodDelegateEditType MethodEditType
        {
            get { return mMethodEditType; }
            set
            {
                mMethodEditType = value;
            }
        }

        public MethodNode(Canvas parentCanvas, string methodInfo)
            : base(parentCanvas, methodInfo)
        {

        }

        public override void Save(XmlNode xmlNode, bool newGuid, XmlHolder holder)
        {
            if (string.IsNullOrEmpty(CurrentState))
                xmlNode.AddAttrib("CurrentState", CurrentState);
            if (string.IsNullOrEmpty(TargetState))
                xmlNode.AddAttrib("TargetState", TargetState);
            if (string.IsNullOrEmpty(ChangeToState))
                xmlNode.AddAttrib("ChangeToState", ChangeToState);
            xmlNode.AddAttrib("MethodEditType", MethodEditType.ToString());

            base.Save(xmlNode, newGuid, holder);
        }

        public override void Load(XmlNode xmlNode, double deltaX, double deltaY)
        {
            var att = xmlNode.FindAttrib("CurrentState");
            if (att != null)
                CurrentState = att.Value;
            att = xmlNode.FindAttrib("TargetState");
            if (att != null)
                TargetState = att.Value;
            att = xmlNode.FindAttrib("ChangeToState");
            if (att != null)
                ChangeToState = att.Value;
            att = xmlNode.FindAttrib("MethodEditType");
            if (att != null)
                MethodEditType = (StateMethodsEditorControl.enMethodDelegateEditType)System.Enum.Parse(typeof(StateMethodsEditorControl.enMethodDelegateEditType), att.Value);

            base.Load(xmlNode, deltaX, deltaY);
        }

        #region 代码生成

        public static string GetDelegateMethodName(string curStateName, string tagStateName, StateMethodsEditorControl.enMethodDelegateEditType editType, string methodName)
        {
            switch (editType)
            {
                case StateMethodsEditorControl.enMethodDelegateEditType.Default:
                    return "Default_" + methodName;

                case StateMethodsEditorControl.enMethodDelegateEditType.CurrentState:
                    return "ChangeStateTo" + tagStateName + "_" + methodName;

                case StateMethodsEditorControl.enMethodDelegateEditType.TargetState:
                    return "ChangeStateFrom" + curStateName + "_" + methodName;

                case StateMethodsEditorControl.enMethodDelegateEditType.SelfChange:
                    return "ChangeToSelfState_" + methodName;
            }

            return "";
        }

        protected override string GetDelegateMethodName()
        {
            return GetDelegateMethodName(CurrentState, TargetState, MethodEditType, MethodName.Text);
        }

        #endregion
    }
}
