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
	/// Class NeoSpin.DataObjects.doIbsDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIbsDetail : doBase
    {
         
         public doIbsDetail() : base()
         {
         }
         public int ibs_detail_id { get; set; }
         public int ibs_header_id { get; set; }
         public int person_id { get; set; }
         public int plan_id { get; set; }
         public DateTime billing_month_and_year { get; set; }
         public int mode_of_payment_id { get; set; }
         public string mode_of_payment_description { get; set; }
         public string mode_of_payment_value { get; set; }
         public string coverage_code { get; set; }
         public decimal member_premium_amount { get; set; }
         public decimal rhic_amount { get; set; }
         public decimal total_premium_amount { get; set; }
         public decimal balance_forward { get; set; }
         public string comment { get; set; }
         public decimal group_health_fee_amt { get; set; }
         public int person_account_id { get; set; }
         public int deposit_id { get; set; }
         public decimal life_basic_premium_amount { get; set; }
         public decimal life_supp_premium_amount { get; set; }
         public decimal life_spouse_supp_premium_amount { get; set; }
         public decimal life_dep_supp_premium_amount { get; set; }
         public decimal othr_rhic_amount { get; set; }
         public decimal js_rhic_amount { get; set; }
         public decimal ltc_member_three_yrs_premium_amount { get; set; }
         public decimal ltc_member_five_yrs_premium_amount { get; set; }
         public decimal ltc_spouse_three_yrs_premium_amount { get; set; }
         public decimal ltc_spouse_five_yrs_premium_amount { get; set; }
         public decimal provider_premium_amount { get; set; }
         public string group_number { get; set; }
         public int payment_election_adjustment_id { get; set; }
         public decimal paid_premium_amount { get; set; }
         public decimal life_basic_coverage_amount { get; set; }
         public decimal life_supp_coverage_amount { get; set; }
         public decimal life_spouse_supp_coverage_amount { get; set; }
         public decimal life_dep_supp_coverage_amount { get; set; }
         public decimal ad_and_d_basic_premium_rate { get; set; }
         public decimal ad_and_d_supplemental_premium_rate { get; set; }
         public int provider_org_id { get; set; }
         public string rate_structure_code { get; set; }
         public string coverage_code_value { get; set; }
         public decimal buydown_amount { get; set; }
         public decimal medicare_part_d_amt { get; set; }
         public string rhic_sent { get; set; }
         public int detail_status_id { get; set; }
         public string detail_status_description { get; set; }
         public string detail_status_value { get; set; }
         public decimal lis_amount { get; set; }
         public decimal lep_amount { get; set; }
    }
    [Serializable]
    public enum enmIbsDetail
    {
         ibs_detail_id ,
         ibs_header_id ,
         person_id ,
         plan_id ,
         billing_month_and_year ,
         mode_of_payment_id ,
         mode_of_payment_description ,
         mode_of_payment_value ,
         coverage_code ,
         member_premium_amount ,
         rhic_amount ,
         total_premium_amount ,
         balance_forward ,
         comment ,
         group_health_fee_amt ,
         person_account_id ,
         deposit_id ,
         life_basic_premium_amount ,
         life_supp_premium_amount ,
         life_spouse_supp_premium_amount ,
         life_dep_supp_premium_amount ,
         othr_rhic_amount ,
         js_rhic_amount ,
         ltc_member_three_yrs_premium_amount ,
         ltc_member_five_yrs_premium_amount ,
         ltc_spouse_three_yrs_premium_amount ,
         ltc_spouse_five_yrs_premium_amount ,
         provider_premium_amount ,
         group_number ,
         payment_election_adjustment_id ,
         paid_premium_amount ,
         life_basic_coverage_amount ,
         life_supp_coverage_amount ,
         life_spouse_supp_coverage_amount ,
         life_dep_supp_coverage_amount ,
         ad_and_d_basic_premium_rate ,
         ad_and_d_supplemental_premium_rate ,
         provider_org_id ,
         rate_structure_code ,
         coverage_code_value ,
         buydown_amount ,
         medicare_part_d_amt ,
         rhic_sent ,
         detail_status_id ,
         detail_status_description ,
         detail_status_value ,
         lis_amount ,
         lep_amount ,
    }
}

