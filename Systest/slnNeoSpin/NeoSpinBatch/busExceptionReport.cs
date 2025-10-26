#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    class busExceptionReport : busNeoSpinBatch
    {
        public busExceptionReport()
        {

        }

        DataTable idtResultTable = new DataTable();
        public void GenerateExceptionReport()
        {
            istrProcessName = "Generating Exception Batch Report";

            idtResultTable = CreateExceptionBatchReportDataset();

            DataTable ldtbProcess = busBase.Select("entBpmActivityInstance.rptExceptionReport", new object[] { });           

            foreach (DataRow dr in ldtbProcess.Rows)
            {
                AddToNewDataRow(dr);
            }

            if (idtResultTable.Rows.Count > 0)
            {
                //create report for Insufficient report details
                CreateReport("rptExceptionReport.rpt", idtResultTable);

                idlgUpdateProcessLog("Generating Exception report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }

        }

        private DataTable CreateExceptionBatchReportDataset()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("ACTIVITY_ID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("PERSON_ID", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("STANDARD_TIME", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("SUMDATEDIFF", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("TIMEDIFF", Type.GetType("System.Int32"));
            DataColumn ldc6 = new DataColumn("FORMATEDSTD_TIME", Type.GetType("System.String"));
            DataColumn ldc7 = new DataColumn("FORMATED_ACTUALDURATION", Type.GetType("System.String"));
            DataColumn ldc8 = new DataColumn("FORMATTED_TIMEDIFF", Type.GetType("System.String"));
            DataColumn ldc9 = new DataColumn("CHECKED_OUT_USER", Type.GetType("System.String"));
            DataColumn ldc10 = new DataColumn("PROCESS_NAME", Type.GetType("System.String"));
            DataColumn ldc11 = new DataColumn("ACTIVITY_NAME", Type.GetType("System.String"));
            
            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);
            ldtbReportTable.Columns.Add(ldc7);
            ldtbReportTable.Columns.Add(ldc8);
            ldtbReportTable.Columns.Add(ldc9);
            ldtbReportTable.Columns.Add(ldc10);
            ldtbReportTable.Columns.Add(ldc11);
            
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(DataRow adrow)
        {
            DataRow dr = idtResultTable.NewRow();
            dr["ACTIVITY_ID"] = adrow["ACTIVITY_ID"];
            dr["PERSON_ID"] = adrow["PERSON_ID"];
            dr["STANDARD_TIME"] = adrow["STANDARD_TIME"];
            dr["SUMDATEDIFF"] = adrow["SUMDATEDIFF"];
            dr["TIMEDIFF"] = adrow["TIMEDIFF"];
            dr["FORMATEDSTD_TIME"] = adrow["FORMATEDSTD_TIME"];
            dr["FORMATED_ACTUALDURATION"] = adrow["FORMATED_ACTUALDURATION"];
            dr["FORMATTED_TIMEDIFF"] = adrow["FORMATTED_TIMEDIFF"];
            dr["CHECKED_OUT_USER"] = adrow["CHECKED_OUT_USER"];
            dr["PROCESS_NAME"] = adrow["PROCESS_NAME"];
            dr["ACTIVITY_NAME"] = adrow["ACTIVITY_NAME"];
            
            idtResultTable.Rows.Add(dr);

        }
    }
}
