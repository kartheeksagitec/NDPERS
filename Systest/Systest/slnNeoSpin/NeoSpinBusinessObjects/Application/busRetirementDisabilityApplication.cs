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
using NeoSpin.Common;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busRetirementDisabilityApplication : busBenefitApplication
    {

        public Collection<cdoCodeValue> LoadBenefitOptionsBasedOnPlans()
        {
            return LoadBenefitOptionsBasedOnPlans(icdoBenefitApplication.plan_id, icdoBenefitApplication.benefit_account_type_value);
        }
		//PIR 15831 - APP 7007 template to be associated with case management screen too
        public busRetirementDisabilityApplication ibusRetirementDisabilityApplication { get; set; }
        public bool iblnisVerifyBtnClicked { get; set; } // PIR 14614
		//PIR 15421
        public bool iblnisDeferBtnClicked { get; set; }
        public bool iblnisPendingBtnClicked { get; set; }
        public bool iblnisCancelBtnClicked { get; set; }
        public bool iblnisDenyBtnClicked { get; set; }

        private bool _iblnIsRaiseWarningFor180Days;
        public bool iblnIsRaiseWarningFor180Days
        {
            get { return _iblnIsRaiseWarningFor180Days; }
            set { _iblnIsRaiseWarningFor180Days = value; }
        }

        public string IsDeferralDateSameAsNormalRetirementDate
        {
            get
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.ibusPlan == null)
                    ibusPersonAccount.LoadPlan();
                string lstrPlanCode = ibusPersonAccount.ibusPlan.icdoPlan.plan_code;
                if (IComparer.Equals(icdoBenefitApplication.retirement_date, GetNormalRetirementDateBasedOnNormalEligibility(ibusPersonAccount.ibusPlan.icdoPlan.plan_code)))
                    return "Y";
                return "N";
            }
        }
        //****************************************** these is used in Deffered Batch

        // this property is used in the correspondence APP- 7002
        public DateTime idtRetirementDateMInus30Days
        {
            get
            {
                //PIR 23086 - Changed 30 days to 31 days prior to retirement date 
                return (icdoBenefitApplication.retirement_date != DateTime.MinValue ? icdoBenefitApplication.retirement_date.AddDays(-31) : DateTime.MinValue);
            }
        }


        //this value is used to set SSLI Age in the new mode as per Plan
        //the value of SSLI Age for plan is stored in the Code value table
        public int GetSSLIAgeByPlan()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();

            string lstrPlanCode = ibusPersonAccount.ibusPlan.icdoPlan.plan_code;
            return Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(1906, lstrPlanCode, iobjPassInfo));
        }

        //BR-62
        //Check the eligibility for Disability 
        //For plan Main/LE/NG/HP/Judges check if the working days is more than 180 days(6 months)
        //For Plan job Service check the Total VSC is greater than 60 months
        public bool CheckMemberEligibilityForDisabilityRetirement()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            // for Main/LE/NG/HP/Judges
            //6 months of employment
            if ((icdoBenefitApplication.plan_id == busConstant.PlanIdMain)
               || (icdoBenefitApplication.plan_id == busConstant.PlanIdMain2020) //PIR 20232                
               || (icdoBenefitApplication.plan_id == busConstant.PlanIdLEWithoutPS)
               || (icdoBenefitApplication.plan_id == busConstant.PlanIdJudges) || (icdoBenefitApplication.plan_id == busConstant.PlanIdLE)
               || (icdoBenefitApplication.plan_id == busConstant.PlanIdNG) || (icdoBenefitApplication.plan_id == busConstant.PlanIdHP)
               || (icdoBenefitApplication.plan_id == busConstant.PlanIdBCILawEnf)// pir 7943
               || (icdoBenefitApplication.plan_id == busConstant.PlanIdStatePublicSafety) //PIR 25729
               )
            {
                //no of working days should exceed 180 days               
                return IsNumberOfEmploymentDays180();
            }

            //for Job Service - 60 months of VSC
            if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
            {
                // added retirement_date as parameter done by Deepa
                // in order to get only the contribution those falls below this date                
                if (ibusPersonAccount.ibusPerson.GetTotalVSCForPerson(true, icdoBenefitApplication.retirement_date) >= 60)
                    return true;
            }
            return false;
        }

        //check for employment days 180
        public bool IsNumberOfEmploymentDays180()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            //This method excludes withdrawn
            if (ibusPersonAccount.ibusPerson.icolPersonAccountByBenefitType == null)
                ibusPersonAccount.ibusPerson.LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeRetirement);

            Collection<busPersonEmploymentDetail> lclbPerEmpDetail = new Collection<busPersonEmploymentDetail>();
            foreach (busPersonAccount lbusPersonAccount in ibusPersonAccount.ibusPerson.icolPersonAccountByBenefitType)
            {
                if (lbusPersonAccount.iclbAccountEmploymentDetail == null)
                    lbusPersonAccount.LoadPersonAccountEmploymentDetails();

                foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in lbusPersonAccount.iclbAccountEmploymentDetail)
                {
                    if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                        lbusPAEmpDetail.LoadPersonEmploymentDetail();

                    if (!lclbPerEmpDetail.Any(i => i.icdoPersonEmploymentDetail.person_employment_dtl_id ==
                        lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id))
                    {
                        lclbPerEmpDetail.Add(lbusPAEmpDetail.ibusEmploymentDetail);
                    }
                }
            }

            int lintTotalContributionDays = 0;
            _iblnIsRaiseWarningFor180Days = false;
            foreach (busPersonEmploymentDetail lbusPEDetail in lclbPerEmpDetail)
            {
                DateTime ldtEndDate = lbusPEDetail.icdoPersonEmploymentDetail.end_date;
                if (ldtEndDate == DateTime.MinValue)
                {
                    ldtEndDate = DateTime.Now;
                    _iblnIsRaiseWarningFor180Days = true;
                }
                lintTotalContributionDays += busGlobalFunctions.DateDiffInDays(lbusPEDetail.icdoPersonEmploymentDetail.start_date, ldtEndDate);
            }

            if (lintTotalContributionDays >= 180) return true;
            return false;
        }

        //check is person Eligible
        public override bool CheckIsPersonEligible()
        {
            if ((icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                  && (icdoBenefitApplication.plan_id != busConstant.PlanIdDC &&
                   icdoBenefitApplication.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                   icdoBenefitApplication.plan_id != busConstant.PlanIdDC2025)) //PIR 25920
            {
                if (CheckMemberEligibilityForDisabilityRetirement())
                    return true;
            }
            else
            {
                return base.CheckIsPersonEligible();
            }
            return false;
        }

        //BR - 51-57
        //Load latest Employment details
        //Check if the application date is within the 12 months passed employment end date
        public bool IsRecievedDateIsValidDate()
        {
            //if (ibusPersonAccount == null)
            //    LoadPersonAccount();
            //if (ibusPersonEmploymentDtl == null)
            //    ibusPersonEmploymentDtl = ibusPersonAccount.GetLatestEmploymentDetail();

            bool lblnResult = false;
            if (icdoBenefitApplication.received_date != DateTime.MinValue)
            {
                //DateTime ldtEndDate = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date;
                //DateTime ldtExpectedTerminationDate = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date.AddMonths(12);

                DateTime ldtExpectedTerminationDate = icdoBenefitApplication.termination_date.AddMonths(12);

                // UAT PIR ID 1351 && UAT PIR ID 1154
                if (icdoBenefitApplication.termination_date == DateTime.MinValue)
                    lblnResult = true;
                else
                {
                    if (icdoBenefitApplication.received_date <= ldtExpectedTerminationDate)
                        lblnResult = true;
                }
                //if (ldtEndDate == DateTime.MinValue
                //    || (icdoBenefitApplication.received_date <= ldtExpectedTerminationDate))
                //    lblnResult = true;
            }

            return lblnResult;
        }

        //BR-51-77
        //for disability only
        //check if plan particippation status is Withdrawan then return bool value
        public bool CheckIfPlanInWithdrawnStatus()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            return (ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn);
        }

        //BR-51-18 & 76
        //check application already exists for person and plan
        //Load all application for the plan and person
        //check if for the beenfit type retirement, application benefit type disability exists and  status is (Valid or review)
        //likewise loop thru the collection of already existing application and check for the disability
        public override bool CheckBenefitApplicationIsValid()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbBenefitApplication == null)
                ibusPersonAccount.ibusPerson.LoadBenefitApplication();
            foreach (busBenefitApplication lobjApplication in ibusPersonAccount.ibusPerson.iclbBenefitApplication)
            {
                if (lobjApplication.icdoBenefitApplication.plan_id == icdoBenefitApplication.plan_id)
                {
                    //check for retirement application
                    //throw warning message if a disability application already exist for person and plan with status Review Or valid.
                    //disability with processed status and active payment status exists (will be done after 71)
                    if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                    {
                        if (lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                            if (
                                (((lobjApplication.icdoBenefitApplication.status_value == busConstant.ApplicationStatusReview)
                                    ||
                                    (lobjApplication.icdoBenefitApplication.status_value == busConstant.ApplicationStatusValid)))
                                ||
                                    ((lobjApplication.icdoBenefitApplication.status_value == busConstant.ApplicationStatusProcessed)
                                    && (lobjApplication.CheckActivePayeeAccountExists())))
                            {
                                iblnDisabilityWithRvwOrVldStatusExistsForRetirement = true;
                                return iblnDisabilityWithRvwOrVldStatusExistsForRetirement;
                            }
                    }
                    //throw warning message if a disability application already exist for person and plan with status Review Or valid.
                    if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    {
                        if ((lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                            &&
                            (lobjApplication.icdoBenefitApplication.status_value == busConstant.ApplicationStatusProcessed)
                            &&
                            ((lobjApplication.icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
                            || (lobjApplication.icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal)))
                        {
                            iblnRetirementWithProcessedStatusExistsForDisability = true;
                            return iblnRetirementWithProcessedStatusExistsForDisability;
                        }
                    }
                }
            }
            return base.CheckBenefitApplicationIsValid(); ;
        }
        // BR - 051 - 15
        //Check if the retirement date is earlier than the first of the following the most recent month of employment
        public bool CheckRetDateIsEarlierThanFirstOfFollowingEmploymentEndDate()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonEmploymentDtl == null)
                ibusPersonEmploymentDtl = ibusPersonAccount.GetLatestEmploymentDetail();
            if (icdoBenefitApplication.retirement_date != DateTime.MinValue)
            {
                DateTime ldtFirstOFFollowingEmploymentEndDate = new DateTime(ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date.AddMonths(1).Year, ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date.AddMonths(1).Month, 1); ;
                if ((icdoBenefitApplication.retirement_date < ldtFirstOFFollowingEmploymentEndDate && icdoBenefitApplication.tffr_calculation_method_value.IsNullOrEmpty()) //PROD PIR 6389
                    || icdoBenefitApplication.retirement_date < icdoBenefitApplication.termination_date)
                {
                    return true;
                }
            }
            return false;
        }

        public DateTime GetNormalRetirementDateBasedOnNormalEligibility(string astrPlanCode)
        {
            return GetNormalRetirementDateBasedOnNormalEligibility(astrPlanCode, false);
        }

        //get normal retirement date based on the normal retirement eligibility
        //This Overridden Method is created as a special case of finding the NRD for Disability based on Retirement Eligibility Rules.
        //The boolean parameter will be set only when checking for Disability
        public DateTime GetNormalRetirementDateBasedOnNormalEligibility(string astrPlanCode, bool ablnIsBenefitTypeRetirement)
        {
            string lstrBenefitAccountType = string.Empty;
            if (ablnIsBenefitTypeRetirement)
            {
                lstrBenefitAccountType = busConstant.ApplicationBenefitTypeRetirement;
            }
            else
            {
                lstrBenefitAccountType = icdoBenefitApplication.benefit_account_type_value;
            }

            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();
            if (icdoBenefitApplication.idecTVSC == 0.00M)
                icdoBenefitApplication.idecTVSC = GetRoundedTVSC();


            //PIR 13003
            //Calculating Consolidated Extra Service Credit for Final Calcultaion
            decimal ldecConsolidatedServiceCredit = 0.0M;

            ldecConsolidatedServiceCredit = GetConsolidatedExtraServiceCredit();

            icdoBenefitApplication.idtNormalRetirementDate = GetNormalRetirementDateBasedOnNormalEligibility(icdoBenefitApplication.plan_id, astrPlanCode,
                                                             ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id,
                                                             lstrBenefitAccountType,
                                                             ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth,
                                                             icdoBenefitApplication.idecTVSC, 0, iobjPassInfo,
                                                             icdoBenefitApplication.termination_date, ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                             false, ldecConsolidatedServiceCredit, icdoBenefitApplication.retirement_date); //PIR 14646

            return icdoBenefitApplication.idtNormalRetirementDate;
        }

        //PIR 13003
        //Calculating Consolidated Extra Service Credit for Final Calcultaion
        //adding SGT_PERSON_TFFR_TIAA_SERVICE in APRV status only
        public decimal GetConsolidatedExtraServiceCredit()
        {
            decimal ldecConsolidatedExtraServiceCredit = 0.0M;
            if (ibusPersonAccount == null)
                LoadPersonAccount();


            decimal ldecTFFRService = 0.00M;
            decimal ldecTIAAService = 0.00M;
            decimal ldecTentativeTFFRService = 0.00M;
            decimal ldecTentativeTIAAService = 0.00M;

            ibusPersonAccount.LoadTFFRTIAAService(ref ldecTFFRService, ref ldecTIAAService, ref ldecTentativeTFFRService, ref ldecTentativeTIAAService);
            
                //adding SGT_PERSON_TFFR_TIAA_SERVICE in APRV status only (excluding tentative values)
                ldecConsolidatedExtraServiceCredit = ldecTFFRService + ldecTIAAService;
            
            return ldecConsolidatedExtraServiceCredit;
        }

     

        //get Early retirment date based on the early retirement eligibility
        private DateTime GetEarlyRetirementDateBasedOnEarlyRetirement(int aintPlanID)
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();

            DateTime ldtEarlyRetirementDate = DateTime.MinValue;

            ldtEarlyRetirementDate = GetEarlyRetirementDateBasedOnEarlyRetirement(icdoBenefitApplication.plan_id,
                                        ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id, icdoBenefitApplication.benefit_account_type_value,
                                        ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth, icdoBenefitApplication.idecTVSC, iobjPassInfo, ibusPersonAccount);

            return ldtEarlyRetirementDate;
        }

        //BR-051-85,86
        //Check if the Retirement date is less than or greater than the normal retirement date eligible for Deferred       
        public void SetRetirementDateBasedOnEligibilityForDeferred()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();

            DateTime ldtFirstOfMonthAfterMemberAttainsDefEligibility = DateTime.MinValue;

            if (icdoBenefitApplication.benefit_sub_type_value == null)
                SetBenefitSubType();

            if ((icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal)
                || (icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO))
            {
                ldtFirstOfMonthAfterMemberAttainsDefEligibility = GetNormalRetirementDateForDeferringAppl();

                if (icdoBenefitApplication.retirement_date == DateTime.MinValue)
                {
                    icdoBenefitApplication.retirement_date = ldtFirstOfMonthAfterMemberAttainsDefEligibility;
                    iintIsRetirementDateNotEqualToNormalOREalryEligibleDate = 0;
                }
            }

            if (icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
            {
                if (icdoBenefitApplication.retirement_date == DateTime.MinValue)
                {
                    string lstrPlanCode = ibusPersonAccount.ibusPlan.icdoPlan.plan_code;
                    icdoBenefitApplication.retirement_date = GetNormalRetirementDateBasedOnNormalEligibility(lstrPlanCode);
                    iintIsRetirementDateNotEqualToNormalOREalryEligibleDate = 0;
                }
            }
        }

        private DateTime GetNormalRetirementDateForDeferringAppl()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            DateTime ldtFirstOfMonthAfterMemberAttainsDefEligibility = DateTime.MinValue;
            DateTime ldtMemberAttainsDefEligibleAge = ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth.AddYears(73);//PIR 24248 - Eligible Age changed from 70.5 to 72.
            ldtFirstOfMonthAfterMemberAttainsDefEligibility = new DateTime(ldtMemberAttainsDefEligibleAge.AddMonths(1).Year, ldtMemberAttainsDefEligibleAge.AddMonths(1).Month, 1);
            return ldtFirstOfMonthAfterMemberAttainsDefEligibility;
        }

        //Validate the retirement date based on benefit sub type        
        public void ValidateRetirementDateBasedOnBenefitSubType()
        {
            iintIsRetirementDateNotEqualToNormalOREalryEligibleDate = 0;
            if ((icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal)
                || (icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO))
            {
                if (GetNormalRetirementDateForDeferringAppl() < icdoBenefitApplication.retirement_date)
                {
                    iintIsRetirementDateNotEqualToNormalOREalryEligibleDate = 1;
                }
            }
            if (icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
            {
                if (icdoBenefitApplication.plan_id != busConstant.PlanIdDC &&
                  icdoBenefitApplication.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                  icdoBenefitApplication.plan_id != busConstant.PlanIdDC2025) //PIR 25920
                {
                    if (icdoBenefitApplication.retirement_date < GetEarlyRetirementDateBasedOnEarlyRetirement(icdoBenefitApplication.plan_id))
                    {
                        iintIsRetirementDateNotEqualToNormalOREalryEligibleDate = 2;
                    }
                }
            }
        }

        //BR-051-08
        //Check SSLI entered is valid
        //1. if it is less than the SSLI based on Plan
        //2. Check if it is less that members age.
        // if it fails any of the condition then returns error.
        public bool CheckSSLIEnteredIsValid()
        {
            if (icdoBenefitApplication.ssli_age != 0)
            {
                if ((icdoBenefitApplication.ssli_age < GetSSLIAgeByPlan())
                    || (icdoBenefitApplication.ssli_age < idecMemberAgeBasedOnRetirementDate))
                { return true; }
            }
            return false;
        }

        //Get Latest Employment and Set Employment end date 
        public void SetTerminationDateAsEmploymentEndDate()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            ibusPersonEmploymentDtl = ibusPersonAccount.GetLatestEmploymentDetail();
            if (ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date != DateTime.MinValue)
            {
                icdoBenefitApplication.termination_date = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date;
            }
        }

        //Set SSLI age in the new mode only as per plan
        //when ever the SSLI age is set mark the uniform flag as YES
        //SSLI must be set for all plans except HP and DC
        public void SetSSLIAge()
        {
            if ((icdoBenefitApplication.plan_id != busConstant.PlanIdHP)
                    &&
                  (icdoBenefitApplication.plan_id != busConstant.PlanIdDC &&
                    icdoBenefitApplication.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                    icdoBenefitApplication.plan_id != busConstant.PlanIdDC2025)) //PIR 25920
            {
                icdoBenefitApplication.ssli_age = GetSSLIAgeByPlan();
            }
        }


        //BR-051-63
        //Check if the Member s age plus TVSC is greater than 79
        public bool CheckMemberAgePlusTVSCLessThan79()
        {
            if ((iintMembersAgeInMonthsAsOnRetirementDate + icdoBenefitApplication.idecTVSC) < (12 * 79))
            {
                return false;
            }
            return true;
        }

        //BR-51-46
        // this method will check if the member is allowed to have reduced RHIC option
        // 1. applicable for DB and disability
        //      RHIC reduced option selected and member is not married throw error       
        public bool CheckMemberEligibleForReducedRHICForDBAndDisability()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            if (IsReducedRHICSelected())
            {
                if (ibusPersonAccount.ibusPlan.IsDBRetirementPlan())
                {
                    if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    {
                        if (ibusPersonAccount.ibusPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried)
                            return false;
                    }
                }
            }
            return true;
        }

        //BR-51-46
        // For DC only
        //member is not married and reduced option is selected.
        public bool CheckMemberEligibleForReducedRHICForDC()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            if (IsReducedRHICSelected())
            {
                if (ibusPersonAccount.ibusPlan.IsDCRetirementPlan() || ibusPersonAccount.ibusPlan.IsHBRetirementPlan())
                {
                    if (ibusPersonAccount.ibusPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried)
                        return false;
                }
            }
            return true;
        }

        //for plan as HP or Judges, Reduced RHIC must not be allowed to select if the Benefit option is Normal.
        public bool IsHpJudgesAllowedToReducedRHIC()
        {
            if ((icdoBenefitApplication.benefit_option_value != null)
                && (icdoBenefitApplication.rhic_option_value != null))
            {
                if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit)
                {
                    if ((icdoBenefitApplication.plan_id == busConstant.PlanIdHP)
                        || (icdoBenefitApplication.plan_id == busConstant.PlanIdJudges))
                    {
                        if (IsReducedRHICSelected())
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        // check if the 100% j & S is selected for the reduced RHIC option
        public bool Is100PercentJAndSReducedRHICOptionSelected()
        {
            if ((icdoBenefitApplication.benefit_option_value != null)
                          && (icdoBenefitApplication.rhic_option_value != null))
            {
                //if ((icdoBenefitApplication.plan_id == busConstant.PlanIdHP)
                //       || (icdoBenefitApplication.plan_id == busConstant.PlanIdJudges))
                //{
                if ((icdoBenefitApplication.benefit_option_value == busConstant.BenefitOption100PercentJS)
                    && (IsReducedRHICSelected()))
                    return true;
                //}
            }
            return false;
        }
        //BR-051-37
        //check the joint annuintant is Active Spouse 
        //Load Contact and loop thru collection
        //filter the record that is having contact person id same as joint annuitant id and 
        //with relationship value as Spouse
        //TODO: do we need to check the status of the Person Contact????????????????
        public bool IsJointAnnuitantIsActiveSpouse()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                ibusPersonAccount.ibusPerson.LoadContacts();
            foreach (busPersonContact lobjPersonContact in ibusPersonAccount.ibusPerson.icolPersonContact)
            {
                if (lobjPersonContact.icdoPersonContact.contact_person_id == icdoBenefitApplication.joint_annuitant_perslink_id)
                {
                    if ((lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                        &&
                        (lobjPersonContact.icdoPersonContact.status_value.TrimEnd() == busConstant.PersonContactStatusActive))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void SetReducedBenefitFlagOn()
        {
            if ((icdoBenefitApplication.plan_id == busConstant.PlanIdMain)
                || (icdoBenefitApplication.plan_id == busConstant.PlanIdMain2020) //PIR 20232
                || (icdoBenefitApplication.plan_id == busConstant.PlanIdLE)
                || (icdoBenefitApplication.plan_id == busConstant.PlanIdLEWithoutPS)
                || (icdoBenefitApplication.plan_id == busConstant.PlanIdJudges)
                || (icdoBenefitApplication.plan_id == busConstant.PlanIdBCILawEnf) // pir 7943
                || (icdoBenefitApplication.plan_id == busConstant.PlanIdNG) // PIR 25729
                || (icdoBenefitApplication.plan_id == busConstant.PlanIdStatePublicSafety)) //PIR 25729
            {
                if (IsReducedRHICSelected())
                    icdoBenefitApplication.reduced_benefit_flag = busConstant.Flag_Yes;
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            LoadMethodsToSetFieldAndLoadObject();
            if (icdoBenefitApplication.tffr_calculation_method_value.IsNotNullOrEmpty())
                icdoBenefitApplication.termination_date = idtTerminationDate; //PROD PIR ID 6389
            base.BeforeValidate(aenmPageMode);
        }

        //BR-051-67
        //Load Other Disability Benefits 
        //load datatable from code values for the code value Other disability benefits
        //loop through the datatable and create cdo object for Other disability Benefits
        //map the fields to cdoBenAppOthersDisBenefits
        //and add the newly created instance of cdo object to collection
        public void LoadOtherDisabilityBenefits()
        {
            //this check is done in order to know that user never entered any other disability benefit option            
            iclcBenAppOtherDisBenefit = new Collection<cdoBenAppOtherDisBenefit>();
            DataTable ldtbOtherDisBen = Select<cdoBenAppOtherDisBenefit>(new string[1] { "benefit_application_id" }, new object[1] { icdoBenefitApplication.benefit_application_id }, null, null);
            if (ldtbOtherDisBen.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbOtherDisBen.Rows)
                {
                    cdoBenAppOtherDisBenefit lobjOtherDisabilityBenefit = new cdoBenAppOtherDisBenefit();
                    lobjOtherDisabilityBenefit.LoadData(dr);
                    lobjOtherDisabilityBenefit.other_disability_benefit_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1911,
                                                                                               lobjOtherDisabilityBenefit.other_disability_benefit_value);
                    iclcBenAppOtherDisBenefit.Add(lobjOtherDisabilityBenefit);
                }
            }
            //check if already options are entered for the application
            else
            {
                DataTable ldtbGetOthrDisBen = iobjPassInfo.isrvDBCache.GetCodeValues(1911);
                int lintIndex = 1;
                foreach (DataRow dr in ldtbGetOthrDisBen.Rows)
                {
                    cdoBenAppOtherDisBenefit lobjOtherDisabilityBenefit = new cdoBenAppOtherDisBenefit();
                    lobjOtherDisabilityBenefit.ben_app_other_dis_benefit_id = lintIndex * -1;
                    lobjOtherDisabilityBenefit.benefit_application_id = icdoBenefitApplication.benefit_application_id;
                    lobjOtherDisabilityBenefit.other_disability_benefit_value = dr["code_value"].ToString();
                    lobjOtherDisabilityBenefit.other_disability_benefit_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1911,
                                                                                                lobjOtherDisabilityBenefit.other_disability_benefit_value);
                    iclcBenAppOtherDisBenefit.Add(lobjOtherDisabilityBenefit);
                    lintIndex++;
                }
            }
        }

        //BR-051-69
        //Check if WSI Benefits entered for plan other than Judges and HP
        //if collection is null then Load collection
        //loop through collection and check if benefit begin date and end date is entered for WSI
        //if so then raise error
        public bool CheckWSIEnteredForValidPlan()
        {
            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                if (iclcBenAppOtherDisBenefit == null)
                    LoadOtherDisabilityBenefits();
                if ((icdoBenefitApplication.plan_id != busConstant.PlanIdHP) && (icdoBenefitApplication.plan_id != busConstant.PlanIdJudges))
                {
                    foreach (cdoBenAppOtherDisBenefit lobjBenApplotherDisBenefits in iclcBenAppOtherDisBenefit)
                    {
                        if (lobjBenApplotherDisBenefits.other_disability_benefit_value == busConstant.OtherDisBenWSI)
                        {
                            if ((lobjBenApplotherDisBenefits.benefit_begin_date != DateTime.MinValue)
                                || (lobjBenApplotherDisBenefits.monthly_benefit_amount != 0.00M)
                                || (lobjBenApplotherDisBenefits.benefit_end_date != DateTime.MinValue))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        //Validate date field in grid.
        //1. Check start date can not be blank if the end date/ benefit amount is entered       
        //3. Check benefit amount can not be blank if the end date/ start date is entered
        //4. Check benefit amount can not be negative if benefit amount is entered
        //5. Check start date can not be greater than end date        
        private void SetBooleanAfterValidatingGrid()
        {
            icdoBenefitApplication.iblnIsStartDateBlankOthrDisBenefit = false;
            icdoBenefitApplication.iblnIsBenefitAmountBlankOthrDisBenefit = false;
            icdoBenefitApplication.iblnIsBenefitAmountNegativeOthrDisBenefit = false;
            icdoBenefitApplication.iblnIsStartDateGreaterThanEndDateOthrDisBenefit = false;

            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                if (iclcBenAppOtherDisBenefit == null)
                    LoadOtherDisabilityBenefits();

                foreach (cdoBenAppOtherDisBenefit lobjBenApplotherDisBenefits in iclcBenAppOtherDisBenefit)
                {
                    //1.
                    if ((lobjBenApplotherDisBenefits.benefit_begin_date == DateTime.MinValue)
                        && ((lobjBenApplotherDisBenefits.benefit_end_date != DateTime.MinValue)
                        || (lobjBenApplotherDisBenefits.monthly_benefit_amount != 0.00M)))
                    {
                        icdoBenefitApplication.iblnIsStartDateBlankOthrDisBenefit = true;
                    }
                    //2.
                    if ((lobjBenApplotherDisBenefits.monthly_benefit_amount == 0.00M)
                        && ((lobjBenApplotherDisBenefits.benefit_end_date != DateTime.MinValue)
                        || (lobjBenApplotherDisBenefits.benefit_begin_date != DateTime.MinValue)))
                    {
                        icdoBenefitApplication.iblnIsBenefitAmountBlankOthrDisBenefit = true;
                    }
                    //3.
                    if (lobjBenApplotherDisBenefits.monthly_benefit_amount < 0.00M)
                    {
                        icdoBenefitApplication.iblnIsBenefitAmountNegativeOthrDisBenefit = true;
                    }
                    //4.
                    if ((lobjBenApplotherDisBenefits.benefit_begin_date != DateTime.MinValue)
                        && (lobjBenApplotherDisBenefits.benefit_end_date != DateTime.MinValue)
                        && (lobjBenApplotherDisBenefits.benefit_begin_date > lobjBenApplotherDisBenefits.benefit_end_date))
                    {
                        icdoBenefitApplication.iblnIsStartDateGreaterThanEndDateOthrDisBenefit = true;
                    }
                }
            }
        }

        public override void BeforePersistChanges()
        {
            //Set termination date
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            
            LoadMethodsToSetFieldAndLoadObject();
            if (icdoBenefitApplication.tffr_calculation_method_value.IsNotNullOrEmpty())
                icdoBenefitApplication.termination_date = idtTerminationDate; //PROD PIR ID 6389
            if (icdoBenefitApplication.ienuObjectState == ObjectState.Insert)
            {
                icdoBenefitApplication.Insert();
            }

            //Set the Object State for the Collection based on the Mode
            foreach (cdoBenAppOtherDisBenefit lcdoBenAppOtherDisBenefit in iclcBenAppOtherDisBenefit)
            {
                lcdoBenAppOtherDisBenefit.benefit_application_id = icdoBenefitApplication.benefit_application_id;
                lcdoBenAppOtherDisBenefit.ienuObjectState = ObjectState.Insert;
                if ((lcdoBenAppOtherDisBenefit.benefit_application_id > 0) && (lcdoBenAppOtherDisBenefit.ben_app_other_dis_benefit_id > 0))
                    lcdoBenAppOtherDisBenefit.ienuObjectState = ObjectState.Update;
            }
            if (iclbBenefitDROApplication == null)
                LoadDROApplication();
            //UAT pir - 1348
            //benefit reciept date to be populated only on approval of calculation for retirement benefit type
            if (icdoBenefitApplication.benefit_account_type_value != busConstant.ApplicationBenefitTypeRetirement)
            {
                foreach (busBenefitDroApplication lobjBenefitDROAppilcation in iclbBenefitDROApplication)
                {
                    if (!lobjBenefitDROAppilcation.IsDROApplicationCancelledOrDenied())
                    {
                        if (lobjBenefitDROAppilcation.icdoBenefitDroApplication.time_of_benefit_receipt_calc_value == busConstant.DROApplicationModelTimeOfBenfitRetirementDate)
                        {
                            lobjBenefitDROAppilcation.icdoBenefitDroApplication.benefit_receipt_date = icdoBenefitApplication.retirement_date;
                            lobjBenefitDROAppilcation.icdoBenefitDroApplication.Update();
                        }
                    }
                }
            }
            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            SetSuppressWarningFlagBasedOnErrors();
            
            //PIR 1761
            //suppress warning logic changed as per above PIR
            //icdoBenefitApplication.suppress_warnings_flag = busConstant.Flag_No;
            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                LoadOtherDisabilityBenefits();
            }
            //if the joint perslink id need not to be populated for the current options then set the joint annuitant object to null
            if (!iblnIsJointAnnuitantIdRequired)
            {
                icdoBenefitApplication.joint_annuitant_perslink_id = 0;
                ibusJointAnniutantPerson = new busPerson { icdoPerson = new cdoPerson() };
            }
            if ((icdoBenefitApplication.joint_annuitant_perslink_id != 0)
                && (icdoBenefitApplication.plan_id != busConstant.PlanIdJobService))
            {
                //istrJointAnnuitantRelationship = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(304, busConstant.PersonContactTypeSpouse);
                SetRelationshipOfJointAnnuitant();
            }
            if (icdoBenefitApplication.ssli_age > 0)
            {
                icdoBenefitApplication.ssli_effective_date = GetSSLIEffectivedate(ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth, icdoBenefitApplication.ssli_age);
            }

            /*****************************************************************************************************************************************
             * Change from Raj on the Calculation being set to Review when the Calculation is not in Review Status and Action status is not approved.
             * Code Starts here
            ******************************************************************************************************************************************/
            if (icdoBenefitApplication.benefit_calculation_id == 0)
                LoadBenefitCalculationID();

            if (icdoBenefitApplication.benefit_calculation_id > 0)
            {
                if (ibusBenefitCalculation.IsNull())
                    LoadBenefitCalculation();

                if (ibusBenefitCalculation.IsNotNull())
                {
                    if ((ibusBenefitCalculation.icdoBenefitCalculation.action_status_value != busConstant.CalculationStatusApproval)
                        && (ibusBenefitCalculation.icdoBenefitCalculation.status_value != busConstant.CalculationStatusReview))
                    {
                        ibusBenefitCalculation.icdoBenefitCalculation.status_value = busConstant.CalculationStatusReview;
                        ibusBenefitCalculation.icdoBenefitCalculation.Update();

                        ibusBenefitCalculation.iblnIsApplicationModified = true;
                        ibusBenefitCalculation.ValidateSoftErrors();
                    }
                }
            }
            /************************************************************************************************************************************************
             * Change from Raj on the Calculation being set to Review when the Calculation is not in Review Status and Action status is not approved.
             * Code Ends here
            *************************************************************************************************************************************************/
        }

        //this method checks whether the decimal part entered 
        //in the SSLI age if with in the range of (1/12 to 11/12)
        public bool IsSSLIDecimalPartWithinRange()
        {
            if (icdoBenefitApplication.ssli_age > 0.00M)
            {
                decimal ldecSSLITruncated = decimal.Truncate(icdoBenefitApplication.ssli_age);

                decimal ldecDecimalPart = icdoBenefitApplication.ssli_age - ldecSSLITruncated;
                if (ldecDecimalPart > 0.00M)
                {
                    if (!((ldecDecimalPart >= 0.0833M)
                        && (ldecDecimalPart <= 0.9167M)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //check SSLI based on the date of birth year part and the limits mentioned in the table 
        public bool IsSSLIAgeValid()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            int lintDateOfBirthYearPart = ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth.Year;

            DataTable ldtbGetSSLIAgeLimit = SelectWithOperator<cdoCodeValue>(new string[3] { "Code_id", "data1", "data2" }, new string[3] { "=", "<=", ">=" }, new object[3] { busConstant.CodeValueForSSLILimit, lintDateOfBirthYearPart, lintDateOfBirthYearPart }, null);
            if (ldtbGetSSLIAgeLimit.Rows.Count > 0)
            {
                decimal ldecUpperLimitSSLIAge = Convert.ToDecimal(ldtbGetSSLIAgeLimit.Rows[0]["data3"]);
                decimal ldecLowerLimitSSLIAge = busConstant.SSLIAgeLowerLimit;
                if ((icdoBenefitApplication.ssli_age > ldecUpperLimitSSLIAge)
                    || (icdoBenefitApplication.ssli_age < ldecLowerLimitSSLIAge))
                { return false; }
            }
            return true;
        }
        // UAT PIR: 1701. When perslink entered is a Non Spouse the system still populates the  relationship of the Member. IT should be blank if that person 
        // is not a  contact for the member
        //Set relationship based on contact person relateionship with member
        public void SetRelationshipOfJointAnnuitant()
        {
            istrJointAnnuitantRelationship = string.Empty;
            if (icdoBenefitApplication.joint_annuitant_perslink_id != 0)
            {
                ibusPersonAccount.ibusPerson.LoadContacts();
                foreach (busPersonContact lobjPersonContact in ibusPersonAccount.ibusPerson.icolPersonContact)
                {
                    if ((lobjPersonContact.icdoPersonContact.contact_person_id == icdoBenefitApplication.joint_annuitant_perslink_id)
                        && (lobjPersonContact.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive))
                    {
                        istrJointAnnuitantRelationship = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(304, lobjPersonContact.icdoPersonContact.relationship_value);
                        break;
                    }
                }
            }
        }


        private void LoadPayeeAccountByPersonId()
        {
            LoadPayeeAccountByPersonId(icdoBenefitApplication.joint_annuitant_perslink_id);
        }

        private void LoadPayeeAccountByPersonId(int aintPersonID)
        {
            DataTable ldtbList = Select<cdoPayeeAccount>(
                new string[1] { "payee_perslink_id" },
                new object[1] { icdoBenefitApplication.joint_annuitant_perslink_id }, null, null);
            iclbPayeeAccountByPersonID = GetCollection<busPayeeAccount>(ldtbList, "icdoPayeeAccount");
        }

        //BR- 051 - 44
        //Check if the 
        public bool CheckSpouseHasValidPayeeAccount()
        {
            //UAT PIR: 1474
            //Regardless of RHIC Option or Benefit Option The system should throw this validation if a Spouse is already retired.
            int lintjointAnnuitantID = icdoBenefitApplication.joint_annuitant_perslink_id;
            if (lintjointAnnuitantID == 0)
            {
                lintjointAnnuitantID = GetSpouseContactPersonID();
            }

            if (lintjointAnnuitantID != 0)
            {
                LoadPayeeAccountByPersonId(lintjointAnnuitantID);
                foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccountByPersonID)
                {
                    lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                    if (lobjPayeeAccount.ibusPayeeAccountActiveStatus != null)
                    {
                        //UAT PIR - 1155
                        string lstrData2Value = busGlobalFunctions.GetData2ByCodeValue(2203, lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                        if ((lstrData2Value != busConstant.PayeeAccountStatusCancelled)
                            && (lstrData2Value != busConstant.PayeeAccountStatusPaymentComplete)
                            && (lstrData2Value != busConstant.PayeeAccountStatusRefundProcessed))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        //BR-051-21
        //check if any refund application is already exists for this person and plan.
        public bool IsRefundApplicationExistsForPersonAndPlan()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbBenefitApplication == null)
                ibusPersonAccount.ibusPerson.LoadBenefitApplication();
            //UAT PIR - 1132
            foreach (busBenefitApplication lobjbenefitApplication in ibusPersonAccount.ibusPerson.iclbBenefitApplication)
            {
                lobjbenefitApplication.LoadPersonAccountByApplication();
            }

            var query = from p in ibusPersonAccount.ibusPerson.iclbBenefitApplication
                        where (p.icdoBenefitApplication.plan_id == icdoBenefitApplication.plan_id)
                     && (p.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                     && ((p.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied)
                     && (p.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled)
                     && (p.ibusPersonAccount.icdoPersonAccount.person_account_id == ibusPersonAccount.icdoPersonAccount.person_account_id)) // UAT PIR - 1132 - person accounts should not be same
                        select p;

            if (query.Count<busBenefitApplication>() > 0)
            {
                return true;
            }
            return false;
        }

        //BR-051-29
        public void SetQDROFlag()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbDROApplication == null)
                ibusPersonAccount.ibusPerson.LoadDROApplications();

            var query = from p in ibusPersonAccount.ibusPerson.iclbDROApplication
                        where (p.icdoBenefitDroApplication.status_value == busConstant.StatusValid
                        && icdoBenefitApplication.plan_id == p.icdoBenefitDroApplication.plan_id)
                        && ((p.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusApproved)
                        || (p.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified)
                        || (p.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusRecieved))
                        select p;

            if (query.Count<busBenefitDroApplication>() > 0)
            {
                icdoBenefitApplication.qdro_on_file_flag = busConstant.Flag_Yes;
            }
        }

        # region Button Click Methods

        public ArrayList btnDeferredClicked()
        {
			//PIR 15421
            iarrErrors = new ArrayList();
            iblnisDeferBtnClicked = true;

            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            LoadMethodsToSetFieldAndLoadObject();
            SetRetirementDateBasedOnEligibilityForDeferred();
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                //UAT PIR:1939. Error Removed From Hard Error since it has to be displayed only when deferred button is clicked.
                if (iintIsRetirementDateNotEqualToNormalOREalryEligibleDate == 1)
                {
                    lobjError = AddError(1939, "");
                    alReturn.Add(lobjError);
                    return alReturn;
                }

                SetRetirementDateBasedOnEligibilityForDeferred();
                icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusDeferred;
				
				//PIR 15421
                if (icdoBenefitApplication.benefit_application_id > 0)
                    icdoBenefitApplication.Update();
                else
                {
                    BeforeValidate(utlPageMode.New);
                    BeforePersistChanges();
                    base.PersistChanges();
                    AfterPersistChanges();
                }

                base.ValidateSoftErrors();
                LoadErrors();
                base.UpdateValidateStatus();
                icdoBenefitApplication.Select();
                alReturn.Add(this);

                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
			//PIR 15421
            iblnisDeferBtnClicked = false;
            return alReturn;
        }

        public override ArrayList btnVerfiyClicked()
        {
            iblnisVerifyBtnClicked = true; // PIR 14614
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            LoadMethodsToSetFieldAndLoadObject();
            //UCS 60 RTW member
            //if (IsPendingOverpaymentExists())
            //{
            //    lobjError = AddError(7731, "");
            //    alReturn.Add(lobjError);
            //    return alReturn;
            //}
            if (ibusPerson.IsNull()) LoadPerson();
            //PIR 16284/17801 //PIR 19010 & 18993
            if (DoesPersonHaveOpenContriEmpDtlByPlan())
            {
                lobjError = AddError(7505, "");
                alReturn.Add(lobjError);
                return alReturn;
            }

            //if (IsAllEmploymentEndDated())
            //{
            //    idtTerminationDate = DateTime.MinValue;
            //}
            //else 
            //pir fix. 1611
            //if term end date is blank then raise error.
            //else set the date to the latest employment details  end date.
            if (idtTerminationDate == DateTime.MinValue)
            {
                lobjError = AddError(1946, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            else
            {
                icdoBenefitApplication.termination_date = idtTerminationDate;
                alReturn = base.btnVerfiyClicked();
                if (iblnIsRTWMember)
                {
                    UpdateBenefitApplicationPersonAccount();
                }
            }
            iblnisVerifyBtnClicked = false; // PIR 14614
            return alReturn;
        }
        public override ArrayList btnCancelClicked()
        {
            iblnisCancelBtnClicked = true; // PIR 15421
            ArrayList alReturn = new ArrayList();
            LoadMethodsToSetFieldAndLoadObject();
            //PIR 17082 called base class validation methods
            alReturn = base.btnCancelClicked();
            iblnisCancelBtnClicked = false; // PIR 15421
            return alReturn;
        }

        public override ArrayList btnDenyClicked()
        {
            iblnisDenyBtnClicked = true; // PIR 15421
            iarrErrors = new ArrayList();
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            LoadMethodsToSetFieldAndLoadObject();
            alReturn = base.btnDenyClicked();

            if ((alReturn.Count == 1)
                && (alReturn[0].ToString() == this.ToString()))
            //"NeoSpin.BusinessObjects.busRetirementDisabilityApplication"))
            {
                //if the application is created via WFL, then on denying the application the system must update the workflow status as Completed
                busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Next, busConstant.ActivityStatusProcessed, iobjPassInfo);
            }
            iblnisDenyBtnClicked = false; // PIR 15421

            return alReturn;
        }

        private void LoadMethodsToSetFieldAndLoadObject()
        {
            //UAT PIR - 1207
            //if the SSLI checkbox is checked then only set the SSLI to 62
            //Set SSLI Age for Job Service as it will be always 62
            if ((icdoBenefitApplication.uniform_income_flag == busConstant.Flag_Yes) && (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService))
            {
                icdoBenefitApplication.ssli_age = busConstant.SSLIAgeForJobService;
            }

            //default retirement date for PLan DC
            //****************************Deepa - THIS CODE IS COMMENTED AS PER PIR = 1497 USER MUST ENTER RETIREMENT DATE 
            //****************************SYSTEM SHOULD NOT POPULATE THE RETIREMENT DATE BASED ON NORMAL ELIGIBILIITY DATE.
            //dated - 10/23/2009
            //if (icdoBenefitApplication.retirement_date == DateTime.MinValue)
            //{
            //    if (icdoBenefitApplication.plan_id == busConstant.PlanIdDC)
            //    {
            //        icdoBenefitApplication.retirement_date = ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth.AddYears(55).AddMonths(1);
            //    }
            //    else
            //    {
            if (icdoBenefitApplication.idtNormalRetirementDate == DateTime.MinValue)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.ibusPlan == null)
                    ibusPersonAccount.LoadPlan();
                GetNormalRetirementDateBasedOnNormalEligibility(ibusPersonAccount.ibusPlan.icdoPlan.plan_code);
            }
            //        icdoBenefitApplication.retirement_date = icdoBenefitApplication.idtNormalRetirementDate;
            //    }
            //}
            //this is reloaded again to refresh the value of TVSC after getting normal retirement date based on the new logic.
            icdoBenefitApplication.idecTVSC = GetRoundedTVSC();
            SetMemberAgeBasedOnRetirementDate();
            //SetReducedBenefitFlagOn();          
            SetBenefitSubType();
            SetBooleanAfterValidatingGrid();
            SetRelationshipOfJointAnnuitant();
            ValidateRetirementDateBasedOnBenefitSubType();
            SetQDROFlag();
            SetOrgIdAsLatestEmploymentOrgId();
            //PIR - 1826
            //this should be reset every time on save if there is any change in employment
            if (icdoBenefitApplication.termination_date != idtTerminationDate)
                icdoBenefitApplication.termination_date = idtTerminationDate;
            if (iblnIsRTWMember)
            {
                SetActurialBenefitReductionAllowedFlag();
            }

            //UAT PIR - 1134
            GetJointAnnuitantPerslinkId();
            LoadJointAnniutantPerson();
            CheckBenefitApplicationIsValid();           

        }

        public override ArrayList btnPendingVerificationClicked()
        {
            iblnisPendingBtnClicked = true; // PIR 15421
            iarrErrors = new ArrayList();

            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            LoadMethodsToSetFieldAndLoadObject();
            if (iblnIsRTWMember)
            {
                UpdateBenefitApplicationPersonAccount();
            }
            alReturn = base.btnPendingVerificationClicked();
            iblnisPendingBtnClicked = false; // PIR 15421
            return alReturn;
        }

        # endregion

        public busRetirementBenefitCalculation ibusBenefitCalculation { get; set; }

        public void LoadBenefitCalculation()
        {
            if (ibusBenefitCalculation.IsNull())
                ibusBenefitCalculation = new busRetirementBenefitCalculation();
            ibusBenefitCalculation.FindBenefitCalculation(icdoBenefitApplication.benefit_calculation_id);
        }

        // This method will load the corresponding Calculation ID for this application.
        // Calculation button visibility depends on this Calculation ID.
        // If Calculation ID is 0 Calculation Maintenance New mode else Update mode.
        public void LoadBenefitCalculationID()
        {
            DataTable ldtbResult = Select<cdoBenefitCalculation>(new string[1] { "benefit_application_id" },
                                        new object[1] { icdoBenefitApplication.benefit_application_id }, null, "benefit_calculation_id desc");
            if (ldtbResult.Rows.Count > 0)
            {
                icdoBenefitApplication.benefit_calculation_id = Convert.ToInt32(ldtbResult.Rows[0]["benefit_calculation_id"]);
            }
        }

        public ArrayList NewCalculate_Click(int aintBenefitApplicationId)
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            busRetirementDisabilityApplication lobjBenefitApplication = new busRetirementDisabilityApplication();
            lobjBenefitApplication.FindBenefitApplication(aintBenefitApplicationId);
            if (lobjBenefitApplication.ibusPersonAccount == null)
                lobjBenefitApplication.LoadPersonAccount();
            if (lobjBenefitApplication.ibusPersonAccount.ibusPerson == null)
                lobjBenefitApplication.ibusPersonAccount.LoadPerson();
            if (lobjBenefitApplication.ibusPersonAccount.ibusPlan == null)
                lobjBenefitApplication.ibusPersonAccount.LoadPlan();
            lobjBenefitApplication.LoadMethodsToSetFieldAndLoadObject();

            lobjBenefitApplication.ValidateHardErrors(utlPageMode.All);
            if (lobjBenefitApplication.iarrErrors.Count > 0)
            {
                foreach (utlError larr in lobjBenefitApplication.iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }

            if (alReturn.Count > 0)
            {
                lobjBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
                lobjBenefitApplication.icdoBenefitApplication.Update();
            }
            else
            {
                alReturn.Add(this);
            }
            return alReturn;
        }

        //UCS-80 -12
        //To check if the member has reached Normal Eligibility
        // only for Disability
        public bool IsMemberReachedNormalRetirement()
        {
            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                //Set the Value of Normal Retirement date when not set.
                if (icdoBenefitApplication.idtNormalRetirementDate == DateTime.MinValue)
                {
                    if (ibusPersonAccount == null)
                        LoadPersonAccount();
                    if (ibusPersonAccount.ibusPlan == null)
                        ibusPersonAccount.LoadPlan();
                    GetNormalRetirementDateBasedOnNormalEligibility(ibusPersonAccount.ibusPlan.icdoPlan.plan_code, true);
                }

                if ((icdoBenefitApplication.retirement_date != DateTime.MinValue)
                    && (icdoBenefitApplication.idtNormalRetirementDate != DateTime.MinValue))
                {
                    if (icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified)
                        if (icdoBenefitApplication.retirement_date > icdoBenefitApplication.idtNormalRetirementDate)
                            return true;
                }
            }
            return false;
        }

        //PIR - UAT - 858
        //to validate if the retirement date is not first of month
        public bool IsRetirementDatefirstOfMonth()
        {
            return (icdoBenefitApplication.retirement_date != DateTime.MinValue) ? ((icdoBenefitApplication.retirement_date.Day != 1) ? false : true) : true;
        }

        //Pay4259 property
        //This property would set the Age property from the Eligibility Tables.
        //UAT PIR: 1353
        public decimal idecNormarRetirementAge
        {
            get
            {
                decimal ldecMemberAgeBasedOnNRD = 0.00M;
                if (ibusPersonAccount == null)
                    LoadPersonAccount();

                if (ibusPersonAccount.ibusPlan == null)
                    ibusPersonAccount.LoadPlan();

                if (icdoBenefitApplication.idecTVSC == 0.00M)
                    icdoBenefitApplication.idecTVSC = GetRoundedTVSC();

                Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionNormalEligibility = new Collection<cdoBenefitProvisionEligibility>();
                lclbBenefitProvisionNormalEligibility = LoadEligibilityForPlan(icdoBenefitApplication.plan_id, ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id,
                    busConstant.ApplicationBenefitTypeRetirement, busConstant.BenefitProvisionEligibilityNormal, iobjPassInfo, ibusPersonAccount?.icdoPersonAccount?.start_date);

                if (lclbBenefitProvisionNormalEligibility.Count > 0)
                {
                    if (icdoBenefitApplication.plan_id != busConstant.PlanIdJobService)
                    {
                        ldecMemberAgeBasedOnNRD = (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age));
                    }
                    else
                    {
                        if (icdoBenefitApplication.idecTVSC >= lclbBenefitProvisionNormalEligibility[1].service)
                        {
                            ldecMemberAgeBasedOnNRD = lclbBenefitProvisionNormalEligibility[1].age;
                        }
                        else if (icdoBenefitApplication.idecTVSC >= lclbBenefitProvisionNormalEligibility[3].service)
                        {
                            ldecMemberAgeBasedOnNRD = lclbBenefitProvisionNormalEligibility[3].age;
                        }
                        else if (icdoBenefitApplication.idecTVSC >= lclbBenefitProvisionNormalEligibility[2].service)
                        {
                            ldecMemberAgeBasedOnNRD = lclbBenefitProvisionNormalEligibility[2].age;
                        }
                        else
                        {
                            ldecMemberAgeBasedOnNRD = lclbBenefitProvisionNormalEligibility[0].age;
                        }
                    }
                }
                return ldecMemberAgeBasedOnNRD;
            }
        }

        // SYSTEST - PIR - 1497
        //changed as per mail from sujatha dated - 3/4/2010
        public void LoadDeferralDate()
        {
            if (icdoBenefitApplication.deferral_date == DateTime.MinValue)
            {
                int lintMembersAgeInMonthsAsOnToday = 0;
                decimal ldecMemberAgeBasedOnToday = 0.00M;
                int lintMembersAgeInMonthsAsOnNRD = 0;
                decimal ldecMemberAgeBasedOnNRD = 0.00M;
                int lintMemberAgeMonthPart = 0;
                int lintMemberAgeYearPart = 0;
                DateTime ldteTempdate = DateTime.MinValue;

                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.ibusPerson == null)
                    ibusPersonAccount.LoadPerson();
                if (ibusPersonAccount.ibusPlan == null)
                    ibusPersonAccount.LoadPlan();

                //set termination date
                SetOrgIdAsLatestEmploymentOrgId();
                if (idtTerminationDate != DateTime.MinValue)
                    icdoBenefitApplication.termination_date = idtTerminationDate;

                //set member age as on todays date
                CalculateAge(ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth,
                    DateTime.Today, ref lintMembersAgeInMonthsAsOnToday, ref ldecMemberAgeBasedOnToday,
                   2, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);

                GetNormalRetirementDateBasedOnNormalEligibility(ibusPersonAccount.ibusPlan.icdoPlan.plan_code);

                if (icdoBenefitApplication.termination_date != DateTime.MinValue)
                {
                    ldteTempdate = ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth.AddMonths(847);
                    ldteTempdate = new DateTime(ldteTempdate.Year, ldteTempdate.Month, 01);

                    if (icdoBenefitApplication.idtNormalRetirementDate != DateTime.MinValue)
                        CalculateAge(ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth, icdoBenefitApplication.idtNormalRetirementDate,
                            ref lintMembersAgeInMonthsAsOnNRD, ref ldecMemberAgeBasedOnNRD,
                                                    2, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);

                    if (ibusPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB)
                    {
                        if (ldecMemberAgeBasedOnToday <= ldecMemberAgeBasedOnNRD)
                        {
                            icdoBenefitApplication.deferral_date = icdoBenefitApplication.idtNormalRetirementDate;
                        }
                        //this is commented as the member age will be calculated based on the Normal retirment date. PIR 1497 as discussed with Maik dated 11 2 2009
                        else
                        {
                            icdoBenefitApplication.deferral_date = ldteTempdate;
                        }
                        if (ldecMemberAgeBasedOnToday > 70.5M)
                            icdoBenefitApplication.deferral_date = icdoBenefitApplication.termination_date;
                    }
                    if (ibusPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC)
                    {
                        icdoBenefitApplication.deferral_date = ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth.AddYears(55);
                    }
                }
            }
        }

        public utlCollection<busError> iclbOldSoftErrors { get; set; }
        public utlCollection<busError> iclbCurrentSoftErrors { get; set; }
        //PIR - 1761 
        // to check if any new Soft error is raised by the system then check the suppress warning checkbox as checked 
        //else set as unchecked
        public void SetSuppressWarningFlagBasedOnErrors()
        {
            bool lblnIsNewErrorRaised = false;
            if (iclbOldSoftErrors != null)
            {
                iclbCurrentSoftErrors = new utlCollection<busError>();
                if (ibusSoftErrors.iclbError.Count > 0)
                {
                    foreach (busError lerr in ibusSoftErrors.iclbError)
                    {
                        iclbCurrentSoftErrors.Add(lerr);
                    }
                    foreach (busError lNewErr in iclbCurrentSoftErrors)
                    {
                        if (iclbOldSoftErrors.Where(lobj => lobj.message_id.Equals(lNewErr.message_id)).Count() == 0)
                        {
                            lblnIsNewErrorRaised = true;
                            break;
                        }
                    }
                    if (lblnIsNewErrorRaised)
                    {
                        if (icdoBenefitApplication.suppress_warnings_flag == busConstant.Flag_Yes)
                        {
                            icdoBenefitApplication.suppress_warnings_flag = busConstant.Flag_No;
                            icdoBenefitApplication.Update();
                        }
                    }
                }
            }
        }
        //PIR - 1761 
        public void LoadOldSoftErrors()
        {
            if (ibusSoftErrors.iclbError.IsNull())
                LoadErrors();
            if (ibusSoftErrors.iclbError.Count > 0)
            {
                iclbOldSoftErrors = new utlCollection<busError>();
                foreach (busError lerr in ibusSoftErrors.iclbError)
                {
                    iclbOldSoftErrors.Add(lerr);
                }
            }
        }


        # region UCS 60
        //UCS-060 31,32,33
        //reduced benefit option
        public bool IsValidReducedBenefitOption()
        {
            if (!string.IsNullOrEmpty(icdoBenefitApplication.reduced_benefit_flag) && icdoBenefitApplication.reduced_benefit_flag.Equals(busConstant.Flag_Yes))
            {
                if (icdoBenefitApplication.rtw_refund_election_value != null)
                {
                    if ((iblnIsRTWMember)
                        && (icdoBenefitApplication.rtw_refund_election_value.Equals(busConstant.Flag_No_Value.ToUpper())))
                    {
                        if (ibusRTWPayeeAccount.IsNull())
                            LoadRTWPayeeAccount();
                        if (ibusRTWPayeeAccount.ibusApplication.IsNull())
                            ibusRTWPayeeAccount.LoadApplication();
                        if (ibusRTWPayeeAccount.ibusApplication.icdoBenefitApplication.reduced_benefit_flag.IsNotNull())
                        {
                            if (ibusRTWPayeeAccount.ibusApplication.icdoBenefitApplication.reduced_benefit_flag.Equals(busConstant.Flag_Yes))
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        //UCS - 60- 20 
        public string istrIsActurialAllowed { get; set; }
        public string istrIsActurialNotAllowed { get; set; }
        public busPayeeAccount ibusRTWPayeeAccount { get; set; }

        public void LoadRTWPayeeAccount()
        {
            if (ibusRTWPayeeAccount.IsNull())
                ibusRTWPayeeAccount = new busPayeeAccount();
            ibusRTWPayeeAccount.FindPayeeAccount(icdoBenefitApplication.pre_rtw_payeeaccount_id);
        }

        public void SetActurialBenefitReductionAllowedFlag()
        {
            istrIsActurialAllowed = string.Empty;
            istrIsActurialNotAllowed = string.Empty;
            if (!String.IsNullOrEmpty(icdoBenefitApplication.benefit_option_value))
            {
                if (!icdoBenefitApplication.pre_rtw_payeeaccount_id.Equals(0))
                {
                    if (ibusRTWPayeeAccount.IsNull())
                        LoadRTWPayeeAccount();
                    if (ibusRTWPayeeAccount.icdoPayeeAccount.benefit_option_value.CompareTo(icdoBenefitApplication.benefit_option_value) == 0)
                    {
                        if (!icdoBenefitApplication.rtw_benefit_option_factor.Equals(0))
                            istrIsActurialAllowed = busConstant.Flag_Yes;
                    }
                    else
                    {
                        if (icdoBenefitApplication.rtw_benefit_option_factor.Equals(0))
                            istrIsActurialNotAllowed = busConstant.Flag_Yes;
                    }
                }
            }
        }

        #endregion

        //UAT PIR -1134
        //modifed - 23/06/2010 
        //populate the joint annuitant perslink id
        public void GetJointAnnuitantPerslinkId()
        {
            iblnIsJointAnnuitantIdRequired = false;
            if (icdoBenefitApplication.plan_id == busConstant.PlanIdDC ||
                icdoBenefitApplication.plan_id == busConstant.PlanIdDC2020 || //Systest PIR - 2630 //PIR 20232
                icdoBenefitApplication.plan_id == busConstant.PlanIdDC2025) //PIR 25920
            {
                if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionPeriodicPayment
                    && IsReducedRHICSelected())
                    iblnIsJointAnnuitantIdRequired = true;
            }
            else
            {               
                if (icdoBenefitApplication.rhic_option_value == busConstant.RHICOptionStandard
                    && icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionSingleLife)
                {
                    iblnIsJointAnnuitantIdRequired = false;
                }
                if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionSingleLife
                    && IsReducedRHICSelected())
                {
                    iblnIsJointAnnuitantIdRequired = true;
                }
                if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit)
                {
                    if ((IsReducedRHICSelected()
                        || icdoBenefitApplication.rhic_option_value == busConstant.RHICOptionStandard)
                        && (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)) // PIR 10486 - Check marital status
                        iblnIsJointAnnuitantIdRequired = true;
                }
                //if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionStraightLife
                //   && icdoBenefitApplication.rhic_option_value == busConstant.RHICOptionStandard)
                //{
                //    iblnIsJointAnnuitantIdRequired = true;
                //}
                if (icdoBenefitApplication.rhic_option_value == busConstant.RHICOptionStandard)
                {
                    //prod pir 7468 
                    //as per Maik, mail dated 09 july 2011, we need not check the joint survior for term certain benefit option
                    //if (IsJSBenefitOption
                    //  || (icdoBenefitApplication.IsTermCertainBenefitOption()
                    //   && ibusPerson.icdoPerson.marital_status_value == busConstant.PayeeAccountMaritalStatusMarried))
                    //    iblnIsJointAnnuitantIdRequired = true;

                    if (IsJSBenefitOption)
                        iblnIsJointAnnuitantIdRequired = true;
                }
                if (IsReducedRHICSelected() && icdoBenefitApplication.IsTermCertainBenefitOption())
                    iblnIsJointAnnuitantIdRequired = true;
            }
            /* this is commented. new logic written as per systest2621 PIR 
            //if (icdoBenefitApplication.plan_id != busConstant.PlanIdJobService)
            //{
            //    //1 - If the RHIC option is reduced option
            //    if (IsReducedRHICSelected())
            //    {
            //        if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit
            //            || icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionSingleLife)
            //        {
            //            lblnAllowedToSetJointPersonId = true;
            //        }
            //    }
            //    else if (icdoBenefitApplication.rhic_option_value == busConstant.RHICOptionStandard)
            //    {
            //        if (IsJSBenefitOption)
            //            lblnAllowedToSetJointPersonId = true;
            //    }
            //    if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit)
            //    {
            //        if (icdoBenefitApplication.plan_id == busConstant.PlanIdJudges
            //            || icdoBenefitApplication.plan_id == busConstant.PlanIdHP)
            //        {
            //            lblnAllowedToSetJointPersonId = true;
            //        }
            //    }
            //}

            ////if ((icdoBenefitApplication.plan_id != busConstant.PlanIdJobService)
            ////    && ((IsJSBenefitOption) || icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit || icdoBenefitApplication.benefit_option_value
            ////    == busConstant.BenefitOptionSingleLife || (IsReducedRHICSelected())))*/
            if (iblnIsJointAnnuitantIdRequired)
            {
                //Populate the ID when nothing is set.
                if (icdoBenefitApplication.joint_annuitant_perslink_id == 0)
                {
                    icdoBenefitApplication.joint_annuitant_perslink_id = GetSpouseContactPersonID();
                }
            }
            else
            {
                icdoBenefitApplication.joint_annuitant_perslink_id = 0;
                ibusJointAnniutantPerson = new busPerson();
            }
        }

        public int GetSpouseContactPersonID()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccountByApplication();
            if (ibusPersonAccount.ibusPerson.IsNull())
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.icolPersonContact.IsNull())
                ibusPersonAccount.ibusPerson.LoadContacts();

            var lenumSpouse = ibusPersonAccount.ibusPerson.icolPersonContact.Where(lobjcontact => lobjcontact.icdoPersonContact.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse
                    && lobjcontact.icdoPersonContact.status_value.ToString().Trim() == busConstant.PersonContactStatusActive);
            if (lenumSpouse.Count() > 0)
                if (lenumSpouse.FirstOrDefault().icdoPersonContact.contact_person_id > 0)
                {
                    return lenumSpouse.FirstOrDefault().icdoPersonContact.contact_person_id;
                }
            return 0;
        }


        //used for correspondence
        //since this is used in retirement disablity applications only
        //defaulting the benefit account type to Retirement
        public void LoadNormalEligibilityDetails(busDBCacheData abusDBCacheData = null)
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
				//PIR 14646 - New Benefit Tier Changes
            if (ibusPersonAccountRetirement.IsNull())
                LoadRetirementPersonAccount();
            string lstrBenefitTierValue = string.Empty;
            if (ibusPlan.IsNotNull() && ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMain) //PIR 26282
            {
                if (ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.MainBenefit2016Tier)
                    lstrBenefitTierValue = ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
                else
                    lstrBenefitTierValue = busConstant.MainBenefit1997Tier;
            }
            //PIR 26282
            else if (ibusPlan.IsNotNull() && ibusPlan.icdoPlan.plan_id == busConstant.PlanIdBCILawEnf)
            {
                if (ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.BCIBenefit2023Tier)
                    lstrBenefitTierValue = ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
                else
                    lstrBenefitTierValue = busConstant.BCIBenefit2011Tier;
            }
            //PIR 26544
            else if (ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value.IsNotNullOrEmpty())
                lstrBenefitTierValue = ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
            //load benefit Provision Eligibility details from DB cache
            if (abusDBCacheData == null)
            {
                abusDBCacheData = new busDBCacheData();
                abusDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(iobjPassInfo);
            }
            Collection<cdoBenefitProvisionEligibility> lobjBenefitProvisionEligibility = new Collection<cdoBenefitProvisionEligibility>();
            var lobjResult = from a in abusDBCacheData.idtbCachedBenefitProvisionEligibility.AsEnumerable()
                             where a.Field<int>("benefit_provision_id") == ibusPlan.icdoPlan.benefit_provision_id
                              && a.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement
                              && a.Field<string>("ELIGIBILITY_TYPE_VALUE") == busConstant.BenefitProvisionEligibilityNormal
                              && a.Field<string>("BENEFIT_TIER_VALUE") == (string.IsNullOrEmpty(lstrBenefitTierValue) ? null : lstrBenefitTierValue)
                             select a;

            lobjBenefitProvisionEligibility = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(lobjResult.AsDataTable());


            //get rule no based on plan
            if (lobjBenefitProvisionEligibility.Count > 0)
            {
                if ((icdoBenefitApplication.plan_id != busConstant.PlanIdNG)
                    && (icdoBenefitApplication.plan_id != busConstant.PlanIdJobService))
                {
                    istrNormalEligibilityRule = lobjBenefitProvisionEligibility[0].age_plus_service.ToString();
                }
                iintNormalEligibilityAge = (int)lobjBenefitProvisionEligibility[0].age;
            }

            decimal ldecTVSC = GetRoundedTVSC();
            //get current normal retirement date
            DateTime ldtNormalEligibilityDate = icdoBenefitApplication.idtNormalRetirementDate;

            // get normal retirement date based on Age and Rule
            if (icdoBenefitApplication.benefit_account_type_value != null)
            {
                idtNormalEligibilityDateByAge = GetNormalRetirementDateBasedOnNormalEligibility(icdoBenefitApplication.plan_id, ibusPlan.icdoPlan.plan_code,
                                                                ibusPlan.icdoPlan.benefit_provision_id, busConstant.ApplicationBenefitTypeRetirement,
                                                                ibusPerson.icdoPerson.date_of_birth, ldecTVSC,
                                                                2, iobjPassInfo, icdoBenefitApplication.termination_date,
                                                                ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                                false, 0, icdoBenefitApplication.retirement_date, abusDBCacheData, ibusPersonAccount); //PIR 14646

                idtNormalEligibilityDateByRule = GetNormalRetirementDateBasedOnNormalEligibility(icdoBenefitApplication.plan_id, ibusPlan.icdoPlan.plan_code,
                                                                ibusPlan.icdoPlan.benefit_provision_id, busConstant.ApplicationBenefitTypeRetirement,
                                                                ibusPerson.icdoPerson.date_of_birth, ldecTVSC,
                                                                1, iobjPassInfo, icdoBenefitApplication.termination_date,
                                                                ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                                false, 0, icdoBenefitApplication.retirement_date, abusDBCacheData, ibusPersonAccount); //PIR 14646
            }
            //get first day on month of normal eligibility date
            idtFirstDayOfMonthNormalEligibilityDate = new DateTime(idtNormalEligibilityDateByAge.Year, idtNormalEligibilityDateByAge.Month, 1);

            //get attained age as per noraml retirement date
            idecNormalEligibilityAttainedAge = Convert.ToDecimal(busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, ldtNormalEligibilityDate));
        }

        public string istrNormalEligibilityRule { get; set; }
        public int iintNormalEligibilityAge { get; set; }
        public DateTime idtNormalEligibilityDateByAge { get; set; }
        public DateTime idtNormalEligibilityDateByRule { get; set; }
        //UAT PIR - 1352
        public string istrdtNormalEligibilityDateByRule
        {
            get
            {
                return idtNormalEligibilityDateByRule.ToString(busConstant.DateFormatLongDate);
            }
        }
        public DateTime idtFirstDayOfMonthNormalEligibilityDate { get; set; }
        public string istrdFirstDayOfMonthNormalEligibilityDate //UAT PIR - 1352
        {
            get
            {
                return idtFirstDayOfMonthNormalEligibilityDate.ToString(busConstant.DateFormatLongDate);
            }
        }
        public decimal idecNormalEligibilityAttainedAge { get; set; }
        public string idtEligibleDateForRuleConversionMinus2Months //UAT PIR - 1352
        {
            get
            {
                DateTime ldtReturnDate = DateTime.MinValue;
                if (idtNormalEligibilityDateByRule != DateTime.MinValue)
                    ldtReturnDate = idtNormalEligibilityDateByRule.AddMonths(-1); //PIR 19145
                return ldtReturnDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        //UAT PIR - 1608
        public bool IsMemberDual()
        {
            decimal ldecTVSC = 0M;
            ibusPersonAccount.LoadTotalPSC(DateTime.MinValue,false);
            decimal ldecPSC = ibusPersonAccount.idecTotalPSC_Rounded;
            //TVSC needs to  be included irrespective of retirement date

            if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
                ldecTVSC = ibusPerson.GetTotalVSCForPerson(true, DateTime.MinValue, false,false);
            else
                ldecTVSC = ibusPerson.GetTotalVSCForPerson(false, DateTime.MinValue, false,false);
            if (ldecTVSC != ldecPSC)
                return true;

            return false;
        }

        //this field is used in deferred batch letter correspondence
        public decimal idecTaxableAmount { get; set; }

        //PIR - 1494
        //used for deferred batch letter
        public string istrRetirementLongDate
        {
            get
            {
                return icdoBenefitApplication.retirement_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        //PIR - 1494
        //used for deferred batch letter
        public string istrNormalRetirementLongDate
        {
            get
            {
                return icdoBenefitApplication.idtNormalRetirementDate.ToString(busConstant.DateFormatLongDate);
            }
        }
        //PIR - 1494
        //used for deferred batch letter
        public string istrRetirementDateMInus30DaysLongDate
        {
            get
            {
                return idtRetirementDateMInus30Days.ToString(busConstant.DateFormatLongDate);
            }
        }
        public bool iblnIsJointAnnuitantIdRequired { get; set; }

        //UAT PIR 1868
        public bool IsRetirementDatePastDate()
        {
            if (icdoBenefitApplication.received_date != DateTime.MinValue
                && icdoBenefitApplication.retirement_date != DateTime.MinValue)
            {
                return icdoBenefitApplication.received_date > icdoBenefitApplication.retirement_date.AddDays(60) ? true : false;
            }
            return false;

        }

        //UAT PIR - 925
        public DateTime idtGraduatedBenefitOptionDate
        {
            get
            { return Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.GraduatedBenefitOptionDateCodeValue, iobjPassInfo)); }
        }

        // PROD PIR ID 6389
        public bool IsTFFRServiceExistsTentativeOrApproved()
        {         
            if (ibusPersonAccount == null) LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null) ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbTffrTiaaService == null) ibusPersonAccount.ibusPerson.LoadTffrTiaaService();
            if (ibusPersonAccount.ibusPerson.iclbTffrTiaaService.Where(lobj =>
                lobj.icdoPersonTffrTiaaService.tffr_service_status_value == busConstant.PersonTFFRTIAAServiceStatusTentative ||
                lobj.icdoPersonTffrTiaaService.tffr_service_status_value == busConstant.PersonTFFRTIAAServiceStatusApproved).Any())
                return true;
            return false;          
        }

        // PIR 11236
        public bool IsSSLIUniformIncomeApplicable()
        {
            if (icdoBenefitApplication.retirement_date >= busConstant.SSLIUniformIncomeEffectiveRetirementDate)
            {
                if ((icdoBenefitApplication.ssli_age > 0) || (icdoBenefitApplication.uniform_income_flag == busConstant.Flag_Yes))
                {
                    if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
                        return true;
                    else
                        return false;
                }
            }
            else
                return true;

            return true;
        }

        // PIR 10329 - If Retirement Date is equal to the next payment cycle and person is past NRD, if the DNRO Request flag is not check throw an error
        // PIR 11646 - validation should only apply after termination date
        public bool IsDNROFlagRequired()
        {
            if (icdoBenefitApplication.dnro_flag == busConstant.Flag_No && iblnisVerifyBtnClicked) // PIR 14614
            {                
                if (icdoBenefitApplication.termination_date == DateTime.MinValue)
                    return false;
                else
                {
                    //PIR 12674 - Maik Mail dated March 9, 2015, - Retirement Date is NOT month following Termination Date but Retirement Date still has to be equal to or greater than the NRD
                    //if (icdoBenefitApplication.termination_date.AddMonths(1).GetFirstDayofCurrentMonth() == icdoBenefitApplication.retirement_date &&
                    //    ldtNextBenefitPaymentDate == icdoBenefitApplication.retirement_date &&
                    //    DateTime.Now > icdoBenefitApplication.idtNormalRetirementDate)
                    //{
                    //    return false;                                                                    
                    //}
                    //else if (icdoBenefitApplication.retirement_date == ldtNextBenefitPaymentDate &&
                    //    DateTime.Now > icdoBenefitApplication.idtNormalRetirementDate &&
                    //    DateTime.Now > icdoBenefitApplication.termination_date)
                    //{
                    //    return true;
                    //}


                    //PIR 18053 - DNRO cannot be selected for subsequent retirement calculation.
                    if (!(icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement &&
                        icdoBenefitApplication.retirement_date >= busConstant.RTWEffectiveDate && icdoBenefitApplication.pre_rtw_payeeaccount_id > 0))
                    {
                        //PIR 12674 - Maik Mail dated March 9, 2015, - Retirement Date is NOT month following Termination Date but Retirement Date still has to be equal to or greater than the NRD
                        if (icdoBenefitApplication.retirement_date == icdoBenefitApplication.termination_date.AddMonths(1).GetFirstDayofCurrentMonth() &&
                            icdoBenefitApplication.retirement_date >= icdoBenefitApplication.idtNormalRetirementDate.AddMonths(1) && CheckIsPersonVested()) //PIR 14811
                            return false;
                        else if (icdoBenefitApplication.retirement_date != icdoBenefitApplication.termination_date.AddMonths(1).GetFirstDayofCurrentMonth() &&
                            icdoBenefitApplication.retirement_date >= icdoBenefitApplication.idtNormalRetirementDate.AddMonths(1) && CheckIsPersonVested()) //PIR 14811
                            return true;
                    }
                }
            }
            return false;
        }

        //PIR  12674
        public bool IsEligibleForDNRO()
        {
            if (icdoBenefitApplication.dnro_flag == busConstant.Flag_Yes && iblnisVerifyBtnClicked) // PIR 14614
            {                
                if (icdoBenefitApplication.termination_date == DateTime.MinValue)
                    return true;
                else
                {
					//PIR 14811
                    if (icdoBenefitApplication.retirement_date == icdoBenefitApplication.termination_date.AddMonths(1).GetFirstDayofCurrentMonth() &&
                        icdoBenefitApplication.retirement_date >= icdoBenefitApplication.idtNormalRetirementDate.AddMonths(1) && CheckIsPersonVested())
                        return false;
                    else
                    {
                        if (icdoBenefitApplication.retirement_date < icdoBenefitApplication.idtNormalRetirementDate)
                            return false;
                    }
                }
            }
            return true;
        }

		//PIR 15421
        public bool IsDefferedBtnVisible()
        {
            if ((icdoBenefitApplication.benefit_application_id == 0 && icdoBenefitApplication.deferral_date != DateTime.MinValue)
                || (icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDeferred && icdoBenefitApplication.status_value != busConstant.ApplicationStatusProcessed && icdoBenefitApplication.benefit_application_id > 0))
            {
                return true;
            }
            return false;
        }
		//PIR 15831 - APP 7007 template to be associated with case management screen too
        public void LoadBenefitApplicationForCorrespondance()
        {
            if(ibusRetirementDisabilityApplication.IsNull()) ibusRetirementDisabilityApplication = new busRetirementDisabilityApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            ibusRetirementDisabilityApplication = this;
            ibusRetirementDisabilityApplication.icdoBenefitApplication = this.icdoBenefitApplication;
        }

        //PIR 18053 - Any benefit modifications (PLSO, DNRO, Graduated Benefit) would only apply to the original retirement and not the subsequent retirement calculation.
        public bool IsBenefitModifiedForSubsequentCal()
        {
            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (icdoBenefitApplication.retirement_date >= busConstant.RTWEffectiveDate)
                {
                    if (icdoBenefitApplication.pre_rtw_payeeaccount_id > 0)
                    {
                        if (icdoBenefitApplication.graduated_benefit_option_value != null ||
                            icdoBenefitApplication.dnro_flag ==  busConstant.Flag_Yes ||
                            icdoBenefitApplication.plso_requested_flag == busConstant.Flag_Yes)
                            return true;
                    }
                }
            }

            return false;
        }

        //PIR 18053 - Rule to check if Benefit option is changed for RTW. Benefit option should be same as the prev Payee Account.
        public bool IsBenefitOptionChangedForRTW()
        {
            if (ibusPreRTWPayeeAccount.IsNull())
                LoadPreRTWPayeeAccount();

            busRetirementDisabilityApplication lobjRetirementApplication = new busRetirementDisabilityApplication();

            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (icdoBenefitApplication.retirement_date >= busConstant.RTWEffectiveDate)
                {
                    if (lobjRetirementApplication.FindBenefitApplication(ibusPreRTWPayeeAccount.icdoPayeeAccount.application_id))
                    {
                        if (icdoBenefitApplication.benefit_option_value != lobjRetirementApplication.icdoBenefitApplication.benefit_option_value)
                            return true;
                    }
                }
            }
            return false;
        }

        //PIR 18053 - Rule to check if RHIC option is changed for RTW. RHIC option should be same as the prev Payee Account.
        public bool IsRHICOptionChangedForRTW()
        {
            if (ibusPreRTWPayeeAccount.IsNull())
                LoadPreRTWPayeeAccount();

            busRetirementDisabilityApplication lobjRetirementApplication = new busRetirementDisabilityApplication();

            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (icdoBenefitApplication.retirement_date >= busConstant.RTWEffectiveDate)
                {
                    if (lobjRetirementApplication.FindBenefitApplication(ibusPreRTWPayeeAccount.icdoPayeeAccount.application_id))
                    {
                        if (icdoBenefitApplication.rhic_option_value != lobjRetirementApplication.icdoBenefitApplication.rhic_option_value)
                            return true;
                    }
                }
            }
            return false;
        }
    }
}