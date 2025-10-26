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
	/// Class NeoSpin.BusinessObjects.busPlanMemberTypeLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busPlanMemberTypeLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busPlanMemberTypeCrossref. 
		/// </summary>
		public Collection<busPlanMemberTypeCrossref> iclbPlanMemberTypeCrossref { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busPlanMemberTypeLookupGen.LoadPlanMemberTypeCrossrefs(DataTable):
		/// Loads Collection object iclbPlanMemberTypeCrossref of type busPlanMemberTypeCrossref.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPlanMemberTypeLookupGen.iclbPlanMemberTypeCrossref</param>
		public virtual void LoadPlanMemberTypeCrossrefs(DataTable adtbSearchResult)
		{
			iclbPlanMemberTypeCrossref = GetCollection<busPlanMemberTypeCrossref>(adtbSearchResult, "icdoPlanMemberTypeCrossref");
		}
	}
}
