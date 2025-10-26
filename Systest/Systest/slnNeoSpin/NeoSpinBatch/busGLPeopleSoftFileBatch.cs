#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    public class busGLPeopleSoftFileBatch : busNeoSpinBatch
    {
        public void ProcessGLPeopleSoftFile()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            CreateGLReport();
            CreateGLFile();
        }

        private void CreateGLFile()
        {
            try
            {             
                idlgUpdateProcessLog("Creation of G/L Peoplesoft File started", "INFO", istrProcessName);
                // G/L Out File For PeopleSoft - Daily Run
                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                lobjProcessFiles.iarrParameters = new object[1];
                lobjProcessFiles.iarrParameters[0] = iobjSystemManagement.icdoSystemManagement.batch_date;
                lobjProcessFiles.CreateOutboundFile(busConstant.GLPeopleSoftOutFileID);
                idlgUpdateProcessLog("Creation of G/L Peoplesoft File successfully completed", "INFO", istrProcessName);
               
            }
            catch (Exception e)
            {
                idlgUpdateProcessLog("Creation of G/L Peoplesoft File failed", "INFO", istrProcessName);
                throw e;
            }
        }

        private void CreateGLReport()
        {
            try
            {
                idlgUpdateProcessLog("Creation of G/L Report started", "INFO", istrProcessName);
                DataTable ldtReportResult = busBase.Select("cdoGLTransaction.rptGLPeopleSoft",
                    new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date }); //PIR 16031 - Changed from Today's date to batch date
                if (ldtReportResult.Rows.Count > 0)
                    CreateReport("rptGLPeopleSoft.rpt", ldtReportResult);
                idlgUpdateProcessLog("Creation of G/L Report successfully completed", "INFO", istrProcessName);

            }
            catch (Exception e)
            {
                idlgUpdateProcessLog("Creation of G/L Report failed", "INFO", istrProcessName);
                throw e;
            }
        }
    }
}
