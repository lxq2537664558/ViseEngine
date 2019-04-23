using System;

namespace CCore.Mesh
{
    public partial class Mesh
    {
        CSUtility.Support.ConcurentObjManager<Guid, CCore.Socket.ISocketComponent> mSocketComponents = new CSUtility.Support.ConcurentObjManager<Guid, Socket.ISocketComponent>();
        /// <summary>
        /// 只读属性，获取mesh的挂接组件
        /// </summary>
        public CSUtility.Support.ConcurentObjManager<Guid, CCore.Socket.ISocketComponent> SocketComponents
        {
            get { return mSocketComponents; }
        }
        /// <summary>
        /// 清空挂接组件
        /// </summary>
        public void ClearSocketItem()
        {
            mSocketComponents.Clear();
        }
        /// <summary>
        /// 添加挂接成员
        /// </summary>
        /// <param name="info">挂接成员对象</param>
        /// <returns>返回添加的挂接成员</returns>
        public CCore.Socket.ISocketComponent AddSocketItem(CCore.Socket.ISocketComponentInfo info)
        {
            if (info == null)
                return null;

            var copyedInfo = System.Activator.CreateInstance(info.GetType()) as CCore.Socket.ISocketComponentInfo;
            copyedInfo.CopyComponentInfoFrom(info);
            copyedInfo.SocketComponentInfoId = info.SocketComponentInfoId;
            var socketComponent = System.Activator.CreateInstance(info.GetSocketComponentType()) as CCore.Socket.ISocketComponent;
            socketComponent.ComponentHostMesh = this;
            socketComponent.InitializeSocketComponent(copyedInfo);

            var socketComp = mSocketComponents.FindObj(socketComponent.SocketComponentInfo.SocketComponentInfoId);
            mSocketComponents.Remove(socketComponent.SocketComponentInfo.SocketComponentInfoId);
            socketComp?.Cleanup();
            mSocketComponents[socketComponent.SocketComponentInfo.SocketComponentInfoId] = socketComponent;

            UpdateBoundingBox();

            return socketComponent;
        }
        /// <summary>
        /// 是否包括某一名称的挂接成员
        /// </summary>
        /// <param name="socketName">挂接成员的名称</param>
        /// <returns>含有该挂接成员返回true，否则返回false</returns>
        public bool HasSocketItem(string socketName)
        {
            var tempName = socketName.ToLower();
            bool bHave = false;
            mSocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent item, object arg) =>
            {
                if (item?.SocketComponentInfo?.SocketName?.ToLower() == tempName)
                {
                    bHave = true;
                    return CSUtility.Support.EForEachResult.FER_Stop;
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            return bHave;
        }
        /// <summary>
        /// 根据名称获取挂接成员
        /// </summary>
        /// <param name="socketName">挂接成员的名称</param>
        /// <returns>返回与名称相对应的挂接成员</returns>
        public CCore.Socket.ISocketComponent GetSocketItem(string socketName)
        {
            var tempName = socketName.ToLower();
            CCore.Socket.ISocketComponent bHave = null;
            mSocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent item, object arg) =>
            {
                if (item?.SocketComponentInfo?.SocketName?.ToLower() == tempName)
                {
                    bHave = item;

                    return CSUtility.Support.EForEachResult.FER_Stop;
                }

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            return bHave;
        }

        //public CCore.Socket.SocketItem FindSocketItem(string socketName)
        //{
        //    var tempName = socketName.ToLower();
        //    CCore.Socket.SocketItem retSocketItem = null;
        //    mSocketItems.For_Each((Guid id, CCore.Socket.SocketItem item, object arg) =>
        //    {
        //        if (item?.Info?.SocketName?.ToLower() == tempName)
        //        {
        //            retSocketItem = item;
        //            return CSUtility.Support.EForEachResult.FER_Stop;
        //        }

        //        return CSUtility.Support.EForEachResult.FER_Continue;
        //    }, null);

        //    return retSocketItem;
        //}

        //public void RemoveSocketItem(string socketName)
        //{
        //    var tempName = socketName.ToLower();
        //    mSocketItems.For_Each((Guid id, CCore.Socket.SocketItem item, object arg) =>
        //    {
        //        if (item?.Info?.SocketName?.ToLower() == tempName)
        //        {
        //            return CSUtility.Support.EForEachResult.FER_Erase;
        //        }

        //        return CSUtility.Support.EForEachResult.FER_Continue;
        //    }, null);
        //}
        /// <summary>
        /// 删除该ID的挂接成员
        /// </summary>
        /// <param name="id">挂接成员的ID</param>
        public void RemoveSocketItem(Guid id)
        {
            var comp = mSocketComponents.FindObj(id);
            mSocketComponents.Remove(id);
            comp?.Cleanup();
            UpdateBoundingBox();
        }

        //private void _OnActorEnterScene(CCore.World.IActor actor, CCore.Scene.ISceneGraph scene)
        //{
        //    // 播放Socket声音
        //    foreach (var socketItem in mSocketItems)
        //    {
        //        foreach (var sv in socketItem.SoundDatas)
        //        {
        //            if (sv == null || sv.AudioSourceData == null)
        //                continue;

        //            sv.AudioSourceData.Play();
        //        }
        //    }
        //}
        //private void _OnActorRemoveFromScene(CCore.World.IActor actor, CCore.Scene.ISceneGraph scene)
        //{
        //    // 停止播放Socket声音
        //    foreach (var socketItem in mSocketItems)
        //    {
        //        foreach (var sv in socketItem.SoundDatas)
        //        {
        //            if (sv == null)
        //                continue;

        //            sv.AudioSourceData.Stop();
        //        }
        //    }
        //}

        //public SlimDX.Matrix CommitSocketVisual(CCore.Component.IVisual socketVisual, CCore.Mesh.Mesh parentMesh, CCore.Socket.SocketItem socketItem, string socketName, SlimDX.Vector3 itemPos, SlimDX.Vector3 itemScale, SlimDX.Quaternion itemQuat,
        //    CCore.Graphics.REnviroment env, ref SlimDX.Matrix matrix, CCore.Camera.Camera eye, bool inheritRotate)
        //{
        //    SlimDX.Matrix result = SlimDX.Matrix.Identity;

        //    if (socketVisual != null && parentMesh != null)
        //    {
        //        if (socketVisual.Visible == false)
        //            return result;

        //        var scalRot = SlimDX.Quaternion.Identity;
        //        var weaponM = matrix;
        //        SlimDX.Vector3 parentPos = new SlimDX.Vector3();
        //        SlimDX.Vector3 parentScale = new SlimDX.Vector3();
        //        SlimDX.Quaternion parentQuat = new SlimDX.Quaternion();
        //        weaponM.Decompose(out parentScale, out parentQuat, out parentPos);

        //        var socket = parentMesh.GetSocket(socketName);
        //        if (socket != null)
        //        {
        //            socket.IsInheritRotate = socketItem.Info.InheritBoneRotate;

        //            SlimDX.Matrix socketMatrix = new SlimDX.Matrix();
        //            var scale = itemScale;
        //            SlimDX.Vector3 pos;
        //            SlimDX.Quaternion quat;
        //            if (inheritRotate == true)
        //            {
        //                var tempPos = SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.Modulate(socket.AbsPos, parentScale), parentQuat);
        //                SlimDX.Vector3 localPos = tempPos + itemPos;
        //                pos = parentPos + localPos;
        //                quat = itemQuat * socket.AbsQuat * parentQuat;
        //            }
        //            else
        //            {
        //                pos = parentPos + SlimDX.Vector3.Modulate(socket.AbsPos, parentScale) + itemPos;
        //                quat = SlimDX.Quaternion.Identity;
        //            }
        //            SlimDX.Matrix.Transformation(ref SlimDX.Vector3.Zero, ref scalRot, ref scale, ref SlimDX.Vector3.Zero, ref quat, ref pos, out socketMatrix);

        //            var effect = socketVisual as CCore.Component.EffectVisual;
        //            if (effect != null)
        //            {
        //                if (effect.EffectInit.InheritRotateWhenBorn == true)
        //                {
        //                    if (effect.mBorn == false)
        //                    {
        //                        effect.mBornMatrix = socketMatrix;
        //                        effect.mBorn = true;
        //                    }
        //                    socketMatrix = effect.mBornMatrix;
        //                }
        //            }

        //            weaponM = socketMatrix;
        //            socketVisual.Commit(env, ref weaponM, eye);

        //            result = weaponM;
        //        }
        //    }

        //    return result;
        //}

        /// <summary>
        /// 提交挂接成员到渲染环境
        /// </summary>
        /// <param name="env">渲染环境</param>
        /// <param name="matrix">该对象的位置矩阵</param>
        /// <param name="eye">视野</param>
        public void CommitSocketItem(CCore.Graphics.REnviroment env, SlimDX.Matrix matrix, CCore.Camera.CameraObject eye)
        {
            mSocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent item, object arg) =>
            {
                if (item == null || item.SocketComponentInfo == null)
                    return CSUtility.Support.EForEachResult.FER_Continue;
                var socket = GetSocket(item.SocketComponentInfo.SocketName);
                if (socket == null)
                    return CSUtility.Support.EForEachResult.FER_Continue;

                item.SocketComponentCommit(env, socket.AbsMatrix, matrix, eye);

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            //foreach (var socketItem in mSocketItems)
            //{
            //    var itemPos = socketItem.Info.Pos;
            //    var scale = socketItem.Info.Scale;
            //    var rotate = socketItem.Info.Rotate * (float)(System.Math.PI) / 180.0f;
            //    var itemQuat = SlimDX.Quaternion.RotationYawPitchRoll(rotate.Y, rotate.X, rotate.Z);
            //    SlimDX.Vector3 itemScale = scale * SlimDX.Vector3.UnitXYZ;
            //    var scalRot = SlimDX.Quaternion.Identity;
            //    SlimDX.Vector3 parentPos = new SlimDX.Vector3();
            //    SlimDX.Vector3 parentScale = new SlimDX.Vector3();
            //    SlimDX.Quaternion parentQuat = new SlimDX.Quaternion();
            //    matrix.Decompose(out parentScale, out parentQuat, out parentPos);

            //    foreach (var role in socketItem.RoleActors)
            //    {
            //        if (role == null)
            //            continue;
            //        var identityQuat = SlimDX.Quaternion.Identity;

            //        var socket = GetSocket(socketItem.Info.SocketName);
            //        if (socket != null)
            //        {
            //            var rolePos = role.RoleTemplate.SocketPos;
            //            var roleScale = role.RoleTemplate.SocketScale;
            //            var roleRotate = role.RoleTemplate.SocketRotate * (float)(System.Math.PI) / 180.0f;
            //            var roleQuat = SlimDX.Quaternion.RotationYawPitchRoll(roleRotate.Y, roleRotate.X, roleRotate.Z);

            //            var finalItemPos = itemPos + SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.Modulate(rolePos, itemScale), itemQuat);
            //            var finalItemQuat = roleQuat * itemQuat;
            //            var finalItemScale = SlimDX.Vector3.Modulate(itemScale, roleScale);

            //            socket.IsInheritRotate = socketItem.Info.InheritBoneRotate;

            //            SlimDX.Matrix socketMatrix = new SlimDX.Matrix();
            //            SlimDX.Vector3 pos;
            //            SlimDX.Quaternion quat;

            //            if (/*socketItem.Info.InheritRotate */role.RoleTemplate.SocketInheritRotate == true)
            //            {
            //                SlimDX.Vector3 localPos = finalItemPos + SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.Modulate(socket.AbsPos, parentScale), parentQuat);
            //                pos = parentPos + localPos;
            //                var fixquat = SlimDX.Quaternion.RotationYawPitchRoll((float)role.RoleTemplate.MeshFixAngle, 0, 0);
            //                quat = finalItemQuat * socket.AbsQuat * parentQuat * fixquat;
            //            }
            //            else
            //            {
            //                pos = parentPos + SlimDX.Vector3.Modulate(socket.AbsPos, parentScale) + finalItemPos;
            //                quat = SlimDX.Quaternion.Identity;
            //            }

            //            // 平滑过渡
            //            var currPos = role.Placement.GetLocation();
            //            var targetPos = pos;
            //            if (targetPos != currPos)
            //            {
            //                var delta = targetPos - currPos;
            //                var moveDir = SlimDX.Vector3.Normalize(delta);
            //                var length = delta.Length();
            //                if (length > 10)
            //                {
            //                    currPos = targetPos;
            //                }
            //                else if (length > 2)
            //                {
            //                    //var moveDist = role.MoveSpeed * 1 * MidLayer.IEngine.Instance.GetElapsedMillisecond()*0.001f;
            //                    //if (moveDist > length)
            //                    //    moveDist = length;
            //                    //currPos += moveDir * moveDist;
            //                    currPos = targetPos - moveDir * 1.98f;
            //                }
            //                else
            //                {
            //                    var moveDist = role.MoveSpeed * 0.5f * CCore.IEngine.Instance.GetElapsedMillisecond() * 0.001f;
            //                    if (moveDist > length)
            //                        moveDist = length;

            //                    currPos += moveDir * moveDist;
            //                    //if (currPos.X > targetPos.X)
            //                    //    currPos.X -= moveDist;
            //                    //else if (currPos.X < targetPos.X)
            //                    //    currPos.X += moveDist;
            //                    //if (System.Math.Abs(targetPos.X - currPos.X) < moveDist)
            //                    //    currPos.X = targetPos.X;

            //                    //if (currPos.Y > targetPos.Y)
            //                    //    currPos.Y -= moveDist;
            //                    //else if (currPos.Y < targetPos.Y)
            //                    //    currPos.Y += moveDist;
            //                    //if (System.Math.Abs(targetPos.Y - currPos.Y) < moveDist)
            //                    //    currPos.Y = targetPos.Y;

            //                    //if (currPos.Z > targetPos.Z)
            //                    //    currPos.Z -= moveDist;
            //                    //else if (currPos.Z < targetPos.Z)
            //                    //    currPos.Z += moveDist;
            //                    //if (System.Math.Abs(targetPos.Z - currPos.Z) < moveDist)
            //                    //    currPos.Z = targetPos.Z;
            //                }
            //            }
            //            pos = currPos;
            //            role.Placement.SetLocation(ref pos);

            //            SlimDX.Matrix.Transformation(ref SlimDX.Vector3.Zero, ref scalRot, ref itemScale, ref SlimDX.Vector3.Zero, ref quat, ref pos, out socketMatrix);

            //            //role.Placement.SetMatrix(ref socketMatrix);
            //            if (role.Visual != null)
            //            {
            //                role.Visual.Commit(env, ref socketMatrix, eye);
            //                role.LastRenderFrame = CCore.IEngine.Instance.CurRenderFrame;
            //            }
            //        }
            //    }

            //    foreach (var mesh in socketItem.Meshes)
            //    {
            //        if (mesh == null)
            //            continue;

            //        var meshInit = mesh.VisualInit as MeshInit;
            //        if (meshInit.MeshTemplate != null)
            //            itemScale = itemScale * meshInit.MeshTemplate.Scale;

            //        if (mesh.IsTrail == true)
            //        {
            //            mesh.EnableTrail = enableTrail;
            //            if (enableTrail == true)            // 刀光
            //            {
            //                var s1 = GetSocket(socketItem.Info.SocketName);
            //                var s2 = GetSocket(socketItem.Info.TargetSocketName);
            //                if (s1 == null || s2 == null)
            //                    continue;

            //                var absPos1 = SlimDX.Vector3.TransformCoordinate(s1.AbsPos, matrix);
            //                var absPos2 = SlimDX.Vector3.TransformCoordinate(s2.AbsPos, matrix);
            //                mesh.SetTrailSegment(absPos1, absPos2);

            //                var tempM = SlimDX.Matrix.Identity;
            //                mesh.Commit(env, ref tempM, eye);
            //                //CommitSocketVisual(mesh, this, socktItem.Key, itemPos, itemScale, itemQuat, env, ref tempM, eye);
            //            }
            //            else if (mesh.AutoSpawn == false)      // 链接法术
            //            {
            //                var tempM = SlimDX.Matrix.Identity;
            //                mesh.Commit(env, ref tempM, eye);
            //            }
            //        }
            //        else
            //        {
            //            var finalScale = new SlimDX.Vector3(itemScale.X * parentScale.X, itemScale.Y * parentScale.Y, itemScale.Z * parentScale.Z);
            //            var parentMatrix = CommitSocketVisual(mesh, this, socketItem, socketItem.Info.SocketName, itemPos, finalScale, itemQuat, env, ref matrix, eye, socketItem.Info.InheritRotate);
            //            mesh.CommitSocketItem(env, ref parentMatrix, eye, enableTrail);
            //        }
            //    }

            //    //foreach (var light in socktItem.Value.Info.Lights)
            //    //{
            //    //    var parentMatrix = CommitSocketVisual(light, this, socktItem.Key, scale, itemQuat, env, ref matrix, eye);
            //    //}

            //    foreach (var lightActor in socketItem.LightActors)
            //    {
            //        var lightMatrix = SlimDX.Matrix.Identity;
            //        lightActor.Placement.GetMatrix(out lightMatrix);
            //        SlimDX.Vector3 lightPos = new SlimDX.Vector3();
            //        SlimDX.Vector3 lightScale = new SlimDX.Vector3();
            //        SlimDX.Quaternion lightQuat = new SlimDX.Quaternion();
            //        lightMatrix.Decompose(out lightScale, out lightQuat, out lightPos);
            //        var finalM = CommitSocketVisual(lightActor.Visual, this, socketItem, socketItem.Info.SocketName, itemPos, lightScale * scale, lightQuat * itemQuat, env, ref matrix, eye, socketItem.Info.InheritRotate);
            //    }

            //    // 提交Particles
            //    foreach (var effect in socketItem.ParticleEffects)
            //    {
            //        if (effect == null)
            //            continue;
            //        if (effect.Visible == false)
            //            continue;

            //        SlimDX.Matrix effectM = matrix;
            //        //if (effect.EffectInit.InheritRotateWhenBorn == true)
            //        //{
            //        //    if (effect.mBorn == false)
            //        //    {
            //        //        effect.mBornMatrix = matrix;
            //        //        effect.mBorn = true;
            //        //    }
            //        //    effectM = effect.mBornMatrix;
            //        //}

            //        var ePos = effect.EffectInit.mPos;
            //        var eScale = effect.EffectInit.mScale;
            //        rotate = effect.EffectInit.mRotate * (float)(System.Math.PI) / 180.0f;
            //        var eQuat = SlimDX.Quaternion.RotationYawPitchRoll(rotate.Y, rotate.X, rotate.Z);

            //        var finalPos = itemPos + SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.Modulate(ePos, itemScale), itemQuat);
            //        var finalQuat = eQuat * itemQuat;
            //        var finalScale = itemScale * eScale;

            //        var parentMatrix = CommitSocketVisual(effect, this, socketItem, socketItem.Info.SocketName, finalPos, finalScale, finalQuat, env, ref effectM, eye, effect.EffectInit.InheritRotate);
            //    }

            //    // 提交SoundVisual
            //    foreach (var sv in socketItem.SoundDatas)
            //    {
            //        if (sv == null)
            //            continue;

            //        sv.Commit(env, ref matrix, eye);
            //    }
            //}
        }
        /// <summary>
        /// 提交挂接成员的阴影
        /// </summary>
        /// <param name="light">光源</param>
        /// <param name="matrix">挂接对象的位置矩阵</param>
        /// <param name="isDynamic">是否为动态的(动态静态阴影都实时更新，只是动态阴影更新频率较高)</param>
        public void CommitSocketShadow(CCore.Light.Light light, SlimDX.Matrix matrix, bool isDynamic)
        {
            mSocketComponents.For_Each((Guid id, Socket.ISocketComponent item, object arg) =>
            {
                if (item == null || item.SocketComponentInfo == null)
                    return CSUtility.Support.EForEachResult.FER_Continue;
                var socket = GetSocket(item.SocketComponentInfo.SocketName);
                if (socket == null)
                    return CSUtility.Support.EForEachResult.FER_Continue;

                item.SocketComponentCommitShadow(light, socket.AbsMatrix, matrix, isDynamic);

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);
        }
        /// <summary>
        /// 提交挂接的mesh高亮
        /// </summary>
        /// <param name="light">光源</param>
        /// <param name="socketMesh">挂接成员的mesh</param>
        /// <param name="parentMesh">父mesh</param>
        /// <param name="socketName">挂接成员的名称</param>
        /// <param name="itemPos">挂接成员的位置</param>
        /// <param name="itemScale">挂接成员的缩放值</param>
        /// <param name="itemQuat">挂接成员的四元数</param>
        /// <param name="matrix">挂接成员的位置矩阵</param>
        /// <param name="inheritRotate">是否跟随父对象旋转</param>
        /// <returns>返回计算后的挂接点的矩阵</returns>
        public SlimDX.Matrix CommitSocketMeshLighting(CCore.Light.Light light, CCore.Mesh.Mesh socketMesh, CCore.Mesh.Mesh parentMesh, string socketName, SlimDX.Vector3 itemPos, SlimDX.Vector3 itemScale, SlimDX.Quaternion itemQuat,
                ref SlimDX.Matrix matrix, bool inheritRotate)
        {
            SlimDX.Matrix result = SlimDX.Matrix.Identity;

            if (socketMesh != null && parentMesh != null)
            {
                var scalRot = SlimDX.Quaternion.Identity;
                var weaponM = matrix;
                SlimDX.Vector3 parentPos = new SlimDX.Vector3();
                SlimDX.Vector3 parentScale = new SlimDX.Vector3();
                SlimDX.Quaternion parentQuat = new SlimDX.Quaternion();
                weaponM.Decompose(out parentScale, out parentQuat, out parentPos);

                var socket = parentMesh.GetSocket(socketName);
                if (socket != null)
                {
                    SlimDX.Matrix socketMatrix = new SlimDX.Matrix();
                    var scale = new SlimDX.Vector3(itemScale.X * parentScale.X, itemScale.Y * parentScale.Y, itemScale.Z * parentScale.Z);
                    var tempPos = SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.Modulate(socket.AbsPos, parentScale), parentQuat);
                    SlimDX.Vector3 localPos = tempPos + itemPos;
                    var pos = parentPos + localPos;
                    SlimDX.Quaternion quat;
                    if (inheritRotate == true)
                    {
                        quat = itemQuat * socket.AbsQuat * parentQuat;
                    }
                    else
                    {
                        quat = SlimDX.Quaternion.Identity;
                    }
                    SlimDX.Matrix.Transformation(scale, quat, pos, out socketMatrix);

                    weaponM = socketMatrix;

                    unsafe
                    {
                        bool dynamic = false;
                        if (socketMesh.ShadingEnv == EShadingEnv.SDE_Deffered)
                        {
                            for (int j = 0; j < socketMesh.MeshParts.Count; ++j)
                            {
                                //if (mHostActor == null)
                                //    dynamic = false;
                                //else if (mHostActor.IsDynamic == true)
                                //    dynamic = true;
                                DllImportAPI.vLightProxy_CommitShadowMesh(light.Inner, CCore.Engine.Instance.GetFrameSecondTime(), socketMesh.MeshParts[j].Mesh, &weaponM, dynamic);
                            }
                        }
                    }

                    result = weaponM;
                }
            }

            return result;
        }
        /// <summary>
        /// 获取挂接件
        /// </summary>
        /// <param name="name">挂接件的名称</param>
        /// <returns>返回该名称的挂接件</returns>
        public CCore.Socket.Socket GetSocket(System.String name)
        {
            if (mFullSocketTable == null)
            {
                return null;
            }

            return mFullSocketTable.GetSocket(name);
        }
        /// <summary>
        /// 获取挂接件的AABB包围盒
        /// </summary>
        /// <param name="vMin"></param>
        /// <param name="vMax"></param>
        private void GetSocketAABB(out SlimDX.Vector3 vMin, out SlimDX.Vector3 vMax)
        {
            var refMin = new SlimDX.Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var refMax = new SlimDX.Vector3(float.MinValue, float.MinValue, float.MinValue);

            mSocketComponents.For_Each((Guid id, CCore.Socket.ISocketComponent item, object arg) =>
            {
                SlimDX.Vector3 tempMin = SlimDX.Vector3.UnitXYZ, tempMax = -SlimDX.Vector3.UnitXYZ;
                item.GetAABB(ref tempMin, ref tempMax);
                if (refMin.X > tempMin.X)
                    refMin.X = tempMin.X;
                if (refMin.Y > tempMin.Y)
                    refMin.Y = tempMin.Y;
                if (refMin.Z > tempMin.Z)
                    refMin.Z = tempMin.Z;
                if (refMax.X < tempMax.X)
                    refMax.X = tempMax.X;
                if (refMax.Y < tempMax.Y)
                    refMax.Y = tempMax.Y;
                if (refMax.Z < tempMax.Z)
                    refMax.Z = tempMax.Z;

                return CSUtility.Support.EForEachResult.FER_Continue;
            }, null);

            vMin = refMin;
            vMax = refMax;
        }
        
    }
}
