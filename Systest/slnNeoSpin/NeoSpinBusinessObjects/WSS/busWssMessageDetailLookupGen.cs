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
	/// Class NeoSpin.BusinessObjects.busWssMessageDetailLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busWssMessageDetailLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busWssMessageDetail. 
		/// </summary>
		public Collection<busWssMessageDetail> iclbWssMessageDetail { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busWssMessageDetailLookupGen.LoadWssMessageDetails(DataTable):
		/// Loads Collection object iclbWssMessageDetail of type busWssMessageDetail.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busWssMessageDetailLookupGen.iclbWssMessageDetail</param>
		public virtual void LoadWssMessageDetails(DataTable adtbSearchResult)
		{
			iclbWssMessageDetail = GetCollection<busWssMessageDetail>(adtbSearchResult, "icdoWssMessageDetail");
		}
	}
}
