using System.Windows.Controls;

namespace UIEditor.Panel.PropertyAndBind
{
    /// <summary>
    /// Interaction logic for CommandBindItem.xaml
    /// </summary>
    public partial class CommandBindItem : UserControl
    {
        UISystem.WinBase mHostControl;
        System.Reflection.EventInfo mHostEvent;

        //UISystem.WinBase mBindedWinControl;
        public CommandBindItem(UISystem.WinBase hostControl, System.Reflection.EventInfo eventInfo)
        {
            InitializeComponent();

            mHostControl = hostControl;
            mHostEvent = eventInfo;

            var formChildControls = mHostControl.GetRoot(typeof(UISystem.WinForm)).GetAllChildControls();

            var bindCommandInfos = CCore.Support.ReflectionManager.Instance.GetBindCommandInfosWithEvent(mHostControl, mHostEvent);
            foreach (var bindInfo in bindCommandInfos)
            {
                ComboBox_Controls.Items.Add(((UISystem.WinBase)bindInfo.TargetControl).NameInEditor);
            }

        }

        public void SetTarget(UISystem.WinBase ctrl, string methodName)
        {
            if (ctrl == null)
                return;

            ComboBox_Controls.SelectedItem = ctrl.NameInEditor;
            ComboBox_Methods.SelectedItem = methodName;
            //var bindCommandInfos = UISystem.UIReflectionManager.Instance.GetBindCommandInfosWithEvent(mHostControl, mHostEvent);
            //foreach (var bindInfo in bindCommandInfos)
            //{
            //    if (bindInfo.TargetControl == ctrl)
            //    {
            //        foreach ()
            //        {
            //        }
            //        bindInfo.MethodInfos
            //        break;
            //    }
            //    ComboBox_Controls.Items.Add(bindInfo.TargetControl.NameInEditor);
            //}            
        }

        private void ComboBox_Controls_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBox_Controls.SelectedIndex < 0)
                return;

            ComboBox_Methods.Items.Clear();

            var bindCommandInfos = CCore.Support.ReflectionManager.Instance.GetBindCommandInfosWithEvent(mHostControl, mHostEvent);
            foreach (var method in bindCommandInfos[ComboBox_Controls.SelectedIndex].MethodInfos)
            {
                ComboBox_Methods.Items.Add(method.Name);
            }
        }

        private void ComboBox_Methods_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(ComboBox_Controls.SelectedIndex < 0)
                return;

            if(ComboBox_Methods.SelectedIndex < 0)
                return;

            var bindCommandInfos = CCore.Support.ReflectionManager.Instance.GetBindCommandInfosWithEvent(mHostControl, mHostEvent);
            var info = bindCommandInfos[ComboBox_Controls.SelectedIndex];

            mHostControl.SetCommandBinding(mHostEvent.Name, ((UISystem.WinBase)info.TargetControl).Id, info.MethodInfos[ComboBox_Methods.SelectedIndex].Name);
        }
    }
}
