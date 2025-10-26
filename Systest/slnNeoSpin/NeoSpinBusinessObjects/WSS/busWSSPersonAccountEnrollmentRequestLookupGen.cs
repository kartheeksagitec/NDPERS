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
	/// Class NeoSpin.BusinessObjects.busWSSPersonAccountEnrollmentRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busWSSPersonAccountEnrollmentRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busWssPersonAccountEnrollmentRequest. 
		/// </summary>
		public Collection<busWssPersonAccountEnrollmentRequest> iclbWssPersonAccountEnrollmentRequest { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busWSSPersonAccountEnrollmentRequestLookupGen.LoadWssPersonAccountEnrollmentRequests(DataTable):
		/// Loads Collection object iclbWssPersonAccountEnrollmentRequest of type busWssPersonAccountEnrollmentRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busWSSPersonAccountEnrollmentRequestLookupGen.iclbWssPersonAccountEnrollmentRequest</param>
		public virtual void LoadWssPersonAccountEnrollmentRequests(DataTable adtbSearchResult)
		{
			iclbWssPersonAccountEnrollmentRequest = GetCollection<busWssPersonAccountEnrollmentRequest>(adtbSearchResult, "icdoWssPersonAccountEnrollmentRequest");
		}
	}
}
