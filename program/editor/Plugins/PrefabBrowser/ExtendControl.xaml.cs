using System.Windows;
using System.Windows.Controls;

namespace PrefabBrowser
{
    /// <summary>
    /// Interaction logic for ExtendControl.xaml
    /// </summary>
    public partial class ExtendControl : UserControl
    {
        public delegate void Delegate_OnGroup();
        public Delegate_OnGroup OnGroup;
        public delegate void Delegate_OnUnGroup();
        public Delegate_OnUnGroup OnUnGroup;
        public delegate void Delegate_SaveToResource(CCore.World.Prefab.PrefabResource res);
        public Delegate_SaveToResource OnSaveToResource;

        public ExtendControl()
        {
            InitializeComponent();
        }

        public void SetD3DViewer(FrameworkElement element)
        {
            Grid_Preview.Children.Clear();
            Grid_Preview.Children.Add(element);
        }

        public void ShowObjectProperty(object obj)
        {
            ProGrid.Instance = obj;
        }

        private void Button_Group_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OnGroup != null)
                OnGroup();
        }

        private void Button_UnGroup_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OnUnGroup != null)
                OnUnGroup();
        }

        private void Button_SaveToResource_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var win = new InputWindow.InputWindow();
            win.Description = "输入资源名称";
            win.Value = "未命名";
            if (win.ShowDialog() == false)
                return;

            if (OnSaveToResource != null)
            {
                var res = new CCore.World.Prefab.PrefabResource();
                res.PrefabName = System.Convert.ToString(win.Value);
                OnSaveToResource(res);
            }
        }
    }
}
