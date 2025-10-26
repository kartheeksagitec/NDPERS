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
    /// Class NeoSpin.BusinessObjects.busIbsCheckEntryDetail:
    /// Inherited from busIbsCheckEntryDetailGen, the class is used to customize the business object busIbsCheckEntryDetailGen.
    /// </summary>
    [Serializable]
    public class busIbsCheckEntryDetail : busIbsCheckEntryDetailGen
    {
        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            if (icdoIbsCheckEntryDetail.person_id != 0)
            {
                LoadPerson();
            }
            decimal ldecAmountDue = 0.0m;
            ldecAmountDue = GetDueAmount();
            icdoIbsCheckEntryDetail.amount_paid = ldecAmountDue;
            icdoIbsCheckEntryDetail.due_amount = ldecAmountDue;
            larrList.Add(this);
            return larrList;
        }

        public decimal GetDueAmount()
        {
            decimal ldecAmountDue = 0.0m;
            if (ibusPerson == null)
                LoadPerson();
            ibusPerson.LoadIBSMemberSummary();
            DataTable ldtbAdjustments = Select("cdoIbsDetail.GetAdjustmentPayments", new object[1] { icdoIbsCheckEntryDetail.person_id });
            DataTable ldtbPayments = Select("cdoRemittance.GetPayments", new object[1] { icdoIbsCheckEntryDetail.person_id });
            decimal idecMemberSummaryNetPremiumAmt = ibusPerson.idecMemberSummaryNetPremiumAmt;
            decimal idecTotalAdjustments = ldtbAdjustments.Rows.Count > 0 ? Convert.ToDecimal(ldtbAdjustments.Rows[0]["idecTotalAdjustments"]) : 0.0m;
            decimal idecPaymentsMade = ldtbPayments.Rows.Count > 0 ? Convert.ToDecimal(ldtbPayments.Rows[0]["PAYMENTS_MADE"]) : 0.0m;
            ldecAmountDue = (idecMemberSummaryNetPremiumAmt + idecTotalAdjustments + ibusPerson.idecMemberSummaryBalanceForward) - idecPaymentsMade;
            return ldecAmountDue;
        }
    // If person is not enrolled in insurance plans with individual billing ,throw a error
        public bool IsPersonNotEnrolledinIBS()
        {
            bool lblnIsPersonEnrolledinIBS=false;
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.iclbInsuranceAccounts == null)
                ibusPerson.LoadInsuranceAccounts();
            foreach (busPersonAccount lbusPersonAccount in ibusPerson.iclbInsuranceAccounts)
            {
                lbusPersonAccount.LoadPaymentElection();
                if (lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0)
                {
                    lblnIsPersonEnrolledinIBS = true;
                    break;
                }
            }
            if (!lblnIsPersonEnrolledinIBS)
                return true;
            else
                return false;
        }
        public override void BeforePersistChanges()
        {
            // if reference number is not entered ,get the Last IBS header id and post
            if (string.IsNullOrEmpty(icdoIbsCheckEntryDetail.reference_no))
            {
                DataTable ldtbIBSHeader = Select<cdoIbsHeader>(new string[1] { "report_type_value" },
                    new object[1] { busConstant.IBSHeaderReportTypeRegular }, null, "BILLING_MONTH_AND_YEAR desc");
                if (ldtbIBSHeader.Rows.Count > 0)
                    icdoIbsCheckEntryDetail.reference_no = ldtbIBSHeader.Rows[0]["ibs_header_id"].ToString();
            }
            //PIR 6617
            if (icdoIbsCheckEntryDetail.person_id != 0)
            {
                LoadPerson();
            }
            decimal ldecAmountDue = 0.0m;
            ldecAmountDue = GetDueAmount();
            icdoIbsCheckEntryDetail.due_amount = ldecAmountDue;
            base.BeforePersistChanges();
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            //ibusIbsCheckEntryHeader.LoadIbsCheckEntryDetail();
        }
        
        ////PIR 6617
        //public cdoIbsCheckEntryHeader icdoIbsCheckEntryHeader { get; set; }
        //public Collection<busIbsCheckEntryDetail> iclbIbsCheckEntryDetailFromQuery { get; set; }
        //        public void DisplayIbsCheckEntryDetail()
        //{
        //    if (icdoIbsCheckEntryHeader == null)
        //        icdoIbsCheckEntryHeader = new cdoIbsCheckEntryHeader();
        //    DataTable ldtbCheckEnrtryDetail = Select("cdoIbsCheckEntryDetail.CheckDetails",
        //        new object[1] { icdoIbsCheckEntryDetail.ibs_check_entry_header_id });
        //    iclbIbsCheckEntryDetailFromQuery = GetCollection<busIbsCheckEntryDetail>(ldtbCheckEnrtryDetail, "icdoIbsCheckEntryDetail");

        //}
    }
}