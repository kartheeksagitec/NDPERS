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
	/// Class NeoSpin.DataObjects.doAppointmentSchedule:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doAppointmentSchedule : doBase
    {
         
         public doAppointmentSchedule() : base()
         {
         }
         public int appointment_schedule_id { get; set; }
         public int contact_ticket_id { get; set; }
         public string appointment_name { get; set; }
         public int appointment_type_id { get; set; }
         public string appointment_type_description { get; set; }
         public string appointment_type_value { get; set; }
         public int counselor_user_id { get; set; }
         public DateTime appointment_date { get; set; }
         public int start_time_id { get; set; }
         public string start_time_description { get; set; }
         public string start_time_value { get; set; }
         public int end_time_id { get; set; }
         public string end_time_description { get; set; }
         public string end_time_value { get; set; }
         public string appointment_cancel_flag { get; set; }
         public string notes { get; set; }
         public string meeting_request_uid { get; set; }
         public int time_of_day_id { get; set; }
         public string time_of_day_description { get; set; }
         public string time_of_day_value { get; set; }
         public DateTime requested_date_from { get; set; }
         public DateTime requested_date_to { get; set; }
		 public string no_appointment_necessary { get; set; }
    }
    [Serializable]
    public enum enmAppointmentSchedule
    {
         appointment_schedule_id ,
         contact_ticket_id ,
         appointment_name ,
         appointment_type_id ,
         appointment_type_description ,
         appointment_type_value ,
         counselor_user_id ,
         appointment_date ,
         start_time_id ,
         start_time_description ,
         start_time_value ,
         end_time_id ,
         end_time_description ,
         end_time_value ,
         appointment_cancel_flag ,
         notes ,
         meeting_request_uid ,
         time_of_day_id ,
         time_of_day_description ,
         time_of_day_value ,
         requested_date_from ,
         requested_date_to ,
		 no_appointment_necessary ,
    }
}
