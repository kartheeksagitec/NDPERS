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
using Sagitec.ExceptionPub;
#endregion

namespace NeoSpinBatch
{
    public class busPaymentHistoryDistributionStatusReport : busNeoSpinBatch
    {
        public void GeneratePaymentHistoryDistributionStatusReport()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            idlgUpdateProcessLog(istrProcessName + " Batch Started", "INFO", istrProcessName);
            try
            {
                idlgUpdateProcessLog("Generating Payment History Distribution Status Report", "INFO", istrProcessName);
                DateTime ldtPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate();
                //The system must generate the ‘Payment History Distribution Status’ report defined is UCS-078 for all checks with 
                //a status other than ‘Cleared’ and ‘Outstanding’ with ‘Transaction Date’ between the first day of the previous month through the last day of the previous month inclusive..
                DataTable ldtbStatusReport = busBase.Select("cdoPaymentHistoryDistribution.rptPaymentHistoryDistributionStatusBatchReport",
                                                                        new object[1] { ldtPaymentDate });
                if (ldtbStatusReport.Rows.Count > 0)
                {
                    CreateReport("rptPaymentHistoryDistributionStatus.rpt", ldtbStatusReport);
                }
                idlgUpdateProcessLog("Generating Payment History Distribution Status Report Successful", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Generating Payment History Distribution Status Report Failed", "INFO", istrProcessName);
            }
        }
    }
}
