using System;
using System.Collections.Generic;
using System.Text;

namespace CSUtility.FileDownload
{
    public class ResourceData
    {
        protected string mRelativeFile = "";
        public string RelativeFile
        {
            get { return mRelativeFile; }
            set { mRelativeFile = value; }
        }

        protected string mMD5 = "";
        public string MD5
        {
            get { return mMD5; }
            set { mMD5 = value; }
        }

        protected CSUtility.Support.enResourceType mResourceType = CSUtility.Support.enResourceType.Unknow;
        public CSUtility.Support.enResourceType ResourceType
        {
            get { return mResourceType; }
            set { mResourceType = value; }
        }

        public bool Read(CSUtility.Support.XndAttrib xndAtt)
        {
            Byte version = 0;
            xndAtt.Read(out version);
            switch (version)
            {
                case 0:
                    {
                        xndAtt.Read(out mRelativeFile);
                        xndAtt.Read(out mMD5);

                        string typeStr;
                        xndAtt.Read(out typeStr);
                        ResourceType = (CSUtility.Support.enResourceType)(System.Enum.Parse(typeof(CSUtility.Support.enResourceType), typeStr));
                    }
                    break;
            }

            return true;
        }

        public bool Save(CSUtility.Support.XndAttrib xndAtt)
        {
            Byte version = 0;
            xndAtt.Write(version);

            xndAtt.Write(mRelativeFile);
            xndAtt.Write(mMD5);
            xndAtt.Write(ResourceType.ToString());

            return true;
        }
    }

    public class ResourceDataManager
    {
        //static ResourceDataManager smInstance = new ResourceDataManager();
        //public static ResourceDataManager Instance
        //{
        //    get { return smInstance; }
        //}

        public static Dictionary<string, ResourceData> LoadResourceDatas(string fileName)
        {
            Dictionary<string, ResourceData> retDatas = new Dictionary<string, ResourceData>();

            var holder = CSUtility.Support.XndHolder.LoadXND(fileName);
            if (holder == null)
                return retDatas;

            var att = holder.Node.FindAttrib("ResDatas");
            att.BeginRead();

            int count = 0;
            att.Read(out count);
            for (int i = 0; i < count; i++)
            {
                var data = new ResourceData();
                data.Read(att);
                retDatas[data.RelativeFile] = data;
            }

            att.EndRead();

            holder.Node.TryReleaseHolder();

            return retDatas;
        }

        public static void SaveResourceDatas(Dictionary<string, ResourceData> datas, string fileName)
        {
            var holder = CSUtility.Support.XndHolder.NewXNDHolder();

            var att = holder.Node.AddAttrib("ResDatas");
            att.BeginWrite();

            att.Write(datas.Count);
            foreach (var data in datas.Values)
            {
                data.Save(att);
            }

            att.EndWrite();

            CSUtility.Support.XndHolder.SaveXND(fileName, holder);
        }
    }
}
