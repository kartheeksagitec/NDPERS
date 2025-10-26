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
	/// Class NeoSpin.BusinessObjects.busWSSEmploymentChangeRequestLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busWSSEmploymentChangeRequestLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busWssEmploymentChangeRequest. 
		/// </summary>
		public Collection<busWssEmploymentChangeRequest> iclbWssEmploymentChangeRequest { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busWSSEmploymentChangeRequestLookupGen.LoadWssEmploymentChangeRequests(DataTable):
		/// Loads Collection object iclbWssEmploymentChangeRequest of type busWssEmploymentChangeRequest.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busWSSEmploymentChangeRequestLookupGen.iclbWssEmploymentChangeRequest</param>
		public virtual void LoadWssEmploymentChangeRequests(DataTable adtbSearchResult)
		{
			iclbWssEmploymentChangeRequest = GetCollection<busWssEmploymentChangeRequest>(adtbSearchResult, "icdoWssEmploymentChangeRequest");
            foreach (busWssEmploymentChangeRequest lobjEmploymentChangeRequest in iclbWssEmploymentChangeRequest)
            {
                lobjEmploymentChangeRequest.LoadPerson();
                lobjEmploymentChangeRequest.LoadOrganization();
                lobjEmploymentChangeRequest.istrTypeOfChange = string.IsNullOrEmpty(lobjEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value) ?
                    "TERM" : "STAT";
                lobjEmploymentChangeRequest.ibusPerson.iintOrgID = lobjEmploymentChangeRequest.icdoWssEmploymentChangeRequest.org_id;
                lobjEmploymentChangeRequest.ibusPerson.ESSLoadPersonEmployment();
            }
		}
	}
}
