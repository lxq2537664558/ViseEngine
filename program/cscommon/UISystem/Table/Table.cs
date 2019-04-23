using System.Collections.Generic;
using UISystem.Container.Grid;

namespace UISystem
{
    [CSUtility.Editor.UIEditor_ControlTemplateAbleAttribute("Table")]
    //[CSUtility.Editor.UIEditor_Control("Table")]
    public class Table : Content.ItemsControl
    {
        public Table()
        {
            TGrid = FindControl("TableGrid") as Grid;
            if(TGrid!=null)
                TGrid.GridColumn = 1;

            //List<List<RichTextBox>> dataSrc = new List<List<RichTextBox>>();
            //for(int j = 0; j < 5; ++j)
            //{
            //    List<RichTextBox> row = new List<RichTextBox>();
            //    for(int i = 0; i < 5; ++i)
            //    {
            //        RichTextBox rich = new RichTextBox();
            //        rich.Doc.Text = "<D><F Type='Text' Content='test'></F></D>";
            //        row.Add(rich);
            //    }
            //    dataSrc.Add(row);
            //}

            //TextTable = dataSrc;
        }

        static int mMaxCacheSize = 256;
        RichTextBox[] mRichTextBoxCache = new RichTextBox[mMaxCacheSize];
        int mCurrCacheIndex = 0;
        RichTextBox GetCacheRichTextBox()
        {
            if (mCurrCacheIndex == mMaxCacheSize)
                return null;
            var rich = mRichTextBoxCache[mCurrCacheIndex];
            if (rich == null)
                rich = mRichTextBoxCache[mCurrCacheIndex] = new RichTextBox();
            mCurrCacheIndex++;
            return rich;
        }
        void ClearCache()
        {
            mCurrCacheIndex = 0;
        }
        List<List<RichTextBox>> mRichTextBoxs = new List<List<RichTextBox>>();

        List<List<string>> mTextTable = new List<List<string>>();
        [CSUtility.Editor.UIEditor_BindingProperty]
        public List<List<string>> TextTable
        {
            get { return mTextTable; }
            set
            {
                var parent = GetRoot(typeof(WinForm));
                if (mTextTable == value )
                    return;
                mTextTable.Clear();
                mTextTable = value;

                if (mTextTable == null || mTextTable.Count == 0)
                    return;

                TGrid = FindControl("TableGrid") as Grid;
                if (TGrid == null)
                    return;
                TGrid.ClearChildWindows();
                TGrid.RowDefinitions.Clear();
                TGrid.ColumnDefinitions.Clear();

                int iTemp = GetRoot().GetAllChildControls(false).Count;

                int rowContent = mTextTable.Count;
                int colContent = 0;
                if(rowContent>0)
                    colContent = mTextTable[0].Count;

                //int iRow = rowContent * 2 - 1;
                //int iCol = colContent * 2 - 1;
                int iRow = rowContent;
                int iCol = colContent;

                bool bContent = true;

                for (int i = 0; i < iRow; ++i)
                {
                    UISystem.Container.Grid.RowDefinition obj = new UISystem.Container.Grid.RowDefinition(TGrid);
                    if (bContent)
                    {
                        //obj.HeightProperty.GridUnitType = GridUnitType.Star;
                        obj.HeightProperty.GridUnitType = GridUnitType.Auto;
                        TGrid.RowDefinitions.Add(new UISystem.Container.Grid.RowDefinition(TGrid));
                        bContent = false;
                    }
                    else
                    {
                        obj.HeightProperty.GridUnitType = GridUnitType.Auto;
                        TGrid.RowDefinitions.Add(new UISystem.Container.Grid.RowDefinition(TGrid));
                        bContent = true;
                    }
                }
                TGrid.RowDefinitions = new CSUtility.Support.ThreadSafeObservableCollection<UISystem.Container.Grid.DefinitionBase>(TGrid.RowDefinitions);

                for (int i = 0; i < iCol; ++i)
                {
                    UISystem.Container.Grid.ColumnDefinition obj = new UISystem.Container.Grid.ColumnDefinition(TGrid);
                    if (bContent)
                    {
                        obj.WidthProperty.GridUnitType = GridUnitType.Auto;
                        TGrid.ColumnDefinitions.Add(new UISystem.Container.Grid.ColumnDefinition(TGrid));
                        bContent = false;
                    }
                    else
                    {
                        obj.WidthProperty.GridUnitType = GridUnitType.Auto;
                        TGrid.ColumnDefinitions.Add(new UISystem.Container.Grid.ColumnDefinition(TGrid));
                        bContent = true;
                    }
                }
                TGrid.ColumnDefinitions = new CSUtility.Support.ThreadSafeObservableCollection<UISystem.Container.Grid.DefinitionBase>(TGrid.ColumnDefinitions);

                mRichTextBoxs.Clear();
                //for (int i = 0; i < iCol; ++i)
                //{
                //    if (i % 2 != 0)
                //    {
                //        GridSplitter splitter = new GridSplitter();
                //        splitter.GridColumn = (ushort)i;
                //        splitter.GridColumnSpan = 1;
                //        splitter.GridRow = 0;
                //        splitter.GridRowSpan = (ushort)iRow;
                //        splitter.BackColor = CSUtility.Support.Color.AliceBlue;
                //        splitter.Width = 5;
                //        splitter.Height_Auto = true;
                //        splitter.VerticalAlignment = UI.VerticalAlignment.Stretch;
                //        splitter.HorizontalAlignment = UI.HorizontalAlignment.Stretch;
                //        splitter.Parent = TGrid;
                //    }
                //}
                ClearCache();
                for (int i = 0; i < iCol; ++i)
                {
                    //if (i % 2 == 0)
                    //{
                        List<RichTextBox> richTextBoxRow = new List<RichTextBox>();

                        ushort iTextRow = 0;
                        foreach (var textRow in mTextTable)
                        {
                            //TextBlock richText = new TextBlock();
                            //richText.Text = textRow[i];
                            RichTextBox richText = GetCacheRichTextBox();
                            if (richText == null)
                                break;
                            richText.Doc.Text = textRow[i];
                            richText.GridColumn = (ushort)i;
                            richText.GridColumnSpan = 1;
                            richText.GridRow = iTextRow;
                            richText.GridRowSpan = (ushort)1;
                            richText.BackColor = CSUtility.Support.Color.Black;
                            richText.Width = 200;
                            richText.Height_Auto = true;
                            richText.VerticalAlignment = UI.VerticalAlignment.Stretch;
                            richText.HorizontalAlignment = UI.HorizontalAlignment.Stretch;
                            richText.Parent = TGrid;

                            richTextBoxRow.Add(richText);
                            iTextRow++;
                        }
                        mRichTextBoxs.Add(richTextBoxRow);
                    //}
                }

                BindEvent();

                OnPropertyChanged("TextTable");
            }
        }

        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event RichTextBox.FWinRichTextBoxClick Column1stClick;
        [CSUtility.Editor.UIEditor_BindingEventAttribute]
        public event RichTextBox.FWinRichTextBoxRClick Column1stRClick;

        public void BindEvent()
        {
            if (mRichTextBoxs.Count >= 1)
            {
                foreach (var richText in mRichTextBoxs[0])
                {
                    richText.WinRichTextBoxClick += Column1stClick;
                    richText.WinRichTextBoxRClick += Column1stRClick;
                }
            }
        }
        
        Grid mGrid;
        public Grid TGrid
        {
            get { return mGrid; }
            set
            {
                mGrid = value;
            }
        }

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            //if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "HorizontalScrollBarVisibility"))
            //    pXml.AddAttrib("HorizontalScrollBarVisibility", HorizontalScrollBarVisibility.ToString());
            //if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "VerticalScrollBarVisibility"))
            //    pXml.AddAttrib("VerticalScrollBarVisibility", VerticalScrollBarVisibility.ToString());
        }

        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
            base.OnLoad(pXml);

            //var attr = pXml.FindAttrib("HorizontalScrollBarVisibility");
            //if (attr != null)
            //{
            //    HorizontalScrollBarVisibility = (ScrollBarVisibility)System.Enum.Parse(typeof(ScrollBarVisibility), attr.Value);
            //}
            //attr = pXml.FindAttrib("VerticalScrollBarVisibility");
            //if (attr != null)
            //{
            //    VerticalScrollBarVisibility = (ScrollBarVisibility)System.Enum.Parse(typeof(ScrollBarVisibility), attr.Value);
            //}
        }
    }
}
