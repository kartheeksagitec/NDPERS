#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    class busFinalPurchaseInstallmentDueReport : busNeoSpinBatch
    {
        public busFinalPurchaseInstallmentDueReport()
        {
        }

        DataTable idtResultTable = new DataTable();
        public void GenerateFinalPurchaseInstallmentDue()
        {
            istrProcessName = "Generating Final Purchase Installment Due Report";

            Collection<busServicePurchaseHeader> lclbServicePurchase = new Collection<busServicePurchaseHeader>();            
            idtResultTable = CreateNewDataTable();
            DataTable ldtbGetHeaderWithPaymentStatus = busBase.Select("cdoServicePurchaseHeader.LoadAllInPaymentServicePurchase", new object[0] { });

            busBase lbusBase = new busBase();
            lclbServicePurchase = lbusBase.GetCollection<busServicePurchaseHeader>(ldtbGetHeaderWithPaymentStatus, "icdoServicePurchaseHeader");

            foreach (busServicePurchaseHeader lobjServicePurchaseHeader in lclbServicePurchase)
            {
                if (lobjServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule == null)
                    lobjServicePurchaseHeader.LoadAmortizationSchedule();

                if (lobjServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule != null)
                {
                    if (lobjServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule.Count > 0)
                    {
                        //Get the Last Schdule Entry
                        busServicePurchaseAmortizationSchedule lbusSchedule =
                            lobjServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule[lobjServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule.Count - 1];

                        if (lbusSchedule.icdoServicePurchaseAmortizationSchedule.expected_payment_amount > lobjServicePurchaseHeader.idecPayOffAmount)
                        {
                            //Get the Last Payment Schdule
                            busServicePurchasePaymentAllocation lbusLastPaymentAllocation = lobjServicePurchaseHeader.GetLastPaidPaymentAllocation();

                            busServicePurchaseAmortizationSchedule lbusLastPaymentSchedule = lobjServicePurchaseHeader.GetAmortizationScheduleByPaymentAllocation(
                                lbusLastPaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_allocation_id);

                            idlgUpdateProcessLog("Processing Purchase Id : " + lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id, "INFO", istrProcessName);
                            AddToNewDataRow(lobjServicePurchaseHeader, lbusSchedule, lbusLastPaymentSchedule);
                        }
                    }
                }
            }

            if (idtResultTable.Rows.Count > 0)
            {
                //create report for Insufficient report details
                CreateReport("rptFinalPurchaseInstallmentDue.rpt", idtResultTable);

                idlgUpdateProcessLog("Final Purchase Installment Due Report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
        }
        public DataTable CreateNewDataTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("FullName", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn ldc3 = new DataColumn("PurchaseID", Type.GetType("System.Int32"));
            DataColumn ldc4 = new DataColumn("LastPaymentRecievedDate", Type.GetType("System.DateTime"));
            DataColumn ldc5 = new DataColumn("LastPaymentAmount", Type.GetType("System.Decimal"));
            DataColumn ldc6 = new DataColumn("ExpectedInstallmentAmount", Type.GetType("System.Decimal"));
            DataColumn ldc7 = new DataColumn("BalanceDue", Type.GetType("System.Decimal"));
            DataColumn ldc8 = new DataColumn("Limit415Flag", Type.GetType("System.String"));
            ldtbReportTable.Columns.Add(ldc1);
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

        public void AddToNewDataRow(busServicePurchaseHeader aobjServicePurchase, busServicePurchaseAmortizationSchedule abusSchedule,
            busServicePurchaseAmortizationSchedule abusLastPaymentSchedule)
        {
            if (aobjServicePurchase.ibusPerson == null)
                aobjServicePurchase.LoadPerson();

            DataRow dr = idtResultTable.NewRow();
            dr["FullName"] = aobjServicePurchase.ibusPerson.icdoPerson.FullName;
            dr["PERSLinkID"] = aobjServicePurchase.icdoServicePurchaseHeader.person_id;
            dr["PurchaseID"] = aobjServicePurchase.icdoServicePurchaseHeader.service_purchase_header_id;
            dr["LastPaymentRecievedDate"] = abusLastPaymentSchedule.icdoServicePurchaseAmortizationSchedule.payment_date;
            dr["LastPaymentAmount"] = abusLastPaymentSchedule.icdoServicePurchaseAmortizationSchedule.principle_in_payment_amount;
            dr["ExpectedInstallmentAmount"] = abusSchedule.icdoServicePurchaseAmortizationSchedule.expected_payment_amount;
            dr["BalanceDue"] = aobjServicePurchase.idecPayOffAmount;
            dr["Limit415Flag"] = aobjServicePurchase.icdoServicePurchaseHeader.limit_415_flag;
            idtResultTable.Rows.Add(dr);
        }
    }
}
