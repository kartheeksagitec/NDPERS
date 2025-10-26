using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System;

namespace NeoSpinBatch
{
    public class busDeleteSessionStoreBatch : busNeoSpinBatch
    {
        public void DeleteSessionStore()
        {
            istrProcessName = "Process Delete Session Store";
            try
            {
                int lintNoOfRecordsAffected = Sagitec.DBUtility.DBFunction.DBNonQuery("entSystemManagement.DeleteSessionStore",
                                                                                        new object[] { Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(busConstant.SystemConstantCodeID, "DLSN", iobjPassInfo)) },
                                                                                        utlPassInfo.iobjPassInfo.iconFramework,
                                                                                        utlPassInfo.iobjPassInfo.itrnFramework);
                //idlgUpdateProcessLog("Total number of session store entries deleted - " + lintNoOfRecordsAffected.ToString(), "INFO", istrProcessName);
            }
            catch(Exception ex)
            {
                idlgUpdateProcessLog($"Process Delete Session Store - Error Occured - {ex.Message}", "ERR", istrProcessName);
            }
        }
    }
}
