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
using NeoSpin.BusinessObjects;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin
{
    /// <summary>
    /// Class NeoSpin.busPersonAccountPaymentElectionHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountPaymentElectionHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountPaymentElectionHistoryGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.busPersonAccountPaymentElectionHistoryGen
        /// </summary>
		public busPersonAccountPaymentElectionHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountPaymentElectionHistoryGen.
        /// </summary>
		public cdoPersonAccountPaymentElectionHistory icdoPersonAccountPaymentElectionHistory { get; set; }




        /// <summary>
        /// NeoSpin.busPersonAccountPaymentElectionHistoryGen.FindPersonAccountPaymentElectionHistory():
        /// Finds a particular record from cdoPersonAccountPaymentElectionHistory with its primary key. 
        /// </summary>
        /// <param name="aintAccountPaymentElectionHistoryId">A primary key value of type int of cdoPersonAccountPaymentElectionHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountPaymentElectionHistory(int aintAccountPaymentElectionHistoryId)
		{
			bool lblnResult = false;
			if (icdoPersonAccountPaymentElectionHistory == null)
			{
				icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory();
			}
			if (icdoPersonAccountPaymentElectionHistory.SelectRow(new object[1] { aintAccountPaymentElectionHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
