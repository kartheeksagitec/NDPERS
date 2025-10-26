#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Globalization;
using System.Collections;
using Sagitec.CorBuilder;
using System.Linq;
using System.Linq.Expressions;
#endregion
namespace NeoSpinBatch
{
    class busAllocateRemittanceToARBatch : busNeoSpinBatch
    {
        public busAllocateRemittanceToARBatch()
        {
        }
        private busIbsRemittanceAllocation _ibusIbsRemittanceAllocation;
        public busIbsRemittanceAllocation ibusIbsRemittanceAllocation
        {
            get
            {
                return _ibusIbsRemittanceAllocation;
            }
            set
            {
                _ibusIbsRemittanceAllocation = value;
            }
        }
        private Hashtable ihstPersonIDAmountDetails;
        public void AllocateIBSRemittance()
        {
            istrProcessName = "Allocate Remittance to A/R";
            idlgUpdateProcessLog("Loading Remittance for IBS Members", "INFO", istrProcessName);
            iobjPassInfo.BeginTransaction();
            busBase lobjBase = new busBase();
            DateTime ldtNextBenfitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            DataTable ldtbRemittance = busNeoSpinBase.Select("cdoIbsRemittanceAllocation.GetIBSRemittanceAllocationAmount",
                                                                new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtbPaymentElectionAdjustment = busNeoSpinBase.Select("cdoPaymentElectionAdjustment.LoadApprovedInPaymentRequests", new object[0] { });
            DataTable ldtbPAPIT = busNeoSpinBase.Select("cdoPaymentElectionAdjustment.LoadAllPAPIT", new object[0] { });
            List<int> llstPersonID = new List<int>();
            ihstPersonIDAmountDetails = new Hashtable();
            LoadLastPostedRegularIBSHeader();
            try
            {
                foreach (DataRow ldtrRemittance in ldtbRemittance.Rows)
                {
                    busRemittance lobjRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
                    lobjRemittance.icdoRemittance.LoadData(ldtrRemittance);
                    lobjRemittance.ibusDeposit = new busDeposit { icdoDeposit = new cdoDeposit() };
                    lobjRemittance.ibusDeposit.icdoDeposit.LoadData(ldtrRemittance);
					
					//PIR 15673
                    decimal ldecAvailableAmount = busEmployerReportHelper.GetRemittanceAvailableAmountForIBS(lobjRemittance.icdoRemittance.remittance_id);
                    if (ldecAvailableAmount > 0.00M)
                    {
                        decimal ldecTotalAllocatedAmount = 0;

                        idlgUpdateProcessLog("Loading the due contributions", "INFO", istrProcessName);
                        DataTable ldtbIBSDue = busNeoSpinBase.Select("cdoPersonAccountInsuranceContribution.LoadIBSAllDueContributions",
                                                                            new object[2] { lobjRemittance.icdoRemittance.person_id, 1 });
                        Collection<busPersonAccountInsuranceContribution> lclbIBSDue = new Collection<busPersonAccountInsuranceContribution>();
                        lclbIBSDue = GetCollectionIBSDue(ldtbIBSDue, 1);

                        if (PostAmountAndAllocateRemittance(lobjRemittance, ldecAvailableAmount, ref ldecTotalAllocatedAmount, lclbIBSDue, 1, lobjRemittance.icdoRemittance.person_id))
                            continue;

                        DataRow[] ldarrPaymentElection = null;
                        if (ldtbPaymentElectionAdjustment.IsNotNull() && ldtbPaymentElectionAdjustment.Rows.Count > 0)
                        {
							//PIR 16231 - Filtration by person account id is wrongly done (condition was satisfying every time)
                            ldarrPaymentElection = ldtbPaymentElectionAdjustment.FilterTable(busConstant.DataType.Numeric, "person_id",lobjRemittance.icdoRemittance.person_id);

                            if (ldarrPaymentElection.Length > 0)
                            {
                                ldtbIBSDue = busNeoSpinBase.Select("cdoPersonAccountInsuranceContribution.LoadIBSAllDueContributions",
                                                                                                          new object[2] { lobjRemittance.icdoRemittance.person_id, 2 });
                                lclbIBSDue = new Collection<busPersonAccountInsuranceContribution>();
                                lclbIBSDue = GetCollectionIBSDue(ldtbIBSDue, 2);
                                if (lclbIBSDue.Count > 0 && !ihstPersonIDAmountDetails.ContainsKey(lobjRemittance.icdoRemittance.person_id))
                                {
                                    llstPersonID.Add(lobjRemittance.icdoRemittance.person_id);
                                    ihstPersonIDAmountDetails.Add(lobjRemittance.icdoRemittance.person_id, 0.00);
                                }
                                if (PostAmountAndAllocateRemittance(lobjRemittance, ldecAvailableAmount, ref ldecTotalAllocatedAmount, lclbIBSDue, 2, lobjRemittance.icdoRemittance.person_id))
                                    continue;

                                ldtbIBSDue = busNeoSpinBase.Select("cdoPersonAccountInsuranceContribution.LoadIBSAllDueContributions",
                                                                               new object[2] { lobjRemittance.icdoRemittance.person_id, 3 });
                                lclbIBSDue = new Collection<busPersonAccountInsuranceContribution>();
                                lclbIBSDue = GetCollectionIBSDue(ldtbIBSDue, 3);
                                if (lclbIBSDue.Count > 0 && !ihstPersonIDAmountDetails.ContainsKey(lobjRemittance.icdoRemittance.person_id))
                                {
                                    llstPersonID.Add(lobjRemittance.icdoRemittance.person_id);
                                    ihstPersonIDAmountDetails.Add(lobjRemittance.icdoRemittance.person_id, 0.00);
                                }
                                if (PostAmountAndAllocateRemittance(lobjRemittance, ldecAvailableAmount, ref ldecTotalAllocatedAmount, lclbIBSDue, 3, lobjRemittance.icdoRemittance.person_id))
                                    continue;
                            }
                        }
                        //BR-28
                        DataTable ldtbEnrolledPlans = busNeoSpinBase.Select("cdoPersonAccount.LoadEnrolledPlan",
                                                                            new object[1] { lobjRemittance.icdoRemittance.person_id });
                        if (ldtbEnrolledPlans.Rows.Count == 0)
                            idlgUpdateProcessLog("PERSLink ID " + lobjRemittance.icdoRemittance.person_id.ToString() +
                                                 " is not enrolled in any Insurance Plans", "INFO", istrProcessName);
                    }
                    lobjRemittance = null;
                }
                DataTable ldtbRemainingIBSDue = new DataTable();
                idlgUpdateProcessLog("Updating PAPIT Amount and Payment election adjustment", "INFO", istrProcessName);
                int lintPersonAccountID = 0, lintProviderOrgID = 0;
                foreach (int lintPersonID in llstPersonID)
                {
                    ldtbRemainingIBSDue = busNeoSpinBase.Select("cdoPersonAccountInsuranceContribution.LoadIBSAllDueContributionsWithoutEffectiveDate",
                                                                            new object[1] { lintPersonID });
                    foreach (DataRow dr in ldtbRemainingIBSDue.Rows)
                    {
                        lintPersonAccountID = dr["person_account_id"] != DBNull.Value ? Convert.ToInt32(dr["person_account_id"]) : 0;
                        lintProviderOrgID = dr["provider_org_id"] != DBNull.Value ? Convert.ToInt32(dr["provider_org_id"]) : 0;
                        DataTable ldtPEAdj = ldtbPaymentElectionAdjustment.AsEnumerable()
                                                .Where(o => o.Field<int?>("person_account_id") == lintPersonAccountID &&
                                                    o.Field<int?>("provider_org_id") == lintProviderOrgID).AsDataTable();
                        if (dr["balance_amount"] != DBNull.Value && Convert.ToDecimal(dr["balance_amount"]) == 0)
                        {
                            foreach (DataRow ldrPE in ldtPEAdj.Rows)
                            {
                                busPaymentElectionAdjustment lobjPEAdj = new busPaymentElectionAdjustment { icdoPaymentElectionAdjustment = new cdoPaymentElectionAdjustment() };
                                lobjPEAdj.icdoPaymentElectionAdjustment.LoadData(ldrPE);
                                lobjPEAdj.icdoPaymentElectionAdjustment.status_value = busConstant.IBSAdjustmentStatusCompleted;
                                lobjPEAdj.icdoPaymentElectionAdjustment.Update();
                                if (lobjPEAdj.icdoPaymentElectionAdjustment.payee_account_id > 0)
                                {
                                    DataRow[] ldarrPAPIT = ldtbPAPIT.FilterTable(busConstant.DataType.Numeric, "payee_account_id", lobjPEAdj.icdoPaymentElectionAdjustment.payee_account_id);
                                    foreach(DataRow ldrPAPIT in ldarrPAPIT)
                                    {
                                        if (ldrPAPIT["vendor_org_id"] != DBNull.Value && Convert.ToInt32(ldrPAPIT["vendor_org_id"]) == lobjPEAdj.icdoPaymentElectionAdjustment.provider_org_id)
                                        {
                                            busPayeeAccountPaymentItemType lobjPAPIT = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                                            lobjPAPIT.icdoPayeeAccountPaymentItemType.LoadData(ldrPAPIT);
                                            if (lobjPAPIT.icdoPayeeAccountPaymentItemType.start_date >= ldtNextBenfitPaymentDate)
                                                lobjPAPIT.icdoPayeeAccountPaymentItemType.Delete();
                                            else
                                            {
                                                lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = ldtNextBenfitPaymentDate.AddDays(-1);
                                                lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                            }
                                            ldtbPAPIT.Rows.Remove(ldrPAPIT);
                                            ldtbPAPIT.AcceptChanges();
                                        }
                                    }
                                }
                            }
                        }
                        else if(dr["balance_amount"] != DBNull.Value && Convert.ToDecimal(dr["balance_amount"]) > 0)
                        {
                            foreach (DataRow ldrPE in ldtPEAdj.Rows)
                            {
                                busPaymentElectionAdjustment lobjPEAdj = new busPaymentElectionAdjustment { icdoPaymentElectionAdjustment = new cdoPaymentElectionAdjustment() };
                                lobjPEAdj.icdoPaymentElectionAdjustment.LoadData(ldrPE);
                                if (lobjPEAdj.icdoPaymentElectionAdjustment.status_value == busConstant.IBSAdjustmentStatusApproved)
                                {
                                    lobjPEAdj.icdoPaymentElectionAdjustment.status_value = busConstant.IBSAdjustmentStatusInPayment;
                                    lobjPEAdj.icdoPaymentElectionAdjustment.Update();
                                }
                                if (lobjPEAdj.icdoPaymentElectionAdjustment.payee_account_id > 0)
                                {
                                    DataRow[] ldarrPAPIT = ldtbPAPIT.FilterTable(busConstant.DataType.Numeric, "payee_account_id", lobjPEAdj.icdoPaymentElectionAdjustment.payee_account_id);
                                    foreach (DataRow ldrPAPIT in ldarrPAPIT)
                                    {
                                        if (ldrPAPIT["vendor_org_id"] != DBNull.Value && Convert.ToInt32(ldrPAPIT["vendor_org_id"]) == lobjPEAdj.icdoPaymentElectionAdjustment.provider_org_id)
                                        {
                                            busPayeeAccountPaymentItemType lobjPAPIT = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                                            lobjPAPIT.icdoPayeeAccountPaymentItemType.LoadData(ldrPAPIT);
                                            if (lobjPAPIT.icdoPayeeAccountPaymentItemType.amount > Convert.ToDecimal(dr["balance_amount"]))
                                            {
                                                if (lobjPAPIT.icdoPayeeAccountPaymentItemType.start_date >= ldtNextBenfitPaymentDate)
                                                {
                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.amount = Convert.ToDecimal(dr["balance_amount"]);
                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                                }
                                                else
                                                {
                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = ldtNextBenfitPaymentDate.AddDays(-1);
                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                                    CreateNewPAPIT(lobjPAPIT, ldtNextBenfitPaymentDate, Convert.ToDecimal(dr["balance_amount"]));
                                                }
                                                ldtbPAPIT.Rows.Remove(ldrPAPIT);
                                                ldtbPAPIT.AcceptChanges();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //As discussed with Satya & Ragavan, on 14th Feb 2011
                    //we have a known issue for installment payment payment election adjustment. 
                    //1) When a remittance amount comes which is greater than monthly amount
                    // and less than total due, we update the ibs summary adj amount with the difference amount. it wont hold good if there is a balance forward available
                    //In those cases we may need to manually update the balance forward column in ibs person summary
                    //2) For a member who is having installment payment, two checks come on two different days and each remittance is less than adjustmnet amount but
                    // put together greater than adjustment amount. But total remittance is less than total due, in this scenario also  we may need to fix the ibs person summary manually
                    decimal ldecAllocatedAmount = Convert.ToDecimal(ihstPersonIDAmountDetails[lintPersonID]);
                    DataRow[] ldarrPaymentElection = ldtbPaymentElectionAdjustment.FilterTable(busConstant.DataType.Numeric, "person_id", lintPersonID);
                    decimal ldecMonthlyAmount= 0.00M;
                    foreach (DataRow ldrPE in ldarrPaymentElection)
                    {
                        if (ldrPE["repayment_type_value"] != DBNull.Value && ldrPE["repayment_type_value"].ToString() != busConstant.IBSAdjustmentRepaymentTypeLumpSum)
                            ldecMonthlyAmount += ldrPE["monthly_amount"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldrPE["monthly_amount"]);
                    }
                    busIbsPersonSummary lobjIBSPersonSummary = new busIbsPersonSummary();
                    if (lobjIBSPersonSummary.FindIbsPersonSummaryByIbsHeaderAndPerson(ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id, lintPersonID) &&
                        ldecAllocatedAmount > 0 && ldecMonthlyAmount < ldecAllocatedAmount)
                    {
                        lobjIBSPersonSummary.icdoIbsPersonSummary.adjustment_amount += (ldecAllocatedAmount - ldecMonthlyAmount);
                        lobjIBSPersonSummary.icdoIbsPersonSummary.Update();
                    }
                    lobjIBSPersonSummary = null;
                }
                idlgUpdateProcessLog("Updated PAPIT Amount and Payment election adjustment Successfully", "INFO", istrProcessName);
                iobjPassInfo.Commit();
                idlgUpdateProcessLog("Remittance Allocation To A/R Process is Completed ", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                iobjPassInfo.Rollback();
                idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "INFO", istrProcessName);
            }
        }

        private void CreateNewPAPIT(busPayeeAccountPaymentItemType aobjPAPIT, DateTime adtNextBenfitPaymentDate, decimal adecAmount)
        {
            busPayeeAccountPaymentItemType lobjNewPAPIT = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            lobjNewPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id = aobjPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id;
            lobjNewPAPIT.icdoPayeeAccountPaymentItemType.account_number = aobjPAPIT.icdoPayeeAccountPaymentItemType.account_number;
            lobjNewPAPIT.icdoPayeeAccountPaymentItemType.amount = adecAmount;
            lobjNewPAPIT.icdoPayeeAccountPaymentItemType.batch_schedule_id = aobjPAPIT.icdoPayeeAccountPaymentItemType.batch_schedule_id;
            lobjNewPAPIT.icdoPayeeAccountPaymentItemType.payee_account_id = aobjPAPIT.icdoPayeeAccountPaymentItemType.payee_account_id;
            lobjNewPAPIT.icdoPayeeAccountPaymentItemType.start_date = adtNextBenfitPaymentDate;
            lobjNewPAPIT.icdoPayeeAccountPaymentItemType.vendor_org_id = aobjPAPIT.icdoPayeeAccountPaymentItemType.vendor_org_id;
            lobjNewPAPIT.icdoPayeeAccountPaymentItemType.reissue_item_flag = aobjPAPIT.icdoPayeeAccountPaymentItemType.reissue_item_flag;
            lobjNewPAPIT.icdoPayeeAccountPaymentItemType.miscellaneous_correction_flag = aobjPAPIT.icdoPayeeAccountPaymentItemType.miscellaneous_correction_flag;
            lobjNewPAPIT.icdoPayeeAccountPaymentItemType.Insert();
        }

        private Collection<busPersonAccountInsuranceContribution> GetCollectionIBSDue(DataTable adtbIBSDue, int aintRecOrder)
        {
            DataTable ldtFilteredIBSDue = new DataTable();
            Collection<busPersonAccountInsuranceContribution> lclbIBSDue = new Collection<busPersonAccountInsuranceContribution>();
            DataRow[] ldarrIBSDue = adtbIBSDue.FilterTable(busConstant.DataType.Numeric, "rec_order", aintRecOrder);
            foreach (DataRow ldrIBSDue in ldarrIBSDue)
            {
                busPersonAccountInsuranceContribution lobjPAIC = new busPersonAccountInsuranceContribution 
                { icdoPersonAccountInsuranceContribution = new cdoPersonAccountInsuranceContribution() };
                lobjPAIC.icdoPersonAccountInsuranceContribution.LoadData(ldrIBSDue);
                lclbIBSDue.Add(lobjPAIC);
            }
            return lclbIBSDue;
        }

        private bool PostAmountAndAllocateRemittance(busRemittance aobjRemittance, decimal adecAvailableAmount, ref decimal adecTotalAllocatedAmount, 
            Collection<busPersonAccountInsuranceContribution> aclbIBSDue, int aintRecordOrder, int aintPersonID)
        {
            string lstrTransactionType;
            foreach (busPersonAccountInsuranceContribution lobjContribution in aclbIBSDue)
            {
                lstrTransactionType = string.Empty;
                //Post Available Amount
                decimal ldecAllocatedAmount = adecAvailableAmount - adecTotalAllocatedAmount;
                if (ldecAllocatedAmount > lobjContribution.icdoPersonAccountInsuranceContribution.balance_amount)
                    ldecAllocatedAmount = lobjContribution.icdoPersonAccountInsuranceContribution.balance_amount;
                if (ldecAllocatedAmount > 0.00M)
                {
                    //ucs - 038  addendum
                    if (lobjContribution.icdoPersonAccountInsuranceContribution.rec_order == "1")
                    {
                        lstrTransactionType = busConstant.TransactionTypeIBSPayment;
                    }
                    else
                    {
                        lstrTransactionType = busConstant.PersonAccountTransactionTypeIBSAdjPayment;
                    }
                    PostPaidAmount(lobjContribution.icdoPersonAccountInsuranceContribution.person_account_id, ldecAllocatedAmount,
                                aobjRemittance.icdoRemittance.remittance_id, lobjContribution.icdoPersonAccountInsuranceContribution.effective_date,
                                aobjRemittance.ibusDeposit.icdoDeposit.payment_history_header_id, lstrTransactionType,
                                lobjContribution.icdoPersonAccountInsuranceContribution.provider_org_id);

                    AllocateRemittance(ldecAllocatedAmount, aobjRemittance.icdoRemittance.remittance_id,
                        lobjContribution.icdoPersonAccountInsuranceContribution.person_account_id,
                        lobjContribution.icdoPersonAccountInsuranceContribution.effective_date);

                    adecTotalAllocatedAmount += ldecAllocatedAmount;
                    if (aintRecordOrder == 3)
                        ihstPersonIDAmountDetails[aintPersonID] = Convert.ToDecimal(ihstPersonIDAmountDetails[aintPersonID]) + ldecAllocatedAmount;
                }
                if (adecTotalAllocatedAmount == adecAvailableAmount) return true;
            }
            return false;
        }

        private void PostPaidAmount(int aintPersonAccountID, decimal adecPaidAmount, int aintRemittanceID, DateTime adtEffectiveDate, int aintPaymentHistoryHeaderID,
            string astrTransactionType, int aintProviderOrgID)
        {
            idlgUpdateProcessLog("Posting Paid Amount into Contributions", "INFO", istrProcessName);
            cdoPersonAccountInsuranceContribution lobjInsrContribution = new cdoPersonAccountInsuranceContribution();
            lobjInsrContribution.person_account_id = aintPersonAccountID;
            lobjInsrContribution.paid_premium_amount = adecPaidAmount;
            lobjInsrContribution.subsystem_ref_id = aintRemittanceID;
            lobjInsrContribution.subsystem_value = busConstant.SubSystemValueIBSPayment;
            lobjInsrContribution.transaction_date = DateTime.Today;
            lobjInsrContribution.transaction_type_value = astrTransactionType;
            lobjInsrContribution.effective_date = adtEffectiveDate;
            lobjInsrContribution.payment_history_header_id = aintPaymentHistoryHeaderID;
            lobjInsrContribution.provider_org_id = aintProviderOrgID;
            lobjInsrContribution.Insert();
            idlgUpdateProcessLog("Paid Amount Posted", "INFO", istrProcessName);
        }
        private void AllocateRemittance(decimal adecTotalAmountTobeAllocated, int lintRemittanceID, int aintPersonAccountID,
            DateTime adtEffectiveDate)
        {
            idlgUpdateProcessLog("Inserting an Allocation Entry in IBS Remittance Allocation", "INFO", istrProcessName);
            if (_ibusIbsRemittanceAllocation == null)
            {
                ibusIbsRemittanceAllocation = new busIbsRemittanceAllocation();
                ibusIbsRemittanceAllocation.icdoIbsRemittanceAllocation = new cdoIbsRemittanceAllocation();
            }
            ibusIbsRemittanceAllocation.icdoIbsRemittanceAllocation.allocated_amount = adecTotalAmountTobeAllocated;
            ibusIbsRemittanceAllocation.icdoIbsRemittanceAllocation.remittance_id = lintRemittanceID;
            ibusIbsRemittanceAllocation.icdoIbsRemittanceAllocation.person_account_id = aintPersonAccountID;
            ibusIbsRemittanceAllocation.icdoIbsRemittanceAllocation.effective_date = adtEffectiveDate;
            ibusIbsRemittanceAllocation.icdoIbsRemittanceAllocation.ibs_allocation_status_value = busConstant.IBSAllocationStatusAllocated;
            ibusIbsRemittanceAllocation.icdoIbsRemittanceAllocation.Insert();

            idlgUpdateProcessLog("Inserted", "INFO", istrProcessName);
        }

        public busIbsHeader ibusLastPostedRegularIBSHeader { get; set; }
        public void LoadLastPostedRegularIBSHeader()
        {
            ibusLastPostedRegularIBSHeader = new busIbsHeader { icdoIbsHeader = new cdoIbsHeader() };
            DataTable ldtbList = busNeoSpinBase.SelectWithOperator<cdoIbsHeader>(
              new string[2] { "report_type_value", "report_status_value" },
               new string[2] { "=", "=" },
              new object[2] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted }, "billing_month_and_year desc");
            if (ldtbList.Rows.Count > 0)
            {
                ibusLastPostedRegularIBSHeader.icdoIbsHeader.LoadData(ldtbList.Rows[0]);
            }
        }
    }
}