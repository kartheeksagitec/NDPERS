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
	/// Class NeoSpin.DataObjects.doPayeeAccountPaymentItemType:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountPaymentItemType : doBase
    {
        
         public doPayeeAccountPaymentItemType() : base()
         {
         }
         public int payee_account_payment_item_type_id { get; set; }
         public int payee_account_id { get; set; }
         public int payment_item_type_id { get; set; }
         public string account_number { get; set; }
         public int vendor_org_id { get; set; }
         public decimal amount { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int batch_schedule_id { get; set; }
         public string reissue_item_flag { get; set; }
         public string miscellaneous_correction_flag { get; set; }
         public int person_account_id { get; set; }
         public int pre_rtw_payee_account_id { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountPaymentItemType
    {
         payee_account_payment_item_type_id ,
         payee_account_id ,
         payment_item_type_id ,
         account_number ,
         vendor_org_id ,
         amount ,
         start_date ,
         end_date ,
         batch_schedule_id ,
         reissue_item_flag ,
         miscellaneous_correction_flag ,
         person_account_id ,
         pre_rtw_payee_account_id ,
    }
}

