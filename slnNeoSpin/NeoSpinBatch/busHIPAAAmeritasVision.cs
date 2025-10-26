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
    public class busHIPAAAmeritasVision : busNeoSpinBatch
    {
        public void CreateHIPAAAmeritasVisionFile(bool ablnIsSuperVision = false)
        {
            string lstrStepName = ablnIsSuperVision ? "HIPAA Superior Vision" : "HIPAA Vision Ameritas";
            Collection<int> lclbVision = new Collection<int>();
            DataTable ldtbVision = new DataTable();
            if (ablnIsSuperVision)
                ldtbVision = busBase.Select("cdoPersonAccountGhdv.HIPAA_834_Vision_SuperiorVision", new object[3] {
                                                                                        iobjSystemManagement.icdoSystemManagement.batch_date,
                                                                                        iobjSystemManagement.icdoSystemManagement.batch_date.GetLastDayofMonth(),
                                                                                        iobjSystemManagement.icdoSystemManagement.batch_date.GetFirstDayofCurrentMonth()});
            else
                ldtbVision = busBase.Select("cdoPersonAccountGhdv.HIPAA_834_Vision_Ameritas", new object[] { });
            DataTable ldtbAllPersonAccountDependent = busBase.Select("cdoPersonAccountGhdv.LoadAllDependents", new object[2] { busConstant.PlanIdVision, 
                                                                                                                iobjSystemManagement.icdoSystemManagement.batch_date});
            DataTable ldtbAllPersonAddress = busBase.Select("cdoPersonAccountGhdv.LoadAllAddress", new object[1] { busConstant.PlanIdVision });
            DataTable ldtbAllPersonEmployment = busBase.Select("cdoPersonAccountGhdv.LoadAllEmployment", new object[1] { busConstant.PlanIdVision });
            int lintCurrIndex = 0;
            foreach (DataRow dr in ldtbVision.Rows)
            {
                lclbVision.Add(lintCurrIndex);
                lintCurrIndex++;
            }
            if (lclbVision.Count > 0)
            {
                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                lobjProcessFiles.iobjSystemManagement = iobjSystemManagement;
                idlgUpdateProcessLog("Creating HIPAA file for " + ldtbVision.Rows.Count.ToString() + " Members", "INFO", lstrStepName);
                lobjProcessFiles.iarrParameters = new object[6];
                lobjProcessFiles.iarrParameters[0] = lclbVision;
                lobjProcessFiles.iarrParameters[1] = ldtbVision;
                lobjProcessFiles.iarrParameters[2] = ldtbAllPersonAccountDependent;
                lobjProcessFiles.iarrParameters[3] = ldtbAllPersonAddress;
                lobjProcessFiles.iarrParameters[4] = ldtbAllPersonEmployment;
                lobjProcessFiles.iarrParameters[5] = ablnIsSuperVision;
                lobjProcessFiles.CreateOutboundFile(55);
            }
            else
            {
                idlgUpdateProcessLog("No Records Found.", "INFO", lstrStepName);
            }
        }
    }
}
