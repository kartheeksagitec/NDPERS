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
	/// Class NeoSpin.DataObjects.doMasPersonPlan:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasPersonPlan : doBase
    {
         
         public doMasPersonPlan() : base()
         {
         }
         public int mas_person_plan_id { get; set; }
         public int mas_person_id { get; set; }
         public int plan_id { get; set; }
         public string plan_name { get; set; }
         public decimal previous_member_account_balance_ltd { get; set; }
         public string is_eligible_for_enrol_flag { get; set; }
         public string provider_name { get; set; }
         public decimal vested_employer_cont { get; set; }
         public decimal service_purchase_amount { get; set; }
         public decimal interest_amount { get; set; }
         public decimal taxable_amount { get; set; }
         public decimal nontaxable_amount { get; set; }
         public decimal vested_employer_cont_percent { get; set; }
         public decimal ytd_member_contributions { get; set; }
         public decimal ytd_employer_contributions { get; set; }
         public decimal qdro_amount { get; set; }
         public string is_rtw_indicator { get; set; }
         public decimal ee_rhic_contribution_amount { get; set; }
         public decimal member_account_balance_ltd { get; set; }
         public decimal ltd_member_contributions { get; set; }
         public decimal ltd_employer_contributions { get; set; }
         public decimal psc_in_year_months { get; set; }
         public decimal tvsc_in_year_months { get; set; }
         public decimal normal_retirement_age { get; set; }
         public DateTime normal_retirement_date { get; set; }
         public decimal single_life_benefit_amount { get; set; }
         public decimal rhic_amount { get; set; }
         public decimal tffr_service { get; set; }
         public decimal tiaa_cref_service { get; set; }
         public decimal jobservice_sick_leave { get; set; }
         public decimal member_contributions { get; set; }
         public decimal tier1 { get; set; }
         public decimal tier2 { get; set; }
         public decimal tier3 { get; set; }
         public string is_db_plan { get; set; }
         public string is_dc_plan { get; set; }
        public int person_account_id { get; set; }
        public string plan_benefit_tier_description { get; set; }
    }
    [Serializable]
    public enum enmMasPersonPlan
    {
         mas_person_plan_id ,
         mas_person_id ,
         plan_id ,
         plan_name ,
         previous_member_account_balance_ltd ,
         is_eligible_for_enrol_flag ,
         provider_name ,
         vested_employer_cont ,
         service_purchase_amount ,
         interest_amount ,
         taxable_amount ,
         nontaxable_amount ,
         vested_employer_cont_percent ,
         ytd_member_contributions ,
         ytd_employer_contributions ,
         qdro_amount ,
         is_rtw_indicator ,
         ee_rhic_contribution_amount ,
         member_account_balance_ltd ,
         ltd_member_contributions ,
         ltd_employer_contributions ,
         psc_in_year_months ,
         tvsc_in_year_months ,
         normal_retirement_age ,
         normal_retirement_date ,
         single_life_benefit_amount ,
         rhic_amount ,
         tffr_service ,
         tiaa_cref_service ,
         jobservice_sick_leave ,
         member_contributions ,
         tier1 ,
         tier2 ,
         tier3 ,
         is_db_plan ,
         is_dc_plan ,
        person_account_id,
        plan_benefit_tier_description,
    }
}

