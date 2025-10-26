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
	/// Class NeoSpin.BusinessObjects.busDuesRateChangeRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busDuesRateChangeRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busDuesRateChangeRequest. 
		/// </summary>
		public Collection<busDuesRateChangeRequest> iclbDuesRateChangeRequest { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busDuesRateChangeRequestLookupGen.LoadDuesRateChangeRequests(DataTable):
		/// Loads Collection object iclbDuesRateChangeRequest of type busDuesRateChangeRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busDuesRateChangeRequestLookupGen.iclbDuesRateChangeRequest</param>
		public virtual void LoadDuesRateChangeRequests(DataTable adtbSearchResult)
		{
			iclbDuesRateChangeRequest = GetCollection<busDuesRateChangeRequest>(adtbSearchResult, "icdoDuesRateChangeRequest");
		}
	}
}
