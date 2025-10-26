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
	/// Class NeoSpin.BusinessObjects.busUserActivityLogQueryLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busUserActivityLogQueryLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busUserActivityLogQueries. 
		/// </summary>
		public Collection<busUserActivityLogQueries> iclbUserActivityLogQueries { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busUserActivityLogQueryLookupGen.LoadUserActivityLogQueriess(DataTable):
		/// Loads Collection object iclbUserActivityLogQueries of type busUserActivityLogQueries.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busUserActivityLogQueryLookupGen.iclbUserActivityLogQueries</param>
		public virtual void LoadUserActivityLogQueriess(DataTable adtbSearchResult)
		{
			iclbUserActivityLogQueries = GetCollection<busUserActivityLogQueries>(adtbSearchResult, "icdoUserActivityLogQueries");
		}
	}
}
