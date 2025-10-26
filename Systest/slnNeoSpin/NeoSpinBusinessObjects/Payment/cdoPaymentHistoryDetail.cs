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
	public class cdoPaymentHistoryDetail : doPaymentHistoryDetail
	{
		public cdoPaymentHistoryDetail() : base()
		{
		}
        //prop used for grouping the items
        public DateTime payment_date { get; set; }
    } 
} 
