using System;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;


namespace NeoSpinBatch
{
    public class busProcessRequiredMinimumDistributionNotices : busNeoSpinBatch
    {
        public void ProcessRMDMonthlyNotices()
        {
            istrProcessName = "Process RMD Notices - Monthly Batch";
            DateTime ldtBatchDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            DateTime ldtBatchEndDate = ldtBatchDate.AddMonths(1).AddDays(-1);
            // Records Fetched as per the rule: BR-093-01
            DataTable ldtbResult = busBase.Select("cdoBenefitApplication.ProcessRMDBatch", new object[1] { ldtBatchEndDate });
            if (ldtbResult.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Processing RMD Notices", "INFO", istrProcessName);
                foreach (DataRow dr in ldtbResult.Rows)
                {
                    busPersonAccount lobjPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    lobjPersonAccount.icdoPersonAccount.LoadData(dr);
                    try
                    {
                        idlgUpdateProcessLog("Processing Person Id = " + lobjPersonAccount.icdoPersonAccount.person_id, "INFO", istrProcessName);
                        // Initializing RMD workflow
                        InitializeWorkFlow(busConstant.Map_Process_RMD,
                                          lobjPersonAccount.icdoPersonAccount.person_id, 0,
                                          busConstant.WorkflowProcessStatus_UnProcessed,
                                          busConstant.WorkflowProcessSource_Batch);
                        lobjPersonAccount.icdoPersonAccount.rmd_batch_initiated_flag = busConstant.Flag_Yes;
                        lobjPersonAccount.icdoPersonAccount.Update();

                    }
                    catch (Exception e)
                    {
                        idlgUpdateProcessLog("Not able to process Person Id = " + lobjPersonAccount.icdoPersonAccount.person_id + "for the following exception" + e, "INFO", istrProcessName); ;
                    }
                }
            }
            else
                idlgUpdateProcessLog("No Records Fetched", "INFO", istrProcessName);
        }
    }
}
