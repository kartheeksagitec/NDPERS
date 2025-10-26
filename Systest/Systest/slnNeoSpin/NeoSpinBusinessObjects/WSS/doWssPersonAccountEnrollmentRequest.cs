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
	/// Class NeoSpin.DataObjects.doWssPersonAccountEnrollmentRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountEnrollmentRequest : doBase
    {
         
         public doWssPersonAccountEnrollmentRequest() : base()
         {
         }
         public int wss_person_account_enrollment_request_id { get; set; }
         public int person_id { get; set; }
         public int plan_id { get; set; }
         public int target_person_account_id { get; set; }
         public int person_employment_dtl_id { get; set; }
         public string is_enrolled_in_tffr_flag { get; set; }
         public string is_enrolled_in_tiaa_cref_flag { get; set; }
         public string employer_name { get; set; }
         public DateTime effective_from_date { get; set; }
         public DateTime effective_to_date { get; set; }
         public string agent_name { get; set; }
         public int reason_id { get; set; }
         public string reason_description { get; set; }
         public string reason_value { get; set; }
         public DateTime date_of_change { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string rejected_by { get; set; }
         public DateTime rejected_date { get; set; }
         public string comments { get; set; }
         public string ee_acknowledgement_agreement_flag { get; set; }
         public string acknolwedgement_part_c_flag { get; set; }
         public string acknolwedgement_part_d_flag { get; set; }
         public int enrollment_type_id { get; set; }
         public string enrollment_type_description { get; set; }
         public string enrollment_type_value { get; set; }
         public DateTime acknolwedgement_part_c_date { get; set; }
         public DateTime acknolwedgement_part_d_date { get; set; }
         public DateTime effective_reason_from_date { get; set; }
         public DateTime effective_reason_to_date { get; set; }
         public string acknolwedgement_part_b_flag { get; set; }
         public DateTime change_effective_date { get; set; }
         public string not_applicable { get; set; }
         public string rejection_reason { get; set; }
         public string employer_name1 { get; set; }
         public DateTime effective_from_date1 { get; set; }
         public DateTime effective_to_date1 { get; set; }
         public string ee_acknowledgement_waiver_flag { get; set; }
         public int provider_org_id { get; set; }
         public string is_enrollment_report_generated { get; set; }
         public int plan_enrollment_option_id { get; set; }
         public string plan_enrollment_option_description { get; set; }
         public string plan_enrollment_option_value { get; set; }
         public string is_changes_in_anne_flag { get; set; }
         public DateTime wss_hsa_contribution_start_date { get; set; }
         public decimal wss_hsa_contribution_amount { get; set; }

        public int wss_ben_app_id { get; set; }
        public string mss_retiree_flag { get; set; }
        public string is_applied_for_divorce { get; set; }
        public int addl_ee_contribution_percent { get; set; }       //PIR 25920 DC 2025 changes
        public int addl_ee_contribution_percent_temp { get; set; }       //PIR 25920 DC 2025 changes
    }
    [Serializable]
    public enum enmWssPersonAccountEnrollmentRequest
    {
         wss_person_account_enrollment_request_id ,
         person_id ,
         plan_id ,
         target_person_account_id ,
         person_employment_dtl_id ,
         is_enrolled_in_tffr_flag ,
         is_enrolled_in_tiaa_cref_flag ,
         employer_name ,
         effective_from_date ,
         effective_to_date ,
         agent_name ,
         reason_id ,
         reason_description ,
         reason_value ,
         date_of_change ,
         status_id ,
         status_description ,
         status_value ,
         rejected_by ,
         rejected_date ,
         comments ,
         ee_acknowledgement_agreement_flag ,
         acknolwedgement_part_c_flag ,
         acknolwedgement_part_d_flag ,
         enrollment_type_id ,
         enrollment_type_description ,
         enrollment_type_value ,
         acknolwedgement_part_c_date ,
         acknolwedgement_part_d_date ,
         effective_reason_from_date ,
         effective_reason_to_date ,
         acknolwedgement_part_b_flag ,
         change_effective_date ,
         not_applicable ,
         rejection_reason ,
         employer_name1 ,
         effective_from_date1 ,
         effective_to_date1 ,
         ee_acknowledgement_waiver_flag ,
         provider_org_id ,
         is_enrollment_report_generated ,
         plan_enrollment_option_id ,
         plan_enrollment_option_description ,
         plan_enrollment_option_value ,
         is_changes_in_anne_flag ,
         wss_hsa_contribution_start_date ,
         wss_hsa_contribution_amount ,
        wss_ben_app_id,
        mss_retiree_flag,
        is_applied_for_divorce,
        addl_ee_contribution_percent,
        addl_ee_contribution_percent_temp,
    }
}

