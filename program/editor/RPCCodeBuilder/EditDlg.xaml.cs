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
using System.Windows.Shapes;

namespace RPCCodeBuilder
{
    /// <summary>
    /// SelectDlg.xaml 的交互逻辑
    /// </summary>
    public partial class EditDlg : DockControl.Controls.DockAbleWindowBase
    {
        public EditDlg()
        {
            InitializeComponent();
        }

        public delegate void FRefreshProject(object sender, RoutedEventArgs e);
        public FRefreshProject RefreshProject;

        public void Init(string[] items)
        {
            if (items == null)
                return;
                        
            foreach(var i in items)
            {
                listBox.Items.Add(i);
            }            
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {                        
            propertGrid.Instance = listBox.SelectedItem;
        }

        private void Button_Click_OK(object sender, RoutedEventArgs e)
        {
            Config.Instance.Save(Config.Instance.ConfigFile);

            this.Close();
        }        

        private void Button_Click_Cancle(object sender, RoutedEventArgs e)
        {            
            if (RefreshProject != null)
            {
                RefreshProject(null,null);
            }
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (RefreshProject != null)
            {
                RefreshProject(null,null);
            }
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            var proj = new ProjectConfig();
            proj.ProjectName = GetNewName();
            Config.Instance.ProjConfigList.Add(proj);

            listBox.Items.Refresh();
            listBox.SelectedItem = proj;
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            var proj = listBox.SelectedItem as ProjectConfig;
            Config.Instance.ProjConfigList.Remove(proj);          
            
            listBox.Items.Refresh();
        }

        string GetNewName()
        {
            int i = 1;
            ProjectConfig proj = null;
            string name = "";
            do
            {
                name = "Project" + i.ToString();
                proj = Config.Instance.GetProject(name);
                i++;
            }
            while (proj != null);            

            return name;
        }
    }
}
