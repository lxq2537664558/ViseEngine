using CodeGenerateSystem.Base;
using System.Windows.Controls;

namespace MaterialEditor.Controls
{
    public class BaseNodeControl_ShaderVar : BaseNodeControl
    {
        public delegate void Delegate_OnShaderVarChanged(BaseNodeControl_ShaderVar control);
        public Delegate_OnShaderVarChanged OnShaderVarChanged;
        public delegate bool Delegate_OnShaderVarRenamed(BaseNodeControl_ShaderVar control, string oldName, string newName);
        public Delegate_OnShaderVarRenamed OnShaderVarRenamed;

        public delegate void Delegate_OnIsGenericChanging(BaseNodeControl_ShaderVar ctrl, bool oldValue, bool newValue);
        public Delegate_OnIsGenericChanging OnIsGenericChanging;


        CCore.Material.MaterialShaderVarInfo mShaderVarInfo = new CCore.Material.MaterialShaderVarInfo();
        protected CCore.Material.MaterialShaderVarInfo ShaderVarInfo
        {
            get { return mShaderVarInfo; }
        }

        protected bool mIsGeneric = false;
        /// <summary>
        /// 是否为外部可用参数
        /// </summary>
        public bool IsGeneric
        {
            get { return mIsGeneric; }
            set
            {
                if (OnIsGenericChanging != null)
                    OnIsGenericChanging(this, mIsGeneric, value);

                mIsGeneric = value;
                IsDirty = true;
            }
        }

        public BaseNodeControl_ShaderVar()
        {
            ShaderVarInfo.OnReName = new CCore.Material.MaterialShaderVarInfo.Delegate_OnReName(_OnShaderVarRenamed);
        }

        public BaseNodeControl_ShaderVar(Canvas parentDrawCanvas, string strParam)
            : base(parentDrawCanvas, strParam)
        {
            ShaderVarInfo.OnReName = new CCore.Material.MaterialShaderVarInfo.Delegate_OnReName(_OnShaderVarRenamed);
        }

        protected virtual void InitializeShaderVarInfo()
        {

        }

        protected bool _OnShaderVarRenamed(CCore.Material.MaterialShaderVarInfo info, string oldName, string newName)
        {
            if (OnShaderVarRenamed != null)
            {
                if (OnShaderVarRenamed(this, oldName, newName) == true)
                    return true;
            }

            return false;
        }

        public virtual string GetValueDefine() { return ""; }

        public virtual CCore.Material.MaterialShaderVarInfo GetShaderVarInfo(bool force = false)
        {
            if (IsGeneric || force)
                return ShaderVarInfo;

            return null;
        }
    }
}
