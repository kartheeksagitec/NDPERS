#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    class busNoHDVDependentReportBatch : busNeoSpinBatch
    {
        public busNoHDVDependentReportBatch()
        {

        }

        DataTable idtResultTable = new DataTable();
        public void GenerateNoHDvDependentReport()
        {
            istrProcessName = "Generating Insurance Discrepancies Batch Report";

            idtResultTable = CreateInsuranceDiscrepanciesBatchReportDataset();

            DateTime ldtBatchRunDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            DataTable ldtbProcess = busBase.Select("cdoPersonAccountGhdv.rptInsuranceDiscrepanciesReport", new object[1] { ldtBatchRunDate });
            
            foreach (DataRow dr in ldtbProcess.Rows)
            {
                AddToNewDataRow(dr);
            }

            if (idtResultTable.Rows.Count > 0)
            {
                //create report for Insufficient report details
                CreateReport("rptInsuranceDiscrepanciesReport.rpt", idtResultTable);

                idlgUpdateProcessLog("Insurance Discrepancies report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }

        }

        private DataTable CreateInsuranceDiscrepanciesBatchReportDataset()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("PERSON_ID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("LAST_NAME", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("FIRST_NAME", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("MIDDLE_NAME", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("LEVEL_OF_COVERAGE_VALUE", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("PLAN", Type.GetType("System.String"));

            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);

            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(DataRow adrow)
        {
            DataRow dr = idtResultTable.NewRow();            
          
            dr["PERSON_ID"] = adrow["PERSON_ID"];
            dr["LAST_NAME"] = adrow["LAST_NAME"];
            dr["FIRST_NAME"] = adrow["FIRST_NAME"];
            dr["MIDDLE_NAME"] = adrow["MIDDLE_NAME"];
            dr["LEVEL_OF_COVERAGE_VALUE"] = adrow["LEVEL_OF_COVERAGE_VALUE"];
            dr["PLAN"] = adrow["PLAN"];

            idtResultTable.Rows.Add(dr);
        }
    }
}
