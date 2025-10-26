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
    /// Class NeoSpin.BusinessObjects.busPsEmploymentGen:
    /// Inherited from busBase, used to create new business object for main table cdoPsEmployment and its children table. 
    /// </summary>
	[Serializable]
	public class busPsEmploymentGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPsEmploymentGen
        /// </summary>
		public busPsEmploymentGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPsEmploymentGen.
        /// </summary>
		public cdoPsEmployment icdoPsEmployment { get; set; }




        /// <summary>
        /// NeoSpin.busPsEmploymentGen.FindPsEmployment():
        /// Finds a particular record from cdoPsEmployment with its primary key. 
        /// </summary>
        /// <param name="aintPsEmploymentId">A primary key value of type int of cdoPsEmployment on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPsEmployment(int aintPsEmploymentId)
		{
			bool lblnResult = false;
			if (icdoPsEmployment == null)
			{
				icdoPsEmployment = new cdoPsEmployment();
			}
			if (icdoPsEmployment.SelectRow(new object[1] { aintPsEmploymentId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
