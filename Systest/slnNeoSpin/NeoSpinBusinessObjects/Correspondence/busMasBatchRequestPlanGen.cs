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
    /// Class NeoSpin.BusinessObjects.busMasBatchRequestPlanGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasBatchRequestPlan and its children table. 
    /// </summary>
	[Serializable]
	public class busMasBatchRequestPlanGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasBatchRequestPlanGen
        /// </summary>
		public busMasBatchRequestPlanGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasBatchRequestPlanGen.
        /// </summary>
		public cdoMasBatchRequestPlan icdoMasBatchRequestPlan { get; set; }




        /// <summary>
        /// NeoSpin.busMasBatchRequestPlanGen.FindMasBatchRequestPlan():
        /// Finds a particular record from cdoMasBatchRequestPlan with its primary key. 
        /// </summary>
        /// <param name="aintmasbatchrequestplanid">A primary key value of type int of cdoMasBatchRequestPlan on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasBatchRequestPlan(int aintmasbatchrequestplanid)
		{
			bool lblnResult = false;
			if (icdoMasBatchRequestPlan == null)
			{
				icdoMasBatchRequestPlan = new cdoMasBatchRequestPlan();
			}
			if (icdoMasBatchRequestPlan.SelectRow(new object[1] { aintmasbatchrequestplanid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
