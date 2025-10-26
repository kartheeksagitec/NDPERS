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

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitPostRetirementDROCalculation : busBenefitDroCalculation
    {
        //Property to store gross monthly amount for the person
        public decimal idecGrossMonthlyAmount { get; set; }        

        //Property to store alt payee's monthly non taxable amount
        public decimal idecAltPayeeNonTaxableAmt { get; set; }

        //Property to store alt payee's monthly taxable amount
        public decimal idecAltPayeeTaxableAmt { get; set; }  
     
        //Property to store Member's Taxable Amount
        public decimal idecMemberTaxableAmount { get; set; }

        //Property to store Alternate Payee Starting Non taxable amount
        public decimal idecAltPayeeStartingNonTaxableAmount { get; set; }

        /// <summary>
        /// Function to return Gross monthly amount
        /// </summary>
        /// <param name="adtPaymentDate">Date</param>
        /// <param name="aobjPayeeAccount">Payee account</param>
        public void CalculateGrossMonthlyAmount(DateTime adtPaymentDate, busPayeeAccount aobjPayeeAccount)
        {
            if (aobjPayeeAccount != null && aobjPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
            {
                if (aobjPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                    aobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                if (aobjPayeeAccount.iclbPaymentItemType == null)
                    aobjPayeeAccount.LoadPaymentItemType();

                idecGrossMonthlyAmount = (from lobjPAPIT in aobjPayeeAccount.iclbPayeeAccountPaymentItemType
                                          join lobjPIT in aobjPayeeAccount.iclbPaymentItemType
                                            on lobjPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id
                                                equals lobjPIT.icdoPaymentItemType.payment_item_type_id
                                          where lobjPIT.icdoPaymentItemType.item_type_direction == 1
                                                    && lobjPAPIT.icdoPayeeAccountPaymentItemType.start_date <= adtPaymentDate
                                                    && (lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue ?
                                                        DateTime.MaxValue : lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date) >= adtPaymentDate
                                                    && string.IsNullOrEmpty(lobjPIT.icdoPaymentItemType.retro_payment_type_value)
                                                    && lobjPIT.icdoPaymentItemType.retro_special_payment_ind == 0
                                                    && lobjPIT.icdoPaymentItemType.plso_flag == busConstant.Flag_No
                                                    && lobjPIT.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes
                                                    && lobjPIT.icdoPaymentItemType.vendor_flag == busConstant.Flag_No
                                                    && lobjPIT.icdoPaymentItemType.item_usage_value == busConstant.PaymentItemTypeUsageMonthly
                                                    && lobjPIT.icdoPaymentItemType.check_type_value == busConstant.PaymentItemTypeCheckRegular
                                          select lobjPAPIT.icdoPayeeAccountPaymentItemType.amount).Sum();
            }
        }

        /// <summary>
        /// Method to calculate Total gross amount paid as of a given date
        /// </summary>
        /// <param name="adtPaymentDate">Date</param>
        /// <param name="aobjPayeeAccount">Payee Account</param>
        /// <param name="adecGrossAmount">Total Gross amount</param>
        public void GetGrossAmountUptoGivenDate(DateTime adtPaymentDate, busPayeeAccount aobjPayeeAccount, ref decimal adecGrossAmount)
        {
            if (aobjPayeeAccount != null && aobjPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
            {
                if (aobjPayeeAccount.iclbPaymentHistoryHeader == null)
                    aobjPayeeAccount.LoadPaymentHistoryHeader();
                if (aobjPayeeAccount.iclbPaymentHistoryDetail == null)
                    aobjPayeeAccount.LoadPaymentHistoryDetail();
                if (aobjPayeeAccount.iclbPaymentItemType == null)
                    aobjPayeeAccount.LoadPaymentItemType();

                adecGrossAmount = (from lobjPH in aobjPayeeAccount.iclbPaymentHistoryHeader
                                   join lobjPHID in aobjPayeeAccount.iclbPaymentHistoryDetail
                                     on lobjPH.icdoPaymentHistoryHeader.payment_history_header_id
                                        equals lobjPHID.icdoPaymentHistoryDetail.payment_history_header_id
                                   join lobjPIT in aobjPayeeAccount.iclbPaymentItemType
                                     on lobjPHID.icdoPaymentHistoryDetail.payment_item_type_id
                                        equals lobjPIT.icdoPaymentItemType.payment_item_type_id
                                   where lobjPIT.icdoPaymentItemType.item_type_direction == 1
                                            && lobjPH.icdoPaymentHistoryHeader.payment_date <= adtPaymentDate
                                   select lobjPHID.icdoPaymentHistoryDetail.amount).Sum();
            }                                
        }        

        /// <summary>
        /// Method to calculate Total Non Taxable amount paid till a given date
        /// </summary>
        /// <param name="adtPaymentDate">Date</param>
        /// <param name="aobjPayeeAccount">Payee Account</param>
        /// <returns>Total Non taxable amount</returns>
        public decimal CalculateTotalNonTaxableAmountPaid(DateTime adtPaymentDate, busPayeeAccount aobjPayeeAccount)
        {
            decimal ldecNonTaxableAmount = 0.0M;
            if (aobjPayeeAccount != null && aobjPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
            {
                if (aobjPayeeAccount.iclbPaymentHistoryHeader == null)
                    aobjPayeeAccount.LoadPaymentHistoryHeader();
                if (aobjPayeeAccount.iclbPaymentHistoryDetail == null)
                    aobjPayeeAccount.LoadPaymentHistoryDetail();
                if (aobjPayeeAccount.iclbPaymentItemType == null)
                    aobjPayeeAccount.LoadPaymentItemType();

                ldecNonTaxableAmount = (from lobjPH in aobjPayeeAccount.iclbPaymentHistoryHeader
                                        join lobjPHID in aobjPayeeAccount.iclbPaymentHistoryDetail
                                          on lobjPH.icdoPaymentHistoryHeader.payment_history_header_id
                                             equals lobjPHID.icdoPaymentHistoryDetail.payment_history_header_id
                                        join lobjPIT in aobjPayeeAccount.iclbPaymentItemType
                                          on lobjPHID.icdoPaymentHistoryDetail.payment_item_type_id
                                             equals lobjPIT.icdoPaymentItemType.payment_item_type_id
                                        where lobjPIT.icdoPaymentItemType.item_type_direction == 1
                                                 && lobjPIT.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_No
                                                 && lobjPH.icdoPaymentHistoryHeader.payment_date <= adtPaymentDate
                                        select lobjPHID.icdoPaymentHistoryDetail.amount).Sum();
            }
            return ldecNonTaxableAmount;
        }

        /// <summary>
        /// Method to calculate Non taxable begining balance for Member account
        /// </summary>
        /// <param name="aobjMemberPayeeAccount">Member Payee Account</param>
        /// <param name="aobjAltPayeeAccount">Alternate Payee Account</param>
        public void CalculateNonTaxableBeginingBalanceForMember(busPayeeAccount aobjMemberPayeeAccount, busPayeeAccount aobjAltPayeeAccount)
        {
            aobjMemberPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance =
                aobjMemberPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance > aobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance ?
                aobjMemberPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance - aobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance :
                0;
        }

        /// <summary>
        /// Method to calculate Non Taxable Begining balance for Alternate Payee account
        /// </summary>
        /// <param name="aobjMemberPayeeAccount">Member Payee Account</param>
        /// <param name="aobjAltPayeeAccount">Alternate Payee Account</param>
        /// <param name="adecMonthlyBenefitPer">Monthly benefit percentage</param>
        /// <param name="adecNonTaxableAmount">Non taxable amount paid</param>
        public void CalculateNonTaxableBeginingBalanceForAltPayee(busPayeeAccount aobjMemberPayeeAccount, busPayeeAccount aobjAltPayeeAccount,
                                                                decimal adecMonthlyBenefitPer, decimal adecNonTaxableAmount)
        {
            aobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance = icdoBenefitDroCalculation.starting_non_taxable;
            idecAltPayeeStartingNonTaxableAmount = aobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance;
        }

        /// <summary>
        /// Method to calculate Monthly taxable amount for Alternate Payee
        /// </summary>
        /// <param name="aobjAltPayeeAccount">Alternate Payee account</param>
        public void CalculateMonthlyNonTaxableAmountForAltPayee(busPayeeAccount aobjAltPayeeAccount)
        {
            int lintAge = 0, lintMonths = 0, lintMemberAgeMonths = 0;
            decimal ldecMonthAndYear = 0.0M;
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(aobjAltPayeeAccount.icdoPayeeAccount.payee_perslink_id))
            {
                busPersonBase.CalculateAge(lobjPerson.icdoPerson.date_of_birth.AddMonths(1), icdoBenefitDroCalculation.benefit_begin_date, ref lintMonths, ref ldecMonthAndYear,
                                            0, ref lintAge, ref lintMemberAgeMonths);
                busBenefitCalculation lobjBenefitCalculation = new busBenefitCalculation();
                idecAltPayeeNonTaxableAmt = aobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance /
                                                lobjBenefitCalculation.GetNumberofPayments(busConstant.ExclusionCalcPaymentTypeSingleLife, lintAge, 
                                                icdoBenefitDroCalculation.benefit_begin_date);
            }
        }

        /// <summary>
        /// Method to update minimum guarantee amount for member
        /// </summary>
        /// <param name="adtPaymentDate">Date</param>
        /// <param name="aobjMemPayeeAccount">Member Payee Account</param>
        /// <param name="aobjAltPayeeAccount">Alternate Payee Account</param>
        /// <param name="adecMonthlyBenefitPer">Monthly benefit Percentage</param>
        /// <param name="iblnMember">Member check flag</param>
        public void UpdateMinimumGuaranteeForMember(DateTime adtPaymentDate, busPayeeAccount aobjMemPayeeAccount, busPayeeAccount aobjAltPayeeAccount,
            decimal adecMonthlyBenefitPer, bool iblnMember)
        {
            decimal ldecGrossAmount = 0.0M;
            if (iblnMember)
            {                
                //TODO take amount from new col.
                aobjAltPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount = icdoBenefitDroCalculation.minimum_guarantee;             
            }
            else
            {
                //function to calculate total gross amount paid by alternate payee
                GetGrossAmountUptoGivenDate(adtPaymentDate, aobjAltPayeeAccount, ref ldecGrossAmount);
                //step to update alt payee's minimum guarantee amount
                aobjAltPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount -=
                    aobjAltPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount > ldecGrossAmount ?
                    ldecGrossAmount : aobjAltPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount;
                //step to update member's minimum guarantee amount
                aobjMemPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount += aobjAltPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount;           
            }
        }

        /// <summary>
        /// Method to calculate and create PAPI
        /// </summary>
        /// <param name="abusMemPayeeAccount">BAO Payee account</param>
        /// <param name="abusAltPayeeAccount">AP payee account</param>
        /// <param name="adecGrossMonthlyBenefit">Gross monthly benefit</param>
        /// <param name="adecMonthlyBenefitAmount">Monthly benefit amount</param>
        /// <param name="adecPercentage">Monthly benefit percentage</param>
        /// <param name="ablnMember">Member check flag</param>
        public void CalculateAndCreatePAPIT(busPayeeAccount abusMemPayeeAccount, busPayeeAccount abusAltPayeeAccount, decimal adecGrossMonthlyBenefit,
            decimal adecDROApplicationMonthlyBenefitAmount, decimal adecMonthlyBenefitAmount, decimal adecPercentage, bool ablnMember)
        {
            decimal ldecPreviousMemberTaxableAmount = 0.0M, ldecMemberTaxableAmount = 0.0M;

            abusMemPayeeAccount.LoadPayeeAccountPaymentItemType();
            abusMemPayeeAccount.LoadPaymentItemType();

            IEnumerable<busPayeeAccountPaymentItemType> lenuPAPIT =
                    abusMemPayeeAccount.iclbPayeeAccountPaymentItemType.Where(o=>o.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                    .Join(abusMemPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes),
                    o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id,
                    l => l.icdoPaymentItemType.payment_item_type_id,
                    (o, l) =>  o );            

            ldecPreviousMemberTaxableAmount = lenuPAPIT.Sum(o => o.icdoPayeeAccountPaymentItemType.amount);
            
            decimal ldecGrossReductionAmount = 0.0M;
            ldecGrossReductionAmount = adecMonthlyBenefitAmount > 0.0M ? adecMonthlyBenefitAmount : (adecGrossMonthlyBenefit * adecPercentage / 100);
            
            //On approval of DRO Calculation
            if (ablnMember)
            {
                //Steps to calculate new taxable amount for member
                ldecMemberTaxableAmount = ldecPreviousMemberTaxableAmount;
                //calculated non taxable amt is subtracted from monthly benefit amt in Calculation to get monthly taxable amount
                idecAltPayeeTaxableAmt = ldecGrossReductionAmount - idecAltPayeeNonTaxableAmt;
            }
            //On Nullify of DRO Application
            else
            {
                //Steps to calculate new taxable amount for member
                ldecMemberTaxableAmount = ldecPreviousMemberTaxableAmount + adecDROApplicationMonthlyBenefitAmount;
            }
            CreatePAPIProportionately(abusMemPayeeAccount, abusAltPayeeAccount, lenuPAPIT, ldecMemberTaxableAmount, ldecPreviousMemberTaxableAmount, ablnMember);

            idecMemberTaxableAmount = ldecMemberTaxableAmount;

            if (!ablnMember)
            {
                //Function to calculate new tax
                abusMemPayeeAccount.CalculateAdjustmentTax(false);

                //Checking whether net amount is below zero and making payee account to review status
                abusMemPayeeAccount.LoadBenefitAmount();
                if (abusMemPayeeAccount.idecBenefitAmount < 0)
                {
                    if (abusMemPayeeAccount.ibusSoftErrors == null)
                        abusMemPayeeAccount.LoadErrors();
                    abusMemPayeeAccount.iblnClearSoftErrors = false;
                    abusMemPayeeAccount.ibusSoftErrors.iblnClearError = false;
                    abusMemPayeeAccount.CreateReviewPayeeAccountStatus();
                    abusMemPayeeAccount.iblnInvalidNetAmountIndicator = true;
                    abusMemPayeeAccount.ValidateSoftErrors();
                    abusMemPayeeAccount.UpdateValidateStatus();
                }
            }
        }

        /// <summary>
        /// Create PAPI proportionately
        /// </summary>
        /// <param name="abusMemPayeeAccount">Member Payee Account</param>
        /// <param name="abusAltPayeeAccount">Alternate Payee Account</param>
        /// <param name="adecMemberTaxableAmount">Member Monthly taxable amount</param>
        /// <param name="aobjPAPIT">Payee Account Payment Item Type bus obj for Taxable item</param>
        private void CreatePAPIProportionately(busPayeeAccount abusMemPayeeAccount, busPayeeAccount abusAltPayeeAccount,
            IEnumerable<busPayeeAccountPaymentItemType> aenuPAPIT, decimal adecMemberTaxableAmount, decimal adecPreviousMemberTaxableAmount, bool ablnMember)
        {
            if (abusAltPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                abusAltPayeeAccount.LoadNexBenefitPaymentDate();
            if (abusMemPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                abusMemPayeeAccount.LoadNexBenefitPaymentDate();
            DateTime ldtStartDate = abusMemPayeeAccount.idtNextBenefitPaymentDate > abusAltPayeeAccount.idtNextBenefitPaymentDate ?
                abusMemPayeeAccount.idtNextBenefitPaymentDate : abusAltPayeeAccount.idtNextBenefitPaymentDate;
            foreach (busPayeeAccountPaymentItemType lobjPAPI in aenuPAPIT)
            {
                decimal ldecRatio = lobjPAPI.icdoPayeeAccountPaymentItemType.amount / adecPreviousMemberTaxableAmount;
                decimal ldecMemberAmount = adecMemberTaxableAmount * ldecRatio;
                decimal ldecAPAmount = idecAltPayeeTaxableAmt * ldecRatio;
                
                //Posting into PAPIT for alternate payee
                if (ablnMember)
                {
                    abusAltPayeeAccount.CreatePayeeAccountPaymentItemType(lobjPAPI.icdoPayeeAccountPaymentItemType.payment_item_type_id, ldecAPAmount,
                        string.Empty, 0, ldtStartDate, DateTime.MinValue, false);
                }
                else
                {
                    //Posting into PAPIT for member
                    abusMemPayeeAccount.CreatePayeeAccountPaymentItemType(lobjPAPI.icdoPayeeAccountPaymentItemType.payment_item_type_id, ldecMemberAmount,
                        string.Empty, 0, ldtStartDate, DateTime.MinValue, false);
                }
            }
        }

        /// <summary>
        /// Method to post minimum guarantee and non taxable amount of Alternate payee to retirement contribution of Member
        /// </summary>
        /// <param name="adtNullifyDate">Nullfy Date</param>
        /// <param name="aobjAltPayeeAccount">Alternate Payee Account</param>
        /// <param name="aobjPersonAccount">Member Person Account</param>
        public void CalculateAndPostRetirementContributionAmounts(DateTime adtNullifyDate, busPayeeAccount aobjAltPayeeAccount, busPersonAccount aobjPersonAccount)
        {
            decimal ldecRatio = 0.0M, ldecEEPreTaxAmount = 0.0M, ldecEEPostTaxAmount = 0.0M, ldecEEERPickupAmount = 0.0M, ldecERVestedAmount = 0.0M,
                ldecInterestAmount = 0.0M, ldecCapitalGainAmount = 0.0M, ldecGrossAmount = 0.0M, ldecNonTaxableAmount = 0.0M;

            GetGrossAmountUptoGivenDate(adtNullifyDate, aobjAltPayeeAccount, ref ldecGrossAmount);
            ldecNonTaxableAmount = CalculateTotalNonTaxableAmountPaid(adtNullifyDate, aobjAltPayeeAccount);

            if (aobjAltPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount != 0.0M && ldecGrossAmount < aobjAltPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount)
            {
                ldecRatio = ldecGrossAmount / aobjAltPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount;
                ldecRatio = (ldecRatio == 0.0M ? 1 : ldecRatio);
                ldecRatio = 1 - ldecRatio;
            }
            if (ldecRatio > 0)
            {
                ldecEEPreTaxAmount = icdoBenefitDroCalculation.ee_pre_tax_amount * ldecRatio;
                ldecEEERPickupAmount = icdoBenefitDroCalculation.ee_er_pickup_amount * ldecRatio;
                ldecERVestedAmount = icdoBenefitDroCalculation.er_vested_amount * ldecRatio;
                ldecInterestAmount = (icdoBenefitDroCalculation.interest_amount + icdoBenefitDroCalculation.additional_interest_amount) * ldecRatio;
                ldecCapitalGainAmount = icdoBenefitDroCalculation.capital_gain * ldecRatio;
            }

            ldecRatio = 0.0M;
            if (aobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance != 0.0M)
            {
                ldecRatio = ldecNonTaxableAmount / aobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance;
                ldecRatio = (ldecRatio == 0.0M ? 1 : ldecRatio);
                ldecRatio = 1 - ldecRatio;
            }
            if (ldecRatio > 0)
            {
                ldecEEPostTaxAmount = icdoBenefitDroCalculation.ee_post_tax_amount * ldecRatio;
            }

            //Updating Capital Gain
            aobjPersonAccount.UpdateCapitalGain(ldecCapitalGainAmount, true);
            //Posting record into Retirement contribution
            if (ldecEEPostTaxAmount > 0 || ldecEEPreTaxAmount > 0 || ldecEEERPickupAmount > 0 || ldecERVestedAmount > 0 || ldecInterestAmount > 0)
            {
                aobjPersonAccount.PostRetirementContribution(busConstant.SubSystemValueAdjustment, 0, adtNullifyDate, adtNullifyDate, adtNullifyDate.Month,
                    adtNullifyDate.Year, 0, busConstant.TransactionTypeAlternatePayeePayment, 0.0M, 0.0M, ldecEEPostTaxAmount, 0.0M, ldecEEPreTaxAmount, 0.0M,
                    0.0M, ldecEEERPickupAmount, ldecERVestedAmount, ldecInterestAmount, 0.0M, 0.0M);
            }
        }

        //Property to contain balance minimum guarantee amount
        public decimal idecBalanceMinimumGuarantee { get; set; }
        /// <summary>
        /// Method to load Balance minimum guarantee amount
        /// </summary>
        /// <param name="abusPayeeAccount">Payee acct bus. object</param>
        /// <param name="adtPaymentDate">Payment date</param>
        public void LoadBalanceMinimumGuarantee(busPayeeAccount abusPayeeAccount, DateTime adtPaymentDate)
        {
            decimal ldecGrossAmtPaid = 0.0M;
            GetGrossAmountUptoGivenDate(adtPaymentDate, abusPayeeAccount, ref ldecGrossAmtPaid);
            idecBalanceMinimumGuarantee = abusPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount - ldecGrossAmtPaid;
        }

        //Property to contain balance non taxable amount
        public decimal idecBalanceNonTaxableAmount { get; set; }
        /// <summary>
        /// Method to load balance non taxable amount
        /// </summary>
        /// <param name="abusPayeeAccount">payee acct. bus. object</param>
        /// <param name="adtPaymentDate">Payment date</param>
        public void LoadBalanceNonTaxableAmount(busPayeeAccount abusPayeeAccount, DateTime adtPaymentDate)
        {
            idecBalanceNonTaxableAmount = abusPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance -
                CalculateTotalNonTaxableAmountPaid(adtPaymentDate, abusPayeeAccount);
        }

        //prod pir 6420
        //--start--//

        /// <summary>
        /// Method to calculate Non taxable begining balance for Member account
        /// </summary>
        /// <param name="aobjMemberPayeeAccount"></param>
        /// <param name="adecMonthlyBenefitPer"></param>
        /// <param name="adecNonTaxableAmount"></param>
        internal void CalculateNonTaxableBeginingBalanceForMember(busPayeeAccount aobjMemberPayeeAccount, decimal adecMonthlyBenefitPer, decimal adecNonTaxableAmount)
        {
            decimal ldecAltPayeeStartingNonTaxableAmount =
               (aobjMemberPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance > adecNonTaxableAmount ?
               (aobjMemberPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance - adecNonTaxableAmount) :
               0) * (adecMonthlyBenefitPer / 100);

            icdoBenefitDroCalculation.starting_non_taxable = ldecAltPayeeStartingNonTaxableAmount;

            aobjMemberPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance =
                aobjMemberPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance > ldecAltPayeeStartingNonTaxableAmount ?
                aobjMemberPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance - ldecAltPayeeStartingNonTaxableAmount : 0;
        }

        /// <summary>
        /// Method to calculate and create PAPI
        /// </summary>
        /// <param name="abusMemberPayeeAccount"></param>
        /// <param name="adecGrossMonthlyBenefit"></param>
        /// <param name="adecDROApplicationMonthlyBenefitAmount"></param>
        /// <param name="adecMonthlyBenefitAmount"></param>
        /// <param name="adecPercentage"></param>
        internal void CalculateAndCreatePAPIT(busPayeeAccount abusMemberPayeeAccount, decimal adecGrossMonthlyBenefit,
            decimal adecDROApplicationMonthlyBenefitAmount, decimal adecMonthlyBenefitAmount, decimal adecPercentage)
        {
            decimal ldecPreviousMemberTaxableAmount = 0.0M, ldecMemberTaxableAmount = 0.0M;

            abusMemberPayeeAccount.LoadPayeeAccountPaymentItemType();
            abusMemberPayeeAccount.LoadPaymentItemType();

            IEnumerable<busPayeeAccountPaymentItemType> lenuPAPIT =
                    abusMemberPayeeAccount.iclbPayeeAccountPaymentItemType.Where(o => o.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                    .Join(abusMemberPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes),
                    o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id,
                    l => l.icdoPaymentItemType.payment_item_type_id,
                    (o, l) => o);

            ldecPreviousMemberTaxableAmount = lenuPAPIT.Sum(o => o.icdoPayeeAccountPaymentItemType.amount);

            decimal ldecGrossReductionAmount = 0.0M;
            ldecGrossReductionAmount = adecMonthlyBenefitAmount > 0.0M ? adecMonthlyBenefitAmount : (adecGrossMonthlyBenefit * adecPercentage / 100);
            
            //Steps to calculate new taxable amount for member
            ldecMemberTaxableAmount = ldecPreviousMemberTaxableAmount > adecDROApplicationMonthlyBenefitAmount ?
                ldecPreviousMemberTaxableAmount - adecDROApplicationMonthlyBenefitAmount : 0;
                        
            CreatePAPIProportionately(abusMemberPayeeAccount, lenuPAPIT, ldecMemberTaxableAmount, ldecPreviousMemberTaxableAmount);

            idecMemberTaxableAmount = ldecMemberTaxableAmount;

            //Function to calculate new tax
            abusMemberPayeeAccount.CalculateAdjustmentTax(false);

            //Checking whether net amount is below zero and making payee account to review status
            abusMemberPayeeAccount.LoadBenefitAmount();
            if (abusMemberPayeeAccount.idecBenefitAmount < 0)
            {
                if (abusMemberPayeeAccount.ibusSoftErrors == null)
                    abusMemberPayeeAccount.LoadErrors();
                abusMemberPayeeAccount.iblnClearSoftErrors = false;
                abusMemberPayeeAccount.ibusSoftErrors.iblnClearError = false;
                abusMemberPayeeAccount.CreateReviewPayeeAccountStatus();
                abusMemberPayeeAccount.iblnInvalidNetAmountIndicator = true;
                abusMemberPayeeAccount.ValidateSoftErrors();
                abusMemberPayeeAccount.UpdateValidateStatus();
            }
        }

        /// <summary>
        /// Create PAPI proportionately
        /// </summary>
        /// <param name="abusMemPayeeAccount"></param>
        /// <param name="aenuPAPIT"></param>
        /// <param name="adecMemberTaxableAmount"></param>
        /// <param name="adecPreviousMemberTaxableAmount"></param>
        private void CreatePAPIProportionately(busPayeeAccount abusMemPayeeAccount, IEnumerable<busPayeeAccountPaymentItemType> aenuPAPIT, 
            decimal adecMemberTaxableAmount, decimal adecPreviousMemberTaxableAmount)
        {
            if (abusMemPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                abusMemPayeeAccount.LoadNexBenefitPaymentDate();
            DateTime ldtStartDate = abusMemPayeeAccount.idtNextBenefitPaymentDate;

            foreach (busPayeeAccountPaymentItemType lobjPAPI in aenuPAPIT)
            {
                decimal ldecRatio = lobjPAPI.icdoPayeeAccountPaymentItemType.amount / adecPreviousMemberTaxableAmount;
                decimal ldecMemberAmount = adecMemberTaxableAmount * ldecRatio;
                
                //Posting into PAPIT for member
                abusMemPayeeAccount.CreatePayeeAccountPaymentItemType(lobjPAPI.icdoPayeeAccountPaymentItemType.payment_item_type_id, ldecMemberAmount,
                    string.Empty, 0, ldtStartDate, DateTime.MinValue, false);                
            }
        }

        /// <summary>
        /// Method to update minimum guarantee amount for member
        /// </summary>
        /// <param name="adtPaymentDate"></param>
        /// <param name="aobjMemPayeeAccount"></param>
        /// <param name="adecMonthlyBenefitPer"></param>
        internal void UpdateMinimumGuaranteeForMember(DateTime adtPaymentDate, busPayeeAccount aobjMemPayeeAccount, decimal adecMonthlyBenefitPer)
        {
            decimal ldecGrossAmount = 0.0M, ldecMinimumGuaranteeReduction = 0.0M;
            
            //function to calculate total gross amount paid by member
            GetGrossAmountUptoGivenDate(adtPaymentDate, aobjMemPayeeAccount, ref ldecGrossAmount);
            //step to update member's minimum guarantee amount
            ldecMinimumGuaranteeReduction = aobjMemPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount > ldecGrossAmount ?
                (aobjMemPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount - ldecGrossAmount) * adecMonthlyBenefitPer / 100 :
                aobjMemPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount;
            icdoBenefitDroCalculation.minimum_guarantee = ldecMinimumGuaranteeReduction;
            aobjMemPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount -= ldecMinimumGuaranteeReduction;           
        }

        //--end--//
    }
}
