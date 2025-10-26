#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    class busProcessBatchReport : busNeoSpinBatch
    {
        public busProcessBatchReport()
        {

        }

        DataTable idtResultTable = new DataTable();
        public void GenerateProcessBatchReport()
        {
            istrProcessName = "Generating Process Batch Report";

            idtResultTable = CreateProcessBatchReportDataset();
           
            DataTable ldtbProcess = busBase.Select("entSolBpmActivityInstance.rptProcessBatchReport", new object[] { });           

            foreach (DataRow dr in ldtbProcess.Rows)
            {
                AddToNewDataRow(dr);
            }

            if (idtResultTable.Rows.Count > 0)
            {
                //create report for Insufficient report details
                CreateReport("rptProcessBatch.rpt", idtResultTable);

                idlgUpdateProcessLog("Generating Monthly Process Batch report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }

        }

        private DataTable CreateProcessBatchReportDataset()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("ACTIVITY_NAME", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("ACTION_USER_ID", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("PROCESS_NAME", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("INPC_COUNT", Type.GetType("System.Int32"));
            DataColumn ldc5 = new DataColumn("INPC_TIME_MEAN_AVG", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("PROC_COUNT", Type.GetType("System.Int32"));
            DataColumn ldc7 = new DataColumn("PROC_TIME_MEAN_AVG", Type.GetType("System.String"));
            DataColumn ldc8 = new DataColumn("RELE_COUNT", Type.GetType("System.Int32"));
            DataColumn ldc9 = new DataColumn("RELE_TIME_MEAN_AVG", Type.GetType("System.String"));
            DataColumn ldc10 = new DataColumn("SUSP_COUNT", Type.GetType("System.Int32"));
            DataColumn ldc11 = new DataColumn("SUSP_TIME_MEAN_AVG", Type.GetType("System.String"));
            DataColumn ldc12 = new DataColumn("RETU_COUNT", Type.GetType("System.Int32"));
            DataColumn ldc13 = new DataColumn("RETU_TIME_MEAN_AVG", Type.GetType("System.String"));
            DataColumn ldc14 = new DataColumn("RESU_COUNT", Type.GetType("System.Int32"));
            DataColumn ldc15 = new DataColumn("RESU_TIME_MEAN_AVG", Type.GetType("System.String"));
            DataColumn ldc16 = new DataColumn("REAU_COUNT", Type.GetType("System.Int32"));
            DataColumn ldc17 = new DataColumn("REAU_TIME_MEAN_AVG", Type.GetType("System.String"));
            DataColumn ldc18 = new DataColumn("UNPC", Type.GetType("System.Int32"));
            DataColumn ldc19 = new DataColumn("INPC", Type.GetType("System.Int32"));
            DataColumn ldc20 = new DataColumn("PROC", Type.GetType("System.Int32"));
            DataColumn ldc21 = new DataColumn("RELE", Type.GetType("System.Int32"));
            DataColumn ldc22 = new DataColumn("SUSP", Type.GetType("System.Int32"));
            DataColumn ldc23 = new DataColumn("CANC", Type.GetType("System.Int32"));
            DataColumn ldc24 = new DataColumn("RETU", Type.GetType("System.Int32"));
            DataColumn ldc25 = new DataColumn("REAU", Type.GetType("System.Int32"));
            DataColumn ldc26 = new DataColumn("RESU", Type.GetType("System.Int32"));

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
            ldtbReportTable.Columns.Add(ldc12);
            ldtbReportTable.Columns.Add(ldc13);
            ldtbReportTable.Columns.Add(ldc14);
            ldtbReportTable.Columns.Add(ldc15);
            ldtbReportTable.Columns.Add(ldc16);
            ldtbReportTable.Columns.Add(ldc17);
            ldtbReportTable.Columns.Add(ldc18);
            ldtbReportTable.Columns.Add(ldc19);
            ldtbReportTable.Columns.Add(ldc20);
            ldtbReportTable.Columns.Add(ldc21);
            ldtbReportTable.Columns.Add(ldc22);
            ldtbReportTable.Columns.Add(ldc23);
            ldtbReportTable.Columns.Add(ldc24);
            ldtbReportTable.Columns.Add(ldc25);
            ldtbReportTable.Columns.Add(ldc26);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }


        public void AddToNewDataRow(DataRow adrow)
        {
            idlgUpdateProcessLog("Processing Process name : " + adrow["PROCESS_NAME"], "INFO", istrProcessName);

            DataRow dr = idtResultTable.NewRow();
            dr["ACTIVITY_NAME"] = adrow["ACTIVITY_NAME"];
            dr["ACTION_USER_ID"] = adrow["ACTION_USER_ID"];
            dr["PROCESS_NAME"] = adrow["PROCESS_NAME"];
            dr["INPC_COUNT"] = adrow["INPC_COUNT"];
            dr["INPC_TIME_MEAN_AVG"] = adrow["INPC_TIME_MEAN_AVG"];
            dr["PROC_COUNT"] = adrow["PROC_COUNT"];
            dr["PROC_TIME_MEAN_AVG"] = adrow["PROC_TIME_MEAN_AVG"];
            dr["RELE_COUNT"] = adrow["RELE_COUNT"];
            dr["RELE_TIME_MEAN_AVG"] = adrow["RELE_TIME_MEAN_AVG"];
            dr["SUSP_COUNT"] = adrow["SUSP_COUNT"];
            dr["SUSP_TIME_MEAN_AVG"] = adrow["SUSP_TIME_MEAN_AVG"];
            dr["RETU_COUNT"] = adrow["RETU_COUNT"];
            dr["RETU_TIME_MEAN_AVG"] = adrow["RETU_TIME_MEAN_AVG"];
            dr["RESU_COUNT"] = adrow["RESU_COUNT"];
            dr["RESU_TIME_MEAN_AVG"] = adrow["RESU_TIME_MEAN_AVG"];
            dr["REAU_COUNT"] = adrow["REAU_COUNT"];
            dr["REAU_TIME_MEAN_AVG"] = adrow["REAU_TIME_MEAN_AVG"];
            dr["UNPC"] = adrow["UNPC"];
            dr["INPC"] = adrow["INPC"];
            dr["PROC"] = adrow["PROC"];
            dr["RELE"] = adrow["RELE"];
            dr["SUSP"] = adrow["SUSP"];
            dr["CANC"] = adrow["CANC"];
            dr["RETU"] = adrow["RETU"];
            dr["REAU"] = adrow["REAU"];
            dr["RESU"] = adrow["RESU"];

            idtResultTable.Rows.Add(dr);

        }
    }
}
