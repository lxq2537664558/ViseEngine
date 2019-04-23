using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace RoleTemplateEditor
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "RoleTemplateEditor")]
    //[EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/RoleTemplateEditor")]
    [Guid("706DE305-3622-4D71-B203-CCC623B5C46E")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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
            get { return "角色模板编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "角色模板编辑器",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            D3DViewer.Activated = true;
            return true;
        }
        public bool OnDeactive()
        {
            D3DViewer.Activated = false;
            return true;
        }

        public void SetObjectToEdit(object[] obj)
        {
            if (obj == null)
                return;
            if (obj.Length == 0)
                return;

            try
            {
                D3DViewer.SetObjectToEdit(null);
                SetRoleTemplate(obj[0] as RoleTemplateResourceInfo);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
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
            //InitializeD3DShow();
            D3DViewer.Tick();

//             if (mD3DShowPlugin != null)
//                 mD3DShowPlugin.Tick();
        }

#region D3DPreView
        
        //[Import("D3DViewer_GetCameraController", typeof(Func<CCore.Camera.CameraController>))]
        //public Func<CCore.Camera.CameraController> D3DViewer_GetCameraController_Delegate { get; set; }

//         [Import("D3DShow", AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]
//         EditorCommon.PluginAssist.IEditorPlugin mD3DShowPlugin = null;
//         public EditorCommon.PluginAssist.IEditorPlugin D3DShowPlugin
//         {
//             get { return mD3DShowPlugin; }
//         }

        bool d3dShowInited = false;
        void InitializeD3DShow(bool force = false)
        {
            if (d3dShowInited && !force)
                return;

//             if (mD3DShowPlugin == null)
//                 return;
// 
//             if (mD3DShowPlugin is FrameworkElement)
//             {
//                 Grid_Preview.Children.Clear();
//                 Grid_Preview.Children.Add(mD3DShowPlugin as FrameworkElement);
//             }

            d3dShowInited = true;
        }

#endregion

        RoleTemplateResourceInfo mCurRoleTemplate = null;
        UInt16 CurRoleTemplateID
        {
            get
            {
                if (mCurRoleTemplate != null && mCurRoleTemplate.RoleTemplate != null)
                    return mCurRoleTemplate.RoleTemplate.Id;

                return UInt16.MaxValue;
            }
        }

        public MainControl()
        {
            InitializeComponent();
        }

        bool mInitialized = false;
        private void userControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!mInitialized)
            {
                if (ActionListBox.Items.Count > 0)
                {
                    var scrollView = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(ActionListBox, 0), 0) as ScrollViewer;
                    var binding = new Binding("HorizontalOffset") { Source = scrollView };
                    var offsetChangeListener = DependencyProperty.RegisterAttached("RoleControlListenerHorizontalOffset", typeof(object), typeof(UserControl), new PropertyMetadata(OnActionListBoxHorizontalScrollChanged));
                    scrollView.SetBinding(offsetChangeListener, binding);

                    mInitialized = true;
                }
            }
        }

        private void OnActionListBoxHorizontalScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (double)e.NewValue;

            Grid_ActionListTitle.Margin = new Thickness(-newValue, Grid_ActionListTitle.Margin.Top, Grid_ActionListTitle.Margin.Right, Grid_ActionListTitle.Margin.Bottom);
        }

        private void AddAction_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_ActionNames.SelectedIndex < 0)
                return;

            CSUtility.Data.ActionNamePair an = new CSUtility.Data.ActionNamePair();
            an.Name = (string)ListBox_ActionNames.SelectedItem;
            mCurRoleTemplate.RoleTemplate.Actions.Add(an.Name, an);

            ListBox_ActionNames.Items.RemoveAt(ListBox_ActionNames.SelectedIndex);

            var item = new RoleControl_ListItem();
            BindingOperations.SetBinding(item, RoleControl_ListItem.ActionNameProperty, new Binding("Name") { Source = an, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            BindingOperations.SetBinding(item, RoleControl_ListItem.ActionFileProperty, new Binding("ActionFile") { Source = an, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            item.OnPreviewCheckedChanged = new RoleControl_ListItem.Delegate_OnPreviewCheckedChanged(OnActionPreviewCheckedChanged);

            //item.ActionName.Text = an.Name;
            //item.ActionRes.CurAIActionName = an.ActionFile;
            ActionListBox.Items.Add(item);

            mCurRoleTemplate.IsDirty = true;
        }

        void OnActionPreviewCheckedChanged(RoleControl_ListItem item)
        {
            int index = 0;
            if (ListBox_MeshList.SelectedIndex != -1)
            {
                index = ListBox_MeshList.SelectedIndex;
            }

            if (item.PreViewChecked == true)
            {
                foreach (RoleControl_ListItem listItem in ActionListBox.Items)
                {
                    if (listItem == item)
                        continue;

                    if (listItem.PreViewChecked == true)
                        listItem.PreViewChecked = false;
                }

                foreach (RoleControl_ListItem listItem in ActionListBox_Ext.Items)
                {
                    if (listItem == item)
                        continue;

                    if (listItem.PreViewChecked == true)
                        listItem.PreViewChecked = false;
                }                
                                
                var action = new CCore.AnimTree.AnimTreeNode_Action();
                action.Initialize();
                action.ActionName = item.ActionFile;                

                D3DViewer.SetObjectToEdit(new object[] { new object[] { "Action", true },
                                                              new object[] { index, action }});
            }
            else
            {
                D3DViewer.SetObjectToEdit(new object[] { new object[] { "Action", true },
                                                              new object[] { index, null }});
            }
        }

        void OnActionFileChanged(RoleControl_ListItem item)
        {
            mCurRoleTemplate.IsDirty = true;
        }

        private void RemoveAction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ActionListBox.SelectedIndex < 0)
                return;

            var item = ActionListBox.SelectedItem as RoleControl_ListItem;

            ListBox_ActionNames.Items.Add(item.ActionName);
            
            mCurRoleTemplate.RoleTemplate.Actions.Remove(item.ActionName);

            BindingOperations.ClearBinding(item, RoleControl_ListItem.ActionNameProperty);
            BindingOperations.ClearBinding(item, RoleControl_ListItem.ActionFileProperty);

            ActionListBox.Items.RemoveAt(ActionListBox.SelectedIndex);

            mCurRoleTemplate.IsDirty = true;
        }

        public void SetRoleTemplate(RoleTemplateResourceInfo roleTemplateRes)
        {
            if (roleTemplateRes == null || roleTemplateRes.RoleTemplate == null)
                return;

            if (roleTemplateRes.RoleTemplate.Id == CurRoleTemplateID)
                return;
            
            mCurRoleTemplate = roleTemplateRes;

            PropertyGrid_RoleTemplate.Instance = roleTemplateRes.RoleTemplate;

            // 初始化动作列表
            var actionNameList = new List<string>();
            actionNameList.AddRange(CSUtility.Data.RoleTemplateManager.Instance.ActionNameList);

            ActionListBox.Items.Clear();
            ActionListBox_Ext.Items.Clear();
            foreach (var action in roleTemplateRes.RoleTemplate.Actions.Values)
            {
                var item = new RoleControl_ListItem();

                BindingOperations.SetBinding(item, RoleControl_ListItem.ActionNameProperty, new Binding("Name") { Source = action, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                BindingOperations.SetBinding(item, RoleControl_ListItem.ActionFileProperty, new Binding("ActionFile") { Source = action, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                item.OnPreviewCheckedChanged = new RoleControl_ListItem.Delegate_OnPreviewCheckedChanged(OnActionPreviewCheckedChanged);
                item.OnActionFileChanged = new RoleControl_ListItem.Delegate_OnActionFileChanged(OnActionFileChanged);

                if (actionNameList.Contains(action.Name))
                {
                    ActionListBox.Items.Add(item);
                    actionNameList.Remove(action.Name);
                }
                else
                {
                    item.ActionNameEditable = true;
                    ActionListBox_Ext.Items.Add(item);
                }
            }

            // 设置未使用动作列表
            ListBox_ActionNames.Items.Clear();
            foreach (var aName in actionNameList)
            {
                ListBox_ActionNames.Items.Add(aName);
            }

            // 初始化模型列表
            ListBox_MeshList.Items.Clear();
            foreach (var meshId in roleTemplateRes.RoleTemplate.DefaultMeshs)
            {
                var meshTemplate = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(meshId);
                var item = new RoleControl_MeshListItem(meshTemplate);
                item.OnMeshSet = new RoleControl_MeshListItem.Delegate_OnMeshSet(OnMeshListItemMeshSet);
                ListBox_MeshList.Items.Add(item);

                var mshInit = new CCore.Mesh.MeshInit()
                {
                    MeshTemplateID = meshId,
                    CanHitProxy = false
                };
                var mesh = new CCore.Mesh.Mesh();
                mesh.Initialize(mshInit, null);

                var atInit = new CCore.World.MeshActorInit();
                var actor = new CCore.World.MeshActor();
                actor.Initialize(atInit);
                actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));
                actor.mUpdateAnimByDistance = false;
                actor.Visual = mesh;

                D3DViewer.SetObjectToEdit(new object[] { new object[]{ "Actor", true },
                                                         new object[]{ actor }});
                item.AttachedActor = actor;
            }

            // D3D显示
            //             if (D3DShowPlugin != null)
            //             {
            //                 D3DShowPlugin.SetObjectToEdit(new object[] { new object[] { "RoleTemplate", false },
            //                                                              new object[] { mCurRoleTemplate.RoleTemplate }});
            //             }
            
        }

        private void Button_AddMeshTemplate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mCurRoleTemplate == null || mCurRoleTemplate.RoleTemplate == null)
                return;

            
            mCurRoleTemplate.RoleTemplate.DefaultMeshs.Add(Guid.Empty);

            var item = new RoleControl_MeshListItem(null);
            item.OnMeshSet = new RoleControl_MeshListItem.Delegate_OnMeshSet(OnMeshListItemMeshSet);
            ListBox_MeshList.Items.Add(item);

            mCurRoleTemplate.IsDirty = true;
        }

        private void Button_DelMeshTemplate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ListBox_MeshList.SelectedIndex < 0 || mCurRoleTemplate == null)
                return;

            var item = ListBox_MeshList.SelectedItem as RoleControl_MeshListItem;
            if (item == null)
                return;
        
            mCurRoleTemplate.RoleTemplate.DefaultMeshs.RemoveAt(ListBox_MeshList.SelectedIndex);
            ListBox_MeshList.Items.RemoveAt(ListBox_MeshList.SelectedIndex);

            D3DViewer.RemoveObjects(new object[] { "Actor", item.AttachedActor });            

            mCurRoleTemplate.IsDirty = true;
        }

        private void OnMeshListItemMeshSet(RoleControl_MeshListItem item)
        {
            if (mCurRoleTemplate == null)
                return;

            var idx = ListBox_MeshList.Items.IndexOf(item);
            if (idx < 0)
                return;

            if (mCurRoleTemplate.RoleTemplate.DefaultMeshs.Count > idx && mCurRoleTemplate.RoleTemplate.DefaultMeshs[idx] == item.AttachedMeshTemplate.MeshID)
            {
                return;
            }

            if (item.AttachedActor != null)
                D3DViewer.World.RemoveActor(item.AttachedActor);

            mCurRoleTemplate.RoleTemplate.DefaultMeshs[idx] = item.AttachedMeshTemplate.MeshID;
            
            var mshInit = new CCore.Mesh.MeshInit()
            {
                MeshTemplateID = item.AttachedMeshTemplate.MeshID,
                CanHitProxy = false
            };
            var mesh = new CCore.Mesh.Mesh();
            mesh.Initialize(mshInit, null);

            var atInit = new CCore.World.MeshActorInit();
            var actor = new CCore.World.MeshActor();
            actor.Initialize(atInit);
            actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));
            actor.mUpdateAnimByDistance = false;
            actor.Visual = mesh;

            D3DViewer.SetObjectToEdit(new object[] { new object[]{ "Actor", true },
                                                         new object[]{ actor }});
            item.AttachedActor = actor;            

            mCurRoleTemplate.IsDirty = true;
        }

        private void Menu_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Program.SaveRoleTemplate(mCurRoleTemplate);
        }

        private ValidationResult CheckValid(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "名称不能为空");

            string valueStr = (string)value;
            if (string.IsNullOrEmpty(valueStr))
                return new ValidationResult(false, "名称不能为空");

            if(mCurRoleTemplate.RoleTemplate.Actions.ContainsKey(valueStr))
                return new ValidationResult(false, $"已存在名称为{valueStr}的动作");

            return new ValidationResult(true, null);
        }
        // 添加扩展动作
        private void Button_AddExtAction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InputWindow.InputWindow win = new InputWindow.InputWindow();
            win.Description = "请输入动作名称:";
            win.Value = "";
            if (win.ShowDialog(CheckValid) == false)
                return;
            CSUtility.Data.ActionNamePair an = new CSUtility.Data.ActionNamePair();
            an.Name = (string)win.Value;
            mCurRoleTemplate.RoleTemplate.Actions.Add(an.Name, an);

            var item = new RoleControl_ListItem();
            //item.ActionNameEditable = true;
            BindingOperations.SetBinding(item, RoleControl_ListItem.ActionNameProperty, new Binding("Name") { Source = an, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            BindingOperations.SetBinding(item, RoleControl_ListItem.ActionFileProperty, new Binding("ActionFile") { Source = an, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            item.OnPreviewCheckedChanged = new RoleControl_ListItem.Delegate_OnPreviewCheckedChanged(OnActionPreviewCheckedChanged);

            ActionListBox_Ext.Items.Add(item);

            mCurRoleTemplate.IsDirty = true;
        }

        // 删除扩展动作
        private void Button_DelExtAction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ActionListBox_Ext.SelectedIndex < 0)
                return;

            var item = ActionListBox_Ext.SelectedItem as RoleControl_ListItem;
            
            mCurRoleTemplate.RoleTemplate.Actions.Remove(item.ActionName);

            BindingOperations.ClearBinding(item, RoleControl_ListItem.ActionNameProperty);
            BindingOperations.ClearBinding(item, RoleControl_ListItem.ActionFileProperty);

            ActionListBox_Ext.Items.RemoveAt(ActionListBox.SelectedIndex);

            mCurRoleTemplate.IsDirty = true;
        }
    }
}
