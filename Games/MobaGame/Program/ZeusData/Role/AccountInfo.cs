using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Role
{
    
    [CSUtility.DBBindTable("Account")]
    public class AccountInfo : RPC.IAutoSaveAndLoad
    {
        System.DateTime mCreateTime = System.DateTime.Now;
        [CSUtility.DBBindField("CreateTime")]
        [RPC.FieldDontAutoSingleSaveLoad()]
        public System.DateTime CreateTime
        {
            get { return mCreateTime; }
            set { mCreateTime = value; }
        }

        Guid mLinkSerialId = Guid.Empty;
        [RPC.FieldDontAutoSaveLoad()]
        public Guid LinkSerialId
        {
            get { return mLinkSerialId; }
            set { mLinkSerialId = value; }
        }

        string mUserName;
        [CSUtility.DBBindField("UserName")]
        public string UserName
        {
            get { return mUserName; }
            set { mUserName = value; }
        }
        string mPassword;
        [CSUtility.DBBindField("Password")]
        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }
        Guid mId;
        [CSUtility.DBBindField("Id")]
        public Guid Id
        {
            get { return mId; }
            set { mId = value; }
        }
    }
}
