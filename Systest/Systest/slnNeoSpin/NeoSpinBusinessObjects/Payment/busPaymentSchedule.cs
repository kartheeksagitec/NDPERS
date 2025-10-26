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
using System.Collections.Generic;
using Sagitec.ExceptionPub;
using static Sagitec.BusinessObjects.busProcessFiles;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPaymentSchedule : busPaymentScheduleGen
    {
        public busPaymentMonthlyBenefitSummary ibusPaymentMonthlyBenefitSummary { get; set; }

        //this Flag  is to identify whether the current mode of screen is new or update
        private bool iblnIsNewMode = false;
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (aenmPageMode == utlPageMode.New)
            {
                iblnIsNewMode = true;
            }
            base.BeforeValidate(aenmPageMode);
        }
        //Load Next Benefit Payment Date
        public DateTime idtNextBenefitPaymentDate { get; set; }
        public void LoadNexBenefitPaymentDate()
        {
            idtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
        }

        //this method will be called when the user clciks on Run Trial Reports button in payment schedule maitnenance screen
        public ArrayList btnRunTrialReports_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            int lintrtn = 0;
            //if schedule type is Adhoc then Payment date should be equal to Today's date
            if (icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeAdhoc
                && !IsPaymentDateSameAsToday())
            {
                lobjError = AddError(6431, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            if (iclbPaymentScheduleStep == null)
                LoadPaymentScheduleSteps();

            List<busPaymentScheduleStep> lclbPaymentSteps = new List<busPaymentScheduleStep>();
            lclbPaymentSteps = iclbPaymentScheduleStep.Where(o => o.ibusPaymentStep.icdoPaymentStepRef.schedule_type_value == icdoPaymentSchedule.schedule_type_value
                                                        && o.ibusPaymentStep.icdoPaymentStepRef.trial_run_flag == busConstant.Flag_Yes).ToList();
            foreach (busPaymentScheduleStep lobjPaymentStep in lclbPaymentSteps)
            {
                lobjPaymentStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusPending;
                lobjPaymentStep.icdoPaymentScheduleStep.Update();
            }
            //DBFunction.StoreProcessLog(0,"Test", "INFO", "Payment Steps count is" + lclbPaymentSteps.Count(), iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            lintrtn = RunTrialReports(lclbPaymentSteps);
            //Update the status only if all reports executed successfully
            if (lintrtn != -1)
            {
                busPaymentScheduleStep lobjTrialReportStep = lclbPaymentSteps
                                                                .Where(o => o.icdoPaymentScheduleStep.run_sequence == 200).FirstOrDefault();
                if (lobjTrialReportStep != null)
                {
                    lobjTrialReportStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                    lobjTrialReportStep.icdoPaymentScheduleStep.Update();
                }

                icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusTrialExecuted;
                icdoPaymentSchedule.Update();
            }
            alReturn.Add(this);
            return alReturn;
        }

        //Property to contain minimum guarantee amount for New Payees
        public decimal idecMinimumGuaranteeNewPayees { get; set; }
        //Property to contain minimum guarantee amount for Cancelled or Payments Complete Payees
        public decimal idecMinimumGuaranteeCancelledorPaymentsCompletePayees { get; set; }
        /// <summary>
        /// Method to run Create Trial reports
        /// </summary>
        /// <param name="aclbPaymentSteps">Payment step ref </param>
        /// <returns>int value</returns>
        private int RunTrialReports(List<busPaymentScheduleStep> aclbPaymentSteps)
        {
            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Run trail reports", iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            busCreateReports lobjCreateReports = new busCreateReports();
            DataTable ldtReportResult;
            DataSet ldsReportResult;
            busNeoSpinBase lobjNSBase = new busNeoSpinBase();
            int lintrtn = 1;
            foreach (busPaymentScheduleStep lobjPaymentStep in aclbPaymentSteps)
            {
                //DBFunction.StoreProcessLog(0, "Test", "INFO", "Run trail reports "+ lobjPaymentStep.icdoPaymentScheduleStep.run_sequence, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                switch (lobjPaymentStep.icdoPaymentScheduleStep.run_sequence)
                {
                    case 210:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentbyItemReport(icdoPaymentSchedule.payment_date, true);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptMonthlyBenefitPaymentbyItem.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                //PIR-10808 PDF Reports converted into Excel format                                                               
                                lobjNSBase.CreateExcelReport("rptMonthlyBenefitPaymentbyItem.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                    case 1760:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentbyItemReport(icdoPaymentSchedule.payment_date, false);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptMonthlyBenefitPaymentbyItem.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);                                
                                //PIR-10808 PDF Reports converted into Excel format                                                                
                                lobjNSBase.CreateExcelReport("rptMonthlyBenefitPaymentbyItem.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                    case 220:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialNewRetireeDetailReport(icdoPaymentSchedule.payment_date);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptNewPayeeDetail.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                //PIR-10808 PDF Reports converted into new Excel format                                                                
                                lobjNSBase.CreateExcelReport("rptNewPayeeDetailExcel.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                                idecMinimumGuaranteeNewPayees = ldtReportResult.AsEnumerable().Sum(o => o.Field<decimal>("minimum_guarantee_amount"));
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                    case 230:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialReinstatedRetireeDetailReport(icdoPaymentSchedule.payment_date);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptReinstatedPayeeDetail.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);                               
                                //PIR-10808 PDF Reports converted into Excel format                                                                
                                lobjNSBase.CreateExcelReport("rptReinstatedPayeeDetail.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                    case 240:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialClosedorSuspendedPayeeAccountReport(icdoPaymentSchedule.payment_date);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptClosedorSuspendedPayeeAccount.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                //PIR-10808 PDF Reports converted into Excel format                                                               
                                lobjNSBase.CreateExcelReport("rptClosedorSuspendedPayeeAccount.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                                idecMinimumGuaranteeCancelledorPaymentsCompletePayees =
                                ldtReportResult.AsEnumerable().Where(o => o.Field<string>("data2") == busConstant.PayeeAccountStatusPaymentComplete
                                    || o.Field<string>("data2") == busConstant.PayeeAccountStatusCancelled)
                                    .Sum(o => o.Field<decimal>("minimum_guarantee_amount"));
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                    case 245:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialMinimumGuaranteeChangeSummaryReport(icdoPaymentSchedule.payment_date,
                                icdoPaymentSchedule.payment_schedule_id);                            
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
                                lobjNSBase.CreateReport("rptMinimumGuaranteeChangeSummary.rpt", ldtFinal,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                //PIR-10808 PDF Reports converted into Excel format                                                               
                                lobjNSBase.CreateExcelReport("rptMinimumGuaranteeChangeSummary.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }                            
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);                            
                            return -1;
                        }
                        break;
                    case 250:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialRetirementOptionSummaryReport(icdoPaymentSchedule.payment_date);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptRetirementOptionSummary.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                //PIR-10808 PDF Reports converted into Excel format                                                                
                                lobjNSBase.CreateExcelReport("rptRetirementOptionSummary.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                    case 260:
                        try
                        {
                            ldsReportResult = new DataSet();
                            ldsReportResult = lobjCreateReports.TrialBenefitPaymentChangeReport(icdoPaymentSchedule.payment_date,
                                icdoPaymentSchedule.payment_schedule_id);
                            if (ldsReportResult.Tables.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptBenefitPaymentChange.rpt", ldsReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);                                
                                //PIR-10808 PDF Reports converted into Excel format                                                                
                                lobjNSBase.CreateExcelReport("rptBenefitPaymentChange_Excel.rpt", ldsReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);                          
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                    case 270:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialMonthlyBenefitPaymentSummaryReport(icdoPaymentSchedule.payment_date,
                                icdoPaymentSchedule.payment_schedule_id);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptMonthlyBenefitPaymentSummary.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                PostEndingBalance(ldtReportResult);
                                //PIR-10808 PDF Reports converted into Excel format                                                                
                                lobjNSBase.CreateExcelReport("rptMonthlyBenefitPaymentSummary.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                    case 280:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialNonMonthlyPaymentDetailReport(icdoPaymentSchedule.payment_date);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptNonMonthlyPaymentDetail.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);                                
                                //PIR-10808 PDF Reports converted into new Excel format                                                             
                                lobjNSBase.CreateExcelReport("rptNonMonthlyPaymentDetailExcel.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                    case 290:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialVendorPaymentSummary(icdoPaymentSchedule.payment_date);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptVendorPaymentSummary.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);                                
                                //PIR-10808 PDF Reports converted into Excel format                                                               
                                lobjNSBase.CreateExcelReport("rptVendorPaymentSummary.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            return -1;
                        }
                        break;
                    case 1770:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialVendorPaymentSummaryAdHoc(icdoPaymentSchedule.payment_date);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptVendorPaymentSummary.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                //PIR-10808 PDF Reports converted into Excel format                                                                
                                lobjNSBase.CreateExcelReport("rptVendorPaymentSummary.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            return -1;
                        }
                        break;
                    case 1790:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialNonMonthlyPaymentDetailReportAdHoc(icdoPaymentSchedule.payment_date);
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptNonMonthlyPaymentDetail.rpt", ldtReportResult,icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                //PIR-10808 PDF Reports converted into new Excel format                                                               
                                lobjNSBase.CreateExcelReport("rptNonMonthlyPaymentDetailExcel.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_", busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            //DBFunction.StoreProcessLog(0, "Test", "INFO", "Error Message is - " + e.InnerException.Message, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                    case 295:
                    case 1795:
                        try
                        {
                            ldtReportResult = new DataTable();
                            ldtReportResult = lobjCreateReports.TrialRefundsWithPayrollAdjustments(icdoPaymentSchedule.payment_schedule_id, icdoPaymentSchedule.payment_date, icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeMonthly);
                            icdoPaymentSchedule.PopulateDescriptions();
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                lobjNSBase.CreateReport("rptRefundsWithPayrollAdjustments.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_Trial" + icdoPaymentSchedule.schedule_type_description, busConstant.PaymentReportPath);
                                lobjNSBase.CreateExcelReport("rptRefundsWithPayrollAdjustments.rpt", ldtReportResult, icdoPaymentSchedule.payment_schedule_id.ToString() + "_Trial" + icdoPaymentSchedule.schedule_type_description, busConstant.PaymentReportPath);
                                lintrtn = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            lintrtn = -1;
                        }
                        break;
                }
                //Updating the schedule steps depending on report generation
                if (lintrtn == -1)
                {
                    lobjPaymentStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                    lobjPaymentStep.icdoPaymentScheduleStep.Update();
                    break;
                }
                else
                {
                    if (lobjPaymentStep.icdoPaymentScheduleStep.run_sequence == 200)
                    {
                        lobjPaymentStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusFailed;
                        lobjPaymentStep.icdoPaymentScheduleStep.Update();
                    }
                    else
                    {
                        lobjPaymentStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusProcessed;
                        lobjPaymentStep.icdoPaymentScheduleStep.Update();
                    }
                }
            }
            return lintrtn;
        }

        /// <summary>
        /// Method to post ending balance in Monthly benefit summary table
        /// </summary>
        /// <param name="adtReportResult">Report result</param>
        public void PostEndingBalance(DataTable adtReportResult)
        {
            ibusPaymentMonthlyBenefitSummary = new busPaymentMonthlyBenefitSummary { icdoPaymentMonthlyBenefitSummary = new cdoPaymentMonthlyBenefitSummary() };
            DataTable ldtBenefitSummary = Select<cdoPaymentMonthlyBenefitSummary>
                    (new string[1] { "payment_date" }, new object[1] { icdoPaymentSchedule.payment_date }, null, null);
            if (ldtBenefitSummary.Rows.Count > 0)
            {
                ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.LoadData(ldtBenefitSummary.Rows[0]);
                ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.Delete();
            }
            ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.payment_date = icdoPaymentSchedule.payment_date;
            ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.payment_schedule_id = icdoPaymentSchedule.payment_schedule_id;

            var lvarEndingBalance = (from ldrReportResult in adtReportResult.AsEnumerable()
                                     where ldrReportResult.Field<int>("reporder") != 1
                                     && ldrReportResult.Field<int>("reporder") != 2
                                     select new
                                     {
                                         gros = adtReportResult.AsEnumerable().Where(o => o.Field<int>("reporder") != 1
                                                  && o.Field<int>("reporder") != 2).Sum(o => o.Field<decimal>("gros")),
                                         rhic = adtReportResult.AsEnumerable().Where(o => o.Field<int>("reporder") != 1
                                                  && o.Field<int>("reporder") != 2).Sum(o => o.Field<decimal>("rhic")),
                                         fedl = adtReportResult.AsEnumerable().Where(o => o.Field<int>("reporder") != 1
                                                  && o.Field<int>("reporder") != 2).Sum(o => o.Field<int>("reporder") != 8 ? 
                                                      o.Field<decimal>("fedl") : Math.Abs(o.Field<decimal>("fedl"))),
                                         stat = adtReportResult.AsEnumerable().Where(o => o.Field<int>("reporder") != 1
                                                  && o.Field<int>("reporder") != 2).Sum(o => o.Field<int>("reporder") != 8 ?
                                                      o.Field<decimal>("stat") : Math.Abs(o.Field<decimal>("stat"))),
                                         insr = adtReportResult.AsEnumerable().Where(o => o.Field<int>("reporder") != 1
                                                  && o.Field<int>("reporder") != 2).Sum(o => o.Field<int>("reporder") != 8 ?
                                                      o.Field<decimal>("insr") : Math.Abs(o.Field<decimal>("insr"))),
                                         othr = adtReportResult.AsEnumerable().Where(o => o.Field<int>("reporder") != 1
                                                  && o.Field<int>("reporder") != 2).Sum(o => o.Field<int>("reporder") != 8 ?
                                                      o.Field<decimal>("othr") : Math.Abs(o.Field<decimal>("othr"))),
                                         net_amount = adtReportResult.AsEnumerable().Where(o => o.Field<int>("reporder") != 1
                                                  && o.Field<int>("reporder") != 2).Sum(o => o.Field<decimal>("net_amount"))
                                     });
            Array.ForEach(lvarEndingBalance.ToArray(), o =>
                {
                    ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.gross_amount = o.gros;
                    ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.rhic_benefit = o.rhic;
                    ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.federal_tax = o.fedl;
                    ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.state_tax = o.stat;
                    ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.insurance = o.insr;
                    ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.other_deductions = o.othr;
                    ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.net_amount = o.net_amount;
                });
            ibusPaymentMonthlyBenefitSummary.icdoPaymentMonthlyBenefitSummary.Insert();
        }

        //this method will be called when the user clciks on Approve For Final button in payment schedule maitnenance screen
        public ArrayList btnApproveForFinal_Click()
        {
            ArrayList larrErros = new ArrayList();
            utlError lobjError = new utlError();

            // if the schedule type is monthly scdule ,the payment date should be before 20th of month preceding next payment date
            if (icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeMonthly
                && DateTime.Now < icdoPaymentSchedule.payment_date.AddMonths(-1).AddDays(19))
            {
                lobjError = AddError(6404, "");
                larrErros.Add(lobjError);
                return larrErros;
            }
            //if schedule type is Adhoc then Payment date should be equal to Today's date
            if (icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeAdhoc
                && !IsPaymentDateSameAsToday())
            {
                lobjError = AddError(6431, "");
                larrErros.Add(lobjError);
                return larrErros;
            }
            //Refresh payment steps 1.delete existing steps 2. create new steps

            if (iclbPaymentScheduleStep == null)
                LoadPaymentScheduleSteps();
            foreach (busPaymentScheduleStep lobjStep in iclbPaymentScheduleStep)
                lobjStep.icdoPaymentScheduleStep.Delete();

            //Create New Payment Schedule Steps
            CreatePaymentSteps(true);

            //Reload Payment schedule steps - No null check bcoz of Reload
            LoadPaymentScheduleSteps();

            icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusReadyforFinal;
            icdoPaymentSchedule.Update();
            larrErros.Add(this);
            return larrErros;
        }
        //this method will be called when the user clciks on Cancel button in payment schedule maitnenance screen

        public ArrayList btnCancel_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusCancelled;
            icdoPaymentSchedule.Update();
            alReturn.Add(this);
            return alReturn;
        }
        //BR-075-04
        //While creating a new payment schedule ,check for payment schedules exist with schedule type monthly, action status is not cancelled
        //if records exist,throw ar error
        public bool IsRecordExistforNextPaymentDate()
        {
            if (icdoPaymentSchedule.payment_date != DateTime.MinValue && icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeMonthly)
            {
                Collection<busPaymentSchedule> lclbPaymentSchedule = busPayeeAccountHelper.LoadPaymentSchedules(icdoPaymentSchedule.payment_date);
                if (lclbPaymentSchedule.Where(o => o.icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeMonthly &&
                                                                   o.icdoPaymentSchedule.payment_schedule_id != icdoPaymentSchedule.payment_schedule_id &&
                                                                   o.icdoPaymentSchedule.payment_date == icdoPaymentSchedule.payment_date &&
                                                                   o.icdoPaymentSchedule.action_status_value != busConstant.PaymentScheduleActionStatusCancelled).Count() > 0)
                {

                    return true;
                }
            }
            return false;
        }
        public override int PersistChanges()
        {
            return base.PersistChanges();
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            //Create payment steps once the schedule is createed
            if (iblnIsNewMode)
                CreatePaymentSteps(false);
            LoadPaymentScheduleSteps();
            EvaluateInitialLoadRules();
        }

        /// <summary>
        /// Method to check whether payemnt date and check/ach effective date is of same month/year
        /// </summary>
        /// <returns></returns>
        public bool IsEffectiveDateSameMonthAsPaymentDate()
        {
            bool lblnResult = false;
            DateTime ldtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            if (icdoPaymentSchedule.effective_date != DateTime.MinValue)
            {
                if (icdoPaymentSchedule.effective_date.GetLastDayofMonth() <= ldtNextBenefitPaymentDate.GetLastDayofMonth())
                    lblnResult = true;
                else
                    lblnResult = false;
            }
            else
                lblnResult = true;
            return lblnResult;
        }

        /// <summary>
        /// Method to check whether Payment date is same as Today's date
        /// </summary>
        /// <returns>Boolean value</returns>
        public bool IsPaymentDateSameAsToday()
        {
            bool lblnResult = false;
            busSystemManagement lbusSystemManagement = new busSystemManagement();
            lbusSystemManagement.FindSystemManagement();
            if (icdoPaymentSchedule.payment_date.Date == lbusSystemManagement.icdoSystemManagement.batch_date.Date)
                lblnResult = true;
            return lblnResult;
        }
    }
}