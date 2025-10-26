#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

#endregion


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPaymentScheduleGen : busExtendBase
    {
        public busPaymentScheduleGen()
        {

        }

        private cdoPaymentSchedule _icdoPaymentSchedule;
        public cdoPaymentSchedule icdoPaymentSchedule
        {
            get
            {
                return _icdoPaymentSchedule;
            }
            set
            {
                _icdoPaymentSchedule = value;
            }
        }

        public bool FindPaymentSchedule(int Aintpaymentscheduleid)
        {
            bool lblnResult = false;
            if (_icdoPaymentSchedule == null)
            {
                _icdoPaymentSchedule = new cdoPaymentSchedule();
            }
            if (_icdoPaymentSchedule.SelectRow(new object[1] { Aintpaymentscheduleid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        public Collection<busPaymentScheduleStep> iclbPaymentScheduleStep { get; set; }
        public Collection<busPaymentStepRef> iclbPaymentSteps { get; set; }
        //Load all payment steps which are active for the payment schedule
        public void LoadPaymentScheduleSteps()
        {
            iclbPaymentScheduleStep = new Collection<busPaymentScheduleStep>();
            DataTable ldtbScheduleSteps = Select("cdoPaymentSchedule.LoadPaymentScheduleSteps", new object[1] { icdoPaymentSchedule.payment_schedule_id });

            foreach (DataRow drStep in ldtbScheduleSteps.Rows)
            {
                busPaymentScheduleStep lobjPaymentScheduleStep = new busPaymentScheduleStep { icdoPaymentScheduleStep = new cdoPaymentScheduleStep() };
                lobjPaymentScheduleStep.ibusPaymentStep = new busPaymentStepRef { icdoPaymentStepRef = new cdoPaymentStepRef() };
                lobjPaymentScheduleStep.icdoPaymentScheduleStep.LoadData(drStep);
                lobjPaymentScheduleStep.ibusPaymentStep.icdoPaymentStepRef.LoadData(drStep);
                lobjPaymentScheduleStep.icdoPaymentScheduleStep.batch_schedule_id
                          = icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeAdhoc ?
                          busConstant.AdhocPaymentBatchScheduleID : busConstant.MonthlyPaymentBatchScheduleID;
                iclbPaymentScheduleStep.Add(lobjPaymentScheduleStep);
            }
        }
        //Load All the Payment Steps
        public void LoadPaymentSteps()
        {
            DataTable ldtbPaymentSteps = iobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_step_ref", null);
            iclbPaymentSteps = GetCollection<busPaymentStepRef>(ldtbPaymentSteps, "icdoPaymentStepRef");
        }
        //property to reload the steps
        public Collection<busPaymentStepRef> iclbPaymentStepRefReloaded { get; set; }
        //Reload or Refresh All the Payment Steps
        public void ReLoadPaymentSteps()
        {
            DataTable ldtbPaymentSteps = Select("cdoPaymentSchedule.ReloadPaymentStepRef",
                new object[2] { icdoPaymentSchedule.payment_schedule_id, icdoPaymentSchedule.schedule_type_value });
            iclbPaymentStepRefReloaded = GetCollection<busPaymentStepRef>(ldtbPaymentSteps, "icdoPaymentStepRef");
        }
        //Create payment steps which are active for the payment schedule
        public void CreatePaymentSteps(bool ablnApproved)
        { 
            //if approve button is clicked from the screen refresh the payment step ref collection, so that currently active steps will inserted into payment schedule steps
            if(ablnApproved && iclbPaymentStepRefReloaded==null)
                ReLoadPaymentSteps();
            // On creation of payment schedule steps ,take all the active steps from cache and insert into  payment schedule steps
             if(!ablnApproved && iclbPaymentSteps == null)
                LoadPaymentSteps();

             Collection<busPaymentStepRef> lclbPaymentStepRef = ablnApproved ? iclbPaymentStepRefReloaded : iclbPaymentSteps;
            if (lclbPaymentStepRef.Count > 0)
            {
                foreach (busPaymentStepRef lobjPaymentStep in lclbPaymentStepRef.Where(o => o.icdoPaymentStepRef.active_flag == busConstant.Flag_Yes))
                {
                    if ((icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeMonthly && lobjPaymentStep.icdoPaymentStepRef.schedule_type_value == busConstant.PaymentScheduleScheduleTypeMonthly)
                        || (icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeAdhoc && lobjPaymentStep.icdoPaymentStepRef.schedule_type_value == busConstant.PaymentScheduleScheduleTypeAdhoc))
                    {
                        busPaymentScheduleStep lobjPaymentScheduleStep = new busPaymentScheduleStep { icdoPaymentScheduleStep = new cdoPaymentScheduleStep() };
                        lobjPaymentScheduleStep.icdoPaymentScheduleStep.payment_schedule_id = icdoPaymentSchedule.payment_schedule_id;
                        lobjPaymentScheduleStep.icdoPaymentScheduleStep.payment_step_id = lobjPaymentStep.icdoPaymentStepRef.payment_step_id;
                        lobjPaymentScheduleStep.icdoPaymentScheduleStep.status_value = busConstant.PaymentScheduleStepStatusPending;                      
                        lobjPaymentScheduleStep.icdoPaymentScheduleStep.Insert();
                    }
                }
            }
        }

        //This method is used to set the visibility for Run Trial Reports Button
        public bool IsRunTrialReportsbuttonVisible()
        {
            if ((icdoPaymentSchedule.status_value == busConstant.PaymentScheduleStatusValid &&
                (icdoPaymentSchedule.action_status_value == busConstant.PaymentScheduleActionStatusPending ||
                 icdoPaymentSchedule.action_status_value == busConstant.PaymentScheduleActionStatusTrialExecuted ||
                 icdoPaymentSchedule.action_status_value == busConstant.PaymentScheduleActionStatusFailed)) ||
                (icdoPaymentSchedule.status_value == busConstant.PaymentScheduleStatusReview &&
                (icdoPaymentSchedule.action_status_value == busConstant.PaymentScheduleActionStatusReadyforFinal ||
                icdoPaymentSchedule.action_status_value == busConstant.PaymentScheduleActionStatusTrialExecuted ||
                icdoPaymentSchedule.action_status_value == busConstant.PaymentScheduleActionStatusFailed)))
            {
                return true;
            }
            return false;
        }
    }
}