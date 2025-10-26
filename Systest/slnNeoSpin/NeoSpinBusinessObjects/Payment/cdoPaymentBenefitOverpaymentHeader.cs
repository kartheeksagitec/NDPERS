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
	/// Class NeoSpin.CustomDataObjects.cdoPaymentBenefitOverpaymentHeader:
	/// Inherited from doPaymentBenefitOverpaymentHeader, the class is used to customize the database object doPaymentBenefitOverpaymentHeader.
	/// </summary>
    [Serializable]
	public class cdoPaymentBenefitOverpaymentHeader : doPaymentBenefitOverpaymentHeader
	{
		public cdoPaymentBenefitOverpaymentHeader() : base()
		{
		}

        public int year_1099r { get; set; }
        public decimal gross_amount { get; set; }
        public decimal taxable_amount { get; set; }
        public decimal nontaxable_amount { get; set; }
        public decimal net_amount { get; set; }
        public decimal deduction_amount { get; set; }
        public string suppress_warnings_flag { get; set; }
    } 
} 
