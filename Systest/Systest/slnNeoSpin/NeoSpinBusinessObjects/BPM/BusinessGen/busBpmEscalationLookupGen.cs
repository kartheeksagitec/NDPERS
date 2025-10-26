#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.Bpm;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.busBpmEscalationLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busBpmEscalationLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busBpmEscalation. 
		/// </summary>
		public Collection<busBpmEscalation> iclbBpmEscalation { get; set; }


		/// <summary>
		/// NeoSpin.busBpmEscalationLookupGen.LoadBpmEscalations(DataTable):
		/// Loads Collection object iclbBpmEscalation of type busBpmEscalation.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busBpmEscalationLookupGen.iclbBpmEscalation</param>
		public virtual void LoadBpmEscalations(DataTable adtbSearchResult)
		{
			iclbBpmEscalation = GetCollection<busBpmEscalation>(adtbSearchResult, "icdoBpmEscalation");
		}
	}
}
