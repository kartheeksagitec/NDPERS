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
	/// Class NeoSpin.DataObjects.doServicePurchaseDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doServicePurchaseDetail : doBase
    {
         
         public doServicePurchaseDetail() : base()
         {
         }
         public int service_purchase_detail_id { get; set; }
         public int service_purchase_header_id { get; set; }
         public decimal benefit_multiplier { get; set; }
         public decimal rhic_multiplier { get; set; }
         public DateTime project_retirement_date { get; set; }
         public decimal retirement_benefit_purchased { get; set; }
         public decimal rhic_benefit_purchased { get; set; }
         public decimal retirement_actuarial_factor { get; set; }
         public decimal rhic_retirement_factor { get; set; }
         public decimal retirement_purchase_cost { get; set; }
         public decimal rhic_purchase_cost { get; set; }
         public decimal total_refund_and_interest { get; set; }
         public decimal total_purchase_cost { get; set; }
         public decimal earliest_normal_retirement_age { get; set; }
         public string judges_conversion_flag { get; set; }
         public DateTime termination_date { get; set; }
         public DateTime userra_active_duty_start_date { get; set; }
         public DateTime userra_active_duty_end_date { get; set; }
         public DateTime userra_missed_salary_verification_date { get; set; }
         public string verifier { get; set; }
         public decimal unused_sick_leave_hours { get; set; }
         public DateTime sick_leave_data_confirmed_date { get; set; }
         public string sick_leave_confirmed_by { get; set; }
         public int sick_leave_time_purchased { get; set; }
         public decimal retirement_cost_for_sick_leave_purchase { get; set; }
         public decimal rhic_cost_for_sick_leave_purchase { get; set; }
         public decimal total_time_to_purchase { get; set; }
         public decimal total_time_to_purchase_exclude_free_service { get; set; }
         public decimal total_time_to_purchase_contribution_months { get; set; }
         public string ready_for_posting_flag { get; set; }
         public decimal ret_act_factor_with_pur { get; set; }
         public decimal rhic_ret_factor_with_pur { get; set; }
         public decimal employee_contribution_rate { get; set; }
         public decimal fut_ee_act_factor_without_pur { get; set; }
         public decimal fut_ee_act_factor_with_pur { get; set; }
         public decimal ret_cost_without_purchase { get; set; }
         public decimal ret_cost_with_purchase { get; set; }
         public decimal rhic_cost_without_purchase { get; set; }
         public decimal rhic_cost_with_purchase { get; set; }
         public decimal fut_emp_cost_without_pur { get; set; }
         public decimal fut_emp_cost_with_pur { get; set; }
         public decimal earliest_nor_ret_age_without_service_purchased { get; set; }
         public decimal vsc { get; set; }
         public decimal psc { get; set; }
    }
    [Serializable]
    public enum enmServicePurchaseDetail
    {
         service_purchase_detail_id ,
         service_purchase_header_id ,
         benefit_multiplier ,
         rhic_multiplier ,
         project_retirement_date ,
         retirement_benefit_purchased ,
         rhic_benefit_purchased ,
         retirement_actuarial_factor ,
         rhic_retirement_factor ,
         retirement_purchase_cost ,
         rhic_purchase_cost ,
         total_refund_and_interest ,
         total_purchase_cost ,
         earliest_normal_retirement_age ,
         judges_conversion_flag ,
         termination_date ,
         userra_active_duty_start_date ,
         userra_active_duty_end_date ,
         userra_missed_salary_verification_date ,
         verifier ,
         unused_sick_leave_hours ,
         sick_leave_data_confirmed_date ,
         sick_leave_confirmed_by ,
         sick_leave_time_purchased ,
         retirement_cost_for_sick_leave_purchase ,
         rhic_cost_for_sick_leave_purchase ,
         total_time_to_purchase ,
         total_time_to_purchase_exclude_free_service ,
         total_time_to_purchase_contribution_months ,
         ready_for_posting_flag ,
         ret_act_factor_with_pur ,
         rhic_ret_factor_with_pur ,
         employee_contribution_rate ,
         fut_ee_act_factor_without_pur ,
         fut_ee_act_factor_with_pur ,
         ret_cost_without_purchase ,
         ret_cost_with_purchase ,
         rhic_cost_without_purchase ,
         rhic_cost_with_purchase ,
         fut_emp_cost_without_pur ,
         fut_emp_cost_with_pur ,
         earliest_nor_ret_age_without_service_purchased ,
         vsc ,
         psc ,
    }
}

