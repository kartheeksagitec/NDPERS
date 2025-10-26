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
	/// Class NeoSpin.DataObjects.doPaymentHistoryHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentHistoryHeader : doBase
    {
         public doPaymentHistoryHeader() : base()
         {
         }
         public int payment_history_header_id { get; set; }
         public int plan_id { get; set; }
         public int payee_account_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public DateTime payment_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int payment_schedule_id { get; set; }
         public int fed_tax_option_id { get; set; }
         public string fed_tax_option_description { get; set; }
         public string fed_tax_option_value { get; set; }
         public int fed_tax_allowance { get; set; }
         public int fed_marital_status_id { get; set; }
         public string fed_marital_status_description { get; set; }
         public string fed_marital_status_value { get; set; }
         public decimal fed_additional_tax_amount { get; set; }
         public int state_tax_option_id { get; set; }
         public string state_tax_option_description { get; set; }
         public string state_tax_option_value { get; set; }
         public int state_tax_allowance { get; set; }
         public int state_marital_status_id { get; set; }
         public string state_marital_status_description { get; set; }
         public string state_marital_status_value { get; set; }
         public decimal state_additional_tax_amount { get; set; }
         public int old_payment_history_header_id { get; set; }
         public int rollover_type_id { get; set; }
         public string rollover_type_description { get; set; }
         public string rollover_type_value { get; set; }
         public decimal state_tax_flat_amount { get; set; }
         public int fed_filing_status_id { get; set; }
         public string fed_filing_status_description { get; set; }
         public string fed_filing_status_value { get; set; }
         public int employer_payroll_header_id { get; set; }
    }
    [Serializable]
    public enum enmPaymentHistoryHeader
    {
         payment_history_header_id ,
         plan_id ,
         payee_account_id ,
         person_id ,
         org_id ,
         payment_date ,
         status_id ,
         status_description ,
         status_value ,
         payment_schedule_id ,
         fed_tax_option_id ,
         fed_tax_option_description ,
         fed_tax_option_value ,
         fed_tax_allowance ,
         fed_marital_status_id ,
         fed_marital_status_description ,
         fed_marital_status_value ,
         fed_additional_tax_amount ,
         state_tax_option_id ,
         state_tax_option_description ,
         state_tax_option_value ,
         state_tax_allowance ,
         state_marital_status_id ,
         state_marital_status_description ,
         state_marital_status_value ,
         state_additional_tax_amount ,
         old_payment_history_header_id ,
         rollover_type_id ,
         rollover_type_description ,
         rollover_type_value ,
         state_tax_flat_amount ,
         fed_filing_status_id ,
         fed_filing_status_description ,
         fed_filing_status_value ,
         employer_payroll_header_id,
    }
}
