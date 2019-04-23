using System;
using System.Collections.Generic;
using System.Text;
using CCore.MsgProc;

namespace Client
{
    public class GameUIRoot : UISystem.WinRoot
    {
        public string FPSString = "";
        public string DPInfoString = "";
        public string ActorInfoString = "";
        public string ActorDetailString = "";
        public string ParticleDetailString = "";
        public string DownloadingString = "";
        public string LineCheckString = "";
        public string PerCounterString = "";
        //System.Drawing.Rectangle mDestRect = new System.Drawing.Rectangle();        
        public string PlaneTickCount = "";
        public string GateTickCount = "";

        //CCore.Font.FontRenderParam p;
        //CCore.Font.FontRenderParamList fontParams;
        protected override void AfterStateDraw(UISystem.UIRenderPipe pipe, int zOrder)
        {
            //var pRect = new CSUtility.Support.Rectangle();

            //pRect = this.AbsRect;
            //pRect.X = this.AbsRect.X;
            //pRect.Y = this.AbsRect.Y;
            //pRect.Width = this.AbsRect.Width;
            //pRect.Height = 100;
            UISystem.IRender.GetInstance().SetClipRect(pipe, this.AbsRect);

            unsafe
            {
                SlimDX.Vector4 rect = new SlimDX.Vector4(this.AbsRect.Left, this.AbsRect.Top, this.AbsRect.Right, this.AbsRect.Bottom);
                CCore.DllImportAPI.v3dUIRender_SetClipRect(pipe.InnerPtr, IntPtr.Zero, &rect);
            }

            //UISystem.IRender.GetInstance().SetClipRect(pRect);
            UISystem.IRender.GetInstance().SetClipRect(pipe, this.AbsRect);

            unsafe
            {
                SlimDX.Vector4 rect = new SlimDX.Vector4(this.AbsRect.Left, this.AbsRect.Top, this.AbsRect.Right, this.AbsRect.Bottom);
                CCore.DllImportAPI.v3dUIRender_SetClipRect(pipe.InnerPtr,IntPtr.Zero, &rect);
            }
            //if (fontParams == null)
            {
                //fontParams = new CCore.Font.FontRenderParamList();
                //p = fontParams.AddParam();
                //    p.BLColor = p.BRColor = p.TLColor = p.TRColor = CSUtility.Support.Color.FromArgb(255, 255, 255, 0);

                //    p.BLColor = p.BRColor = p.TLColor = p.TRColor = System.Drawing.Color.FromArgb(255, 0, 0, 0);
                //    p.OutlineType = CCore.Font.enFontOutlineType.Line;
                //    p.OutlineThickness = 1.5f;
                //    p = fontParams.AddParam();
                //    p.BLColor = p.BRColor = p.TLColor = p.TRColor = System.Drawing.Color.FromArgb(255, 255, 255, 0);
            }

            {
                //if (!string.IsNullOrEmpty(FPSString))
                  //  UISystem.IRender.GetInstance().DrawString(20, 100, FPSString, 15, fontParams);

                //SlimDX.Vector3 playerPos = SlimDX.Vector3.Zero;
                //Stage.MainStage.Instance.ChiefRole?.Placement.GetLocation(out playerPos);
                //UISystem.IRender.GetInstance().DrawString(20, 150,"X:" + playerPos.X.ToString("F0") + " Z:" + playerPos.Z.ToString("F0") , 15, fontParams);
                //UISystem.IRender.GetInstance().DrawString(20, 34, DPInfoString, 15, fontParams);
                //UISystem.IRender.GetInstance().DrawString(20, 51, ActorInfoString, 15, fontParams);
                //UISystem.IRender.GetInstance().DrawString(20, 68, ActorDetailString, 15, fontParams);
                //pRender.DrawString(20, 85, DownloadingString, 15, fontParams);
                //pRender.DrawString(20, 102, LineCheckString, 15, fontParams);
                //pRender.DrawString(20, 119, PerCounterString, 15, fontParams);
                //UISystem.IRender.GetInstance().DrawString(20, 135, ParticleDetailString, 15, fontParams);

                //pRender.DrawString(this.Width - 150, this.Height - 225, GateTickCount, 15, fontParams);
                //pRender.DrawString(this.Width - 150, this.Height - 208, PlaneTickCount, 15, fontParams);
            }
        }
    }

    public class RootUIMsgReceiver : CCore.MsgProc.MsgReceiver
    {
        public GameUIRoot Root { get; } = new GameUIRoot();

        public override int OnBehavior(int bhType, BehaviorParameter bhInit, bool bAsync=false)
        {
            Root.DispatchBehavior(bhInit, bAsync);
            return 0;
        }
    }
}
