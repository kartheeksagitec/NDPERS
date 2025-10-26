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

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.busDocUploadGen:
    /// Inherited from busBase, used to create new business object for main table cdoDocUpload and its children table. 
    /// </summary>
	[Serializable]
	public class busDocUploadGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.busDocUploadGen
        /// </summary>
		public busDocUploadGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busDocUploadGen.
        /// </summary>
		public cdoDocUpload icdoDocUpload { get; set; }




        /// <summary>
        /// NeoSpin.busDocUploadGen.FindDocUpload():
        /// Finds a particular record from cdoDocUpload with its primary key. 
        /// </summary>
        /// <param name="aintUploadId">A primary key value of type int of cdoDocUpload on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindDocUpload(int aintUploadId)
		{
			bool lblnResult = false;
			if (icdoDocUpload == null)
			{
				icdoDocUpload = new cdoDocUpload();
			}
			if (icdoDocUpload.SelectRow(new object[1] { aintUploadId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
