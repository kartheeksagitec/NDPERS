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
    class busProcessAdhocIncreaseBatch : busNeoSpinBatch
    {
        public busProcessAdhocIncreaseBatch()
        { }

        //Process each 200 records at one time
        public void ProcessAdhocIncrease()
        {
            istrProcessName = "Process Adhoc Increase Batch";
            idlgUpdateProcessLog("Step : 1 Get all batch request For Adhoc batch processing", "INFO", istrProcessName);
           
            int lintProcessedCount = 0;
            bool lblnSuccess = false;
            int lintRequestId = 0;
            DataTable ldtbList = busBase.Select<cdoPostRetirementIncreaseBatchRequest>(new string[4] { "POST_RETIREMENT_INCREASE_TYPE_VALUE", "BATCH_REQUEST_STATUS_VALUE", 
                                                                                                         "ACTION_STATUS_VALUE", "STATUS_VALUE" }, new object[4] 
                                                                                                     { busConstant.PostRetirementIncreaseTypeValueAdHoc,
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

                    idlgUpdateProcessLog("Step : 2 Get all Payee records For batch processing for request id " +
                    lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id
                    , "INFO", istrProcessName);

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
                                    lintPayeePerslinkId = (int)ldtbPayeeListTobeProcessed.Rows[lintTotalCount]["payee_perslink_id"];
                                    ProcessAdhocIncreasePerPlan(lobjPostRetirementIncrease, ldtbPayeeListTobeProcessed.Rows[lintTotalCount]);

                                    lintTotalCount++;
                                    lintProcessedCount++;
                                }
                                idlgUpdateProcessLog(lintProcessedCount.ToString() + " Records Processed of " + ldtbPayeeListTobeProcessed.Rows.Count.ToString(), "INFO", istrProcessName);
                                iobjPassInfo.Commit();
                            }
                            catch (Exception e)
                            {
                                idlgUpdateProcessLog("Process Adhoc Batch Failed at Processing request ID  " + lintRequestId.ToString() + "and Person id - "
                                                            + lintPayeePerslinkId + " due to following exception - " + e.ToString(), "INFO", istrProcessName);
                                iobjPassInfo.Rollback();
                                break;
                            }
                            lblnSuccess = true;
                        }
                        if (lblnSuccess)
                        {
                            lobjPostRetirementIncrease.LoadPostRetirementIncreaseBatchRequestDetail();
                            //if the number of detail records are equal to number of total records for the batch request
                            //update the status of batch request as Processed
                            lintDetailCount = lobjPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.Count();

                            lintTotalProcessDetailCount = lobjPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.Where(lobjDtl => lobjDtl.is_processed_flag == busConstant.Flag_Yes).Count();
                            if (lintDetailCount.Equals(lintTotalProcessDetailCount))
                            {
                                iobjPassInfo.BeginTransaction();

                                idlgUpdateProcessLog("Step : 3 Insert Or Update Post Retirement Increase Batch Request Detail for Post retirement Increase Batch Request ID = "
                                                   + lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id
                                                    , "INFO", istrProcessName);

                                lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.batch_request_status_value = busConstant.PostRetirementIncreaseBatchTypeValueProcessed;
                                lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.status_value = busConstant.StatusProcessed; // UAT PIR ID 1392
                                lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.Update();
                                iobjPassInfo.Commit();
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
                idlgUpdateProcessLog("Process Adhoc Batch ended", "INFO", istrProcessName);
        }

        private void ProcessAdhocIncreasePerPlan(busPostRetirementIncreaseBatchRequest abusPostRetirementIncrease, DataRow adrowPayee)
        {
            decimal ldecbaseAmount = 0.00M;
            decimal ldecMonthlyAdhocIncrease = 0.00M;
            decimal ldecOldAdhocAmount = 0.00M;
            abusPostRetirementIncrease.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.LoadData(adrowPayee);
            
            if (abusPostRetirementIncrease.ibusPayeeAccount.ibusPlan.IsNull())
            {
                abusPostRetirementIncrease.ibusPayeeAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                abusPostRetirementIncrease.ibusPayeeAccount.ibusPlan.icdoPlan.LoadData(adrowPayee);
            }

            //idlgUpdateProcessLog("Step : 3 Process Ad hoc for Payee Perslink ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id
            //     
          
            //load all Payment item types
            abusPostRetirementIncrease.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

            //load Base amount
            abusPostRetirementIncrease.ibusPayeeAccount.LoadGrossAmount(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.base_date);
            ldecbaseAmount = abusPostRetirementIncrease.ibusPayeeAccount.idecGrossAmount;

            //load COLA increase
            if (abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount > 0)
            {
                ldecMonthlyAdhocIncrease = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount;
            }
            else
            {
                ldecMonthlyAdhocIncrease = (ldecbaseAmount * abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_percentage) / 100;
                ldecMonthlyAdhocIncrease = busGlobalFunctions.RoundToPenny(ldecMonthlyAdhocIncrease);
            }

          
            var lPaymentItemTypeList = abusPostRetirementIncrease.ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(lobjPayItemType => lobjPayItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITAdhoc
                && busGlobalFunctions.CheckDateOverlapping(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                                lobjPayItemType.icdoPayeeAccountPaymentItemType.start_date, lobjPayItemType.icdoPayeeAccountPaymentItemType.end_date));

            foreach (var lobjPaymentItemType in lPaymentItemTypeList)
            {
                ldecOldAdhocAmount += lobjPaymentItemType.icdoPayeeAccountPaymentItemType.amount;

                if (lobjPaymentItemType.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                {
                    lobjPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date.AddDays(-1);

                    //idlgUpdateProcessLog("Step : 4 Update payment Payee Account Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
                    //                                                                                            , "INFO", istrProcessName);
                    //as per meeting with satya on Aug 13,2010
                    lobjPaymentItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = busConstant.BatchScheduleIDProcessAdhocBatch;
                    lobjPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
                }
            }

            //idlgUpdateProcessLog("Step : 5 Insert New Payee Account payement Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
            //                                                                                                 , "INFO", istrProcessName);


            //insert the new cola amount as new payment type record for this payee account
            busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_id = abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id = abusPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITAdhoc);
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date;
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount = ldecMonthlyAdhocIncrease + ldecOldAdhocAmount;
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = busConstant.BatchScheduleIDProcessAdhocBatch; 
            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Insert();

            abusPostRetirementIncrease.ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Add(lobjPayeeAccountPaymentItemType);


            //**************************************************************************//
            //**************************TO CALCULATE STATE TAX *******************************//
            //**************************************************************************//
            //idlgUpdateProcessLog("Step : 6 Calculate State Tax for payment Payee Account Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
            //                                                                                                , "INFO", istrProcessName);

            decimal ldecCurrentStateTax = 0.00M;
            decimal ldecCurrentStateTaxAfterIncrease = 0.00M;
            int lintNewPayeeAccountStatePaymentItemTypeID = 0;

            int lintPaymentItemTypeId = abusPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDStateTaxAmount);
            //get the current state tax
            var lenumPAPITStateTaxCurrentTax = abusPostRetirementIncrease.ibusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                .Where(lobjPA => lobjPA.icdoPayeeAccountPaymentItemType.payment_item_type_id == lintPaymentItemTypeId
                                                                && busGlobalFunctions.CheckDateOverlapping(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                                lobjPA.icdoPayeeAccountPaymentItemType.start_date, lobjPA.icdoPayeeAccountPaymentItemType.end_date));
            if (lenumPAPITStateTaxCurrentTax.Count() > 0)
                ldecCurrentStateTax = lenumPAPITStateTaxCurrentTax.FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;

            //**************************************************************************//
            //**************************TO CALCULATE FED TAX *******************************//
            //**************************************************************************//
            //idlgUpdateProcessLog("Step : 7 Calculate Fed Tax for payment Payee Account Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
            //                                                                                                , "INFO", istrProcessName);

            decimal ldecCurrentFedTax = 0.00M;
            decimal ldecCurrentFedTaxAfterIncrease = 0.00M;
            int lintNewPayeeAccountFedPaymentItemTypeID = 0;

            int lintPaymentItemTypeIdFedTax = abusPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITFederalTaxAmount);
            //get the current state tax

            var lenumPAPITCurrentFedTax = abusPostRetirementIncrease.ibusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                    .Where(lobjPA => lobjPA.icdoPayeeAccountPaymentItemType.payment_item_type_id == lintPaymentItemTypeIdFedTax
                                                                    && busGlobalFunctions.CheckDateOverlapping(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                                    lobjPA.icdoPayeeAccountPaymentItemType.start_date, lobjPA.icdoPayeeAccountPaymentItemType.end_date));

            if (lenumPAPITCurrentFedTax.Count() > 0)
                ldecCurrentFedTax = lenumPAPITCurrentFedTax.FirstOrDefault().icdoPayeeAccountPaymentItemType.amount;


            //calculate the tax for the new adhoc amount
            abusPostRetirementIncrease.ibusPayeeAccount.iintBatchScheudleID = busConstant.BatchScheduleIDProcessAdhocBatch;
            abusPostRetirementIncrease.ibusPayeeAccount.idtStatusEffectiveDate = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date;
            abusPostRetirementIncrease.ibusPayeeAccount.iblnIsColaOrAdhocIncreaseTaxCalculation = true;
            //To Update the Newly Added the Adhoc COLA Amount
            abusPostRetirementIncrease.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();   
            abusPostRetirementIncrease.ibusPayeeAccount.CalculateAdjustmentTax(false);
            abusPostRetirementIncrease.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            //To Update the Newly Added the Fed and State tax amounts.
            //get the increase in state tax for new adhoc amount
            var lenumPAPITStateTaxIncreasedTax = abusPostRetirementIncrease.ibusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                          .Where(lobjPA => lobjPA.icdoPayeeAccountPaymentItemType.payment_item_type_id == lintPaymentItemTypeId
                                                          && busGlobalFunctions.CheckDateOverlapping(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                          lobjPA.icdoPayeeAccountPaymentItemType.start_date, lobjPA.icdoPayeeAccountPaymentItemType.end_date));
            if (lenumPAPITStateTaxIncreasedTax.Count() > 0)
            {
                ldecCurrentStateTaxAfterIncrease = lenumPAPITStateTaxIncreasedTax.FirstOrDefault().icdoPayeeAccountPaymentItemType.amount - ldecCurrentStateTax;                
                lintNewPayeeAccountStatePaymentItemTypeID = lenumPAPITStateTaxIncreasedTax.FirstOrDefault().icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
            }
            //check if the detail already exist then update else insert

            InsertOrUpdatePostRetirementIncreaseDetail(abusPostRetirementIncrease, lintNewPayeeAccountStatePaymentItemTypeID
                                                         , ldecCurrentStateTax, ldecCurrentStateTaxAfterIncrease, lintPaymentItemTypeId);

            //get the increase in state tax for new adhoc amount
            var lenumPAPITIncreasedFedTax = abusPostRetirementIncrease.ibusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                          .Where(lobjPA => lobjPA.icdoPayeeAccountPaymentItemType.payment_item_type_id == lintPaymentItemTypeIdFedTax
                                                          && busGlobalFunctions.CheckDateOverlapping(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                          lobjPA.icdoPayeeAccountPaymentItemType.start_date, lobjPA.icdoPayeeAccountPaymentItemType.end_date));
            if (lenumPAPITIncreasedFedTax.Count() > 0)
            {
                ldecCurrentFedTaxAfterIncrease = lenumPAPITIncreasedFedTax.FirstOrDefault().icdoPayeeAccountPaymentItemType.amount - ldecCurrentFedTax;
                lintNewPayeeAccountFedPaymentItemTypeID = lenumPAPITIncreasedFedTax.FirstOrDefault().icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
            }
            //check if the detail already exist then update else insert
            InsertOrUpdatePostRetirementIncreaseDetail(abusPostRetirementIncrease, lintNewPayeeAccountFedPaymentItemTypeID
                                                                     , ldecCurrentFedTax, ldecCurrentFedTaxAfterIncrease, lintPaymentItemTypeIdFedTax);

            //**************************** END **************************************// 
            //**************************************************************************//
            //**************************INSERT / UPDATE ADHOC AMOUNT*******************************//
            //**************************************************************************//
            //check if the detail already exist then update else insert
            int lintPaymentItemTypeIdAdhoc = abusPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITAdhoc);
            InsertOrUpdatePostRetirementIncreaseDetail(abusPostRetirementIncrease, lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id, ldecbaseAmount, ldecMonthlyAdhocIncrease, lintPaymentItemTypeIdAdhoc);

            //for correspondence
            abusPostRetirementIncrease.LoadPostRetirementIncreaseBatchRequestDetail(); 
            abusPostRetirementIncrease.LoadBenefitAmount();
            abusPostRetirementIncrease.LoadNewBenefitAmount();

            //generate correspondence
            CreateCorrespondence(abusPostRetirementIncrease, abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id);
        }

        private void InsertOrUpdatePostRetirementIncreaseDetail(busPostRetirementIncreaseBatchRequest aobjPostRetirementIncrease, int aintPayeeAccountPaymentItemTypeId,
                                                                decimal adecCurrentTax, decimal adecIncreasedAmount, int aintPaymentItemTypeId)
        {
            cdoPostRetirementIncreaseBatchRequestDetail lobjPostretirementincreaseDetail = new cdoPostRetirementIncreaseBatchRequestDetail();
            lobjPostretirementincreaseDetail = GetDuplicateRecord(aobjPostRetirementIncrease, aobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, aintPaymentItemTypeId);
            if (lobjPostretirementincreaseDetail.IsNotNull())
            {
                if (lobjPostretirementincreaseDetail.post_retirement_increase_batch_request_detail_id > 0)
                {
                    lobjPostretirementincreaseDetail.is_processed_flag = busConstant.Flag_Yes;
                    lobjPostretirementincreaseDetail.original_amount = adecCurrentTax;
                    lobjPostretirementincreaseDetail.increase_amount = adecCurrentTax + adecIncreasedAmount;
                    lobjPostretirementincreaseDetail.Update();

                    //idlgUpdateProcessLog("Step : 8 Update Post Retirement Increase Batch Request Detail  for Payee Account ID = "
                    //                              + aobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, "INFO", istrProcessName);
                }
            }
            else
            {
                //insert into POST RETIREMENT INCREASE BATCH REQUEST detail table     
                lobjPostretirementincreaseDetail = new cdoPostRetirementIncreaseBatchRequestDetail();
                lobjPostretirementincreaseDetail.is_processed_flag = busConstant.Flag_Yes;
                lobjPostretirementincreaseDetail.original_amount = adecCurrentTax;
                lobjPostretirementincreaseDetail.increase_amount = adecCurrentTax + adecIncreasedAmount;
                lobjPostretirementincreaseDetail.payee_account_id = aobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPostretirementincreaseDetail.payee_account_payment_item_type_id = aintPayeeAccountPaymentItemTypeId;
                lobjPostretirementincreaseDetail.payment_item_type_id = aintPaymentItemTypeId;
                lobjPostretirementincreaseDetail.post_retirement_increase_batch_request_id =
                                        aobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id;
                lobjPostretirementincreaseDetail.Insert();

                //idlgUpdateProcessLog("Step : 8 Insert Post Retirement Increase Batch Request Detail for Payee Account ID = "
                //                              + aobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, "INFO", istrProcessName);
                             
            }
        }

        //check if post retirement increase detail record exist with these values
        private cdoPostRetirementIncreaseBatchRequestDetail GetDuplicateRecord(busPostRetirementIncreaseBatchRequest aobjPostRetirementIncrease,
                                                                                                int aintPayeeAccountId, int astrPaymentItemTypeId)
        {
            if (aobjPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.IsNull())
                aobjPostRetirementIncrease.LoadPostRetirementIncreaseBatchRequestDetail();
            cdoPostRetirementIncreaseBatchRequestDetail lobjPostRetirementIncreaseBatchDetail = new cdoPostRetirementIncreaseBatchRequestDetail();
            lobjPostRetirementIncreaseBatchDetail = aobjPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.Where(lobjPostRetirementIncsDetail =>
                                                    lobjPostRetirementIncsDetail.post_retirement_increase_batch_request_id == aobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id
                                                    && lobjPostRetirementIncsDetail.payee_account_payment_item_type_id == aobjPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITAdhoc) &&
                                                    lobjPostRetirementIncsDetail.payee_account_id == aintPayeeAccountId).FirstOrDefault();

            return lobjPostRetirementIncreaseBatchDetail;
        }

        private void CreateCorrespondence(busPostRetirementIncreaseBatchRequest abusPostRetirementIncreaseBatchRequest, int aintPersonId, int aintPayeeAccountId)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(abusPostRetirementIncreaseBatchRequest);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");

            //idlgUpdateProcessLog("Step : 9 Generate correspondence BENEFIT INCREASE for Perslink Id = " + aintPersonId + " and Payee Account Id = " + aintPayeeAccountId
            //                                                                                                           , "INFO", istrProcessName);

            CreateCorrespondence("PAY-4202", abusPostRetirementIncreaseBatchRequest, lhstDummyTable);
        }
    }
}
