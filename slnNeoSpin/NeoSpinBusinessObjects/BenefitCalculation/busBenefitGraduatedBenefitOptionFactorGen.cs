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
    /// Class NeoSpin.BusinessObjects.busBenefitGraduatedBenefitOptionFactorGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitGraduatedBenefitOptionFactor and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitGraduatedBenefitOptionFactorGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busBenefitGraduatedBenefitOptionFactorGen
        /// </summary>
		public busBenefitGraduatedBenefitOptionFactorGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitGraduatedBenefitOptionFactorGen.
        /// </summary>
		public cdoBenefitGraduatedBenefitOptionFactor icdoBenefitGraduatedBenefitOptionFactor { get; set; }




        /// <summary>
        /// NeoSpin.busBenefitGraduatedBenefitOptionFactorGen.FindBenefitGraduatedBenefitOptionFactor():
        /// Finds a particular record from cdoBenefitGraduatedBenefitOptionFactor with its primary key. 
        /// </summary>
        /// <param name="aintbenefitgraduatedbenefitoptionfactorid">A primary key value of type int of cdoBenefitGraduatedBenefitOptionFactor on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitGraduatedBenefitOptionFactor(int aintbenefitgraduatedbenefitoptionfactorid)
		{
			bool lblnResult = false;
			if (icdoBenefitGraduatedBenefitOptionFactor == null)
			{
				icdoBenefitGraduatedBenefitOptionFactor = new cdoBenefitGraduatedBenefitOptionFactor();
			}
			if (icdoBenefitGraduatedBenefitOptionFactor.SelectRow(new object[1] { aintbenefitgraduatedbenefitoptionfactorid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
