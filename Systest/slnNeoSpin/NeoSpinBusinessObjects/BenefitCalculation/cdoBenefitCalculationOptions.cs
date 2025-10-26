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
    [Serializable]
	public class cdoBenefitCalculationOptions : doBenefitCalculationOptions
	{
		public cdoBenefitCalculationOptions() : base()
		{
		}

        public string payee_name { get; set; }

        public decimal total_member_account_balance
        {
            get
            {
                return taxable_amount + non_taxable_amount;
            }
        }
    } 
} 
