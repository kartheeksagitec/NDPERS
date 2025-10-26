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
	public class cdoBenefitProvisionMultiplier : doBenefitProvisionMultiplier
	{
		public cdoBenefitProvisionMultiplier() : base()
		{
		}
        
        public int service_period
        {
            get 
            {                 
                if(is_flat_percentage_flag==BusinessObjects.busConstant.Flag_No)
                    return service_to-service_from;
                return 0;
            }
        }

        //2 Digit Decimal Derived Property
        public decimal multipier_percentage_formatted
        {
            get
            {
                return Math.Round(multipier_percentage, 2, MidpointRounding.AwayFromZero);
            }
        }

    } 
} 
