#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using System.Linq;
using Sagitec.ExceptionPub;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvPayment : srvNeoSpin
    {
        public srvPayment()
        {
        }
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
            //iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == "wfmPayeeAccountMaintenance")
                {
                    busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
                    larrErrors = lbusPayeeAccount.ValidateNew(ahstParam);
                }
                if (astrFormName == "wfmPostRetirementIncreaseBatchRequestLookup")
                {
                    busPostRetirementIncreaseBatchRequestLookup lbusPsotRetirementIncreaseBatchRequest = new busPostRetirementIncreaseBatchRequestLookup();
                    larrErrors = lbusPsotRetirementIncreaseBatchRequest.ValidateNew(ahstParam);
                }
            }
            finally
            {
               // iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }
        public busBenefitAccount FindBenefitAccount(int Aintbenefitaccountid)
        {
            busBenefitAccount lobjBenefitAccount = new busBenefitAccount();
            if (lobjBenefitAccount.FindBenefitAccount(Aintbenefitaccountid))
            {
            }

            return lobjBenefitAccount;
        }

        public busBenefitTypeItemType FindBenefitTypeItemType(int Aintbenefittypeitemtypeid)
        {
            busBenefitTypeItemType lobjBenefitTypeItemType = new busBenefitTypeItemType();
            if (lobjBenefitTypeItemType.FindBenefitTypeItemType(Aintbenefittypeitemtypeid))
            {
            }

            return lobjBenefitTypeItemType;
        }

        public busPayeeAccount FindPayeeAccount(int Aintpayeeaccountid)
        {
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            if (lobjPayeeAccount.FindPayeeAccount(Aintpayeeaccountid))
            {
                lobjPayeeAccount.ibusPayeeAccountActiveStatus = new busPayeeAccountStatus();
                lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                lobjPayeeAccount.LoadPayee();
                lobjPayeeAccount.LoadApplication();
                lobjPayeeAccount.LoadDROApplication();
                lobjPayeeAccount.ibusApplication.LoadPersonAccount();
                lobjPayeeAccount.LoadPlan();
                lobjPayeeAccount.LoadMember();
                //For Correspondence ucs -073 - COR-073-3- State Tax Withheld for Out of State Addresses
                if (lobjPayeeAccount.ibusPayee != null)
                {
                    lobjPayeeAccount.ibusPayee.LoadPersonCurrentAddress();
                }
                lobjPayeeAccount.LoadStateTaxWithHoldingInfo();
                lobjPayeeAccount.LoadFedTaxWithHoldingInfo();
                lobjPayeeAccount.LoadTaxWithHoldingHistory();
                lobjPayeeAccount.LoadACHDetail();
                lobjPayeeAccount.LoadRolloverDetail();
                lobjPayeeAccount.LoadRetroPayment();
                lobjPayeeAccount.LoadLastBenefitPaymentDate();
                lobjPayeeAccount.LoadRetroTaxableAndNonTaxableAmount();
                lobjPayeeAccount.LoadPayeeAccountStatus();
                lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                lobjPayeeAccount.LoadBenefitAmountForPayee();
                lobjPayeeAccount.LoadDeductions();
                lobjPayeeAccount.LoadMontlyBenefits();
                lobjPayeeAccount.LoadTaxabilityBenefits();
                lobjPayeeAccount.LoadGrossAmount();
                lobjPayeeAccount.LoadGrossBenefitAmount();
                //prod pir 1453 : to load ssli after change amount
                lobjPayeeAccount.LoadSSLIAfterChange();
                lobjPayeeAccount.LoadTotalDeductionAmount();
                lobjPayeeAccount.LoadBenfitAccount();
                lobjPayeeAccount.ibusBenefitAccount.LoadRetirementOrg();
                lobjPayeeAccount.LoadExclusionAmount();
                lobjPayeeAccount.LoadBenefitCalculation();
                lobjPayeeAccount.LoadBenefitRefundCalculation();    //PIR 25920 DC 2025 changes
                lobjPayeeAccount.CalculateMinimumGuaranteeAmount();
                lobjPayeeAccount.LoadBalanceNontaxableAmount();
                // UCS-080 Converting Disability to Normal Payee Account
                lobjPayeeAccount.LoadRetirementBenefitCalculation();

                // UCS-082 Load the Recertification Date
                lobjPayeeAccount.LoadRecertificationDate();

                lobjPayeeAccount.LoadErrors();
                lobjPayeeAccount.LoadPaymentDetails();

                if (lobjPayeeAccount.iclbCase.IsNull())
                    lobjPayeeAccount.LoadCase();
                lobjPayeeAccount.LoadMinimumGuaranteeAmount();
                lobjPayeeAccount.LoadNontaxableBeginningBalnce();
                //UCS - 079 -- Loading benefit overpayment
                lobjPayeeAccount.LoadBenefitOverPaymentHeader();
                lobjPayeeAccount.LoadPaymentRecoveryWithoutBenefitOverpaymentHeader();
                //UCS - 091 -- Loading 1099r history
                lobjPayeeAccount.LoadPayment1099r();
                lobjPayeeAccount.LoadNetPLSO();

                if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    lobjPayeeAccount.GetAccountBalancesForDisability();
                lobjPayeeAccount.iblnSpecialCheckIncludeInAdhocFlag = true;

                //PIR 26896 - Pull Check Flag should be checked for Y and S
                if (lobjPayeeAccount.icdoPayeeAccount.pull_check_flag == busConstant.Flag_No)
                    lobjPayeeAccount.istrPullCheckFlag = busConstant.Flag_No;
                else if (lobjPayeeAccount.icdoPayeeAccount.pull_check_flag == busConstant.Flag_Yes || lobjPayeeAccount.icdoPayeeAccount.pull_check_flag == busConstant.Flag_System)
                    lobjPayeeAccount.istrPullCheckFlag = busConstant.Flag_Yes;
            }
            return lobjPayeeAccount;
        }

        public busPayeeAccountAchDetail NewPayeeAccountAchDetail(int Aintpayeeaccountachdetailid, int Aintpayeeaccountid)
        {
            busPayeeAccountAchDetail lobjPayeeAccountAchDetail = new busPayeeAccountAchDetail();

            if (Aintpayeeaccountachdetailid == 0)
            {
                //This block of code is for btnNewAchDetail on PayeeAccountMaintenance Screen
                lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail();
                lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_id = Aintpayeeaccountid;
                lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.pre_note_flag = busConstant.Flag_Yes;
                lobjPayeeAccountAchDetail.LoadPayeeAccount();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadActiveACHDetail();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadPrimaryACHDetail(lobjPayeeAccountAchDetail.ibusPayeeAccount.idtNextBenefitPaymentDate);
                lobjPayeeAccountAchDetail.SetPrimaryOrSecordaryAch();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadBenefitAmount();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadApplication();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadMember();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadPayee();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadTotalTaxableAmount();
            }
            else
            {
                if (lobjPayeeAccountAchDetail.FindPayeeAccountAchDetail(Aintpayeeaccountachdetailid))
                {
                    //This block of code is for btnCopyAch on PayeeAccountAchDetailMaitenace Screen
                    lobjPayeeAccountAchDetail.LoadPayeeAccount();
                    lobjPayeeAccountAchDetail.LoadBankOrgByOrgID();
                    lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadActiveACHDetail();
                    lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadPrimaryACHDetail(lobjPayeeAccountAchDetail.ibusPayeeAccount.idtNextBenefitPaymentDate);
                    lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadBenefitAmount();
                    lobjPayeeAccountAchDetail.SetPrimaryOrSecordaryAch();
                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.org_code = lobjPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_code;
                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = DateTime.MinValue;
                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.MinValue;
                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_ach_detail_id = 0;
                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ienuObjectState = ObjectState.Insert;
                    lobjPayeeAccountAchDetail.iarrChangeLog.Add(lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail);
                    lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadApplication();
                    lobjPayeeAccountAchDetail.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
                    lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadMember();
                    lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadPayee();
                    lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadTotalTaxableAmount();
                }
            }
            lobjPayeeAccountAchDetail.EvaluateInitialLoadRules();
            return lobjPayeeAccountAchDetail;
        }

        public busPayeeAccountAchDetail FindPayeeAccountAchDetail(int Aintpayeeaccountachdetailid)
        {
            busPayeeAccountAchDetail lobjPayeeAccountAchDetail = new busPayeeAccountAchDetail();
            if (lobjPayeeAccountAchDetail.FindPayeeAccountAchDetail(Aintpayeeaccountachdetailid))
            {
                lobjPayeeAccountAchDetail.LoadPayeeAccount();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadTotalTaxableAmount();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadBenefitAmount();
                lobjPayeeAccountAchDetail.LoadBankOrgByOrgID();
                lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.org_code = lobjPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_code;
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadApplication();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadMember();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadPayee();
                lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadACHDetail();
            }
            return lobjPayeeAccountAchDetail;
        }

        public busPayeeAccountPaymentItemType FindPayeeAccountPaymentItemType(int Aintpayeeaccountpaymentitemtypeid)
        {
            busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType();
            if (lobjPayeeAccountPaymentItemType.FindPayeeAccountPaymentItemType(Aintpayeeaccountpaymentitemtypeid))
            {
            }
            return lobjPayeeAccountPaymentItemType;
        }

        public busMiscellaneousCorrection NewPAPIT(int aintPayeeAccountID)
        {
            busMiscellaneousCorrection lobjPAPIT = new busMiscellaneousCorrection { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            if (lobjPAPIT.LoadPAPITHistory(aintPayeeAccountID))
            {
                lobjPAPIT.icdoPayeeAccountPaymentItemType.payee_account_id = aintPayeeAccountID;
                lobjPAPIT.LoadPayeeAccount();               
                lobjPAPIT.ibusPayeeAccount.LoadApplication();
				lobjPAPIT.ibusPayeeAccount.LoadPayee();
                lobjPAPIT.ibusPayeeAccount.LoadMember();
                lobjPAPIT.ibusPayeeAccount.LoadBenefitAmount();
                lobjPAPIT.ibusPayeeAccount.LoadTotalTaxableAmount();
				lobjPAPIT.iintUserSerialID = iobjPassInfo.iintUserSerialID;
                lobjPAPIT.EvaluateInitialLoadRules();
            }
            return lobjPAPIT;
        }

        public busMiscellaneousCorrection FindPAPITbyPayeeAccountID(int aintPAPITId, int aintPayeeAccountID)
        {
            busMiscellaneousCorrection lobjPAPIT = new busMiscellaneousCorrection { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            if (lobjPAPIT.FindPayeeAccountPaymentItemType(aintPAPITId))
            {
                lobjPAPIT.LoadPaymentItemType();
                lobjPAPIT.LoadPayeeAccount();
                lobjPAPIT.LoadPAPITHistory(aintPayeeAccountID);                
                lobjPAPIT.ibusPayeeAccount.LoadApplication();
				lobjPAPIT.ibusPayeeAccount.LoadPayee();
                lobjPAPIT.ibusPayeeAccount.LoadMember();
                lobjPAPIT.ibusPayeeAccount.LoadBenefitAmount();
                lobjPAPIT.ibusPayeeAccount.LoadTotalTaxableAmount();
				lobjPAPIT.iintUserSerialID = iobjPassInfo.iintUserSerialID;
            }
            return lobjPAPIT;
        }

        public busPayeeAccountStatus NewPayeeAccountStatus(int Aintpayeeaccountid)
        {
            busPayeeAccountStatus lobjPayeeAccountStatus = new busPayeeAccountStatus();
            lobjPayeeAccountStatus.icdoPayeeAccountStatus = new cdoPayeeAccountStatus();
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountStatus.LoadPayeeAccount();
            lobjPayeeAccountStatus.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountStatus.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
            lobjPayeeAccountStatus.ibusPayeeAccount.LoadMember();
            lobjPayeeAccountStatus.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountStatus.ibusPayeeAccount.LoadTotalTaxableAmount();
            lobjPayeeAccountStatus.ibusPayeeAccount.LoadBenefitAmount();
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = DateTime.Today;
            lobjPayeeAccountStatus.EvaluateInitialLoadRules();
            return lobjPayeeAccountStatus;
        }

        public busPayeeAccountStatus FindPayeeAccountStatus(int Aintpayeeaccountstatusid)
        {
            busPayeeAccountStatus lobjPayeeAccountStatus = new busPayeeAccountStatus();
            if (lobjPayeeAccountStatus.FindPayeeAccountStatus(Aintpayeeaccountstatusid))
            {
                lobjPayeeAccountStatus.LoadPayeeAccount();
                lobjPayeeAccountStatus.ibusPayeeAccount.LoadApplication();
                lobjPayeeAccountStatus.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
                lobjPayeeAccountStatus.ibusPayeeAccount.LoadMember();
                lobjPayeeAccountStatus.ibusPayeeAccount.LoadPayee();
                lobjPayeeAccountStatus.ibusPayeeAccount.LoadTotalTaxableAmount();
                lobjPayeeAccountStatus.ibusPayeeAccount.LoadBenefitAmount();
            }
            return lobjPayeeAccountStatus;
        }

        public busPaymentItemType FindPaymentItemType(int Aintpaymentitemtypeid)
        {
            busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
            if (lobjPaymentItemType.FindPaymentItemType(Aintpaymentitemtypeid))
            {
            }
            return lobjPaymentItemType;
        }

        public busPlanItemType FindPlanItemType(int Aintplanitemtypeid)
        {
            busPlanItemType lobjPlanItemType = new busPlanItemType();
            if (lobjPlanItemType.FindPlanItemType(Aintplanitemtypeid))
            {
            }

            return lobjPlanItemType;
        }
        public busPayeeAccountRetroPayment NewPayeeAccountRetroPayment(int AintPayeeAccountid, string astrRetroPaymentType)
        {
            busPayeeAccountRetroPayment lobjPayeeAccountRetroPayment = new busPayeeAccountRetroPayment();
            lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment();
            lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payee_account_id = AintPayeeAccountid;
            lobjPayeeAccountRetroPayment.LoadPayeeAccount();
            lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountRetroPayment.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
            lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadMember();
            lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadTotalTaxableAmount();
            lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadBenefitAmount();

            if (astrRetroPaymentType != null && astrRetroPaymentType == busConstant.RetroPaymentTypePopupBenefits)
            {
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_value = busConstant.RetroPaymentTypePopupBenefits;
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_description
                    = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2204, lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_value);
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.adjustment_reason_value = busConstant.AdjustmentReasonPopUp;
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.adjustment_reason_description
                    = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2901, lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.adjustment_reason_value);
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.is_online_flag = busConstant.Flag_Yes;
            }
            //uat pir 1369 : in new mode only rhic benefit reimbursement should be allowed, other than pop up benefit
            else if (astrRetroPaymentType != null && astrRetroPaymentType == busConstant.RetroPaymentTypeRHICBenefitReimbursement)
            {
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_value = busConstant.RetroPaymentTypeRHICBenefitReimbursement;
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_description
                    = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2204, lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_value);
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.is_online_flag = busConstant.Flag_Yes;
            }
            else if (astrRetroPaymentType != null && astrRetroPaymentType == busConstant.RetroPaymentTypeBenefitUnderpayment)
            {
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_value = busConstant.RetroPaymentTypeBenefitUnderpayment;
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_description
                    = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2204, lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_value);
                lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.is_online_flag = busConstant.Flag_Yes;
                lobjPayeeAccountRetroPayment.InitializePayeeAccountRetroPaymentDetail();
            }

            //UAT PIR - 1281 : Default payment option to regular
            lobjPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payment_option_value = busConstant.PayeeAccountRetroPaymentOptionRegular;
            lobjPayeeAccountRetroPayment.iblnSpecialCheckIncludeInAdhocFlag = true;
            lobjPayeeAccountRetroPayment.EvaluateInitialLoadRules();
            return lobjPayeeAccountRetroPayment;
        }

        public busPayeeAccountRetroPayment FindPayeeAccountRetroPayment(int AintPayeeAccountRetroPaymentid)
        {
            busPayeeAccountRetroPayment lobjPayeeAccountRetroPayment = new busPayeeAccountRetroPayment();
            if (lobjPayeeAccountRetroPayment.FindPayeeAccountRetroPayment(AintPayeeAccountRetroPaymentid))
            {
                lobjPayeeAccountRetroPayment.LoadPayeeAccount();
                lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadApplication();
                lobjPayeeAccountRetroPayment.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
                lobjPayeeAccountRetroPayment.LoadPayeeAccountRetroPaymentDetail();
                lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadMember();
                lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadPayee();
                lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadTotalTaxableAmount();
                lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadBenefitAmount();
                lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadRetroPayment();
                lobjPayeeAccountRetroPayment.ibusPayeeAccount.LoadRetroTaxableAndNonTaxableAmount();
                lobjPayeeAccountRetroPayment.iblnSpecialCheckIncludeInAdhocFlag = true;


            }
            return lobjPayeeAccountRetroPayment;
        }

        public busPayeeAccountLookup LoadPayeeAccounts(DataTable adtbSearchResult)
        {
            busPayeeAccountLookup lobjPayeeAccountLookup = new busPayeeAccountLookup();
            lobjPayeeAccountLookup.LoadPayeeAccounts(adtbSearchResult);
            return lobjPayeeAccountLookup;
        }
        //Update Tax Rate Workflow Changes
        public busFedStateTaxRate NewStateTax()
        {
            busFedStateTaxRate lobjFedTax = new busFedStateTaxRate();
            lobjFedTax.icdoFedStateTaxRate = new cdoFedStateTaxRate();
            lobjFedTax.icdoFedStateTaxRate.tax_identifier_value = busConstant.PayeeAccountTaxIdentifierStateTax;
            return lobjFedTax;
        }
        //Update Tax Rate Workflow Changes
        public busFedStateTaxRate NewFedTax()
        {
            busFedStateTaxRate lobjStateTax = new busFedStateTaxRate();
            lobjStateTax.icdoFedStateTaxRate = new cdoFedStateTaxRate();
            lobjStateTax.icdoFedStateTaxRate.tax_identifier_value = busConstant.PayeeAccountTaxIdentifierFedTax;
            return lobjStateTax;
        }
        //Update Tax Rate Workflow Changes
        public busFedStateTaxRate FindFedStateTax(int AintStateTaxID)
        {
            busFedStateTaxRate lobjStateTax = new busFedStateTaxRate();
            if (lobjStateTax.FindFedStateTaxRate(AintStateTaxID))
            {

            }
            return lobjStateTax;
        }

        public busPayeeAccountRolloverDetail NewPayeeAccountRolloverDetail(int Aintpayeeaccountid, string ablnIsValidateNewTrue)
        {
            busPayeeAccountRolloverDetail lobjPayeeAccountRolloverDetail = new busPayeeAccountRolloverDetail();
            lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail = new cdoPayeeAccountRolloverDetail();
            lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountRolloverDetail.LoadPayeeAccount();
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadRolloverDetail();
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadMember();
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadBenefitAmount();
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadNexBenefitPaymentDate();
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadTaxableAmountForFlatRates(lobjPayeeAccountRolloverDetail.ibusPayeeAccount.idtNextBenefitPaymentDate,
                (lobjPayeeAccountRolloverDetail.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plso_requested_flag == busConstant.Flag_Yes) ? true : false);
            lobjPayeeAccountRolloverDetail.ibusPayeeAccount.iblnIsRollOverFromScreen = true;
            lobjPayeeAccountRolloverDetail.LoadMemberRequestedRolloverDetail(); //PIR 18493 - Show requested rollover detail
            lobjPayeeAccountRolloverDetail.SetPayeeAccountRolloverDetail();
            lobjPayeeAccountRolloverDetail.EvaluateInitialLoadRules();
            lobjPayeeAccountRolloverDetail.RolloverFBOName();
            return lobjPayeeAccountRolloverDetail;
        }

        public busPayeeAccountRolloverDetail FindPayeeAccountRolloverDetail(int Aintpayeeaccountrolloverinfoid)
        {
            busPayeeAccountRolloverDetail lobjPayeeAccountRolloverDetail = new busPayeeAccountRolloverDetail();
            if (lobjPayeeAccountRolloverDetail.FindPayeeAccountRolloverDetail(Aintpayeeaccountrolloverinfoid))
            {
                lobjPayeeAccountRolloverDetail.LoadPayeeAccount();
                lobjPayeeAccountRolloverDetail.LoadRolloverOrgByOrgID();
                lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.org_code
                    = lobjPayeeAccountRolloverDetail.ibusRolloverOrg.icdoOrganization.org_code;
                lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadRolloverDetail();
                lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadApplication();
                lobjPayeeAccountRolloverDetail.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
                lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadMember();
                lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadPayee();
                lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadBenefitAmount();
                lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadNexBenefitPaymentDate();
                lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadApplication();
                lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                lobjPayeeAccountRolloverDetail.ibusPayeeAccount.LoadTaxableAmountForFlatRates(lobjPayeeAccountRolloverDetail.ibusPayeeAccount.idtNextBenefitPaymentDate,
                    (lobjPayeeAccountRolloverDetail.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plso_requested_flag == busConstant.Flag_Yes) ? true : false);
                lobjPayeeAccountRolloverDetail.LoadMemberRequestedRolloverDetail(); //PIR 18493 - Show requested rollover detail
                lobjPayeeAccountRolloverDetail.SetPayeeAccountRolloverDetail();
                lobjPayeeAccountRolloverDetail.EvaluateInitialLoadRules();
                lobjPayeeAccountRolloverDetail.RolloverFBOName();
            }
            return lobjPayeeAccountRolloverDetail;
        }

        public busPayeeAccountPaymentItemType NewDeduction(int AintPayeeAccountID)
        {
            busPayeeAccountPaymentItemType lobjDeduction = new busPayeeAccountPaymentItemType();
            lobjDeduction.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
            lobjDeduction.ibusPayeeAccountDeductionRefund = new busPayeeAccountDeductionRefund();
            lobjDeduction.ibusPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund = new cdoPayeeAccountDeductionRefund();
            lobjDeduction.icdoPayeeAccountPaymentItemType.payee_account_id = AintPayeeAccountID;
            lobjDeduction.LoadPayeeAccount();
            lobjDeduction.ibusPayeeAccount.LoadPayee();
            lobjDeduction.LoadDeductionHistory();
            lobjDeduction.ibusPayeeAccount.LoadApplication();            
            lobjDeduction.ibusPayeeAccount.LoadMember();
            lobjDeduction.ibusPayeeAccount.LoadBenefitAmount();
            lobjDeduction.ibusPayeeAccount.LoadTotalTaxableAmount();
            //prod pir 7312 : to set object state to insert
            lobjDeduction.icdoPayeeAccountPaymentItemType.ienuObjectState = ObjectState.Insert;
            lobjDeduction.EvaluateInitialLoadRules();
            return lobjDeduction;
        }

        public busPayeeAccountPaymentItemType FindDeduction(int AintPayeeAccountPaymentItemTypeID)
        {
            busPayeeAccountPaymentItemType lobjPAPIT = new busPayeeAccountPaymentItemType();
            lobjPAPIT.ibusPayeeAccountDeductionRefund = new busPayeeAccountDeductionRefund();
            if (lobjPAPIT.FindPayeeAccountPaymentItemType(AintPayeeAccountPaymentItemTypeID))
            {
                lobjPAPIT.ibusPayeeAccountDeductionRefund.FindPayeeAccountDeductionRefundByPayeeAccountPaymentItemID(AintPayeeAccountPaymentItemTypeID);
                lobjPAPIT.LoadPayeeAccount();
                lobjPAPIT.LoadPaymentItemType();
                lobjPAPIT.ibusPayeeAccount.LoadPayee();
                lobjPAPIT.LoadDeductionHistory();
                lobjPAPIT.LoadVendor();
                lobjPAPIT.icdoPayeeAccountPaymentItemType.vendor_org_code = lobjPAPIT.ibusVendor.icdoOrganization.org_code;
                lobjPAPIT.LoadDeductionHistory();
                lobjPAPIT.ibusPayeeAccount.LoadApplication();               
                lobjPAPIT.ibusPayeeAccount.LoadMember();
                lobjPAPIT.ibusPayeeAccount.LoadBenefitAmount();
                lobjPAPIT.ibusPayeeAccount.LoadTotalTaxableAmount();
            }
            return lobjPAPIT;
        }

        public busPayeeAccountTaxWithholding NewPayeeAccountTaxWithholding(int Aintpayeeaccountid, string astrTaxIdentidifier, string astrDistributionType, string astrTaxRef = null)
        {
            busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_ref = astrTaxRef;
            lobjPayeeAccountTaxWithholding.LoadPayeeAccount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadMember();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusMember.GetPersonLatestAddress();//PIR 25668 display the payee address 
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTotalTaxableAmount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadBenefitAmount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = astrTaxIdentidifier;
            if (astrTaxIdentidifier == busConstant.PayeeAccountTaxIdentifierFedTax)
            {
                if (astrDistributionType == busConstant.BenefitDistributionMonthlyBenefit)
                {
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option = busConstant.FedTaxOptionFedTaxBasedOnIRS;
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value = busConstant.PayeeAccountMaritalStatusMarried;
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_description
                        = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(306, busConstant.PersonMaritalStatusMarried);
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.no_of_tax_allowance = busConstant.WithholdingTaxAllowance.ToString();
                }
                else if (astrDistributionType == busConstant.BenefitDistributionLumpSum)
                {
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_tax_option = busConstant.FedTaxOptionFederalTaxwithheld;
                    if (lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_ref == busConstant.PayeeAccountTaxRefFed22Tax)
                    {
                        if (lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadNexBenefitPaymentDate();
                        busPayeeAccountHelper.LoadFedStateFlatTaxRates();
                        if (busPayeeAccountHelper.iclbFedStatFlatTax.Count > 0)
                        {
                            busFedStateFlatTaxRate lbusFedStateFlatTaxRate = busPayeeAccountHelper
                                                                                .iclbFedStatFlatTax
                                                                                .OrderByDescending(flattax => flattax.icdoFedStateFlatTaxRate.effective_date)
                                                                                .FirstOrDefault(flattax => flattax.icdoFedStateFlatTaxRate.tax_identifier_value ==
                                                                               astrTaxIdentidifier &&
                                                                               flattax.icdoFedStateFlatTaxRate.tax_ref == busConstant.PayeeAccountTaxRefFed22Tax &&
                                                                               flattax.icdoFedStateFlatTaxRate.is_rmd == busConstant.Flag_No &&
                                                                               flattax.icdoFedStateFlatTaxRate.effective_date.Date <=
                                                                               lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate.Date);
                            if (lbusFedStateFlatTaxRate.IsNotNull())
                            {
                                lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_fed_percent = lbusFedStateFlatTaxRate.icdoFedStateFlatTaxRate.flat_tax_percentage;
                            }
                        }
                    }
                }
                else if (astrDistributionType == busConstant.BenefitDistributionPSLO)
                {
                    //uat pir 1278 : need to default to Federal tax withheld option
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.plso_tax_option = busConstant.TaxOptionFedTaxwithheld;
                    busPayeeAccountHelper.LoadFedStateFlatTaxRates();
                    if (busPayeeAccountHelper.iclbFedStatFlatTax.Count > 0)
                    {
                        busFedStateFlatTaxRate lbusFedStateFlatTaxRate = busPayeeAccountHelper
                                                                            .iclbFedStatFlatTax
                                                                            .OrderByDescending(flattax => flattax.icdoFedStateFlatTaxRate.effective_date)
                                                                            .FirstOrDefault(flattax => flattax.icdoFedStateFlatTaxRate.tax_identifier_value ==
                                                                           astrTaxIdentidifier &&
                                                                           flattax.icdoFedStateFlatTaxRate.tax_ref == busConstant.PayeeAccountTaxRefFed22Tax &&
                                                                           flattax.icdoFedStateFlatTaxRate.is_rmd == busConstant.Flag_No &&
                                                                           flattax.icdoFedStateFlatTaxRate.effective_date.Date <=
                                                                           lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate.Date);
                        if (lbusFedStateFlatTaxRate.IsNotNull())
                        {
                            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_fed_percent = lbusFedStateFlatTaxRate.icdoFedStateFlatTaxRate.flat_tax_percentage;
                        }
                    }
                }
                else if (astrDistributionType == busConstant.BenefitDistributionRMD)
                {
                    if (lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadNexBenefitPaymentDate();
                    busPayeeAccountHelper.LoadFedStateFlatTaxRates();
                    if (busPayeeAccountHelper.iclbFedStatFlatTax.Count > 0)
                    {
                        busFedStateFlatTaxRate lbusFedStateFlatTaxRate = busPayeeAccountHelper
                                                                            .iclbFedStatFlatTax
                                                                            .OrderByDescending(flattax => flattax.icdoFedStateFlatTaxRate.effective_date)
                                                                            .FirstOrDefault(flattax => flattax.icdoFedStateFlatTaxRate.tax_identifier_value ==
                                                                           astrTaxIdentidifier &&
                                                                           flattax.icdoFedStateFlatTaxRate.tax_ref == busConstant.PayeeAccountTaxRefFed22Tax &&
                                                                           flattax.icdoFedStateFlatTaxRate.is_rmd == busConstant.Flag_Yes &&
                                                                           flattax.icdoFedStateFlatTaxRate.effective_date.Date <=
                                                                           lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate.Date);
                        if (lbusFedStateFlatTaxRate.IsNotNull())
                        {
                            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_fed_percent = lbusFedStateFlatTaxRate.icdoFedStateFlatTaxRate.flat_tax_percentage;
                        }
                    }
                }
            }
            else if (astrTaxIdentidifier == busConstant.PayeeAccountTaxIdentifierStateTax)
            {
                if (astrDistributionType == busConstant.BenefitDistributionMonthlyBenefit)
                {
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option = busConstant.StateTaxOptionNoMonthlyNDTax;
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value = busConstant.PayeeAccountMaritalStatusMarried;
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_description
                        = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(306, busConstant.PersonMaritalStatusMarried);
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.no_of_tax_allowance = busConstant.WithholdingTaxAllowance.ToString();
                }
                else if (astrDistributionType == busConstant.BenefitDistributionPSLO)
                {
                    //uat pir 1278 : need to default to State tax withheld option
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.plso_tax_option = busConstant.TaxOptionStateTaxwithheld;
                    if (lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadNexBenefitPaymentDate();
                    busPayeeAccountHelper.LoadFedStateFlatTaxRates();
                    lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxableAmountForFlatRates(lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate, true);
                    if (busPayeeAccountHelper.iclbFedStatFlatTax.Count > 0)
                    {
                        busFedStateFlatTaxRate lbusFedStateFlatTaxRate = busPayeeAccountHelper
                                                                            .iclbFedStatFlatTax
                                                                            .OrderByDescending(flattax => flattax.icdoFedStateFlatTaxRate.effective_date)
                                                                            .FirstOrDefault(flattax => flattax.icdoFedStateFlatTaxRate.tax_identifier_value ==
                                                                           astrTaxIdentidifier &&
                                                                           flattax.icdoFedStateFlatTaxRate.tax_ref == busConstant.PayeeAccountTaxRefState22Tax &&
                                                                           flattax.icdoFedStateFlatTaxRate.effective_date.Date <=
                                                                           lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate.Date);
                        if (lbusFedStateFlatTaxRate.IsNotNull())
                        {
                            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_state_amt = lbusFedStateFlatTaxRate.icdoFedStateFlatTaxRate.flat_tax_percentage * lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idecTotalTaxableAmountForFlatRates / 100;
                        }
                    }

                }
                else if (astrDistributionType == busConstant.BenefitDistributionLumpSum)
                {
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_tax_option = busConstant.StateTaxOptionNoOnetimeNDTax;
                    if (lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_ref == busConstant.PayeeAccountTaxRefState22Tax)
                    {
                        if (lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadNexBenefitPaymentDate();
                        busPayeeAccountHelper.LoadFedStateFlatTaxRates();
                        lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxableAmountForFlatRates(lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate, false);
                        if (busPayeeAccountHelper.iclbFedStatFlatTax.Count > 0)
                        {
                            busFedStateFlatTaxRate lbusFedStateFlatTaxRate = busPayeeAccountHelper
                                                                                .iclbFedStatFlatTax
                                                                                .OrderByDescending(flattax => flattax.icdoFedStateFlatTaxRate.effective_date)
                                                                                .FirstOrDefault(flattax => flattax.icdoFedStateFlatTaxRate.tax_identifier_value ==
                                                                               astrTaxIdentidifier &&
                                                                               flattax.icdoFedStateFlatTaxRate.tax_ref == busConstant.PayeeAccountTaxRefState22Tax &&
                                                                               flattax.icdoFedStateFlatTaxRate.effective_date.Date <=
                                                                               lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate.Date);
                            if (lbusFedStateFlatTaxRate.IsNotNull())
                            {
                                lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_state_amt = lbusFedStateFlatTaxRate.icdoFedStateFlatTaxRate.flat_tax_percentage * lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idecTotalTaxableAmountForFlatRates / 100;
                            }
                        }
                    }
                }
                else if (astrDistributionType == busConstant.BenefitDistributionRMD)
                {
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_tax_option = busConstant.StateTaxOptionNoOnetimeNDTax;
                    if (lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadNexBenefitPaymentDate();
                    busPayeeAccountHelper.LoadFedStateFlatTaxRates();
                    lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxableAmountForFlatRates(lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate, false, true);
                    if (busPayeeAccountHelper.iclbFedStatFlatTax.Count > 0)
                    {
                        busFedStateFlatTaxRate lbusFedStateFlatTaxRate = busPayeeAccountHelper
                                                                            .iclbFedStatFlatTax
                                                                            .OrderByDescending(flattax => flattax.icdoFedStateFlatTaxRate.effective_date)
                                                                            .FirstOrDefault(flattax => flattax.icdoFedStateFlatTaxRate.tax_identifier_value ==
                                                                           astrTaxIdentidifier &&
                                                                           flattax.icdoFedStateFlatTaxRate.tax_ref == busConstant.PayeeAccountTaxRefState22Tax &&
                                                                           flattax.icdoFedStateFlatTaxRate.effective_date.Date <=
                                                                           lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate.Date);
                        if (lbusFedStateFlatTaxRate.IsNotNull())
                        {
                            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_state_amt = lbusFedStateFlatTaxRate.icdoFedStateFlatTaxRate.flat_tax_percentage * lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idecTotalTaxableAmountForFlatRates / 100;
                        }
                    }
                }
            }
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value = astrDistributionType;
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_description
                = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2224, astrDistributionType);
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxWithHoldingHistory();
            lobjPayeeAccountTaxWithholding.EvaluateInitialLoadRules();
            //F/W Upgrade 15738 - Since no changes were made on the screen, the system is not adding to the changelog nor changing the object state 
            //due to which record not getting saved, child records are throwing foreign key error since parent key not generated
            //so adding them here.
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.ienuObjectState = ObjectState.Insert;
            lobjPayeeAccountTaxWithholding.iarrChangeLog.Add(lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding);
            return lobjPayeeAccountTaxWithholding;
        }

        public busPayeeAccountTaxWithholding FindPayeeAccountTaxWithholding(int Aintpayeeaccounttaxwithholdingid)
        {
            busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
            if (lobjPayeeAccountTaxWithholding.FindPayeeAccountTaxWithholding(Aintpayeeaccounttaxwithholdingid))
            {
                lobjPayeeAccountTaxWithholding.LoadPayeeAccount();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadPayee();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadApplication();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxWithHoldingHistory();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadMember();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusMember.GetPersonLatestAddress(); //PIR 25668 display the payee address 
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTotalTaxableAmount();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadBenefitAmount();
                if (lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
                {
                    if (lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.suppress_warnings_flag == busConstant.Flag_Yes)
                    {
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.monthlybenefit_suppress_warning = busConstant.Flag_Yes;
                    }
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option =
                                            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value;
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.no_of_tax_allowance =
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance.ToString();
                    if (lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadNexBenefitPaymentDate();
                    DateTime ldteTaxEffectiveDate = lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date != DateTime.MinValue ?
                                                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date.GetFirstDayofCurrentMonth() :
                                                    lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idtNextBenefitPaymentDate;
                    if (lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                    {
                        lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxableAmountForVariableTax(ldteTaxEffectiveDate);
                        lobjPayeeAccountTaxWithholding.CalculateW4PTaxWithholdingAmount(lobjPayeeAccountTaxWithholding.ibusPayeeAccount.idecTotalTaxableAmountForVariableTax, ldteTaxEffectiveDate);
                    }
                }
                else if (lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO)
                {
                    if (lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.suppress_warnings_flag == busConstant.Flag_Yes)
                    {
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.plso_supress_warning = busConstant.Flag_Yes;
                    }
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.plso_tax_option =
                                           lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value;
                }
                else
                {
                    if (lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.suppress_warnings_flag == busConstant.Flag_Yes)
                    {
                        lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_supress_warning = busConstant.Flag_Yes;
                    }
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_tax_option =
                                           lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value;
                }
                lobjPayeeAccountTaxWithholding.LoadTaxWithHoldingTaxItems();
            }
            return lobjPayeeAccountTaxWithholding;
        }

        public busFedStateFlatTaxRate NewFedStateFlatTaxRates()
        {
            busFedStateFlatTaxRate lobjFedStateFlatTaxRates = new busFedStateFlatTaxRate();
            lobjFedStateFlatTaxRates.icdoFedStateFlatTaxRate = new cdoFedStateFlatTaxRate();
            return lobjFedStateFlatTaxRates;
        }
        public busFedStateFlatTaxRate FindFedStateFlatTaxRates(int Aintfedstateflattaxid)
        {
            busFedStateFlatTaxRate lobjFedStateFlatTaxRates = new busFedStateFlatTaxRate();
            if (lobjFedStateFlatTaxRates.FindFedStateFlatTaxRate(Aintfedstateflattaxid))
            {
            }
            return lobjFedStateFlatTaxRates;
        }

        public busRetroItemType FindRetroItemType(int Aintretroitemtypeid)
        {
            busRetroItemType lobjRetroItemType = new busRetroItemType();
            if (lobjRetroItemType.FindRetroItemType(Aintretroitemtypeid))
            {
            }

            return lobjRetroItemType;
        }

        public busFedStateTaxRateLookup LoadFedStateTaxRates(DataTable adtbSearchResult)
        {
            busFedStateTaxRateLookup lobjFedStateTaxRateLookup = new busFedStateTaxRateLookup();
            lobjFedStateTaxRateLookup.LoadFedStateTaxRates(adtbSearchResult);
            return lobjFedStateTaxRateLookup;
        }

        public busFedStateFlatTaxRateLookup LoadFedStateFlatTaxRates(DataTable adtbSearchResult)
        {
            busFedStateFlatTaxRateLookup lobjFedStateFlatTaxRateLookup = new busFedStateFlatTaxRateLookup();
            lobjFedStateFlatTaxRateLookup.LoadFedStateFlatTaxRates(adtbSearchResult);
            return lobjFedStateFlatTaxRateLookup;
        }

        public busPaymentHistoryHeader FindPaymentHistoryHeader(int Aintpaymenthistoryid)
        {
            busPaymentHistoryHeader lobjPaymentHistoryHeader = new busPaymentHistoryHeader();
            if (lobjPaymentHistoryHeader.FindPaymentHistoryHeader(Aintpaymenthistoryid))
            {
                lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.istrStateTaxTaxExemption =
                    lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.state_tax_allowance > 0 ? lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.state_tax_allowance.ToString() : string.Empty;
                lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.istrFedTaxExemtion =
                    lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.fed_tax_allowance > 0 ? lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.fed_tax_allowance.ToString() : string.Empty;
                if (lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id > 0)
                {
                    lobjPaymentHistoryHeader.LoadOrganization();
                }
                lobjPaymentHistoryHeader.CalculateAmounts();
                lobjPaymentHistoryHeader.LoadPlan();
                lobjPaymentHistoryHeader.LoadPaymentHistoryDetails();
                lobjPaymentHistoryHeader.LoadPaymentHistoryDistribution();
                lobjPaymentHistoryHeader.LoadTaxAmount();
                lobjPaymentHistoryHeader.LoadPaymentSchedule();
            }
            return lobjPaymentHistoryHeader;
        }

        public busPaymentHistoryDetail FindPaymentHistoryDetail(int Aintpaymenthistoryitemdetailid)
        {
            busPaymentHistoryDetail lobjPaymentHistoryDetail = new busPaymentHistoryDetail();
            if (lobjPaymentHistoryDetail.FindPaymentHistoryDetail(Aintpaymenthistoryitemdetailid))
            {
            }
            return lobjPaymentHistoryDetail;
        }

        public busPaymentSchedule NewPaymentSchedule()
        {
            busPaymentSchedule lobjPaymentSchedule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
            lobjPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleStatusValid;
            lobjPaymentSchedule.icdoPaymentSchedule.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2502, lobjPaymentSchedule.icdoPaymentSchedule.status_value);
            lobjPaymentSchedule.icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusPending;
            lobjPaymentSchedule.icdoPaymentSchedule.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2503, lobjPaymentSchedule.icdoPaymentSchedule.action_status_value);
            lobjPaymentSchedule.LoadNexBenefitPaymentDate();
            lobjPaymentSchedule.icdoPaymentSchedule.payment_date = lobjPaymentSchedule.idtNextBenefitPaymentDate;
            lobjPaymentSchedule.EvaluateInitialLoadRules();
            return lobjPaymentSchedule;
        }
        public busPaymentSchedule FindPaymentSchedule(int Aintpaymentscheduleid)
        {
            busPaymentSchedule lobjPaymentSchedule = new busPaymentSchedule();
            if (lobjPaymentSchedule.FindPaymentSchedule(Aintpaymentscheduleid))
            {
                lobjPaymentSchedule.LoadPaymentSteps();
                lobjPaymentSchedule.LoadPaymentScheduleSteps();
                lobjPaymentSchedule.LoadNexBenefitPaymentDate();
            }
            return lobjPaymentSchedule;
        }

        public busPaymentScheduleLookup LoadPaymentSchedules(DataTable adtbSearchResult)
        {
            busPaymentScheduleLookup lobjPaymentScheduleLookup = new busPaymentScheduleLookup();
            lobjPaymentScheduleLookup.LoadPaymentSchedules(adtbSearchResult);
            return lobjPaymentScheduleLookup;
        }

        public busPaymentHistoryHeaderLookup LoadPaymentHistoryHeaders(DataTable adtbSearchResult)
        {
            busPaymentHistoryHeaderLookup lobjPaymentHistoryHeaderLookup = new busPaymentHistoryHeaderLookup();
            lobjPaymentHistoryHeaderLookup.LoadPaymentHistoryHeaders(adtbSearchResult);
            return lobjPaymentHistoryHeaderLookup;
        }

        public busPaymentHistoryDistribution FindPaymentHistoryDistribution(int Aintpaymenthistorydistributionid)
        {
            busPaymentHistoryDistribution lobjPaymentHistoryDistribution = new busPaymentHistoryDistribution();
            if (lobjPaymentHistoryDistribution.FindPaymentHistoryDistribution(Aintpaymenthistorydistributionid))
            {
                lobjPaymentHistoryDistribution.LoadPerson();
                lobjPaymentHistoryDistribution.LoadOrganization();
                lobjPaymentHistoryDistribution.LoadPaymentHistoryHeader();
                lobjPaymentHistoryDistribution.LoadPaymentHistoryDetails();
                lobjPaymentHistoryDistribution.LoadStatus();
            }
            lobjPaymentHistoryDistribution.EvaluateInitialLoadRules();
            return lobjPaymentHistoryDistribution;
        }
        public busPayeeAccountDeductionRefundItem FindPayeeAccountDeductionRefundItem(int Aintpayeeaccountdeductionrefunditemid)
        {
            busPayeeAccountDeductionRefundItem lobjPayeeAccountDeductionRefundItem = new busPayeeAccountDeductionRefundItem();
            if (lobjPayeeAccountDeductionRefundItem.FindPayeeAccountDeductionRefundItem(Aintpayeeaccountdeductionrefunditemid))
            {
                lobjPayeeAccountDeductionRefundItem.LoadibusPayeeAccountDeductionRefund();
                lobjPayeeAccountDeductionRefundItem.LoadibusPaymentItemType();
            }

            return lobjPayeeAccountDeductionRefundItem;
        }
        public busPayeeAccountDeductionRefund NewPayeeAccountDeductionRefund(int Aintpayeeaccountid)
        {
            busPayeeAccountDeductionRefund lobjPayeeAccountDeductionRefund = new busPayeeAccountDeductionRefund();
            lobjPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund = new cdoPayeeAccountDeductionRefund();
            lobjPayeeAccountDeductionRefund.icdoPayeeAccountDeductionRefund.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountDeductionRefund.LoadibusPayeeAccount();
            lobjPayeeAccountDeductionRefund.ibusPayeeAccount.LoadBenefitAmount();
            lobjPayeeAccountDeductionRefund.LoadPayeeAccountDeductionRefundItems();
            lobjPayeeAccountDeductionRefund.EvaluateInitialLoadRules();
            return lobjPayeeAccountDeductionRefund;
        }
        public busPayeeAccountDeductionRefund FindPayeeAccountDeductionRefund(int Aintpayeeaccountdeductionrefundid)
        {
            busPayeeAccountDeductionRefund lobjPayeeAccountDeductionRefund = new busPayeeAccountDeductionRefund();
            if (lobjPayeeAccountDeductionRefund.FindPayeeAccountDeductionRefund(Aintpayeeaccountdeductionrefundid))
            {
                lobjPayeeAccountDeductionRefund.LoadibusPayeeAccount();
                lobjPayeeAccountDeductionRefund.ibusPayeeAccount.LoadBenefitAmount();
                lobjPayeeAccountDeductionRefund.LoadPayeeAccountDeductionRefundItems();
            }
            return lobjPayeeAccountDeductionRefund;
        }

        public busPaymentStepRef FindPaymentStepRef(int aintpaymentstepid)
        {
            busPaymentStepRef lobjPaymentStepRef = new busPaymentStepRef();
            if (lobjPaymentStepRef.FindPaymentStepRef(aintpaymentstepid))
            {
            }
            return lobjPaymentStepRef;
        }

        public busPaymentMonthlyBenefitSummary FindPaymentMonthlyBenefitSummary(int aintpaymentmonthlybenefitsummaryid)
        {
            busPaymentMonthlyBenefitSummary lobjPaymentMonthlyBenefitSummary = new busPaymentMonthlyBenefitSummary();
            if (lobjPaymentMonthlyBenefitSummary.FindPaymentMonthlyBenefitSummary(aintpaymentmonthlybenefitsummaryid))
            {
            }
            return lobjPaymentMonthlyBenefitSummary;
        }

        # region UCS-084

        public busPostRetirementIncreaseBatchRequest FindPostRetirementIncreaseBatchRequest(int aintpostretirementincreasebatchrequestid)
        {
            busPostRetirementIncreaseBatchRequest lobjPostRetirementIncreaseBatchRequest = new busPostRetirementIncreaseBatchRequest();
            if (lobjPostRetirementIncreaseBatchRequest.FindPostRetirementIncreaseBatchRequest(aintpostretirementincreasebatchrequestid))
            {
                lobjPostRetirementIncreaseBatchRequest.LoadPlanList();
                lobjPostRetirementIncreaseBatchRequest.LoadAccountRelationshipList();
                lobjPostRetirementIncreaseBatchRequest.LoadBenefitAccoutTypeList();
            }
            return lobjPostRetirementIncreaseBatchRequest;
        }

        public busPostRetirementIncreaseBatchRequestLookup LoadPostRetirementIncreaseBatchRequests(DataTable adtbSearchResult)
        {
            busPostRetirementIncreaseBatchRequestLookup lobjPostRetirementIncreaseBatchRequestLookup = new busPostRetirementIncreaseBatchRequestLookup();
            lobjPostRetirementIncreaseBatchRequestLookup.LoadPostRetirementIncreaseBatchRequests(adtbSearchResult);
            return lobjPostRetirementIncreaseBatchRequestLookup;
        }
        public busPostRetirementIncreaseBatchRequest NewPostRetirementIncreaseBatchRequest(int aintPostretirmentIncreaseBatchRequestId, string astrPostRetirmentIncreaseType)
        {
            busPostRetirementIncreaseBatchRequest lobjPostRetirementIncreaseBatchRequest = new busPostRetirementIncreaseBatchRequest
            {
                icdoPostRetirementIncreaseBatchRequest = new cdoPostRetirementIncreaseBatchRequest()
            };
            lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value = astrPostRetirmentIncreaseType;

            //initialize the utlCollections
            lobjPostRetirementIncreaseBatchRequest.iclcPlan = new utlCollection<cdoPostRetirementIncreaseBatchPlan>();

            //New Mode, Checking all the List Items By Default
            DataTable ldtbPlanList = Sagitec.DBUtility.DBFunction.DBSelect("cdoPostRetirementIncreaseBatchRequest.LoadRetirementPlansExceptOasisDC",
                new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            foreach (DataRow dr in ldtbPlanList.Rows)
            {
                cdoPostRetirementIncreaseBatchPlan lobjPRIBPlan = new cdoPostRetirementIncreaseBatchPlan();
                lobjPRIBPlan.plan_id = Convert.ToInt32(dr["plan_id"]);
                lobjPRIBPlan.ienuObjectState = ObjectState.None;
                //check for increase type value is not COLA 
                //for increase typw is not COLA then do not allow plan JOb Service and Job service 3rd Party Payor
                if (astrPostRetirmentIncreaseType != busConstant.PostRetirementIncreaseTypeValueCOLA)
                {
                    if ((lobjPRIBPlan.plan_id != busConstant.PlanIdJobService)
                        && (lobjPRIBPlan.plan_id != busConstant.PlanIdJobService3rdPartyPayor))
                    {
                        lobjPostRetirementIncreaseBatchRequest.iclcPlan.Add(lobjPRIBPlan);
                    }
                }
                else
                {
                    if ((lobjPRIBPlan.plan_id == busConstant.PlanIdJobService)
                       || (lobjPRIBPlan.plan_id == busConstant.PlanIdJobService3rdPartyPayor))
                    {
                        lobjPostRetirementIncreaseBatchRequest.iclcPlan.Add(lobjPRIBPlan);
                    }
                }
            }

            lobjPostRetirementIncreaseBatchRequest.iclcBenefitAccount = new utlCollection<cdoPostRetirementIncreaseBatchBenefitAccountType>();
            //New Mode, Checking all the List Items By Default
            DataTable ldtbBenefitAccountList = Sagitec.DBUtility.DBFunction.DBSelect("cdoPostRetirementIncreaseBatchRequest.LoadBenefitAccountType",
                                                new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            foreach (DataRow dr in ldtbBenefitAccountList.Rows)
            {
                cdoPostRetirementIncreaseBatchBenefitAccountType lobjPRIBRelationship = new cdoPostRetirementIncreaseBatchBenefitAccountType();
                lobjPRIBRelationship.benefit_account_type_value = dr["code_value"].ToString();
                lobjPRIBRelationship.ienuObjectState = ObjectState.None;
                lobjPostRetirementIncreaseBatchRequest.iclcBenefitAccount.Add(lobjPRIBRelationship);
            }

            lobjPostRetirementIncreaseBatchRequest.iclcAccountRelationship = new utlCollection<cdoPostRetirementIncreaseBatchAccountRelationship>();
            //New Mode, Checking all the List Items By Default
            DataTable ldtbAccountRelationshipList = iobjPassInfo.isrvDBCache.GetCodeValues(2225);
            foreach (DataRow dr in ldtbAccountRelationshipList.Rows)
            {
                cdoPostRetirementIncreaseBatchAccountRelationship lobjPRIBBenefitAccount = new cdoPostRetirementIncreaseBatchAccountRelationship();
                lobjPRIBBenefitAccount.account_relationship_value = dr["code_value"].ToString();
                lobjPRIBBenefitAccount.ienuObjectState = ObjectState.None;
                lobjPostRetirementIncreaseBatchRequest.iclcAccountRelationship.Add(lobjPRIBBenefitAccount);
            }
            if (aintPostretirmentIncreaseBatchRequestId == 0)
            {
                //BR-084-04
                DateTime ldtSystemDate = busGlobalFunctions.GetSysManagementBatchDate();
                //Set base date based on batch run date
                if (lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value == busConstant.PostRetirementIncreaseTypeValueCOLA)
                {
                    lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.base_date = new DateTime(ldtSystemDate.Year, 11, 01);
                    lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.effective_date = new DateTime(ldtSystemDate.Year, 12, 01);
                }
            }
            else
            {
                lobjPostRetirementIncreaseBatchRequest.FindPostRetirementIncreaseBatchRequest(aintPostretirmentIncreaseBatchRequestId);
                lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id = 0;
                lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.ienuObjectState = ObjectState.Insert;
                lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.created_date = DateTime.Now;
            }

            lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.batch_request_status_value = busConstant.PostRetirementIncreaseBatchTypeValueUnProcessed;
            lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.action_status_value = busConstant.BenefitActionStatusPending;
            lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.status_value = busConstant.ApplicationStatusValid;

            lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(3002, busConstant.ApplicationStatusValid);
            lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.action_status_value);
            lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(3000, astrPostRetirmentIncreaseType);
            lobjPostRetirementIncreaseBatchRequest.EvaluateInitialLoadRules();
            return lobjPostRetirementIncreaseBatchRequest;
        }

        #endregion

        public busPaymentBenefitOverpaymentDetail FindPaymentBenefitOverpaymentDetail(int aintbenefitoverpaymentdetailid)
        {
            busPaymentBenefitOverpaymentDetail lobjPaymentBenefitOverpaymentDetail = new busPaymentBenefitOverpaymentDetail();
            if (lobjPaymentBenefitOverpaymentDetail.FindPaymentBenefitOverpaymentDetail(aintbenefitoverpaymentdetailid))
            {
            }

            return lobjPaymentBenefitOverpaymentDetail;
        }

        public busPaymentBenefitOverpaymentHeader FindPaymentBenefitOverpaymentHeader(int aintbenefitoverpaymentid)
        {
            busPaymentBenefitOverpaymentHeader lobjPaymentBenefitOverpaymentHeader = new busPaymentBenefitOverpaymentHeader();
            if (lobjPaymentBenefitOverpaymentHeader.FindPaymentBenefitOverpaymentHeader(aintbenefitoverpaymentid))
            {
                lobjPaymentBenefitOverpaymentHeader.LoadPaymentBenefitOverpaymentDetails();
                lobjPaymentBenefitOverpaymentHeader.LoadTotalAmountDue();
                lobjPaymentBenefitOverpaymentHeader.LoadPayeeAccount();
                lobjPaymentBenefitOverpaymentHeader.ibusPayeeAccount.LoadApplication();               
                lobjPaymentBenefitOverpaymentHeader.ibusPayeeAccount.LoadMember();
                lobjPaymentBenefitOverpaymentHeader.ibusPayeeAccount.LoadPayee();
                lobjPaymentBenefitOverpaymentHeader.ibusPayeeAccount.LoadBenefitAmount();
                lobjPaymentBenefitOverpaymentHeader.ibusPayeeAccount.LoadTotalTaxableAmount();
            }

            return lobjPaymentBenefitOverpaymentHeader;
        }

        public busPaymentBenefitOverpaymentHeader NewPaymentBenefitOverpaymentHeader(int aintPayeeAccountID)
        {
            busPaymentBenefitOverpaymentHeader lobjPaymentBenefitOverpaymentHeader = new busPaymentBenefitOverpaymentHeader
            { icdoPaymentBenefitOverpaymentHeader = new cdoPaymentBenefitOverpaymentHeader() };
            lobjPaymentBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.payee_account_id = aintPayeeAccountID;
            lobjPaymentBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.is_online_flag = busConstant.Flag_Yes;
            lobjPaymentBenefitOverpaymentHeader.iblnIsNewMode = true;
            lobjPaymentBenefitOverpaymentHeader.LoadPayeeAccount();
            lobjPaymentBenefitOverpaymentHeader.ibusPayeeAccount.LoadApplication();            
            lobjPaymentBenefitOverpaymentHeader.ibusPayeeAccount.LoadMember();
            lobjPaymentBenefitOverpaymentHeader.ibusPayeeAccount.LoadPayee();
            lobjPaymentBenefitOverpaymentHeader.ibusPayeeAccount.LoadBenefitAmount();
            lobjPaymentBenefitOverpaymentHeader.ibusPayeeAccount.LoadTotalTaxableAmount();
            lobjPaymentBenefitOverpaymentHeader.InitializeBenefitOverpaymentDetails();
            lobjPaymentBenefitOverpaymentHeader.EvaluateInitialLoadRules();
            return lobjPaymentBenefitOverpaymentHeader;
        }

        public busPaymentRecovery NewPaymentRecovery(int aintBenfitOverpaymentID, int aintPayeeAccountID)
        {
            busPaymentRecovery lobjPaymentRecovery = new busPaymentRecovery { icdoPaymentRecovery = new cdoPaymentRecovery() };
            lobjPaymentRecovery.icdoPaymentRecovery.benefit_overpayment_id = aintBenfitOverpaymentID;
            lobjPaymentRecovery.icdoPaymentRecovery.payee_account_id = aintPayeeAccountID;
            lobjPaymentRecovery.LoadTotalRecoveryAmount();
            //PIR 11436 Added for loading member and payee information
            lobjPaymentRecovery.LoadPayeeAccount();
            lobjPaymentRecovery.ibusPayeeAccount.LoadApplication();
            lobjPaymentRecovery.ibusPayeeAccount.LoadMember();
            lobjPaymentRecovery.ibusPayeeAccount.LoadPayee();
            lobjPaymentRecovery.ibusPayeeAccount.LoadBenefitAmount();
            lobjPaymentRecovery.ibusPayeeAccount.LoadTotalTaxableAmount();
            lobjPaymentRecovery.iclbPayeeAccountRetroPaymentRecoveryDetail = new Collection<busPayeeAccountRetroPaymentRecoveryDetail>();
            lobjPaymentRecovery.iclbPaymentRecoveryHistory = new Collection<busPaymentRecoveryHistory>();
            lobjPaymentRecovery.EvaluateInitialLoadRules(utlPageMode.All);
            return lobjPaymentRecovery;
        }

        public busPaymentRecovery FindPaymentRecovery(int aintpaymentrecoveryid)
        {
            busPaymentRecovery lobjPaymentRecovery = new busPaymentRecovery();
            if (lobjPaymentRecovery.FindPaymentRecovery(aintpaymentrecoveryid))
            {
                lobjPaymentRecovery.LoadPayeeAccountRetroPaymentRecoveryDetails();
                lobjPaymentRecovery.LoadPaymentRecoveryHistory();
                lobjPaymentRecovery.LoadLTDAmortizationInterestPaid();                
                lobjPaymentRecovery.LoadLTDAmountPaid();
                lobjPaymentRecovery.LoadLTDPrincipleAmountPaid();
                lobjPaymentRecovery.LoadAvailableRemittance();
                //PIR 11436 Added for loading member and payee information
                lobjPaymentRecovery.ibusPayeeAccount.LoadApplication();
                lobjPaymentRecovery.ibusPayeeAccount.LoadMember();
                lobjPaymentRecovery.ibusPayeeAccount.LoadPayee();
                lobjPaymentRecovery.ibusPayeeAccount.LoadBenefitAmount();
                lobjPaymentRecovery.ibusPayeeAccount.LoadTotalTaxableAmount();
                if (lobjPaymentRecovery.icdoPaymentRecovery.status_value != busConstant.RecoveryStatusPendingApproval)
                    lobjPaymentRecovery.CalculateEstimatedEndDate();
            }

            return lobjPaymentRecovery;
        }

        public busPaymentRecoveryHistory FindPaymentRecoveryHistory(int aintrecoveryhistoryid)
        {
            busPaymentRecoveryHistory lobjPaymentRecoveryHistory = new busPaymentRecoveryHistory();
            if (lobjPaymentRecoveryHistory.FindPaymentRecoveryHistory(aintrecoveryhistoryid))
            {
            }

            return lobjPaymentRecoveryHistory;
        }

        public busPayeeAccountRetroPaymentRecoveryDetail FindPayeeAccountRetroPaymentRecoveryDetail(int aintretropaymentrecoverydetailid)
        {
            busPayeeAccountRetroPaymentRecoveryDetail lobjPayeeAccountRetroPaymentRecoveryDetail = new busPayeeAccountRetroPaymentRecoveryDetail();
            if (lobjPayeeAccountRetroPaymentRecoveryDetail.FindPayeeAccountRetroPaymentRecoveryDetail(aintretropaymentrecoverydetailid))
            {
            }

            return lobjPayeeAccountRetroPaymentRecoveryDetail;
        }

        public busPayment1099r FindPayment1099r(int aintpayment1099rid)
        {
            busPayment1099r lobjPayment1099r = new busPayment1099r();
            if (lobjPayment1099r.FindPayment1099r(aintpayment1099rid))
            {
                lobjPayment1099r.istrReportTemplateName = "rptForm1099R_" + lobjPayment1099r.icdoPayment1099r.tax_year;
                lobjPayment1099r.LoadPayeeAccount();
                lobjPayment1099r.ibusPayeeAccount.LoadPayee();
                lobjPayment1099r.ibusPayeeAccount.LoadApplication();
                lobjPayment1099r.ibusPayeeAccount.LoadMember();
                lobjPayment1099r.ibusPayeeAccount.LoadBenefitAmount();
                lobjPayment1099r.ibusPayeeAccount.LoadTotalTaxableAmount();
            }
            return lobjPayment1099r;
        }
        public ArrayList ViewReport_Click(int aint1099rID)
        {
            return new busPayment1099r().ViewReport_Click(aint1099rID);
        }
        public busPaymentLifeTimeReductionRef FindPaymentLifeTimeReductionRef(int aintlifetimereductionrefid)
        {
            busPaymentLifeTimeReductionRef lobjPaymentLifeTimeReductionRef = new busPaymentLifeTimeReductionRef();
            if (lobjPaymentLifeTimeReductionRef.FindPaymentLifeTimeReductionRef(aintlifetimereductionrefid))
            {
            }

            return lobjPaymentLifeTimeReductionRef;
        }

        public busPayeeAccountMonthwiseAdjustmentDetail FindPayeeAccountMonthwiseAdjustmentDetail(int aintretropaymentmonthwisedetailid)
        {
            busPayeeAccountMonthwiseAdjustmentDetail lobjPayeeAccountMonthwiseAdjustmentDetail = new busPayeeAccountMonthwiseAdjustmentDetail();
            if (lobjPayeeAccountMonthwiseAdjustmentDetail.FindPayeeAccountMonthwiseAdjustmentDetail(aintretropaymentmonthwisedetailid))
            {
            }
            return lobjPayeeAccountMonthwiseAdjustmentDetail;
        }

        public busPayeeAccountMinimumGuaranteeHistory FindPayeeAccountMinimumGuaranteeHistory(int aintmininumguaranteehistoryid)
        {
            busPayeeAccountMinimumGuaranteeHistory lobjPayeeAccountMinimumGuaranteeHistory = new busPayeeAccountMinimumGuaranteeHistory();
            if (lobjPayeeAccountMinimumGuaranteeHistory.FindPayeeAccountMinimumGuaranteeHistory(aintmininumguaranteehistoryid))
            {
            }

            return lobjPayeeAccountMinimumGuaranteeHistory;
        }

        public busPayment1099rRequest NewPayment1099rRequest()
        {
            busPayment1099rRequest lobjPayment1099rRequest = new busPayment1099rRequest { icdoPayment1099rRequest = new cdoPayment1099rRequest() };
            return lobjPayment1099rRequest;
        }

        public busPayment1099rRequest FindPayment1099rRequest(int aintrequestid)
        {
            busPayment1099rRequest lobjPayment1099rRequest = new busPayment1099rRequest();
            if (lobjPayment1099rRequest.FindPayment1099rRequest(aintrequestid))
            {
            }

            return lobjPayment1099rRequest;
        }

        public busPayment1099rRequest NewMonthly1099rRequest()
        {
            busPayment1099rRequest lobjPayment1099rRequest = new busPayment1099rRequest { icdoPayment1099rRequest = new cdoPayment1099rRequest() };
            lobjPayment1099rRequest.icdoPayment1099rRequest.request_type_value = busConstant.Monthly1099RReportsBatch;

            lobjPayment1099rRequest.icdoPayment1099rRequest.request_type_description = busConstant.Monthly1009rRequestBatchDescription;
            return lobjPayment1099rRequest;
        }


        public busPayment1099rRequest FindMonthly1099rRequest(int aintrequestid)
        {
            busPayment1099rRequest lobjPayment1099rRequest = new busPayment1099rRequest();
            if (lobjPayment1099rRequest.FindPayment1099rRequest(aintrequestid))
            {
            }

            return lobjPayment1099rRequest;
        }

        public bus1099rPaymentHistoryLink Find1099rPaymentHistoryLink(int aintpayment1099rhistorylinkid)
        {
            bus1099rPaymentHistoryLink lobj1099rPaymentHistoryLink = new bus1099rPaymentHistoryLink();
            if (lobj1099rPaymentHistoryLink.Find1099rPaymentHistoryLink(aintpayment1099rhistorylinkid))
            {
            }

            return lobj1099rPaymentHistoryLink;
        }
        //Lookup screen is required for Actuarial file batch as per pir 1807
        public busActuaryFileHeader NewActuaryFileHeader()
        {
            busActuaryFileHeader lbusActuaryFileHeader = new busActuaryFileHeader { icdoActuaryFileHeader = new cdoActuaryFileHeader() };
            return lbusActuaryFileHeader;
        }
        public busActuaryFileHeader FindActuaryFileHeader(int aintActuaryFileHeaderid)
        {
            busActuaryFileHeader lbusActuaryFileHeader = new busActuaryFileHeader();
            if (lbusActuaryFileHeader.FindActuaryFileHeader(aintActuaryFileHeaderid))
            {

            }
            return lbusActuaryFileHeader;
        }
        public busActuaryFileHeaderLookup LoadActuaryFileHeaders(DataTable adtbSearchResult)
        {
            busActuaryFileHeaderLookup lobjActuaryFileHeaderLookup = new busActuaryFileHeaderLookup();
            lobjActuaryFileHeaderLookup.LoadActuaryFileHeaders(adtbSearchResult);
            return lobjActuaryFileHeaderLookup;
        }

        public busPayment1099rRequestLookup LoadPayment1099rRequests(DataTable adtbSearchResult)
        {
            busPayment1099rRequestLookup lobjPayment1099rRequestLookup = new busPayment1099rRequestLookup();
            lobjPayment1099rRequestLookup.LoadPayment1099rRequests(adtbSearchResult);
            return lobjPayment1099rRequestLookup;
        }

        public busPaymentElectionAdjustment FindPaymentElectionAdjustment(int aintpaymentelectionadjustmentid)
        {
            busPaymentElectionAdjustment lobjPaymentElectionAdjustment = new busPaymentElectionAdjustment();
            if (lobjPaymentElectionAdjustment.FindPaymentElectionAdjustment(aintpaymentelectionadjustmentid))
            {
                lobjPaymentElectionAdjustment.LoadPersonAccount();
                lobjPaymentElectionAdjustment.ibusPersonAccount.LoadPerson();
                lobjPaymentElectionAdjustment.ibusPersonAccount.LoadPlan();
                lobjPaymentElectionAdjustment.LoadTotalPaidAmount();
            }

            return lobjPaymentElectionAdjustment;
        }

        public busPaymentElectionAdjustmentLookup LoadPaymentElectionAdjustments(DataTable adtbSearchResult)
        {
            busPaymentElectionAdjustmentLookup lobjPaymentElectionAdjustmentLookup = new busPaymentElectionAdjustmentLookup();
            lobjPaymentElectionAdjustmentLookup.LoadPaymentElectionAdjustments(adtbSearchResult);
            return lobjPaymentElectionAdjustmentLookup;
        }
        public busPayeeAccountTaxWithholding NewWFourRTaxWithholding(int Aintpayeeaccountid, string astrTaxIdentidifier)
        {
            busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountTaxWithholding.LoadPayeeAccount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadMember();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTotalTaxableAmount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadBenefitAmount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayee.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = astrTaxIdentidifier;


            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxWithHoldingHistory();
            lobjPayeeAccountTaxWithholding.EvaluateInitialLoadRules();
            ////F/W Upgrade 15738 - Since no changes were made on the screen, the system is not adding to the changelog nor changing the object state 
            ////due to which record not getting saved, child records are throwing foreign key error since parent key not generated
            ////so adding them here.
            //lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.ienuObjectState = ObjectState.Insert;
            //lobjPayeeAccountTaxWithholding.iarrChangeLog.Add(lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding);
            return lobjPayeeAccountTaxWithholding;
        }
        public busPayeeAccountTaxWithholding NewW4PTaxWithholding(int Aintpayeeaccountid, string astrTaxIdentidifier)
        {
            busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountTaxWithholding.LoadPayeeAccount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadMember();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTotalTaxableAmount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadBenefitAmount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayee.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = astrTaxIdentidifier;


            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxWithHoldingHistory();
            lobjPayeeAccountTaxWithholding.EvaluateInitialLoadRules();
            return lobjPayeeAccountTaxWithholding;
        }
        public busPayeeAccountTaxWithholding FindW4PTaxWithholding(int Aintpayeeaccounttaxwithholdingid)
        {
            busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
            if (lobjPayeeAccountTaxWithholding.FindPayeeAccountTaxWithholding(Aintpayeeaccounttaxwithholdingid))
            {
                lobjPayeeAccountTaxWithholding.LoadPayeeAccount();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadPayee();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadApplication();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxWithHoldingHistory();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadMember();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTotalTaxableAmount();
                lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadBenefitAmount();
            }
            return lobjPayeeAccountTaxWithholding;
        }
        public busPayeeAccountTaxWithholding NewMwpPaymentTaxWithholding(int Aintpayeeaccountid, string astrTaxIdentidifier)
        {
            busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = Aintpayeeaccountid;
            lobjPayeeAccountTaxWithholding.LoadPayeeAccount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadMember();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTotalTaxableAmount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadBenefitAmount();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadPayee();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.ibusPayee.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = astrTaxIdentidifier;


            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
            lobjPayeeAccountTaxWithholding.ibusPayeeAccount.LoadTaxWithHoldingHistory();
            lobjPayeeAccountTaxWithholding.EvaluateInitialLoadRules();
            return lobjPayeeAccountTaxWithholding;
        }
        public ArrayList View1099RReport_Click(int aint1099rID)
        {
            busMSSHome lbusMSSHome = new busMSSHome();
            ArrayList larlstResult = lbusMSSHome.View1099RReport(aint1099rID);
            return larlstResult;
        }
    }
}
