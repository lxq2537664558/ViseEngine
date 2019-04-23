using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class RecordV
    {
        public UISystem.WinForm showForm;
        //public UISystem.WinForm settiingForm;

        public RecordV Parent;
        public List<RecordV> Childrens = new List<RecordV>();

        public RecordV(UISystem.WinForm show/*, UISystem.WinForm setting*/)
        {
            showForm = show;
            //settiingForm = setting;
        }
    }

    public class RecordTitle
    {
        public string Name;
        public CCore.Graphics.Texture Texture;
        public UInt16 Width;
        public UInt16 Height;

        public UISystem.WinForm WinForm;

        public RecordTitle(string name, CCore.Graphics.Texture texture, UInt16 width, UInt16 height)
        {
            Name = name;
            Texture = texture;
            Width = width;
            Height = height;
        }
    }

    public class RecordVManager : UISystem.UIBindAutoUpdate
    {
        public static RecordVManager Instance
        {
            get
            {
                return CCore.Support.ReflectionManager.Instance.GetClassObject<RecordVManager>();
            }
        }

        UISystem.WinForm mWinForm = null;
        UISystem.WinForm mMenuForm = null;
        public UISystem.StackPanel mRecordShowStackPanel = null;
        public UISystem.StackPanel mRecordSettingStackPanel = null;
        public UISystem.StackPanel mRecordTitleStackPanel = null;
        public UISystem.StackPanel mRecordPerformanceStackPanel = null;
        //UISystem.ToggleButton mToggleButton = null;
        List<RecordV> mRecordChilds = new List<RecordV>();
        bool mOpenOrCloseAllRecord = true;
        //bool mOpenOrCloseRecordVChidrens = true;
        bool mDraged = false;
        bool mIsVerticalMove = true;

        UISystem.WinBase mDragedForm = null;
        UISystem.StackPanel mDragedPanel = null;

        CSUtility.Support.Point mStartPos = CSUtility.Support.Point.Empty;
        CSUtility.Support.Point mMouseClickLocation = CSUtility.Support.Point.Empty;
        CSUtility.Support.Point mDragFormLocation = CSUtility.Support.Point.Empty;
        CSUtility.Support.Point mDragPanelLocation = CSUtility.Support.Point.Empty;

        List<RecordTitle> mRecordTitles = new List<RecordTitle>();

        public void LoadUI()
        {
            mWinForm = CCore.Support.ReflectionManager.Instance.GetUIForm("RecordUI") as UISystem.WinForm;
            if (mWinForm != null)
            {
                //recordForm.Parent = uiForm;
                mRecordShowStackPanel = mWinForm.FindControl("RecordShowStackPanel") as UISystem.StackPanel;
                mRecordSettingStackPanel = mWinForm.FindControl("RecordSettingStackPanel") as UISystem.StackPanel;
                mRecordTitleStackPanel = mWinForm.FindControl("RecordTitleStackPanel") as UISystem.StackPanel;
                mRecordPerformanceStackPanel = mWinForm.FindControl("RecordPerformanceStackPanel") as UISystem.StackPanel;
                //mRecordShowStackPanel.Visibility = UISystem.Visibility.Collapsed;
            }

            mMenuForm = CCore.Support.ReflectionManager.Instance.GetUIForm("RecrodManagerUI") as UISystem.WinForm;
            if (mMenuForm != null)
            {
                mMenuForm.Visibility = UISystem.Visibility.Collapsed;
                var toggle = mMenuForm.FindControl("RecordShowToggleButton") as UISystem.ToggleButton;
                if (toggle != null)
                {
                    toggle.Checked = true;
                    UpdateShowChildVisible(toggle.Checked);
                }

                toggle = mMenuForm.FindControl("RecordSettingToggleButton") as UISystem.ToggleButton;
                if (toggle != null)
                {
                    toggle.Checked = true;
                    UpdateSettingChildVisible(toggle.Checked);
                }

                toggle = mMenuForm.FindControl("RecordTitleToggleButton") as UISystem.ToggleButton;
                if (toggle != null)
                {
                    toggle.Checked = true;
                    UpdateTitleChildVisible(toggle.Checked);
                }

                toggle = mMenuForm.FindControl("RecordPerformanceToggleButton") as UISystem.ToggleButton;
                if (toggle != null)
                {
                    toggle.Checked = true;
                    UpdatePerformanceChildVisible(toggle.Checked);
                }
            }
        }

        public void SetRoot(UISystem.WinBase root)
        {
            if (mWinForm == null)
                return;

            mWinForm.Parent = null;
            mWinForm.Parent = root;
            mMenuForm.Parent = null;
            mMenuForm.Parent = root;
        }

        int RecordIndex = 0;
        public UISystem.WinForm AddRecord(string name, UISystem.WinForm Parent)
        {
            var form = CCore.Support.ReflectionManager.Instance.GetUIForm("Record_ChildUI", string.Format("Record_ChildUI_{0}", RecordIndex)) as UISystem.WinForm;
            if (form == null)
                return null;
            var nameUI = form.FindControl("Name") as UISystem.TextBlock;
            if (nameUI != null)
                nameUI.Text = name;
            var textBlock = form.FindControl("Value") as UISystem.TextBlock;

            form.Tag = textBlock;

            var toggle = form.FindControl("ToggleButton", 1) as UISystem.ToggleButton;
            if (toggle != null)
                toggle.Disable = true;

            //var setForm = CCore.Support.ReflectionManager.Instance.GetUIForm("RecordSetting_Child", string.Format("RecordSetting_Child_{0}", RecordIndex)) as UISystem.WinForm;
            //if (setForm != null)
            //{
            //    var text = setForm.FindControl("TextBlock", 1) as UISystem.TextBlock;
            //    if (text != null)
            //        text.Text = name;
            //    var toggle = setForm.FindControl("ToggleButton", 1) as UISystem.ToggleButton;
            //    if (toggle != null)
            //    {
            //        toggle.Checked = true;
            //    }
            //    UpdateRecordShow(toggle.Checked, form);
            //}
            var recordV = new RecordV(form);
            if (Parent == null)
            {
                form.Parent = mRecordShowStackPanel;
                //if (setForm != null)
                //    setForm.Parent = mRecordSettingStackPanel;
            }
            else
            {
                var data = GetRecordVData(Parent);
                if (data != null)
                {
                    var parentToggle = data.showForm?.FindControl("ToggleButton", 1) as UISystem.ToggleButton;
                    if (parentToggle != null && parentToggle.Disable)
                        parentToggle.Disable = false;
                    data.Childrens.Add(recordV);
                    var panel = Parent.FindControl("ChildStackPanel") as UISystem.StackPanel;
                    form.Parent = panel;
                    //panel = data.settiingForm.FindControl("ChildStackPanel") as UISystem.StackPanel;
                    //if (setForm != null)
                    //    setForm.Parent = panel;
                    recordV.Parent = data;
                }
            }
            mRecordChilds.Add(recordV);
            RecordIndex++;
            return form;
        }

        public void RemoveRecordV(UISystem.WinForm tb)
        {
            tb.Parent = null;
            var data = GetRecordVData(tb);
            mRecordChilds.Remove(data);
            foreach (var child in data.Childrens)
            {
                RemoveRecordV(child.showForm);
            }
            //var data = GetRecordVData(tb);
            //for (int i = 0; i < mRecordChilds.Count; i++)
            //{
            //    if (mRecordChilds[i].showForm == tb)
            //    {
            //        tb.Parent = null;
            //        //mRecordChilds[i].settiingForm.Parent = null;
            //        mRecordChilds.RemoveAt(i);
            //        break;
            //    }
            //}
        }

        private RecordV GetRecordVData(UISystem.WinForm form)
        {
            foreach (var data in mRecordChilds)
            {
                if (data.showForm == form)
                    return data;
            }
            return null;
        }

        class CtrlCache
        {
            public UISystem.TextBlock Text;
            public UISystem.Image Image;
        }
        int RecordTitleIndex = 0;
        public UISystem.WinForm AddRecordTitle(string name, CCore.Graphics.Texture texture, UInt16 width = UInt16.MaxValue, UInt16 height = UInt16.MaxValue, bool isGray = false)
        {
            var form = GetSameRecordTitle(name);
            if (form == null)
            {
                var recordTitle = new RecordTitle(name, texture, width, height);
                form = CCore.Support.ReflectionManager.Instance.GetUIForm("RecordTitle_Child", string.Format("RecordTitle_Child_{0}", RecordTitleIndex++)) as UISystem.WinForm;
                if (form == null)
                    return null;
                recordTitle.WinForm = form;
                mRecordTitles.Add(recordTitle);
                form.Parent = mRecordTitleStackPanel;

                var cache = new CtrlCache();
                cache.Text = form.FindControl("TextBlock", 1) as UISystem.TextBlock;
                cache.Image = form.FindControl("TitleImage") as UISystem.Image;
                form.Tag = cache;
            }

            var ccch = form.Tag as CtrlCache;
            if (ccch == null)
                return null;
            ccch.Text.Text = name;

            var image = ccch.Image;
            if(image!=null)
            {
                if (width != UInt16.MaxValue)
                    image.Width = width;
                if (height != UInt16.MaxValue)
                    image.Height = height;
                image.State.UVAnim = new UISystem.UVAnim();
                image.State.UVAnim.Id = Guid.NewGuid();
                image.State.UVAnim.Texture = CSUtility.Support.IFileConfig.DefaultTextureFile;
                //if(isGray==false)
                    image.State.UVAnim.TechId = Guid.Parse("6968ae97-95bf-44f7-a58e-fbf12978bf53");
                //else
                //    image.State.UVAnim.TechId = Guid.Parse("54a218dc-0c8c-4771-afe1-7569dbb30201");
                var data = image.State.UVAnim.AddFrame();
                data.U = 0;
                data.V = 0;
                data.SizeU = 1;
                data.SizeV = 1;
                data.Scale9Info = new CSUtility.Support.Thickness(0, 0, 0, 0);
                //image.State.UVAnimId = GameData.Support.ConfigFile.Instance.DefaultUVAnimId;
                image.State.Texture = texture;
            }

            return form;
        }

        public void RemoveRecordTitle(UISystem.WinForm tb)
        {
            tb.Parent = null;
            RecordTitle recordTitle = null;
            foreach (var title in mRecordTitles)
            {
                if (title.WinForm == tb)
                {
                    recordTitle = title;
                    break;
                }
            }
            if (recordTitle != null)
                mRecordTitles.Remove(recordTitle);
        }

        UISystem.WinForm GetSameRecordTitle(string name)
        {
            foreach (var title in mRecordTitles)
            {
                if (title.Name == name)
                    return title.WinForm;
            }
            return null;
        }

        int RecordPerformanceIndex = 0;
        public UISystem.WinForm AddRecordPerformance(string name)
        {
            var form = CCore.Support.ReflectionManager.Instance.GetUIForm("RecordPerformance_Child", string.Format("RecordPerformance_Child_{0}", RecordPerformanceIndex++)) as UISystem.WinForm;
            if (form == null)
                return null;
            var nameUI = form.FindControl("Name") as UISystem.TextBlock;
            if (nameUI != null)
                nameUI.Text = name;
            var button = form.FindControl("Button", 1) as UISystem.Button;
            if (button != null)
                button.Tag = name;
            var text = form.FindControl("TextHint") as UISystem.TextBlock;
            if (text != null)
            {
                var enabled = ChangePerformanceValue(name, false);
                if (enabled)
                    text.Text = "Disable";
                else
                    text.Text = "Enabled";
            }
            form.Parent = mRecordPerformanceStackPanel;
            return form;
        }

        public void RemoveRecordPerformance(UISystem.WinForm tb)
        {
            tb.Parent = null;
        }

        void UpdateRecordShow(bool isShowRecordV, UISystem.WinForm form)
        {
            if (form == null)
                return;

            if (isShowRecordV)
            {
                form.Visibility = UISystem.Visibility.Visible;
            }
            else
            {
                form.Visibility = UISystem.Visibility.Collapsed;
                var point = mRecordShowStackPanel.AbsRect.Location;
                var pt = form.AbsToLocal(ref point);
                pt.Y = 0;
                mRecordShowStackPanel.MoveWin(ref pt);
            }
        }

        public void SetValue(UISystem.WinForm form, string value)
        {
            if (form == null)
                return;
            var textBlock = form.Tag as UISystem.TextBlock;
            textBlock.Text = value;
        }

        bool mStartAction = false;
        float mActionSpeed = 30.0f;
        public void Tick()
        {
            if (mStartAction)
            {
                if (!CanMove || mDragedForm == null || mDragedPanel == null)
                {
                    mStartAction = false;
                    return;
                }
                CSUtility.Support.Point ptMouse;
                double distance;
                UISystem.WinBase dragedWinBase = null;
                if (!mIsVerticalMove)
                {
                    dragedWinBase = mDragedForm;
                    ptMouse = ((UISystem.WinBase)mDragedForm.Parent).AbsToLocal(mDragedForm.AbsRect.X, mDragedForm.AbsRect.Y);
                    distance = Math.Sqrt(Math.Pow(Math.Abs(ptMouse.X - mStartPos.X), 2) + Math.Pow(Math.Abs(ptMouse.Y - mStartPos.Y), 2));
                    if (distance >= 200)
                    {
                        CanMove = false;
                        mStartAction = false;
                        mDragedForm.MoveWin(ref mStartPos);
                        mDragedForm.Visibility = UISystem.Visibility.Collapsed;
                        return;
                    }
                }
                else
                {
                    dragedWinBase = mDragedPanel;
                    ptMouse = ((UISystem.WinBase)mDragedPanel.Parent).AbsToLocal(mDragedPanel.AbsRect.X, mDragedPanel.AbsRect.Y);
                    distance = Math.Sqrt(Math.Pow(Math.Abs(ptMouse.X - mStartPos.X), 2) + Math.Pow(Math.Abs(ptMouse.Y - mStartPos.Y), 2));
                }

                if (distance <= mActionSpeed)
                {
                    mDraged = false;
                    CanMove = false;
                    mStartAction = false;
                    ptMouse = mStartPos;
                }
                else
                {
                    var star = new SlimDX.Vector2(mStartPos.X, mStartPos.Y);
                    var dir = new SlimDX.Vector2(ptMouse.X - mStartPos.X, ptMouse.Y - mStartPos.Y);
                    dir.Normalize();
                    var pos = star + dir * (float)(distance - mActionSpeed);
                    ptMouse.X = (int)pos.X;
                    ptMouse.Y = (int)pos.Y;
                }

                dragedWinBase.MoveWin(ref ptMouse);
            }
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void CloseOrOpenRecord(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;

            var toggle = sender as UISystem.ToggleButton;
            if (toggle == null)
                return;

            UpdateShowChildVisible(toggle.Checked);
        }

        void UpdateShowChildVisible(bool isChecked)
        {
            if (mRecordShowStackPanel == null)
                return;

            if (isChecked)
                ((UISystem.WinBase)mRecordShowStackPanel.Parent).Visibility = UISystem.Visibility.Collapsed;
            else
                ((UISystem.WinBase)mRecordShowStackPanel.Parent).Visibility = UISystem.Visibility.Visible;
        }

        void UpdateSettingChildVisible(bool isChecked)
        {
            if (mRecordSettingStackPanel == null)
                return;

            if (isChecked)
                ((UISystem.WinBase)mRecordSettingStackPanel.Parent).Visibility = UISystem.Visibility.Collapsed;
            else
                ((UISystem.WinBase)mRecordSettingStackPanel.Parent).Visibility = UISystem.Visibility.Visible;
        }

        void UpdateTitleChildVisible(bool isChecked)
        {
            if (mRecordTitleStackPanel == null)
                return;

            if (isChecked)
                ((UISystem.WinBase)mRecordTitleStackPanel.Parent).Visibility = UISystem.Visibility.Collapsed;
            else
                ((UISystem.WinBase)mRecordTitleStackPanel.Parent).Visibility = UISystem.Visibility.Visible;
        }

        void UpdatePerformanceChildVisible(bool isChecked)
        {
            if (mRecordPerformanceStackPanel == null)
                return;

            if (isChecked)
                ((UISystem.WinBase)mRecordPerformanceStackPanel.Parent).Visibility = UISystem.Visibility.Collapsed;
            else
                ((UISystem.WinBase)mRecordPerformanceStackPanel.Parent).Visibility = UISystem.Visibility.Visible;
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void CloseOrOpenSettingRecord(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;
            var toggle = sender as UISystem.ToggleButton;
            if (toggle == null)
                return;

            UpdateSettingChildVisible(toggle.Checked);
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void CloseOrOpenTitleRecord(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;
            var toggle = sender as UISystem.ToggleButton;
            if (toggle == null)
                return;

            UpdateTitleChildVisible(toggle.Checked);
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void CloseOrOpenPerformanceRecord(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;
            var toggle = sender as UISystem.ToggleButton;
            if (toggle == null)
                return;

            UpdatePerformanceChildVisible(toggle.Checked);
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void HidOrShowRecord(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;

            var toggle = sender as UISystem.ToggleButton;
            if (toggle == null)
                return;
            var form = toggle.GetRoot(typeof(UISystem.WinForm)) as UISystem.WinForm;
            if (form == null)
                return;

            form.Visibility = UISystem.Visibility.Collapsed;
            //foreach (var data in mRecordChilds)
            //{
            //    if (data.settiingForm == form)
            //    {
            //        SetRecordVisible(toggle.Checked, data);
            //        break;
            //    }
            //}
        }
        [CSUtility.Editor.UIEditor_BindingMethod]
        public void DisableOrUseRecordBlur(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;
            var button = sender as UISystem.Button;
            if (button == null)
                return;
            var name = button.Tag.ToString();

            bool enabled = ChangePerformanceValue(name);
            var text = (sender.GetRoot(typeof(UISystem.WinForm)) as UISystem.WinForm).FindControl("TextHint") as UISystem.TextBlock;
            if (text != null)
            {
                if (enabled)
                    text.Text = "Disable";
                else
                    text.Text = "Enabled";
            }
        }
        bool mDoFSBlur = true;
        bool mDoCopyPre = true;
        bool mDoFSPostBlur = true;
        public bool mDebugDP = false;
        bool ChangePerformanceValue(string name, bool changed = true)
        {
            bool enabled = true;
            switch (name)
            {
                case "Light":
                    if (changed)
                        CCore.Light.Light.IsCommitLight = !CCore.Light.Light.IsCommitLight;
                    enabled = CCore.Light.Light.IsCommitLight;
                    break;
                case "ColorGrading":
                    {
                        var postProcess = CCore.Engine.Instance.Client.MainWorld.PostProceses;
                        foreach (var i in postProcess)
                        {
                            if (i.Name == "色调曲线")
                            {
                                if (changed)
                                    i.Enable = !i.Enable;
                                enabled = i.Enable;
                            }
                        }
                    }
                    break;
                case "Bloom":
                    {
                        var postProcess = CCore.Engine.Instance.Client.MainWorld.PostProceses;
                        foreach (var i in postProcess)
                        {
                            if (i.Name == "GLOW")
                            {
                                if (changed)
                                    i.Enable = !i.Enable;
                                enabled = i.Enable;
                            }
                        }
                    }
                    break;
                case "HDR":
                    {
                        var postProcess = CCore.Engine.Instance.Client.MainWorld.PostProceses;
                        foreach (var i in postProcess)
                        {
                            if (i.Name == "HDR")
                            {
                                if (changed)
                                    i.Enable = !i.Enable;
                                enabled = i.Enable;
                            }
                        }
                    }
                    break;
                case "FSBlur":
                    {
                        if (changed)
                        {
                            mDoFSBlur = !mDoFSBlur;
                            Client.Game.Instance.REnviroment.DoFSBlur = mDoFSBlur;
                        }
                        enabled = mDoFSBlur;
                    }
                    break;
                case "FSCopyPre":
                    {
                        if (changed)
                        {
                            mDoCopyPre = !mDoCopyPre;
                            Client.Game.Instance.REnviroment.DoCopyPreFinal = mDoCopyPre;
                        }
                        enabled = mDoCopyPre;
                    }
                    break;
                case "FAlpha":
                    {
                        if (changed)
                            CCore.Mesh.Mesh.IsCommitFSTranslucent = !CCore.Mesh.Mesh.IsCommitFSTranslucent;
                        enabled = CCore.Mesh.Mesh.IsCommitFSTranslucent;
                    }
                    break;
                case "FSPostBlur":
                    {
                        if (changed)
                        {
                            mDoFSPostBlur = !mDoFSPostBlur;
                            Client.Game.Instance.REnviroment.DoFSPostBlur = mDoFSPostBlur;
                        }
                        enabled = mDoFSPostBlur;
                    }
                    break;
                //case "PostProcess":
                //    {
                //        Client.Game.Instance.REnviroment.DoPostProcess = !Client.Game.Instance.REnviroment.DoPostProcess;
                //        enabled = Client.Game.Instance.REnviroment.DoPostProcess;
                //    }
                //    break;
                case "HitProxy":
                    {
                        if (changed)
                            Client.Game.Instance.REnviroment.DoHitProxy = !Client.Game.Instance.REnviroment.DoHitProxy;
                        enabled = Client.Game.Instance.REnviroment.DoHitProxy;
                    }
                    break;
                case "Shadow":
                    {
                        if (changed)
                            CCore.World.World.IsShadow = !CCore.World.World.IsShadow;
                        enabled = CCore.World.World.IsShadow;
                    }
                    break;
                case "DebugDP":
                    {
                        if (changed)
                        {
                            mDebugDP = !mDebugDP;
                            if(mDebugDP)
                                CCore.Engine.Instance.Client.Graphics.SetDrawSubsetCallBack(OnDrawSubset);
                            else
                                CCore.Engine.Instance.Client.Graphics.SetDrawSubsetCallBack(null);
                        }
                        
                        enabled = mDebugDP;
                    }
                    break;
            }
            return enabled;
        }

        static CCore.DllImportAPI.Delegate_FOnDrawSubset OnDrawSubset = OnDrawSubsetCallBack;
        static string mTMaterial;
        static string mTMaxSourceFile;
        static string mTMaxShader;
        static int mTMaxTriCount;
        static int mTMaxInstance;
        static int mTMaxTime = 0;
        public static void OnDrawSubsetCallBack(int time, string material, string sourceFile, string shader, int triCount, int instance)
        {
            if(instance>1)
            {
                if(time>1500000)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Block Time:{0},Material:{1},Tri={2},Instance={3}", time, material, triCount, instance));
                }
                if(time>mTMaxTime)
                {
                    mTMaxTime = time;
                    mTMaterial = material;
                    mTMaxSourceFile = sourceFile;
                    mTMaxShader = shader;
                    mTMaxTriCount = triCount;
                    mTMaxInstance = instance;
                    System.Diagnostics.Debug.WriteLine(string.Format("Block Time:{0},Material:{1},Tri={2},Instance={3}", time, material, triCount, instance));
                }
            }
        }

        private void SetRecordVisible(bool isShowRecordV, RecordV data)
        {
            if (isShowRecordV)
            {
                if (data.Parent != null)
                {
                    //var toggle = data.Parent.settiingForm?.FindControl("ToggleButton", 1) as UISystem.ToggleButton;
                    //if (toggle != null)
                    //{
                    //    toggle.Checked = isShowRecordV;
                    //    SetRecordVisible(isShowRecordV, data.Parent);
                    //}
                }
            }
            else
            {
                foreach (var child in data.Childrens)
                {
                    //var toggle = child.settiingForm?.FindControl("ToggleButton", 1) as UISystem.ToggleButton;
                    //if (toggle != null)
                    //{
                    //    toggle.Checked = isShowRecordV;
                    //    SetRecordVisible(isShowRecordV, child);
                    //}
                }
            }

            UpdateRecordShow(isShowRecordV, data.showForm);
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void HidOrShowAllRecord(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;

            var text = (sender.Parent as UISystem.WinBase).FindControl("TextBlock", 1) as UISystem.TextBlock;
            if (text == null)
                return;

            mOpenOrCloseAllRecord = !mOpenOrCloseAllRecord;
            if (mOpenOrCloseAllRecord)
            {
                text.Text = "CloseAll";
            }
            else
            {
                text.Text = "OpenAll";
            }
            foreach (var data in mRecordChilds)
            {
                if (mOpenOrCloseAllRecord)
                    data.showForm.Visibility = UISystem.Visibility.Visible;
                else
                    data.showForm.Visibility = UISystem.Visibility.Collapsed;
                //var toggle = data.settiingForm.FindControl("ToggleButton", 1) as UISystem.ToggleButton;
                //if (toggle != null)
                //{
                //    toggle.Checked = mOpenOrCloseAllRecord;
                //    UpdateRecordShow(toggle.Checked, data.showForm);
                //}
            }
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void OpenOrCloseRecordVChilds(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;
            var form = sender.GetRoot(typeof(UISystem.WinForm)) as UISystem.WinForm;
            if (form == null)
                return;
            var panel = form.FindControl("ChildStackPanel") as UISystem.StackPanel;
            if (panel == null)
                return;
            var toggle = sender as UISystem.ToggleButton;
            if (toggle == null)
                return;
            //mOpenOrCloseRecordVChidrens = !mOpenOrCloseRecordVChidrens;
            if (toggle.Checked)
                panel.Visibility = UISystem.Visibility.Visible;
            else
                panel.Visibility = UISystem.Visibility.Collapsed;
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void OpenRecordManagerUI(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;
            if (mMenuForm.Visibility != UISystem.Visibility.Visible)
                mMenuForm.Show();
            else
                mMenuForm.Close(init.BehaviorId);
        }

        //[CSUtility.Editor.UIEditor_BindingMethod]
        //public void RecordVStackPanelMouseButtonDown(UISystem.WinBase sender, UISystem.Message.MouseEventArgs e)
        //{

        //}

        //[CSUtility.Editor.UIEditor_BindingMethod]
        //public void RecordVStackPanelMouseButtonUp(UISystem.WinBase sender, UISystem.Message.MouseEventArgs e)
        //{

        //}

        //[CSUtility.Editor.UIEditor_BindingMethod]
        //public void RecordVStackPanelMouseMove(UISystem.WinBase sender, UISystem.Message.MouseEventArgs e)
        //{

        //}

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void RecordVFormMouseButtonDown(UISystem.WinBase sender, UISystem.Message.MouseEventArgs e)
        {
            if (sender == null)
                return;

            if (mStartAction)
                return;
            
            mDragedForm = sender as UISystem.WinForm;
            mDragedPanel = mRecordShowStackPanel;
            mStartPos = ((UISystem.WinBase)mDragedForm.Parent).AbsToLocal(mDragedForm.AbsRect.Location.X, mDragedForm.AbsRect.Location.Y);
            mDragFormLocation = mDragedForm.AbsToLocal(e.X, e.Y);
            mDragPanelLocation = mDragedPanel.AbsToLocal(e.X, e.Y);
            mMouseClickLocation = mWinForm.AbsToLocal(e.X, e.Y);

            mDraged = true;

            UISystem.Device.Mouse.Instance.Capture(mDragedForm, 0);
            e.Handled = true;
        }

        //UISystem.WinForm GetRootParentForm(UISystem.WinForm form)
        //{
        //    var data = GetRecordVData(form);
        //    if (data.Parent == null)
        //        return form;
        //    return GetRootParentForm(data.Parent.showForm);
        //}

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void RecordVFormMouseButtonUp(UISystem.WinBase sender, UISystem.Message.MouseEventArgs e)
        {
            if (sender == null)
                return;

            if (mStartAction)
                return;

            mDraged = false;
            FirstMove = true;
            mStartAction = true;
            UISystem.Device.Mouse.Instance.ReleaseCapture(0);
            e.Handled = true;
        }

        bool FirstMove = true;
        bool CanMove = false;
        [CSUtility.Editor.UIEditor_BindingMethod]
        public void RecordVFormMouseMove(UISystem.WinBase sender, UISystem.Message.MouseEventArgs e)
        {
            if (sender == null)
                return;
            
            if (mDraged && !mStartAction)
            {
                if (FirstMove)
                {
                    var mouseClickPos = mWinForm.AbsToLocal(e.X, e.Y);
                    var distance = Math.Sqrt(Math.Pow(Math.Abs(mouseClickPos.X - mMouseClickLocation.X), 2) + Math.Pow(Math.Abs(mouseClickPos.Y - mMouseClickLocation.Y), 2));
                    if (distance > 10)
                    {
                        CanMove = true;
                        FirstMove = false;
                        if (Math.Abs(mouseClickPos.X - mMouseClickLocation.X) > Math.Abs(mouseClickPos.Y - mMouseClickLocation.Y))
                            mIsVerticalMove = false;
                        else
                            mIsVerticalMove = true;
                    }
                }
                if (CanMove)
                {
                    if (!mIsVerticalMove)
                    {
                        var ptMouse = ((UISystem.WinBase)mDragedForm.Parent).AbsToLocal(e.X, e.Y);

                        ptMouse.X -= mDragFormLocation.X;
                        ptMouse.Y = ((UISystem.WinBase)mDragedForm.Parent).AbsToLocal(mDragedForm.AbsRect.X, mDragedForm.AbsRect.Y).Y;

                        mDragedForm.MoveWin(ref ptMouse);
                    }
                    else
                    {
                        var parentBase = (UISystem.WinBase)mDragedPanel.Parent;
                        var ptMouse = ((UISystem.WinBase)mDragedPanel.Parent).AbsToLocal(e.X, e.Y);
                        var panelPos = parentBase.AbsToLocal(mDragedPanel.AbsRect.X, mDragedPanel.AbsRect.Y);
                        ptMouse.X = panelPos.X;
                        ptMouse.Y -= mDragPanelLocation.Y;
                        mStartPos.X = ptMouse.X;
                        if (mDragedPanel.Height > parentBase.Height)
                        {
                            if (ptMouse.Y > 0)
                                mStartPos.Y = 0;
                            else if (ptMouse.Y + mDragedPanel.Height < parentBase.Height)
                                mStartPos.Y = parentBase.Height - mDragedPanel.Height;
                            else
                                mStartPos.Y = ptMouse.Y;
                        }
                        else
                        {
                            mStartPos.Y = 0;
                        }
                        //if (mDragedPanel.Height > parentBase.Height)
                        //{
                        //    if (ptMouse.Y > 0)
                        //        ptMouse.Y = 0;
                        //    else if (ptMouse.Y + mDragedPanel.Height < parentBase.Height)
                        //        ptMouse.Y = parentBase.Height - mDragedPanel.Height;
                        //}
                        //else
                        //{
                        //    ptMouse.Y = 0;
                        //}

                        mDragedPanel.MoveWin(ref ptMouse);
                    }
                }
            }
        }
    }
}
