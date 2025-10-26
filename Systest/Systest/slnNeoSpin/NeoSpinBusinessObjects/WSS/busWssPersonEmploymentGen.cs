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
    /// Class NeoSpin.BusinessObjects.busWssPersonEmploymentGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssPersonEmployment and its children table. 
    /// </summary>
	[Serializable]
	public class busWssPersonEmploymentGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssPersonEmploymentGen
        /// </summary>
		public busWssPersonEmploymentGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssPersonEmploymentGen.
        /// </summary>
		public cdoWssPersonEmployment icdoWssPersonEmployment { get; set; }




        /// <summary>
        /// NeoSpin.busWssPersonEmploymentGen.FindWssPersonEmployment():
        /// Finds a particular record from cdoWssPersonEmployment with its primary key. 
        /// </summary>
        /// <param name="aintwsspersonemploymentid">A primary key value of type int of cdoWssPersonEmployment on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssPersonEmployment(int aintwsspersonemploymentid)
		{
			bool lblnResult = false;
			if (icdoWssPersonEmployment == null)
			{
				icdoWssPersonEmployment = new cdoWssPersonEmployment();
			}
			if (icdoWssPersonEmployment.SelectRow(new object[1] { aintwsspersonemploymentid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
