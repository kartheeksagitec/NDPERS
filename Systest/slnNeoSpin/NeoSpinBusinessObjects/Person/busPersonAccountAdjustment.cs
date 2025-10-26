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
using System.Linq;
using NeoSpin.DataObjects;
using NeoSpin.Common;
using System.Collections.Generic;
using Sagitec.Bpm;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
    public class busPersonAccountAdjustment : busPersonAccountAdjustmentGen
	{
        // Person account
        public busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get
            {
                return _ibusPersonAccount;
            }
            set
            {
                _ibusPersonAccount = value;
            }
        }
        public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
            {
                _ibusPersonAccount = new busPersonAccountRetirement();
            }
            if (icdoPersonAccountAdjustment.person_account_id > 0)
            {
                ((busPersonAccountRetirement)_ibusPersonAccount).FindPersonAccountRetirement(icdoPersonAccountAdjustment.person_account_id);                
                _ibusPersonAccount.LoadPerson();
                _ibusPersonAccount.LoadPlan();
            }
        }
        public decimal idecTotPostTaxERAmt { get; set; }
        public decimal idecTotPostTaxEEAmt { get; set; }
        public decimal idecTotPreTaxERAmt { get; set; }
        public decimal idecTotPreTaxEEAmt { get; set; }
        public decimal idecTotEERhicAmt { get; set; }
        public decimal idecTotERRhicAmt { get; set; }
        public decimal idecTotEEERPickupAmt { get; set; }
        public decimal idecTotERVestedAmt { get; set; }
        public decimal idecTotIntAmt { get; set; }
        public decimal idecTotERIntAmt { get; set; }
        public decimal idecTotVestedServiceCredit { get; set; }
        public decimal idecTotPensionServiceCredit { get; set; }

        public decimal idecTotalTaxableAmt { get; set; }
        public decimal idecTotalNonTaxableAmt { get; set; }

        public decimal idecTotalMGPortion { get; set; }

        public bool iblnSalaryAmountChangedForAnyPayPeriodMonth { get; set; } //PIR 18128


        // Retirement adjustment detail
        public Collection<busPersonAccountRetirementAdjustmentContribution> _iclbRetirementAdjustmentContribution;

        public Collection<busPersonAccountRetirementAdjustmentContribution> iclbRetirementAdjustmentContribution
        {
            get
            {
                return _iclbRetirementAdjustmentContribution;
            }
            set
            {
                _iclbRetirementAdjustmentContribution = value;
            }
        }
        public void LoadRetirementAdjustmentContribution()
        {
            if (_iclbRetirementAdjustmentContribution == null)
            {
                _iclbRetirementAdjustmentContribution = new Collection<busPersonAccountRetirementAdjustmentContribution>();
            }
            DataTable ldtbList = Select<cdoPersonAccountRetirementAdjustmentContribution>(
                new string[1] { "person_account_adjustment_id" }, new object[1] { icdoPersonAccountAdjustment.person_account_adjustment_id }, null, null);

            _iclbRetirementAdjustmentContribution = GetCollection<busPersonAccountRetirementAdjustmentContribution>(ldtbList,
                "icdoRetirementAdjustmentContribution");
            foreach (busPersonAccountRetirementAdjustmentContribution lbusAdjustmentContribution in _iclbRetirementAdjustmentContribution)
                lbusAdjustmentContribution.ibusPersonAccountAdjustment = this;
        }
        public ArrayList btnPost_Click()
        {
            ArrayList larrError = new ArrayList();
            utlError lobjError = null;
            if (_iclbRetirementAdjustmentContribution == null)
                LoadRetirementAdjustmentContribution();
            foreach (busPersonAccountRetirementAdjustmentContribution lbusRetirementAdjustmentContribution in _iclbRetirementAdjustmentContribution)
            {
                if (_ibusPersonAccount.icdoPersonAccount == null)
                    _ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();

                if (!CheckIfEffectiveDateIsBetweenStartAndEndDate(lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.effective_date))
                {
                    lobjError = AddError(6658, "");
                    larrError.Add(lobjError);
                    return larrError;
                }
                _ibusPersonAccount.PostRetirementContribution(busConstant.SubSystemValueAdjustment, icdoPersonAccountAdjustment.person_account_adjustment_id,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.transaction_date,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.effective_date,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.pay_period_month,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.pay_period_year,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.person_employment_dtl_id,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.transaction_type_value,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.salary_amount,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.post_tax_er_amount,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.post_tax_ee_amount,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.pre_tax_er_amount,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.pre_tax_ee_amount,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.ee_rhic_amount,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.er_rhic_amount,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.ee_er_pickup_amount,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.er_vested_amount,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.interest_amount,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.vested_service_credit,
                    lbusRetirementAdjustmentContribution.icdoRetirementAdjustmentContribution.pension_service_credit);
            }
            busPersonAccountRetirement lbusPersonAccountRetirement = (busPersonAccountRetirement)_ibusPersonAccount;
            lbusPersonAccountRetirement.icdoPersonAccountRetirement.rhic_benfit_amount += icdoPersonAccountAdjustment.rhic_benfit_amount;
            lbusPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain += icdoPersonAccountAdjustment.capital_gain;
            lbusPersonAccountRetirement.icdoPersonAccountRetirement.Update();
            // Update Status
            icdoPersonAccountAdjustment.iblnUpdateModifiedBy = false;
            icdoPersonAccountAdjustment.posted_by = iobjPassInfo.istrUserID;
            icdoPersonAccountAdjustment.posted_date = DateTime.Now;
            icdoPersonAccountAdjustment.status_value = busConstant.AdjustmentStatusPosted;
            icdoPersonAccountAdjustment.Update();
            icdoPersonAccountAdjustment.iblnUpdateModifiedBy = true;
            //PIR 17140, 16601, 16896, when additional contributions come after a final calculation had alerady been made 
            bool lblnResult = busGlobalFunctions.PutCalculationInReviewIfExists(_ibusPersonAccount.icdoPersonAccount.person_account_id);
            if (!lblnResult && _ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                AdjustPayeeAccountOrCreateAdjtCalc(_ibusPersonAccount.icdoPersonAccount.person_account_id);
            larrError.Add(this);
            return larrError;
        }
        /// <summary>
        /// PIR 17140, 16601, 16896
        /// Adjusts Payee Account if exists or creates adjustment calculation.
        /// </summary>
        public void AdjustPayeeAccountOrCreateAdjtCalc(int aintPersonAccountId, DataTable adtbPostedContributions = null, bool ablnIsFromEmpRepPosting = false)
        {
            //Fetch retirement or disability or refund payee account by personaccount id
            DataTable ldtbRefOrDisaOrRetrPayeeAccounts = Select("cdoPayeeAccount.LoadPayeeAccountByPersonAccountId",
                new object[2] { aintPersonAccountId, -999 });
            if (ldtbRefOrDisaOrRetrPayeeAccounts.Rows.Count > 0)
            {
                AdjustRefOrDisaOrRetrPayeeAccounts(ldtbRefOrDisaOrRetrPayeeAccounts.Rows[0], adtbPostedContributions, ablnIsFromEmpRepPosting);
            }
            //preretirementdeath payee accounts adjustment
            DataTable ldtbDeathPayeeAccounts = Select("cdoPayeeAccount.LoadPayeeAccountByPersonAccountId",
            new object[2] { aintPersonAccountId, -998 });
            if (ldtbDeathPayeeAccounts.Rows.Count > 0)
            {
                AdjustPreRetDeathPayeeAccounts(ldtbDeathPayeeAccounts.Rows, adtbPostedContributions, ablnIsFromEmpRepPosting);
            }
            DataTable ldtbPSTDPayeeAccounts = Select("cdoPayeeAccount.LoadPayeeAccountByPersonAccountId",
            new object[2] { aintPersonAccountId, -997 });
            //postretirementdeath payee accounts adjustment
            if (ldtbPSTDPayeeAccounts.Rows.Count > 0)
            {
                AdjustPSTDPayeeAccounts(aintPersonAccountId);
            }
        }
        private void AdjustPSTDPayeeAccounts(int aintPersonAccountId)
        {
            busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            if (lbusPersonAccount.FindPersonAccount(aintPersonAccountId))
                InitiateWorkflow(_ibusPersonAccount.icdoPersonAccount.person_id, 0, 0, busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
        }
        private void AdjustRefOrDisaOrRetrPayeeAccounts(DataRow adtrPayeeAccount, DataTable adtbPostedContributions = null, bool ablnIsFromEmpReportPosting = false)
        {
            busPayeeAccount lbusLatestPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            lbusLatestPayeeAccount.icdoPayeeAccount.LoadData(adtrPayeeAccount);
            lbusLatestPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            lbusLatestPayeeAccount.LoadPaymentHistoryHeader();
            /*REFUND PAYEE ACCOUNT IN APPROVED OR REFUND STATUS WITH NO PAYMENT MADE YET,
             * 1.   UPDATE TAXABLE AND NONTAXABLE ON BENEFIT ACCOUNT.
             * 2.   NO MG UPDATE
             * 3.   UPDATE PAPITs
             * 4.   Adjust rollover and tax.
             * 5.   UPDATE THE JUST POSTED CONTRIBUTIONS'S TRANSFER FLAG TO C. 
             */
            if ((lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund) &&
                (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusRefundApprovedOrRefundReview()) &&
                (lbusLatestPayeeAccount.iclbPaymentHistoryHeader.Count == 0))
            {
                LoadTaxableAndNonTaxableAmountsPostedWithThisAdjustment(adtbPostedContributions);
                //UPDATE TAXABLE AND NONTAXABLE ON BENEFIT ACCOUNT
                if (lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_id > 0 &&
                    (idecTotalTaxableAmt != 0 || idecTotalNonTaxableAmt != 0))
                {
                    busGlobalFunctions.UpdateBenActTaxAndNonTaxableAmts(lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_id, idecTotalTaxableAmt, idecTotalNonTaxableAmt);
                }
                UpdatePAPITAmounts(lbusLatestPayeeAccount); //UPDATE PAPITs
                lbusLatestPayeeAccount.iclbPayeeAccountPaymentItemType = null;
                lbusLatestPayeeAccount.AdjustRollOverAndTax(); //Adjust rollover and tax
                UpdateRetirementAdjustmentContributionsToC(adtbPostedContributions, ablnIsFromEmpReportPosting); //UPDATE THE JUST POSTED CONTRIBUTIONS'S TRANSFER FLAG TO C
            }
            /*      
                REFUND PAYEE ACCOUNT IN REVIEW, PROCESSED, OR STATUS APPROVED WITH AT LEAST ONE PAYMENT MADE SCENARIO
                1.  Put payee account in review if not already in review.
                2.  Initiate workflow, which would create the adjustment.
                3.  if adjustment already exists 
                    in pending approval status,
                        i. Cancel the calculation.
                        ii. Reset the contributions' transfer flag FROM 'C' back to null.
                        iii. Cancel all the running process instances by the payee perslinkid, process id, and refenrence id, 
                        which is payee account id
                    in approved but not yet processed, meaning calculation is approved, adjust PAPITS inserted, but not paid yet to member
                        i. Cancel the calculation.
                        ii. Reset the contributions' transfer flag FROM 'C' back to null.
                        iii. Delete the open PAPITs.
                        IV. Cancel all the running process instances by the payee perslinkid, process id, and refenrence id, 
                        which is payee account id
            */
            else if ((lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                && ((lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusReview())
                || (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusProcessed())
                || (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusApproved())) &&
                (lbusLatestPayeeAccount.iclbPaymentHistoryHeader.Count > 0))
            {
                if (!lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusReview())
                {
                    lbusLatestPayeeAccount.CreateReviewStatus();
                    lbusLatestPayeeAccount.iblnAddionalContributionsIndicatorFlag = true;
                    lbusLatestPayeeAccount.ValidateSoftErrors();
                    lbusLatestPayeeAccount.UpdateValidateStatus();
                }
                lbusLatestPayeeAccount.LoadPayeeAccountPaymentItemType();
                busBenefitRefundApplication lbusRefundBenefitApplication = new busBenefitRefundApplication
                { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusRefundBenefitApplication.FindBenefitApplication(lbusLatestPayeeAccount.icdoPayeeAccount.application_id);
                lbusRefundBenefitApplication.LoadRefundBenefitCalculation();
                var lclbAdjustBenefitRefundCalculations = lbusRefundBenefitApplication
                                                            .iclbBenefitRefundCalculation
                                                            .Where(c => (c.icdoBenefitCalculation.calculation_type_value
                                                            == busConstant.CalculationTypeAdjustments || c.icdoBenefitCalculation.calculation_type_value    //PIR 18053
                                                            == busConstant.CalculationTypeSubsequentAdjustment) &&
                                                            c.icdoBenefitCalculation.action_status_value != busConstant.CalculationStatusCancel);
                if (lclbAdjustBenefitRefundCalculations.Count() > 0 &&
                    lclbAdjustBenefitRefundCalculations
                    .Any(c => c.icdoBenefitCalculation.action_status_value == busConstant.CalculationStatusPendingApproval) &&
                    (lbusLatestPayeeAccount.iclbPayeeAccountPaymentItemType.Count(PAPIT => PAPIT.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue) == 0))
                {
                    var lclbPendingApprovalAdjustCalculations =
                        lclbAdjustBenefitRefundCalculations
                        .Where(c => c.icdoBenefitCalculation.action_status_value == busConstant.CalculationStatusPendingApproval);
                    foreach (busBenefitRefundCalculation lbusBenefitRefundCalculation in lclbPendingApprovalAdjustCalculations)
                    {
                        lbusBenefitRefundCalculation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusCancel;
                        lbusBenefitRefundCalculation.icdoBenefitCalculation.Update();
                        lbusBenefitRefundCalculation.ResetCalculationTransferFlagToNull(lbusBenefitRefundCalculation.icdoBenefitCalculation.benefit_application_id, busConstant.RetirementContributionTransferFlagC);
                    }
                    CancelRunningProcessInstances(lbusLatestPayeeAccount.icdoPayeeAccount.payee_perslink_id, busConstant.Map_Process_Remainder_Transfer_Refund, lbusLatestPayeeAccount.icdoPayeeAccount.payee_account_id);
                }
                else if (lclbAdjustBenefitRefundCalculations.Count() > 0 &&
                    lclbAdjustBenefitRefundCalculations
                    .Any(c => c.icdoBenefitCalculation.action_status_value == busConstant.CalculationStatusApproval &&
                    c.icdoBenefitCalculation.status_value != busConstant.CalculationStatusProcessed))
                {
                    busBenefitRefundCalculation lbusBenefitRefundCalculation = lclbAdjustBenefitRefundCalculations
                                                .FirstOrDefault(c => c.icdoBenefitCalculation.action_status_value == busConstant.CalculationStatusApproval &&
                                                c.icdoBenefitCalculation.status_value != busConstant.CalculationStatusProcessed);
                    lbusBenefitRefundCalculation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusCancel;
                    lbusBenefitRefundCalculation.icdoBenefitCalculation.Update();
                    lbusBenefitRefundCalculation.ResetCalculationTransferFlagToNull(lbusBenefitRefundCalculation.icdoBenefitCalculation.benefit_application_id, busConstant.RetirementContributionTransferFlagC);
                    if (lbusLatestPayeeAccount.iclbPayeeAccountPaymentItemType.Any(PAPIT => PAPIT.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue))
                    {
                        var lclbOpenPAPITs = lbusLatestPayeeAccount.iclbPayeeAccountPaymentItemType.Where(PAPIT => PAPIT.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue);
                        foreach (busPayeeAccountPaymentItemType lbusPAPIT in lclbOpenPAPITs)
                        {
                            lbusPAPIT.icdoPayeeAccountPaymentItemType.Delete();
                        }
                    }
                    CancelRunningProcessInstances(lbusLatestPayeeAccount.icdoPayeeAccount.payee_perslink_id, busConstant.Map_Process_Remainder_Transfer_Refund, lbusLatestPayeeAccount.icdoPayeeAccount.payee_account_id);
                }
                InitiateWorkflow(lbusLatestPayeeAccount.icdoPayeeAccount.payee_perslink_id, 0, lbusLatestPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.Map_Process_Remainder_Transfer_Refund);
            }
            else if (((lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
                (lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                && (!lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusPaymentCompleteOrProcessed()))
            {
                DataTable ldtbPAEmpDetail = Select("cdoPersonAccount.LoadPAEmpDetailWithoutEndDate", new object[1] { ibusPersonAccount.icdoPersonAccount.person_account_id });
                bool lblnPAEmpDetailWithEndDate = ldtbPAEmpDetail.IsNotNull() && ldtbPAEmpDetail.Rows.Count > 0;

                if (!(lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability &&
                    lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusSuspended() && lblnPAEmpDetailWithEndDate))
                {
                    LoadTaxableAndNonTaxableAmountsPostedWithThisAdjustment(adtbPostedContributions);
                    if ((idecTotPensionServiceCredit != 0) || (idecTotVestedServiceCredit != 0) || iblnSalaryAmountChangedForAnyPayPeriodMonth)
                    {
                        if (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusApproved() || lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusReceiving())
                        {
                            lbusLatestPayeeAccount.CreateReviewStatus();
                            lbusLatestPayeeAccount.iblnAddionalContributionsIndicatorFlag = true;
                            lbusLatestPayeeAccount.ValidateSoftErrors();
                            lbusLatestPayeeAccount.UpdateValidateStatus();
                        }
                        InitiateWorkflow(lbusLatestPayeeAccount.icdoPayeeAccount.payee_perslink_id, 0, lbusLatestPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
                    }
                    else if ((idecTotPensionServiceCredit == 0) && (idecTotVestedServiceCredit == 0) && !iblnSalaryAmountChangedForAnyPayPeriodMonth)
                    {
                        if (lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_id > 0 &&
                        (idecTotalTaxableAmt != 0 || idecTotalNonTaxableAmt != 0))
                        {
                            busGlobalFunctions.UpdateBenActTaxAndNonTaxableAmts(lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_id, idecTotalTaxableAmt, idecTotalNonTaxableAmt);
                        }
                        if (lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement
                            && lbusLatestPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption10YearCertain &&
                                    lbusLatestPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption15YearCertain &&
                                    lbusLatestPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption20YearCertain &&
                                    lbusLatestPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption5YearTermLife &&
                                    idecTotalMGPortion != 0)
                        {
                            lbusLatestPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount += idecTotalMGPortion;
                            lbusLatestPayeeAccount.icdoPayeeAccount.Update();
                        }
                    }
                }
            }
            else if (((lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
                (lbusLatestPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                && (lbusLatestPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusPaymentCompleteOrProcessed()))
            {
                InitiateWorkflow(_ibusPersonAccount.ibusPerson.icdoPerson.person_id, 0, lbusLatestPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
            }
        }

        private void CancelRunningProcessInstances(int aintPersonId, int aintProcessId, int aintReferenceId)
        {
            //venkat check query
            DataTable ldtActivityInstance = Select("entSolBpmActivityInstance.LoadRunningInstancesByPersonProcessRef",
                    new object[3] { aintPersonId, aintProcessId, aintReferenceId });
            Collection<busBpmActivityInstance> lclbActivityInstance = GetCollection<busBpmActivityInstance>(ldtActivityInstance, "icdoBpmActivityInstance");
            //try
            //{
            foreach (busBpmActivityInstance lbusTempActivityInstance in lclbActivityInstance)
            {
                busBpmActivityInstance lobjActivtyInstance = busWorkflowHelper.GetActivityInstance(lbusTempActivityInstance.icdoBpmActivityInstance.activity_instance_id);
                busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusCancelled, lobjActivtyInstance, iobjPassInfo);
            }
            //}
            //catch (Exception ex)
            //{
            //    Sagitec.ExceptionPub.ExceptionManager.Publish(ex);
            //}
        }
        public bool iblnBenefitAccountUpdated { get; set; }
        private void AdjustPreRetDeathPayeeAccounts(DataRowCollection adrcPreRetDeathPayeeAccounts, DataTable adtbPostContFromERP, bool ablnIsFromERP)
        {
            iblnBenefitAccountUpdated = false;
            foreach (DataRow ldataRow in adrcPreRetDeathPayeeAccounts)
            {
                busPayeeAccount lbusPreRetDeathPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lbusPreRetDeathPayeeAccount.icdoPayeeAccount.LoadData(ldataRow);
                lbusPreRetDeathPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                lbusPreRetDeathPayeeAccount.LoadPaymentHistoryHeader();
                /*
                 * 1. Update taxable and nontaxable on benefit account.
                 * 2. Update Minimum guarantee.
                 * 3. Update PAPITs
                 * 4. Adjust rollover details and tax calculation.
                 * */
                if ((lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund) &&
                ((lbusPreRetDeathPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusApproved()) || (lbusPreRetDeathPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusReview()))
                && (lbusPreRetDeathPayeeAccount.iclbPaymentHistoryHeader.Count == 0))
                {
                    LoadTaxableAndNonTaxableAmountsPostedWithThisAdjustment(adtbPostContFromERP);
                    //UPDATE TAXABLE AND NONTAXABLE ON BENEFIT ACCOUNT
                    if (lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_account_id > 0 &&
                        (idecTotalTaxableAmt != 0 || idecTotalNonTaxableAmt != 0) && !iblnBenefitAccountUpdated)
                    {
                        busGlobalFunctions.UpdateBenActTaxAndNonTaxableAmts(lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_account_id, idecTotalTaxableAmt, idecTotalNonTaxableAmt);
                        iblnBenefitAccountUpdated = true;
                    }
                    //Update minimum guarantee
                    decimal ldecBenefitPercentage = 0.0M;
                    if (lbusPreRetDeathPayeeAccount.ibusBenefitCalculaton.IsNull()) lbusPreRetDeathPayeeAccount.LoadBenefitCalculation();
                    if (lbusPreRetDeathPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationPayee.IsNull()) lbusPreRetDeathPayeeAccount.ibusBenefitCalculaton.LoadBenefitCalculationPayee();
                    if (lbusPreRetDeathPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationOptions.IsNull())
                        lbusPreRetDeathPayeeAccount.ibusBenefitCalculaton.LoadBenefitCalculationOptions();
                    busBenefitCalculationPayee lbusBenefitCalculationPayee = lbusPreRetDeathPayeeAccount
                                                                            .ibusBenefitCalculaton
                                                                            .iclbBenefitCalculationPayee.
                                                                            FirstOrDefault(i => i.icdoBenefitCalculationPayee.payee_account_id ==
                                                                                lbusPreRetDeathPayeeAccount.icdoPayeeAccount.payee_account_id);
                    if (lbusBenefitCalculationPayee.IsNotNull())
                    {
                        ldecBenefitPercentage = lbusBenefitCalculationPayee.icdoBenefitCalculationPayee.benefit_percentage;
                        busBenefitCalculationOptions lbusBenefitCalculationOptions = lbusPreRetDeathPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationOptions.FirstOrDefault(option => option.icdoBenefitCalculationOptions.benefit_calculation_payee_id == lbusBenefitCalculationPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id);
                        if (ldecBenefitPercentage > 0)
                            UpdateBenefitCalculationOption(lbusBenefitCalculationOptions, ldecBenefitPercentage);
                    }
                    if (idecTotalMGPortion != 0 && ldecBenefitPercentage > 0)
                    {
                        decimal ldecPercentageInterestAmount = Math.Round(idecTotalMGPortion * ldecBenefitPercentage / 100, 6, MidpointRounding.AwayFromZero);
                        lbusPreRetDeathPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount += ldecPercentageInterestAmount;
                        lbusPreRetDeathPayeeAccount.icdoPayeeAccount.Update();
                    }
                    if (ldecBenefitPercentage > 0)
                        UpdatePAPITAmounts(lbusPreRetDeathPayeeAccount, ldecBenefitPercentage); //UPDATE PAPITs
                    lbusPreRetDeathPayeeAccount.iclbPayeeAccountPaymentItemType = null;
                    lbusPreRetDeathPayeeAccount.AdjustRollOverAndTax(); //Adjust rollover and tax
                }
                else if ((lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund)
                && ((lbusPreRetDeathPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusReview())
                || (lbusPreRetDeathPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusPaymentCompleteOrProcessed())
                || (lbusPreRetDeathPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusApproved())) &&
                (lbusPreRetDeathPayeeAccount.iclbPaymentHistoryHeader.Count > 0))
                {
                    if (!lbusPreRetDeathPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusReview())
                    {
                        lbusPreRetDeathPayeeAccount.CreateReviewStatus();
                        lbusPreRetDeathPayeeAccount.iblnAddionalContributionsIndicatorFlag = true;
                        lbusPreRetDeathPayeeAccount.ValidateSoftErrors();
                        lbusPreRetDeathPayeeAccount.UpdateValidateStatus();
                    }
                    InitiateWorkflow(lbusPreRetDeathPayeeAccount.icdoPayeeAccount.payee_perslink_id, 0, lbusPreRetDeathPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.Map_Process_Remainder_Transfer_Refund);
                }
                else if (lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOptionRefund
                    && (!lbusPreRetDeathPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusPaymentCompleteOrProcessed()))
                {
                    LoadTaxableAndNonTaxableAmountsPostedWithThisAdjustment(adtbPostContFromERP);
                    if ((idecTotPensionServiceCredit != 0) || (idecTotVestedServiceCredit != 0) || iblnSalaryAmountChangedForAnyPayPeriodMonth)
                    {
                        if (lbusPreRetDeathPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusApproved() || lbusPreRetDeathPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusReceiving())
                        {
                            lbusPreRetDeathPayeeAccount.CreateReviewStatus();
                            lbusPreRetDeathPayeeAccount.iblnAddionalContributionsIndicatorFlag = true;
                            lbusPreRetDeathPayeeAccount.ValidateSoftErrors();
                            lbusPreRetDeathPayeeAccount.UpdateValidateStatus();
                        }
                        InitiateWorkflow(lbusPreRetDeathPayeeAccount.icdoPayeeAccount.payee_perslink_id, 0, lbusPreRetDeathPayeeAccount.icdoPayeeAccount.payee_account_id,
                                        busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
                    }
                    else if ((idecTotPensionServiceCredit == 0) && (idecTotVestedServiceCredit == 0) && !iblnSalaryAmountChangedForAnyPayPeriodMonth)
                    {
                        if (lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_account_id > 0 &&
                        (idecTotalTaxableAmt != 0 || idecTotalNonTaxableAmt != 0) && !iblnBenefitAccountUpdated)
                        {
                            busGlobalFunctions.UpdateBenActTaxAndNonTaxableAmts(lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_account_id, idecTotalTaxableAmt, idecTotalNonTaxableAmt);
                            iblnBenefitAccountUpdated = true;
                        }
                        //Update minimum guarantee
                        decimal ldecBenefitPercentage = 0.0M;
                        if (lbusPreRetDeathPayeeAccount.ibusBenefitCalculaton.IsNull()) lbusPreRetDeathPayeeAccount.LoadBenefitCalculation();
                        if (lbusPreRetDeathPayeeAccount.ibusBenefitCalculaton.iclbBenefitCalculationPayee.IsNull()) lbusPreRetDeathPayeeAccount.ibusBenefitCalculaton.LoadBenefitCalculationPayee();
                        busBenefitCalculationPayee lbusBenefitCalculationPayee = lbusPreRetDeathPayeeAccount
                                                                                .ibusBenefitCalculaton
                                                                                .iclbBenefitCalculationPayee.
                                                                                FirstOrDefault(i => i.icdoBenefitCalculationPayee.payee_account_id ==
                                                                                    lbusPreRetDeathPayeeAccount.icdoPayeeAccount.payee_account_id);
                        if (lbusBenefitCalculationPayee.IsNotNull())
                        {
                            ldecBenefitPercentage = lbusBenefitCalculationPayee.icdoBenefitCalculationPayee.benefit_percentage;
                        }
                        if (idecTotalMGPortion > 0 && ldecBenefitPercentage > 0)
                        {
                            if (lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption10YearCertain &&
                                lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption15YearCertain &&
                                lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption20YearCertain &&
                                lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption5YearTermLife)
                            {
                                decimal ldecPercentageInterestAmount = Math.Round(idecTotalMGPortion * ldecBenefitPercentage / 100, 6, MidpointRounding.AwayFromZero);
                                lbusPreRetDeathPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount += ldecPercentageInterestAmount;
                                lbusPreRetDeathPayeeAccount.icdoPayeeAccount.Update();
                            }
                        }
                    }
                }
                //Preretirement death payee account, benefit option not refund, annuity option, when payment complete, only initiate workflow.
                else if (lbusPreRetDeathPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOptionRefund &&
                    lbusPreRetDeathPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusPaymentCompleteOrProcessed())
                {
                    InitiateWorkflow(lbusPreRetDeathPayeeAccount.icdoPayeeAccount.payee_perslink_id, 0, lbusPreRetDeathPayeeAccount.icdoPayeeAccount.payee_account_id,
                                    busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
                }
            }
        }

        private void UpdateBenefitCalculationOption(busBenefitCalculationOptions abusBenefitCalculationOptions, decimal adecBenefitPercentage)
        {
            abusBenefitCalculationOptions.icdoBenefitCalculationOptions.interest_amount += Math.Round(((idecTotIntAmt * adecBenefitPercentage) / 100), 6, MidpointRounding.AwayFromZero);
            abusBenefitCalculationOptions.icdoBenefitCalculationOptions.pre_tax_ee_amount += Math.Round(((idecTotPreTaxEEAmt * adecBenefitPercentage) / 100), 6, MidpointRounding.AwayFromZero);
            abusBenefitCalculationOptions.icdoBenefitCalculationOptions.post_tax_ee_amount += Math.Round(((idecTotPostTaxEEAmt * adecBenefitPercentage) / 100), 6, MidpointRounding.AwayFromZero);
            abusBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_rhic_amount += Math.Round(((idecTotEERhicAmt * adecBenefitPercentage) / 100), 6, MidpointRounding.AwayFromZero);
            abusBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_er_pickup_amount += Math.Round(((idecTotEEERPickupAmt * adecBenefitPercentage) / 100), 6, MidpointRounding.AwayFromZero);
            abusBenefitCalculationOptions.icdoBenefitCalculationOptions.er_vested_amount += Math.Round(((idecTotERVestedAmt * adecBenefitPercentage) / 100), 6, MidpointRounding.AwayFromZero);
            abusBenefitCalculationOptions.icdoBenefitCalculationOptions.taxable_amount += Math.Round(((idecTotalTaxableAmt * adecBenefitPercentage) / 100), 6, MidpointRounding.AwayFromZero);
            abusBenefitCalculationOptions.icdoBenefitCalculationOptions.non_taxable_amount += Math.Round(((idecTotalNonTaxableAmt * adecBenefitPercentage) / 100), 6, MidpointRounding.AwayFromZero);
            abusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_option_amount += Math.Round((((idecTotalNonTaxableAmt + idecTotalTaxableAmt) * adecBenefitPercentage) / 100), 2, MidpointRounding.AwayFromZero);
            abusBenefitCalculationOptions.icdoBenefitCalculationOptions.Update();
        }

        private void UpdatePAPITAmounts(busPayeeAccount abusPayeeAccount, decimal adecBenefitPercentage = 0.0M)
        {
            abusPayeeAccount.LoadPayeeAccountPaymentItemType();
            if (abusPayeeAccount.iclbPayeeAccountPaymentItemType.Count > 0)
            {
                if (idecTotPreTaxEEAmt != 0)
                {
                    UpdatePAPITByType(abusPayeeAccount, busConstant.RefundPaymentItemPreTaxEEContributionAmount, adecBenefitPercentage, idecTotPreTaxEEAmt);
                }
                if (idecTotPostTaxEEAmt != 0)
                {
                    UpdatePAPITByType(abusPayeeAccount, busConstant.RefundPaymentItemPostTaxEEContributionAmount, adecBenefitPercentage, idecTotPostTaxEEAmt);
                }
                if (idecTotEEERPickupAmt != 0)
                {
                    UpdatePAPITByType(abusPayeeAccount, busConstant.RefundPaymentItemEEERPickupAmount, adecBenefitPercentage, idecTotEEERPickupAmt);
                }
                if (idecTotIntAmt != 0)
                {
                    UpdatePAPITByType(abusPayeeAccount, busConstant.RefundPaymentItemEEInterestAmount, adecBenefitPercentage, idecTotIntAmt);
                }
                if (idecTotERVestedAmt != 0)
                {
                    UpdatePAPITByType(abusPayeeAccount, busConstant.RefundPaymentItemVestedERContributionAmount, adecBenefitPercentage, idecTotERVestedAmt);
                }
                if (abusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund ||
                    abusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionAutoRefund ||
                    abusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRegularRefund)
                {
                    if (idecTotEERhicAmt != 0)
                    {
                        UpdatePAPITByType(abusPayeeAccount, busConstant.RefundPaymentItemRHICEEAmount, adecBenefitPercentage, idecTotEERhicAmt);
                    }
                }
                if (abusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToDCTransfer ||
                    abusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection ||
                    abusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer ||
                    abusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE ||
                    abusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDualMembers)
                {
                    if (idecTotPreTaxERAmt != 0)
                    {
                        UpdatePAPITByType(abusPayeeAccount, busConstant.RefundPaymentItemERPreTaxAmount, adecBenefitPercentage, idecTotPreTaxERAmt);
                    }
                    if (idecTotERIntAmt != 0)
                    {
                        UpdatePAPITByType(abusPayeeAccount, busConstant.RefundPaymentItemERInterestAmount, adecBenefitPercentage, idecTotERIntAmt);
                    }
                }
            }
        }
        private void UpdatePAPITByType(busPayeeAccount abusPayeeAccount, string astrPAPITItemTypeCode, decimal adecBenefitPercentage, decimal adecPAPITAmt)
        {
            busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = abusPayeeAccount
                                                                                    .iclbPayeeAccountPaymentItemType
                                                                                    .FirstOrDefault(PAPIT => PAPIT
                                                                                    .ibusPaymentItemType
                                                                                    .icdoPaymentItemType.item_type_code ==
                                                                                    astrPAPITItemTypeCode &&
                                                                                    PAPIT.icdoPayeeAccountPaymentItemType.end_date
                                                                                    == DateTime.MinValue);
            decimal ldecPerPayeeAmt = adecBenefitPercentage == 0 ?
                    adecPAPITAmt : Math.Round(((adecPAPITAmt * adecBenefitPercentage) / 100), 6, MidpointRounding.AwayFromZero);
            if (lbusPayeeAccountPaymentItemType.IsNotNull())
            {
                lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount += ldecPerPayeeAmt;
                lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
            }
            else
            {
                if (adecPAPITAmt > 0)
                {
                    abusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPreTaxEEContributionAmount,
                                                               adecPAPITAmt, string.Empty, 0,
                                                               busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1), DateTime.MinValue);
                }
            }
        }

        private void UpdateRetirementAdjustmentContributionsToC(DataTable adtbPostedContributions, bool ablnIsFromEmpPosting)
        {
            if (!ablnIsFromEmpPosting)
            {
                if (_ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                DBFunction.DBNonQuery("cdoPersonAccountRetirementContribution.UpdateContributionTransferFlagToCFromPAadjust",
                                           new object[3] { iobjPassInfo.istrUserID, icdoPersonAccountAdjustment.person_account_adjustment_id, _ibusPersonAccount.icdoPersonAccount.person_account_id },
                                          iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                if (adtbPostedContributions.IsNotNull() && adtbPostedContributions.Rows.Count > 0)
                {
                    foreach (DataRow dr in adtbPostedContributions.Rows)
                    {
                        cdoPersonAccountRetirementContribution lcdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
                        lcdoPersonAccountRetirementContribution.LoadData(dr);
                        lcdoPersonAccountRetirementContribution.transfer_flag_value = busConstant.RetirementContributionTransferFlagC;
                        lcdoPersonAccountRetirementContribution.Update();
                    }
                }
            }
        }

        public Collection<busPersonAccountRetirementContribution> iclbRetContributionsFromThisAdjt { get; set; }
        private void LoadTaxableAndNonTaxableAmountsPostedWithThisAdjustment(DataTable adtbPostedContributions)
        {
            LoadRetContributionsFromThisAdjustment(adtbPostedContributions);
            if (iclbRetContributionsFromThisAdjt.IsNotNull() && iclbRetContributionsFromThisAdjt.Count > 0)
            {
                idecTotPostTaxERAmt = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.post_tax_er_amount);
                idecTotPostTaxEEAmt = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.post_tax_ee_amount);
                idecTotPreTaxERAmt = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.pre_tax_er_amount);
                idecTotPreTaxEEAmt = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.pre_tax_ee_amount);
                idecTotEERhicAmt = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.ee_rhic_amount);
                idecTotERRhicAmt = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.er_rhic_amount);
                idecTotEEERPickupAmt = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.ee_er_pickup_amount);
                idecTotERVestedAmt = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.er_vested_amount);
                idecTotIntAmt = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.interest_amount);
                idecTotERIntAmt = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.employer_interest);
                idecTotVestedServiceCredit = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.vested_service_credit);
                idecTotPensionServiceCredit = iclbRetContributionsFromThisAdjt.Sum(c => c.icdoPersonAccountRetirementContribution.pension_service_credit);
                iblnSalaryAmountChangedForAnyPayPeriodMonth = iclbRetContributionsFromThisAdjt.Any(c => c.icdoPersonAccountRetirementContribution.salary_amount != 0); //PIR 18128
                idecTotalTaxableAmt = idecTotPreTaxEEAmt + idecTotEEERPickupAmt + idecTotIntAmt + idecTotERVestedAmt;
                idecTotalNonTaxableAmt = idecTotPostTaxEEAmt + idecTotEERhicAmt;
                idecTotalMGPortion = idecTotalTaxableAmt + idecTotalNonTaxableAmt;
            }
        }

        private void LoadRetContributionsFromThisAdjustment(DataTable adtbPosteDContributionsFromERP)
        {
            if (iclbRetContributionsFromThisAdjt == null)
            {
                iclbRetContributionsFromThisAdjt = new Collection<busPersonAccountRetirementContribution>();
            }
            DataTable ldtbList = null;
            if (adtbPosteDContributionsFromERP.IsNotNull())
            {
                ldtbList = adtbPosteDContributionsFromERP;
            }
            else
            {
                ldtbList = Select<cdoPersonAccountRetirementContribution>(
                    new string[3]
                    { enmPersonAccountRetirementContribution.person_account_id.ToString(), enmPersonAccountRetirementContribution.subsystem_value.ToString(), enmPersonAccountRetirementContribution.subsystem_ref_id.ToString() }
                    , new object[3] { _ibusPersonAccount.icdoPersonAccount.person_account_id, busConstant.SubSystemValueAdjustment, icdoPersonAccountAdjustment.person_account_adjustment_id }, null, null);
            }
            iclbRetContributionsFromThisAdjt = GetCollection<busPersonAccountRetirementContribution>(ldtbList,
                "icdoPersonAccountRetirementContribution");
        }
        public bool CheckIfEffectiveDateIsBetweenStartAndEndDate(DateTime ldtEffectiveDate)
        {
            DateTime ldtStartDate = _ibusPersonAccount.icdoPersonAccount.start_date;
            DateTime ldtEndDate = _ibusPersonAccount.icdoPersonAccount.end_date;
            if (ldtEndDate == DateTime.MinValue)
                ldtEndDate = DateTime.MaxValue;

            if ((ldtEffectiveDate >= ldtStartDate) && (ldtEffectiveDate < ldtEndDate))
                return true;
            else
                return false;
        }

        /// <summary>
        /// method to initiate workflow
        /// </summary>
        /// <param name="aintPersonID">Person ID</param>
        /// <param name="aintPayeeAccountID">Payee Account ID</param>
        public void InitiateWorkflow(int aintPersonID, int aintBeneficiaryID, int aintReferenceID, int aintTypeID)
        {
            if (!busWorkflowHelper.IsActiveInstanceAvailable(aintPersonID, aintTypeID))
            {
                Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                ldctParams["additional_parameter1"] = aintBeneficiaryID > 0 ? aintBeneficiaryID.ToString() : "";
                busWorkflowHelper.InitiateBpmRequest(aintTypeID, aintPersonID, 0, aintReferenceID, iobjPassInfo, busConstant.WorkflowProcessSource_Batch, adictInstanceParameters: ldctParams);

            }
        }
        //UAT PIR - 1710
        public override bool SeBpmActivityInstanceReferenceID()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            if (ibusBaseActivityInstance.IsNotNull())
            {

                ibusPersonAccount.ibusPerson.ibusBaseActivityInstance = ibusBaseActivityInstance;
                ibusPersonAccount.ibusPerson.SetFlagIs1099RExits();
                ibusPersonAccount.ibusPerson.SeBpmActivityInstanceReferenceID();
                // ibusPersonAccount.ibusPerson.SetProcessInstanceParameters();
                ibusPersonAccount.ibusPerson.SetCaseInstanceParameters();
            }
            return true;
        }
    }

}
