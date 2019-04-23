using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Threading;

namespace Viewer
{
    public class XNDAttribInfo : INotifyPropertyChanged
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

        CSUtility.Support.XndAttrib mAttrib;
        public CSUtility.Support.XndAttrib Attrib
        {
            get { return mAttrib; }
        }

        XNDAttribInfo mOppositeXNDAttrib = null;
        public XNDAttribInfo OppositeXNDAttrib
        {
            get { return mOppositeXNDAttrib; }
            set { mOppositeXNDAttrib = value; }
        }

        string mAttName;
        public string AttName
        {
            get { return mAttName; }
            set
            {
                mAttName = value;
                OnPropertyChanged("AttName");
            }
        }

        byte mAttVersion;
        public byte AttVersion
        {
            get { return mAttVersion; }
            set
            {
                mAttVersion = value;
                OnPropertyChanged("AttVersion");
            }
        }

        string mInfo;
        public string Info
        {
            get { return mInfo; }
            set
            {
                mInfo = value;
                OnPropertyChanged("Info");
            }
        }

        bool mIsVisible = true;
        public bool IsVisible
        {
            get { return mIsVisible; }
        }

        Visibility mAttribVisible = Visibility.Visible;
        public Visibility AttribVisible
        {
            get { return mAttribVisible; }
            set
            {
                mAttribVisible = value;
                OnPropertyChanged("AttribVisible");
            }
        }

        public XNDAttribInfo(CSUtility.Support.XndAttrib att, bool isVisible)
        {
            mAttrib = att;
            AttName = att.GetName();
            AttVersion = att.Version;
            Info = "Length: " + att.Length + "b";
            mIsVisible = isVisible;
            if (mIsVisible)
                AttribVisible = Visibility.Visible;
            else
                AttribVisible = Visibility.Hidden;
        }

        public XNDAttribInfo CopyAttrib()
        {
            XNDAttribInfo attribInfo = null;
            if (IsVisible)
            {
                attribInfo = new XNDAttribInfo(mAttrib, IsVisible);
            }
            return attribInfo;
        }

        public void ChangeAttribVisible(bool isVisible)
        {
            if (isVisible)
                AttribVisible = Visibility.Visible;
            else
                AttribVisible = Visibility.Hidden;

            mIsVisible = isVisible;
        }
    }

    public class XNDNodeInfo : INotifyPropertyChanged
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

        CSUtility.Support.XndNode mNode;
        public CSUtility.Support.XndNode Node
        {
            get { return mNode; }
        }

        XNDNodeInfo mOppositeXNDNode = null;
        public XNDNodeInfo OppositeXNDNode
        {
            get { return mOppositeXNDNode; }
            set { mOppositeXNDNode = value; }
        }

        XNDNodeInfo mParentXNDNode = null;
        public XNDNodeInfo ParentXNDNode
        {
            get { return mParentXNDNode; }
        }

        System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> mChildren = new System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo>();
        public System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> Children
        {
            get { return mChildren; }
            set
            {
                mChildren = value;
                OnPropertyChanged("Children");
            }
        }

        System.Collections.ObjectModel.ObservableCollection<XNDAttribInfo> mAttribs = new System.Collections.ObjectModel.ObservableCollection<XNDAttribInfo>();
        public System.Collections.ObjectModel.ObservableCollection<XNDAttribInfo> Attribs
        {
            get { return mAttribs; }
            set
            {
                mAttribs = value;
                OnPropertyChanged("Attribs");
            }
        }

        string mNodeName;
        public string NodeName
        {
            get { return mNodeName; }
            set
            {
                mNodeName = value;
                OnPropertyChanged("NodeName");
            }
        }

        string mInfo;
        public string Info
        {
            get { return mInfo; }
            set
            {
                mInfo = value;
                OnPropertyChanged("Info");
            }
        }

        Visibility mNodeVisible = Visibility.Visible;
        public Visibility NodeVisible
        {
            get { return mNodeVisible; }
            set
            {
                mNodeVisible = value;
                OnPropertyChanged("NodeVisible");
            }
        }

        bool mIsVisible = true;
        public bool IsVisible
        {
            get { return mIsVisible; }
        }

        XNDFileViewer mXndFile = null;

        public XNDNodeInfo(XNDFileViewer XNDFile, CSUtility.Support.XndNode node, bool isVisible, 
            System.Windows.Threading.Dispatcher dispatcher, XNDNodeInfo Parent)
        {
            mNode = node;
            NodeName = mNode.GetName();
            Info = mNode.GetNodes().Count + "个节点，" + mNode.GetAttribs().Count + "个Attrib";

            mParentXNDNode = Parent;
            mXndFile = XNDFile;

            mIsVisible = isVisible;
            if (mIsVisible)
                NodeVisible = Visibility.Visible;
            else
                NodeVisible = Visibility.Hidden;
            if (dispatcher != null)
            {
                dispatcher.Invoke(() =>
                {
                    XNDFile.UpdateProcessingInfo(NodeName);
                    XNDFile.UpdateProcessPercent();
                });
            }

            foreach (var cn in mNode.GetNodes())
            {
                var cnInfo = new XNDNodeInfo(XNDFile, cn, isVisible, dispatcher, this);
                Children.Add(cnInfo);
            }

            foreach (var ca in mNode.GetAttribs())
            {
                var caInfo = new XNDAttribInfo(ca, isVisible);
                Attribs.Add(caInfo);
            }
        }

        public void ClearNoUseNode()
        {
            ClearNoUseAttrib();
            for (int i = 0; i < Children.Count; i++)
            {
                if (!Children[i].IsVisible)
                {
                    Children.Remove(Children[i]);
                    i--;
                    continue;
                }
                Children[i].ClearNoUseNode();
            }
        }
        
        void ClearNoUseAttrib()
        {
            for (int i = 0; i < Attribs.Count; i++)
            {
                if (!Attribs[i].IsVisible)
                {
                    Attribs.Remove(Attribs[i]);
                    i--;
                }
            }
        }

        public XNDNodeInfo CopyNode(XNDFileViewer XNDFile)
        {
            XNDNodeInfo newNode = null;
            if (mIsVisible)
            {
                newNode = new XNDNodeInfo(XNDFile, mNode, mIsVisible, null, null);
            }
            return newNode;
        }

        public void ChangeNodeVisible(bool isVisible, bool isLeft)
        {
            if (mIsVisible == isVisible)
                return;
            foreach (var attrib in Attribs)
            {
                attrib.ChangeAttribVisible(isVisible);
            }
            if (isVisible)
            {
                NodeVisible = Visibility.Visible;
                if (mParentXNDNode != null && !mParentXNDNode.IsVisible)
                {
                    mParentXNDNode.ChangeNodeVisible(isVisible, isLeft);
                }
            }
            else
            {
                NodeVisible = Visibility.Hidden;
            }

            ChangeNodeColorBule(isLeft);
            OppositeXNDNode.ChangeNodeColorBule(!isLeft);

            mIsVisible = isVisible;
        }

        void ChangeNodeColorBule(bool isLeft)
        {
            System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> nodeInfos = null;
            System.Windows.Controls.TreeView treeView = null;
            if (isLeft)
            {
                nodeInfos = mXndFile.LeftRootNodes;
                treeView = mXndFile.TreeView_InfosLeft;
            }
            else
            {
                nodeInfos = mXndFile.RightRootNodes;
                treeView = mXndFile.TreeView_InfosRight;
            }
            var itemView = mXndFile.GetTreeViewItem(nodeInfos, mXndFile.GetStackPanel(treeView), this);
            if (itemView == null)
                return;
            var grid = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                    VisualTreeHelper.GetChild(itemView, 0), 1), 0), 0), 0) as Grid;
            if (grid != null)
            {
                grid.Background = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                var boxGrid = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                    VisualTreeHelper.GetChild(grid, 3), 0), 0), 0) as Grid;
                if (boxGrid != null)
                    boxGrid.Background = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            }
        }

        public void SetOppositeNode(XNDNodeInfo OppositeNode)
        {
            mOppositeXNDNode = OppositeNode;
            if (Children.Count != OppositeNode.Children.Count)
                return;
            for (UInt16 i = 0; i < Children.Count; i++)
            {
                Children[i].SetOppositeNode(OppositeNode.Children[i]);
            }
        }
    }

    public class XNDFileNodeCompare
    {
        XNDFileViewer mXNDFile = null;

        VirtualizingStackPanel mLeftStackPanel = null;
        VirtualizingStackPanel mRightStackPanel = null;

        public System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> mLeftRootNodes = new System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo>();
        public System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> mRightRootNodes = new System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo>();

        int mNodeIndex = 0;
        int mAttribIndex = 0;

        public XNDFileNodeCompare(XNDFileViewer XNDFile, VirtualizingStackPanel leftPanel, VirtualizingStackPanel rightPanel, 
            System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> leftNode,System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> rightNode)
        {
            mXNDFile = XNDFile;
            mLeftStackPanel = leftPanel;
            mRightStackPanel = rightPanel;
            mLeftRootNodes = leftNode;
            mRightRootNodes = rightNode;
        }

        public void CompareNode()
        {
            if (mLeftRootNodes.Count == 0 || mRightRootNodes.Count == 0)
                return;
            for (int i = 0; i < mLeftRootNodes.Count; i++)
            {
                if (mNodeIndex >= mRightRootNodes.Count)
                {
                    ChangeColorLeftNode(mLeftRootNodes.Count);
                    ResultRightNode(mLeftRootNodes.Count, mRightRootNodes.Count);
                    return;
                }
                for (int j = mNodeIndex; j < mRightRootNodes.Count; j++)
                {
                    if (mLeftRootNodes[i].NodeName == mRightRootNodes[j].NodeName)
                    {
                        mLeftRootNodes[i].OppositeXNDNode = mRightRootNodes[j];
                        mRightRootNodes[j].OppositeXNDNode = mLeftRootNodes[i];

                        System.Collections.ObjectModel.ObservableCollection<TreeViewItem> treeViewers = new System.Collections.ObjectModel.ObservableCollection<TreeViewItem>();
                        
                        i += ResultLeftNode(i, j);
                        j += ResultRightNode(i, j);
                        ChangeColorRightNode(j);
                        ChangeColorLeftNode(i);

                        CompareAttrib(i, j);

                        XNDFileNodeCompare compare = new XNDFileNodeCompare(mXNDFile, GetLeftStackPanel(i), GetRightStackPanel(j), mLeftRootNodes[i].Children, mRightRootNodes[j].Children);
                        compare.CompareNode();
                        mNodeIndex = j + 1;
                        break;
                    }
                }
            }
            if (mNodeIndex < mRightRootNodes.Count)
            {
                ResultLeftNode(mNodeIndex, mRightRootNodes.Count);
                ChangeColorLeftNode(mLeftRootNodes.Count);

                ResultRightNode(mLeftRootNodes.Count, mRightRootNodes.Count);
                ChangeColorRightNode(mRightRootNodes.Count);
            }
        }

        int ResultLeftNode(int indexL, int indexR)
        {
            int addCount = 0;
            for (int i = indexR - 1; i >= mNodeIndex; i--)
            {
                if (!mRightRootNodes[i].IsVisible)
                    continue;
                XNDNodeInfo OppositeNode = null;
                if (mRightRootNodes[i].ParentXNDNode != null)
                    OppositeNode = mRightRootNodes[i].ParentXNDNode.OppositeXNDNode;
                var nodeInfo = new XNDNodeInfo(mXNDFile, mRightRootNodes[i].Node, false, null, OppositeNode);
                nodeInfo.SetOppositeNode(mRightRootNodes[i]);
                mRightRootNodes[i].SetOppositeNode(nodeInfo);
//                 nodeInfo.OppositeXNDNode = mRightRootNodes[i];
//                 mRightRootNodes[i].OppositeXNDNode = nodeInfo;
                mLeftRootNodes.Insert(indexL, nodeInfo);
                addCount++;
            }
            if (addCount > 0)
            {
                mXNDFile.DifferenceNodes.Add(mLeftRootNodes[mNodeIndex]);
            }
            mLeftStackPanel.UpdateLayout();
            return addCount;
        }

        void ChangeColorLeftNode(int index)
        {
            for (int i = mNodeIndex; i < index; i++)
            {
                if (!mLeftRootNodes[i].IsVisible)
                    continue;
                var border = GetBorderWithLeftTreeView(i);
                if (border == null)
                    return;

                var grid = VisualTreeHelper.GetChild(border, 0) as Grid;
                if (grid != null)
                {
                    grid.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    var boxGrid = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                        VisualTreeHelper.GetChild(grid, 3), 0), 0), 0) as Grid;
                    if (boxGrid != null)
                        boxGrid.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                }
                XNDFileNodeCompare compare = new XNDFileNodeCompare(mXNDFile, GetLeftStackPanel(i), null, mLeftRootNodes[i].Children, null);
                compare.ChangeColorLeftNode(compare.mLeftRootNodes.Count);
            }
            if (mNodeIndex < index)
            {
                mXNDFile.DifferenceNodes.Add(mLeftRootNodes[mNodeIndex]);
            }
        }

        void CompareAttrib(int indexL, int indexR)
        {
            for (int i = 0; i < mLeftRootNodes[indexL].Attribs.Count; i++)
            {
                if (mAttribIndex >= mRightRootNodes[indexR].Attribs.Count)
                {
                    ResultRightAttrib(indexL, indexR, mLeftRootNodes[indexL].Attribs.Count, mRightRootNodes[indexR].Attribs.Count);
                    ChangeColorLeftAttrib(indexL, i + mAttribIndex);
                    return;
                }
                for (int j = mAttribIndex; j < mRightRootNodes[indexR].Attribs.Count; j++)
                {
                    if (mLeftRootNodes[indexL].Attribs[i].AttName == mRightRootNodes[indexR].Attribs[j].AttName &&
                        mLeftRootNodes[indexL].Attribs[i].AttVersion == mRightRootNodes[indexR].Attribs[j].AttVersion &&
                        mLeftRootNodes[indexL].Attribs[i].Attrib.Length == mRightRootNodes[indexR].Attribs[j].Attrib.Length)
                    {
                        mLeftRootNodes[indexL].Attribs[i].OppositeXNDAttrib = mRightRootNodes[indexR].Attribs[j];
                        mRightRootNodes[indexR].Attribs[j].OppositeXNDAttrib = mLeftRootNodes[indexL].Attribs[i];
                        i += ResultLeftAttrib(indexL, indexR, i, j);
                        j += ResultRightAttrib(indexL, indexR, i, j);
                        ChangeColorRightAttrib(indexR, j);
                        ChangeColorLeftAttrib(indexL, i);
                        mAttribIndex = j + 1;
                        break;
                    }
                }
            }
            if (mAttribIndex < mRightRootNodes[indexR].Attribs.Count)
            {
                ResultLeftAttrib(indexL, indexR, mAttribIndex, mRightRootNodes[indexR].Attribs.Count);
                ResultRightAttrib(indexL, indexR, mLeftRootNodes[indexL].Attribs.Count, mRightRootNodes[indexR].Attribs.Count);
                ChangeColorLeftAttrib(indexL, mLeftRootNodes[indexL].Attribs.Count);
                ChangeColorRightAttrib(indexL, mRightRootNodes[indexL].Attribs.Count);
            }
        }

        int ResultLeftAttrib(int indexLNode, int indexRNode, int indexLAttrib, int indexRAttrib)
        {
            int addCount = 0;
            for (int i = indexRAttrib - 1; i >= mAttribIndex; i--)
            {
                if (!mRightRootNodes[indexRNode].Attribs[i].IsVisible)
                    continue;
                var attribInfo = new XNDAttribInfo(mRightRootNodes[indexRNode].Attribs[i].Attrib, false);
                attribInfo.OppositeXNDAttrib = mRightRootNodes[indexRNode].Attribs[i];
                mRightRootNodes[indexRNode].Attribs[i].OppositeXNDAttrib = attribInfo;
                mLeftRootNodes[indexLNode].Attribs.Insert(indexLAttrib, attribInfo);
                addCount++;
            }
            if (addCount > 0)
            {
                mXNDFile.DifferenceNodes.Add(mLeftRootNodes[indexLNode]);
            }
            mLeftStackPanel.UpdateLayout();
            return addCount;
        }

        void ChangeColorLeftAttrib(int indexLNode, int indexLAttrib)
        {
            var border = GetBorderWithLeftTreeView(indexLNode);
            if (border == null)
                return;

            var grid = VisualTreeHelper.GetChild(border, 0) as Grid;
            if (grid == null)
                return;
            var panel = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(grid, 3), 0), 0), 0), 1), 0), 0) as VirtualizingStackPanel;
            if (panel == null)
                return;
            for (int i = mAttribIndex; i < indexLAttrib; i++)
            {
                if (!mLeftRootNodes[indexLNode].Attribs[i].IsVisible)
                    continue;
                var listBoxItemGrid = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                    VisualTreeHelper.GetChild(panel, i), 0), 0), 0) as Grid;
                if (listBoxItemGrid != null)
                    listBoxItemGrid.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            if (mAttribIndex < indexLAttrib)
            {
                mXNDFile.DifferenceNodes.Add(mLeftRootNodes[indexLNode]);
            }
        }

        int ResultRightNode(int indexL, int indexR)
        {
            int addCount = 0;
            for (int i = indexL - 1; i >= mNodeIndex; i--)
            {
                if (!mLeftRootNodes[i].IsVisible)
                    continue;
                XNDNodeInfo OppositeNode = null;
                if (mLeftRootNodes[i].ParentXNDNode != null)
                    OppositeNode = mLeftRootNodes[i].ParentXNDNode.OppositeXNDNode;
                var nodeInfo = new XNDNodeInfo(mXNDFile, mLeftRootNodes[i].Node, false, null, OppositeNode);
                nodeInfo.SetOppositeNode(mLeftRootNodes[i]);
                mLeftRootNodes[i].SetOppositeNode(nodeInfo);
//                 nodeInfo.OppositeXNDNode = mLeftRootNodes[i];
//                 mLeftRootNodes[i].OppositeXNDNode = nodeInfo;
                mRightRootNodes.Insert(indexR, nodeInfo);
                addCount++;
            }
            mRightStackPanel.UpdateLayout();
            return addCount;
        }

        void ChangeColorRightNode(int index)
        {
            for (int i = mNodeIndex; i < index; i++)
            {
                if (!mRightRootNodes[i].IsVisible)
                    continue;
                var border = GetBorderWithRightTreeView(i);
                if (border == null)
                    return;

                var grid = VisualTreeHelper.GetChild(border, 0) as Grid;
                if (grid != null)
                {
                    grid.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    var boxGrid = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                        VisualTreeHelper.GetChild(grid, 3), 0), 0), 0) as Grid;
                    if (boxGrid != null)
                        boxGrid.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                }
                XNDFileNodeCompare compare = new XNDFileNodeCompare(mXNDFile, null, GetRightStackPanel(i), null, mRightRootNodes[i].Children);
                compare.ChangeColorRightNode(compare.mRightRootNodes.Count);
            }
        }

        int ResultRightAttrib(int indexLNode, int indexRNode, int indexLAttrib, int indexRAttrib)
        {
            int addCount = 0;
            for (int i = indexLAttrib - 1; i >= mAttribIndex; i--)
            {
                if (!mLeftRootNodes[indexLNode].Attribs[i].IsVisible)
                    continue;
                var attribInfo = new XNDAttribInfo(mLeftRootNodes[indexLNode].Attribs[i].Attrib, false);
                attribInfo.OppositeXNDAttrib = mLeftRootNodes[indexLNode].Attribs[i];
                mLeftRootNodes[indexLNode].Attribs[i].OppositeXNDAttrib = attribInfo;
                mRightRootNodes[indexRNode].Attribs.Insert(indexRAttrib, attribInfo);
                addCount++;
            }
            mRightStackPanel.UpdateLayout();
            return addCount;
        }

        void ChangeColorRightAttrib(int indexRNode, int indexRAttrib)
        {
            var border = GetBorderWithRightTreeView(indexRNode);
            if (border == null)
                return;

            var grid = VisualTreeHelper.GetChild(border, 0) as Grid;
            if (grid == null)
                return;
            var panel = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(grid, 3), 0), 0), 0), 1), 0), 0) as VirtualizingStackPanel;
            if (panel == null)
                return;
            for (int i = mAttribIndex; i < indexRAttrib; i++)
            {
                if (!mRightRootNodes[indexRNode].Attribs[i].IsVisible)
                    continue;
                var listBoxItemGrid = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                    VisualTreeHelper.GetChild(panel, i), 0), 0), 0) as Grid;
                if (listBoxItemGrid != null)
                    listBoxItemGrid.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }

        //TreeViewItem GetLeftTreeViewItem(int index)
        //{
        //    var child = VisualTreeHelper.GetChild(mLeftStackPanel, index) as TreeViewItem;
        //    return child;
        //}

        //TreeViewItem GetRightTreeViewItem(int index)
        //{
        //    var child = VisualTreeHelper.GetChild(mRightStackPanel, index) as TreeViewItem;
        //    return child;
        //}

        VirtualizingStackPanel GetLeftStackPanel(int index)
        {
            if (VisualTreeHelper.GetChildrenCount(mLeftStackPanel) > index)
            {
                var child = VisualTreeHelper.GetChild(mLeftStackPanel, index);
                if (VisualTreeHelper.GetChildrenCount(child) > 0)
                {
                    child = VisualTreeHelper.GetChild(child, 0);
                    if (VisualTreeHelper.GetChildrenCount(child) > 2)
                    {
                        child = VisualTreeHelper.GetChild(child, 2);
                        if (VisualTreeHelper.GetChildrenCount(child) > 0)
                        {
                            var panel = VisualTreeHelper.GetChild(child, 0) as VirtualizingStackPanel;
                            return panel;
                        }
                    }
                }
            }
//             var child = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
//                 VisualTreeHelper.GetChild(mLeftStackPanel, index), 0), 2), 0) as VirtualizingStackPanel;
            return null;
        }
        VirtualizingStackPanel GetRightStackPanel(int index)
        {
            if (VisualTreeHelper.GetChildrenCount(mRightStackPanel) > index)
            {
                var child = VisualTreeHelper.GetChild(mRightStackPanel, index);
                if (VisualTreeHelper.GetChildrenCount(child) > 0)
                {
                    child = VisualTreeHelper.GetChild(child, 0);
                    if (VisualTreeHelper.GetChildrenCount(child) > 2)
                    {
                        child = VisualTreeHelper.GetChild(child, 2);
                        if (VisualTreeHelper.GetChildrenCount(child) > 0)
                        {
                            var panel = VisualTreeHelper.GetChild(child, 0) as VirtualizingStackPanel;
                            return panel;
                        }
                    }
                }
            }
//             var child = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
//                 VisualTreeHelper.GetChild(mRightStackPanel, index), 0), 2), 0) as VirtualizingStackPanel;
            return null;
        }

        Border GetBorderWithLeftTreeView(int index)
        {
            Border border = null;
            if (VisualTreeHelper.GetChildrenCount(mLeftStackPanel) > index)
            {
                var child = VisualTreeHelper.GetChild(mLeftStackPanel, index);
                if (VisualTreeHelper.GetChildrenCount(child) > 0)
                {
                    child = VisualTreeHelper.GetChild(child, 0);
                    if (VisualTreeHelper.GetChildrenCount(child) > 1)
                    {
                        child = VisualTreeHelper.GetChild(child, 1);
                        if (VisualTreeHelper.GetChildrenCount(child) > 0)
                        {
                            child = VisualTreeHelper.GetChild(child, 0);
                            if (VisualTreeHelper.GetChildrenCount(child) > 0)
                            {
                                border = VisualTreeHelper.GetChild(child, 0) as Border;
                            }
                        }
                    }
                }
//                 border = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
//                     VisualTreeHelper.GetChild(mLeftStackPanel, index), 0), 1), 0), 0) as Border;
            }
            return border;
        }

        Border GetBorderWithRightTreeView(int index)
        {
            Border border = null;
            if (VisualTreeHelper.GetChildrenCount(mRightStackPanel) > index)
            {
                var child = VisualTreeHelper.GetChild(mRightStackPanel, index);
                if (VisualTreeHelper.GetChildrenCount(child) > 0)
                {
                    child = VisualTreeHelper.GetChild(child, 0);
                    if (VisualTreeHelper.GetChildrenCount(child) > 1)
                    {
                        child = VisualTreeHelper.GetChild(child, 1);
                        if (VisualTreeHelper.GetChildrenCount(child) > 0)
                        {
                            child = VisualTreeHelper.GetChild(child, 0);
                            if (VisualTreeHelper.GetChildrenCount(child) > 0)
                            {
                                border = VisualTreeHelper.GetChild(child, 0) as Border;
                            }
                        }
                    }
                }
//                 border = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
//                     VisualTreeHelper.GetChild(mRightStackPanel, index), 0), 1), 0), 0) as Border;
            }
            return border;
        }


        public void ClearLeftNodeColor()
        {
            for (int i = 0; i < mLeftRootNodes.Count; i++)
            {
                var border = GetBorderWithLeftTreeView(i);
                if (border == null)
                    return;

                var grid = VisualTreeHelper.GetChild(border, 0) as Grid;
                if (grid != null)
                {
                    if (grid.Background != null)
                    {
                        grid.Background = null;
                        var boxGrid = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                            VisualTreeHelper.GetChild(grid, 3), 0), 0), 0) as Grid;
                        if (boxGrid != null && boxGrid.Background != null)
                            boxGrid.Background = null;
                    }
                    var panel = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                        VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(grid, 3), 0), 0), 0), 1), 0), 0) as VirtualizingStackPanel;
                    if (panel != null)
                    {
                        for (int j = 0; j < mLeftRootNodes[i].Attribs.Count; j++)
                        {
                            var listBoxItemGrid = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                                VisualTreeHelper.GetChild(panel, j), 0), 0), 0) as Grid;
                            if (listBoxItemGrid != null && listBoxItemGrid.Background != null)
                                listBoxItemGrid.Background = null;
                        }
                    }
                }
                XNDFileNodeCompare compare = new XNDFileNodeCompare(mXNDFile, GetLeftStackPanel(i), null, mLeftRootNodes[i].Children, null);
                compare.ClearLeftNodeColor();
            }
        }

        public void ClearRightNodeColor()
        {
            for (int i = 0; i < mRightRootNodes.Count; i++)
            {
                var border = GetBorderWithRightTreeView(i);
                if (border == null)
                    return;

                var grid = VisualTreeHelper.GetChild(border, 0) as Grid;
                if (grid != null)
                {
                    if (grid.Background != null)
                    {
                        grid.Background = null;
                        var boxGrid = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                            VisualTreeHelper.GetChild(grid, 3), 0), 0), 0) as Grid;
                        if (boxGrid != null && boxGrid.Background != null)
                            boxGrid.Background = null;
                    }
                    var panel = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                        VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(grid, 3), 0), 0), 0), 1), 0), 0) as VirtualizingStackPanel;
                    if (panel != null)
                    {
                        for (int j = 0; j < mRightRootNodes[i].Attribs.Count; j++)
                        {
                            var listBoxItemGrid = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                                VisualTreeHelper.GetChild(panel, j), 0), 0), 0) as Grid;
                            if (listBoxItemGrid != null && listBoxItemGrid.Background != null)
                                listBoxItemGrid.Background = null;
                        }
                    }
                }
                XNDFileNodeCompare compare = new XNDFileNodeCompare(mXNDFile, null, GetRightStackPanel(i), null, mRightRootNodes[i].Children);
                compare.ClearRightNodeColor();
            }
        }
    }

    /// <summary>
    /// Interaction logic for XNDFileViewer.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "XNDFileViewer")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/XND文件查看器")]
    [Guid("31D5417A-D6E6-4429-8DD2-667FA32DD82E")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class XNDFileViewer : System.Windows.Controls.UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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

        public string PluginName
        {
            get { return "XND文件查看器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }
  
        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "XND文件查看器",
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public string AssemblyPath
        {
            get { return this.GetType().Assembly.Location; }
        }
        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void SetObjectToEdit(object[] obj)
        {

        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        public void Tick()
        {
            
        }

        ///////////////////////////////////////////////////////////

        string mFileNameLeft = "";
        public string FileNameLeft
        {
            get { return mFileNameLeft; }
            set
            {
                mFileNameLeft = value;
                OnPropertyChanged("FileNameLeft");
            }
        }

        string mFileNameRight = "";
        public string FileNameRight
        {
            get { return mFileNameRight; }
            set
            {
                mFileNameRight = value;
                OnPropertyChanged("FileNameRight");
            }
        }

        ScrollViewer mViewerLeft = null;
        ScrollViewer mViewerRight = null;        

        System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> mLeftRootNodes = new System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo>();
        public System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> LeftRootNodes
        {
            get { return mLeftRootNodes; }
            set { mLeftRootNodes = value; }
        }

        System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> mRightRootNodes = new System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo>();
        public System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> RightRootNodes
        {
            get { return mRightRootNodes; }
            set { mRightRootNodes = value; }
        }

        int mAllNodeCount = 0;

        int mCurNodeIndex = 0;
        public int CurNodeIndex
        {
            get { return mCurNodeIndex; }
            set { mCurNodeIndex = value; }
        }

        System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> mDifferenceNodes = new System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo>();
        public System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> DifferenceNodes
        {
            get { return mDifferenceNodes; }
        }

        int mDifferenceNodeIndex = 0;

        string mLeftFilter = string.Empty;
        string mRightFilter = string.Empty;

        static System.Windows.Threading.DispatcherTimer mAnimaTimer = new System.Windows.Threading.DispatcherTimer();

        public XNDFileViewer()
        {
            InitializeComponent();

            TreeView_InfosLeft.ItemsSource = LeftRootNodes;
            TreeView_InfosRight.ItemsSource = RightRootNodes;
        }

        void InitData()
        {
            mAllNodeCount = 0;
            mCurNodeIndex = 0;
            ProcessPercent = 0;
            DifferenceNodes.Clear();
        }

        void ClearNoUseNode(System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> nodes)
        {
            if (nodes.Count > 0)
            {
                if (!nodes[0].IsVisible)
                {
                    nodes.Clear();
                    return;
                }
                nodes[0].ClearNoUseNode();
                var node = nodes[0];
                nodes.Clear();
                nodes.Add(node);
            }
        }

        XNDNodeInfo LoadXNDFile(string fileName, System.Windows.Threading.Dispatcher dispatcher)
        {
            if (fileName == "")
                return null;

            var holder = CSUtility.Support.XndHolder.LoadXND(fileName);
            if (holder == null)
            {
                EditorCommon.MessageBox.Show(fileName + " 不是合法的XND文件, 无法打开!");
                return null;
            }

            var nodeInfo = new XNDNodeInfo(this, holder.Node, true, dispatcher, null);
            return nodeInfo;
        }

        void UpdateAllNodeCount(string fileName)
        {
            if (fileName == "")
                return;
            var holder = CSUtility.Support.XndHolder.LoadXND(fileName);
            if (holder == null)
                return;
            _UpdateAllNodeCount(holder.Node);
        }

        void _UpdateAllNodeCount(CSUtility.Support.XndNode node)
        {
            foreach (var cn in node.GetNodes())
            {
                mAllNodeCount++;
                _UpdateAllNodeCount(cn);
            }
        }

        private void Button_OpenFileLeft_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mLeftFilter = GetFilterStr(ofd.SafeFileName);
                FileNameLeft = ofd.FileName;
                LeftRootNodes.Clear();
                InitData();
                XNDFileNodeCompare compare = new XNDFileNodeCompare(this, null, GetStackPanel(TreeView_InfosRight), null, RightRootNodes);
                compare.ClearRightNodeColor();
                ClearNoUseNode(RightRootNodes);
                TreeView_InfosRight.UpdateLayout();
                UpdateAllNodeCount(FileNameLeft);
                LoadProcess(true, FileNameLeft);
            }
        }

        private void Button_OpenFileRight_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mRightFilter = GetFilterStr(ofd.SafeFileName);
                FileNameRight = ofd.FileName;
                RightRootNodes.Clear();
                InitData();
                XNDFileNodeCompare compare = new XNDFileNodeCompare(this, GetStackPanel(TreeView_InfosLeft), null, LeftRootNodes, null);
                compare.ClearLeftNodeColor();
                ClearNoUseNode(LeftRootNodes);
                TreeView_InfosLeft.UpdateLayout();
                UpdateAllNodeCount(FileNameRight);
                LoadProcess(false, FileNameRight);
            }
        }

        string GetFilterStr(string safeFileName)
        {
            var str = safeFileName.Split('.');
            return '.' + str[str.Length - 1];
        }

        void LoadProcess(bool isLeft, string fileName)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            StartProcess(() =>
            {
                var node = LoadXNDFile(fileName, this.Dispatcher);
                this.Dispatcher.Invoke(() =>
                {
                    UpdateProcessPercent();
                    if (node == null)
                    {
                        if (isLeft)
                        {
                            var rightPanel = GetStackPanel(TreeView_InfosRight);
                            ExpandAllTreeView(rightPanel);
                        }
                        else
                        {
                            var leftPanel = GetStackPanel(TreeView_InfosLeft);
                            ExpandAllTreeView(leftPanel);
                        }
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                        return;
                    }
                        
                    if (isLeft)
                    {
                        LeftRootNodes.Add(node);
                        TreeView_InfosLeft.UpdateLayout();
                    }
                    else
                    {
                        RightRootNodes.Add(node);
                        TreeView_InfosRight.UpdateLayout();
                    }

                    //TreeView_InfosLeft.UpdateLayout();
                    //TreeView_InfosRight.UpdateLayout();
                    UpdateTreeViewNode();
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                });
            });
        }

        public void UpdateProcessPercent()
        {
            mCurNodeIndex++;
            ProcessPercent = (float)mCurNodeIndex / (float)mAllNodeCount;
        }

        public void UpdateProcessingInfo(string nodeName)
        {
            ProcessingInfo = "正在加载：" + nodeName;
        }

        void UpdateTreeViewNode()
        {
            var leftPanel = GetStackPanel(TreeView_InfosLeft);
            var rightPanel = GetStackPanel(TreeView_InfosRight);
            ExpandAllTreeView(leftPanel);
            ExpandAllTreeView(rightPanel);
            var compare = new XNDFileNodeCompare(this, leftPanel, rightPanel, LeftRootNodes, RightRootNodes);
            compare.CompareNode();

            ExpandAllTreeView(leftPanel);
            ExpandAllTreeView(rightPanel);
            TreeView_Expanded(leftPanel, rightPanel);
        }

        void ExpandAllTreeView(VirtualizingStackPanel panel)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(panel); i++)
            {
                var treeView = VisualTreeHelper.GetChild(panel, i) as TreeViewItem;
                treeView.IsExpanded = true;
                treeView.UpdateLayout();
                var nextPanel = (VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                    VisualTreeHelper.GetChild(treeView, 0), 2), 0)) as VirtualizingStackPanel;
                if (nextPanel != null)
                    ExpandAllTreeView(nextPanel);
            }
        }

        private void TreeView_Expanded(VirtualizingStackPanel leftPanel, VirtualizingStackPanel rightPanel)
        {
            if (VisualTreeHelper.GetChildrenCount(leftPanel) != VisualTreeHelper.GetChildrenCount(rightPanel))
                return;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(leftPanel); i++)
            {
                var leftTreeView = VisualTreeHelper.GetChild(leftPanel, i) as TreeViewItem;
                var rightTreeView = VisualTreeHelper.GetChild(rightPanel, i) as TreeViewItem;
                BindingOperations.ClearBinding(leftTreeView, TreeViewItem.IsExpandedProperty);
                BindingOperations.SetBinding(leftTreeView, TreeViewItem.IsExpandedProperty, new System.Windows.Data.Binding("IsExpanded") { Source = rightTreeView, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                var nextLeftPanel = (VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                    VisualTreeHelper.GetChild(leftTreeView, 0), 2), 0)) as VirtualizingStackPanel;
                var nextRightPanel = (VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                    VisualTreeHelper.GetChild(rightTreeView, 0), 2), 0)) as VirtualizingStackPanel;
                if (nextLeftPanel != null && nextRightPanel != null)
                    TreeView_Expanded(nextLeftPanel, nextRightPanel);
            }
        }

        //TreeViewItem GetTreeViewItem(VirtualizingStackPanel MainPanel, VirtualizingStackPanel SecondPanel, TreeViewItem treeView)
        //{
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(MainPanel); i++)
        //    {
        //        var mainView = VisualTreeHelper.GetChild(MainPanel, i) as TreeViewItem;
        //        if (VisualTreeHelper.GetChildrenCount(SecondPanel) <= i)
        //            return null;
        //        var secondView = VisualTreeHelper.GetChild(SecondPanel, i) as TreeViewItem;
        //        if (mainView == treeView)
        //            return secondView;
        //        var nextView = GetTreeViewItem(GetStackPanel(mainView), GetStackPanel(secondView), treeView);
        //        if (nextView != null)
        //            return nextView;
        //    }
        //    return null;
        //}

        public VirtualizingStackPanel GetStackPanel(System.Windows.Controls.TreeView treeViewer)
        {
            System.Collections.ObjectModel.ObservableCollection<TreeViewItem> treeViewItems = new System.Collections.ObjectModel.ObservableCollection<TreeViewItem>();
            DependencyObject child = treeViewer;
            for (int i = 0; i < 3; i++)
            {
                child = VisualTreeHelper.GetChild(child, 0);
            }
            child = VisualTreeHelper.GetChild(child, 1);
            for (int i = 0; i < 2; i++)
            {
                child = VisualTreeHelper.GetChild(child, 0);
            }
            return (child as VirtualizingStackPanel);
        }

        public VirtualizingStackPanel GetStackPanel(TreeViewItem view)
        {
            var child = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(
                VisualTreeHelper.GetChild(view, 0), 2), 0) as VirtualizingStackPanel;
            return child;
        }

        public TreeViewItem GetTreeViewItem(System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> NodeInfos,
            VirtualizingStackPanel panel, XNDNodeInfo node)
        {
            TreeViewItem treeView = null;
            for (int i = 0; i < NodeInfos.Count; i++)
            {
                treeView = VisualTreeHelper.GetChild(panel, i) as TreeViewItem;
                if (node == NodeInfos[i])
                    break;
                treeView = GetTreeViewItem(NodeInfos[i].Children, GetStackPanel(treeView), node);
                if (treeView != null)
                    break;
            }
            return treeView; 
        }

        private void userControl_Initialized(object sender, EventArgs e)
        {
            // 多线程加载有问题，先注掉有时间再写
            //Thread visualLoad = new Thread(VisualLoadThread);
            //visualLoad.Start();
            mAnimaTimer.IsEnabled = true;
            mAnimaTimer.Interval = TimeSpan.FromMilliseconds(300);
            mAnimaTimer.Tick += VisualLoadThread;
            mAnimaTimer.Start();
        }

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            //             mViewerLeft = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(TreeView_InfosLeft, 0), 0) as ScrollViewer;
            //             mViewerRight = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(TreeView_InfosRight, 0), 0) as ScrollViewer;
            // 
            //             var scrollLeft = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(mViewerLeft, 0), 1) as System.Windows.Controls.Primitives.ScrollBar;
            //             var scrollRight = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(mViewerRight, 0), 1) as System.Windows.Controls.Primitives.ScrollBar;
            // 
            //             scrollLeft.ValueChanged += ScrollLeft_ValueChanged;
            //             scrollRight.ValueChanged += ScrollRight_ValueChanged;
            // 
            //             BindingOperations.SetBinding(scrollLeft, System.Windows.Controls.Primitives.ScrollBar.ValueProperty, new System.Windows.Data.Binding("Value") { Source = scrollRight, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });            
        }        
        void VisualLoadThread(object sender, EventArgs e)
        {
            System.Windows.Controls.Primitives.ScrollBar scrollLeft = null;
            System.Windows.Controls.Primitives.ScrollBar scrollRight = null;

            if (mViewerLeft == null && VisualTreeHelper.GetChildrenCount(TreeView_InfosLeft) > 0)
            {
                var objLeft = VisualTreeHelper.GetChild(TreeView_InfosLeft, 0);
                if (VisualTreeHelper.GetChildrenCount(objLeft) > 0)
                {
                    mViewerLeft = VisualTreeHelper.GetChild(objLeft, 0) as ScrollViewer;
                }
            }
            if (mViewerRight == null && VisualTreeHelper.GetChildrenCount(TreeView_InfosRight) > 0)
            {
                var objRight = VisualTreeHelper.GetChild(TreeView_InfosRight, 0);
                if (VisualTreeHelper.GetChildrenCount(objRight) > 0)
                {
                    mViewerRight = VisualTreeHelper.GetChild(objRight, 0) as ScrollViewer;
                }
            }
            if (mViewerLeft != null && mViewerRight != null)
            {
                if (scrollLeft == null && VisualTreeHelper.GetChildrenCount(mViewerLeft) > 0)
                {
                    var objLeft = VisualTreeHelper.GetChild(mViewerLeft, 0);
                    if (VisualTreeHelper.GetChildrenCount(objLeft) > 0)
                    {
                        scrollLeft = VisualTreeHelper.GetChild(objLeft, 2) as System.Windows.Controls.Primitives.ScrollBar;
                    }
                }

                if (scrollRight == null && VisualTreeHelper.GetChildrenCount(mViewerRight) > 0)
                {
                    var objRight = VisualTreeHelper.GetChild(mViewerRight, 0);
                    if (VisualTreeHelper.GetChildrenCount(objRight) > 0)
                    {
                        scrollRight = VisualTreeHelper.GetChild(objRight, 2) as System.Windows.Controls.Primitives.ScrollBar;
                    }
                }

                if (scrollLeft != null && scrollRight != null)
                {
                    scrollLeft.ValueChanged += ScrollLeft_ValueChanged;
                    scrollRight.ValueChanged += ScrollRight_ValueChanged;

                    mAnimaTimer.IsEnabled = false;
                    //BindingOperations.SetBinding(scrollLeft, System.Windows.Controls.Primitives.ScrollBar.ValueProperty, new System.Windows.Data.Binding("Value") { Source = scrollRight, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                }
            }
        }

        private void ScrollRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var scrollRight = sender as System.Windows.Controls.Primitives.ScrollBar;
            if (scrollRight == null)
                return;
            var scrollLeft = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(mViewerLeft, 0), 2) as System.Windows.Controls.Primitives.ScrollBar;
            if (scrollLeft == null)
                return;
            if (scrollLeft.Value == scrollRight.Value)
                return;
            mViewerLeft.ScrollToVerticalOffset(scrollRight.Value);
        }

        private void ScrollLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var scrollLeft = sender as System.Windows.Controls.Primitives.ScrollBar;
            if (scrollLeft == null)
                return;
            var scrollRight = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(mViewerRight, 0), 2) as System.Windows.Controls.Primitives.ScrollBar;
            if (scrollRight == null)
                return;
            if (scrollLeft.Value == scrollRight.Value)
                return;
            mViewerRight.ScrollToVerticalOffset(scrollLeft.Value);
        }

        #region 长时间处理

        Visibility mProcessingVisible = Visibility.Collapsed;
        public Visibility ProcessingVisible
        {
            get { return mProcessingVisible; }
            set
            {
                mProcessingVisible = value;
                OnPropertyChanged("ProcessingVisible");
            }
        }

        string mProcessingInfo = "";
        public string ProcessingInfo
        {
            get { return mProcessingInfo; }
            set
            {
                mProcessingInfo = value;
                OnPropertyChanged("ProcessingInfo");
            }
        }

        float mProcessPercent = 0;
        public float ProcessPercent
        {
            get { return mProcessPercent; }
            set
            {
                mProcessPercent = value;
                OnPropertyChanged("ProcessPercent");
            }
        }

        System.Threading.Thread mProcessThread;
        Action mProcessAction;
        //Action mProcessFinishAction;
        void StartProcessThread()
        {
            mProcessThread = new System.Threading.Thread(new System.Threading.ThreadStart(DoProcess));
            mProcessThread.Name = "XNDFileViewer长时间处理操作线程";
            mProcessThread.IsBackground = true;
            mProcessThread.Start();
        }

        public void StartProcess(Action doAction)
        {
            ProcessingVisible = Visibility.Visible;
            mProcessAction = doAction;
            if (mProcessThread == null)
                StartProcessThread();
        }

        void DoProcess()
        {
            if (mProcessAction == null)
                return;

            mProcessAction.Invoke();

            ProcessingVisible = Visibility.Collapsed;
            mProcessThread = null;

            //mProcessFinishAction.Invoke();
        }

        #endregion

        private void Button_CopyNodeToLeftClick(object sender, RoutedEventArgs e)
        {
            var nodeInfo = TreeView_InfosRight.SelectedItem as XNDNodeInfo;
            if (nodeInfo != null && nodeInfo.OppositeXNDNode != null)
            {
                nodeInfo.OppositeXNDNode.ChangeNodeVisible(nodeInfo.IsVisible, true);
            }
        }

        private void Button_CopyNodeToRightClick(object sender, RoutedEventArgs e)
        {
            var nodeInfo = TreeView_InfosLeft.SelectedItem as XNDNodeInfo;
            if (nodeInfo != null && nodeInfo.OppositeXNDNode != null)
            {
                nodeInfo.OppositeXNDNode.ChangeNodeVisible(nodeInfo.IsVisible, false);
            }
        }

        private void Button_CopyToLeftClick(object sender, RoutedEventArgs e)
        {
            LeftRootNodes.Clear();
            if (RightRootNodes.Count != 0)
            {
                var nodeInfo = RightRootNodes[0].CopyNode(this);
                if (nodeInfo != null)
                    LeftRootNodes.Add(nodeInfo);
            }
            TreeView_InfosLeft.UpdateLayout();

            XNDFileNodeCompare compare = new XNDFileNodeCompare(this, null, GetStackPanel(TreeView_InfosRight), null, RightRootNodes);
            compare.ClearRightNodeColor();
            ClearNoUseNode(RightRootNodes);
            TreeView_InfosRight.UpdateLayout();

            UpdateTreeViewNode();
        }

        private void Button_CopyToRightClick(object sender, RoutedEventArgs e)
        {
            RightRootNodes.Clear();
            if (LeftRootNodes.Count != 0)
            {
                var nodeInfo = LeftRootNodes[0].CopyNode(this);
                if (nodeInfo != null)
                    RightRootNodes.Add(nodeInfo);
            }
            TreeView_InfosRight.UpdateLayout();

            XNDFileNodeCompare compare = new XNDFileNodeCompare(this, GetStackPanel(TreeView_InfosLeft), null, LeftRootNodes, null);
            compare.ClearLeftNodeColor();
            ClearNoUseNode(LeftRootNodes);
            TreeView_InfosLeft.UpdateLayout();

            UpdateTreeViewNode();
        }

        void InitNewXNDNode(CSUtility.Support.XndNode node, System.Collections.ObjectModel.ObservableCollection<XNDNodeInfo> XndNodeInfos, bool isFirst)
        {
            foreach (var info in XndNodeInfos)
            {
                if (!info.IsVisible)
                    continue;
                CSUtility.Support.XndNode newNode = null;
                if (isFirst)
                {
                    newNode = node;
                    newNode.SetName(info.Node.GetName());
                }
                else
                {
                    newNode = node.AddNode(info.Node.GetName(), 0, 0); //new CSUtility.Support.XndNode(IntPtr.Zero);
                }
                foreach (var attrib in info.Attribs)
                {
                    byte[] data = null;
                    attrib.Attrib.BeginRead();
                    attrib.Attrib.Read(out data, (int)attrib.Attrib.Length);
                    attrib.Attrib.EndRead();

                    var att = newNode.AddAttrib(attrib.Attrib.GetName());
                    att.Version = attrib.Attrib.Version;
                    att.BeginWrite();
                    att.Write(data);
                    att.EndWrite();
                }
                InitNewXNDNode(newNode, info.Children, false);
            }
        }

        private void Button_SaveLeft_Click(object sender, RoutedEventArgs e)
        {
            if (LeftRootNodes.Count > 0)
            {
                //XNDFileNodeCompare compare = new XNDFileNodeCompare(this, GetStackPanel(TreeView_InfosLeft), GetStackPanel(TreeView_InfosRight), LeftRootNodes, RightRootNodes);
                //compare.ClearLeftNodeColor();
                //compare.ClearRightNodeColor();
                //ClearNoUseNode(LeftRootNodes);
                //ClearNoUseNode(RightRootNodes);
                //TreeView_InfosLeft.UpdateLayout();
                //TreeView_InfosRight.UpdateLayout();

                var holder = CSUtility.Support.XndHolder.NewXNDHolder();
                InitNewXNDNode(holder.Node, LeftRootNodes, true);
                if (FileNameLeft == string.Empty)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "(*" + mRightFilter + ")|";
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        FileNameLeft = sfd.FileName;

                        CSUtility.Support.XndHolder.SaveXND(FileNameLeft, holder);
                        //UpdateTreeViewNode();
                    }
                }
                else
                {
                    CSUtility.Support.XndHolder.SaveXND(FileNameLeft, holder);
                    //UpdateTreeViewNode();
                }
                EditorCommon.MessageBox.Show("文件保存完毕！");
            }
        }

        private void Button_SaveRight_Click(object sender, RoutedEventArgs e)
        {
            if (RightRootNodes.Count > 0)
            {
                //XNDFileNodeCompare compare = new XNDFileNodeCompare(this, GetStackPanel(TreeView_InfosLeft), GetStackPanel(TreeView_InfosRight), LeftRootNodes, RightRootNodes);
                //compare.ClearLeftNodeColor();
                //compare.ClearRightNodeColor();
                //ClearNoUseNode(LeftRootNodes);
                //ClearNoUseNode(RightRootNodes);
                //TreeView_InfosLeft.UpdateLayout();
                //TreeView_InfosRight.UpdateLayout();
                var holder = CSUtility.Support.XndHolder.NewXNDHolder();
                InitNewXNDNode(holder.Node, RightRootNodes, true);
                if (FileNameRight == string.Empty)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "(*" + mLeftFilter + ")|";
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        FileNameRight = sfd.FileName;

                        CSUtility.Support.XndHolder.SaveXND(FileNameRight, holder);
                        //UpdateTreeViewNode();
                    }

                }
                else
                {
                    CSUtility.Support.XndHolder.SaveXND(FileNameRight, holder);
                    //UpdateTreeViewNode();
                }
                EditorCommon.MessageBox.Show("文件保存完毕！");
            }
        }

        private void Button_PrevDifferenceClick(object sender, RoutedEventArgs e)
        {
            if (mDifferenceNodeIndex <= 0)
                return;
            mDifferenceNodeIndex--;
            var item = GetTreeViewItem(LeftRootNodes, GetStackPanel(TreeView_InfosLeft), mDifferenceNodes[mDifferenceNodeIndex]);
            if (item != null)
                item.BringIntoView();
        }

        private void Button_NextDifferenceClick(object sender, RoutedEventArgs e)
        {
            if (mDifferenceNodeIndex >= mDifferenceNodes.Count - 1)
                return;
            mDifferenceNodeIndex++;
            var item = GetTreeViewItem(LeftRootNodes, GetStackPanel(TreeView_InfosLeft), mDifferenceNodes[mDifferenceNodeIndex]);
            if (item != null)
                item.BringIntoView();
            
        }
    }
}
