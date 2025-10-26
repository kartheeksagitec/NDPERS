#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;
using NeoSpin.DataObjects;
using System.Collections.Generic;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busDeposit : busExtendBase
    {
        private string _istrPersonName;
        public string istrPersonName
        {
            get { return _istrPersonName; }
            set { _istrPersonName = value; }
        }

        private string _istrOrgName;
        public string istrOrgName
        {
            get { return _istrOrgName; }
            set { _istrOrgName = value; }
        }

        private decimal _ldclTotalRemittanceAmount;
        public decimal ldclTotalRemittanceAmount
        {
            get { return _ldclTotalRemittanceAmount; }
            set { _ldclTotalRemittanceAmount = value; }
        }

        private Collection<busRemittance> _iclbRemittance;
        public Collection<busRemittance> iclbRemittance
        {
            get { return _iclbRemittance; }
            set { _iclbRemittance = value; }
        }

        // PIR 6905
        public decimal idecTotalRemittance { get; set; }
        public void LoadTotalRemittance(int aintDepositTapeId)
        {
            DataTable ldtbDeposit = Select("cdoDeposit.GetTotalRemittance", new object[1] { aintDepositTapeId });
            if (ldtbDeposit.Rows.Count > 0)
                idecTotalRemittance = Convert.ToDecimal(ldtbDeposit.Rows[0]["TOTAL_REMITTANCE"]);
            else
                idecTotalRemittance = 0;
        }

        public void LoadRemittances()
        {
            DataTable ldtbList = Select<cdoRemittance>(
                new string[1] { "deposit_id" },
                new object[1] { _icdoDeposit.deposit_id }, null, null);
            _iclbRemittance = GetCollection<busRemittance>(ldtbList, "icdoRemittance");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busRemittance)
            {
                busRemittance lobjRemittance = (busRemittance)aobjBus;

                lobjRemittance.ldclAppliedAmount = busEmployerReportHelper.GetRemittanceAllocatedAmount(lobjRemittance.icdoRemittance.remittance_id);
                lobjRemittance.ldclBalanceAmount = lobjRemittance.icdoRemittance.remittance_amount - lobjRemittance.ldclAppliedAmount - lobjRemittance.icdoRemittance.allocated_negative_deposit_amount;
                //If Refund amount is there, we need to subtract from Balance Amount
                if (!string.IsNullOrEmpty(lobjRemittance.icdoRemittance.refund_pend_by))
                    lobjRemittance.ldclBalanceAmount -= lobjRemittance.icdoRemittance.overridden_refund_amount > 0 ? lobjRemittance.icdoRemittance.overridden_refund_amount :
                                                        lobjRemittance.icdoRemittance.computed_refund_amount;
                lobjRemittance.ibusDepositTape = new busDepositTape();
                lobjRemittance.ibusDepositTape.icdoDepositTape = new cdoDepositTape();
                lobjRemittance.ibusDeposit = new busDeposit();
                lobjRemittance.ibusDeposit.FindDeposit(_icdoDeposit.deposit_id);        // To Avoid deleting Remittance in Deposit maintenance Screen.
                lobjRemittance.ibusDepositTape.icdoDepositTape.deposit_tape_id = _icdoDeposit.deposit_tape_id;
                lobjRemittance.LoadOrgCodeID();
                lobjRemittance.LoadPlan();
             
            }
        }

        private busDepositTape _ibusDepositTape;
        public busDepositTape ibusDepositTape
        {
            get { return _ibusDepositTape; }
            set { _ibusDepositTape = value; }
        }

        public void LoadDepositTape()
        {
            if (_ibusDepositTape == null)
            {
                _ibusDepositTape = new busDepositTape();
            }
            _ibusDepositTape.FindDepositTape(_icdoDeposit.deposit_tape_id);
        }

        public void LoadOtherDeposits()
        {
            DataTable ldtbOtherDeposits = Select("cdoDeposit.LoadOtherDeposits", new object[2] { _icdoDeposit.deposit_tape_id, _icdoDeposit.deposit_id });
            _ibusDepositTape.iclbDeposit = new Collection<busDeposit>();
            _ibusDepositTape.iclbDeposit = GetCollection<busDeposit>(ldtbOtherDeposits, "icdoDeposit");
            /*Org code and remittance amount is taken from the query
            foreach (busDeposit lobjDeposit in _ibusDepositTape.iclbDeposit)
            {
                lobjDeposit.LoadOrgCodeID();
                lobjDeposit.LoadRemittanceAmount();
            }*/
        }

        // PIR ID 106
        private decimal _ldclRemittanceAmount;
        public decimal ldclRemittanceAmount
        {
            get { return _ldclRemittanceAmount; }
            set { _ldclRemittanceAmount = value; }
        }

        //PIR ID 106
        public void LoadRemittanceAmount()
        {
            if (_iclbRemittance == null)
                LoadRemittances();
            foreach (busRemittance lobjRemittance in _iclbRemittance)
            {
                _ldclRemittanceAmount += lobjRemittance.icdoRemittance.remittance_amount;
            }
        }

        private decimal _ldclTotalDepositAmount;
        public decimal ldclTotalDepositAmount
        {
            get { return _ldclTotalDepositAmount; }
            set { _ldclTotalDepositAmount = value; }
        }

        public void LoadTotalsFromDetail()
        {
            busDepositTape lobjDepositTape = new busDepositTape();
            lobjDepositTape.FindDepositTape(_icdoDeposit.deposit_tape_id);
            if (_icdoDeposit.deposit_tape_id != 0)
            {
               lobjDepositTape.LoadDeposits();
                lobjDepositTape.LoadDepositsCountAndTotalAmount();
            }
           
            _ldclTotalDepositAmount = lobjDepositTape.icdoDepositTape.TotalDepositAmount;
        }

        public void LoadPersonOrgNameByID()
        {
            if (_icdoDeposit.person_id != 0)
            {
                //UAT - PIR 168
                LoadPerson();
                _istrPersonName = _ibusPerson.icdoPerson.FullName;
            }
            if (_icdoDeposit.org_id != 0)
            {
                _istrOrgName = busGlobalFunctions.GetOrgNameByOrgID(_icdoDeposit.org_id);
            }
        }

        public ArrayList btnNSF_Click()
        {
            ArrayList larrError = new ArrayList();

            _icdoDeposit.status_value = busConstant.DepositDetailStatusNonSufficientFund;
            _icdoDeposit.Update();

            //Generate the Reverse GL    
            if (_iclbRemittance == null)
            {
                LoadRemittances();
            }
            foreach (busRemittance lobjRemittance in _iclbRemittance)
            {
                cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
                lcdoAcccountReference.plan_id = lobjRemittance.icdoRemittance.plan_id;
                lcdoAcccountReference.source_type_value = busConstant.SourceTypeRemittance;
                lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeStatusTransition;
                lcdoAcccountReference.item_type_value = lobjRemittance.icdoRemittance.remittance_type_value;
                lcdoAcccountReference.status_transition_value = busConstant.StatusTransitionAppliedToNSF;

                busDepositTape lobjDepositTape = new busDepositTape();
                lobjDepositTape.FindDepositTape(_icdoDeposit.deposit_tape_id);

                larrError = busGLHelper.GenerateGL(lcdoAcccountReference,
                                             lobjRemittance.icdoRemittance.person_id,
                                             lobjRemittance.icdoRemittance.org_id,
                                             lobjRemittance.icdoRemittance.remittance_id,
                                             lobjRemittance.icdoRemittance.remittance_amount,
                                             DateTime.Now,
                                             DateTime.Now, iobjPassInfo);


                //Reversal of Service Purchase
                ReverseServicePurchase(lobjRemittance.icdoRemittance.remittance_id);
                //Negation of IBS payments
                NegateIBSPayments(lobjRemittance);
                //UCS - 079 Start -- Updating Recovery history amounts to zero
                //lobjRemittance.UpdateRecoveryHistory();
                //PIR 26043 -- Delete latest recovery history -- also included UpdateRecoveryHistory() in it
                lobjRemittance.DeleteRecoveryHistory();
                //UCS - 079 End
                //PIR 8583
                UpdateEmployerPayrollMonthlyStatement(lobjRemittance);
            }
            return larrError;
        }

        public ArrayList btnInvalidate_Click()
        {
            ArrayList larrError = new ArrayList();

            _icdoDeposit.status_value = busConstant.DepositDetailStatusInvalidated;
            _icdoDeposit.Update();

            //Generate the Reverse GL    
            if (_iclbRemittance == null)
            {
                LoadRemittances();
            }
            foreach (busRemittance lobjRemittance in _iclbRemittance)
            {
                cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
                lcdoAcccountReference.plan_id = lobjRemittance.icdoRemittance.plan_id;
                lcdoAcccountReference.source_type_value = busConstant.SourceTypeRemittance;
                lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeStatusTransition;
                lcdoAcccountReference.item_type_value = lobjRemittance.icdoRemittance.remittance_type_value;
                lcdoAcccountReference.status_transition_value = busConstant.StatusTransitionAppliedToInvalidated;

                busDepositTape lobjDepositTape = new busDepositTape();
                lobjDepositTape.FindDepositTape(_icdoDeposit.deposit_tape_id);

                larrError = busGLHelper.GenerateGL(lcdoAcccountReference,
                                             lobjRemittance.icdoRemittance.person_id,
                                             lobjRemittance.icdoRemittance.org_id,
                                             lobjRemittance.icdoRemittance.remittance_id,
                                             lobjRemittance.icdoRemittance.remittance_amount,
                                             DateTime.Now,
                                             DateTime.Now, iobjPassInfo);

                //Reversal of Service Purchase
                ReverseServicePurchase(lobjRemittance.icdoRemittance.remittance_id);
                //UCS - 079 Start -- Updating Recovery history amounts to zero
                //lobjRemittance.UpdateRecoveryHistory();
                //PIR 26043 -- Delete latest recovery history -- also included UpdateRecoveryHistory() in it
                lobjRemittance.DeleteRecoveryHistory();
                //UCS - 079 End
                //UCS 11 Addendum - Reverse Seminar Payment Allocation
                ReverseSeminarPaymentAllocation(lobjRemittance.icdoRemittance.remittance_id);
                //Negation of IBS payments
                NegateIBSPayments(lobjRemittance);
                //pir 8583
                UpdateEmployerPayrollMonthlyStatement(lobjRemittance);
            }
            return larrError;
        }

        public busEmployerPayrollMonthlyStatement ibusLatestEmployerPayrollMonthlyStatement { get; set; }
        /// <summary>
        /// Update Latest Employer Payroll Monthly Statement's Month due 
        /// </summary>
        /// <param name="aobjRemittance">Remittance Amount to be added to month due</param>
        private void UpdateEmployerPayrollMonthlyStatement(busRemittance aobjRemittance)
        {
            //Update employer payroll monthly statement
            if (ibusLatestEmployerPayrollMonthlyStatement == null)
                LoadLatestEmployerPayrollMonthlyStatement(aobjRemittance.icdoRemittance.org_id, aobjRemittance.icdoRemittance.plan_id);

            if (aobjRemittance.icdoRemittance.applied_date <= ibusLatestEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.run_date)
            {
                ibusLatestEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.remittance_amount -= aobjRemittance.icdoRemittance.remittance_amount;
                ibusLatestEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.month_due_amt += aobjRemittance.icdoRemittance.remittance_amount;
                ibusLatestEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.Update();
            }
        }

        /// <summary>
        /// Load Latest Employer Payroll Monthly Statement
        /// </summary>
        /// <param name="aintOrgID">Org ID</param>
        /// <param name="aintPlanID">Plan ID</param>
        private void LoadLatestEmployerPayrollMonthlyStatement(int aintOrgID, int aintPlanID)
        {
            // PIR 9171 - object reference
            ibusLatestEmployerPayrollMonthlyStatement = new busEmployerPayrollMonthlyStatement { icdoEmployerPayrollMonthlyStatement = new cdoEmployerPayrollMonthlyStatement() };
            
            DataTable ldtbList = busNeoSpinBase.SelectWithOperator<cdoEmployerPayrollMonthlyStatement>(
             new string[2] { "org_id", "plan_id" },
              new string[2] { "=", "=" },
             new object[2] { aintOrgID, aintPlanID }, "run_date desc");
            if (ldtbList.Rows.Count > 0)
            {
                ibusLatestEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.LoadData(ldtbList.Rows[0]);
            }
        }

        private void ReverseSeminarPaymentAllocation(int aintRemittanceID)
        {
            DataTable ldtbList = Select<cdoSeminarAttendeePaymentAllocation>(
              new string[1] { "remittance_id" },
              new object[1] { aintRemittanceID }, null, null);
            Collection<busSeminarAttendeePaymentAllocation> lclbSemAttPaymentAllocation =
               GetCollection<busSeminarAttendeePaymentAllocation>(ldtbList, "icdoSeminarAttendeePaymentAllocation");
            foreach (busSeminarAttendeePaymentAllocation lbusSemAttPaymentAllocation in lclbSemAttPaymentAllocation)
            {
                if (lbusSemAttPaymentAllocation.ibusSeminarAttendeeDetail == null)
                    lbusSemAttPaymentAllocation.LoadSeminarAttendeeDetail();

                lbusSemAttPaymentAllocation.ibusSeminarAttendeeDetail.icdoSeminarAttendeeDetail.attendee_fee_paid_flag = busConstant.Flag_No;
                lbusSemAttPaymentAllocation.ibusSeminarAttendeeDetail.icdoSeminarAttendeeDetail.Update();

                //Reverse GL
                GenerateReverseSeminarGL(lbusSemAttPaymentAllocation.ibusSeminarAttendeeDetail.icdoSeminarAttendeeDetail.person_id,
                                        lbusSemAttPaymentAllocation.ibusSeminarAttendeeDetail.icdoSeminarAttendeeDetail.seminar_attendee_detail_id,
                                        lbusSemAttPaymentAllocation.icdoSeminarAttendeePaymentAllocation.applied_amount);

                lbusSemAttPaymentAllocation.Delete();

            }
        }

        private void GenerateReverseSeminarGL(int aintPersonID, int aintSeminarAttendeeDetailID, decimal adecAmount)
        {
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = busConstant.PlanIdMain;//PIR 20232 ?code
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeSeminar;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeAllocation;
            lcdoAcccountReference.from_item_type_value = busConstant.FromItemTypeSeminarSessions;

            busGLHelper.GenerateGL(lcdoAcccountReference, aintPersonID, 0,
                                               aintSeminarAttendeeDetailID,
                                               adecAmount,
                                               DateTime.Now,
                                               DateTime.Now, iobjPassInfo);
        }

        private void ReverseServicePurchase(int aintRemittanceId)
        {
            int lintReversalUnit = -1;
            DateTime idatRunDate = ibusDepositTape.icdoDepositTape.deposit_date; // PIR 9008 - change date to deposit date

            //Load the List of Payment Allocation which are using this Remittance ID
            DataTable ldtbList = Select<cdoServicePurchasePaymentAllocation>(
                new string[2] { "remittance_id", "posted_flag" },
                new object[2] { aintRemittanceId, busConstant.Flag_Yes }, null, null);

            Collection<busServicePurchasePaymentAllocation> _iclbSerPurPayAllocation =
                GetCollection<busServicePurchasePaymentAllocation>(ldtbList, "icdoServicePurchasePaymentAllocation");
            foreach (busServicePurchasePaymentAllocation lobjAllocation in _iclbSerPurPayAllocation)
            {
                lobjAllocation.ibusServicePurchaseHeader = new busServicePurchaseHeader();
                if (lobjAllocation.ibusServicePurchaseHeader.FindServicePurchaseHeader(lobjAllocation.icdoServicePurchasePaymentAllocation.service_purchase_header_id))
                {
                    if (lobjAllocation.ibusServicePurchaseHeader.ibusPersonAccount == null)
                        lobjAllocation.ibusServicePurchaseHeader.LoadPersonAccount();

                    if (lobjAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPlan == null)
                        lobjAllocation.ibusServicePurchaseHeader.ibusPersonAccount.LoadPlan();

                    if (lobjAllocation.ibusServicePurchaseHeader.ibusPerson == null)
                        lobjAllocation.ibusServicePurchaseHeader.LoadPerson();

                    decimal ldecPreTaxEE = lobjAllocation.icdoServicePurchasePaymentAllocation.pre_tax_ee_amount;
                    decimal ldecPostTaxEE = lobjAllocation.icdoServicePurchasePaymentAllocation.post_tax_ee_amount;
                    decimal ldecPostTaxER = lobjAllocation.icdoServicePurchasePaymentAllocation.post_tax_er_amount;
                    decimal ldecPostTaxEERHIC = lobjAllocation.icdoServicePurchasePaymentAllocation.ee_rhic_amount;
                    decimal ldecPreTaxERRHIC = lobjAllocation.icdoServicePurchasePaymentAllocation.er_rhic_amount;
                    decimal ldecPreTaxER = lobjAllocation.icdoServicePurchasePaymentAllocation.pre_tax_er_amount;
                    decimal ldecPSCToPost = lobjAllocation.icdoServicePurchasePaymentAllocation.prorated_psc;
                    decimal ldecVSCToPost = lobjAllocation.icdoServicePurchasePaymentAllocation.prorated_vsc;

                    //Reversal of GL
                    lobjAllocation.ibusServicePurchaseHeader.PostGLAccount(ldecPreTaxEE, ldecPostTaxEE, ldecPostTaxER, ldecPostTaxEERHIC, ldecPreTaxERRHIC, ldecPreTaxER, lintReversalUnit);

                    // If DC
                    if (lobjAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPlan.IsDCRetirementPlan() ||
                        lobjAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPlan.IsHBRetirementPlan())
                    {
                        busProviderReportDataDC lbusProviderReport = new busProviderReportDataDC();
                        lbusProviderReport.PostContribution(busConstant.SubSystemValueServicePurchase,
                            lobjAllocation.ibusServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id,
                            lobjAllocation.ibusServicePurchaseHeader.ibusPerson.icdoPerson.ssn,
                            lobjAllocation.ibusServicePurchaseHeader.ibusPerson.icdoPerson.person_id,
                            lobjAllocation.ibusServicePurchaseHeader.ibusPerson.icdoPerson.last_name,
                            lobjAllocation.ibusServicePurchaseHeader.ibusPerson.icdoPerson.first_name,
                            lobjAllocation.ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.plan_id,
                            idatRunDate, ldecPostTaxER * lintReversalUnit, ldecPostTaxEE * lintReversalUnit,
                            ldecPreTaxER * lintReversalUnit, ldecPreTaxEE * lintReversalUnit, 0, 0, 0,
                            lobjAllocation.ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.person_account_id,
                            lobjAllocation.ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.provider_org_id);
                    }

                    // reverse Person Account contribution
                    lobjAllocation.ibusServicePurchaseHeader.ReverseRetirementContribution(ldecPreTaxEE, ldecPostTaxEE,
                                                                                           ldecPostTaxER,
                                                                                           ldecPostTaxEERHIC,
                                                                                           ldecPreTaxERRHIC,
                                                                                           ldecPreTaxER, -1,
                                                                                           ldecPSCToPost, ldecVSCToPost,
                                                                                           idatRunDate, lobjAllocation);
                }
            }
        }

        public void btnValidate_Click()
        {
            _icdoDeposit.status_value = busConstant.DepositDetailStatusValid;
            _icdoDeposit.Update();
        }

        public ArrayList btnApply_Click()
        {
            ArrayList larrError = new ArrayList();

            if (_iclbRemittance == null)
            {
                LoadRemittances();
            }

            //Update the Applied Date
            foreach (busRemittance lobjRemittances in _iclbRemittance)
            {
                _ldclTotalRemittanceAmount += lobjRemittances.icdoRemittance.remittance_amount;
            }

            if (_icdoDeposit.deposit_amount == _ldclTotalRemittanceAmount)
            {
                _icdoDeposit.status_value = busConstant.DepositDetailStatusApplied;
                _icdoDeposit.Update();

                //Generate the GL
                foreach (busRemittance lobjRemittance in _iclbRemittance)
                {
                    cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
                    lcdoAcccountReference.plan_id = lobjRemittance.icdoRemittance.plan_id;
                    lcdoAcccountReference.source_type_value = busConstant.SourceTypeRemittance;
                    lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeStatusTransition;
                    lcdoAcccountReference.item_type_value = lobjRemittance.icdoRemittance.remittance_type_value;
                    lcdoAcccountReference.status_transition_value = busConstant.StatusTransitionValidatedToApplied;
                    busDepositTape lobjDepositTape = new busDepositTape();
                    lobjDepositTape.FindDepositTape(_icdoDeposit.deposit_tape_id);

                    larrError = busGLHelper.GenerateGL(lcdoAcccountReference,
                                                lobjRemittance.icdoRemittance.person_id,
                                                lobjRemittance.icdoRemittance.org_id,
                                                lobjRemittance.icdoRemittance.remittance_id,
                                                Math.Abs(lobjRemittance.icdoRemittance.remittance_amount), //PROD PIR 4500 : Negative Remittance Also can come.. but, in GL, we must store Abs Value
                                                DateTime.Now,
                                                lobjDepositTape.icdoDepositTape.deposit_date, iobjPassInfo);

                    //Update the Remitance Applied Date
                    lobjRemittance.icdoRemittance.applied_date = DateTime.Today;//uat pir 1756
                    lobjRemittance.icdoRemittance.Update();
                }
            }
            else
            {
                utlError lobjError = AddError(4123, "");
                larrError.Add(lobjError);
            }
            return larrError;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadPersonOrgNameByID();
            LoadTotalsFromDetail();
        }

        public void SetNewDepositStatus(int AintDepositTapeID)
        {
            int lintInvalidDepositCount = 0;
            lintInvalidDepositCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoDeposit.GetInvalidDepositCountByDepositTapeID", new object[1] { AintDepositTapeID },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            if (lintInvalidDepositCount > 0)
            {
                _icdoDeposit.status_value = busConstant.DepositDetailStatusCorrected;
            }
            else
            {
                _icdoDeposit.status_value = busConstant.DepositDetailStatusReview;
            }
        }

        public bool IsDepositDateValid()
        {
            if ((_ibusDepositTape.icdoDepositTape.deposit_date >= DateTime.Now.AddDays(ibusDepositTape.iintDepositValidityDays)) ||
                (_ibusDepositTape.icdoDepositTape.deposit_date <= DateTime.Now.AddDays(-ibusDepositTape.iintDepositValidityDays)))
            {
                return false;
            }
            return true;
        }

        public bool IsSaveButtonVisible()
        {
            if (_ibusDepositTape == null)
            {
                LoadDepositTape();
            }
            if (IsDepositDateValid())
            {
                if (_ibusDepositTape.icdoDepositTape.status_value == busConstant.DepositTapeStatusReview)
                {
                    return true;
                }
                else if (_ibusDepositTape.icdoDepositTape.status_value == busConstant.DepositTapeStatusValid)
                {
                    if (_icdoDeposit.status_value == busConstant.DepositDetailStatusCorrected)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsNewButtonVisible()
        {
            if (IsDepositDateValid())
            {
                if (_icdoDeposit.status_value == busConstant.DepositDetailStatusReview ||
                    _icdoDeposit.status_value == busConstant.DepositDetailStatusValid)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDeleteButtonVisible()
        {
            if (IsDepositDateValid())
            {
                return true;
            }
            return false;
        }

        public bool IsApplyButtonVisible()
        {
            if (IsDepositDateValid())
            {
                if ((_ibusDepositTape.icdoDepositTape.status_value == busConstant.DepositTapeStatusValid) &&
                    (_icdoDeposit.status_value == busConstant.DepositDetailStatusValid))
                    return true;
            }
            return false;
        }

        public bool IsInvalidateNSFButtonVisible()
        {
            if (IsDepositDateValid())
            {
                if ((_ibusDepositTape.icdoDepositTape.status_value == busConstant.DepositTapeStatusValid) &&
                    (_icdoDeposit.status_value == busConstant.DepositDetailStatusApplied))
                    return true;
            }
            return false;
        }

        private string _lstrOrgCodeID;
        public string lstrOrgCodeID
        {
            get { return _lstrOrgCodeID; }
            set { _lstrOrgCodeID = value; }
        }

        public void LoadOrgID()
        {
            // _icdoDeposit.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(_lstrOrgCodeID);
            _icdoDeposit.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoDeposit.istrOrgCode);
        }

        public void LoadOrgCodeID()
        {
            //_lstrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(_icdoDeposit.org_id);
            icdoDeposit.istrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(_icdoDeposit.org_id);
        }
        
        public void LoadBenfitTypeValue()
        {
            DataTable ldtbBenfitType = Select("cdoDeposit.LoadBenfitTypeValue", new object[1] { icdoDeposit.deposit_id });
            if (ldtbBenfitType.Rows.Count > 0)
            icdoDeposit.istrBenfitTypeValue= Convert.ToString(ldtbBenfitType.Rows[0]["DESCRIPTION"]);
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            LoadOrgID();
            base.BeforeValidate(aenmPageMode);
        }

        // PIR ID - 384
        public string istrPERSLinkID
        {
            get
            {
                string lstrPersonID = string.Empty;
                if (_icdoDeposit.person_id != 0)
                    lstrPersonID = Convert.ToString(_icdoDeposit.person_id);
                return lstrPersonID;
            }
        }

        //Correspondence
        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }

        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();
            return this._ibusPerson;
        }
        //loaded this method to load Person related details
        //this method is called in Find method
        //specially for cor - Insurance payment - PAY-4300
        ////UAT - PIR 168
        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            if (_icdoDeposit.person_id != 0)
            {
                _ibusPerson.FindPerson(_icdoDeposit.person_id);
            }
        }

        public void CreateDeposit(int aintOrgID, int aintPersonID, string astrRefNo, decimal adecAmt, string astrStatus, string astrSource)
        {
            if (_icdoDeposit == null)
                _icdoDeposit = new cdoDeposit();
            _icdoDeposit.org_id = aintOrgID;
            _icdoDeposit.person_id = aintPersonID;
            _icdoDeposit.reference_no = astrRefNo;
            _icdoDeposit.deposit_amount = adecAmt;
            _icdoDeposit.status_value = astrStatus;
            _icdoDeposit.deposit_source_value = astrSource;
            _icdoDeposit.deposit_date = DateTime.Today;
           _icdoDeposit.Insert();
        }
        public void AddRemittance(decimal adecRemittanceAmount, int aintPlanId, string astrRemittanceType)
        {
            busRemittance lbusRemittance = new busRemittance();
            lbusRemittance.icdoRemittance = new cdoRemittance();
            lbusRemittance.icdoRemittance.org_id = _icdoDeposit.org_id;
            lbusRemittance.icdoRemittance.person_id = _icdoDeposit.person_id;
            lbusRemittance.icdoRemittance.plan_id = aintPlanId;
            lbusRemittance.icdoRemittance.deposit_id = _icdoDeposit.deposit_id;
            lbusRemittance.icdoRemittance.remittance_type_value = astrRemittanceType;
            lbusRemittance.icdoRemittance.remittance_amount = adecRemittanceAmount;
            lbusRemittance.icdoRemittance.Insert();
            if (_iclbRemittance != null)
                _iclbRemittance.Add(lbusRemittance);
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            LoadOrgID();
            LoadPersonOrgNameByID();
            larrList.Add(this);
            return larrList;
        }

        public busOrganization ibusOrganization { get; set; }
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
                ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoDeposit.org_id);
        }

        //prod pir 6018 : validation to check whether organization is Active
        public bool IsOrganizationNotActive()
        {
            bool lblnResult = false;
            if (icdoDeposit.org_id > 0)
            {
                if (ibusOrganization == null)
                    LoadOrganization();

                if (ibusOrganization.icdoOrganization.status_value != busConstant.OrganizationStatusActive)
                    lblnResult = true;
            }
            return lblnResult;
        }


        #region UCS - 079

        public Collection<busPaymentRecoveryHistory> iclbRecoveryHistory { get; set; }

        public void LoadRecoveryHistoryByRemittanceID(int aintRemittanceID)
        {
            DataTable ldtRecoveryHistory = Select<cdoPaymentRecoveryHistory>
                (new string[1] { enmPaymentRecoveryHistory.remittance_id.ToString() },
                new object[1] { aintRemittanceID }, null, null);
        }

        #endregion

        #region UCS - 038

        /// <summary>
        /// Method to delete entries from IBS remittance allocation and post negative entries in insurance contribution on NSF of deposit
        /// </summary>
        /// <param name="aintRemittanceID">Remittance ID</param>
        private void NegateIBSPayments(busRemittance aobjRemittance)
        {
            Collection<busIbsRemittanceAllocation> lclbIBSRemittanceAllocation = new Collection<busIbsRemittanceAllocation>();
            Collection<busPersonAccountInsuranceContribution> lclbInsuranceContribution = new Collection<busPersonAccountInsuranceContribution>();
            busPersonAccount lobjPersonAccount;

            //selecting the ibs remittance allocation and insurance contribution for the corresponding remittance
            DataTable ldtIBSRemittanceAllocation = Select<cdoIbsRemittanceAllocation>
                    (new string[2] { enmIbsRemittanceAllocation.remittance_id.ToString(), enmIbsRemittanceAllocation.ibs_allocation_status_value.ToString() },
                    new object[2] { aobjRemittance.icdoRemittance.remittance_id, busConstant.IBSAllocationStatusAllocated }, null, null);
            DataTable ldtInsuranceContribution = Select<cdoPersonAccountInsuranceContribution>
                    (new string[2] { enmPersonAccountInsuranceContribution.subsystem_ref_id.ToString(), enmPersonAccountInsuranceContribution.subsystem_value.ToString() },
                    new object[2] { aobjRemittance.icdoRemittance.remittance_id, busConstant.SubSystemValueIBSPayment }, null, null);
            DataTable ldtIBSDetail = new DataTable();
            if (ldtIBSRemittanceAllocation.Rows.Count > 0)
            {
                lclbIBSRemittanceAllocation = GetCollection<busIbsRemittanceAllocation>(ldtIBSRemittanceAllocation, "icdoIbsRemittanceAllocation");
            }
            if (ldtInsuranceContribution.Rows.Count > 0)
            {
                lclbInsuranceContribution = GetCollection<busPersonAccountInsuranceContribution>(ldtInsuranceContribution, "icdoPersonAccountInsuranceContribution");

                //prod pir 6405
                //need to update payment election adj in completed status to inpayment
                //--start--//
                //get the distinct effective date from insurance contribution of type IBAP
                IEnumerable<busPersonAccountInsuranceContribution> lenmFilteredInsContr = lclbInsuranceContribution.Where(o => o.icdoPersonAccountInsuranceContribution.transaction_type_value == busConstant.PersonAccountTransactionTypeIBSAdjPayment)
                    .GroupBy(o => o.icdoPersonAccountInsuranceContribution.effective_date).Select(o => o.First());
                foreach (busPersonAccountInsuranceContribution lobjContr in lenmFilteredInsContr)
                {
                    //get the ibs details for the effective date and person id and merge into one single table
                    if (ldtIBSDetail.Rows.Count <= 0)
                    {
						//PIR 8164 & 8165
                        ldtIBSDetail = SelectWithOperator<cdoIbsDetail>(new string[3] { "billing_month_and_year", "person_id", "DETAIL_STATUS_VALUE" },
                                                                        new string[3] { "=", "=", "!=" },
                                                                        new object[3] { lobjContr.icdoPersonAccountInsuranceContribution.effective_date, aobjRemittance.icdoRemittance.person_id, busConstant.PayrollDetailStatusIgnored },
                                                                        null);
                    }
                    else
                    {
                        DataTable ldtTempIBSDetail = SelectWithOperator<cdoIbsDetail>(new string[3] { "billing_month_and_year", "person_id", "DETAIL_STATUS_VALUE" },
                                                                            new string[3] {"=", "=", "!="},
                                                                            new object[3] { lobjContr.icdoPersonAccountInsuranceContribution.effective_date, aobjRemittance.icdoRemittance.person_id, busConstant.PayrollDetailStatusIgnored }, 
                                                                            null);
                        ldtIBSDetail.Merge(ldtTempIBSDetail);
                        ldtIBSDetail.AcceptChanges();
                    }
                }
                if (ldtIBSDetail.Rows.Count > 0)
                {
                    //get the distinct payment election adjustments
                    IEnumerable<int?> lenmPEAdj = ldtIBSDetail.AsEnumerable().Where(o => o.Field<int?>("payment_election_adjustment_id") != null)
                        .Select(o => o.Field<int?>("payment_election_adjustment_id")).Distinct();
                    foreach (int lintPEAdjID in lenmPEAdj)
                    {
                        busPaymentElectionAdjustment lobjPEAdj = new busPaymentElectionAdjustment();
                        //load the payment election adjusment and update to in payment if it is in Completed status
                        if (lobjPEAdj.FindPaymentElectionAdjustment(lintPEAdjID) && lobjPEAdj.icdoPaymentElectionAdjustment.status_value == busConstant.IBSAdjustmentStatusCompleted)
                        {
                            lobjPEAdj.icdoPaymentElectionAdjustment.status_value = busConstant.IBSAdjustmentStatusInPayment;
                            lobjPEAdj.icdoPaymentElectionAdjustment.Update();
                        }
                    }
                }
                //--end--//
            }
            //deleting the ibs remittance allocation
            foreach (busIbsRemittanceAllocation lobjAllocation in lclbIBSRemittanceAllocation)
                lobjAllocation.Delete();
            //posting negative entries in insurance contribution
            foreach (busPersonAccountInsuranceContribution lobjContribution in lclbInsuranceContribution)
            {
                lobjPersonAccount = new busPersonAccount();
                if (lobjPersonAccount.FindPersonAccount(lobjContribution.icdoPersonAccountInsuranceContribution.person_account_id))
                {
                    lobjPersonAccount.PostInsuranceContribution(lobjContribution.icdoPersonAccountInsuranceContribution.subsystem_value,
                                                                lobjContribution.icdoPersonAccountInsuranceContribution.subsystem_ref_id,
                                                                DateTime.Today,//todo change to date time if transaction date needs time stamp
                                                                lobjContribution.icdoPersonAccountInsuranceContribution.effective_date,
                                                                lobjContribution.icdoPersonAccountInsuranceContribution.person_employment_dtl_id,
                                                                lobjContribution.icdoPersonAccountInsuranceContribution.transaction_type_value,
                                                                0.0M, lobjContribution.icdoPersonAccountInsuranceContribution.paid_premium_amount * -1,
                                                                0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M,
                                                                lobjContribution.icdoPersonAccountInsuranceContribution.provider_org_id);
                }
            }
            //if applied date < latest ibs regular run date then update IBS person summary for that header, balance forward = balance forward + remittance amount
            //if no ibs person summary record then dont do anything
            if (ibusLastPostedRegularIBSHeader == null)
                LoadLastPostedRegularIBSHeader();
            if (aobjRemittance.icdoRemittance.applied_date < ibusLastPostedRegularIBSHeader.icdoIbsHeader.run_date &&
                aobjRemittance.icdoRemittance.remittance_type_value == busConstant.RemittanceTypeIBSDeposit)
            {
                busIbsPersonSummary lobjIBSPersonSummary = new busIbsPersonSummary();
                lobjIBSPersonSummary.FindIbsPersonSummaryByIbsHeaderAndPerson(ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id,
                    aobjRemittance.icdoRemittance.person_id);
                if (lobjIBSPersonSummary.icdoIbsPersonSummary.ibs_person_summary_id > 0)
                {
                    lobjIBSPersonSummary.icdoIbsPersonSummary.balance_forward += aobjRemittance.icdoRemittance.remittance_amount;
                    lobjIBSPersonSummary.icdoIbsPersonSummary.Update();
                }
            }
        }

        public busIbsHeader ibusLastPostedRegularIBSHeader { get; set; }
        public void LoadLastPostedRegularIBSHeader()
        {
            ibusLastPostedRegularIBSHeader = new busIbsHeader { icdoIbsHeader = new cdoIbsHeader() };
            DataTable ldtbList = busNeoSpinBase.SelectWithOperator<cdoIbsHeader>(
              new string[2] { "report_type_value", "report_status_value" },
               new string[2] { "=", "=" },
              new object[2] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted }, "billing_month_and_year desc");
            if (ldtbList.Rows.Count > 0)
            {
                ibusLastPostedRegularIBSHeader.icdoIbsHeader.LoadData(ldtbList.Rows[0]);
            }
        }
        #endregion
    }
}


