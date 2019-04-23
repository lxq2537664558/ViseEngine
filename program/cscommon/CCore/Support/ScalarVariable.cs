using System;
/// <summary>
/// Support的名称空间
/// </summary>
namespace CCore.Support
{
    // 与Victory3D中v3dVariable::Type对应
    /// <summary>
    /// 可变类型
    /// </summary>
    public enum enVariableType
    {
        Constant,
        ConstantRange,
        Curve,
    }
    /// <summary>
    /// 标量变量
    /// </summary>
    public class ScalarVariable : CSUtility.Support.XndSaveLoadProxy
    {
        /// <summary>
        /// 克隆另一个标量变量
        /// </summary>
        /// <param name="src">需要克隆的标量变量</param>
        public virtual void Clone( ScalarVariable src )
        {
            VariableType = src.VariableType;
            ConstantValue = src.ConstantValue;
            ConstantRangeBegin = src.ConstantRangeBegin;
            ConstantRangeEnd = src.ConstantRangeEnd;
            Bezier2DCurve = new CSUtility.Support.V3dxBezier2DWrapper();
            Bezier2DCurve = src.Bezier2DCurve;
        }

        IntPtr mScalarVariablePtr = IntPtr.Zero;
        /// <summary>
        /// 标量变量的地址
        /// </summary>
        [CSUtility.Support.DoNotCopy]
        public IntPtr ScalarVariablePtr    // v3dScalarVariable 这里只是引用，不做生存管理
        {
            get { return mScalarVariablePtr; }
            set
            {
                //if(mScalarVariablePtr!=IntPtr.Zero)
                //    IDllImportAPI.v3dScalarVariable_Delete(mScalarVariablePtr);

                mScalarVariablePtr = value;
                if (Bezier2DCurve == null)
                {
                    Bezier2DCurve = new CSUtility.Support.V3dxBezier2DWrapper();
                    var ptr = DllImportAPI.v3dScalarVariable_GetBezierPtr(mScalarVariablePtr);
                    Bezier2DCurve.GetDataFromIntptr(ptr);
                    Bezier2DCurve.BezierPtr = ptr;
                }
            }
        }
        /// <summary>
        /// 只读属性，某一个变量类型的最大值
        /// </summary>
        public float MaxValue
        {
            get
            {
                switch (VariableType)
                {
                    case enVariableType.Constant:
                        return ConstantValue;
                    case enVariableType.ConstantRange:
                    case enVariableType.Curve:
                        return System.Math.Max(ConstantRangeBegin, ConstantRangeEnd);
                }

                return 0;
            }
        }

        enVariableType mVariableType = enVariableType.Constant;
        /// <summary>
        /// 变量类型
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public enVariableType VariableType
        {
            get { return mVariableType; }
            set
            {
                mVariableType = value;
                unsafe
                {
                    DllImportAPI.v3dScalarVariable_SetVariableType(ScalarVariablePtr, (int)value);

                    if (mVariableType == enVariableType.Curve)
                    {
                        Bezier2DCurve = new CSUtility.Support.V3dxBezier2DWrapper();
                        var ptr = DllImportAPI.v3dScalarVariable_GetBezierPtr(mScalarVariablePtr);
                        Bezier2DCurve.GetDataFromIntptr(ptr);
                        Bezier2DCurve.BezierPtr = ptr;
                    }
                }
            }
        }

        float mConstantValue = 0;
        /// <summary>
        /// 常量值
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public float ConstantValue
        {
            get { return mConstantValue; }
            set
            {
                mConstantValue = value;
                unsafe
                {
                    DllImportAPI.v3dScalarVariable_SetValue(ScalarVariablePtr, value);
                }
            }
        }

        float mConstantRangeBegin = 0;
        /// <summary>
        /// 常量的起始范围
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public float ConstantRangeBegin
        {
            get { return mConstantRangeBegin; }
            set
            {
                mConstantRangeBegin = value;
                unsafe
                {
                    DllImportAPI.v3dScalarVariable_SetValueBegin(ScalarVariablePtr, value);
                }
            }
        }

        float mConstantRangeEnd = 0;
        /// <summary>
        /// 常量的结束范围
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        public float ConstantRangeEnd
        {
            get { return mConstantRangeEnd; }
            set
            {
                mConstantRangeEnd = value;
                unsafe
                {
                    DllImportAPI.v3dScalarVariable_SetValueEnd(ScalarVariablePtr, value);
                }
            }
        }

        CSUtility.Support.V3dxBezier2DWrapper mBezier2DCurve = new CSUtility.Support.V3dxBezier2DWrapper();
        /// <summary>
        /// 2D下的贝塞尔曲线
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]        
        public CSUtility.Support.V3dxBezier2DWrapper Bezier2DCurve
        {
            get { return mBezier2DCurve; }
            set
            {
                mBezier2DCurve = value;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ScalarVariable()
        {
            //mScalarVariablePtr = IDllImportAPI.v3dScalarVariable_New();
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="value">赋值给常量值</param>
        public ScalarVariable(float value)
        {
            //mScalarVariablePtr = IDllImportAPI.v3dScalarVariable_New();
            mConstantValue = value;
        }
        /// <summary>
        /// 带两个参数的构造函数
        /// </summary>
        /// <param name="bgn">常量的起始点</param>
        /// <param name="end">常量的终点</param>
        public ScalarVariable(float bgn, float end)
        {
            //mScalarVariablePtr = IDllImportAPI.v3dScalarVariable_New();
            mConstantRangeBegin = bgn;
            mConstantRangeEnd = end;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~ScalarVariable()
        {
            //IDllImportAPI.v3dScalarVariable_Delete(ScalarVariablePtr);
        }
        /// <summary>
        /// 是否可以转变类型
        /// </summary>
        /// <param name="type">转换后的类型</param>
        /// <returns>如果可以转换返回true，否则返回false</returns>
        public bool CanChangeToType(enVariableType type)
        {
            unsafe
            {
                return ((DllImportAPI.v3dScalarVariable_CanChangeToType(mScalarVariablePtr, (int)type)) != 0 ? true : false);
            }
        }
        /// <summary>
        /// 读取XND文件
        /// </summary>
        /// <param name="xndAtt">XND文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (!base.Read(xndAtt))
                return false;

            if (VariableType == enVariableType.Curve)
            {
                var ptr = DllImportAPI.v3dScalarVariable_GetBezierPtr(mScalarVariablePtr);
                Bezier2DCurve.SetDataToIntptr(ptr);
                Bezier2DCurve.BezierPtr = ptr;
            }

            return true;
        }
        /// <summary>
        /// 设置所有常量的值
        /// </summary>
        /// <param name="start">常量的起点及其值</param>
        /// <param name="end">常量的终点</param>
        public void SetAllValue(float start, float end)
        {
            ConstantValue = start;
            ConstantRangeBegin = start;
            ConstantRangeEnd = end;
        }
        /// <summary>
        /// 得到变量的值
        /// </summary>
        /// <param name="t">变量值</param>
        /// <returns>返回标量标量变量的值</returns>
        public float GetValue(float t)
        {
            return DllImportAPI.v3dScalarVariable_GetValue(ScalarVariablePtr, t);
        }
        /// <summary>
        /// 得到随机数值
        /// </summary>
        /// <returns>返回随机变量值</returns>
        public float GetRandomValue()
        {
            return DllImportAPI.v3dScalarVariable_GetRandomValue(ScalarVariablePtr);
        }
        /// <summary>
        /// 根据指针地址获取值
        /// </summary>
        /// <param name="ptr">对象指针</param>
        public void GetValueFromIntptr(IntPtr ptr)
        {
            mVariableType = (enVariableType)DllImportAPI.v3dScalarVariable_GetVariableType(ptr);
            mConstantValue = DllImportAPI.v3dScalarVariable_GetValue(ptr, 0);
            mConstantRangeBegin = DllImportAPI.v3dScalarVariable_GetValueBegin(ptr);
            mConstantRangeEnd = DllImportAPI.v3dScalarVariable_GetValueEnd(ptr);

            if (mVariableType == enVariableType.Curve)
            {
                Bezier2DCurve = new CSUtility.Support.V3dxBezier2DWrapper();
                var bezierPtr = DllImportAPI.v3dScalarVariable_GetBezierPtr(ptr);
                Bezier2DCurve.GetDataFromIntptr(bezierPtr);
                Bezier2DCurve.BezierPtr = bezierPtr;
            }
        }
        /// <summary>
        /// 给指针赋值
        /// </summary>
        /// <param name="ptr">对象指针</param>
        public void SetValueToIntptr(IntPtr ptr)
        {
            DllImportAPI.v3dScalarVariable_SetVariableType(ScalarVariablePtr, (int)VariableType);
            DllImportAPI.v3dScalarVariable_SetValue(ScalarVariablePtr, ConstantValue);
            DllImportAPI.v3dScalarVariable_SetValueBegin(ScalarVariablePtr, ConstantRangeBegin);
            DllImportAPI.v3dScalarVariable_SetValueEnd(ScalarVariablePtr, ConstantRangeEnd);

            DllImportAPI.v3dScalarVariable_SetValues(ScalarVariablePtr, (int)VariableType, ConstantValue, ConstantRangeBegin, ConstantRangeEnd);

            if (VariableType == enVariableType.Curve)
            {
                var bezierPtr = DllImportAPI.v3dScalarVariable_GetBezierPtr(ptr);
                Bezier2DCurve.SetDataToIntptr(bezierPtr);
                Bezier2DCurve.BezierPtr = bezierPtr;
            }
        }
    }
}
