using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QnaBOT.Helpers
{
    [Serializable]
    public class flag
    {
        public int threshold1flag;
        public  int threshold2flag;

        public int Flags1
        {
            get;
            set;
        }
        public int Flags2
        {
            get;
            set;
        }
    }
}