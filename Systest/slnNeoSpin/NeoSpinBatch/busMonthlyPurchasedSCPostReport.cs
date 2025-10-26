#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;

#endregion


namespace NeoSpinBatch
{
    class busMonthlyPurchasedSCPostReport : busNeoSpinBatch
    {
        public busMonthlyPurchasedSCPostReport()
        {
        }

        DataTable idtResultTable = new DataTable();
        public void GenerateMonthlyPurchasedSCPostingReport()
        {
            istrProcessName = "Generating Monthly Purchased Service Credit Posting Report";

            Collection<busServicePurchaseHeader> lclbServicePurchase = new Collection<busServicePurchaseHeader>();
            idtResultTable = CreateMonthlyPurchaseSCReportDataSet();

            DataTable ldtbPurchases = busBase.Select("cdoServicePurchaseHeader.LoadAllInPaymentClosedPaidInFullPurchases", new object[0] { });
            busBase lbusBase = new busBase();
            lclbServicePurchase = lbusBase.GetCollection<busServicePurchaseHeader>(ldtbPurchases, "icdoServicePurchaseHeader");

            foreach (busServicePurchaseHeader lobjServicePurchaseHeader in lclbServicePurchase)
            {
                busServicePurchasePaymentAllocation lbusLastPaymentAllocation = lobjServicePurchaseHeader.GetLastPaidPaymentAllocation();

                //In Payment Status, Include it in Report. 
                //For Paid In Full and Closed Entries, Check the Last payment date with previous month Date
                if (lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment)
                {
                    //Fill new dataset
                    AddToNewDataRow(lobjServicePurchaseHeader, lbusLastPaymentAllocation);
                }
                else
                {
                    if (lobjServicePurchaseHeader.iclbAllocatedRemittance.Count > 0)
                    {
                        //This Batch runs on First of the Month and we need to get the report based on the previous month
                        DateTime ldtCurrentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);

                        if ((lbusLastPaymentAllocation.icdoServicePurchasePaymentAllocation.payment_date.Month == ldtCurrentDateTime.Month)
                            && (lbusLastPaymentAllocation.icdoServicePurchasePaymentAllocation.payment_date.Year == ldtCurrentDateTime.Year))
                        {
                            AddToNewDataRow(lobjServicePurchaseHeader, lbusLastPaymentAllocation);
                        }
                    }
                }
            }

            if (idtResultTable.Rows.Count > 0)
            {
                //create report for Insufficient report details
                CreateReport("rptMonthlyPurchasedServiceCreditPosting.rpt", idtResultTable);

                idlgUpdateProcessLog("Generating Monthly Purchased Service Credit Posting report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
        }

        public DataTable CreateMonthlyPurchaseSCReportDataSet()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("Service_Purchase_Header_ID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("PERSLink_ID", Type.GetType("System.Int32"));
            DataColumn ldc3 = new DataColumn("FullName", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("Beginning_Principal_Balance", Type.GetType("System.Decimal"));
            DataColumn ldc5 = new DataColumn("Payment_Amount", Type.GetType("System.Decimal"));
            DataColumn ldc6 = new DataColumn("Principal", Type.GetType("System.Decimal"));
            DataColumn ldc7 = new DataColumn("Ending_Principal_Balance", Type.GetType("System.Decimal"));
            DataColumn ldc8 = new DataColumn("ProratedVSC", Type.GetType("System.String"));
            DataColumn ldc9 = new DataColumn("ProratedPSC", Type.GetType("System.String"));
            DataColumn ldc10 = new DataColumn("Interest", Type.GetType("System.Decimal"));
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
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(busServicePurchaseHeader abusServicePurchaseHeader, busServicePurchasePaymentAllocation abusLastPaymentAllocation)
        {
            idlgUpdateProcessLog("Processing Purchase Id : " + abusServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id, "INFO", istrProcessName);
            //Load Person details
            if (abusServicePurchaseHeader.ibusPerson == null)
                abusServicePurchaseHeader.LoadPerson();

            if (abusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail == null)
                abusServicePurchaseHeader.LoadServicePurchaseDetail();

            //Load the Amotization Schedule
            if (abusServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule == null)
                abusServicePurchaseHeader.LoadAmortizationSchedule();

            if (abusServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule != null)
            {
                busServicePurchaseAmortizationSchedule lbusSchedule =
                    abusServicePurchaseHeader.GetAmortizationScheduleByPaymentAllocation(
                        abusLastPaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_allocation_id);

                DataRow dr = idtResultTable.NewRow();
                dr["FullName"] = abusServicePurchaseHeader.ibusPerson.icdoPerson.FullName;
                dr["PERSLink_ID"] = abusServicePurchaseHeader.icdoServicePurchaseHeader.person_id;
                dr["Service_Purchase_Header_ID"] = abusServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id;
                dr["Beginning_Principal_Balance"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.idecBeginningPrincipalBalance;
                dr["Payment_Amount"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.payment_amount;
                dr["Principal"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.principle_in_payment_amount;
                dr["Interest"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.interest_in_payment_amount;
                dr["Ending_Principal_Balance"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.principle_balance;
                dr["ProratedVSC"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.prorated_vsc_formatted;
                dr["ProratedPSC"] = lbusSchedule.icdoServicePurchaseAmortizationSchedule.prorated_psc_formatted;
                idtResultTable.Rows.Add(dr);
            }
        }
    }
}
