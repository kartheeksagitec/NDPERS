#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    class busInsufficientPurchasePaymentReportBatch : busNeoSpinBatch
    {
        public busInsufficientPurchasePaymentReportBatch()
        {
        }
        
        DataTable idtResultTable = new DataTable();
        public void GenerateInsufficientPurchasePayment()
        {
            istrProcessName = "Generating Insufficient Purchase Payment Report";

            Collection<busServicePurchaseHeader> lclbServicePurchase = new Collection<busServicePurchaseHeader>();            
            idtResultTable = CreateNewDataTable();
            DataTable ldtbGetHeaderWithPaymentStatus = busBase.Select("cdoServicePurchaseHeader.LoadAllInPaymentServicePurchase", new object[0] { });

            busBase lbusBase = new busBase();
            lclbServicePurchase = lbusBase.GetCollection<busServicePurchaseHeader>(ldtbGetHeaderWithPaymentStatus, "icdoServicePurchaseHeader");

            foreach (busServicePurchaseHeader lobjServicePurchaseHeader in lclbServicePurchase)
            {
                idlgUpdateProcessLog("Processing Purchase Id : " + lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id, "INFO", istrProcessName);

                if (lobjServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule == null)
                    lobjServicePurchaseHeader.LoadAmortizationSchedule();

                if (lobjServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule != null)
                {
                    busServicePurchasePaymentAllocation lbusLastPaymentAllocation = lobjServicePurchaseHeader.GetLastPaidPaymentAllocation();

                    busServicePurchaseAmortizationSchedule lbusSchedule =
                   lobjServicePurchaseHeader.GetAmortizationScheduleByPaymentAllocation(
                       lbusLastPaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_allocation_id);

                    if (lbusSchedule.icdoServicePurchaseAmortizationSchedule.payment_amount < lbusSchedule.icdoServicePurchaseAmortizationSchedule.interest_in_payment_amount)
                    {
                        AddToNewDataRow(lobjServicePurchaseHeader, lbusSchedule);
                    }
                }
            }

            if (idtResultTable.Rows.Count > 0)
            {
                //create report for Insufficient report details
                CreateReport("rptInsufficientPurchasePayment.rpt", idtResultTable);

                idlgUpdateProcessLog("Insufficient Purchase Payment report generated succesfully", "INFO", istrProcessName);
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
            DataColumn ldc5 = new DataColumn("PrincipleDue", Type.GetType("System.Decimal"));
            DataColumn ldc6 = new DataColumn("InterestDue", Type.GetType("System.Decimal"));
            DataColumn ldc7 = new DataColumn("PaymentAmount", Type.GetType("System.Decimal"));
            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);
            ldtbReportTable.Columns.Add(ldc7);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(busServicePurchaseHeader aobjServicePurchase, busServicePurchaseAmortizationSchedule lbusSchedule)
        {
            if (aobjServicePurchase.ibusPerson == null)
                aobjServicePurchase.LoadPerson();

            DataRow dr = idtResultTable.NewRow();
            dr["FullName"] = aobjServicePurchase.ibusPerson.icdoPerson.FullName;
            dr["PERSLinkID"] = aobjServicePurchase.icdoServicePurchaseHeader.person_id;
            dr["PurchaseID"] = aobjServicePurchase.icdoServicePurchaseHeader.service_purchase_header_id;
            dr["LastPaymentRecievedDate"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.payment_date;
            dr["PrincipleDue"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.principle_in_payment_amount;
            dr["InterestDue"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.interest_in_payment_amount;
            dr["PaymentAmount"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.payment_amount;
            idtResultTable.Rows.Add(dr);
        }
    }
}
