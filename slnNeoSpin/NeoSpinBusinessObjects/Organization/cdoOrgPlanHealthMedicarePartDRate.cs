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
	public class cdoOrgPlanHealthMedicarePartDRate : doOrgPlanHealthMedicarePartDRate
	{
		public cdoOrgPlanHealthMedicarePartDRate() : base()
		{
		}

        public decimal premium_amount 
        {
            get
            {
                return provider_premium_amt + fee_amt;
            }
        }
        public decimal revised_premium_amount
        {
            get
            {
                return provider_premium_amt + fee_amt + health_savings_amount - buydown_amount + medicare_part_d_amt;//PIR 14271
            }
        }
    } 
} 
