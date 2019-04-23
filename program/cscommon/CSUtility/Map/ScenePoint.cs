using System;
using System.ComponentModel;

namespace CSUtility.Map
{
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("场景点")]
    [Description("初始化时调用脚本")]
    public delegate void FOnInit(ScenePoint scenePoint);
    [CSUtility.Editor.DelegateMethodEditor_AllowedDelegate("场景点")]
    [Description("运行时不断调用此脚本")]
    public delegate void FOnTick(ScenePoint scenePoint);

    public class ScenePoint : CSUtility.Support.XndSaveLoadProxy
    {
        public delegate void Delegate_OnMatrixChanged(ScenePoint pt);
        public event Delegate_OnMatrixChanged OnMatrixChanged;

        protected bool mIsDirty = false;
        [CSUtility.Support.DoNotCopy]
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;

                if(HostGroup != null && mIsDirty)
                    HostGroup.IsDirty = true;
            }
        }

        SlimDX.Matrix mTransMatrix = SlimDX.Matrix.Identity;
        [CSUtility.Support.AutoSaveLoad]
        public SlimDX.Matrix TransMatrix
        {
            get { return mTransMatrix; }
            set
            {
                mTransMatrix = value;

                if (OnMatrixChanged != null)
                    OnMatrixChanged(this);

                IsDirty = true;
            }
        }

        public ScenePointGroup HostGroup;

        public ScenePoint()
        {            
        }

        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            if (!base.Read(xndAtt))
                return false;

            IsDirty = false;
            return true;
        }

        [CSUtility.Event.Attribute.AllowMember("场景点.函数.获取位置", Helper.enCSType.Common, "获取场景点在场景中的位置")]
        public SlimDX.Vector3 GetPosition()
        {
            return SlimDX.Vector3.TransformCoordinate(SlimDX.Vector3.Zero, TransMatrix);
        }

        [CSUtility.Event.Attribute.AllowMember("场景点.函数.获取旋转", Helper.enCSType.Common, "获取场景点在场景中的旋转")]
        public SlimDX.Quaternion GetRotation()
        {
            return SlimDX.Quaternion.RotationMatrix(TransMatrix);
        }

        public virtual void Tick(Int64 elapsedMiliSeccond)
        {

        }
    }
}
