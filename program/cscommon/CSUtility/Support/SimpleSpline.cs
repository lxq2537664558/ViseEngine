using System;
using System.Collections.Generic;

namespace CSUtility.Support
{
    public class SimpleSpline
    {
        protected bool mAutoCalc = true;
        protected List<SlimDX.Vector3> mPoints = new List<SlimDX.Vector3>();
        public List<SlimDX.Vector3> Points
        {
            get { return mPoints; }
        }
        protected List<SlimDX.Vector3> mTangents = new List<SlimDX.Vector3>();
        protected SlimDX.Matrix mCoeffs;

        public SimpleSpline()
        {
            mCoeffs.M11 = 2;
            mCoeffs.M12 = -2;
            mCoeffs.M13 = 1;
            mCoeffs.M14 = 1;
            mCoeffs.M21 = -3;
            mCoeffs.M22 = 3;
            mCoeffs.M23 = -2;
            mCoeffs.M24 = -1;
            mCoeffs.M31 = 0;
            mCoeffs.M32 = 0;
            mCoeffs.M33 = 1;
            mCoeffs.M34 = 0;
            mCoeffs.M41 = 1;
            mCoeffs.M42 = 0;
            mCoeffs.M43 = 0;
            mCoeffs.M44 = 0;
        }

        /** Adds a control point to the end of the spline. */
        public void AddPoint(ref SlimDX.Vector3 p)
        {
            mPoints.Add(p);
            if (mAutoCalc)
                RecalcTangents();
        }

        public void InsertPoint(int index, ref SlimDX.Vector3 p)
        {
            mPoints.Insert(index, p);
            if (mAutoCalc)
                RecalcTangents();
        }

        public void RemovePoint(int index)
        {
            System.Diagnostics.Debug.Assert(index < mPoints.Count && index >= 0, "Point index is out of bounds!!");

            mPoints.RemoveAt(index);
            if (mAutoCalc)
                RecalcTangents();
        }

        /** Gets the detail of one of the control points of the spline. */
        public SlimDX.Vector3 GetPoint(UInt16 index)
        {
            System.Diagnostics.Debug.Assert(index < mPoints.Count, "Point index is out of bounds!!");
            return mPoints[index];
        }

        /** Gets the number of control points in the spline. */
        public UInt16 GetNumPoints()
        {
            return (UInt16)mPoints.Count;
        }

        /** Clears all the points in the spline. */
        public void Clear()
        {
            mPoints.Clear();
            mTangents.Clear();
        }

        /** Updates a single point in the spline. 
        @remarks
            This point must already exist in the spline.
        */
        public void UpdatePoint(UInt16 index, ref SlimDX.Vector3 value)
        {
            System.Diagnostics.Debug.Assert(index < mPoints.Count, "Point index is out of bounds!!");
            mPoints[index] = value;
            if (mAutoCalc)
                RecalcTangents();
        }
        
        /** Returns an interpolated point based on a parametric value over the whole series.
        @remarks
            Given a t value between 0 and 1 representing the parametric distance along the
            whole length of the spline, this method returns an interpolated point.
        @param t Parametric value.
        */
        public SlimDX.Vector3 Interpolate(float t)
        {
            float fSeg = t * (mPoints.Count - 1);
            UInt32 segIdx = (UInt32)fSeg;
            t = fSeg - segIdx;

            return Interpolate(segIdx, t);
        }

        /** Interpolates a single segment of the spline given a parametric value.
        @param fromIndex The point index to treat as t=0. fromIndex + 1 is deemed to be t=1
        @param t Parametric value
        */
        public SlimDX.Vector3 Interpolate(UInt32 fromIndex, float t)
        {
            System.Diagnostics.Debug.Assert(fromIndex < mPoints.Count, "Point index is out of bounds!!");

            if ((fromIndex + 1) == mPoints.Count)
            {
                // Duff request, cannot blend to nothing
                // Just return source
                return mPoints[(int)fromIndex];
            }

            // Fast special cases
            if (t == 0.0f)
                return mPoints[(int)fromIndex];
            else if (t == 1.0f)
                return mPoints[(int)fromIndex + 1];

            // Real interpolation
            // Form a vector of powers of t
            float t2, t3;
            t2 = t * t;
            t3 = t2 * t;
            SlimDX.Vector4 powers;
            powers.X = t3;
            powers.Y = t2;
            powers.Z = t;
            powers.W = 1;

            // Algorithm is ret = powers * mCoeffs * Matrix4(point1, point2, tangent1, tangent2)
            SlimDX.Vector3 point1 = mPoints[(int)fromIndex];
            SlimDX.Vector3 point2 = mPoints[(int)fromIndex + 1];
            SlimDX.Vector3 tan1 = mTangents[(int)fromIndex];
            SlimDX.Vector3 tan2 = mTangents[(int)fromIndex + 1];
            SlimDX.Matrix pt;

            pt.M11 = point1.X;
            pt.M12 = point1.Y;
            pt.M13 = point1.Z;
            pt.M14 = 1.0f;
            pt.M21 = point2.X;
            pt.M22 = point2.Y;
            pt.M23 = point2.Z;
            pt.M24 = 1.0f;
            pt.M31 = tan1.X;
            pt.M32 = tan1.Y;
            pt.M33 = tan1.Z;
            pt.M34 = 1.0f;
            pt.M41 = tan2.X;
            pt.M42 = tan2.Y;
            pt.M43 = tan2.Z;
            pt.M44 = 1.0f;

            SlimDX.Matrix tempM = mCoeffs * pt;
            SlimDX.Vector4 ret = SlimDX.Vector4.Transform(powers, tempM);

            return new SlimDX.Vector3(ret.X, ret.Y, ret.Z);
        }

        /** Tells the spline whether it should automatically calculate tangents on demand
            as points are added.
        @remarks
            The spline calculates tangents at each point automatically based on the input points.
            Normally it does this every time a point changes. However, if you have a lot of points
            to add in one go, you probably don't want to incur this overhead and would prefer to 
            defer the calculation until you are finished setting all the points. You can do this
            by calling this method with a parameter of 'false'. Just remember to manually call 
            the recalcTangents method when you are done.
        @param autoCalc If true, tangents are calculated for you whenever a point changes. If false, 
            you must call reclacTangents to recalculate them when it best suits.
        */
        public void SetAutoCalculate(bool autoCalc)
        {
            mAutoCalc = autoCalc;
        }

        /** Recalculates the tangents associated with this spline. 
        @remarks
            If you tell the spline not to update on demand by calling setAutoCalculate(false)
            then you must call this after completing your updates to the spline points.
        */
        public void RecalcTangents()
        {
            // Catmull-Rom approach
            // 
            // tangent[i] = 0.5 * (point[i+1] - point[i-1])
            //
            // Assume endpoint tangents are parallel with line with neighbour

            int numPoints;
            bool isClosed;

            numPoints = mPoints.Count;
            if (numPoints < 2)
            {
                // Can't do anything yet
                return;
            }

            // Closed or open?
            if (mPoints[0] == mPoints[numPoints - 1])
                isClosed = true;
            else
                isClosed = false;

            mTangents.Clear();
            for (int i = 0; i < numPoints; i++ )
            {
                mTangents.Add(new SlimDX.Vector3());
            }

            for (int i = 0; i < numPoints; ++i)
            {
                if (i == 0)
                {
                    if (isClosed)
                        mTangents[i] = 0.5f * (mPoints[1] - mPoints[numPoints - 2]);
                    else
                        mTangents[i] = 0.5f * (mPoints[1] - mPoints[0]);
                }
                else if (i == numPoints - 1)
                {
                    if (isClosed)
                        mTangents[i] = mTangents[0];
                    else
                        mTangents[i] = 0.5f * (mPoints[i] - mPoints[i - 1]);
                }
                else
                {
                    mTangents[i] = 0.5f * (mPoints[i + 1] - mPoints[i - 1]);
                }
            }
        }
    }
}
