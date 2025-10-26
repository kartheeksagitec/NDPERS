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
	public class cdoBenefitCalculationPersonAccount : doBenefitCalculationPersonAccount
	{
		public cdoBenefitCalculationPersonAccount() : base()
		{
		}

        //this is used in 60 
        //to set whether the payee account is used or not.
        public string istrUse { get; set; }
    } 
} 
