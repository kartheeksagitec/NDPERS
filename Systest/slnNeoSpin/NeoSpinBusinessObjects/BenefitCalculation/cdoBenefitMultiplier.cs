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
	public class cdoBenefitMultiplier : doBenefitMultiplier
	{
		public cdoBenefitMultiplier() : base()
		{
		}

        public decimal benefit_multiplier_rate_percentage 
        { 
            get
            {
                return benefit_multiplier_rate * 100;
            }
        }

        /// <summary>
        /// Used to display FAS in MSS
        /// </summary>
        public decimal final_average_salary { get; set; }
    } 
} 
