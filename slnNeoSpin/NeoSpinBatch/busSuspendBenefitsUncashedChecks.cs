using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpinBatch
{
    public class busSuspendBenefitsUncashedChecks : busNeoSpinBatch
    {
        public void SuspendBenefitsUncashedChecks()
        {
            istrProcessName = "Suspend Benefits Uncashed Checks Batch";
            idlgUpdateProcessLog("Loading all Payee Account have 2 or more Outstanding payments.", "INFO", istrProcessName);
            DataTable ldtbResult = busBase.Select("cdoPayeeAccount.LoadPayeeAccountWithOutstandingStatus", new object[0] { });
            if(ldtbResult.Rows.Count > 0)
            {
                    bool lblnTransaction = false;
                    int lintrtn = 0;
                    try
                    {
                        if (!lblnTransaction)
                        {
                            utlPassInfo.iobjPassInfo.BeginTransaction();
                            lblnTransaction = true;
                        }
                        idlgUpdateProcessLog("Initailization of Suspend Benefits Uncashed Checks worklfow for outstanding payees.", "INFO", istrProcessName);
                        lintrtn = DBFunction.DBNonQuery("cdoPayeeAccount.InsertWorkflowRequestForProcess359", new object[2] {                                     
                                    iobjSystemManagement.icdoSystemManagement.batch_date, iobjBatchSchedule.batch_schedule_id,                                   
                                }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (lblnTransaction)
                        {
                            utlPassInfo.iobjPassInfo.Commit();
                            lblnTransaction = false;
                        }
                        idlgUpdateProcessLog("Total " + lintrtn + " worklfow have been initaited.", "INFO", istrProcessName);
                    }
                    catch (Exception Ex)
                    {
                        ExceptionManager.Publish(Ex);
                        idlgUpdateProcessLog("Suspend Benefits Uncashed Checks Batch failed " + " " +" Message : " + Ex.Message, "ERR", iobjBatchSchedule.step_name);
                        if (lblnTransaction)
                        {
                            utlPassInfo.iobjPassInfo.Rollback();
                            lblnTransaction = false;
                        }
                    }
            }
        }
    }
}
