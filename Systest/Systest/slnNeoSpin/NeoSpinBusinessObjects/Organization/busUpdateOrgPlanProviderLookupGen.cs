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
	/// Class NeoSpin.BusinessObjects.busUpdateOrgPlanProviderLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busUpdateOrgPlanProviderLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busUpdateOrgPlanProvider. 
		/// </summary>
		public Collection<busUpdateOrgPlanProvider> iclbUpdateOrgPlanProvider { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busUpdateOrgPlanProviderLookupGen.LoadUpdateOrgPlanProviders(DataTable):
		/// Loads Collection object iclbUpdateOrgPlanProvider of type busUpdateOrgPlanProvider.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busUpdateOrgPlanProviderLookupGen.iclbUpdateOrgPlanProvider</param>
		public virtual void LoadUpdateOrgPlanProviders(DataTable adtbSearchResult)
		{
			iclbUpdateOrgPlanProvider = GetCollection<busUpdateOrgPlanProvider>(adtbSearchResult, "icdoUpdateOrgPlanProvider");
		}
	}
}
