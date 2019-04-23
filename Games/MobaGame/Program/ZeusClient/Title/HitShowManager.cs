using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Title
{
    public enum enHitType
    {
        Normal = 0,    // 普攻
        //Crit = 1,    // 暴击
        //Skill = 2,    // 技能
        //NotHit = 3,    // 未命中
        //Miss = 4,    // 闪避
        //Block = 5,    // 格挡
        //Parry = 6,    // 招架
        //Blind = 7,    // 致盲
        //Reflect = 8,    // 反射
        //NormalCure = 9,  // 治愈
        SelfHurtNormal = 1, // 主角受到普通伤害
        Count,
    }

    public class HitShowManager
    {
        static HitShowManager mInstance = new HitShowManager();
        public static HitShowManager Instance
        {
            get { return mInstance; }
        }

        public static void FinalInstance()
        {
            mInstance = null;
        }

        System.Random mRand = new System.Random();
        UISystem.WinForm mForm = null;
        UISystem.Container.Canvas mMainCanvas = null;
        float mHitShowSpeed = 0.25f;
        float mLifeTime = 2000.0f;

        public class CTextEffectorInit
        {
            public CTickData parentData = null;
        }
        public class CTextEffector
        {
            public CTextEffectorInit mInit = null;
            public bool mFirstTick = true;
            public virtual void Initialize(CTextEffectorInit init)
            {
                mInit = init;
            }
            public virtual void effectText(float elapsedMillisecond)
            {

            }

        }
        public class CScaleEffectorInit : CTextEffectorInit
        {
            public float scaleAdj = 0;
            public float minScale = 0.4f;
            public float maxScale = 1.5f;
        }
        public class CScaleEffector : CTextEffector
        {
            public float mScaleAdj = 0;
            public float mMinScale = 0;
            public float mMaxScale = 0;

            public override void Initialize(CTextEffectorInit init)
            {
                base.Initialize(init);
                var scaleInit = init as CScaleEffectorInit;
                mScaleAdj = scaleInit.scaleAdj;
                mMinScale = scaleInit.minScale;
                mMaxScale = scaleInit.maxScale;
            }

            public override void effectText(float elapsedMillisecond)
            {
                if (mInit.parentData == null)
                    return;
                if (mFirstTick == true)
                {
                    mFirstTick = false;
                    return;
                }

                // Scale adjustments by time
                var ds = mScaleAdj * (float)elapsedMillisecond * 0.001f;
                mInit.parentData.mCurScale += ds;

                if (mInit.parentData.mCurScale > mMaxScale)
                    mInit.parentData.mCurScale = mMaxScale;
                if (mInit.parentData.mCurScale < mMinScale)
                    mInit.parentData.mCurScale = mMinScale;
            }
        }

        public class CLinearForceEffectorInit : CTextEffectorInit
        {
            public SlimDX.Vector2 forceVector = SlimDX.Vector2.Zero;
            public float startTime = 0;
            public float toplineTime = 0;
        }
        public class CLinearForceEffector : CTextEffector
        {
            public SlimDX.Vector2 mForceVector = SlimDX.Vector2.Zero;
            public float mStartTime = 0;
            public float mToplineTime = 0;

            public override void Initialize(CTextEffectorInit init)
            {
                base.Initialize(init);
                var lfInit = init as CLinearForceEffectorInit;
                mForceVector = lfInit.forceVector;
                mStartTime = lfInit.startTime;
                mToplineTime = lfInit.toplineTime;
            }

            public override void effectText(float elapsedMillisecond)
            {
                if (mInit.parentData == null)
                    return;

                if (mStartTime > mInit.parentData.LiveTime)
                    return;

                if (mFirstTick == true)
                {
                    mFirstTick = false;
                    return;
                }

                SlimDX.Vector2 scaledVector = mForceVector;
                if (mToplineTime > mInit.parentData.LiveTime)
                {
                    scaledVector.Y = -mForceVector.Y;
                }
                else
                {
                    scaledVector.Y = mForceVector.Y;
                }

                // Scale adjustments by time
                // x轴沿方向移动
                scaledVector = scaledVector * (float)elapsedMillisecond * 0.001f;
                if (mInit.parentData.XOffset == 0)
                {
                    scaledVector.X = 0;
                }
                else
                {
                    scaledVector.X = mInit.parentData.XOffset / System.Math.Abs(mInit.parentData.XOffset) * scaledVector.X;
                }
                mInit.parentData.mDeltaX += scaledVector.X;
                mInit.parentData.mDeltaY += scaledVector.Y;
            }
        }

        public class CAlphaEffectorInit : CTextEffectorInit
        {
            public float alphaAdjust = 0;
            public float startTime = 0;
        }
        public class CAlphaEffector : CTextEffector
        {
            public float mAlphaAdj = 0;
            public float mStartTime = 0;

            public override void Initialize(CTextEffectorInit init)
            {
                base.Initialize(init);
                var alphaInit = init as CAlphaEffectorInit;
                mAlphaAdj = alphaInit.alphaAdjust;
                mStartTime = alphaInit.startTime;
            }

            public override void effectText(float elapsedMillisecond)
            {
                if (mInit.parentData == null)
                    return;
                if (mStartTime > mInit.parentData.LiveTime)
                    return;
                if (mFirstTick == true)
                {
                    mFirstTick = false;
                    return;
                }

                // Scale adjustments by time
                var da = mAlphaAdj * (float)elapsedMillisecond * 0.001f;
                mInit.parentData.TextCtrl.Opacity += da;
                if (mInit.parentData.TextCtrl.Opacity < 0)
                    mInit.parentData.TextCtrl.Opacity = 0;
            }
        }


        public class CTickData
        {
            public UISystem.TextBlock TextCtrl;
            public UISystem.WinBase Win;
            public SlimDX.Vector3 mBornPos = SlimDX.Vector3.Zero;
            public float mDeltaX = 0;
            public float mDeltaY = 0;
            public float XOffset = 0;
            public float YOffset = 0;
            public float TextCenterXOffset = 0;
            public float TextCenterYOffset = 0;
            public CCore.Camera.CameraObject Camera;
            public CCore.World.Actor Actor;
            public float LiveTime;
            public bool IsDead = false;
            enHitType mHitType = enHitType.Normal;
            public enHitType HitType
            {
                get { return mHitType; }
            }
            public float HitShowSpeed;
            public float LifeTime;
            //int mMoveState = 0;

            // 缩放参数
            //float mMaxScale = 1.8f;
            public float mCurScale = 1;
            //float mScaleSpeed = 0.008f;
            int mFontSize;
            //int mScaleState = 0;

            public List<CTextEffector> mEffectorList = new List<CTextEffector>();

            public string TextShow
            {
                get
                {
                    if (TextCtrl != null)
                        return TextCtrl.Text;
                    return "";
                }
                set
                {
                    if (TextCtrl != null)
                    {
                        TextCtrl.Text = value;
                        TextCtrl.UpdateLayout(true);
                    }
                }
            }

            public CTickData(UISystem.WinBase win, enHitType hitType, float moveSpeed, float lifeTime)//, float x, float y)
            {
                Win = win;
                if (Win != null)
                {
                    TextCtrl = Win.FindControl("TextShow") as UISystem.TextBlock;
                    if (TextCtrl != null)
                        mFontSize = TextCtrl.FontSize;
                }
                mHitType = hitType;
                HitShowSpeed = moveSpeed;
                LifeTime = lifeTime;

                Reset();
            }

            public void Reset()
            {
                if (Win != null)
                    Win.Visibility = UISystem.Visibility.Collapsed;

                mDeltaX = 0;
                mDeltaY = 0;
                XOffset = 0;
                YOffset = 0;
                TextCenterXOffset = 0;
                TextCenterYOffset = 0;
                LiveTime = 0;
                IsDead = false;
                mCurScale = 1;
                //mScaleState = 0;
                //mMoveState = 0;
                Camera = null;
                Actor = null;
                if (TextCtrl != null)
                {
                    TextCtrl.Opacity = 1;
                    foreach (var effector in mEffectorList)
                    {
                        effector.mFirstTick = true;
                    }
                }
            }

            public void Tick(float elapsedMillisecond)
            {
                if (IsDead)
                    return;

                foreach (var effector in mEffectorList)
                {
                    effector.effectText(elapsedMillisecond);
                }

                TextCtrl.FontSize = (int)(mFontSize * mCurScale);

                SlimDX.Vector3 screenLoc = SlimDX.Vector3.Zero;
                if (Camera != null && Actor != null && Actor.Placement != null)
                {
                    var loc = Actor.Placement.GetLocation();
                    screenLoc = Camera.GetScreenCoord(ref mBornPos, Client.Game.Instance.GInit.Vector2ScreenCoordScale);
                }
                var tempX = mDeltaX + screenLoc.X + XOffset + TextCenterXOffset;
                var tempY = mDeltaY + screenLoc.Y + YOffset + TextCenterYOffset;
                var pt = new CSUtility.Support.Point((int)tempX, (int)tempY);
                Win.MoveWin(ref pt);

                LiveTime += elapsedMillisecond;
                if (LiveTime >= LifeTime)
                    IsDead = true;
            }
        }
        List<List<CTickData>> mHitShowDatas = new List<List<CTickData>>();
        List<CTickData> mTickControls = new List<CTickData>();
        int[] mCurrentShowIndexs;

        public HitShowManager()
        {
            var form = CCore.Support.ReflectionManager.Instance.GetUIForm("HitShowForm");
            if (form == null)
                return;
            mForm = form as UISystem.WinForm;
            mMainCanvas = mForm.FindControl("MainCanvas") as UISystem.Container.Canvas;
            var critCanvas = mForm.FindControl("CritCanvas") as UISystem.Container.Canvas;

            mCurrentShowIndexs = new int[(int)enHitType.Count];
            string hitShowFormName = "";
            int maxCount = 50;
            for (int typeId = 0; typeId < (int)enHitType.Count; typeId++)
            {
                UISystem.Container.Canvas currCanvas = mMainCanvas;
                switch ((enHitType)typeId)
                {
                    case enHitType.Normal:
                        maxCount = 50;
                        hitShowFormName = "NormalHit";
                        break;
                    //case enHitType.Skill:
                    //    maxCount = 50;
                    //    hitShowFormName = "SkillHit";
                    //    break;
                    //case enHitType.Crit:
                    //    maxCount = 10;
                    //    hitShowFormName = "CriticalHit";
                    //    currCanvas = critCanvas;
                    //    break;
                    //case enHitType.NotHit:
                    //    maxCount = 10;
                    //    hitShowFormName = "NotHit";
                    //    break;
                    //case enHitType.Miss:
                    //    maxCount = 10;
                    //    hitShowFormName = "Miss";
                    //    break;
                    //case enHitType.Block:       // 显示格挡和招架数字太密，直接显示数字
                    //    maxCount = 10;
                    //    hitShowFormName = "NormalHit";
                    //    //hitShowFormName = "Block";
                    //    break;
                    //case enHitType.Parry:
                    //    maxCount = 10;
                    //    hitShowFormName = "NormalHit";
                    //    //hitShowFormName = "Parry";
                    //    break;
                    //case enHitType.Blind:
                    //    maxCount = 10;
                    //    hitShowFormName = "Blind";
                    //    break;
                    //case enHitType.Reflect:
                    //    maxCount = 10;
                    //    hitShowFormName = "Reflect";
                    //    break;
                    //case enHitType.NormalCure:
                    //    maxCount = 10;
                    //    hitShowFormName = "NormalCure";
                    //    break;
                    case enHitType.SelfHurtNormal:
                        maxCount = 20;
                        hitShowFormName = "SelfHurtNormalHit";
                        break;
                }
                var datas = new List<CTickData>();
                for (int i = 0; i < maxCount; i++)
                {
                    var ctrl = CCore.Support.ReflectionManager.Instance.GetUIForm(hitShowFormName, hitShowFormName + i) as UISystem.WinBase;
                    if (ctrl == null)
                        continue;

                    ctrl.Visibility = UISystem.Visibility.Collapsed;
                    ctrl.Parent = currCanvas;

                    var data = new CTickData(ctrl, (enHitType)typeId, mHitShowSpeed, mLifeTime);//, 0, 0);
                    switch ((enHitType)typeId)
                    {
                        //case enHitType.Crit:
                        case enHitType.Normal:
                        //case enHitType.NormalCure:
                        //case enHitType.Skill:
                        case enHitType.SelfHurtNormal:
                            {
                                {
                                    CScaleEffector scaleEffector = new CScaleEffector();
                                    CScaleEffectorInit scaleInit = new CScaleEffectorInit();
                                    scaleInit.parentData = data;
                                    scaleInit.scaleAdj = -4;
                                    scaleEffector.Initialize(scaleInit);
                                    data.mEffectorList.Add(scaleEffector);
                                }
                                {
                                    CAlphaEffector effector = new CAlphaEffector();
                                    CAlphaEffectorInit init = new CAlphaEffectorInit();
                                    init.parentData = data;
                                    init.startTime = (mLifeTime - 200) / 1.5f + 200;
                                    init.alphaAdjust = -2.0f;
                                    effector.Initialize(init);
                                    data.mEffectorList.Add(effector);
                                }
                            }
                            break;
                    }
                    {
                        CLinearForceEffector lfEffector = new CLinearForceEffector();
                        CLinearForceEffectorInit lfInit = new CLinearForceEffectorInit();
                        lfInit.parentData = data;
                        lfInit.forceVector = new SlimDX.Vector2(30, 60);
                        lfInit.startTime = 200;
                        lfInit.toplineTime = (mLifeTime - 200) / 3 + 200;
                        lfEffector.Initialize(lfInit);
                        data.mEffectorList.Add(lfEffector);
                    }

                    datas.Add(data);
                }

                mHitShowDatas.Add(datas);
            }

            SetRoot(Game.Instance.RootUIMsg.Root);
        }

        public void SetRoot(UISystem.WinBase rootWin)
        {
            mForm.Parent = rootWin;
        }

        private CTickData GetAvaliableControl(int hitTypeIdx)
        {
            if (mCurrentShowIndexs == null)
                return null;
            mCurrentShowIndexs[hitTypeIdx]++;

            if (mCurrentShowIndexs[hitTypeIdx] >= mHitShowDatas[hitTypeIdx].Count)
                mCurrentShowIndexs[hitTypeIdx] = 0;

            return mHitShowDatas[hitTypeIdx][mCurrentShowIndexs[hitTypeIdx]];
        }

        public void ShowHit(enHitType type, string hitString, CCore.Camera.CameraObject camera, CCore.World.Actor actor)//int x, int y)
        {
            if (camera == null || actor == null || actor.Placement == null)
                return;

            var data = GetAvaliableControl((int)type);
            if (data == null)
                return;

            switch (type)
            {
                case enHitType.Normal:
                //case enHitType.Skill:
                //case enHitType.Crit:
                //case enHitType.NormalCure:
                //case enHitType.Block:
                //case enHitType.Parry:
                    data.TextShow = hitString;
                    data.XOffset = mRand.Next(-50, 50);
                    data.YOffset = mRand.Next(-60, -40);
                    break;
                case enHitType.SelfHurtNormal:
                    data.TextShow = hitString;
                    data.XOffset = mRand.Next(-30, 30);
                    data.YOffset = mRand.Next(-20, -10);
                    break;
                //case enHitType.NotHit:
                //case enHitType.Miss:
                //case enHitType.Blind:
                //case enHitType.Reflect:
                //    data.XOffset = mRand.Next(-30, 30);
                //    data.YOffset = mRand.Next(20, 40);
                //    break;
            }

            var loc = actor.Placement.GetLocation();
            if (actor is Role.RoleActor)
            {
                // 加上角色的半高偏移
                var roleActor = actor as Role.RoleActor;
                loc.Y += roleActor.RoleTemplate.HalfHeight * 1.5f;
            }
            data.mBornPos = loc;
            var screenLoc = camera.GetScreenCoord(ref loc, Client.Game.Instance.GInit.Vector2ScreenCoordScale);
            //data.XOffset = mRand.Next(-60, 60);
            //data.YOffset = mRand.Next(-60, -40);
            data.TextCenterXOffset = -hitString.Length * 20 / 2;
            data.TextCenterYOffset = -data.Win.Height / 2;
            data.Camera = camera;
            data.Actor = actor;
            //data.X = screenLoc.X;
            //data.Y = screenLoc.Y;
            var pt = new CSUtility.Support.Point((int)(screenLoc.X + data.XOffset),
                (int)(screenLoc.Y + data.YOffset));
            data.Win.MoveWin(ref pt);
            data.Win.Visibility = UISystem.Visibility.Visible;

            mTickControls.Add(data);
        }

        public void Tick(long milliSecondElapsedTime)
        {
            for (int i = 0; i < mTickControls.Count;)
            {
                mTickControls[i].LiveTime += milliSecondElapsedTime;
                if (mTickControls[i].LiveTime >= mLifeTime)
                {
                    mTickControls[i].Reset();
                    mTickControls.RemoveAt(i);
                }
                else
                {
                    mTickControls[i].Tick(milliSecondElapsedTime);

                    i++;
                }
            }
        }
    }
}
