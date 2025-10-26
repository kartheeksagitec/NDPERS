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
#endregion

namespace NeoSpinBatch
{
    public class busAnnual1099rBatch : busNeoSpinBatch
    {
        public busPayment1099rRequest ibusPayment1099rRequest { get; set; }

        // PIR 10838
        public busPayment1099r ibusPayment1099r { get; set; }
        public bool iblnTransaction { get; set; }

        public void ProcessAnnual1099rBatch()
        {
            try
            {
                istrProcessName = iobjBatchSchedule.step_name;
                //Loading the approved Annual batch request
                if (ibusPayment1099rRequest == null)
                    LoadPayment1099rRequest(busConstant.BatchRequest1099rStatusApproved);
                if (ibusPayment1099rRequest.icdoPayment1099rRequest.request_id > 0)
                {
                    iblnTransaction = false;
                    busPayment1099r lobjPayment1099r = new busPayment1099r();
                    //dropping temp 1099r table if any
                    lobjPayment1099r.DropTemp1099rTable();
                    //creating new temp 1099r table for Tax year
                    lobjPayment1099r.CreateTemp1099rTableWithData(ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year);

                    // PIR 10838 - don't insert data if already exist
                    if (ibusPayment1099rRequest.icdoPayment1099rRequest.bulk_insert_1099r_data_flag != busConstant.Flag_Yes)
                    {
                        // PIR 10838 - Begin transaction
                        if (!iblnTransaction)
                        {
                            utlPassInfo.iobjPassInfo.BeginTransaction();
                            iblnTransaction = true;
                        }

                        //create 1099r
                        CreatePayment1099r(iobjBatchSchedule.batch_schedule_id);

                        // PIR 10838 - Update flag
                        ibusPayment1099rRequest.icdoPayment1099rRequest.bulk_insert_1099r_data_flag = busConstant.Flag_Yes;
                        ibusPayment1099rRequest.icdoPayment1099rRequest.Update();

                        // PIR 10838 - Commit transaction
                        if (iblnTransaction)
                        {
                            utlPassInfo.iobjPassInfo.Commit();
                            iblnTransaction = false;
                        }
                    }

                    // PIR 10838 - Don't create report if already created
                    if (ibusPayment1099rRequest.icdoPayment1099rRequest.created_1099r_details_report != busConstant.Flag_Yes)
                    {
                        //create 1099r details report
                        Create1099rDetailsReport();
                    }
                    //create 1099r Form and IRS file
                    Create1099rFormAndIRSFile();

                    // PIR 10838 - Don't create report if already created
                    if (ibusPayment1099rRequest.icdoPayment1099rRequest.created_945_report!= busConstant.Flag_Yes)
                    {
                        //create 945 report
                        Create945Report();
                    }
                    //updating the annual batch status
                    UpdateBatchRequest();
                    //dropping temp 1099r table if any
                    lobjPayment1099r.DropTemp1099rTable();
                }
            }
            catch (Exception ex)
            {
                // PIR 10838 - Rollback transaction
                if (iblnTransaction)
                {
                    utlPassInfo.iobjPassInfo.Rollback();
                    iblnTransaction = false;
                }
                idlgUpdateProcessLog(iobjBatchSchedule.step_name+" failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
            }
        }

        public void ProcessTrialAnnual1099rBatch()
        {
            try
            {
                istrProcessName = iobjBatchSchedule.step_name;
                //Loading the approved Annual batch request
                if (ibusPayment1099rRequest == null)
                    LoadPayment1099rRequest(busConstant.BatchRequest1099rStatusPending);
                if (ibusPayment1099rRequest.icdoPayment1099rRequest.request_id > 0)
                {
                    busPayment1099r lobjPayment1099r = new busPayment1099r();
                    lobjPayment1099r.DropTrialTemp1099rTable();
                    lobjPayment1099r.CreateTemp1099rTableWithData(ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year);
                    CreateTrialPayment1099r(iobjBatchSchedule.batch_schedule_id);
                    CreateTrial1099rForm();
                    lobjPayment1099r.DropTrialTemp1099rTable();
                }
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog(iobjBatchSchedule.step_name + " failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
            }
        }

        /// <summary>
        /// Method to update the batch request to Processed
        /// </summary>
        private void UpdateBatchRequest()
        {
            try
            {
                idlgUpdateProcessLog("Updating Annual 1099r Batch Request started", "INFO", istrProcessName);
                ibusPayment1099rRequest.icdoPayment1099rRequest.status_value = busConstant.BatchRequest1099rStatusProcessed;
                ibusPayment1099rRequest.icdoPayment1099rRequest.process_date = DateTime.Now;
                ibusPayment1099rRequest.icdoPayment1099rRequest.Update();
                idlgUpdateProcessLog("Updating Annual 1099r Batch Request finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Updating Annual 1099r Batch Request failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Method to create 945 report
        /// </summary>
        private void Create945Report()
        {
            try
            {
                idlgUpdateProcessLog("Creating 945 Report started", "INFO", istrProcessName);
                DataTable ldtReportResult = busBase.Select("cdoPayment1099r.rpt945",
                    new object[1] { ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year });
                ldtReportResult.TableName = busConstant.ReportTableName;
                if (ldtReportResult.Rows.Count > 0)
                    CreateReport("rpt945.rpt", ldtReportResult, busConstant.Report1099rPath);
                idlgUpdateProcessLog("Creating 945 Report finished successfully", "INFO", istrProcessName);
                // PIR 10838 - Update flag
                ibusPayment1099rRequest.icdoPayment1099rRequest.created_945_report = busConstant.Flag_Yes;
                ibusPayment1099rRequest.icdoPayment1099rRequest.Update();
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating 945 Report failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Method to create 1099r Form and IRS File
        /// </summary>
        private void Create1099rFormAndIRSFile()
        {
            try
            {
                string lstrReportPrefix = string.Empty;
                DataTable ldt1099rForm = new DataTable();
                DataTable ldtReportResult = busBase.Select("cdoPayment1099r.rpt1099rForm",
                                                        new object[1] { ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year });
                // PIR 10838 - Don't create report if already created
                //if (ibusPayment1099rRequest.icdoPayment1099rRequest.created_irs_file != busConstant.Flag_Yes)
                //{
                //    idlgUpdateProcessLog("Creating Annual IRS File started", "INFO", istrProcessName);
                //    if (ldtReportResult.Rows.Count > 0)
                //    {
                //        busProcessOutboundFile lobjProcessPensionFile = new busProcessOutboundFile();
                //        lobjProcessPensionFile.iarrParameters = new object[3];
                //        lobjProcessPensionFile.iarrParameters[0] = ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year;
                //        lobjProcessPensionFile.iarrParameters[1] = false;
                //        lobjProcessPensionFile.iarrParameters[2] = ldtReportResult;
                //        lobjProcessPensionFile.CreateOutboundFile(73);
                //    }
                //    idlgUpdateProcessLog("Creating Annual IRS File finished successfully", "INFO", istrProcessName);
                //    // PIR 10838 - Update flag
                //    ibusPayment1099rRequest.icdoPayment1099rRequest.created_irs_file = busConstant.Flag_Yes;
                //    ibusPayment1099rRequest.icdoPayment1099rRequest.Update();
                //}
                    idlgUpdateProcessLog("Creating Annual IRS 1099r Form started", "INFO", istrProcessName);

                    foreach (DataRow dr in ldtReportResult.Rows)
                    {
                        LoadPayment1099r(Convert.ToInt32(dr["payment_1099r_id"])); // PIR 10838
                                                                                   // PIR 10838 - Don't create 1099r form if already created
                        if (ibusPayment1099r.icdoPayment1099r.created_annual_1099r_form != busConstant.Flag_Yes)
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
                            // PIR 10838 - Update flag
                            ibusPayment1099r.icdoPayment1099r.created_annual_1099r_form = busConstant.Flag_Yes;
                            ibusPayment1099r.icdoPayment1099r.Update();
                        }
                    }
                    idlgUpdateProcessLog("Creating Annual 1099r Form finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Annual 1099r Form failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Method to create Trial 1099r Form and IRS File
        /// </summary>
        private void CreateTrial1099rForm()
        {
            try
            {
                string lstrReportPrefix = string.Empty;
                DataTable ldt1099rForm = new DataTable();
                DataTable ldtReportResult = busBase.Select("cdoPayment1099r.rptTrial1099rForm",
                    new object[1] { ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year });
                idlgUpdateProcessLog("Creating 100 Trial Annual 1099r Form started ", "INFO", istrProcessName);
                foreach (DataRow dr in ldtReportResult.Rows)
                {
                    lstrReportPrefix = string.Empty;
                    lstrReportPrefix = "Trial_" + dr["payee_account_id"].ToString() + "_" + dr["distribution_code"].ToString() + "_" +
                                        dr["age59_split_flag"].ToString() + "_" + dr["run_year"].ToString() + "_" + dr["corrected_flag"].ToString() + "_";
                    ldt1099rForm = new DataTable();
                    ldt1099rForm = ldtReportResult.Clone();
                    ldt1099rForm.ImportRow(dr);
                    ldt1099rForm.TableName = busConstant.ReportTableName;
                    ldt1099rForm.AcceptChanges();
                    //PIR-16715 created report file with respective year
                    CreateReportWithPrefix("rptForm1099R_" + dr["run_year"] + ".rpt", ldt1099rForm, lstrReportPrefix, busConstant.Report1099rPath);
                }
                idlgUpdateProcessLog("Creating 100 Trial Annual 1099r Form finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Trial Annual 1099r Form failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Method to create 1099r Details Report
        /// </summary>
        public void Create1099rDetailsReport()
        {
            try
            {
                idlgUpdateProcessLog("Creating 1099R Details Report started", "INFO", istrProcessName);
                DataTable ldtReportResult = busBase.Select("cdoPayment1099r.rptCreate1099rDetails",
                    new object[1] { ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year });
                ldtReportResult.TableName = busConstant.ReportTableName;
                if (ldtReportResult.Rows.Count > 0)
                    CreateReport("rpt1099RDetails.rpt", ldtReportResult, busConstant.Report1099rPath);
                idlgUpdateProcessLog("Creating 1099R Details Report finished successfully", "INFO", istrProcessName);
                // PIR 10838 - Update flag
                ibusPayment1099rRequest.icdoPayment1099rRequest.created_1099r_details_report = busConstant.Flag_Yes;
                ibusPayment1099rRequest.icdoPayment1099rRequest.Update();
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating 1099R Details Report failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Method to create entries in Payment 1099r table for tax year
        /// </summary>
        private void CreatePayment1099r(int aintBatchScheduleId)
        {
            try
            {
                idlgUpdateProcessLog("Creating Payment 1099r started", "INFO", istrProcessName);
                DBFunction.DBNonQuery("cdoPayment1099r.Create1099r",
                    new object[2] { ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year ,aintBatchScheduleId},
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                idlgUpdateProcessLog("Creating Payment 1099r finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Payment 1099r failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Method to create Trial entries in Payment 1099r table for tax year
        /// </summary>
        private void CreateTrialPayment1099r(int aintBatchScheduleId)
        {
            try
            {
                idlgUpdateProcessLog("Creating Trial Payment 1099r started", "INFO", istrProcessName);
                DBFunction.DBNonQuery("cdoPayment1099r.CreateTrial1099r",
                    new object[2] { ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year ,aintBatchScheduleId},
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                idlgUpdateProcessLog("Creating Trial Payment 1099r finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Trial Payment 1099r failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Method to load approved Annual batch request
        /// </summary>
        private void LoadPayment1099rRequest(string astrStatusValue)
        {
            ibusPayment1099rRequest = new busPayment1099rRequest { icdoPayment1099rRequest = new cdoPayment1099rRequest() };

            DataTable ldt1099rRequests = busBase.Select<cdoPayment1099rRequest>
                (new string[2] { enmPayment1099rRequest.status_value.ToString(), enmPayment1099rRequest.request_type_value.ToString() },
                new object[2] { astrStatusValue, busConstant.BatchRequest1099rTypeAnnual },
                null, "tax_year desc");
            if (ldt1099rRequests.Rows.Count > 0)
            {
                ibusPayment1099rRequest.icdoPayment1099rRequest.LoadData(ldt1099rRequests.Rows[0]);
            }
        }

        // PIR 10838
        private void LoadPayment1099r(int aintPayment1099rId)
        {
            if (ibusPayment1099r == null)
            {
                ibusPayment1099r = new busPayment1099r();
            }
            ibusPayment1099r.FindPayment1099r(aintPayment1099rId);
        }
    }
}
