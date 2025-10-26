using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoSpinMSS.Model
{
    public class wfmMSSSwitchMember
    {
        public int MemberID { get; set; }
        public string LoginWindowName { get; set; }
        public string AntiForgeryToken { get; set; }
        public string Message { get; set; }
    }
}