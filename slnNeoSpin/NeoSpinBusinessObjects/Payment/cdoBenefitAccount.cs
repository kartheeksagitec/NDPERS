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
    public class cdoBenefitAccount : doBenefitAccount
    {
        public cdoBenefitAccount()
            : base()
        {
        }
        public decimal TotalAccountBalance
        {
            get
            {
                return starting_nontaxable_amount + starting_taxable_amount;
            }
        }
    }
}