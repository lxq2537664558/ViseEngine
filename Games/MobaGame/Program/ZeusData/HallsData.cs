using System;
using System.Collections.Generic;
using System.Text;

namespace GameData
{
    [System.ComponentModel.TypeConverterAttribute("System.ComponentModel.ExpandableObjectConverter")]
    public class HallsData : RPC.IAutoSaveAndLoad
    {
        public HallsData()
        {
            mCreateTime = DateTime.Now;
            mHallsId = Guid.NewGuid();
        }

        DateTime mCreateTime;
        public DateTime CreateTime
        {
            get { return mCreateTime; }
            set { mCreateTime = value; }
        }

        Guid mHallsId;
        public Guid HallsId
        {
            get { return mHallsId; }
            set { mHallsId = value; }
        }
    }
}
