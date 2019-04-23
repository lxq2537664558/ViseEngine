using Server.Hall.Map;
using System;
using System.Collections.Generic;
using System.Text;
using GameData.Skill;

namespace Server.Hall.Role.Monster
{
    class LockOnSelector
    {
        public Role.RoleActor LockOnTarget = null;
        public float Distance = float.MaxValue;
    }

    public class MonsterInstance : RoleActor
    {
        GameData.Role.MonsterData  mMonsterData=null;
        [CSUtility.Event.Attribute.AllowMember("怪物.怪物数据", CSUtility.Helper.enCSType.Server, "怪物数据")]
        public GameData.Role.MonsterData MonsterData
        {
            get
            {
                return mMonsterData;
            }
            set
            {
                mMonsterData = value;
                mMonsterData._SetHostNpc(this);
            }
        }

        ~MonsterInstance()
        {
            MonsterInstanceNumber--;
        }

        public override int FactionId
        {
            get
            {
                return MonsterData.MonsterFaction;
            }
        }

        public bool InitMonster(GameData.Role.MonsterData data)
        {
            base.InitActor();
            this._SetId( data.RoleId );
            data._SetHostNpc(this);
            MonsterData = data;
            if (string.IsNullOrEmpty(data.Name))
                MonsterData.Name = data.Template.NickName;
            else
                MonsterData.Name = data.Name;

            mPlacement = new Role.RolePlacement(this);
            MonsterData.RoleMoveSpeed = MonsterData.RoleTemplate.MoveSpeed;
            MonsterData.MonsterFaction =data.RoleTemplate.FactionId;

            #region 技能初始化
            mMonsterData.Skills.Clear();
            var roleTemplate = mMonsterData.RoleTemplate;
            if (roleTemplate != null)
            {
                foreach (var info in roleTemplate.SkillInfos)
                {
                    var skill = new GameData.Skill.SkillData();
                    skill.TemplateId = info.SkillId;
                    skill.SkillLevel = (UInt16)(info.SkillLevel > 0 ? info.SkillLevel : 1);
                    if (skill.Template != null)
                        mMonsterData.Skills.Add(skill);
                }
                if (mMonsterData.Skills.Count == 0)
                {
                    var skill = new GameData.Skill.SkillData();
                    skill.TemplateId = 0;
                    skill.SkillLevel = 1;
                    if (skill.Template != null)
                        mMonsterData.Skills.Add(skill);
                }
            }

            #endregion

            return true;
        }

        #region 重载

        public override string RoleName
        {
            get { return RoleTemplate.RoleName; }
        }

        public override GameData.Role.RoleTemplate RoleTemplate
        {
            get { return MonsterData.RoleTemplate; }
        }

        public override int RoleMaxHP
        {
            get { return MonsterData.MaxRoleHp; }
            set { MonsterData.MaxRoleHp = value; }
        }

        public override int RoleHP
        {
            get{return MonsterData.RoleHp;}
            set{MonsterData.RoleHp = value;}
        }

        public override int RoleMaxMP
        {
            get { return MonsterData.MaxRoleMp; }
            set { MonsterData.MaxRoleMp = value; }
        }

        public override int RoleMP
        {
            get { return MonsterData.RoleMp; }
            set { MonsterData.RoleMp= value; }
        }

        public override void OnPlacementUpdatePosition(ref SlimDX.Vector3 pos)
        {
            base.OnPlacementUpdatePosition(ref pos);

            MonsterData.Position = pos;

            HostMap.TourRoles((Int32)GameData.Role.ERoleType.Trigger, ref pos, 20, this.OnVisitRole_ProcessTrigger, null);
        }

        public override void OnPlacementUpdateDirectionY(float angle)
        {
            base.OnPlacementUpdateDirectionY(angle);
            
        }

        public override void OnEnterMap(MapInstance map)
        {
            //基类已经把角色放到SceneGraph里面去
            base.OnEnterMap(map);

            map.AddMonster(this);
        }

        public override void DangrouseOnLeaveMap()
        {
            if (!HostMap.IsNullMap)
                HostMap.RemoveMonster(this);

            base.DangrouseOnLeaveMap();
        }

        public override void TargetDeath(RoleActor target, int index)
        {
            base.TargetDeath(target, index);
        }

        public override void ResetDefaultState()
        {
            base.ResetDefaultState();
            
        }

        public override void FreshRoleValue(bool freshhpmp)
        {
            base.FreshRoleValue(freshhpmp);
        }

        public override void ProcDeath()
        {
            base.ProcDeath();
            if (MonsterData.RoleTemplate.Reborn)
            {
                TimerManager.AddLogicTimer("复活计时器", MonsterData.RoleTemplate.RebornElapsed, Reborn, CSUtility.Helper.enCSType.Server);
            }                        
        }

        private bool Reborn(string name)
        {
            TimerManager.RemoveLogicTimer("复活计时器");
            
            //this.CurrentState.ToState("Idle", null);
            this.ResetDefaultState();
            Placement.SetLocation(MonsterData.OriPosition);            
            return true;
        }

        public override void TickSkillCD(long elapseMillsecond)
        {
            base.TickSkillCD(elapseMillsecond);
            foreach (var skill in MonsterData.Skills)
            {
                if (skill.RemainCD > 0)
                    skill.RemainCD -= (float)elapseMillsecond / 1000f;
                if (skill.RemainCD < 0)
                    skill.RemainCD = 0;
            }
        }

        public override SkillData FindSkillData(UInt16 templateId)
        {
            foreach (var skill in MonsterData.Skills)
            {
                if (skill.TemplateId == templateId)
                    return skill;
            }
            return null;
        }

        //long mEnterStayAttDu = 1000;
        bool mIsDoAttackTarget = false;
        public override void Tick(long elapseMillsecond)
        {
            base.Tick(elapseMillsecond);

//             mEnterStayAttDu -= elapseMillsecond;
//             if (CurrentState != null)
//             {
//                 if (CurrentState.StateName == "Idle" || CurrentState.StateName == "Walk")
//                 {
//                     if (mEnterStayAttDu < 0)
//                     {
//                         mEnterStayAttDu = 500;
//                         mIsDoAttackTarget = DoAttackCurTargetAIBehavior();
//                         //临时函数
//                         Dowander(elapseMillsecond);
//                     }
//                 }
//             }            

        }

        public override void TickBloodReturn(long elapseMillsecond)
        {
            base.TickBloodReturn(elapseMillsecond);
            if (IsEnterHotSpring)
            {
                if (RoleTemplate.MonsterType == GameData.Role.MonsterType.Normal)
                {
                    RoleHP += (int)(((float)elapseMillsecond / 1000.0f) * 50);
                    if (RoleHP > RoleMaxHP)
                        RoleHP = RoleMaxHP;
                }
            }
        }

        [CSUtility.AISystem.Attribute.AllowMember("怪物.寻路", CSUtility.Helper.enCSType.Server, "怪物.寻路")]
        public void Dowander()
        {
            if (mIsDoAttackTarget)
                return;

            if (this.CurrentState.StateName == "Idle")
            {                
                if (this.AIStates.GetState("Walk") == null)
                    return;
                var faction = CSUtility.Data.DataTemplateManager<UInt16, GameData.Faction>.Instance.GetDataTemplate((UInt16)this.FactionId);// GameData.FactionManager.Instance.FindFaction((UInt16)this.FactionId);
                if (faction != null)
                {
                    DoWalkToTargetPos(faction.EndPoint.X,faction.EndPoint.Z);
                }        
            }
        }

        public override void OnEnterState(string name)
        {
            base.OnEnterState(name);   
        }

        public override void SetRoleAttackImider()
        {
            //mEnterStayAttDu = 0;
        }

        public override bool GetRoleAttachFighting()
        {
            return true;
        }

        public override bool CanLockon(RoleActor target)
        {
            return base.CanLockon(target);
        }

        public override bool CanAttack(RoleActor target)
        {            
            return base.CanAttack(target);
        }

        public override RoleActor FindTargetTemplateRole()
        {
            return base.FindTargetTemplateRole();
        }

        #endregion

        #region 创建

        public static int MonsterInstanceNumber = 1;//这是一个RPC对象，有一个不通过CreatePlayerInstance创建的，然后销毁了
        [CSUtility.AISystem.Attribute.AllowMember("角色对象.召唤怪物", CSUtility.Helper.enCSType.Server, "召唤怪物(AutoCreateMonster)")]
        public static Role.Monster.MonsterInstance AutoCreateMonster(ushort tempid, float posx, float posz, Hall.HallInstance halls, Map.MapInstance map)
        {
            var data = new GameData.Role.MonsterData();
            data.TemplateId = tempid;
            data.RoleId = Guid.NewGuid();
            var pos = new SlimDX.Vector3(posx, 0, posz);
            data.OriPosition = pos;
            var role = Role.Monster.MonsterInstance.CreateMonsterInstance(data, halls, map);
            if(role !=null)
                role.RoleProcDeath += role.DoLeaveMap;//死亡移除地图

            return role;
        }

        public static MonsterInstance CreateMonsterInstance(GameData.Role.MonsterData md, Hall.HallInstance halls, Map.MapInstance map)
        {
            if (md == null || md.Template == null)
                return null;

            if (md.AIGuid == Guid.Empty && md.RoleTemplate.AIGuid == Guid.Empty)
                md.AIGuid = CSUtility.Support.IFileConfig.DefaultAI;

            CSUtility.Component.ActorInitBase actInit = new CSUtility.Component.ActorInitBase();
            actInit.GameType = (UInt16)CSUtility.Component.EActorGameType.Npc;
            MonsterInstance ret = new MonsterInstance();
            ret.Initialize(actInit);
            ret.HallInstance = halls;
            ret.RoleCreateType = GameData.Role.ERoleType.Monster;
            ret.OwnerRole = ret;
            ret.InitMonster(md);

            //AI
            if (md.AIGuid == Guid.Empty)//如果怪物没有Ai,就用模板里面的默认的
            {
                md.AIGuid = ret.RoleTemplate.AIGuid;
            }
            if (false == ret.InitFSM(md.AIGuid, true))
            {
                Log.FileLog.WriteLine(string.Format("NPC {0} Create Failed", md.Template.NickName));
                return null;
            }

            ret.ResetDefaultState();

            if (ret.CurrentState == null)
            {
                Log.FileLog.WriteLine(string.Format("NPC {0} Create Failed CurrentState==null", md.Template.NickName));
                return null;
            }
            if (ret.CurrentState != null)
                ret.CurrentState.OnRoleCreatEnd();
            ret.OnEnterMap(map);
            ret.FreshRoleValue(true);
            var loc = md.OriPosition;
            loc.Y = ret.GetAltitude(loc.X,loc.Z);
            ret.Placement.SetLocation(ref loc);
            var scale = new SlimDX.Vector3(md.RoleTemplate.Scale);
            ret.Placement.SetScale(ref scale);
            ret.Placement.SetRotationY(md.Direction, md.RoleTemplate.MeshFixAngle);
            MonsterInstanceNumber++;
            if (ret != null && !ret.MonsterData.RoleTemplate.Reborn)
                ret.RoleProcDeath += ret.DoLeaveMap;//死亡移除地图
            return ret;
        }

        #endregion        
    }
}
