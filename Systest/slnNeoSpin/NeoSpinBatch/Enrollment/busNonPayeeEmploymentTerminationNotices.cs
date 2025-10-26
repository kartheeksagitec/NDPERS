#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.CorBuilder;
using System.Linq;
#endregion

namespace NeoSpinBatch
{
    class busNonPayeeEmploymentTerminationNotices : busNeoSpinBatch
    {
        private bool iblnGenerateDB = false;
        private bool iblnGenerateDC = false;
        private bool iblnGenerateDeffComp = false;
        private bool iblnGenerateHealth = false;
        private bool iblnGenerateDental = false;
        private bool iblnGenerateVision = false;
        private bool iblnGenerateFlex = false;
        private string istrFileName = string.Empty;

        private busPersonAccount ibusDBPersonAccount = null;
        private busPersonAccount ibusDCPersonAccount = null;
        private busPersonAccount ibusDeffCompPersonAccount = null;
        private busPersonAccount ibusHealthPersonAccount = null;
        private busPersonAccount ibusDentalPersonAccount = null;
        private busPersonAccount ibusVisionPersonAccount = null;
        private busPersonAccount ibusFlexPersonAccount = null;

        public bool iblnIsMemberAccountBalanceltd { get; set; }
        public void GenerateCorrespondenceForNonPayeeTerminationNotice()
        {
            istrProcessName = "Non Payee Employee Termination Notice";
            // The Query will fetch the record where EmploymentEndDate is NOT Null and TerminationLetterSentFlag is LetterNotSent(LNS).
            idlgUpdateProcessLog("Creating Correspondence for Non Payee Employee Termination Notice", "INFO", istrProcessName);
            DataTable ldtResult = DBFunction.DBSelect("cdoPersonAccount.NonPayeeEmployeeTerminationLetter", new object[] { },
                                            iobjPassInfo.iconFramework,
                                            iobjPassInfo.itrnFramework);
            busBase lbusBase = new busBase();
            Collection<busPersonEmployment> lclbPersonEmployment = lbusBase.GetCollection<busPersonEmployment>(ldtResult, "icdoPersonEmployment");
            foreach (DataRow ldtr in ldtResult.Rows)
            {
                string lstrStatus = busConstant.TerminationLetterStatusNotRequired;
                // Reset the flags
                iblnGenerateDB = false;
                iblnGenerateDC = false;
                iblnGenerateHealth = false;
                iblnGenerateDental = false;
                iblnGenerateVision = false;
                iblnGenerateDeffComp = false;
                iblnGenerateFlex = false;

                ibusDBPersonAccount = null;
                ibusDCPersonAccount = null;
                ibusDeffCompPersonAccount = null;
                ibusHealthPersonAccount = null;
                ibusDentalPersonAccount = null;
                ibusVisionPersonAccount = null;
                ibusFlexPersonAccount = null;

                busPersonEmployment lbusPersonEmployment = new busPersonEmployment
                {
                    icdoPersonEmployment = new cdoPersonEmployment(),
                    ibusPerson = new busPerson { icdoPerson = new cdoPerson() }
                };
                lbusPersonEmployment.icdoPersonEmployment.LoadData(ldtr);
                lbusPersonEmployment.ibusPerson.icdoPerson.LoadData(ldtr);

                //if (lbusPersonEmployment.ibusPerson.icolPersonAccount.IsNull())
                //    lbusPersonEmployment.ibusPerson.LoadPersonAccount();

                if (lbusPersonEmployment.ibusLatestEmploymentDetail == null)
                    lbusPersonEmployment.LoadLatestPersonEmploymentDetail();

                DataTable ldtResultPA = DBFunction.DBSelect("entPersonAccount.GetPersonAccountByPersonEmploymentDetail", new object[] { lbusPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                lbusPersonEmployment.ibusPerson.icolPersonAccount = lbusBase.GetCollection<busPersonAccount>(ldtResultPA, "icdoPersonAccount");

                //Skip the Record If the New Employment Starts before 31 days                
                busPersonEmployment lbusNewPersonEmployment = null;
                bool lblnNewEmploymentFound = IsNewEmploymentExistsBefore31Days(lbusPersonEmployment, lbusPersonEmployment.ibusPerson, ref lbusNewPersonEmployment);

                foreach (busPersonAccount lbusPersonAccount in lbusPersonEmployment.ibusPerson.icolPersonAccount)
                {
                    if (lbusPersonAccount.ibusPlan == null)
                        lbusPersonAccount.LoadPlan();

                    //PIR 22932
					string lstrInsuranceType = string.Empty;
                    string lstrCobraType = string.Empty;
                    if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth ||
                        lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental ||
                        lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision)
                    {
                        if (lbusPersonAccount.ibusPersonAccountGHDV == null)
                            lbusPersonAccount.LoadPersonAccountGHDV();                       
                        if (lbusPersonAccount.ibusPersonAccountGHDV.IsNotNull() && lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv?.person_account_ghdv_id > 0)
                        {
                            lstrCobraType = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value;
                            if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth)
                                lstrInsuranceType = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value;
                            else if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental)
                                lstrInsuranceType = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value;
                            else if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision)
                                lstrInsuranceType = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value;
                        }
                    }

                    if (!lblnNewEmploymentFound)
                    {
                        if ((IsPlanParticipationStatusEnrolled(lbusPersonAccount.icdoPersonAccount.plan_participation_status_value)) ||
                            (IsPlanParticipationStatusSuspended(lbusPersonAccount.icdoPersonAccount.plan_participation_status_value)))
                        {
                            //lobjPersonAccount.ibusPlan = lobjPA.ibusPlan;
                            if (lbusPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB)
                            {
                                iblnGenerateDB = true;
                                ibusDBPersonAccount = lbusPersonAccount;
                            }
                            if (lbusPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC ||
                                lbusPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB) //PIR 27178 
                            {
                                iblnGenerateDC = true;
                                ibusDCPersonAccount = lbusPersonAccount;
                            }
                            if (lbusPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
                            {                                
                                if (lbusPersonAccount.ibusPersonAccountFlex.IsNull())
                                    lbusPersonAccount.LoadPersonAccountFlex();

                                if (lbusPersonAccount.ibusPersonAccountFlex.IsNotNull())
                                {
                                    //PIR 24498 -- Change logic to check from History table instead of Option table
                                    if (lbusPersonAccount.ibusPersonAccountFlex.iclbFlexCompHistory.IsNullOrEmpty())
                                        lbusPersonAccount.ibusPersonAccountFlex.LoadFlexCompHistory();

                                    // PROD PIR ID 6870 -- If other than enrolled, employment end date should overlaps Medicare spending option dates.
                                    if (lbusPersonAccount.ibusPersonAccountFlex.iclbFlexCompHistory.IsNotNull())
                                    {
                                        lbusPersonEmployment.lintBatchIDEmpTerminationNotices = this.iobjBatchSchedule.batch_schedule_id;
                                        if(lbusPersonAccount.ibusPersonAccountFlex.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                                        {
                                            //Prod PIR: 4606. The Letter should be generated only when the Option is Medical Spending.                                        
                                            if (lbusPersonAccount.ibusPersonAccountFlex.iclbFlexCompHistory.Any(x =>
                                                (x.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                                                && (x.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0.0M)))
                                            {
                                                iblnGenerateFlex = true;
                                                ibusFlexPersonAccount = lbusPersonAccount;
                                            }
                                        }
                                        else
                                        {
                                            if(lbusPersonAccount.ibusPersonAccountFlex.iclbFlexCompHistory.Where(o => o.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending &&
                                            o.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0.0M && busGlobalFunctions.CheckDateOverlapping(lbusPersonEmployment.icdoPersonEmployment.end_date,
                                            o.icdoPersonAccountFlexCompHistory.effective_start_date, o.icdoPersonAccountFlexCompHistory.effective_end_date)).Any())
                                            {
                                                iblnGenerateFlex = true;
                                                ibusFlexPersonAccount = lbusPersonAccount;
                                            }
                                        }
                                    }
                                }
                            }
                            if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDeferredCompensation) //PROD PIR ID 6760
                            {
                                iblnGenerateDeffComp = true;
                                ibusDeffCompPersonAccount = lbusPersonAccount;
                            }
                        }
                    }

                    if ((lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth) ||
                            (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision) ||
                            (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental))
                    {
                        //If there is no new employment found, we just need to check only if the plan suspended date
                        //should not be less than employment end date
                        if (!lblnNewEmploymentFound)
                        {
                            //UAT PIR: 774. Select Records in Enrolled Status as of Termination Date.
                             if ((IsPlanParticipationStatusEnrolled(lbusPersonAccount.icdoPersonAccount.plan_participation_status_value)) &&
                                (((lbusPersonAccount.icdoPersonAccount.history_change_date_no_null <= lbusPersonEmployment.icdoPersonEmployment.end_date) && (!(IsInsuranceTypeRetiree(lstrInsuranceType))))
                                || ((lbusPersonAccount.icdoPersonAccount.history_change_date_no_null > lbusPersonEmployment.icdoPersonEmployment.end_date) && (IsInsuranceTypeRetiree(lstrInsuranceType)) && (lbusPersonAccount.icdoPersonAccount.start_date <= lbusPersonEmployment.icdoPersonEmployment.end_date))
                                || ((lbusPersonAccount.icdoPersonAccount.history_change_date_no_null > lbusPersonEmployment.icdoPersonEmployment.end_date) && (!string.IsNullOrEmpty(lstrCobraType)))))
                            {
                                UpdateGHDVFlag(lbusPersonAccount.icdoPersonAccount.plan_id, lbusPersonAccount);
                            }
                            else if (IsPlanParticipationStatusSuspended(lbusPersonAccount.icdoPersonAccount.plan_participation_status_value))
                            {
                                if (lbusPersonAccount.icdoPersonAccount.history_change_date_no_null >= lbusPersonEmployment.icdoPersonEmployment.end_date)
                                {
                                    UpdateGHDVFlag(lbusPersonAccount.icdoPersonAccount.plan_id, lbusPersonAccount);
                                }
                            }
                        }
                        else
                        {
                            if (lbusNewPersonEmployment != null)
                            {
                                if (lbusNewPersonEmployment.ibusOrganization == null)
                                    lbusNewPersonEmployment.LoadOrganization();

                                if (lbusNewPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans == null)
                                    lbusNewPersonEmployment.ibusOrganization.LoadOrganizationOfferedPlans();

                                bool lblnPlanOffered = false;
                                foreach (busOrgPlan lbusOrgPlan in lbusNewPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans)
                                {
                                    if (lbusOrgPlan.icdoOrgPlan.plan_id == lbusPersonAccount.icdoPersonAccount.plan_id)
                                    {
                                        lblnPlanOffered = true;
                                        break;
                                    }
                                }

                                if (!lblnPlanOffered)
                                    UpdateGHDVFlag(lbusPersonAccount.icdoPersonAccount.plan_id, lbusPersonAccount);
                            }
                        }
                    }
                }

                if ((iblnGenerateDB) || (iblnGenerateFlex) || (iblnGenerateDC) || (iblnGenerateHealth) ||
                    (iblnGenerateDental) || (iblnGenerateVision) || (iblnGenerateDeffComp))
                {
                    GenerateCorrespondence(lbusPersonEmployment);
                    lstrStatus = busConstant.TerminationLetterStatusSent;
                }
                lbusPersonEmployment.icdoPersonEmployment.termination_letter_status_value = lstrStatus;
                lbusPersonEmployment.icdoPersonEmployment.Update();
            }
            idlgUpdateProcessLog("Correspondence created successfully", "INFO", istrProcessName);
        }

        private void UpdateGHDVFlag(int aintPlanId, busPersonAccount abusPersonAccount)
        {
            if (aintPlanId == busConstant.PlanIdGroupHealth)
            {
                iblnGenerateHealth = true;
                ibusHealthPersonAccount = abusPersonAccount;
            }
            else if (aintPlanId == busConstant.PlanIdDental)
            {
                iblnGenerateDental = true;
                ibusDentalPersonAccount = abusPersonAccount;
            }
            else if (aintPlanId == busConstant.PlanIdVision)
            {
                iblnGenerateVision = true;
                ibusVisionPersonAccount = abusPersonAccount;
            }
        }

        private bool IsNewEmploymentExistsBefore31Days(busPersonEmployment abusPersonEmployment, busPerson abusPerson, ref busPersonEmployment abusNewPersonEmployment)
        {
            bool lblnResult = false;
            if (abusPerson.icolPersonEmployment == null)
                abusPerson.LoadPersonEmployment();

            foreach (busPersonEmployment lbusPersonEmployment in abusPerson.icolPersonEmployment)
            {
                if (lbusPersonEmployment.icdoPersonEmployment.person_employment_id != abusPersonEmployment.icdoPersonEmployment.person_employment_id)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(lbusPersonEmployment.icdoPersonEmployment.start_date,
                                                       abusPersonEmployment.icdoPersonEmployment.end_date,
                                                       abusPersonEmployment.icdoPersonEmployment.end_date.AddDays(31)))
                    {
                        abusNewPersonEmployment = lbusPersonEmployment;
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
        }

        private void GenerateCorrespondence(busPersonEmployment abusPersonEmployment)
        {
            //For Correspondence Properties
            if (abusPersonEmployment.ibusOrganization == null)
                abusPersonEmployment.LoadOrganization();
            // ** BR - 242 ** The Batch correspondence letter will generate in the following sequence.
            if ((iblnIsMemberAccountBalanceltd && iblnGenerateDB) || (iblnGenerateFlex) || (iblnGenerateDC) || (iblnGenerateHealth) || (iblnGenerateDental) || (iblnGenerateVision) || (iblnGenerateDeffComp))
                CreateAddressEnvelope(abusPersonEmployment.ibusPerson);
            if (iblnGenerateDB)
                CreateDBVestedNonVestedLetter(abusPersonEmployment);           
            if (iblnGenerateDC)
                CreateDCTerminationNotice(abusPersonEmployment.ibusPerson);
            if (iblnGenerateDeffComp)
                Create457TerminationNotice(abusPersonEmployment);
            if (iblnGenerateHealth || iblnGenerateDental || iblnGenerateVision) // PIR 6919
            {
                ibusHealthPersonAccount = ibusHealthPersonAccount == null ? (ibusDentalPersonAccount == null ? ibusVisionPersonAccount : ibusDentalPersonAccount) : ibusHealthPersonAccount;
                CreateCOBRAInsuranceNotice(abusPersonEmployment, ibusHealthPersonAccount);
                ibusHealthPersonAccount.UpdatePersonAccountDependentList();
            }
            if (iblnGenerateFlex)
                CreateCOBRAFlexCompNotice(abusPersonEmployment);
            // ** BR - 236 ** IRS Special Tax Notice letter will generate either a DB Vested/Non Vested letter or DC Terminated letter generated.
            if (((iblnIsMemberAccountBalanceltd && iblnGenerateDB) || (iblnGenerateDC)) && (Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckRETRApplicationOfLast6Months", new object[2]
               { abusPersonEmployment.ibusPerson.icdoPerson.person_id, busGlobalFunctions.GetSysManagementBatchDate() }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) == 0))
            {
                CreateIRSNotice(abusPersonEmployment.ibusPerson);
            }
        }

        private void CreateAddressEnvelope(busPerson abusPerson)
        {
            //ArrayList larrList = new ArrayList();
            //larrList.Add(abusPerson);

            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");

            idlgUpdateProcessLog("Creating Envelope", "INFO", istrProcessName);
            istrFileName = CreateCorrespondence("PER-0950", abusPerson, lshtTemp);
            //CreateContactTicket(abusPerson.icdoPerson.person_id);
        }

        private void CreateDBVestedNonVestedLetter(busPersonEmployment abusPersonEmployment)
        {

            try
            {
                //PID - 675
                //check whether the person is vested or not.
                //1. If the member is not vested then generate DB NON VESTED LETTER

                //check vesting eligibility of the Person                   
                //get total tvsc
                //ibusDBPersonAccount.LoadTotalVSC();
                iblnIsMemberAccountBalanceltd = false;
                //Load the Account Balance
                busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
                lbusPersonAccountRetirement.FindPersonAccountRetirement(ibusDBPersonAccount.icdoPersonAccount.person_account_id);
                lbusPersonAccountRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonAccountRetirement.ibusPerson.icdoPerson = abusPersonEmployment.ibusPerson.icdoPerson;
                lbusPersonAccountRetirement.LoadLTDSummary();
                lbusPersonAccountRetirement.LoadRetirementContributionAll();
                if (lbusPersonAccountRetirement.ibusPlan == null)
                    lbusPersonAccountRetirement.LoadPlan();
                //lbusPersonAccountRetirement.LoadTotalVSC();
                //PIR 26345
                decimal ldecTotTVSCIncludingTentativeService = Math.Round(lbusPersonAccountRetirement.ibusPerson.GetTotalVSCForPerson(lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJobService, DateTime.MinValue,
                                                                                             true, false, iintBenefitPlanId: lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_id), 4, MidpointRounding.AwayFromZero);
                
                //calculate person age
                int lintMemberAgeInMonths = 0;
                int lintMemberAgeInYears = 0;
                decimal ldecMonthAndYear = 0m;
                int lintMonths = 0;
                busPersonBase.CalculateAge(abusPersonEmployment.ibusPerson.icdoPerson.date_of_birth,
                    abusPersonEmployment.icdoPersonEmployment.end_date, ref lintMonths, ref ldecMonthAndYear, 2,
                    ref lintMemberAgeInYears, ref lintMemberAgeInMonths);

                //PIR 21435 - start - consider any service credit from SGT_PERSON_TFFR_TIAA_SERVICE In TTNTV or APRV status while calculating TVSC
                //decimal ldecTFFRAppService = 0.0M;
                //decimal ldecTIAAAppService = 0.0M;
                //decimal ldecTFFRTenService = 0.0M;
                //decimal ldecTIAATenService = 0.0M;
                //lbusPersonAccountRetirement.LoadTFFRTIAAService(ref ldecTFFRAppService, ref ldecTIAAAppService, ref ldecTFFRTenService, ref ldecTIAATenService);
                //decimal ldecTotTVSCIncludingTentativeService = lbusPersonAccountRetirement.icdoPersonAccount.Total_VSC + ldecTFFRTenService + ldecTIAATenService;
                //PIR 21435 - end - consider any service credit from SGT_PERSON_TFFR_TIAA_SERVICE In TTNTV or APRV status while calculating TVSC
                //if(busPersonBase.CheckIsPersonVestedForEstimateServicePurchase(lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_id,lbusPersonAccountRetirement.ibusPlan.icdoPlan.benefit_provision_id,
                //                                                                   busConstant.ApplicationBenefitTypeRetirement, ldecTotTVSCIncludingTentativeService, ibusDBPersonAccount, iobjPassInfo))
                if (busPersonBase.CheckIsPersonVested(lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_id
                        , lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_code,
                        lbusPersonAccountRetirement.ibusPlan.icdoPlan.benefit_provision_id,
                        busConstant.ApplicationBenefitTypeRetirement,
                        ldecTotTVSCIncludingTentativeService,
                        ldecMonthAndYear, abusPersonEmployment.icdoPersonEmployment.end_date, false, abusPersonEmployment.icdoPersonEmployment.end_date.AddMonths(-1).GetLastDayofMonth(), lbusPersonAccountRetirement, iobjPassInfo, ablnIsFromBatch : true)) //PIR 14646 - Vesting logic changes
                {
                    if (Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckRETRApplicationOfLast6Months", new object[2]
                            { lbusPersonAccountRetirement.ibusPerson.icdoPerson.person_id, busGlobalFunctions.GetSysManagementBatchDate() }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) == 0)
                    {
                        //DB Vested Letter
                        if (lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.IsNull())
                        {
                            lbusPersonAccountRetirement.ibusRetirementBenefitCalculation = new busRetirementBenefitCalculation
                            {
                                icdoBenefitCalculation = new cdoBenefitCalculation()
                            };
                            //this declaration is jus dummy to avoid object ref error
                            lbusPersonAccountRetirement.ibusDisabilityBenefitCalculation = new busRetirementBenefitCalculation
                            {
                                icdoBenefitCalculation = new cdoBenefitCalculation()
                            };
                            lbusPersonAccountRetirement.ibusVestedBenefitCalculation = new busRetirementBenefitCalculation
                            {
                                icdoBenefitCalculation = new cdoBenefitCalculation()
                            };
                            lbusPersonAccountRetirement.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                            lbusPersonAccountRetirement.ibusPersonAccount.icdoPersonAccount = ibusDBPersonAccount.icdoPersonAccount;
                            lbusPersonAccountRetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeEstimate, adteActEmpTermDate: abusPersonEmployment.icdoPersonEmployment.end_date);  //PIR 13948
                            lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.ibusPlan = lbusPersonAccountRetirement.ibusPlan;
                            lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.LoadBenefitProvisionMultiplier();
                            lbusPersonAccountRetirement.CalculateRetirementBenefitAmount();
                            lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.SetBenefitSubType();
                        }
                        //ArrayList larrList = new ArrayList();
                        //larrList.Add(lbusPersonAccountRetirement);
                        if (lbusPersonAccountRetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.minimum_guarentee_amount > 0.00M)
                        {
                            iblnIsMemberAccountBalanceltd = true;
                            //PIR 25936 added this condition if member account balance is greater than zero Correspondence will generate
                            Hashtable lshtTemp = new Hashtable();
                            lshtTemp.Add("FormTable", "Batch");
                            idlgUpdateProcessLog("Creating Correspondence for DB Vested", "INFO", istrProcessName);
                            istrFileName = CreateCorrespondence("APP-7359", lbusPersonAccountRetirement, lshtTemp);
                        }
                    }
                    //CreateContactTicket(abusPersonEmployment.icdoPersonEmployment.person_id);
                }
                else
                {
                    //DB Non Vested Letter
                    //ArrayList larrList = new ArrayList();
                    //larrList.Add(lbusPersonAccountRetirement);
                    if (lbusPersonAccountRetirement.Member_Account_Balance_ltd > 0.00M)
                    {
                        iblnIsMemberAccountBalanceltd = true;
                        //PIR 25936 added this condition if member account balance is greater than zero Correspondence will generate
                        Hashtable lshtTemp = new Hashtable();
                        lshtTemp.Add("FormTable", "Batch");
                        idlgUpdateProcessLog("Creating Correspondence for DB Non Vested", "INFO", istrProcessName);
                        istrFileName = CreateCorrespondence("APP-7358", lbusPersonAccountRetirement, lshtTemp);
                        
                        //CreateContactTicket(abusPersonEmployment.icdoPersonEmployment.person_id);
                    }
                }       
            }
            catch (Exception e)
            {
                idlgUpdateProcessLog("Generating Correspondence for DB Vested / Non Vested Failed for the following reason" + e, "INFO", istrProcessName);
            }
        }

        private void CreateDCTerminationNotice(busPerson abusPerson)
        {
            //ArrayList larrList = new ArrayList();
            //larrList.Add(abusPerson);

            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");
            idlgUpdateProcessLog("Creating Correspondence for DC", "INFO", istrProcessName);
            istrFileName = CreateCorrespondence("PER-0153", abusPerson, lshtTemp);
            //CreateContactTicket(abusPerson.icdoPerson.person_id);
        }

        private void CreateIRSNotice(busPerson abusPerson)
        {
            //ArrayList larrList = new ArrayList();
            //larrList.Add(abusPerson);

            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");
            istrFileName = CreateCorrespondence("PER-0106", abusPerson, lshtTemp);
            //CreateContactTicket(abusPerson.icdoPerson.person_id);
            
        }

        private void Create457TerminationNotice(busPersonEmployment abusPersonEmployment)
        {
            //Assign the Person Employment into Person Account in order to display Employment End Date
            if (ibusDeffCompPersonAccount.ibusPersonEmploymentDetail == null)
                ibusDeffCompPersonAccount.ibusPersonEmploymentDetail = new busPersonEmploymentDetail();

            ibusDeffCompPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment = abusPersonEmployment;
            if (ibusDeffCompPersonAccount.ibusPlan == null)
                ibusDeffCompPersonAccount.LoadPlan();


            //ArrayList larrList = new ArrayList();
            //larrList.Add(ibusDeffCompPersonAccount);

            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");
            idlgUpdateProcessLog("Creating Correspondence for 457 Termination Letter", "INFO", istrProcessName);
            istrFileName = CreateCorrespondence("PER-0152", ibusDeffCompPersonAccount, lshtTemp);
            //CreateContactTicket(abusPersonEmployment.icdoPersonEmployment.person_id);
        }

        private void CreateCOBRAInsuranceNotice(busPersonEmployment abusPersonEmployment, busPersonAccount abusPersonAccount)
        {
            //Assign the Person Employment into Person Account in order to display Employment End Date
            if (abusPersonAccount.ibusPersonEmploymentDetail == null)
                abusPersonAccount.ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
            abusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment = abusPersonEmployment;
            if (abusPersonAccount.ibusPlan == null)
                abusPersonAccount.LoadPlan();

            //PROD PIR 4518
            if ((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental) ||
                (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) ||
                (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) ||
                (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD))
            {
                if (abusPersonAccount.ibusPersonAccountGHDV == null)
                    abusPersonAccount.LoadPersonAccountGHDV();

                abusPersonAccount.ibusPersonAccountGHDV.LoadPlanEffectiveDate();
                abusPersonAccount.ibusPersonAccountGHDV.DetermineEnrollmentAndLoadObjects(abusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate, false);

                if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                    abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeCOBRA;
                else if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                    abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeCOBRA;

                if (abusPersonAccount.ibusPersonAccountGHDV.IsHealthOrMedicare)
                {
                    if (abusPersonAccount.ibusPersonAccountGHDV.IsCoverageCodeCodeSingle())
                        abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0004";
                    else
                        abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0005";

                    abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = busConstant.COBRAType18Month;

                    if (abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                    {
                        abusPersonAccount.ibusPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                    }
                    else
                    {
                        abusPersonAccount.ibusPersonAccountGHDV.LoadHealthParticipationDate();
                        abusPersonAccount.ibusPersonAccountGHDV.LoadHealthPlanOption();
                        //To Get the Rate Structure Code (Derived Field)
                        abusPersonAccount.ibusPersonAccountGHDV.LoadRateStructure();
                    }

                    //Get the Coverage Ref ID
                    abusPersonAccount.ibusPersonAccountGHDV.LoadCoverageRefID();

                    //Get the Premium Amount
                    abusPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmountByRefID();
                }
                else
                {
                    abusPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmount();
                }

                abusPersonAccount.ldclTotalCOBRAPremiumAmount = abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
            }
            abusPersonAccount.ibusPersonAccountGHDV.LoadEnrolledPlanAndPremiumForPersonAccountGHDV(); // PIR 6919
            //ArrayList larrList = new ArrayList();
            //larrList.Add(abusPersonAccount);

            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");
            idlgUpdateProcessLog("Creating Correspondence for COBRA Insurance", "INFO", istrProcessName);
            istrFileName = CreateCorrespondence("PER-0105", abusPersonAccount, lshtTemp);
            //CreateContactTicket(abusPersonEmployment.icdoPersonEmployment.person_id);
        }

        private void CreateCOBRAFlexCompNotice(busPersonEmployment abusPersonEmployment)
        {
            if (abusPersonEmployment.ibusOrganization == null)
                abusPersonEmployment.LoadOrganization();

            if (abusPersonEmployment.ibusPerson == null)
                abusPersonEmployment.LoadPerson();

            //ArrayList larrList = new ArrayList();
            //larrList.Add(abusPersonEmployment);

            Hashtable lshtTemp = new Hashtable();
            lshtTemp.Add("FormTable", "Batch");
            idlgUpdateProcessLog("Creating Correspondence for COBRA FlexComp", "INFO", istrProcessName);
            istrFileName = CreateCorrespondence("PER-0104", abusPersonEmployment, lshtTemp);
            //CreateContactTicket(abusPersonEmployment.icdoPersonEmployment.person_id);
        }

        private bool IsPlanParticipationStatusEnrolled(string astPlanParticipationStatus)
        {
            if ((astPlanParticipationStatus == busConstant.PlanParticipationStatusDefCompEnrolled) ||
                (astPlanParticipationStatus == busConstant.PlanParticipationStatusFlexCompEnrolled) ||
                (astPlanParticipationStatus == busConstant.PlanParticipationStatusInsuranceEnrolled) ||
                (astPlanParticipationStatus == busConstant.PlanParticipationStatusRetirementEnrolled))
                return true;
            else
                return false;
        }

        private bool IsPlanParticipationStatusSuspended(string astPlanParticipationStatus)
        {
            if ((astPlanParticipationStatus == busConstant.PlanParticipationStatusInsuranceSuspended) ||
                (astPlanParticipationStatus == busConstant.PlanParticipationStatusFlexSuspended) ||
                (astPlanParticipationStatus == busConstant.PlanParticipationStatusRetimentSuspended) ||
                (astPlanParticipationStatus == busConstant.PlanParticipationStatusDefCompSuspended))
                return true;
            else
                return false;
        }
		//PIR 22932
        private bool IsInsuranceTypeRetiree(string astInsuranceType)
        {
            if ((astInsuranceType == busConstant.DentalInsuranceTypeRetiree) ||
                (astInsuranceType == busConstant.HealthInsuranceTypeRetiree) ||
                (astInsuranceType == busConstant.VisionInsuranceTypeRetiree))
                return true;
            else
                return false;
        }

        private void CreateContactTicket(int aintPersonID)
        {
            cdoContactTicket lobjContactTicket = new cdoContactTicket();
            CreateContactTicket(aintPersonID, busConstant.ContactTicketTypeInsuranceRetiree, lobjContactTicket);
        }

        public void GenerateCorrespondenceForNonPayeeCOBRATerminationNotice()
        {
            // The Query will fetch the record where EmploymentDetailEndDate is NOT Null and COBRALetterSentFlag is LetterNotSent(LNS).
            DataTable ldtResult = DBFunction.DBSelect("entPersonAccount.NonPayeeEmployeeCOBRAletter", new object[] { },
                                            iobjPassInfo.iconFramework,
                                            iobjPassInfo.itrnFramework);
            foreach (DataRow ldtr in ldtResult.Rows)
            {
                busPersonEmploymentDetail lbusPersonEmploymentDetail = new busPersonEmploymentDetail
                {
                    icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail()                    
                };
                lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtr);
                    if (lbusPersonEmploymentDetail.ibusPersonEmployment == null)
                        lbusPersonEmploymentDetail.LoadPersonEmployment();
                    if (lbusPersonEmploymentDetail.ibusPersonEmployment.ibusPerson == null)
                        lbusPersonEmploymentDetail.ibusPersonEmployment.LoadPerson();
                    ibusHealthPersonAccount = new busPersonAccount(){ icdoPersonAccount=new cdoPersonAccount()};
                    DataTable ldtPAResult = busNeoSpinBase.Select<cdoPersonAccount>(
                    new string[2] { NeoSpin.DataObjects.enmPersonAccount.person_id.ToString(), NeoSpin.DataObjects.enmPersonAccount.plan_id.ToString()},
                    new object[2] { lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_id, busConstant.PlanIdGroupHealth }, null, null);
                    if (ldtPAResult.IsNotNull() && ldtPAResult.Rows.Count > 0)
                    {
                        ibusHealthPersonAccount.icdoPersonAccount.LoadData(ldtPAResult.Rows[0]);
                    }

                   if (ibusHealthPersonAccount.icdoPersonAccount.person_account_id > 0)
                    {
                        // ** BR - 242 ** The Batch correspondence letter will generate in the following sequence.
                        CreateAddressEnvelope(lbusPersonEmploymentDetail.ibusPersonEmployment.ibusPerson);
                        CreateCOBRAInsuranceNotice(lbusPersonEmploymentDetail.ibusPersonEmployment, ibusHealthPersonAccount);
                        ibusHealthPersonAccount.UpdatePersonAccountDependentList();
                    }
                lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.cobra_letter_status_value = busConstant.COBRALetterStatusSent; 
                lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();
            }
        }
    }
}
