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
	/// Class NeoSpin.busBpmProcessEscalationLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busBpmProcessEscalationLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busBpmProcessEscalation. 
		/// </summary>
		public Collection<busSolBpmProcessEscalation> iclbBpmProcessEscalation { get; set; }


		/// <summary>
		/// NeoSpin.busBpmProcessEscalationLookupGen.LoadBpmProcessEscalations(DataTable):
		/// Loads Collection object iclbBpmProcessEscalation of type busBpmProcessEscalation.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busBpmProcessEscalationLookupGen.iclbBpmProcessEscalation</param>
		public virtual void LoadBpmProcessEscalations(DataTable adtbSearchResult)
		{
			iclbBpmProcessEscalation = GetCollection<busSolBpmProcessEscalation>(adtbSearchResult, "icdoBpmProcessEscalation");
            foreach (busSolBpmProcessEscalation lbusSolBpmProcessEscalation in iclbBpmProcessEscalation)
            {
                lbusSolBpmProcessEscalation.LoadBpmCase();
                lbusSolBpmProcessEscalation.LoadBpmProcess();
            }
		}
	}
}
