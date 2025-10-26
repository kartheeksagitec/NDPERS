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

#endregion

namespace NeoSpinBatch
{
    class busNonPeopleSoftFlexCompBatchReport : busNeoSpinBatch
    {
        DataTable ldtbDeferredCompAgentSeminar = new DataTable();
        public busNonPeopleSoftFlexCompBatchReport()
        {

        }

        DataTable idtResultTable = new DataTable();
        public void GenerateReportForNonPeopleSoftEmployeesInFlexComp()
        {
            istrProcessName = "Flex Comp Change Report Batch";
            idlgUpdateProcessLog("Getting Flex Comp Change Employee details", "INFO", istrProcessName);
            DateTime ldtBatchRunDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            DataTable ldtbList = busNeoSpinBase.Select("cdoPersonAccountFlexComp.rptNonPeopleSoftEmployeesReport", new object[1] { ldtBatchRunDate });
            idtResultTable = CreateNewDataTable();
            foreach (DataRow dr in ldtbList.Rows)
            {
                AddToNewDataRow(dr);
            }
            try
                {
                    if (idtResultTable.Rows.Count > 0)
                    {
                        //create report for JobService RHIC
                        CreateReport("rptFlexCompChangeReport.rpt", idtResultTable);
                        idlgUpdateProcessLog("Flex Comp Change Batch Report generated successfully", "INFO", istrProcessName);
                        idlgUpdateProcessLog("Resetting Flex Comp Change Batch Report fields on " + DateTime.Now, "INFO", istrProcessName);
                        //reset the NPSP column in person account table
                       ResetPersonAccountNPSPFields(ldtbList);
                    }
                    else
                    {
                        idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                    }
                }
                catch (Exception _exc)
                {
                    idlgUpdateProcessLog("ERROR:" + _exc.Message, "INFO", istrProcessName);
                }
        }

        private void ResetPersonAccountNPSPFields(DataTable adtTable)
        {
            foreach (DataRow dr in adtTable.Rows)
            {
                busPersonAccount lobjPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lobjPersonAccount.icdoPersonAccount.LoadData(dr);
                lobjPersonAccount.icdoPersonAccount.npsp_flexcomp_change_date = DateTime.MinValue;
                lobjPersonAccount.icdoPersonAccount.npsp_flexcomp_flag = busConstant.Flag_No;
                lobjPersonAccount.icdoPersonAccount.Update();
            } 
        }

        private DataTable CreateNewDataTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("PERSLINKID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("PEOPLESOFTID", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("NAME", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("ORGCODEID", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("DATEOFCHANGE", Type.GetType("System.DateTime"));
            DataColumn ldc6 = new DataColumn("ORGGROUPVALUE", Type.GetType("System.String"));

            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);

            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        private void AddToNewDataRow(DataRow adrow)
        {
            DataRow dr = idtResultTable.NewRow();

            dr["PERSLINKID"] = adrow["PERSLINKID"];
            dr["PEOPLESOFTID"] = adrow["PEOPLESOFTID"];
            dr["NAME"] = adrow["NAME"];
            dr["ORGCODEID"] = adrow["ORGCODEID"];
            dr["DATEOFCHANGE"] = adrow["DATEOFCHANGE"];
            dr["ORGGROUPVALUE"] = adrow["ORGGROUPVALUE"];

            idtResultTable.Rows.Add(dr);
        }
    }
}
