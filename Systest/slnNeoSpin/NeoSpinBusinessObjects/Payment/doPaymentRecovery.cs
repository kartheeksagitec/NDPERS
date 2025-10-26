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
	/// Class NeoSpin.DataObjects.doPaymentRecovery:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentRecovery : doBase
    {
         
         public doPaymentRecovery() : base()
         {
         }
         public int payment_recovery_id { get; set; }
         public int payee_account_id { get; set; }
         public int benefit_overpayment_id { get; set; }
         public int repayment_type_id { get; set; }
         public string repayment_type_description { get; set; }
         public string repayment_type_value { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int life_time_reduction_ref_id { get; set; }
         public decimal recovery_amount { get; set; }
         public decimal gross_reduction_amount { get; set; }
         public DateTime effective_date { get; set; }
         public DateTime write_off_date { get; set; }
         public int calculation_id { get; set; }
         public DateTime approved_date { get; set; }
         public int payee_account_payment_item_type_id { get; set; }
    }
    [Serializable]
    public enum enmPaymentRecovery
    {
         payment_recovery_id ,
         payee_account_id ,
         benefit_overpayment_id ,
         repayment_type_id ,
         repayment_type_description ,
         repayment_type_value ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
         status_id ,
         status_description ,
         status_value ,
         life_time_reduction_ref_id ,
         recovery_amount ,
         gross_reduction_amount ,
         effective_date ,
         write_off_date ,
         calculation_id ,
         approved_date ,
         payee_account_payment_item_type_id ,
    }
}

