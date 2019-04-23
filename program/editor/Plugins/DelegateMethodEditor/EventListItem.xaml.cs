using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DelegateMethodEditor
{
    /// <summary>
    /// Interaction logic for EventListItem.xaml
    /// </summary>
    public partial class EventListItem : UserControl, INotifyPropertyChanged
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

        CSUtility.Helper.EventCallBack mEventCallBack;
        public CSUtility.Helper.EventCallBack EventCallBack
        {
            get { return mEventCallBack; }
        }

        System.Reflection.MethodInfo mMethodInfo;
        public System.Reflection.MethodInfo MethodInfo
        {
            get { return mMethodInfo; }
        }

        public Guid EventId
        {
            get
            {
                if (mEventCallBack != null)
                    return mEventCallBack.Id;

                return Guid.Empty;
            }
        }

        string mNickName = "";
        public string NickName
        {
            get { return mNickName; }
            set
            {
                mNickName = value;

                if (mEventCallBack != null)
                    mEventCallBack.NickName = mNickName;

                IsDirty = true;

                OnPropertyChanged("NickName");
            }
        }

        string mDescription = "";
        public string Description
        {
            get { return mDescription; }
            set
            {
                mDescription = value;

                TextBlock_NickName.ToolTip = mDescription;// +"(双击修改)";

                if (mEventCallBack != null)
                    mEventCallBack.Description = mDescription;

                IsDirty = true;

                OnPropertyChanged("Description");
            }
        }

        string mTempNickName;
        string mTempDescription;

        bool mCommonHasLinks = false;
        public bool CommonHasLinks
        {
            get { return mCommonHasLinks; }
            set
            {
                mCommonHasLinks = value;

                if (mCommonHasLinks)
                {
                    Rect_Common.Fill = this.FindResource("CommonHasLinksBrush") as Brush;
                    Rect_Common.ToolTip = "公共包含实现";
                }
                else
                {
                    Rect_Common.Fill = this.FindResource("DontHasLinksBrush") as Brush;
                    Rect_Common.ToolTip = "公共没有实现";
                }
            }
        }
        bool mClientHasLinks = false;
        public bool ClientHasLinks
        {
            get { return mClientHasLinks; }
            set
            {
                mClientHasLinks = value;

                if (mClientHasLinks)
                {
                    Rect_Client.Fill = this.FindResource("ClientHasLinksBrush") as Brush;
                    Rect_Client.ToolTip = "客户端包含实现";
                }
                else
                {
                    Rect_Client.Fill = this.FindResource("DontHasLinksBrush") as Brush;
                    Rect_Client.ToolTip = "客户端没有实现";
                }
            }
        }
        bool mServerHasLinks = false;
        public bool ServerHasLinks
        {
            get { return mServerHasLinks; }
            set
            {
                mServerHasLinks = value;

                if (mServerHasLinks)
                {
                    Rect_Server.Fill = this.FindResource("ServerHasLinksBrush") as Brush;
                    Rect_Server.ToolTip = "服务器端包含实现";
                }
                else
                {
                    Rect_Server.Fill = this.FindResource("DontHasLinksBrush") as Brush;
                    Rect_Server.ToolTip = "服务器端没有实现";
                }
            }
        }

        bool mIsDirty = false;
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;

                if (mIsDirty)
                    TextBlock_UnSaveTip.Visibility = Visibility.Visible;
                else
                    TextBlock_UnSaveTip.Visibility = Visibility.Collapsed;

                OnPropertyChanged("IsDirty");
            }
        }
        bool mCommonIsDirty = false;
        public bool CommonIsDirty
        {
            get { return mCommonIsDirty; }
            set
            {
                mCommonIsDirty = value;

                if (mCommonIsDirty)
                    IsDirty = true;

                OnPropertyChanged("CommonIsDirty");
            }
        }
        bool mClientIsDirty = false;
        public bool ClientIsDirty
        {
            get { return mClientIsDirty; }
            set
            {
                mClientIsDirty = value;

                if (mClientIsDirty)
                    IsDirty = true;

                OnPropertyChanged("ClientIsDirty");
            }
        }
        bool mServerIsDirty = false;
        public bool ServerIsDirty
        {
            get { return mServerIsDirty; }
            set
            {
                mServerIsDirty = value;

                if (mServerIsDirty)
                    IsDirty = true;

                OnPropertyChanged("ServerIsDirty");
            }
        }

        public EventListItem(CSUtility.Helper.EventCallBack eventCallBack)
        {
            InitializeComponent();

            mEventCallBack = eventCallBack;
            if (mEventCallBack != null)
            {
                NickName = mEventCallBack.NickName;
                Description = mEventCallBack.Description;

                mMethodInfo = mEventCallBack.CBType.GetMethod("Invoke");
            }

            IsDirty = false;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                TextBlock_NickName.Visibility = System.Windows.Visibility.Collapsed;
                Grid_Edit.Visibility = System.Windows.Visibility.Visible;
                mTempNickName = NickName;
                mTempDescription = Description;
            }
        }

        private void Button_OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBlock_NickName.Visibility = System.Windows.Visibility.Visible;
            Grid_Edit.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBlock_NickName.Visibility = System.Windows.Visibility.Visible;
            Grid_Edit.Visibility = System.Windows.Visibility.Collapsed;
            NickName = mTempNickName;
            Description = mTempDescription;
        }


        public void OnContainLinkNodesChanged(bool contain, CSUtility.Helper.enCSType csType)
        {
            switch (csType)
            {
                case CSUtility.Helper.enCSType.Common:
                    CommonHasLinks = contain;
                    break;
                case CSUtility.Helper.enCSType.Client:
                    ClientHasLinks = contain;
                    break;
                case CSUtility.Helper.enCSType.Server:
                    ServerHasLinks = contain;
                    break;
            }
        }

        public void OnCommonLinkControlDirtyChanged(bool dirty)
        {
            CommonIsDirty = dirty;
        }
        public void OnClientLinkControlDirtyChanged(bool dirty)
        {
            ClientIsDirty = dirty;
        }
        public void OnServerLinkControlDirtyChanged(bool dirty)
        {
            ServerIsDirty = dirty;
        }

        public static string GetFileName(Guid eventId, CSUtility.Helper.enCSType csType)
        {
            return eventId.ToString() + "_" + csType.ToString() + ".xml";
        }

        public void Load(CSUtility.Support.XmlHolder holder, CSUtility.Helper.enCSType csType)
        {
            EventCallBack.Load(holder, csType);

            var att = holder.RootNode.FindAttrib("CommonHasLinks");
            if (att != null)
            {
                CommonHasLinks = System.Convert.ToBoolean(att.Value);
            }
            att = holder.RootNode.FindAttrib("ClientHasLinks");
            if (att != null)
            {
                ClientHasLinks = System.Convert.ToBoolean(att.Value);
            }
            att = holder.RootNode.FindAttrib("ServerHasLinks");
            if (att != null)
            {
                ServerHasLinks = System.Convert.ToBoolean(att.Value);
            }

            IsDirty = false;
        }

        public void Save(CSUtility.Support.XmlHolder holder, CSUtility.Helper.enCSType csType)
        {
            EventCallBack.Save(holder, csType);

            holder.RootNode.AddAttrib("CommonHasLinks", CommonHasLinks.ToString());
            holder.RootNode.AddAttrib("ClientHasLinks", ClientHasLinks.ToString());
            holder.RootNode.AddAttrib("ServerHasLinks", ServerHasLinks.ToString());

            IsDirty = false;
        }

        public void Copy(EventListItem item)
        {
            mEventCallBack = item.EventCallBack;
            mMethodInfo = item.MethodInfo;
            mNickName = item.NickName;
            mDescription = item.Description;
            CommonHasLinks = item.CommonHasLinks;
            ClientHasLinks = item.ClientHasLinks;
            ServerHasLinks = item.ServerHasLinks;
            IsDirty = false;
        }
    }
}
