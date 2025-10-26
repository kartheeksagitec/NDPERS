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
	/// Class NeoSpin.CustomDataObjects.cdoPaymentMonthlyBenefitSummary:
	/// Inherited from doPaymentMonthlyBenefitSummary, the class is used to customize the database object doPaymentMonthlyBenefitSummary.
	/// </summary>
    [Serializable]
	public class cdoPaymentMonthlyBenefitSummary : doPaymentMonthlyBenefitSummary
	{
		public cdoPaymentMonthlyBenefitSummary() : base()
		{
		}
    } 
} 
