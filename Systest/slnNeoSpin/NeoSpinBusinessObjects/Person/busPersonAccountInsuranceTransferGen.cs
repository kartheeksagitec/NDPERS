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
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPersonAccountInsuranceTransferGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountInsuranceTransfer and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountInsuranceTransferGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPersonAccountInsuranceTransferGen
        /// </summary>
		public busPersonAccountInsuranceTransferGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountInsuranceTransferGen.
        /// </summary>
		public cdoPersonAccountInsuranceTransfer icdoPersonAccountInsuranceTransfer { get; set; }  

        public busPersonAccount ibusFromPersonAccount { get; set; }

        public busPersonAccount ibusToPersonAccount { get; set; }

        //used to load Source Insurance Contribution 
        public Collection<busPersonAccountInsuranceContribution> iclbPersonAccountInsuranceTransferSourceContribution { get; set; }

        //used to load Destination Insurance Contribution 
        public Collection<busPersonAccountInsuranceTransferContribution> iclbPersonAccountInsuranceTransferDestinationContribution { get; set; }

        /// <summary>
        /// NeoSpin.BusinessObjects.busPersonAccountInsuranceTransferGen.FindPersonAccountInsuranceTransfer():
        /// Finds a particular record from cdoPersonAccountInsuranceTransfer with its primary key. 
        /// </summary>
        /// <param name="aintpersonaccountinsurancetransferid">A primary key value of type int of cdoPersonAccountInsuranceTransfer on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountInsuranceTransfer(int aintpersonaccountinsurancetransferid)
		{
			bool lblnResult = false;
			if (icdoPersonAccountInsuranceTransfer.IsNull())
			{
				icdoPersonAccountInsuranceTransfer = new cdoPersonAccountInsuranceTransfer();
			}
			if (icdoPersonAccountInsuranceTransfer.SelectRow(new object[1] { aintpersonaccountinsurancetransferid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
