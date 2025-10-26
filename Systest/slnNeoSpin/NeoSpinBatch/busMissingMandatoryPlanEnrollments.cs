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
    public class busMissingMandatoryPlanEnrollments : busNeoSpinBatch
    {
        public void CreateMissingMandatoryPlanEnrollmentsReport()
        {
            try
            {
                istrProcessName = "Missing Mandatory Plan Enrollments Report";
                idlgUpdateProcessLog("Creating Missing Mandatory Plan Enrollments Report", "INFO", istrProcessName);
                DataTable ldtbMissingMandatoryPlanEnrollments = busBase.Select("cdoPerson.MissingMandatoryPlanEnrollments", new object[0] { });
                CreateReport("rptMissingMandatoryPlanEnrollments.rpt", ldtbMissingMandatoryPlanEnrollments);
                idlgUpdateProcessLog("Missing Mandatory Plan Enrollments Report Generated Successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
              idlgUpdateProcessLog("Missing Mandatory Plan Enrollments Report generation failed", "INFO", istrProcessName);
            }
        }
    }
}
