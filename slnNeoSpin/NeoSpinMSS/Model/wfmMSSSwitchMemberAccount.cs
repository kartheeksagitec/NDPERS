using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoSpinMSS.Model
{
    public class wfmMSSSwitchMemberAccount
    {
        public string Message { get; set; }
        public string IsActiveAccountSelected { get; set; }
        public string AntiForgeryToken { get; set; }
        public string LoginWindowName { get; set; }
    }
}