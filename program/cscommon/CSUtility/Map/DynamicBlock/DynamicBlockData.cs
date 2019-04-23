using System;
using System.ComponentModel;

namespace CSUtility.Map.DynamicBlock
{
    public abstract class DynamicBlockDataFactory
    {
        public static DynamicBlockDataFactory Instance;
        // 根据类型来创建DynamicBlockData
        public abstract DynamicBlockData CreateDynamicBlockData(Byte dataType);
    }
    public class DynamicBlockData : RPC.IAutoSaveAndLoad, CSUtility.Support.IXndSaveLoadProxy, INotifyPropertyChanged
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

        // 需同步的属性改变时调用，用于同步更新服务器数据
        public delegate void Delegate_OnPropertyUpdate(DynamicBlockData data);
        public Delegate_OnPropertyUpdate OnPropertyUpdate;

        #endregion

        Guid mId = Guid.Empty;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DoNotCopy]
        [ReadOnly(true)]
        public Guid Id
        {
            get { return mId; }
            set { mId = value; }
        }

        string mName = "";
        [CSUtility.Support.AutoSaveLoadAttribute]
        public string Name
        {
            get { return mName; }
            set
            {
                mName = value;

                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);

                OnPropertyChanged("Name");
            }
        }

        Byte mInstanceStep = Byte.MaxValue;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [RPC.FieldDontAutoSingleSaveLoad]
        public Byte InstanceStep
        {
            get { return mInstanceStep; }
            set
            {
                mInstanceStep = value;
                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);
            }
        }

        bool mIsBlock = true;
        [CSUtility.Support.AutoSaveLoadAttribute]
        public bool IsBlock
        {
            get { return mIsBlock; }
            set { mIsBlock = value; }
        }        

        SlimDX.Matrix mTransMatrix = SlimDX.Matrix.Identity;
        [CSUtility.Support.AutoSaveLoadAttribute]
        [Browsable(false)]
        public SlimDX.Matrix TransMatrix
        {
            get { return mTransMatrix; }
            set
            {
                mTransMatrix = value;
                if (OnPropertyUpdate != null)
                    OnPropertyUpdate(this);

                OnPropertyChanged("TransMatrix");
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
            return CSUtility.Support.Copyable.CopyFrom(this, srcData);
        }

#endregion
    }
}
