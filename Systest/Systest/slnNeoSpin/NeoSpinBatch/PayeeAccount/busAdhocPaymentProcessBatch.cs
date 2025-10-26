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
using System.IO;
using System.Text;
using Sagitec.ExceptionPub;
using System.Linq;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
#endregion

namespace NeoSpinBatch
{
    public class busAdhocPaymentProcessBatch : busNeoSpinBatch
    {
        public DataTable idtExcessContributionPaymentDetails { get; set; }
        List<string> llstGeneratedCorrespondence = new List<string>();
        List<string> llstGeneratedReports = new List<string>();
        List<string> llstGeneratedFiles = new List<string>();
        string lstrReportPrefixPaymentScheduleID = string.Empty;
        int lintNoOfChecksNeeded = 0;
        busBase lobjBase = new busBase();
        busNeoSpinBase lbusBase = new busNeoSpinBase();
        busPaymentProcess lobjPaymentProcess = new busPaymentProcess();
        //Load the Next valid Adhoc payment schedule into the property ibusPaymentSchedule
        public busPaymentSchedule ibusPaymentSchedule { get; set; }
        public Collection<busPaymentScheduleStep> iclbProcessedSteps { get; set; }

        //Load Next Benefit Payment Date        
        public DateTime idtNextBenefitPaymentDate { get; set; }

        public void LoadNextBenefitPaymentDate()
        {
            idtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
        }
        public void LoadPaymentSchedule()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNextBenefitPaymentDate();
            if (ibusPaymentSchedule == null) ibusPaymentSchedule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
            DataTable ldtbPaymentSchedule = busBase.SelectWithOperator<cdoPaymentSchedule>
                                   (new string[4] { "schedule_type_value", "status_value", "action_status_value", "payment_date" },
                                      new string[4] { "=", "=", "=", "<=" },
                                      new object[4] {busConstant.PaymentScheduleScheduleTypeAdhoc,busConstant.PaymentScheduleStatusValid,
                                                     busConstant.PaymentScheduleActionStatusReadyforFinal,iobjSystemManagement.icdoSystemManagement.batch_date}, null);
            if (ldtbPaymentSchedule.Rows.Count > 0)
                ibusPaymentSchedule.icdoPaymentSchedule.LoadData(ldtbPaymentSchedule.Rows[0]);
        }
        //Method to be called from the batch
        public void ProcessPayments()
        {
            istrProcessName = "Adhoc Payment Batch";

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

                //Load Payment Batch schedule steps
                if (ibusPaymentSchedule != null)
                    if (lobjPaymentProcess.iclbBatchScheduleSteps == null)
                        lobjPaymentProcess.LoadBatchScheduleSteps(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                try
                {
                    iobjPassInfo.BeginTransaction();
                    //Loop through the steps and process payments
                    if (lobjPaymentProcess.iclbBatchScheduleSteps != null)
                    {
                        foreach (busPaymentScheduleStep lobjPaymentScheduleStep in lobjPaymentProcess.iclbBatchScheduleSteps)
                        {
                            if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1710 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1800)
                            {
                                // Load All the payee accounts to be considered for this payment process
                                if (StartPaymentProcess(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
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
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1800 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 1900)
                            {

                                // Load All the payee accounts to be considered for this payment process
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
                            }
                            else if (lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence >= 1900 && lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence < 2000)
                            {
                                //Call the methods which are related to updating payee account related tables
                                if (UpdateStatusAdhoc(lobjPaymentScheduleStep.icdoPaymentScheduleStep.run_sequence) < 0)
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
                            }
                        }
                        //To create reports which doesnt have individual steps                       
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
                    }
                    else
                    {
                        iobjPassInfo.Commit();
                        llstGeneratedFiles.Clear();
                        llstGeneratedReports.Clear();
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    iobjPassInfo.Rollback();
                    DeleteGeneratedReports();
                    llstGeneratedReports.Clear();
                    idlgUpdateProcessLog("Error Occured with Message = " + e.Message, "INFO", istrProcessName);
                }
                try
                {
                    iobjPassInfo.BeginTransaction();
                    //Update Payment shedule status                    
                    foreach (busPaymentScheduleStep lobjScheduleStep in lobjPaymentProcess.iclbBatchScheduleSteps)
                    {
                        if (lobjScheduleStep.iblnStepFailedIndicator)
                        {
                            lblnStepFailedIndicator = true;
                        }
                        lobjScheduleStep.icdoPaymentScheduleStep.Update();
                    }
                    if (lblnStepFailedIndicator)
                        ibusPaymentSchedule.icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusFailed;
                    else
                    {
                        ibusPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleStatusProcessed;
                        ibusPaymentSchedule.icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusProcessed;
                        //to delete back up tables
                        DeleteBackUpTables();
                    }
                    //System.Threading.Thread.Sleep(1000 * 60);
                    ibusPaymentSchedule.icdoPaymentSchedule.process_end_time = DateTime.Now;
                    ibusPaymentSchedule.icdoPaymentSchedule.Update();
                    iobjPassInfo.Commit();
                    idlgUpdateProcessLog("Adhoc Payment Process Successful", "INFO", istrProcessName);
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    iobjPassInfo.Rollback();                  
                    idlgUpdateProcessLog("Error Occured in Updating Schedule Status", "INFO", istrProcessName);
                }
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

            string lstrDCFileOutPath = iobjPassInfo.isrvDBCache.GetPathInfo("DCOut");
            string lstrDCFileProcessedPath = iobjPassInfo.isrvDBCache.GetPathInfo("DCProc");

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
                    else if (lobjFileHdr.icdoFileHdr.file_id == busConstant.DCFileID)
                    {
                        string lstrOutFileName = lstrDCFileOutPath + lobjFileHdr.icdoFileHdr.mailbox_file_name;
                        llstGeneratedFiles.Add(lstrOutFileName);
                        string lstrProcessedFileName = lstrDCFileProcessedPath + lobjFileHdr.icdoFileHdr.processed_file_name;
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
            DataTable ldtbFiles = busBase.Select("cdoFileHdr.LoadDCTransferFileInfo",
                            new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.process_start_time });
            iclbFileHdr = lobjBase.GetCollection<busFileHdr>(ldtbFiles, "icdoFileHdr");
        }
        private int StartPaymentProcess(int aintRunSequence)
        {
            string lstrReportPath = string.Empty;
            int lintrtn = 0;
            busCreateReports lobjCreateReports = new busCreateReports();
            DataTable ldtReportResult;
            switch (aintRunSequence)
            {
                case 1720:
                    idlgUpdateProcessLog("Start Payment Process ", "INFO", istrProcessName);
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
                case 1740:
                    //Calculate interest fot DB to Dc transfer options and create payment items into payee account payment item table
                    try
                    {
                        idlgUpdateProcessLog("Update Payment Date and Interest for ‘DB to DC Transfer’", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CalculateInterestAdhoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn == -1) ? "Updating Payment Date and Interest for DB to DC Transfer Failed." : "Updating Payment Date and Interest for DB to DC Transfer Processed . (Records Created " + lintrtn + ") ", "INFO", istrProcessName);

                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating Payment Date and Interest for DB to DC Transfer Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1760:
                    try
                    {                        
                        idlgUpdateProcessLog("Monthly Benefit Payment by Item Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentbyItemReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date, false);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptMonthlyBenefitPaymentbyItem.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            //PIR-10808 PDF Reports converted into Excel format                                                      
                            lbusBase.CreateExcelReport("rptMonthlyBenefitPaymentbyItem.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("Monthly Benefit Payment by Item Report generated succesfully", "INFO", istrProcessName);
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
                case 1770:
                    try
                    {
                        idlgUpdateProcessLog("Vendor Payment Trial Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialVendorPaymentSummaryAdHoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptVendorPaymentSummary.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptVendorPaymentSummary.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("Vendor Payment Trial Summary Report generated succesfully", "INFO", istrProcessName);
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
                        idlgUpdateProcessLog("Vendor Payment Trial Summary Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1790:
                    try
                    {
                        idlgUpdateProcessLog("Payee List Report", "INFO", istrProcessName);                       
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TrialNonMonthlyPaymentDetailReportAdHoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptNonMonthlyPaymentDetail.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            //PIR-10808 PDF Reports converted into new Excel format                
                              lbusBase.CreateExcelReport("rptNonMonthlyPaymentDetailExcel.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            lintrtn = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Payee List Report Failed.", "INFO", istrProcessName);
                        return -1;
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
            //// BR-125 The system must generate ‘Insurance Refund’ letter for the person in the Adhoc process for ‘Return Excess Contribution’.            

            foreach (DataRow ldtrPayment in idtExcessContributionPaymentDetails.Rows)
            {
                busPaymentHistoryDistribution lobjPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
                lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
                lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.LoadData(ldtrPayment);
                lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(ldtrPayment);
                lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPlan.icdoPlan.LoadData(ldtrPayment);
                if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.org_id > 0)
                {
                    llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPaymentHistoryDistribution, "PAY-4304"));
                }
                else
                {
                    llstGeneratedCorrespondence.Add(CreateCorrespondence(lobjPaymentHistoryDistribution, "PAY-4309"));
                }
            }

        }
        private string CreateCorrespondence(busPaymentHistoryDistribution aobjPaymentHistoryDistribution, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjPaymentHistoryDistribution);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            return CreateCorrespondence(astrCorName, aobjPaymentHistoryDistribution, lhstDummyTable);
        }
        //steps to create payment history    
        public int CreatePaymentHistory(int aintRunSequence)
        {
            int lintrtn = 0;
            switch (aintRunSequence)
            {
                case 1810:
                    try
                    {
                        //Create Payment History Header for all the payee accounts considered for this adhoc payment process
                        idlgUpdateProcessLog("Creating Payment History Headers", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateAdhocPaymentHistoryHeader(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        lintNoOfChecksNeeded += lintrtn;
                        idlgUpdateProcessLog((lintrtn == -1) ? "Creation of Payment History Header Failed." : "Payment History Headers Created. (Records Created " + lintrtn + ") ", "INFO", istrProcessName);
                        //Create Reissue Rollover Payments Header
                        idlgUpdateProcessLog("Creating Reissue Rollover Payments", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateReissueRolloverPaymentsHeader(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        lintNoOfChecksNeeded += lintrtn;
                        idlgUpdateProcessLog((lintrtn == -1) ? "Creation of Reissue Rollover Failed." : "Reissue Rollover Payment History Headers Created. (Records Created " + lintrtn + ") ", "INFO", istrProcessName);
                        idlgUpdateProcessLog("Creating Escheat Reissue Payment Headers", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateEscheatReissuepaymentsHeader(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        lintNoOfChecksNeeded += lintrtn;
                        idlgUpdateProcessLog((lintrtn == -1) ? "Creation of Escheat Reissue Payment History Header Failed." : "Escheat Reissue Payment History Headers Created. (Records Created " + lintrtn + ") ", "INFO", istrProcessName);
                        idlgUpdateProcessLog("Creating Escheat To State Payment Headers", "INFO", istrProcessName);
						//PIR 16219 - Escheat To State Payment Headers
                        lintrtn = lobjPaymentProcess.CreateEscheatToStatepaymentsHeader(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        lintNoOfChecksNeeded += lintrtn;
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Escheat To State Headers Failed." : "Escheat To State Payments Headers Created. (Records Created " + lintrtn + ") ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment History Headers Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    //Create Payment History details for all the payee accounts considered for this adchoc payment process
                    try
                    {
                        idlgUpdateProcessLog("Creating Payment History details", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateAdhocPaymentHistoryDetail(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn == -1) ? "Creation of Payment History Details Failed." : "Payment History Details created. (Records Created " + lintrtn + ") ", "INFO", istrProcessName);

                        //Create Reissue Rollover Payment Details
                        idlgUpdateProcessLog("Create Reissue Rollover Payments", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateReissueRolloverPaymentDetails(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);

                        lintrtn = lobjPaymentProcess.CreateEscheatReissueRolloverPaymentDetails(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
						//PIR 16219 - Escheat To State Payment Details
                        lintrtn = lobjPaymentProcess.CreateEscheatToStatePaymentDetails(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                      
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Reissue Rollover Payments Failed." : "Reissue Rollover Payments Details Created. ", "INFO", istrProcessName);
                                                
                        //lobjPaymentProcess.CalculateTaxForRolloverAmountToPayee(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment History Details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        idlgUpdateProcessLog("Update Adjustments with Payment History information", "INFO", istrProcessName);
                        //Updating Retro table with Payment History ID
                        lintrtn = lobjPaymentProcess.UpdateAdjustmentsWithPaymentHistorySpecial(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        //Updating Deduction table with Payment History ID
                        lintrtn = lobjPaymentProcess.UpdateDeductionsWithPaymentHistorySpecial(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn == -1) ? "Updating Adjustments with Payment History information Failed." : "Updating Adjustments with Payment History information processed. (Records processed " + lintrtn + ") ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating Adjustments with Payment History information Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1820:
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
                        idlgUpdateProcessLog("Creation of Payment History Distribution Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Create Payment Check History details for all the payee accounts considered for this adhoc payment process
                        idlgUpdateProcessLog("Creating Check History for Payees", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateCheckHistoryforPayees(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, idtNextBenefitPaymentDate,iobjBatchSchedule.batch_schedule_id);
						//PIR 16219 - Escheat To State Payment History Distribution
                        lintrtn = lobjPaymentProcess.CreateCheckHistoryforEscheatToState(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        //Update FBO CO
                        lobjPaymentProcess.UpdateFBOCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);

                        idlgUpdateProcessLog((lintrtn == -1) ? "Creation of Payment Check History details Failed." : "Payment Check History details Created. (Records Created " + lintrtn + ") ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Payment Check History details Failed.", "INFO", istrProcessName);
                        return -1;
                    }

                    try
                    {
                        //Create ACH Check History for all the payee accounts considered for this adhoc payment process
                        idlgUpdateProcessLog("Creating ACH History for Payees", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateACHHistoryforPayees(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, idtNextBenefitPaymentDate, iobjBatchSchedule.batch_schedule_id);

                        //Update FBO CO
                        lobjPaymentProcess.UpdateFBOCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);


                        idlgUpdateProcessLog((lintrtn == -1) ? "Creation of ACH History details Failed." : "ACH Payment History details Created. ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of ACH/Check History details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Create Rollover Check History for all the payee accounts considered for this adhoc payment process
                        idlgUpdateProcessLog("Creating Check History Rollover", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateRollOverCheckACHHistoryforPayees(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);

                        //Update FBO CO
                        lobjPaymentProcess.UpdateFBOCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);


                        idlgUpdateProcessLog((lintrtn == -1) ? "Creation of Rollover Check History details Failed." : "Rollover Check History details Created.", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Rollover Check History details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        //Create Check History for all the persons or oranizations approved for deposit refund
                        idlgUpdateProcessLog("Creating Check History for Deposit Refund", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateChkACHHistoryforExcessContrReturnAdhoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        
                        //Update FBO CO
                        lobjPaymentProcess.UpdateFBOCO(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);

                        idlgUpdateProcessLog((lintrtn == -1) ? "Creation of Check History for Deposit Refund details Failed." : "Deposit Refund Check History details Created. ", "INFO", istrProcessName);

                        //Update Old Distribution Id For OrgID
                        lintrtn = lobjPaymentProcess.UpdateOldDistributionId(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);

                        //Create Reissue Rollover Payment Details
                        idlgUpdateProcessLog("Create Vendor  Receivables", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateVendorReceivables(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Create Vendor  Receivables Failed." : "Create Vendor  Receivables Details Created. ", "INFO", istrProcessName);


                        //Update Distribution From Reissue Approve To Reissued
                        lintrtn = lobjPaymentProcess.UpdateDistributionFromReissueApproveToReissued(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);

                        lintrtn = lobjPaymentProcess.UpdateDistributionFromEscheatApproveToEscheatReissue(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
						//PIR 16219 - Updating the distribution records from Escheat to state to Payment issued to state
                        lintrtn = lobjPaymentProcess.UpdateDistributionFromEscheatToStateToPaymentIssued(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id); //PIR 16219

                        //Update Recipient Name For Death Issue
                        lintrtn = lobjPaymentProcess.UpdateRecipientNameForDeathIssue(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);

                        lintrtn = lobjPaymentProcess.CreateOutstandingHistoryRecords(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of Check History for Deposit Refund details Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1840:
                    try
                    {
                        //CreatingGL
                        idlgUpdateProcessLog("Create GL", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateGLAdHoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                                                                ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                                                                busConstant.GLSourceTypeValueBenefitPayment,iobjBatchSchedule.batch_schedule_id);
                        lintrtn = lobjPaymentProcess.CreateGLAdHoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                                                                ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                                                                busConstant.GLSourceTypeValueInsuranceTransfer,iobjBatchSchedule.batch_schedule_id);
						//PIR 16219 - GL creation for escheat to state, debit using personid/orgid and credit using org_id(6083)										
                        lintrtn = lobjPaymentProcess.CreateGLEscheatToState(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                                                                busConstant.GLSourceTypeValueBenefitPayment,iobjBatchSchedule.batch_schedule_id);
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

        //Update payee account status and person account status
        public int UpdateStatusAdhoc(int aintRunSequence)
        {
            string lstrReportPath = string.Empty;
            int lintrtn = 0;
            busCreateReports lobjCreateReports = new busCreateReports();
            DataTable ldtReportResult;
            switch (aintRunSequence)
            {
                case 1900:
                    try
                    {
                        //Create Check File
                        idlgUpdateProcessLog("Create Check Files", "INFO", istrProcessName);
                        DataTable ldtCheckFile = busBase.Select("cdoPaymentHistoryDistribution.LoadCheckPaymentDistribution", new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                        //PIR-25920 insert data into Report data DC and used for vendor payment instead of check file 
						lobjPaymentProcess.InsertIntoReportDataDCInDBDCSPEL(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);

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
                case 1910:
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
                case 1915:
                    try
                    {
                        lintrtn = CreatePremiumPaymentsForIBS(aintRunSequence);
                    }
                    catch (Exception e)
                    {
                        return -1;
                    }
                    break;
                case 1920:
                    //update the plan participation status to withdrawn if the benfit type is 'REFUND'
                    try
                    {                        

                        idlgUpdateProcessLog("Updating Person account Information", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdatePersonAccountStatusAdhoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Person account Information Failed." : "Updating Person account Information Successful .  ", "INFO", istrProcessName);

                        //Reduce the transferred amount from the member contributions for the member payee account   for all transfers
                        //For DB to DC transfer ,transfer the whole amount to DC plan account
                        lintrtn = lobjPaymentProcess.UpdateRetirementContributionForAdhoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Person account Information Failed." : "Updating Person account retirement contribution for member  Successful .  ", "INFO", istrProcessName);

                        lintrtn = lobjPaymentProcess.UpdatePersonAccountBeneficiaryAdhoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Person account Information Failed." : "Updating Person account retirement contribution for member  Successful .  ", "INFO", istrProcessName);

                       
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating Person account Information Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;                
                case 1930:
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
                            //PIR-10808 PDF Reports converted into Excel format                                                      
                            lbusBase.CreateExcelReport("rptMonthlyBenefitPaymentbyItemFinal.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("Final Monthly Benefit Payment by Item Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
						//PIR-25920 Create a new report 
                        idlgUpdateProcessLog("Empower Special Election Transfer Detail Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.LoadEmpowerSpecialElectionPaymentsReport(ibusPaymentSchedule.icdoPaymentSchedule.effective_date);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptEmpowerSpecialElectionTransferDetail.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            //PIR-25920 PDF Reports converted into Excel format                                                      
                            lbusBase.CreateExcelReport("rptEmpowerSpecialElectionTransferDetail.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID, busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("Empower Special Election Transfer Detail Report generated succesfully", "INFO", istrProcessName);
                            lintrtn = 1;
                        }                        
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Final Monthly Benefit Payment by Item Report Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1950:
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
                            //PIR-10808 PDF Reports converted into Excel format                                                       
                            lbusBase.CreateExcelReport("rptMasterPayment.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("Master Payment Report generated succesfully", "INFO", istrProcessName);
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
                    try
                    {
                        idlgUpdateProcessLog("ACH Register Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalACHRegisterReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = busConstant.ReportTableName;
                        //prod pir 5391 : new table to show payee with multiple ACH
                        DataTable ldtMultipleACH = new DataTable();
                        ldtMultipleACH = lobjCreateReports.MultipleACHOrCheckReport(2, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtMultipleACH.TableName = busConstant.ReportTableName02;
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            DataSet ldsReportResult = new DataSet();
                            ldsReportResult.Tables.Add(ldtReportResult.Copy());
                            ldsReportResult.Tables.Add(ldtMultipleACH.Copy());
                            lstrReportPath = CreateReportWithPrefix("rptACHRegister.rpt", ldsReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            //PIR-10808 PDF Reports converted into new Excel format                                                                                
                            lbusBase.CreateExcelReport("rptACHRegisterExcel.rpt", ldsReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("ACH Register Report generated succesfully", "INFO", istrProcessName);
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
                        //PIR 24886 - Show Payees with multiple payments
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
                        idlgUpdateProcessLog("Check Register Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalCheckRegisterReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtReportResult.TableName = busConstant.ReportTableName;
                        //prod pir 5391 : new table to show payee with multiple CHK
                        DataTable ldtMultipleCHK = new DataTable();
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
                            llstGeneratedReports.Add(lstrReportPath);
                            //PIR-10808 PDF Reports converted into new Excel format                                                        
                            lbusBase.CreateExcelReport("rptCheckRegisterExcel.rpt", ldsReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("Check Register Report generated succesfully", "INFO", istrProcessName);
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
                        idlgUpdateProcessLog("Child Support Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalChildSupportReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptChildSupport.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            //PIR-10808 PDF Reports converted into Excel format                                                       
                            lbusBase.CreateExcelReport("rptChildSupport.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("Child Support Report generated succesfully", "INFO", istrProcessName);
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
                        idlgUpdateProcessLog("TFFR Transfer Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TFFRorTIAACREFReport(busConstant.Flag_Yes, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        DataTable ldtTFFRResult = AddReportData(ldtReportResult,busConstant.Flag_Yes);
                        DataTable ldtTFFRSalaryRecords = lobjCreateReports.TFFRSalaryRecords(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        ldtTFFRSalaryRecords.TableName = busConstant.ReportTableName02;
                        DataSet ldsFinalTFFR = new DataSet();
                        ldsFinalTFFR.Tables.Add(ldtTFFRResult.Copy());
                        ldsFinalTFFR.Tables.Add(ldtTFFRSalaryRecords.Copy());
                        ldsFinalTFFR.AcceptChanges();
                        if (ldtTFFRResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptTFFRTransfer.rpt", ldsFinalTFFR, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptTFFRTransfer.rpt", ldsFinalTFFR, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("TFFR Transfer Report generated succesfully", "INFO", istrProcessName);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("TFFR Transfer Report Failed.", "INFO", istrProcessName);
                    }
                    try
                    {
                        idlgUpdateProcessLog("TIAA-CREF Transfer Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.TFFRorTIAACREFReport(busConstant.Flag_No, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        DataTable ldtTIAACREFResult = AddReportData(ldtReportResult, busConstant.Flag_No);
                        if (ldtTIAACREFResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptTIAACREFTransfer.rpt", ldtTIAACREFResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            //PIR-10808 PDF Reports converted into Excel format                                                      
                            lbusBase.CreateExcelReport("rptTIAACREFTransfer.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("TIAA-CREF Transfer Report generated succesfully", "INFO", istrProcessName);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("TIAA-CREF Transfer Report Failed.", "INFO", istrProcessName);
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
                case 1970:
                    try
                    {
                        //Create Vendor Payment Summary
                        idlgUpdateProcessLog("Create Vendor Payment Summary", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.CreateVendorPaymentsForAdhoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Creation of Vendor Payment Summary Failed." : "Vendor Payment Summary Created. ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Create Vendor Payment Summary Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    try
                    {
                        idlgUpdateProcessLog("Vendor Payment Final Summary Report", "INFO", istrProcessName);
                        ldtReportResult = new DataTable();
                        ldtReportResult = lobjCreateReports.FinalVendorPaymentSummary(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                        if (ldtReportResult.Rows.Count > 0)
                        {
                            lstrReportPath = CreateReportWithPrefix("rptVendorPaymentSummary.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "FINAL_", busConstant.PaymentReportPath);
                            llstGeneratedReports.Add(lstrReportPath);
                            //PIR-10808 PDF Reports converted into Excel format                                                        
                            lbusBase.CreateExcelReport("rptVendorPaymentSummary.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
                            idlgUpdateProcessLog("Vendor Payment Final Summary Report generated succesfully", "INFO", istrProcessName);
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Vendor Payment Final Summary Report Failed.", "INFO", istrProcessName);
                    }
                    break;
                case 1980:
                    try
                    {
                        ////update end date for deduction refund for all the payee accounts considered for  adhoc payment process
                        //idlgUpdateProcessLog("Update end date for deduction refund ", "INFO", istrProcessName);
                        //lintrtn = lobjPaymentProcess.UpdateDeductionRefundEndDateForSpecialPayment(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date);
                        //idlgUpdateProcessLog((lintrtn == -1) ? "Updating end date for deduction refund Failed." : "Updating end date for deduction refund processed.", "INFO", istrProcessName);

                        idtExcessContributionPaymentDetails = busNeoSpinBase.Select("cdoPaymentHistoryHeader.LoadExcessContributionPayments", new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });

                        //Update Deposit Refund To Processed for Adhoc payment process
                        idlgUpdateProcessLog("Update Deposit Refund To Processed ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateDepositRefundStatus(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn == -1) ? "Updating Deposit Refund To Processed Failed." : "Updating Deposit Refund To Processed Successful.", "INFO", istrProcessName);


                        //update payee account status  to processed for All the payee accounts considered for  adhoc payment process
                        idlgUpdateProcessLog("Update payee account status  to processed", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdatePayeeAccountStatusAdhoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn == -1) ? "Updating payee account status Failed." : "Updating payee account status processed Successful. ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Updating Rollover Detail status to processed ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateRolloverDetailStatusToProcessed(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Rollover Detail status failed." : "Updating Rollover Detail status Successful . ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Update refund payee account items wth end date", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateRefundItemsAdhoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                            ibusPaymentSchedule.icdoPaymentSchedule.payment_date, iobjBatchSchedule.batch_schedule_id);


                        idlgUpdateProcessLog("Updating Benefit Application Status to Processed ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateBenefitApplication(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Benefit Application Status Failed." : "Updating Benefit Application Status Successful . ", "INFO", istrProcessName);

                        idlgUpdateProcessLog("Updating Benefit Calculation Status to Processed", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateBenefitCalculations(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Benefit Calculation Status Failed." : "Updating Benefit Calculation Status Successful . ", "INFO", istrProcessName);


                        idlgUpdateProcessLog("Updating Benefit End date for Transfer Payee Accounts ", "INFO", istrProcessName);
                        lintrtn = lobjPaymentProcess.UpdateBenefitEndDateFromAdhoc(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);
                        idlgUpdateProcessLog((lintrtn < 0) ? "Updating Benefit End date for Transfer Payee Accounts failed." :
                            "Updating Benefit End date for Transfer Payee Accounts Successful . ", "INFO", istrProcessName);

                       
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Updating payee account status Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1960:
                    try
                    {
                        //Create DC Transfer File
                        idlgUpdateProcessLog("Create DC Transfer File", "INFO", istrProcessName);
                        DataTable ldtDCTransfer = busBase.Select("cdoPaymentHistoryDetail.LoadDCTransferRecords", new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                        if (ldtDCTransfer.Rows.Count > 0)
                        {
                            busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                            lobjProcessFiles.iarrParameters = new object[2];
                            lobjProcessFiles.iarrParameters[0] = true;
                            lobjProcessFiles.iarrParameters[1] = ldtDCTransfer;
                            lobjProcessFiles.CreateOutboundFile(67);
                            idlgUpdateProcessLog("DC Transfer File created successfully", "INFO", istrProcessName);
                        }
                        else
                            idlgUpdateProcessLog("No records exist", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creation of DC Transfer File Failed.", "INFO", istrProcessName);
                        return -1;
                    }
                    break;
                case 1990:
                    //Generate Correspondence
                    idlgUpdateProcessLog("Generate Correspondence ", "INFO", istrProcessName);
                    try
                    {
                   // GenerateCorrespondence();
                    idlgUpdateProcessLog("Correspondence Generated Successfully", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Correspondence Generation Failed.", "INFO", istrProcessName);
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
                
            }
            return lintrtn;
        }
        /// <summary>
        /// Method to add TFFR or TIAA - CREF Report Data into DataTable
        /// </summary>
        /// <param name="ldtReportResult">Report Result from SQL Query</param>
        /// <returns>Loaded datatable to be binded to report</returns>
        private DataTable AddReportData(DataTable ldtReportResult,string astrFlag)
        {
            DataTable ldtResult = new DataTable();
            ldtResult = CreateNewDataTable();
            foreach (DataRow ldrResult in ldtReportResult.Rows)
            {
                DataRow dr = ldtResult.NewRow();

                dr["PERSLinkID"] = ldrResult["PERSLinkID"];
                dr["SSN"] = ldrResult["SSN"];
                dr["LastName"] = ldrResult["LastName"];
                dr["FirstName"] = ldrResult["FirstName"];
                dr["EEPreTaxAmount"] = ldrResult["EEPreTaxAmount"];
                dr["EEPostTaxAmount"] = ldrResult["EEPostTaxAmount"];
                dr["EEEmpPickupAmount"] = ldrResult["EEEmpPickupAmount"];
                dr["EEInterestAmount"] = ldrResult["EEInterestAmount"];
                dr["ERPreTaxAmount"] = ldrResult["ERPreTaxAmount"];
                dr["ERInterestAmount"] = ldrResult["ERInterestAmount"];

                busBenefitRefundCalculation lobjBenefitRefundCalculation = new busBenefitRefundCalculation();
                lobjBenefitRefundCalculation.FindBenefitRefundCalculation(Convert.ToInt32(ldrResult["benefit_calculation_id"]));                
                lobjBenefitRefundCalculation.CalculateTotalAmountForTransferOptions();
                dr["TotalTransferAmount"] = lobjBenefitRefundCalculation.icdoBenefitRefundCalculation.total_transfer_amount;

                if (astrFlag == busConstant.Flag_Yes)
                {
                    busPersonAccount lobjPersonAccount = new busPersonAccount();
                    lobjPersonAccount.FindPersonAccount(Convert.ToInt32(ldrResult["person_account_id"]));
                    lobjPersonAccount.LoadTotalPSC();
                    dr["PSC"] = lobjPersonAccount.idecTotalPSC_Rounded;

                    busRetirementBenefitCalculation lobjRetirementBenefitCalculation = new busRetirementBenefitCalculation();
                    lobjRetirementBenefitCalculation.FindBenefitCalculation(Convert.ToInt32(ldrResult["benefit_calculation_id"]));
                    //Setting termination date and retirment date to max value, to load last contribution date
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.termination_date =
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.MaxValue;
                    //loading the last contribution record (min of last salary record or termination date)
                    lobjRetirementBenefitCalculation.LoadLastContributedDate();
                    //setting the retirment date and termination date to last contribution date
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date =
                        lobjRetirementBenefitCalculation.icdoBenefitCalculation.termination_date =
                        lobjRetirementBenefitCalculation.idteLastContributedDate;
                    //setting benefit acct type to Retirement
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                    //setting calculation type to Final
                    lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
                    //Calculating FAS
                    lobjRetirementBenefitCalculation.CalculateFAS();
                    dr["CurrentFAS"] = lobjRetirementBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary;
                    
                    dr["PlanParticipationStartDate"] = ldrResult["PlanParticipationStartDate"];
                    dr["NumberofMissedMonths"] = ldrResult["NumberofMissedMonths"];
                    dr["EmploymentStartDate"] = ldrResult["EmploymentStartDate"];
                    dr["EmploymentEndDate"] = ldrResult["EmploymentEndDate"];
                    dr["OrgCodeID_OrgName"] = ldrResult["OrgCodeID_OrgName"];                    
                }
                dr["Indicator"] = ldrResult["Indicator"];
                ldtResult.Rows.Add(dr);
            }
            ldtResult.AcceptChanges();
            return ldtResult;
        }

        /// <summary>
        /// Method to create a new empty data table with same structure as that binded to TFFR or TIAA - CREF report
        /// </summary>
        /// <returns>New Datatable</returns>
        public DataTable CreateNewDataTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("SSN", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("LastName", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("FirstName", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("EEPreTaxAmount", Type.GetType("System.Decimal"));
            DataColumn ldc6 = new DataColumn("EEPostTaxAmount", Type.GetType("System.Decimal"));
            DataColumn ldc7 = new DataColumn("EEEmpPickupAmount", Type.GetType("System.Decimal"));
            DataColumn ldc8 = new DataColumn("EEInterestAmount", Type.GetType("System.Decimal"));
            DataColumn ldc9 = new DataColumn("ERPreTaxAmount", Type.GetType("System.Decimal"));
            DataColumn ldc10 = new DataColumn("ERInterestAmount", Type.GetType("System.Decimal"));
            DataColumn ldc11 = new DataColumn("TotalTransferAmount", Type.GetType("System.Decimal"));
            DataColumn ldc12 = new DataColumn("PSC", Type.GetType("System.Decimal"));
            DataColumn ldc13 = new DataColumn("CurrentFAS", Type.GetType("System.Decimal"));
            DataColumn ldc14 = new DataColumn("PlanParticipationStartDate", Type.GetType("System.DateTime"));
            DataColumn ldc15 = new DataColumn("NumberofMissedMonths", Type.GetType("System.Int32"));
            DataColumn ldc16 = new DataColumn("EmploymentStartDate", Type.GetType("System.DateTime"));
            DataColumn ldc17 = new DataColumn("EmploymentEndDate", Type.GetType("System.DateTime"));
            DataColumn ldc18 = new DataColumn("OrgCodeID_OrgName", Type.GetType("System.String"));            
            DataColumn ldc21 = new DataColumn("Indicator", Type.GetType("System.String"));
            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);
            ldtbReportTable.Columns.Add(ldc7);
            ldtbReportTable.Columns.Add(ldc8);
            ldtbReportTable.Columns.Add(ldc9);
            ldtbReportTable.Columns.Add(ldc10);
            ldtbReportTable.Columns.Add(ldc11);
            ldtbReportTable.Columns.Add(ldc12);
            ldtbReportTable.Columns.Add(ldc13);
            ldtbReportTable.Columns.Add(ldc14);
            ldtbReportTable.Columns.Add(ldc15);
            ldtbReportTable.Columns.Add(ldc16);
            ldtbReportTable.Columns.Add(ldc17);
            ldtbReportTable.Columns.Add(ldc18);           
            ldtbReportTable.Columns.Add(ldc21);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        //Premium Payments for IBS
        public int CreatePremiumPaymentsForIBS(int aintRunSequence)
        {
            int lintrtn = 0;
            try
            {
                idlgUpdateProcessLog("Premium Payments for IBS", "INFO", istrProcessName);

                lintrtn = lobjPaymentProcess.CreatePremiumPaymentsForIBS(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                                                                    ibusPaymentSchedule.icdoPaymentSchedule.payment_date,iobjBatchSchedule.batch_schedule_id);

                idlgUpdateProcessLog(lintrtn >= 0 ? "Premium Payments for IBS Created. " : "Creation of Premium Payments for IBS Failed.", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Creation of Premium Payments for IBS Failed.", "INFO", istrProcessName);
                return -1;
            }          
            return lintrtn;
        }

        private void DeleteBackUpTables()
        {
            try
            {
                int lintrtn = 0;
                idlgUpdateProcessLog("Delete back-up data", "INFO", istrProcessName);
                lintrtn = DBFunction.DBNonQuery("cdoPayeeAccount.DeleteBackUpAdhoc",
                          new object[2] { ibusPaymentSchedule.icdoPaymentSchedule.payment_date, ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //idlgUpdateProcessLog((lintrtn < 0) ? "Delete back-up data failed." : "Delete back-up data - Successful.", "INFO", istrProcessName);
                idlgUpdateProcessLog("Delete back-up data - Successful.", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Delete back-up data Failed.", "INFO", istrProcessName);
            }
        }

        #region PIR 14264

        public DataTable idtPaymentDetailsAdhoc { get; set; }
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

                ldtPaymentDetails = (from ldrPaymentDetails in idtPaymentDetailsAdhoc.AsEnumerable()
                                     where (ldrPaymentDetails.Field<int>("PAYMENT_HISTORY_HEADER_ID") == Convert.ToInt32(ldtbRow["PAYMENT_HISTORY_HEADER_ID"]))
                                     select ldrPaymentDetails).AsDataTable();
                ldtPaymentDetails.TableName = busConstant.ReportTableName02;

                ldsCheckFile.Tables.Add(ldtbData.Copy());
                ldsCheckFile.Tables.Add(ldtPaymentDetails.Copy());

                CreateReportCheck("rptCheckFile.rpt", ldsCheckFile, "Check_" + ldtbRow["CHECK_NUMBER"].ToString(), "PHChkOut");
                //PIR-10808 PDF Reports converted into Excel format                                
                lbusBase.CreateExcelReport("rptCheckFile.rpt", ldsCheckFile, lstrReportPrefixPaymentScheduleID + "_", busConstant.PaymentReportPath);
            }
        }

        public void CreateNewDataTablePayments()
        {
            idtPaymentDetailsAdhoc = new DataTable();

            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("PAYMENT_HISTORY_HEADER_ID", Type.GetType("System.Int32")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TOTAL_RHIC_AMOUNT", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("GROUP_HEALTH_PREMIUM", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("MEDICARE_PREMIUM", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("RHIC_APPLIED", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("NET_PREMIUM", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("NET_AMT_IN_WORDS", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("NET_AMT_PADDED", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CHECK_BOTTOM", Type.GetType("System.String")));

            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TYPE1", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CURRENT1", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("YTD1", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TYPE2", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CURRENT2", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("YTD2", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TYPE3", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CURRENT3", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("YTD3", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TYPE4", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CURRENT4", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("YTD4", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TYPE5", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CURRENT5", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("YTD5", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TYPE6", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CURRENT6", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("YTD6", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TYPE7", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CURRENT7", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("YTD7", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TYPE8", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CURRENT8", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("YTD8", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TYPE9", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CURRENT9", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("YTD9", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("TYPE10", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("CURRENT10", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("YTD10", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_TYPE1", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_CURRENT1", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_YTD1", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_TYPE2", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_CURRENT2", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_YTD2", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_TYPE3", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_CURRENT3", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_YTD3", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_TYPE4", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_CURRENT4", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_YTD4", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_TYPE5", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_CURRENT5", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_YTD5", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_TYPE6", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_CURRENT6", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_YTD6", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_TYPE7", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_CURRENT7", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_YTD7", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_TYPE8", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_CURRENT8", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_YTD8", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_TYPE9", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_CURRENT9", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_YTD9", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_TYPE10", Type.GetType("System.String")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_CURRENT10", Type.GetType("System.Decimal")));
            idtPaymentDetailsAdhoc.Columns.Add(new DataColumn("DED_YTD10", Type.GetType("System.Decimal")));
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
            DataRow drNew = idtPaymentDetailsAdhoc.NewRow();
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
            aobjCheckFileData.idecDeductionTotals = (from ldrPayment in ldtPayments.AsEnumerable()
                                                     select ldrPayment.Field<decimal>("ftm_deduction")).Sum();


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
            string lstrBenefitType = (aobjCheckFileData.ibusPaymentHistoryHeader.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance) //PIR 17504 (938 residual changes logged under this PIR ID)
                ? busConstant.PlanBenefitTypeInsurance : busConstant.PlanBenefitTypeRetirement;
            drNew["CHECK_BOTTOM"] = "C" + aobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_number.ToString().PadLeft(7, '0') + "C A" +
                                    busGlobalFunctions.GetData1ByCodeValue(7005, lstrBenefitType, iobjPassInfo) + "A " +
                                    busGlobalFunctions.GetData2ByCodeValue(7005, lstrBenefitType, iobjPassInfo) + "C";
			//PIR 15323 - start - Check stub details not being accurate when multiple checks are reissued to the same member
            if (aobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.net_amount != (aobjCheckFileData.idecPayTotals - aobjCheckFileData.idecDeductionTotals)
                //&& aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id > 0
                )
            {
                DataTable ldtbPaymentHistoryDetails = busBase.Select("cdoPaymentHistoryDetail.LoadPaymentHistoryDetailsFromDB", new object[1] { aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id });
                busPaymentHistoryHeader lbusPaymentHistoryHeader = null;
                if(ldtbPaymentHistoryDetails.Rows.Count>0)
                {
                    ResetStubDetailsToNull(drNew);
                    lbusPaymentHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
                    lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(ldtbPaymentHistoryDetails.Rows[0]);
                    lbusPaymentHistoryHeader.iclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
                    foreach (DataRow dr in ldtbPaymentHistoryDetails.Rows)
                    {
                        busPaymentHistoryDetail lbusPaymentHistoryDetail = new busPaymentHistoryDetail { icdoPaymentHistoryDetail = new cdoPaymentHistoryDetail() };
                        lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.LoadData(dr);
                        lbusPaymentHistoryDetail.ibusPaymentItemType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                        lbusPaymentHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.LoadData(dr);
                        lbusPaymentHistoryHeader.iclbPaymentHistoryDetail.Add(lbusPaymentHistoryDetail);
                    }
                    int i = 1;
                    foreach (busPaymentHistoryDetail lbusPaymentHistoryDetail in lbusPaymentHistoryHeader.iclbPaymentHistoryDetail.Where(x=>x.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1))
                    {
                        drNew["TYPE" + i] = lbusPaymentHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_description;
                        drNew["CURRENT" + i] = lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount;
                        object lobjFTDAmount = DBFunction.DBExecuteScalar("cdoPaymentHistoryDetail.LoadYTDByPaymentItemTypeID", new object[3] 
                                {aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id,
                                 lbusPaymentHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.payment_item_type_id,
                                 ibusPaymentSchedule.icdoPaymentSchedule.payment_date}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                        drNew["YTD" + i] = lobjFTDAmount ?? 0;
                        i++;
                    }
                    int j = 1;
                    foreach (busPaymentHistoryDetail lbusPaymentHistoryDetail in lbusPaymentHistoryHeader.iclbPaymentHistoryDetail.Where(x => x.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == -1))
                    {
                            drNew["DED_TYPE" + j] = lbusPaymentHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_description;
                            drNew["DED_CURRENT" + j] = lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount;
                            object lobjFTDDedAmount = DBFunction.DBExecuteScalar("cdoPaymentHistoryDetail.LoadYTDByPaymentItemTypeID", new object[3] 
                                {aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id,
                                 lbusPaymentHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.payment_item_type_id,
                                 ibusPaymentSchedule.icdoPaymentSchedule.payment_date}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            drNew["DED_YTD" + j] = lobjFTDDedAmount ?? 0;
                            j++;
                    }
                }
            }
            //PIR 15323 - start - Check stub details not being accurate when multiple checks are reissued to the same member
            idtPaymentDetailsAdhoc.Rows.Add(drNew);
        }
		//PIR 15323, fixed as part of PIR 16219
        private void ResetStubDetailsToNull(DataRow drNew)
        {
            drNew["TYPE1"] = DBNull.Value;
            drNew["CURRENT1"] = DBNull.Value;
            drNew["YTD1"] = DBNull.Value;
            drNew["TYPE2"] = DBNull.Value;
            drNew["CURRENT2"] = DBNull.Value;
            drNew["YTD2"] = DBNull.Value;
            drNew["TYPE3"] = DBNull.Value;
            drNew["CURRENT3"] = DBNull.Value;
            drNew["YTD3"] = DBNull.Value;
            drNew["TYPE4"] = DBNull.Value;
            drNew["CURRENT4"] = DBNull.Value;
            drNew["YTD4"] = DBNull.Value;
            drNew["TYPE5"] = DBNull.Value;
            drNew["CURRENT5"] = DBNull.Value;
            drNew["YTD5"] = DBNull.Value;
            drNew["TYPE6"] = DBNull.Value;
            drNew["CURRENT6"] = DBNull.Value;
            drNew["YTD6"] = DBNull.Value;
            drNew["TYPE7"] = DBNull.Value;
            drNew["CURRENT7"] = DBNull.Value;
            drNew["YTD7"] = DBNull.Value;
            drNew["TYPE8"] = DBNull.Value;
            drNew["CURRENT8"] = DBNull.Value;
            drNew["YTD8"] = DBNull.Value;
            drNew["TYPE9"] = DBNull.Value;
            drNew["CURRENT9"] = DBNull.Value;
            drNew["YTD9"] = DBNull.Value;
            drNew["TYPE10"] = DBNull.Value;
            drNew["CURRENT10"] = DBNull.Value;
            drNew["YTD10"] = DBNull.Value;

            drNew["DED_TYPE1"] = DBNull.Value;
            drNew["DED_CURRENT1"] = DBNull.Value;
            drNew["DED_YTD1"] = DBNull.Value;
            drNew["DED_TYPE2"] = DBNull.Value;
            drNew["DED_CURRENT2"] = DBNull.Value;
            drNew["DED_YTD2"] = DBNull.Value;
            drNew["DED_TYPE3"] = DBNull.Value;
            drNew["DED_CURRENT3"] = DBNull.Value;
            drNew["DED_YTD3"] = DBNull.Value;
            drNew["DED_TYPE4"] = DBNull.Value;
            drNew["DED_CURRENT4"] = DBNull.Value;
            drNew["DED_YTD4"] = DBNull.Value;
            drNew["DED_TYPE5"] = DBNull.Value;
            drNew["DED_CURRENT5"] = DBNull.Value;
            drNew["DED_YTD5"] = DBNull.Value;
            drNew["DED_TYPE6"] = DBNull.Value;
            drNew["DED_CURRENT6"] = DBNull.Value;
            drNew["DED_YTD6"] = DBNull.Value;
            drNew["DED_TYPE7"] = DBNull.Value;
            drNew["DED_CURRENT7"] = DBNull.Value;
            drNew["DED_YTD7"] = DBNull.Value;
            drNew["DED_TYPE8"] = DBNull.Value;
            drNew["DED_CURRENT8"] = DBNull.Value;
            drNew["DED_YTD8"] = DBNull.Value;
            drNew["DED_TYPE9"] = DBNull.Value;
            drNew["DED_CURRENT9"] = DBNull.Value;
            drNew["DED_YTD9"] = DBNull.Value;
            drNew["DED_TYPE10"] = DBNull.Value;
            drNew["DED_CURRENT10"] = DBNull.Value;
            drNew["DED_YTD10"] = DBNull.Value;
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
