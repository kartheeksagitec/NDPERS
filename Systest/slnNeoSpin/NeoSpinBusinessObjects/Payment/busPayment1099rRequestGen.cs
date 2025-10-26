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
    /// Class NeoSpin.BusinessObjects.busPayment1099rRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayment1099rRequest and its children table. 
    /// </summary>
	[Serializable]
	public class busPayment1099rRequestGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPayment1099rRequestGen
        /// </summary>
		public busPayment1099rRequestGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayment1099rRequestGen.
        /// </summary>
		public cdoPayment1099rRequest icdoPayment1099rRequest { get; set; }




        /// <summary>
        /// NeoSpin.busPayment1099rRequestGen.FindPayment1099rRequest():
        /// Finds a particular record from cdoPayment1099rRequest with its primary key. 
        /// </summary>
        /// <param name="aintrequestid">A primary key value of type int of cdoPayment1099rRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayment1099rRequest(int aintrequestid)
		{
			bool lblnResult = false;
			if (icdoPayment1099rRequest == null)
			{
				icdoPayment1099rRequest = new cdoPayment1099rRequest();
			}
			if (icdoPayment1099rRequest.SelectRow(new object[1] { aintrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
