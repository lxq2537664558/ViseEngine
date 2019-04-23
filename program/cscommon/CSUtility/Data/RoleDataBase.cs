using System;
using System.ComponentModel;

namespace CSUtility.Data
{
    public abstract class RoleDataBase : RPC.IAutoSaveAndLoad, CSUtility.Support.IXndSaveLoadProxy, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // 需同步的属性改变时调用，用于同步更新服务器数据
        public delegate void Delegate_OnPropertyUpdate(RoleDataBase data);
        public Delegate_OnPropertyUpdate OnPropertyUpdate;
        
        #endregion
        
        string mName = "";
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.AISystem.Attribute.AllowMember("角色数据.属性.名称", CSUtility.Helper.enCSType.Common, "角色数据名称")]
        [CSUtility.Event.Attribute.AllowMember("角色数据.属性.名称", CSUtility.Helper.enCSType.Common, "角色数据名称")]
        public string Name
        {
            get { return mName; }
            set
            {
                mName = value;
                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);
            }
        }

        RoleTemplateBase mTemplate = null;
        [Browsable(false)]
        [RPC.FieldDontAutoSaveLoadAttribute()]
        [CSUtility.AISystem.Attribute.AllowMember("角色数据.属性.模板", CSUtility.Helper.enCSType.Common, "获取角色数据使用的角色模板")]
        [CSUtility.Event.Attribute.AllowMember("角色数据.属性.模板", CSUtility.Helper.enCSType.Common, "获取角色数据使用的角色模板")]
        public RoleTemplateBase Template
        {
            get { return mTemplate; }
        }

        [RPC.FieldDontAutoSaveLoadAttribute()]
        public string TemplateName
        {
            get
            {
                if (mTemplate != null)
                {
                    return mTemplate.NickName;
                }

                return "";
            }
        }

        UInt16 mTemplateId;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.AISystem.Attribute.AllowMember("角色数据.属性.模板ID", CSUtility.Helper.enCSType.Common, "获取角色数据使用的角色模板ID")]
        [CSUtility.Event.Attribute.AllowMember("角色数据.属性.模板ID", CSUtility.Helper.enCSType.Common, "获取角色数据使用的角色模板ID")]
        public UInt16 TemplateId
        {
            get { return mTemplateId; }
            set
            {
                mTemplateId = value;
                mTemplate = CSUtility.Data.RoleTemplateManager.Instance.FindRoleTemplate(value);

                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);
            }
        }

        Guid mRoleId = Guid.NewGuid();
        [CSUtility.Support.DoNotCopy]
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.AISystem.Attribute.AllowMember("角色数据.属性.角色ID", CSUtility.Helper.enCSType.Common, "获取角色数据使用的角色ID")]
        [CSUtility.Event.Attribute.AllowMember("角色数据.属性.角色ID", CSUtility.Helper.enCSType.Common, "获取角色数据使用的角色ID")]
        [ReadOnly(true)]
        public Guid RoleId
        {
            get { return mRoleId; }
            set { mRoleId = value; }
        }

        SlimDX.Vector3 mPosition;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.AISystem.Attribute.AllowMember("角色数据.属性.位置", CSUtility.Helper.enCSType.Common, "获取角色数据记录的的角色场景位置")]
        [CSUtility.Event.Attribute.AllowMember("角色数据.属性.位置", CSUtility.Helper.enCSType.Common, "获取角色数据记录的的角色场景位置")]
        [Browsable(false)]
        public virtual SlimDX.Vector3 Position
        {
            get { return mPosition; }
            set
            {
                mPosition = value;

                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);
            }
        }

        SlimDX.Vector3 mOriPosition;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [Browsable(false)]
        [CSUtility.AISystem.Attribute.AllowMember("角色数据.属性.原始位置", CSUtility.Helper.enCSType.Common, "获取或设置角色数据记录的的角色场景原始位置")]
        [CSUtility.Event.Attribute.AllowMember("角色数据.属性.原始位置", CSUtility.Helper.enCSType.Common, "获取或设置角色数据记录的的角色场景原始位置")]
        public virtual SlimDX.Vector3 OriPosition
        {
            get { return mOriPosition; }
            set
            {
                mOriPosition = value;

                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);
            }
        }

        // NPC朝向(弧度)
        float mDirection = 0;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [System.ComponentModel.Browsable(false)]
        public virtual float Direction
        {
            get { return mDirection; }
            set
            {
                if (System.Math.Abs(mDirection - value) < 0.00001f)
                    return;

                mDirection = value;

                //mDirectionAngle = (float)(value / System.Math.PI * 180.0f);

                //if (OnDirectionChanged != null)
                //    OnDirectionChanged(mDirection);

                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);
            }
        }

        float mScale = 1;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [Browsable(false)]
        [CSUtility.AISystem.Attribute.AllowMember("角色数据.属性.缩放", CSUtility.Helper.enCSType.Common, "获取或设置角色的缩放")]
        [CSUtility.Event.Attribute.AllowMember("角色数据.属性.缩放", CSUtility.Helper.enCSType.Common, "获取或设置角色的缩放")]
        public virtual float Scale
        {
            get { return mScale; }
            set
            {
                if (System.Math.Abs(mScale - value) < 0.00001f)
                    return;

                mScale = value;

                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);
            }
        }

        [Browsable(false)]
        [CSUtility.Support.DoNotCopy]
        public float FinalScale
        {
            get
            {
                if (mTemplate != null)
                    return mScale * mTemplate.Scale;

                return mScale;
            }
            set
            {
                if (mTemplate == null)
                    mScale = value;
                else
                {
                    mScale = value / mTemplate.Scale;
                }
            }
        }

        #region IXndSaveLoadProxy

        public bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            return CSUtility.Support.XndSaveLoadProxy.Read(this, xndAtt);
        }
        public bool Write(CSUtility.Support.XndAttrib xndAtt)
        {
            return CSUtility.Support.XndSaveLoadProxy.Write(this, xndAtt);
        }
        public bool CopyFrom(CSUtility.Support.ICopyable srcData)
        {
            return CSUtility.Support.Copyable.CopyFrom(srcData, this);
        }

        #endregion

    }
}
