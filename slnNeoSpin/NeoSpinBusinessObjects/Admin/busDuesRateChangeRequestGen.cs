#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using  NeoSpin.CustomDataObjects;

#endregion

namespace  NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class  NeoSpin.BusinessObjects.busDuesRateChangeRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoDuesRateChangeRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busDuesRateChangeRequestGen : busExtendBase
    {
        /// <summary>
        /// Constructor for  NeoSpin.BusinessObjects.busDuesRateChangeRequestGen
        /// </summary>
		public busDuesRateChangeRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busDuesRateChangeRequestGen.
        /// </summary>
		public cdoDuesRateChangeRequest icdoDuesRateChangeRequest { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busOrganization.
        /// </summary>
		public busOrganization ibusVendor { get; set; }




        /// <summary>
        ///  NeoSpin.busDuesRateChangeRequestGen.FindDuesRateChangeRequest():
        /// Finds a particular record from cdoDuesRateChangeRequest with its primary key. 
        /// </summary>
        /// <param name="aintduesratechangerequestid">A primary key value of type int of cdoDuesRateChangeRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindDuesRateChangeRequest(int aintduesratechangerequestid)
		{
			bool lblnResult = false;
			if (icdoDuesRateChangeRequest == null)
			{
				icdoDuesRateChangeRequest = new cdoDuesRateChangeRequest();
			}
			if (icdoDuesRateChangeRequest.SelectRow(new object[1] { aintduesratechangerequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  NeoSpin.busDuesRateChangeRequestGen.LoadOrganization():
        /// Loads non-collection object ibusOrganization of type busOrganization.
        /// </summary>
		public virtual void LoadVendor()
		{
			if (ibusVendor == null)
			{
				ibusVendor = new busOrganization();
			}
			ibusVendor.FindOrganization(icdoDuesRateChangeRequest.vendor_org_id);
		}

	}
}
