using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Title
{
    public enum TitleType
    {
        Role,
        Guild,
        Title,
        Faction,
    }
    public class TitleShowManager
    {
        static TitleShowManager mInstance = new TitleShowManager();
        public static TitleShowManager Instance
        {
            get { return mInstance; }
        }

        public static void FinalInstance()
        {
            mInstance = null;
        }

        UISystem.WinForm mForm = null;
        public UISystem.WinForm Form
        {
            get { return mForm; }
        }
        UISystem.Container.Canvas mMainCanvas = null;
        public UISystem.Container.Canvas MainCanvas
        {
            get { return mMainCanvas; }
        }

        public bool mIsShowOtherPlayerName = true;
        public bool mIsShowMonsterName = true;
        public bool mIsShowDropItemName = true;
        public bool mIsShowNpcName = true;

        public class CTitleData
        {
            public UISystem.TextBlock RoleNameText = null;
            public UISystem.TextBlock RoleLevelText = null;
            public UISystem.Container.Grid.Grid RoleLevelBackGrid = null;
            public UISystem.Progress.ProgressBar RoleHpImg = null;
            public UISystem.Progress.ProgressBar RoleMpImg = null;
            public float X = 0;
            public float Y = 0;
            public Role.RoleActor mRole = null;
            public long LifeTime;

            public UISystem.WinForm Win = null;
            public CCore.Camera.CameraObject Camera;

            bool mIsShowTitleWinUI = true;

            public CTitleData(UISystem.WinForm win, Role.RoleActor role, float x, float y, CCore.Camera.CameraObject camera)
            {
                Reset();
                Win = win;
                X = x;
                Y = y;
                Camera = camera;
                mRole = role;
                if (Win != null)
                {
                    InitTitle();
                }
            }

            public void InitTitle()
            {
                RoleNameText = Win.FindControl("RoleNameText") as UISystem.TextBlock;
                RoleLevelText = Win.FindControl("RoleLevelText") as UISystem.TextBlock;
                RoleLevelBackGrid = Win.FindControl("LevelBackGrid") as UISystem.Container.Grid.Grid;
                RoleHpImg = Win.FindControl("Hp") as UISystem.Progress.ProgressBar;
                RoleMpImg = Win.FindControl("Mp") as UISystem.Progress.ProgressBar;

                Win.Tag = mRole;
                Win.HitTestVisible = false;
                //mRole.ShowCloseRoleTitle = SetRoleTitleUI;
                //InitImage();
                SetWinUI();
            }
            
            public void SetRoleTitleUI(bool bShow)      //隐身技能显示OR隐藏Title
            {
                if (Win == null)
                    return;
                if (bShow)
                    mIsShowTitleWinUI = true;
                else
                    mIsShowTitleWinUI = false;
            }

            public void DrawTitle()
            {
                if (mRole == null)
                    return;

                lock (mRole)
                {
                    double ratio = 0.0f;
                    if (RoleHpImg != null)
                    {
                        if (mRole.RoleData.RoleMaxHP == 0)
                            ratio = 1.0f;
                        else
                            ratio = (double)mRole.RoleData.RoleHP / (double)mRole.RoleData.RoleMaxHP;
                        RoleHpImg.Percent = ratio;
                    }
                    if (RoleMpImg != null)
                    {
                        if (mRole.RoleData.RoleMaxMP == 0)
                            ratio = 1.0f;
                        else
                            ratio = (double)mRole.RoleData.RoleMP / (double)mRole.RoleData.RoleMaxMP;
                        RoleMpImg.Percent = ratio;
                    }
                    if (RoleLevelText != null)
                    {
                        RoleLevelText.Text = mRole.RoleData.RoleLevel.ToString();
                    }

                    UpdataPos(X - Win.Width / 2, Y - Win.Height);
                }

                if(mRole.CurrentState!=null)
                {
                    if (mIsShowTitleWinUI && mRole.CurrentState.StateName != "Death")
                    {
                        if (Win.Visibility != UISystem.Visibility.Visible)
                        {
                            Win.Show();
                        }
                    }
                    else
                    {
                        Win.Close(0);
                    }
                }
                Win.UpdateUI();
            }

            public void SetWinUI()
            {
                switch (mRole.RoleData.RoleType)
                {
                    case Role.EClientRoleType.ChiefPlayer:
                    case Role.EClientRoleType.OtherPlayer:
                        {
                            UpdataWinUI(mRole.RoleName, mRole.RoleData.PlayerData.RoleLevel);
                        }
                        break;
                    case Role.EClientRoleType.DropedItem:
                        break;
                    case Role.EClientRoleType.Monster:
                        {
                            UpdataWinUI(mRole.RoleName, 0);
                        }
                        break;
                    case Role.EClientRoleType.GatherItem:
                        break;
                    case Role.EClientRoleType.Summon:
                        break;
                }
                Win.UpdateUI();
            }

            public void Reset()
            {
                if (Win != null)
                {
                    Win.Tag = null;
                    Win.Close(0);
                }

                X = 0;
                Y = 0;
                if (mRole != null)
                {
                    lock (mRole)
                    {
                        mRole = null;
                    }
                }
                Camera = null;
                LifeTime = 1000;
            }

            public class TitleFontRenderParamManager
            {
                public static TitleFontRenderParamManager Instance
                {
                    get
                    {
                        return CCore.Support.ReflectionManager.Instance.GetClassObject<TitleFontRenderParamManager>();
                    }
                }

                //public Map<TitleType, MidLayer.IFontRenderParamList> mFontRenderParamsList;
                public CCore.Font.FontRenderParamList GetFontRenderParam(TitleType titleType, Role.RoleActor role)
                {
                    CCore.Font.FontRenderParamList fontRenderParam = new CCore.Font.FontRenderParamList();
                    switch (titleType)
                    {
                        case TitleType.Role:
                        case TitleType.Title:
                        case TitleType.Faction:
                            var fontParam1 = fontRenderParam.AddParam();
                            fontParam1.OutlineType = CCore.Font.enFontOutlineType.Line;
                            fontParam1.OutlineThickness = 1;
                            fontParam1.TLColor = fontParam1.TRColor = fontParam1.BLColor = fontParam1.BRColor = CSUtility.Support.Color.FromArgb(128, 25, 25, 25);
                            var fontParam2 = fontRenderParam.AddParam();
                            fontParam2.OutlineType = CCore.Font.enFontOutlineType.None;
                            fontParam2.TLColor = fontParam2.TRColor = fontParam2.BLColor = fontParam2.BRColor = CSUtility.Support.Color.WhiteSmoke;
                            break;
                        case TitleType.Guild:
//                             var fontParam3 = fontRenderParam.AddParam();
//                             fontParam3.OutlineType = CCore.Font.enFontOutlineType.Line;
//                             fontParam3.OutlineThickness = 1;
//                             //fontParam3.TLColor = fontParam3.TRColor = fontParam3.BLColor = fontParam3.BRColor = CSUtility.Support.IFileConfig.;
//                             var fontParam4 = fontRenderParam.AddParam();
//                             fontParam4.OutlineType = CCore.Font.enFontOutlineType.None;
//                             fontParam4.TLColor = fontParam4.TRColor = fontParam4.BLColor = fontParam4.BRColor = CSUtility.Support.Color.Blue;
                            break;
                    }

                    return fontRenderParam;
                }
            }

            public void UpdataWinUI(string RoleName, byte RoleLevel)
            {
                if (RoleNameText != null)
                {
                    RoleNameText.Text = RoleName;
                    RoleNameText.FontRenderParams = TitleFontRenderParamManager.Instance.GetFontRenderParam(TitleType.Role, mRole);
                }
                if (RoleLevelText != null)
                {
                    RoleLevelText.Text = RoleLevel.ToString();
                    RoleLevelText.FontRenderParams = TitleFontRenderParamManager.Instance.GetFontRenderParam(TitleType.Role, mRole);
                }
                if (RoleLevelBackGrid != null && Stage.MainStage.Instance.ChiefRole != null)
                {
                    if (mRole.IsChielfPlayer())
                        RoleLevelBackGrid.BackColor = CSUtility.Support.Color.LightSeaGreen;
                    else
                    {
                        if (mRole.RoleData.FactionId != Stage.MainStage.Instance.ChiefRole.RoleData.FactionId)
                            RoleLevelBackGrid.BackColor = CSUtility.Support.Color.OrangeRed;
                        else
                            RoleLevelBackGrid.BackColor = CSUtility.Support.Color.AliceBlue;
                    }
                }

                Win.UpdateLayout(true);
            }

            public void UpdataPos(float x, float y)
            {
                var pForm = Win as UISystem.WinForm;
                var pos = pForm.AbsRect.Location;
                pos.X = (int)x;
                pos.Y = (int)y;
                pForm.MoveWin(ref pos);
            }
        }
        Dictionary<UInt32, CTitleData> mTitleShowDatas = new Dictionary<UInt32, CTitleData>();
        public Dictionary<UInt32, CTitleData> TitleShowDatas
        {
            get { return mTitleShowDatas; }
        }

        List<List<UISystem.WinForm>> mTiltlePondUI = new List<List<UISystem.WinForm>>();

        int[] mCurrentShowIndexs;

        //脏数据
        Dictionary<UInt32, CTitleData> mNoUserTitleDatas = new Dictionary<UInt32, CTitleData>();

        public void Tick(long milliSecondElapsedTime)
        {
            try
            {
                //脏数据处理
                ClearNoUserTitleDatas();

                lock (mTitleShowDatas)
                {
                    List<uint> removeList = new List<uint>();
                    foreach (var i in mTitleShowDatas)
                    {
                        i.Value.LifeTime -= milliSecondElapsedTime;
                        var pos = i.Value.mRole.Placement.GetLocation();
                        var screenPt = i.Value.Camera.GetScreenCoord(ref pos, Client.Game.Instance.GInit.Vector2ScreenCoordScale);
                        if (i.Value.mRole == null || !i.Value.mRole.Visible || i.Value.mRole.IsLeaveMap || i.Value.Win == null || i.Value.LifeTime <= 0)
                        {
                            removeList.Add(i.Key);
                        }
                    }
                    foreach (var key in removeList)
                    {
                        mTitleShowDatas[key].Reset();
                        mTitleShowDatas.Remove(key);
                    }

                    foreach (var i in mTitleShowDatas)
                    {
                        //                         if (CanShowWinUI(i.Value))
                        //                         {
                        //                             if (!i.Value.mRole.IsChielfPlayer())
                        //                             {
                        //                                 UpdateWinWithHatred(i.Value);
                        //                             }
                        //                             i.Value.DrawTitle();
                        //                         }
                        //                         else
                        //                         {
                        //                             i.Value.Win.Close();
                        //                         }
                        i.Value.DrawTitle();
                    }
                }
            }
            catch (System.Exception)
            {

            }
            //TitleShowDatas.Clear();
        }

//         public bool CanShowWinUI(CTitleData data)
//         {
//             if (data.mRole.RoleData.RoleType == Role.EClientRoleType.OtherPlayer && !mIsShowOtherPlayerName)
//                 return false;
//             else if (data.mRole.RoleData.RoleType == Role.EClientRoleType.DropedItem && !mIsShowDropItemName)
//                 return false;
//             else if (data.mRole.RoleData.RoleType == Role.EClientRoleType.Npc)
//             {
//                 if (data.mRole.RoleData.MonsterData.RoleType >= CSCommon.Data.ERoleType.Monster && data.mRole.RoleData.MonsterData.RoleType <= CSCommon.Data.ERoleType.Boss)
//                 {
//                     if (!mIsShowMonsterName)
//                         return false;
//                 }
//                 else if (!mIsShowNpcName)
//                 {
//                     return false;
//                 }
//             }
//             return true;
//         }

        public TitleShowManager()
        {
//             var performOptions = CSCommon.Data.PerformOptions.PerformOptionsTemplateManager.Instance.FindPerformOptions(0);
//             if (performOptions != null)
//             {
//                 mIsShowOtherPlayerName = performOptions.IsShowOtherPlayerName;
//                 mIsShowMonsterName = performOptions.IsShowMonsterName;
//                 mIsShowDropItemName = performOptions.IsShowDropItemName;
//             }
            mForm = CCore.Support.ReflectionManager.Instance.GetUIForm("TitleRoot") as UISystem.WinForm;
            if (mForm == null)
                return;
            mMainCanvas = mForm.FindControl("MainCanvas") as UISystem.Container.Canvas;

            int maxCount = 0;
            mCurrentShowIndexs = new int[(int)Role.EClientRoleType.Count];
            for (var typeId = 0; typeId < (int)Role.EClientRoleType.Count; typeId++)
            {
                var datas = new List<UISystem.WinForm>();
                mTiltlePondUI.Add(datas);
                switch ((Role.EClientRoleType)typeId)
                {
                    case Role.EClientRoleType.ChiefPlayer:
                        maxCount = 1;
                        break;
                    case Role.EClientRoleType.OtherPlayer:
                        maxCount = 5;
                        break;
                    case Role.EClientRoleType.Monster:
                        maxCount = 5;
                        break;
                    case Role.EClientRoleType.DropedItem:
                        maxCount = 5;
                        break;
                    case Role.EClientRoleType.Summon:
                        maxCount = 0;
                        break;
                    case Role.EClientRoleType.GatherItem:
                        maxCount = 5;
                        break;
                }
                for (int i = 0; i < maxCount; i++)
                {
                    var ctrl = CreateTitleForm(typeId);
                    mTiltlePondUI[typeId].Add(ctrl);
                }
            }
            SetRoot(Game.Instance.RootUIMsg.Root);
        }

        private UISystem.WinForm CreateTitleForm(int type)
        {
            string titleShowFormName = "";
            string titleWinUIName = "";
            switch ((Role.EClientRoleType)type)
            {
                case Role.EClientRoleType.ChiefPlayer:
                    titleShowFormName = "ChiefPlayerTitle";
                    titleWinUIName = "RoleTitle";
                    break;
                case Role.EClientRoleType.OtherPlayer:
                    titleShowFormName = "OtherPlayerTitle";
                    titleWinUIName = "RoleTitle";
                    break;
                case Role.EClientRoleType.Monster:
                    titleShowFormName = "NpcTitle";
                    titleWinUIName = "NpcTitle";
                    break;
                case Role.EClientRoleType.DropedItem:
                    titleShowFormName = "DropItemTitle";
                    titleWinUIName = "ItemTitle";
                    break;
                case Role.EClientRoleType.Summon:
                    return null;
                //                     titleShowFormName = "";
                //                     break;
                case Role.EClientRoleType.GatherItem:
                    titleShowFormName = "GatherItemTitle";
                    titleWinUIName = "ItemTitle";
                    break;
            }

            var ctrl = CCore.Support.ReflectionManager.Instance.GetUIForm(titleWinUIName, titleShowFormName + mTiltlePondUI[type].Count) as UISystem.WinForm;
            if (ctrl == null)
                return null;
            ctrl.Visibility = UISystem.Visibility.Collapsed;
            ctrl.Parent = mMainCanvas;

            return ctrl;
        }

        private void ClearNoUserTitleDatas()
        {
            lock (mNoUserTitleDatas)
            {
                foreach (var data in mNoUserTitleDatas)
                {
                    if (data.Value.Win.Visibility == UISystem.Visibility.Collapsed)
                        continue;
                    data.Value.Win.Visibility = UISystem.Visibility.Collapsed;
                }
                mNoUserTitleDatas.Clear();
            }
        }

        private UISystem.WinForm GetWinPondUI(int titleTypeId)
        {
            if (titleTypeId == (int)Role.EClientRoleType.ChiefPlayer)
            {
                if (mTiltlePondUI[titleTypeId].Count > mCurrentShowIndexs[titleTypeId])
                    return mTiltlePondUI[titleTypeId][mCurrentShowIndexs[titleTypeId]];
                else
                    return null;
            }
            for (int i = 0; i < mTiltlePondUI[titleTypeId].Count; i++)
            {
                mCurrentShowIndexs[titleTypeId]++;

                if (mCurrentShowIndexs[titleTypeId] >= mTiltlePondUI[titleTypeId].Count)
                    mCurrentShowIndexs[titleTypeId] = 0;

                if (mTiltlePondUI[titleTypeId][mCurrentShowIndexs[titleTypeId]].Visibility != UISystem.Visibility.Collapsed)
                {
                    if (mTiltlePondUI[titleTypeId][mCurrentShowIndexs[titleTypeId]].Tag != null)
                    {
                        var role = mTiltlePondUI[titleTypeId][mCurrentShowIndexs[titleTypeId]].Tag as Role.RoleActor;
                        if (role != null)
                        {
                            if (role.IsLeaveMap)
                                mTiltlePondUI[titleTypeId][mCurrentShowIndexs[titleTypeId]].Visibility = UISystem.Visibility.Collapsed;
                            else
                                continue;
                        }
                    }
                    else
                        mTiltlePondUI[titleTypeId][mCurrentShowIndexs[titleTypeId]].Visibility = UISystem.Visibility.Collapsed;
                }
                if (mTiltlePondUI[titleTypeId][mCurrentShowIndexs[titleTypeId]].Tag == null)
                    return mTiltlePondUI[titleTypeId][mCurrentShowIndexs[titleTypeId]];
            }
            var ctrl = CreateTitleForm(titleTypeId);
            if (ctrl == null)
                return null;
            mTiltlePondUI[titleTypeId].Add(ctrl);
            mCurrentShowIndexs[titleTypeId] = mTiltlePondUI[titleTypeId].Count - 1;
            return ctrl;
        }

        public void SetRoot(UISystem.WinBase rootWin)
        {
            mForm.Parent = rootWin;
        }

        public void Add(float x, float y, Role.RoleActor role, CCore.Camera.CameraObject camera)
        {
            if (role == null)
                return;

            if (role.RoleData.RoleType == Role.EClientRoleType.Monster && (role.RoleData.RoleTemplate.MonsterType >= GameData.Role.MonsterType.Symbol || role.RoleData.MonsterData.Unrivaled))
                return;

            CTitleData data = null;
            if (mTitleShowDatas.TryGetValue(role.SingleId, out data) == false)
            {
                lock (mTitleShowDatas)
                {
                    var win = GetWinPondUI((int)role.RoleData.RoleType);
                    data = new CTitleData(win, role, x, y, camera);
                    if (data.Win != null)
                        mTitleShowDatas[role.SingleId] = data;
                }
            }
            else
            {
                if(System.Math.Abs(data.X - x) > 1)
                    data.X = x;
                if(System.Math.Abs(data.Y - y) > 1)
                    data.Y = y;
                data.LifeTime = 1000;
            }
        }

        public bool RemoveTitle(UInt32 id)
        {
            lock (mTitleShowDatas)
            {
                if (!mTitleShowDatas.ContainsKey(id))
                    return false;

                mTitleShowDatas[id].Reset();
                try
                {
                    //脏数据存储
                    lock (mNoUserTitleDatas)
                    {
                        mNoUserTitleDatas[id] = mTitleShowDatas[id];
                    }
                }
                catch (System.Exception)
                {
                }
                mTitleShowDatas.Remove(id);
            }
            return true;
        }

        public void ClearTitleList()
        {
            lock (mTitleShowDatas)
            {
                foreach (var i in mTitleShowDatas)
                {
                    //脏数据存储
                    lock (mNoUserTitleDatas)
                    {
                        mNoUserTitleDatas[i.Key] = i.Value;
                    }
                    i.Value.Reset();
                }
                mTitleShowDatas.Clear();
            }
        }

    }
}
