#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
#endregion

namespace NeoSpinBatch
{
    class busTFFRTransferReport : busNeoSpinBatch
    {
        public busTFFRTransferReport()
        { }

        DataTable idtResultTable = new DataTable();        

        public void GenerateTFFRTransferReport()
        {
            istrProcessName = "TFFR Transfer Report";

            DataTable ldtResultTable = busBase.Select("cdoBenefitApplication.rptTFFRTransferReport", new object[0] { });
            idtResultTable = CreateNewDataTable();
            
            foreach (DataRow dr in ldtResultTable.Rows)
            {
                AddToNewDataRow(dr);
            }

            if (idtResultTable.Rows.Count > 0)
            {
                //create report for TFFR Transfer
                CreateReport("rptTFFRTransferReport.rpt", idtResultTable);

                idlgUpdateProcessLog("TFFR Transfer Report generated succesfully", "INFO", istrProcessName);
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
            DataColumn ldc2 = new DataColumn("SSN", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("LastName", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("FirstName", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("EEPreTaxAmount", Type.GetType("System.Decimal"));
            DataColumn ldc6 = new DataColumn("EEPostTaxAmount", Type.GetType("System.Decimal"));
            DataColumn ldc7 = new DataColumn("EEEmpPickupAmount", Type.GetType("System.Decimal"));
            DataColumn ldc8 = new DataColumn("EEInterestAmount", Type.GetType("System.Decimal"));
            DataColumn ldc9 = new DataColumn("ERPreTaxAmount", Type.GetType("System.Decimal"));
            DataColumn ldc10 = new DataColumn("ERInterestAmount", Type.GetType("System.Decimal"));
            DataColumn ldc11 = new DataColumn("TotalTransferAmount", Type.GetType("System.Decimal"));
            DataColumn ldc12 = new DataColumn("PSC", Type.GetType("System.Decimal"));
            DataColumn ldc13 = new DataColumn("CurrentFAS", Type.GetType("System.Decimal"));
            DataColumn ldc14 = new DataColumn("PlanParticipationStartDate", Type.GetType("System.DateTime"));
            DataColumn ldc15 = new DataColumn("NumberofMissedMonths", Type.GetType("System.Int32"));
            DataColumn ldc16 = new DataColumn("EmploymentStartDate", Type.GetType("System.DateTime"));
            DataColumn ldc17 = new DataColumn("EmploymentEndDate", Type.GetType("System.DateTime"));
            DataColumn ldc18 = new DataColumn("OrgCodeID_OrgName", Type.GetType("System.String"));
            DataColumn ldc19 = new DataColumn("EffectiveDate", Type.GetType("System.DateTime"));
            DataColumn ldc20 = new DataColumn("SalaryAmount", Type.GetType("System.Decimal"));
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
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(DataRow adrResult)
        {
            DataRow dr = idtResultTable.NewRow();
            
            dr["PERSLinkID"] = adrResult["PERSLinkID"];
            dr["SSN"] = adrResult["SSN"];
            dr["LastName"] = adrResult["LastName"];
            dr["FirstName"] = adrResult["FirstName"];
            dr["EEPreTaxAmount"] = adrResult["EEPreTaxAmount"];
            dr["EEPostTaxAmount"] = adrResult["EEPostTaxAmount"];
            dr["EEEmpPickupAmount"] = adrResult["EEEmpPickupAmount"];
            dr["EEInterestAmount"] = adrResult["EEInterestAmount"];
            dr["ERPreTaxAmount"] = adrResult["ERPreTaxAmount"];
            dr["ERInterestAmount"] = adrResult["ERInterestAmount"];

            busBenefitRefundCalculation lobjBenefitRefundCalculation = new busBenefitRefundCalculation();
            lobjBenefitRefundCalculation.FindBenefitRefundCalculation(Convert.ToInt32(adrResult["benefit_calculation_id"]));
            lobjBenefitRefundCalculation.CalculateTotalAmountForTransferOptions();
            dr["TotalTransferAmount"] = lobjBenefitRefundCalculation.icdoBenefitRefundCalculation.total_transfer_amount;

            busPersonAccount lobjPersonAccount = new busPersonAccount();
            lobjPersonAccount.FindPersonAccount(Convert.ToInt32(adrResult["person_account_id"]));
            lobjPersonAccount.LoadTotalPSC();
            dr["PSC"] = lobjPersonAccount.idecTotalPSC_Rounded;

            //need to check for current FAS            
            dr["CurrentFAS"] = 0;//need to put the logic for FAS
            dr["PlanParticipationStartDate"] = adrResult["PlanParticipationStartDate"];
            dr["NumberofMissedMonths"] = adrResult["NumberofMissedMonths"];
            dr["EmploymentStartDate"] = adrResult["EmploymentStartDate"];
            dr["EmploymentEndDate"] = adrResult["EmploymentEndDate"];
            dr["OrgCodeID_OrgName"] = adrResult["OrgCodeID_OrgName"];     
            dr["EffectiveDate"] = adrResult["EffectiveDate"];
            dr["SalaryAmount"] = adrResult["SalaryAmount"];
            idtResultTable.Rows.Add(dr);            
        }
    }
}
