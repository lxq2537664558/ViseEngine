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
    /// CodeViewerControl.xaml 的交互逻辑
    /// </summary>
    public partial class CodeViewerTabItemControl : TabItem
    {
        public string Text_Client
        {
            get { return TextEditor_Code_Client.Text; }
            set
            {
                TextEditor_Code_Client.Text = value;

                ListView_Errors_Client.ItemsSource = AIEditor.CodeGenerate.CodeGenerator.CompileCode(value, CSUtility.Helper.enCSType.Client).Errors;
            }
        }

        public string Text_Server
        {
            get { return TextEditor_Code_Server.Text; }
            set
            {
                TextEditor_Code_Server.Text = value;

                ListView_Errors_Server.ItemsSource = AIEditor.CodeGenerate.CodeGenerator.CompileCode(value, CSUtility.Helper.enCSType.Server).Errors;
            }
        }

        TabControl mHostTab;
        public CodeViewerTabItemControl(TabControl hostTab)
        {
            InitializeComponent();

            mHostTab = hostTab;
            
            TextEditor_Code_Client.ShowLineNumbers = true;
            TextEditor_Code_Server.ShowLineNumbers = true;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            mHostTab.Items.Remove(this);
        }
    }
}
