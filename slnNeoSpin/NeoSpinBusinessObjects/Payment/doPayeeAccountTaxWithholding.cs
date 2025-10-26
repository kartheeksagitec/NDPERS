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
	/// Class NeoSpin.DataObjects.doPayeeAccountTaxWithholding:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountTaxWithholding : doBase
    {
         public doPayeeAccountTaxWithholding() : base()
         {
         }
         public int payee_account_tax_withholding_id { get; set; }
         public int payee_account_id { get; set; }
         public int tax_identifier_id { get; set; }
         public string tax_identifier_description { get; set; }
         public string tax_identifier_value { get; set; }
         public int benefit_distribution_type_id { get; set; }
         public string benefit_distribution_type_description { get; set; }
         public string benefit_distribution_type_value { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int tax_option_id { get; set; }
         public string tax_option_description { get; set; }
         public string tax_option_value { get; set; }
         public int tax_allowance { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
         public decimal additional_tax_amount { get; set; }
         public string suppress_warnings_flag { get; set; }
         public decimal refund_fed_percent { get; set; }
         public decimal refund_state_amt { get; set; }
         public string filing_status_value { get; set; }
         public int filing_status_id { get; set; }
         public string filing_status_description { get; set; }
         public string tax_ref { get; set; }
         public decimal two_b_i { get; set; }
         public decimal two_b_ii { get; set; }
         public decimal two_b_iii { get; set; }
         public decimal three_1 { get; set; }
         public decimal three_2 { get; set; }
         public decimal three_3 { get; set; }
         public decimal three_total { get; set; }
         public decimal four_a { get; set; }
         public decimal four_b { get; set; }
         public decimal four_c { get; set; }
         public decimal state_flat_amount { get; set; }
         public string no_fed_withholding { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountTaxWithholding
    {
         payee_account_tax_withholding_id ,
         payee_account_id ,
         tax_identifier_id ,
         tax_identifier_description ,
         tax_identifier_value ,
         benefit_distribution_type_id ,
         benefit_distribution_type_description ,
         benefit_distribution_type_value ,
         start_date ,
         end_date ,
         tax_option_id ,
         tax_option_description ,
         tax_option_value ,
         tax_allowance ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
         additional_tax_amount ,
         suppress_warnings_flag ,
         refund_fed_percent ,
         refund_state_amt ,
         filing_status_value ,
         filing_status_id ,
         filing_status_description ,
         tax_ref ,
         two_b_i ,
         two_b_ii ,
         two_b_iii ,
         three_1 ,
         three_2 ,
         three_3 ,
         three_total ,
         four_a ,
         four_b ,
         four_c ,
         state_flat_amount ,
         no_fed_withholding ,
    }
}
