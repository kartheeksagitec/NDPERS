using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Collections;

namespace NeoSpinBatch
{
    class busProcessSupplementalCheckBatch : busNeoSpinBatch
    {
        public busProcessSupplementalCheckBatch()
        { }
        //Process each 200 records at one time
        public void ProcessSupplementalCheck()
        {
            istrProcessName = "Process Supplemental Batch";
            idlgUpdateProcessLog("Step : 1 Get all batch request For Supplemental batch processing", "INFO", istrProcessName);


            int lintProcessedCount = 0;
            bool lblnSuccess = false;
            int lintRequestId = 0;
            DataTable ldtbList = busBase.Select<cdoPostRetirementIncreaseBatchRequest>(new string[4] { "POST_RETIREMENT_INCREASE_TYPE_VALUE", "BATCH_REQUEST_STATUS_VALUE", 
                                                                                                         "ACTION_STATUS_VALUE", "STATUS_VALUE" }, new object[4] 
                                                                                                     { busConstant.PostRetirementIncreaseTypeValueSupplemental,
                                                                                                        busConstant.PostRetirementIncreaseBatchTypeValueUnProcessed,
                                                                                                        busConstant.CalculationStatusApproval,
                                                                                                        busConstant.ApplicationStatusValid}, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbList.Rows)
                {
                    int lintDetailCount = 0;
                    int lintTotalProcessDetailCount = 0;
                    int lintPayeePerslinkId = 0;
                    int lintPayeeOrgId = 0;
                    int lintTotalCount = 0;
                    lblnSuccess = false;
                    busPostRetirementIncreaseBatchRequest lobjPostRetirementIncrease = new busPostRetirementIncreaseBatchRequest
                    {
                        icdoPostRetirementIncreaseBatchRequest = new cdoPostRetirementIncreaseBatchRequest()
                    };
                    lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.LoadData(dr);
                    lintRequestId = lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id;
                    //load details records if exist
                    if (lobjPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.IsNull())
                        lobjPostRetirementIncrease.LoadPostRetirementIncreaseBatchRequestDetail();

                    idlgUpdateProcessLog("Step : 2 Get all Payee records For Supplemental batch processing", "INFO", istrProcessName);

                    DataTable ldtbPayeeListTobeProcessed = busBase.Select("cdoPostRetirementIncreaseBatchRequest.ProcessCOLAAdhocSupplementBatch",
                                                new object[2] {lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                        lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id});
                    if (ldtbPayeeListTobeProcessed.Rows.Count > 0)
                    {

                        while (true)
                        {
                            if (lintTotalCount == ldtbPayeeListTobeProcessed.Rows.Count)
                                break;
                            iobjPassInfo.BeginTransaction();
                            try
                            {
                                for (int i = 0; i < 100; i++)
                                {
                                    if (lintTotalCount == ldtbPayeeListTobeProcessed.Rows.Count)
                                        break;
                                    if (!Convert.IsDBNull(ldtbPayeeListTobeProcessed.Rows[lintTotalCount]["payee_perslink_id"]))
                                    {
                                        lintPayeePerslinkId = (int)ldtbPayeeListTobeProcessed.Rows[lintTotalCount]["payee_perslink_id"];
                                    }
                                    else
                                    {
                                        lintPayeeOrgId = (int)ldtbPayeeListTobeProcessed.Rows[lintTotalCount]["PAYEE_ORG_ID"];
                                    }
                                    ProcessSupplementalCheckPerPlan(lobjPostRetirementIncrease, ldtbPayeeListTobeProcessed.Rows[lintTotalCount]);

                                    lintTotalCount++;
                                    lintProcessedCount++;
                                }
                                idlgUpdateProcessLog(lintProcessedCount.ToString() + " Records Processed of " + ldtbPayeeListTobeProcessed.Rows.Count.ToString(), "INFO", istrProcessName);
                                iobjPassInfo.Commit();
                            }
                            catch (Exception e)
                            {
                                if (lintPayeePerslinkId != 0)
                                {
                                    idlgUpdateProcessLog("Process Supplemental Batch Failed at Processing request ID  " + lintRequestId.ToString() + "and Person id - "
                                                                + lintPayeePerslinkId + " due to following exception - " + e.ToString(), "INFO", istrProcessName);
                                }
                                else
                                {
                                    idlgUpdateProcessLog("Process Supplemental Batch Failed at Processing request ID  " + lintRequestId.ToString() + "and Org id - "
                                                               + lintPayeeOrgId + " due to following exception - " + e.ToString(), "INFO", istrProcessName);
                                }
                                iobjPassInfo.Rollback();
                                break;
                            }
                            lblnSuccess = true;
                        }
						//UAT PIR 1490
                        if (lblnSuccess)
                        {
                            //if the number of detail records are equal to number of total records for the batch request
                            //update the status of batch request as Processed
                            lobjPostRetirementIncrease.LoadPostRetirementIncreaseBatchRequestDetail();

                            lintDetailCount = lobjPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.Count();

                            lintTotalProcessDetailCount = lobjPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.Where(lobjDtl => lobjDtl.is_processed_flag == busConstant.Flag_Yes).Count();
                            if (lintDetailCount.Equals(lintTotalProcessDetailCount))
                            {
                                iobjPassInfo.BeginTransaction();
                                try
                                {
                                    idlgUpdateProcessLog("Step : 10 Insert Or Update Post Retirement Increase Batch Request Detail for Post retirement Increase Batch Request ID = "
                                                       + lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id
                                                        , "INFO", istrProcessName);

                                    lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.batch_request_status_value = busConstant.PostRetirementIncreaseBatchTypeValueProcessed;
                                    lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.status_value = busConstant.StatusProcessed; // UAT PIR ID 1392
                                    lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.Update();
                                    iobjPassInfo.Commit();
                                }
                                catch (Exception e)
                                {
                                    idlgUpdateProcessLog("Process Supplemental Batch Failed at Processing request ID  " + lintRequestId.ToString() + "and Person id - "
                                                                + lintPayeePerslinkId + " due to following exception - " + e.ToString(), "INFO", istrProcessName);
                                    iobjPassInfo.Rollback();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        idlgUpdateProcessLog("No Payee Account found for the for Post retirement Increase Batch Request ID = "
                                                                  + lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id
                                                                   , "INFO", istrProcessName);
                    }
                }
            }
            if (lblnSuccess)
                idlgUpdateProcessLog("Process Supplemental Batch ended", "INFO", istrProcessName);
        }
        public void ProcessSupplementalCheckPerPlan(busPostRetirementIncreaseBatchRequest abusPostRetirementIncrease, DataRow adrPayee)
        {
            int lintStateTaxPayeeAccountTypeId = 0;
            int lintFedTaxPayeeAccountTypeId = 0;
            int lintSupplementalTaxPayeeAccountTypeId = 0;
            decimal ldecbaseAmount = 0.00M;
            decimal ldecMonthlySupplementalTaxableAmount = 0.00M;
            string lstrPaymentItemtype = string.Empty;

            abusPostRetirementIncrease.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.LoadData(adrPayee);

            if (abusPostRetirementIncrease.ibusPayeeAccount.ibusPlan.IsNull())
            {
                abusPostRetirementIncrease.ibusPayeeAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                abusPostRetirementIncrease.ibusPayeeAccount.ibusPlan.icdoPlan.LoadData(adrPayee);
            }
            //idlgUpdateProcessLog("Step : 3 Process Supplemental for Payee Perslink ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id
            //                                                                                                    , "INFO", istrProcessName);

            //load all Payment item types
            abusPostRetirementIncrease.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

            //load Base amount
            abusPostRetirementIncrease.ibusPayeeAccount.LoadGrossAmount(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.base_date);
            ldecbaseAmount = abusPostRetirementIncrease.ibusPayeeAccount.idecGrossAmount;

            //load COLA increase
            if (abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount > 0)
            {
                ldecMonthlySupplementalTaxableAmount = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount;
            }
            else
            {
                ldecMonthlySupplementalTaxableAmount = (ldecbaseAmount * abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_percentage) / 100;
                ldecMonthlySupplementalTaxableAmount = busGlobalFunctions.RoundToPenny(ldecMonthlySupplementalTaxableAmount);
            }

            //**************************************************************************//
            //**************************TO CALCULATE STATE TAX *******************************//
            //**************************************************************************//
            //idlgUpdateProcessLog("Step : 5 Calculate State Tax for payment Payee Account Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
            //                                                                                                , "INFO", istrProcessName);

            decimal ldecCurrentStateTax = 0.00M;
            decimal ldecCurrentStateTaxAfterIncrease = 0.00M;

            int lintPaymentItemTypeId = abusPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDStateTaxAmount);
            //get the current state tax
            var lenumCurrentStateTax = abusPostRetirementIncrease.ibusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                .Where(lobjPA => lobjPA.icdoPayeeAccountPaymentItemType.payment_item_type_id == lintPaymentItemTypeId
                                                                && busGlobalFunctions.CheckDateOverlapping(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                                lobjPA.icdoPayeeAccountPaymentItemType.start_date, lobjPA.icdoPayeeAccountPaymentItemType.end_date));
            if (lenumCurrentStateTax.Count() > 0)
                ldecCurrentStateTax = lenumCurrentStateTax.FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;


            if (abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount > 0)
            {
                //ldecCurrentStateTaxAfterIncrease = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount;
                //if (abusPostRetirementIncrease.ibusPayeeAccount.iclbTaxWithholingHistory == null)
                //{
                //    abusPostRetirementIncrease.ibusPayeeAccount.LoadTaxWithHoldingHistory();   
                //}
                //if ((abusPostRetirementIncrease.ibusPayeeAccount.iclbTaxWithholingHistory.Count > 0)
                //    && (abusPostRetirementIncrease.ibusPayeeAccount.iclbTaxWithholingHistory.Where(lobjHis =>
                //        lobjHis.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax
                //        && busGlobalFunctions.CheckDateOverlapping(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                //        lobjHis.icdoPayeeAccountTaxWithholding.start_date,lobjHis.icdoPayeeAccountTaxWithholding.end_date)    
                //        ).Count() > 0))
                //{
                //PIR: 1858 As per Satya, the Flat Tax has to be calculated whenever flat increase is provided.
                //ldecCurrentStateTaxAfterIncrease = busPayeeAccountHelper.CalculateFlatTax(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount,
                //abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
                //abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value,
                //abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.account_relation_value,
                //abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                //busConstant.PayeeAccountTaxIdentifierStateTax, busConstant.Flag_No,
                //abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.family_relation_value, busConstant.Flag_Yes);
                //}

            }
            else
            {
                if (ldecCurrentStateTax > 0.00M)
                {
                    ldecCurrentStateTaxAfterIncrease = ((Convert.ToDecimal(ldecCurrentStateTax) * abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_percentage) / 100);
                    ldecCurrentStateTaxAfterIncrease = busGlobalFunctions.RoundToPenny(ldecCurrentStateTaxAfterIncrease);
                }
            }

            lstrPaymentItemtype = busConstant.PAPITNDStateTaxableAmountOneTimePayment;
            lintPaymentItemTypeId = abusPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(lstrPaymentItemtype);

            if (ldecCurrentStateTaxAfterIncrease > 0.00M)
            {
                //get Vendor Org id for the state tax
                //int lintOrgCodeId = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, "STCM", iobjPassInfo));
                int lintOrgId = busPayeeAccountHelper.GetStateTaxVendorID();

                //idlgUpdateProcessLog("Step : 7 Insert State Tax amount for payment Payee Account Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
                //                                                                                            , "INFO", istrProcessName);
                InsertPayeeAccountPaymentType(abusPostRetirementIncrease, ldecCurrentStateTaxAfterIncrease, lintPaymentItemTypeId, ref lintStateTaxPayeeAccountTypeId, lintOrgId);
            }

            //**************************UPDATE/INSERT ND STATE AMOUNT*******************************//
            //**************************************************************************//
            //CHECK IF THE DETAIL ALREADY EXIST THEN UPDATE ELSE INSERT

            cdoPostRetirementIncreaseBatchRequestDetail lobjPostRetirementIncreaseDetailForState = new cdoPostRetirementIncreaseBatchRequestDetail();
            lobjPostRetirementIncreaseDetailForState = InsertUpdatePostRetirementIncreaseDetail(abusPostRetirementIncrease, lintStateTaxPayeeAccountTypeId, ldecCurrentStateTaxAfterIncrease, lintPaymentItemTypeId, 0);
            abusPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.Add(lobjPostRetirementIncreaseDetailForState);

            //***************** END *************************************************************************************************

            //**************************TO CALCULATE FED TAX *******************************//
            //**************************************************************************//
            //idlgUpdateProcessLog("Step : 7 Calculate Fed Tax for payment Payee Account Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
            //                                                                                                , "INFO", istrProcessName);

            decimal ldecCurrentFedTax = 0.00M;
            decimal ldecCurrentFedTaxAfterIncrease = 0.00M;

            int lintPaymentItemTypeIdFedTax = abusPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITFederalTaxAmount);
            //get the current state tax
            var lenumCurrentFedTax = abusPostRetirementIncrease.ibusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                .Where(lobjPA => lobjPA.icdoPayeeAccountPaymentItemType.payment_item_type_id == lintPaymentItemTypeIdFedTax
                                                                && busGlobalFunctions.CheckDateOverlapping(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                                lobjPA.icdoPayeeAccountPaymentItemType.start_date, lobjPA.icdoPayeeAccountPaymentItemType.end_date));
            if (lenumCurrentFedTax.Count() > 0)
                ldecCurrentFedTax = lenumCurrentFedTax.FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;

            if (abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount > 0)
            {
                //ldecCurrentFedTaxAfterIncrease = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount;
                //if (abusPostRetirementIncrease.ibusPayeeAccount.iclbTaxWithholingHistory == null)
                //{
                //    abusPostRetirementIncrease.ibusPayeeAccount.LoadTaxWithHoldingHistory();
                //}
                //if ((abusPostRetirementIncrease.ibusPayeeAccount.iclbTaxWithholingHistory.Count > 0)
                //    && (abusPostRetirementIncrease.ibusPayeeAccount.iclbTaxWithholingHistory.Where(lobjHis =>
                //        lobjHis.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax 
                //        && busGlobalFunctions.CheckDateOverlapping(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                //        lobjHis.icdoPayeeAccountTaxWithholding.start_date, lobjHis.icdoPayeeAccountTaxWithholding.end_date)
                //        ).Count() > 0))
                //{
                //PIR: 1858 As per Satya, the Flat Tax has to be calculated whenever flat increase is provided.
               // ldecCurrentFedTaxAfterIncrease = busPayeeAccountHelper.CalculateFlatTax(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount,
               // abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
               // abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value,
               // abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.account_relation_value,
               // abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
               //busConstant.PayeeAccountTaxIdentifierFedTax, busConstant.Flag_No,
               //abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.family_relation_value, busConstant.Flag_Yes);
                //}

            }
            else
            {
                if (ldecCurrentFedTax > 0.00M)
                {
                    ldecCurrentFedTaxAfterIncrease = (Convert.ToDecimal(ldecCurrentFedTax) * abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_percentage) / 100;
                    ldecCurrentFedTaxAfterIncrease = busGlobalFunctions.RoundToPenny(ldecCurrentFedTaxAfterIncrease);
                }
            }

            lstrPaymentItemtype = busConstant.PAPITFedTaxableOneTimePayment;
            lintPaymentItemTypeId = abusPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(lstrPaymentItemtype);
            if (ldecCurrentFedTaxAfterIncrease > 0.00M)
            {
                //get Vendor Org id for the Fed tax
                //int lintOrgCodeId = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, "FTVN", iobjPassInfo));
                int lintOrgId = busPayeeAccountHelper.GetFedTaxVendorID();
                //idlgUpdateProcessLog("Step : 7 Insert State Tax amount for payment Payee Account Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
                //                                                                                            , "INFO", istrProcessName);

                InsertPayeeAccountPaymentType(abusPostRetirementIncrease, ldecCurrentFedTaxAfterIncrease, lintPaymentItemTypeId, ref lintFedTaxPayeeAccountTypeId, lintOrgId);
            }

            //**************************UPDATE/INSERT FED AMOUNT*******************************//
            //**************************************************************************//
            //check if the detail already exist then update else insert

            cdoPostRetirementIncreaseBatchRequestDetail lobjPostretirementincreaseDetailForFed = new cdoPostRetirementIncreaseBatchRequestDetail();
            lobjPostretirementincreaseDetailForFed = InsertUpdatePostRetirementIncreaseDetail(abusPostRetirementIncrease, lintFedTaxPayeeAccountTypeId, ldecCurrentFedTaxAfterIncrease, lintPaymentItemTypeId, 0);
            abusPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.Add(lobjPostretirementincreaseDetailForFed);

            //**************************** END **************************************//                                           
            //----------------------------------------------------------------------------------------------------------------------
            //*********************** INSERT SUPPLEMENTAL TAXABLE AMOUNT TO PAYEE ACCOUNT PAYMENT ITEM TYPE
            //****************************************************************** 
            //idlgUpdateProcessLog("Step : 7 Insert Supplemental amount for payment Payee Account Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
            //                                                                                                , "INFO", istrProcessName);
            lstrPaymentItemtype = busConstant.PAPITTaxableAmountOneTimePayment;
            lintPaymentItemTypeId = abusPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(lstrPaymentItemtype);
            if (ldecMonthlySupplementalTaxableAmount > 0.00M)
            {
                InsertPayeeAccountPaymentType(abusPostRetirementIncrease, ldecMonthlySupplementalTaxableAmount, lintPaymentItemTypeId, ref lintSupplementalTaxPayeeAccountTypeId, 0);
            }

            //****************************************************************** 
            //*********UPDATE/INSERT SUPPLEMENTAL AMOUNT TO POST RETIREMENT INCREASE DETAIL************//
            //**************************************************************************//
            //check if the detail already exist then update else insert

            cdoPostRetirementIncreaseBatchRequestDetail lobjPostretirementincreaseDetailForSupplemental = new cdoPostRetirementIncreaseBatchRequestDetail();
            lobjPostretirementincreaseDetailForSupplemental = InsertUpdatePostRetirementIncreaseDetail(abusPostRetirementIncrease, lintSupplementalTaxPayeeAccountTypeId, ldecMonthlySupplementalTaxableAmount, lintPaymentItemTypeId, ldecbaseAmount);
            abusPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.Add(lobjPostretirementincreaseDetailForSupplemental);
        }
        private cdoPostRetirementIncreaseBatchRequestDetail InsertUpdatePostRetirementIncreaseDetail(busPostRetirementIncreaseBatchRequest
            abusPostRetirementIncrease, int aintPaymentPayeeAccountTypeId, decimal adecCurrentTaxAfterIncrease, int aintPaymentItemTypeId, decimal adecBaseamount)
        {
            cdoPostRetirementIncreaseBatchRequestDetail lobjPostRetirementIncreaseDetail = new cdoPostRetirementIncreaseBatchRequestDetail();
            lobjPostRetirementIncreaseDetail = GetDuplicateRecord(abusPostRetirementIncrease, abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, aintPaymentItemTypeId);
            if (lobjPostRetirementIncreaseDetail.IsNotNull())
            {
                if (lobjPostRetirementIncreaseDetail.post_retirement_increase_batch_request_detail_id > 0)
                {
                    lobjPostRetirementIncreaseDetail.is_processed_flag = busConstant.Flag_Yes;
                    lobjPostRetirementIncreaseDetail.original_amount = adecBaseamount;
                    lobjPostRetirementIncreaseDetail.increase_amount = adecCurrentTaxAfterIncrease;
                    lobjPostRetirementIncreaseDetail.Update();

                    //idlgUpdateProcessLog("Step : 9 Update Post Retirement Increase Death Batch Request Detail for Payee Account ID = "
                    //                              + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, "INFO", istrProcessName);
                }
            }
            else
            {
                //INSERT INTO POST RETIREMENT INCREASE BATCH REQUEST DETAIL TABLE      
                lobjPostRetirementIncreaseDetail = new cdoPostRetirementIncreaseBatchRequestDetail();
                lobjPostRetirementIncreaseDetail.is_processed_flag = busConstant.Flag_Yes;
                lobjPostRetirementIncreaseDetail.original_amount = adecBaseamount;
                lobjPostRetirementIncreaseDetail.increase_amount = adecCurrentTaxAfterIncrease;
                lobjPostRetirementIncreaseDetail.payee_account_id = abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPostRetirementIncreaseDetail.payee_account_payment_item_type_id = aintPaymentPayeeAccountTypeId;
                lobjPostRetirementIncreaseDetail.payment_item_type_id = aintPaymentItemTypeId;
                lobjPostRetirementIncreaseDetail.post_retirement_increase_batch_request_id = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id;
                lobjPostRetirementIncreaseDetail.Insert();

                //idlgUpdateProcessLog("Step : 9 Insert Post Retirement Increase Death Batch Request Detail for Payee Account ID = "
                //                              + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, "INFO", istrProcessName);
            }
            return lobjPostRetirementIncreaseDetail;
        }

        private void InsertPayeeAccountPaymentType(busPostRetirementIncreaseBatchRequest abusPostRetirementIncrease,
            decimal adecCurrentStateTaxAfterIncrease, int aintPaymentItemTypeId, ref int aintPayeeAccountPaymentItemTypeId, int aintOrgId)
        {
            busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_id = abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id = aintPaymentItemTypeId;
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date;
            if (aintOrgId > 0)
            {
                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.vendor_org_id = aintOrgId;
            }

            DateTime ldtTempDate = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date.AddMonths(1);
            ldtTempDate = new DateTime(ldtTempDate.Year, ldtTempDate.Month, 1);
            ldtTempDate = ldtTempDate.AddDays(-1);
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = ldtTempDate;
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount = adecCurrentStateTaxAfterIncrease;
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = busConstant.BatchScheduleIDProcessSupplemental;
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Insert();

            aintPayeeAccountPaymentItemTypeId = lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
        }

        //check if post retirement increase detail record exist with these values
        private cdoPostRetirementIncreaseBatchRequestDetail GetDuplicateRecord(busPostRetirementIncreaseBatchRequest abusPostRetirementIncrease,
            int aintPayeeAccountId, int aintPaymentItemTypeId)
        {
            if (abusPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.IsNull())
                abusPostRetirementIncrease.LoadPostRetirementIncreaseBatchRequestDetail();
            cdoPostRetirementIncreaseBatchRequestDetail lobjPostRetirementIncreaseBatchDetail = new cdoPostRetirementIncreaseBatchRequestDetail();
            lobjPostRetirementIncreaseBatchDetail = abusPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.Where(lobjPostRetirementIncsDetail =>
                                                    lobjPostRetirementIncsDetail.post_retirement_increase_batch_request_id == abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id
                                                    && lobjPostRetirementIncsDetail.payee_account_payment_item_type_id == aintPaymentItemTypeId &&
                                                    lobjPostRetirementIncsDetail.payee_account_id == aintPayeeAccountId).FirstOrDefault();
            return lobjPostRetirementIncreaseBatchDetail;
        }
    }
}
