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
	/// Class NeoSpin.DataObjects.doPayeeAccountMonthwiseAdjustmentDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountMonthwiseAdjustmentDetail : doBase
    {
         
         public doPayeeAccountMonthwiseAdjustmentDetail() : base()
         {
         }
         public int retro_payment_monthwise_detail_id { get; set; }
         public int payee_account_retro_payment_id { get; set; }
         public int benefit_overpayment_id { get; set; }
         public DateTime effective_date { get; set; }
         public decimal amount { get; set; }
         public decimal interest_amount { get; set; }
         public int payee_account_id { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountMonthwiseAdjustmentDetail
    {
         retro_payment_monthwise_detail_id ,
         payee_account_retro_payment_id ,
         benefit_overpayment_id ,
         effective_date ,
         amount ,
         interest_amount ,
         payee_account_id ,
    }
}

