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
    /// Class NeoSpin.busRateChangeLetterDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoRateChangeLetterDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busRateChangeLetterDetailGen : busExtendBase
	{
        /// <summary>
        /// Constructor for NeoSpin.busRateChangeLetterDetailGen
        /// </summary>
		public busRateChangeLetterDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busRateChangeLetterDetailGen.
        /// </summary>
		public cdoRateChangeLetterDetail icdoRateChangeLetterDetail { get; set; }




        /// <summary>
        /// NeoSpin.busRateChangeLetterDetailGen.FindRateChangeLetterDetail():
        /// Finds a particular record from cdoRateChangeLetterDetail with its primary key. 
        /// </summary>
        /// <param name="aintRateChangeLetterDetailId">A primary key value of type int of cdoRateChangeLetterDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindRateChangeLetterDetail(int aintRateChangeLetterDetailId)
		{
			bool lblnResult = false;
			if (icdoRateChangeLetterDetail == null)
			{
				icdoRateChangeLetterDetail = new cdoRateChangeLetterDetail();
			}
			if (icdoRateChangeLetterDetail.SelectRow(new object[1] { aintRateChangeLetterDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
