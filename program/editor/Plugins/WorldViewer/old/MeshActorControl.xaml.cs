using System;
using System.Windows;
using System.Windows.Controls;

namespace WorldViewer
{
    /// <summary>
    /// Interaction logic for MeshActorControl.xaml
    /// </summary>
    public partial class MeshActorControl : UserControl
    {
        bool mShowTitle = true;
        public bool ShowTitle
        {
            get { return mShowTitle; }
            set
            {
                mShowTitle = value;

                if (mShowTitle)
                    TextBlock_Name.Visibility = Visibility.Visible;
                else
                    TextBlock_Name.Visibility = Visibility.Collapsed;
            }
        }

        //CCore.World.Actor mHostActor;
        //public CCore.World.Actor HostActor
        //{
        //    get { return mHostActor; }
        //    set
        //    {
        //        mHostActor = value;

        //        TextBlock_Name.Text = mHostActor.GetType().Name;
        //        var mesh = mHostActor.Visual as CCore.Mesh.Mesh;
        //        if (mesh != null)
        //        {
        //            var meshInit = mesh.VisualInit as CCore.Mesh.MeshInit;
        //            if(meshInit != null)
        //            {
        //                MeshTemplateControl.CurMeshId = meshInit.MeshTemplateID;
        //                if(meshInit.MeshTemplate != null)
        //                    MeshPartsControl.MeshInitParts = meshInit.MeshTemplate.MeshInitList;
        //            }
        //        }
        //    }
        //}

        string mTitleString = "";
        public string TitleString
        {
            get { return mTitleString; }
            set
            {
                mTitleString = value;

                TextBlock_Name.Text = mTitleString;
            }
        }

        Guid mMeshTemplateId;
        public Guid MeshTemplateId
        {
            get { return mMeshTemplateId; }
            set
            {
                mMeshTemplateId = value;

                ////////////MeshTemplateControl.CurMeshId = mMeshTemplateId;
                ////////////var meshTemplate = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(mMeshTemplateId);
                ////////////if (meshTemplate != null)
                ////////////{
                ////////////    MeshPartsControl.MeshInitParts = meshTemplate.MeshInitList;
                ////////////}
            }
        }

        public MeshActorControl()
        {
            InitializeComponent();
        }
    }
}
