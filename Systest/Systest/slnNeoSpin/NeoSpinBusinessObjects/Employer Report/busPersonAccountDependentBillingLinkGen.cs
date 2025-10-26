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
    /// Class NeoSpin.BusinessObjects.busPersonAccountDependentBillingLinkGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountDependentBillingLink and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountDependentBillingLinkGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPersonAccountDependentBillingLinkGen
        /// </summary>
		public busPersonAccountDependentBillingLinkGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountDependentBillingLinkGen.
        /// </summary>
		public cdoPersonAccountDependentBillingLink icdoPersonAccountDependentBillingLink { get; set; }




        /// <summary>
        /// NeoSpin.busPersonAccountDependentBillingLinkGen.FindPersonAccountDependentBillingLink():
        /// Finds a particular record from cdoPersonAccountDependentBillingLink with its primary key. 
        /// </summary>
        /// <param name="aintpersonaccountdependentbillinglinkid">A primary key value of type int of cdoPersonAccountDependentBillingLink on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountDependentBillingLink(int aintpersonaccountdependentbillinglinkid)
		{
			bool lblnResult = false;
			if (icdoPersonAccountDependentBillingLink == null)
			{
				icdoPersonAccountDependentBillingLink = new cdoPersonAccountDependentBillingLink();
			}
			if (icdoPersonAccountDependentBillingLink.SelectRow(new object[1] { aintpersonaccountdependentbillinglinkid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
