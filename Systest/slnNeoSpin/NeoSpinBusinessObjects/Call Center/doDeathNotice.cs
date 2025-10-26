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
	/// Class NeoSpin.DataObjects.doDeathNotice:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDeathNotice : doBase
    {
         public doDeathNotice() : base()
         {
         }
         public int death_notice_id { get; set; }
         public int contact_ticket_id { get; set; }
         public string deceased_member_flag { get; set; }
         public string deceased_spouse_flag { get; set; }
         public string deceased_dependant_flag { get; set; }
         public string deceased_name { get; set; }
         public DateTime death_date { get; set; }
         public string contact_name { get; set; }
         public string contact_address { get; set; }
         public string contact_city { get; set; }
         public string contact_zip { get; set; }
         public int contact_state_id { get; set; }
         public string contact_state_description { get; set; }
         public string contact_state_value { get; set; }
         public string contact_phone { get; set; }
         public DateTime deceased_member_date_of_birth { get; set; }
         public string deacease_member_last_4_digits_of_ssn { get; set; }
         public string ess_deceased_member_flag { get; set; }
         public int perslink_id { get; set; }
         public string contact_zip_4_code { get; set; }
         public string address_validate_flag { get; set; }
         public string address_validate_error { get; set; }
         public DateTime last_regular_paycheck_date { get; set; }
         public DateTime last_reporting_month_for_retirement_contributions { get; set; }
         public string death_already_reported { get; set; }      
    }
    [Serializable]
    public enum enmDeathNotice
    {
         death_notice_id ,
         contact_ticket_id ,
         deceased_member_flag ,
         deceased_spouse_flag ,
         deceased_dependant_flag ,
         deceased_name ,
         death_date ,
         contact_name ,
         contact_address ,
         contact_city ,
         contact_zip ,
         contact_state_id ,
         contact_state_description ,
         contact_state_value ,
         contact_phone ,
         deceased_member_date_of_birth ,
         deacease_member_last_4_digits_of_ssn ,
         ess_deceased_member_flag ,
         perslink_id ,
         contact_zip_4_code ,
         address_validate_flag ,
         address_validate_error ,
         last_regular_paycheck_date ,
         last_reporting_month_for_retirement_contributions ,
         death_already_reported ,
    }
}
