using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DelegateMethodEditor
{
    /// <summary>
    /// Interaction logic for CodeViewer.xaml
    /// </summary>
    public partial class CodeViewer : UserControl, INotifyPropertyChanged
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

        //bool mIsClosed = false;
        //public bool IsClosed
        //{
        //    get { return mIsClosed; }
        //}

        public CSUtility.Helper.enCSType CSType = CSUtility.Helper.enCSType.Common;

        public Guid EventId
        {
            get;
            set;
        }

        public string Text_Code
        {
            get { return TextEditor_Code.Text; }
            set
            {
                TextEditor_Code.Text = value;

                ListView_Errors.ItemsSource = DelegateMethodEditor.CodeGenerator.CodeGenerator.CompileCode(value, CSType, EventId).Errors;
            }
        }

        //public string Text_Server
        //{
        //    get { return TextEditor_Code_Server.Text; }
        //    set
        //    {
        //        TextEditor_Code_Server.Text = value;

        //        ListView_Errors_Server.ItemsSource = DelegateMethodEditor.CodeGenerator.CodeGenerator.CompileCode(value, CSUtility.Helper.enCSType.Server, EventId).Errors;
        //    }
        //}

        public CodeViewer()
        {
            InitializeComponent();
            
            TextEditor_Code.ShowLineNumbers = true;
            //TextEditor_Code_Server.ShowLineNumbers = true;
        }
    }
}
