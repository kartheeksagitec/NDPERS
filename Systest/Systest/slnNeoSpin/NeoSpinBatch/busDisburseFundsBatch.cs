using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.IO;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.DBUtility;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using Sagitec.ExceptionPub;
using Sagitec.Common;
using Sagitec.DataObjects;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace NeoSpinBatch
{
    /// <summary>
    /// Disburse funds to Provider and Vendors.
    /// </summary>
    class busDisburseFundsBatch : busNeoSpinBatch
    {
        busBase lobjBase = new busBase();
        bool lblnCheckAvailable = false;
        private Collection<busACHProviderReportData> _iclbACHProviderReportData;
        public Collection<busACHProviderReportData> iclbACHProviderReportData
        {
            get { return _iclbACHProviderReportData; }
            set { _iclbACHProviderReportData = value; }
        }
        public int BatchRequestID { get; set; }
        // PIR 24921 added because a null record is inserted when diff comp data exceuted
        public bool iblnBatchRequestID { get; set; }
        ArrayList iarrProviders = new ArrayList();
        ArrayList iarrInsurancePlansRequested = new ArrayList();
        public busPaymentSchedule ibusPaymentSchedule { get; set; }
        public void DisburseFunds()
        {
            try
            {
                istrProcessName = "Disburse Funds to Providers and Vendors Batch";

                ibusPaymentSchedule = busPayeeAccountHelper.GetPaymentSchedule(iobjSystemManagement.icdoSystemManagement.batch_date, "VNPM");
                
                GenerateDCFileOut();
                GenerateDeffCompFileOut();
                GenerateInsuranceFileOut();
                CreateVendorPaymentsforStateTaxCommisioner();
                int lintBatchRequestID = CreateVendorPayments();

                busPaymentProcess lobjPaymentProcess = new busPaymentProcess();
                lobjPaymentProcess.CreateGL(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id,
                    iobjSystemManagement.icdoSystemManagement.batch_date, busConstant.GLSourceTypeValueVendorPayment, iobjBatchSchedule.batch_schedule_id);
                ibusPaymentSchedule.icdoPaymentSchedule.process_date = DateTime.Today;
                ibusPaymentSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleStatusProcessed;
                ibusPaymentSchedule.icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusProcessed;
                ibusPaymentSchedule.icdoPaymentSchedule.Update();

                string lstrBankofNorthDakotaOrgCode = busGlobalFunctions.GetData1ByCodeValue(5012, busConstant.Provider_BankOfNorthDakota, iobjPassInfo);
                string lstrWaddellAndReedOrgCode = busGlobalFunctions.GetData1ByCodeValue(5012, busConstant.Provider_WaddellAndReed, iobjPassInfo);
                try
                {
                    //Create Check File
                    idlgUpdateProcessLog("Create Check Files", "INFO", istrProcessName);
                    DataTable ldtCheckFile = busBase.Select("cdoPaymentHistoryDistribution.LoadCheckPaymentDistribution", 
                        new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                    ldtCheckFile = ldtCheckFile.AsEnumerable()
                                    .Where(o => o.Field<string>("istrPersonIdOrOrgCode") != lstrBankofNorthDakotaOrgCode &&
                                        o.Field<string>("istrPersonIdOrOrgCode") != lstrWaddellAndReedOrgCode)
                                    .AsDataTable();

                    if (ldtCheckFile.Rows.Count > 0)
                    {
                        CreateCheckFiles(ldtCheckFile);
                        idlgUpdateProcessLog("Check Files created successfully", "INFO", istrProcessName);
                    }
                    else
                        idlgUpdateProcessLog("No records exist", "INFO", istrProcessName);
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Creation of Check Files Failed.", "INFO", istrProcessName);
                    throw e;
                }
                busCreateReports lobjCreateReports = new busCreateReports();
                try
                {
                    idlgUpdateProcessLog("Check Register Report", "INFO", istrProcessName);
                    DataTable ldtReportResult = new DataTable();
                    ldtReportResult = lobjCreateReports.FinalCheckRegisterReport(ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                        ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                    ldtReportResult = ldtReportResult.AsEnumerable()
                                    .Where(o => o.Field<string>("person_org_id") != lstrBankofNorthDakotaOrgCode &&
                                    o.Field<string>("person_org_id") != lstrWaddellAndReedOrgCode)
                                    .AsDataTable();
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
                        CreateReport("rptCheckRegister.rpt", ldsReportResult);
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
                    idlgUpdateProcessLog("Generate Dues Withholding Report", "INFO", istrProcessName);
                    DataTable ldtReportResult = new DataTable();

                    //Backlog PIR 13031 - for report used 'FinalDuesWithholdingReportDisburse' instead of 'cdoProviderReportPayment.DuesReportVendorPaymentPaymentBatch' query.
                    ldtReportResult = lobjCreateReports.FinalDuesWithholdingReportDisburse(lintBatchRequestID);

                    if (ldtReportResult.Rows.Count > 0)
                    {
                        CreateReport("rptDuesWithholding.rpt", ldtReportResult);
                        idlgUpdateProcessLog("Dues Withholding Report generated succesfully", "INFO", istrProcessName);
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
                    throw e;
                }

                try
                {
                    idlgUpdateProcessLog("Child Support Report", "INFO", istrProcessName);
                    DataTable ldtReportResult = new DataTable();
                    ldtReportResult = new DataTable();
                    ldtReportResult = busBase.Select("cdoProviderReportPayment.ChildSupportVendorPaymentPaymentBatch",
                                                  new object[2] {ibusPaymentSchedule.icdoPaymentSchedule.payment_date,
                                            ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id});
                    if (ldtReportResult.Rows.Count > 0)
                    {
                        CreateReport("rptChildSupport.rpt", ldtReportResult);
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
                    throw e;
                }

                try
                {
                    idlgUpdateProcessLog("Vendor Payment Detail Report", "INFO", istrProcessName);
                    DataTable ldtReportResult = new DataTable();
                    ldtReportResult = new DataTable();
                    ldtReportResult = busBase.Select("cdoProviderReportPayment.rptVendorPaymentDetailReport",
                                                  new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                    if (ldtReportResult.Rows.Count > 0)
                    {
                        CreateReport("rptVendorPaymentDetailReport.rpt", ldtReportResult);
                        idlgUpdateProcessLog("Vendor Payment Detail Report generated succesfully", "INFO", istrProcessName);
                    }
                    else
                    {
                        idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Vendor Payment Detail Report Failed.", "INFO", istrProcessName);
                    throw e;
                }

                try
                {
                    idlgUpdateProcessLog("Wire Transfer Report", "INFO", istrProcessName);
                    DataTable ldtReportResult = new DataTable();
                    ldtReportResult = new DataTable();
                    ldtReportResult = busBase.Select("cdoPaymentHistoryHeader.LoadWIRETransferDetails",
                                                  new object[1] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id });
                    if (ldtReportResult.Rows.Count > 0)
                    {
                        CreateReport("rptWireTransferSchedule.rpt", ldtReportResult);
                        idlgUpdateProcessLog("Wire Transfer Report generated succesfully", "INFO", istrProcessName);
                    }
                    else
                    {
                        idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Wire Transfer Report Failed.", "INFO", istrProcessName);
                    throw e;
                }
                try
                {
                    CreateReportForPayeesWithMixedPayments(ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id);
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Payees with mixed payments Report Failed.", "INFO", istrProcessName);
                    throw e;
				}
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Disburse Funds to Providers and Vendors Batch Failed.", "INFO", istrProcessName);
                throw e;
            }
        }

        private void CreateVendorPaymentsforStateTaxCommisioner()
        {
            try
            {
                // Load State Tax Vendor Payment Data Where RequestBatchID=Null.
                DataTable ldtbPaymentRecords = busBase.Select("cdoProviderReportPayment.LoadStateTaxProviderAmount", new object[] { });
                Collection<busProviderReportPayment> lclbProviderReportPayment
                    = lobjBase.GetCollection<busProviderReportPayment>(ldtbPaymentRecords, "icdoProviderReportPayment");
                busPaymentHistoryHeader lbusPaymentHistoryHeader = new busPaymentHistoryHeader();
                if (lclbProviderReportPayment.Count > 0 && lclbProviderReportPayment[0].icdoProviderReportPayment.amount > 0.0m)
                {
                    //Get Latest check book
					//Backlog PIR 938
                    busPaymentCheckBook lobjAvailableCheckbook = busPayeeAccountHelper.GetPaymentCheckBookForGivenDate(iobjSystemManagement.icdoSystemManagement.batch_date, 0, busConstant.PlanBenefitTypeRetirement);
                    //Check available number of checks 
                    if (lobjAvailableCheckbook != null)
                        lblnCheckAvailable = lclbProviderReportPayment.Count <=
                        Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.max_check_number) -
                        Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number) ? true : false;
                    //if it is less than number records in Number of Payment headers to be created,then skip the payment process for all DC providers
                    if (lblnCheckAvailable)
                    {
                        try
                        {
                            idlgUpdateProcessLog("Creating Payment History Details State Tax Commissioner", "INFO", istrProcessName);
                            int lintLastCheckNumber = Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number);
                            int lintLastCheckNumberFromVendor = lintLastCheckNumber;

                            //Create Payment History header
                            lbusPaymentHistoryHeader.CreateVendorPaymentHistoryHeader(lclbProviderReportPayment[0].icdoProviderReportPayment.provider_org_id
                                , busConstant.PlanIdMain, iobjSystemManagement.icdoSystemManagement.batch_date);//PIR 20232 ?code
                            //Create Payment History details
                            lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(lclbProviderReportPayment[0].icdoProviderReportPayment.amount
                                , busConstant.VendorPaymentItemStateTax, 0);
                            //Create Payment History Check details
                            lintLastCheckNumberFromVendor = lbusPaymentHistoryHeader.CreateVendorPaymentDistributionDetails(lclbProviderReportPayment[0].icdoProviderReportPayment.amount, lintLastCheckNumberFromVendor);
                            //Update Check with last check number
                            if (lintLastCheckNumberFromVendor != lintLastCheckNumber)
                            {
                                lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number = lintLastCheckNumberFromVendor.ToString();
                                lobjAvailableCheckbook.icdoPaymentCheckBook.Update();
                            }
                            idlgUpdateProcessLog("Payment History Details for State Tax commissioner are created", "INFO", istrProcessName);
                        }
                        catch (Exception e)
                        {
                            idlgUpdateProcessLog("Creating Payment History Details for State Tax commissioner is failed", "INFO", istrProcessName);
                            throw e;
                        }
                    }
                    else
                    {
                        idlgUpdateProcessLog("The Check Book has reached the Maximum Limit.", "INFO", istrProcessName);
                        throw new Exception();
                    }

                    busProviderReportDataBatchRequest lobjRequest = new busProviderReportDataBatchRequest { icdoProviderReportDataBatchRequest = new cdoProviderReportDataBatchRequest() };
                    //Update Batch Request ID in Provider Report Payment table for state tax vendor records
                    DataTable ldtbStateTaxPaymentRecords = busBase.Select("cdoProviderReportPayment.LoadStateTaxVendorRecords", new object[] { });
                    foreach (DataRow ldtrStateTax in ldtbStateTaxPaymentRecords.Rows)
                    {
                        busProviderReportPayment lobjProviderReportPayment = new busProviderReportPayment { icdoProviderReportPayment = new cdoProviderReportPayment() };
                        lobjProviderReportPayment.ibusProviderReportDataBatchRequest = new busProviderReportDataBatchRequest { icdoProviderReportDataBatchRequest = new cdoProviderReportDataBatchRequest() };
                        lobjProviderReportPayment.icdoProviderReportPayment.LoadData(ldtrStateTax);
                        lobjProviderReportPayment.ibusProviderReportDataBatchRequest.icdoProviderReportDataBatchRequest.LoadData(ldtrStateTax);
                        if (lobjRequest.icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id == 0)
                        {
                            lobjRequest = lobjProviderReportPayment.ibusProviderReportDataBatchRequest;
                        }
                        lobjProviderReportPayment.icdoProviderReportPayment.payment_history_header_id = lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id;
                        lobjProviderReportPayment.icdoProviderReportPayment.batch_request_id
                            = lobjProviderReportPayment.ibusProviderReportDataBatchRequest.icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id;
                        lobjProviderReportPayment.icdoProviderReportPayment.Update();
                    }
                    if (lobjRequest.icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id > 0)
                    {
                        lobjRequest.icdoProviderReportDataBatchRequest.status_value = busConstant.Vendor_Payment_Status_Processed;
                        lobjRequest.icdoProviderReportDataBatchRequest.Update();
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                throw e;
            }
        }
        private int CreateVendorPayments()
        {
            // Load Vendor Payment Data Where RequestBatchID=Null.
            DataTable ldtbPaymentRecords = busBase.Select("cdoPaymentHistoryHeader.LoadVendorPaymentHistoryHeader", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            Collection<busProviderReportPayment> lclbProviderReportPayment = lobjBase.GetCollection<busProviderReportPayment>(ldtbPaymentRecords, "icdoProviderReportPayment");

            Collection<busPaymentHistoryHeader> lclbPaymentHistoryHeader = new Collection<busPaymentHistoryHeader>();
            iarrProviders = new ArrayList();
            if (lclbProviderReportPayment.Count > 0)
            {
                //Get Latest check 
				//Backlog PIR 938
                busPaymentCheckBook lobjAvailableCheckbook = busPayeeAccountHelper.GetPaymentCheckBookForGivenDate(iobjSystemManagement.icdoSystemManagement.batch_date, 0, busConstant.PlanBenefitTypeRetirement);
                //Check available number of checks 
                if (lobjAvailableCheckbook != null)
                    lblnCheckAvailable = lclbProviderReportPayment.Count <=
                    Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.max_check_number) -
                    Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number) ? true : false;
                //if it is less than number records in Number of Payment headers to be created,then skip the payment process for all DC providers
                if (lblnCheckAvailable)
                {
                    try
                    {
                        idlgUpdateProcessLog("Creating Vendor Payment History Details", "INFO", istrProcessName);
                        int lintLastCheckNumber = Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number);
                        int lintLastCheckNumberFromVendor = lintLastCheckNumber;
                        foreach (busProviderReportPayment lobjProviderReportPayment in lclbProviderReportPayment)
                        {
                            busPaymentHistoryHeader lbusPaymentHistoryHeader = new busPaymentHistoryHeader();
                            

                            DataTable ldtbPaymentDetail = busBase.Select("cdoPaymentHistoryDetail.CreateVendorPaymentHistoryDetails",
                                                 new object[2] { lobjProviderReportPayment.icdoProviderReportPayment.provider_org_id, iobjSystemManagement.icdoSystemManagement.batch_date });
                            if (ldtbPaymentDetail.Rows.Count > 0)
                            {
                                decimal ldecTotalAmount = ldtbPaymentDetail.AsEnumerable()
                                                                           .Sum(o => o.Field<decimal>("amount"));
                                if (ldecTotalAmount > 0.00M)
                                {
                                    iarrProviders.Add(lobjProviderReportPayment.icdoProviderReportPayment.provider_org_id);
                                    //Create Payment History header
                                    lbusPaymentHistoryHeader.CreateVendorPaymentHistoryHeader(lobjProviderReportPayment.icdoProviderReportPayment.provider_org_id
                                                                , busConstant.PlanIdMain, iobjSystemManagement.icdoSystemManagement.batch_date);//PIR 20232 ?code
                                    //Create Payment History details
                                    foreach (DataRow dr in ldtbPaymentDetail.Rows)
                                    {
                                        if (dr["payment_item_type_id"] != DBNull.Value)
                                        {
                                            lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(Convert.ToDecimal(dr["amount"])
                                                , string.Empty, Convert.ToInt32(dr["payment_item_type_id"]));
                                        }
                                    }
                                    //Create Payment History Check details
                                    lintLastCheckNumberFromVendor = lbusPaymentHistoryHeader.CreateVendorPaymentDistributionDetails(ldecTotalAmount, lintLastCheckNumberFromVendor);
                                    lclbPaymentHistoryHeader.Add(lbusPaymentHistoryHeader);
                                }
                            }
                        }
                        //Update Check with last check number    
                        if (lintLastCheckNumberFromVendor != lintLastCheckNumber)
                        {
                            lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number = lintLastCheckNumberFromVendor.ToString();
                            lobjAvailableCheckbook.icdoPaymentCheckBook.Update();
                        }
                        idlgUpdateProcessLog("Payment History Details for Vendors are created", "INFO", istrProcessName);
                    }
                    catch(Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog("Creating Payment History Details for Vendors is failed", "INFO", istrProcessName);
                        throw e;
                    }
                }
                else
                {
                    idlgUpdateProcessLog("The Check Book has reached the Maximum Limit.", "INFO", istrProcessName);
                    throw new Exception();
                }
            }
            try
            {
                idlgUpdateProcessLog("Creating ACH Outbound File for Provider Payment", "INFO", istrProcessName);
                /// Generate ACH File out for Provider.
                busProcessOutboundFile lobjProcessACHFile = new busProcessOutboundFile();
                LoadACHProviderReportPaymentData();
                /// Generates ACH only if Record Exists
                if (_iclbACHProviderReportData.Count > 0)
                {
                    lobjProcessACHFile.iarrParameters = new object[3];
                    lobjProcessACHFile.iarrParameters[0] = _iclbACHProviderReportData;
                    lobjProcessACHFile.iarrParameters[1] = busConstant.Provider_Vendor;
                    lobjProcessACHFile.iarrParameters[2] = busConstant.ACHFileNameRetirmentVendorPayment;
                    lobjProcessACHFile.CreateOutboundFile(37);
                }
                idlgUpdateProcessLog(" ACH Outbound File for Provider Payment is created ", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog(" Creating ACH Outbound File for Provider Payment is failed ", "INFO", istrProcessName);
                throw e;
            }
            //Update Batch Request ID in Provider Report Payment table for all except state tax vendor records
            if (iblnBatchRequestID == false)
            {
                int lintBatchRequestID = InsertBatchRequestID();
                BatchRequestID = lintBatchRequestID;
            }
            DataTable ldtbNonStateTaxPaymentRecords = busBase.Select("cdoProviderReportPayment.LoadVendorPaymentRecords",
                new object[2] { ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id, iobjSystemManagement.icdoSystemManagement.batch_date });
            if (ldtbNonStateTaxPaymentRecords.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Update Batch Request ID for Vendor Payment Data.", "INFO", istrProcessName);
                foreach (DataRow ldtrVendorPayments in ldtbNonStateTaxPaymentRecords.Rows)
                {
                    if (ldtrVendorPayments["provider_org_id"] != DBNull.Value && iarrProviders.Contains(Convert.ToInt32(ldtrVendorPayments["provider_org_id"])))
                    {
                        busProviderReportPayment lobjProviderReportPayment = new busProviderReportPayment { icdoProviderReportPayment = new cdoProviderReportPayment() };
                        lobjProviderReportPayment.icdoProviderReportPayment.LoadData(ldtrVendorPayments);
                        lobjProviderReportPayment.icdoProviderReportPayment.payment_history_header_id =
                            lclbPaymentHistoryHeader.Where(o =>
                                o.icdoPaymentHistoryHeader.org_id == lobjProviderReportPayment.icdoProviderReportPayment.provider_org_id).Select(o =>
                                    o.icdoPaymentHistoryHeader.payment_history_header_id).FirstOrDefault();
                        lobjProviderReportPayment.icdoProviderReportPayment.batch_request_id = BatchRequestID;
                        lobjProviderReportPayment.icdoProviderReportPayment.Update();
                    }
                }
            } 
            //PIR 4711 - Insert new report - VendorPaymentReportToIRS
            int lintIRSOrgID = GetIRSOrgID();
            DataTable ldtbIRSPaymentRecords = busBase.Select("cdoProviderReportPayment.rptIRSPaymentReport",
             new object[2] { BatchRequestID, lintIRSOrgID });
            
            if (ldtbIRSPaymentRecords.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Creating IRS Payment Report", "INFO", istrProcessName);
                try
                {
                    CreateReport("rptIRSVendorPayment.rpt", ldtbIRSPaymentRecords);
                    idlgUpdateProcessLog("IRS Vendor Payment Report generated succesfully", "INFO", istrProcessName);
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("IRS Vendor Payment Report Failed.", "INFO", istrProcessName);
                    throw e;
                }
            }
            else
            {
                idlgUpdateProcessLog("IRS Vendor Payment Report not generated", "INFO", istrProcessName);
            }

            return BatchRequestID;
        }

        private int GetIRSOrgID()
        {
            int lintIRSOrgID;
            lintIRSOrgID = busGlobalFunctions.GetOrgIdFromOrgCode(busGlobalFunctions.GetData1ByCodeValue(5012, busConstant.Provider_IRS, iobjPassInfo));
            return lintIRSOrgID;
        }

        private void LoadACHProviderReportPaymentData()
        {
            _iclbACHProviderReportData = new Collection<busACHProviderReportData>();
            DataTable ldtbACHData = new DataTable();
            /// Loads the ACH data based on the Benefit Type
            DataTable ldtbACHPaymenData = busBase.Select("cdoProviderReportPayment.LoadACHFileData", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            foreach (DataRow dr in ldtbACHPaymenData.Rows)
            {
                AddACHProviderData(dr);
            }
        }

        public DataTable GetDataTable()
        {
            DataTable ldtbProviderReportDataforDC = new DataTable();
            DataColumn SSN = new DataColumn("SSN");
            DataColumn Amount = new DataColumn("Amount", typeof(decimal));
            DataColumn Source = new DataColumn("Source");
            ldtbProviderReportDataforDC.Columns.Add(SSN);
            ldtbProviderReportDataforDC.Columns.Add(Amount);
            ldtbProviderReportDataforDC.Columns.Add(Source);
            return ldtbProviderReportDataforDC;
        }

        public DataTable GetDataTableEmpower()
        {
            DataTable ldtbProviderReportDataforDC = new DataTable();
            DataColumn SSN = new DataColumn("SSN");
            DataColumn SUMEEPOSTAMOUNT = new DataColumn("SUMEEPOSTAMOUNT", typeof(decimal));
            DataColumn SUMEEPREAMOUNT = new DataColumn("SUMEEPREAMOUNT", typeof(decimal));
            DataColumn SUMERPREAMOUNT = new DataColumn("SUMERPREAMOUNT", typeof(decimal));
            DataColumn TOTALSUM = new DataColumn("TOTALSUM", typeof(decimal));
            ldtbProviderReportDataforDC.Columns.Add(SSN);
            ldtbProviderReportDataforDC.Columns.Add(SUMEEPOSTAMOUNT);
            ldtbProviderReportDataforDC.Columns.Add(SUMEEPREAMOUNT);
            ldtbProviderReportDataforDC.Columns.Add(SUMERPREAMOUNT);
            ldtbProviderReportDataforDC.Columns.Add(TOTALSUM);
            return ldtbProviderReportDataforDC;
        }

        public void AddDCDataRow(DataTable adtDCTable, string astrSSN, string astrSource, decimal adecAmount)
        {
            DataRow ldr = adtDCTable.NewRow();
            ldr["SSN"] = astrSSN;
            ldr["Source"] = astrSource;
            ldr["Amount"] = adecAmount;
            adtDCTable.Rows.Add(ldr);

        }
        public void AddDCDataRowEmpower(DataTable adtDCTable, string astrSSN, decimal adecSUMEEPOSTAMOUNT, decimal adecSUMEEPREAMOUNT, decimal adecSUMERPREAMOUNT, decimal adecTOTALSUM)
        {
            DataRow ldr = adtDCTable.NewRow();
            ldr["SSN"] = astrSSN;
            ldr["SUMEEPOSTAMOUNT"] = adecSUMEEPOSTAMOUNT;
            ldr["SUMEEPREAMOUNT"] = adecSUMEEPREAMOUNT;
            ldr["SUMERPREAMOUNT"] = adecSUMERPREAMOUNT;
            ldr["TOTALSUM"] = adecTOTALSUM;

            adtDCTable.Rows.Add(ldr);

        }

        public void CreateDCFileEmpower(int aintProviderOrgID)
        {
            DataTable ldtDCRecords = busBase.Select("entProviderReportDataDC.LoadReportDataByProviderEmpower",
                                        new object[2] { aintProviderOrgID, iobjSystemManagement.icdoSystemManagement.batch_date });

            DataTable ldtbProviderReportPositiveDataforDC = GetDataTableEmpower();
            DataTable ldtbProviderReportNegativeDataforDC = GetDataTableEmpower();

            foreach (DataRow dr in ldtDCRecords.Rows)
            {
                string lstrSSN = string.Empty;
                int lintPersonID = 0, lintPlanID = 0;
                decimal ldecSUMEEPOSTAMOUNT = Convert.ToDecimal(dr["SUMEEPOSTAMOUNT"]);
                decimal ldecSUMEEPREAMOUNT = Convert.ToDecimal(dr["SUMEEPREAMOUNT"]);
                decimal ldecSUMERPREAMOUNT = Convert.ToDecimal(dr["SUMERPREAMOUNT"]);
                decimal ldecTOTALSUM = Convert.ToDecimal(dr["TOTALSUM"]);

                if (dr["ssn"] != DBNull.Value)
                    lstrSSN = dr["ssn"].ToString();
                if (dr["person_id"] != DBNull.Value)
                    lintPersonID = Convert.ToInt32(dr["person_id"]);
                if (dr["plan_id"] != DBNull.Value)
                    lintPlanID = Convert.ToInt32(dr["plan_id"]);

                if (ldecTOTALSUM > 0)
                    AddDCDataRowEmpower(ldtbProviderReportPositiveDataforDC, lstrSSN, ldecSUMEEPOSTAMOUNT, ldecSUMEEPREAMOUNT, ldecSUMERPREAMOUNT, ldecTOTALSUM);

                if (ldecTOTALSUM < 0)
                    AddDCDataRowEmpower(ldtbProviderReportNegativeDataforDC, lstrSSN, ldecSUMEEPOSTAMOUNT, ldecSUMEEPREAMOUNT, ldecSUMERPREAMOUNT, ldecTOTALSUM);
            }
                if (ldtbProviderReportPositiveDataforDC.Rows.Count > 0)
                {
                    busProcessOutboundFile lobjProcessDCFile = new busProcessOutboundFile();
                    idlgUpdateProcessLog("Generating Positive Contribution Outbound File for DC Provider ", "INFO", istrProcessName);

                    lobjProcessDCFile = new busProcessOutboundFile();
                    lobjProcessDCFile.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessDCFile.iarrParameters = new object[2];
                    lobjProcessDCFile.iarrParameters[0] = ldtbProviderReportPositiveDataforDC;
                    lobjProcessDCFile.iarrParameters[1] = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1);

                    lobjProcessDCFile.CreateOutboundFile(108);
                    idlgUpdateProcessLog("Positive Contribution Outbound File for DC Provider is created ", "INFO", istrProcessName);

            }

            if (ldtbProviderReportNegativeDataforDC.Rows.Count > 0)
                {
                    busProcessOutboundFile lobjProcessDCFile = new busProcessOutboundFile();
                    idlgUpdateProcessLog("Generating Negative Contribution Outbound File for DC Provider ", "INFO", istrProcessName);

                    lobjProcessDCFile = new busProcessOutboundFile();
                    lobjProcessDCFile.iobjSystemManagement = iobjSystemManagement;
                    utlPassInfo.iobjPassInfo = iobjPassInfo;
                    lobjProcessDCFile.iarrParameters = new object[2];

                    lobjProcessDCFile.iarrParameters[0] = ldtbProviderReportNegativeDataforDC;
                    lobjProcessDCFile.iarrParameters[1] = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1);
                    lobjProcessDCFile.CreateOutboundFile(109);
                    idlgUpdateProcessLog("Negative Contribution Outbound File for DC Provider is created ", "INFO", istrProcessName);

            }
        }
        public void DCFileforFidelity(int aintProviderOrgID)
        {
            DataTable ldtbDCRecordsBySSN = busBase.Select("entProviderReportDataDC.LoadReportDataByProvider",
                                                       new object[2] { aintProviderOrgID, iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtbProviderReportPositiveDataforDC = GetDataTable();
            DataTable ldtbProviderReportNegativeDataforDC = GetDataTable();
            foreach (DataRow drDC in ldtbDCRecordsBySSN.Rows)
            {
                int lintPersonID = 0, lintPlanID = 0;
                string lstrSSN = string.Empty, lstrEEPreTax = string.Empty, lstrEEPostTax = string.Empty, lstrER = string.Empty;
                if (drDC["ssn"] != DBNull.Value)
                    lstrSSN = drDC["ssn"].ToString();
                if (drDC["person_id"] != DBNull.Value)
                    lintPersonID = Convert.ToInt32(drDC["person_id"]);
                if (drDC["plan_id"] != DBNull.Value)
                    lintPlanID = Convert.ToInt32(drDC["plan_id"]);
                lstrEEPreTax = "I"; lstrEEPostTax = "Q"; lstrER = "F";
                decimal SumOfPreTaxEmpPickupMmbrInt = Convert.ToDecimal(drDC["SumOfPreTaxEmpPickupMmbrInt"]);
                decimal SumOfPostTax = Convert.ToDecimal(drDC["SumOfPostTax"]);
                decimal SumofEREmpInt = Convert.ToDecimal(drDC["SumofEREmpInt"]);
                   if (SumOfPreTaxEmpPickupMmbrInt > 0) 
                      AddDCDataRow(ldtbProviderReportPositiveDataforDC, lstrSSN, lstrEEPreTax, SumOfPreTaxEmpPickupMmbrInt);
                   if (SumOfPostTax > 0)
                      AddDCDataRow(ldtbProviderReportPositiveDataforDC, lstrSSN, lstrEEPostTax, SumOfPostTax);
                   if(SumofEREmpInt > 0)
                      AddDCDataRow(ldtbProviderReportPositiveDataforDC, lstrSSN, lstrER, SumofEREmpInt);
                   if (SumOfPreTaxEmpPickupMmbrInt < 0)
                       AddDCDataRow(ldtbProviderReportNegativeDataforDC, lstrSSN, lstrEEPreTax, SumOfPreTaxEmpPickupMmbrInt);
                   if (SumOfPostTax < 0)
                       AddDCDataRow(ldtbProviderReportNegativeDataforDC, lstrSSN, lstrEEPostTax, SumOfPostTax);
                   if (SumofEREmpInt < 0)
                       AddDCDataRow(ldtbProviderReportNegativeDataforDC, lstrSSN, lstrER, SumofEREmpInt);
            }
            Collection<busProviderReportDataDC> lclbPositiveContribution = new Collection<busProviderReportDataDC>();
            foreach (DataRow dr in ldtbProviderReportPositiveDataforDC.Rows)
            {
                busProviderReportDataDC lobjProviderReportDataDC = new busProviderReportDataDC();
                lobjProviderReportDataDC.icdoProviderReportDataDc = new cdoProviderReportDataDc();
                lobjProviderReportDataDC.icdoProviderReportDataDc.LoadData(dr);
                lobjProviderReportDataDC.ldclTotalContributionAmount = Convert.ToDecimal(dr["Amount"]);
                lobjProviderReportDataDC.icdoProviderReportDataDc.Source = Convert.ToString(dr["Source"]);
                lclbPositiveContribution.Add(lobjProviderReportDataDC);
            }
            Collection<busProviderReportDataDC> lclbNegativeContribution = new Collection<busProviderReportDataDC>();
            foreach (DataRow dr in ldtbProviderReportNegativeDataforDC.Rows)
            {
                busProviderReportDataDC lobjProviderReportDataDC = new busProviderReportDataDC();
                lobjProviderReportDataDC.icdoProviderReportDataDc = new cdoProviderReportDataDc();
                lobjProviderReportDataDC.icdoProviderReportDataDc.LoadData(dr);
                lobjProviderReportDataDC.ldclTotalContributionAmount = Convert.ToDecimal(dr["Amount"]);
                lobjProviderReportDataDC.icdoProviderReportDataDc.Source = Convert.ToString(dr["Source"]);
                lclbNegativeContribution.Add(lobjProviderReportDataDC);
            }
            try
            {
                // Generate ACH File out for Provider.63
                busProcessOutboundFile lobjProcessDCFile = new busProcessOutboundFile();

                /// Generates ACH only if Record Exists
                if (lclbPositiveContribution.Count > 0)
                {
                    idlgUpdateProcessLog("Generating Positive Contribution Outbound File for DC Provider ", "INFO", istrProcessName);
                
                    lobjProcessDCFile.iarrParameters = new object[2];
                    lobjProcessDCFile.iarrParameters[0] = lclbPositiveContribution;
                    lobjProcessDCFile.iarrParameters[1] = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1);
                    lobjProcessDCFile.CreateOutboundFile(86);

                    idlgUpdateProcessLog("Positive Contribution Outbound File for DC Provider is created ", "INFO", istrProcessName);
                }                
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Creating Positive Contribution Outbound File for DC Provider is failed ", "INFO", istrProcessName);
                throw e;
            }
            try
            {
                busProcessOutboundFile lobjProcessDCFile = new busProcessOutboundFile();

                /// Generates ACH only if Record Exists
                if (lclbNegativeContribution.Count > 0)
                {
                    idlgUpdateProcessLog("Generating Negative Contribution Outbound File for DC Provider ", "INFO", istrProcessName);
                
                    lobjProcessDCFile.iarrParameters = new object[2];
                    lobjProcessDCFile.iarrParameters[0] = lclbNegativeContribution;
                    lobjProcessDCFile.iarrParameters[1] = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1);
                    lobjProcessDCFile.CreateOutboundFile(87);

                    idlgUpdateProcessLog("Negative Contribution Outbound File for DC Provider is created ", "INFO", istrProcessName);
                }                
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Creating Negative Contribution Outbound File for DC Provider is failed ", "INFO", istrProcessName);
                throw e;
            }
        }
        public void GenerateDCFileOut()
        {
            // Load DC Provider Data Where RequestBatchID=Null Group by org II.
            DataTable ldtbDCRecordsByOrg = busBase.Select("entProviderReportDataDC.LoadDistinctProviderOrgCodeID", new object[] { });
            iarrProviders = new ArrayList();
            if (ldtbDCRecordsByOrg.Rows.Count > 0)
            {
                bool lblnfileCreated = false;
                try
                {
                    idlgUpdateProcessLog("Generating DC Provider Outbound File", "INFO", istrProcessName);
                    // Generate DC File out for Provider.
                    foreach (DataRow dr in ldtbDCRecordsByOrg.Rows)
                    {
                        int lintProviderOrgID = Convert.ToInt32(dr["PROVIDER_ORG_ID"]);
                        string lstrProviderOrgCodeID = Convert.ToString(dr["ORG_CODE"]);
                        string lstrVendorPaymentFlag = busGlobalFunctions.GetData1ByCodeValue(52, "VPFG", iobjPassInfo);
                        if (lstrVendorPaymentFlag.Equals(busConstant.Flag_Yes))
                        {
                            CreateDCFileEmpower(lintProviderOrgID);
                            //DCFileforFidelity(lintProviderOrgID);
                        }
                        else
                        {
                            busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                            lobjProcessFiles.iarrParameters = new object[3];
                            lobjProcessFiles.iarrParameters[0] = lstrProviderOrgCodeID;
                            lobjProcessFiles.iarrParameters[1] = iobjSystemManagement.icdoSystemManagement.batch_date;
                            lobjProcessFiles.iarrParameters[2] = iobjBatchSchedule.email_notification;
                            lobjProcessFiles.CreateOutboundFile(33);
                        }
                    }
                    lblnfileCreated = true;
                    idlgUpdateProcessLog("DC Provider Outbound File is generated", "INFO", istrProcessName);
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Creating DC Provider Outbound File is failed ", "INFO", istrProcessName);
                    throw e;
                }
                // Load DC Provider Data Where RequestBatchID=Null.
                DataTable ldtbDCRecords = busBase.Select("entProviderReportDataDC.LoadProviderDCData", new object[1] {iobjSystemManagement.icdoSystemManagement.batch_date });
                Collection<busProviderReportDataDC> lclbProviderReportDataDC = lobjBase.GetCollection<busProviderReportDataDC>(ldtbDCRecords, "icdoProviderReportDataDc");
                
                //if records exist in Provider Report DC Data,then create payment details for each org and plan
                if (lclbProviderReportDataDC.Count > 0 && lblnfileCreated)
                {
                    //Load vendor payment amount details group by Org
                    var lvarDCProviderDataByOrg = from lobjDCProviderData in lclbProviderReportDataDC
                                                  group lobjDCProviderData by new { lobjDCProviderData.icdoProviderReportDataDc.provider_org_id, lobjDCProviderData.icdoProviderReportDataDc.plan_id }
                                                      into ProviderDataByOrg
                                                      select new
                                                      {
                                                          lintPlanID = ProviderDataByOrg.Key.plan_id,
                                                          lintOrgID = ProviderDataByOrg.Key.provider_org_id,
                                                          ldecContributionAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataDc.ee_contribution) +
                                                                                   ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataDc.ee_pre_tax) +
                                                                                   ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataDc.ee_employer_pickup) +
                                                                                   ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataDc.er_contribution) +
                                                                                   ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataDc.member_interest) +
                                                                                   ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataDc.employer_interest) +
                                                                                   ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataDc.employer_rhic_interest)
                                                      };
                    lvarDCProviderDataByOrg = lvarDCProviderDataByOrg.Where(o => o.ldecContributionAmount > 0.00M);
                    //Get Latest check book
					//Backlog PIR 938
                    busPaymentCheckBook lobjAvailableCheckbook = busPayeeAccountHelper.GetPaymentCheckBookForGivenDate(iobjSystemManagement.icdoSystemManagement.batch_date, 0, busConstant.PlanBenefitTypeRetirement);
                    //Check available number of checks 
                    if (lobjAvailableCheckbook != null)
                        lblnCheckAvailable = lvarDCProviderDataByOrg.Count() <=
                        Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.max_check_number) -
                        Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number) ? true : false;
                    //if it is less than number records in Number of Payment headers to be created,then skip the payment process for all DC providers
                    if (lblnCheckAvailable)
                    {
                        try
                        {
                            idlgUpdateProcessLog("Creating Payment History Details for DC Providers", "INFO", istrProcessName);
                            int lintLastCheckNumber = Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number);
                            int lintLastCheckNumberAfterVendorPayments = lintLastCheckNumber;
                            Array.ForEach(lvarDCProviderDataByOrg.ToArray(), o =>
                           {                               
                               iarrProviders.Add(o.lintOrgID);
                               busPaymentHistoryHeader lbusPaymentHistoryHeader = new busPaymentHistoryHeader();
                               //Create Payment History header
                               lbusPaymentHistoryHeader.CreateVendorPaymentHistoryHeader(o.lintOrgID, o.lintPlanID, iobjSystemManagement.icdoSystemManagement.batch_date);
                               //Create Payment History details
                               lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(o.ldecContributionAmount, busConstant.VendorPaymentItemDCContrib, 0);
                               //Create Payment History Check details
                               lintLastCheckNumberAfterVendorPayments = lbusPaymentHistoryHeader.CreateVendorPaymentDistributionDetails(o.ldecContributionAmount, lintLastCheckNumberAfterVendorPayments);

                           });
                            //Update Check with last check number
                            if (lintLastCheckNumber != lintLastCheckNumberAfterVendorPayments)
                            {
                                lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number = lintLastCheckNumberAfterVendorPayments.ToString();
                                lobjAvailableCheckbook.icdoPaymentCheckBook.Update();
                            }
                            idlgUpdateProcessLog("Payment History Details for DC Providers are created", "INFO", istrProcessName);
                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("Creating Payment History Details for DC Providers is failed", "INFO", istrProcessName);
                            throw e;
                        }
                    }
                    else
                    {
                        idlgUpdateProcessLog("The Check Book has reached the Maximum Limit.", "INFO", istrProcessName);
                        throw new Exception();
                    }
                }

                //Files added for positive and negative contributions separately.
                try
                {
                    idlgUpdateProcessLog("Generating ACH Outbound File DC Provider ", "INFO", istrProcessName);
                    // Generate ACH File out for Provider.63
                    busProcessOutboundFile lobjProcessACHFile = new busProcessOutboundFile();
                    LoadACHProviderReportData(busConstant.Provider_Retirement);
                    /// Generates ACH only if Record Exists
                    if (_iclbACHProviderReportData.Count > 0)
                    {
                        lobjProcessACHFile.iarrParameters = new object[3];
                        lobjProcessACHFile.iarrParameters[0] = _iclbACHProviderReportData;
                        lobjProcessACHFile.iarrParameters[1] = busConstant.Provider_Retirement;
                        lobjProcessACHFile.iarrParameters[2] = string.Empty;
                        lobjProcessACHFile.CreateOutboundFile(37);
                    }
                    idlgUpdateProcessLog(" ACH Outbound File DC Provider is created ", "INFO", istrProcessName);
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog(" Creating ACH Outbound File DC Provider is failed ", "INFO", istrProcessName);
                    throw e;
                }

                //int lintBatchRequestID = InsertBatchRequestID();
                //Added new collection to stored separte batchrequestId 
                Collection<busProviderReportDataDC> lclbProviderProviderReportDataDC = new Collection<busProviderReportDataDC>();
                if (lclbProviderReportDataDC.Count > 0)
                {
                    var lvarDCProviderDataByOrg = from lobjDCProviderData in lclbProviderReportDataDC
                                                  group lobjDCProviderData by new { lobjDCProviderData.icdoProviderReportDataDc.provider_org_id, lobjDCProviderData.icdoProviderReportDataDc.plan_id }
                                                      into ProviderDataByOrg
                                                  select new
                                                  {
                                                      lintPlanID = ProviderDataByOrg.Key.plan_id,
                                                      lintOrgID = ProviderDataByOrg.Key.provider_org_id,
                                                  };
                    Array.ForEach(lvarDCProviderDataByOrg.ToArray(), o =>
                    {
                        int lintBatchRequestID = InsertBatchRequestIDOtherPlan(o.lintOrgID, o.lintPlanID);
                        BatchRequestID = lintBatchRequestID;
                        busProviderReportDataDC lbusProviderReportDataDC = new busProviderReportDataDC() { icdoProviderReportDataDc = new cdoProviderReportDataDc() };
                        lbusProviderReportDataDC.icdoProviderReportDataDc.provider_org_id = o.lintOrgID;
                        lbusProviderReportDataDC.icdoProviderReportDataDc.plan_id = o.lintPlanID;
                        lbusProviderReportDataDC.icdoProviderReportDataDc.batch_request_id = lintBatchRequestID;
                        lclbProviderProviderReportDataDC.Add(lbusProviderReportDataDC);
                        iblnBatchRequestID = true;
                    });
                }
                idlgUpdateProcessLog("Update Batch Request ID for DC Provider Data.", "INFO", istrProcessName);
                //Updates the Batch Request ID.
                DataTable ldtbReportData = busBase.Select("entProviderReportDataDC.LoadByBatchRequestID", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
                foreach (DataRow dr in ldtbReportData.Rows)
                {
                    if (dr["PROVIDER_ORG_ID"] != DBNull.Value &&
                        iarrProviders.Contains(Convert.ToInt32(dr["PROVIDER_ORG_ID"])))
                    {
                        busProviderReportDataDC lobjDC = new busProviderReportDataDC();
                        lobjDC.icdoProviderReportDataDc = new cdoProviderReportDataDc();
                        lobjDC.icdoProviderReportDataDc.LoadData(dr);
                        //update separate batch request id for each provider
                        if (lclbProviderProviderReportDataDC.IsNotNull())
                        {
                            BatchRequestID = lclbProviderProviderReportDataDC.OrderByDescending(i => i.icdoProviderReportDataDc.batch_request_id).Where(i => i.icdoProviderReportDataDc.provider_org_id == lobjDC.icdoProviderReportDataDc.provider_org_id && i.icdoProviderReportDataDc.plan_id == lobjDC.icdoProviderReportDataDc.plan_id).FirstOrDefault().icdoProviderReportDataDc.batch_request_id;
                        }
                        lobjDC.icdoProviderReportDataDc.batch_request_id = BatchRequestID;
                        lobjDC.icdoProviderReportDataDc.Update();
                    }
                }
            }
        }

     
        public void GenerateDeffCompFileOut()
        {
            bool lblnfileCreated = false;
            // Load DeffComp Provider Data Where RequestBatchID=Null.                 
            DataTable ldtbDeffCompRecords = busBase.Select("cdoProviderReportDataDeffComp.LoadDistinctProviderOrgCodeID", new object[] { });
            iarrProviders = new ArrayList();
            if (ldtbDeffCompRecords.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Generating  Deferred Comp Provider Outbound File", "INFO", istrProcessName);
                try
                {
                    // Generate DeffComp file out for each Provider.
                    foreach (DataRow dr in ldtbDeffCompRecords.Rows)
                    {
                        int lintProviderOrgID = Convert.ToInt32(dr["PROVIDER_ORG_ID"]);
                        string lstrProviderOrgCodeID = Convert.ToString(dr["ORG_CODE"]);
                        Collection<busCodeValue> lclbCodeValue = busGlobalFunctions.LoadCodeValueByData1(5012, lstrProviderOrgCodeID);
                        if (lclbCodeValue.Count > 0)
                        {
                            switch (lclbCodeValue[0].icdoCodeValue.code_value)
                            {
                                case busConstant.Provider_AXA:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 36);
                                    break;
                                case busConstant.Provider_Fidelity:
                                      string lstrVendorPaymentFlag = busGlobalFunctions.GetData1ByCodeValue(52,"VPFG",iobjPassInfo);
                                      if (lstrVendorPaymentFlag.Equals(busConstant.Flag_Yes))
                                      {
                                          GenerateDeferredCompensationOutFile(lstrProviderOrgCodeID);
                                      }
                                      else
                                      {
                                          GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 38);
                                      }
                                    break;
                                case busConstant.Provider_HartFordLife:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 39);
                                    break;
                                case busConstant.Provider_ING:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 44);
                                    break;
                                case busConstant.Provider_SYMETRA:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 42);
                                    break;
                                case busConstant.Provider_AIGVALIC:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 43);
                                    break;
                                case busConstant.Provider_NationWideLife:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 40);
                                    break;
                                case busConstant.Provider_KANSAS:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 41);
                                    break;
                                case busConstant.Provider_AmericanTrustCenter:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 29);
                                    break;
                                case busConstant.Provider_JacksonNationalLife:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 30);
                                    break;
                                case busConstant.Provider_KEMPER:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 31);
                                    break;
                                case busConstant.Provider_LincolnNational:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 32);
                                    break;
                                case busConstant.Provider_WaddellAndReed:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 34);
                                    break;
                                case busConstant.Provider_BankOfNorthDakota:
                                    GenerateDeferredCompOutFile(lstrProviderOrgCodeID, 35);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    lblnfileCreated = true;
                    idlgUpdateProcessLog("Deferred Comp Provider Outbound File is generated", "INFO", istrProcessName);
                }
                catch(Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Creating Deferred Comp Provider Outbound File is failed", "INFO", istrProcessName);
                    throw e;
                }                          
                // Load DC Provider Data Where RequestBatchID=Null.
                DataTable ldtbDefRecords = busBase.Select("cdoProviderReportDataDeffComp.LoadDefProviderData", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
                Collection<busProviderReportDataDeffComp> lclbProviderReportDataDef = new Collection<busProviderReportDataDeffComp>();
                foreach (DataRow ldtr in ldtbDefRecords.Rows)
                {
                    busProviderReportDataDeffComp lobjProviderReportDataDeffComp = new busProviderReportDataDeffComp { icdoProviderReportDataDeffComp = new cdoProviderReportDataDeffComp() };
                    lobjProviderReportDataDeffComp.ibusProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
                    lobjProviderReportDataDeffComp.icdoProviderReportDataDeffComp.LoadData(ldtr);
                    lobjProviderReportDataDeffComp.ibusProvider.icdoOrganization.LoadData(ldtr);
                    lclbProviderReportDataDef.Add(lobjProviderReportDataDeffComp);
                }
                //if records exist in Provider Report DC Data,then create payment details for each org and plan
                if (lclbProviderReportDataDef.Count > 0 && lblnfileCreated)
                {
                    //Load vendor payment amount details group by Org
                    var lvarDefProviderDataByOrg = from lobjDefProviderData in lclbProviderReportDataDef
                                                   group lobjDefProviderData
                                                   by new
                                                   {
                                                       lobjDefProviderData.icdoProviderReportDataDeffComp.provider_org_id,
                                                       lobjDefProviderData.ibusProvider.icdoOrganization.org_code,
                                                       lobjDefProviderData.icdoProviderReportDataDeffComp.plan_id
                                                   }
                                                       into ProviderDataByOrg
                                                       select new
                                                       {
                                                           lintPlanID = ProviderDataByOrg.Key.plan_id,
                                                           lintOrgID = ProviderDataByOrg.Key.provider_org_id,
                                                           lstrOrgCode = ProviderDataByOrg.Key.org_code,
                                                           ldecContributionAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => (lobjProviderDataByOrg.icdoProviderReportDataDeffComp.contribution_amount + lobjProviderDataByOrg.icdoProviderReportDataDeffComp.er_pretax_match))		//PIR 25920 DC 2025 Changes
                                                       };
                    //Get Latest check book
                    lvarDefProviderDataByOrg = lvarDefProviderDataByOrg.Where(o => o.ldecContributionAmount > 0.00M);
					//Backlog PIR 938
                    busPaymentCheckBook lobjAvailableCheckbook = busPayeeAccountHelper.GetPaymentCheckBookForGivenDate(iobjSystemManagement.icdoSystemManagement.batch_date, 0, busConstant.PlanBenefitTypeDeferredComp);
                    //Check available number of checks 
                    if (lobjAvailableCheckbook != null)
                        lblnCheckAvailable = lclbProviderReportDataDef.Count() <=
                        Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.max_check_number) -
                        Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number) ? true : false;
                    //if it is less than number records in Number of Payment headers to be created,then skip the payment process for all Def providers
                    if (lblnCheckAvailable)
                    {
                        try
                        {
                            idlgUpdateProcessLog("Creating Payment History Details for Deferred Comp Providers", "INFO", istrProcessName);
                            int lintLastCheckNumber = Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number) ;
                            int lintLastCheckNumberAfterVendor = lintLastCheckNumber;
                            Array.ForEach(lvarDefProviderDataByOrg.ToArray(), o =>
                            {
                                busPaymentHistoryHeader lbusPaymentHistoryHeader = new busPaymentHistoryHeader();
                                //Create payment history header for each provider
                                lbusPaymentHistoryHeader.CreateVendorPaymentHistoryHeader(o.lintOrgID, o.lintPlanID, iobjSystemManagement.icdoSystemManagement.batch_date);
                                string lstrItemCode;
                                if(o.lstrOrgCode == busGlobalFunctions.GetData1ByCodeValue(5012, busConstant.Provider_Fidelity, iobjPassInfo))
                                    lstrItemCode = busConstant.VendorPaymentItemDefContribFidelity;
                                else
                                    lstrItemCode = busConstant.VendorPaymentItemDefContrib;
                                //Create payment history details for each provider
                                lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(o.ldecContributionAmount, lstrItemCode,0);
                                //Create payment history check details for each provider
                                lintLastCheckNumberAfterVendor = lbusPaymentHistoryHeader.CreateVendorPaymentDistributionDetails(o.ldecContributionAmount, lintLastCheckNumberAfterVendor);
                             
                            });
                            //Update Check with last check number
                            if (lintLastCheckNumber != lintLastCheckNumberAfterVendor)
                            {
                                lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number = lintLastCheckNumberAfterVendor.ToString();
                                lobjAvailableCheckbook.icdoPaymentCheckBook.Update();
                            }
                            idlgUpdateProcessLog("Payment History Details for deferred comp providers are created", "INFO", istrProcessName);
                        }
                        catch(Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("Creating Payment History Details for Deferred Comp Providers is failed", "INFO", istrProcessName);
                            throw e;
                        }
                    }
                    else
                    {
                        idlgUpdateProcessLog("The Check Book has reached the Maximum Limit.", "INFO", istrProcessName);
                        throw new Exception();
                    }
                }
                //int lintBatchRequestID = InsertBatchRequestID();
                //Added new collection to stored separte batchrequestId 
                Collection<busProviderReportDataDeffComp> lclbProviderReportDataDeffComp = new Collection<busProviderReportDataDeffComp>();
                if (lclbProviderReportDataDef.Count > 0)
                {
                    //Load vendor payment amount details group by Org
                    var lvarDefProviderDataByOrg = from lobjDefProviderData in lclbProviderReportDataDef
                                                   group lobjDefProviderData
                                                   by new
                                                   {
                                                       lobjDefProviderData.icdoProviderReportDataDeffComp.provider_org_id,
                                                       lobjDefProviderData.ibusProvider.icdoOrganization.org_code,
                                                       lobjDefProviderData.icdoProviderReportDataDeffComp.plan_id
                                                   }
                                                       into ProviderDataByOrg
                                                   select new
                                                   {
                                                       lintPlanID = ProviderDataByOrg.Key.plan_id,
                                                       lintOrgID = ProviderDataByOrg.Key.provider_org_id,
                                                       lstrOrgCode = ProviderDataByOrg.Key.org_code
                                                   };
                    Array.ForEach(lvarDefProviderDataByOrg.ToArray(), o =>
                    {
                        int lintBatchRequestID = InsertBatchRequestIDOtherPlan(o.lintOrgID, o.lintPlanID);
                        BatchRequestID = lintBatchRequestID;
                        busProviderReportDataDeffComp lbusProviderReportDataDeffComp = new busProviderReportDataDeffComp() { icdoProviderReportDataDeffComp= new cdoProviderReportDataDeffComp()};
                        lbusProviderReportDataDeffComp.icdoProviderReportDataDeffComp.provider_org_id = o.lintOrgID;
                        lbusProviderReportDataDeffComp.icdoProviderReportDataDeffComp.plan_id = o.lintPlanID;
                        lbusProviderReportDataDeffComp.icdoProviderReportDataDeffComp.batch_request_id = lintBatchRequestID;
                        lclbProviderReportDataDeffComp.Add(lbusProviderReportDataDeffComp);
                        iblnBatchRequestID = true;
                    });
                }
                // Generate ACH file out for Provider
                try
                {
                    idlgUpdateProcessLog(" Creating ACH Outbound File DC Provider", "INFO", istrProcessName);
                    busProcessOutboundFile lobjProcessACHFile = new busProcessOutboundFile();
                    LoadACHProviderReportData(busConstant.Provider_DeffComp);
                    /// Generates ACH only if Record Exists
                    if (_iclbACHProviderReportData.Count > 0)
                    {
                        lobjProcessACHFile.iarrParameters = new object[3];
                        lobjProcessACHFile.iarrParameters[0] = _iclbACHProviderReportData;
                        lobjProcessACHFile.iarrParameters[1] = busConstant.Provider_DeffComp;
                        lobjProcessACHFile.iarrParameters[2] = busConstant.ACHFileNameDefCompVendorPayment;
                        lobjProcessACHFile.CreateOutboundFile(37);
                    }
                    idlgUpdateProcessLog(" ACH Outbound File Deferred Comp Provider is created ", "INFO", istrProcessName);
                }
                catch(Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog(" Creating ACH Outbound File Deferred Comp Provider is failed ", "INFO", istrProcessName);
                    throw e;
                }
                /// Updates the Batch Request ID to only the providers file were generated.
                foreach (object lobjProviderOrgCode in iarrProviders)
                {
                    DataTable ldtbReportData = busBase.Select("cdoProviderReportDataDeffComp.LoadByBatchRequestID", new object[2] { 
                                            busGlobalFunctions.GetOrgIdFromOrgCode(Convert.ToString(lobjProviderOrgCode)), iobjSystemManagement.icdoSystemManagement.batch_date});
                    foreach (DataRow dr in ldtbReportData.Rows)
                    {
                        busProviderReportDataDeffComp lobjDeffComp = new busProviderReportDataDeffComp();
                        lobjDeffComp.icdoProviderReportDataDeffComp = new cdoProviderReportDataDeffComp();
                        lobjDeffComp.icdoProviderReportDataDeffComp.LoadData(dr);
                        //update separate batch request id for each provider
                        if (lclbProviderReportDataDeffComp.IsNotNull())
                        {
                            BatchRequestID = lclbProviderReportDataDeffComp.OrderByDescending(i => i.icdoProviderReportDataDeffComp.batch_request_id).Where(i => i.icdoProviderReportDataDeffComp.provider_org_id == lobjDeffComp.icdoProviderReportDataDeffComp.provider_org_id && i.icdoProviderReportDataDeffComp.plan_id == lobjDeffComp.icdoProviderReportDataDeffComp.plan_id).FirstOrDefault().icdoProviderReportDataDeffComp.batch_request_id;
                        }
                        lobjDeffComp.icdoProviderReportDataDeffComp.batch_request_id = BatchRequestID;
                        lobjDeffComp.icdoProviderReportDataDeffComp.Update();
                    }
                }
            }
        }

       
        private void GenerateDeferredCompOutFile(string AstrProviderOrgCode, int AintFileID)
        {
            /// PIR ID 313 - Generate Deferred Comp files for Provider only if the Contribution Amount is greater than Zero
            decimal ldclTotalContributionAmount = Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoProviderReportDataDeffComp.GetSumGroupByProviderOrg",
                                                    new object[2] { AstrProviderOrgCode, iobjSystemManagement.icdoSystemManagement.batch_date },
                                                    iobjPassInfo.iconFramework,
                                                    iobjPassInfo.itrnFramework));
            
            if (ldclTotalContributionAmount > 0)
            {
                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                lobjProcessFiles.iarrParameters = new object[3];
                lobjProcessFiles.iarrParameters[0] = AstrProviderOrgCode;
                lobjProcessFiles.iarrParameters[1] = iobjSystemManagement.icdoSystemManagement.batch_date;
                lobjProcessFiles.iarrParameters[2] = iobjBatchSchedule.email_notification;
                lobjProcessFiles.CreateOutboundFile(AintFileID);
                iarrProviders.Add(AstrProviderOrgCode);
            }
        }

        private void GenerateDeferredCompensationOutFile(string AstrProviderOrgCode)
        {
            int lintProviderOrgID = busGlobalFunctions.GetOrgIdFromOrgCode(AstrProviderOrgCode);
            DataTable ldtbDeffCompRecord = busBase.Select("cdoProviderReportDataDeffComp.LoadReportDataByProviderOrgCode", new object[2] { lintProviderOrgID, iobjSystemManagement.icdoSystemManagement.batch_date });
            Collection<busProviderReportDataDeffComp> lclbProviderReportData = lobjBase.GetCollection<busProviderReportDataDeffComp>(ldtbDeffCompRecord, "icdoProviderReportDataDeffComp");   //GetCollection<busProviderReportDataDeffComp>(ldtbDeffCompRecord,"icdoProviderReportDataDeffComp");
            Collection<busProviderReportDataDeffComp> lclbPositiveContribution = new Collection<busProviderReportDataDeffComp> { };
            Collection<busProviderReportDataDeffComp> lclbNegativeContribution = new Collection<busProviderReportDataDeffComp> { };

            lclbPositiveContribution = lclbProviderReportData.Where(o => o.icdoProviderReportDataDeffComp.total_contribution > 0).ToList().ToCollection();
            lclbNegativeContribution = lclbProviderReportData.Where(o => o.icdoProviderReportDataDeffComp.total_contribution < 0).ToList().ToCollection();

            try
            {
                busProcessOutboundFile lobjProcessDCFile = new busProcessOutboundFile();

                /// Generates ACH only if Record Exists
                if (lclbPositiveContribution.Count > 0)
                {
                    idlgUpdateProcessLog("Generating Positive Contribution Outbound File for  Deffered Compensation Provider ", "INFO", istrProcessName);
                
                    lobjProcessDCFile.iarrParameters = new object[2];
                    lobjProcessDCFile.iarrParameters[0] = lclbPositiveContribution;
                    lobjProcessDCFile.iarrParameters[1] = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1);
                    lobjProcessDCFile.CreateOutboundFile(110);

                    idlgUpdateProcessLog("Positive Contribution Outbound File for Deffered Compensation Provider is created ", "INFO", istrProcessName);
                }                
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog(" Creating Positive Contribution Outbound File for Deffered Compensation Provider failed ", "INFO", istrProcessName);
                throw e;
            }
            try
            {
                busProcessOutboundFile lobjProcessDCFile = new busProcessOutboundFile();

                /// Generates ACH only if Record Exists
                if (lclbNegativeContribution.Count > 0)
                {
                    idlgUpdateProcessLog("Generating Negative Contribution Outbound File for Deffered Compensation Provider ", "INFO", istrProcessName);
                
                    lobjProcessDCFile.iarrParameters = new object[2];
                    lobjProcessDCFile.iarrParameters[0] = lclbNegativeContribution;
                    lobjProcessDCFile.iarrParameters[1] = iobjSystemManagement.icdoSystemManagement.batch_date.AddDays(1);
                    lobjProcessDCFile.CreateOutboundFile(111);

                    idlgUpdateProcessLog("Negative Contribution Outbound File for Deffered Compensation Provider is created ", "INFO", istrProcessName);
                }
                
                if (lclbNegativeContribution.Count > 0 || lclbPositiveContribution.Count > 0)
                    iarrProviders.Add(AstrProviderOrgCode);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Creating Negative Contribution Outbound File for Deffered Compensation Provider failed ", "INFO", istrProcessName);
                throw e;
            }
        }

        public void GenerateInsuranceFileOut()
        {
            busBase lobjBase = new busBase();
            busProviderReportDataBatchRequest lbusProviderReportDataBatchRequest= new busProviderReportDataBatchRequest();
            DataTable ldtNonProcessedBatchRequests = new DataTable();
            if (lbusProviderReportDataBatchRequest.IsReloadInsuranceBatchRunForCurrentRequest())
            {
                ldtNonProcessedBatchRequests = busBase.Select("cdoProviderReportDataBatchRequest.GetNonProcessedRequests", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            }
            else
            {
                //PROD PIR 7974
                try
                {
                    string lstrSubject = "Disburse Funds Batch";
                    string lstrMessage = "The Insurance files to Vendors was not generated as the Re-Load Insurance batch has not been run yet.";
                    busGlobalFunctions.SendMail(iobjSystemManagement.icdoSystemManagement.email_notification, iobjBatchSchedule.email_notification, lstrSubject, lstrMessage, true, true);
                }
                catch (Exception _exc)
                {
                    ExceptionManager.Publish(_exc);
                }
            }
            Collection<busProviderReportDataInsurance> lclbProviderReportDataInsr = new Collection<busProviderReportDataInsurance>();
            Collection<busProviderReportDataMedicarePartD> lclbProviderReportDataMedicarePartD = new Collection<busProviderReportDataMedicarePartD>();
            iarrProviders = new ArrayList();
            iarrInsurancePlansRequested = new ArrayList();
            string lstrMedicareCoverageCode, lstrNonMedicareCoverageCode;
            int lintMedicareCount, lintNonMedicareCount;
            busPersonAccountGhdv lobjGHDV = new busPersonAccountGhdv();
            if (ldtNonProcessedBatchRequests.Rows.Count > 0)
            {
                #region HSA Contribution FIle
                ///PIR 7705
                if (IsHSAProviderBatchRequestExist(ldtNonProcessedBatchRequests))
                {
                    GenerateHSAContributionFile(ldtNonProcessedBatchRequests); 
                }
                #endregion
                
                DataTable ldtMedicareCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(1922);
                DataTable ldtNonMedicareCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(1923);
                //933 : flag to check whether we need to split the coverage code or not
                string lstrCheckSplitFlag = busGlobalFunctions.GetData1ByCodeValue(52, busConstant.CoverageCodeSplitFlag, iobjPassInfo);

                // Load DC Provider Data Where RequestBatchID=Null.
                DataTable ldtbInsrRecords = busBase.Select("cdoProviderReportDataInsurance.LoadInsuranceProviderData",
                                                            new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
                lclbProviderReportDataInsr =
                                lobjBase.GetCollection<busProviderReportDataInsurance>(ldtbInsrRecords, "icdoProviderReportDataInsurance");

                DataTable ldtbInsrRecordsMedicarePartD = busBase.Select("cdoProviderReportDataMedicarePartD.LoadInsuranceProviderDataMedicare",
                                                            new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
                lclbProviderReportDataMedicarePartD =
                                lobjBase.GetCollection<busProviderReportDataMedicarePartD>(ldtbInsrRecordsMedicarePartD, "icdoProviderReportDataMedicare");

                //if records exist in Provider Report DC Data,then create payment details for each org and plan
                #region otherthan medicare
                if (lclbProviderReportDataInsr.Count > 0)
                {
                    #region Payment History Details Creation

                    //Load vendor payment amount details group by Org
                    var lvarDefProviderDataByOrg = from lobjInsrProviderData in lclbProviderReportDataInsr
                                                   group lobjInsrProviderData
                                                   by new
                                                   {
                                                       lobjInsrProviderData.icdoProviderReportDataInsurance.provider_org_id,
                                                       lobjInsrProviderData.icdoProviderReportDataInsurance.plan_id
                                                   }
                                                       into ProviderDataByOrg
                                                       select new
                                                       {
                                                           lintPlanID = ProviderDataByOrg.Key.plan_id,
                                                           lintOrgID = ProviderDataByOrg.Key.provider_org_id,
                                                           ldecPremiumAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataInsurance.premium_amount),
                                                           ldecFeeAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataInsurance.fee_amount),
                                                           ldecBuydownAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataInsurance.buydown_amount), // PIR 11239
                                                           ldecMedicarePartDAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataInsurance.medicare_part_d_amt),//PIR 14271
                                                           ldecRHICAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataInsurance.rhic_amount_for_gl),
                                                           ldecHSAAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataInsurance.hsa_amount_for_gl)
                                                           //ldecHSAVendorAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataInsurance.vendor_amount_for_gl)
                                                       };
                    //Get Latest check book
					//Backlog PIR 938
                    busPaymentCheckBook lobjAvailableCheckbook = busPayeeAccountHelper.GetPaymentCheckBookForGivenDate(iobjSystemManagement.icdoSystemManagement.batch_date, 0, busConstant.PlanBenefitTypeInsurance);
                    //Check available number of checks 
                    if (lobjAvailableCheckbook != null)
                        lblnCheckAvailable = lvarDefProviderDataByOrg.Count() <=
                        Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.max_check_number) -
                        Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number) ? true : false;

                    //if it is less than number records in Number of Payment headers to be created,then skip the payment process for all Insr providers
                    if (lblnCheckAvailable)
                    {
                        try
                        {
                            idlgUpdateProcessLog("Creating Payment History Details for Insurance Providers", "INFO", istrProcessName);
                            int lintLastCheckNumber = Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number) ;
                            int lintLastCheckNumberAfterVendor = lintLastCheckNumber;
                            Array.ForEach(lvarDefProviderDataByOrg.ToArray(), o =>
                            {
                                int lintPHHId = 0;
                                if (o.ldecPremiumAmount > 0.0m)
                                {                                    
                                    iarrProviders.Add(o.lintOrgID);
                                    iarrInsurancePlansRequested.Add(o.lintPlanID);
                                    string lstrItemCode = string.Empty;
                                    busPaymentHistoryHeader lbusPaymentHistoryHeader = new busPaymentHistoryHeader();
                                    //Create payment history header for each provider and plan
                                    lbusPaymentHistoryHeader.CreateVendorPaymentHistoryHeader(o.lintOrgID, o.lintPlanID, iobjSystemManagement.icdoSystemManagement.batch_date);
                                    lintPHHId = lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id;
                                    if (o.lintPlanID == busConstant.PlanIdDental)
                                    {
                                        lstrItemCode = busConstant.VendorPaymentItemDental;
                                    }
                                    else if (o.lintPlanID == busConstant.PlanIdEAP)
                                    {
                                        lstrItemCode = busConstant.VendorPaymentItemEAP;
                                    }
                                    else if (o.lintPlanID == busConstant.PlanIdGroupHealth)
                                    {
                                        lstrItemCode = busConstant.VendorPaymentItemHealth;
                                        //lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(o.ldecFeeAmount, busConstant.VendorPaymentItemHealthFee,0);
                                    }
                                    else if (o.lintPlanID == busConstant.PlanIdGroupLife)
                                    {
                                        lstrItemCode = busConstant.VendorPaymentItemLife;
                                    }
                                    else if (o.lintPlanID == busConstant.PlanIdLTC)
                                    {
                                        lstrItemCode = busConstant.VendorPaymentItemLTC;
                                    }
                                    else if (o.lintPlanID == busConstant.PlanIdMedicarePartD)
                                    {
                                        lstrItemCode = busConstant.VendorPaymentItemMedicarePartD;
                                        //lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(o.ldecFeeAmount, busConstant.VendorPaymentItemMedicarePartDFee,0);
                                    }
                                    else if (o.lintPlanID == busConstant.PlanIdVision)
                                    {
                                        lstrItemCode = busConstant.VendorPaymentItemVision;
                                    }
                                    else if (o.lintPlanID == busConstant.PlanIdHMO)
                                    {
                                        lstrItemCode = busConstant.VendorPaymentItemHMO;
                                    }
                                    if (!string.IsNullOrEmpty(lstrItemCode))
                                    {
                                        //Create payment history details for each provider
                                        lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(o.ldecPremiumAmount, lstrItemCode, 0);
                                        
                                        // PIR 11239
                                        if (o.lintPlanID == busConstant.PlanIdGroupHealth && lstrItemCode == busConstant.VendorPaymentItemHealth)
                                        {
                                            lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(o.ldecBuydownAmount, busConstant.VendorPaymentItemBuydown, 0);
                                            lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(o.ldecMedicarePartDAmount, busConstant.VendorPaymentItemMedicarePartDAmount, 0);//PIR 14271
                                        }
                                        //PIR 7705 - Create GL for HSA Amount
                                        //if(o.ldecHSAAmount > 0)
                                        //    lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(o.ldecHSAAmount, busConstant.PAPITHSAAmount, 0);

                                        //Create payment history check details for each provider
                                        lintLastCheckNumberAfterVendor = lbusPaymentHistoryHeader.CreateVendorPaymentDistributionDetails(o.ldecPremiumAmount, lintLastCheckNumberAfterVendor);
                                    }
                                }
                                //PROD PIR 4318 : GL for fee amount and rhic amount
                                if (lintPHHId > 0)
                                {
                                    if (o.ldecFeeAmount > 0)
                                    {
                                        GenerateGLByType(o.lintPlanID, busConstant.ItemTypeHealthAdminFee, o.ldecFeeAmount, o.lintOrgID, lintPHHId);
                                    }
                                    if (o.ldecRHICAmount > 0)
                                    {
                                        GenerateGLByType(o.lintPlanID, busConstant.ItemTypeRHICAmount, o.ldecRHICAmount, o.lintOrgID, lintPHHId);
                                    }
                                    //PIR 7705 - Create GL for Vendor amount
                                    //if (o.ldecHSAVendorAmount > 0)
                                    //{
                                    //    GenerateGLByType(o.lintPlanID, busConstant.ItemTypeHSAVendorPayment, o.ldecHSAVendorAmount, o.lintOrgID, lintPHHId);
                                    //}
                                    //pir 7705 - Create GL For HSA Amount
                                    if (o.ldecHSAAmount > 0)
                                    {
                                        GenerateGLByType(o.lintPlanID, busConstant.ItemTypeHSAPremiumPayment, o.ldecHSAAmount, o.lintOrgID, lintPHHId);
                                    }
                                }
                            });
                            //Update Check with last check number
                            if (lintLastCheckNumber != lintLastCheckNumberAfterVendor)
                            {
                                lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number = lintLastCheckNumberAfterVendor.ToString();
                                lobjAvailableCheckbook.icdoPaymentCheckBook.Update();
                            }
                            idlgUpdateProcessLog("Payment History Details for Insurance providers are created", "INFO", istrProcessName);
                        }
                        catch(Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("Creating Payment History Details for Insurance providers is failed", "INFO", istrProcessName);
                            throw e;
                        }
                    }
                    else
                    {
                        idlgUpdateProcessLog("The Check Book has reached the Maximum Limit.", "INFO", istrProcessName);
                        throw new Exception();
                    }
                    #endregion
                    
                    #region ACH Outbound File Creation
                    
                    try
                    {
                        idlgUpdateProcessLog("Creating ACH Outbound File Insurance Provider ", "INFO", istrProcessName);
                        /// Generate ACH File out for Provider.
                        busProcessOutboundFile lobjProcessACHFile = new busProcessOutboundFile();
                        LoadACHProviderReportData(busConstant.Provider_Insurance);
                        /// Generates ACH only if Record Exists
                        if (_iclbACHProviderReportData.Count > 0)
                        {
                            lobjProcessACHFile.iarrParameters = new object[3];
                            lobjProcessACHFile.iarrParameters[0] = _iclbACHProviderReportData;
                            lobjProcessACHFile.iarrParameters[1] = busConstant.Provider_Insurance;
                            lobjProcessACHFile.iarrParameters[2] = busConstant.ACHFileNameInsuranceVendorPayment;
                            lobjProcessACHFile.CreateOutboundFile(37);
                        }
                        idlgUpdateProcessLog(" ACH Outbound File Insurance Provider is created ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog(" Creating ACH Outbound File Insurance Provider is failed ", "INFO", istrProcessName);
                        throw e;
                    }
                    #endregion
                    
                    foreach (DataRow dr in ldtNonProcessedBatchRequests.Rows)
                    {
                        /// Load the Batch Request Data
                        busProviderReportDataBatchRequest lobjBatchRequest = new busProviderReportDataBatchRequest();
                        lobjBatchRequest.icdoProviderReportDataBatchRequest = new cdoProviderReportDataBatchRequest();
                        lobjBatchRequest.icdoProviderReportDataBatchRequest.LoadData(dr);

                        #region HIPAA 820 File Creation
                        DateTime ldteBatchEffectiveDate = lobjBatchRequest.icdoProviderReportDataBatchRequest.effective_start_date;
                        if (lobjBatchRequest.icdoProviderReportDataBatchRequest.plan_id != 0)
                        {
                            if (lobjBatchRequest.icdoProviderReportDataBatchRequest.plan_id != busConstant.PlanIdGroupLife &&
                                lobjBatchRequest.icdoProviderReportDataBatchRequest.plan_id != busConstant.PlanIdMedicarePartD)
                            {
                                    #region PIR 933
                                //prod pir 933

                                //--start--//
                                if (iarrProviders.Contains(lobjBatchRequest.icdoProviderReportDataBatchRequest.org_id))
                                {
                                    if (lstrCheckSplitFlag == busConstant.Flag_Yes)
                                    {
                                        if (lobjBatchRequest.icdoProviderReportDataBatchRequest.plan_id == busConstant.PlanIdGroupHealth)
                                        {
                                            DataTable ldtSplitInfo = busBase.Select("cdoProviderReportDataInsuranceSplit.LoadSplitForHealthPlan",
                                                                        new object[3]{lobjBatchRequest.icdoProviderReportDataBatchRequest.org_id,
                                                                    lobjBatchRequest.icdoProviderReportDataBatchRequest.effective_start_date, iobjBatchSchedule.batch_schedule_id});

                                            DataTable ldtFinalSplitInfo = ldtSplitInfo.Clone();

                                            IEnumerable<busProviderReportDataInsurance> lenmProviderReportDataIns =
                                                lclbProviderReportDataInsr.Where(o => o.icdoProviderReportDataInsurance.provider_org_id == lobjBatchRequest.icdoProviderReportDataBatchRequest.org_id);
                                            //BLOCK TO GROUP THE CONTRACT BASED ON BELOW RULES
                                            //If member is in the group, then member is subsriber
                                            //If members is not in the group, then spouse is subscriber unless no spouse, then oldest dependent is subsriber
                                            bool lblnMedicare = false, lblnNonMedicare = false;
                                            foreach (busProviderReportDataInsurance lobjInsurance in lenmProviderReportDataIns)
                                            {
                                                DataRow[] ldarrSplit = ldtSplitInfo.FilterTable(busConstant.DataType.Numeric, "provider_report_data_insurance_comp_id",
                                                                            lobjInsurance.icdoProviderReportDataInsurance.provider_report_data_insurance_comp_id);
                                                // Medicare count needs to be check with Effective date too. 
                                                // Future dated medicare should not be included in Medicare count.
                                                // We only need to check Part B date as it will always be equal or greater than Part A.
                                                lintMedicareCount = ldarrSplit.Where(o => o.Field<string>("medicare_claim_no") != null &&
                                                                                          o.Field<DateTime?>("medicare_effective_date") != null &&
                                                                                          Convert.ToDateTime(o.Field<DateTime?>("medicare_effective_date")) <= ldteBatchEffectiveDate).Count();
                                                lintNonMedicareCount = ldarrSplit.Where(o => o.Field<string>("medicare_claim_no") == null ||
                                                                                            (o.Field<DateTime?>("medicare_effective_date") != null &&
                                                                                          Convert.ToDateTime( o.Field<DateTime?>("medicare_effective_date")) > ldteBatchEffectiveDate)).Count();

                                                IEnumerable<DataRow> lenmdrMember = ldarrSplit.Where(o => o.Field<int>("person_id") == lobjInsurance.icdoProviderReportDataInsurance.person_id);
                                                
                                                foreach (DataRow ldr in lenmdrMember)
                                                {
                                                    lblnMedicare = lblnNonMedicare = false;
                                                    if (ldr["medicare_claim_no"] == DBNull.Value ||
                                                        (ldr["medicare_effective_date"] != DBNull.Value && ldr["medicare_effective_date"] != null && 
                                                        Convert.ToDateTime(ldr["medicare_effective_date"]) > ldteBatchEffectiveDate))
                                                    {
                                                        ldr["cnt"] = lintNonMedicareCount;
                                                        lblnNonMedicare = true;
                                                    }
                                                    else
                                                    {
                                                        ldr["cnt"] = lintMedicareCount;
                                                        lblnMedicare = true;
                                                    }
                                                    ldtFinalSplitInfo.ImportRow(ldr);
                                                    break;
                                                }

                                                if (lblnMedicare)
                                                {
                                                    IEnumerable<DataRow> lenmdrSpouse = ldarrSplit.Where(o => 
                                                                (o.Field<string>("medicare_claim_no") == null ||
                                                                (o.Field<DateTime?>("medicare_effective_date") != null &&
                                                                 Convert.ToDateTime(o.Field<DateTime?>("medicare_effective_date")) > ldteBatchEffectiveDate)) &&
                                                                 o.Field<string>("relationship_value") == busConstant.DependentRelationshipSpouse);
                                                    if (lenmdrSpouse.Count() > 0)
                                                    {
                                                        foreach (DataRow ldr in lenmdrSpouse)
                                                        {
                                                            ldr["cnt"] = lintNonMedicareCount;                                                            
                                                            ldtFinalSplitInfo.ImportRow(ldr);
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        DataRow ldrDepenedent = ldarrSplit.Where(o => 
                                                                  (o.Field<string>("medicare_claim_no") == null ||
                                                                  (o.Field<DateTime?>("medicare_effective_date") != null &&
                                                                   Convert.ToDateTime(o.Field<DateTime?>("medicare_effective_date")) > ldteBatchEffectiveDate)))
                                                                    .OrderByDescending(o => o.Field<DateTime>("dob")).FirstOrDefault();
                                                        if (ldrDepenedent != null)
                                                        {
                                                            ldrDepenedent["cnt"] = lintNonMedicareCount;
                                                            ldtFinalSplitInfo.ImportRow(ldrDepenedent);
                                                        }
                                                    }
                                                }
                                                else if (lblnNonMedicare)
                                                {
                                                    IEnumerable<DataRow> lenmdrSpouse = ldarrSplit.Where(o => 
                                                                                    o.Field<string>("medicare_claim_no") != null &&
                                                                                    o.Field<DateTime?>("medicare_effective_date") != null &&
                                                                                    Convert.ToDateTime(o.Field<DateTime?>("medicare_effective_date")) <= ldteBatchEffectiveDate &&
                                                                                    o.Field<string>("relationship_value") == busConstant.DependentRelationshipSpouse);
                                                    if (lenmdrSpouse.Count() > 0)
                                                    {
                                                        foreach (DataRow ldr in lenmdrSpouse)
                                                        {
                                                            ldr["cnt"] = lintMedicareCount;
                                                            ldtFinalSplitInfo.ImportRow(ldr);
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        DataRow ldrDepenedent = ldarrSplit.Where(o => 
                                                                                    o.Field<string>("medicare_claim_no") != null &&
                                                                                    o.Field<DateTime?>("medicare_effective_date") != null &&
                                                                                    Convert.ToDateTime(o.Field<DateTime?>("medicare_effective_date")) <= ldteBatchEffectiveDate)
                                                                                    .OrderByDescending(o => o.Field<DateTime>("dob")).FirstOrDefault();
                                                        if (ldrDepenedent != null)
                                                        {
                                                            ldrDepenedent["cnt"] = lintMedicareCount;
                                                            ldtFinalSplitInfo.ImportRow(ldrDepenedent);
                                                        }
                                                    }
                                                }
                                                ldtFinalSplitInfo.AcceptChanges();
                                            }
                                            //block to get the coverage code and premium amounts
                                            foreach (busProviderReportDataInsurance lobjInsurance in lenmProviderReportDataIns)
                                            {
                                                lobjGHDV = new busPersonAccountGhdv();
                                                DataRow[] ldarrSplit = ldtFinalSplitInfo.FilterTable(busConstant.DataType.Numeric, "provider_report_data_insurance_comp_id",
                                                                            lobjInsurance.icdoProviderReportDataInsurance.provider_report_data_insurance_comp_id);
                                                
                                                if (lobjInsurance.icdoProviderReportDataInsurance.plan_id != busConstant.PlanIdGroupHealth)
                                                {
                                                    foreach (DataRow ldrSplitInfo in ldarrSplit)
                                                    {
                                                        ldrSplitInfo["premium_amount"] = lobjInsurance.icdoProviderReportDataInsurance.premium_amount;
                                                        ldrSplitInfo["fee_amount"] = lobjInsurance.icdoProviderReportDataInsurance.fee_amount;
                                                        ldrSplitInfo["coverage_code"] = lobjInsurance.icdoProviderReportDataInsurance.coverage_code;
                                                        InsertIntoSplitInfo(ldrSplitInfo);
                                                    }
                                                }
                                                else
                                                {
                                                    lstrMedicareCoverageCode = lstrNonMedicareCoverageCode = string.Empty;
                                                    lintMedicareCount = lintNonMedicareCount = 0;
                                                    if (ldtMedicareCodeValue.AsEnumerable()
                                                        .Where(o => o.Field<string>("data1") == lobjInsurance.icdoProviderReportDataInsurance.coverage_code).Any())
                                                    {
                                                        lintMedicareCount = ldarrSplit.Where(o => o.Field<string>("medicare_claim_no") != null &&
                                                                                                  o.Field<DateTime?>("medicare_effective_date") != null &&
                                                                                                  Convert.ToDateTime(o.Field<DateTime?>("medicare_effective_date")) <= ldteBatchEffectiveDate).Sum(o => o.Field<int>("cnt"));
                                                        lintNonMedicareCount = ldarrSplit.Where(o => o.Field<string>("medicare_claim_no") == null ||
                                                                                                    (o.Field<DateTime?>("medicare_effective_date") != null &&
                                                                                                   Convert.ToDateTime( o.Field<DateTime?>("medicare_effective_date")) > ldteBatchEffectiveDate)).Sum(o => o.Field<int>("cnt"));
                                                        DataTable ldtFilteredMedicareCodeValue = ldtMedicareCodeValue.AsEnumerable()
                                                                                            .Where(o => o.Field<string>("data1") == lobjInsurance.icdoProviderReportDataInsurance.coverage_code &&
                                                                                                    o.Field<string>("data3") == lintMedicareCount.ToString()).AsDataTable();
                                                        string lstrData3 = lintNonMedicareCount > 1 ? busConstant.NonMedicareSplitCountTwoorMore : busConstant.NonMedicareSplitCountOne;
                                                        DataTable ldtFilteredNonMedicareCodeValue = ldtNonMedicareCodeValue.AsEnumerable()
                                                                                            .Where(o => o.Field<string>("data3") == lstrData3).AsDataTable();
                                                        lstrMedicareCoverageCode = lobjInsurance.icdoProviderReportDataInsurance.coverage_code;
                                                        lstrNonMedicareCoverageCode = lobjInsurance.icdoProviderReportDataInsurance.coverage_code;
                                                        if (ldtFilteredMedicareCodeValue.Rows.Count > 0)
                                                            lstrMedicareCoverageCode = ldtFilteredMedicareCodeValue.Rows[0]["data2"].ToString();
                                                        if (ldtFilteredNonMedicareCodeValue.Rows.Count > 0)
                                                            lstrNonMedicareCoverageCode = ldtFilteredNonMedicareCodeValue.Rows[0]["data1"].ToString();

                                                        busPerson lobjPerson = new busPerson();
                                                        lobjPerson.FindPerson(lobjInsurance.icdoProviderReportDataInsurance.person_id);
                                                        lobjPerson.LoadPersonAccountByPlan(lobjInsurance.icdoProviderReportDataInsurance.plan_id);
                                                        //busPersonAccount lobjPersonAccount = lobjPerson.icolPersonAccountByPlan
                                                        //    .Where(o => busGlobalFunctions.CheckDateOverlapping(lobjInsurance.icdoProviderReportDataInsurance.effective_date,
                                                        //                                                        o.icdoPersonAccount.start_date,
                                                        //                                                        o.icdoPersonAccount.end_date_no_null)).FirstOrDefault();
                                                        // The ended accounts were not included and hence the Premium was not calculated.
                                                        busPersonAccount lobjPersonAccount = lobjPerson.icolPersonAccountByPlan.FirstOrDefault();
                                                        if (lobjPersonAccount != null)
                                                        {
                                                            lobjGHDV.FindGHDVByPersonAccountID(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                                            lobjGHDV.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                                                            // Start --In case of Dependent COBRA the Member GHDV object should be loaded.
                                                            if (lobjGHDV.icdoPersonAccount.from_person_account_id > 0)
                                                            {
                                                                lobjGHDV.ibusPerson = lobjPerson;
                                                                var lcdoMemberPersonAccount = new cdoPersonAccount();
                                                                //PIR 22945 - If 'Dependent of' is selected then load that members person account.
                                                                lobjGHDV.iblnIsDependentCobra = lobjGHDV.ibusPerson.IsDependentCobra(lobjGHDV.icdoPersonAccount.plan_id, ldteBatchEffectiveDate, ref lcdoMemberPersonAccount, lobjGHDV.icdoPersonAccount.from_person_account_id);
                                                                //Load Member GHDV Object
                                                                lobjGHDV.ibusMemberGHDVForDependent = new busPersonAccountGhdv();
                                                                lobjGHDV.ibusMemberGHDVForDependent.FindGHDVByPersonAccountID(lcdoMemberPersonAccount.person_account_id);
                                                                lobjGHDV.ibusMemberGHDVForDependent.icdoPersonAccount = lcdoMemberPersonAccount;

                                                                lobjGHDV.LoadEmploymentDetailByDate(ldteBatchEffectiveDate, lobjGHDV.ibusMemberGHDVForDependent, true, true);
                                                                lobjGHDV.icdoPersonAccountGhdv.plan_option_value = lobjGHDV.ibusMemberGHDVForDependent.icdoPersonAccountGhdv.plan_option_value;
                                                            }
                                                            // End
                                                            busPersonAccountGhdvHistory lobjGHDVHistory = new busPersonAccountGhdvHistory();
                                                            lobjGHDVHistory = lobjGHDV.LoadHistoryByDate(lobjInsurance.icdoProviderReportDataInsurance.effective_date);
                                                            lobjGHDV = lobjGHDVHistory.LoadGHDVObject(lobjGHDV);
                                                            if (!string.IsNullOrEmpty(lobjGHDV.icdoPersonAccountGhdv.overridden_structure_code))
                                                                lobjGHDV.LoadRateStructureForUserStructureCode();
                                                            else
                                                                lobjGHDV.LoadRateStructure(lobjInsurance.icdoProviderReportDataInsurance.effective_date);
                                                            lobjGHDVHistory = null;
                                                        }
                                                        lobjPerson = null;
                                                        lobjPersonAccount = null;
                                                    }
                                                    int lintMedCnt = 0, lintNonMedCnt = 0;
                                                    decimal ldecLowIncomeCredit = 0.00M;
                                                    foreach (DataRow ldrSplitInfo in ldarrSplit)
                                                    {
                                                        if (ldtMedicareCodeValue.AsEnumerable()
                                                            .Where(o => o.Field<string>("data1") == lobjInsurance.icdoProviderReportDataInsurance.coverage_code).Any())
                                                        {
                                                            if (lobjGHDV != null && lobjGHDV.icdoPersonAccountGhdv != null && lobjGHDV.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                                                            {
                                                                if (ldrSplitInfo["medicare_claim_no"] != DBNull.Value &&
                                                                    ldrSplitInfo["medicare_effective_date"] != DBNull.Value &&
                                                                    Convert.ToDateTime(ldrSplitInfo["medicare_effective_date"]) <= ldteBatchEffectiveDate)
                                                                {
                                                                    lobjGHDV.icdoPersonAccountGhdv.coverage_code = lstrMedicareCoverageCode;
                                                                    lintMedCnt++;
                                                                }
                                                                else
                                                                {
                                                                    lobjGHDV.icdoPersonAccountGhdv.coverage_code = lstrNonMedicareCoverageCode;
                                                                    lintNonMedCnt++;
                                                                }
                                                                lobjGHDV.idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheDataWithoutFlagCheck(iobjPassInfo);
                                                                lobjGHDV.LoadCoverageRefID();
                                                                //low income credit to be applied only to medicare part
                                                                if (lobjGHDV.icdoPersonAccountGhdv.coverage_code == lstrNonMedicareCoverageCode)
                                                                {
                                                                    ldecLowIncomeCredit = lobjGHDV.icdoPersonAccountGhdv.low_income_credit;
                                                                    lobjGHDV.icdoPersonAccountGhdv.low_income_credit = 0;
                                                                }
                                                                lobjGHDV.GetMonthlyPremiumAmountByRefID(lobjInsurance.icdoProviderReportDataInsurance.effective_date);
                                                                //assigning the low income credit back after premium calculation
                                                                lobjGHDV.icdoPersonAccountGhdv.low_income_credit = ldecLowIncomeCredit;
                                                                ldecLowIncomeCredit = 0.00M;

                                                                //block to check whether split has happened and if not, then assign full premium to existing record                                                                
                                                                if (ldrSplitInfo["medicare_claim_no"] != DBNull.Value && 
                                                                    ldrSplitInfo["medicare_effective_date"] != DBNull.Value &&
                                                                    Convert.ToDateTime(ldrSplitInfo["medicare_effective_date"]) <= ldteBatchEffectiveDate &&
                                                                    lintNonMedicareCount == 0)
                                                                {
                                                                    ldrSplitInfo["premium_amount"] = lobjInsurance.icdoProviderReportDataInsurance.premium_amount;
                                                                    ldrSplitInfo["fee_amount"] = lobjInsurance.icdoProviderReportDataInsurance.fee_amount;
                                                                }
                                                                else if (ldrSplitInfo["medicare_claim_no"] != DBNull.Value &&
                                                                    ldrSplitInfo["medicare_effective_date"] != DBNull.Value &&
                                                                    Convert.ToDateTime(ldrSplitInfo["medicare_effective_date"]) <= ldteBatchEffectiveDate &&
                                                                    lstrMedicareCoverageCode == lobjInsurance.icdoProviderReportDataInsurance.coverage_code)
                                                                {
                                                                    continue;
                                                                }
                                                                else if ((ldrSplitInfo["medicare_claim_no"] == DBNull.Value ||
                                                                    (ldrSplitInfo["medicare_effective_date"] != DBNull.Value &&
                                                                    Convert.ToDateTime(ldrSplitInfo["medicare_effective_date"]) > ldteBatchEffectiveDate)) &&
                                                                    lstrMedicareCoverageCode == lobjInsurance.icdoProviderReportDataInsurance.coverage_code)
                                                                {
                                                                    ldrSplitInfo["premium_amount"] = lobjInsurance.icdoProviderReportDataInsurance.premium_amount;
                                                                    ldrSplitInfo["fee_amount"] = lobjInsurance.icdoProviderReportDataInsurance.fee_amount;
                                                                }
                                                                else
                                                                {
                                                                    ldrSplitInfo["premium_amount"] = lobjGHDV.icdoPersonAccountGhdv.PremiumExcludingFeeAmount *
                                                                            (lobjInsurance.icdoProviderReportDataInsurance.premium_amount < 0 ? -1 : 1);
                                                                    ldrSplitInfo["fee_amount"] = lobjGHDV.icdoPersonAccountGhdv.FeeAmount *
                                                                        (lobjInsurance.icdoProviderReportDataInsurance.premium_amount < 0 ? -1 : 1);
                                                                }
                                                                ldrSplitInfo["coverage_code"] = lobjGHDV.icdoPersonAccountGhdv.coverage_code;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ldrSplitInfo["premium_amount"] = lobjInsurance.icdoProviderReportDataInsurance.premium_amount;
                                                            ldrSplitInfo["fee_amount"] = lobjInsurance.icdoProviderReportDataInsurance.fee_amount;
                                                            ldrSplitInfo["coverage_code"] = lobjInsurance.icdoProviderReportDataInsurance.coverage_code;
                                                        }
                                                        InsertIntoSplitInfo(ldrSplitInfo);
                                                    }
                                                    lobjGHDV = null;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            DBFunction.DBNonQuery("cdoProviderReportDataInsuranceSplit.InsertSplitForInsrPlansByProvider",
                                                                new object[3]{lobjBatchRequest.icdoProviderReportDataBatchRequest.org_id,
                                                            lobjBatchRequest.icdoProviderReportDataBatchRequest.effective_start_date, iobjBatchSchedule.batch_schedule_id},
                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                        }
                                    }
                                    else
                                    {
                                        DBFunction.DBNonQuery("cdoProviderReportDataInsuranceSplit.InsertSplitForInsrPlansByProvider",
                                                            new object[3]{lobjBatchRequest.icdoProviderReportDataBatchRequest.org_id,
                                                            lobjBatchRequest.icdoProviderReportDataBatchRequest.effective_start_date,iobjBatchSchedule.batch_schedule_id},
                                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                    }

                                    //--end of pir 933 changes--//
                                    #endregion
                                    /// Create Outbound file for the Request
                                    busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                                    lobjProcessFiles.iarrParameters = new object[3];
                                    lobjProcessFiles.iarrParameters[0] = lobjBatchRequest.icdoProviderReportDataBatchRequest.org_id;
                                    lobjProcessFiles.iarrParameters[1] = lobjBatchRequest.icdoProviderReportDataBatchRequest.effective_start_date;
                                    lobjProcessFiles.iarrParameters[2] = iobjBatchSchedule.email_notification;

                                    string lstrProviderOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(lobjBatchRequest.icdoProviderReportDataBatchRequest.org_id);
                                    string lstrSVOrgCode = busGlobalFunctions.GetData1ByCodeValue(1213, busConstant.SuperiorVisionProviderCodeValue, iobjPassInfo);
                                    string lstrDeltaOrgCode = busGlobalFunctions.GetData1ByCodeValue(1213, busConstant.DELTAProviderCodeValue, iobjPassInfo); // PIR 10448
                                    string lstrHealthSanfordOrgCode = busGlobalFunctions.GetData1ByCodeValue(1213, busConstant.SanfordProviderCodeValue, iobjPassInfo);

                                    if (lstrProviderOrgCode == lstrSVOrgCode)
                                    {
                                        idlgUpdateProcessLog(" Creating HIPAA 820 Superior Vision Insurance Payment File ", "INFO", istrProcessName);
                                        lobjProcessFiles.CreateOutboundFile(77);
                                    }
                                    else if (lstrProviderOrgCode == lstrDeltaOrgCode) // PIR 10448
                                    {
                                        idlgUpdateProcessLog(" Creating HIPAA 820 Delta Dental Insurance Payment File ", "INFO", istrProcessName);
                                        lobjProcessFiles.CreateOutboundFile(96);
                                    }
                                    else if (lstrProviderOrgCode == lstrHealthSanfordOrgCode)   //PIR 14241
                                    { 
                                        idlgUpdateProcessLog("Creating HIPAA 820 Sanford Insurance Payment File ", "INFO", istrProcessName);
                                        lobjProcessFiles.CreateOutboundFile(98);
                                    }
                                    else
                                    {
                                        idlgUpdateProcessLog(" Creating HIPAA 820 Insurance Payment File ", "INFO", istrProcessName);
                                        lobjProcessFiles.CreateOutboundFile(45);
                                    }
                                }  
                            }

                            /// Update the Batch Request status to Processed 
                            /// Updates the Batch Request ID   
                            /// PIR 7705 - Do not update for HSA Provider here - Logically HSA provider shouldn't come but just in case it is picked up. 
                            if (!CheckIfHSAProvider(Convert.ToInt32(lobjBatchRequest.icdoProviderReportDataBatchRequest.org_id)) && lobjBatchRequest.icdoProviderReportDataBatchRequest.plan_id != busConstant.PlanIdMedicarePartD)
                            {
                                UpdateBatchRequestStatusToProcessed(lobjBatchRequest);
                                UpdateInsuranceBatchRequestID(lobjBatchRequest);
                            }
                        }
                        #endregion

                        //uat pir 1911 : create report only for life requests
                        #region Life Premium Report Creation
                        if (lobjBatchRequest.icdoProviderReportDataBatchRequest.plan_id == busConstant.PlanIdGroupLife)
                        {
                            DataTable ldtReportResult = busBase.Select("cdoProviderReportDataInsurance.rptGroupLifePremium",
                                new object[1] { lobjBatchRequest.icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id });
                            if (ldtReportResult.Rows.Count > 0)
                            {
                                CreateReport("rptGroupLifePremium.rpt", ldtReportResult);
                                
                                //PIR 24197
                                busNeoSpinBase lobjNeospin = new busNeoSpinBase();
                                lobjNeospin.CreateExcelReport("rptGroupLifePremium.rpt", ldtReportResult, "", busConstant.ReportPath);
                            }
                        }
                        #endregion
                    }
                }
                #endregion

				//PIR 14848 - Medicare Part D changes
                #region MEDICARE PART D INSURANCE PROVIDER DATA 14848

                //if records exist in Provider Report DC Data,then create payment details for each org and plan
                if (lclbProviderReportDataMedicarePartD.Count > 0)
                {
                    #region Payment History Details Creation

                    //Load vendor payment amount details group by Org
                    var lvarProviderReportDataMedicarePartDByOrg = from lobjInsrProviderDataMedicare in lclbProviderReportDataMedicarePartD
                                                              group lobjInsrProviderDataMedicare
                                                               by new
                                                               {
                                                                   lobjInsrProviderDataMedicare.icdoProviderReportDataMedicare.provider_org_id,
                                                                   lobjInsrProviderDataMedicare.icdoProviderReportDataMedicare.plan_id
                                                               }
                                                                  into ProviderDataMedicareByOrg
                                                                  select new
                                                                  {
                                                                      lintPlanID = ProviderDataMedicareByOrg.Key.plan_id,
                                                                      lintOrgID = ProviderDataMedicareByOrg.Key.provider_org_id,
                                                                      ldecPremiumAmount = ProviderDataMedicareByOrg.Sum(lobjProviderMedicareDataByOrg => lobjProviderMedicareDataByOrg.icdoProviderReportDataMedicare.premium_amount),
                                                                      ldecLISAmount = ProviderDataMedicareByOrg.Sum(lobjProviderMedicareDataByOrg => lobjProviderMedicareDataByOrg.icdoProviderReportDataMedicare.lis_amount),
                                                                      ldecLEPAmount = ProviderDataMedicareByOrg.Sum(lobjProviderMedicareDataByOrg => lobjProviderMedicareDataByOrg.icdoProviderReportDataMedicare.lep_amount)
                                                                  };
                    //Get Latest check book
					//Backlog PIR 938
                    busPaymentCheckBook lobjAvailableCheckbook = busPayeeAccountHelper.GetPaymentCheckBookForGivenDate(iobjSystemManagement.icdoSystemManagement.batch_date, 0, busConstant.PlanBenefitTypeInsurance);
                    //Check available number of checks 
                    if (lobjAvailableCheckbook != null)
                        lblnCheckAvailable = lvarProviderReportDataMedicarePartDByOrg.Count() <=
                        Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.max_check_number) -
                        Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number) ? true : false;

                    //if it is less than number records in Number of Payment headers to be created,then skip the payment process for all Insr providers
                    if (lblnCheckAvailable)
                    {
                        try
                        {
                            idlgUpdateProcessLog("Creating Payment History Details for Medicare Part D Insurance Providers", "INFO", istrProcessName);
                            int lintLastCheckNumber = Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number);
                            int lintLastCheckNumberAfterVendor = lintLastCheckNumber;
                            Array.ForEach(lvarProviderReportDataMedicarePartDByOrg.ToArray(), o =>
                            {
                                int lintPHHId = 0;
                                decimal ldecTotalAmount = o.ldecPremiumAmount + o.ldecLEPAmount - o.ldecLISAmount;
                                decimal ldecTotalPremiumPlusLEPAmount = o.ldecPremiumAmount + o.ldecLEPAmount;
                                if (ldecTotalPremiumPlusLEPAmount > 0.0m)
                                {
                                    iarrProviders.Add(o.lintOrgID);
                                    iarrInsurancePlansRequested.Add(o.lintPlanID);
                                    string lstrItemCode = string.Empty;
                                    busPaymentHistoryHeader lbusPaymentHistoryHeader = new busPaymentHistoryHeader();
                                    //Create payment history header for each provider and plan
                                    lbusPaymentHistoryHeader.CreateVendorPaymentHistoryHeader(o.lintOrgID, o.lintPlanID, iobjSystemManagement.icdoSystemManagement.batch_date);
                                    lintPHHId = lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id;
                                    if (o.lintPlanID == busConstant.PlanIdMedicarePartD)
                                    {
                                        lstrItemCode = busConstant.VendorPaymentItemMedicarePartD;
                                    }
                                    if (!string.IsNullOrEmpty(lstrItemCode))
                                    {
                                        //Create payment history details for each provider
                                        lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(ldecTotalPremiumPlusLEPAmount, lstrItemCode, 0);
                                        lintLastCheckNumberAfterVendor = lbusPaymentHistoryHeader.CreateVendorPaymentDistributionDetails(ldecTotalPremiumPlusLEPAmount, lintLastCheckNumberAfterVendor);
                                    }
                                }
                                if (lintPHHId > 0)
                                {
                                    if (ldecTotalAmount > 0)
                                    {
                                        GenerateGLByType(o.lintPlanID, busConstant.ItemTypeMedicarePartDAmountIBS, ldecTotalAmount, o.lintOrgID, lintPHHId);
                                    }
                                    if (ldecTotalPremiumPlusLEPAmount > 0)
                                    {
                                        GenerateGLByTypeTransfer(o.lintPlanID, busConstant.ItemTypeMedicarePartDAmountIBS, ldecTotalPremiumPlusLEPAmount, o.lintOrgID, lintPHHId);
                                    }
                                }
                            });
                            //Update Check with last check number
                            if (lintLastCheckNumber != lintLastCheckNumberAfterVendor)
                            {
                                lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number = lintLastCheckNumberAfterVendor.ToString();
                                lobjAvailableCheckbook.icdoPaymentCheckBook.Update();
                            }
                            idlgUpdateProcessLog("Payment History Details for Medicare Part D Insurance providers are created", "INFO", istrProcessName);
                        }
                        catch (Exception e)
                        {
                            ExceptionManager.Publish(e);
                            idlgUpdateProcessLog("Creating Payment History Details for Medicare Part D Insurance providers is failed", "INFO", istrProcessName);
                            throw e;
                        }
                    }
                    else
                    {
                        idlgUpdateProcessLog("The Check Book has reached the Maximum Limit.", "INFO", istrProcessName);
                        throw new Exception();
                    }
                    #endregion
                    #region ACH Outbound File Creation

                    try
                    {
                        idlgUpdateProcessLog("Creating ACH Outbound File Medicare Insurance Provider ", "INFO", istrProcessName);
                        /// Generate ACH File out for Provider.
                        busProcessOutboundFile lobjProcessACHFile = new busProcessOutboundFile();
                        LoadACHProviderReportData(busConstant.Provider_Insurance_Medicare);
                        /// Generates ACH only if Record Exists
                        if (_iclbACHProviderReportData.Count > 0)
                        {
                            lobjProcessACHFile.iarrParameters = new object[3];
                            lobjProcessACHFile.iarrParameters[0] = _iclbACHProviderReportData;
                            lobjProcessACHFile.iarrParameters[1] = busConstant.Provider_Insurance;
                            lobjProcessACHFile.iarrParameters[2] = busConstant.ACHFileNameInsuranceVendorPayment;
                            lobjProcessACHFile.CreateOutboundFile(37);
                        }
                        idlgUpdateProcessLog(" ACH Outbound File For Medicare Insurance Provider is created ", "INFO", istrProcessName);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e);
                        idlgUpdateProcessLog(" Creating ACH Outbound File For Medicare Insurance Provider is failed ", "INFO", istrProcessName);
                        throw e;
                    }
                    #endregion
                    foreach (DataRow dr in ldtNonProcessedBatchRequests.Rows)
                    {
                        /// Load the Batch Request Data
                        busProviderReportDataBatchRequest lobjBatchRequest = new busProviderReportDataBatchRequest();
                        lobjBatchRequest.icdoProviderReportDataBatchRequest = new cdoProviderReportDataBatchRequest();
                        lobjBatchRequest.icdoProviderReportDataBatchRequest.LoadData(dr);

                        if (lobjBatchRequest.icdoProviderReportDataBatchRequest.plan_id != 0)
                        {
                            if (lobjBatchRequest.icdoProviderReportDataBatchRequest.plan_id == busConstant.PlanIdMedicarePartD)
                            {
                                DataTable ldtbInsrRecordsMedicare = busBase.Select("cdoProviderReportDataMedicarePartD.LoadInsuranceProviderDataMedicare",
                                                                new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
                                busNeoSpinBase lobjNeospin = new busNeoSpinBase();

                                if (ldtbInsrRecordsMedicare.Rows.Count > 0)
                                {
                                    lobjNeospin.CreateExcelReport("rptMedicarePartDVendorSummary.rpt", ldtbInsrRecordsMedicare, "", busConstant.ReportPath);
                                }

                                //PIR 24382 - Humana provider File
                                idlgUpdateProcessLog("Create Humana provider File", "INFO", istrProcessName);
                                DataTable ldtbHumanaPaymentMedicare = busBase.Select("entOrganization.LoadHumanaProviderDataMedicarePartD", 
                                                                new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
                                if (ldtbHumanaPaymentMedicare.Rows.Count > 0)
                                {
                                    busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                                    lobjProcessFiles.iarrParameters = new object[1];
                                    lobjProcessFiles.iarrParameters[0] = ldtbHumanaPaymentMedicare;
                                    lobjProcessFiles.CreateOutboundFile(105);
                                    idlgUpdateProcessLog("Humana provider File created successfully", "INFO", istrProcessName);
                                }
                                else
                                    idlgUpdateProcessLog("No records exist", "INFO", istrProcessName);


                                UpdateBatchRequestStatusToProcessed(lobjBatchRequest);
                                UpdateInsuranceBatchRequestIDMedicare(lobjBatchRequest);
                            }
                        }
                    }

                }
                #endregion
            }
        }
		//PIR 14848 - Medicare Part D changes
        private void GenerateGLByTypeTransfer(int aintPlanID, string astrItemType, decimal adecTotalAmount, int aintOrgID, int aintSourceID)
        {
            if (idtBatchDate == DateTime.MinValue)
                idtBatchDate = busGlobalFunctions.GetSysManagementBatchDate();

                cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
                lcdoAcccountReference.plan_id = aintPlanID;
                lcdoAcccountReference.source_type_value = busConstant.GLSourceTypeValueVendorPayment;
                lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeTransfer;
                lcdoAcccountReference.item_type_value = astrItemType;
                lcdoAcccountReference.status_transition_value = string.Empty;

                busGLHelper.GenerateGL(lcdoAcccountReference, 0, aintOrgID, aintSourceID, adecTotalAmount, idtBatchDate, idtBatchDate, iobjPassInfo);
        }

        #region PIR 7705
       
        /// <summary>
        /// Generates HSA Contribution File
        /// </summary>
        /// <param name="adtLists"></param>
        private void GenerateHSAContributionFile(DataTable adtLists)
        {
            //Filter HSA Batch Requests alone
            var lenumHSABatchRequests = adtLists.AsEnumerable().Where(i => i.Field<int>("org_id").ToString() != string.Empty
                && i.Field<int>("plan_id").ToString() != string.Empty
                && CheckIfHSAProvider(i.Field<int>("org_id"))
                && i.Field<int>("plan_id") == busConstant.PlanIdGroupHealth
                );
            if (lenumHSABatchRequests.IsNotNull())
            {
                foreach (DataRow ldrHSABatchRequest in lenumHSABatchRequests)
                {
                    busProviderReportDataBatchRequest lobjBatchRequest = new busProviderReportDataBatchRequest();
                    lobjBatchRequest.icdoProviderReportDataBatchRequest = new cdoProviderReportDataBatchRequest();
                    Collection<busProviderReportDataInsurance> lclbHSARecordsForFile = new Collection<busProviderReportDataInsurance>();
                    Collection<busProviderReportDataInsurance> lclbHSARecordsForBatchUpdate = new Collection<busProviderReportDataInsurance>();
                    lobjBatchRequest.icdoProviderReportDataBatchRequest.LoadData(ldrHSABatchRequest);
                    
                    lclbHSARecordsForFile = CreateHSARecordsForFile();
                    if (lclbHSARecordsForFile.Count > 0)
                    { 
                        CreateHSAContributionFileOut(lclbHSARecordsForFile);

                        lclbHSARecordsForBatchUpdate = GetHSARecordsForBatchUpdate();
                        if (lclbHSARecordsForBatchUpdate.Count > 0)
                        {
                            CreatePaymentHistoryHeaderForHSA(lclbHSARecordsForBatchUpdate);
                            SetBatchRequestID(lclbHSARecordsForBatchUpdate, lobjBatchRequest.icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id);
                            UpdateBatchRequestStatusToProcessed(lobjBatchRequest);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get HSA Records For Batch Update
        /// </summary>
        /// <returns></returns>
        private Collection<busProviderReportDataInsurance> GetHSARecordsForBatchUpdate()
        {
            return GetHSAPersonsForFile();
        }

        /// <summary>
        /// Creation of Contribution file to HSA Provider
        /// </summary>
        /// <param name="aobjBatchRequest"></param>
        private void CreateHSAContributionFileOut(Collection<busProviderReportDataInsurance> aclbHSARecordsForFile)
        {
            busProcessOutboundFile lobjProcessHSAContributionFile = new busProcessOutboundFile();
            try
            {
                if (aclbHSARecordsForFile.Count > 0)
                {
                    idlgUpdateProcessLog("Generating HSA Contribution File", "INFO", istrProcessName);
                    lobjProcessHSAContributionFile.iarrParameters = new object[2];
                    lobjProcessHSAContributionFile.iarrParameters[0] = aclbHSARecordsForFile;
                    lobjProcessHSAContributionFile.iarrParameters[1] = idtBatchDate;
                    lobjProcessHSAContributionFile.CreateOutboundFile(91);
                    idlgUpdateProcessLog(" HSA Contribution File has been created ", "INFO", istrProcessName);
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("HSA Contribution File creation has failed ", "INFO", istrProcessName);
                throw e;
            }           
        }

        /// <summary>
        /// Gets HSA Records for HSA Contribution File
        /// Logic: Person enrolled in HSA should have his total vendor amount sum across effective dates > 0 
        /// Sum of Vendor amount per month for a person to go to file as contribution Amount
        /// </summary>
        /// <returns>HSA Records for HSA Contribution File</returns>
        private Collection<busProviderReportDataInsurance> CreateHSARecordsForFile()
        {
            Collection<busProviderReportDataInsurance> lclbHSARecordsForFile = new Collection<busProviderReportDataInsurance>();
            Collection<busProviderReportDataInsurance> lclbHSAPersonsInFile = new Collection<busProviderReportDataInsurance>();

            //This collection will contain the list of all Persons who have to be in the HSA contribution File
            lclbHSAPersonsInFile = GetHSAPersonsForFile();

            if (lclbHSAPersonsInFile.Count  > 0)
            {
                  
                //Group by ssn and effective date  
                var lenumHSAEligiblePersonsByEffectiveDate =    from HSARecordForFile in lclbHSAPersonsInFile
                                                                group HSARecordForFile
                                                                by new
                                                                {
                                                                    HSARecordForFile.icdoProviderReportDataInsurance.ssn,
                                                                    HSARecordForFile.icdoProviderReportDataInsurance.effective_date,
                                                                    HSARecordForFile.icdoProviderReportDataInsurance.person_id
                                                                }
                                                                into HSARecords
                                                                select new
                                                                {
                                                                    lstrPersonSSN = HSARecords.Key.ssn,
                                                                    ldecTotalVendorAmountForPersonByEffectiveDate = HSARecords.Sum(i => i.icdoProviderReportDataInsurance.premium_amount),
                                                                    ldtEffectiveDate = HSARecords.Key.effective_date,
                                                                    lintPersonID = HSARecords.Key.person_id
                                                                };
                    
                if (lenumHSAEligiblePersonsByEffectiveDate.IsNotNull())
                {
                    //Create new PRDI for Contribution File that contains the newly summed amount as premium amount. 
                    foreach (var lobjPRDI in lenumHSAEligiblePersonsByEffectiveDate)
                    {
                        busProviderReportDataInsurance lobjHSAPersonForFile = new busProviderReportDataInsurance { icdoProviderReportDataInsurance = new cdoProviderReportDataInsurance() };
                        lobjHSAPersonForFile.icdoProviderReportDataInsurance.ssn = lobjPRDI.lstrPersonSSN;
                        lobjHSAPersonForFile.icdoProviderReportDataInsurance.effective_date = lobjPRDI.ldtEffectiveDate;
                        lobjHSAPersonForFile.icdoProviderReportDataInsurance.premium_amount = lobjPRDI.ldecTotalVendorAmountForPersonByEffectiveDate;
                        lobjHSAPersonForFile.icdoProviderReportDataInsurance.person_id = lobjPRDI.lintPersonID;
                        lclbHSARecordsForFile.Add(lobjHSAPersonForFile);
                    } 
                }                  
            }
            return lclbHSARecordsForFile;
        }

      
        /// <summary>
        /// Gets all the persons who need to be in the HSA contribution File 
        /// </summary>
        /// <returns></returns>
        private Collection<busProviderReportDataInsurance> GetHSAPersonsForFile()
        {
            Collection<busProviderReportDataInsurance> lclbAllHSARecords = new Collection<busProviderReportDataInsurance>();
            Collection<busProviderReportDataInsurance> lclbHSAPersonsForFile = new Collection<busProviderReportDataInsurance>();

            lclbAllHSARecords = GetAllNonProcessedHSARecords();
            //Group by ssn and get total vendor amount per person across effective dates
            var lenumHSAPersonsForFile =    from lobjHSARecords in lclbAllHSARecords
                                            group lobjHSARecords
                                            by new
                                            {
                                                lobjHSARecords.icdoProviderReportDataInsurance.ssn
                                            }
                                            into HSAEligiblePerson
                                            select new
                                            {
                                                lintPersonSSN = HSAEligiblePerson.Key.ssn,
                                                //Premium amount contains the vendor amount with sign value
                                                ldecTotalVendorAmountForPerson = HSAEligiblePerson.Sum(i => i.icdoProviderReportDataInsurance.premium_amount)
                                            };
            //Filter persons whose total vendor amount > 0
            lenumHSAPersonsForFile = lenumHSAPersonsForFile.Where(i => i.ldecTotalVendorAmountForPerson > 0);

            if (lenumHSAPersonsForFile.IsNotNull())
            {
                //Get the PRDI records for the filtered persons
                foreach (busProviderReportDataInsurance lobjPRDI in lclbAllHSARecords)
                {
                    if (lenumHSAPersonsForFile.Where(i => i.lintPersonSSN == lobjPRDI.icdoProviderReportDataInsurance.ssn).Any())
                    {
                        //As per Mail from Maik dated 03/08/2012. 
                        //The records that were catch-up(retro) contributions 
                        //{(i.e) Example HDHP effective since 01/01/2012 but HSA effective 04/01/2012, effective dates will be marked as 03/01/2012 }
                        //need to be sent with the effective_date = hsa_effective_date
                        if (lobjPRDI.icdoProviderReportDataInsurance.hsa_effective_date.IsNotNull() && lobjPRDI.icdoProviderReportDataInsurance.hsa_effective_date.Date > lobjPRDI.icdoProviderReportDataInsurance.effective_date.Date)
                            lobjPRDI.icdoProviderReportDataInsurance.effective_date = lobjPRDI.icdoProviderReportDataInsurance.hsa_effective_date;
                        lclbHSAPersonsForFile.Add(lobjPRDI);
                    }
                }
            }
            return lclbHSAPersonsForFile;
        }

        /// <summary>
        /// Gets all Records with HSA Flag = 'Y' and batch request ID null 
        /// </summary>
        /// <returns></returns>
        private Collection<busProviderReportDataInsurance> GetAllNonProcessedHSARecords()
        {
            DataTable ldtHSARecords = busBase.Select("cdoProviderReportDataInsurance.fleHSAContributionFileOut",
                                              new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            return new busBase().GetCollection<busProviderReportDataInsurance>(ldtHSARecords, "icdoProviderReportDataInsurance");
        }

        /// <summary>
        /// Checks whether HSA provider Batch Request is not processed
        /// </summary>
        /// <param name="adtBatchRequests"></param>
        /// <returns></returns>
        private bool IsHSAProviderBatchRequestExist(DataTable adtBatchRequests)
        {
            bool lblnResult = adtBatchRequests.AsEnumerable().Where(i => i.Field<int>("org_id").ToString() != string.Empty
                && i.Field<int>("plan_id").ToString() != string.Empty
                && CheckIfHSAProvider(i.Field<int>("org_id"))
                && i.Field<int>("plan_id") == busConstant.PlanIdGroupHealth
                ).Any();

            return lblnResult;
        }

        /// <summary>
        /// Updates Batch Request ID for insurance records
        /// </summary>
        /// <param name="aobjBatchRequest"></param>
        private void UpdateInsuranceBatchRequestID(busProviderReportDataBatchRequest aobjBatchRequest)
        {
            DataTable ldtbReportData = busBase.Select("cdoProviderReportDataInsurance.LoadByOrgID",
                                           new object[2] { aobjBatchRequest.icdoProviderReportDataBatchRequest.org_id, aobjBatchRequest.icdoProviderReportDataBatchRequest.effective_start_date });
            Collection<busProviderReportDataInsurance> lclbReportDataByOrgID = new busBase().GetCollection<busProviderReportDataInsurance>(ldtbReportData, "icdoProviderReportDataInsurance");
            SetBatchRequestID(lclbReportDataByOrgID, aobjBatchRequest.icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id);
        }

        private void UpdateInsuranceBatchRequestIDMedicare(busProviderReportDataBatchRequest aobjBatchRequest)
        {
            DataTable ldtbReportData = busBase.Select("cdoProviderReportDataMedicarePartD.LoadByOrgIDMedicare",
                                           new object[2] { aobjBatchRequest.icdoProviderReportDataBatchRequest.org_id, aobjBatchRequest.icdoProviderReportDataBatchRequest.effective_start_date });
            Collection<busProviderReportDataMedicarePartD> lclbReportDataByOrgIDMedicare = new busBase().GetCollection<busProviderReportDataMedicarePartD>(ldtbReportData, "icdoProviderReportDataMedicare");
            SetBatchRequestIDMedicare(lclbReportDataByOrgIDMedicare, aobjBatchRequest.icdoProviderReportDataBatchRequest.provider_report_data_batch_request_id);
        }



        /// <summary>
        /// Checks if Org ID is HSA Provider Org ID
        /// </summary>
        /// <param name="aintOrgID"></param>
        /// <returns></returns>
        private bool CheckIfHSAProvider(int aintOrgID)
        {
            if (busGlobalFunctions.GetData1ByCodeValue(52, busConstant.HSAProvider, iobjPassInfo)
                     == busGlobalFunctions.GetOrgCodeFromOrgId(aintOrgID))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates PaymentHistoryHeader for HSA Provider and Vendor Amount GL
        /// </summary>
        /// <param name="aclbHSARecords">HSA Records that were used for File</param>
        private void CreatePaymentHistoryHeaderForHSA(Collection<busProviderReportDataInsurance> aclbHSARecords)
        {
            var lvarDefProviderDataByOrg = from lobjInsrProviderData in aclbHSARecords
                                           group lobjInsrProviderData
                                           by new
                                           {
                                               lobjInsrProviderData.icdoProviderReportDataInsurance.provider_org_id,
                                               lobjInsrProviderData.icdoProviderReportDataInsurance.plan_id
                                           }
                                            into ProviderDataByOrg
                                            select new
                                            {
                                                lintPlanID = ProviderDataByOrg.Key.plan_id,
                                                lintOrgID = ProviderDataByOrg.Key.provider_org_id,
                                                //ldecHSAAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataInsurance.hsa_amount_for_gl),
                                                ldecHSAVendorAmount = ProviderDataByOrg.Sum(lobjProviderDataByOrg => lobjProviderDataByOrg.icdoProviderReportDataInsurance.vendor_amount_for_gl)
                                            };

            //Get Latest check book
			//Backlog PIR 938
            busPaymentCheckBook lobjAvailableCheckbook = busPayeeAccountHelper.GetPaymentCheckBookForGivenDate(iobjSystemManagement.icdoSystemManagement.batch_date, 0, busConstant.PlanBenefitTypeInsurance);
            //Check available number of checks 
            if (lobjAvailableCheckbook != null)
                lblnCheckAvailable = lvarDefProviderDataByOrg.Count() <=
                Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.max_check_number) -
                Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number) ? true : false;

            //if it is less than number records in Number of Payment headers to be created,then skip the payment process for all Insr providers
            if (lblnCheckAvailable)
            {
                try
                {
                    idlgUpdateProcessLog("Creating Payment History Details for HSA Provider", "INFO", istrProcessName);
                    int lintLastCheckNumber = Convert.ToInt32(lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number);
                    int lintLastCheckNumberAfterVendor = lintLastCheckNumber;
                    Array.ForEach(lvarDefProviderDataByOrg.ToArray(), o =>
                    {
                        int lintPHHId = 0;
                        if (o.ldecHSAVendorAmount > 0.0m)
                        {
                            string lstrItemCode = string.Empty;
                            busPaymentHistoryHeader lbusPaymentHistoryHeader = new busPaymentHistoryHeader();
                            //Create payment history header for each provider and plan
                            lbusPaymentHistoryHeader.CreateVendorPaymentHistoryHeader(o.lintOrgID, o.lintPlanID, iobjSystemManagement.icdoSystemManagement.batch_date);
                            lintPHHId = lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id;
                            if (o.lintPlanID == busConstant.PlanIdGroupHealth)
                            {
                                lstrItemCode = busConstant.VendorPaymentItemHealth;
                            }
                            if (!string.IsNullOrEmpty(lstrItemCode))
                            {
                                //PIR 7705 - Create GL for HSA Vendor Amount
                                if (o.ldecHSAVendorAmount > 0)
                                    lbusPaymentHistoryHeader.CreateVendorPaymentHistoryDetails(o.ldecHSAVendorAmount, busConstant.PAPITHSAAmount, 0);
                                //Create payment history check details for each provider
                                lintLastCheckNumberAfterVendor = lbusPaymentHistoryHeader.CreateVendorPaymentDistributionDetails(o.ldecHSAVendorAmount, lintLastCheckNumberAfterVendor);
                            }
                        }
                        //if (lintPHHId > 0)
                        //{
                        //    //PIR 7705 - Create GL for HSA Amount
                        //    //if (o.ldecHSAAmount > 0)
                        //    //{
                        //    //    GenerateGLByType(o.lintPlanID, busConstant.ItemTypeHSAPremiumPayment, o.ldecHSAAmount, o.lintOrgID, lintPHHId);
                        //    //}
                        //}
                    });
                    //Update Check with last check number
                    if (lintLastCheckNumber != lintLastCheckNumberAfterVendor)
                    {
                        lobjAvailableCheckbook.icdoPaymentCheckBook.last_check_number = lintLastCheckNumberAfterVendor.ToString();
                        lobjAvailableCheckbook.icdoPaymentCheckBook.Update();
                    }
                    idlgUpdateProcessLog("Payment History Details for HSA provider are created", "INFO", istrProcessName);
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Creating Payment History Details for HSA provider has failed", "INFO", istrProcessName);
                    throw e;
                }
            }
            else
            {
                idlgUpdateProcessLog("The Check Book has reached the Maximum Limit.", "INFO", istrProcessName);
                throw new Exception();
            }
        }

        /// <summary>
        /// Updates the Batch Request Status to Processed
        /// </summary>
        /// <param name="aobjBatchRequest"></param>
        private void UpdateBatchRequestStatusToProcessed(busProviderReportDataBatchRequest aobjBatchRequest)
        {
            aobjBatchRequest.icdoProviderReportDataBatchRequest.status_value = busConstant.Vendor_Payment_Status_Processed;
            aobjBatchRequest.icdoProviderReportDataBatchRequest.Update();
        }

        /// <summary>
        ///  Set Batch Request ID to Provider Report Data Insurance records
        /// </summary>
        /// <param name="aclbRecords">Provider Report Data Insurance records that were processed for payment</param>
        /// <param name="aintBatchRequestID">Batch Request ID</param>
        private void SetBatchRequestID(Collection<busProviderReportDataInsurance> aclbRecords, int aintBatchRequestID)
        {
            foreach (busProviderReportDataInsurance lobjPRDI in aclbRecords)
            {
                lobjPRDI.icdoProviderReportDataInsurance.batch_request_id = aintBatchRequestID;
                lobjPRDI.icdoProviderReportDataInsurance.Update();
            }
        }

        private void SetBatchRequestIDMedicare(Collection<busProviderReportDataMedicarePartD> aclbRecords, int aintBatchRequestID)
        {
            foreach (busProviderReportDataMedicarePartD lobjPRDM in aclbRecords)
            {
                lobjPRDM.icdoProviderReportDataMedicare.batch_request_id = aintBatchRequestID;
                lobjPRDM.icdoProviderReportDataMedicare.Update();
            }
        }

        #endregion

        public void LoadACHProviderReportData(string astrBenefitType)
        {
            _iclbACHProviderReportData = new Collection<busACHProviderReportData>();
            DataTable ldtbACHData = new DataTable();
            /// Loads the ACH data based on the Benefit Type
            if (astrBenefitType == busConstant.Provider_Retirement)
            {
                ldtbACHData = busBase.Select("entProviderReportDataDC.LoadDCDataGroupByOrgIDPlanID", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
                foreach (DataRow dr in ldtbACHData.Rows)
                {
                    AddACHProviderData(dr);
                }
            }
            if (astrBenefitType == busConstant.Provider_DeffComp)
            {
                foreach (object lobjProviderOrgCode in iarrProviders)
                {
                    if (lobjProviderOrgCode.ToString() == busGlobalFunctions.GetData1ByCodeValue(5012, busConstant.Provider_BankOfNorthDakota, iobjPassInfo) ||
                        lobjProviderOrgCode.ToString() == busGlobalFunctions.GetData1ByCodeValue(5012, busConstant.Provider_WaddellAndReed, iobjPassInfo))
                    {
                        continue;
                    }
                    ldtbACHData = busBase.Select("cdoProviderReportDataDeffComp.LoadDeffCompDataGroupByOrgIDPlanID", new object[2] { Convert.ToString(lobjProviderOrgCode), 
                        iobjSystemManagement.icdoSystemManagement.batch_date });
                    foreach (DataRow dr in ldtbACHData.Rows)
                    {
                        decimal ldclContributionAmount = Convert.ToDecimal(dr["CONTRIBUTION_AMOUNT"]);
                        string lstrOrgCode = Convert.ToString(dr["ORG_CODE"]);
                        string lstrOrgName = Convert.ToString(dr["ORG_NAME"]);
                        int lintProviderOrgID = Convert.ToInt32(dr["PROVIDER_ORG_ID"]);
                        int lintPlanID = Convert.ToInt32(dr["PLAN_ID"]);
                        string lstrPaymentOption = Convert.ToString(dr["payment_option_value"]);
                        /// Adds to the ACH Provider Report Data collection only if the Org Plan Payment Option is ACH
                        if (lstrPaymentOption == busConstant.PaymentOption_ACH)
                        {
                            busACHProviderReportData lobjACH = new busACHProviderReportData();
                            lobjACH.lstrOrgCodeID = lstrOrgCode;
                            if (lstrOrgName != string.Empty)
                            {
                                lobjACH.lstrOrgName = lstrOrgName.Trim().ToUpper();
                                if (lobjACH.lstrOrgName.Length > 16)
                                    lobjACH.lstrOrgName = lobjACH.lstrOrgName.Substring(0, 16);
                            }
                            lobjACH.LoadDFIAccountNo(lintProviderOrgID);
                            lobjACH.ldclContributionAmount = ldclContributionAmount;
                            lobjACH.lstrTransactionCode = GetTransactionCode(lobjACH, lintProviderOrgID);
                            if (lobjACH.ldclContributionAmount > 0.00M)
                                _iclbACHProviderReportData.Add(lobjACH);
                        }
                    }
                }
            }
            if (astrBenefitType == busConstant.Provider_Insurance)
            {
                ldtbACHData = busBase.Select("cdoProviderReportDataInsurance.LoadInsuranceDataGroupByOrgIDPlanID", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
                
                foreach (DataRow dr in ldtbACHData.Rows)
                {
                    decimal ldclContributionAmount = Convert.ToDecimal(dr["CONTRIBUTION_AMOUNT"]);
                    string lstrOrgCode = Convert.ToString(dr["ORG_CODE"]);
                    string lstrOrgName = Convert.ToString(dr["ORG_NAME"]);
                    int lintProviderOrgID = Convert.ToInt32(dr["PROVIDER_ORG_ID"]);
                    int lintPlanID = Convert.ToInt32(dr["PLAN_ID"]);
                    string lstrPaymentOption = Convert.ToString(dr["payment_option_value"]);
                    /// Adds to the ACH Provider Report Data collection only if the Org Plan Payment Option is ACH
                    if (lstrPaymentOption == busConstant.PaymentOption_ACH 
                        && iarrInsurancePlansRequested.Contains(lintPlanID)
                        )
                    {
                        busACHProviderReportData lobjACH = new busACHProviderReportData();
                        lobjACH.lstrOrgCodeID = lstrOrgCode;
                        if (lstrOrgName != string.Empty)
                        {
                            lobjACH.lstrOrgName = lstrOrgName.Trim().ToUpper();
                            if (lobjACH.lstrOrgName.Length > 16)
                                lobjACH.lstrOrgName = lobjACH.lstrOrgName.Substring(0, 16);
                        }
                        lobjACH.LoadDFIAccountNo(lintProviderOrgID);
                        lobjACH.ldclContributionAmount = ldclContributionAmount;
                        lobjACH.lstrTransactionCode = GetTransactionCode(lobjACH, lintProviderOrgID);
                        if (lobjACH.ldclContributionAmount > 0.00M)
                            _iclbACHProviderReportData.Add(lobjACH);
                    }
                }
            } //PIR 14848 - Medicare Part D changes
            if (astrBenefitType == busConstant.Provider_Insurance_Medicare)
            {
                ldtbACHData = busBase.Select("cdoProviderReportDataMedicarePartD.LoadMedicareInsuranceDataGroupByOrgIDPlanID", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });

                foreach (DataRow dr in ldtbACHData.Rows)
                {
                    decimal ldclContributionAmount = Convert.ToDecimal(dr["CONTRIBUTION_AMOUNT"]);
                    string lstrOrgCode = Convert.ToString(dr["ORG_CODE"]);
                    string lstrOrgName = Convert.ToString(dr["ORG_NAME"]);
                    int lintProviderOrgID = Convert.ToInt32(dr["PROVIDER_ORG_ID"]);
                    int lintPlanID = Convert.ToInt32(dr["PLAN_ID"]);
                    string lstrPaymentOption = Convert.ToString(dr["payment_option_value"]);
                    /// Adds to the ACH Provider Report Data collection only if the Org Plan Payment Option is ACH
                    if (lstrPaymentOption == busConstant.PaymentOption_ACH
                        && iarrInsurancePlansRequested.Contains(lintPlanID)
                        )
                    {
                        busACHProviderReportData lobjACH = new busACHProviderReportData();
                        lobjACH.lstrOrgCodeID = lstrOrgCode;
                        if (lstrOrgName != string.Empty)
                        {
                            lobjACH.lstrOrgName = lstrOrgName.Trim().ToUpper();
                            if (lobjACH.lstrOrgName.Length > 16)
                                lobjACH.lstrOrgName = lobjACH.lstrOrgName.Substring(0, 16);
                        }
                        lobjACH.LoadDFIAccountNo(lintProviderOrgID);
                        lobjACH.ldclContributionAmount = ldclContributionAmount;
                        lobjACH.lstrTransactionCode = GetTransactionCode(lobjACH, lintProviderOrgID);
                        if (lobjACH.ldclContributionAmount > 0.00M)
                            _iclbACHProviderReportData.Add(lobjACH);
                    }
                }
            }
        }

        public string GetTransactionCode(busACHProviderReportData aobjACH, int aintOrgID)
        {
            string lstrTransactionCode = string.Empty;
            busOrgBank lobjOrgBank = new busOrgBank { icdoOrgBank = new cdoOrgBank() };
            DataTable ldtOrgBank = busBase.Select<cdoOrgBank>(new string[3] { "org_id", "usage_value", "status_value" },
                            new object[3] { aintOrgID, busConstant.BankUsageDirectDeposit, busConstant.OrgBankStatusActive }, null, null);
            if (ldtOrgBank.Rows.Count > 0)
                lobjOrgBank.icdoOrgBank.LoadData(ldtOrgBank.Rows[0]);
            if (lobjOrgBank.icdoOrgBank.account_type_value == busConstant.BankAccountSavings)
                lstrTransactionCode = busConstant.CreditTransactionCodeNonPrenoteSavings;
            else if (lobjOrgBank.icdoOrgBank.account_type_value == busConstant.BankAccountChecking)
                lstrTransactionCode = busConstant.CreditTransactionCodeNonPrenoteChecking;
            return lstrTransactionCode;
        }
        private void AddACHProviderData(DataRow adtr)
        {
            decimal ldclContributionAmount = Convert.ToDecimal(adtr["CONTRIBUTION_AMOUNT"]);
            string lstrOrgCode = Convert.ToString(adtr["ORG_CODE"]);
            string lstrOrgName = Convert.ToString(adtr["ORG_NAME"]);
            int lintProviderOrgID = Convert.ToInt32(adtr["PROVIDER_ORG_ID"]);
            int lintPlanID = Convert.ToInt32(adtr["PLAN_ID"]);
            string lstrPaymentOption = Convert.ToString(adtr["payment_option_value"]);

            /// Adds to the ACH Provider Report Data collection only if the Org Plan Payment Option is ACH
            if (lstrPaymentOption == busConstant.PaymentOption_ACH)
            {
                busACHProviderReportData lobjACH = new busACHProviderReportData();
                lobjACH.lstrOrgCodeID = lstrOrgCode;
                if (lstrOrgName != string.Empty)
                {
                    lobjACH.lstrOrgName = lstrOrgName.Trim().ToUpper();
                    if (lobjACH.lstrOrgName.Length > 16)
                        lobjACH.lstrOrgName = lobjACH.lstrOrgName.Substring(0, 16);
                }
                lobjACH.LoadDFIAccountNo(lintProviderOrgID);
                lobjACH.ldclContributionAmount = ldclContributionAmount;
                lobjACH.lstrTransactionCode = GetTransactionCode(lobjACH, lintProviderOrgID);
                if (lobjACH.ldclContributionAmount > 0.00M)
                    _iclbACHProviderReportData.Add(lobjACH);
            }
        }
        private int InsertBatchRequestID()
        {
            cdoProviderReportDataBatchRequest lobjBatchRequest = new cdoProviderReportDataBatchRequest();
            lobjBatchRequest.effective_start_date = iobjSystemManagement.icdoSystemManagement.batch_date;
            lobjBatchRequest.visibility_flag = busConstant.Flag_No;
            lobjBatchRequest.status_value = busConstant.Vendor_Payment_Status_Processed;
            lobjBatchRequest.Insert();
            return lobjBatchRequest.provider_report_data_batch_request_id;
        }
        //PIR 24921
        private int InsertBatchRequestIDOtherPlan(int aintOrgID,int aintPlanID)
        {
            cdoProviderReportDataBatchRequest lobjBatchRequest = new cdoProviderReportDataBatchRequest();
            lobjBatchRequest.effective_start_date = iobjSystemManagement.icdoSystemManagement.batch_date;
            lobjBatchRequest.visibility_flag = busConstant.Flag_Yes;
            lobjBatchRequest.org_id = aintOrgID;
            lobjBatchRequest.plan_id = aintPlanID;
            lobjBatchRequest.status_value = busConstant.Vendor_Payment_Status_Processed;
            lobjBatchRequest.Insert();
            return lobjBatchRequest.provider_report_data_batch_request_id;
        }

        public DateTime idtBatchDate { get; set; }
        private void GenerateGLByType(int aintPlanID, string astrItemType, decimal adecTotalAmount, int aintOrgID, int aintSourceID)
        {
            if (idtBatchDate == DateTime.MinValue)
                idtBatchDate = busGlobalFunctions.GetSysManagementBatchDate();            

            //Generate Fund Transfer GL for RHIC and HSA Vendor Payment
            if (astrItemType == busConstant.ItemTypeRHICAmount || astrItemType == busConstant.ItemTypeHSAVendorPayment)
            {
                cdoAccountReference lcdoAcccountReferenceTransfer = new cdoAccountReference();
                lcdoAcccountReferenceTransfer.plan_id = aintPlanID;
                lcdoAcccountReferenceTransfer.source_type_value = busConstant.GLSourceTypeValueVendorPayment;
                lcdoAcccountReferenceTransfer.transaction_type_value = busConstant.TransactionTypeTransfer;
                lcdoAcccountReferenceTransfer.status_transition_value = string.Empty;
                lcdoAcccountReferenceTransfer.item_type_value = astrItemType;
                busGLHelper.GenerateGL(lcdoAcccountReferenceTransfer, 0, aintOrgID, aintSourceID, adecTotalAmount, idtBatchDate, idtBatchDate, iobjPassInfo);
            }
            else
            {
                cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
                lcdoAcccountReference.plan_id = aintPlanID;
                lcdoAcccountReference.source_type_value = busConstant.GLSourceTypeValueVendorPayment;
                lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeItemLevel;
                lcdoAcccountReference.item_type_value = astrItemType;

                busGLHelper.GenerateGL(lcdoAcccountReference, 0, aintOrgID, aintSourceID, adecTotalAmount, idtBatchDate, idtBatchDate, iobjPassInfo);
            }
        }

        /// <summary>
        /// Inserts split info into Provider Report Data Insurance Split
        /// Used only for health plan
        /// </summary>
        /// <param name="adrSplitInfo"></param>
        private void InsertIntoSplitInfo(DataRow adrSplitInfo)
        {
            cdoProviderReportDataInsuranceSplit lcdoSplitInfo = new cdoProviderReportDataInsuranceSplit();
            lcdoSplitInfo.LoadData(adrSplitInfo);
            lcdoSplitInfo.Insert();
        }


        #region PIR 14264

        public DataTable idtPaymentDetailsAdhoc { get; set; }
        public DataTable idtPayments { get; set; }
        public DateTime idtPaymentDate { get; set; }
        public busDBCacheData ibusDBCacheData { get; set; }

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
            idtPaymentDate = ldtPaymentDate;

            //Loading DB Cache (optimization)
            LoadDBCacheData();

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
            idtPaymentDetailsAdhoc.Rows.Add(drNew);
        }

        #endregion


    }
}
