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
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoWssMemberRecordRequest:
	/// Inherited from doWssMemberRecordRequest, the class is used to customize the database object doWssMemberRecordRequest.
	/// </summary>
    [Serializable]
	public class cdoWssMemberRecordRequest : doWssMemberRecordRequest
	{
		public cdoWssMemberRecordRequest() : base()
		{
		}
        public int org_id { get; set; }
        /// <summary>
        /// pir 7952 : 3 Properties added to Set details of Panel Requested By
        /// </summary>
        /// <returns></returns>
        public string Requested_By_Contact_id { get; set; }
        public string Requested_By_Contact_Name { get; set; }
        public string Requested_By_Contact_Phone_No { get; set; }
        public string Requested_By_Contact_Email { get; set; } // PIR 10391
        public string ReenterSSN { get; set; }
        public string istrRetrStatus { get; set; }
        public string istrLifeStatus { get; set; }
        public string istrEapStatus { get; set; }

        public string istrLastFourDigitsSSN
        {
            get 
            {
                if(!string.IsNullOrEmpty(ssn))
                    return ssn.Substring(5,4);
                else
                    return string.Empty;
            }
        }

        public String FullName
        {
            get
            {
                string lstrName = String.Empty;
                //PIR 13380 added Suffix and prefix
                if (!String.IsNullOrEmpty(name_prefix_value))
                {
                    lstrName = name_prefix_value.Trim();
                }
                if (!String.IsNullOrEmpty(first_name))
                {
                    //lstrName = first_name.Trim();
                    lstrName += " " + first_name.Trim();
                }
                if (!String.IsNullOrEmpty(middle_name))
                {
                    lstrName += " " + middle_name.Trim();
                }
                if (!String.IsNullOrEmpty(last_name))
                {
                    lstrName += " " + last_name.Trim();
                }
                if (!String.IsNullOrEmpty(name_suffix_value))
                {
                    lstrName += " " + name_suffix_value.Trim();
                }

                return lstrName;
            }
        }
    } 
} 
