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
	public class cdoBenefitDroCalculation : doBenefitDroCalculation
	{
		public cdoBenefitDroCalculation() : base()
		{
		}

        private decimal _non_taxable_amount;

        public decimal non_taxable_amount
        {
            get { return _non_taxable_amount; }
            set { _non_taxable_amount = value; }
        }
        private decimal _monthly_taxable_amount;

        public decimal monthly_taxable_amount
        {
            get { return _monthly_taxable_amount; }
            set { _monthly_taxable_amount = value; }
        }
        private decimal _monthly_nontaxable_amount;

        public decimal monthly_nontaxable_amount
        {
            get { return _monthly_nontaxable_amount; }
            set { _monthly_nontaxable_amount = value; }
        }
        //This property is to display the additional interest in the screen 
        //There is already a field called additional_interest_amount which will be stored in the table when the status is approved
        public decimal additional_interest { get; set; }
    } 
} 
