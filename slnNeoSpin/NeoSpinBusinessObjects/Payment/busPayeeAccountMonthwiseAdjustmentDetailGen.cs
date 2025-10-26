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
    /// Class NeoSpin.BusinessObjects.busPayeeAccountMonthwiseAdjustmentDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountMonthwiseAdjustmentDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountMonthwiseAdjustmentDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPayeeAccountMonthwiseAdjustmentDetailGen
        /// </summary>
		public busPayeeAccountMonthwiseAdjustmentDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountMonthwiseAdjustmentDetailGen.
        /// </summary>
		public cdoPayeeAccountMonthwiseAdjustmentDetail icdoPayeeAccountMonthwiseAdjustmentDetail { get; set; }




        /// <summary>
        /// NeoSpin.busPayeeAccountMonthwiseAdjustmentDetailGen.FindPayeeAccountMonthwiseAdjustmentDetail():
        /// Finds a particular record from cdoPayeeAccountMonthwiseAdjustmentDetail with its primary key. 
        /// </summary>
        /// <param name="aintretropaymentmonthwisedetailid">A primary key value of type int of cdoPayeeAccountMonthwiseAdjustmentDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountMonthwiseAdjustmentDetail(int aintretropaymentmonthwisedetailid)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountMonthwiseAdjustmentDetail == null)
			{
				icdoPayeeAccountMonthwiseAdjustmentDetail = new cdoPayeeAccountMonthwiseAdjustmentDetail();
			}
			if (icdoPayeeAccountMonthwiseAdjustmentDetail.SelectRow(new object[1] { aintretropaymentmonthwisedetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
