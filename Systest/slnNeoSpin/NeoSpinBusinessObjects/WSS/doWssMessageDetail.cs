#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace NeoSpin.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doWssMessageDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssMessageDetail : doBase
    {
         
         public doWssMessageDetail() : base()
         {
         }
         public int wss_message_detail_id { get; set; }
         public int wss_message_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int contact_id { get; set; }
         public string web_link { get; set; }
         public string correspondence_link { get; set; }
         public string clear_message_flag { get; set; }
         public int tracking_id { get; set; }
         public string template_name { get; set; }
         public DateTime report_created_date { get; set; }
         public string mss_email_sent_flag { get; set; }
        public string ess_email_sent_flag { get; set; }
    }
    [Serializable]
    public enum enmWssMessageDetail
    {
         wss_message_detail_id ,
         wss_message_id ,
         person_id ,
         org_id ,
         contact_id ,
         web_link ,
         correspondence_link ,
         clear_message_flag ,
         tracking_id ,
         template_name ,
         report_created_date ,
         mss_email_sent_flag ,
        ess_email_sent_flag,
    }
}

