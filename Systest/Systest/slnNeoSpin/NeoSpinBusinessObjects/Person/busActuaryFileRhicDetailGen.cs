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
    /// Class NeoSpin.BusinessObjects.busActuaryFileRhicDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoActuaryFileRhicDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busActuaryFileRhicDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busActuaryFileRhicDetailGen
        /// </summary>
		public busActuaryFileRhicDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busActuaryFileRhicDetailGen.
        /// </summary>
		public cdoActuaryFileRhicDetail icdoActuaryFileRhicDetail { get; set; }




        /// <summary>
        /// NeoSpin.BusinessObjects.busActuaryFileRhicDetailGen.FindActuaryFileRhicDetail():
        /// Finds a particular record from cdoActuaryFileRhicDetail with its primary key. 
        /// </summary>
        /// <param name="aintactuaryfilerhicdetailid">A primary key value of type int of cdoActuaryFileRhicDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindActuaryFileRhicDetail(int aintactuaryfilerhicdetailid)
		{
			bool lblnResult = false;
			if (icdoActuaryFileRhicDetail.IsNull())
			{
				icdoActuaryFileRhicDetail = new cdoActuaryFileRhicDetail();
			}
			if (icdoActuaryFileRhicDetail.SelectRow(new object[1] { aintactuaryfilerhicdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
