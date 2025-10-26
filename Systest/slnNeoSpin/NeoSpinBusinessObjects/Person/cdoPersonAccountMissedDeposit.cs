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
	public class cdoPersonAccountMissedDeposit : doPersonAccountMissedDeposit
	{
		public cdoPersonAccountMissedDeposit() : base()
		{
		}
        public DateTime pay_period_year_and_month { get; set; }
    } 
} 
