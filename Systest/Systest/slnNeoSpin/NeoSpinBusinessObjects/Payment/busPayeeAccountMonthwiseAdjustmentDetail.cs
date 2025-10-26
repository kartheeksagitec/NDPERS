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

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busPayeeAccountMonthwiseAdjustmentDetail:
	/// Inherited from busPayeeAccountMonthwiseAdjustmentDetailGen, the class is used to customize the business object busPayeeAccountMonthwiseAdjustmentDetailGen.
	/// </summary>
	[Serializable]
	public class busPayeeAccountMonthwiseAdjustmentDetail : busPayeeAccountMonthwiseAdjustmentDetailGen
	{
        /// <summary>
        /// Method to calculate Interest
        /// </summary>
        /// <param name="adtNextBenefitPaymentDate">Next benefit payment date</param>
        public void CalculateInterest(DateTime adtNextBenefitPaymentDate)
        {
            decimal ldecPercentage = 0.0M;
            int lintNoofMonths = 0;

            //Interest Percentage
            ldecPercentage = Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.RetroPaymentInterestPercentage, iobjPassInfo));

            //Getting number of months for which interest need to be calculated
            lintNoofMonths = busEmployerReportHelper.GetTotalDueMonths(adtNextBenefitPaymentDate, icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date) - 1;
            if (lintNoofMonths > 0)
            {
                icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount =
                    icdoPayeeAccountMonthwiseAdjustmentDetail.amount *
                    Convert.ToDecimal(Math.Pow(Convert.ToDouble((1200 + ldecPercentage) / Convert.ToDecimal(1200)), Convert.ToDouble(lintNoofMonths)))
                    - icdoPayeeAccountMonthwiseAdjustmentDetail.amount;
            }
        }
	}
}
