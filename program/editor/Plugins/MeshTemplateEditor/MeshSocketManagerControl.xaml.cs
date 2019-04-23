using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MeshTemplateEditor
{
    /// <summary>
    /// Interaction logic for MeshSocketManagerControl.xaml
    /// </summary>
    public partial class MeshSocketManagerControl : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public delegate void Delegate_OnSelectedChanged(string socketName);
        public Delegate_OnSelectedChanged OnSelectedChanged;

        MainControl mHostEditor = null;
        public MainControl HostEditor
        {
            get { return mHostEditor; }
        }

        public MeshSocketManagerControl()
        {
            InitializeComponent();
            InitializeComponentInfoList();
        }

        string mSocketFilterString = "";
        public string SocketComponentInfoCreateFilterString
        {
            get { return mSocketFilterString; }
            set
            {
                mSocketFilterString = value;

                if (string.IsNullOrEmpty(mSocketFilterString))
                {
                    foreach (var child in StackPanel_Components.Children)
                    {
                        var btn = child as Button;
                        if (btn == null)
                            continue;
                        var textBlock = btn.Content as EditorControlLib.CustomTextBlock;
                        if (textBlock == null)
                            continue;

                        btn.Visibility = Visibility.Visible;
                        textBlock.HighLightString = "";
                    }
                }
                else
                {
                    foreach (var child in StackPanel_Components.Children)
                    {
                        var btn = child as Button;
                        if (btn == null)
                            continue;
                        var textBlock = btn.Content as EditorControlLib.CustomTextBlock;
                        if (textBlock == null)
                            continue;

                        if (textBlock.Text.IndexOf(mSocketFilterString, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            textBlock.HighLightString = "";
                            btn.Visibility = Visibility.Collapsed;
                            continue;
                        }

                        textBlock.HighLightString = mSocketFilterString;
                        btn.Visibility = Visibility.Visible;
                    }
                }

                OnPropertyChanged("SocketComponentInfoCreateFilterString");
            }
        }

        private void InitializeComponentInfoList()
        {
            StackPanel_Components.Children.Clear();

            foreach (var assem in CSUtility.Program.GetAnalyseAssemblys(CSUtility.Helper.enCSType.Client))
            {
                foreach(var type in assem.GetTypes())
                {
                    if(type.GetInterface(typeof(CCore.Socket.ISocketComponentInfo).FullName) != null)
                    {
                        var att = CSUtility.Helper.AttributeHelper.GetCustomAttribute(type, "CCore.Socket.SocketComponentInfoAttribute", false);
                        if (att == null)
                            continue;
                        var disName = (string)(CSUtility.Helper.AttributeHelper.GetCustomAttributePropertyValue(att, "DisplayName"));
                        if (string.IsNullOrEmpty(disName))
                            continue;

                        var btn = new Button()
                        {
                            Style = TryFindResource(new ComponentResourceKey(typeof(ResourceLibrary.CustomResources), "ButtonStyle_Default")) as Style,
                            Content = new EditorControlLib.CustomTextBlock()
                            {
                                Text = disName
                            },
                            Tag = type,
                            Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)),
                            BorderBrush = null,
                        };
                        btn.Click += ComponentCreateButton_Click;

                        StackPanel_Components.Children.Add(btn);
                    }
                }
            }
        }

        private void ComponentCreateButton_Click(object sender, RoutedEventArgs e)
        {
            toggleButton_AddComponent.IsChecked = false;

            if (HostEditor == null)
                return;
            if (HostEditor.CurMeshTemplate == null || HostEditor.CurMesh == null)
                return;
            if (HostEditor.CurSocketShowData == null)
            {
                EditorCommon.MessageBox.Show("请先选中一个Socket再点击添加!");
                return;
            }

            var btn = sender as Button;
            var tagType = (Type)(btn.Tag);

            var comInfo = System.Activator.CreateInstance(tagType) as CCore.Socket.ISocketComponentInfo;
            if (comInfo == null)
                return;

            comInfo.SocketName = HostEditor.CurSocketShowData.SocketName;
            HostEditor.CurMeshTemplate.SocketComponentInfoList[comInfo.SocketComponentInfoId] = comInfo;
            HostEditor.CurMeshTemplate.IsDirty = true;

            var comp = HostEditor.CurMesh.AddSocketItem(comInfo);

            mSocketInfoList.Add(comInfo);
            ListBox_Sockets.SelectedIndex = mSocketInfoList.Count - 1;
            ((INotifyPropertyChanged)comInfo).PropertyChanged += SocketComponentInfo_PropertyChanged;
        }

        private void SocketComponentInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var info = sender as CCore.Socket.ISocketComponentInfo;
            //HostEditor.CurMeshTemplate.SocketComponentInfoList.Remove(info.SocketComponentInfoId);
            HostEditor.CurMeshTemplate.IsDirty = true;

            HostEditor.CurMesh.RemoveSocketItem(info.SocketComponentInfoId);
            var comp = HostEditor.CurMesh.AddSocketItem(info);
        }

        ObservableCollection<CCore.Socket.ISocketComponentInfo> mSocketInfoList = new ObservableCollection<CCore.Socket.ISocketComponentInfo>();
        public ObservableCollection<CCore.Socket.ISocketComponentInfo> SocketInfoList
        {
            get { return mSocketInfoList; }
            set
            {
                mSocketInfoList = value;
                OnPropertyChanged("SocketInfoList");
            }
        }

        public void Initialize(MainControl hostEditor)
        {
            if (hostEditor == null)
                return;

            mHostEditor = hostEditor;

            SocketInfoList.Clear();
            hostEditor?.CurMeshTemplate?.SocketComponentInfoList?.For_Each((System.Guid id, CCore.Socket.ISocketComponentInfo info, object arg) =>
            {
                //var socketItem = hostEditor.CurMesh.AddSocketItem(info);
                info.PropertyChanged += SocketComponentInfo_PropertyChanged;

                SocketInfoList.Add(info);

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }
        
        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HostEditor == null)
                return;

            if (ListBox_Sockets.SelectedIndex < 0)
                return;

            if (HostEditor.CurMeshTemplate == null || HostEditor.CurMesh == null)
                return;

            var compInfo = mSocketInfoList[ListBox_Sockets.SelectedIndex];
            HostEditor.CurMeshTemplate.SocketComponentInfoList.Remove(compInfo.SocketComponentInfoId);
            HostEditor.CurMeshTemplate.IsDirty = true;

            HostEditor.CurMesh.RemoveSocketItem(compInfo.SocketComponentInfoId);
            mSocketInfoList.Remove(compInfo);
        }

        private void ListBox_Sockets_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OnSelectedChanged == null)
                return;

            if (ListBox_Sockets.SelectedIndex < 0)
            {
                OnSelectedChanged("");
                return;
            }
            
            var info = ListBox_Sockets.SelectedItem as CCore.Socket.ISocketComponentInfo;
            if (info == null)
            {
                OnSelectedChanged("");
                return;
            }
            OnSelectedChanged(info.SocketName);

            HostEditor.SocketItemProGrid.Instance = info;
        }
        
        // 点击标题头排序
        private void ListViewColumn_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader)
            {
                //获得点击的列  
                GridViewColumn clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column;
                //数据源集合为ObservableCollection<T>类型，保证数据源和ListView试图同步。                            
                CCore.Socket.ISocketComponentInfo[] infos = new CCore.Socket.ISocketComponentInfo[SocketInfoList.Count];
                SocketInfoList.CopyTo(infos, 0);
                var list = new List<CCore.Socket.ISocketComponentInfo>(infos);
                if (clickedColumn != null)
                {
                    list.Sort(delegate (CCore.Socket.ISocketComponentInfo a, CCore.Socket.ISocketComponentInfo b)
                    {
                        switch (clickedColumn.Header.ToString())
                        {
                            case "挂接点":
                                return a.SocketName.CompareTo(b.SocketName);
                            case "类型":
                                return a.SocketComponentType.CompareTo(b.SocketComponentType);
                            case "说明":
                                return a.Description.CompareTo(b.Description);
                            default:
                                break;
                        }
                        return 0;
                    });
                    SocketInfoList.Clear();
                    foreach (CCore.Socket.ISocketComponentInfo item in list)
                    {
                        SocketInfoList.Add(item);
                    }
                }
            }
        }
    }
}
