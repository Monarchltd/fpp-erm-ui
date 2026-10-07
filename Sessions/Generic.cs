using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fppErm.Sessions
{
    public static class Generic
    {
        public static upload upload { get; set; }
    }

    public class upload
    {
        public string tenderid { get; set; }
    }
}