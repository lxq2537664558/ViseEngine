using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Support
{
    public class ConfigFile
    {
        static ConfigFile mInstance = new ConfigFile();
        public static ConfigFile Instance
        {
            get { return mInstance; }
        }

        public void LoadFile()
        {
            string pathname = "Config.cfg";
            if (false == CSUtility.Support.IConfigurator.FillProperty(this, pathname))
            {
                CSUtility.Support.IConfigurator.SaveProperty(this, "", pathname);
            }
        }

        Guid mDefaultMapId = CSUtility.Support.IHelper.GuidParse("4af6b11b-d714-4aca-88b2-8a22fcbe079a");
        [CSUtility.Support.DataValueAttribute("DefaultMapId")]
        public Guid DefaultMapId
        {
            get { return mDefaultMapId; }
            set { mDefaultMapId = value; }
        }

        Guid mDefaultUVAnimId = CSUtility.Support.IHelper.GuidParse("87962d54-1e73-45e7-8215-b54478fef81a");
        [CSUtility.Support.DataValueAttribute("DefaultUVAnimId")]
        public Guid DefaultUVAnimId
        {
            get { return mDefaultUVAnimId; }
            set { mDefaultUVAnimId = value; }
        }

        Guid mDefaultHallId = Guid.Empty;
        [CSUtility.Support.DataValueAttribute("DefaultHallId")]
        public Guid DefaultHallId
        {
            get
            {
                if (mDefaultHallId == Guid.Empty)
                    mDefaultHallId = Guid.NewGuid();
                return mDefaultHallId;
            }
            set { mDefaultHallId = value; }
        }

        [CSUtility.Support.DataValueAttribute("MapSize")]
        public ushort MapSize { get; set; } = 128;

        [CSUtility.Support.DataValueAttribute("RevertEffect")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EffectSet")]
        [CSUtility.Support.ResourcePublishAttribute(CSUtility.Support.enResourceType.Effect)]
        public Guid RevertEffect { get; set; } = CSUtility.Support.IHelper.GuidParse("8a90610c-e0ff-4f55-9d85-00615e6a75f1");

        [CSUtility.Support.DataValueAttribute("LevelUpEffect")]
        [CSUtility.Editor.Editor_PropertyGridDataTemplate("EffectSet")]
        [CSUtility.Support.ResourcePublishAttribute(CSUtility.Support.enResourceType.Effect)]
        public Guid LevelUpEffect { get; set; } = CSUtility.Support.IHelper.GuidParse("8a90610c-e0ff-4f55-9d85-00615e6a75f1");
    }
}
