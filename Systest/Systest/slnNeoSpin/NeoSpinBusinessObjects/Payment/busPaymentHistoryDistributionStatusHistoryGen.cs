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
    /// Class NeoSpin.BusinessObjects.busPaymentHistoryDistributionStatusHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentHistoryDistributionStatusHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentHistoryDistributionStatusHistoryGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPaymentHistoryDistributionStatusHistoryGen
        /// </summary>
		public busPaymentHistoryDistributionStatusHistoryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentHistoryDistributionStatusHistoryGen.
        /// </summary>
		public cdoPaymentHistoryDistributionStatusHistory icdoPaymentHistoryDistributionStatusHistory { get; set; }




        /// <summary>
        /// NeoSpin.busPaymentHistoryDistributionStatusHistoryGen.FindPaymentHistoryDistributionStatusHistory():
        /// Finds a particular record from cdoPaymentHistoryDistributionStatusHistory with its primary key. 
        /// </summary>
        /// <param name="aintdistributionstatushistoryid">A primary key value of type int of cdoPaymentHistoryDistributionStatusHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentHistoryDistributionStatusHistory(int aintdistributionstatushistoryid)
		{
			bool lblnResult = false;
			if (icdoPaymentHistoryDistributionStatusHistory == null)
			{
				icdoPaymentHistoryDistributionStatusHistory = new cdoPaymentHistoryDistributionStatusHistory();
			}
			if (icdoPaymentHistoryDistributionStatusHistory.SelectRow(new object[1] { aintdistributionstatushistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
