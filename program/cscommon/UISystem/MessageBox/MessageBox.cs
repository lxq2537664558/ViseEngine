namespace UISystem
{
    public enum VMessageBoxType 
    {
        Ok,
        OkCancel,
    }

    public class VMessageBox : UISystem.UIBindAutoUpdate
    {
        public enum eMessageBoxResult
        {
            Unknow,
            OK,
            Cancel,
        }

        public delegate void Delegate_OnButtonClicked(eMessageBoxResult result);
        public Delegate_OnButtonClicked OnButtonClicked;

        static VMessageBox smInstance = new VMessageBox();
        static public VMessageBox Instance
        {
            //get{return smInstance;}
            get
            {
                return CCore.Support.ReflectionManager.Instance.GetClassObject(typeof(VMessageBox)) as VMessageBox;
            }
        }

        string mTitle;
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public string Title
        {
            get { return mTitle; }
            set
            {
                mTitle = value;
                OnPropertyChanged("Title");
            }
        }

        string mInformation;
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public string Information
        {
            get { return mInformation; }
            set
            {
                mInformation = value;
                OnPropertyChanged("Information");
            }
        }

        string mOkContent = "确定";
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public string OkContent
        {
            get { return mOkContent; }
            set
            {
                mOkContent = value;
                OnPropertyChanged("OkContent");
            }
        }

        string mCancelContent = "取消";
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public string CancelContent
        {
            get { return mCancelContent; }
            set
            {
                mCancelContent = value;
                OnPropertyChanged("CancelContent");
            }
        }

        Visibility mOkVisibility = Visibility.Visible;
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public Visibility OkVisibility
        {
            get { return mOkVisibility; }
            set
            {
                mOkVisibility = value;
                OnPropertyChanged("OkVisibility");
            }
        }

        Visibility mCancelVisibility = Visibility.Visible;
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        public Visibility CancelVisibility
        {
            get { return mCancelVisibility; }
            set
            {
                mCancelVisibility = value;
                OnPropertyChanged("CancelVisibility");
            }
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void OKButtonClick(WinBase sender, CCore.MsgProc.BehaviorParameter beh)
        {
            if (OnButtonClicked != null)
                OnButtonClicked(eMessageBoxResult.OK);
            this.Hide();
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void CancelButtonClick(WinBase sender, CCore.MsgProc.BehaviorParameter beh)
        {
            if (OnButtonClicked != null)
                OnButtonClicked(eMessageBoxResult.Cancel);
            this.Hide();
        }

        private eMessageBoxResult CheckResult(int callDuration, out int threadId)
        {
            System.Threading.Thread.Sleep(5000);
            threadId = 0;
            return eMessageBoxResult.Cancel;
        }

        public void Show(UIInterface parent, string title, string info)
        {
            Show(parent, title, info, "确定", "取消", VMessageBoxType.Ok);
        }

        public void Show(UIInterface parent, string title, string info, VMessageBoxType type)
        {
            Show(parent, title, info, "确定", "取消", type);
        }

        public void Hide()
        {
            var form = CCore.Support.ReflectionManager.Instance.GetUIForm("VMessageBox");
            if (null != form)
                form.Visibility = Visibility.Collapsed;
        }

        public void ProcTopMostWhenVisible()
        {
            var form = CCore.Support.ReflectionManager.Instance.GetUIForm("VMessageBox");
            if (form != null && form.Parent!=null && form.Visibility == Visibility.Visible)
            {
                var parent = form.Parent;
                form.Parent = null;
                form.Parent = parent;
            }
        }

        public void Show(UIInterface parent, string title, string info, string ok, string cancel, VMessageBoxType type)
        {
            OnButtonClicked = null;

            var form = CCore.Support.ReflectionManager.Instance.GetUIForm("VMessageBox") as UISystem.WinBase;
            if (form != null)
            {
                Title = title;
                Information = info;
                OkContent = ok;
                CancelContent = CancelContent;

                switch (type)
                {
                    case VMessageBoxType.Ok:
                        {
                            OkVisibility = Visibility.Visible;
                            CancelVisibility = Visibility.Collapsed;
                        }
                        break;
                    case VMessageBoxType.OkCancel:
                        {
                            OkVisibility = Visibility.Visible;
                            CancelVisibility = Visibility.Visible;
                        }
                        break;
                    default:
                        break;
                }

                form.Parent = null;
                form.Parent = parent;
                form.Visibility = Visibility.Visible;
                form.UpdateLayout();
            }
        }
    }
}
