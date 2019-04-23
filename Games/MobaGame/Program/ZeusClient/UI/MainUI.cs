using System;
using System.Collections.Generic;
using System.Text;

namespace Client.UI
{
    public class MainUIManager : UISystem.UIBindAutoUpdate
    {
        public static MainUIManager Instance
        {
            get
            {
                return CCore.Support.ReflectionManager.Instance.GetClassObject<MainUIManager>();
            }
        }

        const byte cMaxSkillCount = 4;
        UISystem.WinForm mWinForm = null;
        List<UISystem.Image> mSkillCDImages = new List<UISystem.Image>();
        List<UISystem.TextBlock> mSkillCDTextBlocks = new List<UISystem.TextBlock>();

        public void InitMainUI()
        {
            mWinForm = CCore.Support.ReflectionManager.Instance.GetUIForm("MainUI") as UISystem.WinForm;
            if (mWinForm == null)
                return;
            mWinForm.Parent = Game.Instance.RootUIMsg.Root;
            RecordVManager.Instance.SetRoot(Game.Instance.RootUIMsg.Root);
            //var joysticks = mWinForm.FindControl("Joysticks") as UISystem.Joysticks.Joysticks;
            //if (joysticks != null)
            //{
            //    joysticks.OnValueChanged += Move;
            //    joysticks.OnValueChangedLeave += StopMove;
            //}
            //for (int i = 0; i < cMaxSkillCount; i++)
            //{
            //    var button = mWinForm.FindControl(string.Format("SkillButton{0}", i)) as UISystem.Button;
            //    if (button != null)
            //        button.Tag = i;
            //}
            for (int i = 0; i < Stage.MainStage.Instance.ChiefRole.RoleData.PlayerData.Skills.Count; i++)
            {
                var skill = Stage.MainStage.Instance.ChiefRole.RoleData.PlayerData.Skills[i];
                var image = mWinForm.FindControl(string.Format("SkillCDImage{0}", i)) as UISystem.Image;
                if (image != null)
                {
                    image.Tag = skill;
                    image.State.UVAnim.MaterialObject.SetFloat("time", 1);
                    mSkillCDImages.Add(image);
                }
                var text = mWinForm.FindControl(string.Format("SkillCDText{0}", i)) as UISystem.TextBlock;
                if (text != null)
                {
                    text.Text = "0";
                    text.Visibility = UISystem.Visibility.Collapsed;
                    mSkillCDTextBlocks.Add(text);
                }
            }

            UpdateRoleSkill();
        }

        public void UpdateRoleSkill()
        {
            var chiefRole = Stage.MainStage.Instance.ChiefRole;
            if (chiefRole == null)
                return;

            for (int i = 0; i < chiefRole.RoleData.PlayerData.Skills.Count; i++)
            {
                var button = mWinForm.FindControl(string.Format("SkillButton{0}", i)) as UISystem.Button;
                if (button == null)
                    continue;

                button.Tag = i;
                var skill = chiefRole.RoleData.PlayerData.Skills[i];
                if (skill.SkillLevel > 0)
                {
                    button.Disable = false;
                }
                else
                {
                    button.Disable = true;
                }
                if (button.NormalState.UVAnimId != skill.Template.SkillIcon)
                    button.NormalState.UVAnimId = skill.Template.SkillIcon;
                if (button.PressState.UVAnimId != skill.Template.SkillPressIcon)
                    button.PressState.UVAnimId = skill.Template.SkillPressIcon;
                if (button.DisableState.UVAnimId != skill.Template.SkillDisableIcon)
                    button.DisableState.UVAnimId = skill.Template.SkillDisableIcon;

                var lvUpButton = mWinForm.FindControl(string.Format("SkillLvUpButton{0}", i)) as UISystem.Button;
                if (lvUpButton != null)
                {
                    lvUpButton.Tag = i;
                    //判断该技能下一等级是否可升级
                    if (skill.Template.SkillLevelDatas.Count > skill.SkillLevel && chiefRole.RoleData.PlayerData.RoleSkillPoint > 0 &&
                    skill.Template.SkillLevelDatas[skill.SkillLevel].LevelupNeedRoleLevel <= chiefRole.RoleData.RoleLevel)
                    {
                        lvUpButton.Visibility = UISystem.Visibility.Visible;
                    }
                    else
                    {
                        lvUpButton.Visibility = UISystem.Visibility.Hidden;
                    }
                }

                var text = mWinForm.FindControl(string.Format("SkillLevelText{0}", i)) as UISystem.TextBlock;
                if (text != null)
                {
                    text.Text = skill.SkillLevel.ToString();
                }
            }
            mWinForm.UpdateUI();
        }
        
        public void TickEffect(long milliSecondElapsedTime)
        {
            for (int i = 0; i < mSkillCDImages.Count; i++)
            {
                var skill = mSkillCDImages[i].Tag as GameData.Skill.SkillData;
                if (skill == null)
                    continue;

                UISystem.TextBlock text = null;
                if (mSkillCDTextBlocks.Count > i)
                    text = mSkillCDTextBlocks[i];

                var data = skill.GetSkillLevelTemp();
                float radio = 1;
                if (data != null)
                    radio = 1 - skill.RemainCD / data.CD;
                
                mSkillCDImages[i].State.UVAnim.MaterialObject.SetFloat("time", radio);

                if (text != null)
                {
                    if (radio < 1)
                    {
                        text.Text = ((int)skill.RemainCD + 1).ToString();
                        text.Visibility = UISystem.Visibility.Visible;
                    }
                    else
                    {
                        text.Text = "0";
                        text.Visibility = UISystem.Visibility.Collapsed;
                    }
                }
            }
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void Move(SlimDX.Vector3 dir)
        {
            SlimDX.Matrix matrix = SlimDX.Matrix.Identity;
            Role.ChiefRoleActorController.Instance.CameraController.Camera.GetViewMatrix(ref matrix);
            matrix.Invert();
            dir = SlimDX.Vector3.TransformNormal(dir, matrix);
            Stage.MainStage.Instance.ChiefRole?.ActorController.Move(dir);
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void StopMove()
        {
            if (Stage.MainStage.Instance.ChiefRole == null || Stage.MainStage.Instance.ChiefRole.CurrentState == null)
                return;

            var idle = Stage.MainStage.Instance.ChiefRole.AIStates.GetState("Walk").Parameter as CSUtility.AISystem.States.IWalkParameter;
            Stage.MainStage.Instance.ChiefRole.CurrentState.ToState("Idle", idle);
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void OnFireSkillButtonClick(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;

            var index = Convert.ToInt32(sender.Tag);
            //var button = sender as UISystem.Button;
            //if (button != null)
            //{
            //    var texture = button.NormalState.Texture;
            //    RecordVManager.Instance.AddRecordTitle(string.Format("Skill_{0}", index), texture);
            //}
            OnFireSkill(index);
        }

        void OnFireSkill(int index)
        {
            if (index < 0)
                return;

            var chiefRole = Stage.MainStage.Instance.ChiefRole;
            if (chiefRole == null || chiefRole.RoleData == null || chiefRole.RoleData.PlayerData == null || chiefRole.RoleData.PlayerData.Skills.Count == 0 ||
                chiefRole.RoleData.PlayerData.Skills.Count <= index)
                return;

            //Skill.SkillController.Instance.AddCommand(chiefRole.RoleData.PlayerData.Skills[index]);
            chiefRole.OnFireSkill(chiefRole.RoleData.PlayerData.Skills[index]);
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void SkillLevelUpClick(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            if (sender == null)
                return;

            var index = (int)sender.Tag;
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            Server.Hall.Role.Player.H_PlayerInstance.smInstance.RPC_SkillLevelUp(pkg, index);
            pkg.WaitDoClient2PlanesPlayer(CCore.Engine.Instance.Client.GateSvrConnect, 10).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                    return;

                sbyte successed;
                _io.Read(out successed);
                if (successed == 1)
                {
                    Stage.MainStage.Instance.ChiefRole.RoleData.PlayerData.Skills[index].SkillLevel++;
                    UpdateRoleSkill();
                }
            };
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void OnSelectRole(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            var btn = sender as UISystem.Button;
            if (btn == null)
                return;            

            Stage.MainStage.Instance.RoleTemplateId = Convert.ToUInt16(btn.WinName.Substring(4));
            Game.Instance.SetCurrentStage(Stage.MainStage.Instance);            
        }

        [CSUtility.Editor.UIEditor_BindingMethod]
        public void OnStartGame(UISystem.WinBase sender, CCore.MsgProc.BehaviorParameter init)
        {
            var btn = sender as UISystem.Button;
            if (btn == null)
                return;

            Stage.MainStage.Instance.RoleTemplateId = 1;
            Game.Instance.SetCurrentStage(Stage.MainStage.Instance);
        }

        string mRelifeTimeStr;
        [CSUtility.Editor.UIEditor_BindingProperty]

        public string RelifeTimeStr
        {
            get { return mRelifeTimeStr; }
            set
            {
                mRelifeTimeStr = value;
                OnPropertyChanged("RelifeTimeStr");
            }
        }
        public void ShowDeathUI(bool show)
        {
            var form = CCore.Support.ReflectionManager.Instance.GetUIForm("PlayerDeath") as UISystem.WinForm;
            if (form == null)
                return;
            if (show)
            {
                form.Parent = mWinForm;
            }
            else
            {
                form.Parent = null;
            }
        }
    }
}
