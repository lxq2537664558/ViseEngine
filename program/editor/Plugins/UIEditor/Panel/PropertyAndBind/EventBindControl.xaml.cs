using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace UIEditor.Panel.PropertyAndBind
{
    /// <summary>
    /// Interaction logic for EventBindControl.xaml
    /// </summary>
    public partial class EventBindControl : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        System.Reflection.EventInfo mEvent = null;
        public System.Reflection.EventInfo Event
        {
            get { return mEvent; }
            set
            {
                mEvent = value;

                if (mEvent != null)
                {
                    EventName = mEvent.Name;
                    System.Reflection.MethodInfo invoke = mEvent.EventHandlerType.GetMethod("Invoke");
                    ParameterInfos = invoke.GetParameters();
                    var bindClassList = CCore.Support.ReflectionManager.Instance.GetBindClassInfosWithEvent(Event);
                    //FindClassWithEvent(mEvent);

                    ComboBox_Class.Items.Clear();
                    ComboBox_Class.Items.Add("None");
                    ComboBox_Method.Items.Clear();
                    foreach (var bindClassInfo in bindClassList)
                    {
                        ComboBox_Class.Items.Add(bindClassInfo.ClassType.FullName);
                    }

                    string dllName = "", className = "", methodName = "";
                    mHostWinBase.GetBindingEventInfoFromEventName(EventName, ref dllName, ref className, ref methodName);
                    ComboBox_Class.SelectedItem = className;
                    ComboBox_Method.SelectedItem = methodName;
                }

                OnPropertyChanged("Event");
            }
        }

        string mEventName = "";
        public string EventName
        {
            get { return mEventName; }
            set
            {
                mEventName = value;
                OnPropertyChanged("EventName");
            }
        }

        System.Reflection.ParameterInfo[] mParameterInfos = null;
        public System.Reflection.ParameterInfo[] ParameterInfos
        {
            get { return mParameterInfos; }
            set
            {
                mParameterInfos = value;

                StackPanel_Params.Children.Clear();
                if (mParameterInfos.Length == 0)
                {
                    StackPanel_Params.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    StackPanel_Params.Visibility = System.Windows.Visibility.Visible;
                    foreach (var par in mParameterInfos)
                    {
                        TextBlock text = new TextBlock();
                        text.Text = par.Name + " (" + par.ParameterType + ")";
                        StackPanel_Params.Children.Add(text);
                    }
                }
            }
        }

        //protected class BindClassInfo
        //{
        //    public Type ClassType;
        //    public List<System.Reflection.MethodInfo> MethodInfos = new List<System.Reflection.MethodInfo>();
        //}
        //protected Dictionary<string, UISystem.BindClassInfo> mBindingClassDictionary = new Dictionary<string, UISystem.BindClassInfo>();

        protected UISystem.WinBase mHostWinBase = null;

        public EventBindControl(Object obj)
        {
            InitializeComponent();

            mHostWinBase = obj as UISystem.WinBase;
        }

        //private void FindClassWithEvent(System.Reflection.EventInfo info)
        //{
        //    var evtInvoke = info.EventHandlerType.GetMethod("Invoke");
        //    var evtParamInfos = evtInvoke.GetParameters();

        //    mBindingClassDictionary.Clear();

        //    var types = Program.GetTypes(Program.enAssemblyType.FrameSet);
        //    foreach (var classType in types)
        //    {
        //        var methods = classType.GetMethods();
        //        foreach (var method in methods)
        //        {
        //            var attributes = method.GetCustomAttributes(typeof(EditorCommon.Assist.UIEditor_BindingMethodAttribute), true);
        //            if (attributes.Length <= 0)
        //                continue;

        //            var methodParams = method.GetParameters();
        //            if (methodParams.Length != evtParamInfos.Length)
        //                continue;

        //            bool isEqual = true;
        //            for (int i = 0; i < methodParams.Length; i++)
        //            {
        //                if (methodParams[i].ParameterType != evtParamInfos[i].ParameterType)
        //                {
        //                    isEqual = false;
        //                    break;
        //                }
        //            }

        //            if (isEqual == false)
        //                continue;

        //            UISystem.BindClassInfo bindClassInfo = null;
        //            if (!mBindingClassDictionary.TryGetValue(classType.FullName, out bindClassInfo))
        //            {
        //                bindClassInfo = new UISystem.BindClassInfo();
        //                bindClassInfo.ClassType = classType;
        //                mBindingClassDictionary[classType.FullName] = bindClassInfo;
        //            }

        //            bindClassInfo.MethodInfos.Add(method);
        //        }
        //    }
        //}

        private void ComboBox_Class_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox_Method.Items.Clear();

            //if (ComboBox_Class.SelectedItem == null)
            //    return;
            if (ComboBox_Class.SelectedIndex < 0)
                return;

            //var selectString = ComboBox_Class.SelectedItem.ToString();

            var bindClassList = CCore.Support.ReflectionManager.Instance.GetBindClassInfosWithEvent(Event);
            if (ComboBox_Class.SelectedIndex > bindClassList.Count)
            {
                ComboBox_Class.SelectedIndex = -1;
                return;
            }

            if (ComboBox_Class.SelectedIndex == 0)
            {
                ComboBox_Method.Items.Clear();
            }
            else
            {
                CCore.Support.BindClassInfo info = bindClassList[ComboBox_Class.SelectedIndex - 1];
                //if (mBindingClassDictionary.TryGetValue(selectString, out info))
                //{
                    foreach (var methodInfo in info.MethodInfos)
                    {
                        ComboBox_Method.Items.Add(methodInfo.Name);
                    }
                //}
            }
        }

        private void ComboBox_Method_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (mHostWinBase == null || ComboBox_Class.SelectedIndex < 0 || ComboBox_Method.SelectedIndex < 0)
                return;

            //UISystem.BindClassInfo info = null;
            //if(!mBindingClassDictionary.TryGetValue(ComboBox_Class.SelectedItem.ToString(), out info))
            //    return;
            var bindClassList = CCore.Support.ReflectionManager.Instance.GetBindClassInfosWithEvent(Event);
            if (ComboBox_Class.SelectedIndex > bindClassList.Count)
            {
                ComboBox_Class.SelectedIndex = -1;
                return;
            }
            var info = bindClassList[ComboBox_Class.SelectedIndex - 1];

            var dllKeyName = CSUtility.Program.GetAnalyseAssemblyKeyName(CSUtility.Helper.enCSType.All, CSUtility.Program.CurrentPlatform, info.ClassType.Assembly);
            mHostWinBase.SetBindingEventInfo(EventName, dllKeyName, info.ClassType.FullName, ComboBox_Method.SelectedItem.ToString());
        }
    }
}
