using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace UVAnimEditor
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "UVAnimEditor")]
    //[EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/UVAnimEditor")]
    [Guid("5379FC7D-4167-4EA3-A661-DF4B47E0497C")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainControl : UserControl, EditorCommon.PluginAssist.IEditorPlugin//DockControl.IDockAbleControl
    {
        public string PluginName
        {
            get { return "UI图元编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "UI图元编辑器",
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
                SetUVAnimFile((UVAnimResourceInfo)(obj[0]));
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

        //public static string StaticKeyValue
        //{
        //    get { return "UVAnim Editor"; }
        //}
        //public bool IsShowing { get; set; }
        //public string KeyValue
        //{
        //    get { return StaticKeyValue; }
        //}

        //public void SaveElement(CSUtility.Support.XmlNode node, CSUtility.Support.XmlHolder holder)
        //{

        //}
        //public void LoadElement()
        //{

        //}

        public delegate void Delegate_OnSaveSource();
        public Delegate_OnSaveSource OnSaveSource;

        UISystem.UVAnim mUVAnim;
        UVAnimResourceInfo mResInfo;

        //public List<UISystem.UVFrame> UVAnimFrames
        //{
        //    get { return (List<UISystem.UVFrame>)GetValue(UVAnimFramesProperty); }
        //    set { SetValue(UVAnimFramesProperty, value); }
        //}
        //public static readonly DependencyProperty UVAnimFramesProperty = DependencyProperty.Register("UVAnimFrames", typeof(List<UISystem.UVFrame>), typeof(MainControl),
        //                                        new FrameworkPropertyMetadata(new List<UISystem.UVFrame>(), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnUVAnimFramesChanged)));

        //public static void OnUVAnimFramesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //}

        public Visibility EditorVisible
        {
            get { return (Visibility)GetValue(EditorVisibleProperty); }
            set { SetValue(EditorVisibleProperty, value); }
        }
        public static readonly DependencyProperty EditorVisibleProperty = DependencyProperty.Register("EditorVisible", typeof(Visibility), typeof(MainControl),
                                                        new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnEditorVisibleChanged)));

        public static void OnEditorVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainControl control = d as MainControl;
            Visibility value = (Visibility)e.NewValue;

            var column1 = control.Grid_Main.ColumnDefinitions[Grid.GetColumn(control.ImagePanel)];
            var column2 = control.Grid_Main.ColumnDefinitions[2];

            if (value == Visibility.Visible)
            {
                column1.Width = column2.Width;
            }
            else
            {
                column1.Width = GridLength.Auto;
            }
        }

        //Visibility mEditorVisible = Visibility.Collapsed;
        //public Visibility EditorVisible
        //{
        //    get { return mEditorVisible; }
        //    set
        //    {
        //        mEditorVisible = value;

        //        ImagePanel.Visibility = mEditorVisible;
        //        GridSplitter_Editor.Visibility = mEditorVisible;
        //        Button_Add.Visibility = mEditorVisible;
        //        Button_Del.Visibility = mEditorVisible;



        //        OnPropertyChanged("EditorVisible");
        //    }
        //}

        public MainControl()
        {
            InitializeComponent();

            ImagePanel.OnUpdateUVAnimFrames = new ImageOperationPanel.Delegate_OnUpdateUVAnimFrames(_OnUpdateUVAnimFrames);
            ImagePanel.OnSaveSource = new ImageOperationPanel.Delegate_OnSaveSource(_OnSaveSource);
        }

        private void _OnUpdateUVAnimFrames(UISystem.UVAnim uvAnim)
        {
            ListBox_Frames.Items.Clear();
            for (int i = 0; i < mUVAnim.Frames.Count; i++)
            {
                ListBox_Frames.Items.Add("关键帧" + i);
            }
            Property_Frame.Instance = null;
            if (ListBox_Frames.Items.Count > 0)
                ListBox_Frames.SelectedIndex = 0;
            else
                ListBox_Frames.SelectedIndex = 1;
        }

        private void _OnSaveSource()
        {
            if (mResInfo != null)
                mResInfo.Save();

            //Program.SaveUVAnim(mResInfo);
            //             if (mResInfo != null)
            //             {
            //                 if (string.IsNullOrEmpty(mResInfo.UVAnim.UVAnimName))
            //                 {
            //                     EditorCommon.MessageBox.Show("UVAnimName未设置，请设置后再保存!");
            //                     return;
            //                 }
            // 
            //                 UISystem.UVAnimMgr.Instance.SaveUVAnim(mResInfo.UVAnim.Id);
            //                 EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult result) =>
            //                 {
            //                     if(result.Result != EditorCommon.VersionControl.EProcessResult.Success)
            //                     {
            // 
            //                     }
            //                 }, mResInfo.AbsResourceFileName, "AutoCommit");
            //                 //SvnInterface.Commander.Instance.Commit(mResInfo.AbsFileName);
            // 
            //                 //mResInfo.HostControl.Browser.ReCreateSnapshot(mResInfo);
            //                 EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult result) =>
            //                 {
            //                     if(result.Result != EditorCommon.VersionControl.EProcessResult.Success)
            //                     {
            // 
            //                     }
            //                 }, mResInfo.AbsResourceFileName + "_Snapshot.png", "AutoCommit");
            //                 //SvnInterface.Commander.Instance.Commit(mResInfo.AbsFileName + "_Snapshot.png");
            //             }
        }

        public void SetUVAnimFile(UVAnimResourceInfo resInfo)
        {
            ListBox_Frames.Items.Clear();

            mResInfo = resInfo;
            //var file = fileName.Replace("\\", "/").Replace(CSUtility.Support.IFileManager.Instance.Root, "");
            //file = file.Remove(file.LastIndexOf('.'));
            //file = file.Remove(0, file.LastIndexOf('/') + 1);
            mUVAnim = mResInfo.UVAnim;// UISystem.UVAnimMgr.Instance.Find(Guid.Parse(file), true);

            //BindingOperations.ClearBinding(this, UVAnimFramesProperty);
            //BindingOperations.SetBinding(this, UVAnimFramesProperty, new Binding("Frames")
            //{
            //    Source = mUVAnim,
            //    Mode = BindingMode.TwoWay,
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            //});

            //var sourcePath = fileName.Replace("\\", "/").Replace(CSUtility.Support.IFileManager.Instance.Root, "");
            //mUVAnim = new UISystem.UVAnim();
            //CSUtility.Support.IConfigurator.FillProperty(mUVAnim, sourcePath);

            Property_UVAnim.Instance = mUVAnim;

            //foreach (var frame in mUVAnim.Frames)
            for(int i=0; i<mUVAnim.Frames.Count; i++)
            {
                ListBox_Frames.Items.Add("关键帧" + i);
            }

            //ImagePanel.SetImageFile(mUVAnim.Texture);
            ImagePanel.SetUVAnim(mUVAnim);
            PreView.SetUVAnim(mUVAnim);

            Property_Frame.Instance = null;
            if(ListBox_Frames.Items.Count > 0)
                ListBox_Frames.SelectedIndex = 0;
            else
                ListBox_Frames.SelectedIndex = 1;
        }

        private void ListBox_Frames_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (mUVAnim == null)
                return;

            if (ListBox_Frames.SelectedIndex < 0 || ListBox_Frames.SelectedIndex >= mUVAnim.Frames.Count)
                return;

            Property_Frame.Instance = mUVAnim.Frames[ListBox_Frames.SelectedIndex];
            ImagePanel.SelectedFrame(ListBox_Frames.SelectedIndex);
        }

        private void Button_Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mUVAnim == null)
                return;

            //UISystem.UVFrame frame = new UISystem.UVFrame();
            //frame.mParentAnim = mUVAnim;
            //mUVAnim.Frames.Add(frame);
            mUVAnim.AddFrame();
            ListBox_Frames.Items.Add("关键帧" + (mUVAnim.Frames.Count - 1));

            ImagePanel.AddFrame();
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mUVAnim == null)
                return;

            // 至少有一帧
            if (mUVAnim.Frames.Count <= 1)
                return;

            if (ListBox_Frames.SelectedIndex < 0 || ListBox_Frames.SelectedIndex >= mUVAnim.Frames.Count)
                return;

            ImagePanel.RemoveFrame(ListBox_Frames.SelectedIndex);

            //mUVAnim.Frames.RemoveAt(ListBox_Frames.SelectedIndex);
            mUVAnim.DelFrame(ListBox_Frames.SelectedIndex);
            ListBox_Frames.Items.RemoveAt(ListBox_Frames.SelectedIndex);

            for (int i = 0; i < ListBox_Frames.Items.Count; i++)
            {
                ListBox_Frames.Items[i] = "关键帧" + i;
            }
        }
    }
}
