using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamePublisher
{
    public class PublisherInfo
    {
        string mVersion = "0.0.0";
        public string Version
        {
            get { return mVersion; }
            set { mVersion = value; }
        }

        UInt64 mSVNVersion = 0;
        public UInt64 SVNVersion
        {
            get { return mSVNVersion; }
            set { mSVNVersion = value; }
        }

        System.DateTime mDateTime;
        public System.DateTime DateTime
        {
            get { return mDateTime; }
            set { mDateTime = value; }
        }

        string mIdentityString = "";
        public string IdentityString
        {
            get { return mIdentityString; }
            set { mIdentityString = value; }
        }

        public PublisherInfo()
        {
            DateTime = System.DateTime.Now;
        }
        public PublisherInfo(string ver, UInt64 svnVersion)
        {
            Version = ver;
            SVNVersion = svnVersion;
            DateTime = System.DateTime.Now;
        }

        public void Save(CSUtility.Support.XmlNode node)
        {
            node.AddAttrib("Version", Version);
            node.AddAttrib("SVNVersion", SVNVersion.ToString());
            node.AddAttrib("DateTime", DateTime.ToString());
            node.AddAttrib("IdentityString", IdentityString);
        }
        public void Load(CSUtility.Support.XmlNode node)
        {
            var att = node.FindAttrib("Version");
            if (att != null)
                Version = att.Value;
            att = node.FindAttrib("SVNVersion");
            if (att != null)
                SVNVersion = System.Convert.ToUInt64(att.Value);
            att = node.FindAttrib("DateTime");
            if (att != null)
            {
                System.DateTime time;
                if(System.DateTime.TryParse(att.Value, out time))
                    DateTime = time;
            }
            att = node.FindAttrib("IdentityString");
            if (att != null)
                IdentityString = att.Value;
        }
    }

    public class PublisherInfoManager
    {
        static PublisherInfoManager smInstance = new PublisherInfoManager();
        public static PublisherInfoManager Instance
        {
            get { return smInstance; }
        }

        List<PublisherInfo> mInfos = new List<PublisherInfo>();
        public List<PublisherInfo> Infos
        {
            get { return mInfos; }
            set { mInfos = value; }
        }

        List<PublisherInfo> mLauncherInfos = new List<PublisherInfo>();
        public List<PublisherInfo> LauncherInfos
        {
            get { return mLauncherInfos; }
            set { mLauncherInfos = value; }
        }

        public string GetLastVersion()
        {
            if (Infos.Count <= 0)
                return "";

            return Infos[Infos.Count - 1].Version;
        }

        public PublisherInfo GetLauncherLastVersionInfo()
        {
            if (LauncherInfos.Count <= 0)
                return null;

            return LauncherInfos[LauncherInfos.Count - 1];
        }

        public void SaveInfos()
        {
            var fileName = AppDomain.CurrentDomain.BaseDirectory + "PublisherInfo.xml";
            CSUtility.Support.XmlHolder holder = CSUtility.Support.XmlHolder.NewXMLHolder("PublisherInfos", "");

            foreach (var info in Infos)
            {
                var node = holder.RootNode.AddNode("Info", "", holder);
                info.Save(node);
            }
            foreach (var info in LauncherInfos)
            {
                var node = holder.RootNode.AddNode("LauncherInfo", "", holder);
                info.Save(node);
            }

            CSUtility.Support.XmlHolder.SaveXML(fileName, holder, true);
        }
        public void LoadInfos()
        {
            var fileName = AppDomain.CurrentDomain.BaseDirectory + "PublisherInfo.xml";
            var holder = CSUtility.Support.XmlHolder.LoadXML(fileName);
            if (holder == null)
                return;

            Infos.Clear();
            foreach (var node in holder.RootNode.FindNodes("Info"))
            {
                PublisherInfo info = new PublisherInfo();
                info.Load(node);
                Infos.Add(info);
            }
            LauncherInfos.Clear();
            foreach (var node in holder.RootNode.FindNodes("LauncherInfo"))
            {
                PublisherInfo info = new PublisherInfo();
                info.Load(node);
                LauncherInfos.Add(info);
            }
        }
    }
}
