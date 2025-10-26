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
    /// Class NeoSpin.BusinessObjects.busWssPersonAccountOtherCoverageDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonAccountOtherCoverageDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonAccountOtherCoverageDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonAccountOtherCoverageDetailGen
        /// </summary>
		public busWssPersonAccountOtherCoverageDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonAccountOtherCoverageDetailGen.
        /// </summary>
		public cdoWssPersonAccountOtherCoverageDetail icdoWssPersonAccountOtherCoverageDetail { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonAccountOtherCoverageDetailGen.FindWssPersonAccountOtherCoverageDetail():
        /// Finds a particular record from cdoWssPersonAccountOtherCoverageDetail with its primary key. 
        /// </summary>
        /// <param name="aintwsspersonaccountothercoveragedetailid">A primary key value of type int of cdoWssPersonAccountOtherCoverageDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonAccountOtherCoverageDetail(int aintwsspersonaccountothercoveragedetailid)
		{
			bool lblnResult = false;
			if (icdoWssPersonAccountOtherCoverageDetail == null)
			{
				icdoWssPersonAccountOtherCoverageDetail = new cdoWssPersonAccountOtherCoverageDetail();
			}
			if (icdoWssPersonAccountOtherCoverageDetail.SelectRow(new object[1] { aintwsspersonaccountothercoveragedetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
