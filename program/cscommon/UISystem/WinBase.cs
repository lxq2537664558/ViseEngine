using System;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Linq;
using System.ComponentModel;

namespace UISystem
{
	[System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
	public partial class WinBase : UIBindAutoUpdate, UIInterface, INotifyPropertyChanged
	{
		public delegate void Delegate_OnPropertyChanged(WinBase control, string propertyName);
		public event Delegate_OnPropertyChanged OnPropertyChangedEvent;
		public void RemoveEventWithPropertyChangedEventWithChild(Delegate_OnPropertyChanged handle)
		{
			OnPropertyChangedEvent -= handle;

			foreach (var child in ChildWindows)
			{
				child.RemoveEventWithPropertyChangedEventWithChild(handle);
			}
		}
		public void AddEventWithPropertyChangedEventWithChild(Delegate_OnPropertyChanged handle)
		{
			OnPropertyChangedEvent += handle;

			foreach (var child in ChildWindows)
			{
				child.AddEventWithPropertyChangedEventWithChild(handle);
			}
		}

		#region INotifyPropertyChangedMembers
		public event PropertyChangedEventHandler PropertyChanged;
		protected override void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}

			if (OnPropertyChangedEvent != null)
				OnPropertyChangedEvent(this, propertyName);

			base.OnPropertyChanged(propertyName);

		}
		//protected void OnPropertyChanged(string propertyName, object oldValue, object newValue)
		//{


		//    if(!object.Equals(oldValue, newValue))
		//        UpdateBindValue(propertyName);


		//}
		#endregion

		public enum enContainerType
		{
			None,       // 不允许有子
			One,        // 只有一个子
			Multi,      // 允许有多个子 
		}
		private enContainerType mContainerType = enContainerType.None;
		[Browsable(false)]
		public enContainerType ContainerType
		{
			get { return mContainerType; }
			set
			{
				mContainerType = value;
			}
		}

		public virtual bool CanInsertChild()
		{
			if(ContainerType != UISystem.WinBase.enContainerType.None &&
			   !(ContainerType == UISystem.WinBase.enContainerType.One && GetChildWinCount() > 0))
			{
				return true;
			}

			return false;
		}

		protected SlimDX.Matrix mTransMatrix = SlimDX.Matrix.Identity;
		[Browsable(false)]
		public SlimDX.Matrix TransMatrix
		{
			get { return mTransMatrix; }
			protected set { mTransMatrix = value; }
		}

		protected float mRotation = 0;
		[Category("变换"), DisplayName("旋转")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public float Rotation
		{
			get { return mRotation; }
			set
			{
				mRotation = value;
				UpdateTransMatrix();
			}
		}

		protected float mTransCenterX = 0.5f;
		[Category("变换"), DisplayName("中心X")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public float TransCenterX
		{
			get { return mTransCenterX; }
			set 
			{ 
				mTransCenterX = value;
				UpdateTransMatrix();
			}
		}
		protected float mTransCenterY = 0.5f;
		[Category("变换"), DisplayName("中心Y")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public float TransCenterY
		{
			get { return mTransCenterY; }
			set
			{ 
				mTransCenterY = value;
				UpdateTransMatrix();
			}
		}

		protected void UpdateTransMatrix()
		{
			WinRoot root = GetRoot() as WinRoot;
			if (root == null)
				return;

			//var transCenter = new SlimDX.Vector3(((this.AbsRect.X + this.AbsRect.Width * TransCenterX) - root.Width * 0.5f),// * 2.0f / root.Width,
			//                                     (root.Height * 0.5f - (this.AbsRect.Y + this.AbsRect.Height * TransCenterY)),// * 2.0f / root.Height,
			//                                     0);
			var transCenter = new SlimDX.Vector3((this.AbsRect.X + this.AbsRect.Width * TransCenterX),// * 2.0f / root.Width,
												 (this.AbsRect.Y + this.AbsRect.Height * TransCenterY),// * 2.0f / root.Height,
												 0);

			//var mat = SlimDX.Matrix.Translation(-transCenter);
			//mat *= SlimDX.Matrix.RotationZ((float)(Rotation / 180.0f * System.Math.PI));
			//mTransMatrix = mat * SlimDX.Matrix.Translation(transCenter);

			//mTransMatrix = SlimDX.Matrix.RotationZ((float)(Rotation / 180.0f * System.Math.PI));

			mTransMatrix = SlimDX.Matrix.Transformation(transCenter, SlimDX.Quaternion.Identity, new SlimDX.Vector3(ScaleX, ScaleY, 1),
										 transCenter,
										 SlimDX.Quaternion.RotationAxis(SlimDX.Vector3.UnitZ, (float)(Rotation / 180.0f * System.Math.PI)),
										 SlimDX.Vector3.Zero);
			//mTransMatrix = SlimDX.Matrix.Transformation2D(SlimDX.Vector2.Zero, 0, SlimDX.Vector2.UnitXY,
			//                                            new SlimDX.Vector2(TransCenterX, TransCenterY),
			//                                            (float)(Rotation / 180.0f * System.Math.PI),
			//                                            SlimDX.Vector2.Zero);
		}

		public WinBase()
		{
			//mVisible = true;
			//mDockMode = System.Windows.Forms.DockStyle.None;
			mForeColor = CSUtility.Support.Color.FromArgb(255, 255, 255, 255);

			//mForceChildMsg = false;

			NeverMeasured = true;
			NeverArranged = true;

			mSize.Width = 1024;
			mSize.Height = 768;

			ChildWindows.CollectionChanged += ChildWindows_CollectionChanged;
			LogicChildren.CollectionChanged += LogicChildren_CollectionChanged;

			InitializeBehaviorProcesses();

			RegisterDefaultValue();
		}

		public void MoveToTop()
		{
			var oldPar = this.Parent;
			this.Parent = null;
			this.Parent = oldPar;
		}

		public WinBase FindControlInParentForm(string controlName)
		{
			var form = GetRoot(typeof(WinForm)) as WinBase;
			return form.FindControl(controlName);
		}

		public WinBase FindControlInParentForm(Guid id)
		{
			var form = GetRoot(typeof(WinForm)) as WinBase;
			return form.FindControl(id);
		}

		public WinBase FindControl(string controlName)
		{
			if(this.WinName == controlName)
				return this;

			foreach (var child in mChildWindows)
			{
				var ctrl = child.FindControl(controlName);
				if (ctrl != null)
					return ctrl;
			}

			return null;
		}

		static int iIndex = 0;
		public WinBase FindControl(string ControlType, int Number)
		{
			iIndex = Number;
			if (this.GetType().Name == ControlType) 
			{
				if (1 == iIndex)
					return this;
				else
					--iIndex;
			}
			foreach (var child in mChildWindows)
			{
				var ctrl = child.FindControl(ControlType, iIndex);
				if (ctrl != null)
					return ctrl;
			}
			return null;
		}

		public WinBase FindControl(Guid id)
		{
			if ((IsTemplateControl || TemplateId != Guid.Empty) && this.CopyedFromId == id)
			{
				return this;
			}
			else if (this.Id == id)
				return this;

			foreach (var child in mChildWindows)
			{
				var ctrl = child.FindControl(id);
				if (ctrl != null)
					return ctrl;
			}

			return null;
		}

		public WinBase FindControl(Type type, bool findBaseType = false)
		{
			if(this.GetType() == type)
				return this;

			if (findBaseType)
			{
				var baseType = this.GetType().BaseType;
				while (baseType != null)
				{
					if(baseType == type)
						return this;

					baseType = baseType.BaseType;
				}
			}

			foreach (var child in mChildWindows)
			{
				var ctrl = child.FindControl(type, findBaseType);
				if (ctrl != null)
					return ctrl;
			}

			return null;
		}

#region 默认值

		protected DefaultValueTemplate mDefaultValueTemplate = new DefaultValueTemplate();
		[Browsable(false)]
		public DefaultValueTemplate DefaultValueTemplate
		{
			get { return mDefaultValueTemplate; }
		}
		protected virtual void RegisterDefaultValue()
		{
			//mDefaultValueTemplate.RegisterDefaultValue("WinName", "");
			//mDefaultValueTemplate.RegisterDefaultValue("DragEnable", false);
			//mDefaultValueTemplate.RegisterDefaultValue("Width", (int)0);
			//mDefaultValueTemplate.RegisterDefaultValue("Width_Auto", false);
			//mDefaultValueTemplate.RegisterDefaultValue("MinWidth", (int)0);
			//mDefaultValueTemplate.RegisterDefaultValue("MaxWidth", int.MaxValue);
			//mDefaultValueTemplate.RegisterDefaultValue("Height", (int)0);
			//mDefaultValueTemplate.RegisterDefaultValue("Height_Auto", false);
			//mDefaultValueTemplate.RegisterDefaultValue("MinHeight", (int)0);
			//mDefaultValueTemplate.RegisterDefaultValue("MaxHeight", int.MaxValue);
			//mDefaultValueTemplate.RegisterDefaultValue("GridColumn", (UInt16)0);
			//mDefaultValueTemplate.RegisterDefaultValue("GridColumnSpan", (UInt16)1);
			//mDefaultValueTemplate.RegisterDefaultValue("GridRow", (UInt16)0);
			//mDefaultValueTemplate.RegisterDefaultValue("GridRowSpan", (UInt16)1);
			//mDefaultValueTemplate.RegisterDefaultValue("HorizontalAlignment", UI.HorizontalAlignment.Left);
			//mDefaultValueTemplate.RegisterDefaultValue("VerticalAlignment", UI.VerticalAlignment.Top);
			//mDefaultValueTemplate.RegisterDefaultValue("Margin", new CSCommon.Support.Thickness(0));
			//mDefaultValueTemplate.RegisterDefaultValue("ForeColor", CSUtility.Support.Color.Black);
			//mDefaultValueTemplate.RegisterDefaultValue("BackColor", CSUtility.Support.Color.LightGray);
			//mDefaultValueTemplate.RegisterDefaultValue("Font", null);
			//mDefaultValueTemplate.RegisterDefaultValue("ScaleX", 1.0f);
			//mDefaultValueTemplate.RegisterDefaultValue("ScaleY", 1.0f);
			//mDefaultValueTemplate.RegisterDefaultValue("ScaleCenter", CSUtility.Support.Point.Empty);
			//mDefaultValueTemplate.RegisterDefaultValue("Visibility", Visibility.Visible);
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
			{
				mDefaultValueTemplate.RegisterDefaultValue(property.Name, property.GetValue(this));
			}
		}

#endregion

		public UIInterface GetRoot(Type rootType = null)
		{
			if (mParent != null && mParent.GetType() == rootType)
				return mParent;
			if( mParent==null )
				return this;
			return mParent.GetRoot(rootType);
		}

		public List<UIInterface> GetAllChildControls(bool bExceptTemplateControl = true)
		{
			List<UIInterface> retList = new List<UIInterface>();
			foreach (var child in ChildWindows)
			{
				if (!(child.IsTemplateControl && bExceptTemplateControl))
				{
					retList.Add(child);
				}

				retList.AddRange(child.GetAllChildControls(bExceptTemplateControl));
			}

			return retList;
		}

        #region  Mobile
        public bool mIsOnFocus = false;
        #endregion

        #region Props

        protected string mWinName = "";
		//[Category("杂项")]
		//[CSUtility.Editor.UIEditor_DefaultValue("")]
		[Browsable(false)]
		public string WinName
		{
			get { return mWinName; }
			set
			{
				string tempName = value;

				//if (!string.IsNullOrEmpty(tempName) && LoadFinished)
				//{
				//    // 查找是否有同名控件，避免重名
				//    var form = GetRoot(typeof(WinForm));
				//    var ctrl = form.FindControl(tempName);
				//    int i=1;
				//    while (ctrl != null)
				//    {
				//        tempName = value + i++;
				//        ctrl = form.FindControl(tempName);
				//    }
				//}

				mWinName = tempName;
				NameInEditor = mWinName;
				OnPropertyChanged("WinName");
			}
		}
		protected WinBase mParent;
		[Browsable(false)]
		public UIInterface Parent
		{
			get { return mParent; }
			set
			{
				//System.Diagnostics.Debug.Assert(value!=null);
				if (mParent == value)
					return;

				if (mParent != null)
				{
					//mParent.mChildWindows.Remove(this);
					mParent.RemoveChild(this);
				}
				mParent = value as WinBase;
				if (mParent != null )
				{
					TreeLevel = mParent.TreeLevel + 1;

					//mParent.mChildWindows.Add(this);
					////var tempLeft = mAbsRect.Left - mParent.AbsRect.Left;
					////var tempTop = mAbsRect.Top - mParent.AbsRect.Top;
					////var left = tempLeft;
					////var top = tempTop;
					////Margin = GetMargin(left, top, Width, Height, mParent);
					mParent.OnAddChild(this);

					OnSetParent(mParent);
				}

				UpdateClipRect();
			}
		}

		protected virtual void OnSetParent(WinBase parent) { }

		// 添加子不要直接调用此函数，请使用Parent=XXX
		protected virtual void OnAddChild(WinBase child)
		{
			this.mChildWindows.Add(child);
			CalculateTabIndexMaxMinValue();
		}

		protected virtual void RemoveChild(WinBase child)
		{
			this.mChildWindows.Remove(child);
			CalculateTabIndexMaxMinValue();
		}

		//protected bool mVisible;
		//[Category("杂项")]
		//[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		//public bool Visible
		//{
		//    get { return mVisible; }
		//    set 
		//    { 
		//        mVisible = value; 
		//        if (WinVisbleChanged!=null) 
		//            WinVisbleChanged(mVisible);

		//        OnPropertyChanged("Visible");
		//    }
		//}
		protected bool mIgnoreSaver;
		[Browsable(false)]
		public bool IgnoreSaver
		{
			get { return mIgnoreSaver; }
			set
			{
				mIgnoreSaver = value;

				OnPropertyChanged("IgnoreSaver");
			}
		}
		protected bool mDragEnable = false;
		//[CSUtility.Editor.UIEditor_DefaultValue(false)]
		[Category("行为")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public bool DragEnable
		{
			get { return mDragEnable; }
			set
			{
				mDragEnable = value;

				OnPropertyChanged("DragEnable");
			}
		}

		protected bool mLockingX = false;
		//[CSUtility.Editor.UIEditor_DefaultValue(false)]
		[Category("行为")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public bool LockingX    //Cavas有效，锁定x坐标不能移动
		{
			get { return mLockingX; }
			set
			{
				mLockingX = value;

				OnPropertyChanged("LockingX");
			}
		}

		protected bool mLockingY = false;
		//[CSUtility.Editor.UIEditor_DefaultValue(false)]
		[Category("行为")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public bool LockingY    //Cavas有效，锁定y坐标不能移动
		{
			get { return mLockingY; }
			set
			{
				mLockingY = value;

				OnPropertyChanged("LockingY");
			}
		}

		protected bool mAstrictX = false;
		//[CSUtility.Editor.UIEditor_DefaultValue(false)]
		[Category("行为")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public bool AstrictX    //Cavas有效，限制x坐标不能大于0，小于该控件宽度和cavas的宽度的差值
		{
			get { return mAstrictX; }
			set
			{
				mAstrictX = value;

				OnPropertyChanged("AstrictX");
			}
		}

		protected bool mAstrictY = false;
		//[CSUtility.Editor.UIEditor_DefaultValue(false)]
		[Category("行为")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public bool AstrictY    //Cavas有效，限制y坐标不能大于0，小于该控件高度和cavas的高度的差值
		{
			get { return mAstrictY; }
			set
			{
				mAstrictY = value;

				OnPropertyChanged("AstrictY");
			}
		}

		protected CSUtility.Support.Point mLocation = CSUtility.Support.Point.Empty;
		//[Category("布局")]
		//[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		[Browsable(false)]
		public int Left
		{
			get { return mLocation.X; }
			protected set
			{
				if (mLocation.X == value)
					return;

				mLocation.X = value;
				UpdateLayout();
				//MoveToWin(ref mLocation);

				//OnPropertyChanged("Left");
			}
		}
		[Browsable(false)]
		//[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public int Top
		{
			get { return mLocation.Y; }
			protected set
			{
				if (mLocation.Y == value)
					return;

				mLocation.Y = value;
				UpdateLayout();
				//MoveToWin(ref mLocation);

				//OnPropertyChanged("Top");
			}
		}
		[Browsable(false)]
		public int Right
		{
			get { return mLocation.X + Width; }
		}
		[Browsable(false)]
		public int Bottom
		{
			get { return mLocation.Y + Height; }
		}
		protected CSUtility.Support.Size mSize = CSUtility.Support.Size.Empty;
		[Category("布局")]
		//[CSUtility.Editor.UIEditor_DefaultValue(0)]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		[CSUtility.Editor.UIEditor_PropertysWithAutoSet]
		public int Width
		{
			get { return mSize.Width; }
			set
			{
				if (mSize.Width == value)
					return;

				mSize.Width = value;// System.Math.Max(System.Math.Min(value, MaxWidth), MinWidth);
				//UpdateAbsRect();
				//if (mParent != null)
				//    mParent.DockChildWin();
				//else
				//    DockChildWin();
			//    UpdateHorizontalArrangement();
				if(!Width_Auto)
				{
					if(Parent != null)
					{
						var p = Parent as WinBase;
						p.OnChildDesiredSizeChanged(this);
					}
					UpdateLayout();
				}

				WinSizeChanged?.Invoke(mSize.Width, mSize.Height, this);

				OnPropertyChanged("Width");
			}
		}
		protected bool mWidth_Auto = false;
		[Browsable(false)]
		//[CSUtility.Editor.UIEditor_DefaultValue(false)]
		public bool Width_Auto
		{
			get { return mWidth_Auto; }
			set
			{
				mWidth_Auto = value;

				//UpdateWidthFromChildren();
				UpdateLayout();

				OnPropertyChanged("Width_Auto");
			}
		}

		protected int mMinWidth = 0;
		[Category("布局")]
		//[CSUtility.Editor.UIEditor_DefaultValue(0)]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public int MinWidth
		{
			get { return mMinWidth; }
			set
			{
				mMinWidth = value;

				OnPropertyChanged("MinWidth");
			}
		}

		protected int mMaxWidth = int.MaxValue;
		[Category("布局")]
		//[CSUtility.Editor.UIEditor_DefaultValue(int.MaxValue)]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public int MaxWidth
		{
			get { return mMaxWidth; }
			set
			{
				mMaxWidth = value;

				OnPropertyChanged("MaxWidth");
			}
		}

		[Category("布局")]
		//[CSUtility.Editor.UIEditor_DefaultValue(0)]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		[CSUtility.Editor.UIEditor_PropertysWithAutoSet]
		public int Height
		{
			get { return mSize.Height; }
			set
			{
				if (mSize.Height == value)
					return;

				mSize.Height = value;// System.Math.Max(System.Math.Min(value, MaxHeight), MinHeight); ;
				//UpdateAbsRect();
				//if (mParent != null)
				//    mParent.DockChildWin();
				//else
				//    DockChildWin();
			//    UpdateVerticalArrangement();
				if(!Height_Auto)
				{
					if (Parent != null)
					{
						var p = Parent as WinBase;
						p.OnChildDesiredSizeChanged(this);
					}
					UpdateLayout();
				}

				WinSizeChanged?.Invoke(mSize.Width, mSize.Height, this);

				OnPropertyChanged("Height");
			}
		}

		protected bool mHeight_Auto = false;
		//[CSUtility.Editor.UIEditor_DefaultValue(false)]
		[Browsable(false)]        
		public bool Height_Auto
		{
			get { return mHeight_Auto; }
			set
			{
				mHeight_Auto = value;

				//UpdateHeightFromChildren();
				UpdateLayout();

				OnPropertyChanged("Height_Auto");
			}
		}

		protected int mMinHeight = 0;
		//[CSUtility.Editor.UIEditor_DefaultValue(0)]
		[Category("布局")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public int MinHeight
		{
			get { return mMinHeight; }
			set
			{
				mMinHeight = value;

				OnPropertyChanged("MinHeight");
			}
		}

		protected int mMaxHeight = int.MaxValue;
		//[CSUtility.Editor.UIEditor_DefaultValue(int.MaxValue)]
		[Category("布局")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public int MaxHeight
		{
			get { return mMaxHeight; }
			set
			{
				mMaxHeight = value;

				OnPropertyChanged("MaxHeight");
			}
		}

		//protected System.Windows.Forms.DockStyle mDockMode;
		//[Category("布局")]
		//[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		//public System.Windows.Forms.DockStyle DockMode
		//{
		//    get { return mDockMode; }
		//    set
		//    {
		//        mDockMode = value;
		//        if (mParent!=null)
		//            Parent.DockChildWin();

		//        OnPropertyChanged("DockMode");
		//    }
		//}

#region for Grid

		protected UInt16 mGridColumn = 0;
		[Category("布局")]
		//[CSUtility.Editor.UIEditor_DefaultValue((UInt16)0)]
		[CSUtility.Editor.UIEditor_WhenWinBaseParentIsTypeShow(new Type[] { typeof(UISystem.Container.Grid.Grid) })]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public UInt16 GridColumn
		{
			get { return mGridColumn; }
			set
			{
				mGridColumn = value;

				if (Parent is UISystem.Container.Grid.Grid)
				{
					var p = Parent as UISystem.Container.Grid.Grid;
					p.CellsStructureDirty = true;
					p.OnChildDesiredSizeChanged(this);

					UpdateLayout();
				}

				OnPropertyChanged("GridColumn");
			}
		}

		protected UInt16 mGridColumnSpan = 1;
		[Category("布局")]
		//[CSUtility.Editor.UIEditor_DefaultValue((UInt16)1)]
		[CSUtility.Editor.UIEditor_WhenWinBaseParentIsTypeShow(new Type[] { typeof(UISystem.Container.Grid.Grid) })]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public UInt16 GridColumnSpan
		{
			get { return mGridColumnSpan; }
			set
			{
				mGridColumnSpan = value;
				if (Parent is UISystem.Container.Grid.Grid)
				{
					var p = Parent as UISystem.Container.Grid.Grid;
					p.CellsStructureDirty = true;
					p.OnChildDesiredSizeChanged(this);
					UpdateLayout();
				}
				OnPropertyChanged("GridColumnSpan");
			}
		}

		protected UInt16 mGridRow = 0;
		[Category("布局")]
		//[CSUtility.Editor.UIEditor_DefaultValue((UInt16)0)]
		[CSUtility.Editor.UIEditor_WhenWinBaseParentIsTypeShow(new Type[] { typeof(UISystem.Container.Grid.Grid) })]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public UInt16 GridRow
		{
			get { return mGridRow; }
			set
			{
				mGridRow = value;
				if (Parent is UISystem.Container.Grid.Grid)
				{
					var p = Parent as UISystem.Container.Grid.Grid;
					p.CellsStructureDirty = true;
					p.OnChildDesiredSizeChanged(this);

					UpdateLayout();
				}
				OnPropertyChanged("GridRow");
			}
		}

		protected UInt16 mGridRowSpan = 1;
		[Category("布局")]
		//[CSUtility.Editor.UIEditor_DefaultValue((UInt16)1)]
		[CSUtility.Editor.UIEditor_WhenWinBaseParentIsTypeShow(new Type[] { typeof(UISystem.Container.Grid.Grid) })]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public UInt16 GridRowSpan
		{
			get { return mGridRowSpan; }
			set
			{
				mGridRowSpan = value;
				if (Parent is UISystem.Container.Grid.Grid)
				{
					var p = Parent as UISystem.Container.Grid.Grid;
					p.CellsStructureDirty = true;
					p.OnChildDesiredSizeChanged(this);
					UpdateLayout();
				}
				OnPropertyChanged("GridRowSpan");
			}
		}

#endregion


		public List<UI.HorizontalAlignment> LockedHorizontals = new List<UI.HorizontalAlignment>();
		public List<UI.VerticalAlignment> LockedVerticals = new List<UI.VerticalAlignment>();

		protected UI.HorizontalAlignment mHorizontalAlignment = UI.HorizontalAlignment.Left;
		//[CSUtility.Editor.UIEditor_DefaultValue(UI.HorizontalAlignment.Left)]
		[Category("布局")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public UI.HorizontalAlignment HorizontalAlignment
		{
			get { return mHorizontalAlignment; }
			set
			{
				if (LockedHorizontals.Count > 0 && !LockedHorizontals.Contains(value))
					return;

				if (mHorizontalAlignment == value)
					return;

				mHorizontalAlignment = value;

				//Margin = GetMargin(Left, Top, Width, Height, this.Parent);
				UpdateLayout();

				OnPropertyChanged("HorizontalAlignment");
			}
		}

		protected UI.VerticalAlignment mVerticalAlignment = UI.VerticalAlignment.Top;
		//[CSUtility.Editor.UIEditor_DefaultValue(UI.VerticalAlignment.Top)]
		[Category("布局")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public UI.VerticalAlignment VerticalAlignment
		{
			get { return mVerticalAlignment; }
			set
			{
				if (LockedVerticals.Count > 0 && !LockedVerticals.Contains(value))
					return;

				if (mVerticalAlignment == value)
					return;

				mVerticalAlignment = value;

				//Margin = GetMargin(Left, Top, Width, Height, this.Parent);
				UpdateLayout();

				OnPropertyChanged("VerticalAlignment");
			}
		}

		//protected static CSCommon.Support.Thickness DefaultMargin = new CSCommon.Support.Thickness(0);
		protected CSUtility.Support.Thickness mMargin = new CSUtility.Support.Thickness();
		//[CSUtility.Editor.UIEditor_DefaultValue(null)]
		[Category("布局")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public CSUtility.Support.Thickness Margin
		{
			get { return mMargin; }
			set
			{
				if (mMargin == value)
					return;

				mMargin = value;

				//int left = Left;
				//int top = Top;
				//int width = Width;
				//int height = Height;
				//GetLocationAndSize(ref left, ref top, ref width, ref height);
				//Left = left;
				//Top = top;
				//Width = width;
				//Height = height;
				UpdateLayout();

				OnPropertyChanged("Margin");
			}
		}

		Visibility mTargetVisibility = Visibility.Visible;
		//bool mNeedChangeVisibility = false;
		void ChangeVisibility()
		{
			var oldVisibility = mVisibility;
			mVisibility = mTargetVisibility;

			if (WinVisibilityChanged != null)
				WinVisibilityChanged(mVisibility);

			if ((mVisibility == Visibility.Collapsed && oldVisibility == Visibility.Visible) ||
			   (mVisibility == Visibility.Collapsed && oldVisibility == Visibility.Hidden) ||
			   (mVisibility == Visibility.Visible && oldVisibility == Visibility.Collapsed) ||
			   (mVisibility == Visibility.Hidden && oldVisibility == Visibility.Collapsed))
			{
				if(Parent != null)
				{
					var p = Parent as WinBase;
					p.OnChildDesiredSizeChanged(this);
				}
				UpdateLayout();
			}

			//mNeedChangeVisibility = false;
		}
		protected Visibility mVisibility = Visibility.Visible;
		//[CSUtility.Editor.UIEditor_DefaultValue(Visibility.Visible)]
		[Category("布局")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public Visibility Visibility
		{
			get { return mTargetVisibility;}// mVisibility; }
			set
			{
				var oldVisibility = mTargetVisibility;
				//if (mVisibility == value)
				//    return;
				if (mTargetVisibility == value)
					return;

				mTargetVisibility = value;
				//mNeedChangeVisibility = true;

				ChangeVisibility();
				//UpdateLayout();

				OnPropertyChanged("Visibility");
			}
		}

		protected CSUtility.Support.Rectangle mAbsRect;
		[Browsable(false)]
		public CSUtility.Support.Rectangle AbsRect
		{
			get { return mAbsRect; }
		}
		protected CSUtility.Support.Rectangle mClipRect;
		[Browsable(false)]
		public CSUtility.Support.Rectangle ClipRect
		{
			get { return mClipRect; }
		}

		protected float mOpacity = 1.0f;
		[Category("外观")]
		[CSUtility.Editor.Editor_ValueWithRange(0, 1)]
		public float Opacity
		{
			get { return mOpacity; }
			set
			{
				mOpacity = value;
				OnPropertyChanged("Opacity");
			}
		}

		protected CSUtility.Support.Color mForeColor = CSUtility.Support.Color.Black;
		//[CSUtility.Editor.UIEditor_DefaultValue(null)]
		[Category("外观")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public CSUtility.Support.Color ForeColor
		{
			get { return mForeColor; }
			set
			{
				mForeColor = value;

				OnPropertyChanged("ForeColor");
			}
		}
		protected CSUtility.Support.Color mBackColor = CSUtility.Support.Color.FromArgb(0, CSUtility.Support.Color.LightGray);
		//[CSUtility.Editor.UIEditor_DefaultValue(null)]
		[Category("外观")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public CSUtility.Support.Color BackColor
		{
			get { return mBackColor; }
			set
			{
				mBackColor = value;
				mBackColorVertex = new SlimDX.Vector4(mBackColor.R / 255.0f, mBackColor.G / 255.0f, mBackColor.B / 255.0f, mBackColor.A / 255.0f);
				OnPropertyChanged("BackColor");
			}
		}
		protected SlimDX.Vector4 mBackColorVertex;
		//protected System.Drawing.Font mFont = null;
		////[CSUtility.Editor.UIEditor_DefaultValue(null)]
		//[Category("外观")]
		//[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		//public System.Drawing.Font Font
		//{
		//    get
		//    {
		//        if (mFont != null)
		//        {
		//            return mFont;
		//        }
		//        if (Parent == null)
		//        {
		//            return null;
		//        }
		//        return Parent.Font;
		//    }
		//    set
		//    {
		//        if (mFont != value)
		//        {
		//            IRender.GetInstance().WhenUISetFont(mFont);
		//        }

		//        mFont = value;

		//        OnPropertyChanged("Font");
		//    }
		//}
		protected object mTag;
		[Category("杂项")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public object Tag
		{
			get { return mTag; }
			set { mTag = value; }
		}
		protected WinState mWinState;
		[Browsable(false)]
		public WinState RState
		{
			get { return mWinState; }
			set 
			{
				if (value == null)
					return;
				mWinState = value; 
			}
		}

		protected float mScaleX = 1;
		//[CSUtility.Editor.UIEditor_DefaultValue(1.0f)]
		[Category("变换")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public float ScaleX
		{
			get { return mScaleX; }
			set
			{
				mScaleX = value;
				UpdateTransMatrix();
				OnPropertyChanged("ScaleX");
			}
		}

		protected float mScaleY = 1;
		//[CSUtility.Editor.UIEditor_DefaultValue(1.0f)]
		[Category("变换")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public float ScaleY
		{
			get { return mScaleY; }
			set
			{
				mScaleY = value;
				UpdateTransMatrix();
				OnPropertyChanged("ScaleY");
			}
		}

		//protected CSUtility.Support.Point mScaleCenter = CSUtility.Support.Point.Empty;
		////[CSUtility.Editor.UIEditor_DefaultValue(null)]
		//[Category("变换")]
		//[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		//public CSUtility.Support.Point ScaleCenter
		//{
		//    get { return mScaleCenter; }
		//    set
		//    {
		//        mScaleCenter = value;
		//        OnPropertyChanged("ScaleCenter");
		//    }
		//}

		protected bool mCanHaveChildren = true;
		[Browsable(false)]
		public bool CanHaveChildren
		{
			get { return mCanHaveChildren; }
			set
			{
				mCanHaveChildren = value;
				OnPropertyChanged("CanHaveChildren");
			}
		}

		protected bool mHitTestVisible = true;
		[Category("杂项")]
		[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public bool HitTestVisible
		{
			get { return mHitTestVisible; }
			set
			{
				mHitTestVisible = value;
				OnPropertyChanged("HitTestVisible");
			}
		}
		#endregion
		
		#region Save, Load and Copy

		bool mLoadFinished = true;
		protected bool LoadFinished
		{
			get { return mLoadFinished; }
			set
			{
				mLoadFinished = value;
			}
		}

		public void Save(CSUtility.Support.XmlNode pXml,CSUtility.Support.XmlHolder holder)
		{
			BeforeSave(pXml);

			var att = pXml.AddAttrib("TypeName", this.GetType().ToString());
			OnSave(pXml,holder);
			SaveEventBindInfo(pXml,holder);
			SavePropertyBindInfo(pXml,holder);
			SaveCommandBindInfo(pXml,holder);
		
			foreach( WinBase i in mChildWindows )
			{
				if(i.IgnoreSaver)
					continue;

				CSUtility.Support.XmlNode pChildXml = pXml.AddNode(i.GetModuleName(), i.GetType().FullName, holder);
				i.Save(pChildXml, holder);
			}

			AfterSave(pXml);
		}
		public void Load(CSUtility.Support.XmlNode pXml)
		{
			try
			{
				LoadFinished = false;

				BeforeLoad(pXml);

				OnLoad(pXml);
				LoadEventBindInfo(pXml);
				LoadPropertyBindInfo(pXml);
				LoadCommandBindInfo(pXml);

				List<CSUtility.Support.XmlNode> children = pXml.GetNodes();
				foreach (CSUtility.Support.XmlNode i in children)
				{
					var assembly = CSUtility.Program.GetAnalyseAssembly(CSUtility.Helper.enCSType.All, CSUtility.Program.CurrentPlatform, i.Name);
					if(assembly != null)
					{
						WinBase pChildWin = (WinBase)(assembly.CreateInstance(i.FindAttrib("TypeName").Value));
						if (pChildWin != null)
						{
							pChildWin.Parent = this;
							pChildWin.Load(i);
						}
					}
				}

				LoadFinished = true;
				AfterLoad(pXml);

				CalculateTabIndexMaxMinValue();

				//LayoutDirty = true;
				UpdateLayout(true);
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}
		}

		static Dictionary<string, System.Reflection.Assembly> mAssemblyDic = new Dictionary<string, System.Reflection.Assembly>();
		public static System.Reflection.Assembly GetAssembly(string fullDllName)
		{
			System.Reflection.Assembly retAssembly;
			if (mAssemblyDic.TryGetValue(fullDllName, out retAssembly))
			{
				return retAssembly;
			}

			retAssembly = System.Reflection.Assembly.LoadFile(fullDllName);
			mAssemblyDic[fullDllName] = retAssembly;

			return retAssembly;
		}

		public UIInterface Clone()
		{
			var type = this.GetType();
			var retForm = type.Assembly.CreateInstance(type.FullName) as WinBase;

			retForm.CopyFrom(this, new List<string>(), true, true);
			return retForm;
		}

		public static WinBase CreateFromXml(string name)
		{
			CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.LoadXML( name );
		
			string dllKeyName = xmlHolder.RootNode.Name;//.Substring(0,xmlHolder.RootNode.Name.Length-1);
			var assembly = CSUtility.Program.GetAnalyseAssembly(CSUtility.Helper.enCSType.All, CSUtility.Program.CurrentPlatform, dllKeyName);
			if (assembly == null)
				return null;

			WinBase pChildWin = (WinBase)(assembly.CreateInstance(xmlHolder.RootNode.FindAttrib("TypeName").Value));
			if (pChildWin == null)
				return null;
			pChildWin.Load( xmlHolder.RootNode );
		
			return pChildWin;
		}
		public static WinBase CreateFromXml(Type rootType, string name)
		{
			CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.LoadXML(name);

			WinBase pChildWin = (WinBase)(System.Activator.CreateInstance(rootType));
			if (pChildWin == null)
				return null;
			pChildWin.Load(xmlHolder.RootNode);

			return pChildWin;
		}
		public static void SaveToXml(string name, WinBase win)
		{
			CSUtility.Support.XmlHolder xmlHolder = CSUtility.Support.XmlHolder.NewXMLHolder( win.GetModuleName() , win.GetType().FullName );
			win.Save( xmlHolder.RootNode , xmlHolder);
			CSUtility.Support.XmlHolder.SaveXML( name , xmlHolder, true );
		}

		protected virtual string GetModuleName()
		{
			//switch(CSUtility.Program.CurrentPlatform)
			//{
			//    case CSUtility.enPlatform.Android:
			//        return "Client.Android.dll";
			//    case CSUtility.enPlatform.IOS:
			//        break;
			//    case CSUtility.enPlatform.Windows:
			//        return "Client.Windows.dll";
			//}
			var name = CSUtility.Program.GetAnalyseAssemblyKeyName(CSUtility.Helper.enCSType.All, CSUtility.Program.CurrentPlatform, this.GetType().Assembly);
			if (string.IsNullOrEmpty(name))
				name = "cscommon";

			return name;
		}

		//protected bool IsEqualDefaultValue(string propertyName, object value)
		//{
		//    AttributeCollection attributes = TypeDescriptor.GetProperties(this)[propertyName].Attributes;
		//    DefaultValueAttribute defAtt = attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
		//    if (defAtt != null)
		//    {
		//        return object.Equals(value, defAtt.Value);
		//    }

		//    return false;
		//}

		protected virtual void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
		{
			pXml.AddAttrib("Id", Id.ToString());

			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TemplateId"))
				pXml.AddAttrib("TemplateId", TemplateId.ToString());

			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "WinName"))
				pXml.AddAttrib("WinName", WinName);

			//attr = pXml.AddAttrib( "Left" );
			//attr.Value = Left.ToString();
			//attr = pXml.AddAttrib( "Top" );
			//attr.Value = Top.ToString();
			if(!mDefaultValueTemplate.IsEqualDefaultValue(this, "Width"))
				pXml.AddAttrib("Width", Width.ToString());
			//if (!object.Equals(Width_Auto, GetDefaultValue("Width_Auto")))
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Width_Auto"))
				pXml.AddAttrib("Width_Auto", Width_Auto.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "MinWidth"))
				pXml.AddAttrib("MinWidth", MinWidth.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "MaxWidth"))
				pXml.AddAttrib("MaxWidth", MaxWidth.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Height"))
				pXml.AddAttrib("Height", Height.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Height_Auto"))
				pXml.AddAttrib("Height_Auto", Height_Auto.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "MinHeight"))
				pXml.AddAttrib("MinHeight", MinHeight.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "MaxHeight"))
				pXml.AddAttrib("MaxHeight", MaxHeight.ToString());

			//pXml.AddAttrib("Visible", Visible.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "DragEnable"))
				pXml.AddAttrib("DragEnable", DragEnable.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "LockingX"))
				pXml.AddAttrib("LockingX", LockingX.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "LockingY"))
				pXml.AddAttrib("LockingY", LockingY.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "AstrictX"))
				pXml.AddAttrib("AstrictX", AstrictX.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "AstrictY"))
				pXml.AddAttrib("AstrictY", AstrictY.ToString());


			//pXml.AddAttrib("DockMode", DockMode.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Opacity"))
				pXml.AddAttrib("Opacity", Opacity.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ForeColor"))
				pXml.AddAttrib("ForeColor", ForeColor.Name);//System.String.Format("{0},{1},{2},{3}", ForeColor.A, ForeColor.R, ForeColor.G, ForeColor.B));
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "BackColor"))
				pXml.AddAttrib("BackColor", BackColor.Name);//System.String.Format("{0},{1},{2},{3}", BackColor.A, BackColor.R, BackColor.G, BackColor.B));

			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "HorizontalAlignment"))
				pXml.AddAttrib("HorizontalAlignment", HorizontalAlignment.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "VerticalAlignment"))
				pXml.AddAttrib("VerticalAlignment", VerticalAlignment.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Margin"))
				pXml.AddAttrib("Margin", Margin.ToString());

			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Visibility"))
				pXml.AddAttrib("Visibility", Visibility.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "GridColumn"))
				pXml.AddAttrib("GridColumn", GridColumn.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "GridColumnSpan"))
				pXml.AddAttrib("GridColumnSpan", GridColumnSpan.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "GridRow"))
				pXml.AddAttrib("GridRow", GridRow.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "GridRowSpan"))
				pXml.AddAttrib("GridRowSpan", GridRowSpan.ToString());
			if(!mDefaultValueTemplate.IsEqualDefaultValue(this,  "HitTestVisible"))
				pXml.AddAttrib("HitTestVisible", HitTestVisible.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "HitThrough"))
				pXml.AddAttrib("HitThrough", HitThrough.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ScaleX"))
				pXml.AddAttrib("ScaleX", ScaleX.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "ScaleY"))
				pXml.AddAttrib("ScaleY", ScaleY.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "Rotation"))
				pXml.AddAttrib("Rotation", Rotation.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TransCenterX"))
				pXml.AddAttrib("TransCenterX", TransCenterX.ToString());
			if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "TransCenterY"))
				pXml.AddAttrib("TransCenterY", TransCenterY.ToString());
	   }
		protected virtual void OnLoad(CSUtility.Support.XmlNode pXml)
		{
			CSUtility.Support.XmlAttrib attr = pXml.FindAttrib( "Id" );
			if (attr != null)
				mGuid = CSUtility.Support.IHelper.GuidTryParse(attr.Value);

			attr = pXml.FindAttrib("TemplateId");
			if (attr != null)
				TemplateId = CSUtility.Support.IHelper.GuidTryParse(attr.Value);

			attr = pXml.FindAttrib( "WinName" );
			if(attr!=null)
				WinName = attr.Value;

			//attr = pXml.FindAttrib( "Left" );
			//if(attr!=null)
			//    Left = System.Convert.ToInt32( attr.Value );
			//attr = pXml.FindAttrib( "Top" );
			//if(attr!=null)
			//    Top = System.Convert.ToInt32( attr.Value );

			attr = pXml.FindAttrib( "Width" );
			if(attr!=null)
				Width = System.Convert.ToInt32( attr.Value );
			attr = pXml.FindAttrib("Width_Auto");
			if (attr != null)
				Width_Auto = System.Convert.ToBoolean(attr.Value);
			attr = pXml.FindAttrib("MinWidth");
			if (attr != null)
				MinWidth = System.Convert.ToInt32(attr.Value);
			attr = pXml.FindAttrib("MaxWidth");
			if (attr != null)
				MaxWidth = System.Convert.ToInt32(attr.Value);

			attr = pXml.FindAttrib( "Height" );
			if(attr!=null)
				Height = System.Convert.ToInt32( attr.Value );
			attr = pXml.FindAttrib("Height_Auto");
			if (attr != null)
				Height_Auto = System.Convert.ToBoolean(attr.Value);
			attr = pXml.FindAttrib("MinHeight");
			if (attr != null)
				MinHeight = System.Convert.ToInt32(attr.Value);
			attr = pXml.FindAttrib("MaxHeight");
			if (attr != null)
				MaxHeight = System.Convert.ToInt32(attr.Value);

			//attr = pXml.FindAttrib( "Visible" );
			//if(attr!=null)
			//    Visible = System.Convert.ToBoolean( attr.Value );

			attr = pXml.FindAttrib( "DragEnable" );
			if(attr!=null)
				DragEnable = System.Convert.ToBoolean( attr.Value );
			attr = pXml.FindAttrib("LockingX");
			if (attr != null)
				LockingX = System.Convert.ToBoolean(attr.Value);
			attr = pXml.FindAttrib("LockingY");
			if (attr != null)
				LockingY = System.Convert.ToBoolean(attr.Value);
			attr = pXml.FindAttrib("AstrictX");
			if (attr != null)
				AstrictX = System.Convert.ToBoolean(attr.Value);
			attr = pXml.FindAttrib("AstrictY");
			if (attr != null)
				AstrictY = System.Convert.ToBoolean(attr.Value);

			//attr = pXml.FindAttrib( "DockMode" );
			//if(attr!=null)
			//{
			//    if(attr.Value=="Left")
			//        DockMode = System.Windows.Forms.DockStyle.Left;
			//    else if(attr.Value=="Right")
			//        DockMode = System.Windows.Forms.DockStyle.Right;
			//    else if(attr.Value=="Top")
			//        DockMode = System.Windows.Forms.DockStyle.Top;
			//    else if(attr.Value=="Bottom")
			//        DockMode = System.Windows.Forms.DockStyle.Bottom;
			//    else if(attr.Value=="Fill")
			//        DockMode = System.Windows.Forms.DockStyle.Fill;
			//    else if(attr.Value=="None")
			//        DockMode = System.Windows.Forms.DockStyle.None;
			//}
			attr = pXml.FindAttrib("Opacity");
			if (attr != null)
				Opacity = System.Convert.ToSingle(attr.Value);

			attr = pXml.FindAttrib( "ForeColor" );
			if(attr!=null)
			{
				//int a,r,g,b;
				//string[] substrs = attr.Value.Split(',');
				//if(substrs!=null&&substrs.Length>=4)
				//{
				//    a = System.Convert.ToInt32(substrs[0]);
				//    r = System.Convert.ToInt32(substrs[1]);
				//    g = System.Convert.ToInt32(substrs[2]);
				//    b = System.Convert.ToInt32(substrs[3]);
				//    ForeColor = CSUtility.Support.Color.FromArgb(a,r,g,b);
				//}
				int result;
				var provider = System.Globalization.CultureInfo.InvariantCulture;
				if(Int32.TryParse(attr.Value, System.Globalization.NumberStyles.AllowHexSpecifier, provider, out result))
					ForeColor = CSUtility.Support.Color.FromArgb(result);
			}
			attr = pXml.FindAttrib( "BackColor" );
			if(attr!=null)
			{
				//int a,r,g,b;
				//string[] substrs = attr.Value.Split(',');
				//if(substrs!=null&&substrs.Length>=4)
				//{
				//    a = System.Convert.ToInt32(substrs[0]);
				//    r = System.Convert.ToInt32(substrs[1]);
				//    g = System.Convert.ToInt32(substrs[2]);
				//    b = System.Convert.ToInt32(substrs[3]);
				//    BackColor = CSUtility.Support.Color.FromArgb(a,r,g,b);
				//}
				//var name = attr.Value;
				int result;
				var provider = System.Globalization.CultureInfo.InvariantCulture;
				if (Int32.TryParse(attr.Value, System.Globalization.NumberStyles.AllowHexSpecifier, provider, out result))
				BackColor = CSUtility.Support.Color.FromArgb(result);
			}
			attr = pXml.FindAttrib("HorizontalAlignment");
			if(attr != null)
				HorizontalAlignment = (UI.HorizontalAlignment)System.Enum.Parse(typeof(UI.HorizontalAlignment), attr.Value);
			attr = pXml.FindAttrib("VerticalAlignment");
			if(attr != null)
				VerticalAlignment = (UI.VerticalAlignment)System.Enum.Parse(typeof(UI.VerticalAlignment), attr.Value);
			attr = pXml.FindAttrib("Margin");
			if(attr != null)
			{
				var values = attr.Value.Split(',');
				Margin = new CSUtility.Support.Thickness(
						System.Convert.ToDouble(values[0]),
						System.Convert.ToDouble(values[1]),
						System.Convert.ToDouble(values[2]),
						System.Convert.ToDouble(values[3])
					);
			}

			attr = pXml.FindAttrib("Visibility");
			if (attr != null)
			{
				Visibility = (Visibility)System.Enum.Parse(typeof(Visibility), attr.Value);
			}

			attr = pXml.FindAttrib("GridColumn");
			if (attr != null)
			{
				GridColumn = System.Convert.ToUInt16(attr.Value);
			}
			attr = pXml.FindAttrib("GridColumnSpan");
			if (attr != null)
			{
				GridColumnSpan = System.Convert.ToUInt16(attr.Value);
			}
			attr = pXml.FindAttrib("GridRow");
			if (attr != null)
			{
				GridRow = System.Convert.ToUInt16(attr.Value);
			}
			attr = pXml.FindAttrib("GridRowSpan");
			if (attr != null)
			{
				GridRowSpan = System.Convert.ToUInt16(attr.Value);
			}
			attr = pXml.FindAttrib("HitTestVisible");
			if (attr != null)
			{
				HitTestVisible = System.Convert.ToBoolean(attr.Value);
			}
			attr = pXml.FindAttrib("HitThrough");
			if (attr != null)
			{
				HitThrough = System.Convert.ToBoolean(attr.Value);
			}
			attr = pXml.FindAttrib("ScaleX");
			if (attr != null)
			{
				ScaleX = System.Convert.ToSingle(attr.Value);
			}
			attr = pXml.FindAttrib("ScaleY");
			if (attr != null)
			{
				ScaleY = System.Convert.ToSingle(attr.Value);
			}
			attr = pXml.FindAttrib("Rotation");
			if (attr != null)
			{
				Rotation = System.Convert.ToSingle(attr.Value);
			}
			attr = pXml.FindAttrib("TransCenterX");
			if (attr != null)
			{
				TransCenterX = System.Convert.ToSingle(attr.Value);
			}
			attr = pXml.FindAttrib("TransCenterY");
			if (attr != null)
			{
				TransCenterY = System.Convert.ToSingle(attr.Value);
			}
		}

		protected virtual void BeforeSave(CSUtility.Support.XmlNode pXml) { }
		protected virtual void AfterSave(CSUtility.Support.XmlNode pXml) { }
		protected virtual void BeforeLoad(CSUtility.Support.XmlNode pXml) { }
		protected virtual void AfterLoad(CSUtility.Support.XmlNode pXml) { }
		#endregion
		
		#region PointConvert
		public CSUtility.Support.Point AbsToLocal(ref CSUtility.Support.Point pt)
		{
			return new CSUtility.Support.Point(pt.X - mAbsRect.Location.X, pt.Y - mAbsRect.Location.Y);
		}
		public CSUtility.Support.Point AbsToLocal(int x, int y)
		{
			return new CSUtility.Support.Point(x - mAbsRect.Location.X, y - mAbsRect.Location.Y);
		}
		public CSUtility.Support.Point LocalToAbs(ref CSUtility.Support.Point pt)
		{
			return new CSUtility.Support.Point(pt.X + mAbsRect.Location.X, pt.Y + mAbsRect.Location.Y);
		}
		public CSUtility.Support.Point LocalToAbs(int x, int y)
		{
			return new CSUtility.Support.Point(x + mAbsRect.Location.X, y + mAbsRect.Location.Y);
		}

		public virtual WinBase StayWindow(ref CSUtility.Support.Point pt, bool ignoreHitTest)
		{
			if (mAbsRect.Contains(pt))
			{
				for (int i = mChildWindows.Count - 1; i >= 0; --i)
				{
					if (mChildWindows[i].Visibility != Visibility.Visible)
						continue;
					if (!ignoreHitTest && mChildWindows[i].HitTestVisible == false)
						continue;
					WinBase pWin = mChildWindows[i].StayWindow(ref pt, ignoreHitTest);
					if (pWin != null && !pWin.HitThrough)
						return pWin;
				}

				if (this is UISystem.Content.ContentPresenter ||
				   this is UISystem.Content.ItemsPresenter ||
				   this is UISystem.Content.ScrollContentPresenter)
					return null;

				return this;
			}
			return null;
		}
		//得到该窗体所附属的Form
		public WinForm GetWinForm(WinBase pRoot)
		{
			if (this.GetType().IsSubclassOf(typeof(WinForm)))
			{
				return (WinForm)this;
			}
			else
			{
				if (mParent != null && mParent != pRoot)
					return mParent.GetWinForm(pRoot);
				return null;
			}
		}
		#endregion

		#region Child
		
		private CSUtility.Support.ThreadSafeObservableCollection<WinBase> mChildWindows = new CSUtility.Support.ThreadSafeObservableCollection<WinBase>();
		//protected List<WinBase> mChildWindows = new List<WinBase>();
		[Browsable(false)]
		protected CSUtility.Support.ThreadSafeObservableCollection<WinBase> ChildWindows
		//protected List<WinBase> ChildWindows
		{
			get { return mChildWindows; }
			set
			{
				if (value == null)
				{
					mChildWindows.Clear();
					return;
				}

				mChildWindows.Clear();
				foreach (var child in value)
				{
					mChildWindows.Add(child);
				}
			}
		}

		protected virtual void ChildWindows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (WinBase item in e.NewItems)
					{
						item.EnableEditorMouseMove = true;
						//item.MouseMoveAbleInEditor = true;
						item.EnableHorizontalArrangementLineShow = true;
						item.EnableVerticalArrangementLineShow = true;
						item.LockedHorizontals.Clear();
						item.LockedVerticals.Clear();

						if(!item.IsTemplateControl)
							LogicChildren.Add(item);
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					{
						foreach (WinBase item in e.OldItems)
						{
							if (LogicChildren.Contains(item))
							{
								LogicChildren.Remove(item);
							}
						}
					}
					break;

				case NotifyCollectionChangedAction.Move:
					break;

				case NotifyCollectionChangedAction.Replace:
					break;

				case NotifyCollectionChangedAction.Reset:
					break;
			}
		}

		public int GetChildWinCount()
		{
			return mChildWindows.Count;
		}
		public WinBase GetChildWin(int i)
		{
			return mChildWindows[i];
		}
		public void ClearChildWindows()
		{
			for (int i = 0; i < mChildWindows.Count; ++i )
			{
				WinBase pWin = null;
				try
				{
					pWin = mChildWindows[i];
				}
				catch (System.Exception)
				{
					continue;
				}
				
				pWin.mParent = null;
				pWin.UpdateClipRect();
			}
			mChildWindows.Clear();
			mLogicChildren.Clear();
		}
		public WinBase FindFirstChildByWinName(string name)
		{
			for (int i = 0; i < mChildWindows.Count; ++i )
			{
				WinBase pWin = null;
				try
				{
					pWin = mChildWindows[i];
				}
				catch (System.Exception)
				{
					continue;
				}
				
				if (pWin.WinName == name)
					return pWin;
			}
			return null;
		}
		public int IndexOfChild(UIInterface win)
		{
			return ChildWindows.IndexOf(win as WinBase);
		}
		public WinBase[] GetChildWindows()
		{
			return ChildWindows.ToArray();
		}
		public void MoveChild(int fromIndex, int toIndex)
		{
			ChildWindows.Move(fromIndex, toIndex);
		}

		public void AddChildWindowsCollectionChangedHandler(NotifyCollectionChangedEventHandler handler)
		{
			ChildWindows.CollectionChanged += handler;
		}
		public void RemoveChildWindowsCollectionChangedHandler(NotifyCollectionChangedEventHandler handler)
		{
			ChildWindows.CollectionChanged -= handler;
		}
		#endregion

		#region Render
		/*
			广度深度算法
		*/
		public virtual void Draw(UIRenderPipe pipe, int zOrder, ref SlimDX.Matrix parentMatrix)
		{
            //DrawDepthFirst(pipe, zOrder, ref parentMatrix);
            //DrawBreadthFirst(pipe, zOrder, ref parentMatrix);
            DrawUIState(pipe, zOrder, ref parentMatrix);
            DrawUI(pipe, zOrder+1, ref parentMatrix);
        }
        public void DrawUI(UIRenderPipe pipe, int zOrder, ref SlimDX.Matrix parentMatrix)
        {
            //DrawUIState(pipe, zOrder, ref parentMatrix);
            for (int i = 0; i < mChildWindows.Count; ++i)
            {
                WinBase wCurBase = mChildWindows[i];
                if (wCurBase.Visibility == Visibility.Visible)
                {
                    wCurBase.DrawUIState(pipe, zOrder, ref parentMatrix);
                }
            }
            for (int i = 0; i < mChildWindows.Count; ++i)
            {
                WinBase wCurBase = mChildWindows[i];
                if (wCurBase.Visibility == Visibility.Visible)
                {
                    wCurBase.DrawUI(pipe, zOrder + 1, ref parentMatrix);
                }
            }
        }
        //广度
        public virtual void DrawBreadthFirst(UIRenderPipe pipe, int zOrder, ref SlimDX.Matrix parentMatrix)
        {
            CSUtility.Support.ThreadSafeObservableCollection<WinBase> mScopeChild = new CSUtility.Support.ThreadSafeObservableCollection<WinBase>();

            DrawUIState(pipe, zOrder, ref parentMatrix);
            //开始遍历
            for (int i = 0; i < mChildWindows.Count; ++i)
            {
                WinBase wCurBase = mChildWindows[i];
                //try
                //{
                //    wCurBase = mChildWindows[i];
                //}
                //catch (System.Exception)
                //{
                //    continue;
                //}
                //if (wCurBase == null)
                //    continue;
                if (wCurBase.Visibility == Visibility.Visible && IsVisibleInEditor)
                {
                    wCurBase.DrawUIState(pipe, zOrder, ref parentMatrix);
                    mScopeChild.Add(wCurBase);
                }
            }

            if (mScopeChild.Count > 0)
            {
                DrawMyChild(mScopeChild, pipe, zOrder + 10, ref parentMatrix);
                mScopeChild.Clear();
            }
        }
        static CSUtility.Performance.PerfCounter mDrawUIStateTimer = new CSUtility.Performance.PerfCounter("WinBase.DrawUIState");
        static CSUtility.Performance.PerfCounter mDrawTimer = new CSUtility.Performance.PerfCounter("WinBase.Draw");
        //绘制UI
        public virtual void DrawUIState(UIRenderPipe pipe, int zOrder, ref SlimDX.Matrix parentMatrix)
        {
            mDrawUIStateTimer.Begin();
            //在此绘制每个UI
            if (Visibility == Visibility.Visible && IsVisibleInEditor)
            {
                SlimDX.Matrix matTrans = mTransMatrix;// parentMatrix* mTransMatrix;

                BeforeStateDraw(pipe, zOrder);
                OnBeforeDraw?.Invoke(this, ref parentMatrix);
                
                if (mWinState != null)
                {
                    mDrawTimer.Begin();
                    //mWinState.Draw(this, ref mBackColorVertex, ScaleX, ScaleY, ref matTrans, ScaleCenter);
                    mWinState.Draw(pipe, zOrder, this, ref mBackColorVertex, ref matTrans);
                    mDrawTimer.End();
                }

                AfterStateDraw(pipe, zOrder);
                OnAfterDraw?.Invoke(this, ref parentMatrix);
            }
            mDrawUIStateTimer.End();
        }
        /*！ 每个孩子*/
        public virtual void DrawMyChild(CSUtility.Support.ThreadSafeObservableCollection<WinBase> wParentArray, UIRenderPipe pipe, int zOrder, ref SlimDX.Matrix parentMatrix)
        {
            CSUtility.Support.ThreadSafeObservableCollection<WinBase> mScopeChild = new CSUtility.Support.ThreadSafeObservableCollection<WinBase>();
            //开始遍历
            for (int i = 0; i < wParentArray.Count; ++i)
            {
                WinBase wCurBase = null;
                try
                {
                    wCurBase = wParentArray[i];
                }
                catch (System.Exception)
                {
                    continue;
                }
                if (wCurBase == null)
                    continue;
                for (int j = 0; j < wCurBase.ChildWindows.Count; ++j)
                {
                    WinBase wCurChildBase = null;
                    try
                    {
                        wCurChildBase = wCurBase.ChildWindows[j];
                    }
                    catch (System.Exception)
                    {
                        continue;
                    }
                    if (wCurChildBase == null)
                        continue;
                    if (wCurChildBase.Visibility == Visibility.Visible && IsVisibleInEditor)
                    {
                        wCurChildBase.DrawUIState(pipe, zOrder, ref parentMatrix);
                        mScopeChild.Add(wCurChildBase);
                    }
                }
            }
            if (mScopeChild.Count > 0)
            {
                DrawMyChild(mScopeChild, pipe, zOrder + 10, ref parentMatrix);
                mScopeChild.Clear();
            }
        }
      //深度运算
        public virtual void DrawDepthFirst(UIRenderPipe pipe, int zOrder, ref SlimDX.Matrix parentMatrix)
        {
            //if (mVisible)
            if (Visibility == Visibility.Visible && IsVisibleInEditor)
            {
                SlimDX.Matrix matTrans = mTransMatrix;// parentMatrix* mTransMatrix;

                //BeforeStateDraw(pipe, zOrder);
                //OnBeforeDraw?.Invoke(this, ref parentMatrix);

                //if (mWinState != null)
                //{
                //    //mWinState.Draw(this, ref mBackColorVertex, ScaleX, ScaleY, ref matTrans, ScaleCenter);
                //    mWinState.Draw(pipe, zOrder, this, ref mBackColorVertex, ref matTrans);
                //}
                DrawUIState(pipe, zOrder, ref parentMatrix);
                for (int i = 0; i < mChildWindows.Count; ++i)
                {
                    WinBase childWin = null;
                    try
                    {
                        childWin = mChildWindows[i];
                    }
                    catch (System.Exception)
                    {
                        continue;
                    }
                    if (childWin == null)
                        continue;

                    childWin.Draw(pipe, zOrder + 10, ref matTrans);
                }
                AfterStateDraw(pipe, zOrder);
                OnAfterDraw?.Invoke(this, ref parentMatrix);
            }
        }
		protected virtual void BeforeStateDraw(UIRenderPipe pipe, int zOrder) { }
		protected virtual void AfterStateDraw(UIRenderPipe pipe, int zOrder) { }

		public delegate void FWinOnDraw(WinBase win, ref SlimDX.Matrix parentMatrix);
		public event FWinOnDraw OnBeforeDraw;
		public event FWinOnDraw OnAfterDraw;
		#endregion
		
		#region DockAndMove
		//public virtual bool DockToWin(WinBase pWin)
		//{
		//    if( pWin.mClientSize.Width == 0 || pWin.mClientSize.Height == 0 )
		//        return false;

		//    Parent = pWin;

		//    //ResumeNoneDock();
		//    switch( mDockMode )
		//    {
		//    case System.Windows.Forms.DockStyle.None:
		//        {
				
		//        }
		//        break;
		//    case System.Windows.Forms.DockStyle.Fill:
		//        {
		//            if( null==mDockSaver )
		//            {
		//                mDockSaver = new DockSaver();
		//                mDockSaver.Left = Left;
		//                mDockSaver.Top = Top;
		//                mDockSaver.Width = Width;
		//                mDockSaver.Height = Height;
		//            }

		//            mLocation.X = pWin.mClientLocation.X;
		//            mLocation.Y = pWin.mClientLocation.Y;
		//            mSize.Width = pWin.mClientSize.Width;
		//            mSize.Height = pWin.mClientSize.Height;
		//            pWin.mClientSize.Width = 0;
		//            pWin.mClientSize.Height = 0;
		//            UpdateAbsRect();
				
		//            DockChildWin();
		//        }
		//        break;
		//    case System.Windows.Forms.DockStyle.Left:
		//        {
		//            if( null==mDockSaver )
		//            {
		//                mDockSaver = new DockSaver();
		//                mDockSaver.Left = Left;
		//                mDockSaver.Top = Top;
		//                mDockSaver.Width = Width;
		//                mDockSaver.Height = Height;
		//            }
		//            mLocation.X = pWin.mClientLocation.X;
		//            mLocation.Y = pWin.mClientLocation.Y;
		//            mSize.Height = pWin.mClientSize.Height;
				
		//            //停靠的窗口大于客户区，让出客户区一半大小
		//            if( mSize.Width>pWin.mClientSize.Width )
		//                mSize.Width = pWin.mClientSize.Width/2;

		//            pWin.mClientLocation.X += mSize.Width;
		//            pWin.mClientSize.Width -= mSize.Width;
		//            UpdateAbsRect();
				
		//            DockChildWin();
		//        }
		//        break;
		//    case System.Windows.Forms.DockStyle.Top:
		//        {
		//            if( null==mDockSaver )
		//            {
		//                mDockSaver = new DockSaver();
		//                mDockSaver.Left = Left;
		//                mDockSaver.Top = Top;
		//                mDockSaver.Width = Width;
		//                mDockSaver.Height = Height;
		//            }
		//            mLocation.X = pWin.mClientLocation.X;
		//            mLocation.Y = pWin.mClientLocation.Y;
		//            mSize.Width = pWin.mClientSize.Width;

		//            //停靠的窗口大于客户区，让出客户区一半大小
		//            if( mSize.Height>pWin.mClientSize.Height )
		//                mSize.Height = pWin.mClientSize.Height/2;

		//            pWin.mClientLocation.Y += mSize.Height;
		//            pWin.mClientSize.Height -= mSize.Height;
		//            UpdateAbsRect();
				
		//            DockChildWin();
		//        }
		//        break;
		//    case System.Windows.Forms.DockStyle.Bottom:
		//        {
		//            if( null==mDockSaver )
		//            {
		//                mDockSaver = new DockSaver();
		//                mDockSaver.Left = Left;
		//                mDockSaver.Top = Top;
		//                mDockSaver.Width = Width;
		//                mDockSaver.Height = Height;
		//            }
		//            mLocation.X = pWin.mClientLocation.X;
		//            mSize.Width = pWin.mClientSize.Width;

		//            //停靠的窗口大于客户区，让出客户区一半大小
		//            if( mSize.Height>pWin.mClientSize.Height )
		//                mSize.Height = pWin.mClientSize.Height/2;

		//            mLocation.Y = pWin.mClientLocation.Y + pWin.mClientSize.Height - mSize.Height;

		//            pWin.mClientSize.Height -= mSize.Height;

		//            UpdateAbsRect();
				
		//            DockChildWin();
		//        }
		//        break;
		//    case System.Windows.Forms.DockStyle.Right:
		//        {
		//            mLocation.Y = pWin.mClientLocation.Y;
		//            mSize.Height = pWin.mClientSize.Height;

		//            //停靠的窗口大于客户区，让出客户区一半大小
		//            if( mSize.Width>pWin.mClientSize.Width )
		//                mSize.Width = pWin.mClientSize.Width/2;

		//            mLocation.X = pWin.mClientLocation.X + pWin.mClientSize.Width - mSize.Width;

		//            pWin.mClientSize.Width -= mSize.Width;

		//            UpdateAbsRect();
		//            DockChildWin();
		//        }
		//        break;
		//    }
		//    return true;
		//}
		protected virtual void MoveToWin(ref CSUtility.Support.Point pt, bool withChildren = true)
		{
			mLocation = pt;
			UpdateAbsRect(false);

			//foreach( WinBase i in mChildWindows )
			if(withChildren)
			{
				for (int i = 0; i < mChildWindows.Count;i++ )
				{
					WinBase win = null;
					try
					{
						win = mChildWindows[i];
					}
					catch (System.Exception)
					{
						return;
					}
				
					win.MoveToWin(ref win.mLocation);
				}
			}
		}

		private bool mMoveWinWaitToTick = false;
		public virtual void MoveWin(ref CSUtility.Support.Point pt)
		{
			if (!mMoveWinWaitToTick)
			{
				if (this.Parent is UISystem.Container.Canvas)
				{
					mMargin.Left = pt.X;
					mMargin.Top = pt.Y;

					MoveToWin(ref pt);
				}
				else
				{
					var offsetX = pt.X - mLocation.X;
					var offsetY = pt.Y - mLocation.Y;

					this.Margin = new CSUtility.Support.Thickness(this.Margin.Left + offsetX, this.Margin.Top + offsetY, this.Margin.Right, this.Margin.Bottom);

				}

				mMoveWinWaitToTick = true;
			}
		}

		//private void DockChildWin()
		//{
		//    mClientLocation.X = 0;
		//    mClientLocation.Y = 0;
		//    mClientSize.Width = mSize.Width;
		//    mClientSize.Height = mSize.Height;
		//    ///检测子窗口的Dock属性
		
		//    foreach( WinBase i in mChildWindows )
		//    {
		//        i.DockToWin( this );
		//    }
		//}
		protected virtual void UpdateAbsRect(bool bWithChildren = true)
		{
			CSUtility.Support.Rectangle oldRect = mAbsRect;

			if( mParent!=null )
			{
				/*mAbsRect.FromLTRB( mParent.AbsRect.Left + mLocation.X ,  
					mParent.AbsRect.Top + mLocation.Y , 
					mParent.AbsRect.Left + mLocation.X + mSize.Width , 
					mParent.AbsRect.Top + mLocation.Y + mSize.Height );*/
				mAbsRect.Location = new CSUtility.Support.Point(mParent.AbsRect.Left + mLocation.X, mParent.AbsRect.Top + mLocation.Y);
				mAbsRect.Size = mSize;
			}
			else
			{
				mAbsRect.Location = new CSUtility.Support.Point(0, 0);
				mAbsRect.Size = mSize;
			}

			if (!FloatUtil.AreClose(mAbsRect, oldRect))
			{
				UpdateClipRect();
				UpdateTransMatrix();
			}
		}
		public virtual void UpdateClipRect(bool bWithChildren = true)
		{
			if( Parent!=null )
			{
				var parentClipRect = ((WinBase)Parent).GetClipRect(this);
				mClipRect = CSUtility.Support.Rectangle.Intersect(parentClipRect, mAbsRect);
			}
			else
			{
				mClipRect = mAbsRect;
			}

			if(bWithChildren)
			{
				for (int i = mChildWindows.Count - 1; i >= 0; i--)
				{
					mChildWindows[i].UpdateClipRect();
				}
			}
		}
		public virtual CSUtility.Support.Rectangle GetClipRect(WinBase child)
		{
			return mClipRect;
		}

		// 获得鼠标相对于本控件的位置
		public CSUtility.Support.Point GetLocalMousePoint()
		{
			var pt = UISystem.Device.Mouse.Instance.Position;
			return AbsToLocal(ref pt);
		}

		//private void ResumeNoneDock()
		//{
		//    if( null!=mDockSaver )
		//    {
		//        switch( mDockMode )
		//        {
		//        case System.Windows.Forms.DockStyle.None:
		//            {

		//            }
		//            break;
		//        case System.Windows.Forms.DockStyle.Fill:
		//            {

		//            }
		//            break;
		//        }
		//        mLocation.X = mDockSaver.Left;
		//        mLocation.Y = mDockSaver.Top;
		//        mSize.Width = mDockSaver.Width;
		//        mSize.Height = mDockSaver.Height;
		//    }
		//    MoveToWin( ref mLocation );
		//}
		//private class DockSaver
		//{
		//    public int Left;
		//    public int Top;
		//    public int Width;
		//    public int Height;
		//};
		//private DockSaver mDockSaver;
		#endregion

		//#region MouseFocus

		//bool mKeyboardFocusEnable = false;
		//[Category("焦点")]
		//public bool KeyboardFocusEnable
		//{
		//    get { return mKeyboardFocusEnable; }
		//    set
		//    {
		//        mKeyboardFocusEnable = value;
		//    }
		//}

		//bool mKeyboardFocus = false;
		//[Browsable(false)]
		//public bool KeyboardFocus
		//{
		//    get { return mKeyboardFocus; }
		//    set
		//    {
		//        if(mKeyboardFocus == value)
		//            return;

		//        mKeyboardFocus = value;

		//        if (KeyboardFocusEnable == true)
		//        {
		//            if (mKeyboardFocus == true)
		//                OnGotKeyboardFocus();
		//            else
		//                OnLostKeyboardFocus();
		//        }
		//    }
		//}

		//protected virtual void OnGotKeyboardFocus()
		//{

		//}

		//protected virtual void OnLostKeyboardFocus()
		//{
		//}

		//#endregion

		//protected bool mForceChildMsg;
		//[Browsable(false)]
		//public bool ForceChildMsg
		//{
		//    get { return mForceChildMsg; }
		//}

          
		
		protected bool mDraging;
		protected CSUtility.Support.Point mDragLocation = CSUtility.Support.Point.Empty;
		protected CSUtility.Support.Point mDragOffset = CSUtility.Support.Point.Empty;

		//客户区，相对窗口内的矩形，用来处理Dock
		protected CSUtility.Support.Point mClientLocation;
		protected CSUtility.Support.Size mClientSize;

		//bool mStartAction = false;
		//float mActionSpeed = 30.0f;
		CSUtility.Support.Point mStartLocation = CSUtility.Support.Point.Empty;
		public virtual void Tick(float elapsedMillisecondTime)
		{
			mMoveWinWaitToTick = false;
			//if (mNeedChangeVisibility)
			//{
			//    ChangeVisibility();
			//}

			if (Visibility != Visibility.Visible)
				return;

			//if (mStartAction)
			//{
			//    var ptMouse = ((WinBase)Parent).AbsToLocal(AbsRect.X, AbsRect.Y);
			//    var distance = Math.Sqrt(Math.Pow(Math.Abs(ptMouse.X - mStartLocation.X), 2) + Math.Pow(Math.Abs(ptMouse.Y - mStartLocation.Y), 2));
			//    if (distance <= mActionSpeed)
			//    {
			//        mStartAction = false;
			//        ptMouse = mStartLocation;
			//    }
			//    else
			//    {
			//        var star = new SlimDX.Vector2(mStartLocation.X, mStartLocation.Y);
			//        var dir = new SlimDX.Vector2(ptMouse.X - mStartLocation.X, ptMouse.Y - mStartLocation.Y);
			//        dir.Normalize();
			//        var pos = star + dir * (float)(distance - mActionSpeed);
			//        ptMouse.X = (int)pos.X;
			//        ptMouse.Y = (int)pos.Y;
			//    }

			//    MoveWin(ref ptMouse);
			//}

			//foreach (var child in ChildWindows)
			for (int i = 0; i < ChildWindows.Count; i++)
			{
				WinBase child = null;
				try
				{
					child = ChildWindows[i];
				}
				catch (System.Exception)
				{
					continue;
				}
				if (child == null)
					continue;
				child.Tick(elapsedMillisecondTime);
			}
		}
	}
}
