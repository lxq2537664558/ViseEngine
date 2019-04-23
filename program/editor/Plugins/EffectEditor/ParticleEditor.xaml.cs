using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace EffectEditor
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "EffectEditor")]
    //[EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/EffectEditor")]
    [Guid("225ECCDA-1B90-48FF-8D28-675CDC3F7396")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class ParticleEditor : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
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
            get { return "粒子编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "粒子编辑器",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            D3DViewerControl.Activated = true;
            return true;
        }
        public bool OnDeactive()
        {
            D3DViewerControl.Activated = false;
            return true;
        }

        EffectResourceInfo mCurrentEffectTemplateResInfo;
        public void SetObjectToEdit(object[] obj)
        {
            if (obj == null)
                return;
            if (obj.Length == 0)
                return;

            try
            {
                mCurrentEffectTemplateResInfo = obj[0] as EffectResourceInfo;
                SetEffectTemplate(mCurrentEffectTemplateResInfo.EffectTemplate);
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
            //             InitializeD3DShow();
            // 
            //             if (mD3DShowPlugin != null)
            //                 mD3DShowPlugin.Tick();


            D3DViewerControl.Tick();

            if (HostEffect != null)
            {
                HostEffect.SetCamera(D3DViewerControl?.CameraController?.Camera);

                if (LoopView)
                {
                    if (HostEffect.IsFinished)
                    {
                        HostEffect.Reset();
                    }
                }

                // 计算当前场景中粒子的数量
                //                 if (D3DShowPlugin != null)
                //                 {
                //                     D3DShowPlugin.SetObjectToEdit(new object[] { new object[] { "Text", true },
                //                                                                   new object[] { 0, HostEffect.ParticlesCount + "  Particles" }});
                //                 }
                D3DViewerControl.SetObjectToEdit(new object[] { new object[] { "Text", true },
                                                                  new object[] { 0, HostEffect.ParticlesCount + "  Particles" }});

                mInfoControl.UpdateEffectInfo(HostEffect);
            }
        }

        #region D3DPreView

        //[Import("D3DViewer_RemoveActor", typeof(Action<CCore.World.Actor>))]
        //public Action<CCore.World.Actor> D3DViewer_RemoveActor_Delegate { get; set; }

        //[Import("D3DViewer_GetCameraController", typeof(Func<CCore.Camera.CameraController>))]
        //public Func<CCore.Camera.CameraController> D3DViewer_GetCameraController_Delegate { get; set; }

        //[Import("D3DShow", AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]
        //EditorCommon.PluginAssist.IEditorPlugin mD3DShowPlugin = null;
        //public EditorCommon.PluginAssist.IEditorPlugin D3DShowPlugin
        //{
        //    get { return mD3DShowPlugin; }
        //}

        //bool d3dShowInited = false;
        //void InitializeD3DShow(bool force = false)
        //{
        //    if (d3dShowInited && !force)
        //        return;

        //    if (mD3DShowPlugin == null)
        //        return;

        //    if (mD3DShowPlugin is FrameworkElement)
        //    {
        //        Grid_Preview.Children.Clear();
        //        Grid_Preview.Children.Add(mD3DShowPlugin as FrameworkElement);
        //    }

        //    d3dShowInited = true;
        //}

        #endregion

        List<CCore.World.MeshActor> mWorldMeshActors = new List<CCore.World.MeshActor>();
        Dictionary<CCore.Mesh.Mesh, CCore.Modifier.ParticleModifier> mModifierMeshDictionary = new Dictionary<CCore.Mesh.Mesh, CCore.Modifier.ParticleModifier>();
        //CCore.World.EffectActor mEffectActor;

        bool mLoopView = true;
        public bool LoopView
        {
            get { return mLoopView; }
            set
            {
                mLoopView = value;
            }
        }

        static CCore.Modifier.ParticleModifier CopyedParticleModifier = new CCore.Modifier.ParticleModifier();

        public delegate void Delegate_OnSaveSource(object obj);
        public Delegate_OnSaveSource OnSaveSource;

        ParticleInfo mInfoControl = new ParticleInfo();

        public ParticleEditor()
        {
            InitializeComponent();

            Grid_Main.Children.Remove(ToolBar_Main);
            D3DViewerControl.AddToolBar(ToolBar_Main);
            D3DViewerControl.AddInfoControl(mInfoControl);

            var template = this.TryFindResource("EffectSetControl") as DataTemplate;
            WPG.Program.RegisterDataTemplate("EffectSet", template);            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public CCore.Effect.EffectTemplate HostEffect
        {
            get;
            private set;
        }
        //public ParticleModifierTreeNode RootTNode
        //{
        //    get;
        //    private set;
        //}

        public void RefreshEffectTemplate()
        {
            SetEffectTemplate(HostEffect);
        }
        private void SetEffectTemplate(CCore.Effect.EffectTemplate effectTemplate)
        {
            var isDirty = effectTemplate.IsDirty;

            //D3DEvrCtrl.SetMesh("");

            if (HostEffect != null)
                HostEffect._OnPropertyChanged = null;

            HostEffect = effectTemplate;

            foreach (var modifier in HostEffect.Modifiers)
            {
                modifier.SetWorldTransMatrix(SlimDX.Matrix.Identity);
            }

            HostEffect._OnPropertyChanged = OnEffectTemplatePropertyChanged;
            
            // 注意，这里有可能CameraController为空
            HostEffect.SetCamera(D3DViewerControl?.CameraController?.Camera);

            //if (mEffectActor != null)
            //{
            //    D3DViewerControl.RemoveActorFromWorld(mEffectActor);                
            //}

            //var visInit = new CCore.Component.EffectVisualInit()
            //{
            //    EffectTemplateID = HostEffect.Id
            //};

            //var vis = new CCore.Component.EffectVisual();
            //vis.Initialize(visInit, null);
            //var effActInit = new CCore.World.EffectActorInit();
            //mEffectActor = new CCore.World.EffectActor();
            //mEffectActor.Initialize(effActInit);
            //mEffectActor.Visual = vis;
            //mEffectActor.SetPlacement(new CSUtility.Component.StandardPlacement(mEffectActor));            
            //D3DViewerControl.AddActorToWorld(mEffectActor);           

            MainProperties.Instance = HostEffect;

            D3DViewerControl.SetObjectToEdit(null);
            foreach (var actor in mWorldMeshActors)
            {             
                var meshVis = actor.Visual as CCore.Mesh.Mesh;
                meshVis.OnMeshReInitialized -= OnReInitMesh;
            }
            mWorldMeshActors.Clear();
            StackPanel_Particles.Children.Clear();
            mModifierMeshDictionary.Clear();
            foreach (var modifier in HostEffect.Modifiers)
            {
                var meshInit = new CCore.Mesh.MeshInit()
                {
                    MeshTemplateID = modifier.MeshTemplateId,
                };
                var mesh = new CCore.Mesh.Mesh();
                mesh.Initialize(meshInit, null);
                mesh.OnMeshReInitialized += OnReInitMesh;

                var meshActorInit = new CCore.World.MeshActorInit();
                var meshActor = new CCore.World.MeshActor();
                meshActor.Initialize(meshActorInit);
                meshActor.Visual = mesh;
                meshActor.SetPlacement(new CSUtility.Component.StandardPlacement(meshActor));                
                mesh.SetParticleModifier(modifier);                
                mModifierMeshDictionary[mesh] = modifier;

                D3DViewerControl.SetObjectToEdit(new object[] { new object[] { "Actor", true  },
                                                               new object[] { meshActor }});
                //D3DViewerControl.AddActorToWorld(meshActor);
                mWorldMeshActors.Add(meshActor);

                var modifierItem = new ParticleModifierItem(this, modifier);

                if (GetSelectedparticleEditorItem() != null && GetSelectedparticleEditorItem().GetPropertyShowObject() == modifier)
                {
                    SetSelectedParticleEditorItem(modifierItem);
                }
                
                StackPanel_Particles.Children.Add(modifierItem);
            }

            effectTemplate.IsDirty = isDirty;
            ItemProperties.Instance = null;
        }

        private void OnReInitMesh(CCore.Mesh.Mesh mesh)
        {
            CCore.Modifier.ParticleModifier modifier;
            if (mModifierMeshDictionary.TryGetValue(mesh, out modifier))
                mesh.SetParticleModifier(modifier);
        }

        static ParticleEditorItemInterface mSelectedItem = null;
        public static ParticleEditorItemInterface SelectedItem
        {
            get { return mSelectedItem; }
        }
        public ParticleEditorItemInterface GetSelectedparticleEditorItem()
        {
            return mSelectedItem;
        }
        public void SetSelectedParticleEditorItem(ParticleEditorItemInterface item)
        {
            if (mSelectedItem != null)
            {
                mSelectedItem.IsSelected = false;
                mSelectedItem.IsCurrentEditModifier = false;
            }

            mSelectedItem = item;
            if (mSelectedItem != null)
            {
                mSelectedItem.IsSelected = true;
                mSelectedItem.IsCurrentEditModifier = true;
                var obj = mSelectedItem.GetPropertyShowObject();
                ItemProperties.Instance = obj;
                if (obj != null)
                {
                    ItemProperties.Headline = obj.GetType().Name;
                }
                else
                {
                    ItemProperties.Headline = "";
                }
            }
            else
            {
                ItemProperties.Instance = null;
                ItemProperties.Headline = "";
            }
        }
        //ParticleModifierItem mSelectedModifierItem = null;
        //private void OnSelectedParticleModifierItem(ParticleModifierItem item)
        //{
        //    if (mSelectedModifierItem != null)
        //        mSelectedModifierItem.IsSelected = false;

        //    mSelectedModifierItem = item;
        //    if(mSelectedModifierItem != null)
        //        mSelectedModifierItem.IsSelected = true;
        //}
        //ParticleModifierItem mCurrentEditModifier = null;
        //public void SetCurrentEditModifier(ParticleModifierItem item)
        //{
        //    if (mCurrentEditModifier != null)
        //        mCurrentEditModifier.IsCurrentEditModifier = false;

        //    mCurrentEditModifier = item;
        //    if (mCurrentEditModifier != null)
        //        mCurrentEditModifier.IsCurrentEditModifier = true;
        //}

        private void Button_AddParticle_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HostEffect == null)
                return;

            var modifier = HostEffect.AddParticleModifier();
            SetEffectTemplate(HostEffect);
        }

        public void RemoveParticle(ParticleModifierItem item)
        {
            HostEffect.RemoveParticleModifier(item.HostParticleModifier);
            SetEffectTemplate(HostEffect);
        }
        //private void bnDelEmitter_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    var node = trvParticleObjs.SelectedItem as ParticleModifierTreeNode;
        //    if (node.TargetObj is CCore.Particle.ParticleEmitter)
        //    {
        //        HostModifier.RemoveEmitterById(((CCore.Particle.ParticleEmitter)node.TargetObj).Id);
        //        RefreshModifierTree();
        //    }
        //}

        private void Button_Reset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HostEffect == null)
                return;

            HostEffect.Reset();
        }

        private void Button_Copy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mSelectedItem == null)
                return;

            var modifierItem = mSelectedItem as ParticleModifierItem;
            if (modifierItem == null)
                return;

            CopyedParticleModifier.CopyFrom(modifierItem.HostParticleModifier);
        }
        private void Button_Paste_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var modifier = HostEffect.AddParticleModifier();
            modifier.CopyFrom(CopyedParticleModifier);
            SetEffectTemplate(HostEffect);
        }
        private void Button_ResetID_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mSelectedItem is ParticleModifierItem)
            {
                var modifierItem = mSelectedItem as ParticleModifierItem;
                if (modifierItem == null)
                    return;

                if (modifierItem.HostParticleModifier == null)
                    return;

                modifierItem.HostParticleModifier.Id = Guid.NewGuid();
                foreach (var emit in modifierItem.HostParticleModifier.Emitters)
                {
                    emit.Id = Guid.NewGuid();
                }
            }
            else if (mSelectedItem is ParticleEmitterItem)
            {
                var emitterItem = mSelectedItem as ParticleEmitterItem;
                if (emitterItem == null)
                    return;

                if (emitterItem.HostParticleEmitter == null)
                    return;

                emitterItem.HostParticleEmitter.Id = Guid.NewGuid();
            }
        }

        private void Button_ShowAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var modifierItem in StackPanel_Particles.Children)
            {
                if (!(modifierItem is ParticleModifierItem))
                    continue;

                var item = modifierItem as ParticleModifierItem;

                item.IsActive = true;
            }
        }
        private void Button_HideAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var modifierItem in StackPanel_Particles.Children)
            {
                if (!(modifierItem is ParticleModifierItem))
                    continue;

                var item = modifierItem as ParticleModifierItem;

                item.IsActive = false;
            }
        }

        private void bnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mCurrentEffectTemplateResInfo == null)
                return;

            Program.SaveEffectTemplate(mCurrentEffectTemplateResInfo);
            
        }

        private void OnEffectTemplatePropertyChanged(string propertyName)
        {
            if (HostEffect == null)
                return;
// 
//             if (mCurrentEffectTemplateResInfo != null && propertyName == "NickName")
//             {
//                 mCurrentEffectTemplateResInfo.Name = HostEffect.NickName;
//             }
        }

        private void Button_SaveSnapshot_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentEffectTemplateResInfo == null)
                return;

            var snapShotFile = mCurrentEffectTemplateResInfo.AbsResourceFileName + ResourcesBrowser.Program.SnapshotExt;

            this.Dispatcher.Invoke(new Action(() =>
            {
                var path = CSUtility.Support.IFileManager.Instance.GetPathFromFullName(snapShotFile);
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                D3DViewerControl.SaveToFile(snapShotFile, CCore.enD3DXIMAGE_FILEFORMAT.D3DXIFF_PNG);
            }));

            mCurrentEffectTemplateResInfo.Snapshot = ResourcesBrowser.Program.LoadImage(snapShotFile);
        }

        //public void SetItemShowProperty(object item)
        //{
        //    ItemProperties.Instance = item;
        //    ItemProperties.Headline = item.GetType().Name;
        //}
    }
}
