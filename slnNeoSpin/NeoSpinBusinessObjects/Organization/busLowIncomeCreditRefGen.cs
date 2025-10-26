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
    /// Class NeoSpin.BusinessObjects.busLowIncomeCreditRefGen:
    /// Inherited from busBase, used to create new business object for main table cdoLowIncomeCreditRef and its children table. 
    /// </summary>
	[Serializable]
	public class busLowIncomeCreditRefGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busLowIncomeCreditRefGen
        /// </summary>
		public busLowIncomeCreditRefGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busLowIncomeCreditRefGen.
        /// </summary>
		public cdoLowIncomeCreditRef icdoLowIncomeCreditRef { get; set; }




        /// <summary>
        /// NeoSpin.busLowIncomeCreditRefGen.FindLowIncomeCreditRef():
        /// Finds a particular record from cdoLowIncomeCreditRef with its primary key. 
        /// </summary>
        /// <param name="aintlowincomecreditrefid">A primary key value of type int of cdoLowIncomeCreditRef on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindLowIncomeCreditRef(int aintlowincomecreditrefid)
		{
			bool lblnResult = false;
			if (icdoLowIncomeCreditRef == null)
			{
				icdoLowIncomeCreditRef = new cdoLowIncomeCreditRef();
			}
			if (icdoLowIncomeCreditRef.SelectRow(new object[1] { aintlowincomecreditrefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
