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
	/// Class NeoSpin.DataObjects.doMasPerson:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasPerson : doBase
    {
         
         public doMasPerson() : base()
         {
         }
         public int mas_person_id { get; set; }
         public int mas_selection_id { get; set; }
         public string full_name { get; set; }
         public DateTime date_of_birth { get; set; }
         public string gender_description { get; set; }
         public string marital_description { get; set; }
         public string spouse_full_name { get; set; }
         public DateTime spouse_dob { get; set; }
         public string spouse_gender_description { get; set; }
         public int person_empl_detail_id { get; set; }
         public string seasonal_months_description { get; set; }
         public string addr_line1 { get; set; }
         public string addr_line2 { get; set; }
         public string addr_city { get; set; }
         public string addr_state_description { get; set; }
         public int addr_country_id { get; set; }
         public string addr_country_description { get; set; }
         public string addr_country_value { get; set; }
         public string zipcode { get; set; }
         public string foreign_province { get; set; }
         public string foreign_postal_code { get; set; }
         public string is_person_married { get; set; }
         public string is_deferred_enrolled { get; set; }
         public string is_health_enrolled { get; set; }
         public string is_life_enrolled { get; set; }
         public string is_dental_enrolled { get; set; }
         public string is_vision_enrolled { get; set; }
         public string is_ltc_enrolled { get; set; }
         public string is_eap_enrolled { get; set; }
         public string is_flex_enrolled { get; set; }
         public string health_level_of_coverage { get; set; }
         public string dental_level_of_coverage { get; set; }
         public string vision_level_of_coverage { get; set; }
         public string ltc_level_of_coverage { get; set; }
         public string ltc_insurance_type { get; set; }
         public string ltc_spouse_level_of_coverage { get; set; }
         public string eap_provider_name { get; set; }
         public decimal vested_employer_schedule_percentage { get; set; }
         public decimal dc_vested_employer_schedule_percentage { get; set; }
         public string is_db_plan_enroled { get; set; }
         public string is_dc_plan_enroled { get; set; }
         public string is_457_deferred_enrolled { get; set; }
         public string is_medicare_part_d_enrolled { get; set; }
         public string is_payee_account_exist_flag { get; set; }
         public string is_beneficiary_required { get; set; }
         public string is_job_service_enrolled { get; set; }
         public string is_hp_enrolled { get; set; }
         public string is_judge_enrolled { get; set; }
         public string is_adjustments_exists { get; set; }
         public string is_main_enrolled { get; set; }
         public string is_le_enrolled { get; set; }
         public string is_ng_enrolled { get; set; }
         public string is_eligible_for_insurance_plans { get; set; }
         public string is_eligible_for_health { get; set; }
         public string is_eligible_for_life { get; set; }
         public string is_eligible_for_dental { get; set; }
         public string is_eligible_for_vision { get; set; }
         public string is_eligible_for_ltc { get; set; }
         public string is_eligible_for_flex { get; set; }
         public string is_eligible_for_deff { get; set; }
         public string is_eligible_for_457 { get; set; }
         public string is_vested_in_judges { get; set; }
         public string is_vested_in_db { get; set; }
         public string is_eligile_for_eap { get; set; }
         public string is_employment_ended_a_year_back { get; set; }
         public decimal approved_tffr_tiaa_service { get; set; }
         public decimal tentative_tffr_tiaa_service { get; set; }
         public string is_employment_ended { get; set; }
         public string is_db_suspended { get; set; }
    }
    [Serializable]
    public enum enmMasPerson
    {
         mas_person_id ,
         mas_selection_id ,
         full_name ,
         date_of_birth ,
         gender_description ,
         marital_description ,
         spouse_full_name ,
         spouse_dob ,
         spouse_gender_description ,
         person_empl_detail_id ,
         seasonal_months_description ,
         addr_line1 ,
         addr_line2 ,
         addr_city ,
         addr_state_description ,
         addr_country_id ,
         addr_country_description ,
         addr_country_value ,
         zipcode ,
         foreign_province ,
         foreign_postal_code ,
         is_person_married ,
         is_deferred_enrolled ,
         is_health_enrolled ,
         is_life_enrolled ,
         is_dental_enrolled ,
         is_vision_enrolled ,
         is_ltc_enrolled ,
         is_eap_enrolled ,
         is_flex_enrolled ,
         health_level_of_coverage ,
         dental_level_of_coverage ,
         vision_level_of_coverage ,
         ltc_level_of_coverage ,
         ltc_insurance_type ,
         ltc_spouse_level_of_coverage ,
         eap_provider_name ,
         vested_employer_schedule_percentage ,
         dc_vested_employer_schedule_percentage ,
         is_db_plan_enroled ,
         is_dc_plan_enroled ,
         is_457_deferred_enrolled ,
         is_medicare_part_d_enrolled ,
         is_payee_account_exist_flag ,
         is_beneficiary_required ,
         is_job_service_enrolled ,
         is_hp_enrolled ,
         is_judge_enrolled ,
         is_adjustments_exists ,
         is_main_enrolled ,
         is_le_enrolled ,
         is_ng_enrolled ,
         is_eligible_for_insurance_plans ,
         is_eligible_for_health ,
         is_eligible_for_life ,
         is_eligible_for_dental ,
         is_eligible_for_vision ,
         is_eligible_for_ltc ,
         is_eligible_for_flex ,
         is_eligible_for_deff ,
         is_eligible_for_457 ,
         is_vested_in_judges ,
         is_vested_in_db ,
         is_eligile_for_eap ,
         is_employment_ended_a_year_back ,
         approved_tffr_tiaa_service ,
         tentative_tffr_tiaa_service ,
         is_employment_ended ,
         is_db_suspended ,
    }
}

