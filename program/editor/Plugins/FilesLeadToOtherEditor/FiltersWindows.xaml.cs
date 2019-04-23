using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace FilesLeadToOtherEditor
{
    /// <summary>
    /// FiltersWindows.xaml 的交互逻辑
    /// </summary>
    public partial class FiltersWindows : ResourceLibrary.WindowBase
    {
        //#region INotifyPropertyChangedMembers
        //public event PropertyChangedEventHandler PropertyChanged;
        //private void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = this.PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
        //#endregion

        FilesLeadToOther mFilesLeadToOther = null;

        public FiltersWindows(FilesLeadToOther Parent)
        {
            InitializeComponent();

            mFilesLeadToOther = Parent;
        }

        //public void ShowWindow()
        //{
        //    string filterFileStrs = "";
        //    for (int i = 0; i < mFilterFileLines.Count; i++)
        //    {
        //        filterFileStrs += mFilterFileLines[i] + '\n';
        //    }
        //    ExcludeFilesTextBox.Text = filterFileStrs;

        //    string filterFolderStrs = "";
        //    for (int i = 0; i < mFilterFolderLines.Count; i++)
        //    {
        //        filterFolderStrs += mFilterFolderLines[i] + '\n';
        //    }
        //    ExcludeFoldersTextBox.Text = filterFolderStrs;

        //    ShowDialog();
        //}

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            //mFilterFileLines.Clear();
            //for (int line = 0; line < ExcludeFilesTextBox.LineCount; line++)
            //{
            //    mFilterFileLines.Add(ExcludeFilesTextBox.GetLineText(line));
            //}

            //mFilterFolderLines.Clear();
            //for (int line = 0; line < ExcludeFoldersTextBox.LineCount; line++)
            //{
            //    mFilterFolderLines.Add(ExcludeFoldersTextBox.GetLineText(line));
            //}
            mFilesLeadToOther.AddFilters();
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
