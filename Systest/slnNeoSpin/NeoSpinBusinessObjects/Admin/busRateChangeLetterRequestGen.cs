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
using System.Collections.Generic;

#endregion

namespace  NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class  NeoSpin.BusinessObjects.busRateChangeLetterRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoRateChangeLetterRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busRateChangeLetterRequestGen : busExtendBase
    {
        /// <summary>
        /// Constructor for  NeoSpin.BusinessObjects.busRateChangeLetterRequestGen
        /// </summary>
		public busRateChangeLetterRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busRateChangeLetterRequestGen.
        /// </summary>
		public cdoRateChangeLetterRequest icdoRateChangeLetterRequest { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busOrganization.
        /// </summary>
		public busOrganization ibusProviderOrganization { get; set; }

        public busOrgPlan ibusProviderOrgPlan { get; set; }

        public List<busOrgPlan> iclbOrgPlan { get; set; }

        public busPlan ibusPlan { get; set; }

        /// <summary>
        ///  NeoSpin.busRateChangeLetterRequestGen.FindRateChangeLetterRequest():
        /// Finds a particular record from cdoRateChangeLetterRequest with its primary key. 
        /// </summary>
        /// <param name="aintratechangeletterrequestid">A primary key value of type int of cdoRateChangeLetterRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindRateChangeLetterRequest(int aintratechangeletterrequestid)
		{
			bool lblnResult = false;
			if (icdoRateChangeLetterRequest == null)
			{
				icdoRateChangeLetterRequest = new cdoRateChangeLetterRequest();
			}
			if (icdoRateChangeLetterRequest.SelectRow(new object[1] { aintratechangeletterrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        ///  NeoSpin.busRateChangeLetterRequestGen.LoadOrganization():
        /// Loads non-collection object ibusOrganization of type busOrganization.
        /// </summary>
		public virtual void LoadProviderOrganization()
		{
			if (ibusProviderOrganization == null)
			{
				ibusProviderOrganization = new busOrganization();
			}
			ibusProviderOrganization.FindOrganization(icdoRateChangeLetterRequest.provider_org_id);
		}

	}
}
