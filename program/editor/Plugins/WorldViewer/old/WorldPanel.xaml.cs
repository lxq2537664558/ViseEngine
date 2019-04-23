using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace WorldViewer
{


    /// <summary>
    /// Interaction logic for WorldPanel.xaml
    /// </summary>
    //[EditorCommon.PluginAssist.EditorPlugin(PluginType = "WorldViewer")]
    //[EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/WorldViewer")]
    //[Guid("340AEE3A-1002-4DFD-BC72-AF4220AEA17D")]
    //[PartCreationPolicy(CreationPolicy.Shared)]
    public partial class WorldPanel : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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
            get { return "WorldViewer"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "WorldViewer",
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

        ///////////////////////////////////////////////////////////

        public delegate void Delegate_OnSelectedActor(CCore.World.Actor actor, bool bMultiSelected);
        public Delegate_OnSelectedActor OnSelectedActor;

        //List<WorldPanelTreeViewItem> mSelectedItem = new List<WorldPanelTreeViewItem>();

        //static WorldPanel smInstance = null;
        //public static WorldPanel Instance
        //{
        //    get
        //    {
        //        if (smInstance == null)
        //            smInstance = new WorldPanel();

        //        return smInstance;
        //    }
        //}

        bool mShowNPCInitializer = false;
        public bool ShowNPCInitializer
        {
            get { return mShowNPCInitializer; }
            set
            {
                mShowNPCInitializer = value;

#warning 特殊对象处理，写成通用的
                /*if (mShowNPCInitializer)
                {
                    CCore.Client.MainWorldInstance.LoadWorld( LoadServerActor(CSUtility.Component.EActorGameType.NpcInitializer);

                    List<WorldItem> items;
                    if (mWorldItemDictionary.TryGetValue("NPCInitializer", out items))
                    {
                        foreach (var item in items)
                        {
                            item.ActorVisible = true;
                        }
                    }
                    else
                    {
                        Refresh();
                        if (mWorldItemDictionary.TryGetValue("NPCInitializer", out items))
                        {
                            foreach (var item in items)
                            {
                                item.ActorVisible = true;
                            }
                        }
                    }
                }
                else
                {
                    List<WorldItem> items;
                    if (mWorldItemDictionary.TryGetValue("NPCInitializer", out items))
                    {
                        foreach (var item in items)
                        {
                            item.ActorVisible = false;
                        }
                    }
                }*/

                OnPropertyChanged("ShowNPCInitializer");
            }
        }

        //Dictionary<string, TreeViewItem> mLayerTreeViewItemDictionary = new Dictionary<string, TreeViewItem>();

        Dictionary<string, List<WorldItem>> mWorldItemDictionary = new Dictionary<string, List<WorldItem>>();
        Dictionary<string, WorldPanelItemsTab> mItemsTabDictionary = new Dictionary<string, WorldPanelItemsTab>();
        Dictionary<Guid, WorldItem> mWorldItemGuidDictionary = new Dictionary<Guid, WorldItem>();

        public WorldPanel()
        {
            InitializeComponent();

        }

        private void Button_Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Refresh();
            System.GC.Collect();
        }

        private void Button_ClearInvalidActor_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var allActors = CCore.Client.MainWorldInstance.GetActors((UInt16)CSUtility.Component.EActorGameType.Common);
            int i = 0;
            foreach (var actor in allActors)
            {
                if (actor == null)
                    continue;
                
                var mesh = actor.Visual as CCore.Mesh.Mesh;
                if(mesh == null)
                    continue;

                var meshInit = mesh.VisualInit as CCore.Mesh.MeshInit;
                if(meshInit == null)
                    continue;

                var meshTemplateFile = CCore.Mesh.MeshTemplateMgr.Instance.GetMeshTemplateFile(meshInit.MeshTemplateID);
                if (string.IsNullOrEmpty(meshTemplateFile))
                {
                    CCore.Client.MainWorldInstance.RemoveActor(actor);
                    i++;
                }
            }

            EditorCommon.MessageBox.Show("清除了" + i + "个无效对象! 请保存场景!");
        }

        public void Refresh()
        {
            WorldItemsCtrl.UpdateActors();

            //mWorldItemDictionary.Clear();
            //mItemsTabDictionary.Clear();
            //mWorldItemGuidDictionary.Clear();
            //TabControl_Layers.Items.Clear();

            //var prefabs = CCore.Client.MainWorldInstance.GetActors(CSUtility.Component.EActorGameType.Prefab);

            //var allActors = CCore.Client.MainWorldInstance.GetActors(CSUtility.Component.EActorGameType.Unknow);
            //foreach (var actor in allActors)
            //{
            //    if (actor == null)
            //        continue;

            //    bool bContainInPrefab = false;
            //    foreach (CCore.World.Prefab.Prefab prefab in prefabs)
            //    {
            //        if (prefab.ContainActor(actor.Id))
            //        {
            //            bContainInPrefab = true;
            //            break;
            //        }
            //    }
            //    if (bContainInPrefab)
            //        continue;

            //    var layerName = actor.GetLayerName();

            //    WorldPanelItemsTab itemTabCtrl;
            //    if (!mItemsTabDictionary.TryGetValue(layerName, out itemTabCtrl))
            //    {
            //        itemTabCtrl = new WorldPanelItemsTab();
            //        mItemsTabDictionary[layerName] = itemTabCtrl;

            //        List<WorldItem> items = new List<WorldItem>();
            //        mWorldItemDictionary[layerName] = items;
            //        itemTabCtrl.ItemsList = items;
            //        itemTabCtrl.OnSelectedActor = WorldPanelTab_OnSelectedActor;

            //        var tabItem = new TabItem();
            //        tabItem.Header = layerName;
            //        tabItem.Foreground = Brushes.White;
            //        tabItem.Content = itemTabCtrl;
            //        TabControl_Layers.Items.Add(tabItem);
            //    }

            //    List<WorldItem> worldItems;
            //    if (!mWorldItemDictionary.TryGetValue(layerName, out worldItems))
            //        return;

            //    WorldItem item = new WorldItem();
            //    item.HostActor = actor;
            //    item.OnVisibleChanged += WorldItem_OnVisibleChanged;
            //    worldItems.Add(item);
            //    mWorldItemGuidDictionary[actor.Id] = item;
            //}
        }

        void WorldItem_OnVisibleChanged(bool visible)
        {

        }

        void WorldPanelTab_OnSelectedActor(CCore.World.Actor actor, bool bMultiSelected)
        {
            if (OnSelectedActor != null)
                OnSelectedActor(actor, bMultiSelected);
        }
        //public void Refresh()
        //{
        //    mLayerTreeViewItemDictionary.Clear();
        //    TreeView_Items.Items.Clear();
        //    System.GC.Collect();
        //    System.GC.WaitForPendingFinalizers();
        //    System.GC.Collect();
        //    System.GC.WaitForPendingFinalizers();

        //    var prefabs = CCore.Engine.Instance.MainWorld.GetActors(CSUtility.Component.EActorGameType.Prefab);

        //    var allActors = CCore.Engine.Instance.MainWorld.GetActors(CSUtility.Component.EActorGameType.Unknow);
        //    foreach (var actor in allActors)
        //    {
        //        if (actor == null)
        //            continue;

        //        bool bContainInPrefab = false;
        //        foreach (CCore.World.Prefab.Prefab prefab in prefabs)
        //        {
        //            if (prefab.ContainActor(actor.Id))
        //            {
        //                bContainInPrefab = true;
        //                break;
        //            }
        //        }
        //        if (bContainInPrefab)
        //            continue;

        //        var layerName = actor.GetLayerName();

        //        TreeViewItem layerItem = null;
        //        WorldPanelTreeViewItem vItem = null;
        //        if (!mLayerTreeViewItemDictionary.TryGetValue(layerName, out layerItem))
        //        {
        //            layerItem = new TreeViewItem();
        //            vItem = new WorldPanelTreeViewItem();
        //            vItem.DisplayName = layerName;
        //            layerItem.Header = vItem;
        //            mLayerTreeViewItemDictionary[layerName] = layerItem;
        //            TreeView_Items.Items.Add(layerItem);
        //        }
        //        else
        //        {
        //            vItem = layerItem.Header as WorldPanelTreeViewItem;
        //        }

        //        var treeViewItem = new TreeViewItem();
        //        var tVActItem = new WorldPanelTreeViewItem();
        //        tVActItem.HostActor = actor;
        //        treeViewItem.Header = tVActItem;
        //        ////treeViewItem.Selected += ActorItemSelected;
        //        ////treeViewItem.Selected += tVActItem.OnHostTreeViewItemSelected;
        //        ////treeViewItem.Unselected += tVActItem.OnHostTreeViewItemUnSelected;
        //        tVActItem.MouseDown += WorldPanelTreeViewItem_MouseDown;
        //        vItem.OnVisibleChanged += tVActItem.OnLayerVisibleChanged;
        //        layerItem.Items.Add(treeViewItem);
        //    }

        //    ShowActorSpecialProperty(null);

        //    //mLayerTreeViewItemDictionary.Clear();
        //    //TreeView_Items.Items.Clear();

        //    //System.GC.Collect();
        //    //System.GC.WaitForPendingFinalizers();
        //    //System.GC.Collect();
        //    //System.GC.WaitForPendingFinalizers();
        //}

        //void WorldPanelTreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    var wptvItem = sender as WorldPanelTreeViewItem;

        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        if (OnSelectedActor != null)
        //            OnSelectedActor(wptvItem.HostActor, false);
        //    }
        //    else if (e.RightButton == MouseButtonState.Pressed)
        //    {
        //        if (OnSelectedActor != null)
        //            OnSelectedActor(wptvItem.HostActor, true);
        //    }

        //    CCore.Engine.Instance.MainWorld.RemoveEditorActor(wptvItem.HostActor);
        //    CCore.Engine.Instance.MainWorld.AddActor(wptvItem.HostActor);
        //}

        private void ShowActorSpecialProperty(CCore.World.Actor actor)
        {
            Grid_SelectedPro.Children.Clear();

#warning 通用处理
            /*if (actor != null)
            {
                switch (actor.GameType)
                {
                    case CSUtility.Component.EActorGameType.Common:
                        {
                            if (actor.Visual is CCore.Mesh.Mesh)
                            {
                                var ctrl = new MeshActorControl();

                                var mesh = actor.Visual as CCore.Mesh.Mesh;
                                if (mesh != null)
                                {
                                    var meshInit = mesh.VisualInit as CCore.Mesh.MeshInit;
                                    if (meshInit != null)
                                    {
                                        ctrl.MeshTemplateId = meshInit.MeshTemplateID;
                                        ctrl.TitleString = WorldItem.GetDisplayName(actor);
                                        Grid_SelectedPro.Children.Add(ctrl);
                                    }
                                }
                            }
                        }
                        break;
                    case CSUtility.Component.EActorGameType.Npc:
                    case CSUtility.Component.EActorGameType.Player:
                        {
                            var ctrl = new RoleActorControl();

                            var roleActor = actor as FrameSet.Role.RoleActor;
                            ctrl.AIGuid = roleActor.RoleData.AIGuid;
                            ctrl.RoleTemplateId = roleActor.RoleTemplate.Id;
                            ctrl.TitleString = WorldItem.GetDisplayName(actor);
                            Grid_SelectedPro.Children.Add(ctrl);
                        }
                        break;
                    case CSUtility.Component.EActorGameType.NpcInitializer:
                        {
                            var ctrl = new RoleActorControl();

                            var npcInit = actor.ActorInit as CSUtility.Data.NPCInitializerActorInit;
                            if (npcInit.NpcType == CSUtility.Data.NPCType.NPC)
                            {
                                var npcData = npcInit.NPCData as CSUtility.Data.NPCData;
                                if (npcData != null)
                                    ctrl.AIGuid = npcData.AIGuid;
                            }
                            ctrl.RoleTemplateId = npcInit.NPCData.TemplateId;
                            ctrl.TitleString = WorldItem.GetDisplayName(actor);
                            Grid_SelectedPro.Children.Add(ctrl);
                        }
                        break;
                }
            }*/
        }

        //private void ActorItemSelected(object sender, RoutedEventArgs e)
        //{

        //}

        List<CCore.World.Actor> mSelectedActors = new List<CCore.World.Actor>();
        public void SelectedActors(List<CCore.World.Actor> actors)
        {
            //foreach (var item in mSelectedItem)
            //{
            //    item.IsSelected = false;
            //}
            //mSelectedItem.Clear();

            //List<CCore.World.Actor> selectedActors = new List<CCore.World.Actor>(actors);
            //foreach (TreeViewItem item in TreeView_Items.Items)
            //{
            //    SelectedActors(selectedActors, item);
            //}

            if (actors.Count > 0)
            {
                List<CCore.World.Actor> selectedActors = new List<CCore.World.Actor>();
                List<CCore.World.Actor> unSelectedActors = new List<CCore.World.Actor>();

                foreach (var actor in mSelectedActors)
                {
                    if (!actors.Contains(actor))
                    {
                        unSelectedActors.Add(actor);
                    }
                }

                foreach (var actor in actors)
                {
                    if (!mSelectedActors.Contains(actor))
                    {
                        selectedActors.Add(actor);
                    }
                }
                mSelectedActors.Clear();
                mSelectedActors.AddRange(actors);

                foreach (var actor in unSelectedActors)
                {
                    var layerName = actor.GetLayerName();

                    WorldPanelItemsTab itemsTab;
                    if (mItemsTabDictionary.TryGetValue(layerName, out itemsTab))
                    {
                        //itemsTab.ListBox_Items.SelectedItems.Clear();

                        WorldItem worldItem;
                        if (!mWorldItemGuidDictionary.TryGetValue(actor.Id, out worldItem))
                            continue;

                        itemsTab.SelectedFromWorld = true;
                        itemsTab.ListBox_Items.SelectedItems.Remove(worldItem);
                        itemsTab.SelectedFromWorld = false;
                    }
                }
                foreach (var actor in selectedActors)
                {
                    var layerName = actor.GetLayerName();

                    WorldPanelItemsTab itemsTab;
                    if (mItemsTabDictionary.TryGetValue(layerName, out itemsTab))
                    {
                        //itemsTab.ListBox_Items.SelectedItems.Clear();

                        WorldItem worldItem;
                        if (!mWorldItemGuidDictionary.TryGetValue(actor.Id, out worldItem))
                            continue;

                        itemsTab.SelectedFromWorld = true;
                        itemsTab.ListBox_Items.SelectedItems.Add(worldItem);
                        itemsTab.SelectedFromWorld = false;
                    }
                }

                if (actors.Count > 0)
                    ShowActorSpecialProperty(actors[actors.Count - 1]);
            }
            else
            {
                mSelectedActors.Clear();

                foreach (var tab in mItemsTabDictionary.Values)
                {
                    tab.ListBox_Items.SelectedItems.Clear();
                }
            }

        }

        //private void SelectedActors(List<CCore.World.Actor> actors, TreeViewItem item)
        //{
        //    var wptvItem = item.Header as WorldPanelTreeViewItem;
        //    if (actors.Contains(wptvItem.HostActor))
        //    {
        //        wptvItem.IsSelected = true;
        //        mSelectedItem.Add(wptvItem);
        //        actors.Remove(wptvItem.HostActor);
        //    }

        //    if (actors.Count > 0)
        //    {
        //        foreach (TreeViewItem cItem in item.Items)
        //        {
        //            SelectedActors(actors, cItem);
        //        }
        //    }
        //}

        private void Button_ResetCamera_Click(object sender, System.Windows.RoutedEventArgs e)
        {
#warning 插件化备注
            //MainEditor.MainWindow.Instance.ResetGameCamera();
        }

        private void Button_ActorLayerManager_Click(object sender, RoutedEventArgs e)
        {
            var win = new ActorLayerEditorWindow();
            win.ShowDialog();
        }
    }
}
