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
using Sagitec.CustomDataObjects;
using System.Linq;
using System.Collections.Generic;

#endregion


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPostRetirementDeathApplication : busBenefitApplication
    {
        private Collection<busBenefitApplication> _iclbOtherApplications;
        public Collection<busBenefitApplication> iclbOtherApplications
        {
            get { return _iclbOtherApplications; }
            set { _iclbOtherApplications = value; }
        }
		
		//PIR 14391
        public busBenefitApplication ibusBenefitApplication { get; set; }
		
		//PIR 14391
        public void LoadBenefitApplication()
        {
            if (ibusBenefitApplication.IsNull())
                ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            ibusBenefitApplication.icdoBenefitApplication = icdoBenefitApplication;
            ibusBenefitApplication.ibusPersonAccount = ibusPersonAccount;
        }

        # region Load Methods

        public void LoadOtherApplications()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            if (ibusPersonAccount.ibusPerson.iclbBenefitApplication == null)
                ibusPersonAccount.ibusPerson.LoadBenefitApplication();

            if (ibusOriginatingPayeeAccount.IsNull())
                LoadOriginatingPayeeAccount();

            _iclbOtherApplications = new Collection<busBenefitApplication>();

            if ((icdoBenefitApplication.post_retirement_death_reason_type_value == busConstant.PostRetirementFirstBeneficiaryDeath) ||
                (icdoBenefitApplication.post_retirement_death_reason_type_value == busConstant.PostRetirementAlternatePayeeDeath))
            {

                if (icdoBenefitApplication.account_relationship_value == busConstant.AccountRelationshipBeneficiary)
                {
                    busPerson lobjBenePerson = new busPerson();
                    if (lobjBenePerson.FindPerson(icdoBenefitApplication.beneficiary_person_id))
                    {
                        if (icdoBenefitApplication.post_retirement_death_reason_type_value == busConstant.PostRetirementFirstBeneficiaryDeath)
                        {
                            if (lobjBenePerson.iclbPersonBeneficiary.IsNull())
                                lobjBenePerson.LoadApplicationBeneficiary();

                            Collection<busPersonBeneficiary> lclbPersonBenficiary = new Collection<busPersonBeneficiary>();
                            foreach (busPersonBeneficiary lobjPersonBeneficiary in lobjBenePerson.iclbPersonBeneficiary)
                            {
                                if ((lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.benefit_application_id ==
                                    ibusOriginatingPayeeAccount.icdoPayeeAccount.application_id) && (lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary
                                    .icdoBenefitApplicationBeneficiary.beneficiary_type_value==busConstant.BeneficiaryMemberTypePrimary))
                                    lclbPersonBenficiary.Add(lobjPersonBeneficiary);
                            }
                            lobjBenePerson.iclbPersonBeneficiary = lclbPersonBenficiary;
                        }
                        else
                        {
                            if (lobjBenePerson.iclbPersonBeneficiary.IsNull())
                                lobjBenePerson.LoadApplicationBeneficiary();

                            Collection<busPersonBeneficiary> lclbDROPersonBenficiary = new Collection<busPersonBeneficiary>();
                            foreach (busPersonBeneficiary lobjPersonBeneficiary in lobjBenePerson.iclbPersonBeneficiary)
                            {
                                if ((lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id ==
                                    ibusOriginatingPayeeAccount.icdoPayeeAccount.dro_application_id) && (lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary
                                    .icdoBenefitApplicationBeneficiary.beneficiary_type_value==busConstant.BeneficiaryMemberTypePrimary))
                                    lclbDROPersonBenficiary.Add(lobjPersonBeneficiary);
                            }
                            lobjBenePerson.iclbPersonBeneficiary = lclbDROPersonBenficiary;
                        }

                        if (icdoBenefitApplication.post_retirement_death_reason_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                        {
                            if (lobjBenePerson.iclbBenefitApplication.IsNull())
                                lobjBenePerson.LoadBenefitApplication();
                        }
                        else
                        {
                            if (ibusPerson.IsNull())
                                LoadPerson();


                            if (ibusPerson.iclbBenefitApplication.IsNull())
                                ibusPerson.LoadBenefitApplication();

                            lobjBenePerson.iclbBeneficiaryApplication = ibusPerson.iclbBenefitApplication;
                        }

                        foreach (busPersonBeneficiary lobjPersonBen in lobjBenePerson.iclbPersonBeneficiary)
                        {

                            var lclbOtherApplications = ibusPersonAccount.ibusPerson.iclbBenefitApplication
                                                        .Where(lobjBA => lobjBA.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath
                                                        && lobjBA.icdoBenefitApplication.post_retirement_death_reason_type_value == icdoBenefitApplication.post_retirement_death_reason_type_value
                                                        && lobjBA.icdoBenefitApplication.benefit_application_id != icdoBenefitApplication.benefit_application_id
                                                        && lobjBA.icdoBenefitApplication.originating_payee_account_id == icdoBenefitApplication.originating_payee_account_id
                                                        && lobjBA.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled
                                                        && lobjBA.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied);

                            if (lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id > 0)
                            {
                                lclbOtherApplications = lclbOtherApplications.Where(lobjBA => lobjBA.icdoBenefitApplication.recipient_person_id == lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id);
                            }
                            else
                            {
                                lclbOtherApplications = lclbOtherApplications.Where(lobjBA => lobjBA.icdoBenefitApplication.payee_org_id == lobjPersonBen.icdoPersonBeneficiary.benificiary_org_id);
                            }

                            if (lclbOtherApplications.Count() > 0)
                            {
                                foreach (busBenefitApplication lobjApplication in lclbOtherApplications)
                                {
                                    if (lobjApplication.icdoBenefitApplication.recipient_person_id > 0)
                                    {
                                        //if (lobjApplication.icdoBenefitApplication.recipient_person_id == lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id)
                                        //{
                                        lobjApplication.icdoBenefitApplication.istrBeneficiaryName = lobjPersonBen.icdoPersonBeneficiary.beneficiary_name;
                                        iclbOtherApplications.Add(lobjApplication);
                                        break;
                                        //}
                                    }
                                    else
                                    {
                                        //if (lobjApplication.icdoBenefitApplication.payee_org_id == lobjPersonBen.icdoPersonBeneficiary.benificiary_org_id)
                                        //{
                                        lobjApplication.icdoBenefitApplication.istrBeneficiaryName = lobjPersonBen.icdoPersonBeneficiary.beneficiary_name;
                                        iclbOtherApplications.Add(lobjApplication);
                                        break;
                                        //}
                                    }
                                }
                            }
                            else
                            {
                                if (icdoBenefitApplication.recipient_person_id > 0)
                                {
                                    if (icdoBenefitApplication.recipient_person_id != lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id)
                                    {
                                        busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                                        lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id;
                                        lobjBenefitApplication.icdoBenefitApplication.istrBeneficiaryName = lobjPersonBen.icdoPersonBeneficiary.beneficiary_name;
                                        iclbOtherApplications.Add(lobjBenefitApplication);

                                    }
                                }
                                else if (icdoBenefitApplication.payee_org_id > 0)
                                {
                                    if (icdoBenefitApplication.payee_org_id != lobjPersonBen.icdoPersonBeneficiary.benificiary_org_id)
                                    {
                                        busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                                        lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = 0;
                                        lobjBenefitApplication.icdoBenefitApplication.istrBeneficiaryName = lobjPersonBen.icdoPersonBeneficiary.beneficiary_name;
                                        iclbOtherApplications.Add(lobjBenefitApplication);

                                    }
                                }
                            }
                        }

                    }
                }
            }
            else if (icdoBenefitApplication.account_relationship_value == busConstant.AccountRelationshipBeneficiary)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();
                if (ibusPerson.iclbPersonBeneficiary.IsNull())
                    ibusPerson.LoadBeneficiary();

                foreach (busPersonBeneficiary lobjPersonBen in ibusPerson.iclbPersonBeneficiary)
                {
                    if (lobjPersonBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
                    {
                        if (lobjPersonBen.ibusPersonAccountBeneficiary.ibusPersonAccount.IsNull())
                            lobjPersonBen.ibusPersonAccountBeneficiary.LoadPersonAccount();
                        if (lobjPersonBen.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == icdoBenefitApplication.plan_id &&
                            lobjPersonBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary && // PROD PIR ID 7076
                            busGlobalFunctions.CheckDateOverlapping(lobjPersonBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                                                                    lobjPersonBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date,
                                                                    ibusPerson.icdoPerson.date_of_death)) // PROD PIR 8907
                        {
                            var lclbOtherApplications = ibusPersonAccount.ibusPerson.iclbBenefitApplication
                                                        .Where(lobjBA => lobjBA.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath
                                                        && lobjBA.icdoBenefitApplication.post_retirement_death_reason_type_value == icdoBenefitApplication.post_retirement_death_reason_type_value
                                                        && lobjBA.icdoBenefitApplication.benefit_application_id != icdoBenefitApplication.benefit_application_id
                                                        && lobjBA.icdoBenefitApplication.originating_payee_account_id == icdoBenefitApplication.originating_payee_account_id
                                                        && lobjBA.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled
                                                        && lobjBA.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied);

                            if (lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id > 0)
                            {
                                lclbOtherApplications = lclbOtherApplications.Where(lobjBA => lobjBA.icdoBenefitApplication.recipient_person_id == lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id);
                            }
                            else
                            {
                                lclbOtherApplications = lclbOtherApplications.Where(lobjBA => lobjBA.icdoBenefitApplication.payee_org_id == lobjPersonBen.icdoPersonBeneficiary.benificiary_org_id);
                            }
                            if (lclbOtherApplications.Count() > 0)
                            {
                                foreach (busBenefitApplication lobjApplication in lclbOtherApplications)
                                {
                                    if (lobjApplication.icdoBenefitApplication.recipient_person_id > 0)
                                    {
                                        //if (lobjApplication.icdoBenefitApplication.recipient_person_id == lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id)
                                        //{
                                        lobjApplication.icdoBenefitApplication.istrBeneficiaryName = lobjPersonBen.icdoPersonBeneficiary.beneficiary_name;
                                        iclbOtherApplications.Add(lobjApplication);
                                        break;
                                        //}
                                    }
                                    else
                                    {
                                        //if (lobjApplication.icdoBenefitApplication.payee_org_id == lobjPersonBen.icdoPersonBeneficiary.benificiary_org_id)
                                        //{
                                        lobjApplication.icdoBenefitApplication.istrBeneficiaryName = lobjPersonBen.icdoPersonBeneficiary.beneficiary_name;
                                        iclbOtherApplications.Add(lobjApplication);
                                        break;
                                        //}
                                    }
                                }
                            }
                            else
                            {
                                if (icdoBenefitApplication.recipient_person_id > 0)
                                {
                                    if (icdoBenefitApplication.recipient_person_id != lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id)
                                    {
                                        busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                                        lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id;
                                        lobjBenefitApplication.icdoBenefitApplication.istrBeneficiaryName = lobjPersonBen.icdoPersonBeneficiary.beneficiary_name;
                                        iclbOtherApplications.Add(lobjBenefitApplication);

                                    }
                                }
                                else if (icdoBenefitApplication.payee_org_id > 0)
                                {
                                    if (icdoBenefitApplication.payee_org_id != lobjPersonBen.icdoPersonBeneficiary.benificiary_org_id)
                                    {
                                        busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                                        lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = 0;
                                        lobjBenefitApplication.icdoBenefitApplication.istrBeneficiaryName = lobjPersonBen.icdoPersonBeneficiary.beneficiary_name;
                                        iclbOtherApplications.Add(lobjBenefitApplication);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LoadBenefitCalculationID()
        {
            // PROD PIR ID 5729 -- If calculation is "canceled" do not link new application to canceled calculation.
            DataTable ldtbResult = SelectWithOperator<cdoBenefitCalculation>(
                                        new string[7] { "person_id", "plan_id", "benefit_account_type_value", "Calculation_Type_Value", 
                                                        "post_retirement_death_reason_type_value","originating_payee_account_id", "action_status_value"},
                                        new string[7] { "=", "=", "=", "=", "=", "=", "<>" },
                                        new object[7] { icdoBenefitApplication.member_person_id, 
                                                        icdoBenefitApplication.plan_id,
                                                        icdoBenefitApplication.benefit_account_type_value,
                                                        busConstant.CalculationTypeFinal,
                                                        icdoBenefitApplication.post_retirement_death_reason_type_value,
                                                        icdoBenefitApplication.originating_payee_account_id,
                                                        busConstant.ApplicationActionStatusCancelled}, "benefit_calculation_id desc");
            if (ldtbResult.Rows.Count > 0)
            {
                icdoBenefitApplication.benefit_calculation_id = Convert.ToInt32(ldtbResult.Rows[0]["benefit_calculation_id"]);
            }
        }

        //BR-54-18
        //Set values for RTW member
        //check in only in New mode
        public void SetFieldsForRTWMember()
        {
            int lintPayeeAccount = 0;
            if (ibusPersonAccount.ibusPerson.IsRTWMember(icdoBenefitApplication.plan_id, busConstant.PayeeStatusForRTW.IgnoreStatus, ref lintPayeeAccount))
            {
                iblnIsRTWMember = true;
                if (lintPayeeAccount == icdoBenefitApplication.originating_payee_account_id)
                {
                    if (ibusOriginatingPayeeAccount.IsNull())
                        LoadOriginatingPayeeAccount();
                    if (ibusOriginatingPayeeAccount.ibusApplication.IsNull())
                        ibusOriginatingPayeeAccount.LoadApplication();

                    icdoBenefitApplication.rtw_refund_election_value = busConstant.Flag_No_Value.ToUpper();
                    icdoBenefitApplication.pre_rtw_payeeaccount_id = lintPayeeAccount;
                    //set RTW flag based on the PSC as on date of death
                    ibusPersonAccount.LoadTotalPSC(ibusPerson.icdoPerson.date_of_death);
                    SetIsRTWFlagLessThan2Years(ibusPersonAccount.idecTotalPSC_Rounded);

                    //load all RTW person accounts
                    LoadRTWPersonAccount();

                    //set SSLI
                    icdoBenefitApplication.uniform_income_flag = ibusOriginatingPayeeAccount.ibusApplication.icdoBenefitApplication.uniform_income_flag;
                    icdoBenefitApplication.ssli_age = ibusOriginatingPayeeAccount.ibusApplication.icdoBenefitApplication.ssli_age;
                    icdoBenefitApplication.estimated_ssli_benefit_amount = ibusOriginatingPayeeAccount.ibusApplication.icdoBenefitApplication.estimated_ssli_benefit_amount;

                }
            }
        }
        #endregion

        # region Rules

        //BR-054-16,
        //BR-054-17
        //check if the member is having valid member balance amount 
        public bool IsMemberHavingMemberBalanceAmountisValid()
        {
            if (ibusOriginatingPayeeAccount.IsNull())
                LoadOriginatingPayeeAccount();

            if (((ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                || (ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                && (ibusOriginatingPayeeAccount.icdoPayeeAccount.dro_application_id == 0))
            {
                //1. get person account retirement details
                if (ibusOriginatingPayeeAccount.ibusApplication.IsNull())
                    ibusOriginatingPayeeAccount.LoadApplication();
                if (ibusOriginatingPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts.IsNull())
                    ibusOriginatingPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();

                var lobjBAPersonAccount = ibusOriginatingPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts
                                            .Where(lobjBAPA => lobjBAPA.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).FirstOrDefault();
                if (lobjBAPersonAccount.IsNotNull())
                {
                    lobjBAPersonAccount.LoadPersonAccountRetirement();

                    //3. check for amount is valid
                    if (((ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                        || (ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                        && (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionRefund))
                    {
                        ibusOriginatingPayeeAccount.LoadPaymentDetails();
                        decimal ldecResultAmount = 0.0M;
                        //UAT PIR:1224
                        //For Disability since the Minimum Guarantee is not stored, it is calculated as on date of death so that the interest amount is added.
                        //do not minus the paid amount
                        //2. get LTD summary
                        if (ibusPersonAccount.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                        {
                            lobjBAPersonAccount.ibusPersonAccountRetirement.LoadLTDSummaryForCalculation(ibusPersonAccount.ibusPerson.icdoPerson.date_of_death,
                                                                                        ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value);
                        }

                        if (ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                        {
                            ldecResultAmount = lobjBAPersonAccount.ibusPersonAccountRetirement.Member_Account_Balance_ltd;

                        }
                        else
                        {
                            ldecResultAmount = ibusOriginatingPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount
                                - ibusOriginatingPayeeAccount.idecpaidgrossamount;
                        }

                        if (ldecResultAmount <= 0.00M)
                            return false;
                    }
                    else
                    {
                        //if the option is not refund then the method should written true; since no need to check for the Member account balance.
                        return true;
                    }
                }
                else
                    return false;
            }
            return true;
        }

        //Set Person Account
        public void SetPersonAccount()
        {
            if (iclbBenefitApplicationPersonAccounts.IsNull())
                LoadBenefitApplicationPersonAccount();
            var lobjBAPersonAccount = iclbBenefitApplicationPersonAccounts.Where(lobjBA => lobjBA.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).FirstOrDefault();
            if (lobjBAPersonAccount.IsNotNull())
            {
                if (lobjBAPersonAccount.ibusPersonAccount.IsNull())
                    lobjBAPersonAccount.LoadPersonAccount();
                ibusPersonAccount = lobjBAPersonAccount.ibusPersonAccount;
            }
        }

        //BR-54-18
        //to check if the plan is DC and 
        public bool IsRefundIsApplicableForPlanDC()
        {
            if (icdoBenefitApplication.plan_id == busConstant.PlanIdDC ||
                icdoBenefitApplication.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                icdoBenefitApplication.plan_id == busConstant.PlanIdDC2025) //PIR 25920
            {
                if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionRefund)
                {
                    if (ibusOriginatingPayeeAccount.IsNull())
                        LoadOriginatingPayeeAccount();
                    if (ibusOriginatingPayeeAccount.ibusApplication.IsNull())
                        ibusOriginatingPayeeAccount.LoadApplication();
                    if (!(ibusOriginatingPayeeAccount.ibusApplication.icdoBenefitApplication.rhic_option_value.Equals(busConstant.RHICOptionStandard)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        //BR- 31
        private bool IsDeathNotificationExistForMember()
        {
           
            Collection<busDeathNotification> iclbDeathNotification = new Collection<busDeathNotification>();

            DataTable ldtbDeathNotification = Select<cdoDeathNotification>(new string[1] { "person_id" }, new object[1] { iintAccountPayeeId }, null, null);
            iclbDeathNotification = GetCollection<busDeathNotification>(ldtbDeathNotification,"icdoDeathNotification");

            if ( iclbDeathNotification.Where(i => i.icdoDeathNotification.action_status_value != busConstant.DeathNotificationActionStatusCancelled
                   && i.icdoDeathNotification.action_status_value != busConstant.DeathNotificationActionStatusErroneous
                   && i.icdoDeathNotification.death_certified_flag == busConstant.Flag_Yes).Count() > 0)
            {
                return true;
            }
         return false;
        }
        # endregion

        # region Button Logic

        public override ArrayList btnVerfiyClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            //if (icdoBenefitApplication.termination_date == DateTime.MinValue)
            //{
            //    lobjError = AddError(1946, "");
            //    alReturn.Add(lobjError);
            //    return alReturn;
            //}
            if (!IsDeathNotificationExistForMember())
            {
                lobjError = AddError(1992, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            alReturn = base.btnVerfiyClicked();
            return alReturn;
        }

        public override ArrayList btnPendingVerificationClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            alReturn = base.btnPendingVerificationClicked();
            return alReturn;
        }
        public override ArrayList btnDenyClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            IsDeniedButtonClicked = true;
            alReturn = base.btnDenyClicked();
            return alReturn;
        }
        #endregion

        # region Override methods
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            SetOrgIdAsLatestEmploymentOrgId();
            base.BeforeValidate(aenmPageMode);
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadOtherApplications();
            /*****************************************************************************************************************************************
            * Change from Raj on the Calculation being set to Review when the Calculation is not in Review Status and Action status is not approved.
            * Code Starts here
            ******************************************************************************************************************************************/
            if (icdoBenefitApplication.benefit_calculation_id == 0)
                LoadBenefitCalculationID();

            busPostRetirementDeathBenefitCalculation ibusPostRetirementDeathBenefitCalculation = new busPostRetirementDeathBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            if (icdoBenefitApplication.benefit_calculation_id > 0)
            {
                ibusPostRetirementDeathBenefitCalculation.FindBenefitCalculation(icdoBenefitApplication.benefit_calculation_id);
                if (ibusPostRetirementDeathBenefitCalculation.IsNotNull())
                {
                    if (ibusPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.status_value != busConstant.CalculationStatusReview)
                    {
                        ibusPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.status_value = busConstant.CalculationStatusReview;
                        ibusPostRetirementDeathBenefitCalculation.icdoBenefitCalculation.Update();

                        ibusPostRetirementDeathBenefitCalculation.iblnIsApplicationModified = true;
                        ibusPostRetirementDeathBenefitCalculation.ValidateSoftErrors();
                    }
                }
            }
            /************************************************************************************************************************************************
             * Change from Raj on the Calculation being set to Review when the Calculation is not in Review Status and Action status is not approved.
             * Code Ends here
            *************************************************************************************************************************************************/
        }

        public override void BeforePersistChanges()
        {
            DateTime ldtRetTempDate = ibusPersonAccount.ibusPerson.icdoPerson.date_of_death.AddMonths(1);
            icdoBenefitApplication.retirement_date = new DateTime(ldtRetTempDate.Year, ldtRetTempDate.Month, 1);
            icdoBenefitApplication.benefit_sub_type_value = busConstant.BenefitAccountSubTypePostRetDeath;
            base.BeforePersistChanges();
        }

        # endregion

        # region Validate benefit Option
        public busDBCacheData ibusDBCacheData { get; set; }
        //BR 18,19,20
        public void ValidatePostRetirementDeathApplication(int aintAccountOwnerId, int aintRecipientPersonId, int aintRecipientOrgid,
                                                            int aintoriginatingPayeeAccountId, int aintPlanID, bool astrIsPersonMarried,
                                                            ref string astrDeathReasonvalue, ref string astrBenefitOption,
                                                            ref DateTime adtTerminationdate, ref string astrAccountrelationshipvalue)
        {
            string lstrIsMonthlyBenefitFlag = String.Empty;
            string lstrIsSpouseAtDeath = String.Empty;
            string lstrBenefitOption = String.Empty;

            if (ibusPlan.IsNull())
                LoadPlan();

            if (aintoriginatingPayeeAccountId > 0)
            {
                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                if (lobjPayeeAccount.FindPayeeAccount(aintoriginatingPayeeAccountId))
                {
                    lobjPayeeAccount.LoadPayeePerson();
                    //check if DRO Application
                    if (lobjPayeeAccount.icdoPayeeAccount.dro_application_id > 0)
                    {
                        //check if account owner is the payee account owner 
                        lobjPayeeAccount.LoadDROApplication();
                        if (lobjPayeeAccount.ibusPayee.IsNull())
                            lobjPayeeAccount.LoadPayeePerson();
                        //*********************ALTERNATE PAYEE DEATH
                        if ((lobjPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.member_perslink_id == aintAccountOwnerId)
                            && (lobjPayeeAccount.ibusPayee.icdoPerson.date_of_death != DateTime.MinValue))
                        {
                            string lstrBenefitAccountType = null;
                            //load death benefit options
                            bool iblnIsTermCertainDatePastDateofDeath = IsTermCertainDatePastDateofDeath(busConstant.PostRetirementAlternatePayeeDeath,
                                lobjPayeeAccount.icdoPayeeAccount.term_certain_end_date,
                                lobjPayeeAccount.ibusPayee.icdoPerson.date_of_death, lobjPayeeAccount.icdoPayeeAccount.benefit_option_value);

                            FetchDeathBenefitOption(ibusPlan.icdoPlan.benefit_provision_id,
                                                     lstrBenefitAccountType, astrIsPersonMarried,
                                                     lobjPayeeAccount.icdoPayeeAccount.benefit_option_value, busConstant.PostRetirementAlternatePayeeDeath,
                                                     false, ref lstrBenefitOption,
                                                     ref astrAccountrelationshipvalue, ref lstrIsSpouseAtDeath, ref lstrIsMonthlyBenefitFlag,
                                                     iblnIsTermCertainDatePastDateofDeath, iobjPassInfo);

                            if (IsBeneficiary(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, busConstant.PostRetirementAlternatePayeeDeath))
                            {
                                astrBenefitOption = lstrBenefitOption;
                                astrDeathReasonvalue = busConstant.PostRetirementAlternatePayeeDeath;
                                icdoBenefitApplication.account_relationship_value = astrAccountrelationshipvalue;
                                icdoBenefitApplication.post_retirement_death_reason_type_value = busConstant.PostRetirementAlternatePayeeDeath;
                                icdoBenefitApplication.beneficiary_person_id = lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id;
                            }
                        }
                    }
                    else if ((lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value.Equals(busConstant.ApplicationBenefitTypeRetirement))
                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value.Equals(busConstant.ApplicationBenefitTypeDisability))
                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value.Equals(busConstant.ApplicationBenefitTypePreRetirementDeath))
                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value.Equals(busConstant.ApplicationBenefitTypePostRetirementDeath)))
                    {
                        //ACCOUNT OWNER DEATH
                        bool lblnAccountOwnerDeath = false;
                        if ((lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id == aintAccountOwnerId)
                            && ((lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value.Equals(busConstant.ApplicationBenefitTypeRetirement))
                            || (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value.Equals(busConstant.ApplicationBenefitTypeDisability))))
                        {
                            bool iblnIsTermCertainDatePastDateofDeath = IsTermCertainDatePastDateofDeath(busConstant.PostRetirementAccountOwnerDeath,
                            lobjPayeeAccount.icdoPayeeAccount.term_certain_end_date,
                            lobjPayeeAccount.ibusPayee.icdoPerson.date_of_death, lobjPayeeAccount.icdoPayeeAccount.benefit_option_value);

                            //load death benefit options
                            FetchDeathBenefitOption(ibusPlan.icdoPlan.benefit_provision_id,
                                                     lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value, astrIsPersonMarried,
                                                     lobjPayeeAccount.icdoPayeeAccount.benefit_option_value, busConstant.PostRetirementAccountOwnerDeath,
                                                     false, ref lstrBenefitOption,
                                                     ref astrAccountrelationshipvalue, ref lstrIsSpouseAtDeath, ref lstrIsMonthlyBenefitFlag, iblnIsTermCertainDatePastDateofDeath,
                                                     iobjPassInfo);

                            SetAccountOwnerDetails(aintAccountOwnerId, ref astrDeathReasonvalue, ref astrBenefitOption, astrAccountrelationshipvalue,
                                lstrBenefitOption, ref lblnAccountOwnerDeath);

                        }
                        if (!lblnAccountOwnerDeath)
                        {
                            //FISRT BEN DEATH
                            FetchDeathBenefitOption(ibusPlan.icdoPlan.benefit_provision_id,
                                               lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value, astrIsPersonMarried,
                                               lobjPayeeAccount.icdoPayeeAccount.benefit_option_value, busConstant.PostRetirementFirstBeneficiaryDeath,
                                               false, ref lstrBenefitOption,
                                               ref astrAccountrelationshipvalue, ref lstrIsSpouseAtDeath, ref lstrIsMonthlyBenefitFlag, false, iobjPassInfo);

                            SetFirstBeneficiaryDetails(ref astrDeathReasonvalue, ref astrBenefitOption, busConstant.AccountRelationshipBeneficiary, lstrBenefitOption);
                        }
                    }
                    adtTerminationdate = icdoBenefitApplication.termination_date;
                }
            }
        }

        private void SetAccountOwnerDetails(int aintAccountOwnerPersonId, ref string astrDeathReasonvalue,
                                            ref string astrBenefitOption, string astrAccountrelationshipvalue,
                                            string astrOldBenefitOption, ref bool ablnAccountOwnerDeath)
        {
            bool lblnRecordFound = false;
            if (astrAccountrelationshipvalue == busConstant.AccountRelationshipJointAnnuitant)
            {
                //check for active spouse
                lblnRecordFound = IsActiveSpouseExists();
            }
            else if (astrAccountrelationshipvalue == busConstant.AccountRelationshipBeneficiary)
            {
                lblnRecordFound = IsBeneficiary(aintAccountOwnerPersonId);
            }

            if (lblnRecordFound)
            {
                ablnAccountOwnerDeath = true;
                astrDeathReasonvalue = busConstant.PostRetirementAccountOwnerDeath;
                astrBenefitOption = astrOldBenefitOption;
                icdoBenefitApplication.account_relationship_value = astrAccountrelationshipvalue;
                icdoBenefitApplication.post_retirement_death_reason_type_value = busConstant.PostRetirementAccountOwnerDeath;
                // adtTerminationdate = icdoBenefitApplication.termination_date;
            }
            else
                ablnAccountOwnerDeath = false;
        }

        private void SetFirstBeneficiaryDetails(ref string astrDeathReasonvalue, ref string astrBenefitOption,
            string astrAccountrelationshipvalue, string astrOldBenefitOption)
        {
            bool lblnActiveSpouseOnly = false;
            if (astrAccountrelationshipvalue.IsNotNullOrEmpty())
            {
                if (astrAccountrelationshipvalue == busConstant.AccountRelationshipJointAnnuitant)
                {
                    lblnActiveSpouseOnly = true;
                }
                if (IsActiveSpouseOrBeneExists(lblnActiveSpouseOnly))
                {
                    astrDeathReasonvalue = busConstant.PostRetirementFirstBeneficiaryDeath;
                    astrBenefitOption = astrOldBenefitOption;
                    icdoBenefitApplication.account_relationship_value = astrAccountrelationshipvalue;
                    icdoBenefitApplication.post_retirement_death_reason_type_value = busConstant.PostRetirementFirstBeneficiaryDeath;
                    // adtTerminationdate = icdoBenefitApplication.termination_date;
                }
            }
        }
        public bool IsBeneficiary(int aintPersonId)
        {
            return IsBeneficiary(aintPersonId, busConstant.PostRetirementAccountOwnerDeath);
        }
        /// <summary>
        /// this is used for account owner death
        /// to check if the recepient is beneficiary to the account owner
        /// </summary>
        /// <returns></returns>
        /// 
        public bool IsBeneficiary(int aintPersonId, string astrPostRetirementDeathReason)
        {
            busPerson lobjTempPerson = new busPerson();
            lobjTempPerson.FindPerson(aintPersonId);

            List<busPersonBeneficiary> lclbActivePersonBeneficiaries = new List<busPersonBeneficiary>();

            if (astrPostRetirementDeathReason == busConstant.PostRetirementAccountOwnerDeath)
            {
                if (lobjTempPerson.iclbPersonBeneficiary.IsNull())
                    lobjTempPerson.LoadBeneficiary();
                lclbActivePersonBeneficiaries = lobjTempPerson.iclbPersonBeneficiary
                                                     .Where(lobjPerBen => busGlobalFunctions.CheckDateOverlapping(lobjTempPerson.icdoPerson.date_of_death,
                                                         lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                                                         lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date)).ToList();
            }
            else if (astrPostRetirementDeathReason == busConstant.PostRetirementAlternatePayeeDeath)
            {
                if (ibusOriginatingPayeeAccount == null)
                    LoadOriginatingPayeeAccount();

                if (lobjTempPerson.iclbPersonBeneficiary.IsNull())
                    lobjTempPerson.LoadApplicationBeneficiary();

                lclbActivePersonBeneficiaries = lobjTempPerson.iclbPersonBeneficiary
                                                     .Where(lobjPerBen => busGlobalFunctions.CheckDateOverlapping(lobjTempPerson.icdoPerson.date_of_death,
                                                         lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                                            lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date
                                                         ) && (lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id == ibusOriginatingPayeeAccount.icdoPayeeAccount.dro_application_id)
                                                         && (lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value==busConstant.BeneficiaryMemberTypePrimary)
                                                         ).ToList();
            }
            else if (astrPostRetirementDeathReason == busConstant.PostRetirementFirstBeneficiaryDeath)
            {
                if (lobjTempPerson.iclbPersonBeneficiary.IsNull())
                    lobjTempPerson.LoadApplicationBeneficiary();

                lclbActivePersonBeneficiaries = lobjTempPerson.iclbPersonBeneficiary
                                     .Where(lobjPerBen => busGlobalFunctions.CheckDateOverlapping(lobjTempPerson.icdoPerson.date_of_death,
                                         lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                            lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date
                                         ) && (lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.benefit_application_id == ibusOriginatingPayeeAccount.icdoPayeeAccount.application_id)
                                         && (lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id == 0)
                                          && (lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary)
                                         ).ToList();
            }


            foreach (busPersonBeneficiary lobjPersonBeneficiary in lclbActivePersonBeneficiaries)
            {
                if (lobjPersonBeneficiary.iclbPersonAccountBeneficiary.IsNull())
                    lobjPersonBeneficiary.LoadPersonAccountBeneficiary();

                foreach (busPersonAccountBeneficiary lobjPersonAccountBene in lobjPersonBeneficiary.iclbPersonAccountBeneficiary)
                {
                    if (lobjPersonAccountBene.ibusPersonAccount.IsNull())
                        lobjPersonAccountBene.LoadPersonAccount();
                    if (icdoBenefitApplication.plan_id == lobjPersonAccountBene.ibusPersonAccount.icdoPersonAccount.plan_id)
                    {
                        if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0)
                        {
                            busPerson lobjBenePerson = new busPerson();
                            if (lobjBenePerson.FindPerson(lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id))
                            {
                                if (lobjBenePerson.icdoPerson.date_of_death == DateTime.MinValue)
                                {
                                    if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id == icdoBenefitApplication.recipient_person_id)
                                    {
                                        icdoBenefitApplication.family_relationship_value = lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value;
                                        icdoBenefitApplication.termination_date = lobjTempPerson.icdoPerson.date_of_death;
                                        return true;
                                    }
                                }
                            }
                        }
                        else if (lobjPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id > 0)
                        {
                            if (lobjPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id == icdoBenefitApplication.payee_org_id)
                            {
                                icdoBenefitApplication.family_relationship_value = lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value;
                                icdoBenefitApplication.termination_date = lobjTempPerson.icdoPerson.date_of_death;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 1) to check if Active spouse exists
        /// 2) to check if recipient is first beneficiary to spouse
        /// </summary>
        /// <param name="ablnIsActiveSpouse">Is active spouse exists</param>
        /// <param name="ablnIsFirstBeneOrAlternatePayeeDeath">this parameter is used to set the beneficiary person id</param>
        /// <returns></returns>
        private bool IsActiveSpouseExists()
        {
            bool lblnBeneficiaryFound = false;

            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = ibusPerson.LoadActivePersonAccountByPlan(icdoBenefitApplication.plan_id);
            if (ibusPersonAccount.ibusPerson.IsNull())
                ibusPersonAccount.ibusPerson = ibusPerson;
            if (ibusPersonAccount.ibusPerson.iclbPersonBeneficiary.IsNull())
                ibusPersonAccount.ibusPerson.LoadBeneficiary();
            //load all beneficiaries with plan participation end date as on date of death or after date of death
            var lclbActivePersonBeneficiaries = ibusPersonAccount.ibusPerson.iclbPersonBeneficiary
                                                                           .Where(lobjPerBen => (lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date
                                                                            >= ibusPersonAccount.ibusPerson.icdoPerson.date_of_death)
                                                                             || (lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue));

            foreach (busPersonBeneficiary lobjPersonBeneficiary in lclbActivePersonBeneficiaries)
            {
                if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPerson.IsNull())
                    lobjPersonBeneficiary.ibusPersonAccountBeneficiary.LoadPersonAccount();
                if (icdoBenefitApplication.plan_id == lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id)
                {
                    if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0)
                    {
                        busPerson lobjBenePerson = new busPerson();
                        if (lobjBenePerson.FindPerson(lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id))
                        {
                            if ((lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                               && (lobjBenePerson.icdoPerson.date_of_death == DateTime.MinValue))
                            {
                                if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id == icdoBenefitApplication.recipient_person_id)
                                {
                                    lblnBeneficiaryFound = true;
                                }
                            }
                        }
                    }

                    if (lblnBeneficiaryFound)
                    {
                        icdoBenefitApplication.family_relationship_value = busConstant.FamilyRelationshipSpouse;
                        icdoBenefitApplication.termination_date = ibusPersonAccount.ibusPerson.icdoPerson.date_of_death;

                        icdoBenefitApplication.beneficiary_person_id = icdoBenefitApplication.recipient_person_id;
                        break;
                    }
                }
            }
            return lblnBeneficiaryFound;
        }

        /// <summary>
        /// 1) to check if recepient is the Active spouse of the beneficiary of the account owner       
        /// </summary>       
        /// <returns></returns>
        private bool IsActiveSpouseOrBeneExists(bool ablnCheckForSpouseonly)
        {
            bool lblnBeneficiaryFound = false;
            //load all beneficiaries with plan participation end date as on date of death or after date of death
            if (ibusOriginatingPayeeAccount == null)
                LoadOriginatingPayeeAccount();

            busPerson lobjTempPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjTempPerson.FindPerson(ibusOriginatingPayeeAccount.icdoPayeeAccount.payee_perslink_id);

            if (lobjTempPerson.iclbPersonBeneficiary.IsNull())
                lobjTempPerson.LoadApplicationBeneficiary();

            /*if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = ibusPerson.LoadActivePersonAccountByPlan(icdoBenefitApplication.plan_id);
            if (ibusPersonAccount.ibusPerson.IsNull())
                ibusPersonAccount.ibusPerson = ibusPerson;
            if (ibusPersonAccount.ibusPerson.iclbPersonBeneficiary.IsNull())
                ibusPersonAccount.ibusPerson.LoadApplicationBeneficiary();

            
            var lclbActivePersonBeneficiaries = ibusPersonAccount.ibusPerson.iclbPersonBeneficiary
                                                 .Where(lobjPerBen => (lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date
                                                                            >= ibusPersonAccount.ibusPerson.icdoPerson.date_of_death)
                                                                            || (lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date.Equals(DateTime.MinValue)));
             */

            var lclbActivePersonBeneficiaries = lobjTempPerson.iclbPersonBeneficiary
                                     .Where(lobjPerBen => busGlobalFunctions.CheckDateOverlapping(lobjTempPerson.icdoPerson.date_of_death,
                                         lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                            lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date
                                         ) && (lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.benefit_application_id == ibusOriginatingPayeeAccount.icdoPayeeAccount.application_id)
                                         && (lobjPerBen.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value==busConstant.BeneficiaryMemberTypePrimary)
                                         );

            foreach (busPersonBeneficiary lobjPersonBeneficiary in lclbActivePersonBeneficiaries)
            {
                if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0)
                {

                    if (lobjPersonBeneficiary.ibusBeneficiaryPerson.IsNull())
                        lobjPersonBeneficiary.LoadBeneficiaryPerson();

                    //This Person should not be dead.
                    if (lobjPersonBeneficiary.ibusBeneficiaryPerson.icdoPerson.date_of_death == DateTime.MinValue)
                    {
                        if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id == icdoBenefitApplication.recipient_person_id)
                        {
                            if ((ablnCheckForSpouseonly))
                            {
                                if (lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                                {
                                    lblnBeneficiaryFound = true;
                                }
                            }
                            else
                            {
                                lblnBeneficiaryFound = true;
                            }
                        }
                    }
                }
                else
                {
                    if (lobjPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id == icdoBenefitApplication.payee_org_id)
                    {
                        lblnBeneficiaryFound = true;
                    }
                }
                if (lblnBeneficiaryFound)
                {
                    icdoBenefitApplication.family_relationship_value = lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value;
                    icdoBenefitApplication.termination_date = lobjTempPerson.icdoPerson.date_of_death;
                    icdoBenefitApplication.beneficiary_person_id = lobjTempPerson.icdoPerson.person_id;
                    break;
                }

            }

            //        if (lobjBenePerson.icdoPerson.date_of_death != DateTime.MinValue)
            //        {
            //            if (lobjBenePerson.iclbPersonBeneficiary.IsNull())
            //                lobjBenePerson.LoadBeneficiary();

            //            if (ablnCheckForSpouseonly)
            //            {
            //                if (lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
            //                {

            //                    var lclbSouseBeneficiaries = lobjBenePerson.iclbPersonBeneficiary
            //                                                                     .Where(lobjPerBen => (lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date
            //                                                                       >= ibusPersonAccount.ibusPerson.icdoPerson.date_of_death)
            //                                                                       || (lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue)
            //                                                                       && ((lobjPerBen.icdoPersonBeneficiary.beneficiary_person_id == icdoBenefitApplication.recipient_person_id)
            //                                                                       || (lobjPerBen.icdoPersonBeneficiary.benificiary_org_id == icdoBenefitApplication.payee_org_id)));

            //                    if (lclbSouseBeneficiaries.Count() > 0)
            //                    {
            //                        foreach (var lobjBenePersonBeneficiary in lclbSouseBeneficiaries)
            //                        {
            //                            if (lobjBenePersonBeneficiary.ibusPersonAccountBeneficiary.ibusPerson.IsNull())
            //                                lobjBenePersonBeneficiary.ibusPersonAccountBeneficiary.LoadPersonAccount();

            //                            if (lobjBenePersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
            //                            {
            //                                if (lobjBenePersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == icdoBenefitApplication.plan_id)
            //                                {
            //                                    icdoBenefitApplication.family_relationship_value = lobjBenePersonBeneficiary.icdoPersonBeneficiary.relationship_value;
            //                                    icdoBenefitApplication.termination_date = lobjBenePerson.icdoPerson.date_of_death;
            //                                    lblnBeneficiaryFound = true;
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                        if (lblnBeneficiaryFound)
            //                        {
            //                            //this should only be stored in case of first bene death or alternate payee death
            //                            icdoBenefitApplication.beneficiary_person_id = lobjBenePerson.icdoPerson.person_id;
            //                            break;
            //                        }

            //                    }
            //                }
            //            }
            //            else
            //            {
            //                var lclbBeneficiaries = lobjBenePerson.iclbPersonBeneficiary
            //                                                                    .Where(lobjPerBen => (lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date
            //                                                                      >= ibusPersonAccount.ibusPerson.icdoPerson.date_of_death)
            //                                                                      || (lobjPerBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue)
            //                                                                      && (((lobjPerBen.icdoPersonBeneficiary.beneficiary_person_id == icdoBenefitApplication.recipient_person_id)
            //                                                                      && (lobjPerBen.icdoPersonBeneficiary.beneficiary_person_id > 0))
            //                                                                      || ((lobjPerBen.icdoPersonBeneficiary.benificiary_org_id == icdoBenefitApplication.payee_org_id)
            //                                                                      && (lobjPerBen.icdoPersonBeneficiary.benificiary_org_id > 0))));

            //                if (lclbBeneficiaries.Count() > 0)
            //                {
            //                    foreach (var lobjBenePersonBeneficiary in lclbBeneficiaries)
            //                    {
            //                        if (lobjBenePersonBeneficiary.ibusPersonAccountBeneficiary.ibusPerson.IsNull())
            //                            lobjBenePersonBeneficiary.ibusPersonAccountBeneficiary.LoadPersonAccount();

            //                        if (lobjBenePersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
            //                        {
            //                            if (lobjBenePersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == icdoBenefitApplication.plan_id)
            //                            {
            //                                icdoBenefitApplication.family_relationship_value = lobjBenePersonBeneficiary.icdoPersonBeneficiary.relationship_value;
            //                                icdoBenefitApplication.termination_date = lobjBenePerson.icdoPerson.date_of_death;
            //                                lblnBeneficiaryFound = true;
            //                            }
            //                        }
            //                    }
            //                    if (lblnBeneficiaryFound)
            //                    {
            //                        //this should only be stored in case of first bene death or alternate payee death
            //                        icdoBenefitApplication.beneficiary_person_id = lobjBenePerson.icdoPerson.person_id;
            //                        break;
            //                    }
            //                }
            //            }
            //        }


            //    }
            //}
            return lblnBeneficiaryFound;
        }

        # endregion

        // BR-054-11 && UAT PIR ID 1237
        public bool IsActiveApplicationExists()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.iclbBenefitApplication.IsNull()) ibusPerson.LoadBenefitApplication();
            foreach (busBenefitApplication lobjApplication in ibusPerson.iclbBenefitApplication)
            {
                if (lobjApplication.icdoBenefitApplication.benefit_account_type_value != busConstant.ApplicationBenefitTypePostRetirementDeath)
                {
                    if (((lobjApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusDeferred) ||
                        (lobjApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified) ||
                        (lobjApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusPending)) &&
                        (lobjApplication.icdoBenefitApplication.status_value != busConstant.ApplicationStatusProcessed))
                        return true;
                }
            }
            return false;
        }


        private bool IsTermCertainDatePastDateofDeath(string astrDeathReasonTypeValue, DateTime adtTermCertainDate, DateTime adtDateofDeath, string astrbenefitOptionName)
        {
            bool iblnIsTermCertainDatePastDateofDeath = false;

            if ((adtTermCertainDate != DateTime.MinValue) && (adtDateofDeath != DateTime.MinValue)
                && (astrDeathReasonTypeValue != busConstant.PostRetirementFirstBeneficiaryDeath))
            {
                if ((adtTermCertainDate < adtDateofDeath) & IsTermCertainOption(astrbenefitOptionName))
                {
                    iblnIsTermCertainDatePastDateofDeath = true;
                }
            }
            return iblnIsTermCertainDatePastDateofDeath;
        }

        private bool IsTermCertainOption(string astrbenefitOptionName)
        {
            if ((astrbenefitOptionName == "L05C") ||
               (astrbenefitOptionName == "L10C") ||
               (astrbenefitOptionName == "L20C") ||
               (astrbenefitOptionName == "LA10") ||
               (astrbenefitOptionName == "LA15") ||
               (astrbenefitOptionName == "LA20") ||
               (astrbenefitOptionName == "LB05") ||
               (astrbenefitOptionName == "LB10") ||
               (astrbenefitOptionName == "LB15") ||
               (astrbenefitOptionName == "LB20") ||
               (astrbenefitOptionName == "T10C") ||
               (astrbenefitOptionName == "T15C") ||
               (astrbenefitOptionName == "T20C") ||
               (astrbenefitOptionName == "5YTL"))
                return true;
            return false;

        }

    }
}
