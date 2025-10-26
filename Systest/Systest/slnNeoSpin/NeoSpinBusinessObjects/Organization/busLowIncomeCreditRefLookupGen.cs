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
	/// Class NeoSpin.BusinessObjects.busLowIncomeCreditRefLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busLowIncomeCreditRefLookupGen : busExtendBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busLowIncomeCreditRef. 
		/// </summary>
		public Collection<busLowIncomeCreditRef> iclbLowIncomeCreditRef { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busLowIncomeCreditRefLookupGen.LoadLowIncomeCreditRefs(DataTable):
		/// Loads Collection object iclbLowIncomeCreditRef of type busLowIncomeCreditRef.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busLowIncomeCreditRefLookupGen.iclbLowIncomeCreditRef</param>
		public virtual void LoadLowIncomeCreditRefs(DataTable adtbSearchResult)
		{
			iclbLowIncomeCreditRef = GetCollection<busLowIncomeCreditRef>(adtbSearchResult, "icdoLowIncomeCreditRef");
		}
	}
}
