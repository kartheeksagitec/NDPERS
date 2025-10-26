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
	/// Class NeoSpin.DataObjects.doContactTicket:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doContactTicket : doBase
    {
         
         public doContactTicket() : base()
         {
         }
         public int contact_ticket_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string callback_phone { get; set; }
         public string email { get; set; }
         public int contact_type_id { get; set; }
         public string contact_type_description { get; set; }
         public string contact_type_value { get; set; }
         public int event_type_id { get; set; }
         public string event_type_description { get; set; }
         public string event_type_value { get; set; }
         public string caller_name { get; set; }
         public int caller_relationship_id { get; set; }
         public string caller_relationship_description { get; set; }
         public string caller_relationship_value { get; set; }
         public int contact_method_id { get; set; }
         public string contact_method_description { get; set; }
         public string contact_method_value { get; set; }
         public int response_method_id { get; set; }
         public string response_method_description { get; set; }
         public string response_method_value { get; set; }
         public int assign_to_user_id { get; set; }
         public string notes { get; set; }
         public int ticket_type_id { get; set; }
         public string ticket_type_description { get; set; }
         public string ticket_type_value { get; set; }
         public string copy_status_flag { get; set; }
         public int original_contact_ticket_id { get; set; }
         public int time_of_day_id { get; set; }
         public string time_of_day_description { get; set; }
         public string time_of_day_value { get; set; }
         public string idb_remittance_created_flag { get; set; }
         public string is_ticket_created_from_portal_flag { get; set; }
         public int web_contact_id { get; set; }
         public string callback_phone_2 { get; set; }
         public string extn { get; set; }
         public string extn2 { get; set; }
    }
    [Serializable]
    public enum enmContactTicket
    {
         contact_ticket_id ,
         person_id ,
         org_id ,
         status_id ,
         status_description ,
         status_value ,
         callback_phone ,
         email ,
         contact_type_id ,
         contact_type_description ,
         contact_type_value ,
         event_type_id ,
         event_type_description ,
         event_type_value ,
         caller_name ,
         caller_relationship_id ,
         caller_relationship_description ,
         caller_relationship_value ,
         contact_method_id ,
         contact_method_description ,
         contact_method_value ,
         response_method_id ,
         response_method_description ,
         response_method_value ,
         assign_to_user_id ,
         notes ,
         ticket_type_id ,
         ticket_type_description ,
         ticket_type_value ,
         copy_status_flag ,
         original_contact_ticket_id ,
         time_of_day_id ,
         time_of_day_description ,
         time_of_day_value ,
         idb_remittance_created_flag ,
         is_ticket_created_from_portal_flag ,
         web_contact_id ,
         callback_phone_2 ,
         extn ,
         extn2 ,
    }
}

