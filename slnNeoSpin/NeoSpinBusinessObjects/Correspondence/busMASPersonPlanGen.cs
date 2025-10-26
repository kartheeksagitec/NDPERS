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
    /// Class NeoSpin.BusinessObjects.busMasPersonPlanGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasPersonPlan and its children table. 
    /// </summary>
	[Serializable]
	public class busMasPersonPlanGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasPersonPlanGen
        /// </summary>
		public busMasPersonPlanGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasPersonPlanGen.
        /// </summary>
		public cdoMasPersonPlan icdoMasPersonPlan { get; set; }




        /// <summary>
        /// NeoSpin.busMasPersonPlanGen.FindMasPersonPlan():
        /// Finds a particular record from cdoMasPersonPlan with its primary key. 
        /// </summary>
        /// <param name="aintmaspersonplanid">A primary key value of type int of cdoMasPersonPlan on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasPersonPlan(int aintmaspersonplanid)
		{
			bool lblnResult = false;
			if (icdoMasPersonPlan == null)
			{
				icdoMasPersonPlan = new cdoMasPersonPlan();
			}
			if (icdoMasPersonPlan.SelectRow(new object[1] { aintmaspersonplanid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
