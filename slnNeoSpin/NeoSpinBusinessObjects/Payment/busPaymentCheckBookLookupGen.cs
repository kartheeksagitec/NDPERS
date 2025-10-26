#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busPaymentCheckBookLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busPaymentCheckBookLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busPaymentCheckBook. 
		/// </summary>
		public Collection<busPaymentCheckBook> iclbPaymentCheckBook { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busPaymentCheckBookLookupGen.LoadPaymentCheckBooks(DataTable):
		/// Loads Collection object iclbPaymentCheckBook of type busPaymentCheckBook.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busPaymentCheckBookLookupGen.iclbPaymentCheckBook</param>
		public virtual void LoadPaymentCheckBooks(DataTable adtbSearchResult)
		{
			iclbPaymentCheckBook = GetCollection<busPaymentCheckBook>(adtbSearchResult, "icdoPaymentCheckBook");
		}
	}
}
