using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class AppConfig
    {
        static AppConfig smInstance = new AppConfig();
        public static AppConfig Instance
        {
            get { return smInstance; }
        }

        public void LoadFile()
        {
            string pathname = "Zero.cfg";
            if (false == CSUtility.Support.IConfigurator.FillProperty(this, pathname))
            {
                CSUtility.Support.IConfigurator.SaveProperty(this, "", pathname);
            }
        }

        [CSUtility.Support.DataValueAttribute("Interval", false)]
        public int Interval
        {
            get;
            set;
        }

        [CSUtility.Support.DataValueAttribute("HostIpAddress", false)]//注册服务器链接ip        
        public string HostIpAddress
        {
            get;
            set;
        } = "127.0.0.1";
        
        [CSUtility.Support.DataValueAttribute("HostPort", false)]
        public UInt16 HostPort
        {
            get;
            set;
        } = 9998;
        
        [CSUtility.Support.DataValueAttribute("UseIntZ", false)]
        public bool UseIntZ
        {
            get;
            set;
        } = true;

        bool mFinalRelease = false;
        [CSUtility.Support.DataValueAttribute("FinalRelease", false)]
        public bool FinalRelease
        {
            get { return mFinalRelease; }
            set
            {
                mFinalRelease = value;
                CSUtility.Program.FinalRelease = mFinalRelease;
            }
        }
        
        [CSUtility.Support.DataValueAttribute("MTRendering", false)]
        public bool MTRendering
        {
            get;
            set;
        } = false;
        [CSUtility.Support.DataValueAttribute("AndroidPackageName", false)]
        public string AndroidPackageName
        {
            get;
            set;
        } = "vise3d.moba";
    }
}
