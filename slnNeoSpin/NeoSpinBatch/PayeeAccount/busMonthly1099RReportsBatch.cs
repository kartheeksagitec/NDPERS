#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.ExceptionPub;
using System.IO;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;

#endregion
namespace NeoSpinBatch
{
    public class busMonthly1099RReportsBatch : busNeoSpinBatch
    {
        public busPayment1099rRequest ibusPayment1099rRequest { get; set; } //PIR-8946
        public busMonthly1099RReportsBatch()
        {
        }
        public void GenerateMonthly1099rReport()
        {
            string lstrGenReportName = string.Empty;
            istrProcessName = iobjBatchSchedule.step_name;
            
            idlgUpdateProcessLog(istrProcessName + "  Started", "INFO", istrProcessName);
            try
            {
                //Loading the approved Monthly batch request
                if (ibusPayment1099rRequest == null)
                    LoadMonthly1099rRequest(busConstant.BatchRequest1099rStatusApproved);
                if (ibusPayment1099rRequest.icdoPayment1099rRequest.request_id > 0)
                {
                    busPayment1099r lobjPayment1099r = new busPayment1099r();
                    busNeoSpinBase lbusBase = new busNeoSpinBase();
                    //dropping temp 1099r table if any
                    lobjPayment1099r.DropTemp1099rTable();
                    lobjPayment1099r.CreateTemp1099rTableWithData(iobjSystemManagement.icdoSystemManagement.batch_date.Year);

                    DataTable ldtbMonthlyRefund1099R = busBase.Select("cdoPayment1099r.rptMonthlyRefund1099R", new object[0] { });

                    if (ldtbMonthlyRefund1099R.Rows.Count > 0)
                    {
                        idlgUpdateProcessLog("Generate Monthly Refund 1099R Report", "INFO", istrProcessName);
                        CreateReport("rptMonthlyRefund1099R.rpt", ldtbMonthlyRefund1099R, busConstant.Report1099rPath);
                        //PIR-8984 --> Generate the report in Excel format 
                        lstrGenReportName = lbusBase.CreateExcelReport("rptMonthlyRefund1099R_EXCEL.rpt", ldtbMonthlyRefund1099R, "_", busConstant.Report1099rPath);
                        //PIR-8984 End 
                        idlgUpdateProcessLog("Monthly Refund 1099R Report generated successfully", "INFO", istrProcessName);
                    }

                    DataTable ldtbMonthlyAnnuitant1099R = busBase.Select("cdoPayment1099r.rptMonthlyAnnuitant1099R",
                        new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });

                    if (ldtbMonthlyAnnuitant1099R.Rows.Count > 0)
                    {
                        idlgUpdateProcessLog("Generate Monthly Annuitant 1099R Report", "INFO", istrProcessName);
                        CreateReport("rptMonthlyAnnuitant1099R.rpt", ldtbMonthlyAnnuitant1099R, busConstant.Report1099rPath);
                        //PIR-8984 --> Generate the report in Excel format 
                        lstrGenReportName = lbusBase.CreateExcelReport("rptMonthlyAnnuitant1099R_EXCEL.rpt", ldtbMonthlyAnnuitant1099R, "_", busConstant.Report1099rPath);
                        //PIR-8984 End
                        UpdateBatchRequest();
                        idlgUpdateProcessLog("Monthly Annuitant 1099R Report generated successfully", "INFO", istrProcessName);
                    }
                    lobjPayment1099r.DropTemp1099rTable();
                }
                idlgUpdateProcessLog("No Monthly 1099r requests are in approved status", "INFO", istrProcessName);
            }

            catch (Exception ex)
            {
                idlgUpdateProcessLog(iobjBatchSchedule.step_name + " failed", "ERR", istrProcessName);
                ExceptionManager.Publish(ex);
            }
            idlgUpdateProcessLog(istrProcessName + "  Ended", "INFO", istrProcessName); 
        }

        //PIR-8946 Start
        public void GenerateTrialMonthly1099rReport()
        {
            string lstrGenReportName = string.Empty;
            istrProcessName = iobjBatchSchedule.step_name;

            idlgUpdateProcessLog(istrProcessName + "  Started", "INFO", istrProcessName);
            try
            {
                 //Loading the Pending Montlhy batch request
                if (ibusPayment1099rRequest == null)
                    LoadMonthly1099rRequest(busConstant.BatchRequest1099rStatusPending);
                if (ibusPayment1099rRequest.icdoPayment1099rRequest.request_id > 0)
                {
                busPayment1099r lobjPayment1099r = new busPayment1099r();
                busNeoSpinBase lbusBase = new busNeoSpinBase();
                //dropping temp 1099r table if any
                lobjPayment1099r.DropTemp1099rTable();
                lobjPayment1099r.CreateTemp1099rTableWithData(iobjSystemManagement.icdoSystemManagement.batch_date.Year);

                DataTable ldtbMonthlyRefund1099R = busBase.Select("cdoPayment1099r.rptMonthlyRefund1099R", new object[0] { });

                if (ldtbMonthlyRefund1099R.Rows.Count > 0)
                {
                    idlgUpdateProcessLog("Generate Trail Monthly Refund 1099R Report", "INFO", istrProcessName);
                    
                    CreateReportWithPrefix("rptMonthlyRefund1099R.rpt", ldtbMonthlyRefund1099R, "Trial_", busConstant.Report1099rPath);
                    //PIR-8984 --> Generate the report in Excel format 
                    lstrGenReportName = lbusBase.CreateExcelReport("rptMonthlyRefund1099R_EXCEL.rpt", ldtbMonthlyRefund1099R, "Trial_", busConstant.Report1099rPath);
                    //PIR-8984 End 
                    idlgUpdateProcessLog("Monthly Trail Refund 1099R Report generated successfully", "INFO", istrProcessName);
                }

                DataTable ldtbMonthlyAnnuitant1099R = busBase.Select("cdoPayment1099r.rptMonthlyAnnuitant1099R",
                    new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });

                if (ldtbMonthlyAnnuitant1099R.Rows.Count > 0)
                {
                    idlgUpdateProcessLog("Generate Trail Monthly Annuitant 1099R Report", "INFO", istrProcessName);
                    
                    CreateReportWithPrefix("rptMonthlyAnnuitant1099R.rpt", ldtbMonthlyAnnuitant1099R, "Trial_", busConstant.Report1099rPath);
                    //PIR-8984 --> Generate the report in Excel format 
                    lstrGenReportName = lbusBase.CreateExcelReport("rptMonthlyAnnuitant1099R_EXCEL.rpt", ldtbMonthlyAnnuitant1099R, "Trial_", busConstant.Report1099rPath);
                    //PIR-8984 End
                    idlgUpdateProcessLog("Trail Monthly Annuitant 1099R Report generated successfully", "INFO", istrProcessName);
                }
                lobjPayment1099r.DropTemp1099rTable();
                    }
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog(iobjBatchSchedule.step_name + " failed", "ERR", istrProcessName);
                ExceptionManager.Publish(ex);
            }
            idlgUpdateProcessLog(istrProcessName + "  Ended", "INFO", istrProcessName);
        }

        
        private void LoadMonthly1099rRequest(string astrStatusValue)
        {
            ibusPayment1099rRequest = new busPayment1099rRequest { icdoPayment1099rRequest = new cdoPayment1099rRequest() };

            DataTable ldt1099rRequests = busBase.Select<cdoPayment1099rRequest>
                (new string[2] { enmPayment1099rRequest.status_value.ToString(), enmPayment1099rRequest.request_type_value.ToString() },
                new object[2] { astrStatusValue, busConstant.Monthly1099RReportsBatch },
                null, "tax_year desc");
            if (ldt1099rRequests.Rows.Count > 0)
            {
                ibusPayment1099rRequest.icdoPayment1099rRequest.LoadData(ldt1099rRequests.Rows[0]);
            }
        }


        private void UpdateBatchRequest()
        {
            try
            {
                idlgUpdateProcessLog("Updating Monthly 1099r Batch Request started", "INFO", istrProcessName);
                ibusPayment1099rRequest.icdoPayment1099rRequest.status_value = busConstant.BatchRequest1099rStatusProcessed;
                ibusPayment1099rRequest.icdoPayment1099rRequest.process_date = DateTime.Now;
                ibusPayment1099rRequest.icdoPayment1099rRequest.Update();
                idlgUpdateProcessLog("Updating Monthly 1099r Batch Request finished successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Updating Monthly 1099r Batch Request failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        //PIR-8946 End
    }
}