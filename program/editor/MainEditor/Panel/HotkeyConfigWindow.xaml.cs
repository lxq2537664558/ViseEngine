using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MainEditor.Panel
{
    /// <summary>
    /// HotkeyConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HotkeyConfigWindow : DockControl.Controls.DockAbleWindowBase
    {
        static HotkeyConfigWindow smInstance = new HotkeyConfigWindow();
        public static HotkeyConfigWindow Instance
        {
            get { return smInstance; }
        }

        KeyButton[] mKeyButtons = new KeyButton[(int)(CCore.MsgProc.BehaviorParameter.enKeys.Count)];

        enum enEditState
        {
            PreView,
            ModifyKey,
        }

        enEditState mEditState = enEditState.PreView;

        public HotkeyConfigWindow()
        {
            InitializeComponent();

            this.Owner = MainEditor.MainWindow.Instance;
            LayoutManaged = false;
            CanClose = false;
            InitializeKeyButtons();

            //Test only/////////////////////////////////////////////////////////////////////////////////////
            //EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("默认", "XXX", "111111111111111111111111111", CCore.MsgProc.BehaviorParameter.enKeys.E, true, false, false, EditorCommon.Hotkey.HotkeyManager.enMouseType.None, 
            //    new Action<CCore.MsgProc.BehaviorParameter, object>((param, obj)=>
            //    {
            //        EditorCommon.MessageBox.Show("默认-XXX");
            //    }));
            //EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("默认", "aaa", "3333333333333333333333", CCore.MsgProc.BehaviorParameter.enKeys.Q, true, false, false, EditorCommon.Hotkey.HotkeyManager.enMouseType.None, 
            //    new Action<CCore.MsgProc.BehaviorParameter, object>((param, obj)=>
            //    {
            //        EditorCommon.MessageBox.Show("默认-aaa");
            //    }));
            //EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("默认", "bbb", "66666666666666666666", CCore.MsgProc.BehaviorParameter.enKeys.X, false, false, false, EditorCommon.Hotkey.HotkeyManager.enMouseType.None, 
            //    new Action<CCore.MsgProc.BehaviorParameter, object>((param, obj)=>
            //    {
            //        EditorCommon.MessageBox.Show("默认-bbb");
            //    }));
            //EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup("默认");
            ///////////////////////////////////////////////////////////////////////////////////////
        }

        // 初始化按键列表
        private void InitializeKeyButtons()
        {
            foreach(var keyName in System.Enum.GetNames(typeof(CCore.MsgProc.BehaviorParameter.enKeys)))
            {
                var btn = this.FindName("Btn_" + keyName) as KeyButton;
                if(btn != null)
                {
                    var key = (int)(System.Enum.Parse(typeof(CCore.MsgProc.BehaviorParameter.enKeys), keyName));
                    mKeyButtons[key] = btn;
                }
            }
        }

        private void DockAbleWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            //ListBox_Groups.ItemsSource = EditorCommon.Hotkey.HotkeyManager.Instance.GroupCollection;
        }        

        private void DockAbleWindowBase_KeyDown(object sender, KeyEventArgs e)
        {
            // 测试 ///////////////////////////////////////////////////////////////////
            //var key = new CCore.MsgProc.Behavior.KB_Char();
            //key.Key = CCore.MsgProc.BehaviorParameter.WindowsKeyToKey(e.Key);
            //key.behavior = CCore.MsgProc.BehaviorType.BHT_KB_Char_Down;
            //EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(key, null);

            //if(mKeyButtons[(int)(key.Key)] != null)
            //{
            //    mKeyButtons[(int)(key.Key)].IsChecked = true;
            //}
            ///////////////////////////////////////////////////////////////////////////
        }
        private void DockAbleWindowBase_KeyUp(object sender, KeyEventArgs e)
        {
            // 测试 ///////////////////////////////////////////////////////////////////
            //var key = CCore.MsgProc.BehaviorParameter.WindowsKeyToKey(e.Key);

            //if(mKeyButtons[(int)(key)] != null)
            //{
            //    mKeyButtons[(int)(key)].IsChecked = false;
            //}
            ///////////////////////////////////////////////////////////////////////////
        }

        private void SetKeyButtonToState(KeyButton btn, KeyButton.enKeyButtonState state, bool isMouse)
        {
            if (btn == null)
                return;

            btn.ButtonState = state;
        }

        EditorCommon.Hotkey.HotkeyGroup mCurrentSelectedGroup;
        private void ListBox_Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Groups.SelectedIndex < 0)
                return;

            mCurrentSelectedGroup = ListBox_Groups.SelectedItem as EditorCommon.Hotkey.HotkeyGroup;
            ListBox_Commands.ItemsSource = mCurrentSelectedGroup.CommandCollection;

            ShowGroupOccupancy(mCurrentSelectedGroup);
        }

        // 显示group的快捷键占用
        private void ShowGroupOccupancy(EditorCommon.Hotkey.HotkeyGroup group)
        {
            if (group == null)
                return;

            SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Ctrl, KeyButton.enKeyButtonState.Normal);
            SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Alt, KeyButton.enKeyButtonState.Normal);
            SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Shift, KeyButton.enKeyButtonState.Normal);
            SetKeyButtonToState(Btn_MouseWheel, KeyButton.enKeyButtonState.Normal, true);
            SetKeyButtonToState(Btn_MouseMove, KeyButton.enKeyButtonState.Normal, true);

            foreach(var btn in mKeyButtons)
            {
                if (btn == null)
                    continue;

                btn.ButtonState = KeyButton.enKeyButtonState.Normal;
            }

            bool ctrlOccupancy = false;
            bool altOccupancy = false;
            bool shiftOccupancy = false;
            bool mouseWheelOccupancy = false;
            bool mouseMoveOccupancy = false;
            foreach (var command in group.CommandCollection)
            {
                SetKeyState(command.Hotkey, KeyButton.enKeyButtonState.Occupancy);

                if (command.WithCtrl)
                    ctrlOccupancy = true;
                if (command.WithAlt)
                    altOccupancy = true;
                if (command.WithShift)
                    shiftOccupancy = true;
                if (command.MouseType == EditorCommon.Hotkey.enMouseType.MouseWheel)
                    mouseWheelOccupancy = true;
                if (command.MouseType == EditorCommon.Hotkey.enMouseType.MouseMove)
                    mouseMoveOccupancy = true;
            }

            if (ctrlOccupancy)
            {
                SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Ctrl, KeyButton.enKeyButtonState.Occupancy);
            }
            if (altOccupancy)
            {
                SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Alt, KeyButton.enKeyButtonState.Occupancy);
            }
            if (shiftOccupancy)
            {
                SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Shift, KeyButton.enKeyButtonState.Occupancy);
            }
            if (mouseWheelOccupancy)
            {
                SetKeyButtonToState(Btn_MouseWheel, KeyButton.enKeyButtonState.Occupancy, true);
            }
            if (mouseMoveOccupancy)
            {
                SetKeyButtonToState(Btn_MouseMove, KeyButton.enKeyButtonState.Occupancy, true);
            }
        }

        private void ShowGroupOccupancy(EditorCommon.Hotkey.HotkeyGroup group, bool withCtrl, bool withAlt, bool withShift, EditorCommon.Hotkey.enMouseType mouseType)
        {
            if (group == null)
                return;

            //SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Ctrl, KeyButton.enKeyButtonState.Normal);
            //SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Alt, KeyButton.enKeyButtonState.Normal);
            //SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Shift, KeyButton.enKeyButtonState.Normal);
            //SetKeyButtonToState(Btn_MouseWheel, KeyButton.enKeyButtonState.Normal, true);
            //SetKeyButtonToState(Btn_MouseMove, KeyButton.enKeyButtonState.Normal, true);

            foreach (var command in group.CommandCollection)
            {
                if (command == mCurrentSelectedCommand)
                    continue;

                if (mKeyButtons[(int)(command.Hotkey)] == null)
                    continue;

                if (command.WithCtrl == withCtrl &&
                   command.WithAlt == withAlt &&
                   command.WithShift == withShift &&
                   command.MouseType == mouseType)
                {
                    SetKeyState(command.Hotkey, KeyButton.enKeyButtonState.Occupancy);
                }
                else
                    SetKeyState(command.Hotkey, KeyButton.enKeyButtonState.Normal);
            }
        }

        private void SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys key, KeyButton.enKeyButtonState state)
        {

            switch (key)
            {
                case CCore.MsgProc.BehaviorParameter.enKeys.LButton:
                case CCore.MsgProc.BehaviorParameter.enKeys.RButton:
                case CCore.MsgProc.BehaviorParameter.enKeys.MButton:
                case CCore.MsgProc.BehaviorParameter.enKeys.XButton1:
                case CCore.MsgProc.BehaviorParameter.enKeys.XButton2:
                    {
                        if (mKeyButtons[(int)key] == null)
                            return;
                        SetKeyButtonToState(mKeyButtons[(int)key], state, true);
                    }
                    break;

                case CCore.MsgProc.BehaviorParameter.enKeys.Ctrl:
                case CCore.MsgProc.BehaviorParameter.enKeys.LeftCtrl:
                case CCore.MsgProc.BehaviorParameter.enKeys.RightCtrl:
                    SetKeyButtonToState(Btn_LeftCtrl, state, false);
                    SetKeyButtonToState(Btn_RightCtrl, state, false);
                    break;

                case CCore.MsgProc.BehaviorParameter.enKeys.Alt:
                case CCore.MsgProc.BehaviorParameter.enKeys.LeftAlt:
                case CCore.MsgProc.BehaviorParameter.enKeys.RightAlt:
                    SetKeyButtonToState(Btn_LeftAlt, state, false);
                    SetKeyButtonToState(Btn_RightAlt, state, false);
                    break;

                case CCore.MsgProc.BehaviorParameter.enKeys.Shift:
                case CCore.MsgProc.BehaviorParameter.enKeys.LeftShift:
                case CCore.MsgProc.BehaviorParameter.enKeys.RightShift:
                    SetKeyButtonToState(Btn_LeftShift, state, false);
                    SetKeyButtonToState(Btn_RightShift, state, false);
                    break;

                default:
                    {
                        if (mKeyButtons[(int)key] == null)
                            return;
                        SetKeyButtonToState(mKeyButtons[(int)key], state, false);
                    }
                    break;
            }
        }

        private void ShowCommandHotkeySelected(EditorCommon.Hotkey.HotkeyCommand command)
        {
            if(command.Hotkey != CCore.MsgProc.BehaviorParameter.enKeys.None)
                SetKeyState(command.Hotkey, KeyButton.enKeyButtonState.Selected);

            if (command.WithCtrl)
            {
                SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Ctrl, KeyButton.enKeyButtonState.Selected);
            }
            if (command.WithAlt)
            {
                SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Alt, KeyButton.enKeyButtonState.Selected);
            }
            if (command.WithShift)
            {
                SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Shift, KeyButton.enKeyButtonState.Selected);
            }
            if (command.MouseType == EditorCommon.Hotkey.enMouseType.MouseWheel)
            {
                SetKeyButtonToState(Btn_MouseWheel, KeyButton.enKeyButtonState.Selected, true);
            }
            if (command.MouseType == EditorCommon.Hotkey.enMouseType.MouseMove)
            {
                SetKeyButtonToState(Btn_MouseMove, KeyButton.enKeyButtonState.Selected, true);
            }
        }

        EditorCommon.Hotkey.HotkeyCommand mCurrentSelectedCommand;
        private void ListBox_Commands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Commands.SelectedIndex < 0)
                return;

            mCurrentSelectedCommand = ListBox_Commands.SelectedItem as EditorCommon.Hotkey.HotkeyCommand;
            TextBlock_Description.Text = mCurrentSelectedCommand.Description;
            BindingOperations.ClearBinding(TextBlock_Hotkey, TextBlock.TextProperty);
            BindingOperations.SetBinding(TextBlock_Hotkey, TextBlock.TextProperty, new Binding("HotkeyString") { Source = mCurrentSelectedCommand });

            ShowGroupOccupancy(mCurrentSelectedGroup);
            ShowCommandHotkeySelected(mCurrentSelectedCommand);
        }

        private void Button_ResetHotkey_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentSelectedGroup == null || mCurrentSelectedCommand == null)
                return;

            foreach (var btn in mKeyButtons)
            {
                if (btn == null)
                    continue;
                btn.IsEnabled = true;
            }
            Btn_MouseMove.IsEnabled = true;
            Btn_MouseWheel.IsEnabled = true;

            SetKeyState(mCurrentSelectedCommand.Hotkey, KeyButton.enKeyButtonState.Normal);
            ShowGroupOccupancy(mCurrentSelectedGroup, false, false, false, EditorCommon.Hotkey.enMouseType.None);
            SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Ctrl, KeyButton.enKeyButtonState.Normal);
            SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Alt, KeyButton.enKeyButtonState.Normal);
            SetKeyState(CCore.MsgProc.BehaviorParameter.enKeys.Shift, KeyButton.enKeyButtonState.Normal);
            SetKeyButtonToState(Btn_MouseWheel, KeyButton.enKeyButtonState.Normal, true);
            SetKeyButtonToState(Btn_MouseMove, KeyButton.enKeyButtonState.Normal, true);

            mEditState = enEditState.ModifyKey;

            Border_SettingHotkey.Visibility = Visibility.Visible;
        }

        private void Button_ClearHotkey_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentSelectedGroup == null || mCurrentSelectedCommand == null)
                return;

            SetKeyState(mCurrentSelectedCommand.Hotkey, KeyButton.enKeyButtonState.Normal);
            EditorCommon.Hotkey.HotkeyManager.Instance.ClearCommandHotkey(mCurrentSelectedGroup.GroupName, mCurrentSelectedCommand.CommandName);
            ShowGroupOccupancy(mCurrentSelectedGroup);
        }

        private void ToggleButton_Key_StateChanged(KeyButton.enKeyButtonState state, KeyButton keyButton)
        {
            if(mEditState == enEditState.ModifyKey && state == KeyButton.enKeyButtonState.Setting)
            { 
                mEditState = enEditState.PreView;
                
                foreach (var btn in mKeyButtons)
                {
                    if (btn == null)
                        continue;
                    btn.IsEnabled = false;
                }
                Btn_MouseMove.IsEnabled = false;
                Btn_MouseWheel.IsEnabled = false;

                var key = (CCore.MsgProc.BehaviorParameter.enKeys)(System.Enum.Parse(typeof(CCore.MsgProc.BehaviorParameter.enKeys), keyButton.Name.Replace("Btn_", "")));
                EditorCommon.Hotkey.HotkeyManager.Instance.ChangeCommandHotkey(mCurrentSelectedGroup.GroupName, mCurrentSelectedCommand.CommandName, key, mWithCtrl, mWithShift, mWithAlt, mMouseType);

                ShowGroupOccupancy(mCurrentSelectedGroup);
                ShowCommandHotkeySelected(mCurrentSelectedCommand);
                Border_SettingHotkey.Visibility = Visibility.Collapsed;

                EditorCommon.Hotkey.HotkeyManager.Instance.SaveHotkeyConfig();
            }
        }

        bool mWithCtrl = false;
        private void ToggleButton_KeyCtrl_StateChanged(KeyButton.enKeyButtonState state, KeyButton keyButton)
        {
            if(mEditState == enEditState.ModifyKey)
            {
                switch(state)
                {
                    case KeyButton.enKeyButtonState.Setting:
                        mWithCtrl = true;
                        break;
                    default:
                        mWithCtrl = false;
                        break;
                }
                ShowGroupOccupancy(mCurrentSelectedGroup, mWithCtrl, mWithAlt, mWithShift, mMouseType);
            }
        }
        bool mWithAlt = false;
        private void ToggleButton_KeyAlt_StateChanged(KeyButton.enKeyButtonState state, KeyButton keyButton)
        {
            if (mEditState == enEditState.ModifyKey)
            {
                switch (state)
                {
                    case KeyButton.enKeyButtonState.Setting:
                        mWithAlt = true;
                        break;
                    default:
                        mWithAlt = false;
                        break;
                }
                ShowGroupOccupancy(mCurrentSelectedGroup, mWithCtrl, mWithAlt, mWithShift, mMouseType);
            }
        }
        bool mWithShift = false;
        private void ToggleButton_KeyShift_StateChanged(KeyButton.enKeyButtonState state, KeyButton keyButton)
        {
            if (mEditState == enEditState.ModifyKey)
            {
                switch (state)
                {
                    case KeyButton.enKeyButtonState.Setting:
                        mWithShift = true;
                        break;
                    default:
                        mWithShift = false;
                        break;
                }
                ShowGroupOccupancy(mCurrentSelectedGroup, mWithCtrl, mWithAlt, mWithShift, mMouseType);
            }
        }
        EditorCommon.Hotkey.enMouseType mMouseType = EditorCommon.Hotkey.enMouseType.None;
        private void ToggleButton_MouseWheel_StateChanged(KeyButton.enKeyButtonState state, KeyButton keyButton)
        {
            if (mEditState == enEditState.ModifyKey)
            {
                if (state == KeyButton.enKeyButtonState.Setting)
                {
                    Btn_MouseMove.ButtonState = KeyButton.enKeyButtonState.Normal;
                    mMouseType = EditorCommon.Hotkey.enMouseType.MouseWheel;
                }
                else
                    mMouseType = EditorCommon.Hotkey.enMouseType.None;

                mEditState = enEditState.PreView;
                foreach (var btn in mKeyButtons)
                {
                    if (btn == null)
                        continue;
                    btn.IsEnabled = false;
                }
                Btn_MouseMove.IsEnabled = false;
                Btn_MouseWheel.IsEnabled = false;

                EditorCommon.Hotkey.HotkeyManager.Instance.ChangeCommandHotkey(mCurrentSelectedGroup.GroupName, mCurrentSelectedCommand.CommandName, CCore.MsgProc.BehaviorParameter.enKeys.None, mWithCtrl, mWithShift, mWithAlt, mMouseType);

                ShowGroupOccupancy(mCurrentSelectedGroup);
                ShowCommandHotkeySelected(mCurrentSelectedCommand);
                Border_SettingHotkey.Visibility = Visibility.Collapsed;
            }
        }
        private void ToggleButton_MouseMove_StateChanged(KeyButton.enKeyButtonState state, KeyButton keyButton)
        {
            if (mEditState == enEditState.ModifyKey)
            {
                if (state == KeyButton.enKeyButtonState.Setting)
                {
                    Btn_MouseWheel.ButtonState = KeyButton.enKeyButtonState.Normal;
                    mMouseType = EditorCommon.Hotkey.enMouseType.MouseMove;
                }
                else
                    mMouseType = EditorCommon.Hotkey.enMouseType.None;

                ShowGroupOccupancy(mCurrentSelectedGroup, mWithCtrl, mWithAlt, mWithShift, mMouseType);
            }
        }

        private void Button_CancelEditHotkey_Click(object sender, RoutedEventArgs e)
        {
            mEditState = enEditState.PreView;
            foreach (var btn in mKeyButtons)
            {
                if (btn == null)
                    continue;
                btn.IsEnabled = false;
            }
            Btn_MouseMove.IsEnabled = false;
            Btn_MouseWheel.IsEnabled = false;

            ShowGroupOccupancy(mCurrentSelectedGroup);
            ShowCommandHotkeySelected(mCurrentSelectedCommand);
            Border_SettingHotkey.Visibility = Visibility.Collapsed;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = comboBox_Preset.SelectedItem as EditorCommon.Hotkey.PresetHotkeyGroup;
            if (item == null)
                return;

            EditorCommon.Hotkey.HotkeyManager.Instance.SetPresetHotkeyGroup(item.Name);

            ListBox_Groups.ItemsSource = item.GroupCollection;
            ListBox_Groups.SelectedIndex = 0;

            EditorCommon.Hotkey.HotkeyManager.Instance.SaveHotkeyConfig();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox_Preset.ItemsSource = EditorCommon.Hotkey.HotkeyManager.Instance.PresetCollection;
            comboBox_Preset.DisplayMemberPath = "Name";
            
            for(int i = 0; i < EditorCommon.Hotkey.HotkeyManager.Instance.PresetCollection.Count;++i)
            {
                if (EditorCommon.Hotkey.HotkeyManager.Instance.PresetCollection[i].IsDefault)
                {
                    comboBox_Preset.SelectedIndex = i;
                    break;
                }
            }
            if (comboBox_Preset.SelectedIndex == -1)
                comboBox_Preset.SelectedIndex = 0;
        }

        private void Button_Click_Del(object sender, RoutedEventArgs e)
        {
            if (comboBox_Preset.SelectedIndex == 0)
            {
                return;
            }

            var item = comboBox_Preset.SelectedItem as EditorCommon.Hotkey.PresetHotkeyGroup;
            if (item == null)
                return;
            
            if (EditorCommon.Hotkey.HotkeyManager.Instance.RemovePresetHotkeyGroup(item.Name))
            {
                comboBox_Preset.SelectedIndex = 0;
            }

            EditorCommon.Hotkey.HotkeyManager.Instance.SaveHotkeyConfig();
        }

        private void Button_Click_New(object sender, RoutedEventArgs e)
        {            
            var inputWindow = new InputWindow.InputWindow();
            inputWindow.Title = "创建快捷键预设";
            inputWindow.Description = "预设快捷键名称:";
            inputWindow.Value = GetPresetName();

            if (inputWindow.ShowDialog((value, cultureInfo) =>
            {
                if (value.ToString() == "")
                    return new ValidationResult(false, "名字不能为空");
                foreach(var i in EditorCommon.Hotkey.HotkeyManager.Instance.PresetCollection)
                {
                    if(i.Name == (string)value)
                    {
                        return new ValidationResult(false, "该名称已存在");
                    }
                }

                return new ValidationResult(true, null);
            }) == false)
                return;

            EditorCommon.Hotkey.HotkeyManager.Instance.AddPresetHotkeyGroup((string)inputWindow.Value);
            EditorCommon.Hotkey.HotkeyManager.Instance.SaveHotkeyConfig();
        }

        object GetPresetName()
        {
            string name = "设置";
            int i = 1;
            
            while (true)
            {
                bool exist = false;
                foreach (var group in EditorCommon.Hotkey.HotkeyManager.Instance.PresetCollection)
                {
                    if (name + i.ToString() == group.Name)
                    {
                        exist = true;
                        break;
                    }
                }

                if (exist)
                {
                    ++i;
                    continue;
                }

                return name + i.ToString();
            }
        }        
    }
}
