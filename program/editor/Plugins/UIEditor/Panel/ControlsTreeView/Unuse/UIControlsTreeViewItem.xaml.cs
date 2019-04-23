using System.Windows;
using System.Windows.Controls;

namespace UIEditor.Panel.ControlsTreeView
{
    /// <summary>
    /// Interaction logic for UIControlsTreeViewItem.xaml
    /// </summary>
    public partial class UIControlsTreeViewItem : TreeViewItem
    {
        public Visibility InsertLineTopVisible
        {
            get { return (Visibility)GetValue(InsertLineTopVisibleProperty); }
            set { SetValue(InsertLineTopVisibleProperty, value); }
        }
        public static readonly DependencyProperty InsertLineTopVisibleProperty = DependencyProperty.Register("InsertLineTopVisible", typeof(Visibility), typeof(UIControlsTreeViewItem), new UIPropertyMetadata());


        public UIControlsTreeViewItem()
        {
            InitializeComponent();
        }
    }
}
