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
    /// Class NeoSpin.BusinessObjects.busHealthPremiumReportBatchRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoHealthPremiumReportBatchRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busHealthPremiumReportBatchRequestGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busHealthPremiumReportBatchRequestGen
        /// </summary>
		public busHealthPremiumReportBatchRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busHealthPremiumReportBatchRequestGen.
        /// </summary>
		public cdoHealthPremiumReportBatchRequest icdoHealthPremiumReportBatchRequest { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPerson.
        /// </summary>
		public busPerson ibusPerson { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busOrganization.
        /// </summary>
		public busOrganization ibusOrganization { get; set; }




        /// <summary>
        /// NeoSpin.busHealthPremiumReportBatchRequestGen.FindHealthPremiumReportBatchRequest():
        /// Finds a particular record from cdoHealthPremiumReportBatchRequest with its primary key. 
        /// </summary>
        /// <param name="aintbatchrequestid">A primary key value of type int of cdoHealthPremiumReportBatchRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindHealthPremiumReportBatchRequest(int aintbatchrequestid)
		{
			bool lblnResult = false;
			if (icdoHealthPremiumReportBatchRequest == null)
			{
				icdoHealthPremiumReportBatchRequest = new cdoHealthPremiumReportBatchRequest();
			}
			if (icdoHealthPremiumReportBatchRequest.SelectRow(new object[1] { aintbatchrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busHealthPremiumReportBatchRequestGen.LoadPerson():
        /// Loads non-collection object ibusPerson of type busPerson.
        /// </summary>
		public virtual void LoadPerson()
		{
			if (ibusPerson == null)
			{
				ibusPerson = new busPerson();
			}
			ibusPerson.FindPerson(icdoHealthPremiumReportBatchRequest.perslink_id);
		}

        /// <summary>
        /// NeoSpin.busHealthPremiumReportBatchRequestGen.LoadOrganization():
        /// Loads non-collection object ibusOrganization of type busOrganization.
        /// </summary>
		public virtual void LoadOrganization()
		{
			if (ibusOrganization == null)
			{
				ibusOrganization = new busOrganization();
			}
			ibusOrganization.FindOrganization(icdoHealthPremiumReportBatchRequest.org_id);
		}

	}
}
