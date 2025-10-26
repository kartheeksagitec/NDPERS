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
	/// Class NeoSpin.BusinessObjects.busMASBatchRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busMASBatchRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busMASBatchRequest. 
		/// </summary>
		public Collection<busMASBatchRequest> iclbMASBatchRequest { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busMASBatchRequestLookupGen.LoadMASBatchRequests(DataTable):
		/// Loads Collection object iclbMASBatchRequest of type busMASBatchRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busMASBatchRequestLookupGen.iclbMASBatchRequest</param>
		public virtual void LoadMASBatchRequests(DataTable adtbSearchResult)
		{
            iclbMASBatchRequest = GetCollection<busMASBatchRequest>(adtbSearchResult, "icdoMasBatchRequest");
		}
	}
}
