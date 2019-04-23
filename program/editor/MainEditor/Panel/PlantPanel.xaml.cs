using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace MainEditor.Panel
{
    /// <summary>
    /// PlantPanel.xaml 的交互逻辑
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "CommonPanel")]
    [EditorCommon.PluginAssist.PluginMenuItem("窗口/种植面板")]
    [Guid("E75ACC89-4465-42DF-9DF2-F9F6C6A7430A")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class PlantPanel : UserControl, System.ComponentModel.INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public string PluginName
        {
            get { return "种植面板"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "场景常用对象的种植操作",
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        string mFilterString = "";
        public string FilterString
        {
            get { return mFilterString; }
            set
            {
                mFilterString = value;
                
                ShowItemWithFilter(treeView_Nodes.Items, mFilterString);

                OnPropertyChanged("FilterString");
            }
        }
        private bool ShowItemWithFilter(ItemCollection items, string filter)
        {
            bool retValue = false;
            foreach(var item in items)
            {
                var pi = item as PlantItem;
                if (pi == null)
                    continue;

                if(string.IsNullOrEmpty(filter))
                {
                    pi.Visibility = System.Windows.Visibility.Visible;
                    pi.SelectString = filter;
                    ShowItemWithFilter(pi.Items, filter);
                }
                else
                {
                    if(pi.Items.Count == 0)
                    {
                        if (pi.NodeName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            pi.Visibility = System.Windows.Visibility.Visible;
                            pi.SelectString = filter;
                            retValue = true;
                        }
                        else
                            pi.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        bool bFind = ShowItemWithFilter(pi.Items, filter);
                        if (bFind == false)
                            pi.Visibility = System.Windows.Visibility.Collapsed;
                        else
                        {
                            pi.Visibility = Visibility.Visible;
                            pi.IsExpanded = true;
                            retValue = true;
                        }
                    }
                }
            }

            return retValue;
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
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

        public void Tick()
        {
        }

        public PlantPanel()
        {
            InitializeComponent();
        }

        bool mInitialized = false;
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mInitialized)
                return;

            InitialziePlantItems();
            InitializeGameWindowDragDrop();

            mInitialized = true;
        }

        private void InitialziePlantItems()
        {
            var interfaceTypeStr = typeof(CCore.EditorAssist.IPlantAbleObject).FullName;
            foreach(var assembly in CSUtility.Program.GetAnalyseAssemblys(CSUtility.Helper.enCSType.All))
            {
                foreach(var type in assembly.GetTypes())
                {
                    var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(type, "CCore.EditorAssist.PlantAbleAttribute", false);
                    if (att == null)
                        continue;

                    if(type.GetInterface(interfaceTypeStr) == null)
                        continue;

                    var pathName = (string)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "PathName");
                    var iconName = (string)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "IconName");
                    var description = (string)CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "Description");

                    var splits = pathName.Split('.');
                    CreateNodeListItem(new List<string>(splits), treeView_Nodes.Items, type, iconName, description);
                }
            }
        }

        private void CreateNodeListItem(List<string> nodePath, ItemCollection items, Type nodeType, string icon, string description)
        {
            if (nodePath.Count == 0)
                return;

            bool bFind = false;
            foreach (var item in items)
            {
                if (item.GetType() != typeof(PlantItem))
                    continue;

                var listItem = item as PlantItem;
                if (listItem.NodeName.Equals(nodePath[0]))
                {
                    // 同名称的有可能是重载，所以这里继续往后加
                    if (nodePath.Count == 1)
                    {
                        break;
                    }
                    else
                    {
                        nodePath.RemoveAt(0);
                        CreateNodeListItem(nodePath, listItem.Items, nodeType, icon, description);
                        bFind = true;
                        break;
                    }
                }
            }

            if (!bFind)
            {
                var listItem = new PlantItem(this);
                listItem.NodeName = nodePath[0];
                items.Add(listItem);

                if (nodePath.Count > 1)
                {
                    nodePath.RemoveAt(0);
                    CreateNodeListItem(nodePath, listItem.Items, nodeType, icon, description);
                }
                else
                {
                    listItem.NodeType = nodeType;
                    listItem.IconUri = icon;
                    listItem.Description = description;
                }
            }
        }

        public void ShowObjectProperty(object obj)
        {
            ProGrid_Obj.Instance = obj;
        }

        private void Button_ExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in treeView_Nodes.Items)
            {
                ExpandItem(child as PlantItem, true);
            }
        }
        private void ExpandItem(PlantItem item, bool expand)
        {
            if (item == null)
                return;

            item.IsExpanded = expand;
            foreach (var child in item.Items)
            {
                ExpandItem(child as PlantItem, expand);
            }
        }
        private void Button_FoldAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in treeView_Nodes.Items)
            {
                ExpandItem(child as PlantItem, false);
            }
        }

        #region 拖动到游戏窗口

        private void InitializeGameWindowDragDrop()
        {
            EditorCommon.WorldEditorOperation.OnGameWindowDragEnter += WorldEditorOperation_OnGameWindowDragEnter;
            EditorCommon.WorldEditorOperation.OnGameWindowDragLeave += WorldEditorOperation_OnGameWindowDragLeave;
            EditorCommon.WorldEditorOperation.OnGameWindowDragOver += WorldEditorOperation_OnGameWindowDragOver;
            EditorCommon.WorldEditorOperation.OnGameWindowDragDrop += WorldEditorOperation_OnGameWindowDragDrop;
        }

        private void WorldEditorOperation_OnGameWindowDragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            EditorCommon.DragDrop.DragDropManager.Instance.ShowFlyWindow(false);
            e.Effect = System.Windows.Forms.DragDropEffects.Copy;
            foreach (var dragObj in EditorCommon.DragDrop.DragDropManager.Instance.DragedObjectList)
            {
                var dtg = dragObj as PlantItem;
                if (dtg == null)
                    continue;

                dtg.OnDragEnterGameWindow(sender as System.Windows.Forms.Form, e);
            }
        }
        private void WorldEditorOperation_OnGameWindowDragLeave(object sender, EventArgs e)
        {
            EditorCommon.DragDrop.DragDropManager.Instance.ShowFlyWindow(true);
            foreach (var dragObj in EditorCommon.DragDrop.DragDropManager.Instance.DragedObjectList)
            {
                var dtg = dragObj as PlantItem;
                if (dtg == null)
                    continue;

                dtg.OnDragLeaveGameWindow(sender as System.Windows.Forms.Form, e);
            }
        }
        private void WorldEditorOperation_OnGameWindowDragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            foreach (var dragObj in EditorCommon.DragDrop.DragDropManager.Instance.DragedObjectList)
            {
                var dtg = dragObj as PlantItem;
                if (dtg == null)
                    continue;

                dtg.OnDragOverGameWindow(sender as System.Windows.Forms.Form, e);
            }
        }
        private void WorldEditorOperation_OnGameWindowDragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            List<CCore.World.Actor> actors = new List<CCore.World.Actor>();
            foreach (var dragObj in EditorCommon.DragDrop.DragDropManager.Instance.DragedObjectList)
            {
                var dtg = dragObj as PlantItem;
                if (dtg == null)
                    continue;

                var act = dtg.OnDragDropGameWindow(sender as System.Windows.Forms.Form, e);
                if(act != null)
                {
                    actors.Add(act);
                }
            }

            EditorCommon.Assist.Assist.TrySelectActors(actors.ToArray());
        }


        #endregion
    }
}
