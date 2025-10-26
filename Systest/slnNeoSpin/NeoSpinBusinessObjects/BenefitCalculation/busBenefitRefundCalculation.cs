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
using System.Linq.Expressions;
using System.Collections.Generic;
using Sagitec.Bpm;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitRefundCalculation : busBenefitRefundCalculationGen
    {
        private string istrRHICRefundFlag = string.Empty;
        public DateTime payment_date { get; set; }
        public void LoadPaymentDate()
        {
            if (ibusRefundBenefitApplication == null)
                LoadRefundBenefitApplication();
            DateTime ldtPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);

            if (ibusRefundBenefitApplication.icdoBenefitApplication.payment_date > ldtPaymentDate)
            {
                payment_date = ibusRefundBenefitApplication.icdoBenefitApplication.payment_date;
            }
            else
            {
                payment_date = ldtPaymentDate;
            }
        }
        //Visible Rule For Er Pre Tax and ER Interest Amount
        public bool IsERPreTaxAmountEnterable()
        {
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB)
            {
                if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToDCTransfer || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection)
                {
                    if (ibusPersonAccount == null)
                        LoadPersonAccount();
                    DateTime ldtPersLinkGoLiveDate = busPayeeAccountHelper.GetPERSLinkGoLiveDate();
                    if ((ldtPersLinkGoLiveDate != DateTime.MinValue) && (ibusPersonAccount.icdoPersonAccount.start_date < ldtPersLinkGoLiveDate))
                    {
                        return true;
                    }
                }
                else if (IsBenefitOptionTFFROrTIAA())
                {
                    return true;
                }
            }
            return false;
        }
        //Load all default application values into calculation values
        public void LoadDefaultValues(bool ablnIsRegular)
        {
            if (ablnIsRegular)
            {
                icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
            }
            else
            {
                icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeAdjustments;
            }
            icdoBenefitCalculation.calculation_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, icdoBenefitCalculation.calculation_type_value);
            icdoBenefitCalculation.benefit_account_type_value = ibusRefundBenefitApplication.icdoBenefitApplication.benefit_account_type_value; ;
            icdoBenefitCalculation.benefit_option_value = ibusRefundBenefitApplication.icdoBenefitApplication.benefit_option_value;
            icdoBenefitCalculation.benefit_option_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2216, icdoBenefitCalculation.benefit_option_value);
            icdoBenefitCalculation.benefit_account_sub_type_value = ibusRefundBenefitApplication.icdoBenefitApplication.benefit_sub_type_value;
            icdoBenefitCalculation.termination_date = ibusRefundBenefitApplication.icdoBenefitApplication.termination_date;
            icdoBenefitCalculation.plso_requested_flag = busConstant.Flag_No;
            icdoBenefitCalculation.reduced_benefit_flag = busConstant.Flag_No;
            icdoBenefitCalculation.uniform_income_or_ssli_flag = busConstant.Flag_No;
            icdoBenefitCalculation.rhic_option_value = string.Empty;
            icdoBenefitCalculation.benefit_account_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, icdoBenefitCalculation.benefit_account_type_value);
            icdoBenefitCalculation.status_value = busConstant.ApplicationStatusReview;
            icdoBenefitCalculation.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1902, busConstant.ApplicationStatusReview);
            icdoBenefitCalculation.action_status_value = busConstant.BenefitActionStatusPending;
            icdoBenefitCalculation.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, busConstant.BenefitActionStatusPending);
            icdoBenefitRefundCalculation.calculation_date = DateTime.Now;
            icdoBenefitCalculation.created_date = DateTime.Now;
        }
        //Calculate all amount fields as per business rules
        public void CalculateRefund()
        {
            decimal ldecPreTaxEEamount = 0.0M, ldecPostTaxEEAmount = 0.0M, ldecEEERPickupAmount = 0.0M, ldecInterest = 0.0M,
                ldecERVestedAmount = 0.0M, ldecRhicEEAmount = 0.0M, ldecERPretaxAmount = 0.0M;
            //PIR 1468 Reduce the refund amount by QDRO amount
            CalculateQDROAmount();

            if (ibusBenefitApplication == null)
                LoadBenefitApplication();
            if (ibusBenefitApplication.iclbBenefitApplicationPersonAccounts == null)
                ibusBenefitApplication.LoadBenefitApplicationPersonAccount();
            if (ibusPlan == null)
                LoadPlan();
            //uat pir 1419 ;  take benefit application person account with person account flag = Y
            IEnumerable<busBenefitApplicationPersonAccount> lenmBAPA = ibusBenefitApplication.iclbBenefitApplicationPersonAccounts
                                                                            .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes);
            foreach (busBenefitApplicationPersonAccount lobjBenefitApplicationPersonAccount in lenmBAPA)
            {
                lobjBenefitApplicationPersonAccount.LoadPersonAccount();
                lobjBenefitApplicationPersonAccount.LoadPersonAccountRetirement();
                lobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount = lobjBenefitApplicationPersonAccount.ibusPersonAccount.icdoPersonAccount;
                //Additional requirement added as per discussion with satya.
                icdoBenefitRefundCalculation.capital_gain = lobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain - dro_capital_gain;
                //For the TFFR transfer options ,take sum of contribution amounts based on the period specified.
                if ((icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE) ||
                    (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDualMembers))
                {
                    if (ibusBenefitApplication.iclbBenefitApplicationDbTffrTransfer == null)
                        ibusBenefitApplication.LoadBenefitApplicationDbTffrTransfer();
                    foreach (busBenefitApplicationDbTffrTransfer lobjBenefitApplicationDbTffrTransfer in ibusBenefitApplication.iclbBenefitApplicationDbTffrTransfer)
                    {
                        lobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.LoadContributiomSummaryByPeriod(lobjBenefitApplicationDbTffrTransfer.icdoBenefitApplicationDbTffrTransfer.start_date,
                            lobjBenefitApplicationDbTffrTransfer.icdoBenefitApplicationDbTffrTransfer.end_date, true);
                        CalculateContributionAmounForTFFR(lobjBenefitApplicationPersonAccount, ref ldecPreTaxEEamount, ref ldecPostTaxEEAmount, ref ldecEEERPickupAmount, ref ldecInterest, ref ldecERVestedAmount, ref ldecRhicEEAmount, ref ldecERPretaxAmount);
                    }
                    CalculateContributionAmount(ref ldecPreTaxEEamount, ref ldecPostTaxEEAmount, ref ldecEEERPickupAmount, ref ldecInterest, ref ldecERVestedAmount, ref ldecRhicEEAmount, ref ldecERPretaxAmount);
                }
                //For Regular Refund and DB to TIAA and DB to DC Transfer options ,take sum of all contribution amounts and display in the respective fields
                else
                {
                    lobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.LoadLTDSummary();
                    if (ibusPlan.IsDBRetirementPlan())
                    {
                        CalculateContributionAmount(lobjBenefitApplicationPersonAccount, ref ldecPreTaxEEamount, ref ldecPostTaxEEAmount, ref ldecEEERPickupAmount, ref ldecInterest, ref ldecERVestedAmount, ref ldecRhicEEAmount, ref ldecERPretaxAmount);
                        CalculateContributionAmount(ref ldecPreTaxEEamount, ref ldecPostTaxEEAmount, ref ldecEEERPickupAmount, ref ldecInterest, ref ldecERVestedAmount, ref ldecRhicEEAmount, ref ldecERPretaxAmount);
                        //PIR 25920 Part 2 - benefit application and calculation fro DBDC Transfer
                        if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection)
                        {
                            CalculateCalculatedActuarialValue(lobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement);
                            CalculateDBAccountBalance(lobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement);
                        }
                    }
                    //If the plan is Dc ,then Calculate only EE RHIC Amount
                    else
                    {
                        icdoBenefitRefundCalculation.rhic_ee_amount = lobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.ee_rhic_amount_ltd + lobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.ee_rhic_ser_pur_cont_ltd;
                    }
                }
            }
        }
        //Assign the calculated amount to the respective table fields
        private void CalculateContributionAmount(ref decimal adecPreTaxEEamount, ref decimal adecPostTaxEEAmount, ref decimal adecEEERPickupAmount,
            ref decimal adecInterest, ref decimal adecERVestedAmount, ref decimal adecRhicEEAmount, ref decimal adecERPretaxAmount)
        {
            icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount = adecPreTaxEEamount - dro_ee_pretax_amount;
            icdoBenefitRefundCalculation.post_tax_ee_contribution_amount = adecPostTaxEEAmount - dro_ee_posttax_amount;
            icdoBenefitRefundCalculation.ee_er_pickup_amount = adecEEERPickupAmount - dro_ee_er_pickup_amount;
            icdoBenefitRefundCalculation.ee_interest_amount = adecInterest > dro_interest_amount ? adecInterest - dro_interest_amount :  dro_interest_amount == 0.0M ? adecInterest : 0.00M; //PIR 23806
            icdoBenefitRefundCalculation.vested_er_contribution_amount = adecERVestedAmount - dro_er_vested_amount;
            icdoBenefitRefundCalculation.rhic_ee_amount = adecRhicEEAmount;
            //Do not calculate ER Pretax amount For Regular and Auto Refund
            if (!IsBenefitOptionRegularRefundOrAutoRefund())
            {
                icdoBenefitRefundCalculation.er_pre_tax_amount = adecERPretaxAmount;
            }
        }
        //Calculate contribution amounts
        private void CalculateContributionAmount(busBenefitApplicationPersonAccount aobjBenefitApplicationPersonAccount, ref decimal adecPreTaxEEamount,
                ref decimal adecPostTaxEEAmount, ref decimal adecEEERPickupAmount, ref decimal adecInterest, ref decimal adecERVestedAmount,
                ref decimal adecRhicEEAmount, ref decimal adecERPretaxAmount)
        {
            adecPreTaxEEamount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.pre_tax_ee_amount_ltd + aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.pre_tax_ee_ser_pur_cont_ltd;
            adecPostTaxEEAmount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.post_tax_ee_amount_ltd + aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.post_tax_ee_ser_pur_cont_ltd;
            adecEEERPickupAmount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.ee_er_pickup_amount_ltd + aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.ee_er_pickup_ser_pur_cont_ltd;
            adecInterest += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.interest_amount_ltd;
            adecERVestedAmount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.er_vested_amount_ltd;
            adecRhicEEAmount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.ee_rhic_amount_ltd + aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.ee_rhic_ser_pur_cont_ltd;
            adecERPretaxAmount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.pre_tax_er_amount_ltd + aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.pre_tax_er_ser_pur_cont_ltd;
        }
        private void CalculateContributionAmounForTFFR(busBenefitApplicationPersonAccount aobjBenefitApplicationPersonAccount, ref decimal adecPreTaxEEamount,
                ref decimal adecPostTaxEEAmount, ref decimal adecEEERPickupAmount, ref decimal adecInterest, ref decimal adecERVestedAmount,
                ref decimal adecRhicEEAmount, ref decimal adecERPretaxAmount)
        {
            adecPreTaxEEamount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.pre_tax_ee_amount + aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.pre_tax_ee_ser_pur_cont;
            adecPostTaxEEAmount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.post_tax_ee_amount + aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.post_tax_ee_ser_pur_cont;
            adecEEERPickupAmount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.ee_er_pickup_amount + aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.ee_er_pickup_ser_pur_cont;
            adecInterest += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.interest_amount;
            adecERVestedAmount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.er_vested_amount;
            adecRhicEEAmount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.ee_rhic_amount + aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.ee_rhic_ser_pur_cont;
            adecERPretaxAmount += aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.pre_tax_er_amount + aobjBenefitApplicationPersonAccount.ibusPersonAccountRetirement.pre_tax_er_ser_pur_cont;
        }

        //Calculate ER Interest for ER Pre Tax Amount
        public void CalculateInterestForERPreTaxAmount()
        {
            if (!IsBenefitOptionRegularRefundOrAutoRefund())
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                DateTime ldtFirstContDate = DateTime.MinValue;
                //add month minus from last interest batch run date as per discussion with satya on 26/11/2009
                DateTime ldtInterestBatchLastRunDate = busInterestCalculationHelper.GetInterestBatchLastRunDate() != DateTime.MinValue ?
                    busInterestCalculationHelper.GetInterestBatchLastRunDate().AddMonths(-1) : DateTime.Today.AddMonths(-2);
                Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution = LoadContributions();
                if ((lclbPersonAccountRetirementContribution != null) && (lclbPersonAccountRetirementContribution.Count > 0))
                {
                    ldtFirstContDate = new DateTime(lclbPersonAccountRetirementContribution[0].icdoPersonAccountRetirementContribution.effective_date.Year,
                        lclbPersonAccountRetirementContribution[0].icdoPersonAccountRetirementContribution.effective_date.Month, 1);
                }
                decimal ldecTotalPreTaxERInterest = CalculateInterest(ldtFirstContDate, ldtInterestBatchLastRunDate, lclbPersonAccountRetirementContribution);
                icdoBenefitRefundCalculation.er_interest_amount = Math.Round(ldecTotalPreTaxERInterest, 2, MidpointRounding.AwayFromZero);
            }
        }
        //Calculate Additional Interest for ER Pre Tax Amount
        public void CalculateAddiionalInterestForERPreTaxAmount()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            DateTime ldtFirstContDate = ibusPersonAccount.GetLastcontibutionDate();
            DateTime ldtNextPayementDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution = LoadContributions();
            if (ldtFirstContDate != DateTime.MinValue)
            {
                decimal ldecTotalPreTaxERInterest = CalculateInterest(ldtFirstContDate, ldtNextPayementDate, lclbPersonAccountRetirementContribution);
                icdoBenefitRefundCalculation.additional_er_amount_interest = Math.Round(ldecTotalPreTaxERInterest, 2, MidpointRounding.AwayFromZero);
            }
        }
        private Collection<busPersonAccountRetirementContribution> LoadContributions()
        {
            busPersonAccountRetirement lobjPersonAccountRetirement = new busPersonAccountRetirement();
            lobjPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
            lobjPersonAccountRetirement.LoadRetirementContributionAll();
            Collection<busPersonAccountRetirementContribution> lclbPersonAccountRetirementContribution
                = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>("icdoPersonAccountRetirementContribution.effective_date", lobjPersonAccountRetirement.iclbRetirementContributionAll);
            return lclbPersonAccountRetirementContribution;
        }

        private decimal CalculateInterest(DateTime adtFirstContDate, DateTime adtNextPayementDate, Collection<busPersonAccountRetirementContribution> aclbPersonAccountRetirementContribution)
        {
            decimal ldecPreTaxERAmount = 0.0M, ldecPreTaxERInterest = 0.0M, ldecTotalPreTaxERAmount = 0.0M, ldecTotalPreTaxERInterest = 0.0M;
            List<busPersonAccountRetirementContribution> llstcontributions = new List<busPersonAccountRetirementContribution>();
            foreach (busPersonAccountRetirementContribution lobjPersonAccountRetirementContribution in aclbPersonAccountRetirementContribution)
            {
                llstcontributions.Add(lobjPersonAccountRetirementContribution);
            }
            while (adtFirstContDate <= adtNextPayementDate)
            {
                busPersonAccountRetirementContribution lobjPersonAccountRetirementContribution = GetContributionRecord(llstcontributions, adtFirstContDate);
                if (IsBenefitOptionTFFR())
                {
                    if (IsItValidPeriodToCalculateInterestForTFFR(adtFirstContDate))
                    {
                        CalculateInterest(ref ldecPreTaxERAmount, ref ldecPreTaxERInterest, ref ldecTotalPreTaxERAmount, ref ldecTotalPreTaxERInterest, lobjPersonAccountRetirementContribution);
                    }
                    else
                    {
                        CalculateInterest(ref ldecPreTaxERAmount, ref ldecPreTaxERInterest, ref ldecTotalPreTaxERAmount, ref ldecTotalPreTaxERInterest, 0.0M, adtFirstContDate);
                    }
                }
                else
                {
                    CalculateInterest(ref ldecPreTaxERAmount, ref ldecPreTaxERInterest, ref ldecTotalPreTaxERAmount, ref ldecTotalPreTaxERInterest, lobjPersonAccountRetirementContribution);
                }
                adtFirstContDate = adtFirstContDate.AddMonths(1);
            }
            return ldecTotalPreTaxERInterest;
        }

        public busPersonAccountRetirementContribution GetContributionRecord(List<busPersonAccountRetirementContribution> alstcontributions, DateTime adtContributionDate)
        {
            busPersonAccountRetirementContribution lbusContribution = new busPersonAccountRetirementContribution();
            lbusContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            /* .. load objects .. */
            List<busPersonAccountRetirementContribution> llstcontributions =
                alstcontributions.FindAll(delegate(busPersonAccountRetirementContribution lobjTemp)
                {
                    return lobjTemp.icdoPersonAccountRetirementContribution.effective_date.Month == adtContributionDate.Month
                    && lobjTemp.icdoPersonAccountRetirementContribution.effective_date.Year == adtContributionDate.Year;
                });
            if (llstcontributions != null)
            {
                if ((llstcontributions.Count == 1) && (llstcontributions[0].icdoPersonAccountRetirementContribution.retirement_contribution_id > 0))
                {
                    lbusContribution = llstcontributions[0];
                }
                else
                {
                    foreach (busPersonAccountRetirementContribution lobjContribution in llstcontributions)
                    {
                        lbusContribution.icdoPersonAccountRetirementContribution.retirement_contribution_id = lobjContribution.icdoPersonAccountRetirementContribution.retirement_contribution_id;
                        lbusContribution.icdoPersonAccountRetirementContribution.effective_date = lobjContribution.icdoPersonAccountRetirementContribution.effective_date;
                        lbusContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount += lobjContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount;
                        lbusContribution.icdoPersonAccountRetirementContribution.pay_period_year = lobjContribution.icdoPersonAccountRetirementContribution.pay_period_year;
                        lbusContribution.icdoPersonAccountRetirementContribution.pay_period_month = lobjContribution.icdoPersonAccountRetirementContribution.pay_period_month;
                    }
                }
            }
            return lbusContribution;
        }

        public bool IsItValidPeriodToCalculateInterestForTFFR(DateTime adtFirstContDate)
        {
            if (ibusRefundBenefitApplication == null)
                LoadRefundBenefitApplication();
            if (ibusRefundBenefitApplication.iclbBenefitApplicationDbTffrTransfer == null)
                ibusRefundBenefitApplication.LoadBenefitApplicationDbTffrTransfer();
            foreach (busBenefitApplicationDbTffrTransfer lobjTFFR in ibusRefundBenefitApplication.iclbBenefitApplicationDbTffrTransfer)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adtFirstContDate, lobjTFFR.icdoBenefitApplicationDbTffrTransfer.start_date, lobjTFFR.icdoBenefitApplicationDbTffrTransfer.end_date))
                {
                    return true;
                }
            }
            return false;
        }
        private void CalculateInterest(ref decimal adecPreTaxERAmount, ref decimal adecPreTaxERInterest, ref decimal adecTotalPreTaxERAmount, ref decimal adecTotalPreTaxERInterest, busPersonAccountRetirementContribution aobjPersonAccountRetirementContribution)
        {
            decimal ldecIntialPreTaxERAmount = aobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount;
            CalculateInterest(ref adecPreTaxERAmount, ref adecPreTaxERInterest, ref adecTotalPreTaxERAmount, ref adecTotalPreTaxERInterest, ldecIntialPreTaxERAmount, aobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date);
        }

        private void CalculateInterest(ref decimal adecPreTaxERAmount, ref decimal adecPreTaxERInterest, ref decimal adecTotalPreTaxERAmount, ref decimal adecTotalPreTaxERInterest, decimal adecIntialPreTaxERAmount, DateTime adtEffectiveDate)
        {
            if (ibusRefundBenefitApplication == null)
                LoadRefundBenefitApplication();
            adecPreTaxERAmount = adecPreTaxERAmount + adecPreTaxERInterest + adecIntialPreTaxERAmount;
            if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer)
            {
                adecPreTaxERInterest = busInterestCalculationHelper.CalculateInterestForFlatPercentage(adecPreTaxERAmount, adtEffectiveDate);
            }
            else
            {
                adecPreTaxERInterest = busInterestCalculationHelper.CalculateInterest(adecPreTaxERAmount, ibusRefundBenefitApplication.icdoBenefitApplication.plan_id, adtEffectiveDate);
            }
            adecTotalPreTaxERAmount = adecTotalPreTaxERAmount + adecIntialPreTaxERAmount;
            adecTotalPreTaxERInterest = adecPreTaxERInterest + adecTotalPreTaxERInterest;
        }

        public ArrayList btnApproveClicked()
        {
            ArrayList larrList = new ArrayList();
            if (ibusMember == null)
                LoadMember();
            ValidateApprove(larrList);
            if (larrList.Count == 0)
            {
                if (ibusBenefitApplication == null)
                    LoadBenefitApplication();
                if (ibusRefundBenefitApplication == null)
                    LoadRefundBenefitApplication();
                if (ibusRefundBenefitApplication.iclbPayeeAccount == null)
                    ibusRefundBenefitApplication.LoadPayeeAccount();
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if ((ibusRefundBenefitApplication.iclbPayeeAccount.Count == 0) && (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal))
                {
                    ManageRefundsPayeeAccount(null);
                }
                else if ((ibusRefundBenefitApplication.iclbPayeeAccount.Count > 0) && (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments))
                {
                    if ((IsBenefitOptionRegularRefundOrAutoRefund()) || (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToDCTransfer || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection))
                    {
                        if (ibusRefundBenefitApplication.iclbPayeeAccount[0].ibusPayeeAccountActiveStatus == null)
                        {
                            ibusRefundBenefitApplication.iclbPayeeAccount[0].LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                        }
                        if (ibusRefundBenefitApplication.iclbPayeeAccount[0].ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value != busConstant.PayeeAccountStatusRefundCancelled)
                        {
                            ManageAdjustmentRefundsPayeeAccount(ibusRefundBenefitApplication.iclbPayeeAccount[0]);
                        }
                    }
                    else
                    {
                        foreach (busPayeeAccount lobjPayeeAccount in ibusRefundBenefitApplication.iclbPayeeAccount)
                        {
                            if (lobjPayeeAccount.ibusPayeeAccountActiveStatus == null)
                            {
                                lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                            }
                            if (ibusRefundBenefitApplication.iclbPayeeAccount[0].ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value != busConstant.PayeeAccountStatusRefundCancelled)
                            {
                                ManageAdjustmentRefundsPayeeAccount(lobjPayeeAccount);
                            }
                        }
                    }
                }
                icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusApproval;
                icdoBenefitCalculation.Update();

                //PIR 18974
                IsBenefitOverpaymentBPMInitiate();

                LoadPayeeAccountID(true);
                if (ibusBaseActivityInstance.IsNotNull())
                {
                    //SetProcessInstanceParameters();
                    SetCaseInstanceParameters();
                    if (iintPayeeAccountID > 0)
                    {
                        busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                        lobjPayeeAccount.FindPayeeAccount(iintPayeeAccountID);
                        if (lobjPayeeAccount.ibusBaseActivityInstance.IsNull())
                            lobjPayeeAccount.ibusBaseActivityInstance = ClassMapper.GetObject<busBpmActivityInstance>();
                        lobjPayeeAccount.ibusBaseActivityInstance = ibusBaseActivityInstance;
                        // lobjPayeeAccount.SetProcessInstanceParameters();
                        lobjPayeeAccount.SetCaseInstanceParameters();
                    }
                }
                //PIR 23806 - Calculate Overpayment for refunds
                if (iintPayeeAccountID > 0 && icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments && 
                    icdoBenefitRefundCalculation.total_refund_amount < 0.0M)
                {
                    CreateOverpaymentForRefundsOnApprove();
                }

                //PIR 18493 - WSS Application Wizard
                if (Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.IsWSSAppWizardVisible, iobjPassInfo)).ToUpper() == busConstant.Flag_Yes
                    && iintPayeeAccountID > 0 && icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionRegularRefund)
                {
                    InitiatePostCreatePayeeAccountProcess(iintPayeeAccountID);
                }
                //PIR 25084 - Create Tax withholding Records Automatically On Calc Approval
                if (iintPayeeAccountID > 0 && iintBenAppID == 0)
                   CreateDefaultWithHolding(iintPayeeAccountID);
            }
            return larrList;
        }


        private void ManageAdjustmentRefundsPayeeAccount(busPayeeAccount aobjPayeeAccount)
        {
            EndExistingWithholding(aobjPayeeAccount);
            ManageRefundsPayeeAccount(aobjPayeeAccount);
            aobjPayeeAccount.icdoPayeeAccount.Select();
            if (aobjPayeeAccount.ibusSoftErrors == null)
                aobjPayeeAccount.LoadErrors();
            aobjPayeeAccount.iblnClearSoftErrors = false;
            aobjPayeeAccount.ibusSoftErrors.iblnClearError = false;
            aobjPayeeAccount.iblnAddionalContributionsIndicatorFlag = true;
            aobjPayeeAccount.ValidateSoftErrors();
            aobjPayeeAccount.UpdateValidateStatus();
            if (aobjPayeeAccount.ibusPayeeAccountActiveStatus != null)
                aobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.Select();
            else
                aobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();

            aobjPayeeAccount.CreateReviewPayeeAccountStatus();
            if (aobjPayeeAccount.iclbActiveRolloverDetails == null)
                aobjPayeeAccount.LoadActiveRolloverDetail();
            if (aobjPayeeAccount.iclbActiveRolloverDetails.Count > 0)
            {
                aobjPayeeAccount.CreateRolloverAdjustment();
            }
            else
            {
                aobjPayeeAccount.CalculateAdjustmentTax(true);
            }
        }

        private void EndExistingWithholding(busPayeeAccount aobjPayeeAccount)
        {
            if (aobjPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
            {
                aobjPayeeAccount.LoadTaxWithHoldingHistory();
                if (aobjPayeeAccount.idtNextBenefitPaymentDate != DateTime.MinValue)
                {
                    DateTime ldteWithholdingEndDate = aobjPayeeAccount.idtNextBenefitPaymentDate.AddMonths(-1).GetLastDayofMonth();
                    foreach (busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding in aobjPayeeAccount.iclbTaxWithholingHistory)
                    {
                        if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue)
                        {
                            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date = ldteWithholdingEndDate;
                            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.Update();

                            // To delete old Items
                            lbusPayeeAccountTaxWithholding.DeleteTaxItems();
                        }                        
                    }
                }
            }
        }

        private void ManageRefundsPayeeAccount(busPayeeAccount aobjPayeeAccount)
        {
            //PIR 1131 - Set the benefit Begin date
            if (payment_date == DateTime.MinValue)
                LoadPaymentDate();

            DateTime ldtPaymentDate = payment_date;

            LoadPersonAccountInfo();

            decimal ldecDRONontaxableBeginningBalanceAmount = 0.0m;
            if (ibusRefundBenefitApplication.IsBenefitDROApplicationExist())
            {
                InitiateQDROWorkflow();
            }
            //PIR 1779 - do not post starting non taxable beginning balance for Transfers
            if (ibusRefundBenefitApplication.IsBenefitOptionRegularRefundOrAutoRefund())
            {
                ldecDRONontaxableBeginningBalanceAmount = ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd - dro_ee_posttax_amount;
            }
            //Check whether benefit account exists for the member ,if exists ,use the same benefit account id for the payee account creation 
            int lintBenefitAccountID = IsBenefitAccountExists(ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                              icdoBenefitCalculation.benefit_account_type_value);
            if (lintBenefitAccountID > 0)
            {
                lintBenefitAccountID = ManageBenefitAccount(ibusPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd,
                                                            ldecDRONontaxableBeginningBalanceAmount, busConstant.StatusValid, string.Empty, 0.0M, 0.0M, 0.0M,
                                                            ibusBenefitApplication.icdoBenefitApplication.retirement_org_id, lintBenefitAccountID,
                                                            DateTime.MinValue, 0.0M, string.Empty, 0M, 0M);
                CreatePayeeAccount(lintBenefitAccountID, ldtPaymentDate, ldecDRONontaxableBeginningBalanceAmount, aobjPayeeAccount);
            }
            else //if benefit account does not exist for the member ,then create a new benefit account and payee account
            {
                lintBenefitAccountID = ManageBenefitAccount(ibusPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd,
                                                            ldecDRONontaxableBeginningBalanceAmount, busConstant.StatusValid, string.Empty, 0.0M, 0.0M, 0.0M,
                                                            ibusBenefitApplication.icdoBenefitApplication.retirement_org_id, 0,
                                                            DateTime.MinValue, 0.0M, string.Empty, 0M, 0M);
                CreatePayeeAccount(lintBenefitAccountID, ldtPaymentDate, ldecDRONontaxableBeginningBalanceAmount, aobjPayeeAccount);
            }
        }
        //PIR 1468 Reduce the refund amount by QDRO amount
        public void CalculateQDROAmount()
        {
            busPersonBase lobjPersonBase = new busPersonBase();
            decimal ldecqdroeeposttaxamount = 0.0m, ldecqdroeepretaxamount = 0.0m, ldeceeerpickupamount = 0.0m, ldecqdroervestedamount = 0.0m, ldecqdrointerestamount = 0.0m,
                ldecqdrocapitalGain = 0.0m, ldecqdrototalamount = 0.0m, ldecqdrototaltaxableamount = 0.0m, ldectotalnontaxableamount = 0.0m,
                ldecQDROAdditionalInterest = 0.0M; //PROD PIR 4800
            lobjPersonBase.CalculateQDROAmount(true, icdoBenefitCalculation.person_id, icdoBenefitCalculation.plan_id, ref ldecqdroeeposttaxamount, ref ldecqdroeepretaxamount, ref ldeceeerpickupamount,
                ref ldecqdroervestedamount, ref ldecqdrointerestamount, ref ldecqdrocapitalGain, ref ldecqdrototalamount, ref ldecqdrototaltaxableamount, ref ldectotalnontaxableamount,
                ref ldecQDROAdditionalInterest);
            dro_ee_posttax_amount = ldecqdroeeposttaxamount;
            dro_ee_pretax_amount = ldecqdroeepretaxamount;
            dro_er_vested_amount = ldecqdroervestedamount;
            dro_interest_amount = ldecqdrointerestamount + ldecQDROAdditionalInterest; //PROD PIR 4800
            dro_ee_er_pickup_amount = ldeceeerpickupamount;
            dro_capital_gain = ldecqdrocapitalGain;
        }
        private void InitiateQDROWorkflow()
        {
            if (ibusRefundBenefitApplication.iclbBenefitDROApplication == null)
                ibusRefundBenefitApplication.LoadDROApplication();
            foreach (busBenefitDroApplication lobjBenefitDROAppilcation in ibusRefundBenefitApplication.iclbBenefitDROApplication)
            {
                if (!lobjBenefitDROAppilcation.IsDROApplicationCancelledOrDenied())
                {

                    if (lobjBenefitDROAppilcation.ibusBenefitDroCalculation == null)
                        lobjBenefitDROAppilcation.ibusBenefitDroCalculation = new busBenefitDroCalculation();
                    lobjBenefitDROAppilcation.ibusBenefitDroCalculation.FindBenefitDroCalculationByApplicationID(lobjBenefitDROAppilcation.icdoBenefitDroApplication.dro_application_id);

                    if (lobjBenefitDROAppilcation.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified)
                    {
                        if ((lobjBenefitDROAppilcation.icdoBenefitDroApplication.work_flow_intiated_flag == busConstant.Flag_No) || (lobjBenefitDROAppilcation.icdoBenefitDroApplication.work_flow_intiated_flag == null))
                        {
                            lobjBenefitDROAppilcation.InitiateDROWorkflow();
                            lobjBenefitDROAppilcation.icdoBenefitDroApplication.work_flow_intiated_flag = busConstant.Flag_Yes;
                            lobjBenefitDROAppilcation.icdoBenefitDroApplication.Update();
                        }
                    }
                }
            }
        }

       
        public override ArrayList  btnCancelClick() 
        {
            ArrayList larrList = new ArrayList();
    		larrList = base.btnCancelClick();
            if (larrList.Count == 0)
            {
                  //PIR 11946
                ResetCalculationTransferFlagToNull(ibusBenefitApplication.icdoBenefitApplication.benefit_application_id, busConstant.RetirementContributionTransferFlagC);
                larrList.Add(this);
            }
            return larrList;
        }

        //PIR 11946
        public int ResetCalculationTransferFlagToNull(int aintApplicationID, string astrTransferFlagFrom)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoBenefitCalculation.UpdateContributionTransferFlagToNull",
                                       new object[3] { aintApplicationID, iobjPassInfo.istrUserID, astrTransferFlagFrom },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }

        private void LoadPersonAccountInfo()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonAccountRetirement == null)
                ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
            ibusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
            ibusPersonAccount.ibusPersonAccountRetirement.LoadLTDSummary();
        }

        private void CreatePayeeAccount(int lintBenefitAccountID, DateTime adtPaymentDate, decimal adecDRONontaxableBeginningBalanceAmount, busPayeeAccount aobjPayeeAccount)
        {
            int lintPayeeAccountId = 0;
            if (aobjPayeeAccount != null)
                lintPayeeAccountId = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
            if (icdoBenefitRefundCalculation.total_refund_amount == 0.0M)
                CalculateTotalRefundAmountForRegulaOrAutoRefund();

            if (icdoBenefitRefundCalculation.total_transfer_amount == 0.0M)
                CalculateTotalAmountForTransferOptions();
            if ((icdoBenefitRefundCalculation.total_refund_amount > 0.00M) || (icdoBenefitRefundCalculation.total_transfer_amount > 0.00M))
            {
                if (IsBenefitOptionTFFROrTIAA())
                {
                    if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments) && (aobjPayeeAccount != null))
                    {
                        if (!aobjPayeeAccount.IsitRefundRHICPayeeAccount())
                        {
                            CreatePayeeAccountDetails(lintBenefitAccountID, adtPaymentDate, adecDRONontaxableBeginningBalanceAmount, lintPayeeAccountId);
                        }
                        else
                        {
                            //Create Separate Payee Account for RHIC EE Amount if Benefit option is DB To TFFR Transfer For DP-CTE or DB To TFFR Transfer For Dual Members
                            //or DB To TIAA - CREF Transfer
                            if (icdoBenefitRefundCalculation.rhic_ee_amount > 0)
                            {
                                istrRHICRefundFlag = busConstant.Flag_Yes;
                                int lintPayeeAccoutId = CreatePayeeAccountAndStatus(lintBenefitAccountID, adtPaymentDate, adecDRONontaxableBeginningBalanceAmount, lintPayeeAccountId);
                                CreatePayeeAccountItemForRHICAmount(lintPayeeAccoutId, adtPaymentDate);
                            }
                        }
                    }
                    else
                    {
                        CreatePayeeAccountDetails(lintBenefitAccountID, adtPaymentDate, adecDRONontaxableBeginningBalanceAmount, lintPayeeAccountId);

                        //Create Separate Payee Account for RHIC EE Amount if Benefit option is DB To TFFR Transfer For DP-CTE or DB To TFFR Transfer For Dual Members
                        //or DB To TIAA - CREF Transfer
                        if (icdoBenefitRefundCalculation.rhic_ee_amount > 0)
                        {
                            istrRHICRefundFlag = busConstant.Flag_Yes;
                            int lintPayeeAccoutId = CreatePayeeAccountAndStatus(lintBenefitAccountID, adtPaymentDate, adecDRONontaxableBeginningBalanceAmount, lintPayeeAccountId);
                            CreatePayeeAccountItemForRHICAmount(lintPayeeAccoutId, adtPaymentDate);
                        }
                    }
                }
                else
                {
                    CreatePayeeAccountDetails(lintBenefitAccountID, adtPaymentDate, adecDRONontaxableBeginningBalanceAmount, lintPayeeAccountId);
                }
            }
            else
            {
                CreatePayeeAccountDetails(lintBenefitAccountID, adtPaymentDate, adecDRONontaxableBeginningBalanceAmount, lintPayeeAccountId);
            }
        }

        //Check whether the payee account is already created for the application or not
        public bool IsPayeeAccountCreated()
        {
            if (icdoBenefitCalculation.benefit_calculation_id > 0)
            {
                if (iclbPayeeAccount == null)
                    LoadPayeeAccount();
                if (iclbPayeeAccount.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CanRMDAmountBeReduced()
        {
            if ((icdoBenefitRefundCalculation.ee_er_pickup_amount >= icdoBenefitCalculation.rmd_amount)
            || (icdoBenefitRefundCalculation.ee_interest_amount >= icdoBenefitCalculation.rmd_amount)
            || (icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount >= icdoBenefitCalculation.rmd_amount)
            || (icdoBenefitRefundCalculation.rhic_ee_amount >= icdoBenefitCalculation.rmd_amount)
            || (icdoBenefitRefundCalculation.vested_er_contribution_amount >= icdoBenefitCalculation.rmd_amount)
            || (icdoBenefitRefundCalculation.post_tax_ee_contribution_amount >= icdoBenefitCalculation.rmd_amount))
            {
                return true;
            }
            return false;
        }
        //Creating payee account payment items
        private void CreatePayeeAccountDetails(int aintBenefitAccountID, DateTime adtPaymentDate, decimal adecDRONontaxableBeginningBalanceAmount, int aintPayeeAccountID)
        {
            int lintPayeeAccountID = CreatePayeeAccountAndStatus(aintBenefitAccountID, adtPaymentDate, adecDRONontaxableBeginningBalanceAmount, aintPayeeAccountID);
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            lobjPayeeAccount.FindPayeeAccount(lintPayeeAccountID);
            decimal ldecERInterestAmount = icdoBenefitRefundCalculation.overridden_er_interest_amount > 0.0M ?
                icdoBenefitRefundCalculation.overridden_er_interest_amount : icdoBenefitRefundCalculation.er_interest_amount;
            decimal ldecERPreTaxAmount = icdoBenefitRefundCalculation.overridden_er_pre_tax_amount > 0.0M ?
                icdoBenefitRefundCalculation.overridden_er_pre_tax_amount : icdoBenefitRefundCalculation.er_pre_tax_amount;
            if (icdoBenefitCalculation.rmd_amount > 0.0M && CanRMDAmountBeReduced())
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITRMDAmount, icdoBenefitCalculation.rmd_amount,
                                "", 0, adtPaymentDate, DateTime.MinValue, false);
                if (icdoBenefitRefundCalculation.ee_interest_amount >= icdoBenefitCalculation.rmd_amount)
                {
                    icdoBenefitRefundCalculation.ee_interest_amount = icdoBenefitRefundCalculation.ee_interest_amount - icdoBenefitCalculation.rmd_amount;
                }
                else if (icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount >= icdoBenefitCalculation.rmd_amount)
                {
                    icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount = icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount - icdoBenefitCalculation.rmd_amount;
                }
                else if (icdoBenefitRefundCalculation.ee_er_pickup_amount >= icdoBenefitCalculation.rmd_amount)
                {
                    icdoBenefitRefundCalculation.ee_er_pickup_amount = icdoBenefitRefundCalculation.ee_er_pickup_amount - icdoBenefitCalculation.rmd_amount;
                }
                else if (icdoBenefitRefundCalculation.vested_er_contribution_amount >= icdoBenefitCalculation.rmd_amount)
                {
                    icdoBenefitRefundCalculation.vested_er_contribution_amount = icdoBenefitRefundCalculation.vested_er_contribution_amount - icdoBenefitCalculation.rmd_amount;
                }
                else if (icdoBenefitRefundCalculation.post_tax_ee_contribution_amount >= icdoBenefitCalculation.rmd_amount)
                {
                    icdoBenefitRefundCalculation.post_tax_ee_contribution_amount = icdoBenefitRefundCalculation.post_tax_ee_contribution_amount - icdoBenefitCalculation.rmd_amount;
                }
                else if (icdoBenefitRefundCalculation.rhic_ee_amount >= icdoBenefitCalculation.rmd_amount)
                {
                    icdoBenefitRefundCalculation.rhic_ee_amount = icdoBenefitRefundCalculation.rhic_ee_amount - icdoBenefitCalculation.rmd_amount;
                }
                else if (icdoBenefitRefundCalculation.capital_gain >= icdoBenefitCalculation.rmd_amount)
                {
                    icdoBenefitRefundCalculation.capital_gain = icdoBenefitRefundCalculation.capital_gain - icdoBenefitCalculation.rmd_amount;
                }
            }
            if (icdoBenefitRefundCalculation.ee_er_pickup_amount > 0.0M)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemEEERPickupAmount, icdoBenefitRefundCalculation.ee_er_pickup_amount,
                                                "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
            if (icdoBenefitRefundCalculation.ee_interest_amount > 0.0M)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemEEInterestAmount, icdoBenefitRefundCalculation.ee_interest_amount,
                                                "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
            if (ldecERInterestAmount > 0.0M && icdoBenefitCalculation.benefit_option_value != busConstant.BenefitOptionDBToDCTransferSpecialElection)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemERInterestAmount, ldecERInterestAmount,
                                "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
            if (ldecERPreTaxAmount > 0.0M && icdoBenefitCalculation.benefit_option_value != busConstant.BenefitOptionDBToDCTransferSpecialElection)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemERPreTaxAmount, ldecERPreTaxAmount,
                                "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
            if (icdoBenefitRefundCalculation.post_tax_ee_contribution_amount > 0.0M)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPostTaxEEContributionAmount, icdoBenefitRefundCalculation.post_tax_ee_contribution_amount,
                                "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
            if (icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount > 0.0M)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPreTaxEEContributionAmount, icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount,
                                "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
            if (ibusRefundBenefitApplication.IsBenefitOptionRegularRefundOrAutoRefund() || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection)
            {
                CreatePayeeAccountItemForRHICAmount(lintPayeeAccountID, adtPaymentDate);
            }
            if (icdoBenefitRefundCalculation.vested_er_contribution_amount > 0.0M)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemVestedERContributionAmount, icdoBenefitRefundCalculation.vested_er_contribution_amount,
                                "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
            if (icdoBenefitRefundCalculation.additional_ee_amount > 0.0M && icdoBenefitCalculation.benefit_option_value != busConstant.BenefitOptionDBToDCTransferSpecialElection)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemAdditionalEEAmount, icdoBenefitRefundCalculation.additional_ee_amount,
                                "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
            if (icdoBenefitRefundCalculation.additional_er_amount > 0.0M && icdoBenefitCalculation.benefit_option_value != busConstant.BenefitOptionDBToDCTransferSpecialElection)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemAdditionalERAmount, icdoBenefitRefundCalculation.additional_er_amount,
                                "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
            if (icdoBenefitRefundCalculation.capital_gain > 0.0M)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PaymentItemCapitalGain, icdoBenefitRefundCalculation.capital_gain,
                                "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
            //PIR 25920 DC 2025 changes part 2 - If the ‘Total Transfer Amount’ is > DB Account Balance, add the remaining balance as a separate ‘ER Pre-Tax Amount’ 
            if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection)
            {
                //decimal ldecDiffERPreTaxAmount = icdoBenefitRefundCalculation.er_pre_tax_amount;
                if (icdoBenefitRefundCalculation.calculated_actuarial_value > icdoBenefitRefundCalculation.db_account_balance &&
                    icdoBenefitRefundCalculation.calculated_actuarial_value - icdoBenefitRefundCalculation.db_account_balance > 0)
                {
                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemERPreTaxAmount, icdoBenefitRefundCalculation.calculated_actuarial_value - icdoBenefitRefundCalculation.db_account_balance,
                                    "", 0, adtPaymentDate, DateTime.MinValue, false);
                    //if ((icdoBenefitRefundCalculation.calculated_actuarial_value - icdoBenefitRefundCalculation.db_account_balance) > icdoBenefitRefundCalculation.er_pre_tax_amount)
                    //    ldecDiffERPreTaxAmount = -1 * ((icdoBenefitRefundCalculation.calculated_actuarial_value - icdoBenefitRefundCalculation.db_account_balance) - icdoBenefitRefundCalculation.er_pre_tax_amount);
                    //else if ((icdoBenefitRefundCalculation.calculated_actuarial_value - icdoBenefitRefundCalculation.db_account_balance) < icdoBenefitRefundCalculation.er_pre_tax_amount)
                    //    ldecDiffERPreTaxAmount = (icdoBenefitRefundCalculation.er_pre_tax_amount - (icdoBenefitRefundCalculation.calculated_actuarial_value - icdoBenefitRefundCalculation.db_account_balance));
                    decimal ldecDiffERPreTaxAmount = icdoBenefitRefundCalculation.calculated_actuarial_value - icdoBenefitRefundCalculation.db_account_balance - icdoBenefitRefundCalculation.er_pre_tax_amount;
                    if (ldecDiffERPreTaxAmount > 0)
                    {
                        if (this.ibusBenefitApplication.IsNull() || this.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id == 0)
                            this.LoadBenefitApplication();
                        CreateRetirementContribution(ldecDiffERPreTaxAmount, this.ibusBenefitApplication.icdoBenefitApplication.payment_date, lintPayeeAccountID);
                    }
                }
            }
        }
        //PIR 25920 DC 2025 changes part 2 - If the ‘Total Transfer Amount’ is > DB Account Balance, add the remaining balance as a separate ‘ER Pre-Tax Amount’ 
        public void CreateRetirementContribution(decimal adecPreTaxERAmount, DateTime adtPaymentDate, int aintPayeeAccountID)
        {
            busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            int lintPersonAccountId = ibusPersonAccount.icdoPersonAccount.person_account_id;
            if(ibusPersonAccount.icdoPersonAccount.person_account_id ==0)
            {
                if (ibusBenefitApplication.iclbBenefitApplicationPersonAccounts == null)
                    ibusBenefitApplication.LoadBenefitApplicationPersonAccount();
                if(ibusBenefitApplication.iclbBenefitApplicationPersonAccounts.Count > 0)
                {
                    lintPersonAccountId = ibusBenefitApplication.iclbBenefitApplicationPersonAccounts.FirstOrDefault().icdoBenefitApplicationPersonAccount.person_account_id;
                }
            }
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = lintPersonAccountId;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_id = 349;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value = busConstant.SubSystemValueBenefitPayment;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id = aintPayeeAccountID;

            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = adtPaymentDate;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = adtPaymentDate;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month = adtPaymentDate.Month;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year = adtPaymentDate.Year;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_id = 350;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.Refund;

            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount = adecPreTaxERAmount;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transfer_flag_id = 7006;
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.transfer_flag_value = "C";
            lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
        }
        //Create separate payee account for RHIC EE amount for TFFR and TIAA CREF Transfer options
        private void CreatePayeeAccountItemForRHICAmount(int lintPayeeAccountID, DateTime adtPaymentDate)
        {
            if (icdoBenefitRefundCalculation.rhic_ee_amount > 0)
            {
                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                lobjPayeeAccount.FindPayeeAccount(lintPayeeAccountID);
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemRHICEEAmount, icdoBenefitRefundCalculation.rhic_ee_amount,
                                                            "", 0, adtPaymentDate, DateTime.MinValue, false);
            }
        }

        private int CreatePayeeAccountAndStatus(int aintBenefitAccountID, DateTime adtPaymentDate, decimal adecDRONontaxableBeginningBalanceAmount, int aintPayeeAccountID)
        {
            if (ibusRefundBenefitApplication == null)
                LoadRefundBenefitApplication();
            DateTime ltdBenefitBeginDate = DateTime.MinValue;
            //as per pir 1691,1753
            if (ibusRefundBenefitApplication.icdoBenefitApplication.hardship_approved_flag == busConstant.Flag_Yes ||
                ibusRefundBenefitApplication.icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRegularRefund)
                ltdBenefitBeginDate = ibusRefundBenefitApplication.icdoBenefitApplication.payment_date;
            else
                ltdBenefitBeginDate = adtPaymentDate;

            int lintPayeeAccountID = busPayeeAccountHelper.ManagePayeeAccount(icdoBenefitCalculation.person_id, 0, icdoBenefitCalculation.benefit_application_id, icdoBenefitCalculation.benefit_calculation_id,
                                    aintBenefitAccountID, 0, busConstant.StatusValid, icdoBenefitCalculation.benefit_account_type_value, icdoBenefitCalculation.benefit_account_sub_type_value, busConstant.Flag_No, ltdBenefitBeginDate,
                                    DateTime.MinValue, busConstant.AccountRelationshipMember, busConstant.AccountRelationshipMember, 0.0M, adecDRONontaxableBeginningBalanceAmount,
                                    icdoBenefitCalculation.benefit_option_value, 0.0M, aintPayeeAccountID,
                                    busConstant.PayeeAccountExclusionMethodSimplified, 0.0m, DateTime.MinValue, istrRHICRefundFlag, 0, icdoBenefitCalculation.graduated_benefit_option_value);
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            lobjPayeeAccount.FindPayeeAccount(lintPayeeAccountID);
            lobjPayeeAccount.CreateReviewPayeeAccountStatus();
            return lintPayeeAccountID;
        }

        private void ValidateApprove(ArrayList arrErrorList)
        {
            utlError lobjError = new utlError();
            if (ibusMember == null)
                LoadMember();

            if (iclbBenefitCalculationPersonAccount == null)
                LoadBenefitCalculationPersonAccount();
            if (ibusRefundBenefitApplication == null)
                LoadRefundBenefitApplication();
            //Check whether Recalculated amount and the amount in the table are same or not.if it is not same ,throw an error
            if (IsCalculationAmountMemberAccountNotEqual() && !ibusRefundBenefitApplication.IsBenefitDROApplicationExist())
            {
                if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal)
                {
                    lobjError = AddError(7509, "");
                    arrErrorList.Add(lobjError);
                }
            }
            //If the member is died ,do not allow to approve
            if (ibusMember.icdoPerson.date_of_death != DateTime.MinValue)
            {
                lobjError = AddError(1913, "");
                arrErrorList.Add(lobjError);
            }
            //PIR 19010 & 18993 //PIR 16284, 17801 - Check the validations in App and Calc to see if we stop users from Verifying (App) or Approving (Calc) if a new employment with Contribution exists
            if (ibusRefundBenefitApplication.DoesPersonHaveOpenContriEmpDtlByPlan())
            {
                lobjError = AddError(7505, "");
                arrErrorList.Add(lobjError);
            }
            if (ibusMember.iclbBenefitApplication == null)
            {
                ibusMember.LoadBenefitApplication();
            }

            if ((ibusBenefitApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToDCTransfer) ||
                (ibusBenefitApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection) ||
                (ibusBenefitApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer) ||
                (ibusBenefitApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE) ||
                (ibusBenefitApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDualMembers))
            {
                ValidateApproveForTransferOptions(arrErrorList, lobjError);
            }
        }

        private void ValidateApproveForTransferOptions(ArrayList arrErrorList, utlError aobjError)
        {
            //BR - 78 If DRO Appplication already exists for member,then throw an error
            if (ibusRefundBenefitApplication == null)
                LoadRefundBenefitApplication();
            if (ibusRefundBenefitApplication.ValidateBenefitDROApplicationExist() == busConstant.DROApplicationStatusApproved)
            {
                aobjError = AddError(7519, "");
                arrErrorList.Add(aobjError);
            }
            //if Benefit option is DB to DC transfer ,Check person enrolled in DC Plan ,else throw an error
            if (ibusBenefitApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToDCTransfer ||
                ibusBenefitApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection)
            {
                if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB)
                {
                    if (ibusMember == null)
                        LoadMember();
                    busPersonAccount lobjPersonAccount = new busPersonAccount();
                    lobjPersonAccount = ibusMember.LoadActivePersonAccountByPlan(busConstant.PlanIdDC);
                    if (lobjPersonAccount.icdoPersonAccount.person_account_id == 0)
                    {
                        lobjPersonAccount = ibusMember.LoadActivePersonAccountByPlan(busConstant.PlanIdDC2020); //PIR 20232
                    }
                    if (lobjPersonAccount.icdoPersonAccount.person_account_id == 0)
                    {
                        lobjPersonAccount = ibusMember.LoadActivePersonAccountByPlan(busConstant.PlanIdDC2025); //PIR 25920
                    }
                    if (lobjPersonAccount.icdoPersonAccount.person_account_id == 0)
                    {
                        aobjError = AddError(7510, "");
                        arrErrorList.Add(aobjError);
                    }
                    else if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementEnrolled)
                    {
                        aobjError = AddError(7510, "");
                        arrErrorList.Add(aobjError);
                    }
                }
                //BR -78a check whether there is any record with   ‘In Payment’ or ‘Approved’ status. If exists then generate an error message 
                if (ibusMember.iclbServicePurchaseHeader == null)
                    ibusMember.LoadServicePurchase();
                foreach (busServicePurchaseHeader lobjServicePurhaseHeader in ibusMember.iclbServicePurchaseHeader)
                {
                    if ((lobjServicePurhaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment)
                        ||
                        (lobjServicePurhaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Approved))
                    {
                        aobjError = AddError(7510, "");
                        arrErrorList.Add(aobjError);
                    }
                }
            }
        }
        //BR - 47 Compare the calculated amount with sum of life to date countribution amount.if not matching trow an error
        public bool IsCalculationAmountMemberAccountNotEqual()
        {
            if (ibusBenefitApplication.iclbBenefitApplicationPersonAccounts == null)
                ibusBenefitApplication.LoadBenefitApplicationPersonAccount();
            if (!IsBenefitOptionTFFR())
            {
                if (ibusPlan == null)
                    LoadPlan();

                decimal ldecPreTaxEEamount = 0.0M, ldecPostTaxEEAmount = 0.0M, ldecEEERPickupAmount = 0.0M, ldecInterest = 0.0M, ldecERVestedAmount = 0.0M, ldecEERHICAmount = 0.0M;
                //uat pir 1419 : benefit application person account with application person account flag = Y
                IEnumerable<busBenefitApplicationPersonAccount> lenmBAPA = ibusBenefitApplication.iclbBenefitApplicationPersonAccounts
                                                                            .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes);

                foreach (busBenefitApplicationPersonAccount lobjBenefitCalculationPersonAccount in lenmBAPA)
                {
                    lobjBenefitCalculationPersonAccount.LoadPersonAccountRetirement();
                    lobjBenefitCalculationPersonAccount.LoadPersonAccount();
                    lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount = lobjBenefitCalculationPersonAccount.ibusPersonAccount.icdoPersonAccount;
                    lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.LoadLTDSummary();

                    ldecPreTaxEEamount += lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.pre_tax_ee_amount_ltd + lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.pre_tax_ee_ser_pur_cont_ltd;
                    ldecPostTaxEEAmount += lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.post_tax_ee_amount_ltd + lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.post_tax_ee_ser_pur_cont_ltd;
                    ldecEEERPickupAmount += lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.ee_er_pickup_amount_ltd + lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.ee_er_pickup_ser_pur_cont_ltd;
                    ldecInterest = lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.interest_amount_ltd;
                    ldecERVestedAmount += lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.er_vested_amount_ltd;
                    ldecEERHICAmount += lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.ee_rhic_amount_ltd + lobjBenefitCalculationPersonAccount.ibusPersonAccountRetirement.ee_rhic_ser_pur_cont_ltd;
                }
                if (ibusPlan.IsDBRetirementPlan())
                {
                    if ((ldecPreTaxEEamount != icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount) ||
                       (ldecPostTaxEEAmount != icdoBenefitRefundCalculation.post_tax_ee_contribution_amount) ||
                       (ldecERVestedAmount != icdoBenefitRefundCalculation.vested_er_contribution_amount) ||
                       (ldecEEERPickupAmount != icdoBenefitRefundCalculation.ee_er_pickup_amount) ||
                       (ldecInterest != icdoBenefitRefundCalculation.ee_interest_amount))
                    {
                        return true;
                    }
                }
                else
                {
                    if (ldecEERHICAmount != icdoBenefitRefundCalculation.rhic_ee_amount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override void BeforePersistChanges()
        {
            //As Per PIR from Satya no need to populate er pretax amount and interest for regular refund and auto refund
            if (!IsBenefitOptionRegularRefundOrAutoRefund())
            {
                CalculateInterestForERPreTaxAmount();
            }


            icdoBenefitRefundCalculation.benefit_option_value_to_compare = icdoBenefitCalculation.benefit_option_value;
            icdoBenefitRefundCalculation.person_id = icdoBenefitCalculation.person_id;


            if (icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeAdjustments)
            {
                //Recalculate Every times
                CalculateRefund();
            }

            //Calculate Total Refund Amount
            if (IsBenefitOptionRegularRefundOrAutoRefund())
            {
                CalculateTotalRefundAmountForRegulaOrAutoRefund();
            }
            else
            {
                CalculateTotalAmountForTransferOptions();
            }
            // BR-057-90	DB to TIAA-CREF Transfer	The system must calculate ER interest on the additional ER Pre-Tax Amount posted as part of ‘Remainder Transfer’.
            if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments) && (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer))
            {
                CalculateAddiionalInterestForERPreTaxAmount();
            }

            //DO NOT CALL base.BeforePersistChanges() till framework team release the version with empty beforepersistchanges()
        }

        public override int PersistChanges()
        {
            int lintResult = 0;
            if (icdoBenefitCalculation.benefit_calculation_id == 0)
            {
                icdoBenefitCalculation.Insert();
                icdoBenefitRefundCalculation.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                icdoBenefitRefundCalculation.Insert();
                CreateBenefitCalculationPersonAccount();
                CreateBenefitCalculationPayeeDetails();
            }
            else //PIR 17140 - When calculation is put into review, the posted additional contributions will not reflect on the calculation without this block
            {
                if (icdoBenefitRefundCalculation.ienuObjectState != ObjectState.Update) icdoBenefitRefundCalculation.ienuObjectState = ObjectState.Update;
                if (iarrChangeLog.OfType<cdoBenefitRefundCalculation>().Count() == 0) iarrChangeLog.Add(this.icdoBenefitRefundCalculation);
                if (icdoBenefitCalculation.ienuObjectState != ObjectState.Update) icdoBenefitCalculation.ienuObjectState = ObjectState.Update;
                if (iarrChangeLog.OfType<cdoBenefitCalculation>().Count() == 0) iarrChangeLog.Add(this.icdoBenefitCalculation);
            }

            //PIR 11946
            if (ibusBenefitApplication == null)
                LoadBenefitApplication();
            if (ibusBenefitApplication.iclbBenefitApplicationPersonAccounts == null)
                ibusBenefitApplication.LoadBenefitApplicationPersonAccount();

            int lintPersonAccountID = 0;
            busBenefitApplicationPersonAccount lbusBenefitApplicationPersonAccount = ibusBenefitApplication.iclbBenefitApplicationPersonAccounts.Where(i => i.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).FirstOrDefault();
            if (lbusBenefitApplicationPersonAccount.IsNotNull())
                lintPersonAccountID = lbusBenefitApplicationPersonAccount.icdoBenefitApplicationPersonAccount.person_account_id;

            if (lintPersonAccountID > 0)
                UpdateContributionTransferFlagToCalculation(lintPersonAccountID);

            return base.PersistChanges();
        }

        public int UpdateContributionTransferFlagToCalculation(int aintPersonAccountID)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoBenefitCalculation.UpdateContributionTransferFlagToC",
                                       new object[2] { aintPersonAccountID, iobjPassInfo.istrUserID },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (ibusBaseActivityInstance != null)
            {
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
                if (lbusActivityInstance.ibusBpmProcessInstance == null)
                {
                    lbusActivityInstance.LoadBpmProcessInstance();
                }
                int lintProcessId = busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name);
                if ((lintProcessId == busConstant.Map_Process_DB_To_DC_Transfer_Application)
                     || (lintProcessId == busConstant.Map_Process_Remainder_Transfer_Refund) ||
                     (lintProcessId == busConstant.Map_Initialize_Process_Refund_Application_And_Calculation)
                     || lintProcessId == busConstant.Map_MSS_Initialize_Process_Refund_Application_And_Calculation)
                {
                    lbusActivityInstance.UpdateParameter("calculation_reference_id", icdoBenefitRefundCalculation.benefit_calculation_id.ToString());
                }

                if ((lintProcessId == busConstant.Map_Process_DB_To_TFFR_Transfer_Application) ||
                    (lintProcessId == busConstant.Map_Process_DB_To_TIAA_CREF_Transfer) ||
                    (lintProcessId == busConstant.Map_Initialize_Process_Auto_Refund_Workflow))
                {
                    lbusActivityInstance.UpdateParameter("refund_calc_reference_id", icdoBenefitRefundCalculation.benefit_calculation_id.ToString());
                }
                //Pir:27434
            }

        }

        public void CreateBenefitCalculationPayeeDetails()
        {
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayeeForNewMode();
            foreach (busBenefitCalculationPayee lobjBenefitCalculationPayee in iclbBenefitCalculationPayee)
            {
                lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.Insert();
            }
        }
        public void CreateBenefitCalculationPersonAccount()
        {
            if (iclbBenefitCalculationPersonAccount == null)
                LoadBenefitCalculationPersonAccount();
            foreach (busBenefitCalculationPersonAccount lobjBenefitCalculationPersonAccount in iclbBenefitCalculationPersonAccount)
            {
                lobjBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                lobjBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.Insert();
            }
        }

        #region Properties for Correspondence
        //Property to contain Payment Item Type
        private Collection<busPaymentItemType> _iclbPaymentItemType;
        public Collection<busPaymentItemType> iclbPaymentItemType
        {
            get { return _iclbPaymentItemType; }
            set { _iclbPaymentItemType = value; }
        }



        //Total Taxable amt        
        private decimal _idecTaxableAmount;
        public decimal idecTaxableAmount
        {
            get
            {
                return _idecTaxableAmount;
            }
            set
            {
                _idecTaxableAmount = value;
            }
        }

        //Total Non Taxable amt
        private decimal _idecNonTaxableAmount;
        public decimal idecNonTaxableAmount
        {

            get
            {
                return _idecNonTaxableAmount;
            }
            set
            {
                _idecNonTaxableAmount = value;
            }
        }

        //Total Acct balance
        public decimal idecAcctBalance
        {
            get
            {
                return idecNonTaxableAmount + idecTaxableAmount;
            }
        }

        #endregion

        #region Methods for Correpondence
        public override busBase GetCorPerson()
        {
            return ibusMember;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            LoadPayementItemType();
            LoadTaxableNonTaxableAmount();
            if (payment_date == DateTime.MinValue)
                LoadPaymentDate();
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.idteLastContributedDate == DateTime.MinValue)
                ibusPersonAccount.GetLastcontibutionDate();

            LoadCorrespondenceData();
        }

        public string istrDateofDistributionFormatted
        {
            get
            {
                if (payment_date == DateTime.MinValue)
                    return string.Empty;
                else
                    return payment_date.ToString(busConstant.DateFormatMonthYear); //PROD PIR ID 6783
            }
        }
		//PIR-10361
        public string istrDateofLastPayCheckFormatted
        {
            get
            {
                if (ibusMember.ibusLastEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
                    return string.Empty;
                else
                    return ibusMember.ibusLastEmployment.icdoPersonEmployment.end_date.GetLastDayofMonth().ToString(busConstant.DateFormatLongDate);
            }
        }

        //Method to load all payment item type
        //NOTE : If a new item is added into Refund calc. it should be included in below query so as to get correct data for correspondence
        public void LoadPayementItemType()
        {
            if (iclbPaymentItemType == null)
                iclbPaymentItemType = new Collection<busPaymentItemType>();
            DataTable ldtList = Select("cdoPaymentItemType.LoadPaymentItemType", new object[0] { });
            iclbPaymentItemType = GetCollection<busPaymentItemType>(ldtList, "icdoPaymentItemType");
        }

        //Method to find the taxable and nontaxable amount by looping through payment item type
        //NOTE : If a new item is added into Refund calc. it should be included in below code so as to get correct data for correspondence
        public void LoadTaxableNonTaxableAmount()
        {
            decimal ldecTaxable = 0.0M, ldecNonTaxable = 0.0M;
            foreach (busPaymentItemType lobjPaymentItemType in iclbPaymentItemType)
            {
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemAdditionalEEAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.additional_ee_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.additional_ee_amount;
                }
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemAdditionalERAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.additional_er_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.additional_er_amount;
                }
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemEEERPickupAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.ee_er_pickup_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.ee_er_pickup_amount;
                }
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemEEInterestAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.ee_interest_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.ee_interest_amount;
                }
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemERInterestAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.er_interest_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.er_interest_amount;
                }
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemERPreTaxAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.er_pre_tax_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.er_pre_tax_amount;
                }
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemPostTaxEEContributionAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.post_tax_ee_contribution_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.post_tax_ee_contribution_amount;
                }
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemPreTaxEEContributionAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount;
                }
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemRHICEEAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.rhic_ee_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.rhic_ee_amount;
                }
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemVestedERContributionAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.vested_er_contribution_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.vested_er_contribution_amount;
                }
                //Commented as part of pir 8129
                /*if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRMDAmount)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitCalculation.rmd_amount;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitCalculation.rmd_amount;
                }*/
                if (lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PaymentItemCapitalGain)
                {
                    if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "Y")
                        ldecTaxable += icdoBenefitRefundCalculation.capital_gain;
                    else if (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag.ToUpper() == "N")
                        ldecNonTaxable += icdoBenefitRefundCalculation.capital_gain;
                }
            }
            idecTaxableAmount = ldecTaxable;
            idecNonTaxableAmount = ldecNonTaxable;
        }

        #endregion

        public void LoadBenefitCalculationPayeeForNewMode()
        {
            if (ibusMember == null)
                LoadMember();
            iclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();
            busBenefitCalculationPayee lobjBenefitCalculationPayee = new busBenefitCalculationPayee();
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_person_id = icdoBenefitCalculation.person_id;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_date_of_birth = ibusMember.icdoPerson.date_of_birth;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_first_name = ibusMember.icdoPerson.first_name;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_last_name = ibusMember.icdoPerson.last_name;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_middle_name = ibusMember.icdoPerson.middle_name;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipMember;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, busConstant.AccountRelationshipMember);
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_value = busConstant.FamilyRelationshipUnknown;
            iclbBenefitCalculationPayee.Add(lobjBenefitCalculationPayee);
            // Spouse Record is added only in case of Final
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal)
            {
                // Joint Annuitant will not be null in case Final, Since it is loaded with Joint Annuitant PERSLink ID
                if (ibusJointAnnuitant == null)
                    LoadJointAnnuitant();
                // First name is checked since the Joint Annuitant may not have PERSLink ID
                if (!string.IsNullOrEmpty(ibusJointAnnuitant.icdoPerson.first_name))
                {
                    busBenefitCalculationPayee lobjJointAnnuitant = new busBenefitCalculationPayee();
                    lobjJointAnnuitant.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.payee_person_id = ibusJointAnnuitant.icdoPerson.person_id;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.payee_date_of_birth = ibusJointAnnuitant.icdoPerson.date_of_birth;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.payee_first_name = ibusJointAnnuitant.icdoPerson.first_name; ;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.payee_last_name = ibusJointAnnuitant.icdoPerson.last_name;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.payee_middle_name = ibusJointAnnuitant.icdoPerson.middle_name;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipJointAnnuitant;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.account_relationship_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, busConstant.AccountRelationshipJointAnnuitant);
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.family_relationship_value = busConstant.FamilyRelationshipSpouse;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.family_relationship_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(321, busConstant.FamilyRelationshipSpouse);
                    iclbBenefitCalculationPayee.Add(lobjJointAnnuitant);
                }
            }
        }

        // UCS-093 Calculate Required Minimum Distribution Amount
        public void CalculateRequiredMinimumDistributionAmount()
        {
            if ((icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionAutoRefund) ||
                (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionRegularRefund))
            {
                DateTime ltdBenefitBeginDate = DateTime.MinValue;
                decimal ldecMemberAge = 0M;
                if (ibusMember == null)
                    LoadMember();
                if (ibusRefundBenefitApplication == null)
                    LoadRefundBenefitApplication();

                if (ibusRefundBenefitApplication.icdoBenefitApplication.hardship_approved_flag == busConstant.Flag_Yes)
                    ltdBenefitBeginDate = ibusRefundBenefitApplication.icdoBenefitApplication.payment_date;
                else
                {
                    if (payment_date == DateTime.MinValue)
                        LoadPaymentDate();
                    ltdBenefitBeginDate = payment_date;
                }

                if (icdoBenefitCalculation.member_account_balance == 0M)
                {
                    icdoBenefitCalculation.member_account_balance = icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount +
                                                                    icdoBenefitRefundCalculation.post_tax_ee_contribution_amount +
                                                                    icdoBenefitRefundCalculation.ee_er_pickup_amount +
                                                                    icdoBenefitRefundCalculation.ee_interest_amount +
                                                                    icdoBenefitRefundCalculation.vested_er_contribution_amount +
                                                                    icdoBenefitRefundCalculation.rhic_ee_amount;
                }
                CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, ltdBenefitBeginDate, ref ldecMemberAge, 4);
                idecMemberAgeBasedOnRetirementDate = ldecMemberAge; // Visible rule is based on idecMemberAgeBasedOnRetirementDate
                icdoBenefitCalculation.rmd_amount = CalculateRMDAmount(ldecMemberAge,
                                                    busConstant.ApplicationBenefitTypeRefund, icdoBenefitCalculation.member_account_balance, ltdBenefitBeginDate);
            }
        }

        public bool IsRefundCalculationProccessedforAdjustment()
        {
            if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                && (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                && (icdoBenefitCalculation.status_value == busConstant.CalculationStatusProcessed))
            {
                return true;
            }
            return false;
        }

        public void SetAdjustmentRefundForAdditionalContributions(busEmployerPayrollDetail aobjbusEmployerPayrollDetail)
        {
            icdoBenefitRefundCalculation.ee_er_pickup_amount += aobjbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported;
            icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount += aobjbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported;
            icdoBenefitRefundCalculation.post_tax_ee_contribution_amount += aobjbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported;
            if (!IsBenefitOptionRegularRefundOrAutoRefund())
            {
                icdoBenefitRefundCalculation.er_pre_tax_amount += aobjbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported;
            }
            icdoBenefitRefundCalculation.ee_interest_amount += aobjbusEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated;
            icdoBenefitRefundCalculation.rhic_ee_amount += aobjbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported;
        }

        public void UpdateAdjustmentForRefundCalculation(busEmployerPayrollDetail aobjbusEmployerPayrollDetail, bool ablnEmployerReportBatchIndicator,
            decimal adecAdditionalInterestAmount = 0, decimal adecERPreTaxInterestAmount = 0, decimal adecERVestedAmount = 0)
        {
            icdoBenefitRefundCalculation.benefit_option_value_to_compare = icdoBenefitCalculation.benefit_option_value;
            icdoBenefitRefundCalculation.person_id = icdoBenefitCalculation.person_id;

            if (ablnEmployerReportBatchIndicator)
            {
                SetAdjustmentRefundForAdditionalContributions(aobjbusEmployerPayrollDetail);
                //prod pir 6153
                icdoBenefitRefundCalculation.ee_interest_amount += adecAdditionalInterestAmount;
            }
            else
            {
                icdoBenefitRefundCalculation.ee_interest_amount += adecAdditionalInterestAmount;
                icdoBenefitRefundCalculation.er_interest_amount += adecERPreTaxInterestAmount;
                icdoBenefitRefundCalculation.vested_er_contribution_amount += adecERVestedAmount;
            }
            icdoBenefitRefundCalculation.Update();
        }

        /// <summary>
        /// method to load correspondence data for PAY-4055
        /// </summary>
        public void LoadCorrespondenceData()
        {
            icdoBenefitRefundCalculation.idecTaxableMC = icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount + icdoBenefitRefundCalculation.ee_er_pickup_amount +
                                                        icdoBenefitRefundCalculation.additional_ee_amount;
            icdoBenefitRefundCalculation.idecNonTaxableMC = icdoBenefitRefundCalculation.post_tax_ee_contribution_amount;
            icdoBenefitRefundCalculation.idecTaxableMI = icdoBenefitRefundCalculation.ee_interest_amount;
            icdoBenefitRefundCalculation.idecTaxableCG = icdoBenefitRefundCalculation.capital_gain;
            icdoBenefitRefundCalculation.idecTaxableEC = icdoBenefitRefundCalculation.vested_er_contribution_amount +
                (icdoBenefitRefundCalculation.overridden_er_pre_tax_amount > 0 ? icdoBenefitRefundCalculation.overridden_er_pre_tax_amount : icdoBenefitRefundCalculation.er_pre_tax_amount);
            icdoBenefitRefundCalculation.idecNonTaxableEC = icdoBenefitRefundCalculation.additional_er_amount;
            icdoBenefitRefundCalculation.idecTaxableEI = icdoBenefitRefundCalculation.additional_er_amount_interest +
                (icdoBenefitRefundCalculation.overridden_er_interest_amount > 0 ? icdoBenefitRefundCalculation.overridden_er_interest_amount : icdoBenefitRefundCalculation.er_interest_amount);

            if (ibusBenefitApplication == null)
                LoadBenefitApplication();
            if (ibusBenefitApplication.iclbPayeeAccount == null)
                ibusBenefitApplication.LoadPayeeAccount();
            if (ibusMember == null)
                LoadMember();
            if (ibusMember.ibusCurrentEmployment == null)
                ibusMember.LoadCurrentEmployer(icdoBenefitCalculation.plan_id);
            busPayeeAccount lobjMemberPayeeAccount = ibusBenefitApplication.iclbPayeeAccount
                .Where(o => o.icdoPayeeAccount.payee_perslink_id == ibusMember.icdoPerson.person_id).FirstOrDefault();
            if (lobjMemberPayeeAccount != null)
            {
                if (lobjMemberPayeeAccount.ibusBenefitAccount == null)
                    lobjMemberPayeeAccount.LoadBenfitAccount();
                if (lobjMemberPayeeAccount.ibusBenefitAccount.ibusRetirementOrg == null)
                    lobjMemberPayeeAccount.ibusBenefitAccount.LoadRetirementOrg();
                icdoBenefitRefundCalculation.istrOrgName = lobjMemberPayeeAccount.ibusBenefitAccount.ibusRetirementOrg.icdoOrganization.org_name;
                //PIR 16304 - Commented as icdoBenefitRefundCalculation.idecCheckAmount is taken from the Total Transfer Amount from the Calculation screen
                //if (lobjMemberPayeeAccount.iclbPaymentHistoryHeader == null)
                //    lobjMemberPayeeAccount.LoadPaymentHistoryHeader();

                //busPaymentHistoryHeader lobjPHH = lobjMemberPayeeAccount.iclbPaymentHistoryHeader.FirstOrDefault();
                //if (lobjPHH != null)
                //{
                //    if (lobjPHH.iclbPaymentHistoryDistribution == null)
                //        lobjPHH.LoadPaymentHistoryDistribution();
                //    busPaymentHistoryDistribution lobjPHD = lobjPHH.iclbPaymentHistoryDistribution
                //                                                        .Where(o => o.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
                //                                                        .FirstOrDefault();
                //    if (lobjPHD != null)
                //    {
                //        icdoBenefitRefundCalculation.idecCheckAmount = lobjPHD.icdoPaymentHistoryDistribution.net_amount;
                //    }
                //}
            }
            CalculateTotalAmountForTransferOptions(); //PIR 16304
            icdoBenefitRefundCalculation.idecCheckAmount = icdoBenefitRefundCalculation.total_transfer_amount;
        }

        //PIR 23806
        public void CreateOverpaymentForRefundsOnApprove()
        {
            busPaymentBenefitOverpaymentHeader lobjPaymentBenefitOverpaymentHeader = new busPaymentBenefitOverpaymentHeader { icdoPaymentBenefitOverpaymentHeader = new cdoPaymentBenefitOverpaymentHeader() };

            lobjPaymentBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.payee_account_id = iintPayeeAccountID;
            lobjPaymentBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.adjustment_reason_value = busConstant.BenRecalAdjustmentReasonRecalculation;
            lobjPaymentBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.Insert();
            lobjPaymentBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.Select();

            lobjPaymentBenefitOverpaymentHeader.CreateBenefitOverpaymentDetail(icdoBenefitRefundCalculation.total_refund_amount * -1, 32, DateTime.Today, 0);
        }

        //PIR 25920 DC 2025 Changes Part 2 App calculation DB DC Transfer
        public void CalculateCalculatedActuarialValue(busPersonAccountRetirement abusPersonAccountRetirement)
        {

            icdoBenefitRefundCalculation.accrued_benefit_amount = CalculateAccruedBenefitAmt(abusPersonAccountRetirement);

            decimal ldecActuarialFactor = CalculateActuarialFactor();

            icdoBenefitRefundCalculation.calculated_actuarial_value = icdoBenefitRefundCalculation.accrued_benefit_amount * 12 * ldecActuarialFactor;
        }
        public void CalculateDBAccountBalance(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            //abusPersonAccountRetirement.LoadLTDSummary();
            //icdoBenefitRefundCalculation.db_account_balance = abusPersonAccountRetirement.Member_Account_Balance_ltd;
            icdoBenefitRefundCalculation.db_account_balance =
                icdoBenefitRefundCalculation.pre_tax_ee_contribution_amount + icdoBenefitRefundCalculation.post_tax_ee_contribution_amount +
                icdoBenefitRefundCalculation.ee_er_pickup_amount + icdoBenefitRefundCalculation.ee_interest_amount + 
                icdoBenefitRefundCalculation.vested_er_contribution_amount + icdoBenefitRefundCalculation.rhic_ee_amount +
                icdoBenefitRefundCalculation.capital_gain;
        }
        public decimal CalculateAccruedBenefitAmt(busPersonAccountRetirement abusPersonAccountRetirement)
        {
            DateTime ldtNRD = new DateTime();
            ldtNRD = GetNRD();
            abusPersonAccountRetirement.LoadPerson();
            abusPersonAccountRetirement.LoadPlan();
            abusPersonAccountRetirement.LoadPersonAccount();
            if(abusPersonAccountRetirement.ibusPersonAccount.IsNull())
                abusPersonAccountRetirement.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            abusPersonAccountRetirement.ibusPersonAccount.icdoPersonAccount = abusPersonAccountRetirement.icdoPersonAccount;

            abusPersonAccountRetirement.LoadRetirementContributionAll();
            abusPersonAccountRetirement.LoadNewRetirementBenefitCalculation();
            abusPersonAccountRetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeFinal);
            if (abusPersonAccountRetirement.ibusRetirementBenefitCalculation.idteLastContributedDate == DateTime.MinValue)
                abusPersonAccountRetirement.ibusRetirementBenefitCalculation.LoadLastContributedDate();
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date = new DateTime(2025, 01, 01).AddDays(-1);
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date > ldtNRD ?
                abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date.GetFirstDayofNextMonth() : ldtNRD;
            
            //abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.percentage_salary_increase = busConstant.AnnualSalaryIncreaseRate;
            //abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.salary_month_increase = busConstant.AnnualSalaryIncreaseMonth;

            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.CalculateConsolidatedPSC();
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.fas_termination_date = abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date;
            int lintMonthGap = busGlobalFunctions.DateDiffByMonth(new DateTime(2025, 01, 01), abusPersonAccountRetirement.ibusRetirementBenefitCalculation.idteLastContributedDate);
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.ibusPersonAccount.icdoPersonAccount.Total_PSC -= lintMonthGap;
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.credited_psc -= lintMonthGap;

            
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.idteLastContributedDate = abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date;
            abusPersonAccountRetirement.ibusRetirementBenefitCalculation.CalculateRetirementBenefit();
            return abusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.unreduced_benefit_amount;
        }
        public decimal CalculateActuarialFactor()
        {
            busHb1040DcTransferFactors lbusHb1040DcTransferFactors = new busHb1040DcTransferFactors();
            decimal ldecAgeAtNRD = 0.00m;
            decimal ldecAgeAt2025 = 0.00m;
            decimal ldecAgeAsOfCalcDate = 0.00m;
            DateTime ldtNRD = new DateTime();
            ldtNRD = GetNRD();
            
            CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, new DateTime(2025, 01, 01), ref ldecAgeAt2025, 4);
            CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, ldtNRD, ref ldecAgeAtNRD, 4);
            CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitRefundCalculation.calculation_date, ref ldecAgeAsOfCalcDate, 4);
            if (ldecAgeAtNRD < ldecAgeAt2025)
            {
                ldecAgeAtNRD = ldecAgeAsOfCalcDate + 0.083333M;
            }
            if (lbusHb1040DcTransferFactors.FindDcTransferFactors(ldecAgeAt2025.RoundToTwoDecimalPoints(), ldecAgeAtNRD.RoundToTwoDecimalPoints()))
            {
                return lbusHb1040DcTransferFactors.icdoHb1040DcTransferFactors.factor;
            }
            return 0.00m;
        }
        public DateTime GetNRD()
        {
            DateTime ldtNRD = new DateTime();
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson.IsNull())
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPlan.IsNull())
                ibusPersonAccount.LoadPlan();
            busRetirementBenefitCalculation lobjRetirementBenefitCalculation = new busRetirementBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            decimal ldecConsolidatedServiceCredit = 0.0M;
            lobjRetirementBenefitCalculation.icdoBenefitCalculation.person_id = ibusPersonAccount.icdoPersonAccount.person_id;
            lobjRetirementBenefitCalculation.icdoBenefitCalculation.plan_id = ibusPersonAccount.icdoPersonAccount.plan_id;
            lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
            //ldecConsolidatedServiceCredit = lobjRetirementBenefitCalculation.GetConsolidatedExtraServiceCredit();

            //if (ibusPersonAccount.icdoPersonAccount.Total_VSC == 0.00M)
            //    ibusPersonAccount.LoadTotalVSC();

            ldtNRD = busPersonBase.GetNormalRetirementDateBasedOnNormalEligibility(ibusPersonAccount.icdoPersonAccount.plan_id, ibusPersonAccount.ibusPlan.icdoPlan.plan_code,
                    ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id, busConstant.ApplicationBenefitTypeRetirement,
                    ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth, ibusPersonAccount.icdoPersonAccount.Total_VSC,
                    0, iobjPassInfo, DateTime.MinValue,
                    ibusPersonAccount.icdoPersonAccount.person_account_id, true,
                    ldecConsolidatedServiceCredit, icdoBenefitCalculation.retirement_date, null, ibusPersonAccount); //PIR 14646

            return ldtNRD;
        }


        
        
    }

}