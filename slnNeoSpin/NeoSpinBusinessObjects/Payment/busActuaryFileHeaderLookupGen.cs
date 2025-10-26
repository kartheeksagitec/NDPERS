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
	/// Class NeoSpin.BusinessObjects.busActuaryFileHeaderLookupGen:
	/// Inherited from busMainBase,It is used to create new look up business object. 
	/// </summary>

	[Serializable]
	public class busActuaryFileHeaderLookupGen : busMainBase
	{
		/// <summary>
		/// Gets or sets the collection object of type busActuaryFileHeader. 
		/// </summary>
		public Collection<busActuaryFileHeader> iclbActuaryFileHeader { get; set; }


		/// <summary>
		/// NeoSpin.BusinessObjects.busActuaryFileHeaderLookupGen.LoadActuaryFileHeaders(DataTable):
		/// Loads Collection object iclbActuaryFileHeader of type busActuaryFileHeader.
		/// </summary>
		/// <param name="adtbSearchResult">DataTable that holds search result to fill the collection object busActuaryFileHeaderLookupGen.iclbActuaryFileHeader</param>
		public virtual void LoadActuaryFileHeaders(DataTable adtbSearchResult)
		{
			iclbActuaryFileHeader = GetCollection<busActuaryFileHeader>(adtbSearchResult, "icdoActuaryFileHeader");
		}
	}
}
