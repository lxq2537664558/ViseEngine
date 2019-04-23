using System;
using System.Collections.Generic;

namespace CSUtility.Support
{
    public class V3dxBezierWrapper : CSUtility.Support.XndSaveLoadProxy
    {
        public class sBezierPoint : CSUtility.Support.XndSaveLoadProxy
        {
            SlimDX.Vector3 mPos = new SlimDX.Vector3();
            [CSUtility.Support.AutoSaveLoad]   
            public SlimDX.Vector3 vPos          // 点位置
            {
                get { return mPos; }
                set { mPos = value; }
            }
            SlimDX.Vector3 mCtrlPos1 = new SlimDX.Vector3();
            [CSUtility.Support.AutoSaveLoad]
            public SlimDX.Vector3 vCtrlPos1     // 控制点1位置(相对于vPos位置)
            {
                get { return mCtrlPos1; }
                set { mCtrlPos1 = value; }
            }
            SlimDX.Vector3 mCtrlPos2 = new SlimDX.Vector3();
            [CSUtility.Support.AutoSaveLoad]
            public SlimDX.Vector3 vCtrlPos2     // 控制点2位置(相对于vPos位置)
            {
                get { return mCtrlPos2; }
                set { mCtrlPos2 = value; }
            }

            public void Clone(sBezierPoint src)
            {
                vPos = src.vPos;
                vCtrlPos1 = src.vCtrlPos1;
                vCtrlPos2 = src.vCtrlPos2;
            }
        }

        //IntPtr mBezierPtr; 
        public IntPtr BezierPtr;     // v3dxBezier(生存期由外部管理)
        //{
        //    get { return mBezierPtr; }
        //    set { mBezierPtr = value; }
        //}
        List<sBezierPoint> mBezierPointList = new List<sBezierPoint>();
        [CSUtility.Support.AutoSaveLoad]
        public List<sBezierPoint> BezierPointList
        {
            get { return mBezierPointList; }
            set { mBezierPointList = value; }
        }

        ~V3dxBezierWrapper()
        {
            BezierPtr = IntPtr.Zero;
        }

        public virtual void Clone(V3dxBezierWrapper src)
        {
            BezierPointList.Clear();
            foreach(var srcP in src.BezierPointList)
            {
                var p = new sBezierPoint();
                p.Clone(srcP);
                BezierPointList.Add(p);
            }
        }

        public void GetDataFromIntptr(IntPtr ptr)
        {
            unsafe
            {
                BezierPointList.Clear();

                var count = DllImportAPI.v3dxBezier_GetNodesCount(ptr);
                for (int i = 0; i < count; i++)
                {
                    sBezierPoint bPt = new sBezierPoint();

                    SlimDX.Vector3 v = new SlimDX.Vector3();
                    DllImportAPI.v3dxBezier_GetPosition(ptr, i, (IntPtr)(&v));
                    bPt.vPos = v;

                    DllImportAPI.v3dxBezier_GetControlPos1(ptr, i, (IntPtr)(&v));
                    bPt.vCtrlPos1 = v;

                    DllImportAPI.v3dxBezier_GetControlPos2(ptr, i, (IntPtr)(&v));
                    bPt.vCtrlPos2 = v;

                    BezierPointList.Add(bPt);
                }
            }
        }

        private void GetDataFromIntptr(int idx, IntPtr ptr)
        {
            if (idx < 0 || idx >= mBezierPointList.Count)
                return;

            unsafe
            {
                SlimDX.Vector3 v = new SlimDX.Vector3();
                DllImportAPI.v3dxBezier_GetPosition(ptr, idx, (IntPtr)(&v));
                mBezierPointList[idx].vPos = v;

                DllImportAPI.v3dxBezier_GetControlPos1(ptr, idx, (IntPtr)(&v));
                mBezierPointList[idx].vCtrlPos1 = v;

                DllImportAPI.v3dxBezier_GetControlPos2(ptr, idx, (IntPtr)(&v));
                mBezierPointList[idx].vCtrlPos2 = v;
            }
        }

        public void SetDataToIntptr(IntPtr ptr)
        {
            unsafe
            {
                DllImportAPI.v3dxBezier_ClearNodes(ptr);
                int i=0;
                foreach (var pt in BezierPointList)
                {
                    var vPos = pt.vPos;
                    var vCtrl1 = pt.vCtrlPos1;
                    var vCtrl2 = pt.vCtrlPos2;
                    DllImportAPI.v3dxBezier_InsertNode(ptr, i, (IntPtr)(&vPos), (IntPtr)(&vCtrl1), (IntPtr)(&vCtrl2));
                    i++;
                }
            }
        }

        public void InsertNode(int idx, SlimDX.Vector3 pos, SlimDX.Vector3 ctrlPos1, SlimDX.Vector3 ctrlPos2)
        {
            unsafe
            {
                DllImportAPI.v3dxBezier_InsertNode(BezierPtr, idx, (IntPtr)(&pos), (IntPtr)(&ctrlPos1), (IntPtr)(&ctrlPos2));
            }
            GetDataFromIntptr(BezierPtr);
        }

        public void DeleteNode(int idx)
        {
            unsafe
            {
                DllImportAPI.v3dxBezier_DeleteNode(BezierPtr, idx);
            }
            GetDataFromIntptr(BezierPtr);
        }

        // fTime范围0-1
        public SlimDX.Vector3 GetValue(float fTime)
        {
            unsafe
            {
                SlimDX.Vector3 v;
                DllImportAPI.v3dxBezier_GetValue(BezierPtr, fTime, (IntPtr)(&v));
                return v;
            }
        }

        public SlimDX.Vector3 GetPosition(int idx)
        {
            unsafe
            {
                SlimDX.Vector3 v;
                DllImportAPI.v3dxBezier_GetPosition(BezierPtr, idx, (IntPtr)(&v));
                return v;
            }
        }
        public void SetPosition(int idx, SlimDX.Vector3 pos)
        {
            unsafe
            {
                DllImportAPI.v3dxBezier_SetPosition(BezierPtr, idx, (IntPtr)(&pos));
            }
            GetDataFromIntptr(idx, BezierPtr);
        }

        public SlimDX.Vector3 GetControlPos1(int idx)
        {
            unsafe
            {
                SlimDX.Vector3 v;
                DllImportAPI.v3dxBezier_GetControlPos1(BezierPtr, idx, (IntPtr)(&v));
                return v;
            }
        }
        public void SetControlPos1(int idx, SlimDX.Vector3 pos, bool bWithPos2)
        {
            unsafe
            {
                DllImportAPI.v3dxBezier_SetControlPos1(BezierPtr, idx, (IntPtr)(&pos), bWithPos2);
            }
            GetDataFromIntptr(idx, BezierPtr);
        }

        public SlimDX.Vector3 GetControlPos2(int idx)
        {
            unsafe
            {
                SlimDX.Vector3 v;
                DllImportAPI.v3dxBezier_GetControlPos2(BezierPtr, idx, (IntPtr)(&v));
                return v;
            }
        }
        public void SetControlPos2(int idx, SlimDX.Vector3 pos, bool bWithPos1)
        {
            unsafe
            {
                DllImportAPI.v3dxBezier_SetControlPos2(BezierPtr, idx, (IntPtr)(&pos), bWithPos1);
            }
            GetDataFromIntptr(idx, BezierPtr);
        }

        public int GetNodesCount()
        {
            unsafe
            {
                return DllImportAPI.v3dxBezier_GetNodesCount(BezierPtr);
            }
        }

        public static IntPtr NewV3dxBezierPtr()
        {
            unsafe
            {
                return DllImportAPI.v3dxBezier_New();
            }
        }

        public static void DeleteV3dxBezierPtr(IntPtr ptr)
        {
            unsafe
            {
                DllImportAPI.v3dxBezier_Delete(ptr);
            }
        }

        //public override bool Read(CSUtility.Support.IXndAttrib xndAtt)
        //{
        //    if (!base.Read(xndAtt))
        //        return false;



        //    return true;
        //}
    }

    public class V3dxBezier2DWrapper : V3dxBezierWrapper
    {
        // 对应的Intptr为v3dxBezier2D
        public float GetValueY(float fTime)
        {
            unsafe
            {
                return DllImportAPI.v3dxBezier2D_GetValueY(BezierPtr, fTime);
            }
        }
    }
}
