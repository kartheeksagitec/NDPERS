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
	/// Class NeoSpin.DataObjects.doBenefitProvisionBenefitType:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionBenefitType : doBase
    {
         
         public doBenefitProvisionBenefitType() : base()
         {
         }
         public int benefit_provision_benefit_type_id { get; set; }
         public int benefit_provision_id { get; set; }
         public DateTime effective_date { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int benefit_formula_id { get; set; }
         public string benefit_formula_description { get; set; }
         public string benefit_formula_value { get; set; }
         public int early_reduction_method_id { get; set; }
         public string early_reduction_method_description { get; set; }
         public string early_reduction_method_value { get; set; }
         public decimal early_reduction_factor { get; set; }
         public int fas_formula_id { get; set; }
         public string fas_formula_description { get; set; }
         public string fas_formula_value { get; set; }
         public int fas_no_periods { get; set; }
         public int fas_no_periods_range { get; set; }
         public string plso_flag { get; set; }
         public int plso_factor_method_id { get; set; }
         public string plso_factor_method_description { get; set; }
         public string plso_factor_method_value { get; set; }
         public decimal plso_factor { get; set; }
         public string dnro_flag { get; set; }
         public int dnro_factor_method_id { get; set; }
         public string dnro_factor_method_description { get; set; }
         public string dnro_factor_method_value { get; set; }
         public decimal dnro_factor { get; set; }
         public decimal rhic_service_factor { get; set; }
         public int benefit_tier_id { get; set; }
         public string benefit_tier_description { get; set; }
         public string benefit_tier_value { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionBenefitType
    {
         benefit_provision_benefit_type_id ,
         benefit_provision_id ,
         effective_date ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         benefit_formula_id ,
         benefit_formula_description ,
         benefit_formula_value ,
         early_reduction_method_id ,
         early_reduction_method_description ,
         early_reduction_method_value ,
         early_reduction_factor ,
         fas_formula_id ,
         fas_formula_description ,
         fas_formula_value ,
         fas_no_periods ,
         fas_no_periods_range ,
         plso_flag ,
         plso_factor_method_id ,
         plso_factor_method_description ,
         plso_factor_method_value ,
         plso_factor ,
         dnro_flag ,
         dnro_factor_method_id ,
         dnro_factor_method_description ,
         dnro_factor_method_value ,
         dnro_factor ,
         rhic_service_factor ,
         benefit_tier_id ,
         benefit_tier_description ,
         benefit_tier_value ,
    }
}

