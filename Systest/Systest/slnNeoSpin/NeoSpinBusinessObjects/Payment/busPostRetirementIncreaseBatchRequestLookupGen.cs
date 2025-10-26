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
	/// Class NeoSpin.BusinessObjects.busPostRetirementIncreaseBatchRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busPostRetirementIncreaseBatchRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busPostRetirementIncreaseBatchRequest. 
		/// </summary>
		public Collection<busPostRetirementIncreaseBatchRequest> iclbPostRetirementIncreaseBatchRequest { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busPostRetirementIncreaseBatchRequestLookupGen.LoadPostRetirementIncreaseBatchRequests(DataTable):
		/// Loads Collection object iclbPostRetirementIncreaseBatchRequest of type busPostRetirementIncreaseBatchRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPostRetirementIncreaseBatchRequestLookupGen.iclbPostRetirementIncreaseBatchRequest</param>
		public virtual void LoadPostRetirementIncreaseBatchRequests(DataTable adtbSearchResult)
		{
			iclbPostRetirementIncreaseBatchRequest = GetCollection<busPostRetirementIncreaseBatchRequest>(adtbSearchResult, "icdoPostRetirementIncreaseBatchRequest");
		}
	}
}
