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
	/// Class NeoSpin.DataObjects.doPersonAccountInsuranceContribution:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountInsuranceContribution : doBase
    {
         
         public doPersonAccountInsuranceContribution() : base()
         {
         }
         public int health_insurance_contribution_id { get; set; }
         public int person_account_id { get; set; }
         public int subsystem_id { get; set; }
         public string subsystem_description { get; set; }
         public string subsystem_value { get; set; }
         public int subsystem_ref_id { get; set; }
         public DateTime transaction_date { get; set; }
         public DateTime effective_date { get; set; }
         public int person_employment_dtl_id { get; set; }
         public int transaction_type_id { get; set; }
         public string transaction_type_description { get; set; }
         public string transaction_type_value { get; set; }
         public decimal due_premium_amount { get; set; }
         public decimal paid_premium_amount { get; set; }
         public decimal rhic_benefit_amount { get; set; }
         public decimal group_health_fee_amt { get; set; }
         public decimal life_basic_premium_amount { get; set; }
         public decimal life_supp_premium_amount { get; set; }
         public decimal life_spouse_supp_premium_amount { get; set; }
         public decimal life_dep_supp_premium_amount { get; set; }
         public int payment_history_header_id { get; set; }
         public decimal othr_rhic_amount { get; set; }
         public decimal js_rhic_amount { get; set; }
         public decimal ltc_member_three_yrs_premium_amount { get; set; }
         public decimal ltc_member_five_yrs_premium_amount { get; set; }
         public decimal ltc_spouse_three_yrs_premium_amount { get; set; }
         public decimal ltc_spouse_five_yrs_premium_amount { get; set; }
         public string group_number { get; set; }
         public decimal life_basic_coverage_amount { get; set; }
         public decimal life_supp_coverage_amount { get; set; }
         public decimal life_spouse_supp_coverage_amount { get; set; }
         public decimal life_dep_supp_coverage_amount { get; set; }
         public decimal ad_and_d_basic_premium_rate { get; set; }
         public decimal ad_and_d_supplemental_premium_rate { get; set; }
         public int provider_org_id { get; set; }
         public string rate_structure_code { get; set; }
         public string coverage_code { get; set; }
         public decimal hsa_amount { get; set; }
         public decimal vendor_amount { get; set; }
         public decimal buydown_amount { get; set; }
         public decimal medicare_part_d_amt { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountInsuranceContribution
    {
         health_insurance_contribution_id ,
         person_account_id ,
         subsystem_id ,
         subsystem_description ,
         subsystem_value ,
         subsystem_ref_id ,
         transaction_date ,
         effective_date ,
         person_employment_dtl_id ,
         transaction_type_id ,
         transaction_type_description ,
         transaction_type_value ,
         due_premium_amount ,
         paid_premium_amount ,
         rhic_benefit_amount ,
         group_health_fee_amt ,
         life_basic_premium_amount ,
         life_supp_premium_amount ,
         life_spouse_supp_premium_amount ,
         life_dep_supp_premium_amount ,
         payment_history_header_id ,
         othr_rhic_amount ,
         js_rhic_amount ,
         ltc_member_three_yrs_premium_amount ,
         ltc_member_five_yrs_premium_amount ,
         ltc_spouse_three_yrs_premium_amount ,
         ltc_spouse_five_yrs_premium_amount ,
         group_number ,
         life_basic_coverage_amount ,
         life_supp_coverage_amount ,
         life_spouse_supp_coverage_amount ,
         life_dep_supp_coverage_amount ,
         ad_and_d_basic_premium_rate ,
         ad_and_d_supplemental_premium_rate ,
         provider_org_id ,
         rate_structure_code ,
         coverage_code ,
         hsa_amount ,
         vendor_amount ,
         buydown_amount ,
         medicare_part_d_amt ,
    }
}

