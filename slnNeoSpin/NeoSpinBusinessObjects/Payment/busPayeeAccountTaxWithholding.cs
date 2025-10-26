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
using System.Linq;
using System.Linq.Expressions;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPayeeAccountTaxWithholding : busPayeeAccountTaxWithholdingGen
    {
        public busTaxRefConfig ibusTaxRefConfig { get; set; }
        public decimal idecTaxAmount { get; set; } //pir 8618
        public bool iblnIsCalculateButtonClicked = false;
        //If Start Date is less than  next benefit payment date or not first of month,then throw an error
        public bool AreFieldsReadOnly()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if ((icdoPayeeAccountTaxWithholding.ienuObjectState != ObjectState.Insert) && (icdoPayeeAccountTaxWithholding.start_date != DateTime.MinValue)
                && (icdoPayeeAccountTaxWithholding.start_date < ibusPayeeAccount.idtNextBenefitPaymentDate))
            {
                return true;
            }
            return false;
        }
        //20% federal tax mandatory for member or spouse for refunds
        public bool IsTaxMandatoryForMemberOrSpouse()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if ((icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax &&
                icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum &&
                icdoPayeeAccountTaxWithholding.tax_option_value != busConstant.FedTaxOptionFederalTaxwithheld) &&
                (ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember ||
                ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipSpouse))
            {
                return true;
            }
            return false;
        }
        public bool IsBenefitOptionTransfers()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            if (ibusPayeeAccount.ibusApplication.IsBenefitOptionTransfers())
                return true;
            return false;
        }
        //start date should be next payment date and first of month
        public bool IsStartdateNotValid()
        {
            if ((icdoPayeeAccountTaxWithholding.start_date != DateTime.MinValue) && (!AreFieldsReadOnly()))
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                DateTime ldtStartDate = new DateTime(ibusPayeeAccount.idtNextBenefitPaymentDate.Year, ibusPayeeAccount.idtNextBenefitPaymentDate.Month, 1);
                if (icdoPayeeAccountTaxWithholding.start_date < ldtStartDate)
                {
                    return true;
                }
                else if (icdoPayeeAccountTaxWithholding.start_date >= ldtStartDate)
                {
                    if (icdoPayeeAccountTaxWithholding.start_date.Day != 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //Check whether tax record  exist for given date
        public bool IsTaxRecordExist()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbTaxWithholingHistory == null)
                ibusPayeeAccount.LoadTaxWithHoldingHistory();
            foreach (busPayeeAccountTaxWithholding lobjTaxWithHolding in ibusPayeeAccount.iclbTaxWithholingHistory)
            {
                if ((lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id != icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id) &&
                    (lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.tax_identifier_value == icdoPayeeAccountTaxWithholding.tax_identifier_value) &&
                    (lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == icdoPayeeAccountTaxWithholding.benefit_distribution_type_value) &&
                    (busGlobalFunctions.CheckDateOverlapping(icdoPayeeAccountTaxWithholding.start_date,
                    lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.start_date, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.end_date)))
                {
                    return true;
                }
            }
            return false;
        }
        //If  End Date is less than or equal to next benefit payment date or one day before next benefit payment date ,then throw an error 
        public bool IsEnddateNotValid()
        {
            if ((icdoPayeeAccountTaxWithholding.start_date != DateTime.MinValue) && (icdoPayeeAccountTaxWithholding.end_date != DateTime.MinValue))
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                DateTime ldtBatchDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
                if (icdoPayeeAccountTaxWithholding.end_date < ldtBatchDate.AddDays(-1))
                {
                    return true;
                }
                else if (icdoPayeeAccountTaxWithholding.end_date >= ldtBatchDate.AddDays(-1))
                {
                    DateTime ldtLastDateOfMonth = busGlobalFunctions.GetLastDayOfMonth(icdoPayeeAccountTaxWithholding.end_date);
                    if (ldtLastDateOfMonth.Day != icdoPayeeAccountTaxWithholding.end_date.Day)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            if (IsUpdateModeEndDateNotNull())
            {
                UpdateEndateForOldTax();
            }
            //If there is any Change Tax withholding information ,Change the payee account status to review
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();

            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();

            if (ibusPayeeAccount.ibusSoftErrors == null)
                ibusPayeeAccount.LoadErrors();
            ibusPayeeAccount.iblnClearSoftErrors = false;
            ibusPayeeAccount.ibusSoftErrors.iblnClearError = false;

            if (ibusPayeeAccount.idecBenefitAmount == 0.0m)
                ibusPayeeAccount.LoadBenefitAmount();

            if (ibusPayeeAccount.idecBenefitAmount < 0.0m)
                ibusPayeeAccount.iblnInvalidNetAmountIndicator = true;
            // pir 8618
            if (!ibusPayeeAccount.iblnIsFromMSS)
                ibusPayeeAccount.CreateReviewPayeeAccountStatus();
            ibusPayeeAccount.iblnTaxwithholdingInfoChangeIndicator = true;
            ibusPayeeAccount.ValidateSoftErrors();
            ibusPayeeAccount.UpdateValidateStatus();
            ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
            ibusPayeeAccount.LoadFedTaxWithHoldingInfo();

            LoadTaxWithHoldingTaxItems();

            //We must reload this collection from DB (Fix : Multiple save on same screen)
            ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            if (icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue)
            {
                //Delete old items
                DeleteTaxItems();

                //Calculate tax
                CalculateTax(true);

                //Post into Payee account payment item type table and tax withholidng detail table
                PostTaxAmount(false);
                //pir 8618
                if (ibusPayeeAccount.iblnIsFromMSS)
                {
                    ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                    ibusPayeeAccount.LoadBenefitAmount();

                    if (ibusPayeeAccount.idecBenefitAmount < 0.0m)
                        ibusPayeeAccount.CreateReviewPayeeAccountStatus();
                }
            }
            ibusPayeeAccount.LoadTaxWithHoldingHistory();
        }

        public void DeleteOldItems()
        {
            if (iclbTaxWithHoldingTaxItems == null)
                LoadTaxWithHoldingTaxItems();
            foreach (busPayeeAccountTaxWithholdingItemDetail lobjTaxWithholdingDetail in iclbTaxWithHoldingTaxItems)
            {
                if (lobjTaxWithholdingDetail.ibusPayeeAccountPaymentItemType == null)
                    lobjTaxWithholdingDetail.LoadPayeeAccountPaymentItemType();
                if (lobjTaxWithholdingDetail.ibusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id > 0)
                {
                    lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.Delete();
                }
            }

        }
        public void PostTaxAmount(bool ablnAfterRollover)
        {
            bool lbnCheckUpdate = true;
            int lintVendorOrgId = 0;
            if (ablnAfterRollover)
            {
                lbnCheckUpdate = false;
            }
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();

            if (iclbTaxWithHoldingTaxItems != null)
            {
                foreach (busPayeeAccountTaxWithholdingItemDetail lobjTaxWithholdingDetail in iclbTaxWithHoldingTaxItems)
                {
                    DateTime ldtStartDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
                    if (ibusPayeeAccount.idtStatusEffectiveDate != DateTime.MinValue && ibusPayeeAccount.iblnIsColaOrAdhocIncreaseTaxCalculation)
                        ldtStartDate = ibusPayeeAccount.idtStatusEffectiveDate;
                    //prod pir 6236 : need to set taxwithholding start date if it is in future
                    if (icdoPayeeAccountTaxWithholding.start_date > ldtStartDate)
                        ldtStartDate = icdoPayeeAccountTaxWithholding.start_date;
                    if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                    {
                        lintVendorOrgId = busPayeeAccountHelper.GetFedTaxVendorID();
                    }
                    else
                    {
                        lintVendorOrgId = busPayeeAccountHelper.GetStateTaxVendorID();
                    }
                    //UpdateEndateForOldTax(lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id);
                    int lintPayeeAccountItemId = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id,
                        lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.amount, string.Empty, lintVendorOrgId, ldtStartDate, icdoPayeeAccountTaxWithholding.end_date, lbnCheckUpdate);
                    if (lintPayeeAccountItemId > 0)
                    {
                        lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id = lintPayeeAccountItemId;
                        lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.Insert();
                    } // while relcalculating tax for PLSO and refund if the recalculated tax is zero ,delete the old items(so that tax becomes zero)
                    else if (lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.amount == 0.0M)
                    {
                        ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                        if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                            lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id &&
                            o.icdoPayeeAccountPaymentItemType.start_date > ibusPayeeAccount.idtNextBenefitPaymentDate).Any())
                        {
                            // DeleteTaxItems();
                        }
                    }
                    lobjTaxWithholdingDetail.LoadPayeeAccountPaymentItemType();
                }
            }
        }
        // PIR 17060 - Roth Rollover Taxes Implementation
        public void PostTaxAmountForRothRollover(string astrTaxIdentifierValue, int aintPaymentItemTypeId, decimal PAPITAmount, bool ablnDeleteIndicator)
        {
            int lintVendorOrgId = 0;
            DateTime? adtEndDt = null;
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            DateTime ldtStartDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
            if (astrTaxIdentifierValue == busConstant.PayeeAccountTaxIdentifierStateTax)
            {
                lintVendorOrgId = busPayeeAccountHelper.GetStateTaxVendorID();
            }
            else
            {
                lintVendorOrgId = busPayeeAccountHelper.GetFedTaxVendorID();
            }
            int lintPayeeAccountItemId = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(aintPaymentItemTypeId,
                PAPITAmount, string.Empty, lintVendorOrgId, ldtStartDate, Convert.ToDateTime(adtEndDt), false);
            if (iclbPayeeAccountRolloverDetail.IsNotNull() && !(ablnDeleteIndicator) && lintPayeeAccountItemId > 0 && ibusPayeeAccount.iblnCreateFlag)
            {
                foreach (busPayeeAccountRolloverItemDetail lbusRolloverItemDetail in iclbPayeeAccountRolloverDetail)
                {
                    lbusRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_payment_item_type_id = lintPayeeAccountItemId;
                    lbusRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.Insert();
                }
            }
        }



        //Post tax amounts for reissued amounts
        public void PostTaxAmountForReissuedAmount()
        {
            int lintVendorOrgId = 0;
            if (iclbTaxWithHoldingTaxItems != null)
            {
                foreach (busPayeeAccountTaxWithholdingItemDetail lobjTaxWithholdingDetail in iclbTaxWithHoldingTaxItems)
                {
                    if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                    {
                        lintVendorOrgId = busPayeeAccountHelper.GetFedTaxVendorID();
                    }
                    else
                    {
                        lintVendorOrgId = busPayeeAccountHelper.GetStateTaxVendorID();
                    }
                    //UpdateEndateForOldTax(lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id);
                    int lintPayeeAccountItemId = CreateReissuePapitItems(lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id,
                        lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.amount, lintVendorOrgId);
                    if (lintPayeeAccountItemId > 0)
                    {
                        lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id = lintPayeeAccountItemId;
                        lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.Insert();
                    }
                    lobjTaxWithholdingDetail.LoadPayeeAccountPaymentItemType();
                }
            }
        }
        //Create papit items for taxes calculated for the reissued taxable amount
        private int CreateReissuePapitItems(int aintItemid, decimal adecAmount, int aintVendorOrgID)
        {
            busPayeeAccountPaymentItemType lobjPapitItem = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            lobjPapitItem.icdoPayeeAccountPaymentItemType.payment_item_type_id = aintItemid;
            lobjPapitItem.icdoPayeeAccountPaymentItemType.payee_account_id = ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
            lobjPapitItem.icdoPayeeAccountPaymentItemType.start_date = DateTime.Today;
            lobjPapitItem.icdoPayeeAccountPaymentItemType.amount = adecAmount;
            lobjPapitItem.icdoPayeeAccountPaymentItemType.vendor_org_id = aintVendorOrgID;
            lobjPapitItem.icdoPayeeAccountPaymentItemType.reissue_item_flag = busConstant.Flag_Yes;
            lobjPapitItem.icdoPayeeAccountPaymentItemType.Insert();
            return lobjPapitItem.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
        }
        public ArrayList btnCalculateTax()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            int lintNoOfallowances = 0;
            if (!string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.no_of_tax_allowance) && int.TryParse(icdoPayeeAccountTaxWithholding.no_of_tax_allowance, out lintNoOfallowances) &&
                icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
            {
                icdoPayeeAccountTaxWithholding.tax_allowance = lintNoOfallowances;
            }
            iblnIsCalculateButtonClicked = true;

            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                iblnIsCalculateButtonClicked = true;
                CalculateTax(true, false);
                iblnIsCalculateButtonClicked = false;
                alReturn.Add(this);
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }


        public void CalculateTax(bool ablnRegularTax, bool ablnModifyPapits = true)
        {
            //If the Benefit type is Non Refund , then Call the method  busPayeeAccountHelper.CalculateFedOrStateTax() to 
            //calculate the federal or state tax based on the tax identifier
            if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
            {
                if (!ablnRegularTax)
                {
                    icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option = icdoPayeeAccountTaxWithholding.tax_option_value;
                }
                CalculateTaxForMonthlyBenefit(ablnRegularTax, ablnModifyPapits); //pir 8457
            }
            //If the Benefit type is Refund or Reitiement PLSO , then Call the method  busPayeeAccountHelper.CalculateFlatTax() to 
            //calculate the Flat tax based on the tax identifier
            else if ((icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO) ||
                (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum) ||
                (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD))
            {
                if (!ablnRegularTax)
                {
                    if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO)
                    {
                        icdoPayeeAccountTaxWithholding.plso_tax_option = icdoPayeeAccountTaxWithholding.tax_option_value;
                    }
                    else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum)
                    {
                        icdoPayeeAccountTaxWithholding.refund_tax_option = icdoPayeeAccountTaxWithholding.tax_option_value;
                    }
                }
                CalculateTaxForRefundOrPLSO();
            }
        }

        private void CalculateTaxForRefundOrPLSO()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            //Get the Taxable amount for Refund Benefit type or PLSO
            if (ibusPayeeAccount.idecTotalTaxableAmountForFlatRates == 0.00M)
            {
                if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO)
                {
                    ibusPayeeAccount.LoadTaxableAmountForFlatRates(ibusPayeeAccount.idtNextBenefitPaymentDate, true);
                }
                else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum)
                {
                    ibusPayeeAccount.LoadTaxableAmountForFlatRates(ibusPayeeAccount.idtNextBenefitPaymentDate, false);
                }
                else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD)
                {
                    ibusPayeeAccount.LoadTaxableAmountForFlatRates(ibusPayeeAccount.idtNextBenefitPaymentDate, false, true);
                }
            }
            if ((icdoPayeeAccountTaxWithholding.refund_tax_option == busConstant.TaxOptionFedTaxwithheld)
                || (icdoPayeeAccountTaxWithholding.refund_tax_option == busConstant.TaxOptionStateTaxwithheld))
            {
                CreatePaymentItemForFlatTax(ibusPayeeAccount.idecTotalTaxableAmountForFlatRates);
            }
            else if ((icdoPayeeAccountTaxWithholding.plso_tax_option == busConstant.TaxOptionFedTaxwithheld)
                || (icdoPayeeAccountTaxWithholding.plso_tax_option == busConstant.TaxOptionStateTaxwithheld))
            {
                CreatePaymentItemForFlatTax(ibusPayeeAccount.idecTotalTaxableAmountForFlatRates);
            }
            else if (icdoPayeeAccountTaxWithholding.tax_ref == busConstant.PayeeAccountTaxRefFed22Tax ||
                icdoPayeeAccountTaxWithholding.tax_ref == busConstant.PayeeAccountTaxRefState22Tax)
            {
                CreatePaymentItemForFlatTax(ibusPayeeAccount.idecTotalTaxableAmountForFlatRates);
            }
        }

        private void CalculateTaxForMonthlyBenefit(bool ablnRegularTax, bool ablnModifyPAPITs = true) //pir 8457
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            //Get the Taxable amount for Non -Refund Benefit type
            //Always reload the amount as it is beinng called for the same payee account more than once
            if (ibusPayeeAccount.idtStatusEffectiveDate != DateTime.MinValue && ibusPayeeAccount.iblnIsColaOrAdhocIncreaseTaxCalculation)
            {
                ibusPayeeAccount.LoadTaxableAmountForVariableTax(ibusPayeeAccount.idtStatusEffectiveDate);
            }
            else
            {
                ibusPayeeAccount.LoadTaxableAmountForVariableTax(ibusPayeeAccount.idtNextBenefitPaymentDate);
            }
            if (((icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option == busConstant.FedTaxOptionFedTaxBasedOnIRS) ||
                 (icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option == busConstant.FedTaxOptionFedTaxBasedOnIRSAndAdditional) ||
                (icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option == busConstant.StateTaxOptionFedTaxBasedOnIRS) ||
                (icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option == busConstant.StateTaxOptionFedTaxBasedOnIRSAndAdditional)) ||
                !string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref))
            {
                CreatePaymentItemForFedOrStateTax(ibusPayeeAccount.idecTotalTaxableAmountForVariableTax, ablnRegularTax, ablnModifyPAPITs); //pir 8457
            }
        }

        private void CreatePaymentItemForFlatTax(decimal adecTaxableAmount)
        {
            decimal ldecFlatTax = 0.00M;
            //Find the Flat tax for the taxable amount for the next benefit payment date
            if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO)
            {
                //PIR- 14407 added this block to substract PSLO Rollover Amount from Taxable Amount. 
                if (ibusPayeeAccount.iclbMonthlyBenefits.IsNullOrEmpty())
                    ibusPayeeAccount.LoadMontlyBenefits();

                decimal idecPlsoTaxableRolloverAmount = ibusPayeeAccount.iclbMonthlyBenefits
                                                       .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSOTaxableRollover).Sum(o => o.icdoPayeeAccountPaymentItemType.amount);
                if (idecPlsoTaxableRolloverAmount <= adecTaxableAmount)
                    adecTaxableAmount = adecTaxableAmount - idecPlsoTaxableRolloverAmount;
                else
                    adecTaxableAmount = 0;

                if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                {
                    ldecFlatTax = adecTaxableAmount * icdoPayeeAccountTaxWithholding.refund_fed_percent / 100;
                }
                else if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
                {
                    ldecFlatTax = icdoPayeeAccountTaxWithholding.refund_state_amt;
                }
            }
            else
            {   //PIR 1223 fix
                //if (ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipBeneficiary &&
                //    ibusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.FamilyRelationshipSpouse)
                //{
                //    ldecFlatTax = busPayeeAccountHelper.CalculateFlatTax(adecTaxableAmount,
                //  ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value, ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value,
                //  ibusPayeeAccount.icdoPayeeAccount.account_relation_value, ibusPayeeAccount.idtNextBenefitPaymentDate,
                //  icdoPayeeAccountTaxWithholding.tax_identifier_value, busConstant.Flag_No, ibusPayeeAccount.icdoPayeeAccount.family_relation_value, busConstant.Flag_No);
                //}
                //else
                //{
                //    ldecFlatTax = busPayeeAccountHelper.CalculateFlatTax(adecTaxableAmount,
                //  ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value, ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value,
                //  ibusPayeeAccount.icdoPayeeAccount.account_relation_value, ibusPayeeAccount.idtNextBenefitPaymentDate,
                //  icdoPayeeAccountTaxWithholding.tax_identifier_value, busConstant.Flag_No, null, busConstant.Flag_No);
                //}
                //PIR 25084 - Old Refund Fed Tax Calculation - Entering old way, calculating the new way, only for interim period as 
                //long as member submits the old withholding form.
                if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum)
                {
                    if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                    {
                        ldecFlatTax = icdoPayeeAccountTaxWithholding.refund_fed_percent * adecTaxableAmount / 100;
                    }
                    else if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
                    {
                        //ldecFlatTax = icdoPayeeAccountTaxWithholding.refund_state_amt;
                        if (icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.StateTaxOptionNoOnetimeNDTax)
                            ldecFlatTax = 0;
                        else if(icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.TaxOptionStateTaxwithheld)
                        {
                            ldecFlatTax = busPayeeAccountHelper.CalculateFlatTax(adecTaxableAmount, ibusPayeeAccount.idtNextBenefitPaymentDate, busConstant.PayeeAccountTaxIdentifierStateTax, icdoPayeeAccountTaxWithholding.tax_ref, busConstant.Flag_No);
                        }
                    }
                }
                else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD)
                {
                    if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                    {
                        ldecFlatTax = icdoPayeeAccountTaxWithholding.refund_fed_percent * adecTaxableAmount / 100;
                    }
                    else if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
                    {
                        ldecFlatTax = icdoPayeeAccountTaxWithholding.refund_state_amt;
                    }
                }
            }
            if (ldecFlatTax >= 0.00M)
            {
                if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
                {
                    //Put the Tax amount into payee account payment item type table as a payable item for the next benefit payment date   
                    if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO)
                    {
                        busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                        lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITPLSOStateTaxAmount);
                        ldecFlatTax = Math.Round(ldecFlatTax, 2, MidpointRounding.AwayFromZero);
                        if (!IsUpdateModeEndDateNotNull())
                        {
                            CreatePayeeAccoutTaxWithholdingDetail(lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFlatTax);
                        }
                    }
                    else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum)
                    {
                        busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                        lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITRefundNDStateTaxAmount);
                        ldecFlatTax = Math.Round(ldecFlatTax, 2, MidpointRounding.AwayFromZero);
                        if (!IsUpdateModeEndDateNotNull())
                        {
                            CreatePayeeAccoutTaxWithholdingDetail(lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFlatTax);
                        }
                    }
                    else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD)
                    {
                        busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                        lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITRMDStateTaxAmount);
                        ldecFlatTax = Math.Round(ldecFlatTax, 2, MidpointRounding.AwayFromZero);
                        if (!IsUpdateModeEndDateNotNull())
                        {
                            CreatePayeeAccoutTaxWithholdingDetail(lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFlatTax);
                        }
                    }
                }
                else
                {
                    //Put the Tax amount into payee account payment item type table as a payable item for the next benefit payment date   
                    if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO)
                    {
                        busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                        lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITPLSOFederalTaxAmount);
                        ldecFlatTax = Math.Round(ldecFlatTax, 2, MidpointRounding.AwayFromZero);
                        if (!IsUpdateModeEndDateNotNull())
                        {
                            CreatePayeeAccoutTaxWithholdingDetail(lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFlatTax);
                        }
                    }
                    else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum)
                    {
                        busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                        lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITRefundFederalTaxAmount);
                        ldecFlatTax = Math.Round(ldecFlatTax, 2, MidpointRounding.AwayFromZero);
                        if (!IsUpdateModeEndDateNotNull())
                        {
                            CreatePayeeAccoutTaxWithholdingDetail(lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFlatTax);
                        }
                    }
                    else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD)
                    {
                        busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                        lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITRMDFederalTaxAmount);
                        ldecFlatTax = Math.Round(ldecFlatTax, 2, MidpointRounding.AwayFromZero);
                        if (!IsUpdateModeEndDateNotNull())
                        {
                            CreatePayeeAccoutTaxWithholdingDetail(lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFlatTax);
                        }
                    }
                }
            }
        }
        //PIR 17060 - Roth Rollover Taxes Implementation
        public void CreatePaymentItemForFlatTaxForRothRollover(int aintRolloverOrgId, int aintPayeeAcccountId, int aintPayeeAccountRolloverDetailId, string astrstateTaxOption, bool ablnPlso, bool ablnDeleteIndicator)
        {
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            decimal idecRothTaxableRolloverAmount = 0.00M;
            decimal ldecFlatTax = 0.00M;
            string lstrRolloverTypeValue = string.Empty;
            DataTable ldtbMultipleRolloverOrg = new DataTable();
            ldtbMultipleRolloverOrg = Select("cdoPayeeAccountRolloverDetail.LoadTaxableAmountBasedOnRolloverId",
                new object[2] { aintPayeeAcccountId, aintPayeeAccountRolloverDetailId });
            DataTable ldtbMultipleRolloverOrgStateTax = new DataTable();
            ldtbMultipleRolloverOrgStateTax = Select("cdoPayeeAccountRolloverDetail.LoadTaxableAmountBasedOnstateTaxOptionValue", new object[3] { aintPayeeAcccountId, astrstateTaxOption, aintPayeeAccountRolloverDetailId });
            if (ldtbMultipleRolloverOrg.Rows.Count > 0)
            {
                idecRothTaxableRolloverAmount = Convert.ToDecimal(ldtbMultipleRolloverOrg.Rows[0]["AMOUNT"]);
                ldecFedTaxForRothRollverAmount = busPayeeAccountHelper.CalculateFlatTax(idecRothTaxableRolloverAmount,
                ibusPayeeAccount.idtNextBenefitPaymentDate,
                busConstant.PayeeAccountTaxIdentifierFedTax, busConstant.PayeeAccountTaxRefFed22Tax, busConstant.Flag_No);
            }
            if (ldtbMultipleRolloverOrgStateTax.Rows.Count > 0)
            {
                idecRothTaxableRolloverAmount = Convert.ToDecimal(ldtbMultipleRolloverOrgStateTax.Rows[0]["AMOUNT"]);
                ldecStateTaxForRothRollverAmount = busPayeeAccountHelper.CalculateFlatTax(idecRothTaxableRolloverAmount,
                ibusPayeeAccount.idtNextBenefitPaymentDate,
                busConstant.PayeeAccountTaxIdentifierStateTax, busConstant.PayeeAccountTaxRefState22Tax, busConstant.Flag_No);
            }
            FindAndUpdatePAPIT(aintPayeeAccountRolloverDetailId, ldecFedTaxForRothRollverAmount, ldecStateTaxForRothRollverAmount, astrstateTaxOption);

            if (idecRothTaxableRolloverAmount >= 0.00M)
            {
                busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                if (ablnPlso)
                {
                    CreatePayeeAccountRolloverItemDetail(aintPayeeAccountRolloverDetailId);
                    if (astrstateTaxOption == busConstant.TaxOptionStateTaxwithheld)
                    {
                        lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITPLSORothRolloverStateTaxAmount);
                        iblnUpdateFlag = SetUpdateFlag(aintPayeeAcccountId, aintRolloverOrgId, lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id);
                        ldecFlatTax = Math.Round(ldecStateTaxForRothRollverAmount, 2, MidpointRounding.AwayFromZero);
                        PostTaxAmountForRothRollover(busConstant.PayeeAccountTaxIdentifierStateTax, lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFlatTax, ablnDeleteIndicator);
                    }
                    lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITPLSORothRolloverFederalTaxAmount);
                    iblnUpdateFlag = SetUpdateFlag(aintPayeeAcccountId, aintRolloverOrgId, lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id);
                    ldecFlatTax = Math.Round(ldecFedTaxForRothRollverAmount, 2, MidpointRounding.AwayFromZero);
                    PostTaxAmountForRothRollover(busConstant.PayeeAccountTaxIdentifierFedTax, lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFlatTax, ablnDeleteIndicator);
                }
                else
                {
                    CreatePayeeAccountRolloverItemDetail(aintPayeeAccountRolloverDetailId);
                    if (astrstateTaxOption == busConstant.TaxOptionStateTaxwithheld)
                    {
                        lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITRothRolloverStateTaxAmount);
                        iblnUpdateFlag = SetUpdateFlag(aintPayeeAcccountId, aintRolloverOrgId, lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id);
                        ldecFlatTax = Math.Round(ldecStateTaxForRothRollverAmount, 2, MidpointRounding.AwayFromZero);
                        PostTaxAmountForRothRollover(busConstant.PayeeAccountTaxIdentifierStateTax, lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFlatTax, ablnDeleteIndicator);
                    }
                    lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITRothRolloverFederalTaxAmount);
                    iblnUpdateFlag = SetUpdateFlag(aintPayeeAcccountId, aintRolloverOrgId, lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id);
                    ldecFlatTax = Math.Round(ldecFedTaxForRothRollverAmount, 2, MidpointRounding.AwayFromZero);
                    PostTaxAmountForRothRollover(busConstant.PayeeAccountTaxIdentifierFedTax, lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFlatTax, ablnDeleteIndicator);

                }
            }
        }

        public void FindAndUpdatePAPIT(int aintPayeeAccountRolloverDetailId, decimal adecFedTaxForRothRollverAmount, decimal adecStateTaxForRothRollverAmount, string astrstateTaxOption)
        {
            DataTable ldtblist = Select<cdoPayeeAccountRolloverItemDetail>(
                new string[1] { "PAYEE_ACCOUNT_ROLLOVER_DETAIL_ID" },
                new object[1] { aintPayeeAccountRolloverDetailId }, null, null);
            int lintPayeeAccountPaymentItemTypeId = 0;
            if (ldtblist.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtblist.Rows)
                {
                    lintPayeeAccountPaymentItemTypeId = Convert.ToInt32(dr["PAYEE_ACCOUNT_PAYMENT_ITEM_TYPE_ID"]);
                    if (lintPayeeAccountPaymentItemTypeId > 0)
                    {
                        busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType();
                        if (lobjPayeeAccountPaymentItemType.FindPayeeAccountPaymentItemType(lintPayeeAccountPaymentItemTypeId))
                        {
                            adecFedTaxForRothRollverAmount = Math.Round(adecFedTaxForRothRollverAmount, 2, MidpointRounding.AwayFromZero);
                            adecStateTaxForRothRollverAmount = Math.Round(adecStateTaxForRothRollverAmount, 2, MidpointRounding.AwayFromZero);
                            busPaymentItemType lbusPaymentItemType = new busPaymentItemType();
                            lbusPaymentItemType.FindPaymentItemType(lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id);

                            if (lbusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                            {
                                if (astrstateTaxOption == busConstant.TaxOptionStateTaxwithheld)
                                {
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount = lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount - adecStateTaxForRothRollverAmount - adecFedTaxForRothRollverAmount;
                                }
                                else
                                {
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount = lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount - adecFedTaxForRothRollverAmount;
                                }

                                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
                            }
                        }
                    }
                }
            }
        }
        public bool SetUpdateFlag(int aintPayeeAccountId, int aintRolloverOrgId, int aintPaymentItemTypeId)
        {
            DataTable ldtbUpdateFlag = new DataTable();
            if (ibusPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                ldtbUpdateFlag = Select("cdoPayeeAccountRolloverDetail.SetUpdateFlag", new object[3] { aintPayeeAccountId, aintRolloverOrgId, aintPaymentItemTypeId });
            if (ldtbUpdateFlag.IsNotNull() && ldtbUpdateFlag.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void CreatePayeeAccountRolloverItemDetail(int aintPayeeAccountRolloverDetailId)
        {
            if (iclbPayeeAccountRolloverDetail == null)
                iclbPayeeAccountRolloverDetail = new Collection<busPayeeAccountRolloverItemDetail>();
            busPayeeAccountRolloverItemDetail lobjPayeeAccountRolloverItemDetail = new busPayeeAccountRolloverItemDetail();
            lobjPayeeAccountRolloverItemDetail.icdoPayeeAccountRolloverItemDetail = new cdoPayeeAccountRolloverItemDetail();
            lobjPayeeAccountRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_rollover_detail_id = aintPayeeAccountRolloverDetailId;
            iclbPayeeAccountRolloverDetail.Add(lobjPayeeAccountRolloverItemDetail);
        }
        private void CreatePaymentItemForFedOrStateTax(decimal adecTaxableAmount, bool ablnRegularTax, bool ablnModifyPapits = true) //pir 8457
        {
            int lintallowance = 0;
            decimal ldecFedOrStateTaxAmount = 0.00M;
            string lstrMaritalStatus = string.Empty;
            if (icdoPayeeAccountTaxWithholding.marital_status_value == busConstant.MaritalStatusMarriedWithholdAtSingleRate)
            {
                lstrMaritalStatus = busConstant.PersonMaritalStatusSingle;
            }
            else
            {
                lstrMaritalStatus = icdoPayeeAccountTaxWithholding.marital_status_value;
            }
            if (!string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.no_of_tax_allowance))
            {
                lintallowance = Convert.ToInt32(icdoPayeeAccountTaxWithholding.no_of_tax_allowance);
            }
            //Find the Fed or state tax for the taxable amount for the next benefit payment date
            //pir 8457 start
            DateTime ldteTaxAsOfDate = DateTime.MinValue;
            if (ibusPayeeAccount.idtStatusEffectiveDate != DateTime.MinValue && ibusPayeeAccount.iblnIsColaOrAdhocIncreaseTaxCalculation)
            {
                ldteTaxAsOfDate = ibusPayeeAccount.idtStatusEffectiveDate;
            }
            else if (ablnRegularTax)
            {
                ldteTaxAsOfDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
            }
            else
            {
                ldteTaxAsOfDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
            }
            if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax &&
               icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
            {
                ldecFedOrStateTaxAmount = CalculateW4PTaxWithholdingAmount(adecTaxableAmount, ldteTaxAsOfDate);
            }
            else if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax &&
               icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
            {
                if (string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref))
                {
                    ldecFedOrStateTaxAmount = busPayeeAccountHelper.CalculateFedOrStateTax(adecTaxableAmount,
                                                                                            lintallowance, ldteTaxAsOfDate/*pir 8457*/,
                                                                                            lstrMaritalStatus, icdoPayeeAccountTaxWithholding.tax_identifier_value,
                                                                                            icdoPayeeAccountTaxWithholding.additional_tax_amount);
                    icdoPayeeAccountTaxWithholding.state_flat_amount = ldecFedOrStateTaxAmount;
                }
                else
                {
                    ldecFedOrStateTaxAmount = icdoPayeeAccountTaxWithholding.state_flat_amount;
                }
            }
            //pir 8457 end
            //ldecFedOrStateTaxAmount = busPayeeAccountHelper.CalculateFedOrStateTax(adecTaxableAmount,
            //    lintallowance, ldteTaxAsOfDate/*pir 8457*/,
            //    lstrMaritalStatus, icdoPayeeAccountTaxWithholding.tax_identifier_value,
            //    icdoPayeeAccountTaxWithholding.additional_tax_amount);
            ldecFedOrStateTaxAmount = Math.Round(ldecFedOrStateTaxAmount, 2, MidpointRounding.AwayFromZero);
            idecTaxAmount = ldecFedOrStateTaxAmount; //pir 8618
            if (ldecFedOrStateTaxAmount != 0.00M)
            {
                if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
                {

                    busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                    lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITNDStateTaxAmount);

                    if (!IsUpdateModeEndDateNotNull())
                    {
                        //Put the Tax amount into payee account payment item type table as a payable item for the next benefit payment date   
                        CreatePayeeAccoutTaxWithholdingDetail(lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFedOrStateTaxAmount);
                    }
                }
                else
                {
                    busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                    lobjPaymentItemType.FindPaymentItemTypeByItemCode(busConstant.PAPITFederalTaxAmount);

                    if (!IsUpdateModeEndDateNotNull())
                    {
                        //Put the Tax amount into payee account payment item type table as a payable item for the next benefit payment date  
                        CreatePayeeAccoutTaxWithholdingDetail(lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id, ldecFedOrStateTaxAmount);
                    }
                }
            }
            //uat pir 1242 : need to enddate any existing tax item if new tax is zero
            else if (ablnModifyPapits)
            {
                EndDateFedOrStateTaxItems();
            }
        }

        /// <summary>
        /// method to enddate any fed or state tax, if the current amount is zero
        /// </summary>
        private void EndDateFedOrStateTaxItems()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            busPayeeAccountPaymentItemType lobjPAPIT;
            if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
            {
                lobjPAPIT = ibusPayeeAccount.GetLatestPayeeAccountPaymentItemType(busConstant.PAPITNDStateTaxAmount);
            }
            else
            {
                lobjPAPIT = ibusPayeeAccount.GetLatestPayeeAccountPaymentItemType(busConstant.PAPITFederalTaxAmount);
            }

            if (lobjPAPIT != null)
            {
                //to delete or end date old entry
                ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lobjPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id, 0.0M,
                                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.account_number,
                                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.vendor_org_id,
                                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.start_date,
                                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date, false);
            }
        }

        //There are different tax option code values for Monthly Benefit and PLSO.        
        //If Benefit Type is non refund and PLSO flag checked ,then we wil have two tax option dropdown list         
        //Based on the tax identifier in the data2 and data1 in code value for id 2218,display values
        //The method is used to load value in Tax option drop down list
        public Collection<cdoCodeValue> LoadTaxOptionBasedOnTaxIdentifier()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            Collection<cdoCodeValue> lclcTaxOption = new Collection<cdoCodeValue>();
            DataTable ldtbTaxOptions = iobjPassInfo.isrvDBCache.GetCodeValues(busConstant.TaxOptionCodeID);
            foreach (DataRow dr in ldtbTaxOptions.Rows)
            {
                cdoCodeValue lobjCodeValue = new cdoCodeValue();
                if (dr["data2"].ToString() == icdoPayeeAccountTaxWithholding.tax_identifier_value)
                {
                    if ((icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
                    && (dr["data1"].ToString() == busConstant.Flag_Yes))
                    {
                        lobjCodeValue.LoadData(dr);
                        lclcTaxOption.Add(lobjCodeValue);
                    }
                    else if ((icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO)
                        || (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum))
                    {
                        if (dr["data1"].ToString() == busConstant.Flag_No)
                        {
                            lobjCodeValue.LoadData(dr);
                            lclcTaxOption.Add(lobjCodeValue);
                        }
                    }
                }
            }
            return lclcTaxOption;
        }

        public Collection<cdoCodeValue> LoadPLSOTaxOptionBasedOnTaxIdentifier()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            Collection<cdoCodeValue> lclcTaxOption = new Collection<cdoCodeValue>();
            DataTable ldtbTaxOptions = iobjPassInfo.isrvDBCache.GetCodeValues(busConstant.TaxOptionCodeID);
            foreach (DataRow dr in ldtbTaxOptions.Rows)
            {
                cdoCodeValue lobjCodeValue = new cdoCodeValue();
                if (dr["data2"].ToString() == icdoPayeeAccountTaxWithholding.tax_identifier_value)
                {
                    if (dr["data3"].ToString() == busConstant.Flag_Yes)
                    {
                        lobjCodeValue.LoadData(dr);
                        lclcTaxOption.Add(lobjCodeValue);
                    }
                }
            }
            return lclcTaxOption;
        }

        public bool IsSuppressWarningFlagChecked()
        {
            if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
            {
                if (icdoPayeeAccountTaxWithholding.monthlybenefit_suppress_warning == busConstant.Flag_Yes)
                {
                    return true;
                }
            }
            else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO)
            {
                if (icdoPayeeAccountTaxWithholding.plso_supress_warning == busConstant.Flag_Yes)
                {
                    return true;
                }
            }
            else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == BusinessObjects.busConstant.BenefitDistributionLumpSum)
            {
                if (icdoPayeeAccountTaxWithholding.refund_supress_warning == busConstant.Flag_Yes)
                {
                    return true;
                }
            }
            return false;
        }
        //BR 27, If the Payee is not in north dakota ,throw a warnig "Send North Dakota Withholding Letter"
        public bool IsPayeeNotinNorthDakota()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (!IsSuppressWarningFlagChecked())
            {
                // PIR 10932 - Warning should not be thrown in MSS
                if (ibusPayeeAccount.iblnIsFromMSS)
                {
                    return false;
                }
                if (ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != 0)
                {
                    if (ibusPayeeAccount.ibusPayee == null)
                        ibusPayeeAccount.LoadPayee();
                    if (ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress == null)
                        ibusPayeeAccount.ibusPayee.LoadPersonCurrentAddress(icdoPayeeAccountTaxWithholding.start_date);
                    if (ibusPayeeAccount.ibusPayee.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value != busConstant.StateNorthDakota)
                    {
                        return true;
                    }
                }
                else if (ibusPayeeAccount.icdoPayeeAccount.payee_org_id != 0)
                {
                    if (ibusPayeeAccount.ibusRecipientOrganization == null)
                        ibusPayeeAccount.LoadRecipientOrganization();
                    if (ibusPayeeAccount.ibusRecipientOrganization.ibusOrgPrimaryAddress == null)
                        ibusPayeeAccount.ibusRecipientOrganization.LoadOrgPrimaryAddress();
                    if (ibusPayeeAccount.ibusRecipientOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.status_value != busConstant.StateNorthDakota)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (iobjPassInfo.istrPostBackControlID == "btnSave")
            {
                iblnIsSaveButtonClicked = true;
            }
            if (aenmPageMode == utlPageMode.Update)
            {
                iblnIsUpdateMode = true;
            }
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            //Set the start date as next benfit payment date for plso and refund                        
            if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO)
            {
                icdoPayeeAccountTaxWithholding.start_date = ibusPayeeAccount.idtNextBenefitPaymentDate;
                icdoPayeeAccountTaxWithholding.tax_option_value = icdoPayeeAccountTaxWithholding.plso_tax_option;
            }
            else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum)
            {
                icdoPayeeAccountTaxWithholding.start_date = ibusPayeeAccount.idtNextBenefitPaymentDate;
                icdoPayeeAccountTaxWithholding.tax_option_value = icdoPayeeAccountTaxWithholding.refund_tax_option;
            }
            else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD)
            {
                icdoPayeeAccountTaxWithholding.start_date = ibusPayeeAccount.idtNextBenefitPaymentDate;
            }
            else
            {
                icdoPayeeAccountTaxWithholding.tax_option_value = icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option;
                if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax &&
                    icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit &&
                    string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref))
                {
                    if (icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option == busConstant.NoFedTax)
                        icdoPayeeAccountTaxWithholding.no_fed_withholding = busConstant.Flag_Yes;
                    LoadTaxRef();
                }

            }
            base.BeforeValidate(aenmPageMode);
        }
        //Create Payee Accout TaxWithholding Detail for state tax ,fed tax and flat tax
        public void CreatePayeeAccoutTaxWithholdingDetail(int aintPaymentItemTypeID, decimal adecAmount)
        {
            if (iclbTaxWithHoldingTaxItems == null)
                iclbTaxWithHoldingTaxItems = new Collection<busPayeeAccountTaxWithholdingItemDetail>();
            busPayeeAccountTaxWithholdingItemDetail lobjPayeeAccountTaxWithholdingItemDetail = new busPayeeAccountTaxWithholdingItemDetail();
            lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail = new cdoPayeeAccountTaxWithholdingItemDetail();
            lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_tax_withholding_id
                = icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id;
            //lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id = aintPayeeAccountPaymentItemTypeID;
            lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id = aintPaymentItemTypeID;
            lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.amount = adecAmount;
            if (lobjPayeeAccountTaxWithholdingItemDetail.ibusPaymentItemType == null)
                lobjPayeeAccountTaxWithholdingItemDetail.LoadPaymentItemType();
            if (iblnIsCalculateButtonClicked)
                iclbTaxWithHoldingTaxItems.Clear();
            iclbTaxWithHoldingTaxItems.Add(lobjPayeeAccountTaxWithholdingItemDetail);
        }
        //prod pir 7472 : property to save old tax withholding end date
        public DateTime idtOldTaxwithholdingEndDate { get; set; }

        public override void BeforePersistChanges()
        {
            if (icdoPayeeAccountTaxWithholding.no_of_tax_allowance != null)
            {
                icdoPayeeAccountTaxWithholding.tax_allowance = Convert.ToInt32(icdoPayeeAccountTaxWithholding.no_of_tax_allowance);
            }
            if (IsSuppressWarningFlagChecked())
            {
                icdoPayeeAccountTaxWithholding.suppress_warnings_flag = busConstant.Flag_Yes;
            }
            else
            {
                icdoPayeeAccountTaxWithholding.suppress_warnings_flag = busConstant.Flag_No;
            }
            //prod pir 7472
            if (icdoPayeeAccountTaxWithholding.ihstOldValues.Count > 0 && icdoPayeeAccountTaxWithholding.ihstOldValues["end_date"] != null &&
                Convert.ToDateTime(icdoPayeeAccountTaxWithholding.ihstOldValues["end_date"]) != DateTime.MinValue)
            {
                idtOldTaxwithholdingEndDate = Convert.ToDateTime(icdoPayeeAccountTaxWithholding.ihstOldValues["end_date"]);
            }
            if (!ibusPayeeAccount.iblnIsFromMSS && icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax &&
                icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
            {
                if (!string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref))
                {
                    //icdoPayeeAccountTaxWithholding.two_b_iii = icdoPayeeAccountTaxWithholding.two_b_i + icdoPayeeAccountTaxWithholding.two_b_ii;
                    //icdoPayeeAccountTaxWithholding.three_total = icdoPayeeAccountTaxWithholding.three_1 + icdoPayeeAccountTaxWithholding.three_2 + icdoPayeeAccountTaxWithholding.three_3;
                }
                else
                {
                    if (ibusTaxRefConfig.IsNull()) LoadTaxRef();
                    icdoPayeeAccountTaxWithholding.four_a = (icdoPayeeAccountTaxWithholding.marital_status_value == busConstant.PersonMaritalStatusSingle || icdoPayeeAccountTaxWithholding.marital_status_value == busConstant.MaritalStatusMarriedWithholdAtSingleRate)
                                                            ? ibusTaxRefConfig.icdoTaxRefConfig.oneg_amt_hohh : (icdoPayeeAccountTaxWithholding.marital_status_value == busConstant.PersonMaritalStatusMarried)
                                                            ? ibusTaxRefConfig.icdoTaxRefConfig.oneg_amt_married : ibusTaxRefConfig.icdoTaxRefConfig.oneg_amt_hohh;
                    icdoPayeeAccountTaxWithholding.four_b = icdoPayeeAccountTaxWithholding.tax_allowance * (ibusTaxRefConfig?.icdoTaxRefConfig?.default_allowance_amt > 0 ? ibusTaxRefConfig.icdoTaxRefConfig.default_allowance_amt : 4300);
                    icdoPayeeAccountTaxWithholding.four_c = icdoPayeeAccountTaxWithholding.additional_tax_amount;
                }
            }

            if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum
                && icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax &&
                string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref) && icdoPayeeAccountTaxWithholding.refund_fed_percent == 0.0M)
            {
                icdoPayeeAccountTaxWithholding.refund_fed_percent = 20.0M;
            }
            //else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum
            //    && icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax &&
            //    //string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref) && icdoPayeeAccountTaxWithholding.refund_state_amt == 0.0M &&
            //    icdoPayeeAccountTaxWithholding.tax_option_value != busConstant.NoStateTaxWithheld)
            //{
            //    if (ibusPayeeAccount == null)
            //        LoadPayeeAccount();
            //    if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
            //        ibusPayeeAccount.LoadNexBenefitPaymentDate();
            //    ibusPayeeAccount.LoadTaxableAmountForFlatRates(ibusPayeeAccount.idtNextBenefitPaymentDate, false);
            //    icdoPayeeAccountTaxWithholding.refund_state_amt = ibusPayeeAccount.idecTotalTaxableAmountForFlatRates * 0.0392M;
            //}
            base.BeforePersistChanges();
        }
        public override int Delete()
        {
            //On deletion of  Details change the payee account status to review
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusSoftErrors == null)
                ibusPayeeAccount.LoadErrors();
            ibusPayeeAccount.iblnClearSoftErrors = false;
            ibusPayeeAccount.ibusSoftErrors.iblnClearError = false;
            ibusPayeeAccount.CreateReviewPayeeAccountStatus();
            ibusPayeeAccount.iblnTaxWithholdingInfoDeleteIndicator = true;
            ibusPayeeAccount.ValidateSoftErrors();
            ibusPayeeAccount.UpdateValidateStatus();
            DeleteTaxItems();
            return base.Delete();
        }

        public void DeleteTaxItems()
        {
            //here it is no null check bcoz it has to reloaded always            
            LoadTaxWithHoldingTaxItems();
            LoadPayeeAccountTaxItems();
            foreach (busPayeeAccountTaxWithholdingItemDetail lobjTaxWithholdingDetail in iclbTaxWithHoldingTaxItems)
            {
                lobjTaxWithholdingDetail.icdoPayeeAccountTaxWithholdingItemDetail.Delete();
            }

            foreach (busPayeeAccountPaymentItemType lobjTaxItems in iclbPayeeAccountTaxItems)
            {
                if (lobjTaxItems.FindPayeeAccountPaymentItemType(lobjTaxItems.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id))
                {
                    lobjTaxItems.icdoPayeeAccountPaymentItemType.Delete();
                }
            }
            //here it is no null check bcoz it has to reloaded always            
            LoadPayeeAccountTaxItems();
            LoadTaxWithHoldingTaxItems();
        }
        public void UpdateEndateForOldTax()
        {
            if (iclbTaxWithHoldingTaxItems == null)
                LoadTaxWithHoldingTaxItems();
            if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            Collection<busPayeeAccountPaymentItemType> lclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItemType in ibusPayeeAccount.iclbPayeeAccountPaymentItemType)
            {
                foreach (busPayeeAccountTaxWithholdingItemDetail lobjDetails in iclbTaxWithHoldingTaxItems)
                {
                    if ((lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id == lobjDetails.icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id)
                                    && (lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue ||
                                    (idtOldTaxwithholdingEndDate != DateTime.MinValue && idtOldTaxwithholdingEndDate != icdoPayeeAccountTaxWithholding.end_date &&
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date == idtOldTaxwithholdingEndDate))) //prod pir 7472
                    {
                        lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = icdoPayeeAccountTaxWithholding.end_date;
                        lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
                    }
                }
            }
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

        //uat pir 1420
        public bool IsUpdateNotAllowedWhenPayeeAccountStatusCancelPendingOrCancelled()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatus();

            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
            {
                if (ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_effective_date < icdoPayeeAccountTaxWithholding.end_date)
                    lblnResult = true;
            }

            return lblnResult;
        }

        #region Method for Correspondence UCS - 073

        public override busBase GetCorPerson()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayee == null)
                ibusPayeeAccount.LoadPayee();

            return ibusPayeeAccount.ibusPayee;
        }
        #endregion
        public Collection<busPayeeAccountRolloverItemDetail> iclbPayeeAccountRolloverDetail { get; set; }
        public bool iblnUpdateFlag { get; set; }
        public decimal ldecStateTaxForRothRollverAmount { get; set; }
        public decimal ldecFedTaxForRothRollverAmount { get; set; }
        public bool iblnSkipSave { get; set; }

        public decimal CalculateW4PTaxWithholdingAmount(decimal adecTaxableAmount, DateTime adteEffectiveDate)
        {
            //1. Reset W4P Calculation
            ResetCalculatedFields();
            // 2. Do not run W4P tax withholding calculation if the 'no_fed_withholding' flag is set
            if (!string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.no_fed_withholding) && icdoPayeeAccountTaxWithholding.no_fed_withholding == busConstant.Flag_Yes)
                return 0.0m;

            // 3. Initialize W4P Calculation values
            LoadW4PTaxCalculationValues();

            // 4. Calculate the Adjusted Annual Payment Amount - Steps 1 from W4P
            CalculateAdjustedAnnualPaymentAmount(adecTaxableAmount);

            // 5. Load Tax Calculation Effective Date.
            LoadTaxCalculationEffectiveDate(adteEffectiveDate);

            // 6. Calculate the Tentative Annual Withholding Amount - Steps 2 from W4P 
            CalculateTentativeAnnualWithholdingAmount();

            // 7. Calculate Tax Credit Amount - Steps 3 from W4P 
            CalculateTaxCreditAmount();

            // 8. Calculate Tax Withholding Amount - Steps 4 from W4P
            CalculateWithholdingAmount();

            // 9. Persist Calculation
            if (!iblnSkipSave)
            {
                if (icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id == 0)
                    icdoPayeeAccountTaxWithholding.Insert();
                else
                    icdoPayeeAccountTaxWithholding.Update();
            }

            return icdoPayeeAccountTaxWithholding.fed_tax_amt;
        }

        private void ResetCalculatedFields()
        {
            icdoPayeeAccountTaxWithholding.no_of_periods = 0;
            icdoPayeeAccountTaxWithholding.tax_payer_deduction_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.annual_benefit_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.net_benefit_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.net_deduction_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.post_ded_benefit_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.adj_annual_benefit_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.total_income_and_pension_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.step2c_computed_net_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.adjusted_taxable_income_tax_rate_amount = 0.0M;
            icdoPayeeAccountTaxWithholding.exceeding_tax_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.tentative_tax_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.tax_percentage = 0.0M;
            icdoPayeeAccountTaxWithholding.calc_taxable_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.calc_taxable_perc_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.tentative_annual_withhold_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.final_tentative_annual_withhold_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.part2_total_income_and_pension_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.step2k_computed_partii_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.part_2_exceeding_tax_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.part_2_tentative_tax_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.part_2_tax_percentage = 0.0M;
            icdoPayeeAccountTaxWithholding.part_2_calc_taxable_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.part_2_calc_taxable_perc_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.part_2_tentative_annual_withhold_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.part_2_comp_tentative_annual_withhold_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.taxable_credit_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.calc_withhold_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.fed_tax_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.calc_allowance_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.post_allowance_benefit_amt = 0.0M;
            icdoPayeeAccountTaxWithholding.monthly_taxable_amt = 0.0M;
        }

        private void CalculateWithholdingAmount()
        {
            if (icdoPayeeAccountTaxWithholding.no_of_periods == 0)
                icdoPayeeAccountTaxWithholding.no_of_periods = busConstant.Twelve;
            // #Step 4a
            icdoPayeeAccountTaxWithholding.calc_withhold_amt = icdoPayeeAccountTaxWithholding.taxable_credit_amt / icdoPayeeAccountTaxWithholding.no_of_periods;

            // #Step 4b is Additional Withholding from W4P Form, #Step 4c is calculated as the final Fed Tax Amount
            icdoPayeeAccountTaxWithholding.fed_tax_amt = Math.Round(icdoPayeeAccountTaxWithholding.calc_withhold_amt + icdoPayeeAccountTaxWithholding.four_c, 2, MidpointRounding.AwayFromZero);
        }

        private void CalculateTaxCreditAmount()
        {
            //#Step 3b - Calculate the final Tax Credit Amount by subtracting #3a from #2s
            icdoPayeeAccountTaxWithholding.taxable_credit_amt = icdoPayeeAccountTaxWithholding.final_tentative_annual_withhold_amt - icdoPayeeAccountTaxWithholding.three_total;

            if (icdoPayeeAccountTaxWithholding.taxable_credit_amt < busConstant.ZeroDecimal)
                icdoPayeeAccountTaxWithholding.taxable_credit_amt = busConstant.ZeroDecimal;
        }

        private void CalculateTentativeAnnualWithholdingAmount()
        {
            // Step 2 - Part I
            CalculatePart1TentativeAnnualWithholdingAmount();

            // Step 2 - Part II When there is an amount in #Step 2a and no amount in #Step 2d
            if (icdoPayeeAccountTaxWithholding.two_b_iii != busConstant.ZeroDecimal)
            {
                CalculatePart2TentativeWithholdingAmount();

                // Part III amount 
                // # Step 2u - When Step 2(b)(iii) amount is given then this is derived from #2t
                icdoPayeeAccountTaxWithholding.final_tentative_annual_withhold_amt = icdoPayeeAccountTaxWithholding.part_2_comp_tentative_annual_withhold_amt;
            }
            else // # Step 2u - When Step 2(b)(iii) amount is given then this is derived from #2t // Part III amount 
                icdoPayeeAccountTaxWithholding.final_tentative_annual_withhold_amt = icdoPayeeAccountTaxWithholding.tentative_annual_withhold_amt;
        }

        private void CalculatePart2TentativeWithholdingAmount()
        {
            // #Step #2k - Calculate the Total Income from Annual Taxable Payment Amount Step 2(b)(iii) + Adjusted Annual Benefit Amount from 1i
            icdoPayeeAccountTaxWithholding.part2_total_income_and_pension_amt = icdoPayeeAccountTaxWithholding.two_b_iii;

            // #Step 2l - is derived from #Step 1i which is post_ded_benefit_amt. Use this amount even if it is negative.

            // #Step 2m - Computed Amount 2k + 2l - When the amount is less than $0.00, this field will be $0.00
            icdoPayeeAccountTaxWithholding.step2k_computed_partii_amt = icdoPayeeAccountTaxWithholding.part2_total_income_and_pension_amt + icdoPayeeAccountTaxWithholding.post_ded_benefit_amt;
            if (icdoPayeeAccountTaxWithholding.step2k_computed_partii_amt < busConstant.ZeroDecimal)
                icdoPayeeAccountTaxWithholding.step2k_computed_partii_amt = busConstant.ZeroDecimal;

            // Determine the Tax Rate based on the Adjusted Taxable Income Tax Rate Amount
            DataTable ldtbResult = busBase.Select(busConstant.GetTaxRateBasedOnEffecDate, new object[5] {
                                                    icdoPayeeAccountTaxWithholding.tax_identifier_value,
                                                    string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref) ? busConstant.PayeeAccountTaxRefFed22Tax : icdoPayeeAccountTaxWithholding.tax_ref,
                                                    icdoPayeeAccountTaxWithholding.filing_status_value,
                                                    icdoPayeeAccountTaxWithholding.idteTaxCalcultionEffectiveDate,
                                                    icdoPayeeAccountTaxWithholding.step2k_computed_partii_amt
                                                });
            if (ldtbResult.IsNotNull() && ldtbResult.Rows.Count > busConstant.ZeroInt)
            {
                // Initialize the W4 Tax Rate object
                busFedStateTaxRate lbusFedStateTaxRate = new busFedStateTaxRate() { icdoFedStateTaxRate = new cdoFedStateTaxRate() };
                lbusFedStateTaxRate.icdoFedStateTaxRate.LoadData(ldtbResult.Rows[busConstant.ZeroInt]);

                // #Step 2n
                icdoPayeeAccountTaxWithholding.part_2_exceeding_tax_amt = lbusFedStateTaxRate.icdoFedStateTaxRate.minimum_amount;

                // #Step 2o
                icdoPayeeAccountTaxWithholding.part_2_tentative_tax_amt = lbusFedStateTaxRate.icdoFedStateTaxRate.tax_amount;

                // #Step 2p
                icdoPayeeAccountTaxWithholding.part_2_tax_percentage = lbusFedStateTaxRate.icdoFedStateTaxRate.percentage;

                // #Step 2q - Computed 2m - 2n
                icdoPayeeAccountTaxWithholding.part_2_calc_taxable_amt = icdoPayeeAccountTaxWithholding.step2k_computed_partii_amt - icdoPayeeAccountTaxWithholding.part_2_exceeding_tax_amt;

                // #Step 2r
                icdoPayeeAccountTaxWithholding.part_2_calc_taxable_perc_amt = icdoPayeeAccountTaxWithholding.part_2_calc_taxable_amt * icdoPayeeAccountTaxWithholding.part_2_tax_percentage / busConstant.HundredDecimal;

                // #Step 2s Computed 2o + 2r
                icdoPayeeAccountTaxWithholding.part_2_tentative_annual_withhold_amt = icdoPayeeAccountTaxWithholding.part_2_calc_taxable_perc_amt + icdoPayeeAccountTaxWithholding.part_2_tentative_tax_amt;

                // #Step 2t Computed 2s - 2j
                icdoPayeeAccountTaxWithholding.part_2_comp_tentative_annual_withhold_amt = icdoPayeeAccountTaxWithholding.part_2_tentative_annual_withhold_amt - icdoPayeeAccountTaxWithholding.tentative_annual_withhold_amt;
                if (icdoPayeeAccountTaxWithholding.part_2_comp_tentative_annual_withhold_amt < busConstant.ZeroDecimal)
                    icdoPayeeAccountTaxWithholding.part_2_comp_tentative_annual_withhold_amt = busConstant.ZeroDecimal;
            }
        }

        private void CalculatePart1TentativeAnnualWithholdingAmount()
        {
            // #Step #2a - Calculate the Total Income from other job and pension annuity amount, already captured in field two_b_iii
            if (icdoPayeeAccountTaxWithholding.two_b_iii != busConstant.ZeroDecimal)
            {
                // #Step 2b - Initialized from tax ref config in field 

                // #Step 2c - Computed Field Step 2a - Step 2b
                icdoPayeeAccountTaxWithholding.step2c_computed_net_amt = icdoPayeeAccountTaxWithholding.two_b_iii - icdoPayeeAccountTaxWithholding.tax_payer_deduction_amt;

                // #Step 2d - Adjusted Taxable Income Tax Rate Amount
                if (icdoPayeeAccountTaxWithholding.step2c_computed_net_amt > busConstant.ZeroDecimal)
                    icdoPayeeAccountTaxWithholding.adjusted_taxable_income_tax_rate_amount = icdoPayeeAccountTaxWithholding.step2c_computed_net_amt;
                else
                    icdoPayeeAccountTaxWithholding.adjusted_taxable_income_tax_rate_amount = busConstant.ZeroDecimal;
            }
            else // #Step 2d - When #Step 2a is not given this amount is derived from #Step 1l
                icdoPayeeAccountTaxWithholding.adjusted_taxable_income_tax_rate_amount = icdoPayeeAccountTaxWithholding.adj_annual_benefit_amt;


            // Determine the Tax Rate based on the Adjusted Taxable Income Tax Rate Amount
            DataTable ldtbTaxRateResult = busBase.Select(busConstant.GetTaxRateBasedOnEffecDate, new object[5] {
                                                    icdoPayeeAccountTaxWithholding.tax_identifier_value,
                                                    string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref) ? busConstant.PayeeAccountTaxRefFed22Tax : icdoPayeeAccountTaxWithholding.tax_ref,
                                                    icdoPayeeAccountTaxWithholding.filing_status_value,
                                                    icdoPayeeAccountTaxWithholding.idteTaxCalcultionEffectiveDate,
                                                    icdoPayeeAccountTaxWithholding.adjusted_taxable_income_tax_rate_amount
                                                });
            if (ldtbTaxRateResult.IsNotNull() && ldtbTaxRateResult.Rows.Count > busConstant.ZeroInt)
            {
                // Initialize the W4 Tax Rate object
                busFedStateTaxRate lbusFedStateTaxRate = new busFedStateTaxRate() { icdoFedStateTaxRate = new cdoFedStateTaxRate() };
                lbusFedStateTaxRate.icdoFedStateTaxRate.LoadData(ldtbTaxRateResult.Rows[busConstant.ZeroInt]);

                // #Step 2e
                icdoPayeeAccountTaxWithholding.exceeding_tax_amt = lbusFedStateTaxRate.icdoFedStateTaxRate.minimum_amount;

                // #Step 2f
                icdoPayeeAccountTaxWithholding.tentative_tax_amt = lbusFedStateTaxRate.icdoFedStateTaxRate.tax_amount;

                // #Step 2g
                icdoPayeeAccountTaxWithholding.tax_percentage = lbusFedStateTaxRate.icdoFedStateTaxRate.percentage;

                // #Step 2h Computed 2d - 2e
                icdoPayeeAccountTaxWithholding.calc_taxable_amt = icdoPayeeAccountTaxWithholding.adjusted_taxable_income_tax_rate_amount - icdoPayeeAccountTaxWithholding.exceeding_tax_amt;

                // #Step 2i
                icdoPayeeAccountTaxWithholding.calc_taxable_perc_amt = icdoPayeeAccountTaxWithholding.calc_taxable_amt * icdoPayeeAccountTaxWithholding.tax_percentage / busConstant.HundredDecimal;

                // #Step 2j Computed 2f + 2i
                icdoPayeeAccountTaxWithholding.tentative_annual_withhold_amt = icdoPayeeAccountTaxWithholding.calc_taxable_perc_amt + icdoPayeeAccountTaxWithholding.tentative_tax_amt;
            }
        }

        private void LoadTaxCalculationEffectiveDate(DateTime adteEffectiveDate)
        {
            if (ibusPayeeAccount.IsNull() || ibusPayeeAccount.icdoPayeeAccount.payee_account_id == 0)
                LoadPayeeAccount();
            if (adteEffectiveDate == DateTime.MinValue)
            {
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                icdoPayeeAccountTaxWithholding.idteTaxCalcultionEffectiveDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
            }
            else
            {
                icdoPayeeAccountTaxWithholding.idteTaxCalcultionEffectiveDate = adteEffectiveDate;
            }
        }
        private void LoadBridgeTaxCalculationEffectiveDate(DateTime adteEffectiveDate)
        {
            if (ibusPayeeAccount.IsNull() || ibusPayeeAccount.icdoPayeeAccount.payee_account_id == 0)
                LoadPayeeAccount();
            if (adteEffectiveDate == DateTime.MinValue)
            {
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                icdoPayeeAccountTaxWithholding.idteTaxCalcultionEffectiveDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
            }
            else
            {
                if (ibusTaxRefConfig?.icdoTaxRefConfig?.effective_date.Date > adteEffectiveDate.Date)
                    adteEffectiveDate = ibusTaxRefConfig.icdoTaxRefConfig.effective_date;
                icdoPayeeAccountTaxWithholding.idteTaxCalcultionEffectiveDate = adteEffectiveDate;
            }
        }

        private void CalculateAdjustedAnnualPaymentAmount(decimal adecTaxableAmount)
        {
            // #Step 1c - Calculate Annual Benefit Amount
            icdoPayeeAccountTaxWithholding.annual_benefit_amt = adecTaxableAmount * icdoPayeeAccountTaxWithholding.no_of_periods;

            // #Step 1d - Other Income Pulled from Withholding Form Step 4(a) icdoPayeeAccountTaxWithholding.four_a

            // #Step 1e - Compute Net Benefit Amount 1c + 1d
            icdoPayeeAccountTaxWithholding.net_benefit_amt = icdoPayeeAccountTaxWithholding.annual_benefit_amt + icdoPayeeAccountTaxWithholding.four_a;

            // #Step 1f - Pulled from Withholding Form Step 4(b) - Deductions icdoPayeeAccountTaxWithholding.four_b
            // #Step 1g - Initialized from tax_ref table.

            // #Step 1h - Compute Net Deduction Amount 1f + 1g
            icdoPayeeAccountTaxWithholding.net_deduction_amt = icdoPayeeAccountTaxWithholding.four_b + icdoPayeeAccountTaxWithholding.tax_payer_deduction_amt;

            // #Step 1i - Compute Post Deduction Benefit Amount. 1e - 1h This amount can go negative
            icdoPayeeAccountTaxWithholding.post_ded_benefit_amt = icdoPayeeAccountTaxWithholding.net_benefit_amt - icdoPayeeAccountTaxWithholding.net_deduction_amt;

            // Note - #Step 1j & 1k are not applicable for 2022 form and later.

            // #Step 1l - Initialize the Adjusted Annual Benefit Amount
            icdoPayeeAccountTaxWithholding.adj_annual_benefit_amt = (icdoPayeeAccountTaxWithholding.post_ded_benefit_amt > 0.0m) ? icdoPayeeAccountTaxWithholding.post_ded_benefit_amt : 0.0m;
        }

        private void LoadW4PTaxCalculationValues()
        {
            // 1. Initialize the No. of Periods - #Step 1b
            LoadTaxRef();
            icdoPayeeAccountTaxWithholding.no_of_periods = ibusTaxRefConfig.icdoTaxRefConfig.no_of_periods;
            // 2.1 Initialize Tax Payer Deduction Amount if any - #Step 1g
            //if (string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref)) //SetFilingStatusBasedOnMarriedStatus();
            LoadW4POneGAmtBasedOnFilingStatus();
        }

        private void LoadW4POneGAmtBasedOnFilingStatus()
        {
            if (ibusPayeeAccount.IsNull())
                LoadPayeeAccount();
            if (ibusPayeeAccount.iblnIsFromMSS && string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.filing_status_value))
                icdoPayeeAccountTaxWithholding.filing_status_value = busConstant.FilingStatusSingleOrMrrdFlgSeparately;
            switch (icdoPayeeAccountTaxWithholding.filing_status_value)
            {
                case busConstant.FilingStatusSingleOrMrrdFlgSeparately:
                    icdoPayeeAccountTaxWithholding.tax_payer_deduction_amt = ibusTaxRefConfig.icdoTaxRefConfig.oneg_amt_single;
                    break;
                case busConstant.FilingStatusMrdFilingJointlyOrQualWidower:
                    icdoPayeeAccountTaxWithholding.tax_payer_deduction_amt = ibusTaxRefConfig.icdoTaxRefConfig.oneg_amt_married;
                    break;
                case busConstant.FilingStatusHeadOfHousehold:
                    icdoPayeeAccountTaxWithholding.tax_payer_deduction_amt = ibusTaxRefConfig.icdoTaxRefConfig.oneg_amt_hohh;
                    break;
                default:
                    break;
            }
        }
        private void LoadTaxRef()
        {
            string lstrTaxRef = icdoPayeeAccountTaxWithholding.tax_ref;
            if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax && string.IsNullOrEmpty(lstrTaxRef))
                lstrTaxRef = busConstant.PayeeAccountTaxRefFed22Tax;
            if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax && string.IsNullOrEmpty(lstrTaxRef))
                lstrTaxRef = busConstant.PayeeAccountTaxRefState22Tax;
            if (ibusTaxRefConfig.IsNull())
                ibusTaxRefConfig = new busTaxRefConfig() { icdoTaxRefConfig = new DataObjects.doTaxRefConfig() };
            ibusTaxRefConfig.FindTaxRefConfig(icdoPayeeAccountTaxWithholding.tax_identifier_value, lstrTaxRef, busGlobalFunctions.GetSysManagementBatchDate());
        }
        //PIR 25699  
        //private void SetFilingStatusBasedOnMarriedStatus()
        //{
        //    if ((string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.marital_status_value)) || (icdoPayeeAccountTaxWithholding.marital_status_value == busConstant.PersonMaritalStatusSingle)
        //        || (icdoPayeeAccountTaxWithholding.marital_status_value == busConstant.MaritalStatusMarriedWithholdAtSingleRate))
        //        icdoPayeeAccountTaxWithholding.filing_status_value = busConstant.FilingStatusSingleOrMrrdFlgSeparately;
        //    else if (icdoPayeeAccountTaxWithholding.marital_status_value == busConstant.PersonMaritalStatusMarried)
        //        icdoPayeeAccountTaxWithholding.filing_status_value = busConstant.FilingStatusMrdFilingJointlyOrQualWidower;
        //}

        public void LoadDefaultTaxValues()
        {
            ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO)
            {
                ibusPayeeAccount.LoadTaxableAmountForFlatRates(ibusPayeeAccount.idtNextBenefitPaymentDate, true);
            }
            else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum)
            {
                ibusPayeeAccount.LoadTaxableAmountForFlatRates(ibusPayeeAccount.idtNextBenefitPaymentDate, false);
            }
            else if (icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD)
            {
                ibusPayeeAccount.LoadTaxableAmountForFlatRates(ibusPayeeAccount.idtNextBenefitPaymentDate, false, true);
            }
            if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
            {
                icdoPayeeAccountTaxWithholding.refund_state_amt = busPayeeAccountHelper.CalculateFlatTax(ibusPayeeAccount.idecTotalTaxableAmountForFlatRates, ibusPayeeAccount.idtNextBenefitPaymentDate, icdoPayeeAccountTaxWithholding.tax_identifier_value, icdoPayeeAccountTaxWithholding.tax_ref, icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD ? busConstant.Flag_Yes : busConstant.Flag_No);
            }
            else if (icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
            {
                icdoPayeeAccountTaxWithholding.refund_fed_percent = busPayeeAccountHelper.CalculateFlatTaxValues(ibusPayeeAccount.idtNextBenefitPaymentDate, icdoPayeeAccountTaxWithholding.tax_identifier_value, icdoPayeeAccountTaxWithholding.tax_ref, icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD ? busConstant.Flag_Yes : busConstant.Flag_No);
            }
        }

        public decimal CalculateW4PTaxBridgeWithholdingAmount(decimal adexTaxableAmt, DateTime adteEffectiveDate)
        {
            // 1. Do not run Tax Calculation when Do not Withhold flag is set. 
            if (!string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.no_fed_withholding) && icdoPayeeAccountTaxWithholding.no_fed_withholding == busConstant.Flag_Yes)
                return busConstant.ZeroDecimal;

            // 2. Initialize Pre-Tax calculation values
            LoadW4PTaxCalculationValues();


            // 3. Calculate the Taxable Benefit Amount - Steps 1 from W4P
            CalculateBridgeTaxableBenefitAmount(adexTaxableAmt);

            // 4. Load Tax Calculation Effective Date.
            LoadBridgeTaxCalculationEffectiveDate(adteEffectiveDate);

            // 5. Calculate the Tentative Annual Withholding Amount - Steps 2 from W4P 
            CalculateBridgeTentativeAnnualWithholdingAmount();

            // 6. Calculate Tax Credit Amount - Steps 3 from W4P 
            CalculateBridgeTaxCreditAmount();

            // 7. Calculate Tax Withholding Amount - Steps 4 from W4P
            CalculateBridgeWithholdingAmount();
            //8. Persist Withholding
            if (!iblnSkipSave)
            {
                if (icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id == 0)
                    icdoPayeeAccountTaxWithholding.Insert();
                else
                    icdoPayeeAccountTaxWithholding.Update();
            }
            return icdoPayeeAccountTaxWithholding.fed_tax_amt;
        }

        /// <summary>
        /// Method to calculate the taxable Benefit Amount
        /// </summary>
        private void CalculateBridgeTaxableBenefitAmount(decimal adecTaxableAmount)
        {
            // #Step 1a - 
            // #Step 1c - Calculate Annual Benefit Amount
            icdoPayeeAccountTaxWithholding.annual_benefit_amt = adecTaxableAmount * icdoPayeeAccountTaxWithholding.no_of_periods;

            // #Step 1k - Compute Calculated Allowance Amount
            icdoPayeeAccountTaxWithholding.calc_allowance_amt = ibusTaxRefConfig.icdoTaxRefConfig.default_allowance_amt * icdoPayeeAccountTaxWithholding.tax_allowance;

            // #Step 1l - Compute Post Allowance Benefit Amount
            icdoPayeeAccountTaxWithholding.post_allowance_benefit_amt = icdoPayeeAccountTaxWithholding.annual_benefit_amt - icdoPayeeAccountTaxWithholding.calc_allowance_amt;
            if (icdoPayeeAccountTaxWithholding.post_allowance_benefit_amt < busConstant.ZeroDecimal)
                icdoPayeeAccountTaxWithholding.post_allowance_benefit_amt = busConstant.ZeroDecimal;

            // #Step 1l - Initialize the Adjusted Annual Benefit Amount
            icdoPayeeAccountTaxWithholding.adj_annual_benefit_amt = icdoPayeeAccountTaxWithholding.post_allowance_benefit_amt;
        }
        /// <summary>
        /// Method to calculate tentative Annual Withholding Amount using the W4P Tax Rate table
        /// </summary>
        private void CalculateBridgeTentativeAnnualWithholdingAmount()
        {
            DataTable ldtbTaxRateResult = busBase.Select(busConstant.GetTaxRateBasedOnEffecDate, new object[5] {
                                                    icdoPayeeAccountTaxWithholding.tax_identifier_value,
                                                    string.IsNullOrEmpty(icdoPayeeAccountTaxWithholding.tax_ref) ? busConstant.PayeeAccountTaxRefFed22Tax : icdoPayeeAccountTaxWithholding.tax_ref,
                                                    icdoPayeeAccountTaxWithholding.filing_status_value,
                                                    icdoPayeeAccountTaxWithholding.idteTaxCalcultionEffectiveDate,
                                                    icdoPayeeAccountTaxWithholding.adj_annual_benefit_amt
                                                });
            if (ldtbTaxRateResult.IsNotNull() && ldtbTaxRateResult.Rows.Count > busConstant.ZeroInt)
            {
                // Initialize the W4 Tax Rate object
                busFedStateTaxRate lbusFedStateTaxRate = new busFedStateTaxRate() { icdoFedStateTaxRate = new cdoFedStateTaxRate() };
                lbusFedStateTaxRate.icdoFedStateTaxRate.LoadData(ldtbTaxRateResult.Rows[busConstant.ZeroInt]);

                // #Step 2c
                icdoPayeeAccountTaxWithholding.exceeding_tax_amt = lbusFedStateTaxRate.icdoFedStateTaxRate.minimum_amount;

                // #Step 2d
                icdoPayeeAccountTaxWithholding.tentative_tax_amt = lbusFedStateTaxRate.icdoFedStateTaxRate.tax_amount;

                // #Step 2e
                icdoPayeeAccountTaxWithholding.tax_percentage = lbusFedStateTaxRate.icdoFedStateTaxRate.percentage;

                // #Step 2f
                icdoPayeeAccountTaxWithholding.calc_taxable_amt = icdoPayeeAccountTaxWithholding.adj_annual_benefit_amt - icdoPayeeAccountTaxWithholding.exceeding_tax_amt;

                // #Step 2g
                icdoPayeeAccountTaxWithholding.calc_taxable_perc_amt = icdoPayeeAccountTaxWithholding.calc_taxable_amt * icdoPayeeAccountTaxWithholding.tax_percentage / busConstant.HundredDecimal;

                // #Step 2h
                icdoPayeeAccountTaxWithholding.monthly_taxable_amt = icdoPayeeAccountTaxWithholding.calc_taxable_perc_amt + icdoPayeeAccountTaxWithholding.tentative_tax_amt;

                // #Step 2s
                icdoPayeeAccountTaxWithholding.tentative_annual_withhold_amt = icdoPayeeAccountTaxWithholding.monthly_taxable_amt;
            }

        }


        /// <summary>
        /// Method to calculate Tax Credit Amount
        /// </summary>
        private void CalculateBridgeTaxCreditAmount()
        {
            // #Step 3b
            icdoPayeeAccountTaxWithholding.taxable_credit_amt = icdoPayeeAccountTaxWithholding.tentative_annual_withhold_amt;
        }

        /// <summary>
        /// Method to calculate Withholding Amount
        /// </summary>
        private void CalculateBridgeWithholdingAmount()
        {
            if (icdoPayeeAccountTaxWithholding.no_of_periods == 0)
                icdoPayeeAccountTaxWithholding.no_of_periods = busConstant.Twelve;
            // #Step 4a
            icdoPayeeAccountTaxWithholding.calc_withhold_amt = icdoPayeeAccountTaxWithholding.taxable_credit_amt / icdoPayeeAccountTaxWithholding.no_of_periods;

            // #Step 4c
            icdoPayeeAccountTaxWithholding.fed_tax_amt = Math.Round(icdoPayeeAccountTaxWithholding.calc_withhold_amt + icdoPayeeAccountTaxWithholding.additional_tax_amount, 2, MidpointRounding.AwayFromZero);
        }
        public bool IsFedTaxPercentLessThanMinimum()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            busPayeeAccountHelper.LoadFedStateFlatTaxRates();
            busFedStateFlatTaxRate lbusFedStateFlatTaxRate = busPayeeAccountHelper.iclbFedStatFlatTax
                                                            .OrderByDescending(tax => tax.icdoFedStateFlatTaxRate.effective_date)
                                                            .FirstOrDefault(tax => tax.icdoFedStateFlatTaxRate.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax &&
                                                                                    tax.icdoFedStateFlatTaxRate.effective_date <= ibusPayeeAccount.idtNextBenefitPaymentDate &&
                                                                                    tax.icdoFedStateFlatTaxRate.tax_ref == icdoPayeeAccountTaxWithholding.tax_ref &&
                                                                                    tax.icdoFedStateFlatTaxRate.is_rmd == busConstant.Flag_No);
            if (lbusFedStateFlatTaxRate.IsNotNull())
            {
                return icdoPayeeAccountTaxWithholding.refund_fed_percent < lbusFedStateFlatTaxRate.icdoFedStateFlatTaxRate.min_tax_percentage; 
            }
            return false;
        }
        public decimal idecMinFedTaxPercentage
        {
            get
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                busPayeeAccountHelper.LoadFedStateFlatTaxRates();
                busFedStateFlatTaxRate lbusFedStateFlatTaxRate = busPayeeAccountHelper.iclbFedStatFlatTax
                                                                .OrderByDescending(tax => tax.icdoFedStateFlatTaxRate.effective_date)
                                                                .FirstOrDefault(tax => tax.icdoFedStateFlatTaxRate.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax &&
                                                                                        tax.icdoFedStateFlatTaxRate.effective_date <= ibusPayeeAccount.idtNextBenefitPaymentDate &&
                                                                                        tax.icdoFedStateFlatTaxRate.tax_ref == icdoPayeeAccountTaxWithholding.tax_ref &&
                                                                                        tax.icdoFedStateFlatTaxRate.is_rmd == busConstant.Flag_No);
                if (lbusFedStateFlatTaxRate.IsNotNull())
                {
                    return lbusFedStateFlatTaxRate.icdoFedStateFlatTaxRate.min_tax_percentage;
                }
                return 0.0M;
            }
        }
        public decimal idecDeductionAmt { get; set; }
        public decimal idecEstimatedFedTaxAmt { get; set; }
        public ArrayList CalculateEstimateTax()
        {

            ArrayList alReturn = new ArrayList();
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            DateTime ldteTaxAsOfDate = DateTime.MinValue;
            
            //Get the Taxable amount for Non -Refund Benefit type
            //Always reload the amount as it is being called for the same payee account more than once
            if (ibusPayeeAccount.idtStatusEffectiveDate != DateTime.MinValue && ibusPayeeAccount.iblnIsColaOrAdhocIncreaseTaxCalculation)
            {
                ibusPayeeAccount.LoadTaxableAmountForVariableTax(ibusPayeeAccount.idtStatusEffectiveDate);
                ldteTaxAsOfDate = ibusPayeeAccount.idtStatusEffectiveDate;
            }
            else
            {
                ibusPayeeAccount.LoadTaxableAmountForVariableTax(ibusPayeeAccount.idtNextBenefitPaymentDate);
                ldteTaxAsOfDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
            }
            iblnSkipSave = true;
            idecEstimatedFedTaxAmt = CalculateW4PTaxWithholdingAmount(ibusPayeeAccount.idecTotalTaxableAmountForVariableTax, ldteTaxAsOfDate);
            iblnSkipSave = false;
            //PIR 25699 Added validation when click on EstimateTax button no selection made in Step 1 c Filing Status.
            if (icdoPayeeAccountTaxWithholding.filing_status_value.IsNullOrEmpty())
            {
                utlError lobjError = new utlError();
                lobjError = AddError(10473, busGlobalFunctions.GetMessageTextByMessageID(10473, iobjPassInfo));
                alReturn.Add(lobjError);
                return alReturn;
            }
            return alReturn;
        }
        //PIR 25699 
        //for visibility rules of New Fed and State Tax buttons
        public bool IsMonthTaxButtonVisible()
        {
            if (this.ibusPayeeAccount.IsNull())
                this.LoadPayeeAccount();
            if ((this.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && !IsBenefitoptionDeathRefund()) ||
                (this.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && !IsBenefitoptionDeathRefund()) ||
                this.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement ||
                this.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                return true;
            }
            return false;
        }
        private bool IsBenefitoptionDeathRefund()
        {
            if (this.ibusPayeeAccount.IsNull())
                this.LoadPayeeAccount();
            if (this.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund
                || this.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionAutoRefund)
            {
                return true;
            }
            return false;
        }
        public bool IsTaxWithholdingRefundVisible()
        {
            if (this.ibusPayeeAccount.IsNull())
                this.LoadPayeeAccount();
            if (this.ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsNull())
                this.ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            return (((this.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && IsBenefitoptionDeathRefund()) ||
                (this.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && IsBenefitoptionDeathRefund()) ||
                this.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund) && (this.ibusPayeeAccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusReview ||
                this.ibusPayeeAccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusApproved));
        }
    }
}
