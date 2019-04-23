using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace MainEditor.PluginAssist
{
    /// <summary>
    /// Interaction logic for PluginManagerWindow.xaml
    /// </summary>
    public partial class PluginManagerWindow : DockControl.Controls.DockAbleWindowBase
    {
        static PluginManagerWindow smInstance = new PluginManagerWindow();
        public static PluginManagerWindow Instance
        {
            get { return smInstance; }
        }


        ObservableCollection<PluginItem> mPluginItems = new ObservableCollection<PluginItem>();
        public ObservableCollection<PluginItem> PluginItems
        {
            get { return mPluginItems; }
            set
            {
                mPluginItems = value;
            }
        }

        private PluginManagerWindow()
        {
            InitializeComponent();

            EditorCommon.PluginAssist.PluginManager.Instance.GenericPluginItemAction = GenericPluginItemAction;
            LayoutManaged = false;
            CanClose = false;
        }

        private void GenericPluginItemAction(Guid pluginGuid, EditorCommon.PluginAssist.IEditorPlugin pluginValue, EditorCommon.PluginAssist.IEditorPluginData pluginData)
        {
            this.Dispatcher.Invoke(() =>
            {
                var item = new PluginItem(pluginGuid, pluginValue, pluginData);
                item.PluginName = pluginValue.PluginName;
                item.Version = pluginValue.Version;
                PluginItems.Add(item);

                //if (!plugin.Value.OnActive())
                //{
                //    item.Active = false;
                //    continue;
                //}
                var atts = pluginValue.GetType().GetCustomAttributes(typeof(EditorCommon.PluginAssist.PluginMenuItemAttribute), false);
                if (atts.Length > 0)
                {
                    var piAtt = (EditorCommon.PluginAssist.PluginMenuItemAttribute)(atts[0]);
                    MainEditor.MainWindow.Instance.SetPluginMenu(piAtt.MenuString, item);
                }

                // 将同类型的插件放入选择器字典表中
                EditorCommon.PluginAssist.PluginSelector selector;
                if (!EditorCommon.PluginAssist.PluginManager.Instance.PluginDicWithTypeKey.TryGetValue(item.PluginData.PluginType, out selector))
                {
                    selector = new EditorCommon.PluginAssist.PluginSelector(item.PluginData.PluginType);
                    EditorCommon.PluginAssist.PluginManager.Instance.PluginDicWithTypeKey[item.PluginData.PluginType] = selector;
                }
                selector.Plugins[pluginGuid] = item;

                EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey[pluginGuid] = item;
            });
        }

        public PluginItem GetPluginItem(string pluginType)
        {
            EditorCommon.PluginAssist.PluginSelector selector;
            if (!EditorCommon.PluginAssist.PluginManager.Instance.PluginDicWithTypeKey.TryGetValue(pluginType, out selector))
                return null;

            Dictionary<Guid, PluginItem> activedPlufinItems = new Dictionary<Guid, PluginItem>();

            foreach (var i in selector.Plugins)
            {
                if (i.Value.Active)
                {
                    activedPlufinItems[i.Key] = i.Value as PluginItem;
                }
            }

            if (activedPlufinItems.Count == 0)
                return null;

            if (selector.DefaultPlugin == null || !selector.DefaultPlugin.Active)
            {
                if (activedPlufinItems.Count == 1)
                {
                    foreach (var obj in activedPlufinItems)
                    {
                        return obj.Value;
                    }
                }
                else
                {
                    // 弹出窗口选择本次处理要使用的插件
                    var dsw = new PluginDefaultSelectorWindow();
                    foreach (var obj in activedPlufinItems)
                    {
                        dsw.PluginItems.Add(obj.Value);
                    }
                    dsw.ShowDialog();
                    if (dsw.NeverShow)
                    {
                        selector.SetDefaultPlugin(dsw.SelectedItem.Id);
                        return selector.DefaultPlugin as PluginItem;
                    }
                    else
                        return dsw.SelectedItem;
                }
            }
            else
                return selector.DefaultPlugin as PluginItem;

            return null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ListView_Plugins.SelectedIndex < 0 || ListView_Plugins.SelectedIndex >= PluginItems.Count)
                return;

            Grid_Instruction.Children.Clear();
            Grid_Instruction.Children.Add(PluginItems[ListView_Plugins.SelectedIndex].InstructionControl);

            pluginInfo.Instance = PluginItems[ListView_Plugins.SelectedIndex];
        }

        private void Button_Click_Delete(object sender, System.Windows.RoutedEventArgs e)
        {
            if (EditorCommon.MessageBox.enMessageBoxResult.Yes == EditorCommon.MessageBox.Show("确定要删除？", "提示", EditorCommon.MessageBox.enMessageBoxButton.YesNo))
            {
                var btn = sender as Button;
                var id = (Guid)btn.Tag;
                if (EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey.ContainsKey(id))
                {
                    foreach (var i in EditorCommon.PluginAssist.PluginManager.Instance.Catalog.Catalogs)
                    {
                        var cata = i as AssemblyCatalog;
                        if (cata == null)
                            continue;
                        if (cata.Assembly.Location == EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey[id].AssemblyPath)
                        {
                            EditorCommon.PluginAssist.PluginManager.Instance.Catalog.Catalogs.Remove(cata);
                            break;
                        }
                    }

                    if (EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey[id].HostMeuItem != null)
                    {
                        var menuItem = EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey[id].HostMeuItem.Parent as MenuItem;
                        if (menuItem != null)
                            menuItem.Items.Remove(EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey[id].HostMeuItem);
                    }

                    if (EditorCommon.PluginAssist.PluginManager.Instance.PluginDicWithTypeKey.ContainsKey(EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey[id].PluginData.PluginType))
                    {
                        EditorCommon.PluginAssist.PluginManager.Instance.PluginDicWithTypeKey[EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey[id].PluginData.PluginType].Plugins.Remove(id);
                    }

                    var delObj = EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey[id].PluginObject as EditorCommon.PluginAssist.IEditorPluginOperation;
                    if (delObj != null)
                    {
                        delObj.Delete();                                                               
                    }

                    mPluginItems.Remove(EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey[id] as PluginItem);
                    EditorCommon.PluginAssist.PluginManager.Instance.PluginsDicWithIdKey.Remove(id);

                    EditorCommon.PluginAssist.PluginManager.Instance.AddDelAssembly(delObj.AssemblyPath);
                    EditorCommon.PluginAssist.PluginManager.Instance.SavePluginInfo();                 
                }
            }
        }

        private void Button_Click_New(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "|*.dll";               
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {                            
                if (EditorCommon.PluginAssist.PluginManager.Instance.AssemblyCatalogExist(ofd.FileName))
                {
                    EditorCommon.MessageBox.Show("该插件已加载！","提示");
                    return;
                }

                var file = CSUtility.Support.IFileManager.Instance.Bin + "Plugins/" + ofd.SafeFileName;
                if (!System.IO.File.Exists(file))
                {
                    System.IO.File.Copy(ofd.FileName, file);
                }                

                var cata = new AssemblyCatalog(System.Reflection.Assembly.LoadFrom(file));
                EditorCommon.PluginAssist.PluginManager.Instance.Catalog.Catalogs.Add(cata);

                EditorCommon.PluginAssist.PluginManager.Instance.DelAssembly(ofd.FileName);
                EditorCommon.PluginAssist.PluginManager.Instance.GeneratePlugins(0, null);
            }
        }


              
    }
}
