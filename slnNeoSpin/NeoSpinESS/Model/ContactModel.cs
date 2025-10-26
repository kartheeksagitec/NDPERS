using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neo.Model
{
    public class ContactModel
    {
        public int EmployerID { get; set; }
        public int ContactID { get; set; }
        public string Message { get; set; }
        public string AntiForgeryToken { get; set; }
    }
}