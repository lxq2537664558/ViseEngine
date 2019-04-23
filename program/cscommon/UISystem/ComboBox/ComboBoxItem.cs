namespace UISystem.ComboBox
{
    [CSUtility.Editor.UIEditor_ControlTemplateAble("ComboBoxItem")]
    public class ComboBoxItem : Content.ContentControl
    {
        public delegate void Delegate_OnComboBoxItemSelected(ComboBoxItem item);
        public event Delegate_OnComboBoxItemSelected OnComboBoxItemSelected;
        public delegate void Delegate_OnComboBoxItemUnSelected(ComboBoxItem item);
        public event Delegate_OnComboBoxItemUnSelected OnComboBoxItemUnSelected;

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
                    if (OnComboBoxItemSelected != null)
                        OnComboBoxItemSelected(this);
                }
                else
                {
                    if (OnComboBoxItemUnSelected != null)
                        OnComboBoxItemUnSelected(this);
                }

                OnPropertyChanged("IsSelected");
            }
        }

        public ComboBoxItem()
        {
            WinMouseButtonDown += ComboBoxItem_WinMouseButtonDown;
        }

        void ComboBoxItem_WinMouseButtonDown(WinBase sender, Message.MouseEventArgs e)
        {
            IsSelected = true;
        }
    }
}
