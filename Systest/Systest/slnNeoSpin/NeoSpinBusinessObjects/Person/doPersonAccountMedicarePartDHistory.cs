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
	/// Class NeoSpin.DataObjects.doPersonAccountMedicarePartDHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountMedicarePartDHistory : doBase
    {
         
         public doPersonAccountMedicarePartDHistory() : base()
         {
         }
         public int person_account_medicare_part_d_history_id { get; set; }
         public int person_account_id { get; set; }
         public int person_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int plan_participation_status_id { get; set; }
         public string plan_participation_status_description { get; set; }
         public string plan_participation_status_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int from_person_account_id { get; set; }
         public int to_person_account_id { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string suppress_warnings_by { get; set; }
         public DateTime suppress_warnings_date { get; set; }
         public string medicare_claim_no { get; set; }
         public DateTime medicare_part_a_effective_date { get; set; }
         public DateTime medicare_part_b_effective_date { get; set; }
         public decimal low_income_credit { get; set; }
         public decimal late_enrollment_penalty { get; set; }
         public int provider_org_id { get; set; }
         public int reason_id { get; set; }
         public string reason_description { get; set; }
         public string reason_value { get; set; }
         public string enrollment_file_sent_flag { get; set; }
         public string record_type_flag { get; set; }
         public int member_person_id { get; set; }
         public DateTime send_after { get; set; }
         public DateTime initial_enroll_date { get; set; }
         public string modified_after_tffr_file_sent_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountMedicarePartDHistory
    {
         person_account_medicare_part_d_history_id ,
         person_account_id ,
         person_id ,
         start_date ,
         end_date ,
         plan_participation_status_id ,
         plan_participation_status_description ,
         plan_participation_status_value ,
         status_id ,
         status_description ,
         status_value ,
         from_person_account_id ,
         to_person_account_id ,
         suppress_warnings_flag ,
         suppress_warnings_by ,
         suppress_warnings_date ,
         medicare_claim_no ,
         medicare_part_a_effective_date ,
         medicare_part_b_effective_date ,
         low_income_credit ,
         late_enrollment_penalty ,
         provider_org_id ,
         reason_id ,
         reason_description ,
         reason_value ,
         enrollment_file_sent_flag ,
         record_type_flag ,
         member_person_id ,
         send_after ,
         initial_enroll_date ,
         modified_after_tffr_file_sent_flag ,
    }
}

