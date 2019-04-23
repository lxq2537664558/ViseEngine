using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainEditor
{
    public partial class WorldEditorOperation
    {
        // 检查鼠标与场景的碰撞并返回碰撞点
        SlimDX.Vector3 IntersectWithScene(int x, int y)
        {
            SlimDX.Vector3 start = mREnviroment.Camera.GetLocation();
            SlimDX.Vector3 end = start + mREnviroment.Camera.GetPickDirection(x, y) * 1000.0F;

            var result = new CSUtility.Support.stHitResult();
            result.mHitFlags |= (uint)CSUtility.enHitFlag.HitMeshTriangle;
            if (CCore.Client.MainWorldInstance.LineCheck(ref start, ref end, ref result))
                return result.mHitPosition;

            return IntersectWithZeroPlane(x, y, SlimDX.Vector3.Zero, SlimDX.Vector3.UnitY);
        }

        // 检测鼠标与地形的碰撞
        SlimDX.Vector3 IntersectWithTerrain(int x, int y, bool withDeletePatch)
        {
            if (CCore.Client.MainWorldInstance.Terrain != null)
            {
                SlimDX.Vector3 start = mREnviroment.Camera.GetLocation();
                SlimDX.Vector3 end = start + mREnviroment.Camera.GetPickDirection(x, y) * 1000.0F;

                var result = new CSUtility.Support.stHitResult();
                if (CCore.Client.MainWorldInstance.Terrain.LineCheck(ref start, ref end, ref result, withDeletePatch))
                    return result.mHitPosition;
            }

            return IntersectWithZeroPlane(x, y, SlimDX.Vector3.Zero, SlimDX.Vector3.UnitY);
        }

        // 检测鼠标与标准平面的碰撞
        SlimDX.Vector3 IntersectWithZeroPlane(int x, int y, SlimDX.Vector3 planePoint, SlimDX.Vector3 planeNormal)
        {
            SlimDX.Vector3 start = mREnviroment.Camera.GetLocation();
            SlimDX.Vector3 dir = mREnviroment.Camera.GetPickDirection(x, y);

            var t = (SlimDX.Vector3.Dot(planeNormal, planePoint) - SlimDX.Vector3.Dot(planeNormal, start)) / SlimDX.Vector3.Dot(planeNormal, dir);
            return start + dir * t;
        }

        // 检测鼠标与标准平面的碰撞
        SlimDX.Vector3 IntersectWithZeroPlane(int x, int y, CCore.Support.V3DAxis.enAxisType axisType)
        {
            SlimDX.Vector3 start = mREnviroment.Camera.GetLocation();
            SlimDX.Vector3 dir = mREnviroment.Camera.GetPickDirection(x, y);
            SlimDX.Vector3 axisLoc = m3dAxis.Placement.GetLocation();
            var vPlaneNormal = SlimDX.Vector3.Zero;

            switch (axisType)
            {
                // XZ平面
                case CCore.Support.V3DAxis.enAxisType.Move_X:
                case CCore.Support.V3DAxis.enAxisType.Move_Z:
                case CCore.Support.V3DAxis.enAxisType.Move_XZ:
                case CCore.Support.V3DAxis.enAxisType.Rot_X:
                case CCore.Support.V3DAxis.enAxisType.Rot_Z:
                case CCore.Support.V3DAxis.enAxisType.Scale_X:
                case CCore.Support.V3DAxis.enAxisType.Scale_Z:
                case CCore.Support.V3DAxis.enAxisType.Scale_XZ:
                    vPlaneNormal = SlimDX.Vector3.UnitY;
                    break;

                // XY平面
                case CCore.Support.V3DAxis.enAxisType.Move_Y:
                case CCore.Support.V3DAxis.enAxisType.Rot_Y:
                case CCore.Support.V3DAxis.enAxisType.Scale_Y:
                case CCore.Support.V3DAxis.enAxisType.Move_XY:
                case CCore.Support.V3DAxis.enAxisType.Scale_XY:
                    vPlaneNormal = SlimDX.Vector3.UnitZ;
                    break;

                // YZ平面
                case CCore.Support.V3DAxis.enAxisType.Move_YZ:
                case CCore.Support.V3DAxis.enAxisType.Scale_YZ:
                    vPlaneNormal = SlimDX.Vector3.UnitX;
                    break;
            }

            var t = (SlimDX.Vector3.Dot(vPlaneNormal, axisLoc) - SlimDX.Vector3.Dot(vPlaneNormal, start)) / SlimDX.Vector3.Dot(vPlaneNormal, dir);
            return start + dir * t;
        }
    }
}
