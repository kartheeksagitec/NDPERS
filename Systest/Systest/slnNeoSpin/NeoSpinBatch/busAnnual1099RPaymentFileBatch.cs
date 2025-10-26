using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using Sagitec.Bpm;
using NeoSpin.DataObjects;
using System.Collections.ObjectModel;
using System.Linq;

namespace NeoSpinBatch
{
    class busAnnual1099RPaymentFileBatch : busNeoSpinBatch
    {
        public busPayment1099rRequest ibusPayment1099rRequest { get; set; }
        public busAnnual1099rBatch ibusAnnual1099rBatch { get; set; }
        public void Create1099RReportAndFiles()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            //Loading the approved Annual batch request
            if (ibusPayment1099rRequest == null)
                LoadPayment1099rRequest();
            if (ibusPayment1099rRequest.icdoPayment1099rRequest.request_id > 0)
            {
                ibusAnnual1099rBatch.istrProcessName = istrProcessName;
                ibusAnnual1099rBatch.ibusPayment1099rRequest = ibusPayment1099rRequest;
                ibusAnnual1099rBatch.Create1099rDetailsReport();
                CreatingAnnual1099rIRSFile();
                DateTime ldtCreatedDate = new DateTime(ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 00, 00);
                CreateCorrected1099rIRSFile(ldtCreatedDate);
                busPayment1099r lbusPayment1099r = new busPayment1099r();
                //dropping temp 1099r table if any
                lbusPayment1099r.DropTempCorrected1099rTable();
            }
        }

        private void CreatingAnnual1099rIRSFile()
        {
            try
            {
                DataTable ldtReportResult = busBase.Select("cdoPayment1099r.rpt1099rForm",
                                                                        new object[1] { ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year });
                // PIR 10838 - Don't create report if already created
                if (ibusPayment1099rRequest.icdoPayment1099rRequest.created_irs_file != busConstant.Flag_Yes)
                {
                    idlgUpdateProcessLog("Creating Annual IRS File started", "INFO", istrProcessName);
                    if (ldtReportResult.Rows.Count > 0)
                    {
                        busProcessOutboundFile lobjProcessPensionFile = new busProcessOutboundFile();
                        lobjProcessPensionFile.iarrParameters = new object[3];
                        lobjProcessPensionFile.iarrParameters[0] = ibusPayment1099rRequest.icdoPayment1099rRequest.tax_year;
                        lobjProcessPensionFile.iarrParameters[1] = false;
                        lobjProcessPensionFile.iarrParameters[2] = ldtReportResult;
                        lobjProcessPensionFile.CreateOutboundFile(73);
                    }
                    idlgUpdateProcessLog("Creating Annual IRS File finished successfully", "INFO", istrProcessName);
                    // PIR 10838 - Update flag
                    ibusPayment1099rRequest.icdoPayment1099rRequest.created_irs_file = busConstant.Flag_Yes;
                    ibusPayment1099rRequest.icdoPayment1099rRequest.Update();
                }
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Annual 1099r IRS File failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }
        private void CreateCorrected1099rIRSFile(DateTime adtCreatedDate)
        {
            try
            {
                string lstrReportPrefix = string.Empty;
                DataTable ldt1099rForm = new DataTable();

                DataTable ldtReportResult = busBase.Select("cdoPayment1099r.rptCorrected1099rForm",
                    new object[1] { adtCreatedDate });

                idlgUpdateProcessLog("Creating Corrected IRS File started", "INFO", istrProcessName);
                if (ldtReportResult.Rows.Count > 0)
                {
                    busBase lobjbase = new busBase();
                    Collection<busPayment1099r> lclbPayment1099r = lobjbase.GetCollection<busPayment1099r>(ldtReportResult, "icdoPayment1099r");
                    var lintYears = (from lobjPayment1099r in lclbPayment1099r select lobjPayment1099r.icdoPayment1099r.tax_year).Distinct().OrderBy(n => n);
                    foreach (int lintYear in lintYears)
                    {
                        DataTable ldtbCorrected1099rdata = ldtReportResult.AsEnumerable().Where(o =>
                                                                 o.Field<int>("run_year") == lintYear).AsDataTable();
                        busProcessOutboundFile lobjProcessPensionFile = new busProcessOutboundFile();
                        lobjProcessPensionFile.iarrParameters = new object[3];
                        lobjProcessPensionFile.iarrParameters[0] = lintYear;
                        lobjProcessPensionFile.iarrParameters[1] = true;
                        lobjProcessPensionFile.iarrParameters[2] = ldtbCorrected1099rdata;
                        lobjProcessPensionFile.CreateOutboundFile(73);
                    }
                }
                idlgUpdateProcessLog("Creating Corrected IRS File finished successfully", "INFO", istrProcessName);                
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Creating Corrected 1099r IRS File failed", "INFO", istrProcessName);
                throw ex;
            }
        }
        private void LoadPayment1099rRequest()
        {
            ibusPayment1099rRequest = new busPayment1099rRequest { icdoPayment1099rRequest = new cdoPayment1099rRequest() };
            DataTable ldt1099rRequests = busBase.Select("entPayment1099rRequest.LoadPayment1099RRequest", null);
            if (ldt1099rRequests.Rows.Count > 0)
            {
                ibusPayment1099rRequest.icdoPayment1099rRequest.LoadData(ldt1099rRequests.Rows[0]);
            }
        }

        public int iintTaxYear { get; set; }
    }
}
