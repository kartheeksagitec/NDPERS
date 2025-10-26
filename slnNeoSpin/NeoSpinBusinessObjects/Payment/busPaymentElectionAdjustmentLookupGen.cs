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
	/// Class NeoSpin.BusinessObjects.busPaymentElectionAdjustmentLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busPaymentElectionAdjustmentLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busPaymentElectionAdjustment. 
		/// </summary>
		public Collection<busPaymentElectionAdjustment> iclbPaymentElectionAdjustment { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busPaymentElectionAdjustmentLookupGen.LoadPaymentElectionAdjustments(DataTable):
		/// Loads Collection object iclbPaymentElectionAdjustment of type busPaymentElectionAdjustment.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPaymentElectionAdjustmentLookupGen.iclbPaymentElectionAdjustment</param>
		public virtual void LoadPaymentElectionAdjustments(DataTable adtbSearchResult)
		{
			iclbPaymentElectionAdjustment = GetCollection<busPaymentElectionAdjustment>(adtbSearchResult, "icdoPaymentElectionAdjustment");
            foreach (busPaymentElectionAdjustment lobjAdjustment in iclbPaymentElectionAdjustment)
            {
                lobjAdjustment.LoadPersonAccount();
                lobjAdjustment.ibusPersonAccount.LoadPerson();
                lobjAdjustment.ibusPersonAccount.LoadPlan();
            }
		}
	}
}
