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
    /// Class NeoSpin.BusinessObjects.busProviderReportPaymentGen:
    /// Inherited from busBase, used to create new business object for main table cdoProviderReportPayment and its children table. 
    /// </summary>
	[Serializable]
	public class busProviderReportPaymentGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busProviderReportPaymentGen
        /// </summary>
		public busProviderReportPaymentGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busProviderReportPaymentGen.
        /// </summary>
		public cdoProviderReportPayment icdoProviderReportPayment { get; set; }




        /// <summary>
        /// NeoSpin.BusinessObjects.busProviderReportPaymentGen.FindProviderReportPayment():
        /// Finds a particular record from cdoProviderReportPayment with its primary key. 
        /// </summary>
        /// <param name="aintproviderreportpaymentid">A primary key value of type int of cdoProviderReportPayment on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindProviderReportPayment(int aintproviderreportpaymentid)
		{
			bool lblnResult = false;
			if (icdoProviderReportPayment.IsNull())
			{
				icdoProviderReportPayment = new cdoProviderReportPayment();
			}
			if (icdoProviderReportPayment.SelectRow(new object[1] { aintproviderreportpaymentid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
