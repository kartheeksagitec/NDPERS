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
	public class cdoEmployerPayrollBonusDetail : doEmployerPayrollBonusDetail
	{
		public cdoEmployerPayrollBonusDetail() : base()
		{
		}

        public decimal eligible_wages_rounded
        {
            get
            {
                return Math.Round(eligible_wages, 2, MidpointRounding.AwayFromZero);
            }
        }
    } 
} 
