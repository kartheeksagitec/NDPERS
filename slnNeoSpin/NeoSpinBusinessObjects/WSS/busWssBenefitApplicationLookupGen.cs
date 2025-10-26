#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.busWssBenefitApplicationLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busWssBenefitApplicationLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busWssBenApp. 
		/// </summary>
		public Collection<busWssBenApp> iclbWssBenApp { get; set; }


		/// <summary>
		/// NeoSpin.busWssBenefitApplicationLookupGen.LoadWssBenApps(DataTable):
		/// Loads Collection object iclbWssBenApp of type busWssBenApp.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busWssBenefitApplicationLookupGen.iclbWssBenApp</param>
		public virtual void LoadWssBenApps(DataTable adtbSearchResult)
		{
			iclbWssBenApp = GetCollection<busWssBenApp>(adtbSearchResult, "icdoWssBenApp");
		}
	}
}
