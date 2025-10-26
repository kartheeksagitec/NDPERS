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
    /// Class NeoSpin.BusinessObjects.busPersonAccountInsuranceTransferContributionGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountInsuranceTransferContribution and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountInsuranceTransferContributionGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPersonAccountInsuranceTransferContributionGen
        /// </summary>
		public busPersonAccountInsuranceTransferContributionGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountInsuranceTransferContributionGen.
        /// </summary>
		public cdoPersonAccountInsuranceTransferContribution icdoPersonAccountInsuranceTransferContribution { get; set; }




        /// <summary>
        /// NeoSpin.BusinessObjects.busPersonAccountInsuranceTransferContributionGen.FindPersonAccountInsuranceTransferContribution():
        /// Finds a particular record from cdoPersonAccountInsuranceTransferContribution with its primary key. 
        /// </summary>
        /// <param name="aintpersonaccountinsurancetransfercontributionid">A primary key value of type int of cdoPersonAccountInsuranceTransferContribution on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountInsuranceTransferContribution(int aintpersonaccountinsurancetransfercontributionid)
		{
			bool lblnResult = false;
			if (icdoPersonAccountInsuranceTransferContribution.IsNull())
			{
				icdoPersonAccountInsuranceTransferContribution = new cdoPersonAccountInsuranceTransferContribution();
			}
			if (icdoPersonAccountInsuranceTransferContribution.SelectRow(new object[1] { aintpersonaccountinsurancetransfercontributionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
