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
    /// Class NeoSpin.BusinessObjects.busUpdateOrgPlanProviderGen:
    /// Inherited from busBase, used to create new business object for main table cdoUpdateOrgPlanProvider and its children table. 
    /// </summary>
	[Serializable]
	public class busUpdateOrgPlanProviderGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busUpdateOrgPlanProviderGen
        /// </summary>
		public busUpdateOrgPlanProviderGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busUpdateOrgPlanProviderGen.
        /// </summary>
		public cdoUpdateOrgPlanProvider icdoUpdateOrgPlanProvider { get; set; }




        /// <summary>
        /// NeoSpin.busUpdateOrgPlanProviderGen.FindUpdateOrgPlanProvider():
        /// Finds a particular record from cdoUpdateOrgPlanProvider with its primary key. 
        /// </summary>
        /// <param name="aintrequestid">A primary key value of type int of cdoUpdateOrgPlanProvider on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindUpdateOrgPlanProvider(int aintrequestid)
		{
			bool lblnResult = false;
			if (icdoUpdateOrgPlanProvider == null)
			{
				icdoUpdateOrgPlanProvider = new cdoUpdateOrgPlanProvider();
			}
			if (icdoUpdateOrgPlanProvider.SelectRow(new object[1] { aintrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
