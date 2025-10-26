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
	/// Class NeoSpin.DataObjects.doPlan:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPlan : doBase
    {
         
         public doPlan() : base()
         {
         }
         public int plan_id { get; set; }
         public string plan_name { get; set; }
         public int benefit_type_id { get; set; }
         public string benefit_type_description { get; set; }
         public string benefit_type_value { get; set; }
         public string plan_code { get; set; }
         public string allow_new_member_enroll_flag { get; set; }
         public string allow_deposit_flag { get; set; }
         public string allow_new_employer_flag { get; set; }
         public string allow_provider_flag { get; set; }
         public int bank_account_type_id { get; set; }
         public string bank_account_type_description { get; set; }
         public string bank_account_type_value { get; set; }
         public int retirement_type_id { get; set; }
         public string retirement_type_description { get; set; }
         public string retirement_type_value { get; set; }
         public int allow_interest_posting { get; set; }
         public int allow_pep_calculation { get; set; }
         public int vsc_threshold_months { get; set; }
         public int sort_order { get; set; }
         public int benefit_provision_id { get; set; }
         public string apply_rhic_flag { get; set; }
         public string include_in_retiree_annual_statement_flag { get; set; }
    }
    [Serializable]
    public enum enmPlan
    {
         plan_id ,
         plan_name ,
         benefit_type_id ,
         benefit_type_description ,
         benefit_type_value ,
         plan_code ,
         allow_new_member_enroll_flag ,
         allow_deposit_flag ,
         allow_new_employer_flag ,
         allow_provider_flag ,
         bank_account_type_id ,
         bank_account_type_description ,
         bank_account_type_value ,
         retirement_type_id ,
         retirement_type_description ,
         retirement_type_value ,
         allow_interest_posting ,
         allow_pep_calculation ,
         vsc_threshold_months ,
         sort_order ,
         benefit_provision_id ,
         apply_rhic_flag ,
         include_in_retiree_annual_statement_flag ,
    }
}

