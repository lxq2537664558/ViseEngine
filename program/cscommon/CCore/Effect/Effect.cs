using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.Effect
{
    /// <summary>
    /// 特效类型
    /// </summary>
    public enum EffectType
    {
        Common,
        Enhance,
        Affix
    }
    /// <summary>
    /// 特效模板类
    /// </summary>
    public class EffectTemplate : CSUtility.Support.XndSaveLoadProxy, INotifyPropertyChanged, CSUtility.Helper.IVersionInterface, CCore.Modifier.IEffectorBase
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 属性改变时调用的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变时调用
        /// </summary>
        /// <param name="propertyName">更改的属性的名称</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

            if (_OnPropertyChanged != null)
                _OnPropertyChanged(propertyName);
        }
        #endregion
        /// <summary>
        /// 声明属性改变时调用的委托事件
        /// </summary>
        /// <param name="propertyName">更改的属性的名称</param>
        public delegate void Delegate_Editor_OnPropertyChanged(string propertyName);
        /// <summary>
        /// 定义属性改变的委托事件
        /// </summary>
        public Delegate_Editor_OnPropertyChanged _OnPropertyChanged;
        /// <summary>
        /// 声明编辑器是否置脏的委托事件
        /// </summary>
        /// <param name="mt">特效模板</param>
        public delegate void Delegate_Editor_OnDirtyChanged(EffectTemplate mt);
        /// <summary>
        /// 定义是否置脏的委托事件
        /// </summary>
        public Delegate_Editor_OnDirtyChanged Editor_OnPropertyChanged;

        bool mIsDirty = false;
        /// <summary>
        /// 该实例是否为脏
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;
                OnPropertyChanged("IsDirty");

                if (Editor_OnPropertyChanged != null)
                    Editor_OnPropertyChanged(this);
            }
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~EffectTemplate()
        {

        }
        
        /// <summary>
        /// 当前绘制的粒子数量
        /// </summary>
        [Browsable(false)]
        public int ParticlesCount
        {
            get
            {
                int retCount = 0;
                foreach (var modifier in mModifierList)
                {
                    retCount += DllImportAPI.V3DParticleModifier_GetParticlesCount(modifier.Modifier);
                }

                return retCount;
            }
        }
        /// <summary>
        /// 粒子存在总时间
        /// </summary>
        [Browsable(false)]
        public float TotalTime
        {
            get
            {
                float retValue = 0;
                foreach (var modifier in mModifierList)
                {
                    if(float.IsPositiveInfinity(modifier.TotalTime))
                        return float.PositiveInfinity;

                    if (retValue < modifier.TotalTime)
                        retValue = modifier.TotalTime;
                }

                return retValue;
            }
        }

        /// <summary>
        /// 是否全部播放完毕
        /// </summary>
        [Browsable(false)]
        public bool IsFinished
        {
            get
            {
                foreach (var modifier in mModifierList)
                {
                    if(!modifier.IsFinished)
                        return false;
                }

                return true;
            }
        }

        bool mEnableEmitter = true;
        /// <summary>
        /// 是否发射
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public bool EnableEmitter
        {
            get { return mEnableEmitter; }
            set
            {
                mEnableEmitter = value;

                foreach (var modifier in mModifierList)
                {
                    modifier.EnableEmitter = value;
                }
            }
        }
        /// <summary>
        /// 得到当前版本
        /// </summary>
        /// <returns>当前版本</returns>
        public UInt32 GetVersion()
        {
            return mVer;
        }

        UInt32 mVer = 0;
        /// <summary>
        /// 粒子版本
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [ReadOnly(true)]
        [Category("基本")]
        public UInt32 Ver
        {
            get { return mVer; }
            set
            {
                mVer = value;
                OnPropertyChanged("Ver");
            }
        }

        Guid mId = Guid.Empty;
        /// <summary>
        /// 粒子ID
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [ReadOnly(true)]
        [Category("基本")]
        public Guid Id
        {
            get { return mId; }
            set { mId = value; }
        }

        bool mLoop = true;
        /// <summary>
        /// 是否循环播放
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [DisplayName("是否循环"), Category("基本")]
        public bool Loop
        {
            get { return mLoop; }
            set { mLoop = value; }
        }


        string mNickName = "Unknow";
        /// <summary>
        /// 粒子名称
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [DisplayName("名称"), Category("基本")]
        public string NickName
        {
            get { return mNickName; }
            set
            {
                var tempValue = mNickName;
                mNickName = value;
                OnPropertyChanged("NickName");

                IsDirty = (tempValue != mNickName);
            }
        }

        string mLayerName = "Effect";
        /// <summary>
        /// 粒子所在层名称
        /// </summary>
        [CSUtility.Support.DataValueAttribute("LayerName")]
        [CSUtility.Editor.Editor_ActorLayerSetter]
        [DisplayName("层"), Category("基本")]
        public string LayerName
        {
            get { return mLayerName; }
            set
            {
                mLayerName = value;
                IsDirty = true;
                OnPropertyChanged("LayerName");
            }
        }

        EffectType mEffectType = EffectType.Common;
        /// <summary>
        /// 特效类型
        /// </summary>
        [CSUtility.Support.AutoSaveLoad]
        [DisplayName("特效类型"), Category("基本")]
        public EffectType EffectType
        {
            get { return mEffectType; }
            set { mEffectType = value; }
        }
        /// <summary>
        /// 父粒子对象
        /// </summary>
        public EffectTemplate ParentEffect = null;

        List<CCore.Modifier.ParticleModifier> mModifierList = new List<CCore.Modifier.ParticleModifier>();
        /// <summary>
        /// 只读属性，模拟器名称
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public CCore.Modifier.ParticleModifier[] Modifiers
        {
            get { return mModifierList.ToArray(); }
            //set
            //{
            //    mModifierList = value;
            //}
        }


        //private void OnModifierSetMeshTemplateId(Guid meshTemplateId, MidLayer.IParticleModifier modifier)
        //{

        //}
        /// <summary>
        /// 添加粒子模拟器
        /// </summary>
        /// <returns>粒子系统模拟器</returns>
        public CCore.Modifier.ParticleModifier AddParticleModifier()
        {
            CCore.Modifier.ParticleModifier modifier = new CCore.Modifier.ParticleModifier();
            modifier.HostEffector = this;
            //modifier.OnSetMeshTemplateId = OnModifierSetMeshTemplateId;
            mModifierList.Add(modifier);

            IsDirty = true;

            return modifier;
        }
        /// <summary>
        /// 删除粒子系统模拟器
        /// </summary>
        /// <param name="modifier">需要删除的粒子系统模拟器</param>
        /// <returns>是否删除成功，成功返回true，否则返回false</returns>
        public bool RemoveParticleModifier(CCore.Modifier.ParticleModifier modifier)
        {
            //modifier.OnSetMeshTemplateId = null;
            modifier.HostEffector = null;
            IsDirty = true;
            return mModifierList.Remove(modifier);
        }
        /// <summary>
        /// 插入粒子系统模拟器
        /// </summary>
        /// <param name="idx">插入的模拟器索引</param>
        /// <param name="modifier">粒子模拟器</param>
        public void InsertParticleModifier(int idx, CCore.Modifier.ParticleModifier modifier)
        {
            if (idx >= mModifierList.Count)
            {
                mModifierList.Add(modifier);
                modifier.HostEffector = this;
            }
            else
            {
                mModifierList.Insert(idx, modifier);
                modifier.HostEffector = this;
            }

            IsDirty = true;
        }
        /// <summary>
        /// 设置摄像机
        /// </summary>
        /// <param name="eye">视野</param>
        public void SetCamera(CCore.Camera.CameraObject eye)
        {
            foreach (var modifier in mModifierList)
            {
                modifier.SetCamera(eye);
            }
        }
        /// <summary>
        /// 复位，还原为默认设置
        /// </summary>
        public void Reset()
        {
            foreach (var modifier in Modifiers)
            {
                modifier.Reset();
            }
        }
        /// <summary>
        /// 保存粒子系统
        /// </summary>
        /// <param name="node">XND数据格式的节点</param>
        /// <returns>保存成功返回true，否则返回false</returns>
        public bool Save(CSUtility.Support.XndNode node)
        {
            var attr = node.AddAttrib("EffectData");
            attr.BeginWrite();
            this.Write(attr);
            attr.EndWrite();

            var modifierNode = node.AddNode("Modifiers", 0, 0);
            foreach (var modifer in mModifierList)
            {
                attr = modifierNode.AddAttrib("Modifier");
                attr.BeginWrite();
                modifer.Write(attr);
                attr.EndWrite();
            }

            IsDirty = false;

            return true;
        }
        /// <summary>
        /// 根据XND数据节点加载粒子系统
        /// </summary>
        /// <param name="node">XND数据节点</param>
        /// <returns>成功加载返回true，否则返回false</returns>
        public bool Load(CSUtility.Support.XndNode node)
        {
            var attr = node.FindAttrib("EffectData");
            if(attr != null)
            {
                attr.BeginRead();
                this.Read(attr);
                attr.EndRead();
            }

            mModifierList.Clear();
            var modifierNode = node.FindNode("Modifiers");
            if (modifierNode != null)
            {
                foreach (var mdAtt in modifierNode.GetAttribs())
                {
                    CCore.Modifier.ParticleModifier modifier = new CCore.Modifier.ParticleModifier();
                    //modifier.OnSetMeshTemplateId = OnModifierSetMeshTemplateId;
                    modifier.HostEffector = this;
                    mdAtt.BeginRead();
                    modifier.Read(mdAtt);
                    mdAtt.EndRead();
                    mModifierList.Add(modifier);
                }
            }                
            
            UpdateAfterLoad();

            IsDirty = false;

            return true;
        }
        /// <summary>
        /// 复制粒子系统
        /// </summary>
        /// <param name="srcData">需要复制的源数据</param>
        /// <returns>复制成功返回true，否则返回false</returns>
        public override bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            //MidLayer.IEngine.Instance.CheckNativeMemoryState("1.CopyFrom");

            //MidLayer.IEngine.Instance.CheckNativeMemoryState("1.BaseCopyFrom");
            if (!base.CopyFrom(srcData))
                return false;
            //MidLayer.IEngine.Instance.CheckNativeMemoryState("2.BaseCopyFrom");

            //MidLayer.IEngine.Instance.CheckNativeMemoryState("1.Modifier.CopyFrom");
            var srcEffect = srcData as EffectTemplate;
            mModifierList.Clear();
            foreach (var modifier in srcEffect.Modifiers)
            {
                CCore.Modifier.ParticleModifier newModifier = new CCore.Modifier.ParticleModifier();
                newModifier.HostEffector = this;
                //newModifier.CopyFrom(modifier);
                newModifier.Clone(modifier);
                mModifierList.Add(newModifier);
            }
            //MidLayer.IEngine.Instance.CheckNativeMemoryState("2.Modifier.CopyFrom");

            UpdateAfterLoad();

            IsDirty = false;
            return true;
            //MidLayer.IEngine.Instance.CheckNativeMemoryState("2.CopyFrom");
        }
        /// <summary>
        /// 根据粒子ID查找粒子发射器
        /// </summary>
        /// <param name="id">粒子ID</param>
        /// <returns>粒子发射器</returns>
        public CCore.Particle.ParticleEmitter FindEmitter(Guid id)
        {
            foreach (var modifier in mModifierList)
            {
                var emt = modifier.FindEmitter(id);
                if (emt != null)
                    return emt;
            }

            return null;
        }
        /// <summary>
        /// 加载后更新
        /// </summary>
        public void UpdateAfterLoad()
        {
            foreach (var modifier in mModifierList)
            {
                modifier.UpdateAfterLoad();
            }
        }
    }
}
