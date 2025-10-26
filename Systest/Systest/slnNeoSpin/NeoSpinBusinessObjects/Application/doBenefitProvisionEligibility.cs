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
	/// Class NeoSpin.DataObjects.doBenefitProvisionEligibility:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionEligibility : doBase
    {
         public doBenefitProvisionEligibility() : base()
         {
         }
         public int benefit_provision_eligibility_id { get; set; }
         public int benefit_provision_id { get; set; }
         public DateTime effective_date { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int eligibility_type_id { get; set; }
         public string eligibility_type_description { get; set; }
         public string eligibility_type_value { get; set; }
         public int age { get; set; }
         public int service { get; set; }
         public int age_plus_service { get; set; }
         public string combined_service_flag { get; set; }
         public string consecutive_service_flag { get; set; }
         public string immediately_before_service_flag { get; set; }
         public string age_while_employed_flag { get; set; }
         public string grouping_logic { get; set; }
         public string other_logic_flag { get; set; }
         public int eligibility_group_code { get; set; }
         public int minimum_age { get; set; }
         public int benefit_tier_id { get; set; }
         public string benefit_tier_description { get; set; }
         public string benefit_tier_value { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionEligibility
    {
         benefit_provision_eligibility_id ,
         benefit_provision_id ,
         effective_date ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         eligibility_type_id ,
         eligibility_type_description ,
         eligibility_type_value ,
         age ,
         service ,
         age_plus_service ,
         combined_service_flag ,
         consecutive_service_flag ,
         immediately_before_service_flag ,
         age_while_employed_flag ,
         grouping_logic ,
         other_logic_flag ,
         eligibility_group_code ,
         minimum_age ,
         benefit_tier_id ,
         benefit_tier_description ,
         benefit_tier_value ,
    }
}

