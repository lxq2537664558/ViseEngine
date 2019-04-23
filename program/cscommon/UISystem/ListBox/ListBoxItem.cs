namespace UISystem.ListBox
{
    [CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute("ListBoxItem")]
    public class ListBoxItem : Content.ContentControl
    {
        public delegate void Delegate_OnListBoxItemSelected(ListBoxItem item);
        public event Delegate_OnListBoxItemSelected OnListBoxItemSelected;
        public delegate void Delegate_OnListBoxItemUnSelected(ListBoxItem item);
        public event Delegate_OnListBoxItemUnSelected OnListBoxItemUnSelected;

        protected bool mIsSelected = false;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                if (mIsSelected == value)
                    return;

                mIsSelected = value;

                if (mIsSelected)
                {
                    if (OnListBoxItemSelected != null)
                        OnListBoxItemSelected(this);
                }
                else
                {
                    if (OnListBoxItemUnSelected != null)
                        OnListBoxItemUnSelected(this);
                }

                OnPropertyChanged("IsSelected");
            }
        }

        //protected override MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    return base.OnMsg(ref msg);
        //}

        public ListBoxItem()
        {
            WinMouseButtonDown += ListBoxItem_WinMouseButtonDown;
        }

        void ListBoxItem_WinMouseButtonDown(WinBase sender, Message.MouseEventArgs e)
        {
            IsSelected = true;
        }
    }
}
