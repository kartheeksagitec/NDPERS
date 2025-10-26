#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoContact : doContact
    {
        public cdoContact()
            : base()
        { }
        //PIR 134 Prod. To display attendee name in seminar sign up sheet
        public String ContactName
        {
            get
            {
                string seperator = ", ";
                StringBuilder sb = new StringBuilder();
                sb.Append(this.last_name);
                if (this.first_name != null && this.first_name.Trim() != "")
                    sb.Append(seperator + this.first_name);
                if (this.middle_name != null && this.middle_name.Trim() != "")
                    sb.Append(" " + this.middle_name);
                return sb.ToString();
            }
        }

        public string full_name
        {
            get
            {
                string lstrName = String.Empty;
                if (!String.IsNullOrEmpty(first_name))
                    lstrName = first_name;
                if (!String.IsNullOrEmpty(middle_name))
                    lstrName += " " + middle_name;
                if (!String.IsNullOrEmpty(last_name))
                    lstrName += " " + last_name;
                return lstrName;
            }
        }

        public string full_name_Caps
        {
            get
            {
                return string.IsNullOrEmpty(full_name) ? string.Empty : full_name.ToUpper();
            }
        }
    }
}
