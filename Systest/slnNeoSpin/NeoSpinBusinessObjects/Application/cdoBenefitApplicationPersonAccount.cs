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
	public class cdoBenefitApplicationPersonAccount : doBenefitApplicationPersonAccount
	{
		public cdoBenefitApplicationPersonAccount() : base()
		{
		}

        //this is used in 60 
        //to allow user to select multiple person account
        public string istrUse { get; set; }
    } 
} 
