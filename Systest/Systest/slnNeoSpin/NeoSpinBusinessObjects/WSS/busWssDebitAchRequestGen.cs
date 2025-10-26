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
    /// Class NeoSpin.BusinessObjects.busWssDebitAchRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssDebitAchRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busWssDebitAchRequestGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssDebitAchRequestGen
        /// </summary>
		public busWssDebitAchRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssDebitAchRequestGen.
        /// </summary>
		public cdoWssDebitAchRequest icdoWssDebitAchRequest { get; set; }




        /// <summary>
        /// NeoSpin.busWssDebitAchRequestGen.FindWssDebitAchRequest():
        /// Finds a particular record from cdoWssDebitAchRequest with its primary key. 
        /// </summary>
        /// <param name="aintdebitachrequestid">A primary key value of type int of cdoWssDebitAchRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssDebitAchRequest(int aintdebitachrequestid)
		{
			bool lblnResult = false;
			if (icdoWssDebitAchRequest == null)
			{
				icdoWssDebitAchRequest = new cdoWssDebitAchRequest();
			}
			if (icdoWssDebitAchRequest.SelectRow(new object[1] { aintdebitachrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
