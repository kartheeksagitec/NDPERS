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
	/// Class NeoSpin.DataObjects.doWssBenApp:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssBenApp : doBase
    {
         public doWssBenApp() : base()
         {
         }
         public int wss_ben_app_id { get; set; }
         public int person_id { get; set; }
         public int plan_id { get; set; }
         public int ben_type_id { get; set; }
         public string ben_type_description { get; set; }
         public string ben_type_value { get; set; }
         public int ben_opt_id { get; set; }
         public string ben_opt_description { get; set; }
         public string ben_opt_value { get; set; }
         public int ben_action_status_id { get; set; }
         public string ben_action_status_description { get; set; }
         public string ben_action_status_value { get; set; }
         public DateTime retirement_date { get; set; }
         public string is_deferred { get; set; }
         public DateTime deferred_date { get; set; }
         public string ref_dist_value { get; set; }
         public string ref_state_tax_not_withhold { get; set; }
         public int rhic_option_id { get; set; }
         public string rhic_option_description { get; set; }
         public string rhic_option_value { get; set; }
         public int graduated_benefit_option_id { get; set; }
         public string graduated_benefit_option_description { get; set; }
         public string graduated_benefit_option_value { get; set; }
         public string plso_requested_flag { get; set; }
         public string rollover_plso_flag { get; set; }
         public string sick_leave_purchase_indicated_flag { get; set; }
         public string dep_medicare_plan_option_value { get; set; }
         public string continue_flex_med_spending { get; set; }
         public string pay_premium_pre_tax { get; set; }
         public string pay_premium_post_tax { get; set; }
         public string is_social_security_applied { get; set; }
         public string is_worker_comp_benefit_applied { get; set; }
         public int bene_appl_id { get; set; }
         public string otp { get; set; }
         public DateTime otp_expiry_date { get; set; }
         public string is_otp_validated { get; set; }
         public string is_hpacknowledgement_checked { get; set; }
         public string is_dpacknowledgement_checked { get; set; }
         public string is_vpacknowledgement_checked { get; set; }
         public string is_mpacknowledgement_checked { get; set; }
         public string is_inspayachdtls_acknowledgement_checked { get; set; }
         public DateTime termination_date { get; set; }
         public string is_lpacknowledgement_checked { get; set; }
         public string is_gbacknowledgement_checked { get; set; }
         public string is_slcacknowledgement_checked { get; set; }
         public string is_fpacknowledgement_checked { get; set; }
         public string is_plsoacknowledgement_checked { get; set; }
         public string is_apdbdcacknowledgement_checked { get; set; }
         public string last_step_where_user_left { get; set; }
    }
    [Serializable]
    public enum enmWssBenApp
    {
         wss_ben_app_id ,
         person_id ,
         plan_id ,
         ben_type_id ,
         ben_type_description ,
         ben_type_value ,
         ben_opt_id ,
         ben_opt_description ,
         ben_opt_value ,
         ben_action_status_id ,
         ben_action_status_description ,
         ben_action_status_value ,
         retirement_date ,
         is_deferred ,
         deferred_date ,
         ref_dist_value ,
         ref_state_tax_not_withhold ,
         rhic_option_id ,
         rhic_option_description ,
         rhic_option_value ,
         graduated_benefit_option_id ,
         graduated_benefit_option_description ,
         graduated_benefit_option_value ,
         plso_requested_flag ,
         rollover_plso_flag ,
         sick_leave_purchase_indicated_flag ,
         dep_medicare_plan_option_value ,
         continue_flex_med_spending ,
         pay_premium_pre_tax ,
         pay_premium_post_tax ,
         is_social_security_applied ,
         is_worker_comp_benefit_applied ,
         bene_appl_id ,
         otp ,
         otp_expiry_date ,
         is_otp_validated ,
         is_hpacknowledgement_checked ,
         is_dpacknowledgement_checked ,
         is_vpacknowledgement_checked ,
         is_mpacknowledgement_checked ,
         is_inspayachdtls_acknowledgement_checked ,
         termination_date ,
         is_lpacknowledgement_checked ,
         is_gbacknowledgement_checked ,
         is_slcacknowledgement_checked ,
         is_fpacknowledgement_checked ,
         is_plsoacknowledgement_checked ,
         is_apdbdcacknowledgement_checked ,
         last_step_where_user_left,
    }
}
