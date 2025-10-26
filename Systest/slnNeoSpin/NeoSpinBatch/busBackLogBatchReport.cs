#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    class busBackLogBatchReport : busNeoSpinBatch
    {
        public busBackLogBatchReport()
        {

        }

        DataTable idtResultTable = new DataTable();
        public void GenerateBackLogReport()
        {
            istrProcessName = "Generating BackLog Batch Report";

            idtResultTable = CreateBackLogBatchReportDataset();

            DataTable ldtbProcess = busBase.Select("entBpmActivityInstance.rptActivitiesInStatusExceeding3Days", new object[] { });

            foreach (DataRow dr in ldtbProcess.Rows)
            {
                AddToNewDataRow(dr);
            }

            if (idtResultTable.Rows.Count > 0)
            {
                //create report for Insufficient report details
                CreateReport("rptActivitiesInStatusExceeding3Days.rpt", idtResultTable);

                idlgUpdateProcessLog("Generating BackLog report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }

        }

        private DataTable CreateBackLogBatchReportDataset()
        {
            DataTable ldtbReportTable = new DataTable();
           // DataColumn ldc1 = new DataColumn("ACTIVITY_INSTANCE_ID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("PERSLinkID_OrgID", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("LastName_OrgName", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("Initiated", Type.GetType("System.DateTime"));
            DataColumn ldc5 = new DataColumn("current_status", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("Role", Type.GetType("System.String"));
            DataColumn ldc7 = new DataColumn("elapsed_time", Type.GetType("System.String"));
            DataColumn ldc8 = new DataColumn("activity_name", Type.GetType("System.String"));           

           // ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);
            ldtbReportTable.Columns.Add(ldc7);
            ldtbReportTable.Columns.Add(ldc8);           

            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(DataRow adrow)
        {
            DataRow dr = idtResultTable.NewRow();            
            //dr["ACTIVITY_INSTANCE_ID"] = adrow["ACTIVITY_INSTANCE_ID"];
            dr["PERSLinkID_OrgID"] = adrow["PERSLinkID_OrgID"];
            dr["LastName_OrgName"] = adrow["LastName_OrgName"];
            dr["Initiated"] = adrow["Initiated"];
            dr["current_status"] = adrow["current_status"];
            dr["Role"] = adrow["Role"];
            dr["elapsed_time"] = adrow["elapsed_time"];
            dr["activity_name"] = adrow["activity_name"];

            idtResultTable.Rows.Add(dr);

        }
    }
}
