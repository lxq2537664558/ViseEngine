using System.ComponentModel;
using System.Windows.Controls;

namespace RoleTemplateEditor
{
    /// <summary>
    /// Interaction logic for RoleControl_MeshListItem.xaml
    /// </summary>
    public partial class RoleControl_MeshListItem : UserControl, INotifyPropertyChanged
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

        public delegate void Delegate_OnMeshSet(RoleControl_MeshListItem item);
        public Delegate_OnMeshSet OnMeshSet;

        string mMeshName = "";
        public string MeshName
        {
            get { return mMeshName; }
            set
            {
                mMeshName = value;
                OnPropertyChanged("MeshName");
            }
        }

        CCore.Mesh.MeshTemplate mAttachedMeshTemplate;
        public CCore.Mesh.MeshTemplate AttachedMeshTemplate
        {
            get { return mAttachedMeshTemplate; }
        }

        CCore.World.Actor mAttachedActor;
        public CCore.World.Actor AttachedActor
        {
            get { return mAttachedActor; }
            set { mAttachedActor = value; }
        }

        public RoleControl_MeshListItem(CCore.Mesh.MeshTemplate meshTemplate)
        {
            InitializeComponent();

            mAttachedMeshTemplate = meshTemplate;

            if (mAttachedMeshTemplate != null)
            {
                MeshName = mAttachedMeshTemplate.NickName;
            }
        }

        private void Button_MeshSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mAttachedMeshTemplate == null)
                return;
            var meshFileName = CSUtility.Support.IFileManager.Instance.Root + CCore.Mesh.MeshTemplateMgr.Instance.GetMeshTemplateFile(mAttachedMeshTemplate.MeshID);

            EditorCommon.PluginAssist.PluginOperation.SetObjectToPluginForEdit(new object[] { "ResourcesBrowser", meshFileName });            
        }

        private void Button_MeshSet_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = EditorCommon.PluginAssist.PropertyGridAssist.GetSelectedObjectData("MeshTemplate");
            if (data == null)
                return;

            if (data.Length == 0)
                return;

            var meshId = CSUtility.Program.GetIdFromFile((string)data[0]);

            mAttachedMeshTemplate = CCore.Mesh.MeshTemplateMgr.Instance.FindMeshTemplate(meshId);
            if (mAttachedMeshTemplate != null)
            {
                MeshName = mAttachedMeshTemplate.NickName;

                if (OnMeshSet != null)
                    OnMeshSet(this);
            }
        }
    }
}
