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
    public class busAchStatusUpdateBatch : busNeoSpinBatch
    {
        //Load all the payments which are having payment status as OUTSTANDING and Update the Status to cleared  
        public void UpdateACHStatus()
        {
            istrProcessName = "ACH Status Update Batch";

            idlgUpdateProcessLog("Updating ACH Status", "INFO", istrProcessName);
            try
            {
                DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.ACHStatusUpdateBatch",
                    new object[3] { busPayeeAccountHelper.GetLastCheckEffectiveDate(), iobjSystemManagement.icdoSystemManagement.batch_date , iobjBatchSchedule.batch_schedule_id},
                                          iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            catch(Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Updating ACH Status failed", "INFO", istrProcessName);
                throw e;
            }
            idlgUpdateProcessLog("Updating ACH Status Successful", "INFO", istrProcessName);
        }
    }
}
