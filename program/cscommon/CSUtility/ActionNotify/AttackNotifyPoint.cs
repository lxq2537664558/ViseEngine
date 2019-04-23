using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CSUtility.ActionNotify
{
    /// <summary>
    /// 攻击监听点类
    /// </summary>
    public class AttackNotifyPoint : Animation.NotifyPoint
    {
        List<SlimDX.Matrix> mBoxList = new List<SlimDX.Matrix>();
        /// <summary>
        /// 对象的矩阵列表
        /// </summary>
        [Browsable(false)]
        [CSUtility.Support.AutoSaveLoadAttribute]
        public List<SlimDX.Matrix> BoxList
        {
            get { return mBoxList; }
            set
            {
                mBoxList = value;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AttackNotifyPoint()
        {            
            NotifyTime = 0;
            NotifyName = "";

            HeaderName = "AttacKBox";
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="time">监听时间</param>
        /// <param name="name">监听点名称</param>
        public AttackNotifyPoint(Int64 time, string name)
        {
            KeyFrameMilliTimeStart = time;
            KeyFrameMilliTimeEnd = time;
            NotifyTime = time;
            NotifyName = name;

            HeaderName = "AttacKBox";
        }
        /// <summary>
        /// 添加监听点元素数据
        /// </summary>
        /// <param name="point">动作监听点对象</param>
        public override void AddItemDatas(Animation.NotifyPoint point)
        {
            for(int i = 0;i < BoxList.Count;++i)
            {
                PointDatas.Add(new AttackItemData(BoxList[i], this) { Index = i});
            }            
        }
        /// <summary>
        /// 添加新的监听数据
        /// </summary>
        /// <param name="point">动作监听点对象</param>
        /// <returns>返回添加的监听点元素数据</returns>
        public override Animation.NotifyItemDataBase AddNewItemData(Animation.NotifyPoint point)
        {
            var item = new AttackItemData(SlimDX.Matrix.Identity, this) { Index = BoxList.Count };
            PointDatas.Add(item);
            BoxList.Add(SlimDX.Matrix.Identity);

            return item;
        }
        /// <summary>
        /// 删除元素数据
        /// </summary>
        /// <param name="itemData">监听点元素数据</param>
        public override void RemoveItemData(Animation.NotifyItemDataBase itemData)
        {
            if (itemData == null)
                return;

            PointDatas.Remove(itemData);
            BoxList.RemoveAt(itemData.Index);

            UpdateItemDataIndex();
        }
    }
    /// <summary>
    /// 攻击类监听点元素数据类
    /// </summary>
    public class AttackItemData : Animation.NotifyItemDataBase,Animation.IGetVisualActor
    {
        Component.ActorBase mActor = null;
        /// <summary>
        /// 组件Actor对象
        /// </summary>
        [Browsable(false)]
        [Animation.V3dBox3Attribute]
        public Component.ActorBase Actor
        {
            get { return mActor; }
            set
            {
                mActor = value;                
            }
        }        

        float mLocationX = 0;
        /// <summary>
        /// 当前对象的X坐标
        /// </summary>
        [Category("属性")]
        public float LocationX
        {
            get { return mLocationX; }
            set
            {
                mLocationX = value;

                var loc = Actor.Placement.GetLocation();
                loc.X = mLocationX;
                Actor.Placement.SetLocation(ref loc);

                UpdateBoxMatrix();

                OnPropertyChanged("LocationX");
            }
        }

        float mLocationY = 0;
        /// <summary>
        /// 当前对象的Y坐标
        /// </summary>
        [Category("属性")]
        public float LocationY
        {
            get { return mLocationY; }
            set
            {
                mLocationY = value;

                var loc = Actor.Placement.GetLocation();
                loc.Y = mLocationY;
                Actor.Placement.SetLocation(ref loc);

                UpdateBoxMatrix();

                OnPropertyChanged("LocationY");
            }
        }

        float mLocationZ = 0;
        /// <summary>
        /// 当前对象的Z坐标
        /// </summary>
        [Category("属性")]
        public float LocationZ
        {
            get { return mLocationZ; }
            set
            {
                mLocationZ = value;

                var loc = Actor.Placement.GetLocation();
                loc.Z = mLocationZ;
                Actor.Placement.SetLocation(ref loc);

                UpdateBoxMatrix();

                OnPropertyChanged("LocationZ");
            }
        }

        float mSizeX = 2;
        /// <summary>
        /// X方向的大小，默认为2
        /// </summary>
        [Category("属性")]
        public float SizeX
        {
            get { return mSizeX; }
            set
            {
                mSizeX = value;

                var scale = Actor.Placement.GetScale();
                scale.X = mSizeX;
                Actor.Placement.SetScale(ref scale);

                UpdateBoxMatrix();

                OnPropertyChanged("SizeX");
            }
        }

        float mSizeY = 2;
        /// <summary>
        /// Y方向的大小，默认为2
        /// </summary>
        [Category("属性")]
        public float SizeY
        {
            get { return mSizeY; }
            set
            {
                mSizeY = value;

                var scale = Actor.Placement.GetScale();
                scale.Y = mSizeY;
                Actor.Placement.SetScale(ref scale);

                UpdateBoxMatrix();

                OnPropertyChanged("SizeY");
            }
        }

        float mSizeZ = 2;
        /// <summary>
        /// Z方向的大小，默认为2
        /// </summary>
        [Category("属性")]
        public float SizeZ
        {
            get { return mSizeZ; }
            set
            {
                mSizeZ = value;

                var scale = Actor.Placement.GetScale();
                scale.Z = mSizeZ;
                Actor.Placement.SetScale(ref scale);

                UpdateBoxMatrix();

                OnPropertyChanged("SizeZ");
            }
        }

        float mRotation = 0;
        /// <summary>
        /// 对象的旋转值
        /// </summary>
        [Category("属性")]
        public float Rotation
        {
            get { return mRotation; }
            set
            {
                mRotation = value;

                var angle = mRotation / 180 * System.Math.PI;
                Actor.Placement.SetRotationY((float)angle);

                UpdateBoxMatrix();

                OnPropertyChanged("Rotation");
            }
        }
        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="mat">对象的位置矩阵</param>
        /// <param name="point">攻击监听点对象</param>
        public AttackItemData(SlimDX.Matrix mat, AttackNotifyPoint point)
        {
            HostNotifyPoint = point;

            //             CCore.Component.V3dBox3 boxVis = new CCore.Component.V3dBox3();
            //             boxVis.Color = CSUtility.Support.Color.Red;
            //             var actInit = new CCore.World.ActorInit();
            //             mActor = new CCore.World.Actor();
            //             mActor.Initialize(actInit);
            //             mActor.Visual = boxVis;
            mActor = new Component.ActorBase();
            mActor.SetPlacement(new CSUtility.Component.StandardPlacement(mActor));
            //var loc = box.GetCenter();
            //mActor.Placement.SetLocation(ref loc);
            //var scale = new SlimDX.Vector3(box.Maximum.X - box.Minimum.X,
            //                                box.Maximum.Y - box.Minimum.Y,
            //                                box.Maximum.Z - box.Minimum.Z);
            //mActor.Placement.SetScale(ref scale);
            mActor.Placement.SetMatrix(ref mat);

            var loc = mActor.Placement.GetLocation();
            mLocationX = loc.X;
            mLocationY = loc.Y;
            mLocationZ = loc.Z;
            mRotation = (float)(mActor.Placement.GetRotationY() / System.Math.PI * 180);
            var scale = mActor.Placement.GetScale();
            mSizeX = scale.X;
            mSizeY = scale.Y;
            mSizeZ = scale.Z;
        }

        private void UpdateBoxMatrix()
        {
            ((AttackNotifyPoint)HostNotifyPoint).BoxList[Index] = mActor.Placement.mMatrix;
        }
        /// <summary>
        /// 获取可视化的Actor对象
        /// </summary>
        /// <returns>返回Actor对象</returns>
        public CSUtility.Component.ActorBase GetVisualActor()
        {
            return mActor;
        }
    }
}
