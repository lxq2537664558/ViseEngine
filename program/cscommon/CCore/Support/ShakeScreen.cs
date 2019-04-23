using System;

namespace CCore.Support
{
    /// <summary>
    /// 屏幕震动类
    /// </summary>
    public class ShakeScreen : CSUtility.Animation.NotifyItemDataBase
    {
        /// <summary>
        /// X轴震动幅度
        /// </summary>
        public double mShakeX = 0;
        /// <summary>
        /// Y轴震动幅度
        /// </summary>
        public double mShakeY = 0;

        string mName = "";
        /// <summary>
        /// 震动名称
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.DisplayName("名称")]
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        CCore.Support.ScalarVariable mShakeXVariable = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// X轴震动的值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.DisplayName("X轴震动")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable ShakeXVariable
        {
            get { return mShakeXVariable; }
            set { mShakeXVariable = value; }
        }

        CCore.Support.ScalarVariable mShakeYVariable = new CCore.Support.ScalarVariable(0);
        /// <summary>
        /// Y轴震动的值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.DisplayName("Y轴震动")]
        [CSUtility.Editor.Editor_ScalarVariableSetter]
        public CCore.Support.ScalarVariable ShakeYVariable
        {
            get { return mShakeYVariable; }
            set { mShakeYVariable = value; }
        }

        //int mFPS = 20;
        //[CSUtility.Support.AutoSaveLoad]
        //[System.ComponentModel.DisplayName("震动FPS")]
        //public int FPS
        //{
        //    get { return mFPS; }
        //    set { mFPS = value; }
        //}

        int mShakeDuration = 300;
        /// <summary>
        /// 震动时长(ms)
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.DisplayName("震动时长(ms)")]
        public int ShakeDuration
        {
            get { return mShakeDuration; }
            set { mShakeDuration = value; }
        }

        long mShakeTime = 0;
        /// <summary>
        /// 震动时间
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public long ShakeTime
        {
            get { return mShakeTime; }
            set { mShakeTime = value; }
        }

        bool mUseRandomValueX = false;
        /// <summary>
        /// X轴随机取值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.DisplayName("X轴随机取值")]
        public bool UseRandomValueX
        {
            get { return mUseRandomValueX; }
            set { mUseRandomValueX = value; }
        }
        bool mUseRandomValueY = false;
        /// <summary>
        /// Y轴随机取值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [System.ComponentModel.DisplayName("Y轴随机取值")]
        public bool UseRandomValueY
        {
            get { return mUseRandomValueY; }
            set { mUseRandomValueY = value; }
        }

        bool mEnable = false;
        /// <summary>
        /// 只读属性，是否开始震动
        /// </summary>
        public bool Enable
        {
            get { return mEnable; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ShakeScreen()
        {
//             ShakeXVariable.ScalarVariablePtr = DllImportAPI.v3dScalarVariable_New();
//             ShakeYVariable.ScalarVariablePtr = DllImportAPI.v3dScalarVariable_New();
        }
        /// <summary>
        /// 析构函数，删除实例对象
        /// </summary>
        ~ShakeScreen()
        {
            DllImportAPI.v3dScalarVariable_Delete(ShakeXVariable.ScalarVariablePtr);
            DllImportAPI.v3dScalarVariable_Delete(ShakeYVariable.ScalarVariablePtr);
        }
        /// <summary>
        /// 从XND文件读取数据
        /// </summary>
        /// <param name="xndAtt">XND文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (base.Read(xndAtt) == false)
                return false;

            if (ShakeXVariable.ScalarVariablePtr != IntPtr.Zero)
                DllImportAPI.v3dScalarVariable_Delete(ShakeXVariable.ScalarVariablePtr);
            if (ShakeYVariable.ScalarVariablePtr != IntPtr.Zero)
                DllImportAPI.v3dScalarVariable_Delete(ShakeYVariable.ScalarVariablePtr);
            ShakeXVariable.ScalarVariablePtr = DllImportAPI.v3dScalarVariable_New();
            ShakeXVariable.SetValueToIntptr(ShakeXVariable.ScalarVariablePtr);
            ShakeYVariable.ScalarVariablePtr = DllImportAPI.v3dScalarVariable_New();
            ShakeYVariable.SetValueToIntptr(ShakeYVariable.ScalarVariablePtr);

            return true;
        }
        /// <summary>
        /// 从源数据复制对象
        /// </summary>
        /// <param name="srcData">XND的源数据</param>
        /// <returns>复制成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            if (!base.CopyFrom(srcData))
                return false;

            if (ShakeXVariable.ScalarVariablePtr != IntPtr.Zero)
                DllImportAPI.v3dScalarVariable_Delete(ShakeXVariable.ScalarVariablePtr);
            if (ShakeYVariable.ScalarVariablePtr != IntPtr.Zero)
                DllImportAPI.v3dScalarVariable_Delete(ShakeYVariable.ScalarVariablePtr);
            ShakeXVariable.ScalarVariablePtr = DllImportAPI.v3dScalarVariable_New();
            ShakeXVariable.SetValueToIntptr(ShakeXVariable.ScalarVariablePtr);
            ShakeYVariable.ScalarVariablePtr = DllImportAPI.v3dScalarVariable_New();
            ShakeYVariable.SetValueToIntptr(ShakeYVariable.ScalarVariablePtr);

            return true;
        }
        /// <summary>
        /// 开始震动
        /// </summary>
        public void Start()
        {
            mEnable = true;
            mShakeTime = 0;
            mShakeX = 0;
            mShakeY = 0;
        }
        /// <summary>
        /// 结束震动
        /// </summary>
        public void End()
        {
            mEnable = false;
            mShakeTime = 0;
            mShakeX = 0;
            mShakeY = 0;
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        public void Tick()
        {
            if (mEnable == false)
                return;

            mShakeTime += CCore.Engine.Instance.GetElapsedMillisecond();
            if (mShakeTime > mShakeDuration)
            {
                End();
                return;
            }

            float t = (float)mShakeTime / (float)mShakeDuration;
            if (mUseRandomValueX == true)
            {
                mShakeX = mShakeXVariable.GetRandomValue();
            }
            else
            {
                mShakeX = mShakeXVariable.GetValue(t);
            }
            if (mUseRandomValueY == true)
            {
                mShakeY = mShakeYVariable.GetRandomValue();
            }
            else
            {
                mShakeY = mShakeYVariable.GetValue(t);
            }
        }
    }
}
