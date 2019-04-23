using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MeshSourceEditor
{
    /// <summary>
    /// MainControl.xaml 的交互逻辑
    /// </summary>
    /// 
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "MeshSourceEditor")]    
    [Guid("a3e4c25a-0160-4dd0-8fc2-16cea9f949cb")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class MainControl : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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

        public string PluginName
        {
            get { return "模型资源编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }
 
        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "MeshSourceEditor",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }
        
        public bool OnActive()
        {
            D3DViewer.Activated = true;
            return true;
        }
        public bool OnDeactive()
        {
            D3DViewer.Activated = false;
            return true;
        }

        MeshSourceResourceInfo mCurrentMeshSourceResInfo;
        public void SetObjectToEdit(object[] obj)
        {
            if (obj == null)
                return;
            if (obj.Length == 0)
                return;

            try
            {
                mCurrentMeshSourceResInfo = obj[0] as MeshSourceResourceInfo;
                SetMeshSource(mCurrentMeshSourceResInfo);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
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
            D3DViewer.Tick();
        }

        MeshInfo mInfoControl = new MeshInfo();

        public MainControl()
        {
            InitializeComponent();

            D3DViewer.AddInfoControl(mInfoControl);

            mainGrid.Children.Remove(toolBar);
            D3DViewer.AddToolBar(toolBar);
            toolBar.Visibility = Visibility.Visible;
        }        

        CCore.Mesh.Mesh mCurMesh;        
        private void SetMeshSource(MeshSourceResourceInfo meshSourceInfo)
        {
            var actorInit = new CCore.World.MeshActorInit();
            var actor = new CCore.World.MeshActor();
            actor.Initialize(actorInit);
            actor.SetPlacement(new CSUtility.Component.StandardPlacement(actor));

            var visual = new CCore.Mesh.Mesh();
            var mshInit = new CCore.Mesh.MeshInit();
            CCore.Mesh.MeshInitPart mshInitPart = new CCore.Mesh.MeshInitPart();
            mshInitPart.MeshName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(meshSourceInfo.AbsResourceFileName);
            mshInit.MeshInitParts.Add(mshInitPart);
            mshInit.CanHitProxy = false;
            visual.Initialize(mshInit, null);                                    

            actor.Visual = visual;

            D3DViewer.SetObjectToEdit(new object[] { new object[]{ "Actor", false },
                                                         new object[]{ actor }});            
            mCurMesh = visual;


            SimMeshCtrl.CurrentMesh = mCurMesh;
            SimMeshCtrl.CurrentSourceInfo = meshSourceInfo;

            ProGrid.Instance = actor;
            var anim = visual.GetAnimTree();
            //             if (anim != null)
            //             {
            //                 AnimationPlaying = !anim.GetPause();
            //             }

            ShowPathMesh = mCurMesh.PathMeshVisible;
//            ShowSimplateMesh = mCurMesh.EditorShow;
            ShowMainMesh = mCurMesh.MainMeshVisible;

            mInfoControl.UpdateMeshInfo(visual);
        }

        private void CreateCollision_Click(object sender, RoutedEventArgs e)
        {
//             if (popup != null)
//             {
//                 popup.PlacementTarget = sender as UIElement;
//                 popup.IsOpen = true;
//             }                
        }

        bool mShowPathMesh = false;
        public bool ShowPathMesh
        {
            get { return mShowPathMesh; }
            set
            {
                mShowPathMesh = value;
                
                if (mCurMesh != null)
                    mCurMesh.PathMeshVisible = value;

                OnPropertyChanged("ShowPathMesh");
            }
        }

        bool mShowSimplateMesh = false;
        public bool ShowSimplateMesh
        {
            get { return mShowSimplateMesh; }
            set
            {
                mShowSimplateMesh = value;

                //                if (mCurMesh != null)
                //                    mCurMesh.EditorShow = value;
                CCore.Program.SetActorTypeShow(D3DViewer.World, CCore.Program.SimplyMeshTypeName, mShowSimplateMesh);
                
                OnPropertyChanged("ShowSimplateMesh");
            }
        }

        bool mShowMainMesh = false;
        public bool ShowMainMesh
        {
            get { return mShowMainMesh; }
            set
            {
                mShowMainMesh = value;

                if (mCurMesh != null)
                    mCurMesh.MainMeshVisible = value;

                OnPropertyChanged("ShowMainMesh");
            }
        }
    }
}
