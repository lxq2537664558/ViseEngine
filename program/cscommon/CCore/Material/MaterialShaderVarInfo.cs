using System.ComponentModel;

namespace CCore.Material
{
    /// <summary>
    /// 材质shader的信息
    /// </summary>
    public class MaterialShaderVarInfo : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变的委托事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        /// <summary>
        /// 声明材质shader置脏的委托事件
        /// </summary>
        /// <param name="info">需要置脏的材质shader的info</param>
        public delegate void Delegate_OnDirtyChanged(MaterialShaderVarInfo info);
        /// <summary>
        /// 定义材质shader置脏时调用的委托事件
        /// </summary>
        public Delegate_OnDirtyChanged OnDirtyChanged;
        /// <summary>
        /// 声明材质重命名调用的委托事件
        /// </summary>
        /// <param name="info">材质shader信息类</param>
        /// <param name="oldName">旧名称</param>
        /// <param name="newName">新名称</param>
        /// <returns>重命名成功返回true，否则返回false</returns>
        public delegate bool Delegate_OnReName(MaterialShaderVarInfo info, string oldName, string newName);
        /// <summary>
        /// 定义材质重命名时调用的委托事件
        /// </summary>
        public Delegate_OnReName OnReName;

        bool m_bIsDirty = false;
        /// <summary>
        /// 是否置脏
        /// </summary>
        public bool IsDirty
        {
            get { return m_bIsDirty; }
            set
            {
                m_bIsDirty = value;

                if (OnDirtyChanged != null)
                    OnDirtyChanged(this);
            }
        }
        /// <summary>
        /// 只读属性，获取命名前的名称
        /// </summary>
        public static string ValueNamePreString
        {
            get { return "ShaderVar_"; }
        }
        
        /// <summary>
        /// 使用的编辑器类型
        /// </summary>
        public string EditorType = "";

        string mVarName;
        /// <summary>
        /// 无效名称
        /// </summary>
        public string VarName
        {
            get { return mVarName; }
            set
            {
                if (mVarName != value)
                {
                    mVarName = value;
                    NickName = mVarName.Remove(0, ValueNamePreString.Length);
                    IsDirty = true;
                }
            }
        }

        string mNickName = "";
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName
        {
            get { return mNickName; }
            set
            {
                mNickName = value;
                OnPropertyChanged("NickName");
            }
        }

        string m_VarType;
        /// <summary>
        /// 无效类型
        /// </summary>
        public string VarType
        {
            get { return m_VarType; }
            set
            {
                m_VarType = value;
                IsDirty = true;
            }
        }
        string m_VarValue;
        /// <summary>
        /// 无效的数据
        /// </summary>
        public string VarValue
        {
            get { return m_VarValue; }
            set
            {
                m_VarValue = value;
                IsDirty = true;
            }
        }
        /// <summary>
        /// 复制材质shader信息
        /// </summary>
        /// <param name="info">材质的shader信息类</param>
        public void Copy(MaterialShaderVarInfo info)
        {
            VarName = info.VarName;
            VarType = info.VarType;
            VarValue = info.VarValue;
            EditorType = info.EditorType;
        }
        /// <summary>
        /// 将该对象保存到XND文档
        /// </summary>
        /// <param name="node">XND文档的节点</param>
        public void Save(CSUtility.Support.XmlNode node)
        {
            node.AddAttrib("EditorType", EditorType);
            node.AddAttrib("Type", VarType);
            node.AddAttrib("Value", VarValue);
        }
        /// <summary>
        /// 从xml加载数据
        /// </summary>
        /// <param name="node">xml数据节点</param>
        public void Load(CSUtility.Support.XmlNode node)
        {
            var vAttr = node.FindAttrib("EditorType");
            if (vAttr != null)
                EditorType = vAttr.Value;
            vAttr = node.FindAttrib("Type");
            if (vAttr != null)
                VarType = vAttr.Value;
            vAttr = node.FindAttrib("Value");
            if (vAttr != null)
                VarValue = vAttr.Value;
            VarName = node.Name;
        }
        /// <summary>
        /// 新的参数类型
        /// </summary>
        /// <param name="varType">数据类型</param>
        /// <returns>返回更新后的值</returns>
        public static string NewValueString(string varType)
        {
            switch (varType)
            {
                case "texture":
                    return CSUtility.Support.IFileConfig.DefaultTextureFile;

                case "float":
                case "float1":
                    return "0";

                case "float2":
                    return "0,0";

                case "float3":
                    return "0,0,0";

                case "float4":
                    return "0,0,0,0";
            }

            return "";
        }
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="newName">新名称</param>
        /// <returns>重命名成功返回true，否则返回false</returns>
        public bool Rename(string newName)
        {
            if (OnReName != null)
            {
                if (OnReName(this, mVarName, newName) == false)
                    return false;
            }

            mVarName = newName;

            return true;
        }
    }
}
