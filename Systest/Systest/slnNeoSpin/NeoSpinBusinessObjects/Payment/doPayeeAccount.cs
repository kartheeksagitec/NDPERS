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
	/// Class NeoSpin.DataObjects.doPayeeAccount:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccount : doBase
    {
         
         public doPayeeAccount() : base()
         {
         }
         public int payee_account_id { get; set; }
         public int payee_perslink_id { get; set; }
         public int payee_org_id { get; set; }
         public int application_id { get; set; }
         public int calculation_id { get; set; }
         public int benefit_account_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int benefit_account_sub_type_id { get; set; }
         public string benefit_account_sub_type_description { get; set; }
         public string benefit_account_sub_type_value { get; set; }
         public string pull_check_flag { get; set; }
         public DateTime benefit_begin_date { get; set; }
         public DateTime benefit_end_date { get; set; }
         public int account_relation_id { get; set; }
         public string account_relation_description { get; set; }
         public string account_relation_value { get; set; }
         public int family_relation_id { get; set; }
         public string family_relation_description { get; set; }
         public string family_relation_value { get; set; }
         public decimal minimum_guarantee_amount { get; set; }
         public decimal nontaxable_beginning_balance { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public decimal rhic_amount { get; set; }
         public int dro_calculation_id { get; set; }
         public int exclusion_method_id { get; set; }
         public string exclusion_method_description { get; set; }
         public string exclusion_method_value { get; set; }
         public string workflow_age_conversion_flag { get; set; }
         public string workflow_rule_conversion_flag { get; set; }
         public DateTime recertification_date { get; set; }
         public string is_medical_batch_letter_sent_flag { get; set; }
         public string is_disability_batch_letter_sent_flag { get; set; }
         public DateTime case_recertification_date { get; set; }
         public string is_recertification_date_set_flag { get; set; }
         public string is_pre_1991_disability_flag { get; set; }
         public DateTime term_certain_end_date { get; set; }
         public string rhic_ee_amount_refund_flag { get; set; }
         public int dro_application_id { get; set; }
         public string account_owner_batch_initiated_flag { get; set; }
         public string alternate_payee_batch_initiated_flag { get; set; }
         public string first_beneficiary_batch_initiated_flag { get; set; }
         public string legacy_account_no { get; set; }
         public int graduated_benefit_option_id { get; set; }
         public string graduated_benefit_option_description { get; set; }
         public string graduated_benefit_option_value { get; set; }
         public DateTime disa_normal_effective_date { get; set; }
         public string include_in_adhoc_flag { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccount
    {
         payee_account_id ,
         payee_perslink_id ,
         payee_org_id ,
         application_id ,
         calculation_id ,
         benefit_account_id ,
         status_id ,
         status_description ,
         status_value ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         benefit_account_sub_type_id ,
         benefit_account_sub_type_description ,
         benefit_account_sub_type_value ,
         pull_check_flag ,
         benefit_begin_date ,
         benefit_end_date ,
         account_relation_id ,
         account_relation_description ,
         account_relation_value ,
         family_relation_id ,
         family_relation_description ,
         family_relation_value ,
         minimum_guarantee_amount ,
         nontaxable_beginning_balance ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         rhic_amount ,
         dro_calculation_id ,
         exclusion_method_id ,
         exclusion_method_description ,
         exclusion_method_value ,
         workflow_age_conversion_flag ,
         workflow_rule_conversion_flag ,
         recertification_date ,
         is_medical_batch_letter_sent_flag ,
         is_disability_batch_letter_sent_flag ,
         case_recertification_date ,
         is_recertification_date_set_flag ,
         is_pre_1991_disability_flag ,
         term_certain_end_date ,
         rhic_ee_amount_refund_flag ,
         dro_application_id ,
         account_owner_batch_initiated_flag ,
         alternate_payee_batch_initiated_flag ,
         first_beneficiary_batch_initiated_flag ,
         legacy_account_no ,
         graduated_benefit_option_id ,
         graduated_benefit_option_description ,
         graduated_benefit_option_value ,
         disa_normal_effective_date ,
         include_in_adhoc_flag ,
    }
}

