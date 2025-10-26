using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.DBUtility;
using NeoSpin.DataObjects;
using System.Linq;
using System.Linq.Expressions;

namespace NeoSpin.BusinessObjects
{
    public static class busGLHelper
    {

        /// <summary>
        /// Function to Generate GL 
        /// </summary>		
        /// <param name="acdoAccountReference"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aintOrgId"></param>
        /// <param name="aintSourceId"></param>
        /// <param name="adecAmount"></param>
        /// <param name="adtEffectiveDate"></param>
        /// <param name="adtPostingDate"></param>
        /// <returns>ArrayList</returns>
        public static ArrayList GenerateGL(cdoAccountReference acdoAccountReference, int aintPersonId, int aintOrgId,
                                           int aintSourceId, decimal adecAmount, DateTime adtEffectiveDate,
                                           DateTime adtPostingDate, utlPassInfo aobjPassInfo, int aintOrgIdEmpRep = 0,
                                           decimal adecDebitAmount = 0.0M, decimal adecCreditAmount = 0.0M,
                                           bool ablnAreCreditAmountDebitAmountDfrt = false, bool ablnNoGlCredEntry = false,
                                           bool ablnIsNDPOrgIdDebitEntry = false, bool ablnIsNDPOrgIdCreditEntryTransfer = false,
                                           bool ablnIsGLFromPaymentCanceled = false, bool ablnIsTaxItemDebActChanged = false, bool ablnIsTFFRCanceled = false)
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            busBase lobjBase = new busBase();
            Collection<busAccountReference> lclbAccountReference;
            DataTable ldtbAccountReference;

            if (acdoAccountReference == null)
            {
                lobjError.istrErrorID = "0";
                lobjError.istrErrorMessage = "Invalid Parameters!";
                larrList.Add(lobjError);
                return larrList;
            }

            if (acdoAccountReference.plan_id == 0)
            {
                lobjError.istrErrorID = "0";
                lobjError.istrErrorMessage = "Invalid Plan!";
                larrList.Add(lobjError);
                return larrList;
            }

            if (String.IsNullOrEmpty(acdoAccountReference.source_type_value))
            {
                lobjError.istrErrorID = "0";
                lobjError.istrErrorMessage = "Invalid Source Type!";
                larrList.Add(lobjError);
                return larrList;
            }

            /*****************************************************************************
             * Transaction Type = Status Transition 
             ****************************************************************************/
            if (acdoAccountReference.transaction_type_value == busConstant.TransactionTypeStatusTransition)
            {
                //If Status Transition Value is invalid
                if (String.IsNullOrEmpty(acdoAccountReference.status_transition_value))
                {
                    lobjError.istrErrorID = "0";
                    lobjError.istrErrorMessage = "Invalid Status Transition!";
                    larrList.Add(lobjError);
                    return larrList;
                }

                //If Item Type is invalid
                if (String.IsNullOrEmpty(acdoAccountReference.item_type_value))
                {
                    lobjError.istrErrorID = "0";
                    lobjError.istrErrorMessage = "Invalid Item Type!";
                    larrList.Add(lobjError);
                    return larrList;
                }

                //Get the Account Reference Object By Given Values
                //It should not exceed more than one. else throws an error.

                ldtbAccountReference =
                     busBase.Select<cdoAccountReference>(
                         new string[5]
                            {
                                "item_type_value", "transaction_type_value", "source_type_value", "plan_id",
                                "status_transition_value"
                            },
                         new object[5]
                            {
                                acdoAccountReference.item_type_value, acdoAccountReference.transaction_type_value,
                                acdoAccountReference.source_type_value, acdoAccountReference.plan_id,
                                acdoAccountReference.status_transition_value
                            }, null, null);
                lclbAccountReference = lobjBase.GetCollection<busAccountReference>(ldtbAccountReference,
                                                                                   "icdoAccountReference");

                //Validate the Account Reference (If No Records or Multiple Records found for the Given Combination)
                larrList = ValidateAccountReference(lclbAccountReference);
                if (larrList.Count > 0) return larrList;

                //Generate the GL only if the Flag is true
                AddGLTransaction(lclbAccountReference[0].icdoAccountReference, aintPersonId, aintOrgId, aintSourceId,
                    adecAmount, adtEffectiveDate, adtPostingDate, 0, adecDebitAmount,
                    adecCreditAmount, ablnAreCreditAmountDebitAmountDfrt, ablnNoCreditEntry: ablnNoGlCredEntry, ablnIsNDOrgForDebit: ablnIsNDPOrgIdDebitEntry,
                    ablnIsTaxItemDebitActChanged : ablnIsTaxItemDebActChanged, ablnIsTFFRPaymentCancel : ablnIsTFFRCanceled);

                //Generate the Transfer GL for Status Transition
                ldtbAccountReference =
                   busBase.Select<cdoAccountReference>(
                       new string[5]
                            {
                                "item_type_value", "transaction_type_value", "source_type_value", "plan_id",
                                "status_transition_value"
                            },
                       new object[5]
                            {
                                acdoAccountReference.item_type_value, busConstant.TransactionTypeTransfer,
                                acdoAccountReference.source_type_value, acdoAccountReference.plan_id,
                                acdoAccountReference.status_transition_value
                            }, null, null);
                lclbAccountReference = lobjBase.GetCollection<busAccountReference>(ldtbAccountReference,
                                                                                   "icdoAccountReference");
                bool lblnTempFlag = false;
                if (ablnIsGLFromPaymentCanceled && ablnIsNDPOrgIdCreditEntryTransfer)
                    lblnTempFlag = true;
                foreach (busAccountReference lobjAccountReference in lclbAccountReference)
                {
                    if (ablnIsGLFromPaymentCanceled && lobjAccountReference.icdoAccountReference.debit_account_id == busConstant.CanceledPremiumReceivableAccount)
                        ablnIsNDPOrgIdCreditEntryTransfer = false;
                    else if (ablnIsGLFromPaymentCanceled && lblnTempFlag)
                        ablnIsNDPOrgIdCreditEntryTransfer = true;
                    AddGLTransaction(lobjAccountReference.icdoAccountReference, aintPersonId, aintOrgId,
                        aintSourceId, adecAmount, adtEffectiveDate, adtPostingDate, ablnIsNDOrgForCreditTran: ablnIsNDPOrgIdCreditEntryTransfer, ablnIsGLEntryFromPaymentCanceled: ablnIsGLFromPaymentCanceled);
                }

                //If Status Transtion is Applied To NSF, Or Invalidated, Verify with Employer Reporting Remittance Allocation
                //and sets the status balancing status to unbalanced.
                if ((acdoAccountReference.status_transition_value == busConstant.StatusTransitionAppliedToNSF) ||
                    (acdoAccountReference.status_transition_value == busConstant.StatusTransitionAppliedToInvalidated))
                {
                    /* Get the Collection of Employer Payroll Header which are having this Remittance ID (aintSourceId) in allocated status
                     * and update the balancing status to unbalanced*/
                    busEmployerPayrollHeader lobjEmployerPayrollHeader = new busEmployerPayrollHeader();
                    DataTable ldtbGetPayrollHeaderByRemittanceID = busBase.Select("cdoEmployerRemittanceAllocation.LoadHeaderByRemittanceID", new object[1] { aintSourceId });
                    Collection<busEmployerPayrollHeader> lclbEmployerPayroll = new Collection<busEmployerPayrollHeader>();
                    lclbEmployerPayroll = lobjEmployerPayrollHeader.GetCollection<busEmployerPayrollHeader>(ldtbGetPayrollHeaderByRemittanceID, "icdoEmployerPayrollHeader");


                    foreach (busEmployerPayrollHeader lobjEmployerPayroll in lclbEmployerPayroll)
                    {
                        //Delete the remittance allocation having remittance with deposit status as (NSF or Invalidated)
                        DBFunction.DBNonQuery("cdoEmployerRemittanceAllocation.DeleteRemittanceAllocationWithDepositStatusNSFOrInValid",
                                 new object[2] { lobjEmployerPayroll.icdoEmployerPayrollHeader.employer_payroll_header_id, aintSourceId },
                                  aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);

                        // Get count of remittance allocation with status as Allocated
                        //if Count is greater than 0 then set balancing status as Unbalanced
                        //else set as NoREmittance
                        int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoEmployerRemittanceAllocation.GetCountOfRemittanceWithAlocSatus",
                                                 new object[1] { lobjEmployerPayroll.icdoEmployerPayrollHeader.employer_payroll_header_id },
                                                 aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework));
                        if (lintCount > 0)
                        {
                            lobjEmployerPayroll.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusUnbalanced;
                        }
                        else
                        {
                            lobjEmployerPayroll.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;
                        }
                        if ((lobjEmployerPayroll.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp) &&
                            (lobjEmployerPayroll.icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusReadyToPost))
                        {
                            if ((lobjEmployerPayroll.icdoEmployerPayrollHeader.ignore_balancing_status_flag != busConstant.Flag_Yes) &&
                                (lobjEmployerPayroll.icdoEmployerPayrollHeader.balancing_status_value != busConstant.PayrollHeaderBalancingStatusBalanced))
                            {
                                lobjEmployerPayroll.icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusValid;
                            }
                        }
                        lobjEmployerPayroll.icdoEmployerPayrollHeader.Update();
                    }

                }

            } // End of Transaction Type = Status Transition 

            /*****************************************************************************
            * Transaction Type = Item Type (Employer Reporting - Regular , +ADJ, Bonus) 
            ****************************************************************************/
            if (acdoAccountReference.transaction_type_value == busConstant.TransactionTypeItemLevel)
            {
                //If Item Type is invalid
                if (String.IsNullOrEmpty(acdoAccountReference.item_type_value))
                {
                    lobjError.istrErrorID = "0";
                    lobjError.istrErrorMessage = "Invalid Item Type!";
                    larrList.Add(lobjError);
                    return larrList;
                }

                //Get the Account Reference Object By Given Values
                //It should not exceed more than one. else throws an error.                
                ldtbAccountReference =
                     busBase.Select<cdoAccountReference>(
                         new string[4]
                            {
                                "item_type_value", "transaction_type_value", "source_type_value", "plan_id"                                
                            },
                         new object[4]
                            {
                                acdoAccountReference.item_type_value, acdoAccountReference.transaction_type_value,
                                acdoAccountReference.source_type_value, acdoAccountReference.plan_id
                            }, null, null);

                lclbAccountReference = lobjBase.GetCollection<busAccountReference>(ldtbAccountReference,
                                                                                   "icdoAccountReference");
                //PIR 23999 additional filter status transaction for approved or cancel transaction
                if (acdoAccountReference.item_type_value == busConstant.ItemTypePenalty)
                {
                    lclbAccountReference = lclbAccountReference?.Where(x => x.icdoAccountReference.status_transition_value == acdoAccountReference.status_transition_value).ToList().ToCollection();
                }
                //Validate the Account Reference (If No Records or Multiple Records found for the Given Combination)
                larrList = ValidateAccountReference(lclbAccountReference);
                if (larrList.Count > 0) return larrList;

                //Generate the GL only if the Flag is true
                AddGLTransaction(lclbAccountReference[0].icdoAccountReference, aintPersonId, aintOrgId, aintSourceId, adecAmount, adtEffectiveDate, adtPostingDate);
            } //End of If Transaction Type == Item Level

            /*****************************************************************************
            * Transaction Type = Allocation (Employer Reporting - Negative ADJ , IBS - JSRHIC Reverse Allocation, IBS - Negative Adjustment, Service Purchase) 
            ****************************************************************************/
            if (acdoAccountReference.transaction_type_value == busConstant.TransactionTypeAllocation)
            {
                //If From Item Type is invalid
                if (String.IsNullOrEmpty(acdoAccountReference.from_item_type_value))
                {
                    lobjError.istrErrorID = "0";
                    lobjError.istrErrorMessage = "Invalid From Item Type!";
                    larrList.Add(lobjError);
                    return larrList;
                }

                if (!String.IsNullOrEmpty(acdoAccountReference.to_item_type_value))
                {
                    //Null Handling Error
                    if (acdoAccountReference.status_transition_value == null)
                        acdoAccountReference.status_transition_value = String.Empty;

                    //Get the Account Reference Object By Given Values
                    //It should not exceed more than one. else throws an error. 

                    ldtbAccountReference =
                        busBase.Select("cdoAccountReference.GetValidAccountReferenceForFromTypeToType",
                                new object[6]
                            {
                                acdoAccountReference.transaction_type_value,acdoAccountReference.source_type_value,
                                acdoAccountReference.from_item_type_value,acdoAccountReference.to_item_type_value, 
                                acdoAccountReference.plan_id,acdoAccountReference.status_transition_value
                            });
                }
                else
                {
                    //Get the Account Reference Object By Given Values
                    //It should not exceed more than one. else throws an error.                
                    ldtbAccountReference =
                         busBase.Select<cdoAccountReference>(
                             new string[4]
                            {
                                "from_item_type_value", "transaction_type_value", "source_type_value", "plan_id"                                
                            },
                             new object[4]
                            {
                                acdoAccountReference.from_item_type_value, acdoAccountReference.transaction_type_value,
                                acdoAccountReference.source_type_value, acdoAccountReference.plan_id
                            }, null, null);
                }

                lclbAccountReference = lobjBase.GetCollection<busAccountReference>(ldtbAccountReference, "icdoAccountReference");

                //Validate the Account Reference (If No Records or Multiple Records found for the Given Combination)
                larrList = ValidateAccountReference(lclbAccountReference);
                if (larrList.Count > 0) return larrList;

                //Generate the GL only if the Flag is true
                AddGLTransaction(lclbAccountReference[0].icdoAccountReference, aintPersonId, aintOrgId, aintSourceId, adecAmount, adtEffectiveDate, adtPostingDate);
            } //End of If Transaction Type == Allocation (Negative Adjustments)

            /*****************************************************************************
            * Transaction Type = Transfer
            ****************************************************************************/
            if (acdoAccountReference.transaction_type_value == busConstant.TransactionTypeTransfer)
            {
                //If Item Type is invalid
                if (String.IsNullOrEmpty(acdoAccountReference.item_type_value))
                {
                    lobjError.istrErrorID = "0";
                    lobjError.istrErrorMessage = "Invalid Item Type!";
                    larrList.Add(lobjError);
                    return larrList;
                }

                //Generate the Transfer GL
                ldtbAccountReference =
                    busBase.Select("cdoAccountReference.GetValidAccountReference",
                            new object[5]
                            {
                                acdoAccountReference.transaction_type_value,acdoAccountReference.source_type_value,
                                acdoAccountReference.item_type_value, acdoAccountReference.plan_id,                                 
                                acdoAccountReference.status_transition_value
                            });
                lclbAccountReference = lobjBase.GetCollection<busAccountReference>(ldtbAccountReference,"icdoAccountReference");
                foreach (busAccountReference lobjAccountReference in lclbAccountReference)
                {
                    AddGLTransaction(lobjAccountReference.icdoAccountReference, aintPersonId, aintOrgId, aintSourceId, adecAmount, adtEffectiveDate, adtPostingDate, aintOrgIdEmpRep); //938 GL - Derrick Mail
                }
            } //End of If Transaction Type == Transfer

            return larrList;
        }

        private static ArrayList ValidateAccountReference(Collection<busAccountReference> aclbAccountReference)
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();

            //If No Records, throw an Error Message
            if (aclbAccountReference.Count == 0)
            {
                lobjError.istrErrorID = "0";
                lobjError.istrErrorMessage = "No Account Reference found for Given Combination!";
                larrList.Add(lobjError);
                return larrList;
            }

            //If More than One Returns, throw an error message
            if (aclbAccountReference.Count > 1)
            {
                lobjError.istrErrorID = "0";
                lobjError.istrErrorMessage = "Multiple Account Reference found for Given Combination!";
                larrList.Add(lobjError);
                return larrList;
            }
            return larrList;
        }

        /// <summary>
        /// Function to Add GL Transaction
        /// </summary>
        /// <param name="acdoAccountReference"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aintOrgId"></param>
        /// <param name="aintSourceId"></param>
        /// <param name="adecAmount"></param>
        /// <param name="adtEffectiveDate"></param>
        /// <param name="adtPostingDate"></param>
        private static void AddGLTransaction(cdoAccountReference acdoAccountReference, int aintPersonId,
            int aintOrgId, int aintSourceId, decimal adecAmount, DateTime adtEffectiveDate,
            DateTime adtPostingDate, int aintOrgIdEmpRep = 0, decimal adecDbAmt = 0.0M, decimal adecCrAmt = 0.0M,
            bool ablnDCAmtDft = false, bool ablnNoCreditEntry = false, bool ablnIsNDOrgForDebit = false, bool ablnIsNDOrgForCreditTran = false,
            bool ablnIsGLEntryFromPaymentCanceled = false, bool ablnIsTaxItemDebitActChanged = false, bool ablnIsTFFRPaymentCancel = false)
        {
            //If GL Flag is Not Set, Dont Generate the GL
            if (acdoAccountReference.generate_gl_flag != busConstant.IsGLFlagTrue) return;
            //Insert the GL Transaction
            //Create the Debit Entry

            cdoGlTransaction lcdoDebitGlTransaction = new cdoGlTransaction();
            lcdoDebitGlTransaction.plan_id = acdoAccountReference.plan_id;
            lcdoDebitGlTransaction.fund_value = acdoAccountReference.fund_value;
            lcdoDebitGlTransaction.dept_value = acdoAccountReference.dept_value;
            lcdoDebitGlTransaction.source_type_value = acdoAccountReference.source_type_value;
            lcdoDebitGlTransaction.item_type_value = acdoAccountReference.item_type_value;
            lcdoDebitGlTransaction.transaction_type_value = acdoAccountReference.transaction_type_value;
            lcdoDebitGlTransaction.account_reference_id = acdoAccountReference.account_reference_id;
            lcdoDebitGlTransaction.account_id = ablnIsTaxItemDebitActChanged ? busConstant.TaxItemDebitAccountId : acdoAccountReference.debit_account_id;
            lcdoDebitGlTransaction.org_id = aintOrgIdEmpRep > 0 ? aintOrgIdEmpRep : (ablnIsNDOrgForDebit || (ablnIsGLEntryFromPaymentCanceled && (acdoAccountReference.debit_account_id == busConstant.CanceledPremiumReceivableAccount))) ? busConstant.NDPERSOrgId : aintOrgId;
            lcdoDebitGlTransaction.person_id = aintOrgIdEmpRep > 0 ? 0 : (ablnIsNDOrgForDebit || (ablnIsGLEntryFromPaymentCanceled && (acdoAccountReference.debit_account_id == busConstant.CanceledPremiumReceivableAccount))) ? 0 : (ablnIsTFFRPaymentCancel && aintPersonId > 0) ? 0 : aintPersonId;
            lcdoDebitGlTransaction.source_id = aintSourceId;
            lcdoDebitGlTransaction.debit_amount = ablnDCAmtDft ? adecDbAmt : adecAmount;
            lcdoDebitGlTransaction.credit_amount = 0;
            lcdoDebitGlTransaction.posting_date = adtPostingDate;
            lcdoDebitGlTransaction.journal_description = acdoAccountReference.journal_description;      // PIR ID 158
            lcdoDebitGlTransaction.effective_date = adtEffectiveDate;
            lcdoDebitGlTransaction.Insert();
            if (!ablnNoCreditEntry)
            {
                //Create the Credit Entry
                cdoGlTransaction lcdoCreditGlTransaction = new cdoGlTransaction();
                lcdoCreditGlTransaction.plan_id = acdoAccountReference.plan_id;
                lcdoCreditGlTransaction.fund_value = acdoAccountReference.fund_value;
                lcdoCreditGlTransaction.dept_value = acdoAccountReference.dept_value;
                lcdoCreditGlTransaction.source_type_value = acdoAccountReference.source_type_value;
                lcdoCreditGlTransaction.item_type_value = acdoAccountReference.item_type_value;
                lcdoCreditGlTransaction.transaction_type_value = acdoAccountReference.transaction_type_value;
                lcdoCreditGlTransaction.account_reference_id = acdoAccountReference.account_reference_id;
                lcdoCreditGlTransaction.account_id = acdoAccountReference.credit_account_id;
                lcdoCreditGlTransaction.org_id = ablnIsNDOrgForCreditTran ? busConstant.NDPERSOrgId : (ablnIsTFFRPaymentCancel && aintPersonId > 0) ? 0 : aintOrgId;
                lcdoCreditGlTransaction.person_id = ablnIsNDOrgForCreditTran ? 0 : aintPersonId;
                lcdoCreditGlTransaction.source_id = aintSourceId;
                lcdoCreditGlTransaction.credit_amount = ablnDCAmtDft ? adecCrAmt : adecAmount;
                lcdoCreditGlTransaction.debit_amount = 0;
                lcdoCreditGlTransaction.posting_date = adtPostingDate;
                lcdoCreditGlTransaction.effective_date = adtEffectiveDate;
                lcdoCreditGlTransaction.journal_description = acdoAccountReference.journal_description;      // PIR ID 158
                lcdoCreditGlTransaction.Insert();
            }
        }

        /// <summary>
        ///  Method to generate GL for status transition from Payment history header, distribution and Recovery
        /// </summary>
        /// <param name="astrStatusTransitionType">Status Transition Type</param>
        /// <param name="astrOldStatus">Old Status</param>
        /// <param name="astrNewStatus">new Status</param>
        /// <param name="astrTransactionType">Transaction Type</param>
        /// <param name="astrSourceTypeValue">Source Type</param>
        /// <param name="abusPaymentHistoryHeader">Payment History header object</param>
        /// <param name="abusPaymentRecovery">Recovery object</param>
        /// <param name="aobjPassInfo">Obj Pass Info</param>
        public static void GenerateGL(string astrStatusTransitionType, string astrOldStatus, string astrNewStatus, string astrTransactionType,
                                        string astrSourceTypeValue, busPaymentHistoryHeader abusPaymentHistoryHeader, busPaymentRecovery abusPaymentRecovery,
                                        utlPassInfo aobjPassInfo, string astrPaymentItemCodeValue = null)
        {
            //taking from DB Cache
            DataTable ldtGLRef = aobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_status_transition_gl_ref", null);
            //Taking corresponding transition type value for the status transition
            DataTable ldtGLFiltered = ldtGLRef.AsEnumerable()
                                                .Where(o => o.Field<string>("status_transition_type_value") == astrStatusTransitionType
                                                        && o.Field<string>("from_status") == astrOldStatus
                                                        && o.Field<string>("to_status") == astrNewStatus
                                                        && o.Field<string>("generate_gl_flag") == busConstant.Flag_Yes)
                                                .AsDataTable();

            //if transition type value is available
            foreach (DataRow dr in ldtGLFiltered.Rows)
            {
                //if Generate method called from Recv. created
                if (astrStatusTransitionType == busConstant.GLStatusTransitionRecovery)
                {
                    if (abusPaymentRecovery.ibusBenefitOverpaymentHeader == null)
                        abusPaymentRecovery.LoadBenefitOverpaymentHeader();
                    if (abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbMonthwiseAdjustmentDetails == null)
                        abusPaymentRecovery.ibusBenefitOverpaymentHeader.LoadMonthwiseAdjustmentDetails();
                    if (abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbPaymentBenefitOverpaymentDetail == null)
                        abusPaymentRecovery.ibusBenefitOverpaymentHeader.LoadPaymentBenefitOverpaymentDetails();
                    if (abusPaymentRecovery.ibusPayeeAccount == null)
                        abusPaymentRecovery.LoadPayeeAccount();
                    if (abusPaymentRecovery.ibusPayeeAccount.ibusApplication == null)
                        abusPaymentRecovery.ibusPayeeAccount.LoadApplication();
                    if (dr["status_transition_value"] != DBNull.Value && dr["status_transition_value"].ToString() == busConstant.GLStatusTransitionValueWriteOff)
                    {
                        decimal ldecGLWriteOffAmount = 0.0M;
                        abusPaymentRecovery.LoadLTDPrincipleAmountPaid();
                        ldecGLWriteOffAmount = abusPaymentRecovery.icdoPaymentRecovery.recovery_amount - abusPaymentRecovery.idecLTDPrincipleAmountPaid;
                        string lstrItemTypeCodeValue = string.Empty;
                        lstrItemTypeCodeValue = ((abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.Refund) ||
                            (abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund)) ?
                            busConstant.GLOneTimePayPenRecItemTypeCodeValue :
                            busConstant.GLMonthlyPayPenRecItemTypeCodeValue;
                        GenerateGL(abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                            abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_org_id,
                                            abusPaymentRecovery.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id,
                                            abusPaymentRecovery.ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id,
                                            ldecGLWriteOffAmount,
                                            astrSourceTypeValue,
                                            astrTransactionType,
                                            dr["status_transition_value"].ToString(),
                                            lstrItemTypeCodeValue,
                                            DateTime.Today,
                                            DateTime.Today,
                                            aobjPassInfo);
                    }
                    else if(dr["status_transition_value"] != DBNull.Value && dr["status_transition_value"].ToString() == busConstant.GLStatusTransitionValueRecPendingApprToApproved)
                    {
                        decimal ldecNetPensionRecv = 0.0M;
                        decimal ldecGrossPensionRecv = 0.0M;
                        decimal ldecInterestRecv = 0.0M;
                        ldecNetPensionRecv = abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbPaymentBenefitOverpaymentDetail.Sum(o => o.icdoPaymentBenefitOverpaymentDetail.amount);
                        ldecGrossPensionRecv = abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbPaymentBenefitOverpaymentDetail.Where(i=>i.icdoPaymentBenefitOverpaymentDetail.amount > 0).Sum(o => o.icdoPaymentBenefitOverpaymentDetail.amount);
                        ldecInterestRecv = abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbMonthwiseAdjustmentDetails.Sum(o => o.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount);
                        //taking from DB Cache
                        DataTable ldtPaymentItem = aobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_item_type", null);
                        DataTable ldtFilteredPITPenRecv = new DataTable();
                        //GL need to be made according to the type of they payee account - annuity or refund - PIR 938 - Multiple Bank Accounts
                        if ((abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.Refund) ||
                            (abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund))
                        {
                            ldtFilteredPITPenRecv = ldtPaymentItem.AsEnumerable()
                                                                        .Where(o => o.Field<string>("item_type_code") == busConstant.PAPITPensionReceivableRefund)
                                                                        .AsDataTable();
                        }
                        else
                        {
                            ldtFilteredPITPenRecv = ldtPaymentItem.AsEnumerable()
                                                                        .Where(o => o.Field<string>("item_type_code") == busConstant.PAPITPensionReceivable)
                                                                        .AsDataTable();
                        }
                        if (ldtFilteredPITPenRecv.Rows.Count > 0)
                        {
                            //generating GL for pension recv
                            GenerateGL(abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                            abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_org_id,
                                            abusPaymentRecovery.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id,
                                            abusPaymentRecovery.ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id,
                                            0.0M,
                                            astrSourceTypeValue,
                                            astrTransactionType,
                                            dr["status_transition_value"].ToString(),
                                            ldtFilteredPITPenRecv.Rows[0]["item_type_code_value"].ToString(),
                                            DateTime.Today,
                                            DateTime.Today,
                                            aobjPassInfo, adecDebAmt : ldecNetPensionRecv, adecCredAmt : ldecGrossPensionRecv, ablnAreDebCreditAmtDfrt : true);
                        }
                        if (ldecInterestRecv > 0)
                        {
                            //generating GL for pension recv interest
                            GenerateGL(abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                            abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_org_id,
                                            abusPaymentRecovery.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id,
                                            abusPaymentRecovery.ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id,
                                            Math.Abs(ldecInterestRecv),
                                            astrSourceTypeValue,
                                            astrTransactionType,
                                            dr["status_transition_value"].ToString(),
                                            busConstant.PaymentItemCodeValueRecvInterest,
                                            DateTime.Today,
                                            DateTime.Today,
                                            aobjPassInfo);
                        }
                        DateTime  ldtePaymentDate = DateTime.MinValue;
                        busPaymentBenefitOverpaymentDetail lbusPaymentBenefitOverpaymentDetail = abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbPaymentBenefitOverpaymentDetail.FirstOrDefault();
                        if (lbusPaymentBenefitOverpaymentDetail.IsNotNull())
                            ldtePaymentDate = lbusPaymentBenefitOverpaymentDetail.icdoPaymentBenefitOverpaymentDetail.date_of_1099r;
                        foreach (busPaymentBenefitOverpaymentDetail lobjDetail in abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbPaymentBenefitOverpaymentDetail)
                        {
                            if (lobjDetail.icdoPaymentBenefitOverpaymentDetail.amount < 0)
                            {
                                ldtFilteredPITPenRecv = new DataTable();
                                ldtFilteredPITPenRecv = ldtPaymentItem.AsEnumerable()
                                                        .Where(o => o.Field<int>("payment_item_type_id") == lobjDetail.icdoPaymentBenefitOverpaymentDetail.payment_item_type_id)
                                                        .AsDataTable();
                                if (ldtFilteredPITPenRecv.Rows.Count > 0)
                                {
                                    //generating GL for pension recv
                                    GenerateGL(abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                               abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_org_id,
                                                    abusPaymentRecovery.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id,
                                                    lobjDetail.icdoPaymentBenefitOverpaymentDetail.benefit_overpayment_detail_id,
                                                    Math.Abs(lobjDetail.icdoPaymentBenefitOverpaymentDetail.amount),
                                                    astrSourceTypeValue,
                                                    astrTransactionType,
                                                    dr["status_transition_value"].ToString(),
                                                    ldtFilteredPITPenRecv.Rows[0]["item_type_code_value"].ToString(),
                                                    DateTime.Today,
                                                    DateTime.Today,
                                                    aobjPassInfo, adecDebAmt: 0.0M, adecCredAmt: 0.0M, ablnAreDebCreditAmtDfrt: false,
                                                    ablnNoCreditEntry: ShouldThereBeACreditEntry(ldtFilteredPITPenRecv.Rows[0]["item_type_code_value"].ToString()),
                                                    ablnIsNDPERSOrgIdForForDebit: IsInsuranceDeductionItemTypeCodeValue(ldtFilteredPITPenRecv.Rows[0]["item_type_code_value"].ToString()),
                                                    ablnIsNDPERSOrgIdForCreditTran: IsInsuranceDeductionItemTypeCodeValue(ldtFilteredPITPenRecv.Rows[0]["item_type_code_value"].ToString()),
                                                    ablnIsFromPaymentCanceled : false, ablnIsTaxItemDebitAccountChanged : ShouldTheTaxItemDebitActChange(ldtFilteredPITPenRecv.Rows[0]["item_type_code_value"].ToString(), ldtePaymentDate));
                                }
                            }
                        }
                    }
                    else if (dr["status_transition_value"] != DBNull.Value && dr["status_transition_value"].ToString() == busConstant.GLStatusTransitionValueApproveToCancel) // PIR 20688
                    {
                        DataTable ldtPaymentItem = aobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_item_type", null);
                        DataTable ldtFilteredPITPenRecv = new DataTable();
                        ldtFilteredPITPenRecv = ldtPaymentItem.AsEnumerable()
                                                                    .Where(o => o.Field<string>("item_type_code") == busConstant.PAPITPensionReceivable)
                                                                    .AsDataTable();
                        decimal ldecPensionRecv = 0.0M;
                        ldecPensionRecv = abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbPaymentBenefitOverpaymentDetail.Where(i => i.icdoPaymentBenefitOverpaymentDetail.amount > 0).Sum(o => o.icdoPaymentBenefitOverpaymentDetail.amount);
                        GenerateGL(abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                            abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_org_id,
                                            abusPaymentRecovery.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id,
                                            abusPaymentRecovery.ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id,
                                            Math.Abs(ldecPensionRecv),
                                            astrSourceTypeValue,
                                            astrTransactionType,
                                            dr["status_transition_value"].ToString(),
                                            ldtFilteredPITPenRecv.Rows[0]["item_type_code_value"].ToString(),
                                            DateTime.Today,//only date part in GL as per Satya
                                            DateTime.Today,
                                            aobjPassInfo);
                    }
                    else
                    {
                        decimal ldecPensionRecv = 0.0M;
                        decimal ldecInterestRecv = 0.0M;
                        ldecPensionRecv = abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbMonthwiseAdjustmentDetails.Sum(o => o.icdoPayeeAccountMonthwiseAdjustmentDetail.amount);
                        ldecInterestRecv = abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbMonthwiseAdjustmentDetails.Sum(o => o.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount);
                        //taking from DB Cache
                        DataTable ldtPaymentItem = aobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_item_type", null);
                        DataTable ldtFilteredPITPenRecv = new DataTable();
                        ldtFilteredPITPenRecv = ldtPaymentItem.AsEnumerable()
                                                                    .Where(o => o.Field<string>("item_type_code") == busConstant.PAPITPensionReceivable)
                                                                    .AsDataTable();
                        if (ldtFilteredPITPenRecv.Rows.Count > 0 && ldecPensionRecv > 0)
                        {
                            //generating GL for pension recv
                            GenerateGL(abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                            abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_org_id,
                                            abusPaymentRecovery.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id,
                                            abusPaymentRecovery.ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id,
                                            Math.Abs(ldecPensionRecv),
                                            astrSourceTypeValue,
                                            astrTransactionType,
                                            dr["status_transition_value"].ToString(),
                                            ldtFilteredPITPenRecv.Rows[0]["item_type_code_value"].ToString(),
                                            DateTime.Today,//only date part in GL as per Satya
                                            DateTime.Today,
                                            aobjPassInfo);
                        }

                        if (ldecInterestRecv > 0)
                        {
                            //generating GL for pension recv interest
                            GenerateGL(abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                            abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_org_id,
                                            abusPaymentRecovery.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id,
                                            abusPaymentRecovery.ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id,
                                            Math.Abs(ldecInterestRecv),
                                            astrSourceTypeValue,
                                            astrTransactionType,
                                            dr["status_transition_value"].ToString(),
                                            busConstant.PaymentItemCodeValueRecvInterest,
                                            DateTime.Today,
                                            DateTime.Today,
                                            aobjPassInfo);
                        }

                        //prod pir 5142 : GL for deduction items
                        foreach (busPaymentBenefitOverpaymentDetail lobjDetail in abusPaymentRecovery.ibusBenefitOverpaymentHeader.iclbPaymentBenefitOverpaymentDetail)
                        {
                            if (lobjDetail.icdoPaymentBenefitOverpaymentDetail.amount < 0)
                            {
                                ldtFilteredPITPenRecv = new DataTable();
                                ldtFilteredPITPenRecv = ldtPaymentItem.AsEnumerable()
                                                        .Where(o => o.Field<int>("payment_item_type_id") == lobjDetail.icdoPaymentBenefitOverpaymentDetail.payment_item_type_id)
                                                        .AsDataTable();
                                if (ldtFilteredPITPenRecv.Rows.Count > 0)
                                {
                                    //generating GL for pension recv
                                    GenerateGL(abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                                    abusPaymentRecovery.ibusPayeeAccount.icdoPayeeAccount.payee_org_id,
                                                    abusPaymentRecovery.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id,
                                                    lobjDetail.icdoPaymentBenefitOverpaymentDetail.benefit_overpayment_detail_id,
                                                    Math.Abs(lobjDetail.icdoPaymentBenefitOverpaymentDetail.amount),
                                                    astrSourceTypeValue,
                                                    astrTransactionType,
                                                    dr["status_transition_value"].ToString(),
                                                    ldtFilteredPITPenRecv.Rows[0]["item_type_code_value"].ToString(),
                                                    DateTime.Today,
                                                    DateTime.Today,
                                                    aobjPassInfo);
                                }
                            }
                        }
                    }
                }
                //if called from payment history header
                else if (astrStatusTransitionType == busConstant.GLStatusTransitionPaymentHistory)
                {
                    if (abusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                        abusPaymentHistoryHeader.LoadPaymentHistoryDetails();
                    if (abusPaymentHistoryHeader.ibusPayeeAccount == null)
                        abusPaymentHistoryHeader.LoadPayeeAccount();
                    if ((dr["status_transition_value"] != DBNull.Value && ((dr["status_transition_value"].ToString() == busConstant.GLStatusTransitionValueCanceled) ||
                        (dr["status_transition_value"].ToString() == busConstant.GLStatusTransitionValueCancelPendingToCancelPriorPayment))))
                    {
                        int lintPaymentDate = 0;
                        if (abusPaymentHistoryHeader.IsNotNull() && abusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id > 0)
                            lintPaymentDate = abusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date.Year;
                        /*The account references for cancel and cancel prior payment are same except federal and state tax mapping*/
                        if (lintPaymentDate > 0 && dr["status_transition_value"].ToString() == busConstant.GLStatusTransitionValueCanceled && (lintPaymentDate < DateTime.Today.Year))
                            dr["status_transition_value"] = busConstant.GLStatusTransitionValueCancelPendingToCancelPriorPayment;
                        bool lblnIsTFFRPaymentCancelation = false;
                        int lintTFFRRemitCanPersonId = 0;
                        if (abusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id == 0 && abusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id == 0)
                        {
                            DataTable ldtbRemittance = busBase.Select<cdoRemittance>(new string[2] { enmRemittance.payment_history_header_id.ToString(), enmRemittance.refund_to_org_id.ToString() }, new object[2] { abusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id, busConstant.OrganizationTFFROrgId }, null, null);
                            if (ldtbRemittance.Rows.Count > 0 && ldtbRemittance.Rows[0][enmRemittance.person_id.ToString()] != DBNull.Value 
                                && Convert.ToInt32(ldtbRemittance.Rows[0][enmRemittance.person_id.ToString()]) > 0)
                            {
                                lblnIsTFFRPaymentCancelation = true;
                                lintTFFRRemitCanPersonId = Convert.ToInt32(ldtbRemittance.Rows[0][enmRemittance.person_id.ToString()]);
                            }
                        }
                        foreach (busPaymentHistoryDetail lobjDetail in abusPaymentHistoryHeader.iclbPaymentHistoryDetail)
                        {
                            GenerateGL((lblnIsTFFRPaymentCancelation && lintTFFRRemitCanPersonId > 0) ? lintTFFRRemitCanPersonId : abusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id, //938-GL changes
                                        abusPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id,//938-GL changes
                                        abusPaymentHistoryHeader.icdoPaymentHistoryHeader.plan_id,
                                        lobjDetail.icdoPaymentHistoryDetail.payment_history_detail_id,
                                        Math.Abs(lobjDetail.icdoPaymentHistoryDetail.amount),
                                        astrSourceTypeValue,
                                        astrTransactionType,
                                        dr["status_transition_value"].ToString(),
                                        lobjDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code_value,
                                        DateTime.Today,
                                        DateTime.Today,
                                        aobjPassInfo, adecDebAmt: 0.0M, adecCredAmt: 0.0M, ablnAreDebCreditAmtDfrt: false,
                                                    ablnNoCreditEntry: false,
                                                    ablnIsNDPERSOrgIdForForDebit: false,
                                                    ablnIsNDPERSOrgIdForCreditTran: IsInsuranceDeductionItemTypeCodeValue(lobjDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code_value),
                                                    ablnIsFromPaymentCanceled : IsInsuranceDeductionItemTypeCodeValue(lobjDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code_value), ablnIsTFFRCancel : (lblnIsTFFRPaymentCancelation && lintTFFRRemitCanPersonId > 0));
                        }
                    }
                    else
                    {
                        foreach (busPaymentHistoryDetail lobjDetail in abusPaymentHistoryHeader.iclbPaymentHistoryDetail)
                        {
                            GenerateGL(abusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id, //938-GL changes
                                        abusPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id,//938-GL changes
                                        abusPaymentHistoryHeader.icdoPaymentHistoryHeader.plan_id,
                                        lobjDetail.icdoPaymentHistoryDetail.payment_history_detail_id,
                                        Math.Abs(lobjDetail.icdoPaymentHistoryDetail.amount),
                                        astrSourceTypeValue,
                                        astrTransactionType,
                                        dr["status_transition_value"].ToString(),
                                        lobjDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code_value,
                                        DateTime.Today,
                                        DateTime.Today,
                                        aobjPassInfo);
                        }
                    }
                }
                //if called from payment history distribution
                else
                {
                    //PIR 16219
                    if (astrPaymentItemCodeValue == null) astrPaymentItemCodeValue = busConstant.PaymentItemCodeValueNetAmountReissue;
                    if (abusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                        abusPaymentHistoryHeader.LoadPaymentHistoryDetails();
                    abusPaymentHistoryHeader.CalculateAmounts();
                    if (abusPaymentHistoryHeader.ibusPayeeAccount == null)
                        abusPaymentHistoryHeader.LoadPayeeAccount();
                    if (abusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                    {
                        GenerateGL(abusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id, // PIR 938
                                        abusPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id, // PIR 938
                                        abusPaymentHistoryHeader.icdoPaymentHistoryHeader.plan_id,
                                        abusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id,
                                        Math.Abs(abusPaymentHistoryHeader.icdoPaymentHistoryHeader.net_amount),
                                        astrSourceTypeValue,
                                        astrTransactionType,
                                        dr["status_transition_value"].ToString(),
                                        astrPaymentItemCodeValue, //PIR 16219
                                        DateTime.Today,
                                        DateTime.Today,
                                        aobjPassInfo);
                    }
                    else // PIR 12271 - GL issue
                    {
                        GenerateGL(abusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id,
                                        abusPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id,
                                        abusPaymentHistoryHeader.icdoPaymentHistoryHeader.plan_id,
                                        abusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id,
                                        Math.Abs(abusPaymentHistoryHeader.icdoPaymentHistoryHeader.net_amount),
                                        astrSourceTypeValue,
                                        astrTransactionType,
                                        dr["status_transition_value"].ToString(),
                                        astrPaymentItemCodeValue, //PIR 16219
                                        DateTime.Today,
                                        DateTime.Today,
                                        aobjPassInfo);
                    }
                }
            }
        }

        private static bool ShouldTheTaxItemDebitActChange(string astrItemTypeCodeValue, DateTime adtePaymentDate)
        {
            return (adtePaymentDate != DateTime.MinValue && adtePaymentDate.Year < DateTime.Now.Year && IsTaxItemTypeCodeValue(astrItemTypeCodeValue));           
        }

        private static bool ShouldThereBeACreditEntry(string astrItemTypeCodeValue)
        {
            return ((astrItemTypeCodeValue == busConstant.GLDuesReceivableItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLDuesItemTypeCodeValue) || IsTaxItemTypeCodeValue(astrItemTypeCodeValue) ||
                    IsInsuranceDeductionItemTypeCodeValue(astrItemTypeCodeValue));
        }

        private static bool IsTaxItemTypeCodeValue(string astrItemTypeCodeValue)
        {
            return ((astrItemTypeCodeValue == busConstant.GLStateTaxAmountItemTypeCodeValue) ||
                     (astrItemTypeCodeValue == busConstant.GLPLSOFederalTaxAmountItemTypeCodeValue) ||
                     (astrItemTypeCodeValue == busConstant.GLPLSOStateTaxAmountItemTypeCodeValue) ||
                     (astrItemTypeCodeValue == busConstant.PaymentItemCodeValueFedTaxAmount) ||
                     (astrItemTypeCodeValue == busConstant.PaymentItemCodeValueStateTaxAmount) ||
                     (astrItemTypeCodeValue == busConstant.GLStateTaxOnInterestAmountItemTypeCodeValue) ||
                     (astrItemTypeCodeValue == busConstant.GLFedTaxOnInterestAmountItemTypeCodeValue) ||
                     (astrItemTypeCodeValue == busConstant.GLRegOneTimeFedTaxAmtItemTypeCodeValue) ||
                     (astrItemTypeCodeValue == busConstant.GLRegOneTimeStateTaxAmtItemTypeCodeValue) ||
                     (astrItemTypeCodeValue == busConstant.GLSpecialOneTimeFedTaxAmtItemTypeCodeValue) ||
                     (astrItemTypeCodeValue == busConstant.GLRegMonthlyFedTaxAmtItemTypeCodeValue) ||
                     (astrItemTypeCodeValue == busConstant.GLRegMonthlyStateTaxAmtItemTypeCodeValue) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyFederalTaxonInterestAmount) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyStateTaxonInterestAmount) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyFederalTaxonInterestAmount58) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyStateTaxonInterestAmount59) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyFederalTaxRefund) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyStateTaxRefund) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyFederalTaxAmount114) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyStateTaxAmount115) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyFederalTaxAmount116) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyStateTaxAmount117) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyFederalTaxAmount118) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularMonthlyStateTaxAmount119) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularOneTimeFederalTaxonInterestAmount) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularOneTimeStateTaxonInterestAmount) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularOneTimeFederalTaxAmount) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularOneTimeStateTaxAmount) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularOneTimeFederalTaxonInterestAmount414) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularOneTimeFederalTaxAmount418) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularOneTimeStateTaxAmount419) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularOneTimeStateTaxonInterestAmount425) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularOneTimeFederalTaxAmount428) ||
                     (astrItemTypeCodeValue == busConstant.GLRegularOneTimeStateTaxAmount429));
        }

        private static bool IsInsuranceDeductionItemTypeCodeValue(string astrItemTypeCodeValue)
        {
            return ((astrItemTypeCodeValue == busConstant.GLThirdPartyHealthInsuranceItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremHealthItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremLifeItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremDentalItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremVisItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremLTCItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLThirdPartyHealthInsRecItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremHealthRecItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremLifeRecItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremDentRecItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremVisRecItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremLTCRecItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremMedPartDItemTypeCodeValue) ||
                    (astrItemTypeCodeValue == busConstant.GLInsuPremMedPartDRecItemTypeCodeValue));
        }

        /// <summary>
        /// Method to generate GL for status transition from Payment history header, distribution and Recovery
        /// </summary>
        /// <param name="aintPersonID">Person Id</param>
        /// <param name="aintOrgID">Org ID</param>
        /// <param name="aintPlanID">Plan Id</param>
        /// <param name="aintSourceID">Source Id</param>
        /// <param name="adecAmount">Amount</param>
        /// <param name="astrSourceTypeValue">Source Type Value</param>
        /// <param name="astrTransactionType">Transaction Type</param>
        /// <param name="astrStatusTransitionValue">Transition Type</param>
        /// <param name="astrItemTypeValue">Item type value</param>
        /// <param name="adtEffectiveDate">effective date</param>
        /// <param name="adtPostingDate">Posting Date</param>
        /// <param name="aobjPassInfo">Obj. pass info</param>
        public static void GenerateGL(int aintPersonID, int aintOrgID, int aintPlanID, int aintSourceID, decimal adecAmount,
            string astrSourceTypeValue, string astrTransactionType, string astrStatusTransitionValue, string astrItemTypeValue,
            DateTime adtEffectiveDate, DateTime adtPostingDate, utlPassInfo aobjPassInfo, decimal adecDebAmt = 0.0M, decimal adecCredAmt = 0.0M,
            bool ablnAreDebCreditAmtDfrt = false, bool ablnNoCreditEntry = false, bool ablnIsNDPERSOrgIdForForDebit = false,
            bool ablnIsNDPERSOrgIdForCreditTran = false, bool ablnIsFromPaymentCanceled = false, bool ablnIsTaxItemDebitAccountChanged = false, bool ablnIsTFFRCancel = false)
        {
            //creating Account reference
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = aintPlanID;
            lcdoAcccountReference.source_type_value = astrSourceTypeValue;
            lcdoAcccountReference.transaction_type_value = astrTransactionType;
            lcdoAcccountReference.item_type_value = astrItemTypeValue;
            lcdoAcccountReference.status_transition_value = astrStatusTransitionValue;
            //Generating GL
            GenerateGL(lcdoAcccountReference, aintPersonID, aintOrgID, aintSourceID, adecAmount, adtEffectiveDate,
                adtPostingDate, aobjPassInfo, 0, adecDebitAmount: adecDebAmt, adecCreditAmount: adecCredAmt,
                ablnAreCreditAmountDebitAmountDfrt: ablnAreDebCreditAmtDfrt, ablnNoGlCredEntry: ablnNoCreditEntry,
                ablnIsNDPOrgIdDebitEntry: ablnIsNDPERSOrgIdForForDebit, ablnIsNDPOrgIdCreditEntryTransfer: ablnIsNDPERSOrgIdForCreditTran,
                ablnIsGLFromPaymentCanceled: ablnIsFromPaymentCanceled, ablnIsTaxItemDebActChanged : ablnIsTaxItemDebitAccountChanged, ablnIsTFFRCanceled : ablnIsTFFRCancel);
        }
        /// <summary>
        /// Function to Reverse GL Transaction
        /// </summary>
        public static void ReverseGLTransactionForCancelRefund (cdoAccountReference acdoAccountReference, int aintPersonId,
            int aintOrgId, int aintSourceId, decimal adecAmount, DateTime adtEffectiveDate,
            DateTime adtPostingDate, int aintOrgIdEmpRep = 0, decimal adecDbAmt = 0.0M, decimal adecCrAmt = 0.0M,
            bool ablnDCAmtDft = false, bool ablnNoCreditEntry = false, bool ablnIsNDOrgForDebit = false, bool ablnIsNDOrgForCreditTran = false,
            bool ablnIsGLEntryFromPaymentCanceled = false, bool ablnIsTaxItemDebitActChanged = false, bool ablnIsTFFRPaymentCancel = false)
        {
            //If GL Flag is Not Set, Dont Generate the GL
            if (acdoAccountReference.generate_gl_flag != busConstant.IsGLFlagTrue) return;
            //Insert the GL Transaction
            //Create the Credit Entry

            cdoGlTransaction lcdoDebitGlTransaction = new cdoGlTransaction();
            lcdoDebitGlTransaction.plan_id = acdoAccountReference.plan_id;
            lcdoDebitGlTransaction.fund_value = acdoAccountReference.fund_value;
            lcdoDebitGlTransaction.dept_value = acdoAccountReference.dept_value;
            lcdoDebitGlTransaction.source_type_value = acdoAccountReference.source_type_value;
            lcdoDebitGlTransaction.item_type_value = acdoAccountReference.item_type_value;
            lcdoDebitGlTransaction.transaction_type_value = acdoAccountReference.transaction_type_value;
            lcdoDebitGlTransaction.account_reference_id = acdoAccountReference.account_reference_id;
            lcdoDebitGlTransaction.account_id = ablnIsTaxItemDebitActChanged ? busConstant.TaxItemDebitAccountId : acdoAccountReference.debit_account_id;
            lcdoDebitGlTransaction.org_id = aintOrgIdEmpRep > 0 ? aintOrgIdEmpRep : (ablnIsNDOrgForDebit || (ablnIsGLEntryFromPaymentCanceled && (acdoAccountReference.debit_account_id == busConstant.CanceledPremiumReceivableAccount))) ? busConstant.NDPERSOrgId : aintOrgId;
            lcdoDebitGlTransaction.person_id = aintOrgIdEmpRep > 0 ? 0 : (ablnIsNDOrgForDebit || (ablnIsGLEntryFromPaymentCanceled && (acdoAccountReference.debit_account_id == busConstant.CanceledPremiumReceivableAccount))) ? 0 : (ablnIsTFFRPaymentCancel && aintPersonId > 0) ? 0 : aintPersonId;
            lcdoDebitGlTransaction.source_id = aintSourceId;
            lcdoDebitGlTransaction.debit_amount = 0;
            lcdoDebitGlTransaction.credit_amount = ablnDCAmtDft ? adecDbAmt : adecAmount;
            lcdoDebitGlTransaction.posting_date = adtPostingDate;
            lcdoDebitGlTransaction.journal_description = acdoAccountReference.journal_description;      // PIR ID 158
            lcdoDebitGlTransaction.effective_date = adtEffectiveDate;
            lcdoDebitGlTransaction.Insert();
            if (!ablnNoCreditEntry)
            {
                //Create the Debit Entry
                cdoGlTransaction lcdoCreditGlTransaction = new cdoGlTransaction();
                lcdoCreditGlTransaction.plan_id = acdoAccountReference.plan_id;
                lcdoCreditGlTransaction.fund_value = acdoAccountReference.fund_value;
                lcdoCreditGlTransaction.dept_value = acdoAccountReference.dept_value;
                lcdoCreditGlTransaction.source_type_value = acdoAccountReference.source_type_value;
                lcdoCreditGlTransaction.item_type_value = acdoAccountReference.item_type_value;
                lcdoCreditGlTransaction.transaction_type_value = acdoAccountReference.transaction_type_value;
                lcdoCreditGlTransaction.account_reference_id = acdoAccountReference.account_reference_id;
                lcdoCreditGlTransaction.account_id = acdoAccountReference.credit_account_id;
                lcdoCreditGlTransaction.org_id = ablnIsNDOrgForCreditTran ? busConstant.NDPERSOrgId : (ablnIsTFFRPaymentCancel && aintPersonId > 0) ? 0 : aintOrgId;
                lcdoCreditGlTransaction.person_id = ablnIsNDOrgForCreditTran ? 0 : aintPersonId;
                lcdoCreditGlTransaction.source_id = aintSourceId;
                lcdoCreditGlTransaction.credit_amount = 0;
                lcdoCreditGlTransaction.debit_amount = ablnDCAmtDft ? adecCrAmt : adecAmount;
                lcdoCreditGlTransaction.posting_date = adtPostingDate;
                lcdoCreditGlTransaction.effective_date = adtEffectiveDate;
                lcdoCreditGlTransaction.journal_description = acdoAccountReference.journal_description;      // PIR ID 158
                lcdoCreditGlTransaction.Insert();
            }
        }
    }
}
