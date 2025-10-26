#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busCAFRReportBatchRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busCAFRReportBatchRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busCafrReportBatchRequest. 
		/// </summary>
		public Collection<busCafrReportBatchRequest> iclbCafrReportBatchRequest { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busCAFRReportBatchRequestLookupGen.LoadCafrReportBatchRequests(DataTable):
		/// Loads Collection object iclbCafrReportBatchRequest of type busCafrReportBatchRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busCAFRReportBatchRequestLookupGen.iclbCafrReportBatchRequest</param>
		public virtual void LoadCafrReportBatchRequests(DataTable adtbSearchResult)
		{
			iclbCafrReportBatchRequest = GetCollection<busCafrReportBatchRequest>(adtbSearchResult, "icdoCafrReportBatchRequest");
		}
	}
}
