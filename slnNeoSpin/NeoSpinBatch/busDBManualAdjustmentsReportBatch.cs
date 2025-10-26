#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    class busDBManualAdjustmentsReportBatch : busNeoSpinBatch
    {
        public busDBManualAdjustmentsReportBatch()
        {
        }

        DataTable idtResultTable = new DataTable();
        public void GenerateDBManualAdjustmentReport()
        {
            istrProcessName = "Manual Adjustments Report";

            Collection<busPersonAccountRetirementContribution> lclbRetirementcontribution = new Collection<busPersonAccountRetirementContribution>();
            idtResultTable = CreateNewDataTable();
            DataTable ldtbAdjustments = busBase.Select("cdoPersonAccountRetirementContribution.rptManualAdjustmentsReport", new object[0] { });
            busBase lbusBase = new busBase();
            lclbRetirementcontribution = lbusBase.GetCollection<busPersonAccountRetirementContribution>(ldtbAdjustments, "icdoPersonAccountRetirementContribution");

            foreach (busPersonAccountRetirementContribution lobjAdjustments in lclbRetirementcontribution)
            {
                if (lobjAdjustments.ibusPersonAccountDBDBTransfer == null)
                    lobjAdjustments.ibusPersonAccountDBDBTransfer = new busPersonAccountRetirementDbDbTransfer();
                if (lobjAdjustments.ibusPersonAccountDBDBTransfer.FindPersonAccountRetirementDbDbTransfer(
                                                        lobjAdjustments.icdoPersonAccountRetirementContribution.subsystem_ref_id))
                {
                    lobjAdjustments.ibusPersonAccountDBDBTransfer.LoadOtherDetails();                                  
                    AddToNewDataRow(lobjAdjustments);   
                }                             
            }
            if (idtResultTable.Rows.Count > 0)
            {
                //create report for all adjust details between plan
                CreateReport("rptManualAdjustmentsReport.rpt", idtResultTable);

                idlgUpdateProcessLog("Manual Adjustments between DB Plans report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
        }
        public DataTable CreateNewDataTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("MemberName", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn ldc3 = new DataColumn("LastName", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("TransferFrom", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("TransferTo", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("PostTaxEEContribution", Type.GetType("System.Decimal"));
            DataColumn ldc7 = new DataColumn("PostTaxEESerPurContribution", Type.GetType("System.Decimal"));
            DataColumn ldc8 = new DataColumn("RHICEEContribution", Type.GetType("System.Decimal"));
            DataColumn ldc9 = new DataColumn("RHICEESerPurContribution", Type.GetType("System.Decimal"));
            DataColumn ldc10 = new DataColumn("CapitalGains", Type.GetType("System.Decimal"));
            DataColumn ldc17 = new DataColumn("ERVSC", Type.GetType("System.Decimal"));
            DataColumn ldc11 = new DataColumn("Interest", Type.GetType("System.Decimal"));
            DataColumn ldc12 = new DataColumn("PreTaxEEContribution", Type.GetType("System.Decimal"));
            DataColumn ldc13 = new DataColumn("PreTaxEEERPickup", Type.GetType("System.Decimal"));
            DataColumn ldc14 = new DataColumn("PreTaxEESerPurContribution", Type.GetType("System.Decimal"));
            DataColumn ldc15 = new DataColumn("RHICERContribution", Type.GetType("System.Decimal"));
            DataColumn ldc16 = new DataColumn("RHICERSerPurContribution", Type.GetType("System.Decimal"));
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
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(busPersonAccountRetirementContribution aobjAdjustments)
        {  
            DataRow dr = idtResultTable.NewRow();
            dr["MemberName"] = aobjAdjustments.ibusPersonAccountDBDBTransfer.ibusToPersonAccountRetirement.ibusPerson.icdoPerson.FullName;
            dr["PERSLinkID"] = aobjAdjustments.ibusPersonAccountDBDBTransfer.ibusToPersonAccountRetirement.icdoPersonAccount.person_id;
            dr["LastName"] = aobjAdjustments.ibusPersonAccountDBDBTransfer.ibusToPersonAccountRetirement.ibusPerson.icdoPerson.last_name;
            dr["TransferFrom"] = aobjAdjustments.ibusPersonAccountDBDBTransfer.ibusFromPersonAccountRetirement.ibusPlan.icdoPlan.plan_name;
            dr["TransferTo"] = aobjAdjustments.ibusPersonAccountDBDBTransfer.ibusToPersonAccountRetirement.ibusPlan.icdoPlan.plan_name;
            dr["PostTaxEEContribution"] = aobjAdjustments.icdoPersonAccountRetirementContribution.post_tax_ee_amount;
            dr["PostTaxEESerPurContribution"] = aobjAdjustments.icdoPersonAccountRetirementContribution.post_tax_ee_ser_pur_cont;
            dr["RHICEEContribution"] = aobjAdjustments.icdoPersonAccountRetirementContribution.ee_rhic_amount;
            dr["RHICEESerPurContribution"] = aobjAdjustments.icdoPersonAccountRetirementContribution.ee_rhic_ser_pur_cont;
            dr["CapitalGains"] = aobjAdjustments.ibusPersonAccountDBDBTransfer.ibusToPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain;
            dr["Interest"] = aobjAdjustments.icdoPersonAccountRetirementContribution.interest_amount;
            dr["PreTaxEEContribution"] = aobjAdjustments.icdoPersonAccountRetirementContribution.pre_tax_ee_amount;
            dr["PreTaxEEERPickup"] = aobjAdjustments.icdoPersonAccountRetirementContribution.ee_er_pickup_amount;
            dr["PreTaxEESerPurContribution"] = aobjAdjustments.icdoPersonAccountRetirementContribution.pre_tax_ee_ser_pur_cont;
            dr["RHICERContribution"] = aobjAdjustments.icdoPersonAccountRetirementContribution.er_rhic_amount;
            dr["RHICERSerPurContribution"] = aobjAdjustments.icdoPersonAccountRetirementContribution.er_rhic_ser_pur_cont;
            dr["ERVSC"] = aobjAdjustments.icdoPersonAccountRetirementContribution.er_vested_amount;
            idtResultTable.Rows.Add(dr);
        }
    }
}
