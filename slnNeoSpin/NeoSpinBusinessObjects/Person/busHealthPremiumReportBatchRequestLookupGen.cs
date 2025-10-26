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
	/// Class NeoSpin.BusinessObjects.busHealthPremiumReportBatchRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busHealthPremiumReportBatchRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busHealthPremiumReportBatchRequest. 
		/// </summary>
		public Collection<busHealthPremiumReportBatchRequest> iclbHealthPremiumReportBatchRequest { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busHealthPremiumReportBatchRequestLookupGen.LoadHealthPremiumReportBatchRequests(DataTable):
		/// Loads Collection object iclbHealthPremiumReportBatchRequest of type busHealthPremiumReportBatchRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busHealthPremiumReportBatchRequestLookupGen.iclbHealthPremiumReportBatchRequest</param>
		public virtual void LoadHealthPremiumReportBatchRequests(DataTable adtbSearchResult)
		{
            iclbHealthPremiumReportBatchRequest = GetCollection<busHealthPremiumReportBatchRequest>(adtbSearchResult, "icdoHealthPremiumReportBatchRequest");
		}
	}
}
