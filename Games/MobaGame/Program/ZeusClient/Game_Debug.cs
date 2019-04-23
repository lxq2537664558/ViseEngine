using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public partial class Game
    {
        Int64 PrevTickTime = 0;
        int FrameCount = 0;
        public float Fps;

        long mPrevGCTime = CCore.Engine.Instance._GetTickCount();

        bool mShowPing = false;
        public bool ShowPing
        {
            get { return mShowPing; }
            set { mShowPing = value; }
        }

        bool mShowPingOptions = true;
        public bool ShowPingOptions
        {
            get { return mShowPingOptions; }
            set { mShowPingOptions = value; }
        }

        int mDPCount = 0;
        int mTriCount = 0;
        int mClearCount = 0;
        static CSUtility.Performance.PerfCounter mDebugInfoTickTimer = new CSUtility.Performance.PerfCounter("IGame.DbgInfo");
        void UpdateDebugInfo()
        {
            mDebugInfoTickTimer.Begin();
            var manager = RecordVManager.Instance;

            var grid = RecordVManager.Instance.mRecordTitleStackPanel.Parent as UISystem.Container.Grid.Grid;
            if (grid!=null && grid.Visibility == UISystem.Visibility.Visible)
            {
                for (int i = 0; i < CCore.Engine.Instance.Client.Graphics.GetDebugTextureCount(); ++i)
                {
                    CCore.Graphics.Texture tex = null;
                    string name;
                    int w, h;
                    bool isGray;
                    CCore.Engine.Instance.Client.Graphics.GetDebugTextureParams(i, out tex, out name, out w, out h, out isGray);
                    RecordVManager.Instance.AddRecordTitle(name, tex, (ushort)w, (ushort)h, isGray);
                }
            }
            
            grid = RecordVManager.Instance.mRecordShowStackPanel.Parent as UISystem.Container.Grid.Grid;
            if (grid != null && grid.Visibility == UISystem.Visibility.Visible)
            {
                this.TickSampDatas();
                manager.SetValue(uiFPS, Fps.ToString("F1"));

                var graphics = CCore.Engine.Instance.Client.Graphics;
                manager.SetValue(uiForce, graphics.GetResourceForcePreUseCount().ToString());
                manager.SetValue(uiAsync, graphics.GetResourceAsyncCount().ToString());

                manager.SetValue(uiTexture, graphics.GetResourceCountByType(0).ToString() + "/" + (graphics.TextureTotalSize / 1024).ToString());
                manager.SetValue(uiVMObj, graphics.GetResourceCountByType(1).ToString() + "/" + (graphics.VMObjTotalSize / 1024).ToString());
                manager.SetValue(uiRAMObj, graphics.GetResourceCountByType(2).ToString() + "/" + (graphics.RAMObjTotalSize / 1024).ToString());

                manager.SetValue(uiClear, mClearCount.ToString());
                manager.SetValue(uiDP, string.Format("{0}/{1}", mDPCount, DPCountUI));//CCore.Engine.Instance.Client.Graphics.GetDPCount().ToString());
                    manager.SetValue(uiDSDP, mREnviroment.DSDrawCall.ToString());
                    manager.SetValue(uiFSDP, mREnviroment.FSDrawCall.ToString());
                    manager.SetValue(uiShadowDP, ShadowDP.ToString());
                manager.SetValue(uiTri, mTriCount.ToString());
                    manager.SetValue(uiDSTri, mREnviroment.DSDrawTri.ToString());
                    manager.SetValue(uiFSTri, mREnviroment.FSDrawTri.ToString());
                    manager.SetValue(uiShadowTri, ShadowTri.ToString());

                manager.SetValue(uiMgrMemInfo, (System.GC.GetTotalMemory(false) / 1024).ToString());
                manager.SetValue(uiCppMem, ((int)CCore.Engine.Instance.CppMemUsed / 1024).ToString());
                manager.SetValue(uiCppMemMax, ((int)CCore.Engine.Instance.CppMemMax / 1024).ToString());
                manager.SetValue(uiCppMemTimes, (CCore.Engine.Instance.CppMemAllocTimes).ToString());

                var curScene = CCore.Engine.Instance.Client.MainWorld.SceneGraph as CCore.Scene.TileScene.TileScene;
                if(uiRActorNum!=null)
                    manager.SetValue(uiRActorNum, (CCore.Engine.Instance.RenderActorNumber).ToString());
                if(uiTObjNum!=null && curScene!=null)
                    manager.SetValue(uiTObjNum, (curScene.AllTileObjects.Count).ToString());
                if(uiTActorNum!=null)
                    manager.SetValue(uiTActorNum, (CCore.Engine.Instance.TickActorNumber).ToString());
                if(uiNVActorNum!=null)
                    manager.SetValue(uiNVActorNum, (CCore.Engine.Instance.ActorNoVisualTickNumber).ToString());
                if(uiRoleNum!=null)
                    manager.SetValue(uiRoleNum, (Role.RoleManager.Instance.TableSingleIds.Count).ToString());

                if(uiACommonNum != null)
                    manager.SetValue(uiACommonNum, (CCore.Engine.Instance.RenderActor_Common_Number).ToString());
                if (uiAPlayerNum != null)
                    manager.SetValue(uiAPlayerNum, (CCore.Engine.Instance.RenderActor_Player_Number).ToString());
                if (uiANpcNum != null)
                    manager.SetValue(uiANpcNum, (CCore.Engine.Instance.RenderActor_Npc_Number).ToString());
                if (uiALightNum != null)
                    manager.SetValue(uiALightNum, (CCore.Engine.Instance.RenderActor_Light_Number).ToString());
                if (uiADecalNum != null)
                    manager.SetValue(uiADecalNum, (CCore.Engine.Instance.RenderActor_Decal_Number).ToString());
                if (uiATriggerNum != null)
                    manager.SetValue(uiATriggerNum, (CCore.Engine.Instance.RenderActor_Trigger_Number).ToString());
                if (uiAEffectNum != null)
                    manager.SetValue(uiAEffectNum, (CCore.Engine.Instance.RenderActor_Effect_Number).ToString());
                if (uiAEffectNpcNum != null)
                    manager.SetValue(uiAEffectNpcNum, (CCore.Engine.Instance.RenderActor_EffectNpc_Number).ToString());
                if (uiANpcInitlNum != null)
                    manager.SetValue(uiANpcInitlNum, (CCore.Engine.Instance.RenderActor_NpcInitializer_Number).ToString());

                if (uiPModNum != null)
                    manager.SetValue(uiPModNum, (CCore.Engine.Instance.Effect_ParticleMesh_Number).ToString());
                if (uiPPoolNum != null)
                    manager.SetValue(uiPPoolNum, (CCore.Engine.Instance.Effect_ParticlePool_Number).ToString());
                if (uiPLiveNum != null)
                    manager.SetValue(uiPLiveNum, (CCore.Engine.Instance.Effect_ParticleLive_Number).ToString());

                #region Unused
                if (curScene != null)
                {
                    //RootUIMsg.Root.ActorInfoString = String.Format("Async={0},Render={1},Tick=[{2}:{3}:{4}],TickRole={5},NoVisualTick={6},AnimTick={7},RoleNum={8}",
                    //CCore.Engine.Instance.GetAsyncLoadNumber().ToString("D2"),
                    //CCore.Engine.Instance.RenderActorNumber.ToString("D3"),
                    //curScene.AllTileObjects.Count.ToString("D4"),
                    //curScene.WaitAddTileObjects.Count.ToString("D4"),
                    //curScene.WaitRemoveTileObjects.Count.ToString("D4"),
                    //CCore.Engine.Instance.TickActorNumber.ToString("D4"),
                    //CCore.Engine.Instance.ActorNoVisualTickNumber.ToString("D3"),
                    //CCore.Engine.Instance.TickAnimationTreeNumber.ToString("D3"),
                    //Role.RoleManager.Instance.TableSingleIds.Count);
                }
                
                //RootUIMsg.Root.ActorDetailString = String.Format("Common={0},Player={1},Npc={2},Light={3},Decal={4},NpcInit={5},Trigger={6},Effect={7},EffectNpc={8}",
                //    CCore.Engine.Instance.RenderActor_Common_Number.ToString("D3"),
                //    CCore.Engine.Instance.RenderActor_Player_Number.ToString("D3"),
                //    CCore.Engine.Instance.RenderActor_Npc_Number.ToString("D3"),
                //    CCore.Engine.Instance.RenderActor_Light_Number.ToString("D3"),
                //    CCore.Engine.Instance.RenderActor_Decal_Number.ToString("D3"),
                //    CCore.Engine.Instance.RenderActor_NpcInitializer_Number.ToString("D3"),
                //    CCore.Engine.Instance.RenderActor_Trigger_Number.ToString("D3"),
                //    CCore.Engine.Instance.RenderActor_Effect_Number.ToString("D3"),
                //    CCore.Engine.Instance.RenderActor_EffectNpc_Number.ToString("D3")
                //    );

                //RootUIMsg.Root.ParticleDetailString = String.Format("ParticleModifier={0},PoolParticle={1},LiveParticle={2},",
                //    CCore.Engine.Instance.Effect_ParticleMesh_Number.ToString("D3"),
                //    CCore.Engine.Instance.Effect_ParticlePool_Number.ToString("D3"),
                //    CCore.Engine.Instance.Effect_ParticleLive_Number.ToString("D3")
                //    );

                //if (CSUtility.FileDownload.FileDownloadManager.Instance == null)
                //    return;
                //var infos = CSUtility.FileDownload.FileDownloadManager.Instance.GetCurrentDownloadingFiles();
                //string downloadingFiles = "";
                //foreach (var info in infos)
                //{
                //    if (info == null)
                //        continue;

                //    downloadingFiles += info.SavePath + "\r\n";
                //}
                //RootUIMsg.Root.DownloadingString = "DownloadingFilesCount=" + CSUtility.FileDownload.FileDownloadManager.Instance.DownloadingFilesCount + "," +
                //                                    "DownloadServicesCount=" + CSUtility.FileDownload.FileDownloadManager.Instance.DownloadingServicesCount + ",\r\n" +
                //                                    "mDownloadedFiles=" + mDownloadedFiles + ",\r\n" +
                //                                    "CurrentFile=" + downloadingFiles;

                //RootUIMsg.Root.LineCheckString = string.Format("BSP[{0}]Mesh[{1}]Tri[{2}]Box[{3}]",
                //    MidLayer.IMesh.MeshBSPLineCheckNumber.ToString("D4"),
                //    MidLayer.IMesh.MeshTriLineCheckNumber.ToString("D4"),
                //    MidLayer.IMesh.TotalTringleTest.ToString("D6"),
                //    MidLayer.IMesh.TotalBoxTest.ToString("D6")
                //    );

                //RootUIMsg.Root.PerCounterString = MidLayer.IMesh.CountLineCheck.Infomation;

                ////RootUIMsg.Root.DownloadingString = string.Format("Elapse = {0},Distance = {1}",FrameSet.ClientStates.Walk.STickMovementElapse.ToString("D8"), FrameSet.ClientStates.Walk.STickMovementDistance);
                ////var iii = String.Format("Elapse={2},Tri={0},DP={1}"
                ////        , CCore.Engine.Instance.Client.Graphics.GetDrawTriangleCount()
                ////        , CCore.Engine.Instance.Client.Graphics.GetDPCount()
                ////        , CCore.Engine.Instance.GetElapsedMillisecond());
                ////System.Console.WriteLine(iii);
                #endregion

                manager.Tick();

                mDebugInfoTickTimer.End();
            }
        }

        public static string mOutputFile;
        private void LoadOutputXml(string file)
        {
            var xmlHolder = CSUtility.Support.XmlHolder.LoadXML(file);
            if (xmlHolder == null)
            {
                xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder("Output", "0");
                var node = xmlHolder.RootNode.AddNode("FPS", "", xmlHolder);
                node.AddAttrib("Variable", "uiFPS");
                uiFPS = RecordVManager.Instance.AddRecord("FPS", null);
                node = xmlHolder.RootNode.AddNode("Gpu", "", xmlHolder);
                var uiGpu = RecordVManager.Instance.AddRecord("Gpu", null);
                node.AddNode("DrCall", "", xmlHolder).AddAttrib("Variable", "uiDP");
                uiDP = RecordVManager.Instance.AddRecord("DrCall", uiGpu);
                node = xmlHolder.RootNode.AddNode("MTick", "", xmlHolder);
                node.AddAttrib("Value", "IGame.MTick");
                this.AddSamp("IGame.MTick", "MTick", null);
                CSUtility.Support.XmlHolder.SaveXML(file, xmlHolder, true);
                return;
            }
            LoadOutputUI(xmlHolder.RootNode, null);
        }

        private void LoadOutputUI(CSUtility.Support.XmlNode parentNode, UISystem.WinForm parentUI)
        {
            foreach (var node in parentNode.GetNodes())
            {
                UISystem.WinForm form = null;
                if (node.GetAttribs().Count > 0)
                {
                    var attrib = node.FindAttrib("Value");
                    if (attrib != null)
                        form = this.AddSamp(attrib.Value, node.Name, parentUI).Form;
                    attrib = node.FindAttrib("Variable");
                    if (attrib != null)
                    {
                        var field = this.GetType().GetField(attrib.Value, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        form =  RecordVManager.Instance.AddRecord(node.Name, parentUI);
                        if (field != null)
                            field.SetValue(this, form);
                    }
                }
                else
                {
                    form = RecordVManager.Instance.AddRecord(node.Name, parentUI);
                }
                LoadOutputUI(node, form);
            }
        }

        public void InitUI()
        {
            var manager = RecordVManager.Instance;
            manager.LoadUI();
            manager.SetRoot(RootUIMsg.Root);
            mOutputFile = CSUtility.Support.IFileManager.Instance.Root + "OutputConfig.cfg";
            LoadOutputXml(mOutputFile);

            RecordVManager.Instance.AddRecordPerformance("Light");
            RecordVManager.Instance.AddRecordPerformance("ColorGrading");
            RecordVManager.Instance.AddRecordPerformance("Bloom");
            RecordVManager.Instance.AddRecordPerformance("HDR");
            RecordVManager.Instance.AddRecordPerformance("FSBlur");
            RecordVManager.Instance.AddRecordPerformance("FSCopyPre");
            RecordVManager.Instance.AddRecordPerformance("FAlpha");
            RecordVManager.Instance.AddRecordPerformance("FSPostBlur");
            RecordVManager.Instance.AddRecordPerformance("HitProxy");
            RecordVManager.Instance.AddRecordPerformance("Shadow");
            RecordVManager.Instance.AddRecordPerformance("DebugDP");
        }

        public void InitOutputUI()
        {
            var manager = RecordVManager.Instance;
            manager.LoadUI();
            manager.SetRoot(RootUIMsg.Root);
            
            uiFPS = manager.AddRecord("FPS", null);
            var uiGpu = manager.AddRecord("Gpu", null);
                uiDP = manager.AddRecord("DrCall", uiGpu);
                    uiDSDP = manager.AddRecord("DSDP", uiDP);
                    uiFSDP = manager.AddRecord("FSDP", uiDP);
                    uiShadowDP = manager.AddRecord("ShadowDP", uiDP);
                uiTri = manager.AddRecord("Triangle", uiGpu);
                    uiDSTri = manager.AddRecord("DSTri", uiTri);
                    uiFSTri = manager.AddRecord("FSTri", uiTri);
                    uiShadowTri = manager.AddRecord("ShadowTri", uiTri);
                uiClear = manager.AddRecord("Clear", uiGpu);
                this.AddSamp("CriticalDebugger", "LockDebugger", uiGpu);

            var uiRes = manager.AddRecord("Res", null);
                uiForce = manager.AddRecord("Force", uiRes);
                uiAsync = manager.AddRecord("Async", uiRes);
                uiTexture = manager.AddRecord("Texture", uiRes);
                uiVMObj = manager.AddRecord("VMObj", uiRes);
                uiRAMObj = manager.AddRecord("RAMObj", uiRes);
                this.AddSamp("v3dModelSource.LoadShareHead", "LoadMesh", uiRes);
                this.AddSamp("vTerrainPatch.LoadPatch", "LoadTerrain", uiRes);
                this.AddSamp("vTilePatch.LoadPatch", "LoadTile", uiRes);

            var uiMem = manager.AddRecord("Mem", null);
                uiMgrMemInfo = manager.AddRecord("Mgr", uiMem);
                uiCppMem = manager.AddRecord("Cpp", uiMem);
                    uiCppMemMax = manager.AddRecord("Max", uiCppMem);
                    uiCppMemTimes = manager.AddRecord("Times", uiCppMem);

            var mtick = this.AddSamp("IGame.MTick", "MTick", null);
                this.AddSamp("IGame.MWait", "Wait", mtick.Form);
            var uiSTick = this.AddSamp("IGame.SyncTick", "STick", null);

            var samp = this.AddSamp("IGame.RenderTick", "RTick", null);
                this.AddSamp("IGame.RShadow", "RShadow", samp.Form);
                var sampChild1 = this.AddSamp("REnv.Render", "Render", samp.Form);
                    var sampChild2 = this.AddSamp("REnv.Render.DrawAll", "DrAll", sampChild1.Form);
                        var sampChild3 = this.AddSamp("Env.DrawDS", "DS", sampChild2.Form);
                        var sampChild4 = this.AddSamp("Env.DShading", "DSDraw", sampChild3.Form);
                        sampChild4 = this.AddSamp("Env.PostScreen", "PstScr", sampChild3.Form);
                                this.AddSamp("Env.Post.PostProcessPreLight", "PLight", sampChild4.Form);
                                this.AddSamp("Env.DrawEdgeDetect", "Edge", sampChild4.Form);
                                this.AddSamp("Env.Lighting", "Lt", sampChild4.Form);
                                this.AddSamp("Env.HDR", "HDR", sampChild4.Form);            
                        sampChild3 = this.AddSamp("Env.DrawFS", "FS", sampChild2.Form);
                            this.AddSamp("FS.Pre", "FAlpha", sampChild3.Form);
                            sampChild4 = this.AddSamp("FS.Blur", "Blur", sampChild3.Form);
                                this.AddSamp("FS.Blur.DownSampler", "DSamp", sampChild4.Form);
                                this.AddSamp("FS.Blur.Blur", "FBlur", sampChild4.Form);
                                this.AddSamp("FS.Blur.Bloom", "Bloom", sampChild4.Form);
                            this.AddSamp("FS.Blur.CopyPreFinalShow", "CpFinal", sampChild3.Form);
                            sampChild4 = this.AddSamp("FS.Post", "LAlpha", sampChild3.Form);
                                this.AddSamp("Env.Post.SSAO", "SSAO", sampChild4.Form);
                        sampChild3 = this.AddSamp("Env.DrawHlp", "HLP", sampChild2.Form);
                    sampChild2 = this.AddSamp("REnv.Render.DrawPost", "Post", sampChild1.Form);
                            sampChild3 = this.AddSamp("Env.Post.PostProcessAfterLight", "ALight", sampChild2.Form);
                            this.AddSamp("Env.Post.ColorGrading", "CGrade", sampChild3.Form);
                            this.AddSamp("Env.Post.Bloom", "Bloom", sampChild3.Form);
                    this.AddSamp("REnv.Render.R2View", "R2V", sampChild1.Form);
                    sampChild2 = this.AddSamp("REnv.Render.BeforeR2View", "BfrR2V", sampChild1.Form);
                        if (this.GInit.ScaleUIWidthViewTarget)
                            InitOP_DrawUI(sampChild2);
                    sampChild2 = this.AddSamp("REnv.Render.AfterR2View", "AftR2V", sampChild1.Form);
                        if (this.GInit.ScaleUIWidthViewTarget==false)
                            InitOP_DrawUI(sampChild2);
                    this.AddSamp("REnv.Render.HitProxy", "HProxy", sampChild1.Form);
            
                    
            samp = this.AddSamp("IGame.LogicTick", "LTick", null);
                    this.AddSamp("Env.ClearDrawCommits", "ClearCommits", samp.Form);
                    sampChild1 = this.AddSamp("IGame.LTick.Engine", "Engine", samp.Form);
                    sampChild2 = this.AddSamp("LTick.Engine.Client", "Client", sampChild1.Form);
                        this.AddSamp("TileScene.TickActor", "TickActor", sampChild2.Form);
                    this.AddSamp("LTick.Engine.LogiTimer", "LTimer", sampChild1.Form);
                sampChild1 = this.AddSamp("IGame.LTick.RWorld", "RWorld", samp.Form);
                    this.AddSamp("World.TerrainVisible", "TrVis", sampChild1.Form);
                    this.AddSamp("World.TileVisible", "TiVis", sampChild1.Form);
                this.AddSamp("IGame.LTick.Stage", "Stage", samp.Form);
                sampChild1 = this.AddSamp("IGame.TickUI", "TickUI", samp.Form);
                    this.AddSamp("TickUI.Layout", "Layout", sampChild1.Form);
                    this.AddSamp("TickUI.Tick", "Tick", sampChild1.Form);
                sampChild1 = this.AddSamp("IGame.CommitUI", "CommitUI", samp.Form);
                this.AddSamp("IGame.DbgInfo", "DbgInfo", samp.Form);

            samp = this.AddSamp("DrawAtom", "DrawAtom", null);
                this.AddSamp("ModStack.OnDraw", "Modifier", samp.Form);
                this.AddSamp("EftTech", "Tech", samp.Form);
                this.AddSamp("EftPass", "Pass", samp.Form);
                this.AddSamp("EftVB", "VB", samp.Form);
                this.AddSamp("EftDP", "DP", samp.Form);
                this.AddSamp("EftDPInst", "DPInst", samp.Form);

            

            var uiPostProcess = manager.AddRecord("PostProcess", null);
                this.AddSamp("Env.Post.DownSampler", "PP.DSmp", uiPostProcess);
                this.AddSamp("Env.Post.Blur", "PP.Blur", uiPostProcess);
                this.AddSamp("Env.Post.Copy", "PP.Copy", uiPostProcess);

            samp = this.AddSamp("Env.Post.ToneMapping", "HDR", null);
                this.AddSamp("Env.Post.SumLum", "SL", samp.Form);
                this.AddSamp("Env.Post.AdaptedLum", "AdL", samp.Form);
                this.AddSamp("Env.Post.BrightDownsampler", "BDSmp", samp.Form);
                this.AddSamp("Env.Post.LensEffects", "Lens", samp.Form);

            var uiGles = manager.AddRecord("ESR2T", null);
                this.AddSamp("ESR2T.Bind", "Bind", uiGles);
                this.AddSamp("ESR2T.ApplyLayer", "ALayer", uiGles);
                this.AddSamp("ESR2T.ApplyDepth", "ADepth", uiGles);

            

            RecordVManager.Instance.AddRecordPerformance("Light");
            RecordVManager.Instance.AddRecordPerformance("ColorGrading");
            RecordVManager.Instance.AddRecordPerformance("Bloom");
            RecordVManager.Instance.AddRecordPerformance("HDR");
            RecordVManager.Instance.AddRecordPerformance("FSBlur");
            RecordVManager.Instance.AddRecordPerformance("FSCopyPre");
            RecordVManager.Instance.AddRecordPerformance("FAlpha");
            RecordVManager.Instance.AddRecordPerformance("FSPostBlur");
            RecordVManager.Instance.AddRecordPerformance("HitProxy");
            RecordVManager.Instance.AddRecordPerformance("Shadow");
            RecordVManager.Instance.AddRecordPerformance("DebugDP");
        }
        private void InitOP_DrawUI(SampData parent)
        {
            var c1 = this.AddSamp("UIDraw.CommitAll", "CommitUI", parent.Form);
                this.AddSamp("DrawUI.Text", "Text", c1.Form);
                var c2 = this.AddSamp("DrawAtomUI", "DrawAtomUI", c1.Form);
                    this.AddSamp("DrawUI.Tech", "Tech", c2.Form);
                    this.AddSamp("DrawUI.Pass", "Pass", c2.Form);
                    this.AddSamp("DrawUI.VB", "VB", c2.Form);
                    this.AddSamp("DrawUI.DP", "DP", c2.Form);
        }
        UISystem.WinForm uiFPS;
        UISystem.WinForm uiForce;
        UISystem.WinForm uiAsync;
        UISystem.WinForm uiTexture;
        UISystem.WinForm uiVMObj;
        UISystem.WinForm uiRAMObj;

        UISystem.WinForm uiClear;
        UISystem.WinForm uiDP;
        UISystem.WinForm uiDSDP;
        UISystem.WinForm uiFSDP;
        UISystem.WinForm uiShadowDP;
        UISystem.WinForm uiTri;
        UISystem.WinForm uiDSTri;
        UISystem.WinForm uiFSTri;
        UISystem.WinForm uiShadowTri;

        UISystem.WinForm uiMgrMemInfo;
        UISystem.WinForm uiCppMem;
        UISystem.WinForm uiCppMemMax;
        UISystem.WinForm uiCppMemTimes;

        UISystem.WinForm uiRActorNum;
        UISystem.WinForm uiTObjNum;
        UISystem.WinForm uiTActorNum;
        UISystem.WinForm uiNVActorNum;
        UISystem.WinForm uiRoleNum;

        UISystem.WinForm uiACommonNum;
        UISystem.WinForm uiAPlayerNum;
        UISystem.WinForm uiANpcNum;
        UISystem.WinForm uiALightNum;
        UISystem.WinForm uiADecalNum;
        UISystem.WinForm uiATriggerNum;
        UISystem.WinForm uiAEffectNum;
        UISystem.WinForm uiAEffectNpcNum;
        UISystem.WinForm uiANpcInitlNum;

        UISystem.WinForm uiPModNum;
        UISystem.WinForm uiPPoolNum;
        UISystem.WinForm uiPLiveNum;

        public class SampData
        {
            public bool IsMaxTime = false;
            public UISystem.WinForm Form;
            public CSUtility.Performance.PerfCounter Counter;
        }
        Dictionary<string, SampData> SampDatas = new Dictionary<string, SampData>();
        private SampData AddSamp(string name, string showName, UISystem.WinForm Parent, bool maxTime=false)
        {
            var data = new SampData();
            data.IsMaxTime = maxTime;
            data.Form = RecordVManager.Instance.AddRecord(showName, Parent);
            data.Counter = new CSUtility.Performance.PerfCounter(name);
            if (maxTime)
                data.Counter.AvgCounter = int.MaxValue;
            SampDatas.Add(name, data);
            return data;
        }

        private void TickSampDatas()
        {
            var manager = RecordVManager.Instance;
            foreach (var i in SampDatas)
            {
                var v = i.Value;
                if (v.IsMaxTime)
                {
                    manager.SetValue(v.Form, v.Counter.MaxTimeInCounter.ToString());
                }
                else
                {
                    manager.SetValue(v.Form, v.Counter.AvgTime.ToString() + ":" + v.Counter.AvgHit);
                }
            }
        }
        
        public CSUtility.Performance.PerfCounter GetSamp(string name)
        {
            SampData oData;
            if (SampDatas.TryGetValue(name, out oData))
                return oData.Counter;
            return AddSamp(name, name, null).Counter;
        }

        //public void OutPutNetPing()
        //{
        //    if (!IZeusGame.Instance.ShowPing || !IZeusGame.Instance.ShowPingOptions)
        //    {
        //        RootUIMsg.Root.PlaneTickCount = "";
        //        RootUIMsg.Root.GateTickCount = "";
        //        return;
        //    }


        //    Int64 preTime = MidLayer.IDllImportAPI.HighPrecision_GetTickCount();
        //    RPC.PackageWriter pkg = new RPC.PackageWriter();
        //    ServerCommon.Planes.H_PlayerInstance.smInstance.RPC_GetNetElapsed(pkg);
        //    pkg.WaitDoClient2PlanesPlayer(MidLayer.CCore.Engine.Instance.Client.GateSvrConnect, 10).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
        //    {
        //        if (bTimeOut)
        //        {
        //            RootUIMsg.Root.PlaneTickCount = "超时";
        //            return;
        //        }

        //        float fps = 0;
        //        Int64 nowTime = MidLayer.IDllImportAPI.HighPrecision_GetTickCount();
        //        float elapsedCount = (float)(nowTime - preTime) / 1000f;

        //        sbyte successed = -1;
        //        _io.Read(out successed);
        //        if (successed == 1)
        //        {
        //            _io.Read(out fps);
        //        }

        //        RootUIMsg.Root.PlaneTickCount = (int)elapsedCount + "ms:Fps = " + fps.ToString("F1");
        //    };

        //    preTime = MidLayer.IDllImportAPI.HighPrecision_GetTickCount();
        //    RPC.PackageWriter pkg1 = new RPC.PackageWriter();
        //    ServerCommon.H_RPCRoot.smInstance.HGet_GateServer(pkg1).RPC_GetNetElapsed(pkg1);
        //    pkg1.WaitDoCommandWithTimeOut(10, MidLayer.CCore.Engine.Instance.Client.GateSvrConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace()).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
        //    {
        //        if (bTimeOut)
        //        {
        //            RootUIMsg.Root.GateTickCount = "超时";
        //            return;
        //        }

        //        Int64 nowTime = MidLayer.IDllImportAPI.HighPrecision_GetTickCount();
        //        float elapsedCount = (float)(nowTime - preTime) / 1000f;

        //        RootUIMsg.Root.GateTickCount = "ping:" + (int)elapsedCount;
        //    };
        //}
    }
}
