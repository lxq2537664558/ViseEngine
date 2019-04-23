using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace GMCommand
{
    /// <summary>
    /// Interaction logic for GMCommand.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "GMCommand")]
    [EditorCommon.PluginAssist.PluginMenuItem("调试(_D)/GM指令")]
    [Guid("9379CDFF-C8C3-437E-9C72-C89A92F14236")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class GMCommandControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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

        public string PluginName
        {
            get { return "GMCommand"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "GMCommand",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void Tick()
        {
        }

        public void SetObjectToEdit(object[] obj)
        {
        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        //////////////////////////////////////////////////////////////////

        string mExplain = "";
        public string Explain
        {
            get { return mExplain; }
            set
            {
                mExplain = value;
                OnPropertyChanged("Explain");
            }
        }

        public GMCommandControl()
        {
            InitializeComponent();

            foreach (var akey in CCore.Support.GMCommandManager.Instance.Commands.Keys)
                OrderList.Items.Add(akey);
        }

        private void Button_Send_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string strSecOrder = SecOrder.Text;

            if (OrderList.SelectedIndex >= 0)
            {
                strSecOrder.Replace(" ", "");
                var args = strSecOrder.Split(',');

                CCore.Support.GMCommand gmCommand;
                if (CCore.Support.GMCommandManager.Instance.Commands.TryGetValue(OrderList.Text, out gmCommand))
                {
                    var parameters = gmCommand.CommandMethod.GetParameters();
                    if (!((args == null && parameters.Length == 0) || (args != null && parameters.Length == args.Length)))
                    {
                        EditorCommon.MessageBox.Show("无法执行GM指令，参数数量不匹配", "错误");
                        return;
                    }

                    try
                    {
                        var argParams = new object[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            argParams[i] = System.Convert.ChangeType(args[i], parameters[i].ParameterType);
                        }
                    }
                    catch (System.Exception exp)
                    {
                        EditorCommon.MessageBox.Show("无法执行GM指令，参数类型不匹配\r\n" + exp.ToString(), "错误");
                        return;
                    }

                    CCore.Support.GMCommandManager.Instance.GMExecute(OrderList.Text, args);
                }


            }
        }

        private void OrderList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OrderList.SelectedIndex < 0)
                return;

            var key = OrderList.Items[OrderList.SelectedIndex].ToString();

            CCore.Support.GMCommand gmCommand;
            if (CCore.Support.GMCommandManager.Instance.Commands.TryGetValue(key, out gmCommand))
            {
                Explain = gmCommand.Description;
                var parameters = gmCommand.CommandMethod.GetParameters();
                if(parameters.Length > 0)
                {
                    Explain += "\r\n参数：";
                    foreach (var param in parameters)
                    {
                        Explain += param.ParameterType + " " + param.Name + ", ";
                    }

                    Explain = Explain.Remove(Explain.Length - 2);
                    Explain += "\r\n输入参数之间使用 , 分隔";
                }
            }
        }
    }
}
