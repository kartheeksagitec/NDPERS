#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.CorBuilder;
using System.Linq;
using System.Linq.Expressions;
using Sagitec.ExceptionPub;
#endregion



namespace NeoSpinBatch
{
    class busPayeeErrorReport : busNeoSpinBatch
    {
        public busPayeeErrorReport()
        {

        }
        DataTable idtResultTable = new DataTable();
        busBase lobjBase = new busBase();
        private DateTime idtNextPaymentDate
        {
            get
            {
                return busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            }
        }

        //property to load all the payee accounts
        //public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }

        //property to load all Taxwithholding details
        //public Collection<busPayeeAccountTaxWithholding> iclbTaxWithholindingHistory { get; set; }

        //property to load all the payee account ach details
        //public Collection<busPayeeAccountAchDetail> iclbPayeeAccountAchDetail { get; set; }

        //property to load all the payee account rollover details
        //public Collection<busPayeeAccountRolloverDetail> iclbPayeeAccountRolloverDetail { get; set; }

        //property to load all the payee account details and status
        //public Collection<busPayeeAccountPaymentItemType> iclbPayeeAccountPaymentItemType { get; set; }
        //Load All the payee account considere next benefit payment 


        //Ashish Modified the DataTables as Class Variables 
        private DataTable idtbPayeeAccount { get; set; }
        private DataTable idtbTaxwithHoldingDetails { get; set; }
        private DataTable idtbPayeeAccountPaymentItemType { get; set; }
        private DataTable idtbPayeeAccountRolloverDetail { get; set; }
        private DataTable idtbAchDetails { get; set; }

        
       
        public void LoadPayeeAccounts()
        {
            //iclbPayeeAccount = new Collection<busPayeeAccount>();
            idtbPayeeAccount = busNeoSpinBase.Select("cdoPayeeAccount.LoadPayeeAcountAndStatus", new object[1] { idtNextPaymentDate });
            
                //foreach (DataRow drPayeeAccount in ldtbPayeeAccount.Rows)
                //{
                //    busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                //    lobjPayeeAccount.icdoPayeeAccount.LoadData(drPayeeAccount);
                //    lobjPayeeAccount.ibusApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                //    lobjPayeeAccount.ibusApplication.icdoBenefitApplication.LoadData(drPayeeAccount);
                //    if (lobjPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_application_id > 0)
                //    {
                //        lobjPayeeAccount.ibusApplication.icdoBenefitApplication.status_id = Convert.ToInt32(drPayeeAccount["Status_id1"]);
                //        lobjPayeeAccount.ibusApplication.icdoBenefitApplication.status_value = drPayeeAccount["Status_Value1"] == DBNull.Value ? "" : drPayeeAccount["Status_Value1"].ToString();
                //    }
                //    lobjPayeeAccount.ibusPayeeAccountActiveStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
                //    lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.LoadData(drPayeeAccount);
                //    if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.payee_account_status_id > 0)
                //    {
                //        lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_id = Convert.ToInt32(drPayeeAccount["Status_id2"]);
                //        lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value = drPayeeAccount["Status_Value2"] == DBNull.Value ? "" : drPayeeAccount["Status_Value2"].ToString();
                //    }
                //    //lobjPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
                //    //lobjPayeeAccount.ibusPayee.icdoPerson.LoadData(drPayeeAccount);
                //    //iclbPayeeAccount.Add(lobjPayeeAccount);
                //}
            
        }

       
        //Load all the payee account taxwithholiding details details into a collection
        public void LoadTaxwithHolingDetails()
        {
            idtbTaxwithHoldingDetails = busNeoSpinBase.Select("cdoPayeeAccountTaxWithholding.LoadTaxWithholingDetailsForNextPaymentDate", new object[1] { idtNextPaymentDate });
            //iclbTaxWithholindingHistory = lobjBase.GetCollection<busPayeeAccountTaxWithholding>(ldtbList, "icdoPayeeAccountTaxWithholding");
            
        }
        //Load all the payee account ACH details into a collection
        public void LoadAchDetails()
        {
            idtbAchDetails = busNeoSpinBase.Select("cdoPayeeAccountAchDetail.LoadACHDetailsForNextPaymentDate", new object[1] { idtNextPaymentDate });
            //iclbPayeeAccountAchDetail = lobjBase.GetCollection<busPayeeAccountAchDetail>(ldtbList, "icdoPayeeAccountAchDetail");
            
        }
        
        //Load all the payee account payment item details into a collection
        public void LoadPayeeAccountPaymentItemType()
        {
            idtbPayeeAccountPaymentItemType = busNeoSpinBase.Select("cdoPayeeAccountPaymentItemType.LoadPaymentItemsForNextPaymentDate", new object[1] { idtNextPaymentDate });
            //iclbPayeeAccountPaymentItemType = lobjBase.GetCollection<busPayeeAccountPaymentItemType>(ldtbList, "icdoPayeeAccountPaymentItemType");
            
        }

        
        //Load all the payee account rollover details into a collection
        public void LoadRolloverDetails()
        {
            idtbPayeeAccountRolloverDetail = busNeoSpinBase.Select("cdoPayeeAccountRolloverDetail.LoadRolloverDetailsForNextPaymentDate", new object[1] { idtNextPaymentDate });
            //iclbPayeeAccountRolloverDetail = lobjBase.GetCollection<busPayeeAccountRolloverDetail>(ldtbList, "icdoPayeeAccountRolloverDetail");
            
        }

        public void GeneratePayeeErrorReport()
        {
            //call the methods for validating payee accounts
            ValidatePayeeAccounts();

            istrProcessName = "Generating Payee Error Report";
            DataTable idtResultTable = busBase.Select("cdoPayeeAccount.LoadErrorReport", new object[1] { idtNextPaymentDate });

            if (idtResultTable.Rows.Count > 0)
            {
                var lenum = idtResultTable.AsEnumerable().Where(dr => dr.Field<String>("Payee_status_value") != busConstant.PayeeAccountStatusCancelled 
                            || (dr.Field<String>("Payee_status_value") == busConstant.PayeeAccountStatusCancelled && dr.Field<DateTime>("status_effective_date") > DateTime.Now.AddDays(-30)));

                if (lenum.Count() > 0)
                {
                    idtResultTable = lenum.AsDataTable();
                    //create report for Insufficient report details
                    CreateReport("rptPayeeErrorReport.rpt", idtResultTable);

                    idlgUpdateProcessLog("Payee Error Report generated succesfully", "INFO", istrProcessName);
                }
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
        }
        //Load ach details,rollover details,taxwithholding details and payment item details
        public void LoadPayeeAccountDetails()
        {
            if (idtbPayeeAccount == null)
                LoadPayeeAccounts();
            if (idtbPayeeAccountRolloverDetail == null)
                LoadRolloverDetails();
            if (idtbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            if (idtbAchDetails == null)
                LoadAchDetails();
            if (idtbTaxwithHoldingDetails == null)
                LoadTaxwithHolingDetails();
         }
        //Validate all the payee accounts to check whether it has taxwithholding record ,
        //rollover information are valid ,ach information are valid and the net amount is valid
        public void ValidatePayeeAccounts()
        {
            int lintrecordcount = 0;
            
                LoadPayeeAccountDetails();

                istrProcessName = "Validating Payee Accounts";
                DataRow[] ldrDataRows = null;
                foreach (DataRow drPayeeAccount in idtbPayeeAccount.Rows)
                {
                    lintrecordcount= lintrecordcount+1;
               
                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPayeeAccount.icdoPayeeAccount.LoadData(drPayeeAccount);
                    lobjPayeeAccount.ibusApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lobjPayeeAccount.ibusApplication.icdoBenefitApplication.LoadData(drPayeeAccount);
                    if (lobjPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_application_id > 0)
                    {
                        lobjPayeeAccount.ibusApplication.icdoBenefitApplication.status_id = Convert.ToInt32(drPayeeAccount["Status_id1"]);
                        lobjPayeeAccount.ibusApplication.icdoBenefitApplication.status_value = drPayeeAccount["Status_Value1"] == DBNull.Value ? "" : drPayeeAccount["Status_Value1"].ToString();
                    }
                    lobjPayeeAccount.ibusPayeeAccountActiveStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
                    lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.LoadData(drPayeeAccount);
                    if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.payee_account_status_id > 0)
                    {
                        lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_id = Convert.ToInt32(drPayeeAccount["Status_id2"]);
                        lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value = drPayeeAccount["Status_Value2"] == DBNull.Value ? "" : drPayeeAccount["Status_Value2"].ToString();
                    }

                    lobjPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusPayee.icdoPerson.LoadData(drPayeeAccount);

                    //Load payee account status
                    if (lobjPayeeAccount.ibusPayeeAccountActiveStatus == null)
                        lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                    if (!(lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusReview() || lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled()
                        || lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusPaymentCompleteOrProcessed()))
                    {
                        //Load benefit application details
                        if (lobjPayeeAccount.ibusApplication == null)
                            lobjPayeeAccount.LoadApplication();

                        //Load Payee details
                        if (lobjPayeeAccount.ibusPayee == null)
                            lobjPayeeAccount.LoadPayee();

                        //load ach details history and active ach details of the payee account
                        lobjPayeeAccount.iclbAchDetail = new Collection<busPayeeAccountAchDetail>();
                        lobjPayeeAccount.iclbActiveACHDetails = new Collection<busPayeeAccountAchDetail>();

                        ldrDataRows = busGlobalFunctions.FilterTable(idtbAchDetails, busConstant.DataType.Numeric, "payee_account_id", lobjPayeeAccount.icdoPayeeAccount.payee_account_id);
                        foreach (DataRow ldrAchDtls in ldrDataRows)
                        {
                            busPayeeAccountAchDetail lobjPayeeAccountAchDetail = new busPayeeAccountAchDetail { icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail() };
                            lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.LoadData(ldrAchDtls);
                            lobjPayeeAccount.iclbAchDetail.Add(lobjPayeeAccountAchDetail);
                            if (busGlobalFunctions.CheckDateOverlapping(idtNextPaymentDate, lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date,
                                lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date))
                            {
                                lobjPayeeAccount.iclbActiveACHDetails.Add(lobjPayeeAccountAchDetail);
                            }
                        }

                        //load rollover details history and active rollover details of the payee account
                        lobjPayeeAccount.iclbActiveRolloverDetails = new Collection<busPayeeAccountRolloverDetail>();
                        lobjPayeeAccount.iclbRolloverDetail = new Collection<busPayeeAccountRolloverDetail>();
                        //var lvarrollverdetails = ldtbPayeeAccountRolloverDetail.AsEnumerable().Where(dr => dr.Field <int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id);

                        ldrDataRows = busGlobalFunctions.FilterTable(idtbPayeeAccountRolloverDetail, busConstant.DataType.Numeric, "payee_account_id", lobjPayeeAccount.icdoPayeeAccount.payee_account_id);

                        foreach (DataRow ldrRollOverDtl in ldrDataRows)
                        {
                            busPayeeAccountRolloverDetail lobjPayeeAccountRolloverDetail = new busPayeeAccountRolloverDetail { icdoPayeeAccountRolloverDetail = new cdoPayeeAccountRolloverDetail() };
                            lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.LoadData(ldrRollOverDtl);
                            lobjPayeeAccount.iclbRolloverDetail.Add(lobjPayeeAccountRolloverDetail);
                            if (lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusActive)
                                lobjPayeeAccount.iclbActiveRolloverDetails.Add(lobjPayeeAccountRolloverDetail);
                        }
                        
                        //Load the payment items for the payee account 
                        lobjPayeeAccount.iclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();

                        //Ashish
                        ldrDataRows = busGlobalFunctions.FilterTable(idtbPayeeAccountPaymentItemType, busConstant.DataType.Numeric, "payee_account_id", lobjPayeeAccount.icdoPayeeAccount.payee_account_id);
                        foreach (DataRow ldrPaymentitems in ldrDataRows)
                        {
                            busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                            lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.LoadData(ldrPaymentitems);
                            lobjPayeeAccount.iclbPayeeAccountPaymentItemType.Add(lobjPayeeAccountPaymentItemType);
                        }

                        //Load the tax withholdinghistory for the payee account
                        lobjPayeeAccount.iclbTaxWithholingHistory = new Collection<busPayeeAccountTaxWithholding>();

                        ldrDataRows = busGlobalFunctions.FilterTable(idtbTaxwithHoldingDetails, busConstant.DataType.Numeric, "Payee_account_id", lobjPayeeAccount.icdoPayeeAccount.payee_account_id);

                        foreach (DataRow ldrTaxwithHolding in ldrDataRows)
                        {
                            busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                            lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.LoadData(ldrTaxwithHolding);
                            if (busGlobalFunctions.CheckDateOverlapping(idtNextPaymentDate, lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.start_date, lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date))
                                lobjPayeeAccount.iclbTaxWithholingHistory.Add(lobjPayeeAccountTaxWithholding);

                        }

                        //validate whether payee account has taxwithholding details or not
                        if (ldrDataRows.Count() == 0)
                        {
                            if (lobjPayeeAccount.ibusApplication == null)
                                lobjPayeeAccount.LoadApplication();
                            if (!lobjPayeeAccount.ibusApplication.IsBenefitOptionTransfers())
                                lobjPayeeAccount.iblnNoTaxWithholdingRecordIndicator = true;
                        }

                        if (lobjPayeeAccount.ibusPayeeAccountActiveStatus != null)
                        {
                            //validate whether payee account has valid ach details or not
                            if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.ValidateWhenStatusApprovedOrRecievedForAch())
                                lobjPayeeAccount.iblnInvalidACHIndicator = true;

                            if (lobjPayeeAccount.IsRHICGreaterThan3rdPary())
                                lobjPayeeAccount.iblnIsRHICGreaterThan3rdParty = true;

                            //validate whether payee account has valid rollover details or not
                            if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.ValidateWhenStatusApprovedOrRecievedForRollover())
                                lobjPayeeAccount.iblnInvalidRolloverIndicator = true;

                            if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsAddressNotExistForPayee() && lobjPayeeAccount.iclbActiveACHDetails.Count == 0)
                                lobjPayeeAccount.iblnPayeeAddressNotExistsIndicator = true;
                        }

                        //Calculate Benefit Amount
                        if (lobjPayeeAccount.idecBenefitAmount == 0.00m)
                            lobjPayeeAccount.LoadBenefitAmount();
                        if (lobjPayeeAccount.idecBenefitAmount < 0.0m)
                            lobjPayeeAccount.iblnInvalidNetAmountIndicator = true;

                        //validate whether payee is deseased or not
                        if (lobjPayeeAccount.ibusPayee.icdoPerson.date_of_death != DateTime.MinValue)
                            lobjPayeeAccount.iblnDeathNotificationIndicator = true;

                        //PIR 24561 validate whether approved and auto refund, and gross amount > $1000 and change status to review
                        lobjPayeeAccount.LoadGrossAmount();
                        if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundApproved &&
                           lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionAutoRefund &&
                           lobjPayeeAccount.idecGrossAmount > 1000.0M)
                            lobjPayeeAccount.iblnIsApprovedAutoRefundAndGrossAmountCheck = true;

                        //Validate the payee account and change the status
                        lobjPayeeAccount.ValidateSoftErrors();
                        //PIR 25824 SKip changing status to review if address validation occurs and set pull check flag to Yes for the payee account.
                        if (lobjPayeeAccount.ibusSoftErrors != null &&
                                lobjPayeeAccount.ibusSoftErrors.iclbError != null && 
                                lobjPayeeAccount.ibusSoftErrors.iclbError.Count == 1 && lobjPayeeAccount.iblnPayeeAddressNotExistsIndicator == true)
                        {
                            lobjPayeeAccount.icdoPayeeAccount.pull_check_flag = busConstant.Flag_Yes;
                            lobjPayeeAccount.icdoPayeeAccount.Update();
                        }
                        else
                        {
                            lobjPayeeAccount.UpdateValidateStatus();
                            if (lobjPayeeAccount.ibusSoftErrors != null &&
                                lobjPayeeAccount.ibusSoftErrors.iclbError != null &&
                                lobjPayeeAccount.ibusSoftErrors.iclbError.Count > 0)
                            {
                                //lobjPayeeAccount.CreateReviewPayeeAccountStatus();
                                idlgUpdateProcessLog("Error in Payee Account ID " + ' ' + lobjPayeeAccount.icdoPayeeAccount.payee_account_id.ToString(), "INFO", istrProcessName);
                            }
                        }
                    }
                }
               
                
                //Change the payee account status to review after validating all payee accounts
                DBFunction.DBNonQuery("cdoPayeeAccountStatus.PayeeErrorReportCreareReviewPayeeAccountStatus",
                                new object[3] { iobjSystemManagement.icdoSystemManagement.batch_date, idtNextPaymentDate ,iobjBatchSchedule.batch_schedule_id}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
           }           
        }
    }
