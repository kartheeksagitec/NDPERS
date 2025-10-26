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
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using Sagitec.ExceptionPub;
using System.Collections.Specialized;
using Sagitec.Bpm;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busDepositTape : busExtendBase
    {
        // PIR 6905
        public decimal idecTotalRemittance {get;set;}
        public void LoadTotalRemittance(int aintDepositTapeId)
        {
            IsDepositOrgIdNotExist();
            DataTable ldtbDeposit = Select("cdoDeposit.GetTotalRemittance", new object[1] { aintDepositTapeId });
            if (ldtbDeposit.Rows.Count > 0)
                idecTotalRemittance = Convert.ToDecimal(ldtbDeposit.Rows[0]["TOTAL_REMITTANCE"]);
            else
                idecTotalRemittance = 0;
        }

        public int iintActivityInstanceID { get; set; }
        // to get deposit tape validity days from code value
        public int iintDepositValidityDays
        {
            get
            {
                int lintDepositDays = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, "DTVD", iobjPassInfo));
                return lintDepositDays;
            }
        }

        private Collection<busDeposit> _iclbDeposit;
        public Collection<busDeposit> iclbDeposit
        {
            get { return _iclbDeposit; }
            set { _iclbDeposit = value; }
        }

        public void LoadDeposits()
        {
            DataTable ldtbList = Select<cdoDeposit>(
                new string[1] { "deposit_tape_id" },
                new object[1] { _icdoDepositTape.deposit_tape_id }, null, null);
            _iclbDeposit = GetCollection<busDeposit>(ldtbList, "icdoDeposit");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busDeposit)
            {
                busDeposit lobjDeposit = (busDeposit)aobjBus;
                lobjDeposit.LoadOrgCodeID();
            }
        }

        public bool IsDepositOrgIdNotExist()
        {
            DataTable ldtbList = Select<cdoDeposit>(
               new string[2] { "deposit_tape_id", "org_id" },
               new object[2] { _icdoDepositTape.deposit_tape_id, null }, null, null);
            if (ldtbList.Rows.Count > 0)
                return false;
            return true;
        }

        public void LoadDepositsCountAndTotalAmount()
        {
            if (_iclbDeposit == null)
            {
                LoadDeposits();
            }
            foreach (busDeposit lobjDeposit in _iclbDeposit)
            {
                if ((lobjDeposit.icdoDeposit.status_value != busConstant.DepositDetailStatusInvalidated)
                    && (lobjDeposit.icdoDeposit.status_value != busConstant.DepositDetailStatusNonSufficientFund))
                {
                    _icdoDepositTape.DepositsCount += 1;
                    _icdoDepositTape.TotalDepositAmount += lobjDeposit.icdoDeposit.deposit_amount;
                }
            }
        }

        public override void BeforePersistChanges()
        {
            if (_icdoDepositTape.ienuObjectState == ObjectState.Insert)
            {
                _icdoDepositTape.status_value = busConstant.DepositTapeStatusReview;

                if ((iintActivityInstanceID == 0) && (ibusBaseActivityInstance != null))
                {
                    //iintActivityInstanceID = ((busActivityInstance)ibusBaseActivityInstance).icdoActivityInstance.activity_instance_id;
                    iintActivityInstanceID = iintActivityInstanceId;
                }
                icdoDepositTape.pull_ach_status_value = busConstant.PullACHReadyStatus;
            }
            base.BeforePersistChanges();
        }

        public ArrayList btnACHPull_Click()
        {
            ArrayList larrReturnList = new ArrayList();
            utlError lobjError;
            /*if (IsACHPullApprovedStatusExists())
            {
                lobjError = new utlError { istrErrorID = "4175", istrErrorMessage = "There is already a Deposit tape with Pull ACH approved" };
                larrReturnList.Add(lobjError);
                return larrReturnList;
            }

            //Updating the Pull ACH status to ready, so that batch will select these records and create the file
            icdoDepositTape.pull_ach_status_value = busConstant.PullACHReadyStatus;
            icdoDepositTape.Update();*/

            GeneratePullACHForIBS();

            icdoDepositTape.Select();

            LoadDeposits();
            LoadDepositsCountAndTotalAmount();
            larrReturnList.Add(this);
            this.EvaluateInitialLoadRules();

            return larrReturnList;
        }

        public ArrayList ValidateDepositTape()
        {
            ArrayList larrError = new ArrayList();
            utlError lobjError = null;
            //PIR 451 
            //Validation can not be done if the deposit date is greater than today's date
            if (_icdoDepositTape.deposit_date > DateTime.Now)
            {
                lobjError = AddError(1343, "");
                larrError.Add(lobjError);
                return larrError;
            }

            /// PIR ID 407
            /// Change to Valid Status only if the Total Remittance amount matches the corresponding Deposit Amount
            if (_icdoDepositTape.total_amount == _icdoDepositTape.TotalDepositAmount)
            {
                int lintCount = 0;
                foreach (busDeposit lobjDeposit in _iclbDeposit)
                {
                    lobjDeposit.iclbRemittance = new Collection<busRemittance>();
                    lobjDeposit.LoadRemittances();
                    decimal ldclTotalRemittanceAmount = 0.0M;
                    foreach (busRemittance lobjRemittance in lobjDeposit.iclbRemittance)
                        ldclTotalRemittanceAmount += lobjRemittance.icdoRemittance.remittance_amount;
                    if (ldclTotalRemittanceAmount == lobjDeposit.icdoDeposit.deposit_amount)
                        lintCount++;
                }
                if (_iclbDeposit.Count == lintCount)
                {
                    foreach (busDeposit lobjDeposit in _iclbDeposit)
                    {
                        lobjDeposit.icdoDeposit.status_value = busConstant.DepositDetailStatusValid;
                        lobjDeposit.icdoDeposit.Update();
                    }
                    _icdoDepositTape.status_value = busConstant.DepositDetailStatusValid;
                    _icdoDepositTape.Update();
                    return larrError;
                }
            }

            lobjError = AddError(4108, "");
            larrError.Add(lobjError);
            return larrError;
        }
        /// <summary>
        /// To Make the Status of Every Deposit In DepositTape to Applied Status And trigger the G/L   ..PIR-5952 Start
        /// </summary>
        /// <returns></returns>
        public ArrayList btnApply_Click()
        {
            ArrayList larrError = new ArrayList();
            if (iclbDeposit == null)
                LoadDeposits();
            if (this.iclbDeposit != null && this.iclbDeposit.Count != 0)
            {
                foreach (busDeposit lbusDeposit in this.iclbDeposit)
                {
                    if (lbusDeposit.iclbRemittance == null)
                        lbusDeposit.LoadRemittances();
                    foreach (busRemittance lobjRemittance in lbusDeposit.iclbRemittance)
                    {
                        lbusDeposit.ldclTotalRemittanceAmount += lobjRemittance.icdoRemittance.remittance_amount;
                    }
                    if (lbusDeposit.icdoDeposit.deposit_amount == lbusDeposit.ldclTotalRemittanceAmount)
                    {
                        if (lbusDeposit.icdoDeposit.status_value == busConstant.DepositDetailStatusValid)
                        {
                            lbusDeposit.icdoDeposit.status_value = busConstant.DepositDetailStatusApplied;
                            lbusDeposit.icdoDeposit.Update();

                            foreach (busRemittance lobjRemmitance in lbusDeposit.iclbRemittance)
                            {
                                cdoAccountReference lcdoAccountReference = new cdoAccountReference();
                                lcdoAccountReference.plan_id = lobjRemmitance.icdoRemittance.plan_id;
                                lcdoAccountReference.source_type_value = busConstant.SourceTypeRemittance;
                                lcdoAccountReference.transaction_type_value = busConstant.TransactionTypeStatusTransition;
                                lcdoAccountReference.item_type_value = lobjRemmitance.icdoRemittance.remittance_type_value;
                                lcdoAccountReference.status_transition_value = busConstant.StatusTransitionValidatedToApplied;
                                busDepositTape lobjDepositTape = new busDepositTape();
                                lobjDepositTape.FindDepositTape(lbusDeposit.icdoDeposit.deposit_tape_id);

                                larrError = busGLHelper.GenerateGL(lcdoAccountReference,
                                                    lobjRemmitance.icdoRemittance.person_id,
                                                    lobjRemmitance.icdoRemittance.org_id,
                                                    lobjRemmitance.icdoRemittance.remittance_id,
                                                    Math.Abs(lobjRemmitance.icdoRemittance.remittance_amount),
                                                    DateTime.Now,
                                                    lobjDepositTape.icdoDepositTape.deposit_date, iobjPassInfo);

                                //Update the Remitance Applied Date
                                lobjRemmitance.icdoRemittance.applied_date = DateTime.Today;//uat pir 1756
                                lobjRemmitance.icdoRemittance.Update();
                            }
                        }
                    }
                    else
                    {
                        utlError lobjError = AddError(4123, "");
                        larrError.Add(lobjError);
                        break;
                    }
                }
            }
            return larrError;
        }
        //PIR-5952 End
        public bool IsDepositDateValid()
        {
            if ((_icdoDepositTape.deposit_date >= DateTime.Now.AddDays(iintDepositValidityDays)) ||
                (_icdoDepositTape.deposit_date <= DateTime.Now.AddDays(-iintDepositValidityDays)))
            {
                return false;
            }
            return true;
        }

        //PIR-5952 Start 
        public bool IsDepositTapeValid()
        {
            if (icdoDepositTape.status_value == busConstant.DepositTapeStatusValid && iclbDeposit.Any(o=>o.icdoDeposit.status_value != busConstant.DepositDetailStatusApplied))
                return true;
            return false;
        }
        //PIR-5952 End

        public bool IsSaveButtonVisible()
        {
            if (IsDepositDateValid())
            {
                if (_icdoDepositTape.status_value == busConstant.DepositTapeStatusReview)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsValidateButtonVisible()
        {
            if (IsDepositDateValid())
            {
                if (_icdoDepositTape.status_value == busConstant.DepositTapeStatusReview)
                    return true;
            }
            return false;
        }

        public bool IsNewButtonVisible()
        {
            if (IsDepositDateValid())
            {
                int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoDeposit.GetInvalidDepositCountByDepositTapeID", new object[1] { _icdoDepositTape.deposit_tape_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                if ((_icdoDepositTape.status_value == busConstant.DepositTapeStatusReview) || (lintCount > 0))
                    return true;
            }
            return false;
        }

        public bool IsACHPullApprovedStatusExists()
        {
            bool lblnResult = false;
            DataTable ldtDepositTape = Select<cdoDepositTape>(new string[1] { "pull_ach_status_value" },
                                                                new object[1] { busConstant.PullACHReadyStatus }, null, null);
            DataTable ldtDepositTapeFiltered = ldtDepositTape.AsEnumerable()
                                                            .Where(o => o.Field<int>("deposit_tape_id") != icdoDepositTape.deposit_tape_id)
                                                            .AsDataTable();
            if (ldtDepositTapeFiltered.Rows.Count > 0)
                lblnResult = true;
            return lblnResult;
        }

        public ArrayList btnACHPullRetirement_Click()
        {
            ArrayList larrReturnList = new ArrayList();
            utlError lobjError;

            GeneratePullACHForEmployers(busConstant.PayrollHeaderBenefitTypeRtmt);

            icdoDepositTape.Select();
            LoadDeposits();
            LoadDepositsCountAndTotalAmount();
            larrReturnList.Add(this);
            iblnAllocateRemittances = true;
            this.EvaluateInitialLoadRules();

            return larrReturnList;
        }

        public ArrayList btnACHPullInsurance_Click()
        {
            ArrayList larrReturnList = new ArrayList();
            utlError lobjError;

            GeneratePullACHForEmployers(busConstant.PayrollHeaderBenefitTypeInsr);

            icdoDepositTape.Select();
            LoadDeposits();
            LoadDepositsCountAndTotalAmount();
            larrReturnList.Add(this);
            iblnAllocateRemittances = true;
            this.EvaluateInitialLoadRules();

            return larrReturnList;
        }

        public ArrayList btnACHPullDefComp_Click()
        {
            ArrayList larrReturnList = new ArrayList();
            utlError lobjError;

            GeneratePullACHForEmployers(busConstant.PayrollHeaderBenefitTypeDefComp);

            icdoDepositTape.Select();
            larrReturnList.Add(this);
            LoadDeposits();
            LoadDepositsCountAndTotalAmount();
            iblnAllocateRemittances = true;
            this.EvaluateInitialLoadRules();

            return larrReturnList;
        }

        public ArrayList btnACHPullServicePurchase_Click()
        {
            ArrayList larrReturnList = new ArrayList();
            utlError lobjError;

            GeneratePullACHForEmployers(busConstant.PayrollHeaderBenefitTypePurchases);

            icdoDepositTape.Select();
            LoadDeposits();
            LoadDepositsCountAndTotalAmount();
            larrReturnList.Add(this);
            this.EvaluateInitialLoadRules();

            return larrReturnList;
        }

        public string istrHeaderType { get; set; }

        //PIR-20539 
        public ArrayList btnAllocateRemittance_Click()
        {
            ArrayList larrReturnList = new ArrayList();
            utlError lobjError;
            AllocateRemittanceToEmployerHeader();

            icdoDepositTape.Select();
            LoadDeposits();
            LoadDepositsCountAndTotalAmount();
            larrReturnList.Add(this);
            iblnAllocateRemittances = false;
            this.EvaluateInitialLoadRules();
            return larrReturnList;
        }
        public bool iblnPullACHForInsurance { get; set; }
        public bool iblnPullACHForRetirement { get; set; }
        public bool iblnPullACHForPurchase { get; set; }
        public bool iblnPullACHForDeferredComp { get; set; }
        public bool iblnPullACHForIBS { get; set; }
        public bool iblnAllocateRemittances { get; set; }
        public void SetVisibilityForPullACH()
        {
            if (iintActivityInstanceID > 0)
            {
                busBpmActivityInstance lbusActivityInstance = busWorkflowHelper.GetActivityInstance(iintActivityInstanceID);
                if (lbusActivityInstance.icdoBpmActivityInstance.activity_instance_id > 0)
                {
                    if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_ACH_Pull_For_IBS_Insurance)
                    {
                        iblnPullACHForIBS = true;
                    }
                    else
                    {
                        string lstrAdditionalParameter1 = Convert.ToString(lbusActivityInstance.GetBpmParameterValue("additional_parameter1"));
                        if (!string.IsNullOrEmpty(lstrAdditionalParameter1))
                        {
                            int lintEmployerPayrollHeaderID = Convert.ToInt32(lstrAdditionalParameter1);
                            busEmployerPayrollHeader lbusEmpHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                            lbusEmpHeader.FindEmployerPayrollHeader(lintEmployerPayrollHeaderID);
                            if (lbusEmpHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                                iblnPullACHForRetirement = true;
                            else if (lbusEmpHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                                iblnPullACHForPurchase = true;
                            else if (lbusEmpHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                                iblnPullACHForInsurance = true;
                            else if (lbusEmpHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                                iblnPullACHForDeferredComp = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// method to create deposits , remittances and generate pull ach file for ibs
        /// </summary>
        public void GeneratePullACHForIBS()
        {
            busNeoSpinBase lobjNSBase = new busNeoSpinBase();
            int lintPrevPersonID = 0, lintDepositID = 0, lintRemittanceId = 0; 
            string lstrPrevBankAccountNumber = string.Empty;
            decimal ldecTotalDepositTapeAmount = 0.0M;
            // Select from IBS Detail table
            DataTable ldtbIBSDetail = busBase.Select("cdoIbsDetail.LoadACHIBSDetail", new object[0] { });
            Collection<busACHProviderReportData> lclbACHPullFile = new Collection<busACHProviderReportData>();

            decimal ldecTotalPremium = 0.0M, ldecAdjustmentAmount = 0.0M, ldecTotalBalanceForward = 0.0M, ldecTotalRemittanceBalFwd = 0.0M;
            foreach (DataRow dr in ldtbIBSDetail.Rows)
            {
                busIbsDetail lobjIbsDetail = new busIbsDetail { icdoIbsDetail = new cdoIbsDetail() };
                lobjIbsDetail.icdoIbsDetail.LoadData(dr);
                busPersonAccountAchDetail lobjACHDetail = new busPersonAccountAchDetail { icdoPersonAccountAchDetail = new cdoPersonAccountAchDetail() };
                lobjACHDetail.icdoPersonAccountAchDetail.LoadData(dr);
                busIbsPersonSummary lobjIBSSummary = new busIbsPersonSummary { icdoIbsPersonSummary = new cdoIbsPersonSummary() };
                lobjIBSSummary.icdoIbsPersonSummary.LoadData(dr);
                busOrganization lobjOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lobjOrganization.icdoOrganization.LoadData(dr);
                //balance forward col. appears twice, so need to load explicitly
                //TODO: need to remove the below code once balance forward removed from ibs detail table
                lobjIBSSummary.FindIbsPersonSummary(lobjIBSSummary.icdoIbsPersonSummary.ibs_person_summary_id);

                if (lobjIbsDetail.icdoIbsDetail.deposit_id == 0)
                {
                    if (lintPrevPersonID != lobjIbsDetail.icdoIbsDetail.person_id || lstrPrevBankAccountNumber != lobjACHDetail.icdoPersonAccountAchDetail.bank_account_number.Trim())
                    {
                        ldecAdjustmentAmount = ldecTotalBalanceForward = ldecTotalRemittanceBalFwd = 0.0M;
                        ldecTotalPremium = 0.0M;
                        lintDepositID = 0;
                        //PIR 13714
                        //DataRow[] ldarrIBSDetail = ldtbIBSDetail.FilterTable(busConstant.DataType.Numeric, "person_id", lobjIbsDetail.icdoIbsDetail.person_id);
                        DataTable ldtbIBSDetailByPersonAndBankAccountNumber = ldtbIBSDetail.AsEnumerable().Where(i => i.Field<int>("person_id") == lobjIbsDetail.icdoIbsDetail.person_id
                                                     && i.Field<string>("bank_account_number").Trim() == lobjACHDetail.icdoPersonAccountAchDetail.bank_account_number.Trim()).AsDataTable();

                        if (ldtbIBSDetailByPersonAndBankAccountNumber != null && ldtbIBSDetailByPersonAndBankAccountNumber.Rows.Count > 0)
                        {
                            ldecTotalPremium = ldtbIBSDetailByPersonAndBankAccountNumber.AsEnumerable().Sum(o => o.Field<decimal>("member_premium_amount"));
                            int lintPreviousIBSHeaderID = 0;
                            foreach (DataRow ldr in ldtbIBSDetailByPersonAndBankAccountNumber.Rows)
                            {
                                if (lintPreviousIBSHeaderID != Convert.ToInt32(ldr["ibs_header_id"]))
                                {
                                    if (ldr["summ_balance_forward"] != DBNull.Value)
                                        ldecTotalBalanceForward += Convert.ToDecimal(ldr["summ_balance_forward"]);
                                    if (ldr["remittance_balance_forward"] != DBNull.Value)
                                        ldecTotalRemittanceBalFwd += Convert.ToDecimal(ldr["remittance_balance_forward"]);
                                    if (ldr["adjustment_amount"] != DBNull.Value)
                                        ldecAdjustmentAmount += Convert.ToDecimal(ldr["adjustment_amount"]);
                                }
                                lintPreviousIBSHeaderID = Convert.ToInt32(ldr["ibs_header_id"]);
                            }
                        }
                        //should not subtract remittance balance forward since balance fwd is arrived after reducing remittance balance fwd from last month in IBS Billing batch
                        ldecTotalPremium += (ldecAdjustmentAmount + ldecTotalBalanceForward); 

                        if (ldecTotalPremium > 0.0M)
                        {
                            string lstrReferenceNo = string.Empty;
                            lstrReferenceNo = string.Concat("ACH Pull IBS ", Convert.ToString(DateTime.Today.ToString(busConstant.DateFormatMMddyyyy)));
                            lintDepositID = CreateDeposit(ldecTotalPremium, lobjIbsDetail.icdoIbsDetail.billing_month_and_year,
                            busConstant.DepositDetailStatusApplied, busConstant.DepositSourceRegularDeposits, lstrReferenceNo, aintPersonId: lobjIbsDetail.icdoIbsDetail.person_id);
                            ldecTotalDepositTapeAmount += ldecTotalPremium;

                            //Remittance Creation
                            lintRemittanceId = 0;
                            decimal ldecRemittanceAmount = 0.0M;
                            ldecRemittanceAmount = ldecTotalPremium;
                            //Insert Remittance
                            lintRemittanceId = CreateRemittance(lintDepositID, lobjIbsDetail.icdoIbsDetail.plan_id, ldecRemittanceAmount,
                                            busConstant.RemittanceTypeIBSDeposit, DateTime.Today,//uat pir 1756
                                            aintPersonId: lobjIbsDetail.icdoIbsDetail.person_id);

                            //Generate GL                            
                            CreateGL(lobjIbsDetail.icdoIbsDetail.plan_id, lintRemittanceId, ldecRemittanceAmount, busConstant.RemittanceTypeIBSDeposit,
                            aintPersonId: lobjIbsDetail.icdoIbsDetail.person_id);
                            
                            // Add to ACH file collection
                            busACHProviderReportData lobjProviderReportData = new busACHProviderReportData();
                            lobjProviderReportData.lintPERSLinkID = lobjIbsDetail.icdoIbsDetail.person_id;
                            lobjProviderReportData.ldclContributionAmount = ldecRemittanceAmount;
                            lobjProviderReportData.lstrDFIAccountNo = lobjACHDetail.icdoPersonAccountAchDetail.bank_account_number;
                            //We are taking routing number from bank organization
                            lobjProviderReportData.lstrRoutingNumber = string.IsNullOrEmpty(lobjOrganization.icdoOrganization.routing_no) ?
                                string.Empty : lobjOrganization.icdoOrganization.routing_no;
                            lobjProviderReportData.istrRoutingNumberFirstEightDigits = lobjProviderReportData.lstrRoutingNumber
                                .Substring(0, lobjProviderReportData.lstrRoutingNumber.Length - 1).PadLeft(8, '0');
                            lobjProviderReportData.istrCheckLastDigit = lobjProviderReportData.lstrRoutingNumber
                                                                                   .Substring(lobjProviderReportData.lstrRoutingNumber.Length - 1, 1);

                            if (lobjACHDetail.icdoPersonAccountAchDetail.bank_account_type_value == busConstant.PersonAccountBankAccountSavings)
                            {
                                lobjProviderReportData.lstrTransactionCode = busConstant.DebitTransactionCodeNonPrenoteSavings;
                            }
                            else if (lobjACHDetail.icdoPersonAccountAchDetail.bank_account_type_value == busConstant.PersonAccountBankAccountChecking)
                            {
                                lobjProviderReportData.lstrTransactionCode = busConstant.DebitTransactionCodeNonPrenoteChecking;
                            }

                            bool lblnIsRecordExists = false;
                            foreach (busACHProviderReportData lobjPrevRec in lclbACHPullFile.Where(o =>
                                o.lintPERSLinkID == lobjProviderReportData.lintPERSLinkID &&
                                   o.lstrDFIAccountNo == lobjProviderReportData.lstrDFIAccountNo &&
                                      o.lstrRoutingNumber == lobjProviderReportData.lstrRoutingNumber))
                            {
                                lobjPrevRec.ldclContributionAmount += lobjProviderReportData.ldclContributionAmount;
                                lblnIsRecordExists = true;
                            }
                            if (!lblnIsRecordExists)
                            {
                                lclbACHPullFile.Add(lobjProviderReportData);
                            }                            
                        }
                        lintPrevPersonID = lobjIbsDetail.icdoIbsDetail.person_id;
                        lstrPrevBankAccountNumber = lobjACHDetail.icdoPersonAccountAchDetail.bank_account_number.Trim();
                    }
                    if (lintDepositID != 0)
                    {
                        //Updating Ibs Detail with the deposit id
                        lobjIbsDetail.icdoIbsDetail.deposit_id = lintDepositID;                       
                        lobjIbsDetail.icdoIbsDetail.Update();                                          
                    }
                }
                lobjIbsDetail = null;
                lobjACHDetail = null;
                lobjIBSSummary = null;
            }
            // Create an Outbound ACH File
            UpdateDepositTapeStatusAndCreateFile(lclbACHPullFile, ldecTotalDepositTapeAmount, 8);

            DataTable ldtbPullACHError = busBase.Select("cdoIbsDetail.rptPullACHErrorReport", new object[0] { });
            if (ldtbPullACHError.Rows.Count > 0)
            {
                lobjNSBase.CreateReport("rptPullACHError.rpt", ldtbPullACHError, string.Empty);
            }
        }

        /// <summary>
        /// Method to create deposits, remittances and generate pull ach files for Employers
        /// </summary>
        /// <param name="astrHeaderTypeValue">Employer payroll header type value</param>
        public void GeneratePullACHForEmployers(string astrHeaderTypeValue)
        {
            DataTable ldtEmployerPayrollHeader = busBase.Select("cdoEmployerPayrollHeader.PullACHForEmployer", new object[1] { astrHeaderTypeValue });
            Collection<busACHProviderReportData> lclbACHPullFile = new Collection<busACHProviderReportData>();
            decimal ldecTotalAmount = 0.0M;

            foreach (DataRow dr in ldtEmployerPayrollHeader.Rows)
            {
                busWssDebitAchRequest lobjDebiACHRequest = new busWssDebitAchRequest();
                busEmployerPayrollHeader lobjEmployerHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                busOrgBank lobjOrgBank = new busOrgBank { icdoOrgBank = new cdoOrgBank() };
                lobjDebiACHRequest.FindWssDebitAchRequest(Convert.ToInt32(dr["DEBIT_ACH_REQUEST_ID"]));
                lobjEmployerHeader.icdoEmployerPayrollHeader.LoadData(dr);
                lobjOrgBank.icdoOrgBank.LoadData(dr);

                busACHProviderReportData lobjProviderReportData = new busACHProviderReportData();
                lobjProviderReportData.istrOrgCode = (dr["org_code"] == DBNull.Value ? "" : dr["org_code"].ToString()).PadRight(15, ' ');
                lobjProviderReportData.istrOrgName = (dr["org_name"] == DBNull.Value ? "" : dr["org_name"].ToString()).PadRight(16, ' ');
                if (lobjProviderReportData.istrOrgName.Length > 16)
                    lobjProviderReportData.istrOrgName = lobjProviderReportData.istrOrgName.Substring(0, 16);
                lobjProviderReportData.lstrDFIAccountNo = lobjOrgBank.icdoOrgBank.account_no;
                lobjProviderReportData.lstrRoutingNumber = dr["routing_no"] == DBNull.Value ? "0" : dr["routing_no"].ToString();
                lobjProviderReportData.istrRoutingNumberFirstEightDigits = lobjProviderReportData.lstrRoutingNumber
                    .Substring(0, lobjProviderReportData.lstrRoutingNumber.Length - 1).PadLeft(8, '0');
                lobjProviderReportData.istrCheckLastDigit = lobjProviderReportData.lstrRoutingNumber
                                                                       .Substring(lobjProviderReportData.lstrRoutingNumber.Length - 1, 1);

                if (astrHeaderTypeValue == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    //prod pir 6512
                    if (lobjEmployerHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                        lobjProviderReportData.ldclContributionAmount = CreateDepositRemittanceGLForRetirement(lobjEmployerHeader, lobjDebiACHRequest);
                    else if (lobjEmployerHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                        lobjProviderReportData.ldclContributionAmount = CreateDepositRemittanceGLForPurchase(lobjEmployerHeader, lobjDebiACHRequest);
                }
                else if (astrHeaderTypeValue == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    lobjProviderReportData.ldclContributionAmount = CreateDepositRemittanceGLForInsurance(lobjEmployerHeader, lobjDebiACHRequest);
                }
                else if (astrHeaderTypeValue == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    lobjProviderReportData.ldclContributionAmount = CreateDepositRemittanceGLForDeffComp(lobjEmployerHeader, lobjDebiACHRequest);
                }

                if (lobjOrgBank.icdoOrgBank.account_type_value == busConstant.BankAccountSavings)
                    lobjProviderReportData.lstrTransactionCode = busConstant.DebitTransactionCodeNonPrenoteSavings;
                else if (lobjOrgBank.icdoOrgBank.account_type_value == busConstant.BankAccountChecking)
                    lobjProviderReportData.lstrTransactionCode = busConstant.DebitTransactionCodeNonPrenoteChecking;
                ldecTotalAmount += lobjProviderReportData.ldclContributionAmount;
                lclbACHPullFile.Add(lobjProviderReportData);
                if (lobjDebiACHRequest.icdoWssDebitAchRequest.status_value != busConstant.WSSDebitBatchRequestProcessed)
                {
                    lobjDebiACHRequest.icdoWssDebitAchRequest.status_value = busConstant.WSSDebitBatchRequestProcessed;
                    lobjDebiACHRequest.icdoWssDebitAchRequest.Update();
                }
            }
            //18882 we filter the collection here before generating the out file - to exclude 0 amount -- need to confirm from maik if need to filter header type value
            lclbACHPullFile = lclbACHPullFile.Where(i => i.ldclContributionAmount > 0).ToList().ToCollection();
            UpdateDepositTapeStatusAndCreateFile(lclbACHPullFile, ldecTotalAmount, 75, astrHeaderTypeValue: astrHeaderTypeValue);
        }

        /// <summary>
        /// method to create deposits
        /// </summary>
        /// <param name="adecTotalPremium"></param>
        /// <param name="adtPaymentdate"></param>
        /// <param name="astrStatusValue"></param>
        /// <param name="astrDepositSource"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aintOrgId"></param>
        /// <returns></returns>
        public int CreateDeposit(decimal adecTotalPremium, DateTime adtPaymentdate, string astrStatusValue, string astrDepositSource, string astrReferenceNo, int aintPersonId = 0, int aintOrgId = 0)
        {
            busDeposit lobjDeposit = new busDeposit { icdoDeposit = new cdoDeposit() };
            // Insert Deposits
            lobjDeposit.icdoDeposit.deposit_tape_id = icdoDepositTape.deposit_tape_id;
            lobjDeposit.icdoDeposit.deposit_amount = adecTotalPremium;
            lobjDeposit.icdoDeposit.person_id = aintPersonId;
            lobjDeposit.icdoDeposit.org_id = aintOrgId;
            lobjDeposit.icdoDeposit.payment_date = adtPaymentdate;
            lobjDeposit.icdoDeposit.status_value = astrStatusValue;
            lobjDeposit.icdoDeposit.deposit_source_value = astrDepositSource;
            lobjDeposit.icdoDeposit.reference_no = astrReferenceNo;
            lobjDeposit.icdoDeposit.deposit_date = icdoDepositTape.deposit_tape_id != 0 ? icdoDepositTape.deposit_date : DateTime.Today;
            // TODO:Reference number not added yet.                            
            lobjDeposit.icdoDeposit.Insert();

            return lobjDeposit.icdoDeposit.deposit_id;
        }
        /// <summary>
        /// method to create remittances
        /// </summary>
        /// <param name="aintDepositId"></param>
        /// <param name="aintPlanId"></param>
        /// <param name="adecAmount"></param>
        /// <param name="astrRemittanceTypeValue"></param>
        /// <param name="adtAppliedDate"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aintOrgId"></param>
        /// <returns></returns>
        public int CreateRemittance(int aintDepositId, int aintPlanId, decimal adecAmount, string astrRemittanceTypeValue, DateTime adtAppliedDate,
            int aintPersonId = 0, int aintOrgId = 0)
        {
            //Insert Remittance
            busRemittance lobjRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
            lobjRemittance.icdoRemittance.deposit_id = aintDepositId;
            lobjRemittance.icdoRemittance.person_id = aintPersonId;
            lobjRemittance.icdoRemittance.plan_id = aintPlanId;
            lobjRemittance.icdoRemittance.remittance_amount = adecAmount;
            lobjRemittance.icdoRemittance.remittance_type_value = astrRemittanceTypeValue;
            lobjRemittance.icdoRemittance.org_id = aintOrgId;
            lobjRemittance.icdoRemittance.applied_date = adtAppliedDate;
            lobjRemittance.icdoRemittance.Insert();
            return lobjRemittance.icdoRemittance.remittance_id;
        }
        /// <summary>
        /// Method to generate gl
        /// </summary>
        /// <param name="aintPlanId"></param>
        /// <param name="aintRemittanceId"></param>
        /// <param name="adecAmount"></param>
        /// <param name="astrRemittanceType"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aintOrgId"></param>
        public void CreateGL(int aintPlanId, int aintRemittanceId, decimal adecAmount, string astrRemittanceType, int aintPersonId = 0, int aintOrgId = 0)
        {
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = aintPlanId;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeRemittance;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeStatusTransition;
            lcdoAcccountReference.item_type_value = astrRemittanceType;
            lcdoAcccountReference.status_transition_value = busConstant.StatusTransitionValidatedToApplied;

            busGLHelper.GenerateGL(lcdoAcccountReference,
                                        aintPersonId,
                                        aintOrgId,
                                        aintRemittanceId,
                                        adecAmount,
                                        DateTime.Now,
                                        icdoDepositTape.deposit_date, iobjPassInfo);
        }

        /// <summary>
        /// Method to update deposit tape with total amount and create the ach file
        /// </summary>
        /// <param name="aclbACHPullFile">collection containing data for file</param>
        /// <param name="adecAmount">Total deposit tape amount</param>
        /// <param name="aintFileID">File id</param>
        public void UpdateDepositTapeStatusAndCreateFile(Collection<busACHProviderReportData> aclbACHPullFile, decimal adecAmount, int aintFileID, string astrHeaderTypeValue = "")
        {
            if (aclbACHPullFile.Count > 0)
            {
                //Updating the status to Pull ACH complete in Deposit TapeadecAmount
                icdoDepositTape.pull_ach_status_value = busConstant.PullACHCompleteStatus;
                icdoDepositTape.status_value = busConstant.DepositTapeStatusValid;
                icdoDepositTape.total_amount = adecAmount;
                icdoDepositTape.Update();

                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                lobjProcessFiles.iarrParameters = new object[5];
                lobjProcessFiles.iarrParameters[0] = aclbACHPullFile;
                lobjProcessFiles.iarrParameters[1] = busConstant.Flag_Yes;
                lobjProcessFiles.iarrParameters[2] = busGlobalFunctions.GetData1ByCodeValue(icdoDepositTape.bank_account_id, icdoDepositTape.bank_account_value, iobjPassInfo);
                if (astrHeaderTypeValue == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    lobjProcessFiles.iarrParameters[3] = busConstant.ACHFileNameRetirmentEmployerPayment;
                }
                else if (astrHeaderTypeValue == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    lobjProcessFiles.iarrParameters[3] = busConstant.ACHFileNameInsuranceEmployerPayment;
                }
                else if (astrHeaderTypeValue == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    lobjProcessFiles.iarrParameters[3] = busConstant.ACHFileNameDefCompEmployerPayment;
                }
                else
                {
                    lobjProcessFiles.iarrParameters[3] = string.Empty;
                }
                lobjProcessFiles.iarrParameters[4] = icdoDepositTape.deposit_date;
                lobjProcessFiles.CreateOutboundFile(aintFileID);

            }
        }
        /// <summary>
        /// Method to create deposit and remittance for employer payroll header type retirement
        /// </summary>
        /// <param name="abusPayrollHeader">employer payroll bus. object</param>
        /// <param name="abusDebiACHRequest">debit ach request bus. object</param>
        /// <returns></returns>
        public decimal CreateDepositRemittanceGLForRetirement(busEmployerPayrollHeader abusPayrollHeader, busWssDebitAchRequest abusDebiACHRequest)
        {
            int lintDepositId1 = 0, lintDepositId2 = 0, lintDepositId3 = 0, lintRemittance1 = 0, lintRemittance2 = 0, lintRemittance3 = 0;
            decimal ldecTotalAmount = 0.0M, ldecTotalRhicContribution = 0.0M, ldecTotalADECAmount = 0.0M;
            abusPayrollHeader.LoadRetirementContributionByPlan();
            ldecTotalAmount = abusPayrollHeader.iclbRetirementContributionByPlan.Sum(o => o.idecOutstandingRetirementBalanceForACHPull);
            ldecTotalRhicContribution = abusPayrollHeader.iclbRetirementContributionByPlan.Sum(o => o.idecOutstandingRHICBalance);
            ldecTotalADECAmount = abusPayrollHeader.iclbRetirementContributionByPlan.Sum(o => o.idecOutstandingADECBalance);

            string lstrReferenceNo = string.Empty;
            lstrReferenceNo = string.Concat("ACH Pull Retirement ", Convert.ToString(DateTime.Today.ToString(busConstant.DateFormatMMddyyyy)));
            if (ldecTotalAmount > 0)
            {
                //prod pir 5808 : need to post today's date                
                lintDepositId1 = CreateDeposit(ldecTotalAmount,
                    DateTime.Today, busConstant.DepositDetailStatusApplied,
                    busConstant.DepositSourceRegularDeposits, lstrReferenceNo, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);
                abusDebiACHRequest.CreateDebitACHRequestDetails(lintDepositId1);
            }
            if (ldecTotalRhicContribution > 0)
            {
                //prod pir 5808 : need to post today's date
                lintDepositId2 = CreateDeposit(ldecTotalRhicContribution,
                    DateTime.Today, busConstant.DepositDetailStatusApplied,
                    busConstant.DepositSourceRegularDeposits, lstrReferenceNo, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);

                abusDebiACHRequest.CreateDebitACHRequestDetails(lintDepositId2);
            }
            if (ldecTotalADECAmount > 0)
            {
                //PIR 25920 - 1040 Changes : need to post today's date
                lintDepositId3 = CreateDeposit(ldecTotalADECAmount,
                    DateTime.Today, busConstant.DepositDetailStatusApplied,
                    busConstant.DepositSourceRegularDeposits, lstrReferenceNo, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);

                abusDebiACHRequest.CreateDebitACHRequestDetails(lintDepositId3);
            }
            foreach (busEmployerPayrollHeader lobjHeader in abusPayrollHeader.iclbRetirementContributionByPlan)
            {
                if (lintDepositId1 > 0 && lobjHeader.idecOutstandingRetirementBalanceForACHPull > 0)
                {
                    lintRemittance1 = CreateRemittance(lintDepositId1, lobjHeader.ibusPlan.icdoPlan.plan_id, lobjHeader.idecOutstandingRetirementBalanceForACHPull,
                        busConstant.ItemTypeContribution, DateTime.Today, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);//uat pir 1756
                    //Generate GL                            
                    CreateGL(lobjHeader.ibusPlan.icdoPlan.plan_id, lintRemittance1, lobjHeader.idecOutstandingRetirementBalanceForACHPull,
                        busConstant.ItemTypeContribution, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);
                }
                if (lintDepositId2 > 0 && lobjHeader.idecOutstandingRHICBalance > 0)
                {
                    lintRemittance2 = CreateRemittance(lintDepositId2, lobjHeader.ibusPlan.icdoPlan.plan_id, lobjHeader.idecOutstandingRHICBalance,
                        busConstant.ItemTypeRHICContribution, DateTime.Today, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);//uat pir 1756

                    //Generate GL                            
                    CreateGL(lobjHeader.ibusPlan.icdoPlan.plan_id, lintRemittance2, lobjHeader.idecOutstandingRHICBalance,
                        busConstant.ItemTypeRHICContribution, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);
                }
                if (lintDepositId3 > 0 && lobjHeader.idecOutstandingADECBalance > 0)
                {
                    lintRemittance3 = CreateRemittance(lintDepositId3, lobjHeader.ibusPlan.icdoPlan.plan_id, lobjHeader.idecOutstandingADECBalance,
                        busConstant.ItemTypeADECContribution, DateTime.Today, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);//uat pir 1756

                    //Generate GL                            
                    CreateGL(lobjHeader.ibusPlan.icdoPlan.plan_id, lintRemittance3, lobjHeader.idecOutstandingADECBalance,
                        busConstant.ItemTypeADECContribution, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);
                }
            }
            return ldecTotalAmount + ldecTotalRhicContribution + ldecTotalADECAmount;
        }
        /// <summary>
        /// Method to create deposit and remittance for employer payroll header type insurance
        /// </summary>
        /// <param name="abusPayrollHeader">employer payroll bus. object</param>
        /// <param name="abusDebiACHRequest">debit ach request bus. object</param>
        /// <returns></returns>
        public decimal CreateDepositRemittanceGLForInsurance(busEmployerPayrollHeader abusPayrollHeader, busWssDebitAchRequest abusDebiACHRequest)
        {
            int lintDepositId1 = 0, lintRemittance1 = 0;
            decimal ldecTotalAmount = 0.0M;
            string lstrRemittanceType = string.Empty;
            abusPayrollHeader.LoadInsurancePremiumByPlan();
            ldecTotalAmount = abusPayrollHeader.iclbInsuranceContributionByPlan.Sum(o => o.idecOutstandingInsuranceBalance);
            string lstrReferenceNo = string.Empty;
            lstrReferenceNo = string.Concat("ACH Pull Insurance ", Convert.ToString(DateTime.Today.ToString(busConstant.DateFormatMMddyyyy)));
            if (ldecTotalAmount > 0)
            {
                //prod pir 5808 : need to post today's date
                lintDepositId1 = CreateDeposit(ldecTotalAmount,
                   DateTime.Today, busConstant.DepositDetailStatusApplied,
                   busConstant.DepositSourceRegularDeposits, lstrReferenceNo, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);

                abusDebiACHRequest.CreateDebitACHRequestDetails(lintDepositId1);
            }
            foreach (busEmployerPayrollHeader lobjHeader in abusPayrollHeader.iclbInsuranceContributionByPlan)
            {
                lstrRemittanceType = string.Empty;
                if (lintDepositId1 == 0)
                    break;
                switch (lobjHeader.ibusPlan.icdoPlan.plan_id)
                {
                    case busConstant.PlanIdGroupHealth:
                        lstrRemittanceType = busConstant.ItemTypeGroupHealthDeposit;
                        break;
                    case busConstant.PlanIdGroupLife:
                        lstrRemittanceType = busConstant.ItemTypeGroupLifeDeposit;
                        break;
                    case busConstant.PlanIdVision:
                        lstrRemittanceType = busConstant.ItemTypeGroupVisionDeposit;
                        break;
                    case busConstant.PlanIdLTC:
                        lstrRemittanceType = busConstant.ItemTypeLTCDeposit;
                        break;
                    case busConstant.PlanIdMedicarePartD:
                        lstrRemittanceType = busConstant.ItemTypeMedicareDeposit;
                        break;
                    case busConstant.PlanIdDental:
                        lstrRemittanceType = busConstant.ItemTypeGroupDentalDeposit;
                        break;
                    case busConstant.PlanIdEAP:
                        lstrRemittanceType = busConstant.ItemTypeEAPDeposit;
                        break;
                }
                if (!string.IsNullOrEmpty(lstrRemittanceType))
                {
                    if (lobjHeader.idecOutstandingInsuranceBalance > 0)
                    {
                        lintRemittance1 = CreateRemittance(lintDepositId1, lobjHeader.ibusPlan.icdoPlan.plan_id, lobjHeader.idecOutstandingInsuranceBalance,
                            lstrRemittanceType, DateTime.Today, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);//uat pir 1756
                                                                                                                               //Generate GL                            
                        CreateGL(lobjHeader.ibusPlan.icdoPlan.plan_id, lintRemittance1, lobjHeader.idecOutstandingInsuranceBalance,
                            lstrRemittanceType, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);
                    }
                }
            }
            return ldecTotalAmount;
        }
        /// <summary>
        /// Method to create deposit and remittance for employer payroll header type def. comp
        /// </summary>
        /// <param name="abusPayrollHeader">employer payroll bus. object</param>
        /// <param name="abusDebiACHRequest">debit ach request bus. object</param>
        /// <returns></returns>
        public decimal CreateDepositRemittanceGLForDeffComp(busEmployerPayrollHeader abusPayrollHeader, busWssDebitAchRequest abusDebiACHRequest)
        {
            int lintDepositId1 = 0, lintRemittance1 = 0;
            abusPayrollHeader.LoadDeferredCompContributionByPlan();
            decimal ldecTotalAmount = 0.0M;
            //no need to include amounts for other 457 plan
            ldecTotalAmount = abusPayrollHeader.idecTotOutstandDefCompBal;
            if (ldecTotalAmount > 0)
            {
                string lstrReferenceNo = string.Empty;
                lstrReferenceNo = string.Concat("ACH Pull Def Comp ", Convert.ToString(DateTime.Today.ToString(busConstant.DateFormatMMddyyyy)));
                //prod pir 5808 : need to post today's date
                lintDepositId1 = CreateDeposit(ldecTotalAmount,
                   DateTime.Today, busConstant.DepositDetailStatusApplied,
                   busConstant.DepositSourceRegularDeposits, lstrReferenceNo, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);
                if (lintDepositId1 > 0)
                {
                    abusDebiACHRequest.CreateDebitACHRequestDetails(lintDepositId1);
                    //no need to include amounts for other 457 plan
                    lintRemittance1 = CreateRemittance(lintDepositId1, busConstant.PlanIdDeferredCompensation, ldecTotalAmount,
                    busConstant.ItemTypeDefeCompDeposit, DateTime.Today, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);//uat pir 1756
                    //Generate GL                            
                    CreateGL(busConstant.PlanIdDeferredCompensation, lintRemittance1, ldecTotalAmount,
                        busConstant.ItemTypeDefeCompDeposit, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);
                }                
            }
            return ldecTotalAmount;
        }
        /// <summary>
        /// Method to create deposit and remittance for employer payroll header type service purchase
        /// </summary>
        /// <param name="abusPayrollHeader">employer payroll bus. object</param>
        /// <param name="abusDebiACHRequest">debit ach request bus. object</param>
        /// <returns></returns>
        public decimal CreateDepositRemittanceGLForPurchase(busEmployerPayrollHeader abusPayrollHeader, busWssDebitAchRequest abusDebiACHRequest)
        {
            int lintDepositId1 = 0, lintRemittance1 = 0;
            abusPayrollHeader.LoadPurchaseByPlan();
            decimal ldecTotalAmount = 0.0M;
            ldecTotalAmount = abusPayrollHeader.iclbPurchaseContributionByPlan.Sum(o => o.idecOutstandingAmount);
            if (ldecTotalAmount > 0)
            {
                //prod pir 5808 : need to post today's date
                lintDepositId1 = CreateDeposit(ldecTotalAmount,
                   DateTime.Today, busConstant.DepositDetailStatusApplied,
                   busConstant.DepositSourceRegularDeposits, string.Empty, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);

                abusDebiACHRequest.CreateDebitACHRequestDetails(lintDepositId1);
            }
            foreach (busEmployerPayrollHeader lobjHeader in abusPayrollHeader.iclbPurchaseContributionByPlan)
            {
                if (lintDepositId1 == 0)
                    break;
                if (lobjHeader.idecOutstandingAmount > 0)
                {
                    lintRemittance1 = CreateRemittance(lintDepositId1, lobjHeader.ibusPlan.icdoPlan.plan_id, lobjHeader.idecOutstandingAmount,
                        busConstant.ItemTypePurchase, DateTime.Today, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);//uat pir 1756
                                                                                                                                     //Generate GL                            
                    CreateGL(lobjHeader.ibusPlan.icdoPlan.plan_id, lintRemittance1, lobjHeader.idecOutstandingAmount,
                        busConstant.ItemTypePurchase, aintOrgId: abusPayrollHeader.icdoEmployerPayrollHeader.org_id);
                }
            }
            return ldecTotalAmount;
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            SetVisibilityForPullACH();
        }
        //PIR # 20539 if Any changes made in below function, then need to make same changes in remittance allocation to Employer Header batch files as well
        private void DeleteInvalidAllocationAndUpdateEmpHeader(DataTable adtInvalidAllocations, bool ablnIsInsurance = false)
        {
            int lintEmployerPayrollHeaderID = 0;
            foreach (DataRow ldr in adtInvalidAllocations.Rows)
            {
                busEmployerRemittanceAllocation lobjRemittanceAllocation = new busEmployerRemittanceAllocation { icdoEmployerRemittanceAllocation = new cdoEmployerRemittanceAllocation() };
                lobjRemittanceAllocation.ibusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };

                lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.LoadData(ldr);
                lobjRemittanceAllocation.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldr);
                lobjEmpHeader.icdoEmployerPayrollHeader.LoadData(ldr);
                lobjEmpHeader.icdoEmployerPayrollHeader.created_by = ldr["HED_CREATED_BY"] == DBNull.Value ? iobjPassInfo.istrUserID : Convert.ToString(ldr["HED_CREATED_BY"]);
                lobjEmpHeader.icdoEmployerPayrollHeader.created_date = ldr["HED_CREATED_DATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(ldr["HED_CREATED_DATE"]);
                // iobjSystemManagement.icdoSystemManagement.batch_date : Convert.ToDateTime(ldr["HED_CREATED_DATE"]);//need to revert the commented code
                lobjEmpHeader.icdoEmployerPayrollHeader.modified_by = ldr["HED_MODIFIED_BY"] == DBNull.Value ? iobjPassInfo.istrUserID : Convert.ToString(ldr["HED_MODIFIED_BY"]);
                lobjEmpHeader.icdoEmployerPayrollHeader.modified_date = ldr["HED_MODIFIED_DATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(ldr["HED_MODIFIED_DATE"]);
                //  iobjSystemManagement.icdoSystemManagement.batch_date : Convert.ToDateTime(ldr["HED_MODIFIED_DATE"]);//need to revert the commented code
                lobjEmpHeader.icdoEmployerPayrollHeader.update_seq = ldr["HED_UPDATE_SEQ"] == DBNull.Value ? 0 : Convert.ToInt32(ldr["HED_UPDATE_SEQ"]);
                lobjEmpHeader.iintPlanID = ldr["iintPlanID"] == DBNull.Value ? 0 : Convert.ToInt32(ldr["iintPlanID"]);

                if (lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.difference_amount > 0 &&
                    !string.IsNullOrEmpty(lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.difference_type_value) && ablnIsInsurance) //As discussed on call with Maik, no GL here for retirement headers
                {
                    lobjRemittanceAllocation.CreateGLForDifferenceAmount(lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.difference_amount, lobjEmpHeader.iintPlanID);
                }
                lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.Delete();
                if (lintEmployerPayrollHeaderID != lobjEmpHeader.icdoEmployerPayrollHeader.employer_payroll_header_id)
                {
                    lobjEmpHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusUnbalanced;
                    lobjEmpHeader.icdoEmployerPayrollHeader.Update();
                }
                lintEmployerPayrollHeaderID = lobjEmpHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;

                lobjEmpHeader = null;
                lobjRemittanceAllocation = null;
            }
        }

        /// <summary>
        /// method to allocated amount for payroll header
        /// </summary>
        /// <param name="aobjHeader">Payroll header object</param>
        /// <param name="aobjHeaderByPlan">payroll header by plan</param>
        /// <param name="adtRemittance">available remittance datatable</param>
        /// <param name="adecTotalDueAmount">due amount</param>
		//PIR # 20539 if Any changes made in below function, then need to make same changes in remittance allocation to Employer Header batch files as well
        private void AllocateAmount(busEmployerPayrollHeader aobjHeader, busEmployerPayrollHeader aobjHeaderByPlan, DataTable adtRemittance, decimal adecTotalDueAmount)
        {
            decimal ldecAvailableAmount = 0.00M, ldecDifferenceAmount = 0.00M;
            string lstrDifferenceTypeValue = string.Empty;
            foreach (DataRow ldrRem in adtRemittance.Rows)
            {
                busRemittance lobjRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
                lobjRemittance.icdoRemittance.LoadData(ldrRem);

                ldecDifferenceAmount = 0.00M;
                lstrDifferenceTypeValue = string.Empty;
                //getting available amount for remittance
                ldecAvailableAmount = busEmployerReportHelper.GetRemittanceAvailableAmount(lobjRemittance.icdoRemittance.remittance_id);
                if (ldecAvailableAmount > 0)
                {
                    decimal ldecAllocatedAmount = ldecAvailableAmount;
                    if (ldecAllocatedAmount > adecTotalDueAmount)
                        ldecAllocatedAmount = adecTotalDueAmount;

                    //inserting remittance allocation
                    CreateRemittanceAllocation(aobjHeader.icdoEmployerPayrollHeader.employer_payroll_header_id, lobjRemittance.icdoRemittance.remittance_id,
                        ldecAllocatedAmount, Math.Abs(ldecDifferenceAmount), lstrDifferenceTypeValue);
                    //if (Math.Abs(ldecDifferenceAmount) > 0)
                    //{
                    //    aobjHeader.CreateGLForDifferenceAmount(lobjRemittance.icdoRemittance.remittance_type_value, ldecDifferenceAmount, aobjHeaderByPlan.iintPlanID);
                    //}
                    adecTotalDueAmount -= (ldecAllocatedAmount + (ldecDifferenceAmount < 0 ? Math.Abs(ldecDifferenceAmount) : 0.00M));
                    if (adecTotalDueAmount == 0.00M)
                        break;
                }

                lobjRemittance = null;
            }
        }

        /// <summary>
        /// method to insert new remittance allocation
        /// </summary>
        /// <param name="aintEmployerPayrollHeaderID">employer payroll header id</param>
        /// <param name="aintRemittanceID">remittance id</param>
        /// <param name="adecAllocatedAmount">amount allocated</param>
        /// <param name="adecDifferenceAmount">difference amount</param>
        /// <param name="astrDifferenceTypeValue">difference type value</param>
		//PIR # 20539 if Any changes made in below function, then need to make same changes in remittance allocation to Employer Header batch files as well
        private void CreateRemittanceAllocation(int aintEmployerPayrollHeaderID, int aintRemittanceID, decimal adecAllocatedAmount,
            decimal adecDifferenceAmount, string astrDifferenceTypeValue)
        {
            cdoEmployerRemittanceAllocation lcdoRemittanceAllocation = new cdoEmployerRemittanceAllocation();
            lcdoRemittanceAllocation.employer_payroll_header_id = aintEmployerPayrollHeaderID;
            lcdoRemittanceAllocation.remittance_id = aintRemittanceID;
            lcdoRemittanceAllocation.allocated_amount = adecAllocatedAmount;
            lcdoRemittanceAllocation.payroll_allocation_status_value = busConstant.Allocated;
            //lcdoRemittanceAllocation.allocated_date = iobjSystemManagement.icdoSystemManagement.batch_date; //need to revert the commented code
            lcdoRemittanceAllocation.difference_amount = adecDifferenceAmount;
            lcdoRemittanceAllocation.difference_type_value = astrDifferenceTypeValue;
            //PIR-20539 
            lcdoRemittanceAllocation.created_by = iobjPassInfo.istrUserID;
            lcdoRemittanceAllocation.modified_by = iobjPassInfo.istrUserID;
            lcdoRemittanceAllocation.created_date = DateTime.Now;
            lcdoRemittanceAllocation.modified_date = DateTime.Now;
            lcdoRemittanceAllocation.Insert();
        }


        public decimal idecAllowableVariance { get; set; }
        public busSystemManagement iobjSystemManagement;

        //PIR # 20539 if Any changes made in below function, then need to make same changes in remittance allocation to Employer Header batch files as well
        public void AllocateRemittanceToEmployerHeader(int aintBatchScheduleId=0)
        {
            foreach (int iintOrgID in iclbDeposit.Select(i => i.icdoDeposit.org_id).Distinct())
            {
                busEmployerPayrollHeader lobjEmployerPayrollHeader = new busEmployerPayrollHeader() { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() { org_id = iintOrgID } };
                lobjEmployerPayrollHeader.AllocateRemittanceToEmployerPayroll(istrHeaderType, aintBatchScheduleId);
            }
        }

        //ACH Pull Automation
        public void CreateDepositTape(string astrHeaderType)
        {
            icdoDepositTape.deposit_date = busGlobalFunctions.GetSysManagementBatchDate();
            icdoDepositTape.status_value = busConstant.StatusValid;
            icdoDepositTape.deposit_method_value = busConstant.DepositTapeMethodACHPull;

            if (astrHeaderType == busConstant.PayrollHeaderBenefitTypeRtmt)
                icdoDepositTape.bank_account_value = busConstant.DepositTapeBankAccountRetirement;
            else if (astrHeaderType == busConstant.PayrollHeaderBenefitTypeInsr)
                icdoDepositTape.bank_account_value = busConstant.DepositTapeBankAccountInsurance;
            else if (astrHeaderType == busConstant.PayrollHeaderBenefitTypeDefComp)
                icdoDepositTape.bank_account_value = busConstant.DepositTapeBankAccountDeferredComp;

            icdoDepositTape.total_amount = 0;
            icdoDepositTape.pull_ach_status_value = busConstant.PullACHReadyStatus;

            icdoDepositTape.Insert();           
        }

        public busBpmRequest InitiateWorkflowForDebitACHRequest(int aintTypeID)
        {
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            ldctParams["org_code"] = busConstant.DebitACHRequestWorkflowOrgCode;
            //ldctParams["additional_parameter1"] = aintEmployerPayrollHeaderID;

            return busWorkflowHelper.InitiateBpmRequest(aintTypeID, 0, 0, 0, iobjPassInfo, busConstant.WorkflowProcessSource_Batch, adictInstanceParameters: ldctParams);
        }
    }
}
