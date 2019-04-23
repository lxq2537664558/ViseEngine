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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainEditor.BackgroundWorkerView
{
    /// <summary>
    /// BackgroundWorkerViewer.xaml 的交互逻辑
    /// </summary>
    public partial class BackgroundWorkerViewer : UserControl
    {
        CSUtility.Support.AsyncObjManager<string, BGViewItem> mItemDic = new CSUtility.Support.AsyncObjManager<string, BGViewItem>();
        bool mShow = false;
        public BackgroundWorkerViewer()
        {
            InitializeComponent();
            InitializeBackgroundWorkerView();
        }

        public void InitializeBackgroundWorkerView()
        {
            EditorCommon.Background.BackgroundWorkerViewManager.Instance.BackgroundWorkerViewUpdateAction = UpdateBackgroundWorkerView;
        }
        public void UpdateBackgroundWorkerView()
        {
            if (mItemDic.Count <= 0 && EditorCommon.Background.BackgroundWorkerViewManager.Instance.ViewDatas.Count <= 0)
                return;

            // 删除不存在的对象
            mItemDic.For_Each((string key, BGViewItem value, object arg)=>
            {
                if(!EditorCommon.Background.BackgroundWorkerViewManager.Instance.ViewDatas.ContainsKey(key))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        StackPanel_BGView.Children.Remove(value);
                    });
                    return CSUtility.Support.EForEachResult.FER_Erase;
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            EditorCommon.Background.BackgroundWorkerViewManager.Instance.ViewDatas.For_Each((string key, EditorCommon.Background.BackgroundWorkerViewManager.BackgroundWorkerViewData value, object arg) =>
            {
                BGViewItem item;
                if(!mItemDic.TryGetValue(key, out item))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        item = new BGViewItem(value.UseProgress)
                        {
                            KeyName = key,
                            InfoString = value.CurrentInfo,
                            Progress = value.Progress,
                        };

                        mItemDic[key] = item;
                        StackPanel_BGView.Children.Add(item);
                    });
                }
                else
                {
                    item.KeyName = key;
                    item.InfoString = value.CurrentInfo;
                    item.Progress = value.Progress;
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            if(mItemDic.Count > 0 && !mShow)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var sb = this.FindResource("Storyboard_StartShow") as Storyboard;
                    if (sb != null)
                        sb.Begin();
                });
                mShow = true;
            }
            else if(mItemDic.Count == 0 && mShow)
            {
                this.Dispatcher.Invoke(() =>
                {
                    BackgroundWorkPopup.IsOpen = false;
                    var sb = this.FindResource("Storyboard_EndShow") as Storyboard;
                    if (sb != null)
                        sb.Begin();
                });
                mShow = false;
            }
        }
    }
}
