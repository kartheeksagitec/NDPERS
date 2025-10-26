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
    /// Class NeoSpin.BusinessObjects.busActuaryFilePensionDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoActuaryFilePensionDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busActuaryFilePensionDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busActuaryFilePensionDetailGen
        /// </summary>
		public busActuaryFilePensionDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busActuaryFilePensionDetailGen.
        /// </summary>
		public cdoActuaryFilePensionDetail icdoActuaryFilePensionDetail { get; set; }




        /// <summary>
        /// NeoSpin.BusinessObjects.busActuaryFilePensionDetailGen.FindActuaryFilePensionDetail():
        /// Finds a particular record from cdoActuaryFilePensionDetail with its primary key. 
        /// </summary>
        /// <param name="aintactuaryfilepensiondetailid">A primary key value of type int of cdoActuaryFilePensionDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindActuaryFilePensionDetail(int aintactuaryfilepensiondetailid)
		{
			bool lblnResult = false;
			if (icdoActuaryFilePensionDetail.IsNull())
			{
				icdoActuaryFilePensionDetail = new cdoActuaryFilePensionDetail();
			}
			if (icdoActuaryFilePensionDetail.SelectRow(new object[1] { aintactuaryfilepensiondetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
