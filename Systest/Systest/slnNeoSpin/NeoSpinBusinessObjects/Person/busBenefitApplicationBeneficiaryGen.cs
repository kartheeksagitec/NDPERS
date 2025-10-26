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
    /// Class NeoSpin.BusinessObjects.busBenefitApplicationBeneficiaryGen:
    /// Inherited from busBase, used to create new business object for main table cdoBenefitApplicationBeneficiary and its children table. 
    /// </summary>
	[Serializable]
	public class busBenefitApplicationBeneficiaryGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busBenefitApplicationBeneficiaryGen
        /// </summary>
		public busBenefitApplicationBeneficiaryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busBenefitApplicationBeneficiaryGen.
        /// </summary>
		public cdoBenefitApplicationBeneficiary icdoBenefitApplicationBeneficiary { get; set; }




        /// <summary>
        /// NeoSpin.busBenefitApplicationBeneficiaryGen.FindBenefitApplicationBeneficiary():
        /// Finds a particular record from cdoBenefitApplicationBeneficiary with its primary key. 
        /// </summary>
        /// <param name="aintapplicationbeneficiaryid">A primary key value of type int of cdoBenefitApplicationBeneficiary on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindBenefitApplicationBeneficiary(int aintapplicationbeneficiaryid)
		{
			bool lblnResult = false;
			if (icdoBenefitApplicationBeneficiary == null)
			{
				icdoBenefitApplicationBeneficiary = new cdoBenefitApplicationBeneficiary();
			}
			if (icdoBenefitApplicationBeneficiary.SelectRow(new object[1] { aintapplicationbeneficiaryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
