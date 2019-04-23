using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Client.World
{
    public enum enWorldInitType : byte
    {
        [Description("使用二维网格进行场景管理")]
        Tile,
    }

    public class WorldInitFactory : CCore.World.WorldInitFactory
    {
        public override CCore.World.WorldInit CreateWorldInit(byte type)
        {
            switch((enWorldInitType)type)
            {
                case enWorldInitType.Tile:
                    var wi = new CCore.World.WorldInit();
                    wi.SceneGraphInfo = new CCore.Scene.TileScene.TileSceneInfo();
                    return wi;
                default:
                    return new CCore.World.WorldInit();
            }
        }
    }
}
