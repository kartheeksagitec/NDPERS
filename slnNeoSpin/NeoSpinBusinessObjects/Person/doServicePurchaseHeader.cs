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
	/// Class NeoSpin.DataObjects.doServicePurchaseHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doServicePurchaseHeader : doBase
    {
         
         public doServicePurchaseHeader() : base()
         {
         }
         public int service_purchase_header_id { get; set; }
         public int person_id { get; set; }
         public int action_status_id { get; set; }
         public string action_status_description { get; set; }
         public string action_status_value { get; set; }
         public int service_purchase_status_id { get; set; }
         public string service_purchase_status_description { get; set; }
         public string service_purchase_status_value { get; set; }
         public int service_purchase_type_id { get; set; }
         public string service_purchase_type_description { get; set; }
         public string service_purchase_type_value { get; set; }
         public DateTime date_of_purchase { get; set; }
         public DateTime expiration_date { get; set; }
         public int current_age_month_part { get; set; }
         public int current_age_year_part { get; set; }
         public int plan_id { get; set; }
         public int payor_id { get; set; }
         public string payor_description { get; set; }
         public string payor_value { get; set; }
         public decimal final_average_salary { get; set; }
         public decimal total_contract_amount { get; set; }
         public decimal down_payment { get; set; }
         public decimal contract_interest { get; set; }
         public string limit_415_flag { get; set; }
         public string grant_free_flag { get; set; }
         public int free_or_dual_service { get; set; }
         public decimal prorated_vsc { get; set; }
         public decimal prorated_psc { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string suppressed_by { get; set; }
         public decimal expected_installment_amount { get; set; }
         public int number_of_payments { get; set; }
         public int payment_frequency_id { get; set; }
         public string payment_frequency_description { get; set; }
         public string payment_frequency_value { get; set; }
         public string payroll_deduction { get; set; }
         public string pre_tax { get; set; }
         public int contact_ticket_id { get; set; }
         public int member_type_id { get; set; }
         public string member_type_description { get; set; }
         public string member_type_value { get; set; }
         public decimal paid_service_credit { get; set; }
         public decimal paid_free_service_credit { get; set; }
         public decimal paid_rhic_cost_amount { get; set; }
         public decimal paid_retirement_ee_cost_amount { get; set; }
         public decimal paid_retirement_er_cost_amount { get; set; }
         public decimal paid_contract_amount_used { get; set; }
         public string delinquent_letter1_sent_flag { get; set; }
         public string delinquent_letter2_sent_flag { get; set; }
         public decimal service_purchase_adjustment_fraction_psc { get; set; }
         public decimal service_purchase_adjustment_fraction_vsc { get; set; }
         public int be_with_service_purchase_calc_id { get; set; }
         public int be_without_service_purchase_calc_id { get; set; }
         public int be_death_with_service_purchase_calc_id { get; set; }
         public int be_death_without_service_purchase_calc_id { get; set; }
         public string is_created_from_portal { get; set; }
         public int benefit_calculation_id { get; set; }
         public decimal overridden_final_average_salary { get; set; }
    }
    [Serializable]
    public enum enmServicePurchaseHeader
    {
         service_purchase_header_id ,
         person_id ,
         action_status_id ,
         action_status_description ,
         action_status_value ,
         service_purchase_status_id ,
         service_purchase_status_description ,
         service_purchase_status_value ,
         service_purchase_type_id ,
         service_purchase_type_description ,
         service_purchase_type_value ,
         date_of_purchase ,
         expiration_date ,
         current_age_month_part ,
         current_age_year_part ,
         plan_id ,
         payor_id ,
         payor_description ,
         payor_value ,
         final_average_salary ,
         total_contract_amount ,
         down_payment ,
         contract_interest ,
         limit_415_flag ,
         grant_free_flag ,
         free_or_dual_service ,
         prorated_vsc ,
         prorated_psc ,
         suppress_warnings_flag ,
         suppressed_by ,
         expected_installment_amount ,
         number_of_payments ,
         payment_frequency_id ,
         payment_frequency_description ,
         payment_frequency_value ,
         payroll_deduction ,
         pre_tax ,
         contact_ticket_id ,
         member_type_id ,
         member_type_description ,
         member_type_value ,
         paid_service_credit ,
         paid_free_service_credit ,
         paid_rhic_cost_amount ,
         paid_retirement_ee_cost_amount ,
         paid_retirement_er_cost_amount ,
         paid_contract_amount_used ,
         delinquent_letter1_sent_flag ,
         delinquent_letter2_sent_flag ,
         service_purchase_adjustment_fraction_psc ,
         service_purchase_adjustment_fraction_vsc ,
         be_with_service_purchase_calc_id ,
         be_without_service_purchase_calc_id ,
         be_death_with_service_purchase_calc_id ,
         be_death_without_service_purchase_calc_id ,
         is_created_from_portal ,
         benefit_calculation_id ,
         overridden_final_average_salary ,
    }
}

