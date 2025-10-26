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
	/// Class NeoSpin.DataObjects.doPaymentBenefitOverpaymentHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentBenefitOverpaymentHeader : doBase
    {
         
         public doPaymentBenefitOverpaymentHeader() : base()
         {
         }
         public int benefit_overpayment_id { get; set; }
         public int payee_account_id { get; set; }
         public int adjustment_reason_id { get; set; }
         public string adjustment_reason_description { get; set; }
         public string adjustment_reason_value { get; set; }
         public string calculate_interest_flag { get; set; }
         public string amortized_interest_flag { get; set; }
         public string is_online_flag { get; set; }
    }
    [Serializable]
    public enum enmPaymentBenefitOverpaymentHeader
    {
         benefit_overpayment_id ,
         payee_account_id ,
         adjustment_reason_id ,
         adjustment_reason_description ,
         adjustment_reason_value ,
         calculate_interest_flag ,
         amortized_interest_flag ,
         is_online_flag ,
    }
}

