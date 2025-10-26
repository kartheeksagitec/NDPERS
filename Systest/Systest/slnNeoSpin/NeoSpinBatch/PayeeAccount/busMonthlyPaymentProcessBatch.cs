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
using System.IO;
using System.Text;
using Sagitec.ExceptionPub;
using NeoSpin.DataObjects;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Office.Interop.Word;
#endregion

namespace NeoSpinBatch
{
    public class busMonthlyPaymentProcessBatch : busNeoSpinBatch
    {
        //Datatable to contain all payees for the monthly payroll process, used for correspondence
        public DataTable idtPayeeDetails { get; set; }
        //Property to generate correspondence for payees coming in New Payee detail report
        public DataTable idtNewPayees { get; set; }
        //Property to contain minimum guarantee amount for New Payees
        public decimal idecMinimumGuaranteeNewPayees { get; set; }
        //Property to contain minimum guarantee amount for Cancelled or Payments Complete Payees
        public decimal idecMinimumGuaranteeCancelledorPaymentsCompletePayees { get; set; }
        List<string> llstGeneratedCorrespondence = new List<string>();
        List<string> llstGeneratedReports = new List<string>();
        List<string> llstGeneratedFiles = new List<string>();
        busPaymentProcess lobjPaymentProcess = new busPaymentProcess();
        string lstrReportPrefixPaymentScheduleID = string.Empty;
        busBase lobjBase = new busBase();
        busNeoSpinBase lbusBase = new busNeoSpinBase();
        int lintNoOfChecksNeeded = 0;
        //Load Next Benefit Payment Date        
        public DateTime idtNextBenefitPaymentDate { get; set; }

      

        //Datatable to contain all Payment details processed for the schedule, used for correspondence
        public DataTable idtPaymentDetails { get; set; }
       
        //Datatable to contain all non taxable ending Payment details processed for the schedule, used for correspondence
        public DataTable idtNontaxable { get; set; }
        //Load All the payee accounts to be considered for the next payment date
        //Datatable to contain All the payee accounts to be considered for the next payment date, used for correspondence
        public DataTable idtPayeeAccount { get; set; }

        public Collection<busPayeeAccount> iclbBenefitPaymentChangePayeeAccounts { get; set; }


        public Collection<busPayeeAccount> iclbPayeeAccounts { get; set; }
        public void LoadPayeesForPaymentProcess(DateTime adtPaymentScheduleDate)
        {
            busBase lobjBase = new busBase();
            idtPayeeAccount = busNeoSpinBase.Select("cdoPaymentSchedule.LoadPayeeAccountsForPaymentProcess", new object[1] { adtPaymentScheduleDate });
            iclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(idtPayeeAccount, "icdoPayeeAccount");
        }
        public void LoadNextBenefitPaymentDate()
        {
            idtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
        }
        //Load the Next valid monthly payment schedule into the property ibusPaymentSchedule
        public busPaymentSchedule ibusPaymentSchedule { get; set; }
        public Collection<busPaymentScheduleStep> iclbProcessedSteps { get; set; }
        public void LoadPaymentSchedule()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNextBenefitPaymentDate();
            if (ibusPaymentSchedule == null) ibusPaymentSchedule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
            DataTable ldtbPaymentSchedule = busNeoSpinBase.Select<cdoPaymentSchedule>(new string[4] { "schedule_type_value", "status_value", "action_status_value", "payment_date" },
                                                   new object[4] {busConstant.PaymentScheduleScheduleTypeMonthly,busConstant.PaymentScheduleStatusValid,
                                                   busConstant.PaymentScheduleActionStatusReadyforFinal,idtNextBenefitPaymentDate}, null, null);
            if (ldtbPaymentSchedule.Rows.Count == 1)
            {
                ibusPaymentSchedule.icdoPaymentSchedule.LoadData(ldtbPaymentSchedule.Rows[0]);
            }
        }

        public busPaymentSchedule ibusLastPaymentScheule { get; set; }
        public bool LoadPaymentScheduleByPaymentDate(DateTime adtPaymentDate)
        {
            bool lblnResult = false;
            ibusLastPaymentScheule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
            DataTable ldtPaymentSchedule = busNeoSpinBase.Select<cdoPaymentSchedule>
                (new string[2] { "payment_date", "schedule_type_value" }, new object[2] { adtPaymentDate, busConstant.PaymentScheduleScheduleTypeMonthly },
                null, null);
            if (ldtPaymentSchedule.Rows.Count > 0)
            {
                lblnResult = true;
                ibusLastPaymentScheule.icdoPaymentSchedule.LoadData(ldtPaymentSchedule.Rows[0]);
            }
            return lblnResult;
        }

        //Method to be called from the batch
        public void ProcessPayments()
        {
            istrProcessName = "Monthly Payment Batch";
            bool lblnStepFailedIndicator = false;
            //Load the payment schedule
            if (ibusPaymentSchedule == null)
                LoadPaymentSchedule();
            if (ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id > 0)
            {
                lstrReportPrefixPaymentScheduleID = ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id + "_";
                //update the process start time
                ibusPaymentSchedule.icdoPaymentSchedule.process_date = DateTime.Today;
                ibusPaymentSchedule.icdoPaymentSchedule.process_start_time = DateTime.Now;
                //intialize proccessed steps collection
                iclbProcessedSteps = new Collection<busPaymentScheduleStep>();
                //Load Payment Batch schedule steps
                if (ibusPaymentSchedule != null)
                    if (lobjPaymentProcess.iclbBatchScheduleSteps == null)
                        lobjPaymentProcess.LoadBatchScheduleSteps(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                try
                {
                    iobjPassInfo.BeginTransaction();
                    idtPayeeDetails = busNeoSpinBase.Select("cdoPayeeAccount.LoadPayeeAcountAndStatus",
                        new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date });
                    idtNewPayees = new DataTable();                        
                    //Loop through the step and process payments
                    if (lobjPaymentProcess.iclbBatchScheduleSteps != null)
                    {
                        foreach (busPaymentScheduleStep lobjPaymentScheduleStep in lobjPaymentProcess.iclbBatchScheduleSteps)
                        {
                            if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 100 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 200)
                            {
                                switch (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence)
                                {
                                    // Load All the payee accounts to be considered for this payment process
                                    case 100:
                                        idlgUpdateProcessLog("Start Payment Process ", "INFO", istrProcessName);
                                        if (iclbPayeeAccounts == null)
                                            LoadPayeesForPaymentProcess(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                                        lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                        iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                                        break;
                                }
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 200 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 300)
                            {
                                //Execute all the trial reports
                                if (ExecuteTrialReports(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 300 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 400)
                            {
                                //Back up data prior to batch
                                if (BackUpData(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 400 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 500)
                            {
                                // Call the methods which are related to creating payment history
                                if (CreatePaymentHistory(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 500 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 600)
                            {
                                // Create GL
                                if (CreatingGL(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 600 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 700)
                            {
                                // Create Check File
                                if (CreateCheckFile(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 700 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 800)
                            {
                                // Create ACH File
                                if (CreateACHFile(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 800 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 900)
                            {
                                // Call the methods which are related to creating premium payment for IBS
                                if (CreatePremiumPaymentsForIBS(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 900 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1000)
                            {
                                // Call the methods which are related to updating person account information
                                if (UpdatePersonAccountInfo(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            //Single step to call all final reports
                            else if ((lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1000 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1100) ||
                                (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1400 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1600))
                            {
                                // Call the methods to create ‘Final Total Per Items Report’ from Payment History
                                if (ExecuteFinalReports(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1200 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1300)
                            {
                                // Call the methods which are related to creating Payment register report
                                if (ExecuteFinalReports(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1300 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1400)
                            {
                                // Call the methods which are related to creating vendor payment  summary  
                                if (CreateVendorPayment(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1600 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence <= 1700)
                            {
                                //Call the methods which are related to updating payee account related tables
                                if (PreparePayeeAccountForNextPayPeriod(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                    lobjPaymentScheduleStep.iblnStepFailedIndicator = true;
                                    lblnStepFailedIndicator = true;
                                    break;
                                }
                                else
                                {
                                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                }
                                iclbProcessedSteps.Add(lobjPaymentScheduleStep);
                            }
                        }
                    }
                    if (lblnStepFailedIndicator)
                    {
                        idlgUpdateProcessLog("Batch Process Failed", "INFO", istrProcessName);
                        LoadGeneratedFileInfo();
                        iobjPassInfo.Rollback();
                        DeleteGeneratedReports();
                        DeleteGeneratedFiles();
                        llstGeneratedFiles.Clear();
                        llstGeneratedReports.Clear();
                        llstGeneratedCorrespondence.Clear();
                    }
                    else
                    {
                        //To execute reports which doesnt have separate step number
                        ExecuteFinalReports(-1);
                        iobjPassInfo.Commit();
                        llstGeneratedFiles.Clear();
                        llstGeneratedReports.Clear();                        
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    iobjPassInfo.Rollback();
                    LoadGeneratedFileInfo();
                    iobjPassInfo.Rollback();
                    DeleteGeneratedReports();
                    DeleteGeneratedFiles();
                    llstGeneratedFiles.Clear();
                    llstGeneratedReports.Clear();
                    idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "INFO", istrProcessName);
                }
                try
                {
                    iobjPassInfo.BeginTransaction();
                    //Update Payment shedule status                    
                    foreach (busPaymentScheduleStep lobjScheduleStep in lobjPaymentProcess.iclbBatchScheduleSteps)
                    {
                        lobjScheduleStep.icdoPaymentScheduleStep.batch_schedule_id = busConstant.MonthlyPaymentBatchScheduleID;
                        lobjScheduleStep.icdoPaymentScheduleStep.Update();
                    }
                    if (lblnStepFailedIndicator)
                    {
                        ibusPaymentSchedule.icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusFailed;
                    }
                    else
                    {
                        ibusPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleStatusProcessed;
                        ibusPaymentSchedule.icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusProcessed;
                        //to delete back up tables
                        DeleteBackUpTables();
                    }
                    //Wait for one min
                    //System.Threading.Thread.Sleep(1000 * 60);
                    ibusPaymentSchedule.icdoPaymentSchedule.process_end_time = DateTime.Now;
                    ibusPaymentSchedule.icdoPaymentSchedule.Update();
                    iobjPassInfo.Commit();
                    if (lblnStepFailedIndicator)
                    {
                        idlgUpdateProcessLog("Monthly Payment Process Ended with some Failed Step(s)", "INFO", istrProcessName);
                    }
                    else
                    {
                        //Generate Correspondence
                        idlgUpdateProcessLog("Generate Correspondence ", "INFO", istrProcessName);
                        try
                        {
                            //NOTE : Since outside commit, if any correspondence is using idtNextBenefitPaymentDate, need to test whether appropriate date is coming up
                            GenerateCorrespondence();
                            idlgUpdateProcessLog("Correspondence Generated Successfully", "INFO", istrProcessName);
                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("Correspondence Generation Failed.", "INFO", istrProcessName);
                            DeleteGeneratedCorrespondence();
                            throw e;
                        }
                        llstGeneratedCorrespondence.Clear();
                        idlgUpdateProcessLog("Monthly Payment Process Successful", "INFO", istrProcessName);
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    iobjPassInfo.Rollback();
                    idlgUpdateProcessLog("Error Occured in Updating Schedule Status" + e.Message, "INFO", istrProcessName);
                }
            }
        }

        private void DeleteBackUpTables()
        {
            try
            {
                int lintrtn = 0;
                idlgUpdateProcessLog("Delete back-up data for previous payroll", "INFO", istrProcessName);
                lintrtn = DBFunction.DBNonQuery("cdoPayeeAccount.DeleteBackUpMonthly",
                          new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //idlgUpdateProcessLog((lintrtn < 0) ? "Delete back-up data failed." + lintrtn.ToString() : "Delete back-up data - Successful." + lintrtn.ToString(), "INFO", istrProcessName);
                idlgUpdateProcessLog("Delete back-up data for previous payroll - Successful.", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Delete back-up data Failed.", "INFO", istrProcessName);               
            }
        }

        private void LoadBenefitPaymentChangeCorrespondenceData()
        {
            try
            {
                #region commented code
                /*idlgUpdateProcessLog("Load Retiree Change Notice Data ", "INFO", istrProcessName);               
                DataTable ldtdBenefitPaymentChange = busNeoSpinBase.Select("cdoPayeeAccount.LoadBenefitPaymentChangeCorData",
                                                    new object[0] {});
                DataTable ldtdBenefitPaymentChangeBene = busNeoSpinBase.Select("cdoPayeeAccount.LoadBenefitPaymentChangeBeneficiaryData",
                                                   new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date });
                DataTable ldtdBenefitPaymentChangeHistoryHeader = busNeoSpinBase.Select("cdoPayeeAccount.LoadBenefitPaymentChangePaymentHeaders",
                                                   new object[0] {});
                DataTable ldtdBenefitPaymentChangeHistoryDetails = busNeoSpinBase.Select("cdoPayeeAccount.LoadBenefitPaymentChangePaymentDetails",
                                                   new object[0] {});
                DataTable ldtdBenefitPaymentChangePapit = busNeoSpinBase.Select("cdoPayeeAccount.LoadBenefitPaymentChanngePapitDetails",
                                                   new object[0] {});
                foreach (DataRow ldtrPayee in ldtdBenefitPaymentChange.Rows)
                {
                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.icdoPayeeAccount.LoadData(ldtrPayee);

                    //Load Payment history headers for the payee account
                    DataTable ldtbHistoryHeaderByPayeeAccount = ldtdBenefitPaymentChangeHistoryHeader.AsEnumerable().Where(o =>
                        o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id).AsDataTable();
                    Collection<busPaymentHistoryHeader> lclbPaymentHistoyHeader = lobjBase.GetCollection<busPaymentHistoryHeader>(ldtbHistoryHeaderByPayeeAccount, "icdoPaymentHistoryHeader");
                    lobjPayeeAccount.iclbPaymentHistoryHeader = lclbPaymentHistoyHeader;

                    //Load Payment history details for the payee account
                    DataTable ldtlHistoryDetailByPayeeAccount = ldtdBenefitPaymentChangeHistoryDetails.AsEnumerable().Where(o =>
                       o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id).AsDataTable();
                    Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();

                    foreach (DataRow ldtrPaymentDetail in ldtlHistoryDetailByPayeeAccount.Rows)
                    {
                        busPaymentHistoryDetail lobjPaymentHistoryDetail = new busPaymentHistoryDetail { icdoPaymentHistoryDetail = new cdoPaymentHistoryDetail() };
                        lobjPaymentHistoryDetail.ibusPaymentItemType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                        lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.LoadData(ldtrPaymentDetail);
                        lobjPaymentHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.LoadData(ldtrPaymentDetail);
                        lclbPaymentHistoryDetail.Add(lobjPaymentHistoryDetail);
                    }
                    lobjPayeeAccount.iclbPaymentHistoryDetail = lclbPaymentHistoryDetail;


                    foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in lclbPaymentHistoyHeader)
                    {
                        List<busPaymentHistoryDetail> llistPaymentFistoryDetail = lclbPaymentHistoryDetail.Where(o =>
                            o.icdoPaymentHistoryDetail.payment_history_header_id == lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id).ToList();

                        lobjPaymentHistoryHeader.iclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
                        foreach (busPaymentHistoryDetail lobjDetail in llistPaymentFistoryDetail)
                        {
                            lobjPaymentHistoryHeader.iclbPaymentHistoryDetail.Add(lobjDetail);
                        }
                    }
                    //Load Papit for the payee account
                    DataTable ldtdPapitByPayeeAccount = ldtdBenefitPaymentChangePapit.AsEnumerable().Where(o =>
                       o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id).AsDataTable();
                    Collection<busPayeeAccountPaymentItemType> lclbPapit = new Collection<busPayeeAccountPaymentItemType>();

                    foreach (DataRow ldtrPapit in ldtdPapitByPayeeAccount.Rows)
                    {
                        busPayeeAccountPaymentItemType lobjPapit = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                        lobjPapit.ibusPaymentItemType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                        lobjPapit.icdoPayeeAccountPaymentItemType.LoadData(ldtrPapit);
                        lobjPapit.ibusPaymentItemType.icdoPaymentItemType.LoadData(ldtrPapit);
                        lclbPapit.Add(lobjPapit);
                    }
                    lobjPayeeAccount.iclbPayeeAccountPaymentItemType = lclbPapit;
                    lobjPayeeAccount.idtNextBenefitPaymentDate = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;

                    DataRow ldtrBene = ldtdBenefitPaymentChangeBene.AsEnumerable().Where(o =>
                       o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id).FirstOrDefault();
                    if (ldtrBene != null)
                    {
                        lobjPayeeAccount.istrPrimaryBeneficiaryName = ldtrBene["istrPrimaryBeneficiaryName"].ToString();
                        lobjPayeeAccount.istrContingentBeneficiaryName = ldtrBene["istrContingentBeneficiaryName"].ToString();
                    }
                    lobjPayeeAccount.LoadNetAmountYTD();
                    lobjPayeeAccount.LoadNontaxableBeginningBalnce();
                    lobjPayeeAccount.LoadNontaxableAmountYTD();
                    lobjPayeeAccount.LoadTaxableAmountYTD();
                    lobjPayeeAccount.LoadTaxAmounts();
                    lobjPayeeAccount.LoadOtherDeductionAmount();
                    lobjPayeeAccount.LoadInsurancePremiumAmount();
                    lobjPayeeAccount.LoadOldTaxAmounts();
                    lobjPayeeAccount.LoadOldOtherDeductionAmount();
                    lobjPayeeAccount.LoadOldInsurancePremiumAmount();
                    lobjPayeeAccount.LoadYTDTaxAmounts();
                    lobjPayeeAccount.LoadYTDOtherDeductionAmount();
                    lobjPayeeAccount.LoadYTDInsurancePremiumAmount();
                    lobjPayeeAccount.ibusPayee.icdoPerson.LoadData(ldtrPayee);
                    iclbBenefitPaymentChangePayeeAccounts.Add(lobjPayeeAccount);
                }*/
                #endregion
                /*PIR 17763- for retiree change data reports system used to look at temp table called TEMP_BEN_PAY_CHANGE_PAYEE which gets created by query TrialBenefitPaymentChangeMainReport 
				now these reports no longer use that table , now looks at TEMP_CURRENT_PAYEE_ACCOUNTS table.
				Changed sequence of filling datatables because now we are filling TEMP_CURRENT_PAYEE_ACCOUNTS table in LoadRetireeChangeData query
                which we use later in LoadBenefitPaymentChangeCorData and LoadBenefitPaymentChangeBeneficiaryData query to fill ldtdBenefitPaymentChange 
                and ldtdBenefitPaymentChangeBene datatables respectively. */
                DataTable ldtRetireeChangeData = busNeoSpinBase.Select("cdoPayeeAccount.LoadRetireeChangeData",
                                                   new object[2] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                DataTable ldtdBenefitPaymentChange = busNeoSpinBase.Select("cdoPayeeAccount.LoadBenefitPaymentChangeCorData",
                                                   new object[0] { });
                DataTable ldtdBenefitPaymentChangeBene = busNeoSpinBase.Select("cdoPayeeAccount.LoadBenefitPaymentChangeBeneficiaryData",
                                                   new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date });

                //PIR-18827
                string lstrRetireeChangeNoticeMsg = NeoSpin.Common.ApplicationSettings.Instance.RetireeChangeNotice;
                string lstrGeneratePath = iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
                string lstrDestinationPath = iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenWSS");
				string lstrPdfFile = string.Empty;
                foreach (DataRow ldtrPayee in ldtdBenefitPaymentChange.Rows)
                {
                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.icdoPayeeAccount.LoadData(ldtrPayee);
                    lobjPayeeAccount.ibusPayee.icdoPerson.LoadData(ldtrPayee);
                    
                    //commented since non taxable amount is included in the main query
                    //lobjPayeeAccount.LoadNontaxableBeginningBalnce();

                    IEnumerable<DataRow> ldtrBene = ldtdBenefitPaymentChangeBene.AsEnumerable().Where(o =>
                       o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id);
                    lobjPayeeAccount.iclbPersonAccountBene = new Collection<busPersonAccountBeneficiary>();
                    bool lblnBeneAval = false;
                    foreach (DataRow ldr in ldtrBene)
                    {
                        busPersonAccountBeneficiary lobjPAB = new busPersonAccountBeneficiary { icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary() };
                        lobjPAB.icdoPersonAccountBeneficiary.LoadData(ldr);
                        lobjPayeeAccount.iclbPersonAccountBene.Add(lobjPAB);
                        lblnBeneAval = true;
                    }
                    if (lobjPayeeAccount.ibusPayee.icdoPerson.beneficiary_required_flag == busConstant.Flag_Yes &&
                        lblnBeneAval)
                    {
                        lobjPayeeAccount.istrIsBeneficiaryBlockVisible = busConstant.Flag_Yes;
                    }
                    else
                    {
                        lobjPayeeAccount.istrIsBeneficiaryBlockVisible = busConstant.Flag_No;
                    }
                    //if (ldtrBene != null)
                    //{
                    //    lobjPayeeAccount.istrPrimaryBeneficiaryName = ldtrBene["istrPrimaryBeneficiaryName"] == DBNull.Value ?
                    //        "None" : ldtrBene["istrPrimaryBeneficiaryName"].ToString();
                    //    lobjPayeeAccount.istrContingentBeneficiaryName = ldtrBene["istrContingentBeneficiaryName"] == DBNull.Value ?
                    //        string.Empty : ldtrBene["istrContingentBeneficiaryName"].ToString();
                    //}
                    //else
                    //{
                    //    lobjPayeeAccount.istrPrimaryBeneficiaryName = "None";
                    //    lobjPayeeAccount.istrContingentBeneficiaryName = string.Empty;
                    //}
                    DataTable ldtFilteredRetireeChange = ldtRetireeChangeData.AsEnumerable()
                                                                             .Where(o => o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id)
                                                                             .AsDataTable();
                    if (ldtFilteredRetireeChange.Rows.Count > 0)
                    {                        
                        lobjPayeeAccount.idecOldDentalInsurancePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_dental"]);
                        lobjPayeeAccount.idecDentalInsurancePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_dental"]);
                        lobjPayeeAccount.idecYTDDentalInsurancePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_dental"]);
                        lobjPayeeAccount.idecOldFedTaxAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_fed_tax"]);
                        lobjPayeeAccount.idecFedTaxAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_fed_tax"]);
                        lobjPayeeAccount.idecYTDFedTaxAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_fed_tax"]);
                        lobjPayeeAccount.idecOldHealthInsurancePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_health"]);
                        lobjPayeeAccount.idecHealthInsurancePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_health"]);
                        lobjPayeeAccount.idecYTDHealthInsurancePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_health"]);
                        lobjPayeeAccount.idecOldLifePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_life"]);
                        lobjPayeeAccount.idecLifePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_life"]);
                        lobjPayeeAccount.idecYTDLifePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_life"]);
                        lobjPayeeAccount.idecOldMiscellaneousDueAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_misc"]);
                        lobjPayeeAccount.idecMiscellaneousDueAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_misc"]);
                        lobjPayeeAccount.idecYTDMiscellaneousDueAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_misc"]);
                        lobjPayeeAccount.idecOldNDPEADuesAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_ndpea_dues"]);
                        lobjPayeeAccount.idecNDPEADuesAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_ndpea_dues"]);
                        lobjPayeeAccount.idecYTDNDPEADuesAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_ndpea_dues"]);
                        lobjPayeeAccount.idecOldStateTaxAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_state_tax"]);
                        lobjPayeeAccount.idecStateTaxAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_state_tax"]);
                        lobjPayeeAccount.idecYTDStateTaxAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_state_tax"]);
                        lobjPayeeAccount.idecOldNetAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_net_amount"]);
                        lobjPayeeAccount.idecBenefitAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_net_amount"]);
                        lobjPayeeAccount.idecNetAmountYTD = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_net_amount"]);
                        lobjPayeeAccount.idecNontaxableAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_nontaxable_amount"]);
                        lobjPayeeAccount.idecNontaxableYTD = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_nontaxable_amount"]);
                        lobjPayeeAccount.idecOldNonTaxable = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_nontaxable_amount"]);
                        lobjPayeeAccount.idecOldTaxable = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_taxable_amount"]);
                        lobjPayeeAccount.idecTotalTaxableAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_taxable_amount"]);
                        lobjPayeeAccount.idecTaxableYTD = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_taxable_amount"]);
                        lobjPayeeAccount.idecOldVisionInsurancePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["old_vision"]);
                        lobjPayeeAccount.idecVisionInsurancePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["new_vision"]);
                        lobjPayeeAccount.idecYTDVisionInsurancePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ytd_vision"]);
                        lobjPayeeAccount.idecTotalLTDReceived = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["ltd_nontaxable_amount"]);
                        lobjPayeeAccount.idecNontaxableBeginningBalnce = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["starting_nontaxable_amount"]);
                        //Since correspondence is outside transaction, next benefit payment date will be next month
                        //so explicitly setting the value of next benefit payment date to current payment date
                        lobjPayeeAccount.idtNextBenefitPaymentDate = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                        lobjPayeeAccount.istrChangeDate = ibusPaymentSchedule.icdoPaymentSchedule.payment_date.ToString(busConstant.DateFormatLongDate).ToUpper();
                        //PIR 15347 - Medicare Part D bookmarks
                        lobjPayeeAccount.idecOldMedicarePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["OLD_MEDICARE"]);
                        lobjPayeeAccount.idecMedicarePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["NEW_MEDICARE"]);
                        lobjPayeeAccount.idecYTDMedicarePremiumAmount = Convert.ToDecimal(ldtFilteredRetireeChange.Rows[0]["YTD_MEDICARE"]);
                    }
                    #region PIR-18827                    
                    string lstrFileName = CreateCorrespondence(lobjPayeeAccount, "PAY-4261");
                    llstGeneratedCorrespondence.Add(lstrFileName);
                    if (File.Exists(lstrGeneratePath + lstrFileName))
                    {
                        WordApp?.Documents.Open(lstrGeneratePath +lstrFileName);
                        lstrPdfFile = lstrFileName.Replace(".docx", "_Retiree_Change_Notice.pdf");
                        WordApp?.ActiveDocument.SaveAs(lstrDestinationPath + lstrPdfFile, 17);
                        WordApp?.ActiveDocument.Close(WdSaveOptions.wdDoNotSaveChanges);
						busWSSHelper.PublishMSSMessage(0, 0, lstrRetireeChangeNoticeMsg, busConstant.WSS_MessageBoard_Priority_High,
                                                                     lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, 0, null, null, lstrDestinationPath + lstrPdfFile);
                    }
                    #endregion PIR-18827
                    //iclbBenefitPaymentChangePayeeAccounts.Add(lobjPayeeAccount);
                    lobjPayeeAccount = null;
                }
            }
            catch (Exception e)
            {
                idlgUpdateProcessLog("Load Retiree Change Notice Data failed", "INFO", istrProcessName);
                ExceptionManager.Publish(e);
            }
        }
        // when the batch fails,delete all the reports which are all generated from the batch
        private void DeleteGeneratedReports()
        {
            idlgUpdateProcessLog("Deleting Generated Reports", "INFO", istrProcessName);
            foreach (string lstrReportPath in llstGeneratedReports)
            {
                if (File.Exists(lstrReportPath))
                {
                    File.Delete(lstrReportPath);
                }
            }
            idlgUpdateProcessLog("Generated Reports are deleted", "INFO", istrProcessName);
        }
        // when the batch fails,delete all the reports which are all generated from the batch
        private void DeleteGeneratedCorrespondence()
        {
            idlgUpdateProcessLog("Deleting Generated Correspondence", "INFO", istrProcessName);
            foreach (string lstrCorPath in llstGeneratedCorrespondence)
            {
                if (File.Exists(lstrCorPath))
                {
                    File.Delete(lstrCorPath);
                }
            }
            idlgUpdateProcessLog("Generated Correspondence are deleted", "INFO", istrProcessName);
        }
        // when the batch fails,delete all the reports which are all generated from the batch
        private void DeleteGeneratedFiles()
        {
            idlgUpdateProcessLog("Deleting Generated Files", "INFO", istrProcessName);
            string lstrACHFileOutPath = iobjPassInfo.isrvDBCache.GetPathInfo("PHACHOut");
            string lstrCheckFileOutPath = iobjPassInfo.isrvDBCache.GetPathInfo("PHChkOut");

            string lstrACHFileProcessedPath = iobjPassInfo.isrvDBCache.GetPathInfo("PHACHProc");
            string lstrCheckFileProcessedPath = iobjPassInfo.isrvDBCache.GetPathInfo("PHChkProc");
            if (iclbFileHdr != null)
            {
                foreach (busFileHdr lobjFileHdr in iclbFileHdr)
                {
                    if (lobjFileHdr.icdoFileHdr.file_id == busConstant.ACHFileID)
                    {
                        string lstrOutFileName = lstrACHFileOutPath + lobjFileHdr.icdoFileHdr.mailbox_file_name;
                        llstGeneratedFiles.Add(lstrOutFileName);
                        string lstrProcessedFileName = lstrACHFileProcessedPath + lobjFileHdr.icdoFileHdr.processed_file_name;
                        llstGeneratedFiles.Add(lstrProcessedFileName);
                    }
                    else if (lobjFileHdr.icdoFileHdr.file_id == busConstant.CheckFileID)
                    {
                        string lstrOutFileName = lstrCheckFileOutPath + lobjFileHdr.icdoFileHdr.mailbox_file_name;
                        llstGeneratedFiles.Add(lstrOutFileName);
                        string lstrProcessedFileName = lstrCheckFileProcessedPath + lobjFileHdr.icdoFileHdr.processed_file_name;
                        llstGeneratedFiles.Add(lstrProcessedFileName);
                    }
                }
                foreach (string lstrFile in llstGeneratedFiles)
                {
                    if (File.Exists(lstrFile))
                    {
                        File.Delete(lstrFile);
                    }
                }
            }
            idlgUpdateProcessLog("Generated Files are deleted", "INFO", istrProcessName);
        }
        public Collection<busFileHdr> iclbFileHdr { get; set; }
        public void LoadGeneratedFileInfo()
        {
            DataTable ldtbFiles = busBase.Select("cdoFileHdr.LoadACHAndCheckFileHdrInfo",
                            new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.process_start_time });
            iclbFileHdr = lobjBase.GetCollection<busFileHdr>(ldtbFiles, "icdoFileHdr");
        }
        //Create Payment History details 
        public int CreatePaymentHistory(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 400:
                    try
                    {
                        //Create Payment History Header for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating Payment History Headers", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreatePaymentHistoryHeader(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        lintNoOfChecksNeeded = lintrtn;
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Payment History Header Failed." : "Payment History Headers Created. ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment History Header Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Create Payment History Header for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Validate available Checks", "INFO", istrProcessName);
                        //PIR 938
                        bool lblnChecksAvailable = lobjPaymentProcess.GetAvailableChecks(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (!lblnChecksAvailable)
                        {
                            idlgUpdateProcessLog("The Check Book has reached the Maximum Limit/Check book is not available for the payment date.", "INFO", istrProcessName);
                            return -1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment History Header Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    //Create Payment History details for all the payee accounts considered for this payment process
                    try
                    {
                        idlgUpdateProcessLog("Creating Payment History details", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreatePaymentHistoryDetail(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Payment History Details Failed." : "Payment History Details created. ", "INFO", istrProcessName);

                        // PIR 26652 - Calculate Fed and State tax on Excess Taxable Contribution
                        lobjPaymentProcess.CalculateTaxForExcessTaxableContributionAmountToMember(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment History Details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 410:
                    try
                    {
                        //Create Payment Check History details for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating Check History for Payees", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateCheckHistoryforPayees(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        
                        //PIR 26652 - create payment history distribution for excess contribution
                        //Create Check History for all the persons or oranizations approved for deposit refund
                        idlgUpdateProcessLog("Creating Check History for Deposit Refund", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateChkACHHistoryforExcessContrReturnAdhoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);

                        //Update Deposit Refund To Processed for Adhoc payment process
                        idlgUpdateProcessLog("Update Deposit Refund To Processed ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateDepositRefundStatusToProcessed(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn == -1) ? "Updating Deposit Refund To Processed Failed." : "Updating Deposit Refund To Processed Successful.", "INFO", istrProcessName);

                        //Update FBO CO
                        lobjPaymentProcess.UpdateFBOCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment Check History details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 420:
                    try
                    {
                        //Create ACH Check History for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating ACH History for Payees", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateACHHistoryforPayees(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);

                        //Update FBO CO
                        lobjPaymentProcess.UpdateFBOCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);


                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of ACH Check History details Failed." : "ACH Payment Check History details Created. ", "INFO", istrProcessName);

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of ACH Check History details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 430:
                    try
                    {
                        //Create Rollover Check History for all the payee accounts considered for this payment process
                        idlgUpdateProcessLog("Creating Check History Rollover", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateRollOverCheckACHHistoryforPayees(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);

                        //Update FBO CO
                        lobjPaymentProcess.UpdateFBOCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        lintrtn = lobjPaymentProcess.CreateOutstandingHistoryRecords(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Rollover Check History details Failed." : "Rollover Check History details Created. ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Rollover Check History details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        //Update payments
        //Update payments
        public int PreparePayeeAccountForNextPayPeriod(int aintRunSequence)
        {
            int lintrtn = 0, lintPlanID = 0, lintPersonID = 0, lintOrgID = 0;
            switch (aintRunSequence)
            {
                case 1600:
                    try
                    {
                        idlgUpdateProcessLog("Prepare Payee Account for Next Pay Period ", "INFO", istrProcessName);
                        //Load payment details correspondence
                        idtPaymentDetails = busNeoSpinBase.Select("cdoPaymentHistoryHeader.LoadPayments", 
                                            new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                       
                     
                        //Load Pension Recievavle History
                        DataTable ldtPensionReceivableHistory = busNeoSpinBase.Select("cdoPaymentHistoryDetail.LoadPensionReceivableHistory",
                                                                new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                        foreach (DataRow ldtrPensionReceivable in ldtPensionReceivableHistory.Rows)
                        {
                            busPaymentRecovery lbusPaymentRecovery = new busPaymentRecovery { icdoPaymentRecovery = new cdoPaymentRecovery() };
                            lbusPaymentRecovery.icdoPaymentRecovery.LoadData(ldtrPensionReceivable);
                            lbusPaymentRecovery.LoadLastPostedDate();
                            lbusPaymentRecovery.LoadLTDPrincipleAmountPaid();
                            lbusPaymentRecovery.LoadBenefitOverpaymentHeader();
                            decimal ldecAmortizationInterestAmount = 0.0M;

                            //calcualte amortization interest for the paid monthly pension receivable due
                            if (lbusPaymentRecovery.ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.amortized_interest_flag == busConstant.Flag_Yes)
                            {
                                ldecAmortizationInterestAmount = busInterestCalculationHelper.CalculateAmortizationInterest(
                                    ibusPaymentSchedule.icdoPaymentSchedule.payment_date, lbusPaymentRecovery.idtLastPostedDate,
                                    lbusPaymentRecovery.icdoPaymentRecovery.recovery_amount - lbusPaymentRecovery.idecLTDPrincipleAmountPaid);
                            }
                            if (ldecAmortizationInterestAmount <= lbusPaymentRecovery.icdoPaymentRecovery.gross_reduction_amount)
                            {
                                //Create History for the recovery paid
                                lbusPaymentRecovery.CreateRecoveryHistory(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                                    0, Convert.ToInt32(ldtrPensionReceivable["payment_history_header_id"]),
                                    lbusPaymentRecovery.icdoPaymentRecovery.gross_reduction_amount - ldecAmortizationInterestAmount, ldecAmortizationInterestAmount,
                                    0.00m);
                                //creating item level GL
                                lintPlanID = ldtrPensionReceivable["plan_id"] == DBNull.Value ? 0 : Convert.ToInt32(ldtrPensionReceivable["plan_id"]);
                                lintPersonID = ldtrPensionReceivable["person_id"] == DBNull.Value ? 0 : Convert.ToInt32(ldtrPensionReceivable["person_id"]);
                                lintOrgID = ldtrPensionReceivable["org_id"] == DBNull.Value ? 0 : Convert.ToInt32(ldtrPensionReceivable["org_id"]);
                                if (ldecAmortizationInterestAmount > 0)
                                {
                                    lbusPaymentRecovery.GenerateItemLevelGL(ldecAmortizationInterestAmount, lintPlanID, lintPersonID, lintOrgID);
                                }
                                //Change the recovery status from approved to inprocess at the time first payment
                                if (lbusPaymentRecovery.icdoPaymentRecovery.status_value == busConstant.RecoveryStatusApproved)
                                {
                                    lbusPaymentRecovery.icdoPaymentRecovery.status_value = busConstant.RecoveryStatusInProcess;
                                    lbusPaymentRecovery.icdoPaymentRecovery.Update();
                                }
                            }
                        }
                        idlgUpdateProcessLog("Update Pension Recievables Final Payment", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdatePensionRecievablesfinalPayment(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Update Pension Recievables Final Payment Failed." : "Update Pension Recievables Final Payment Successful . ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Update Pension Recievables Final Payment Failed.", "INFO", istrProcessName);
                        return -1;
                    }

                    break;
                case 1610:
                    //Updating Recovery Payment Corrections
                    //TODO
                    break;
                case 1620:
                    //Closing Recovered Corrections  

                    idlgUpdateProcessLog("Updating Recovery Status to satisfied ", "INFO", istrProcessName);
                    lintrtn = lobjPaymentProcess.UpdateRecoveryToSatified(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                    idlgUpdateProcessLog((lintrtn < 0) ? "Updating Recovery Status to satisfied Failed." : "Updating Recovery Status to satisfied Successful . ", "INFO", istrProcessName);


                    idlgUpdateProcessLog("Update Pension Recievables with end date", "INFO", istrProcessName);
                    lintrtn = lobjPaymentProcess.UpdatePensionRecievables(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                    idlgUpdateProcessLog((lintrtn < 0) ? "Update Pension Recievables with end date Failed." : "Update Pension Recievables with end date Successful . ", "INFO", istrProcessName);


                    idlgUpdateProcessLog("Update Minimum Guarantee History", "INFO", istrProcessName);
                    lintrtn = lobjPaymentProcess.UpdateMinimumGuaranteeHistory(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                    idlgUpdateProcessLog((lintrtn < 0) ? "Update Minimum Guarantee History Failed." : "Update Minimum Guarantee History Successful . ", "INFO", istrProcessName);

                    break;
                case 1630:
                    //Updating Underpayment Corrections
                    //TODO
                    break;
                case 1640:
                    //Updating Item History for Un-recovered Corrections
                    //TODO
                    break;
                case 1650:
                    try
                    {
                        //Updating Non-Taxable Amount
                        idlgUpdateProcessLog("Updating Non-Taxable Amount ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateNonTaxableAmount(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Non-Taxable Amount Failed." : "Updating Non-Taxable Amount Successful . ", "INFO", istrProcessName);
                        //Updating Taxable amount with SSLI Reduction amount
                        idlgUpdateProcessLog("Reducing Taxable amount with SSLI Reduction amount ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateTaxableAmountWithSSLReduction(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Reducing Taxable amount with SSLI Reduction amount Failed" : "Reducing Taxable amount with SSLI Reduction amount Successful . ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating Non-Taxable Amount Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1660:
                    //Updating Application Status to Processed, 
                    try
                    {
                        idlgUpdateProcessLog("Updating Benefit Application Status to Processed ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateBenefitApplication(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Benefit Application Status Failed." : "Updating Benefit Application Status Successful . ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Updating DRO Application Status to Processed ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateBenefitDROApplication(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating DRO Application Status Failed." : "Updating DRO apllication Status Successful . ", "INFO", istrProcessName);

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating Application Status to Process Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1670:
                    //Updating Benefit Calculation Status to Processed, 
                    try
                    {
                        idlgUpdateProcessLog("Updating Benefit Calculation Status to Processed", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateBenefitCalculations(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Benefit Calculation Status Failed." : "Updating Benefit Calculation Status Successful . ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Updating DRO Calculation Status to Processed ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateBenefitDROCalcualtions(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating DRO Calculation Status Failed." : "Updating DRO Calculation Status to Successful . ", "INFO", istrProcessName);

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating Benefit Calculation Status Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1680:
                    try
                    {
                        //Update Payee Account Status
                        idlgUpdateProcessLog("Updating Payment history header id For Regular Adjustment Payment ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateAdjustmentsWithPaymentHistoryRegular(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Payment history header id For Regular Adjustment Payment failed." : "Updating Payment history header id For Regular Adjustment Payment Successful . ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Updating Deduction Refund End  date For Regular Payment ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateDeductionRefundEndDateForRegularPayment(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Deduction Refund End  date For Regular Payment failed." : "Updating Deduction Refund End  date For Regular Payment Successful . ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Updating Rollover Detail status to processed ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateRolloverDetailStatusToProcessed(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Rollover Detail status failed." : "Updating Rollover Detail status Successful . ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Updating payee status to Receiving / Processed", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdatePayeeAccountStatus(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Payee status to Receiving / Processed Failed." : "Updating Payee status to Receiving / Processed - Successful ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Updating Person Beneficiary flag required flag", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateBeneficiaryFlag(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Person Beneficiary flag required flag failed." : "Updating Person Beneficiary flag required flag Successful . ", "INFO", istrProcessName);

                        //Update end date for refund items from the  monthly or adhoc batch - As per Satya discussion
                        idlgUpdateProcessLog("Update refund payee account items wth end date", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateRefundItems(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);

                        //prod pir 6326 : no need to update capital gain amount
                        //lintrtn = lobjPaymentProcess.UpdateCapitalGain(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                        //    ibusPaymentSchedule.icdoPaymentSchedule.payment_date, busConstant.MonthlyPaymentBatchScheduleID);
                        
                        idlgUpdateProcessLog("Updating Benefit End date for Refund Payee Accounts ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateBenefitEndDateFromMonthly(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Benefit End date for Refund Payee Accounts failed." : 
                            "Updating Benefit End date for Refund Payee Accounts Successful . ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Updating Benefit End date for Term Certain Payee Accounts ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateBenefitEndDateFromMonthlyForTermCertain(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Benefit End date for Term Certain Accounts failed." :
                            "Updating Benefit End date for Term Certain Accounts Successful . ", "INFO", istrProcessName);

                        DataTable ldtTCPayees = lobjPaymentProcess.LoadPayeeForMaintainRHICWorkflow(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);

                        idlgUpdateProcessLog("Updating payee status to Payments Complete", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdatePayeeAccountStatustoPaymentComplete(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Payee status to Payments Complete Failed." : "Updating Payee status to Payments Complete - Successful ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Creating Maintain RHIC workflow for Term Certain Payees", "INFO", istrProcessName);
                        foreach (DataRow dr in ldtTCPayees.Rows)
                        {
                            if (dr["payee_perslink_id"] != DBNull.Value)
                                InitiateRHICCombineWorkflow(Convert.ToInt32(dr["payee_perslink_id"]));
                        }
                        idlgUpdateProcessLog("Creating Maintain RHIC workflow for Term Certain Payees completed", "INFO", istrProcessName);
                        
                        //uat pir 925 : graduated benefit option factor addition in benefit calculation
                        idlgUpdateProcessLog("Adding Graduated benefit to Taxable part", "INFO", istrProcessName);
                        lobjPaymentProcess.AddGraduatedBenefit(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog("Adding Graduated benefit to Taxable part Successful", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating payee status to Receiving / Processed Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1690:
                    //Recalculte Tax
                    idlgUpdateProcessLog("Re-calculating Taxes ", "INFO", istrProcessName);
                    try
                    {
                        idtNontaxable = busBase.Select("cdoPayeeAccount.LoadPayeeAccountsForRecalculatingTax",
                                                     new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date });                        
                        busUpdateTaxAmount lobjUpdateTax = new busUpdateTaxAmount();
                        lobjUpdateTax.ReCalculateTax(idtNontaxable, true, true);
                        idlgUpdateProcessLog("Re-calculating Taxes Passed.", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Re-calculating Taxes Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1695:
                    /*//Generate Correspondence
                    idlgUpdateProcessLog("Generate Correspondence ", "INFO", istrProcessName);
                    try
                    {
                        GenerateCorrespondence();
                        idlgUpdateProcessLog("Correspondence Generated Successfully", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Correspondence Generation Failed.", "INFO", istrProcessName);
                        return -1;
                    }*/
                    break;
                case 1700:
                    //Initiate Remit Tax withholding Amounts Workflow
                    idlgUpdateProcessLog("Initiate Remit Tax withholding Amounts Workflow", "INFO", istrProcessName);
                    try
                    {
                        Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                        ldctParams["org_code"] = busGlobalFunctions.GetData1ByCodeValue(busConstant.SystemConstantCodeID,
                            busConstant.SystemConstantFedTaxVendor, iobjSystemManagement.iobjPassInfo);
                        busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Remit_Tax_Withholding_Amounts, 0, 0, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, iobjPassInfo, busConstant.WorkflowProcessSource_Batch, adictInstanceParameters: ldctParams);
                        idlgUpdateProcessLog(" Remit Tax withholding Amounts Workflow is initiated", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Initiate Remit Tax withholding Amounts Workflow process Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        //Premium Payments for IBS
        public int CreatePremiumPaymentsForIBS(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 800:
                    try
                    {
                        idlgUpdateProcessLog("Premium Payments for IBS", "INFO", istrProcessName);

                        lintrtn = lobjPaymentProcess.CreatePremiumPaymentsForIBS(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                                                                            ibusPaymentSchedule.icdoPaymentSchedule.effective_date,iobjBatchSchedule.batch_schedule_id);

                        idlgUpdateProcessLog(lintrtn >= 0 ? "Premium Payments for IBS Created. " : "Creation of Premium Payment Deposit for IBS Failed.", "INFO", istrProcessName);

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Premium Payment Deposit for IBS Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        //Updating Person account Information
        public int UpdatePersonAccountInfo(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 900:
                    //update the plan participation status to withdrawn if the benfit type is 'REFUND' and or Retired if the benefit type is 'Retirement' or
                    //Pre -retirement death
                    try
                    {
                        idlgUpdateProcessLog("Updating Person account Information", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdatePersonAccountStatus(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Person account Information Failed." : "Updating Person account Information Successful .  ", "INFO", istrProcessName);

                        //Reduce the paid amount from the member contributions for the member payee account
                        //not for benefit type disability
                        lintrtn = lobjPaymentProcess.UpdateRetirementContributionForMember(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Person account Information Failed." : "Updating Person account retirement contribution for member  Successful .  ", "INFO", istrProcessName);

                        //Reduce the paid amount from the member contributions for the alternate payee account
                        //not for benefit type disability
                        lintrtn = lobjPaymentProcess.UpdateRetirementContributionForAltPayee(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Person account Information Failed." : "Updating Person account retirement contribution for alternate payee Successful .  ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating Person account Information Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        //Create Vendor Payment Summary
        public int CreateVendorPayment(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 1300:
                    try
                    {
                        //Create Vendor Payment Summary
                        idlgUpdateProcessLog("Create Vendor Payment Summary", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateVendorPayments(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Vendor Payment Summary Failed." : "Vendor Payment Summary Created. ", "INFO", istrProcessName);
                        ExecuteFinalReports(1300);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Create Vendor Payment Summary Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        //Create GL
        public int CreatingGL(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 500:
                    try
                    {
                        //CreatingGL
                        idlgUpdateProcessLog("Create GL", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateGL(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                                                                ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                                                                busConstant.GLSourceTypeValueBenefitPayment, iobjBatchSchedule.batch_schedule_id);
                        lintrtn = lobjPaymentProcess.CreateGL(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                                                                ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                                                                busConstant.GLSourceTypeValueInsuranceTransfer, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of GL Failed." : "GL Created. ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of GL Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        //Create Check File
        public int CreateCheckFile(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 600:
                    try
                    {
                        //Create Check File
                        idlgUpdateProcessLog("Create Check Files", "INFO", istrProcessName);
                        DataTable ldtCheckFile = busBase.Select("cdoPaymentHistoryDistribution.LoadCheckPaymentDistribution", new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                        if (ldtCheckFile.Rows.Count > 0)
                        {
                            CreateCheckFiles(ldtCheckFile); //PIR 14264
                            idlgUpdateProcessLog("Check Files created successfully", "INFO", istrProcessName);
                        }
                        else
                            idlgUpdateProcessLog("No records exist", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Check Files Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        //Create ACH File
        public int CreateACHFile(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 700:
                    try
                    {
                        //Create ACH File
                        idlgUpdateProcessLog("Create ACH File", "INFO", istrProcessName);
                        DataTable ldtACHPaymentDistribution = busBase.Select("cdoPaymentHistoryDistribution.LoadACHPaymentDistribution", new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                        if (ldtACHPaymentDistribution.Rows.Count > 0)
                        {
                            busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                            lobjProcessFiles.iarrParameters = new object[3];
                            lobjProcessFiles.iarrParameters[0] = true;
                            lobjProcessFiles.iarrParameters[1] = ldtACHPaymentDistribution;
                            lobjProcessFiles.iarrParameters[2] = ibusPaymentSchedule.icdoPaymentSchedule.effective_date;
                            lobjProcessFiles.CreateOutboundFile(68);
                            idlgUpdateProcessLog("ACH File created successfully", "INFO", istrProcessName);
                        }
                        else
                            idlgUpdateProcessLog("No records exist", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of ACH File Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }
        //Execute Trial Reports
        public int ExecuteTrialReports(int aintRunSequence)
        {
            int lintrtn = 0;
            busCreateReports lobjCreateReports = new busCreateReports();
            DataTable ldtReportResult;
            DataSet ldsReportResult;
            string lstrReportPath = string.Empty;
            switch (aintRunSequence)
            {
                case 210:
                    try
                    {
                        idlgUpdateProcessLog("Monthly Benefit Payment by Item Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentbyItemReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date, true);
                        if (ldtReportResult.Rows.Count > 0)
                        {

                            lstrReportPath = CreateReportWithPrefix("rptMonthlyBenefitPaymentbyItem.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Monthly Benefit Payment by Item Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into Excel format                                                       
                            lbusBase.CreateExcelReport("rptMonthlyBenefitPaymentbyItem.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Monthly Benefit Payment by Item Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 220:
                    try
                    {
                        idlgUpdateProcessLog("New Retiree Detail Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialNewRetireeDetailReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptNewPayeeDetail.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("New Retiree Detail Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into new Excel format                                                       
                            lbusBase.CreateExcelReport("rptNewPayeeDetailExcel.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                            //For generating correspondence related to BenefitCalculation
                            IEnumerable<DataRow> lenuPayee =
                                from r1 in idtPayeeDetails.AsEnumerable()
                                join r2 in ldtReportResult.AsEnumerable()
                                on r1.Field<int>("payee_account_id") equals r2.Field<int>("payee_account_id")
                                select r1;
                            idtNewPayees = lenuPayee.CopyToDataTable();
                            idecMinimumGuaranteeNewPayees = ldtReportResult.AsEnumerable().Sum(o => o.Field<decimal>("minimum_guarantee_amount"));
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("New Retiree Detail Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 230:
                    try
                    {
                        idlgUpdateProcessLog("Reinstated Retiree Detail Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialReinstatedRetireeDetailReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptReinstatedPayeeDetail.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Reinstated Retiree Detail Report generated succesfully", "INFO", istrProcessName);                                                       
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptReinstatedPayeeDetail.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Reinstated Retiree Detail Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 240:
                    try
                    {
                        idlgUpdateProcessLog("Closed Payee Account Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialClosedorSuspendedPayeeAccountReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptClosedorSuspendedPayeeAccount.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Closed Payee Account Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptClosedorSuspendedPayeeAccount.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                            idecMinimumGuaranteeCancelledorPaymentsCompletePayees =
                                ldtReportResult.AsEnumerable().Where(o => o.Field<string>("data2") == busConstant.PayeeAccountStatusPaymentComplete
                                    || o.Field<string>("data2") == busConstant.PayeeAccountStatusCancelled)
                                    .Sum(o => o.Field<decimal>("minimum_guarantee_amount"));
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Closed Payee Account Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 245:
                    try
                    {
                        idlgUpdateProcessLog("Minimum Guarantee Change Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMinimumGuaranteeChangeSummaryReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);                   
                        DataRow ldrNewDetail = ldtReportResult.NewRow();
                        ldrNewDetail["REPORT_ID"] = 1;
                        ldrNewDetail["REPORT_NAME"] = "New Payee Detail Report";
                        ldrNewDetail["MIN_GUARANTEE_AMOUNT"] = idecMinimumGuaranteeNewPayees;
                        ldtReportResult.Rows.Add(ldrNewDetail);
                        DataRow ldrClosed = ldtReportResult.NewRow();
                        ldrClosed["REPORT_ID"] = 2;
                        ldrClosed["REPORT_NAME"] = "Closed / Suspended Payee Account Report";
                        ldrClosed["MIN_GUARANTEE_AMOUNT"] = idecMinimumGuaranteeCancelledorPaymentsCompletePayees;
                        ldtReportResult.Rows.Add(ldrClosed);                        
                        ldtReportResult.AcceptChanges();
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            DataTable ldtFinal = ldtReportResult.AsEnumerable().OrderBy(o => o.Field<int>("report_id")).AsDataTable();
                            lstrReportPath = CreateReportWithPrefix("rptMinimumGuaranteeChangeSummary.rpt", ldtFinal, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Minimum Guarantee Change Summary Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptMinimumGuaranteeChangeSummary.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Minimum Guarantee Change Summary Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 250:
                    try
                    {
                        idlgUpdateProcessLog("Retirement Option Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialRetirementOptionSummaryReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptRetirementOptionSummary.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Retirement Option Summary Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into Excel format                                                       
                            lbusBase.CreateExcelReport("rptRetirementOptionSummary.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Retirement Option Summary Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 260:
                    try
                    {
                        iclbBenefitPaymentChangePayeeAccounts = new Collection<busPayeeAccount>();
                        idlgUpdateProcessLog("Benefit Payment Change Report", "INFO", istrProcessName);
                        ldsReportResult = new DataSet();
                        ldsReportResult = lobjCreateReports.TrialBenefitPaymentChangeReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (ldsReportResult.Tables.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptBenefitPaymentChange.rpt", ldsReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Benefit Payment Change Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptBenefitPaymentChange_Excel.rpt", ldsReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);                            
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Benefit Payment Change Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 270:
                    try
                    {
                        idlgUpdateProcessLog("Monthly Benefit Payment Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentSummaryReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptMonthlyBenefitPaymentSummary.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            ibusPaymentSchedule.PostEndingBalance(ldtReportResult);
                            idlgUpdateProcessLog("Monthly Benefit Payment Summary Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptMonthlyBenefitPaymentSummary.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Monthly Benefit Payment Summary Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 280:
                    try
                    {
                        idlgUpdateProcessLog("Non-Monthly Payment Detail Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialNonMonthlyPaymentDetailReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptNonMonthlyPaymentDetail.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Non-Monthly Payment Detail Report generated succesfully", "INFO", istrProcessName);                            
                            //PIR-10808 PDF Reports converted into new Excel format                                                        
                            lbusBase.CreateExcelReport("rptNonMonthlyPaymentDetailExcel.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Non-Monthly Payment Detail Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 290:
                    try
                    {
                        idlgUpdateProcessLog("Vendor Payment Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialVendorPaymentSummary(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptVendorPaymentSummary.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Vendor Payment Summary Report generated succesfully", "INFO", istrProcessName);                            
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptVendorPaymentSummary.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Vendor Payment Summary Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }

        //Method to create Final Reports
        private int ExecuteFinalReports(int aintRunSequence)
        {
            int lintrtn = 0;
            string lstrReportPath = string.Empty;
            busCreateReports lobjCreateReports = new busCreateReports();
            DataTable ldtReportResult;
            switch (aintRunSequence)
            {
                case 1000:
                    try
                    {
                        idlgUpdateProcessLog("Final Monthly Benefit Payment per Item Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalMonthlyBenefitPaymentbyItemReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptMonthlyBenefitPaymentbyItemFinal.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Final Monthly Benefit Payment by Item Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into Excel format                                                       
                            lbusBase.CreateExcelReport("rptMonthlyBenefitPaymentbyItemFinal.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Final Monthly Benefit Payment by Item Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1200:
                    try
                    {
                        idlgUpdateProcessLog("Master Payment Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalMasterPaymentReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptMasterPayment.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Master Payment Report generated succesfully", "INFO", istrProcessName);                            
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptMasterPayment.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Master Payment Report Failed.", "INFO", istrProcessName);
                    }
                    break;
                case 1300:
                    try
                    {
                        idlgUpdateProcessLog("Vendor Payment Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalVendorPaymentSummary(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptVendorPaymentSummary.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Vendor Payment Summary Report generated succesfully", "INFO", istrProcessName);                                                       
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptVendorPaymentSummary.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Vendor Payment Summary Report Failed.", "INFO", istrProcessName);
                    }
                    break;
                case 1400:
                    try
                    {
                        idlgUpdateProcessLog("Dues Withholding Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalDuesWithholdingReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptDuesWithholding.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Dues Withholding Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptDuesWithholding.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Dues Withholding Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;                
                case -1:
                    //9th month we need to execute IRS limit batch report
                    DataTable ldtbIRSM = iobjPassInfo.isrvDBCache.GetCodeDescription(busConstant.SystemConstantCodeID, busConstant.IRSLimitBatchRunMonth);
                    int lintIRSLimitBatchRunMonth = ldtbIRSM.Rows.Count > 0 ? Convert.ToInt32(ldtbIRSM.Rows[0]["data1"]) : 0;
                    if (ibusPaymentSchedule.icdoPaymentSchedule.payment_date.Month == lintIRSLimitBatchRunMonth)
                    {
                        try
                        {
                            idlgUpdateProcessLog("IRS 415(b) Limit Report", "INFO", istrProcessName);
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.FinalIRSLimitReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lstrReportPath = CreateReportWithPrefix("rptIRSLimit.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                                llstGeneratedReports.Add(lstrReportPath);
                                idlgUpdateProcessLog("IRS 415(b) Limit Report generated succesfully", "INFO", istrProcessName);
                                //PIR-10808 PDF Reports converted into Excel format                                                                
                                lbusBase.CreateExcelReport("rptIRSLimit.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                            }
                            else
                            {
                                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("IRS 415(b) Limit Report Failed.", "INFO", istrProcessName);
                        }
                    }
                    //Below three reports doesnt have individual steps
                    try
                    {
                        idlgUpdateProcessLog("ACH Register Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalACHRegisterReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = busConstant.ReportTableName;
                        //prod pir 5391 : new table to show payee with multiple ACH
                        DataTable ldtMultipleACH = new DataTable();
                        ldtMultipleACH =lobjCreateReports.MultipleACHOrCheckReport(2, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtMultipleACH.TableName = busConstant.ReportTableName02;
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            DataSet ldsReportResult = new DataSet();
                            ldsReportResult.Tables.Add(ldtReportResult.Copy());
                            ldsReportResult.Tables.Add(ldtMultipleACH.Copy());
                            lstrReportPath = CreateReportWithPrefix("rptACHRegister.rpt", ldsReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("ACH Register Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into new Excel format                                                        
                            lbusBase.CreateExcelReport("rptACHRegisterExcel.rpt", ldsReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);

                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("ACH Register Report Failed.", "INFO", istrProcessName);
                    }
                    try
                    {
                        idlgUpdateProcessLog("Check Register Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalCheckRegisterReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = busConstant.ReportTableName;
                        //prod pir 5391 : new table to show payee with multiple CHK
                        DataTable ldtMultipleCHK= new DataTable();
                        ldtMultipleCHK = lobjCreateReports.MultipleACHOrCheckReport(1, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtMultipleCHK.TableName = busConstant.ReportTableName02;
                        DataTable ldtSummary = new DataTable();
                        ldtSummary = lobjCreateReports.CheckRegisterReportSummary(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtSummary.TableName = busConstant.ReportTableName03;
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            DataSet ldsReportResult = new DataSet();
                            ldsReportResult.Tables.Add(ldtReportResult.Copy());
                            ldsReportResult.Tables.Add(ldtMultipleCHK.Copy());
                            ldsReportResult.Tables.Add(ldtSummary.Copy());
                            lstrReportPath = CreateReportWithPrefix("rptCheckRegister.rpt", ldsReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("Check Register Report generated succesfully", "INFO", istrProcessName);                                                       
                            //PIR-10808 PDF Reports converted into new Excel format                                                        
                            lbusBase.CreateExcelReport("rptCheckRegisterExcel.rpt", ldsReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Check Register Report Failed.", "INFO", istrProcessName);
                    }
                    try
                    {
                        //PIR 24886
                        DataTable ldtMultiplePayments = new DataTable();
                        ldtMultiplePayments = lobjCreateReports.LoadPayeeWithMultiplePaymentsReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtMultiplePayments.TableName = busConstant.ReportTableName;
                        if (ldtMultiplePayments.Rows.Count > 0)
                        {
                            DataSet ldsReportResult = new DataSet();
                            ldsReportResult.Tables.Add(ldtMultiplePayments.Copy());
                            lstrReportPath = CreateReportWithPrefix("rptPayeesWithMultiplePayments.rpt", ldsReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("Payees With Multiple Payments Report generated succesfully", "INFO", istrProcessName);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Payees With Multiple Payments Report Failed.", "INFO", istrProcessName);
                    }
                    try
                    {
                        idlgUpdateProcessLog("Child Support Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalChildSupportReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptChildSupport.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            idlgUpdateProcessLog("Child Support Report generated succesfully", "INFO", istrProcessName);
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptChildSupport.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID + "_",busConstant.PaymentReportPath);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Child Support Report Failed.", "INFO", istrProcessName);
                    }
                    try
                    {
                        CreateReportForPayeesWithMixedPayments(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Payees with mixed payments Report Failed.", "INFO", istrProcessName);
                    }
                    try
                    {
                        CreateReportForRefundsWithPayrollAdjustments(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Refunds With Payroll Adjustments Report Failed.", "INFO", istrProcessName);
                    }
                    break;
            }
            return lintrtn;
        }

        /// <summary>
        /// Method to generate 75 correspondence
        /// </summary>
        public void GenerateCorrespondence()
        {
            try
            {
                //BR-118 The system must generate ‘Company Rollover Confirm’ letter when a ‘Payment History Distribution’ is created for 
                //‘Rollover Organization’.The system must generate ‘Member Rollover Confirm’ letter for the payee with ‘Rollover Instruction’ 
                //and ‘Payment History Distribution’ is created for the Rollover organization. 
                DataTable ldtbRolloverPayments = idtPaymentDetails.AsEnumerable().Where(o =>
                    o.Field<string>("payment_method_value") == busConstant.PaymentDistributionPaymentMethodRACH ||
                    o.Field<string>("payment_method_value") == busConstant.PaymentDistributionPaymentMethodRCHK).AsDataTable();

                foreach (DataRow ldtrPayment in ldtbRolloverPayments.Rows)
                {
                    busPaymentHistoryDistribution lobjPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.LoadRolloverAmounts();
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.RolloverInformation();
                    if (lobjPaymentHistoryDistribution.istrPaymentMethodRollover == busConstant.Flag_Yes)
                    {
                        llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader, "PAY-4008"));
                        llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader, "PAY-4054"));
                    }
                }
                //BR - 131 The system must generate ‘Reaching All Taxable’ letter for the payee who had ‘Non Taxable Amount’ in ‘Payment History Details’ 
                //as of current ‘Benefit Payment Date’ and does not exists in ‘payment History Details’ for next ‘Benefit Payment Date’.
                //Code added so that idnontaxable will contain payee accounts for whom SSLI amount is reduced
                //Corrs. need not be generated for those payees
                DataTable ldtNonTaxable = idtNontaxable.AsEnumerable()
                                                        .Where(o => o.Field<string>("indicator") == busConstant.Flag_Yes)
                                                        .AsDataTable();
                foreach (DataRow ldtrPayment in ldtNonTaxable.Rows)
                {
                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPayeeAccount.icdoPayeeAccount.LoadData(ldtrPayment);
                    lobjPayeeAccount.LoadBenfitAccount();
                    if (lobjPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                        lobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                    lobjPayeeAccount.istrTaxChanges = busConstant.Flag_Yes;
                    lobjPayeeAccount.LoadTaxAmounts(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                    lobjPayeeAccount.LoadNexBenefitPaymentDate();
                    llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4152"));
                }

                //// BR-129 The system must generate ‘Late Contr-Additional Refund’ letter for the payee with 
                ////‘Benefit Account Type’ of ‘Refunds and Transfer’ and the batch is processing ‘Remainder Refund as defined in UCS-057         
                DataTable ldtbAdditionalContribPayments = idtPaymentDetails.AsEnumerable().Where(o =>
                    o.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRefund &&
                   (o.Field<string>("calculation_type_value") == busConstant.CalculationTypeAdjustments ||  //PIR 18053
                   o.Field<string>("calculation_type_value") == busConstant.CalculationTypeSubsequentAdjustment)).AsDataTable();
                foreach (DataRow ldtrPayment in ldtbAdditionalContribPayments.Rows)
                {
                    busPaymentHistoryDistribution lobjPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.ibusApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.ibusBenefitAccount = new busBenefitAccount { icdoBenefitAccount = new cdoBenefitAccount() };
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();
                    foreach (busBenefitApplicationPersonAccount lobjBAPA in lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts)
                    {
                        if (lobjBAPA.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes)
                        {
                            lobjBAPA.LoadPersonAccountRetirement();
                            lobjBAPA.ibusPersonAccountRetirement.LoadLTDSummaryAsonDate(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                            lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.idecTotalAccountBalance
                                = lobjBAPA.ibusPersonAccountRetirement.Member_Account_Balance_ltd;
                        }
                    }
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadTaxableAmount(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadNontaxableAmount();
                    if (lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPaymentHistoryHeader == null)
                        lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadPaymentHistoryHeader();
                    busPaymentHistoryHeader lobjPaymentHistoryHeader
                        = lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPaymentHistoryHeader.Where(o =>
                    o.icdoPaymentHistoryHeader.payment_date < ibusPaymentSchedule.icdoPaymentSchedule.effective_date).FirstOrDefault();
                    if (lobjPaymentHistoryHeader != null && lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id > 0)
                    {
                        if (lobjPaymentHistoryHeader.ibusOrganization == null)
                            lobjPaymentHistoryHeader.LoadOrganization();
                        lobjPaymentHistoryDistribution.istrPriorRolloverOrg = lobjPaymentHistoryHeader.ibusOrganization.icdoOrganization.org_name;
                    }
                    llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPaymentHistoryDistribution, "PAY-4200"));
                }

                
                ////BR-120 The system must generate ‘Disability 20th Payment’ letter for the payee when the ‘benefit Account Type’ is ‘Disability’ and 
                ////‘Benefit Sub Type’ is ‘Disability’  or ‘Benefit Account Type’ is ‘Retirement’ and ‘Benefit Account Sub Type’ is ‘Disability to Normal Retirement’ and number of ‘Payment History – Distribution’ is twenty.
                DataTable ldtbDisability = idtPaymentDetails.AsEnumerable().Where(o =>
                    (o.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement &&
                    o.Field<string>("benefit_account_sub_type_value") == busConstant.ApplicationBenefitSubTypeDisabilitytoNormal) ||
                    (o.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeDisability &&
                    o.Field<string>("benefit_account_sub_type_value") == busConstant.ApplicationBenefitSubTypeDisability)).AsDataTable();

                foreach (DataRow ldtrPayment in ldtbDisability.Rows)
                {
                    DateTime ldtBenefitBeginDate = DateTime.MinValue;
                    busPaymentHistoryDistribution lobjPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.LoadData(ldtrPayment);
                    //uat pir 1973 : to check whether any disability PA exists, then number of payments to be calculated from disability PA benefit begin date
                    if (lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement &&
                        lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDisabilitytoNormal)
                    {
                        lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadApplication();
                        lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.ibusApplication.LoadPayeeAccount();
                        busPayeeAccount lobjPayeeAccount = lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.ibusApplication.iclbPayeeAccount
                                                                        .Where(o => o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                                                                        .FirstOrDefault();
                        if (lobjPayeeAccount != null)
                            ldtBenefitBeginDate = lobjPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                        else
                            ldtBenefitBeginDate = lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                    }
                    else
                        ldtBenefitBeginDate = lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                    
                    //20th payment to be calculated based on benefit begin date
                    if (busGlobalFunctions.DateDiffByMonth(ldtBenefitBeginDate, ibusPaymentSchedule.icdoPaymentSchedule.payment_date) == 20)
                    {
                        llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount, "PAY-4262"));
                    }                   
                }
                
                ////The system must generate ‘SSLI Age Attained’ letter for the payee with ‘Benefit Account Type’ is ‘Retirement’, ‘Benefit Option’ is ‘Single Life’
                //or ‘Straight Life’ and ‘SSLI/Uniform Income’ is selected and the ‘SSLI/Uniform Reduction Date’ is equal to next ‘Benefit Payment Date’.
                DataTable ldtbSsli = idtPaymentDetails.AsEnumerable().Where(o =>
                    (o.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement &&
                    (o.Field<string>("benefit_option_value") == busConstant.BenefitOptionSingleLife ||
                    o.Field<string>("benefit_option_value") == busConstant.BenefitOptionStraightLife) &&
                    o.Field<string>("uniform_income_or_ssli_flag") == busConstant.Flag_Yes) &&
                    o.Field<DateTime>("ssli_change_date") >= ibusPaymentSchedule.icdoPaymentSchedule.payment_date.AddMonths(1) &&
                    o.Field<DateTime>("ssli_change_date") <= ibusPaymentSchedule.icdoPaymentSchedule.payment_date.AddMonths(2).AddDays(-1)).AsDataTable();
                foreach (DataRow ldtrPayment in ldtbSsli.Rows)
                {
                    busPaymentHistoryDistribution lobjPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.LoadData(ldtrPayment);
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadTaxAmounts();
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadOtherDeductionAmount();
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadInsurancePremiumAmount();
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadGrossBenefitAmount();
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadBenefitAmount();
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.idecGrossBenfitAmount +=
                    lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.LoadLatestPAPITAmount(busConstant.PAPITRHICBenefitReimbursement);
                    llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount, "PAY-4201"));
                }

                //Load benefit payment change correspondence data
                LoadBenefitPaymentChangeCorrespondenceData();
                /*foreach (busPayeeAccount lobjPayeeAccount in iclbBenefitPaymentChangePayeeAccounts)
                {
                    llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4261"));
                }*/

                // The system must generate ‘Final Beneficiary Payment’ letter for the payee with ‘Benefit End Date’ equal to ‘Benefit Payment Date’.

                DataTable ldtbBene = busNeoSpinBase.Select("cdoPayeeAccountStatus.LoadPaymentCompletePayeeAccounts",
                                                          new object[2] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                                                                       ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id});

                Collection<busPayeeAccount> lclbFinalPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(ldtbBene, "icdoPayeeAccount");


                //Collection<busPayeeAccount> lclbProcessedAccounts = lobjBase.GetCollection<busPayeeAccount>(idtPaymentDetails, "icdoPayeeAccount");
                //var lclbFinalPayeeAccounts = lclbProcessedAccounts.Where(o =>
                //    o.icdoPayeeAccount.benefit_end_date == DateTime.MinValue);
                if (lclbFinalPayeeAccounts != null && lclbFinalPayeeAccounts.Count() > 0)
                {
                    foreach (busPayeeAccount lobjPayeeAccount in lclbFinalPayeeAccounts)
                    {                       
                        if (lobjPayeeAccount.ibusApplication == null)
                            lobjPayeeAccount.LoadApplication();
                        if (lobjPayeeAccount.ibusApplication.ibusPerson == null)
                            lobjPayeeAccount.ibusApplication.LoadPerson();
                        llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4102"));
                    }
                }

                //PAY - 4103 - its dependent on New payee report, so moved down
                /*
                //Alternate Payee
                DataTable ldtbAlternatePayee = idtPaymentDetails.AsEnumerable().Where(o =>
                                        o.Field<string>("dro_model_value") != null).AsDataTable();

                Collection<busPayeeAccount> lclbldtbAlternatePayee = lobjBase.GetCollection<busPayeeAccount>(ldtbAlternatePayee, "icdoPayeeAccount");

                if (lclbldtbAlternatePayee != null && lclbldtbAlternatePayee.Count() > 0)
                {
                    foreach (busPayeeAccount lobjPayeeAccount in lclbldtbAlternatePayee)
                    {
                        decimal ldecTaxable = 0.0m, ldecNontaxable = 0.0m;
                        if (lobjPayeeAccount.ibusPayee == null)
                            lobjPayeeAccount.LoadPayee();
                        lobjPayeeAccount.LoadBenefitAmount();
                        lobjPayeeAccount.LoadDROApplication();
                        //lobjPayeeAccount.ibusDROApplication.CalculateMonthlyPaymentItems(
                        //    lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value,lobjPayeeAccount.icdoPayeeAccount.benefit_option_value,
                        //    lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value,ref ldecTaxable,ref ldecNontaxable);
                        llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4103"));
                    }
                }*/

                DataTable ldtbDeathRefundMembers = busNeoSpinBase.Select("cdoPayeeAccount.LoadDeathRefundMemberAccountDetails",
                                                   new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                //DataTable ldtbDeathRefundPayees = busNeoSpinBase.Select("cdoPayeeAccount.LoadDeathRefundPayeeAccountDetails",
                //                                new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                foreach (DataRow ldtrMember in ldtbDeathRefundMembers.Rows)
                {
                    busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };

                    lobjPerson.ibusBenefitAccount = new busBenefitAccount { icdoBenefitAccount = new cdoBenefitAccount() };

                    lobjPerson.icdoPerson.LoadData(ldtrMember);

                    lobjPerson.ibusBenefitAccount.icdoBenefitAccount.LoadData(ldtrMember);

                    lobjPerson.LoadPaymentDetails();

                    lobjPerson.ibusBenefitAccount.LoadPayeeAccounts();

                    lobjPerson.ibusBenefitAccount.LoadPayeesAddress(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);

                    llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPerson, "PAY-4020"));
                }
                
                busOrganization lobjPriorJudgesOrganization = new busOrganization();
                lobjPriorJudgesOrganization.FindOrganizationByOrgCode(busConstant.PriorJudgesOrgCode);
                if (lobjPriorJudgesOrganization.icdoOrganization.org_id > 0)
                {
                    DataTable ldtbJudges = idtPaymentDetails.AsEnumerable().Where(o =>
                                         o.Field<int>("plan_id") == busConstant.PlanIdPriorJudges).AsDataTable();
                    if (ldtbJudges.Rows.Count > 0)
                    {
                        lobjPriorJudgesOrganization.LoadPaymentsForJudges(ldtbJudges);
                        if (lobjPriorJudgesOrganization.iclbPaymentHistoryHeader != null && lobjPriorJudgesOrganization.iclbPaymentHistoryHeader.Count > 0)
                        {
                            llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPriorJudgesOrganization, "PAY-4017"));
                        }
                    }
                }

                //All benefit calculation related Correspondence(Payees coming in New payee detail report)

                // PIR 9282 - Alpha sorting of correspondences on the basis of LastName,FirstName

                if (idtNewPayees.Rows.Count > 0) 
                    idtNewPayees = idtNewPayees.AsEnumerable()
                                                .OrderBy(row => row.Field<string>("Last_Name"))
                                                .ThenBy(row => row.Field<string>("First_Name"))
                                                .AsDataTable();

                foreach (DataRow dr in idtNewPayees.Rows)
                {
                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                    lobjPayeeAccount.ibusBenefitCalculaton = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
                    lobjPayeeAccount.ibusBenefitAccount = new busBenefitAccount { icdoBenefitAccount = new cdoBenefitAccount() };
                    lobjPayeeAccount.ibusApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lobjPayeeAccount.ibusMember = new busPerson { icdoPerson = new cdoPerson() };
                    lobjPayeeAccount.ibusDROApplication = new busBenefitDroApplication { icdoBenefitDroApplication = new cdoBenefitDroApplication() };
                    lobjPayeeAccount.icdoPayeeAccount.LoadData(dr);
                    //prod pir 6252 : letter should not be generated for disability to normal payee account
                    if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value != busConstant.ApplicationBenefitSubTypeDisabilitytoNormal)
                    {
                        lobjPayeeAccount.ibusMember.icdoPerson.LoadData(dr);
                        lobjPayeeAccount.ibusApplication.icdoBenefitApplication.LoadData(dr);
                        lobjPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.LoadData(dr);
                        lobjPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.LoadData(dr);
                        lobjPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.LoadData(dr);
                        lobjPayeeAccount.LoadMonthlyBenefitAmount(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        lobjPayeeAccount.NumberofMonthsFirstPaymentRepresents(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        lobjPayeeAccount.FormatPaymentDate(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        lobjPayeeAccount.LoadPlanCode();
                        lobjPayeeAccount.LoadJSPercent();
                        lobjPayeeAccount.LoadJointAnnuitant();
                        lobjPayeeAccount.LoadBenefits();
                        lobjPayeeAccount.SetHPJudgesNBCondition();
                        lobjPayeeAccount.SetBenefitAcctSubType();
                        lobjPayeeAccount.SetJobServiceCondition();
                        lobjPayeeAccount.SetJSRHICCondition();
                        lobjPayeeAccount.LoadSurvivingSpouseCondition();
                        lobjPayeeAccount.SetQDROApplies();
                        lobjPayeeAccount.SetRetirementCondtion();
                        lobjPayeeAccount.SetReducedBenefit();
                        lobjPayeeAccount.LoadRTWOrReducedBeneftiOption();

                        //to load last month monthly batch
                        if (ibusLastPaymentScheule == null)
                            LoadPaymentScheduleByPaymentDate(ibusPaymentSchedule.icdoPaymentSchedule.payment_date.AddMonths(-1));
                        DataTable ldtPaymentHistroyHeader = busBase.SelectWithOperator<cdoPaymentHistoryHeader>
                            (new string[3] { enmPaymentHistoryHeader.payee_account_id.ToString(), enmPaymentHistoryHeader.payment_date.ToString(),
                    enmPaymentHistoryHeader.payment_date.ToString()},
                            new string[3] { "=", ">", "<" },
                            new object[3] { lobjPayeeAccount.icdoPayeeAccount.payee_account_id, ibusLastPaymentScheule.icdoPaymentSchedule.effective_date,
                    ibusPaymentSchedule.icdoPaymentSchedule.effective_date}, null);
                        //to check whether payee account got paid in Adhoc batch after last monthly batch.
                        if (ldtPaymentHistroyHeader.Rows.Count == 0)
                        {
                            //BR-075-121
                            //The system must generate ‘Disability-SL Normal’ letter for the payee when ‘Benefit Account Type’ is ‘Disability’ and 
                            //‘Benefit Option’ is ‘Single Life’ or ‘Normal’ and the payee is selected in the ‘New Payee Detail Report’.
                            //PIR 16574 - Removed condition for BenefitSubType != Disability Retirement
                            if ((lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
                                (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionSingleLife ||
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit))
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4263"));

                            //BR-075-122
                            //The system must generate ‘Disability-TC’ letter for the payee with:
                            //    •	‘Benefit Account Type’ as ‘Disability’ and ‘Benefit Sub Type’ of ‘Disability’ 
                            //    •	OR ‘Benefit Account Type’ of ‘Retirement’ and ‘Benefit Sub Type’ of ‘Disability to Normal’ 
                            //    •	AND ‘Benefit Option’ 
                            //    o	10 Year Term Certain
                            //    o	20 Year Term Certain
                            //    •	AND payee is selected in the ‘New Payee Detail Report’.
                            // Backlog PIR 10307 - added brackets
                            if  (((lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability &&
                                lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDisability) ||
                                (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement &&
                                lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDisabilitytoNormal)) &&
                                (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption10YearCertain ||
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption20YearCertain))
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4264"));

                            //BR-075-124
                            //The system must generate ‘HP-Judge Normal Benefit’ letter for the payee with ‘Benefit Option’ of ‘Normal Retirement’ and 
                            //‘Plan’ is ‘Judges’ or ‘Highway Patrol’ plan and the payee is populated in the ‘New Payee Detail Report’.
                            if (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit &&
                                (lobjPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdHP ||
                                lobjPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJudges))
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4010"));

                            //BR-075-126
                            //The system must generate ‘Job Service Alt Payee or Beneficiary’ for the payee with ‘Account Relationship of ‘Spouse’, ‘Child’ or 
                            //‘Alternate Payee’ and ‘Plan’ is ‘Job Service’ and the payee is selected in the ‘New Payee Detail Report’.
                            if ((lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipBeneficiary ||
                                lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipAlternatePayee) &&
                                lobjPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJobService)
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4011"));

                            //BR-075-127
                            //The system must generate ‘Job Service Disability’ letter for the payee with ‘Account Relationship’ of ‘Member’ and 
                            //‘Benefit Option’ is ‘Disability’ and ‘Plan’ is ‘Job Service’ and the payee is selected in the ‘New Payee Detail Report’.
                            if (lobjPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJobService &&
                                lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember &&
                                lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4012"));

                            //BR-075-128
                            //The system must generate ‘Job Service Retirement’ letter for the payee with ‘Account Relationship’ of ‘Member’ and 
                            //‘Benefit Option’ is ‘Straight Life’ and ‘Plan’ is ‘Job Service’ and the payee is selected in the ‘New Payee Detail Report’.
                            if (lobjPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJobService &&
                               lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember &&
                               lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeDisability) //prod pir 4345
                                //lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionStraightLife)
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4013"));

                            //BR-075-133
                            //The system must generate ‘Retirement-SL’ for the payee with ‘Benefit Account Type’ is ‘Retirement’ 
                            //and ‘Benefit Option’ is ‘Single Life’ and ‘SSLI/Uniform Income’ is not selected. 
                            //The payee also must be selected in the ‘New Payee Detail Report’.
                            if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement &&
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionSingleLife &&
                                lobjPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.estimated_ss_benefit_amount == 0)
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4014"));

                            //BR-075-134	
                            //The system must generate ‘Retirement-TC’ letter for the payee with ‘Benefit Account Type’ is ‘Retirement’ 
                            //and ‘Benefit Option’ is ’10 Year Term Certain’ or ’20 year Term Certain’ and the payee is selected in the ‘New Payee Detail Report’.
                            if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement &&
                                (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption10YearCertain ||
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption20YearCertain) &&
                                lobjPayeeAccount.ibusPlan.icdoPlan.plan_id != busConstant.PlanIdJobService) // PROD PIR ID 5773
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4015"));

                            //BR-075-135		
                            //The system must generate ‘Retirement-SSLI’ letter for the payee with ‘Benefit Account Type’ is ‘Retirement’ 
                            //and ‘Benefit Option’ is ‘Single Life’ and ‘SSLI/Uniform Income’ is selected. The payee also must be selected in the ‘New Payee Detail Report’.
                            if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement &&
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionSingleLife &&
                                lobjPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.estimated_ss_benefit_amount > 0)
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4016"));

                            //BR-075-138		
                            //The system must generate ‘Disability-J&S’ letter for the payee with ‘Benefit Account Type’ as ‘Disability’, 
                            //‘Benefit Option’ of ‘50% Joint & Survivor’ or ‘100% Joint & Survivor’, Plan is not ‘Job service’ and 
                            //    the payee selected in the ‘New Payee Detail Report’.
                            if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability &&
                                (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption50PercentJS ||
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption100PercentJS) &&
                                lobjPayeeAccount.ibusPlan.icdoPlan.plan_id != busConstant.PlanIdJobService)
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4265"));

                            //BR-075-139		
                            //The system must generate ‘Retirement-J&S’ letter for the payee with ‘Benefit Account Type’ as ‘Retirement’, 
                            //‘Benefit Option’ of ‘50% Joint & Survivor’ or ‘100% Joint & Survivor’, Plan is not ‘Job service’ and 
                            //    the payee selected in the ‘New Payee Detail Report’.
                            if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement &&
                                (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption50PercentJS ||
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption100PercentJS) &&
                                lobjPayeeAccount.ibusPlan.icdoPlan.plan_id != busConstant.PlanIdJobService)
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4018"));

                            //BR-075-140		
                            //The system must generate ‘Surviving Spouse-J&S’ letter to the payee with ‘Account Relationship’ is ‘Joint Annuitant’, 
                            //‘Benefit Option’ of ‘50% Joint & Survivor’ or ‘100% Joint & Survivor’, Plan is not ‘Job service’ and 
                            //    the payee selected in the ‘New Payee Detail Report’.
                            //PIR - 16348 -  Added 50% Life benefit option to the conditions
                            if (lobjPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipJointAnnuitant &&
                                (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption50PercentJS ||
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption100PercentJS || 
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption50Percent) &&
                                lobjPayeeAccount.ibusPlan.icdoPlan.plan_id != busConstant.PlanIdJobService)
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4019"));

                            //BR-075-119		
                            //The system must generate ‘Defined Contribution’ letter for the payee once the ‘Payee Status’ is updated from ‘Approved’ to ‘DC Receiving’.
                            if (lobjPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC ||
                               lobjPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                               lobjPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC2025) //PIR 25920
                            {
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4009"));
                            }

                            //BR-075-117	
                            //The system must generate ‘Beneficiary-TC’ letter for ‘Account Relationship’ other than ‘Member’ and ‘Alternate Payee’ and 
                            //there exists a payee account under same benefit account with ‘Account Relationship’ as ‘Member’ with status as ‘Payment Complete’ and
                            //the other payee account did not receive payments last month i.e. the payee account is displayed in ‘New Payee Detail Report’.
                            if (lobjPayeeAccount.icdoPayeeAccount.account_relation_value != busConstant.AccountRelationshipMember &&
                                   lobjPayeeAccount.icdoPayeeAccount.account_relation_value != busConstant.AccountRelationshipAlternatePayee &&
                                (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption10YearCertain ||
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption15YearCertain ||
                                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption20YearCertain))//uat pir 2217 : benefit option also to be added
                            {
                                lobjPayeeAccount.ibusBenefitAccount.LoadPayeeAccounts();
                                busPayeeAccount lobjPA = lobjPayeeAccount.ibusBenefitAccount.iclbPayeeAccount
                                    .Where(o => o.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember)
                                    .FirstOrDefault();
                                if (lobjPA != null)
                                {
                                    lobjPA.LoadPayeeAccountStatus(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                                    if (lobjPA.ibusPASOnGivenDate != null &&
                                        lobjPA.ibusPASOnGivenDate.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusPaymentComplete)
                                        llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4007"));
                                }
                            }

                            //BR-075-116a	
                            //The system must generate ‘Alternate Payee’ letter for 'Account Relationship' of 'Alternate Payee' and did not receive any payments last month 
                            //i.e. when this payee account is populated in ‘New Payee Detail Report’.
                            if (!string.IsNullOrEmpty(lobjPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.dro_model_value))
                            {
                                if (lobjPayeeAccount.ibusPayee == null)
                                    lobjPayeeAccount.LoadPayee();
                                lobjPayeeAccount.LoadBenefitAmount();
                                //pir 7891 start
                                lobjPayeeAccount.LoadGrossBenefitAmount();
                                lobjPayeeAccount.SetFirstPaymentDate();
                                lobjPayeeAccount.SetNumberOfPayments();
                                //end
                                llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPayeeAccount, "PAY-4103"));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private string CreateCorrespondence(busPerson aobjPerson, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjPerson);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            return CreateCorrespondence(astrCorName, aobjPerson, lhstDummyTable);
        }
        private string CreateCorrespondence(busPaymentHistoryDistribution aobjPaymentHistoryDistribution, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjPaymentHistoryDistribution);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            return CreateCorrespondence(astrCorName, aobjPaymentHistoryDistribution, lhstDummyTable);
        }
        private string CreateCorrespondence(busOrganization abusOrganization, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(abusOrganization);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            return CreateCorrespondence(astrCorName, abusOrganization, lhstDummyTable);
        }
        private string CreateCorrespondence(busPaymentHistoryHeader aobjPaymentHistoryHeader, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjPaymentHistoryHeader);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            return CreateCorrespondence(astrCorName, aobjPaymentHistoryHeader, lhstDummyTable);
        }
        private string CreateCorrespondence(busPayeeAccount aobjPayeeAccount, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjPayeeAccount);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            return CreateCorrespondence(astrCorName, aobjPayeeAccount, lhstDummyTable);
        }

        //Back up data Prior to Batch
        public int BackUpData(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 300:
                    try
                    {
                        if (!string.IsNullOrEmpty(ibusPaymentSchedule.icdoPaymentSchedule.backup_table_prefix))
                        {
                            idlgUpdateProcessLog("Deleting back up table for the current schedule id", "INFO", istrProcessName);
                            lobjPaymentProcess.DeleteBackUpDataForCurrentScheduleId(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                            idlgUpdateProcessLog("Deleting back up table for the current schedule id Successful.", "INFO", istrProcessName);
                        }
                        idlgUpdateProcessLog("Back-up data Prior to Batch", "INFO", istrProcessName);
                        ibusPaymentSchedule.icdoPaymentSchedule.backup_table_prefix = DBFunction.DBExecuteScalar("cdoPayeeAccount.BackupBeforePayroll",
                                  new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id },
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework).ToString();
                        idlgUpdateProcessLog((lintrtn < 0) ? "Back-up data Prior to Batch Failed." : "Back-up data - Successful.", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Back-up data Prior to Batch Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
            }
            return lintrtn;
        }

        public void InitiateRHICCombineWorkflow(int aintPersonID)
        {
            
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Maintain_Rhic, aintPersonID, 0, 0, iobjPassInfo);
        }

        #region PIR 14264

        public DataTable idtPaymentDetailsMonthly { get; set; }
        public DataTable idtGHDVPersonAccount { get; set; }
        public DataTable idtPayments { get; set; }
        public DataTable idtRHICDetails { get; set; }
        public DateTime idtPaymentDate { get; set; }
        public busDBCacheData ibusDBCacheData { get; set; }
        public DataTable idtbGHDVHistory { get; set; }

        private string iabsRptDefPath;
        private string iabsRptGenPath;
        public ReportDocument irptDocument;

        //Call this Method outside the Loop (From Caller) for Optimization
        public void InitializeReportBuilder(string astrReportGNPath)
        {
            iabsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptDF");
            iabsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(astrReportGNPath);
        }

        // Initialize the report documnet. This event removes any databse logon information 
        // saved in the report. The call to Load the report in the above function fires this event.
        private void OnReportDocInit(object sender, System.EventArgs e)
        {
            irptDocument.SetDatabaseLogon("", "");
        }

        public string CreateReportCheck(string astrReportName, DataSet adstResult, string astrPrefix, string astrReportGNPath)
        {
            InitializeReportBuilder(astrReportGNPath);
            string lstrReportFullName = string.Empty;
            irptDocument = new ReportDocument();
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init            
            irptDocument.Load(iabsRptDefPath + astrReportName);
            // gets the data and bind to the report doc control
            irptDocument.SetDataSource((DataSet)adstResult);

            lstrReportFullName = iabsRptGenPath + astrPrefix + ".rpt_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            irptDocument.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName + ".pdf");
            irptDocument.Close();
            irptDocument.Dispose();
            return lstrReportFullName;
        }

        public void CreateCheckFiles(DataTable adtCheckFile)
        {
            CreateNewDataTablePayments();

            LoadCheckFile(adtCheckFile);
            foreach (DataRow ldtbRow in adtCheckFile.Rows)
            {
                DataSet ldsCheckFile = new DataSet();
                DataTable ldtbData = new DataTable();
                DataTable ldtPaymentDetails = new DataTable();

                ldtbData = adtCheckFile.Clone();
                ldtbData.ImportRow(ldtbRow);
                ldtbData.TableName = busConstant.ReportTableName;

                ldtPaymentDetails = (from ldrPaymentDetails in idtPaymentDetailsMonthly.AsEnumerable()
                                     where (ldrPaymentDetails.Field<int>("PAYMENT_HISTORY_HEADER_ID") == Convert.ToInt32(ldtbRow["PAYMENT_HISTORY_HEADER_ID"]))
                                     select ldrPaymentDetails).AsDataTable();
                ldtPaymentDetails.TableName = busConstant.ReportTableName02;

                ldsCheckFile.Tables.Add(ldtbData.Copy());
                ldsCheckFile.Tables.Add(ldtPaymentDetails.Copy());

                CreateReportCheck("rptCheckFile.rpt", ldsCheckFile, "Check_" + ldtbRow["CHECK_NUMBER"].ToString(), "PHChkOut");
            }
        }

        public void CreateNewDataTablePayments()
        {
            idtPaymentDetailsMonthly = new DataTable();

            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("PAYMENT_HISTORY_HEADER_ID", Type.GetType("System.Int32")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TOTAL_RHIC_AMOUNT", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("GROUP_HEALTH_PREMIUM", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("MEDICARE_PREMIUM", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("RHIC_APPLIED", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("NET_PREMIUM", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("NET_AMT_IN_WORDS", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("NET_AMT_PADDED", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CHECK_BOTTOM", Type.GetType("System.String")));

            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TYPE1", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CURRENT1", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("YTD1", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TYPE2", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CURRENT2", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("YTD2", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TYPE3", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CURRENT3", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("YTD3", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TYPE4", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CURRENT4", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("YTD4", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TYPE5", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CURRENT5", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("YTD5", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TYPE6", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CURRENT6", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("YTD6", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TYPE7", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CURRENT7", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("YTD7", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TYPE8", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CURRENT8", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("YTD8", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TYPE9", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CURRENT9", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("YTD9", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("TYPE10", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("CURRENT10", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("YTD10", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_TYPE1", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_CURRENT1", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_YTD1", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_TYPE2", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_CURRENT2", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_YTD2", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_TYPE3", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_CURRENT3", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_YTD3", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_TYPE4", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_CURRENT4", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_YTD4", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_TYPE5", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_CURRENT5", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_YTD5", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_TYPE6", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_CURRENT6", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_YTD6", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_TYPE7", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_CURRENT7", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_YTD7", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_TYPE8", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_CURRENT8", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_YTD8", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_TYPE9", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_CURRENT9", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_YTD9", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_TYPE10", Type.GetType("System.String")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_CURRENT10", Type.GetType("System.Decimal")));
            idtPaymentDetailsMonthly.Columns.Add(new DataColumn("DED_YTD10", Type.GetType("System.Decimal")));
        }

        public void LoadCheckFile(DataTable adtCheckFile)
        {
            DateTime ldtPaymentDate = ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
            idtPayments = busBase.Select("cdoPaymentHistoryDetail.LoadPaymentDeductionRecords", new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
            idtGHDVPersonAccount = busBase.Select("cdoPaymentHistoryDetail.LoadGHDVAccounts", new object[2] { ldtPaymentDate, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
            idtRHICDetails = busBase.Select("cdoBenefitRhicCombine.LoadRHICDetails", new object[2] { ldtPaymentDate, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
            idtPaymentDate = ldtPaymentDate;

            //Loading DB Cache (optimization)
            LoadDBCacheData();

            //Loading the GHDV History (Optimization)
            LoadGHDVHistory(ldtPaymentDate);

            foreach (DataRow dr in adtCheckFile.Rows)
            {
                busCheckFileData lobjCheckFileData = new busCheckFileData();
                lobjCheckFileData.InitializeObjects();

                lobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(dr);
                lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.LoadData(dr);
                if (!string.IsNullOrEmpty(lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description) &&
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_value != busConstant.US_Code_ID)
                {
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description =
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description.ToUpper();
                }
                else
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description = string.Empty;

                LoadRHICDetails(lobjCheckFileData);

                LoadPaymentDetails(lobjCheckFileData);
            }
        }


        public void LoadDBCacheData()
        {
            if (ibusDBCacheData == null)
                ibusDBCacheData = new busDBCacheData();
            ibusDBCacheData.idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedRateStructureRef = busGlobalFunctions.LoadHealthRateStructureCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHealthRate = busGlobalFunctions.LoadHealthRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLifeRate = busGlobalFunctions.LoadLifeRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedDentalRate = busGlobalFunctions.LoadDentalRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHMORate = busGlobalFunctions.LoadHMORateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLtcRate = busGlobalFunctions.LoadLTCRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedVisionRate = busGlobalFunctions.LoadVisionRateCacheData(iobjPassInfo);
        }

        public void LoadGHDVHistory(DateTime adtBatchDate)
        {
            idtbGHDVHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadGHDVHistory", new object[1] { adtBatchDate });
        }

        /// <summary>
        /// method to load payment details (Both payments and deductions)
        /// </summary>
        /// <param name="aobjCheckFileData">Check file business object</param>
        private void LoadPaymentDetails(busCheckFileData aobjCheckFileData)
        {
            DataTable ldtPayments = new DataTable();
            DataTable ldtDeductions = new DataTable();
            ldtPayments = (from ldrPayment in idtPayments.AsEnumerable()
                           where (ldrPayment.Field<int?>("payee_account_id").IsNull() || ldrPayment.Field<int>("payee_account_id") ==
                                               aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id) &&
                                               ldrPayment.Field<decimal>("ftm_payment") != 0 &&
                                   ldrPayment.Field<int>("item_type_direction") == 1 &&
                                   (
                                   aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id > 0 &&
                                       ((ldrPayment.Field<int?>("person_id").IsNotNull() && ldrPayment.Field<int>("person_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id) ||
                                           (ldrPayment.Field<int?>("org_id").IsNotNull() && ldrPayment.Field<int>("org_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id))
                                   )
                           orderby ldrPayment.Field<int>("payment_item_type_id")
                           select ldrPayment).AsDataTable();
            ldtDeductions = (from ldrPayment in idtPayments.AsEnumerable()
                             where (ldrPayment.Field<int?>("payee_account_id").IsNull() || ldrPayment.Field<int>("payee_account_id") ==
                                                 aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id) &&
                                                 ldrPayment.Field<decimal>("ftm_deduction") != 0 &&
                                 ldrPayment.Field<int>("item_type_direction") == -1 &&
                                 (
                                 aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id > 0 &&
                                     ((ldrPayment.Field<int?>("person_id").IsNotNull() && ldrPayment.Field<int>("person_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id) ||
                                         (ldrPayment.Field<int?>("org_id").IsNotNull() && ldrPayment.Field<int>("org_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id))
                                 )
                             orderby ldrPayment.Field<int>("payment_item_type_id")
                             select ldrPayment).AsDataTable();

            int lintChkCompNumber = 1;
            DataRow drNew = idtPaymentDetailsMonthly.NewRow();
            foreach (DataRow dr in ldtPayments.Rows)
            {
                if (Convert.ToDecimal(dr["ftm_payment"]) != 0 || Convert.ToDecimal(dr["ytd_payment"]) != 0)
                {
                    switch (lintChkCompNumber)
                    {
                        case 1:
                            drNew["TYPE1"] = dr["item_type_description"];
                            drNew["CURRENT1"] = dr["ftm_payment"];
                            drNew["YTD1"] = dr["ytd_payment"];
                            break;
                        case 2:
                            drNew["TYPE2"] = dr["item_type_description"];
                            drNew["CURRENT2"] = dr["ftm_payment"];
                            drNew["YTD2"] = dr["ytd_payment"];
                            break;
                        case 3:
                            drNew["TYPE3"] = dr["item_type_description"];
                            drNew["CURRENT3"] = dr["ftm_payment"];
                            drNew["YTD3"] = dr["ytd_payment"];
                            break;
                        case 4:
                            drNew["TYPE4"] = dr["item_type_description"];
                            drNew["CURRENT4"] = dr["ftm_payment"];
                            drNew["YTD4"] = dr["ytd_payment"];
                            break;
                        case 5:
                            drNew["TYPE5"] = dr["item_type_description"];
                            drNew["CURRENT5"] = dr["ftm_payment"];
                            drNew["YTD5"] = dr["ytd_payment"];
                            break;
                        case 6:
                            drNew["TYPE6"] = dr["item_type_description"];
                            drNew["CURRENT6"] = dr["ftm_payment"];
                            drNew["YTD6"] = dr["ytd_payment"];
                            break;
                        case 7:
                            drNew["TYPE7"] = dr["item_type_description"];
                            drNew["CURRENT7"] = dr["ftm_payment"];
                            drNew["YTD7"] = dr["ytd_payment"];
                            break;
                        case 8:
                            drNew["TYPE8"] = dr["item_type_description"];
                            drNew["CURRENT8"] = dr["ftm_payment"];
                            drNew["YTD8"] = dr["ytd_payment"];
                            break;
                        case 9:
                            drNew["TYPE9"] = dr["item_type_description"];
                            drNew["CURRENT9"] = dr["ftm_payment"];
                            drNew["YTD9"] = dr["ytd_payment"];
                            break;
                        case 10:
                            drNew["TYPE10"] = dr["item_type_description"];
                            drNew["CURRENT10"] = dr["ftm_payment"];
                            drNew["YTD10"] = dr["ytd_payment"];
                            break;
                    }
                }
                lintChkCompNumber++;
            }
            aobjCheckFileData.idecPayTotals = (from ldrPayment in ldtPayments.AsEnumerable()
                                               select ldrPayment.Field<decimal>("ftm_payment")).Sum();

            lintChkCompNumber = 1;
            foreach (DataRow dr in ldtDeductions.Rows)
            {
                if (Convert.ToDecimal(dr["ftm_deduction"]) != 0 || Convert.ToDecimal(dr["ytd_deduction"]) != 0)
                {
                    switch (lintChkCompNumber)
                    {
                        case 1:
                            drNew["DED_TYPE1"] = dr["item_type_description"];
                            drNew["DED_CURRENT1"] = dr["ftm_deduction"];
                            drNew["DED_YTD1"] = dr["ytd_deduction"];
                            break;
                        case 2:
                            drNew["DED_TYPE2"] = dr["item_type_description"];
                            drNew["DED_CURRENT2"] = dr["ftm_deduction"];
                            drNew["DED_YTD2"] = dr["ytd_deduction"];
                            break;
                        case 3:
                            drNew["DED_TYPE3"] = dr["item_type_description"];
                            drNew["DED_CURRENT3"] = dr["ftm_deduction"];
                            drNew["DED_YTD3"] = dr["ytd_deduction"];
                            break;
                        case 4:
                            drNew["DED_TYPE4"] = dr["item_type_description"];
                            drNew["DED_CURRENT4"] = dr["ftm_deduction"];
                            drNew["DED_YTD4"] = dr["ytd_deduction"];
                            break;
                        case 5:
                            drNew["DED_TYPE5"] = dr["item_type_description"];
                            drNew["DED_CURRENT5"] = dr["ftm_deduction"];
                            drNew["DED_YTD5"] = dr["ytd_deduction"];
                            break;
                        case 6:
                            drNew["DED_TYPE6"] = dr["item_type_description"];
                            drNew["DED_CURRENT6"] = dr["ftm_deduction"];
                            drNew["DED_YTD6"] = dr["ytd_deduction"];
                            break;
                        case 7:
                            drNew["DED_TYPE7"] = dr["item_type_description"];
                            drNew["DED_CURRENT7"] = dr["ftm_deduction"];
                            drNew["DED_YTD7"] = dr["ytd_deduction"];
                            break;
                        case 8:
                            drNew["DED_TYPE8"] = dr["item_type_description"];
                            drNew["DED_CURRENT8"] = dr["ftm_deduction"];
                            drNew["DED_YTD8"] = dr["ytd_deduction"];
                            break;
                        case 9:
                            drNew["DED_TYPE9"] = dr["item_type_description"];
                            drNew["DED_CURRENT9"] = dr["ftm_deduction"];
                            drNew["DED_YTD9"] = dr["ytd_deduction"];
                            break;
                        case 10:
                            drNew["DED_TYPE10"] = dr["item_type_description"];
                            drNew["DED_CURRENT10"] = dr["ftm_deduction"];
                            drNew["DED_YTD10"] = dr["ytd_deduction"];
                            break;
                    }
                }
                lintChkCompNumber++;
            }

            drNew["PAYMENT_HISTORY_HEADER_ID"] = aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id;
            drNew["TOTAL_RHIC_AMOUNT"] = aobjCheckFileData.idecTotalRHICAmount;
            drNew["GROUP_HEALTH_PREMIUM"] = aobjCheckFileData.idecGroupHealthPremium;
            drNew["MEDICARE_PREMIUM"] = aobjCheckFileData.idecMedicarePartDPremium;
            drNew["RHIC_APPLIED"] = aobjCheckFileData.idecRHICApplied;
            drNew["NET_PREMIUM"] = aobjCheckFileData.idecNetPremium;
            drNew["NET_AMT_IN_WORDS"] = busGlobalFunctions.AmountToWords(aobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.net_amount.ToString()).ToUpper();
            drNew["NET_AMT_PADDED"] = aobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.net_amount.ToString("#,##0.00").PadLeft(23, '*');

            //Backlog PIR 938 
            aobjCheckFileData.ibusPaymentHistoryHeader.LoadPlan();
            string lstrBenefitType = (aobjCheckFileData.ibusPaymentHistoryHeader.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance) //PIR 17504 (938 residual changes logged under this PIR)
                ? busConstant.PlanBenefitTypeInsurance : busConstant.PlanBenefitTypeRetirement;
            drNew["CHECK_BOTTOM"] = "C" + aobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_number.ToString().PadLeft(7, '0') + "C A" +
                                    busGlobalFunctions.GetData1ByCodeValue(7005, lstrBenefitType, iobjPassInfo) + "A " +
                                    busGlobalFunctions.GetData2ByCodeValue(7005, lstrBenefitType, iobjPassInfo) + "C";
            idtPaymentDetailsMonthly.Rows.Add(drNew);
        }


        /// <summary>
        /// Method to load RHIC details
        /// </summary>
        /// <param name="aobjCheckFileData">check file business object</param>
        private void LoadRHICDetails(busCheckFileData aobjCheckFileData)
        {
            DataRow ldrGHDV = idtGHDVPersonAccount.AsEnumerable()
                                .Where(o => o.Field<int>("payee_account_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id)
                                .FirstOrDefault();
            aobjCheckFileData.idecTotalRHICAmount = idtRHICDetails.AsEnumerable()
                        .Where(o => o.Field<int>("payee_account_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id)
                        .Select(o => o.Field<decimal>("combined_rhic_amount")).Sum();
            aobjCheckFileData.idecRHICApplied = idtRHICDetails.AsEnumerable()
                       .Where(o => o.Field<int>("payee_account_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id)
                       .Select(o => o.Field<decimal>("total_other_rhic_amount")).Sum();

            if (ldrGHDV != null && ldrGHDV["plan_id"] != DBNull.Value)
            {
                aobjCheckFileData.idecNetPremium = LoadHealthMedicarePremium(ldrGHDV);

                if (Convert.ToInt32(ldrGHDV["plan_id"]) == busConstant.PlanIdGroupHealth)
                {
                    aobjCheckFileData.idecGroupHealthPremium = LoadHealthMedicarePremium(ldrGHDV);
                    aobjCheckFileData.idecNetPremium = aobjCheckFileData.idecGroupHealthPremium - aobjCheckFileData.idecRHICApplied;
                }
                else if (Convert.ToInt32(ldrGHDV["plan_id"]) == busConstant.PlanIdMedicarePartD)
                {
                    aobjCheckFileData.idecMedicarePartDPremium = LoadHealthMedicarePremium(ldrGHDV);
                    aobjCheckFileData.idecNetPremium = aobjCheckFileData.idecMedicarePartDPremium - aobjCheckFileData.idecRHICApplied;
                }
            }
        }


        private decimal LoadHealthMedicarePremium(DataRow adrRow)
        {
            int lintCurrIndex = 0;
            bool lblnErrorFound = false;
            decimal ldecEmprSharePremium = 0.00M, ldecEmprShareFee = 0.00M, ldecEmprShareRHICAmt = 0.00M, ldecEmprShareOtherRHICAmt = 0.00M, ldecEmprShareJSRHICAmt = 0.00M;
            decimal ldecEmpShareBuydown = 0.00M;
            decimal ldecEmpShareMedicarePartD = 0.00M;
            busBase lbusBase = new busBase();
            lintCurrIndex++;
            lblnErrorFound = false;

            var lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lbusPersonAccount.icdoPersonAccount.LoadData(adrRow);

            lbusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusPersonAccount.ibusPerson.icdoPerson.LoadData(adrRow);

            lbusPersonAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lbusPersonAccount.ibusPlan.icdoPlan.LoadData(adrRow);

            lbusPersonAccount.ibusPaymentElection = new busPersonAccountPaymentElection
            {
                icdoPersonAccountPaymentElection =
                    new cdoPersonAccountPaymentElection()
            };
            lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(adrRow);

            string lstrCoverageCode = string.Empty;
            decimal ldecGroupHealthFeeAmt = 0.00M;
            decimal ldecBuydownAmt = 0.00M;
            decimal ldecRHICAmt = 0.00M;
            decimal ldecOthrRHICAmount = 0.00M;
            decimal ldecJSRHICAmount = 0.00M;
            decimal ldecPremiumAmt = 0.00M;
            decimal ldecTotalPremiumAmt = 0.00M;
            decimal ldecProviderPremiumAmt = 0.00M;
            decimal ldecMemberPremiumAmt = 0.00M;
            decimal ldecMedicarePartD = 0.00M;
            int lintGHDVHistoryID = 0;
            string lstrGroupNumber = string.Empty;

            var lobjGhdv = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
            lobjGhdv.icdoPersonAccountGhdv.LoadData(adrRow);
            lobjGhdv.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
            lobjGhdv.ibusPerson = lbusPersonAccount.ibusPerson;
            lobjGhdv.ibusPlan = lbusPersonAccount.ibusPlan;
            lobjGhdv.ibusPaymentElection = lbusPersonAccount.ibusPaymentElection;

            lobjGhdv.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
            lobjGhdv.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
            lobjGhdv.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
            lobjGhdv.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
            lobjGhdv.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
            lobjGhdv.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
            lobjGhdv.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;

            //Loading the History Object                
            if ((idtbGHDVHistory != null) && (idtbGHDVHistory.Rows.Count > 0))
            {
                DataRow[] larrRow = idtbGHDVHistory.FilterTable(busConstant.DataType.Numeric,
                                                                "person_account_ghdv_id",
                                                                lobjGhdv.icdoPersonAccountGhdv.person_account_ghdv_id);

                lobjGhdv.iclbPersonAccountGHDVHistory =
                    lbusBase.GetCollection<busPersonAccountGhdvHistory>(larrRow, "icdoPersonAccountGhdvHistory");
            }

            //Get the GHDV History Object By Billing Month Year
            busPersonAccountGhdvHistory lobjPAGhdvHistory = lobjGhdv.LoadHistoryByDate(idtPaymentDate);
            if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id == 0)
            {
                lblnErrorFound = true;
            }

            if (!lblnErrorFound)
            {

                lobjGhdv = lobjPAGhdvHistory.LoadGHDVObject(lobjGhdv);

                lintGHDVHistoryID = lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                lstrGroupNumber = lobjGhdv.GetGroupNumber();

                if (lobjGhdv.ibusPerson == null)
                    lobjGhdv.LoadPerson();

                if (lobjGhdv.ibusPlan == null)
                    lobjGhdv.LoadPlan();
                //Initialize the Org Object to Avoid the NULL error
                lobjGhdv.InitializeObjects();
                lobjGhdv.idtPlanEffectiveDate = idtPaymentDate;

                lobjGhdv.LoadActiveProviderOrgPlan(idtPaymentDate);

                if (lobjGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    lobjGhdv.LoadRateStructureForUserStructureCode();
                }
                else
                {
                    lobjGhdv.LoadHealthParticipationDate();
                    //To Get the Rate Structure Code (Derived Field)
                    lobjGhdv.LoadRateStructure(idtPaymentDate);
                }

                //Get the Coverage Ref ID
                lobjGhdv.LoadCoverageRefID();

                //Get the Premium Amount
                lobjGhdv.GetMonthlyPremiumAmountByRefID(idtPaymentDate);

                if (lobjGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID == 0)
                {
                    lblnErrorFound = true;
                }
                if (!lblnErrorFound)
                {
                    lstrCoverageCode =
                        GetGroupHealthCoverageCodeDescription(lobjGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID);
                    ldecEmprSharePremium = ldecEmprShareFee = ldecEmprShareRHICAmt = ldecEmprShareOtherRHICAmt = ldecEmprShareJSRHICAmt = 0.0m;
                    ldecEmpShareBuydown = ldecEmpShareMedicarePartD = 0.0m;
                    if (!string.IsNullOrEmpty(lobjGhdv.icdoPersonAccountGhdv.cobra_type_value) &&
                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share > 0 &&
                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share < 100)
                    {
                        ldecEmprSharePremium = Math.Round(lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount *
                                                    lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmprShareFee = Math.Round(lobjGhdv.icdoPersonAccountGhdv.FeeAmount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmpShareBuydown = Math.Round(lobjGhdv.icdoPersonAccountGhdv.BuydownAmount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmpShareMedicarePartD = Math.Round(lobjGhdv.icdoPersonAccountGhdv.MedicarePartDAmount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);//PIR 14271
                        ldecEmprShareRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmprShareOtherRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.other_rhic_amount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmprShareJSRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.js_rhic_amount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                    }
                    ldecPremiumAmt = lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount - ldecEmprSharePremium;
                    ldecGroupHealthFeeAmt = lobjGhdv.icdoPersonAccountGhdv.FeeAmount - ldecEmprShareFee;
                    ldecBuydownAmt = lobjGhdv.icdoPersonAccountGhdv.BuydownAmount - ldecEmpShareBuydown;
                    ldecMedicarePartD = lobjGhdv.icdoPersonAccountGhdv.MedicarePartDAmount - ldecEmpShareMedicarePartD;//PIR 14271

                    ldecRHICAmt = lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount - ldecEmprShareRHICAmt;
                    ldecOthrRHICAmount = lobjGhdv.icdoPersonAccountGhdv.other_rhic_amount - ldecEmprShareOtherRHICAmt;
                    ldecJSRHICAmount = lobjGhdv.icdoPersonAccountGhdv.js_rhic_amount - ldecEmprShareJSRHICAmt;
                    ldecMemberPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecRHICAmt - ldecBuydownAmt + ldecMedicarePartD;//PIR 14271
                    ldecTotalPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecBuydownAmt + ldecMedicarePartD;//PIR 14271
                    ldecProviderPremiumAmt = ldecPremiumAmt;
                }
            }

            return ldecTotalPremiumAmt;
        }


        public string GetGroupHealthCoverageCodeDescription(int AintCoverageRefID)
        {
            if (AintCoverageRefID > 0)
            {
                DataTable ldtbCoverageCode = busNeoSpinBase.Select("cdoIbsHeader.GetCoverageCodeDescription",
                                                                                            new object[1] { AintCoverageRefID });
                if (ldtbCoverageCode.Rows.Count > 0)
                {
                    string lstrCoverageCodeDescription = ldtbCoverageCode.Rows[0]["CLIENT_DESCRIPTION"].ToString();
                    return lstrCoverageCodeDescription;
                }
            }
            return string.Empty;
        }
        #endregion
    }
}
