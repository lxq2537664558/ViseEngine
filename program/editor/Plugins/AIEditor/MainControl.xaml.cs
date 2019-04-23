using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace AIEditor
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "AIEditor")]
    //[EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/AIEditor")]
    [Guid("ABDFFCAB-5B89-4B52-A597-B4D2DBA9A15A")]
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
            get { return "AI编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "AI编辑器",
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
            if (obj == null)
                return;

            if (obj.Length == 0)
                return;

            try
            {
                SetAIInstanceInfo((AIResourceInfo)(obj[0]));
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

        private class TypeGridInfo
        {
            public string type;
            public int idx;
            //public List<UIElement> Grid_Data_Children = new List<UIElement>();
            //public UIElement Grid_Column_Child;
            //public UIElement Grid_Row_Child;
        }
        Dictionary<string, TypeGridInfo> mTypeGridInfoDictionary = new Dictionary<string, TypeGridInfo>();
        // 在网格中显示的状态类型
        List<string> mTypesInStateGrid = new List<string>();

        AIEditor.FSMTemplateInfo mFSMTemplateInfo = null;
        //CodeViewer mCodeViewer = null;

        //Guid mAIInstanceGuid = Guid.Empty;
        public Guid AIInstanceGuid
        {
            get
            {
                if (mFSMTemplateInfo != null)
                    return mFSMTemplateInfo.Id;

                return Guid.Empty;
            }
        }

        bool mIsDirty = false;
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;

                if (mFSMTemplateInfo != null)
                {
                    if(mFSMTemplateInfo.IsDirty != true)
                        mFSMTemplateInfo.IsDirty = value;
                }

                //if (mIsDirty == true)
                //{
                //    if (mNeedSave)
                //    {
                //        SaveStateGrid();
                //    }
                //}
            }
        }

        //bool mNeedSave = true;
        
        public bool IsDebug
        {
            get { return FSMTemplateInfoManager.Instance.IsDebug; }
            set
            {
                FSMTemplateInfoManager.Instance.IsDebug = value;

                OnPropertyChanged("IsDebug");
            }
        }

        public MainControl()
        {
            InitializeComponent();

            var template = this.TryFindResource("AISetControl") as DataTemplate;
            WPG.Program.RegisterDataTemplate("AISet", template);
            //InitializeAIState();
        }

        void UpdateAddStateComboBox()
        {
            ComboBox_States.Items.Clear();

            var attFullName = typeof(CSUtility.AISystem.Attribute.StatementClassAttribute).FullName;
            var types = CSUtility.Program.GetTypes(CSUtility.Helper.enCSType.Client, attFullName, false);
            foreach (var type in types)// Program.CSCommonAssm.GetTypes())
            {
                //if (mTypesInStateGrid.Contains(type))
                //    continue;

                //var atts = type.GetCustomAttributes(typeof(CSUtility.AISystem.Attribute.StatementClassAttribute), true);
                //if (atts.Length == 0)
                //    continue;

                var tempName = type.Name;//Program.CurrentHostAIInstanceInfo.GetStateNickName(type.Name);//AIEditor.FSMTemplateInfo.GetStateTypeAttributeName(type);
                if (string.IsNullOrEmpty(tempName))
                    continue;

                if (!string.Equals(CSUtility.Helper.enCSType.Common.ToString(), (CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(type, attFullName, "CSType", false).ToString())))
                    continue;

                //var typeName = type.Name;
                //if (mTypesInStateGrid.Contains(typeName))
                //    continue;

                //AISystem.Attribute.StatementClassAttribute att = atts[0] as AISystem.Attribute.StatementClassAttribute;

                ComboBoxItem item = new ComboBoxItem();
                item.Content = tempName;//att.m_strName;
                item.Tag = type;
                ComboBox_States.Items.Add(item);

                ////stateTypeList.Add(type);
                //AddRow(type.FullName, att.m_strName);
                //AddColumn(type.FullName, att.m_strName);
            }
        }

#region 表格横列

        void AddColumn(string typeName, string columnName)
        {
            //var type = Program.GetType(typeName);

            //if (type == null)
            //    return;

            if (!mTypesInStateGrid.Contains(typeName))
                mTypesInStateGrid.Add(typeName);

            TypeGridInfo info = null;
            if (!mTypeGridInfoDictionary.TryGetValue(typeName, out info))
            {
                info = new TypeGridInfo();
                info.type = typeName;
                info.idx = Grid_Column.ColumnDefinitions.Count;
                mTypeGridInfoDictionary.Add(typeName, info);
            }

            /*var columnGrid = new Grid();
            var columnText = new TextBlock()
            {
                Text = columnName,
                Foreground = Brushes.White,
                Margin = new Thickness(5),
                FontSize = 15
            };
            columnGrid.Children.Add(columnText);

            Grid.SetColumn(columnGrid, Grid_Column.ColumnDefinitions.Count);// - 1);
            Grid_Column.Children.Add(columnGrid);*/

            //info.Grid_Column_Child = columnGrid;
            StateGridColumnHeader cHeader = new StateGridColumnHeader(typeName);
            cHeader.NickName = columnName;
            Grid.SetColumn(cHeader, Grid_Column.ColumnDefinitions.Count);
            Grid_Column.Children.Add(cHeader);

            var cDef = new ColumnDefinition();
            cDef.Width = new GridLength(1, GridUnitType.Auto);
            Grid_Data.ColumnDefinitions.Add(cDef);

            for (int i = 0; i < Grid_Row.RowDefinitions.Count; i++)
            {
                if (i == Grid_Column.ColumnDefinitions.Count)
                    continue;

                var stateChangeInfo = mFSMTemplateInfo.GetStateSwitchInfo(mTypesInStateGrid[i], typeName);
                StateGridStateChange rect = new StateGridStateChange(stateChangeInfo, this);

                Grid.SetColumn(rect, Grid_Column.ColumnDefinitions.Count);// - 1);
                Grid.SetRow(rect, i);

                rect.SetBinding(Rectangle.WidthProperty, new Binding("ActualWidth") { Source = cHeader });
                rect.SetBinding(Rectangle.HeightProperty, new Binding("ActualHeight") { Source = Grid_Row.Children[i] });

                Grid_Data.Children.Add(rect);

                //info.Grid_Data_Children.Add(rect);
            }

            cDef = new ColumnDefinition();
            cDef.Width = new GridLength(1, GridUnitType.Auto);
            Grid_Column.ColumnDefinitions.Add(cDef);
        }

        void AddRow(string typeName, string rowName)
        {
            //var type = Program.GetType(typeName);

            //if (type == null)
            //    return;

            if (!mTypesInStateGrid.Contains(typeName))
                mTypesInStateGrid.Add(typeName);

            TypeGridInfo info = null;
            if (!mTypeGridInfoDictionary.TryGetValue(typeName, out info))
            {
                info = new TypeGridInfo();
                info.type = typeName;
                info.idx = Grid_Row.RowDefinitions.Count;
                mTypeGridInfoDictionary.Add(typeName, info);
            }

            //var rowGrid = new Grid();
            //var rowStack = new StackPanel()
            //{
            //    Orientation = Orientation.Horizontal
            //};
            //var rowText = new TextBlock()
            //{
            //    Text = rowName,
            //    Foreground = Brushes.White,
            //    Margin = new Thickness(5),
            //    FontSize = 15,
            //};
            //rowStack.Children.Add(rowText);
            //var rowBtn = new Button()
            //{
            //    Content = " X ",
            //    Margin = new Thickness(2),
            //    Foreground = Brushes.Red
            //};
            //rowBtn.Tag = type;
            //rowBtn.Click += DeleteStateButton_Click;
            //rowStack.Children.Add(rowBtn);
            //rowGrid.Children.Add(rowStack);

            //Grid.SetRow(rowGrid, Grid_Row.RowDefinitions.Count);// - 1);
            //Grid_Row.Children.Add(rowGrid);

            //info.Grid_Row_Child = rowGrid;
            StateGridRowHeader rHeader = new StateGridRowHeader(typeName, AIInstanceGuid, this);
            rHeader.NickName = rowName;
            rHeader.OnDelete = DeleteState;
            rHeader.OnSetDefaultState = SetDefaultState;
            rHeader.OnStateNickNameChanged = OnStateNickNameChanged;
            Grid.SetRow(rHeader, Grid_Row.RowDefinitions.Count);
            Grid_Row.Children.Add(rHeader);

            if (mFSMTemplateInfo.DefaultStateTypeName == typeName)//type.Name)//type.FullName)
                SetDefaultState(rHeader);

            var rDef = new RowDefinition();
            rDef.Height = new GridLength(1, GridUnitType.Auto);
            Grid_Data.RowDefinitions.Add(rDef);

            for (int i = 0; i < Grid_Column.ColumnDefinitions.Count; i++)
            {
                //if (i == Grid_Row.RowDefinitions.Count)
                //    continue;

                var stateChangeInfo = mFSMTemplateInfo.GetStateSwitchInfo(typeName, mTypesInStateGrid[i]);
                StateGridStateChange rect = new StateGridStateChange(stateChangeInfo, this);

                Grid.SetColumn(rect, i);
                Grid.SetRow(rect, Grid_Row.RowDefinitions.Count);// - 1);

                rect.SetBinding(Rectangle.WidthProperty, new Binding("ActualWidth") { Source = Grid_Column.Children[i] });
                rect.SetBinding(Rectangle.HeightProperty, new Binding("ActualHeight") { Source = rHeader });

                Grid_Data.Children.Add(rect);

                //info.Grid_Data_Children.Add(rect);
            }

            rDef = new RowDefinition();
            rDef.Height = new GridLength(1, GridUnitType.Auto);
            Grid_Row.RowDefinitions.Add(rDef);
        }

        void InitializeFromAIInstance(AIEditor.FSMTemplateInfo info)
        {
            if (info == null)
                return;

            ClearStates();

            foreach (var stateType in info.StateTypes)
            {
                AddColumn(stateType, info.GetStateNickName(stateType));
                AddRow(stateType, info.GetStateNickName(stateType));
            }

            UpdateAddStateComboBox();
        }

        //void InitializeAIState()
        //{
        //    mNeedSave = false;

        //    //Grid_Column.Children.Clear();
        //    //Grid_Row.Children.Clear();
        //    //Grid_Data.Children.Clear();
        //    //Grid_Data.ColumnDefinitions.Clear();
        //    //Grid_Data.RowDefinitions.Clear();
        //    //Grid_Column.ColumnDefinitions.Clear();
        //    //Grid_Row.RowDefinitions.Clear();
        //    ClearStates();
                        
        //    List<Type> stateTypeList = new List<Type>();

        //    var type = Program.GetType("AISystem.States.Idle");
        //    AddState(type);
        //    //if(type != null)
        //    //{
        //    //    var atts = type.GetCustomAttributes(typeof(AISystem.Attribute.StatementClassAttribute), true);
        //    //    if (atts.Length != 0)
        //    //    {
        //    //        AISystem.Attribute.StatementClassAttribute att = atts[0] as AISystem.Attribute.StatementClassAttribute;
        //    //        AddColumn(type.FullName, att.m_strName);                    
        //    //        AddRow(type.FullName, att.m_strName);
        //    //    }
        //    //}

        //    //foreach (var type in Program.AIStateAssem.GetTypes())
        //    //{
        //    //    var atts = type.GetCustomAttributes(typeof(AISystem.Attribute.StatementClassAttribute), true);
        //    //    if (atts.Length == 0)
        //    //        continue;

        //    //    AISystem.Attribute.StatementClassAttribute att = atts[0] as AISystem.Attribute.StatementClassAttribute;

        //    //    //stateTypeList.Add(type);
        //    //    AddRow(type.FullName, att.m_strName);
        //    //    AddColumn(type.FullName, att.m_strName);
        //    //}

        //    mNeedSave = true;
        //}

#endregion

#region ScrollView同步滚动相关

        bool mIsScrollListenerRegisted = false;
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!mIsScrollListenerRegisted)
            {
                var binding = new Binding("VerticalOffset") { Source = ScrollViewer_Data };
                var offsetChangeListener = DependencyProperty.RegisterAttached("AIGrid_ListenerVerticalOffset", typeof(object), typeof(UserControl), new PropertyMetadata(OnVerticalScrollChanged));
                ScrollViewer_Data.SetBinding(offsetChangeListener, binding);

                binding = new Binding("HorizontalOffset") { Source = ScrollViewer_Data };
                offsetChangeListener = DependencyProperty.RegisterAttached("AIGrid_ListenerHorizontalOffset", typeof(object), typeof(UserControl), new PropertyMetadata(OnHorizontalScrollChanged));
                ScrollViewer_Data.SetBinding(offsetChangeListener, binding);

                mIsScrollListenerRegisted = true;
            }
        }

        public void OnVerticalScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer_Row.ScrollToVerticalOffset((double)(e.NewValue));
        }

        public void OnHorizontalScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer_Column.ScrollToHorizontalOffset((double)(e.NewValue));
        }

#endregion

        public void ClearStates()
        {
            Grid_Column.Children.Clear();
            Grid_Row.Children.Clear();
            Grid_Data.Children.Clear();
            Grid_Data.ColumnDefinitions.Clear();
            Grid_Data.RowDefinitions.Clear();
            Grid_Column.ColumnDefinitions.Clear();
            Grid_Row.RowDefinitions.Clear();
            TabControl_Items.Items.Clear();

            mTypesInStateGrid.Clear();
        }

        private void AddState(string type, string nickName, string baseType)
        {
            if (mFSMTemplateInfo != null)
            {
            //    if(!mFSMTemplateInfo.StateTypes.Contains(type))
            //        mFSMTemplateInfo.StateTypes.Add(type);
                mFSMTemplateInfo.AddState(type, nickName, baseType);
            }

            //var atts = type.GetCustomAttributes(typeof(AISystem.Attribute.StatementClassAttribute), true);
            //if (atts.Length != 0)
            //{

                //AISystem.Attribute.StatementClassAttribute att = atts[0] as AISystem.Attribute.StatementClassAttribute;

            AddColumn(type, nickName);//AIEditor.FSMTemplateInfo.GetStateTypeAttributeName(baseType));
            AddRow(type, nickName);//AIEditor.FSMTemplateInfo.GetStateTypeAttributeName(baseType));
            //}            
        }

        private void Button_AddState_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Popup_StateAdd.IsOpen = false;

            ComboBoxItem item = ComboBox_States.SelectedItem as ComboBoxItem;
            if (item == null)
                return;

            var type = item.Tag as Type;

            var stateName = TextBox_NewStateName.Text;
            if (string.IsNullOrEmpty(stateName))
                stateName = type.Name;
            // 检查状态名称是否重复
            if (mTypesInStateGrid.Contains(stateName))
            {
                EditorCommon.MessageBox.Show("状态名称重复，请修改名称");
                return;
            }

            AddState(stateName, stateName, type.FullName);

            // 如果添加的是第一个状态，则设此状态为默认状态
            if (mFSMTemplateInfo.StateTypes.Length == 1)
            {
                var gridRowHeader = Grid_Row.Children[0] as StateGridRowHeader;
                SetDefaultState(gridRowHeader);
            }
            IsDirty = true;

            UpdateAddStateComboBox();
        }

        void SetDefaultState(StateGridRowHeader item)
        {
            TypeGridInfo info = null;
            if (!mTypeGridInfoDictionary.TryGetValue(item.StateType, out info))
                return;

            mFSMTemplateInfo.DefaultStateTypeName = item.StateType;//.Name;//.ToString();

            foreach (UIElement child in Grid_Row.Children)
            {
                if (child == item)
                {
                    ((StateGridRowHeader)child).IsDefaultState = true;
                    continue;
                }

                if (child is StateGridRowHeader)
                {
                    ((StateGridRowHeader)child).IsDefaultState = false;
                }
            }
        }

        void OnStateNickNameChanged(string stateName, string newNickName)
        {
            foreach (StateGridColumnHeader cHeader in Grid_Column.Children)
            {
                cHeader.ChangeStateName(stateName, newNickName);
            }

            foreach (StateGridStateChange change in Grid_Data.Children)
            {
                change.ChangeStateName(stateName, newNickName);
            }
        }

        void DeleteState(StateGridRowHeader item)
        {
            TypeGridInfo info = null;
            if (!mTypeGridInfoDictionary.TryGetValue(item.StateType, out info))
                return;

            //mFSMTemplateInfo.StateTypes.Remove(item.StateType);
            mFSMTemplateInfo.RemoveState(item.StateType);

            mTypeGridInfoDictionary.Remove(item.StateType);
            foreach (var value in mTypeGridInfoDictionary.Values)
            {
                if (value.idx > info.idx)
                    value.idx--;
            }

            //Grid_Column.Children.Remove(info.Grid_Column_Child);
            //Grid_Row.Children.Remove(info.Grid_Row_Child);
            //foreach (var child in info.Grid_Data_Children)
            //{
            //    Grid_Data.Children.Remove(child);
            //}
            if (mTypesInStateGrid.Contains(info.type))
                mTypesInStateGrid.Remove(info.type);

            Grid_Column.ColumnDefinitions.RemoveAt(info.idx);
            Grid_Row.RowDefinitions.RemoveAt(info.idx);
            Grid_Data.ColumnDefinitions.RemoveAt(info.idx);
            Grid_Data.RowDefinitions.RemoveAt(info.idx);

            UIElement deletedElement = null; ;
            foreach (UIElement child in Grid_Column.Children)
            {
                var column = Grid.GetColumn(child);
                if (column > info.idx)
                    Grid.SetColumn(child, column - 1);
                else if (column == info.idx)
                    deletedElement = child;
            }
            if(deletedElement != null)
                Grid_Column.Children.Remove(deletedElement);
            deletedElement = null;
            foreach (UIElement child in Grid_Row.Children)
            {
                var row = Grid.GetRow(child);
                if (row > info.idx)
                    Grid.SetRow(child, row - 1);
                else if (row == info.idx)
                    deletedElement = child;
            }
            if(deletedElement != null)
                Grid_Row.Children.Remove(deletedElement);
            List<UIElement> deletedElements = new List<UIElement>();
            foreach (UIElement child in Grid_Data.Children)
            {
                var column = Grid.GetColumn(child);
                var row = Grid.GetRow(child);

                if (column > info.idx)
                    Grid.SetColumn(child, column - 1);
                else if (column == info.idx)
                    deletedElements.Add(child);

                if (row > info.idx)
                    Grid.SetRow(child, row - 1);
                else if (row == info.idx)
                    deletedElements.Add(child);
            }
            foreach (var child in deletedElements)
            {
                Grid_Data.Children.Remove(child);
            }

            UpdateAddStateComboBox();
            IsDirty = true;
        }
        //void DeleteStateButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button btn = sender as Button;

        //    TypeGridInfo info = null;
        //    if (!mTypeGridInfoDictionary.TryGetValue(btn.Tag as Type, out info))
        //        return;

        //    mTypeGridInfoDictionary.Remove(btn.Tag as Type);
        //    foreach (var value in mTypeGridInfoDictionary.Values)
        //    {
        //        if (value.idx > info.idx)
        //            value.idx--;
        //    }

        //    //Grid_Column.Children.Remove(info.Grid_Column_Child);
        //    //Grid_Row.Children.Remove(info.Grid_Row_Child);
        //    //foreach (var child in info.Grid_Data_Children)
        //    //{
        //    //    Grid_Data.Children.Remove(child);
        //    //}
        //    if (mTypesInStateGrid.Contains(info.type))
        //        mTypesInStateGrid.Remove(info.type);

        //    Grid_Column.ColumnDefinitions.RemoveAt(info.idx);
        //    Grid_Row.RowDefinitions.RemoveAt(info.idx);
        //    Grid_Data.ColumnDefinitions.RemoveAt(info.idx);
        //    Grid_Data.RowDefinitions.RemoveAt(info.idx);

        //    UIElement deletedElement = null; ;
        //    foreach (UIElement child in Grid_Column.Children)
        //    {
        //        var column = Grid.GetColumn(child);
        //        if (column > info.idx)
        //            Grid.SetColumn(child, column - 1);
        //        else if (column == info.idx)
        //            deletedElement = child;
        //    }
        //    if(deletedElement != null)
        //        Grid_Column.Children.Remove(deletedElement);
        //    deletedElement = null;
        //    foreach (UIElement child in Grid_Row.Children)
        //    {
        //        var row = Grid.GetRow(child);
        //        if (row > info.idx)
        //            Grid.SetRow(child, row - 1);
        //        else if (row == info.idx)
        //            deletedElement = child;
        //    }
        //    if(deletedElement != null)
        //        Grid_Row.Children.Remove(deletedElement);
        //    List<UIElement> deletedElements = new List<UIElement>();
        //    foreach (UIElement child in Grid_Data.Children)
        //    {
        //        var column = Grid.GetColumn(child);
        //        var row = Grid.GetRow(child);

        //        if (column > info.idx)
        //            Grid.SetColumn(child, column - 1);
        //        else if (column == info.idx)
        //            deletedElements.Add(child);

        //        if (row > info.idx)
        //            Grid.SetRow(child, row - 1);
        //        else if (row == info.idx)
        //            deletedElements.Add(child);
        //    }
        //    foreach (var child in deletedElements)
        //    {
        //        Grid_Data.Children.Remove(child);
        //    }

        //    UpdateAddStateComboBox();
        //}

        private void StateOpBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Popup_StateAdd.IsOpen = true;
        }

        //public void SaveStateGrid()
        //{
        //    System.Diagnostics.Debug.Assert(mFSMTemplateInfo != null);

        //    mFSMTemplateInfo.FSMGridXmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("FSMGrid", "");
        //    foreach (var gridInfo in mTypeGridInfoDictionary.Values)
        //    {
        //        var stateNode = mFSMTemplateInfo.FSMGridXmlHolder.RootNode.AddNode("State", "");
        //        stateNode.AddAttrib("Type", gridInfo.type.FullName);
        //    }
        //}

        //public void LoadStateGrid()
        //{
        //    System.Diagnostics.Debug.Assert(mFSMTemplateInfo != null);

        //    mNeedSave = false;
        //    ClearStates();

        //    if (mFSMTemplateInfo.FSMGridXmlHolder == null)
        //    {
        //        InitializeAIState();
        //    }
        //    else
        //    {
        //        foreach (var node in mFSMTemplateInfo.FSMGridXmlHolder.RootNode.FindNodes("State"))
        //        {
        //            var att = node.FindAttrib("Type");
        //            if (att == null)
        //                continue;

        //            var stateType = Program.GetType(att.Value);
        //            AddState(stateType);
        //        }
        //    }
        //    //var childNodes = mFSMTemplateInfo.FSMGridXmlHolder.RootNode.FindNodes("State");
        //    //if (childNodes.Count > 0)
        //    //{

        //    //}
        //    //else
        //    //{
        //    //    InitializeAIState();
        //    //}

        //    UpdateAddStateComboBox();

        //    IsDirty = false;
        //    mNeedSave = true;
        //}

        public void SetAIInstanceInfo(AIResourceInfo resInfo)
        {
            ClearStates();

            TextBox_StateId.Text = resInfo.Id.ToString();

            if (mFSMTemplateInfo != null)
                mFSMTemplateInfo.OnSave = null;

            mFSMTemplateInfo = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(resInfo.Id, false);
            if (mFSMTemplateInfo == null)
                return;
            Program.CurrentHostAIInstanceInfo = mFSMTemplateInfo;

            BindingOperations.ClearBinding(TextBox_StateName, TextBox.TextProperty);
            BindingOperations.SetBinding(TextBox_StateName, TextBox.TextProperty, new Binding("Name") { Source = mFSMTemplateInfo, Mode=BindingMode.TwoWay, UpdateSourceTrigger=UpdateSourceTrigger.PropertyChanged });

            //LoadStateGrid();
            InitializeFromAIInstance(mFSMTemplateInfo);

            UpdateAddStateComboBox();
        }

        private void Button_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mFSMTemplateInfo == null)
                return;

            FSMTemplateInfoManager.Instance.SaveFSMTemplate(mFSMTemplateInfo.Id, true);
        }

        private void ToggleButton_PreViewCode_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PreviewCode();
        }

        CodeViewerTabItemControl mCodeViewerTabItem;
        private void PreviewCode()
        {
            if(mCodeViewerTabItem != null)
            {
                if (mCodeViewerTabItem.Parent == null)
                    TabControl_Items.Items.Add(mCodeViewerTabItem);
                mCodeViewerTabItem.IsSelected = true;
            }
            else
            {
                mCodeViewerTabItem = new CodeViewerTabItemControl(this.TabControl_Items);
                TabControl_Items.Items.Add(mCodeViewerTabItem);
                mCodeViewerTabItem.IsSelected = true;
            }

            var tw = AIEditor.CodeGenerate.CodeGenerator.GenerateCode(mFSMTemplateInfo, CSUtility.Helper.enCSType.Client);
            mCodeViewerTabItem.Text_Client = tw.ToString();
            tw = AIEditor.CodeGenerate.CodeGenerator.GenerateCode(mFSMTemplateInfo, CSUtility.Helper.enCSType.Server);
            mCodeViewerTabItem.Text_Server = tw.ToString();

        	/*/ 代码预览窗口
            if (mCodeViewer == null || mCodeViewer.IsClosed == true)
            {
                mCodeViewer = new CodeViewer();
            }

            var tw = AIEditor.CodeGenerate.CodeGenerator.GenerateCode(mFSMTemplateInfo, CSUtility.Helper.enCSType.Client);
            mCodeViewer.Text_Client = tw.ToString();
            tw = AIEditor.CodeGenerate.CodeGenerator.GenerateCode(mFSMTemplateInfo, CSUtility.Helper.enCSType.Server);
            mCodeViewer.Text_Server = tw.ToString();
            //mCodeViewer.Text = GenerateCode();

            mCodeViewer.Show();*/
        }


    }
}
