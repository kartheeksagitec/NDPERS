using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neo.Model
{
    public class wfmEmployerSelection
    {
        public int ORG_ID { get; set; }
        public int ORG_CODE { get; set; }
        public string ORG_NAME { get; set; }
        public string EMP_TYPE { get; set; }
        public int ContactID { get; set; }
        public string Message { get; set; }
        public string LoginWindowName { get; set; }
        public string AntiForgeryToken { get; set; }
        public IEnumerable<SelectListItem> EmployerList { get; set; }
    }
}