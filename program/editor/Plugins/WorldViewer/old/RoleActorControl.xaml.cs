using System;
using System.Windows;
using System.Windows.Controls;

namespace WorldViewer
{
    /// <summary>
    /// Interaction logic for RoleActorControl.xaml
    /// </summary>
    public partial class RoleActorControl : UserControl
    {
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

        UInt16 mRoleTemplateId = UInt16.MaxValue;
        public UInt16 RoleTemplateId
        {
            get { return mRoleTemplateId; }
            set
            {
                mRoleTemplateId = value;

                //RoleTemplateSetterCtrl.RoleTemplateId = mRoleTemplateId;

                var roleTemplate = CSUtility.Data.RoleTemplateManager.Instance.FindRoleTemplate(RoleTemplateId);
                if (roleTemplate == null)
                {
                    EditorCommon.MessageBox.Show("RoleTemplateID(" + RoleTemplateId + ") 未找到!!!");
                }
                else
                {
                    foreach (var meshId in roleTemplate.DefaultMeshs)
                    {
                        MeshActorControl ctrl = new MeshActorControl();
                        ctrl.MeshTemplateId = meshId;
                        ctrl.ShowTitle = false;
                        StackPanel_Meshes.Children.Add(ctrl);
                    }
                }
            }
        }

        Guid mAIGuid = Guid.Empty;
        public Guid AIGuid
        {
            get { return mAIGuid; }
            set
            {
                mAIGuid = value;
                //AISetterCtrl.AIGuid = mAIGuid;
            }
        }

        public RoleActorControl()
        {
            InitializeComponent();
        }
    }
}
