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
    /// Class NeoSpin.BusinessObjects.busPersonAccountDeferredCompTransferContributionGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountDeferredCompTransferContribution and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountDeferredCompTransferContributionGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPersonAccountDeferredCompTransferContributionGen
        /// </summary>
		public busPersonAccountDeferredCompTransferContributionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountDeferredCompTransferContributionGen.
        /// </summary>
		public cdoPersonAccountDeferredCompTransferContribution icdoPersonAccountDeferredCompTransferContribution { get; set; }




        /// <summary>
        /// NeoSpin.BusinessObjects.busPersonAccountDeferredCompTransferContributionGen.FindPersonAccountDeferredCompTransferContribution():
        /// Finds a particular record from cdoPersonAccountDeferredCompTransferContribution with its primary key. 
        /// </summary>
        /// <param name="aintpersonaccountdeferredcomptransfercontributionid">A primary key value of type int of cdoPersonAccountDeferredCompTransferContribution on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountDeferredCompTransferContribution(int aintpersonaccountdeferredcomptransfercontributionid)
		{
			bool lblnResult = false;
			if (icdoPersonAccountDeferredCompTransferContribution.IsNull())
			{
				icdoPersonAccountDeferredCompTransferContribution = new cdoPersonAccountDeferredCompTransferContribution();
			}
			if (icdoPersonAccountDeferredCompTransferContribution.SelectRow(new object[1] { aintpersonaccountdeferredcomptransfercontributionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
