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
	/// Class NeoSpin.DataObjects.doBenefitCalculationOptions:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitCalculationOptions : doBase
    {
         public doBenefitCalculationOptions() : base()
         {
         }
         public int benefit_calculation_options_id { get; set; }
         public int benefit_calculation_id { get; set; }
         public int benefit_provision_benefit_option_id { get; set; }
         public int benefit_calculation_payee_id { get; set; }
         public decimal benefit_option_increase_or_decrease { get; set; }
         public decimal ssli_factor { get; set; }
         public decimal before_ssli_amount { get; set; }
         public decimal after_ssli_amount { get; set; }
         public decimal benefit_option_amount { get; set; }
         public decimal benefit_with_plso { get; set; }
         public decimal option_factor { get; set; }
         public decimal taxable_amount { get; set; }
         public decimal non_taxable_amount { get; set; }
         public decimal pre_tax_ee_amount { get; set; }
         public decimal post_tax_ee_amount { get; set; }
         public decimal ee_er_pickup_amount { get; set; }
         public decimal interest_amount { get; set; }
         public decimal er_vested_amount { get; set; }
         public decimal ee_rhic_amount { get; set; }
         public decimal capital_gain { get; set; }
         public decimal post_tax_ee_ser_pur_cont { get; set; }
         public decimal ee_rhic_ser_pur_cont { get; set; }
         public decimal pre_tax_ee_ser_pur_cont { get; set; }
         public int payee_sort_order { get; set; }
         public int graduated_benefit_option_id { get; set; }
         public string graduated_benefit_option_description { get; set; }
         public string graduated_benefit_option_value { get; set; }
         public decimal graduated_benefit_factor { get; set; }
         public decimal graduated_benefit_option_amount { get; set; }
    }
    [Serializable]
    public enum enmBenefitCalculationOptions
    {
         benefit_calculation_options_id ,
         benefit_calculation_id ,
         benefit_provision_benefit_option_id ,
         benefit_calculation_payee_id ,
         benefit_option_increase_or_decrease ,
         ssli_factor ,
         before_ssli_amount ,
         after_ssli_amount ,
         benefit_option_amount ,
         benefit_with_plso ,
         option_factor ,
         taxable_amount ,
         non_taxable_amount ,
         pre_tax_ee_amount ,
         post_tax_ee_amount ,
         ee_er_pickup_amount ,
         interest_amount ,
         er_vested_amount ,
         ee_rhic_amount ,
         capital_gain ,
         post_tax_ee_ser_pur_cont ,
         ee_rhic_ser_pur_cont ,
         pre_tax_ee_ser_pur_cont ,
         payee_sort_order ,
         graduated_benefit_option_id ,
         graduated_benefit_option_description ,
         graduated_benefit_option_value ,
         graduated_benefit_factor ,
         graduated_benefit_option_amount ,
    }
}

