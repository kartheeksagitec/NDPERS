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
	/// Class NeoSpin.DataObjects.doActuaryFilePensionDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doActuaryFilePensionDetail : doBase
    {
         
         public doActuaryFilePensionDetail() : base()
         {
         }
         public int actuary_file_pension_detail_id { get; set; }
         public int actuary_file_header_id { get; set; }
         public int person_id { get; set; }
         public int payee_account_id { get; set; }
         public int person_account_id { get; set; }
         public int plan_participation_status_id { get; set; }
         public string plan_participation_status_description { get; set; }
         public int account_relation_id { get; set; }
         public string account_relation_description { get; set; }
         public string account_relation_value { get; set; }
         public string plan_participation_status_value { get; set; }
         public int employment_status_id { get; set; }
         public string employment_status_description { get; set; }
         public string employment_status_value { get; set; }
         public int application_status_id { get; set; }
         public string application_status_description { get; set; }
         public string application_status_value { get; set; }
         public int payee_status_id { get; set; }
         public string payee_status_description { get; set; }
         public string payee_status_value { get; set; }
         public int employment_type_id { get; set; }
         public string employment_type_description { get; set; }
         public string employment_type_value { get; set; }
         public int hourly_id { get; set; }
         public string hourly_description { get; set; }
         public string hourly_value { get; set; }
         public int seasonal_id { get; set; }
         public string seasonal_description { get; set; }
         public string seasonal_value { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
         public DateTime plan_participation_start_date { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public decimal pension_service_credit { get; set; }
         public decimal total_vested_service_credit { get; set; }
         public DateTime retirement_date { get; set; }
         public DateTime spouse_dob { get; set; }
         public DateTime joint_annuitant_dob { get; set; }
         public int org_id { get; set; }
         public decimal total_salary { get; set; }
         public decimal account_balance { get; set; }
         public decimal pension_service_credit_ba { get; set; }
         public decimal total_vested_service_credit_ba { get; set; }
         public decimal accrued_benefit_calculation { get; set; }
         public decimal minimum_guarantee_amount { get; set; }
         public decimal gross_current_monthly_benefit_amount { get; set; }
         public decimal amount_paid_ltd { get; set; }
         public decimal purchased_service_credit_sum { get; set; }
         public decimal ssli_or_uniform_income_commencement_age { get; set; }
         public decimal estimated_ssli_benefit_amount { get; set; }
         public decimal option_factor { get; set; }
         public decimal vested_er_amount { get; set; }
         public DateTime ssli_change_date { get; set; }
         public decimal travellers_base_benefit_amount { get; set; }
         public decimal travellers_cumulative_cola_amount { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int benefit_sub_type_id { get; set; }
         public string benefit_sub_type_description { get; set; }
         public string benefit_sub_type_value { get; set; }
    }
    [Serializable]
    public enum enmActuaryFilePensionDetail
    {
         actuary_file_pension_detail_id ,
         actuary_file_header_id ,
         person_id ,
         payee_account_id ,
         person_account_id ,
         plan_participation_status_id ,
         plan_participation_status_description ,
         account_relation_id ,
         account_relation_description ,
         account_relation_value ,
         plan_participation_status_value ,
         employment_status_id ,
         employment_status_description ,
         employment_status_value ,
         application_status_id ,
         application_status_description ,
         application_status_value ,
         payee_status_id ,
         payee_status_description ,
         payee_status_value ,
         employment_type_id ,
         employment_type_description ,
         employment_type_value ,
         hourly_id ,
         hourly_description ,
         hourly_value ,
         seasonal_id ,
         seasonal_description ,
         seasonal_value ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
         plan_participation_start_date ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         pension_service_credit ,
         total_vested_service_credit ,
         retirement_date ,
         spouse_dob ,
         joint_annuitant_dob ,
         org_id ,
         total_salary ,
         account_balance ,
         pension_service_credit_ba ,
         total_vested_service_credit_ba ,
         accrued_benefit_calculation ,
         minimum_guarantee_amount ,
         gross_current_monthly_benefit_amount ,
         amount_paid_ltd ,
         purchased_service_credit_sum ,
         ssli_or_uniform_income_commencement_age ,
         estimated_ssli_benefit_amount ,
         option_factor ,
         vested_er_amount ,
         ssli_change_date ,
         travellers_base_benefit_amount ,
         travellers_cumulative_cola_amount ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         benefit_sub_type_id ,
         benefit_sub_type_description ,
         benefit_sub_type_value ,
    }
}

