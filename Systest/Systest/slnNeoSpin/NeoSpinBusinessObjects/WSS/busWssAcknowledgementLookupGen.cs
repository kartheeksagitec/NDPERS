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
	/// Class NeoSpin.BusinessObjects.busWssAcknowledgementLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busWssAcknowledgementLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busWssAcknowledgement. 
		/// </summary>
		public Collection<busWssAcknowledgement> iclbWssAcknowledgement { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busWssAcknowledgementLookupGen.LoadWssAcknowledgements(DataTable):
		/// Loads Collection object iclbWssAcknowledgement of type busWssAcknowledgement.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busWssAcknowledgementLookupGen.iclbWssAcknowledgement</param>
		public void LoadWssAcknowledgement(DataTable adtbSearchResult)
		{
            iclbWssAcknowledgement = GetCollection<busWssAcknowledgement>(adtbSearchResult, "icdoWssAcknowledgement");
        }
	}
}
