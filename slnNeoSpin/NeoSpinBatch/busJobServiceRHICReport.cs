#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
#endregion

namespace NeoSpinBatch
{
    class busJobServiceRHICReport : busNeoSpinBatch
    {
        public busJobServiceRHICReport()
        { }

        DataTable idtResultTable = new DataTable();

        public void GenerateJobServiceRHICReport()
        {
            istrProcessName = "Job Service RHIC Report";

            DataTable ldtList = busBase.Select("cdoIbsHeader.rptJobServiceRHIC", new object[0] { });
            idtResultTable = CreateNewDataTable();

            busBase lobjBase = new busBase();
            Collection<busIbsDetail> lclbIbsDetail = lobjBase.GetCollection<busIbsDetail>(ldtList, "icdoIbsDetail");
            foreach (busIbsDetail lobjIbsDetail in lclbIbsDetail)
            {
                lobjIbsDetail.LoadPerson();

                AddToNewDataRow(lobjIbsDetail);
            }
            if (idtResultTable.Rows.Count > 0)
            {
                //create report for JobService Rhic
                CreateReport("rptJobServiceRHICReport.rpt", idtResultTable);

                idlgUpdateProcessLog("Job Service RHIC Report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
        }

        public DataTable CreateNewDataTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("FirstName", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("LastName", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("RHICAmount", Type.GetType("System.Decimal"));
           
            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(busIbsDetail aobjIbsDetail)
        {
            DataRow dr = idtResultTable.NewRow();

            dr["PERSLinkID"] = aobjIbsDetail.ibusPerson.icdoPerson.person_id;
            dr["FirstName"] = aobjIbsDetail.ibusPerson.icdoPerson.first_name;
            dr["LastName"] = aobjIbsDetail.ibusPerson.icdoPerson.last_name;
            dr["RHICAmount"] = aobjIbsDetail.icdoIbsDetail.js_rhic_amount;

            idtResultTable.Rows.Add(dr);
        }
    }
}
