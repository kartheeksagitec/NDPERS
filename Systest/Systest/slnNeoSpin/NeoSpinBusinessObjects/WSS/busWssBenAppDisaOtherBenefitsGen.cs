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
    /// Class NeoSpin.busWssBenAppDisaOtherBenefitsGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssBenAppDisaOtherBenefits and its children table. 
    /// </summary>
	[Serializable]
	public class busWssBenAppDisaOtherBenefitsGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.busWssBenAppDisaOtherBenefitsGen
        /// </summary>
		public busWssBenAppDisaOtherBenefitsGen():base()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssBenAppDisaOtherBenefitsGen.
        /// </summary>
		public cdoWssBenAppDisaOtherBenefits icdoWssBenAppDisaOtherBenefits { get; set; }




        /// <summary>
        /// NeoSpin.busWssBenAppDisaOtherBenefitsGen.FindWssBenAppDisaOtherBenefits():
        /// Finds a particular record from cdoWssBenAppDisaOtherBenefits with its primary key. 
        /// </summary>
        /// <param name="aintDisaOthrBenefitId">A primary key value of type int of cdoWssBenAppDisaOtherBenefits on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssBenAppDisaOtherBenefits(int aintDisaOthrBenefitId)
		{
			bool lblnResult = false;
			if (icdoWssBenAppDisaOtherBenefits == null)
			{
				icdoWssBenAppDisaOtherBenefits = new cdoWssBenAppDisaOtherBenefits();
			}
			if (icdoWssBenAppDisaOtherBenefits.SelectRow(new object[1] { aintDisaOthrBenefitId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
