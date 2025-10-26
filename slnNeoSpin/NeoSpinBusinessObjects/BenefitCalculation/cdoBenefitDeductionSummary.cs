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
	public class cdoBenefitDeductionSummary : doBenefitDeductionSummary
	{
		public cdoBenefitDeductionSummary() : base()
		{
		}

        #region Correspondence properties
        public decimal total_net_health_insurance_premium_amount
        {
            get
            {
                if (net_health_insurance_premium_amount > 0)
                    return net_health_insurance_premium_amount;
                else
                    return 0;
            }
        }
        
        #endregion
    } 
} 
