using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

namespace UIEditor
{
    public class UIEditorWinRoot : UISystem.WinRoot
    {
        public string FPSString = "";
        //System.Drawing.Rectangle mDestRect = new System.Drawing.Rectangle();
        protected override void AfterStateDraw(UISystem.UIRenderPipe pipe, int zOrder)
        {
            //UISystem.IRender.GetInstance().SetClipRect(this.AbsRect);
            //UISystem.IRender pRender = UISystem.IRender.GetInstance();
            ////if (FrameSet.Inventory.TransforBar.Instance.Item != null)
            ////{
            ////    UISystem.UVFrame frame = FrameSet.Inventory.TransforBar.Instance.Item.Data.Template.Icon.GetUVFrame(CCore.Engine.Instance.GetFrameSecondTimeFloat());
            ////    pRender.DrawImage(this.Width, this.Height,
            ////        FrameSet.Inventory.TransforBar.Instance.Item.Data.Template.Icon.TextureObject,
            ////        ref mDestRect,
            ////        ref frame.mUVRect,
            ////        FrameSet.Inventory.TransforBar.Instance.Item.Data.Template.Icon.MaterialObject);
            ////}
            ////UISystem.IRender.GetInstance().SetClipRect(this.AbsRect);
            //pRender.DrawString(20, 20, FPSString, 20, System.Drawing.Color.FromArgb(255, 255, 0, 0));
        }

        //public override UISystem.MSG_PROC ProcMessage(ref UISystem.WinMSG msg)
        //{
        //    //if (msg.message == (UInt32)SysMessage.VWM_MOUSEMOVE)
        //    //{
        //    //    mDestRect.X = msg.pt.X;
        //    //    mDestRect.Y = msg.pt.Y;
        //    //    mDestRect.Width = 32;
        //    //    mDestRect.Height = 32;
        //    //}
        //    //return base.ProcMessage(ref msg);
        //    return UISystem.MSG_PROC.Finished;
        //}
    }

	/// <summary>
	/// DrawPanel.xaml 的交互逻辑
	/// </summary>
	public partial class DrawPanel : UserControl
	{
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public delegate void Delegate_OnDeleteControls(List<UISystem.WinBase> controls);
        public Delegate_OnDeleteControls OnDeleteControls;

        private CCore.Graphics.REnviroment m_REnviroment;
        private CCore.World.WorldRenderParam m_Renderparam;
        CCore.World.World m_World;
        //ITracePosCameraController m_CameraController;

        /////////////////////////////////
        //CCore.Camera.TracePosCameraController m_CameraController;

        //CCore.Mesh.Mesh m_Mesh = null;
        //CCore.World.Actor m_MeshActor = null;
        //////////////////////////////////////////

        public delegate void Delegate_OnRootFormChanged(UISystem.WinBase form);
        public event Delegate_OnRootFormChanged OnRootFormChanged;

        UIEditorWinRoot mWinRoot = new UIEditorWinRoot();
        public UIEditorWinRoot WinRoot
        {
            get { return mWinRoot; }
        }
        UISystem.WinBase mWinMousePointAtControl = null;
        UISystem.WinBase mWinCreatedParentControl = null;
        //UISystem.WinBase mWinUIRoot = new UISystem.WinBase();
        UISystem.WinBase mWinRootForm = new UISystem.WinForm();
        public UISystem.WinBase WinRootForm
        {
            get { return mWinRootForm; }
            set
            {
                if (mWinRootForm != null)
                {
                    mWinRootForm.RemoveEventWithPropertyChangedEventWithChild(OnWinControlPropertyChanged);
                }

                mWinRoot.CleanPopupedWins();

                mWinRootForm = value;

                if (mWinRootForm != null)
                {
                    Clear();
                    mWinRootForm.Parent = mWinRoot;// mWinUIRoot;
                    //mAllWinControls.Add(mWinRootForm);
                    mWinRootForm.AddEventWithPropertyChangedEventWithChild(OnWinControlPropertyChanged);

                    mWinRootForm.UpdateLayout();
                }

                //if (UIControlsContainerHolder != null)
                //    UIControlsContainerHolder.SetRootForm(mWinRootForm);

                if (OnRootFormChanged != null)
                    OnRootFormChanged(mWinRootForm);
            }
        }

        private void OnWinRootFormUpdateLayout()
        {
            if (!mIsMouseDragging)
                UpdateUIControlsSelection(SelectedWinControls, false);

            //var rect = GetSelectionControlsBoundRect(SelectedWinControls);
            //SetControlContainerRect(rect.X, rect.Y, rect.Width, rect.Height);
            //mControlContainerRectStore = mControlContainerRect;
            //UpdateUIControlRectRestores(SelectedWinControls);
        }

        // 窗口大小
        CSUtility.Support.Rectangle mWindowsRect = new CSUtility.Support.Rectangle(0, 0, 1024, 768);
        //// 场景里所有的UI控件
        //List<UISystem.WinBase> mAllWinControls = new List<UISystem.WinBase>();
        // 选中的UI控件
        //List<UISystem.WinBase> mSelectedWinControls = new List<UISystem.WinBase>();
        //public List<UISystem.WinBase> SelectedWinControls
        //{
        //    get { return mSelectedWinControls; }
        //    set
        //    {
        //        if (mSelectedWinControls == value)
        //            return;

        //        mSelectedWinControls = value;
        //    }
        //}

        public ObservableCollection<UIEditor.WinBase> SelectedWinControls
        {
            get { return (ObservableCollection<UIEditor.WinBase>)GetValue(SelectedWinControlsProperty); }
            set { SetValue(SelectedWinControlsProperty, value); }
        }
        public static readonly DependencyProperty SelectedWinControlsProperty = DependencyProperty.Register("SelectedWinControls", typeof(ObservableCollection<UIEditor.WinBase>), typeof(DrawPanel), new FrameworkPropertyMetadata(new ObservableCollection<UIEditor.WinBase>(), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSelectedWinControlsChanged)));

        public static void OnSelectedWinControlsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DrawPanel control = d as DrawPanel;

            control.SelectedWinControls.CollectionChanged += control.SelectedWinControls_CollectionChanged;

            control.UpdateUIControlsSelection((ObservableCollection<UIEditor.WinBase>)e.NewValue);
        }

        //CSUtility.Support.Rectangle mClipRect = new CSUtility.Support.Rectangle();

        Panel.ControlsTreeView.UIControlsContainer mUIControlsContainerHolder;
        public Panel.ControlsTreeView.UIControlsContainer UIControlsContainerHolder
        {
            get { return mUIControlsContainerHolder; }
            set
            {
                mUIControlsContainerHolder = value;

                if (mUIControlsContainerHolder != null)
                {
                    mUIControlsContainerHolder.SetRootForm(mWinRootForm);
                    //BindingOperations.SetBinding(this, SelectedWinControlsProperty,
                    //                        new Binding("SelectedItems")
                    //                        {
                    //                            Source = mUIControlsContainerHolder,
                    //                            Mode = BindingMode.TwoWay,
                    //                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    //                        });

                    //ObservableCollection<WinBase> sels = new ObservableCollection<WinBase>();
                    //foreach (var ctrl in SelectedWinControls)
                    //{
                    //    WinBase win = new WinBase(ctrl);
                    //    sels.Add(win);
                    //}
                    //mUIControlsContainerHolder.SelectedItems = sels;// SelectedWinControls;

                    mUIControlsContainerHolder.SelectedItems = SelectedWinControls;
                }
            }
        }

        public PropertyAndBindPanel ProAndBindPanel;

        //CCore.Mesh.Mesh m_Mesh;
        //CCore.World.Actor m_MeshActor;
        bool mInitialized = false;

        protected bool mSnapEnable = true;
        public bool SnapEnable
        {
            get { return mSnapEnable; }
            set { mSnapEnable = value; }
        }
        int mSnapDistance = 10;
        //int mSnapPosX = 0;
        //int mSnapPosY = 0;
        //bool mSnapXShow = false;
        //bool mSnapYShow = false;

        bool mRecordMode = false;
        public bool RecordMode
        {
            get { return mRecordMode; }
            set
            {
                mRecordMode = value;

                if (mRecordMode)
                    Border_Record.BorderBrush = Brushes.Red;
                else
                    Border_Record.BorderBrush = null;
            }
        }

        //public delegate void Delegate_Tick();
		public DrawPanel()
		{
			this.InitializeComponent();
            
            SelectedWinControls.CollectionChanged += SelectedWinControls_CollectionChanged;
            
            //mWinUIRoot.OnUpdateLayout = new UISystem.WinBase.Delegate_OnUpdateLayout(OnWinRootFormUpdateLayout);
            mWinRoot.OnUpdateLayout = new UISystem.WinBase.Delegate_OnUpdateLayout(OnWinRootFormUpdateLayout);

            var storyboard = TryFindResource("Storyboard_PreSelect_Show") as Storyboard;
            storyboard?.Begin();
            storyboard = TryFindResource("Storyboard_PreSelect_Hide") as Storyboard;
            storyboard?.Begin();
        }

        public void CreateNew()
        {
            //Clear();

            WinRootForm = new UISystem.WinForm();
            //mUIControlsContainerHolder.SetRootForm(mWinRootForm);
            mWinRootForm.HorizontalAlignment = UISystem.UI.HorizontalAlignment.Left;
            mWinRootForm.VerticalAlignment = UISystem.UI.VerticalAlignment.Top;
            mWinRootForm.Margin = new CSUtility.Support.Thickness(0, 0, 0, 0);
            //mWinRootForm.Left = 0;
            //mWinRootForm.Top = 0;
            mWinRootForm.Width = mWindowsRect.Width;
            mWinRootForm.Height = mWindowsRect.Height;
            //mWinRootForm.Parent = mWinUIRoot;
            //mWinRootForm.Visible = true;
            //mWinRootForm.DragEnable = false;
            //mWinRootForm.DockMode = System.Windows.Forms.DockStyle.None;
            //mWinRootForm.ForeColor = System.Drawing.Color.White;
            //mWinRootForm.BackColor = System.Drawing.Color.LightGray;
            //mWinRootForm.FixSizeByUVAnim = false;
            //mWinRootForm.RState.UVAnim = null;// UISystem.UVAnimMgr.Instance.Find(Guid.Parse("8a267cd1-f4c3-473e-8de9-24a8986057bf"), true);
            //mAllWinControls.Add(mWinRootForm);
        }

        public void CreateNewTemplate()
        {
            WinRootForm = new UISystem.Template.ControlTemplate();
            mWinRootForm.HorizontalAlignment = UISystem.UI.HorizontalAlignment.Left;
            mWinRootForm.VerticalAlignment = UISystem.UI.VerticalAlignment.Top;
            mWinRootForm.Margin = new CSUtility.Support.Thickness(0, 0, 0, 0);
            mWinRootForm.Width = mWindowsRect.Width;
            mWinRootForm.Height = mWindowsRect.Height;
        }

        public delegate void Delegete_OnSelectedWinControlsCollectionChanged(ObservableCollection<UIEditor.WinBase> collection);
        public event Delegete_OnSelectedWinControlsCollectionChanged OnSelectedWinControlsCollectionChanged;
        private void SelectedWinControls_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //ObservableCollection<UISystem.WinBase> collection = new ObservableCollection<UISystem.WinBase>();

            //if (e.NewItems != null)
            //{
            //    foreach (var item in e.NewItems)
            //    {
            //        collection.Add((UISystem.WinBase)item);
            //    }
            //}
            ObservableCollection<UISystem.WinBase> selWins = new ObservableCollection<UISystem.WinBase>();
            foreach (var ctrl in SelectedWinControls)
            {
                selWins.Add(ctrl.UIWin);
            }
            Program.mSelectionWinControlsCollection = selWins;// SelectedWinControls;

            UpdateUIControlsSelection(SelectedWinControls);// false);

            if (OnSelectedWinControlsCollectionChanged != null)
                OnSelectedWinControlsCollectionChanged(SelectedWinControls);
        }

        public void Clear()
        {
            //mAllWinControls.Clear();
            //mbShowPreSelectionRect = false;
            SelectedWinControls.Clear();
            mControlContainerVisible = Visibility.Collapsed;
            mSelectionControlRectRestores.Clear();

            //mWinRootForm.ClearChildWindows();
            //mWinUIRoot.ClearChildWindows();
            //mWinUIRoot.Margin = new CSUtility.Support.Thickness(0);
            //mWinUIRoot.CurFinalRect = System.Windows.Rect.Empty;
            //mWinUIRoot.DesiredSize = Size.Empty;
            //mWinRootForm.Parent = null;
            mWinRoot.ClearChildWindows();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!mInitialized)
            {
                m_Renderparam = new CCore.World.WorldRenderParam();
                this.InitD3DEnvironment();
                //////InitControlContainer();

                //if (mOperationWin == null)
                //{
                //    mOperationWin = new OperationWindow();
                //    mOperationWin.Show();

                //    //var p = VisualTreeHelper.GetParent(this);
                //    //while(p != null)
                //    //{
                //    //    if (p is Window)
                //    //    {
                //    //        mOperationWin.Owner = (Window)p;
                //    //        break;
                //    //    }

                //    //    p = VisualTreeHelper.GetParent(p);
                //    //}
                //}
                //KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.None);
            }
        }
		
		private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Keyboard.Focus(this);
        }

        public void SetScreenSize(EditorCommon.Assist.ScreenSizeInfo info)
        {
            if (info == null)
                return;

            mWindowsRect.Width = (int)info.ScreenWidth;
            mWindowsRect.Height = (int)info.ScreenHeight;

            mWinRoot.Width = mWindowsRect.Width;
            mWinRoot.Height = mWindowsRect.Height;
            //mWinUIRoot.Width = mWindowsRect.Width;
            //mWinUIRoot.Height = mWindowsRect.Height;

            ViewBoxMain.Width = mWindowsRect.Width;
            //WinDrawPanel.Width = mWindowsRect.Width;
            //WinDrawPanel.Height = mWindowsRect.Height;
            WinDrawPanel.Width = mWindowsRect.Width;
            WinDrawPanel.Height = mWindowsRect.Height;

            MainDrawCanvas.Width = mWindowsRect.Width;
            MainDrawCanvas.Height = mWindowsRect.Height;

            CreateRTBitmap(mWindowsRect.Width, mWindowsRect.Height);
        }

        private System.Windows.Forms.Panel WinDrawPanel = new System.Windows.Forms.Panel();
        private void InitD3DEnvironment()
        {
            var _reInit = new CCore.Graphics.REnviromentInit();
            _reInit.ViewInit = new CCore.Graphics.ViewInit();
            _reInit.ViewInit.ViewWnd.SetControl(WinDrawPanel);// mRenderPanel;// WinDrawPanel;
            var view = new CCore.Graphics.View();
            view.Initialize(_reInit.ViewInit);
            m_REnviroment = new CCore.Graphics.REnviroment();
            if (false == m_REnviroment.Initialize(_reInit, view))
                return;

            m_REnviroment.SetClearColorMRT(CSUtility.Support.Color.FromArgb(0, CSUtility.Support.Color.Black));
            //m_REnviroment.SetClearColorMRT(System.Drawing.Color.Black);
            
            m_REnviroment.AfterRender2View += this.AfterRender2View;

            m_Renderparam.Enviroment = m_REnviroment;

            var worldInit = new CCore.World.WorldInit();
            m_World = new CCore.World.World(Guid.NewGuid());
            m_World.Initialize(worldInit);
            //m_World.Initialize(new MidLayer.ISingleSceneGraph(), new MidLayer.ICollision(m_World), null);

            //mWinRoot.Width = mWindowsRect.Width;
            //mWinRoot.Height = mWindowsRect.Height;

            //mWinUIRoot.Left = 0;
            //mWinUIRoot.Top = 0;
            //mWinUIRoot.HorizontalAlignment = UISystem.UI.HorizontalAlignment.Left;
            //mWinUIRoot.VerticalAlignment = UISystem.UI.VerticalAlignment.Top;
            //mWinUIRoot.Margin = new CSUtility.Support.Thickness(0);
            //mWinUIRoot.Width = mWindowsRect.Width;
            //mWinUIRoot.Height = mWindowsRect.Height;
            //mWinUIRoot.Parent = mWinRoot;
            //mWinUIRoot.Visible = true;
            //mWinUIRoot.Visibility = System.Windows.Visibility.Visible;
            //mWinUIRoot.DragEnable = false;
            //mWinUIRoot.DockMode = System.Windows.Forms.DockStyle.None;
            //mWinUIRoot.ForeColor = System.Drawing.Color.White;
            //mWinUIRoot.BackColor = System.Drawing.Color.LightGray;

            // 界面
            //UISystem.WinForm form = new UISystem.WinForm();  // .WinBase win = UISystem.WinBase.CreateFromXml("UI/EquipBar.xml");
            CreateNew();

            //UI.UISystemManager.Instance.ResizeView(m_REnviroment.View);

            //////////////////////////////////////////////////////////////////////////
            // 默认box
            /*m_Mesh = new CCore.Mesh.Mesh();
            SetMesh("Mesh/Assist/sphere.vms");
            //IMeshInit mshInit = new IMeshInit();
            //mshInit.MeshName = "Mesh/Assist/cylinder.vms";//"@Box";
            //m_Mesh.Initialize(mshInit);
            //IMaterial mtl = IEngine.Instance.Client.Graphics.MaterialMgr.LoadMaterial(new IMaterialParameter("Tex2DMaterial.mtl", "Tech0"));
            //for (int i = 0; i < m_Mesh.MaxMaterial; ++i)
            //{
            //    m_Mesh.SetMeterial(i, mtl);
            //}

            m_Mesh.CanHitProxy = false;

            MidLayer.ICommActorInit caInit = new MidLayer.ICommActorInit();
            m_MeshActor = new CCore.World.Actor();
            m_MeshActor.Initialize(caInit);
            m_MeshActor.Visual = m_Mesh;
            m_MeshActor.mPlacement = new CSUtility.Component.StandardPlacement(m_MeshActor);
            m_World.AddCommActor(m_MeshActor);*/

            //// CameraController
            //m_CameraController = new CCore.Camera.TracePosCameraController();
            ////m_CameraController = new ICameraController();
            //m_CameraController.Camera = m_REnviroment.Camera;
            //m_CameraController.Enable = true;

            ////SlimDX.Vector3 pos = new SlimDX.Vector3(0, 50, -50);
            ////SlimDX.Vector3 lookAt = new SlimDX.Vector3(0, 0, 0);
            ////SlimDX.Vector3 up = new SlimDX.Vector3(0, 1, 0);
            ////m_CameraController.SetPosLookAtUp(ref pos, ref lookAt, ref up);

            //SlimDX.Vector3 dir = new SlimDX.Vector3(1, 0, 0);
            //dir.Normalize();
            //(m_CameraController).Initialize(ref dir, 20);
            /////////////////////////////////////////////////////////////////////////*/

            mInitialized = true;
        }
        /////////////////////////////////////////////////////////////////////////        
        //public CCore.Mesh.Mesh SetMesh(string strFileName)
        //{
        //    if (string.IsNullOrEmpty(strFileName))
        //    {
        //        m_Mesh.Cleanup();
        //        return null;
        //    }

        //    var mshInit = new CCore.Mesh.MeshInit();
        //    CCore.Mesh.MeshInitPart mshInitPart = new CCore.Mesh.MeshInitPart();
        //    mshInitPart.MeshName = strFileName;
        //    mshInit.MeshInitParts.Add(mshInitPart);
        //    m_Mesh.Cleanup();
        //    m_Mesh.Initialize(mshInit, null);
        //    var mtl = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetDefaultMaterial();//IEngine.Instance.Client.Graphics.MaterialMgr.LoadMaterial(new IMaterialParameter("Tex2DMaterial.mtl", "Tech0"));
        //    for (int i = 0; i < m_Mesh.GetMaxMaterial(0); ++i)
        //    {
        //        m_Mesh.SetMaterial(0, i, mtl);
        //    }

        //    if (m_CameraController != null)
        //    {
        //        var vObjCenter = (m_Mesh.vMax - m_Mesh.vMin) * 0.5f + m_Mesh.vMin;
        //        m_CameraController.SetTargetPos(ref vObjCenter);
        //    }

        //    return m_Mesh;
        //}
        /////////////////////////////////////////////////////////////////////////
        //long mlCurrTime = 0;



#region 鼠标操作
        //enControlRectType mSelectedControlRectType = enControlRectType.None;

        //System.Drawing.Point mMouseBtnDownPoint;
        CSUtility.Support.Point mRenderScaleCenter = CSUtility.Support.Point.Empty;
        float mRenderScaleX = 1;
        float mRenderScaleY = 1;
        //System.Drawing.Point mUIRootLocationRestore = new System.Drawing.Point();
        //bool mMouseMoved = false;
        //bool mbShowPreSelectionRect = false;
        bool mIsMouseDragging = false;

        //private CSUtility.Support.Point GetScreenPointToRenderPoint(System.Drawing.Point pt)
        //{
        //    var left = (int)(-(mRenderScaleCenter.X * (mRenderScaleX - 1)));
        //    var top = (int)(-(mRenderScaleCenter.Y * (mRenderScaleY - 1)));
        //    var width = (int)(WinDrawPanel.Width * mRenderScaleX);
        //    var height = (int)(WinDrawPanel.Height * mRenderScaleY);

        //    pt.X += (int)(left / mRenderScaleX);// -mWinUIRoot.Left;
        //    pt.Y += (int)(top / mRenderScaleY);// -mWinUIRoot.Top;
        //    var tagLeft = (int)(((float)pt.X) / (WinDrawPanel.Width / mRenderScaleX) * WinDrawPanel.Width);
        //    var tagTop = (int)(((float)pt.Y) / (WinDrawPanel.Height / mRenderScaleY) * WinDrawPanel.Height);
        //    return new CSUtility.Support.Point(tagLeft, tagTop);
        //}

        
        //private void ScaleDraw(float delta, CSUtility.Support.Point scaleCenter)
        //{
        //    //scaleCenter = new System.Drawing.Point(mWindowsRect.Width / 2, mWindowsRect.Height / 2);
        //    mRenderScaleCenter = new CSUtility.Support.Point(mWindowsRect.Width / 2, mWindowsRect.Height / 2);

        //    delta = delta * 0.01f;
        //    mRenderScaleX += delta;
        //    if (mRenderScaleX < 0.001f)
        //        mRenderScaleX = 0.001f;
        //    //if (mWindowsRect.Width * mRenderScaleX > this.Width)
        //    //{
        //    //    mRenderScaleX -= delta;
        //    //}
        //    //////if (mRenderScaleX > 1)
        //    //////    mRenderScaleX = 1;
        //    mRenderScaleY += delta;
        //    if (mRenderScaleY < 0.001f)
        //        mRenderScaleY = 0.001f;
        //    //if (mWindowsRect.Height * mRenderScaleY > this.Height)
        //    //{
        //    //    mRenderScaleY -= delta;
        //    //}
        //    //////if (mRenderScaleY > 1)
        //    //////    mRenderScaleY = 1;
        //}
                       

#endregion

#region 选择操作

        // 用于保存选择控件变换前的包围盒
        List<System.Drawing.Rectangle> mSelectionControlRectRestores = new List<System.Drawing.Rectangle>();

        // 相对于根Form的绝对位置
        private CSUtility.Support.Point GetAbsoluteLocation(UISystem.WinBase srcControl)
        {
            if (srcControl == null)
                return CSUtility.Support.Point.Empty;

            var absolutePos = new CSUtility.Support.Point(srcControl.Left, srcControl.Top);
            if (srcControl == mWinRoot)
                return CSUtility.Support.Point.Empty;

            var parent = srcControl.Parent as UISystem.WinBase;
            while (parent != null && parent != mWinRoot)
            {
                absolutePos.X += parent.Left;
                absolutePos.Y += parent.Top;

                parent = parent.Parent as UISystem.WinBase;
            }

            return absolutePos;
        }

        private System.Drawing.Rectangle GetSelectionControlsBoundRect(ObservableCollection<UIEditor.WinBase> selection)
        {
            System.Drawing.Rectangle retRect = System.Drawing.Rectangle.Empty;
            if (selection != null && selection.Count > 0)
            {
                int minLeft = int.MaxValue;//mWinRootForm.Right;
                int minTop = int.MaxValue;//mWinRootForm.Bottom;
                int maxRight = 0;
                int maxBottom = 0;

                foreach (var uiControl in selection)
                {
                    var deltaPt = GetAbsoluteLocation(uiControl.UIWin);
                    int leftDelta = deltaPt.X - uiControl.UIWin.Left;
                    int topDelta = deltaPt.Y - uiControl.UIWin.Top;
                    //var parent = uiControl.Parent;
                    //while (parent != null && parent != mWinUIRoot)
                    //{
                    //    leftDelta += parent.Left;
                    //    topDelta += parent.Top;

                    //    parent = parent.Parent;
                    //}

                    if (uiControl.UIWin.Left + leftDelta < minLeft)
                        minLeft = uiControl.UIWin.Left + leftDelta;
                    if (uiControl.UIWin.Top + topDelta < minTop)
                        minTop = uiControl.UIWin.Top + topDelta;
                    if (uiControl.UIWin.Right + leftDelta > maxRight)
                        maxRight = uiControl.UIWin.Right + leftDelta;
                    if (uiControl.UIWin.Bottom + topDelta > maxBottom)
                        maxBottom = uiControl.UIWin.Bottom + topDelta;
                }

                retRect.X = minLeft;
                retRect.Y = minTop;
                retRect.Width = maxRight - minLeft;
                retRect.Height = maxBottom - minTop;
            }

            return retRect;
        }

        public void UpdateUIControlsSelection(ObservableCollection<UIEditor.WinBase> selection, bool reUpdateProperty = true)//bool bUpdateControlContainer)
        {
            var rect = GetSelectionControlsBoundRect(selection);
            //if (rect == mControlContainerRect)
            //    return;

            if (rect == System.Drawing.Rectangle.Empty)
            {
                if (ProAndBindPanel != null && reUpdateProperty)
                {
                    if (selection.Count > 0)
                    {
                        ProAndBindPanel.PropertyInstanceObject = selection[0].UIWin;
                        mControlContainerVisible = Visibility.Visible;
                    }
                    else
                    {
                        ProAndBindPanel.PropertyInstanceObject = null;
                        mControlContainerVisible = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                mControlContainerVisible = Visibility.Visible;

                if (selection.Count > 0 && ProAndBindPanel != null && reUpdateProperty)
                    ProAndBindPanel.PropertyInstanceObject = selection[0].UIWin;


                //////SetControlContainerRect(rect.X, rect.Y, rect.Width, rect.Height);

                //////mControlContainerRectStore = mControlContainerRect;
                //if (mUIControlsContainerHolder != null)
                //    mUIControlsContainerHolder.SelectedItems = mSelectedWinControls;
            }

            UpdateUIControlRectRestores(selection);

            //if (bUpdateControlContainer && UIControlsContainerHolder != null)
            //{
            //    UIControlsContainerHolder.UpdateSelectItems(false);
            //}
        }


        private UISystem.WinBase CheckSelectUI(CSUtility.Support.Point pt, UISystem.WinBase control)
        {
            if (control == null)
                return null;

            UISystem.WinBase retControl = null;

            //if (control.Parent != null && control.Parent != mWinUIRoot)
            //{
            //    pt.X -= ((UISystem.WinBase)control.Parent).Left;
            //    pt.Y -= ((UISystem.WinBase)control.Parent).Top;
            //}

            //var rect = new System.Drawing.Rectangle(control.Left, control.Top, control.Width, control.Height);
            //rect = GetControlContainerUIRootOffsetRect(rect);

            //if (rect.Contains(pt))
            //{
            //    retControl = control;

            //    ////if (control.GetChildWinCount() > 0)
            //    //{
            //    //    for (int i = 0; i < control.GetChildWinCount(); i++)
            //    //    {
            //    //        var childWin = control.GetChildWin(i);
            //    //        if (childWin.Visibility != Visibility.Visible || !childWin.IsVisibleInEditor
            //    //            || childWin.VisibleInTreeView != Visibility.Visible)
            //    //            continue;

            //    //        var selectCtrl = CheckSelectUI(pt, childWin);
            //    //        if (selectCtrl != null)
            //    //        {
            //    //            retControl = selectCtrl;
            //    //            break;
            //    //        }
            //    //    }
            //    //}
            //    foreach (var childWin in control.LogicChildren)
            //    {
            //        var selectCtrl = CheckSelectUI(pt, childWin);
            //        if (selectCtrl != null)
            //        {
            //            retControl = selectCtrl;
            //            break;
            //        }
            //    }
            //}

            //if (SelectedWinControls.Contains(retControl))
            //    retControl = null;

            var rootWin = control.GetRoot() as UISystem.WinRoot;
            if(rootWin==null)
                return null;

            var popStay = rootWin.PopupedStayWindow(ref pt);

            if (popStay != null)
                retControl = GetMousePointAtWin(ref pt, popStay);
            else
                retControl = GetMousePointAtWin(ref pt, rootWin);

            return retControl;
        }

        private UISystem.WinBase GetMousePointAtWin(ref CSUtility.Support.Point pt, UISystem.WinBase parentWin)
        {
            if (parentWin.AbsRect.Contains(pt))
            {
                var childWins = parentWin.GetChildWindows();
                for(int i = childWins.Length - 1; i >= 0; --i)
                {
                    if (childWins[i].Visibility != UISystem.Visibility.Visible)
                        continue;
                    if (childWins[i].VisibleInTreeView != UISystem.Visibility.Visible &&
                        !(childWins[i] is UISystem.Content.ContentPresenter) &&
                        !(childWins[i] is UISystem.Content.ItemsPresenter) &&
                        !(childWins[i] is UISystem.Content.ScrollContentPresenter))
                        continue;
                    if (childWins[i].IsVisibleInEditor == false)
                        continue;

                    var pWin = GetMousePointAtWin(ref pt, childWins[i]);
                    if (pWin != null)
                        return pWin;
                }

                return parentWin;
            }

            return null;
        }

        private void UpdateSelectionControlTransWithRect(ref CSUtility.Support.Rectangle tagRect, ref CSUtility.Support.Rectangle oldRect)
        {
            //mControlContainerRectStore
            //var controlBoundRect = GetSelectionControlsBoundRect();

            var leftOffset = tagRect.Left - oldRect.Left;
            var topOffset = tagRect.Top - oldRect.Top;

            var scaleX = ((float)tagRect.Width) / oldRect.Width;
            var scaleY = ((float)tagRect.Height) / oldRect.Height;

            if (leftOffset == 0 && topOffset == 0 && scaleX == 1 && scaleY == 1)
                return;

            //if (float.IsNaN(scaleX) || scaleX == 0)
            //    scaleX = 1;
            //if (float.IsNaN(scaleY) || scaleY == 0)
            //    scaleY = 1;

            for (int i = 0; i < SelectedWinControls.Count; i++)
            {
                var uiControl = SelectedWinControls[i].UIWin;

                if (!uiControl.EnableEditorMouseMove)
                    continue;

                int leftDelta = 0, topDelta = 0;

                //if (uiControl != mWinRootForm)
                //{
                //    leftDelta = mWinRootForm.Left;
                //    topDelta = mWinRootForm.Top;
                //}
                
                var ctrlOriRect = mSelectionControlRectRestores[i];

                var parentLoc = CSUtility.Support.Point.Empty;
                if(uiControl != mWinRootForm)
                    parentLoc = GetAbsoluteLocation(uiControl.Parent as UISystem.WinBase);
                
                var deltaLeft = ((float)(ctrlOriRect.Left - oldRect.Left)) / oldRect.Width;
                //uiControl.Left = ctrlOriRect.Left - parentLoc.X + (int)(leftOffset + deltaLeft * (tagRect.Width - oldRect.Width)) - leftDelta;

                var deltaTop = ((float)(ctrlOriRect.Top - oldRect.Top)) / oldRect.Height;
                //uiControl.Top = ctrlOriRect.Top - parentLoc.Y + (int)(topOffset + deltaTop * (tagRect.Height - oldRect.Height)) - topDelta;

                var leftSet = ctrlOriRect.Left - parentLoc.X + (int)(leftOffset + deltaLeft * (tagRect.Width - oldRect.Width)) - leftDelta;
                var topSet = ctrlOriRect.Top - parentLoc.Y + (int)(topOffset + deltaTop * (tagRect.Height - oldRect.Height)) - topDelta;
                
                uiControl.Width = (int)(ctrlOriRect.Width * scaleX);
                uiControl.Height = (int)(ctrlOriRect.Height * scaleY);
                var pt = new CSUtility.Support.Point(leftSet, topSet);
                //uiControl.MoveWin(ref pt);
                uiControl.Margin = uiControl.GetMargin(leftSet, topSet, uiControl.Width, uiControl.Height, uiControl.Parent as UISystem.WinBase);
            }

            //mWinRoot.Tick(CCore.Engine.Instance.GetFrameMillisecond());

            //foreach (var uiControl in mSelectedWinControls)
            //for (int i = 0; i < mSelectedWinControls.Count; i++ )
            //{
            //    var uiControl = mSelectedWinControls[i];
            //    var ctrlOriRect = mSelectionControlRectRestores[i];

            //    var deltaPt = GetAbsoluteLocation(uiControl);

            //    var deltaLeft = ((float)(deltaPt.X - mControlContainerRectStore.Left)) / mControlContainerRectStore.Width;
            //    uiControl.Left += (int)System.Math.Floor(leftOffset + deltaLeft * (tagRect.Width - mControlContainerRectStore.Width));

            //    var deltaTop = ((float)(deltaPt.Y - mControlContainerRectStore.Top)) / mControlContainerRectStore.Height;
            //    uiControl.Top += (int)System.Math.Floor(topOffset + deltaTop * (tagRect.Height - mControlContainerRectStore.Height));

            //    uiControl.Width = (int)(uiControl.Width * scaleX);
            //    uiControl.Height = (int)(uiControl.Height * scaleY);

            //    //uiControl.Left += leftOffset;
            //    //uiControl.Top += topOffset;
            //}
        }

        #endregion

        #region 选择框数据

        enum enControlRectType
        {
            LeftTop = 0,
            Top = 1,
            RightTop = 2,
            Right = 3,
            RightBottom = 4,
            Bottom = 5,
            LeftBottom = 6,
            Left = 7,
            Container = 8,
            None,
        }
        
        //CSUtility.Support.Rectangle mControlContainerRect = new CSUtility.Support.Rectangle();
        /* 0 1 2
           7   3
           6 5 4 */
        //CSUtility.Support.Rectangle[] mControlContainerHandleRect = new CSUtility.Support.Rectangle[8];
        //CSUtility.Support.Rectangle mControlContainerRectStore = new CSUtility.Support.Rectangle();
        //CSUtility.Support.Rectangle mPreSelectionRect = new CSUtility.Support.Rectangle();

        //public void InitControlContainer()
        //{
        //    for (int i = 0; i < 8; i++)
        //    {
        //        mControlContainerHandleRect[i] = new CSUtility.Support.Rectangle(0, 0, 10, 10);
        //    }

        //    SetControlContainerRect(50, 50, 100, 100);
        //}


        //public void SetControlContainerRect(int x, int y, int width, int height)//System.Drawing.Rectangle rect)
        //{
        //    if (width < 0)
        //    {
        //        mControlContainerRect.X = x + width;
        //        mControlContainerRect.Width = -width;
        //    }
        //    else
        //    {
        //        mControlContainerRect.X = x;
        //        mControlContainerRect.Width = width;
        //    }

        //    if (height < 0)
        //    {
        //        mControlContainerRect.Y = y + height;
        //        mControlContainerRect.Height = -height;
        //    }
        //    else
        //    {
        //        mControlContainerRect.Y = y;
        //        mControlContainerRect.Height = height;
        //    }

        //    mControlContainerHandleRect[0].X = mControlContainerRect.X - mControlContainerHandleRect[0].Width;
        //    mControlContainerHandleRect[0].Y = mControlContainerRect.Y - mControlContainerHandleRect[0].Height;

        //    mControlContainerHandleRect[1].X = mControlContainerRect.X + mControlContainerRect.Width / 2 - mControlContainerHandleRect[1].Width / 2;
        //    mControlContainerHandleRect[1].Y = mControlContainerRect.Y - mControlContainerHandleRect[1].Height;

        //    mControlContainerHandleRect[2].X = mControlContainerRect.Right;
        //    mControlContainerHandleRect[2].Y = mControlContainerRect.Y - mControlContainerHandleRect[2].Height;

        //    mControlContainerHandleRect[3].X = mControlContainerRect.Right;
        //    mControlContainerHandleRect[3].Y = mControlContainerRect.Y + mControlContainerRect.Height / 2 - mControlContainerHandleRect[3].Height / 2;

        //    mControlContainerHandleRect[4].X = mControlContainerRect.Right;
        //    mControlContainerHandleRect[4].Y = mControlContainerRect.Bottom;

        //    mControlContainerHandleRect[5].X = mControlContainerRect.X + mControlContainerRect.Width / 2 - mControlContainerHandleRect[5].Width / 2;
        //    mControlContainerHandleRect[5].Y = mControlContainerRect.Bottom;

        //    mControlContainerHandleRect[6].X = mControlContainerRect.X - mControlContainerHandleRect[6].Width;
        //    mControlContainerHandleRect[6].Y = mControlContainerRect.Bottom;

        //    mControlContainerHandleRect[7].X = mControlContainerRect.X - mControlContainerHandleRect[7].Width;
        //    mControlContainerHandleRect[7].Y = mControlContainerRect.Y + mControlContainerRect.Height / 2 - mControlContainerHandleRect[7].Height / 2;

        //}

        private CSUtility.Support.Rectangle GetControlContainerUIRootOffsetRect(CSUtility.Support.Rectangle rect)
        {
            CSUtility.Support.Rectangle drawRect = rect;

            drawRect.X += mWinRoot.Left;
            drawRect.Y += mWinRoot.Top;

            //drawRect.Width = (int)(rect.Width * mRenderScaleX);
            //drawRect.Height = (int)(rect.Height * mRenderScaleY);

            return drawRect;
        }

        public CSUtility.Support.Point GetControlContainerUIRootOffsetPoint(CSUtility.Support.Point pt)
        {
            CSUtility.Support.Point retPt = pt;
            retPt.X += mWinRoot.Left;
            retPt.Y += mWinRoot.Top;
            return retPt;
        }

#endregion

#region 渲染部分

        public void Tick()
        {
            if (!mInitialized)
                return;

            if (m_REnviroment != null)
            {
                m_World.Tick();
                m_World.Render2Enviroment(m_Renderparam);

                m_REnviroment.Tick();
                m_REnviroment.RenderUIWithScale(mRenderScaleX, mRenderScaleY, mRenderScaleCenter);

                UISystem.IRender.GetInstance().UIRenderer.ClearAllCommit(mUIRenderPipe);
                mWinRoot.Tick(CCore.Engine.Instance.GetFrameMillisecond());
                SlimDX.Matrix transMat = SlimDX.Matrix.Identity;
                mWinRoot.Draw(mUIRenderPipe, 0, ref transMat);

                //////////////////////////////////////////////////////////////////////////
                //if (m_CameraController != null)
                //    m_CameraController.Tick();

                //if (mlCurrTime == 0)
                //{
                //    mlCurrTime = CCore.Engine.Instance.GetFrameMillisecond();
                //}
                //long elapse = CCore.Engine.Instance.GetFrameMillisecond() - mlCurrTime;
                //mlCurrTime = CCore.Engine.Instance.GetFrameMillisecond();
                //if (m_Mesh != null)
                //{
                //    m_Mesh.Update(elapse);
                //}
                //////////////////////////////////////////////////////////////////////////

                UpdateRTBitmapData();
                UISystem.IRender.GetInstance().UIRenderer.SwapQueue(mUIRenderPipe);
            }
        }

        public void SaveToFile(string strFileName, CCore.enD3DXIMAGE_FILEFORMAT fileFormat, int tickTimes = 2)
        {            
            for (int i = 0; i < tickTimes; ++i)
            {
                CCore.Engine.Instance.Tick(true);

                //if (m_CameraController != null)
                //    m_CameraController.Tick();

                m_World.Tick();
                m_World.Render2Enviroment(m_Renderparam);

                m_REnviroment.Tick();
                //m_REnviroment.RenderUIWithScale(mRenderScaleX, mRenderScaleY, mRenderScaleCenter);

                CCore.Engine.Instance.Client.Graphics.BeginDraw();
                m_REnviroment.Render();
                CCore.Engine.Instance.Client.Graphics.EndDraw();
                m_REnviroment.SwapPipe();
                m_REnviroment.ClearAllDrawingCommits();

                System.Threading.Thread.Sleep(10);
            }

            m_REnviroment.Save2File(strFileName, fileFormat);
        }
        //protected override Size ArrangeOverride(Size arrangeBounds)
        //{
        //    return base.ArrangeOverride(arrangeBounds);
        //}
        //protected override Size MeasureOverride(Size constraint)
        //{
        //    return base.MeasureOverride(constraint);
        //}
        //System.Drawing.Rectangle mMousePointRect = new System.Drawing.Rectangle(0,0,10,10);
        UISystem.UIRenderPipe mUIRenderPipe = new UISystem.UIRenderPipe();
        //UISystem.UIRenderPipe NullUIPipe = new UISystem.UIRenderPipe(IntPtr.Zero);
        public void AfterRender2View(CCore.Graphics.REnviroment env)
        {
            UISystem.IRender.GetInstance().UIRenderer.CommitDrawCall(mUIRenderPipe);
            //SlimDX.Matrix transMat = SlimDX.Matrix.Identity;
            //mWinRoot.Draw(NullUIPipe, 0, ref transMat);

            /*UISystem.IRender.GetInstance().SetClipRect(NullUIPipe, mClipRect);
            if (mControlContainerVisible)
            {
                // 绘制选择控件的包围盒
                foreach (var ctrlWin in SelectedWinControls)
                {
                    var ctrl = ctrlWin.UIWin;

                    if (ctrl.Parent == null)
                        continue;

                    var loc = GetAbsoluteLocation(ctrl);
                    var rect = GetControlContainerUIRootOffsetRect(new CSUtility.Support.Rectangle(loc.X, loc.Y, ctrl.Width, ctrl.Height));
                    UISystem.IRender.GetInstance().DrawRect(NullUIPipe, 511, rect, CSUtility.Support.Color.Red);

                    if (ctrl != mWinRootForm)
                    {
                        var parLoc = GetControlContainerUIRootOffsetPoint(GetAbsoluteLocation(ctrl.Parent as UISystem.WinBase));
                        var offset = ((UISystem.WinBase)ctrl.Parent).GetChildOffset(ctrl);
                        parLoc.X += offset.X;
                        parLoc.Y += offset.Y;
                        var parSize = ((UISystem.WinBase)ctrl.Parent).GetSizeByChild(ctrl);

                        if(ctrl.EnableHorizontalArrangementLineShow)
                        {
                            var leftCenter = new CSUtility.Support.Point(rect.X, rect.Y + rect.Height / 2);
                            var rightCenter = new CSUtility.Support.Point(rect.X + rect.Width, rect.Y + rect.Height / 2);
                            switch (ctrl.HorizontalAlignment)
                            {
                                case UISystem.UI.HorizontalAlignment.Left:
                                    UISystem.IRender.GetInstance().DrawLine(NullUIPipe, 511, new CSUtility.Support.Point(parLoc.X, leftCenter.Y), leftCenter, CSUtility.Support.Color.Red);
                                    break;
                                case UISystem.UI.HorizontalAlignment.Center:
                                    break;
                                case UISystem.UI.HorizontalAlignment.Right:
                                    UISystem.IRender.GetInstance().DrawLine(NullUIPipe, 511, new CSUtility.Support.Point(parLoc.X + parSize.Width, rightCenter.Y),
                                                                            rightCenter, CSUtility.Support.Color.Red);
                                    break;
                                case UISystem.UI.HorizontalAlignment.Stretch:
                                    UISystem.IRender.GetInstance().DrawLine(NullUIPipe, 511, new CSUtility.Support.Point(parLoc.X, leftCenter.Y), leftCenter, CSUtility.Support.Color.Red);
                                    UISystem.IRender.GetInstance().DrawLine(NullUIPipe, 511, new CSUtility.Support.Point(parLoc.X + parSize.Width, rightCenter.Y),
                                                                            rightCenter, CSUtility.Support.Color.Red);
                                    break;
                            }
                        }

                        if (ctrl.EnableVerticalArrangementLineShow)
                        {
                            var topCenter = new CSUtility.Support.Point(rect.X + rect.Width / 2, rect.Y);
                            var bottomCenter = new CSUtility.Support.Point(rect.X + rect.Width / 2, rect.Y + rect.Height);
                            switch (ctrl.VerticalAlignment)
                            {
                                case UISystem.UI.VerticalAlignment.Top:
                                    UISystem.IRender.GetInstance().DrawLine(NullUIPipe, 511, new CSUtility.Support.Point(topCenter.X, parLoc.Y), topCenter, CSUtility.Support.Color.Red);
                                    break;
                                case UISystem.UI.VerticalAlignment.Center:
                                    break;
                                case UISystem.UI.VerticalAlignment.Bottom:
                                    UISystem.IRender.GetInstance().DrawLine(NullUIPipe, 511, new CSUtility.Support.Point(topCenter.X, parLoc.Y + parSize.Height), bottomCenter, CSUtility.Support.Color.Red);
                                    break;
                                case UISystem.UI.VerticalAlignment.Stretch:
                                    UISystem.IRender.GetInstance().DrawLine(NullUIPipe, 511, new CSUtility.Support.Point(topCenter.X, parLoc.Y), topCenter, CSUtility.Support.Color.Red);
                                    UISystem.IRender.GetInstance().DrawLine(NullUIPipe, 511, new CSUtility.Support.Point(topCenter.X, parLoc.Y + parSize.Height), bottomCenter, CSUtility.Support.Color.Red);
                                    break;
                            }
                        }
                    }
                }

                UISystem.IRender.GetInstance().DrawRect(NullUIPipe, 511, GetControlContainerUIRootOffsetRect(mControlContainerRect), CSUtility.Support.Color.Red);
                var clr = new SlimDX.Vector4(0.5647f,0.9333f,0.5647f,1);
                foreach (var rect in mControlContainerHandleRect)
                {
                    UISystem.IRender.GetInstance().FillRectangle(NullUIPipe, 511, mWinRoot.Width, mWinRoot.Height, GetControlContainerUIRootOffsetRect(rect), ref clr, ref transMat);
                    UISystem.IRender.GetInstance().DrawRect(NullUIPipe, 511, GetControlContainerUIRootOffsetRect(rect), CSUtility.Support.Color.Red);
                }
            }

            // 绘制选择控件的辅助信息
            if (mWinRootForm != null)
            {
                var winParent = mWinRootForm.Parent as UISystem.WinBase;
                mWinRootForm.RenderAssist(NullUIPipe, 511, new CSUtility.Support.Point(winParent.Left, winParent.Top));
            }
            //foreach (var ctrl in mAllWinControls)
            //{
            //    var loc = GetAbsoluteLocation(ctrl);
            //    ctrl.RenderAssist(loc);
            //}
            
            // 绘制吸附线
            if (mSnapXShow)
            {
                UISystem.IRender.GetInstance().DrawLine(NullUIPipe, 511, new CSUtility.Support.Point(mSnapPosX + mWinUIRoot.Left, 0), new CSUtility.Support.Point(mSnapPosX + mWinUIRoot.Left, (int)FormHost.ActualHeight), CSUtility.Support.Color.Green);
            }
            if (mSnapYShow)
            {
                UISystem.IRender.GetInstance().DrawLine(NullUIPipe, 511, new CSUtility.Support.Point(0, mSnapPosY + mWinUIRoot.Top), new CSUtility.Support.Point((int)FormHost.ActualWidth, mSnapPosY + mWinUIRoot.Top), CSUtility.Support.Color.Green);
            }

            // 绘制窗口大小
            UISystem.IRender.GetInstance().DrawRect(NullUIPipe, 511, GetControlContainerUIRootOffsetRect(mWindowsRect), CSUtility.Support.Color.White);
            //UISystem.IRender.GetInstance().DrawRect(mMousePointRect, System.Drawing.Color.Red);  
      
            // 绘制预选择框
            if (mbShowPreSelectionRect)
                UISystem.IRender.GetInstance().DrawRect(NullUIPipe, 511, GetControlContainerUIRootOffsetRect(mPreSelectionRect), CSUtility.Support.Color.OrangeRed);*/
        }

        #endregion

        #region 对象创建

        UIControlsBrowser_Item mSelectedUIControlsBrowserItem = null;
        public UIControlsBrowser_Item SelectedUIControlsBrowserItem
        {
            get { return mSelectedUIControlsBrowserItem; }
            set { mSelectedUIControlsBrowserItem = value; }
        }

#endregion
        
        public delegate void Delegate_UpdateIndex(int index);
        public Delegate_UpdateIndex OnUpdateIndex;

        System.DateTime mLastKeyDownTime = System.DateTime.Now;
        List<int> mKeyList = new List<int>();
        private void CheckNumberKey(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                var now = System.DateTime.Now;
                var timeSpan = now - mLastKeyDownTime;
                if (timeSpan.TotalSeconds > 0.6)
                    mKeyList.Clear();
                mLastKeyDownTime = now;
                mKeyList.Add((int)e.Key - (int)Key.D0);

                UpdateIndex();
            }
        }
        private void UpdateIndex()
        {
            int j = 1;
            int index = 0;
            for (int i = mKeyList.Count - 2; i >= 0; i--)
            {
                index += mKeyList[i] * (j * 10);
                j++;
            }
            index += mKeyList[mKeyList.Count - 1] - 1;

            if (OnUpdateIndex != null)
                OnUpdateIndex(index);
        }

        private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    {
                        foreach (var ctrl in SelectedWinControls)
                        {
                            if (ctrl.UIWin == mWinRootForm)
                            {
                                CheckNumberKey(sender, e);
                                return;
                            }
                        }
                        //if (SelectedWinControls.Contains(mWinRootForm))
                        //    break;

                        List<UISystem.WinBase> deletedControls = new List<UISystem.WinBase>();
                        foreach (var ctrl in SelectedWinControls)
                        {
                            ctrl.UIWin.Parent = null;
                            //mAllWinControls.Remove(ctrl);
                            deletedControls.Add(ctrl.UIWin);
                        }
                        SelectedWinControls.Clear();

                        if (OnDeleteControls != null)
                            OnDeleteControls(deletedControls);
                    }
                    break;

                case Key.D:     // 向右移动
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            //int delta = 1;
                            //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            //    delta = 10;
                            //mControlContainerRectStore = mControlContainerRect;
                            //SetControlContainerRect(mControlContainerRect.X + delta, mControlContainerRect.Y, mControlContainerRect.Width, mControlContainerRect.Height);
                            //UpdateSelectionControlTransWithRect(mControlContainerRect);
                            //UpdateUIControlsSelection(SelectedWinControls);
                        }
                    }
                    break;

                case Key.A:     // 向左移动
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            //int delta = 1;
                            //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            //    delta = 10;
                            //mControlContainerRectStore = mControlContainerRect;
                            //SetControlContainerRect(mControlContainerRect.X - delta, mControlContainerRect.Y, mControlContainerRect.Width, mControlContainerRect.Height);
                            //UpdateSelectionControlTransWithRect(mControlContainerRect);
                            //UpdateUIControlsSelection(SelectedWinControls);
                        }
                        //else
                        //{
                        //    ShowAll();
                        //}
                    }
                    break;

                case Key.W:     // 向上移动
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            //int delta = 1;
                            //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            //    delta = 10;
                            //mControlContainerRectStore = mControlContainerRect;
                            //SetControlContainerRect(mControlContainerRect.X, mControlContainerRect.Y - delta, mControlContainerRect.Width, mControlContainerRect.Height);
                            //UpdateSelectionControlTransWithRect(mControlContainerRect);
                            //UpdateUIControlsSelection(SelectedWinControls);
                        }
                    }
                    break;

                case Key.S:     // 向下移动
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            //int delta = 1;
                            //if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            //    delta = 10;
                            //mControlContainerRectStore = mControlContainerRect;
                            //SetControlContainerRect(mControlContainerRect.X, mControlContainerRect.Y + delta, mControlContainerRect.Width, mControlContainerRect.Height);
                            //UpdateSelectionControlTransWithRect(mControlContainerRect);
                            //UpdateUIControlsSelection(SelectedWinControls);
                        }
                    }
                    break;

                case Key.F:
                    {
                        //FocusSelected();
                    }
                    break;

                case Key.C:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            // 复制
                            CopySelectedControls();
                        }
                    }
                    break;

                case Key.V:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            // 粘贴
                            PasteControls();
                        }
                    }
                    break;
            }

            CheckNumberKey(sender, e);
        }
        
        public void FillParent()
        {
            if(SelectedWinControls.Count <= 0)
                return;

            // 先判断选定的对象是否为同父，不同父则不能进行填充操作
            if (SelectedWinControls.Count > 1)
            {
                var parent = SelectedWinControls[0].UIWin.Parent;
                for (int i = 1; i < SelectedWinControls.Count; i++)
                {
                    if (SelectedWinControls[i].UIWin.Parent != parent)
                    {
                        EditorCommon.MessageBox.Show("填充操作控件的父对象必须相同", "错误");
                        return;
                    }
                }
            }

            var tempParent = SelectedWinControls[0].UIWin.Parent as UISystem.WinBase;
            var loc = GetAbsoluteLocation(tempParent as UISystem.WinBase);
            //////mControlContainerRectStore = mControlContainerRect;
            //////SetControlContainerRect(loc.X, loc.Y, tempParent.Width, tempParent.Height);
            //////UpdateSelectionControlTransWithRect(mControlContainerRect);
            UpdateUIControlsSelection(SelectedWinControls);
            //mSelectionControlRectRestores.Clear();
            //foreach (var ctrl in SelectedWinControls)
            //{
            //    var ctrlLoc = GetAbsoluteLocation(ctrl);
            //    var boundRect = new System.Drawing.Rectangle(ctrlLoc.X, ctrlLoc.Y, ctrl.Width, ctrl.Height);
            //    mSelectionControlRectRestores.Add(boundRect);
            //}
        }

        public void SetToImageSize()
        {
            mSelectionControlRectRestores.Clear();
            foreach (var ctrlWin in SelectedWinControls)
            {
                var ctrl = ctrlWin.UIWin;

                if (ctrl.RState != null)
                {
                    var size = ctrl.RState.GetImageSize();
                    if (size != CSUtility.Support.Size.Empty)
                    {
                        ctrl.Width = size.Width;
                        ctrl.Height = size.Height;
                    }
                }

                var ctrlLoc = GetAbsoluteLocation(ctrl);
                var boundRect = new System.Drawing.Rectangle(ctrlLoc.X, ctrlLoc.Y, ctrl.Width, ctrl.Height);
                mSelectionControlRectRestores.Add(boundRect);
            }

            UpdateUIControlsSelection(SelectedWinControls);
        }

        public void SetToHorizontalCenter()
        {
            if (SelectedWinControls.Count <= 0)
                return;

            if (SelectedWinControls.Count > 1)
            {
                var parent = SelectedWinControls[0].UIWin.Parent;
                for (int i = 1; i < SelectedWinControls.Count; i++)
                {
                    if (SelectedWinControls[i].UIWin.Parent != parent)
                    {
                        EditorCommon.MessageBox.Show("横向居中操作控件的父对象必须相同", "错误");
                        return;
                    }
                }
            }

            var tempParent = SelectedWinControls[0].UIWin.Parent as UISystem.WinBase;
            var loc = GetAbsoluteLocation(tempParent as UISystem.WinBase);
            //////loc.X = loc.X + tempParent.Width / 2 - mControlContainerRect.Width / 2;
            //////mControlContainerRectStore = mControlContainerRect;
            //////SetControlContainerRect(loc.X, mControlContainerRect.Top, mControlContainerRect.Width, mControlContainerRect.Height);
            //////UpdateSelectionControlTransWithRect(mControlContainerRect);
            UpdateUIControlsSelection(SelectedWinControls);
            //mSelectionControlRectRestores.Clear();
            //foreach (var ctrl in SelectedWinControls)
            //{
            //    var ctrlLoc = GetAbsoluteLocation(ctrl);
            //    var boundRect = new System.Drawing.Rectangle(ctrlLoc.X, ctrlLoc.Y, ctrl.Width, ctrl.Height);
            //    mSelectionControlRectRestores.Add(boundRect);
            //}
        }

        public void SetToVerticalCenter()
        {
            if (SelectedWinControls.Count <= 0)
                return;

            if (SelectedWinControls.Count > 1)
            {
                var parent = SelectedWinControls[0].UIWin.Parent;
                for (int i = 1; i < SelectedWinControls.Count; i++)
                {
                    if (SelectedWinControls[i].UIWin.Parent != parent)
                    {
                        EditorCommon.MessageBox.Show("纵向居中操作控件的父对象必须相同", "错误");
                        return;
                    }
                }
            }

            var tempParent = SelectedWinControls[0].UIWin.Parent as UISystem.WinBase;
            var loc = GetAbsoluteLocation(tempParent as UISystem.WinBase);
            //////loc.Y = loc.Y + tempParent.Height / 2 - mControlContainerRect.Height / 2;
            //////mControlContainerRectStore = mControlContainerRect;
            //////SetControlContainerRect(mControlContainerRect.Left, loc.Y, mControlContainerRect.Width, mControlContainerRect.Height);
            //////UpdateSelectionControlTransWithRect(mControlContainerRect);
            UpdateUIControlsSelection(SelectedWinControls);
            //mSelectionControlRectRestores.Clear();
            //foreach (var ctrl in SelectedWinControls)
            //{
            //    var ctrlLoc = GetAbsoluteLocation(ctrl);
            //    var boundRect = new System.Drawing.Rectangle(ctrlLoc.X, ctrlLoc.Y, ctrl.Width, ctrl.Height);
            //    mSelectionControlRectRestores.Add(boundRect);
            //}
        }

#region 复制粘贴

        List<UIEditor.WinBase> mCopyedControls = new List<UIEditor.WinBase>();
        //bool mCopyByMove = false;
        public void CopySelectedControls()
        {
            mCopyedControls.Clear();
            mCopyedControls.AddRange(SelectedWinControls);
        }

        public void PasteControls()
        {
            SelectedWinControls.Clear();
            //UpdateUIControlsSelection();


            foreach (var edControl in mCopyedControls)
            {
                var control = edControl.UIWin;

                if (control == WinRootForm)
                    continue;

                var pCtrl = control.GetType().Assembly.CreateInstance(control.GetType().ToString()) as UISystem.WinBase;

                //foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(control))
                //{
                //    property.SetValue(pCtrl, property.GetValue(control));
                //}
                pCtrl.CopyFrom(control, new List<string>(){"LogicChildren"}, false, true);
                //pCtrl.CopyFrom(control);

                pCtrl.Parent = control.Parent;
                pCtrl.OnPropertyChangedEvent += OnWinControlPropertyChanged;
                SelectedWinControls.Add(WinBase.GetHostWin(pCtrl));
            }

            UpdateUIControlRectRestores(SelectedWinControls);
            //UpdateUIControlsSelection(SelectedWinControls);
        }

#endregion

        public event UISystem.WinBase.Delegate_OnPropertyChanged OnWinControlPropertyChangedEvent;
        private void OnWinControlPropertyChanged(UISystem.WinBase control, string propertyName)
        {
            if (OnWinControlPropertyChangedEvent != null)
                OnWinControlPropertyChangedEvent(control, propertyName);
        }

        #region 旧代码

        /*
        private enControlRectType IsPointInRect(CSUtility.Support.Point pt)
        {
            // pt 经过缩放变换处理
            for (int i = 0; i < 8; i++)
            {
                var drawRect = GetControlContainerUIRootOffsetRect(mControlContainerHandleRect[i]);
                if (drawRect.Contains(pt))
                    return (enControlRectType)i;
            }

            return enControlRectType.None;
        }

        private void WinDrawPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var deltaX = e.Location.X - mMouseBtnDownPoint.X;
            var deltaY = e.Location.Y - mMouseBtnDownPoint.Y;

            var mpt = GetScreenPointToRenderPoint(e.Location);

            if (e.Button == System.Windows.Forms.MouseButtons.None)
            {
                // 预选择框
                mWinMousePointAtControl = CheckSelectUI(mpt, mWinRootForm);
                if (mWinMousePointAtControl != null)
                {
                    if (EditMode != enEditMode.Create ||
                       (EditMode == enEditMode.Create && mWinMousePointAtControl.CanInsertChild()))
                    {
                        var loc = GetAbsoluteLocation(mWinMousePointAtControl);
                        mPreSelectionRect.X = loc.X;
                        mPreSelectionRect.Y = loc.Y;
                        mPreSelectionRect.Width = mWinMousePointAtControl.Width;
                        mPreSelectionRect.Height = mWinMousePointAtControl.Height;
                        mbShowPreSelectionRect = true;
                    }
                }
                else
                    mbShowPreSelectionRect = false;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if ((deltaX * deltaX + deltaY * deltaY) > 5)
                    mMouseMoved = true;

                switch (EditMode)
                {
                    case enEditMode.Create:
                        {
                            if (mWinCreatedParentControl != null)
                            {
                                var deltaLeft = mWinCreatedParentControl.AbsRect.Left;
                                var deltaTop = mWinCreatedParentControl.AbsRect.Top;
                                var mouseRenderPosition = GetScreenPointToRenderPoint(mMouseBtnDownPoint);
                                mouseRenderPosition.X -= deltaLeft;// mWinUIRoot.Left;
                                mouseRenderPosition.Y -= deltaTop;// mWinUIRoot.Top;
                                var tempRenderPosition = GetScreenPointToRenderPoint(e.Location);
                                tempRenderPosition.X -= deltaLeft;// mWinUIRoot.Left;
                                tempRenderPosition.Y -= deltaTop;// mWinUIRoot.Top;
                                deltaX = tempRenderPosition.X - mouseRenderPosition.X;
                                deltaY = tempRenderPosition.Y - mouseRenderPosition.Y;
                                SetControlContainerRect(mouseRenderPosition.X, mouseRenderPosition.Y, deltaX, deltaY);

                                if (SelectedWinControls.Count > 0)
                                {
                                    var uiControl = SelectedWinControls[0].UIWin;
                                    //uiControl.Left = mControlContainerRect.X - mWinRootForm.Left;
                                    //uiControl.Top = mControlContainerRect.Y - mWinRootForm.Top;
                                    uiControl.Margin = new CSUtility.Support.Thickness(mControlContainerRect.X - mWinRootForm.Left, mControlContainerRect.Y - mWinRootForm.Top, uiControl.Margin.Right, uiControl.Margin.Bottom);
                                    uiControl.Width = mControlContainerRect.Width;
                                    uiControl.Height = mControlContainerRect.Height;
                                }
                                //UpdateSelectionControlTransWithRect(mControlContainerRect);
                            }

                        }
                        break;

                    case enEditMode.MoveCanvas:
                        break;

                    case enEditMode.SelectAndOperateControl:
                        {
                            mIsMouseDragging = true;

                            var selCtrl = new CSUtility.Support.ThreadSafeObservableCollection<UISystem.WinBase>();
                            foreach (var ctrl in SelectedWinControls)
                            {
                                selCtrl.Add(ctrl.UIWin);
                            }

                            if (mCopyByMove)
                            {
                                CopySelectedControls();
                                PasteControls();
                                mCopyByMove = false;
                            }

                            switch (mSelectedControlRectType)
                            {
                                case enControlRectType.Container:
                                    {
                                        var left = (int)(mControlContainerRectStore.X + deltaX * mRenderScaleX);
                                        var top = (int)(mControlContainerRectStore.Y + deltaY * mRenderScaleY);
                                        if (SnapEnable)
                                        {
                                            // 吸附计算
                                            bool bGetLeftValue = false, bGetRightValue = false;
                                            int leftOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestX(left, ref leftOutValue, mSnapDistance, selCtrl))
                                            {
                                                bGetLeftValue = true;
                                            }
                                            int rightOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestX(left + mControlContainerRect.Width, ref rightOutValue, mSnapDistance, selCtrl))
                                            {
                                                bGetRightValue = true;
                                            }
                                            if (!bGetLeftValue && !bGetRightValue)
                                                mSnapXShow = false;
                                            else
                                            {
                                                var leftOffset = System.Math.Abs(left - leftOutValue);
                                                var rightOffset = System.Math.Abs(left + mControlContainerRect.Width - rightOutValue);

                                                if (leftOffset <= rightOffset && leftOffset < mSnapDistance)
                                                {
                                                    left = leftOutValue;
                                                    mSnapXShow = true;
                                                    mSnapPosX = left;
                                                }
                                                else if (rightOffset < leftOffset && rightOffset < mSnapDistance)
                                                {
                                                    left = rightOutValue - mControlContainerRect.Width;
                                                    mSnapXShow = true;
                                                    mSnapPosX = rightOutValue;
                                                }
                                            }

                                            bool bGetTopValue = false, bGetBottomValue = false;
                                            int topOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestY(top, ref topOutValue, mSnapDistance, selCtrl))
                                            {
                                                bGetTopValue = true;
                                            }
                                            int bottomOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestY(top + mControlContainerRect.Height, ref bottomOutValue, mSnapDistance, selCtrl))
                                            {
                                                bGetBottomValue = true;
                                            }
                                            if (!bGetTopValue && !bGetBottomValue)
                                                mSnapYShow = false;
                                            else
                                            {
                                                var topOffset = System.Math.Abs(top - topOutValue);
                                                var bottomOffset = System.Math.Abs(top + mControlContainerRect.Height - bottomOutValue);

                                                if (topOffset <= bottomOffset && topOffset < mSnapDistance)
                                                {
                                                    top = topOutValue;
                                                    mSnapYShow = true;
                                                    mSnapPosY = top;
                                                }
                                                else if (bottomOffset < topOffset && bottomOffset < mSnapDistance)
                                                {
                                                    top = bottomOutValue - mControlContainerRect.Height;
                                                    mSnapYShow = true;
                                                    mSnapPosY = bottomOutValue;
                                                }
                                            }
                                        }
                                        SetControlContainerRect(left, top, mControlContainerRect.Width, mControlContainerRect.Height);
                                    }
                                    break;
                                case enControlRectType.LeftTop:
                                    {
                                        int left = (int)(mControlContainerRectStore.X + deltaX * mRenderScaleX);
                                        int top = (int)(mControlContainerRectStore.Y + deltaY * mRenderScaleY);

                                        if (SnapEnable)
                                        {
                                            // 吸附计算
                                            int leftOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestX(left, ref leftOutValue, mSnapDistance, selCtrl))
                                            {
                                                left = leftOutValue;
                                                mSnapXShow = true;
                                                mSnapPosX = left;
                                            }
                                            else
                                            {
                                                mSnapXShow = false;
                                            }
                                            int topOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestY(top, ref topOutValue, mSnapDistance, selCtrl))
                                            {
                                                top = topOutValue;
                                                mSnapYShow = true;
                                                mSnapPosY = top;
                                            }
                                            else
                                            {
                                                mSnapYShow = false;
                                            }
                                        }

                                        int width = (int)(mControlContainerRectStore.Width + (mControlContainerRectStore.X - left));
                                        int height = (int)(mControlContainerRectStore.Height + (mControlContainerRectStore.Y - top));
                                        SetControlContainerRect(left, top, width, height);
                                    }
                                    break;
                                case enControlRectType.Top:
                                    {
                                        int left = mControlContainerRectStore.X;
                                        int top = (int)(mControlContainerRectStore.Y + deltaY * mRenderScaleY);

                                        if (SnapEnable)
                                        {
                                            // 吸附计算
                                            int topOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestY(top, ref topOutValue, mSnapDistance, selCtrl))
                                            {
                                                top = topOutValue;
                                                mSnapYShow = true;
                                                mSnapPosY = top;
                                            }
                                            else
                                            {
                                                mSnapYShow = false;
                                            }
                                        }

                                        int width = mControlContainerRectStore.Width;
                                        //int height = (int)(mControlContainerRectStore.Height - deltaY * mRenderScaleY);
                                        int height = (int)(mControlContainerRectStore.Height + (mControlContainerRectStore.Y - top));
                                        SetControlContainerRect(left, top, width, height);
                                    }
                                    break;
                                case enControlRectType.RightTop:
                                    {
                                        int left = mControlContainerRectStore.X;
                                        int top = (int)(mControlContainerRectStore.Y + deltaY * mRenderScaleY);
                                        int width = (int)(mControlContainerRectStore.Width + deltaX * mRenderScaleX);

                                        if (SnapEnable)
                                        {
                                            // 吸附计算
                                            int rightOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestX(left + width, ref rightOutValue, mSnapDistance, selCtrl))
                                            {
                                                width = rightOutValue - left;
                                                mSnapXShow = true;
                                                mSnapPosX = rightOutValue;
                                            }
                                            else
                                                mSnapXShow = false;

                                            int topOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestY(top, ref topOutValue, mSnapDistance, selCtrl))
                                            {
                                                top = topOutValue;
                                                mSnapYShow = true;
                                                mSnapPosY = top;
                                            }
                                            else
                                            {
                                                mSnapYShow = false;
                                            }
                                        }

                                        //int height = (int)(mControlContainerRectStore.Height - deltaY * mRenderScaleY);
                                        int height = (int)(mControlContainerRectStore.Height + (mControlContainerRectStore.Y - top));
                                        SetControlContainerRect(left, top, width, height);
                                    }
                                    break;
                                case enControlRectType.Right:
                                    {
                                        int left = mControlContainerRectStore.X;
                                        int top = mControlContainerRectStore.Y;
                                        int width = (int)(mControlContainerRectStore.Width + deltaX * mRenderScaleX);

                                        if (SnapEnable)
                                        {
                                            // 吸附计算
                                            int rightOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestX(left + width, ref rightOutValue, mSnapDistance, selCtrl))
                                            {
                                                width = rightOutValue - left;
                                                mSnapXShow = true;
                                                mSnapPosX = rightOutValue;
                                            }
                                            else
                                                mSnapXShow = false;
                                        }

                                        //int width = (int)(mControlContainerRectStore.Width - (mControlContainerRectStore.X - left));
                                        int height = mControlContainerRectStore.Height;
                                        SetControlContainerRect(left, top, width, height);
                                    }
                                    break;
                                case enControlRectType.RightBottom:
                                    {
                                        int left = mControlContainerRectStore.X;
                                        int top = mControlContainerRectStore.Y;
                                        int width = (int)(mControlContainerRectStore.Width + deltaX * mRenderScaleX);
                                        int height = (int)(mControlContainerRectStore.Height + deltaY * mRenderScaleY);

                                        if (SnapEnable)
                                        {
                                            // 吸附计算
                                            int rightOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestX(left + width, ref rightOutValue, mSnapDistance, selCtrl))
                                            {
                                                width = rightOutValue - left;
                                                mSnapXShow = true;
                                                mSnapPosX = rightOutValue;
                                            }
                                            else
                                                mSnapXShow = false;

                                            int bottomOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestY(top + height, ref bottomOutValue, mSnapDistance, selCtrl))
                                            {
                                                height = bottomOutValue - top;
                                                mSnapYShow = true;
                                                mSnapPosY = bottomOutValue;
                                            }
                                            else
                                                mSnapYShow = false;
                                        }

                                        SetControlContainerRect(left, top, width, height);
                                    }
                                    break;
                                case enControlRectType.Bottom:
                                    {
                                        int left = mControlContainerRectStore.X;
                                        int top = mControlContainerRectStore.Y;
                                        int width = mControlContainerRectStore.Width;
                                        int height = (int)(mControlContainerRectStore.Height + deltaY * mRenderScaleY);

                                        if (SnapEnable)
                                        {
                                            int bottomOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestY(top + height, ref bottomOutValue, mSnapDistance, selCtrl))
                                            {
                                                height = bottomOutValue - top;
                                                mSnapYShow = true;
                                                mSnapPosY = bottomOutValue;
                                            }
                                            else
                                                mSnapYShow = false;
                                        }

                                        SetControlContainerRect(left, top, width, height);
                                    }
                                    break;
                                case enControlRectType.LeftBottom:
                                    {
                                        int left = (int)(mControlContainerRectStore.X + deltaX * mRenderScaleX);
                                        int top = mControlContainerRectStore.Y;
                                        //int width = (int)(mControlContainerRectStore.Width - deltaX * mRenderScaleX);
                                        int height = (int)(mControlContainerRectStore.Height + deltaY * mRenderScaleY);

                                        if (SnapEnable)
                                        {
                                            // 吸附计算
                                            int leftOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestX(left, ref leftOutValue, mSnapDistance, selCtrl))
                                            {
                                                left = leftOutValue;
                                                mSnapXShow = true;
                                                mSnapPosX = left;
                                            }
                                            else
                                            {
                                                mSnapXShow = false;
                                            }

                                            int bottomOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestY(top + height, ref bottomOutValue, mSnapDistance, selCtrl))
                                            {
                                                height = bottomOutValue - top;
                                                mSnapYShow = true;
                                                mSnapPosY = bottomOutValue;
                                            }
                                            else
                                                mSnapYShow = false;
                                        }

                                        int width = (int)(mControlContainerRectStore.Width + (mControlContainerRectStore.X - left));
                                        SetControlContainerRect(left, top, width, height);
                                    }
                                    break;
                                case enControlRectType.Left:
                                    {
                                        int left = (int)(mControlContainerRectStore.X + deltaX * mRenderScaleX);

                                        if (SnapEnable)
                                        {
                                            // 吸附计算
                                            int leftOutValue = int.MaxValue;
                                            if (WinRootForm.GetNearestX(left, ref leftOutValue, mSnapDistance, selCtrl))
                                            {
                                                left = leftOutValue;
                                                mSnapXShow = true;
                                                mSnapPosX = left;
                                            }
                                            else
                                            {
                                                mSnapXShow = false;
                                            }
                                        }

                                        int top = mControlContainerRectStore.Y;
                                        //int width = (int)(mControlContainerRectStore.Width - deltaX * mRenderScaleX);
                                        int width = (int)(mControlContainerRectStore.Width + (mControlContainerRectStore.X - left));
                                        int height = mControlContainerRectStore.Height;
                                        SetControlContainerRect(left, top, width, height);
                                    }
                                    break;
                            }

                            UpdateSelectionControlTransWithRect(mControlContainerRect);
                        }
                        break;
                }

            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //mMouseBtnDownPoint = e.Location;

                // 右键拖动
                //mWinUIRoot.Left = mUIRootLocationRestore.X + (int)(deltaX * mRenderScaleX);
                //mWinUIRoot.Top = mUIRootLocationRestore.Y + (int)(deltaY * mRenderScaleY);
                mWinUIRoot.Margin = new CSUtility.Support.Thickness(mUIRootLocationRestore.X + (int)(deltaX * mRenderScaleX), mUIRootLocationRestore.Y + (int)(deltaY * mRenderScaleY), 0, 0);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                mMouseBtnDownPoint = e.Location;

                //mRenderScaleX += deltaY * 0.01f;
                //mRenderScaleY += deltaY * 0.01f;

                //// 缩放
                //foreach (var ctrl in mAllWinControls)
                //{
                //    ctrl.ScaleX = mRenderScaleX;
                //    ctrl.ScaleY = mRenderScaleY;
                //    ctrl.ScaleCenter = mRenderScaleCenter;
                //}
                //mWinUIRoot.Width += deltaY;
                //mWinUIRoot.Height += deltaY;
                ScaleDraw(deltaY, mRenderScaleCenter);
            }
        }

        private void WinDrawPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mMouseBtnDownPoint = e.Location;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mMouseMoved = false;

                switch (EditMode)
                {
                    case enEditMode.Create:
                        // 根据缩放和平移计算鼠标位置
                        mControlContainerVisible = true;
                        var mouseRenderPosition = GetScreenPointToRenderPoint(mMouseBtnDownPoint);
                        mouseRenderPosition.X -= mWinUIRoot.Left;
                        mouseRenderPosition.Y -= mWinUIRoot.Top;
                        SetControlContainerRect(mouseRenderPosition.X, mouseRenderPosition.Y, 1, 1);

                        mWinCreatedParentControl = mWinRootForm;
                        if (SelectedWinControls.Count == 1)
                        {
                            if (SelectedWinControls[0].UIWin.CanInsertChild())
                                mWinCreatedParentControl = SelectedWinControls[0].UIWin;
                        }
                        else
                        {
                            if (mWinMousePointAtControl != null && mWinMousePointAtControl.CanInsertChild())
                                mWinCreatedParentControl = mWinMousePointAtControl;
                        }

                        // 根据选择创建控件
                        if (SelectedUIControlsBrowserItem != null)
                        {
                            SelectedWinControls.Clear();

                            UISystem.WinBase uiControl = null;

                            if (SelectedUIControlsBrowserItem.IsTemplate)
                            {
                                uiControl = SelectedUIControlsBrowserItem.TargetType.Assembly.CreateInstance(SelectedUIControlsBrowserItem.TargetType.ToString()) as UISystem.WinBase;

                                uiControl.TemplateId = SelectedUIControlsBrowserItem.TemplateInfo.ControlTemplate.Id;
                            }
                            else
                            {
                                uiControl = SelectedUIControlsBrowserItem.TargetType.Assembly.CreateInstance(SelectedUIControlsBrowserItem.TargetType.ToString()) as UISystem.WinBase;
                            }
                            //uiControl.Left = mouseRenderPosition.X;
                            //uiControl.Top = mouseRenderPosition.Y;
                            uiControl.OnPropertyChangedEvent += OnWinControlPropertyChanged;

                            uiControl.Width = 1;
                            uiControl.Height = 1;
                            //uiControl.BackColor = System.Drawing.Color.Yellow;
                            //uiControl.ForeColor = System.Drawing.Color.Green;
                            uiControl.Parent = mWinCreatedParentControl;//mWinRootForm;
                            //uiControl.Visible = true;
                            //uiControl.Visibility = System.Windows.Visibility.Visible;
                            //mAllWinControls.Add(uiControl);

                            SelectedWinControls.Add(WinBase.GetHostWin(uiControl));
                            //UpdateUIControlsSelection();
                            UpdateUIControlRectRestores(SelectedWinControls);

                            // 这里强制刷新一下界面，否则有可能不显示
                            //mWinRootForm.Left += 0;
                            //mWinRootForm.Margin = new Thickness(mWinRootForm.Left + 0, mWinRootForm.Margin.Top, mWinRootForm.Margin.Right, mWinRootForm.Margin.Bottom);
                            mWinRootForm.UpdateLayout();
                        }
                        break;

                    case enEditMode.MoveCanvas:
                        break;

                    case enEditMode.SelectAndOperateControl:
                        {
                            var mpt = GetScreenPointToRenderPoint(e.Location);
                            mControlContainerRectStore = mControlContainerRect;

                            if (GetControlContainerUIRootOffsetRect(mControlContainerRect).Contains(mpt))
                            {
                                mSelectedControlRectType = enControlRectType.Container;
                            }
                            else
                            {
                                mSelectedControlRectType = IsPointInRect(mpt);
                            }

                            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            {
                                mCopyByMove = true;
                            }
                        }
                        break;
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                mUIRootLocationRestore.X = mWinUIRoot.Left;
                mUIRootLocationRestore.Y = mWinUIRoot.Top;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                //mRenderScaleCenter = e.Location;
            }
        }

        private void WinDrawPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mIsMouseDragging = false;

            UpdateUIControlRectRestores(SelectedWinControls);

            mSnapXShow = false;
            mSnapYShow = false;
            //mSnapMouseLocationXRestored = false;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                switch (EditMode)
                {
                    case enEditMode.Create:
                        break;

                    case enEditMode.MoveCanvas:
                        break;

                    case enEditMode.SelectAndOperateControl:
                        {
                            if (//e.Location.X - mMouseBtnDownPoint.X < 2 &&
                                //e.Location.Y - mMouseBtnDownPoint.Y < 2 &&
                               mMouseMoved == false &&
                               (mSelectedControlRectType == enControlRectType.None || mSelectedControlRectType == enControlRectType.Container))
                            {
                                // 选择
                                //bool selectionChanged = false;
                                var mpt = GetScreenPointToRenderPoint(e.Location);

                                var selCtrl = CheckSelectUI(mpt, mWinRootForm);
                                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
                                {
                                    if (selCtrl != null && !(selCtrl is UISystem.WinRoot))
                                    {
                                        var availWin = WinBase.GetAvailableUIWin(selCtrl);
                                        SelectedWinControls.Add(WinBase.GetHostWin(availWin));
                                        //selectionChanged = true;
                                    }
                                }
                                else
                                {
                                    SelectedWinControls.Clear();
                                    if (selCtrl != null && !(selCtrl is UISystem.WinRoot))
                                    {
                                        var availWin = WinBase.GetAvailableUIWin(selCtrl);
                                        SelectedWinControls.Add(WinBase.GetHostWin(availWin));
                                    }
                                    //selectionChanged = true;
                                }

                                //foreach (var uiControl in mAllWinControls)
                                //{
                                //    if (mSelectedWinControls.Contains(uiControl))
                                //        continue;

                                //    var mpt = GetScreenPointToRenderPoint(e.Location);
                                //    //mMousePointRect.X = mpt.X;
                                //    //mMousePointRect.Y = mpt.Y;
                                //    var rect = new System.Drawing.Rectangle(uiControl.Left, uiControl.Top, uiControl.Width, uiControl.Height);
                                //    rect = GetControlContainerUIRootOffsetRect(rect);
                                //    if (rect.Contains(mpt))
                                //    {
                                //        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                                //        {
                                //            mSelectedWinControls.Add(uiControl);
                                //            selectionChanged = true;
                                //        }
                                //        else
                                //        {
                                //            mSelectedWinControls.Clear();
                                //            mSelectedWinControls.Add(uiControl);
                                //            selectionChanged = true;
                                //        }
                                //    }
                                //}

                                //if (selectionChanged)
                                //{
                                //    UpdateUIControlsSelection(SelectedWinControls);
                                //}
                            }
                        }
                        break;
                }
            }
        }
        */

        #endregion


    }
}