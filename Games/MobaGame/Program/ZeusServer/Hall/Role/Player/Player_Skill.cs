using System;
using System.Collections.Generic;
using System.Text;
using GameData.Skill;

namespace Server.Hall.Role.Player
{
    public partial class PlayerInstance : RPC.RPCObject
    {
        #region 技能

        public override SkillData FindSkillData(ushort templateId)
        {
            foreach (var sd in PlayerData.Skills)
            {
                if (templateId == sd.TemplateId)
                {
                    return sd;
                }
            }
            return null;
        }

        public bool IsCDMPReady(ushort skillId)
        {
            var skill = this.FindSkillData(skillId);
            if (skill == null || skill.SkillLevel == 0)
                return false;

            //return true;
            //     System.Diagnostics.Debug.WriteLine(string.Format("RemainCD:{0}", skill.SkillData.RemainCD));
            if (skill.RemainCD <= skill.Template.SkillLevelDatas[skill.SkillLevel - 1].CD / 9)//cd/9 的容差，考虑各种网络延迟时间不对称
            {
                if (PlayerData.RoleMp >= skill.Template.SkillLevelDatas[skill.SkillLevel - 1].SkillConsumeMP)
                    return true;
                else
                {
                    //魔法不足
                }
            }
            return false;
        }

        public bool FreshRoleCDMP(ushort skillId)
        {
            var skill = this.FindSkillData(skillId);
            if (skill == null || skill.Template == null || skill.Template.SkillLevelDatas.Count < skill.SkillLevel || skill.SkillLevel == 0)
            {
                return false;
            }

            int conMp = (int)(float)skill.Template.SkillLevelDatas[skill.SkillLevel - 1].SkillConsumeMP;
            if (PlayerData.RoleMp < conMp)
                return false;
            PlayerData.RoleMp -= conMp;
            skill.RemainCD = skill.Template.SkillLevelDatas[skill.SkillLevel - 1].CD;
            return true;
        }
        
        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_SkillLevelUp(int index, RPC.RPCForwardInfo fwd)
        {
            RPC.PackageWriter retPkg = new RPC.PackageWriter();
            if (PlayerData.Skills.Count <= index)
            {
                retPkg.Write((sbyte)-1);    //服务器没有这个技能，客户端可能作弊
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }
            var skill = PlayerData.Skills[index];
            if (skill.Template.SkillLevelDatas.Count <= skill.SkillLevel || skill.Template.SkillLevelDatas[skill.SkillLevel].LevelupNeedRoleLevel > PlayerData.RoleLevel)
            {
                retPkg.Write((sbyte)-2);    //该技能等级已经到达上限或该技能下个等级升级需求角色等级不足，无法升级
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }
            if (PlayerData.RoleSkillPoint < 1)
            {
                retPkg.Write((sbyte)-3);    //该角色没有技能点，无法升级
                retPkg.DoReturnPlanes2Client(fwd);
                return;
            }
            retPkg.Write((sbyte)1);
            skill.SkillLevel++;
            PlayerData.RoleSkillPoint--;
            retPkg.DoReturnPlanes2Client(fwd);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_FireInitiativeSkill(UInt16 skillId, UInt32 lockroleid,SlimDX.Vector3 summonPos, SlimDX.Vector3 tarPos, SlimDX.Vector3 tarDir, float tarAngle, RPC.RPCForwardInfo fwd)
        {
            RPC.PackageWriter retPkg = new RPC.PackageWriter();
            var skill = FindSkillData(skillId);
            if (null == skill)
                return;
            if (skill.Template.SkillType == GameData.Skill.ESkillType.InitiativeSkill)
            {
                if (!IsCDMPReady(skillId)) //不可放
                {
                    retPkg.Write((sbyte)-2);
                    retPkg.DoReturnPlanes2Client(fwd);
                    //Log.FileLog.WriteLine("RPC_FireInitiativeSkill错误:-2");
                    return;
                }
                if (skill.Template.SkillOperation == ESkillOperationType.Skillshot && lockroleid == UInt32.MaxValue)
                {
                    retPkg.Write((sbyte)-3);
                    retPkg.DoReturnPlanes2Client(fwd);
                    return;
                }
                bool success = false;

                FreshRoleCDMP(skillId);//先返回，为了手感
                retPkg.Write((sbyte)1);//成功
                retPkg.DoReturnPlanes2Client(fwd);

     //           if (CurrentState.Parameter.CanInterrupt == true)
                {
                    var remote = this.StateNotify2Remote;
                    StateNotify2Remote = true;
                    var prim = CurrentState.Parameter.IsPrimaryState;
                    CurrentState.Parameter.IsPrimaryState = false;
                    success = DoFireSkill(skillId, tarDir, tarPos, tarAngle, summonPos, lockroleid);
                    CurrentState.Parameter.IsPrimaryState = prim;
                    StateNotify2Remote = remote;
                }
       //         else
       //         {
                    //success = FireBuffSkill(skillId);
        //        }
                if (success == false)
                {
                    Log.FileLog.WriteLine("RPC_FireInitiativeSkill技能施法错误");
                }
            }
            else
            {
                retPkg.Write((sbyte)-4);//被动技能
                retPkg.DoReturnPlanes2Client(fwd);
                Log.FileLog.WriteLine("RPC_FireInitiativeSkill错误:-4");
                return;
            }
        }

        #endregion
    }
}
