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
	public class cdoCaseFinancialHardshipProviderDetail : doCaseFinancialHardshipProviderDetail
	{
		public cdoCaseFinancialHardshipProviderDetail() : base()
		{
		}

        public decimal per_pay_period_contribution_amount { get; set; }

        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }
    } 
} 
