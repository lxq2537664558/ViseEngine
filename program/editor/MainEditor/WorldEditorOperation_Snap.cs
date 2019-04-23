using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainEditor
{
    public partial class WorldEditorOperation
    {
        private bool GetNearstSnapPosition(SlimDX.Vector3 vLoc, ref SlimDX.Vector3 vTag, CSUtility.Support.ThreadSafeObservableCollection<CCore.World.Actor> SrcActors)
        {
            bool retValue = true;
            vTag = vLoc;

            bool snapFromFace = false;
            var srcActorList = SrcActors.ToList();
            bool exceptSrcActor = false;
            if (m3dAxis.OperationType == CCore.Support.V3DAxis.enOperationType.MoveObject)
                exceptSrcActor = true;
            var nearestLoc = SlimDX.Vector3.Zero;
            float minDistance = Single.MaxValue;
            var fMaxCheckDistance = GameAssistWindow.Instance.SnapRange;

            if (GameAssistWindow.Instance.SnapFace)
            {
                var camPos = FreeCameraController.Camera.Location;
                var endPos = vLoc - camPos;
                endPos.Normalize();
                endPos = endPos * 1000 + camPos;
                var hitResult = new CSUtility.Support.stHitResult();
                hitResult.mHitFlags |= (UInt32)CSUtility.enHitFlag.HitMeshTriangle;
                if(exceptSrcActor)
                {
                    if(CCore.Client.MainWorldInstance.LineCheck(ref camPos, ref endPos, ref hitResult, srcActorList))
                    {
                        vLoc = hitResult.mHitPosition;
                        vTag = hitResult.mHitPosition;
                        retValue = true;
                        snapFromFace = true;
                    }
                }
                else
                {
                    if(CCore.Client.MainWorldInstance.LineCheck(ref camPos, ref endPos, ref hitResult))
                    {
                        vLoc = hitResult.mHitPosition;
                        vTag = hitResult.mHitPosition;
                        retValue = true;
                        snapFromFace = true;
                    }
                }
            }

            if(GameAssistWindow.Instance.SnapOrigin || GameAssistWindow.Instance.SnapVertex)
            {
                var vStart = vLoc + new SlimDX.Vector3(-fMaxCheckDistance - 1, -10000, -fMaxCheckDistance - 1);
                var vEnd = vLoc + new SlimDX.Vector3(fMaxCheckDistance + 1, 10000, fMaxCheckDistance + 1);
                var commonActors = CCore.Client.MainWorldInstance.GetActors(ref vStart, ref vEnd, (UInt16)CSUtility.Component.EActorGameType.Common);//.GetVisualActors();
                foreach (var actor in commonActors)
                {
                    if (actor == m3dAxis)
                        continue;

                    if (exceptSrcActor && SrcActors.Contains(actor))
                        continue;

                    if (actor.Visual != null)
                    {
                        if (GameAssistWindow.Instance.SnapOrigin)
                        {
                            var origion = actor.Placement.GetLocation();
                            float fDis = (vLoc - origion).Length();
                            if (fDis < minDistance && fDis < fMaxCheckDistance)
                            {
                                minDistance = fDis;
                                retValue = true;
                                vTag = origion;
                                snapFromFace = false;
                            }
                        }

                        if (actor.Visual is CCore.Mesh.Mesh && GameAssistWindow.Instance.SnapVertex)
                        {
                            var actMesh = actor.Visual as CCore.Mesh.Mesh;
                            SlimDX.Matrix transMatrix = actor.Placement.mMatrix;

                            if (actMesh.GetNearestVertexPos(ref vLoc, ref nearestLoc, ref transMatrix, fMaxCheckDistance))
                            {
                                float fDis = (vLoc - nearestLoc).Length();
                                if (fDis < minDistance)
                                {
                                    minDistance = fDis;
                                    retValue = true;
                                    vTag = nearestLoc;
                                    snapFromFace = false;
                                }
                            }
                        }
                    }
                }
            }

            if(GameAssistWindow.Instance.SnapTile)
            {
                bool bRevise = true;
                if (retValue == false)
                {
                    vTag = vLoc;
                    retValue = true;
                }
                else
                {
                    if (!snapFromFace)
                        bRevise = false;
                }
                var tileInterval = GameAssistWindow.Instance.TileDensity;       // 网格间隔，从编辑器里取得

                if (bRevise)
                {
                    switch (m3dAxis.AxisType)
                    {
                        case CCore.Support.V3DAxis.enAxisType.Move_X:
                            break;

                        case CCore.Support.V3DAxis.enAxisType.Move_Y:
                            break;

                        case CCore.Support.V3DAxis.enAxisType.Move_Z:
                            break;
                    }

                    if (m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_X ||
                       m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_XY ||
                       m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_XZ ||
                       m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_XYZ)
                        vTag.X = (int)(vTag.X / tileInterval) * tileInterval;

                    if (m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_Y ||
                       m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_XY ||
                       m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_YZ ||
                       m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_XYZ)
                        vTag.Y = (int)(vTag.Y / tileInterval) * tileInterval;

                    if (m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_Z ||
                       m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_XZ ||
                       m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_YZ ||
                       m3dAxis.AxisType == CCore.Support.V3DAxis.enAxisType.Move_XYZ)
                        vTag.Z = (int)(vTag.Z / tileInterval) * tileInterval;
                }
            }

            return retValue;
        }
    }
}
