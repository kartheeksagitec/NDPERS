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
	/// Class NeoSpin.BusinessObjects.busWSSMessageHeaderLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busWSSMessageHeaderLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busWssMessageHeader. 
		/// </summary>
		public Collection<busWssMessageHeader> iclbWssMessageHeader { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busWSSMessageHeaderLookupGen.LoadWssMessageHeaders(DataTable):
		/// Loads Collection object iclbWssMessageHeader of type busWssMessageHeader.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busWSSMessageHeaderLookupGen.iclbWssMessageHeader</param>
		public virtual void LoadWssMessageHeaders(DataTable adtbSearchResult)
		{
			iclbWssMessageHeader = GetCollection<busWssMessageHeader>(adtbSearchResult, "icdoWssMessageHeader");

            foreach (busWssMessageHeader lobjHeader in iclbWssMessageHeader)
            {
                lobjHeader.LoadPerson();
                lobjHeader.LoadOrganization();
            }
		}
	}
}
