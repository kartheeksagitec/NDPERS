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
using Sagitec.Bpm;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busRetirementBenefitCalculation : busBenefitCalculation
    {
        public bool iblnIsNewMode = false;
        public int iintPayeeAccountID{get;set;}
        private bool _iblnIsRaiseWarningFor180Days;
        public bool iblnIsRaiseWarningFor180Days
        {
            get { return _iblnIsRaiseWarningFor180Days; }
            set { _iblnIsRaiseWarningFor180Days = value; }
        }
        //PIR 25784 Capital PlanName
        public string istrCapitalPlanName
        {
            get
            {
                if (ibusPlan.IsNull())
                    LoadPlan();
                return ibusPlan.icdoPlan.plan_name.ToUpper();
            }
        }
        private bool _iblnIsBenefitOptionFactorNotRetrieved;
        public bool iblnIsDisabilityToNormalFromStart = false; //Backlog PIR 1869
        public bool iblnIsBenefitOptionFactorNotRetrieved
        {
            get { return _iblnIsBenefitOptionFactorNotRetrieved; }
            set { _iblnIsBenefitOptionFactorNotRetrieved = value; }
        }
        public decimal idecRTWPrevBenAmt
        {
            get
            {
                if(icdoBenefitCalculation.pre_rtw_payee_account_id > 0)
                {
                    busPayeeAccount lbusPayeeAccount = new busPayeeAccount() { icdoPayeeAccount = new cdoPayeeAccount() };
                    lbusPayeeAccount.FindPayeeAccount(icdoBenefitCalculation.pre_rtw_payee_account_id);
                    lbusPayeeAccount.LoadGrossBenefitAmount();
                    return lbusPayeeAccount.idecGrossBenfitAmount;
                }
                return 0.0M;
            }
        }
        public bool IsMemberhasWagesAndServiceCredit()
        {
            bool lblnIsPersonHasPSCandWages = false;
            if (iobjPassInfo.idictParams.ContainsKey("FormName") &&
                iobjPassInfo.idictParams["FormName"] != null &&
               ( iobjPassInfo.idictParams["FormName"].Equals("wfmRetirementBenefitCalculationFinalMaintenance")|| iobjPassInfo.idictParams["FormName"].Equals("wfmRetirementBenefitCalculationEstimateMaintenance"))
                && ibusPersonAccount.IsNotNull() && ibusPersonAccount.icdoPersonAccount.person_account_id > 0 && icdoBenefitCalculation.suppress_warnings_flag !=busConstant.Flag_Yes)
            {
                if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate)
                {
                    lblnIsPersonHasPSCandWages = ibusPersonAccount.LoadMemberWithWagesAndServiceCredit().Rows.Count > 0;
                }
            }
            return lblnIsPersonHasPSCandWages;
        }
        // PIR 9652
        public bool iblnIsMemberVested { get; set; }

        // UAT - PIR - 858
        public bool iblnIsRetirementDateFirstofMonth
        {
            get
            {
                if (icdoBenefitCalculation.retirement_date != DateTime.MinValue)
                {
                    if (icdoBenefitCalculation.retirement_date.Day != 01)
                        return false;
                }
                return true;
            }
        }

        //Load Benefit Options based on Plan and benefit type 
        //get collection of code values
        //loop thru collection and get select oly those values that are of same benefit type
        public Collection<cdoCodeValue> LoadBenefitOptionsBasedOnPlans()
        {
            return LoadBenefitOptionsBasedOnPlans(icdoBenefitCalculation.plan_id, icdoBenefitCalculation.benefit_account_type_value);
        }

        //check rule of 85
        //this method checks if member's age in months is greater than equal to (85 * 12 - TVSC)to satisfy the rule of 85
        private bool IsMemberSatisfyRuleOf85()
        {
            if ((iintMembersAgeInMonthsAsOnRetirementDate) >= ((85 * 12) - icdoBenefitCalculation.credited_vsc))
                return true;
            return false;
        }

        public bool IsAdjustedPSCOrVSCValid()
        {
            if ((icdoBenefitCalculation.adjusted_psc > 999) || (icdoBenefitCalculation.adjusted_psc < -999))
            {
                return true;
            }
            else if ((icdoBenefitCalculation.adjusted_tvsc > 999) || (icdoBenefitCalculation.adjusted_tvsc < -999))
            {
                return true;
            }
            return false;
        }

        //Check SSLI entered is valid
        //1. if it is less than the SSLI based on Plan
        //2. Check if it is less that members age.
        // if it fails any of the condition then returns error.
        public bool IsSSLIEnteredValid()
        {
            if (icdoBenefitCalculation.ssli_or_uniform_income_commencement_age != 0)
            {
                if ((icdoBenefitCalculation.ssli_or_uniform_income_commencement_age < GetSSLIAgeByPlan())
                    || (icdoBenefitCalculation.ssli_or_uniform_income_commencement_age < idecMemberAgeBasedOnRetirementDate))
                { return true; }
            }
            return false;
        }

        public bool IsJointAnnuitantExist()
        {
            bool IsJointAnnuitantExist = false;
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();
            foreach (busBenefitCalculationPayee lobjBenefitCalculationPayee in iclbBenefitCalculationPayee)
            {
                if (!lobjBenefitCalculationPayee.IsMember)
                {
                    IsJointAnnuitantExist = true;
                }
            }
            return IsJointAnnuitantExist;
        }

        public bool IsJointAnnuitantDeceased()
        {
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();
            foreach (busBenefitCalculationPayee lobjBenefitCalculationPayee in iclbBenefitCalculationPayee)
            {
                lobjBenefitCalculationPayee.ibusBenefitCalculation = this;
                if (!lobjBenefitCalculationPayee.IsMember)
                {
                    if (lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_person_id != 0)
                    {
                        lobjBenefitCalculationPayee.LoadPayee();
                        if (lobjBenefitCalculationPayee.ibusPayee.icdoPerson.date_of_death != DateTime.MinValue)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool iblnIsMemberRestrictedForRHIC { get; set; }
        //this value is used to set SSLI Age in the new mode as per Plan
        //the value of SSLI Age for plan is stored in the Code value table
        public int GetSSLIAgeByPlan()
        {
            string lstrPlanCode = ibusPlan.icdoPlan.plan_code;
            return Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(1906, lstrPlanCode, iobjPassInfo));
        }

        public bool IsESTMFromLOB { get; set; } // PIR 23878

        //Set SSLI age in the new mode only as per plan
        //when ever the SSLI age is set mark the uniform flag as YES
        //SSLI must be set for all plans except HP and DC
        public void SetSSLIAge()
        {
            if ((icdoBenefitCalculation.plan_id != busConstant.PlanIdHP)
                    &&
                     (icdoBenefitCalculation.plan_id != busConstant.PlanIdDC &&
                    icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2020 &&//PIR 20232
                    icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2025)) //PIR 25920
            {
                icdoBenefitCalculation.ssli_or_uniform_income_commencement_age = GetSSLIAgeByPlan();
            }
        }

        //Check SSLI based on the date of birth year part and the limits mentioned in the table 
        public bool IsSSLIAgeValid()
        {
            if (ibusMember == null)
                LoadMember();
            int lintDateOfBirthYearPart = ibusMember.icdoPerson.date_of_birth.Year;

            DataTable ldtbGetSSLIAgeLimit = SelectWithOperator<cdoCodeValue>(new string[3] { "Code_id", "data1", "data2" },
                                            new string[3] { "=", "<=", ">=" },
                                            new object[3] { busConstant.CodeValueForSSLILimit, lintDateOfBirthYearPart, lintDateOfBirthYearPart }, null);
            if (ldtbGetSSLIAgeLimit.Rows.Count > 0)
            {
                decimal ldecUpperLimitSSLIAge = Convert.ToDecimal(ldtbGetSSLIAgeLimit.Rows[0]["data3"]);
                decimal ldecLowerLimitSSLIAge = busConstant.SSLIAgeLowerLimit;
                if ((icdoBenefitCalculation.ssli_or_uniform_income_commencement_age > ldecUpperLimitSSLIAge)
                    || (icdoBenefitCalculation.ssli_or_uniform_income_commencement_age < ldecLowerLimitSSLIAge))
                { return false; }
            }
            return true;
        }

        //this method contains three sub methods to check the eligibity of the person for the following
        //For DB plan check this  
        // 1. Vesting
        // 2. Normal retirement
        // 3. Early retirement
        //For DC plan check the eligibility based on the no of employment days
        public bool IsPersonEligible()
        {
            SetBenefitSubType();
            /*************************************************************************
            //UAT PIR: 2058. To Calculate RHIC For PLan when Eligble for DC Starts
             **************************************************************************/
            if (ibusMember == null)
                LoadMember();

            if (icdoBenefitCalculation.plan_id == busConstant.PlanIdDC ||
               icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
               icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025) //PIR 25920
            {
                decimal ldecMemberAgeMontAndYear = 0.0M;
                string lstrBenefitSubType = string.Empty;

                CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.retirement_date, ref ldecMemberAgeMontAndYear, 4);

                IsMemberEligibleForDCPlan(ldecMemberAgeMontAndYear, ref lstrBenefitSubType);

                if ((lstrBenefitSubType.IsNullOrEmpty()))
                    return false;
                else
                    return true;
            }
            /***********************************************************************
            //UAT PIR: 2058. To Calculate RHIC For PLan when Eligble for DC Ends
             ***********************************************************************/
            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (IsPersonVested())
                    if ((icdoBenefitCalculation.benefit_account_sub_type_value != null) &&
                        ((icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal)
                        || (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
                        || (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO)
                        || (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDisabilitytoNormal))) //Backlog PIR 1869
                        return true;
            }
            if ((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                && (icdoBenefitCalculation.plan_id != busConstant.PlanIdDC &&
                icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2025)) //PIR 25920
            {
                if (IsMemberEligibilityForDisabilityRetirement())
                    return true;
            }
            return false;
        }

        /// This method is the same functionality as IsPersonEligible, It is used only in MAS for optimization.
        /// Cannot overload method or use nullable parameters, since the business XML couldnt refer this method.
        public bool CheckPersonEligible(busDBCacheData aobjDBCacheData)
        {
            SetBenefitSubType(aobjDBCacheData);
            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (IsPersonVested())
                {
                    iblnIsMemberVested = true; // PIR 9652
                    if ((icdoBenefitCalculation.benefit_account_sub_type_value != null) &&
                        ((icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal)
                        || (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
                        || (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO)))
                        return true;
                }
                else
                {
                    iblnIsMemberVested = false; // PIR 9652
                }
            }
            if ((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                && (icdoBenefitCalculation.plan_id != busConstant.PlanIdDC &&
                icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2020  && //PIR 20232
                icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2025)) //PIR 25920
            {
                if (IsMemberEligibilityForDisabilityRetirement())
                    return true;
            }
            return false;
        }

        //Check for vesting Eligibility 
        //eligibility changes as per the plan selected and based 
        //on the member s age based on the retirement entered on the screen
        //returns bool value after comparing values
        //and code values entered in the code value table
        public bool IsPersonVested()
        {
            if (icdoBenefitCalculation.plan_id == busConstant.PlanIdDC ||
               icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025) return true; //PIR 20232 //PIR 25920
            if (ibusPlan == null)
                LoadPlan();
            bool lblnIsPersonEligibleForVesting = CheckIsPersonVested(icdoBenefitCalculation.plan_id,
                                                        ibusPlan.icdoPlan.plan_code, ibusPlan.icdoPlan.benefit_provision_id,
                                                        icdoBenefitCalculation.benefit_account_type_value,
                                                        icdoBenefitCalculation.credited_vsc,
                                                        idecMemberAgeBasedOnRetirementDate,
                                                        icdoBenefitCalculation.retirement_date,
                                                        (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate ||
                                                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent  //PIR 19594
                                                        ) ? true : false,
                                                        icdoBenefitCalculation.termination_date,
                                                        ibusPersonAccount, iobjPassInfo);
            return lblnIsPersonEligibleForVesting;
        }

        public bool IsJointBenefitOptionSelected()
        {
            if (!String.IsNullOrEmpty(icdoBenefitCalculation.benefit_option_value))
            {
                if ((icdoBenefitCalculation.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value50JointSurvivor)
                    || (icdoBenefitCalculation.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value55JointSurvivor)
                    || (icdoBenefitCalculation.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value75JointSurvivor)
                    || (icdoBenefitCalculation.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value100JointSurvivor))
                { return true; }
            }
            return false;
        }

        // For DC only
        //member is not married and reduced option is selected.
        public bool IsMemberEligibleForReducedRHICForDC()
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusPlan == null)
                LoadPlan();
            if (IsReducedRHICSelected())
            {
                if (ibusPlan.IsDCRetirementPlan() || ibusPlan.IsHBRetirementPlan())
                {
                    if (ibusMember.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried)
                        return false;
                }
            }
            return true;
        }

        // Set benefit sub type based on the member's eligibility
        public void SetBenefitSubType(busDBCacheData aobjDBCacheData = null)
        {
            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement && !iblnIsDisabilityToNormalFromStart) //Backlog PIR 1869
            {
                if (ibusPlan == null)
                    LoadPlan();
                //this is set to null in order to check old value of benefit sub type is not retained for validation
                icdoBenefitCalculation.benefit_account_sub_type_value = null;
                GetNormalRetirementDateBasedOnNormalEligibility(ibusPlan.icdoPlan.plan_code, aobjDBCacheData);
                if (IsPersonEligibleforNormal())
                {
					//PIR 16612
                    if(icdoBenefitCalculation.is_rule_or_age_conversion > 0)
                    {
                        icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeDisabilitytoNormal;
                        return;
                    }
                    /*****************************************************************************************************************************************
                    //Prod PIR:4660  For handling Disability to Normal Since the Retirement Date is passed as a Parameter from payee account screen itself
                     ******************************************************************************************************************************************/
                    if ((icdoBenefitCalculation.normal_retirement_date < icdoBenefitCalculation.retirement_date) && (icdoBenefitCalculation.is_rule_or_age_conversion==0))
                    {
                        if (ibusBenefitApplication == null) //Backlog PIR 1869
                            LoadBenefitApplication();
                        if (icdoBenefitCalculation.benefit_account_type_value == busConstant.BenefitAppealTypeRetirement && 
                            ibusBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability && 
                            icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal)
                        {
                            icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeDisabilitytoNormal;
                            return;
                        }
                        /* UAT PIR: 1617. Unlike DNRO For other DB plans Job Service requires no gap between termination
                        //Date and retirement date. So a person Aged 66 can have a termination date say 03/31/2010 and retirement date as 04/01/2010
                         * and still DNRO will be calculated.
                         */
                        if (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService)
                        {
                            DateTime ldtDNRORetirementDate = icdoBenefitCalculation.normal_retirement_date;
                            DateTime ldtChangedEmpEnddate = icdoBenefitCalculation.termination_date.AddMonths(1);
                            // Maximum date of dtNormalRetirementDate and dtChangedEmpEnddate
                            DateTime ldtDNROComapredate = DateTime.Compare(ldtDNRORetirementDate, ldtChangedEmpEnddate) > 0 ? ldtDNRORetirementDate : ldtChangedEmpEnddate;

                            int lintDNROMonthsToIncrease = busGlobalFunctions.DateDiffByMonth(ldtDNROComapredate, icdoBenefitCalculation.retirement_date);
                            if (lintDNROMonthsToIncrease > 0)
                            {
                                lintDNROMonthsToIncrease = lintDNROMonthsToIncrease - 1;
                            }

                            if (lintDNROMonthsToIncrease > 0)
                            {
                                icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeDNRO;
                            }
                            else
                            {
                                icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeNormal;
                            }
                        }
                        else
                        {
                            icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeDNRO;
                        }
                    }
                    else
                    {
                        icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeNormal;
                    }

                    // PIR 10329 - set benefit subtype to DNRO if DNRO flag is true
                    if (ibusBenefitApplication == null)
                        LoadBenefitApplication();
                    if (ibusBenefitApplication.icdoBenefitApplication.dnro_flag == busConstant.Flag_Yes)
                    {
                        icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeDNRO;
                    }
                }
                else if (IsPersonEligibleForEarly())
                {
                    /*****************************************************************************************************************************************
                    //Prod PIR:4660  For handling Disability to Normal Since the Retirement Date is passed as a Parameter from payee account screen itself
                    This one is a very rare case...but still handled. in case of Disability to Normal. The case should be handled in payee Account screen itself.
                    ******************************************************************************************************************************************/
                    if (icdoBenefitCalculation.is_rule_or_age_conversion == 0)
                    {
                        icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeEarly;
                    }
                    else
                    {
                        icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeNormal;
                    }
                }
            }
        }

        //Check the eligibility for Disability 
        //For plan Main/LE/NG/HP/Judges check if the working days is more than 180 days(6 months)
        //For Plan job Service check the Total VSC is greater than 60 months
        public bool IsMemberEligibilityForDisabilityRetirement()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            // for Main/LE/NG/HP/Judges
            //6 months of employment
            if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdMain)
                || (icdoBenefitCalculation.plan_id == busConstant.PlanIdMain2020) //PIR 20232
                || (icdoBenefitCalculation.plan_id == busConstant.PlanIdLEWithoutPS)
                || (icdoBenefitCalculation.plan_id == busConstant.PlanIdJudges) || (icdoBenefitCalculation.plan_id == busConstant.PlanIdLE)
                || (icdoBenefitCalculation.plan_id == busConstant.PlanIdNG) || (icdoBenefitCalculation.plan_id == busConstant.PlanIdHP)
                || (icdoBenefitCalculation.plan_id == busConstant.PlanIdBCILawEnf) // pir 7943
                || (icdoBenefitCalculation.plan_id == busConstant.PlanIdStatePublicSafety)) //PIR 25729
            {
                ibusPersonAccount.LoadRetirementContributionAll();
                //no of working days should exceed 180 days               
                return IsNumberOfEmploymentDays180();
            }

            //for Job Service - 60 months of VSC
            if (icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService)
            {
                // added retirement_date as parameter done by Deepa
                // in order to get only the contribution those falls below this date
                if (ibusMember.GetTotalVSCForPerson(true, icdoBenefitCalculation.retirement_date) >= 60)
                    return true;
            }
            return false;
        }

        //Check for employment days 180
        public bool IsNumberOfEmploymentDays180()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            //This method excludes withdrawn
            //UAT PIR: 1383
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

        //checks for normal eligibility           
        //this method checks for eligibility based on the plan and rule of 85 or 80 
        //which ever is applicable as per plan defined in UCS
        public bool IsPersonEligibleforNormal()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();
			//Prod PIR: 4370. Extra VSC entered should be accounted inorder to find NRD Date.
            decimal ldecConsolidatedServiceCredit = 0.0M;
            ldecConsolidatedServiceCredit = GetConsolidatedExtraServiceCredit();
            if (icdoBenefitCalculation.fas_termination_date == DateTime.MinValue) SetDualFASTerminationDate(); //PROD PIR ID 5148
            bool lblnIsPersonEligibleForNormal = CheckIsPersonEligibleforNormal(icdoBenefitCalculation.plan_id,
                                                    ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id, icdoBenefitCalculation.benefit_account_type_value,
                                                    idecMemberAgeBasedOnRetirementDate, iintMembersAgeInMonthsAsOnRetirementDate,
                                                    icdoBenefitCalculation.credited_vsc, icdoBenefitCalculation.retirement_date, ibusPersonAccount, iobjPassInfo,
                                                    (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate ||
                                                    icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) ? true : false, //PIR 19594
                                                    icdoBenefitCalculation.fas_termination_date, ldecConsolidatedServiceCredit);
            return lblnIsPersonEligibleForNormal;
        }
        //UAT PIR:950 Method to set Indicator whether Tentative service is used for not for Estimate calculation added.	
        public void SetIsTentativeTFFRServiceusedFlag()
        {
            //icdoBenefitApplication.reduced_benefit_flag = busConstant.Flag_Yes;
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) //PIR 19594
            {
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                icdoBenefitCalculation.is_tentative_tffr_tiaa_used = busConstant.Flag_No;
                if (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService)
                {
                    decimal ldecTFFRService = 0.00M;
                    decimal ldecTIAAService = 0.00M;
                    decimal ldecTentativeTFFRService = 0.00M;
                    decimal ldecTentativeTIAAService = 0.00M;

                    ibusPersonAccount.LoadTFFRTIAAService(ref ldecTFFRService, ref ldecTIAAService, ref ldecTentativeTFFRService, ref ldecTentativeTIAAService);

                    if ((ldecTentativeTFFRService > 0) || (ldecTentativeTIAAService > 0))
                    {
                        icdoBenefitCalculation.is_tentative_tffr_tiaa_used = busConstant.Flag_Yes;
                    }
                }
            }
        }

        /// UAT PIR ID 929 - Need to add a Rule Indicator Flag in the Benefit Info tab of Calculations Screen.
        public void SetRuleIndicator()
        {
            decimal ldecMemberAgeAsofNRD = 0M;
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPlan.IsNull())
                ibusPersonAccount.LoadPlan();

            Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionNormalEligibility = new Collection<cdoBenefitProvisionEligibility>();
            lclbBenefitProvisionNormalEligibility = LoadEligibilityForPlan(icdoBenefitCalculation.plan_id,
                                                        ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id,
                                                        icdoBenefitCalculation.benefit_account_type_value,
                                                        busConstant.BenefitProvisionEligibilityNormal,
                                                        iobjPassInfo, ibusPersonAccount?.icdoPersonAccount?.start_date);
            //PIR 14646 - Setting rule indicator for new main benefit tier members
            busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
            lbusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
            if (icdoBenefitCalculation.plan_id == busConstant.PlanIdMain)
            {
                if (string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) || lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.MainBenefit1997Tier)
                    lclbBenefitProvisionNormalEligibility = lclbBenefitProvisionNormalEligibility.Where(i => i.benefit_tier_value == busConstant.MainBenefit1997Tier).ToList().ToCollection();
                else if (lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.MainBenefit2016Tier)
                    lclbBenefitProvisionNormalEligibility = lclbBenefitProvisionNormalEligibility.Where(i => i.benefit_tier_value == busConstant.MainBenefit2016Tier).ToList().ToCollection();
            }
            else if(icdoBenefitCalculation.plan_id == busConstant.PlanIdBCILawEnf) // PIR 26282
            {
                if (string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) || lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.BCIBenefit2011Tier)
                    lclbBenefitProvisionNormalEligibility = lclbBenefitProvisionNormalEligibility.Where(i => i.benefit_tier_value == busConstant.BCIBenefit2011Tier).ToList().ToCollection();
                else if (lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.BCIBenefit2023Tier)
                    lclbBenefitProvisionNormalEligibility = lclbBenefitProvisionNormalEligibility.Where(i => i.benefit_tier_value == busConstant.BCIBenefit2023Tier).ToList().ToCollection();
            }            
            //PIR 26544
            else if(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value.IsNotNullOrEmpty())
                lclbBenefitProvisionNormalEligibility = lclbBenefitProvisionNormalEligibility.Where(i => i.benefit_tier_value == lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value).ToList().ToCollection();

            if ((icdoBenefitCalculation.normal_retirement_date != DateTime.MinValue) &&
                (lclbBenefitProvisionNormalEligibility.Count > 0) &&
                ((icdoBenefitCalculation.plan_id == busConstant.PlanIdMain) 
                ||(icdoBenefitCalculation.plan_id  == busConstant.PlanIdMain2020) //PIR 20232
                || (icdoBenefitCalculation.plan_id == busConstant.PlanIdJudges) ||
                  (icdoBenefitCalculation.plan_id == busConstant.PlanIdLE) || (icdoBenefitCalculation.plan_id == busConstant.PlanIdLEWithoutPS) ||
                  (icdoBenefitCalculation.plan_id == busConstant.PlanIdHP) || (icdoBenefitCalculation.plan_id == busConstant.PlanIdStatePublicSafety) //PIR 25729
                  || (icdoBenefitCalculation.plan_id == busConstant.PlanIdNG) //PIR 25729
                  || (icdoBenefitCalculation.plan_id == busConstant.PlanIdBCILawEnf))) // pir 7943
            {
                CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.normal_retirement_date, ref ldecMemberAgeAsofNRD, 2);
                icdoBenefitCalculation.rule_indicator_value = null;
                //PIR 26282
                DateTime ldtProjectedRetirementDate = DateTime.MinValue;
                if (lclbBenefitProvisionNormalEligibility[0].age_while_employed_flag != busConstant.Flag_Yes && icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService)
                {
                    if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                    {
                        decimal ldecConsolidatedServiceCredit = 0.0M;
                        ldecConsolidatedServiceCredit = GetConsolidatedExtraServiceCredit();
                        if (ibusPersonAccount.idtProjectedRetirementDateByService == DateTime.MinValue)
                            ibusPersonAccount.CalculateProjectedRetirementDateByService(lclbBenefitProvisionNormalEligibility[0].service, 
                                                                                        (icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate) ||
                                                                                         icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimateSubsequent)), ldecConsolidatedServiceCredit);
                        ldtProjectedRetirementDate = ibusPersonAccount.idtProjectedRetirementDateByService;
                    }
                    if (lclbBenefitProvisionNormalEligibility[0].service <= 0)
                    {
                        DateTime ldtmLastContributionDate = ibusPersonAccount.LoadLastSalaryWithoutPersonAccount();
                        ldtProjectedRetirementDate = ldtmLastContributionDate.AddMonths(1);
                        ldtProjectedRetirementDate = new DateTime(ldtProjectedRetirementDate.Year, ldtProjectedRetirementDate.Month, 1);
                    }
                }

                if ((ldtProjectedRetirementDate != icdoBenefitCalculation.normal_retirement_date) 
                    &&  (ldecMemberAgeAsofNRD != (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age)) ||
                         ldecMemberAgeAsofNRD == (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].minimum_age))))
                {
                    if (icdoBenefitCalculation.retirement_date >= icdoBenefitCalculation.normal_retirement_date)
                    {
                        if (lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.MainBenefit2016Tier &&
                           (icdoBenefitCalculation.plan_id == busConstant.PlanIdMain))//PIR 20232
                        {
                            //PIR 14646 - Setting rule indicator for new benefit tier members of main
                            if (lclbBenefitProvisionNormalEligibility[0].age_plus_service == busConstant.AgePlusServiceRuleof90)
                                icdoBenefitCalculation.rule_indicator_value = busConstant.RuleIndicatorRuleof90;                            
                        }
                        else 
                        {
                            if (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age_plus_service) == busConstant.AgePlusServiceRuleof80)
                                icdoBenefitCalculation.rule_indicator_value = busConstant.RuleIndicatorRuleof80;
                            if (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age_plus_service) == busConstant.AgePlusServiceRuleof85)
                                icdoBenefitCalculation.rule_indicator_value = busConstant.RuleIndicatorRuleof85;
                            if (lclbBenefitProvisionNormalEligibility[0].age_plus_service == busConstant.AgePlusServiceRuleof90)
                                icdoBenefitCalculation.rule_indicator_value = busConstant.RuleIndicatorRuleof90;
                        }
                    }
                }
                icdoBenefitCalculation.rule_indicator_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2229, icdoBenefitCalculation.rule_indicator_value);
            }
        }

        //checks for early retirement eligibility
        //eligibility changes as per the plan selected 
        //returns bool value based on the member s age based on the retirement entered on the screen
        //and code values entered in the code value table
        public bool IsPersonEligibleForEarly()
        {
            string lstrEarlyReducedwaivedFlag = String.Empty;
            bool lblnIsPersoneligibleForEalry = CheckISPersonEligibleForEarly(icdoBenefitCalculation.plan_id,
                                                    ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id, icdoBenefitCalculation.benefit_account_type_value,
                                                    idecMemberAgeBasedOnRetirementDate, icdoBenefitCalculation.credited_vsc,
                                                    ref lstrEarlyReducedwaivedFlag, icdoBenefitCalculation.retirement_date, ibusPersonAccount, iobjPassInfo,
                                                    (icdoBenefitCalculation.calculation_type_value==busConstant.CalculationTypeEstimate ||
                                                    icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) ? true:false); //PIR 19594
            if (lstrEarlyReducedwaivedFlag == busConstant.Flag_Yes)
                iblnWaiveEarlyReduction = true;
            else
                iblnWaiveEarlyReduction = false;
            //iblnWaiveEarlyReduction = lstrEarlyReducedwaivedFlag;
            return lblnIsPersoneligibleForEalry;
        }

        //returns bool value after checking if the RHIC optional value is reduced
        public bool IsReducedRHICSelected()
        {
            if ((icdoBenefitCalculation.rhic_option_value == busConstant.ApplicationRHICReduced50)
              ||
              (icdoBenefitCalculation.rhic_option_value == busConstant.ApplicationRHICReduced100))
            {
                return true;
            }
            return false;
        }

        // check if the 100% j & S is selected for the reduced RHIC option
        public bool Is100PercentJAndSReducedRHICOptionSelected()
        {
            if ((icdoBenefitCalculation.benefit_option_value != null)
                          && (icdoBenefitCalculation.rhic_option_value != null))
            {
                if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdHP)
                       || (icdoBenefitCalculation.plan_id == busConstant.PlanIdJudges))
                {
                    if ((icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption100PercentJS)
                        && (IsReducedRHICSelected()))
                        return false;
                }
            }
            return true;
        }

        // this method will check if the member is allowed to have reduced RHIC option
        // 1. applicable for DB and disability
        //      RHIC reduced option selected and member is not married throw error       
        public bool IsMemberEligibleForReducedRHICForDBAndDisability()
        {
            if (ibusMember == null)
                LoadMember();

            if (ibusPlan == null)
                LoadPlan();

            if (IsReducedRHICSelected())
            {
                if (ibusMember.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried)
                {
                    if (ibusPlan.IsDBRetirementPlan())
                    {
                        if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                        {
                            return false;
                        }
                    }
                }
                if (ibusPlan.IsDCRetirementPlan() || ibusPlan.IsHBRetirementPlan())
                {
                    if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //Check if the retirement date is earlier than the first of the following the most recent month of employment
        public bool IsRetDateEarlierThanFirstOfFollowingEmploymentEndDate()
        {
            if (icdoBenefitCalculation.retirement_date != DateTime.MinValue)
            {
                DateTime ldtFirstOFFollowingEmploymentEndDate = new DateTime(icdoBenefitCalculation.termination_date.AddMonths(1).Year,
                                                                        icdoBenefitCalculation.termination_date.AddMonths(1).Month, 1); ;
                if (icdoBenefitCalculation.retirement_date < ldtFirstOFFollowingEmploymentEndDate)
                {
                    return true;
                }
            }
            return false;
        }

        //Validate the retirement date based on benefit sub type
        public void ValidateRetirementDateBasedOnBenefitSubType()
        {
            if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal)
            {
                if (GetNormalRetirementDateForDeferringAppl() < icdoBenefitCalculation.retirement_date)
                {
                    iintIsRetirementDateNotEqualToNormalOREalryEligibleDate = 1;
                }
            }
            if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
            {
                if (icdoBenefitCalculation.plan_id != busConstant.PlanIdDC &&
                    icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                    icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2025) //PIR 25920
                {
                    string lstrPlanCode = ibusPersonAccount.ibusPlan.icdoPlan.plan_code;

                    if (icdoBenefitCalculation.retirement_date < GetEarlyRetirementDateBasedOnEarlyRetirement(lstrPlanCode))
                    {
                        iintIsRetirementDateNotEqualToNormalOREalryEligibleDate = 2;
                    }
                }
            }
            if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO)
            {
                if (idecMemberAgeBasedOnRetirementDate > busConstant.MaxRetirementAgeAtNormalRetDate)
                {
                    iintIsRetirementDateNotEqualToNormalOREalryEligibleDate = 3;
                }
            }
        }

        //get Early retirment date based on the early retirement eligibility
        private DateTime GetEarlyRetirementDateBasedOnEarlyRetirement(string astrPlanCode)
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();

            DateTime ldtDateOnWhichMemberAttainsEarlyEligibility = GetEarlyRetirementDateBasedOnEarlyRetirement(
                                                                            icdoBenefitCalculation.plan_id,
                                                                            ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id,
                                                                            icdoBenefitCalculation.benefit_account_type_value,
                                                                            ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth,
                                                                            icdoBenefitCalculation.credited_psc,
                                                                            iobjPassInfo, ibusPersonAccount);
            return ldtDateOnWhichMemberAttainsEarlyEligibility;
        }

        private DateTime GetNormalRetirementDateForDeferringAppl()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusMember == null)
                LoadMember();
            DateTime ldtFirstOfMonthAfterMemberAttainsDefEligibility = DateTime.MinValue;
            DateTime ldtMemberAttainsDefEligibleAge = ibusMember.icdoPerson.date_of_birth.AddYears(73);//PIR 24248 - Eligible Age changed from 70.5 to 72.
            ldtFirstOfMonthAfterMemberAttainsDefEligibility =
                new DateTime(ldtMemberAttainsDefEligibleAge.AddMonths(1).Year, ldtMemberAttainsDefEligibleAge.AddMonths(1).Month, 1);
            return ldtFirstOfMonthAfterMemberAttainsDefEligibility;
        }
        public DateTime GetNormalRetirementDateBasedOnNormalEligibility(string astrPlanCode, busDBCacheData aobjDBCacheData = null)
        {
            if (ibusPlan == null)
                LoadPlan();

            return GetNormalRetirementDateBasedOnNormalEligibility(astrPlanCode, ibusPlan.icdoPlan.plan_id, ibusPlan.icdoPlan.benefit_provision_id, aobjDBCacheData);
        }
		//Prod PIR: 4370. Extra VSC entered should be accounted inorder to find NRD Date.
		//Method to Get Extra VSC.
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

            //UAT PIR: 950 Inclusion of Tentative and Approved TFFR Service for Arriving at NRD.
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) //PIR 19594
            {
                if (idecRemainingServiceCredit == 0.00M)
                    LoadRemainingServicePurchaseCredit();


                ldecConsolidatedExtraServiceCredit = idecRemainingServiceCredit + icdoBenefitCalculation.adjusted_tvsc;
                ldecConsolidatedExtraServiceCredit = ldecConsolidatedExtraServiceCredit + ldecTFFRService + ldecTIAAService + ldecTentativeTFFRService + ldecTentativeTIAAService;

            }
            else
            {
                ldecConsolidatedExtraServiceCredit = ldecTFFRService + ldecTIAAService;
            }
            return ldecConsolidatedExtraServiceCredit;
        }

        //get normal retirement date based on the normal retirement eligibility
        public DateTime GetNormalRetirementDateBasedOnNormalEligibility(string astrPlanCode, int aintPlanID, int aintBenefitProvisionid, busDBCacheData aobjDBCacheData = null)
        {
            if (ibusMember == null)
                LoadMember();
            //if (ibusPlan == null)
            //    LoadPlan();
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            decimal ldecConsolidatedServiceCredit = 0.0M;

            ldecConsolidatedServiceCredit = GetConsolidatedExtraServiceCredit();
            //UAT PIR: 1131 Dual FAS Date.
            SetDualFASTerminationDate();
            icdoBenefitCalculation.normal_retirement_date = GetNormalRetirementDateBasedOnNormalEligibility(aintPlanID, astrPlanCode,
                                                            aintBenefitProvisionid, icdoBenefitCalculation.benefit_account_type_value,
                                                            ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.credited_vsc,
                                                            icdoBenefitCalculation.is_rule_or_age_conversion, iobjPassInfo, icdoBenefitCalculation.fas_termination_date,
                                                            ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            (icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate) ||
                                                            icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimateSubsequent)),//PIR 19594
                                                            ldecConsolidatedServiceCredit, icdoBenefitCalculation.retirement_date, aobjDBCacheData, ibusPersonAccount); //PIR 14646
            return icdoBenefitCalculation.normal_retirement_date;
        }

        private void DeleteAndInsertBenefitCalculationPersonAccount()
        {
            DataTable ldtbResult = Select<cdoBenefitCalculationPersonAccount>(new string[1] { "benefit_calculation_id" },
                                                                    new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                    null, "benefit_calculation_person_account_id desc");
            foreach (DataRow ldtr in ldtbResult.Rows)
            {
                busBenefitCalculationPersonAccount lobjBenCalcPersonAccount = new busBenefitCalculationPersonAccount
                {
                    icdoBenefitCalculationPersonAccount = new cdoBenefitCalculationPersonAccount()
                };
                lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.LoadData(ldtr);
                // Delete all Person accounts except the current one.
                if (lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.person_account_id != ibusPersonAccount.icdoPersonAccount.person_account_id)
                {
                    lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.Delete();
                }
            }

            if (iclbBenefitCalculationPersonAccount.IsNotNull())
            {
                foreach (busBenefitCalculationPersonAccount lobjBenCalc in iclbBenefitCalculationPersonAccount)
                {
                    // Inserts the Benefit calc person account except the current account.
                    if (lobjBenCalc.icdoBenefitCalculationPersonAccount.person_account_id != ibusPersonAccount.icdoPersonAccount.person_account_id)
                    {
                        lobjBenCalc.icdoBenefitCalculationPersonAccount.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                        lobjBenCalc.icdoBenefitCalculationPersonAccount.Insert();
                    }
                }
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (aenmPageMode == utlPageMode.New)
            {
                iblnIsNewMode = true;
            }
            else
            {
                iblnIsNewMode = false;
            }
            //Reloading and Resetting the  Boolean Varaibles for Recalculating the objects.
            iblnBenefitMultiplierLoaded = false;
            iblnConsoldatedVSCLoaded = false;
            iblnConsolidatedPSCLoaded = false;

            if (icdoBenefitCalculation.retirement_date != DateTime.MinValue)
            {
                //Backlog PIR 10051 - reset variable value.
                idecRemainingServiceCredit = 0;

                CalculateConsolidatedVSC();
                CalculateConsolidatedPSC();
            }
            else
            {
                if (icdoBenefitCalculation.ienuObjectState == ObjectState.Insert)
                {
                    //this is to set retirement date as per eligibility.
                    //if user can over write this, then the user entered data has to to stored.
                    if (ibusPlan == null)
                        LoadPlan();
                    GetNormalRetirementDateBasedOnNormalEligibility(ibusPlan.icdoPlan.plan_code);
                }
            }
            CalculateMemberAge();
            //pir 8703
            if (icdoBenefitCalculation.retirement_date != DateTime.MinValue)
            {
                SetBenefitSubType();
                ValidateRetirementDateBasedOnBenefitSubType();
            }
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            if (icdoBenefitCalculation.suppress_warnings_flag == busConstant.Flag_Yes)
                icdoBenefitCalculation.suppress_warnings_by = iobjPassInfo.istrUserID;

            // Delete the Calculation Details if there is any change in the Application Details.
            DeleteBenefitCalculationDetails();

            // CALCULATE BENEFIT 
            CalculateRetirementBenefit();

            //PIR 24243
            if (IsPersonReached401aFlag() && icdoBenefitCalculation.overridden_final_average_salary > 0 &&
               (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal))
            {
                icdoBenefitCalculation.computed_final_average_salary = icdoBenefitCalculation.overridden_final_average_salary;
                icdoBenefitCalculation.fas_2010 = icdoBenefitCalculation.overridden_final_average_salary;
                icdoBenefitCalculation.fas_2019 = icdoBenefitCalculation.overridden_final_average_salary;
                icdoBenefitCalculation.fas_2020 = icdoBenefitCalculation.overridden_final_average_salary;
                icdoBenefitCalculation.calculation_final_average_salary = icdoBenefitCalculation.overridden_final_average_salary;
            }

            if (icdoBenefitCalculation.ienuObjectState == ObjectState.Insert)
            {
                icdoBenefitCalculation.Insert();
            }
            else
            {
                icdoBenefitCalculation.recalculated_by = iobjPassInfo.istrUserID; // UAT PIR ID 1280
            }

            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                foreach (busBenefitCalculationOtherDisBenefit lcdocalAppOtherDisBenefit in iclcBenCalcOtherDisBenefit)
                {
                    lcdocalAppOtherDisBenefit.icdoBenefitCalculationOtherDisBenefit.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    if ((lcdocalAppOtherDisBenefit.icdoBenefitCalculationOtherDisBenefit.benefit_calculation_id > 0) && (lcdocalAppOtherDisBenefit.icdoBenefitCalculationOtherDisBenefit.benefit_estimate_other_dis_benefit_id > 0))
                        lcdocalAppOtherDisBenefit.icdoBenefitCalculationOtherDisBenefit.Update();
                    else
                        lcdocalAppOtherDisBenefit.icdoBenefitCalculationOtherDisBenefit.Insert();
                }
            }

            // Delete And Insert Benefit calculation person account
            if (icdoBenefitCalculation.is_return_to_work_member)
            {
                DeleteAndInsertBenefitCalculationPersonAccount();
            }
            base.BeforePersistChanges();
        }

        public override int PersistChanges()
        {
            int lintResult = base.PersistChanges();
            if (iblnIsNewMode)
            {
                // Insert the Benefit Calculation Person Account only once in case of Insert.
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusBenefitCalculationPersonAccount == null)
                {
                    ibusBenefitCalculationPersonAccount = new busBenefitCalculationPersonAccount();
                    ibusBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount = new cdoBenefitCalculationPersonAccount();
                }
                ibusBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                ibusBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
                ibusBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.is_calculation_person_account_flag = busConstant.Flag_Yes;
                ibusBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag = busConstant.Flag_Yes;
                ibusBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.Insert();

                // Insert Benefit Payee only in New Mode
                if (iclbBenefitCalculationPayee == null)
                    LoadBenefitCalculationPayeeForNewMode();
                CreateBenefitCalculationPayeeDetails();

                // PIR ID 1190 - To Load Account and Family Relationship in Benefit Options once the Payee got saved.
                int lintMemberPayeeID = 0, lintSpousePayeeID = 0;
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    if (icdoBenefitCalculation.person_id == lobjPayee.icdoBenefitCalculationPayee.payee_person_id)
                        lintMemberPayeeID = lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id;
                    if (lobjPayee.icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipSpouse)
                        lintSpousePayeeID = lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id;
                }
                foreach (busBenefitCalculationOptions lobjOptions in iclbBenefitCalculationOptions)
                {
                    if ((lobjOptions.ibusBenefitCalculationPayee != null) &&
                        (lobjOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee != null))
                        if (lobjOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value == busConstant.AccountRelationshipMember)
                            lobjOptions.icdoBenefitCalculationOptions.benefit_calculation_payee_id = lintMemberPayeeID;
                    if (lobjOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipSpouse)
                        lobjOptions.icdoBenefitCalculationOptions.benefit_calculation_payee_id = lintSpousePayeeID;
                }

                // Make the Action Status initially to Pending Approval.
                icdoBenefitCalculation.action_status_value = busConstant.BenefitActionStatusPending;
            }
            return lintResult;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            CreateBenefitCalculationDetails();
            EvaluateInitialLoadRules();
            if ((icdoBenefitCalculation.ssli_or_uniform_income_commencement_age > 0) && (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
            {
                icdoBenefitCalculation.ssli_effective_date = GetSSLIEffectivedate(ibusMember.icdoPerson.date_of_birth,
                            icdoBenefitCalculation.ssli_or_uniform_income_commencement_age);
            }

            //PIR - 1394
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent)
                UpdateApplicationStatusIfEligibilityChanges();

            if (ibusBaseActivityInstance != null)
            {
                busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;

                if (lbusActivityInstance.ibusBpmProcessInstance == null)
                {
                    lbusActivityInstance.LoadBpmProcessInstance();
                }
                int lintProcessId = busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name);
                if (lintProcessId == busConstant.Map_Recalculate_Pension_and_RHIC_Benefit
                    || lintProcessId == busConstant.Map_Process_Disability_Application
                    || lintProcessId == busConstant.Map_Process_Disability_to_Normal_Rule_Conversion
                    || lintProcessId == busConstant.Map_Process_Disability_to_Normal_Age_Conversion
                    || lintProcessId == busConstant.Map_MSS_Process_Disability_Application
                    || lintProcessId == busConstant.Map_MSS_Process_DB_Retirement_Application
                    || lintProcessId == busConstant.Map_MSS_Process_DC_Retirement_Application
                    || lintProcessId == busConstant.Map_MSS_Process_Deferred_Retirement_Application
                    || lintProcessId == busConstant.Map_MSS_Process_Job_Service_Application)
                {
                    lbusActivityInstance.UpdateParameter("calculation_reference_id", icdoBenefitCalculation.benefit_calculation_id.ToString());
                }
            }
        }

        public void GetRetirementCalculationByDisabilityPayeeAccount(busBenefitApplication aobjBenefitApplication, int aintDisabilityPayeeAccountID, int aintPlanID)
        {
            icdoBenefitCalculation.disability_payee_account_id = aintDisabilityPayeeAccountID;
            if (ibusDisabilityPayeeAccount == null)
                LoadDisabilityPayeeAccount();

            icdoBenefitCalculation.benefit_application_id = 0;

            //PIR 26074
            busRetirementDisabilityApplication lobjRetirementApplication = new busRetirementDisabilityApplication();
            lobjRetirementApplication.FindBenefitApplication(ibusDisabilityPayeeAccount.icdoPayeeAccount.application_id);
            
            GetCalculationByApplication(lobjRetirementApplication);

            //Commented code under PIR 26074 - To load all values from Application

            //if (ibusDisabilityPayeeAccount.ibusBenefitCalculaton == null)
            //    ibusDisabilityPayeeAccount.LoadBenefitCalculation();

            //if (ibusDisabilityPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation.benefit_calculation_id != 0)
            //{
            //    icdoBenefitCalculation = ibusDisabilityPayeeAccount.ibusBenefitCalculaton.icdoBenefitCalculation;
            //}
            //else
            //{
            //    icdoBenefitCalculation.person_id = aobjBenefitApplication.icdoBenefitApplication.member_person_id;
            //    icdoBenefitCalculation.termination_date = aobjBenefitApplication.icdoBenefitApplication.termination_date;
            //    icdoBenefitCalculation.benefit_option_value = ibusDisabilityPayeeAccount.icdoPayeeAccount.benefit_option_value;
            //    icdoBenefitCalculation.benefit_option_description =
            //        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2216, icdoBenefitCalculation.benefit_option_value);
            //    icdoBenefitCalculation.rhic_option_value = aobjBenefitApplication.icdoBenefitApplication.rhic_option_value;
            //    icdoBenefitCalculation.rhic_option_description =
            //        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1905, icdoBenefitCalculation.rhic_option_value);
            //    icdoBenefitCalculation.benefit_application_id = ibusDisabilityPayeeAccount.icdoPayeeAccount.application_id;
            //}

            if (ibusDisabilityPayeeAccount.ibusPayee == null)
                ibusDisabilityPayeeAccount.LoadPayee();
            if (ibusMember == null)
                ibusMember = ibusDisabilityPayeeAccount.ibusPayee;

            icdoBenefitCalculation.plan_id = aintPlanID;
            if (ibusPlan == null)
                LoadPlan();
            icdoBenefitCalculation.benefit_calculation_id = 0;
            icdoBenefitCalculation.disability_payee_account_id = aintDisabilityPayeeAccountID;
            icdoBenefitCalculation.ienuObjectState = ObjectState.Insert;

            icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
            icdoBenefitCalculation.benefit_account_type_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, icdoBenefitCalculation.benefit_account_type_value);

            icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
            icdoBenefitCalculation.calculation_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeFinal);

            icdoBenefitCalculation.status_value = busConstant.StatusReview;
            icdoBenefitCalculation.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2301, icdoBenefitCalculation.status_value);

            icdoBenefitCalculation.action_status_value = busConstant.BenefitActionStatusPending;
            icdoBenefitCalculation.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, icdoBenefitCalculation.action_status_value);

            icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
            icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeNormal;

            if (!iblnConsolidatedPSCLoaded)
                CalculateConsolidatedPSC();
            if (!iblnConsoldatedVSCLoaded)
                CalculateConsolidatedVSC();

            // Need to set Is_rule_or_age_conversion_flag before calculating Normal retirement Date
            if (ibusDisabilityPayeeAccount.icdoPayeeAccount.workflow_age_conversion_flag == busConstant.Flag_Yes)
                icdoBenefitCalculation.is_rule_or_age_conversion = busConstant.ConvertedToNormalByAge;
            else
                icdoBenefitCalculation.is_rule_or_age_conversion = busConstant.ConvertedToNormalByRule;

            icdoBenefitCalculation.normal_retirement_date = GetNormalRetirementDateBasedOnNormalEligibility(ibusPlan.icdoPlan.plan_code);
            /*****************************************************************************************************************************************
            //Prod PIR:4660  For handling Disability to Normal Since the Retirement Date is passed as a Parameter from payee account screen itself Starts here
            ******************************************************************************************************************************************/
            if (ibusDisabilityPayeeAccount.icdoPayeeAccount.disa_normal_effective_date != DateTime.MinValue)
            {                
                icdoBenefitCalculation.retirement_date = ibusDisabilityPayeeAccount.icdoPayeeAccount.disa_normal_effective_date;
            }
            else
            {                
                icdoBenefitCalculation.retirement_date = icdoBenefitCalculation.normal_retirement_date;
            }
            /*****************************************************************************************************************************************
            //Prod PIR:4660  For handling Disability to Normal Since the Retirement Date is passed as a Parameter from payee account screen itself Ends here
            ******************************************************************************************************************************************/
            // UAT PIR ID 1337 && 1350
            icdoBenefitCalculation.annuitant_id = aobjBenefitApplication.icdoBenefitApplication.joint_annuitant_perslink_id;

            // UAT PIR ID 1349
            ibusDisabilityPayeeAccount.LoadGrossBenefitAmount();
            icdoBenefitCalculation.disability_gross_benefit_amount = ibusDisabilityPayeeAccount.idecGrossBenfitAmount;
        }

        public void GetCalculationByApplication(busRetirementDisabilityApplication aobjRetirementBenefitApplication)
        {
            icdoBenefitCalculation.person_id = aobjRetirementBenefitApplication.icdoBenefitApplication.member_person_id;
            if (ibusMember == null)
                LoadMember();

            icdoBenefitCalculation.plan_id = aobjRetirementBenefitApplication.icdoBenefitApplication.plan_id;
            if (ibusPlan == null)
                LoadPlan();

            icdoBenefitCalculation.normal_retirement_date =
                aobjRetirementBenefitApplication.GetNormalRetirementDateBasedOnNormalEligibility(ibusPlan.icdoPlan.plan_code);
            icdoBenefitCalculation.benefit_account_type_value = aobjRetirementBenefitApplication.icdoBenefitApplication.benefit_account_type_value;
            icdoBenefitCalculation.benefit_account_type_description = aobjRetirementBenefitApplication.icdoBenefitApplication.benefit_account_type_description;
            //calculation type setting code is removed and added in srvBenefitCalculation
            icdoBenefitCalculation.retirement_date = aobjRetirementBenefitApplication.icdoBenefitApplication.retirement_date;
            icdoBenefitCalculation.benefit_application_id = aobjRetirementBenefitApplication.icdoBenefitApplication.benefit_application_id;
            icdoBenefitCalculation.termination_date = aobjRetirementBenefitApplication.icdoBenefitApplication.termination_date;
            icdoBenefitCalculation.plso_requested_flag = aobjRetirementBenefitApplication.icdoBenefitApplication.plso_requested_flag;
            icdoBenefitCalculation.benefit_option_value = aobjRetirementBenefitApplication.icdoBenefitApplication.benefit_option_value;
            icdoBenefitCalculation.benefit_option_description = aobjRetirementBenefitApplication.icdoBenefitApplication.benefit_option_description;
            icdoBenefitCalculation.ssli_or_uniform_income_commencement_age = aobjRetirementBenefitApplication.icdoBenefitApplication.ssli_age;
            icdoBenefitCalculation.ssli_effective_date = aobjRetirementBenefitApplication.icdoBenefitApplication.ssli_effective_date;
            icdoBenefitCalculation.uniform_income_or_ssli_flag = aobjRetirementBenefitApplication.icdoBenefitApplication.uniform_income_flag;
            icdoBenefitCalculation.estimated_ssli_benefit_amount = aobjRetirementBenefitApplication.icdoBenefitApplication.estimated_ssli_benefit_amount;
            icdoBenefitCalculation.reduced_benefit_flag = aobjRetirementBenefitApplication.icdoBenefitApplication.reduced_benefit_flag;
            icdoBenefitCalculation.rhic_option_value = aobjRetirementBenefitApplication.icdoBenefitApplication.rhic_option_value;
            icdoBenefitCalculation.rhic_option_description = aobjRetirementBenefitApplication.icdoBenefitApplication.rhic_option_description;
            icdoBenefitCalculation.paid_up_annuity_amount = aobjRetirementBenefitApplication.icdoBenefitApplication.paid_up_annuity_amount;
            icdoBenefitCalculation.annuitant_id = aobjRetirementBenefitApplication.icdoBenefitApplication.joint_annuitant_perslink_id;
            iintRetirementOrgId = aobjRetirementBenefitApplication.icdoBenefitApplication.retirement_org_id;
            icdoBenefitCalculation.graduated_benefit_option_value = aobjRetirementBenefitApplication.icdoBenefitApplication.graduated_benefit_option_value; // UAT PIR ID 925
            icdoBenefitCalculation.graduated_benefit_option_description = aobjRetirementBenefitApplication.icdoBenefitApplication.graduated_benefit_option_description;

            // UCS-060 Return to Work Member
            icdoBenefitCalculation.pre_rtw_payee_account_id = aobjRetirementBenefitApplication.icdoBenefitApplication.pre_rtw_payeeaccount_id;
            icdoBenefitCalculation.actuarial_benefit_reduction = aobjRetirementBenefitApplication.icdoBenefitApplication.rtw_benefit_option_factor;
            icdoBenefitCalculation.rtw_refund_election_value = aobjRetirementBenefitApplication.icdoBenefitApplication.rtw_refund_election_value;
            if (icdoBenefitCalculation.rtw_refund_election_value.IsNullOrEmpty())
            {
                icdoBenefitCalculation.rtw_refund_election_value = busConstant.Flag_No_Value.ToUpper();
            }
            icdoBenefitCalculation.rtw_refund_election_description = aobjRetirementBenefitApplication.icdoBenefitApplication.rtw_refund_election_description;

            // Get RTW Benefit Application Person Account <=> RTW Benefit Calculation Person Account
            if (aobjRetirementBenefitApplication.iclbBenefitApplicationPersonAccounts == null)
                aobjRetirementBenefitApplication.LoadBenefitApplicationPersonAccount();
            if (iclbBenefitCalculationPersonAccount == null)
                iclbBenefitCalculationPersonAccount = new Collection<busBenefitCalculationPersonAccount>();
            foreach (busBenefitApplicationPersonAccount lobjBenAppPersonAccount in aobjRetirementBenefitApplication.iclbBenefitApplicationPersonAccounts)
            {
                busBenefitCalculationPersonAccount lobjBenCalcPersonAccount = new busBenefitCalculationPersonAccount
                {
                    icdoBenefitCalculationPersonAccount = new cdoBenefitCalculationPersonAccount()
                };
                if (lobjBenAppPersonAccount.icdoBenefitApplicationPersonAccount.is_application_person_account_flag != busConstant.Flag_Yes)
                {
                    lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.payee_account_id =
                                                            lobjBenAppPersonAccount.icdoBenefitApplicationPersonAccount.payee_account_id;
                    lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.person_account_id =
                                                            lobjBenAppPersonAccount.icdoBenefitApplicationPersonAccount.person_account_id;
                    lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_calculation_person_account_flag =
                                                            lobjBenAppPersonAccount.icdoBenefitApplicationPersonAccount.is_application_person_account_flag;
                    lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag =
                                                            lobjBenAppPersonAccount.icdoBenefitApplicationPersonAccount.is_person_account_selected_flag;

                    if (lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.payee_account_id > 0)
                    {
                        lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.istrUse = busConstant.Flag_Yes_Value;
                    }
                    else
                    {
                        lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.istrUse = busConstant.Flag_No_Value;
                    }
                    lobjBenCalcPersonAccount.LoadPersonAccount();
                    lobjBenCalcPersonAccount.ibusPersonAccount.LoadPlan();
                    if (lobjBenCalcPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value !=
                        busConstant.PlanParticipationStatusRetirementWithDrawn) // PIR ID 1701
                        iclbBenefitCalculationPersonAccount.Add(lobjBenCalcPersonAccount);
                }
            }

            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonAccountRetirement.IsNull())
                ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
            ibusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
            ibusPersonAccount.ibusPersonAccountRetirement.LoadTotalVSC();
            if (ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.Total_VSC >= 24)
                icdoBenefitCalculation.is_rtw_less_than_2years_flag = busConstant.Flag_No;
            else
                icdoBenefitCalculation.is_rtw_less_than_2years_flag = busConstant.Flag_Yes;
        }

        // Final Screen Approve Button Click Event handled
        public ArrayList btnApprove_Clicked()
        {
            ArrayList larrErrors = new ArrayList();
            //PIR - 1954
            //is disability application then
            //check if this person is having a receiving payee account for Early retirement application           
            if (IsCancelledPayeeAccountNotExits())
            {
                utlError lobjError = new utlError();
                lobjError = AddError(2100, "");
                larrErrors.Add(lobjError);
            }
            if (icdoBenefitCalculation.is_rule_or_age_conversion == busConstant.ConvertedToNormalByRule)
            {
                if (icdoBenefitCalculation.final_monthly_benefit < icdoBenefitCalculation.disability_gross_benefit_amount)
                {
                    utlError lobjError = new utlError();
                    lobjError = AddError(2080, "");
                    larrErrors.Add(lobjError);
                }
            }
            if (icdoBenefitCalculation.is_return_to_work_member)
            {
                // BR-060-24 - No RTW calculation can be approved until the Pre-RTW Payee account status(es) is/are Suspended.
                if (ibuspre_RTW_Payee_Account == null)
                    LoadPreRTWPayeeAccount();
                if (ibuspre_RTW_Payee_Account.ibusPayeeAccountActiveStatus == null)
                    ibuspre_RTW_Payee_Account.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                // UAT PIR ID 1210 - Other than Payee Account Status Suspended or Payment Complete
                if (!(ibuspre_RTW_Payee_Account.ibusPayeeAccountActiveStatus.IsStatusSuspended() ||
                    (ibuspre_RTW_Payee_Account.ibusPayeeAccountActiveStatus.IsStatusCompleted())))
                {
                    utlError lobjError = new utlError();
                    lobjError = AddError(2096, "");
                    larrErrors.Add(lobjError);
                }
            }
            //PIRs 15600
            //Skip new plans [Main2020 and DC2020] for RHIC - PIR 20232
            if (icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2020 && icdoBenefitCalculation.plan_id != busConstant.PlanIdMain2020 && icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2025) // PIR 20232
            {
                if (icdoBenefitCalculation.rhic_effective_date == DateTime.MinValue)
                {
                    utlError lobjError = new utlError();
                    lobjError = AddError(10279, "");
                    larrErrors.Add(lobjError);
                }
            }
            //PIR 16284/17801 //PIR 19010 & 18993
            if (ibusBenefitApplication == null)
                LoadBenefitApplication();
            if (ibusBenefitApplication.DoesPersonHaveOpenContriEmpDtlByPlan())
            {
                utlError lobjError = new utlError();
                lobjError = AddError(7505, "");
                larrErrors.Add(lobjError);
            }
            this.ValidateHardErrors(utlPageMode.All);
            if (larrErrors.Count == 0)
            {

                if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdDC && idecMemberRHICAmount == 0M) ||
                         ((icdoBenefitCalculation.plan_id != busConstant.PlanIdDC && icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2020 && icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2025) //PIR 25920
                         && (icdoBenefitCalculation.final_monthly_benefit == 0M))) //PIR 20232
                {
                    // Final Monthly Benefit or Member RHIC amount is zero.
                    utlError lobjError = new utlError();
                    lobjError = AddError(2059, "");
                    larrErrors.Add(lobjError);
                }
                if (((icdoBenefitCalculation.plan_id == busConstant.PlanIdDC) && (idecMemberRHICAmount > 0M)) ||
                    ((icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020) && (idecMemberRHICAmount >= 0M)) ||
                    ((icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025) && (idecMemberRHICAmount >= 0M)) || //PIR 25920
                    ((icdoBenefitCalculation.plan_id != busConstant.PlanIdDC && icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2020 && icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2025) && (icdoBenefitCalculation.final_monthly_benefit > 0M))) //PIR 20232
                {
                    int lintBenefitAccountID = 0;
                    int lintPayeeAccountID = 0;
                    int lintRTWRefundBenefitAccountID = 0;
                    int lintRTWRefundPayeeAccountID = 0;

                    // Fetch Variables Required for Benefit updation or Insertion
                    decimal ldecPreTaxContribution = 0M;
                    decimal ldecPostTaxContribution = 0M;
                    decimal ldecLTDAccountBalance = 0M;
                    decimal ldecStartingTaxableAmount = 0.0M;
                    decimal ldecStartingNonTaxableAmount = 0.0M;
                    decimal ldecAccountOwnerStartingTaxableAmount = 0.0M;
                    decimal ldecAccountOwnerStartingNonTaxableAmount = 0.0M;
                    decimal ldecAccountOwnerLTDAccountBalance = 0M;
                    decimal ldecExclusionRatio = 0M;

                    decimal ldecOptionFactor = GetMemberOptionFactor();
                    decimal ldecRHICOptionFactor = 0M;
                    if ((iclbBenefitRHICOption.IsNotNull()) && (iclbBenefitRHICOption.Count > 0))
                        ldecRHICOptionFactor = iclbBenefitRHICOption[0].icdoBenefitRhicOption.option_factor;

                    DateTime ldteBenefitBeginDate = new DateTime();
                    DateTime ldteNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
                    DateTime ldteTerminationDateLastDayofMonth = icdoBenefitCalculation.termination_date.GetLastDayofMonth(); //PROD PIR ID 4851

                    if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
                        icdoBenefitCalculation.is_rtw_member_recalculate = true;

                    if (ibusBenefitProvisionBenefitOption == null)
                        LoadBenefitProvisionBenefitOption();

                    if (ibusPersonAccount == null)
                        LoadPersonAccount();

                    if (icdoBenefitCalculation.is_return_to_work_member && !icdoBenefitCalculation.is_rtw_member_subsequent_retirement)
                    {
                        if (iclbBenefitCalculationPersonAccount.IsNull())
                            LoadBenefitCalculationPersonAccount();
                        foreach (busBenefitCalculationPersonAccount lobjBenCalcPersonAccount in iclbBenefitCalculationPersonAccount)
                        {
                            if (lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag == busConstant.Flag_Yes)
                            {
                                if (lobjBenCalcPersonAccount.ibusPersonAccount.IsNull())
                                    lobjBenCalcPersonAccount.LoadPersonAccount();
                                if (lobjBenCalcPersonAccount.ibusPersonAccount.ibusPersonAccountRetirement.IsNull())
                                {
                                    lobjBenCalcPersonAccount.ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
                                    lobjBenCalcPersonAccount.ibusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(
                                                        lobjBenCalcPersonAccount.ibusPersonAccount.icdoPersonAccount.person_account_id);
                                }
                                lobjBenCalcPersonAccount.ibusPersonAccount.ibusPersonAccountRetirement.LoadLTDSummaryForCalculation(
                                                        ldteTerminationDateLastDayofMonth, icdoBenefitCalculation.benefit_account_type_value);
                                lobjBenCalcPersonAccount.ibusPayeeAccount.LoadPaymentDetails();
                                ldecLTDAccountBalance += (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.Member_Account_Balance_ltd -
                                                        lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidgrossamount);
                                ldecPreTaxContribution += (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd -
                                                        lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidtaxableamount);
                                ldecPostTaxContribution += (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd -
                                                        lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidnontaxableamount);

                            }
                        }
                    }
                    if (ibusPersonAccount.ibusPersonAccountRetirement == null)
                    {
                        ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
                        ibusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
                    }

                    ibusPersonAccount.ibusPersonAccountRetirement.LoadLTDSummaryForCalculation(ldteTerminationDateLastDayofMonth, icdoBenefitCalculation.benefit_account_type_value);
                    ldecAccountOwnerStartingTaxableAmount = (ibusPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd < 0) ? 0M :
                                                                ibusPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd;
                    ldecAccountOwnerStartingNonTaxableAmount = (ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd < 0) ? 0M :
                                                                ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd;
                    ldecAccountOwnerLTDAccountBalance = ibusPersonAccount.ibusPersonAccountRetirement.Total_Account_Balance_ltd;

                    if ((!icdoBenefitCalculation.is_return_to_work_member) ||
                        ((icdoBenefitCalculation.is_return_to_work_member) && (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_No_Value.ToUpper())))
                    {
                        ldecPreTaxContribution += ibusPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd;
                        ldecPostTaxContribution += ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd;
                        ldecLTDAccountBalance += ibusPersonAccount.ibusPersonAccountRetirement.Total_Account_Balance_ltd;
                    }

                    // Setting of Variables for Benefit A/c Updation or Insertion
                    CalculateQDROAmount(true);
                    ldecPreTaxContribution = ldecPreTaxContribution - icdoBenefitCalculation.taxable_qdro_amount;
                    ldecPostTaxContribution = ldecPostTaxContribution - icdoBenefitCalculation.non_taxable_qdro_amount;

                    ldecStartingTaxableAmount = ldecPreTaxContribution;
                    ldecStartingNonTaxableAmount = ldecPostTaxContribution;

                    //PIR 18053 - Adding up Starting Taxable and Non Taxable amounts to the current account for RTW
                    if (icdoBenefitCalculation.is_rtw_member_subsequent_retirement)
                    {
                        if (ibuspre_RTW_Payee_Account == null)
                            LoadPreRTWPayeeAccount();
                        if (ibuspre_RTW_Benefit_Account == null)
                            LoadBenefitAccountRTW(ibuspre_RTW_Payee_Account.icdoPayeeAccount.benefit_account_id);

                        ldecStartingTaxableAmount += ibuspre_RTW_Benefit_Account.icdoBenefitAccount.starting_taxable_amount;
                        ldecStartingNonTaxableAmount += ibuspre_RTW_Benefit_Account.icdoBenefitAccount.starting_nontaxable_amount;

                        idecMemberRHICAmount += ibuspre_RTW_Benefit_Account.icdoBenefitAccount.rhic_benefit_amount;
                    }


                    //PIR: 1958: As per satya, For Disability Payee account also the benefit begin date should be retirement date.
                    /*if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                    {*/
                    ldteBenefitBeginDate = icdoBenefitCalculation.retirement_date;
                    if (icdoBenefitCalculation.disability_to_normal == busConstant.Flag_Yes)
                    { 
                        // PROD PIR ID 5482
                        if (ibusDisabilityPayeeAccount == null)
                            LoadDisabilityPayeeAccount();
                        ldteBenefitBeginDate = ibusDisabilityPayeeAccount.icdoPayeeAccount.disa_normal_effective_date_no_null;
                    }
                    /*}
                    else
                    {
                        ldteBenefitBeginDate = busGlobalFunctions.GetMax(icdoBenefitCalculation.retirement_date, ldteNextBenefitPaymentDate);
                    }
                    */
                    // 1. Creates or Updates the Benefit Account
                    if (icdoBenefitCalculation.disability_to_normal == busConstant.Flag_Yes)
                    {
                        // PIR ID 1778 - Do not create new BenefitAccountID in case of Disability to Normal.
                        lintBenefitAccountID = IsBenefitAccountExists(ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            busConstant.ApplicationBenefitTypeDisability);
                    }
                    else
                    {
                        lintBenefitAccountID = IsBenefitAccountExists(ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            icdoBenefitCalculation.benefit_account_type_value);
                    }

                    if (icdoBenefitCalculation.uniform_income_or_ssli_flag == busConstant.Flag_Yes)
                    {
                        if (icdoBenefitCalculation.ssli_effective_date == DateTime.MinValue)
                            icdoBenefitCalculation.ssli_effective_date = GetSSLIEffectivedate(ibusMember.icdoPerson.date_of_birth,
                                                                                icdoBenefitCalculation.ssli_or_uniform_income_commencement_age);
                    }
                    if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    {
                        lintBenefitAccountID = ManageBenefitAccount(ldecStartingTaxableAmount, 0M, busConstant.StatusValid,
                                                        icdoBenefitCalculation.rhic_option_value, idecMemberRHICAmount, icdoBenefitCalculation.credited_psc,
                                                        icdoBenefitCalculation.credited_vsc, iintRetirementOrgId, lintBenefitAccountID,
                                                        icdoBenefitCalculation.ssli_effective_date, icdoBenefitCalculation.estimated_ssli_benefit_amount,
                                                        string.Empty, ldecOptionFactor, ldecRHICOptionFactor, idecSpouseRHICAmount);
                    }
                    else
                    {
                        lintBenefitAccountID = ManageBenefitAccount(ldecStartingTaxableAmount, ldecStartingNonTaxableAmount, busConstant.StatusValid,
                                                        icdoBenefitCalculation.rhic_option_value, idecMemberRHICAmount, icdoBenefitCalculation.credited_psc,
                                                        icdoBenefitCalculation.credited_vsc, iintRetirementOrgId, lintBenefitAccountID,
                                                        icdoBenefitCalculation.ssli_effective_date, icdoBenefitCalculation.estimated_ssli_benefit_amount,
                                                        icdoBenefitCalculation.rule_indicator_value, ldecOptionFactor, ldecRHICOptionFactor, idecSpouseRHICAmount);
                    }

                    if (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper())
                    {
                        // If the RTW chooses Refund.
                        ibusPersonAccount.LoadTotalPSC(icdoBenefitCalculation.termination_date);
                        ibusPersonAccount.LoadTotalVSC();
                        lintRTWRefundBenefitAccountID = GetRTWRefundBenefitAccountID();
                        lintRTWRefundBenefitAccountID = ManageBenefitAccount(ldecAccountOwnerStartingTaxableAmount, ldecAccountOwnerStartingNonTaxableAmount,
                                                            busConstant.StatusValid, string.Empty, 0M, ibusPersonAccount.icdoPersonAccount.Total_PSC,
                                                            ibusPersonAccount.icdoPersonAccount.Total_VSC, iintRetirementOrgId,
                                                            lintRTWRefundBenefitAccountID, DateTime.MinValue, 0M,
                                                            icdoBenefitCalculation.rule_indicator_value, ldecOptionFactor, ldecRHICOptionFactor, 0M);

                        lintRTWRefundPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(
                                    icdoBenefitCalculation.person_id, lintRTWRefundBenefitAccountID,
                                    busConstant.PayeeAccountAccountRelationshipOwner,
                                    icdoBenefitCalculation.benefit_account_type_value, false);
                        if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                        {
                            lintRTWRefundPayeeAccountID = busPayeeAccountHelper.ManagePayeeAccount(icdoBenefitCalculation.person_id, 0,
                                                            icdoBenefitCalculation.benefit_application_id,
                                                            icdoBenefitCalculation.benefit_calculation_id,
                                                            lintRTWRefundBenefitAccountID, 0, busConstant.StatusValid, busConstant.ApplicationBenefitTypeRefund,
                                                            busConstant.BenefitOptionRegularRefund, busConstant.Flag_No, DateTime.MinValue,
                                                            DateTime.MinValue, busConstant.PayeeAccountAccountRelationshipOwner, busConstant.FamilyRelationshipUnknown,
                                                            ldecAccountOwnerLTDAccountBalance, ldecAccountOwnerStartingNonTaxableAmount,
                                                            busConstant.BenefitOptionRefund, 0, lintRTWRefundPayeeAccountID,
                                                            busConstant.PayeeAccountExclusionMethodSimplified, 0.0M, DateTime.MinValue, string.Empty, 0, icdoBenefitCalculation.graduated_benefit_option_value);
                        }
                        else
                        {
                            lintRTWRefundPayeeAccountID = busPayeeAccountHelper.ManagePayeeAccount(icdoBenefitCalculation.person_id, 0,
                                                            icdoBenefitCalculation.benefit_application_id,
                                                            icdoBenefitCalculation.benefit_calculation_id,
                                                            lintRTWRefundBenefitAccountID, 0, busConstant.StatusValid, busConstant.ApplicationBenefitTypeRefund,
                                                            busConstant.BenefitOptionRegularRefund, busConstant.Flag_No, ldteBenefitBeginDate,
                                                            DateTime.MinValue, busConstant.PayeeAccountAccountRelationshipOwner, busConstant.FamilyRelationshipUnknown,
                                                            ldecAccountOwnerLTDAccountBalance, ldecAccountOwnerStartingNonTaxableAmount,
                                                            busConstant.BenefitOptionRefund, 0, lintRTWRefundPayeeAccountID,
                                                            busConstant.PayeeAccountExclusionMethodSimplified, 0.0M, DateTime.MinValue, string.Empty, 0, icdoBenefitCalculation.graduated_benefit_option_value);
                        }

                        busPayeeAccount lobjRTWRefundPayeeAccount = new busPayeeAccount();
                        lobjRTWRefundPayeeAccount.FindPayeeAccount(lintRTWRefundPayeeAccountID);

                        decimal ldecpretaxeeamountltd = ibusPersonAccount.ibusPersonAccountRetirement.pre_tax_ee_amount_ltd +
                                                        ibusPersonAccount.ibusPersonAccountRetirement.pre_tax_ee_ser_pur_cont_ltd;
                        decimal ldecposttaxeeamountltd = ibusPersonAccount.ibusPersonAccountRetirement.post_tax_ee_amount_ltd +
                                                        ibusPersonAccount.ibusPersonAccountRetirement.post_tax_ee_ser_pur_cont_ltd;
                        decimal ldeceerhicamountltd = ibusPersonAccount.ibusPersonAccountRetirement.ee_rhic_amount_ltd +
                                                        ibusPersonAccount.ibusPersonAccountRetirement.ee_rhic_ser_pur_cont_ltd;

                        if (ibusPersonAccount.ibusPersonAccountRetirement.ee_er_pickup_amount_ltd > 0.0M)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemEEERPickupAmount,
                                                        ibusPersonAccount.ibusPersonAccountRetirement.ee_er_pickup_amount_ltd, "", 0,
                                                        ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        //UCS - 079 : If new amount is 0.0M, need to end date existing one
                        else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemEEERPickupAmount,
                                                                                   0, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        if (ibusPersonAccount.ibusPersonAccountRetirement.interest_amount_ltd > 0.0M)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemEEInterestAmount,
                                                        ibusPersonAccount.ibusPersonAccountRetirement.interest_amount_ltd, "", 0,
                                                        ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        //UCS - 079 : If new amount is 0.0M, need to end date existing one
                        else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemEEInterestAmount,
                                                        0, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        if (ldeceerhicamountltd > 0.0M)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemRHICEEAmount,
                                                        ldeceerhicamountltd, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        //UCS - 079 : If new amount is 0.0M, need to end date existing one
                        else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemRHICEEAmount,
                                                        0, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        if (ldecposttaxeeamountltd > 0.0M)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPostTaxEEContributionAmount,
                                                        ldecposttaxeeamountltd, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        //UCS - 079 : If new amount is 0.0M, need to end date existing one
                        else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPostTaxEEContributionAmount,
                                                        0, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        if (ldecpretaxeeamountltd > 0.0M)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPreTaxEEContributionAmount,
                                                        ldecpretaxeeamountltd, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        //UCS - 079 : If new amount is 0.0M, need to end date existing one
                        else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPreTaxEEContributionAmount,
                                                        0, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        if (ibusPersonAccount.ibusPersonAccountRetirement.er_vested_amount_ltd > 0.0M)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemVestedERContributionAmount,
                                                        ibusPersonAccount.ibusPersonAccountRetirement.er_vested_amount_ltd, "", 0,
                                                        ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        //UCS - 079 : If new amount is 0.0M, need to end date existing one
                        else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemVestedERContributionAmount,
                                                        0, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        if (ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain > 0.0M)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemCapitalGain,
                                                        ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain, "", 0,
                                                        ldteBenefitBeginDate, DateTime.MinValue, false);
                        }
                        //UCS - 079 : If new amount is 0.0M, need to end date existing one
                        else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                        {
                            lobjRTWRefundPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemCapitalGain,
                                                        0, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                        }

                        // Insert the Payee Account status.
                        // No record will be created if the status is already in Review.
                        lobjRTWRefundPayeeAccount.CreateReviewPayeeAccountStatus();
                    }

                    // 2. Creates or Updates the Payee Account
                    lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(
                                                    icdoBenefitCalculation.person_id, lintBenefitAccountID,
                                                    busConstant.PayeeAccountAccountRelationshipOwner,
                                                    icdoBenefitCalculation.benefit_account_type_value, false);
                    string astrBenefitSubtype = icdoBenefitCalculation.benefit_account_sub_type_value;

                    if ((string.IsNullOrEmpty(astrBenefitSubtype)) && (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                    {
                        astrBenefitSubtype = busConstant.ApplicationBenefitSubTypeDisability; //PIR 9899
                    }

                    //Backlog PIR 10403 - Added Condition for DC plan
                    if (string.IsNullOrEmpty(astrBenefitSubtype) && (icdoBenefitCalculation.plan_id == busConstant.PlanIdDC
                        || icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 //PIR 20232
                        || icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025)) //PIR 25920
                    {
                        decimal ldecMemberAgeMontAndYear = 0.0M;
                        string lstrBenefitSubType = string.Empty;

                        CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.retirement_date, ref ldecMemberAgeMontAndYear, 4);

                        IsMemberEligibleForDCPlan(ldecMemberAgeMontAndYear, ref lstrBenefitSubType);

                        if (!(lstrBenefitSubType.IsNullOrEmpty()))
                            astrBenefitSubtype = lstrBenefitSubType;
                    }

                    decimal ldecMinimumGuarantee = 0.0M;
                    decimal ldecNonTaxableBeginningBalance = 0.0M;
                    if (icdoBenefitCalculation.benefit_account_type_value != busConstant.ApplicationBenefitTypeDisability)
                    {
                        //As per Satya PIR:1440  Minimum Guarantee and Non Taxable Beginning Balance should not persisted in
                        //Payee Account since it is taken as on that date in Payee Account Maintenance Screen.
                        ldecMinimumGuarantee = icdoBenefitCalculation.minimum_guarentee_amount;
                        ldecNonTaxableBeginningBalance = ldecPostTaxContribution;
                    }

                    //PIR 18053
                    if (icdoBenefitCalculation.is_rtw_member_subsequent_retirement)
                    {
                        if (ibuspre_RTW_Payee_Account == null)
                            LoadPreRTWPayeeAccount();
                        
                        ibuspre_RTW_Payee_Account.LoadMinimumGuaranteeAmount();

                        ldecMinimumGuarantee += ibuspre_RTW_Payee_Account.idecMinimumGuaranteeAmount;
                        ldecNonTaxableBeginningBalance += ibuspre_RTW_Payee_Account.icdoPayeeAccount.nontaxable_beginning_balance;
                    }

                    DateTime ldteTermCertainEndDate = new DateTime();
                    if (IsTermCertainBenefitOption())
                    {
                        ldteTermCertainEndDate = ldteBenefitBeginDate.AddMonths(iintTermCertainMonths);
                        // PIR ID 1879 Term- Certain Date should be last day of month
                        if (ldteTermCertainEndDate != DateTime.MinValue)
                            ldteTermCertainEndDate = ldteTermCertainEndDate.AddDays(-1);
                        //PIR 17089 - Term Certain End Date should remain the same when converting from Disability to Normal
                        if (icdoBenefitCalculation.disability_to_normal == busConstant.Flag_Yes)
                        {
                            if (ibusDisabilityPayeeAccount == null) LoadDisabilityPayeeAccount();
                            if (ibusDisabilityPayeeAccount.icdoPayeeAccount.term_certain_end_date != DateTime.MinValue)
                                ldteTermCertainEndDate = ibusDisabilityPayeeAccount.icdoPayeeAccount.term_certain_end_date;
                        }
                    }
                    //UCS - 079 : if payee account exists and calculation type is Adjustments, need to store history for minimum guarantee amount
                    if (lintPayeeAccountID > 0 && (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment))
                    {
                        //loading payee account bus. object
                        busPayeeAccount lobjPA = new busPayeeAccount();
                        lobjPA.FindPayeeAccount(lintPayeeAccountID);
                        if (ibusBaseActivityInstance.IsNotNull())
                        {
                            //if (lobjPA.ibusBaseActivityInstance.IsNull())
                            //    lobjPA.ibusBaseActivityInstance = new busActivityInstance();
                            lobjPA.ibusBaseActivityInstance = ibusBaseActivityInstance;
                            // lobjPA.SetProcessInstanceParameters();
                            lobjPA.SetCaseInstanceParameters();
                        }
                        //Creating minimum guarantee history
                        busPayeeAccountMinimumGuaranteeHistory lobjMinimumGuaranteeHistory = new busPayeeAccountMinimumGuaranteeHistory();
                        lobjMinimumGuaranteeHistory.CreateMinimumGuaranteeHistory(lobjPA.icdoPayeeAccount.payee_account_id,
                                                                                    lobjPA.icdoPayeeAccount.minimum_guarantee_amount,
                                                                                    DateTime.Now,
                                                                                    busConstant.Flag_No);

                    }
                    if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
                    {
                        lintPayeeAccountID = busPayeeAccountHelper.ManagePayeeAccount(icdoBenefitCalculation.person_id, 0,
                                                            icdoBenefitCalculation.benefit_application_id,
                                                            icdoBenefitCalculation.benefit_calculation_id,
                                                            lintBenefitAccountID, 0, busConstant.StatusValid, icdoBenefitCalculation.benefit_account_type_value,
                                                            astrBenefitSubtype, busConstant.Flag_No, DateTime.MinValue,
                                                            DateTime.MinValue, busConstant.PayeeAccountAccountRelationshipOwner, busConstant.FamilyRelationshipUnknown,
                                                            ldecMinimumGuarantee, ldecNonTaxableBeginningBalance,
                                                            icdoBenefitCalculation.benefit_option_value, idecMemberRHICAmount,
                                                            lintPayeeAccountID, busConstant.PayeeAccountExclusionMethodSimplified, 0.0M, ldteTermCertainEndDate, string.Empty, 0, icdoBenefitCalculation.graduated_benefit_option_value);
                    }
                    else
                    {
                        lintPayeeAccountID = busPayeeAccountHelper.ManagePayeeAccount(icdoBenefitCalculation.person_id, 0,
                                                            icdoBenefitCalculation.benefit_application_id,
                                                            icdoBenefitCalculation.benefit_calculation_id,
                                                            lintBenefitAccountID, 0, busConstant.StatusValid, icdoBenefitCalculation.benefit_account_type_value,
                                                            astrBenefitSubtype, busConstant.Flag_No, ldteBenefitBeginDate,
                                                            DateTime.MinValue, busConstant.PayeeAccountAccountRelationshipOwner, busConstant.FamilyRelationshipUnknown,
                                                            ldecMinimumGuarantee, ldecNonTaxableBeginningBalance,
                                                            icdoBenefitCalculation.benefit_option_value, idecMemberRHICAmount,
                                                            lintPayeeAccountID, busConstant.PayeeAccountExclusionMethodSimplified, 0.0M, ldteTermCertainEndDate, string.Empty, 0, icdoBenefitCalculation.graduated_benefit_option_value);
                    }

                    // 3. Update Payee Account ID in Benefit Calculation Payee             
                    if (iclbBenefitCalculationPayee == null)
                        LoadBenefitCalculationPayee();
                    foreach (busBenefitCalculationPayee lobjBenCalcPayee in iclbBenefitCalculationPayee)
                    {
                        if (lobjBenCalcPayee.icdoBenefitCalculationPayee.payee_person_id == icdoBenefitCalculation.person_id)
                        {
                            lobjBenCalcPayee.icdoBenefitCalculationPayee.payee_account_id = lintPayeeAccountID;
                            lobjBenCalcPayee.icdoBenefitCalculationPayee.Update();
                        }
                    }

                    decimal ldecTaxablePLSO = 0M;
                    decimal ldecNonTaxablePLSO = 0M;
                    decimal ldecExclusionRatioAmount = 0M;
                    decimal ldecTaxableAmount = 0M;
                    decimal ldecNonTaxableAmount = 0M;

                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                    lobjPayeeAccount.FindPayeeAccount(lintPayeeAccountID);
                    if (ibusBaseActivityInstance.IsNotNull())
                    {
                        //if (lobjPayeeAccount.ibusBaseActivityInstance.IsNull())
                        //    lobjPayeeAccount.ibusBaseActivityInstance = new busActivityInstance();
                        lobjPayeeAccount.ibusBaseActivityInstance = ibusBaseActivityInstance;
                        lobjPayeeAccount.SetCaseInstanceParameters();
                    }

                    decimal ldecFinalMonthlyBenefit = icdoBenefitCalculation.final_monthly_benefit;
                    // 4. Check if Converting from Disability
                    // UCS-080 - Executes only While Converting Disability To Normal Payee Account
                    if (icdoBenefitCalculation.disability_to_normal == busConstant.Flag_Yes)
                    {
                        if (ibusDisabilityPayeeAccount == null)
                            LoadDisabilityPayeeAccount();
                        if (icdoBenefitCalculation.is_rule_or_age_conversion == busConstant.ConvertedToNormalByAge)
                        {
                            CancelCalculationCreatedAsPerRule();
                            ldecFinalMonthlyBenefit = busGlobalFunctions.GetMax(
                                                        icdoBenefitCalculation.final_monthly_benefit, icdoBenefitCalculation.disability_gross_benefit_amount);
                        }
                        // ** BR-080-04 ** Terminating the Disability Account
                        busPayeeAccountHelper.CreatePayeeAccountStatus(
                                                        ibusDisabilityPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                        busConstant.PayeeAccountStatusDisabilityPaymentCompleted,
                                                        ibusDisabilityPayeeAccount.icdoPayeeAccount.disa_normal_effective_date_no_null.AddDays(-1),
                                                        string.Empty, busConstant.TerminationReasonDisabilityToNormal);
                        ibusDisabilityPayeeAccount.icdoPayeeAccount.benefit_end_date = ibusDisabilityPayeeAccount.icdoPayeeAccount.disa_normal_effective_date_no_null.AddDays(-1);
                        ibusDisabilityPayeeAccount.icdoPayeeAccount.Update();

                        lobjPayeeAccount.icdoPayeeAccount.benefit_account_id = lintBenefitAccountID;
                        CopyFromDisabilityPayeeAccount(lobjPayeeAccount);
                        lobjPayeeAccount.idtStatusEffectiveDate = DateTime.Today; // PROD PIR ID 5482
                    }

                    // 5. Inserts the Payee Account Payment Item Type
                    if (icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes)
                    {
                        CalculatePLSOTaxComponents(icdoBenefitCalculation.plso_lumpsum_amount, ldecPostTaxContribution,
                                                        icdoBenefitCalculation.reduced_monthly_after_dnro_early, icdoBenefitCalculation.plso_factor, 0,
                                                        ref ldecNonTaxablePLSO, ref ldecTaxablePLSO, ref ldecExclusionRatio);
                        if (ldecTaxablePLSO > 0M)
                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPLSOTaxableAmount, ldecTaxablePLSO, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false, ablnIsRTWRecalculate : icdoBenefitCalculation.is_rtw_member_recalculate);
                        //UCS - 079 : If new amount is 0.0M, need to end date existing one
                        else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPLSOTaxableAmount, 0, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false, ablnIsRTWRecalculate: icdoBenefitCalculation.is_rtw_member_recalculate);
                        if (ldecNonTaxablePLSO > 0M)
                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPLSONonTaxableAmount, ldecNonTaxablePLSO, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false, ablnIsRTWRecalculate: icdoBenefitCalculation.is_rtw_member_recalculate);
                        //UCS - 079 : If new amount is 0.0M, need to end date existing one
                        else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPLSONonTaxableAmount, 0, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false, ablnIsRTWRecalculate: icdoBenefitCalculation.is_rtw_member_recalculate);
                    }
                    if (icdoBenefitCalculation.benefit_account_type_value != busConstant.ApplicationBenefitTypeDisability)
                    {
                        CalculateMonthlyTaxComponents(icdoBenefitCalculation.created_date, idecMemberAgeBasedOnRetirementDate, icdoBenefitCalculation.annuitant_age,
                                                        ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.exclusive_calc_payment_type_value,
                                                        ldecPostTaxContribution, ldecFinalMonthlyBenefit, ldecNonTaxablePLSO, 0,
                                                        ref ldecNonTaxableAmount, ref ldecTaxableAmount, ref ldecExclusionRatioAmount, 0M);
                    }
                    else
                    {
                        ldecNonTaxableAmount = 0M; ldecExclusionRatioAmount = 0M;
                        ldecTaxableAmount = ldecFinalMonthlyBenefit - ldecNonTaxableAmount;
                    }

                    // PIR ID 1888 - Reduce Gross_Reduction_Amount from Taxable amount has to be done from the old payee account
                    if (icdoBenefitCalculation.is_return_to_work_member)
                    {
                        decimal ldecGrossReductionAmount = Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoPaymentRecovery.GetGrossReductionAmount",
                                                        new object[1] { icdoBenefitCalculation.pre_rtw_payee_account_id },
                                                        iobjPassInfo.iconFramework,
                                                        iobjPassInfo.itrnFramework));
                        ldecTaxableAmount = ldecTaxableAmount - ldecGrossReductionAmount;
                    }

                    if (ldecTaxableAmount > 0M)
                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxableAmount, ldecTaxableAmount, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false, ablnIsRTWRecalculate: icdoBenefitCalculation.is_rtw_member_recalculate);
                    //UCS - 079 : If new amount is 0.0M, need to end date existing one
                    else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxableAmount, 0, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false, ablnIsRTWRecalculate: icdoBenefitCalculation.is_rtw_member_recalculate);
                    if (ldecNonTaxableAmount > 0M)
                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxalbeAmount, ldecNonTaxableAmount, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false, ablnIsRTWRecalculate: icdoBenefitCalculation.is_rtw_member_recalculate);
                    //UCS - 079 : If new amount is 0.0M, need to end date existing one
                    else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxalbeAmount, 0, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false, ablnIsRTWRecalculate: icdoBenefitCalculation.is_rtw_member_recalculate);

                    //PIR 18053
                    //In case of RTW new logic, the PAPITs of old payee account should be added to the new payee account to calculate the sum of benefit amount 
                    //from both the employments.
                    if (icdoBenefitCalculation.is_rtw_member_subsequent_retirement && !icdoBenefitCalculation.is_rtw_member_recalculate)
                        lobjPayeeAccount.CreatePayeeAccountPaymentItemTypeRTW(icdoBenefitCalculation.pre_rtw_payee_account_id,ldteNextBenefitPaymentDate);

                    // 6. Make the Payee Account Status to Review
                    // No record will be created if the Status is already in Review
                    lobjPayeeAccount.CreateReviewPayeeAccountStatus();

                    // 7. Update the Benefit Calculation
                    icdoBenefitCalculation.plso_exclusion_ratio = ldecExclusionRatio;
                    icdoBenefitCalculation.action_status_value = busConstant.BenefitActionStatusApproved;
                    icdoBenefitCalculation.action_status_description =
                                                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, icdoBenefitCalculation.action_status_value);
                    icdoBenefitCalculation.approved_by = iobjPassInfo.istrUserID;
                    icdoBenefitCalculation.Update();

                    // 8. Make the Payee Account status to Payment Complete - BR-060-23
                    if (icdoBenefitCalculation.is_return_to_work_member)
                    {
                        if (iclbBenefitCalculationPersonAccount == null)
                            LoadBenefitCalculationPersonAccount();
                        foreach (busBenefitCalculationPersonAccount lobjBenCalcPersonAccount in iclbBenefitCalculationPersonAccount)
                        {
                            if (lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag == busConstant.Flag_Yes)
                            {
                                busPayeeAccountHelper.CreatePayeeAccountStatus(
                                                        lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.payee_account_id,
                                                        busConstant.PayeeAccountStatusPaymentComplete, DateTime.Today, string.Empty, string.Empty);
                            }
                        }
                    }
                    //UCS -079 : if RTW, then need to update new payee account id and calculation id
                    //--Start of code--//
                    if (icdoBenefitCalculation.is_return_to_work_member && lintPayeeAccountID > 0)
                    {
                        UpdatePayeeAccountIDAndCalculationID(lintPayeeAccountID);
                    }
                    //--End of code--//
                    //UCS - 079 : If calculation type is Adjustments, then need to create Overpayment/Underpayment
                    //--Start of code --//
                    if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
                    {
                        lobjPayeeAccount.CreateBenefitOverPaymentorUnderPayment(busConstant.BenRecalAdjustmentReasonRecalculation);
                    }
                    //--End of code --//
                    //UAT pir - 1348
                    //benefit receipt date to be populated only on approval of calculation for retirement benefit type
                    //--Start--//
                    UpdateDROBenefitReceiptDate();
                    //--End--//
                    // Validate and Load Errors
                    base.ValidateSoftErrors();
                    LoadErrors();
                    base.UpdateValidateStatus();
                    CreateRHICCombineOnCalculationApproval(lobjPayeeAccount, icdoBenefitCalculation.calculation_type_value);
                    // Refresh the cdo
                    icdoBenefitCalculation.Select();
                    foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                    {
                        lobjPayee.icdoBenefitCalculationPayee.Select();
                    }
                    //PIR 18493 - WSS Application Wizard
                    if (Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.IsWSSAppWizardVisible, iobjPassInfo)).ToUpper() == busConstant.Flag_Yes)
                    {
                        if (lobjPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                            InitiatePostCreatePayeeAccountProcess(lobjPayeeAccount.icdoPayeeAccount.payee_account_id);
                    }
                    if (ibusBaseActivityInstance != null)
                    {
                        busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
                        if (lbusActivityInstance.ibusBpmProcessInstance == null)
                        {
                            lbusActivityInstance.LoadBpmProcessInstance();
                        }
                        if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Recalculate_Pension_and_RHIC_Benefit)
                        {
                            lbusActivityInstance.UpdateParameter("payee_account_reference_id", lintPayeeAccountID.ToString());
                        }
                        if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Process_DB_Retirement_Application
                            || busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Process_Disability_Application
                            || busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_MSS_Process_DB_Retirement_Application)
                        {
                            lbusActivityInstance.UpdateParameter("payee_account_reference_id", lintPayeeAccountID.ToString());
                        }
                    }
                    //PIR 18974
                    IsBenefitOverpaymentBPMInitiate();

                    iintPayeeAccountID = lintPayeeAccountID;
                    larrErrors.Add(this);
                    //Load initial rules
                    this.EvaluateInitialLoadRules();
                }
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    larrErrors.Add(larr);
                }
            }
            return larrErrors;
        }

        // Updates the Benefit Calculation to Cancelled, since the New calculation created as per Rule.
        private void CancelCalculationCreatedAsPerRule()
        {
            DataTable ldtbResult = Select<cdoBenefitCalculation>(
                        new string[2] { "disability_payee_account_id", "is_rule_or_age_conversion" },
                        new object[2] { icdoBenefitCalculation.disability_payee_account_id, busConstant.ConvertedToNormalByRule },
                        null, null);

            foreach (DataRow dr in ldtbResult.Rows)
            {
                busBenefitCalculation lobjRetrCalc = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
                lobjRetrCalc.icdoBenefitCalculation.LoadData(dr);
                lobjRetrCalc.icdoBenefitCalculation.action_status_value = busConstant.BenefitActionStatusCancelled;
                lobjRetrCalc.icdoBenefitCalculation.Update();
            }
        }

        // ** BR-080-10 ** Copies the Disability Payee Account Items to the Given/Created Payee Account
        private void CopyFromDisabilityPayeeAccount(busPayeeAccount aobjPayeeAccount)
        {
            if (ibusDisabilityPayeeAccount == null)
                LoadDisabilityPayeeAccount();

            if (aobjPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                aobjPayeeAccount.LoadNexBenefitPaymentDate();

            if (aobjPayeeAccount.icdoPayeeAccount != null)
            {
                // Copy the Deductions
                if (ibusDisabilityPayeeAccount.iclbDeductions == null)
                    ibusDisabilityPayeeAccount.LoadDeductions();
                aobjPayeeAccount.iclbDeductions = new Collection<busPayeeAccountPaymentItemType>();
                foreach (busPayeeAccountPaymentItemType lobjDeduction in ibusDisabilityPayeeAccount.iclbDeductions)
                {
                    if ((lobjDeduction.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue) ||
                        (lobjDeduction.icdoPayeeAccountPaymentItemType.end_date >= aobjPayeeAccount.idtNextBenefitPaymentDate))
                    {
                        lobjDeduction.icdoPayeeAccountPaymentItemType.payee_account_id = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                        lobjDeduction.icdoPayeeAccountPaymentItemType.Insert();
                        aobjPayeeAccount.iclbDeductions.Add(lobjDeduction);
                    }
                }

                // Copy the Federal Tax Withholding Info
                if (ibusDisabilityPayeeAccount.iclbPayeeAccountFedTaxWithHolding == null)
                    ibusDisabilityPayeeAccount.LoadFedTaxWithHoldingInfo();
                aobjPayeeAccount.iclbPayeeAccountFedTaxWithHolding = new Collection<busPayeeAccountTaxWithholding>();
                foreach (busPayeeAccountTaxWithholding lobjFedTaxWithholding in ibusDisabilityPayeeAccount.iclbPayeeAccountFedTaxWithHolding)
                {
                    if ((lobjFedTaxWithholding.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue) ||
                        (lobjFedTaxWithholding.icdoPayeeAccountTaxWithholding.end_date >= aobjPayeeAccount.idtNextBenefitPaymentDate))
                    {
                        lobjFedTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                        //PIR 24506
                        lobjFedTaxWithholding.icdoPayeeAccountTaxWithholding.start_date = aobjPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                        lobjFedTaxWithholding.icdoPayeeAccountTaxWithholding.modified_date = DateTime.Now;
                        lobjFedTaxWithholding.icdoPayeeAccountTaxWithholding.created_date = DateTime.Now;
                        lobjFedTaxWithholding.icdoPayeeAccountTaxWithholding.modified_by = iobjPassInfo.istrUserID;
                        lobjFedTaxWithholding.icdoPayeeAccountTaxWithholding.created_by = iobjPassInfo.istrUserID;
                        //end
                        lobjFedTaxWithholding.icdoPayeeAccountTaxWithholding.Insert();
                        aobjPayeeAccount.iclbPayeeAccountFedTaxWithHolding.Add(lobjFedTaxWithholding);
                    }
                }

                // Copy the State Tax Withholding Info
                if (ibusDisabilityPayeeAccount.iclbPayeeAccountStateTaxWithHolding == null)
                    ibusDisabilityPayeeAccount.LoadStateTaxWithHoldingInfo();
                aobjPayeeAccount.iclbPayeeAccountStateTaxWithHolding = new Collection<busPayeeAccountTaxWithholding>();
                foreach (busPayeeAccountTaxWithholding lobjStateTaxWithholding in ibusDisabilityPayeeAccount.iclbPayeeAccountStateTaxWithHolding)
                {
                    if ((lobjStateTaxWithholding.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue) ||
                        (lobjStateTaxWithholding.icdoPayeeAccountTaxWithholding.end_date >= aobjPayeeAccount.idtNextBenefitPaymentDate))
                    {
                        lobjStateTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                        //PIR 24506
                        lobjStateTaxWithholding.icdoPayeeAccountTaxWithholding.start_date = aobjPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                        lobjStateTaxWithholding.icdoPayeeAccountTaxWithholding.modified_date = DateTime.Now;
                        lobjStateTaxWithholding.icdoPayeeAccountTaxWithholding.created_date = DateTime.Now;
                        lobjStateTaxWithholding.icdoPayeeAccountTaxWithholding.modified_by = iobjPassInfo.istrUserID;
                        lobjStateTaxWithholding.icdoPayeeAccountTaxWithholding.created_by = iobjPassInfo.istrUserID;
                        //end
                        lobjStateTaxWithholding.icdoPayeeAccountTaxWithholding.Insert();
                        aobjPayeeAccount.iclbPayeeAccountStateTaxWithHolding.Add(lobjStateTaxWithholding);
                    }
                }

                // Copy the Payee Account ACH Details
                if (ibusDisabilityPayeeAccount.iclbActiveACHDetails == null)
                    ibusDisabilityPayeeAccount.LoadActiveACHDetail();
                aobjPayeeAccount.iclbActiveACHDetails = new Collection<busPayeeAccountAchDetail>();
                foreach (busPayeeAccountAchDetail lobjPAACHDetail in ibusDisabilityPayeeAccount.iclbActiveACHDetails)
                {
                    if ((lobjPAACHDetail.icdoPayeeAccountAchDetail.ach_end_date == DateTime.MinValue) ||
                        (lobjPAACHDetail.icdoPayeeAccountAchDetail.ach_end_date >= aobjPayeeAccount.idtNextBenefitPaymentDate))
                    {
                        lobjPAACHDetail.icdoPayeeAccountAchDetail.payee_account_id = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                        //PIR 24506
                        lobjPAACHDetail.icdoPayeeAccountAchDetail.ach_start_date = aobjPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                        lobjPAACHDetail.icdoPayeeAccountAchDetail.modified_date = DateTime.Now;
                        lobjPAACHDetail.icdoPayeeAccountAchDetail.created_date = DateTime.Now;
                        lobjPAACHDetail.icdoPayeeAccountAchDetail.modified_by = iobjPassInfo.istrUserID;
                        lobjPAACHDetail.icdoPayeeAccountAchDetail.created_by = iobjPassInfo.istrUserID;
                        //end
                        lobjPAACHDetail.icdoPayeeAccountAchDetail.Insert();
                        aobjPayeeAccount.iclbActiveACHDetails.Add(lobjPAACHDetail);
                    }
                }

                // Copy the Active Rollover Details
                if (ibusDisabilityPayeeAccount.iclbActiveRolloverDetails == null)
                    ibusDisabilityPayeeAccount.LoadActiveRolloverDetail();
                aobjPayeeAccount.iclbActiveRolloverDetails = new Collection<busPayeeAccountRolloverDetail>();
                foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in ibusDisabilityPayeeAccount.iclbActiveRolloverDetails)
                {
                    lobjRolloverDetail.iclbRolloverItemDetail = new Collection<busPayeeAccountRolloverItemDetail>();
                    lobjRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_id = aobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                    lobjRolloverDetail.icdoPayeeAccountRolloverDetail.Insert();
                    lobjRolloverDetail.LoadRolloverItemDetail();
                    foreach (busPayeeAccountRolloverItemDetail lobjRolloverItemDetail in lobjRolloverDetail.iclbRolloverItemDetail)
                    {
                        lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.payee_account_rollover_detail_id =
                            lobjRolloverDetail.icdoPayeeAccountRolloverDetail.payee_account_rollover_detail_id;
                        lobjRolloverItemDetail.icdoPayeeAccountRolloverItemDetail.Insert();
                    }
                }
            }
        }

        /// <summary>
        /// ************************************************************************ 
        ///                 DETERMINES THE RETIREMENT BENEFIT AMOUNT 
        /// ************************************************************************
        /// </summary>
        public void CalculateRetirementBenefit()
        {
            bool lblnIsJointannuitantExists = false;
            if (ibusMember == null)
                LoadMember();
            if (ibusJointAnnuitant == null)
                LoadJointAnnuitant();
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();

            // PIR ID 1119
            if (ibusJointAnnuitant.icdoPerson.date_of_birth != DateTime.MinValue && ibusJointAnnuitant.icdoPerson.date_of_death == DateTime.MinValue)
                lblnIsJointannuitantExists = true;

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            ResetCalculationObject();

            // ********* GET BENEFIT PROVISION BENEFIT TYPE *********
            if (ibusBenefitProvisionBenefitType == null)
                LoadBenefitProvisionBenefitType();

            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)//to handle when RecalculateBenefit is clicked from PA
                CalculateQDROAmount(true);

            // ********* CALCULATE FAS *********
            if (iclbBenefitCalculationFASMonths == null)
                iclbBenefitCalculationFASMonths = new Collection<busBenefitCalculationFasMonths>();
            CalculateFAS();

            // ********* CALCULATE BENEFIT MULTIPLIER *********
            if (!iblnBenefitMultiplierLoaded)
                CalculateBenefitMultiplier();

            // Disability reduced amount
            decimal adecReducedAmount1 = 0.0M;
            decimal adecReducedAmount2 = 0.0M;
            if ((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
                ((icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Judges) ||
                (icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Highway_Patrol)))
            {
                LoadSsliAndWsiAmount();
                adecReducedAmount2 = idecWSIBenefitAmount;
                if (icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Judges)
                {
                    adecReducedAmount1 = idecSSliBenefitAmount;
                }
            }

            // ********* CALCULATE UNREDUCED BENEFIT AMOUNT *********
            icdoBenefitCalculation.unreduced_benefit_amount =
                                                        CalculateUnReducedMonthlyBenefitAmount(
                                                        icdoBenefitCalculation.calculation_final_average_salary,
                                                        idecFASBenefitMultiplier, icdoBenefitCalculation.plan_id,
                                                        icdoBenefitCalculation.benefit_account_type_value,
                                                        adecReducedAmount1, adecReducedAmount2,
                                                        ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_formula_value);
            //UAT PIR:1657. For JobService Disability do not set the default amount when less than zero.
            // PIR 9021 Effective 4/1/2012, the following Business Rule has been eliminated.
            //if ((icdoBenefitCalculation.unreduced_benefit_amount < 100) &&
            //    (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
            //    (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_formula_value != busConstant.BenefitFormulaValueOther) &&
            //    (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService))
            //{
            //    icdoBenefitCalculation.unreduced_benefit_amount = 100;
            //    icdoBenefitCalculation.reduced_monthly_after_dnro_early = 100;
            //    icdoBenefitCalculation.reduced_monthly_after_plso_deduction = 100;
            //}

            decimal ldecMemberAgeMontAndYear = 0.00M;
            decimal ldecBenAgeMontAndYear = 0.00M;
            DateTime ldtSpouseDateBirth = DateTime.MinValue;
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();

            bool lblnSpouseAlreadyExists = false;

            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                if (lobjPayee.icdoBenefitCalculationPayee.payee_person_id != icdoBenefitCalculation.person_id)
                {
                    ldtSpouseDateBirth = lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth;
                    lblnSpouseAlreadyExists = true;
                    lblnIsJointannuitantExists = true;
                }
            }

            DateTime ldtMemberDateOfBirth = DateTime.MinValue;
            if (ibusMember != null)
            {
                ldtMemberDateOfBirth = ibusMember.icdoPerson.date_of_birth;
            }

            if (ibusJointAnnuitant != null)
            {
                if (icdoBenefitCalculation.is_created_from_portal == busConstant.Flag_Yes) //PIR 14652 
                {
                    if (ibusMember.ibusSpouse.IsNull()) ibusMember.LoadSpouse(); //F/W Upgrade PIR 17373
                    ldtSpouseDateBirth = ibusMember.ibusSpouse.icdoPerson.date_of_birth;
                }
                if (!lblnSpouseAlreadyExists && icdoBenefitCalculation.is_created_from_portal == busConstant.Flag_No)
                    ldtSpouseDateBirth = ibusJointAnnuitant.icdoPerson.date_of_birth;
            }

            icdoBenefitCalculation.reduced_monthly_after_dnro_early = icdoBenefitCalculation.unreduced_benefit_amount;

            // UCS-060 Return to Work
            if ((icdoBenefitCalculation.is_return_to_work_member) &&
                (icdoBenefitCalculation.actuarial_benefit_reduction != 0M))
            {
                icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit = Math.Round(icdoBenefitCalculation.unreduced_benefit_amount *
                                                                                            (icdoBenefitCalculation.actuarial_benefit_reduction / 100),
                                                                                            2, MidpointRounding.AwayFromZero);
                icdoBenefitCalculation.reduced_monthly_after_dnro_early = Math.Round(icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit, 2);
            }

            icdoBenefitCalculation.reduced_monthly_after_plso_deduction = icdoBenefitCalculation.reduced_monthly_after_dnro_early;
            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                CalculatePersonAge(ldtMemberDateOfBirth, icdoBenefitCalculation.retirement_date, ref ldecMemberAgeMontAndYear, 4);
                CalculatePersonAge(ldtSpouseDateBirth, icdoBenefitCalculation.retirement_date, ref ldecBenAgeMontAndYear, 4);
                // ********* CALCULATE DNRO BENEFIT AMOUNT *********

                //PIR 11608
                //If the DRO estimate is selected the system should never calculate DNRO.  
				//PIR 14811
                if ((icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO) &&
                    (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.dnro_flag == busConstant.Flag_Yes) &&
                    (icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_No || string.IsNullOrEmpty(icdoBenefitCalculation.is_dro_estimate)))
                {
                    decimal ldecDNROMissedPaymentAmount = 0, ldecMonthlyActuarialIncrease = 0, ldecDNROBenefitAmount = 0, ldecHocAmount = 0;

                    DateTime ldtRetirementDate = icdoBenefitCalculation.retirement_date;
                    DateTime ldtDNRORetirementDate = icdoBenefitCalculation.normal_retirement_date;
                    DateTime ldtChangedEmpEnddate = icdoBenefitCalculation.termination_date.AddMonths(1);
                    DateTime ldtDNROComapredate = DateTime.MinValue;
                    
                    if (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService)
                    {
                    ldtDNROComapredate = DateTime.Compare(icdoBenefitCalculation.normal_retirement_date, icdoBenefitCalculation.termination_date.AddMonths(1)) > 0 ? ldtDNRORetirementDate : ldtChangedEmpEnddate;
                    }
                    else
                    {
                    ldtDNROComapredate = icdoBenefitCalculation.normal_retirement_date;
                    }

                    DateTime ldtPersLinkGoLiveDate = busPayeeAccountHelper.GetPERSLinkGoLiveDate();
                    //Commented intentionally will be removed after today's Migration. : Sujatha 30 Sep 2010
                    //uncommented on 1 oct 2010 
                    if (ldtDNROComapredate > ldtPersLinkGoLiveDate)
                    {
                        ldecHocAmount = FetchTotalCOLAorAdhocAmount(ldtDNROComapredate, icdoBenefitCalculation.retirement_date, icdoBenefitCalculation.reduced_monthly_after_dnro_early);
                    }

                    //PIR 14811 - deleted foreach loop which was for DRO process
                    CalculateDNROBenefitAmount(ldecMemberAgeMontAndYear, ldecBenAgeMontAndYear, icdoBenefitCalculation.reduced_monthly_after_dnro_early,
                                                    ldecHocAmount, ref ldecDNROMissedPaymentAmount,
                                                    ref ldecMonthlyActuarialIncrease, ref ldecDNROBenefitAmount);

                    icdoBenefitCalculation.reduced_monthly_after_dnro_early = ldecDNROBenefitAmount;
                }
                // ********* CALCULATE EARLY BENEFIT AMOUNT *********
                else if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
                {
                    decimal ldecEarlyReductionPercentage = 0.00M, ldecEarlyReductionAmount = 0.00M, ldecEarlyReducedMonthlyBenefitAmount = 0.00M;
                    if (!iblnWaiveEarlyReduction)
                    {
                        if (!iblnConsolidatedPSCLoaded)
                            CalculateConsolidatedPSC();

                        CalculateEARLYBenefitAmount(ref ldecEarlyReductionPercentage, ref ldecEarlyReductionAmount, icdoBenefitCalculation.reduced_monthly_after_dnro_early,
                                                    ref ldecEarlyReducedMonthlyBenefitAmount, ldecMemberAgeMontAndYear, ldecBenAgeMontAndYear);
                        icdoBenefitCalculation.reduced_monthly_after_dnro_early = ldecEarlyReducedMonthlyBenefitAmount;
                    }
                }


                // ********* CALCULATE QDRO DEDUCTIONS *********
                icdoBenefitCalculation.reduced_monthly_after_dnro_early =
                        icdoBenefitCalculation.reduced_monthly_after_dnro_early - icdoBenefitCalculation.qdro_amount;

                icdoBenefitCalculation.reduced_monthly_after_plso_deduction = icdoBenefitCalculation.reduced_monthly_after_dnro_early;


                // ********* CALCULATE PLSO BENEFIT AMOUNT *********
                if ((icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes) &&
                    (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.plso_flag == busConstant.Flag_Yes))
                {
                    decimal adecPLSOReducedBenefitAmt = 0.00M, adecPLSOLumpSumAmt = 0.00M;
                    decimal ldecPLSOFactor = 0.0M, adecPLSOReductionAmt = 0.0M;
                    CalculatePLSOBenefitAmount(ldecMemberAgeMontAndYear, ldecBenAgeMontAndYear,
                                                    icdoBenefitCalculation.reduced_monthly_after_plso_deduction, ref adecPLSOReductionAmt,
                                                    ref adecPLSOReducedBenefitAmt, ref adecPLSOLumpSumAmt, ref ldecPLSOFactor);
                    icdoBenefitCalculation.reduced_monthly_after_plso_deduction = adecPLSOReducedBenefitAmt;
                    icdoBenefitCalculation.plso_factor = ldecPLSOFactor;
                    icdoBenefitCalculation.plso_lumpsum_amount = adecPLSOLumpSumAmt;
                    icdoBenefitCalculation.plso_reduction_amount = adecPLSOReductionAmt;
                }
            }

            // ********* CALCULATE BENEFIT OPTIONS *********
            iclbBenefitCalculationOptions = CalculateBenefitAmountForOptionFactors(lblnIsJointannuitantExists, ldtSpouseDateBirth);


            //***********CALCULATE RHIC *************************************************

            string astrBenefitSubType = string.Empty;
            astrBenefitSubType = icdoBenefitCalculation.benefit_account_sub_type_value;
            iblnIsMemberRestrictedForRHIC = false;

            //*************Fix for RHIC Reduction Factor For DC Plan
            //Special Case: For DC Since we dont have eligibility we cannot set the Benefit Subtype as Early or DNRO.The Early Reduction Factor
            // For RHIC for DC Plan is applicable only if the former plan is Main or NG.
            CalculatePersonAge(ldtMemberDateOfBirth, icdoBenefitCalculation.retirement_date, ref ldecMemberAgeMontAndYear, 4);

            if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdDC || icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025) && (astrBenefitSubType.IsNullOrEmpty())) //PIR 20232
            {
                IsMemberEligibleForDCPlan(ldecMemberAgeMontAndYear, ref astrBenefitSubType);
                if (astrBenefitSubType.IsNullOrEmpty())
                {
                    iblnIsMemberRestrictedForRHIC = true;
                }
            }
            if (!(iblnIsMemberRestrictedForRHIC))
            {
                LoadBenefitProvisionBenefitType(DateTime.Today); // PROD PIR ID 5867 - When re-calculate calculate based on Calculation date.
                icdoBenefitCalculation.standard_rhic_amount = CalculateStandardRHICAmount(
                                                            Math.Round(icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero),
                                                            ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor,
                                                            icdoBenefitCalculation.plan_id);
            }
            else
            {
                icdoBenefitCalculation.standard_rhic_amount = 0.0M;
            }
            //// UAT PIR ID 1170
            //if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdDC) &&
            //    ((icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal) ||
            //    (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)))
            //    icdoBenefitCalculation.standard_rhic_amount = 0M;
            icdoBenefitCalculation.rhic_factor_amount = ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor;
            icdoBenefitCalculation.unreduced_rhic_amount = icdoBenefitCalculation.standard_rhic_amount;

            // ********* CALCULATE RHIC OPTIONS *********
            decimal ldecRHICEARLYReductionAmount = 0.0M, ldecRHICAfterEARLYReduction = 0.0M;
            decimal ldecRHICEarlyReductionFactor = 0.0M;

            iclbBenefitRHICOption = CalculateBenefitAmountForRHICOptionFactor(
                                                    icdoBenefitCalculation.calculation_type_value,
                                                    icdoBenefitCalculation.benefit_account_type_value,
                                                    icdoBenefitCalculation.standard_rhic_amount, ldtMemberDateOfBirth,
                                                    ldtSpouseDateBirth, icdoBenefitCalculation.benefit_option_value,
                                                    iblnWaiveEarlyReduction, lblnIsJointannuitantExists,
                                                    astrBenefitSubType,
                                                    icdoBenefitCalculation.retirement_date, DateTime.Today,
                                                    icdoBenefitCalculation.rhic_option_value,
                                                    ref ldecRHICEARLYReductionAmount, ref ldecRHICAfterEARLYReduction, ref ldecRHICEarlyReductionFactor, false);
            icdoBenefitCalculation.rhic_early_reduction_factor = ldecRHICEarlyReductionFactor;

            // ********* CALCULATE MINIMUM GUARANTEE AMOUNT *********
            CalculateMinimumGuaranteedMemberAccount();

            // PIR ID : 1301 - If Reduced Benefit Flag is Checked dont display following values.
            if (icdoBenefitCalculation.reduced_benefit_flag == busConstant.Flag_Yes)
            {
                icdoBenefitCalculation.unreduced_benefit_amount = 0M;
                icdoBenefitCalculation.early_retirement_percentage_decrease = 0M;
                icdoBenefitCalculation.early_monthly_decrease = 0M;
                icdoBenefitCalculation.dnro_factor = 0M;
                icdoBenefitCalculation.dnro_monthly_increase = 0M;
                icdoBenefitCalculation.adhoc_or_cola_amount = 0M;
                icdoBenefitCalculation.plso_lumpsum_amount = 0M;
                icdoBenefitCalculation.reduced_monthly_after_plso_deduction = 0M;
                icdoBenefitCalculation.plso_reduction_amount = 0M;
                icdoBenefitCalculation.plso_factor = 0M;
            }

            //Atlast set the Joint Annuitant Age

            if (ldtSpouseDateBirth != DateTime.MinValue)
            {
                CalculatePersonAge(ldtSpouseDateBirth, icdoBenefitCalculation.retirement_date, ref ldecBenAgeMontAndYear, 4);
                icdoBenefitCalculation.annuitant_age = ldecBenAgeMontAndYear;
            }

            SetRuleIndicator();
            SetIsTentativeTFFRServiceusedFlag();
        }

        public decimal idecMASBenefitAmount { get; set; }

        public Collection<busBenefitCalculationOptions> CalculateBenefitAmountForOptionFactors(bool ablnJointAnnuitantExists, DateTime ldtSpouseDateOfBirth)
        {
            Collection<busBenefitCalculationOptions> lclbBenefitCalcOptions = new Collection<busBenefitCalculationOptions>();

            decimal ldecMemberAge = 0M;
            decimal ldecBeneficiaryAge = 0M;

            /* This is the Prelimnary Age setting for a member and Date.*/
            if (ldecMemberAge == 0)
            {
                CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.retirement_date, ref ldecMemberAge, 4);
            }
            if ((ablnJointAnnuitantExists) && (ldecBeneficiaryAge == 0))
            {
                CalculatePersonAge(ldtSpouseDateOfBirth, icdoBenefitCalculation.retirement_date, ref ldecBeneficiaryAge, 4);
            }

            // Get the Provision Benefit Type for the Current object
            if (ibusBenefitProvisionBenefitType == null)
                LoadBenefitProvisionBenefitType();

            //Setting the Gross Monthly Benefit Amount
            //Set the PLSO Non Taxable Amount
            decimal ldecPostTaxContributionAmount = 0.0M;
            decimal ldecMemberFinalMonthlyAmount = 0.0M;
            decimal ldecSpouseFinalMonthlyAmount = 0.0M;

            if (IsJobService)
            {
                icdoBenefitCalculation.final_monthly_benefit = Slice(icdoBenefitCalculation.reduced_monthly_after_plso_deduction, 2);
            }
            else
            {
                icdoBenefitCalculation.final_monthly_benefit =
                    Math.Round(icdoBenefitCalculation.reduced_monthly_after_plso_deduction, 2, MidpointRounding.AwayFromZero);
            }


            SetPLSONonTaxableAmount(ref ldecPostTaxContributionAmount);

            // Load all the Benefit Provision Options
            Collection<busBenefitProvisionBenefitOption> lclbProvisionBenefitOptions = new Collection<busBenefitProvisionBenefitOption>();
            lclbProvisionBenefitOptions = GetAvailableOptionsDetails(
                                                        ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_provision_id,
                                                        DateTime.Today,
                                                        icdoBenefitCalculation.benefit_account_type_value,
                                                        icdoBenefitCalculation.benefit_option_value,
                                                        icdoBenefitCalculation.uniform_income_or_ssli_flag);
            int lintCounter = 0;
            foreach (busBenefitProvisionBenefitOption lobjProvisionOption in lclbProvisionBenefitOptions)
            {
                if ((string.IsNullOrEmpty(lobjProvisionOption.icdoBenefitProvisionBenefitOption.factor_method_value) ||
                    lobjProvisionOption.icdoBenefitProvisionBenefitOption.factor_method_value == busConstant.FactorMethodValueOther))
                    continue;

                string lstrBenefitOption = lobjProvisionOption.icdoBenefitProvisionBenefitOption.benefit_option_value;
                // string lstrBenefitOption = 
                //busGlobalFunctions.GetData3ByCodeValue(1903, lobjProvisionOption.icdoBenefitProvisionBenefitOption.benefit_option_value);
                if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) && //PIR 19594
                    (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService) &&
                    (!ablnJointAnnuitantExists) &&
                    ((lstrBenefitOption == busConstant.BenefitOption100PercentJS) ||
                     (lstrBenefitOption == busConstant.BenefitOption50PercentJS)))
                    continue;

                // The Five year Term certain is used only for Conversion purpose only. So dont populate it in the Grid. (UAT PIR: 1093).
                //Prod PIR:4467
                //Do not Populate 5 yr term life in case of estimate. For Final with convert to normal this should be present.
                if ((lstrBenefitOption == busConstant.BenefitOption5YearTermLife) && (icdoBenefitCalculation.calculation_type_value== busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
                {
                    continue;
                }

                // Load Member Options
                busBenefitCalculationOptions lobjMemberBenefitOption = new busBenefitCalculationOptions();
                lobjMemberBenefitOption.icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions();
                lobjMemberBenefitOption.ibusBenefitCalculationPayee = new busBenefitCalculationPayee();
                lobjMemberBenefitOption.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
                lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_provision_benefit_option_id =
                    lobjProvisionOption.icdoBenefitProvisionBenefitOption.benefit_provision_benefit_option_id;
                if (iclbBenefitCalculationPayee == null)
                    LoadBenefitCalculationPayee();
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    if (lobjPayee.icdoBenefitCalculationPayee.payee_person_id == icdoBenefitCalculation.person_id)
                    {
                        lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_calculation_payee_id =
                            lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id;
                        lobjMemberBenefitOption.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_value =
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value;
                        lobjMemberBenefitOption.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_description =
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_description;
                        lobjMemberBenefitOption.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value =
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value;
                        lobjMemberBenefitOption.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_description =
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_description;
                    }
                }

                decimal ldecInterimSSLIAmt = 0M;
                decimal ldecSSLIAge = 0M;
                decimal ldecBenefitOptionFactor = 0M;
                if ((icdoBenefitCalculation.uniform_income_or_ssli_flag == busConstant.Flag_Yes) &&
                    (lobjProvisionOption.icdoBenefitProvisionBenefitOption.ssli_factor_method_value == busConstant.FactorMethodValueMemberAndSurvivorAge))
                {
                    CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.retirement_date, ref ldecMemberAge, 4);
                    if (IsJobService)
                    {
                        ldecSSLIAge = 0M;
                    }
                    else
                    {
                        ldecSSLIAge = icdoBenefitCalculation.ssli_or_uniform_income_commencement_age;
                    }

                    decimal ldecSSLIBenefitAmount = icdoBenefitCalculation.estimated_ssli_benefit_amount;
                    decimal ldecSSLIFactor = GetSSLIFactor(icdoBenefitCalculation.plan_id,
                                                        icdoBenefitCalculation.created_date,
                                                        ldecMemberAge, ldecSSLIAge);
                    ldecInterimSSLIAmt = Math.Round((ldecSSLIBenefitAmount * ldecSSLIFactor), 2, MidpointRounding.AwayFromZero);
                    icdoBenefitCalculation.ssli_uniform_income_factor = ldecSSLIFactor;
                    lobjMemberBenefitOption.icdoBenefitCalculationOptions.ssli_factor = ldecSSLIFactor;

                }

                // Need to Load Normal Retirement Date
                if ((!IsJobService) &&
                    (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO))
                {
                    //DateTime ldteLaterTerminationDate = icdoBenefitCalculation.termination_date.AddMonths(1);
                    //ldteLaterTerminationDate = new DateTime(ldteLaterTerminationDate.Year, ldteLaterTerminationDate.Month, 01);
                    //DateTime ldteNormalRetirementDate = new DateTime();
                    //if (ldteLaterTerminationDate > icdoBenefitCalculation.normal_retirement_date)
                    //    ldteNormalRetirementDate = ldteLaterTerminationDate;
                    //else
                    //    ldteNormalRetirementDate = icdoBenefitCalculation.normal_retirement_date;

                    //// PROD PIR ID 5390
                    //if (lobjProvisionOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption100PercentJS)

                    //Backlog PIR 12421 - old code commented.Instead of NRD it should be the Retirement Date in the age Calculation to determine the factors.
                    DateTime ldteRetirementDate = new DateTime();
                    ldteRetirementDate = icdoBenefitCalculation.retirement_date;

                    CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, ldteRetirementDate, ref ldecMemberAge, 4);
                    if ((lstrBenefitOption != busConstant.BenefitOption10YearCertain) ||
                        (lstrBenefitOption != busConstant.BenefitOption20YearCertain))
                    {
                        if (ablnJointAnnuitantExists)
                        {
                            CalculatePersonAge(ldtSpouseDateOfBirth, ldteRetirementDate, ref ldecBeneficiaryAge, 4);
                        }
                    }
                }
                string lstrTempBenefitOption = lobjProvisionOption.icdoBenefitProvisionBenefitOption.benefit_option_value;
                if ((lstrTempBenefitOption == busConstant.BenefitOption10YearCertain) || (lstrTempBenefitOption == busConstant.BenefitOption15YearCertain)
                    || (lstrTempBenefitOption == busConstant.BenefitOption20YearCertain) || (lstrTempBenefitOption == busConstant.BenefitOptionStraightLife))
                {
                    ldecBeneficiaryAge = 0;
                }

                decimal ldecTotAdjustedAmount = 0.0M;

                if (icdoBenefitCalculation.reduced_benefit_option_amount == 0M)
                {
                    if (lobjProvisionOption.icdoBenefitProvisionBenefitOption.factor_method_value == busConstant.FactorMethodValueMemberAndSurvivorAge)
                    {
                        ldecBenefitOptionFactor = GetOptionFactorsForPlan(ldecMemberAge, ldecBeneficiaryAge,
                                                    icdoBenefitCalculation.benefit_account_type_value, icdoBenefitCalculation.plan_id,
                                                    lobjProvisionOption.icdoBenefitProvisionBenefitOption.benefit_option_value);
                    }
                    else if (lobjProvisionOption.icdoBenefitProvisionBenefitOption.factor_method_value == busConstant.FactorMethodValueFactor)
                    {
                        ldecBenefitOptionFactor = lobjProvisionOption.icdoBenefitProvisionBenefitOption.option_factor;
                    }
                    lobjMemberBenefitOption.icdoBenefitCalculationOptions.option_factor = ldecBenefitOptionFactor;
                    decimal ldecGrossAdjustedAmount = 0M;
                    decimal ldecInterimWithoutPLSO = 0M;

                    /* Variables to be binded into Grid which would show the Rounded or truncated Decimals*/
                    decimal ldecBenefitOptionIncreaseorDecrease = 0M;
                    decimal ldecBeforeSSLIAmount = 0M;
                    decimal ldecAfterSSLIAmount = 0M;

                    if (IsJobService)
                    {
						//Backlog PIR 13296
                        decimal ldecBenefitOptionFactorNew = 0;
                        if (lstrBenefitOption == busConstant.BenefitOptionStraightLife || lstrBenefitOption == busConstant.BenefitOption15YearCertain || lstrBenefitOption == busConstant.BenefitOption20YearCertain)
                        {
                            decimal ldecBenefitOptionFactor1 = GetOptionFactorsForPlan(ldecMemberAge + 1, ldecBeneficiaryAge,
                                                        icdoBenefitCalculation.benefit_account_type_value, icdoBenefitCalculation.plan_id,
                                                        lobjProvisionOption.icdoBenefitProvisionBenefitOption.benefit_option_value);

                            int lintTotalMonths = 0;
                            int lintMemberAgeMonthPart = 0;
                            int lintMemberAgeYearPart = 0;
                            decimal adecMonthAndYear = 0;
                            int aintDecimallength = 0;

                            CalculateAge(ibusMember.icdoPerson.date_of_birth.AddMonths(1), icdoBenefitCalculation.retirement_date, ref lintTotalMonths, ref adecMonthAndYear,
                        aintDecimallength, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);

                            ldecBenefitOptionFactorNew = ldecBenefitOptionFactor - ((ldecBenefitOptionFactor - ldecBenefitOptionFactor1) / 12) * lintMemberAgeMonthPart;
                            ldecBenefitOptionFactorNew = Slice(ldecBenefitOptionFactorNew, 4);
                            lobjMemberBenefitOption.icdoBenefitCalculationOptions.option_factor = ldecBenefitOptionFactorNew;
                        }
                        else
                        {
                            ldecBenefitOptionFactorNew = ldecBenefitOptionFactor;
                        }

                        ldecGrossAdjustedAmount = icdoBenefitCalculation.reduced_monthly_after_plso_deduction * ldecBenefitOptionFactorNew;
                        ldecGrossAdjustedAmount = ldecGrossAdjustedAmount - icdoBenefitCalculation.paid_up_annuity_amount;

                        ldecInterimWithoutPLSO = icdoBenefitCalculation.reduced_monthly_after_dnro_early * ldecBenefitOptionFactorNew;
                        ldecInterimWithoutPLSO = ldecInterimWithoutPLSO - icdoBenefitCalculation.paid_up_annuity_amount;

                        ldecBenefitOptionIncreaseorDecrease = ldecGrossAdjustedAmount - icdoBenefitCalculation.reduced_monthly_after_plso_deduction;

                        ldecBeforeSSLIAmount = ldecInterimSSLIAmt + ldecGrossAdjustedAmount;
                        //UAT PIR: 1641 IF beforeSSLIAmount - EstimatedSSLI benefit is ls than 0 then display 0
                        if (ldecBeforeSSLIAmount - icdoBenefitCalculation.estimated_ssli_benefit_amount > 0)
                        {
                            ldecAfterSSLIAmount = ldecBeforeSSLIAmount - icdoBenefitCalculation.estimated_ssli_benefit_amount;
                        }
                        else
                        {
                            ldecAfterSSLIAmount = 0.0M;
                        }

                        if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                            icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments ||
                            icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent ||
                            icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment) ||//to handle when RecalculateBenefit is clicked from PA
                            (!string.IsNullOrEmpty(icdoBenefitCalculation.benefit_option_value)))
                        {
                            icdoBenefitCalculation.final_monthly_benefit = Slice(ldecBeforeSSLIAmount, 2);
                        }

                        //****************Option Factors Values Truncation begins**********************************//
                        ldecBenefitOptionIncreaseorDecrease = Slice(ldecBenefitOptionIncreaseorDecrease, 2);
                        ldecInterimWithoutPLSO = Slice(ldecInterimWithoutPLSO, 2);
                        ldecGrossAdjustedAmount = Slice(ldecGrossAdjustedAmount, 2);
                        ldecBeforeSSLIAmount = Slice(ldecBeforeSSLIAmount, 2);
                        ldecAfterSSLIAmount = Slice(ldecAfterSSLIAmount, 2);
                        ldecTotAdjustedAmount = ldecGrossAdjustedAmount;
                        //****************Option Factors Values Truncation Ends**********************************//

                    }
                    else
                    {
                        ldecGrossAdjustedAmount = icdoBenefitCalculation.reduced_monthly_after_plso_deduction * ldecBenefitOptionFactor;

                        ldecInterimWithoutPLSO = icdoBenefitCalculation.reduced_monthly_after_dnro_early * ldecBenefitOptionFactor;

                        ldecBenefitOptionIncreaseorDecrease = ldecGrossAdjustedAmount - icdoBenefitCalculation.reduced_monthly_after_plso_deduction;

                        ldecBeforeSSLIAmount = ldecInterimSSLIAmt + ldecGrossAdjustedAmount;
                        //UAT PIR: 1641 IF beforeSSLIAmount - EstimatedSSLI benefit is ls than 0 then display 0
                        if (ldecBeforeSSLIAmount - icdoBenefitCalculation.estimated_ssli_benefit_amount > 0)
                        {
                            ldecAfterSSLIAmount = ldecBeforeSSLIAmount - icdoBenefitCalculation.estimated_ssli_benefit_amount;
                        }
                        else
                        {
                            ldecAfterSSLIAmount = 0.0M;
                        }

                        //****************Option Factors Values Rounding begins**********************************//
                        ldecBenefitOptionIncreaseorDecrease = Math.Round(ldecBenefitOptionIncreaseorDecrease, 2, MidpointRounding.AwayFromZero);
                        ldecInterimWithoutPLSO = Math.Round(ldecInterimWithoutPLSO, 2, MidpointRounding.AwayFromZero);
                        ldecGrossAdjustedAmount = Math.Round(ldecGrossAdjustedAmount, 2, MidpointRounding.AwayFromZero);
                        ldecBeforeSSLIAmount = Math.Round(ldecBeforeSSLIAmount, 2, MidpointRounding.AwayFromZero);
                        ldecAfterSSLIAmount = Math.Round(ldecAfterSSLIAmount, 2, MidpointRounding.AwayFromZero);
                        ldecTotAdjustedAmount = ldecGrossAdjustedAmount;
                        //****************Option Factors Values Rounding Ends**********************************//
                    }

                    lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_increase_or_decrease =
                                    ldecBenefitOptionIncreaseorDecrease;

                    lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount = ldecInterimWithoutPLSO;

                    if (icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes)
                    {
                        lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_with_plso = ldecGrossAdjustedAmount;
                    }
                    lobjMemberBenefitOption.icdoBenefitCalculationOptions.before_ssli_amount = ldecBeforeSSLIAmount;
                    if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment) ||//to handle when RecalculateBenefit is clicked from PA
                        (!string.IsNullOrEmpty(icdoBenefitCalculation.benefit_option_value)))
                    {
                        icdoBenefitCalculation.final_monthly_benefit = Math.Round(ldecBeforeSSLIAmount, 2, MidpointRounding.AwayFromZero);
                    }

                    if (icdoBenefitCalculation.uniform_income_or_ssli_flag == busConstant.Flag_Yes)
                    {
                        lobjMemberBenefitOption.icdoBenefitCalculationOptions.after_ssli_amount =
                                    ldecAfterSSLIAmount;
                    }

                }
                else
                {
                    //Reduced Benefit Option Amount is only for Main,LE and Judges and Hence it would be Rounding to 2 decimal places.
                    lobjMemberBenefitOption.icdoBenefitCalculationOptions.before_ssli_amount =
                        Math.Round(icdoBenefitCalculation.reduced_benefit_option_amount, 2, MidpointRounding.AwayFromZero);
                    lobjMemberBenefitOption.icdoBenefitCalculationOptions.after_ssli_amount =
                        Math.Round(icdoBenefitCalculation.reduced_benefit_option_amount, 2, MidpointRounding.AwayFromZero);
                    lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount =
                        Math.Round(icdoBenefitCalculation.reduced_benefit_option_amount, 2, MidpointRounding.AwayFromZero);
                    //lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_with_plso = 
                    //Math.Round(icdoBenefitCalculation.reduced_benefit_option_amount, 2, MidpointRounding.AwayFromZero);
                    icdoBenefitCalculation.final_monthly_benefit =
                        Math.Round(icdoBenefitCalculation.reduced_benefit_option_amount, 2, MidpointRounding.AwayFromZero);
                }

                //UAT PIR: 925 Get Factors for Graduated benefit Option
                decimal ldecGraduatedBenefitFactor = 0.0M;
                if ((icdoBenefitCalculation.graduated_benefit_option_value == busConstant.GraduatedBenefit1PerDecrease) ||
                    (icdoBenefitCalculation.graduated_benefit_option_value == busConstant.GraduatedBenefit2PerDecrease))
                {
                    if (icdoBenefitCalculation.is_return_to_work_member)
                    {
                        ldecGraduatedBenefitFactor = icdoBenefitCalculation.rtw_graduated_benefit_factor;
                    }
                    else
                    {
                        ldecGraduatedBenefitFactor = GetGraduatedBenefitFactor(lobjProvisionOption.icdoBenefitProvisionBenefitOption.benefit_option_value, ldecMemberAge,
                            ldecBeneficiaryAge, icdoBenefitCalculation.graduated_benefit_option_value);
                    }

                    if (ldecGraduatedBenefitFactor > 0.0M)
                    {
                        lobjMemberBenefitOption.icdoBenefitCalculationOptions.graduated_benefit_factor = ldecGraduatedBenefitFactor;
                        lobjMemberBenefitOption.icdoBenefitCalculationOptions.graduated_benefit_option_value = icdoBenefitCalculation.graduated_benefit_option_value;
                        lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount = Math.Round(lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount *
                            ldecGraduatedBenefitFactor, 2, MidpointRounding.AwayFromZero);
                        lobjMemberBenefitOption.icdoBenefitCalculationOptions.graduated_benefit_option_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1913, lobjMemberBenefitOption.icdoBenefitCalculationOptions.graduated_benefit_option_value);

                        lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_increase_or_decrease =
                                    lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount - icdoBenefitCalculation.reduced_monthly_after_plso_deduction;

                        icdoBenefitCalculation.final_monthly_benefit = lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount;
                    }
                }

                ldecMemberFinalMonthlyAmount = lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount;
                if (icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes)
                {
                    ldecMemberFinalMonthlyAmount = lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_with_plso;
                }
                if (icdoBenefitCalculation.uniform_income_or_ssli_flag == busConstant.Flag_Yes)
                {
                    ldecMemberFinalMonthlyAmount = lobjMemberBenefitOption.icdoBenefitCalculationOptions.before_ssli_amount;
                }

                lintCounter = lintCounter + 1;
                if (lintCounter == 1)
                    idecMASBenefitAmount = ldecMemberFinalMonthlyAmount; // To display in MAS 

                // Find the Taxable and non taxable Monthly Components
                decimal ldecNontaxableMonthly = 0.0M;
                decimal ldecTaxableMonthly = 0.0M;
                decimal ldecExclusionAmount = 0.0M;

                GetMonthlyBenefitTaxableComponents(lobjProvisionOption.icdoBenefitProvisionBenefitOption.exclusive_calc_payment_type_value,
                    ldecPostTaxContributionAmount, ldecMemberFinalMonthlyAmount,
                    ref ldecNontaxableMonthly, ref ldecTaxableMonthly, ref ldecExclusionAmount);

                lobjMemberBenefitOption.icdoBenefitCalculationOptions.taxable_amount = ldecTaxableMonthly;
                lobjMemberBenefitOption.icdoBenefitCalculationOptions.non_taxable_amount = ldecNontaxableMonthly;

                lobjMemberBenefitOption.ibusBenefitCalculation = new busBenefitCalculation();
                lobjMemberBenefitOption.ibusBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
                lobjMemberBenefitOption.ibusBenefitCalculation.icdoBenefitCalculation = icdoBenefitCalculation;
                if ((lobjMemberBenefitOption.icdoBenefitCalculationOptions.option_factor == 0M) && (icdoBenefitCalculation.reduced_benefit_option_amount == 0M))
                    iblnIsBenefitOptionFactorNotRetrieved = true;
                else
                    lclbBenefitCalcOptions.Add(lobjMemberBenefitOption);

                // Load Spouse Options
                if ((ablnJointAnnuitantExists) &&
                    (lobjProvisionOption.icdoBenefitProvisionBenefitOption.spouse_factor != 0M))
                {
                    busBenefitCalculationOptions lobjSpouseOptions = new busBenefitCalculationOptions();
                    lobjSpouseOptions.icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions();
                    lobjSpouseOptions.ibusBenefitCalculationPayee = new busBenefitCalculationPayee();
                    lobjSpouseOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
                    lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_provision_benefit_option_id =
                        lobjProvisionOption.icdoBenefitProvisionBenefitOption.benefit_provision_benefit_option_id;
                    foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                    {
                        if (lobjPayee.icdoBenefitCalculationPayee.payee_person_id != icdoBenefitCalculation.person_id)
                        {
                            lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_calculation_payee_id =
                                lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id;
                            lobjSpouseOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_value =
                                lobjPayee.icdoBenefitCalculationPayee.family_relationship_value;
                            lobjSpouseOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_description =
                                lobjPayee.icdoBenefitCalculationPayee.family_relationship_description;
                            lobjSpouseOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value =
                                lobjPayee.icdoBenefitCalculationPayee.account_relationship_value;
                            lobjSpouseOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_description =
                                lobjPayee.icdoBenefitCalculationPayee.account_relationship_description;
                        }
                    }
                    lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_option_amount = lobjProvisionOption.icdoBenefitProvisionBenefitOption.spouse_factor *
                                                                lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount;
                    lobjSpouseOptions.icdoBenefitCalculationOptions.option_factor = lobjProvisionOption.icdoBenefitProvisionBenefitOption.spouse_factor;


                    //UAT PIR:925 Calculation for Spouse Graduated benefit Option
                    if ((icdoBenefitCalculation.graduated_benefit_option_value == busConstant.GraduatedBenefit1PerDecrease) ||
                        (icdoBenefitCalculation.graduated_benefit_option_value == busConstant.GraduatedBenefit2PerDecrease))
                    {
                        //PIR 24881 Graduated Benefit option need to be considered in the templates "PAY-4018" & "PAY-4014" 
                        lobjSpouseOptions.icdoBenefitCalculationOptions.graduated_benefit_factor = lobjProvisionOption.icdoBenefitProvisionBenefitOption.spouse_factor;
                        lobjSpouseOptions.icdoBenefitCalculationOptions.graduated_benefit_option_value = icdoBenefitCalculation.graduated_benefit_option_value;
                        lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_option_amount = Math.Round(lobjMemberBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount *
                                lobjProvisionOption.icdoBenefitProvisionBenefitOption.spouse_factor, 2, MidpointRounding.AwayFromZero);
                        lobjSpouseOptions.icdoBenefitCalculationOptions.graduated_benefit_option_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1913, lobjSpouseOptions.icdoBenefitCalculationOptions.graduated_benefit_option_value);
                    }

                    ldecSpouseFinalMonthlyAmount = lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_option_amount;
                    if (IsJobService)
                    {
                        lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_option_amount =
                            Slice(lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_option_amount, 2);

                        if (icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes)
                        {
                            lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_with_plso = Slice(ldecTotAdjustedAmount *
                                lobjProvisionOption.icdoBenefitProvisionBenefitOption.spouse_factor, 2);
                            ldecSpouseFinalMonthlyAmount = lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_with_plso;
                        }
                    }
                    else
                    {
                        lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_option_amount =
                                    Math.Round(lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_option_amount, 2, MidpointRounding.AwayFromZero);

                        if (icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes)
                        {
                            lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_with_plso = Math.Round(ldecTotAdjustedAmount *
                                lobjProvisionOption.icdoBenefitProvisionBenefitOption.spouse_factor, 2, MidpointRounding.AwayFromZero);
                            ldecSpouseFinalMonthlyAmount = lobjSpouseOptions.icdoBenefitCalculationOptions.benefit_with_plso;
                        }
                    }

                    if (icdoBenefitCalculation.uniform_income_or_ssli_flag == busConstant.Flag_Yes)
                    {
                        lobjSpouseOptions.icdoBenefitCalculationOptions.before_ssli_amount =
                        lobjProvisionOption.icdoBenefitProvisionBenefitOption.spouse_factor *
                                                                lobjMemberBenefitOption.icdoBenefitCalculationOptions.before_ssli_amount;

                        lobjSpouseOptions.icdoBenefitCalculationOptions.after_ssli_amount =
                        lobjProvisionOption.icdoBenefitProvisionBenefitOption.spouse_factor *
                                                                lobjMemberBenefitOption.icdoBenefitCalculationOptions.after_ssli_amount;
                        if (IsJobService)
                        {
                            lobjSpouseOptions.icdoBenefitCalculationOptions.before_ssli_amount =
                                Slice(lobjSpouseOptions.icdoBenefitCalculationOptions.before_ssli_amount, 2);

                            lobjSpouseOptions.icdoBenefitCalculationOptions.after_ssli_amount =
                                Slice(lobjSpouseOptions.icdoBenefitCalculationOptions.after_ssli_amount, 2);

                        }
                        else
                        {
                            lobjSpouseOptions.icdoBenefitCalculationOptions.before_ssli_amount =
                                Math.Round(lobjSpouseOptions.icdoBenefitCalculationOptions.before_ssli_amount, 2, MidpointRounding.AwayFromZero);

                            lobjSpouseOptions.icdoBenefitCalculationOptions.after_ssli_amount =
                                Math.Round(lobjSpouseOptions.icdoBenefitCalculationOptions.after_ssli_amount, 2, MidpointRounding.AwayFromZero);
                        }
                        ldecSpouseFinalMonthlyAmount = lobjSpouseOptions.icdoBenefitCalculationOptions.before_ssli_amount;
                    }

                    // Find the Taxable and non taxable Monthly Components
                    decimal ldecSpouseNontaxableMonthly = 0.0M;
                    decimal ldecSpouseTaxableMonthly = 0.0M;
                    decimal ldecSpouseExclusionAmount = 0.0M;

                    GetMonthlyBenefitTaxableComponents(lobjProvisionOption.icdoBenefitProvisionBenefitOption.exclusive_calc_payment_type_value,
                        ldecPostTaxContributionAmount, ldecSpouseFinalMonthlyAmount,
                        ref ldecSpouseNontaxableMonthly, ref ldecSpouseTaxableMonthly, ref ldecSpouseExclusionAmount);

                    lobjSpouseOptions.icdoBenefitCalculationOptions.taxable_amount = ldecSpouseTaxableMonthly;
                    lobjSpouseOptions.icdoBenefitCalculationOptions.non_taxable_amount = ldecSpouseNontaxableMonthly;

                    lobjSpouseOptions.ibusBenefitCalculation = new busBenefitCalculation();
                    lobjSpouseOptions.ibusBenefitCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
                    lobjSpouseOptions.ibusBenefitCalculation.icdoBenefitCalculation = icdoBenefitCalculation;
                    lobjSpouseOptions.LoadBenefitProvisionOption();
                    if (lobjSpouseOptions.icdoBenefitCalculationOptions.option_factor == 0M)
                        iblnIsBenefitOptionFactorNotRetrieved = true;
                    else
                        lclbBenefitCalcOptions.Add(lobjSpouseOptions);
                }

                //icdoBenefitCalculation.final_monthly_benefit =
                //                    Math.Round(icdoBenefitCalculation.final_monthly_benefit, 2, MidpointRounding.AwayFromZero);
            }
            return lclbBenefitCalcOptions;
        }

        public void LoadBenefitCalculationPayeeForNewMode()
        {
            if (ibusMember == null)
                LoadMember();
            iclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();
            // Member Record is added for both the case, Estimate and Final
            busBenefitCalculationPayee lobjBenefitCalculationPayee = new busBenefitCalculationPayee();
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.benefit_application_id = icdoBenefitCalculation.benefit_application_id;
            if (lobjBenefitCalculationPayee.ibusBenefitApplication == null)
                lobjBenefitCalculationPayee.LoadBenefitApplication();
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_person_id = icdoBenefitCalculation.person_id;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_date_of_birth = ibusMember.icdoPerson.date_of_birth;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_first_name = ibusMember.icdoPerson.first_name;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_last_name = ibusMember.icdoPerson.last_name;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_middle_name = ibusMember.icdoPerson.middle_name;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipMember;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, busConstant.AccountRelationshipMember);
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_value = busConstant.FamilyRelationshipMember;
            lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(321, busConstant.FamilyRelationshipMember);
            iclbBenefitCalculationPayee.Add(lobjBenefitCalculationPayee);
            // Joint Annuitant Spouse
            if (ibusJointAnnuitant == null)
                LoadJointAnnuitant();
            // First name is checked since the Joint Annuitant may not have PERSLink ID
            if ((!string.IsNullOrEmpty(ibusJointAnnuitant.icdoPerson.first_name)) &&
                (ibusJointAnnuitant.icdoPerson.date_of_death == DateTime.MinValue))
            {
                //UAT PIR:1774. Eventhough Joint Annuitant is Provided for Final Application,
                //Do not load it into payee grid if the option or rhic option do not support it.
                if (IsJointAnnuitantEligibleForPayee())
                {
                    busBenefitCalculationPayee lobjJointAnnuitant = new busBenefitCalculationPayee();
                    lobjJointAnnuitant.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.payee_person_id = ibusJointAnnuitant.icdoPerson.person_id;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.payee_date_of_birth = ibusJointAnnuitant.icdoPerson.date_of_birth;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.payee_first_name = ibusJointAnnuitant.icdoPerson.first_name; ;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.payee_last_name = ibusJointAnnuitant.icdoPerson.last_name;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.payee_middle_name = ibusJointAnnuitant.icdoPerson.middle_name;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipJointAnnuitant;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.account_relationship_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, busConstant.AccountRelationshipJointAnnuitant);
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.family_relationship_value = busConstant.FamilyRelationshipSpouse;
                    lobjJointAnnuitant.icdoBenefitCalculationPayee.family_relationship_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(321, busConstant.FamilyRelationshipSpouse);
                    iclbBenefitCalculationPayee.Add(lobjJointAnnuitant);
                }
            }
        }

        public bool IsJointAnnuitantEligibleForPayee()
        {
            if (ibusPlan.IsNull())
                LoadPlan();

            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) //PIR 19594
            {
                return true;
            }
            else
            {
                if (icdoBenefitCalculation.rhic_option_value != busConstant.RHICOptionStandard)
                {
                    return true;
                }

                busBenefitProvisionBenefitOption lobjbenefitProvisionBenefitOption = new busBenefitProvisionBenefitOption { icdoBenefitProvisionBenefitOption = new cdoBenefitProvisionBenefitOption() };
                lobjbenefitProvisionBenefitOption = GetAvailableOptionsDetails(
                                                        ibusPlan.icdoPlan.benefit_provision_id,
                                                        DateTime.Today,
                                                        icdoBenefitCalculation.benefit_account_type_value,
                                                        icdoBenefitCalculation.benefit_option_value,
                                                        icdoBenefitCalculation.uniform_income_or_ssli_flag).FirstOrDefault();
                if (lobjbenefitProvisionBenefitOption.IsNotNull())
                {
                    if (lobjbenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.spouse_factor > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override busBase GetCorPerson()
        {
            if (ibusBenefitDeductionSummary == null)
                LoadBenefitDeductionSummary();
            if (ibusMember == null)
                LoadMember();
            return ibusMember;
        }

        public override busBase GetCorOrganization()
        {
            DateTime ldteTempDate = new DateTime();
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            int lintOrgID = GetOrgIdAsLatestEmploymentOrgId(ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            busConstant.ApplicationBenefitTypeRetirement, ref ldteTempDate);

            ibusMember.ibusCurrentEmployment = new busPersonEmployment { ibusOrganization = new busOrganization() };
            ibusMember.ibusCurrentEmployment.ibusOrganization.FindOrganization(lintOrgID);
            return ibusMember.ibusCurrentEmployment.ibusOrganization;
        }

        // PIR ID 1392
        // Should throw a Warning message if there exists a Bulk contribution record exists for the Member.
        public bool IsBulkRecordExists()
        {
            // Check for VSC
            if ((ibusMember != null) && (ibusMember.icolPersonAccountByBenefitType != null))
            {
                foreach (busPersonAccount lobjPersonAccount in ibusMember.icolPersonAccountByBenefitType)
                {
                    if (lobjPersonAccount.iclbRetirementContributionAll != null)
                    {
                        foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lobjPersonAccount.iclbRetirementContributionAll)
                        {
                            if ((lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value ==
                                                busConstant.TransactionTypeRegularPayroll) &&
                                (lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit > 1M))
                                return true;
                        }
                    }
                }
            }

            // Check for PSC
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.iclbRetirementContributionAll == null)
                ibusPersonAccount.LoadRetirementContributionAll();
            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in ibusPersonAccount.iclbRetirementContributionAll)
            {
                if ((lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value ==
                                        busConstant.TransactionTypeRegularPayroll) &&
                    (lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit > 1M))
                    return true;
            }
            return false;
        }

        public decimal idec20PercentageMemberAccountBalance
        {
            get
            {
                return icdoBenefitCalculation.member_account_balance * 0.2M;
            }
        }

        // This method is called in New Estimate calculation to load the Person Account Grid.
        public void LoadPersonPlanAccounts()
        {
            int lintPreRTWPayeeAccountID = 0;
            bool lblnIsRTWMember = false;

            if (iclbBenefitCalculationPersonAccount.IsNull())
                iclbBenefitCalculationPersonAccount = new Collection<busBenefitCalculationPersonAccount>();
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            if (ibusMember.IsNull())
                LoadMember();
            if (ibusMember.iclbRetirementAccount.IsNull())
                ibusMember.LoadRetirementAccount();

            ibusPersonAccount.LoadTotalPSC(DateTime.Now);


            lblnIsRTWMember = ibusMember.IsRTWMember(icdoBenefitCalculation.plan_id, busConstant.PayeeStatusForRTW.SuspendedOnly, ref lintPreRTWPayeeAccountID);
            if ((lblnIsRTWMember) && lintPreRTWPayeeAccountID != 0)
            {
                if (ibusPersonAccount.icdoPersonAccount.Total_PSC >= 24)
                {
                    icdoBenefitCalculation.is_rtw_less_than_2years_flag = busConstant.Flag_No;
                }
                else
                {
                    icdoBenefitCalculation.is_rtw_less_than_2years_flag = busConstant.Flag_Yes;
                }

                icdoBenefitCalculation.pre_rtw_payee_account_id = lintPreRTWPayeeAccountID;
                icdoBenefitCalculation.rtw_refund_election_value = busConstant.Flag_No_Value.ToUpper(); // Default
                LoadPreRTWPayeeAccount();

                var lFiltered = ibusMember.iclbRetirementAccount.Where(
                        o => o.icdoPersonAccount.plan_id == icdoBenefitCalculation.plan_id &&
                        o.icdoPersonAccount.person_account_id != ibusPersonAccount.icdoPersonAccount.person_account_id &&
                        o.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn); // PIR ID 1701

                // The collection will have only Person Accounts whose Payee Account status is in Suspended.
                foreach (busPersonAccount lobjPersonAccount in lFiltered)
                {
                    busBenefitCalculationPersonAccount lobjBenCalcPersonAccount = new busBenefitCalculationPersonAccount
                    {
                        icdoBenefitCalculationPersonAccount = new cdoBenefitCalculationPersonAccount()
                    };
                    lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.person_account_id = lobjPersonAccount.icdoPersonAccount.person_account_id;
                    lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.payee_account_id = ibuspre_RTW_Payee_Account.icdoPayeeAccount.payee_account_id;
                    lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_calculation_person_account_flag = busConstant.Flag_No;
                    if (lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.payee_account_id > 0)
                    {
                        lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.istrUse = busConstant.Flag_Yes_Value;
                    }
                    lobjBenCalcPersonAccount.LoadPersonAccount();
                    lobjBenCalcPersonAccount.ibusPersonAccount.LoadPlan();
                    lobjPersonAccount.LoadTotalPSC(DateTime.Now);
                    /*if (icdoBenefitCalculation.is_rtw_less_than_2years_flag == busConstant.Flag_No)*/
                    lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag = busConstant.Flag_Yes;
                    /*else
                        lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag = busConstant.Flag_No;*/
                    iclbBenefitCalculationPersonAccount.Add(lobjBenCalcPersonAccount);
                }
            }
        }

        // BR-060-31,32,33
        public bool IsValidReducedBenefitOption()
        {
            if (icdoBenefitCalculation.reduced_benefit_flag == busConstant.Flag_Yes)
            {
                if (icdoBenefitCalculation.is_return_to_work_member)
                {
                    if (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_No_Value.ToUpper())
                    {
                        if (ibuspre_RTW_Payee_Account == null)
                            LoadPreRTWPayeeAccount();
                        if (ibuspre_RTW_Payee_Account.ibusApplication == null)
                            ibuspre_RTW_Payee_Account.LoadApplication();
                        if (ibuspre_RTW_Payee_Account.ibusApplication.icdoBenefitApplication.reduced_benefit_flag == busConstant.Flag_Yes)
                            return false;
                    }
                }
            }
            return true;
        }

        // BR-060-20
        public bool IsBothBenefitOptionSameForRTW()
        {
            if (icdoBenefitCalculation.is_return_to_work_member)
            {
                if (ibuspre_RTW_Payee_Account == null)
                    LoadPreRTWPayeeAccount();
                if (icdoBenefitCalculation.benefit_option_value == ibuspre_RTW_Payee_Account.icdoPayeeAccount.benefit_option_value)
                {
                    if (icdoBenefitCalculation.actuarial_benefit_reduction != 0)
                        return true;
                }
            }
            return false;
        }

        // BR-060-20
        public bool IsBothBenefitOptionNotSameForRTW()
        {
            // UAT PIR ID 1238
            // Validation 2094 & 2099 is disabled and added a new Warning 2014
            if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) && //PIR 19594
                (icdoBenefitCalculation.is_return_to_work_member))
            {
                if (ibuspre_RTW_Payee_Account == null)
                    LoadPreRTWPayeeAccount();
                if ((icdoBenefitCalculation.benefit_option_value.IsNullOrEmpty()) ||
                    (icdoBenefitCalculation.benefit_option_value != ibuspre_RTW_Payee_Account.icdoPayeeAccount.benefit_option_value))
                    return true;
            }
            return false;
        }

        // SYSTEST-PIR-ID-1702
        // If RTW Refund is Selected as 'Y' and no Person Account is selected throw this exception.
        public bool IsValidRTWRefundOption()
        {
            if (icdoBenefitCalculation.is_return_to_work_member)
            {
                /// UAT PIR ID 1212 : This Validation is no more Valid.
                /// All Validations related to RTW_Refun_Election_Value is disabled.
                if (icdoBenefitCalculation.rtw_refund_election_value.IsNotNull())
                {
                    if (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper())
                    {
                        if (iclbBenefitCalculationPersonAccount.IsNotNull())
                        {
                            var lclbBenCalcPA = iclbBenefitCalculationPersonAccount.Where(
                                lobjBenCalcPA => lobjBenCalcPA.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag == busConstant.Flag_Yes);
                            if (lclbBenCalcPA.Count() == 0)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        //Used only this Calculation.
        public int GetRTWRefundBenefitAccountID()
        {
            if (ibusMember == null)
                LoadMember();
            DataTable ldtbResult;
            ldtbResult = busBase.Select<cdoPayeeAccount>(new string[3] { "PAYEE_PERSLINK_ID", "APPLICATION_ID", "BENEFIT_ACCOUNT_TYPE_VALUE" },
                                                            new object[3] { ibusMember.icdoPerson.person_id, icdoBenefitCalculation.benefit_application_id, busConstant.ApplicationBenefitTypeRefund }, null, null);

            foreach (DataRow ldr in ldtbResult.Rows)
            {
                busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lobjPayeeAccount.icdoPayeeAccount.LoadData(ldr);
                lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsNotNull())
                {
                    if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value != busConstant.PayeeAccountStatusRefundCancelled)
                    {
                        return lobjPayeeAccount.icdoPayeeAccount.benefit_account_id;
                    }
                }
            }
            return 0;
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
                new object[1] { icdoBenefitCalculation.pre_rtw_payee_account_id }, null, null);
            iclbRecovery = new Collection<busPaymentRecovery>();
            iclbRecovery = GetCollection<busPaymentRecovery>(ldtRecovery, "icdoPaymentRecovery");
        }

        /// <summary>
        /// Method to update Recovery with new payee account id and calculation id
        /// </summary>
        /// <param name="aintPayeeAccountID">New Payee account ID</param>
        public void UpdatePayeeAccountIDAndCalculationID(int aintPayeeAccountID)
        {
            if (iclbRecovery == null)
                LoadPaymentRecovery();
            foreach (busPaymentRecovery lobjRecovery in iclbRecovery)
            {
                if (lobjRecovery.icdoPaymentRecovery.status_value != busConstant.RecoveryStatusCancel &&
                    lobjRecovery.icdoPaymentRecovery.status_value != busConstant.RecoveryStatusSatisfied &&
                    lobjRecovery.icdoPaymentRecovery.status_value != busConstant.RecoveryStatusWriteOff)
                {
                    lobjRecovery.icdoPaymentRecovery.payee_account_id = aintPayeeAccountID;
                    lobjRecovery.icdoPaymentRecovery.calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjRecovery.icdoPaymentRecovery.Update();
                }
            }
        }

        #endregion

        //PIR - 1954
        //is disbaility application then
        //check if this person is having a receiving payee acocunt for Early retirement application
        public bool IsCancelledPayeeAccountNotExits()
        {
            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                if (ibusMember.iclbBenefitApplication.IsNull())
                    ibusMember.LoadBenefitApplication();
                if (ibusMember.iclbPayeeAccount.IsNull())
                    ibusMember.LoadPayeeAccount(true);
                var lenumEarlyRetApplication = ibusMember.iclbBenefitApplication.Where(lobjBA => 
                    lobjBA.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement && 
                    lobjBA.icdoBenefitApplication.benefit_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly);

                // PROD PIR ID 5356
                if (lenumEarlyRetApplication.IsNotNull())
                {
                    foreach (busBenefitApplication lobjApplication in lenumEarlyRetApplication)
                    {
                        if (ibusMember.iclbPayeeAccount.Where(lobjPA =>
                                lobjPA.icdoPayeeAccount.application_id == lobjApplication.icdoBenefitApplication.benefit_application_id &&
                                lobjPA.ibusPayeeAccountActiveStatus.IsStatusNotCancelled()).Any())
                            return true;
                    }
                }
            }
            return false;
        }

        public void CalculateUnRedecuedBenefitAmountForPensionFileBatch(busDBCacheData abusDBCacheData = null)
        {
            if (!iblnBenefitMultiplierLoaded)
                CalculateBenefitMultiplier();

            // Calculate the amount to  be reduce in case of Disability.
            decimal adecReducedAmount1 = 0.0M;
            decimal adecReducedAmount2 = 0.0M;
            if ((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
                ((icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Judges) ||
                (icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Highway_Patrol)))
            {
                LoadSsliAndWsiAmount();
                adecReducedAmount2 = idecWSIBenefitAmount;
                if (icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Judges)
                {
                    adecReducedAmount1 = idecSSliBenefitAmount;
                }
            }

            // ********* CALCULATE UNREDUCED BENEFIT AMOUNT *********
            icdoBenefitCalculation.unreduced_benefit_amount =
                                                        CalculateUnReducedMonthlyBenefitAmount(
                                                        icdoBenefitCalculation.calculation_final_average_salary,
                                                        idecFASBenefitMultiplier, icdoBenefitCalculation.plan_id,
                                                        icdoBenefitCalculation.benefit_account_type_value,
                                                        adecReducedAmount1, adecReducedAmount2,
                                                        ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_formula_value);

            // For Disability case, the minimum benefit amount should be $100.
            // PIR 9021 Effective 4/1/2012, the following Business Rule has been eliminated.
            //if ((icdoBenefitCalculation.unreduced_benefit_amount < 100) &&
            //    (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
            //    (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_formula_value != busConstant.BenefitFormulaValueOther))
            //{
            //    icdoBenefitCalculation.unreduced_benefit_amount = 100;
            //}
        }

        /// This Method calculated the Retirement benefit for the Member Annual Statement
        public void CalculateMASRetirementBenefit(busDBCacheData abusDBCacheData = null)
        {
            bool lblnIsJointannuitantExists = false;
            // PIR ID 1119
            if (ibusJointAnnuitant.icdoPerson.date_of_birth != DateTime.MinValue && ibusJointAnnuitant.icdoPerson.date_of_death == DateTime.MinValue)
                lblnIsJointannuitantExists = true;
            DateTime ldtMemberDateOfBirth = DateTime.MinValue;
            if (ibusMember != null)
            {
                ldtMemberDateOfBirth = ibusMember.icdoPerson.date_of_birth;
            }
            CalculateMemberAge();
            if (!iblnBenefitMultiplierLoaded)
                CalculateBenefitMultiplier();

            //Load RTW Payee Option 
            if (icdoBenefitCalculation.pre_rtw_payee_account_id > 0)
            {
                if (ibuspre_RTW_Payee_Account.IsNull())
                    LoadPreRTWPayeeAccount();

                icdoBenefitCalculation.benefit_option_value = ibuspre_RTW_Payee_Account.icdoPayeeAccount.benefit_option_value;
            }

            // Calculate the amount to  be reduce in case of Disability.
            decimal adecReducedAmount1 = 0.0M;
            decimal adecReducedAmount2 = 0.0M;
            if ((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
                ((icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Judges) ||
                (icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Highway_Patrol)))
            {
                LoadSsliAndWsiAmount();
                adecReducedAmount2 = idecWSIBenefitAmount;
                if (icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Judges)
                {
                    adecReducedAmount1 = idecSSliBenefitAmount;
                }
            }

            // ********* CALCULATE UNREDUCED BENEFIT AMOUNT *********
            icdoBenefitCalculation.unreduced_benefit_amount =
                                                        CalculateUnReducedMonthlyBenefitAmount(
                                                        icdoBenefitCalculation.calculation_final_average_salary,
                                                        idecFASBenefitMultiplier, icdoBenefitCalculation.plan_id,
                                                        icdoBenefitCalculation.benefit_account_type_value,
                                                        adecReducedAmount1, adecReducedAmount2,
                                                        ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_formula_value);

            // For Disability case, the minimum benefit amount should be $100.
            // PIR 9021 Effective 4/1/2012, the following Business Rule has been eliminated.
            //if ((icdoBenefitCalculation.unreduced_benefit_amount < 100) &&
            //    (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
            //    (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_formula_value != busConstant.BenefitFormulaValueOther))
            //{
            //    icdoBenefitCalculation.unreduced_benefit_amount = 100;
            //    icdoBenefitCalculation.reduced_monthly_after_dnro_early = 100;
            //    icdoBenefitCalculation.reduced_monthly_after_plso_deduction = 100;
            //}

            // Benefit Sub-Type            
            SetBenefitSubType();
            GetNormalEligibilityDetails(abusDBCacheData);

            decimal ldecMemberAgeMontAndYear = 0.00M;
            decimal ldecBenAgeMontAndYear = 0.00M;
            DateTime ldtSpouseDateBirth = DateTime.MinValue;
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();

            bool lblnSpouseAlreadyExists = false;

            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                if (lobjPayee.icdoBenefitCalculationPayee.payee_person_id != icdoBenefitCalculation.person_id)
                {
                    ldtSpouseDateBirth = lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth;
                    lblnSpouseAlreadyExists = true;
                }
            }

            if (ibusJointAnnuitant != null)
            {
                if (!lblnSpouseAlreadyExists)
                    ldtSpouseDateBirth = ibusJointAnnuitant.icdoPerson.date_of_birth;
            }

            icdoBenefitCalculation.reduced_monthly_after_dnro_early = icdoBenefitCalculation.unreduced_benefit_amount;

            // UCS-060 Return to Work
            if ((icdoBenefitCalculation.is_return_to_work_member) &&
                (icdoBenefitCalculation.actuarial_benefit_reduction != 0M))
            {
                icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit = Math.Round(icdoBenefitCalculation.unreduced_benefit_amount *
                                                                                            (icdoBenefitCalculation.actuarial_benefit_reduction / 100),
                                                                                            2, MidpointRounding.AwayFromZero);
                icdoBenefitCalculation.reduced_monthly_after_dnro_early = Math.Round(icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit, 2);
            }

            icdoBenefitCalculation.reduced_monthly_after_plso_deduction = icdoBenefitCalculation.reduced_monthly_after_dnro_early;
            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.retirement_date, ref ldecMemberAgeMontAndYear, 4);
                CalculatePersonAge(ldtSpouseDateBirth, icdoBenefitCalculation.retirement_date, ref ldecBenAgeMontAndYear, 4);
                // ********* CALCULATE DNRO BENEFIT AMOUNT *********
                if ((icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO) &&
                    (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.dnro_flag == busConstant.Flag_Yes))
                {
                    decimal ldecDNROMissedPaymentAmount = 0, ldecMonthlyActuarialIncrease = 0, ldecDNROBenefitAmount = 0, ldecHocAmount = 0;
                    CalculateDNROBenefitAmount(ldecMemberAgeMontAndYear, ldecBenAgeMontAndYear, icdoBenefitCalculation.reduced_monthly_after_dnro_early,
                                                    ldecHocAmount, ref ldecDNROMissedPaymentAmount,
                                                    ref ldecMonthlyActuarialIncrease, ref ldecDNROBenefitAmount);
                    icdoBenefitCalculation.reduced_monthly_after_dnro_early = ldecDNROBenefitAmount;
                }
                // ********* CALCULATE EARLY BENEFIT AMOUNT *********
                else if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
                {
                    decimal ldecEarlyReductionPercentage = 0.00M, ldecEarlyReductionAmount = 0.00M, ldecEarlyReducedMonthlyBenefitAmount = 0.00M;
                    if (!iblnWaiveEarlyReduction)
                    {
                        if (!iblnConsolidatedPSCLoaded)
                            CalculateConsolidatedPSC();

                        CalculateEARLYBenefitAmount(ref ldecEarlyReductionPercentage, ref ldecEarlyReductionAmount, icdoBenefitCalculation.reduced_monthly_after_dnro_early,
                                                    ref ldecEarlyReducedMonthlyBenefitAmount, ldecMemberAgeMontAndYear, ldecBenAgeMontAndYear);
                        icdoBenefitCalculation.reduced_monthly_after_dnro_early = ldecEarlyReducedMonthlyBenefitAmount;
                    }
                }
            }

            // ********* CALCULATE QDRO DEDUCTIONS *********
            icdoBenefitCalculation.reduced_monthly_after_dnro_early =
                    icdoBenefitCalculation.reduced_monthly_after_dnro_early - icdoBenefitCalculation.qdro_amount;

            icdoBenefitCalculation.reduced_monthly_after_plso_deduction = icdoBenefitCalculation.reduced_monthly_after_dnro_early;

            // ********* CALCULATE BENEFIT OPTIONS *********
            iclbBenefitCalculationOptions = CalculateBenefitAmountForOptionFactors(lblnIsJointannuitantExists, ldtSpouseDateBirth);

            // ********* CALCULATE RHIC OPTIONS *********
            string astrBenefitSubType = icdoBenefitCalculation.benefit_account_sub_type_value;
            iblnIsMemberRestrictedForRHIC = false;

            CalculatePersonAge(ldtMemberDateOfBirth, icdoBenefitCalculation.retirement_date, ref ldecMemberAgeMontAndYear, 4);

            if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdDC || icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025) && (astrBenefitSubType.IsNullOrEmpty())) //PIR 20232
            {
                IsMemberEligibleForDCPlan(ldecMemberAgeMontAndYear, ref astrBenefitSubType);
                if (astrBenefitSubType.IsNullOrEmpty())
                {
                    iblnIsMemberRestrictedForRHIC = true;
                }
            }
            if (!(iblnIsMemberRestrictedForRHIC))
            {
                icdoBenefitCalculation.standard_rhic_amount = CalculateStandardRHICAmount(
                                                    Math.Round(icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero),
                                                    ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor,
                                                    icdoBenefitCalculation.plan_id);
            }
            else
            {
                icdoBenefitCalculation.standard_rhic_amount = 0.0M;
            }

            icdoBenefitCalculation.rhic_factor_amount = ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor;
            icdoBenefitCalculation.unreduced_rhic_amount = icdoBenefitCalculation.standard_rhic_amount;

            // ********* CALCULATE RHIC OPTIONS *********
            decimal ldecRHICEARLYReductionAmount = 0.0M, ldecRHICAfterEARLYReduction = 0.0M;
            decimal ldecRHICEarlyReductionFactor = 0.0M;


            iclbBenefitRHICOption = CalculateBenefitAmountForRHICOptionFactor(
                                                    icdoBenefitCalculation.calculation_type_value,
                                                    icdoBenefitCalculation.benefit_account_type_value,
                                                    icdoBenefitCalculation.standard_rhic_amount, ldtMemberDateOfBirth,
                                                    ldtSpouseDateBirth, icdoBenefitCalculation.benefit_option_value,
                                                    iblnWaiveEarlyReduction, lblnIsJointannuitantExists,
                                                    astrBenefitSubType,
                                                    icdoBenefitCalculation.retirement_date, DateTime.Today,
                                                    icdoBenefitCalculation.rhic_option_value,
                                                    ref ldecRHICEARLYReductionAmount, ref ldecRHICAfterEARLYReduction, ref ldecRHICEarlyReductionFactor, true);
            icdoBenefitCalculation.rhic_early_reduction_factor = ldecRHICEarlyReductionFactor;
            //// ********* CALCULATE MINIMUM GUARANTEE AMOUNT *********
            CalculateMinimumGuaranteedMemberAccount();
        }

        // NRD Changes
        public bool IsValidTerminationDateForEstimate()
        {
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) //PIR 19594
            {
                // UAT PIR ID 1700
                if (!IsMemberDual())
                {
                    DateTime ldteActualEmployeeTerminationDate = new DateTime();
                    GetOrgIdAsLatestEmploymentOrgId(
                                        ibusPersonAccount.icdoPersonAccount.person_account_id,
                                        busConstant.ApplicationBenefitTypeRetirement,
                                        ref ldteActualEmployeeTerminationDate);
                    if (ldteActualEmployeeTerminationDate != DateTime.MinValue && 
                        icdoBenefitCalculation.termination_date != ldteActualEmployeeTerminationDate &&
                        icdoBenefitCalculation.tffr_calculation_method_value.IsNullOrEmpty()) // PROD PIR ID 6389
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Used in UCS-060 Correspondences
        //PIR: 1976: This property would return yes only when the old benefit option and the new benefit option is different.
        public string istrIsDifferentBenefitOption
        {
            get
            {
                if (ibuspre_RTW_Payee_Account.IsNull())
                    LoadPreRTWPayeeAccount();
                if (ibuspre_RTW_Payee_Account.icdoPayeeAccount.benefit_option_value != icdoBenefitCalculation.benefit_option_value)
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        public bool IsSSLIIncomeInValid()
        {
            //UAT PIR: 1641.For Joint Annuitants the SSLI After and before Amounts will be Zero. So Filtered to Check only for Member Alone.
            if (icdoBenefitCalculation.uniform_income_or_ssli_flag == busConstant.Flag_Yes)
            {
                if (iclbBenefitCalculationOptions.IsNotNull())
                {
                    foreach (busBenefitCalculationOptions lobjbenefitoptions in iclbBenefitCalculationOptions)
                    {
                        if (lobjbenefitoptions.ibusBenefitCalculationPayee.IsNull())
                            lobjbenefitoptions.LoadBenefitCalculationPayee();

                        if (lobjbenefitoptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value == busConstant.AccountRelationshipMember)
                        {
                            if ((lobjbenefitoptions.icdoBenefitCalculationOptions.after_ssli_amount < 100M) ||
                                (lobjbenefitoptions.icdoBenefitCalculationOptions.before_ssli_amount < 100M))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public string istrRetirementLongDate
        {
            get
            {
                return icdoBenefitCalculation.retirement_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        //PIR - 1394
        // if the eligibility changed then the system should verify rules
        //. and set the application status to Pending.
        public bool IsAnyApplicationsExistsPending()
        {
            if (ibusBenefitApplication.IsNull())
                LoadBenefitApplication();

            ibusBenefitApplication.ValidateHardErrors(utlPageMode.Update);
            if (ibusBenefitApplication.iarrErrors.Count > 0)
                return true;
            return false;
        }

        //PIR - 1394
        // if the eligibility changed then the system should verify rules
        //. and set the application status to Pending.
        public void UpdateApplicationStatusIfEligibilityChanges()
        {
            if (ibusBenefitApplication == null)
                LoadBenefitApplication();
            if (ibusBenefitApplication.iarrErrors.IsNull())
                ibusBenefitApplication.ValidateHardErrors(utlPageMode.Update);
            if (ibusBenefitApplication.iarrErrors.Count > 0)
            {
                ibusBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
                ibusBenefitApplication.icdoBenefitApplication.Update();
            }
        }

        /// <summary>
        /// uat pir - 1348 :- Method to update benefit receipt date in dro when calculation is approved
        /// </summary>
        private void UpdateDROBenefitReceiptDate()
        {
            if (ibusBenefitApplication == null)
                LoadBenefitApplication();
            if (ibusBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (ibusBenefitApplication.iclbBenefitDROApplication == null)
                    ibusBenefitApplication.LoadDROApplication();
                foreach (busBenefitDroApplication lobjBenefitDROAppilcation in ibusBenefitApplication.iclbBenefitDROApplication)
                {
                    if (!lobjBenefitDROAppilcation.IsDROApplicationCancelledOrDenied())
                    {
                        if (lobjBenefitDROAppilcation.icdoBenefitDroApplication.time_of_benefit_receipt_calc_value == busConstant.DROApplicationModelTimeOfBenfitRetirementDate)
                        {
                            lobjBenefitDROAppilcation.icdoBenefitDroApplication.benefit_receipt_date = ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                            lobjBenefitDROAppilcation.icdoBenefitDroApplication.Update();
                        }
                    }
                }
            }
        }

        public void SetPLSONonTaxableAmount(ref decimal adecPostTaxContributionAmount)
        {
            //Step:1 Find the Post Tax Contribution.
            //decimal ldecPostTaxContribution = 0M;
            //decimal ldecAccountOwnerStartingTaxableAmount = 0.0M;
            //decimal ldecAccountOwnerStartingNonTaxableAmount = 0.0M;
            //decimal ldecAccountOwnerLTDAccountBalance = 0M;
            decimal ldecLTDAccountBalance = 0M;
            decimal ldecTaxablePLSO = 0M;
            decimal ldecNonTaxablePLSO = 0M;
            decimal ldecExclusionRatio = 0M;


            if (ibusPersonAccount == null)
                LoadPersonAccount();

            //UAT PIR: 1131 For Estimates the MAB should be calculated as on Retirement date instead of Termination date.
            DateTime ldteTerminationDate = DateTime.MinValue;

            if (((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                || (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                && (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
            {
                ldteTerminationDate = icdoBenefitCalculation.retirement_date;
            }
            else
            {
                ldteTerminationDate = icdoBenefitCalculation.termination_date.GetLastDayofMonth(); //PROD PIR ID 4851
            }

            if (icdoBenefitCalculation.is_return_to_work_member && (!icdoBenefitCalculation.is_rtw_member_subsequent_retirement) &&
                icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeEstimateSubsequent) //PIR 19594 
            {
                if (iclbBenefitCalculationPersonAccount.IsNull())
                    LoadBenefitCalculationPersonAccount();
                foreach (busBenefitCalculationPersonAccount lobjBenCalcPersonAccount in iclbBenefitCalculationPersonAccount)
                {
                    if (lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag == busConstant.Flag_Yes)
                    {
                        if (lobjBenCalcPersonAccount.ibusPersonAccount.IsNull())
                            lobjBenCalcPersonAccount.LoadPersonAccount();
                        if (lobjBenCalcPersonAccount.ibusPersonAccount.ibusPersonAccountRetirement.IsNull())
                        {
                            lobjBenCalcPersonAccount.ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
                            lobjBenCalcPersonAccount.ibusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(
                                                lobjBenCalcPersonAccount.ibusPersonAccount.icdoPersonAccount.person_account_id);
                        }

                        lobjBenCalcPersonAccount.ibusPersonAccount.ibusPersonAccountRetirement.LoadLTDSummaryForCalculation(
                                                ldteTerminationDate,icdoBenefitCalculation.benefit_account_type_value);
                        if (lobjBenCalcPersonAccount.ibusPayeeAccount.IsNull())
                            lobjBenCalcPersonAccount.LoadPayeeAccount();
                        lobjBenCalcPersonAccount.ibusPayeeAccount.LoadPaymentDetails();
                        adecPostTaxContributionAmount += (lobjBenCalcPersonAccount.ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd -
                                                lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidnontaxableamount);
                    }
                }
            }
            if (ibusPersonAccount.ibusPersonAccountRetirement == null)
            {
                ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };
                ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
            }
            ibusPersonAccount.ibusPersonAccountRetirement.LoadLTDSummaryForCalculation(ldteTerminationDate, icdoBenefitCalculation.benefit_account_type_value);

            //ldecAccountOwnerStartingTaxableAmount = (ibusPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd < 0) ? 0M :
            //                            ibusPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd;
            //ldecAccountOwnerStartingNonTaxableAmount = (ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd < 0) ? 0M :
            //                                            ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd;
            //ldecAccountOwnerLTDAccountBalance = ibusPersonAccount.ibusPersonAccountRetirement.Total_Account_Balance_ltd;

            if ((!icdoBenefitCalculation.is_return_to_work_member) ||
                ((icdoBenefitCalculation.is_return_to_work_member) && (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_No_Value.ToUpper())))
            {
                adecPostTaxContributionAmount += ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd;
                ldecLTDAccountBalance += ibusPersonAccount.ibusPersonAccountRetirement.Total_Account_Balance_ltd;
            }

            // Setting of Variables for Benefit A/c Updation or Insertion
            if (icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) //PIR 19594
            {
                CalculateQDROAmount(false);
            }
            adecPostTaxContributionAmount = adecPostTaxContributionAmount - icdoBenefitCalculation.non_taxable_qdro_amount;

            if (icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes)
            {
                CalculatePLSOTaxComponents(icdoBenefitCalculation.plso_lumpsum_amount, adecPostTaxContributionAmount,
                                                        icdoBenefitCalculation.reduced_monthly_after_dnro_early, icdoBenefitCalculation.plso_factor, 0,
                                                        ref ldecNonTaxablePLSO, ref ldecTaxablePLSO, ref ldecExclusionRatio);
            }

            icdoBenefitCalculation.plso_exclusion_ratio = ldecExclusionRatio;
            icdoBenefitCalculation.non_taxable_plso = ldecNonTaxablePLSO;
        }

        public void GetMonthlyBenefitTaxableComponents(string astrExclusionCalculationPaymentValue, decimal adecPostTaxContribution, decimal ldecFinalMonthlyBenefit,
            ref decimal adecNontaxableAmount, ref decimal adecTaxableAmount, ref decimal adecExclusionRatioAmount)
        {
            if (icdoBenefitCalculation.benefit_account_type_value != busConstant.ApplicationBenefitTypeDisability)
            {
                CalculateMonthlyTaxComponents(icdoBenefitCalculation.created_date, idecMemberAgeBasedOnRetirementDate, icdoBenefitCalculation.annuitant_age,
                                                astrExclusionCalculationPaymentValue,
                                                adecPostTaxContribution, ldecFinalMonthlyBenefit, icdoBenefitCalculation.non_taxable_plso, 0,
                                                ref adecNontaxableAmount, ref adecTaxableAmount, ref adecExclusionRatioAmount, 0M);
            }
            else
            {
                adecNontaxableAmount = 0M; adecExclusionRatioAmount = 0M;
                adecTaxableAmount = ldecFinalMonthlyBenefit - adecNontaxableAmount;
            }
        }


        public bool IsMemberEligibleForDCPlan(decimal adecMemberAge, ref string astrbenefitsubtype)
        {
            if (ibusMember == null)
                LoadMember();

            int lintTempPlanId = 0;
            DateTime ldtEarlyRetirementDate = DateTime.MinValue;

            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                //UAT PIR:2077 Changes.             
                if (ibusMember.IsFormerDBPlanTransfertoDC(busConstant.PlanIdNG))
                {
                    lintTempPlanId = busConstant.PlanIdNG;
                }
                else if (ibusMember.IsFormerDBPlanTransfertoDC(busConstant.PlanIdMain))
                {
                    lintTempPlanId = busConstant.PlanIdMain;
                }
                else if (ibusMember.IsFormerDBPlanTransfertoDC(busConstant.PlanIdMain2020))//PIR 20232 
                {
                    lintTempPlanId = busConstant.PlanIdMain2020;
                }

                busPlan ibusTempPlan = new busPlan { icdoPlan = new cdoPlan() };
                ibusTempPlan.FindPlan(lintTempPlanId);
                //PIR 26282
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();

                if (lintTempPlanId > 0)
                {
                    ldtEarlyRetirementDate = GetEarlyRetirementDateBasedOnEarlyRetirement(ibusTempPlan.icdoPlan.plan_id, ibusTempPlan.icdoPlan.benefit_provision_id,
                       icdoBenefitCalculation.benefit_account_type_value, ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.credited_vsc, iobjPassInfo, ibusPersonAccount);

                    DateTime ldtNRDDate = GetNormalRetirementDateBasedOnNormalEligibility(ibusTempPlan.icdoPlan.plan_code, ibusTempPlan.icdoPlan.plan_id, ibusTempPlan.icdoPlan.benefit_provision_id);
                    icdoBenefitCalculation.normal_retirement_date = ldtNRDDate;
                    //Step:1
                    //Retirement Date is after NRD --> Unreduced RHIC should be applied --> no change in logic
                    if (icdoBenefitCalculation.retirement_date > ldtNRDDate)
                    {
                        astrbenefitsubtype = busConstant.ApplicationBenefitSubTypeNormal;
                        return true;
                    }
                    //Step:2
                    //Retirement Date is between Early Retirement age and NRD --> Calculate Reduced RHIC, create Payee Account, etc --> no change in logic
                    else if (busGlobalFunctions.CheckDateOverlapping(icdoBenefitCalculation.retirement_date, ldtEarlyRetirementDate, ldtNRDDate))
                    {
                        astrbenefitsubtype = busConstant.ApplicationBenefitSubTypeEarly;
                        return true;
                    }
                    //Step:3
                    //Retirement Date is prior to Early Retirement age--> Calculate Reduced RHIC, create Payee Account, create RHIC record.  
                    else if (icdoBenefitCalculation.retirement_date < ldtEarlyRetirementDate)
                    {
                        astrbenefitsubtype = busConstant.ApplicationBenefitSubTypeEarly;
                        return true;
                    }
                }

            }
            else if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                if (IsNumberOfEmploymentDays180())
                {
                    astrbenefitsubtype = busConstant.ApplicationBenefitSubTypeDisability;
                    return true;
                }
            }
            /***********************Commented Code not Removed Intentionally for Future FollowUp Purpose ***********************/
            //UAT PIR: 1170, 1164 RHIC to be calculated and NRD to be displayed only when they are eligible for Main Early or Normal        
            //For Disability they have to be eligible for Disability. ie 180 Days of employment.
            //if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            //{
            //    if ((adecMemberAge >= 65) || (IsMemberSatisfyRuleOf85()))
            //    {
            //        astrbenefitsubtype = busConstant.ApplicationBenefitSubTypeNormal;
            //        return true;
            //    }
            //    else if ((adecMemberAge >= 55))
            //    {
            //        astrbenefitsubtype = busConstant.ApplicationBenefitSubTypeEarly;
            //        return true;
            //    }
            //}
            //else if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            //{
            //    if (IsNumberOfEmploymentDays180())
            //    {
            //        astrbenefitsubtype = busConstant.ApplicationBenefitSubTypeDisability;
            //        return true;
            //    }
            //}
            return false;
        }

        //UAT PIR:2190.Gross Benefit on Payee Acct not matching calculation
        //Property Set in order to Avoid Recalculating.
        public void SetAdjustedMonthlySingleLifeBenefitAmount()
        {
            //It is nothing but ReducedMonthlyAfterEarlyorDNRO
            /* reduced_monthly_after_dnro_early = Unreduced Amount
             * IF RTW then it is Actuarially Adjusted Amount
             * IF Early then it is Reduced by Early Decrease 
             * For DNRO it is increased by DNRO Increase
             * Then Finally Reduced by QDRo
            */

            icdoBenefitCalculation.reduced_monthly_after_dnro_early = icdoBenefitCalculation.unreduced_benefit_amount;

            if (icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit > 0)
            {
                icdoBenefitCalculation.reduced_monthly_after_dnro_early = icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit;
            }

            if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
            {
                icdoBenefitCalculation.reduced_monthly_after_dnro_early = icdoBenefitCalculation.reduced_monthly_after_dnro_early - icdoBenefitCalculation.early_monthly_decrease;
            }
            else if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO)
            {
                //UAT: PIR: 1687 dnro_monthly_increase being set 
                icdoBenefitCalculation.reduced_monthly_after_dnro_early = icdoBenefitCalculation.reduced_monthly_after_dnro_early + icdoBenefitCalculation.dnro_monthly_increase;
            }

            if (icdoBenefitCalculation.qdro_amount > 0)
            {
                icdoBenefitCalculation.reduced_monthly_after_dnro_early = icdoBenefitCalculation.reduced_monthly_after_dnro_early - icdoBenefitCalculation.qdro_amount;
            }
        }

        /// UAT PIR 925
        /// For RTW Member, Both values should be empty or notempty.
        public bool IsGraduatedOptionSelectedValid()
        {
            if (icdoBenefitCalculation.is_return_to_work_member)
            {
                if (icdoBenefitCalculation.graduated_benefit_option_value.IsNotNullOrEmpty())
                {
                    if (icdoBenefitCalculation.rtw_graduated_benefit_factor == 0M)
                        return false;
                }
                else
                {
                    if (icdoBenefitCalculation.rtw_graduated_benefit_factor != 0M)
                        return false;
                }
            }
            return true;
        }

        //UAT PIR:925
        //Method to Fetch the Graduated benefit Factor From the DB Cache table
        public decimal GetGraduatedBenefitFactor(string astrBenefitOption, decimal adecMemberAge, decimal adecJointAnnuitantAge, string astrGraduatedbenefitOptionValue)
        {
            busBenefitGraduatedBenefitOptionFactor lbusBenefitGraduatedBenefitOptionFactor = new busBenefitGraduatedBenefitOptionFactor { icdoBenefitGraduatedBenefitOptionFactor = new cdoBenefitGraduatedBenefitOptionFactor() };
            decimal ldecMemberAge = Slice(adecMemberAge, 0);
            decimal ldecBeneficiaryAge = Slice(adecJointAnnuitantAge, 0);
            DateTime ldteRetirementDate = (icdoBenefitCalculation.retirement_date != DateTime.MinValue) ? icdoBenefitCalculation.retirement_date : DateTime.Today;

            DataTable ldtbResult = Select<cdoBenefitGraduatedBenefitOptionFactor>(
                                                new string[3] { "BENEFIT_OPTION_VALUE", "GRADUATED_BENEFIT_OPTION_VALUE", "PLAN_ID" },
                                                new object[3] { astrBenefitOption, astrGraduatedbenefitOptionValue, icdoBenefitCalculation.plan_id }, null, "EFFECTIVE_DATE DESC");

            if ((astrBenefitOption == busConstant.BenefitOption10YearCertain) ||
                (astrBenefitOption == busConstant.BenefitOptionSingleLife) ||
                (astrBenefitOption == busConstant.BenefitOption20YearCertain))
            {
                ldecBeneficiaryAge = 0;
            }

            if (ldecBeneficiaryAge == 0)
            {
                ldtbResult = ldtbResult.AsEnumerable().Where(i => i.Field<decimal>("MEMBER_AGE") == ldecMemberAge
                                                                                 && i.Field<DateTime>("EFFECTIVE_DATE") <= ldteRetirementDate).AsDataTable();
            }
            else
            {
                ldtbResult = ldtbResult.AsEnumerable().Where(i => i.Field<decimal>("MEMBER_AGE") == ldecMemberAge &&
                                                                                    (i.Field<decimal>("BEN_AGE") == ldecBeneficiaryAge)
                                                                                    && i.Field<DateTime>("EFFECTIVE_DATE") <= ldteRetirementDate).AsDataTable();
            }
            if (ldtbResult.Rows.Count > 0)
                lbusBenefitGraduatedBenefitOptionFactor.icdoBenefitGraduatedBenefitOptionFactor.LoadData(ldtbResult.Rows[0]);

            return lbusBenefitGraduatedBenefitOptionFactor.icdoBenefitGraduatedBenefitOptionFactor.factor;
        }
		//UAT PIR:2355 DNRO Adhoc Amount Change.
        public decimal FetchTotalCOLAorAdhocAmount(DateTime adtStartDate, DateTime adtEnddate, decimal adecBaseAmount)
        {
            //Step:1 For the Start date and End date Find all Requests            
            //Step:2 Check whether the Request that is pulled is eligible for the current Calculation or not.
            //Step:3 Loop through the Requests
            //step:4 Find the Adhoc and COLA AMount.        

            Collection<busPostRetirementIncreaseBatchRequest> iclbPostRetirementIncreaseBatchRequest = new Collection<busPostRetirementIncreaseBatchRequest>();

            DataTable ldtbRequests = Select("cdoPostRetirementIncreaseBatchRequest.GetCOLADHOCRequestFortheGivenPeriod",
                new object[5] { adtStartDate, adtEnddate, icdoBenefitCalculation.benefit_account_type_value, icdoBenefitCalculation.plan_id, busConstant.PayeeAccountAccountRelationshipOwner });

            iclbPostRetirementIncreaseBatchRequest = GetCollection<busPostRetirementIncreaseBatchRequest>(ldtbRequests, "icdoPostRetirementIncreaseBatchRequest");
            decimal ldecCOLAAmount = 0.0M;
            decimal ldecTotalCOLAorIncreaseAmount = 0.0M;
            foreach (busPostRetirementIncreaseBatchRequest lobjPostRetirementIncreaseBatchRequest in iclbPostRetirementIncreaseBatchRequest)
            {
                ldecCOLAAmount = CalculateCOLAorAdhocAmount(lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value == busConstant.PostRetirementIncreaseTypeValueCOLA ? true : false,
                    adecBaseAmount, lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount,
                    lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.increase_percentage,
                    lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.effective_date);
                adecBaseAmount += ldecCOLAAmount;
                ldecTotalCOLAorIncreaseAmount += ldecCOLAAmount;
            }
            return ldecTotalCOLAorIncreaseAmount;
        }

        public decimal CalculateCOLAorAdhocAmount(bool ablnIsForCOLA, decimal adecBaseAmount, decimal adecIncreaseFlatAmount, decimal adecIncreasePercentage,
            DateTime adtRequestEffectiveDate)
        {
            //load Base amount
            //Place holder to be verified
            //if (icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService)
            //{
            //adecBaseAmount+=icdoBenefitCalculation.paid_up_annuity_amount; // UAT PIR ID 1341;
            //}

            //load COLA/Adhoc increase
            decimal ldecMonthlyIncrease = 0.0M;
            int lintNumberOfMonths = 0;
            decimal ldecAdjustedAmount = 0.0M;

            if (adecIncreaseFlatAmount > 0)
            {
                ldecMonthlyIncrease = adecIncreaseFlatAmount;
            }
            else
            {
                ldecMonthlyIncrease = (adecBaseAmount * adecIncreasePercentage) / 100;
                ldecMonthlyIncrease = busGlobalFunctions.RoundToPenny(ldecMonthlyIncrease);
            }

            //only for COLA
            if (ablnIsForCOLA)
            {
                //now get adjusted COLA                   
                lintNumberOfMonths = busGlobalFunctions.DateDiffByMonth(icdoBenefitCalculation.retirement_date, adtRequestEffectiveDate);

                if (lintNumberOfMonths > 1)
                    lintNumberOfMonths = lintNumberOfMonths - 1;

                //check if the member dies within one year of retirement if so then joint annuitant pension benefits will be calculated
                //else member payee account will continue.

                bool blnAllowProration = false;
                /* UAT PIR: 2416 COLA Proration logic provided by Leon
                * 1)      Benefit Type is Retirement or Disability:     if the numbers of Months between Payee Account Benefit begin date and COLA Effective date is less than 12.
                2)      Benefit Type: Pre Retirement :  if the number of months between Payee Account Benefit Begin date and COLA Effective date is less than 12. 
                3)      Benefit Type: Post Retirement:  :   if the numbers of Member payments plus joint annuitant payments and the COLA Effective date are less than 12.             
                */                
                if (lintNumberOfMonths / 12 < 1)
                {
                    blnAllowProration = true;
                }
                


                if (blnAllowProration)
                {
                    ldecAdjustedAmount = ((ldecMonthlyIncrease * lintNumberOfMonths) / 12);
                }
                else
                {
                    ldecAdjustedAmount = ldecMonthlyIncrease;
                }
            }
            else
                ldecAdjustedAmount = ldecMonthlyIncrease;
            return ldecAdjustedAmount;
        }

		// UAT PIR ID 2355
        public bool IsDNROMissedPaymentAmountValid()
        {
            if (icdoBenefitCalculation.overridden_dnro_missed_payment_amount != 0M )
            {
                if (IsJobService)
                {
                    if (icdoBenefitCalculation.termination_date != DateTime.MinValue)
                        if (icdoBenefitCalculation.termination_date >= busPayeeAccountHelper.GetPERSLinkGoLiveDate())
                            return false;
                }
                else
                {
                    if (icdoBenefitCalculation.normal_retirement_date != DateTime.MinValue && icdoBenefitCalculation.termination_date != DateTime.MinValue)
                        if (busGlobalFunctions.GetMax(icdoBenefitCalculation.termination_date, icdoBenefitCalculation.normal_retirement_date) >= busPayeeAccountHelper.GetPERSLinkGoLiveDate())
                            return false;
                }
            }
            return true;
        }

        //PROD PIR ID 5867
        public string istrSuppressRHICAdjustmentWarning { get; set; }

        // PIR 10051
        public bool IsMemberHaveServicePurchase()
        {
            // Only for final Calculation
            if (icdoBenefitCalculation.benefit_application_id > 0)
            {
                if (ibusBenefitApplication.IsNull()) LoadBenefitApplication();
                return ibusBenefitApplication.CheckPersonHavingServiceWithStatusApprvdORInPaymentOrPend();
            }
            return false;
        }

        // PIR 11236
        public bool IsSSLIUniformIncomeApplicable()
        {
            if (icdoBenefitCalculation.retirement_date >= busConstant.SSLIUniformIncomeEffectiveRetirementDate)
            {
                if ((icdoBenefitCalculation.ssli_or_uniform_income_commencement_age > 0) || (icdoBenefitCalculation.uniform_income_or_ssli_flag == busConstant.Flag_Yes))
                {
                    if (icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService)
                        return true;
                    else
                        return false;
                }
            }
            else
                return true;

            return true;
        }
        
		
		//PIR 19594
        public busBenefitCalculation LoadPrevRTWBenefitCalculation()
        {
            busBenefitCalculation lobjBenCal = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            DataTable ldtbResult = busBase.Select("entBenefitCalculation.LoadPrevBenefitCalculationForRTW", new object[1] { icdoBenefitCalculation.person_id });
            if (ldtbResult.Rows.Count > 0)
                lobjBenCal.icdoBenefitCalculation.LoadData(ldtbResult.Rows[0]);
            return lobjBenCal;
        }

        //PIR 20233 - added a property For check plan is main2020 or dc2020
        public int iintIsMain2020orDC2020
        {
            get
            {
                return (icdoBenefitCalculation.plan_id == busConstant.PlanIdMain2020 || icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020) ? 1 : 0;
            }
        }
        public DateTime idtRetirementDateForPreRTWPayeeAccountRHICReduction => busConstant.RetirementDateForPreRTWPayeeAccountRHICReduction;
        public DateTime idtPreRTWPayeeAccountRetirementDate
        {
            get
            {
                DataTable ldtbResult = Select("entBenefitCalculation.LoadPrevBenefitCalculationForRTW", new object[1] { icdoBenefitCalculation.person_id });
                if (ldtbResult.Rows.Count > 0 && ldtbResult.Rows[0]["RETIREMENT_DATE"] !=DBNull.Value)
                    return Convert.ToDateTime(ldtbResult.Rows[0]["RETIREMENT_DATE"]);
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// PIR 23878
        /// </summary>
        /// <returns>if Employment End Date is greater than 12 months from System Management Date then return true else false</returns>
        public bool IsEmpEndDate12MonthGreaterThanSystemMgntDate()
        {           
            DateTime ldtBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount?.icdoPersonAccount?.person_account_id > 0)
            {
                    DateTime ldteEmploymentEndDate = new DateTime();
                    GetOrgIdAsLatestEmploymentOrgId(
                                        ibusPersonAccount.icdoPersonAccount.person_account_id,
                                        busConstant.ApplicationBenefitTypeDisability,
                                        ref ldteEmploymentEndDate);

                    return (ldteEmploymentEndDate != DateTime.MinValue && ldtBatchDate != DateTime.MinValue &&
                           (ldteEmploymentEndDate.AddMonths(12) < ldtBatchDate));
            }
            return false;
        }
    }
}
