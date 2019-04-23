using System;
using System.Windows;

namespace AIEditor
{
    /// <summary>
    /// Interaction logic for StateMethodsEditorWindow.xaml
    /// </summary>
    public partial class StateMethodsEditorWindow : DockControl.Controls.DockAbleWindowBase
    {
        //Type mStateType;
        //public Type StateType
        //{
        //    get { return mStateType; }
        //    set
        //    {
        //        mStateType = value;
        //        SMEControl.CurStateType = mStateType;
        //    }
        //}
        public StateMethodsEditorControl.Delegate_OnSaveMethodInfo OnSaveMethodInfo;

        public StateMethodsEditorControl EditorControl_Common
        {
            get { return SMEControl_Common; }
        }

        public StateMethodsEditorControl EditorControl_Server
        {
            get { return SMEControl_Server; }
        }

        public StateMethodsEditorControl EditorControl_Client
        {
            get { return SMEControl_Client; }
        }

        Guid mHostAIInstanceInfoId = Guid.Empty;

        public StateMethodsEditorWindow(Guid id)
        {
            InitializeComponent();

            this.LayoutManaged = false;
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);

            mHostAIInstanceInfoId = id;
            SMEControl_Common.HostAIInstanceInfoId = mHostAIInstanceInfoId;
            SMEControl_Common.OnSaveMethodInfo = _OnSaveMethodInfo;
            SMEControl_Server.HostAIInstanceInfoId = mHostAIInstanceInfoId;
            SMEControl_Server.OnSaveMethodInfo = _OnSaveMethodInfo;
            SMEControl_Client.HostAIInstanceInfoId = mHostAIInstanceInfoId;
            SMEControl_Client.OnSaveMethodInfo = _OnSaveMethodInfo;
        }

        public void Initialize(string curState, string tagState, string changeToState, AIEditor.StateMethodsEditorControl.enMethodDelegateEditType editType)
        {
            SMEControl_Common.Initialize(curState, tagState, changeToState, editType, CSUtility.Helper.enCSType.Common);
            SMEControl_Server.Initialize(curState, tagState, changeToState, editType, CSUtility.Helper.enCSType.Server);
            SMEControl_Client.Initialize(curState, tagState, changeToState, editType, CSUtility.Helper.enCSType.Client);

            var hostInstance = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceInfoId, false);
            if (hostInstance != null)
            {
                SMEControl_Common.InitStatePropertyNodesList(hostInstance.GetStatePropertys(curState));
                SMEControl_Server.InitStatePropertyNodesList(hostInstance.GetStatePropertys(curState));
                SMEControl_Client.InitStatePropertyNodesList(hostInstance.GetStatePropertys(curState));
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            SMEControl_Common.Save();
            SMEControl_Server.Save();
            SMEControl_Client.Save();
        }

        private void _OnSaveMethodInfo()
        {
            if (OnSaveMethodInfo != null)
                OnSaveMethodInfo();
        }
    }
}
