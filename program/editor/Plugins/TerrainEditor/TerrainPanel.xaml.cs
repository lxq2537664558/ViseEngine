using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using CCore.MsgProc;

namespace TerrainEditor
{
    /// <summary>
    /// Interaction logic for TerrainPanel.xaml
    /// </summary>
    [EditorCommon.PluginAssist.EditorPlugin(PluginType = "TerrainEditor")]
    [EditorCommon.PluginAssist.PluginMenuItem("工具(_T)/地形编辑器")]
    [Guid("AD9378F6-CFF6-475B-ADB7-FCBC0C15223E")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class TerrainPanel : UserControl, INotifyPropertyChanged, EditorCommon.PluginAssist.IEditorPlugin
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        //         private FBehaviorProcess mKeyDownBehavior;
        //         private FBehaviorProcess mKeyUpBehavior;
        //         private FBehaviorProcess mLBDownBehavior;
        //         private FBehaviorProcess mLBUpBehavior;
        //         private FBehaviorProcess mRBDownBehavior;
        //         private FBehaviorProcess mMBDownBehavior;
        //         private FBehaviorProcess mMouseMoveBehavior;
        //         private FBehaviorProcess mMouseWheelBehavior;

        public string PluginName
        {
            get { return "地形编辑器"; }
        }
        public string Version
        {
            get { return "1.0.0"; }
        }

        System.Windows.UIElement mInstructionControl = new System.Windows.Controls.TextBlock()
        {
            Text = "地形编辑器",
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        public System.Windows.UIElement InstructionControl
        {
            get { return mInstructionControl; }
        }

        public bool OnActive()
        {
            return true;
        }
        public bool OnDeactive()
        {
            return true;
        }

        public void Tick()
        {
            //             if (HostTerrain != CCore.Client.MainWorldInstance.Terrain)
            //                 HostTerrain = CCore.Client.MainWorldInstance.Terrain;
        }

        public void SetObjectToEdit(object[] obj)
        {

        }

        public object[] GetObjects(object[] param)
        {
            return null;
        }

        public bool RemoveObjects(object[] param)
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////

        //         public delegate void Delegate_OnBrushSizeValueChanged();
        //         public Delegate_OnBrushSizeValueChanged OnBrushSizeValueChanged;
        //         public delegate void Delegate_OnDeleteLayer(LayerItem item);
        //         public Delegate_OnDeleteLayer OnDeleteLayer;
        //         public delegate void Delegate_OnResetLayer(LayerItem item);
        //         public Delegate_OnResetLayer OnResetLayer;
        //         public delegate void Delegate_OnRefreshTerrainEffect(CCore.Material.Material mtl);
        //         public Delegate_OnRefreshTerrainEffect OnRefreshTerrainEffect;
        //         public delegate void Delegate_OnTerrainToolTypeChanged(enTerrainToolType toolType);
        //         public Delegate_OnTerrainToolTypeChanged OnTerrainToolTypeChanged;
        //         public delegate void Delegate_OnBrushChanged(string brushImageFile);
        //         public Delegate_OnBrushChanged OnBrushChanged;

        public enum enTerrainToolType
        {
            Generic = 0,    // 默认
            Flatten = 1,    // 削平
            Smooth = 2,    // 平滑
            Layer = 3,    // 层笔刷
            PaintBrush = 4,    // 绘制笔刷
            PickMaterial = 5,    // 选取地形材质
            AddPatch = 6,    // 添加地块
            DelPatch = 7,    // 删除地块
            Slope = 8,    // 斜坡
            ObliqueSlip = 9,    // 斜滑
        }
        enTerrainToolType mTerrainToolType = enTerrainToolType.Generic;
        public enTerrainToolType TerrainToolType
        {
            get { return mTerrainToolType; }
            set
            {
                mTerrainToolType = value;

                OnTerrainToolChanged(mTerrainToolType);
            }
        }

        public enum enTerrainBrushMode
        {
            Dots,
            FreeHand,
        }
        public enTerrainBrushMode TerrainBrushMode = enTerrainBrushMode.Dots;

        protected CCore.Support.ITerrainBrushActor mTerrainBrushActor;
        //static TerrainPanel smInstance = null;
        //public static TerrainPanel Instance
        //{
        //    get 
        //    {
        //        if (smInstance == null)
        //            smInstance = new TerrainPanel();
        //        return smInstance;
        //    }
        //}

        EditorCommon.Hotkey.HotkeyGroup mCurrentActiveGroup = null;

        bool mEnableEdit = false;
        public bool EnableEdit
        {
            get { return mEnableEdit; }
            set
            {
                if (mEnableEdit == value)
                    return;

                mEnableEdit = value;

                if (mEnableEdit)
                {
                    mCurrentActiveGroup = EditorCommon.Hotkey.HotkeyManager.Instance.CurrentActiveGroup;
                    EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup("地形编辑操作");
                    EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_Terrain);
                }
                else
                {
                    if (mCurrentActiveGroup != null)
                        EditorCommon.Hotkey.HotkeyManager.Instance.ActiveGroup(mCurrentActiveGroup.GroupName);
                    EditorCommon.WorldEditMode.Instance.RestoreEditMode();
                }

                OnPropertyChanged("EnableEdit");
            }
        }

        double mTerrainBrushRadius = 5;
        public double TerrainBrushRadius
        {
            get { return mTerrainBrushRadius; }
            set
            {
                if (value < 1)
                    return;
                mTerrainBrushRadius = value;

                OnTerrainBrushSizeValueChanged();

                if (mSelectedBrushData != null)
                    mSelectedBrushData.BrushRadius = mTerrainBrushRadius;

                OnPropertyChanged("TerrainBrushRadius");
            }
        }

        double mTerrainBrushFalloff = 0;
        public double TerrainBrushFalloff
        {
            get { return mTerrainBrushFalloff; }
            set
            {
                mTerrainBrushFalloff = value;

                OnTerrainBrushSizeValueChanged();

                if (mSelectedBrushData != null)
                    mSelectedBrushData.BrushFalloff = mTerrainBrushFalloff;

                OnPropertyChanged("TerrainBrushFalloff");
            }
        }

        double mTerrainBrushStrength = 1;
        public double TerrainBrushStrength
        {
            get { return mTerrainBrushStrength; }
            set
            {
                mTerrainBrushStrength = value;

                if (mSelectedBrushData != null)
                    mSelectedBrushData.BrushStrength = mTerrainBrushStrength;

                OnPropertyChanged("TerrainBrushStrength");
            }
        }

        double mTerrainBrushInterval = 1;
        public double TerrainBrushInterval
        {
            get { return mTerrainBrushInterval; }
            set
            {
                mTerrainBrushInterval = value;

                if (mSelectedBrushData != null)
                    mSelectedBrushData.BrushInterval = mTerrainBrushInterval;

                OnPropertyChanged("TerrainBrushInterval");
            }
        }

        private CCore.Terrain.Terrain mHostTerrain = null;
        public CCore.Terrain.Terrain HostTerrain
        {
            get { return mHostTerrain; }
            set
            {
                mHostTerrain = value;
                TerrainGridPanel.Instance.HostTerrain = mHostTerrain;

                UpdateTerrainMaterialLayers(mHostTerrain);
                InitTerrainBrushActor();
            }
        }

        bool bCanRotateBrush = true;
        public bool CanRotateBrush
        {
            get { return bCanRotateBrush; }
            set
            {
                bCanRotateBrush = value;

                if (mSelectedBrushData != null)
                    mSelectedBrushData.CanRotateBrush = bCanRotateBrush;

                OnPropertyChanged("CanRotateBrush");
            }
        }

        bool bCanSetRoleActorHeight = false;
        public bool CanSetRoleActorHeight
        {
            get { return bCanSetRoleActorHeight; }
            set
            {
                bCanSetRoleActorHeight = value;

                OnPropertyChanged("CanSetRoleActorHeight");
            }
        }

        TerrainEditorManager mTerrainEditorManager = null;

        public TerrainPanel()
        {
            InitializeComponent();

            for (int i = 0; i < Enum.GetNames(typeof(enTerrainToolType)).Length; i++)
            {
                BrushDatas.Add(new CBrushData());
            }
            SelectedBrushData = BrushDatas[0];

            mTerrainEditorManager = new TerrainEditorManager(this);

            InitializeLayerComboBox();

            CCore.Program.OnWorldLoaded += Program_OnWorldLoaded;

            EditorCommon.WorldEditMode.Instance.OnEditModeChangeTo += WorldEditMode_OnEditModeChangeTo;
            EditorCommon.WorldEditMode.Instance.OnEditModeChangeFrom += WorldEditMode_OnEditModeChangeFrom;

            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_转动摄像机", "转动场景中的视口摄像机", BehaviorParameter.enKeys.LButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.RotateCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_按视平面移动摄像机", "按照视平面移动场景中的视口摄像机", BehaviorParameter.enKeys.MButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.ScreenMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_按水平面移动摄像机", "按照水平面移动场景中的视口摄像机", BehaviorParameter.enKeys.MButton, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.HorizontalMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_轴向移动摄像机1", "沿摄像机方向移动场景中的视口摄像机", BehaviorParameter.enKeys.RButton, false, false, true, EditorCommon.Hotkey.enMouseType.MouseMove, EditorCommon.GameCameraOperation.StartTransCamera, EditorCommon.GameCameraOperation.DirMoveCamera);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_轴向移动摄像机2", "沿摄像机方向移动场景中的视口摄像机", BehaviorParameter.enKeys.None, false, false, false, EditorCommon.Hotkey.enMouseType.MouseWheel, null, EditorCommon.GameCameraOperation.DirMoveCamera2);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_笔刷尺寸变小", "笔刷逐渐变小", BehaviorParameter.enKeys.Oem4, false, false, false, OnBrushRadiusValueChangedSmall);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_笔刷尺寸变大", "笔刷逐渐变大", BehaviorParameter.enKeys.Oem6, false, false, false, OnBrushRadiusValueChangedBig);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_更新地形刷", "刷新地形刷的坐标", BehaviorParameter.enKeys.None, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, null, OnMouseMove);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_刷地形1", "在地面上移动鼠标刷地形", BehaviorParameter.enKeys.LButton, false, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, null, OnLBMouseMove);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_刷地形2", "在地面上移动鼠标向下刷地形", BehaviorParameter.enKeys.LButton, true, false, false, EditorCommon.Hotkey.enMouseType.MouseMove, null, OnCtrlLBMouseMove);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_UnDo", "可以返回当前上一次操作的地形状态", BehaviorParameter.enKeys.Z, true, false, false, Undo);
            EditorCommon.Hotkey.HotkeyManager.Instance.RegisterDefaultHotkey("地形编辑操作", "地形编辑_Redo", "可以返回当前下一次操作的地形状态", BehaviorParameter.enKeys.Y, true, false, false, Redo);
        }

        private void Program_OnWorldLoaded(System.String strAbsFolder, string componentName, CCore.World.World world)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (componentName)
                {
                    case "场景":
                        OnWorldLoaded();
                        break;
                    case "地形":
                        HostTerrain = CCore.Client.MainWorldInstance.Terrain;
                        break;
                }
            });
        }

        public void OnWorldLoaded()
        {
            if (CCore.Client.MainWorldInstance.IsNullWorld)
                return;
            
            // 初始化完WORLD后刷新一下模式
            EditorCommon.WorldEditMode.Instance.SetEditMode(EditorCommon.WorldEditMode.enEditMode.Edit_Camera);
        }

        #region 消息响应

        //// 在开始旋转摄像机时设置初始值
        //public void StartTransCamera(CCore.MsgProc.BehaviorParameter param, object obj)
        //{
        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Move;
        //    mStartRotateCameraMousePos.X = mouseMoveParam.X;
        //    mStartRotateCameraMousePos.Y = mouseMoveParam.Y;
        //}

        //// 按视平面移动摄像机
        //public void ScreenMoveCamera(CCore.MsgProc.BehaviorParameter param, object obj)
        //{
        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Move;

        //    var deltaX = mouseMoveParam.X - mStartRotateCameraMousePos.X;
        //    var deltaY = mouseMoveParam.Y - mStartRotateCameraMousePos.Y;

        //    var vFDir = EditorCommon.Program.EditorCameraController.Camera.Direction;
        //    vFDir.Y = 0;
        //    vFDir.Normalize();
        //    vFDir *= deltaY * EditorCommon.Program.CameraMoveSpdRate;

        //    EditorCommon.Program.EditorCameraController.Move(CCore.CoordAxis.X, -deltaX * EditorCommon.Program.CameraMoveSpdRate);
        //    EditorCommon.Program.EditorCameraController.Move(CCore.CoordAxis.Y, deltaY * EditorCommon.Program.CameraMoveSpdRate);

        //    mStartRotateCameraMousePos.X = mouseMoveParam.X;
        //    mStartRotateCameraMousePos.Y = mouseMoveParam.Y;
        //}

        //public void OnMoveCameraWithMouse(BehaviorParameter param, object obj)
        //{
        //    if (EditorCommon.Program.EditorCameraController == null)
        //        return;

        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Move;

        //    var deltaX = mouseMoveParam.X - mStartRotateCameraMousePos.X;
        //    var deltaY = mouseMoveParam.Y - mStartRotateCameraMousePos.Y;

        //    var vFDir = EditorCommon.Program.EditorCameraController.Camera.Direction;
        //    vFDir.Y = 0;
        //    vFDir.Normalize();
        //    vFDir *= deltaY * EditorCommon.Program.CameraMoveSpdRate;

        //    EditorCommon.Program.EditorCameraController.Move(vFDir);
        //    EditorCommon.Program.EditorCameraController.Move(CCore.CoordAxis.X, -deltaX * EditorCommon.Program.CameraMoveSpdRate);

        //    mStartRotateCameraMousePos.X = mouseMoveParam.X;
        //    mStartRotateCameraMousePos.Y = mouseMoveParam.Y;

        //    TerrainGridPanel.Instance.UpdateCamera();
        //}

        //// 旋转摄像机
        //public void RotateCamera(CCore.MsgProc.BehaviorParameter param, object obj)
        //{
        //    if (m_CanDrawTerrain)
        //        m_CanDrawTerrain = false;

        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Move;

        //    var deltaX = mouseMoveParam.X - mStartRotateCameraMousePos.X;
        //    var deltaY = mouseMoveParam.Y - mStartRotateCameraMousePos.Y;

        //    EditorCommon.Program.EditorCameraController.Turn(CCore.CoordAxis.Y, deltaX * 0.01f);
        //    EditorCommon.Program.EditorCameraController.Turn(CCore.CoordAxis.X, deltaY * 0.01f);

        //    mStartRotateCameraMousePos.X = mouseMoveParam.X;
        //    mStartRotateCameraMousePos.Y = mouseMoveParam.Y;
        //}

        //// 按摄像机方向移动摄像机
        //public void OnDirMoveCamera(CCore.MsgProc.BehaviorParameter param, object obj)
        //{
        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Move;

        //    var deltaX = mouseMoveParam.X - mStartRotateCameraMousePos.X;
        //    var deltaY = mouseMoveParam.Y - mStartRotateCameraMousePos.Y;

        //    int delta = (deltaX + deltaY) / 2;
        //    EditorCommon.Program.EditorCameraController.Move(CCore.CoordAxis.Z, delta * 30 * EditorCommon.Program.CameraZoomSpdRate);

        //    mStartRotateCameraMousePos.X = mouseMoveParam.X;
        //    mStartRotateCameraMousePos.Y = mouseMoveParam.Y;
        //}

        //public void OnDirMoveCamera2(BehaviorParameter param, object obj)
        //{
        //    var mouseMoveParam = param as CCore.MsgProc.Behavior.Mouse_Wheel;

        //    EditorCommon.Program.EditorCameraController.Move(CCore.CoordAxis.Z, mouseMoveParam.delta * EditorCommon.Program.CameraZoomSpdRate);
        //}

        public void OnBrushRadiusValueChangedSmall(BehaviorParameter param, object obj)
        {
            TerrainBrushRadius--;
        }

        public void OnBrushRadiusValueChangedBig(BehaviorParameter param, object obj)
        {
            TerrainBrushRadius++;
        }

        public void OnLBDown(BehaviorParameter param)
        {
            if (!m_CanDrawTerrain || HostTerrain == null)
                return;

            CCore.MsgProc.Behavior.Mouse_Key parameter = param as CCore.MsgProc.Behavior.Mouse_Key;

            mPreMouseIntersectWithTerrainPoint = EditorCommon.Assist.Assist.IntersectWithTerrain(parameter.X, parameter.Y, true, EditorCommon.Program.GameREnviroment?.Camera);
            mTerrainRisedDataDic.Clear();
            mTerraindModifyDataDic.Clear();
            mOldTerrainModifyDataDic.Clear();

            switch (TerrainToolType)
            {
                case enTerrainToolType.Flatten:
                    {
                        var u = HostTerrain.GetUWithX(mPreMouseIntersectWithTerrainPoint.X);
                        var v = HostTerrain.GetVWithZ(mPreMouseIntersectWithTerrainPoint.Z);
                        HostTerrain.GetHeight(u, v, ref mMouseLBDownPtTerrainHeight, false);
                    }
                    break;

                case enTerrainToolType.PickMaterial:
                    {
                        var u = HostTerrain.GetUWithX(mPreMouseIntersectWithTerrainPoint.X);
                        var v = HostTerrain.GetVWithZ(mPreMouseIntersectWithTerrainPoint.Z);
                        var matId = HostTerrain.PickMaterial(u, v);
                        SelectedLayerWithId(matId);
                    }
                    break;

                case enTerrainToolType.AddPatch:
                    {
                        HostTerrain.AddPatchF(mPreMouseIntersectWithTerrainPoint.X, mPreMouseIntersectWithTerrainPoint.Z);

                        var u = HostTerrain.GetUWithX(mPreMouseIntersectWithTerrainPoint.X);
                        var v = HostTerrain.GetVWithZ(mPreMouseIntersectWithTerrainPoint.Z);
                        GetTerrainModifyDataDic(u, v, 0, 0);
                    }
                    break;

                case enTerrainToolType.DelPatch:
                    {
                        HostTerrain.DelPatchF(mPreMouseIntersectWithTerrainPoint.X, mPreMouseIntersectWithTerrainPoint.Z);

                        var u = HostTerrain.GetUWithX(mPreMouseIntersectWithTerrainPoint.X);
                        var v = HostTerrain.GetVWithZ(mPreMouseIntersectWithTerrainPoint.Z);
                        GetTerrainModifyDataDic(u, v, 0, 0);
                    }
                    break;
                case enTerrainToolType.ObliqueSlip:
                    {
                        GetObliqueSlipPlane();
                    }
                    break;
            }
            //Log.FileLog.WriteLine(string.Format("OnLBDown : mPreMouseIntersectWithTerrainPoint_{1}", mPreMouseIntersectWithTerrainPoint));
        }

        public void OnLBUp(BehaviorParameter param)
        {
            if (TerrainToolType == enTerrainToolType.Slope)
            {
                mTerrainBrushActor.BrushType = CCore.Support.ITerrainBrushVisual.enBrushType.Circle;
                DrawSlopeTerrainWithMouse();

                if (mTerrainBrushActor != null)
                    mTerrainBrushActor.BrushAngle = 0;
            }

            if (mOldTerrainModifyDataDic.Count > 0 && mTerraindModifyDataDic.Count > 0)
            {
                var oldTerrainModifyDataDic = NewTerrainModifyDataDic(mOldTerrainModifyDataDic);
                var terrainModifyDataDic = NewTerrainModifyDataDic(mTerraindModifyDataDic);
                mTerrainEditorManager.AddTerrainModifyData(oldTerrainModifyDataDic, terrainModifyDataDic, TerrainToolType, !m_bCtrlKeyIsDown, bCanSetRoleActorHeight);

                EditorCommon.UndoRedoManager.Instance.AddCommand(new Action(() =>
                {
                    if (HostTerrain == null)
                        return;

                    mTerrainEditorManager.RedoTerrainModify();

                    var data = mTerrainEditorManager.GetTerrainUndoRedoDataWithIndex();
                    if (data == null)
                        return;

                    foreach (var i in data.mModifyDataDic.Keys)
                    {
                        foreach (var j in data.mModifyDataDic[i].Keys)
                        {
                            if (data.mToolType == enTerrainToolType.PaintBrush)
                            { }
                            else if (data.mToolType == enTerrainToolType.AddPatch)
                            {
                                HostTerrain.AddPatch(i, j);
                            }
                            else if (data.mToolType == enTerrainToolType.DelPatch)
                            {
                                HostTerrain.DelPatch(i, j);
                            }
                            else
                            {
                                HostTerrain.SetHeight(i, j, (short)data.mModifyDataDic[i][j], true);
                            }
                        }
                    }
                    if (data.mToolType == enTerrainToolType.PaintBrush)
                    { }
                    else if (data.mToolType == enTerrainToolType.AddPatch || data.mToolType == enTerrainToolType.DelPatch)
                    { }
                    else
                    {
                        SetActorHeight(data.mModifyDataDic, data.mCanSetActorHeight);
                    }
                }), new Action(() =>
                {
                    if (HostTerrain == null)
                        return;

                    var data = mTerrainEditorManager.GetTerrainUndoRedoDataWithIndex();
                    if (data == null)
                        return;

                    foreach (var i in data.mOldModifyDataDic.Keys)
                    {
                        foreach (var j in data.mOldModifyDataDic[i].Keys)
                        {
                            if (data.mToolType == enTerrainToolType.PaintBrush)
                            { }
                            else if (data.mToolType == enTerrainToolType.AddPatch)
                            {
                                HostTerrain.DelPatch(i, j);
                            }
                            else if (data.mToolType == enTerrainToolType.DelPatch)
                            {
                                HostTerrain.AddPatch(i, j);
                            }
                            else
                            {
                                HostTerrain.SetHeight(i, j, (short)data.mOldModifyDataDic[i][j], true);
                            }
                        }
                    }
                    if (data.mToolType == enTerrainToolType.PaintBrush)
                    { }
                    else if (data.mToolType == enTerrainToolType.AddPatch || data.mToolType == enTerrainToolType.DelPatch)
                    { }
                    else
                    {
                        SetActorHeight(data.mOldModifyDataDic, data.mCanSetActorHeight);
                    }

                    mTerrainEditorManager.UndoTerrainModify();
                }), "地形编辑操作");
            }
            mTerraindModifyDataDic.Clear();
            mOldTerrainModifyDataDic.Clear();
            mTerrainRisedDataDic.Clear();
            m_CanDrawTerrain = true;
        }

        public void OnMouseMove(BehaviorParameter param, object obj)
        {
            CCore.MsgProc.Behavior.Mouse_Move parameter = param as CCore.MsgProc.Behavior.Mouse_Move;

            TerrainGridPanel.Instance.UpdateCamera();

            SlimDX.Vector3 end = EditorCommon.Assist.Assist.IntersectWithTerrain(parameter.X, parameter.Y, true, EditorCommon.Program.GameREnviroment?.Camera);
            //switch(mTerrainBrushActor)
            switch (mTerrainBrushActor.BrushType)
            {
                case CCore.Support.ITerrainBrushVisual.enBrushType.Circle:
                    {
                        mTerrainBrushActor.Placement.SetLocation(ref end);
                    }
                    break;

                case CCore.Support.ITerrainBrushVisual.enBrushType.Rect:
                    {
                        SlimDX.Vector3 loc = new SlimDX.Vector3();
                        if (HostTerrain != null)
                        {
                            HostTerrain.GetPatchLocation(end.X, end.Z, ref loc);
                            mTerrainBrushActor.Placement.SetLocation(ref loc);
                        }
                    }
                    break;
            }

            //UpdateTerrainBrush();
            if (HostTerrain != null)
                mTerrainBrushActor.UpdateBrush(HostTerrain);
        }

        public void OnLBMouseMove(BehaviorParameter param, object obj)
        {
            m_bCtrlKeyIsDown = false;
            DrawTerrainWithMouse(param);

            //继续执行Move函数
            OnMouseMove(param, obj);
        }

        public void OnCtrlLBMouseMove(BehaviorParameter param, object obj)
        {
            m_bCtrlKeyIsDown = true;
            DrawTerrainWithMouse(param);

            //继续执行Move函数
            OnMouseMove(param, obj);
        }

        public void Redo(BehaviorParameter param, object obj)
        {
            EditorCommon.UndoRedoManager.Instance.Redo();
        }

        public void Undo(BehaviorParameter param, object obj)
        {
            EditorCommon.UndoRedoManager.Instance.Undo();
            Log.FileLog.WriteLine("Undo");
        }

        Dictionary<uint, Dictionary<uint, int>> NewTerrainModifyDataDic(Dictionary<uint, Dictionary<uint, int>> terrainModifyData)
        {
            Dictionary<uint, Dictionary<uint, int>> modifyDataDics = new Dictionary<uint, Dictionary<uint, int>>();
            
            foreach (var i in terrainModifyData.Keys)
            {
                Dictionary<uint, int> risedDataDic = new Dictionary<uint, int>();
                modifyDataDics[i] = risedDataDic;
                foreach (var j in terrainModifyData[i].Keys)
                {
                    risedDataDic[j] = terrainModifyData[i][j];
                }
            }
            return modifyDataDics;
        }

        #endregion

        void WorldEditMode_OnEditModeChangeFrom(EditorCommon.WorldEditMode.enEditMode editMode)
        {
            if (mTerrainBrushActor == null)
                return;

            if (editMode == EditorCommon.WorldEditMode.enEditMode.Edit_Terrain)
                mTerrainBrushActor.Visible = false;
        }

        void WorldEditMode_OnEditModeChangeTo(EditorCommon.WorldEditMode.enEditMode editMode)
        {
            if (mTerrainBrushActor == null)
                return;

            if (editMode == EditorCommon.WorldEditMode.enEditMode.Edit_Terrain)
            {
                mTerrainBrushActor.Visible = true;
                OnTerrainBrushSizeValueChanged();
            }
        }

        void OnTerrainToolChanged(enTerrainToolType toolType)
        {
            if (mTerrainBrushActor == null)
                return;

            switch (toolType)
            {
                case enTerrainToolType.Generic:
                case enTerrainToolType.Flatten:
                case enTerrainToolType.Smooth:
                case enTerrainToolType.Layer:
                case enTerrainToolType.PaintBrush:
                case enTerrainToolType.PickMaterial:
                    {
                        //mTerrainBrushActor.SetRadius(MainEditor.Panel.TerrainPanel.Instance.)
                        OnTerrainBrushBrushChanged(mSelectedBrushImage);
                        mTerrainBrushActor.BrushType = CCore.Support.ITerrainBrushVisual.enBrushType.Circle;
                    }
                    break;

                case enTerrainToolType.AddPatch:
                case enTerrainToolType.DelPatch:
                    {
                        OnTerrainBrushBrushChanged(CSUtility.Support.IFileConfig.DefaultTileBrushTechnique);
                        mTerrainBrushActor.BrushType = CCore.Support.ITerrainBrushVisual.enBrushType.Rect;
                        mTerrainBrushActor.SetRectSize(HostTerrain.GetPatchWidth(), HostTerrain.GetPatchHeight());
                    }
                    break;
            }
        }

        void OnTerrainBrushSizeValueChanged()
        {
            if (mTerrainBrushActor == null)
                return;
            var REnviroment = EditorCommon.Program.GameREnviroment;
            if (REnviroment == null)
                return;

            if (!mTerrainBrushActor.Visible)
            {
                // 将TerrainBrush放到屏幕中间，方便查看笔刷大小
                SlimDX.Vector3 end = EditorCommon.Assist.Assist.IntersectWithTerrain(REnviroment.View.Width / 2, REnviroment.View.Height / 2, true, REnviroment.Camera);
                mTerrainBrushActor.Placement.SetLocation(ref end);
            }
            
            float radius = (float)TerrainBrushRadius;
            float innerRadius = (float)(radius * (1 - TerrainBrushFalloff));
            mTerrainBrushActor.SetRadius(innerRadius, radius);

            //UpdateTerrainBrush();
        }

        void OnTerrainBrushBrushChanged(string brushImageFile)
        {
            if (mTerrainBrushActor == null)
                return;

            var REnviroment = EditorCommon.Program.GameREnviroment;
            if (REnviroment == null)
                return;

            if (!mTerrainBrushActor.Visible)
            {
                // 将TerrainBrush放到屏幕中间，方便查看笔刷大小
                SlimDX.Vector3 end = EditorCommon.Assist.Assist.IntersectWithTerrain(REnviroment.View.Width / 2, REnviroment.View.Height / 2, true, REnviroment.Camera);
                mTerrainBrushActor.Placement.SetLocation(ref end);
            }

            var fileName = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(brushImageFile, CSUtility.Support.IFileManager.Instance.Root);
            mTerrainBrushActor.SetBrushImageFile(fileName);
        }

        void OnResetTerrainLayerGrass(CCore.Grass.GrassData grass, Guid matId)
        {
            if (matId != Guid.Empty)
            {
                HostTerrain.ResetLayerGrass(matId, grass);
            }
        }

        void OnRefreshTerrainEffect(CCore.Material.Material mtl)
        {
            HostTerrain.RefreshEffect(mtl);
        }

        void OnResetTerrainLayer(LayerItem layerItem)
        {
            switch (layerItem.LayerType)
            {
                case LayerItem.enLayerType.Material:
                    {
                        if (layerItem.MaterialId != Guid.Empty)
                        {
                            var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetMaterialDefaultTechnique(layerItem.MaterialId);
                            HostTerrain.ResetLayerMaterial(layerItem.OldMaterialId, mtl);
                        }
                    }
                    break;
            }
        }

        #region 配置存储读取

        string mConfigFileName = CSUtility.Support.IFileConfig.EditorSourceDirectory + "\\TerrainEditorConfig\\Config.xml";
        public void SaveConfig()
        {
            CSUtility.Support.IConfigurator.SaveProperty(this, this.GetType().ToString(), mConfigFileName);

            var fullName = CSUtility.Support.IFileManager.Instance.Root + mConfigFileName;
            EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
            {
                if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                {
                    EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"地形编辑器: 配置文件{fullName}通过版本控制上传失败!");
                }
                else
                {
                    EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                    {
                        if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"地形编辑器: 配置文件{fullName}通过版本控制上传失败!");
                        }
                    }, fullName, $"AutoCommit 修改地形编辑器配置");
                }
            }, fullName);
        }
        public void LoadConfig()
        {
            CSUtility.Support.IConfigurator.FillProperty(this, mConfigFileName);
        }

        #endregion


        #region 笔刷操作

        public class CBrushData
        {
            double mBrushRadius = 5;
            [CSUtility.Support.DataValueAttribute("BrushRadius")]
            public double BrushRadius
            {
                get { return mBrushRadius; }
                set { mBrushRadius = value; }
            }

            double mBrushFalloff = 0;
            [CSUtility.Support.DataValueAttribute("BrushFalloff")]
            public double BrushFalloff
            {
                get { return mBrushFalloff; }
                set { mBrushFalloff = value; }
            }

            double mBrushStrength = 1;
            [CSUtility.Support.DataValueAttribute("BrushStrength")]
            public double BrushStrength
            {
                get { return mBrushStrength; }
                set { mBrushStrength = value; }
            }

            double mBrushInterval = 1;
            [CSUtility.Support.DataValueAttribute("BrushInterval")]
            public double BrushInterval
            {
                get { return mBrushInterval; }
                set { mBrushInterval = value; }
            }

            bool mCanRotateBrush = true;
            [CSUtility.Support.DataValueAttribute("CanRotateBrush")]
            public bool CanRotateBrush
            {
                get { return mCanRotateBrush; }
                set { mCanRotateBrush = value; }
            }
        }

        CBrushData mSelectedBrushData = null;
        public CBrushData SelectedBrushData
        {
            get { return mSelectedBrushData; }
            set
            {
                mSelectedBrushData = value;
                if (mSelectedBrushData != null)
                {
                    TerrainBrushRadius = mSelectedBrushData.BrushRadius;
                    TerrainBrushFalloff = mSelectedBrushData.BrushFalloff;
                    TerrainBrushStrength = mSelectedBrushData.BrushStrength;
                    TerrainBrushInterval = mSelectedBrushData.BrushInterval;
                    CanRotateBrush = mSelectedBrushData.CanRotateBrush;
                }
            }
        }

        BitmapSource mSelectedImage = null;
        public BitmapSource SelectedImage
        {
            get { return mSelectedImage; }
        }

        string mSelectedBrushImage = string.Empty;

        public double BrushMaxStrength
        {
            get
            {
                return Slider_Strength.Maximum;
            }
        }
        public double BrushMinStrength
        {
            get
            {
                return Slider_Strength.Minimum;
            }
        }

        // 存储的文件相对于EditorSourceDir;
        List<string> mBrushFiles = new List<string>();
        [CSUtility.Support.DataValueAttribute("BrushFiles")]
        public List<string> BrushFiles
        {
            get { return mBrushFiles; }
            set
            {
                mBrushFiles = value;

                WrapPanel_Brush.Children.Clear();
                WrapPanel_Brush.Children.Add(Button_BrushAdd);

                foreach (var file in mBrushFiles)
                {
                    var fullFile = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.EditorSourceDirectory + "\\" + file;
                    AddBrush(fullFile, false);
                }

//                 // 默认选中第一个
//                 foreach (var item in WrapPanel_Brush.Children)
//                 {
//                     if (item is RadioButton)
//                     {
//                         ((RadioButton)item).IsChecked = true;
//                         break;
//                     }
//                 }
            }
        }

        List<CBrushData> mBrushDatas = new List<CBrushData>();
        //[CSUtility.Support.DataValueAttribute("BrushDatas")]
        public List<CBrushData> BrushDatas
        {
            get { return mBrushDatas; }
            set
            {
                mBrushDatas = value;

                //WrapPanel_Brush.Children.Clear();
                //WrapPanel_Brush.Children.Add(Button_BrushAdd);

                //foreach (var data in mBrushDatas)
                //{
                //    AddBrush(data);
                //}

                //// 默认选中第一个
                //foreach (var item in WrapPanel_Brush.Children)
                //{
                //    if (item is RadioButton)
                //    {
                //        ((RadioButton)item).IsChecked = true;
                //        break;
                //    }
                //}
                SelectedBrushData = BrushDatas[0];
            }
        }

        private void AddBrush(string fullFileName, bool withSVN = true)
        {
            //var fullFileName = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.EditorSourceDirectory + "\\" + data.BrushFile;

            if (withSVN)
            {
                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"地形编辑器: {fullFileName}笔刷文件通过版本控制上传失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Commit((EditorCommon.VersionControl.VersionControlCommandResult resultCommit) =>
                        {
                            if (resultCommit.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"地形编辑器: {fullFileName}笔刷文件通过版本控制上传失败!");
                            }
                        }, fullFileName, "AutoCommit 地形编辑器添加笔刷");
                    }
                }, fullFileName);
            }

            RadioButton tgBtn = new RadioButton()
            {
                Width = Button_BrushAdd.ActualWidth,
                Height = Button_BrushAdd.ActualHeight,
                Style = this.FindResource("RadioButtonStyle_TGButton") as System.Windows.Style,
                Margin = new Thickness(1, 1, 1, 1),
                GroupName = "BrushType",
            };
            Image image = new Image();
            BitmapSource source = EditorCommon.ImageInit.GetImage(fullFileName) as BitmapSource;//new BitmapImage(new Uri(fullFileName));
            // 判断source的图像格式
            if (source.Format != PixelFormats.Gray8)
            {
                // 重新生成一张8位灰度的图
                FormatConvertedBitmap formatBitmpa = new FormatConvertedBitmap(source, PixelFormats.Gray8, source.Palette, 0);
                source = formatBitmpa.Source;
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                using (System.IO.Stream stream = System.IO.File.Create(fullFileName))
                {
                    encoder.Save(stream);
                }
            }
            image.Source = source;
            tgBtn.Content = image;
            tgBtn.Tag = fullFileName;
            tgBtn.Checked += OnBrushToggleButtonChecked;
            tgBtn.ContextMenu = new System.Windows.Controls.ContextMenu();
            MenuItem mItem = new MenuItem()
            {
                Header = "删除",
                Tag = tgBtn,
            };
            mItem.Click += BrushMenuItem_Delete_Click;
            tgBtn.ContextMenu.Items.Add(mItem);

            WrapPanel_Brush.Children.Insert(WrapPanel_Brush.Children.Count - 1, tgBtn);
        }

        // 笔刷菜单，删除按钮操作
        void BrushMenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            var idx = WrapPanel_Brush.Children.IndexOf((UIElement)(item.Tag));
            if (idx < 0)
                return;

            try
            {
                var fileName = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.EditorSourceDirectory + "\\" + BrushFiles[idx];

                EditorCommon.VersionControl.VersionControlManager.Instance.Update((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                {
                    if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                    {
                        EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"地形编辑器: {fileName}笔刷文件通过版本控制删除失败!");
                    }
                    else
                    {
                        EditorCommon.VersionControl.VersionControlManager.Instance.Delete((EditorCommon.VersionControl.VersionControlCommandResult resultDelete) =>
                        {
                            if (resultDelete.Result != EditorCommon.VersionControl.EProcessResult.Success)
                            {
                                EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"地形编辑器: {fileName}笔刷文件通过版本控制删除失败!");
                            }
                        }, fileName, "AutoCommit 地形编辑器删除笔刷");
                    }
                }, fileName);
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);
            }
            catch
            {
            }

            BrushFiles.RemoveAt(idx);
            WrapPanel_Brush.Children.RemoveAt(idx);

            SaveConfig();
        }

        void OnBrushToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            var tgBtn = sender as System.Windows.Controls.Primitives.ToggleButton;
            mSelectedImage = ((Image)(tgBtn.Content)).Source as BitmapSource;

            mSelectedBrushImage = (string)tgBtn.Tag;
            OnTerrainBrushBrushChanged(mSelectedBrushImage);
//             if (OnBrushChanged != null)
//             {
//                 OnBrushChanged((string)(tgBtn.Tag));
//             }
            //foreach (var child in WrapPanel_Brush.Children)
            //{
            //    if (child == sender)
            //        continue;

            //    if (child is ToggleButton)
            //    {
            //        ((ToggleButton)child).IsChecked = false;
            //    }
            //}
        }

        private void Button_BrushAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "jpg files(*.jpg)|*.jpg|jpeg files(*.jpeg)|*.jpeg|png files(*.png)|*.png|All files(*.*)|*.*";
            ofd.FilterIndex = 4;
            ofd.RestoreDirectory = true;
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 拷贝到EditorSource目录下
                var tagDir = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.EditorSourceDirectory + "\\TerrainEditorConfig\\Brushes";
                if (!System.IO.Directory.Exists(tagDir))
                {
                    System.IO.Directory.CreateDirectory(tagDir);
                    EditorCommon.VersionControl.VersionControlManager.Instance.Add((EditorCommon.VersionControl.VersionControlCommandResult result) =>
                    {
                        if (result.Result != EditorCommon.VersionControl.EProcessResult.Success)
                        {
                            EditorCommon.MessageReport.Instance.ReportMessage(EditorCommon.MessageReport.enMessageType.Warning, $"地形编辑器: 通过版本控制创建笔刷文件夹{tagDir}失败!");
                        }
                    }, tagDir, "AutoCommit 地形编辑器创建地形笔刷目录");
                }

                foreach (var file in ofd.FileNames)
                {
                    var tagFile = tagDir + "\\" + Guid.NewGuid().ToString() + "." + CSUtility.Support.IFileManager.Instance.GetFileExtension(file);
                    System.IO.File.Copy(ofd.FileName, tagFile);
                    var saveFile = CSUtility.Support.IFileManager.Instance._GetRelativePathFromAbsPath(tagFile, CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.EditorSourceDirectory);
                    mBrushFiles.Add(saveFile);

                    AddBrush(tagFile);
                }

                SaveConfig();
            }
        }

        private void GetRotationSize(uint srcSizeX, uint srcSizeY, float rotAngle, out uint tagSizeX, out uint tagSizeY)
        {
            var rotSin = System.Math.Sin(rotAngle);
            var rotCos = System.Math.Cos(rotAngle);

            // 计算源图的四个角的坐标（以图像中心为坐标系原点）
            var srcX1 = -(srcSizeX - 1) / 2;
            var srcY1 = (srcSizeY - 1) / 2;
            var srcX2 = (srcSizeX - 1) / 2;
            var srcY2 = (srcSizeY - 1) / 2;
            var srcX3 = -(srcSizeX - 1) / 2;
            var srcY3 = -(srcSizeY - 1) / 2;
            var srcX4 = (srcSizeX - 1) / 2;
            var srcY4 = -(srcSizeY - 1) / 2;

            // 计算新图的四个角的坐标（以图像中心为坐标系原点）
            var dstX1 = rotCos * srcX1 + rotSin * srcY1;
            var dstY1 = -rotSin * srcX1 + rotCos * srcY1;
            var dstX2 = rotCos * srcX2 + rotSin * srcY2;
            var dstY2 = -rotSin * srcX2 + rotCos * srcY2;
            var dstX3 = rotCos * srcX3 + rotSin * srcY3;
            var dstY3 = -rotSin * srcX3 + rotCos * srcY3;
            var dstX4 = rotCos * srcX4 + rotSin * srcY4;
            var dstY4 = -rotSin * srcX4 + rotCos * srcY4;

            // 计算旋转后的图像宽度
            tagSizeX = (uint)System.Math.Max(System.Math.Abs(dstX4 - dstX1), System.Math.Abs(dstX3 - dstX2) + 0.5);
            // 计算旋转后的图像高度
            tagSizeY = (uint)System.Math.Max(System.Math.Abs(dstY4 - dstY1), System.Math.Abs(dstY3 - dstY2) + 0.5);
        }

        public byte[] GetRotateBrushData(byte[] srcBrush, uint srcSizeX, uint srcSizeY, float rotAngle, out uint tagSizeX, out uint tagSizeY)
        {
            var rotSin = System.Math.Sin(rotAngle);
            var rotCos = System.Math.Cos(rotAngle);

            GetRotationSize(srcSizeX, srcSizeY, rotAngle, out tagSizeX, out tagSizeY);
            //// 计算源图的四个角的坐标（以图像中心为坐标系原点）
            //// 3----4
            //// |    |
            //// 1----2
            //var srcX1 = -(srcSizeX - 1) / 2;
            //var srcY1 = (srcSizeY - 1) / 2;
            //var srcX2 = (srcSizeX - 1) / 2;
            //var srcY2 = (srcSizeY - 1) / 2;
            //var srcX3 = -(srcSizeX - 1) / 2;
            //var srcY3 = -(srcSizeY - 1) / 2;
            //var srcX4 = (srcSizeX - 1) / 2;
            //var srcY4 = -(srcSizeY - 1) / 2;

            //// 计算新图的四个角的坐标（以图像中心为坐标系原点）
            //var dstX1 = rotCos * srcX1 + rotSin * srcY1;
            //var dstY1 = -rotSin * srcX1 + rotCos * srcY1;
            //var dstX2 = rotCos * srcX2 + rotSin * srcY2;
            //var dstY2 = -rotSin * srcX2 + rotCos * srcY2;
            //var dstX3 = rotCos * srcX3 + rotSin * srcY3;
            //var dstY3 = -rotSin * srcX3 + rotCos * srcY3;
            //var dstX4 = rotCos * srcX4 + rotSin * srcY4;
            //var dstY4 = -rotSin * srcX4 + rotCos * srcY4;

            //// 计算旋转后的图像宽度
            //tagSizeX = (uint)System.Math.Max(System.Math.Abs(dstX4 - dstX1), System.Math.Abs(dstX3 - dstX2) + 0.5);
            //// 计算旋转后的图像高度
            //tagSizeY = (uint)System.Math.Max(System.Math.Abs(dstY4 - dstY1), System.Math.Abs(dstY3 - dstY2) + 0.5);

            // 两个常数
            var f1 = (-0.5 * (tagSizeX - 1) * rotCos - 0.5 * (tagSizeY - 1) * rotSin + 0.5 * (srcSizeX - 1));
            var f2 = (0.5 * (tagSizeX - 1) * rotSin - 0.5 * (tagSizeY - 1) * rotCos + 0.5 * (srcSizeY - 1));

            byte[] retByte = new byte[tagSizeX * tagSizeY];
            for (var i = 0; i < tagSizeY; i++)
            {
                for (var j = 0; j < tagSizeX; j++)
                {
                    var i0 = (uint)(srcSizeY - (-j * rotSin + i * rotCos + f2 + 0.5));
                    var j0 = (uint)(j * rotCos + i * rotSin + f1 + 0.5);

                    // 判断是否在原图范围内
                    if ((j0 >= 0) && (j0 < srcSizeX) && (i0 >= 0) && (i0 < srcSizeY))
                    {
                        retByte[i * tagSizeX + j] = srcBrush[i0 * srcSizeX + j0];
                    }
                    else
                        retByte[i * tagSizeX + j] = 0;
                }
            }

            return retByte;
        }

        public byte[] GetBrushData(uint sizeX, uint sizeY, out byte dataValueRange, float rotAngle)
        {
            if (mSelectedImage == null)
            {
                dataValueRange = 1;
                return null;
            }

            byte[] retValue = new byte[sizeX * sizeY];

            var stride = mSelectedImage.Format.BitsPerPixel * mSelectedImage.PixelWidth / 8;
            byte[] pixelData = new byte[mSelectedImage.PixelHeight * stride];
            mSelectedImage.CopyPixels(pixelData, stride, 0);

            uint tagImageSizeX = 0, tagImageSizeY = 0;
            byte[] tagImagePixelData;

            if (CanRotateBrush)
            {
                tagImagePixelData = GetRotateBrushData(pixelData, (uint)mSelectedImage.PixelWidth, (uint)mSelectedImage.PixelHeight, rotAngle, out tagImageSizeX, out tagImageSizeY);
            }
            else
            {
                tagImagePixelData = pixelData;
                tagImageSizeX = (uint)mSelectedImage.PixelWidth;
                tagImageSizeY = (uint)mSelectedImage.PixelHeight;
            }

            //uint tagSizeX, tagSizeY;
            //GetRotationSize(sizeX, sizeY, rotAngle, out tagSizeX, out tagSizeY);

            byte maxValue = 0;
            byte minValue = 255;
            for (uint i = 0; i < sizeX; i++)
            {
                for (uint j = 0; j < sizeY; j++)
                {
                    var pixelX = i * tagImageSizeX / sizeX;
                    var pixelY = j * tagImageSizeY / sizeY;

                    retValue[j * sizeX + i] = tagImagePixelData[pixelY * tagImageSizeX + pixelX];

                    if (retValue[j * sizeX + i] > maxValue)
                        maxValue = retValue[j * sizeX + i];
                    if (retValue[j * sizeX + i] < minValue)
                        minValue = retValue[j * sizeX + i];
                }
            }

            dataValueRange = (byte)(maxValue - minValue);
            if (dataValueRange == 0)
                dataValueRange = 1;

            return retValue;
        }

        #endregion

        bool mIsLoaded = false;
        private void panelBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadConfig();
            mIsLoaded = true;
            HostTerrain = CCore.Client.MainWorldInstance.Terrain;

            UpdateTerrainMaterialLayers(HostTerrain);
            InitializeTerrainLayers();
        }

        private void panelBase_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveConfig();
            HostTerrain = CCore.Client.MainWorldInstance.Terrain;

            UpdateTerrainMaterialLayers(HostTerrain);
            InitializeTerrainLayers();
        }

        private void ToggleButton_Tools_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var tgBtn = sender as System.Windows.Controls.Primitives.ToggleButton;
            if (tgBtn.Tag != null)
            {
                TerrainToolType = (enTerrainToolType)System.Enum.Parse(typeof(enTerrainToolType), tgBtn.Tag.ToString());

                SelectedBrushData = BrushDatas[(int)TerrainToolType];
            }

            //foreach (var child in WrapPanel_Tools.Children)
            //{
            //    if (child == sender)
            //        continue;

            //    if (child is ToggleButton)
            //    {
            //        ((ToggleButton)child).IsChecked = false;
            //    }
            //}
        }

        private void ToggleButton_BrushMode_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var tgBtn = sender as System.Windows.Controls.Primitives.ToggleButton;
            if (tgBtn.Tag != null)
                TerrainBrushMode = (enTerrainBrushMode)System.Enum.Parse(typeof(enTerrainBrushMode), tgBtn.Tag.ToString());

            //foreach (var child in WrapPanel_BrushMode.Children)
            //{
            //    if (child == sender)
            //        continue;

            //    if (child is ToggleButton)
            //    {
            //        ((ToggleButton)child).IsChecked = false;
            //    }
            //}
        }

        #region 层操作

        LayerItem mSelectedLayerItem = null;
        public LayerItem SelectedLayerItem
        {
            get { return mSelectedLayerItem; }
        }

        List<Guid> mMaterialIdList = new List<Guid>();
        public List<Guid> MaterialIdList
        {
            get { return mMaterialIdList; }
        }
        List<CCore.Grass.GrassData> mGrassList = new List<CCore.Grass.GrassData>();
        public List<CCore.Grass.GrassData> GrassList
        {
            get { return mGrassList; }
        }
        List<string> mRemarkList = new List<string>();
        public List<string> RemarkList
        {
            get { return mRemarkList; }
        }

        public void UpdateTerrainMaterialLayers(CCore.Terrain.Terrain terrain)
        {
            if (terrain != null)
            {
                mMaterialIdList.Clear();
                mGrassList.Clear();
                mRemarkList.Clear();

                List<System.IntPtr> tempList = new List<System.IntPtr>();
                terrain.GetLayerMaterials(mMaterialIdList, tempList, mRemarkList);
                var baseMaterial = terrain.GetBaseMaterial();
                if (baseMaterial != null && mMaterialIdList.Contains(baseMaterial))
                {
                    mMaterialIdList.Remove(baseMaterial);
                    mMaterialIdList.Insert(0, baseMaterial);
                }
                int i = 0;
                foreach (var innerPtr in tempList)
                {
                    var grass = new CCore.Grass.GrassData(innerPtr);
                    if (i < mMaterialIdList.Count)
                    {
                        grass.OwnerMatId = mMaterialIdList[i];
                    }
                    mGrassList.Add(grass);
                    i++;
                }

                if (mIsLoaded)
                {
                    InitializeTerrainLayers();
                }
            }
        }

        private void InitializeTerrainLayers()
        {
            if (HostTerrain == null)
                return;

            ListBox_Layer.Items.Clear();

            // 高度层
            AddLayer(LayerItem.enLayerType.HightMap);

            var material = HostTerrain.GetBaseMaterial();
            // 材质层
            for (int i = 0; i < mMaterialIdList.Count; ++i)
            {
                var item = AddLayer(LayerItem.enLayerType.Material);
                item.SetMaterialData(mMaterialIdList[i]);
                if (i < mGrassList.Count)
                    item.Grass = mGrassList[i];
                else
                    item.Grass = null;
                item.Remarks = mRemarkList[i];
                if (material == mMaterialIdList[i])
                {
                    var brush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    item.Border_LayerItem.BorderBrush = brush;
                }
            }
            //foreach (var matId in mMaterialIdList)
            //{
            //    var item = AddLayer(Terrain.LayerItem.enLayerType.Material);
            //    item.SetMaterialData(matId);
            //}

            ListBox_Layer.SelectedIndex = 0;
        }

        private void InitializeLayerComboBox()
        {
            for (var i = LayerItem.enLayerType.Material; i < LayerItem.enLayerType.Count; i++)
            {
                ComboBox_LayerType.Items.Add(i);
            }
            ComboBox_LayerType.SelectedIndex = 0;
        }

        private void Button_AddLayer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HostTerrain == null)
                return;

            var layerType = (LayerItem.enLayerType)ComboBox_LayerType.SelectedItem;
            var layerItem = AddLayer(layerType);
            layerItem.Grass = new CCore.Grass.GrassData();

            switch (layerItem.LayerType)
            {
                case LayerItem.enLayerType.Material:
                    {
                        //mHostTerrain.AddLayerMaterial(layerItem.MaterialId);
                        //UpdateTerrainMaterialLayers(mHostTerrain);
                        //InitializeTerrainLayers();
                    }
                    break;
            }

            CCore.Client.MainWorldInstance.IsDirty = true;
        }

        private void Button_DelLayer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HostTerrain == null)
                return;

            if (ListBox_Layer.SelectedIndex < 1)
                return;

            var layerItem = ListBox_Layer.SelectedItem as LayerItem;
            switch (layerItem.LayerType)
            {
                case LayerItem.enLayerType.Material:
                    {
                        if (HostTerrain.GetBaseMaterial() == layerItem.MaterialId)
                        {
                            EditorCommon.MessageBox.Show("地形基础纹理不能删除!");
                            return;
                        }

                        if (layerItem.MaterialId != Guid.Empty)
                            HostTerrain.RemoveLayerMaterial(layerItem.MaterialId);
                    }
                    break;
            }

            //if (OnDeleteLayer != null)
            //    OnDeleteLayer(ListBox_Layer.SelectedItem as Terrain.LayerItem);
            CCore.Client.MainWorldInstance.IsDirty = true;

            ListBox_Layer.Items.RemoveAt(ListBox_Layer.SelectedIndex);
        }

        public LayerItem AddLayer(LayerItem.enLayerType layerType)
        {
            LayerItem item = new LayerItem(layerType, this);
            ListBox_Layer.Items.Add(item);
            ListBox_Layer.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("LayerType", System.ComponentModel.ListSortDirection.Ascending));

            return item;
        }

        public System.Windows.Controls.ItemCollection GetLayers()
        {
            return ListBox_Layer.Items;
        }

        //public Terrain.LayerItem GetSelectedLayerItem()
        //{
        //    //if (ListBox_Layer.SelectedIndex < 0)
        //    //    return null;

        //    return ListBox_Layer.SelectedItem as Terrain.LayerItem;
        //}

        public void _OnResetLayer(LayerItem item)
        {
            CCore.Client.MainWorldInstance.IsDirty = true;

            switch (item.LayerType)
            {
                case LayerItem.enLayerType.Material:
                    {
                        var idx = mMaterialIdList.IndexOf(item.OldMaterialId);
                        if (idx >= 0)
                        {
                            MaterialIdList[idx] = item.MaterialId;
                        }
                    }
                    break;
            }
            
            OnResetTerrainLayer(item);
        }

        public void RefreshTerrainEffect(CCore.Material.Material mtl)
        {
            OnRefreshTerrainEffect(mtl);
        }

        private void ListBox_Layer_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            mSelectedLayerItem = ListBox_Layer.SelectedItem as LayerItem;
        }

        public void SelectedLayerWithId(Guid matId)
        {
            foreach (LayerItem item in ListBox_Layer.Items)
            {
                if (item.LayerType == LayerItem.enLayerType.Material)
                {
                    if (item.MaterialId == matId)
                    {
                        ListBox_Layer.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        #endregion

        private void Button_TerrainGridEditor_Click(object sender, System.Windows.RoutedEventArgs e)
        {
#warning 插件化备注：打开TerrainGridEditor窗口
            ////////////FlyWindow window = new FlyWindow();
            ////////////window.SizeToContent = SizeToContent.WidthAndHeight;
            //////////////System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window);
            //////////////Terrain.TerrainGridPanel.Instance.HostTerrain = HostTerrain;
            ////////////window.SetControl(Terrain.TerrainGridPanel.Instance, "地形网格编辑");
            ////////////window.Show();
        }

        private void Button_UpdateTerrainClick(object sender, RoutedEventArgs e)
        {
            //if (HostTerrain == null || HostTerrain.Inner == IntPtr.Zero)
            //{
            //    TCControl.UpdateTerrain(HostTerrain);
            //    return;
            //}

            //var x = HostTerrain.GetPatchWidth() / 2f;
            //var z = HostTerrain.GetPatchHeight() / 2f;
            //if (HostTerrain.TravelTo(x, z))
            //{
            //    TCControl.UpdateTerrain(HostTerrain);
            //}
        }

        private void Button_DeleteTerrainClick(object sender, RoutedEventArgs e)
        {
            HostTerrain.Cleanup();
        }

        #region 地形编辑

        bool m_bCtrlKeyIsDown = false;
        //bool m_bStartMove = false;
        bool m_CanDrawTerrain = true;
        // 记录从鼠标按下到抬起后地形变化量
        Dictionary<uint, Dictionary<uint, int>> mTerrainRisedDataDic = new Dictionary<uint, Dictionary<uint, int>>();
        SlimDX.Vector3 mPreMouseIntersectWithTerrainPoint = SlimDX.Vector3.Zero;
        private short mMouseLBDownPtTerrainHeight = 0;

        //地形的4个点
        SlimDX.Vector3 mLeftTopPos = SlimDX.Vector3.Zero;
        SlimDX.Vector3 mRightTopPos = SlimDX.Vector3.Zero;
        SlimDX.Vector3 mLeftDownPos = SlimDX.Vector3.Zero;
        SlimDX.Vector3 mRightDownPos = SlimDX.Vector3.Zero;

        SlimDX.Vector3 mTerrainIntersectPt = SlimDX.Vector3.Zero;

        SlimDX.Plane mObliqueSlipPlane;
        //System.Drawing.Point mStartRotateCameraMousePos;

        //用来redo或undo的地形变化量
        Dictionary<uint, Dictionary<uint, int>> mOldTerrainModifyDataDic = new Dictionary<uint, Dictionary<uint, int>>();
        Dictionary<uint, Dictionary<uint, int>> mTerraindModifyDataDic = new Dictionary<uint, Dictionary<uint, int>>();

        void GetRectanglePoints(SlimDX.Vector3 brushLocBegin, SlimDX.Vector3 brushLocEnd, bool isTopToDown, float brushRadius)
        {
            var VerticalYLoc = new SlimDX.Vector3(0, 1, 0);
            var dragLength = brushLocEnd - brushLocBegin;
            if (isTopToDown)
            {
                var loc = SlimDX.Vector3.Cross(brushLocEnd - brushLocBegin, VerticalYLoc);
                var vectorLoc = SlimDX.Vector3.Normalize(loc);
                mLeftTopPos = vectorLoc * brushRadius + brushLocBegin;
                mLeftDownPos = vectorLoc * brushRadius + brushLocEnd;
                loc = SlimDX.Vector3.Cross(brushLocBegin - brushLocEnd, VerticalYLoc);
                vectorLoc = SlimDX.Vector3.Normalize(loc);
                mRightTopPos = vectorLoc * brushRadius + brushLocBegin;
                mRightDownPos = vectorLoc * brushRadius + brushLocEnd;
            }
            else if (!isTopToDown)
            {
                var loc = SlimDX.Vector3.Cross(brushLocEnd - brushLocBegin, VerticalYLoc);
                var vectorLoc = SlimDX.Vector3.Normalize(loc);
                mLeftTopPos = vectorLoc * brushRadius + brushLocEnd;
                mLeftDownPos = vectorLoc * brushRadius + brushLocBegin;
                loc = SlimDX.Vector3.Cross(brushLocBegin - brushLocEnd, VerticalYLoc);
                vectorLoc = SlimDX.Vector3.Normalize(loc);
                mRightTopPos = vectorLoc * brushRadius + brushLocEnd;
                mRightDownPos = vectorLoc * brushRadius + brushLocBegin;
            }
        }

        private void InitTerrainBrushActor()
        {
            if(mTerrainBrushActor != null)
            {
                CCore.Client.MainWorldInstance.RemoveEditorActor(mTerrainBrushActor);
                mTerrainBrushActor.Cleanup();
            }

            // 设置地形刷
            mTerrainBrushActor = new CCore.Support.ITerrainBrushActor();
            var tbActorInit = new CCore.Support.ITerrainBrushInit();
            mTerrainBrushActor.Initialize(tbActorInit);
            //mTerrainBrushActor.SetRadius(20, 30);
            mTerrainBrushActor.AddFlag(CSUtility.Component.ActorInitBase.EActorFlag.ForEditor);
            mTerrainBrushActor.ActorName = "地形刷";
            CCore.Client.MainWorldInstance.AddEditorActor(mTerrainBrushActor);
            //UpdateTerrainBrush();
            mTerrainBrushActor.Visible = false;

            // 默认选中第一个
            foreach (var item in WrapPanel_Brush.Children)
            {
                if (item is RadioButton)
                {
                    ((RadioButton)item).IsChecked = true;
                    break;
                }
            }
        }

        void DrawTerrainWithMouse(BehaviorParameter parameter)
        {
            if (!mTerrainBrushActor.Visible || HostTerrain == null)
                return;

            if (mSelectedLayerItem == null)
                return;

//             if (m_bStartMove)
//             {
//                 //清理上次笔刷数据
//                 mTerrainRisedDataDic.Clear();
//             }

            if (parameter is CCore.MsgProc.Behavior.Mouse_Move)
            {
                var param = parameter as CCore.MsgProc.Behavior.Mouse_Move;
                if (param.button == CCore.MsgProc.Behavior.Mouse_Move.MouseButtons.Left)
                {
                    if (TerrainToolType != enTerrainToolType.Slope)
                    {
                        Dictionary<uint, Dictionary<uint, int>> terrainModifyDataDic = new Dictionary<uint, Dictionary<uint, int>>();
                        var terrainIntersectPt = mTerrainBrushActor.Placement.GetLocation();
                        // 计算间隔
                        terrainIntersectPt.Y = 0;
                        mPreMouseIntersectWithTerrainPoint.Y = 0;
                        var dirPt = terrainIntersectPt - mPreMouseIntersectWithTerrainPoint;
                        var length = dirPt.Length();
                        var brushInterval = TerrainBrushInterval * TerrainBrushRadius;
                        if (length > brushInterval)
                        {
                            dirPt.Normalize();
                            float dirAngle = 0;
                            var commonDir = new SlimDX.Vector3(1, 0, 0);
                            var dirCross = SlimDX.Vector3.Cross(commonDir, dirPt);
                            var acosDir = (float)System.Math.Acos(SlimDX.Vector3.Dot(commonDir, dirPt));
                            if (dirCross.Y > 0)
                                dirAngle = (float)(System.Math.PI / 2 - acosDir);
                            else
                                dirAngle = (float)(System.Math.PI / 2 + acosDir);
                            if (brushInterval == 0)
                                brushInterval = length;
                            var drawTimes = (int)(length / brushInterval);

                            // 取得笔刷覆盖范围
                            for (int i = 0; i < drawTimes; i++)
                            {
                                var loc = SlimDX.Vector3.Multiply(dirPt, (float)((i + 1) * brushInterval)) + mPreMouseIntersectWithTerrainPoint;
                                //DrawTerrain(param.button, loc);
                                GetBrushTerrainPointList(loc, (float)TerrainBrushRadius, dirAngle, terrainModifyDataDic);
                            }

                            // 绘制地形
                            if (m_bCtrlKeyIsDown)//param.button == 0x0009)
                                DrawTerrain(terrainModifyDataDic, false);
                            else
                                DrawTerrain(terrainModifyDataDic, true);

                            if (TerrainToolType != enTerrainToolType.ObliqueSlip)
                                mPreMouseIntersectWithTerrainPoint = terrainIntersectPt;

                            if (mTerrainBrushActor != null && CanRotateBrush)
                                mTerrainBrushActor.BrushAngle = (float)(-dirAngle);

                            // World置脏
                            CCore.Client.MainWorldInstance.IsDirty = true;
                        }
                        //Log.FileLog.WriteLine(string.Format("terrainIntersectPt_{0},mPreMouseIntersectWithTerrainPoint_{1}", terrainIntersectPt, mPreMouseIntersectWithTerrainPoint));
                    }
                    else
                    {
                        mTerrainIntersectPt = EditorCommon.Assist.Assist.IntersectWithTerrain(param.X, param.Y, true, EditorCommon.Program.GameREnviroment?.Camera);
                        //var u = HostTerrain.GetUWithX(mTerrainIntersectPt.X);
                        //var v = HostTerrain.GetVWithZ(mTerrainIntersectPt.Z);
                        //short height = 0;
                        //HostTerrain.GetHeight(u, v, ref height, true);
                        //SlimDX.Vector3 point1 = new SlimDX.Vector3(u, height, v);
                        //u = HostTerrain.GetUWithX(mPreMouseIntersectWithTerrainPoint.X);
                        //v = HostTerrain.GetVWithZ(mPreMouseIntersectWithTerrainPoint.Z);
                        //HostTerrain.GetHeight(u, v, ref height, true);
                        //SlimDX.Vector3 point2 = new SlimDX.Vector3(u, height, v);
                        bool isTopToDown = false;
                        if (mPreMouseIntersectWithTerrainPoint.Z < mTerrainIntersectPt.Z)
                        {
                            isTopToDown = true;
                        }
                        GetRectanglePoints(mPreMouseIntersectWithTerrainPoint, mTerrainIntersectPt, isTopToDown, (float)TerrainBrushRadius);
                        mTerrainBrushActor.BrushType = CCore.Support.ITerrainBrushVisual.enBrushType.RectCg;

                        mTerrainBrushActor.SetRectSize(mLeftTopPos, mRightTopPos, mLeftDownPos, mRightDownPos);

                        //mTerrainIntersectPt = EditorCommon.Assist.Assist.IntersectWithTerrain(param.X, param.Y, true, EditorCommon.Program.GameREnviroment?.Camera);
                        //bool isTopToDown = false;
                        //if (mPreMouseIntersectWithTerrainPoint.Z < mTerrainIntersectPt.Z)
                        //{
                        //    isTopToDown = true;
                        //}
                        //GetRectanglePoints(mPreMouseIntersectWithTerrainPoint, mTerrainIntersectPt, isTopToDown, (float)TerrainBrushRadius);
                        //mTerrainBrushActor.BrushType = CCore.Support.ITerrainBrushVisual.enBrushType.RectCg;
                        //var distance = Math.Sqrt(Math.Pow(Math.Abs(mTerrainIntersectPt.X - mPreMouseIntersectWithTerrainPoint.X), 2) +
                        //    Math.Pow(Math.Abs(mTerrainIntersectPt.Z - mPreMouseIntersectWithTerrainPoint.Z), 2));

                        //var height = GetHeight(TerrainBrushStrength, distance) / 10d;
                        //if (isTopToDown)
                        //{
                        //    mLeftDownPos.Y = (float)height;
                        //    mRightDownPos.Y = (float)height;
                        //}
                        //else
                        //{
                        //    mLeftTopPos.Y = (float)height;
                        //    mRightTopPos.Y = (float)height;
                        //}
                        //mTerrainBrushActor.SetRectSize(mLeftTopPos, mRightTopPos, mLeftDownPos, mRightDownPos);
                    }
                }
            }
        }

        void GetBrushTerrainPointList(SlimDX.Vector3 brushLoc, float brushRadius, float brushAngle, Dictionary<uint, Dictionary<uint, int>> terrainModifyData)
        {
            var minU = HostTerrain.GetUWithX(brushLoc.X - brushRadius);
            var minV = HostTerrain.GetVWithZ(brushLoc.Z - brushRadius);
            var maxU = HostTerrain.GetUWithX(brushLoc.X + brushRadius);
            var maxV = HostTerrain.GetVWithZ(brushLoc.Z + brushRadius);

            byte range = 1;
            var brushData = GetBrushData(maxU - minU + 1, maxV - minV + 1, out range, brushAngle);
            if (brushData != null)
            {
                switch (TerrainToolType)
                {
                    case enTerrainToolType.Generic:
                        {
                            for (var i = minU; i <= maxU; i++)
                            {
                                for (var j = minV; j <= maxV; j++)
                                {
                                    uint idx = (j - minV) * (maxU - minU + 1) + (i - minU);
                                    if (idx >= brushData.Length)
                                        idx = (uint)(brushData.Length - 1);
                                    var pixData = brushData[idx];
                                    int modifyData = (int)(TerrainBrushStrength * pixData * range / 255 / 25);

                                    Dictionary<uint, int> dicModifyDatas = null;
                                    if (!terrainModifyData.TryGetValue(i, out dicModifyDatas))
                                    {
                                        dicModifyDatas = new Dictionary<uint, int>();
                                        terrainModifyData[i] = dicModifyDatas;
                                    }

                                    int dicData = 0;
                                    if (dicModifyDatas.TryGetValue(j, out dicData))
                                        modifyData = (dicData + modifyData) / 2;//System.Math.Max(dicData, modifyData);

                                    dicModifyDatas[j] = modifyData;
                                }
                            }
                        }
                        break;

                    case enTerrainToolType.Flatten:
                    case enTerrainToolType.Layer:
                        {
                            for (var i = minU; i <= maxU; i++)
                            {
                                for (var j = minV; j <= maxV; j++)
                                {
                                    uint idx = (j - minV) * (maxU - minU + 1) + (i - minU);
                                    if (idx >= brushData.Length)
                                        idx = (uint)(brushData.Length - 1);
                                    var pixData = brushData[idx];
                                    int modifyData = (int)(TerrainBrushStrength * pixData * range / 255 / 25);

                                    Dictionary<uint, int> dicModifyDatas = null;
                                    if (!terrainModifyData.TryGetValue(i, out dicModifyDatas))
                                    {
                                        dicModifyDatas = new Dictionary<uint, int>();
                                        terrainModifyData[i] = dicModifyDatas;
                                    }

                                    int dicData = 0;
                                    if (dicModifyDatas.TryGetValue(j, out dicData))
                                        modifyData = System.Math.Max(dicData, modifyData);

                                    dicModifyDatas[j] = modifyData;
                                }
                            }
                        }
                        break;
                    case enTerrainToolType.Smooth:
                        {
                            float k = (float)(1 - (TerrainBrushStrength - BrushMinStrength) /
                                              (BrushMaxStrength - BrushMinStrength) * 0.5);
                            var heightDatas = new short[(maxU - minU + 1) * (maxV - minV + 1)];
                            for (var i = minU; i <= maxU; i++)
                            {
                                for (var j = minV; j <= maxV; j++)
                                {
                                    uint idx = (j - minV) * (maxU - minU + 1) + (i - minU);
                                    if (idx >= brushData.Length)
                                        idx = (uint)(brushData.Length - 1);
                                    var pixData = brushData[idx];
                                    double temp = TerrainBrushStrength * pixData * range / 255;
                                    if (temp > 0)
                                    {
                                        short value = 0, valueLeft = 0, valueRight = 0, valueTop = 0, valueBottom = 0;
                                        HostTerrain.GetHeight(i, j, ref value, false);
                                        HostTerrain.GetHeight(i - 1, j, ref valueLeft, false);
                                        HostTerrain.GetHeight(i + 1, j, ref valueRight, false);
                                        HostTerrain.GetHeight(i, j - 1, ref valueTop, false);
                                        HostTerrain.GetHeight(i, j + 1, ref valueBottom, false);

                                        var newValue = valueLeft * (1 - k) + value * k;
                                        newValue = valueRight * (1 - k) + newValue * k;
                                        newValue = valueTop * (1 - k) + newValue * k;
                                        newValue = valueBottom * (1 - k) + newValue * k;

                                        Dictionary<uint, int> dicModifyDatas = null;
                                        if (!terrainModifyData.TryGetValue(i, out dicModifyDatas))
                                        {
                                            dicModifyDatas = new Dictionary<uint, int>();
                                            terrainModifyData[i] = dicModifyDatas;
                                        }

                                        int dicData = 0;
                                        if (dicModifyDatas.TryGetValue(j, out dicData))
                                            newValue = System.Math.Max(dicData, newValue);

                                        dicModifyDatas[j] = (int)newValue;
                                    }
                                }
                            }
                        }
                        break;

                    case enTerrainToolType.AddPatch:
                    case enTerrainToolType.DelPatch:
                        {
                            var u = HostTerrain.GetUWithX(brushLoc.X);
                            var v = HostTerrain.GetVWithZ(brushLoc.Z);
                            Dictionary<uint, int> dicModifyDatas = null;
                            if (!terrainModifyData.TryGetValue(u, out dicModifyDatas))
                            {
                                dicModifyDatas = new Dictionary<uint, int>();
                                terrainModifyData[u] = dicModifyDatas;
                            }

                            dicModifyDatas[v] = 0;
                        }
                        break;

                    case enTerrainToolType.PaintBrush:
                        {
                            for (var i = minU; i <= maxU; i++)
                            {
                                for (var j = minV; j <= maxV; j++)
                                {
                                    uint idx = (j - minV) * (maxU - minU + 1) + (i - minU);
                                    if (idx >= brushData.Length)
                                        idx = (uint)(brushData.Length - 1);
                                    var pixData = brushData[idx];
                                    int modifyValue = (int)(TerrainBrushStrength * pixData * range / 255);

                                    Dictionary<uint, int> dicModifyDatas = null;
                                    if (!terrainModifyData.TryGetValue(i, out dicModifyDatas))
                                    {
                                        dicModifyDatas = new Dictionary<uint, int>();
                                        terrainModifyData[i] = dicModifyDatas;
                                    }

                                    int dicData = 0;
                                    if (dicModifyDatas.TryGetValue(j, out dicData))
                                        modifyValue = System.Math.Max(dicData, modifyValue);

                                    dicModifyDatas[j] = (int)modifyValue;
                                }
                            }
                        }
                        break;
                    case enTerrainToolType.ObliqueSlip:
                        {
                            var preU = HostTerrain.GetUWithX(mPreMouseIntersectWithTerrainPoint.X);
                            var preV = HostTerrain.GetVWithZ(mPreMouseIntersectWithTerrainPoint.Z);

                            for (var i = minU; i <= maxU; i++)
                            {
                                for (var j = minV; j < maxV; j++)
                                {
                                    uint idx = (j - minV) * (maxU - minU + 1) + (i - minU);
                                    if (idx >= brushData.Length)
                                        idx = (uint)(brushData.Length - 1);
                                    var pixDarta = brushData[idx];
                                    if (pixDarta == 0)
                                        continue;

                                    var height = -(mObliqueSlipPlane.Normal.X * (float)i + mObliqueSlipPlane.Normal.Z * (float)j + mObliqueSlipPlane.D) / mObliqueSlipPlane.Normal.Y;

                                    Dictionary<uint, int> dicModifyDatas = null;
                                    if (!terrainModifyData.TryGetValue(i, out dicModifyDatas))
                                    {
                                        dicModifyDatas = new Dictionary<uint, int>();
                                        terrainModifyData[i] = dicModifyDatas;
                                    }
                                    dicModifyDatas[j] = (int)height;
                                }
                            }
                        }
                        break;
                }
            }
        }

        void SetActorHeight(Dictionary<uint, Dictionary<uint, int>> terrainModifyData, bool canSetRoleActorHeight)
        {
            if (!canSetRoleActorHeight)
                return;

            uint minU, minV, maxU, maxV = 0;
            int minValue = 0;
            int maxValue = 0;
            List<uint> Ukeys = new List<uint>();
            List<uint> Vkeys = new List<uint>();
            foreach (var i in terrainModifyData.Keys)
            {
                Ukeys.Add(i);
                foreach (var j in terrainModifyData[i].Keys)
                {
                    Vkeys.Add(j);
                    minValue = (minValue <= terrainModifyData[i][j] ? minValue : terrainModifyData[i][j]);
                    maxValue = (maxValue >= terrainModifyData[i][j] ? maxValue : terrainModifyData[i][j]);
                }
            }
            minU = Ukeys[0];
            minV = Vkeys[0];
            maxU = Ukeys[Ukeys.Count - 1];
            maxV = Vkeys[Vkeys.Count - 1];

            SlimDX.Vector3 startPoint = new SlimDX.Vector3(minU, minValue, minV);
            SlimDX.Vector3 endPoint = new SlimDX.Vector3(maxU, maxValue, maxV);
            var actors = CCore.Client.MainWorldInstance.GetActors(ref startPoint, ref endPoint, 0);
            foreach (var actor in actors)
            {
                float height = 0;
                var pt = actor.Placement.GetLocation();
                HostTerrain.GetHeightF(pt.X, pt.Z, ref height, true);
                SlimDX.Vector3 start = pt;
                SlimDX.Vector3 end = pt;
                start.Y += 100;
                end.Y -= 100;
//                 if (pt.Y > height)
//                 {
//                     start.Y += 1;
//                     end.Y = height - pt.Y + height - 1;
//                 }
//                 else
//                 {
//                     start.Y = height - pt.Y + height + 1;
//                     end.Y -= 1;
//                 }
                var result = new CSUtility.Support.stHitResult();
                if (!HostTerrain.LineCheck(ref start, ref end, ref result, false))
                    continue;
                var hitPos = result.mHitPosition;
                //hitPos.Y += Math.Abs(height - pt.Y);
                actor.Placement.SetLocation(hitPos);
            }
        }

        void GetTerrainModifyDataDic(uint x, uint z, int height, int modifyData)
        {
            Dictionary<uint, int> oldRisedDataDic = null;
            if (!mOldTerrainModifyDataDic.TryGetValue(x, out oldRisedDataDic))
            {
                oldRisedDataDic = new Dictionary<uint, int>();
                mOldTerrainModifyDataDic[x] = oldRisedDataDic;
                oldRisedDataDic[z] = height;
            }
            else
            {
                if (!oldRisedDataDic.ContainsKey(z))
                    oldRisedDataDic[z] = height;
            }

            Dictionary<uint, int> risedDataDic = null;
            if (!mTerraindModifyDataDic.TryGetValue(x, out risedDataDic))
            {
                risedDataDic = new Dictionary<uint, int>();
                mTerraindModifyDataDic[x] = risedDataDic;
            }
            risedDataDic[z] = modifyData;
        }

        void DrawTerrain(Dictionary<uint, Dictionary<uint, int>> terrainModifyData, bool Increase)
        {
            if (SelectedLayerItem == null)
                return;

            switch (SelectedLayerItem.LayerType)
            {
                case LayerItem.enLayerType.HightMap:
                    {
                        //GetTerrainModifyDataDic(terrainModifyData);
                        switch (TerrainToolType)
                        {
                            case enTerrainToolType.Generic:
                                {
                                    foreach (var i in terrainModifyData.Keys)
                                    {
                                        foreach (var j in terrainModifyData[i].Keys)
                                        {
                                            short height = 0;
                                            HostTerrain.GetHeight(i, j, ref height, false);

                                            int modifyData = 0;
                                            if (Increase)
                                                modifyData = terrainModifyData[i][j];
                                            else
                                                modifyData = -terrainModifyData[i][j];

                                            GetTerrainModifyDataDic(i, j, height, modifyData + height);

                                            HostTerrain.RiseHeight(i, j, (short)modifyData, true);
                                        }
                                    }
                                }
                                break;

                            case enTerrainToolType.Flatten:
                                {
                                    foreach (var i in terrainModifyData.Keys)
                                    {
                                        foreach (var j in terrainModifyData[i].Keys)
                                        {
                                            short height = 0;
                                            HostTerrain.GetHeight(i, j, ref height, false);

                                            var modify = height;

                                            if (Increase && terrainModifyData[i][j] > 0)
                                                modify = mMouseLBDownPtTerrainHeight;
                                            else if (!Increase && terrainModifyData[i][j] == 0)
                                                modify = mMouseLBDownPtTerrainHeight;

                                            GetTerrainModifyDataDic(i, j, height, modify);

                                            if (modify != height)
                                                HostTerrain.SetHeight(i, j, modify, true);
                                        }
                                    }
                                }
                                break;

                            case enTerrainToolType.Smooth:
                                {
                                    foreach (var i in terrainModifyData.Keys)
                                    {
                                        foreach (var j in terrainModifyData[i].Keys)
                                        {
                                            short height = 0;
                                            HostTerrain.GetHeight(i, j, ref height, false);
                                            
                                            if (Increase)
                                                GetTerrainModifyDataDic(i, j, height, terrainModifyData[i][j]);
                                            else
                                                GetTerrainModifyDataDic(i, j, height, height);

                                            if (Increase)
                                                HostTerrain.SetHeight(i, j, (short)terrainModifyData[i][j], true);
                                        }
                                    }
                                }
                                break;

                            case enTerrainToolType.Layer:
                                {
                                    foreach (var i in terrainModifyData.Keys)
                                    {
                                        foreach (var j in terrainModifyData[i].Keys)
                                        {
                                            var modifyData = terrainModifyData[i][j];

                                            short height = 0;
                                            HostTerrain.GetHeight(i, j, ref height, false);

                                            // 记录修改的值
                                            Dictionary<uint, int> risedDataDic = null;
                                            if (!mTerrainRisedDataDic.TryGetValue(i, out risedDataDic))
                                            {
                                                risedDataDic = new Dictionary<uint, int>();
                                                mTerrainRisedDataDic[i] = risedDataDic;
                                            }
                                            int risedData = 0;
                                            risedDataDic.TryGetValue(j, out risedData);

                                            int strengthValue = (int)(TerrainBrushStrength * 10);

                                            if (Increase)
                                            {
                                                risedData += modifyData;
                                                if (risedData >= strengthValue)
                                                {
                                                    modifyData = strengthValue - (risedData - modifyData);
                                                    risedData = strengthValue;
                                                }
                                            }
                                            else
                                            {
                                                modifyData = -modifyData;
                                                risedData += modifyData;
                                                if (risedData <= -strengthValue)
                                                {
                                                    modifyData = -strengthValue - (risedData - modifyData);
                                                    risedData = -strengthValue;
                                                }
                                            }

                                            GetTerrainModifyDataDic(i, j, height, modifyData);

                                            if (Increase)
                                            {
                                                HostTerrain.RiseHeight(i, j, (short)modifyData, true);
                                                risedDataDic[j] = risedData;
                                            }
                                            else
                                            {
                                                HostTerrain.RiseHeight(i, j, (short)modifyData, true);
                                                risedDataDic[j] = risedData;
                                            }
                                        }
                                    }
                                }
                                break;

                            case enTerrainToolType.AddPatch:
                                {
                                    //foreach (var i in terrainModifyData.Keys)
                                    //{
                                    //    foreach (var j in terrainModifyData[i].Keys)
                                    //    {
                                    //        HostTerrain.AddPatch(i, j);

                                    //        GetTerrainModifyDataDic(i, j, 0, 0);
                                    //    }
                                    //}
                                }
                                break;

                            case enTerrainToolType.DelPatch:
                                {
                                    //foreach (var i in terrainModifyData.Keys)
                                    //{
                                    //    foreach (var j in terrainModifyData[i].Keys)
                                    //    {
                                    //        HostTerrain.DelPatch(i, j);

                                    //        GetTerrainModifyDataDic(i, j, 0, 0);
                                    //    }
                                    //}
                                }
                                break;
                            case enTerrainToolType.ObliqueSlip:
                                {
                                    foreach (var i in terrainModifyData.Keys)
                                    {
                                        foreach (var j in terrainModifyData[i].Keys)
                                        {
                                            var modifyData = terrainModifyData[i][j];
                                            short height = 0;
                                            HostTerrain.GetHeight(i, j, ref height, false);
                                            // 记录修改的值
                                            Dictionary<uint, int> risedDataDic = null;
                                            if (!mTerrainRisedDataDic.TryGetValue(i, out risedDataDic))
                                            {
                                                risedDataDic = new Dictionary<uint, int>();
                                                mTerrainRisedDataDic[i] = risedDataDic;
                                            }
                                            int risedData = 0;
                                            risedDataDic.TryGetValue(j, out risedData);
                                            if (risedData != 0)
                                                modifyData = risedData;
                                            else
                                                risedData = modifyData;
                                            risedDataDic[j] = risedData;

                                            GetTerrainModifyDataDic(i, j, height, modifyData);

                                            HostTerrain.SetHeight(i, j, (short)modifyData, true);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;

                case LayerItem.enLayerType.Material:
                    {
                        if (TerrainToolType == enTerrainToolType.PaintBrush)
                        {
                            if (SelectedLayerItem.MaterialId != Guid.Empty)
                            {
                                var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetMaterialDefaultTechnique(SelectedLayerItem.MaterialId);
                                if (mtl == null)
                                    break;

                                var grass = SelectedLayerItem.Grass;

                                foreach (var i in terrainModifyData.Keys)
                                {
                                    foreach (var j in terrainModifyData[i].Keys)
                                    {
                                        var materialId = HostTerrain.PickMaterial(i, j);

                                        if (Increase)
                                            HostTerrain.PaintLayerData(mtl, grass, terrainModifyData[i][j], i, j);
                                        else
                                            HostTerrain.PaintLayerData(mtl, grass, -terrainModifyData[i][j], i, j);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            SetActorHeight(terrainModifyData, bCanSetRoleActorHeight);
        }

        void GetBrushSlopeTerrainPointList(SlimDX.Vector3 brushLocBegin, SlimDX.Vector3 brushLocEnd, float brushRadius, float brushAngle, SlimDX.Plane plane, Dictionary<uint, Dictionary<uint, int>> terrainModifyData)
        {
            UInt32 minU = 0, minV = 0, maxU = 0, maxV = 0;
            if (brushLocBegin.X <= brushLocEnd.X)
            {
                minU = HostTerrain.GetUWithX(brushLocBegin.X - brushRadius);
                maxU = HostTerrain.GetUWithX(brushLocEnd.X + brushRadius);
            }
            else
            {
                maxU = HostTerrain.GetUWithX(brushLocBegin.X + brushRadius);
                minU = HostTerrain.GetUWithX(brushLocEnd.X - brushRadius);
            }
            if (brushLocBegin.Z <= brushLocEnd.Z)
            {
                minV = HostTerrain.GetVWithZ(brushLocBegin.Z - brushRadius);
                maxV = HostTerrain.GetVWithZ(brushLocEnd.Z + brushRadius);
            }
            else
            {
                maxV = HostTerrain.GetVWithZ(brushLocBegin.Z + brushRadius);
                minV = HostTerrain.GetVWithZ(brushLocEnd.Z - brushRadius);
            }

            byte range = 1;
            var brushData = GetBrushData(maxU - minU + 1, maxV - minV + 1, out range, brushAngle);
            if (brushData != null)
            {
                for (var i = minU; i <= maxU; i++)
                {
                    for (var j = minV; j <= maxV; j++)
                    {
                        uint idx = (j - minV) * (maxU - minU + 1) + (i - minU);
                        if (idx >= brushData.Length)
                            idx = (uint)(brushData.Length - 1);
                        var pixData = brushData[idx];
                        if (pixData == 0)
                            continue;

                        var height = -(plane.Normal.X * (float)i + plane.Normal.Z * (float)j + plane.D) / plane.Normal.Y;

                        Dictionary<uint, int> dicModifyDatas = null;
                        if (!terrainModifyData.TryGetValue(i, out dicModifyDatas))
                        {
                            dicModifyDatas = new Dictionary<uint, int>();
                            terrainModifyData[i] = dicModifyDatas;
                        }
                        dicModifyDatas[j] = (int)height;
                    }
                }
            }
        }

        void GetBrushSlopeTerrainPointList(SlimDX.Vector3 brushLocBegin, SlimDX.Vector3 brushLocEnd, float brushRadius, float brushAngle, Dictionary<uint, Dictionary<uint, int>> terrainModifyData)
        {
            var dirDistance = System.Math.Sqrt(System.Math.Pow(System.Math.Abs(brushLocBegin.X - brushLocEnd.X), 2) +
                System.Math.Pow(System.Math.Abs(brushLocBegin.Z - brushLocEnd.Z), 2));

            bool isLeftToRight = true;
            bool isTopToDown = true;
            UInt32 minU = 0, minV = 0, maxU = 0, maxV = 0;
            if (brushLocBegin.X <= brushLocEnd.X)
            {
                minU = HostTerrain.GetUWithX(brushLocBegin.X - brushRadius);
                maxU = HostTerrain.GetUWithX(brushLocEnd.X + brushRadius);
            }
            else
            {
                isLeftToRight = false;
                maxU = HostTerrain.GetUWithX(brushLocBegin.X + brushRadius);
                minU = HostTerrain.GetUWithX(brushLocEnd.X - brushRadius);
            }
            if (brushLocBegin.Z <= brushLocEnd.Z)
            {
                minV = HostTerrain.GetVWithZ(brushLocBegin.Z - brushRadius);
                maxV = HostTerrain.GetVWithZ(brushLocEnd.Z + brushRadius);
            }
            else
            {
                isTopToDown = false;
                maxV = HostTerrain.GetVWithZ(brushLocBegin.Z + brushRadius);
                minV = HostTerrain.GetVWithZ(brushLocEnd.Z - brushRadius);
            }

            //terrainPanel.HostTerrain.GetGridX
            byte range = 1;
            var brushData = GetBrushData(maxU - minU + 1, maxV - minV + 1, out range, brushAngle);
            if (brushData != null)
            {
                float rate = 0f;
                byte data = 0;
                for (var i = minU; i <= maxU; i++)
                {
                    for (var j = minV; j <= maxV; j++)
                    {
                        uint idx = (j - minV) * (maxU - minU + 1) + (i - minU);
                        if (idx >= brushData.Length)
                            idx = (uint)(brushData.Length - 1);
                        var pixData = brushData[idx];
                        int modifyData = 0;
                        if (pixData != 0)
                        {
                            rate = 1f;
                            if (isLeftToRight)
                            {
                                var lastIdx = (j - minV) * (maxU - minU + 1) + (i - minU - 1);
                                if (lastIdx < brushData.Length)
                                    data = brushData[lastIdx];
                                if (data == 0)
                                    rate = 0.5f;
                                var nextIdx = (j - minV) * (maxU - minU + 1) + (i - minU + 1);
                                if (nextIdx < brushData.Length)
                                    data = brushData[nextIdx];
                                if (data == 0)
                                    rate = 0.5f;
                            }
                            else
                            {
                                var lastIdx = (j - minV - 1) * (maxU - minU + 1) + (i - minU);
                                if (lastIdx < brushData.Length)
                                    data = brushData[lastIdx];
                                if (data == 0)
                                    rate = 0.5f;
                                var nextIdx = (j - minV + 1) * (maxU - minU + 1) + (i - minU);
                                if (nextIdx < brushData.Length)
                                    data = brushData[nextIdx];
                                if (data == 0)
                                    rate = 0.5f;
                            }
                            modifyData = (int)(GetSlopeHeight(isLeftToRight, isTopToDown, i, j) * rate);
                        }

                        modifyData += (int)brushLocBegin.Y;
                        Dictionary<uint, int> dicModifyDatas = null;
                        if (!terrainModifyData.TryGetValue(i, out dicModifyDatas))
                        {
                            dicModifyDatas = new Dictionary<uint, int>();
                            terrainModifyData[i] = dicModifyDatas;
                        }

                        if (modifyData != 0)
                            dicModifyDatas[j] = modifyData;
                    }
                }
            }
        }

        double GetSlopeHeight(bool isLeftToRight, bool isTopToDown, uint idX, uint idZ)
        {
            double distance = 0;
            var gridX = (UInt32)HostTerrain.GetGridX();
            var gridXCount = (UInt32)HostTerrain.GetGridXCount();
            var gridZ = (UInt32)HostTerrain.GetGridZ();
            var gridZCount = (UInt32)HostTerrain.GetGridZCount();
            var resultX = gridXCount / gridX;
            var resultZ = gridZCount / gridZ;
            if (isLeftToRight)
            {
                if (!isTopToDown)
                {
                    distance = System.Math.Sqrt(System.Math.Pow((double)(resultX * (idX - mLeftDownPos.X)), 2d) +
                        System.Math.Pow((double)(resultZ * (mRightDownPos.Z - idZ)), 2d));
                }
                else
                {
                    distance = System.Math.Sqrt(System.Math.Pow((double)(resultX * (idX - mLeftTopPos.X)), 2d) +
                        System.Math.Pow((double)(resultZ * (idZ - mRightTopPos.Z)), 2d));
                }
            }
            else
            {
                if (!isTopToDown)
                {
                    distance = System.Math.Sqrt(System.Math.Pow((double)(resultX * (mRightDownPos.X - idX)), 2d) +
                        System.Math.Pow((double)(resultZ * (mLeftDownPos.Z - idZ)), 2d));
                }
                else
                {
                    distance = System.Math.Sqrt(System.Math.Pow((double)(resultX * (mRightTopPos.X - idX)), 2d) +
                        System.Math.Pow((double)(resultZ * (idZ - mLeftTopPos.Z)), 2d));
                }
            }
            return GetHeight(TerrainBrushStrength, distance);
        }

        double GetHeight(double BrushStrength, double distance)
        {
            //根据笔刷强度和最高90°角算出当前角度、弧度
            var angle = BrushStrength / 10d * 90d;
            var rotation = angle * Math.PI / 180d;
            //根据角度算出高度
            var height = System.Math.Tan(rotation) * distance * 10;

            return height;
        }

        SlimDX.Vector3 GetBrushBeginPoint(bool isLeftToRight, bool isTopToDown)
        {
            SlimDX.Vector3 pt = new SlimDX.Vector3();
            if (isTopToDown)
            {
                pt.X = mRightTopPos.X - (mRightTopPos.X - mLeftTopPos.X) / 2f;
                pt.Y = mLeftTopPos.Y;
                if (isLeftToRight)
                    pt.Z = mRightTopPos.Z - (mRightTopPos.Z - mLeftTopPos.Z) / 2f;
                else
                    pt.Z = mLeftTopPos.Z - (mLeftTopPos.Z - mRightTopPos.Z) / 2f;
            }
            else
            {
                pt.X = mRightDownPos.X - (mRightDownPos.X - mLeftDownPos.X) / 2f;
                pt.Y = mLeftDownPos.Y;
                if (isLeftToRight)
                    pt.Z = mRightDownPos.Z - (mRightDownPos.Z - mLeftDownPos.Z) / 2f;
                else
                    pt.Z = mLeftDownPos.Z - (mLeftDownPos.Z - mRightDownPos.Z) / 2f;
            }
            return pt;
        }

        SlimDX.Vector3 GetBrushEndPoint(bool isLeftToRight, bool isTopToDown)
        {
            SlimDX.Vector3 pt = new SlimDX.Vector3();
            if (!isTopToDown)
            {
                pt.X = mRightTopPos.X - (mRightTopPos.X - mLeftTopPos.X) / 2f;
                pt.Y = mLeftTopPos.Y;
                if (!isLeftToRight)
                    pt.Z = mRightTopPos.Z - (mRightTopPos.Z - mLeftTopPos.Z) / 2f;
                else
                    pt.Z = mLeftTopPos.Z - (mLeftTopPos.Z - mRightTopPos.Z) / 2f;
            }
            else
            {
                pt.X = mRightDownPos.X - (mRightDownPos.X - mLeftDownPos.X) / 2f;
                pt.Y = mLeftDownPos.Y;
                if (!isLeftToRight)
                    pt.Z = mRightDownPos.Z - (mRightDownPos.Z - mLeftDownPos.Z) / 2f;
                else
                    pt.Z = mLeftDownPos.Z - (mLeftDownPos.Z - mRightDownPos.Z) / 2f;
            }
            return pt;
        }

        SlimDX.Vector3 SetTerrainPoint(SlimDX.Vector3 point)
        {
            SlimDX.Vector3 pt = new SlimDX.Vector3();
            var ptX = (int)point.X;
            if (point.X - ptX > 0.5f)
                pt.X = ptX + 1;
            else
                pt.X = ptX;
            var ptZ = (int)point.Z;
            if (point.Z - ptZ > 0.5f)
                pt.Z = ptZ + 1;
            else
                pt.Z = ptZ;
            pt.Y = point.Y;
            return pt;
        }

        public void DrawSlopeTerrainWithMouse()
        {
            if (!mTerrainBrushActor.Visible || !m_CanDrawTerrain || HostTerrain == null)
                return;
            
            if (TerrainToolType != enTerrainToolType.Slope)
                return;

            if (SelectedLayerItem == null)
                return;

            Dictionary<uint, Dictionary<uint, int>> terrainModifyDataDic = new Dictionary<uint, Dictionary<uint, int>>();

            var u1 = HostTerrain.GetUWithX(mTerrainIntersectPt.X);
            var v1 = HostTerrain.GetVWithZ(mTerrainIntersectPt.Z);
            short height1 = 0;
            HostTerrain.GetHeight(u1, v1, ref height1, true);
            SlimDX.Vector3 point1 = new SlimDX.Vector3(u1, height1, v1);
            var u2 = HostTerrain.GetUWithX(mPreMouseIntersectWithTerrainPoint.X);
            var v2 = HostTerrain.GetVWithZ(mPreMouseIntersectWithTerrainPoint.Z);
            short height2 = 0;
            HostTerrain.GetHeight(u2, v2, ref height2, true);
            SlimDX.Vector3 point2 = new SlimDX.Vector3(u2, height2, v2);

            SlimDX.Vector3 point3 = new SlimDX.Vector3((u1 + u2) / 2, (height1 + height2) / 2, (v1 + v2) / 2);

            var plane = new SlimDX.Plane(point1, point2, mLeftTopPos);

            var dirPt = mTerrainIntersectPt - mPreMouseIntersectWithTerrainPoint;
            if (dirPt != SlimDX.Vector3.Zero)
            {
                dirPt.Normalize();
                float dirAngle = 0;
                var commonDir = new SlimDX.Vector3(1, 0, 0);
                var dirCross = SlimDX.Vector3.Cross(commonDir, dirPt);
                var acosDir = (float)System.Math.Acos(SlimDX.Vector3.Dot(commonDir, dirPt));
                if (dirCross.Y > 0)
                    dirAngle = (float)(System.Math.PI / 2 - acosDir);
                else
                    dirAngle = (float)(System.Math.PI / 2 + acosDir);


                // 取得笔刷覆盖范围
                GetBrushSlopeTerrainPointList(mPreMouseIntersectWithTerrainPoint, mTerrainIntersectPt, (float)TerrainBrushRadius, dirAngle, plane, terrainModifyDataDic);

                DrawSlopeTerrain(terrainModifyDataDic);
            }

            //mTerrainIntersectPt.Y = 0;
            //mPreMouseIntersectWithTerrainPoint.Y = 0;

            //bool isLeftToRight = false;
            //bool isTopToDown = false;
            //if (mPreMouseIntersectWithTerrainPoint.Z < mTerrainIntersectPt.Z)
            //{
            //    isTopToDown = true;
            //}
            //if (mPreMouseIntersectWithTerrainPoint.X < mTerrainIntersectPt.X)
            //{
            //    isLeftToRight = false;
            //}
            //mLeftTopPos = SetTerrainPoint(mLeftTopPos);
            //mRightTopPos = SetTerrainPoint(mRightTopPos);
            //mLeftDownPos = SetTerrainPoint(mLeftDownPos);
            //mRightDownPos = SetTerrainPoint(mRightDownPos);
            //mPreMouseIntersectWithTerrainPoint = GetBrushBeginPoint(isLeftToRight, isTopToDown);
            //mTerrainIntersectPt = GetBrushEndPoint(isLeftToRight, isTopToDown);

            //var dirPt = mTerrainIntersectPt - mPreMouseIntersectWithTerrainPoint;
            //if (dirPt != SlimDX.Vector3.Zero)
            //{
            //    dirPt.Normalize();
            //    float dirAngle = 0;
            //    var commonDir = new SlimDX.Vector3(1, 0, 0);
            //    var dirCross = SlimDX.Vector3.Cross(commonDir, dirPt);
            //    var acosDir = (float)System.Math.Acos(SlimDX.Vector3.Dot(commonDir, dirPt));
            //    if (dirCross.Y > 0)
            //        dirAngle = (float)(System.Math.PI / 2 - acosDir);
            //    else
            //        dirAngle = (float)(System.Math.PI / 2 + acosDir);


            //    // 取得笔刷覆盖范围
            //    GetBrushSlopeTerrainPointList(mPreMouseIntersectWithTerrainPoint, mTerrainIntersectPt, (float)TerrainBrushRadius, dirAngle, terrainModifyDataDic);

            //    DrawSlopeTerrain(terrainModifyDataDic);
            //}

        }

        public void DrawSlopeTerrain(Dictionary<uint, Dictionary<uint, int>> terrainModifyData)
        {
            if (SelectedLayerItem == null || SelectedLayerItem.LayerType != LayerItem.enLayerType.HightMap)
                return;

            foreach (var i in terrainModifyData.Keys)
            {
                foreach (var j in terrainModifyData[i].Keys)
                {
                    short height = 0;
                    HostTerrain.GetHeight(i, j, ref height, false);

                    GetTerrainModifyDataDic(i, j, height, terrainModifyData[i][j]);

                    HostTerrain.SetHeight(i, j, (short)terrainModifyData[i][j], true);
                }
            }
            SetActorHeight(terrainModifyData, bCanSetRoleActorHeight);
            
            //mTerraindModifyDataDic = terrainModifyData;
        }

        public void GetObliqueSlipPlane()
        {
            var terrain = HostTerrain;
            var idU = terrain.GetUWithX(mPreMouseIntersectWithTerrainPoint.X);
            var idV = terrain.GetVWithZ(mPreMouseIntersectWithTerrainPoint.Z);
            var gridX = terrain.GetGridX() / terrain.GetGridXCount();
            var gridZ = terrain.GetGridZ() / terrain.GetGridZCount();
            var x = idU * (uint)gridX;
            var z = (idV + 1) * (uint)gridZ;
            short y = 0;
            terrain.GetHeight(x, z, ref y, false);
            var leftUpGridPos = new SlimDX.Vector3(x, y, z);
            x = idU * (uint)gridX;
            z = idV * (uint)gridZ;
            terrain.GetHeight(x, z, ref y, false);
            var leftDownGridPos = new SlimDX.Vector3(x, y, z);
            x = (idU + 1) * (uint)gridX;
            z = (idV + 1) * (uint)gridZ;
            terrain.GetHeight(x, z, ref y, false);
            var rightUpGridPos = new SlimDX.Vector3(x, y, z);
            x = (idU + 1) * (uint)gridX;
            z = idV * (uint)gridZ;
            terrain.GetHeight(x, z, ref y, false);
            var rightDownGridPos = new SlimDX.Vector3(x, y, z);
            var HalfArea = gridX * gridZ / 2f;
            var obliqueArea = (mPreMouseIntersectWithTerrainPoint.X - (float)idU) * (mPreMouseIntersectWithTerrainPoint.Z - (float)idV) +
                (gridX - mPreMouseIntersectWithTerrainPoint.X + (float)idU) * (mPreMouseIntersectWithTerrainPoint.X - (float)idU) / 2f +
                (gridZ - mPreMouseIntersectWithTerrainPoint.Z + (float)idV) * (mPreMouseIntersectWithTerrainPoint.Z - (float)idV) / 2f;
            
            if (obliqueArea < HalfArea)
            {
                mObliqueSlipPlane = new SlimDX.Plane(leftUpGridPos, leftDownGridPos, rightDownGridPos);
            }
            else
            {
                mObliqueSlipPlane = new SlimDX.Plane(leftUpGridPos, rightUpGridPos, rightDownGridPos);
            }

        }

        #endregion

    }
}
