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
	/// Class NeoSpin.DataObjects.doBenefitCalculation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitCalculation : doBase
    {
         public doBenefitCalculation() : base()
         {
         }
         public int benefit_calculation_id { get; set; }
         public int benefit_application_id { get; set; }
         public int person_id { get; set; }
         public int plan_id { get; set; }
         public int calculation_type_id { get; set; }
         public string calculation_type_description { get; set; }
         public string calculation_type_value { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int benefit_account_sub_type_id { get; set; }
         public string benefit_account_sub_type_description { get; set; }
         public string benefit_account_sub_type_value { get; set; }
         public string plso_requested_flag { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public string uniform_income_or_ssli_flag { get; set; }
         public decimal ssli_or_uniform_income_commencement_age { get; set; }
         public decimal estimated_ssli_benefit_amount { get; set; }
         public string reduced_benefit_flag { get; set; }
         public decimal reduced_benefit_option_amount { get; set; }
         public DateTime termination_date { get; set; }
         public DateTime normal_retirement_date { get; set; }
         public DateTime retirement_date { get; set; }
         public DateTime date_of_death { get; set; }
         public string combined_dual_fas_flag { get; set; }
         public int rhic_option_id { get; set; }
         public string rhic_option_description { get; set; }
         public string rhic_option_value { get; set; }
         public decimal unreduced_rhic_amount { get; set; }
         public decimal rhic_early_reduction_factor { get; set; }
         public decimal adjusted_psc { get; set; }
         public decimal adjusted_tvsc { get; set; }
         public string comments { get; set; }
         public decimal js_rhic_amount { get; set; }
         public decimal paid_up_annuity_amount { get; set; }
         public decimal computed_final_average_salary { get; set; }
         public decimal overridden_final_average_salary { get; set; }
         public decimal unreduced_benefit_amount { get; set; }
         public decimal credited_psc { get; set; }
         public decimal projected_psc { get; set; }
         public decimal credited_vsc { get; set; }
         public decimal projected_vsc { get; set; }
         public decimal indexed_final_average_salary { get; set; }
         public decimal percentage_salary_increase { get; set; }
         public int salary_month_increase { get; set; }
         public int dnro_missed_months { get; set; }
         public decimal adhoc_or_cola_amount { get; set; }
         public decimal dnro_total_missed_amount { get; set; }
         public decimal dnro_factor { get; set; }
         public decimal dnro_monthly_increase { get; set; }
         public int early_reduced_months { get; set; }
         public decimal early_reduction_factor { get; set; }
         public decimal plso_lumpsum_amount { get; set; }
         public decimal plso_factor { get; set; }
         public decimal plso_reduction_amount { get; set; }
         public decimal minimum_guarentee_amount { get; set; }
         public decimal minimum_guarentee_amount_taxable_amount { get; set; }
         public decimal minimum_guarentee_amount_non_taxable_amount { get; set; }
         public decimal taxable_amount { get; set; }
         public decimal non_taxable_amount { get; set; }
         public decimal js_residual_mg_amount { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int action_status_id { get; set; }
         public string action_status_description { get; set; }
         public string action_status_value { get; set; }
         public string approved_by { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string suppress_warnings_by { get; set; }
         public DateTime suppress_warnings_date { get; set; }
         public decimal early_monthly_decrease { get; set; }
         public decimal plso_exclusion_ratio { get; set; }
         public decimal ssli_uniform_income_factor { get; set; }
         public decimal qdro_amount { get; set; }
         public decimal taxable_qdro_amount { get; set; }
         public decimal non_taxable_qdro_amount { get; set; }
         public int disability_payee_account_id { get; set; }
         public int is_rule_or_age_conversion { get; set; }
         public decimal final_monthly_benefit { get; set; }
         public decimal rmd_amount { get; set; }
         public decimal actuarial_benefit_reduction { get; set; }
         public int rtw_refund_election_id { get; set; }
         public string rtw_refund_election_description { get; set; }
         public string rtw_refund_election_value { get; set; }
         public string is_rtw_less_than_2years_flag { get; set; }
         public int pre_rtw_payee_account_id { get; set; }
         public decimal actuarially_adjusted_monthly_single_life_benefit { get; set; }
         public string is_rule_applied_flag { get; set; }
         public decimal rhic_factor_amount { get; set; }
         public int originating_payee_account_id { get; set; }
         public int post_retirement_death_reason_type_id { get; set; }
         public string post_retirement_death_reason_type_description { get; set; }
         public string post_retirement_death_reason_type_value { get; set; }
         public int beneficiary_person_id { get; set; }
         public int parent_benefit_calculation_id { get; set; }
         public string is_calculation_visible_flag { get; set; }
         public int rule_indicator_id { get; set; }
         public string rule_indicator_description { get; set; }
         public string rule_indicator_value { get; set; }
         public DateTime ssli_effective_date { get; set; }
         public string recalculated_by { get; set; }
         public string is_created_from_portal { get; set; }
         public decimal non_taxable_plso { get; set; }
         public DateTime fas_termination_date { get; set; }
         public string is_tentative_tffr_tiaa_used { get; set; }
         public int graduated_benefit_option_id { get; set; }
         public string graduated_benefit_option_description { get; set; }
         public string graduated_benefit_option_value { get; set; }
         public decimal rtw_graduated_benefit_factor { get; set; }
         public decimal overridden_dnro_missed_payment_amount { get; set; }
         public string is_dro_estimate { get; set; }
         public int tffr_calculation_method_id { get; set; }
         public string tffr_calculation_method_description { get; set; }
         public string tffr_calculation_method_value { get; set; }
         public DateTime rhic_effective_date { get; set; }

        public decimal fas_2019 { get; set; }
        public decimal fas_2020 { get; set; }

        public decimal fas_2010 { get; set; }
    }
    [Serializable]
    public enum enmBenefitCalculation
    {
         benefit_calculation_id ,
         benefit_application_id ,
         person_id ,
         plan_id ,
         calculation_type_id ,
         calculation_type_description ,
         calculation_type_value ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         benefit_account_sub_type_id ,
         benefit_account_sub_type_description ,
         benefit_account_sub_type_value ,
         plso_requested_flag ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         uniform_income_or_ssli_flag ,
         ssli_or_uniform_income_commencement_age ,
         estimated_ssli_benefit_amount ,
         reduced_benefit_flag ,
         reduced_benefit_option_amount ,
         termination_date ,
         normal_retirement_date ,
         retirement_date ,
         date_of_death ,
         combined_dual_fas_flag ,
         rhic_option_id ,
         rhic_option_description ,
         rhic_option_value ,
         unreduced_rhic_amount ,
         rhic_early_reduction_factor ,
         adjusted_psc ,
         adjusted_tvsc ,
         comments ,
         js_rhic_amount ,
         paid_up_annuity_amount ,
         computed_final_average_salary ,
         overridden_final_average_salary ,
         unreduced_benefit_amount ,
         credited_psc ,
         projected_psc ,
         credited_vsc ,
         projected_vsc ,
         indexed_final_average_salary ,
         percentage_salary_increase ,
         salary_month_increase ,
         dnro_missed_months ,
         adhoc_or_cola_amount ,
         dnro_total_missed_amount ,
         dnro_factor ,
         dnro_monthly_increase ,
         early_reduced_months ,
         early_reduction_factor ,
         plso_lumpsum_amount ,
         plso_factor ,
         plso_reduction_amount ,
         minimum_guarentee_amount ,
         minimum_guarentee_amount_taxable_amount ,
         minimum_guarentee_amount_non_taxable_amount ,
         taxable_amount ,
         non_taxable_amount ,
         js_residual_mg_amount ,
         status_id ,
         status_description ,
         status_value ,
         action_status_id ,
         action_status_description ,
         action_status_value ,
         approved_by ,
         suppress_warnings_flag ,
         suppress_warnings_by ,
         suppress_warnings_date ,
         early_monthly_decrease ,
         plso_exclusion_ratio ,
         ssli_uniform_income_factor ,
         qdro_amount ,
         taxable_qdro_amount ,
         non_taxable_qdro_amount ,
         disability_payee_account_id ,
         is_rule_or_age_conversion ,
         final_monthly_benefit ,
         rmd_amount ,
         actuarial_benefit_reduction ,
         rtw_refund_election_id ,
         rtw_refund_election_description ,
         rtw_refund_election_value ,
         is_rtw_less_than_2years_flag ,
         pre_rtw_payee_account_id ,
         actuarially_adjusted_monthly_single_life_benefit ,
         is_rule_applied_flag ,
         rhic_factor_amount ,
         originating_payee_account_id ,
         post_retirement_death_reason_type_id ,
         post_retirement_death_reason_type_description ,
         post_retirement_death_reason_type_value ,
         beneficiary_person_id ,
         parent_benefit_calculation_id ,
         is_calculation_visible_flag ,
         rule_indicator_id ,
         rule_indicator_description ,
         rule_indicator_value ,
         ssli_effective_date ,
         recalculated_by ,
         is_created_from_portal ,
         non_taxable_plso ,
         fas_termination_date ,
         is_tentative_tffr_tiaa_used ,
         graduated_benefit_option_id ,
         graduated_benefit_option_description ,
         graduated_benefit_option_value ,
         rtw_graduated_benefit_factor ,
         overridden_dnro_missed_payment_amount ,
         is_dro_estimate ,
         tffr_calculation_method_id ,
         tffr_calculation_method_description ,
         tffr_calculation_method_value ,
         rhic_effective_date ,
        fas_2019,
        fas_2020,
        fas_2010,
    }
}

