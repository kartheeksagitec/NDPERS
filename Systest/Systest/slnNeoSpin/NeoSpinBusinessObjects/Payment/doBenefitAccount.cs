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
	/// Class NeoSpin.DataObjects.doBenefitAccount:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitAccount : doBase
    {
         
         public doBenefitAccount() : base()
         {
         }
         public int benefit_account_id { get; set; }
         public decimal starting_taxable_amount { get; set; }
         public decimal starting_nontaxable_amount { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int rhic_benefit_option_id { get; set; }
         public string rhic_benefit_option_description { get; set; }
         public string rhic_benefit_option_value { get; set; }
         public decimal rhic_benefit_amount { get; set; }
         public decimal pension_service_credit { get; set; }
         public decimal total_vested_service_credit { get; set; }
         public int retirement_org_id { get; set; }
         public DateTime ssli_change_date { get; set; }
         public decimal estimated_ss_benefit_amount { get; set; }
         public int rule_indicator_id { get; set; }
         public string rule_indicator_description { get; set; }
         public string rule_indicator_value { get; set; }
         public decimal option_factor { get; set; }
         public decimal rhic_option_factor { get; set; }
         public decimal spouse_rhic_amount { get; set; }
    }
    [Serializable]
    public enum enmBenefitAccount
    {
         benefit_account_id ,
         starting_taxable_amount ,
         starting_nontaxable_amount ,
         status_id ,
         status_description ,
         status_value ,
         rhic_benefit_option_id ,
         rhic_benefit_option_description ,
         rhic_benefit_option_value ,
         rhic_benefit_amount ,
         pension_service_credit ,
         total_vested_service_credit ,
         retirement_org_id ,
         ssli_change_date ,
         estimated_ss_benefit_amount ,
         rule_indicator_id ,
         rule_indicator_description ,
         rule_indicator_value ,
         option_factor ,
         rhic_option_factor ,
         spouse_rhic_amount ,
    }
}

