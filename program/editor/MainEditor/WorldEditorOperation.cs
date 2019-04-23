namespace MainEditor
{
    public partial class WorldEditorOperation : CCore.EditorClassBase
    {
        //static WorldEditorOperation smInstance = new WorldEditorOperation();
        //public static WorldEditorOperation Instance
        //{
        //    get { return smInstance; }
        //}

        CCore.Graphics.REnviroment mREnviroment;
        public CCore.Graphics.REnviroment REnviroment
        {
            get { return mREnviroment; }
        }
        public CCore.World.WorldRenderParam RenderParam
        {
            get;
        } = new CCore.World.WorldRenderParam();

        public bool IsEditorMode
        {
            get { return CCore.Engine.Instance.IsEditorMode; }
            set
            {
                CCore.Engine.Instance.IsEditorMode = value;
                
            }
        }

        public void FinalInstance()
        {
            CCore.Engine.Instance.Client.MsgRecieverMgr.UnRegReciever(this);

            EditorCommon.Hotkey.HotkeyManager.FinalInstance();
            FinalInstance_Axis();
            FinalInstance_Camera();

            EditorCommon.WorldEditMode.FinalInstance();

            mREnviroment = null;
            MainWindow.FinalInstance();
            DockControl.DockManager.FinalInstance();
            GameAssistWindow.FinalInstance();
        }

        // 格子对象
        CCore.World.Actor mGroupGridActor;
        public CCore.World.Actor GroupGridActor
        {
            get { return mGroupGridActor; }
        }
        public void Initialize(CCore.Graphics.REnviroment env, System.Windows.Forms.Form gameForm)
        {
            GameAssistWindow.Instance.WorldEditorOperation = this;

            mREnviroment = env;
            RenderParam.Enviroment = mREnviroment;
            EditorCommon.Program.GameREnviroment = mREnviroment;
            EditorCommon.Program.GameForm = gameForm;
            CCore.Engine.Instance.Client.MsgRecieverMgr.RegReciever(this);
            InitializeMsg();

            Initialzie_Axis();
            Initialize_Camera();
            Initialize_GameWindowDragOperation(gameForm);
            Initialize_ActorOperation();

            var gridInit = new CCore.World.ActorInit();
            mGroupGridActor = new CCore.World.Actor();
            mGroupGridActor.Initialize(gridInit);
            mGroupGridActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
            mGroupGridActor.ActorName = "场景辅助网格";
            mGroupGridActor.Visual = new CCore.Support.GroupGrid();
            mGroupGridActor.SetPlacement(new CSUtility.Component.StandardPlacement(mGroupGridActor));
            mGroupGridActor.Visible = false;

            OnWorldLoaded();

            CCore.Program.OnWorldLoaded += Program_OnWorldLoaded;
        }

        private void Program_OnWorldLoaded(System.String strAbsFolder, string componentName, CCore.World.World world)
        {
            EditorCommon.Program.MainDispatcher.Invoke(() =>
            {
                switch (componentName)
                {
                    case "场景":
                        OnWorldLoaded();
                        break;
                }
            });
        }

        public void OnWorldLoaded()
        {
            if (CCore.Client.MainWorldInstance.IsNullWorld)
                return;

            CCore.Client.MainWorldInstance.AddEditorActor(mGroupGridActor);
            OnWorldLoaded_Axis();

            // 初始化完WORLD后刷新一下模式
            EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_Camera);
        }

        public void OnGameWindowChanged(int left, int top, int height, int width)
        {
            // 在游戏主窗口变化时更新停靠窗口
        }
        private void MoveActorsWithMouse()
        {
            //if(mStartMove)
            //{

            //}
        }
        private void CopySelectedActors()
        {

        }

        public void Tick()
        {
            Tick_Camera();
            EditorCommon.TickInfo.Instance.Tick();
        }
    }
}
