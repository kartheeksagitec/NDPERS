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
    /// Class NeoSpin.BusinessObjects.busWssDebitAchRequestDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssDebitAchRequestDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busWssDebitAchRequestDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssDebitAchRequestDetailGen
        /// </summary>
		public busWssDebitAchRequestDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssDebitAchRequestDetailGen.
        /// </summary>
		public cdoWssDebitAchRequestDetail icdoWssDebitAchRequestDetail { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busWssDebitAchRequest.
        /// </summary>
		public busWssDebitAchRequest ibusWssDebitAchRequest { get; set; }




        /// <summary>
        /// NeoSpin.busWssDebitAchRequestDetailGen.FindWssDebitAchRequestDetail():
        /// Finds a particular record from cdoWssDebitAchRequestDetail with its primary key. 
        /// </summary>
        /// <param name="aintdebitachrequestdtlid">A primary key value of type int of cdoWssDebitAchRequestDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssDebitAchRequestDetail(int aintdebitachrequestdtlid)
		{
			bool lblnResult = false;
			if (icdoWssDebitAchRequestDetail == null)
			{
				icdoWssDebitAchRequestDetail = new cdoWssDebitAchRequestDetail();
			}
			if (icdoWssDebitAchRequestDetail.SelectRow(new object[1] { aintdebitachrequestdtlid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busWssDebitAchRequestDetailGen.LoadWssDebitAchRequest():
        /// Loads non-collection object ibusWssDebitAchRequest of type busWssDebitAchRequest.
        /// </summary>
		public virtual void LoadWssDebitAchRequest()
		{
			if (ibusWssDebitAchRequest == null)
			{
				ibusWssDebitAchRequest = new busWssDebitAchRequest();
			}
			ibusWssDebitAchRequest.FindWssDebitAchRequest(icdoWssDebitAchRequestDetail.debit_ach_request_id);
		}

	}
}
