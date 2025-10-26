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
    public class busHIPAAHealthSanford : busNeoSpinBatch
    {
        public void CreateHIPAAHealthSanfordFile()
        {
            Collection<int> lclbHealthSanford = new Collection<int>();
            DataTable ldtbHealthData = busBase.Select("cdoPersonAccountGhdv.HIPAA_834_Health_Sanford", new object[3] { 
                                                                iobjSystemManagement.icdoSystemManagement.batch_date,
                                                                iobjSystemManagement.icdoSystemManagement.batch_date.GetLastDayofMonth(),
                                                                iobjSystemManagement.icdoSystemManagement.batch_date.GetFirstDayofCurrentMonth()});
            DataTable ldtbAllDependents = busBase.Select("cdoPersonAccountGhdv.LoadAllHealthDependentsSanford", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtbAllAddresses = busBase.Select("cdoPersonAccountGhdv.LoadAllAddress", new object[1] { busConstant.PlanIdGroupHealth });
            DataTable ldtbAllEmployments = busBase.Select("cdoPersonAccountGhdv.LoadallHealthEmploymentsSanford", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtbAllOrgPlans = busBase.Select("cdoPersonAccountGhdv.LoadAllOrgPlansSanford", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtbAllDependentMember = busBase.Select("cdoPersonAccountGhdv.LoadAllHealthDependentMemberSanford", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            int lintCurrIndex = 0;
            foreach (DataRow dr in ldtbHealthData.Rows)
            {
                lclbHealthSanford.Add(lintCurrIndex);
                lintCurrIndex++;
            }
            if (lclbHealthSanford.Count > 0)
            {
                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                lobjProcessFiles.iobjSystemManagement = iobjSystemManagement;
                idlgUpdateProcessLog("Creating Sanford HIPAA file for " + ldtbHealthData.Rows.Count.ToString() + " Members", "INFO", "HIPAA Health Sanford");
                lobjProcessFiles.iarrParameters = new object[7];
                lobjProcessFiles.iarrParameters[0] = lclbHealthSanford;
                lobjProcessFiles.iarrParameters[1] = ldtbHealthData;
                lobjProcessFiles.iarrParameters[2] = ldtbAllDependents;
                lobjProcessFiles.iarrParameters[3] = ldtbAllAddresses;
                lobjProcessFiles.iarrParameters[4] = ldtbAllEmployments;
                lobjProcessFiles.iarrParameters[5] = ldtbAllOrgPlans;
                lobjProcessFiles.iarrParameters[6] = ldtbAllDependentMember;
                lobjProcessFiles.CreateOutboundFile(97);
            }
            else
            {
                idlgUpdateProcessLog("No Records Found.", "INFO", "HIPAA Health BCBS");
            }
        }
    }
}
