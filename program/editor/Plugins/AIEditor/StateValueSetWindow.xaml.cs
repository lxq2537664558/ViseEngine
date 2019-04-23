using System;
using System.Windows;

namespace AIEditor
{
    /// <summary>
    /// Interaction logic for StateValueSetWindow.xaml
    /// </summary>
    public partial class StateValueSetWindow : DockControl.Controls.DockAbleWindowBase
    {
        public delegate void Delegate_OnUpdateStatePropertys();
        public Delegate_OnUpdateStatePropertys OnUpdateStatePropertys;

        private AIEditor.FSMTemplateInfo mAIInstanceInfo = null;
        private Guid mAIInstanceInfoId = Guid.Empty;
        public Guid AIInstanceInfoId
        {
            get { return mAIInstanceInfoId; }
        }

        private string mStateType = null;
        public string StateType
        {
            get { return mStateType; }
        }

        public StateValueSetWindow(Guid AIInsId, string stateType)
        {
            InitializeComponent();
            this.LayoutManaged = false;
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);

            mAIInstanceInfoId = AIInsId;
            mStateType = stateType;

            mAIInstanceInfo = AIEditor.FSMTemplateInfoManager.Instance.GetFSMTemplateInfo(mAIInstanceInfoId, false);
            if (mAIInstanceInfo != null)
            {
                //ListView_StatePropertys.ItemsSource = mAIInstanceInfo.GetStatePropertys(stateType);
                //ListBox_StatePropertys.ItemsSource = mAIInstanceInfo.GetStatePropertys(stateType);

                ListBox_StatePropertys_Common.Items.Clear();
                ListBox_StatePropertys_Server.Items.Clear();
                ListBox_StatePropertys_Client.Items.Clear();
                foreach (var proInfo in mAIInstanceInfo.GetStatePropertys(mStateType))
                {
                    StateValueSetItem item = new StateValueSetItem(proInfo);

                    switch (proInfo.CSType)
                    {
                        case CSUtility.Helper.enCSType.Common:
                            ListBox_StatePropertys_Common.Items.Add(item);
                            break;

                        case CSUtility.Helper.enCSType.Server:
                            ListBox_StatePropertys_Server.Items.Add(item);
                            break;

                        case CSUtility.Helper.enCSType.Client:
                            ListBox_StatePropertys_Client.Items.Add(item);
                            break;
                    }
                }
            }

            UpdateStatePropertys();
        }

        private void UpdateStatePropertys()
        {
            if (OnUpdateStatePropertys != null)
                OnUpdateStatePropertys();

            return;
            //if (StateType != null || mAIInstanceInfo != null)
            //{
            //    Grid_Propertys.Children.Clear();

            //    Grid grid = new Grid();
            //    var cDef = new ColumnDefinition()
            //    {
            //        Width = new GridLength(1, GridUnitType.Star),
            //        SharedSizeGroup = "TypeWidth"
            //    };
            //    grid.ColumnDefinitions.Add(cDef);
            //    cDef = new ColumnDefinition()
            //    {
            //        Width = new GridLength(3, GridUnitType.Pixel),
            //    };
            //    grid.ColumnDefinitions.Add(cDef);
            //    cDef = new ColumnDefinition()
            //    {
            //        Width = new GridLength(1, GridUnitType.Star),
            //        SharedSizeGroup = "NameWidth"
            //    };
            //    grid.ColumnDefinitions.Add(cDef);
            //    GridSplitter gs = new GridSplitter()
            //    {
            //        HorizontalAlignment = HorizontalAlignment.Stretch,
            //        VerticalAlignment = VerticalAlignment.Stretch,
            //    };
            //    Grid.SetColumn(gs, 1);
            //    grid.Children.Add(gs);
            //    TextBlock tb = new TextBlock()
            //    {
            //        Text = "类型"
            //    };
            //    grid.Children.Add(tb);
            //    tb = new TextBlock()
            //    {
            //        Text = "名称"
            //    };
            //    grid.Children.Add(tb);
            //    Grid_Propertys.Children.Add(grid);

            //    foreach (var pro in mAIInstanceInfo.GetStatePropertys(StateType))
            //    {
            //        AddStateProperty(pro);
            //    }
            //}
        }
        //private void AddStateProperty(AIEditor.FSMTemplateInfo.StatePropertyInfo proInfo)
        //{
        //    Grid grid = new Grid();
        //    var cDef = new ColumnDefinition()
        //    {
        //        Width = new GridLength(1, GridUnitType.Auto),
        //        SharedSizeGroup = "TypeWidth",
        //    };
        //    grid.ColumnDefinitions.Add(cDef);
        //    cDef = new ColumnDefinition()
        //    {
        //        Width = new GridLength(3, GridUnitType.Pixel),
        //    };
        //    grid.ColumnDefinitions.Add(cDef);
        //    cDef = new ColumnDefinition()
        //    {
        //        Width = new GridLength(1, GridUnitType.Auto),
        //        SharedSizeGroup = "NameWidth",
        //    };
        //    grid.ColumnDefinitions.Add(cDef);

        //    ComboBox comb = new ComboBox()
        //    {
        //        ItemsSource = Enum.GetValues(proInfo.StatePropertyTypeEnum.GetType()),
        //    };
        //    comb.SetBinding(ComboBox.SelectedItemProperty, new Binding("StatePropertyTypeEnum") { Source = proInfo });
        //    grid.Children.Add(comb);

        //    TextBox textBox = new TextBox();
        //    Grid.SetColumn(textBox, 2);
        //    textBox.SetBinding(TextBox.TextProperty, new Binding("PropertyName") { Source = proInfo });
        //    grid.Children.Add(textBox);

        //    Grid_Propertys.Children.Add(grid);
        //}

        // Common
        private void Button_AddProperty_Common_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mAIInstanceInfo == null)
                return;

            AIEditor.FSMTemplateInfo.StatePropertyInfo proInfo = new AIEditor.FSMTemplateInfo.StatePropertyInfo(mStateType, CSUtility.Helper.enCSType.Common);
            mAIInstanceInfo.AddStateProperty(mStateType, proInfo);
            var proItem = new StateValueSetItem( proInfo );
            ListBox_StatePropertys_Common.Items.Add(proItem);

            UpdateStatePropertys();
        }

        private void Button_DelProperty_Common_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mAIInstanceInfo == null)
                return;

            //mAIInstanceInfo.RemoveStateProperty(mStateType, ListView_StatePropertys.SelectedIndex);
            if (ListBox_StatePropertys_Common.SelectedIndex < 0)
                return;

            var selItem = ListBox_StatePropertys_Common.SelectedItem as StateValueSetItem;
            mAIInstanceInfo.RemoveStateProperty(mStateType, selItem.Info);
            ListBox_StatePropertys_Common.Items.Remove(selItem);

            UpdateStatePropertys();
        }

        private void Button_AddProperty_Server_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mAIInstanceInfo == null)
                return;

            AIEditor.FSMTemplateInfo.StatePropertyInfo proInfo = new AIEditor.FSMTemplateInfo.StatePropertyInfo(mStateType, CSUtility.Helper.enCSType.Server);
            mAIInstanceInfo.AddStateProperty(mStateType, proInfo);
            var proItem = new StateValueSetItem(proInfo);
            ListBox_StatePropertys_Server.Items.Add(proItem);

            UpdateStatePropertys();
        }
        private void Button_DelProperty_Server_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mAIInstanceInfo == null)
                return;

            if (ListBox_StatePropertys_Server.SelectedIndex < 0)
                return;

            var selItem = ListBox_StatePropertys_Server.SelectedItem as StateValueSetItem;
            mAIInstanceInfo.RemoveStateProperty(mStateType, selItem.Info);
            ListBox_StatePropertys_Server.Items.Remove(selItem);

            UpdateStatePropertys();
        }

        private void Button_AddProperty_Client_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mAIInstanceInfo == null)
                return;

            AIEditor.FSMTemplateInfo.StatePropertyInfo proInfo = new AIEditor.FSMTemplateInfo.StatePropertyInfo(mStateType, CSUtility.Helper.enCSType.Client);
            mAIInstanceInfo.AddStateProperty(mStateType, proInfo);
            var proItem = new StateValueSetItem(proInfo);
            ListBox_StatePropertys_Client.Items.Add(proItem);

            UpdateStatePropertys();
        }
        private void Button_DelProperty_Client_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mAIInstanceInfo == null)
                return;

            if (ListBox_StatePropertys_Client.SelectedIndex < 0)
                return;

            var selItem = ListBox_StatePropertys_Client.SelectedItem as StateValueSetItem;
            mAIInstanceInfo.RemoveStateProperty(mStateType, selItem.Info);
            ListBox_StatePropertys_Client.Items.Remove(selItem);

            UpdateStatePropertys();
        }

    }
}
