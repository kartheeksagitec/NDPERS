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
	/// Class NeoSpin.DataObjects.doBenefitApplication:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitApplication : doBase
    {
         public doBenefitApplication() : base()
         {
         }
         public int benefit_application_id { get; set; }
         public int member_person_id { get; set; }
         public int plan_id { get; set; }
         public int payee_org_id { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public int action_status_id { get; set; }
         public string action_status_description { get; set; }
         public string action_status_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime retirement_date { get; set; }
         public DateTime termination_date { get; set; }
         public DateTime received_date { get; set; }
         public string plso_requested_flag { get; set; }
         public string uniform_income_flag { get; set; }
         public decimal ssli_age { get; set; }
         public decimal estimated_ssli_benefit_amount { get; set; }
         public int rhic_option_id { get; set; }
         public string rhic_option_description { get; set; }
         public string rhic_option_value { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string suppress_warnings_by { get; set; }
         public DateTime suppress_warnings_date { get; set; }
         public string sick_leave_purchase_indicated_flag { get; set; }
         public string disability_approved_by { get; set; }
         public int recipient_person_id { get; set; }
         public int joint_annuitant_perslink_id { get; set; }
         public string reduced_benefit_flag { get; set; }
         public int benefit_sub_type_id { get; set; }
         public string benefit_sub_type_description { get; set; }
         public string benefit_sub_type_value { get; set; }
         public string dnro_flag { get; set; }
         public int letter_sent { get; set; }
         public int retirement_org_id { get; set; }
         public int tffr_calculation_method_id { get; set; }
         public string tffr_calculation_method_description { get; set; }
         public string tffr_calculation_method_value { get; set; }
         public decimal paid_up_annuity_amount { get; set; }
         public string early_reduction_waived_flag { get; set; }
         public int account_relationship_id { get; set; }
         public string account_relationship_description { get; set; }
         public string account_relationship_value { get; set; }
         public int family_relationship_id { get; set; }
         public string family_relationship_description { get; set; }
         public string family_relationship_value { get; set; }
         public DateTime payment_date { get; set; }
         public string thirty_days_waiver_flag { get; set; }
         public string hardship_approved_flag { get; set; }
         public DateTime action_status_effective_date { get; set; }
         public string qdro_on_file_flag { get; set; }
         public int nd_univ_code_id { get; set; }
         public string nd_univ_code_description { get; set; }
         public string nd_univ_code_value { get; set; }
         public DateTime deferral_date { get; set; }
         public decimal rtw_benefit_option_factor { get; set; }
         public int rtw_refund_election_id { get; set; }
         public string rtw_refund_election_description { get; set; }
         public string rtw_refund_election_value { get; set; }
         public string is_rtw_less_than_2years_flag { get; set; }
         public int pre_rtw_payeeaccount_id { get; set; }
         public int originating_payee_account_id { get; set; }
         public int post_retirement_death_reason_type_id { get; set; }
         public string post_retirement_death_reason_type_description { get; set; }
         public string post_retirement_death_reason_type_value { get; set; }
         public int beneficiary_person_id { get; set; }
         public DateTime ssli_effective_date { get; set; }
         public int graduated_benefit_option_id { get; set; }
         public string graduated_benefit_option_description { get; set; }
         public string graduated_benefit_option_value { get; set; }
    }
    [Serializable]
    public enum enmBenefitApplication
    {
         benefit_application_id ,
         member_person_id ,
         plan_id ,
         payee_org_id ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         action_status_id ,
         action_status_description ,
         action_status_value ,
         status_id ,
         status_description ,
         status_value ,
         retirement_date ,
         termination_date ,
         received_date ,
         plso_requested_flag ,
         uniform_income_flag ,
         ssli_age ,
         estimated_ssli_benefit_amount ,
         rhic_option_id ,
         rhic_option_description ,
         rhic_option_value ,
         suppress_warnings_flag ,
         suppress_warnings_by ,
         suppress_warnings_date ,
         sick_leave_purchase_indicated_flag ,
         disability_approved_by ,
         recipient_person_id ,
         joint_annuitant_perslink_id ,
         reduced_benefit_flag ,
         benefit_sub_type_id ,
         benefit_sub_type_description ,
         benefit_sub_type_value ,
         dnro_flag ,
         letter_sent ,
         retirement_org_id ,
         tffr_calculation_method_id ,
         tffr_calculation_method_description ,
         tffr_calculation_method_value ,
         paid_up_annuity_amount ,
         early_reduction_waived_flag ,
         account_relationship_id ,
         account_relationship_description ,
         account_relationship_value ,
         family_relationship_id ,
         family_relationship_description ,
         family_relationship_value ,
         payment_date ,
         thirty_days_waiver_flag ,
         hardship_approved_flag ,
         action_status_effective_date ,
         qdro_on_file_flag ,
         nd_univ_code_id ,
         nd_univ_code_description ,
         nd_univ_code_value ,
         deferral_date ,
         rtw_benefit_option_factor ,
         rtw_refund_election_id ,
         rtw_refund_election_description ,
         rtw_refund_election_value ,
         is_rtw_less_than_2years_flag ,
         pre_rtw_payeeaccount_id ,
         originating_payee_account_id ,
         post_retirement_death_reason_type_id ,
         post_retirement_death_reason_type_description ,
         post_retirement_death_reason_type_value ,
         beneficiary_person_id ,
         ssli_effective_date ,
         graduated_benefit_option_id ,
         graduated_benefit_option_description ,
         graduated_benefit_option_value ,
    }
}

