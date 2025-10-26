#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using System.Linq;
using Sagitec.ExceptionPub;
#endregion

namespace NeoSpinBatch
{
    public class AdhocProcessTrialReportBatch : busNeoSpinBatch
    {
        busBase lobjBase = new busBase();
        string lstrReportPrefixPaymentScheduleID = string.Empty;
        busPaymentProcess lobjPaymentProcess = new busPaymentProcess();
        //Load the Next valid Adhoc payment schedule into the property ibusPaymentSchedule
        public busPaymentSchedule ibusPaymentSchedule { get; set; }
        public Collection<busPaymentScheduleStep> iclbProcessedSteps { get; set; }
        public void LoadPaymentSchedule()
        {
            if (ibusPaymentSchedule == null) ibusPaymentSchedule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
            DataTable ldtbPaymentSchedule = busBase.SelectWithOperator<cdoPaymentSchedule>
                                   (
                                      new string[4] { "schedule_type_value", "status_value", "action_status_value", "payment_date" },
                                      new string[4] { "=", "=", "=", "<=" },
                                      new object[4] {busConstant.PaymentScheduleScheduleTypeAdhoc,busConstant.PaymentScheduleStatusValid,
                                                     busConstant.PaymentScheduleActionStatusPending,DateTime.Now}, null);
            if (ldtbPaymentSchedule.Rows.Count > 0)
                ibusPaymentSchedule.icdoPaymentSchedule.LoadData(ldtbPaymentSchedule.Rows[0]);
            else
                CreateAdhocSchedule();
        }
        //if there is no Achoc payment exists for the batch run date ,create a achoc payment schedule and run it
        public void CreateAdhocSchedule()
        {
            ibusPaymentSchedule.icdoPaymentSchedule.payment_date = iobjSystemManagement.icdoSystemManagement.batch_date;
            ibusPaymentSchedule.icdoPaymentSchedule.process_date = DateTime.Now;
            ibusPaymentSchedule.icdoPaymentSchedule.schedule_type_value = busConstant.PaymentScheduleScheduleTypeAdhoc;
            ibusPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleStatusValid;
            ibusPaymentSchedule.icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusReadyforFinal;
            ibusPaymentSchedule.icdoPaymentSchedule.Insert();
            ibusPaymentSchedule.CreatePaymentSteps(false);
        }
        public void RunTrialReport()
        {
            istrProcessName = "Adhoc Payment Batch";
            bool lblnStepFailedIndicator = false;
            if (ibusPaymentSchedule == null)
                LoadPaymentSchedule();
            busCreateReports lobjCreateReports = new busCreateReports();
            DataTable ldtReportResult;
            //Load adhoc batch schedule steps
            if (ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id > 0)
            {
                lstrReportPrefixPaymentScheduleID = ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id + "_";
                if (lobjPaymentProcess.iclbBatchScheduleSteps == null)
                    lobjPaymentProcess.LoadBatchScheduleSteps(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                //Load payment Step details
                lobjPaymentProcess.LoadBatchScheduleStepRef(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                // Run the bacth only for the steps which are having trial run flag as 'YES'
                foreach (busPaymentScheduleStep lobjPaymentScheduleStep in
                    lobjPaymentProcess.iclbBatchScheduleSteps.Where(o => o.ibusPaymentStep.icdoPaymentStepRef.trial_run_flag == busConstant.Flag_Yes))
                {
                    switch (lobjPaymentScheduleStep.ibusPaymentStep.icdoPaymentStepRef.run_sequence)
                    {
                        case 1760:
                            try
                            {
                                idlgUpdateProcessLog("Monthly Benefit Payment by Item Report", "INFO", istrProcessName);
                                ldtReportResult = new DataTable();
                                ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentbyItemReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date, false);
                                if (ldtReportResult.Rows.Count > 0)
                                {
                                    CreateReportWithPrefix("rptMonthlyBenefitPaymentbyItem.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID);
                                    idlgUpdateProcessLog("Monthly Benefit Payment by Item Report generated succesfully", "INFO", istrProcessName);
                                }
                                else
                                {
                                    idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                                }
                                lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                            }
                            catch (Exception e)
                            {
                                ExceptionManager.Publish(e);
                                idlgUpdateProcessLog("Monthly Benefit Payment by Item Report Failed.", "INFO", istrProcessName);
                                lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                lblnStepFailedIndicator = true;
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
                                    CreateReportWithPrefix("rptVendorPaymentSummary.rpt", ldtReportResult, lstrReportPrefixPaymentScheduleID);
                                    idlgUpdateProcessLog("Vendor Payment Trial Summary Report generated succesfully", "INFO", istrProcessName);
                                }
                                else
                                {
                                    idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                                }
                                lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                            }
                            catch (Exception e)
                            {
                                ExceptionManager.Publish(e);
                                idlgUpdateProcessLog("Vendor Payment Trial Summary Report Failed.", "INFO", istrProcessName);
                                lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                lblnStepFailedIndicator = true;
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
                                    CreateReportWithPrefix("rptNonMonthlyPaymentDetail.rpt", ldtReportResult,lstrReportPrefixPaymentScheduleID);
                                }
                                lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                                idlgUpdateProcessLog("Payee List Report generated succesfully", "INFO", istrProcessName);
                            }
                            catch (Exception e)
                            {
                                ExceptionManager.Publish(e);
                                idlgUpdateProcessLog("Payee List Report Failed.", "INFO", istrProcessName);
                                lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                                lblnStepFailedIndicator = true;
                            }
                            break;
                    }
                    lobjPaymentScheduleStep.icdoPaymentScheduleStep.Update();
                }
                if (lblnStepFailedIndicator)
                {
                    ibusPaymentSchedule.icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusFailed;
                    idlgUpdateProcessLog("Adhoc Trial Report Batch failed", "INFO", istrProcessName);
                }
                else
                {
                    ibusPaymentSchedule.icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusTrialExecuted;
                  
                    idlgUpdateProcessLog("Adhoc Trial Report Batch excuted succesfully", "INFO", istrProcessName);
                }
                ibusPaymentSchedule.icdoPaymentSchedule.Update();
            }
        }
    }
}
