using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UIEditor.Panel.ControlsBrowser
{
    /// <summary>
    /// UITemplateBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class UITemplateBrowser : UserControl
    {
        public UITemplateBrowser()
        {
            InitializeComponent();
        }

        public void UpdateTemplateShow()
        {
            //ListBox_Templates.Items.Clear();
            TreeView_Templates.Items.Clear();
            UISystem.Template.TemplateMananger.Instance.ReloadAllTemplates();//.LoadAllTemplates();

            //foreach (var info in UISystem.Template.TemplateMananger.Instance.ControlTemplateDictionary)
            foreach (var fileInfo in UISystem.Template.TemplateMananger.Instance.TemplateIdToFileDictionary)
            {
                var dir = fileInfo.Value.Replace("/", "\\");
                dir = dir.Replace(CSUtility.Support.IFileConfig.DefaultUITemplateDirectory + "\\", "");

                var splits = dir.Split('\\');

                //foreach (var split in splits)
                if (splits.Length == 1)
                {
                    UISystem.Template.ControlTemplateInfo templateInfo = UISystem.Template.TemplateMananger.Instance.FindControlTemplate(fileInfo.Key);
                    UIControlsBrowser_Item item = new UIControlsBrowser_Item();
                    item.TemplateInfo = templateInfo;
                    TreeView_Templates.Items.Add(item);
                }
                else
                {
                    var itemsCollection = TreeView_Templates.Items;
                    for (int i = 0; i < splits.Length; i++)
                    {
                        var split = splits[i];
                        if (string.IsNullOrEmpty(split))
                            continue;

                        if (i == splits.Length - 1)
                        {
                            UISystem.Template.ControlTemplateInfo templateInfo = UISystem.Template.TemplateMananger.Instance.FindControlTemplate(fileInfo.Key);
                            UIControlsBrowser_Item item = new UIControlsBrowser_Item();
                            item.TemplateInfo = templateInfo;
                            itemsCollection.Add(item);
                        }
                        else
                        {
                            bool bFind = false;
                            foreach (var item in itemsCollection)
                            {
                                if (!(item is TreeViewItem))
                                    continue;

                                if (((TreeViewItem)item).Header.ToString() == split)
                                {
                                    itemsCollection = ((TreeViewItem)item).Items;
                                    bFind = true;
                                    break;
                                }
                            }
                            if (!bFind)
                            {
                                var treeViewItem = new TreeViewItem();
                                treeViewItem.Header = split;
                                treeViewItem.Foreground = System.Windows.Media.Brushes.LightGray;
                                treeViewItem.FontSize = 13.5;
                                itemsCollection.Add(treeViewItem);
                                itemsCollection = treeViewItem.Items;
                            }
                        }
                    }
                }

                //ListBox_Templates.Items.Add(item);
            }

        }

        private static TreeViewItem FindTreeViewItem(object obj)
        {
            DependencyObject dpObj = obj as DependencyObject;
            if (dpObj == null)
                return null;
            if (dpObj is TreeViewItem)
                return (TreeViewItem)dpObj;
            return FindTreeViewItem(VisualTreeHelper.GetParent(dpObj));
        }


        private void TreeView_Templates_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            //if (TreeView_Templates.SelectedItem == null)
            //    return;

            //ListBox_Controls.SelectedIndex = -1;
            //mSelectedTreeViewTemplateItem = FindTreeViewItem(e.NewValue);

            //var selectedItem = TreeView_Templates.SelectedItem as UIControlsBrowser_Item;
            //if (selectedItem == null)
            //    return;

            //WPG.Data.EditorContext.SelectedUIControlTemplateId = selectedItem.TemplateInfo.ControlTemplate.Id;

            //if (OnSelectedChanged != null)
            //    OnSelectedChanged(selectedItem);
        }
    }
}
