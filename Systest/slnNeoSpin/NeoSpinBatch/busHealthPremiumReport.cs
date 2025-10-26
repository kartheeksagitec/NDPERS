#region Using directives

using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpinBatch
{
    public class busHealthPremiumReport : busNeoSpinBatch
    {
        public void CreateHealthPremiumReport()
        {
            istrProcessName = "Health Premium Report";
            idlgUpdateProcessLog("Getting Non Processed Request", "INFO", istrProcessName);
            DataTable ldtbResults = busBase.Select<cdoHealthPremiumReportBatchRequest>(new string[1] { "STATUS_VALUE" }, new object[1]{
                                                busConstant.Vendor_Payment_Status_NotProcessed}, null, null);
            foreach (DataRow ldtrRequest in ldtbResults.Rows)
            {
                busHealthPremiumReportBatchRequest lobjBatchRequest = new busHealthPremiumReportBatchRequest
                {
                    icdoHealthPremiumReportBatchRequest = new cdoHealthPremiumReportBatchRequest()
                };
                lobjBatchRequest.icdoHealthPremiumReportBatchRequest.LoadData(ldtrRequest);                
                DataSet ldsResult = lobjBatchRequest.CreateHealthPremiumReport();

                idlgUpdateProcessLog("Creating Report for Batch Request ID :" + lobjBatchRequest.icdoHealthPremiumReportBatchRequest.batch_request_id.ToString(), "INFO", istrProcessName);                                    
                CreateReportWithPrefix("rptHealthPremiumReport.rpt", ldsResult, lobjBatchRequest.icdoHealthPremiumReportBatchRequest.batch_request_id.ToString() + "_");

                lobjBatchRequest.icdoHealthPremiumReportBatchRequest.status_value = busConstant.Vendor_Payment_Status_Processed;
                lobjBatchRequest.icdoHealthPremiumReportBatchRequest.Update();
            }
        }
    }
}
