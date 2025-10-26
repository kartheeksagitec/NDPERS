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
using Sagitec.CustomDataObjects;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using NeoSpin.DataObjects;
using NeoSpin.Common;
using System.Collections.Generic;
using Sagitec.Bpm;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPayeeAccountStatus : busPayeeAccountStatusGen
    {
        //BR -071 -38,BR -071 -39         
        //Fill the Payee Account Status DropDownList Based on the Benefit Type
        public Collection<cdoCodeValue> LoadPayeeAccountStatusByBenefitType()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            return ibusPayeeAccount.LoadPayeeAccountStatusByBenefitAccountType();
        }

        //  This validation will occur only when status Changed To Approved or Recieving .
        //  Get The Total ACH amount and Compare with Benefit Amount . 
        //  If it does not match ,throw an error - BR -071 -09      
        public bool ValidateWhenStatusApprovedOrRecievedForAch()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbActiveACHDetails == null)
                ibusPayeeAccount.LoadActiveACHDetail();
            if (ibusPayeeAccount.ibusPrimaryAchDetail == null)
                ibusPayeeAccount.LoadPrimaryACHDetail(ibusPayeeAccount.idtNextBenefitPaymentDate);
            if (ibusPayeeAccount.iclbActiveACHDetails.Count > 0)
            {
                if ((icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundApproved) ||
                    (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentApproved) ||
                    (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentDCRecieving) ||
                    (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentRecieving))
                {
                    ibusPayeeAccount.GetTotalACHAmountAndPercentage();
                    if (ibusPayeeAccount.idecBenefitAmount == 0.00M)
                    {
                        ibusPayeeAccount.LoadBenefitAmount();
                    }
                    foreach (busPayeeAccountAchDetail lobjPayeeAccountAchDetail in ibusPayeeAccount.iclbActiveACHDetails)
                    {

                        if ((lobjPayeeAccountAchDetail.IsPrimaryACHSelected) && (ibusPayeeAccount.iclbActiveACHDetails.Count == 1)
                            && (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.partial_amount > 0.0M) &&
                            (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.partial_amount < ibusPayeeAccount.idecBenefitAmount))
                        {
                            return true;
                        }
                        else if ((lobjPayeeAccountAchDetail.IsPrimaryACHSelected) && (ibusPayeeAccount.iclbActiveACHDetails.Count == 1)
                            && (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount > 0.0M) &&
                            (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount < 100.0M))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //  This validation will occur only when status Changed To Approved or Recieving .
        //  Get The Total Rollover amount and Compare with taxable Amount . 
        // If Rollover amount is greater than taxable amount ,throw an error
        public bool ValidateWhenStatusApprovedOrRecievedForRollover()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbActiveRolloverDetails == null)
                ibusPayeeAccount.LoadActiveRolloverDetail();
            //Rollover is only for PLSO and Refund , checking for taxable amount for flat tax calculations
            if (ibusPayeeAccount.iclbActiveRolloverDetails.Count > 0)
            {
                if ((icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundApproved) ||
                    (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentApproved) ||
                    (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentDCRecieving) ||
                    (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentRecieving))
                {
                    if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                        ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                    if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Count > 0)
                    {
                        decimal ldecRolloverAmount = ibusPayeeAccount.iclbPayeeAccountPaymentItemType.
                            Where(o => o.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolloverItemReductionCheck).
                            Sum(o => o.icdoPayeeAccountPaymentItemType.amount);
                        decimal ldecTaxableAmount = ibusPayeeAccount.iclbPayeeAccountPaymentItemType.
                            Where(o => o.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolleverCodeValueCanBeRolled).
                            Sum(o => o.icdoPayeeAccountPaymentItemType.amount);
                        if (ldecRolloverAmount > ldecTaxableAmount)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //  This validation will occur only when status Changed To Approved or Recieving .
        // If there is no tax record for the next benefit payment date then throw an error
        public bool ValidateWhenStatusApprovedOrRecievedForTaxwithHolding()
        {
            bool lblnFedTaxWithHoldingExist = false;
            bool lblnStateTaxWithHoldingExist = false;
            bool lblnTaxWithHoldingExist = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            //uat pir 2174 : no need for tax withholding for DC payee account
            if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id != busConstant.PlanIdDC &&
                ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id != busConstant.PlanIdDC2025) //PIR 25920 
            {
                if (ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding == null)
                    ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
                if (ibusPayeeAccount.iclbPayeeAccountStateTaxWithHolding == null)
                    ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();

                foreach (busPayeeAccountTaxWithholding lobjTaxWithHolding in ibusPayeeAccount.iclbPayeeAccountStateTaxWithHolding)
                {
                    if (lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.start_date,
                            lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.end_date))
                        {
                            lblnStateTaxWithHoldingExist = true;
                        }
                    }
                    else
                    {
                        lblnStateTaxWithHoldingExist = true;
                    }
                }
                foreach (busPayeeAccountTaxWithholding lobjTaxWithHolding in ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding)
                {
                    if (lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.start_date,
                            lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.end_date))
                        {
                            lblnFedTaxWithHoldingExist = true;
                        }
                    }
                    else
                    {
                        lblnStateTaxWithHoldingExist = true;
                    }
                }
                if ((!IsBenefitOptionTransfers()) && (!lblnFedTaxWithHoldingExist)
                    && (!lblnStateTaxWithHoldingExist) && (ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeDisability)
                    && !IsStatusCancelPending() && !IsStatusCancelled())
                {
                    return true;
                }
            }
            return false;
        }
        /* 1.Check Payee Account Active status whether it terminated or suspended
           2.And if Payee date of death is not null, then throw an error. */
        public bool IsStatusTerminatedOrSuspended()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            //Commented since LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
            //if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
            ibusPayeeAccount.LoadActivePayeeStatus();
            if (ibusPayeeAccount.ibusPayee == null)
                ibusPayeeAccount.LoadPayeePerson();
            if ((ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountRolloverDetailStatusTerminated) ||
                (ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountRolloverDetailStatusSuspended))
            {
                if (ibusPayeeAccount.ibusPayee.icdoPerson.date_of_death != DateTime.MinValue)
                {
                    return true;
                }
            }
            return false;
        }
        //Check whether Next Paymenet Date is greater than Retirement_date,if no , throw an error
        public bool IsNextPaymentDateInvalid()
        {
            if ((icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundApproved) ||
             (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentApproved))
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.ibusApplication == null)
                    ibusPayeeAccount.LoadApplication();
                if (idtNextPaymentDate == DateTime.MinValue)
                {
                    GetNextPaymentDate();
                }
                if (idtNextPaymentDate != DateTime.MinValue)
                {
                    // Check whether Next Paymenet Date is greater than Retirement_date,if no , throw an error
                    if (idtNextPaymentDate <= ibusPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_date)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //pir FIX
        public bool IsBenefitOptionTransfers()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            if (ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund
                && ibusPayeeAccount.ibusApplication.IsBenefitOptionTransfers())
            {
                return true;
            }
            return false;
        }
        //if payee restricted from person maintenace screen ,do allow to to approv
        public bool IsPayeeRestricted()
        {
            if (IsStatusApproved())
            {
                if (ibusPayeeAccount.IsNull()) LoadPayeeAccount();
                if (ibusPayeeAccount.ibusPayee.IsNull()) ibusPayeeAccount.LoadPayee();
                if (ibusPayeeAccount.ibusPayee.icdoPerson.restriction_flag == busConstant.Flag_Yes)
                    return true;
            }
            return false;
        }
        //check the amount in PAPIT as of next benefit paymentdate, if less than zero donot allow payee account status to be approved
        public bool IsNetAmountNegative()
        {
            if (IsStatusApproved())
            {
                if (ibusPayeeAccount.IsNull()) LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType.IsNull())
                    ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                IEnumerable<busPayeeAccountPaymentItemType> lenmPAPIT = ibusPayeeAccount.iclbPayeeAccountPaymentItemType
                    .Where(o => busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date,
                        o.icdoPayeeAccountPaymentItemType.end_date));
                if (lenmPAPIT != null && lenmPAPIT.Sum(o => o.icdoPayeeAccountPaymentItemType.amount * o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction) < 0)
                {
                    return true;
                }
            }
            return false;
        }
        public void GetNextPaymentDate()
        {
            //Whenever the Status is changed to ‘Approved’ then display the ‘Next_Payment_Date’ 
            //as benefit_begin_Date or Benefit Payment Date which ever is greater
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date > ibusPayeeAccount.idtNextBenefitPaymentDate)
            {
                idtNextPaymentDate = ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
            }
            else
            {
                idtNextPaymentDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
            }
        }
        public busPayeeAccountStatus ibusStatusBeforePersistChanges { get; set; }

        public override void BeforePersistChanges()
        {
            string lstrStatus = string.Empty;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            if (ibusPayeeAccount.ibusDROApplication == null)
                ibusPayeeAccount.LoadDROApplication();
            //Commented since LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
            //if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
            ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            //Whenever the Status is changed to ‘Approved’ then display the ‘Next_Payment_Date’ 
            //as benefit_begin_Date or Benefit Payment Date which ever is greater BR =45,46
            if ((icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundApproved) ||
              (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentApproved))
            {
                if (idtNextPaymentDate == DateTime.MinValue)
                {
                    GetNextPaymentDate();
                }
            }
            ibusStatusBeforePersistChanges = ibusPayeeAccount.ibusPayeeAccountActiveStatus;
            if (IsAddressNotExistForPayee()== true){
                ibusPayeeAccount.icdoPayeeAccount.pull_check_flag = "Y";
            }
        }
        //Create over payment header and details when payee account get cancelled
        //public void CreateOverPaymentWhenPayeeAccountCancelled()
        //{
        //    if (iclbCancelledPaymentHistoryHeader != null)
        //    {
        //        //Cancelling the header
        //        busPaymentHistoryHeader lbusCancelledHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
        //        lbusCancelledHistoryHeader.icdoPaymentHistoryHeader.payee_account_id = icdoPayeeAccountStatus.payee_account_id;
        //        lbusCancelledHistoryHeader.ibusPayeeAccount = ibusPayeeAccount;
        //        lbusCancelledHistoryHeader.iclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
        //        //Receivable Created for Header
        //        busPaymentHistoryHeader lbusReceivableCreatedHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
        //        lbusReceivableCreatedHistoryHeader.iclbMonthwiseAdjustmentDetails = new Collection<busPayeeAccountMonthwiseAdjustmentDetail>();
        //        lbusReceivableCreatedHistoryHeader.icdoPaymentHistoryHeader.payee_account_id = icdoPayeeAccountStatus.payee_account_id;
        //        lbusReceivableCreatedHistoryHeader.ibusPayeeAccount = ibusPayeeAccount;
        //        lbusReceivableCreatedHistoryHeader.iclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
        //        foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in iclbCancelledPaymentHistoryHeader)
        //        {
        //            if (lobjPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
        //                lobjPaymentHistoryHeader.LoadPaymentHistoryDetails();
        //            foreach (busPaymentHistoryDetail lobjPaymentHistoryDetail in lobjPaymentHistoryHeader.iclbPaymentHistoryDetail)
        //            {
        //                lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_date = lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date;
        //                if (lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value == busConstant.HistoryHeaderStatusCancel)
        //                {
        //                    lbusCancelledHistoryHeader.iclbPaymentHistoryDetail.Add(lobjPaymentHistoryDetail);
        //                }
        //                else if (lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value == busConstant.HistoryHeaderStatusReceivableCreated)
        //                {
        //                    lbusReceivableCreatedHistoryHeader.iclbPaymentHistoryDetail.Add(lobjPaymentHistoryDetail);
        //                }
        //            }
        //            if (lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value == busConstant.HistoryHeaderStatusReceivableCreated)
        //            {
        //                busPayeeAccountMonthwiseAdjustmentDetail lobjMonthwiseAdjustment = new busPayeeAccountMonthwiseAdjustmentDetail { icdoPayeeAccountMonthwiseAdjustmentDetail = new cdoPayeeAccountMonthwiseAdjustmentDetail() };
        //                lobjMonthwiseAdjustment.icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date =
        //                    lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date;
        //                lobjMonthwiseAdjustment.icdoPayeeAccountMonthwiseAdjustmentDetail.amount =
        //                    ibusPayeeAccount.GetTotalAmountPaidForTheMonth(lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date,
        //                                                                    lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date,
        //                                                                    lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id, 0);
        //                if (lobjMonthwiseAdjustment.icdoPayeeAccountMonthwiseAdjustmentDetail.amount != 0)
        //                    lbusReceivableCreatedHistoryHeader.iclbMonthwiseAdjustmentDetails.Add(lobjMonthwiseAdjustment);
        //            }
        //        }
        //        //Receivable creation part
        //        if (lbusReceivableCreatedHistoryHeader.iclbPaymentHistoryDetail.Count > 0 &&
        //            lbusReceivableCreatedHistoryHeader.iclbPaymentHistoryDetail.Sum(o => o.icdoPaymentHistoryDetail.amount) != 0)
        //        {
        //            lbusReceivableCreatedHistoryHeader.iblnIsPayeeAccountCancelled = true;
        //            lbusReceivableCreatedHistoryHeader.CreateBenefitOverPayment(busConstant.HistoryHeaderStatusReceivableCreated);
        //        }
        //        //Cancelling part 
        //        if (lbusCancelledHistoryHeader.iclbPaymentHistoryDetail.Count > 0 &&
        //            lbusCancelledHistoryHeader.iclbPaymentHistoryDetail.Sum(o => o.icdoPaymentHistoryDetail.amount) != 0)
        //        {
        //            lbusCancelledHistoryHeader.iblnIsPayeeAccountCancelled = true;
        //            lbusCancelledHistoryHeader.CreateBenefitOverPayment(busConstant.HistoryHeaderStatusCancel);
        //        }
        //    }
        //}
        //Update Person account to suspended  when payee account status changed to cancelled        
        public void UpdatePersonAccountInfoWhenPayeeAccountCancelled()
        {
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_application_id > 0)
            {
                if (ibusPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts == null)
                    ibusPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();
                foreach (busBenefitApplicationPersonAccount lobjBenAppPersonAccount in
                    ibusPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts)
                {
                    if (lobjBenAppPersonAccount.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes)
                    {
                        if (ibusPayeeAccount.ibusApplication.ibusPersonAccountRetirement == null)
                            ibusPayeeAccount.ibusApplication.ibusPersonAccountRetirement = new busPersonAccountRetirement();
                        if (ibusPayeeAccount.ibusApplication.ibusPersonAccountRetirement.FindPersonAccountRetirement(
                            lobjBenAppPersonAccount.icdoBenefitApplicationPersonAccount.person_account_id))
                        {
                            ibusPayeeAccount.ibusApplication.ibusPersonAccountRetirement.icdoPersonAccount.plan_participation_status_value
                                = busConstant.PlanParticipationStatusRetimentSuspended;
                            if (ibusPayeeAccount.ibusApplication.ibusPersonAccountRetirement.iclbRetirementContributionAll == null)
                                ibusPayeeAccount.ibusApplication.ibusPersonAccountRetirement.LoadRetirementContributionAll();
                            ibusPayeeAccount.ibusApplication.ibusPersonAccountRetirement.SetHistoryEntryRequiredOrNot();
                            ibusPayeeAccount.ibusApplication.ibusPersonAccountRetirement.ProcessHistory();
                            UpdateContributions(ibusPayeeAccount.ibusApplication.ibusPersonAccountRetirement.iclbRetirementContributionAll);
                            //PIR 11946
                            busBenefitRefundCalculation abusBenefitRefundCalculation = new busBenefitRefundCalculation();
                            abusBenefitRefundCalculation.ResetCalculationTransferFlagToNull(lobjBenAppPersonAccount.icdoBenefitApplicationPersonAccount.benefit_application_id, null);
                        }
                    }
                }
            }
            else
            {
                if (ibusPayeeAccount.ibusDROApplication == null)
                    ibusPayeeAccount.LoadDROApplication();
                if (ibusPayeeAccount.ibusDROApplication.ibusPersonAccountRetirement == null)
                    ibusPayeeAccount.ibusDROApplication.LoadRetirementPersonAccount();
                ibusPayeeAccount.ibusDROApplication.ibusPersonAccountRetirement.icdoPersonAccount.plan_participation_status_value
                    = busConstant.PlanParticipationStatusRetimentSuspended;
                ibusPayeeAccount.ibusDROApplication.ibusPersonAccountRetirement.SetHistoryEntryRequiredOrNot();
                ibusPayeeAccount.ibusDROApplication.ibusPersonAccountRetirement.ProcessHistory();
                if (ibusPayeeAccount.ibusDROApplication.ibusPersonAccountRetirement.iclbRetirementContributionAll == null)
                    ibusPayeeAccount.ibusDROApplication.ibusPersonAccountRetirement.LoadRetirementContributionAll();
                UpdateContributions(ibusPayeeAccount.ibusDROApplication.ibusPersonAccountRetirement.iclbRetirementContributionAll);
            }
        }

        

        //if payment history header is in processed status ,change to Receivable Pending  or if it is outstanding status ,change it to Cancel Pending
        private void UpdatePaymentHistoryHeaderStatusToPending()
        {
            if (ibusPayeeAccount.iclbPaymentHistoryHeader == null)
                ibusPayeeAccount.LoadPaymentHistoryHeader();
            foreach (busPaymentHistoryHeader lbusPaymentHistoryHeader in ibusPayeeAccount.iclbPaymentHistoryHeader)
            {
                lbusPaymentHistoryHeader.iblnIsPayeeAccountCancelled = true;
                if (lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusOutstanding)
                {
                    lbusPaymentHistoryHeader.btnCancelPending_Click();
                }
                else if (lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusProcessed)
                {
                    lbusPaymentHistoryHeader.btnReceivablesPending_Click();
                }
            }
        }
        //prop to add cancelled payment history header 
        //public Collection<busPaymentHistoryHeader> iclbCancelledPaymentHistoryHeader { get; set; }
        //if payment history header is in Receivable Pending status ,change to Receivable created  or if it is Cancel Pending status ,change it to Cancel         
        //public void UpdatePaymentHistoryHeaderStatusCancelledOrReceivableCreated()
        //{
        //    if (ibusPayeeAccount.iclbPaymentHistoryHeader == null)
        //        ibusPayeeAccount.LoadPaymentHistoryHeader();
        //    iclbCancelledPaymentHistoryHeader = new Collection<busPaymentHistoryHeader>();
        //    foreach (busPaymentHistoryHeader lbusPaymentHistoryHeader in ibusPayeeAccount.iclbPaymentHistoryHeader)
        //    {
        //        lbusPaymentHistoryHeader.iblnIsPayeeAccountCancelled = true;
        //        if (lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value == busConstant.HistoryHeaderStatusCancelPending)
        //        {
        //            lbusPaymentHistoryHeader.btnCancel_Click();
        //        }
        //        else if (lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value == busConstant.HistoryHeaderStatusReceivablePending)
        //        {
        //            lbusPaymentHistoryHeader.btnReceivablesCreated_Click();
        //        }
        //        iclbCancelledPaymentHistoryHeader.Add(lbusPaymentHistoryHeader);
        //    }
        //}
        //When changing the status to cancelled ,Load negative contribution which are posted at the time of retirement and 
        // re-post it as positive contributions
        private void UpdateContributions(Collection<busPersonAccountRetirementContribution> aclbPersonAccountRetContribution)
        {
            //if multiple times, contribution is negated need to aggregate that amount
            //as per Satya 14/april/10
            IEnumerable<busPersonAccountRetirementContribution> lenmRetrContribution = aclbPersonAccountRetContribution.Where(o =>
                o.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueBenefitPayment); //PIR 15304 - As per Maik, sum all benefit payment lines for each bucket 
            ibusPayeeAccount.LoadPaymentHistoryHeader(); //PIR 15304
            if (lenmRetrContribution.Any() && ibusPayeeAccount.iclbPaymentHistoryHeader.IsNotNull() && ibusPayeeAccount.iclbPaymentHistoryHeader.Count > 0)
            {
                busPersonAccountRetirementContribution lobjNewContribution = new busPersonAccountRetirementContribution
                {
                    icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution()
                };
                var lobjRetrContribution = lenmRetrContribution.FirstOrDefault();
                lobjNewContribution.icdoPersonAccountRetirementContribution.person_account_id =
                    lobjRetrContribution.icdoPersonAccountRetirementContribution.person_account_id;
                lobjNewContribution.icdoPersonAccountRetirementContribution.subsystem_value =
                    lobjRetrContribution.icdoPersonAccountRetirementContribution.subsystem_value;
                lobjNewContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id =
                  lobjRetrContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id;
                lobjNewContribution.icdoPersonAccountRetirementContribution.effective_date = DateTime.Today;
                lobjNewContribution.icdoPersonAccountRetirementContribution.transaction_date =
                    icdoPayeeAccountStatus.status_effective_date;
                lobjNewContribution.icdoPersonAccountRetirementContribution.pay_period_month =
                   icdoPayeeAccountStatus.status_effective_date.Month;
                lobjNewContribution.icdoPersonAccountRetirementContribution.pay_period_year =
                   icdoPayeeAccountStatus.status_effective_date.Year;
                if (ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                    lobjNewContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypeCancelRetirement;
                else if (ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                    lobjNewContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypeCancelRefund;
                else if (ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                    lobjNewContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypeCancelPreRetirementDeath;
                lobjNewContribution.icdoPersonAccountRetirementContribution.post_tax_ee_amount =
                    lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.post_tax_ee_amount) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.post_tax_er_amount =
                    lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.post_tax_er_amount) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount =
                    lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.pre_tax_er_amount) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.pre_tax_ee_amount =
                    lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.pre_tax_ee_amount) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.ee_rhic_amount =
                    lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.ee_rhic_amount) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.er_rhic_amount =
                    lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.er_rhic_amount) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_amount =
                    lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.ee_er_pickup_amount) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.er_vested_amount =
                    lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.er_vested_amount) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.interest_amount =
                  lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.interest_amount) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.vested_service_credit =
                  lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.vested_service_credit) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.pension_service_credit =
                  lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.pension_service_credit) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.employer_interest =
                  lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.employer_interest) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.employer_rhic_interest =
                  lenmRetrContribution.Sum(o => o.icdoPersonAccountRetirementContribution.employer_rhic_interest) * -1;
                lobjNewContribution.icdoPersonAccountRetirementContribution.Insert();
            }
        }
        //BR-079-79d	Payee Status	The system must not allow user to update the ‘Payee Status’ to ‘Approved’ if the ‘Payment Option’ under ‘Benefit Underpayment’ is Blank.
        public bool IsUnderPaymentOptionBlank()
        {
            if (IsStatusApproved())
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.iclbRetroPayment == null)
                    ibusPayeeAccount.LoadRetroPayment();
                if (ibusPayeeAccount.iclbRetroPayment.Where(o =>
                    o.icdoPayeeAccountRetroPayment.retro_payment_type_value != busConstant.RetroPaymentTypePopupBenefits &&
                    (string.IsNullOrEmpty(o.icdoPayeeAccountRetroPayment.payment_option_value)
                    || o.icdoPayeeAccountRetroPayment.payment_option_value == " ")).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        // Load Top one row, order by payee account status id desc
        public void LoadPreviousStatus()
        {
            if (ibusPreviousStatus == null)
                ibusPreviousStatus = new busPayeeAccountStatus();
            ibusPreviousStatus.icdoPayeeAccountStatus = new cdoPayeeAccountStatus();
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbPayeeAccountStatus == null)
                ibusPayeeAccount.LoadPayeeAccountStatus();
            if (ibusPayeeAccount.iclbPayeeAccountStatus.Count > 0)
                ibusPreviousStatus.icdoPayeeAccountStatus = ibusPayeeAccount.iclbPayeeAccountStatus[0].icdoPayeeAccountStatus;
        }

        // *** BR-074-16 *** System must not allow same user to change the deduction and then audit the Payee Account for the change.
        public bool IsSameUserApprovingReviewedRecord()
        {
            if (ibusPreviousStatus == null)
                LoadPreviousStatus();
            if (icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusRetirmentApproved)
            {
                if (ibusPreviousStatus.icdoPayeeAccountStatus.modified_by.Trim().ToLower() == iobjPassInfo.istrUserID.Trim().ToLower())
                    return true;
            }
            return false;
        }
        // *** BR-079-83 *** the same user who changed the status to cancel pending can not cancel the Payee Account.
        public bool IsSameUserCancelPayeeAccount()
        {
            if (ibusPreviousStatus == null)
                LoadPreviousStatus();
            if (icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusCancelled)
            {
                if (ibusPreviousStatus.icdoPayeeAccountStatus.modified_by.Trim().ToLower() == iobjPassInfo.istrUserID.Trim().ToLower())
                    return true;
            }
            return false;
        }
        // *** BR-074-14 *** Total RHIC Benefit Reimbursement should not be greater than 3rd Party Health Insurance.
        public bool IsRHICBenefitGreaterThanHealthInsurance()
        {
            if (IsApprovedOrReceiving)
            {
                if (icdoPayeeAccountStatus.status_effective_date != DateTime.MinValue)
                {
                    decimal ldecRHICBenefitAmount = 0.0M;
                    decimal ldec3rdPartyHealthInsuranceAmount = 0.0M;
                    if (ibusPayeeAccount == null)
                        LoadPayeeAccount();
                    // Load all the Payment Item Type
                    if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                        ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                    foreach (busPayeeAccountPaymentItemType lobjPAPIT in ibusPayeeAccount.iclbPayeeAccountPaymentItemType)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(icdoPayeeAccountStatus.status_effective_date,
                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.start_date,
                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date))
                        {
                            // Calculate RHIC Benefit Amount
                            if (lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PaymentItemTypeIDRHICBenefitReimbursement)
                                ldecRHICBenefitAmount += lobjPAPIT.icdoPayeeAccountPaymentItemType.amount;
                            // Calculate 3rd Health Insurance 
                            else if (lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PaymentItemTypeID3rdPartyHealthInsurance)
                                ldec3rdPartyHealthInsuranceAmount += lobjPAPIT.icdoPayeeAccountPaymentItemType.amount;
                        }
                    }
                    if (ldec3rdPartyHealthInsuranceAmount > 0 && ldecRHICBenefitAmount > ldec3rdPartyHealthInsuranceAmount)
                        return true;
                }
            }
            return false;
        }
        //Current Status effective date cannot be less than previous status effective date
        public bool IsStatusEffectiveDateInvalid()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbPayeeAccountStatus == null)
                ibusPayeeAccount.LoadPayeeAccountStatus();
            if (ibusPayeeAccount.iclbPayeeAccountStatus.Count > 0)
                if (icdoPayeeAccountStatus.status_effective_date < ibusPayeeAccount.iclbPayeeAccountStatus[0].icdoPayeeAccountStatus.status_effective_date)
                    return true;
            return false;
        }

        //PIR 1996-Do not allow to approve if there is no address for the payee

        public bool IsAddressNotExistForPayee()
        {
            if (IsStatusApproved())
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.ibusPayee == null)
                    ibusPayeeAccount.LoadPayee();
                if (ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != 0)
                {
                    if (ibusPayeeAccount.ibusPayee.iclbPersonAddress == null)
                        ibusPayeeAccount.ibusPayee.LoadPersonAddress();
                    if (ibusPayeeAccount.ibusPayee.iclbPersonAddress.Where(o =>
                        busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate,
                        o.icdoPersonAddress.start_date, o.icdoPersonAddress.end_date)).Count() == 0)
                        return true;
                }
                else if (ibusPayeeAccount.icdoPayeeAccount.payee_org_id != 0)
                {
                    // PROD PIR 4713 - Throws an error if the Recipient org has no Primary address
                    if (ibusPayeeAccount.ibusRecipientOrganization.icdoOrganization.primary_address_id == 0)
                        return true;
                }
            }
            return false;
        }

        //PIR 1438 - Do not allow to approve when calculation cancelled

        public bool IsCalculationCancelled()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusBenefitCalculaton == null)
                ibusPayeeAccount.LoadBenefitCalculation();
            if (IsStatusApproved() &&
                ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.action_status_value == busConstant.CalculationStatusCancel)
                return true;
            else
                return false;
        }

        //BR-057-12
        public bool Is31DayRuleMet()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if ((ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund) &&
                (ibusPayeeAccount.icdoPayeeAccount.application_id > 0))
            {
                if (ibusPayeeAccount.ibusApplication == null)
                    ibusPayeeAccount.LoadApplication();
                TimeSpan ltsDays = icdoPayeeAccountStatus.status_effective_date.Subtract(ibusPayeeAccount.ibusApplication.icdoBenefitApplication.termination_date);
                if (ltsDays.Days < 31)
                {
                    return true;
                }
            }
            return false;
        }
        //PIR 1565
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (icdoPayeeAccountStatus.status_value != null)
            {
                DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203, icdoPayeeAccountStatus.status_value);
                icdoPayeeAccountStatus.status_value_data1 = ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString() : null;
            }
            base.BeforeValidate(aenmPageMode);
        }
        //Constants used for validation
        //TERR - terminated_status_reason_value is required : TERN - terminated_status_reason_value is not required
        //SUSR - suspension_status_reason_value is required : SUSN - suspension_status_reason_value is not required
        public string IsSuspensionOrTerminationStatusValid()
        {
            if (icdoPayeeAccountStatus.status_value_data1 != null)
            {
                if ((icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusTerminated ||
                    icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusCancelPending ||
                   icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusCancelled) &&
                    icdoPayeeAccountStatus.terminated_status_reason_value == null)
                {
                    return "TERR";
                }
                else if (icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusSuspended &&
                    icdoPayeeAccountStatus.suspension_status_reason_value == null)
                {
                    return "SUSR";
                }

                if (icdoPayeeAccountStatus.suspension_status_reason_value != null &&
                    icdoPayeeAccountStatus.status_value_data1 != busConstant.PayeeAccountStatusSuspended)
                {
                    return "SUSN";
                }
                else if (icdoPayeeAccountStatus.terminated_status_reason_value != null && !(
                    icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusTerminated ||
                    icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusCancelPending ||
                   icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusCancelled))
                {
                    return "TERN";
                }
            }
            return string.Empty;
        }

        public override void AfterPersistChanges()
        {
            //UCS- 082-38
            //The system must un-suspend a suspended ‘Disability Recertification’ workflow associated with a
            //payee account being changed to a ‘Cancelled’ or ‘Payment Complete’ status 
            if ((icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusDisabilityCancelled) ||
                (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusDisabilityPaymentCompleted))
            {
                // Fetch all cases for that Payee Account ID
                DataTable ldtCases = Select<cdoCase>(new string[1] { "payee_account_id" }, new object[1] { icdoPayeeAccountStatus.payee_account_id }, null, null);
                foreach (DataRow ldtrCase in ldtCases.Rows)
                {
                    busCase lobjCase = new busCase { icdoCase = new cdoCase() };
                    lobjCase.icdoCase.LoadData(ldtrCase);

                    int lintMapID;
                    if (lobjCase.icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
                        lintMapID = busConstant.Map_Recertify_Disability;
                    else
                        lintMapID = busConstant.Map_Recertify_Pre1991_Disability;

                    // Resume Suspended activity
                    //venkat check query
                    DataTable ldtActivityInstance = Select("entSolBpmActivityInstance.LoadSuspendedInstancesByProcessAndReference", new object[2] { lintMapID, lobjCase.icdoCase.case_id });
                    foreach (DataRow dr in ldtActivityInstance.Rows)
                    {
                        busBpmActivityInstance lobjActivityInstance = busWorkflowHelper.GetActivityInstance(Convert.ToInt32(dr["activity_instance_id"]));
                        lobjActivityInstance.ibusBaseActivityInstance = lobjActivityInstance;
                        busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusResumed, lobjActivityInstance, iobjPassInfo);
                    }
                }
            }
            //UCS - 079 : to check whether there is an early retirement payee account and there is a disablility application
            if (ibusPayeeAccount.iclbBenefitApplication == null)
                ibusPayeeAccount.LoadDisabilityBenefitApplicationsByPersonID(ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id);
            //BR-79-84  The system must change the ‘Payment History Status’ to ‘Cancel Pending’ once the ‘Payee Status’ changed to ‘Cancel Pending’.
            if (IsStatusCancelPending())
            {
                if (!(ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly &&
                    ibusPayeeAccount.iclbBenefitApplication.Count > 0))
                {
                    UpdatePaymentHistoryHeaderStatusToPending();
                }
            }
            //Check status cancelled
            if (IsStatusCancelled())
            {
                //BR-057-44
                //PIR 1607 Change application status to valid when payee account is cancelled
                if (ibusPayeeAccount.icdoPayeeAccount.application_id > 0)
                {
                    if (ibusPayeeAccount.ibusBenefitCalculaton == null)
                        ibusPayeeAccount.LoadBenefitCalculation();
                    if (ibusPayeeAccount.ibusApplication == null)
                        ibusPayeeAccount.LoadApplication();
                    if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.status_value == busConstant.ApplicationStatusProcessed)
                    {
                        ibusPayeeAccount.ibusApplication.icdoBenefitApplication.status_value = busConstant.ApplicationStatusValid;
                    }
                    ibusPayeeAccount.ibusApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusCancelled;
                    ibusPayeeAccount.ibusApplication.icdoBenefitApplication.Update();


                    //Satya told if there is more than payee account one for calculation,do not cancell the calculation,cancell application - PIR 1129
                    if (ibusPayeeAccount.icdoPayeeAccount.calculation_id > 0)
                    {
                        if (ibusPayeeAccount.ibusBenefitCalculaton.iclbPayeeAccount == null)
                            ibusPayeeAccount.ibusBenefitCalculaton.LoadPayeeAccount();
                        if (ibusPayeeAccount.ibusBenefitCalculaton.iclbPayeeAccount.Count == 1)
                        {
                            ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.status_value = busConstant.ApplicationStatusValid;
                            ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusCancel;
                            ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.Update();
                        }
                        else
                        {
                            if (ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.status_value == busConstant.CalculationStatusProcessed)
                            {
                                ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.status_value = busConstant.ApplicationStatusValid;
                                ibusPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.Update();
                            }
                        }
                    }
                }
                else if (ibusPayeeAccount.icdoPayeeAccount.dro_calculation_id > 0)
                {
                    if (ibusPayeeAccount.ibusDROApplication == null)
                        ibusPayeeAccount.LoadDROApplication();
                    if (ibusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.status_value == busConstant.ApplicationStatusProcessed)
                    {
                        ibusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.status_value = busConstant.ApplicationStatusValid;
                    }
                    ibusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.dro_status_value = busConstant.DROApplicationStatusCancelled;
                    ibusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.Update();
                }
                //PIR 18100 - MOUs are Doubled When a Payment is Receivable Created & Payee Account then Canceled, 
                //17988 ensures that user cannot cancel a payee account before canceling the outstanding payment headers and receivable creating 
                //the processed payments, btnCancel_Click and btnReceivableCreated_Click of the payment headers already have the 
                //logic to create benefit overpayment, so we do not need this logic here anymore, commented as discussed with Maik dated Janaury 10, 2018
                //if (!(ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly &&
                //    ibusPayeeAccount.iclbBenefitApplication.Count > 0))
                //{
                //    //Update Payment History Header Status Cancelled Or ReceivableCreated              
                //    UpdatePaymentHistoryHeaderStatusCancelledOrReceivableCreated();
                //    CreateOverPaymentWhenPayeeAccountCancelled();
                //}
                //Update Person account information related when payee account status changed to cancelled
                UpdatePersonAccountInfoWhenPayeeAccountCancelled();
            }
            if (IsStatusApproved())
            {
                //PIR 18974
                if (IsBenefitOverpayment())
                {
                    if (ibusPayeeAccount.IsNotNull() && ibusPayeeAccount.icdoPayeeAccount.payee_account_id > 0 && ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath ||
                        ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                    {
                        busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Initialize_Process_PA_Death_Notification_Workflow, ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id, 0, 0, iobjPassInfo);
                    }
                    else
                    {
                        busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Process_MOU_Collection, ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id, 0, 0, iobjPassInfo);
                    }
                }
                //UCS - 079 - br-079-54 : update recovery status to statisfied on approval of payee account 
                UpdateRecoveryStatus();

                if (ibusPayeeAccount.ibusSoftErrors == null)
                    ibusPayeeAccount.LoadErrors();
                ibusPayeeAccount.iblnClearSoftErrors = true;
                ibusPayeeAccount.ibusSoftErrors.iblnClearError = true;
                ibusPayeeAccount.ValidateSoftErrors();
                ibusPayeeAccount.UpdateValidateStatus();
            }
            string lstrStatusBeforePersistChanges = busGlobalFunctions.GetData2ByCodeValue(2203, ibusStatusBeforePersistChanges.icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (IsStatusReview() && lstrStatusBeforePersistChanges == busConstant.PayeeAccountStatusSuspended)
            {
                CreateReactivationBenfitUnderPayment();
            }
            //PIR 14346 - Commented this block
            //UCS 55 - RHIC Combine
            //if (IsStatusApproved())
            //{
            //    if (ibusPayeeAccount.ibusBenefitCalculaton == null)
            //        ibusPayeeAccount.LoadBenefitCalculation();

            //    //Initial Approval
            //    if ((ibusPayeeAccount.ibusBenefitCalculaton.IsCalcTypeFinalApproved()) && (!ibusPayeeAccount.IsPaymentHistoryRecordFound()))
            //    {
            //        CreateRHICCombineOnInitialPaymentApproval();
            //    }
            //    //Recalculation of Benefit Adjustments
            //    else if (ibusPayeeAccount.ibusBenefitCalculaton.IsCalcTypeAdjustmentApproved())
            //    {
            //        CreateRHICCombineOnPaymentAdjustmentApproval();
            //    }
            //}
            //else if (IsStatusCancelled())
            //{
            //    CreateRHICCombineForPaymentCancelled();
            //}
            //else if (IsStatusSuspended()) //UAT PIR - 2166 : initiate without checking suspension reason // && icdoPayeeAccountStatus.suspension_status_reason_value == busConstant.PayeeAccountStatusSuspensionReasonRTW)
            //{
            //    InitiateRhicCombineWorkflow();
            //}
            //else if (IsStatusCompleted())
            //{
            //    CreateRHICCombineOnPaymentCompleted();
            //}

            if (IsStatusCancelled())
            {
                CancelRHICCombineOnPaymentCancelled();
            }
            else if (IsStatusSuspended() || IsStatusCompleted())
            {
                UpdateRHICCombineToEndedOnPaymentCompleteOrSuspended();
            }
        }
        //PIR 26896 - Removed validation 5646
        //public ArrayList LoadHardErrorforPayeeHavingAddress()
        //{
        //    ArrayList larrlist = new ArrayList();
            
        //    if (IsAddressNotExistForPayee())
        //    {
        //        utlError lobjError = new utlError();
                
        //        string lstrErrorMessage = busGlobalFunctions.GetMessageTextByMessageID(8, iobjPassInfo) + " " + busGlobalFunctions.GetMessageTextByMessageID(5646, iobjPassInfo);
        //        lobjError.istrErrorMessage = lstrErrorMessage;
        //        larrlist.Add(lobjError);
        //        return larrlist;
        //    }
        //    return larrlist;
        //}
        private void CancelRHICCombineOnPaymentCancelled()
        {
            bool lblnInitiateCreateRHIC = false;
            if (ibusPayeeAccount.ibusBenefitAccount == null)
                ibusPayeeAccount.LoadBenfitAccount();
            if ((ibusPayeeAccount.icdoPayeeAccount.rhic_amount > 0) ||
                (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0))
            {
                //Load the existing approved combined Rhic Record for this Donor Payee Account
                if (ibusPayeeAccount.ibusLatestBenefitRhicCombine == null)
                    ibusPayeeAccount.LoadLatestBenefitRhicCombine(true);

                //We cancel the previous record only if the old rhic record exists
                if (ibusPayeeAccount.ibusLatestBenefitRhicCombine != null)
                {
                    //If the existing approved combine record has other donors too, we have to call the automativeRhicCombine method to cancel the previous and establish the new one.
                    //otherwise, we can simply cancel the current rhic here itself
                    if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail == null)
                        ibusPayeeAccount.ibusLatestBenefitRhicCombine.LoadBenefitRhicCombineDetails();
                    //If the receiver and the payee whose payee account is canceled are the same
                    if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id == ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id)
                    {
                        if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.Count <= 1) //Conversion records may have 0 details
                        {
                            //Just Cancel the Previous one
                            ibusPayeeAccount.ibusLatestBenefitRhicCombine.CancelRHICCombine();
                        }
                        else if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.Count == 2) //this receiver has other donor payee accounts  
                        {
                            foreach (busBenefitRhicCombineDetail lbusBenefitRhicCombineDetail in ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail)
                            {
                                lbusBenefitRhicCombineDetail.LoadPayeeAccount();
                            }
                            if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail
                                .Any(i => i.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id))
                            { //donor details are two, one of the detail is spouse's
                                //ibusPayeeAccount.ibusLatestBenefitRhicCombine.CancelRHICCombine();
                                busBenefitRhicCombineDetail lbusBenefitRhicCombineDetail = ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail
                                    .Where(i => i.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id
                                            && i.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes).FirstOrDefault();

                                if (lbusBenefitRhicCombineDetail.IsNotNull())
                                {   //spouse's detail is combined scenario
                                    int lintSpousePerslinkID = lbusBenefitRhicCombineDetail.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id;
                                    int lintSpousePersonID = 0;
                                    //Before Rhic Record gets created, spouse may go inactive.. so, lets find out the spouse of each other and create an automatic RHIC for that spouse
                                    if (ibusPayeeAccount.ibusPayee.iclbAllSpouse == null)
                                        ibusPayeeAccount.ibusPayee.LoadAllSpouse();
                                    foreach (busPerson lbusSpouse in ibusPayeeAccount.ibusPayee.iclbAllSpouse)
                                    {
                                        if (lbusSpouse.iclbAllSpouse == null)
                                            lbusSpouse.LoadAllSpouse();

                                        if (lbusSpouse.iclbAllSpouse.Any(i => i.icdoPerson.person_id == ibusPayeeAccount.ibusPayee.icdoPerson.person_id))
                                        {
                                            lintSpousePersonID = lbusSpouse.icdoPerson.person_id;
                                            break;
                                        }
                                    }
                                    if (lintSpousePerslinkID == lintSpousePersonID)
                                    {
                                        ibusPayeeAccount.ibusLatestBenefitRhicCombine.CancelRHICCombine();
                                        busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
                                        lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = lintSpousePerslinkID;
                                        lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = icdoPayeeAccountStatus.status_effective_date.GetFirstDayofCurrentMonth();
                                        lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.payment_cancelled;
                                        lbusBenefitRhicCombine.CreateAutomatedRHICCombine();
                                    }
                                    else
                                    {
                                        ibusPayeeAccount.ibusLatestBenefitRhicCombine.InitiateRHICCombineWorkflow();
                                    }
                                }
                                else
                                {
                                    //donor details count 2, one of the detail is spouse, but spouse's is not combined scenario, simply cancel the combine record
                                    ibusPayeeAccount.ibusLatestBenefitRhicCombine.CancelRHICCombine();
                                }
                            }
                            else
                            {
                                lblnInitiateCreateRHIC = true;
                            }
                        }
                        else
                        {
                            lblnInitiateCreateRHIC = true;
                        }
                    }
                    else //This payee account is donor for the spouse
                    {
                        int lintSpousePersonID = 0;
                        //Before Rhic Record gets created, spouse may go inactive.. so, lets find out the spouse of each other and create an automatic RHIC for that spouse
                        if (ibusPayeeAccount.ibusPayee.iclbAllSpouse == null)
                            ibusPayeeAccount.ibusPayee.LoadAllSpouse();
                        foreach (busPerson lbusSpouse in ibusPayeeAccount.ibusPayee.iclbAllSpouse)
                        {
                            if (lbusSpouse.iclbAllSpouse == null)
                                lbusSpouse.LoadAllSpouse();

                            if (lbusSpouse.iclbAllSpouse.Any(i => i.icdoPerson.person_id == ibusPayeeAccount.ibusPayee.icdoPerson.person_id))
                            {
                                lintSpousePersonID = lbusSpouse.icdoPerson.person_id;
                                break;
                            }
                        }

                        if (lintSpousePersonID > 0 && lintSpousePersonID == ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id)
                        {
                            lblnInitiateCreateRHIC = true;
                        }
                        else
                        {
                            ibusPayeeAccount.ibusLatestBenefitRhicCombine.InitiateRHICCombineWorkflow();                        
                        }
                    }
                    if (lblnInitiateCreateRHIC)
                    {
                        ibusPayeeAccount.ibusLatestBenefitRhicCombine.CancelRHICCombine();
                        busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
                        lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id;
                        lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = icdoPayeeAccountStatus.status_effective_date.GetFirstDayofCurrentMonth();
                        lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.payment_cancelled;
                        lbusBenefitRhicCombine.CreateAutomatedRHICCombine();
                    }
                }
            }
        }

        public void UpdateRHICCombineToEndedOnPaymentCompleteOrSuspended()
        {
            bool lblnInitiateCreateRHIC = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayee == null)
                ibusPayeeAccount.LoadPayee();

            if (ibusPayeeAccount.ibusBenefitAccount == null)
                ibusPayeeAccount.LoadBenfitAccount();
            if ((ibusPayeeAccount.icdoPayeeAccount.rhic_amount > 0) ||
               (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0))
            {
                //Load the existing approved Rhic Combine Record for this Donor Payee Account
                if (ibusPayeeAccount.ibusLatestBenefitRhicCombine == null)
                    ibusPayeeAccount.LoadLatestBenefitRhicCombine(true);

                //We end the previous record only if an approved rhic combine record exists
                if (ibusPayeeAccount.ibusLatestBenefitRhicCombine != null)
                {
                    //If the existing approved combine record has other donors too, we have to call the automatic RHIC Combine method to cancel the previous and establish the new one.
                    //otherwise, we can simply cancel the current rhic here itself
                    if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail == null)
                        ibusPayeeAccount.ibusLatestBenefitRhicCombine.LoadBenefitRhicCombineDetails();
                    //If the receiver and the payee whose payee account is completed or suspended are the same
                    if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id == ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id)
                    {
                        //Conversion Records may have 0 details or has only one donor payee account(its payee account status has just been changed to completed or suspended)
                        if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.Count <= 1) 
                        {
                            ibusPayeeAccount.ibusLatestBenefitRhicCombine.EndRHICCombine(icdoPayeeAccountStatus.status_effective_date.GetLastDayofMonth()); //PIR 14810
                        }
                        else if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.Count == 2) //this receiver has other donor payee accounts  
                        {
                            foreach (busBenefitRhicCombineDetail lbusBenefitRhicCombineDetail in ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail)
                            {
                                lbusBenefitRhicCombineDetail.LoadPayeeAccount();
                            }
                            if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail
                                .Any(i => i.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id)) //one of the donors is spouse
                            {
                                //ibusPayeeAccount.ibusLatestBenefitRhicCombine.EndRHICCombine(icdoPayeeAccountStatus.status_effective_date);
                                busBenefitRhicCombineDetail lbusBenefitRhicCombineDetail = ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail
                                    .Where(i => i.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id
                                    && i.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes).FirstOrDefault();

                                if (lbusBenefitRhicCombineDetail.IsNotNull())
                                {
                                    int lintSpousePerslinkID = lbusBenefitRhicCombineDetail.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id;
                                    int lintSpousePersonID = 0;
                                    //Before Rhic Record gets created, spouse may go inactive.. so, lets find out the spouse of each other and create an automatic RHIC for that spouse
                                    if (ibusPayeeAccount.ibusPayee.iclbAllSpouse == null)
                                        ibusPayeeAccount.ibusPayee.LoadAllSpouse();
                                    foreach (busPerson lbusSpouse in ibusPayeeAccount.ibusPayee.iclbAllSpouse)
                                    {
                                        if (lbusSpouse.iclbAllSpouse == null)
                                            lbusSpouse.LoadAllSpouse();

                                        if (lbusSpouse.iclbAllSpouse.Any(i => i.icdoPerson.person_id == ibusPayeeAccount.ibusPayee.icdoPerson.person_id))
                                        {
                                            lintSpousePersonID = lbusSpouse.icdoPerson.person_id;
                                            break;
                                        }
                                    }
                                    if (lintSpousePerslinkID == lintSpousePersonID)
                                    {
                                        ibusPayeeAccount.ibusLatestBenefitRhicCombine.EndRHICCombine(icdoPayeeAccountStatus.status_effective_date.GetLastDayofMonth()); //PIR 14810
                                        busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
                                        lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = lintSpousePerslinkID;
                                        lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = icdoPayeeAccountStatus.status_effective_date.GetFirstDayofNextMonth(); //PIR 14810
                                        lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.payment_suspended_or_completed;
                                        lbusBenefitRhicCombine.CreateAutomatedRHICCombine();
                                    }
                                    else
                                    {
                                        ibusPayeeAccount.ibusLatestBenefitRhicCombine.InitiateRHICCombineWorkflow();
                                    }
                                }
                                else
                                {
                                    ibusPayeeAccount.ibusLatestBenefitRhicCombine.EndRHICCombine(icdoPayeeAccountStatus.status_effective_date.GetLastDayofMonth()); //PIR 14810                                                                    
                                }
                            }
                            else // all donor payee accounts are receiver payee accounts, donor payee accounts more than one, 
                            {
                                lblnInitiateCreateRHIC = true;
                            }
                        }
                        else // Donor payee accounts are more than two, which may or may not contain spouse donor, one of the donor payee account has just been completed or suspended scenario
                        {
                            lblnInitiateCreateRHIC = true;
                        }
                    }
                    else //This payee account is donor for the spouse
                    {
                        int lintSpousePersonID = 0;
                        //Before Rhic Record gets created, spouse may go inactive.. so, lets find out the spouse of each other and create an automatic RHIC for that spouse
                        if (ibusPayeeAccount.ibusPayee.iclbAllSpouse == null)
                            ibusPayeeAccount.ibusPayee.LoadAllSpouse();
                        foreach (busPerson lbusSpouse in ibusPayeeAccount.ibusPayee.iclbAllSpouse)
                        {
                            if (lbusSpouse.iclbAllSpouse == null)
                                lbusSpouse.LoadAllSpouse();

                            if (lbusSpouse.iclbAllSpouse.Any(i => i.icdoPerson.person_id == ibusPayeeAccount.ibusPayee.icdoPerson.person_id))
                            {
                                lintSpousePersonID = lbusSpouse.icdoPerson.person_id;
                                break;
                            }
                        }

                        if (lintSpousePersonID > 0 && lintSpousePersonID == ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id)
                        {
                            lblnInitiateCreateRHIC = true;
                        }
                        else 
                        {
                            ibusPayeeAccount.ibusLatestBenefitRhicCombine.InitiateRHICCombineWorkflow();                                                    
                        }
                    }
                    if (lblnInitiateCreateRHIC)
                    {
                        ibusPayeeAccount.ibusLatestBenefitRhicCombine.EndRHICCombine(icdoPayeeAccountStatus.status_effective_date.GetLastDayofMonth()); //PIR 14810
                        busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
                        lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id;
                        lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = icdoPayeeAccountStatus.status_effective_date.GetFirstDayofNextMonth(); //PIR 14810
                        lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.payment_suspended_or_completed;
                        lbusBenefitRhicCombine.CreateAutomatedRHICCombine();
                    }
                }
            }
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

        //BR=79-BR-079-89a	Benefit Under-payment	The system must create ‘Benefit Underpayment’ with ‘Adjustment Type’ of ‘Reactivation Retropayment’ 
        //only if the ‘Total Gross Amount’ is greater than ZERO and ‘Payee Status’ is changed from ‘Suspended’ to ‘Review’ 
        //with ‘Effective Start Date’ as 1st day of the month after last ‘Payment Date’ in ‘Payment History Header’ for that payee.
        //‘Effective End Date’ equal to one day MINUS next ‘Benefit Payment Date’ , and ‘Status’ as ‘Approved’. 
        //The system must not create the record for ‘RTW Members’.
        private void CreateReactivationBenfitUnderPayment()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203,
                ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusSuspended)
            {
                PostRetirementIncreaseforSuspensionPeriod();

                busPayeeAccountRetroPayment lobjUnderPaymentHeader = new busPayeeAccountRetroPayment { icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment() };
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.payee_account_id = icdoPayeeAccountStatus.payee_account_id;
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.retro_payment_type_value = busConstant.RetroPaymentTypeReactivation;
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.adjustment_reason_value = busConstant.AdjustmentReasonRecalculation;
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.reactivation_date = ibusStatusBeforePersistChanges.icdoPayeeAccountStatus.status_effective_date;
                lobjUnderPaymentHeader.iarrChangeLog.Add(lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment);
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.ienuObjectState = ObjectState.Insert;
                ibusPayeeAccount.CreateBenefitUnderpaymentAdjustmentsHeader(lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.retro_payment_type_value,
                    lobjUnderPaymentHeader, DateTime.MinValue, DateTime.MinValue);
                lobjUnderPaymentHeader.BeforePersistChanges();
                lobjUnderPaymentHeader.PersistChanges();
                lobjUnderPaymentHeader.iblnIsReacivation = true;
                lobjUnderPaymentHeader.AfterPersistChanges();
            }
        }
        //UAT PIR 1173 if RHIC amount is greater than 3rd party amount ,throw an error
        public bool IsRHICGreaterThan3rdPary()
        {
            if (IsStatusApproved())
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.IsRHICGreaterThan3rdPary())
                    return true;
            }
            return false;
        }
        private void PostRetirementIncreaseforSuspensionPeriod()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();

            DataTable ldtbRequests = Select("cdoPayeeAccount.LoadPostRetirmentIncreaseRequests", new object[2]{ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_effective_date,
                    icdoPayeeAccountStatus.status_effective_date});
            foreach (DataRow ldtrReuest in ldtbRequests.Rows)
            {
                decimal ldecRefAmount = 0.0m;
                busPayeeAccountPaymentItemType lobjPAPIT = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                busPostRetirementIncreaseBatchRequest lobjPostRetirementIncreaseBatchRequest = new busPostRetirementIncreaseBatchRequest
                {
                    icdoPostRetirementIncreaseBatchRequest = new cdoPostRetirementIncreaseBatchRequest()
                };
                lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.LoadData(ldtrReuest);
                ibusPayeeAccount.GetCOLAOrAdhocAmount(lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id, ref ldecRefAmount, 0, lobjPAPIT);
                if (lobjPAPIT.icdoPayeeAccountPaymentItemType.amount > 0)
                {
                    if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                        ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                    var lPaymentItemTypeList = ibusPayeeAccount.iclbPayeeAccountPaymentItemType
                                        .Where(lobjPayItemType => lobjPayItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id == lobjPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id
                                        && busGlobalFunctions.CheckDateOverlapping(lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                        lobjPayItemType.icdoPayeeAccountPaymentItemType.start_date, lobjPayItemType.icdoPayeeAccountPaymentItemType.end_date));
                    //updating papit if same entry exists
                    foreach (var lobjPaymentItemType in lPaymentItemTypeList)
                    {
                        if (lobjPaymentItemType.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                        {
                            lobjPaymentItemType.icdoPayeeAccountPaymentItemType.end_date =
                                lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.effective_date.AddDays(-1);
                            lobjPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
                        }
                    }

                    //inserting new cola/adhoc amount
                    lobjPAPIT.icdoPayeeAccountPaymentItemType.Insert();

                    ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                    ibusPayeeAccount.LoadTaxAmounts();

                    ibusPayeeAccount.CalculateAdjustmentTax(false);
                }
            }
        }
        // BR-060-03 - , Raise an Error if the previous payee account is changed into 
        // Approved status, then check whether the RTW Person plan account status is Enrolled or Suspended
        // The system must throw the following error message if a ‘Payee’ account status is changed to ‘Approved’ on the ‘Payee’ 
        // account before RTW and member’s RTW ‘Plan Participation’ status is ‘Enrolled’ or ‘Suspended’ for the re-enrolled person plan account
        public bool IsRTWPersonAccountValid()
        {
            if ((icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusDisabilityApproved) ||
                (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentApproved))
            {
                bool lblnRTWMember = false;
                int lintPreRTWPayeeAccountID = 0;
                int lintPayeePersonAccountID = 0;

                if (ibusPayeeAccount.IsNull())
                    LoadPayeeAccount();
                if (ibusPayeeAccount.ibusMember.IsNull())
                    ibusPayeeAccount.LoadMember();
                if (ibusPayeeAccount.ibusMember.iclbRetirementAccount.IsNull())
                    ibusPayeeAccount.ibusMember.LoadRetirementAccount();
                if (ibusPayeeAccount.ibusPlan.IsNull())
                    ibusPayeeAccount.LoadPlan();

                // 1. Determine RTW Account for the Member.
                lblnRTWMember = ibusPayeeAccount.ibusMember.IsRTWMember(ibusPayeeAccount.ibusPlan.icdoPlan.plan_id,
                    busConstant.PayeeStatusForRTW.SuspendedOnly, ref lintPreRTWPayeeAccountID);

                if (lblnRTWMember)
                {
                    //2. Fetch the current payee account's Person accountid
                    ibusPayeeAccount.LoadApplication();
                    ibusPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();
                    var lobjBenefitApplicationPersonAccount = ibusPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts
                                            .Where(lobjBAPA => lobjBAPA.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).FirstOrDefault();
                    lintPayeePersonAccountID = lobjBenefitApplicationPersonAccount.icdoBenefitApplicationPersonAccount.person_account_id;

                    // 3. Fetch the RTW Person Account ID
                    busPersonAccount lobjRTWPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    lobjRTWPersonAccount = ibusPayeeAccount.ibusMember.LoadActivePersonAccountByPlan(ibusPayeeAccount.ibusPlan.icdoPlan.plan_id);

                    //4. The current Payee Account Id should be a pre RTW Person Account and not current RTW Payee Account ID.Since the Error has to be raised only
                    //For Pre RTW Payee Accounts
                    if ((lintPayeePersonAccountID != lobjRTWPersonAccount.icdoPersonAccount.person_account_id) && (lintPayeePersonAccountID > 0))
                    {
                        //5. Check the status of the Current RTW Person AccountId.
                        if ((lobjRTWPersonAccount.icdoPersonAccount.plan_participation_status_value.Equals(busConstant.PlanParticipationStatusRetimentSuspended)) ||
                            (lobjRTWPersonAccount.icdoPersonAccount.plan_participation_status_value.Equals(busConstant.PlanParticipationStatusRetirementEnrolled)))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void LoadDROApplication()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusDROApplication == null)
                ibusPayeeAccount.LoadDROApplication();
        }

        /// <summary>
        /// Method to check whether DRO application is in Qualified status
        /// </summary>
        /// <returns>Boolean value</returns>
        public bool IsDROApplicationQualified()
        {
            bool lblResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusDROApplication == null)
                LoadDROApplication();

            if (!string.IsNullOrEmpty(ibusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.dro_status_value))
            {
                if (ibusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.dro_status_value != busConstant.DROApplicationStatusQualified)
                    lblResult = true;
            }

            return lblResult;
        }

        //check payee account is suspended, Payment complete, cancelled
        //for benefit account type value Retirement
        public bool IsSuspendedOrCancelledOrCompletedForDisability()
        {
            if (!string.IsNullOrEmpty(icdoPayeeAccountStatus.status_value) && ((icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusDisabilityCancelled))
                            || (icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusDisabilityPaymentCompleted))
                            || (icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusDisabilitySuspended))))
            {
                return true;
            }
            return false;
        }


        //check payee account is Payment complete, cancelled
        //for benefit account type value Retirement
        public bool IsCancelledOrCompletedForDisability()
        {
            if (!string.IsNullOrEmpty(icdoPayeeAccountStatus.status_value) && ((icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusDisabilityCancelled))
                            || (icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusDisabilityPaymentCompleted))))
            {
                return true;
            }
            return false;
        }
        //check payee account is Payment complete, cancelled
        //for benefit account type value Retirement
        public bool IsCancelledOrCompletedForRetirement()
        {
            if (!string.IsNullOrEmpty(icdoPayeeAccountStatus.status_value) && ((icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusPaymentComplete))
                            || (icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusRetirmentCancelled))))
            {
                return true;
            }
            return false;
        }

        //check payee account is suspended, Payment complete, cancelled
        //for benefit account type value Retirement
        public bool IsSuspendedOrCancelledOrCompletedForRetirement()
        {
            if (!string.IsNullOrEmpty(icdoPayeeAccountStatus.status_value) && ((icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusPaymentComplete))
                            || (icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusSuspended))
                            || (icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusRetirmentCancelled))))
            {
                return true;
            }
            return false;
        }

        public bool IsSuspendedForRetirementOrDisability()
        {
            if (!string.IsNullOrEmpty(icdoPayeeAccountStatus.status_value) && (icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusSuspended)
                || (icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusDisabilitySuspended))))
            {
                return true;
            }
            return false;

        }
        
        public bool IsStatusReceiving()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusReceiving)
                return true;
            return false;
        }
		//PIR 17987 - Only approved or in payment payee accounts need to be put into review when their account holder deceases
        public bool IsStatusInPayment()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            return (lstrData2 == busConstant.PayeeAccountStatusReceiving || lstrData2 == busConstant.PayeeAccountStatusDCReceiving || lstrData2 == busConstant.PayeeAccountStatusReceiving);
        }

        public bool IsStatusApproved()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusApproved)
                return true;
            return false;
        }

        public bool IsStatusSuspended()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusSuspended)
                return true;
            return false;
        }

        public bool IsStatusReview()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusReview)
                return true;
            return false;
        }
        public bool IsStatusPaymentCompleteOrProcessed()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusPaymentComplete || lstrData2 == busConstant.PayeeAccountStatusRefundProcessed)
                return true;
            return false;
        }
        public bool IsStatusCancelPending()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusCancelPending)
                return true;
            return false;
        }
        public bool IsStatusCancelled()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusCancelled)
                return true;
            return false;
        }
        public bool IsStatusCompletedOrProcessed()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusPaymentComplete || lstrData2 == busConstant.PayeeAccountStatusRefundProcessed)
                return true;
            return false;
        }
        public bool IsStatusCompleted()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusPaymentComplete)
                return true;
            return false;
        }
        public bool IsStatusNotProcessed()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 != busConstant.PayeeAccountStatusRefundProcessed)
                return true;
            return false;
        }
        public bool IsStatusProcessed()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203, icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrData2 == busConstant.PayeeAccountStatusRefundProcessed)
                return true;
            return false;
        }

        //For all retirements, status cannot be changed to cancelled before changing to Cancel pending
        public bool IsStatusCancelNotAllowed()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            //Commented since LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
            //if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
            ibusPayeeAccount.LoadActivePayeeStatus();
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203,
                ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (IsStatusCancelled() && //ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeRefund && //uat pir 1420
                lstrData2 != busConstant.PayeeAccountStatusCancelPending)
            {
                return true;
            }
            return false;
        }

        // BR-054-43,56 - The system must not allow a Payee Status of 'Approved','Receiving' or 'Review' 
        // when a Death Notification, other than ‘Cancelled’, exists for the payee.
        public bool IsDeathNotifiedForPayee()
        {
            if (ibusPayeeAccount.IsNull())
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayee.IsNull())
                ibusPayeeAccount.LoadPayee();
            if (ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0)
                return ibusPayeeAccount.ibusPayee.IsDeathNotified();
            else
                return false;
        }

        public bool IsStatusRefundApprovedOrRefundReview()
        {
            if ((icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundApproved) ||
                (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundReview))
                return true;
            return false;
        }

        //uat pir 1420
        //For refund payee account, status can be changed from processed to only cancel pending
        public bool IsStatusCancelPendingForRefundPayeeAccount()
        {
            bool lblnResult = false;

            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            //Commented since LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
            //if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
            ibusPayeeAccount.LoadActivePayeeStatus();
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203,
                ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund &&
                lstrData2 == busConstant.PayeeAccountStatusRefundProcessed && !IsStatusCancelPending())
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool IsStatusNotCancelled()
        {
            bool lblnResult = false;
            //prod pir 5785 : loading payee account if null
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();

            //Commented since LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
            //if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
            ibusPayeeAccount.LoadActivePayeeStatus();
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2203,
                ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);

            if (lstrData2 == busConstant.PayeeAccountStatusCancelPending && !IsStatusCancelled())
                lblnResult = true;

            return lblnResult;
        }

        #region UCS - 079

        /// <summary>
        /// Method to check whether user is eligible for approval of PAS
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsUserEligibleForApproval()
        {
            bool lblnResult = true;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            //check whether PAS is approved
            if (icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusApproved)
            {
                foreach (busPayeeAccountPaymentItemType lobjPAPIT in ibusPayeeAccount.iclbPayeeAccountPaymentItemType)
                {
                    //checking whether created by or modified by is same user
                    if ((lobjPAPIT.icdoPayeeAccountPaymentItemType.created_by == iobjPassInfo.istrUserID ||
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.modified_by == iobjPassInfo.istrUserID) &&
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.miscellaneous_correction_flag == busConstant.Flag_Yes)
                    {
                        lblnResult = false;
                        break;
                    }
                }
            }
            return lblnResult;
        }
        //check whether adjustments with type "popup benefit" is approved  
        public bool IsPopupBenefitApproved()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbRetroPayment == null)
                ibusPayeeAccount.LoadRetroPayment();
            if (ibusPayeeAccount.iclbRetroPayment.Where(o =>
                o.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypePopupBenefits &&
                o.icdoPayeeAccountRetroPayment.approved_flag == busConstant.Flag_Yes).Count() > 0)
            {
                return true;
            }
            return false;
        }
        //check whether Popup Benefit Approved and Benefit option is other than Single life
        public bool IsPopupBenefitApprovedAndNotSingleLifeOption()
        {
            if (IsStatusApproved())
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.iclbRetroPayment == null)
                    ibusPayeeAccount.LoadRetroPayment();
                if (ibusPayeeAccount.iclbRetroPayment.Where(o =>
                    o.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypePopupBenefits &&
                    o.icdoPayeeAccountRetroPayment.approved_flag == busConstant.Flag_Yes).Count() > 0)
                {
                    if (ibusPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOptionSingleLife)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //check whether Popup Benefit Approved and rhic option is not standard rhic
        public bool IsPopupBenefitApprovedAndNotRHICOptionStandardOrOptionFactorNotNull()
        {
            if (IsStatusApproved())
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.iclbRetroPayment == null)
                    ibusPayeeAccount.LoadRetroPayment();
                if (ibusPayeeAccount.iclbRetroPayment.Where(o =>
                    o.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypePopupBenefits &&
                    o.icdoPayeeAccountRetroPayment.approved_flag == busConstant.Flag_Yes).Count() > 0)
                {
                    if (ibusPayeeAccount.ibusBenefitAccount == null)
                        ibusPayeeAccount.LoadBenfitAccount();

                    if (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value != busConstant.RHICOptionStandard ||
                        ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.option_factor > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //Property to contain approved Payment recovery for payee account
        public busPaymentRecovery ibusPaymentRecovery { get; set; }
        /// <summary>
        /// Method to load Payment recovery
        /// </summary>
        public void LoadPaymentRecovery()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            DataTable ldtRecovery = Select<cdoPaymentRecovery>(new string[2] { enmPaymentRecovery.payee_account_id.ToString(), enmPaymentRecovery.status_value.ToString() },
                                                                new object[2] { ibusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.RecoveryStatusApproved },
                                                                null, null);
            ibusPaymentRecovery = new busPaymentRecovery { icdoPaymentRecovery = new cdoPaymentRecovery() };
            if (ldtRecovery.Rows.Count > 0)
                ibusPaymentRecovery.icdoPaymentRecovery.LoadData(ldtRecovery.Rows[0]);
        }
        /// <summary>
        /// Method to update recovery status to satisfied
        /// </summary>
        private void UpdateRecoveryStatus()
        {
            if (ibusPaymentRecovery == null)
                LoadPaymentRecovery();
            if (ibusPaymentRecovery.icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReductionForRTW)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRecovery, ibusPaymentRecovery.icdoPaymentRecovery.status_value,
                                        busConstant.RecoveryStatusSatisfied, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValuePensionRecv, null, ibusPaymentRecovery, iobjPassInfo);
                ibusPaymentRecovery.icdoPaymentRecovery.status_value = busConstant.RecoveryStatusSatisfied;
                ibusPaymentRecovery.icdoPaymentRecovery.Update();
            }
        }

        #endregion

        # region RHIC Combine

        //UCS - 55 - 139
        private DateTime CalculateRHICEffectiveStartDateForInitialPaymentApproval(ref bool ablnOutInitiateWorkflow)
        {
            DateTime ldtEffectiveStartDate = DateTime.MinValue;

            if ((ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
               (ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
            {
                //NDPERS defined first payment date
                if (ibusPayeeAccount.ibusApplication == null)
                    ibusPayeeAccount.LoadApplication();

                if (ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value != busConstant.ApplicationBenefitSubTypeDNRO
                    && ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value != busConstant.ApplicationBenefitSubTypeEarly) //PIR 13085
                {
                    ldtEffectiveStartDate = ibusPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_date.GetFirstDayofNextMonth();
                }
                else //for DNRO retirement payee account
                {
                    ldtEffectiveStartDate = ibusPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_date;
                }

                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();

                //If the first payment is being disbursed later than defined benefit payment cycle, initiate workflow
                if (ldtEffectiveStartDate < ibusPayeeAccount.idtNextBenefitPaymentDate)
                {
                    ablnOutInitiateWorkflow = true;
                    return DateTime.MinValue;
                }
            }
            else if ((ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) ||
                     (ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath))
            {
                ldtEffectiveStartDate = ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
            }

            //DC Plan
            if (ibusPayeeAccount.ibusPlan == null)
                ibusPayeeAccount.LoadPlan();

            if (ibusPayeeAccount.ibusPayee == null)
                ibusPayeeAccount.LoadPayee();
            if (ibusPayeeAccount.ibusPlan.IsDCRetirementPlan() || ibusPayeeAccount.ibusPlan.IsHBRetirementPlan())
            {
                //Sujatha Sent Mail Maik dated on 9/3/2010
                //If prior NG, logic needs to modify
                bool lblnPriorNG = false;
                //UAT PIR:2077 Changes.
                if (ibusPayeeAccount.ibusPayee.IsFormerDBPlanTransfertoDC(busConstant.PlanIdNG))
                    lblnPriorNG = true;

                if (lblnPriorNG)
                {
                    DateTime ldtRetirementDate = ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                    DateTime ldtPayee50YearDate = ibusPayeeAccount.ibusPayee.icdoPerson.date_of_birth.AddYears(50).GetFirstDayofNextMonth();
                    ldtEffectiveStartDate = ldtRetirementDate > ldtPayee50YearDate ? ldtRetirementDate : ldtPayee50YearDate;
                }
                else
                {
                    DateTime ldtRetirementDate = ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                    DateTime ldtPayee55YearDate = ibusPayeeAccount.ibusPayee.icdoPerson.date_of_birth.AddYears(55).GetFirstDayofNextMonth();
                    DateTime ldtRuleOf85Date = ibusPayeeAccount.GetRuleOf85Date().GetFirstDayofNextMonth();

                    //Logic Change UAT PIR : 2052 Ref: David Mail dated on 7/28/2010
                    DateTime ldtEarlierOfAge55VsRule85 = ldtPayee55YearDate < ldtRuleOf85Date ? ldtPayee55YearDate : ldtRuleOf85Date; //Earlier

                    ldtEffectiveStartDate = ldtRetirementDate > ldtEarlierOfAge55VsRule85 ? ldtRetirementDate : ldtEarlierOfAge55VsRule85; //later
                }
            }

            return ldtEffectiveStartDate;
        }
        //PIR - 14346 - Commented as we do not need to create RHIC combine record anymore on initial payment approval
        //private void CreateRHICCombineOnInitialPaymentApproval()
        //{
        //    if (ibusPayeeAccount.ibusBenefitAccount == null)
        //        ibusPayeeAccount.LoadBenfitAccount();
        //    if ((ibusPayeeAccount.icdoPayeeAccount.rhic_amount > 0) ||
        //        (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0))
        //    {
        //        busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
        //        lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id;
        //        lbusBenefitRhicCombine.ibusPerson = ibusPayeeAccount.ibusPayee;

        //        bool lblnInitiateWorkflow = false;
        //        DateTime ldtEffectiveStartDate = CalculateRHICEffectiveStartDateForInitialPaymentApproval(ref lblnInitiateWorkflow);
        //        if (lblnInitiateWorkflow)
        //        {
        //            lbusBenefitRhicCombine.InitiateRHICCombineWorkflow();
        //            return;
        //        }

        //        if (ldtEffectiveStartDate != DateTime.MinValue)
        //        {
        //            lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = ldtEffectiveStartDate;
        //            lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.initial_payment_approval;
        //            bool lblnAutoRhicEstablished = lbusBenefitRhicCombine.CreateAutomaticRHICCombine();
        //            if (lblnAutoRhicEstablished)
        //            {
        //                lbusBenefitRhicCombine.CreatePayrollAdjustment();
        //                //PROD PIR : 5183
        //                lbusBenefitRhicCombine.CreatePAPITAdjustment();
        //            }

        //        }
        //    }
        //}
        //PIR - 14346 - Commented as we do not need to create RHIC combine record anymore on adjustment approval
        //private void CreateRHICCombineOnPaymentAdjustmentApproval()
        //{
        //    if (ibusPayeeAccount.ibusBenefitAccount == null)
        //        ibusPayeeAccount.LoadBenfitAccount();
        //    if ((ibusPayeeAccount.icdoPayeeAccount.rhic_amount > 0) ||
        //        (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0))
        //    {
        //        //Load the Old Rhic Combine Record for this Donor Payee Account
        //        if (ibusPayeeAccount.ibusLatestBenefitRhicCombine == null)
        //            ibusPayeeAccount.LoadLatestBenefitRhicCombine();

        //        //If there is no Previous Record, we are not going to establish Rhic Combine Record. 
        //        //TODO: Check with Raj, is it okay to initiate workflow here
        //        if (ibusPayeeAccount.ibusLatestBenefitRhicCombine != null)
        //        {
        //            busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
        //            lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id;
        //            lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = ibusPayeeAccount.GetRHICCombineStartDate();
        //            lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.benefit_adjustment_approval;
        //            bool lblnAutoRhicEstablished = lbusBenefitRhicCombine.CreateAutomaticRHICCombine();
        //            if (lblnAutoRhicEstablished)
        //            {
        //                lbusBenefitRhicCombine.CreatePayrollAdjustment();
        //                lbusBenefitRhicCombine.CreatePAPITAdjustment();
        //            }
        //        }
        //    }
        //}
        //PIR - 14346 - Commented as we do not need to create RHIC combine record anymore on payment canceled
        //private void CreateRHICCombineForPaymentCancelled()
        //{
        //    if (ibusPayeeAccount.ibusBenefitAccount == null)
        //        ibusPayeeAccount.LoadBenfitAccount();
        //    if ((ibusPayeeAccount.icdoPayeeAccount.rhic_amount > 0) ||
        //        (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0))
        //    {
        //        //Load the Old Rhic Combine Record for this Donor Payee Account
        //        if (ibusPayeeAccount.ibusLatestBenefitRhicCombine == null)
        //            ibusPayeeAccount.LoadLatestBenefitRhicCombine();

        //        //We cancel the previous record only if the old rhic record exists
        //        if (ibusPayeeAccount.ibusLatestBenefitRhicCombine != null)
        //        {
        //            //If the Previous combine has other donors too,we have to call the automativeRhicCombine method to cancel the previous and establish the new one.
        //            //otherwise, we can simply cancel the current rhic here itself
        //            if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail == null)
        //                ibusPayeeAccount.ibusLatestBenefitRhicCombine.LoadBenefitRhicCombineDetails();

        //            //Other donors exists
        //            if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.Count == 1)
        //            {
        //                //Just Cancel the Previous one
        //                ibusPayeeAccount.ibusLatestBenefitRhicCombine.CancelRHICCombine();
        //            }
        //            else //Conversion will have 0 Records.. that also goes to the same logic which will cancel the old and create the new one.
        //            {
        //                busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
        //                lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id;
        //                lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.start_date;
        //                lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.payment_cancelled;
        //                lbusBenefitRhicCombine.CreateAutomaticRHICCombine();
        //            }
        //        }
        //    }
        //}
        //PIR - 14346 - Commented 
        //private void InitiateRhicCombineWorkflow()
        //{
        //    if (ibusPayeeAccount.ibusBenefitAccount == null)
        //        ibusPayeeAccount.LoadBenfitAccount();
        //    if ((ibusPayeeAccount.icdoPayeeAccount.rhic_amount > 0) ||
        //        (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0))
        //    {
        //        //Load the Old Rhic Combine Record for this Donor Payee Account
        //        if (ibusPayeeAccount.ibusLatestBenefitRhicCombine == null)
        //            ibusPayeeAccount.LoadLatestBenefitRhicCombine();

        //        //We cancel the previous record only if the old rhic record exists
        //        if (ibusPayeeAccount.ibusLatestBenefitRhicCombine != null)
        //        {
        //            ibusPayeeAccount.ibusLatestBenefitRhicCombine.InitiateRHICCombineWorkflow();
        //        }
        //    }
        //}
        //PIR 14346 - Commented
        //public void CreateRHICCombineOnPaymentCompleted()
        //{
        //    if (ibusPayeeAccount == null)
        //        LoadPayeeAccount();
        //    if (ibusPayeeAccount.ibusPayee == null)
        //        ibusPayeeAccount.LoadPayee();

        //    if (ibusPayeeAccount.ibusBenefitAccount == null)
        //        ibusPayeeAccount.LoadBenfitAccount();
        //    if ((ibusPayeeAccount.icdoPayeeAccount.rhic_amount > 0) ||
        //       (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0))
        //    {
        //        //Load the Old Rhic Combine Record for this Donor Payee Account
        //        if (ibusPayeeAccount.ibusLatestBenefitRhicCombine == null)
        //            ibusPayeeAccount.LoadLatestBenefitRhicCombine();

        //        //We cancel the previous record only if the old rhic record exists
        //        if (ibusPayeeAccount.ibusLatestBenefitRhicCombine != null)
        //        {
        //            bool lblnInitiateCombine = false;

        //            //Discussion with Raj : On Payment Completion only for death scenario put the end of the month of death date 
        //            //all other cases initiate the workflow.
        //            if (ibusPayeeAccount.ibusPayee.icdoPerson.date_of_death == DateTime.MinValue)
        //            {
        //                ibusPayeeAccount.ibusLatestBenefitRhicCombine.InitiateRHICCombineWorkflow();
        //            }
        //            else
        //            {
        //                //If the death person himself as a receiver, no other payee account assciated, we need to end the rhic only
        //                //else we need to establish new rhic too
        //                if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail == null)
        //                    ibusPayeeAccount.ibusLatestBenefitRhicCombine.LoadBenefitRhicCombineDetails();

        //                //Death Person himself as receiver
        //                if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id == ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id)
        //                {
        //                    //Other donors exists (conversion may have 0 records)
        //                    if (ibusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.Count <= 1)
        //                    {
        //                        //Just Cancel the Previous one
        //                        ibusPayeeAccount.ibusLatestBenefitRhicCombine.EndRHICCombine(ibusPayeeAccount.ibusPayee.icdoPerson.date_of_death.GetLastDayofMonth());
        //                    }
        //                    else
        //                    {
        //                        lblnInitiateCombine = true;
        //                    }
        //                }
        //                else
        //                {
        //                    lblnInitiateCombine = true;
        //                }

        //                if (lblnInitiateCombine)
        //                {
        //                    busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
        //                    lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id;
        //                    lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = ibusPayeeAccount.ibusPayee.icdoPerson.date_of_death.GetFirstDayofNextMonth();
        //                    lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.payment_completed_for_death_member;
        //                    lbusBenefitRhicCombine.CreateAutomaticRHICCombine();
        //                }
        //            }
        //        }

        //        //UAT PIR 2106 - According to Raj Comment, when the payee account get terminated, and member is having spouse rhic amount, we must do AR for that spouse too
        //        //This will needed for RHIC File (UCS 92 also)
        //        if ((ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0) &&
        //            (ibusPayeeAccount.ibusPayee.icdoPerson.date_of_death != DateTime.MinValue))
        //        {
        //            int lintSpousePersonID = 0;
        //            //Before Rhic Record gets created, spouse may go inactive.. so, lets find out the spouse of each other and create a AR for that spouse
        //            if (ibusPayeeAccount.ibusPayee.iclbAllSpouse == null)
        //                ibusPayeeAccount.ibusPayee.LoadAllSpouse();
        //            foreach (busPerson lbusSpouse in ibusPayeeAccount.ibusPayee.iclbAllSpouse)
        //            {
        //                if (lbusSpouse.iclbAllSpouse == null)
        //                    lbusSpouse.LoadAllSpouse();

        //                if (lbusSpouse.iclbAllSpouse.Any(i => i.icdoPerson.person_id == ibusPayeeAccount.ibusPayee.icdoPerson.person_id))
        //                {
        //                    lintSpousePersonID = lbusSpouse.icdoPerson.person_id;
        //                    break;
        //                }
        //            }

        //            if (lintSpousePersonID > 0)
        //            {
        //                busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
        //                lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = lintSpousePersonID;
        //                lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = ibusPayeeAccount.ibusPayee.icdoPerson.date_of_death.GetFirstDayofNextMonth();
        //                lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.spouse_rhic_for_termination_of_death_member;
        //                lbusBenefitRhicCombine.CreateAutomaticRHICCombine();
        //            }
        //        }
        //    }
        //}
        # endregion

        //uat pir 1605
        //method to check whether application related to payee account is in cancelled status
        public bool IsApplicationInValidStatus()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusCancelled ||
                ibusPayeeAccount.ibusApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusDeferred ||
                ibusPayeeAccount.ibusApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusDenied)
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        //uat pir 2167
        public bool IsAnyOutstandingCheckAvailable()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (IsStatusApproved() && (ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipJointAnnuitant ||
                ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipBeneficiary))
            {
                if (ibusPayeeAccount.ibusBenefitAccount == null)
                    ibusPayeeAccount.LoadBenfitAccount();
                if (ibusPayeeAccount.ibusBenefitAccount.iclbPayeeAccount == null)
                    ibusPayeeAccount.ibusBenefitAccount.LoadPayeeAccounts();
                busPayeeAccount lobjPayeeAccount = ibusPayeeAccount.ibusBenefitAccount.iclbPayeeAccount
                                                            .Where(o => o.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember)
                                                            .FirstOrDefault();
                if (lobjPayeeAccount.IsNotNull())
                {
                    lobjPayeeAccount.LoadPayee();
                    if (lobjPayeeAccount.ibusPayee.icdoPerson.date_of_death != DateTime.MinValue)
                    {
                        lobjPayeeAccount.LoadPaymentHistoryHeader();
                        if (lobjPayeeAccount.iclbPaymentHistoryHeader.Where(o => o.icdoPaymentHistoryHeader.payment_date > lobjPayeeAccount.ibusPayee.icdoPerson.date_of_death
                                 && o.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusOutstanding).Any())
                        {
                            lblnResult = true;
                        }
                    }
                }
            }
            return lblnResult;
        }
        //pir 8536
        public string istrPayeeAccountStatusData2 { get; set; }

        //PIR 13854
        public bool IsStatusApprovedForFedTax()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding == null)
                ibusPayeeAccount.LoadFedTaxWithHoldingInfo();

            if (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusApprovedPSTD || icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusApprovedDETH)
            {
                if (ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding.Count() == 0 && ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipBeneficiary &&
                    (ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.PayrollDetailStatusPosted || ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.PayrollDetailMemberStatusDeath)
                    && (ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund || ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionAutoRefund))
                    return true;
                else
                    return false;
            }
            return false;
        }
        //PIR 17215 - Throw an error when payee account status is changed to Cancel Pending and payments exist that are not canceled 
        //PIR 17988 - Modified the above validation, when payee account status is changed to Cancel Pending and payments exist that are not canceled or Receivable Created.
        //PIR 18477 - Modified the above validation, when payee account status is changed to Cancel Pending and payments exist that are not canceled, cancel Prior Year Payment, or Receivable Created
        public bool AreNotCanceledPaymentsExist()
        {
            if (!(string.IsNullOrEmpty(icdoPayeeAccountStatus.status_value)) && (string.IsNullOrEmpty(icdoPayeeAccountStatus.status_value_data1)))
            {
                DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203, icdoPayeeAccountStatus.status_value);
                icdoPayeeAccountStatus.status_value_data1 = ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString() : null;
            }
            if (!(string.IsNullOrEmpty(icdoPayeeAccountStatus.status_value_data1)) && (icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusCancelPending))
            {
                if (ibusPayeeAccount == null) LoadPayeeAccount();
                if (ibusPayeeAccount.iclbPaymentHistoryHeader.IsNull()) ibusPayeeAccount.LoadPaymentHistoryHeader();
                if (ibusPayeeAccount.iclbPaymentHistoryHeader.Count > 0)
                    return ibusPayeeAccount.iclbPaymentHistoryHeader.Any(i => (i.icdoPaymentHistoryHeader.status_value != busConstant.PaymentStatusCancelled && 
                                                                                i.icdoPaymentHistoryHeader.status_value != busConstant.HistoryHeaderStatusCancelPriorPayment && //PIR 18477
                                                                                i.icdoPaymentHistoryHeader.status_value != busConstant.HistoryHeaderStatusReceivableCreated)); //PIR - 17988
            }
            return false;
        }

        //PIR 17382 - Throw an error if status is changed to Approved and there is Restro Payment with Approved flag as 'N' or Null.
        public bool IsUnderPaymentNotApproved()
        {
            if (IsStatusApproved())
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.iclbRetroPayment == null)
                    ibusPayeeAccount.LoadRetroPayment();
                if (ibusPayeeAccount.iclbRetroPayment.Where(o =>
                    ((string.IsNullOrEmpty(o.icdoPayeeAccountRetroPayment.approved_flag) ||
                    (o.icdoPayeeAccountRetroPayment.approved_flag == busConstant.Flag_No)))).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
		
        // PIR 14288 - Added Validation for RTW employee if an overpayment exists with no associated Recovery, throw hard error.
        public bool IsBenefitOverpayment()
        {
            bool iblnIsBenefitOverPayment = false;
            DataTable idtAllPayeeAccount = Select("cdoPayeeAccountStatus.LoadPayeeAccountByPayeePerslinkId", new object[1] { icdoPayeeAccountStatus.payee_account_id });
            foreach (DataRow dr in idtAllPayeeAccount.Rows)
            {
                DataTable idtOverPaymentHdr = Select<cdoPaymentBenefitOverpaymentHeader>(new string[1] { enmPaymentRecovery.payee_account_id.ToString() }, new object[1] { dr["PAYEE_ACCOUNT_ID"] }, null, null);
                if (idtOverPaymentHdr.Rows.Count > 0)
                {
                    foreach (DataRow ldr in idtOverPaymentHdr.Rows)
                    {
                        DataTable ldtRecovery = Select<cdoPaymentRecovery>(new string[1] { enmPaymentRecovery.benefit_overpayment_id.ToString() },
                                                                   new object[1] { ldr["BENEFIT_OVERPAYMENT_ID"] },
                                                                   null, null);
                        if (ldtRecovery.Rows.Count == 0)
                        {
                            iblnIsBenefitOverPayment = true;
                        }
                        else
                        {
                            foreach (DataRow ldrRecovery in ldtRecovery.Rows)
                            {
                                if (Convert.ToString(ldrRecovery["STATUS_VALUE"]) == busConstant.RecoveryStatusPendingApproval)
                                {
                                    iblnIsBenefitOverPayment = true;
                                }
                            }
                        }
                    }
                }
            }
            return iblnIsBenefitOverPayment;
        }

        public bool IsNotCanceledRolloverExisting()
        {
            if (IsStatusApproved())
            {
                if (icdoPayeeAccountStatus.suppress_warnings_flag == busConstant.Flag_Yes)
                    return true;
                if (ibusPayeeAccount.IsNull()) LoadPayeeAccount();
                if (ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypePreRetirementDeath &&
                    ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypePostRetirementDeath)
                {
                    if (ibusPayeeAccount.ibusApplication.IsNull()) ibusPayeeAccount.LoadApplication();
                    if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_application_id > 0)
                    {
                        busWssBenApp lbusWssBenApp = new busWssBenApp();
                        if (lbusWssBenApp.FindWssBenAppByBenAppId(ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_application_id))
                        {
                            if (Select<cdoWssBenAppRolloverDetail>(new string[1] { enmWssBenAppRolloverDetail.wss_ben_app_id.ToString() },
                                                        new object[1] { lbusWssBenApp.icdoWssBenApp.wss_ben_app_id }, null, "wss_ben_app_id desc").Rows.Count > 0)
                            {
                                return SelectWithOperator<cdoPayeeAccountRolloverDetail>(new string[2] { enmPayeeAccountRolloverDetail.payee_account_id.ToString(),
                            enmPayeeAccountRolloverDetail.status_value.ToString()}, new string[2] { "=", "!=" },
                                                        new object[2] { ibusPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.PayeeAccountRolloverDetailStatusCancelled }, null).Rows.Count > 0;
                            }
                        }
                    }
                }
            }
            return true;
        }
        public bool IsThereAPriorPopupExisting()
        {
            if (ibusPayeeAccount.IsNull()) LoadPayeeAccount();
            if (ibusPayeeAccount.iclbRetroPayment.IsNull()) ibusPayeeAccount.LoadRetroPayment();
            return icdoPayeeAccountStatus.suppress_warnings_flag != busConstant.Flag_Yes 
                    && IsStatusApproved() 
                    && ibusPayeeAccount.iclbRetroPayment.Any(retropayment => 
                                        retropayment.icdoPayeeAccountRetroPayment.retro_payment_type_value ==
                                        busConstant.RetroPaymentTypePopupBenefits && 
                                        retropayment.icdoPayeeAccountRetroPayment.approved_flag == busConstant.Flag_Yes);
        }
        public bool IsAutoRefundAndAmountGreaterThan1000()
        {
            ibusPayeeAccount.LoadGrossAmount();
            if (ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionAutoRefund &&
               ibusPayeeAccount.idecGrossAmount > 1000.0M && icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundApproved)
                return true;
            else
                return false;
        }
        // PIR 19158  
        public bool IsStatusSuspendedToApproved()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbPayeeAccountStatus.IsNull())
                ibusPayeeAccount.LoadPayeeAccountStatus();
            if (ibusPayeeAccount.iclbPayeeAccountStatus.IsNotNull() && ibusPayeeAccount.iclbPayeeAccountStatus.Count > 0)
            {
                busPayeeAccountStatus lobjPreviousPayeeAccountStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
                lobjPreviousPayeeAccountStatus = ibusPayeeAccount.iclbPayeeAccountStatus.FirstOrDefault();

                if (lobjPreviousPayeeAccountStatus.IsNotNull() && ibusPayeeAccount.icdoPayeeAccount.rhic_amount > 0 &&
                    (lobjPreviousPayeeAccountStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusSuspended||
                    lobjPreviousPayeeAccountStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusSuspendedPSTD||
                    lobjPreviousPayeeAccountStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusDisabilitySuspended||
                    lobjPreviousPayeeAccountStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusSuspendedPERDT)
                    && (icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusApproved||
                    icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusApprovedDETH||
                    icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusDisabilityApproved||
                    icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusApprovedPSTD))
                {
                    return true;
                }
            }
            return false;   
        }
    }
}
