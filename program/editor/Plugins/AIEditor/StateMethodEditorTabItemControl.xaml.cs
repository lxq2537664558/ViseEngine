using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AIEditor
{
    /// <summary>
    /// StateMethodEditorTabControl.xaml 的交互逻辑
    /// </summary>
    public partial class StateMethodEditorTabItemControl : TabItem
    {
        public StateMethodsEditorControl.Delegate_OnSaveMethodInfo OnSaveMethodInfo;
        Guid mHostAIInstanceInfoId = Guid.Empty;

        public string HeaderName
        {
            get { return (string)GetValue(HeaderNameProperty); }
            set { SetValue(HeaderNameProperty, value); }
        }
        public static readonly DependencyProperty HeaderNameProperty =
            DependencyProperty.Register("HeaderName", typeof(string), typeof(StateMethodEditorTabItemControl), new FrameworkPropertyMetadata(""));

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

        TabControl mHostTab;
        public StateMethodEditorTabItemControl(Guid id, TabControl hostTab)
        {
            InitializeComponent();

            mHostTab = hostTab;

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
        
        private void _OnSaveMethodInfo()
        {
            if (OnSaveMethodInfo != null)
                OnSaveMethodInfo();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            SMEControl_Common.Save();
            SMEControl_Server.Save();
            SMEControl_Client.Save();

            mHostTab.Items.Remove(this);
        }
    }
}
