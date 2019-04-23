using System;

namespace CCore.World
{
    /// <summary>
    /// 区域Actor类
    /// </summary>
    public class IRegionActor : Actor
    {
        /// <summary>
        /// 只读属性，区域半径
        /// </summary>
        public float Radius
        {
            get
            {
                if (Visual != null)
                {
                    var vis = Visual as CCore.Component.RegionVisual;
                    return vis.Radius;
                }

                return 0;
            }
        }
        /// <summary>
        /// 区域的角度
        /// </summary>
        public float BrushAngle
        {
            get
            {
                if (Visual != null)
                {
                    var vis = Visual as CCore.Component.RegionVisual;
                    return vis.RegionAngle;
                }

                return 0;
            }
            set
            {
                if (Visual != null)
                {
                    var vis = Visual as CCore.Component.RegionVisual;
                    vis.RegionAngle = value;
                }
            }
        }
        /// <summary>
        /// 区域是否激活
        /// </summary>
        public bool RegionActivity
        {
            get { return Visible; }
            set
            {
                Visible = value;
                if (CCore.Engine.Instance.IsEditorMode == false)
                {
                    if (Visible == true)
                        CCore.Engine.ChiefRoleMoveWithClick = false;
                    else
                        CCore.Engine.ChiefRoleMoveWithClick = true;
                }
            } 
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public IRegionActor()
        {

        }
        /// <summary>
        /// 对象初始化
        /// </summary>
        /// <param name="_init">用于初始化对象的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public override bool Initialize(CSUtility.Component.ActorInitBase _init)
        {
            Visual = new CCore.Component.RegionVisual();
            mPlacement = new CSUtility.Component.StandardPlacement(this);

            return true;
        }
        /// <summary>
        /// 设置区域办结
        /// </summary>
        /// <param name="r">半径的值</param>
        public void SetRadius(float r)
        {
            if (Visual == null)
                return;

            var vis = Visual as CCore.Component.RegionVisual;
            vis.Radius = r;
        }
        /// <summary>
        /// 设置区域图片文件
        /// </summary>
        /// <param name="imageFile">区域显示的图片</param>
        public void SetRegionImageFile(string imageFile)
        {
            if (Visual == null)
                return;

            var vis = Visual as CCore.Component.RegionVisual;
            vis.SetRegionImage(imageFile);
        }
        /// <summary>
        /// 设置区域的材质
        /// </summary>
        /// <param name="matId">材质ID</param>
        public void SetRegionMaterial(Guid matId)
        {
            if (Visual == null)
                return;

            var vis = Visual as CCore.Component.RegionVisual;
            vis.SetRegionMaterial(matId);
        }

    }

}
