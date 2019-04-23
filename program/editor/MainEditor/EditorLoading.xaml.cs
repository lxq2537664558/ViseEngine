using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MainEditor
{
    /// <summary>
    /// EditorLoading.xaml 的交互逻辑
    /// </summary>
    public partial class EditorLoading : ResourceLibrary.WindowBase
    {
        static EditorLoading smInstance = null;
        public static EditorLoading Instance
        {
            get
            {
                if (smInstance == null)
                    smInstance = new EditorLoading();
                return smInstance;
            }
        }

        public EditorLoading()
        {
            InitializeComponent();
        }

        public bool InitializeRuning = false;
        bool mInitializeFinished = false;
        public void DoInitializeEditorMainWindow()
        {
            if (InitializeRuning)
                return;

            if(mInitializeFinished)
            {
                MainWindow.Instance.Show();
            }
            else
            {
                this.Show();
                MainWindow.Instance.AsyncShow();
            }
        }

        public void FinishInitialize()
        {
            InitializeRuning = false;
            mInitializeFinished = true;
            this.Hide();
        }

        public void UpdateProcess(string info, double progressChange)
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBlock_Info.Text = info;
                ProgressBar_Progress.Value += progressChange;
            });
        }
    }
}
