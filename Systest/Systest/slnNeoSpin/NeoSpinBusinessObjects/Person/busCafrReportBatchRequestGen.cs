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
    /// Class NeoSpin.BusinessObjects.busCafrReportBatchRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoCafrReportBatchRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busCafrReportBatchRequestGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busCafrReportBatchRequestGen
        /// </summary>
		public busCafrReportBatchRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busCafrReportBatchRequestGen.
        /// </summary>
		public cdoCafrReportBatchRequest icdoCafrReportBatchRequest { get; set; }




        /// <summary>
        /// NeoSpin.busCafrReportBatchRequestGen.FindCafrReportBatchRequest():
        /// Finds a particular record from cdoCafrReportBatchRequest with its primary key. 
        /// </summary>
        /// <param name="aintcafrreportbatchrequestid">A primary key value of type int of cdoCafrReportBatchRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindCafrReportBatchRequest(int aintcafrreportbatchrequestid)
		{
			bool lblnResult = false;
			if (icdoCafrReportBatchRequest == null)
			{
				icdoCafrReportBatchRequest = new cdoCafrReportBatchRequest();
			}
			if (icdoCafrReportBatchRequest.SelectRow(new object[1] { aintcafrreportbatchrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
