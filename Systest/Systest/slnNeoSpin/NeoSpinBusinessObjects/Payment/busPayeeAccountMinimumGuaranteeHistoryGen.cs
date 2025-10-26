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
    /// Class NeoSpin.BusinessObjects.busPayeeAccountMinimumGuaranteeHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountMinimumGuaranteeHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountMinimumGuaranteeHistoryGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPayeeAccountMinimumGuaranteeHistoryGen
        /// </summary>
		public busPayeeAccountMinimumGuaranteeHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountMinimumGuaranteeHistoryGen.
        /// </summary>
		public cdoPayeeAccountMinimumGuaranteeHistory icdoPayeeAccountMinimumGuaranteeHistory { get; set; }




        /// <summary>
        /// NeoSpin.busPayeeAccountMinimumGuaranteeHistoryGen.FindPayeeAccountMinimumGuaranteeHistory():
        /// Finds a particular record from cdoPayeeAccountMinimumGuaranteeHistory with its primary key. 
        /// </summary>
        /// <param name="aintmininumguaranteehistoryid">A primary key value of type int of cdoPayeeAccountMinimumGuaranteeHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountMinimumGuaranteeHistory(int aintmininumguaranteehistoryid)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountMinimumGuaranteeHistory == null)
			{
				icdoPayeeAccountMinimumGuaranteeHistory = new cdoPayeeAccountMinimumGuaranteeHistory();
			}
			if (icdoPayeeAccountMinimumGuaranteeHistory.SelectRow(new object[1] { aintmininumguaranteehistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
