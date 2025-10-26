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
    /// Class NeoSpin.BusinessObjects.busPersonAccountDeferredCompTransferGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountDeferredCompTransfer and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountDeferredCompTransferGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPersonAccountDeferredCompTransferGen
        /// </summary>
		public busPersonAccountDeferredCompTransferGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountDeferredCompTransferGen.
        /// </summary>
		public cdoPersonAccountDeferredCompTransfer icdoPersonAccountDeferredCompTransfer { get; set; }
        
        //used to load Def Comp From Person Account details
        public busPersonAccountDeferredComp ibusFromPADefComp { get; set; }

        //used to load Def Comp To Person Account details
        public busPersonAccountDeferredComp ibusToPADefComp { get; set; }

        //to load destination collection
        public Collection<busPersonAccountDeferredCompContribution> iclbPersonAccountDefCompTransferSourceContribution { get; set; }   

        //to load destination collection
        public Collection<busPersonAccountDeferredCompTransferContribution> iclbPersonAccountDefCompTransferDestinationContribution { get; set; }    

        /// <summary>
        /// NeoSpin.BusinessObjects.busPersonAccountDeferredCompTransferGen.FindPersonAccountDeferredCompTransfer():
        /// Finds a particular record from cdoPersonAccountDeferredCompTransfer with its primary key. 
        /// </summary>
        /// <param name="aintpersonaccountdeferredcomptransferid">A primary key value of type int of cdoPersonAccountDeferredCompTransfer on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountDeferredCompTransfer(int aintpersonaccountdeferredcomptransferid)
		{
			bool lblnResult = false;
			if (icdoPersonAccountDeferredCompTransfer ==  null)
			{
				icdoPersonAccountDeferredCompTransfer = new cdoPersonAccountDeferredCompTransfer();
			}
			if (icdoPersonAccountDeferredCompTransfer.SelectRow(new object[1] { aintpersonaccountdeferredcomptransferid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
