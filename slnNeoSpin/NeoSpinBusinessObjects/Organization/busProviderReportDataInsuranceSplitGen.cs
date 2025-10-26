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
    /// Class NeoSpin.BusinessObjects.busProviderReportDataInsuranceSplitGen:
    /// Inherited from busBase, used to create new business object for main table cdoProviderReportDataInsuranceSplit and its children table. 
    /// </summary>
	[Serializable]
	public class busProviderReportDataInsuranceSplitGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busProviderReportDataInsuranceSplitGen
        /// </summary>
		public busProviderReportDataInsuranceSplitGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busProviderReportDataInsuranceSplitGen.
        /// </summary>
		public cdoProviderReportDataInsuranceSplit icdoProviderReportDataInsuranceSplit { get; set; }




        /// <summary>
        /// NeoSpin.busProviderReportDataInsuranceSplitGen.FindProviderReportDataInsuranceSplit():
        /// Finds a particular record from cdoProviderReportDataInsuranceSplit with its primary key. 
        /// </summary>
        /// <param name="aintproviderreportdatainsurancesplitid">A primary key value of type int of cdoProviderReportDataInsuranceSplit on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindProviderReportDataInsuranceSplit(int aintproviderreportdatainsurancesplitid)
		{
			bool lblnResult = false;
			if (icdoProviderReportDataInsuranceSplit == null)
			{
				icdoProviderReportDataInsuranceSplit = new cdoProviderReportDataInsuranceSplit();
			}
			if (icdoProviderReportDataInsuranceSplit.SelectRow(new object[1] { aintproviderreportdatainsurancesplitid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
