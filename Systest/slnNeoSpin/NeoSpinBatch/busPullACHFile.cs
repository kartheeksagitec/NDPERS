#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;
#endregion

namespace NeoSpinBatch
{
    public class busPullACHFile : busNeoSpinBatch
    {
        public busPullACHFile()
        { }

        public void GenerateACHPullForBNDFileOut()
        {
            istrProcessName = "ACH Pull File for BND";

            DataTable ldtbDepositTape = busBase.Select("cdoDepositTape.LoadPullACHStatusReadyRecords", new object[] { });
            int lintPrevPersonID = 0, lintDepositID = 0;
            decimal ldecTotalDepositTapeAmount = 0.0M;

            if (ldtbDepositTape.Rows.Count > 0)
            {
                busDepositTape lobjDepositTape = new busDepositTape { icdoDepositTape = new cdoDepositTape() };
                lobjDepositTape.icdoDepositTape.LoadData(ldtbDepositTape.Rows[0]);

                // Select from IBS Detail table
                DataTable ldtbIBSDetail = busBase.Select("cdoIbsDetail.LoadACHIBSDetail", new object[0] { });
                busBase lobjBase = new busBase();
                Collection<busIbsDetail> lclbIbsDetail = lobjBase.GetCollection<busIbsDetail>(ldtbIBSDetail, "icdoIbsDetail");
                Collection<busACHProviderReportData> lclbACHPullFile = new Collection<busACHProviderReportData>();

                foreach (DataRow dr in ldtbIBSDetail.Rows)
                {
                    busIbsDetail lobjIbsDetail = new busIbsDetail { icdoIbsDetail = new cdoIbsDetail() };
                    lobjIbsDetail.icdoIbsDetail.LoadData(dr);
                    busPersonAccountAchDetail lobjACHDetail = new busPersonAccountAchDetail { icdoPersonAccountAchDetail = new cdoPersonAccountAchDetail() };
                    lobjACHDetail.icdoPersonAccountAchDetail.LoadData(dr);

                    if (lobjIbsDetail.icdoIbsDetail.deposit_id == 0)
                    {
                        //Loading person account to get organization details
                        if (lobjIbsDetail.ibusPerson == null)
                            lobjIbsDetail.LoadPerson();
                        busPersonAccount lobjPersonAccount = new busPersonAccount();
                        lobjPersonAccount = lobjIbsDetail.ibusPerson.LoadActivePersonAccountByPlan(lobjIbsDetail.icdoIbsDetail.plan_id);
                        lobjPersonAccount.icdoPersonAccount.person_employment_dtl_id =
                            lobjPersonAccount.GetEmploymentDetailID(lobjIbsDetail.icdoIbsDetail.created_date);
                        lobjPersonAccount.LoadPersonEmploymentDetail();
                        lobjPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();
                        lobjPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                        busDeposit lobjDeposit = new busDeposit { icdoDeposit = new cdoDeposit() };

                        if (lintPrevPersonID != lobjIbsDetail.icdoIbsDetail.person_id)
                        {
                            decimal ldecTotalPremium = 0.0M;
                            ldecTotalPremium = (from lobjDetail in lclbIbsDetail
                                                where lobjDetail.icdoIbsDetail.person_id == lobjIbsDetail.icdoIbsDetail.person_id
                                                select lobjDetail.icdoIbsDetail.total_premium_amount).Sum();
                            // Insert Deposits
                            lobjDeposit.icdoDeposit.deposit_tape_id = lobjDepositTape.icdoDepositTape.deposit_tape_id;
                            lobjDeposit.icdoDeposit.deposit_amount = ldecTotalPremium;
                            lobjDeposit.icdoDeposit.person_id = lobjIbsDetail.icdoIbsDetail.person_id;
                            lobjDeposit.icdoDeposit.payment_date = lobjIbsDetail.icdoIbsDetail.billing_month_and_year;
                            lobjDeposit.icdoDeposit.status_value = busConstant.DepositDetailStatusApplied;
                            lobjDeposit.icdoDeposit.deposit_source_value = busConstant.DepositSourceRegularDeposits;
                            lobjDeposit.icdoDeposit.deposit_date = lobjDepositTape.icdoDepositTape.deposit_tape_id != 0 ? lobjDepositTape.icdoDepositTape.deposit_date : DateTime.Today;
                            // TODO:Reference number not added yet.                            
                            lobjDeposit.icdoDeposit.Insert();

                            lintPrevPersonID = lobjIbsDetail.icdoIbsDetail.person_id;
                            lintDepositID = lobjDeposit.icdoDeposit.deposit_id;
                            ldecTotalDepositTapeAmount += ldecTotalPremium;
                        }
                        if (lintDepositID != 0)
                        {
                            //Insert Remittance
                            busRemittance lobjRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
                            lobjRemittance.icdoRemittance.deposit_id = lintDepositID;
                            lobjRemittance.icdoRemittance.person_id = lobjIbsDetail.icdoIbsDetail.person_id;
                            lobjRemittance.icdoRemittance.plan_id = lobjIbsDetail.icdoIbsDetail.plan_id;
                            lobjRemittance.icdoRemittance.remittance_amount = lobjIbsDetail.icdoIbsDetail.total_premium_amount;
                            lobjRemittance.icdoRemittance.remittance_type_value = busConstant.RemittanceTypeIBSDeposit;
                            lobjRemittance.icdoRemittance.org_id =
                                lobjPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_id;
                            lobjRemittance.icdoRemittance.applied_date = DateTime.Today; //uat pir 1756
                            lobjRemittance.icdoRemittance.Insert();

                            //Generate GL
                            //lobjDeposit.btnApply_Click();
                            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
                            lcdoAcccountReference.plan_id = lobjRemittance.icdoRemittance.plan_id;
                            lcdoAcccountReference.source_type_value = busConstant.SourceTypeRemittance;
                            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeStatusTransition;
                            lcdoAcccountReference.item_type_value = lobjRemittance.icdoRemittance.remittance_type_value;
                            lcdoAcccountReference.status_transition_value = busConstant.StatusTransitionValidatedToApplied;

                            busGLHelper.GenerateGL(lcdoAcccountReference,
                                                        lobjRemittance.icdoRemittance.person_id,
                                                        lobjRemittance.icdoRemittance.org_id,
                                                        lobjRemittance.icdoRemittance.remittance_id,
                                                        lobjRemittance.icdoRemittance.remittance_amount,
                                                        DateTime.Now,
                                                        lobjDepositTape.icdoDepositTape.deposit_date, iobjPassInfo);

                            //Updating Ibs Detail with the deposit id
                            lobjIbsDetail.icdoIbsDetail.deposit_id = lintDepositID;
                            lobjIbsDetail.icdoIbsDetail.Update();

                            // Add to ACH file collection
                            busACHProviderReportData lobjProviderReportData = new busACHProviderReportData();
                            lobjProviderReportData.lintPERSLinkID = lobjIbsDetail.icdoIbsDetail.person_id;
                            lobjProviderReportData.ldclContributionAmount = lobjIbsDetail.icdoIbsDetail.total_premium_amount;
                            lobjProviderReportData.lstrDFIAccountNo = lobjACHDetail.icdoPersonAccountAchDetail.bank_account_number;
                            lobjProviderReportData.lstrRoutingNumber = lobjACHDetail.icdoPersonAccountAchDetail.aba_number.ToString();
                            lobjProviderReportData.istrRoutingNumberFirstEightDigits = lobjProviderReportData.lstrRoutingNumber
                                .Substring(0, lobjProviderReportData.lstrRoutingNumber.Length - 1).PadLeft(8, '0');
                            lobjProviderReportData.istrCheckLastDigit = lobjProviderReportData.lstrRoutingNumber
                                                                                   .Substring(lobjProviderReportData.lstrRoutingNumber.Length - 1, 1);

                            if (lobjACHDetail.icdoPersonAccountAchDetail.bank_account_type_value == busConstant.PersonAccountBankAccountSavings)
                            {
                                lobjProviderReportData.lstrTransactionCode = lobjProviderReportData.ldclContributionAmount >= 0 ?
                                    busConstant.DebitTransactionCodeNonPrenoteSavings : busConstant.CreditTransactionCodeNonPrenoteSavings;
                            }
                            else if (lobjACHDetail.icdoPersonAccountAchDetail.bank_account_type_value == busConstant.PersonAccountBankAccountChecking)
                            {
                                lobjProviderReportData.lstrTransactionCode = lobjProviderReportData.ldclContributionAmount >= 0 ?
                                    busConstant.DebitTransactionCodePrenoteChecking : busConstant.CreditTransactionCodeNonPrenoteChecking;
                            }
                            //Commented since directly handling in the cdo query
                            //lobjProviderReportData.LoadDFIAccountNoByPERSLinkID(lobjIbsDetail.icdoIbsDetail.person_account_id);
                            //if (!string.IsNullOrEmpty(lobjProviderReportData.lstrDFIAccountNo))
                            //{                      
                            bool lblnIsRecordExists = false;
                            foreach (busACHProviderReportData lobjPrevRec in lclbACHPullFile.Where(o =>
                                o.lintPERSLinkID == lobjProviderReportData.lintPERSLinkID &&
                                   o.lstrDFIAccountNo == lobjProviderReportData.lstrDFIAccountNo &&
                                      o.lstrRoutingNumber == lobjProviderReportData.lstrRoutingNumber))
                            {
                                lobjPrevRec.ldclContributionAmount += lobjProviderReportData.ldclContributionAmount;
                                lblnIsRecordExists = true;
                            }
                            if (!lblnIsRecordExists)
                            {
                                lclbACHPullFile.Add(lobjProviderReportData);
                            }
                            //}                            
                        }
                    }
                }

                // Create an Outbound ACH File
                if (lclbACHPullFile.Count > 0)
                {
                    //Updating the status to Pull ACH complete in Deposit Tape
                    lobjDepositTape.icdoDepositTape.pull_ach_status_value = busConstant.PullACHCompleteStatus;
                    lobjDepositTape.icdoDepositTape.status_value = busConstant.DepositTapeStatusValid;
                    lobjDepositTape.icdoDepositTape.total_amount = ldecTotalDepositTapeAmount;
                    lobjDepositTape.icdoDepositTape.Update();

                    busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                    lobjProcessFiles.iarrParameters = new object[2];
                    lobjProcessFiles.iarrParameters[0] = lclbACHPullFile;
                    lobjProcessFiles.iarrParameters[1] = busConstant.Flag_Yes;
                    lobjProcessFiles.CreateOutboundFile(8);
                    idlgUpdateProcessLog("ACH Pull File for BND generated succesfully", "INFO", istrProcessName);
                }
                else
                {
                    idlgUpdateProcessLog("No File Generated", "INFO", istrProcessName);
                }

                DataTable ldtbPullACHError = busBase.Select("cdoIbsDetail.rptPullACHErrorReport", new object[0] { });
                if (ldtbPullACHError.Rows.Count > 0)
                {
                    CreateReport("rptPullACHError.rpt", ldtbPullACHError);
                    idlgUpdateProcessLog("Pull ACH Error Report Generated successfully", "INFO", istrProcessName);
                }
            }
        }
    }
}
