#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    class busUserBatchReport : busNeoSpinBatch
    {
        public busUserBatchReport()
        {

        }

        DataTable idtResultTable = new DataTable();
        public void GenerateUserBatchReport()
        {
            istrProcessName = "Generating User Batch Report";

            idtResultTable = CreateUserBatchReportDataset();
           
            DataTable ldtbProcess = busBase.Select("entBpmActivityInstance.rptUserBatchReport", new object[] { });           

            foreach (DataRow dr in ldtbProcess.Rows)
            {
                AddToNewDataRow(dr);
            }

            if (idtResultTable.Rows.Count > 0)
            {
                //create report for Insufficient report details
                CreateReport("rptUserBatch.rpt", idtResultTable);

                idlgUpdateProcessLog("Generating Monthly User Batch report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }

        }

        private DataTable CreateUserBatchReportDataset()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("ACTION_USER_ID", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("ACTIVITY_ID", Type.GetType("System.Int32"));
            DataColumn ldc3 = new DataColumn("STATUS_DESCRIPTION", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("COUNT", Type.GetType("System.Int32"));
            DataColumn ldc5 = new DataColumn("MAXDATEDIFF", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("MEDIANDATEDIFF", Type.GetType("System.String"));
            DataColumn ldc7 = new DataColumn("MINDATEDIFF", Type.GetType("System.String"));
            DataColumn ldc8 = new DataColumn("MEAN_AVERAGE", Type.GetType("System.String"));
            DataColumn ldc9 = new DataColumn("ROLE_NAME", Type.GetType("System.String"));     

            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);
            ldtbReportTable.Columns.Add(ldc7);
            ldtbReportTable.Columns.Add(ldc8);
            ldtbReportTable.Columns.Add(ldc9);
            
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(DataRow adrow)
        {
            DataRow dr = idtResultTable.NewRow();
            dr["ACTION_USER_ID"] = adrow["ACTION_USER_ID"];
            dr["ACTIVITY_ID"] = adrow["ACTIVITY_ID"];
            dr["STATUS_DESCRIPTION"] = adrow["STATUS_DESCRIPTION"];
            dr["COUNT"] = adrow["COUNT"];
            dr["MAXDATEDIFF"] = adrow["MAXDATEDIFF"];
            dr["MEDIANDATEDIFF"] = adrow["MEDIANDATEDIFF"];
            dr["MINDATEDIFF"] = adrow["MINDATEDIFF"];
            dr["MEAN_AVERAGE"] = adrow["MEAN_AVERAGE"];
            dr["ROLE_NAME"] = adrow["ROLE_NAME"];
        
            idtResultTable.Rows.Add(dr);
        }
    }
}
