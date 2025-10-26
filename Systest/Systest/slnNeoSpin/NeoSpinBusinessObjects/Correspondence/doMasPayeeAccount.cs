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
	/// Class NeoSpin.DataObjects.doMasPayeeAccount:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasPayeeAccount : doBase
    {
         
         public doMasPayeeAccount() : base()
         {
         }
         public int mas_payee_account_id { get; set; }
         public int mas_person_id { get; set; }
         public int member_person_id { get; set; }
         public int payee_perslink_id { get; set; }
         public int plan_id { get; set; }
         public int payee_account_id { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int benefit_account_sub_type_id { get; set; }
         public string benefit_account_sub_type_description { get; set; }
         public string benefit_account_sub_type_value { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public int account_relation_id { get; set; }
         public string account_relation_description { get; set; }
         public string account_relation_value { get; set; }
         public int family_relation_id { get; set; }
         public string family_relation_description { get; set; }
         public string family_relation_value { get; set; }
         public decimal minimum_guarantee_amount { get; set; }
         public decimal nontaxable_beginning_balance { get; set; }
         public int rhic_benefit_option_id { get; set; }
         public string rhic_benefit_option_description { get; set; }
         public string rhic_benefit_option_value { get; set; }
         public DateTime ssli_change_date { get; set; }
         public decimal ssli_age { get; set; }
         public string uniform_income_flag { get; set; }
         public DateTime benefit_begin_date { get; set; }
         public DateTime benefit_end_date { get; set; }
         public int joint_annuitant_perslink_id { get; set; }
         public int retirement_org_id { get; set; }
         public decimal account_balance { get; set; }
         public decimal member_rhic_amount { get; set; }
         public decimal spouse_rhic_amount { get; set; }
         public DateTime term_certain_end_date { get; set; }
        public int person_account_id { get; set; }
        public string plan_benefit_tier_description { get; set; }
    }
    [Serializable]
    public enum enmMasPayeeAccount
    {
         mas_payee_account_id ,
         mas_person_id ,
         member_person_id ,
         payee_perslink_id ,
         plan_id ,
         payee_account_id ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         benefit_account_sub_type_id ,
         benefit_account_sub_type_description ,
         benefit_account_sub_type_value ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         account_relation_id ,
         account_relation_description ,
         account_relation_value ,
         family_relation_id ,
         family_relation_description ,
         family_relation_value ,
         minimum_guarantee_amount ,
         nontaxable_beginning_balance ,
         rhic_benefit_option_id ,
         rhic_benefit_option_description ,
         rhic_benefit_option_value ,
         ssli_change_date ,
         ssli_age ,
         uniform_income_flag ,
         benefit_begin_date ,
         benefit_end_date ,
         joint_annuitant_perslink_id ,
         retirement_org_id ,
         account_balance ,
         member_rhic_amount ,
         spouse_rhic_amount ,
         term_certain_end_date ,
        person_account_id,
        plan_benefit_tier_description,
    }
}

