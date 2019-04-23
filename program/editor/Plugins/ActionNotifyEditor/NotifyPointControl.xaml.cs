using System;

namespace ActionNotifyEditor
{
    /// <summary>
    /// Interaction logic for EffectNotifyPointControl.xaml
    /// </summary>
    public partial class NotifyPointControl : TimeLine.TimeLineTrackItemPropertyControl_Base
    {
        public ActionNotifyEditorControl HostControl = null;

        CSUtility.Animation.NotifyPoint mNotifyPoint = null;
        public CSUtility.Animation.NotifyPoint NotifyPoint
        {
            get { return mNotifyPoint; }
        }

        public override object PropertyInstance
        {
            get { return mPropertyInstance; }
            set
            {
                mPropertyInstance = value;
                if (value == null)
                {
                    ListBox_PointData.Items.Clear();
                }
                else
                {
                    mNotifyPoint = value as CSUtility.Animation.NotifyPoint;
                    if (mNotifyPoint != null)
                    {
                        ListBox_PointData.Items.Clear();
                        foreach (var i in mNotifyPoint.PointDatas)
                        {
                            ListBox_PointData.Items.Add(new NotifyPointListItem(i));
                        }
                    }                    
                }
                               

                OnPropertyChanged("PropertyInstance");
            }
        }

        public void InitPointItemData(CSUtility.Animation.NotifyItemDataBase itemData)
        {
            Type type = itemData.GetType();
            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                var atts = p.GetCustomAttributes(typeof(CSUtility.Animation.V3dBox3Attribute), true);
                if (atts.Length == 0)
                    continue;
                var att = atts[0] as CSUtility.Animation.V3dBox3Attribute;

                CCore.Component.V3dBox3 boxVis = new CCore.Component.V3dBox3();
                boxVis.Color = att.NormalColor;

                var act = p.GetValue(itemData) as CCore.World.Actor;
                if (act == null)
                {
                    var actor = p.GetValue(itemData) as CSUtility.Component.ActorBase;
                    var newActor = new CCore.World.Actor();
                    newActor.SetPlacement(new CSUtility.Component.StandardPlacement(newActor));
                    var actInit = new CCore.World.ActorInit();
                    newActor.Initialize(actInit);
                    newActor.Visual = boxVis;

                    SlimDX.Matrix mat = actor.Placement.mMatrix;
                    newActor.Placement.SetMatrix(ref mat);

                    p.SetValue(itemData, newActor);                    
                }
            }
        }

        public override bool IsActive
        {
            get { return mIsActive; }
            set
            {
                mIsActive = value;

                OnPropertyChanged("IsActive");
            }
        }

        public NotifyPointControl()
        {
            InitializeComponent();
        }

        private void Button_Add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mNotifyPoint == null)
                return;

            var itemData = mNotifyPoint?.AddNewItemData(mNotifyPoint);
            if (itemData == null)
                return;
            InitPointItemData(itemData);

            ListBox_PointData.Items.Add(new NotifyPointListItem(itemData));

            HostControl?.AddActorToD3DViewer(itemData, true);
        }

        private void Button_Del_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = ListBox_PointData.SelectedItem as NotifyPointListItem;
            if (item == null)
            {
                return;
            }
            
            ListBox_PointData.Items.Remove(item);
            mNotifyPoint?.RemoveItemData(item.PointItemData);
            HostControl?.AddActorToD3DViewer(item.PointItemData, false);
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
//             foreach(NotifyPointListItem i in e.AddedItems)
//             {
//                 //OnItemDataSelected(i.PointItemData, true);
//             }
// 
//             foreach (NotifyPointListItem i in e.RemovedItems)
//             {
//                 //OnItemDataSelected(i.PointItemData, false);
//             }
        }

        public void OnItemDataSelected(CSUtility.Animation.NotifyItemDataBase itemData, bool selected)
        {
            if (itemData == null)
                return;
            Type type = itemData.GetType();
            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                var atts = p.GetCustomAttributes(typeof(CSUtility.Animation.V3dBox3Attribute), true);
                if (atts.Length == 0)
                    continue;
                var att = atts[0] as CSUtility.Animation.V3dBox3Attribute;

                var actor = p.GetValue(itemData) as CCore.World.Actor;
                var box = actor.Visual as CCore.Component.V3dBox3;
                if (selected)
                {
                    box.Color = att.NormalColor;
                }
                else
                {
                    box.Color = att.SelectedColor;
                }
            }            
        }
    }
}
