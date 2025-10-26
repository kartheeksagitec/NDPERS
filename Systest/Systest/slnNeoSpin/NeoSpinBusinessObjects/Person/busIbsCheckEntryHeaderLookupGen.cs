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
	/// Class NeoSpin.BusinessObjects.busIbsCheckEntryHeaderLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busIbsCheckEntryHeaderLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busIbsCheckEntryHeader. 
		/// </summary>
		public Collection<busIbsCheckEntryHeader> iclbIbsCheckEntryHeader { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busIbsCheckEntryHeaderLookupGen.LoadIbsCheckEntryHeaders(DataTable):
		/// Loads Collection object iclbIbsCheckEntryHeader of type busIbsCheckEntryHeader.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busIbsCheckEntryHeaderLookupGen.iclbIbsCheckEntryHeader</param>
		public virtual void LoadIbsCheckEntryHeaders(DataTable adtbSearchResult)
		{
			iclbIbsCheckEntryHeader = GetCollection<busIbsCheckEntryHeader>(adtbSearchResult, "icdoIbsCheckEntryHeader");
		}
	}
}
