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
	/// Class NeoSpin.DataObjects.doBenefitDroApplication:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitDroApplication : doBase
    {
         public doBenefitDroApplication() : base()
         {
         }
         public int dro_application_id { get; set; }
         public int member_perslink_id { get; set; }
         public int alternate_payee_perslink_id { get; set; }
         public int dro_model_id { get; set; }
         public string dro_model_description { get; set; }
         public string dro_model_value { get; set; }
         public int plan_id { get; set; }
         public int person_account_id { get; set; }
         public string case_number { get; set; }
         public DateTime date_of_marriage { get; set; }
         public DateTime date_of_divorce { get; set; }
         public DateTime received_date { get; set; }
         public string approved_by_user { get; set; }
         public DateTime approved_date { get; set; }
         public string qualified_by_user { get; set; }
         public DateTime qualified_date { get; set; }
         public decimal monthly_benefit_percentage { get; set; }
         public decimal monthly_benefit_amount { get; set; }
         public int time_of_benefit_receipt_calc_id { get; set; }
         public string time_of_benefit_receipt_calc_description { get; set; }
         public string time_of_benefit_receipt_calc_value { get; set; }
         public DateTime benefit_receipt_date { get; set; }
         public int benefit_duration_option_id { get; set; }
         public string benefit_duration_option_description { get; set; }
         public string benefit_duration_option_value { get; set; }
         public decimal member_withdrawal_percentage { get; set; }
         public decimal member_withdrawal_amount { get; set; }
         public decimal alternate_payee_death_percentage { get; set; }
         public decimal member_death_percentage { get; set; }
         public decimal computed_ee_pre_tax_amount { get; set; }
         public decimal computed_ee_post_tax_amount { get; set; }
         public decimal computed_ee_er_pickup_amount { get; set; }
         public decimal computed_er_vested_amount { get; set; }
         public decimal computed_interest_amount { get; set; }
         public decimal overridden_ee_pre_tax_amount { get; set; }
         public decimal overridden_ee_post_tax_amount { get; set; }
         public decimal overridden_ee_er_pickup_amount { get; set; }
         public decimal overridden_er_vested_amount { get; set; }
         public decimal overridden_interest_amount { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int dro_status_id { get; set; }
         public string dro_status_description { get; set; }
         public string dro_status_value { get; set; }
         public decimal lumpsum_payment_percentage { get; set; }
         public decimal lumpsum_payment_amount { get; set; }
         public decimal computed_capital_gain { get; set; }
         public decimal overridden_capital_gain { get; set; }
         public decimal computed_member_gross_monthly_amount { get; set; }
         public decimal overridden_member_gross_monthly_amount { get; set; }
         public string letter_sent_488_days_flag { get; set; }
         public string letter_sent_503_days_flag { get; set; }
         public string letter_sent_18_months_flag { get; set; }
         public string work_flow_intiated_flag { get; set; }
         public string pending_nullification_by { get; set; }
         public DateTime pending_nullification_date { get; set; }
    }
    [Serializable]
    public enum enmBenefitDroApplication
    {
         dro_application_id ,
         member_perslink_id ,
         alternate_payee_perslink_id ,
         dro_model_id ,
         dro_model_description ,
         dro_model_value ,
         plan_id ,
         person_account_id ,
         case_number ,
         date_of_marriage ,
         date_of_divorce ,
         received_date ,
         approved_by_user ,
         approved_date ,
         qualified_by_user ,
         qualified_date ,
         monthly_benefit_percentage ,
         monthly_benefit_amount ,
         time_of_benefit_receipt_calc_id ,
         time_of_benefit_receipt_calc_description ,
         time_of_benefit_receipt_calc_value ,
         benefit_receipt_date ,
         benefit_duration_option_id ,
         benefit_duration_option_description ,
         benefit_duration_option_value ,
         member_withdrawal_percentage ,
         member_withdrawal_amount ,
         alternate_payee_death_percentage ,
         member_death_percentage ,
         computed_ee_pre_tax_amount ,
         computed_ee_post_tax_amount ,
         computed_ee_er_pickup_amount ,
         computed_er_vested_amount ,
         computed_interest_amount ,
         overridden_ee_pre_tax_amount ,
         overridden_ee_post_tax_amount ,
         overridden_ee_er_pickup_amount ,
         overridden_er_vested_amount ,
         overridden_interest_amount ,
         status_id ,
         status_description ,
         status_value ,
         dro_status_id ,
         dro_status_description ,
         dro_status_value ,
         lumpsum_payment_percentage ,
         lumpsum_payment_amount ,
         computed_capital_gain ,
         overridden_capital_gain ,
         computed_member_gross_monthly_amount ,
         overridden_member_gross_monthly_amount ,
         letter_sent_488_days_flag ,
         letter_sent_503_days_flag ,
         letter_sent_18_months_flag ,
         work_flow_intiated_flag ,
         pending_nullification_by ,
         pending_nullification_date ,
    }
}

