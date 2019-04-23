using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.EditorAssist
{
    /// <summary>
    /// 可在场景中种植的对象
    /// </summary>
    public interface IPlantAbleObject
    {
        /// <summary>
        /// 得到种植的Actor对象
        /// </summary>
        /// <param name="world">种植对象的世界</param>
        /// <returns>返回种植的Actor对象</returns>
        CCore.World.Actor GetPlantActor(CCore.World.World world);

        /// <summary>
        /// 获取预览用对象，在拖动对象进入场景时显示预览对象
        /// </summary>
        /// <param name="world">拖动进入的世界</param>
        /// <returns>返回预览用的对象</returns>
        CCore.World.Actor GetPreviewActor(CCore.World.World world);

        /// <summary>
        /// 获取需要显示属性的对象
        /// </summary>
        /// <returns>返回显示属性的对象</returns>
        object GetPropertyShowObject();
    }
    /// <summary>
    /// 种植属性
    /// </summary>
    public sealed class PlantAbleAttribute : System.Attribute
    {
        /// <summary>
        /// 在种植面板中的组名称(XX.XX.XX)
        /// </summary>
        public string PathName
        {
            get;
            private set;
        }

        /// <summary>
        /// 图标名称
        /// </summary>
        public string IconUri
        {
            get;
            private set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// 种植的属性信息
        /// </summary>
        /// <param name="pathName">在种植面板中的组名称(XX.XX.XX)</param>
        /// <param name="iconUri">图标名称</param>
        /// <param name="description">描述</param>
        public PlantAbleAttribute(string pathName, string iconUri, string description)
        {
            PathName = pathName;
            IconUri = iconUri;
            Description = description;
        }
    }
}
