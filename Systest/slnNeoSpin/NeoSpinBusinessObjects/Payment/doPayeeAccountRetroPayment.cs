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
	/// Class NeoSpin.DataObjects.doPayeeAccountRetroPayment:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountRetroPayment : doBase
    {
         
         public doPayeeAccountRetroPayment() : base()
         {
         }
         public int payee_account_retro_payment_id { get; set; }
         public int payee_account_id { get; set; }
         public int retro_payment_type_id { get; set; }
         public string retro_payment_type_description { get; set; }
         public string retro_payment_type_value { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
         public decimal gross_payment_amount { get; set; }
         public decimal net_payment_amount { get; set; }
         public string approved_flag { get; set; }
         public string calculate_interest_flag { get; set; }
         public int payment_history_header_id { get; set; }
         public int adjustment_reason_id { get; set; }
         public string adjustment_reason_description { get; set; }
         public string adjustment_reason_value { get; set; }
         public string is_online_flag { get; set; }
         public int fed_tax_option_id { get; set; }
         public string fed_tax_option_description { get; set; }
         public string fed_tax_option_value { get; set; }
         public int fed_tax_allowance { get; set; }
         public int fed_marital_status_id { get; set; }
         public string fed_marital_status_description { get; set; }
         public string fed_marital_status_value { get; set; }
         public decimal fed_additional_tax_amount { get; set; }
         public int stat_tax_option_id { get; set; }
         public string stat_tax_option_description { get; set; }
         public string stat_tax_option_value { get; set; }
         public int stat_tax_allowance { get; set; }
         public int stat_marital_status_id { get; set; }
         public string stat_marital_status_description { get; set; }
         public string stat_marital_status_value { get; set; }
         public decimal stat_additional_tax_amount { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountRetroPayment
    {
         payee_account_retro_payment_id ,
         payee_account_id ,
         retro_payment_type_id ,
         retro_payment_type_description ,
         retro_payment_type_value ,
         effective_start_date ,
         effective_end_date ,
         start_date ,
         end_date ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
         gross_payment_amount ,
         net_payment_amount ,
         approved_flag ,
         calculate_interest_flag ,
         payment_history_header_id ,
         adjustment_reason_id ,
         adjustment_reason_description ,
         adjustment_reason_value ,
         is_online_flag ,
         fed_tax_option_id ,
         fed_tax_option_description ,
         fed_tax_option_value ,
         fed_tax_allowance ,
         fed_marital_status_id ,
         fed_marital_status_description ,
         fed_marital_status_value ,
         fed_additional_tax_amount ,
         stat_tax_option_id ,
         stat_tax_option_description ,
         stat_tax_option_value ,
         stat_tax_allowance ,
         stat_marital_status_id ,
         stat_marital_status_description ,
         stat_marital_status_value ,
         stat_additional_tax_amount ,
    }
}

