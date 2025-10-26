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
    public class busHIPAAHealthBCBS : busNeoSpinBatch
    {
        public void CreateHIPAAHealthBCBSFile()
        {
            Collection<int> lclbHealth = new Collection<int>();
            // SYS PIR ID 2625: Only select effective dates up to 60 days for the future dated enrolled records.
            DataTable ldtbHealthBCBS = busBase.Select("cdoPersonAccountGhdv.HIPAA_834_Health_BCBS", new object[3] { 
                                                                iobjSystemManagement.icdoSystemManagement.batch_date,
                                                                iobjSystemManagement.icdoSystemManagement.batch_date.GetLastDayofMonth(),
                                                                iobjSystemManagement.icdoSystemManagement.batch_date.GetFirstDayofCurrentMonth()});
            DataTable ldtbAllDependents = busBase.Select("cdoPersonAccountGhdv.LoadAllHealthDependents", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtbAllAddresses = busBase.Select("cdoPersonAccountGhdv.LoadAllAddress", new object[1] { busConstant.PlanIdGroupHealth });
            DataTable ldtbAllEmployments = busBase.Select("cdoPersonAccountGhdv.LoadallHealthEmployments", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtbAllOrgPlans = busBase.Select("cdoPersonAccountGhdv.LoadAllOrgPlans", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtbAllDependentMember = busBase.Select("cdoPersonAccountGhdv.LoadAllHealthDependentMember", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            int lintCurrIndex = 0;
            foreach (DataRow dr in ldtbHealthBCBS.Rows)
            {
                lclbHealth.Add(lintCurrIndex);
                lintCurrIndex++;
            }
            if (lclbHealth.Count > 0)
            {
                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                lobjProcessFiles.iobjSystemManagement = iobjSystemManagement;
                idlgUpdateProcessLog("Creating BCBS HIPAA file for " + ldtbHealthBCBS.Rows.Count.ToString() + " Members", "INFO", "HIPAA Health BCBS");
                lobjProcessFiles.iarrParameters = new object[7];
                lobjProcessFiles.iarrParameters[0] = lclbHealth;
                lobjProcessFiles.iarrParameters[1] = ldtbHealthBCBS;
                lobjProcessFiles.iarrParameters[2] = ldtbAllDependents;
                lobjProcessFiles.iarrParameters[3] = ldtbAllAddresses;
                lobjProcessFiles.iarrParameters[4] = ldtbAllEmployments;
                lobjProcessFiles.iarrParameters[5] = ldtbAllOrgPlans;
                lobjProcessFiles.iarrParameters[6] = ldtbAllDependentMember;
                lobjProcessFiles.CreateOutboundFile(53);
            }
            else
            {
                idlgUpdateProcessLog("No Records Found.", "INFO", "HIPAA Health BCBS");
            }
        }
    }
}
