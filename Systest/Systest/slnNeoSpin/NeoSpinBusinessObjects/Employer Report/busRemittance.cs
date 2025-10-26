
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
using Sagitec.DataObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.DataObjects;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busRemittance : busExtendBase
    {
        private busDepositTape _ibusDepositTape;
        public busDepositTape ibusDepositTape
        {
            get { return _ibusDepositTape; }
            set { _ibusDepositTape = value; }
        }

        #region TFFRGroupPaymentInbound
        /*This property is used in Payment file also */
        private decimal _idecDepositAmount;
        public decimal idecDepositAmount
        {
            get
            {
                return _idecDepositAmount;
            }

            set
            {
                _idecDepositAmount = value;
            }
        }
        private String _istrLastName;
        public String istrLastName
        {
            get { return _istrLastName; }
            set { _istrLastName = value; }
        }
        private String _istrSSN;
        public String istrSSN
        {
            get { return _istrSSN; }
            set { _istrSSN = value; }
        }

        #endregion

        #region PaymentInfoFromCentralPayroll

        private String _istrCheckNo;
        public String istrCheckNo
        {
            get { return _istrCheckNo; }
            set { _istrCheckNo = value; }
        }
        private String _istrOrgCodeID;
        public String istrOrgCodeID
        {
            get { return _istrOrgCodeID; }
            set { _istrOrgCodeID = value; }
        }
        private String _istrPlanCode;
        public String istrPlanCode
        {
            get { return _istrPlanCode; }
            set { _istrPlanCode = value; }
        }
        #endregion

        private String _istrPersonName;
        public String istrPersonName
        {
            get { return _istrPersonName; }
            set { _istrPersonName = value; }
        }

        private String _istrOrgName;
        public String istrOrgName
        {
            get { return _istrOrgName; }
            set { _istrOrgName = value; }
        }

        private String _istrRemittancePersonName;
        public String istrRemittancePersonName
        {
            get { return _istrRemittancePersonName; }
            set { _istrRemittancePersonName = value; }

        }
        private String _istrRemittanceOrgName;
        public String istrRemittanceOrgName
        {
            get { return _istrRemittanceOrgName; }
            set { _istrRemittanceOrgName = value; }
        }

        private string _istrPlanName;
        public string istrPlanName
        {
            get { return _istrPlanName; }
            set { _istrPlanName = value; }
        }

        private busDeposit _ibusDeposit;
        public busDeposit ibusDeposit
        {
            get { return _ibusDeposit; }
            set { _ibusDeposit = value; }
        }

        private decimal _ldclAppliedAmount;
        public decimal ldclAppliedAmount
        {
            get { return _ldclAppliedAmount; }
            set { _ldclAppliedAmount = value; }
        }

        private decimal _ldclBalanceAmount;
        public decimal ldclBalanceAmount
        {
            get { return _ldclBalanceAmount; }
            set { _ldclBalanceAmount = value; }
        }

        public void LoadDeposit()
        {
            if (_ibusDeposit == null)
            {
                _ibusDeposit = new busDeposit();
            }
            _ibusDeposit.FindDeposit(_icdoRemittance.deposit_id);
        }

        public void LoadDeposit(int AintDepositID)
        {
            if (_ibusDeposit == null)
            {
                _ibusDeposit = new busDeposit();
            }
            _ibusDeposit.FindDeposit(AintDepositID);
        }

        public void LoadDepositTape()
        {
            if (_ibusDeposit == null)
            {
                LoadDeposit();
                _ibusDeposit.LoadOrgCodeID();
            }
            if (_ibusDepositTape == null)
            {
                _ibusDepositTape = new busDepositTape();
            }
            _ibusDepositTape.FindDepositTape(_ibusDeposit.icdoDeposit.deposit_tape_id);
        }

        public void LoadPersonOrgNameByID()
        {
            if (_ibusDeposit == null)
            {
                LoadDeposit();
            }
            if (_ibusDeposit.icdoDeposit.person_id != 0)
            {
                _istrPersonName = busGlobalFunctions.GetPersonNameByPersonID(_ibusDeposit.icdoDeposit.person_id);
            }
            if (_ibusDeposit.icdoDeposit.org_id != 0)
            {
                _istrOrgName = busGlobalFunctions.GetOrgNameByOrgID(_ibusDeposit.icdoDeposit.org_id);
            }
        }
        //Load Payment History Header
        public busPaymentHistoryHeader ibusPaymentHistoryHeader { get; set; }
        public void LoadPaymentHistoryHeader()
        {
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            ibusPaymentHistoryHeader.FindPaymentHistoryHeader(icdoRemittance.payment_history_header_id);
        }

        public bool IsValidRemittanceAmount()
        {
            if (_ibusDeposit.iclbRemittance == null)
                LoadOtherRemittances();
            decimal ldclOtherRemittedAmount = 0.00M;

            foreach (busRemittance lobjRemittance in _ibusDeposit.iclbRemittance)
            {
                ldclOtherRemittedAmount += lobjRemittance.icdoRemittance.remittance_amount;
            }
            if (_ibusDeposit.icdoDeposit.deposit_amount >= (ldclOtherRemittedAmount + _icdoRemittance.remittance_amount))
            {
                return true;
            }
            return false;
        }

        // Backlog PIR 13705 - Added Visible Rule for 'Update Refund amount' button to hide it for Remmittance with 'Invalidated' status.

        public bool IsRemittanceStatusInvalidated()
        {
            if(ibusDeposit.icdoDeposit.status_value == busConstant.DepositDetailStatusInvalidated)
            {
                return false;
            }
            return true;
        }

        // Load Remittance Type DropdownList by Bank Account value.
        public Collection<cdoCodeValue> GetRemittanceType()
        {
            if (_ibusDepositTape == null)
            {
                LoadDepositTape();
            }
            if (_ibusDepositTape.icdoDepositTape.bank_account_value != null)
            {
                DataTable ldtbResult = Select("cdoRemittance.GetRemittanceTypeByBankAccountType", new object[1] { _ibusDepositTape.icdoDepositTape.bank_account_value ?? String.Empty });
                // PIR ID 82
                if (ldtbResult.Rows.Count == 1)
                {
                    busCodeValue lobjCV = new busCodeValue();
                    lobjCV.icdoCodeValue = new cdoCodeValue();
                    lobjCV.icdoCodeValue.LoadData(ldtbResult.Rows[0]);
                    icdoRemittance.remittance_type_value = lobjCV.icdoCodeValue.code_value;
                }
                return doBase.LoadData<cdoCodeValue>(ldtbResult);
            }
            else
            {
                Collection<cdoCodeValue> lclbCodeValue = new Collection<cdoCodeValue>();
                cdoCodeValue lobjCodeValue = new cdoCodeValue();
                DataTable ldtResult = Select("cdoRemittance.GetRemitBankCrossRefByRemitType", new object[1] { _icdoRemittance.remittance_type_value ?? string.Empty });
                if (ldtResult.Rows.Count > 0)
                    lobjCodeValue.LoadData(ldtResult.Rows[0]);
                lclbCodeValue.Add(lobjCodeValue);
                return lclbCodeValue;
            }
        }

        public Collection<cdoPlan> GetPlan()
        {
            if (_ibusDepositTape == null)
                LoadDepositTape();
            if (_ibusDepositTape.icdoDepositTape.bank_account_value != null)
            {
                DataTable ldtbPlan = Select("cdoRemittance.GetPlanByBankAccountType", new object[1] { _ibusDepositTape.icdoDepositTape.bank_account_value });
                // PIR ID 82
                if (ldtbPlan.Rows.Count == 1)
                {
                    busPlan lobjPlan = new busPlan();
                    lobjPlan.icdoPlan = new cdoPlan();
                    lobjPlan.icdoPlan.LoadData(ldtbPlan.Rows[0]);
                    icdoRemittance.plan_id = lobjPlan.icdoPlan.plan_id;
                }
                return doBase.LoadData<cdoPlan>(ldtbPlan);
            }
            else
            {
                Collection<cdoPlan> lclbPlans = new Collection<cdoPlan>();
                cdoPlan lobjPlan = new cdoPlan();
                lobjPlan.SelectRow(new object[1] { _icdoRemittance.plan_id });
                lclbPlans.Add(lobjPlan);
                return lclbPlans;
            }
        }

        /// PIR ID 241
        private decimal _ldclTotalRemittanceAmount;
        public decimal ldclTotalRemittanceAmount
        {
            get { return _ldclTotalRemittanceAmount; }
            set { _ldclTotalRemittanceAmount = value; }
        }

        public void LoadTotalRemittanceAmount()
        {
            _ldclTotalRemittanceAmount = 0.0M;
            DataTable ldtbList = Select<cdoRemittance>(new string[1] { "deposit_id" }, new object[1] { icdoRemittance.deposit_id }, null, null);
            Collection<busRemittance> iclbRemittance = new Collection<busRemittance>();
            iclbRemittance = GetCollection<busRemittance>(ldtbList, "icdoRemittance");
            foreach (busRemittance lobjRemittance in iclbRemittance)
                _ldclTotalRemittanceAmount += lobjRemittance.icdoRemittance.remittance_amount;
        }

        public busPlan ibusPlan { get; set; }

        public void LoadPlan()
        {
            if (ibusPlan.IsNull())
                ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            ibusPlan.FindPlan(icdoRemittance.plan_id);
           ibusPlan.icdoPlan.PopulateDescriptions();
        }

        public void LoadOtherRemittances()
        {
            DataTable ldtbList = Select("cdoRemittance.LoadOtherRemittances", new object[2] { _icdoRemittance.deposit_id, _icdoRemittance.remittance_id });
            _ibusDeposit.iclbRemittance = new Collection<busRemittance>();
            foreach (DataRow ldtrRow in ldtbList.Rows)
            {
                busRemittance lobjRemittance = new busRemittance
                {
                    icdoRemittance = new cdoRemittance(),
                    ibusDeposit = new busDeposit { icdoDeposit = new cdoDeposit() },
                    ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() },
                    ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                };
                lobjRemittance.icdoRemittance.LoadData(ldtrRow);
                lobjRemittance.ibusDeposit.icdoDeposit.LoadData(ldtrRow);
                lobjRemittance.ibusOrganization.icdoOrganization.LoadData(ldtrRow);
                lobjRemittance.ibusPlan.icdoPlan.LoadData(ldtrRow);

                lobjRemittance.ldclAppliedAmount = busEmployerReportHelper.GetRemittanceAllocatedAmount(lobjRemittance.icdoRemittance.remittance_id, lobjRemittance);
                lobjRemittance.ldclBalanceAmount = lobjRemittance.icdoRemittance.remittance_amount - lobjRemittance.ldclAppliedAmount - lobjRemittance.icdoRemittance.allocated_negative_deposit_amount;
                //If Refund amount is there, we need to subtract from Balance Amount
                if (!string.IsNullOrEmpty(lobjRemittance.icdoRemittance.refund_pend_by))
                    lobjRemittance.ldclBalanceAmount -= lobjRemittance.icdoRemittance.overridden_refund_amount > 0 ? lobjRemittance.icdoRemittance.overridden_refund_amount :
                                                        lobjRemittance.icdoRemittance.computed_refund_amount;                
                //prod pir 6422
                if (lobjRemittance.ibusDeposit.icdoDeposit.status_value == busConstant.DepositDetailStatusInvalidated)
                    lobjRemittance.ldclBalanceAmount = 0.00M;
                //prod pir 6323
                if (lobjRemittance.ldclBalanceAmount == 0 && lobjRemittance.ibusDeposit.icdoDeposit.status_value != busConstant.DepositDetailStatusInvalidated)
                    lobjRemittance.icdoRemittance.allocation_status = busConstant.RemittanceLookUpAllocationStatusAllocated;                
                lobjRemittance.istrOrgCodeID = lobjRemittance.ibusOrganization.icdoOrganization.org_code;
                lobjRemittance.istrPlanName = lobjRemittance.ibusPlan.icdoPlan.plan_name;

                _ibusDeposit.iclbRemittance.Add(lobjRemittance);
            }
        }

        private string _lstrOrgCodeID;
        public string lstrOrgCodeID
        {
            get { return _lstrOrgCodeID; }
            set { _lstrOrgCodeID = value; }
        }

        public void LoadOrgID()
        {
            _icdoRemittance.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(_istrOrgCodeID);
        }

        public void LoadOrgCodeID()
        {
            _istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(_icdoRemittance.org_id);
        }

        public void LoadPlanName()
        {
            busPlan lobjPlan = new busPlan();
            lobjPlan.FindPlan(_icdoRemittance.plan_id);
            _istrPlanName = lobjPlan.icdoPlan.plan_name;
        }

        private Collection<cdoReviewRemittanceDistribution> _iclbRevRemittance;
        public Collection<cdoReviewRemittanceDistribution> iclbRevRemittance
        {
            get { return _iclbRevRemittance; }
            set { _iclbRevRemittance = value; }
        }

        public void LoadReviewRemittanceDistribution()
        {
            DataTable ldtbRevRemittanceDist = DBFunction.DBSelect("cdoRemittance.GetRevRemittanceDist", new object[1] { icdoRemittance.remittance_id },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            _iclbRevRemittance = new Collection<cdoReviewRemittanceDistribution>();
            foreach (DataRow dr in ldtbRevRemittanceDist.Rows)
            {
                cdoReviewRemittanceDistribution lobjRevwRemittance = new cdoReviewRemittanceDistribution();
                lobjRevwRemittance.PayrollHeaderID = Convert.ToInt32(dr["PayrollHeaderID"]);
                lobjRevwRemittance.SeminarID = Convert.ToInt32(dr["SeminarID"]);
                lobjRevwRemittance.AgreementID = Convert.ToInt32(dr["AgreementID"]);
                lobjRevwRemittance.PymtReceivableID = Convert.ToInt32(dr["PymtReceivableID"]);
                lobjRevwRemittance.AppliedAmount = Convert.ToDecimal(dr["AppliedAmount"]);
                lobjRevwRemittance.ReportedAmount = Convert.ToDecimal(dr["ReportedAmount"]);
                _iclbRevRemittance.Add(lobjRevwRemittance);
            }
        }

        public bool IsSaveButtonVisible()
        {
            if (_ibusDepositTape == null)
                LoadDepositTape();
            if (_ibusDepositTape.IsDepositDateValid())
            {
                if (_ibusDeposit.icdoDeposit.status_value == busConstant.DepositDetailStatusReview ||
                    _ibusDeposit.icdoDeposit.status_value == busConstant.DepositDetailStatusValid)
                {
                    return true;
                }
            }
            return false;
        }

        // PIR ID - 393
        public string istrPERSLinkID
        {
            get
            {
                string lstrPersonID = string.Empty;
                if (_icdoRemittance.person_id != 0)
                    lstrPersonID = Convert.ToString(_icdoRemittance.person_id);
                return lstrPersonID;
            }
        }
        // PIR 2082 - validation to check person account exist for person and plan 
        public bool IsPersonAccountNotExist()
        {
            if (icdoRemittance.person_id > 0 && icdoRemittance.plan_id > 0)
            {
                DataTable ldtdPersonAccount = Select<cdoPersonAccount>(new string[2] { "person_id", "plan_id" },
                            new object[2] { icdoRemittance.person_id, icdoRemittance.plan_id }, null, null);
                if (ldtdPersonAccount.Rows.Count == 0)
                {
                    //prod pir 5892 : check for plan VIA payee account
                    //--start--//
                    DataTable ldtPayeeAccount = Select<cdoPayeeAccount>(new string[1] { "payee_perslink_id" },
                                                new object[1] { icdoRemittance.person_id }, null, null);
                    Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
                    lclbPayeeAccount = GetCollection<busPayeeAccount>(ldtPayeeAccount, "icdoPayeeAccount");
                    foreach (busPayeeAccount lobjPA in lclbPayeeAccount)
                    {
                        lobjPA.LoadApplication();
                        if (lobjPA.ibusApplication.icdoBenefitApplication.plan_id == icdoRemittance.plan_id)
                            return false;
                    }
                    //--end--//
                    return true;
                }
            }
            return false;
        }

        //Load Organization
        public busOrganization ibusOrganization { get; set; }

        public void LoadOrganization()
        {
            if (ibusOrganization == null)
                ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoRemittance.org_id);
        }

        public override void BeforePersistChanges()
        {
            //PIR 23649 - When org code is changed in update mode, since it is a field bound to business object's property, the mainCDO is not 
            //being added to automatically to the iarrChangeLog by the system due to which this individual change alone was not getting saved,
            //so adding it to changelog here..
            if (!string.IsNullOrEmpty(_istrOrgCodeID) &&
                iintOldOrgId != busGlobalFunctions.GetOrgIdFromOrgCode(_istrOrgCodeID) &&
                this.iarrChangeLog.Count == 0)
            {
                this.iarrChangeLog.Add(this.icdoRemittance);
                this.icdoRemittance.ienuObjectState = this.icdoRemittance.remittance_id == 0 ? ObjectState.Insert : ObjectState.Update;
            }
            LoadOrgID();
            LoadRemittancePersonOrOrgName();
            if (icdoRemittance.remittance_type_value == busConstant.ItemTypeIBSDeposit && icdoRemittance.plan_id == 0)
                icdoRemittance.plan_id = busConstant.PlanIdGroupHealth;
            //systest pir 2410
            else if (icdoRemittance.remittance_type_value == busConstant.RemittanceTypeSeminar && icdoRemittance.plan_id == 0)
                icdoRemittance.plan_id = busConstant.PlanIdMain;//PIR 20232 ?code
            base.BeforePersistChanges();
        }

        public void LoadRemittancePersonOrOrgName()
        {
            if (icdoRemittance.person_id != 0)
            {
                _istrRemittancePersonName = busGlobalFunctions.GetPersonNameByPersonID(icdoRemittance.person_id);
                _istrRemittanceOrgName = String.Empty;
            }
            else if (icdoRemittance.org_id != 0)
            {
                //icdoRemittance.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrOrgCodeID);
                _istrRemittanceOrgName = busGlobalFunctions.GetOrgNameByOrgID(icdoRemittance.org_id);
                _istrRemittancePersonName = String.Empty;
            }
            else
            {
                _istrRemittanceOrgName = _istrOrgName;
                _istrRemittancePersonName = _istrPersonName;
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadTotalRemittanceAmount();
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            LoadOrgID();
            LoadRemittancePersonOrOrgName();
            larrList.Add(this);
            return larrList;
        }

        public ArrayList btnAllocateNegativeDeposit_Click()
        {
            ArrayList larrList = new ArrayList();

            decimal ldecAllocatedAmount = icdoRemittance.allocated_negative_deposit_amount;

            //Reload the remittance to avoid changes in any other fields.
            icdoRemittance.Select();
            icdoRemittance.allocated_negative_deposit_amount = ldecAllocatedAmount;
            icdoRemittance.Update();

            larrList.Add(this);
            return larrList;
        }

        #region UCS - 079

        //Property to contain Payment Recovery History
        public Collection<busPaymentRecoveryHistory> iclbRecoveryHistory { get; set; }
        /// <summary>
        /// Method to load Payment recovery history
        /// </summary>
        public void LoadPaymentRecoveryHistoryByRemittanceId()
        {
            DataTable ldtRecoveryHistory = Select<cdoPaymentRecoveryHistory>
                (new string[1] { enmPaymentRecoveryHistory.remittance_id.ToString() },
                new object[1] { icdoRemittance.remittance_id }, null, null);
            iclbRecoveryHistory = new Collection<busPaymentRecoveryHistory>();
            iclbRecoveryHistory = GetCollection<busPaymentRecoveryHistory>(ldtRecoveryHistory, "icdoPaymentRecoveryHistory");
            foreach (busPaymentRecoveryHistory lobjRecoveryHistory in iclbRecoveryHistory)
                lobjRecoveryHistory.LoadPaymentRecovery();
        }

        /// <summary>
        /// Method to update Recovery history amounts
        /// </summary>
        public void UpdateRecoveryHistory()
        {
            //need to reload everytime this function is called
            LoadPaymentRecoveryHistoryByRemittanceId();

            int lintPreviousRecoveryId = 0;
            foreach (busPaymentRecoveryHistory lobjRecoveryHistory in iclbRecoveryHistory)
            {
                //Updating recovery history amounts to zero
                lobjRecoveryHistory.ResetAmountFields();
                //updating recovery status to InProgress if the current status is Satisfied
                if (lobjRecoveryHistory.ibusPaymentRecovery.icdoPaymentRecovery.status_value == busConstant.RecoveryStatusSatisfied &&
                    lintPreviousRecoveryId != lobjRecoveryHistory.ibusPaymentRecovery.icdoPaymentRecovery.payment_recovery_id)
                {
                    lobjRecoveryHistory.ibusPaymentRecovery.icdoPaymentRecovery.status_value = busConstant.RecoveryStatusInProcess;
                    lobjRecoveryHistory.ibusPaymentRecovery.icdoPaymentRecovery.Update();
                    //assigning the previous recovery id
                    lintPreviousRecoveryId = lobjRecoveryHistory.ibusPaymentRecovery.icdoPaymentRecovery.payment_recovery_id;
                }
            }
        }

        // PIR 26043 remove the recovery history if the payment is canceled
        public void DeleteRecoveryHistory()
        {
            //need to reload everytime this function is called
            LoadPaymentRecoveryHistoryByRemittanceId();

            //Select the latest recovery history
            busPaymentRecoveryHistory lobjLatestRecoveryHistory = new busPaymentRecoveryHistory { icdoPaymentRecoveryHistory = new cdoPaymentRecoveryHistory() };
            lobjLatestRecoveryHistory = iclbRecoveryHistory.OrderByDescending(i => i.icdoPaymentRecoveryHistory.recovery_history_id).FirstOrDefault();

            //Delete the history line
            if (lobjLatestRecoveryHistory.IsNotNull() && lobjLatestRecoveryHistory.ibusPaymentRecovery.IsNotNull() &&
                lobjLatestRecoveryHistory.icdoPaymentRecoveryHistory.remittance_id > 0 &&
                lobjLatestRecoveryHistory?.ibusPaymentRecovery?.icdoPaymentRecovery?.status_value == busConstant.RecoveryStatusSatisfied)
            {
                UpdateRecoveryHistory();
            }
            else
            {
                if(lobjLatestRecoveryHistory?.icdoPaymentRecoveryHistory?.recovery_history_id > 0)
                    lobjLatestRecoveryHistory.icdoPaymentRecoveryHistory.Delete();
            }

        }

        #endregion


        //prod pir 6018 : validation to check whether organization is Employer
        public bool IsOrganizationNotEmployer()
        {
            bool lblnResult = false;
            if (icdoRemittance.org_id > 0)
            {
                if (ibusOrganization == null)
                    LoadOrganization();

                if (ibusOrganization.icdoOrganization.org_type_value != busConstant.OrganizationTypeEmployer && ibusOrganization.icdoOrganization.org_type_value != busConstant.OrganizationTypeEstate)
                    lblnResult = true;
            }
            return lblnResult;
        }

        // Backlog PIR - 17197 
        public bool IsOrgOrPersonPlanMatch()
        {
            bool lblnResult = false;
            if (this.icdoRemittance.org_id != 0 && icdoRemittance.plan_id > 0)
            {
                if (ibusOrganization.IsNull()) LoadOrganization();
                if (ibusOrganization.iclbOrgPlan.IsNull()) ibusOrganization.LoadOrgPlan();
                bool lblIsOrgPlanExists = false;
                lblIsOrgPlanExists = ibusOrganization.iclbOrgPlan.Any(lobjOrgPlan => lobjOrgPlan.icdoOrgPlan.participation_start_date <= DateTime.Now &&
                                                                                  (lobjOrgPlan.icdoOrgPlan.participation_end_date == DateTime.MinValue ||
                                                                                     lobjOrgPlan.icdoOrgPlan.participation_end_date >= DateTime.Now) && lobjOrgPlan.icdoOrgPlan.plan_id == icdoRemittance.plan_id);
                if ((this.istrSuppressWarnings.IsNullOrEmpty() || this.istrSuppressWarnings == "N") && !lblIsOrgPlanExists)
                {
                    return true;
                }
            }
            if (this.icdoRemittance.person_id != 0 && icdoRemittance.plan_id > 0)
            {
                DataTable ldtAllPersonAccounts = Select<cdoPersonAccount>(new string[2] { enmPersonAccount.person_id.ToString(), enmPersonAccount.plan_id.ToString()},
                                            new object[2] { icdoRemittance.person_id, icdoRemittance.plan_id},null ,null);
                if (this.istrSuppressWarnings.IsNullOrEmpty() || this.istrSuppressWarnings == "N" && ldtAllPersonAccounts.Rows.Count == 0)                                                                                                                  
                {
                  return true;
                }
            }
            return lblnResult;
        }
        public int iintOldOrgId { get; set; }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {

            iintOldOrgId = icdoRemittance.org_id;
            if (!string.IsNullOrEmpty(istrOrgCodeID))
                LoadOrgID();
            else
                icdoRemittance.org_id = 0;
            base.BeforeValidate(aenmPageMode);
        }

        //prod pir 5640 : to remove duplicate while sorting in grid
        public string istrDepositDateRemittanceID
        {
            get
            {
                if (ibusDepositTape == null)
                    LoadDepositTape();

                return (ibusDepositTape.icdoDepositTape.deposit_date == DateTime.MinValue ? "" :
                    ibusDepositTape.icdoDepositTape.deposit_date.ToString(busConstant.DateFormatLongDate)) + icdoRemittance.remittance_id.ToString();
            }
        }

        //prod pir 5640 : to remove duplicate while sorting in grid
        public decimal idecDepositTapeIDRemittanceID
        {
            get
            {
                if (ibusDeposit == null)
                    LoadDeposit();

                return Convert.ToDecimal((ibusDeposit.icdoDeposit.deposit_tape_id <= 0 ? "" : ibusDeposit.icdoDeposit.deposit_tape_id.ToString())
                    + icdoRemittance.remittance_id.ToString());
            }
        }

        //prod pir 5640 : to remove duplicate while sorting in grid
        public decimal idecDepositIDRemittanceID
        {
            get
            {
                return Convert.ToDecimal((icdoRemittance.deposit_id <= 0 ? "" : icdoRemittance.deposit_id.ToString()) + "" + icdoRemittance.remittance_id.ToString());
            }
        }

        //prod pir 5930 : validation to check whether org code differ in deposit and remittance
        public string istrSuppressWarnings { get; set; }

        public bool IsOrgDifferFromDeposit()
        {
            bool lbnResult = false;

            if ((icdoRemittance.org_id > 0 || icdoRemittance.person_id > 0) && (istrSuppressWarnings != busConstant.Flag_Yes))
            {
                if (ibusDeposit == null)
                    LoadDeposit();

                if ((ibusDeposit.icdoDeposit.org_id != icdoRemittance.org_id) || (ibusDeposit.icdoDeposit.person_id != icdoRemittance.person_id))
                    lbnResult = true;
            }

            return lbnResult;
        }
        /// Note :- If Any change in below method or Query for reinsert remittances then please also change in 
        /// busEmployerPayrollHeader - ProcessNegativeAdjustments method code and 'LoadAllNegativeAdjAmountByPlan' query
        public ArrayList btnCancelRefund_Click()
        {
            ArrayList larrError = new ArrayList();
            if (icdoRemittance.deposit_id > 0)
            {
                #region Invalidate logic to unallocate remittances 
                if (ibusDeposit.IsNull())
                    LoadDeposit();

                ibusDeposit.icdoDeposit.status_value = busConstant.DepositDetailStatusInvalidated;
                ibusDeposit.icdoDeposit.Update();

                ibusDeposit.LoadRemittances();

                #endregion
                #region Create new Deposit (same as original) but only Remittances for Org (sum P501/P502 with CNTR for new total of contribution Remittance)

                //LOAD GL ENTRIES BY EMPLOYER PAYROLL HEADER ID 
                DataTable ldtbGLTransactions = Select("entGLTransaction.GetGLTransactionForReverse", new object[1] { ibusDeposit.icdoDeposit.employer_payroll_header_id });
                Collection<busGLTransaction> lclbGLTransaction = GetCollection<busGLTransaction>(ldtbGLTransactions, "icdoGlTransaction");
 
                foreach (DataRow ldtrow in ldtbGLTransactions.Rows)
                {
                    cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
                    lcdoAcccountReference.LoadData(ldtrow);
                    cdoGlTransaction lcdoGlTransaction = new cdoGlTransaction();
                    lcdoGlTransaction.LoadData(ldtrow);
                    if (ibusDeposit.icdoDeposit.employer_payroll_header_id > 0)
                    {
                        //Reverse entry
                        GenerateReverseGLByType(lcdoGlTransaction, lcdoAcccountReference, ibusDeposit.icdoDeposit.employer_payroll_header_id);
                        //Re-Insert GL entries only for Org
                        if (lcdoGlTransaction.org_id.IsNull() || lcdoGlTransaction.org_id == 0)
                            lcdoGlTransaction.org_id = ibusDeposit.icdoDeposit.org_id;
                        GenerateNegativeAdjustmentGLByType(lcdoGlTransaction, lcdoAcccountReference, ibusDeposit.icdoDeposit.employer_payroll_header_id);
                    }
                }

                // Remittances                 
                foreach (busRemittance lobjRemittance in ibusDeposit.iclbRemittance)
                {
                    //Post Production issue
                    //Delete the remittance allocation having remittance with deposit status as (NSF or Invalidated)
                    busEmployerPayrollHeader lobjEmployerPayrollHeader = new busEmployerPayrollHeader();
                    DataTable ldtbGetPayrollHeaderByRemittanceID = busBase.Select("cdoEmployerRemittanceAllocation.LoadHeaderByRemittanceID", new object[1] { lobjRemittance.icdoRemittance.remittance_id });
                    Collection<busEmployerPayrollHeader> lclbEmployerPayroll = new Collection<busEmployerPayrollHeader>();
                    lclbEmployerPayroll = lobjEmployerPayrollHeader.GetCollection<busEmployerPayrollHeader>(ldtbGetPayrollHeaderByRemittanceID, "icdoEmployerPayrollHeader");


                    foreach (busEmployerPayrollHeader lobjEmployerPayroll in lclbEmployerPayroll)
                    {
                        //Delete the remittance allocation having remittance with deposit status as (NSF or Invalidated)
                        DBFunction.DBNonQuery("cdoEmployerRemittanceAllocation.DeleteRemittanceAllocationWithDepositStatusNSFOrInValid",
                                 new object[2] { lobjEmployerPayroll.icdoEmployerPayrollHeader.employer_payroll_header_id, lobjRemittance.icdoRemittance.remittance_id },
                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                        // Get count of remittance allocation with status as Allocated
                        //if Count is greater than 0 then set balancing status as Unbalanced
                        //else set as NoREmittance
                        int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoEmployerRemittanceAllocation.GetCountOfRemittanceWithAlocSatus",
                                                 new object[1] { lobjEmployerPayroll.icdoEmployerPayrollHeader.employer_payroll_header_id },
                                                 iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                        if (lintCount > 0)
                        {
                            lobjEmployerPayroll.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusUnbalanced;
                        }
                        else
                        {
                            lobjEmployerPayroll.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;
                        }
                        lobjEmployerPayroll.icdoEmployerPayrollHeader.Update();
                    }
                    //if (!(lobjRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeMemberExcessContributionTaxable ||
                    //    lobjRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeMemberExcessContributionNonTaxable))
                    //{
                    //    if(lobjRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeContribution)
                    //    {
                    //        decimal ldecTotalExcessTaxableNonTaxableAmt = ibusDeposit.iclbRemittance.Where(i => ((i.icdoRemittance.remittance_type_value == busConstant.ItemTypeMemberExcessContributionTaxable ||
                    //                                                                                         i.icdoRemittance.remittance_type_value == busConstant.ItemTypeMemberExcessContributionNonTaxable) &&
                    //                                                                                         i.icdoRemittance.plan_id == lobjRemittance.icdoRemittance.plan_id)).Sum(i => i.icdoRemittance.remittance_amount);
                    //        CreateRemittanceforNegativeAdjustments(ldecTotalExcessTaxableNonTaxableAmt + lobjRemittance.icdoRemittance.remittance_amount, lobjRemittance.icdoRemittance.plan_id, lobjCdoDeposit.deposit_id, lobjRemittance.icdoRemittance.remittance_type_value, lobjCdoDeposit.org_id);
                    //    }
                    //    else
                    //        CreateRemittanceforNegativeAdjustments(lobjRemittance.icdoRemittance.remittance_amount, lobjRemittance.icdoRemittance.plan_id, lobjCdoDeposit.deposit_id, lobjRemittance.icdoRemittance.remittance_type_value, lobjCdoDeposit.org_id);
                    //}
                    lobjRemittance.icdoRemittance.refund_status_value = busConstant.DepositRefundStatusPending;
                    lobjRemittance.icdoRemittance.Update();
                }

                DataTable ltdbList = busNeoSpinBase.Select("entEmployerPayrollHeader.LoadCancelRefundAdjustmentAmountsByPlan",
                                                                                 new object[1] { ibusDeposit.icdoDeposit.employer_payroll_header_id });
                Collection<busEmployerPayrollHeader> _iclbNegativeAdjustments = new Collection<busEmployerPayrollHeader>();
                decimal ldecTotalNegAdjContribution = 0;

                foreach (DataRow dr in ltdbList.Rows)
                {
                    var empHeader = new busEmployerPayrollHeader();
                    sqlFunction.LoadQueryResult(empHeader, dr);

                    // Calculate Total Reported Retirement Contribution
                    empHeader.idecTotalReportedRetrContributionByPlan = empHeader.idecTotalEEContributionReportedByPlan +
                                                                empHeader.idecTotalERContributionReportedByPlan +
                                                                empHeader.idecTotalEEPreTaxReportedByPlan +
                                                                empHeader.idecTotalEEEmployerPickupReportedByPlan +
                                                                empHeader.idecTotalMemberInterestCalculatedByPlan +
                                                                empHeader.idecTotalEmployerInterestCalculatedByPlan +
                                                                empHeader.idecTotalEEPostTaxAddlCalculatedByPlan +
                                                                empHeader.idecTotalEEPreTaxAddlCalculatedByPlan +
                                                                empHeader.idecTotalERPreTaxMatchCalculatedByPlan; // PIR 25920 New Plan DC 2025

                    // Calculate Total Reported RHIC Contribution
                    empHeader.idecTotalReportedRHICContributionByPlan = empHeader.idecTotalRHICERContributionReportedByPlan +
                                                                empHeader.idecTotalRHICEEContributionReportedByPlan +
                                                                empHeader.idecTotalRHICEmployerInterestCalculatedByPlan;

                    // Total Negative Adjustment
                    ldecTotalNegAdjContribution += empHeader.idecTotalReportedRetrContributionByPlan + empHeader.idecTotalReportedRHICContributionByPlan +
                                                   empHeader.idecTotalADECCalculatedByPlan;

                    _iclbNegativeAdjustments.Add(empHeader);
                }

                if (ldecTotalNegAdjContribution > 0)
                {
                    cdoDeposit lobjCdoDeposit = new cdoDeposit();
                    CreateDepositAfterCancelRefund(lobjCdoDeposit, ldecTotalNegAdjContribution);
                    foreach (busEmployerPayrollHeader lobjEmpHeader in _iclbNegativeAdjustments)
                    {
                        if (lobjEmpHeader.idecTotalReportedRetrContributionByPlan > 0)
                        {
                            CreateRemittanceforNegativeAdjustments(lobjEmpHeader.idecTotalReportedRetrContributionByPlan, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeContribution, lobjCdoDeposit.org_id);
                        }
                        if (lobjEmpHeader.idecTotalReportedRHICContributionByPlan > 0)
                        {
                            CreateRemittanceforNegativeAdjustments(lobjEmpHeader.idecTotalReportedRHICContributionByPlan, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeRHICContribution, lobjCdoDeposit.org_id);
                        }
                        //PIR 25920 DC 2025 changes
                        if (lobjEmpHeader.idecTotalADECCalculatedByPlan > 0)
                        {
                            CreateRemittanceforNegativeAdjustments(lobjEmpHeader.idecTotalADECCalculatedByPlan, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeADECContribution, lobjCdoDeposit.org_id);
                        }
                    }
                }
                #endregion
                //Reload Deposit
                LoadDeposit();
            }
            return larrError;
        }
        public DateTime idtGLPostingDate { get; set; }
        private void GenerateReverseGLByType(cdoGlTransaction aobjGL, cdoAccountReference aobjAccountReference, int aintSourceID)
        {
            if (idtGLPostingDate == DateTime.MinValue)
                idtGLPostingDate = busGlobalFunctions.GetSysManagementBatchDate();
           
            busGLHelper.ReverseGLTransactionForCancelRefund(aobjAccountReference, aobjGL.person_id, aobjGL.org_id, aintSourceID, aobjGL.credit_amount, aobjGL.effective_date, idtGLPostingDate);
        }
        private void GenerateNegativeAdjustmentGLByType(cdoGlTransaction aobjGLTransaction, cdoAccountReference aobjAccountReference, int aintSourceID)
        {
            busGLHelper.GenerateGL(aobjAccountReference,  0 , aobjGLTransaction.org_id,
                                               aintSourceID,
                                               aobjGLTransaction.credit_amount,
                                               aobjGLTransaction.effective_date,
                                               busGlobalFunctions.GetSysManagementBatchDate(), iobjPassInfo);

            //Generate Fund Transfer GL only for RHIC
            if (aobjAccountReference.item_type_value == busConstant.ItemTypeRHICEmpReport)
            {
                aobjAccountReference.plan_id = aobjGLTransaction.plan_id;
                aobjAccountReference.source_type_value = busConstant.SourceTypeEmployerReporting;
                aobjAccountReference.transaction_type_value = busConstant.TransactionTypeTransfer;
                aobjAccountReference.status_transition_value = string.Empty;
                aobjAccountReference.item_type_value = busConstant.ItemTypeNegRHICEmpReport;
                busGLHelper.GenerateGL(aobjAccountReference,  0, aobjGLTransaction.org_id,
                                                   aintSourceID,
                                                   aobjGLTransaction.credit_amount,
                                                   aobjGLTransaction.effective_date,
                                                   busGlobalFunctions.GetSysManagementBatchDate(), iobjPassInfo);
            }
        }

        public void CreateRemittanceforNegativeAdjustments(decimal adecRemittanceAmount, int aintPlanId, int aintDepositID, string astrRemittanceType, int aintOrgId)
        {
            cdoRemittance lobjcdoRemittance = new cdoRemittance();
            lobjcdoRemittance.org_id = aintOrgId;
            lobjcdoRemittance.plan_id = aintPlanId;
            lobjcdoRemittance.deposit_id = aintDepositID;
            lobjcdoRemittance.remittance_type_value = astrRemittanceType;
            lobjcdoRemittance.remittance_amount = adecRemittanceAmount;
            lobjcdoRemittance.applied_date = busGlobalFunctions.GetSysManagementBatchDate().Date;
            lobjcdoRemittance.Insert();
        }
        public void CreateDepositAfterCancelRefund(cdoDeposit lobjCdoDeposit, decimal adecDepositeAmount)
        {
            lobjCdoDeposit.org_id = ibusDeposit.icdoDeposit.org_id;
            lobjCdoDeposit.reference_no = ibusDeposit.icdoDeposit.reference_no;
            lobjCdoDeposit.deposit_amount = adecDepositeAmount;
            lobjCdoDeposit.status_value = busConstant.DepositDetailStatusApplied;
            lobjCdoDeposit.deposit_source_value = busConstant.DepositSourceNegativeAdjustment;
            lobjCdoDeposit.employer_payroll_header_id = ibusDeposit.icdoDeposit.employer_payroll_header_id;
            lobjCdoDeposit.deposit_date = DateTime.Today;
            lobjCdoDeposit.Insert();
        }
        public bool IsCancelRefundButtonVisible()
        {
			if (ibusDeposit.IsNull())
            	LoadDeposit();
        	ibusDeposit.LoadRemittances();
            return ((icdoRemittance.refund_status_value == busConstant.DepositRefundStatusApproved || icdoRemittance.refund_status_value == busConstant.DepositRefundStatusPending) &&
                      ibusDeposit.IsNotNull() && ibusDeposit.icdoDeposit.status_value != busConstant.DepositDetailStatusInvalidated && ibusDeposit.iclbRemittance.Any(i => i.icdoRemittance.remittance_type_value == busConstant.ItemTypeMemberExcessContributionTaxable || i.icdoRemittance.remittance_type_value == busConstant.ItemTypeMemberExcessContributionNonTaxable));
        }
    }
}
