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
using System.Linq;
using System.Linq.Expressions;
using System.Globalization;
using System.Collections.Generic;
#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPaymentRecovery:
    /// Inherited from busPaymentRecoveryGen, the class is used to customize the business object busPaymentRecoveryGen.
    /// </summary>
    [Serializable]
    public class busPaymentRecovery : busPaymentRecoveryGen
    {
        public bool LoadPaymentRecoveryByBenefitOverpaymentID(int aintBenefitOverpaymentID)
        {
            icdoPaymentRecovery = new cdoPaymentRecovery();
            bool lblnResult = false;
            DataTable ldtPaymentRecovery = Select<cdoPaymentRecovery>
                (new string[1] { enmPaymentRecovery.benefit_overpayment_id.ToString() },
                new object[1] { aintBenefitOverpaymentID }, null, null);
            if (ldtPaymentRecovery.Rows.Count > 0)
            {
                lblnResult = true;
                icdoPaymentRecovery.LoadData(ldtPaymentRecovery.Rows[0]);
            }
            return lblnResult;
        }

        //Property to contain LTD amount paid
        public decimal idecLTDAmountPaid { get; set; }
        /// <summary>
        /// Method to load LTD amount from Payment recovery history
        /// </summary>
        public void LoadLTDAmountPaid()
        {
            if (iclbPaymentRecoveryHistory == null)
                LoadPaymentRecoveryHistory();
            idecLTDAmountPaid = iclbPaymentRecoveryHistory.Sum(o => o.icdoPaymentRecoveryHistory.principle_amount_paid + o.icdoPaymentRecoveryHistory.amortization_interest_paid);
        }
        //Property to contain LTD principle amount paid
        public decimal idecLTDPrincipleAmountPaid { get; set; }
        /// <summary>
        /// Method to load LTD amount from Payment recovery history
        /// </summary>
        public void LoadLTDPrincipleAmountPaid()
        {
            if (iclbPaymentRecoveryHistory == null)
                LoadPaymentRecoveryHistory();
            idecLTDPrincipleAmountPaid = iclbPaymentRecoveryHistory.Sum(o => o.icdoPaymentRecoveryHistory.principle_amount_paid);
        }
        //Property to contain LTD Amortization interest paid
        public decimal idecLTDAmortizationInterestPaid { get; set; }
        /// <summary>
        /// Method to load LTD Amortization interest paid from Payment recovery history
        /// </summary>
        public void LoadLTDAmortizationInterestPaid()
        {
            if (iclbPaymentRecoveryHistory == null)
                LoadPaymentRecoveryHistory();
            idecLTDAmortizationInterestPaid = iclbPaymentRecoveryHistory.Sum(o => o.icdoPaymentRecoveryHistory.amortization_interest_paid);
        }
        //Property to contain Latest posted date
        public DateTime idtLastPostedDate { get; set; }
        /// <summary>
        /// Method to load latest posted date
        /// </summary>
        public void LoadLastPostedDate()
        {
            if (iclbPaymentRecoveryHistory == null)
                LoadPaymentRecoveryHistory();
            if (iclbPaymentRecoveryHistory.Count > 0)
            {
                idtLastPostedDate = iclbPaymentRecoveryHistory.Max(o => o.icdoPaymentRecoveryHistory.posted_date);
            }
            //For the first month interest should be calculated from applied date to effective date 
            //as per discussion with satya 
            else
                idtLastPostedDate = icdoPaymentRecovery.effective_date;
        }
        //Property to contain Benefit Overpayment header bus obj for the recovery
        public busPaymentBenefitOverpaymentHeader ibusBenefitOverpaymentHeader { get; set; }
        /// <summary>
        /// Method to load Benefit Overpayment Header
        /// </summary>
        public void LoadBenefitOverpaymentHeader()
        {
            if (ibusBenefitOverpaymentHeader == null)
                ibusBenefitOverpaymentHeader = new busPaymentBenefitOverpaymentHeader();
            ibusBenefitOverpaymentHeader.FindPaymentBenefitOverpaymentHeader(icdoPaymentRecovery.benefit_overpayment_id);
        }

        /// <summary>
        /// Method to load total Recovery amount, need to call only in new mode
        /// </summary>
        public void LoadTotalRecoveryAmount()
        {
            if (ibusBenefitOverpaymentHeader == null)
                LoadBenefitOverpaymentHeader();
            ibusBenefitOverpaymentHeader.LoadTotalAmountDue();
            icdoPaymentRecovery.recovery_amount = ibusBenefitOverpaymentHeader.idecTotalAmountDueFromPayee;
        }

        public busPayeeAccount ibusPayeeAccount { get; set; }
        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount();
            ibusPayeeAccount.FindPayeeAccount(icdoPaymentRecovery.payee_account_id);
        }

        /// <summary>
        /// Method called when approve button is clicked
        /// </summary>
        /// <returns>Array List containing updated bus. object</returns>
        public ArrayList btn_Approve_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.iclbPaymentHistoryHeader == null)
                    ibusPayeeAccount.LoadPaymentHistoryHeader();
                IEnumerable<int> lenmPaymentHistoryHeader = ibusPayeeAccount.iclbPaymentHistoryHeader
                                                                            .Where(o => o.icdoPaymentHistoryHeader.payment_date.Year >= DateTime.Today.Year - 3)
                                                                            .Select(o => o.icdoPaymentHistoryHeader.payment_date.Year)
                                                                            .Distinct();
                if ((icdoPaymentRecovery.istrSuppressWarning == busConstant.Flag_Yes) ||
                    (lenmPaymentHistoryHeader != null && lenmPaymentHistoryHeader.Count() >= 3))
                {
                    //Loading Next benefit payment date
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                    if (ibusBenefitOverpaymentHeader == null)
                        LoadBenefitOverpaymentHeader();
                    icdoPaymentRecovery.effective_date = ibusPayeeAccount.idtNextBenefitPaymentDate;
                    if (!CalculateGrossReductionAmount())
                    {
                        lobjError = new utlError();
                        lobjError = AddError(6714, string.Empty);
                        larrList.Add(lobjError);
                    }
                    else
                    {
                        //If Adjustment reason is recalculatioin, Benefit overpayment details will be created on Approval of recovery
                        if (ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.adjustment_reason_value == busConstant.BenRecalAdjustmentReasonRecalculation)
                        {
                            CreateOverpaymentDetails();
                            LoadTotalRecoveryAmount();
                        }
                        //PIR 17504 - MOU Remapping cleanup - 4th point - Life time reduction should not create GL
                        if ((icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLumpSum) ||
                            (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeRepayOverTime)||
                            icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReduction ||
                            icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReductionForRTW)
                        {
                            //Creating GL
                            busGLHelper.GenerateGL(busConstant.GLStatusTransitionRecovery, busConstant.RecoveryStatusPendingApproval,
                                                    busConstant.RecoveryStatusApproved, busConstant.GLTransactionTypeStatusTransition,
                                                    busConstant.GLSourceTypeValuePensionRecv, null, this, iobjPassInfo);
                        }
                        //BR-079-27a & BR-079-27b
                        //--Start--//
                        if (ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.adjustment_reason_value == busConstant.BenRecalAdjustmentReceivableCreated)
                        {
                            DeleteBenefitOverpaymentDetailItemsOrPostProviderReportPayment();
                            ReloadBenefitOverpaymentHeaderAndDetails();
                            LoadTotalRecoveryAmount();
                            CalculateGrossReductionAmount();
                        }
                        //--End--//            
                        //If payment option is only NDPERS pension check, then create entries in PAPIT table
                        if (icdoPaymentRecovery.payment_option_value == busConstant.RecoveryPaymentOptionNDPERSPensionChk)
                            CreatePAPITItems();
                        //if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReduction)
                        //    icdoPaymentRecovery.status_value = busConstant.RecoveryStatusSatisfied;
                        //else
                            icdoPaymentRecovery.status_value = busConstant.RecoveryStatusApproved;
                        icdoPaymentRecovery.approved_date = DateTime.Now;
                        icdoPaymentRecovery.Update();
                        icdoPaymentRecovery.Select();
                        CalculateEstimatedEndDate();
                        EvaluateInitialLoadRules();
                        larrList.Add(this);
                    }
                }
                else if ((lenmPaymentHistoryHeader == null) || (lenmPaymentHistoryHeader != null && lenmPaymentHistoryHeader.Count() < 3))
                {
                    lobjError = new utlError();
                    lobjError = AddError(6728, string.Empty);
                    larrList.Add(lobjError);
                }
            }
            else
            {
                foreach (utlError lobjErr in iarrErrors)
                    larrList.Add(lobjError);
            }        
            return larrList;
        }

        /// <summary>
        /// method to reload benefit overpayment header and details
        /// </summary>
        private void ReloadBenefitOverpaymentHeaderAndDetails()
        {
            if (ibusBenefitOverpaymentHeader == null)
                LoadBenefitOverpaymentHeader();
            ibusBenefitOverpaymentHeader.LoadPaymentBenefitOverpaymentDetails();
        }

        /// <summary>
        /// Method called when Cancel button is clicked
        /// </summary>
        /// <returns>Arraylist containing </returns>
        public ArrayList btn_Cancel_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRecovery, icdoPaymentRecovery.status_value,
                                        busConstant.RecoveryStatusCancel, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValuePensionRecv, null, this, iobjPassInfo);
                icdoPaymentRecovery.status_value = busConstant.RecoveryStatusCancel;
                icdoPaymentRecovery.Update();
                icdoPaymentRecovery.Select();
                EvaluateInitialLoadRules();
                larrList.Add(this);
            }
            else
            {
                foreach (utlError lobjErr in iarrErrors)
                    larrList.Add(lobjErr);
            }
            return larrList;
        }

        /// <summary>
        /// method to delete items from benefit overpayment details
        /// </summary>
        private void DeleteBenefitOverpaymentDetailItemsOrPostProviderReportPayment()
        {
            if (ibusBenefitOverpaymentHeader.iclbPaymentBenefitOverpaymentDetail == null)
                ibusBenefitOverpaymentHeader.LoadPaymentBenefitOverpaymentDetails();
            foreach (busPaymentBenefitOverpaymentDetail lobjOverpaymentDetail in ibusBenefitOverpaymentHeader.iclbPaymentBenefitOverpaymentDetail)
            {
                if (lobjOverpaymentDetail.icdoPaymentBenefitOverpaymentDetail.amount < 0 &&
                    lobjOverpaymentDetail.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes)
                {
                    ibusPayeeAccount.LoadLatest1099rByPayeeAccount(lobjOverpaymentDetail.icdoPaymentBenefitOverpaymentDetail.date_of_1099r.Year);
                    //delete entries from benefit overpayment details
                    if (ibusPayeeAccount.ibusPayment1099r.icdoPayment1099r.payment_1099r_id > 0)
                    {
                        lobjOverpaymentDetail.icdoPaymentBenefitOverpaymentDetail.Delete();
                    }
                    //Create entries in provider report payment
                    else
                    {
                        busProviderReportPayment lobjProviderReport = new busProviderReportPayment { icdoProviderReportPayment = new cdoProviderReportPayment() };
                        if (lobjOverpaymentDetail.ibusPaymentItemType == null)
                            lobjOverpaymentDetail.LoadPaymentItemType();
                        DataTable ldtCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(2518);
                        DataTable ldtFiltered = ldtCodeValue.AsEnumerable()
                                                            .Where(o => o.Field<string>("data1") == lobjOverpaymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code)
                                                            .AsDataTable();
                        string lstrItemCode = ldtFiltered.Rows.Count > 0 ? ldtFiltered.Rows[0]["data2"].ToString() : string.Empty;
                        DataTable ldtPaymentItemType = iobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_item_type", "item_type_code = '" + lstrItemCode + "'");
                        if (ldtPaymentItemType.Rows.Count > 0)
                        {
                            //New code value added for provider report payment, since its pension receivables
                            lobjProviderReport.CreateProviderReportPayment(busConstant.SubSystemValuePensionRecv,
                                                                            lobjOverpaymentDetail.icdoPaymentBenefitOverpaymentDetail.benefit_overpayment_detail_id,
                                                                            ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                                                            lobjOverpaymentDetail.icdoPaymentBenefitOverpaymentDetail.vendor_org_id,
                                                                            ibusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                                            lobjOverpaymentDetail.icdoPaymentBenefitOverpaymentDetail.date_of_1099r,
                                                                            lobjOverpaymentDetail.icdoPaymentBenefitOverpaymentDetail.amount,
                                                                            0, Convert.ToInt32(ldtPaymentItemType.Rows[0]["payment_item_type_id"]));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// method to create entry in PAPIT table
        /// </summary>
        private void CreatePAPITItems()
        {
            busPayeeAccountPaymentItemType lobjPAPITPension = ibusPayeeAccount.GetLatestPayeeAccountPaymentItemType(busConstant.PAPITPensionReceivable);
            decimal ldecAmount = lobjPAPITPension != null ?
                                    lobjPAPITPension.icdoPayeeAccountPaymentItemType.amount :
                                    0.0M;
            //Creating Pension Receivable if Repayment type is Lump sum or Repay over time
            if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLumpSum ||
                icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeRepayOverTime ||
                icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReduction ||
                icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReductionForRTW)
            {
                icdoPaymentRecovery.payee_account_payment_item_type_id = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPensionReceivable,
                                                                    icdoPaymentRecovery.gross_reduction_amount + ldecAmount,
                                                                    string.Empty,
                                                                    0,
                                                                    ibusPayeeAccount.idtNextBenefitPaymentDate,
                                                                    DateTime.MinValue,
                                                                    false);
                if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReduction ||
                icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReductionForRTW)
                    //Recalcuating new taxes and posting to PAPIT
                    ibusPayeeAccount.CalculateAdjustmentTax(false);
            }
            ////Creating items if Repayment type is Life time reduction or life time reduction for RTW
            //else if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReduction ||
            //    icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReductionForRTW)
            //{
            //    busPayeeAccountPaymentItemType lobjPAPIT = ibusPayeeAccount.GetLatestPayeeAccountPaymentItemType(busConstant.PAPITTaxableAmount);
            //    if (lobjPAPIT != null)
            //    {
            //        decimal ldecNewTaxableAmount = lobjPAPIT.icdoPayeeAccountPaymentItemType.amount - icdoPaymentRecovery.gross_reduction_amount;
            //        //Creating new taxable amount
            //        ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxableAmount,
            //                                                            ldecNewTaxableAmount,
            //                                                            string.Empty,
            //                                                            0,
            //                                                            ibusPayeeAccount.idtNextBenefitPaymentDate,
            //                                                            DateTime.MinValue,
            //                                                            false);
            //        //Recalcuating new taxes and posting to PAPIT
            //        ibusPayeeAccount.CalculateAdjustmentTax(false);
            //    }
            //}
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
        }

        /// <summary>
        /// Method to create benefit overpayment details
        /// </summary>
        private void CreateOverpaymentDetails()
        {
            decimal ldecGrossTaxableAmountPaid, ldecTotalTaxableAmount;
            DateTime ldt1099rDate = new DateTime();
            //Loading all monthwise adjustment details for Benefit Overpayment header id
            if (ibusBenefitOverpaymentHeader.iclbMonthwiseAdjustmentDetails == null)
                ibusBenefitOverpaymentHeader.LoadMonthwiseAdjustmentDetails();
            //Calculating the total amount from monthwise adjustment details
            decimal ldecAmountDue = ibusBenefitOverpaymentHeader.iclbMonthwiseAdjustmentDetails
                .Sum(o => o.icdoPayeeAccountMonthwiseAdjustmentDetail.amount + o.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount);

            for (int i = 0; i < busConstant.Correction1099rYrs; i++)
            {
                ldt1099rDate = new DateTime(DateTime.Today.Year - i, 12, 01);
                if (ldecAmountDue <= 0)
                {
                    break;
                }
                ldecGrossTaxableAmountPaid = ldecTotalTaxableAmount = 0.0M;
                //Loading the Gross Taxable amount Paid for the year
                if (i < busConstant.Correction1099rYrs - 1)
                    ldecGrossTaxableAmountPaid = ibusBenefitOverpaymentHeader.LoadGrossTaxableAmountFTY(i);
                else
                    ldecGrossTaxableAmountPaid = ldecAmountDue + 1;

                //checking whether Total amount due is greater than Gross taxable amount for current year
                ldecTotalTaxableAmount = ldecAmountDue > ldecGrossTaxableAmountPaid ? ldecGrossTaxableAmountPaid : ldecAmountDue;
                if (ldecTotalTaxableAmount > 0)
                {
                    if (i == 0)
                    {
                        if (ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date > DateTime.Today)
                            break;
                        //Creating entry in detail for current date with lower amount (Total amount due or Gross Taxable amount)
                        ibusBenefitOverpaymentHeader.CreateBenefitOverpaymentDetail(ldecTotalTaxableAmount,
                                                        ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITTaxableAmount),
                                                        DateTime.Today, 0);
                    }
                    else
                    {
                        if (ldt1099rDate < ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date)
                            break;
                        //Creating an entry on last(-1 or -2 or -3) year (12/01/XXXX) for the lower amt
                        ibusBenefitOverpaymentHeader.CreateBenefitOverpaymentDetail(ldecTotalTaxableAmount,
                                                        ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITTaxableAmount),
                                                        new DateTime(DateTime.Today.Year - i, 12, 01), 0);
                    }
                }
                //Reducing Total amount due by amount created in benefit overpayment detail
                ldecAmountDue -= ldecTotalTaxableAmount;
            }
        }

        /// <summary>
        /// Method called when Write Off button is clicked
        /// </summary>
        /// <returns>Arraylist containing updated Bus. Object</returns>
        public ArrayList btn_WriteOff_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRecovery, icdoPaymentRecovery.status_value,
                                        busConstant.RecoveryStatusWriteOff, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValuePensionRecv, null, this, iobjPassInfo);
                icdoPaymentRecovery.write_off_date = DateTime.Now;
                icdoPaymentRecovery.status_value = busConstant.RecoveryStatusWriteOff;
                icdoPaymentRecovery.Update();
                icdoPaymentRecovery.Select();
                EvaluateInitialLoadRules();
                larrList.Add(this);
            }
            else
            {
                foreach (utlError lobjErr in iarrErrors)
                    larrList.Add(lobjErr);
            }
            return larrList;
        }

        public string istrOldPaymentOption { get; set; }
        public decimal idecOldGrossReductionAmount { get; set; }
        public int iintOldPayeeAccount { get; set; }

        public override void BeforePersistChanges()
        {
            CalculateEstimatedEndDate();
            LoadTotalRecoveryAmount();
            if (string.IsNullOrEmpty(icdoPaymentRecovery.status_value))
                icdoPaymentRecovery.status_value = busConstant.RecoveryStatusPendingApproval;
            //Updating PAPIT table with new gross reduction amount
            if (icdoPaymentRecovery.status_value == busConstant.RecoveryStatusApproved || icdoPaymentRecovery.status_value == busConstant.RecoveryStatusInProcess)
            {
                if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLumpSum)
                {
                    if (idecLTDPrincipleAmountPaid == 0)
                        LoadLTDPrincipleAmountPaid();
                    icdoPaymentRecovery.gross_reduction_amount = icdoPaymentRecovery.recovery_amount - idecLTDPrincipleAmountPaid;
                }
            }
            //For audit log purpose
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            icdoPaymentRecovery.person_id = ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id;
            icdoPaymentRecovery.org_id = ibusPayeeAccount.icdoPayeeAccount.payee_org_id;
            if (icdoPaymentRecovery.ihstOldValues.Count > 0)
            {
                istrOldPaymentOption = Convert.ToString(icdoPaymentRecovery.ihstOldValues["payment_option_value"]);
                idecOldGrossReductionAmount = Convert.ToDecimal(icdoPaymentRecovery.ihstOldValues["gross_reduction_amount"]);
                iintOldPayeeAccount = Convert.ToInt32(icdoPaymentRecovery.ihstOldValues["payee_account_id"]);
            }
            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (icdoPaymentRecovery.status_value == busConstant.RecoveryStatusApproved || icdoPaymentRecovery.status_value == busConstant.RecoveryStatusInProcess)
            {
                UpdatePensionReceivableinPAPITWhenPayeeAccountChanged();
                UpdatePensionReceivableinPAPIT();
            }
        }
       
        /// <summary>
        /// Method to calculate GrossReductionAmount on Approve of Recovery
        /// </summary>
        public bool CalculateGrossReductionAmount()
        {
            bool lblnResult = true;
            decimal ldecMemberAge = 0.0M, ldecJointAnnuitantAge = 0.0M;
            if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReduction)
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.ibusPayee == null)
                    ibusPayeeAccount.LoadPayee();
                if (ibusPayeeAccount.ibusJointAnnuitant == null)
                    ibusPayeeAccount.LoadJointAnnuitant();
                if (ibusPayeeAccount.ibusApplication == null)
                    ibusPayeeAccount.LoadApplication();
                int lintMonths, lintMemberAgeYear, lintMemberAgeMonths;
                //loading member age
                if (ibusPayeeAccount.ibusPayee.icdoPerson.person_id > 0)
                {
                    lintMonths = lintMemberAgeMonths = lintMemberAgeYear = 0;
                    busPersonBase.CalculateAge(ibusPayeeAccount.ibusPayee.icdoPerson.date_of_birth.AddMonths(1),
                                                    icdoPaymentRecovery.effective_date,
                                                    ref lintMonths, ref ldecMemberAge, 6, ref lintMemberAgeYear, ref lintMemberAgeMonths);
                }
                //loading joint annuitant age
                if (ibusPayeeAccount.ibusJointAnnuitant.icdoPerson.person_id > 0 &&
                    ibusPayeeAccount.ibusJointAnnuitant.icdoPerson.person_id != ibusPayeeAccount.ibusPayee.icdoPerson.person_id)
                {
                    lintMonths = lintMemberAgeMonths = lintMemberAgeYear = 0;
                    busPersonBase.CalculateAge(ibusPayeeAccount.ibusJointAnnuitant.icdoPerson.date_of_birth.AddMonths(1),
                                                    icdoPaymentRecovery.effective_date,
                                                    ref lintMonths, ref ldecJointAnnuitantAge, 6, ref lintMemberAgeYear, ref lintMemberAgeMonths);
                }
                busPaymentLifeTimeReductionRef lobjLTR = new busPaymentLifeTimeReductionRef();

                int lintNoofPayments = 0;
                lintNoofPayments = lobjLTR.GetNumberofPaymentsForRecovery(ibusPayeeAccount.icdoPayeeAccount.benefit_option_value,
                                            ibusPayeeAccount.ibusPayee.icdoPerson.gender_value,
                                            ldecMemberAge,
                                            ldecJointAnnuitantAge, ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value);
                if (lintNoofPayments <= 0)
                    lblnResult = false;
                else
                    icdoPaymentRecovery.gross_reduction_amount = icdoPaymentRecovery.recovery_amount / Convert.ToDecimal(lintNoofPayments);
            }
            else if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLumpSum)
            {
                icdoPaymentRecovery.gross_reduction_amount = icdoPaymentRecovery.recovery_amount;
            }
            return lblnResult;
        }

        //Property to contain estimated end date for recovery
        public DateTime idtEstimatedEndDate { get; set; }
        /// <summary>
        /// Method to calculate estimated end date for recovery based on Gross Reduction amount
        /// </summary>
        public void CalculateEstimatedEndDate()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (idecLTDPrincipleAmountPaid <= 0)
                LoadLTDPrincipleAmountPaid();
            if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeRepayOverTime && icdoPaymentRecovery.gross_reduction_amount != 0)
            {
                int lintMonths =
                    Convert.ToInt32(Math.Round((icdoPaymentRecovery.recovery_amount - idecLTDPrincipleAmountPaid) /
                    icdoPaymentRecovery.gross_reduction_amount, MidpointRounding.AwayFromZero));
                idtEstimatedEndDate = ibusPayeeAccount.idtNextBenefitPaymentDate.AddMonths(lintMonths);
            }
            else if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLumpSum)
                idtEstimatedEndDate = DateTime.MinValue;
        }

        /// <summary>
        /// Method to check whether user entered Payee account ID is valid or not
        /// </summary>
        /// <returns>bool</returns>
        public bool IsPayeeNotValid()
        {
            bool lblnResult = false;
            if (ibusBenefitOverpaymentHeader == null)
                LoadBenefitOverpaymentHeader();
            ibusBenefitOverpaymentHeader.LoadPayeeAccount();
            LoadPayeeAccount();
            ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            string lstrStatus =
                busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
            //if overpayemnt payee account id is not equal to entered payee account id in recovery
            if (ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.payee_account_id != icdoPaymentRecovery.payee_account_id)
            {
                //if PAS is not in Approved or Receiving
                if (icdoPaymentRecovery.payment_option_value != busConstant.RecoveryPaymentOptionPersonalChk && 
                    lstrStatus != busConstant.PayeeAccountStatusApproved && lstrStatus != busConstant.PayeeAccountStatusReceiving)
                {
                    lblnResult = true;
                }
                else
                {
                    //Different benefit account Id
                    if (ibusBenefitOverpaymentHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_account_id !=
                        ibusPayeeAccount.icdoPayeeAccount.benefit_account_id)
                    {
                        ibusBenefitOverpaymentHeader.ibusPayeeAccount.LoadApplication();
                        ibusPayeeAccount.LoadApplication();
                        //Same person id and plan id
                        if ((ibusBenefitOverpaymentHeader.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id !=
                            ibusPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id) ||
                            (ibusBenefitOverpaymentHeader.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id !=
                            ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id))
                        {
                            lblnResult = true;
                        }
                    }
                }
            }
            else
            {
                //if PAS is Cancelled
                if (icdoPaymentRecovery.payment_option_value != busConstant.RecoveryPaymentOptionPersonalChk && lstrStatus == busConstant.PayeeAccountStatusCancelled)
                {
                    lblnResult = true;
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// Method to check whether payment option is changed for Repayment Type Life time reduction
        /// </summary>
        /// <returns>bool</returns>
        public bool IsPaymentOptionChanged()
        {
            bool lblnResult = false;
            if (icdoPaymentRecovery.ihstOldValues.Count > 0 &&
                (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReduction ||
                icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReductionForRTW))
            {
                if (Convert.ToString(icdoPaymentRecovery.ihstOldValues["payment_option_value"]) != string.Empty &&
                    Convert.ToString(icdoPaymentRecovery.ihstOldValues["payment_option_value"]) !=
                    icdoPaymentRecovery.payment_option_value)
                    lblnResult = true;
            }
            return lblnResult;
        }

        /// <summary>
        /// Method to check whether payee account is changed for payment option other than NDPERS pension check
        /// </summary>
        /// <returns></returns>
        public bool IsPayeeAccountChanged()
        {
            bool lblnResult = false;
            if (icdoPaymentRecovery.ihstOldValues.Count > 0 && icdoPaymentRecovery.payment_option_value != busConstant.RecoveryPaymentOptionNDPERSPensionChk)
            {
                if (Convert.ToInt32(icdoPaymentRecovery.ihstOldValues["payee_account_id"]) !=
                                   icdoPaymentRecovery.payee_account_id)
                    lblnResult = true;
            }
            return lblnResult;
        }

        //Property to contain available remittance
        public Collection<busRemittance> iclbAvailableRemittance { get; set; }
        /// <summary>
        /// Method to load available remittance amount for Benefit Payback
        /// </summary>
        public void LoadAvailableRemittance()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();

            DataTable ldtAvailableRemittance = null;
            if (ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0)
            {
                ldtAvailableRemittance = Select<cdoRemittance>
                    (new string[2] { enmRemittance.remittance_type_value.ToString(), enmRemittance.person_id.ToString() },
                    new object[2] { busConstant.ItemTypeBenifitPayback, ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id }, null, null);
            }
            else
            {
                ldtAvailableRemittance = Select<cdoRemittance>
                    (new string[2] { enmRemittance.remittance_type_value.ToString(), enmRemittance.org_id.ToString() },
                    new object[2] { busConstant.ItemTypeBenifitPayback, ibusPayeeAccount.icdoPayeeAccount.payee_org_id }, null, null);
            }
            iclbAvailableRemittance = new Collection<busRemittance>();
            //Start PIR-8796
            if (ibusPayeeAccount.icdoPayeeAccount.application_id != 0)
            {
                ibusPayeeAccount.LoadApplication();
            }
            else
            {
                ibusPayeeAccount.LoadDROApplication();
            }
            //End PIR-8796
            
            foreach (DataRow dr in ldtAvailableRemittance.Rows)
            {
                busRemittance lobjRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
                //Loading deposit object and checking whether its in applied status
                busDeposit lobjDeposit = new busDeposit();
                lobjDeposit.FindDeposit(Convert.ToInt32(dr["deposit_id"]));
                if (lobjDeposit.icdoDeposit.status_value == busConstant.DepositDetailStatusApplied)
                {   
                    //PIR-8796
                    //To include the remiitance associated with the respective Payee Account id.
                    if ((ibusPayeeAccount.ibusApplication != null && ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id == Convert.ToInt32(dr["plan_id"]))
                        || (ibusPayeeAccount.ibusDROApplication != null && ibusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.plan_id == Convert.ToInt32(dr["plan_id"])))
                    {
                        //getting the available remittance amount
                        dr["remittance_amount"] = busEmployerReportHelper.GetRemittanceAvailableAmount(Convert.ToInt32(dr["remittance_id"]));
                        if (Convert.ToDecimal(dr["remittance_amount"]) > 0)
                        {
                            lobjRemittance.icdoRemittance.LoadData(dr);
                            iclbAvailableRemittance.Add(lobjRemittance);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method called when Allocate amount is clicked
        /// </summary>
        /// <returns>Arraylist containing updated bus. object</returns>
        public ArrayList btn_AllocateAmount_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            decimal ldecAmortizationInterest = 0.0M;
            bool lblnIsError = false;

            if (ibusBenefitOverpaymentHeader == null)
                LoadBenefitOverpaymentHeader();

            if (icdoPaymentRecovery.status_value != busConstant.RecoveryStatusApproved && icdoPaymentRecovery.status_value != busConstant.RecoveryStatusInProcess)
            {
                lobjError = AddError(6726, string.Empty);
                larrList.Add(lobjError);
                lblnIsError = true;
            }
            //PIR 21523
            if (iclbAvailableRemittance == null)
                LoadAvailableRemittance();
            foreach (busRemittance lobjRemittance in iclbAvailableRemittance)
            {
                busPaymentRecoveryHistory lobjRecoveryHistory = new busPaymentRecoveryHistory { icdoPaymentRecoveryHistory = new cdoPaymentRecoveryHistory() };
                //Loading total amount paid till now
                LoadLTDAmountPaid();
                //loading total principal amount paid
                LoadLTDPrincipleAmountPaid();
                //Loading lastest posted date in history
                LoadLastPostedDate();
                if (lobjRemittance.icdoRemittance.idtAppliedDate == DateTime.MinValue)
                {
                    lobjError = AddError(6707, string.Empty);
                    larrList.Add(lobjError);
                    lblnIsError = true;
                }
                if (lobjRemittance.icdoRemittance.idecGrossReductionAmount == 0.0M)
                {
                    lobjError = AddError(6708, string.Empty);
                    larrList.Add(lobjError);
                    lblnIsError = true;
                }
                if (icdoPaymentRecovery.recovery_amount - idecLTDPrincipleAmountPaid < lobjRemittance.icdoRemittance.idecGrossReductionAmount)
                {
                    lobjError = AddError(6723, string.Empty);
                    larrList.Add(lobjError);
                    lblnIsError = true;
                }
                if (lobjRemittance.icdoRemittance.idecGrossReductionAmount > lobjRemittance.icdoRemittance.remittance_amount)
                {
                    lobjError = AddError(6709, string.Empty);
                    larrList.Add(lobjError);
                    lblnIsError = true;
                }
                //Calculating Amortization interest
                if (ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.amortized_interest_flag == busConstant.Flag_Yes)
                {
                    ldecAmortizationInterest = busInterestCalculationHelper.CalculateAmortizationInterest(lobjRemittance.icdoRemittance.idtAppliedDate,
                                                                                    idtLastPostedDate,
                                                                                    icdoPaymentRecovery.recovery_amount - idecLTDPrincipleAmountPaid);
                }
                if (ldecAmortizationInterest > lobjRemittance.icdoRemittance.idecGrossReductionAmount)
                {
                    string lstrWhere = "message_id = 6727";
                    DataTable ldtMessage = iobjPassInfo.isrvDBCache.GetCacheData("sgs_messages", lstrWhere);
                    if (ldtMessage.Rows.Count > 0)
                    {
                        string lstrMessage = ldtMessage.Rows[0]["display_message"].ToString();
                        lobjError = AddError(0, string.Format(lstrMessage, ldecAmortizationInterest.ToString()));
                        larrList.Add(lobjError);
                    }
                    lblnIsError = true;
                }
                if (!lblnIsError)
                {
                    //checking amount paid and if first payment making recovery status to Inprocess
                    if (idecLTDPrincipleAmountPaid == 0)
                    {
                        //Creating GL
                        busGLHelper.GenerateGL(busConstant.GLStatusTransitionRecovery, icdoPaymentRecovery.status_value,
                                                busConstant.RecoveryStatusInProcess, busConstant.GLTransactionTypeStatusTransition,
                                                busConstant.GLSourceTypeValuePensionRecv, null, this, iobjPassInfo);
                        icdoPaymentRecovery.status_value = busConstant.RecoveryStatusInProcess;
                        icdoPaymentRecovery.Update();
                    }

                    lobjRecoveryHistory.icdoPaymentRecoveryHistory.payment_recovery_id = icdoPaymentRecovery.payment_recovery_id;
                    lobjRecoveryHistory.icdoPaymentRecoveryHistory.posted_date = lobjRemittance.icdoRemittance.idtAppliedDate;
                    lobjRecoveryHistory.icdoPaymentRecoveryHistory.remittance_id = lobjRemittance.icdoRemittance.remittance_id;
                    lobjRecoveryHistory.icdoPaymentRecoveryHistory.amortization_interest_paid = ldecAmortizationInterest;
                    lobjRecoveryHistory.icdoPaymentRecoveryHistory.principle_amount_paid =
                        lobjRemittance.icdoRemittance.idecGrossReductionAmount - lobjRecoveryHistory.icdoPaymentRecoveryHistory.amortization_interest_paid;
                    lobjRecoveryHistory.icdoPaymentRecoveryHistory.allocated_amount = lobjRemittance.icdoRemittance.idecGrossReductionAmount;
                    lobjRecoveryHistory.icdoPaymentRecoveryHistory.Insert();
                    //Generating item level GL
                    if (ldecAmortizationInterest > 0)
                        GenerateItemLevelGL(ldecAmortizationInterest, lobjRemittance.icdoRemittance.plan_id,lobjRemittance.icdoRemittance.person_id,
                            lobjRemittance.icdoRemittance.org_id);
                }
                else
                    break;
            }
            if (!lblnIsError)
            {
                LoadPaymentRecoveryHistory();
                LoadLTDAmortizationInterestPaid();
                LoadLTDAmountPaid();
                LoadLTDPrincipleAmountPaid();
                CalculateEstimatedEndDate();
                //checking amount paid and if full payment is done, change recovery status to Satisfied
                if (idecLTDPrincipleAmountPaid == icdoPaymentRecovery.recovery_amount)
                {
                    if (ibusPayeeAccount == null)
                        LoadPayeeAccount();
                    if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        ibusPayeeAccount.LoadNexBenefitPaymentDate();
                    busPayeeAccountPaymentItemType lobjPAPIT = ibusPayeeAccount.GetLatestPayeeAccountPaymentItemType(busConstant.PAPITPensionReceivable);
                    if (lobjPAPIT != null && icdoPaymentRecovery.payee_account_payment_item_type_id > 0 && IsPAPITExistsAfterNextBenefitPaymentDate(lobjPAPIT))
                        icdoPaymentRecovery.payee_account_payment_item_type_id = 0;  
                    icdoPaymentRecovery.status_value = busConstant.RecoveryStatusSatisfied;
                    icdoPaymentRecovery.gross_reduction_amount = 0; //PIR 9220
                    icdoPaymentRecovery.Update();
                    if (lobjPAPIT != null)
                    {
                        //updating the PAPIT entry if paid by personal check
                        icdoPaymentRecovery.payee_account_payment_item_type_id = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPensionReceivable,
                                                                            (lobjPAPIT.icdoPayeeAccountPaymentItemType.amount - icdoPaymentRecovery.gross_reduction_amount),
                                                                            string.Empty,
                                                                            lobjPAPIT.icdoPayeeAccountPaymentItemType.vendor_org_id,
                                                                            ibusPayeeAccount.idtNextBenefitPaymentDate,
                                                                            DateTime.MinValue, false);
                    }
                    //Creating GL
                    busGLHelper.GenerateGL(busConstant.GLStatusTransitionRecovery, icdoPaymentRecovery.status_value,
                                            busConstant.RecoveryStatusSatisfied, busConstant.GLTransactionTypeStatusTransition,
                                            busConstant.GLSourceTypeValuePensionRecv, null, this, iobjPassInfo);
                }
                icdoPaymentRecovery.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(icdoPaymentRecovery.status_id, icdoPaymentRecovery.status_value);
                larrList.Add(this);
            }
            return larrList;
        }

        // PROD PIR 8574 - The System tries to delete the future dated PAPIT and failed due to FK reference in Payment recovery table. This method will updates the PAPIT ID in Recovery table.
        private bool IsPAPITExistsAfterNextBenefitPaymentDate(busPayeeAccountPaymentItemType aobjPAPIT)
        {
            if (aobjPAPIT.IsNotNull())
            {
                if (ibusPayeeAccount.IsNull()) LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue) ibusPayeeAccount.LoadNexBenefitPaymentDate();
                if (aobjPAPIT.icdoPayeeAccountPaymentItemType.amount - icdoPaymentRecovery.gross_reduction_amount == 0M &&
                    aobjPAPIT.icdoPayeeAccountPaymentItemType.start_date >= ibusPayeeAccount.idtNextBenefitPaymentDate)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Method to create item level GL for amortization interest
        /// </summary>
        /// <param name="adecAmortizationInterest">Amortization interest</param>
        /// <param name="aintPlanID">Plan ID</param>
        public void GenerateItemLevelGL(decimal adecAmortizationInterest, int aintPlanID, int aintPersonID, int aintOrgID)
        {
            cdoAccountReference lcdoAccountReference = new cdoAccountReference();
            lcdoAccountReference.plan_id = aintPlanID;
            lcdoAccountReference.source_type_value = busConstant.GLSourceTypeValueRecovery;
            lcdoAccountReference.transaction_type_value = busConstant.TransactionTypeItemLevel;
            lcdoAccountReference.item_type_value = busConstant.PaymentItemCodeValueAmortizedInterest;
            lcdoAccountReference.status_transition_value = null;
            //Generating GL
            busGLHelper.GenerateGL(lcdoAccountReference, aintPersonID, aintOrgID, icdoPaymentRecovery.payment_recovery_id, 
                adecAmortizationInterest, DateTime.Now, DateTime.Now, iobjPassInfo);
        }

        /// <summary>
        /// Method to load all Payment Recovery History
        /// </summary>
        public void LoadPaymentRecoveryHistory()
        {
            DataTable ldtRecoveryHistory = Select("cdoPaymentRecoveryHistory.LoadRecoveryHistory", new object[1] { icdoPaymentRecovery.payment_recovery_id });
            iclbPaymentRecoveryHistory = GetCollection<busPaymentRecoveryHistory>(ldtRecoveryHistory, "icdoPaymentRecoveryHistory");
        }

        //Create recovery History

        public void CreateRecoveryHistory(DateTime adtPostedDate, int aintRemittanceID, int aintPaymentHistoryHeaderID,
            decimal adecPrincipleAmountPaid, decimal adecAmortizationInterest, decimal adecAllocatedAmount)
        {
            cdoPaymentRecoveryHistory lcdoPaymentRecoveryHistory = new cdoPaymentRecoveryHistory();
            lcdoPaymentRecoveryHistory.payment_recovery_id = icdoPaymentRecovery.payment_recovery_id;
            lcdoPaymentRecoveryHistory.posted_date = adtPostedDate;
            lcdoPaymentRecoveryHistory.remittance_id = aintRemittanceID;
            lcdoPaymentRecoveryHistory.payment_history_header_id = aintPaymentHistoryHeaderID;
            lcdoPaymentRecoveryHistory.principle_amount_paid = adecPrincipleAmountPaid;
            lcdoPaymentRecoveryHistory.amortization_interest_paid = adecAmortizationInterest;
            lcdoPaymentRecoveryHistory.allocated_amount = adecAllocatedAmount;
            lcdoPaymentRecoveryHistory.Insert();
        }

        /// <summary>
        /// method to update payee account id in papit
        /// </summary>
        private void UpdatePensionReceivableinPAPITWhenPayeeAccountChanged()
        {
            LoadPayeeAccount();
            ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (iintOldPayeeAccount > 0 && iintOldPayeeAccount != ibusPayeeAccount.icdoPayeeAccount.payee_account_id)
            {
                busPayeeAccount lobjOldPA = new busPayeeAccount();
                lobjOldPA.FindPayeeAccount(iintOldPayeeAccount);
                busPayeeAccountPaymentItemType lobjOldPAPITPension = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };                
               
                icdoPaymentRecovery.payee_account_payment_item_type_id = 0;
                icdoPaymentRecovery.Update();
                if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReduction ||
                    icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReductionForRTW)
                {
                    lobjOldPAPITPension = lobjOldPA.GetLatestPayeeAccountPaymentItemType(busConstant.PAPITTaxableAmount);
                    lobjOldPA.CreatePayeeAccountPaymentItemType(lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.payment_item_type_id,
                        lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.amount + icdoPaymentRecovery.gross_reduction_amount,
                        lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.account_number,
                        lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.vendor_org_id,
                        ibusPayeeAccount.idtNextBenefitPaymentDate,
                        DateTime.MinValue, false);

                    busPayeeAccountPaymentItemType lobjNewPAPIT = ibusPayeeAccount.GetLatestPayeeAccountPaymentItemType(busConstant.PAPITTaxableAmount);
                    icdoPaymentRecovery.payee_account_payment_item_type_id = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lobjNewPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id,
                        lobjNewPAPIT.icdoPayeeAccountPaymentItemType.amount - icdoPaymentRecovery.gross_reduction_amount,
                        lobjNewPAPIT.icdoPayeeAccountPaymentItemType.account_number,
                        lobjNewPAPIT.icdoPayeeAccountPaymentItemType.vendor_org_id,
                        ibusPayeeAccount.idtNextBenefitPaymentDate,
                        DateTime.MinValue, false);
                }
                else
                {
                    lobjOldPAPITPension = lobjOldPA.GetLatestPayeeAccountPaymentItemType(busConstant.PAPITPensionReceivable);
                    if (lobjOldPAPITPension != null)
                    {
                        lobjOldPA.CreatePayeeAccountPaymentItemType(lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.payment_item_type_id,
                            0.0M, lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.account_number,
                            lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.vendor_org_id,
                            ibusPayeeAccount.idtNextBenefitPaymentDate,
                            DateTime.MinValue, false);

                        icdoPaymentRecovery.payee_account_payment_item_type_id = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.payment_item_type_id,
                            lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.amount, 
                            lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.account_number,
                            lobjOldPAPITPension.icdoPayeeAccountPaymentItemType.vendor_org_id,
                            ibusPayeeAccount.idtNextBenefitPaymentDate,
                            DateTime.MinValue, false);
                    }
                }                
                icdoPaymentRecovery.Update();
                ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            }            
        }

        /// <summary>
        /// Method to update Pension Recv. item amount in PAPIT table
        /// </summary>
        public void UpdatePensionReceivableinPAPIT()
        {
            bool lblnTrue = true;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeRepayOverTime ||
                icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLumpSum)
            {
                busPayeeAccountPaymentItemType lobjPAPITPension = ibusPayeeAccount.GetLatestPayeeAccountPaymentItemType(busConstant.PAPITPensionReceivable);
                decimal ldecAmount = lobjPAPITPension != null ?
                                        lobjPAPITPension.icdoPayeeAccountPaymentItemType.amount :
                                        0.0M;
                //IF payment option is changed
                if (!string.IsNullOrEmpty(istrOldPaymentOption) && istrOldPaymentOption != icdoPaymentRecovery.payment_option_value)
                {
                    icdoPaymentRecovery.payee_account_payment_item_type_id = 0;
                    icdoPaymentRecovery.Update();
                    //if payment option is NDPERS pension check, create an entry in PAPIT
                    if (icdoPaymentRecovery.payment_option_value == busConstant.RecoveryPaymentOptionNDPERSPensionChk)
                        icdoPaymentRecovery.payee_account_payment_item_type_id = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPensionReceivable,
                                                                        icdoPaymentRecovery.gross_reduction_amount + ldecAmount,
                                                                        string.Empty,
                                                                        0,
                                                                        ibusPayeeAccount.idtNextBenefitPaymentDate,
                                                                        DateTime.MinValue, false);
                    //if payment option is Personal check, end date the old entry in PAPIT
                    else if (lobjPAPITPension.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id > 0)
                        icdoPaymentRecovery.payee_account_payment_item_type_id = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPensionReceivable,
                                                                        ldecAmount - idecOldGrossReductionAmount,
                                                                        string.Empty,
                                                                        0,
                                                                        ibusPayeeAccount.idtNextBenefitPaymentDate,
                                                                        DateTime.MinValue, false);
                    lblnTrue = false;
                    icdoPaymentRecovery.Update();
                }
                //IF gross reduction amount is changed   
                if (lblnTrue && idecOldGrossReductionAmount != icdoPaymentRecovery.gross_reduction_amount)
                {
                    //if payment option is NDPERS pension check and amount changed, end date old entry and create new entry in PAPIT
                    if (icdoPaymentRecovery.payment_option_value == busConstant.RecoveryPaymentOptionNDPERSPensionChk)
                    {
                        icdoPaymentRecovery.payee_account_payment_item_type_id = 0;
                        icdoPaymentRecovery.Update();
                        icdoPaymentRecovery.payee_account_payment_item_type_id = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPensionReceivable,
                                                                            ldecAmount + (icdoPaymentRecovery.gross_reduction_amount - idecOldGrossReductionAmount),
                                                                            string.Empty,
                                                                            0,
                                                                            ibusPayeeAccount.idtNextBenefitPaymentDate,
                                                                            DateTime.MinValue, false);
                        icdoPaymentRecovery.Update();
                    }
                }
            }
            ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
        }

        /// <summary>
        /// Method to check whether gross reduction amount is valid
        /// </summary>
        /// <returns>bool</returns>
        public bool IsGrossReductionAmountValid()
        {
            bool lblnResult = false;

            if (idecLTDPrincipleAmountPaid == 0)
                LoadLTDPrincipleAmountPaid();
            if (ibusBenefitOverpaymentHeader == null)
                LoadBenefitOverpaymentHeader();
            if (ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.adjustment_reason_value == busConstant.BenRecalAdjustmentReasonRecalculation &&
                (string.IsNullOrEmpty(icdoPaymentRecovery.status_value) || icdoPaymentRecovery.status_value == busConstant.RecoveryStatusPendingApproval))
                lblnResult = true;
            else if ((icdoPaymentRecovery.recovery_amount - idecLTDPrincipleAmountPaid) >= icdoPaymentRecovery.gross_reduction_amount)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsAvailableRemittanceNotNull()
        {
            bool lblnResult = false;
            if (iclbAvailableRemittance != null && iclbAvailableRemittance.Count > 0)
                lblnResult = true;
            return lblnResult;
        }

        //properties used for correspondence

        public string istrBillingDate { get; set; }

        public string istrDueDate { get; set; }

        public string IsPastDueValid { get; set; }

        public decimal idecDueAmount { get; set; }
        //Load and set the correspondence property values
        public void LoadCorValues()
        {
            IsPastDueValid = busConstant.Flag_Yes;
            DateTime ldtNextMonth = new DateTime(DateTime.Today.AddMonths(1).Year, DateTime.Today.AddMonths(1).Month, 1);
            istrBillingDate = ldtNextMonth.ToString("MMMM, yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            istrDueDate = ldtNextMonth.ToString("MMMM dd, yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            LoadPaymentRecoveryHistory();
            if (icdoPaymentRecovery.effective_date.Month == ldtNextMonth.Month)
            {
                IsPastDueValid = busConstant.Flag_No;
            }
            else if (
                iclbPaymentRecoveryHistory.Where(o => busGlobalFunctions.CheckDateOverlapping(
                    o.icdoPaymentRecoveryHistory.posted_date, ldtNextMonth.AddMonths(-1), ldtNextMonth)).Count() == 0)
            {
                IsPastDueValid = busConstant.Flag_No;
            }
            LoadLTDPrincipleAmountPaid();
            idecDueAmount = icdoPaymentRecovery.recovery_amount - idecLTDPrincipleAmountPaid;
        }
        public busPerson ibusPerson { get; set; }
        public void LoadPerson()
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoPaymentRecovery.person_id);
        }
        public override busBase GetCorPerson()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayee == null)
                ibusPayeeAccount.LoadPayee();
            return ibusPayeeAccount.ibusPayee;
        }

        public override bool SeBpmActivityInstanceReferenceID()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                ibusPayeeAccount.ibusBaseActivityInstance = ibusBaseActivityInstance;
                ibusPayeeAccount.SeBpmActivityInstanceReferenceID();
                // ibusPayeeAccount.SetProcessInstanceParameters();
                ibusPayeeAccount.SetCaseInstanceParameters();
            }
            return true;
        }
    }
}
