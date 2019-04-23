using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainEditor.Panel
{
    /// <summary>
    /// PlantItem.xaml 的交互逻辑
    /// </summary>
    public partial class PlantItem : TreeViewItem, EditorCommon.DragDrop.IDragAbleObject, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        string mNodeName = "";
        public string NodeName
        {
            get { return mNodeName; }
            set
            {
                mNodeName = value;
                TextBlock_Name.Text = mNodeName;
            }
        }

        string mIconUri = "";
        public string IconUri
        {
            get { return mIconUri; }
            set
            {
                mIconUri = value;
                
                if(!string.IsNullOrEmpty(mIconUri))
                {
                    Image_Icon.Source = new BitmapImage(new Uri(mIconUri, UriKind.Absolute));
                }
            }
        }

        string mDescription = "";
        public string Description
        {
            get { return mDescription; }
            set
            {
                mDescription = value;
                if(string.IsNullOrEmpty(mDescription))
                {
                    ToolTip = null;
                }
                else
                {
                    ToolTip = mDescription;
                }
            }
        }

        Type mNodeType = null;
        public Type NodeType
        {
            get { return mNodeType; }
            set
            {
                mNodeType = value;

                if (mNodeType != null)
                    mTempObject = System.Activator.CreateInstance(mNodeType) as CCore.EditorAssist.IPlantAbleObject;
                else
                    mTempObject = null;

                OnPropertyChanged("NodeType");
            }
        }

        string mSelectString = "";
        public string SelectString
        {
            get { return mSelectString; }
            set
            {
                mSelectString = value;

                if(string.IsNullOrEmpty(mSelectString))
                {
                    TextBlock_Name.Text = "";
                    TextBlock_Name.Inlines.Add(new Run(NodeName));
                }
                else
                {
                    TextBlock_Name.Inlines.Clear();
                    TextBlock_Name.Text = "";
                    
                    int startIdx = 0;
                    while (true)
                    {
                        var idx = NodeName.IndexOf(mSelectString, startIdx, StringComparison.OrdinalIgnoreCase);
                        if (idx < 0)
                            break;

                        TextBlock_Name.Inlines.Add(new Run(NodeName.Substring(startIdx, idx - startIdx)));
                        TextBlock_Name.Inlines.Add(new Run(NodeName.Substring(idx, mSelectString.Length)) { Background = new SolidColorBrush(Color.FromRgb(149,96,0)) });
                        startIdx = idx + mSelectString.Length;
                    }

                    TextBlock_Name.Inlines.Add(new Run(NodeName.Substring(startIdx)));
                }
            }
        }

        CCore.EditorAssist.IPlantAbleObject mTempObject;
        PlantPanel mParentPanel;

        public PlantItem(PlantPanel parentPanel)
        {
            InitializeComponent();

            mParentPanel = parentPanel;
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);

            if (mTempObject == null)
                mParentPanel.ShowObjectProperty(null);
            else
                mParentPanel.ShowObjectProperty(mTempObject.GetPropertyShowObject());
        }

        #region DragDrop

        public System.Windows.FrameworkElement GetDragVisual()
        {
            var header = this.Header as System.Windows.FrameworkElement;
            return VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(header)) as System.Windows.FrameworkElement;
        }

        Point mMouseDownPos = new Point();
        private void TreeViewItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var listItem = sender as PlantItem;
            if (listItem == null)
                return;

            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                mMouseDownPos = e.GetPosition(listItem);
            }
        }

        private void TreeViewItem_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var listItem = sender as PlantItem;
            if (listItem == null)
                return;

            if (listItem.HasItems)
                return;

            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(listItem);
                if (((pos.X - mMouseDownPos.X) > 3) ||
                   ((pos.Y - mMouseDownPos.Y) > 3))
                {
                    EditorCommon.DragDrop.DragDropManager.Instance.StartDrag("WorldPlantItem", new EditorCommon.DragDrop.IDragAbleObject[] { listItem });
                }
            }
        }

        CCore.World.Actor mGameWindowDragActor = null;
        public void OnDragEnterGameWindow(System.Windows.Forms.Form form, System.Windows.Forms.DragEventArgs e)
        {
            var actor = mTempObject.GetPreviewActor(CCore.Engine.Instance.Client.MainWorld);
            CCore.Engine.Instance.Client.MainWorld.AddEditorActor(actor);
            mGameWindowDragActor = actor;
        }
        public void OnDragLeaveGameWindow(System.Windows.Forms.Form form, EventArgs e)
        {
            if (mGameWindowDragActor == null)
                return;

            CCore.Engine.Instance.Client.MainWorld.RemoveEditorActor(mGameWindowDragActor);
            mGameWindowDragActor.Cleanup();
            mGameWindowDragActor = null;
        }
        public void OnDragOverGameWindow(System.Windows.Forms.Form form, System.Windows.Forms.DragEventArgs e)
        {
            if (mGameWindowDragActor == null)
                return;

            var pos = EditorCommon.DragDrop.DragDropManager.Instance.GetMousePos();
            pos.X -= form.Left;
            pos.Y -= form.Top;
            var hitPos = EditorCommon.Assist.Assist.IntersectWithWorld((int)pos.X, (int)pos.Y, CCore.Engine.Instance.Client.MainWorld, EditorCommon.Program.GameREnviroment?.Camera, (UInt32)CSUtility.enHitFlag.HitMeshTriangle);
            hitPos.Y += 0.5f;
            mGameWindowDragActor.Placement.SetLocation(ref hitPos);
        }
        public CCore.World.Actor OnDragDropGameWindow(System.Windows.Forms.Form form, System.Windows.Forms.DragEventArgs e)
        {
            if (mGameWindowDragActor == null)
                return null;

            CCore.Engine.Instance.Client.MainWorld.RemoveEditorActor(mGameWindowDragActor);
            mGameWindowDragActor.Cleanup();
            mGameWindowDragActor = null;

            var actor = mTempObject.GetPlantActor(CCore.Engine.Instance.Client.MainWorld);
            var pos = EditorCommon.DragDrop.DragDropManager.Instance.GetMousePos();
            pos.X -= form.Left;
            pos.Y -= form.Top;
            var hitPos = EditorCommon.Assist.Assist.IntersectWithWorld((int)pos.X, (int)pos.Y, CCore.Engine.Instance.Client.MainWorld, EditorCommon.Program.GameREnviroment?.Camera, (UInt32)CSUtility.enHitFlag.HitMeshTriangle);
            hitPos.Y += 0.5f;
            actor.Placement.SetLocation(ref hitPos);
            CCore.Engine.Instance.Client.MainWorld.AddActor(actor);

            return actor;
        }

        #endregion
    }
}
