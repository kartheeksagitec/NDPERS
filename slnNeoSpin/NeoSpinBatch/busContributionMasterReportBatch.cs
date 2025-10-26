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
using System.IO;
using System.Text;
using Sagitec.ExceptionPub;
#endregion

namespace NeoSpinBatch
{
    public class busContributionMasterReportBatch : busNeoSpinBatch
    {
        /// <summary>
        /// Main function to create reports
        /// </summary>
        public void CreateReports()
        {
            istrProcessName = "Generate Contribution Master Reports";
            //if (DateTime.Today.Day == DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))
            //{            
            idlgUpdateProcessLog("Creating Contribution Master Report", "INFO", istrProcessName);
            CreateContributionMasterReport();
            //}
            ////else
            //    idlgUpdateProcessLog("Contribution Master Reports not generated", "INFO", istrProcessName);            
        }

        /// <summary>
        /// Method to create Contribution Master Report
        /// </summary>
        private void CreateContributionMasterReport()
        {
            try
            {
                DataTable ldtContributionMasterReport = busBase.Select("cdoPersonAccountRetirementContribution.rptContributionMaster",
                    new object[1] { DateTime.Today });
                if (ldtContributionMasterReport.Rows.Count > 0)
                    CreateReport("rptContributionMaster.rpt", ldtContributionMasterReport);
                idlgUpdateProcessLog("Contribution Master Report generated successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Contribution Master Report generation failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
            }
        }
    }
}
