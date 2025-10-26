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
	/// Class NeoSpin.BusinessObjects.busMemberDataRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busMemberDataRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busWssMemberRecordRequest. 
		/// </summary>
		public Collection<busWssMemberRecordRequest> iclbWssMemberRecordRequest { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busMemberDataRequestLookupGen.LoadWssMemberRecordRequests(DataTable):
		/// Loads Collection object iclbWssMemberRecordRequest of type busWssMemberRecordRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busMemberDataRequestLookupGen.iclbWssMemberRecordRequest</param>
		public virtual void LoadWssMemberRecordRequests(DataTable adtbSearchResult)
		{
			iclbWssMemberRecordRequest = GetCollection<busWssMemberRecordRequest>(adtbSearchResult, "icdoWssMemberRecordRequest");
		}
	}
}
