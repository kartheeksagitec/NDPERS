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
	/// Class NeoSpin.BusinessObjects.busProviderReportPayment:
	/// Inherited from busProviderReportPaymentGen, the class is used to customize the business object busProviderReportPaymentGen.
	/// </summary>
	[Serializable]
	public class busProviderReportPayment : busProviderReportPaymentGen
	{
        //property to load vendor details
        public busOrganization ibusVendor { get; set; }

        //Load Vendor Details

        public void LoadVendor()
        {
            if (ibusVendor == null)
                ibusVendor = new busOrganization();
            ibusVendor.FindOrganization(icdoProviderReportPayment.provider_org_id);
        }
        public busProviderReportDataBatchRequest ibusProviderReportDataBatchRequest { get; set; }

        /// <summary>
        /// Method to create a new Provider Report payment
        /// </summary>
        /// <param name="astrSubsystemValue">subsystem value</param>
        /// <param name="aintSubsystemRefID">subsystem ref id</param>
        /// <param name="aintPersonID">Person id</param>
        /// <param name="aintOrgID">Provider org id</param>
        /// <param name="aintPayeeAccountID">payee account id</param>
        /// <param name="adtEffectiveDate">effective date</param>
        /// <param name="adecAmount">amount</param>
        /// <param name="aintBatchRequestID">batch request id</param>
        public void CreateProviderReportPayment(string astrSubsystemValue, int aintSubsystemRefID, int aintPersonID, int aintOrgID, int aintPayeeAccountID,
            DateTime adtEffectiveDate, decimal adecAmount, int aintBatchRequestID, int aintPaymentItemTypeID)
        {
            icdoProviderReportPayment.subsystem_value = astrSubsystemValue;
            icdoProviderReportPayment.subsystem_ref_id = aintSubsystemRefID;
            icdoProviderReportPayment.person_id = aintPersonID;
            icdoProviderReportPayment.provider_org_id = aintOrgID;
            icdoProviderReportPayment.payee_account_id = aintPayeeAccountID;
            icdoProviderReportPayment.effective_date = adtEffectiveDate;
            icdoProviderReportPayment.amount = adecAmount;
            icdoProviderReportPayment.batch_request_id = aintBatchRequestID;
            icdoProviderReportPayment.payment_item_type_id = aintPaymentItemTypeID;
            icdoProviderReportPayment.Insert();
        }

	}
}
