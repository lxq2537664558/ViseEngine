using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AIEditor
{
    /// <summary>
    /// Interaction logic for StateGridStateChange.xaml
    /// </summary>
    public partial class StateGridStateChange : UserControl
    {
        public delegate void Delegate_OnDirtyChanged(bool dirty);
        public Delegate_OnDirtyChanged OnDirtyChanged;

        //Guid mHostAIInstanceId = Guid.Empty;
        public Guid HostAIInstanceId
        {
            get
            {
                if (mStateSwitchInfo == null)
                    return Guid.Empty;

                return mStateSwitchInfo.HostFSM.Id;
            }
        }

        public AIEditor.FSMTemplateInfo HostAIInstance
        {
            get
            {
                if (mStateSwitchInfo == null)
                    return null;

                return mStateSwitchInfo.HostFSM;
            }
        }

        //Type mCurrentStateType = null;
        public string CurrentStateType
        {
            get
            {
                if (mStateSwitchInfo == null)
                    return null;

                return mStateSwitchInfo.CurrentStateType;
            }
        }

        //Type mTargetStateType = null;
        public string TargetStateType
        {
            get
            {
                if (mStateSwitchInfo == null)
                    return null;

                return mStateSwitchInfo.TargetStateType;
            }
        }

        public string ChangeToStateType
        {
            get
            {
                if (mStateSwitchInfo == null)
                    return null;

                return mStateSwitchInfo.NewCurrentStateType;
            }
        }

        public string NewTargetStateType
        {
            get
            {
                if (mStateSwitchInfo == null)
                    return null;

                return mStateSwitchInfo.NewTargetStateType;
            }
        }

        //bool mHasLinks = false;
        //public bool HasLinks
        //{
        //    get { return mHasLinks; }
        //    set
        //    {
        //        mHasLinks = value;
        //        if (mHasLinks)
        //        {
        //            Grid_BG.Background = this.FindResource("HaveLinksBrush") as Brush;
        //        }
        //        else
        //        {
        //            Grid_BG.Background = this.FindResource("DoNotHaveLinksBrush") as Brush;
        //        }
        //    }
        //}

        bool mIsDirty = false;
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;

                if (mIsDirty)
                {
                    Rect_Dirty.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    Rect_Dirty.Visibility = System.Windows.Visibility.Hidden;
                }

                //if (mIsDirty == true)
                //{
                //    var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceId);
                //    if (aiIns != null)
                //    {
                //        aiIns.IsDirty = true;
                //    }
                //}
            }
        }

        //StateGridStateChangeEditorWindow mEditorWin = null;
        AIEditor.FSMTemplateInfo.StateSwitchInfo mStateSwitchInfo = null;
        MainControl mHostControl = null;

        public StateGridStateChange(AIEditor.FSMTemplateInfo.StateSwitchInfo stateSwitchInfo, MainControl hostControl)
        //public StateGridStateChange(Guid hostAIId, Type current, Type target)
        {
            InitializeComponent();

            mStateSwitchInfo = stateSwitchInfo;
            mHostControl = hostControl;

            InitComboBoxState();

            //mHostAIInstanceId = hostAIId;
            //mCurrentStateType = current;
            //mTargetStateType = target;

            var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(HostAIInstanceId, false);
            if (aiIns != null)
            {
                TextBlock_TargetStateName.Text = GetStateInfoName(mStateSwitchInfo.NewCurrentStateType, aiIns.GetStateNickName(mStateSwitchInfo.NewCurrentStateType));
            }

            //TextBlock_TargetStateName.Text = AIEditor.FSMTemplateInfo.GetStateTypeAttributeName(mStateSwitchInfo.NewCurrentStateType);
            if (string.IsNullOrEmpty(mStateSwitchInfo.NewCurrentStateType))
            {
                ComboBox_State.SelectedIndex = 0;
            }
            else
            {
                foreach (ComboBoxItem item in ComboBox_State.Items)
                {
                    if (object.Equals(item.Tag, ChangeToStateType))//mStateSwitchInfo.NewCurrentStateType))
                    {
                        ComboBox_State.SelectedItem = item;
                        break;
                    }
                }
            }

            if (aiIns != null)
            {
                var stateCName = aiIns.GetStateNickName(CurrentStateType);
                var stateTName = aiIns.GetStateNickName(TargetStateType);
                //var atts = CurrentStateType.GetCustomAttributes(typeof(AISystem.Attribute.StatementClassAttribute), true);
                var toolTipStr = "由 " + stateCName + " 转到 " + stateTName + " 的过程";
                this.ToolTip = toolTipStr;
                TextBlock_StateChangeInfo.Text = toolTipStr;
            }

            //var aiInsInfo = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceId);
            //CSUtility.Support.XmlHolder tempXmlHolder = null;
            //if(HostAIInstance.StateChangeXmlHolders.TryGetValue(AIEditor.FSMTemplateInfo.GetStateChangeDictionaryKey(CurrentStateType, TargetStateType), out tempXmlHolder))
            //{
            //    HasLinks = true;
            //}
            //else
            //{
            //    HasLinks = false;
            //}
        }

        private void InitComboBoxState()
        {
            ComboBox_State.Items.Clear();
            
            ComboBoxItem item = new ComboBoxItem();
            item.Content = "不可转换";
            item.Tag = null;
            ComboBox_State.Items.Add(item);

            var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(HostAIInstanceId, false);
            if (aiIns != null)
            {
                //var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceId);
                foreach (var stateType in HostAIInstance.StateTypes)
                {
                    var stateName = aiIns.GetStateNickName(stateType);
                    ComboBoxItem cItem = new ComboBoxItem()
                    {
                        Content = stateName,
                        Tag = stateType,
                    };
                    ComboBox_State.Items.Add(cItem);
                }
            }
        }

        public void ChangeStateName(string stateName, string newNickName)
        {
            if (ChangeToStateType != stateName)
                return;

            //if (mStateSwitchInfo.CurrentStateType == oldName)
            //    mStateSwitchInfo.CurrentStateType = newName;

            //mStateSwitchInfo.ChangeStateName(oldName, newName);
            var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(HostAIInstanceId, false);

            if (aiIns != null)
            {
                var param = aiIns.GetStateSpecialParams(stateName);
                if(param != null)
                    TextBlock_TargetStateName.Text = GetStateInfoName(mStateSwitchInfo.NewCurrentStateType, param.NickName);//aiIns.GetStateTypeAttributeName(mStateSwitchInfo.NewCurrentStateType));
            }
        }

        private void EditorControlDirtyChanged(bool dirty)
        {
            IsDirty = dirty;
        }

        private string GetEditorWinTitle()
        {
            if (CurrentStateType == null || TargetStateType == null)
                return "";

            var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(HostAIInstanceId, false);

            if (aiIns != null)
            {
                string currentStateName = "";
                string targetStateName = "";
                //var atts = CurrentStateType.GetCustomAttributes(typeof(AISystem.Attribute.StatementClassAttribute), true);
                //if (atts.Length > 0)
                //{
                //    var att = atts[0] as AISystem.Attribute.StatementClassAttribute;
                //    currentStateName = att.m_strName;
                //}
                //atts = TargetStateType.GetCustomAttributes(typeof(AISystem.Attribute.StatementClassAttribute), true);
                //if (atts.Length > 0)
                //{
                //    var att = atts[0] as AISystem.Attribute.StatementClassAttribute;
                //    targetStateName = att.m_strName;
                //}

                currentStateName = aiIns.GetStateNickName(CurrentStateType);
                targetStateName = aiIns.GetStateNickName(TargetStateType);

                return currentStateName + " 转 " + targetStateName;
            }

            return "";
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (e.ClickCount == 2)
            //{
            //    if(mEditorWin == null || mEditorWin.WindowClosed)
            //    {
            //        mEditorWin = new StateGridStateChangeEditorWindow();
            //        mEditorWin.Title = GetEditorWinTitle();
            //        mEditorWin.EditorControl.CurrentStateType = CurrentStateType;
            //        mEditorWin.EditorControl.TargetStateType = TargetStateType;
            //        mEditorWin.EditorControl.HostAIInsInfoId = HostAIInstanceId;
            //        mEditorWin.EditorControl.Load();

            //        mEditorWin.EditorControl.OnDirtyChanged = new StateGridStateChangeEditorControl.Delegate_OnDirtyChanged(EditorControlDirtyChanged);
            //    }
                
            //    mEditorWin.Show();
            //}
        }

        private void Grid_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Popup_Details.IsOpen = true;
        }

        StateMethodEditorTabItemControl mCurrentStateTabItem;
        private void Button_EditCurrentState_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Popup_Details.IsOpen = false;
            if (mHostControl == null)
                return;

            var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(HostAIInstanceId, false);
            if (aiIns != null)
            {
                var headerName = "(" + aiIns.GetStateNickName(CurrentStateType) +
                                " 转 " +
                                aiIns.GetStateNickName(TargetStateType) +
                                ") " +
                                aiIns.GetStateNickName(CurrentStateType) +
                                "函数代理编辑";
                if (mCurrentStateTabItem != null)
                {
                    mCurrentStateTabItem.HeaderName = headerName;
                    if (mCurrentStateTabItem.Parent == null)
                        mHostControl.TabControl_Items.Items.Add(mCurrentStateTabItem);
                    mCurrentStateTabItem.IsSelected = true;
                    return;
                }

                mCurrentStateTabItem = new StateMethodEditorTabItemControl(HostAIInstanceId, mHostControl.TabControl_Items);
                mCurrentStateTabItem.HeaderName = headerName;
                if (CurrentStateType == ChangeToStateType)
                    mCurrentStateTabItem.Initialize(CurrentStateType, TargetStateType, ChangeToStateType, StateMethodsEditorControl.enMethodDelegateEditType.SelfChange);
                else
                    mCurrentStateTabItem.Initialize(CurrentStateType, TargetStateType, ChangeToStateType, StateMethodsEditorControl.enMethodDelegateEditType.CurrentState);
                mHostControl.TabControl_Items.Items.Add(mCurrentStateTabItem);
                mCurrentStateTabItem.IsSelected = true;

                /*/ 打开窗口
                StateMethodsEditorWindow window = new StateMethodsEditorWindow(HostAIInstanceId);
                //System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window);
                window.Title = aiIns.GetStateNickName(CurrentStateType) +
                               " 转 " +
                               aiIns.GetStateNickName(TargetStateType) +
                               " " +
                               aiIns.GetStateNickName(CurrentStateType) +
                               "函数代理编辑";

                if (CurrentStateType == ChangeToStateType)
                {
                    window.Initialize(CurrentStateType, TargetStateType, ChangeToStateType, StateMethodsEditorControl.enMethodDelegateEditType.SelfChange);
                }
                else
                {
                    window.Initialize(CurrentStateType, TargetStateType, ChangeToStateType, StateMethodsEditorControl.enMethodDelegateEditType.CurrentState);
                }
                window.Show();*/
            }
        }

        StateMethodEditorTabItemControl mTargetStateTabItem;
        private void Button_EditTargetState_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Popup_Details.IsOpen = false;
            if (ChangeToStateType == null)
                return;

            if (mHostControl == null)
                return;

            var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(HostAIInstanceId, false);
            if (aiIns != null)
            {
                var headerName = "(" + aiIns.GetStateNickName(CurrentStateType) +
                               " 转 " +
                               aiIns.GetStateNickName(TargetStateType) +
                               ") " +
                               aiIns.GetStateNickName(ChangeToStateType) +
                               "函数代理编辑";
                if(mTargetStateTabItem != null)
                {
                    mTargetStateTabItem.HeaderName = headerName;
                    if (mTargetStateTabItem.Parent == null)
                        mHostControl.TabControl_Items.Items.Add(mTargetStateTabItem);
                    mTargetStateTabItem.IsSelected = true;
                    return;
                }

                mTargetStateTabItem = new StateMethodEditorTabItemControl(HostAIInstanceId, mHostControl.TabControl_Items);
                mTargetStateTabItem.HeaderName = headerName;
                if (CurrentStateType == ChangeToStateType)
                    mTargetStateTabItem.Initialize(CurrentStateType, TargetStateType, ChangeToStateType, StateMethodsEditorControl.enMethodDelegateEditType.SelfChange);
                else
                    mTargetStateTabItem.Initialize(CurrentStateType, TargetStateType, ChangeToStateType, StateMethodsEditorControl.enMethodDelegateEditType.TargetState);
                mHostControl.TabControl_Items.Items.Add(mTargetStateTabItem);
                mTargetStateTabItem.IsSelected = true;

                /*/ 打开窗口
                StateMethodsEditorWindow window = new StateMethodsEditorWindow(HostAIInstanceId);
                window.Title = aiIns.GetStateNickName(CurrentStateType) +
                               " 转 " +
                               aiIns.GetStateNickName(TargetStateType) +
                               " " +
                               aiIns.GetStateNickName(ChangeToStateType) +
                               "函数代理编辑";

                if (CurrentStateType == ChangeToStateType)
                {
                    window.Initialize(CurrentStateType, TargetStateType, ChangeToStateType, StateMethodsEditorControl.enMethodDelegateEditType.SelfChange);
                }
                else
                    window.Initialize(CurrentStateType, TargetStateType, ChangeToStateType, StateMethodsEditorControl.enMethodDelegateEditType.TargetState);
                window.Show();*/
            }
        }

        private void ComboBox_State_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_State.SelectedIndex < 0)
                return;

            if (ComboBox_State.SelectedIndex == 0)
            {
                Grid_ChangeDisable.Visibility = System.Windows.Visibility.Visible;
                Button_EditTarget.IsEnabled = false;
            }
            else
            {
                Grid_ChangeDisable.Visibility = System.Windows.Visibility.Hidden;
                Button_EditTarget.IsEnabled = true;
            }

            ComboBoxItem item = ComboBox_State.SelectedItem as ComboBoxItem;
            mStateSwitchInfo.NewCurrentStateType = item.Tag as string;

            var aiIns = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(HostAIInstanceId, false);
            if (aiIns != null)
            {
                TextBlock_TargetStateName.Text = GetStateInfoName(mStateSwitchInfo.NewCurrentStateType, aiIns.GetStateNickName(mStateSwitchInfo.NewCurrentStateType));
            }

            if (mStateSwitchInfo.NewCurrentStateType == TargetStateType)
            {
                Grid_BG.Background = this.TryFindResource("ToSameTarget") as Brush;
            }
            else if (ComboBox_State.SelectedIndex == 0)
            {
                Grid_BG.Background = this.TryFindResource("DefaultBackground") as Brush;
            }
            else
            {
                Grid_BG.Background = this.TryFindResource("ToDifferentTarget") as Brush;
            }
        
        }

        private string GetStateInfoName(string changeToStateType, string stateAttName)
        {
            if (string.IsNullOrEmpty(changeToStateType))
            {
                return "";
            }

            return changeToStateType + "(" + stateAttName + ")";
        }
    }
}
