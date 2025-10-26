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
	/// Class NeoSpin.DataObjects.doPostRetirementDeathBenefitOptionRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPostRetirementDeathBenefitOptionRef : doBase
    {
         
         public doPostRetirementDeathBenefitOptionRef() : base()
         {
         }
         public int post_retirement_death_benefit_option_ref_id { get; set; }
         public int benefit_provision_id { get; set; }
         public int source_benefit_account_type_id { get; set; }
         public string source_benefit_account_type_description { get; set; }
         public string source_benefit_account_type_value { get; set; }
         public int source_benefit_option_id { get; set; }
         public string source_benefit_option_description { get; set; }
         public string source_benefit_option_value { get; set; }
         public int account_relation_id { get; set; }
         public string account_relation_description { get; set; }
         public string account_relation_value { get; set; }
         public string is_spouse_at_death { get; set; }
         public int post_retirement_death_reason_type_id { get; set; }
         public string post_retirement_death_reason_type_description { get; set; }
         public string post_retirement_death_reason_type_value { get; set; }
         public int destination_benefit_option_id { get; set; }
         public string destination_benefit_option_description { get; set; }
         public string destination_benefit_option_value { get; set; }
         public string is_monthly_benefit_flag { get; set; }
         public string is_termdate_past_death_date_flag { get; set; }
    }
    [Serializable]
    public enum enmPostRetirementDeathBenefitOptionRef
    {
         post_retirement_death_benefit_option_ref_id ,
         benefit_provision_id ,
         source_benefit_account_type_id ,
         source_benefit_account_type_description ,
         source_benefit_account_type_value ,
         source_benefit_option_id ,
         source_benefit_option_description ,
         source_benefit_option_value ,
         account_relation_id ,
         account_relation_description ,
         account_relation_value ,
         is_spouse_at_death ,
         post_retirement_death_reason_type_id ,
         post_retirement_death_reason_type_description ,
         post_retirement_death_reason_type_value ,
         destination_benefit_option_id ,
         destination_benefit_option_description ,
         destination_benefit_option_value ,
         is_monthly_benefit_flag ,
         is_termdate_past_death_date_flag ,
    }
}

