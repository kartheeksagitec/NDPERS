#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.Common;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitDroApplication : busBenefitDroApplicationGen
    {
        private decimal _idecMemberAgeBasedOnGivenDate;

        //Property to contain business object for Calculating monthly taxable and non taxable amounts for alt payee
        public busBenefitPostRetirementDROCalculation ibusBenefitPostRetirementDROCalculation { get; set; }

        //Property to contain payee account for member
        public busPayeeAccount ibusMemberPayeeAccount { get; set; }

        //Property to store Updated Monthly Amount
        public decimal idecUpdatedMonthlyAmount { get; set; }

        //Property to store Gross monthly amount for Alternate payee
        public decimal idecAPGrossMonthlyAmount { get; set; }

        public decimal idecMemberAgeBasedOnGivenDate
        {
            get { return _idecMemberAgeBasedOnGivenDate; }
            set { _idecMemberAgeBasedOnGivenDate = value; }
        }
        public string istrIsBenefitRecieptNotNull
        {
            get
            {
                if (icdoBenefitDroApplication.benefit_receipt_date != DateTime.MinValue)
                    return busConstant.Flag_Yes;
                else
                    return busConstant.Flag_No;
            }
        }

        public bool LoadMemberPayeeAccount()
        {
            bool lblnResult = false;
            DataTable ldtPayeeAccount = Select("cdoBenefitDroApplication.LoadMemberReceivingPayeeAccount",
                new object[2] { icdoBenefitDroApplication.member_perslink_id, icdoBenefitDroApplication.plan_id });
            if (ibusMemberPayeeAccount == null)
                ibusMemberPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            if (ldtPayeeAccount.Rows.Count > 0)
            {
                ibusMemberPayeeAccount.icdoPayeeAccount.LoadData(ldtPayeeAccount.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool LoadMemberPayeeAccountUpdateMode()
        {
            bool lblnResult = false;
            DataTable ldtPayeeAccount = Select<cdoPayeeAccount>(new string[1] { enmPayeeAccount.payee_perslink_id.ToString() },
                new object[1] { icdoBenefitDroApplication.member_perslink_id }, null, null);
            DataTable ldtMemberPayeeAccounts = ldtPayeeAccount.AsEnumerable()
                                                                .Where(o => o.Field<string>("account_relation_value") == busConstant.AccountRelationshipMember
                                                                            && o.Field<string>("benefit_account_type_value") != busConstant.ApplicationBenefitTypeRefund
                                                                            && o.Field<string>("benefit_option_value") != busConstant.BenefitOptionAutoRefund
                                                                            && o.Field<string>("benefit_option_value") != busConstant.BenefitOptionRefund
                                                                            && o.Field<string>("benefit_option_value") != busConstant.BenefitOptionRegularRefund)
                                                                .AsDataTable();
            ibusMemberPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            Collection<busPayeeAccount> lclbMemberPayeeAccounts = GetCollection<busPayeeAccount>(ldtMemberPayeeAccounts, "icdoPayeeAccount");
            lclbMemberPayeeAccounts.ForEach(payeeaccount => payeeaccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate());
            busPayeeAccount lbusPayeeAccount = lclbMemberPayeeAccounts.FirstOrDefault(payeeaccount => (payeeaccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 != busConstant.PayeeAccountStatusCancelled &&
                                                                                                        payeeaccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 != busConstant.PayeeAccountStatusPaymentComplete));
            if (lbusPayeeAccount.IsNotNull())
            {
                ibusMemberPayeeAccount = lbusPayeeAccount;
                lblnResult = true;
            }
            return lblnResult;
        }

        public ArrayList btnDenyClicked()
        {
            ArrayList lalReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (iobjPassInfo.istrUserID != icdoBenefitDroApplication.created_by)
            {
                //busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Cancel, busConstant.ActivityStatusCancelled, iobjPassInfo);

                icdoBenefitDroApplication.dro_status_value = busConstant.DROApplicationStatusDenied;
                icdoBenefitDroApplication.Update();
            }
            lalReturn.Add(this);
            return lalReturn;
        }
        private void SetApprovalInfo()
        {
            icdoBenefitDroApplication.dro_status_value = busConstant.DROApplicationStatusApproved;
            icdoBenefitDroApplication.approved_by_user = iobjPassInfo.istrUserID;
            icdoBenefitDroApplication.approved_date = DateTime.Today;
        }
        public ArrayList btnApproveCopyClicked()
        {
            ArrayList lalReturn = new ArrayList();
            utlError lobjError = new utlError();
            ValidateHardErrors(utlPageMode.Update);
            if (iarrErrors.Count == 0)
            {
                SetApprovalInfo();
                CalculateContributionSummary();
                icdoBenefitDroApplication.Update();
                lalReturn.Add(this);
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    lalReturn.Add(larr);
                }
            }
            return lalReturn;
        }
        public ArrayList btnCancelClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            //UAT PIR - 1298  : same user should be able to cancel the dro application
            //if (iobjPassInfo.istrUserID != icdoBenefitDroApplication.created_by)
            //{
            icdoBenefitDroApplication.dro_status_value = busConstant.DROApplicationStatusCancelled;
            icdoBenefitDroApplication.Update();

            //busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Cancel, busConstant.ActivityStatusCancelled, iobjPassInfo);
            //}
            alReturn.Add(this);
            return alReturn;
        }

        public ArrayList btnPendingNullifyClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            //uat pir - 1303
            if (ibusSoftErrors != null)
            {
                ibusSoftErrors.DeleteErrors();
            }

            icdoBenefitDroApplication.dro_status_value = busConstant.DROApplicationStatusPendingNullified;
            icdoBenefitDroApplication.pending_nullification_by = iobjPassInfo.istrUserID;
            icdoBenefitDroApplication.pending_nullification_date = DateTime.Today;
            icdoBenefitDroApplication.Update();

            ChangeAlternatePayeeStatus(true, icdoBenefitDroApplication.pending_nullification_date);

            alReturn.Add(this);
            return alReturn;
        }

        public ArrayList btnCancelNullifyClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            DateTime ldtPendingNullificationDate = icdoBenefitDroApplication.pending_nullification_date;

            icdoBenefitDroApplication.dro_status_value = busConstant.DROApplicationStatusQualified;
            icdoBenefitDroApplication.pending_nullification_by = null;
            icdoBenefitDroApplication.pending_nullification_date = DateTime.MinValue;
            icdoBenefitDroApplication.Update();

            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            busPayeeAccount lobjAltPayeeAccount =
                iclbPayeeAccount.Where(o => o.icdoPayeeAccount.payee_perslink_id == ibusAlternatePayee.icdoPerson.person_id).FirstOrDefault();

            if (lobjAltPayeeAccount != null)
            {
                if (lobjAltPayeeAccount.iclbPayeeAccountStatus == null)
                    lobjAltPayeeAccount.LoadPayeeAccountStatus();
                IEnumerable<busPayeeAccountStatus> lenuPayeeAccountStatus = lobjAltPayeeAccount.iclbPayeeAccountStatus
                                                                            .Where(o => o.icdoPayeeAccountStatus.status_effective_date >= ldtPendingNullificationDate);
                foreach (busPayeeAccountStatus lobjPayeeAccountStatus in lenuPayeeAccountStatus)
                    lobjPayeeAccountStatus.icdoPayeeAccountStatus.Delete();
            }
            alReturn.Add(this);
            return alReturn;
        }

        public ArrayList btnNullifyClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            icdoBenefitDroApplication.dro_status_value = busConstant.DROApplicationStatusNullified;
            icdoBenefitDroApplication.Update();
            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            busPayeeAccount lobjPayeeAccount = iclbPayeeAccount
                .Where(o => o.icdoPayeeAccount.payee_perslink_id == ibusAlternatePayee.icdoPerson.person_id).FirstOrDefault();
            //If nullify button is clicked without approving DRO calculation(ie no Payee acct for alternate payee)            
            if (lobjPayeeAccount != null && lobjPayeeAccount.icdoPayeeAccount != null && lobjPayeeAccount.icdoPayeeAccount.payee_account_id > 0 &&
                icdoBenefitDroApplication.dro_model_value != busConstant.DROApplicationModelDCModel &&
                icdoBenefitDroApplication.dro_model_value != busConstant.DROApplicationModelDeferredCompModel)
                NullifyActions();
            alReturn.Add(this);
            return alReturn;
        }

        public bool IsAlternatePayeePERSLinkIDValid()
        {
            if (icdoBenefitDroApplication.alternate_payee_perslink_id > 0)
            {
                if (!ibusAlternatePayee.FindPerson(icdoBenefitDroApplication.alternate_payee_perslink_id))
                {
                    return true;
                }
            }
            return false;
        }

        //BR - 08  if DRO Application Model is Active DB Former Model and received date is greater than PERSLink Go live date then throw an error
        public bool IsRecievedDateValidDate()
        {
            if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBFormerModel)
            {
                DateTime ldtPersLinkGoLiveDate = busPayeeAccountHelper.GetPERSLinkGoLiveDate();
                if ((ldtPersLinkGoLiveDate != DateTime.MinValue) && (icdoBenefitDroApplication.received_date > ldtPersLinkGoLiveDate))
                {
                    return true;
                }
            }
            return false;
        }
        //BR-09 Check wether Refund Benefit Application exists ,if so throw an error
        public bool IsBenefitRefundApplicationExist()
        {
            if (iclbBenefitRefundApplication == null)
                LoadRefundApplication();
            bool lblnRefundApplicationExists = false;
            //uat pir 1346 : no check to be done if disablilty member (check for application not in cancelled status)
            if (iclbBenefitRefundApplication.Where(o => o.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability &&
                                                        o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled).Any())
            {
                return false;
            }
            //PIR 1524
            foreach (busBenefitRefundApplication lbusBenefitRefundApplication in iclbBenefitRefundApplication)
            {
                if (!lbusBenefitRefundApplication.IsApplicationCancelledOrDenied())
                {
                    if (lbusBenefitRefundApplication.iclbBenefitApplicationPersonAccounts == null)
                        lbusBenefitRefundApplication.LoadBenefitApplicationPersonAccount();
                    if (lbusBenefitRefundApplication.iclbBenefitApplicationPersonAccounts.Where(o =>
                        o.icdoBenefitApplicationPersonAccount.person_account_id == icdoBenefitDroApplication.person_account_id &&
                        o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).Count() > 0)
                    {
                        lblnRefundApplicationExists = true;
                        break;
                    }
                }
            }
            busBenefitRefundApplication lobjRetirementApplication = iclbBenefitRefundApplication
                                                                .Where(o => o.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement
                                                                    && o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied
                                                                    && o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled).FirstOrDefault();
            if (lblnRefundApplicationExists)
            {
                if (lobjRetirementApplication != null)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
        //Calculate Contribution amount based on the divorce date and approved date
        public void CalculateContributionSummary()
        {
            decimal ldecPreTaxEEamount = 0.0M, ldecPostTaxEEAmount = 0.0M, ldecEEERPickupAmount = 0.0M, ldecInterest = 0.0M,
                 ldecERVestedAmount = 0.0M, ldecCapitalGain = 0.0M;
            if ((icdoBenefitDroApplication.approved_date != DateTime.MinValue) && (icdoBenefitDroApplication.received_date != DateTime.MinValue) &&
                icdoBenefitDroApplication.date_of_divorce != DateTime.MinValue)
            {
                //prod pir 1574:need to calculate amounts as of last day of month of divorce date for active db models and 
                //as of divorce date for retiree models
                DateTime ldtDivorceDate = icdoBenefitDroApplication.date_of_divorce.GetLastDayofMonth();
                if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel ||
                icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel)
                {
                    ldtDivorceDate = icdoBenefitDroApplication.date_of_divorce;
                }
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.ibusPersonAccountRetirement == null)
                    ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
                ibusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
                ldecCapitalGain = ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain;
                if (ibusPersonAccount.ibusPersonAccountRetirement.iclbRetirementContributionAll == null)
                    ibusPersonAccount.ibusPersonAccountRetirement.LoadRetirementContributionAll();
                foreach (busPersonAccountRetirementContribution lobjRetirementContribution in ibusPersonAccount.ibusPersonAccountRetirement.iclbRetirementContributionAll)
                {
                    //prod pir 1574:need to calculate amounts as of last day of month of divorce date for active db models and 
                    //as of divorce date for retiree models
                    if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date <= ldtDivorceDate)
                    {
                        ldecPreTaxEEamount += lobjRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_ee_amount;
                        ldecPostTaxEEAmount += lobjRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_ee_amount;
                        ldecEEERPickupAmount += lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_amount;
                        ldecInterest += lobjRetirementContribution.icdoPersonAccountRetirementContribution.interest_amount;
                        ldecERVestedAmount += lobjRetirementContribution.icdoPersonAccountRetirementContribution.er_vested_amount;
                    }
                }
                icdoBenefitDroApplication.computed_ee_er_pickup_amount = ldecEEERPickupAmount;
                icdoBenefitDroApplication.computed_ee_post_tax_amount = ldecPostTaxEEAmount;
                icdoBenefitDroApplication.computed_ee_pre_tax_amount = ldecPreTaxEEamount;
                icdoBenefitDroApplication.computed_er_vested_amount = ldecERVestedAmount;
                icdoBenefitDroApplication.computed_interest_amount = ldecInterest;
                icdoBenefitDroApplication.computed_capital_gain = ldecCapitalGain;
            }
        }
        //Check whether entered percentage is valid or not . BR - 27
        public bool IsPercentageValid()
        {
            if ((icdoBenefitDroApplication.monthly_benefit_percentage < 0) || (icdoBenefitDroApplication.monthly_benefit_percentage > 100.0M))
            {
                return true;
            }
            if ((icdoBenefitDroApplication.member_withdrawal_percentage < 0) || (icdoBenefitDroApplication.member_withdrawal_percentage > 100.0M))
            {
                return true;
            }
            if ((icdoBenefitDroApplication.lumpsum_payment_percentage < 0) || (icdoBenefitDroApplication.lumpsum_payment_percentage > 100.0M))
            {
                return true;
            }
            if ((icdoBenefitDroApplication.member_death_percentage < 0) || (icdoBenefitDroApplication.member_death_percentage > 100.0M))
            {
                return true;
            }
            if ((icdoBenefitDroApplication.alternate_payee_death_percentage < 0) || (icdoBenefitDroApplication.alternate_payee_death_percentage > 100.0M))
            {
                return true;
            }
            return false;
        }
        //PIR 1329
        public bool IsRecievedDateValid()
        {
            if (icdoBenefitDroApplication.received_date != DateTime.MinValue)
            {
                if (icdoBenefitDroApplication.received_date.Year < 1990)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsAmountValid()
        {
            if (icdoBenefitDroApplication.overridden_ee_er_pickup_amount < 0)
            {
                return true;
            }
            if (icdoBenefitDroApplication.overridden_capital_gain < 0)
            {
                return true;
            }
            if (icdoBenefitDroApplication.overridden_ee_post_tax_amount < 0)
            {
                return true;
            }
            if (icdoBenefitDroApplication.overridden_ee_pre_tax_amount < 0)
            {
                return true;
            }
            if (icdoBenefitDroApplication.overridden_er_vested_amount < 0)
            {
                return true;
            }
            if (icdoBenefitDroApplication.overridden_interest_amount < 0)
            {
                return true;
            }
            if (icdoBenefitDroApplication.monthly_benefit_amount < 0)
            {
                return true;
            }
            if (icdoBenefitDroApplication.member_withdrawal_amount < 0)
            {
                return true;
            }
            if (icdoBenefitDroApplication.lumpsum_payment_amount < 0)
            {
                return true;
            }
            return false;
        }
        //get Early retirment date based on the early retirement eligibility
        public bool IsBenefitReceiptDateValid()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPlan == null)
                LoadPlan();

            if (icdoBenefitDroApplication.time_of_benefit_receipt_calc_value == busConstant.DROApplicationModelTimeOfBenfitUserEnteredDate)
            {
                if (idtEarlyRetirementDate == DateTime.MinValue)
                    idtEarlyRetirementDate = GetEarlyRetirementDate();
                if (icdoBenefitDroApplication.benefit_receipt_date != DateTime.MinValue)
                {
                    DateTime ldtFirstOfMonth = new DateTime(icdoBenefitDroApplication.benefit_receipt_date.Year, icdoBenefitDroApplication.benefit_receipt_date.Month, 1);
                    if ((icdoBenefitDroApplication.benefit_receipt_date < idtEarlyRetirementDate) || (icdoBenefitDroApplication.benefit_receipt_date != ldtFirstOfMonth))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsBenefitReceiptDateNotFirstOfMonth()
        {
            if (icdoBenefitDroApplication.benefit_receipt_date != DateTime.MinValue)
            {
                if (icdoBenefitDroApplication.benefit_receipt_date.Day != 1)
                {
                    return true;
                }
            }
            return false;
        }
        //this code has been modified in order to incorporate the changes in  source of eligibility data
        public DateTime GetEarlyRetirementDate()
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();

            DateTime ldtEarlyRetirementDate = GetEarlyRetirementDateBasedOnEarlyRetirement(icdoBenefitDroApplication.plan_id, ibusPlan.icdoPlan.benefit_provision_id,
                                                                            busConstant.ApplicationBenefitTypeRetirement, ibusMember.icdoPerson.date_of_birth, icdoBenefitDroApplication.idecTVSC, iobjPassInfo, ibusPersonAccount);
            return ldtEarlyRetirementDate;
        }

        public void SetAge(DateTime adtDateToCompare, ref decimal ldecMemberAge)
        {
            int lintMonths = 0, lintAgeMonths = 0, lintAgeYear = 0;
            if (ldecMemberAge == 0.00M)
            {
                if (ibusMember == null)
                    LoadMember();
                CalculateAge(ibusMember.icdoPerson.date_of_birth.AddMonths(1), adtDateToCompare, ref lintMonths, ref ldecMemberAge, 2, ref lintAgeYear, ref lintAgeMonths);
            }
        }
        //get alternate payee age
        //for RMD calculation
        public void SetAPAge(DateTime adtDateToCompare, ref decimal ldecAlternatePayeeAge)
        {
            int lintMonths = 0, lintAgeMonths = 0, lintAgeYear = 0;
            if (ldecAlternatePayeeAge == 0.00M)
            {
                if (ibusAlternatePayee == null)
                    LoadAlternatePayee();
                CalculateAge(ibusAlternatePayee.icdoPerson.date_of_birth.AddMonths(1), adtDateToCompare, ref lintMonths, ref ldecAlternatePayeeAge, 2, ref lintAgeYear, ref lintAgeMonths);
            }
        }
        //this code has been modified in order to incorporate the changes in  source of eligibility data
        public bool IsPersonEligibleForEarly()
        {
            decimal ldecMemberAge = 0.00M;
            string lstrEarlyReducedwaivedFlag = String.Empty;

            if (ibusPlan == null)
                LoadPlan();

            if (icdoBenefitDroApplication.idecTVSC == 0.0M)
                GetRoundedTVSC();

            if (idecMemberAgeBasedOnGivenDate == 0)
                SetAge(icdoBenefitDroApplication.received_date, ref ldecMemberAge);
            idecMemberAgeBasedOnGivenDate = ldecMemberAge;

            if (idtNormalRetirementDate == DateTime.MinValue)
                SetNormalRetirementDate();

            bool lblnIsPersonEligibleForEarly = CheckISPersonEligibleForEarly(icdoBenefitDroApplication.plan_id, ibusPlan.icdoPlan.benefit_provision_id,
                                                busConstant.ApplicationBenefitTypeRetirement, idecMemberAgeBasedOnGivenDate, icdoBenefitDroApplication.idecTVSC,
                                                ref lstrEarlyReducedwaivedFlag, idtNormalRetirementDate, ibusPersonAccount, iobjPassInfo,false);
            return lblnIsPersonEligibleForEarly;
        }

        public DateTime GetRetirementDate()
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusMember.iclbBenefitApplication == null)
                ibusMember.LoadBenefitApplication();
            foreach (busBenefitApplication lobjbenefiapplication in ibusMember.iclbBenefitApplication)
            {
                //UAT pir - 1348 :- intead of checking application in verified status, need to check for approved status in benefit calculation
                if ((lobjbenefiapplication.icdoBenefitApplication.member_person_id == icdoBenefitDroApplication.member_perslink_id) &&
                    (lobjbenefiapplication.icdoBenefitApplication.plan_id == icdoBenefitDroApplication.plan_id) &&
                    //(lobjbenefiapplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified) &&
                    (lobjbenefiapplication.icdoBenefitApplication.benefit_account_type_value != busConstant.ApplicationBenefitTypeRefund))
                {
                    busBenefitCalculation lobjCalculation = new busBenefitCalculation();
                    lobjCalculation.FindBenefitCalculationByApplication(lobjbenefiapplication.icdoBenefitApplication.benefit_application_id);
                    if (lobjCalculation.icdoBenefitCalculation.action_status_value == busConstant.CalculationStatusApproval)
                        return lobjbenefiapplication.icdoBenefitApplication.retirement_date;
                    else
                        return DateTime.MinValue;
                }
            }
            return DateTime.MinValue;
        }

        public void SetBenefitReceiptDate()
        {
            if (icdoBenefitDroApplication.time_of_benefit_receipt_calc_value == busConstant.DROApplicationModelTimeOfBenfitRetirementDate)
            {
                icdoBenefitDroApplication.benefit_receipt_date = DateTime.MinValue;
                DateTime ldtDateTime = GetRetirementDate();
                if (ldtDateTime != DateTime.MinValue)
                {
                    icdoBenefitDroApplication.benefit_receipt_date = new DateTime(ldtDateTime.AddMonths(1).Year, ldtDateTime.AddMonths(1).Month, 1);
                    idtRetirementDate = icdoBenefitDroApplication.benefit_receipt_date;
                }
            }
            else if (icdoBenefitDroApplication.time_of_benefit_receipt_calc_value == busConstant.DROApplicationModelTimeOfBenfitNormalRetirementDate)
            {
                icdoBenefitDroApplication.benefit_receipt_date = DateTime.MinValue;
                SetNormalRetirementDate();
                DateTime ldtDateTime = idtNormalRetirementDate;
                if (ldtDateTime != DateTime.MinValue)
                {
                    icdoBenefitDroApplication.benefit_receipt_date = new DateTime(ldtDateTime.AddMonths(1).Year, ldtDateTime.AddMonths(1).Month, 1);
                }
            }
            else if (icdoBenefitDroApplication.time_of_benefit_receipt_calc_value == busConstant.DROApplicationModelTimeOfBenfitEarlyRetirementDate)
            {
                icdoBenefitDroApplication.benefit_receipt_date = DateTime.MinValue;
                DateTime ldtDateTime = GetEarlyRetirementDate();
                if (ldtDateTime != DateTime.MinValue)
                {
                    icdoBenefitDroApplication.benefit_receipt_date = new DateTime(ldtDateTime.AddMonths(1).Year, ldtDateTime.AddMonths(1).Month, 1);
                    idtEarlyRetirementDate = icdoBenefitDroApplication.benefit_receipt_date;
                }
            }
        }
        public bool IsAccountOwnersRetirementValid()
        {
            if ((iblnApproveOrQualifyButtonClick) || (iblnSaveClick))
            {
                if (icdoBenefitDroApplication.time_of_benefit_receipt_calc_value == busConstant.DROApplicationModelTimeOfBenfitRetirementDate)
                {
                    if (icdoBenefitDroApplication.benefit_receipt_date == DateTime.MinValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //this code has been modified in order to incorporate the changes in  source of eligibility data
        private void SetNormalRetirementDate()
        {
            if (icdoBenefitDroApplication.idecTVSC == 0.0M)
                icdoBenefitDroApplication.idecTVSC = GetRoundedTVSC();
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            //get normal retirement date based on the normal retirement eligibility
            idtNormalRetirementDate = GetNormalRetirementDateBasedOnNormalEligibilityForDRO(icdoBenefitDroApplication.plan_id, ibusPlan.icdoPlan.plan_code, ibusPlan.icdoPlan.benefit_provision_id,
                                     busConstant.ApplicationBenefitTypeRetirement, ibusMember.icdoPerson.date_of_birth, icdoBenefitDroApplication.idecTVSC, iobjPassInfo, ibusPersonAccount);
        }

        public bool iblnIsMonthlyAmountInvalid { get; set; }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            SetBenefitReceiptDate();
            switch (iobjPassInfo.istrPostBackControlID)
            {
                case "btnApproved":
                    iblnApproveOrQualifyButtonClick = true;
                    break;
                case "btnQualified":
                    iblnApproveOrQualifyButtonClick = true;
                    //uat pir - 1873
                    if (icdoBenefitDroApplication.date_of_divorce != DateTime.MinValue &&
                        (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel ||
                           icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel))
                    {
                        if (ibusBenefitPostRetirementDROCalculation == null)
                            ibusBenefitPostRetirementDROCalculation = new busBenefitPostRetirementDROCalculation();
                        DateTime ldtPERSLinkLive = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.PERSLinkGoLiveDate, iobjPassInfo));
                        DateTime ldtDivorceDate = new DateTime(icdoBenefitDroApplication.date_of_divorce.Year, icdoBenefitDroApplication.date_of_divorce.Month, 1);
                        ibusBenefitPostRetirementDROCalculation.CalculateGrossMonthlyAmount(ldtDivorceDate > ldtPERSLinkLive ? ldtDivorceDate :
                        ldtPERSLinkLive, ibusMemberPayeeAccount);
                        icdoBenefitDroApplication.computed_member_gross_monthly_amount = ibusBenefitPostRetirementDROCalculation.idecGrossMonthlyAmount;
                        decimal ldecGrossMonthlyAmount = icdoBenefitDroApplication.overridden_member_gross_monthly_amount > 0 ?
                            icdoBenefitDroApplication.overridden_member_gross_monthly_amount : icdoBenefitDroApplication.computed_member_gross_monthly_amount;
                        if (icdoBenefitDroApplication.monthly_benefit_amount > ldecGrossMonthlyAmount)
                            iblnIsMonthlyAmountInvalid = true;
                        else
                            iblnIsMonthlyAmountInvalid = false;
                    }
                    break;
                case "btnSave":
                    if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusApproved)
                    {
                        iblnSaveClick = true;
                    }
                    iblnIntialSaveClick = true;
                    break;
                default:
                    iblnApproveOrQualifyButtonClick = false;
                    iblnIntialSaveClick = false;
                    iblnSaveClick = false;
                    break;
            }
            base.BeforeValidate(aenmPageMode);
        }
        public void InitiateDROWorkflow()
        {
            bool lblnInitiateWorkflow = false;
            DataTable ldtRunningInstance = busWorkflowHelper.LoadRunningInstancesByPersonAndProcess(icdoBenefitDroApplication.alternate_payee_perslink_id,
                                            busConstant.Map_Process_QDRO_Calculation);
            if (ldtRunningInstance.Rows.Count == 0)
            {
                if (ibusBenefitDroCalculation == null)
                {
                    ibusBenefitDroCalculation = new busBenefitDroCalculation();
                    ibusBenefitDroCalculation.FindBenefitDroCalculationByApplicationID(icdoBenefitDroApplication.dro_application_id);
                    if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id == 0)
                        CreateDROBenefit();
                }
                if ((ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == null) ||
                    (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusPending))
                {
                    lblnInitiateWorkflow = true;
                }
                if (lblnInitiateWorkflow)
                {
                    InitializeWorkFlow(icdoBenefitDroApplication.alternate_payee_perslink_id);
                }
            }
        }
        public void InitializeWorkFlow(int aintPersonID)
        {
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Process_QDRO_Calculation, aintPersonID, 0, icdoBenefitDroApplication == null ? 0 : icdoBenefitDroApplication.dro_application_id, iobjPassInfo);
        }
        public override void BeforePersistChanges()
        {
            switch (iobjPassInfo.istrPostBackControlID)
            {
                case "btnApproved":
                    SetApprovalInfo();
                    CalculateContributionSummary();
                    break;
                case "btnQualified":
                    icdoBenefitDroApplication.dro_status_value = busConstant.DROApplicationStatusQualified;
                    icdoBenefitDroApplication.qualified_by_user = iobjPassInfo.istrUserID;
                    icdoBenefitDroApplication.qualified_date = DateTime.Today;
                    if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel ||
                            icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel)
                    {
                        if (ibusBenefitPostRetirementDROCalculation == null)
                            ibusBenefitPostRetirementDROCalculation = new busBenefitPostRetirementDROCalculation();
                        DateTime ldtPERSLinkLive = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.PERSLinkGoLiveDate, iobjPassInfo));
                        DateTime ldtDivorceDate = new DateTime(icdoBenefitDroApplication.date_of_divorce.Year, icdoBenefitDroApplication.date_of_divorce.Month, 1);
                        ibusBenefitPostRetirementDROCalculation.CalculateGrossMonthlyAmount(ldtDivorceDate > ldtPERSLinkLive ? ldtDivorceDate :
                        ldtPERSLinkLive, ibusMemberPayeeAccount);
                        icdoBenefitDroApplication.computed_member_gross_monthly_amount = ibusBenefitPostRetirementDROCalculation.idecGrossMonthlyAmount;
                    }
                    break;
                //UAT PIR - 880 - After approval application data , if any changes made on the screen and save is clicked ,status has to change to received
                case "btnSave":
                    if (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusApproved)
                    {
                        icdoBenefitDroApplication.dro_status_value = busConstant.DROApplicationStatusRecieved;
                        icdoBenefitDroApplication.approved_by_user = string.Empty;
                        icdoBenefitDroApplication.approved_date = DateTime.MinValue;
                    }

                    break;
                default:
                    break;
            }
            icdoBenefitDroApplication.work_flow_intiated_flag = busConstant.Flag_No;
            //For audit log purpose
            icdoBenefitDroApplication.person_id = icdoBenefitDroApplication.member_perslink_id;
        }

        public Collection<cdoCodeValue> LoadPaymentStatus()
        {
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(2451, busConstant.Flag_Yes, null, null);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            return lclcCodeValue;
        }
        //UCS - 059 - Functionality Starts Here
        public void CreateDROBenefit()
        {
            ibusBenefitDroCalculation = new busBenefitDroCalculation();
            ibusBenefitDroCalculation.icdoBenefitDroCalculation = new cdoBenefitDroCalculation();
            ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_application_id = icdoBenefitDroApplication.dro_application_id;
            ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value = busConstant.DROApplicationPaymentStatusPending;
            ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value = string.Empty;

            //PIR 13561 - Added Benefit Begin Date on UI.
            //if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel ||
            //    icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel)
            //{
            //    ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date = new DateTime(icdoBenefitDroApplication.qualified_date.AddMonths(1).Year,
            //                                                                icdoBenefitDroApplication.qualified_date.AddMonths(1).Month, 1);
            //}
            //else
            //{
            //    ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date = icdoBenefitDroApplication.benefit_receipt_date;
            //}

            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0)
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.Update();
            else
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.Insert();
        }

        //PIR 13561 - Added Benefit Begin Date on UI.
        private ArrayList IsBenefitBeginDateNull()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date == DateTime.MinValue)
            {
                lobjError = new utlError { istrErrorID = "", istrErrorMessage = "Please enter Benefit Begin Date." };
                alReturn.Add(lobjError);
            }
            return alReturn;
        }

        public ArrayList btnCalculateAndSave()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            iblnClearSoftErrors = true;
            iblnIsFromCalculateAndSave = true;
            //PIR 13561 - Added Benefit Begin Date on UI.
            alReturn = IsBenefitBeginDateNull();
            if (alReturn.Count > 0) return alReturn;

            if (ibusSoftErrors != null)
                ibusSoftErrors.iblnClearError = true;
            this.ValidateSoftErrors();
            ValidateCalculate(alReturn, lobjError, false);
            if (alReturn.Count == 0)
            {
                decimal ldecCalculationPercentage = GetCalculationPercentage();

                CalculateContributionAmount(ldecCalculationPercentage);

                DateTime ldtInterestBatchLastRunDate = busInterestCalculationHelper.GetInterestBatchLastRunDate();

                ibusBenefitDroCalculation.icdoBenefitDroCalculation.non_taxable_amount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount;

                decimal ldecInterest = 0.00M;

                ldecInterest = ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount + GetInterestAmount();
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount = ldecInterest;
                CalculateAdditionalInterest(ldtInterestBatchLastRunDate);
                //added validation so as to give hard error if any calculated fields are negative
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount < 0 ||
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount < 0 ||
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount < 0 ||
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount < 0 ||
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount < 0 ||
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.capital_gain < 0 ||
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest_amount < 0)
                {
                    lobjError = new utlError { istrErrorID = "7640", istrErrorMessage = "Calculation amounts cannot be less than zero" };
                    alReturn.Add(lobjError);
                    return alReturn;
                }
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value = busConstant.DROApplicationPaymentStatusPending;
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0)
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.Update();

                //UAT PIR 1404 RMD for AP
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0)
                {
                    CalculateRMDAmount();
                }
                alReturn.Add(this);
                EvaluateInitialLoadRules();
                LoadDROCalculation();
            }
            return alReturn;
        }

        private void CalculateRMDAmount()
        {
            bool lblnIsAllowedToCalculateRMD = false;
            decimal ldecAgeToGetRMD = 0M;
            //decimal idecMemberAccountBalance = 0.00M;

            if ((ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeDeath
                && ibusMember.icdoPerson.date_of_death != DateTime.MinValue
                && ibusAlternatePayee.icdoPerson.date_of_death == DateTime.MinValue)
                || (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeRefundSchedule))
            {
                decimal idecAPAgeBasedOnGivenDate = 0.00M;
                if (idecAPAgeBasedOnGivenDate == 0)
                    SetAPAge(ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date, ref idecAPAgeBasedOnGivenDate);

                if (idecAPAgeBasedOnGivenDate >= 70.5M)
                {
                    lblnIsAllowedToCalculateRMD = true;
                    ldecAgeToGetRMD = idecAPAgeBasedOnGivenDate;
                }
            }
            //else if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeRefundSchedule)
            //{
            //    if (idecMemberAgeBasedOnGivenDate == 0M)
            //    {
            //        decimal ldecMemberAge = 0M;
            //        SetAge(icdoBenefitDroApplication.received_date, ref ldecMemberAge);
            //        idecMemberAgeBasedOnGivenDate = ldecMemberAge;
            //    }
            //    if (idecMemberAgeBasedOnGivenDate >= 70.5M)
            //    {
            //        lblnIsAllowedToCalculateRMD = true;
            //        ldecAgeToGetRMD = idecMemberAgeBasedOnGivenDate;
            //    }
            //}

            if (lblnIsAllowedToCalculateRMD)
            {
                //get member account balance                                    
                //idecMemberAccountBalance = ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount +
                //                                                 ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount +
                //                                                 ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount +
                //                                                 ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount +
                //                                                 ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount;
                // icdoBenefitRefundCalculation.rhic_ee_amount;


                ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount = CalculateRMDAmount(ldecAgeToGetRMD,
                                                                                          busConstant.ApplicationBenefitTypePreRetirementDeath, idecMemberAccountBalance, ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date);

                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount > 0M)
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.Update();

            }
        }

        public void LoadDROCalculation()
        {
            if (ibusBenefitDroCalculation == null)
                ibusBenefitDroCalculation = new busBenefitDroCalculation();
            ibusBenefitDroCalculation.FindBenefitDroCalculationByApplicationID(icdoBenefitDroApplication.dro_application_id);
        }

        public void LoadPostRetirementDROCalculation()
        {
            if (ibusBenefitPostRetirementDROCalculation == null)
                ibusBenefitPostRetirementDROCalculation = new busBenefitPostRetirementDROCalculation();
            ibusBenefitPostRetirementDROCalculation.FindBenefitDroCalculationByApplicationID(icdoBenefitDroApplication.dro_application_id);
        }
        public ArrayList btnApprove()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            //is RMD reducable
            bool lblnIsNotEligibleForRMDAmount = true;

            //PIR 13561 - Added Benefit Begin Date on UI.
            alReturn = IsBenefitBeginDateNull();
            if (alReturn.Count > 0) return alReturn;

            iblnClearSoftErrors = true;
            if (ibusSoftErrors != null)
                ibusSoftErrors.iblnClearError = true;
            this.ValidateSoftErrors();
            //UAT PRI 1404
            lblnIsNotEligibleForRMDAmount = IsEligibleForRMD();
            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeDeath
                || ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeRefundSchedule)
            {
                if (lblnIsNotEligibleForRMDAmount == false)
                {
                    lobjError = null;
                    lobjError = AddError(4228, "Unable to Reduce RMD Amount for Spouse. Payee account creation failed.");
                    alReturn.Add(lobjError);
                }
            }
            ValidateCalculate(alReturn, lobjError, true);
            if (alReturn.Count == 0)
            {
                CalculateAdditionalInterest(busInterestCalculationHelper.GetInterestBatchLastRunDate());
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value = busConstant.DROApplicationPaymentStatusApproved;
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest_amount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest;
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.non_taxable_amount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount;
                decimal ldecPercentage = 100.0m;
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0)
                {
                    if (iclbPayeeAccount == null)
                        LoadPayeeAccount();
                    if (iclbPayeeAccount.Count == 0)
                    {
                        if (ibusAlternatePayee.iclbPersonBeneficiary == null)
                            ibusAlternatePayee.LoadApplicationBeneficiary(ablnFromDRO: true);
                        if ((ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeDeath) &&
                            (ibusAlternatePayee.icdoPerson.date_of_death != DateTime.MinValue))
                        {
                            foreach (busPersonBeneficiary lobjBeneficiary in ibusAlternatePayee.iclbPersonBeneficiary)
                            {
                                if (lobjBeneficiary.ibusBenefitApplicationBeneficiary != null &&
                                    lobjBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id > 0)
                                {
                                    CreatePayeeAccount(lobjBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dist_percent, lobjBeneficiary, false);
                                }
                            }
                        }
                        else
                        {
                            CreatePayeeAccount(ldecPercentage, null, lblnIsNotEligibleForRMDAmount);
                        }
                        //UCS - 86 Funcition which contain all calculation logic
                        if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel ||
                            icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel)
                        {
                            DROCalculationApproveLogic();
                        }
                    }
                }
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0)
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.Update();
                LoadDROCalculation();
                LoadPostRetirementDROCalculation();
                LoadPayeeAccount();
                alReturn.Add(this);
                if (ibusBaseActivityInstance.IsNotNull())
                {
                    busPayeeAccount lobjPA = iclbPayeeAccount.Where(o => o.icdoPayeeAccount.dro_calculation_id == ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id)
                                                            .FirstOrDefault();
                    if (lobjPA.IsNotNull())
                    {
                        //if (lobjPA.ibusBaseActivityInstance.IsNull())
                        //    lobjPA.ibusBaseActivityInstance = new busActivityInstance();
                        lobjPA.ibusBaseActivityInstance = ibusBaseActivityInstance;
                        //lobjPA.SetProcessInstanceParameters();
                        lobjPA.SetCaseInstanceParameters();
                    }
                }

                EvaluateInitialLoadRules();
            }
            return alReturn;
        }
        public ArrayList btnVerify()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            iblnClearSoftErrors = true;

            //PIR 13561 - Added Benefit Begin Date on UI.
            alReturn = IsBenefitBeginDateNull();
            if (alReturn.Count > 0) return alReturn;

            if (ibusSoftErrors != null)
                ibusSoftErrors.iblnClearError = true;
            this.ValidateSoftErrors();
            ValidateCalculate(alReturn, lobjError, true);
            if (alReturn.Count == 0)
            {
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value = busConstant.DROApplicationPaymentStatusVerified;
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.verified_by_user = iobjPassInfo.istrUserID;
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0)
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.Update();
                alReturn.Add(this);
                EvaluateInitialLoadRules();
                LoadDROCalculation();
            }
            return alReturn;
        }
        private bool IsPayeeAccountCreationValid()
        {
            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeMonthlyBenefit)
            {
                decimal ldecTotalMonthlyBenefitAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_nontaxable_amount + ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_taxable_amount;
                if (ldecTotalMonthlyBenefitAmount > 0)
                {
                    return true;
                }
            }
            else
            {
                decimal ldecTotalRefundAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount + ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount +
                   ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount + ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount;
                if (ldecTotalRefundAmount > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void ValidateCalculate(ArrayList alReturn, utlError lobjError, bool ablnVerifyOrApprove)
        {
            if (ibusAlternatePayee == null)
                LoadAlternatePayee();
            if (string.IsNullOrEmpty(ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value))
            {
                lobjError = null;
                lobjError = AddError(7639, "Payment Status cannot be empty");
                alReturn.Add(lobjError);
            }
            if ((ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeMonthlyBenefit)
                && (icdoBenefitDroApplication.time_of_benefit_receipt_calc_value == busConstant.DROApplicationModelTimeOfBenfitRetirementDate)
                && (icdoBenefitDroApplication.benefit_receipt_date == DateTime.MinValue))
            {
                lobjError = null;
                lobjError = AddError(7645, "Benefit receipt date is not posted as the member does not have retirement application");
                alReturn.Add(lobjError);
            }
            if (string.IsNullOrEmpty(ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value))
            {
                lobjError = null;
                lobjError = AddError(7940, "Payment Type is required");
                alReturn.Add(lobjError);
            }

            if ((ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id == 0) &&
                (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value != busConstant.DROApplicationPaymentStatusPending))
            {
                lobjError = null;
                lobjError = AddError(7641, "Payment Status should be pending");
                alReturn.Add(lobjError);
            }
            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeDeath)
            {
                if (ibusMember == null)
                    LoadMember();
                if ((ibusMember.icdoPerson.date_of_death == DateTime.MinValue) && (ibusAlternatePayee.icdoPerson.date_of_death == DateTime.MinValue))
                {
                    lobjError = null;
                    lobjError = AddError(7647, "Payment Type can not be death schedule as the member and alternate payee are still alive");
                    alReturn.Add(lobjError);
                }
            }
            if ((ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeDeath) &&
                        (ibusAlternatePayee.icdoPerson.date_of_death != DateTime.MinValue))
            {
                if (IsBeneficiaryTabVisible())
                {
                    if (ibusAlternatePayee.iclbPersonBeneficiary == null)
                        ibusAlternatePayee.LoadApplicationBeneficiary(ablnFromDRO: true);

                    if (ibusAlternatePayee.iclbPersonBeneficiary.Count == 0)
                    {
                        lobjError = null;
                        lobjError = AddError(7648, "Alternate payee does not have beneficiaries");
                        alReturn.Add(lobjError);
                    }
                    else
                    {
                        decimal ldecPercentage = 0.0m;
                        foreach (busPersonBeneficiary lobjBeneficiary in ibusAlternatePayee.iclbPersonBeneficiary)
                        {
                            if (lobjBeneficiary.ibusBenefitApplicationBeneficiary != null &&
                                lobjBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id > 0)
                            {
                                ldecPercentage += lobjBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dist_percent;
                            }
                        }
                        if (ldecPercentage != 100.00M)
                        {
                            lobjError = null;
                            lobjError = AddError(7649, "Total beneficiary percentage should be 100");
                            alReturn.Add(lobjError);
                        }
                    }
                }
                //PIR 1367 - if there is no beneficiary and alternate payee to get the benefit under death schedule,calculation cannot be approved
                else
                {
                    lobjError = null;
                    lobjError = AddError(7650, "The alternate payee is deceased and beneficiaries are not applicable for this benefit duration option,calculation cannot be approved");
                    alReturn.Add(lobjError);
                }
            }
            if ((ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value != busConstant.DROApplicationPaymentTypeMonthlyBenefit) &&
          (ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount > 0.0M))
            {
                lobjError = null;
                lobjError = AddError(7642, "Monthly Benefit Amount cannot be entered for Refund and Death Schedule");
                alReturn.Add(lobjError);
            }
            else if ((ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeMonthlyBenefit) &&
                (ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount <= 0.00M) && ablnVerifyOrApprove)
            {
                lobjError = null;
                lobjError = AddError(7638, "Monthly Benefit Amount is required.");
                alReturn.Add(lobjError);
            }
            if ((icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel ||
                icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel) &&
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value != busConstant.DROApplicationPaymentTypeMonthlyBenefit)
            {
                lobjError = null;
                lobjError = AddError(6301, "Payment Type should be 'Monthly Benefit Schedule' for Retiree Model");
                alReturn.Add(lobjError);
            }

            //ucs - 059 - 38 and 39 
            //implemented on 13/april/10 as per satya
            //if member death date exists, then only month benefit schedule can be selected as payment type
            //if alternate payee death date exists , then only death schedule can be selected as payment type
            if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel)
            {
                if (ibusMember == null)
                    LoadMember();
                if (ibusMember.icdoPerson.date_of_death != DateTime.MinValue &&
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value != busConstant.DROApplicationPaymentTypeMonthlyBenefit)
                {
                    lobjError = null;
                    lobjError = AddError(7666, "Payment Type should be 'Monthly Benefit Schedule' as member is dead");
                    alReturn.Add(lobjError);
                }

                //uat pir - 928 ,1313
                if (ibusAlternatePayee == null)
                    LoadAlternatePayee();
                if (ibusAlternatePayee.icdoPerson.date_of_death != DateTime.MinValue &&
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value != busConstant.DROApplicationPaymentTypeDeath)
                {
                    lobjError = null;
                    lobjError = AddError(7668, "Payment Type should be 'Death Schedule' as alternate payee is dead");
                    alReturn.Add(lobjError);
                }
            }

            //ucs - 059 - 34
            //implemented on 13/april/10 as per satya
            //if alternate payee is dead, then cannot proceed with dro calculation for retiree db and active db former model
            //uat pir - 928 ,1313
            if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel ||
                icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBFormerModel)
            {
                if (ibusAlternatePayee == null)
                    LoadAlternatePayee();
                if (ibusAlternatePayee.icdoPerson.date_of_death != DateTime.MinValue)
                {
                    lobjError = null;
                    lobjError = AddError(7667, "DRO calculation cannot be proceeded as Alternate payee is dead");
                    alReturn.Add(lobjError);
                }
            }
            if(this.iblnIsFromCalculateAndSave && this.istrSupressWarnings != "Y")
            {
                lobjError = null;
                lobjError = AddError(10460, "");
                alReturn.Add(lobjError);
            }
        }
        private decimal GetInterestAmount()
        {
            decimal ldecDifferenceAmount = 0.0M;
            if ((ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeDeath) ||
                (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeRefundSchedule))
            {

                decimal ldecTotalAmount = GetCalculationTotalAmount();
                if (icdoBenefitDroApplication.lumpsum_payment_amount > 0)
                {
                    ldecDifferenceAmount = icdoBenefitDroApplication.lumpsum_payment_amount - ldecTotalAmount;
                }
                else if (icdoBenefitDroApplication.member_withdrawal_amount > 0)
                {
                    ldecDifferenceAmount = icdoBenefitDroApplication.member_withdrawal_amount - ldecTotalAmount;
                }
            }
            return ldecDifferenceAmount;
        }

        private decimal GetCalculationTotalAmount()
        {
            decimal ldecTotalAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount + ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount +
                                      ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount + ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount +
                                      ibusBenefitDroCalculation.icdoBenefitDroCalculation.capital_gain + ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount;

            return ldecTotalAmount;
        }
        private decimal GetCalculationTotalTaxableAmount()
        {
            decimal ldecTotalAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount +
                                      ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount + ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount +
                                      ibusBenefitDroCalculation.icdoBenefitDroCalculation.capital_gain + ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount;

            return ldecTotalAmount;
        }

        public string istrSupressWarnings { get; set; }
        public bool iblnIsFromCalculateAndSave { get; set; }

        public decimal GetCalculationPercentage()
        {
            decimal ldecCalcPercentage = 0.0M;
            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeMonthlyBenefit)
            {
                ldecCalcPercentage = icdoBenefitDroApplication.monthly_benefit_percentage;
            }
            else if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeRefundSchedule)
            {
                if (IsDROModelRefundLumpSumOriented())
                {
                    ldecCalcPercentage = icdoBenefitDroApplication.lumpsum_payment_percentage;
                }
                else
                {
                    ldecCalcPercentage = icdoBenefitDroApplication.member_withdrawal_percentage;
                }
            }
            else if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeDeath)
            {
                if (ibusMember == null)
                    LoadMember();
                if (ibusAlternatePayee == null)
                    LoadAlternatePayee();
                if (ibusMember.icdoPerson.date_of_death != DateTime.MinValue)
                {
                    if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel)
                    {
                        ldecCalcPercentage = icdoBenefitDroApplication.monthly_benefit_percentage;
                    }
                    else
                    {
                        ldecCalcPercentage = icdoBenefitDroApplication.member_death_percentage;
                    }
                }
                else if (ibusAlternatePayee.icdoPerson.date_of_death != DateTime.MinValue)
                {
                    if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBModel)
                    {
                        ldecCalcPercentage = icdoBenefitDroApplication.alternate_payee_death_percentage;
                    }
                }
            }
            return ldecCalcPercentage;
        }

        public void CalculateContributionAmount(decimal adecCalPercentage)
        {
            decimal ldecPercentage = adecCalPercentage / 100;
            if (icdoBenefitDroApplication.overridden_ee_pre_tax_amount > 0.0M)
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount = icdoBenefitDroApplication.overridden_ee_pre_tax_amount * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.overridden_ee_pre_tax_amount * ldecPercentage : 0.0m;
            else
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount = icdoBenefitDroApplication.computed_ee_pre_tax_amount * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.computed_ee_pre_tax_amount * ldecPercentage : 0.0m;

            if (icdoBenefitDroApplication.overridden_ee_post_tax_amount > 0.0M)
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount = icdoBenefitDroApplication.overridden_ee_post_tax_amount * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.overridden_ee_post_tax_amount * ldecPercentage : 0.0m;
            else
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount = icdoBenefitDroApplication.computed_ee_post_tax_amount * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.computed_ee_post_tax_amount * ldecPercentage : 0.0m;

            if (icdoBenefitDroApplication.overridden_er_vested_amount > 0.0M)
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount = icdoBenefitDroApplication.overridden_er_vested_amount * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.overridden_er_vested_amount * ldecPercentage : 0.0m;
            else
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount = icdoBenefitDroApplication.computed_er_vested_amount * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.computed_er_vested_amount * ldecPercentage : 0.0m;

            if (icdoBenefitDroApplication.overridden_ee_er_pickup_amount > 0.0M)
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount = icdoBenefitDroApplication.overridden_ee_er_pickup_amount * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.overridden_ee_er_pickup_amount * ldecPercentage : 0.0m;
            else
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount = icdoBenefitDroApplication.computed_ee_er_pickup_amount * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.computed_ee_er_pickup_amount * ldecPercentage : 0.0m;

            if (icdoBenefitDroApplication.overridden_interest_amount > 0.0M)
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount = icdoBenefitDroApplication.overridden_interest_amount * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.overridden_interest_amount * ldecPercentage : 0.0m;
            else
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount = icdoBenefitDroApplication.computed_interest_amount * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.computed_interest_amount * ldecPercentage : 0.0m;

            if (icdoBenefitDroApplication.overridden_capital_gain > 0.0M)
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.capital_gain = icdoBenefitDroApplication.overridden_capital_gain * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.overridden_capital_gain * ldecPercentage : 0.0m;
            else
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.capital_gain = icdoBenefitDroApplication.computed_capital_gain * ldecPercentage > 0.0m ?
                    icdoBenefitDroApplication.computed_capital_gain * ldecPercentage : 0.0m;
        }

        //Calculate Additional Interest for the contribution amounts

        public void CalculateAdditionalInterest(DateTime adtInterestBatchLastRunDate)
        {
            decimal ldecInterestCalculationAmount = 0.0M;
            decimal ldecPrincipleAmount = 0.0M;
            decimal ldecInterestAmount = 0.0M;
            DateTime ldtInterestCalcStartDate = DateTime.MinValue;
            if (icdoBenefitDroApplication.date_of_divorce != DateTime.MinValue)
            {
                ldtInterestCalcStartDate = new DateTime(icdoBenefitDroApplication.date_of_divorce.Year, icdoBenefitDroApplication.date_of_divorce.Month, 1);
                ldtInterestCalcStartDate = ldtInterestCalcStartDate.AddMonths(1);

                ldecInterestCalculationAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount + ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount +
                                              ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount + ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount +
                                              ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount;

                ldecPrincipleAmount = ldecInterestCalculationAmount;
                DateTime ldtCalcStartDate = new DateTime(ldtInterestCalcStartDate.AddMonths(1).AddDays(-1).Year,
                                            ldtInterestCalcStartDate.AddMonths(1).AddDays(-1).Month, ldtInterestCalcStartDate.AddMonths(1).AddDays(-1).Day);

                while (ldtCalcStartDate <= adtInterestBatchLastRunDate)
                {
                    ldecInterestAmount = busInterestCalculationHelper.CalculateInterest(ldecInterestCalculationAmount, icdoBenefitDroApplication.plan_id);
                    ldecInterestCalculationAmount = ldecInterestCalculationAmount + ldecInterestAmount;
                    ldtCalcStartDate = ldtCalcStartDate.AddMonths(1);
                }
                ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest = Math.Round(ldecInterestCalculationAmount - ldecPrincipleAmount, 2, MidpointRounding.AwayFromZero);
            }
        }

        public void CreatePayeeAccount(decimal adecPercentage, busPersonBeneficiary aobjBeneficiary, bool ablnIsNotAllowedForRMD)
        {
            string lstrBenefitType = GetBenefitType();
            string lstrBenefitOption = GetBenefitOption(lstrBenefitType);
            string lstrBenefitSubType = GetBenefitSubType(lstrBenefitType);
            DateTime ldtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeMonthlyBenefit)
            {
                decimal ldecTaxableAmount = 0.00M, ldecNonTaxableAmount = 0.00M;

                //PIR 13561 - Added Benefit Begin Date on UI.
                //if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel ||
                //    icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel)
                //    ldtNextBenefitPaymentDate = ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date;
                //else
                //    ldtNextBenefitPaymentDate = icdoBenefitDroApplication.benefit_receipt_date;

                CalculateMonthlyPaymentItems(lstrBenefitType, lstrBenefitOption, lstrBenefitSubType, ref ldecTaxableAmount, ref ldecNonTaxableAmount);
            }
            //PIR 13561 - Added Benefit Begin Date on UI.
            //else
            //{
            //    ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date = ldtNextBenefitPaymentDate;
            //}

            LoadPersonAccountInfo();

            DateTime ldtTeminationDate = DateTime.MinValue;
            int lintRetirmentOrgID = GetOrgIdAsLatestEmploymentOrgId(ibusPersonAccount.icdoPersonAccount.person_account_id, lstrBenefitType, ref ldtTeminationDate);
            //Check whether benefit account exists for the member ,if exists ,use the same benefit account id for the payee account creation    
            if (IsPayeeAccountCreationValid())
            {
                int lintBenefitAccountID = IsBenefitAccountExists(ibusPersonAccount.icdoPersonAccount.person_account_id, lstrBenefitType);
                if (lintBenefitAccountID > 0)
                {
                    //PIR 13561 - Added Benefit Begin Date on UI.
                    CreatePayeeAccountDetails(lintBenefitAccountID, ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date, lstrBenefitType, lstrBenefitOption, lstrBenefitSubType, adecPercentage, aobjBeneficiary, ablnIsNotAllowedForRMD);
                }
                else //if benefit account does not exist for the member ,then create a new benefit account and payee account
                {
                    //TODO retirement ORg ID
                    lintBenefitAccountID = ManageBenefitAccount(ibusPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd,
                                                                ibusBenefitDroCalculation.icdoBenefitDroCalculation.non_taxable_amount, busConstant.StatusValid,
                                                                string.Empty, 0.0M, 0.0M, 0.0M, lintRetirmentOrgID, 0, DateTime.MinValue, 0.0M, string.Empty, 0M, 0M);
                    CreatePayeeAccountDetails(lintBenefitAccountID, ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date, lstrBenefitType, lstrBenefitOption, lstrBenefitSubType, adecPercentage, aobjBeneficiary, ablnIsNotAllowedForRMD);
                }
            }
        }

        private void LoadPersonAccountInfo()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonAccountRetirement == null)
                ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
            ibusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
            ibusPersonAccount.ibusPersonAccountRetirement.LoadLTDSummary();
        }

        //Creating payee account payment items
        private void CreatePayeeAccountDetails(int aintBenefitAccountID, DateTime adtPaymentDate, string astrBenefitType, string astrBenefitOption,
            string astrBenefitSubType, decimal adecPercentage,
            busPersonBeneficiary aobjBeneficiary, bool ablnIsNotAllowedForRMD)
        {
            int lintPayeeAccountID = CreatePayeeAccountAndStatus(aintBenefitAccountID, adtPaymentDate, astrBenefitType, astrBenefitOption, astrBenefitSubType, aobjBeneficiary);
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            lobjPayeeAccount.FindPayeeAccount(lintPayeeAccountID);
            //UCS - 86 handles the insertion of Payment Items -- Function DROCalculationApproveLogic()
            if (icdoBenefitDroApplication.dro_model_value != busConstant.DROApplicationModelRetireeDBModel &&
                icdoBenefitDroApplication.dro_model_value != busConstant.DROApplicationModelRetiredJobServiceModel)
            {
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeMonthlyBenefit)
                {
                    CreatePaymentItemsForMonthlyPayment(adtPaymentDate, lobjPayeeAccount);
                }
                else
                {
                    CreatePaymentItemsForDeathAndRefund(adtPaymentDate, lobjPayeeAccount, adecPercentage, ablnIsNotAllowedForRMD);
                }
            }
			
			//PIR 13577
            lobjPayeeAccount.CreateBenefitOverPaymentorUnderPayment(string.Empty, ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id);
        }

        private int CreatePayeeAccountAndStatus(int aintBenefitAccountID, DateTime adtPaymentDate, string astrBenefitType, string astrBenefitOption, string astrBenefitSubType, busPersonBeneficiary aobjBeneficiary)
        {
            int lintPayeeID = 0;
            int lintPayeeOrgID = 0;
            string lstrFamilyRelationship = string.Empty;
            if (aobjBeneficiary != null)
            {
                lstrFamilyRelationship = aobjBeneficiary.icdoPersonBeneficiary.relationship_value;
                if (aobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0)
                    lintPayeeID = aobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id;
                else
                    lintPayeeOrgID = aobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id;
            }
            else
            {
                lintPayeeID = icdoBenefitDroApplication.alternate_payee_perslink_id;
                lstrFamilyRelationship = busConstant.PayeeAccountFamilyRelationshipExSpouse;
            }
            decimal ldecMinimumGauranteeAmount = GetCalculationTotalAmount();

            // BR-054-014 - Term certain end date will be calculated while creating the Payee account if the Benefit option is Term Certain.
            DateTime ldteTermCertainEndDate = new DateTime();
            if (icdoBenefitDroApplication.IsTermCertainBenefitOption())
            {
                string lstrData3Value = busGlobalFunctions.GetData3ByCodeValue(2404, icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);
                string lstrTermCertainYears = busGlobalFunctions.GetData1ByCodeValue(2216, lstrData3Value, iobjPassInfo);
                if (adtPaymentDate != DateTime.MinValue)
                {
                    ldteTermCertainEndDate = adtPaymentDate.AddYears(Convert.ToInt32(lstrTermCertainYears));
                }
            }

            int lintPayeeAccountID = busPayeeAccountHelper.ManagePayeeAccount(lintPayeeID, lintPayeeOrgID, 0, 0, aintBenefitAccountID,
                                            ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id,
                                            busConstant.StatusValid, astrBenefitType, astrBenefitSubType, busConstant.Flag_No,
                                            adtPaymentDate, DateTime.MinValue, busConstant.PayeeAccountAccountRelationshipAlternatePayee,
                                            lstrFamilyRelationship, ldecMinimumGauranteeAmount,
                                            ibusBenefitDroCalculation.icdoBenefitDroCalculation.non_taxable_amount,
                                            astrBenefitOption, 0.0M, 0, busConstant.PayeeAccountExclusionMethodSimplified, 0.0m,
                                            ldteTermCertainEndDate, string.Empty, icdoBenefitDroApplication.dro_application_id, string.Empty);

            busPayeeAccountHelper.CreatePayeeAccountStatus(lintPayeeAccountID, busConstant.PayeeAccountStatusRefundReview, DateTime.Today, null, null);
            return lintPayeeAccountID;
        }

        private void CreatePaymentItemsForMonthlyPayment(DateTime adtPaymentDate, busPayeeAccount lobjPayeeAccount)
        {
            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_nontaxable_amount > 0.0M)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PaymentItemTypeNonTaxableAmount, ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_nontaxable_amount,
                                                "", 0, adtPaymentDate, DateTime.MinValue);
            }
            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_taxable_amount > 0.0M)
            {
                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PaymentItemTypeTaxableAmount, ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_taxable_amount,
                                                "", 0, adtPaymentDate, DateTime.MinValue);
            }
        }

        private void CreatePaymentItemsForDeathAndRefund(DateTime adtPaymentDate, busPayeeAccount lobjPayeeAccount, decimal adecPercentage, bool alblnIsNotAllowedForRMD)
        {
            bool lblnAllowedCreationOfPAPIT = true;
            if ((ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeDeath)
                || ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeRefundSchedule)
            {
                alblnIsNotAllowedForRMD = true;
                bool lblnIsEligibleForRMD = IsEligibleForRMD();
                if (lblnIsEligibleForRMD)
                    alblnIsNotAllowedForRMD = false;
            }
            if (alblnIsNotAllowedForRMD
                && ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeDeath)
            {
                lblnAllowedCreationOfPAPIT = false;
            }

            if (lblnAllowedCreationOfPAPIT)
            {
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount > 0.0M)
                {
                    decimal ldecERVestedAmount = idecERVestedAmount == 0M ? ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount : idecERVestedAmount;

                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemVestedERContributionAmount, (ldecERVestedAmount * adecPercentage) / 100,
                                                    "", 0, adtPaymentDate, DateTime.MinValue);
                }
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount > 0.0M)
                {
                    decimal ldecEEPreTaxAmount = idecEEPreAmount == 0M ? ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount : idecEEPreAmount;

                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPreTaxEEContributionAmount, (ldecEEPreTaxAmount * adecPercentage) / 100,
                                                    "", 0, adtPaymentDate, DateTime.MinValue);
                }
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount > 0.0M)
                {
                    decimal ldecEEPostAmount = idecEEPostAmount == 0M ? ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount : idecEEPostAmount;

                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPostTaxEEContributionAmount, (ldecEEPostAmount * adecPercentage) / 100,
                                                    "", 0, adtPaymentDate, DateTime.MinValue);
                }
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount > 0.0M)
                {
                    decimal ldecEEERAmount = idecEEERPickupAmount == 0M ? ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount : idecEEERPickupAmount;

                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemEEERPickupAmount, (ldecEEERAmount * adecPercentage) / 100,
                                                    "", 0, adtPaymentDate, DateTime.MinValue);
                }
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount > 0.0M)
                {
                    decimal ldecinterestAmount = idecInterestAmount == 0M ? ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount : idecInterestAmount;

                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemEEInterestAmount, (ldecinterestAmount * adecPercentage) / 100,
                                                    "", 0, adtPaymentDate, DateTime.MinValue);
                }
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest > 0.0M)
                {
                    decimal ldecAdditionalAmount = idecAddInterestAmount == 0M ? ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest : idecAddInterestAmount;

                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemAdditionalEEInterestAmount, (ldecAdditionalAmount * adecPercentage) / 100,
                                                    "", 0, adtPaymentDate, DateTime.MinValue);
                }
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.capital_gain > 0.0M)
                {
                    decimal ldecCapitalAmount = idecCapitalGainAmount == 0M ? ibusBenefitDroCalculation.icdoBenefitDroCalculation.capital_gain : idecCapitalGainAmount;

                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemCapitalGain, (ldecCapitalAmount * adecPercentage) / 100,
                                                    "", 0, adtPaymentDate, DateTime.MinValue);
                }
                //UAT PIR 1404
                //if (alblnIsNotAllowedForRMD)
                //{
                if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount > 0.0M)
                {
                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITRMDAmount, (ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount * adecPercentage) / 100,
                                                    "", 0, adtPaymentDate, DateTime.MinValue);
                }
                //}
            }
        }

        public decimal idecERVestedAmount { get; set; }
        public decimal idecEEPreAmount { get; set; }
        public decimal idecEEPostAmount { get; set; }
        public decimal idecEEERPickupAmount { get; set; }
        public decimal idecInterestAmount { get; set; }
        public decimal idecAddInterestAmount { get; set; }
        public decimal idecCapitalGainAmount { get; set; }
        //UAT PIR 1404
        //check is eligible for RMD
        private bool IsEligibleForRMD()
        {
            bool lblnIsRMDReduced = true;

            idecERVestedAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount;
            idecEEPreAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount;
            idecEEPostAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount;
            idecEEERPickupAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount;
            idecInterestAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount;
            idecAddInterestAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest;
            idecCapitalGainAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.capital_gain;

            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount > 0M)
            {
                if (idecERVestedAmount > ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount)
                {
                    idecERVestedAmount = idecERVestedAmount - ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount;
                }
                else if (idecEEPreAmount > ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount)
                {
                    idecEEPreAmount = idecEEPreAmount - ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount;
                }
                else if (idecEEPostAmount > ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount)
                {
                    idecEEPostAmount = idecEEPostAmount - ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount;
                }
                else if (idecEEERPickupAmount > ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount)
                {
                    idecEEERPickupAmount = idecEEERPickupAmount - ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount;
                }
                else if (idecInterestAmount > ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount)
                {
                    idecInterestAmount = idecInterestAmount - ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount;
                }
                else if (idecAddInterestAmount > ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount)
                {
                    idecAddInterestAmount = idecAddInterestAmount - ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount;
                }
                else if (idecCapitalGainAmount > ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount)
                {
                    idecCapitalGainAmount = idecCapitalGainAmount - ibusBenefitDroCalculation.icdoBenefitDroCalculation.rmd_amount;
                }
                else
                    lblnIsRMDReduced = false;
            }

            return lblnIsRMDReduced;
        }

        public int iintNumberOfPaymens { get; set; }

        public DateTime idtBenfitBegindate
        {
            get
            {
                if (iclbPayeeAccount == null)
                    LoadPayeeAccount();
                if (iclbPayeeAccount.Count > 0)
                    return iclbPayeeAccount[0].icdoPayeeAccount.benefit_begin_date;
                return DateTime.MinValue;
            }
        }

        public string istrBenefitBeginDate
        {
            get
            {
                if (idtBenfitBegindate == DateTime.MinValue)
                    return string.Empty;
                else
                    return idtBenfitBegindate.ToString(busConstant.DateFormatLongDate);
            }
        }

        public void CalculateMonthlyPaymentItems(string astrBenefitType, string astrBenefitOption, string astrBenefitSubType,
            ref decimal adecTaxableAmount, ref decimal adecNonTaxableAmount)
        {
            if (ibusBenefitDroCalculation == null)
                LoadDROCalculation();
            //uat pir 1590 : need to load monthly non taxable and monthly taxable amounts if dro calcuation is approved, so this function is called from srvApplication
            ibusBenefitDroCalculation.icdoBenefitDroCalculation.non_taxable_amount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount;
            busBenefitCalculation lobjBenefiCalculation = new busBenefitCalculation();
            lobjBenefiCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
            lobjBenefiCalculation.icdoBenefitCalculation.benefit_account_sub_type_value = astrBenefitSubType;
            lobjBenefiCalculation.icdoBenefitCalculation.benefit_account_type_value = astrBenefitType;
            lobjBenefiCalculation.icdoBenefitCalculation.benefit_option_value = astrBenefitOption;
            //For these models, benefit reciept date will be min value
            if (icdoBenefitDroApplication.dro_model_value != busConstant.DROApplicationModelRetiredJobServiceModel &&
                icdoBenefitDroApplication.dro_model_value != busConstant.DROApplicationModelRetireeDBModel)
            {
                //PIR 13561 - Added Benefit Begin Date on UI.
                //ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date = icdoBenefitDroApplication.benefit_receipt_date;
                lobjBenefiCalculation.icdoBenefitCalculation.retirement_date = icdoBenefitDroApplication.benefit_receipt_date;
            }
            else
                lobjBenefiCalculation.icdoBenefitCalculation.retirement_date = ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date;
            lobjBenefiCalculation.icdoBenefitCalculation.plan_id = icdoBenefitDroApplication.plan_id;
            lobjBenefiCalculation.LoadBenefitProvisionBenefitOption();

            decimal ldecMemberAge = 0M;
            decimal ldecNonTaxableAmount = 0.0M;
            decimal ldecMonthlyTaxableAmount = 0.0M;
            DateTime ldecAgeCalcDate = DateTime.MinValue;
            if (idtBenfitBegindate == DateTime.MinValue)
                SetAge(icdoBenefitDroApplication.benefit_receipt_date, ref ldecMemberAge);
            else
                SetAge(idtBenfitBegindate, ref ldecMemberAge);
            if (lobjBenefiCalculation.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.exclusive_calc_payment_type_value != null)
            {
                int iintNumberOfPaymens = lobjBenefiCalculation.GetNumberofPayments(lobjBenefiCalculation.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.exclusive_calc_payment_type_value, (int)ldecMemberAge, DateTime.Today);
                if (iintNumberOfPaymens > 0)
                {
                    ldecNonTaxableAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.non_taxable_amount / iintNumberOfPaymens;
                }
            }
            ldecMonthlyTaxableAmount = ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount - ldecNonTaxableAmount;
            adecTaxableAmount = ldecMonthlyTaxableAmount;
            adecNonTaxableAmount = ldecNonTaxableAmount;
            ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_taxable_amount = adecTaxableAmount;
            ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_nontaxable_amount = adecNonTaxableAmount;
        }
        private string GetBenefitSubType(string astrBenefitType)
        {
            string lstrBenefitSubType = string.Empty;
            DataTable ldtbSubType = iobjPassInfo.isrvDBCache.GetCodeValues(2228);
            foreach (DataRow dr in ldtbSubType.Rows)
            {
                if ((dr["data1"].ToString() == astrBenefitType) && (dr["data2"].ToString() == icdoBenefitDroApplication.dro_model_value))
                {
                    lstrBenefitSubType = dr["data3"].ToString();
                    break;
                }
            }
            return lstrBenefitSubType;
        }

        private string GetBenefitOption(string astrBenefitType)
        {
            string lstrBenefitOption = string.Empty;

            if (astrBenefitType == busConstant.ApplicationBenefitTypeRetirement)
            {
                lstrBenefitOption = GetBenefitDurationOption();
            }
            else if (astrBenefitType == busConstant.ApplicationBenefitTypeRefund)
            {
                lstrBenefitOption = busConstant.BenefitOptionRegularRefund;
            }
            else if (astrBenefitType == busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                if ((icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBFormerModel) ||
                    (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBModel))
                {
                    lstrBenefitOption = busConstant.BenefitOptionRefund;
                }
            }
            else
            {
                if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel)
                {
                    lstrBenefitOption = GetBenefitDurationOption();
                }
            }
            return lstrBenefitOption;
        }

        public string GetBenefitType()
        {
            string lstrBenefitType = string.Empty;

            if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeMonthlyBenefit)
                lstrBenefitType = busConstant.ApplicationBenefitTypeRetirement;
            else if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeRefundSchedule)
                lstrBenefitType = busConstant.ApplicationBenefitTypeRefund;
            else if (ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value == busConstant.DROApplicationPaymentTypeDeath)
                lstrBenefitType = busConstant.ApplicationBenefitTypePreRetirementDeath;
            return lstrBenefitType;
        }

        private string GetBenefitDurationOption()
        {
            string lstrBenefitOption = string.Empty;
            DataTable ldtbbenefitoption = iobjPassInfo.isrvDBCache.GetCodeDescription(2404, icdoBenefitDroApplication.benefit_duration_option_value);
            if (ldtbbenefitoption.Rows.Count > 0)
            {
                lstrBenefitOption = ldtbbenefitoption.Rows[0]["data3"].ToString();
            }
            return lstrBenefitOption;
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadAlternatePayee();
            ibusAlternatePayee.LoadApplicationBeneficiary(ablnFromDRO: true);
            if (iobjPassInfo.istrPostBackControlID == "btnQualified")
            {
                CreateDROBenefit();
                if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel ||
                icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel)
                {
                    //BR - 086 -02 Initiating workflow
                    if (icdoBenefitDroApplication.work_flow_intiated_flag != busConstant.Flag_Yes)
                    {
                        InitializeWorkFlow(icdoBenefitDroApplication.alternate_payee_perslink_id);
                        icdoBenefitDroApplication.work_flow_intiated_flag = busConstant.Flag_Yes;
                        icdoBenefitDroApplication.Update();
                    }

                    //prod pir 6420
                    DROQualifyForRetireeModels();

                }
            }
            EvaluateInitialLoadRules();
        }

        //Function to loop through the collection of available Benefit duration type and checking whether Beneficiary can be added for it
        public bool IsBeneficiaryTabVisible()
        {
            if (iclcCodeValue == null)
                LoadDurationOfBenefitOptionByDROModel();

            if (ibusBenefitDroCalculation == null)
                LoadDROCalculation();
            //uat pir 1391
            //as per the discussion with satya, if alternate payee death percentage is entered, provision to add beneficiay should be given
            if (icdoBenefitDroApplication.alternate_payee_death_percentage > 0)
                return true;

            foreach (cdoCodeValue lcdoCodeValue in iclcCodeValue)
            {
                if ((ibusBenefitDroCalculation != null && ibusBenefitDroCalculation.icdoBenefitDroCalculation != null &&
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0 &&
                    (lcdoCodeValue.code_value == icdoBenefitDroApplication.benefit_duration_option_value) &&
                    !(ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusApproved ||
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusProcessed) &&
                    lcdoCodeValue.data2 == "Y") ||
                    (lcdoCodeValue.code_value == icdoBenefitDroApplication.benefit_duration_option_value && lcdoCodeValue.data2 == "Y"))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Main method which perform all calculation logic when DRO Calculation Record is approved 
        /// </summary>
        public void DROCalculationApproveLogic()
        {
            LoadPayeeAccount();
            //Method to calculate total non taxable amount paid by member
            decimal ldecNonTaxableAmount = ibusBenefitPostRetirementDROCalculation.CalculateTotalNonTaxableAmountPaid(ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date,
                ibusMemberPayeeAccount);
            foreach (busPayeeAccount lobjAltPayeeAccount in iclbPayeeAccount)
            {
                if (lobjAltPayeeAccount.icdoPayeeAccount.payee_perslink_id == ibusAlternatePayee.icdoPerson.person_id)
                {
                    lobjAltPayeeAccount.LoadNexBenefitPaymentDate();
                    //BR-086-16,BR-086-09,BR-086-14
                    CalculateTaxableAndNonTaxableAmounts(lobjAltPayeeAccount, ldecNonTaxableAmount);

                    //BR-086-19,BR-086-20,BR-086-14,BR-086-15,BR-086-09
                    //function to calculate minimum guarantee amount for member
                    ibusBenefitPostRetirementDROCalculation.UpdateMinimumGuaranteeForMember(new DateTime(icdoBenefitDroApplication.date_of_divorce.Year,
                        icdoBenefitDroApplication.date_of_divorce.Month, 1), ibusMemberPayeeAccount, lobjAltPayeeAccount, icdoBenefitDroApplication.monthly_benefit_percentage, true);

                    lobjAltPayeeAccount.icdoPayeeAccount.Update();
                    break;
                }
            }

            //To get Updated Gross Monthly Benefit amount for Member
            ibusBenefitPostRetirementDROCalculation.CalculateGrossMonthlyAmount(ibusMemberPayeeAccount.idtNextBenefitPaymentDate, ibusMemberPayeeAccount);
            //to load balance minimum guarantee for member
            ibusBenefitPostRetirementDROCalculation.LoadBalanceMinimumGuarantee(ibusMemberPayeeAccount,
                                                                                new DateTime(icdoBenefitDroApplication.date_of_divorce.Year,
                                                                                icdoBenefitDroApplication.date_of_divorce.Month, 1));
            //to load balance non taxable for member
            ibusBenefitPostRetirementDROCalculation.LoadBalanceNonTaxableAmount(ibusMemberPayeeAccount, ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date);
        }

        public void CalculateTaxableAndNonTaxableAmounts(busPayeeAccount aobjAltPayeeAccount, decimal adecNonTaxableAmount)
        {
            //Function to calculate Non taxable begining balance for Alternate Payee
            ibusBenefitPostRetirementDROCalculation.CalculateNonTaxableBeginingBalanceForAltPayee(ibusMemberPayeeAccount, aobjAltPayeeAccount,
                icdoBenefitDroApplication.monthly_benefit_percentage, adecNonTaxableAmount);

            ////Function to calculate Non taxable begining balance for Member
            //ibusBenefitPostRetirementDROCalculation.CalculateNonTaxableBeginingBalanceForMember(ibusMemberPayeeAccount, aobjAltPayeeAccount);

            //Function to calculate monthly non taxable amt for alternate payee
            ibusBenefitPostRetirementDROCalculation.CalculateMonthlyNonTaxableAmountForAltPayee(aobjAltPayeeAccount);

            //BR-086-22,BR-086-09, BR-086-24, BR-086-21  
            //step to get Gross monthly benefit from computer or overriden value
            //First check whether DRO calcuation benefit amount is present, if exists use that for creating papit items for alt payee
            //else use benefit amount defined in DRO Application
            decimal ldecGrossMonthlyBenefit = icdoBenefitDroApplication.overridden_member_gross_monthly_amount > 0.0M ?
                                                icdoBenefitDroApplication.overridden_member_gross_monthly_amount :
                                                icdoBenefitDroApplication.computed_member_gross_monthly_amount;

            //Method to calculate updated tax amount for member
            ibusBenefitPostRetirementDROCalculation.CalculateAndCreatePAPIT(ibusMemberPayeeAccount, aobjAltPayeeAccount, ldecGrossMonthlyBenefit,
                icdoBenefitDroApplication.monthly_benefit_amount, ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount,
                icdoBenefitDroApplication.monthly_benefit_percentage, true);

            if (aobjAltPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                aobjAltPayeeAccount.LoadNexBenefitPaymentDate();
            if (ibusMemberPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusMemberPayeeAccount.LoadNexBenefitPaymentDate();
            DateTime ldtStartDate = ibusMemberPayeeAccount.idtNextBenefitPaymentDate > aobjAltPayeeAccount.idtNextBenefitPaymentDate ?
                ibusMemberPayeeAccount.idtNextBenefitPaymentDate : aobjAltPayeeAccount.idtNextBenefitPaymentDate;

            //Posting Non taxable amount for Alt Payee into PAPIT
            aobjAltPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxalbeAmount, ibusBenefitPostRetirementDROCalculation.idecAltPayeeNonTaxableAmt,
                string.Empty, 0, ldtStartDate, DateTime.MinValue);

            idecAPGrossMonthlyAmount = (ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount > 0 ?
                        ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount :
                        (ldecGrossMonthlyBenefit * icdoBenefitDroApplication.monthly_benefit_percentage / 100));
            idecUpdatedMonthlyAmount = ldecGrossMonthlyBenefit - icdoBenefitDroApplication.monthly_benefit_amount;
        }

        /// <summary>
        /// Main method called to perform calculation on Nullify Click
        /// </summary>
        public void NullifyActions()
        {
            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            ChangeAlternatePayeeStatus(false, DateTime.MinValue);

            //Steps to update minimum guarantee amounts
            foreach (busPayeeAccount lobjAltPayeeAccount in iclbPayeeAccount)
            {
                if (lobjAltPayeeAccount.icdoPayeeAccount.payee_perslink_id == ibusAlternatePayee.icdoPerson.person_id)
                {
                    lobjAltPayeeAccount.LoadNexBenefitPaymentDate();
                    if (lobjAltPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                        lobjAltPayeeAccount.LoadPayeeAccountPaymentItemType();
                    if (lobjAltPayeeAccount.iclbPaymentItemType == null)
                        lobjAltPayeeAccount.LoadPaymentItemType();
                    if (ibusMemberPayeeAccount != null && ibusMemberPayeeAccount.icdoPayeeAccount != null
                        && ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                    {
                        //BR-086-28
                        //Function to update minimum guarantee amount for Alternate payee and member
                        ibusBenefitPostRetirementDROCalculation.UpdateMinimumGuaranteeForMember(DateTime.Now, ibusMemberPayeeAccount, lobjAltPayeeAccount,
                                                                                                       0.0M, false);
                        //BR-086-29
                        //UpdateNonTaxable amount for Member and AlternatePayee
                        UpdateNonTaxableAmount(lobjAltPayeeAccount);
                        //Update Taxable amount for Member
                        //step to get Gross monthly benefit from computer or overriden value
                        decimal ldecGrossMonthlyBenefit = icdoBenefitDroApplication.overridden_member_gross_monthly_amount == 0.0M ?
                            icdoBenefitDroApplication.computed_member_gross_monthly_amount :
                            icdoBenefitDroApplication.overridden_member_gross_monthly_amount;
                        //BR-086-31,BR-086-09  
                        //Method to calculate updated tax amount for member
                        ibusBenefitPostRetirementDROCalculation.CalculateAndCreatePAPIT(ibusMemberPayeeAccount, lobjAltPayeeAccount, ldecGrossMonthlyBenefit,
                            icdoBenefitDroApplication.monthly_benefit_amount, ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount,
                            icdoBenefitDroApplication.monthly_benefit_percentage, false);
                        //Persisting Data
                        if (ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                            ibusMemberPayeeAccount.icdoPayeeAccount.Update();
                    }
                    else
                    {
                        //Method to post entry into Member retirement contribution
                        ibusBenefitPostRetirementDROCalculation.CalculateAndPostRetirementContributionAmounts(DateTime.Now, lobjAltPayeeAccount, ibusPersonAccount);
                    }
                    lobjAltPayeeAccount.icdoPayeeAccount.Update();
                    break;
                }
            }
            //To get Updated Gross Monthly Benefit amount for Member
            ibusBenefitPostRetirementDROCalculation.CalculateGrossMonthlyAmount(DateTime.Now, ibusMemberPayeeAccount);
            //to load balance minimum guarantee for member
            ibusBenefitPostRetirementDROCalculation.LoadBalanceMinimumGuarantee(ibusMemberPayeeAccount,
                                                                                new DateTime(icdoBenefitDroApplication.date_of_divorce.Year,
                                                                                icdoBenefitDroApplication.date_of_divorce.Month, 1));
            //to load balance non taxable for member
            ibusBenefitPostRetirementDROCalculation.LoadBalanceNonTaxableAmount(ibusMemberPayeeAccount, icdoBenefitDroApplication.benefit_receipt_date);
        }

        private void UpdateNonTaxableAmount(busPayeeAccount lobjAltPayeeAccount)
        {
            decimal ldecNonTaxableAmount = 0.0M;
            //Method to calculate total non taxable amount paid by member
            ldecNonTaxableAmount = ibusBenefitPostRetirementDROCalculation.CalculateTotalNonTaxableAmountPaid(DateTime.Now,
                                                    lobjAltPayeeAccount);
            //steps to update non taxable begining balance
            lobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance -=
                lobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance > ldecNonTaxableAmount ?
                ldecNonTaxableAmount : lobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance;
            ibusMemberPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance += lobjAltPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance;
        }

        /// <summary>
        /// Method to create payee account status (either Review or Payments Complete)
        /// </summary>
        /// <param name="iblnReview">Bool value to check whether Review record or Payment Complete record to be created</param>
        private void ChangeAlternatePayeeStatus(bool iblnReview, DateTime adtStatusEffDate)
        {
            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            busPayeeAccount lobjAltPayeeAccount =
                iclbPayeeAccount.Where(o => o.icdoPayeeAccount.payee_perslink_id == ibusAlternatePayee.icdoPerson.person_id).FirstOrDefault();
            //todo add property in payee accout to store date and use it in CreateReviewStatus
            if (lobjAltPayeeAccount != null)
            {
                if (iblnReview)
                {
                    //setting the status effective date to pending nullification date
                    if (lobjAltPayeeAccount.ibusSoftErrors == null)
                        lobjAltPayeeAccount.LoadErrors();
                    lobjAltPayeeAccount.iblnClearSoftErrors = false;
                    lobjAltPayeeAccount.ibusSoftErrors.iblnClearError = false;
                    lobjAltPayeeAccount.idtStatusEffectiveDate = adtStatusEffDate;
                    lobjAltPayeeAccount.CreateReviewPayeeAccountStatus();
                    lobjAltPayeeAccount.iblnNullifyIndicator = true;
                    lobjAltPayeeAccount.ValidateSoftErrors();
                    lobjAltPayeeAccount.UpdateValidateStatus();
                }
                else
                    lobjAltPayeeAccount.CreatePaymentCompletePayeeAccountStatus();
            }
        }

        # region Correspondence

        public override busBase GetCorPerson()
        {
            if (ibusMember == null)
                LoadMember();
            return ibusMember;
        }

        public decimal idecMemberAgeBasedOnReceiptDate
        {
            get
            {
                decimal ldecMemberAge = 0.00M;
                if (icdoBenefitDroApplication.benefit_receipt_date != DateTime.MinValue)
                {
                    if (ibusMember == null)
                        LoadMember();
                    ldecMemberAge = busGlobalFunctions.CalulateAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitDroApplication.benefit_receipt_date);

                } return ldecMemberAge;
            }
        }

        // get Member account balance
        public decimal idecMemberAccountBalance
        {
            get
            {
                //decimal ldecMemberAccountBalance = 0.00M;
                //ldecMemberAccountBalance = icdoBenefitDroApplication.overridden_capital_gain != 0 ? icdoBenefitDroApplication.overridden_capital_gain : icdoBenefitDroApplication.computed_capital_gain;
                //ldecMemberAccountBalance += icdoBenefitDroApplication.overridden_ee_er_pickup_amount != 0.00M ? icdoBenefitDroApplication.overridden_ee_er_pickup_amount : icdoBenefitDroApplication.computed_ee_er_pickup_amount;
                //ldecMemberAccountBalance += icdoBenefitDroApplication.overridden_ee_post_tax_amount != 0.00M ? icdoBenefitDroApplication.overridden_ee_post_tax_amount : icdoBenefitDroApplication.computed_ee_post_tax_amount;
                //ldecMemberAccountBalance += icdoBenefitDroApplication.overridden_ee_pre_tax_amount != 0.00M ? icdoBenefitDroApplication.overridden_ee_pre_tax_amount : icdoBenefitDroApplication.computed_ee_pre_tax_amount;
                //ldecMemberAccountBalance += icdoBenefitDroApplication.overridden_er_vested_amount != 0.00M ? icdoBenefitDroApplication.overridden_er_vested_amount : icdoBenefitDroApplication.computed_er_vested_amount;
                //ldecMemberAccountBalance += icdoBenefitDroApplication.overridden_interest_amount != 0.00M ? icdoBenefitDroApplication.overridden_interest_amount : icdoBenefitDroApplication.computed_interest_amount;

                //return ldecMemberAccountBalance;

                if (ibusBenefitDroCalculation == null)
                    LoadDROCalculation();
                return ibusBenefitDroCalculation.icdoBenefitDroCalculation.capital_gain +
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount +
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount +
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount +
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount +
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount +
                    ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest_amount;
            }
        }

        //Taxable Lumpsum amount
        public decimal idecTaxableLumpSumAmount
        {
            get
            {
                //decimal ldecEEPostTaxAmount = icdoBenefitDroApplication.overridden_ee_post_tax_amount != 0.00M ? icdoBenefitDroApplication.overridden_ee_post_tax_amount : icdoBenefitDroApplication.computed_ee_post_tax_amount;
                //return idecMemberAccountBalance - ldecEEPostTaxAmount;
                if (ibusBenefitDroCalculation == null)
                    LoadDROCalculation();
                return idecMemberAccountBalance - ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount;
            }
        }

        //Non Taxable Lumpsum amount
        public decimal idecNonTaxableLumpSumAmount
        {
            get
            {
                //decimal ldecEEPostTaxAmount = icdoBenefitDroApplication.overridden_ee_post_tax_amount != 0.00M ? icdoBenefitDroApplication.overridden_ee_post_tax_amount : icdoBenefitDroApplication.computed_ee_post_tax_amount;
                //return ldecEEPostTaxAmount;
                if (ibusBenefitDroCalculation == null)
                    LoadDROCalculation();
                return ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount;
            }
        }


        public string istrBenefitLife
        {
            get
            {
                string lstrBenefitOption = String.Empty;
                if (!String.IsNullOrEmpty(icdoBenefitDroApplication.benefit_duration_option_value))
                {

                    string lstrData3 = busGlobalFunctions.GetData3ByCodeValue(2404, icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);
                    string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2216, lstrData3, iobjPassInfo);
                    if (istrLifeOfAP == "1")
                        lstrBenefitOption = "Life Of Alternate Payee";
                    else
                        lstrBenefitOption = "Life Of Benefit Account Owner";

                }
                return lstrBenefitOption;
            }
        }

        public string istrDurationOfPayments
        {
            get
            {
                string lstrDurationOfPayments = String.Empty;
                if (!String.IsNullOrEmpty(icdoBenefitDroApplication.benefit_duration_option_value))
                {
                    string lstrData3 = busGlobalFunctions.GetData3ByCodeValue(2404, icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);
                    lstrDurationOfPayments = busGlobalFunctions.GetData3ByCodeValue(2216, lstrData3, iobjPassInfo);
                }
                return lstrDurationOfPayments;
            }
        }

        public string istrLifeOfAP
        {
            get
            {
                string lstrData2 = string.Empty;
                if (!String.IsNullOrEmpty(icdoBenefitDroApplication.benefit_duration_option_value))
                {
                    string lstrData3 = busGlobalFunctions.GetData3ByCodeValue(2404, icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);
                    lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2216, lstrData3, iobjPassInfo);
                }
                if (lstrData2 == "AP")
                    return "1";
                else
                    return "0";
            }
        }

        public string istrLifeOfAp10Or20
        {
            get
            {
                if ((istrLifeOfAP == "1")
                    && ((iintTermCertain == 10)
                    || (iintTermCertain == 20)))
                    return "1";
                else
                    return "0";
            }
        }


        public string istrLifeOfBenefitAccOwner10Or20
        {
            get
            {
                if ((istrLifeOfBenefitOwner == "1")
                    && ((iintTermCertain == 10)
                    || (iintTermCertain == 20)))
                    return "1";
                else
                    return "0";
            }
        }

        public string istrLifeOfBenefitOwner
        {
            get
            {
                string lstrData2 = string.Empty;
                if (!String.IsNullOrEmpty(icdoBenefitDroApplication.benefit_duration_option_value))
                {
                    string lstrData3 = busGlobalFunctions.GetData3ByCodeValue(2404, icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);
                    lstrData2 = busGlobalFunctions.GetData2ByCodeValue(2216, lstrData3, iobjPassInfo);
                }
                if (lstrData2 == "BAO")
                    return "1";
                else
                    return "0";
            }
        }

        public int iintTermCertain
        {
            get
            {
                int lintData1 = 0;
                if (!String.IsNullOrEmpty(icdoBenefitDroApplication.benefit_duration_option_value))
                {
                    string lstrData3 = busGlobalFunctions.GetData3ByCodeValue(2404, icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);
                    string lstrData1 = busGlobalFunctions.GetData1ByCodeValue(2216, lstrData3, iobjPassInfo);
                    lintData1 = string.IsNullOrEmpty(lstrData1) ? 0 : Convert.ToInt32(lstrData1);
                }
                return lintData1;
            }
        }

        public DateTime idecYearTermCertainEndDate
        {
            get
            {
                DateTime ldtReturnDate = DateTime.MinValue;
                if (iintTermCertain > 0)
                {
                    if (icdoBenefitDroApplication.benefit_receipt_date != DateTime.MinValue)
                        ldtReturnDate = icdoBenefitDroApplication.benefit_receipt_date.AddYears(iintTermCertain);
                }
                return ldtReturnDate;
            }
        }

        public string istrYearTermCertainEndDate
        {
            get
            {
                if (idecYearTermCertainEndDate == DateTime.MinValue)
                    return string.Empty;
                else
                    return idecYearTermCertainEndDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        public string istrDROModel
        {
            get
            {
                string lstrResult = "0";
                if (!string.IsNullOrEmpty(icdoBenefitDroApplication.dro_model_value))
                {
                    if ((icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBModel)
                        || (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel))
                    {
                        lstrResult = "1";
                    }
                    if ((icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel)
                    || (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel))
                    {
                        lstrResult = "2";
                    }
                }
                return lstrResult;
            }
        }

        public busBenefitCalculation ibusBenefitCalculation { get; set; }

        public decimal idecMemberFAS { get; set; }

        public decimal idecAccruedMemberAmount { get; set; }

        public decimal idecAccruedAPAmount { get; set; }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            GetBenefitDurationFullDescription();
            LoadBenefitReceiptDate();
            LoadBenefitCalculation();
            GetFAS();
            LoadMemberGrossBenefitAmount();
            if (icdoBenefitDroApplication.monthly_benefit_percentage < 100)
            {
                idecAccruedMemberAmount = Math.Round((icdoBenefitDroApplication.monthly_benefit_amount * 100) / (100 - icdoBenefitDroApplication.monthly_benefit_percentage),
                    2, MidpointRounding.AwayFromZero);
            }
            else
            {
                idecAccruedMemberAmount = icdoBenefitDroApplication.monthly_benefit_amount;
            }
            idecAccruedAPAmount = Math.Round(idecAccruedMemberAmount * icdoBenefitDroApplication.monthly_benefit_percentage / 100, 2, MidpointRounding.AwayFromZero);
            if (ibusAlternatePayee == null)
                LoadAlternatePayee();
            if (ibusAlternatePayee.ibusPersonCurrentAddress == null)
                ibusAlternatePayee.LoadPersonCurrentAddress();
            if (ibusMember == null)
                LoadMember();
            if (ibusMember.ibusPersonCurrentAddress == null)
                ibusMember.LoadPersonCurrentAddress();
        }

        private void LoadMemberGrossBenefitAmount()
        {
            if (ibusBenefitPostRetirementDROCalculation == null)
                LoadPostRetirementDROCalculation();
            DateTime ldtPERSLinkLive = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.PERSLinkGoLiveDate, iobjPassInfo));
            DateTime ldtDivorceDate = new DateTime(icdoBenefitDroApplication.date_of_divorce.Year, icdoBenefitDroApplication.date_of_divorce.Month, 1);
            ibusBenefitPostRetirementDROCalculation.CalculateGrossMonthlyAmount(ldtDivorceDate > ldtPERSLinkLive ? ldtDivorceDate :
            ldtPERSLinkLive, ibusMemberPayeeAccount);
        }

        private void LoadBenefitReceiptDate()
        {
            if (ibusBenefitDroCalculation == null)
                LoadDROCalculation();
            if (icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetiredJobServiceModel ||
                icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelRetireeDBModel)
                icdoBenefitDroApplication.idtBenefitReceiptDate = ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date;
            else
                icdoBenefitDroApplication.idtBenefitReceiptDate = icdoBenefitDroApplication.benefit_receipt_date;
        }

        private void GetFAS()
        {
            if (ibusBenefitCalculation.icdoBenefitCalculation.overridden_final_average_salary > 0.0M)
                idecMemberFAS = ibusBenefitCalculation.icdoBenefitCalculation.overridden_final_average_salary;
            else if (ibusBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary > 0.0M)
                idecMemberFAS = ibusBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary;
            //else
            //    idecMemberFAS = ibusBenefitCalculation.icdoBenefitCalculation.indexed_final_average_salary;
        }

        private void LoadBenefitCalculation()
        {
            if (ibusBenefitCalculation == null)
                ibusBenefitCalculation = new busBenefitCalculation();
            if (ibusMemberPayeeAccount == null)
                ibusMemberPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };

            ibusBenefitCalculation.FindBenefitCalculation(ibusMemberPayeeAccount.icdoPayeeAccount.calculation_id);
        }

        private void GetBenefitDurationFullDescription()
        {
            string lstrBenefitDuration = string.Empty;
            if (!String.IsNullOrEmpty(icdoBenefitDroApplication.benefit_duration_option_value))
            {
                string lstrData3 = busGlobalFunctions.GetData3ByCodeValue(2404, icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);
                lstrBenefitDuration = busGlobalFunctions.GetCommentsByCodeValue(2216, lstrData3, iobjPassInfo);
            }
            icdoBenefitDroApplication.istrBenefitDurationFullDesc = lstrBenefitDuration;
        }
        # endregion

        private Collection<busPayeeAccount> _iclbPayeeAccount;
        public Collection<busPayeeAccount> iclbPayeeAccount
        {
            get { return _iclbPayeeAccount; }
            set { _iclbPayeeAccount = value; }
        }

        public void LoadPayeeAccount()
        {
            if (iclbPayeeAccount.IsNull())
                iclbPayeeAccount = new Collection<busPayeeAccount>();
            DataTable ldtbResult = Select<cdoPayeeAccount>(
                                      new string[1] { "dro_application_id" },
                                      new object[1] { icdoBenefitDroApplication.dro_application_id }, null, null);
            iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtbResult, "icdoPayeeAccount");
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
                lobjPayeeAccount.LoadPayee();
        }


        //prod pir 6420
        //--start--//

        public void DROQualifyForRetireeModels()
        {
            if (ibusBenefitPostRetirementDROCalculation.icdoBenefitDroCalculation.dro_calculation_id <= 0)
                LoadPostRetirementDROCalculation();

            //Method to calculate total non taxable amount paid by member
            decimal ldecNonTaxableAmount = ibusBenefitPostRetirementDROCalculation.CalculateTotalNonTaxableAmountPaid(ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date,
                ibusMemberPayeeAccount);
            
            //BR-086-16,BR-086-09,BR-086-14
            CalculateTaxableAndNonTaxableAmounts(ldecNonTaxableAmount);

            //BR-086-19,BR-086-20,BR-086-14,BR-086-15,BR-086-09
            //function to calculate minimum guarantee amount for member
            ibusBenefitPostRetirementDROCalculation.UpdateMinimumGuaranteeForMember(new DateTime(icdoBenefitDroApplication.date_of_divorce.Year,
                icdoBenefitDroApplication.date_of_divorce.Month, 1), ibusMemberPayeeAccount, icdoBenefitDroApplication.monthly_benefit_percentage);

            //Persisting Data
            if (ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                ibusMemberPayeeAccount.icdoPayeeAccount.Update();                   
                
            //To get Updated Gross Monthly Benefit amount for Member
            ibusBenefitPostRetirementDROCalculation.CalculateGrossMonthlyAmount(ibusMemberPayeeAccount.idtNextBenefitPaymentDate, ibusMemberPayeeAccount);
            //to load balance minimum guarantee for member
            ibusBenefitPostRetirementDROCalculation.LoadBalanceMinimumGuarantee(ibusMemberPayeeAccount,
                                                                                new DateTime(icdoBenefitDroApplication.date_of_divorce.Year,
                                                                                icdoBenefitDroApplication.date_of_divorce.Month, 1));
            //to load balance non taxable for member
            ibusBenefitPostRetirementDROCalculation.LoadBalanceNonTaxableAmount(ibusMemberPayeeAccount, ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date);

            ibusBenefitPostRetirementDROCalculation.icdoBenefitDroCalculation.Update();

            LoadDROCalculation();
        }

        private void CalculateTaxableAndNonTaxableAmounts(decimal adecNonTaxableAmount)
        {            
            //Function to calculate Non taxable begining balance for Member
            ibusBenefitPostRetirementDROCalculation.CalculateNonTaxableBeginingBalanceForMember(ibusMemberPayeeAccount, icdoBenefitDroApplication.monthly_benefit_percentage, 
                adecNonTaxableAmount);

            //BR-086-22,BR-086-09, BR-086-24, BR-086-21  
            //step to get Gross monthly benefit from computer or overriden value
            //First check whether DRO calcuation benefit amount is present, if exists use that for creating papit items for alt payee
            //else use benefit amount defined in DRO Application
            decimal ldecGrossMonthlyBenefit = icdoBenefitDroApplication.overridden_member_gross_monthly_amount > 0.0M ?
                                                icdoBenefitDroApplication.overridden_member_gross_monthly_amount :
                                                icdoBenefitDroApplication.computed_member_gross_monthly_amount;

            //Method to calculate updated tax amount for member
            ibusBenefitPostRetirementDROCalculation.CalculateAndCreatePAPIT(ibusMemberPayeeAccount, ldecGrossMonthlyBenefit,
                icdoBenefitDroApplication.monthly_benefit_amount, ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount,
                icdoBenefitDroApplication.monthly_benefit_percentage);
            
            //idecAPGrossMonthlyAmount = (ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount > 0 ?
            //            ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount :
            //            (ldecGrossMonthlyBenefit * icdoBenefitDroApplication.monthly_benefit_percentage / 100));
            //idecUpdatedMonthlyAmount = ldecGrossMonthlyBenefit - icdoBenefitDroApplication.monthly_benefit_amount;
        }

        public override void AfterSetActivityInstance()
        {
            base.AfterSetActivityInstance();
            SetBenAppRecivedDate();
        }
        public void SetBenAppRecivedDate() => icdoBenefitDroApplication.received_date = (ibusBaseActivityInstance.IsNotNull() &&
            ibusBaseActivityInstance is busBpmActivityInstance && icdoBenefitDroApplication.received_date == DateTime.MinValue) ?
            ((busBpmActivityInstance)ibusBaseActivityInstance).icdoBpmActivityInstance.created_date.Date : icdoBenefitDroApplication.received_date.Date;
        //--end--//

        //PIR 22861 'Expiring’ text visible logic 
        public bool IsExpiring()
        {
            if ((busGlobalFunctions.GetSysManagementBatchDate() >= icdoBenefitDroApplication.received_date.AddMonths(15)) &&
                 (icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusRecieved ||
                icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusApproved))
            {
                return true;
            }
            return false;
        }
    }
}