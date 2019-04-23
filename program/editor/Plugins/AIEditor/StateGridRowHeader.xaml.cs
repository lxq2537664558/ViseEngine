using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;

namespace AIEditor
{
    /// <summary>
    /// Interaction logic for StateGridRowHeader.xaml
    /// </summary>
    public partial class StateGridRowHeader : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public delegate void Delegate_OnDelete(StateGridRowHeader item);
        public Delegate_OnDelete OnDelete;

        public delegate void Delegate_OnSetDefaultState(StateGridRowHeader item);
        public Delegate_OnSetDefaultState OnSetDefaultState;

        public delegate void Delegate_OnStateNickNameChanged(string stateName, string newNickName);
        public Delegate_OnStateNickNameChanged OnStateNickNameChanged;
        
        //string mHeaderName = "";
        //public string HeaderName
        //{
        //    get { return mHeaderName; }
        //    set
        //    {
        //        mHeaderName = value;
        //        OnPropertyChanged("HeaderName");
        //    }
        //}

        // 状态实例名称，不允许修改
        string mStateName = "";
        public string StateName
        {
            get { return mStateName; }
            set
            {
                //var oldName = mStateName;
                mStateName = value;

                //if (!string.IsNullOrEmpty(oldName))
                //{
                //    var aiInfo = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceInfoId);
                //    if (aiInfo != null)
                //    {
                //        aiInfo.ChangeStateName(oldName, mStateName);
                //    }

                //    if (OnStateNickNameChanged != null)
                //        OnStateNickNameChanged(oldName, mStateName);
                //}

                OnPropertyChanged("StateName");
            }
        }

        string mNickName = "";
        public string NickName
        {
            get { return mNickName; }
            set
            {
                var oldName = mNickName;
                mNickName = value;

                if(!string.IsNullOrEmpty(oldName))
                {
                    var aiInfo = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceInfoId, false);
                    if (aiInfo != null)
                    {
                        aiInfo.ChangeNickName(StateName, mNickName);
                    }

                    if (OnStateNickNameChanged != null)
                        OnStateNickNameChanged(StateName, mNickName);
                }

                OnPropertyChanged("NickName");
            }
        }

        bool mIsDefaultState = false;
        public bool IsDefaultState
        {
            get { return mIsDefaultState; }
            set
            {
                mIsDefaultState = value;

                if (mIsDefaultState)
                {
                    var defaultStateColor = this.TryFindResource("DefaultStateColor") as Brush;
                    TextBox_StateName.Foreground = defaultStateColor;
                    TextBlock_C.Foreground = defaultStateColor;
                    TextBlock_NickName.Foreground = defaultStateColor;
                    TextBlock_D.Foreground = defaultStateColor;
                }
                else
                {
                    var normalStateColor = this.TryFindResource("NormalStateColor") as Brush;
                    TextBox_StateName.Foreground = normalStateColor;
                    TextBlock_C.Foreground = normalStateColor;
                    TextBlock_NickName.Foreground = normalStateColor;
                    TextBlock_D.Foreground = normalStateColor;
                }

                OnPropertyChanged("IsDefaultState");
            }
        }

        string mStateType = null;
        public string StateType
        {
            get { return mStateType; }
        }

        bool mHasStateValues = false;
        public bool HasStateValues
        {
            get { return mHasStateValues; }
            set
            {
                mHasStateValues = value;

                if (mHasStateValues)
                {
                    var image = this.TryFindResource("ActiveValueLinksImage") as Image;
                    Image_Values.Source = image.Source;
                }
                else
                {
                    var image = this.TryFindResource("DeactiveValueLinksImage") as Image;
                    Image_Values.Source = image.Source;
                }
            }
        }

        bool mHasStateMethodOverride = false;
        public bool HasStateMethodOVerride
        {
            get { return mHasStateMethodOverride; }
            set
            {
                mHasStateMethodOverride = value;

                if (mHasStateMethodOverride)
                {
                    var image = this.TryFindResource("ActiveMethodLinksImage") as Image;
                    Image_Methods.Source = image.Source;
                }
                else
                {
                    var image = this.TryFindResource("DeactiveMethodLinksImage") as Image;
                    Image_Methods.Source = image.Source;
                }
            }
        }

        Guid mHostAIInstanceInfoId = Guid.Empty;
        MainControl mHostControl = null;

        public StateGridRowHeader(string stateType, Guid aiInsInfoId, MainControl hostControl)
        {
            InitializeComponent();

            mStateType = stateType;
            mHostAIInstanceInfoId = aiInsInfoId;
            mHostControl = hostControl;

            StateName = stateType;

            //var hostInstance = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceInfoId);
            //if (hostInstance != null)
            //{
            //    var statePros = hostInstance.GetStatePropertys(stateType);
            //    if (statePros != null && statePros.Count > 0)
            //        HasStateValues = true;
            //    else
            //        HasStateValues = false;
            ////    var tw = AIEditor.CodeGenerate.CodeGenerator.GenerateCode(hostInstance);
            ////    ListView_StatePropertys.ItemsSource = AIEditor.CodeGenerate.CodeGenerator.CompileCode(tw.ToString()).Errors;
            ////    //ListView_StatePropertys.ItemsSource = hostInstance.GetStatePropertys(mStateType);
            //}
            UpdateHasStateValues();
            UpdateHasStateMethodOverride();

            //SMEControl.StateType = stateType;
        }

        private void UpdateHasStateValues()
        {
            var hostInstance = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceInfoId, false);
            if (hostInstance != null)
            {
                var statePros = hostInstance.GetStatePropertys(mStateType);
                if (statePros != null && statePros.Count > 0)
                    HasStateValues = true;
                else
                    HasStateValues = false;
            }
        }

        private void UpdateHasStateMethodOverride()
        {
            var hostInstance = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceInfoId, false);
            if (hostInstance != null)
            {
                foreach (var methodDelegate in hostInstance.StateMethodDelegateXmlHolders)
                {
                    if(methodDelegate.Key.Contains(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(mStateType, "", null, null, CSUtility.Helper.enCSType.Common)))
                    {
                        HasStateMethodOVerride = true;
                        return;
                    }

                    if (methodDelegate.Key.Contains(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(mStateType, "", null, null, CSUtility.Helper.enCSType.Server)))
                    {
                        HasStateMethodOVerride = true;
                        return;
                    }

                    if (methodDelegate.Key.Contains(AIEditor.FSMTemplateInfo.GetMethodDelegateDictionaryKey(mStateType, "", null, null, CSUtility.Helper.enCSType.Client)))
                    {
                        HasStateMethodOVerride = true;
                        return;
                    }
                }

                HasStateMethodOVerride = false;
                //var statePros = hostInstance.StateMethodDelegateXmlHolders.TryGetValue()
                //if (statePros != null && statePros.Count > 0)
                //    HasStateValues = true;
                //else
                //    HasStateValues = false;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (EditorCommon.MessageBox.Show("即将删除 " + NickName + " ，无法恢复，是否确定?", "警告", EditorCommon.MessageBox.enMessageBoxButton.YesNo) == EditorCommon.MessageBox.enMessageBoxResult.Yes)
            {
                if (OnDelete != null)
                    OnDelete(this);
            }
        }

        StateMethodEditorTabItemControl mTabItem;
        private void Grid_Methods_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mHostControl == null)
                return;

            if(mTabItem != null)
            {
                if(mTabItem.Parent == null)
                    mHostControl.TabControl_Items.Items.Add(mTabItem);

                mTabItem.IsSelected = true;
                return;
            }

            mTabItem = new StateMethodEditorTabItemControl(mHostAIInstanceInfoId, mHostControl.TabControl_Items);
            mTabItem.SetBinding(StateMethodEditorTabItemControl.HeaderNameProperty, new Binding("NickName") { Source = this });
            mTabItem.Initialize(StateType, null, null, StateMethodsEditorControl.enMethodDelegateEditType.Default);
            mTabItem.OnSaveMethodInfo = UpdateHasStateMethodOverride;
            mHostControl.TabControl_Items.Items.Add(mTabItem);
            mTabItem.IsSelected = true;

            /*/ 打开窗口
            //Popup_Methods.IsOpen = true;
            StateMethodsEditorWindow window = new StateMethodsEditorWindow(mHostAIInstanceInfoId);
            //window.StateType = mStateType;
            //System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window);
            window.Title = NickName;
            window.Initialize(StateType, null, null, StateMethodsEditorControl.enMethodDelegateEditType.Default);
            window.OnSaveMethodInfo = UpdateHasStateMethodOverride;

            //var hostInstance = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceInfoId);
            //if (hostInstance != null)
            //{
            //    window.EditorControl.InitStatePropertyMenu(hostInstance.GetStatePropertys(StateType));
            //}

            window.Show();*/
        }

        private void Popup_Close_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Popup_Methods.IsOpen = false;
            e.Handled = true;
        }

        private void MenuItem_SetDefaultState_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OnSetDefaultState != null)
                OnSetDefaultState(this);

            //IsDefaultState = true;
        }

        private void Grid_Values_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //StatementValueSet.IsOpen = true;

            StateValueSetWindow win = new StateValueSetWindow(mHostAIInstanceInfoId, mStateType);
            win.OnUpdateStatePropertys = UpdateHasStateValues;
            //System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(win);
            win.Title = NickName + "属性设置";
            win.Show();
        }

        private void Button_AddProperty_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //var hostInstance = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceInfoId);
            //if (hostInstance == null)
            //    return;

            //AIEditor.FSMTemplateInfo.StatePropertyInfo proInfo = new AIEditor.FSMTemplateInfo.StatePropertyInfo();
            //hostInstance.AddStateProperty(mStateType, proInfo);
        }

        private void Button_DelProperty_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (ListView_StatePropertys.SelectedIndex < 0)
            //    return;

            //var hostInstance = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mHostAIInstanceInfoId);
            //if (hostInstance == null)
            //    return;

            //hostInstance.RemoveStateProperty(mStateType, ListView_StatePropertys.SelectedIndex);
        }

        private void TextBlock_NickName_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBox_NickName.Visibility = System.Windows.Visibility.Visible;
            Keyboard.Focus(TextBox_NickName);
            TextBox_NickName.SelectAll();
        }

        private void TextBox_NickName_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox_NickName.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
