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
#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busPayeeAccountRolloverDetail : busPayeeAccountRolloverDetailGen
	{
        bool iblnIsNewMode = false;
        //Load Active Rollover Detail based on next benefit payment date and Get The Total rollover Amount and Percentage
        //If the persentage is greater than 100 or the amount is greater than benefit amount,Then throw an error while creating the next one. BR - 34,35
        public bool iblnIsReissueApproveClick { get; set; }
        public DateTime idtReissuedHistoryPaymentDate { get; set; }

        public cdoWssBenAppRolloverDetail icdoWssBenAppRolloverDetailRequested { get; set; }
        public bool IsTotalRolloverPercentageOrAmountInvalid()
        {
            decimal ldecTotalRolloverPercentage = 0.00M;
            decimal ldecTotalRolloverAmount = 0.00M;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            if (idecRolledOverAmount == 0.00m)
                LoadTotalRolloverAmount();
            if (ibusPayeeAccount.iclbActiveRolloverDetails == null)
            {
                ibusPayeeAccount.LoadActiveRolloverDetail();
            }
            if (ibusPayeeAccount.idecTotalTaxableAmountForFlatRates == 0.00M)
            {
                ibusPayeeAccount.LoadTaxableAmountForFlatRates(ibusPayeeAccount.idtNextBenefitPaymentDate,
                    (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plso_requested_flag == busConstant.Flag_Yes) ? true : false);
            }
            if (icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusActive)
            {
                foreach (busPayeeAccountRolloverDetail lobjPayeeAccountRolloverDetail in ibusPayeeAccount.iclbActiveRolloverDetails)
                {
                    if (lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.percent_of_taxable > 0.00m && icdoPayeeAccountRolloverDetail.percent_of_taxable > 0.00m)
                    {
                        ldecTotalRolloverPercentage = lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.percent_of_taxable + icdoPayeeAccountRolloverDetail.percent_of_taxable;
                        if (ldecTotalRolloverPercentage > 100)
                        {
                            return true;
                        }
                    }
                    else if (lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.amount_of_taxable > 0.00m && icdoPayeeAccountRolloverDetail.amount_of_taxable > 0.00m)
                    {
                        if (lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_flag == busConstant.Flag_Yes
                            && icdoPayeeAccountRolloverDetail.amount_of_taxable > ibusPayeeAccount.idecTotalTaxableAmountForFlatRates)
                        {
                            return true;
                        }
                        else if (lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_flag == busConstant.Flag_No
                            && lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.amount_of_taxable + icdoPayeeAccountRolloverDetail.amount_of_taxable > ibusPayeeAccount.idecTotalTaxableAmountForFlatRates)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
      
        //Payroll issue fix-if rollover record already exists for the same org and Payee Account ,throw an error
        public bool IsRolloverExistForSameOrg()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbActiveRolloverDetails == null)
                ibusPayeeAccount.LoadActiveRolloverDetail();
            if (ibusPayeeAccount.iclbActiveRolloverDetails.Where(o =>
                (o.icdoPayeeAccountRolloverDetail.rollover_org_id == icdoPayeeAccountRolloverDetail.rollover_org_id)
                &&(!string.IsNullOrEmpty(o.icdoPayeeAccountRolloverDetail.account_number)&& !string.IsNullOrEmpty(icdoPayeeAccountRolloverDetail.account_number) &&
                    o.icdoPayeeAccountRolloverDetail.account_number==icdoPayeeAccountRolloverDetail.account_number)
                && (o.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id != icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id)
                && (icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusActive)).Count() > 0)
            {
                return true;
            }
            return false;
        }
        public bool IsRolloverAllowed()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbActiveRolloverDetails == null)
                ibusPayeeAccount.LoadActiveRolloverDetail();
            foreach (busPayeeAccountRolloverDetail lobjDetail in ibusPayeeAccount.iclbActiveRolloverDetails)
            {
                if (lobjDetail.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAmountOfTaxable ||
                    lobjDetail.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
                {
                    if (icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfGross ||
                        icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfTaxable)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public decimal idecRolledOverAmount { get; set; }
        public void LoadTotalRolloverAmount()
        {   
            idecRolledOverAmount=0.0m;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if(ibusPayeeAccount.idtNextBenefitPaymentDate==DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
           //Always load ,no null check because it has to be reloaded for recalculating rollover amount
            ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            foreach (busPayeeAccountPaymentItemType lobjPaymentItemType in ibusPayeeAccount.iclbPayeeAccountPaymentItemType)
            {
                if (busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate, lobjPaymentItemType.icdoPayeeAccountPaymentItemType.start_date,
                    lobjPaymentItemType.icdoPayeeAccountPaymentItemType.end_date))
                {
                    if (lobjPaymentItemType.ibusPaymentItemType == null)
                        lobjPaymentItemType.LoadPaymentItemType();
                    if (lobjPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolloverItemReductionCheck)
                    {
                        idecRolledOverAmount += lobjPaymentItemType.icdoPayeeAccountPaymentItemType.amount;
                    }
                }
            }
        }
        public int iintRolloverBatch { get; set; }
        public void CreatRolloverAdjustment()
        {
            if (iblnIsRolloverToBeAdjusted)
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                    ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                iintRolloverBatch = 1;
                btnRollover_Click();
            }
        }
        //while creating new rollover and all item are rolled over,throw an error
        public bool IsThereNoItemToRollover()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (!IsReissueRolloverValid())
            {
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                //Load All the Payment item types whhich can be rolled over for the payee account
                if (ibusPayeeAccount.iclbPaymentItemTypesToRollover == null)
                    ibusPayeeAccount.LoadPaymentItemTypesToRollover();
                if (ibusPayeeAccount.iclbPaymentItemTypesToRollover.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }       
        //if Member check reissued to rollover org ,the option should all of gross
        public bool IsReissueRolloverAllowed()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.IsReIssueToRolloverAllowedForMember() && 
                icdoPayeeAccountRolloverDetail.rollover_option_value!=busConstant.PayeeAccountRolloverOptionAllOfGross)            
            {
                return true;
            }
            return false;
        }
        /*   This method will rollover the amount specified in the screen for the payee account 
         * 1.Load All the Payment item types whhich can be rolled over for the payee account
         * 2.If the rollover option is Amount of Taxable ,add an item with item code ITEM14 into a new collection -lclbRolloverItems
         * 3.For All other rollover options ,Find the Correspondin Rollover items, accumulate it in a new collection -lclbRolloverItems
         * 4.If All of Gross or All of Taxable option is selected ,  accumulate it and and multiply by item_direction 
         * 5.If % of taxable is selected then multiply the amount by Percent of taxable column and divide it by 100 
         * 6.Post All the records into Payee account payment item type table    */
        public void btnRollover_Click()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
          
            bool lbnlAllow = false;          
            if (iblnIsReissueApproveClick)
            {
                if (ibusPayeeAccount.iclbPaymentItemTypesToRollover == null)
                    ibusPayeeAccount.LoadPaymentItemTypesToRolloverForReissue(idtReissuedHistoryPaymentDate);
                lbnlAllow = true;
            }            
            else if (!IsReissueRolloverValid() && (!iblnIsReissueApproveClick))            
            {
                if (ibusPayeeAccount.iclbPaymentItemTypesToRollover == null)
                    ibusPayeeAccount.LoadPaymentItemTypesToRollover();
                lbnlAllow = true;
            }
            if (lbnlAllow)
            {
                if (idecRolledOverAmount == 0.00m)
                    LoadTotalRolloverAmount();                
                DateTime ldtNextBenfitPaymentDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
                if (iblnIsReissueApproveClick)
                    ldtNextBenfitPaymentDate = DateTime.Today;
                Collection<busPayeeAccountPaymentItemType> lclbRolloverItems = new Collection<busPayeeAccountPaymentItemType>();
                //If the rollover option is not  Amount of Taxable ,group the items which can be rolled over 
                foreach (busPayeeAccountPaymentItemType lobjPaymentItemType in ibusPayeeAccount.iclbPaymentItemTypesToRollover)
                {
                    //If the rollover option is All of Taxable ,group the items which can be rolled over anr having taxable indicator as 'Y'
                    if (icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfTaxable)
                    {
                        if (lobjPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                        {
                            LoadRolloverItems(lclbRolloverItems, lobjPaymentItemType);
                        }
                    }
                    else if (icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                    {
                        if (lobjPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                        {
                            LoadRolloverItems(lclbRolloverItems, lobjPaymentItemType);
                            if (lclbRolloverItems.Count > 0)
                                break;
                        }
                    }
                    else if (icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
                    {
                        if (lobjPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                        {
                            LoadRolloverItems(lclbRolloverItems, lobjPaymentItemType);
                        }
                    }
                    //If the rollover option is not All of Taxable ,group the items which can be rolled over and irrespective of taxable indicator
                    else
                    {
                        LoadRolloverItems(lclbRolloverItems, lobjPaymentItemType);
                    }
                }
                if (iintRolloverBatch == 0)
                {
                    //Post into payee account payment item type table
                    foreach (busPayeeAccountPaymentItemType lobjRolloverItems in lclbRolloverItems)
                    {
                        decimal ldecRolloveramount = 0.0m;
                        
                        if (icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
                        {
                            ldecRolloveramount = GetRolloverAmount(lobjRolloverItems);
                            lobjRolloverItems.icdoPayeeAccountPaymentItemType.amount = ldecRolloveramount;
                        }
                        lobjRolloverItems.icdoPayeeAccountPaymentItemType.start_date = ldtNextBenfitPaymentDate;
                        if (iblnIsReissueApproveClick)
                        {
                            lobjRolloverItems.icdoPayeeAccountPaymentItemType.reissue_item_flag = busConstant.Flag_Yes;
                        }
						//PIR 17060 - Roth Rollover taxes Implementation
                        if(icdoPayeeAccountRolloverDetail.rollover_type_value == busConstant.RolloverTypeValueForRothIRA)
                        {
                            busPaymentItemType lobjPaymentAccountType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                            if (ibusPayeeAccount.ibusApplication.IsNull())
                                ibusPayeeAccount.LoadApplication();
                            if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plso_requested_flag == busConstant.Flag_Yes)
                            {
                                if (lobjRolloverItems.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                                {
                                    lobjPaymentAccountType.FindPaymentItemTypeByItemCode(busConstant.PAPITPLSORothRolloverTaxableAmount);
                                    lobjRolloverItems.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPaymentAccountType.icdoPaymentItemType.payment_item_type_id;
                                }
                                else
                                {
                                    lobjPaymentAccountType.FindPaymentItemTypeByItemCode(busConstant.PAPITPLSORothRolloverNonTaxableAmount);
                                    lobjRolloverItems.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPaymentAccountType.icdoPaymentItemType.payment_item_type_id;
                                }
                            
                            }
                            else 
                            {
                                if (lobjRolloverItems.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                                {
                                    lobjPaymentAccountType.FindPaymentItemTypeByItemCode(busConstant.PAPITRolloverTaxableAmountForRothIRA);
                                    lobjRolloverItems.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPaymentAccountType.icdoPaymentItemType.payment_item_type_id;
                                }
                                else
                                {
                                    lobjPaymentAccountType.FindPaymentItemTypeByItemCode(busConstant.PAPITRolloverNonTaxableAmountForRothIRA);
                                    lobjRolloverItems.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPaymentAccountType.icdoPaymentItemType.payment_item_type_id;
                                }
                            }
                        }                       
                        lobjRolloverItems.icdoPayeeAccountPaymentItemType.Insert();
                        InsertIntoRolloverItemDetail(lobjRolloverItems);
                    }
                }
                else
                {
                    foreach (busPayeeAccountPaymentItemType lobjRolloverItems in lclbRolloverItems)
                    {
                        decimal ldecRolloveramount = 0.00m;
                        if (icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
                        {
                            ldecRolloveramount = GetRolloverAmount(lobjRolloverItems);
                        }
                        else
                        {
                            ldecRolloveramount = lobjRolloverItems.icdoPayeeAccountPaymentItemType.amount;
                        }
                        if (iblnIsReissueApproveClick)
                        {
                            lobjRolloverItems.icdoPayeeAccountPaymentItemType.reissue_item_flag=busConstant.Flag_Yes;
                        }
                        if (icdoPayeeAccountRolloverDetail.rollover_type_value == busConstant.RolloverTypeValueForRothIRA)
                        {
                            busPaymentItemType lobjPaymentAccountType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                            if (ibusPayeeAccount.ibusApplication.IsNull())
                                ibusPayeeAccount.LoadApplication();
                            if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plso_requested_flag == busConstant.Flag_Yes)
                            {
                                if (lobjRolloverItems.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                                {
                                    lobjPaymentAccountType.FindPaymentItemTypeByItemCode(busConstant.PAPITPLSORothRolloverTaxableAmount);
                                    lobjRolloverItems.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPaymentAccountType.icdoPaymentItemType.payment_item_type_id;
                                }
                                else
                                {
                                    lobjPaymentAccountType.FindPaymentItemTypeByItemCode(busConstant.PAPITPLSORothRolloverNonTaxableAmount);
                                    lobjRolloverItems.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPaymentAccountType.icdoPaymentItemType.payment_item_type_id;
                                }
                            }
                            else
                            {
                                if (lobjRolloverItems.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                                {
                                    lobjPaymentAccountType.FindPaymentItemTypeByItemCode(busConstant.PAPITRolloverTaxableAmountForRothIRA);
                                    lobjRolloverItems.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPaymentAccountType.icdoPaymentItemType.payment_item_type_id;
                                }
                                else
                                {
                                    lobjPaymentAccountType.FindPaymentItemTypeByItemCode(busConstant.PAPITRolloverNonTaxableAmountForRothIRA);
                                    lobjRolloverItems.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPaymentAccountType.icdoPaymentItemType.payment_item_type_id;
                                }
                            }
                        }        
                        if (ldecRolloveramount > 0.0m)
                        {
                            int lintPaymetnItemId = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lobjRolloverItems.icdoPayeeAccountPaymentItemType.payment_item_type_id,
                                ldecRolloveramount, string.Empty, 0, ldtNextBenfitPaymentDate, DateTime.MinValue, false);

                            if (lintPaymetnItemId > 0 && !IsRolloverItemDetailExists(lintPaymetnItemId))
                            {                                
                                InsertIntoRolloverItemDetailforBatch(lobjRolloverItems, lintPaymetnItemId);
                            }
                        }
                    }
                }
                //update the record that the amount or persentage is rolled over
                icdoPayeeAccountRolloverDetail.rollover_flag = busConstant.Flag_Yes;
                icdoPayeeAccountRolloverDetail.Update();
                //CalculateTax after rolover
                CalculateAdjustmentTax();
            }
        }
        //
        private decimal GetRolloverAmount(busPayeeAccountPaymentItemType lobjRolloverItems)
        {
            decimal ldecRolloveramount = 0.00m;
            if (ibusPayeeAccount.iclbActiveRolloverDetails == null)
                ibusPayeeAccount.LoadActiveRolloverDetail();
           
            int lintActivePercentageRolloverDetail = ibusPayeeAccount.iclbActiveRolloverDetails.Where(o => o.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id
                != icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id && o.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable).Count();
            int lintActiveAmountRolloverDetail = ibusPayeeAccount.iclbActiveRolloverDetails.Where(o => o.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id
               != icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id && o.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAmountOfTaxable).Count();
            decimal ldecActivePercentageRolloverDetail = ibusPayeeAccount.iclbActiveRolloverDetails.Where(o => o.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id
                != icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id && o.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
                .Select(o => o.icdoPayeeAccountRolloverDetail.percent_of_taxable).FirstOrDefault();
            if (icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
            {
                if (lintActivePercentageRolloverDetail > 0)
                {
                    LoadTotalRolloverAmount();

                    //uat pir 1242
                    if (idecRolledOverAmount > 0.0m && (ldecActivePercentageRolloverDetail + icdoPayeeAccountRolloverDetail.percent_of_taxable) == 100)
                        ldecRolloveramount = lobjRolloverItems.icdoPayeeAccountPaymentItemType.amount - idecRolledOverAmount;
                    else
                        ldecRolloveramount = ((lobjRolloverItems.icdoPayeeAccountPaymentItemType.amount) * icdoPayeeAccountRolloverDetail.percent_of_taxable) / 100;
                }
                else
                {
                    if (lintActiveAmountRolloverDetail > 0)
                    {
                        LoadTotalRolloverAmount();
                        ldecRolloveramount = ((lobjRolloverItems.icdoPayeeAccountPaymentItemType.amount - idecRolledOverAmount) * icdoPayeeAccountRolloverDetail.percent_of_taxable) / 100;
                    }
                    else
                    {
                        ldecRolloveramount = ((lobjRolloverItems.icdoPayeeAccountPaymentItemType.amount) * icdoPayeeAccountRolloverDetail.percent_of_taxable) / 100;
                    }
                }
            }
            return ldecRolloveramount;
        }

        public void LoadMemberRequestedRolloverDetail()
        {
            if (icdoWssBenAppRolloverDetailRequested.IsNull())
                icdoWssBenAppRolloverDetailRequested = new cdoWssBenAppRolloverDetail();
            if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_application_id > 0)
            {
                busWssBenApp lbusWssBenApp = new busWssBenApp();
                if (lbusWssBenApp.FindWssBenAppByBenAppId(ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_application_id))
                {
                    busWssBenAppRolloverDetail lbusWssBenAppRolloverDetail = new busWssBenAppRolloverDetail();
                    if (lbusWssBenAppRolloverDetail.FindWssBenAppRolloverDetailByWssBenAppId(lbusWssBenApp.icdoWssBenApp.wss_ben_app_id))
                    {
                        icdoWssBenAppRolloverDetailRequested = lbusWssBenAppRolloverDetail.icdoWssBenAppRolloverDetail;
                        if (string.IsNullOrEmpty(icdoWssBenAppRolloverDetailRequested.rollover_account_number))
                        {
                            icdoWssBenAppRolloverDetailRequested.rollover_account_number = "xxx-xx-" + ibusPayeeAccount.ibusMember.icdoPerson.LastFourDigitsOfSSN;
                        }
                    }
                }
            }
        }
        public void CalculateAdjustmentTax()
        {
		    // PIR 17060 - Roth Rollover Taxes Implementation
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            if (ibusPayeeAccount.iclbTaxWithholingHistory == null)
            {
                ibusPayeeAccount.LoadTaxWithHoldingHistory();
            }
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();   
            if(icdoPayeeAccountRolloverDetail.rollover_type_value == busConstant.RolloverTypeValueForRothIRA && ibusPayeeAccount.iblnRolloverInfoDeleteIndicator == false)
            {
                busPayeeAccountTaxWithholding lobjbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding() { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                lobjbusPayeeAccountTaxWithholding.ibusPayeeAccount = ibusPayeeAccount;
                lobjbusPayeeAccountTaxWithholding.ibusPayeeAccount.icdoPayeeAccount = ibusPayeeAccount.icdoPayeeAccount;
                lobjbusPayeeAccountTaxWithholding.CreatePaymentItemForFlatTaxForRothRollover
                    (icdoPayeeAccountRolloverDetail.rollover_org_id, icdoPayeeAccountRolloverDetail.payee_account_id, icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id,icdoPayeeAccountRolloverDetail.state_tax_option_value,(ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plso_requested_flag == busConstant.Flag_Yes) ? true : false,ibusPayeeAccount.iblnRolloverInfoDeleteIndicator);
            }                
            foreach (busPayeeAccountTaxWithholding lobjTaxWithHolding in ibusPayeeAccount.iclbTaxWithholingHistory)
            {
                if (busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate,
                    lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.start_date, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.end_date))
                {
                    lobjTaxWithHolding.CalculateTax(false);
                    lobjTaxWithHolding.PostTaxAmount(true);
                }
            }
        }
        //Insert entries into Payee account rollover item detail, so that we will have reference for rollover items
        private void InsertIntoRolloverItemDetail(busPayeeAccountPaymentItemType aobjRolloverItems)
        {
            busPayeeAccountRolloverItemDetail lobjRolloverItemDetail = new busPayeeAccountRolloverItemDetail();
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail = new cdoPayeeAccountRolloverItemDetail();
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_rollover_detail_id
                                                = icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id;
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_payment_item_type_id
                                                = aobjRolloverItems.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.Insert();
        }
        //Insert entries into Payee account rollover item detail ,so that we will have reference for rollover items
        private void InsertIntoRolloverItemDetailforBatch(busPayeeAccountPaymentItemType aobjRolloverItems,int aintPayeeAccountPaymentItemTypeid)
        {
            busPayeeAccountRolloverItemDetail lobjRolloverItemDetail = new busPayeeAccountRolloverItemDetail();
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail = new cdoPayeeAccountRolloverItemDetail();
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_rollover_detail_id
                                                = icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id;
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_payment_item_type_id
                                                = aintPayeeAccountPaymentItemTypeid;
            lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.Insert();
        }
        private void LoadRolloverItems(Collection<busPayeeAccountPaymentItemType> aclbRolloverItems, busPayeeAccountPaymentItemType aobjPaymentItemType)
        {
            bool lblnItemExists = false;
            if (aobjPaymentItemType.ibusPaymentItemType == null)
                aobjPaymentItemType.LoadPaymentItemType();
            busPayeeAccountPaymentItemType lobjRolloverItem = new busPayeeAccountPaymentItemType();
            //if the collection is empty ,add the roll over item into collection
            if (aclbRolloverItems.Count == 0)
            {
                GroupRolloverItems(aobjPaymentItemType, lobjRolloverItem);
                aclbRolloverItems.Add(lobjRolloverItem);
            }
            else
            {
                //if the rollover item is already exist ,accumulate it
                foreach (busPayeeAccountPaymentItemType lobjRolloverItems in aclbRolloverItems)
                {
                    if (aobjPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.rollover_item_code
                            == lobjRolloverItems.ibusPaymentItemType.icdoPaymentItemType.item_type_code)
                    {
                        GroupRolloverItems(aobjPaymentItemType, lobjRolloverItems);
                        lblnItemExists = true;
                        break;
                    }
                }
                //if the rollover item is not exist ,add it to the collection
                if (!lblnItemExists)
                {
                    GroupRolloverItems(aobjPaymentItemType, lobjRolloverItem);
                    aclbRolloverItems.Add(lobjRolloverItem);
                }
            }
        }
        
        private void GroupRolloverItems(busPayeeAccountPaymentItemType aobjRolloverFromItem, busPayeeAccountPaymentItemType aobjRolloverToItem)
        {
            if (aobjRolloverToItem.icdoPayeeAccountPaymentItemType == null)
                aobjRolloverToItem.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
            //Find the Rollover items from the existing payment item types
            if (aobjRolloverToItem.ibusPaymentItemType == null)
            { 
                aobjRolloverToItem.ibusPaymentItemType = new busPaymentItemType();
            }
            aobjRolloverToItem.ibusPaymentItemType.FindPaymentItemTypeByItemCode(aobjRolloverFromItem.ibusPaymentItemType.icdoPaymentItemType.rollover_item_code);
            aobjRolloverToItem.icdoPayeeAccountPaymentItemType.payment_item_type_id = aobjRolloverToItem.ibusPaymentItemType.icdoPaymentItemType.payment_item_type_id;               
            //If All of Gross or All of Taxable option is selected , for each item type and accumulate it and multiply by item_direction 
            aobjRolloverToItem.icdoPayeeAccountPaymentItemType.payee_account_id = aobjRolloverFromItem.icdoPayeeAccountPaymentItemType.payee_account_id;            
            if ((icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfGross) ||
                (icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfTaxable))
            {
                  aobjRolloverToItem.icdoPayeeAccountPaymentItemType.amount += aobjRolloverFromItem.icdoPayeeAccountPaymentItemType.amount *
                    aobjRolloverFromItem.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
            }
                 //If % of taxable is selected then multiply the amount by Percent of taxable column and divide it by 100 
            else if (icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
            {
                  aobjRolloverToItem.icdoPayeeAccountPaymentItemType.amount = icdoPayeeAccountRolloverDetail.amount_of_taxable;
            }
            //If $ of taxable is selected then multiply the amount by Percent of taxable column and divide it by 100 
            else if (icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable)
            {
                   if ((aobjRolloverFromItem.icdoPayeeAccountPaymentItemType.amount > 0.00M) && (icdoPayeeAccountRolloverDetail.percent_of_taxable > 0.00M))
                {
                    aobjRolloverToItem.icdoPayeeAccountPaymentItemType.amount +=
                        aobjRolloverFromItem.icdoPayeeAccountPaymentItemType.amount * aobjRolloverFromItem.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                }
            }           
        }       

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (icdoPayeeAccountRolloverDetail.org_code != null)
            {
                LoadRolloverOrgByOrgcode();
                icdoPayeeAccountRolloverDetail.rollover_org_id = ibusRolloverOrg.icdoOrganization.org_id;
            }
            if (aenmPageMode == utlPageMode.New)
            {
                iblnIsNewMode = true;
            }
            base.BeforeValidate(aenmPageMode);
        }
      
        public override void BeforePersistChanges()
        {
            //PIR 981 -The must update sgt_payee_Account_payment with end date when status changed to processed.
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusProcessed)
            {
                if (iclbPayeeAcountRolloverItems == null)
                    LoadPayeeAccountRolloverItems();
                foreach (busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItemType in iclbPayeeAcountRolloverItems)
                {
                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = ibusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1);
                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
                }
            }
            //If there is a record in cancelled status with same data ,there update the old rollover detail id
            busPayeeAccountRolloverDetail lobjOldRolloverDetail = GetSameCancelledRolloverRecord();
            if (lobjOldRolloverDetail != null)
            {
                if (ibusPayeeAccount.iclbRolloverDetail == null)
                    ibusPayeeAccount.LoadRolloverDetail();
                if (ibusPayeeAccount.iclbRolloverDetail.Where(o => o.icdoPayeeAccountRolloverDetail.old_rollover_dtl_id > 0 &&
                     o.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id != icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id &&
                    o.icdoPayeeAccountRolloverDetail.old_rollover_dtl_id == icdoPayeeAccountRolloverDetail.old_rollover_dtl_id).Count() == 0)
                {
                    icdoPayeeAccountRolloverDetail.old_rollover_dtl_id = lobjOldRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id;
                }
            }
          
            base.BeforePersistChanges();
        }
        //Status cannot be changed to Active once it is processed and cancelled
        public bool IsStatusActive()
        {
            if (icdoPayeeAccountRolloverDetail.ihstOldValues.Count > 0 &&
                icdoPayeeAccountRolloverDetail.ihstOldValues["status_value"].ToString() != busConstant.PayeeAccountRolloverDetailStatusActive &&
                icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusActive)
            {
                return true;
            }
            return false;
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            // Update Payee Account Status to review if there is any change Rollover information 
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            //Rollover On Save
            if (iblnIsNewMode && (icdoPayeeAccountRolloverDetail.rollover_flag == null || icdoPayeeAccountRolloverDetail.rollover_flag == busConstant.Flag_No))
            {
                btnRollover_Click();
            }
            if (icdoPayeeAccountRolloverDetail.status_value == busConstant.CodeValueForRolloverCancelled)
            {
                DeleteRolledverItems();
            }
            if (!IsReissueRolloverValid())
            {
                if (ibusPayeeAccount.ibusSoftErrors == null)
                    ibusPayeeAccount.LoadErrors();
                ibusPayeeAccount.iblnClearSoftErrors = false;
                ibusPayeeAccount.ibusSoftErrors.iblnClearError = false;
                ibusPayeeAccount.CreateReviewPayeeAccountStatus();
                ibusPayeeAccount.iblnRolloverInfoChangeIndicator = true;
                ibusPayeeAccount.ValidateSoftErrors();
                ibusPayeeAccount.UpdateValidateStatus();
            }
            ibusPayeeAccount.LoadRolloverDetail();
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
        
        //Delete RolledOverItem beforeRecalculate
        public void DeleteRolledverItems()
        {
            //Reload Before delete
            LoadRolloverItemDetail();
            foreach (busPayeeAccountRolloverItemDetail lobjRolloverItemDetail in iclbRolloverItemDetail)
            {
                if (lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_rollover_item_detail_id > 0)
                    lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.Delete();
            }
            //Reload Before delete
            LoadPayeeAccountRolloverItems();
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItem in iclbPayeeAcountRolloverItems)
            {
                if (lobjPayeeAccountPaymentItem.FindPayeeAccountPaymentItemType(
                    lobjPayeeAccountPaymentItem.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id))
                    lobjPayeeAccountPaymentItem.icdoPayeeAccountPaymentItemType.Delete();
            }
        }
        public override int Delete()
        {
            if(icdoPayeeAccountRolloverDetail.rollover_type_value == busConstant.RolloverTypeValueForRothIRA)
            {
                DeleteRolledverItems();              
            }
            int lintrtn= base.Delete();
            //On deletion of rollover Details change the payee account status to review and recalculate tax
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusSoftErrors == null)
                ibusPayeeAccount.LoadErrors();
            ibusPayeeAccount.iblnClearSoftErrors = false;
            ibusPayeeAccount.ibusSoftErrors.iblnClearError = false;
            ibusPayeeAccount.CreateReviewPayeeAccountStatus();
            ibusPayeeAccount.iblnRolloverInfoDeleteIndicator = true;
            ibusPayeeAccount.icdoPayeeAccount.Select();
            ibusPayeeAccount.ValidateSoftErrors();
            ibusPayeeAccount.UpdateValidateStatus();
            CalculateAdjustmentTax();
            return lintrtn;
        }
        //Rollover Reissue cancel,new are allowed only when refund payee account processed 
        //or retirement plso is in receiving or retirement 5 year term certain is in receiving
        public bool IsReissueRolloverValid()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            string lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption5YearTermLife &&
                lstrStatus == busConstant.PayeeAccountStatusReceiving)
            {
                return false;
            }
            if (lstrStatus == busConstant.PayeeAccountStatusReceiving ||
                   lstrStatus == busConstant.PayeeAccountStatusPaymentComplete ||
                   lstrStatus == busConstant.PayeeAccountStatusRefundProcessed)
            {
                return true;
            }
            return false;
        }
        public bool IsCancelRolloverValid()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            string lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption5YearTermLife &&
                lstrStatus == busConstant.PayeeAccountStatusReceiving)
            {
                return false;
            }
            if (lstrStatus == busConstant.PayeeAccountStatusReceiving ||
                   lstrStatus == busConstant.PayeeAccountStatusPaymentComplete ||
                   lstrStatus == busConstant.PayeeAccountStatusRefundProcessed ||
                   lstrStatus == busConstant.PayeeAccountStatusApproved ||
                   lstrStatus == busConstant.PayeeAccountStatusReview)
            {
                return true;
            }
            return false;
        }
        //When cancel selected ,it should be done only after rollover processed
        //Cancel cannot be selected in new mode
        //Cancel can be selected only where rollover check or ach is is in void pending or stop pay pending
        public bool IsCancelStatusSelect()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();            
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatus();
            if (icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusCancelled)
            {
                string lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                if (!IsCancelRolloverValid())
                {
                    return true;
                }               
                else
                {
                    if (IsCancelRolloverValid())
                    {
                        if (ibusPayeeAccount.iclbPaymentHistoryHeader == null)
                            ibusPayeeAccount.LoadPaymentHistoryHeader();
                        if (ibusPayeeAccount.iclbPaymentHistoryHeader.Where(o =>
                            o.icdoPaymentHistoryHeader.org_id == icdoPayeeAccountRolloverDetail.rollover_org_id).FirstOrDefault() != null)
                        {
                            busPaymentHistoryHeader lobjPaymentHistoryHeader = ibusPayeeAccount.iclbPaymentHistoryHeader.Where(o =>
                            o.icdoPaymentHistoryHeader.org_id == icdoPayeeAccountRolloverDetail.rollover_org_id).FirstOrDefault();
                            if (lobjPaymentHistoryHeader.iclbPaymentHistoryDistribution == null)
                                lobjPaymentHistoryHeader.LoadPaymentHistoryDistribution();
                            if (lobjPaymentHistoryHeader.iclbPaymentHistoryDistribution.Where(o =>
                                !(o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.RACHVoidPending ||
                                  o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.RCHKVoidPending ||
                                  o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.RCHKStopPayPending)).Count() > 0)
                            {
                                return true;
                            }
                        }

                    }
                    if (icdoPayeeAccountRolloverDetail.ienuObjectState == ObjectState.Insert)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //if payee account processed and New rollover allowed then there should be rollover record in cancelled status=new mode
        public bool IsActiveRolloverExistFortheData()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbRolloverDetail == null)
                ibusPayeeAccount.LoadRolloverDetail();
            if (IsReissueRolloverValid())
            {
                if (ibusPayeeAccount.iclbRolloverDetail.Where(o =>
                    o.icdoPayeeAccountRolloverDetail.rollover_option_value == icdoPayeeAccountRolloverDetail.rollover_option_value &&
                    o.icdoPayeeAccountRolloverDetail.amount_of_taxable == icdoPayeeAccountRolloverDetail.amount_of_taxable &&
                    o.icdoPayeeAccountRolloverDetail.percent_of_taxable == icdoPayeeAccountRolloverDetail.percent_of_taxable &&
                     o.icdoPayeeAccountRolloverDetail.status_value == icdoPayeeAccountRolloverDetail.status_value &&
                    o.icdoPayeeAccountRolloverDetail.old_rollover_dtl_id > 0).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        //if payee account processed and New rollover allowed then there should be rollover record in cancelled status=new mode
        public bool IsSameRolloverRecordNotExistinCancelledStatus()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbCancelledRolloverDetail == null)
                ibusPayeeAccount.LoadCancelledRolloverDetails();
            if (ibusPayeeAccount.iclbCancelledRolloverDetail.Count > 0 && IsReissueRolloverValid())
            {
                busPayeeAccountRolloverDetail lobjRolloverDetail = GetSameCancelledRolloverRecord();
                if (lobjRolloverDetail == null)
                    return true;
            }
            return false;
        }
        //Get the same cancelled rollover record if exists
        private busPayeeAccountRolloverDetail GetSameCancelledRolloverRecord()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbCancelledRolloverDetail == null)
                ibusPayeeAccount.LoadCancelledRolloverDetails();

            foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in ibusPayeeAccount.iclbCancelledRolloverDetail)
            {
                if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.amount_of_taxable == icdoPayeeAccountRolloverDetail.amount_of_taxable
                    && lobjRolloverDetail.icdoPayeeAccountRolloverDetail.percent_of_taxable == icdoPayeeAccountRolloverDetail.percent_of_taxable
                    && lobjRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_option_value==icdoPayeeAccountRolloverDetail.rollover_option_value)
                {
                    return lobjRolloverDetail;
                }
            }
            return null;
        }

        //uat pir 1420
        public bool IsPayeeAccountStatusCancelPendingOrCancelled()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();

            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelPending() || ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                lblnResult = true;

            return lblnResult;
        }

        public bool IsRolloverItemDetailExists(int aintPAPITId)
        {
            bool lblnResult = false;

            DataTable ldtRolloverItemDtl = Select<cdoPayeeAccountRolloverItemDetail>(new string[1] { "payee_account_payment_item_type_id" },
                                                new object[1] { aintPAPITId }, null, null);
            if (ldtRolloverItemDtl.Rows.Count > 0)
                lblnResult = true;

            return lblnResult;
        }

        public void SetPayeeAccountRolloverDetail()
        {
            if (icdoWssBenAppRolloverDetailRequested.rollover_option_value.IsNotNullOrEmpty() && icdoWssBenAppRolloverDetailRequested.rollover_type_value.IsNotNullOrEmpty() &&
                icdoWssBenAppRolloverDetailRequested.state_tax_option_value.IsNotNullOrEmpty() && icdoWssBenAppRolloverDetailRequested.amount_of_taxable.IsNotNull() &&
                icdoWssBenAppRolloverDetailRequested.rollover_account_number.IsNotNullOrEmpty() && icdoWssBenAppRolloverDetailRequested.percent_of_taxable.IsNotNull() &&
                icdoWssBenAppRolloverDetailRequested.rollover_institution_name.IsNotNullOrEmpty())
            {
                icdoPayeeAccountRolloverDetail.rollover_option_value = icdoWssBenAppRolloverDetailRequested.rollover_option_value;
                icdoPayeeAccountRolloverDetail.rollover_type_value = icdoWssBenAppRolloverDetailRequested.rollover_type_value;
                icdoPayeeAccountRolloverDetail.state_tax_option_value = icdoWssBenAppRolloverDetailRequested.state_tax_option_value;
                icdoPayeeAccountRolloverDetail.amount_of_taxable = icdoWssBenAppRolloverDetailRequested.amount_of_taxable;
                icdoPayeeAccountRolloverDetail.account_number = icdoWssBenAppRolloverDetailRequested.rollover_account_number;
                icdoPayeeAccountRolloverDetail.percent_of_taxable = icdoWssBenAppRolloverDetailRequested.percent_of_taxable;
                icdoPayeeAccountRolloverDetail.status_value = busConstant.StatusActive;
            }
        }
        //PIR 26488 Assigning Member Full Name to fbo
        public void RolloverFBOName()
        {
            if (ibusPayeeAccount.IsNull())
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayee.IsNull())
                ibusPayeeAccount.LoadPayee();
            if (icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id == 0)
                icdoPayeeAccountRolloverDetail.fbo = ibusPayeeAccount.ibusPayee.icdoPerson.PayeeFullName;
        }
    }
}
