using System.ComponentModel;

namespace UISystem
{
    //[CSUtility.Editor.UIEditor_Control("Panel")]
    public class Panel : WinControl
    {
        public Panel()
        {
            mWinState = new WinState(this);
            //BackColor = CSUtility.Support.Color.FromArgb(200, 200, 200);
        }

        private bool mDragParent = false;
        [Category("行为")]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public bool DragParent
        {
			get { return mDragParent; }
			set { mDragParent = value; }
		}
        //[Category("外观")]
        //[Browsable(false)]
        //[CSUtility.Editor.UIEditor_BindingPropertyAttribute]
        //public override string Text
        //{
        //    get { return base.Text; }
        //    set
        //    {
        //        base.Text = value;
        //        RState.Text = value;
        //    }
        //}
        //[Category("外观")]
        [Browsable(false)]
        [CSUtility.Editor.UIEditor_BindingPropertyAttribute]
		public override UI.ContentAlignment TextAlign
        {
            get { return base.TextAlign; }
			set
            {
                base.TextAlign = value;
		        RState.TextAlign = value;
            }
		}

        //public delegate void FWinPanelClick(WinBase Sender);
        //[CSUtility.Editor.UIEditor_BindingEventAttribute]
        //public event FWinPanelClick WinPanelClick;

        //protected override MSG_PROC OnMsg(ref WinMSG msg)
        //{
        //    switch( msg.message )
        //    {
        //    case (UInt32)MidLayer.SysMessage.VWM_LBUTTONDOWN:
        //        {
        //            if( mDragParent )
        //            {
        //                msg.Sender = this;
        //                Send2ParentWin( ref msg );
        //                return MSG_PROC.Finished;
        //            }
        //            if( mDragEnable )//&& DockMode==System.Windows.Forms.DockStyle.None )
        //            {
        //                mDraging = true;
        //                mDragLocation = AbsToLocal( ref msg.pt );

        //                WinRoot.GetInstance().CaptureMouse( this );
        //            }
        //            else
        //            {
        //                if (WinPanelClick != null)
        //                    WinPanelClick(this);
        //            }
        //            break;
        //        }
        //    case (UInt32)MidLayer.SysMessage.VWM_LBUTTONUP:
        //        {
        //            if (mDragParent)
        //            {
        //                msg.Sender = this;
        //                Send2ParentWin(ref msg);
        //                return MSG_PROC.Finished;
        //            }
        //            break;
        //        }
        //    }
        //    return base.OnMsg(ref msg);
        //}

        protected override void OnSave(CSUtility.Support.XmlNode pXml, CSUtility.Support.XmlHolder holder)
        {
            base.OnSave(pXml,holder);

            if (!mDefaultValueTemplate.IsEqualDefaultValue(this, "DragParent"))
		        pXml.AddAttrib( "DragParent", DragParent.ToString() );

		    if(mWinState!=null)
		    {
			    CSUtility.Support.XmlNode state = pXml.AddNode( "WinState" , "" ,holder );
			    mWinState.OnSave( state ,holder);
		    }
        }
        protected override void OnLoad(CSUtility.Support.XmlNode pXml)
        {
		    base.OnLoad(pXml);

		    CSUtility.Support.XmlAttrib attr = pXml.FindAttrib( "DragParent" );
		    if(attr!=null)
			    DragParent = System.Convert.ToBoolean( attr.Value );

		    CSUtility.Support.XmlNode state = pXml.FindNode( "WinState" );
		    if(state!=null)
		    {
			    if(mWinState==null)
				    mWinState = new WinState(this);
			    mWinState.OnLoad(state);
		    }
        }
    }
}
