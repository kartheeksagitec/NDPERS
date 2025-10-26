using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.Common;
using System.Diagnostics;
using System.Threading;
using NeoSpin.CustomDataObjects;
using Sagitec.ExceptionPub;
using System.Linq;
using System.IO;
using Sagitec.MetaDataCache;
using Sagitec.Rules;
using Sagitec.DBCache;
using NeoSpinBatch.Enrollment;

namespace NeoSpinBatch
{
    public partial class frmNeoSpinBatch : Form
    {
        public long ilngInitialMemoryUsed = 0, ilngFinalMemoryUsed = 0, ilngBefStepMemoryUsed = 0, ilngAfterStepMemoryUsed = 0;
        public delegate void DisplaySummaryMessage(string astrDateTime, string astrStepName, string astrType, string astrMessage);
        public delegate void DisplayDetailMessage(string astrDateTime, string astrStepName, string astrType, string astrMessage);

        public frmNeoSpinBatch(int aintStartStep, int aintStopStep, bool ablnExecuteAll,bool ablnIsfromService=false)
        {
            iblnFromNeoFlowService = ablnIsfromService;
            InitializeComponent();            
            FormInitialize();
            if ((aintStartStep > 0) && (aintStopStep > 0))
                iblnAutoExecute = true;
            else if (ablnExecuteAll)
                iblnAutoExecute = true;

            if (aintStartStep > 0)
            {
                cdoBatchSchedule lobjBatchSchedule;
                for (int i = 0; i < cmbStartStep.Items.Count; i++)
                {
                    lobjBatchSchedule = (cdoBatchSchedule)cmbStartStep.Items[i];
                    if (lobjBatchSchedule.step_no == aintStartStep)
                    {
                        cmbStartStep.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
                cmbStartStep.SelectedIndex = 0;

            if (aintStopStep > 0)
            {
                cdoBatchSchedule lobjBatchSchedule;
                for (int i = 1; i < cmbEndStep.Items.Count; i++)
                {
                    lobjBatchSchedule = (cdoBatchSchedule)cmbStartStep.Items[i];
                    if (lobjBatchSchedule.step_no == aintStopStep)
                    {
                        cmbEndStep.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
                cmbEndStep.SelectedIndex = cmbEndStep.Items.Count - 1;
        }

        static string istrUserId = "PERSLink Batch";

        //private busMQRequestHandler iobjRequestHandler = null;
        private busMQRequestHandler _ibusRequestHandler = null;

        public utlPassInfo iobjPassInfo, iobjPassInfoLog;
        public busSystemManagement iobjSystemManagement;
        public cdoProcessLog iobjProcessLog;
        public cdoBatchSchedule iobjBatchSchedule = null;
        public int iintStepIndex, iintCurrentCycleNo = 0;
        public string istrSummaryQuery = null, istrDetailQuery = null;

        private bool iblnAutoExecute = false;
        private bool iblnBatchStartedServer = false;
        public bool iblnFromNeoFlowService = false;

        private DateTime idteBatchInstanceStart;
        public List<string> ilstNotificationMessages { get; set; }
        private bool iblnSkipEntryForNotification = false;
        private bool iblnErrorEntryFound = false;
        private void FormInitialize()
        {
            StartServers();

            InitializePassInfo();

            iobjProcessLog = new cdoProcessLog();
            iobjSystemManagement = new busSystemManagement();
            ilstNotificationMessages = new List<string>();

            iobjSystemManagement.FindSystemManagement();
            iintCurrentCycleNo = iobjSystemManagement.icdoSystemManagement.current_cycle_no;

            DataTable ldtbList = busBatchSchedule.Select<cdoBatchSchedule>(null, null, null, "step_no");

            if ((ldtbList == null) || (ldtbList.Rows.Count == 0))
            {
                UpdateProcessLog("Batch Scheduler Empty. Unable to Run Batch", "SUMM");
                return;
            }
            else
            {
                foreach (DataRow ldroBatchStep in ldtbList.Rows)
                {
                    cdoBatchSchedule lobjBatchSchedule = new cdoBatchSchedule();
                    lobjBatchSchedule.LoadData(ldroBatchStep);

                    cmbStartStep.Items.Add(lobjBatchSchedule);
                    cmbStartStep.ValueMember = "step_no";
                    cmbStartStep.DisplayMember = "step_name";
                    cmbEndStep.Items.Add(lobjBatchSchedule);
                    cmbEndStep.ValueMember = "step_no";
                    cmbEndStep.DisplayMember = "step_name";
                }
            }
            iintStepIndex = 0;
            cmbEndStep.SelectedIndex = 0;
            DisposePassInfo();
        }

        private void InitializePassInfo()
        {
            iobjPassInfo = new utlPassInfo();
            utlPassInfo.iobjPassInfo = iobjPassInfo;
            utlPassInfo.iobjPassInfo.istrUserID = istrUserId; //F/W PIR 19427
            iobjPassInfo.iconFramework = DBFunction.GetDBConnection();
            iobjPassInfo.istrUserID = istrUserId;

            iobjPassInfoLog = new utlPassInfo();
            utlPassInfo.iobjPassInfoProcessLog = iobjPassInfoLog;
            utlPassInfo.iobjPassInfoProcessLog.istrUserID = istrUserId; //F/W PIR 19427
            iobjPassInfoLog.iconFramework = DBFunction.GetDBConnection();
            iobjPassInfoLog.istrUserID = istrUserId;

        }

        private void DisposePassInfo()
        {
            if ((iobjPassInfo.iconFramework != null) &&
                (iobjPassInfo.iconFramework.State == ConnectionState.Open))
            {
                iobjPassInfo.iconFramework.Close();
                iobjPassInfo.iconFramework.Dispose();
            }

            if ((iobjPassInfoLog.iconFramework != null) &&
                (iobjPassInfoLog.iconFramework.State == ConnectionState.Open))
            {
                iobjPassInfoLog.iconFramework.Close();
                iobjPassInfoLog.iconFramework.Dispose();
            }
        }


        // THIS CODE RUNS BATCH USING BATCH REQUEST TABLE
        private void StartServers()
        {
            //ServiceHelper.Initialize(utlServiceType.Local);
            if (!iblnFromNeoFlowService)
            {
                srvMetaDataCache.LoadXMLCache();
                bool lblnSuccess = srvDBCache.LoadCacheInfo();
                if (!lblnSuccess)
                {
                    MessageBox.Show(srvDBCache.istrResult);
                    Close();
                }
                try
                {
                    // Used for System Queues
                    _ibusRequestHandler = new busMQRequestHandler(utlConstants.SystemQueue);
                    //_ibusRequestHandler.idlgFileProgress = UpdateProcessLog;
                    _ibusRequestHandler.StartProcessing();
                }
                catch (Exception ex)
                {
                    ExceptionManager.Publish(ex);
                }
            }

            utlPassInfo.iobjPassInfo = new utlPassInfo();

            RulesEngine.Initialize(utlExecutionMode.Application);
            ParsingResult lobjResult = RulesEngine.LoadRulesAndExpressions(utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);

            if (lobjResult.ilstErrors.Count > 0)
            {
                StringBuilder lstrbRuleErrors = new StringBuilder();
                lstrbRuleErrors.AppendLine("Rules Errors: ");
                foreach (utlRuleMessage lobjMessage in lobjResult.ilstErrors)
                {
                    lstrbRuleErrors.AppendLine(string.Format("Object ID: {0}; Rule: {1}; Message: {2}",
                        lobjMessage.istrObjectID, lobjMessage.istrRuleID, lobjMessage.istrMessage));
                }
                MessageBox.Show(lstrbRuleErrors.ToString());
            }
        }

        private void frmNeoSpinBatch_Load(object sender, EventArgs e)
        {
            istrSummaryQuery = "select created_date as DateTime, process_name Process, message Message, process_log_id ID from sgs_process_log where cycle_no = @cycle_no " +
                " and message_type_value = 'SUMM' order by process_log_id desc";
            istrDetailQuery = "select created_date as DateTime, process_name as Process, message as Message, process_log_id as ID from sgs_process_log where cycle_no = @cycle_no " +
                " and message_type_value != 'SUMM' order by process_log_id desc";
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (iblnAutoExecute)
            {
                iintStepIndex = cmbStartStep.SelectedIndex;
                RunBatch();
                UpdateProcessLog("Batch ended", "SUMM");
                Application.Exit();
            }
        }

        private void btnRunNeoSpinBatch_Click(object sender, EventArgs e)
        {
            if (cmbStartStep.SelectedIndex > cmbEndStep.SelectedIndex)
            {
                MessageBox.Show("End step cannot precede start step.");
            }
            ((Button)sender).Enabled = false;
            try
            {
                iintStepIndex = cmbStartStep.SelectedIndex;
                RunBatch();
                UpdateProcessLog("Batch ended", "SUMM");
            }
            finally
            {
                ((Button)sender).Enabled = true;
            }
        }

        public void RunBatch()
        {
            ilngInitialMemoryUsed = GC.GetTotalMemory(false);
            InitializePassInfo();
            idteBatchInstanceStart = DateTime.Now;
            while (iintStepIndex <= cmbEndStep.SelectedIndex)
            {
                ilstNotificationMessages.Clear();
                bool lblnHighPriority = false;
                iobjBatchSchedule = (cdoBatchSchedule)cmbStartStep.Items[iintStepIndex];
                if (!SkipStep())
                {
                    if (iobjBatchSchedule.batch_schedule_id > 1)
                    {
                        UpdateProcessLog("Started ", "SUMM", iobjBatchSchedule);
                    }
                    if (iobjBatchSchedule.requires_transaction_flag == "Y")
                    {
                        iobjPassInfo.BeginTransaction();
                        //iobjPassInfo.idictParams[utlConstants.istrProcessAuditLogSync] = true;
                    }
                    else
                    {
                        //iobjPassInfo.idictParams[utlConstants.istrProcessAuditLogSync] = false;
                    }
                    try
                    {
                        ilngBefStepMemoryUsed = GC.GetTotalMemory(false);
                        ExecuteSteps(iobjBatchSchedule.batch_schedule_id);
                        ilngAfterStepMemoryUsed = GC.GetTotalMemory(false);

                        if (iobjBatchSchedule.requires_transaction_flag == "Y")
                        {
                            iobjPassInfo.Commit();
                        }
                        if (!iblnFromNeoFlowService)
                        {
                            //Run finish step only if the step finishes fine
                            FinishStep();
                        }
                    }
                    catch (Exception e)
                    {
                        if (iobjBatchSchedule.requires_transaction_flag == "Y")
                        {
                            iobjPassInfo.Rollback();
                        }
                        UpdateProcessLog("Error occurred : " + e.Message, "ERR", iobjBatchSchedule);
                        ExceptionManager.Publish(e);
                        lblnHighPriority = true;
                    }
                    long llngTotalUsed = (ilngAfterStepMemoryUsed - ilngBefStepMemoryUsed) / 1000000;
                    UpdateProcessLog("Finished Memory Used " + llngTotalUsed.ToString() + " MB", "SUMM", iobjBatchSchedule);
                }
                iintStepIndex++;
                //Notify users with the status of Current Batch Schedule
                SendBatchStatusNotification(lblnHighPriority);
            }
            ilngFinalMemoryUsed = GC.GetTotalMemory(false);
            long llngTotalUSed = (ilngFinalMemoryUsed - ilngInitialMemoryUsed) / 1000000;
            if (!iblnFromNeoFlowService)
            {
                UpdateProcessLog("Batch ended, total memory used " + llngTotalUSed.ToString() + " MB", "SUMM");
            }
            DisposePassInfo();
        }

        private void SendBatchStatusNotification(bool ablnHighPriority)
        {
            //Send Email only if any error occurred for Real Time Batch Service.
            //Send Email all time from Nightly Batch
            if (((iblnFromNeoFlowService) && (iblnErrorEntryFound)) || (!iblnFromNeoFlowService))
            {
                try
                {
                    //Load the Process Log Data
                    string lstrMailFrom = iobjSystemManagement.icdoSystemManagement.email_notification;

                    if (iobjBatchSchedule.email_notification.IsNullOrEmpty() || lstrMailFrom.IsNullOrEmpty())
                        return;

                    if (ilstNotificationMessages.Count > 2) //Sending Mail only when Count is greater than 2 because, sometimes, batch doesnt do anything other started / ended.
                    {
                        string lstrSubject = string.Format("Successfully Executed ({0}): {1}", iobjSystemManagement.icdoSystemManagement.region_description, iobjBatchSchedule.step_name);
                        if (iblnErrorEntryFound)
                            lstrSubject = lstrSubject = string.Format("Error Occurred ({0}): {1}", iobjSystemManagement.icdoSystemManagement.region_description, iobjBatchSchedule.step_name);

                        busGlobalFunctions.SendMail(lstrMailFrom, iobjBatchSchedule.email_notification, lstrSubject,
                            string.Join(@"<br/>", ilstNotificationMessages.Select(i => i).ToArray()), ablnHighPriority, true);
                    }
                }
                catch (Exception _exc)
                {
                    ExceptionManager.Publish(_exc);
                }
            }
            iblnErrorEntryFound = false;
        }

        public void ExecuteSteps(int aintStep)
        {
            busProcessFiles lobjProcessFiles;
            busProcessOutboundFile lobjProcessFilesoutbound;
            busDeferredCompBatchReport lobjDeferredCompBatchReport;
            IBSBillingBatch lobjIBSBillingBatch;
            EmployerReportPostingBatch lobjEmployerReportPostingBatch;
            busDeliquentInstallmentPaymentLetterBatch lobjDeliquentInstallmentPaymentLetterBatch;
            busServicePurchaseVoidBatch lobjServicePurchaseVoidBatch;
            busServicePurchasePostingBatch lobjServicePurchasePostingBatch;
            busPostingInterest lobjPostingInterest;
            busAllocateRemittanceToARBatch lobjAllocateRemittanceToARBatch;
            busPersonAccountEAPEnrollmentBatch lobjPersonAccountEAPBatch;
            busMergeEmployerBatch lobjMergeEmployerBatch;
            busMissingContributionBatch lobjMissingContributionBatch;
            busDefCompCatchUpDateEndingBatch lobjbusDefCompCatchUpDateEndingBatch;
            busIBSAdjustmentPostingBatch lobjIBSAdjustmentPostingBatch;
            busDeferredCompEnrollmentFileBatch lobjDeferredCompEnrollmentFileBatch;
            busAutoRefundBatch lobjAutoRefundBatch;
            AppendBatchScheduleID();
            //This switches on scheduled-id and not on step-no
            switch (aintStep)
            {
                case 1:
                    InitializeStep();
                    break;
                case 3:
                    lobjProcessFiles = new busProcessFiles();
                    lobjProcessFiles.iobjSystemManagement = iobjSystemManagement;
                    lobjProcessFiles.iintCycleNo = iintCurrentCycleNo;
                    lobjProcessFiles.idlgUpdateProcessLog = new busProcessFiles.UpdateProcessLog(UpdateProcessLog);
                    if (!iblnFromNeoFlowService)
                    {
                        utlPassInfo.iobjPassInfo = iobjPassInfo;
                        lobjProcessFiles.ReceiveFiles();
                    }
                    else
                    {
                        lobjProcessFiles.ReceiveFiles(10);
                        lobjProcessFiles.ReceiveFiles(11);
                        lobjProcessFiles.ReceiveFiles(12);
                        lobjProcessFiles.ReceiveFiles(28);
                    }
                    break;
                case 4:
                    lobjProcessFiles = new busProcessFiles();
                    lobjProcessFiles.iobjSystemManagement = iobjSystemManagement;
                    lobjProcessFiles.idlgUpdateProcessLog = new busProcessFiles.UpdateProcessLog(UpdateProcessLog);
                    if (!iblnFromNeoFlowService)
                    {
                        utlPassInfo.iobjPassInfo = iobjPassInfo;
                        lobjProcessFiles.UploadFiles();
                    }
                    else
                    {
                        lobjProcessFiles.UploadFiles(10);
                        lobjProcessFiles.UploadFiles(11);
                        lobjProcessFiles.UploadFiles(12);
                        lobjProcessFiles.UploadFiles(28);
                        lobjProcessFiles.UploadFiles(112); //PIR 26615 fleTFFRDualServiceImport
                    }
                    CheckFileInProperFormat(lobjProcessFiles);
                    break;
                case 5:
                    lobjProcessFiles = new busProcessFiles();
                    lobjProcessFiles.idlgUpdateProcessLog = new busProcessFiles.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFiles.iobjSystemManagement = iobjSystemManagement;
                    if (!iblnFromNeoFlowService)
                    {
                        utlPassInfo.iobjPassInfo = iobjPassInfo;
                        lobjProcessFiles.ProcessInboundFiles();
                    }
                    else
                    {
                        lobjProcessFiles.ProcessInboundFiles(10);
                        lobjProcessFiles.ProcessInboundFiles(11);
                        lobjProcessFiles.ProcessInboundFiles(12);
                        lobjProcessFiles.ProcessInboundFiles(28);
                        lobjProcessFiles.ProcessInboundFiles(112); //PIR 26615 fleTFFRDualServiceImport
                    }
                    break;
                case 6:
                    //Deferred Compensation Agent Batch Report
                    lobjDeferredCompBatchReport = new busDeferredCompBatchReport();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDeferredCompBatchReport.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDeferredCompBatchReport.iobjSystemManagement = iobjSystemManagement;
                    lobjDeferredCompBatchReport.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDeferredCompBatchReport.CreateCorrespondenceForDeferredCompAgentSeminar();
                    break;
                case 8:
                    // Create Mailing Label Client Outbound File.                  
                    busMailingLabelBatch lobjMailingLabelBatch = new busMailingLabelBatch();
                    lobjMailingLabelBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjMailingLabelBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjMailingLabelBatch.iobjBatchSchedule = iobjBatchSchedule;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMailingLabelBatch.GetMailingLabels();
                    break;
                case 9:
                    //Purge Correspondence
                    busPurgeCorrespondence lobjPurgeCorrespondence = new busPurgeCorrespondence();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPurgeCorrespondence.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjPurgeCorrespondence.iobjSystemManagement = iobjSystemManagement;
                    lobjPurgeCorrespondence.iobjBatchSchedule = iobjBatchSchedule;
                    lobjPurgeCorrespondence.ExecuteCorrespondencePurgeProcess();
                    break;
                case 10:
                    // Individual Billing System Batch
                    lobjIBSBillingBatch = new IBSBillingBatch();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjIBSBillingBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjIBSBillingBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjIBSBillingBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjIBSBillingBatch.ProcessIBSDetailRecords();
                    break;
                case 11:
                    // G/L Out File For PeopleSoft - Daily Run
                    busGLPeopleSoftFileBatch lobjGLPeopleSoftFileBatch = new busGLPeopleSoftFileBatch();
                    lobjGLPeopleSoftFileBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjGLPeopleSoftFileBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjGLPeopleSoftFileBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjGLPeopleSoftFileBatch.ProcessGLPeopleSoftFile();
                    break;
                case 12:
                    // G/L Out File For RIO - Monthly Run
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(busConstant.GLRIOOutFileID);
                    break;
                case 13:
                    // Reporting and Posting of Insurance.
                    busPostingInsurance lobjPostInsurance = new busPostingInsurance();
                    lobjPostInsurance.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjPostInsurance.iobjBatchSchedule = iobjBatchSchedule;
                    lobjPostInsurance.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPostInsurance.PostingInsurance();
                    break;
                case 14:
                    // Employer Report Posting and Processing Negative adjustments for Retirement and Deferred Compensation.
                    lobjEmployerReportPostingBatch = new EmployerReportPostingBatch();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjEmployerReportPostingBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjEmployerReportPostingBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjEmployerReportPostingBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjEmployerReportPostingBatch.ProcessEmployerReportPosting();
                    break;
                case 15:
                    // Reload Insurance Batch
                    busReloadInsurance lobjReloadInsurance = new busReloadInsurance();
                    lobjReloadInsurance.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjReloadInsurance.iobjBatchSchedule = iobjBatchSchedule;
                    lobjReloadInsurance.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjReloadInsurance.ReloadInsurance();
                    break;
                case 16:
                    // Disburse funds to Providers and Vendors.
                    busDisburseFundsBatch lobjDisburseFunds = new busDisburseFundsBatch();
                    lobjDisburseFunds.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDisburseFunds.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDisburseFunds.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDisburseFunds.DisburseFunds();
                    break;
                case 17:
                    //Deliquent Installment Payment Letter Batch
                    lobjDeliquentInstallmentPaymentLetterBatch = new busDeliquentInstallmentPaymentLetterBatch();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDeliquentInstallmentPaymentLetterBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDeliquentInstallmentPaymentLetterBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjDeliquentInstallmentPaymentLetterBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDeliquentInstallmentPaymentLetterBatch.CreateCorrespondenceForDeliquentInstallmentPayment();
                    break;
                case 18:
                    //Update Status as void when expiration date is past date.
                    lobjServicePurchaseVoidBatch = new busServicePurchaseVoidBatch();
                    lobjServicePurchaseVoidBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjServicePurchaseVoidBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjServicePurchaseVoidBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjServicePurchaseVoidBatch.UpdateHeaderWithExpirationDateExpiredWithStatusVoid();
                    break;
                case 19:
                    //Posting service Purchase batch
                    lobjServicePurchasePostingBatch = new busServicePurchasePostingBatch();
                    lobjServicePurchasePostingBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjServicePurchasePostingBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjServicePurchasePostingBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjServicePurchasePostingBatch.PostPurchasePaymentAndServiceCredit();
                    break;
                case 20:
                    //Posting Interest
                    lobjPostingInterest = new busPostingInterest();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPostingInterest.iobjSystemManagement = iobjSystemManagement;
                    lobjPostingInterest.iobjBatchSchedule = iobjBatchSchedule;
                    lobjPostingInterest.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjPostingInterest.CreatePostingInterest();
                    break;
                case 21:
                    // Generate Correspondence for InEligible Dependents
                    busIneligibleDependentBatch lobjIneligibleDependents = new busIneligibleDependentBatch();
                    lobjIneligibleDependents.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjIneligibleDependents.iobjBatchSchedule = iobjBatchSchedule;
                    lobjIneligibleDependents.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjIneligibleDependents.CreateIneligibleDependentCorrespondence();
                    break;
                case 22:
                    // COBRA Coverage Termination Batch
                    busCOBRACoverageTerminationBatch lobjCOBRATermination = new busCOBRACoverageTerminationBatch();
                    lobjCOBRATermination.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjCOBRATermination.iobjBatchSchedule = iobjBatchSchedule;
                    lobjCOBRATermination.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjCOBRATermination.CreateCOBRATerminationLetters();
                    break;
                case 23:
                    // Medicare Age 65 Notice Letters
                    busMedicareAge65Batch lobjMedicare = new busMedicareAge65Batch();
                    lobjMedicare.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjMedicare.iobjBatchSchedule = iobjBatchSchedule;
                    lobjMedicare.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMedicare.CreateMedicareAge65BatchLetters();
                    break;
                case 24:
                    // Generate LOA Batch
                    busLOABatch lobjLOABatch = new busLOABatch();
                    lobjLOABatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjLOABatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjLOABatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjLOABatch.ProcessLOABatch();
                    break;
                case 25:
                    // Loss of Supplemental Life Coverage Notice
                    busLossofSupplementalLifeCoverage lobjLifeCoverage = new busLossofSupplementalLifeCoverage();
                    lobjLifeCoverage.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjLifeCoverage.iobjBatchSchedule = iobjBatchSchedule;
                    lobjLifeCoverage.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjLifeCoverage.CreateLifeCoverageCorrespondence();
                    break;
                case 26:
                    // DC Transfer Reminder Batch
                    busDCTransferReminderBatch lobjDCTransferReminderBatch = new busDCTransferReminderBatch();
                    lobjDCTransferReminderBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDCTransferReminderBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDCTransferReminderBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDCTransferReminderBatch.CreateDCTransferReminderBatch();
                    break;
                case 27:
                    /// Create Correspondence Batch for Non Payee Termination Notice
                    busNonPayeeEmploymentTerminationNotices lobjNonPayee = new busNonPayeeEmploymentTerminationNotices();
                    lobjNonPayee.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjNonPayee.iobjBatchSchedule = iobjBatchSchedule;
                    lobjNonPayee.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjNonPayee.GenerateCorrespondenceForNonPayeeTerminationNotice();
                    lobjNonPayee.GenerateCorrespondenceForNonPayeeCOBRATerminationNotice();
                    break;
                case 28:
                    // Approaching 457 Contribution Limit Batch
                    busApproaching457ContributionLimitBatch lobjApproachingContributions = new busApproaching457ContributionLimitBatch();
                    lobjApproachingContributions.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjApproachingContributions.iobjBatchSchedule = iobjBatchSchedule;
                    lobjApproachingContributions.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjApproachingContributions.CreateApproachingContributionLimitCorrespondence();
                    break;
                case 29:
                    /// End Date Disability Term Life Insurance Batch
                    busEndDateDisabilityTermLifeInsuranceBatch lobjEndDateDisability = new busEndDateDisabilityTermLifeInsuranceBatch();
                    lobjEndDateDisability.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjEndDateDisability.iobjBatchSchedule = iobjBatchSchedule;
                    lobjEndDateDisability.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjEndDateDisability.CreateCorrespondenceForEndDateDisability();
                    break;
                case 30:
                    /// Vested Membership Letter Batch
                    busVestedMembershipLetterBatch lobjVestedMember = new busVestedMembershipLetterBatch();
                    lobjVestedMember.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjVestedMember.iobjBatchSchedule = iobjBatchSchedule;
                    lobjVestedMember.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjVestedMember.CreateCorrespondenceForVestedMembership();
                    break;
                case 31:
                    /// Welcome Batch to Member
                    busWelcomeBatch lobjWelcomeBatch = new busWelcomeBatch();
                    lobjWelcomeBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjWelcomeBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjWelcomeBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjWelcomeBatch.CreateWelcomeBatchCorrespondence();
                    break;
                case 32:
                    // IBS Over Due Batch Letter - PDP Disenrollement,non - payment
                    IBSOverDueBatchPDPDisenrollmentLetter lobjIBSOverDueBatchPDPDisenrollmentLetter = new IBSOverDueBatchPDPDisenrollmentLetter();
                    lobjIBSOverDueBatchPDPDisenrollmentLetter.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjIBSOverDueBatchPDPDisenrollmentLetter.iobjBatchSchedule = iobjBatchSchedule;
                    lobjIBSOverDueBatchPDPDisenrollmentLetter.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjIBSOverDueBatchPDPDisenrollmentLetter.CreatePDPDisEnrollmentLetter();
                    break;
                case 33:
                    // IBS Overdue Batch Letter - Delinquent Letter 1
                    IBSOverDueBatchDelinquentLetter1 lobjIBSOverDueBatchDelinquentLetter1 = new IBSOverDueBatchDelinquentLetter1();
                    lobjIBSOverDueBatchDelinquentLetter1.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjIBSOverDueBatchDelinquentLetter1.iobjBatchSchedule = iobjBatchSchedule;
                    lobjIBSOverDueBatchDelinquentLetter1.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjIBSOverDueBatchDelinquentLetter1.CreateIBSDelinquentLetter();
                    break;
                case 34:
                    // IBS Overdue Batch Letter - Delinquent Letter 2
                    IBSOverDueBatchDelinquentLetter2 lobjIBSOverDueBatchDelinquentLetter2 = new IBSOverDueBatchDelinquentLetter2();
                    lobjIBSOverDueBatchDelinquentLetter2.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjIBSOverDueBatchDelinquentLetter2.iobjBatchSchedule = iobjBatchSchedule;
                    lobjIBSOverDueBatchDelinquentLetter2.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjIBSOverDueBatchDelinquentLetter2.CreateIBSDelinquentLetter();
                    break;
                case 35:
                    //Monthly Employer Statement Batch
                    busMonthlyEmployerStatementBatch lobjMonthlyEmployerStatementBatch = new busMonthlyEmployerStatementBatch();
                    lobjMonthlyEmployerStatementBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjMonthlyEmployerStatementBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjMonthlyEmployerStatementBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMonthlyEmployerStatementBatch.GenerateMonthlyEmployerStatment();
                    break;
                case 36:
                    // Allocate Remittance To A/R Batch
                    lobjAllocateRemittanceToARBatch = new busAllocateRemittanceToARBatch();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjAllocateRemittanceToARBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjAllocateRemittanceToARBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjAllocateRemittanceToARBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjAllocateRemittanceToARBatch.AllocateIBSRemittance();
                    break;
                case 37:
                    // 022-Files Batch - HMO Enrollment
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(48);
                    break;
                case 38:
                    // 022-Files Batch - EAP Enrollment
                    lobjPersonAccountEAPBatch = new busPersonAccountEAPEnrollmentBatch();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjPersonAccountEAPBatch.GenerateFiles(lobjProcessFilesoutbound);
                    break;
                case 39:
                    //Missing Contribution batch                                 
                    lobjMissingContributionBatch = new busMissingContributionBatch();
                    lobjMissingContributionBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjMissingContributionBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjMissingContributionBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMissingContributionBatch.CreateCorrForMissingContribution();
                    break;
                case 40:
                    // Merge Employer Batch
                    lobjMergeEmployerBatch = new busMergeEmployerBatch();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMergeEmployerBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjMergeEmployerBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjMergeEmployerBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjMergeEmployerBatch.ProcessMerging();
                    break;
                case 41:
                    // Deferred Comp 3 Yr Catch Up End Date Ending Report batch
                    lobjbusDefCompCatchUpDateEndingBatch = new busDefCompCatchUpDateEndingBatch();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjbusDefCompCatchUpDateEndingBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjbusDefCompCatchUpDateEndingBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjbusDefCompCatchUpDateEndingBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjbusDefCompCatchUpDateEndingBatch.ProcessReport(iobjSystemManagement.icdoSystemManagement.batch_date);
                    break;
                case 42:
                    // Individual Billing System Batch
                    lobjIBSAdjustmentPostingBatch = new busIBSAdjustmentPostingBatch();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjIBSAdjustmentPostingBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjIBSAdjustmentPostingBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjIBSAdjustmentPostingBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjIBSAdjustmentPostingBatch.PostIBSAdjustmentRecords();
                    break;
                case 43:
                    // Service Purchase Insufficient Purchase Payment Report
                    busInsufficientPurchasePaymentReportBatch lobjInsufficientPurchasePaymentReportBatch = new busInsufficientPurchasePaymentReportBatch();
                    lobjInsufficientPurchasePaymentReportBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjInsufficientPurchasePaymentReportBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjInsufficientPurchasePaymentReportBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjInsufficientPurchasePaymentReportBatch.GenerateInsufficientPurchasePayment();
                    break;
                case 44:
                    // Service Purchase Monthly Purchase Service Credit Posting Report
                    busMonthlyPurchasedSCPostReport lobjMonthlyPurchasedSCPostReport = new busMonthlyPurchasedSCPostReport();
                    lobjMonthlyPurchasedSCPostReport.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjMonthlyPurchasedSCPostReport.iobjBatchSchedule = iobjBatchSchedule;
                    lobjMonthlyPurchasedSCPostReport.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMonthlyPurchasedSCPostReport.GenerateMonthlyPurchasedSCPostingReport();
                    break;
                case 45:
                    // Service Purchase - Final Purchase Installment Due Report
                    busFinalPurchaseInstallmentDueReport lobjFinalPurchaseInstallmentDueReport = new busFinalPurchaseInstallmentDueReport();
                    lobjFinalPurchaseInstallmentDueReport.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjFinalPurchaseInstallmentDueReport.iobjBatchSchedule = iobjBatchSchedule;
                    lobjFinalPurchaseInstallmentDueReport.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjFinalPurchaseInstallmentDueReport.GenerateFinalPurchaseInstallmentDue();
                    break;
                case 46:
                    // Service Purchase - Final Purchase Installment Due Report
                    busFinalPurchasePaymentReceivedReport lobjFinalPurchasePaymentReceivedReport = new busFinalPurchasePaymentReceivedReport();
                    lobjFinalPurchasePaymentReceivedReport.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjFinalPurchasePaymentReceivedReport.iobjBatchSchedule = iobjBatchSchedule;
                    lobjFinalPurchasePaymentReceivedReport.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjFinalPurchasePaymentReceivedReport.GenerateFinalPurchasePaymentReceived();
                    break;
                case 47:
                    // 022-Files Batch - Fidelty DC Enrollment file out
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(49);
                    break;
                case 48:
                    // 022-Files Batch - Fidelity Enrollment - 457 File
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(50);
                    break;
                case 49:
                    // 022-Files Batch - Deferred Comp File By Provider                  
                    lobjDeferredCompEnrollmentFileBatch = new busDeferredCompEnrollmentFileBatch();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDeferredCompEnrollmentFileBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDeferredCompEnrollmentFileBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDeferredCompEnrollmentFileBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjDeferredCompEnrollmentFileBatch.GenerateDeferredCompFileOut();
                    break;
                case 50:
                    // 022-Manual Adjustments between DB Plans Report         
                    busDBManualAdjustmentsReportBatch lobjDBManualAdjustmentsReportBatch = new busDBManualAdjustmentsReportBatch();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDBManualAdjustmentsReportBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDBManualAdjustmentsReportBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDBManualAdjustmentsReportBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjDBManualAdjustmentsReportBatch.GenerateDBManualAdjustmentReport();
                    break;
                case 51:
                    // 022-Files Batch - TFFR
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(51);
                    break;
                case 52:
                    // HIPPA Health BCBS
                    busHIPAAHealthBCBS lobjBCBS = new busHIPAAHealthBCBS();
                    lobjBCBS.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjBCBS.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjBCBS.CreateHIPAAHealthBCBSFile();
                    break;
                case 53:
                    // HIPAA Dental CIGNA
                    busHIPAACignaDental lobjDental = new busHIPAACignaDental();
                    lobjDental.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDental.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDental.CreateHIPAACignaDentalFile();
                    break;
                case 54:
                    // HIPAA Vision Ameritas
                    busHIPAAAmeritasVision lobjVision = new busHIPAAAmeritasVision();
                    lobjVision.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjVision.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjVision.CreateHIPAAAmeritasVisionFile();
                    break;
                case 55:
                    //PIR 17081 - Daily PeopleSoft file
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    lobjProcessFilesoutbound.iarrParameters = new object[1];
                    lobjProcessFilesoutbound.iarrParameters[0] = false;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(103);
                    break;
                case 56:
                    // 071 - ACH - Pre-Note Verification Batch
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    lobjProcessFilesoutbound.iarrParameters = new object[1];
                    lobjProcessFilesoutbound.iarrParameters[0] = true; // Turn the Flag of ACH Pre-Note Verification
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    if (lobjProcessFilesoutbound.CreateOutboundFile(57) == -1)
                        UpdateProcessLog("No Records Found", "INFO");
                    break;
                case 57:
                    // Payee attain Age 18 Batch
                    busPayeeAttain18Letter lobjPayee = new busPayeeAttain18Letter();
                    lobjPayee.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjPayee.iobjBatchSchedule = iobjBatchSchedule;
                    lobjPayee.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPayee.CreatePayeeAttain18Letter();
                    break;
                case 58:
                    // Update Federal and State Tax Amount Batch
                    busUpdateTaxAmount lobjUpdateTax = new busUpdateTaxAmount();
                    lobjUpdateTax.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjUpdateTax.iobjBatchSchedule = iobjBatchSchedule;
                    lobjUpdateTax.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    string IsStateTaxApplicable = busGlobalFunctions.GetData2ByCodeValue(52, busConstant.TaxApplicable, iobjPassInfo);
                    lobjUpdateTax.UpdateFederalAndStateTaxAmount(IsStateTaxApplicable == busConstant.Flag_Yes);
                    break;
                case 59:
                    // Deferred Batch Letter
                    busDeferredBatchLetter lobjDeferredBatch = new busDeferredBatchLetter();
                    lobjDeferredBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDeferredBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDeferredBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDeferredBatch.SendLetterToDeferredApplicationMembers();
                    break;
                case 60:
                    // Initiate Retirement WorkFlow
                    busIntiatingRetirementWorkflow lobjInitiatingRetirementWorkflow = new busIntiatingRetirementWorkflow();
                    lobjInitiatingRetirementWorkflow.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjInitiatingRetirementWorkflow.iobjBatchSchedule = iobjBatchSchedule;
                    lobjInitiatingRetirementWorkflow.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjInitiatingRetirementWorkflow.LoadPersonToInitiateWorkflow();
                    break;
                case 61:
                    // Process Batch Report
                    busProcessBatchReport lobjProcessBatchReport = new busProcessBatchReport();
                    lobjProcessBatchReport.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessBatchReport.iobjBatchSchedule = iobjBatchSchedule;
                    lobjProcessBatchReport.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessBatchReport.GenerateProcessBatchReport();
                    break;
                case 62:
                    // User Batch Report
                    busUserBatchReport lobjUserBatchReport = new busUserBatchReport();
                    lobjUserBatchReport.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjUserBatchReport.iobjBatchSchedule = iobjBatchSchedule;
                    lobjUserBatchReport.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjUserBatchReport.GenerateUserBatchReport();
                    break;
                case 63:
                    // Exception Reports
                    busExceptionReport lobjExceptionReport = new busExceptionReport();
                    lobjExceptionReport.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjExceptionReport.iobjBatchSchedule = iobjBatchSchedule;
                    lobjExceptionReport.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjExceptionReport.GenerateExceptionReport();
                    break;
                case 64:
                    // Pre Retirement Death - Pre Retirement Death Workflow
                    busPreRetirmentDeathWorkflowBatch lobjPreRetirmentDeathWorkflowBatch = new busPreRetirmentDeathWorkflowBatch();
                    lobjPreRetirmentDeathWorkflowBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjPreRetirmentDeathWorkflowBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjPreRetirmentDeathWorkflowBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPreRetirmentDeathWorkflowBatch.CreateWorkflowForPreRetirement();
                    break;
                case 66:
                    //Pre Retirement Death , Refund - Auto Refund Batch         
                    lobjAutoRefundBatch = new busAutoRefundBatch();
                    lobjAutoRefundBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjAutoRefundBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjAutoRefundBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjAutoRefundBatch.AutoRefund();
                    break;
                case 67:
                    // Expired DRO Notice Batch        
                    busExpiredDRONoticeBatch lobjExpiredDRONoticeBatch = new busExpiredDRONoticeBatch();
                    lobjExpiredDRONoticeBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjExpiredDRONoticeBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjExpiredDRONoticeBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjExpiredDRONoticeBatch.Generate488DaysDROExpiringExpiredLetter();
                    lobjExpiredDRONoticeBatch.Generate503DaysDROExpiringExpiredLetter();
                    lobjExpiredDRONoticeBatch.Generate18MonthsDROExpiringExpiredLetter();
                    break;
                case 68:
                    // Alter Payee Workflow Batch
                    busAlternatePayeeWorkflowBatch lobjAlternatePayeeWorkflowBatch = new busAlternatePayeeWorkflowBatch();
                    lobjAlternatePayeeWorkflowBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjAlternatePayeeWorkflowBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjAlternatePayeeWorkflowBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjAlternatePayeeWorkflowBatch.IniateWorkflowFolrAlternatePayee();
                    break;
                case 69:
                    //TFFR Transfer Report
                    busTFFRTransferReport lobjTFFRTransferReport = new busTFFRTransferReport();
                    lobjTFFRTransferReport.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjTFFRTransferReport.iobjBatchSchedule = iobjBatchSchedule;
                    lobjTFFRTransferReport.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjTFFRTransferReport.GenerateTFFRTransferReport();
                    break;
                case 70:
                    //Job Service RHIC Report
                    busJobServiceRHICReport lobjJobServiceRHICReport = new busJobServiceRHICReport();
                    lobjJobServiceRHICReport.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjJobServiceRHICReport.iobjBatchSchedule = iobjBatchSchedule;
                    lobjJobServiceRHICReport.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjJobServiceRHICReport.GenerateJobServiceRHICReport();
                    break;
                case 71:
                    // Process Incomplete Refund Application Batch
                    busProcessIncompleteRefundApplicationBatch lobjbusProcessIncompleteRefundApplicationBatch = new busProcessIncompleteRefundApplicationBatch();
                    lobjbusProcessIncompleteRefundApplicationBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjbusProcessIncompleteRefundApplicationBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjbusProcessIncompleteRefundApplicationBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjbusProcessIncompleteRefundApplicationBatch.ProcessIncompleteRefundApplication();
                    break;
                case 72:
                    //Pull ACH File
                    busPullACHFile lobjPullACHFile = new busPullACHFile();
                    lobjPullACHFile.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjPullACHFile.iobjBatchSchedule = iobjBatchSchedule;
                    lobjPullACHFile.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPullACHFile.GenerateACHPullForBNDFileOut();
                    break;
                case 73:
                    //Death Match file for SHA
                    busDeathMatchFile lobjDeathMatchFileSHA = new busDeathMatchFile();
                    lobjDeathMatchFileSHA.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDeathMatchFileSHA.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDeathMatchFileSHA.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDeathMatchFileSHA.GenerateSHAFile();
                    break;

                case 74:
                    //Death Match File for SSA
                    busDeathMatchFile lobjDeathMatchFileSSA = new busDeathMatchFile();
                    lobjDeathMatchFileSSA.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDeathMatchFileSSA.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDeathMatchFileSSA.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDeathMatchFileSSA.GenerateSSAFile();
                    break;
                case 75:
                    //Disability to Normal Conversion - age based
                    busDisabilitytoNormalAgeConversionbatch lobjDisabilitytoNormalAgeConversionbatch = new busDisabilitytoNormalAgeConversionbatch();
                    lobjDisabilitytoNormalAgeConversionbatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDisabilitytoNormalAgeConversionbatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDisabilitytoNormalAgeConversionbatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDisabilitytoNormalAgeConversionbatch.ConvertDisabilityToNormalBasedOnAgeEligibility();
                    break;
                case 76:
                    //Disability to Normal Conversion - rule based
                    busDisabilitytoNormalRuleConversionbatch lobjDisabilitytoNormalRuleConversionbatch = new busDisabilitytoNormalRuleConversionbatch();
                    lobjDisabilitytoNormalRuleConversionbatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDisabilitytoNormalRuleConversionbatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDisabilitytoNormalRuleConversionbatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDisabilitytoNormalRuleConversionbatch.ConvertDisabilityToNormalBasedOnRuleEligibility();
                    break;
                case 77:
                    // UCS-082 - Medical Consultant Letter Batch
                    busMedicalConsultantLetterbatch lobjMedicalConsultantLetter = new busMedicalConsultantLetterbatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMedicalConsultantLetter.MedicalConsultantLetter();
                    break;
                case 78:
                    // UCS-082 - Income Verification Batch
                    busIncomeVerificationbatch lobjIncomeVerfication = new busIncomeVerificationbatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjIncomeVerfication.IncomeVerificationBatch();
                    break;
                case 79:
                    // UCS-082 - Disability Recertification Batch
                    busDisabilityRecertificationbatch lobjDisabilityRecertification = new busDisabilityRecertificationbatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDisabilityRecertification.DisabilityRecertificationBatch();
                    break;
                case 80:
                    // UCS-075 - Payroll Batch
                    busMonthlyPaymentProcessBatch lobjMonthlyPaymentProcessBatch = new busMonthlyPaymentProcessBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMonthlyPaymentProcessBatch.ProcessPayments();
                    break;
                case 85:
                    // UCS-075 - Payee Error Report Batch
                    busPayeeErrorReport lbusPayeeErrorReport = new busPayeeErrorReport()
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusPayeeErrorReport.GeneratePayeeErrorReport();
                    break;
                case 81:
                    // UCS-093 - Process RMD Notices - Monthly Batch
                    busProcessRequiredMinimumDistributionNotices lobjRMDNotices = new busProcessRequiredMinimumDistributionNotices
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjRMDNotices.ProcessRMDMonthlyNotices();
                    break;
                case 82:
                    // UCS-093 - Process Active 70.5 RMD Notices - Monthly Batch
                    busProcessActiveDB70Point5RMDNoticesBatch lobjActiveDBNotices = new busProcessActiveDB70Point5RMDNoticesBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjActiveDBNotices.ProcessActiveDB70Point5RMDNotices();
                    break;
                case 83:
                    // UCS-093 - Process Def Comp Member RMD Notices - Monthly Batch
                    busProcessDefCompMemberRMDNoticesMonthlyBatch lobjDefCompMonthlyRMDNotices = new busProcessDefCompMemberRMDNoticesMonthlyBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDefCompMonthlyRMDNotices.ProcessDefCompMemberRMDNoticesMonthly();
                    break;
                case 84:
                    // UCS-093 - Process Def Comp Member RMD Notices - Yearly Batch
                    busProcessDefCompMemberRMDNoticesYearlyBatch lobjDefCompYearlyRMDNotices = new busProcessDefCompMemberRMDNoticesYearlyBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDefCompYearlyRMDNotices.ProcessDefCompMemberRMDNoticesYearly();
                    break;
                case 86:
                    // UCS-075 - Adhoc Payment Process Batch
                    busAdhocPaymentProcessBatch lbusAdhocPaymentProcessBatch = new busAdhocPaymentProcessBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusAdhocPaymentProcessBatch.ProcessPayments();
                    break;
                case 87:
                    // UCS-053 - Non Employee Death Batch
                    busNonEmployeeDeathBatch lNonEmployeeDeathBatch = new busNonEmployeeDeathBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lNonEmployeeDeathBatch.GenerateCorrepondenceForNonEmployeeDeath();
                    break;
                case 88:
                    // UCS-075 - Adhoc Trial Report Batch
                    AdhocProcessTrialReportBatch lbusAdhocTrialReportBatch = new AdhocProcessTrialReportBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusAdhocTrialReportBatch.RunTrialReport();
                    break;
                case 89:
                    // UCS-092 - Actuarial Pension File Batch
                    busPensionFileBatch lbusPensionFileBatch = new busPensionFileBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusPensionFileBatch.ProcessPensionFiles(busConstant.PensionFileTypeAnnual);
                    break;
                case 90:
                    // UCS-092 - Actuarial Pension File Batch
                    busPensionFileBatch lobjPensionFileBatch = new busPensionFileBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPensionFileBatch.ProcessPensionFiles(busConstant.PensionFileTypeAdhoc);
                    break;
                case 91:
                    // UCS-078 - ACH Status update Batch
                    busAchStatusUpdateBatch lobjAchStatusUpdateBatch = new busAchStatusUpdateBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjAchStatusUpdateBatch.UpdateACHStatus();
                    break;
                case 92:
                    // UCS-054 - Post-Retirement Account Owner Death Batch
                    busPostRetirementAccountOwnerDeathBatch lobjPoRAccountOwnerDeath = new busPostRetirementAccountOwnerDeathBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPoRAccountOwnerDeath.PostRetirementAccountOwnerDeathBatch();
                    break;
                case 93:
                    // UCS-054 - Post-Retirement Alternate Payee Death Batch
                    busPostRetirementAlternatePayeeDeathBatch lobjPoRAlternatePayeeDeath = new busPostRetirementAlternatePayeeDeathBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPoRAlternatePayeeDeath.ProcesPostRetirementAlternatePayeeDeath(); ;
                    break;
                case 94:
                    // UCS-054 - Post-Retirement First Beneficiary Death Batch
                    busPostRetirementFirstBeneficiaryDeathBatch lobjPoRFirstBeneficiaryDeath = new busPostRetirementFirstBeneficiaryDeathBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPoRFirstBeneficiaryDeath.PostRetirementFirstBeneficiaryDeathBatch();
                    break;
                case 95:
                    //ucs - 096 : CAFR Report Generation
                    busCAFRReportBatch lobjCAFRReportBatch = new busCAFRReportBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjCAFRReportBatch.CreateReports();
                    break;
                case 96:
                    // UCS-084 - Process COLA Batch
                    busProcessCOLABatch lobjProcessCOLABatch = new busProcessCOLABatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessCOLABatch.ProcessCOLA();
                    break;
                case 97:
                    // UCS-084 - Process Ad hoc Increase Batch
                    busProcessAdhocIncreaseBatch lobjProcessAdhocIncreaseBatch = new busProcessAdhocIncreaseBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessAdhocIncreaseBatch.ProcessAdhocIncrease();
                    break;
                case 98:
                    // UCS-084 - Process Supplemental Check Batch
                    busProcessSupplementalCheckBatch lobjProcessSupplementalBatch = new busProcessSupplementalCheckBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessSupplementalBatch.ProcessSupplementalCheck();
                    break;
                case 99:
                    // UCS-077 - Generate Cash Outstanding Checks Letters
                    busGenerateCashOutstandingCheckLettersBatch lbusGenerateCashOutstandingCheckLettersBatch = new busGenerateCashOutstandingCheckLettersBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusGenerateCashOutstandingCheckLettersBatch.GenerateCashOutstandingCheckLetters();
                    break;
                case 100:
                    // UCS-007 - Generate Back Log report Batch
                    busBackLogBatchReport lbusBackLogBatch = new busBackLogBatchReport
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusBackLogBatch.GenerateBackLogReport();
                    break;
                case 101:
                    // UCS-054 - Member Annual Statement
                    busMemberAnnualStatement lobjMAS = new busMemberAnnualStatement
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMAS.CreateMemberAnnualStatements();
                    break;
                case 102:
                    // UCS-040 - update dues on rate change
                    busUpdateDuesBatch lbusUpdateDuesBatch = new busUpdateDuesBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusUpdateDuesBatch.UpdateDues();
                    break;
                case 103:
                    // UCS-040 - Life rate change
                    busInsuranceRateChangeLetterBatch lbusInsuranceRateChangeBatch = new busInsuranceRateChangeLetterBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusInsuranceRateChangeBatch.GenerateLettersForRateChange(); //PIR 6933
                    break;

                case 104:
                    //UCS - 091 : Monthly 1099r Reports batch
                    busMonthly1099RReportsBatch lobjMonthly1099RReportsBatch = new busMonthly1099RReportsBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMonthly1099RReportsBatch.GenerateMonthly1099rReport();
                    break;
                case 105:
                    //UCS - 091 : Annual 1099r batch
                    busAnnual1099rBatch lobjAnnual1099rBatch = new busAnnual1099rBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjAnnual1099rBatch.ProcessAnnual1099rBatch();
                    break;
                case 106:
                    //UCS - 091 : Corrected 1099r batch
                    busCorrected1099rBatch lobjCorrected1099rBatch = new busCorrected1099rBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjCorrected1099rBatch.ProcessCorrected1099rBatch();
                    break;
                case 107:
                    //UCS - 022 : No HDV dependent report batch
                    busNoHDVDependentReportBatch lobjNoHDVDependentBatch = new busNoHDVDependentReportBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjNoHDVDependentBatch.GenerateNoHDvDependentReport();
                    break;
                case 108:
                    //UCS - 095 - Retiree Annual Statement Batch
                    busRetireeAnnualStatementBatch lbusRetireeAnnualStatementBatch = new busRetireeAnnualStatementBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusRetireeAnnualStatementBatch.CreateRetireeAnnualStatements();
                    break;
                case 109:
                    //ucs - 096 : CAFR Report Generation
                    busContributionMasterReportBatch lobjContributionMasterReportBatch = new busContributionMasterReportBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjContributionMasterReportBatch.CreateReports();
                    break;
                case 110:
                    busPaymentHistoryDistributionStatusReport lbusPaymentHistoryDistributionStatusReport = new busPaymentHistoryDistributionStatusReport
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusPaymentHistoryDistributionStatusReport.GeneratePaymentHistoryDistributionStatusReport();
                    break;
                case 111:
                    busBenefitOverpaymentBillingStatementBatch lbusBenefitOverpaymentBillingStatementBatch = new busBenefitOverpaymentBillingStatementBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusBenefitOverpaymentBillingStatementBatch.GenerateBenefitOverpaymentBillingStatement();
                    break;
                case 112:
                    //UCS - 11 Addendum : Seminar Attendee Payment Allocation Batch
                    busSeminarPaymentAllocationBatch lbusSeminarPaymentAllocationBatch = new busSeminarPaymentAllocationBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusSeminarPaymentAllocationBatch.AllocateSeminarPayments();
                    break;
                case 113:
                    //Update RHIC Rate Change to Payee Account
                    busUpdateRHICRateChangeBatch lbusUpdateRHICRateChangeBatch = new busUpdateRHICRateChangeBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusUpdateRHICRateChangeBatch.UpdateRHICRateChange();
                    break;
                case 114:
                    //Enrollment changes report to employer
                    busEnrollmentChangeReportToEmployerbatch lbusEnrollmentChangeToEmployerBatch = new busEnrollmentChangeReportToEmployerbatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusEnrollmentChangeToEmployerBatch.GenerateReports();
                    iobjBatchSchedule.step_parameters = DateTime.Now.ToString();
                    break;
                case 115:
                    // UCS-054 Payee Death Letter
                    busPayeeDeathLetterBatch lobjPayeeDeathLetter = new busPayeeDeathLetterBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPayeeDeathLetter.CreatePayeeDeathLetter();
                    iobjBatchSchedule.step_parameters = DateTime.Now.ToString();
                    break;
                case 116:
                    // UCS-056 Employee Death Letter
                    busEmployeeDeathBatch lobjEmployeeDeathLetter = new busEmployeeDeathBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjEmployeeDeathLetter.GenerateCorrepondenceForEmployeeDeath();
                    break;
                case 117:
                    // UCS-011 Seminar Workflow reminder Batch
                    busSeminarWorkflowReminderBatch lobjSeminarWorkflowReminderBatch = new busSeminarWorkflowReminderBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjSeminarWorkflowReminderBatch.ProcessSeminarWorkflow();
                    break;
                case 118:
                    // UAT PIR 2043 - Non PeopleSoft Employee in Flex Comp Batch
                    busNonPeopleSoftFlexCompBatchReport lobjNonPeopleSoftFlexCompBatchReport = new busNonPeopleSoftFlexCompBatchReport
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjNonPeopleSoftFlexCompBatchReport.GenerateReportForNonPeopleSoftEmployeesInFlexComp();
                    break;
                case 119:
                    // UAT PIR 2427 
                    busAllocateRemittanceToEmployerHeader lobjAllocateRemittance = new busAllocateRemittanceToEmployerHeader
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjAllocateRemittance.AllocateRemittanceToEmployerHeader();
                    break;
                case 120:
                    // 022-Files Batch - Annual Peoplesoft File
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    lobjProcessFilesoutbound.iarrParameters = new object[1];
                    lobjProcessFilesoutbound.iarrParameters[0] = true;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(56);
                    break;
                case 121:
                    // HIPAA Superior Vision
                    busHIPAAAmeritasVision lobjSuperiorVision = new busHIPAAAmeritasVision();
                    lobjSuperiorVision.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjSuperiorVision.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjSuperiorVision.CreateHIPAAAmeritasVisionFile(true);
                    break;
                case 122:
                    // New Deferred Compensation Agent Batch Report
                    lobjDeferredCompBatchReport = new busDeferredCompBatchReport();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDeferredCompBatchReport.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDeferredCompBatchReport.iobjSystemManagement = iobjSystemManagement;
                    lobjDeferredCompBatchReport.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDeferredCompBatchReport.CreateCorrespondenceForDeferredCompAgentSeminar(true);
                    break;
                case 123:
                    // Health Premium Report
                    busHealthPremiumReport lobjHealthPremiumReport = new busHealthPremiumReport
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjHealthPremiumReport.CreateHealthPremiumReport();
                    break;
                case 124:
                    // 022-Files Batch - TIAA CREF DC Enrollment file out
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    lobjProcessFilesoutbound.iarrParameters = new object[2];
                    lobjProcessFilesoutbound.iarrParameters[0] = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1);
                    lobjProcessFilesoutbound.iarrParameters[1] = busConstant.Flag_Yes;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(84);
                    break;
                case 125:
                    // 022-Files Batch - TIAA CREF Enrollment - 457 File
                    busTIAACREF457EnrollmentFileOut lobjTIAACREF457EnrollmentFileOut = new busTIAACREF457EnrollmentFileOut
                    {

                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjTIAACREF457EnrollmentFileOut.Generate457EnrollmentFile();
                    break;
                case 126:
                    // pir-7105
                    busMaritalStatusChangeBatch lbusMaritalStatusChangeBatch = new busMaritalStatusChangeBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusMaritalStatusChangeBatch.GenerateCorrepondenceForMaritalStatusChange();
                    break;
                case 127:
                    // pir-7705
                    busEndFlexCompEnrollment lbusEndFlexCompEnrollment = new busEndFlexCompEnrollment
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusEndFlexCompEnrollment.EndFlexCompEnrollment();
                    break;
                case 128:
                    //pir 7705
                    busHSAEnrollmentBatch lobjHSAEnroll = new busHSAEnrollmentBatch();
                    lobjHSAEnroll.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjHSAEnroll.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjHSAEnroll.CreateHSAEnrollmentFile();
                    break;
                case 129:
                    // PIR 6704
                    busAnnual1099rBatch lobjTrialAnnual1099rBatch = new busAnnual1099rBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjTrialAnnual1099rBatch.ProcessTrialAnnual1099rBatch();
                    break;
                case 130:
                    busDropDependentCOBRANoticeLetterBatch lobjDropDependentCOBRANoticeLetterBatch = new busDropDependentCOBRANoticeLetterBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDropDependentCOBRANoticeLetterBatch.GenerateCorrepondenceForDropDependent();
                    break;
                case 131:
                    // HIPAA Dental DELTA
                    busHIPAADeltaDental lobjDeltaDental = new busHIPAADeltaDental();
                    lobjDeltaDental.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDeltaDental.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDeltaDental.CreateHIPAADeltaDentalFile();
                    break;
                case 132:
                    // FSA Eligibility File
                    busFSAFlexfileout lobjFSAFileout = new busFSAFlexfileout();
                    lobjFSAFileout.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjFSAFileout.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjFSAFileout.GenerateFSAFileout();
                    break;
                case 133:
                    //UCS - 091 : Monthly 1099r Reports batch
                    busMonthly1099RReportsBatch lobjMonthlyTrail1099RReportsBatch = new busMonthly1099RReportsBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMonthlyTrail1099RReportsBatch.GenerateTrialMonthly1099rReport();
                    break;
                case 134:
                    busPersonAccountPeoplesoftInterface lobjPersonAccountPeoplesoftInterface = new busPersonAccountPeoplesoftInterface
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPersonAccountPeoplesoftInterface.UpdateWSSRequestTables();
                    break;

                case 135:
                    // HIPPA Health Sanford
                    busHIPAAHealthSanford lobjSanford = new busHIPAAHealthSanford();
                    lobjSanford.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjSanford.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjSanford.CreateHIPAAHealthSanfordFile();
                    break;

                case 136:
                    //ASI Eligibility Fileout
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(99);
                    break;
                case 137:
                    //ASI Claims Fileout
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(100);
                    break;

                case 138:
                    //Missing Mandatory Plan Enrollments
                    busMissingMandatoryPlanEnrollments lobjMissingMandatoryPlanEnrollments = new busMissingMandatoryPlanEnrollments();
                    lobjMissingMandatoryPlanEnrollments.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjMissingMandatoryPlanEnrollments.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMissingMandatoryPlanEnrollments.CreateMissingMandatoryPlanEnrollmentsReport();
                    break;

                case 139:
                    //ESI Medicare Part D Enrollment Fileout
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(101);
                    break;

                case 140:
                    busWssPersonAccountEnrollmentRequest lobjWssPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjWssPersonAccountEnrollmentRequest.PostEnrollments();
                    break;

                case 141:
                    busValidateEmployerPayrollDetailBatch lobjbusValidateEmployerPayrollDetail = new busValidateEmployerPayrollDetailBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjBatchSchedule = iobjBatchSchedule,
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjbusValidateEmployerPayrollDetail.ValidateDetails();
                    break;
                case 142:
                    /// No Beneficiary Batch
                    busNoBeneficiaryLifeRetrBatch lobjNoBeneficiaryLifeRetrBatch = new busNoBeneficiaryLifeRetrBatch();
                    lobjNoBeneficiaryLifeRetrBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjNoBeneficiaryLifeRetrBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjNoBeneficiaryLifeRetrBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjNoBeneficiaryLifeRetrBatch.CreateNoBeneficiaryLifeRetrCorrespondence();
                    break;
                case 143:
                    //Seminar Confirmation Letter Batch  // PIR 6927
                    busSeminarConfirmationLetterBatch lobjbusSeminarConfirmationLetterBatch = new busSeminarConfirmationLetterBatch();
                    lobjbusSeminarConfirmationLetterBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjbusSeminarConfirmationLetterBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjbusSeminarConfirmationLetterBatch.iobjBatchSchedule = iobjBatchSchedule;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjbusSeminarConfirmationLetterBatch.GenerateCorrepondenceForSeminarConfirmationLetter();
                    break;
                case 144:
                    //Waiver of Life Expiring  // PIR 9347
                    busWaiverOfLifeExpiringBatch lobjbusWaiverOfLifeExpiringBatch = new busWaiverOfLifeExpiringBatch();
                    lobjbusWaiverOfLifeExpiringBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjbusWaiverOfLifeExpiringBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjbusWaiverOfLifeExpiringBatch.iobjBatchSchedule = iobjBatchSchedule;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjbusWaiverOfLifeExpiringBatch.ProcessRecordsForWaiverOfLifeExpiring();
                    break;
                case 145:
                    //Voya Port Fileout
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(102);
                    break;
                case 146:
                    //PIR-18492 : Send Email Notification              
                    busEmailNotificationBatch lobjEmailNotificationBatch = new busEmailNotificationBatch
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjSystemManagement = iobjSystemManagement,
                        iobjBatchSchedule = iobjBatchSchedule
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjEmailNotificationBatch.SendEmailNotication();
                    lobjEmailNotificationBatch.SendESSEmailNotification();
                    break;
                case 147:                
                    //WSS message request PIR-17066               
                    busWssMessageBatch lobjWssMessageBatch = new busWssMessageBatch();
                    lobjWssMessageBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjWssMessageBatch.iobjSystemManagement = iobjSystemManagement;
                    lobjWssMessageBatch.iobjBatchSchedule = iobjBatchSchedule;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjWssMessageBatch.PublishMessageToSelectedAudiencePerWssRequest();
                    break;
				case 148:
                    busSuspendBenefitsUncashedChecks lobjSuspendBenefitsUncashedChecks = new busSuspendBenefitsUncashedChecks();
                    lobjSuspendBenefitsUncashedChecks.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjSuspendBenefitsUncashedChecks.iobjSystemManagement = iobjSystemManagement;
                    lobjSuspendBenefitsUncashedChecks.iobjBatchSchedule = iobjBatchSchedule;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjSuspendBenefitsUncashedChecks.SuspendBenefitsUncashedChecks();
                    break;
                case 149:
                    //PIR 17081 - Life Age change for Benefit Enrollment report
                    busLifeAgeChangeBatch lobjLifeAgeChange = new busLifeAgeChangeBatch();
                    lobjLifeAgeChange.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjLifeAgeChange.iobjSystemManagement = iobjSystemManagement;
                    lobjLifeAgeChange.iobjBatchSchedule = iobjBatchSchedule;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjLifeAgeChange.LoadLifeAgeChangeMembers();
                    break;
                case 150:
                    /// PIR - 20868 - Need Confirmation Letter sent when Address is updated on MSS
                    busConfirmationOfAddressChange lobjConfirmationOfAddressChange = new busConfirmationOfAddressChange();
                    lobjConfirmationOfAddressChange.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjConfirmationOfAddressChange.iobjBatchSchedule = iobjBatchSchedule;
                    lobjConfirmationOfAddressChange.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjConfirmationOfAddressChange.CreateAddressChangeCorrespondence();
                    break;
                case 151:
                    busDeleteSessionStoreBatch lbusDeleteSessionStoreBatch = new busDeleteSessionStoreBatch();
                    lbusDeleteSessionStoreBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lbusDeleteSessionStoreBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lbusDeleteSessionStoreBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusDeleteSessionStoreBatch.DeleteSessionStore();
                    break;
                case 152:
                    busPrintingBatchLetter lbusPrintingBatchLetter = new busPrintingBatchLetter();
                    lbusPrintingBatchLetter.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lbusPrintingBatchLetter.iobjBatchSchedule = iobjBatchSchedule;
                    lbusPrintingBatchLetter.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusPrintingBatchLetter.GetAllLettersToPrint();
                    break;
                case 153:
                    //Flex File to Sanford Fileout
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(104);
                    break;
                case 155:
                    //Generate COBRA Letters for Change Reason INEM 
                    busNonPayeeReasonValueINEMCOBRANotices lobjNonPayeeReasonValueINEMCOBRANotices = new busNonPayeeReasonValueINEMCOBRANotices();
                    lobjNonPayeeReasonValueINEMCOBRANotices.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjNonPayeeReasonValueINEMCOBRANotices.iobjBatchSchedule = iobjBatchSchedule;
                    lobjNonPayeeReasonValueINEMCOBRANotices.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjNonPayeeReasonValueINEMCOBRANotices.GenerateCorrespondenceForNonPayeeReasonValueINEMCOBRANotices();
                    break;
                case 156:
                    //Batch to initiate workflow for Line of Duty Survivors
                    busLineOfDutySurvivorHealth lobjLineOfDutySurvivours = new busLineOfDutySurvivorHealth();
                    lobjLineOfDutySurvivours.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjLineOfDutySurvivours.iobjBatchSchedule = iobjBatchSchedule;
                    lobjLineOfDutySurvivours.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjLineOfDutySurvivours.GenerateWorkflowForLineOfDutySurvivours();
                    break;
                case 157:
                    //New Hire Auto-Plan Enrollment
                    busPostNewHire lobjPostNewHire = new busPostNewHire();
                    lobjPostNewHire.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjPostNewHire.iobjBatchSchedule = iobjBatchSchedule;
                    lobjPostNewHire.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPostNewHire.ProcessNewHire();
                    break;
                case 158:
                    //ACH Pull Automation - Retirement
                    busACHPullAutomation lobjACHPullForRetirement = new busACHPullAutomation();
                    lobjACHPullForRetirement.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjACHPullForRetirement.iobjBatchSchedule = iobjBatchSchedule;
                    lobjACHPullForRetirement.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjACHPullForRetirement.CreateACHPull(busConstant.PayrollHeaderBenefitTypeRtmt);
                    break;
                case 159:
                    //ACH Pull Automation - Deferred Comp
                    busACHPullAutomation lobjACHPullForDeferredComp = new busACHPullAutomation();
                    lobjACHPullForDeferredComp.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjACHPullForDeferredComp.iobjBatchSchedule = iobjBatchSchedule;
                    lobjACHPullForDeferredComp.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjACHPullForDeferredComp.CreateACHPull(busConstant.PayrollHeaderBenefitTypeDefComp);
                    break;
                case 160:
                    //ACH Pull Automation - Insurance
                    busACHPullAutomation lobjACHPullForInsurance = new busACHPullAutomation();
                    lobjACHPullForInsurance.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjACHPullForInsurance.iobjBatchSchedule = iobjBatchSchedule;
                    lobjACHPullForInsurance.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjACHPullForInsurance.CreateACHPull(busConstant.PayrollHeaderBenefitTypeInsr);
                    break;
                case 161:
                    //ACH Pull Automation - IBS
                    busACHPullAutomation lobjACHPullForIBS = new busACHPullAutomation();
                    lobjACHPullForIBS.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjACHPullForIBS.iobjBatchSchedule = iobjBatchSchedule;
                    lobjACHPullForIBS.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjACHPullForIBS.CreateACHPull(ablnIsIBSPull: true);
                    break;
                case 162:
                    //Annual 1099R Payment File Batch
                    busAnnual1099RPaymentFileBatch lobjAnnual1099RPaymentFile = new busAnnual1099RPaymentFileBatch() { ibusAnnual1099rBatch = new busAnnual1099rBatch()};
                    lobjAnnual1099RPaymentFile.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjAnnual1099RPaymentFile.ibusAnnual1099rBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjAnnual1099RPaymentFile.iobjBatchSchedule = iobjBatchSchedule;
                    lobjAnnual1099RPaymentFile.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjAnnual1099RPaymentFile.Create1099RReportAndFiles();
                    break;
                case 163:
                    //Missing RHIC Batch
                    busMissingRHIC lobjMissingRHIC = new busMissingRHIC();
                    lobjMissingRHIC.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjMissingRHIC.iobjBatchSchedule = iobjBatchSchedule;
                    lobjMissingRHIC.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjMissingRHIC.GenerateWorkflowForProcessMissingRHICRecord();
                    break;               
                case 165:
                    //  Empower Enrollment - 457 File
                    busEmpower457EnrollmentFileOut lobjEmpower457EnrollmentFile = new busEmpower457EnrollmentFileOut
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjEmpower457EnrollmentFile.Generate457EnrollmentFile();
                    break;
                case 166:
                    // Empower DC Enrollment file out
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.idlgUpdateProcessLog = new busProcessOutboundFile.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    lobjProcessFilesoutbound.iarrParameters = new object[2];
                    lobjProcessFilesoutbound.iarrParameters[0] = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1);
                    lobjProcessFilesoutbound.iarrParameters[1] = busConstant.Flag_Yes;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(107);
                    break;
                case 167:
                    // Contact Expired
                    busContactExpired lobjContactExpired = new busContactExpired
                    {
                        idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog),
                        iobjSystemManagement = iobjSystemManagement
                    };
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjContactExpired.ContactExpiring();
                    break;

                case 168:
                    //TFFR SSN Match
                    busPersonTFFR lobjPersonTFFR = new busPersonTFFR();
                    lobjPersonTFFR.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjPersonTFFR.iobjBatchSchedule = iobjBatchSchedule;
                    lobjPersonTFFR.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFiles = new busProcessFiles();
                    lobjProcessFiles.iobjSystemManagement = iobjSystemManagement;
                    lobjProcessFiles.idlgUpdateProcessLog = new busProcessFiles.UpdateProcessLog(UpdateProcessLog);
                    lobjProcessFiles.UploadFiles(118);
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPersonTFFR.GenerateDualMemberRequestFile();
                    break;
                case 169:
                    lobjProcessFilesoutbound = new busProcessOutboundFile();
                    lobjProcessFilesoutbound.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessFilesoutbound.CreateOutboundFile(116);
                    break;
				case 180:
                    // PIR 25920 DC 2025 Changes HB1040 Comparison Letter
                    busHB1040ComparisonLetter lbusHB1040ComparisonLetter = new busHB1040ComparisonLetter();
                    lbusHB1040ComparisonLetter.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lbusHB1040ComparisonLetter.iobjBatchSchedule = iobjBatchSchedule;
                    lbusHB1040ComparisonLetter.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusHB1040ComparisonLetter.ProcessHB1040ComparisonLetter();
                    break;
                case 181:
                    // PIR 25920 DC 2025 Changes HB1040 Reminder Letter
                    busHB1040ReminderLetter lbusHB1040ReminderLetter = new busHB1040ReminderLetter();
                    lbusHB1040ReminderLetter.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lbusHB1040ReminderLetter.iobjBatchSchedule = iobjBatchSchedule;
                    lbusHB1040ReminderLetter.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusHB1040ReminderLetter.ProcessHB1040ReminderLetter();
                    break;
                case 182:
                    // PIR 27023 Create a reminder batch letter to initiate 14 days after DC2025 enrollment
                    busDC25ReminderLetterBatch lbusDC25ReminderLetterBatch = new busDC25ReminderLetterBatch();
                    lbusDC25ReminderLetterBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lbusDC25ReminderLetterBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lbusDC25ReminderLetterBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lbusDC25ReminderLetterBatch.ProcessDC25ReminderLetterBatch();
                    break;
                case 164:
                    //Update Spouse Contact Status and member Marital Status and MS Change Date
                    busDeceasedStatusChange lobjDeceasedStatusChange = new busDeceasedStatusChange();
                    lobjDeceasedStatusChange.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjDeceasedStatusChange.iobjBatchSchedule = iobjBatchSchedule;
                    lobjDeceasedStatusChange.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjDeceasedStatusChange.UpdateDeseasedMemberDetails();
                    break;
                case 183:
                    //Payment - Pull Check Update Batch
                    busPayeeAccountNightlyBatch lobjPayeeAccountNightlyBatch = new busPayeeAccountNightlyBatch();
                    lobjPayeeAccountNightlyBatch.idlgUpdateProcessLog = new busNeoSpinBatch.UpdateProcessLog(UpdateProcessLog);
                    lobjPayeeAccountNightlyBatch.iobjBatchSchedule = iobjBatchSchedule;
                    lobjPayeeAccountNightlyBatch.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjPayeeAccountNightlyBatch.UpdatePayeeAccountPullFlag();
                    break;
            }
        }
        public bool SkipStep()
        {
            bool lblnSkip = false;

            if (iobjBatchSchedule.run_in_batch_flag == "N")
            {
                iblnSkipEntryForNotification = true;
                UpdateProcessLog("Skipped due to Batch Flag is false ", "SUMM", iobjBatchSchedule);
                lblnSkip = true;
            }

            if (iobjBatchSchedule.active_flag != "Y")
            {
                iblnSkipEntryForNotification = true;
                UpdateProcessLog("Skipped due to inactive status ", "SUMM", iobjBatchSchedule);
                lblnSkip = true;
            }

            if (iobjBatchSchedule.next_run_date > iobjSystemManagement.icdoSystemManagement.batch_date)
            {
                iblnSkipEntryForNotification = true;
                UpdateProcessLog("Did not run due to future next run date", "SUMM", iobjBatchSchedule);
                lblnSkip = true;
            }
            return lblnSkip;
        }

        public void InitializeStep()
        {
            try
            {
                //Current cycle no
                iintCurrentCycleNo = iobjSystemManagement.icdoSystemManagement.current_cycle_no;

                iblnSkipEntryForNotification = true;
                UpdateProcessLog("Initialize step .. Starting new cycle ", "SUMM", "Initialize step");
                //Start the new cycle
                iintCurrentCycleNo++;

                //Fire the query to populate the beginning of cycle data
                iobjSystemManagement.icdoSystemManagement.current_cycle_no = iintCurrentCycleNo;
                iobjSystemManagement.icdoSystemManagement.batch_date = DateTime.Now.Date;
                iobjSystemManagement.icdoSystemManagement.Update();
                iblnSkipEntryForNotification = true;
                UpdateProcessLog("Initialize step .. New cycle started ", "SUMM");
            }
            catch (Exception e)
            {
                throw new Exception("Following error occurred in InitializeStep : " + e.Message);
            }
        }

        public void FinishStep()
        {
            GC.Collect();
            //?Send email that this step is done
            if (utlPassInfo.iobjPassInfo.iconFramework.ConnectionString.IsNullOrEmpty())
                utlPassInfo.iobjPassInfo.iconFramework.ConnectionString = DBFunction.GetDBConnection().ConnectionString;
            iobjPassInfo.BeginTransaction();
            try
            {
                //Days is -1 means the process itself has set the frequency and the update needs to happen

                //Days and months frequency = 0 means that they can be reun several times during the day
                if ((iobjBatchSchedule.frequency_in_days == 0) && (iobjBatchSchedule.frequency_in_months == 0))
                {
                    iobjBatchSchedule.next_run_date = iobjSystemManagement.icdoSystemManagement.batch_date;
                }

                if (iobjBatchSchedule.frequency_in_days > 0)
                {
                    iobjBatchSchedule.next_run_date =
                        iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(iobjBatchSchedule.frequency_in_days);
                }

                if (iobjBatchSchedule.frequency_in_months > 0)
                {
                    //As per RAJ Comments Emailed on 10/23/2010
                    if (iobjSystemManagement.icdoSystemManagement.batch_date.Day >= 28)
                    {
                        iobjBatchSchedule.next_run_date = iobjSystemManagement.icdoSystemManagement.batch_date.AddMonths(iobjBatchSchedule.frequency_in_months).GetLastDayofMonth();
                    }
                    else
                    {
                        iobjBatchSchedule.next_run_date = iobjSystemManagement.icdoSystemManagement.batch_date.AddMonths(iobjBatchSchedule.frequency_in_months);
                    }
                }
                iobjBatchSchedule.Update();
                iobjPassInfo.Commit();
            }
            catch (Exception e)
            {
                iobjPassInfo.Rollback();
                MessageBox.Show("Following error occurred in FinishStep : " + e.Message);
                throw new Exception("Following error occurred in FinishStep : " + e.Message);
            }
        }

        public void UpdateProcessLog(string astrMessage, string astrMessageType)
        {
            iobjPassInfoLog.BeginTransaction();
            try
            {
                DBFunction.StoreProcessLog(iintCurrentCycleNo, "PERSLinkBatch", astrMessageType, astrMessage, istrUserId,
                    iobjPassInfoLog.iconFramework, iobjPassInfoLog.itrnFramework);
                iobjPassInfoLog.Commit();
            }
            catch (Exception e)
            {
                iobjPassInfoLog.Rollback();
                MessageBox.Show("UpdateProcessLog failed with following error : " + e.Message);
            }
            //DisplayProgress();
            UpdateDisplay("PERSLInk Batch", astrMessageType, astrMessage);
            AddToNotificationMessage("PERSLInk Batch", astrMessageType, astrMessage);
        }

        public void UpdateProcessLog(string astrMessage, string astrMessageType, cdoBatchSchedule aobjBatchSchedule)
        {
            iobjPassInfoLog.BeginTransaction();
            try
            {
                DBFunction.StoreProcessLog(iintCurrentCycleNo, aobjBatchSchedule.step_name, astrMessageType, astrMessage, istrUserId,
                    iobjPassInfoLog.iconFramework, iobjPassInfoLog.itrnFramework);
                iobjPassInfoLog.Commit();
            }
            catch (Exception e)
            {
                iobjPassInfoLog.Rollback();
                MessageBox.Show("UpdateProcessLog failed with following error : " + e.Message);
            }
            //DisplayProgress();
            UpdateDisplay(aobjBatchSchedule.step_name, astrMessageType, astrMessage);
            AddToNotificationMessage(aobjBatchSchedule.step_name, astrMessageType, astrMessage);
        }

        public void UpdateProcessLog(string astrMessage, string astrMessageType, string astrStepName)
        {
            iobjPassInfoLog.BeginTransaction();
            try
            {
                DBFunction.StoreProcessLog(iintCurrentCycleNo, astrStepName, astrMessageType, astrMessage, istrUserId,
                    iobjPassInfoLog.iconFramework, iobjPassInfoLog.itrnFramework);
                iobjPassInfoLog.Commit();
            }
            catch (Exception e)
            {
                iobjPassInfoLog.Rollback();
                MessageBox.Show("UpdateProcessLog failed with following error : " + e.Message);
            }
            //DisplayProgress();
            UpdateDisplay(astrStepName, astrMessageType, astrMessage);
            AddToNotificationMessage(astrStepName, astrMessageType, astrMessage);
        }

        private void DisplaySummaryProgress(string astrDateTime, string astrStepName, string astrType, string astrMessage)
        {
            string[] newRow = { astrDateTime, astrStepName, astrType, astrMessage };
            grvSummary.Rows.Add(newRow);
            grvSummary[0, grvSummary.Rows.Count - 1].Selected = true;
            Application.DoEvents();
        }

        private void DisplayDetailProgress(string astrDateTime, string astrStepName, string astrType, string astrMessage)
        {
            string[] newRow = { astrDateTime, astrStepName, astrType, astrMessage };
            grvDetail.Rows.Add(newRow);
            grvDetail[0, grvDetail.Rows.Count - 1].Selected = true;
            Application.DoEvents();
        }

        private void UpdateDisplay(string astrStepName, string astrMessageType, string astrMessage)
        {
            if (astrMessageType == "SUMM")
            {
                if ((grvSummary.InvokeRequired) && (grvSummary.IsHandleCreated))
                {
                    grvSummary.Invoke(new DisplaySummaryMessage(DisplaySummaryProgress),
                                          new object[4] { DateTime.Now.ToString(), astrStepName, astrMessageType, astrMessage });
                }
                else
                {
                    DisplaySummaryProgress(DateTime.Now.ToString(), astrStepName, astrMessageType, astrMessage);
                }
            }

            if ((grvDetail.InvokeRequired) && (grvDetail.IsHandleCreated))
            {
                grvDetail.Invoke(new DisplayDetailMessage(DisplayDetailProgress),
                                      new object[4] { DateTime.Now.ToString(), astrStepName, astrMessageType, astrMessage });
            }
            else
            {
                DisplayDetailProgress(DateTime.Now.ToString(), astrStepName, astrMessageType, astrMessage);
            }

        }

        private void AddToNotificationMessage(string astrStepName, string astrMessageType, string astrMessage)
        {
            if (!iblnSkipEntryForNotification)
                ilstNotificationMessages.Add(DateTime.Now.ToString() + " - " + astrStepName + " - " + astrMessageType + " - " + astrMessage);
            iblnSkipEntryForNotification = false;
            if ((astrMessageType == "ERR") || (astrMessageType == "ERRO"))
                iblnErrorEntryFound = true;
        }

        private void DisplayProgress()
        {
            istrSummaryQuery = istrSummaryQuery.Replace("@cycle_no", iintCurrentCycleNo.ToString());
            grvSummary.DataSource = bsrSummary;
            grvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            DataTable ldtbSummary = DBFunction.DBSelect(istrSummaryQuery, iobjPassInfoLog.iconFramework);
            bsrSummary.DataSource = ldtbSummary.DefaultView;
            grvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            grvSummary.Refresh();

            istrDetailQuery = istrDetailQuery.Replace("@cycle_no", iintCurrentCycleNo.ToString());
            grvDetail.DataSource = bsrDetail;
            grvDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            DataTable ldtbDetail = DBFunction.DBSelect(istrDetailQuery, iobjPassInfoLog.iconFramework);
            bsrDetail.DataSource = ldtbDetail.DefaultView;
            grvDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            grvDetail.Refresh();
            this.ResumeLayout(true);
        }
        private void frmNeoSpinBatch_FormClosing(object sender, FormClosingEventArgs e)
        {
            _ibusRequestHandler?.StopProcessing(); //PIR 14770
            if (busNeoSpinBatch.iobjCorBuilder != null) busNeoSpinBatch.iobjCorBuilder.CloseWord();
            if (busNeoSpinBatch.WordApp != null) busNeoSpinBatch.WordApp.Quit(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges); //PIR 15529
            iobjPassInfo.iconFramework.Close();
            iobjPassInfoLog.iconFramework.Close();
        }

        private void TimerProcessMessages_Tick(object sender, EventArgs e)
        {
            Application.DoEvents();
        }

        public void SetStepIndexFromService()
        {
            iintStepIndex = cmbStartStep.SelectedIndex;
        }

        /// <summary>
        /// method to check whether uploaded file contain all columns
        /// </summary>
        private void CheckFileInProperFormat(busProcessFiles aobjProcessFiles)
        {
            busBase lobjBase = new busBase();
            busFile lobjFile;
            string lstrTransactionCodeValue = string.Empty;
            //load file header which are uploaded
            DataTable ldtFileHdr = busBase.Select<cdoFileHdr>(new string[1] { "status_value" },
                                                                new object[1] { busConstant.FileHdrStatusUpload }, null, null);
            Collection<busFileHdr> lclbFileHdr = new Collection<busFileHdr>();
            lclbFileHdr = lobjBase.GetCollection<busFileHdr>(ldtFileHdr, "icdoFileHdr");
            foreach (busFileHdr lobjHdr in lclbFileHdr)
            {
                //loading the file details
                DataTable ldtFileDtls = busBase.Select<cdoFileDtl>(new string[1] { "file_hdr_id" },
                                                                new object[1] { lobjHdr.icdoFileHdr.file_hdr_id }, null, null);
                //loading the file object
                lobjFile = new busFile();
                lobjFile.FindFile(lobjHdr.icdoFileHdr.file_id);
                if (lobjFile.icdoFile.file_id > 0 && !string.IsNullOrEmpty(lobjFile.icdoFile.delimited_by))
                {
                    //method to load the file layout
                    utlFileLayout lfileLayout = iobjPassInfo.isrvMetaDataCache.GetFileLayout(lobjFile.icdoFile.xml_layout_file);

                    foreach (DataRow ldrDtl in ldtFileDtls.Rows)
                    {
                        lstrTransactionCodeValue = ldrDtl["transaction_code_value"] == DBNull.Value ? "None" : ldrDtl["transaction_code_value"].ToString();
                        if (ldrDtl["record_data"] != DBNull.Value)
                        {
                            string[] lstrRecordData = ldrDtl["record_data"].ToString().Split(lobjFile.icdoFile.delimited_by);
                            //to load the record layout of the file based on transaction code value
                            utlRecordLayout lfileRecordLayout = lfileLayout.FindFileLayout(lstrTransactionCodeValue);
                            //PIR 25920 - HB 1040 New Plan DC 2025 override/skip file column count error for DC25 additional fields
                            bool lblnValidFile = false; 
                            if (lfileRecordLayout != null && lfileRecordLayout.iclbRecordField != null && lstrRecordData.Length != lfileRecordLayout.iclbRecordField.Count())
                            {
                                if (lobjHdr.icdoFileHdr.file_id == busConstant.DefCompFileId)
                                {
                                    if(lstrRecordData.Length == lfileRecordLayout.iclbRecordField.Count() - 2)
                                        lblnValidFile = true;
                                }
                                if (lstrRecordData.Length > 0 && lobjHdr.icdoFileHdr.file_id == busConstant.RetirementInboundFileId)
                                {
                                    if (lstrRecordData[0].IsNotNullOrEmpty() && lstrRecordData[0].Trim() == "1" && lstrRecordData.Length == lfileRecordLayout.iclbRecordField.Count() - 1)
                                        lblnValidFile = true;
                                    if (lstrRecordData[0].IsNotNullOrEmpty() && lstrRecordData[0].Trim() == "2" && lstrRecordData.Length == lfileRecordLayout.iclbRecordField.Count() - 4)
                                        lblnValidFile = true;
                                }
                                if (!lblnValidFile)
                                {
                                    //if invalid format, updating header status and posting message
                                    lobjHdr.icdoFileHdr.status_value = busConstant.FileHdrStatusReview;
                                    lobjHdr.icdoFileHdr.error_message = "Invalid format, inconsistent number of fields, record contains " + lstrRecordData.Length.ToString() +
                                        " , record layout contains " + lfileRecordLayout.iclbRecordField.Count.ToString();
                                    lobjHdr.icdoFileHdr.Update();
                                    //Exception e = new Exception("Please contact IR. File upload error. FileHeader Key=" + lobjHdr.icdoFileHdr.file_hdr_id);
                                    //throw e;
                                    aobjProcessFiles.idlgUpdateProcessLog("Please contact IR. File upload error. FileHeader Key=" + lobjHdr.icdoFileHdr.file_hdr_id, "INFO", "Upload Files");
                                }
                            }
                        }
                    }
                }
                //PIR 7202 - If file hdr goes into review, means no header and detail will be created, so we need to publish a message to employer
                //so that he knows no header is created and there is something wrong with the file.
                if ((lobjHdr.icdoFileHdr.reference_id > 0 && lobjHdr.icdoFileHdr.status_value == busConstant.FileHdrStatusReview) &&
                    (lobjHdr.icdoFileHdr.file_id == busConstant.RetirementInboundFileId || lobjHdr.icdoFileHdr.file_id == busConstant.DefCompFileId ||
                    lobjHdr.icdoFileHdr.file_id == busConstant.PurchaseFileId))
                {
                    string lstrPrioityValue = string.Empty;
                    busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(6, iobjPassInfo, ref lstrPrioityValue),
                             lobjHdr.icdoFileHdr.mailbox_file_name), lstrPrioityValue, aintOrgID: lobjHdr.icdoFileHdr.reference_id);
                    busGlobalFunctions.MoveFileFromProcessedToErrorFolder(lobjHdr.icdoFileHdr, iobjPassInfo);
                }
            }
        }
        public void AppendBatchScheduleID()
        {
            istrUserId = null;
            istrUserId = "PERSLink Batch" + ' ' + iobjBatchSchedule.batch_schedule_id;
            utlPassInfo.iobjPassInfo.istrUserID = istrUserId;
        }
    }
}
