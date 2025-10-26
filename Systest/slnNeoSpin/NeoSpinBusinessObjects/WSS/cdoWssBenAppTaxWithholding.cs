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
	/// Class NeoSpin.CustomDataObjects.cdoWssBenAppTaxWithholding:
	/// Inherited from doWssBenAppTaxWithholding, the class is used to customize the database object doWssBenAppTaxWithholding.
	/// </summary>
    [Serializable]
	public class cdoWssBenAppTaxWithholding : doWssBenAppTaxWithholding
	{
		public cdoWssBenAppTaxWithholding() : base()
		{
		}
        public string no_tax_allowance { get; set; }
        public string three_first_line { get; set; }
        public string three_second_line { get; set; }
        public string three_third_line { get; set; }
        public string two_tip { get; set; }

        public string istrNoFedWithholdingDescription
        {
            get { return no_fed_withholding == NeoSpin.BusinessObjects.busConstant.Flag_Yes ? NeoSpin.BusinessObjects.busConstant.Flag_Yes_Value : NeoSpin.BusinessObjects.busConstant.Flag_No_Value; }
            set { }
        }

    } 
} 
