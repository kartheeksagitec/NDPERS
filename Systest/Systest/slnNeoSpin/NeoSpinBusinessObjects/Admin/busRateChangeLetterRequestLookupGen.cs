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
	/// Class NeoSpin.BusinessObjects.busRateChangeLetterRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busRateChangeLetterRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busRateChangeLetterRequest. 
		/// </summary>
		public Collection<busRateChangeLetterRequest> iclbRateChangeLetterRequest { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busRateChangeLetterRequestLookupGen.LoadRateChangeLetterRequests(DataTable):
		/// Loads Collection object iclbRateChangeLetterRequest of type busRateChangeLetterRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busRateChangeLetterRequestLookupGen.iclbRateChangeLetterRequest</param>
		public virtual void LoadRateChangeLetterRequests(DataTable adtbSearchResult)
		{
			iclbRateChangeLetterRequest = GetCollection<busRateChangeLetterRequest>(adtbSearchResult, "icdoRateChangeLetterRequest");
		}
	}
}
