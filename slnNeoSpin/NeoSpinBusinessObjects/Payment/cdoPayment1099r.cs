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
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoPayment1099r:
	/// Inherited from doPayment1099r, the class is used to customize the database object doPayment1099r.
	/// </summary>
    [Serializable]
	public class cdoPayment1099r : doPayment1099r
	{
		public cdoPayment1099r() : base()
		{
		}
        //used in reports
        private decimal _net_amount;
        public decimal net_amount
        {
            get
            {
                _net_amount = taxable_amount + non_taxable_amount + capital_gain -
                fed_tax_amount - state_tax_amount;
                return _net_amount;
            }
            set
            {
                _net_amount = value;
            }
        }
        public decimal total_employee_contrib_amt { get; set; }

    } 
} 

