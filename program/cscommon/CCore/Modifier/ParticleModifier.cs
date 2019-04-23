using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.Modifier
{
    /// <summary>
    /// 效果器的接口
    /// </summary>
    public interface IEffectorBase
    {
        /// <summary>
        /// 根据粒子ID查找粒子发射器
        /// </summary>
        /// <param name="id">粒子ID</param>
        /// <returns>粒子发射器</returns>
        CCore.Particle.ParticleEmitter FindEmitter(Guid id);
    }
    /// <summary>
    /// 粒子模拟器类
    /// </summary>
    public class ParticleModifier : MeshModifier, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变时调用的函数
        /// </summary>
        /// <param name="propertyName">改变的属性的名称</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

            if (OnPropertyUpdate != null)
                OnPropertyUpdate(this, propertyName);
        }
        #endregion

        //public delegate void Delegate_OnSetMeshTemplateId(Guid meshTemplateId, IParticleModifier modifier);
        //public Delegate_OnSetMeshTemplateId OnSetMeshTemplateId;
        /// <summary>
        /// 声明属性更新的委托事件
        /// </summary>
        /// <param name="modifier">粒子模拟器的名称</param>
        /// <param name="proName">项目名称</param>
        public delegate void Delegate_OnPropertyUpdate(ParticleModifier modifier, string proName);
        /// <summary>
        /// 定义属性更新时调用的委托事件
        /// </summary>
        public Delegate_OnPropertyUpdate OnPropertyUpdate;
        /// <summary>
        /// 所属的效果器
        /// </summary>
        public IEffectorBase HostEffector;
        /// <summary>
        /// 只读属性，总时间
        /// </summary>
        [Browsable(false)]
        public float TotalTime
        {
            get
            {
                float retValue = 0;
                foreach (var emitter in mEmitters)
                {
                    if (float.IsPositiveInfinity(emitter.TotalTime))
                    {
                        return float.PositiveInfinity;
                    }

                    if (retValue < emitter.TotalTime)
                        retValue = emitter.TotalTime;
                }

                return retValue;
            }
        }
        /// <summary>
        /// 只读属性，粒子发射完成
        /// </summary>
        [Browsable(false)]
        public bool IsFinished
        {
            get
            {
                foreach (var emitter in mEmitters)
                {
                    if(!emitter.IsFinished)
                        return false;
                }

                return true;
            }
        }

        bool mEnableEmitter = true;
        /// <summary>
        /// 是否能够发射
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public bool EnableEmitter
        {
            get { return mEnableEmitter; }
            set
            {
                mEnableEmitter = value;

                foreach (var emitter in mEmitters)
                {
                    emitter.EnableEmitter = value;
                }
            }
        }

        Guid mMeshTemplateId = CSUtility.Support.IFileConfig.ParticleDefaultMesh;
        /// <summary>
        /// 模型模板的ID
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("MeshSet")]
        [Category("基本"), DisplayName("模型")]
        public Guid MeshTemplateId
        {
            get { return mMeshTemplateId; }
            set
            {
                mMeshTemplateId = value;

                //if(OnSetMeshTemplateId != null)
                //    OnSetMeshTemplateId(mMeshTemplateId, this);

                OnPropertyChanged("MeshTemplateId");
            }
        }
        /// <summary>
        /// 粒子所在坐标系，可在编辑器中更改
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("坐标系")]
        public CCore.Particle.CoordinateSpaceCN Space
        {
            get
            {
                unsafe
                {
                    if (mModifier != IntPtr.Zero)
                        return (CCore.Particle.CoordinateSpaceCN)DllImportAPI.V3DParticleModifier_GetCoordinateSpace(mModifier);
                    return CCore.Particle.CoordinateSpaceCN.World;
                }
            }
            set
            {
                unsafe
                {
                    if(mModifier != IntPtr.Zero)
                        DllImportAPI.V3DParticleModifier_SetCoordinateSpace(mModifier, (int)value);

                    foreach (var emitter in mEmitters)
                    {
                        emitter.Space = value;
                    }
                }

                OnPropertyChanged("Space");
            }
        }

        List<CCore.Particle.ParticleEmitter> mEmitters = new List<CCore.Particle.ParticleEmitter>();
        /// <summary>
        /// 发射器列表
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Browsable(false)]
        public List<CCore.Particle.ParticleEmitter> Emitters
        {
            get { return mEmitters; }
            set
            {
                mEmitters = value;
            }
        }
        /// <summary>
        /// 只读属性，默认发射器
        /// </summary>
        [Browsable(false)]
        public CCore.Particle.ParticleEmitter DefaultEmitter
        {
            get { return mEmitters[0]; }
        }

        bool mIsDirty;
        /// <summary>
        /// 是否置脏
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public bool IsDirty
        {
            get { return mIsDirty; }
            set { mIsDirty = value; }
        }

        System.Guid mId = Guid.NewGuid();
        /// <summary>
        /// 粒子模拟器的ID
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [CSUtility.Support.DoNotCopy]
        [ReadOnly(true)]
        [Category("基本"), DisplayName("Id")]
        public Guid Id
        {
            get { return mId; }
            set { mId = value; }
        }

        string mParticleModifierName = "Particle";
        /// <summary>
        /// 粒子模拟器的名称
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("名称")]
        public string ParticleModifierName
        {
            get { return mParticleModifierName; }
            set
            {
                mParticleModifierName = value;
                OnPropertyChanged("ParticleModifierName");
            }
        }
        /// <summary>
        /// 粒子池的容量
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("粒子池容量")]
        public int ParticlePoolSize
        {
            get
            {
                unsafe
                {
                    if (mModifier != IntPtr.Zero)
                        return DllImportAPI.V3DParticleModifier_GetParticlePoolSize(mModifier);
                    return 50;
                }
            }
            set
            {
                unsafe
                {
                    if (mModifier != IntPtr.Zero)
                        DllImportAPI.V3DParticleModifier_SetParticlePoolSize(mModifier, value);
                }
            }
        }

        CCore.Performance.EParticleDensity mParticleDensity = CCore.Performance.EParticleDensity.Low;
        /// <summary>
        /// 粒子系统的性能过滤
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("性能"), DisplayName("性能过滤")]
        public CCore.Performance.EParticleDensity ParticleDensity
        {
            get { return mParticleDensity; }
            set
            {
                mParticleDensity = value;
                OnPropertyChanged("ParticleDensity");
            }
        }
        /// <summary>
        /// 面向摄像机类型
        /// </summary>
        public enum enBillboardType
        {
            Disable = 0,
            Free,
            LockY,
        }
        /// <summary>
        /// 面向摄像机类型
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("面向摄像机类型")]
        public enBillboardType BillboardType
        {
            get
            {
                unsafe
                {
                    if (mModifier != IntPtr.Zero)
                        return (enBillboardType)DllImportAPI.V3DParticleModifier_GetDirectionMode(mModifier);
                    return enBillboardType.Disable;
                }
            }
            set
            {
                unsafe
                {
                    if (mModifier != IntPtr.Zero)
                        DllImportAPI.V3DParticleModifier_SetDirectionMode(mModifier, (int)value);
                }
                OnPropertyChanged("BillboardType");
            }
        }

        bool mDelayDead = false;
        /// <summary>
        /// 是否延迟死亡
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [Category("基本"), DisplayName("延迟死亡")]
        public bool DelayDead
        {
            get
            {
                return mDelayDead;
            }
            set
            {
                mDelayDead = value;
            }
        }
        /// <summary>
        /// 构造函数，创建实例对象
        /// </summary>
        public ParticleModifier()
            : base(IntPtr.Zero)
        {
            unsafe
            {
                mIsDirty = false;
                mModifier = DllImportAPI.V3DParticleModifier_New();
                DllImportAPI.V3DParticleModifier_InitObjects(mModifier, CCore.Engine.Instance.Client.Graphics.Device);
                DllImportAPI.V3DParticleModifier_SetParticlePoolSize(mModifier, 50);
                //AddEmitter();
            }
        }
        /// <summary>
        /// 析构函数，删除对象，释放指针内存
        /// </summary>
        ~ParticleModifier()
        {
            foreach (var i in mEmitters)
            {
                i.Cleanup();
            }
            mEmitters.Clear();
            Cleanup();
        }
        /// <summary>
        /// 添加发射器
        /// </summary>
        /// <returns>返回添加的发射器对象</returns>
        public CCore.Particle.ParticleEmitter AddEmitter()
        {
            unsafe
            {
                var pEmitter = new CCore.Particle.ParticleEmitter();
                pEmitter.HostModifier = this;
                DllImportAPI.V3DParticleModifier_AddEmitter(mModifier, pEmitter.Inner);
                mEmitters.Add(pEmitter);
                return pEmitter;
            }
        }
        /// <summary>
        /// 删除指定的发射器
        /// </summary>
        /// <param name="index">索引值</param>
        public void RemoveEmitter(int index)
        {
            if (mEmitters.Count == 1)
                return;
            if (index >= 0 && index < mEmitters.Count)
            {
                unsafe
                {
                    DllImportAPI.V3DParticleModifier_RemoveEmitter(mModifier, index);
                }
                mEmitters[index].HostModifier = null;
                mEmitters.RemoveAt(index);
            }
        }
        /// <summary>
        /// 通过发射器ID删除发射器
        /// </summary>
        /// <param name="id">需要删除的发射器ID</param>
        public void RemoveEmitterById(Guid id)
        {
            if (mEmitters.Count == 1)
                return;

            for (int i = 0; i < mEmitters.Count; ++i)
            {
                if (mEmitters[i].Id == id)
                {
                    mEmitters[i].HostModifier = null;
                    RemoveEmitter(i);
                    break;
                }
            }
        }
        /// <summary>
        /// 通过索引获取发射器
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回与索引相对应的发射器对象</returns>
        public CCore.Particle.ParticleEmitter GetEmitter(int index)
        {
            if(index >= 0 && index < mEmitters.Count)
                return mEmitters[index];
            return null;
        }

        //public void Load(CSUtility.Support.IXndNode node)
        //{
        //    var attr = node.FindAttrib("PtcModifier");
        //    if (attr != null)
        //    {
        //        attr.BeginRead();
        //        this.Read(attr);
        //        attr.EndRead();
        //    }
        //}
        //public void Save(CSUtility.Support.IXndNode node)
        //{
        //    var attr = node.AddAttrib("PtcModifier");
        //    attr.BeginWrite();
        //    this.Write(attr);
        //    attr.EndWrite();
        //}
        //public void CopyFrom(IParticleModifier srcModifier)
        //{

        //}
        /// <summary>
        /// 设置摄像机
        /// </summary>
        /// <param name="eye">视野对象</param>
        public void SetCamera(CCore.Camera.CameraObject eye)
        {
            unsafe
            {
                if(eye != null)
                    DllImportAPI.V3DParticleModifier_SetDirectionCamera(mModifier, eye.CameraPtr);
                else
                    DllImportAPI.V3DParticleModifier_SetDirectionCamera(mModifier, IntPtr.Zero);
            }
        }
        /// <summary>
        /// 设置摄像机
        /// </summary>
        /// <param name="eye">视野的指针</param>
        public void SetCamera(IntPtr eye)
        {
            DllImportAPI.V3DParticleModifier_SetDirectionCamera(mModifier, eye);
        }
        /// <summary>
        /// 重置所有的发射器
        /// </summary>
        public void Reset()
        {
            foreach (var emitter in mEmitters)
            {
                emitter.Reset();
            }
        }

        //public void Load(CSUtility.Support.IXndNode xndNode)
        //{
        //    var att = xndNode.FindAttrib("ModifierData");
        //    if (att != null)
        //    {
        //        att.BeginRead();
        //        this.Read(att);
        //        att.EndRead();
        //    }

        //    var emitterNode = xndNode.FindNode("Emitters");
        //    if (emitterNode != null)
        //    {
        //        foreach (var eNode in emitterNode.GetNodes())
        //        {
        //            var emitter = IParticleEmitter();
        //        }
        //    }
        //}
        //public void Save(CSUtility.Support.IXndNode xndNode)
        //{
        //    var att = xndNode.AddAttrib("ModifierData");
        //    if (att != null)
        //    {
        //        att.BeginWrite();
        //        this.Write(att);
        //        att.EndWrite();
        //    }

        //    var emitterNode = xndNode.AddNode("Emitters", 0, 0);
        //    foreach (var emitter in mEmitters)
        //    {
        //        var eNode = emitterNode.AddNode("Emitter", 0, 0);
        //        emitter.Save(eNode);
        //    }
        //}
        /// <summary>
        /// 从XND中读取发射器等数据
        /// </summary>
        /// <param name="xndAtt">XND数据文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (!base.Read(xndAtt))
                return false;

            foreach (var emitter in mEmitters)
            {
                emitter.HostModifier = this;
                DllImportAPI.V3DParticleModifier_AddEmitter(mModifier, emitter.Inner);
            }

            return true;
        }
        /// <summary>
        /// 克隆粒子模拟器
        /// </summary>
        /// <param name="src">粒子模拟器对象</param>
        public virtual void Clone( ParticleModifier src )
        {
            MeshTemplateId = src.MeshTemplateId;
            Space = src.Space;
            Emitters.Clear();
            foreach( var srcE in src.Emitters )
            {
                var e = new CCore.Particle.ParticleEmitter();
                e.Clone(srcE);
                Emitters.Add(e);
            }
            Id = src.Id;
            ParticleModifierName = src.ParticleModifierName;
            ParticlePoolSize = src.ParticlePoolSize;
            BillboardType = src.BillboardType;
            DelayDead = src.DelayDead;
            ParticleDensity = src.ParticleDensity;

            foreach (var emitter in mEmitters)
            {
                emitter.HostModifier = this;
                DllImportAPI.V3DParticleModifier_AddEmitter(mModifier, emitter.Inner);
            }
        }
        /// <summary>
        /// 从XND数据复制粒子模拟器
        /// </summary>
        /// <param name="srcData">XND数据</param>
        /// <returns>复制成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            if (!base.CopyFrom(srcData))
                return false;

            foreach (var emitter in mEmitters)
            {
                emitter.HostModifier = this;
                DllImportAPI.V3DParticleModifier_AddEmitter(mModifier, emitter.Inner);
            }

            return true;
        }
        /// <summary>
        /// 设置世界转换矩阵
        /// </summary>
        /// <param name="mat">粒子模拟器位置矩阵</param>
        public void SetWorldTransMatrix(SlimDX.Matrix mat)
        {
            unsafe
            {
                DllImportAPI.V3DParticleModifier_SetWorldTransMat(mModifier, &mat);
            }
        }
        /// <summary>
        /// 根据ID查找发射器
        /// </summary>
        /// <param name="id">发射器ID</param>
        /// <returns>返回对应的粒子发射器</returns>
        public CCore.Particle.ParticleEmitter FindEmitter(Guid id)
        {
            foreach (var emitter in mEmitters)
            {
                if (emitter.Id == id)
                    return emitter;
            }

            return null;
        }
        /// <summary>
        /// 加载后更新
        /// </summary>
        public void UpdateAfterLoad()
        {
            foreach (var emitter in mEmitters)
            {
                emitter.UpdateFollowerEmitters();
                emitter.UpdateSpawnEffector();
            }
        }
    }
}
