#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPaymentMonthlyBenefitSummaryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentMonthlyBenefitSummary and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentMonthlyBenefitSummaryGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPaymentMonthlyBenefitSummaryGen
        /// </summary>
		public busPaymentMonthlyBenefitSummaryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentMonthlyBenefitSummaryGen.
        /// </summary>
		public cdoPaymentMonthlyBenefitSummary icdoPaymentMonthlyBenefitSummary { get; set; }




        /// <summary>
        /// NeoSpin.BusinessObjects.busPaymentMonthlyBenefitSummaryGen.FindPaymentMonthlyBenefitSummary():
        /// Finds a particular record from cdoPaymentMonthlyBenefitSummary with its primary key. 
        /// </summary>
        /// <param name="aintpaymentmonthlybenefitsummaryid">A primary key value of type int of cdoPaymentMonthlyBenefitSummary on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentMonthlyBenefitSummary(int aintpaymentmonthlybenefitsummaryid)
		{
			bool lblnResult = false;
			if (icdoPaymentMonthlyBenefitSummary.IsNull())
			{
				icdoPaymentMonthlyBenefitSummary = new cdoPaymentMonthlyBenefitSummary();
			}
			if (icdoPaymentMonthlyBenefitSummary.SelectRow(new object[1] { aintpaymentmonthlybenefitsummaryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
