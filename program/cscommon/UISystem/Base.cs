using System;
using System.Collections.Generic;
using System.ComponentModel;
using CSUtility.Support;

namespace UISystem
{
    [System.ComponentModel.TypeConverter("System.ComponentModel.ExpandableObjectConverter")]
    public class UVFrame : CSUtility.Support.Copyable, INotifyPropertyChanged
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

        protected UVAnim mParentAnim = null;
        [CSUtility.Support.DoNotCopyAttribute]
        [Browsable(false)]
        public UVAnim ParentAnim
        {
            get { return mParentAnim; }
            set
            {
                mParentAnim = value;

                if (mParentAnim != null)
                {
                    UpdateScale9Infos();
                }
            }
        }

        public CSUtility.Support.RectangleF mUVRect = new CSUtility.Support.RectangleF(0.0f, 0.0f, 1.0f, 1.0f);

        private bool mIsDirty = false;
        [System.ComponentModel.Browsable(false)]
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;
                OnPropertyChanged("IsDirty");

                if(mParentAnim != null && value == true)
                    mParentAnim.IsDirty = value;
            }
        }

        [CSUtility.Support.DataValueAttribute("U")]
        public float U
        {
            get { return mUVRect.X; }
            set
            {
                var tempValue = mUVRect.X;
                mUVRect.X = value;
                OnPropertyChanged("U");

                //if(!IsDirty)
                    IsDirty = (tempValue != mUVRect.X);
            }
        }

        [CSUtility.Support.DataValueAttribute("V")]
        public float V
        {
            get { return mUVRect.Y; }
            set
            {
                var tempValue = mUVRect.Y;
                mUVRect.Y = value;
                OnPropertyChanged("V");

                //if(!IsDirty)
                    IsDirty = (tempValue != mUVRect.Y);
            }
        }

        [CSUtility.Support.DataValueAttribute("SizeU")]
        public float SizeU
        {
            get { return mUVRect.Width; }
            set
            {
                var tempValue = mUVRect.Width;
                mUVRect.Width = value;
                OnPropertyChanged("SizeU");

                //if(!IsDirty)
                    IsDirty = (tempValue != mUVRect.Width);
            }
        }

        [CSUtility.Support.DataValueAttribute("SizeV")]
        public float SizeV
        {
            get { return mUVRect.Height; }
            set
            {
                var tempValue = mUVRect.Height;
                mUVRect.Height = value;
                OnPropertyChanged("SizeV");

                //if(!IsDirty)
                    IsDirty = (tempValue != mUVRect.Height);
            }
        }

        // 九宫格 --------------------------------------
        int mLeftPixel = 0;
        [Browsable(false)]
        public int LeftPixel
        {
            get{ return mLeftPixel; }
        }
        int mRightPixel = 0;
        [Browsable(false)]
        public int RightPixel
        {
            get{ return mRightPixel; }
        }
        int mTopPixel = 0;
        [Browsable(false)]
        public int TopPixel
        {
            get{ return mTopPixel; }
        }
        int mBottomPixel = 0;
        [Browsable(false)]
        public int BottomPixel
        {
            get { return mBottomPixel; }
        }

        CSUtility.Support.Thickness mScale9Info = new CSUtility.Support.Thickness();
        [CSUtility.Support.DataValueAttribute("Scale9Info")]
        [CSUtility.Editor.UIEditor_Scale9InfoEditor]
        public CSUtility.Support.Thickness Scale9Info
        {
            get { return mScale9Info; }
            set
            {
                var tempValue = mScale9Info;
                mScale9Info = value;

                UpdateScale9Infos();

                OnPropertyChanged("mScale9Info");

                IsDirty = (tempValue != mScale9Info);
            }
        }
        
        // 九宫格渲染时的图素
        List<Scale9DrawInfo> mScale9DrawRectangles = new List<Scale9DrawInfo>();
        [Browsable(false)]
        public List<Scale9DrawInfo> Scale9DrawRectangles
        {
            get { return mScale9DrawRectangles; }
        }

        public void UpdateScale9Infos()
        {
            if (mParentAnim != null && mParentAnim.TextureObject != null)
            {
                mScale9DrawRectangles.Clear();
                float leftOffset = (float)mScale9Info.Left * SizeU;
                float topOffset = (float)mScale9Info.Top * SizeV;
                float rightOffset = (float)mScale9Info.Right * SizeU;
                float bottomOffset = (float)mScale9Info.Bottom * SizeV;
                mLeftPixel = (int)(mParentAnim.TextureObject.Width * SizeU * mScale9Info.Left);
                mTopPixel = (int)(mParentAnim.TextureObject.Height * SizeV * mScale9Info.Top);
                mRightPixel = (int)(mParentAnim.TextureObject.Width * SizeU * mScale9Info.Right);
                mBottomPixel = (int)(mParentAnim.TextureObject.Height * SizeV * mScale9Info.Bottom);
                if (mScale9Info.Left > 0)
                {
                    if (mScale9Info.Top > 0)
                    {
                        Scale9DrawInfo info = new Scale9DrawInfo(this, Scale9DrawInfo.enScale9Type.LeftTop);
                        info.mDrawUVRect = new CSUtility.Support.RectangleF(U,
                                                                        V,
                                                                        leftOffset,
                                                                        topOffset);
                        mScale9DrawRectangles.Add(info);
                    }
                    if (mScale9Info.Bottom > 0)
                    {
                        Scale9DrawInfo info = new Scale9DrawInfo(this, Scale9DrawInfo.enScale9Type.LeftBottom);
                        info.mDrawUVRect = new CSUtility.Support.RectangleF(U,
                                                                        V + SizeV - bottomOffset,
                                                                        leftOffset,
                                                                        bottomOffset);
                        mScale9DrawRectangles.Add(info);
                    }
                    Scale9DrawInfo oInfo = new Scale9DrawInfo(this, Scale9DrawInfo.enScale9Type.Left);
                    oInfo.mDrawUVRect = new CSUtility.Support.RectangleF(U,
                                                                    V + topOffset,
                                                                    leftOffset,
                                                                    SizeV - topOffset - bottomOffset);
                    mScale9DrawRectangles.Add(oInfo);
                }
                if (mScale9Info.Right > 0)
                {
                    if (mScale9Info.Top > 0)
                    {
                        Scale9DrawInfo info = new Scale9DrawInfo(this, Scale9DrawInfo.enScale9Type.RightTop);
                        info.mDrawUVRect = new CSUtility.Support.RectangleF(U + SizeU - rightOffset,
                                                                                       V,
                                                                                       rightOffset,
                                                                                       topOffset);
                        mScale9DrawRectangles.Add(info);
                    }
                    if (mScale9Info.Bottom > 0)
                    {
                        Scale9DrawInfo info = new Scale9DrawInfo(this, Scale9DrawInfo.enScale9Type.RightBottom);
                        info.mDrawUVRect = new CSUtility.Support.RectangleF(U + SizeU - rightOffset,
                                                                                       V + SizeV - bottomOffset,
                                                                                       rightOffset,
                                                                                       bottomOffset);
                        mScale9DrawRectangles.Add(info);
                    }
                    Scale9DrawInfo oInfo = new Scale9DrawInfo(this, Scale9DrawInfo.enScale9Type.Right);
                    oInfo.mDrawUVRect = new CSUtility.Support.RectangleF(U + SizeU - rightOffset,
                                                                                    V + topOffset,
                                                                                    rightOffset,
                                                                                    SizeV - topOffset - bottomOffset);
                    mScale9DrawRectangles.Add(oInfo);
                }
                if (mScale9Info.Top > 0)
                {
                    Scale9DrawInfo info = new Scale9DrawInfo(this, Scale9DrawInfo.enScale9Type.Top);
                    info.mDrawUVRect = new CSUtility.Support.RectangleF(U + leftOffset,
                                                                                   V,
                                                                                   SizeU - leftOffset - rightOffset,
                                                                                   topOffset);
                    mScale9DrawRectangles.Add(info);
                }
                if (mScale9Info.Bottom > 0)
                {
                    Scale9DrawInfo info = new Scale9DrawInfo(this, Scale9DrawInfo.enScale9Type.Bottom);
                    info.mDrawUVRect = new CSUtility.Support.RectangleF(U + leftOffset,
                                                                                   V + SizeV - bottomOffset,
                                                                                   SizeU - leftOffset - rightOffset,
                                                                                   bottomOffset);
                    mScale9DrawRectangles.Add(info);
                }

                Scale9DrawInfo cInfo = new Scale9DrawInfo(this, Scale9DrawInfo.enScale9Type.Center);
                cInfo.mDrawUVRect.X = U + leftOffset;
                cInfo.mDrawUVRect.Y = V + topOffset;
                cInfo.mDrawUVRect.Width = SizeU - leftOffset - rightOffset;
                cInfo.mDrawUVRect.Height = SizeV - topOffset - bottomOffset;
                mScale9DrawRectangles.Add(cInfo);
            }
        }
    }

    public class Scale9DrawInfo
    {
        public enum enScale9Type
        {
            None,
            LeftTop,
            Top,
            RightTop,
            Right,
            RightBottom,
            Bottom,
            LeftBottom,
            Left,
            Center,
        }

        private UVFrame mHostFrame;
        private enScale9Type mScale9Type;
        public enScale9Type Scale9Type
        {
            get { return mScale9Type; }
        }
        public CSUtility.Support.RectangleF mDrawUVRect = new CSUtility.Support.RectangleF();
        //public SlimDX.Size mDrawSize = new SlimDX.Size();
        public CSUtility.Support.Rectangle mDrawRect = new CSUtility.Support.Rectangle();

        public Scale9DrawInfo(UVFrame hostFrame, enScale9Type type)
        {
            mHostFrame = hostFrame;
            mScale9Type = type;
        }

        public CSUtility.Support.Rectangle GetDrawRect(CSUtility.Support.Rectangle orcRect)
        {
            switch (mScale9Type)
            {
                case enScale9Type.LeftTop:
                    {
                        mDrawRect.X = orcRect.X;
                        mDrawRect.Y = orcRect.Y;
                        mDrawRect.Width = mHostFrame.LeftPixel;
                        mDrawRect.Height = mHostFrame.TopPixel;
                    }
                    break;

                case enScale9Type.Top:
                    {
                        mDrawRect.X = orcRect.X + mHostFrame.LeftPixel;
                        mDrawRect.Y = orcRect.Y;
                        mDrawRect.Width = orcRect.Width - mHostFrame.LeftPixel - mHostFrame.RightPixel;
                        mDrawRect.Height = mHostFrame.TopPixel;
                    }
                    break;

                case enScale9Type.RightTop:
                    {
                        mDrawRect.X = orcRect.X + orcRect.Width - mHostFrame.RightPixel;
                        mDrawRect.Y = orcRect.Y;
                        mDrawRect.Width = mHostFrame.RightPixel;
                        mDrawRect.Height = mHostFrame.TopPixel;
                    }
                    break;

                case enScale9Type.Right:
                    {
                        mDrawRect.X = orcRect.X + orcRect.Width - mHostFrame.RightPixel;
                        mDrawRect.Y = orcRect.Y + mHostFrame.TopPixel;
                        mDrawRect.Width = mHostFrame.RightPixel;
                        mDrawRect.Height = orcRect.Height - mHostFrame.TopPixel - mHostFrame.BottomPixel;
                    }
                    break;

                case enScale9Type.RightBottom:
                    {
                        mDrawRect.X = orcRect.X + orcRect.Width - mHostFrame.RightPixel;
                        mDrawRect.Y = orcRect.Y + orcRect.Height - mHostFrame.BottomPixel;
                        mDrawRect.Width = mHostFrame.RightPixel;
                        mDrawRect.Height = mHostFrame.BottomPixel;
                    }
                    break;

                case enScale9Type.Bottom:
                    {
                        mDrawRect.X = orcRect.X + mHostFrame.LeftPixel;
                        mDrawRect.Y = orcRect.Y + orcRect.Height - mHostFrame.BottomPixel;
                        mDrawRect.Width = orcRect.Width - mHostFrame.LeftPixel - mHostFrame.RightPixel;
                        mDrawRect.Height = mHostFrame.BottomPixel;
                    }
                    break;

                case enScale9Type.LeftBottom:
                    {
                        mDrawRect.X = orcRect.X;
                        mDrawRect.Y = orcRect.Y + orcRect.Height - mHostFrame.BottomPixel;
                        mDrawRect.Width = mHostFrame.LeftPixel;
                        mDrawRect.Height = mHostFrame.BottomPixel;
                    }
                    break;

                case enScale9Type.Left:
                    {
                        mDrawRect.X = orcRect.X;
                        mDrawRect.Y = orcRect.Y + mHostFrame.TopPixel;
                        mDrawRect.Width = mHostFrame.LeftPixel;
                        mDrawRect.Height = orcRect.Height - mHostFrame.TopPixel - mHostFrame.BottomPixel;
                    }
                    break;

                case enScale9Type.Center:
                    {
                        mDrawRect.X = orcRect.X + mHostFrame.LeftPixel;
                        mDrawRect.Y = orcRect.Y + mHostFrame.TopPixel;
                        mDrawRect.Width = orcRect.Width - mHostFrame.LeftPixel - mHostFrame.RightPixel;
                        mDrawRect.Height = orcRect.Height - mHostFrame.TopPixel - mHostFrame.BottomPixel;
                    }
                    break;
            }

            return mDrawRect;
        }
    }

    [System.ComponentModel.TypeConverter("System.ComponentModel.ExpandableObjectConverter")]
    public class UVAnim : CSUtility.Support.Copyable, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        ~UVAnim()
        {
            Cleanup();
        }

        public void ResetUVAnimFramePlay()
        {
            mCurrentState = enState.Frame;
            mCurrentPlayTimes = 0;
            mPreFrameIndex = 0;
            Opacity = mOriginalOpacity;
            mFrameStartTime = CCore.Engine.Instance.GetFrameSecondTimeFloatByUVAnim();
        }

        public enum enState
        {
            Frame,
            FadeOut,
        }
        enState mCurrentState = enState.Frame;
        public enState CurrentState
        {
            get { return mCurrentState; }
        }
        
        private bool mIsDirty = false;
        [System.ComponentModel.Browsable(false)]
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;
                OnPropertyChanged("IsDirty");
            }
        }

        Guid mId;
        [CSUtility.Support.DataValueAttribute("Id")]
        [System.ComponentModel.ReadOnly(true)]
        public Guid Id
        {
            get { return mId; }
            set
            {
                var tempValue = mId;
                mId = value;
                OnPropertyChanged("Id");

                if(!IsDirty)
                    IsDirty = (tempValue != mId);
            }
        }

        string mPath = "";
        [CSUtility.Support.DataValueAttribute("Path")]
        public string Path
        {
            get { return mPath; }
            set
            {
                var tempValue = mPath;
                mPath = value;
                OnPropertyChanged("Path");

                if(!IsDirty)
                    IsDirty = (tempValue != mPath);
            }
        }

        string mUVAnimName = "";
        [CSUtility.Support.DataValueAttribute("UVAnimName")]
        public string UVAnimName
        {
            get { return mUVAnimName; }
            set
            {
                var tempValue = mUVAnimName;
                mUVAnimName = value;
                OnPropertyChanged("UVAnimName");

                if(!IsDirty)
                    IsDirty = (tempValue != mUVAnimName);
            }
        }

        string mTexture = "";
        [CSUtility.Support.DataValueAttribute("Texture")]
        //[CSUtility.Editor.Editor_TexturePathValueAttribute]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("TextureSet")]
        public string Texture
        {
            get { return mTexture; }
            set
            {
                var tempValue = mTexture;
                mTexture = value;
                if (null != mTextureObject)
                {
                    mTextureObject.Cleanup();
                    //mTextureObject.Dispose();
                }

                mTextureObject = new CCore.Graphics.Texture();
                if (mTextureObject.LoadTexture(mTexture) == false)
                {
                    mTextureObject = null;
                }
                
                if (mTextureObject != null)
                {
                    mTextureObject.ColorSpace = CCore.TextureColorSpace.TCS_LINEAR;
                    mTextureObject.PreUse(true, CCore.Engine.Instance.GetFrameMillisecond());
                    //mTextureObject.PreUse(false, CCore.Engine.Instance.GetFrameMillisecond());
                }

                OnPropertyChanged("Texture");

                if(!IsDirty)
                    IsDirty = (tempValue != mTexture);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public int FirstFrameHeight
        {
            get 
            {
                if (Frames.Count <= 0)
                    return 0;

                if (TextureObject == null)
                    return 0;

                var height = (int)(Frames[0].SizeV * TextureObject.Height);
                return height;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public int FirstFrameWidth
        {
            get
            {
                if (Frames.Count <= 0)
                    return 0;

                if (TextureObject == null)
                    return 0;

                var width = (int)(Frames[0].SizeU * TextureObject.Width);
                return width;
            }
        }

        //整个动画播放多久
        float mDuration = 1.0f;
        [CSUtility.Support.DataValueAttribute("Duration")]
        public float Duration
        {
            get { return mDuration; }
            set
            {
                var tempValue = mDuration;
                mDuration = value;
                OnPropertyChanged("Duration");

                if(!IsDirty)
                    IsDirty = (tempValue != mDuration);
            }
        }

        List<UVFrame> mFrames = new List<UVFrame>();
        [CSUtility.Support.DataValueAttribute("Frames")]
        [System.ComponentModel.Browsable(false)]
        public List<UVFrame> Frames
        {
            get { return mFrames; }
            set
            {
                var tempValue = mFrames;
                mFrames = value;

                foreach (var frame in mFrames)
                {
                    frame.ParentAnim = this;
                }

                OnPropertyChanged("Frames");

                //if(!IsDirty)
                    IsDirty = (tempValue != mFrames);
            }
        }

        Guid mTechId = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("TechId")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("TechniqueSet")]
        public Guid TechId
        {
            get { return mTechId; }
            set
            {
                var tempTechId = mTechId;
                mTechId = value;

                if (mMaterialObject != null)
                {
                    mMaterialObject.Cleanup();
                    //mMaterialObject.Dispose();
                }

                mMaterialObject = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(mTechId);
                //if (mMaterialObject == null)
                //    mMaterialObject = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadTechnique(mTechId);
                OnPropertyChanged("TechId");

                IsDirty = (tempTechId != mTechId);
            }
        }

        float mOriginalOpacity = 1.0f;
        float mOpacity = 1.0f;
        [CSUtility.Support.DataValueAttribute("Opacity")]
        [CSUtility.Editor.Editor_ValueWithRange(0, 1)]
        public float Opacity
        {
            get { return mOpacity; }
            set
            {
                var tempOpacity = mOpacity;
                mOpacity = value;
                mOriginalOpacity = mOpacity;

                OnPropertyChanged("Opacity");
                IsDirty = (tempOpacity != mOpacity);
            }
        }

        //MidLayer.IMaterialParameter mMaterial = new MidLayer.IMaterialParameter();
        //[System.ComponentModel.EditorAttribute(typeof(MaterialParameterEditor), typeof(System.Drawing.Design.UITypeEditor))]
        //[CSUtility.Support.DataValueAttribute("Material")]
        //[CSUtility.Editor.Editor_MaterialParamValue]
        //public MidLayer.IMaterialParameter Material
        //{
        //    get { return mMaterial; }
        //    set 
        //    {
        //        var tempValue = mMaterial;
        //        mMaterial = value;
        //        if (null != mMaterialObject)
        //        {
        //            mMaterialObject.Cleanup();
        //            mMaterialObject.Dispose();
        //        }
        //        /*
        //        mMaterialObject = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadMaterial(mMaterial);
        //        if(mMaterialObject==null)
        //            mMaterialObject = CCore.Engine.Instance.Client.Graphics.MaterialMgr.LoadMaterial(mMaterial);
        //        */
        //        mMaterialObject = CCore.Engine.Instance.Client.Graphics.MaterialMgr.GetDefaultMaterial();
        //        OnPropertyChanged("Material");

        //        //if(!IsDirty)
        //            IsDirty = (tempValue != mMaterial);
        //    }
        //}

        CCore.Graphics.Texture mTextureObject;
        [DoNotCopy]
        [System.ComponentModel.Browsable(false)]
        public CCore.Graphics.Texture TextureObject
        {
            get { return mTextureObject; }
            set
            {
                mTextureObject.Cleanup();
                mTextureObject = value;
            }
        }

        CCore.Material.Material mMaterialObject;
        [DoNotCopy]
        [System.ComponentModel.Browsable(false)]
        public CCore.Material.Material MaterialObject
        {
            get { return mMaterialObject; }
        }

        // 播放次数，0表示循环播放
        int mCurrentPlayTimes = 0;
        public int CurrentPlayTimes
        {
            get { return mCurrentPlayTimes; }
        }
        int mPlayTimes = 0;
        [CSUtility.Support.DataValueAttribute("PlayTimes")]
        public int PlayTimes
        {
            get { return mPlayTimes; }
            set
            {
                var tempTimes = mPlayTimes;
                mPlayTimes = value;

                ResetUVAnimFramePlay();

                OnPropertyChanged("PlayTimes");
                IsDirty = (tempTimes != mPlayTimes);
            }
        }

        // 播放结束后渐隐
        bool mFadeoutWhenFrameFinish = true;
        [CSUtility.Support.DataValueAttribute("FadeoutWhenFrameFinish")]
        public bool FadeoutWhenFrameFinish
        {
            get { return mFadeoutWhenFrameFinish; }
            set
            {
                var tempFadeout = mFadeoutWhenFrameFinish;
                mFadeoutWhenFrameFinish = value;

                OnPropertyChanged("FadeoutWhenFrameFinish");
                IsDirty = (tempFadeout != mFadeoutWhenFrameFinish);
            }
        }

        // 渐隐时间
        Int64 mFadeoutTime = 1000;
        [CSUtility.Support.DataValueAttribute("FadeoutTime")]
        public Int64 FadeoutTime
        {
            get { return mFadeoutTime; }
            set
            {
                var tempTime = mFadeoutTime;
                mFadeoutTime = value;

                OnPropertyChanged("FadeoutTime");
                IsDirty = (tempTime != mFadeoutTime);
            }
        }

        public UVAnim()
        {
            TechId = CSUtility.Support.IFileConfig.UVAnimDefaultTechnique;// CSUtility.Support.IHelper.GuidTryParse("a96158b1-4dca-4833-afb8-7be61c2eea73");
        }

        int mPreFrameIndex = 0;
        Int64 mFadeOutStartTime = 0;
        public Int64 FadeOutStartTime
        {
            get { return mFadeOutStartTime; }
        }

        float mFrameStartTime = 0;
        private int GetFrameIndex(float time)
        {
            time -= mFrameStartTime;
            float fRemainder = (float)System.Math.IEEERemainder(time, mDuration) / mDuration;
            if (fRemainder < 0)
                fRemainder = 1 + fRemainder;
            return (int)(fRemainder * Frames.Count);
        }

        public UVFrame GetUVFrame(float time)
        {
            if (Frames.Count == 1 || mDuration == 0)
                return mFrames[0];

            if (mCurrentState == enState.FadeOut)
                return mFrames[mFrames.Count - 1];

            int i = GetFrameIndex(time);

            if (mPreFrameIndex > i)
            {
                mCurrentPlayTimes++;

                if (PlayTimes > 0 && mCurrentPlayTimes >= PlayTimes)
                {
                    if(FadeoutWhenFrameFinish)
                        mCurrentState = enState.FadeOut;
                    mFadeOutStartTime = (Int64)(CCore.Engine.Instance.GetFrameSecondTimeFloatByUVAnim() * 1000);
                    return mFrames[mFrames.Count - 1];
                }
            }
            mPreFrameIndex = i;

            return mFrames[i];
        }

        public void FadeOutProcess()
        {
            if (CurrentState != enState.FadeOut)
                return;

            if (!FadeoutWhenFrameFinish)
                return;

            var time = (Int64)(CCore.Engine.Instance.GetFrameSecondTimeFloatByUVAnim() * 1000) - mFadeOutStartTime;
            mOpacity = (1 - time * 1.0f / FadeoutTime) * mOriginalOpacity;
        }

        public void Cleanup()
        {
            if (null != TextureObject)
            {
                TextureObject.Cleanup();
                //TextureObject.Dispose();
            }

            if (null != MaterialObject)
            {
                MaterialObject.Cleanup();
                //MaterialObject.Dispose();
            }
        }

        public UVFrame AddFrame()
        {
            UVFrame frame = new UVFrame();
            frame.ParentAnim = this;
            Frames.Add(frame);
            IsDirty = true;

            return frame;
        }

        public void DelFrame(int idx)
        {
            // 至少有一帧
            if (Frames.Count <= 1)
                return;

            if (idx < 0 || idx >= Frames.Count)
                return;

            Frames.RemoveAt(idx);
            IsDirty = true;
        }

        UVAnim mTemplateUVAnim = null;
        public UVAnim TemplateUVAnim
        {
            get { return mTemplateUVAnim; }
        }
        public override bool CopyFrom(ICopyable src)
        {
            mTemplateUVAnim = src as UVAnim;
            this.Id = mTemplateUVAnim.Id;
            return base.CopyFrom(src);
        }

        UInt64 mVersion = 0;
        public UInt64 Version
        {
            get { return mVersion; }
            set
            {
                mVersion = value;
                OnPropertyChanged("Version");
            }
        }
        public void CheckAndAutoRefreshFromTemplateUVAnim()
        {
            if (mTemplateUVAnim == null)
                return;

            if(mTemplateUVAnim.Version != Version)
            {
                CopyFrom(mTemplateUVAnim);
            }
        }
    }

    public class UVAnimMgr
    {
        const string mExtName = "uvanim";

        static UVAnimMgr smInstance = new UVAnimMgr();
        public static UVAnimMgr Instance
        {
            get { return smInstance; }
        }

        public void Cleanup()
        {
            mAnims.For_Each((Guid id, UVAnim anim, object argObj) =>
            {
                anim.Cleanup();
                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
            mAnims.Clear();
        }

        CSUtility.Support.ConcurentObjManager<Guid, UVAnim> mAnims = new CSUtility.Support.ConcurentObjManager<Guid, UVAnim>();
        public CSUtility.Support.ConcurentObjManager<Guid, UVAnim> Anims
        {
            get { return mAnims; }
        }
        CSUtility.Support.ConcurentObjManager<Guid, string> mAnimFileNames = new CSUtility.Support.ConcurentObjManager<Guid, string>();
        public void SetUVAnimFileName(Guid id, string tagFileName)
        {
            mAnimFileNames.Add(id, tagFileName);
        }

        public static Guid GetIdFromFile(string fileName)
        {
            var exten = CSUtility.Support.IFileManager.Instance.GetFileExtension(fileName);
            if (("." + exten) != CSUtility.Support.IFileConfig.UVAnimExtension)
                return Guid.Empty;

            fileName = fileName.Replace('/','\\');
            var idStartIdx = fileName.LastIndexOf("\\");
            var idEndIdx = fileName.LastIndexOf(".");
            var idStr = fileName.Substring(idStartIdx + 1, idEndIdx - idStartIdx - 1);

            Guid retId = CSUtility.Support.IHelper.GuidTryParse(idStr);

            return retId;
        }

        public string GetUVAnimFileName(Guid id)
        {
            return mAnimFileNames.FindObj(id);
        }

        // folder为绝对路径
        public UVAnim Add(Guid id, string folder)
        {
            UVAnim anim = mAnims.FindObj(id);
            if (anim != null)
                return anim;

            anim = new UVAnim();
            anim.Id = id;
            mAnims.Add(id, anim);
            mAnimFileNames.Add(id, folder.Replace("\\", "/").Replace(CSUtility.Support.IFileManager.Instance.Root.Replace("\\", "/"), "") + "/" + id.ToString() + CSUtility.Support.IFileConfig.UVAnimExtension);
            return anim;
        }

        public bool Remove(Guid id, bool bDeleteFile = false)
        {
            UVAnim anim = mAnims.FindObj(id);
            if (anim == null)
                return false;

            anim.Cleanup();
            mAnims.Remove(id);

            if (bDeleteFile)
            {
                var file = mAnimFileNames.FindObj(id);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }

            mAnimFileNames.Remove(id);

            return true;
        }

        static CSUtility.Support.ConcurentObjManager<Guid, string> mUVAnims = null;
        static void BuildUVAnimDic(string dir)
        {
            foreach (var file in System.IO.Directory.GetFiles(dir, "*" + CSUtility.Support.IFileConfig.UVAnimExtension, System.IO.SearchOption.AllDirectories))
            {
                var id = GetIdFromFile(file);
                if (id == Guid.Empty)
                    continue;

                string temp = mUVAnims.FindObj(id);
                if (temp != null)
                {
                    Log.FileLog.WriteLine(string.Format("BuildUVAnimDic file={0} ID重复了", id));
                    System.Diagnostics.Debug.WriteLine(string.Format("BuildUVAnimDic file={0} ID重复了", id));
                    continue;
                }
                mUVAnims.Add(id, file);
            }
        }
        private UVAnim FindInFolder(Guid id, string absDir)
        {
            if (mUVAnims==null)
            {
                mUVAnims = new CSUtility.Support.ConcurentObjManager<Guid, string>();
                BuildUVAnimDic(CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory);               
            }
            string tempFile = mUVAnims.FindObj(id);
            if (tempFile != null)
            {
                tempFile = tempFile.Replace(CSUtility.Support.IFileManager.Instance.Root.Replace("\\", "/"), "");
                var anim = new UVAnim();
                if (!CSUtility.Support.IConfigurator.FillProperty(anim, tempFile))
                    return null;

                anim.IsDirty = false;
                mAnims.Add(id, anim);
                mAnimFileNames.Add(id, tempFile);

                return anim;
            }

            return null;
        }

        public UVAnim Find(Guid id, bool bSearchFromFile = false, string strPath = "")
        {
            UVAnim retAnim = null;
            // 返回的UVAnim做一次拷贝，保证使用的数据是独有的
            UVAnim anim = mAnims.FindObj(id);
            if (anim != null)
            {
                retAnim = new UVAnim();
                retAnim.CopyFrom(anim);
                return retAnim;
            }

            if (!bSearchFromFile)
                return null;

            if (string.IsNullOrEmpty(strPath))
                strPath = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultResourceDirectory;

            anim = FindInFolder(id, strPath);
            if(anim != null)
            {
                retAnim = new UVAnim();
                retAnim.CopyFrom(anim);
            }

            return retAnim;
        }

        public void ForceReload(Guid id)
        {
            UVAnim anim = Find(id, true);

            var fileName = mAnimFileNames.FindObj(id);

            if(CSUtility.Support.IConfigurator.FillProperty(anim, fileName))
            {
                anim.IsDirty = false;
            }
        }

        public void SaveUVAnim(Guid id)
        {
            UVAnim anim = mAnims.FindObj(id);
            if (anim == null)
                return;

            if (anim.IsDirty)
            {
                var file = mAnimFileNames.FindObj(id);
                anim.Version++;
                CSUtility.Support.IConfigurator.SaveProperty(anim, anim.UVAnimName, file);
                anim.IsDirty = false;
            }
        }

        public void Editor_RefreshAllUVAnim()
        {
            mUVAnims = new CSUtility.Support.ConcurentObjManager<Guid, string>();
            var strPath = CSUtility.Support.IFileManager.Instance.Root + CSUtility.Support.IFileConfig.DefaultUVAnimDirectory;
            BuildUVAnimDic(strPath);

            mUVAnims.For_Each((Guid id, string value, object argObj) =>
            {
                var uvAnim = Find(id, true);
                if (uvAnim == null)
                    return CSUtility.Support.EForEachResult.FER_Continue;

                uvAnim.IsDirty = true;
                SaveUVAnim(id);

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }
    }

    public enum MSG_PROC
    {
        Finished        ,
		SendToParent	,		
		SendToBrother	,
	}

    public enum MsgDefineUI : int
    {
        MsgUIStart = 100,
        OnSetUVAnim,
    }

    [System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential)]
	public struct WinMSG
	{
        public IntPtr hwnd;
        public int message;
        public IntPtr wParam;
        public IntPtr lParam;
        public Int64 time;
        public CSUtility.Support.Point pt;
        public WinBase Sender;
	};
}