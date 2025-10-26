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
using NeoSpin.DataObjects;
using NeoSpin.Common;
using Sagitec.Bpm;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitApplication : busBenefitApplicationGen
    {
        public bool IsEmploymentEndDateDateCurrent = false;
        public int iintMembersAgeInMonthsAsOnRetirementDate;
        public int iintMembersAgeInMonthsAsOnGivenDate;
        private string istrPreviousActionStatus;
        private ObjectState istrObjectState;
        public bool iblnIsRTWMember { get; set; }
        public DateTime idtTerminationDate { get; set; }
        private bool _IsDeniedButtonClicked;
        public bool IsDeniedButtonClicked
        {
            get { return _IsDeniedButtonClicked; }
            set { _IsDeniedButtonClicked = value; }
        }

        //PIR - 1051       
        public string istrMemberAge
        {
            get
            {
                string lstrMemberAge = String.Empty;
                lstrMemberAge = string.Format("{0:00}", icdoBenefitApplication.iintMemberAgeYearPart) + " years " + string.Format("{0:00}", icdoBenefitApplication.iintMemberAgeMonthPart + " months");

                return lstrMemberAge;
            }
        }
		// PIR 17082
        private Collection<busBenefitCalculation> _iclbBenefitCalculation;
        public Collection<busBenefitCalculation> iclbBenefitCalculation
        {
            get { return _iclbBenefitCalculation; }
            set { _iclbBenefitCalculation = value; }
        }
		
        public void LoadBenefitCalculation()
        {
            DataTable ldtbResult = Select<cdoBenefitCalculation>(
                                      new string[1] { "benefit_application_id" },
                                      new object[1] { icdoBenefitApplication.benefit_application_id }, null, null);
            _iclbBenefitCalculation = GetCollection<busBenefitCalculation>(ldtbResult, "icdoBenefitCalculation");
        }
		
        public bool IsBenefitOptionRegularRefundOrAutoRefund()
        {
            if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionRegularRefund)
            {
                return true;
            }
            else if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionAutoRefund)
            {
                return true;
            }
            return false;
        }
        //elayaraja
        public bool IsBenefitOptionTransfers()
        {
            if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToDCTransfer)
            {
                return true;
            }
            if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection)    //PIR 25920 DC 2025 changes part 2
            {
                return true;
            }
            else if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer)
            {
                return true;
            }
            else if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE)
            {
                return true;
            }
            else if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDualMembers)
            {
                return true;
            }
            return false;
        }
        private decimal _idecMemberAgeBasedOnDateOfDeath;
        public decimal idecMemberAgeBasedOnDateOfDeath
        {
            get { return _idecMemberAgeBasedOnDateOfDeath; }
            set { _idecMemberAgeBasedOnDateOfDeath = value; }
        }

        private int _iiintMemberAgeBasedOnDateOfDeath;
        public int iiintMemberAgeBasedOnDateOfDeath
        {
            get { return _iiintMemberAgeBasedOnDateOfDeath; }
            set { _iiintMemberAgeBasedOnDateOfDeath = value; }
        }

        public string istrBenApplId
        {
            get
            {
                string lstrBenApplId = String.Empty;
                if (icdoBenefitApplication.benefit_application_id != 0)
                    lstrBenApplId = (icdoBenefitApplication.benefit_application_id).ToString();
                return lstrBenApplId;
            }
        }
        public bool IsJSorNormalBenefitOptionSelected()
        {
            if (!String.IsNullOrEmpty(icdoBenefitApplication.benefit_option_value))
            {
                if ((IsJSBenefitOption) || (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit))
                {
                    // UAT PIR ID 1382
                    if ((icdoBenefitApplication.plan_id == busConstant.PlanIdHP) || (icdoBenefitApplication.plan_id == busConstant.PlanIdJudges))
                    {
                        if (ibusPerson.IsNull()) LoadPerson();
                        if (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                            return true;
                    }
                    else
                        return true;
                }
            }
            return false;
        }

        public bool IsJSBenefitOption
        {
            get
            {
                if (icdoBenefitApplication.benefit_option_value.IsNotNullOrEmpty())
                {
                    if ((icdoBenefitApplication.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value50JointSurvivor) ||
                        (icdoBenefitApplication.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value55JointSurvivor) ||
                        (icdoBenefitApplication.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value75JointSurvivor) ||
                        (icdoBenefitApplication.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value100JointSurvivor))
                        return true;
                }
                return false;
            }
        }

        //returns bool value after checking if the RHIC optional value is reduced
        public bool IsReducedRHICSelected()
        {
            if (icdoBenefitApplication.rhic_option_value.IsNotNullOrEmpty())
            {
                if ((icdoBenefitApplication.rhic_option_value == busConstant.ApplicationRHICReduced50) ||
                    (icdoBenefitApplication.rhic_option_value == busConstant.ApplicationRHICReduced100))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsApplicationCancelledOrDenied()
        {
            if (icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusDenied)
            {
                return true;
            }
            else if (icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusCancelled)
            {
                return true;
            }
            return false;
        }
        private Collection<busBenefitDroApplication> _iclbBenefitDROApplication;

        public Collection<busBenefitDroApplication> iclbBenefitDROApplication
        {
            get { return _iclbBenefitDROApplication; }
            set { _iclbBenefitDROApplication = value; }
        }
        public void LoadDROApplication()
        {
            DataTable ldtbList = Select<cdoBenefitDroApplication>(
                                    new string[2] { "member_perslink_id", "plan_id" },
                                    new object[2] { icdoBenefitApplication.member_person_id, icdoBenefitApplication.plan_id }, null, null);
            _iclbBenefitDROApplication = GetCollection<busBenefitDroApplication>(ldtbList, "icdoBenefitDroApplication");
        }
        public bool IsBenefitDROApplicationExist()
        {
            if (_iclbBenefitDROApplication == null)
                LoadDROApplication();
            foreach (busBenefitDroApplication lobjBenefitDROAppilcation in _iclbBenefitDROApplication)
            {
                if (!lobjBenefitDROAppilcation.IsDROApplicationCancelledOrDenied())
                {
                    return true;
                }
            }
            return false;
        }
        // Added for Loading the Beneficiary person for UCS-54 Post Retirement Application.
        public busPerson ibusBeneficiaryPerson { get; set; }

        public void LoadBeneficiaryPerson()
        {
            if (ibusBeneficiaryPerson.IsNull())
                ibusBeneficiaryPerson = new busPerson { icdoPerson = new cdoPerson() };
            if (icdoBenefitApplication.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath)
            {
                ibusBeneficiaryPerson.FindPerson(icdoBenefitApplication.member_person_id);
            }
            else
            {
                ibusBeneficiaryPerson.FindPerson(icdoBenefitApplication.beneficiary_person_id);
            }
        }

        public int iintAccountPayeeId
        {
            get
            {
                if (icdoBenefitApplication.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath)
                    return icdoBenefitApplication.member_person_id;
                else
                    return icdoBenefitApplication.beneficiary_person_id;
            }
        }

        public DateTime idtAccountPayeeDateofDeath { get; set; }

        //Get Total VSC for that person 
        //bool property is passed in order to check if the plan is job service or not.
        //returened value is then rounded to decimal 4.
        public decimal GetRoundedTVSC()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            bool lblnIsPlanJobService = false;
            if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
                lblnIsPlanJobService = true;

            DateTime ldtDateToCompare = DateTime.MinValue;
            if ((icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                || (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
            {
                ldtDateToCompare = icdoBenefitApplication.retirement_date;
            }
            else if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                ldtDateToCompare = ibusPersonAccount.ibusPerson.icdoPerson.date_of_death;
            }
            // added date as parameter done by Deepa
            // in order to get only the contribution those falls below this date
            //below changes are to done for UCS-060
            //when member is RTW and election value is not set
            //take in to account for all person account for person is and plan id
            //else exclude this person account
           //Prod PIR: 4154
            //The Service Credit That has been posted after the Retirement date also needs to be Considered.

            decimal ldecReturnTVSC = ibusPersonAccount.ibusPerson.GetTotalVSCForPerson(lblnIsPlanJobService, ldtDateToCompare,false,false);

            if (ibusPersonAccount.IsNotNull())
            {
                ldecReturnTVSC = ldecReturnTVSC + ibusPersonAccount.GetTotalServicePurchaseCreditPostedAfterRetirementDate(false, icdoBenefitApplication.retirement_date);
            }

            ldecReturnTVSC = Math.Round(ldecReturnTVSC, 4, MidpointRounding.AwayFromZero);
            return ldecReturnTVSC;
            //if ((icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            //    || (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
            //{
            //    if (iblnIsRTWMember)
            //    {
            //        if (icdoBenefitApplication.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper())
            //        {
            //            if (ibusPersonAccount.icdoPersonAccount.Total_VSC.Equals(0.0M))
            //                ibusPersonAccount.LoadTotalVSC();
            //            return ldecReturnTVSC - ibusPersonAccount.icdoPersonAccount.Total_VSC;
            //        }
            //    }
            //    return ldecReturnTVSC;
            //}
            //else
            //{
            //    return ldecReturnTVSC;
            //}
        }

        //Set Member's age based on the retirement date entered by member
        public void SetMemberAgeBasedOnRetirementDate()
        {
            if (icdoBenefitApplication.retirement_date != DateTime.MinValue)
            {
                int lintMembersAgeInMonthsAsOnRetirementDate = 0;
                decimal ldecMemberAgeBasedOnRetirementDate = 0.00M;
                int lintMemberAgeMonthPart = 0;
                int lintMemberAgeYearPart = 0;

                DateTime ldtTempDate = DateTime.MinValue;
                //icdoBenefitApplication.idtNormalRetirementDate;
                if (icdoBenefitApplication.retirement_date != DateTime.MinValue)
                    ldtTempDate = icdoBenefitApplication.retirement_date;

                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.ibusPerson == null)
                    ibusPersonAccount.LoadPerson();

                DateTime ldtFrom = ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth;
                DateTime ldtTo = ldtTempDate.AddMonths(-1);


                CalculateAge(ldtFrom, ldtTo, ref lintMembersAgeInMonthsAsOnRetirementDate, ref ldecMemberAgeBasedOnRetirementDate, 4,
                                ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);

                icdoBenefitApplication.iintMemberAgeMonthPart = lintMemberAgeMonthPart;
                icdoBenefitApplication.iintMemberAgeYearPart = lintMemberAgeYearPart;

                iintMembersAgeInMonthsAsOnGivenDate = lintMembersAgeInMonthsAsOnRetirementDate;
                idecMemberAgeBasedOnGivenDate = ldecMemberAgeBasedOnRetirementDate;
                if (idecMemberAgeBasedOnGivenDate < 0.00M)
                    idecMemberAgeBasedOnGivenDate = 0.00M;
                iintMembersAgeInMonthsAsOnRetirementDate = lintMembersAgeInMonthsAsOnRetirementDate;
                idecMemberAgeBasedOnRetirementDate = ldecMemberAgeBasedOnRetirementDate;
                if (idecMemberAgeBasedOnRetirementDate < 0.00M)
                    idecMemberAgeBasedOnRetirementDate = 0.00M;
            }
        }

        //Set Age as on Date of Death
        public void SetAgeBasedOnDateOfDeath()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPerson == null)
                LoadPerson();

            DateTime ldtMemberDateofDeath = ibusPerson.icdoPerson.date_of_death;
            DateTime ldtMemberDateofBirth = ibusPerson.icdoPerson.date_of_birth;
            //This is also used in Screen constants. So made it common.
            if (ibusBeneficiaryPerson == null)
                LoadBeneficiaryPerson();

            if ((icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath) &&
                (icdoBenefitApplication.post_retirement_death_reason_type_value != busConstant.PostRetirementAccountOwnerDeath))
            {
                ldtMemberDateofDeath = ibusBeneficiaryPerson.icdoPerson.date_of_death;
                ldtMemberDateofBirth = ibusBeneficiaryPerson.icdoPerson.date_of_birth;
            }
            idtAccountPayeeDateofDeath = ldtMemberDateofDeath;
            if (ldtMemberDateofDeath != DateTime.MinValue)
            {
                int lintMonths = 0;
                decimal ldecMemberAgeInMonths = 0.00M;
                int lintMemberAgeMonthPart = 0;
                int lintMemberAgeYearPart = 0;
                CalculateAge(ldtMemberDateofBirth, ldtMemberDateofDeath,
                    ref lintMonths, ref ldecMemberAgeInMonths, 4, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);

                icdoBenefitApplication.iintMemberAgeMonthPart = lintMemberAgeMonthPart;
                icdoBenefitApplication.iintMemberAgeYearPart = lintMemberAgeYearPart;

                idecMemberAgeBasedOnGivenDate = ldecMemberAgeInMonths;
                iintMembersAgeInMonthsAsOnGivenDate = lintMonths;
                idecMemberAgeBasedOnDateOfDeath = ldecMemberAgeInMonths;
                iiintMemberAgeBasedOnDateOfDeath = lintMonths;
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();
            if (ibusPersonEmploymentDtl == null)
                ibusPersonEmploymentDtl = ibusPersonAccount.GetLatestEmploymentDetail();

            if ((icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                || (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement))
                SetMemberAgeBasedOnRetirementDate();

            if (icdoBenefitApplication.ihstOldValues.Count > 0)
            {
                istrPreviousActionStatus = icdoBenefitApplication.ihstOldValues["action_status_value"].ToString();
            }
            //this is used to insert the person account record in the benefit application person account table.
            istrObjectState = icdoBenefitApplication.ienuObjectState;
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            if (this.iarrChangeLog.Count > 0)
            {
                //check if the previous Status is verified
                // and if the changes are made, then update the status as pending verfication
                if (istrPreviousActionStatus == busConstant.ApplicationActionStatusVerified)
                {
                    icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
                    //PIR 1498
                    ChangeStatusOfPayeeCalculationIfAppStatusIsPendingVerified();
                }
            }
            PopulateUserNameForSuppressWarning();


            base.BeforePersistChanges();
        }

        public override int PersistChanges()
        {
            base.PersistChanges();
            //if RTW payee account is not null
            if (iblnIsRTWMember)
            {
                DeleteAndInsertBenefitApplicationPersonAccounts();
                CreateBenefitApplicationPersonAccount();
            }
            else
            {
                //in new mode store the person account id in the benefit application person account table
                if (istrObjectState == ObjectState.Insert)
                {
                    CreateBenefitApplicationPersonAccount();
                }
            }
            return 1;
        }
        # region UCS-060


        public void IsRTWMember(out bool ablnIsRTWMember, out int aintPayeeAccountID)
        {
            ablnIsRTWMember = false;
            aintPayeeAccountID = 0;

            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                ablnIsRTWMember = ibusPersonAccount.ibusPerson.IsRTWMember(icdoBenefitApplication.plan_id, busConstant.PayeeStatusForRTW.IgnoreStatus, ref aintPayeeAccountID);
            else
                ablnIsRTWMember = ibusPersonAccount.ibusPerson.IsRTWMember(icdoBenefitApplication.plan_id, busConstant.PayeeStatusForRTW.SuspendedOnly, ref aintPayeeAccountID);
        }

        public void LoadPensionPlanAccounts()
        {
            iclbRTWPersonAccounts = new Collection<busBenefitApplicationPersonAccount>();
            if (iclbBenefitApplicationPersonAccounts.IsNull())
                LoadBenefitApplicationPersonAccount();
            var lclbBAPersonAccounts = iclbBenefitApplicationPersonAccounts
                                        .Where(lobjBAPA => lobjBAPA.icdoBenefitApplicationPersonAccount.is_application_person_account_flag != busConstant.Flag_Yes);
            foreach (busBenefitApplicationPersonAccount lobjBAPersonAccount in lclbBAPersonAccounts)
            {
                if (lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.payee_account_id > 0)
                    lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.istrUse = busConstant.Flag_Yes_Value;
                if (lobjBAPersonAccount.ibusPersonAccount.IsNull())
                    lobjBAPersonAccount.LoadPersonAccount();
                if (lobjBAPersonAccount.ibusPersonAccount.ibusPlan.IsNull())
                    lobjBAPersonAccount.ibusPersonAccount.LoadPlan();
                iclbRTWPersonAccounts.Add(lobjBAPersonAccount);
            }
        }

        //Set ISRTW less than 2 years
        //calculate the consolidated PSC
        public void SetIsRTWFlagLessThan2Years(decimal adecTotalPSC)
        {
            if (adecTotalPSC > 0)
            {
                if ((adecTotalPSC / 12.0M) > 2)
                {
                    icdoBenefitApplication.is_rtw_less_than_2years_flag = busConstant.Flag_No;
                    icdoBenefitApplication.rtw_refund_election_value = busConstant.Flag_No_Value.ToUpper();
                    icdoBenefitApplication.rtw_refund_election_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2325, icdoBenefitApplication.rtw_refund_election_value);
                }
                else
                { icdoBenefitApplication.is_rtw_less_than_2years_flag = busConstant.Flag_Yes; }
            }
            else
            {
                icdoBenefitApplication.is_rtw_less_than_2years_flag = busConstant.Flag_Yes;
            }
        }

        //UCS -060
        //only for RTW member
        //if the person having person account more than one 
        //insert all benefit application person account
        public void DeleteAndInsertBenefitApplicationPersonAccounts()
        {
            // this collection must reload before delete
            //delete if already there is any record benefit application Person account for this application 
            // if (iclbBenefitApplicationPersonAccounts.IsNull())
            LoadBenefitApplicationPersonAccount();

            foreach (busBenefitApplicationPersonAccount lobjBAPersonAccount in iclbBenefitApplicationPersonAccounts)
            {
                lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.Delete();
            }

            //insert the new collection 
            if (iclbRTWPersonAccounts.IsNull())
                LoadRTWPersonAccount();

            foreach (busBenefitApplicationPersonAccount lobjBAPersonAccount in iclbRTWPersonAccounts)
            {
                lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.is_application_person_account_flag = busConstant.Flag_No;
                lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.benefit_application_id = icdoBenefitApplication.benefit_application_id;
                lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.is_person_account_selected_flag = busConstant.Flag_Yes;
                lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.Insert();
                lobjBAPersonAccount.UpdateDataObject(lobjBAPersonAccount.icdoBenefitApplicationPersonAccount);
            }
        }

        //UCS - 60
        //this is for other buttons functionality
        public void UpdateBenefitApplicationPersonAccount()
        {
            foreach (busBenefitApplicationPersonAccount lobjBAPersonAccount in iclbRTWPersonAccounts)
            {
                if (lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.is_person_account_selected_flag == busConstant.Flag_Yes)
                    lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.Update();
            }
        }

        public void LoadRTWPersonAccount()
        {
            iclbRTWPersonAccounts = new Collection<busBenefitApplicationPersonAccount>();

            if (ibusPerson.IsNull())
                LoadPerson();

            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = ibusPerson.LoadActivePersonAccountByPlan(icdoBenefitApplication.plan_id);

            if (ibusPersonAccount.ibusPerson.IsNull())
                ibusPersonAccount.ibusPerson = ibusPerson;
            if (ibusPersonAccount.ibusPlan.IsNull())
                ibusPersonAccount.LoadPlan();
            if (ibusPersonAccount.ibusPerson.icolPersonAccount.IsNull())
                ibusPersonAccount.ibusPerson.LoadPersonAccount();
            bool lblnIsRTWMember;
            int lintPayeeAccountID;
            int lintPersonAccountId = 0;
            int lintCountIndex = 1;
            IsRTWMember(out lblnIsRTWMember, out lintPayeeAccountID);

            if ((lintPayeeAccountID > 0)
                && (lblnIsRTWMember))
            {
                //set payee account and bool in cdoBenefitApplication
                iblnIsRTWMember = lblnIsRTWMember;
                icdoBenefitApplication.pre_rtw_payeeaccount_id = lintPayeeAccountID;

                //PIR 18053 - Populate benefit option and rhic option from prev payee account
                LoadPreRTWPayeeAccount();
                busRetirementDisabilityApplication lobjRetirementApplication = new busRetirementDisabilityApplication();
                if (lobjRetirementApplication.FindBenefitApplication(ibusPreRTWPayeeAccount.icdoPayeeAccount.application_id))
                {
                    icdoBenefitApplication.benefit_option_value = lobjRetirementApplication.icdoBenefitApplication.benefit_option_value;
                    icdoBenefitApplication.rhic_option_value = lobjRetirementApplication.icdoBenefitApplication.rhic_option_value;
                }

                lintPersonAccountId = ibusPersonAccount.icdoPersonAccount.person_account_id;

                ibusPersonAccount.LoadTotalPSC(icdoBenefitApplication.retirement_date);
                SetIsRTWFlagLessThan2Years(ibusPersonAccount.icdoPersonAccount.Total_PSC);

                var lclbPersonAccount = ibusPersonAccount.ibusPerson.icolPersonAccount.Where(lobjPA => lobjPA.icdoPersonAccount.plan_id == icdoBenefitApplication.plan_id)
                   .OrderByDescending(lobj => lobj.icdoPersonAccount.start_date);
                foreach (busPersonAccount lobjPersonAccount in lclbPersonAccount)
                {
                    if ((!(lobjPersonAccount.icdoPersonAccount.person_account_id.CompareTo(lintPersonAccountId) == 0))
                    && (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value
                                .Equals(busConstant.PlanParticipationStatusRetirementEnrolled) ||
                                lobjPersonAccount.icdoPersonAccount.plan_participation_status_value.Equals(busConstant.PlanParticipationStatusRetirementRetired) ||
                                lobjPersonAccount.icdoPersonAccount.plan_participation_status_value.Equals(busConstant.PlanParticipationStatusRetimentSuspended)))
                    {
                        if (lobjPersonAccount.iclbPayeeAccounts.IsNull())
                            lobjPersonAccount.LoadPayeeAccounts();

                        foreach (busPayeeAccount lobjPayeeAccount in lobjPersonAccount.iclbPayeeAccounts)
                        {
                            if ((lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                                || (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                            {
                                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsNull())
                                    lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();

                                if ((lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusSuspended()) ||
                                    ((icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath) &&
                                    (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted())))
                                {
                                    busBenefitApplicationPersonAccount lobjBAPersonAccount = new busBenefitApplicationPersonAccount
                                    {
                                        icdoBenefitApplicationPersonAccount = new cdoBenefitApplicationPersonAccount(),
                                    };

                                    lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.benefit_application_id = icdoBenefitApplication.benefit_application_id;
                                    lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.person_account_id = lobjPersonAccount.icdoPersonAccount.person_account_id;

                                    if (lobjBAPersonAccount.ibusPersonAccount.IsNull())
                                        lobjBAPersonAccount.LoadPersonAccount();
                                    if (lobjBAPersonAccount.ibusPersonAccount.ibusPlan.IsNull())
                                        lobjBAPersonAccount.ibusPersonAccount.LoadPlan();

                                    lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.is_person_account_selected_flag = busConstant.Flag_No;

                                    if (icdoBenefitApplication.is_rtw_less_than_2years_flag == busConstant.Flag_No)
                                    {
                                        lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.is_person_account_selected_flag = busConstant.Flag_Yes;
                                    }
                                    lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.istrUse = busConstant.Flag_Yes_Value;
                                    lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.is_application_person_account_flag = busConstant.Flag_No;
                                    lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.payee_account_id = lobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                                    lobjBAPersonAccount.icdoBenefitApplicationPersonAccount.benefit_application_person_account_id = lintCountIndex;
                                    iclbRTWPersonAccounts.Add(lobjBAPersonAccount);
                                    lintCountIndex -= lintCountIndex;
                                }
                            }
                        }
                    }
                }
            }
        }

        # endregion
        public void CreateBenefitApplicationPersonAccount()
        {
            cdoBenefitApplicationPersonAccount lobjBenApplPersonAccount = new cdoBenefitApplicationPersonAccount();
            lobjBenApplPersonAccount.person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
            lobjBenApplPersonAccount.benefit_application_id = icdoBenefitApplication.benefit_application_id;
            lobjBenApplPersonAccount.is_application_person_account_flag = busConstant.Flag_Yes;
            lobjBenApplPersonAccount.is_person_account_selected_flag = busConstant.Flag_Yes;
            lobjBenApplPersonAccount.Insert();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            ChangePayeeAccountStatusToReview();
            //CancelRMDProcessInstances();
        }

        //BR- 93- 02
        //private void CancelRMDProcessInstances()
        //{
        //    //PIR 2121
        //    bool lblnIsEligibleForCancellingWorkflow = false;
        //    int lintProcessInstanceId = 0;
        //    if (ibusBaseActivityInstance != null)
        //    {
        //        busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
        //        lintProcessInstanceId = lbusActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id;
        //        if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) != busConstant.Map_Process_RMD)
        //        {
        //            //if (lbusActivityInstance.ibusActivity.icdoActivity.activity_id == 0)
        //            //{
        //            //    lbusActivityInstance.LoadActivity();
        //            //}

        //            //if ((lbusActivityInstance.ibusActivity.icdoActivity.new_mode_screen_name == "wfmRetirementApplicationLookup") ||
        //            //       (lbusActivityInstance.ibusActivity.icdoActivity.update_mode_screen_name == "wfmRetirementApplicationMaintenance") ||
        //            //       (lbusActivityInstance.ibusActivity.icdoActivity.new_mode_screen_name == "wfmRefundApplicationMaintenance") ||
        //            //       (lbusActivityInstance.ibusActivity.icdoActivity.update_mode_screen_name == "wfmRefundApplicationMaintenance"))

        //            //PIR - 1403 UAT
        //            //The system must cancel an initiated or suspended ‘Process RMD’ workflow process 
        //            //when a ‘Retirement’, ‘Disability’ or ‘Refund’ application for the same Member / Plan combination is entered.
        //            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability
        //                || icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund
        //                || icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
        //            {
        //                lblnIsEligibleForCancellingWorkflow = true;
        //            }
        //        }
        //    }
        //    if (ibusPersonAccount.icdoPersonAccount.rmd_batch_initiated_flag != busConstant.Flag_Yes)
        //    {
        //        lblnIsEligibleForCancellingWorkflow = true;
        //    }
        //    if (lblnIsEligibleForCancellingWorkflow)
        //    {
        //        //venkat check query
        //        DataTable ldtActivityInstance = Select("entSolBpmActivityInstance.LoadRunningRMDProcessInstancesByPersonAndPlan",
        //            new object[4] { busConstant.Map_Process_RMD, icdoBenefitApplication.member_person_id, icdoBenefitApplication.plan_id, lintProcessInstanceId });
        //        Collection<busBpmActivityInstance> lclbActivityInstance = GetCollection<busBpmActivityInstance>(ldtActivityInstance, "icdoBpmActivityInstance");
        //        foreach (busBpmActivityInstance lbusTempActivityInstance in lclbActivityInstance)
        //        {
        //            busWorkflowHelper.UpdateWorkflowActivityByEvent(lbusTempActivityInstance, enmNextAction.Cancel, busConstant.ActivityStatusCancelled, iobjPassInfo);
        //        }
        //    }
        //    //    }
        //    //}
        //}

        public void ChangePayeeAccountStatusToReview()
        {
            //PIR 984 Change the payee account status to review if Application status is review
            if (icdoBenefitApplication.status_value == busConstant.StatusReview)
            {
                if (iclbPayeeAccount == null)
                {
                    LoadPayeeAccount();
                }
                foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
                {
                    if (lobjPayeeAccount.ibusSoftErrors == null)
                        lobjPayeeAccount.LoadErrors();
                    lobjPayeeAccount.iblnClearSoftErrors = false;
                    lobjPayeeAccount.ibusSoftErrors.iblnClearError = false;
                    lobjPayeeAccount.CreateReviewPayeeAccountStatus();
                    lobjPayeeAccount.iblnApplicationStatusIndicator = true;
                    lobjPayeeAccount.ValidateSoftErrors();
                    lobjPayeeAccount.UpdateValidateStatus();
                }

                ////UCS - 54
                ////change payee account to review
                ////this is for account owner Payee
                //if (ibusPersonAccount.ibusPerson.IsNull())
                //    ibusPersonAccount.LoadPerson();
                //if (ibusPersonAccount.ibusPerson.iclbPayeeAccount.IsNull())
                //    ibusPersonAccount.ibusPerson.LoadPayeeAccount();
                //foreach (busPayeeAccount lobjPayeeAccount in ibusPersonAccount.ibusPerson.iclbPayeeAccount)
                //{
                //    lobjPayeeAccount.CreateReviewPayeeAccountStatus();
                //    lobjPayeeAccount.iblnApplicationStatusIndicator = true;
                //    lobjPayeeAccount.ValidateSoftErrors();
                //    lobjPayeeAccount.UpdateValidateStatus();
                //}

            }
        }

        //PIR - 1012 systest
        //set org id based on the member's latest employment 
        //set after checking employment status as contributing and end date as null
        //Method modified by -Elayaraja- changed for benefit type Refund
        public void SetOrgIdAsLatestEmploymentOrgId()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            DateTime ldtTerminationDate = DateTime.MinValue;
            icdoBenefitApplication.retirement_org_id = GetOrgIdAsLatestEmploymentOrgId(ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                                                icdoBenefitApplication.benefit_account_type_value, ref ldtTerminationDate);
            icdoBenefitApplication.istrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(icdoBenefitApplication.retirement_org_id);
            if (idtTerminationDate == DateTime.MinValue)
            {
                idtTerminationDate = ldtTerminationDate;
            }            
        }

        //check the person is having tentative TFFR or TIAA service
        //Load TFFRTIAA Service for the person and loop thru
        // check if any service exists with tentative status return true.
        public bool CheckIFTentativeTFFROrTIAAExistsForPerson()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            if (ibusPersonAccount.ibusPerson.iclbTffrTiaaService == null)
                ibusPersonAccount.ibusPerson.LoadTffrTiaaService();
            foreach (busPersonTffrTiaaService lobjPersonTffrTiaaService in ibusPersonAccount.ibusPerson.iclbTffrTiaaService)
            {
                if (lobjPersonTffrTiaaService.icdoPersonTffrTiaaService.tffr_service_status_value == busConstant.PersonTFFRTIAAServiceStatusTentative || lobjPersonTffrTiaaService.icdoPersonTffrTiaaService.tiaa_service_status_value == busConstant.PersonTFFRTIAAServiceStatusTentative)
                {
                    return true;
                }
            }
            return false;
        }

        //BR - 51 - 49
        //throw warning message when service exists with status Approved, In Payment Or Pending
        //Load Service Purchase for that Person and filter as plan id
        //check if any record exists with the Approved, Pending Or In Payment Status.
        public bool CheckPersonHavingServiceWithStatusApprvdORInPaymentOrPend()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            if (ibusPersonAccount.ibusPerson.iclbServicePurchaseHeader == null)
                ibusPersonAccount.ibusPerson.LoadServicePurchase();

            foreach (busServicePurchaseHeader lobjServicePurchase in ibusPersonAccount.ibusPerson.iclbServicePurchaseHeader)
            {
                if (lobjServicePurchase.icdoServicePurchaseHeader.plan_id == icdoBenefitApplication.plan_id)
                {
                    if ((lobjServicePurchase.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment) ||
                        ((lobjServicePurchase.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Approved
                        || lobjServicePurchase.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Pending)
                        && DateTime.Today < lobjServicePurchase.icdoServicePurchaseHeader.expiration_date))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //BR - 51 - 51
        // check if the unused sick leave checkbox is checked
        // and there is no service exists with service type Unsused Sick Leave with status paid in full
        public bool CheckPaidInFullUnusedSickLeaveNotExists()
        {

            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            if (ibusPersonAccount.ibusPerson.iclbServicePurchaseHeader == null)
                ibusPersonAccount.ibusPerson.LoadServicePurchase();
            foreach (busServicePurchaseHeader lobjServicePurchase in ibusPersonAccount.ibusPerson.iclbServicePurchaseHeader)
            {
                if (lobjServicePurchase.icdoServicePurchaseHeader.plan_id == icdoBenefitApplication.plan_id)
                {
                    //check date of purchase exists between the employment end date  and 60 days prior to that

                    //Get Employment end date
                    DateTime ldtEmploymentEndDate = icdoBenefitApplication.termination_date;
                    DateTime ldt60DaysPriorEmploymentEndDate = ldtEmploymentEndDate.AddDays(-60);

                    if (busGlobalFunctions.CheckDateOverlapping(lobjServicePurchase.icdoServicePurchaseHeader.date_of_purchase, ldt60DaysPriorEmploymentEndDate, ldtEmploymentEndDate))
                    {
                        if ((lobjServicePurchase.icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave)
                            && (lobjServicePurchase.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Paid_In_Full))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //BR-51-52
        //get the latest Employment detail
        //set month and year apart
        //loop thru the collection and check if the contribution exists for the last month of employment
        //with subsytem value Payroll  Or Conversion Or Purchase
        public bool IsLastContributionPostedForMember()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            DateTime ldtDateToCompare = DateTime.MinValue;
            DateTime ldtLatestEmploymentEndedDate = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date;
            int lintLatestEmploymentMonth = ldtLatestEmploymentEndedDate.Month;
            int lintLatestEmploymentYear = ldtLatestEmploymentEndedDate.Year;

            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                ldtDateToCompare = icdoBenefitApplication.termination_date; //PROD PIR 5675
            }
            else
            {
                ldtDateToCompare = icdoBenefitApplication.retirement_date;
            }
            ibusPersonAccount.LoadRetirementContributionAll();
            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in ibusPersonAccount.iclbRetirementContributionAll)
            {
                if ((lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month == lintLatestEmploymentMonth)
                    &&
                    (lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year == lintLatestEmploymentYear)
                    && ((lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting)
                    || (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueConversion)
                    || (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueServicePurchase)))
                {
                    if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date <= ldtDateToCompare.GetLastDayofMonth()) //PROD PIR 5675
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //check application already exists for person and plan
        public virtual bool CheckBenefitApplicationIsValid()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbBenefitApplication == null)
                ibusPersonAccount.ibusPerson.LoadBenefitApplication();
            return true;
        }

        //override delete in order to delete errors associated with the application
        public override int Delete()
        {
            if (ValidateDelete())
            {
                //delete all errors
                DBFunction.DBNonQuery("cdoBenefitApplication.DeleteErrors", new object[1] { icdoBenefitApplication.benefit_application_id },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //delete all benefit application person account
                DBFunction.DBNonQuery("cdoBenefitApplication.DeletePersonAccount", new object[1] { icdoBenefitApplication.benefit_application_id },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);


                if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                {
                    //delete all other disability benefits
                    DBFunction.DBNonQuery("cdoBenefitApplication.DeleteOtherDisabilityBenefit", new object[1] { icdoBenefitApplication.benefit_application_id },
                                          iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
                return base.Delete();
            }
            return 0;
        }

        private void PopulateUserNameForSuppressWarning()
        {
            // set the name of the user who has suppressed the warning.
            // first we have to check whether the object is in update mode or insert mode, based on that
            // we have to either 
            if (icdoBenefitApplication.ienuObjectState == ObjectState.Insert)
            {
                if (icdoBenefitApplication.suppress_warnings_flag != null && icdoBenefitApplication.suppress_warnings_flag == busConstant.Flag_Yes)
                {
                    icdoBenefitApplication.suppress_warnings_by = iobjPassInfo.istrUserID;
                }
            }
            else if (icdoBenefitApplication.ienuObjectState == ObjectState.Update)
            {
                //string lstrOldSuppressWarningsFlag =
                //    icdoBenefitApplication.ihstOldValues["suppress_warnings_flag"].ToString();
                //if (lstrOldSuppressWarningsFlag != icdoBenefitApplication.suppress_warnings_flag)
                //{
                if (icdoBenefitApplication.suppress_warnings_flag == busConstant.Flag_Yes)
                {
                    icdoBenefitApplication.suppress_warnings_by = iobjPassInfo.istrUserID;
                }
                else
                {
                    icdoBenefitApplication.suppress_warnings_by = "";
                }
                // }
            }
        }

        //PIR 26088 Refund Application and calculation should have a red suppressable error warning for deal member
        public bool DualMemberSuppressibleWarning()
        {
            if (icdoBenefitApplication.suppress_warnings_flag.IsNullOrEmpty() || icdoBenefitApplication.suppress_warnings_flag == busConstant.Flag_No)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();

                ibusPerson.LoadRetirementAccount();
                if ((ibusPerson.iclbRetirementAccount.IsNotNull() && ibusPerson.iclbRetirementAccount.Count > 0) && 
                    ibusPerson.iclbRetirementAccount.Where(i => i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                                                            i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended).Count() > 1)
                {
                    return true;
                }

                DataTable ldtResult = Select("cdoBenefitRefundApplication.LoadDualMemberSuppressWarning", new object[1] { icdoBenefitApplication.member_person_id });
                if (ldtResult.Rows.Count > 0)
                {
                    return true;
                }
                
            }

            return false;
        }

        public virtual ArrayList btnVerfiyClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {

                //UCS - 079 -- Start -- Updating recovery repayment type and gross reduction amount
                if (!UpdateRepaymentTypeAndGrossReductionAmount())
                {
                    lobjError = new utlError();
                    lobjError = AddError(6714, string.Empty);
                    alReturn.Add(lobjError);
                }
                //UCS - 079 -- End region
                else
                {
				//PIR-16812
                    if (AreAnyOutstandingDetailsExist())
                    {
                        lobjError = new utlError();
                        lobjError = AddError(0, "Member has outstanding Payroll Details.");
                        alReturn.Add(lobjError);
                    }
                    else
                    {
                        icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusVerified;
                        icdoBenefitApplication.Update();
                        //validate errors
                        base.ValidateSoftErrors();
                        LoadErrors();
                        base.UpdateValidateStatus();
                        if (icdoBenefitApplication.status_value == busConstant.StatusReview)
                        {
                            ChangePayeeAccountStatusToReview();
                        }
                        icdoBenefitApplication.Select();
                        alReturn.Add(this);

                        //Load initial rules
                        this.EvaluateInitialLoadRules();
                    }
                }
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }

        // PIR 17082
        public ArrayList CheckCancelClickIsValid() 
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
            {
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsNull())
                    lobjPayeeAccount.LoadActivePayeeStatus();
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 != busConstant.PayeeAccountStatusCancelled)
                {
                    lobjError = AddError(10300, "");
                    alReturn.Add(lobjError);
                    break;
                }
            }
            if (_iclbBenefitCalculation == null)
                LoadBenefitCalculation();
            if (_iclbBenefitCalculation.Any(i => i.icdoBenefitCalculation.action_status_value != busConstant.BenefitActionStatusCancelled))
            {
                lobjError = AddError(10301, "");
                alReturn.Add(lobjError);
            }
            return alReturn;
        }

        public virtual ArrayList btnCancelClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            // PIR 17082
            iarrErrors = CheckCancelClickIsValid();
            if (iarrErrors.Count == 0)
            {
                icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusCancelled;
                icdoBenefitApplication.Update();
                ChangePayeeAccountStatusToReview();
                //validate errors
                base.ValidateSoftErrors();
                LoadErrors();
                base.UpdateValidateStatus();
                if (icdoBenefitApplication.status_value == busConstant.StatusReview)
                {
                    ChangePayeeAccountStatusToReview();
                }
                //refresh cdo
                icdoBenefitApplication.Select();
                alReturn.Add(this);

                //Load intinial rules
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }
        public virtual ArrayList btnDenyClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusDenied;
                icdoBenefitApplication.Update();

                //validate errors
                base.ValidateSoftErrors();
                LoadErrors();
                base.UpdateValidateStatus();

                if (icdoBenefitApplication.status_value == busConstant.StatusReview)
                {
                    ChangePayeeAccountStatusToReview();
                }
                //refresh cdo
                icdoBenefitApplication.Select();
                alReturn.Add(this);

                //Load intinial rules
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }

        public virtual ArrayList btnPendingVerificationClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                //PIR 1498
                if (icdoBenefitApplication.ihstOldValues.Count > 0)
                {
                    if (icdoBenefitApplication.ihstOldValues["action_status_value"].ToString() == busConstant.ApplicationActionStatusVerified)
                    {
                        ChangeStatusOfPayeeCalculationIfAppStatusIsPendingVerified();
                    }
                }
                //*************************************************************************************
                icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
                icdoBenefitApplication.Update();
                ChangePayeeAccountStatusToReview();
                //validate errors
                base.ValidateSoftErrors();
                LoadErrors();
                base.UpdateValidateStatus();
                if (icdoBenefitApplication.status_value == busConstant.StatusReview)
                {
                    ChangePayeeAccountStatusToReview();
                }
                //refresh cdo
                icdoBenefitApplication.Select();
                alReturn.Add(this);

                //Load intinial rules
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }

        public void ChangeStatusOfPayeeCalculationIfAppStatusIsPendingVerified()
        {
            //PIR 1498
            //UCS integration 51/53/55/56/57
            //if the application status is changed from verified to Pending 
            // 1. then change the status of Calculation to Pending Approval
            // 2. Change the status of the Payee account to review
            //check if calculation exists else dont update
            busBenefitCalculation lobjBenefitCalculation = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            if (lobjBenefitCalculation.FindBenefitCalculationByApplication(icdoBenefitApplication.benefit_application_id))
            {
                if (lobjBenefitCalculation.icdoBenefitCalculation.action_status_value != busConstant.CalculationStatusPendingApproval)
                    lobjBenefitCalculation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusPendingApproval;
                lobjBenefitCalculation.icdoBenefitCalculation.Update();
            }
            //load Payee Account
            //load Payee account status
            //create review payee account status
            if (iclbPayeeAccount.IsNull())
                LoadPayeeAccount();
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
            {
                if (lobjPayeeAccount.ibusSoftErrors == null)
                    lobjPayeeAccount.LoadErrors();
                lobjPayeeAccount.iblnClearSoftErrors = false;
                lobjPayeeAccount.ibusSoftErrors.iblnClearError = false;
                lobjPayeeAccount.CreateReviewPayeeAccountStatus();
                lobjPayeeAccount.iblnApplicationStatusIndicator = true;
                lobjPayeeAccount.ValidateSoftErrors();
                lobjPayeeAccount.UpdateValidateStatus();
            }
        }


        public bool CheckIsPersonVested()
        {
            icdoBenefitApplication.istrIsPersonVested = busConstant.Flag_No;
            if (icdoBenefitApplication.plan_id == busConstant.PlanIdDC ||
                icdoBenefitApplication.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                icdoBenefitApplication.plan_id == busConstant.PlanIdDC2025) return true; //PIR 25920
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();
            DateTime ldtDateToCompare = DateTime.MinValue;


            if ((icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                || (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
            {
                ldtDateToCompare = icdoBenefitApplication.retirement_date;
            }
            else if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.ibusPerson == null)
                    ibusPersonAccount.LoadPerson();
                ldtDateToCompare = ibusPersonAccount.ibusPerson.icdoPerson.date_of_death;
            }
            if (icdoBenefitApplication.idecTVSC == 0.00M)
            {
                icdoBenefitApplication.idecTVSC = GetRoundedTVSC();
                //this to be removed if of no use
                //jus repetition of code
                //bool lblnIsPlanJobService = false;
                //if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
                //    lblnIsPlanJobService = true;
                //icdoBenefitApplication.idecTVSC = ibusPersonAccount.ibusPerson.GetTotalVSCForPerson(lblnIsPlanJobService, ldtDateToCompare);
            }
            bool lblnIsPersonEligibleForVesting = CheckIsPersonVested(icdoBenefitApplication.plan_id, ibusPersonAccount.ibusPlan.icdoPlan.plan_code,
                                                    ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id, icdoBenefitApplication.benefit_account_type_value,
                                                    icdoBenefitApplication.idecTVSC, idecMemberAgeBasedOnGivenDate, ldtDateToCompare,
                                                    false, icdoBenefitApplication.termination_date, ibusPersonAccount, iobjPassInfo); //PIR 14646 - Vesting logic changes
            if (lblnIsPersonEligibleForVesting)
            { icdoBenefitApplication.istrIsPersonVested = busConstant.Flag_Yes; }       

            return lblnIsPersonEligibleForVesting;
        }

        //This method is used in Auto refund Batch for Benefit Type defaulted to REtirement
        //since for Refund, everyone is eligible and we dont have entries for Refund in Benefit Provision Eligibility ref table
        public bool IsPersonVestedForRefund()
        {
            if (icdoBenefitApplication.plan_id == busConstant.PlanIdDC
                || icdoBenefitApplication.plan_id == busConstant.PlanIdDC2020 || icdoBenefitApplication.plan_id == busConstant.PlanIdDC2025) return true; // PIR 20232 //PIR 25920
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();

            int lintMonths = 0;
            decimal ldecMemberAgeInMonths = 0.00M;
            int lintMemberAgeMonthPart = 0;
            int lintMemberAgeYearPart = 0;

            icdoBenefitApplication.idecTVSC = GetRoundedTVSC();

            CalculateAge(ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth, DateTime.Now,
                ref lintMonths, ref ldecMemberAgeInMonths, 2, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);

            //prod pir 5644
            //should check vesting based on RFND benefit account type, so defaulting the benefit account type to RETR
            //--start--//
            //bool lblnIsPersonEligibleForVesting = CheckIsPersonVested(icdoBenefitApplication.plan_id, ibusPersonAccount.ibusPlan.icdoPlan.plan_code,
            //                                        ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id, icdoBenefitApplication.benefit_account_type_value,
            //                                        icdoBenefitApplication.idecTVSC, ldecMemberAgeInMonths, DateTime.Now, false, DateTime.MinValue, ibusPersonAccount, iobjPassInfo);
            bool lblnIsPersonEligibleForVesting = CheckIsPersonVested(icdoBenefitApplication.plan_id, ibusPersonAccount.ibusPlan.icdoPlan.plan_code,
                                                    ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id, busConstant.ApplicationBenefitTypeRetirement,
                                                    icdoBenefitApplication.idecTVSC, ldecMemberAgeInMonths, DateTime.Now, false, DateTime.Now.AddMonths(-1).GetLastDayofMonth(), ibusPersonAccount, iobjPassInfo);
            //--end--//
            return lblnIsPersonEligibleForVesting;
        }

        public bool CheckIsPersonEligibleforNormal()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();
            DateTime ldtDateToCompare = DateTime.MinValue;
            if ((icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
               || (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
            {
                ldtDateToCompare = icdoBenefitApplication.retirement_date;
            }
            else if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.ibusPerson == null)
                    ibusPersonAccount.LoadPerson();
                ldtDateToCompare = ibusPersonAccount.ibusPerson.icdoPerson.date_of_death;
            }

            //Prod PIR: 4370. Extra VSC entered should be accounted inorder to find NRD Date.
            // PIR 10537
            decimal ldecTFFRService = 0.00M;
            decimal ldecTIAAService = 0.00M;
            decimal ldecTentativeTFFRService = 0.00M;
            decimal ldecTentativeTIAAService = 0.00M;
            ibusPersonAccount.LoadTFFRTIAAService(ref ldecTFFRService, ref ldecTIAAService, ref ldecTentativeTFFRService, ref ldecTentativeTIAAService);
            decimal ldecExtraServiceCredit = ldecTFFRService + ldecTIAAService;

            bool IsPersonEligibleForNormal = CheckIsPersonEligibleforNormal(icdoBenefitApplication.plan_id, ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id,
                                            icdoBenefitApplication.benefit_account_type_value, idecMemberAgeBasedOnGivenDate, iintMembersAgeInMonthsAsOnGivenDate
                                            , icdoBenefitApplication.idecTVSC, ldtDateToCompare, ibusPersonAccount, iobjPassInfo, false, icdoBenefitApplication.termination_date == DateTime.MinValue ? idtTerminationDate : icdoBenefitApplication.termination_date, ldecExtraServiceCredit); //PIR 15316
            return IsPersonEligibleForNormal;
        }

        public bool CheckISPersonEligibleForEarly()
        {
            string lstrEarlyReducedwaivedFlag = String.Empty;
            DateTime ldtDateToCompare = DateTime.MinValue;

            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();
            if ((icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
               || (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
            {
                ldtDateToCompare = icdoBenefitApplication.retirement_date;
            }

            bool lblnIsPersonEligibleForEarly = CheckISPersonEligibleForEarly(icdoBenefitApplication.plan_id, ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id,
                                        icdoBenefitApplication.benefit_account_type_value, idecMemberAgeBasedOnGivenDate, icdoBenefitApplication.idecTVSC, ref lstrEarlyReducedwaivedFlag,
                                        ldtDateToCompare, ibusPersonAccount, iobjPassInfo,false);
            icdoBenefitApplication.early_reduction_waived_flag = lstrEarlyReducedwaivedFlag;
            return lblnIsPersonEligibleForEarly;
        }

        // Set benefit sub type based on the member's eligibility
        public void SetBenefitSubType()
        {
            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                SetMemberAgeBasedOnRetirementDate();
                if (CheckIsPersonVested())
                    icdoBenefitApplication.istrIsPersonVested = busConstant.Flag_Yes;
                //this is set to null in order to check old value of benefit sub type is not retained for validation
                icdoBenefitApplication.benefit_sub_type_value = null;
                if (CheckIsPersonEligibleforNormal())
                {
                    DateTime ldtDNRORetirementDate = icdoBenefitApplication.idtNormalRetirementDate;
                    DateTime ldtDNROComapredate = ldtDNRORetirementDate;
                    if ((icdoBenefitApplication.termination_date != DateTime.MinValue) && (icdoBenefitApplication.plan_id != busConstant.PlanIdJobService))
                    {
                        DateTime ldtChangedEmpEnddate = icdoBenefitApplication.termination_date.AddMonths(1);
                        ldtDNROComapredate = DateTime.Compare(ldtDNRORetirementDate, ldtChangedEmpEnddate) > 0 ? ldtDNRORetirementDate : ldtChangedEmpEnddate;
                    }
                    // Maximum date of dtNormalRetirementDate and dtChangedEmpEnddate                    

                    int lintDNROMonthsToIncrease = busGlobalFunctions.DateDiffByMonth(ldtDNROComapredate, icdoBenefitApplication.retirement_date);
                    if (lintDNROMonthsToIncrease > 0)
                    {
                        lintDNROMonthsToIncrease = lintDNROMonthsToIncrease - 1;
                    }

                    if (lintDNROMonthsToIncrease > 0)
                    {
                        icdoBenefitApplication.benefit_sub_type_value = busConstant.ApplicationBenefitSubTypeDNRO;
                    }
                    else
                    {
                        icdoBenefitApplication.benefit_sub_type_value = busConstant.ApplicationBenefitSubTypeNormal;
                    }

                    // PIR 10329 - set benefit subtype to DNRO if DNRO flag is true
                    if (icdoBenefitApplication.dnro_flag == busConstant.Flag_Yes)
                    {
                        icdoBenefitApplication.benefit_sub_type_value = busConstant.ApplicationBenefitSubTypeDNRO;
                    }
                }
                else if (CheckISPersonEligibleForEarly())
                    icdoBenefitApplication.benefit_sub_type_value = busConstant.ApplicationBenefitSubTypeEarly;
            }
            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                SetAgeBasedOnDateOfDeath();
                idecMemberAgeBasedOnGivenDate = idecMemberAgeBasedOnDateOfDeath;
                iintMembersAgeInMonthsAsOnGivenDate = iiintMemberAgeBasedOnDateOfDeath;

                if (CheckIsPersonVested())
                    icdoBenefitApplication.istrIsPersonVested = busConstant.Flag_Yes;
                //this is set to null in order to check old value of benefit sub type is not retained for validation
                icdoBenefitApplication.benefit_sub_type_value = null;
                if (CheckIsPersonEligibleforNormal())
                    icdoBenefitApplication.benefit_sub_type_value = busConstant.ApplicationBenefitSubTypeNormal;
            }
        }

        //this method contains three sub methods to check the eligibility of the person for the following
        //For DB plan check this  
        // 1. Vesting
        // 2. Normal retirement
        // 3. Early retirement
        //For DC plan check the eligibility based on the no of employment days
        public virtual bool CheckIsPersonEligible()
        {
            if (icdoBenefitApplication.benefit_sub_type_value == null)
                SetBenefitSubType();
            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (icdoBenefitApplication.istrIsPersonVested == busConstant.Flag_Yes)
                {
                    if ((icdoBenefitApplication.benefit_sub_type_value != null) &&
                        ((icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal) ||
                          (icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO) ||   // SYS PIR ID 2305
                          (icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)))
                        return true;
                }
            }
            return false;
        }

        public bool IsAllEmploymentEndDated()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.iclbEmploymentDetail == null)
                ibusPersonAccount.LoadAllPersonEmploymentDetails(false);

            var lenumCountOfEndDatedRecordsRecords = ibusPersonAccount.iclbEmploymentDetail
                                                    .Where(lobjPE => lobjPE.icdoPersonEmploymentDetail.end_date == DateTime.MinValue);

            var lenumRecordsOtherThanContributing = lenumCountOfEndDatedRecordsRecords.AsEnumerable()
                                                    .Where(lobjPE => lobjPE.icdoPersonEmploymentDetail.status_value != busConstant.EmploymentStatusNonContributing);

            if (lenumRecordsOtherThanContributing.Count() > 0)
            {
                return false;
            }
            return true;

        }

        public override busBase GetCorPerson()
        {
            if (icdoBenefitApplication.benefit_account_type_value != busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.ibusPerson == null)
                    ibusPersonAccount.LoadPerson();
                return ibusPersonAccount.ibusPerson;
            }
            else
            {
                if (ibusRecipient == null)
                    LoadRecipient();
                return ibusRecipient;
            }
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
			//PIR 14391
            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath ||
                icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
            {
                LoadPersonBeneAddress();
            }
        }

        //PIR - UAT 877
        public busPersonBeneficiary ibusPersonBeneficiary { get; set; }

        public string istrBeneFullName { get; set; }
        public string istrBeneSalutaion { get; set; }
        public string istrBeneAdrCorStreet1 { get; set; }
        public string istrBeneAdrCorStreet2 { get; set; }
        public string istrBeneAdrCorCity { get; set; }
        public string istrBeneAdrCorState { get; set; }
        public string istrBeneAdrCorZip { get; set; }
        public int iintBenePerslinkId { get; set; }

        public void LoadPersonBeneAddress()
        {
            if (ibusRecipient.IsNull())
                LoadRecipient();
            if (ibusPersonAccount.ibusPerson.IsNull())
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbPersonBeneficiary.IsNull())
                ibusPersonAccount.ibusPerson.LoadBeneficiary();

            if (icdoBenefitApplication.recipient_person_id > 0)
            {
                var lenumPersonBene = ibusPersonAccount.ibusPerson.iclbPersonBeneficiary
                                .Where(lobjPerBene => lobjPerBene.icdoPersonBeneficiary.person_id == icdoBenefitApplication.member_person_id
                                && (lobjPerBene.icdoPersonBeneficiary.beneficiary_person_id == icdoBenefitApplication.recipient_person_id));
                if (lenumPersonBene.Count() > 0)
                {
                    ibusPersonBeneficiary = lenumPersonBene.First();
                    ibusPersonBeneficiary.LoadPersonAccountBeneficiary();
					
					//PIR 14391
                    if (ibusPersonBeneficiary.ibusBeneficiaryPerson.IsNull()) ibusPersonBeneficiary.LoadBeneficiaryPerson();
                    if (ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.IsNull())
                        ibusPersonBeneficiary.ibusBeneficiaryPerson.LoadPersonCurrentAddress();

                    iintBenePerslinkId = icdoBenefitApplication.recipient_person_id;
                    istrBeneFullName = ibusRecipient.icdoPerson.FullName.ToUpper();
                    istrBeneSalutaion = busGlobalFunctions.ToTitleCase(ibusRecipient.icdoPerson.FullName);
                    if (ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id > 0)
                    {
                        istrBeneAdrCorStreet1 = ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1;
                        istrBeneAdrCorStreet2 = ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2;
                        istrBeneAdrCorCity = ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city;
                        istrBeneAdrCorState = ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value;
                        istrBeneAdrCorZip = ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code;
                        if (ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code != string.Empty)
                            istrBeneAdrCorZip = istrBeneAdrCorZip + "-" + ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code;
                    }
                    else if (ibusPersonBeneficiary.icdoPersonBeneficiary.same_as_member_address == busConstant.Flag_Yes)
                    {
                        //get deceased current address
                        ibusPersonAccount.ibusPerson
                            .LoadPersonCurrentAddress();
                        //ibusPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date);

                        istrBeneAdrCorStreet1 = ibusPersonAccount.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1;
                        istrBeneAdrCorStreet2 = ibusPersonAccount.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2;
                        istrBeneAdrCorCity = ibusPersonAccount.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city;
                        istrBeneAdrCorState = ibusPersonAccount.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value;
                        istrBeneAdrCorZip = ibusPersonAccount.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code;
                        if (ibusPersonAccount.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code != string.Empty)
                            istrBeneAdrCorZip = istrBeneAdrCorZip +"-"+ ibusPersonAccount.ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code;
                    }
                    else
                    {
                        istrBeneAdrCorStreet1 = ibusPersonBeneficiary.icdoPersonBeneficiary.address_line_1;
                        istrBeneAdrCorStreet2 = ibusPersonBeneficiary.icdoPersonBeneficiary.address_line_2;
                        istrBeneAdrCorCity = ibusPersonBeneficiary.icdoPersonBeneficiary.address_city;
                        istrBeneAdrCorState = ibusPersonBeneficiary.icdoPersonBeneficiary.address_state_value;
                        istrBeneAdrCorZip = ibusPersonBeneficiary.icdoPersonBeneficiary.address_zip_code;
                        if (ibusPersonBeneficiary.icdoPersonBeneficiary.address_zip_4_code != string.Empty)
                            istrBeneAdrCorZip = istrBeneAdrCorZip + "-" + ibusPersonBeneficiary.icdoPersonBeneficiary.address_zip_4_code;

                    }
                }
            }
            if (icdoBenefitApplication.payee_org_id > 0)
            {
                var lenumOrgBene = ibusPersonAccount.ibusPerson.iclbPersonBeneficiary
                                .Where(lobjOrgBene => lobjOrgBene.icdoPersonBeneficiary.benificiary_org_id == icdoBenefitApplication.payee_org_id);
                if(lenumOrgBene.Count() > 0)
                {
                    ibusPersonBeneficiary  = lenumOrgBene.First();
                    ibusPersonBeneficiary.LoadPersonAccountBeneficiary();
                    if (ibusPersonBeneficiary.ibusBeneficiaryOrganization.IsNull()) ibusPersonBeneficiary.LoadBeneficiaryOrganization();
                    if (ibusPersonBeneficiary.ibusBeneficiaryOrganization.ibusOrgPrimaryAddress.IsNull())
                        ibusPersonBeneficiary.ibusBeneficiaryOrganization.LoadOrgPrimaryAddress();
                    string istrPayeeOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(icdoBenefitApplication.payee_org_id);
                    iintBenePerslinkId = Convert.ToInt32(istrPayeeOrgCode);
                    istrBeneFullName = ibusPersonBeneficiary.ibusBeneficiaryOrganization.icdoOrganization.org_name;
                    istrBeneSalutaion = busGlobalFunctions.ToTitleCase(ibusPersonBeneficiary.ibusBeneficiaryOrganization.icdoOrganization.org_name);
                    istrBeneAdrCorStreet1 = ibusPersonBeneficiary.ibusBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_1;
                    istrBeneAdrCorStreet2 = ibusPersonBeneficiary.ibusBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2;
                    istrBeneAdrCorCity = ibusPersonBeneficiary.ibusBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.city;
                    istrBeneAdrCorState = ibusPersonBeneficiary.ibusBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value;
                    istrBeneAdrCorZip = ibusPersonBeneficiary.ibusBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code;
                    if (ibusPersonBeneficiary.ibusBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code != string.Empty)
                        istrBeneAdrCorZip = istrBeneAdrCorZip + "-" + ibusPersonBeneficiary.ibusBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code;
                }
            }
        }

        //load the person account from database
        //where the is application flag set as "Yes"
        public void LoadPersonAccountDetailsInUpdateMode()
        {
            ibusPersonAccount = new busPersonAccount
            {
                icdoPersonAccount = new cdoPersonAccount(),
            };
            if (iclbBenefitApplicationPersonAccounts.IsNull())
                LoadBenefitApplicationPersonAccount();
            if (iclbBenefitApplicationPersonAccounts.Count > 0)
            {
                busBenefitApplicationPersonAccount lobjBAPersonAccount = new busBenefitApplicationPersonAccount();
                lobjBAPersonAccount = iclbBenefitApplicationPersonAccounts.Where(lobj => lobj.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).FirstOrDefault();
                if (lobjBAPersonAccount.IsNotNull())
                {
                    lobjBAPersonAccount.LoadPersonAccount();
                    ibusPersonAccount = lobjBAPersonAccount.ibusPersonAccount;
                    if (ibusPersonAccount.ibusPerson.IsNull())
                        ibusPersonAccount.LoadPerson();
                    if (ibusPersonAccount.ibusPlan.IsNull())
                        ibusPersonAccount.LoadPlan();
                }
            }
        }

        // Load the corresponding Person Account for the Application
        public void LoadPersonAccountByApplication()
        {
            if (iclbBenefitApplicationPersonAccounts.IsNull())
                LoadBenefitApplicationPersonAccount();

            busBenefitApplicationPersonAccount lobjBenAppPersonAccount = iclbBenefitApplicationPersonAccounts.Where(
                                o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).FirstOrDefault();

            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

            if (lobjBenAppPersonAccount.IsNotNull())
                ibusPersonAccount.FindPersonAccount(lobjBenAppPersonAccount.icdoBenefitApplicationPersonAccount.person_account_id);
        }

        #region UCS - 079

        //Property to contain all recoveries for Pre RTW payee account id
        public Collection<busPaymentRecovery> iclbRecovery { get; set; }
        /// <summary>
        /// Method to load all Recoveries
        /// </summary>
        public void LoadPaymentRecovery()
        {
            DataTable ldtRecovery = Select<cdoPaymentRecovery>
                (new string[1] { enmPaymentRecovery.payee_account_id.ToString() },
                new object[1] { icdoBenefitApplication.pre_rtw_payeeaccount_id }, null, null);
            iclbRecovery = new Collection<busPaymentRecovery>();
            iclbRecovery = GetCollection<busPaymentRecovery>(ldtRecovery, "icdoPaymentRecovery");
        }

        /// <summary>
        /// Method to update Recovery repayment type and gross reduction amount
        /// </summary>
        /// <returns>boolean value</returns>
        public bool UpdateRepaymentTypeAndGrossReductionAmount()
        {
            bool lblnResult = true;
            decimal ldecMemberAge = 0.0M, ldecJointAnnuitantAge = 0.0M;
            DateTime ldtEffectiveDate;
            busPayeeAccount lobjPA = new busPayeeAccount();
            if (icdoBenefitApplication.pre_rtw_payeeaccount_id > 0)
            {
                if (iclbRecovery == null)
                    LoadPaymentRecovery();
                if (ibusPerson == null)
                    LoadPerson();
                if (ibusJointAnniutantPerson == null)
                    LoadJointAnniutantPerson();
                int lintMonths, lintMemberAgeYear, lintMemberAgeMonths;
                //loading next benefit payment date
                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                lobjPayeeAccount.FindPayeeAccount(icdoBenefitApplication.pre_rtw_payeeaccount_id);
                lobjPayeeAccount.LoadNexBenefitPaymentDate();

                foreach (busPaymentRecovery lobjRecovery in iclbRecovery)
                {
                    if ((lobjRecovery.icdoPaymentRecovery.status_value != busConstant.RecoveryStatusCancel &&
                        lobjRecovery.icdoPaymentRecovery.status_value != busConstant.RecoveryStatusSatisfied &&
                        lobjRecovery.icdoPaymentRecovery.status_value != busConstant.RecoveryStatusWriteOff) ||
                        (lobjRecovery.icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReduction &&
                        lobjRecovery.icdoPaymentRecovery.status_value == busConstant.RecoveryStatusSatisfied))
                    {
                        //uat pir 1565
                        if (icdoBenefitApplication.retirement_date > lobjPayeeAccount.idtNextBenefitPaymentDate)
                        {
                            lobjRecovery.icdoPaymentRecovery.effective_date = icdoBenefitApplication.retirement_date;
                        }
                        else
                        {
                            lobjRecovery.icdoPaymentRecovery.effective_date = lobjPayeeAccount.idtNextBenefitPaymentDate;
                        }
                        ldtEffectiveDate = lobjRecovery.icdoPaymentRecovery.effective_date;

                        if (ibusPerson.icdoPerson.person_id > 0)
                        {
                            lintMonths = lintMemberAgeMonths = lintMemberAgeYear = 0;
                            busPersonBase.CalculateAge(ibusPerson.icdoPerson.date_of_birth.AddMonths(1),
                                                           ldtEffectiveDate,
                                                            ref lintMonths, ref ldecMemberAge, 6, ref lintMemberAgeYear, ref lintMemberAgeMonths);
                        }
                        //loading joint annuitant age
                        if (ibusJointAnniutantPerson.icdoPerson.person_id > 0 && ibusJointAnniutantPerson.icdoPerson.person_id != ibusPerson.icdoPerson.person_id)
                        {
                            lintMonths = lintMemberAgeMonths = lintMemberAgeYear = 0;
                            busPersonBase.CalculateAge(ibusJointAnniutantPerson.icdoPerson.date_of_birth.AddMonths(1),
                                                            ldtEffectiveDate,
                                                            ref lintMonths, ref ldecJointAnnuitantAge, 6, ref lintMemberAgeYear, ref lintMemberAgeMonths);
                        }
                        if (lobjRecovery.icdoPaymentRecovery.repayment_type_value == busConstant.RecoveryRePaymentTypeLifeTimeReduction)
                        {
                            lobjRecovery.icdoPaymentRecovery.repayment_type_value = busConstant.RecoveryRePaymentTypeLifeTimeReductionForRTW;

                            busPaymentLifeTimeReductionRef lobjLTR = new busPaymentLifeTimeReductionRef();
                            int lintNoofPayments = lobjLTR.GetNumberofPaymentsForRecovery(icdoBenefitApplication.benefit_option_value,
                                                        ibusPerson.icdoPerson.gender_value,
                                                        ldecMemberAge,
                                                        ldecJointAnnuitantAge, icdoBenefitApplication.benefit_account_type_value); //PIR 17909
                            if (lintNoofPayments <= 0)
                            {
                                lblnResult = false;
                                break;
                            }
                            else
                            {
                                lobjRecovery.icdoPaymentRecovery.gross_reduction_amount = lobjRecovery.icdoPaymentRecovery.recovery_amount / Convert.ToDecimal(lintNoofPayments);
                                lobjRecovery.icdoPaymentRecovery.status_value = busConstant.RecoveryStatusApproved;
                                lblnResult = true;
                                lobjRecovery.icdoPaymentRecovery.Update();
                            }
                        }
                    }
                }
            }
            return lblnResult;
        }
        #endregion

        #region PIR  - 1863

        public string istrBenefitciaryRelationshipType { get; set; }
        public string istrBenefitciaryType { get; set; }
        public string istrBenefitciaryOrgCode { get; set; }
        public string istrBenefitciaryOrgName { get; set; }
        public string istrPlanName { get; set; }

        public string istrBeneficiaryName { get; set; }
        public string istrBeneficiaryToName { get; set; }
        public string istrBenefitOption { get; set; }
        public decimal idecBenefitciaryPercentage { get; set; }
        public DateTime idtStartDate { get; set; }
        public DateTime idtEndDate { get; set; }
        public int iintBeneficiaryid { get; set; }
        public int iintMemberPerslinkId { get; set; }

        # endregion

        //PIR- 2154
        //for RTW member check if there extis any over payment for which there is no recovery exists
        public bool IsPendingOverpaymentExists()
        {
            if (iblnIsRTWMember)
            {
                if (ibusPreRTWPayeeAccount.IsNull())
                    LoadPreRTWPayeeAccount();
                if (ibusPreRTWPayeeAccount.iclbPaymentBenefitOverpaymentHeader.IsNull())
                    ibusPreRTWPayeeAccount.LoadBenefitOverPaymentHeader();

                foreach (busPaymentBenefitOverpaymentHeader lobjOverPayment in ibusPreRTWPayeeAccount.iclbPaymentBenefitOverpaymentHeader)
                {
                    if (lobjOverPayment.iclbPaymentRecovery.IsNull())
                        lobjOverPayment.LoadPaymentRecoveries();

                    if (lobjOverPayment.iclbPaymentRecovery.Count() == 0)
                        return true;
                }
            }
            return false;
        }

        public busPayeeAccount ibusPreRTWPayeeAccount { get; set; }
        public void LoadPreRTWPayeeAccount()
        {
            if (ibusPreRTWPayeeAccount.IsNull())
                ibusPreRTWPayeeAccount = new busPayeeAccount();
            ibusPreRTWPayeeAccount.FindPayeeAccount(icdoBenefitApplication.pre_rtw_payeeaccount_id);
        }

        public bool IsDCPlan()
        {
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            if (ibusPersonAccount.ibusPlan.IsNull()) ibusPersonAccount.LoadPlan();
            if (ibusPersonAccount.ibusPlan.IsDCRetirementPlan() || ibusPersonAccount.ibusPlan.IsHBRetirementPlan())
                return true;
            return false;
        }

        public bool IsSingleLifeStandardRhicOption()
        {
            if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionSingleLife &&
                icdoBenefitApplication.rhic_option_value == busConstant.RHICOptionStandard)
                return true;
            return false;
        }

        public DataSet rptRetirementMissingPayrollReport()
        {
            DataSet ldsReport = new DataSet("Retirement Missing Payroll");
            DataTable ldtTable = CreateRetirementMissingPayrollTable();
            ldtTable.TableName = busConstant.ReportTableName;
            DataTable ltbResults = Select("cdoPayeeAccount.RetirementMissingPayroll", new object[1] { DateTime.Now.AddMonths(-1).GetFirstDayofCurrentMonth() });
            foreach (DataRow ldtr in ltbResults.Rows)
            {
                DataRow ldtrRow = ldtTable.NewRow();
                ldtrRow["PERSLINKID"] = ldtr["PERSLINKID"];
                ldtrRow["NAME"] = ldtr["NAME"];
                ldtrRow["RETIREMENT_DATE"] = ldtr["RETIREMENT_DATE"];
                ldtrRow["BENEFIT_TYPE"] = ldtr["BENEFIT_TYPE"];
                ldtrRow["STATUS"] = ldtr["STATUS"];
                ldtrRow["ACTION_STATUS"] = ldtr["ACTION_STATUS"];
                ldtrRow["PAYEE_ACCOUNT_ID"] = ldtr["PAYEE_ACCOUNT_ID"];

                ldtTable.Rows.Add(ldtrRow);
            }
            ldsReport.Tables.Add(ldtTable);
            return ldsReport;
        }

        private DataTable CreateRetirementMissingPayrollTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn dc1 = new DataColumn("PERSLINKID", Type.GetType("System.Int32"));
            DataColumn dc2 = new DataColumn("NAME", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("RETIREMENT_DATE", Type.GetType("System.DateTime"));
            DataColumn dc4 = new DataColumn("BENEFIT_TYPE", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("STATUS", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("ACTION_STATUS", Type.GetType("System.String"));
            DataColumn dc7 = new DataColumn("PAYEE_ACCOUNT_ID", Type.GetType("System.Int32"));
            ldtbReportTable.Columns.Add(dc1);
            ldtbReportTable.Columns.Add(dc2);
            ldtbReportTable.Columns.Add(dc3);
            ldtbReportTable.Columns.Add(dc4);
            ldtbReportTable.Columns.Add(dc5);
            ldtbReportTable.Columns.Add(dc6);
            ldtbReportTable.Columns.Add(dc7);
            return ldtbReportTable;
        }


        //PIR-16812
        #region PIR-16812

        public bool VestedEmployerContributionPercentage(DateTime adtePersGoLiveDate)
        {
            /*Def Comp Validation Starts Here..*/
            //1.	Check if Def Comp plan exists (regardless of dates) – Yes, move on; No, no error, done.
            DataTable ldtbDeferredCompAccounts = Select("cdoPersonAccount.LoadDefCompAccountsEnrolled", new object[1] { icdoBenefitApplication.member_person_id });
            if (ldtbDeferredCompAccounts.Rows.Count > 0)
            {
                Collection<busPersonAccount> lclcDeferredCompAccounts = GetCollection<busPersonAccount>(ldtbDeferredCompAccounts, "icdoPersonAccount");
                foreach (busPersonAccount lbusPersonAccount in lclcDeferredCompAccounts)
                {
                    //2.	Check if any Payroll Details for that plan are in Review or Valid status.  Yes, Error, done; No, no error, done
                    DataTable ldtbRevOrValidDetailsByTermDate = Select("cdoEmployerPayrollDetail.LoadDefCompDetailsByEffectiveDate",
                                                        new object[4] { lbusPersonAccount.icdoPersonAccount.plan_id, busConstant.DepositDetailStatusReview, busConstant.PayrollDetailStatusValid, ibusPerson.icdoPerson.ssn });
                    if (ldtbRevOrValidDetailsByTermDate.Rows.Count > 0) //when review or valid details exists 
                    {
                        return true;
                    }
                    //3.	Get latest End Date from SGT_PERSON_ACCOUNT_DEFERRED_COMP_PROVIDER
                    DataTable ldtblatestEndDateProvider = Select("entEmployerPayrollDetail.GetLatestEndDateFromDefComProvider",
                                                        new object[1] { lbusPersonAccount.icdoPersonAccount.person_account_id });
                    if(ldtblatestEndDateProvider.IsNotNull() && ldtblatestEndDateProvider.Rows.Count > 0)
                    {
                        if (ldtblatestEndDateProvider.Rows[0][enmPersonAccountDeferredCompProvider.end_date.ToString()] != DBNull.Value)
                        {
                            if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                            {
                                DateTime ldtelatPvdrEndDate = Convert.ToDateTime(ldtblatestEndDateProvider.Rows[0][enmPersonAccountDeferredCompProvider.end_date.ToString()]);
                                if (ldtelatPvdrEndDate >= adtePersGoLiveDate && busGlobalFunctions.CheckDateOverlapping(ldtelatPvdrEndDate, ibusPersonAccount.icdoPersonAccount.start_date, ibusPersonAccount.icdoPersonAccount.end_date))
                                {
                                    //5. Check Def Comp Contribution table for combined record (amount >0) where End Date between Pay Period Start and Pay Period End Date.  Yes, no error, done; No, error
                                    DataTable ldtbComRecordAmount = Select("entEmployerPayrollDetail.DoesDefCompContriExistsPerEffecDate",
                                                                    new object[2] { lbusPersonAccount.icdoPersonAccount.person_account_id, ldtelatPvdrEndDate });

                                    if (ldtbComRecordAmount.IsNotNull() && ldtbComRecordAmount.Rows.Count > 0
                                        && ldtbComRecordAmount.Rows[0][enmPersonAccountDeferredCompProvider.per_pay_period_contribution_amt.ToString()] != DBNull.Value &&
                                        Convert.ToDecimal(ldtbComRecordAmount.Rows[0][enmPersonAccountDeferredCompProvider.per_pay_period_contribution_amt.ToString()]) > 0)
                                    {
                                        continue;
                                    }
                                    else
                                        return true;
                                }
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool AreAnyOutstandingDetailsExist()
        {
            DateTime ldtPERSLinkLive = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.PERSLinkGoLiveDate, iobjPassInfo));
            if (icdoBenefitApplication.benefit_account_type_value != busConstant.ApplicationBenefitTypePostRetirementDeath)
            {
                DateTime ldteEmpEndate = DateTime.MinValue;
                //Check if any valid review details exists for the plan the application came for and if exist throw error
                DataTable ldtEMployeeDetailValidOrReview = busBase.Select("cdoBenefitApplication.PayrollDetailIsValidOrReview", 
                                                                new object[2] { icdoBenefitApplication.plan_id, ibusPerson.icdoPerson.ssn });
                if (ldtEMployeeDetailValidOrReview.Rows.Count > 0)
                    return true;
                else
                {
                    if (ibusPersonAccount.IsNull()) LoadPersonAccount();
                    if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0 && icdoBenefitApplication.plan_id > 0)
                    {
                        DataTable ldtLatestEmpDetailFromPAED = busBase.Select("cdoBenefitApplication.GetLatestEmpDtlEndDateFromPAED", 
                                                               new object[2] { ibusPersonAccount.icdoPersonAccount.person_account_id, icdoBenefitApplication.plan_id });
                        if (ldtLatestEmpDetailFromPAED.Rows.Count > 0 && ldtLatestEmpDetailFromPAED.Rows[0][enmPersonEmploymentDetail.end_date.ToString()] != DBNull.Value)
                        {
                            ldteEmpEndate = Convert.ToDateTime(ldtLatestEmpDetailFromPAED.Rows[0][enmPersonEmploymentDetail.end_date.ToString()]);
                            if (ldteEmpEndate != DateTime.MinValue && ldteEmpEndate >= ldtPERSLinkLive)
                            {
                                //if employment termination month's contribution is not posted yet, throw an error 
                                DataTable ldtTermDateRetContributions = busBase.Select("cdoBenefitApplication.IsTermDateRetContributionPosted", 
                                                                    new object[2] { ibusPersonAccount.icdoPersonAccount.person_account_id, ldteEmpEndate });



                                if (!(ldtTermDateRetContributions.IsNotNull() && ldtTermDateRetContributions.Rows.Count > 0 &&
                                    ldtTermDateRetContributions.Rows[0][enmPersonAccountRetirementContribution.pension_service_credit.ToString()] != DBNull.Value && 
                                    Convert.ToDecimal(ldtTermDateRetContributions.Rows[0][enmPersonAccountRetirementContribution.pension_service_credit.ToString()]) > 0))
                                    return true;
                            }
                        }
                    }
                    return VestedEmployerContributionPercentage(ldtPERSLinkLive);
                }
            }
            return false;
        }

        #endregion PIR-16812

        //PIR 19010 & 18993

        public bool DoesPersonHaveOpenContriEmpDtlByPlan()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.iclbAccountEmploymentDetail == null)
                ibusPersonAccount.LoadPersonAccountEmploymentDetails();
            if (!IsBenefitOptionTransfers())
            {
                foreach (var lbusPAEmpDetail in ibusPersonAccount.iclbAccountEmploymentDetail)
                {
                    if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                        lbusPAEmpDetail.LoadPersonEmploymentDetail(false);

                    if (lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing &&
                        lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override void AfterSetActivityInstance()
        {
            base.AfterSetActivityInstance();
            SetBenAppRecivedDate();
        }
        public void SetBenAppRecivedDate() =>
                icdoBenefitApplication.received_date = (ibusBaseActivityInstance.IsNotNull() && 
            ibusBaseActivityInstance is busBpmActivityInstance && icdoBenefitApplication.received_date == DateTime.MinValue) ? 
            ((busBpmActivityInstance)ibusBaseActivityInstance).icdoBpmActivityInstance.created_date.Date : icdoBenefitApplication.received_date.Date;

        //PIR 14288 - Added Validation if Benefit Overpayment exists for a RTW employee but there is no recovery for it.
        public bool IsBenefitOverpayment()
        {
            bool iblnIsBenefitOverPayment = false;
            DataTable idtAllPayeeAccount = Select<cdoPayeeAccount>(new string[1] { "payee_perslink_id" }, new object[1] { icdoBenefitApplication.member_person_id }, null, null);
            foreach (DataRow dr in idtAllPayeeAccount.Rows)
            {
                DataTable idtOverPaymentHdr = Select<cdoPaymentBenefitOverpaymentHeader>(new string[1] { "payee_account_id" }, new object[1] { dr["PAYEE_ACCOUNT_ID"] }, null, null);
                if (idtOverPaymentHdr.Rows.Count > 0)
                {
                    foreach (DataRow ldr in idtOverPaymentHdr.Rows)
                    {
                        DataTable ldtRecovery = Select<cdoPaymentRecovery>(new string[1] { "BENEFIT_OVERPAYMENT_ID" },
                                                                   new object[1] { ldr["BENEFIT_OVERPAYMENT_ID"] },
                                                                   null, null);
                        if (ldtRecovery.Rows.Count == 0)
                        {
                            iblnIsBenefitOverPayment = true;
                        }
                        else
                        {
                            foreach (DataRow ldrRecovery in ldtRecovery.Rows)
                            {
                                if (Convert.ToString(ldrRecovery["STATUS_VALUE"]) == busConstant.RecoveryStatusPendingApproval)
                                {
                                    iblnIsBenefitOverPayment = true;
                                }
                            }
                        }
                    }
                }
            }
            return iblnIsBenefitOverPayment;
        }
    }
}
