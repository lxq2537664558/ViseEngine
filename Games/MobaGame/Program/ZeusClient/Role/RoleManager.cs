using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Role
{
    class RoleManager
    {
        static RoleManager smInstance = new RoleManager();
        public static RoleManager Instance
        {
            get { return smInstance; }
        }

        Dictionary<UInt32, RoleActor> mTableSingleIds = new Dictionary<UInt32, RoleActor>();
        public Dictionary<UInt32, RoleActor> TableSingleIds
        {
            get { return mTableSingleIds; }
        }

        List<RoleActor> mErrorActors = new List<RoleActor>();
        public void AddErrorActor(RoleActor role)
        {
            mErrorActors.Add(role);
        }

        public void ClearIds()
        {
            foreach (var i in mTableSingleIds)
            {
                if (i.Value.World != null)
                    i.Value.World.RemoveActor(i.Value);
            }
            mTableSingleIds.Clear();
        }

        public RoleActor FindRoleActor(UInt32 i)
        {
            RoleActor role;
            if (mTableSingleIds.TryGetValue(i, out role))
                return role;
            return null;
        }

        public void MapRoleId(RoleActor role, UInt32 singleId)
        {
            if (role == null)
                return;
            role._SetSingleId(singleId);
            RoleActor findActor = null;
            if (mTableSingleIds.TryGetValue(singleId, out findActor))
            {
                Log.FileLog.WriteLine(string.Format("RoleIdManager 重复要求插入角色[{0}:{1}]", role.RoleName, findActor.RoleName));
                if (role == findActor)
                    return;
                if (role.World != null)
                {
                    role.World.RemoveActor(role);
                }
                return;
            }
            mTableSingleIds[singleId] = role;
        }

        public void UnmapRoleId(RoleActor role)
        {
            if (role == null)
                return;
            mTableSingleIds.Remove(role.SingleId);
            role._SetSingleId(0);
        }

        public void Tick()
        {
            //根据Role类型分别处理掉线和死亡的逻辑
//             for (int i = 0; i < mErrorActors.Count; i++)
//             {
//                 if (mErrorActors[i].IsLeaveMap)
//                 {
//                     if (mErrorActors[i].World != null)
//                     {
//                         mErrorActors[i].World.RemoveActor(mErrorActors[i]);
//                         mErrorActors.RemoveAt(i);
//                         i--;
//                     }
//                 }
//             }
            var unmapRoles = new List<RoleActor>();
            foreach (var i in mTableSingleIds)
            {                
                if (i.Value.IsLeaveMap)
                {
                    unmapRoles.Add(i.Value);
                }
            }

            foreach (var i in unmapRoles)
            {
                if (i.World != null)
                    i.World.RemoveActor(i);
                UnmapRoleId(i);
            }
        }

        public void InitRoleActionNames()
        {
            #region 所有动作           
            var actions = new string[]{
                            "Idle",                                          // 休闲待机
                            "Idle1",
                            "Idle2",
                            "Idle3",
                            "Idle4",
                            "FightIdle",                                     // 无武器战斗待机
                            "FightIdle_1H",                                  // 单手武器战斗待机
                            "FightIdle_2H",                                  // 双手武器战斗待机
                            "FightIdle_2HLong",                              // 双手长柄武器战斗待机
                            "FightIdle_2HXBow",                              // 双持弩枪战斗待机
                            "FightIdle_2HBow",                               // 双手弓战斗待机
                            "FightIdle_1HMage",                              // 单手魔杖战斗待机
                            "FightIdle_2HMage",                              // 双手法杖战斗待机
                            "Walk",                                          // 行走
                            "Walk_1H",                                       // 单手武器行走
                            "Walk_2H",                                       // 双手武器行走
                            "Walk_2HLong",                                   // 双手长柄武器行走
                            "Walk_2HXBow",                                   // 双持弩枪行走
                            "Walk_2HBow",                                    // 双手弓行走
                            "Walk_1HMage",                                   // 单手魔杖行走
                            "Walk_2HMage",                                   // 双手法杖行走
                            "Run",                                           // 跑步
                            "Run_1H",                                        // 单手武器跑步
                            "Run_2H",                                        // 双手武器跑步
                            "Run_2HLong",                                    // 双手长柄武器跑步
                            "Run_2HXBow",                                    // 双持弩枪跑步
                            "Run_2HBow",                                     // 双手弓跑步
                            "Run_1HMage",                                    // 单手魔杖跑步
                            "Run_2HMage",                                    // 双手法杖跑步
                            "Death",                                         // 死亡
                            "DefaultAttack_1",                               // 默认普通攻击动作1
                            "DefaultAttack_2",                               // 默认普通攻击动作2
                            "DefaultAttack_3",                               // 默认普通攻击动作3
                            "DefaultAttack_1H_1",                            // 单手武器默认普通攻击1
                            "DefaultAttack_1H_2",                            // 单手武器默认普通攻击2
                            "DefaultAttack_1H_3",                            // 单手武器默认普通攻击3
                            "DefaultAttack_2H_1",                            // 双手武器默认普通攻击1
                            "DefaultAttack_2H_2",                            // 双手武器默认普通攻击2
                            "DefaultAttack_2H_3",                            // 双手武器默认普通攻击3
                            "DefaultAttack_2HLong_1",                        // 双手长柄武器默认普通攻击
                            "DefaultAttack_2HLong_2",                        // 双手长柄武器默认普通攻击
                            "DefaultAttack_2HLong_3",                        // 双手长柄武器默认普通攻击
                            "DefaultAttack_2HXBow_1",                        // 双持弩枪默认普通攻击
                            "DefaultAttack_2HXBow_2",                        // 双持弩枪默认普通攻击
                            "DefaultAttack_2HXBow_3",                        // 双持弩枪默认普通攻击
                            "DefaultAttack_2HBow_1",                         // 双手弓默认普通攻击
                            "DefaultAttack_2HBow_2",                         // 双手弓默认普通攻击
                            "DefaultAttack_2HBow_3",                         // 双手弓默认普通攻击
                            "DefaultAttack_1HMage_1",                        // 单手魔杖默认普通攻击
                            "DefaultAttack_1HMage_2",                        // 单手魔杖默认普通攻击
                            "DefaultAttack_1HMage_3",                        // 单手魔杖默认普通攻击
                            "DefaultAttack_2HMage_1",                          // 双手法杖默认普通攻击
                            "DefaultAttack_2HMage_2",                          // 双手法杖默认普通攻击
                            "DefaultAttack_2HMage_3",                          // 双手法杖默认普通攻击
                            "DefaultChannel",                                // 默认吟唱
                            "StayAttack",                                    // 默认施法
                            "CastBuff",                                      // 施放BUFF
                            "BeAttack",                                      // 无装备武器被击中
                            "BeAttack_1H",                                   // 单手武器武器被击中
                            "BeAttack_2H",                                   // 双手武器武器被击中
                            "BeAttack_2HLong",                               // 双手长柄武器被击中
                            "BeAttack_2HXBow",                               // 弩枪默认武器被击中
                            "BeAttack_2HBow",                                // 弓默认被击中
                            "BeAttack_1HMage",                               // 单手魔杖被击中
                            "BeAttack_2HMage",                               // 双手法杖被击中
                            "KnockBack_loop",                                // 击飞空中循环
                            "KnockBack_Landing",                             // 击飞落地
                            "hitdown_start",                                 // 击倒落地(wow knockdown) 
                            "hitdown_loop",                                  // 击倒循环 
                            "hitdown_end",                                   // 击倒起身 
                            "Floating",                                      // 浮空
                            "KnockOutStart",                                 // 被击飞起始
                            "KnockOutLoop",                                  // 被击飞循环
                            "KnockOutEnd",                                   // 被击飞落地
                            "KnockDownStart",                                // 被击倒起始
                            "KnockDownLoop",                                 // 被击倒循环
                            "KnockDownEnd",                                  // 被击倒结束
                            "BeStun",                                        // 眩晕
                            "Parry",                                         // 闪避
                            "Parry_1H",                                      // 单手武器闪避
                            "Parry_2H",                                      // 双手武器闪避
                            "Parry_2HLong",                                  // 双手长柄武器闪避
                            "Parry_2HXBow",                                  // 双持弩枪战斗闪避
                            "Parry_2HBow",                                   // 双手弓战斗闪避
                            "Parry_1HMage",                                  // 单手魔杖战斗闪避
                            "Parry_2HMage",                                  // 双手法杖战斗闪避
                            "Riding",                                        // 骑马
                            "Skill001",                                      // 技能1
                            "Skill002",                                      // 技能2
                            "Skill003",                                      // 技能3
                            "Skill004",                                      // 技能4
                            "Skill005",                                      // 技能5
                            "Skill006",                                      // 技能6
                            "Skill007",                                      // 技能7
                            "Skill008",                                      // 技能8
                            "Skill009",                                      // 技能9
                            "Skill010",                                      // 技能10
                            "Skill011",                                      // 技能1
                            "Skill012",                                      // 技能2
                            "Skill013",                                      // 技能3
                            "Skill014",                                      // 技能4
                            "Skill015",                                      // 技能5
                            "Skill016",                                      // 技能6
                            "Skill017",                                      // 技能7
                            "Skill018",                                      // 技能8
                            "Skill019",                                      // 技能9
                            "Skill020",                                      // 技能10
                            "CraftStart",                                    // 锻造起始
                            "CraftLoop",                                     // 锻造循环
                            "CraftEnd",                                      // 锻造结束
                            "LootStart",                                     // 采集起始
                            "LootLoop",                                      // 采集循环
                            "LootEnd",                                       // 采集结束
                            "FishingStart",                                  // 钓鱼起始
                            "FishingLoop",                                   // 钓鱼循环
                            "FishingEnd",                                    // 钓鱼结束
                            "MiningStart",                                   // 采矿起始
                            "MiningLoop",                                    // 采矿循环
                            "MiningEnd",                                     // 采矿结束
                            "FellingStart",                                  // 伐木起始
                            "FellingLoop",                                   // 伐木循环
                            "FellingEnd",                                    // 伐木结束                                      
                            "BeforeDance",                                   // 跳舞起始
                            "Dance",                                         // 跳舞循环
                            "EmoteCheer",                                    //欢呼01
                            "EmoteLaugh",                                    //大笑
                            "EmoteNo",                                       //say NO
                            "EmotePoint",                                    //指前方
                            "EmoteSad_Cry",                                  //哭泣
                            "EmoteTalk01",                                   //谈话01
                            "EmoteTalkLong01",                               //谈话02
                            "EmoteTaunt",                                    //嘲讽
                            "EmoteUse",                                      //使用
                            "EmoteWave",                                     //招手
                            "EmoteYes",                                      //say YES
                            "KneelStart",                                    //下跪开始
                            "KneelLoop",                                     //下跪循环
                            "KneelEnd",                                      //下跪结束
                            "Emote_Dance",                                   //跳舞
                            "Emote_Sleep",                                   //睡觉
                            "Reborn",                                           //复活动作
                                            };
            #endregion 

            foreach(var i in actions)
            {
                CSUtility.Data.RoleTemplateManager.Instance.ActionNameList.Add(i);
            }
        }
    }
}
