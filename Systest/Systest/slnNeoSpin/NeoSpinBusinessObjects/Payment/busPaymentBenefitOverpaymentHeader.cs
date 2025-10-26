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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busPaymentBenefitOverpaymentHeader:
	/// Inherited from busPaymentBenefitOverpaymentHeaderGen, the class is used to customize the business object busPaymentBenefitOverpaymentHeaderGen.
	/// </summary>
	[Serializable]
	public class busPaymentBenefitOverpaymentHeader : busPaymentBenefitOverpaymentHeaderGen
	{
        public busPaymentRecovery ibusPaymentRecovery { get; set; }

        public bool iblnIsNewMode { get; set; }

        public void LoadPaymentRecovery()
        {
            if (ibusPaymentRecovery == null)
                ibusPaymentRecovery = new busPaymentRecovery();
            ibusPaymentRecovery.LoadPaymentRecoveryByBenefitOverpaymentID(icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id);
        }

        //Property to contain all Recovery details for the BenefitOverpayment
        public Collection<busPaymentRecovery> iclbPaymentRecovery { get; set; }
        /// <summary>
        /// Method to load all Recovery details for benefit overpayment
        /// </summary>
        public void LoadPaymentRecoveries()
        {
            DataTable ldtPaymentRecovery = Select<cdoPaymentRecovery>
                (new string[1] { enmPaymentRecovery.benefit_overpayment_id.ToString() },
                new object[1] { icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id }, null, null);
            iclbPaymentRecovery = GetCollection<busPaymentRecovery>(ldtPaymentRecovery, "icdoPaymentRecovery");
            foreach (busPaymentRecovery lobjRecovery in iclbPaymentRecovery)
            {
                lobjRecovery.LoadLTDAmortizationInterestPaid();
                lobjRecovery.LoadLTDAmountPaid();
                lobjRecovery.LoadLTDPrincipleAmountPaid();
                lobjRecovery.CalculateEstimatedEndDate();
            }
        }

        //Property to contain Payment History of a payee account
        public Collection<busPaymentHistoryHeader> iclbPaymentHistoryHeader { get; set; }

        /// <summary>
        /// Method to load Payment History collection
        /// </summary>
        public void LoadPaymentHistoryHeader(int aintPayeeAccountID)
        {
            if (iclbPaymentHistoryHeader == null)
                iclbPaymentHistoryHeader = new Collection<busPaymentHistoryHeader>();
            DataTable ldtPaymentHistory = Select<cdoPaymentHistoryHeader>(new string[1] { enmPayeeAccount.payee_account_id.ToString()},
                                                                    new object[1] { aintPayeeAccountID },
                                                                    null, "PAYMENT_HISTORY_HEADER_ID desc");            
            iclbPaymentHistoryHeader = GetCollection<busPaymentHistoryHeader>(ldtPaymentHistory, "icdoPaymentHistoryHeader");
            foreach (busPaymentHistoryHeader lobjHeader in iclbPaymentHistoryHeader)
            {
                lobjHeader.LoadPaymentHistoryDetails();
            }
        }

        //Property to contain Gross Taxable amount Paid for the year
        public decimal idecGrossTaxableAmountPaidFTY { get; set; }
        /// <summary>
        /// Method to load GrossTaxable amount paid for the year
        /// </summary>
        public decimal LoadGrossTaxableAmountFTY(int aintYear)
        {
            decimal ldecGrossTaxableAmountPaid = 0.0M;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader(ibusPayeeAccount.icdoPayeeAccount.payee_account_id);
            IEnumerable<busPaymentHistoryHeader> lenmPaymentHeader = iclbPaymentHistoryHeader
                                                                        .Where(o => o.icdoPaymentHistoryHeader.payment_date.Year == (DateTime.Today.Year - aintYear) &&
                                                                            (o.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusOutstanding ||
                                                                            o.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusProcessed));

            foreach (busPaymentHistoryHeader lobjPaymentHeader in lenmPaymentHeader)
            {
                //Gross Taxable Amount Paid for the current Year
                ldecGrossTaxableAmountPaid += lobjPaymentHeader.iclbPaymentHistoryDetail
                                                                .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                                                                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                                                                    o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_No &&
                                                                    o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes &&
                                                                    o.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RollItemForCheck)
                                                                .Sum(o => o.icdoPaymentHistoryDetail.amount);
            }
            if (aintYear == 0)
                idecGrossTaxableAmountPaidFTY = ldecGrossTaxableAmountPaid;
            return ldecGrossTaxableAmountPaid;
        }

        //Property to store total amount due from payee
        public decimal idecTotalAmountDueFromPayee { get; set; }
        /// <summary>
        /// Method to load total amount due from payee
        /// </summary>
        public void LoadTotalAmountDue()
        {
            if (iclbPaymentBenefitOverpaymentDetail == null)
                LoadPaymentBenefitOverpaymentDetails();
            idecTotalAmountDueFromPayee = iclbPaymentBenefitOverpaymentDetail
                                            .Sum(o => o.icdoPaymentBenefitOverpaymentDetail.amount);
            if (idecTotalAmountDueFromPayee <= 0)
            {
                if (iclbMonthwiseAdjustmentDetails == null)
                    LoadMonthwiseAdjustmentDetails();
                idecTotalAmountDueFromPayee = iclbMonthwiseAdjustmentDetails
                .Sum(o => o.icdoPayeeAccountMonthwiseAdjustmentDetail.amount + o.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount);
            }
        }

        public busPayeeAccount ibusPayeeAccount { get; set; }
        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount();
            ibusPayeeAccount.FindPayeeAccount(icdoPaymentBenefitOverpaymentHeader.payee_account_id);
        }

        /// <summary>
        /// Method to calculate and insert interest for benefit overpayment
        /// </summary>
        public void CalculateAndInsertMonthwiseAdjustmentInterest()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (iclbMonthwiseAdjustmentDetails == null)
                LoadMonthwiseAdjustmentDetails();
            ibusPayeeAccount.LoadNexBenefitPaymentDate();
            
            //Updating interest amount in payee account monthwise adjustment details
            foreach (busPayeeAccountMonthwiseAdjustmentDetail lobjAdjustments in iclbMonthwiseAdjustmentDetails)
            {
                lobjAdjustments.CalculateInterest(ibusPayeeAccount.idtNextBenefitPaymentDate);
                lobjAdjustments.icdoPayeeAccountMonthwiseAdjustmentDetail.Update();                
            }
            AdjustInterestAmountInBenefitOverpaymentDetail(true);
        }

        /// <summary>
        /// Method to adjust interest in Benefit overpayment detail table
        /// </summary>
        /// <param name="ablnIsAdd">boolen value based on which interest is added or subtracted</param>        
        private void AdjustInterestAmountInBenefitOverpaymentDetail(bool ablnIsAdd)
        {
            decimal ldecInterest = 0.0M;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            //block to add interest amount to taxable amount in Benefit overpayment detail
            if (icdoPaymentBenefitOverpaymentHeader.adjustment_reason_value == busConstant.BenRecalAdjustmentReceivableCreated &&
                iclbMonthwiseAdjustmentDetails.Count > 0)
            {
                if (iclbPaymentBenefitOverpaymentDetail == null)
                    LoadPaymentBenefitOverpaymentDetails();
                IEnumerable<busPaymentBenefitOverpaymentDetail> lenmOverpaymentDetail = iclbPaymentBenefitOverpaymentDetail
                                                                    .Where(o => o.icdoPaymentBenefitOverpaymentDetail.payment_item_type_id ==
                                                                        ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITTaxableAmount));
                //if taxable item exists add interest amount and update
                foreach (busPaymentBenefitOverpaymentDetail lobjDetail in lenmOverpaymentDetail)
                {
                    ldecInterest = 0.0M;
                    ldecInterest = iclbMonthwiseAdjustmentDetails.Where(o => o.icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date.Year ==
                                                                                lobjDetail.icdoPaymentBenefitOverpaymentDetail.date_of_1099r.Year)
                                                                 .Sum(o => o.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount);
                    if (ldecInterest != 0)
                    {
                        if (ablnIsAdd)
                            lobjDetail.icdoPaymentBenefitOverpaymentDetail.amount += ldecInterest;
                        else
                            lobjDetail.icdoPaymentBenefitOverpaymentDetail.amount -= ldecInterest;
                        lobjDetail.icdoPaymentBenefitOverpaymentDetail.Update();
                    }
                }
            }            
        }        

        /// <summary>
        /// Method to create a new benefit overpayment detail
        /// </summary>
        /// <param name="adecAmount">Amount</param>
        /// <param name="aintPaymentItemTypeID">Payment Item type id</param>
        /// <param name="adtPaymentDate">Payment Date</param>
        public void CreateBenefitOverpaymentDetail(decimal adecAmount, int aintPaymentItemTypeID, DateTime adtPaymentDate, int aintVendorOrgID)
        {
            busPaymentBenefitOverpaymentDetail lobjOverPaymentDetail = new busPaymentBenefitOverpaymentDetail 
            { icdoPaymentBenefitOverpaymentDetail = new cdoPaymentBenefitOverpaymentDetail() };

            lobjOverPaymentDetail.icdoPaymentBenefitOverpaymentDetail.benefit_overpayment_id = icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id;
            lobjOverPaymentDetail.icdoPaymentBenefitOverpaymentDetail.payment_item_type_id = aintPaymentItemTypeID;
            lobjOverPaymentDetail.icdoPaymentBenefitOverpaymentDetail.amount = adecAmount;
            lobjOverPaymentDetail.icdoPaymentBenefitOverpaymentDetail.date_of_1099r = adtPaymentDate;
            lobjOverPaymentDetail.icdoPaymentBenefitOverpaymentDetail.vendor_org_id = aintVendorOrgID;            
            lobjOverPaymentDetail.icdoPaymentBenefitOverpaymentDetail.Insert();
        }

        /// <summary>
        /// Method to create Monthwise Adjustment Details
        /// </summary>
        /// <param name="adtEffectiveDate">Effective Date</param>
        /// <param name="adecAmount">Amount</param>
        /// <param name="adecInterest">Interest Amount</param>
        public void CreateMonthwiseAdjustmentDetail(DateTime adtEffectiveDate, decimal adecAmount, decimal adecInterest, int aintPayeeAccountID)
        {
            if (adecAmount != 0)
            {
                busPayeeAccountMonthwiseAdjustmentDetail lobjMonthwiseAdjustmentDetail = new busPayeeAccountMonthwiseAdjustmentDetail { icdoPayeeAccountMonthwiseAdjustmentDetail = new cdoPayeeAccountMonthwiseAdjustmentDetail() };

                lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.benefit_overpayment_id = icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id;
                lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date = adtEffectiveDate;
                lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.amount = adecAmount;
                lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount = adecInterest;
                lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.payee_account_id = aintPayeeAccountID;
                lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.Insert();
            }
        }

        /// <summary>
        /// Method to check whether any recovery is available for Benefit payment header
        /// </summary>
        /// <returns>Boolean value</returns>
        public bool IsRecoveryNotAvailble()
        {
            bool lblnResult = false;
            if (iclbPaymentRecovery == null)
                LoadPaymentRecoveries();
            if (iclbPaymentRecovery.Count == 0)
                lblnResult = true;
            else if (iclbPaymentRecovery[0].icdoPaymentRecovery.status_value == busConstant.RecoveryStatusCancel)
                lblnResult = true;
            return lblnResult;
        }

        //Property to contain Payee Account Monthwise Adjustment details for Overpayment
        public Collection<busPayeeAccountMonthwiseAdjustmentDetail> iclbMonthwiseAdjustmentDetails { get; set; }
        /// <summary>
        /// Method to load Payee account monthwise adjustment details for Overpayment
        /// </summary>
        public void LoadMonthwiseAdjustmentDetails()
        {
            DataTable ldtAdjustments = Select<cdoPayeeAccountMonthwiseAdjustmentDetail>
                (new string[1] { enmPayeeAccountMonthwiseAdjustmentDetail.benefit_overpayment_id.ToString() },
                new object[1] { icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id }, null, null);
            iclbMonthwiseAdjustmentDetails = new Collection<busPayeeAccountMonthwiseAdjustmentDetail>();
            iclbMonthwiseAdjustmentDetails = GetCollection<busPayeeAccountMonthwiseAdjustmentDetail>
                (ldtAdjustments, "icdoPayeeAccountMonthwiseAdjustmentDetail");
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            //If interest flag is checked, calculating interest for Monthwise adjustment details
            if (icdoPaymentBenefitOverpaymentHeader.calculate_interest_flag == busConstant.Flag_Yes)
                CalculateAndInsertMonthwiseAdjustmentInterest();
            else if (string.IsNullOrEmpty(icdoPaymentBenefitOverpaymentHeader.calculate_interest_flag) || icdoPaymentBenefitOverpaymentHeader.calculate_interest_flag == busConstant.Flag_No)
                ResetInterestFieldsInMonthwiseAdjustment();
            LoadTotalAmountDue();

            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            this.EvaluateInitialLoadRules();
        }

         public override bool SeBpmActivityInstanceReferenceID()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                ibusPayeeAccount.ibusBaseActivityInstance = ibusBaseActivityInstance;
                ibusPayeeAccount.SeBpmActivityInstanceReferenceID();
                //   ibusPayeeAccount.SetProcessInstanceParameters();
                ibusPayeeAccount.SetCaseInstanceParameters();
            }
            return true;
        }

        /// <summary>
        /// Method to reset monthwise adjustment interest
        /// </summary>
        private void ResetInterestFieldsInMonthwiseAdjustment()
        {           
            if (iclbMonthwiseAdjustmentDetails == null)
                LoadMonthwiseAdjustmentDetails();
            //adjust interest in benefitoverpayment detail table
            AdjustInterestAmountInBenefitOverpaymentDetail(false);
            //reseting interest amount in payee account monthwise adjustment details
            foreach (busPayeeAccountMonthwiseAdjustmentDetail lobjAdjustments in iclbMonthwiseAdjustmentDetails)
            {
                lobjAdjustments.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount = 0;
                lobjAdjustments.icdoPayeeAccountMonthwiseAdjustmentDetail.Update();
            }
        }

        /// <summary>
        /// Method to load data for Payee receivable summary report
        /// </summary>
        /// <returns></returns>
        public DataSet LoadPayeeReceivableSummaryReport()
        {
            DataSet ldsFinal = new DataSet();
            DataTable ldtFinal = new DataTable();

            ldtFinal = LoadDataForReport();
            ldtFinal.TableName = busConstant.ReportTableName;
            ldsFinal.Tables.Add(ldtFinal.Copy());

            return ldsFinal;
        }

        /// <summary>
        /// Method to merge data from two queries
        /// </summary>
        /// <returns>final data table</returns>
        private DataTable LoadDataForReport()
        {
            DataTable ldtWriteOffSummary = Select("cdoPaymentBenefitOverpaymentHeader.rptPayeeReceivableWriteOff", new object[0] { });
            DataTable ldtReceivableSummary = Select("cdoPaymentBenefitOverpaymentHeader.rptPayeeReceivableOtherThanSatisfiedAndCancel", new object[0] { });
            foreach (DataRow dr in ldtWriteOffSummary.Rows)
            {
                busPaymentRecovery lobjRecovery = new busPaymentRecovery();
                lobjRecovery.FindPaymentRecovery(Convert.ToInt32(dr["PAYMENT_RECOVERY_ID"]));
                lobjRecovery.CalculateEstimatedEndDate();
                if (lobjRecovery.idtEstimatedEndDate != DateTime.MinValue)
                    dr["ESTIMATED_END_DATE"] = lobjRecovery.idtEstimatedEndDate;
                ldtWriteOffSummary.AcceptChanges();
            }
            foreach (DataRow dr in ldtReceivableSummary.Rows)
            {
                DataRow ldrTemp = ldtWriteOffSummary.NewRow();
                ldrTemp["INDICATOR"] = dr["INDICATOR"];
                ldrTemp["PERSON_ID"] = dr["PERSON_ID"];
                ldrTemp["LAST_NAME"] = dr["LAST_NAME"];
                ldrTemp["FIRST_NAME"] = dr["FIRST_NAME"];
                ldrTemp["RECV_AMOUNT"] = dr["RECV_AMOUNT"];
                busPaymentRecovery lobjRecovery = new busPaymentRecovery();
                if (dr["PAYMENT_RECOVERY_ID"] != DBNull.Value)
                {
                    lobjRecovery.FindPaymentRecovery(Convert.ToInt32(dr["PAYMENT_RECOVERY_ID"]));
                    lobjRecovery.LoadLTDPrincipleAmountPaid();
                    lobjRecovery.CalculateEstimatedEndDate();
                    ldrTemp["PAID_AMOUNT"] = lobjRecovery.idecLTDPrincipleAmountPaid;
                    ldrTemp["REPAYMENT_TYPE"] = lobjRecovery.icdoPaymentRecovery.repayment_type_description;
                    ldrTemp["STATUS_DESCR"] = lobjRecovery.icdoPaymentRecovery.status_description;
                    ldrTemp["PAYMENT_OPTION"] = lobjRecovery.icdoPaymentRecovery.payment_option_description;
                    if (lobjRecovery.idtEstimatedEndDate != DateTime.MinValue)
                        ldrTemp["ESTIMATED_END_DATE"] = lobjRecovery.idtEstimatedEndDate;
                }
                ldtWriteOffSummary.Rows.Add(ldrTemp);
                ldtWriteOffSummary.AcceptChanges();
            }
            return ldtWriteOffSummary;
        }

        /// <summary>
        /// Method to initialize benefit overpayment detail in new mode
        /// </summary>
        public void InitializeBenefitOverpaymentDetails()
        {
            iclbPaymentBenefitOverpaymentDetail = new Collection<busPaymentBenefitOverpaymentDetail>();
            //taking from DB Cache
            DataTable ldtPaymentItem = iobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_item_type", null);
            DataTable ldtTaxableItem = ldtPaymentItem.AsEnumerable()
                                                    .Where(o => o.Field<string>("item_type_code") == busConstant.PAPITTaxableAmount)
                                                    .AsDataTable();
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ldtTaxableItem.Rows.Count > 0)
            {
                busPaymentItemType lobjPIT = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                lobjPIT.icdoPaymentItemType.LoadData(ldtTaxableItem.Rows[0]);
                for (int i = 0; i < 4; i++)
                {
                    busPaymentBenefitOverpaymentDetail lobjDetail = new busPaymentBenefitOverpaymentDetail { icdoPaymentBenefitOverpaymentDetail = new cdoPaymentBenefitOverpaymentDetail() };
                    lobjDetail.ibusPaymentItemType = lobjPIT;
                    lobjDetail.icdoPaymentBenefitOverpaymentDetail.payment_item_type_id = lobjPIT.icdoPaymentItemType.payment_item_type_id;
                    if (i == 0)
                    {
                        lobjDetail.icdoPaymentBenefitOverpaymentDetail.date_of_1099r = DateTime.Today;
                        iclbPaymentBenefitOverpaymentDetail.Add(lobjDetail);
                    }
                    else
                    {
                        ibusPayeeAccount.LoadLatest1099rByPayeeAccount(DateTime.Today.Year - i);
                        if (ibusPayeeAccount.ibusPayment1099r.icdoPayment1099r.payment_1099r_id > 0)
                        {
                            lobjDetail.icdoPaymentBenefitOverpaymentDetail.date_of_1099r = new DateTime(DateTime.Today.Year - i, 12, 01);
                            iclbPaymentBenefitOverpaymentDetail.Add(lobjDetail);
                        }
                    }
                }
            }
        }

        public bool IsBenefitOverpaymentExists()
        {
            bool lblnResult = false;

            return lblnResult;
        }

        public override int Delete()
        {
            if (iclbPaymentBenefitOverpaymentDetail == null)
                LoadPaymentBenefitOverpaymentDetails();
            if (iclbMonthwiseAdjustmentDetails == null)
                LoadMonthwiseAdjustmentDetails();
            if (ibusPaymentRecovery == null)
                LoadPaymentRecovery();

            foreach (busPaymentBenefitOverpaymentDetail lobjDetail in iclbPaymentBenefitOverpaymentDetail)
                lobjDetail.icdoPaymentBenefitOverpaymentDetail.Delete();
            foreach (busPayeeAccountMonthwiseAdjustmentDetail lobjMonthwise in iclbMonthwiseAdjustmentDetails)
                lobjMonthwise.icdoPayeeAccountMonthwiseAdjustmentDetail.Delete();
            if (ibusPaymentRecovery.icdoPaymentRecovery.payment_recovery_id > 0)
                ibusPaymentRecovery.icdoPaymentRecovery.Delete();
            return base.Delete();
        }

        //uat pir 1420
        public bool IsPayeeAccountStatusCancelPendingOrCancelled()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatus();

            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelPending() || ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                lblnResult = true;

            return lblnResult;
        }

        public DateTime idtDateafter60daysfromToday
        {
            get
            {
                return DateTime.Today.AddDays(60);
            }
        }
        public DateTime idtNextPaymentDateafter60daysfromToday
        {
            get
            {
                return new DateTime(idtDateafter60daysfromToday.AddMonths(1).Year, idtDateafter60daysfromToday.AddMonths(1).Month, 1);
            }
        }
        public decimal idecRecoveryAmount { get; set; }

        public decimal idecGrossReductionAmount { get; set; }

        public DateTime idtRecoveryEffectiveDate { get; set; }

        public int iintCurrenYear
        {
            get
            {
                return DateTime.Today.Year;
            }
        }
        public int iintNumberOfMonthsToRecover { get; set; }
        public override void LoadCorresProperties(string astrTemplateName)
        {
            base.LoadCorresProperties(astrTemplateName);
            //prod pir 5571 : need to create a recovery object and calculate amounts
            //--start--//
            if (astrTemplateName == "PAY-4207")
            {
                busPaymentRecovery lobjRecovery = new busPaymentRecovery { icdoPaymentRecovery = new cdoPaymentRecovery() };
                lobjRecovery.icdoPaymentRecovery.benefit_overpayment_id = icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id;
                lobjRecovery.icdoPaymentRecovery.payee_account_id = icdoPaymentBenefitOverpaymentHeader.payee_account_id;
                lobjRecovery.LoadTotalRecoveryAmount();
                lobjRecovery.icdoPaymentRecovery.repayment_type_value = busConstant.RecoveryRePaymentTypeLifeTimeReduction;
                lobjRecovery.icdoPaymentRecovery.payment_option_value = busConstant.RecoveryPaymentOptionNDPERSPensionChk;
                lobjRecovery.icdoPaymentRecovery.effective_date = DateTime.Today;
                lobjRecovery.iclbPayeeAccountRetroPaymentRecoveryDetail = new Collection<busPayeeAccountRetroPaymentRecoveryDetail>();
                lobjRecovery.iclbPaymentRecoveryHistory = new Collection<busPaymentRecoveryHistory>();
                if (lobjRecovery.CalculateGrossReductionAmount())
                {
                    idecGrossReductionAmount = lobjRecovery.icdoPaymentRecovery.gross_reduction_amount;
                    idecRecoveryAmount = lobjRecovery.icdoPaymentRecovery.recovery_amount;
                    idtRecoveryEffectiveDate = lobjRecovery.icdoPaymentRecovery.effective_date;
                    iintNumberOfMonthsToRecover = Convert.ToInt32(idecRecoveryAmount / idecGrossReductionAmount);
                }
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
            }
            //--end--//
        }

        public override busBase GetCorPerson()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayee == null)
                ibusPayeeAccount.LoadPayee();
            return ibusPayeeAccount.ibusPayee;
        }
        public bool DoesPayeeAccountHasPriorPopup()
        {
            if (ibusPayeeAccount.IsNull()) LoadPayeeAccount();
            if (ibusPayeeAccount.iclbRetroPayment.IsNull()) ibusPayeeAccount.LoadRetroPayment();
            return icdoPaymentBenefitOverpaymentHeader.suppress_warnings_flag != busConstant.Flag_Yes && ibusPayeeAccount
                .iclbRetroPayment
                .Any(retropayment => retropayment.icdoPayeeAccountRetroPayment.retro_payment_type_value ==
                busConstant.RetroPaymentTypePopupBenefits && retropayment.icdoPayeeAccountRetroPayment.approved_flag == busConstant.Flag_Yes);
        }
    }
}
