using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCore.MsgProc;
using System.Reflection;

namespace MainEditor
{
    public partial class WorldEditorOperation : CCore.MsgProc.MsgReceiver
    {
        private CCore.MsgProc.FBehaviorProcess mKeyDownBehavior;
        private CCore.MsgProc.FBehaviorProcess mKeyUpBehavior;
        private CCore.MsgProc.FBehaviorProcess mLBDownBehavior;
        private CCore.MsgProc.FBehaviorProcess mLBUpBehavior;
        private CCore.MsgProc.FBehaviorProcess mRBDownBehavior;
        private CCore.MsgProc.FBehaviorProcess mRBUpBehavior;
        private CCore.MsgProc.FBehaviorProcess mMBDownBehavior;
        private CCore.MsgProc.FBehaviorProcess mMBUpBehavior;
        private CCore.MsgProc.FBehaviorProcess mMouseMoveBehavior;
        private CCore.MsgProc.FBehaviorProcess mMouseWheelBehavior;

        void InitializeMsg()
        {
            mKeyDownBehavior = OnKeyBoardDown;
            mKeyUpBehavior = OnKeyBoardUp;
            mLBDownBehavior = OnLBDown;
            mLBUpBehavior = OnLBUp;
            mRBDownBehavior = OnRBDown;
            mRBUpBehavior = OnRBUp;
            mMBDownBehavior = OnMBDown;
            mMBUpBehavior = OnMBUp;
            mMouseMoveBehavior = OnMouseMove;
            mMouseWheelBehavior = OnMouseWheel;

            CCore.Engine.Instance.OnMainFormActivated -= _OnMainFormActivated;
            CCore.Engine.Instance.OnMainFormActivated += _OnMainFormActivated;
           

            EditorCommon.Hotkey.HotkeyManager.Instance.LoadHotkeyConfig();

            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "打开编辑器", "打开编辑器", BehaviorParameter.enKeys.F12, false, false, false, (CCore.MsgProc.BehaviorParameter param, object obj)=>
            {
                EditorLoading.Instance.DoInitializeEditorMainWindow();
            });
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "转动摄像机", "转动场景中的视口摄像机", BehaviorParameter.enKeys.LButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.RotateCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "按水平面移动摄像机", "按照水平面移动场景中的视口摄像机", BehaviorParameter.enKeys.MButton, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.HorizontalMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "按视平面移动摄像机", "按照视平面移动场景中的视口摄像机", BehaviorParameter.enKeys.MButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.ScreenMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "轴向移动摄像机", "沿摄像机方向移动场景中的视口摄像机", BehaviorParameter.enKeys.RButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.DirMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "轴向移动摄像机 ", "沿摄像机方向移动场景中的视口摄像机", BehaviorParameter.enKeys.None, false, false, false, EditorCommon.Hotkey.enMouseType.MouseWheel, null, EditorCommon.GameCameraOperation.DirMoveCamera2);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "选择对象", "游戏窗口中选择对象", BehaviorParameter.enKeys.LButton, false, false, false, EditorCommon.Hotkey.enMouseType.None, null, SelectActorBehavior);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "多选对象", "游戏窗口中多重选择对象", BehaviorParameter.enKeys.LButton, true, false, false, EditorCommon.Hotkey.enMouseType.None, null, MultiSelectActorBehavior);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "操作坐标轴", "游戏窗口中平移、旋转或缩放坐标轴", BehaviorParameter.enKeys.LButton, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, StartTranslateAxis, TranslateAxis, EndTranslateAxis);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "复制对象", "复制地图中选中的游戏对象", BehaviorParameter.enKeys.LButton, false, true, false, EditorCommon.Hotkey.enMouseType.MouseMove, GameActorOperation_OnStartCopyActors, TranslateAxis, EndTranslateAxis);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "选择模式", "游戏窗口中选择对象模式", BehaviorParameter.enKeys.Z, false, false, false, EditorCommon.Hotkey.enMouseType.None, null, new Action<BehaviorParameter, object>((param, obj)=>
            {
                EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_Camera);
            }));
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "移动模式", "游戏窗口中选择移动模式", BehaviorParameter.enKeys.X, false, false, false, EditorCommon.Hotkey.enMouseType.None, null, new Action<BehaviorParameter, object>((param, obj)=>
            {
                EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_AxisMove);
            }));
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "旋转模式", "游戏窗口中选择旋转模式", BehaviorParameter.enKeys.C, false, false, false, EditorCommon.Hotkey.enMouseType.None, null, new Action<BehaviorParameter, object>((param, obj) =>
            {
                EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_AxisRot);
            }));
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "缩放模式", "游戏窗口中选择缩放模式", BehaviorParameter.enKeys.V, false, false, false, EditorCommon.Hotkey.enMouseType.None, null, new Action<BehaviorParameter, object>((param, obj) =>
            {
                EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_AxisScale);
            }));
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "删除对象", "游戏窗口中删除选中的对象", BehaviorParameter.enKeys.Delete, false, false, false, EditorCommon.Hotkey.enMouseType.None, null, new Action<BehaviorParameter, object>((param, obj) =>
            {
                foreach(var actor in SelectedActors)
                {
                    CCore.Engine.Instance.Client.MainWorld.RemoveActor(actor);
                    CCore.Engine.Instance.Client.MainWorld.RemoveEditorActor(actor);
                    EditorCommon.GameActorOperation.RemoveActor(actor);
                }
                SelectedActors.Clear();
                UnSelectedAll();
            }));
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "取消选择对象", "游戏窗口中取消选择对象", BehaviorParameter.enKeys.Esc, false, false, false, EditorCommon.Hotkey.enMouseType.None, null, new Action<BehaviorParameter, object>((param, obj) =>
            {
                UnSelectedAll();
            }));
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("游戏窗口操作", "显示灯光辅助", "显示灯光的辅助对象", BehaviorParameter.enKeys.L, false, false, false, EditorCommon.Hotkey.enMouseType.None, null, new Action<BehaviorParameter, object>((param, obj) =>
            {
                CCore.Program.SetActorTypeShow(CCore.Engine.Instance.Client.MainWorld, CCore.Program.LightAssistTypeName, mShowLightAssist);
                mShowLightAssist = !mShowLightAssist;
            }));

            EditorCommon.Hotkey.HotkeyManager.Instance.SetDefaultPresetHotkeyGroup();
            if (EditorCommon.Hotkey.HotkeyManager.Instance.PresetCollection.Count == 0)
            {                
                EditorCommon.Hotkey.HotkeyManager.Instance.AddPresetHotkeyGroup("默认设置");
                EditorCommon.Hotkey.HotkeyManager.Instance.SetPresetHotkeyGroup("默认设置");
            }

            EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup("游戏窗口操作");
        }
        static bool mShowLightAssist = true;
        
        private void _OnMainFormActivated(bool active)
        {
//             if(active)
//                 EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup("游戏窗口操作");
        }

        public override FBehaviorProcess FindBehavior(BehaviorParameter bhInit)
        {
            if (!CCore.Engine.Instance.IsEditorMode)
                return null;
            switch(bhInit.GetBehaviorType())
            {
                case (int)BehaviorType.BHT_KB_Char_Down:
                    return mKeyDownBehavior;
                case (int)BehaviorType.BHT_KB_Char_Up:
                    return mKeyUpBehavior;
                case (int)BehaviorType.BHT_LB_Down:
                    return mLBDownBehavior;
                case (int)BehaviorType.BHT_LB_Up:
                    return mLBUpBehavior;
                case (int)BehaviorType.BHT_RB_Down:
                    return mRBDownBehavior;
                case (int)BehaviorType.BHT_RB_Up:
                    return mRBUpBehavior;
                case (int)BehaviorType.BHT_MB_Down:
                    return mMBDownBehavior;
                case (int)BehaviorType.BHT_MB_Up:
                    return mMBUpBehavior;
                case (int)BehaviorType.BHT_Mouse_Move:
                    return mMouseMoveBehavior;
                case (int)BehaviorType.BHT_Mouse_Wheel:
                    return mMouseWheelBehavior;
            }

            return base.FindBehavior(bhInit);
        }

        int OnKeyBoardDown(CCore.MsgProc.BehaviorParameter parameter)
        {
            EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(parameter, null);
            return 0;
        }
        int OnKeyBoardUp(CCore.MsgProc.BehaviorParameter parameter)
        {
            EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(parameter, null);
            return 0;
        }
        int OnLBDown(CCore.MsgProc.BehaviorParameter parameter)
        {
            EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(parameter, null);
            return 0;
        }
        int OnLBUp(CCore.MsgProc.BehaviorParameter parameter)
        {
            EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(parameter, null);
            return 0;
        }
        int OnRBDown(CCore.MsgProc.BehaviorParameter parameter)
        {
            EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(parameter, null);
            return 0;
        }
        int OnRBUp(CCore.MsgProc.BehaviorParameter parameter)
        {
            EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(parameter, null);
            return 0;
        }
        int OnMBDown(CCore.MsgProc.BehaviorParameter parameter)
        {
            EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(parameter, null);
            return 0;
        }
        int OnMBUp(CCore.MsgProc.BehaviorParameter parameter)
        {
            EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(parameter, null);
            return 0;
        }
        int OnMouseMove(CCore.MsgProc.BehaviorParameter parameter)
        {
            EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(parameter, null);
            PreChooseActor(parameter);
            return 0;
        }
        int OnMouseWheel(CCore.MsgProc.BehaviorParameter parameter)
        {
            EditorCommon.Hotkey.HotkeyManager.Instance.TryExecuteHotkey(parameter, null);
            return 0;
        }
    }
}
