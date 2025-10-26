using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;

namespace NeoSpinBatch
{
    public class busSeminarPaymentAllocationBatch : busNeoSpinBatch
    {
        public void AllocateSeminarPayments()
        {
            istrProcessName = "Seminar Payment Allocation Batch";

            idlgUpdateProcessLog("Loading All unpaid Attendees", "INFO", istrProcessName);
            DataTable ldtbList = busNeoSpinBase.Select("cdoContactTicket.LoadAllUnpaidFeeAttendees", new object[0] { });
            foreach (DataRow ldrRow in ldtbList.Rows)
            {
                busSeminarAttendeeDetail lbusSemAttendeeDetail = new busSeminarAttendeeDetail { icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail() };
                lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.LoadData(ldrRow);
                lbusSemAttendeeDetail.ibusSeminarSchedule = new busSeminarSchedule { icdoSeminarSchedule = new cdoSeminarSchedule() };
                lbusSemAttendeeDetail.ibusSeminarSchedule.icdoSeminarSchedule.LoadData(ldrRow);

                idlgUpdateProcessLog("Processing PERSLink ID : " + lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.person_id, "INFO", istrProcessName);

                //Check any partial payment allocation made for that attendee
                DataTable ldtbPaymentAllocList = busBase.Select<cdoSeminarAttendeePaymentAllocation>(
                   new string[1] { "SEMINAR_ATTENDEE_DETAIL_ID" },
                   new object[1]
                    {
                        lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.seminar_attendee_detail_id
                    }, null, null);

                decimal ldecPartialPaidAmount = 0.00M;
                foreach (DataRow ldrPaymentRow in ldtbPaymentAllocList.Rows)
                {
                    ldecPartialPaidAmount += Convert.ToDecimal(ldrPaymentRow["applied_amount"]);
                }

                decimal ldecPendingPaymentAmount = lbusSemAttendeeDetail.ibusSeminarSchedule.icdoSeminarSchedule.attendee_fee - ldecPartialPaidAmount;

                DataTable ldtbRemittanceList = busNeoSpinBase.Select("cdoRemittance.LoadAppliedSeminarRemittances", new object[1] { lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.person_id });
                foreach (DataRow ldrRemRow in ldtbRemittanceList.Rows)
                {
                    busRemittance lbusRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
                    lbusRemittance.icdoRemittance.LoadData(ldrRemRow);
                    decimal ldecToBeAllocatedAmount = busEmployerReportHelper.GetRemittanceAvailableAmount(lbusRemittance.icdoRemittance.remittance_id);
                    if (ldecToBeAllocatedAmount > 0)
                    {
                        if (ldecToBeAllocatedAmount > ldecPendingPaymentAmount)
                            ldecToBeAllocatedAmount = ldecPendingPaymentAmount;

                        AllocateSeminarPayment(lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.seminar_attendee_detail_id,
                                                lbusRemittance.icdoRemittance.remittance_id, ldecToBeAllocatedAmount);

                        GenerateGL(lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.person_id,
                                    lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.seminar_attendee_detail_id, ldecToBeAllocatedAmount);

                        ldecPendingPaymentAmount -= ldecToBeAllocatedAmount;
                        if (ldecPendingPaymentAmount <= 0)
                            break;
                    }
                }

                if (ldecPendingPaymentAmount == 0)
                {
                    lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.attendee_fee_paid_flag = busConstant.Flag_Yes;
                    lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.Update();

                    if (lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.payment_method_value != busConstant.PaymentMethodIDB)
                    {
                        //BR -11 - Addendum - 119 - Publish message to messageboard
                        if (lbusSemAttendeeDetail.ibusSeminarSchedule == null)
                            lbusSemAttendeeDetail.LoadSeminarSchedule();
                        busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your payment has been received and your seminar registration for {0} has been confirmed.",
                                                            lbusSemAttendeeDetail.ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1.ToString("MM/dd/yyyy")),
                                                            busConstant.WSS_MessageBoard_Priority_High,
                                                            lbusSemAttendeeDetail.icdoSeminarAttendeeDetail.person_id);
                    }
                }
            }
        }

        private void AllocateSeminarPayment(int aintSeminarAttendeeDetailID, int aintRemittanceID, decimal adecPaymentAmount)
        {
            cdoSeminarAttendeePaymentAllocation lcdoSemPayAllocation = new cdoSeminarAttendeePaymentAllocation();
            lcdoSemPayAllocation.seminar_attendee_detail_id = aintSeminarAttendeeDetailID;
            lcdoSemPayAllocation.remittance_id = aintRemittanceID;
            lcdoSemPayAllocation.applied_amount = adecPaymentAmount;
            lcdoSemPayAllocation.payment_date = iobjSystemManagement.icdoSystemManagement.batch_date;
            lcdoSemPayAllocation.Insert();
        }

        private void GenerateGL(int aintPersonID, int aintSeminarAttendeeDetailID, decimal adecAmount)//PIR 20232 ?code
        {
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = busConstant.PlanIdMain;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeSeminar;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeAllocation;
            lcdoAcccountReference.from_item_type_value = busConstant.FromItemTypeSeminar;

            busGLHelper.GenerateGL(lcdoAcccountReference, aintPersonID, 0,
                                               aintSeminarAttendeeDetailID,
                                               adecAmount,
                                               iobjSystemManagement.icdoSystemManagement.batch_date,
                                               iobjSystemManagement.icdoSystemManagement.batch_date, iobjPassInfo);
        }
    }
}
