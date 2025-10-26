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
    /// Class NeoSpin.BusinessObjects.busPaymentRecoveryHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentRecoveryHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentRecoveryHistoryGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPaymentRecoveryHistoryGen
        /// </summary>
		public busPaymentRecoveryHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentRecoveryHistoryGen.
        /// </summary>
		public cdoPaymentRecoveryHistory icdoPaymentRecoveryHistory { get; set; }




        /// <summary>
        /// NeoSpin.busPaymentRecoveryHistoryGen.FindPaymentRecoveryHistory():
        /// Finds a particular record from cdoPaymentRecoveryHistory with its primary key. 
        /// </summary>
        /// <param name="aintrecoveryhistoryid">A primary key value of type int of cdoPaymentRecoveryHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentRecoveryHistory(int aintrecoveryhistoryid)
		{
			bool lblnResult = false;
			if (icdoPaymentRecoveryHistory == null)
			{
				icdoPaymentRecoveryHistory = new cdoPaymentRecoveryHistory();
			}
			if (icdoPaymentRecoveryHistory.SelectRow(new object[1] { aintrecoveryhistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
