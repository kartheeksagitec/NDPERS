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
using Sagitec.ExceptionPub;

namespace NeoSpinBatch
{
    public class busPrintingBatchLetter : busNeoSpinBatch
    {
        public void GetAllLettersToPrint()
        {
            istrProcessName = "Printing Letters";            
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            DataTable ldtbResults = busNeoSpinBase.Select("entCorTracking.GetLettersForBatchPrinting", new object[0] { });
            foreach (DataRow ldrRow in ldtbResults.Rows)
            {
                string lstrTemplateName = ldrRow["TEMPLATE_NAME"].ToString();
                utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
                cdoCorTemplates lobjCorTemplate = new cdoCorTemplates();
                lobjCorTemplate.LoadByTemplateName(lstrTemplateName);

                utlCorresPondenceInfo lobjCorresPondenceInfo =
                    lobjPassInfo.isrvMetaDataCache.GetCorresPondenceInfo(lstrTemplateName);
                bool lblnPrintSuccess = false;
                lobjCorresPondenceInfo.istrPrinterName = lobjCorTemplate.printer_name_description;
                lobjCorresPondenceInfo.iintCorrespondenceTrackingId = Convert.ToInt32(ldrRow["TRACKING_ID"]);
                string strSlNo = lobjCorresPondenceInfo.iintCorrespondenceTrackingId.ToString().PadLeft(10, '0');
                lobjCorresPondenceInfo.istrGeneratedFileName = lobjCorresPondenceInfo.istrTemplateName + "-" + strSlNo + ".docx";
                string lstrCorrsGenPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
                lstrCorrsGenPath = lstrCorrsGenPath + "\\" + lobjCorresPondenceInfo.istrGeneratedFileName;
                try
                {
                    cdoCorTracking lcdoCorTracking = new cdoCorTracking();
                    if (lcdoCorTracking.SelectRow(new object[1] { lobjCorresPondenceInfo.iintCorrespondenceTrackingId }))
                    {
                        if (!string.IsNullOrEmpty(lobjCorresPondenceInfo.istrPrinterName))
                        {
                            OpenDocReadonly(lstrCorrsGenPath);                            
                            PrintDoc(lobjCorresPondenceInfo.istrPrinterName, string.Empty);
                            lcdoCorTracking.cor_status_value = busConstant.CorrespondenceStatus_Generated;
                            lcdoCorTracking.Update();
                            lblnPrintSuccess = true;
                        }
                        else
                        {
                            if (lobjCorresPondenceInfo.iintCorrespondenceTrackingId > 0)
                            {
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
                if (lblnPrintSuccess)                             
                     CreateFileNetImage(lobjCorresPondenceInfo.istrGeneratedFileName);  
            } // end of foreach
        }
    }
}
