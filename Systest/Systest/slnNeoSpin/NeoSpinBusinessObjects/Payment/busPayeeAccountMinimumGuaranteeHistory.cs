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
	/// Class NeoSpin.BusinessObjects.busPayeeAccountMinimumGuaranteeHistory:
	/// Inherited from busPayeeAccountMinimumGuaranteeHistoryGen, the class is used to customize the business object busPayeeAccountMinimumGuaranteeHistoryGen.
	/// </summary>
	[Serializable]
	public class busPayeeAccountMinimumGuaranteeHistory : busPayeeAccountMinimumGuaranteeHistoryGen
	{
        /// <summary>
        /// Method to a new Minimum Guarantee History
        /// </summary>
        /// <param name="aintPayeeAccountID">Payee Account ID</param>
        /// <param name="adecMinimumGuaranteeAmt">Minimum Guarantee Amount</param>
        /// <param name="adtChangeDate">Change Date</param>
        /// <param name="astrProcessStatusFlag">Process Status Flag</param>
        public void CreateMinimumGuaranteeHistory(int aintPayeeAccountID, decimal adecMinimumGuaranteeAmt, DateTime adtChangeDate, string astrProcessStatusFlag)
        {
            icdoPayeeAccountMinimumGuaranteeHistory = new cdoPayeeAccountMinimumGuaranteeHistory();
            icdoPayeeAccountMinimumGuaranteeHistory.payee_account_id = aintPayeeAccountID;
            icdoPayeeAccountMinimumGuaranteeHistory.old_minumum_guarantee_amount = adecMinimumGuaranteeAmt;
            icdoPayeeAccountMinimumGuaranteeHistory.change_date = adtChangeDate;
            icdoPayeeAccountMinimumGuaranteeHistory.process_status_flag = astrProcessStatusFlag;
            icdoPayeeAccountMinimumGuaranteeHistory.Insert();
        }
	}
}
