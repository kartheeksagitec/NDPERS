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
	/// Class NeoSpin.DataObjects.doSeminarAttendeeDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doSeminarAttendeeDetail : doBase
    {
         
         public doSeminarAttendeeDetail() : base()
         {
         }
         public int seminar_attendee_detail_id { get; set; }
         public int seminar_schedule_id { get; set; }
         public int person_id { get; set; }
         public int contact_id { get; set; }
         public string attendee_name { get; set; }
         public string attended_flag { get; set; }
         public string guest_speaker_flag { get; set; }
         public string guest_flag { get; set; }
         public DateTime retirement_effective_date { get; set; }
         public string convert_unused_sick_leave_flag { get; set; }
         public int payment_method_id { get; set; }
         public string payment_method_description { get; set; }
         public string payment_method_value { get; set; }
         public int org_to_bill_id { get; set; }
         public int no_of_guest_attending { get; set; }
         public int attendance_method_id { get; set; }
         public string attendance_method_description { get; set; }
         public string attendance_method_value { get; set; }
         public string unused_sick_leave_hours { get; set; }
         public string attendee_fee_paid_flag { get; set; }
         public int no_of_webcast_attendees { get; set; }
    }
    [Serializable]
    public enum enmSeminarAttendeeDetail
    {
         seminar_attendee_detail_id ,
         seminar_schedule_id ,
         person_id ,
         contact_id ,
         attendee_name ,
         attended_flag ,
         guest_speaker_flag ,
         guest_flag ,
         retirement_effective_date ,
         convert_unused_sick_leave_flag ,
         payment_method_id ,
         payment_method_description ,
         payment_method_value ,
         org_to_bill_id ,
         no_of_guest_attending ,
         attendance_method_id ,
         attendance_method_description ,
         attendance_method_value ,
         unused_sick_leave_hours ,
         attendee_fee_paid_flag ,
         no_of_webcast_attendees ,
    }
}

