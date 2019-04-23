using System;

namespace CCore
{
    /// <summary>
    /// 客户端初始化类
    /// </summary>
    public class ClientInit
    {
        /// <summary>
        /// 显示的初始化类对象
        /// </summary>
        public CCore.Graphics.GraphicsInit GraphicsInit;
        /// <summary>
        /// 消息接收的初始化类对象
        /// </summary>
        public CCore.MsgProc.MsgRecieverMgrInit MsgRecieverMgrInit;
    }
    /// <summary>
    /// 空的世界类
    /// </summary>
    public class NullWorld : CCore.World.World
    {
        private NullWorld() : base(Guid.Empty)
        {
            
        }

        static NullWorld smInstance = new NullWorld();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static NullWorld Instance
        {
            get { return smInstance; }
        }
        /// <summary>
        /// 只读属性，该世界是否为空
        /// </summary>
        public override bool IsNullWorld
        {
            get { return true; }
        }
    }
    /// <summary>
    /// 空Actor类
    /// </summary>
    public class NullActor : CCore.World.Actor
    {
        private NullActor()
        {
            mPlacement = new CSUtility.Component.StandardPlacement(this);
        }

        static NullActor smInstance = new NullActor();
        /// <summary>
        /// 声明该类为单例
        /// </summary>
        public static NullActor Instance
        {
            get { return smInstance; }
        }
        /// <summary>
        /// 只读属性，该Actor为空
        /// </summary>
        public override bool IsNullActor
        {
            get { return true; }
        }
    }
    /// <summary>
    /// 客户端类
    /// </summary>
    public class Client
    {
        /// <summary>
        /// 接收消息的对象
        /// </summary>
        protected CCore.MsgProc.MsgRecieverMgr mMsgRecieverMgr = new CCore.MsgProc.MsgRecieverMgr();
        /// <summary>
        /// 只读属性，接收消息的对象
        /// </summary>
        public CCore.MsgProc.MsgRecieverMgr MsgRecieverMgr
        {
            get { return mMsgRecieverMgr; }
        }
        /// <summary>
        /// 显卡对象
        /// </summary>
        protected CCore.Graphics.Graphics mGraphics;
        /// <summary>
        /// 只读属性，当前显卡
        /// </summary>
        public CCore.Graphics.Graphics Graphics
        {
            get { return mGraphics; }
        }
        /// <summary>
        /// Register服务器的TCP连接
        /// </summary>
        protected CSUtility.Net.TcpClient mRegSvrConnect = new CSUtility.Net.TcpClient();
        /// <summary>
        /// 只读属性，Register服务器的TCP连接
        /// </summary>
        public CSUtility.Net.TcpClient RegSvrConnect
        {
            get { return mRegSvrConnect; }
        }
        /// <summary>
        /// Gate服务器的TCP连接
        /// </summary>
        protected CSUtility.Net.TcpClient mGateSvrConnect = new CSUtility.Net.TcpClient();
        /// <summary>
        /// 只读属性，Gate服务器的TCP连接
        /// </summary>
        public CSUtility.Net.TcpClient GateSvrConnect
        {
            get { return mGateSvrConnect; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Client()
        {

        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~Client()
        {
            Cleanup();
        }
        /// <summary>
        /// 对象的初始化
        /// </summary>
        /// <param name="_init">初始化该对象的对象</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool Initialize(ClientInit _init)
        {
		    mGraphics = new CCore.Graphics.Graphics();
		    if( false == mGraphics.Initialize( _init.GraphicsInit ) )
			    return false;

		    if( false == mMsgRecieverMgr.Initialize( _init.MsgRecieverMgrInit ) )
			    return false;

		    return true;
        }
        /// <summary>
        /// 删除客户端的对象
        /// </summary>
        public void Cleanup()
        {
            if (mMainWorld != null)
            {
                mMainWorld.Cleanup();
                mMainWorld = null;
            }
            if (mGraphics != null)
            {
                mGraphics.Cleanup();
                mGraphics = null;
            }
            if (mMsgRecieverMgr != null)
            {
                mMsgRecieverMgr.Cleanup();
                mMsgRecieverMgr = null;
            }
        }
        /// <summary>
        /// 每帧调用
        /// </summary>
        public void Tick()
        {
            if (mMainWorld != null)
                mMainWorld.Tick();
        }

        CCore.World.World mMainWorld = NullWorld.Instance;
        /// <summary>
        /// 主世界对象
        /// </summary>
        public CCore.World.World MainWorld
        {
            get { return mMainWorld; }
            set
            {
                CCore.World.World OldWorld = mMainWorld;
                if (value == null)
                    mMainWorld = NullWorld.Instance;
                mMainWorld = value;                
            }
        }

        //public CCore.World.World SetMainWorld(CCore.World.World World)
        //{
        //    if (World == null)
        //        World = NullWorld.Instance;
        //    CCore.World.World OldWorld = mMainWorld;

        //    mMainWorld = World;
        //    return OldWorld;
        //}

        CCore.World.Actor mChiefRole = NullActor.Instance;
        /// <summary>
        /// 只读属性，游戏中的主角
        /// </summary>
        public virtual CCore.World.Actor ChiefRole
        {
            get { return mChiefRole; }
        }
        /// <summary>
        /// 设置主角对象
        /// </summary>
        /// <param name="value">主角Actor</param>
        public void SetChiefRole(CCore.World.Actor value)
        {
            if (value == null)
                mChiefRole = NullActor.Instance;
            else
                mChiefRole = value;
        }
        /// <summary>
        /// 声明创建主角时调用的委托事件
        /// </summary>
        /// <returns>返回创建的主角对象</returns>
        public delegate CCore.World.Actor FOnReCreateChiefRole();
        /// <summary>
        /// 定义创建主角时调用的委托事件
        /// </summary>
        public FOnReCreateChiefRole OnReCreateChiefRole;
        /// <summary>
        /// 只读属性，获取当前的世界对象
        /// </summary>
        public static CCore.World.World MainWorldInstance
        {
            get
            {
                if (Engine.Instance.Client != null)
                    return Engine.Instance.Client.MainWorld;
                return null;
            }
        }
        /// <summary>
        /// 只读属性，获取当前的主角对象
        /// </summary>
        public static CCore.World.Actor ChiefRoleInstance
        {
            get
            {
                if (Engine.Instance.Client != null)
                    return Engine.Instance.Client.ChiefRole;
                return null;
            }
        }
    }
}
