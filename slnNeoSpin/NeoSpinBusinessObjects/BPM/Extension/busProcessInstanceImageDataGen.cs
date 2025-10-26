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
    /// Class NeoSpin.BusinessObjects.busProcessInstanceImageDataGen:
    /// Inherited from busBase, used to create new business object for main table cdoProcessInstanceImageData and its children table. 
    /// </summary>
	[Serializable]
	public class busProcessInstanceImageDataGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busProcessInstanceImageDataGen
        /// </summary>
		public busProcessInstanceImageDataGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busProcessInstanceImageDataGen.
        /// </summary>
		public cdoProcessInstanceImageData icdoProcessInstanceImageData { get; set; }




        /// <summary>
        /// NeoSpin.busProcessInstanceImageDataGen.FindProcessInstanceImageData():
        /// Finds a particular record from cdoProcessInstanceImageData with its primary key. 
        /// </summary>
        /// <param name="aintprocessinstanceimagedataid">A primary key value of type int of cdoProcessInstanceImageData on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindProcessInstanceImageData(int aintprocessinstanceimagedataid)
		{
			bool lblnResult = false;
			if (icdoProcessInstanceImageData == null)
			{
				icdoProcessInstanceImageData = new cdoProcessInstanceImageData();
			}
			if (icdoProcessInstanceImageData.SelectRow(new object[1] { aintprocessinstanceimagedataid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
