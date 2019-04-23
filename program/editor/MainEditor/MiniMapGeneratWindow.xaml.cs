using System;
using System.Windows;
using System.ComponentModel;

namespace MainEditor
{
    /// <summary>
    /// Interaction logic for MiniMapGeneratWindow.xaml
    /// </summary>
    public partial class MiniMapGeneratWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        UInt32 mPicSize = 256;
        public UInt32 PicSize
        {
            get { return mPicSize; }
            set
            {
                mPicSize = value;
                OnPropertyChanged("PicSize");
            }
        }

        string mPath = "";
        public string Path
        {
            get { return mPath; }
            set
            {
                mPath = value;
                OnPropertyChanged("Path");
            }
        }

        UInt32 mLevelCountDelta = 1;
        public UInt32 LevelCountDelta
        {
            get { return mLevelCountDelta; }
            set
            {
                mLevelCountDelta = value;
                LevelCountDis = LevelCountDelta + "X" + LevelCountDelta;
                OnPropertyChanged("LevelCountDelta");
            }
        }

        string mLevelCountDis = "";
        public string LevelCountDis
        {
            get { return mLevelCountDis; }
            set
            {
                mLevelCountDis = value;
                OnPropertyChanged("LevelCountDis");
            }
        }

        public MiniMapGeneratWindow()
        {
            InitializeComponent();

            if (CCore.Client.MainWorldInstance != null)
            {
#warning 这里要计算场景的拆分格子
                var levelCountX = 1;// CCore.Client.MainWorldInstance.WorldInit.SceneSizeX;
                var levelCountZ = 1;// CCore.Client.MainWorldInstance.WorldInit.SceneSizeZ;

                var minLevelCount = System.Math.Min(levelCountX, levelCountZ);

                this.Slider_LevelCountDelta.Minimum = 1;
                this.Slider_LevelCountDelta.Maximum = minLevelCount;
                LevelCountDelta = 1;
            }
        }

        private void Button_SetFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Path = dlg.SelectedPath;
            }
        }

        private void Button_OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(mPath))
            {
                EditorCommon.MessageBox.Show("请先设置路径!");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Button_Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
