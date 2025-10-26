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
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using NeoSpin.DataObjects;
#endregion

namespace NeoSpin.BusinessObjects
{
    public class busPostingInterestBatch : busExtendBase
    {
        public busDBCacheData ibusDBCacheData { get; set; }
        public string PERSLinkBatchUser { get; set; }
        public DataTable idtbBenefitAccountPersonData { get; set; }
        public const int SYSTEM_CONSTANT_CODE = 52;
        public DataTable idtbPendingApprovalCalculations; //holding all pending for approval calculations
        /// <summary>
        /// Post Interest Rate for that person account id
        /// </summary>
        /// <param name="ldecInterestRate">Calculated Interest Rate</param>
        /// <param name="lintPersonAcctId">Person Account ID</param>
        /// <param name="ldateEffectiveDate">Effective date</param>
        public int PostInterestRate(decimal adecInterestRate, decimal adecERInterestRate, int aintPersonAcctId, DateTime adateEffectiveDate,
            bool ablnAdjustment, int aintRefID, DateTime adtBatchDate, string astrSubSystemValue)
        {
            busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
            lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = aintPersonAcctId;

            if (ablnAdjustment)
            {
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value = astrSubSystemValue;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id = aintRefID;
            }
            else
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value = busConstant.SubSystemInterestCredit;

            lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypeInterest;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = adtBatchDate;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = adateEffectiveDate;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.interest_amount = adecInterestRate;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.employer_interest = adecERInterestRate;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.employer_rhic_interest = 0.00M;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_employment_dtl_id = 0;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month = adateEffectiveDate.Month;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year = adateEffectiveDate.Year;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
            return lobjRetirementContribution.icdoPersonAccountRetirementContribution.retirement_contribution_id;

        }

        /// <summary>
        /// Calculate Interest Adjustment
        /// </summary>
        /// <param name="lintPlanId">Plan ID</param>
        /// <param name="lintPersonId">Person ID</param>
        /// <param name="ldatePayPeriodDate">Pay Period Date</param>
        /// <param name="ldecBalance">Balance</param>
        public decimal CalculateInterestAdjustment(int aintPlanId, int aintPersonId, DateTime adatePayPeriodDate,
            decimal adecBalance, int aintRefID, DateTime adtBatchDate, int aintPersonAccountID, string astrSubSystemValue)
        {
            decimal ldecTotalInterest = 0.00M;
            if (aintPersonAccountID > 0)
            {
                DataTable ldtbEffectiveAndTransDates =
                    busNeoSpinBase.Select("cdoPersonAccountRetirementContribution.GetEffectiveAndTransactionDates",
                                          new object[1] { aintPersonAccountID });

                //PIR 25931 - Calculate interest for that pay period if there is no previous interest. The interest will be calculated for each month from the reporting month up to the latest month. 
                DateTime ldateEffectiveDate = new DateTime();

                if (ldtbEffectiveAndTransDates.Rows.Count > 0)
                {
                    DateTime ldateTransactionDate = Convert.ToDateTime(ldtbEffectiveAndTransDates.Rows[0]["transaction_date"].ToString());
                    ldateEffectiveDate = Convert.ToDateTime(ldtbEffectiveAndTransDates.Rows[0]["effective_date"].ToString());

                }
                else
                {
                    ldateEffectiveDate = (DateTime)DBFunction.DBExecuteScalar("entPersonAccountRetirementContribution.GetLastInterestPostedDate",
                                                new object[0] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
                decimal ldecInterest = 0;
                int lintMonths = (ldateEffectiveDate.Month - adatePayPeriodDate.Month) + 1;
                int lintYears = ldateEffectiveDate.Year - adatePayPeriodDate.Year;
                lintMonths += lintYears * 12;

                ldateEffectiveDate = new DateTime(adatePayPeriodDate.Year, adatePayPeriodDate.Month, 1).AddMonths(1);

                for (int i = 0; i < lintMonths; i++)
                { 
                    ldecInterest = busInterestCalculationHelper.CalculateInterest(adecBalance, aintPlanId, ldateEffectiveDate.AddDays(-1)); //PIR 17512 interest(interest rate based on effective date)  POINT 3
                    PostInterestRate(ldecInterest, 0.0m, aintPersonAccountID, ldateEffectiveDate.AddDays(-1), true, aintRefID, adtBatchDate, astrSubSystemValue); // This date logic is to get last day of the payperiod month
                    adecBalance += ldecInterest;     //Add Interest to balance for next month.
                    ldateEffectiveDate = ldateEffectiveDate.AddMonths(1); //Add 1 to effective date for next month.   
                    ldecTotalInterest += ldecInterest;
                }
            }
            return ldecTotalInterest;
        }
        //BR 057-35 The system must populate the additional interest that has been posted into the account after the calculation was approved as
        //a separate ‘Payment Item’ under the payee account and update ‘Benefit Account’, Taxes’ and ‘Rollover Information’ accordingly.
        public void AdjustPayeeAccountForAdditionalContributions(int aintPersonAccountID, decimal adecInterestAmount, decimal adecERInterestAmount, int aintPstdIntContributionId,int aintBatchScheduleId)
        {
            DataRow[] larrRow = idtbBenefitAccountPersonData.FilterTable(busConstant.DataType.Numeric, "person_account_id",
                                                                   aintPersonAccountID);

            if (larrRow.IsNotNull() && larrRow.Count() > 0 && (larrRow[0]["benefit_account_id"] != DBNull.Value) &&
                Convert.ToInt32(larrRow[0]["benefit_account_id"]) > 0)
            {

                busBenefitAccount lobjBenefitAccount = LoadBenefitAccount(larrRow[0], adecInterestAmount,aintBatchScheduleId);
                busPersonAccount lobjPersonAccount = LoadPersonAccount(larrRow[0]);
                decimal ldecDROInterestAllocated = 0.0M;
                if (lobjPersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    decimal ldecDROAdditionalInterest = 0.00m;
                    //PIR 1396 changes
                    if (lobjPersonAccount.iclbBenefitDROApplication == null)
                        lobjPersonAccount.LoadDROApplication();
                    foreach (busBenefitDroApplication lobjQDRO in lobjPersonAccount.iclbBenefitDROApplication)
                    {
                        if (lobjQDRO.ibusBenefitDroCalculation == null)
                            lobjQDRO.LoadDROCalculation();
                        //If DRO Application exists Update the additional interest amount and minimum guarantee amount accordingly
                        if (lobjQDRO.ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0 &&
                            (lobjQDRO.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusApproved ||
                            lobjQDRO.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusPending))
                        {
                            decimal ldecPayentItemAdditionalInterest = 0.00m;
                            ldecDROAdditionalInterest = (adecInterestAmount * lobjQDRO.GetCalculationPercentage()) / 100;

                            if (lobjQDRO.iclbPayeeAccount == null)
                                lobjQDRO.LoadPayeeAccount();

                            if (lobjQDRO.iclbPayeeAccount.Count > 0)
                                ldecPayentItemAdditionalInterest = ldecDROAdditionalInterest / lobjQDRO.iclbPayeeAccount.Count;
                            //if more than one payee account exist for a DRO application  ,Divide the additional interest accordingly
                            foreach (busPayeeAccount lobjDROPayeeAccount in lobjQDRO.iclbPayeeAccount)
                            {
                                if (lobjDROPayeeAccount.ibusPayeeAccountActiveStatus == null)
                                    lobjDROPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                                if ((lobjDROPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundApproved)
                                    || (lobjDROPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundReview))
                                {
                                    if (lobjDROPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                                        lobjDROPayeeAccount.LoadNexBenefitPaymentDate();
                                    //PIR 21096 - PAPIT should be end dated as last day of start date
                                    lobjDROPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemAdditionalEEInterestAmount, ldecPayentItemAdditionalInterest,
                                        string.Empty, 0, lobjDROPayeeAccount.idtNextBenefitPaymentDate, lobjDROPayeeAccount.idtNextBenefitPaymentDate.GetLastDayofMonth(), false);

                                    if (lobjQDRO.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeMonthlyBenefit)
                                    {
                                        lobjDROPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount += ldecPayentItemAdditionalInterest;
                                        lobjDROPayeeAccount.icdoPayeeAccount.Update();
                                        ldecDROInterestAllocated += ldecPayentItemAdditionalInterest;
                                    }
                                    if (lobjDROPayeeAccount.ibusSoftErrors == null)
                                        lobjDROPayeeAccount.LoadErrors();
                                    lobjDROPayeeAccount.iblnClearSoftErrors = false;
                                    lobjDROPayeeAccount.ibusSoftErrors.iblnClearError = false;
                                    lobjDROPayeeAccount.iblnAdditionalInterestIndicatorFlag = true;
                                    lobjDROPayeeAccount.CreateReviewPayeeAccountStatus();
                                    lobjDROPayeeAccount.ValidateSoftErrors();
                                    lobjDROPayeeAccount.UpdateValidateStatus();
                                }
                            }
                            lobjQDRO.ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest_amount += ldecPayentItemAdditionalInterest;
                            lobjQDRO.ibusBenefitDroCalculation.icdoBenefitDroCalculation.Update();
                        }
                    }
                    if (lobjPersonAccount.ibusPerson.icdoPerson.person_id > 0)
                    {
                        if (lobjPersonAccount.ibusPerson.iclbPayeeAccount == null)
                            lobjPersonAccount.ibusPerson.LoadPayeeAccount();

                        busPayeeAccount lbusLatestPayeeAccount = lobjPersonAccount.ibusPerson.iclbPayeeAccount.Where(o =>
                                                    o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund &&
                                                    o.icdoPayeeAccount.benefit_account_id == lobjBenefitAccount.icdoBenefitAccount.benefit_account_id &&
                                                    (o.icdoPayeeAccount.rhic_ee_amount_refund_flag == busConstant.Flag_No ||
                                                    o.icdoPayeeAccount.rhic_ee_amount_refund_flag == null))
                                                    .OrderByDescending(o => o.icdoPayeeAccount.payee_account_id).FirstOrDefault();
                        if (lbusLatestPayeeAccount != null && lbusLatestPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                        {
                            lbusLatestPayeeAccount.idtbPaymentItemType = ibusDBCacheData.idtbCachedPaymentItemType;
                            if (lbusLatestPayeeAccount.ibusApplication == null)
                                lbusLatestPayeeAccount.LoadApplication();

                            if (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus == null)
                                lbusLatestPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                            busBenefitRefundApplication lbusBenefitRefundApplication = new busBenefitRefundApplication();
                            lbusBenefitRefundApplication.icdoBenefitApplication = lbusLatestPayeeAccount.ibusApplication.icdoBenefitApplication;
                            if (lbusBenefitRefundApplication.iclbBenefitRefundCalculation == null)
                                lbusBenefitRefundApplication.LoadRefundBenefitCalculation();
                            decimal ldecOverriddenErInterest =
                            lbusBenefitRefundApplication.iclbBenefitRefundCalculation.Where(o =>
                                (o.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                                o.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent)).Count() > 0 ?
                                lbusBenefitRefundApplication.iclbBenefitRefundCalculation[0].icdoBenefitRefundCalculation.overridden_er_interest_amount : 0;
                            //PIR 1476 fix
                            if (lbusBenefitRefundApplication.IsBenefitOptionRegularRefundOrAutoRefund())
                                adecERInterestAmount = 0.0m;
                            //When refund payee account status is processed, do not do anything
                            //if (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundProcessed)
                            //{
                            //    if (!lbusBenefitRefundApplication.IsApplicationCancelledOrDenied())
                            //    {
                            //        lbusBenefitRefundApplication.CreateAdjustmentRefund(null, DateTime.Now, false, adecAdditionalInterestAmount: adecInterestAmount,
                            //        adecERPreTaxInterestAmount: adecERInterestAmount);
                            //    }
                            //}
                            //else 
                            if (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusRefundApprovedOrRefundReview())
                            {
                                lbusLatestPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemAdditionalEEInterestAmount,
                                                               adecInterestAmount - ldecDROAdditionalInterest, string.Empty, 0,
                                                               busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1), DateTime.MinValue);
                                //as per discussion with Satya on 03 may 2010
                                //ER interest item should be created only for transfers
                                if (ldecOverriddenErInterest == 0.0m && adecERInterestAmount > 0.0m &&
                                    (lbusLatestPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToDCTransfer ||
                                    lbusLatestPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection ||
                                    lbusLatestPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE ||
                                    lbusLatestPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDualMembers ||
                                    lbusLatestPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer))
                                {
                                    lbusLatestPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemERInterestAmount,
                                                                   adecERInterestAmount, string.Empty, 0,
                                                                   busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1), DateTime.MinValue);
                                }
                                if (lbusLatestPayeeAccount.iclbActiveRolloverDetails == null)
                                    lbusLatestPayeeAccount.LoadActiveRolloverDetail();

                                if (lbusLatestPayeeAccount.iclbActiveRolloverDetails.Count > 0)
                                {
                                    lbusLatestPayeeAccount.CreateRolloverAdjustment();
                                }
                                else
                                {
                                    lbusLatestPayeeAccount.CalculateAdjustmentTax(false);
                                }
                                UpdateRetirementContributionTransferFlagToC(aintPstdIntContributionId);
                            }
                            //PROD PIR 4851 : need to update benefit account regardless of the benefit account type, so moving the update below
                            //lobjBenefitAccount.icdoBenefitAccount.Update();
                        }
                        //ADJUSTING REFUND PAYEE ACCOUNTS ENDS HERE
                        //block to update PreRetirement death payee account
                        //--start--//
                        //PIR 26246 - To load exact Benefit Calculation

                        DataTable ldtDeathCalculation = Select<cdoBenefitCalculation>
                            (new string[6] { enmBenefitCalculation.person_id.ToString(), enmBenefitCalculation.calculation_type_value.ToString(),
                                enmBenefitCalculation.action_status_value.ToString(),enmBenefitCalculation.benefit_account_type_value.ToString(),
                                enmBenefitCalculation.benefit_option_value.ToString(), enmBenefitCalculation.plan_id.ToString() },
                            new object[6] { lobjPersonAccount.ibusPerson.icdoPerson.person_id, busConstant.CalculationTypeFinal, busConstant.CalculationStatusApproval,
                                busConstant.ApplicationBenefitTypePreRetirementDeath,busConstant.BenefitOptionRefund, lobjPersonAccount.icdoPersonAccount.plan_id }, null, null);
                        //bool lblnCreateNewCalc = false;
                        foreach (DataRow ldrCalc in ldtDeathCalculation.Rows)
                        {
                            //lblnCreateNewCalc = false;
                            busBenefitCalculation lbusDethCalc = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
                            lbusDethCalc.icdoBenefitCalculation.LoadData(ldrCalc);
                            lbusDethCalc.LoadPayeeAccount();
                            foreach (busPayeeAccount lbusPA in lbusDethCalc.iclbPayeeAccount)
                            {
                                lbusPA.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                                lbusPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
                                busGlobalFunctions.GetData2ByCodeValue(2203, lbusPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                                lbusPA.LoadPaymentHistoryHeader();
                            }
                            if (lbusDethCalc.iclbPayeeAccount
                                .All(o => o.iclbPaymentHistoryHeader.Count == 0))
                            {
                                lbusDethCalc.UpdateDeathCalcFromInterestPostingBatch(adecInterestAmount - ldecDROAdditionalInterest);
                            }
                        }
                        //--end--//
                        // PROD PIR 8312: block to update PostRetirement death payee account
                        ///*--start--
                        DataTable ldtPostDeathCalculation = Select<cdoBenefitCalculation>
                            (new string[4] { enmBenefitCalculation.person_id.ToString(), enmBenefitCalculation.calculation_type_value.ToString(),
                                enmBenefitCalculation.action_status_value.ToString(),enmBenefitCalculation.benefit_account_type_value.ToString() },
                            new object[4] { lobjPersonAccount.ibusPerson.icdoPerson.person_id, busConstant.CalculationTypeFinal, busConstant.CalculationStatusApproval,
                                busConstant.ApplicationBenefitTypePostRetirementDeath }, null, null);
                        //bool lblnCreateNewPostCalc = false;
                        //foreach (DataRow ldrCalc in ldtPostDeathCalculation.Rows)
                        //{
                        //lblnCreateNewPostCalc = false;
                        //busBenefitCalculation lbusDethCalc = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
                        //lbusDethCalc.icdoBenefitCalculation.LoadData(ldrCalc);
                        //lbusDethCalc.LoadPayeeAccount();
                        //foreach (busPayeeAccount lbusPA in lbusDethCalc.iclbPayeeAccount)
                        //{
                        //    lbusPA.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                        //    lbusPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
                        //    busGlobalFunctions.GetData2ByCodeValue(2203, lbusPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                        //}
                        //if (lbusDethCalc.iclbPayeeAccount
                        //    .Where(o => o.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusPaymentComplete).Any())
                        //{
                        //    lblnCreateNewPostCalc = true;
                        //}
                        //if (lblnCreateNewPostCalc)
                        //    lbusDethCalc.CreateNewDeathCalcFromInterestPostingBatch(adecInterestAmount - ldecDROAdditionalInterest);
                        //else
                        //    lbusDethCalc.UpdateDeathCalcFromInterestPostingBatch(adecInterestAmount - ldecDROAdditionalInterest);


                        //}
                        //--end--*/
                        if (ldtPostDeathCalculation.Rows.Count > 0)
                        {
                            InitiateWorkflow(lobjPersonAccount.ibusPerson.icdoPerson.person_id, 0, 0, busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
                        }
                    }
                }
                //PROD PIR 4851 : need to update benefit account regardless of the benefit account type
                lobjBenefitAccount.icdoBenefitAccount.Update();

                //PROD Pir 4421
                //need to update minimum guarantee amount for all regular payee accounts
                if (lobjBenefitAccount.icdoBenefitAccount.benefit_account_id > 0)
                {
                    lobjBenefitAccount.LoadPayeeAccounts();
                    IEnumerable<busPayeeAccount> lenmPA = lobjBenefitAccount.iclbPayeeAccount
                        .Where(o => (o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) &&
                            o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption10YearCertain &&
                            o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption15YearCertain &&
                            o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption20YearCertain &&
                            o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption5YearTermLife &&
                            o.icdoPayeeAccount.application_id > 0);
                    foreach (busPayeeAccount lobjPA in lenmPA)
                    {
                        lobjPA.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                        lobjPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
                            busGlobalFunctions.GetData2ByCodeValue(2203, lobjPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                        if (lobjPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusCancelled &&
                            lobjPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusPaymentComplete)
                        {
                            decimal ldecRegularPAInterest = adecInterestAmount > ldecDROInterestAllocated ?
                                adecInterestAmount - ldecDROInterestAllocated : 0.0M;
                            lobjPA.icdoPayeeAccount.minimum_guarantee_amount += ldecRegularPAInterest;
                            lobjPA.icdoPayeeAccount.Update();
                        }
                    }
                    if (lobjBenefitAccount.iclbPayeeAccount.Any(pa => pa.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
                    {
                        AdjustPreRetDeathPayeeAccountOMGAndBenActNonTaxable(lobjBenefitAccount, adecInterestAmount, ldecDROInterestAllocated);
                    }
                    else
                    {
                        if (lobjBenefitAccount.iclbPayeeAccount.Any(pa => pa.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                        {
                            DataRow ldtrPreRetDeathRow = larrRow.FirstOrDefault(datarow => datarow.Field<int>("benefit_account_id") != 0 && datarow.Field<int>("benefit_account_id") 
                                                                                != lobjBenefitAccount.icdoBenefitAccount.benefit_account_id);
                            if (ldtrPreRetDeathRow.IsNotNull() && (ldtrPreRetDeathRow["benefit_account_id"] != DBNull.Value) &&
                                Convert.ToInt32(ldtrPreRetDeathRow["benefit_account_id"]) > 0)
                            {
                                busBenefitAccount lbusPreRetDeathBenAccount = LoadBenefitAccount(ldtrPreRetDeathRow, adecInterestAmount, aintBatchScheduleId);
                                if (lbusPreRetDeathBenAccount.icdoBenefitAccount.benefit_account_id > 0)
                                {
                                    lbusPreRetDeathBenAccount.LoadPayeeAccounts();
                                    if (lbusPreRetDeathBenAccount.iclbPayeeAccount.Count > 0 && 
                                        lbusPreRetDeathBenAccount.iclbPayeeAccount.Any(pa => pa.icdoPayeeAccount.benefit_account_type_value == 
                                        busConstant.ApplicationBenefitTypePreRetirementDeath))
                                    {
                                        lbusPreRetDeathBenAccount.icdoBenefitAccount.Update();
                                        AdjustPreRetDeathPayeeAccountOMGAndBenActNonTaxable(lbusPreRetDeathBenAccount, adecInterestAmount, ldecDROInterestAllocated);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else //no benefit account yet, check if any calculation exists for the person account into which interest has just been posted.
            {
                //put final calculation in review if exists
                busGlobalFunctions.PutCalculationInReviewIfExists(aintPersonAccountID, idtbPendingApprovalCalculations);
            }
        }
        /// <summary>
        /// Updates the posted interest in retirement contribution table to 'C'
        /// </summary>
        /// <param name="aintPstdIntContributionId"></param>
        private void UpdateRetirementContributionTransferFlagToC(int aintPstdIntContributionId)
        {
            DBFunction.DBNonQuery("cdoPersonAccountRetirementContribution.UpdateInterestContributionTransferFlagToC",
                                       new object[2] { iobjPassInfo.istrUserID, aintPstdIntContributionId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
        public void InitiateWorkflow(int aintPersonID, int aintBeneficiaryID, int aintReferenceID, int aintTypeID)
        {
            if (!busWorkflowHelper.IsActiveInstanceAvailable(aintPersonID, aintTypeID))
            {
                //only for Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow, beneficiary id to be posted
                Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                ldctParams["additional_parameter1"] = aintBeneficiaryID > 0 ? aintBeneficiaryID.ToString() : "";
                busWorkflowHelper.InitiateBpmRequest(aintTypeID, aintPersonID, 0, aintReferenceID, iobjPassInfo, busConstant.WorkflowProcessSource_Batch, adictInstanceParameters: ldctParams);
            }
        }
        private busBenefitAccount LoadBenefitAccount(DataRow adtrDataRow, decimal adecInterestAmount, int aintBatchScheduleId)
        {
            busBenefitAccount lobjBenefitAccount = new busBenefitAccount { icdoBenefitAccount = new cdoBenefitAccount() };
            lobjBenefitAccount.icdoBenefitAccount.LoadData(adtrDataRow);
            PERSLinkBatchUser = busConstant.PERSLinkBatchUser + ' ' + aintBatchScheduleId;
            //loading audit fields to avoid cancel/refresh error
            lobjBenefitAccount.icdoBenefitAccount.created_by = adtrDataRow["BEN_CREATED_BY"] == DBNull.Value ? PERSLinkBatchUser :
                Convert.ToString(adtrDataRow["BEN_CREATED_BY"]);
            lobjBenefitAccount.icdoBenefitAccount.created_date = adtrDataRow["BEN_CREATED_DATE"] == DBNull.Value ? DateTime.Now :
                Convert.ToDateTime(adtrDataRow["BEN_CREATED_DATE"]);
            lobjBenefitAccount.icdoBenefitAccount.modified_by = adtrDataRow["BEN_MODIFIED_BY"] == DBNull.Value ? PERSLinkBatchUser :
                Convert.ToString(adtrDataRow["BEN_MODIFIED_BY"]);
            lobjBenefitAccount.icdoBenefitAccount.modified_date = adtrDataRow["BEN_MODIFIED_DATE"] == DBNull.Value ? DateTime.Now :
                Convert.ToDateTime(adtrDataRow["BEN_MODIFIED_DATE"]);
            lobjBenefitAccount.icdoBenefitAccount.update_seq = adtrDataRow["BEN_UPDATE_SEQ"] == DBNull.Value ? 0 :
                Convert.ToInt32(adtrDataRow["BEN_UPDATE_SEQ"]);

            lobjBenefitAccount.icdoBenefitAccount.starting_taxable_amount = lobjBenefitAccount.icdoBenefitAccount.starting_taxable_amount
                                                                            + adecInterestAmount;
            return lobjBenefitAccount;
        }
        private busPersonAccount LoadPersonAccount(DataRow adtrdataRow)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount() };
            lobjPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjPersonAccount.icdoPersonAccount.LoadData(adtrdataRow);
            lobjPersonAccount.ibusPerson.icdoPerson.LoadData(adtrdataRow);
            return lobjPersonAccount;
        }
        private void AdjustPreRetDeathPayeeAccountOMGAndBenActNonTaxable(busBenefitAccount aobjBenefitAccount, decimal adecInterestAmount, decimal adecDROInterestAllocated)
        {
            //PIR 16967 - Pre-Retirement Death Refunds did not issue last month of interest on January payroll
            IEnumerable<busPayeeAccount> lenmPreRetPA = aobjBenefitAccount.iclbPayeeAccount
                .Where(o => (o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath) &&
                    o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption10YearCertain &&
                    o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption15YearCertain &&
                    o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption20YearCertain &&
                    o.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption5YearTermLife &&
                    o.icdoPayeeAccount.application_id > 0);
            decimal ldecRegPAInterest = adecInterestAmount > adecDROInterestAllocated ?
                        adecInterestAmount - adecDROInterestAllocated : 0.0M;
            foreach (busPayeeAccount lobjPreRetPA in lenmPreRetPA)
            {
                lobjPreRetPA.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                lobjPreRetPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
                    busGlobalFunctions.GetData2ByCodeValue(2203, lobjPreRetPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                if (lobjPreRetPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusCancelled &&
                    lobjPreRetPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusPaymentComplete)
                {
                    if (lobjPreRetPA.ibusBenefitCalculaton.IsNull()) lobjPreRetPA.LoadBenefitCalculation();
                    if (lobjPreRetPA.ibusBenefitCalculaton.iclbBenefitCalculationPayee.IsNull()) lobjPreRetPA.ibusBenefitCalculaton.LoadBenefitCalculationPayee();
                    busBenefitCalculationPayee lbusBenefitCalculationPayee = lobjPreRetPA
                                                                            .ibusBenefitCalculaton
                                                                            .iclbBenefitCalculationPayee.
                                                                            FirstOrDefault(i => i.icdoBenefitCalculationPayee.payee_account_id ==
                                                                                lobjPreRetPA.icdoPayeeAccount.payee_account_id);
                    decimal ldecBenefitPercentage = 0.0M;
                    if (lbusBenefitCalculationPayee.IsNotNull())
                    {
                        ldecBenefitPercentage = lbusBenefitCalculationPayee.icdoBenefitCalculationPayee.benefit_percentage;
                    }
                    if (ldecRegPAInterest > 0 && ldecBenefitPercentage > 0)
                    {
                        decimal ldecPercentageInterestAmount = Math.Round(ldecRegPAInterest * ldecBenefitPercentage / 100, 6, MidpointRounding.AwayFromZero);
                        lobjPreRetPA.icdoPayeeAccount.minimum_guarantee_amount += ldecPercentageInterestAmount;
                        lobjPreRetPA.icdoPayeeAccount.Update();
                    }
                }
            }
        }
    }
}