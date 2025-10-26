using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neo.Model
{
    public class MasterModel
    {
        public string istrKTRSID { get; set; }
        public string istrUserName { get; set; }
        public string MessageCount { get; set; }
        public string Language { get; set; }
        public string PostBanner { get; set; }
    }

    public class DownloadModel
    {
        public string FormID { get; set; }
        public string astrMethodName { get; set; }
        public Dictionary<string, string> NavigationParam { get; set; }
    }
}