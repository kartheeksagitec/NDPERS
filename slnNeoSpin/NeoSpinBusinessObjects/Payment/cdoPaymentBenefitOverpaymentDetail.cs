#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoPaymentBenefitOverpaymentDetail:
	/// Inherited from doPaymentBenefitOverpaymentDetail, the class is used to customize the database object doPaymentBenefitOverpaymentDetail.
	/// </summary>
    [Serializable]
	public class cdoPaymentBenefitOverpaymentDetail : doPaymentBenefitOverpaymentDetail
	{
		public cdoPaymentBenefitOverpaymentDetail() : base()
		{
		}
    } 
} 
