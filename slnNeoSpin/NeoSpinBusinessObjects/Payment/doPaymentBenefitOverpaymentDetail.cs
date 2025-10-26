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
	/// Class NeoSpin.DataObjects.doPaymentBenefitOverpaymentDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentBenefitOverpaymentDetail : doBase
    {
         
         public doPaymentBenefitOverpaymentDetail() : base()
         {
         }
         public int benefit_overpayment_detail_id { get; set; }
         public int benefit_overpayment_id { get; set; }
         public int payment_item_type_id { get; set; }
         public DateTime date_of_1099r { get; set; }
         public decimal amount { get; set; }
         public int vendor_org_id { get; set; }
    }
    [Serializable]
    public enum enmPaymentBenefitOverpaymentDetail
    {
         benefit_overpayment_detail_id ,
         benefit_overpayment_id ,
         payment_item_type_id ,
         date_of_1099r ,
         amount ,
         vendor_org_id ,
    }
}

