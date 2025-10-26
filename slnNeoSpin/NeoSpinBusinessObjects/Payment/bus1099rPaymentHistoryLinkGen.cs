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
    /// Class NeoSpin.BusinessObjects.bus1099rPaymentHistoryLinkGen:
    /// Inherited from busBase, used to create new business object for main table cdo1099rPaymentHistoryLink and its children table. 
    /// </summary>
	[Serializable]
	public class bus1099rPaymentHistoryLinkGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.bus1099rPaymentHistoryLinkGen
        /// </summary>
		public bus1099rPaymentHistoryLinkGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in bus1099rPaymentHistoryLinkGen.
        /// </summary>
		public cdo1099rPaymentHistoryLink icdo1099rPaymentHistoryLink { get; set; }




        /// <summary>
        /// NeoSpin.bus1099rPaymentHistoryLinkGen.Find1099rPaymentHistoryLink():
        /// Finds a particular record from cdo1099rPaymentHistoryLink with its primary key. 
        /// </summary>
        /// <param name="aintpayment1099rhistorylinkid">A primary key value of type int of cdo1099rPaymentHistoryLink on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool Find1099rPaymentHistoryLink(int aintpayment1099rhistorylinkid)
		{
			bool lblnResult = false;
			if (icdo1099rPaymentHistoryLink == null)
			{
				icdo1099rPaymentHistoryLink = new cdo1099rPaymentHistoryLink();
			}
			if (icdo1099rPaymentHistoryLink.SelectRow(new object[1] { aintpayment1099rhistorylinkid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
