using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CCore.World.Prefab
{
    /// <summary>
    /// 预制件的资源类
    /// </summary>
    public class PrefabResource : CSUtility.Support.XndSaveLoadProxy, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        /// <summary>
        /// 定义属性改变时调用的委托事件
        /// </summary>
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

        bool mIsDirty = false;
        /// <summary>
        /// 当前对象是否为脏
        /// </summary>
        [CSUtility.Support.DoNotCopy]
        public bool IsDirty
        {
            get { return mIsDirty; }
            set
            {
                mIsDirty = value;
                OnPropertyChanged("IsDirty");
            }
        }

        Guid mId = Guid.NewGuid();
        /// <summary>
        /// 当前对象是ID
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        [CSUtility.Support.DoNotCopy]
        public Guid Id
        {
            get { return mId; }
            set { mId = value; }
        }

        string mPrefabName = "";
        /// <summary>
        /// 预制件的名称
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public string PrefabName
        {
            get { return mPrefabName; }
            set
            {
                mPrefabName = value;

                IsDirty = true;
            }
        }

        SlimDX.Vector3 mOffsetPos = SlimDX.Vector3.Zero;
        /// <summary>
        /// 偏移位置
        /// </summary>
        [CSUtility.Support.AutoSaveLoadAttribute]
        public SlimDX.Vector3 OffsetPos
        {
            get { return mOffsetPos; }
            set { mOffsetPos = value; }
        }

        List<CCore.World.Actor> mActors = new List<CCore.World.Actor>();
        /// <summary>
        /// 只读属性，Actor列表
        /// </summary>
        public CCore.World.Actor[] Actors
        {
            get { return mActors.ToArray(); }
        }
        /// <summary>
        /// 设置预制的Actor列表
        /// </summary>
        /// <param name="actors">Actor列表</param>
        public void SetActors(List<CCore.World.Actor> actors)
        {
            mActors.Clear();

            if (actors.Count > 0)
            {
                OffsetPos = actors[actors.Count - 1].Placement.GetLocation();
            }

            foreach (var actor in actors)
            {
                if (actor is Prefab)
                {
                    var prefabActor = actor as Prefab;
                    foreach (var id in prefabActor.ActorIdList)
                    {
                        var preInActor = CCore.Engine.Instance.Client.MainWorld.FindActor(id);
                        if (preInActor == null)
                        {
                            CCore.Support.MessageBox.Show("Prefab中Id(" + id.ToString() + ")的对象为空，无法保存，请检查！");
                        }

                        var copyedActor = preInActor.Duplicate();

                        var pos = copyedActor.Placement.GetLocation();
                        pos -= OffsetPos;
                        copyedActor.Placement.SetLocation(ref pos);

                        mActors.Add(copyedActor);
                    }
                }
                else
                {
                    var copyedActor = actor.Duplicate();

                    var pos = copyedActor.Placement.GetLocation();
                    pos -= OffsetPos;
                    copyedActor.Placement.SetLocation(ref pos);

                    mActors.Add(copyedActor);
                }
            }
        }
        /// <summary>
        /// 从XND文件读取数据
        /// </summary>
        /// <param name="xndAtt">XND文件</param>
        /// <returns>读取成功返回true，否则返回false</returns>
        public override bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            mActors.Clear();

            base.Read(xndAtt);

            int count = 0;
            xndAtt.Read(out count);
            for (int i = 0; i < count; i++)
            {
                string actorTypeName = "";
                xndAtt.Read(out actorTypeName);

                var actorType = CSUtility.Program.GetTypeFromSaveString(actorTypeName);
                var actor = System.Activator.CreateInstance(actorType) as CCore.World.Actor;
                if (actor == null)
                {
                    System.Diagnostics.Debug.WriteLine(actorTypeName + "未能创建实例!");
                }
                else
                    actor.LoadSceneData(xndAtt);

                mActors.Add(actor);
            }

            IsDirty = false;

            return true;
        }
        /// <summary>
        /// 将对象写入XND文件中
        /// </summary>
        /// <param name="xndAtt">XND文件节点</param>
        /// <returns>写入成功返回true，否则返回false</returns>
        public override bool Write(CSUtility.Support.XndAttrib xndAtt)
        {
            base.Write(xndAtt);

            xndAtt.Write((int)(mActors.Count));
            foreach (var actor in mActors)
            {
                xndAtt.Write(CSUtility.Program.GetTypeSaveString(actor.GetType()));

                actor.SaveSceneData(xndAtt);
            }

            return true;
        }
    }
}
