using System;
using System.Windows.Controls;

namespace AIEditor
{
    /// <summary>
    /// Interaction logic for StateMethodEditorListItem.xaml
    /// </summary>
    public partial class StateMethodEditorListItem : UserControl
    {
        public delegate void Delegate_OnDirtyChanged(bool dirty);
        public Delegate_OnDirtyChanged OnDirtyChanged;

        string mCurStateType = null;
        string mTagStateType = null;
        string mChangeToStateType = null;
        StateMethodsEditorControl.enMethodDelegateEditType mMethodEditType = StateMethodsEditorControl.enMethodDelegateEditType.Default;
        Guid mHostAIInstanceId = Guid.Empty;

        Type mMethodClassType
        {
            get
            {
                switch (mMethodEditType)
                {
                    case StateMethodsEditorControl.enMethodDelegateEditType.Default:
                    case StateMethodsEditorControl.enMethodDelegateEditType.CurrentState:
                    case StateMethodsEditorControl.enMethodDelegateEditType.SelfChange:
                        {
                            var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceId, false);
                            if (aiIns != null)
                            {
                                return aiIns.GetStateBaseType(mCurStateType, mCSType);
                            }
                            //return mCurStateType;
                        }
                        break;

                    case StateMethodsEditorControl.enMethodDelegateEditType.TargetState:
                        {
                            var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceId, false);
                            if (aiIns != null)
                            {
                                return aiIns.GetStateBaseType(mChangeToStateType, mCSType);
                            }
                            //return mChangeToStateType;
                        }
                        break;
                }

                return null;
            }
        }

        System.Reflection.MethodInfo mMethodInfo;
        public System.Reflection.MethodInfo MethodInfo
        {
            get { return mMethodInfo; }
        }

        bool mHasLinks = false;
        public bool HasLinks
        {
            get { return mHasLinks; }
            set
            {
                mHasLinks = value;

                if (mHasLinks)
                {
                    var image = this.TryFindResource("ActiveLinksImage") as Image;
                    Image_Links.Source = image.Source;
                }
                else
                {
                    var image = this.TryFindResource("DeactiveLinksImage") as Image;
                    Image_Links.Source = image.Source;
                }
            }
        }

        bool mIsDirty = false;
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;

                if (OnDirtyChanged != null)
                    OnDirtyChanged(mIsDirty);
            }
        }

        CSUtility.Helper.enCSType mCSType;

        public void OnContainLinkNodesChanged(bool contain, CSUtility.Helper.enCSType csType)
        {
            HasLinks = contain;
        }

        public void OnLinkControlDirtyChanged(bool dirty)
        {
            IsDirty = dirty;
        }

        public StateMethodEditorListItem(Guid guid, string curStateType, string tagStateType, string changeToStateType, StateMethodsEditorControl.enMethodDelegateEditType methodEditType, System.Reflection.MethodInfo methodInfo, CSUtility.Helper.enCSType csType)
        {
            InitializeComponent();

            mCSType = csType;
            mHostAIInstanceId = guid;
            mCurStateType = curStateType;
            mTagStateType = tagStateType;
            mChangeToStateType = changeToStateType;
            mMethodEditType = methodEditType;
            mMethodInfo = methodInfo;
            TextBlock_MethodName.Text = mMethodInfo.Name;

            var aiInsInfo = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(guid, false);
            if (aiInsInfo != null)
            {
                CSUtility.Support.XmlHolder tempXmlHolder = null;
                HasLinks = aiInsInfo.StateMethodDelegateXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(curStateType, tagStateType, mMethodClassType, methodInfo, csType), out tempXmlHolder);
            }
        }
    }
}
