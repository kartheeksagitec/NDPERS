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
	/// Class NeoSpin.BusinessObjects.busPayment1099rRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busPayment1099rRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busPayment1099rRequest. 
		/// </summary>
		public Collection<busPayment1099rRequest> iclbPayment1099rRequest { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busPayment1099rRequestLookupGen.LoadPayment1099rRequests(DataTable):
		/// Loads Collection object iclbPayment1099rRequest of type busPayment1099rRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPayment1099rRequestLookupGen.iclbPayment1099rRequest</param>
		public virtual void LoadPayment1099rRequests(DataTable adtbSearchResult)
		{
			iclbPayment1099rRequest = GetCollection<busPayment1099rRequest>(adtbSearchResult, "icdoPayment1099rRequest");
		}
	}
}
