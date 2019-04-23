using System;

namespace ResourcesBrowser.SnapshotProcess
{
    public class SnapshotCreator : System.Windows.Threading.DispatcherObject
    {
        protected class SnapshotWinRoot : UISystem.WinRoot
        {
            public string FPSString = "";
            //System.Drawing.Rectangle mDestRect = new System.Drawing.Rectangle();
            protected override void AfterStateDraw(UISystem.UIRenderPipe pipe, int zOrder)
            {
                //UISystem.IRender.GetInstance().SetClipRect(this.AbsRect);
                //UISystem.IRender pRender = UISystem.IRender.GetInstance();
                ////if (FrameSet.Inventory.TransforBar.Instance.Item != null)
                ////{
                ////    UISystem.UVFrame frame = FrameSet.Inventory.TransforBar.Instance.Item.Data.Template.Icon.GetUVFrame(CCore.Engine.Instance.GetFrameSecondTimeFloat());
                ////    pRender.DrawImage(this.Width, this.Height,
                ////        FrameSet.Inventory.TransforBar.Instance.Item.Data.Template.Icon.TextureObject,
                ////        ref mDestRect,
                ////        ref frame.mUVRect,
                ////        FrameSet.Inventory.TransforBar.Instance.Item.Data.Template.Icon.MaterialObject);
                ////}
                //UISystem.IRender.GetInstance().SetClipRect(this.AbsRect);
                //pRender.DrawString(20, 20, FPSString, 20, System.Drawing.Color.FromArgb(255, 255, 0, 0));
            }

            //public override UISystem.MSG_PROC ProcMessage(ref UISystem.WinMSG msg)
            //{
            //    //if (msg.message == (UInt32)SysMessage.VWM_MOUSEMOVE)
            //    //{
            //    //    mDestRect.X = msg.pt.X;
            //    //    mDestRect.Y = msg.pt.Y;
            //    //    mDestRect.Width = 32;
            //    //    mDestRect.Height = 32;
            //    //}
            //    return base.ProcMessage(ref msg);
            //}
        }


        public delegate void Delegate_Tick();

        static SnapshotCreator scInstance = new SnapshotCreator();
        public static SnapshotCreator Instance
        {
            get
            {
                return scInstance;
            }
        }

        bool m_bRenderingSnapshots = false;
        public bool RenderingSnapshots
        {
            get { return m_bRenderingSnapshots; }
        }

        CCore.Graphics.REnviroment mREnviroment;
        CCore.World.WorldRenderParam mRenderparam;

        CCore.World.World mWorld;
        public CCore.World.World World
        {
            get { return mWorld; }
        }

        CCore.Camera.CameraController mCameraController;
        
        // 界面
        SnapshotWinRoot mWinRoot = new SnapshotWinRoot();
        UISystem.WinForm mForm;

        UInt32 m_nWidth = 256;
        UInt32 m_nHeight = 256;

        public SnapshotCreator()
        {
            InitD3DEnviroment();
        }

        private void InitD3DEnviroment()
        {
            System.Windows.Forms.Panel drawPanel = new System.Windows.Forms.Panel();
            drawPanel.Width = (int)m_nWidth;
            drawPanel.Height = (int)m_nHeight;

            var _reInit = new CCore.Graphics.REnviromentInit();
            _reInit.ViewInit = new CCore.Graphics.ViewInit();
            _reInit.ViewInit.ViewWnd.SetControl(drawPanel);
            _reInit.ZNear = 0.1f;
            _reInit.ZFar = 10000.0f;
            var view = new CCore.Graphics.View();
            view.Initialize(_reInit.ViewInit);
            mREnviroment = new CCore.Graphics.REnviroment();
            if (false == mREnviroment.Initialize(_reInit, view))
                return;

            //mREnviroment.AfterRender2View += AfterRender2View;
            mREnviroment.BeforeCopyTexture += this.BeforeCopyTexture;
            //mREnviroment.SetClearColorMRT(CSUtility.Support.Color.FromArgb(255, CSUtility.Support.Color.Blue));
            mREnviroment.SetClearColorMRT(CSUtility.Support.Color.FromArgb(0, CSUtility.Support.Color.Black));
            //m_REnviroment.ResizeInternalRT(m_nWidth, m_nHeight);
            mRenderparam = new CCore.World.WorldRenderParam();
            mRenderparam.Enviroment = mREnviroment;
            var worldInit = new CCore.World.WorldInit();
            worldInit.SceneGraphInfo = new CCore.Scene.SingleSceneGraphInfo();
            mWorld = new CCore.World.World(Guid.NewGuid());
            mWorld.Initialize(worldInit);
            mWorld.Initialize("", "场景");

            var sunActor = new CCore.World.LightActor();
            var sunInit = new CCore.World.LightActorInit();
            sunInit.LightType = CCore.Light.ELightType.Dir;
            sunActor.Initialize(sunInit);
            var sun = sunActor.Light as CCore.Light.Light;
            if (sun != null)
            {
                sun.ShadowType = CCore.Light.EShadowType.None;
                sun.Ambient = CSUtility.Support.Color.Gray;
                sun.Diffuse = CSUtility.Support.Color.White;
                sun.Intensity = 2.0f;
                sunActor.SetPlacement(new CSUtility.Component.StandardPlacement(sunActor));
                var sunPos = new SlimDX.Vector3(0, 300, 0);
                sunActor.Placement.SetLocation(ref sunPos);
                var sunQuat = SlimDX.Quaternion.RotationYawPitchRoll(0.0f, 0.3f, 0.3f);
                sunActor.Placement.SetRotation(ref sunQuat);
            }
            mWorld.AddActor(sunActor);


            mWorld.PostProceses.Clear();

            //var ssao = new CCore.Graphics.PostProcess_SSAO();
            //ssao.Enable = false;
            //ssao.RandomNormalTexName = "texture/Effect/Program/RandomNormal.jpg";
            //ssao.DoBlur = true;
            //ssao.SampleRad = 0.1f;
            //ssao.Bias = 0;
            //ssao.Intensity = 1;
            //mWorld.PostProceses.Add(ssao);

            //FrameSet.PostProcess_Bloom bloom = new FrameSet.PostProcess_Bloom();
            //bloom.Enable = true;
            //bloom.BloomImageScale = 0.5f;
            //bloom.BlurStrength = 0.5f;
            //bloom.BlurAmount = 5;
            //bloom.BlurType = MidLayer.enBlurType.BoxBlur;
            //m_World.PostProceses.Add(bloom);

            CCore.Graphics.PostProcess_ToneMapping toneMapping = new CCore.Graphics.PostProcess_ToneMapping();
            toneMapping.Enable = true;
            toneMapping.W = 1;
            toneMapping.BrightFactor = 1;
            mWorld.PostProceses.Add(toneMapping);

            var colorGrading = new CCore.Graphics.PostProcess_ColorGrading();
            colorGrading.Enable = true;
            colorGrading.EnableFXAA = false;
            colorGrading.EnableColorGrading = false;    // 关掉ColorGrading，否则颜色有偏差，抠图错误
            colorGrading.ColorGradingTexName = CSUtility.Support.IFileConfig.DefaultColorGradeFile;
            colorGrading.GammaCorrect = true;
            mWorld.PostProceses.Add(colorGrading);


            //// 格子对象
            //MidLayer.IUtility.IGrid grid = new MidLayer.IUtility.IGrid();
            //MidLayer.ICommActorInit gridInit = new MidLayer.ICommActorInit();
            //CCore.World.Actor gridActor = new CCore.World.Actor();
            //gridActor.Initialize(gridInit);
            //gridActor.Visual = grid;
            //gridActor.mPlacement = new CSUtility.Component.StandardPlacement(gridActor);
            //m_World.AddCommActor(gridActor);

            //m_Mesh = new CCore.Mesh.Mesh();
            ////SetMesh("Mesh/Assist/sphere.vms");

            //m_Mesh.CanHitProxy = false;

            //var caInit = new CCore.World.MeshActorInit();
            //m_MeshActor = new CCore.World.MeshActor();
            //m_MeshActor.Initialize(caInit);
            ////m_MeshActor.Visual = m_Mesh;
            //m_MeshActor.SetPlacement(new CSUtility.Component.StandardPlacement(m_MeshActor));
            //mWorld.AddEditorActor(m_MeshActor);

            //m_ActionNode = new IAnimTreeNode_Action();
            //m_ActionNode.Initialize();
            //m_SkeletonVisual = new ISkeletonVisual();
            //IVisualInit skeInit = new IVisualInit();
            //m_SkeletonVisual.Initialize(skeInit, null);
            //MidLayer.ICommActorInit skeActorInit = new MidLayer.ICommActorInit();
            //m_SkeletonActor = new CCore.World.Actor();
            //m_SkeletonActor.Initialize(skeActorInit);
            //m_SkeletonActor.Visual = m_SkeletonVisual;
            //m_SkeletonActor.SetPlacement(new CSUtility.Component.StandardPlacement(m_SkeletonActor));
            //mWorld.AddEditorActor(m_SkeletonActor);

            // 界面
            mWinRoot.Width = (int)m_nWidth;
            mWinRoot.Height = (int)m_nHeight;
            //mWinRoot.Visible = false;
            mWinRoot.Visibility = UISystem.Visibility.Collapsed;
            mForm = new UISystem.WinForm();
            //mForm.Left = 0;
            //mForm.Top = 0;
            mForm.Width = (int)m_nWidth;
            mForm.Height = (int)m_nHeight;
            mForm.Margin = new CSUtility.Support.Thickness(0, 0, 0, 0);
            mForm.Parent = mWinRoot;
            mForm.ForeColor = CSUtility.Support.Color.White;
            mForm.BackColor = CSUtility.Support.Color.FromArgb(0, 0, 0, 0);  //System.Drawing.Color.Gray;
            //mForm.Visible = false;
            mForm.Visibility = UISystem.Visibility.Collapsed;
            //mForm.DockMode = System.Windows.Forms.DockStyle.None;
            mForm.FixSizeByUVAnim = false;
            mForm.RState.UVAnim = UISystem.UVAnimMgr.Instance.Find(Guid.Parse("8a267cd1-f4c3-473e-8de9-24a8986057bf"), true);

            // CameraController
            mCameraController = new CCore.Camera.CameraController();//new ITraceCameraController(m_MeshActor);
            //m_CameraController = new ICameraController();
            mCameraController.Camera = mREnviroment.Camera;
            mCameraController.Enable = true;

            SlimDX.Vector3 pos = new SlimDX.Vector3(-10, 10, -10);
            SlimDX.Vector3 lookAt = new SlimDX.Vector3(0, 0, 0);
            SlimDX.Vector3 up = new SlimDX.Vector3(0, 1, 0);
            mCameraController.SetPosLookAtUp(ref pos, ref lookAt, ref up);

            //SlimDX.Vector3 dir = new SlimDX.Vector3(2, -1, 1);
            //dir.Normalize();
            //((ITraceCameraController)m_CameraController).Initialize(ref dir, 20);
        }


        public void FinalCleanup()
        {
            if (mWorld != null)
            {
                mWorld.Cleanup();
                mWorld = null;
            }
            if (mREnviroment != null)
            {
                mREnviroment.Cleanup();
                mREnviroment = null;
            }
            scInstance = null;
        }

        //public void SetSkeleton(string strAnimTreeFileName, string strActionFileName)
        //{
        //    if (string.IsNullOrEmpty(strAnimTreeFileName) || (string.IsNullOrEmpty(strActionFileName)))
        //    {
        //        return;
        //    }

        //    m_SkeletonActor.Visual.Visible = true;
        //    //m_World.RemoveAllCommActor();
        //    //m_World.AddCommActor(m_SkeletonActor);

        //    //m_ActionNode.Initialize("Action/Test_ActionNode.vat");
        //    m_ActionNode.SetAction( strActionFileName );

        //    m_ActionNode.Update(m_ActionNode.Duration/2);

        //    ISkeleton ske = null;
        //    m_ActionNode.GetSkeleton(ref ske);
        //    if (ske == null)
        //        return;
        //    ske.CalcBoundingBox();
        //    m_SkeletonVisual.SetSkeleton(ske);
        //    m_SkeletonVisual.SetMeshScale(0.02f, 0.02f, 0.02f);
        //    m_SkeletonVisual.MakeSkeletonRenderData(ISkeletonVisual.SkeletonPose.SP_Now);
        //    m_SkeletonVisual.PreUse(true, 0);

        //    SlimDX.Vector3 pos = new SlimDX.Vector3(100, 50, 0);
        //    var lookat = (ske.vMax - ske.vMin) * 0.5f + ske.vMin;
        //    mCameraController.SetPosLookAtUp(ref pos, ref lookat, ref SlimDX.Vector3.UnitY);
        //    Program.MaxZoomMeshShow(0, 0, mREnviroment.View.Width, mREnviroment.View.Height, ske.vMax, ske.vMin, mCameraController, 1.1);
        //}

        //public void SetRoleTemplate(CSUtility.Data.RoleTemplateBase roleTemplate)
        //{
        //    mVisual = new FrameSet.Role.RoleActorVisual();
        //    m_MeshActor.Visual = mVisual;

        //    if (roleTemplate == null)
        //    {
        //        return;
        //    }

        //    var visInit = new FrameSet.Role.RoleActorVisualInit();
        //    visInit.MeshTemplateIds.Clear();
        //    visInit.MeshTemplateIds.AddRange(roleTemplate.DefaultMeshs);

        //    mVisual.Initialize(visInit, null);

        //    ((FrameSet.Role.RoleActorVisual)mVisual).Preuse(true, 0);
        //    ((FrameSet.Role.RoleActorVisual)mVisual).Update(10);

        //    SlimDX.Vector3 pos = new SlimDX.Vector3(-100, 100, -100);
        //    mCameraController.SetPosLookAtUp(ref pos, ref SlimDX.Vector3.Zero, ref SlimDX.Vector3.UnitY);
        //    SlimDX.Vector3 vMax = SlimDX.Vector3.UnitXYZ;
        //    SlimDX.Vector3 vMin = SlimDX.Vector3.UnitXYZ;
        //    mVisual.GetAABB(ref vMin, ref vMax);
        //    Program.MaxZoomMeshShow(0, 0, mREnviroment.View.Width, mREnviroment.View.Height, vMax, vMin, mCameraController, 1.5);
        //}

        //public void SetMesh(string strFileName)
        //{
        //    mVisual = new CCore.Mesh.Mesh();
        //    m_MeshActor.Visual = mVisual;

        //    if (string.IsNullOrEmpty(strFileName))
        //    {
        //        return;
        //    }

        //    //m_World.RemoveAllCommActor();
        //    //m_World.AddCommActor(m_MeshActor);

        //    var mshInit = new CCore.Mesh.MeshInit();
        //    CCore.Mesh.MeshInitPart mshInitPart = new CCore.Mesh.MeshInitPart();
        //    mshInitPart.MeshName = strFileName;
        //    mshInit.MeshInitParts.Add(mshInitPart);
        //    mshInit.CanHitProxy = false;
        //    mVisual.Initialize(mshInit, null);
        //    IMaterial mtl = IEngine.Instance.Client.Graphics.MaterialMgr.GetDefaultMaterial();//IEngine.Instance.Client.Graphics.MaterialMgr.LoadMaterial(new IMaterialParameter("Tex2DMaterial.mtl", "Tech0"));
        //    //mtl.PreUse();
        //    for (int i = 0; i < ((CCore.Mesh.Mesh)mVisual).GetMaxMaterial(0); ++i)
        //    {
        //        ((CCore.Mesh.Mesh)mVisual).SetMaterial(0, i, mtl);
        //    }
        //    ((CCore.Mesh.Mesh)mVisual).Preuse(true, 0);
        //    ((CCore.Mesh.Mesh)mVisual).Update(10);

        //    SlimDX.Vector3 pos = new SlimDX.Vector3(-100, 100, -100);
        //    mCameraController.SetPosLookAtUp(ref pos, ref SlimDX.Vector3.Zero, ref SlimDX.Vector3.UnitY);
        //    SlimDX.Vector3 vMax = SlimDX.Vector3.UnitXYZ;
        //    SlimDX.Vector3 vMin = SlimDX.Vector3.UnitXYZ;
        //    mVisual.GetAABB(ref vMin, ref vMax);
        //    Program.MaxZoomMeshShow(0, 0, mREnviroment.View.Width, mREnviroment.View.Height, vMax, vMin, mCameraController, 1);
        //}

        //public void SetMesh(CCore.Mesh.MeshTemplate mt)
        //{
        //    mVisual = new CCore.Mesh.Mesh();
        //    m_MeshActor.Visual = mVisual;

        //    if (mt == null)
        //    {
        //        return;
        //    }

        //    //m_World.RemoveAllCommActor();
        //    //m_World.AddCommActor(m_MeshActor);

        //    //for (int i = 0; i < mt.Materials.Count; i++ )
        //    //{
        //    //    var mtl = IEngine.Instance.Client.Graphics.MaterialMgr.LoadMaterial(new IMaterialParameter(mt.Materials[i], mt.Techniques[i]));
        //    //    mtl.PreUse();
        //    //}

        //    var mshInit = new CCore.Mesh.MeshInit()
        //    {
        //        MeshTemplateID = mt.MeshID,
        //        CanHitProxy = false
        //    };
        //    mVisual.Initialize(mshInit, null);
        //    ((CCore.Mesh.Mesh)mVisual).Preuse(true, IEngine.Instance.GetFrameMillisecond());
        //    ((CCore.Mesh.Mesh)mVisual).Update(10);

        //    SlimDX.Vector3 pos = new SlimDX.Vector3(-100, 100, -100);
        //    mCameraController.SetPosLookAtUp(ref pos, ref SlimDX.Vector3.Zero, ref SlimDX.Vector3.UnitY);
        //    SlimDX.Vector3 vMax = SlimDX.Vector3.UnitXYZ;
        //    SlimDX.Vector3 vMin = SlimDX.Vector3.UnitXYZ;
        //    mVisual.GetAABB(ref vMin, ref vMax);
        //    Program.MaxZoomMeshShow(0, 0, mREnviroment.View.Width, mREnviroment.View.Height, vMax, vMin, mCameraController, 1);
        //}

        //public void SetEffect(CCore.Effect.EffectTemplate eft)
        //{
        //    mVisual = new CCore.Component.EffectVisual();
        //    m_MeshActor.Visual = mVisual;

        //    if (eft == null)
        //        return;

        //    var visInit = new FrameSet.Effect.EffectVisualInit()
        //    {
        //        EffectTemplateID = eft.Id,
        //        CanHitProxy = false
        //    };
        //    mVisual.Initialize(visInit, null);
        //    mVisual.PreUse(true, (UInt64)(IEngine.Instance.GetFrameMillisecond()));

        //    SlimDX.Vector3 pos = new SlimDX.Vector3(-100, -100, -100);
        //    mCameraController.SetPosLookAtUp(ref pos, ref SlimDX.Vector3.Zero, ref SlimDX.Vector3.UnitY);
        //    SlimDX.Vector3 vMax = SlimDX.Vector3.UnitXYZ;
        //    SlimDX.Vector3 vMin = SlimDX.Vector3.UnitXYZ;
        //    mVisual.GetAABB(ref vMin, ref vMax);
        //    Program.MaxZoomMeshShow(0, 0, mREnviroment.View.Width, mREnviroment.View.Height, vMax, vMin, mCameraController, 2);
        //}

        //IActor[] mPrefabActors = null;
        //public void SetPrefab(CCore.World.Prefab.PrefabResource res)
        //{
        //    mVisual = null;
        //    m_MeshActor.Visual = null;

        //    if (mPrefabActors != null)
        //    {
        //        foreach (var actor in mPrefabActors)
        //        {
        //            mWorld.RemoveActor(actor);
        //        }
        //    }

        //    SlimDX.Vector3 pos = new SlimDX.Vector3(-100, -100, -100);
        //    mCameraController.SetPosLookAtUp(ref pos, ref SlimDX.Vector3.Zero, ref SlimDX.Vector3.UnitY);
        //    SlimDX.Vector3 vMax = SlimDX.Vector3.UnitXYZ;
        //    SlimDX.Vector3 vMin = SlimDX.Vector3.UnitXYZ;

        //    mPrefabActors = res.Actors;
        //    foreach (var actor in mPrefabActors)
        //    {
        //        SlimDX.Vector3 tempMax = SlimDX.Vector3.UnitXYZ, tempMin = SlimDX.Vector3.UnitXYZ;
        //        actor.GetAABB(ref tempMin, ref tempMax);

        //        if (vMax.X < tempMax.X)
        //            vMax.X = tempMax.X;
        //        if (vMax.Y < tempMax.Y)
        //            vMax.Y = tempMax.Y;
        //        if (vMax.Z < tempMax.Z)
        //            vMax.Z = tempMax.Z;

        //        if (vMin.X > tempMin.X)
        //            vMin.X = tempMin.X;
        //        if (vMin.Y > tempMin.Y)
        //            vMin.Y = tempMin.Y;
        //        if (vMin.Z > tempMin.Z)
        //            vMin.Z = tempMin.Z;

        //        mWorld.AddActor(actor);
        //    }

        //    Program.MaxZoomMeshShow(0, 0, mREnviroment.View.Width, mREnviroment.View.Height, vMax, vMin, mCameraController, 2);
        //}

        public double CameraDelta = 1.3f;
        // 计算摄像机位置,delta影响放大比率
        public void CalculateCamera(double delta)
        {
            SlimDX.Vector3 vMax = SlimDX.Vector3.UnitXYZ * float.MinValue;
            SlimDX.Vector3 vMin = SlimDX.Vector3.UnitXYZ * float.MaxValue;
            foreach(var actor in mWorld.GetActors((UInt16)CSUtility.Component.EActorGameType.Common))
            {
                SlimDX.Vector3 tempMax = SlimDX.Vector3.Zero, tempMin = SlimDX.Vector3.Zero;
                actor.GetAABB(ref tempMin, ref tempMax);
                vMax.X = System.Math.Max(vMax.X, tempMax.X);
                vMax.Y = System.Math.Max(vMax.Y, tempMax.Y);
                vMax.Z = System.Math.Max(vMax.Z, tempMax.Z);
                vMin.X = System.Math.Min(vMin.X, tempMin.X);
                vMin.Y = System.Math.Min(vMin.Y, tempMin.Y);
                vMin.Z = System.Math.Min(vMin.Z, tempMin.Z);
            }

            EditorCommon.Program.MaxZoomMeshShow(0, 0, mREnviroment.View.Width, mREnviroment.View.Height, vMax, vMin, mCameraController, delta);
        }

        //public void SetMaterial(CCore.Material.Material mtl)
        //{
        //    if (mVisual == null || !(mVisual is CCore.Mesh.Mesh))
        //        return;

        //    mtl.PreUse(true, IEngine.Instance.GetFrameMillisecond());

        //    for (int mC = 0; mC < ((CCore.Mesh.Mesh)mVisual).MethPartsCount; ++mC)
        //    {
        //        for (int i = 0; i < ((CCore.Mesh.Mesh)mVisual).GetMaxMaterial(0); ++i)
        //        {
        //            ((CCore.Mesh.Mesh)mVisual).SetMaterial(mC, i, mtl);
        //        }
        //    }
        //}

        public void SetUVAnim(Guid uvAnimId)
        {
            if (uvAnimId == Guid.Empty)
            {
                //mForm.Visible = false;
                mForm.Visibility = UISystem.Visibility.Collapsed;
                return;
            }

            //strFileName = strFileName.Remove(strFileName.LastIndexOf('.'));
            var uvAnim = UISystem.UVAnimMgr.Instance.Find(uvAnimId, true);
            //UISystem.UVAnim uvAnim = new UISystem.UVAnim();
            //CSUtility.Support.IConfigurator.FillProperty(uvAnim, strFileName);
            //uvAnim.MaterialObject.PreUse();
            //uvAnim.TextureObject.PreUse(true, 0);
   
            mForm.RState.UVAnim = uvAnim;
            if(mForm.RState.Texture != null)
                mForm.RState.Texture.PreUse(true, CCore.Engine.Instance.GetFrameMillisecond());
            if(mForm.RState.Material != null)
                mForm.RState.Material.PreUse(true, CCore.Engine.Instance.GetFrameMillisecond());
            //mForm.Visible = true;
            mForm.Visibility = UISystem.Visibility.Visible;
            //mWinRoot.Visible = true;
            mWinRoot.Visibility = UISystem.Visibility.Visible;
        }

        UISystem.WinForm mPreForm;
        public void SetForm(UISystem.WinForm form)
        {
            if (form == null)
                return;
            mPreForm = mForm;

            mForm = form;
            mForm.Parent = mWinRoot;

            int length = mForm.Width >= mForm.Height ? mForm.Width : mForm.Height;
            mWinRoot.Width = mWinRoot.Height = length;            

            mForm.Visibility = UISystem.Visibility.Visible;            
            mWinRoot.Visibility = UISystem.Visibility.Visible;
        }

        public void ReStoreForm()
        {
            if (mPreForm == null)
                return;
            mForm = mPreForm;
            mForm.Parent = mWinRoot;

            mWinRoot.Width = mForm.Width = (int)m_nWidth;
            mWinRoot.Height = mForm.Height = (int)m_nHeight;
        }

        UISystem.UIRenderPipe mNullUIPipe = new UISystem.UIRenderPipe();
        private void BeforeCopyTexture(CCore.Graphics.REnviroment env)
        {
            //{
            //    //if (mWinRoot.Visible)
            //    if (mWinRoot.Visibility == UISystem.Visibility.Visible)
            //    {
            //        SlimDX.Matrix transMat = SlimDX.Matrix.Identity;
            //        mWinRoot.Draw(NullUIPipe, 0, ref transMat);
            //        //UISystem.IRender.GetInstance().DrawLine(new System.Drawing.Point(0, 0), new System.Drawing.Point(128,128), System.Drawing.Color.FromArgb(255, 255, 0, 0));
            //    }
            //}
            UISystem.IRender.GetInstance().UIRenderer.CommitDrawCall(mNullUIPipe);
        }
        // 保存FinalTexture的话， 不能使用AfterDraw，因为此代理是在FinalTexture翻转到屏幕上之后做的 
        //private void AfterRender2View(CCore.Graphics.REnviroment env)
        //{
        //    UISystem.IRender.GetInstance().UIRenderer.CommitDrawCall(mNullUIPipe);
        //}

        public string SourceType;
        public Int64 ElapsedTime
        {
            get { return 50; }
        }

        public void SaveToFile(string strFileName, CCore.enD3DXIMAGE_FILEFORMAT fileFormat, int tickTimes = 2)
        {
            //switch (SourceType)
            //{
            //    case "UVAnim":
            //        {
            //            SlimDX.Vector3 vScale = new SlimDX.Vector3(0.0001f);
            //            m_MeshActor.Placement.SetScale(ref vScale);
            //        }
            //        break;

            //    default:
            //        {
            //            SlimDX.Vector3 vScale = new SlimDX.Vector3(1.0f);
            //            m_MeshActor.Placement.SetScale(ref vScale);
            //        }
            //        break;
            //}

            CalculateCamera(CameraDelta);

            for (int i = 0; i < tickTimes; ++i)
            {
                CCore.Engine.Instance.Tick(true);

                //if (mVisual != null)
                //{
                //    if (mVisual is CCore.Mesh.Mesh)
                //    {
                //        //m_Mesh.Preuse(true, IEngine.Instance.GetFrameMillisecond());
                //        //if (mVisual is CCore.Mesh.Mesh)
                //        ((CCore.Mesh.Mesh)mVisual).Update(ElapsedTime);
                //        //else if (mVisual is FrameSet.Role.RoleActorVisual)
                //        //    ((FrameSet.Role.RoleActorVisual)mVisual).Update(10);
                //    }
                //    else if (mVisual is CCore.Component.EffectVisual)
                //    {
                //        ((CCore.Component.EffectVisual)mVisual).Tick(mVisual.HostActor, ElapsedTime);// m_MeshActor, ElapsedTime);
                //    }
                //}

                mCameraController.Tick();

                mWorld.Tick();
                mWorld.Render2Enviroment(mRenderparam);
                mREnviroment.RefreshPostProcess(mWorld.PostProceses);

                mREnviroment.Tick();
                //UISystem.IRender.GetInstance().UIRenderer.ClearAllCommit(mNullUIPipe);
                mWinRoot.Tick(CCore.Engine.Instance.GetFrameMillisecond());
                var transMat = SlimDX.Matrix.Identity;
                mWinRoot.Draw(mNullUIPipe, 0, ref transMat);


                CCore.Engine.Instance.Client.Graphics.BeginDraw();
                mREnviroment.Render();
                CCore.Engine.Instance.Client.Graphics.EndDraw();
                mREnviroment.SwapPipe();

                UISystem.IRender.GetInstance().UIRenderer.SwapQueue(mNullUIPipe);
                mREnviroment.ClearAllDrawingCommits();

                System.Threading.Thread.Sleep(10);
            }

            m_bRenderingSnapshots = false;

            mREnviroment.Save2File(strFileName, fileFormat);

            //m_SkeletonActor.Visual.Visible = false;

            //if (mPrefabActors != null)
            //{
            //    foreach (var actor in mPrefabActors)
            //    {
            //        mWorld.RemoveActor(actor);
            //    }
            //}
        }
    }
}
