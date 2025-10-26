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
	/// Class NeoSpin.DataObjects.doEmployerPayrollDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doEmployerPayrollDetail : doBase
    {
        
         public doEmployerPayrollDetail() : base()
         {
         }
         public int employer_payroll_detail_id { get; set; }
         public int employer_payroll_header_id { get; set; }
         public int person_id { get; set; }
         public int plan_id { get; set; }
         public string first_name { get; set; }
         public string last_name { get; set; }
         public string ssn { get; set; }
         public int record_type_id { get; set; }
         public string record_type_description { get; set; }
         public string record_type_value { get; set; }
         public DateTime pay_period_date { get; set; }
         public DateTime pay_period_end_month_for_bonus { get; set; }
         public DateTime pay_period_start_date { get; set; }
         public DateTime pay_period_end_date { get; set; }
         public DateTime pay_check_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string comments { get; set; }
         public decimal contribution_amount1 { get; set; }
         public int provider_id1 { get; set; }
         public string provider_org_code_id1 { get; set; }
         public decimal amount_from_enrollment1 { get; set; }
         public decimal contribution_amount2 { get; set; }
         public int provider_id2 { get; set; }
         public string provider_org_code_id2 { get; set; }
         public decimal amount_from_enrollment2 { get; set; }
         public decimal contribution_amount3 { get; set; }
         public int provider_id3 { get; set; }
         public string provider_org_code_id3 { get; set; }
         public decimal amount_from_enrollment3 { get; set; }
         public decimal contribution_amount4 { get; set; }
         public int provider_id4 { get; set; }
         public string provider_org_code_id4 { get; set; }
         public decimal amount_from_enrollment4 { get; set; }
         public decimal contribution_amount5 { get; set; }
         public int provider_id5 { get; set; }
         public string provider_org_code_id5 { get; set; }
         public decimal amount_from_enrollment5 { get; set; }
         public decimal contribution_amount6 { get; set; }
         public int provider_id6 { get; set; }
         public string provider_org_code_id6 { get; set; }
         public decimal amount_from_enrollment6 { get; set; }
         public decimal contribution_amount7 { get; set; }
         public int provider_id7 { get; set; }
         public string provider_org_code_id7 { get; set; }
         public decimal amount_from_enrollment7 { get; set; }
         public decimal eligible_wages { get; set; }
         public decimal premium_amount { get; set; }
         public decimal premium_amount_from_enrollment { get; set; }
         public decimal group_health_fee_amount { get; set; }
         public decimal ee_contribution_reported { get; set; }
         public decimal ee_contribution_calculated { get; set; }
         public decimal ee_pre_tax_reported { get; set; }
         public decimal ee_pre_tax_calculated { get; set; }
         public decimal ee_employer_pickup_reported { get; set; }
         public decimal ee_employer_pickup_calculated { get; set; }
         public decimal er_contribution_reported { get; set; }
         public decimal er_contribution_calculated { get; set; }
         public decimal rhic_er_contribution_reported { get; set; }
         public decimal rhic_er_contribution_calculated { get; set; }
         public decimal rhic_ee_contribution_reported { get; set; }
         public decimal rhic_ee_contribution_calculated { get; set; }
         public decimal member_interest_calculated { get; set; }
         public decimal employer_interest_calculated { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string allow_change_warnings { get; set; }
         public decimal purchase_amount_reported { get; set; }
         public int payment_class_id { get; set; }
         public string payment_class_description { get; set; }
         public string payment_class_value { get; set; }
         public DateTime posted_date { get; set; }
         public decimal rhic_benefit_amount { get; set; }
         public string rgroup_retiree_flag { get; set; }
         public decimal life_ins_age { get; set; }
         public decimal dependent_premium { get; set; }
         public decimal spouse_premium { get; set; }
         public decimal supplemental_premium { get; set; }
         public decimal basic_premium { get; set; }
         public decimal employer_rhic_interest_calculated { get; set; }
         public decimal othr_rhic_amount { get; set; }
         public decimal js_rhic_amount { get; set; }
         public decimal ltc_member_three_yrs_premium { get; set; }
         public decimal ltc_member_five_yrs_premium { get; set; }
         public decimal ltc_spouse_three_yrs_premium { get; set; }
         public decimal ltc_spouse_five_yrs_premium { get; set; }
         public string group_number { get; set; }
         public DateTime purchase_pay_period_date { get; set; }
         public decimal life_basic_coverage_amount { get; set; }
         public decimal life_supp_coverage_amount { get; set; }
         public decimal life_spouse_supp_coverage_amount { get; set; }
         public decimal life_dep_supp_coverage_amount { get; set; }
         public decimal ad_and_d_basic_premium_rate { get; set; }
         public decimal ad_and_d_supplemental_premium_rate { get; set; }
         public int provider_org_id { get; set; }
         public string recalculate_interest_flag { get; set; }
         public string rate_structure_code { get; set; }
         public string coverage_code { get; set; }
         public decimal hsa_amount { get; set; }
         public decimal vendor_amount { get; set; }
         public decimal buydown_amount { get; set; }
         public decimal ee_contribution_original { get; set; }
         public decimal ee_pre_tax_original { get; set; }
         public decimal ee_employer_pickup_original { get; set; }
         public decimal er_contribution_original { get; set; }
         public decimal rhic_er_contribution_original { get; set; }
         public decimal rhic_ee_contribution_original { get; set; }
         public decimal contribution_amount1_original { get; set; }
         public string provider_org_code_id1_original { get; set; }
         public decimal contribution_amount2_original { get; set; }
         public string provider_org_code_id2_original { get; set; }
         public decimal contribution_amount3_original { get; set; }
         public string provider_org_code_id3_original { get; set; }
         public decimal contribution_amount4_original { get; set; }
         public string provider_org_code_id4_original { get; set; }
         public decimal contribution_amount5_original { get; set; }
         public string provider_org_code_id5_original { get; set; }
         public decimal contribution_amount6_original { get; set; }
         public string provider_org_code_id6_original { get; set; }
         public decimal contribution_amount7_original { get; set; }
         public string provider_org_code_id7_original { get; set; }
         public int payment_class_original_id { get; set; }
         public string payment_class_original_description { get; set; }
         public string payment_class_original_value { get; set; }
         public decimal purchase_amount_original { get; set; }
         public decimal wages_original { get; set; }
         public decimal medicare_part_d_amt { get; set; }
         public decimal lis_amount { get; set; }
         public decimal lep_amount { get; set; }
         public int person_account_id { get; set; }
         public int plan_id_original { get; set; }
		 //PIR 25920 New Plan DC 2025
        public decimal ee_pretax_addl_original { get; set; }
        public decimal ee_post_tax_addl_original { get; set; }
        public decimal er_pretax_match_original { get; set; }
        public decimal adec_original { get; set; }
        public decimal ee_pretax_addl_reported { get; set; }
        public decimal ee_post_tax_addl_reported { get; set; }
        public decimal er_pretax_match_reported { get; set; }
        public decimal adec_reported { get; set; }
        public decimal ee_pretax_addl_calculated { get; set; }
        public decimal ee_post_tax_addl_calculated { get; set; }
        public decimal er_pretax_match_calculated { get; set; }
        public decimal adec_calculated { get; set; }
        public decimal eligible_wages_defcomp { get; set; }
    }
    [Serializable]
    public enum enmEmployerPayrollDetail
    {
         employer_payroll_detail_id ,
         employer_payroll_header_id ,
         person_id ,
         plan_id ,
         first_name ,
         last_name ,
         ssn ,
         record_type_id ,
         record_type_description ,
         record_type_value ,
         pay_period_date ,
         pay_period_end_month_for_bonus ,
         pay_period_start_date ,
         pay_period_end_date ,
         pay_check_date ,
         status_id ,
         status_description ,
         status_value ,
         comments ,
         contribution_amount1 ,
         provider_id1 ,
         provider_org_code_id1 ,
         amount_from_enrollment1 ,
         contribution_amount2 ,
         provider_id2 ,
         provider_org_code_id2 ,
         amount_from_enrollment2 ,
         contribution_amount3 ,
         provider_id3 ,
         provider_org_code_id3 ,
         amount_from_enrollment3 ,
         contribution_amount4 ,
         provider_id4 ,
         provider_org_code_id4 ,
         amount_from_enrollment4 ,
         contribution_amount5 ,
         provider_id5 ,
         provider_org_code_id5 ,
         amount_from_enrollment5 ,
         contribution_amount6 ,
         provider_id6 ,
         provider_org_code_id6 ,
         amount_from_enrollment6 ,
         contribution_amount7 ,
         provider_id7 ,
         provider_org_code_id7 ,
         amount_from_enrollment7 ,
         eligible_wages ,
         premium_amount ,
         premium_amount_from_enrollment ,
         group_health_fee_amount ,
         ee_contribution_reported ,
         ee_contribution_calculated ,
         ee_pre_tax_reported ,
         ee_pre_tax_calculated ,
         ee_employer_pickup_reported ,
         ee_employer_pickup_calculated ,
         er_contribution_reported ,
         er_contribution_calculated ,
         rhic_er_contribution_reported ,
         rhic_er_contribution_calculated ,
         rhic_ee_contribution_reported ,
         rhic_ee_contribution_calculated ,
         member_interest_calculated ,
         employer_interest_calculated ,
         suppress_warnings_flag ,
         allow_change_warnings ,
         purchase_amount_reported ,
         payment_class_id ,
         payment_class_description ,
         payment_class_value ,
         posted_date ,
         rhic_benefit_amount ,
         rgroup_retiree_flag ,
         life_ins_age ,
         dependent_premium ,
         spouse_premium ,
         supplemental_premium ,
         basic_premium ,
         employer_rhic_interest_calculated ,
         othr_rhic_amount ,
         js_rhic_amount ,
         ltc_member_three_yrs_premium ,
         ltc_member_five_yrs_premium ,
         ltc_spouse_three_yrs_premium ,
         ltc_spouse_five_yrs_premium ,
         group_number ,
         purchase_pay_period_date ,
         life_basic_coverage_amount ,
         life_supp_coverage_amount ,
         life_spouse_supp_coverage_amount ,
         life_dep_supp_coverage_amount ,
         ad_and_d_basic_premium_rate ,
         ad_and_d_supplemental_premium_rate ,
         provider_org_id ,
         recalculate_interest_flag ,
         rate_structure_code ,
         coverage_code ,
         hsa_amount ,
         vendor_amount ,
         buydown_amount ,
         ee_contribution_original ,
         ee_pre_tax_original ,
         ee_employer_pickup_original ,
         er_contribution_original ,
         rhic_er_contribution_original ,
         rhic_ee_contribution_original ,
         contribution_amount1_original ,
         provider_org_code_id1_original ,
         contribution_amount2_original ,
         provider_org_code_id2_original ,
         contribution_amount3_original ,
         provider_org_code_id3_original ,
         contribution_amount4_original ,
         provider_org_code_id4_original ,
         contribution_amount5_original ,
         provider_org_code_id5_original ,
         contribution_amount6_original ,
         provider_org_code_id6_original ,
         contribution_amount7_original ,
         provider_org_code_id7_original ,
         payment_class_original_id ,
         payment_class_original_description ,
         payment_class_original_value ,
         purchase_amount_original ,
         wages_original ,
         medicare_part_d_amt ,
         lis_amount ,
         lep_amount ,
         person_account_id ,
         plan_id_original ,
         ee_pretax_addl_original ,
         ee_post_tax_addl_original ,
         er_pretax_match_original ,
         adec_original ,
         ee_pretax_addl_reported ,
         ee_post_tax_addl_reported ,
         er_pretax_match_reported ,
         adec_reported ,
         ee_pretax_addl_calculated ,
         ee_post_tax_addl_calculated ,
         er_pretax_match_calculated ,
         adec_calculated ,
         eligible_wages_defcomp,
    }
}

