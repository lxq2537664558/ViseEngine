using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CSUtility.Data
{
    // 角色模板基类
    //[CSUtility.Editor.CDataEditorAttribute(".role")]
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class RoleTemplateBase : CSUtility.Support.Copyable, INotifyPropertyChanged, IDataTemplateBase<UInt16>
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region IDataTemplateBase

        /// <summary>
        /// 模板ID
        /// </summary>
        UInt16 mId;
        [CSUtility.Support.DataValueAttribute("Id")]
        [ReadOnly(true)]
        [Category("基础信息")]
        public UInt16 Id
        {
            get { return mId; }
            set { mId = value; }
        }

        /// <summary>
        /// 数据模板名称
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get { return NickName; }
            set { NickName = value; }
        }

        #endregion

        public delegate void Delegate_Editor_OnPropertyChanged(RoleTemplateBase role);
        public Delegate_Editor_OnPropertyChanged Editor_OnPropertyChanged;

        private UInt32 mVersion = 0;
        [CSUtility.Support.DataValueAttribute("Version")]
        [ReadOnly(true)]
        [Category("基础信息")]
        public UInt32 Version
        {
            get { return mVersion; }
            set
            {
                mVersion = value;
                OnPropertyChanged("Version");
            }
        }

        private bool m_bIsDirty = false;
        [Browsable(false)]
        public bool IsDirty
        {
            get { return m_bIsDirty; }
            set
            {
                m_bIsDirty = value;
                OnPropertyChanged("IsDirty");

                if (Editor_OnPropertyChanged != null)
                    Editor_OnPropertyChanged(this);
            }
        }

        string mNickName = "";
        [CSUtility.Support.DataValueAttribute("NickName")]
        [Category("基础信息")]
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

        float mScale = 1.0f;
        [CSUtility.Support.DataValueAttribute("Scale")]
        [Category("基础信息")]
        public float Scale
        {
            get { return mScale; }
            set { mScale = value; }
        }

        float mMeshFixAngle = 0;
        [CSUtility.Support.DataValueAttribute("MeshFixAngle")]
        [Category("基础信息")]
        public float MeshFixAngle
        {
            get { return mMeshFixAngle; }
            set { mMeshFixAngle = value; }
        }

        Dictionary<string, ActionNamePair> mActions = new Dictionary<string, ActionNamePair>();
        [Browsable(false)]
        [CSUtility.Support.DataValueAttribute("Actions")]        
        public Dictionary<string, ActionNamePair> Actions
        {
            get { return mActions; }
            set { mActions = value; }
        }     

        List<Guid> mDefaultMeshs = new List<Guid>();
        [Browsable(false)]
        [CSUtility.Support.DataValueAttribute("DefaultMeshs")]
        [CSUtility.Support.ResourcePublishAttribute(CSUtility.Support.enResourceType.MeshTemplate)]
        public List<Guid> DefaultMeshs
        {
            get { return mDefaultMeshs; }
            set { mDefaultMeshs = value; }
        }

        public virtual ActionNamePair GetActionNamePair(string name)
        {
            if (mActions.ContainsKey(name))
                return mActions[name];

            return null;
        }
    }
}
