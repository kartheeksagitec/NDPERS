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
	/// Class NeoSpin.BusinessObjects.busMSSPaymentHistoryLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busMSSPaymentHistoryLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busPaymentHistoryHeader. 
		/// </summary>
		public Collection<busPaymentHistoryHeader> iclbPaymentHistoryHeader { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busMSSPaymentHistoryLookupGen.LoadPaymentHistoryHeaders(DataTable):
		/// Loads Collection object iclbPaymentHistoryHeader of type busPaymentHistoryHeader.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busMSSPaymentHistoryLookupGen.iclbPaymentHistoryHeader</param>
		public virtual void LoadPaymentHistoryHeaders(DataTable adtbSearchResult)
		{
			iclbPaymentHistoryHeader = GetCollection<busPaymentHistoryHeader>(adtbSearchResult, "icdoPaymentHistoryHeader");
		}
	}
}
