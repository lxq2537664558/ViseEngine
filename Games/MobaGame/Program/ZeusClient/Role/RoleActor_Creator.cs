using GameData.Role;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Role
{
    public partial class RoleActor
    {
        public static RoleActor CreateChiefPlayer(UInt32 singleId,PlayerData pd)
        {
            RoleActor chiefRole = new RoleActor();
            chiefRole.InitRoleActor(pd);
            var chiefRoleInit = new CCore.World.ActorInit();
            chiefRoleInit.GameType = (UInt16)CSUtility.Component.EActorGameType.Player;
            chiefRoleInit.SceneFlag = CSUtility.Component.enActorSceneFlag.Dynamic_Loc;
            chiefRoleInit.CastShadow = chiefRole.RoleData.RoleTemplate.CastShadow;
            chiefRoleInit.EnableHitProxyInGame = true;
            chiefRole.Initialize(chiefRoleInit);

            chiefRole.StateNotify2Remote = true;
            chiefRole.mIsLeaveMapByDistance = false;

            // 先设置模型再设置AI，以便AI默认状态修改动作
            // Create Mesh
            var roleVisual = new CCore.Component.RoleActorVisual();
            var roleVisualInit = new CCore.Component.RoleActorVisualInit();

            roleVisualInit.MeshTemplateIds = chiefRole.RoleTemplate.DefaultMeshs;
            roleVisualInit.CanHitProxy = false; // 默认主角不应该提交HitProxy，在编辑模式下主角提交HitProxy
            roleVisualInit.RimStart = 0.5f;
            roleVisualInit.RimEnd = 1.0f;
            roleVisualInit.RimMultiply = 1.0f;
            roleVisualInit.IsRimBloom = 0;
            
            // 更新蒙皮装备
            roleVisual.Initialize(roleVisualInit, chiefRole);
            //roleVisual.SetHitProxyAll(MidLayer.IHitProxyMap.Instance.GenHitProxy(chiefRole.Id));
            chiefRole.Visual = roleVisual;

            chiefRole.RoleData.AIGuid = pd.RoleTemplate.AIGuid;

            chiefRole.mPlacement = new ChiefPlayerPlacement(chiefRole);
            //chiefRole.mPlacement.OnLocationChanged += chiefRole.OnChiefRoleLocationChanged_TriggerProcess;
            chiefRole.mGravity = new CSUtility.Component.IGravityComp(chiefRole.RoleTemplate.HalfHeight * 3);
            //chiefRole.Gravity.Gravity = -0.1F;

            chiefRole.Placement.SetLocation(pd.Position);
            float angle = (float)System.Math.PI * 90 / 180;
            SlimDX.Quaternion quat = SlimDX.Quaternion.RotationAxis(SlimDX.Vector3.UnitY, angle);
            /*chiefRole.mPlacement.SetRotation(ref quat);*/
          //  chiefRole.mPlacement.SetRotationY(3.14f,chiefRole.RoleTemplate.MeshFixAngle);

            var cylinder = new CSUtility.Component.IShapeCylinder();

            //             cylinder.CamHeight = pd.PlayerDetail.Template.CameraPointHeight;
            cylinder.HalfHeight = pd.RoleTemplate.HalfHeight;
            cylinder.Radius = pd.RoleTemplate.Radius;
            chiefRole.mShape = cylinder;

            chiefRole.mActorController = ChiefRoleActorController.Instance;
            ChiefRoleActorController.Instance.Host = chiefRole;
            chiefRole.mActorController.CreateCameraController(Game.Instance.REnviroment.Camera);

            CCore.Client.MainWorldInstance.AddActor(chiefRole);
            CCore.Engine.Instance.Client.SetChiefRole(chiefRole);

            RoleManager.Instance.MapRoleId(chiefRole, singleId);

            chiefRole.RoleData.LiveTime = float.MaxValue;

            chiefRole.InitRelationship();

            return chiefRole;
        }

        public static RoleActor CreateOtherPlayer(UInt32 singleId, PlayerData pd,SlimDX.Vector3 loc, SlimDX.Quaternion rot)
        {
            RoleActor chiefRole = new RoleActor();
            chiefRole.InitRoleActor(pd, true);
            var chiefRoleInit = new CCore.World.ActorInit();
            chiefRoleInit.GameType = (UInt16)CSUtility.Component.EActorGameType.Player;
            chiefRoleInit.SceneFlag = CSUtility.Component.enActorSceneFlag.Dynamic_Loc;
            chiefRoleInit.CastShadow = chiefRole.RoleData.RoleTemplate.CastShadow;
            chiefRoleInit.EnableHitProxyInGame = true;
            chiefRole.Initialize(chiefRoleInit);

            chiefRole.StateNotify2Remote = true;
            chiefRole.mIsLeaveMapByDistance = false;

            // 先设置模型再设置AI，以便AI默认状态修改动作
            // Create Mesh
            var roleVisual = new CCore.Component.RoleActorVisual();
            var roleVisualInit = new CCore.Component.RoleActorVisualInit();

            roleVisualInit.MeshTemplateIds = chiefRole.RoleTemplate.DefaultMeshs;
            roleVisualInit.CanHitProxy = false; // 默认主角不应该提交HitProxy，在编辑模式下主角提交HitProxy
            roleVisualInit.RimStart = 0.5f;
            roleVisualInit.RimEnd = 1.0f;
            roleVisualInit.RimMultiply = 1.0f;
            roleVisualInit.IsRimBloom = 0;

            // 更新蒙皮装备
            roleVisual.Initialize(roleVisualInit, chiefRole);            
            chiefRole.Visual = roleVisual;

            chiefRole.RoleData.AIGuid = pd.RoleTemplate.AIGuid;

            chiefRole.mPlacement = new RolePlacement(chiefRole);            
            chiefRole.mGravity = new CSUtility.Component.IGravityComp(chiefRole.RoleTemplate.HalfHeight * 3);
            //chiefRole.Gravity.Gravity = -0.1F;

            chiefRole.Placement.SetLocation(loc);
            chiefRole.mPlacement.SetRotation(ref rot);
            //  chiefRole.mPlacement.SetRotationY(3.14f,chiefRole.RoleTemplate.MeshFixAngle);

            var cylinder = new CSUtility.Component.IShapeCylinder();

            //             cylinder.CamHeight = pd.PlayerDetail.Template.CameraPointHeight;
            cylinder.HalfHeight = pd.RoleTemplate.HalfHeight;
            cylinder.Radius = pd.RoleTemplate.Radius;
            chiefRole.mShape = cylinder;
      
            CCore.Client.MainWorldInstance.AddActor(chiefRole);

            RoleManager.Instance.MapRoleId(chiefRole, singleId);

            chiefRole.RoleData.LiveTime = float.MaxValue;

            chiefRole.InitRelationship();

            return chiefRole;
        }

        public static RoleActor CreateMonster(UInt32 singleId,MonsterData monsterData)
        {            
            if (singleId == 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("出错了，创建了不该创建的npc{0}", monsterData.Template.NickName));
                return null;
            }

            if (monsterData.Template == null)
                return null;

            var role = new RoleActor();
            role.InitRoleActor(monsterData);
            var roleInit = new CCore.World.ActorInit();
            roleInit.GameType = (UInt16)CSUtility.Component.EActorGameType.Npc;
            roleInit.SceneFlag = CSUtility.Component.enActorSceneFlag.Dynamic_Loc;
            roleInit.CastShadow = monsterData.RoleTemplate.CastShadow;
            roleInit.EnableHitProxyInGame = true;
            role.Initialize(roleInit);

            var roleVisual = new CCore.Component.RoleActorVisual();
            var roleVisualInit = new CCore.Component.RoleActorVisualInit();

            roleVisualInit.MeshTemplateIds = role.RoleTemplate.DefaultMeshs;
            roleVisualInit.CanHitProxy = true;

            roleVisual.Initialize(roleVisualInit,role);
            roleVisual.StartFadeIn(CCore.Mesh.MeshFadeType.FadeIn);
            role.Visual = roleVisual;

            role.RoleData.AIGuid = monsterData.AIGuid;

            role.mPlacement = new RolePlacement(role);
            role.MoveSpeed = monsterData.RoleMoveSpeed;
            role.mGravity = new CSUtility.Component.IGravityComp(role.RoleTemplate.HalfHeight * 3);

            var cylinder = new CSUtility.Component.IShapeCylinder();

            //             cylinder.CamHeight = pd.PlayerDetail.Template.CameraPointHeight;
            cylinder.HalfHeight = monsterData.RoleTemplate.HalfHeight;
            cylinder.Radius = monsterData.RoleTemplate.Radius;
            role.mShape = cylinder;

            var placement = role.Placement as RolePlacement;
        //    SlimDX.Vector3 scale = new SlimDX.Vector3(role.RoleData.Scale);
          //  role.Placement.SetScale(ref scale);
            CCore.Client.MainWorldInstance.AddActor(role);
            RoleManager.Instance.MapRoleId(role, singleId);
            var nowTime = CSUtility.Helper.LogicTimer.GetTickCount();
            var postion = monsterData.Position;
            postion.Y = role.GetAltitude(postion.X, postion.Z);
            placement.SetLocation(ref postion);
            role.Placement.SetRotationY(monsterData.Direction, monsterData.RoleTemplate.MeshFixAngle);
            var scale = new SlimDX.Vector3(monsterData.RoleTemplate.Scale);
            role.Placement.SetScale(ref scale);
            placement.mIsUseBornRotate = !role.RoleTemplate.IsRotate;
            //if (role !=null)
            //    System.Diagnostics.Debug.WriteLine(string.Format("CreateMonster，singleid:{0},time{1}", role.SingleId, nowTime/1000));
            //else
            //    System.Diagnostics.Debug.WriteLine(string.Format("出错了CreateMonster，singleid:{0},time:{1}", role.SingleId, nowTime/1000));
            role.RoleData.LiveTime = float.MaxValue;

            role.InitRelationship();

            return role;
        }

        public static RoleActor CreateSummon(UInt32 singleId, SummonData summonData)
        {
            var role = new RoleActor();
            role.InitRoleActor(summonData);
            var roleInit = new CCore.World.ActorInit();
            roleInit.GameType = (UInt16)summonData.SkillData.Template.GameType;
            if (summonData.Template == null)
            {
                var id = summonData.TemplateId;
                summonData.TemplateId = id;
                if (summonData.Template == null)
                    return null;
            }
            roleInit.CastShadow = (summonData.Template as RoleTemplate).CastShadow;
            roleInit.SceneFlag = CSUtility.Component.enActorSceneFlag.Dynamic_Loc;
            roleInit.VisibleCheck = false; // summon暂时不做视裁剪， 如果裁剪会遇到特效突然消失的情况
            role.Initialize(roleInit);

            var roleVisual = new CCore.Component.RoleActorVisual();
            var roleVisualInit = new CCore.Component.RoleActorVisualInit();

            roleVisualInit.MeshTemplateIds.AddRange(summonData.Template.DefaultMeshs);
            roleVisualInit.CanHitProxy = true;

            roleVisual.Initialize(roleVisualInit, role);
            roleVisual.SetHitProxyAll(CCore.Graphics.HitProxyMap.Instance.GenHitProxy(role.Id));
            // 
            // 召唤物的刀光默认打开
            //roleVisual.EnableTrail = true;
            role.Visual = roleVisual;
            if(summonData.SkillData.Template.Gravity)
            {
                role.mGravity =new CSUtility.Component.IGravityComp(role.RoleTemplate.HalfHeight * 3);
                //role.mGravity.Gravity = 0;
            }
            else
            {
                role.mGravity = null;
            }
            //             else
            //             {
            //                 role.mGravity.Gravity = 0;
            //             }
            //role.mActorController = new FrameSet.Role.ChiefRoleActorController(role);
            //role.AddFlag(CSCommon.Component.IActorInitBase.EActorFlag.NotSave);

            role.mForceLeaveMapTime = (long)(summonData.SkillData.Template.DeathTime * 1000);

            role.RoleData.AIGuid = summonData.AIGuid;
            role.RoleData.BornTime = CCore.Engine.Instance.GetFrameMillisecond();
            
            CCore.Client.MainWorldInstance.AddActor(role);
            RoleManager.Instance.MapRoleId(role, singleId);

            role.RoleData.LiveTime = summonData.LiveTime * 1.5f;
            role.InitRelationship();

            role.mPlacement = new RolePlacement(role);// new CSUtility.Component.StandardPlacement(role);
            var postion = summonData.Position;
            postion.Y = role.GetAltitude(postion.X, postion.Z);
            float offHeight = 0.0f;
            offHeight = summonData.SkillData.Template.HalfHeight;
            if (offHeight != 0.0f)
            {
                postion.Y += offHeight;
                var cylinder = new CSUtility.Component.IShapeCylinder();
                //cylinder.CamHeight = summonData.RoleTemplate.CameraPointHeight;
                cylinder.Radius = summonData.SkillData.Template.Radius;
                cylinder.HalfHeight = offHeight;
                role.mShape = cylinder;
            }
            SlimDX.Vector3 scale = new SlimDX.Vector3(role.RoleData.RoleTemplate.Scale);
            role.Placement.SetScale(ref scale);
            role.Placement.SetLocation(ref postion);

            role.RoleData.LiveTime = role.RoleData.SummonData.SkillData.Template.LiveTime * 1.2f;

            if (role.RoleData.SummonData.OiDirection != null)
            {
                //Log.FileLog.WriteLine("SummonRotation" + role.RoleData.SummonData.OiDirection.ToString());
                var placement = role.Placement as RolePlacement;
                if (placement != null)
                {
                    placement.mFixAngle = role.RoleTemplate.MeshFixAngle;
                    placement.SetRotationYbImm(role.RoleData.SummonData.OiDirection.Z, role.RoleData.SummonData.OiDirection.X, role.RoleTemplate.MeshFixAngle);
                }
                else
                {
                    role.Placement.SetRotationY(role.RoleData.SummonData.OiDirection.Z, role.RoleData.SummonData.OiDirection.X, role.RoleTemplate.MeshFixAngle, true);
                }
            }
            return role;
        }
    }
}
