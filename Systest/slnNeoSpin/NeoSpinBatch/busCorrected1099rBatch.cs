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
using NeoSpin.DataObjects;
using System.Linq;
using System.Linq.Expressions;
#endregion

namespace NeoSpinBatch
{
    public class busCorrected1099rBatch : busNeoSpinBatch
    {
        public busPayment1099rRequest ibusPayment1099rRequest { get; set; }

        public void ProcessCorrected1099rBatch()
        {
            try
            {
                istrProcessName = iobjBatchSchedule.step_name;
                //Loading the Corrected 1099r batch request
                if (ibusPayment1099rRequest == null)
                    LoadCorrected1099rRequest();
                busPayment1099r lobjPayment1099r = new busPayment1099r();
                //dropping temp 1099r table if any
                lobjPayment1099r.DropTempCorrected1099rTable();
                //creating new temp 1099r table for Tax year
                lobjPayment1099r.CreateTempCorrected1099rTableWithData(DateTime.Today.Year);
                //create corrected 1099r
                DateTime ldtCreatedDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,DateTime.Now.Hour,00,00);
                CreateCorrectedPayment1099r(iobjBatchSchedule.batch_schedule_id);
                //create corrected 1099r forms
                CreateCorrected1099rForm(ldtCreatedDate);
                //dropping temp 1099r table if any
                lobjPayment1099r.DropTempCorrected1099rTable();
                //Update corrected 1099r batch request
                UpdateCorrectedBatchRequest(ldtCreatedDate);

            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog(iobjBatchSchedule.step_name + " failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
            }
        }

        /// <summary>
        /// Method to create corrected entries in Payment 1099r table
        /// </summary>
        private void CreateCorrectedPayment1099r(int aintBatchScheduleId)
        {
            try
            {
                idlgUpdateProcessLog("Creating Corrected 1099r started", "INFO", istrProcessName);
                DBFunction.DBNonQuery("cdoPayment1099r.CreateCorrected1099r", new object[1] { aintBatchScheduleId },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                idlgUpdateProcessLog("Creating Corrected 1099r finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Corrected 1099r failed", "INFO", istrProcessName);
                throw ex;
            }
        }

        /// <summary>
        /// Method to create Corrected 1099r Form
        /// </summary>
        private void CreateCorrected1099rForm(DateTime adtCreatedDate)
        {
            try
            {
                string lstrReportPrefix = string.Empty;
                DataTable ldt1099rForm = new DataTable();
                
                DataTable ldtReportResult = busBase.Select("cdoPayment1099r.rptCorrected1099rForm",
                    new object[1] { adtCreatedDate});

                //idlgUpdateProcessLog("Creating Corrected IRS File started", "INFO", istrProcessName);
                //if (ldtReportResult.Rows.Count > 0)
                //{
                //    busBase lobjbase=new busBase();
                //    Collection<busPayment1099r> lclbPayment1099r = lobjbase.GetCollection<busPayment1099r>(ldtReportResult, "icdoPayment1099r");
                //    var lintYears = (from lobjPayment1099r in lclbPayment1099r select lobjPayment1099r.icdoPayment1099r.tax_year).Distinct().OrderBy(n => n);
                //    foreach (int lintYear in lintYears)
                //    {
                //        DataTable ldtbCorrected1099rdata = ldtReportResult.AsEnumerable().Where(o =>
                //                                                 o.Field<int>("run_year") == lintYear).AsDataTable();
                //        busProcessOutboundFile lobjProcessPensionFile = new busProcessOutboundFile();
                //        lobjProcessPensionFile.iarrParameters = new object[3];
                //        lobjProcessPensionFile.iarrParameters[0] = lintYear;
                //        lobjProcessPensionFile.iarrParameters[1] = true;
                //        lobjProcessPensionFile.iarrParameters[2] = ldtbCorrected1099rdata;
                //        lobjProcessPensionFile.CreateOutboundFile(73);
                //    }
                //}
                //idlgUpdateProcessLog("Creating Corrected IRS File finished successfully", "INFO", istrProcessName);
                idlgUpdateProcessLog("Creating Corrected 1099r Form started", "INFO", istrProcessName);
                foreach (DataRow dr in ldtReportResult.Rows)
                {
                    lstrReportPrefix = string.Empty;
                    lstrReportPrefix = dr["payee_account_id"].ToString() + "_" +
                                        dr["distribution_code"].ToString() + "_" +
                                        dr["age59_split_flag"].ToString() + "_" +
                                        dr["run_year"].ToString() + "_" +
                                        dr["corrected_flag"].ToString() + "_";
                    ldt1099rForm = new DataTable();
                    ldt1099rForm = ldtReportResult.Clone();
                    ldt1099rForm.ImportRow(dr);
                    ldt1099rForm.TableName = busConstant.ReportTableName;
                    ldt1099rForm.AcceptChanges();
                    //PIR-16715 created report file with respective year
                    CreateReportWithPrefix("rptForm1099R_" + dr["run_year"] + ".rpt", ldt1099rForm, lstrReportPrefix, busConstant.Report1099rPath);
                }
                idlgUpdateProcessLog("Creating Corrected 1099r Form finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Corrected 1099r Form failed", "INFO", istrProcessName);
                throw ex;
            }
        }

        /// <summary>
        /// Method to load Corrected 1099r batch request
        /// </summary>
        private void LoadCorrected1099rRequest()
        {
            ibusPayment1099rRequest = new busPayment1099rRequest { icdoPayment1099rRequest = new cdoPayment1099rRequest() };

            DataTable ldt1099rRequests = busBase.Select<cdoPayment1099rRequest>
                (new string[1] { enmPayment1099rRequest.request_type_value.ToString() },
                new object[1] { busConstant.BatchRequest1099rTypeMonthly },
                null, "tax_year desc");
            if (ldt1099rRequests.Rows.Count > 0)
            {
                ibusPayment1099rRequest.icdoPayment1099rRequest.LoadData(ldt1099rRequests.Rows[0]);                
            }
        }

        /// <summary>
        /// method to update corrected 1099r batch request
        /// </summary>
        /// <param name="adtProcessDate">Process date</param>
        private void UpdateCorrectedBatchRequest(DateTime adtProcessDate)
        {
            try
            {
                idlgUpdateProcessLog("Updating Corrected 1099r Batch Request started", "INFO", istrProcessName);
                //updating the request
                if (ibusPayment1099rRequest.icdoPayment1099rRequest.request_id > 0)
                {
                    ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year = adtProcessDate.Year;
                    ibusPayment1099rRequest.icdoPayment1099rRequest.process_date = adtProcessDate;
                    ibusPayment1099rRequest.icdoPayment1099rRequest.Update();
                }
                //inserting one new request (happens only one time)
                else
                {
                    ibusPayment1099rRequest.icdoPayment1099rRequest.status_value = busConstant.BatchRequest1099rStatusProcessed;
                    ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year = adtProcessDate.Year;
                    ibusPayment1099rRequest.icdoPayment1099rRequest.request_type_value = busConstant.BatchRequest1099rTypeMonthly;
                    ibusPayment1099rRequest.icdoPayment1099rRequest.process_date = adtProcessDate;
                    ibusPayment1099rRequest.icdoPayment1099rRequest.Insert();
                }
                idlgUpdateProcessLog("Updating Corrected 1099r Batch Request finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Updating Corrected 1099r Batch Request failed", "INFO", istrProcessName);
                throw ex;
            }
        }
    }
}
