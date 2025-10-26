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
	/// Class NeoSpin.DataObjects.doSeminarSchedule:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doSeminarSchedule : doBase
    {
         public doSeminarSchedule() : base()
         {
         }
         public int seminar_schedule_id { get; set; }
         public int contact_ticket_id { get; set; }
         public string seminar_name { get; set; }
         public int seminar_type_id { get; set; }
         public string seminar_type_description { get; set; }
         public string seminar_type_value { get; set; }
         public DateTime seminar_date1 { get; set; }
         public DateTime seminar_date2 { get; set; }
         public int facilitator { get; set; }
         public string time_zone { get; set; }
         public string location_name { get; set; }
         public string location_address { get; set; }
         public string location_city { get; set; }
         public string meeting_room { get; set; }
         public decimal attendee_fee { get; set; }
         public string notes { get; set; }
         public string seminar_cancel_flag { get; set; }
         public int seminar_date1_start_time_id { get; set; }
         public string seminar_date1_start_time_description { get; set; }
         public string seminar_date1_start_time_value { get; set; }
         public int seminar_date1_end_time_id { get; set; }
         public string seminar_date1_end_time_description { get; set; }
         public string seminar_date1_end_time_value { get; set; }
         public int seminar_date2_start_time_id { get; set; }
         public string seminar_date2_start_time_description { get; set; }
         public string seminar_date2_start_time_value { get; set; }
         public int seminar_date2_end_time_id { get; set; }
         public string seminar_date2_end_time_description { get; set; }
         public string seminar_date2_end_time_value { get; set; }
         public string seminar_date1_meeting_request_uid { get; set; }
         public string seminar_date2_meeting_request_uid { get; set; }
         public string webcast_available { get; set; }
         public string confirmation_sent_flag { get; set; }
    }
    [Serializable]
    public enum enmSeminarSchedule
    {
         seminar_schedule_id ,
         contact_ticket_id ,
         seminar_name ,
         seminar_type_id ,
         seminar_type_description ,
         seminar_type_value ,
         seminar_date1 ,
         seminar_date2 ,
         facilitator ,
         time_zone ,
         location_name ,
         location_address ,
         location_city ,
         meeting_room ,
         attendee_fee ,
         notes ,
         seminar_cancel_flag ,
         seminar_date1_start_time_id ,
         seminar_date1_start_time_description ,
         seminar_date1_start_time_value ,
         seminar_date1_end_time_id ,
         seminar_date1_end_time_description ,
         seminar_date1_end_time_value ,
         seminar_date2_start_time_id ,
         seminar_date2_start_time_description ,
         seminar_date2_start_time_value ,
         seminar_date2_end_time_id ,
         seminar_date2_end_time_description ,
         seminar_date2_end_time_value ,
         seminar_date1_meeting_request_uid ,
         seminar_date2_meeting_request_uid ,
         webcast_available ,
         confirmation_sent_flag ,
    }
}

