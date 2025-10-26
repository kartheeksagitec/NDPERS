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
    class busProcessCOLABatch : busNeoSpinBatch
    {
        public busProcessCOLABatch()
        { }
        //Process each 200 records at one time
        public void ProcessCOLA()
        {
            istrProcessName = "Process COLA Batch";
            idlgUpdateProcessLog("Step : 1 Get all batch request For COLA batch processing", "INFO", istrProcessName);


           
            bool lblnSuccess = false;
            int lintRequestId = 0;
            DataTable ldtbList = busBase.Select<cdoPostRetirementIncreaseBatchRequest>(new string[4] { "POST_RETIREMENT_INCREASE_TYPE_VALUE", "BATCH_REQUEST_STATUS_VALUE", 
                                                                                                         "ACTION_STATUS_VALUE", "STATUS_VALUE" }, new object[4] 
                                                                                                     { busConstant.PostRetirementIncreaseTypeValueCOLA,
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
                    int lintProcessedCount = 0;
                    lblnSuccess = false;
                    busPostRetirementIncreaseBatchRequest lobjPostRetirementIncrease = new busPostRetirementIncreaseBatchRequest
                    {
                        icdoPostRetirementIncreaseBatchRequest = new cdoPostRetirementIncreaseBatchRequest()
                    };
                    lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.LoadData(dr);

                    //load details records if exist
                    if (lobjPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.IsNull())
                        lobjPostRetirementIncrease.LoadPostRetirementIncreaseBatchRequestDetail();

                    idlgUpdateProcessLog("Step : 2 Get all Payee records For COLA batch processing for Batch request ID " + lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id.ToString(), "INFO", istrProcessName);

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
                                    ProcessCOLAPerPlan(lobjPostRetirementIncrease, ldtbPayeeListTobeProcessed.Rows[lintTotalCount]);

                                    lintTotalCount++;
                                    lintProcessedCount++;
                                }
                                idlgUpdateProcessLog(lintProcessedCount.ToString() + " Records Processed of " + ldtbPayeeListTobeProcessed.Rows.Count.ToString(), "INFO", istrProcessName);
                                iobjPassInfo.Commit();
                            }
                            catch (Exception e)
                            {
                                idlgUpdateProcessLog("Process COLA Batch Failed at Processing request ID  " + lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id.ToString()
                                                    + "and Person id - " + lintPayeePerslinkId + " due to following exception - " + e.ToString(), "INFO", istrProcessName);
                                iobjPassInfo.Rollback();
                                break;
                            }
                            lblnSuccess = true;
                        }
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
                                    //idlgUpdateProcessLog("Insert Or Update Post Retirement Increase Batch Request Detail for Post retirement Increase Batch Request ID = "
                                    //                   + lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id
                                    //                    , "INFO", istrProcessName);

                                    lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.batch_request_status_value = busConstant.PostRetirementIncreaseBatchTypeValueProcessed;
                                    lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.status_value = busConstant.StatusProcessed; // UAT PIR ID 1392
                                    lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.Update();

                                    CreateCorrespondence("PAY-4203", lobjPostRetirementIncrease);

                                    GenerateCOLAReport(lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id,
                                        lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date);

                                    iobjPassInfo.Commit();
                                }
                                catch (Exception e)
                                {
                                    idlgUpdateProcessLog("Process COLA Batch Failed at Processing request ID  " + lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id.ToString() + " due to following exception - "
                                                                   + e.ToString(), "INFO", istrProcessName);
                                    iobjPassInfo.Rollback();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        idlgUpdateProcessLog("No Payee Account found for the for Post retirement Increase Batch Request ID = "
                                                                  + lobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id.ToString()
                                                                   , "INFO", istrProcessName);
                    }
                }
            }
            if (lblnSuccess)
                idlgUpdateProcessLog("Process COLA Batch ended", "INFO", istrProcessName);
        }
        public void ProcessCOLAPerPlan(busPostRetirementIncreaseBatchRequest abusPostRetirementIncrease, DataRow adrowPayee)
        {
            decimal ldecbaseAmount = 0.00M;
            decimal ldecMonthlyCOLAIncrease = 0.00M;
            decimal ldecOldCOLAAmount = 0.00M;
            decimal ldecAdjustedCOLAAmount = 0.00M;
            decimal ldecMonths = 0.00M;
            int lintNumberOfMonths = 0;
            decimal lintMonthsDiff = 0;

            abusPostRetirementIncrease.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.LoadData(adrowPayee);

            if (abusPostRetirementIncrease.ibusPayeeAccount.ibusPlan.IsNull())
            {
                abusPostRetirementIncrease.ibusPayeeAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                abusPostRetirementIncrease.ibusPayeeAccount.ibusPlan.icdoPlan.LoadData(adrowPayee);
            }
            if (abusPostRetirementIncrease.ibusPayeeAccount.ibusApplication.IsNull())
                abusPostRetirementIncrease.ibusPayeeAccount.LoadApplication();

            //idlgUpdateProcessLog("Step : 3 Process COLA for Payee Perslink ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id
            //                                                                                                    , "INFO", istrProcessName);

            //load all Payment item types           
            abusPostRetirementIncrease.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

            //load Base amount
            abusPostRetirementIncrease.ibusPayeeAccount.LoadGrossAmount(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.base_date);
            ldecbaseAmount = abusPostRetirementIncrease.ibusPayeeAccount.idecGrossAmount + abusPostRetirementIncrease.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.paid_up_annuity_amount;
            
            // PIR 13731 - Commented code and added after proration logic

            ////load COLA increase
            //if (abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount > 0)
            //{
            //    ldecMonthlyCOLAIncrease = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount;
            //}
            //else
            //{
            //    ldecMonthlyCOLAIncrease = (ldecbaseAmount * abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_percentage) / 100;
            //    //PIR-10483 for the Monthly Cola increase less than $1 we have to set the value as $1 .
            //    if (ldecMonthlyCOLAIncrease < 1.0m)
            //    {
            //        ldecMonthlyCOLAIncrease = Math.Ceiling(ldecMonthlyCOLAIncrease);
            //    }
            //    else
            //    {
            //        ldecMonthlyCOLAIncrease = busPersonBase.Slice(ldecMonthlyCOLAIncrease, 0);// PROD PIR 8341
            //    }
            //}

            //now get adjusted COLA

            DateTime benefit_begin_date = abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
            //Backlog PIR 13506 
            //If person has done disability to normal conversion then pick benefit begin date of disability acc and not from retirement benefit acc.
            if (abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDisabilitytoNormal)
            {
                //abusPostRetirementIncrease.ibusPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() }; //16758
                //abusPostRetirementIncrease.ibusPayeeAccount.ibusPayee.icdoPerson.person_id = abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id;
                abusPostRetirementIncrease.ibusPayeeAccount.LoadPayee(); //16758
                abusPostRetirementIncrease.ibusPayeeAccount.ibusPayee.LoadPayeeAccount();

                //filter disability acc of that person.
                busPayeeAccount ibusPayeeAcc = abusPostRetirementIncrease.ibusPayeeAccount.ibusPayee.iclbPayeeAccount.Where(i => i.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability).FirstOrDefault();
                if (ibusPayeeAcc.IsNotNull())
                    benefit_begin_date = ibusPayeeAcc.icdoPayeeAccount.benefit_begin_date;
            }

            lintNumberOfMonths = busGlobalFunctions.DateDiffByMonth(benefit_begin_date, abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date);

            if (lintNumberOfMonths > 1)
                lintNumberOfMonths = lintNumberOfMonths - 1;

            lintMonthsDiff = lintNumberOfMonths;

            /* UAT PIR: 2416 COLA Proration logic provided by Leon
             * 1)      Benefit Type is Retirement or Disability:     if the numbers of Months between Payee Account Benefit begin date and COLA Effective date is less than 12.
               2)      Benefit Type: Pre Retirement :  if the number of months between Payee Account Benefit Begin date and COLA Effective date is less than 12. 
               3)      Benefit Type: Post Retirement:  :   if the numbers of Member payments plus joint annuitant payments and the COLA Effective date are less than 12.             
              */

            //check if the member dies within one year of retirement if so then joint annuitant pension benefits will be calculated
            //else member payee account will continue.
            bool blnAllowProration = false;
            int lintProrationMonths = 0;
            if (abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
            {                
                if (abusPostRetirementIncrease.ibusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.IsNull())
                    abusPostRetirementIncrease.ibusPayeeAccount.ibusApplication.LoadOriginatingPayeeAccount();

                if ((abusPostRetirementIncrease.ibusPayeeAccount.ibusApplication.IsNotNull()) &&
                    (abusPostRetirementIncrease.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.originating_payee_account_id > 0)
                    )
                {
                    lintProrationMonths = abusPostRetirementIncrease.ibusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.GetAlreadyPaidNumberofPayments();
                    //lintProrationMonths = busGlobalFunctions.DateDiffByMonth(abusPostRetirementIncrease.ibusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                    //abusPostRetirementIncrease.ibusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_end_date);

                    if ((lintProrationMonths + lintNumberOfMonths) / 12 < 1)
                    {
                        blnAllowProration = true;
                        lintMonthsDiff = lintProrationMonths + lintNumberOfMonths;
                    }
                }
            }
            else if ((abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) ||
                (abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
                (abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
            {
                if (lintNumberOfMonths / 12 < 1)
                {
                    blnAllowProration = true;
                }
            }


            //if (lblnIsCOLAAllowed)
            //{
            //PIR: 1941
            //Proration is done for a COLA Member only if the number of months is less than a year otherwise do not prorate.

            //PIR 13731 - Calculating Monthly Percentage Increase and Adjusted COLA Amount

            if (blnAllowProration)
            {
                ldecMonths = (lintMonthsDiff)/12;
                ldecMonthlyCOLAIncrease = ((abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_percentage / 100) * Math.Round(ldecMonths, 3));
            }
            else
            {
                ldecMonthlyCOLAIncrease = (abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_percentage) / 100;
            }
            //}
            //BR-084-08
            ldecMonthlyCOLAIncrease = Math.Round(ldecMonthlyCOLAIncrease, 3);

            //load COLA increase

            if (abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount > 0)
            {
                ldecMonthlyCOLAIncrease = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount;
            }
            else
            {
                ldecAdjustedCOLAAmount = (ldecbaseAmount * ldecMonthlyCOLAIncrease);
                //PIR-10483 for the Monthly Cola increase less than $1 we have to set the value as $1 .
                if (ldecAdjustedCOLAAmount < 1.0m)
                {
                    ldecAdjustedCOLAAmount = Math.Ceiling(ldecAdjustedCOLAAmount);
                }
                else
                {
                    ldecAdjustedCOLAAmount = busPersonBase.Slice(ldecAdjustedCOLAAmount, 0);// PROD PIR 8341
                }
            }

            if (ldecAdjustedCOLAAmount > 0.00M)
            {
                var lPaymentItemTypeList = abusPostRetirementIncrease.ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(lobjPayItemType => lobjPayItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITCOLABase
                     && busGlobalFunctions.CheckDateOverlapping(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                                lobjPayItemType.icdoPayeeAccountPaymentItemType.start_date, lobjPayItemType.icdoPayeeAccountPaymentItemType.end_date));

                foreach (var lobjPaymentItemType in lPaymentItemTypeList)
                {
                    ldecOldCOLAAmount += lobjPaymentItemType.icdoPayeeAccountPaymentItemType.amount;

                    if (lobjPaymentItemType.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                    {
                        lobjPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date.AddDays(-1);

                        //idlgUpdateProcessLog("Step : 4 Update payment Payee Account Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
                        //                                                                                            , "INFO", istrProcessName);

                        lobjPaymentItemType.icdoPayeeAccountPaymentItemType.Update();
                    }
                }

                //idlgUpdateProcessLog("Step : 5 Insert New Payee Account payment Item Type for Payee Account ID = " + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id
                //                                                                                               , "INFO", istrProcessName);
                int lintPayemntItemTypeID = abusPostRetirementIncrease.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITCOLABase);
                
                // UAT PIR 1341
                //ldecAdjustedCOLAAmount = ldecAdjustedCOLAAmount 
                //insert the new cola amount as new payment type record for this payee account
                busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_id = abusPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id = lintPayemntItemTypeID;
                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date;
                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount = ldecAdjustedCOLAAmount + ldecOldCOLAAmount;
                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = busConstant.BatchScheduleIDProcessCOLABatch;
                lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.Insert();

                //calculate the adjustment for the new COLA
                //idlgUpdateProcessLog("Step : 6 Update Post Retirement Increase Batch Request Detail for Payee Account ID = "
                //                                  + lobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, "INFO", istrProcessName);                
                abusPostRetirementIncrease.ibusPayeeAccount.iintBatchScheudleID = busConstant.BatchScheduleIDProcessCOLABatch;
                abusPostRetirementIncrease.ibusPayeeAccount.idtStatusEffectiveDate = abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date;
                abusPostRetirementIncrease.ibusPayeeAccount.iblnIsColaOrAdhocIncreaseTaxCalculation = true;

                abusPostRetirementIncrease.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();

                //UAT PIR 1942 - Load the Old Benefit Amount based on effective date -1 month. Discussed with David
                abusPostRetirementIncrease.ibusPayeeAccount.LoadTaxAmounts(abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.effective_date.AddMonths(-1));
                abusPostRetirementIncrease.idecFedTaxAmount = abusPostRetirementIncrease.ibusPayeeAccount.idecFedTaxAmount;
                abusPostRetirementIncrease.idecStateTaxAmount = abusPostRetirementIncrease.ibusPayeeAccount.idecStateTaxAmount;

                abusPostRetirementIncrease.ibusPayeeAccount.CalculateAdjustmentTax(false);
                abusPostRetirementIncrease.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                //check if the detail already exist then update else insert

                InsertOrUpdatePostRetirementIncreaseDetail(abusPostRetirementIncrease, lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id,
                                                        ldecbaseAmount - abusPostRetirementIncrease.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.paid_up_annuity_amount, ldecAdjustedCOLAAmount, lintPayemntItemTypeID);

                abusPostRetirementIncrease.LoadPostRetirementIncreaseBatchRequestDetail();
                //load methods for correspondence
                abusPostRetirementIncrease.LoadBenefitAmount();
                abusPostRetirementIncrease.LoadNewBenefitAmount();
                //generate correspondence
                CreateCorrespondence("PAY-4204", abusPostRetirementIncrease);
            }
        }

        private void InsertOrUpdatePostRetirementIncreaseDetail(busPostRetirementIncreaseBatchRequest aobjPostRetirementIncrease, int aintPayeeAccountPaymentItemTypeID,
                                                                decimal adecCurrentTax, decimal adecIncreasedAmount, int aintPaymentItemTypeId)
        {
            cdoPostRetirementIncreaseBatchRequestDetail lobjPostretirementincreaseDetailFedTax = new cdoPostRetirementIncreaseBatchRequestDetail();
            lobjPostretirementincreaseDetailFedTax = GetDuplicateRecord(aobjPostRetirementIncrease, aobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, aintPaymentItemTypeId);
            if (lobjPostretirementincreaseDetailFedTax.IsNotNull())
            {
                if (lobjPostretirementincreaseDetailFedTax.post_retirement_increase_batch_request_detail_id > 0)
                {
                    lobjPostretirementincreaseDetailFedTax.is_processed_flag = busConstant.Flag_Yes;
                    lobjPostretirementincreaseDetailFedTax.original_amount = adecCurrentTax;
                    lobjPostretirementincreaseDetailFedTax.increase_amount = adecCurrentTax + adecIncreasedAmount;
                    lobjPostretirementincreaseDetailFedTax.Update();

                    //idlgUpdateProcessLog("Step : 8 Update Post Retirement Increase Batch Request Detail  for Payee Account ID = "
                    //                              + aobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, "INFO", istrProcessName);
                }
            }
            else
            {
                lobjPostretirementincreaseDetailFedTax = new cdoPostRetirementIncreaseBatchRequestDetail();
                //insert into POST RETIREMENT INCREASE BATCH REQUEST detail table                                
                lobjPostretirementincreaseDetailFedTax.is_processed_flag = busConstant.Flag_Yes;
                lobjPostretirementincreaseDetailFedTax.original_amount = adecCurrentTax;
                lobjPostretirementincreaseDetailFedTax.increase_amount = adecCurrentTax + adecIncreasedAmount;
                lobjPostretirementincreaseDetailFedTax.payee_account_id = aobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPostretirementincreaseDetailFedTax.payee_account_payment_item_type_id = aintPayeeAccountPaymentItemTypeID;
                lobjPostretirementincreaseDetailFedTax.payment_item_type_id = aintPaymentItemTypeId;
                lobjPostretirementincreaseDetailFedTax.post_retirement_increase_batch_request_id = aobjPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id;
                lobjPostretirementincreaseDetailFedTax.Insert();

                //idlgUpdateProcessLog("Step : 8 Insert Post Retirement Increase Batch Request Detail for Payee Account ID = "
                //                              + aobjPostRetirementIncrease.ibusPayeeAccount.icdoPayeeAccount.payee_account_id, "INFO", istrProcessName);

            }
        }

        //check if post retirement increase detail record exist with these values
        private cdoPostRetirementIncreaseBatchRequestDetail GetDuplicateRecord(busPostRetirementIncreaseBatchRequest abusPostRetirementIncrease, int aintPayeeAccountId, int aintPaymentItemTypeID)
        {
            if (abusPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.IsNull())
                abusPostRetirementIncrease.LoadPostRetirementIncreaseBatchRequestDetail();
            cdoPostRetirementIncreaseBatchRequestDetail lobjPostRetirementIncreaseBatchDetail = new cdoPostRetirementIncreaseBatchRequestDetail();
            lobjPostRetirementIncreaseBatchDetail = abusPostRetirementIncrease.iclbPorRetirementIncreaseBatchRequestDetail.Where(lobjPostRetirementIncsDetail =>
                                                    lobjPostRetirementIncsDetail.post_retirement_increase_batch_request_id == abusPostRetirementIncrease.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id
                                                    && lobjPostRetirementIncsDetail.payee_account_payment_item_type_id == busConstant.PAPITCOLABaseItemId &&
                                                    lobjPostRetirementIncsDetail.payee_account_id == aintPayeeAccountId).FirstOrDefault();

            return lobjPostRetirementIncreaseBatchDetail;
        }

        private void CreateCorrespondence(string astrCorTemplateName, busPostRetirementIncreaseBatchRequest abusPostRetirementIncreaseBatchRequest)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(abusPostRetirementIncreaseBatchRequest);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");

            //idlgUpdateProcessLog("Step : 9 Generate correspondence JS or Trav COLA letter for Payee Perslink ID = " + aintPersonId + " and Payee Account Id = " + aintPayeeAccountId
            //                                                                                                          , "INFO", istrProcessName);

            CreateCorrespondence(astrCorTemplateName, abusPostRetirementIncreaseBatchRequest, lhstDummyTable);
            //CreateCorrespondence("PAY-4204", iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, larrlist, lhstDummyTable);

            //idlgUpdateProcessLog("Step : 10 Generate correspondence JS 3rd Party Payor COLA letter for Payee Perslink ID = " + aintPersonId + " and Payee Account Id = " + aintPayeeAccountId
            //                                                                                                         , "INFO", istrProcessName);

            //CreateCorrespondence("PAY-4203", iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, larrlist, lhstDummyTable);
        }

        DataTable idtResultTable = new DataTable();
        public void GenerateCOLAReport(int aintRequestID, DateTime adtEffectiveDate)
        {
            istrProcessName = "Generating Job Service COLA Report for 3rd Party Payor";

            idtResultTable = CreateCOLABatchReportDataset();

            DateTime ldtBatchRunDate = iobjSystemManagement.icdoSystemManagement.batch_date;

            DataTable ldtbProcess = busBase.Select("cdoPostRetirementIncreaseBatchRequest.rptJobServiceCOLAReportfor3rdPartPayorForBatch",
                                                                                      new object[2] { aintRequestID, adtEffectiveDate });

            foreach (DataRow dr in ldtbProcess.Rows)
            {
                AddToNewDataRow(dr);
            }

            if (idtResultTable.Rows.Count > 0)
            {
                //create report for Insufficient report details
                CreateReport("rptJobServiceCOLAReportfor3rdPartPayorForBatch.rpt", idtResultTable);

                idlgUpdateProcessLog("Generating Job Service COLA report for 3rd Party Payor generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }

        }

        private DataTable CreateCOLABatchReportDataset()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("Payee_Perslink_ID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("Payee_Name", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("SSN", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("Monthly_COLA_Amount", Type.GetType("System.Decimal"));
            DataColumn ldc5 = new DataColumn("Effective_Date", Type.GetType("System.DateTime"));
            DataColumn ldc6 = new DataColumn("Cumulative_COLA_increases", Type.GetType("System.Decimal"));

            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);

            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(DataRow adrow)
        {
            DataRow dr = idtResultTable.NewRow();

            dr["Payee_Perslink_ID"] = adrow["Payee_Perslink_ID"];
            dr["Payee_Name"] = adrow["Payee_Name"];
            dr["SSN"] = adrow["SSN"];
            dr["Monthly_COLA_Amount"] = adrow["Monthly_COLA_Amount"];
            dr["Effective_Date"] = adrow["Effective_Date"];
            dr["Cumulative_COLA_increases"] = adrow["Cumulative_COLA_increases"];

            idtResultTable.Rows.Add(dr);
        }
    }
}
