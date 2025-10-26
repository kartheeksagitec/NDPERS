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
	/// Class .DataObjects.doEmployerPayrollHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doEmployerPayrollHeader : doBase
    {
        
         public doEmployerPayrollHeader() : base()
         {
         }
         public int employer_payroll_header_id { get; set; }
         public int org_id { get; set; }
         public int header_type_id { get; set; }
         public string header_type_description { get; set; }
         public string header_type_value { get; set; }
         public int reporting_source_id { get; set; }
         public string reporting_source_description { get; set; }
         public string reporting_source_value { get; set; }
         public int report_type_id { get; set; }
         public string report_type_description { get; set; }
         public string report_type_value { get; set; }
         public DateTime payroll_paid_date { get; set; }
         public DateTime pay_period_start_date { get; set; }
         public DateTime pay_period_end_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime received_date { get; set; }
         public string interest_waiver_flag { get; set; }
         public int balancing_status_id { get; set; }
         public string balancing_status_description { get; set; }
         public string balancing_status_value { get; set; }
         public decimal total_contribution_reported { get; set; }
         public decimal total_contribution_calculated { get; set; }
         public decimal total_wages_reported { get; set; }
         public decimal total_wages_calculated { get; set; }
         public decimal total_interest_reported { get; set; }
         public decimal total_interest_calculated { get; set; }
         public int total_detail_record_count { get; set; }
         public decimal total_purchase_amount { get; set; }
         public decimal total_premium_amount_reported { get; set; }
         public decimal total_purchase_amount_reported { get; set; }
         public string comments { get; set; }
         public DateTime submitted_date { get; set; }
         public DateTime validated_date { get; set; }
         public DateTime posted_date { get; set; }
         public int central_payroll_record_id { get; set; }
         public string ignore_balancing_status_flag { get; set; }
         public DateTime last_reload_run_date { get; set; }
         public DateTime pay_check_date { get; set; }
         public string suppress_salary_variance_validation_flag { get; set; }
         public decimal total_wages_original { get; set; }
         public decimal total_contribution_original { get; set; }
         public decimal total_purchase_amount_original { get; set; }
         public string suppress_3rd_payroll_flag { get; set; }
         public int original_employer_payroll_header_id { get; set; }
         public string detail_comments { get; set; }
		 //PIR 25920 New Plan DC 2025
         public decimal total_adec_amount_reported { get; set; }
         public decimal total_adec_amount_calculated { get; set; }
        public decimal total_adec_amount_original { get; set; }
    }
    [Serializable]
    public enum enmEmployerPayrollHeader
    {
         employer_payroll_header_id ,
         org_id ,
         header_type_id ,
         header_type_description ,
         header_type_value ,
         reporting_source_id ,
         reporting_source_description ,
         reporting_source_value ,
         report_type_id ,
         report_type_description ,
         report_type_value ,
         payroll_paid_date ,
         pay_period_start_date ,
         pay_period_end_date ,
         status_id ,
         status_description ,
         status_value ,
         received_date ,
         interest_waiver_flag ,
         balancing_status_id ,
         balancing_status_description ,
         balancing_status_value ,
         total_contribution_reported ,
         total_contribution_calculated ,
         total_wages_reported ,
         total_wages_calculated ,
         total_interest_reported ,
         total_interest_calculated ,
         total_detail_record_count ,
         total_purchase_amount ,
         total_premium_amount_reported ,
         total_purchase_amount_reported ,
         comments ,
         submitted_date ,
         validated_date ,
         posted_date ,
         central_payroll_record_id ,
         ignore_balancing_status_flag ,
         last_reload_run_date ,
         pay_check_date ,
         suppress_salary_variance_validation_flag ,
         total_wages_original ,
         total_contribution_original ,
         total_purchase_amount_original ,
         suppress_3rd_payroll_flag ,
         original_employer_payroll_header_id ,
         detail_comments ,
        total_adec_amount_reported,
        total_adec_amount_calculated,
        total_adec_amount_original,
    }
}

