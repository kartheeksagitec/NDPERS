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
	/// Class NeoSpin.DataObjects.doPersonAccountLifeHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountLifeHistory : doBase
    {
         
         public doPersonAccountLifeHistory() : base()
         {
         }
         public int person_account_life_history_id { get; set; }
         public int person_account_id { get; set; }
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
         public int life_insurance_type_id { get; set; }
         public string life_insurance_type_description { get; set; }
         public string life_insurance_type_value { get; set; }
         public string premium_waiver_flag { get; set; }
         public DateTime projected_premium_waiver_date { get; set; }
         public DateTime actual_premium_waiver_date { get; set; }
         public decimal waived_amount { get; set; }
         public int level_of_coverage_id { get; set; }
         public string level_of_coverage_description { get; set; }
         public string level_of_coverage_value { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public int plan_option_status_id { get; set; }
         public string plan_option_status_description { get; set; }
         public string plan_option_status_value { get; set; }
         public decimal coverage_amount { get; set; }
         public string disability_letter_sent_flag { get; set; }
         public int provider_org_id { get; set; }
         public int premium_waiver_provider_org_id { get; set; }
         public int reason_id { get; set; }
         public string reason_description { get; set; }
         public string reason_value { get; set; }
         public string premium_conversion_indicator_flag { get; set; }
         public DateTime premium_conversion_effective_date { get; set; }
         public string is_enrollment_report_generated { get; set; }
         public int ps_file_change_event_id { get; set; }
         public string ps_file_change_event_description { get; set; }
         public string ps_file_change_event_value { get; set; }
         public string people_soft_file_sent_flag { get; set; }
         public decimal spouse_waived_amount { get; set; }
         public decimal dependent_waived_amount { get; set; }
         public string is_end_dated_due_to_loss_of_supp_life { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountLifeHistory
    {
         person_account_life_history_id ,
         person_account_id ,
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
         life_insurance_type_id ,
         life_insurance_type_description ,
         life_insurance_type_value ,
         premium_waiver_flag ,
         projected_premium_waiver_date ,
         actual_premium_waiver_date ,
         waived_amount ,
         level_of_coverage_id ,
         level_of_coverage_description ,
         level_of_coverage_value ,
         effective_start_date ,
         effective_end_date ,
         plan_option_status_id ,
         plan_option_status_description ,
         plan_option_status_value ,
         coverage_amount ,
         disability_letter_sent_flag ,
         provider_org_id ,
         premium_waiver_provider_org_id ,
         reason_id ,
         reason_description ,
         reason_value ,
         premium_conversion_indicator_flag ,
         premium_conversion_effective_date ,
         is_enrollment_report_generated ,
         ps_file_change_event_id ,
         ps_file_change_event_description ,
         ps_file_change_event_value ,
         people_soft_file_sent_flag ,
         spouse_waived_amount ,
         dependent_waived_amount ,
         is_end_dated_due_to_loss_of_supp_life,
    }
}

