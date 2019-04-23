/// <summary>
/// 工具支持的命名空间
/// </summary>
namespace CCore.Support
{
    /// <summary>
    /// 抓取世界对象的初始化类
    /// </summary>
    public struct CaptureWorldInit
    {
        /// <summary>
        /// 地图方向
        /// </summary>
        public string MapDir;
    }
    /// <summary>
    /// 抓取时间对象
    /// </summary>
    public class CaptureWorld
    {
        private CCore.World.WorldRenderParam m_RenderParam;
        private CCore.World.WorldRenderParam m_RenderParam2;
        private CCore.World.WorldRenderParam m_RenderParam3;
        private CCore.World.WorldRenderParam m_RenderParam4;
        private CCore.World.WorldRenderParam m_RenderParam5;
        CCore.World.World mWorld = null;

        //CCore.Camera.Camera.CameraController mEquipUICameraController;
        //public CCore.Camera.Camera.CameraController EquipUICameraController
        //{
        //    get { return mEquipUICameraController; }
        //}

        //CCore.Camera.Camera.CameraController mEquipUICameraController2;
        //CCore.Camera.Camera.CameraController mEquipUICameraController3;
        //CCore.Camera.Camera.CameraController mEquipUICameraController4;
        //CCore.Camera.Camera.CameraController mEquipUICameraController5;
        /// <summary>
        /// 构造函数，创建渲染参数
        /// </summary>
        public CaptureWorld()
        {
            m_RenderParam = new  CCore.World.WorldRenderParam();
            m_RenderParam2 = new CCore.World.WorldRenderParam();
            m_RenderParam3 = new CCore.World.WorldRenderParam();
            m_RenderParam4 = new CCore.World.WorldRenderParam();
            m_RenderParam5 = new CCore.World.WorldRenderParam();
        }
        /// <summary>
        /// 析构函数，删除对象，释放指针
        /// </summary>
        ~CaptureWorld()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除世界对象，释放其指针
        /// </summary>
        public void Cleanup()
        {
            mWorld.Cleanup();
            if (m_RenderParam.Enviroment != null)
                m_RenderParam.Enviroment.Cleanup();
            if (m_RenderParam2.Enviroment != null)
                m_RenderParam2.Enviroment.Cleanup();
            if (m_RenderParam3.Enviroment != null)
                m_RenderParam3.Enviroment.Cleanup();
            if (m_RenderParam4.Enviroment != null)
                m_RenderParam4.Enviroment.Cleanup();
            if (m_RenderParam5.Enviroment != null)
                m_RenderParam5.Enviroment.Cleanup();
        }
        /// <summary>
        /// 玩家初始化旋转四元数
        /// </summary>
        public SlimDX.Quaternion mPlayerInitRotation = SlimDX.Quaternion.Identity;
        /// <summary>
        /// 保存玩家初始化四元数
        /// </summary>
        public SlimDX.Quaternion mStorePlayerInitRotation = SlimDX.Quaternion.Identity;
        /// <summary>
        /// NPC的初始化位置坐标
        /// </summary>
        public SlimDX.Vector3 mNpcInitPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 守护的初始化位置坐标
        /// </summary>
        public SlimDX.Vector3 mGuardInitPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 守护的初始化位置坐标
        /// </summary>
        public SlimDX.Vector3 mViceGuardInitPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="captureWorldInit">抓取世界对象的初始化对象</param>
        public void Initialize(CaptureWorldInit captureWorldInit)
        {
            /*mWorld = new CCore.World.World(Guid.NewGuid());
            mWorld.LoadWorld(captureWorldInit.MapDir);
            mWorld.ActorOneFrameOneRender = false;      // 允许一帧渲染多次Actor，以支持多个摄像机渲染多次场景

            // EquipUI Initialize
            var _reInit = new CCore.Graphics.REnviromentInit();
            _reInit.ZFar = 1000;
            _reInit.Width = 238;
            _reInit.Height = 353;
            _reInit.bUseRT = true;
            m_RenderParam.Enviroment = new CCore.Graphics.REnviroment();
            if (false == m_RenderParam.Enviroment.Initialize(_reInit))
                return;
            m_RenderParam.Enviroment.SetClearColorMRT(CSUtility.Support.Color.FromArgb(0, CSUtility.Support.Color.Black));

            var playerPos = SlimDX.Vector3.Zero;
            var pointGroup = CSUtility.Map.ScenePointGroupManager.Instance.FindGroup(mWorld.WorldInit.WorldId, CSUtility.Support.IFileConfig.EquipUIPointGroupId);
            if(pointGroup!=null)
            {
                var lookAt = pointGroup.Points[0].GetPosition();
                var eyePos = pointGroup.Points[1].GetPosition();
                playerPos = pointGroup.Points[2].GetPosition();
                mPlayerInitRotation = pointGroup.Points[2].GetRotation();
                //viewDir = lookAt - eyePos;
                mEquipUICameraController = new CCore.Camera.Camera.CameraController();
                mEquipUICameraController.Camera = m_RenderParam.Enviroment.Camera;
                mEquipUICameraController.Enable = true;
                mEquipUICameraController.SetPosLookAtUp(ref eyePos, ref lookAt, ref SlimDX.Vector3.UnitY);
            }

            var role = FrameSet.Role.RoleActor.mBakedChiefRoleActor;
            var actorInit = new CCore.World.ActorInit();
            actorInit.ActorFlag = CSUtility.Component.ActorInitBase.EActorFlag.WithOutSceneManager;
            actorInit.GameType = (UInt16)CSUtility.Component.EActorGameType.Common;
            role.Initialize(actorInit);
            role.mUpdateAnimByDistance = false;
            if (role.Placement == null)
                role.SetPlacement(new CSUtility.Component.IStandardPlacement(role));
            role.Placement.SetLocation(ref playerPos);
            role.Placement.SetRotation(ref mPlayerInitRotation);
            mWorld.AddActor(role);

            // 2UI Initialize
            m_RenderParam2.Enviroment = new IREnviroment();
            _reInit.Width = 192;
            _reInit.Height = 256;
            if (false == m_RenderParam2.Enviroment.Initialize(_reInit))
                return;
            m_RenderParam2.Enviroment.SetClearColorMRT(CSUtility.Support.Color.FromArgb(0, CSUtility.Support.Color.Black));

            var pointGroup2 = CSCommon.ServerMap.ScenePointGroupManager.Instance.FindGroup(mWorld.SceneInit.Id, CSUtility.Support.IFileConfig.NpcIPointGroupId);
            if (pointGroup2 != null)
            {
                var lookAt = pointGroup2.Points[0].GetPosition();
                var eyePos = pointGroup2.Points[1].GetPosition();
                mNpcInitPos = pointGroup2.Points[2].GetPosition();
                mEquipUICameraController2 = new ICameraController();
                mEquipUICameraController2.Camera = m_RenderParam2.Enviroment.Camera;
                mEquipUICameraController2.Enable = true;
                mEquipUICameraController2.SetPosLookAtUp(ref eyePos, ref lookAt, ref SlimDX.Vector3.UnitY);
            }


            // 3StoreUI Initialize
            m_RenderParam3.Enviroment = new IREnviroment();
            _reInit.Width = 190;
            _reInit.Height = 270;
            _reInit.bUseRT = true;
            m_RenderParam3.Enviroment = new IREnviroment();
            if (false == m_RenderParam3.Enviroment.Initialize(_reInit))
                return;
            m_RenderParam3.Enviroment.SetClearColorMRT(CSUtility.Support.Color.FromArgb(0, CSUtility.Support.Color.Black));

            var StplayerPos = SlimDX.Vector3.Zero;
            var pointGroup3 = CSCommon.ServerMap.ScenePointGroupManager.Instance.FindGroup(mWorld.SceneInit.Id, CSUtility.Support.IFileConfig.StoreUIPointGroupId);
            if (pointGroup3 != null)
            {
                var lookAt = pointGroup3.Points[0].GetPosition();
                var eyePos = pointGroup3.Points[1].GetPosition();
                StplayerPos = pointGroup3.Points[2].GetPosition();
                mStorePlayerInitRotation = pointGroup3.Points[2].GetRotation();
                //viewDir = lookAt - eyePos;
                mEquipUICameraController3 = new ICameraController();
                mEquipUICameraController3.Camera = m_RenderParam3.Enviroment.Camera;
                mEquipUICameraController3.Enable = true;
                mEquipUICameraController3.SetPosLookAtUp(ref eyePos, ref lookAt, ref SlimDX.Vector3.UnitY);
            }

            var srole = FrameSet.Role.RoleActor.mStoreChiefRoleActor;
            IActorInit sactorInit = new IActorInit();
            sactorInit.ActorFlag = CSCommon.Component.IActorInitBase.EActorFlag.WithOutSceneManager;
            sactorInit.GameType = CSCommon.Component.EActorGameType.Common;
            srole.Initialize(sactorInit);
            srole.mUpdateAnimByDistance = false;
            if (srole.Placement == null)
                srole.SetPlacement(new CSCommon.Component.IStandPlacement(srole));
            srole.Placement.SetLocation(ref StplayerPos);
            srole.Placement.SetRotation(ref mStorePlayerInitRotation);
            mWorld.AddActor(srole);

            // 4UI Initialize
            m_RenderParam4.Enviroment = new IREnviroment();
            _reInit.Width = 144;
            _reInit.Height = 175;
            if (false == m_RenderParam4.Enviroment.Initialize(_reInit))
                return;
            m_RenderParam4.Enviroment.SetClearColorMRT(CSUtility.Support.Color.FromArgb(0, CSUtility.Support.Color.Black));

            var pointGroup4 = CSCommon.ServerMap.ScenePointGroupManager.Instance.FindGroup(mWorld.SceneInit.Id, CSUtility.Support.IFileConfig.GuardIPointGroupId);
            if (pointGroup4 != null)
            {
                var lookAt = pointGroup4.Points[0].GetPosition();
                var eyePos = pointGroup4.Points[1].GetPosition();
                mGuardInitPos = pointGroup4.Points[2].GetPosition();
                mEquipUICameraController4 = new ICameraController();
                mEquipUICameraController4.Camera = m_RenderParam4.Enviroment.Camera;
                mEquipUICameraController4.Enable = true;
                mEquipUICameraController4.SetPosLookAtUp(ref eyePos, ref lookAt, ref SlimDX.Vector3.UnitY);
            }

            // 4UI Initialize
            m_RenderParam5.Enviroment = new IREnviroment();
            _reInit.Width = 144;
            _reInit.Height = 175;
            if (false == m_RenderParam5.Enviroment.Initialize(_reInit))
                return;
            m_RenderParam5.Enviroment.SetClearColorMRT(CSUtility.Support.Color.FromArgb(0, CSUtility.Support.Color.Black));

            var pointGroup5 = CSCommon.ServerMap.ScenePointGroupManager.Instance.FindGroup(mWorld.SceneInit.Id, CSUtility.Support.IFileConfig.ViceGuardIPointGroupId);
            if (pointGroup5 != null)
            {
                var lookAt = pointGroup5.Points[0].GetPosition();
                var eyePos = pointGroup5.Points[1].GetPosition();
                mViceGuardInitPos = pointGroup5.Points[2].GetPosition();
                mEquipUICameraController5 = new ICameraController();
                mEquipUICameraController5.Camera = m_RenderParam5.Enviroment.Camera;
                mEquipUICameraController5.Enable = true;
                mEquipUICameraController5.SetPosLookAtUp(ref eyePos, ref lookAt, ref SlimDX.Vector3.UnitY);
            }*/
        }

        /*//FrameSet.Role.RoleActor mBakedNpcActor = null;
        CCore.World.Actor mBakedNpcActor = null;
        public void SetNpcActor(FrameSet.Role.RoleActor srcNpc)
        {
            if (mBakedNpcActor != null)
                mWorld.RemoveActor(mBakedNpcActor);

            mBakedNpcActor = new ICommActor();
            IActorInit npcActorInit = new IActorInit();
            npcActorInit.ActorFlag = CSCommon.Component.IActorInitBase.EActorFlag.WithOutSceneManager;
            npcActorInit.GameType = CSCommon.Component.EActorGameType.Common;
            mBakedNpcActor.Initialize(npcActorInit);

            mBakedNpcActor.mUpdateAnimByDistance = false;
            var npcVisual = new FrameSet.Role.RoleActorVisual();
            var srcNpcVisual = srcNpc.Visual as FrameSet.Role.RoleActorVisual;
            if (srcNpcVisual == null)
                return;
            if (srcNpc.RoleData.RoleType ==FrameSet.Role.EClientRoleType.ChiefPlayer)
            {
                srcNpcVisual.RoleActorInit.MeshTemplateIds.Add(srcNpc.RoleTemplate.HairId);
            }
            npcVisual.Initialize(srcNpcVisual.RoleActorInit, mBakedNpcActor);
            srcNpc.FSMSetAction(npcVisual, "Idle", true, 1.0f, 100);
            mBakedNpcActor.Visual = npcVisual;

            if (mBakedNpcActor.Placement == null)
                mBakedNpcActor.SetPlacement(new CSUtility.Component.IStandardPlacement(mBakedNpcActor));
            mBakedNpcActor.Placement.SetLocation(ref mNpcInitPos);
            mBakedNpcActor.Placement.SetRotationY((float)System.Math.PI / 2f, srcNpc.RoleTemplate.MeshFixAngle);
            mWorld.AddActor(mBakedNpcActor);
        }

        CCore.World.Actor mBakedGuardActor = null;
        Role.RoleActor mBackGuardRole = null;
        public void SetGuardActor(UInt16 tempId)
        {
            if (mBakedGuardActor != null)
            {
                mWorld.RemoveActor(mBakedGuardActor);
            }
            if (mBackGuardRole != null && mBackGuardRole.RoleData.NpcData.TemplateId == tempId)
            {

            }
            else
            {
                var itemTpl = CSCommon.Data.Item.ItemTemplateManager.Instance.FindItem(tempId);
                foreach (var mt in itemTpl.MeshTemplateIds)
                {
                    if (mt.Race == null)
                        break;
                    if (mt.Race.CompareTo("All") == 0)
                    {
                        mBackGuardRole = FrameSet.Role.RoleActor.CreateClientNpc(mt.RoleId);
                    }
                }
            }
            if (mBackGuardRole == null)
                return;
            mBakedGuardActor = new ICommActor();
            IActorInit guardActorInit = new IActorInit();
            guardActorInit.ActorFlag = CSCommon.Component.IActorInitBase.EActorFlag.WithOutSceneManager;
            guardActorInit.GameType = CSCommon.Component.EActorGameType.Common;
            mBakedGuardActor.Initialize(guardActorInit);

            mBakedGuardActor.mUpdateAnimByDistance = false;
            var guardVisual = new FrameSet.Role.RoleActorVisual();
            var srcGuardVisual = mBackGuardRole.Visual as FrameSet.Role.RoleActorVisual;
            if (srcGuardVisual == null)
                return;

            guardVisual.Initialize(srcGuardVisual.RoleActorInit, mBakedGuardActor);
            mBackGuardRole.FSMSetAction(srcGuardVisual, "Idle", true, 1.0f, 100);
            mBakedGuardActor.Visual = guardVisual;

            if (mBakedGuardActor.Placement == null)
                mBakedGuardActor.SetPlacement(new CSCommon.Component.IStandPlacement(mBakedGuardActor));
            var rolepos = new SlimDX.Vector3(mGuardInitPos.X,mGuardInitPos.Y+mBackGuardRole.RoleTemplate.CameraPointHeight ,mGuardInitPos.Z);
            mBakedGuardActor.Placement.SetLocation(ref rolepos);
            SlimDX.Vector3 scale = new SlimDX.Vector3(mBackGuardRole.RoleTemplate.ShieldScale);
            mBakedGuardActor.Placement.SetScale(ref scale);
            mWorld.AddActor(mBakedGuardActor);
        }

        ICommActor mViceBakedGuardActor = null;
        Role.RoleActor mViceBackGuardRole = null;
        public void SetViceGuardActor(UInt16 tempId)
        {
            if (mViceBakedGuardActor != null)
            {
                mWorld.RemoveActor(mViceBakedGuardActor);
            }
            if (mViceBackGuardRole != null && mViceBackGuardRole.RoleData.NpcData.TemplateId == tempId)
            {

            }
            else
            {
                var itemTpl = CSCommon.Data.Item.ItemTemplateManager.Instance.FindItem(tempId);
                foreach (var mt in itemTpl.MeshTemplateIds)
                {
                    if (mt.Race == null)
                        break;
                    if (mt.Race.CompareTo("All") == 0)
                    {
                        mViceBackGuardRole = FrameSet.Role.RoleActor.CreateClientNpc(mt.RoleId);
                    }
                }
            }
            if (mViceBackGuardRole == null)
                return;
            mViceBakedGuardActor = new ICommActor();
            IActorInit guardActorInit = new IActorInit();
            guardActorInit.ActorFlag = CSCommon.Component.IActorInitBase.EActorFlag.WithOutSceneManager;
            guardActorInit.GameType = CSCommon.Component.EActorGameType.Common;
            mViceBakedGuardActor.Initialize(guardActorInit);

            mViceBakedGuardActor.mUpdateAnimByDistance = false;
            var guardVisual = new FrameSet.Role.RoleActorVisual();
            var srcGuardVisual = mViceBackGuardRole.Visual as FrameSet.Role.RoleActorVisual;
            if (srcGuardVisual == null)
                return;

            guardVisual.Initialize(srcGuardVisual.RoleActorInit, mViceBakedGuardActor);
            mViceBackGuardRole.FSMSetAction(srcGuardVisual, "Idle", true, 1.0f, 100);
            mViceBakedGuardActor.Visual = guardVisual;

            if (mViceBakedGuardActor.Placement == null)
                mViceBakedGuardActor.SetPlacement(new CSCommon.Component.IStandPlacement(mViceBakedGuardActor));
            var rolepos = new SlimDX.Vector3(mViceGuardInitPos.X, mViceGuardInitPos.Y + mViceBackGuardRole.RoleTemplate.CameraPointHeight, mViceGuardInitPos.Z);
            mViceBakedGuardActor.Placement.SetLocation(ref rolepos);
            SlimDX.Vector3 scale = new SlimDX.Vector3(mViceBackGuardRole.RoleTemplate.ShieldScale);
            mViceBakedGuardActor.Placement.SetScale(ref scale);
            mWorld.AddActor(mViceBakedGuardActor);
        }*/
        /// <summary>
        /// 装备的UI是否需要持续更新，默认为false
        /// </summary>
        public bool mEquipUINeedTick = false;
        /// <summary>
        /// 商店的UI是否需要持续更新，默认为false
        /// </summary>
        public bool mStoreUINeedTick = false;
        /// <summary>
        /// NPC的UI是否需要持续更新，默认为false
        /// </summary>
        public bool mNpcUINeedTick = false;
        /// <summary>
        /// 守护的UI是否需要持续更新，默认为false
        /// </summary>
        public bool mGuardUINeedTick = false;
        /// <summary>
        /// 守护的UI是否需要持续更新，默认为false
        /// </summary>
        public bool mViceGuardUINeedTick = false;
        int mInterval = 33;
        long mDeltaTime = 0;
        /// <summary>
        /// 每帧调用
        /// </summary>
        public void Tick()
        {
            if (mWorld == null || m_RenderParam.Enviroment == null)
                return;

            mDeltaTime += CCore.Engine.Instance.GetElapsedMillisecond();
            if(mDeltaTime > mInterval)
            {
                mWorld.TravelTo(0, 0);
                mWorld.Tick();

                /*/ Camera 1 Capture
                if (mEquipUINeedTick == true)
                {
                    mWorld.Render2Enviroment(m_RenderParam);
                    m_RenderParam.Enviroment.RefreshPostProcess(mWorld.PostProceses);

                    m_RenderParam.Enviroment.Tick();
                    m_RenderParam.Enviroment.Render();
                    m_RenderParam.Enviroment.SwapPipe();

                    var finalTex = DllImportAPI.vDSRenderEnv_GetFinalTexture(m_RenderParam.Enviroment.DSRenderEnv);
                    DllImportAPI.v3dDevice_SetSceneCapture1(CCore.Engine.Instance.Client.Graphics.Device, finalTex);
                }

                // TODO：Camera 2 Capture
                if (mNpcUINeedTick == true)
                {
                    mWorld.Render2Enviroment(m_RenderParam2);
                    m_RenderParam2.Enviroment.RefreshPostProcess(mWorld.PostProceses);

                    m_RenderParam2.Enviroment.Tick();
                    m_RenderParam2.Enviroment.Render();
                    m_RenderParam2.Enviroment.SwapPipe();

                    var finalTex = DllImportAPI.vDSRenderEnv_GetFinalTexture(m_RenderParam2.Enviroment.DSRenderEnv);
                    DllImportAPI.v3dDevice_SetSceneCapture2(CCore.Engine.Instance.Client.Graphics.Device, finalTex);
                }

                // Camera 3 Capture
                if (mStoreUINeedTick == true)
                {
                    mWorld.Render2Enviroment(m_RenderParam3);
                    m_RenderParam3.Enviroment.RefreshPostProcess(mWorld.PostProceses);

                    m_RenderParam3.Enviroment.Tick();
                    m_RenderParam3.Enviroment.Render();
                    m_RenderParam3.Enviroment.SwapPipe();

                    var finalTex = MidLayer.IDllImportAPI.vDSRenderEnv_GetFinalTexture(m_RenderParam3.Enviroment.DSRenderEnv);
                    MidLayer.IDllImportAPI.v3dDevice_SetSceneCapture3(IEngine.Instance.Client.Graphics.Device, finalTex);
                }

                // Camera 4 Capture
                if (mGuardUINeedTick == true)
                {
//                     if (mBackGuardRole != null)
//                         mBackGuardRole.Tick(MidLayer.IEngine.Instance.GetElapsedMillisecond());
                    mWorld.Render2Enviroment(m_RenderParam4);
                    m_RenderParam4.Enviroment.RefreshPostProcess(mWorld.PostProceses);

                    m_RenderParam4.Enviroment.Tick();
                    m_RenderParam4.Enviroment.Render();
                    m_RenderParam4.Enviroment.SwapPipe();

                    var finalTex = MidLayer.IDllImportAPI.vDSRenderEnv_GetFinalTexture(m_RenderParam4.Enviroment.DSRenderEnv);
                    MidLayer.IDllImportAPI.v3dDevice_SetSceneCapture4(IEngine.Instance.Client.Graphics.Device, finalTex);
                    bool bSave = false;
                    if (bSave)
                    {
                        MidLayer.IDllImportAPI.vDSRenderEnv_SaveFinalTexture(m_RenderParam4.Enviroment.DSRenderEnv, "d:/guard.bmp");
                    }
                }

                // Camera 5 Capture
                if (mViceGuardUINeedTick == true)
                {
                    //                     if (mBackGuardRole != null)
                    //                         mBackGuardRole.Tick(MidLayer.IEngine.Instance.GetElapsedMillisecond());
                    mWorld.Render2Enviroment(m_RenderParam5);
                    m_RenderParam5.Enviroment.RefreshPostProcess(mWorld.PostProceses);

                    m_RenderParam5.Enviroment.Tick();
                    m_RenderParam5.Enviroment.Render();
                    m_RenderParam5.Enviroment.SwapPipe();

                    var finalTex = MidLayer.IDllImportAPI.vDSRenderEnv_GetFinalTexture(m_RenderParam5.Enviroment.DSRenderEnv);
                    MidLayer.IDllImportAPI.v3dDevice_SetSceneCapture5(IEngine.Instance.Client.Graphics.Device, finalTex);
                    bool bSave = false;
                    if (bSave)
                    {
                        MidLayer.IDllImportAPI.vDSRenderEnv_SaveFinalTexture(m_RenderParam5.Enviroment.DSRenderEnv, "d:/viceguard.bmp");
                    }
                }*/

                mDeltaTime = 0;
            }
        }

    }
}
