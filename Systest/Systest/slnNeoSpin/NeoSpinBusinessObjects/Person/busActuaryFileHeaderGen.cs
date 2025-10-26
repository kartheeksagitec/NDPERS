#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busActuaryFileHeaderGen:
    /// Inherited from busBase, used to create new business object for main table cdoActuaryFileHeader and its children table. 
    /// </summary>
	[Serializable]
	public class busActuaryFileHeaderGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busActuaryFileHeaderGen
        /// </summary>
		public busActuaryFileHeaderGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busActuaryFileHeaderGen.
        /// </summary>
		public cdoActuaryFileHeader icdoActuaryFileHeader { get; set; }




        /// <summary>
        /// NeoSpin.BusinessObjects.busActuaryFileHeaderGen.FindActuaryFileHeader():
        /// Finds a particular record from cdoActuaryFileHeader with its primary key. 
        /// </summary>
        /// <param name="aintactuaryfileheaderid">A primary key value of type int of cdoActuaryFileHeader on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindActuaryFileHeader(int aintactuaryfileheaderid)
		{
			bool lblnResult = false;
			if (icdoActuaryFileHeader.IsNull())
			{
				icdoActuaryFileHeader = new cdoActuaryFileHeader();
			}
			if (icdoActuaryFileHeader.SelectRow(new object[1] { aintactuaryfileheaderid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
