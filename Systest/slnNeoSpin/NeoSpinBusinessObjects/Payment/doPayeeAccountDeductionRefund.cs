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
	/// Class NeoSpin.DataObjects.doPayeeAccountDeductionRefund:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountDeductionRefund : doBase
    {
         
         public doPayeeAccountDeductionRefund() : base()
         {
         }
         public int payee_account_deduction_refund_id { get; set; }
         public int payee_account_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
         public int payment_item_type_id { get; set; }
         public int payee_account_payment_item_type_id { get; set; }
         public decimal amount { get; set; }
         public int vendor_org_id { get; set; }
         public string account_number { get; set; }
         public int payment_history_header_id { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountDeductionRefund
    {
         payee_account_deduction_refund_id ,
         payee_account_id ,
         start_date ,
         end_date ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
         payment_item_type_id ,
         payee_account_payment_item_type_id ,
         amount ,
         vendor_org_id ,
         account_number ,
         payment_history_header_id ,
    }
}

