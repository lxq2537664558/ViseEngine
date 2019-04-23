using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.FileDownload
{
    public class GameInfo
    {
        string mVersion = "0.0.0";
        public string Version
        {
            get { return mVersion; }
            set { mVersion = value; }
        }

        Int64 mMinPkgSize = 0;
        public Int64 MinPkgSize
        {
            get { return mMinPkgSize; }
            set { mMinPkgSize = value; }
        }

        string mMinPkgFilesMD5 = "";
        public string MinPkgFilesMD5
        {
            get { return mMinPkgFilesMD5; }
            set { mMinPkgFilesMD5 = value; }
        }

        string mFullPkgMD5 = "";
        public string FullPkgMD5
        {
            get { return mFullPkgMD5; }
            set { mFullPkgMD5 = value; }
        }

        string mDownloadServerUrl = "file://127.0.0.1/Game/0.0.1/";
        public string DownloadServerUrl
        {
            get { return mDownloadServerUrl; }
            set { mDownloadServerUrl = value; }
        }

        public void Save(string fileName)
        {
            var holder = CSUtility.Support.XmlHolder.NewXMLHolder("GameInfo", "");

            CSUtility.Support.XmlNode cNode_Version = holder.RootNode.AddNode("Version", "", holder);
            cNode_Version.AddAttrib("Value", Version);
            CSUtility.Support.XmlNode cNode_MinPkgSize = holder.RootNode.AddNode("MinPkgSize", "", holder);
            cNode_MinPkgSize.AddAttrib("Value", MinPkgSize.ToString());
            CSUtility.Support.XmlNode cNode_MinPkgFilesMD5 = holder.RootNode.AddNode("MinPkgFilesMD5", "", holder);
            cNode_MinPkgFilesMD5.AddAttrib("Value", MinPkgFilesMD5);
            CSUtility.Support.XmlNode cNode_FullPkgMD5 = holder.RootNode.AddNode("FullPkgMD5", "", holder);
            cNode_FullPkgMD5.AddAttrib("Value", FullPkgMD5);
            CSUtility.Support.XmlNode cNode_DownloadServerUrl = holder.RootNode.AddNode("DownloadServerUrl", "", holder);
            cNode_DownloadServerUrl.AddAttrib("Value", DownloadServerUrl);

            CSUtility.Support.XmlHolder.SaveXML(fileName, holder, true);
        }
        public void Load(string fileName)
        {
            //CSUtility.Support.IConfigurator.FillProperty(this, fileName);
            var holder = CSUtility.Support.XmlHolder.LoadXML(fileName);
            if (holder == null || holder.RootNode == null)
                return;

            var node = holder.RootNode.FindNode("Version");
            if (node != null)
            {
                CSUtility.Support.XmlAttrib att = node.FindAttrib("Value");
                if (att != null)
                    Version = att.Value;
            }
            node = holder.RootNode.FindNode("MinPkgSize");
            if (node != null)
            {
                CSUtility.Support.XmlAttrib att = node.FindAttrib("Value");
                if ((att != null))
                {
                    MinPkgSize = System.Convert.ToInt64(att.Value);
                }
            }
            node = holder.RootNode.FindNode("MinPkgFilesMD5");
            if (node != null)
            {
                CSUtility.Support.XmlAttrib att = node.FindAttrib("Value");
                if ((att != null))
                {
                    MinPkgFilesMD5 = att.Value;
                }
            }
            node = holder.RootNode.FindNode("FullPkgMD5");
            if (node != null)
            {
                CSUtility.Support.XmlAttrib att = node.FindAttrib("Value");
                if ((att != null))
                {
                    FullPkgMD5 = att.Value;
                }
            }
            node = holder.RootNode.FindNode("DownloadServerUrl");
            if (node != null)
            {
                CSUtility.Support.XmlAttrib att = node.FindAttrib("Value");
                if ((att != null))
                {
                    DownloadServerUrl = att.Value;
                }
            }
        }
    }
}
