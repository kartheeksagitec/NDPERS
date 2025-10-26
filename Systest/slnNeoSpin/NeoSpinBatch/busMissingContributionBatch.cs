#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Globalization;
using System.Collections;
using Sagitec.CorBuilder;
using NeoSpin.DataObjects;
#endregion

namespace NeoSpinBatch
{
    class busMissingContributionBatch : busNeoSpinBatch
    {
        public void CreateCorrForMissingContribution()
        {
            istrProcessName = "Missing Contribution Batch";
            idlgUpdateProcessLog("Missing Contribution Batch Started", "INFO", istrProcessName);
            DataTable ldtbAllOrgsMissingContributions =
                        DBFunction.DBSelect("entWssPersonEmployment.LoadMissingRetirementContributions",
                                            new object[0] { },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            IEnumerable<string> lenuDistinctOrgCodes = ldtbAllOrgsMissingContributions
                                                        .AsEnumerable()
                                                        .Select(dr => dr.Field<string>(enmOrganization.org_code.ToString()))
                                                        .Distinct();
            foreach (string lstrOrgCode in lenuDistinctOrgCodes)
            {
                DataTable ldtbPerOrgMissingContrib = ldtbAllOrgsMissingContributions
                                                        .FilterTable(busConstant.DataType.String, enmOrganization.org_code.ToString(), lstrOrgCode).CopyToDataTable();
                if(ldtbPerOrgMissingContrib.Rows.Count > 0)
                {
                    idlgUpdateProcessLog("Creating Missing Contribution Report for Org Code ID : " + lstrOrgCode, "INFO", istrProcessName);
                    ldtbPerOrgMissingContrib.TableName = busConstant.ReportTableName;
                    DataSet ldstMissingContrib = new DataSet();
                    ldstMissingContrib.Tables.Add(ldtbPerOrgMissingContrib.Copy());
                    //Create missing contribution report pdf report
                    CreateReportWithPrefix("rptMissingContributions.rpt", ldstMissingContrib, lstrOrgCode+"_");
                }

            }
            idlgUpdateProcessLog("Missing Contribution Batch Ended", "INFO", istrProcessName);
        }
    }
}
