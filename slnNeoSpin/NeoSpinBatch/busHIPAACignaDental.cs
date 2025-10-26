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
    public class busHIPAACignaDental : busNeoSpinBatch
    {
        public void CreateHIPAACignaDentalFile()
        {
            Collection<int> lclbDental = new Collection<int>();
            DataTable ldtbDental = busBase.Select("cdoPersonAccountGhdv.HIPAA_834_Dental_CIGNA", new object[] { });
            DataTable ldtbAllPersonAccountDependent = busBase.Select("cdoPersonAccountGhdv.LoadAllDependents", new object[1] { busConstant.PlanIdDental });
            DataTable ldtbAllPersonAddress = busBase.Select("cdoPersonAccountGhdv.LoadAllAddress", new object[1] { busConstant.PlanIdDental });
            DataTable ldtbAllPersonEmployment = busBase.Select("cdoPersonAccountGhdv.LoadAllEmployment", new object[1] { busConstant.PlanIdDental });
            int lintCurrIndex = 0;
            foreach (DataRow dr in ldtbDental.Rows)
            {
                lclbDental.Add(lintCurrIndex);
                lintCurrIndex++;
            }
            if (lclbDental.Count > 0)
            {
                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                lobjProcessFiles.iobjSystemManagement = iobjSystemManagement;
                idlgUpdateProcessLog("Creating CIGNA HIPAA file for " + ldtbDental.Rows.Count.ToString() + " Members", "INFO", "HIPAA CIGNA Dental");
                lobjProcessFiles.iarrParameters = new object[5];
                lobjProcessFiles.iarrParameters[0] = lclbDental;
                lobjProcessFiles.iarrParameters[1] = ldtbDental;
                lobjProcessFiles.iarrParameters[2] = ldtbAllPersonAccountDependent;
                lobjProcessFiles.iarrParameters[3] = ldtbAllPersonAddress;
                lobjProcessFiles.iarrParameters[4] = ldtbAllPersonEmployment;
                lobjProcessFiles.CreateOutboundFile(54);
            }
            else
            {
                idlgUpdateProcessLog("No Records Found.", "INFO", "HIPAA CIGNA Dental");
            }
        }
    }
}
