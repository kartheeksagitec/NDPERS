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
	/// Class NeoSpin.DataObjects.doPersonAccountGhdvHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountGhdvHistory : doBase
    {
         
         public doPersonAccountGhdvHistory() : base()
         {
         }
         public int person_account_ghdv_history_id { get; set; }
         public int person_account_ghdv_id { get; set; }
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
         public int health_insurance_type_id { get; set; }
         public string health_insurance_type_description { get; set; }
         public string health_insurance_type_value { get; set; }
         public int dental_insurance_type_id { get; set; }
         public string dental_insurance_type_description { get; set; }
         public string dental_insurance_type_value { get; set; }
         public int vision_insurance_type_id { get; set; }
         public string vision_insurance_type_description { get; set; }
         public string vision_insurance_type_value { get; set; }
         public int hmo_insurance_type_id { get; set; }
         public string hmo_insurance_type_description { get; set; }
         public string hmo_insurance_type_value { get; set; }
         public int medicare_insurance_type_id { get; set; }
         public string medicare_insurance_type_description { get; set; }
         public string medicare_insurance_type_value { get; set; }
         public int level_of_coverage_id { get; set; }
         public string level_of_coverage_description { get; set; }
         public string level_of_coverage_value { get; set; }
         public string coverage_code { get; set; }
         public int plan_option_id { get; set; }
         public string plan_option_description { get; set; }
         public string plan_option_value { get; set; }
         public int rate_structure_id { get; set; }
         public string rate_structure_description { get; set; }
         public string rate_structure_value { get; set; }
         public int epo_org_id { get; set; }
         public int cobra_type_id { get; set; }
         public string cobra_type_description { get; set; }
         public string cobra_type_value { get; set; }
         public string medicare_claim_no { get; set; }
         public string keeping_other_coverage_flag { get; set; }
         public DateTime medicare_part_a_effective_date { get; set; }
         public DateTime medicare_part_b_effective_date { get; set; }
         public decimal low_income_credit { get; set; }
         public int reason_id { get; set; }
         public string reason_description { get; set; }
         public string reason_value { get; set; }
         public int provider_org_id { get; set; }
         public int employer_type_id { get; set; }
         public string employer_type_description { get; set; }
         public string employer_type_value { get; set; }
         public string overridden_structure_code { get; set; }
         public string rate_structure_code { get; set; }
         public int alternate_structure_code_id { get; set; }
         public string alternate_structure_code_description { get; set; }
         public string alternate_structure_code_value { get; set; }
         public string premium_conversion_indicator_flag { get; set; }
         public DateTime premium_conversion_effective_date { get; set; }
         public string is_enrollment_report_generated { get; set; }
         public string hsa_enrollment_flag { get; set; }
         public int ps_file_change_event_id { get; set; }
         public string ps_file_change_event_description { get; set; }
         public string ps_file_change_event_value { get; set; }
         public string people_soft_file_sent_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountGhdvHistory
    {
         person_account_ghdv_history_id ,
         person_account_ghdv_id ,
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
         health_insurance_type_id ,
         health_insurance_type_description ,
         health_insurance_type_value ,
         dental_insurance_type_id ,
         dental_insurance_type_description ,
         dental_insurance_type_value ,
         vision_insurance_type_id ,
         vision_insurance_type_description ,
         vision_insurance_type_value ,
         hmo_insurance_type_id ,
         hmo_insurance_type_description ,
         hmo_insurance_type_value ,
         medicare_insurance_type_id ,
         medicare_insurance_type_description ,
         medicare_insurance_type_value ,
         level_of_coverage_id ,
         level_of_coverage_description ,
         level_of_coverage_value ,
         coverage_code ,
         plan_option_id ,
         plan_option_description ,
         plan_option_value ,
         rate_structure_id ,
         rate_structure_description ,
         rate_structure_value ,
         epo_org_id ,
         cobra_type_id ,
         cobra_type_description ,
         cobra_type_value ,
         medicare_claim_no ,
         keeping_other_coverage_flag ,
         medicare_part_a_effective_date ,
         medicare_part_b_effective_date ,
         low_income_credit ,
         reason_id ,
         reason_description ,
         reason_value ,
         provider_org_id ,
         employer_type_id ,
         employer_type_description ,
         employer_type_value ,
         overridden_structure_code ,
         rate_structure_code ,
         alternate_structure_code_id ,
         alternate_structure_code_description ,
         alternate_structure_code_value ,
         premium_conversion_indicator_flag ,
         premium_conversion_effective_date ,
         is_enrollment_report_generated ,
         hsa_enrollment_flag ,
         ps_file_change_event_id ,
         ps_file_change_event_description ,
         ps_file_change_event_value ,
         people_soft_file_sent_flag ,
    }
}

