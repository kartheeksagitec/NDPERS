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
    /// Class NeoSpin.BusinessObjects.busWssPersonEmploymentDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonEmploymentDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonEmploymentDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonEmploymentDetailGen
        /// </summary>
		public busWssPersonEmploymentDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonEmploymentDetailGen.
        /// </summary>
		public cdoWssPersonEmploymentDetail icdoWssPersonEmploymentDetail { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonEmploymentDetailGen.FindWssPersonEmploymentDetail():
        /// Finds a particular record from cdoWssPersonEmploymentDetail with its primary key. 
        /// </summary>
        /// <param name="aintwsspersonemploymentdtlid">A primary key value of type int of cdoWssPersonEmploymentDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonEmploymentDetail(int aintwsspersonemploymentdtlid)
		{
			bool lblnResult = false;
			if (icdoWssPersonEmploymentDetail == null)
			{
				icdoWssPersonEmploymentDetail = new cdoWssPersonEmploymentDetail();
			}
			if (icdoWssPersonEmploymentDetail.SelectRow(new object[1] { aintwsspersonemploymentdtlid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
