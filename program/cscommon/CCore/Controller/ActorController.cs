namespace CCore.Controller
{
    /// <summary>
    /// Actor控制器
    /// </summary>
    public class ActorController : CCore.MsgProc.MsgReceiver
    {
        /// <summary>
        /// Actor的所属Actor
        /// </summary>
        protected CCore.World.Actor mHost;
        /// <summary>
        /// 所属Actor
        /// </summary>
        public virtual CCore.World.Actor Host
        {
            get { return mHost; }
            set { mHost = value; }
        }
    }
}
