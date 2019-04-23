using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    [RPC.RPCClassAttribute(typeof(GameRPC))]
    public class GameRPC : RPC.RPCObject
    {
        #region RPC必须的基础定义
        public static RPC.RPCClassInfo smRpcClassInfo = new RPC.RPCClassInfo();
        public virtual RPC.RPCClassInfo GetRPCClassInfo()
        {
            return smRpcClassInfo;
        }
        #endregion

        static GameRPC smInstance = new GameRPC();
        public static GameRPC Instance
        {
            get { return smInstance; }
        }

        [RPC.RPCIndexObjectAttribute(0, typeof(Guid), (int)RPC.RPCExecuteLimitLevel.All, false)]
        public Role.RoleActor this[UInt32 i]
        {
            get
            {
                return Stage.MainStage.Instance[i];
            }
        }

        public string AppStartParam = "";

        #region 网络连接处理

        public void StartNetWork()
        {
            var assembly = CSUtility.Program.GetAnalyseAssembly(CSUtility.Helper.enCSType.Client, CSUtility.Program.CurrentPlatform, "game");            
            RPC.RPCEntrance.BuildRPCMethordExecuter(assembly);
            CSUtility.Program.LogInfo("StartNetWork ok");
            RPC.RPCNetworkMgr.Instance.mRootObject = GameRPC.Instance;

            CCore.Engine.Instance.Client.RegSvrConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            CCore.Engine.Instance.Client.RegSvrConnect.NewConnect += this.OnRegServerConnected;

            CCore.Engine.Instance.Client.GateSvrConnect.ReceiveData += RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            CCore.Engine.Instance.Client.GateSvrConnect.NewConnect += this.OnGateServerConnected;
            CCore.Engine.Instance.Client.GateSvrConnect.CloseConnect += this.OnGateServerDisConnected;
        }

        public void StopNetWork()
        {
            CCore.Engine.Instance.Client.RegSvrConnect.ReceiveData -= RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            CCore.Engine.Instance.Client.RegSvrConnect.NewConnect -= this.OnRegServerConnected;
            CCore.Engine.Instance.Client.RegSvrConnect.Close();

            CCore.Engine.Instance.Client.GateSvrConnect.ReceiveData -= RPC.RPCNetworkMgr.Instance.ClientReceiveData;
            CCore.Engine.Instance.Client.GateSvrConnect.NewConnect -= this.OnGateServerConnected;
            CCore.Engine.Instance.Client.GateSvrConnect.Close();

            RPC.RPCNetworkMgr.Instance.mRootObject = null;
        }

        public float RecvBytePerSecond = 0;
        public float SendBytePerSecond = 0;
        int RecvSize = 0;
        int SendSize = 0;
        private long PrevNetworkCountTime = 0;
        public void TickNetWork()
        {
            CCore.Engine.Instance.Client.RegSvrConnect.Update();
            CCore.Engine.Instance.Client.GateSvrConnect.Update();

            RPC.RPCNetworkMgr.Instance.Tick(CCore.Engine.Instance.GetElapsedMillisecond());

            var nowTime = CCore.Engine.Instance.GetFrameMillisecond();
            if (nowTime - PrevNetworkCountTime > 5000)
            {
                var deltaRecv = RPC.RPCNetworkMgr.Instance.TotalReceiveSize - RecvSize;
                RecvSize = RPC.RPCNetworkMgr.Instance.TotalReceiveSize;

                RecvBytePerSecond = ((float)deltaRecv) / 5;

                var deltaSend = CCore.Engine.Instance.Client.GateSvrConnect.TotalSendSize - SendSize;
                SendSize = CCore.Engine.Instance.Client.GateSvrConnect.TotalSendSize;

                SendBytePerSecond = ((float)deltaSend) / 5;
                PrevNetworkCountTime = nowTime;
            }
        }

        string mUserName;
        string mPassword;
  //      string mThirdId;
  //      bool mUseThirdIdLogin = false;
        Guid mPlanesId = CSUtility.Support.IHelper.GuidTryParse("CA7D3D3D-C3C5-494C-BF61-0D20342E1527");
        public void Login(string usr, string psw)
        {
            CSUtility.Program.LogInfo("Login 0");
            var ips = System.Net.Dns.GetHostAddresses(AppConfig.Instance.HostIpAddress);
            string ip = ips[0].ToString();
            CCore.Engine.Instance.Client.GateSvrConnect.Close();
            CSUtility.Program.LogInfo("Login 1");
            CCore.Engine.Instance.Client.RegSvrConnect.Connect(ip, AppConfig.Instance.HostPort);
          //  mUseThirdIdLogin = false;
            mUserName = usr;
            mPassword = psw;
            CSUtility.Program.LogInfo("Login ok");
        }
        #endregion

        void OnRegServerConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
            {
                //提示用户网络有问题
                UISystem.VMessageBox.Instance.Show(Game.Instance.RootUIMsg.Root, "错误信息", "未能连接到服务器", UISystem.VMessageBoxType.Ok);
                UISystem.VMessageBox.Instance.OnButtonClicked = (buttonType) =>
                {
                    switch (buttonType)
                    {
                        case UISystem.VMessageBox.eMessageBoxResult.OK:
                            {

                            }
                            break;

                        case UISystem.VMessageBox.eMessageBoxResult.Cancel:
                            {

                            }
                            break;
                    }
                };
                return;
            }
            RPC.PackageWriter pkg = new RPC.PackageWriter();
            Server.H_RPCRoot.smInstance.HGet_RegServer(pkg).GetLowGateServer(pkg);
            pkg.WaitDoCommandWithTimeOut(10.0F, CCore.Engine.Instance.Client.RegSvrConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace()).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
            {
                if (bTimeOut)
                {
                    UISystem.VMessageBox.Instance.Show(Game.Instance.RootUIMsg.Root, "错误信息", "网络响应超时", UISystem.VMessageBoxType.Ok);
                    return;
                }
                RPC.DataReader dr;
                _io.Read(out dr);
                string Ip;
                UInt16 port;
                dr.Read(out Ip);
                dr.Read(out port);

                var ips = System.Net.Dns.GetHostAddresses(Ip);
                Ip = ips[0].ToString();
                CCore.Engine.Instance.Client.RegSvrConnect.Close();
                CCore.Engine.Instance.Client.GateSvrConnect.Connect(Ip, port);
             
            };
        }

        void OnGateServerConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (nLength == 0)
            {
                //提示用户网络有问题
                UISystem.VMessageBox.Instance.Show(Game.Instance.RootUIMsg.Root, "错误信息", "未能连接到Gate服务器", UISystem.VMessageBoxType.Ok);
                return;
            }

            Game.Instance.NetworkIsDisconnected = false;

            if (true)
            {
                Stage.MainStage.mOffLineLogin = false;
                //SureTryLogin();
//                 RPC.PackageWriter pkg = new RPC.PackageWriter();
//                 Server.H_RPCRoot.smInstance.HGet_GateServer(pkg).TestRPCVersion(pkg, RPCClientVersion.GetManager().RPCHashCode, RPCServerVersion.GetManager().RPCHashCode);
//                 pkg.WaitDoCommandWithTimeOut(10, CCore.Engine.Instance.Client.GateSvrConnect, RPC.CommandTargetType.DefaultType, new System.Diagnostics.StackTrace()).OnFarCallFinished = delegate (RPC.PackageProxy _io, bool bTimeOut)
//                 {
//                     if (bTimeOut)
//                     {
//                         UISystem.VMessageBox.Instance.Show(Game.Instance.RootUIMsg.Root, "错误信息", "尝试登陆网络响应超时", UISystem.VMessageBoxType.Ok);
//                         return;
//                     }
//                     sbyte successed = -1;
//                     _io.Read(out successed);
//                     string warningMsg = "";
//                     switch (successed)
//                     {
//                         case -1:
//                             warningMsg = "客户端网络处理版本不一致，某些行为有可能不正常，点击确定继续进游戏";
//                             break;
//                         case -2:
//                             warningMsg = "服务器网络处理版本不一致，某些行为有可能不正常，点击确定继续进游戏";
//                             break;
//                         case 1:
//                             SureTryLogin();
//                             return;
//                     }
// 
//                     UISystem.VMessageBox.Instance.Show(Game.Instance.RootUIMsg.Root, "警告", warningMsg, UISystem.VMessageBoxType.OkCancel);
//                     UISystem.VMessageBox.Instance.OnButtonClicked = (buttonType) =>
//                     {
//                         if (buttonType == UISystem.VMessageBox.eMessageBoxResult.OK)
//                         {
//                             SureTryLogin();
//                         }
//                         else
//                         {
//                             pClient.Close();
//                         }
//                     };
//                 };
            }

        }

        void OnGateServerDisConnected(CSUtility.Net.TcpClient pClient, byte[] pData, int nLength)
        {
            if (Game.Instance.CurrentStage == null)
                return;
            UISystem.VMessageBox.Instance.Show(Game.Instance.RootUIMsg.Root,
                                "错误信息", System.String.Format("与服务器网络连接断开"),
                                UISystem.VMessageBoxType.Ok);

            UISystem.VMessageBox.Instance.OnButtonClicked = (buttonType) =>
            {
                switch (buttonType)
                {
                    case UISystem.VMessageBox.eMessageBoxResult.OK:
                        {
//                             if (string.IsNullOrEmpty(GameRPC.Instance.AppStartParam))
//                             {
//                                 var stage = CCore.Support.ReflectionManager.Instance.GetClassObject<Login.LoginStage>();
//                                 Game.Instance.NetworkIsDisconnected = true;
//                             }
//                             else
//                             {
//                                 var proc = new System.Diagnostics.Process();
//                                 proc.StartInfo.FileName = "Launcher.exe";
//                                 //proc.StartInfo.Arguments = param;
//                                 proc.Start();
// 
// //                                 if (MainForm != null)
// //                                     MainForm.Close();
//                             }
                        }
                        break;
                }
            };
        }

        private void SureTryLogin()
        {
            var uiForm = CCore.Support.ReflectionManager.Instance.GetUIForm("Login") as UISystem.WinForm;
            if (uiForm != null)
            {
                uiForm.Parent = null;
            }
            uiForm = CCore.Support.ReflectionManager.Instance.GetUIForm("RoleSelect") as UISystem.WinForm;
            if (uiForm != null)
            {
                uiForm.Parent = Game.Instance.RootUIMsg.Root;
            }
        }


        #region 客户端给服务器用的RPC

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_EnterMap(UInt32 singleId, Guid mapSourceId, SlimDX.Vector3 pos, RPC.DataReader sceneParameter)
        {//主角进入地图
            Stage.MainStage.Instance.RPC_ChiefPlayerEnterMap(singleId, mapSourceId, pos, sceneParameter);
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_RoleLeave(UInt32 singleId)
        {//有角色离开地图
            Role.RoleActor role = Role.RoleManager.Instance.FindRoleActor(singleId);
            if (role == null)
                return;
            //             if (role.RoleData.RoleType ==FrameSet.Role.EClientRoleType.Summon)
            //             {
            //                 role.mImmediate2Death = true;
            //             }
            role.DoLeaveMap();
        }

        [RPC.RPCMethodAttribute((int)RPC.RPCExecuteLimitLevel.Player, false)]
        public void RPC_CreateSummon(RPC.DataReader parameter)
        {
            UInt16 count;
            parameter.Read(out count);
            for (int i = 0; i < count; i++)
            {
                uint singleId;
                parameter.Read(out singleId);
                GameData.Role.SummonData smdata = new GameData.Role.SummonData();
                parameter.Read(smdata, true);
                uint ownerId;
                parameter.Read(out ownerId);
                SlimDX.Vector3 ownerPos;
                parameter.Read(out ownerPos);
                string curState;
                parameter.Read(out curState);
                var summon = Role.RoleManager.Instance.FindRoleActor(singleId);
                if (summon == null)
                {
                    //smdata.Position = new SlimDX.Vector3(24.50577F,0.05F,23.9985F);
                    summon = Role.RoleActor.CreateSummon(singleId, smdata);
                    if (summon == null)
                        return;
                    summon.OwnerRole = Role.RoleManager.Instance.FindRoleActor(ownerId);
                    if (ownerPos != null && summon.OwnerRole != null && summon.OwnerRole.RoleData.RoleType == Role.EClientRoleType.Monster)//这里只做npc的玩家有可能放技能的时候改变位置
                    {
                        //                         var posY =  summon.OwnerRole.GetAltitude(ownerPos.X, ownerPos.Z);
                        //                         ownerPos.Y = posY;
                        //                         summon.OwnerRole.Placement.SetLocation(ref ownerPos);
                    }
                }

                var state = summon.AIStates.GetState(curState);
                if (state == null)
                {
                    Log.FileLog.WriteLine("RPC_CreateSummon {0} 状态 {1} 找不到", summon.RoleTemplate.Id, curState);
                    return;
                }
                var param = state.Parameter;
                parameter.Read(param, true);


                var walkParam = param as CSUtility.AISystem.States.IWalkParameter;
                if (walkParam != null)
                {
                    //walkParam.TargetPosition = new SlimDX.Vector3(24.53133F, 0.05F, 38.99848F);
                    //Log.FileLog.WriteLine(string.Format("From {0} To {1} Speed {2}", smdata.Position, walkParam.TargetPosition, walkParam.MoveSpeed));
                }
                summon.CurrentState.ToState(curState, param);
            }
        }
        #endregion
    }
}
