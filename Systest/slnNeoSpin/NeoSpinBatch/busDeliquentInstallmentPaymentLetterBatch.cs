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
    class busDeliquentInstallmentPaymentLetterBatch : busNeoSpinBatch
    {
        public busDeliquentInstallmentPaymentLetterBatch()
        {

        }

        DataTable ldtbHeaderWithInPaymentStatus;
        bool iblnGenerateReport;
        string lstrPSCFormattedForClosedAccount;
        DataTable idtResultTable = new DataTable();

        private Collection<busServicePurchaseHeader> _iclbServicePurchaseHeader;
        public Collection<busServicePurchaseHeader> iclbServicePurchaseHeader
        {
            get
            {
                return _iclbServicePurchaseHeader;
            }

            set
            {
                _iclbServicePurchaseHeader = value;
            }
        }
        public void CreateCorrespondenceForDeliquentInstallmentPayment()
        {
            istrProcessName = "Generating Deliquent Installment Payment Letter";

            //Load header with status in In Payment
            ldtbHeaderWithInPaymentStatus = busBase.Select<cdoServicePurchaseHeader>(new string[1] { "action_status_value" },
                                            new object[1] { busConstant.Service_Purchase_Action_Status_In_Payment }, null, null);
            busBase lbusBase = new busBase();
            _iclbServicePurchaseHeader = lbusBase.GetCollection<busServicePurchaseHeader>(ldtbHeaderWithInPaymentStatus, "icdoServicePurchaseHeader");
            idtResultTable = GetDataTable();            
            foreach (busServicePurchaseHeader lobjServicePurchase in _iclbServicePurchaseHeader)
            {
                iblnGenerateReport = false;
                if (lobjServicePurchase.icdoServicePurchaseHeader.payment_frequency_value != null)
                {
                    //Load Person Account to get Total PSC
                    if (lobjServicePurchase.ibusPrimaryServicePurchaseDetail == null)
                    {
                        lobjServicePurchase.LoadServicePurchaseDetail();
                    }

                    if (lobjServicePurchase.ibusPersonAccount == null)
                    {
                        lobjServicePurchase.LoadPersonAccount();
                    }

                    int lintMonthsToBeAddedToDeriveNextDueDate =
                        lobjServicePurchase.GetMonthsToBeAddedToDeriveNextDueDate(lobjServicePurchase.icdoServicePurchaseHeader.payment_frequency_value);

                    //Load the Amortization Schedule to get the last paid entry (To get Actual Payment Due Date)
                    if (lobjServicePurchase.iclbServicePurchaseAmortizationSchedule == null)
                        lobjServicePurchase.LoadAmortizationSchedule();

                    if ((lobjServicePurchase.iclbServicePurchaseAmortizationSchedule != null) && (lobjServicePurchase.iclbServicePurchaseAmortizationSchedule.Count > 0))
                    {
                        //Get the Last Paid Payment Allocation Entry
                        busServicePurchasePaymentAllocation lbusLastPaymentAllocation = lobjServicePurchase.GetLastPaidPaymentAllocation();

                        //Get the Amortization Schdule for the last payment allocation entry
                        busServicePurchaseAmortizationSchedule lbusSchedule =
                            lobjServicePurchase.GetAmortizationScheduleByPaymentAllocation(
                                lbusLastPaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_allocation_id);

                        //get last payment recieved date
                        DateTime ldtLastPaidDueDate = lbusSchedule.icdoServicePurchaseAmortizationSchedule.idtActualDueDate;

                        //Get Next Payment Due Date
                        //Add no of months to the last paid date as per payment frequency to get the First Dua Date                        
                        DateTime ldtFirstDueDate = ldtLastPaidDueDate.AddMonths(lintMonthsToBeAddedToDeriveNextDueDate + 1); //UAT PIR 733

                        //Add 1 month to the first due date to get the second due date
                        DateTime ldtSecondDueDate = ldtFirstDueDate.AddMonths(1);

                        ProcessAndGenerateLetterAndReport(lobjServicePurchase, ldtFirstDueDate, ldtSecondDueDate);

                        if (iblnGenerateReport)
                        {
                            //Get the Balance Due
                            decimal ldecBalanceDue = lbusSchedule.icdoServicePurchaseAmortizationSchedule.idecPayOffAmountActualValue;

                            //Fill datatable with Values.
                            FillDatatableProcessDelinquentPaymentLetter(lobjServicePurchase, ldtLastPaidDueDate, ldecBalanceDue);                            
                        }
                    }
                }
            }
            //Generate report
            if (idtResultTable.Rows.Count > 0)                
                GenerateReport();
        }

        private void ProcessAndGenerateLetterAndReport(busServicePurchaseHeader lobjServicePurchase, DateTime ldtFirstDueDate, DateTime ldtSecondDueDate)
        {
            //If firstDue date is greater than today s date then Last Installment is still pending.
            //otherwise installment is paid for the previous period and not for the last period installment
            if ((DateTime.Now >= ldtFirstDueDate) &&
                (DateTime.Now < ldtSecondDueDate) &&
                (lobjServicePurchase.icdoServicePurchaseHeader.delinquent_letter1_sent_flag != busConstant.Flag_Yes))
            {
                GenerateCorrespondenceForFirstDueDate(lobjServicePurchase);
                lobjServicePurchase.icdoServicePurchaseHeader.delinquent_letter1_sent_flag = busConstant.Flag_Yes;
                lobjServicePurchase.icdoServicePurchaseHeader.Update();

                iblnGenerateReport = true;
            }
            else if ((DateTime.Now >= ldtSecondDueDate) && (lobjServicePurchase.icdoServicePurchaseHeader.delinquent_letter2_sent_flag != busConstant.Flag_Yes))
            {
                decimal idecAdjustedPSC = 0.00M;
                decimal idecAdjustedVSC = 0.00M;
                lobjServicePurchase.PostServicePurchaseAdjstClosingOfContract(ref idecAdjustedPSC, ref idecAdjustedVSC);
                lobjServicePurchase.icdoServicePurchaseHeader.prorated_psc = lobjServicePurchase.icdoServicePurchaseHeader.prorated_psc + idecAdjustedPSC;
                lobjServicePurchase.icdoServicePurchaseHeader.prorated_vsc = lobjServicePurchase.icdoServicePurchaseHeader.prorated_vsc + idecAdjustedVSC;

                lobjServicePurchase.icdoServicePurchaseHeader.service_purchase_adjustment_fraction_psc = idecAdjustedPSC;
                lobjServicePurchase.icdoServicePurchaseHeader.service_purchase_adjustment_fraction_vsc  = idecAdjustedVSC;

                //Close the Service Purchase
                lobjServicePurchase.icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Closed;

                //Create contact ticket and insert contact ticket id in the service purchase header table
                lobjServicePurchase.icdoServicePurchaseHeader.contact_ticket_id = CreateContactTicket(lobjServicePurchase).icdoContactTicket.contact_ticket_id;
                lobjServicePurchase.icdoServicePurchaseHeader.delinquent_letter2_sent_flag = busConstant.Flag_Yes;
                lobjServicePurchase.icdoServicePurchaseHeader.Update();

                GenerateCorrespondenceForSecondDueDate(lobjServicePurchase);

                lstrPSCFormattedForClosedAccount = lobjServicePurchase.icdoServicePurchaseHeader.prorated_psc_formatted;
                iblnGenerateReport = true;
            }
            else
            {
                idlgUpdateProcessLog("No Correspondence Created", "INFO", istrProcessName);
            }
        }

        //generate correspondence if delay period is graeter than 30 days
        private void GenerateCorrespondenceForFirstDueDate(busServicePurchaseHeader aobjServicePurchase)
        {
            istrProcessName = "Generating Deliquent Installment Payment Letter For Exceeding First Due Date";
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjServicePurchase);

            idlgUpdateProcessLog("Creating Correspondence if delay period is greater than First Due Date for Purchase id :" + aobjServicePurchase.icdoServicePurchaseHeader.service_purchase_header_id, "INFO", istrProcessName);

            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("PUR-8300", aobjServicePurchase, lhstDummyTable);

            idlgUpdateProcessLog("Correspondence created successfully for First Due Date for Purchase id :" + aobjServicePurchase.icdoServicePurchaseHeader.service_purchase_header_id, "INFO", istrProcessName);
        }
        //generate correspondence if delay period is graeter than 60 days
        private void GenerateCorrespondenceForSecondDueDate(busServicePurchaseHeader aobjServicePurchase)
        {
            istrProcessName = "Generating Deliquent Installment Payment Letter For Exceeding Second Due Date";
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjServicePurchase);

            idlgUpdateProcessLog("Creating Correspondence if delay period is greater than Second Due Date for Purchase id :" + aobjServicePurchase.icdoServicePurchaseHeader.service_purchase_header_id, "INFO", istrProcessName);

            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("PUR-8301", aobjServicePurchase, lhstDummyTable);

            idlgUpdateProcessLog("Correspondence created successfully for Second Due Date for Purchase id :" + aobjServicePurchase.icdoServicePurchaseHeader.service_purchase_header_id, "INFO", istrProcessName);
        }


        //Create Contact ticket
        public busContactTicket CreateContactTicket(busServicePurchaseHeader abusServicePurchase)
        {
            istrProcessName = "Create contact ticket with status 'Open'.";
            busContactTicket lobjContactTicket = new busContactTicket();
            lobjContactTicket.icdoContactTicket = new cdoContactTicket();
            CreateContactTicket(abusServicePurchase.icdoServicePurchaseHeader.person_id, busConstant.ContactTicketTypeRetPurchases, lobjContactTicket.icdoContactTicket);
            return lobjContactTicket;
        }

        public DataTable FillDatatableProcessDelinquentPaymentLetter(busServicePurchaseHeader aobjServicePurchase,
            DateTime adtLastPaymentReceivedDate, decimal adecBalanceDue)
        {
            if (aobjServicePurchase.ibusPerson == null)
                aobjServicePurchase.LoadPerson();

            DataRow dr = idtResultTable.NewRow();
            dr["PERSLinkID"] = aobjServicePurchase.icdoServicePurchaseHeader.person_id;
            dr["FullName"] = aobjServicePurchase.ibusPerson.icdoPerson.FullName;
            dr["PurchaseID"] = aobjServicePurchase.icdoServicePurchaseHeader.service_purchase_header_id;
            dr["LastPaymentReceivedDate"] = adtLastPaymentReceivedDate;
            dr["BalanceDue"] = adecBalanceDue;
            dr["PSCForClosedAccounts"] = lstrPSCFormattedForClosedAccount;

            idtResultTable.Rows.Add(dr);
            return idtResultTable;
        }

        public void GenerateReport()
        {
            idlgUpdateProcessLog("Creating Deliquent Payments Reports", "INFO", istrProcessName);
            CreateReport("rptProcessDelinquentPaymentLetter.rpt", idtResultTable);
        }

        public DataTable GetDataTable()
        {
            DataTable ldtResultTable = new DataTable();
            DataColumn ldc1 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("FullName", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("PurchaseID", Type.GetType("System.Int32"));
            DataColumn ldc4 = new DataColumn("LastPaymentReceivedDate", Type.GetType("System.DateTime"));
            DataColumn ldc5 = new DataColumn("BalanceDue", Type.GetType("System.Decimal"));
            DataColumn ldc6 = new DataColumn("PSCForClosedAccounts", Type.GetType("System.String"));

            ldtResultTable.Columns.Add(ldc1);
            ldtResultTable.Columns.Add(ldc2);
            ldtResultTable.Columns.Add(ldc3);
            ldtResultTable.Columns.Add(ldc4);
            ldtResultTable.Columns.Add(ldc5);
            ldtResultTable.Columns.Add(ldc6);
            ldtResultTable.TableName = busConstant.ReportTableName;
            return ldtResultTable;
        }
    }
}
