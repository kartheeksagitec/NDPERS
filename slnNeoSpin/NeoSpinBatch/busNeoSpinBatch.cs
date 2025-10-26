using System;
using System.Collections;
using System.Data;
using Sagitec.BusinessObjects;
using Sagitec.CorBuilder;
using Sagitec.Common;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;
using Sagitec.ExceptionPub;
using Sagitec.Bpm;

namespace NeoSpinBatch
{
    public class busNeoSpinBatch
    {
        public static CorBuilderXML iobjCorBuilder;
        public delegate void UpdateProcessLog(string astrMessage, string astrMessageType, string astrStepName);
        public UpdateProcessLog idlgUpdateProcessLog;
        public busSystemManagement iobjSystemManagement;
        public cdoBatchSchedule iobjBatchSchedule;

        //Variables used during processing
        public string istrProcessName;
        public int iintCount = 0;
        public int iintInternalCount = 0;
        public int iintLimit;
        public int iintMaxCount;
        public ReportDocument irptBatch;
        private string istrRBUrl;
        private string iabsRptDefPath;
        private string iabsRptGenPath;
        public static Microsoft.Office.Interop.Word._Application WordApp;
        public _Document doc;

        public void StartCounter()
        {
            idlgUpdateProcessLog("Started processing.. ", "INFO", istrProcessName);
            iintCount = 0;
            iintInternalCount = 0;
        }

        public void StartCounter(int aintLimit)
        {
            StartCounter();
            iintLimit = aintLimit;
        }

        public void CallCounter()
        {
            if (iintInternalCount == iintLimit)
            {
                if (iintMaxCount != 0)
                {
                    //display this message if max count is available
                    idlgUpdateProcessLog("Currently processing record " + iintCount + " of " + iintMaxCount, "INFO", istrProcessName);
                }
                else
                {
                    idlgUpdateProcessLog("Currently processing record " + iintCount, "INFO", istrProcessName);
                }
                iintInternalCount = 0;
            }
            iintCount++;
            iintInternalCount++;
        }

        public void EndCounter()
        {
            idlgUpdateProcessLog("Ended processing.. Total records read " + iintCount, "INFO", istrProcessName);
        }

        public utlPassInfo iobjPassInfo
        {
            get
            {
                return utlPassInfo.iobjPassInfo;
            }
        }


        static busNeoSpinBatch()
        {
            iobjCorBuilder = new CorBuilderXML();
            iobjCorBuilder.InstantiateWord();
            WordApp = new Microsoft.Office.Interop.Word.Application();

        }

        //Call this Method outside the Loop (From Caller) for Optimization
        public void InitializeReportBuilder(string astrReportGNPath)
        {
            iabsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptDF");
            iabsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(astrReportGNPath);
        }

        /// <summary>
        /// Generate correspondence, create the tracking record for the generated letter
        /// </summary>
        /// <param name="aintTemplateID"></param>
        /// <param name="aintPersonID"></param>
        /// <param name="astrUserId"></param>
        /// <param name="aarrResult"></param>
        /// <returns></returns>
        public string CreateCorrespondence(string astrTemplateName, object aarrResult, Hashtable ahtbQueryBkmarks)
        {
            cdoCorTemplates lobjCorTemplate = new cdoCorTemplates();
            lobjCorTemplate.LoadByTemplateName(astrTemplateName);
            if (lobjCorTemplate.IsNotNull() && lobjCorTemplate.active_flag != busConstant.Flag_Yes)
            {
                idlgUpdateProcessLog("Correspondence is not active - " + astrTemplateName, "INFO", istrProcessName);
                return string.Empty;
                //throw new Exception("Unable to create correspondence, Correspondence is not active.");
            }
            utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence(astrTemplateName, aarrResult, ahtbQueryBkmarks);

            if (lobjCorresPondenceInfo == null)
            {
                throw new Exception("Unable to create correspondence, SetCorrespondence method not found in " +
                    " business solutions base object");
            }

            string lstrFileName = "";
            lstrFileName = iobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName,
                lobjCorresPondenceInfo, iobjPassInfo.istrUserID);
            //PIR 15529 - Autoprint functionality
            try
            {
                if (lobjCorresPondenceInfo.istrAutoPrintFlag == busConstant.Flag_Yes)
                {
                    if (!string.IsNullOrEmpty(lobjCorresPondenceInfo.istrPrinterName))
                    {
                        OpenDocReadonly(lobjCorresPondenceInfo.istrGeneratePath + lstrFileName);
                        PrintDoc(lobjCorresPondenceInfo.istrPrinterName, string.Empty);
                    }
                    else
                    {
                        if (lobjCorresPondenceInfo.iintCorrespondenceTrackingId > 0)
                        {
                            cdoCorTracking lcdoCorTracking = new cdoCorTracking();
                            lcdoCorTracking.SelectRow(new object[1] { lobjCorresPondenceInfo.iintCorrespondenceTrackingId });
                            idlgUpdateProcessLog("The printer name value is not set for template ID : " + Convert.ToString(lcdoCorTracking.template_id), "INFO", istrProcessName);
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                ExceptionManager.Publish(_ex);
                if (lobjCorresPondenceInfo.iintCorrespondenceTrackingId > 0)
                {
                    idlgUpdateProcessLog("The AutoPrint failed for tracking ID : " + Convert.ToString(lobjCorresPondenceInfo.iintCorrespondenceTrackingId) + ",  " +
                    "Message : " + _ex.Message, "ERR", istrProcessName);
                }
            }
            if (doc.IsNotNull())
            {
                doc.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
                doc = null;
            }
            //Push into FileNet only When AutoPrint Flag is Checked
            if (lobjCorresPondenceInfo.istrAutoPrintFlag == busConstant.Flag_Yes || astrTemplateName == "PAY-4261")
            {
                CreateFileNetImage(lstrFileName);
            }
            return lstrFileName;
        }
        public void OpenDocReadonly(string astrFilename)
        {
            System.Object FileName = astrFilename;
            System.Object ConfirmConversions = Type.Missing;
            System.Object ReadOnly = true;
            System.Object AddToRecentFiles = Type.Missing;
            System.Object PasswordDocument = Type.Missing;
            System.Object PasswordTemplate = Type.Missing;
            System.Object Revert = Type.Missing;
            System.Object WritePasswordDocument = Type.Missing;
            System.Object WritePasswordTemplate = Type.Missing;
            System.Object Format = Type.Missing;
            System.Object Encoding = Type.Missing;
            System.Object Visible = true;
            System.Object OpenAndRepair = Type.Missing;
            System.Object DocumentDirection = Type.Missing;
            System.Object NoEncodingDialog = Type.Missing;
            System.Object XMLTransform = Type.Missing;

            doc = WordApp.Documents.Open(ref  FileName, ref  ConfirmConversions, ref  ReadOnly,
            ref  AddToRecentFiles, ref  PasswordDocument, ref  PasswordTemplate,
            ref  Revert, ref  WritePasswordDocument, ref  WritePasswordTemplate,
            ref  Format, ref  Encoding, ref  Visible, ref  OpenAndRepair,
            ref  DocumentDirection, ref  NoEncodingDialog, ref  XMLTransform);
        }
        public void PrintDoc(string astrPrinterName, string astrPrintToFileName)
        {
            //Print to currently set active printer if blank
            if (astrPrinterName != "")
            {
                WordApp.ActivePrinter = astrPrinterName;
            }

            System.Object PrintToFile = Type.Missing;
            System.Object OutputFileName = Type.Missing;
            if (astrPrintToFileName != "")
            {
                PrintToFile = true;
                OutputFileName = astrPrintToFileName;
            }

            System.Object Background = Type.Missing;
            System.Object Append = Type.Missing;
            System.Object Range = Microsoft.Office.Interop.Word.WdPrintOutRange.wdPrintAllDocument;
            System.Object From = Type.Missing;
            System.Object To = Type.Missing;
            System.Object Item = Type.Missing;
            System.Object Copies = 1;
            System.Object Pages = Type.Missing;
            System.Object PageType = Type.Missing;

            System.Object Collate = true;
            System.Object ActivePrinterMacGX = Type.Missing;
            System.Object ManualDuplexPrint = Type.Missing;
            System.Object PrintZoomColumn = Type.Missing;
            System.Object PrintZoomRow = Type.Missing;
            System.Object PrintZoomPaperWidth = Type.Missing;
            System.Object PrintZoomPaperHeight = Type.Missing;

            WordApp.ActiveDocument.PrintOut(ref Background, ref Append, ref Range, ref OutputFileName,
                ref From, ref To, ref Item, ref Copies, ref Pages, ref PageType, ref PrintToFile,
                ref Collate, ref ActivePrinterMacGX, ref ManualDuplexPrint, ref PrintZoomColumn,
                ref PrintZoomRow, ref PrintZoomPaperWidth, ref PrintZoomPaperHeight);
        }
        public string CreateReport(string astrReportName, DataTable adstResult, string astrReportGNPath = busConstant.ReportPath)
        {
            return CreateReportWithPrefix(astrReportName, adstResult, string.Empty, astrReportGNPath);
        }
        public string CreateReportWithPrefix(string astrReportName, DataTable adstResult, string astrPrefix, string astrReportGNPath = busConstant.ReportPath)
        {
            InitializeReportBuilder(astrReportGNPath);

            string lstrReportFullName = string.Empty;
            irptBatch = new ReportDocument();
            irptBatch.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init            
            irptBatch.Load(iabsRptDefPath + astrReportName);
            // gets the data and bind to the report doc control
            irptBatch.SetDataSource(adstResult);

            lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            irptBatch.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName + ".pdf");
            irptBatch.Close();
            irptBatch.Dispose();
            return lstrReportFullName;
        }

        public string CreateReport(string astrReportName, DataSet adstResult, string astrReportGNPath = busConstant.ReportPath)
        {
            return CreateReportWithPrefix(astrReportName, adstResult, string.Empty, astrReportGNPath);
        }

        public string CreateReportWithPrefix(string astrReportName, DataSet adstResult, string astrPrefix, string astrReportGNPath = busConstant.ReportPath)
        {
            InitializeReportBuilder(astrReportGNPath);

            string lstrReportFullName = string.Empty;
            irptBatch = new ReportDocument();
            irptBatch.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init            
            irptBatch.Load(iabsRptDefPath + astrReportName);
            // gets the data and bind to the report doc control
            irptBatch.SetDataSource(adstResult);

            lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            irptBatch.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName + ".pdf");
            irptBatch.Close();
            irptBatch.Dispose();
            return lstrReportFullName;
        }

        // Initialize the report documnet. This event removes any databse logon information 
        // saved in the report. The call to Load the report in the above function fires this event.
        private void OnReportDocInit(object sender, System.EventArgs e)
        {
            irptBatch.SetDatabaseLogon("", "");
        }

        public void CreateFileNetImage(string astrFileName)
        {
            int lintTrackingID = Convert.ToInt32(astrFileName.Substring(astrFileName.LastIndexOf("-") + 1, 10));

            cdoCorTracking lcdoTracking = new cdoCorTracking();
            if (lcdoTracking.SelectRow(new object[1] { lintTrackingID }))
            {
                busCorTemplates lobjCorTemplates = new busCorTemplates();
                if (lobjCorTemplates.FindCorTemplates(lcdoTracking.template_id))
                {
                    if (!(String.IsNullOrEmpty(lobjCorTemplates.icdoCorTemplates.filenet_document_type_value) ||
                            String.IsNullOrEmpty(lobjCorTemplates.icdoCorTemplates.image_doc_category_value) ||
                            String.IsNullOrEmpty(lobjCorTemplates.icdoCorTemplates.document_code)))
                    {
                        lcdoTracking.cor_status_value = busConstant.CorrespondenceStatus_Ready_For_Imaging;
                        lcdoTracking.Update();
                    }
                    else
                        idlgUpdateProcessLog(lobjCorTemplates.icdoCorTemplates.template_name + " - " +
                                        "Template's FileNet Details are Null or Empty", "INFO", istrProcessName);
                }
                else
                    idlgUpdateProcessLog("Template ID" + Convert.ToString(lcdoTracking.template_id) + " is Invalid", "INFO", istrProcessName);
            }
            else
                idlgUpdateProcessLog("Tracking ID" + Convert.ToString(lintTrackingID) + " is Invalid", "INFO", istrProcessName);
        }

        //Create Contact Ticket
        public void CreateContactTicket(int aintPersonId, string astrContactType, cdoContactTicket aobjContactTicket)
        {
            CreateContactTicket(aintPersonId, astrContactType, String.Empty, aobjContactTicket);
        }

        public void CreateContactTicket(int aintPersonId, string astrContactType, string astrResponseType, cdoContactTicket aobjContactTicket)
        {
            CreateContactTicket(aintPersonId, astrContactType, string.Empty, astrResponseType, aobjContactTicket);
        }

        public void CreateContactTicket(int aintPersonId, string astrContactType, string astrContactMethod,
                                                    string astrResponseType, cdoContactTicket aobjContactTicket)
        {
            // Creating contact ticket
            busContactTicket lobjNewContactTicket = new busContactTicket
            {
                icdoContactTicket = new cdoContactTicket
                    {
                        person_id = aintPersonId,
                        contact_type_value = astrContactType,
                        contact_method_value = astrContactMethod,
                        response_method_value = astrResponseType,
                        status_value = busConstant.ContactTicketStatusOpen
                    }
            };
            lobjNewContactTicket.icdoContactTicket.Insert();

            // Closing contact ticket
            busContactTicket lobjUpdateContactTicket = new busContactTicket();
            lobjUpdateContactTicket.FindContactTicket(lobjNewContactTicket.icdoContactTicket.contact_ticket_id);
            lobjUpdateContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusClosed;
            lobjUpdateContactTicket.icdoContactTicket.NeedHistory = true; // SysTest PIR 1504
            lobjUpdateContactTicket.icdoContactTicket.Update();

            aobjContactTicket = lobjNewContactTicket.icdoContactTicket;
        }

        public int InitializeWorkFlow(int aintWorkFlowMapID, int aintPersonID, int aintReferenceID, string astrStatusValue, string astrSourceValue, int aintOrgID = 0)
        {
            
            busBpmRequest lbpmRequest = busWorkflowHelper.InitiateBpmRequest(aintWorkFlowMapID, aintPersonID, aintOrgID, aintReferenceID, iobjPassInfo, astrSourceValue);
            return lbpmRequest != null ? lbpmRequest.icdoBpmRequest.request_id : 0;
        }
		
		public void CreateReportForPayeesWithMixedPayments(int aintPaymentScheduleId)
        {
            busNeoSpinBase lbusBase = new busNeoSpinBase();
            idlgUpdateProcessLog("Payees with mixed payments", "INFO", istrProcessName);
            DataTable ldtMultiplePayment = busBase.Select("cdoPaymentHistoryDistribution.LoadPayeesWithMixedPayments", new object[1] { aintPaymentScheduleId });
            ldtMultiplePayment.TableName = busConstant.ReportTableName;
            if (ldtMultiplePayment.Rows.Count > 0)
            {
                 DataSet ldsReportResult = new DataSet();
                 ldsReportResult.Tables.Add(ldtMultiplePayment.Copy());
                 CreateReportWithPrefix("rptPayeesWithMixedPayments.rpt", ldsReportResult, aintPaymentScheduleId + "_" + "FINAL_", busConstant.PaymentReportPath);
                idlgUpdateProcessLog("Payees with mixed payments Report generated succesfully", "INFO", istrProcessName);
                lbusBase.CreateExcelReport("rptPayeesWithMixedPayments.rpt", ldsReportResult, aintPaymentScheduleId + "_" + "_", busConstant.PaymentReportPath);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
        }
        public void CreateReportForRefundsWithPayrollAdjustments(int aintPaymentScheduleId)
        {
            busNeoSpinBase lbusBase = new busNeoSpinBase();
            idlgUpdateProcessLog("Generating Report For Refunds With Payroll Adjustments", "INFO", istrProcessName);
            DataTable ldtbRefundsWithPayrollAdjustments = busBase.Select("cdoPaymentSchedule.LoadFinalMonthlyOrAdhocRefundsWithPayrollAdjustments", new object[1] { aintPaymentScheduleId });
            ldtbRefundsWithPayrollAdjustments.TableName = busConstant.ReportTableName;
            if (ldtbRefundsWithPayrollAdjustments.Rows.Count > 0)
            {
                DataSet ldsReportResult = new DataSet();
                ldsReportResult.Tables.Add(ldtbRefundsWithPayrollAdjustments.Copy());
                CreateReportWithPrefix("rptRefundsWithPayrollAdjustments.rpt", ldsReportResult, aintPaymentScheduleId + "_" + "FINAL_", busConstant.PaymentReportPath);
                idlgUpdateProcessLog("Report For Refunds With Payroll Adjustments Generated Succesfully", "INFO", istrProcessName);
                lbusBase.CreateExcelReport("rptRefundsWithPayrollAdjustments.rpt", ldsReportResult, aintPaymentScheduleId + "_" + "_", busConstant.PaymentReportPath);
            }
            else
            {
                idlgUpdateProcessLog("No Refunds With Payroll Adjustments Found.", "INFO", istrProcessName);
            }
        }
    }
}
