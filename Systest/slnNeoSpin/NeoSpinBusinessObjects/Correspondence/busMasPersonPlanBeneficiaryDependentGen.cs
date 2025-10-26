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
    /// Class NeoSpin.BusinessObjects.busMasPersonPlanBeneficiaryDependentGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasPersonPlanBeneficiaryDependent and its children table. 
    /// </summary>
	[Serializable]
	public class busMasPersonPlanBeneficiaryDependentGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasPersonPlanBeneficiaryDependentGen
        /// </summary>
		public busMasPersonPlanBeneficiaryDependentGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasPersonPlanBeneficiaryDependentGen.
        /// </summary>
		public cdoMasPersonPlanBeneficiaryDependent icdoMasPersonPlanBeneficiaryDependent { get; set; }




        /// <summary>
        /// NeoSpin.busMasPersonPlanBeneficiaryDependentGen.FindMasPersonPlanBeneficiaryDependent():
        /// Finds a particular record from cdoMasPersonPlanBeneficiaryDependent with its primary key. 
        /// </summary>
        /// <param name="aintmaspersonplanbeneficiaryid">A primary key value of type int of cdoMasPersonPlanBeneficiaryDependent on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasPersonPlanBeneficiaryDependent(int aintmaspersonplanbeneficiaryid)
		{
			bool lblnResult = false;
			if (icdoMasPersonPlanBeneficiaryDependent == null)
			{
				icdoMasPersonPlanBeneficiaryDependent = new cdoMasPersonPlanBeneficiaryDependent();
			}
			if (icdoMasPersonPlanBeneficiaryDependent.SelectRow(new object[1] { aintmaspersonplanbeneficiaryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
