using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.IO;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

namespace NeoSpinBatch
{
    class busPurgeCorrespondence : busNeoSpinBatch
    {
        /// <summary>
        /// Function to Purge the Generated,Imaged Correspondence after 24 Hours
        /// It will not purge the Printed Correspondence
        /// </summary>		
        /// <param>None</param>
        /// <returns>None</returns>
        public void ExecuteCorrespondencePurgeProcess()
        {
            string lstrGeneratedPath = iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
            string lstrPurgedPath = iobjPassInfo.isrvDBCache.GetPathInfo("CorrPurg");
            string lstrImagedPath = iobjPassInfo.isrvDBCache.GetPathInfo("CorrImag");

            DataTable ldtbList = busNeoSpinBase.Select("cdoCorTracking.LoadCorrespondenceForPurging", new object[0] { });
            foreach (DataRow ldtrResult in ldtbList.Rows)
            {
                utlPassInfo.iobjPassInfo.BeginTransaction();
                busCorTracking ibusCorTracking = new busCorTracking();
                try
                {
                    ibusCorTracking.icdoCorTracking = new cdoCorTracking();
                    cdoCorTracking icdoCorTracking = ibusCorTracking.icdoCorTracking;
                    icdoCorTracking.LoadData(ldtrResult);
                    ibusCorTracking.ibusCorTemplates = new busCorTemplates();
                    busCorTemplates ibusCorTemplates = ibusCorTracking.ibusCorTemplates;
                    ibusCorTemplates.icdoCorTemplates = new cdoCorTemplates();
                    ibusCorTemplates.icdoCorTemplates.LoadData(ldtrResult);
                    //OpenXML Conversion Code
                    string lstrSourcePath = lstrGeneratedPath + ibusCorTracking.istrWordFileName;
                    string lstrDestinationPath = lstrPurgedPath + ibusCorTracking.istrWordFileName;
                    if (!ibusCorTracking.FileExists(lstrSourcePath))
                    {
                        lstrSourcePath = lstrGeneratedPath + ibusCorTracking.istrWordFileNameDoc;
                        lstrDestinationPath = lstrPurgedPath + ibusCorTracking.istrWordFileNameDoc;
                    }
                    //string lstrSourcePath = lstrGeneratedPath + ibusCorTracking.istrWordFileName;
                    //string lstrDestinationPath = lstrPurgedPath + ibusCorTracking.istrWordFileName;
                    if (File.Exists(lstrSourcePath))
                    {
                        File.Move(lstrSourcePath, lstrDestinationPath);
                        idlgUpdateProcessLog(ibusCorTracking.istrWordFileName + " file moved to Purged Folder", "INFO",
                                             "Purge Correspondence");
                    }

                    ////UAT PIR 89 - Don't Update the Imaged Correspondence Status to Purged. But move the Doc to Purged Folder.
                    /// {Update} This CODE is commented now because imaged correspondence documents are moved to purged folder 
                    /// immediately after pushing it to filenet through neoflow service
                    //if (icdoCorTracking.cor_status_value != busConstant.CorrespondenceStatus_Imaged)
                    //{
                    icdoCorTracking.cor_status_value = busConstant.CorrespondenceStatus_Purged;
                    icdoCorTracking.Update();
                    idlgUpdateProcessLog(
                        "Tracking ID : " + icdoCorTracking.tracking_id.ToString() + " status changed to Purged", "INFO",
                        "Correspondence - Purge Correspondence");
                    //}
                    utlPassInfo.iobjPassInfo.Commit();
                }
                catch (Exception e)
                {
                    utlPassInfo.iobjPassInfo.Rollback();
                    idlgUpdateProcessLog("Error occurred : for file " + ibusCorTracking.istrWordFileName + e.Message, "ERR", "Correspondence - Purge Correspondence");
                }
            }
        }
    }
}
