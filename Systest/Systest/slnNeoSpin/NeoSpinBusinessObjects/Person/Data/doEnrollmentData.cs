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
	/// Class NeoSpin.DataObjects.doEnrollmentData:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doEnrollmentData : doBase
    {
         
         public doEnrollmentData() : base()
         {
         }
         public int enrollment_data_id { get; set; }
         public int source_id { get; set; }
         public int plan_id { get; set; }
         public string benefit_plan { get; set; }
         public string plan_type { get; set; }
         public string ssn { get; set; }
         public int ndpers_member_id { get; set; }
         public int person_account_id { get; set; }
         public string peoplesoft_id { get; set; }
         public int employer_org_id { get; set; }
         public int employment_type_id { get; set; }
         public string employment_type_description { get; set; }
         public string employment_type_value { get; set; }
         public int plan_status_id { get; set; }
         public string plan_status_description { get; set; }
         public string plan_status_value { get; set; }
         public int change_reason_id { get; set; }
         public string change_reason_description { get; set; }
         public string change_reason_value { get; set; }
         public int peoplesoft_change_reason_id { get; set; }
         public string peoplesoft_change_reason_description { get; set; }
         public string peoplesoft_change_reason_value { get; set; }
         public int coverage_election_id { get; set; }
         public string coverage_election_description { get; set; }
         public string coverage_election_value { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public DateTime coverage_begin_date { get; set; }
         public DateTime deduction_begin_date { get; set; }
         public DateTime election_date { get; set; }
         public decimal pay_period_amount { get; set; }
         public int provider_org_id { get; set; }
         public int plan_option_id { get; set; }
         public string plan_option_description { get; set; }
         public string plan_option_value { get; set; }
         public string coverage_code_for_health { get; set; }
         public int level_of_coverage_id { get; set; }
         public string level_of_coverage_description { get; set; }
         public string level_of_coverage_value { get; set; }
         public string coverage_code { get; set; }
         public decimal coverage_amount { get; set; }
         public string direct_deposit_flag { get; set; }
         public string inside_mail_flag { get; set; }
         public decimal monthly_premium { get; set; }
         public string pretaxed_premiums { get; set; }
         public decimal pretax_amount { get; set; }
         public string is_benefit_enrollment_report_generated { get; set; }
         public string peoplesoft_file_sent_flag { get; set; }
         public int peoplesoft_org_group_id { get; set; }
         public string peoplesoft_org_group_description { get; set; }
         public string peoplesoft_org_group_value { get; set; }
         public string lump_sum_payout { get; set; }
         public string employee_employer_match { get; set; }


    }
    [Serializable]
    public enum enmEnrollmentData
    {
         enrollment_data_id ,
         source_id ,
         plan_id ,
         benefit_plan ,
         plan_type ,
         ssn ,
         ndpers_member_id ,
         person_account_id ,
         peoplesoft_id ,
         employer_org_id ,
         employment_type_id ,
         employment_type_description ,
         employment_type_value ,
         plan_status_id ,
         plan_status_description ,
         plan_status_value ,
         change_reason_id ,
         change_reason_description ,
         change_reason_value ,
         peoplesoft_change_reason_id ,
         peoplesoft_change_reason_description ,
         peoplesoft_change_reason_value ,
         coverage_election_id ,
         coverage_election_description ,
         coverage_election_value ,
         start_date ,
         end_date ,
         coverage_begin_date ,
         deduction_begin_date ,
         election_date ,
         pay_period_amount ,
         provider_org_id ,
         plan_option_id ,
         plan_option_description ,
         plan_option_value ,
         coverage_code_for_health,
         level_of_coverage_id ,
         level_of_coverage_description ,
         level_of_coverage_value ,
         coverage_code ,
         coverage_amount ,
         direct_deposit_flag ,
         inside_mail_flag ,
         monthly_premium ,
         pretaxed_premiums ,
         pretax_amount ,
         is_benefit_enrollment_report_generated ,
         peoplesoft_file_sent_flag ,
         peoplesoft_org_group_id ,
         peoplesoft_org_group_description ,
         peoplesoft_org_group_value ,
         lump_sum_payout ,
         employee_employer_match,
    }
}
