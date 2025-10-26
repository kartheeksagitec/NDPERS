using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using System.Collections;
using Sagitec.Common;
using System.Linq.Expressions;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using System.Net.Mail;
using Sagitec.ExceptionPub;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSHome : busWSSHome
    {
        public string istrUpdateAccountURL { get; set; } //F/W Upgrade PIR 21551
        public busPerson ibusPerson { get; set; }

        public cdoPerson lcdoPerson { get; set; }

        public bool iblnExternalLogin { get; set; } = false;

        public string istrProfileEmailID;
        public bool iblnIsFromMSSActivePlan = false; //PIR 16010
		//PIR 19108
        public int iintPersonID { get; set;}
        public int iintPersonAccountDeferredComp { get; set; }
        public int iintPersonEmploymentDetailid { get; set; }
        public int iintPlanID { get; set; }
        public Collection<busWssMessageDetail> iclbWSSMessageDetails { get; set; }

        public int iintMessageCount { get; set; }
        public string benefit_tier_description_display { get; set; }
        
        public bool iblnIsTemporaryMember { get; set; }
        public bool iblnIsACHDetailExixts { get; set; }

        public string istrPeopleSoftURL
        {
            get
            {
                //PIR 24075
                if (ibusPerson?.ibusCurrentEmployment == null)
                    ibusPerson?.LoadCurrentEmployer();
                if (ibusPerson?.ibusCurrentEmployment.ibusOrganization == null)
                    ibusPerson?.ibusCurrentEmployment.LoadOrganization();
                if (ibusPerson?.ibusCurrentEmployment?.ibusOrganization?.icdoOrganization?.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueHigherEd)
                {
                    return utlPassInfo.iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.SystemConstantCodeID, busConstant.PeopleSoftHIEDURL);
                }
                else
                {
                    return utlPassInfo.iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.SystemConstantCodeID, busConstant.PeopleSoftURL);
                }
            }
        }

        //enhancement 6878
        public string istrConfirmationText
        {
            get
            {

                string luserName = ibusPerson.icdoPerson.FullName;
                DateTime Now = DateTime.Now;
                string lstrConfimation = string.Format(busGlobalFunctions.GetMessageTextByMessageID(8566, iobjPassInfo), luserName, Now);
                return lstrConfimation;
            }

        }

        public string istrIsLimitedAccessRetiree { get; set; }

        public string istrOpenElectionText
        {
            get
            {
                if (this.iintElectionId > 0)
                {
                    busBoardMemberElection lbusBoardMemberElection = new busBoardMemberElection() { icdoBoardMemberElection = new doBoardMemberElection() };
                    if (lbusBoardMemberElection.FindByPrimaryKey(this.iintElectionId))
                    {
                        DataTable ldtbBoardMemberElectionText = Select("cdoWssAcknowledgement.SelectAck", new object[2] { busGlobalFunctions.GetSysManagementBatchDate(), lbusBoardMemberElection.icdoBoardMemberElection.audience_value == busConstant.GroupTypeRetired ? busConstant.BoardMemberElectionTextRetiree : busConstant.BoardMemberElectionTextActive });
                        if (ldtbBoardMemberElectionText.Rows.Count > 0)
                        {
                            busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                            lobjWssAcknowledgement.icdoWssAcknowledgement = new CustomDataObjects.cdoWssAcknowledgement();
                            if (ldtbBoardMemberElectionText.Rows.Count > 0 && ldtbBoardMemberElectionText.Rows[0]["acknowledgement_text"] != DBNull.Value)
                            {
                                lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = Convert.ToString(ldtbBoardMemberElectionText.Rows[0]["acknowledgement_text"]);
                                return string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text,
                                                                            lbusBoardMemberElection.icdoBoardMemberElection.end_date.ToLongDateString());
                            }
                        }
                    }
                }
                return string.Empty;
            }
        }

        public bool LoadPerson(int aintPersonID)
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();

            return ibusPerson.FindPerson(aintPersonID);
        }

        public DataTable GetMSSQuestionsForOnlineAccess()
        {
            return iobjPassInfo.isrvDBCache.GetCodeValues(3402);
        }

        public bool IsProfileEmailMatchPersonEmail()
        {
            bool lblnResult = false;
            if (iblnExternalLogin)
            {
                if (ibusPerson.icdoPerson.email_address.IsNotNull() && istrProfileEmailID.IsNotNull())
                {
                    if (ibusPerson.icdoPerson.email_address.ToLower() == istrProfileEmailID.ToLower())
                    {
                        lblnResult = true;
                    }
                }
            }
            else
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        //19997
        public int iintHealthPersonAccountId { get; set; }
        public int iintHealthEnrollmentRequestId { get; set; }
        public int iintHDHPPersonEmploymentDetailid { get; set; }
        //PIR 25920 DC 2025 change
        public int iintNoOfDaysRemainingBeforeElection { get; set; }
        public int iintDC25PersonAccountID { get; set; }
        public void LoadContactTickets()
        {
            if (iclbContactTicket == null)
                iclbContactTicket = new Collection<busContactTicket>();
            DataTable ldtbContactTicket = busNeoSpinBase.Select("cdoContactTicket.LoadMSSContactTickets", new object[1] { ibusPerson.icdoPerson.person_id });
            iclbContactTicket = GetCollection<busContactTicket>(ldtbContactTicket, "icdoContactTicket");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busContactTicket)
            {
                busContactTicket lbusContactTicket = (busContactTicket)aobjBus;
                lbusContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule { icdoAppointmentSchedule = new cdoAppointmentSchedule() };
                lbusContactTicket.ibusAppointmentSchedule.icdoAppointmentSchedule.LoadData(adtrRow);
                lbusContactTicket.ibusCounselor = new busUser { icdoUser = new cdoUser() };
                if (lbusContactTicket.ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_schedule_id > 0)
                {
                    lbusContactTicket.LoadAppointmentCounselorName();
                }
            }
            base.LoadOtherObjects(adtrRow, aobjBus);
        }

        public void LoadSeminars()
        {
            if (iclbSeminars == null)
                iclbSeminars = new Collection<busSeminarSchedule>();
            DataTable ldtbSeminars = busNeoSpinBase.Select("cdoSeminarSchedule.LoadSeminarsForMSS", new object[1] { ibusPerson.icdoPerson.person_id });
            iclbSeminars = GetCollection<busSeminarSchedule>(ldtbSeminars, "icdoSeminarSchedule");
        }

        public bool IsMemberEnrolledOrSuspendedInDBPlan()
        {
            bool lblnResult = false;
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();

                if (lbusPersonAccount.ibusPlan.IsDBRetirementPlan() || lbusPersonAccount.ibusPlan.IsHBRetirementPlan())
                {
                    if (lbusPersonAccount.IsPlanParticipationStatusRetirementEnrolled() || lbusPersonAccount.IsPlanParticipationStatusRetirementSuspended())
                    {
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
        }




        public bool IsMemberEnrolledInDBPlan()
        {
            bool lblnResult = false;
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();

                if (lbusPersonAccount.ibusPlan.IsDBRetirementPlan() || lbusPersonAccount.ibusPlan.IsHBRetirementPlan())
                {
                    if (lbusPersonAccount.IsPlanParticipationStatusRetirementEnrolled())
                    {
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
        }

        public bool IsMemberEnrolledOrSuspendedInDCPlan()
        {
            bool lblnResult = false;
            if (ibusPerson.icolPersonAccountByPlan == null)
                ibusPerson.LoadPersonAccountByPlan(busConstant.PlanIdDC);
            if (ibusPerson.icolPersonAccountByPlan == null)
                ibusPerson.LoadPersonAccountByPlan(busConstant.PlanIdDC2020); //PIR 20232
            if (ibusPerson.icolPersonAccountByPlan == null)
                ibusPerson.LoadPersonAccountByPlan(busConstant.PlanIdDC2025); //PIR 25920

            foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccountByPlan)
            {
                if (lbusPersonAccount.IsPlanParticipationStatusRetirementEnrolled() || lbusPersonAccount.IsPlanParticipationStatusRetirementSuspended())
                {
                    lblnResult = true;
                    break;
                }
            }
            return lblnResult;
        }

        public void SetElectionID(string astrMemberType)
        {
            int lintPerson = ibusPerson.icdoPerson.person_id;
            if (lintPerson > 0)
            {
                DataTable ldtbPersonVoteForOpenElection = Select("entBoardMemberVote.DidPersonVoteForOpenElection", new object[2] { astrMemberType, lintPerson });
                if(ldtbPersonVoteForOpenElection.Rows.Count > 0)
                {
                    if(ldtbPersonVoteForOpenElection.Rows[0]["ELECTION_ID"] != DBNull.Value)
                    {
                        this.iintElectionId = Convert.ToInt32(ldtbPersonVoteForOpenElection.Rows[0]["ELECTION_ID"]);                                                
                    }
                }
            }
        }
        public int iintElectionId { get; set; } //24460
        //with status enrolled, waived and eligible
        //public Collection<busPersonAccountEmploymentDetail> iclbPlans { get; set; }
        public Collection<busBenefitPlanWeb> iclbEnrolledPlans { get; set; }
        public Collection<busBenefitPlanWeb> iclbEligiblePlans { get; set; }
        public Collection<busBenefitPlanWeb> iclbEligiblePlansUI { get; set; } //PIR - 10022
        public Collection<busBenefitPlanWeb> iclbEnrolledPlansUI { get; set; }  //PIR - 10022
        public Collection<busPersonAccountEmploymentDetail> iclbInsurancePlans { get; set; }
        public Collection<busBenefitPlanWeb> iclbBenefitPlans { get; set; }

        public void LoadEnrolledAndEligiblePlans()
        {
            bool lblnDCAccountAlreadyExists = false;//pir-7793

            if (iclbBenefitPlans.IsNull())
                iclbBenefitPlans = new Collection<busBenefitPlanWeb>();

            if (iclbEnrolledPlans.IsNull()) iclbEnrolledPlans = new Collection<busBenefitPlanWeb>();
            if (iclbEligiblePlans.IsNull()) iclbEligiblePlans = new Collection<busBenefitPlanWeb>();

            if (iclbInsurancePlans.IsNull()) iclbInsurancePlans = new Collection<busPersonAccountEmploymentDetail>();
            if (ibusPerson.icolPersonEmployment.IsNull()) ibusPerson.LoadPersonEmployment();
            if (ibusPerson.iclbWSSEnrollmentRequest.IsNull()) ibusPerson.LoadWSSEnrollmentRequest();

            foreach (busPersonEmployment lobjPersonEmployment in ibusPerson.icolPersonEmployment)
            {
                if (busGlobalFunctions.CheckDateOverlapping(DateTime.Today, lobjPersonEmployment.icdoPersonEmployment.start_date, lobjPersonEmployment.icdoPersonEmployment.end_date) ||
                   lobjPersonEmployment.icdoPersonEmployment.start_date >= DateTime.Today) // PROD PIR 7944 -- Future dated scenario
                {
                    LoadPlanPerEmploymentDetail(lobjPersonEmployment);

                    //pir 7793 
                    if (lobjPersonEmployment.ibusPerson.IsDCPersonAccountExists())
                    {
                        if (lobjPersonEmployment.ibusLatestEmploymentDetail.IsNull())
                            lobjPersonEmployment.LoadLatestPersonEmploymentDetail();
                        // PIR 9777 , For Judges, Highway Patrol Person the DC eligility is not available.
                        if (lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value != busConstant.JobClassHighwayPatrolPerson &&
                            lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value != busConstant.JobClassJudge)
                            lblnDCAccountAlreadyExists = true;
                    }
                    //end
                }
                //pir 5500
                else
                    LoadOldDBSuspendedPlanAccounts(lobjPersonEmployment);
            }

            //load retiree insurance plans
            LoadInsuranceRetireesPlanAccounts();

            // PROD PIR 6320 -- START
            foreach (busBenefitPlanWeb lobjBenPlan in iclbEnrolledPlans)
            {
                if (lobjBenPlan.ibusPlan.IsNull()) lobjBenPlan.LoadPlan();
                //PIR- 23093 Provider_Name for NDPERS Plan                
                if (lobjBenPlan.iintPlanId == busConstant.PlanIdGroupHealth || lobjBenPlan.iintPlanId == busConstant.PlanIdDental || lobjBenPlan.iintPlanId == busConstant.PlanIdVision
                   || lobjBenPlan.iintPlanId == busConstant.PlanIdGroupLife || lobjBenPlan.iintPlanId == busConstant.PlanIdDC || lobjBenPlan.iintPlanId == busConstant.PlanIdDC2020 ||
                   lobjBenPlan.iintPlanId == busConstant.PlanIdDC2025) //PIR 25920 DC 2025 changes
                    lobjBenPlan.istrProviderName = GetProvidersName(lobjBenPlan, true);
                int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(lobjBenPlan.iintPlanId, ibusPerson.icdoPerson.person_id);
                if(lintPersonAccountId >0)
                {
                    busPersonAccount lbusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount() };
                    lbusPersonAccount.FindPersonAccount(lintPersonAccountId);
                    lbusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
                    lbusPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(lbusPersonAccount.icdoPersonAccount.person_account_id);
                    lobjBenPlan.benefit_tier_description_display = lbusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_description_display;
                }
				//PIR PIR 26955
                lobjBenPlan.iblnIsLearnMoreLinkVisible = IsLearnMoreLinkVisible(lobjBenPlan.iintPlanId);
                lobjBenPlan.iblnIsVideoLinkVisible = IsVideoLinkVisible(lobjBenPlan.iintPlanId);
            }
            if (iclbEnrolledPlans.IsNotNull() && iclbEnrolledPlans.Count > 0)
                busGlobalFunctions.Sort<busBenefitPlanWeb>("ibusPlan.icdoPlan.sort_order", iclbEnrolledPlans);

            foreach (busBenefitPlanWeb lobjBenPlan in iclbEligiblePlans)
            {
                if (lobjBenPlan.ibusPlan.IsNull()) lobjBenPlan.LoadPlan();
                //PIR- 23093 Provider_Name for NDPERS Plan
                if (lobjBenPlan.iintPlanId == busConstant.PlanIdGroupHealth || lobjBenPlan.iintPlanId == busConstant.PlanIdDental || lobjBenPlan.iintPlanId == busConstant.PlanIdVision
                   || lobjBenPlan.iintPlanId == busConstant.PlanIdGroupLife || lobjBenPlan.iintPlanId == busConstant.PlanIdDC || lobjBenPlan.iintPlanId == busConstant.PlanIdDC2020 ||
                   lobjBenPlan.iintPlanId == busConstant.PlanIdDC2025) //PIR 25920 DC 2025 changes)
                    lobjBenPlan.istrProviderName = GetProvidersName(lobjBenPlan, true);
				//PIR PIR 26955
                lobjBenPlan.iblnIsLearnMoreLinkVisible = IsLearnMoreLinkVisible(lobjBenPlan.iintPlanId);
                lobjBenPlan.iblnIsVideoLinkVisible = IsVideoLinkVisible(lobjBenPlan.iintPlanId);
            }
            if (iclbEligiblePlans.IsNotNull() && iclbEligiblePlans.Count > 0)
                busGlobalFunctions.Sort<busBenefitPlanWeb>("ibusPlan.icdoPlan.sort_order", iclbEligiblePlans);
            // PROD PIR 6320 -- END

            // PROD PIR 7793 -- START   
            if (lblnDCAccountAlreadyExists)
            {
                Collection<busBenefitPlanWeb> lclbDBPlans = new Collection<busBenefitPlanWeb>();
                var lenumList = iclbEligiblePlans.Where(i => i.ibusPlan.icdoPlan.retirement_type_value.IsNotNullOrEmpty()
                    && (i.ibusPlan.icdoPlan.retirement_type_value.Equals(busConstant.PlanRetirementTypeValueDB) || i.ibusPlan.icdoPlan.retirement_type_value.Equals(busConstant.PlanRetirementTypeValueHB)));   //PIR 25920  New DC plan
                if (lenumList.Count() > 0)
                {
                    lenumList.ForEach(i => lclbDBPlans.Add(i));
                    lclbDBPlans.ForEach(i => iclbEligiblePlans.Remove(i));
                }
            }
            // PROD PIR 7793 -- END

            //PIR-10022 Start
            var lenum = from a in iclbEligiblePlans
                        select a;

            iclbEligiblePlansUI = lenum.ToList().ToCollection();

            var lenum1 = from a in iclbEnrolledPlans
                         select a;
            iclbEnrolledPlansUI = lenum1.ToList().ToCollection();

            iclbEnrolledPlansUI = iclbEnrolledPlans
               .GroupBy(p => new { p.iintPlanId })
               .Select(g => g.First())
               .ToList().ToCollection();


            busBenefitPlanWeb lbusBenefitPlanWebRemove = new busBenefitPlanWeb();
            foreach (busBenefitPlanWeb lbusBenefitPlanWeb in iclbEligiblePlans)
            {
                foreach (busBenefitPlanWeb lobjBenefitPlanWeb in iclbEnrolledPlans)
                {
                    if (lbusBenefitPlanWeb.iintPlanId == lobjBenefitPlanWeb.iintPlanId)
                    {
                        if (lbusBenefitPlanWeb.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeDeferredComp)
                        {
                            iclbEligiblePlansUI.Remove(lbusBenefitPlanWeb);
                        }
                        if (lbusBenefitPlanWeb.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
                        {
                            iclbEligiblePlansUI.Remove(lbusBenefitPlanWeb);
                        }
                        //if (lbusBenefitPlanWeb.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                        //{
                        //    //PIR -15544 Added a validation if member is enrolled  as a Retiree in a plan , the plan should not be displayed in Enrolled plans.
                        //    //if (IsInsurancePlanRetirees())
                        //    //{
                        //    //    iclbEnrolledPlansUI.Remove(lobjBenefitPlanWeb);
                        //    //}
                        //    //else
                        //    //    iclbEligiblePlansUI.Remove(lbusBenefitPlanWeb);
                        //}
                    }
                }
            }
            //PIR-10022 End
            //PIR 21636
            foreach (busBenefitPlanWeb lobjBenefitPlanWeb in iclbEligiblePlans)
            {
                int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(lobjBenefitPlanWeb.iintPlanId, ibusPerson.icdoPerson.person_id);
                if (lintPersonAccountId > 0)
                {
                    lobjBenefitPlanWeb.LoadPersonAccount(lintPersonAccountId);
                    if (lobjBenefitPlanWeb.ibusPersonAccount.ibusPlan.IsNull())
                        lobjBenefitPlanWeb.ibusPersonAccount.LoadPlan();
                    if ((lobjBenefitPlanWeb.ibusPersonAccount.ibusPlan.IsGHDVPlan() || lobjBenefitPlanWeb.ibusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife) 
                        && lobjBenefitPlanWeb.ibusPersonAccount.IsRetireePlanEnrolledOrIsCobra())
                      		iclbEligiblePlansUI.Remove(lobjBenefitPlanWeb);
                }
            }
            foreach (busBenefitPlanWeb lobjBenefitPlanWeb in iclbEnrolledPlans)
            {
                int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(lobjBenefitPlanWeb.iintPlanId, ibusPerson.icdoPerson.person_id);
                if (lintPersonAccountId > 0)
                {
                    lobjBenefitPlanWeb.LoadPersonAccount(lintPersonAccountId);
                    if (lobjBenefitPlanWeb.ibusPersonAccount.ibusPlan.IsNull())
                        lobjBenefitPlanWeb.ibusPersonAccount.LoadPlan();
                    if ((lobjBenefitPlanWeb.ibusPersonAccount.ibusPlan.IsGHDVPlan() || lobjBenefitPlanWeb.ibusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife)
                        && lobjBenefitPlanWeb.ibusPersonAccount.IsRetireePlanEnrolledOrIsCobra())
                             iclbEnrolledPlansUI.Remove(lobjBenefitPlanWeb);
                }
                
            }
            if(iclbEnrolledPlansUI.Any(BP=> BP.iintPlanId == busConstant.PlanIdGroupHealth))
            {
                busBenefitPlanWeb lbusBenefitPlanWeb = iclbEnrolledPlansUI.FirstOrDefault(BP => BP.iintPlanId == busConstant.PlanIdGroupHealth);
                if(lbusBenefitPlanWeb.IsNotNull())
                {
                    if(lbusBenefitPlanWeb.iintPersonEmploymentDetailid > 0)
                    {
                        busPersonEmploymentDetail lbusPersonEmploymentDetail = new busPersonEmploymentDetail() { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                        lbusPersonEmploymentDetail.FindPersonEmploymentDetail(lbusBenefitPlanWeb.iintPersonEmploymentDetailid);

                        if (lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                        {
                            ibusPerson.LoadACAEligibilityCertification(lbusBenefitPlanWeb.iintPersonEmploymentDetailid);
                            iintHealthEnrollmentRequestId = lbusBenefitPlanWeb.iintEnrollmentRequestId;
                            if (ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.wss_employment_aca_cert_id > 0 && lbusBenefitPlanWeb.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                                (busGlobalFunctions.CheckDateOverlapping(DateTime.Today.Date, ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.to_date.Date, ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.to_date.Date.AddDays(31))))
                            {
                                lbusBenefitPlanWeb.iblnEnrollInHealthAsTemporary = true;
                            }
                            else if (lbusBenefitPlanWeb.ibusPersonAccount?.icdoPersonAccount?.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                                (!busGlobalFunctions.CheckDateOverlapping(DateTime.Today.Date, ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.to_date.Date, ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.to_date.Date.AddDays(31))))
                            {
                                iclbEnrolledPlansUI.Remove(lbusBenefitPlanWeb);
                            }
                        }
                    }
                }
            }
                iclbEligiblePlansUI = LoadHealthPlanForAcaEligibility(iclbEligiblePlansUI);
        }
        public Collection<busBenefitPlanWeb> LoadHealthPlanForAcaEligibility(Collection<busBenefitPlanWeb> aclbEligiblePlansUI)
        {
            Collection<busBenefitPlanWeb> lclcHealthBenPlans = aclbEligiblePlansUI.Where(benefitplan => benefitplan.iintPlanId == busConstant.PlanIdGroupHealth).ToList().ToCollection();
            lclcHealthBenPlans.ForEach(bp =>
            {
                busPersonEmploymentDetail lbusPersonEmploymentDetail = new busPersonEmploymentDetail() { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lbusPersonEmploymentDetail.FindPersonEmploymentDetail(bp.iintPersonEmploymentDetailid);
                bp.istrEmpTypeValue = lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
            });
            if (lclcHealthBenPlans.Count > 1 && lclcHealthBenPlans.Any(bp => bp.istrEmpTypeValue == busConstant.PersonJobTypeTemporary) && lclcHealthBenPlans.Any(bp => bp.istrEmpTypeValue == busConstant.PersonJobTypePermanent))
            {
                return aclbEligiblePlansUI.Where(benefitplan => !(benefitplan.iintPlanId == busConstant.PlanIdGroupHealth &&
                                                                                    benefitplan.istrEmpTypeValue == busConstant.PersonJobTypeTemporary)).ToList().ToCollection();
            }
            foreach (busBenefitPlanWeb lbusBenefitPlanWeb in lclcHealthBenPlans)
            {
                bool lblnIsEligibleForHealthAsTemporary = true;
                ibusPerson.LoadACAEligibilityCertification(lbusBenefitPlanWeb.iintPersonEmploymentDetailid);
                iintHealthEnrollmentRequestId = lbusBenefitPlanWeb.iintEnrollmentRequestId;
                if (ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.wss_employment_aca_cert_id > 0 && lbusBenefitPlanWeb.istrEmpTypeValue == busConstant.PersonJobTypeTemporary)
                {
                    if ((!busGlobalFunctions.CheckDateOverlapping(DateTime.Today.Date, ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.to_date.Date, ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.to_date.Date.AddDays(31)))||
                        (ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.method == busConstant.ACACertificationMethodLookBack && ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.lb_measure == busConstant.ACACertificationLookBackTypeAnnual))
                    {
                        lblnIsEligibleForHealthAsTemporary = false;
                    }
                    else
                    {
                        lbusBenefitPlanWeb.iblnEnrollInHealthAsTemporary = true;
                    }
                }
                else if (lbusBenefitPlanWeb.istrEmpTypeValue == busConstant.PersonJobTypeTemporary)
                {
                    lblnIsEligibleForHealthAsTemporary = false;
                }
                if (!lblnIsEligibleForHealthAsTemporary)
                    aclbEligiblePlansUI = aclbEligiblePlansUI.Where(benefitplan => !(benefitplan.iintPlanId == busConstant.PlanIdGroupHealth &&
                                                                                    benefitplan.iintPersonEmploymentDetailid == lbusBenefitPlanWeb.iintPersonEmploymentDetailid)).ToList().ToCollection();
            }
            return aclbEligiblePlansUI;
        }
        //pir 5500
        private void LoadOldDBSuspendedPlanAccounts(busPersonEmployment aobjPersonEmployment)
        {
            if (aobjPersonEmployment.ibusOrganization.IsNull())
                aobjPersonEmployment.LoadOrganization();
            //prod pir 7126 : should not display plan for which org plan is ended
            if (aobjPersonEmployment.ibusOrganization.iclbOrgPlan == null)
                aobjPersonEmployment.ibusOrganization.LoadOrgPlan();
            if (aobjPersonEmployment.icolPersonEmploymentDetail.IsNull())
                aobjPersonEmployment.LoadPersonEmploymentDetail();
            foreach (busPersonEmploymentDetail lobjPED in aobjPersonEmployment.icolPersonEmploymentDetail)
            {
                if (lobjPED.iclbAllPersonAccountEmpDtl.IsNull())
                    lobjPED.LoadAllPersonAccountEmploymentDetails();
                aobjPersonEmployment.ibusLatestEmploymentDetail = lobjPED;
                foreach (busPersonAccountEmploymentDetail lobjPAED in lobjPED.iclbAllPersonAccountEmpDtl)
                {
                    if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                    {
                        if (iclbEnrolledPlans.IsNull() || iclbEnrolledPlans.Where(i => i.iintPersonAccountId == lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id && i.istrOrganizationName == aobjPersonEmployment.ibusOrganization.icdoOrganization.org_name).Count() <= 0)
                        {
                            if (lobjPAED.ibusPlan.IsNull())
                                lobjPAED.LoadPlan();
                            if (lobjPAED.ibusPlan.IsDBRetirementPlan() || lobjPAED.ibusPlan.IsHBRetirementPlan())
                            {
                                if (lobjPAED.ibusPersonAccount.IsNull())
                                    lobjPAED.LoadPersonAccount();

                                if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetimentSuspended)
                                {
                                    continue;
                                }
                                ProcessPersonAccountEmploymentDtl(aobjPersonEmployment, lobjPAED);
                            }
                        }
                    }
                }
            }
        }

        private void LoadPlanPerEmploymentDetail(busPersonEmployment aobjPersonEmployment)
        {
            if (aobjPersonEmployment.ibusOrganization.IsNull())
                aobjPersonEmployment.LoadOrganization();
            //prod pir 7126 : should not display plan for which org plan is ended
            if (aobjPersonEmployment.ibusOrganization.iclbOrgPlan == null)
                aobjPersonEmployment.ibusOrganization.LoadOrgPlan();
            if (aobjPersonEmployment.ibusLatestEmploymentDetail.IsNull())
                aobjPersonEmployment.LoadLatestPersonEmploymentDetail();

            if (aobjPersonEmployment.ibusLatestEmploymentDetail.iclbAllPersonAccountEmpDtl.IsNull())
                aobjPersonEmployment.ibusLatestEmploymentDetail.LoadAllPersonAccountEmploymentDetails();

            SetDBDCEligibility(aobjPersonEmployment.ibusLatestEmploymentDetail.iclbAllPersonAccountEmpDtl, aobjPersonEmployment,
                                aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value);

            OnlyOneDBEnrolledPlanAllowed(aobjPersonEmployment.ibusLatestEmploymentDetail.iclbAllPersonAccountEmpDtl);

            foreach (busPersonAccountEmploymentDetail lobjPAED in aobjPersonEmployment.ibusLatestEmploymentDetail.iclbAllPersonAccountEmpDtl)
            {
                //PIR - 2515
                //if (lobjPAED.icdoPersonAccountEmploymentDetail.election_value != busConstant.PersonAccountElectionValueWaived)

                //prod pir 7126 : should not display plan for which org plan is ended
                if (aobjPersonEmployment.ibusOrganization.iclbOrgPlan.Where(o => o.icdoOrgPlan.plan_id == lobjPAED.icdoPersonAccountEmploymentDetail.plan_id &&
                    (busGlobalFunctions.CheckDateOverlapping(DateTime.Today, o.icdoOrgPlan.participation_start_date, o.icdoOrgPlan.participation_end_date)
                        || (IsAnnualEnrollmentPlan(lobjPAED.icdoPersonAccountEmploymentDetail.plan_id) && IsAnnualEnrollment && o.icdoOrgPlan.participation_start_date == AnnualEnrollmentEffectiveDate))).Any()) //PIR 12012
                {
                    if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id != busConstant.PlanIdHMO
                        && ((lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdOther457 && lobjPAED.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled)
                        || lobjPAED.icdoPersonAccountEmploymentDetail.plan_id != busConstant.PlanIdOther457))
                    {
                        if (lobjPAED.ibusPlan.IsNull())
                            lobjPAED.LoadPlan();
                        if (lobjPAED.ibusPersonAccount.IsNull())
                            lobjPAED.LoadPersonAccount();

                        //PIR - 2457 only enrolled must be displayed
                        if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                        {

                            if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                            {
                                //if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled)
                                //    continue;
                            }
                            //PIR 5500
                            else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                            {
                                if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetimentSuspended)
                                {
                                    if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementEnrolled)
                                        continue;
                                }
                            }
                            //PIR 5500 The below commented is added as a part of the above condition
                            //else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                            //{
                            //    if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementEnrolled)
                            //        continue;
                            //}
                            else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
                            {
                                if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusFlexSuspended) // PIR 10394
                                {
                                    if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusFlexCompEnrolled)
                                        continue;
                                }
                            }
                            else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeDeferredComp)
                            {
                                if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusDefCompSuspended) // PIR 10394
                                {
                                    if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusDefCompEnrolled)
                                        continue;
                                }
                            }
                        }
                        ProcessPersonAccountEmploymentDtl(aobjPersonEmployment, lobjPAED);

                    }
                }
            }
            loadOldDBSuspendedPlanAccountsForEndDatedEmpDetails(aobjPersonEmployment); //pir 5500


        }

        private void loadOldDBSuspendedPlanAccountsForEndDatedEmpDetails(busPersonEmployment aobjPersonEmployment)
        {
            if (aobjPersonEmployment.icolPersonEmploymentDetail.IsNull())
                aobjPersonEmployment.LoadPersonEmploymentDetail();
            foreach (busPersonEmploymentDetail lobjPED in aobjPersonEmployment.icolPersonEmploymentDetail)
            {
                if (lobjPED != aobjPersonEmployment.ibusLatestEmploymentDetail)
                {
                    if (lobjPED.iclbAllPersonAccountEmpDtl.IsNull())
                        lobjPED.LoadAllPersonAccountEmploymentDetails();
                    aobjPersonEmployment.ibusLatestEmploymentDetail = lobjPED;
                    foreach (busPersonAccountEmploymentDetail lobjPAED in lobjPED.iclbAllPersonAccountEmpDtl)
                    {

                        if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                        {
                            if (iclbEnrolledPlans.IsNull() || iclbEnrolledPlans.Where(i => i.iintPersonAccountId == lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id && i.istrOrganizationName == aobjPersonEmployment.ibusOrganization.icdoOrganization.org_name).Count() <= 0)
                            {
                                if (lobjPAED.ibusPlan.IsNull())
                                    lobjPAED.LoadPlan();
                                if (lobjPAED.ibusPlan.IsDBRetirementPlan() || lobjPAED.ibusPlan.IsHBRetirementPlan())
                                {
                                    if (lobjPAED.ibusPersonAccount.IsNull())
                                        lobjPAED.LoadPersonAccount();

                                    if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetimentSuspended)
                                    {
                                        if(lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025 &&  
                                            !iclbEligiblePlans.Any(lobjEligiblePlan=> lobjEligiblePlan.iintPlanId == busConstant.PlanIdDC2025) &&  
                                            lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                                            ProcessPersonAccountEmploymentDtl(aobjPersonEmployment, lobjPAED);
                                        continue;
                                    }
                                    ProcessPersonAccountEmploymentDtl(aobjPersonEmployment, lobjPAED);
                                }
                            }
                        }
                    }
                }
            }
        }


        private void ProcessPersonAccountEmploymentDtl(busPersonEmployment aobjPersonEmployment, busPersonAccountEmploymentDetail lobjPAED)
        {
            busBenefitPlanWeb lobjBenefitPlan = new busBenefitPlanWeb();
            int lintPersonAccountId = 0;
            bool lblnAddInInsurance = false;
            lobjPAED.ibusEmploymentDetail = aobjPersonEmployment.ibusLatestEmploymentDetail;
            lobjBenefitPlan.iintOrgId = aobjPersonEmployment.icdoPersonEmployment.org_id;
            lobjBenefitPlan.iintPlanId = lobjPAED.ibusPlan.icdoPlan.plan_id;

            //PIR 24035
            Collection<busCodeValue> lclbLineOfDutySurvivorOrgCode = busGlobalFunctions.LoadData1ByCodeID(7022);

            busOrganization lbusOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
            lbusOrg.FindOrganization(aobjPersonEmployment.icdoPersonEmployment.org_id);

            bool lblnIsLineOfDutySurvivor = lclbLineOfDutySurvivorOrgCode.Any(lbus => lbus.icdoCodeValue.data1 == lbusOrg.icdoOrganization.org_code);

            if (aobjPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
            {
                var lPlanRequestList = ibusPerson.iclbWSSEnrollmentRequest.
                    Where(lobjRequest => lobjRequest.icdoWssPersonAccountEnrollmentRequest.plan_id == lobjPAED.icdoPersonAccountEmploymentDetail.plan_id
                    && (lobjRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id == lobjPAED.icdoPersonAccountEmploymentDetail.person_employment_dtl_id)
                    && (lobjRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusPendingRequest
                    || lobjRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusRejected));

                if (lPlanRequestList.Count() > 0)
                    lobjBenefitPlan.iintEnrollmentRequestId = lPlanRequestList.FirstOrDefault().icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;

                if (lobjBenefitPlan.iintEnrollmentRequestId > 0)
                {
                    //set status of the erollment request
                    busWssPersonAccountEnrollmentRequest lobjPersonAccountErollmentRequest = new busWssPersonAccountEnrollmentRequest();
                    lobjPersonAccountErollmentRequest.FindWssPersonAccountEnrollmentRequest(lobjBenefitPlan.iintEnrollmentRequestId);

                    lobjBenefitPlan.istrEnrollmentRequestStatus = busConstant.EnrollmentStatusUnderReview;
                    if (lobjPersonAccountErollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusPosted)
                        lobjBenefitPlan.istrEnrollmentRequestStatus = "";
                    if (lobjPersonAccountErollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusRejected)
                        lobjBenefitPlan.istrEnrollmentRequestStatus = busConstant.EnrollmentStatusRejected;

                    SetPlanNameVisibilityIfRequestExists(lobjPAED, lobjBenefitPlan);
                }
                else
                {
                    //check for retirement
                    if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                    { //check for DB
                        if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                        {
                            if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC ||
                                lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                                lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2025)
                            {
                                if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementRetired
                                   && lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn)
                                    lintPersonAccountId = lobjPAED.ibusPersonAccount.icdoPersonAccount.person_account_id;
                            }
                            else
                            {
                                if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                                {
                                    if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementRetired)
                                        lintPersonAccountId = lobjPAED.ibusPersonAccount.icdoPersonAccount.person_account_id;
                                }
                            }
                        }
                        SetPlanNameEligibilityForRetirement(aobjPersonEmployment, lobjPAED, lobjBenefitPlan, lintPersonAccountId);
                    }
                    else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                    {
                        SetPlanNameEligibilityForInsurance(lobjPAED, lobjBenefitPlan, false);
                    }
                    else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
                    {
                        if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdFlex)
                        {
                            lobjBenefitPlan.iblnEligibleFlexCompInsuranceEnrollment = true;

                            if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                                lobjBenefitPlan.istrPlanNameNewFlexCompInsurance = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                            else
                                lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                        }
                    }
                    else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeDeferredComp)
                    {
                        ibusPerson.LoadPersonAccountByPlan(lobjPAED.icdoPersonAccountEmploymentDetail.plan_id);
                        var lenum = ibusPerson.icolPersonAccountByPlan.Where(lobjPA => lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled);
                        if (lenum.Count() > 0)
                            lintPersonAccountId = lenum.FirstOrDefault().icdoPersonAccount.person_account_id;
                        //bool lblnIsPlanfromSameOrg = true;
                        //lobjPAED.ibusEmploymentDetail.LoadPersonEmployment(); //Added 22
                        int lintCount = iclbEnrolledPlans.Where(o => o.iintOrgId == lobjBenefitPlan.iintOrgId && o.iintPlanId == lobjBenefitPlan.iintPlanId).Count(); //Added 22
                        bool lblnIsPlanfromSameOrg = lintCount == 0 ? false : true; //Added 22

                        if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDeferredCompensation)
                        {
                            if ((lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                                && (lintPersonAccountId == 0))
                            {
                                lobjBenefitPlan.iblnEligibleDeferredCompEnrollment = true;
                                lobjBenefitPlan.istrPlanNameNewDeferredComp = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                            }
                            else if (lintPersonAccountId != 0 && lobjPAED.icdoPersonAccountEmploymentDetail.election_value != busConstant.PersonAccountElectionValueEnrolled)
                            {
                                lobjBenefitPlan.iblnOpenEligibleDeferredCompEnrollment = true;
                                lobjBenefitPlan.istrPlanNameOpenEligibleDeferredComp = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                                if (lblnIsPlanfromSameOrg)
                                {
                                    lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id = lintPersonAccountId;
                                }
                            }
                            else
                            {
                                lobjBenefitPlan.iblnEligibleDeferredCompEnrollment = true;
                                lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                            }
                        }

                        else
                        {
                            lobjBenefitPlan.iblnEligibleDeferredCompEnrollment = true;
                            lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                            lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id = lintPersonAccountId;

                        }
                    }
                    else
                    {
                        // TODO need to change as per plans
                        lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                    }

                }
                if (lobjPAED.icdoPersonAccountEmploymentDetail.election_value.IsNullOrEmpty() ||
                    (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0 &&
                    lobjPAED.icdoPersonAccountEmploymentDetail.election_value != busConstant.PersonAccountElectionValueWaived) ||
                    (lobjPAED.ibusPlan.IsInsurancePlan() && lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0 && // PIR 9793
                     lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled))
                {
                    //if (lobjPAED.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueWaived)
                    //    lobjPAED.icdoPersonAccountEmploymentDetail.election_description = busConstant.PlanParticipationStatusDescriptionWaived;
                    //else
                        lobjPAED.icdoPersonAccountEmploymentDetail.election_description = busConstant.PlanParticipationStatusDescriptionEligible;
                    if (String.IsNullOrEmpty(lobjBenefitPlan.istrEnrollmentRequestStatus))
                        lobjBenefitPlan.istrEnrollmentRequestStatus = busConstant.EnrollmentStatusEligible;
                }//PIR-10057 Start 
                //else if ((lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0 &&
                //    lobjPAED.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueWaived)||
                //    (lobjPAED.ibusPlan.IsInsurancePlan() && lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0 &&
                //     lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled))
                //{
                //    lobjPAED.icdoPersonAccountEmploymentDetail.election_description = busConstant.PlanParticipationStatusDescriptionWaived;
                //    if (String.IsNullOrEmpty(lobjBenefitPlan.istrEnrollmentRequestStatus))
                //        lobjBenefitPlan.istrEnrollmentRequestStatus = busConstant.EnrollmentStatusEligible;
                //}//PIR-10057 End
                lobjBenefitPlan.istrJobClassValue = aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value;
                lobjBenefitPlan.idtStartDate = aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.start_date;
                lobjBenefitPlan.iintPlanId = lobjPAED.icdoPersonAccountEmploymentDetail.plan_id;
                lobjBenefitPlan.iintPersonAccountId = lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id;
                lobjBenefitPlan.istrElectionValue = lobjPAED.icdoPersonAccountEmploymentDetail.election_value;
                lobjBenefitPlan.istrElectionDescription = lobjPAED.icdoPersonAccountEmploymentDetail.election_description;
                lobjBenefitPlan.istrOrganizationName = aobjPersonEmployment.ibusOrganization.icdoOrganization.org_name;
                lobjBenefitPlan.iintPlanId = lobjPAED.icdoPersonAccountEmploymentDetail.plan_id;
                lobjBenefitPlan.iintPersonEmploymentDetailid = lobjPAED.icdoPersonAccountEmploymentDetail.person_employment_dtl_id;
                //pir 6057
                //--start--//
                lblnAddInInsurance = false;

                if (lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                {
                    if (!lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() &&
                        !lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
                    {
                        DataTable ldtbPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[3] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE" },
                                               new object[3] { lobjBenefitPlan.iintPlanId, lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.job_class_value,
                                               lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.type_value }, null, null);
                        //Checking whether plan is eligible for given job class and job type
                        if (ldtbPlanjobClass.Rows.Count > 0)
                        {
                            if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value != busConstant.PlanBenefitTypeRetirement ||
                                lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing ||
                                lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusNonContributing)
                            {
                                bool lblnRetirementNonContrubuting = false;
                                if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement &&
                                    lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusNonContributing)
                                    lblnRetirementNonContrubuting = true;
                                if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                                    iclbEnrolledPlans.Add(lobjBenefitPlan);

                                else if (!lblnRetirementNonContrubuting) // PIR 9789 corrected in PIR 10087
                                {  // PIR 25290 for DC25 for Temporary so it does not show as eligible after 180 days
                                    if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2025)
                                    {
                                        lobjPAED.ibusEmploymentDetail.SetMSSIsTempEmploymentDtlWithinFirstSixMonths(DateTime.Now);
                                        if (lobjPAED.ibusEmploymentDetail.iblnIsMSSTempEmploymentDtlWithinFirstSixMonths)
                                            iclbEligiblePlans.Add(lobjBenefitPlan);
                                    }
                                    else
                                    {
                                        iclbEligiblePlans.Add(lobjBenefitPlan);
                                    }
                                }
                                lblnAddInInsurance = true;

                            }
                        }
                    }
                }
                //prod pir 6371
                else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                {
                    if (lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing ||
                        lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusNonContributing ||
                        lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA ||
                        lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM ||
                        lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA)
                    {
                        if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id != 0)//PIR 5500
                        {
                            if (!iclbEligiblePlans.Where(lobj => lobj.iintPlanId == lobjBenefitPlan.iintPlanId).Any())// PIR 9709
                                iclbEnrolledPlans.Add(lobjBenefitPlan);
                            else if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0 && !iclbEligiblePlans.Where(lobj => lobj.iintPlanId == lobjBenefitPlan.iintPlanId).Any())//Added 22 //PIT 25920 an issue if the plan is Enrolled/Suspended and a new detail starts with a past date
                                iclbEnrolledPlans.Add(lobjBenefitPlan);
                        }
                        else if (lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value != busConstant.EmploymentStatusNonContributing) // PIR 9789
                            iclbEligiblePlans.Add(lobjBenefitPlan);
                    }
                }
                else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                {
                    // PROD PIR 6379 -- Eligiblility issue - Insurance plans should only show for the employer where the enrollment is linked to.  
                    // Only if no enrollment exists could both show as eligible.
                    if (ibusPerson.icolPersonAccount.IsNull()) ibusPerson.LoadPersonAccount();
                    if (lobjPAED.icdoPersonAccountEmploymentDetail.election_value.IsNotNullOrEmpty() &&
                        ibusPerson.icolPersonAccount.Where(lobj => lobj.icdoPersonAccount.plan_id == lobjPAED.ibusPlan.icdoPlan.plan_id &&
                                                                 lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Any())
                    {
                        if (!(lblnIsLineOfDutySurvivor && lobjBenefitPlan.iintPlanId == busConstant.PlanIdGroupHealth))
                            iclbEnrolledPlans.Add(lobjBenefitPlan);
                    }
                    else
                    {
                        busPersonAccount lobjPA = ibusPerson.LoadActivePersonAccountByPlan(lobjPAED.ibusPlan.icdoPlan.plan_id);
                        lobjPA.LoadPersonAccountEmploymentDetails();
                        if (lobjPA.iclbAccountEmploymentDetail.Where(lobj => lobj.icdoPersonAccountEmploymentDetail.person_employment_dtl_id ==
                            lobjPAED.icdoPersonAccountEmploymentDetail.person_employment_dtl_id).Any() ||
                            lobjPAED.icdoPersonAccountEmploymentDetail.election_value.IsNullOrEmpty() ||
                            lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended 
                            && !(lblnIsLineOfDutySurvivor && lobjBenefitPlan.iintPlanId == busConstant.PlanIdGroupHealth))// PIR 7976   
                            iclbEligiblePlans.Add(lobjBenefitPlan);
                        //PIR-10057 Start
                        else if (lobjPA.icdoPersonAccount.person_account_id == 0 && lobjPAED.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueWaived
                            && !(lblnIsLineOfDutySurvivor && lobjBenefitPlan.iintPlanId == busConstant.PlanIdGroupHealth))
                        {
                            if (String.IsNullOrEmpty(lobjBenefitPlan.istrEnrollmentRequestStatus))
                                lobjBenefitPlan.istrEnrollmentRequestStatus = busConstant.EnrollmentStatusEligible;
                            iclbEligiblePlans.Add(lobjBenefitPlan);
                        }
                        //PIR-10057 End
                    }
                }
                // PIR 10394 - if employment is active flex/def comp should be shown as Eligible
                else if ((lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeDeferredComp)
                        || (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex))
                {
                    busPersonAccount lobjPA = ibusPerson.LoadActivePersonAccountByPlan(lobjPAED.ibusPlan.icdoPlan.plan_id);
                    if ((lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompSuspended)
                        || (lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended))
                    {
                        if (string.IsNullOrEmpty(lobjBenefitPlan.istrEnrollmentRequestStatus))
                            lobjBenefitPlan.istrEnrollmentRequestStatus = busConstant.EnrollmentStatusEligible;
                        lobjBenefitPlan.istrElectionDescription = busConstant.PlanParticipationStatusDescriptionEligible;
                        iclbEligiblePlans.Add(lobjBenefitPlan);
                    }
                    else
                    {
                        if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                            iclbEnrolledPlans.Add(lobjBenefitPlan);
                        else
                            iclbEligiblePlans.Add(lobjBenefitPlan);
                        lblnAddInInsurance = true;
                    }

                }
                else
                {
                    if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                        iclbEnrolledPlans.Add(lobjBenefitPlan);
                    else
                        iclbEligiblePlans.Add(lobjBenefitPlan);
                    lblnAddInInsurance = true;
                }

                if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance && lblnAddInInsurance)
                    iclbInsurancePlans.Add(lobjPAED);
                //--end--//

            }
            else
            {
                lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                {
                    if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                        SetPlanNameEligibilityForInsurance(lobjPAED, lobjBenefitPlan, true);
                    else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
                    {
                        lobjBenefitPlan.iblnEligibleFlexCompInsuranceEnrollment = true;

                        if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                            lobjBenefitPlan.istrPlanNameNewFlexCompInsurance = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                        else
                            lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                    }
                    if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                    {
                        if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                        {
                            if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC ||
                                lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2020 ||
                                lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2025) //PIR 20232 PIR 25920
                            {
                                if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementRetired
                                   && lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn)
                                    lintPersonAccountId = lobjPAED.ibusPersonAccount.icdoPersonAccount.person_account_id;
                            }
                            else
                            {
                                if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                                {
                                    if (lobjPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementRetired)
                                        lintPersonAccountId = lobjPAED.ibusPersonAccount.icdoPersonAccount.person_account_id;
                                }
                            }
                        }

                        SetPlanNameEligibilityForRetirement(aobjPersonEmployment, lobjPAED, lobjBenefitPlan, lintPersonAccountId);
                    }
                    else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeDeferredComp)
                    {
                        lobjBenefitPlan.iblnEligibleDeferredCompEnrollment = true;

                        if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                            lobjBenefitPlan.istrPlanNameNewDeferredComp = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                        else
                            lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                    }
                    //employment is ended this must navigate to the enrolled screen and the update button should not available to this plans
                    lobjPAED.icdoPersonAccountEmploymentDetail.election_value = busConstant.PlanOptionStatusValueEnrolled;
                    lobjBenefitPlan.istrJobClassValue = aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value;
                    lobjBenefitPlan.idtStartDate = aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.start_date;
                    lobjBenefitPlan.iintPlanId = lobjPAED.icdoPersonAccountEmploymentDetail.plan_id;
                    lobjBenefitPlan.iintPersonAccountId = lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id;
                    lobjBenefitPlan.istrElectionValue = lobjPAED.icdoPersonAccountEmploymentDetail.election_value;
                    lobjBenefitPlan.istrElectionDescription = lobjPAED.icdoPersonAccountEmploymentDetail.election_description;
                    lobjBenefitPlan.istrOrganizationName = aobjPersonEmployment.ibusOrganization.icdoOrganization.org_name;
                    lobjBenefitPlan.iintPlanId = lobjPAED.icdoPersonAccountEmploymentDetail.plan_id;
                    lobjBenefitPlan.iintPersonEmploymentDetailid = lobjPAED.icdoPersonAccountEmploymentDetail.person_employment_dtl_id;

                    //pir 6057
                    //--start--//
                    lblnAddInInsurance = false;

                    if (lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    {
                        if (!lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() &&
                            !lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
                        {
                            DataTable ldtbPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[3] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE" },
                                                   new object[3] { lobjBenefitPlan.iintPlanId, lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.job_class_value,
                                               lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.type_value }, null, null);
                            //Checking whether plan is eligible for given job class and job type
                            if (ldtbPlanjobClass.Rows.Count > 0)
                            {
                                if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value != busConstant.PlanBenefitTypeRetirement ||
                                    lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing ||
                                    lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusNonContributing)
                                {
                                    bool lblnRetirementNonContrubuting = false;
                                    if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement &&
                                        lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusNonContributing)
                                        lblnRetirementNonContrubuting = true;
                                    if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id > 0)
                                        iclbEnrolledPlans.Add(lobjBenefitPlan);
                                    else if (!lblnRetirementNonContrubuting) // PIR 9789 corrected in PIR 10087
                                        iclbEligiblePlans.Add(lobjBenefitPlan);
                                    lblnAddInInsurance = true;
                                }
                            }
                        }
                    }
                    //prod pir 6371
                    else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                    {
                        if (lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing ||
                            (lobjPAED.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusNonContributing &&
                            lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id != 0))//PIR 5500
                        {
                            if (!iclbEligiblePlans.Where(lobj => lobj.iintPlanId == lobjBenefitPlan.iintPlanId).Any()) // PIR 9709
                                iclbEnrolledPlans.Add(lobjBenefitPlan);
                        }
                    }
                    else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                    {
                        // PROD PIR 6379 -- Eligiblility issue - Insurance plans should only show for the employer where the enrollment is linked to.  
                        // Only if no enrollment exists could both show as eligible.
                        if (ibusPerson.icolPersonAccount.IsNull()) ibusPerson.LoadPersonAccount();
                        if (ibusPerson.icolPersonAccount.Where(lobj => lobj.icdoPersonAccount.plan_id == lobjPAED.ibusPlan.icdoPlan.plan_id &&
                                                                      lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Any())
                        {
                            busPersonAccount lobjPA = ibusPerson.LoadActivePersonAccountByPlan(lobjPAED.ibusPlan.icdoPlan.plan_id);
                            lobjPA.LoadPersonAccountEmploymentDetails();
                            if (lobjPA.iclbAccountEmploymentDetail.Where(lobj => lobj.icdoPersonAccountEmploymentDetail.person_employment_dtl_id ==
                                lobjPAED.icdoPersonAccountEmploymentDetail.person_employment_dtl_id).Any())
                                if (!(lblnIsLineOfDutySurvivor && lobjBenefitPlan.iintPlanId == busConstant.PlanIdGroupHealth))
                                    iclbEnrolledPlans.Add(lobjBenefitPlan);
                        }
                        else
                        {
                            if (!(lblnIsLineOfDutySurvivor && lobjBenefitPlan.iintPlanId == busConstant.PlanIdGroupHealth))
                                iclbEligiblePlans.Add(lobjBenefitPlan);
                        }
                    }
                    else
                    {
                        iclbEnrolledPlans.Add(lobjBenefitPlan);
                        lblnAddInInsurance = true;
                    }

                    if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance && lblnAddInInsurance)
                        iclbInsurancePlans.Add(lobjPAED);
                    //--end--//
                }
            }
        }

        private void SetPlanNameEligibilityForInsurance(busPersonAccountEmploymentDetail lobjPAED, busBenefitPlanWeb lobjBenefitPlan, bool ablnIsEmploymentEnded)
        {
            string planStatus = string.Empty; // PIR 10269

            if (lobjPAED.ibusPersonAccount.IsNull())
                lobjPAED.LoadPersonAccount();

            if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupHealth
                || lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDental
                || lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdMedicarePartD
                || lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdVision)
            {
                lobjBenefitPlan.iblnEligibleGHDVInsuranceEnrollment = true;
                if (!ablnIsEmploymentEnded)
                {
                    if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                        lobjBenefitPlan.istrPlanNameNewGHDVInsurance = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                    else
                        lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                }
                else
                    lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
            }
            else if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupLife)
            {
                lobjBenefitPlan.iblnEligibleLifeInsuranceEnrollment = true;
                if (!ablnIsEmploymentEnded)
                {
                    if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                        lobjBenefitPlan.istrPlanNameNewLifeInsurance = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                    else
                        lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                }
                else
                {
                    lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                }
            }
            else if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdEAP)
            {
                lobjBenefitPlan.iblnEligibleEAPInsuranceEnrollment = true;
                int iintpersonaccountid = 0;
                ibusPerson.LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeInsurance);
                var lenum = ibusPerson.icolPersonAccountByBenefitType.Where(lobjPA => lobjPA.icdoPersonAccount.plan_id == busConstant.PlanIdEAP
                    && lobjPA.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceCancelled);
                if (lenum.Count() > 0)
                {
                    lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id = lenum.FirstOrDefault().icdoPersonAccount.person_account_id;
                    planStatus = lenum.FirstOrDefault().icdoPersonAccount.plan_participation_status_value; // PIR 10269
                }

                // PIR 10269
                if (!ablnIsEmploymentEnded && planStatus == busConstant.PlanParticipationStatusInsuranceSuspended)
                {
                    lobjBenefitPlan.iblnOpenEmploymentEAP = true;
                }
                else
                {
                    lobjBenefitPlan.iblnOpenEmploymentEAP = false;
                }

                if (lenum.Count() > 0)
                    lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id = lenum.FirstOrDefault().icdoPersonAccount.person_account_id;

                if (!ablnIsEmploymentEnded)
                {
                    if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                    {
                        lobjBenefitPlan.istrPlanNameNewEAPInsurnace = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                        lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id = iintpersonaccountid;
                    }
                    else
                    {
                        lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                    }
                }
                else
                {
                    lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                }
            }
            else if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdLTC)
            {
                lobjBenefitPlan.iblnEligibleLTCInsuranceEnrollment = true;
                if (!ablnIsEmploymentEnded)
                {
                    if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                        lobjBenefitPlan.istrPlanNameNewLTCInsurance = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                    else
                        lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                }
                else
                {
                    lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                }
            }
            else if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdHMO)
            {
                lobjBenefitPlan.iblnEligibleHMOInsuranceEnrollment = true;
                if (!ablnIsEmploymentEnded)
                {
                    if (lobjPAED.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                        lobjBenefitPlan.istrPlanNameNewHMOInsurance = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                    else
                        lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                }
                else
                {
                    lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                }
            }
        }

        private void SetPlanNameEligibilityForRetirement(busPersonEmployment aobjPersonEmployment, busPersonAccountEmploymentDetail lobjPAED, busBenefitPlanWeb lobjBenefitPlan, int lintPersonAccountId)
        {
            bool lblnValueSet = false;
            if (lobjPAED.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || lobjPAED.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB) //PIR 25920 New Plan DC 2025
            {
                if (lintPersonAccountId == 0)
                {
                    if (aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassNonStateElectedOfficial ||
                        aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                    //PIR 23522 - HP and Judges plan should not land on Elected Official screen. 
                    //|| aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.JobClassJudge ||
                    //aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.JobClassHighwayPatrolPerson) // PIR 9777
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                                                                aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                                                aobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddMonths(6)))
                        {
                            lobjBenefitPlan.istrPlanNameNewDBElectedOfficial = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                            lobjBenefitPlan.iblnEligibleDBRetirementElectedOffcialEnrollment = true;
                            lblnValueSet = true;
                        }
                    }
                }
                //if main is optional
                if ((lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdMain ||
                    lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdMain2020 ||
                    lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2025)//PIR 20232 ?code//PIR 25920 New Plan DC 2025
                    && (!lblnValueSet))
                {
                    aobjPersonEmployment.ibusLatestEmploymentDetail.SetMSSIsTempEmploymentDtlWithinFirstSixMonths(DateTime.Now);

                    if (aobjPersonEmployment.ibusLatestEmploymentDetail.iblnIsMSSTempEmploymentDtlWithinFirstSixMonths)
                    {
                        if (lintPersonAccountId == 0)
                        {
                            lobjBenefitPlan.istrPlanNameNewDBOptionalRetirement = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                            lobjBenefitPlan.iblnEligibleDBRetirementOptionalEnrollment = true;
                            lblnValueSet = true;
                        }
                    }
                }//if DB is elected official - 
                //if(lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2025)
                //    lobjBenefitPlan.istrPlanNameDC25 = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                if (!lblnValueSet)
                {
                    if (lintPersonAccountId == 0)
                    {
                        lobjBenefitPlan.istrPlanNameNewDBRetirement = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                        lobjBenefitPlan.iblnEligibleDBRetirementEnrollment = true;
                    }
                    else
                        lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                }
            }
            else
            {
                if (lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC ||
                    lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2020 ||
                    lobjPAED.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2025) //PIR 20232 PIR 25920
                {
                    aobjPersonEmployment.ibusLatestEmploymentDetail.SetMSSIsTempEmploymentDtlWithinFirstSixMonths(DateTime.Now);

                    if (aobjPersonEmployment.ibusLatestEmploymentDetail.iblnIsMSSTempEmploymentDtlWithinFirstSixMonths)
                    {
                        if (lintPersonAccountId == 0)
                        {
                            lobjBenefitPlan.iblnEligibleDCRetirementOptionalEnrollment = true;
                            lobjBenefitPlan.istrPlanNameNewDCOptionalRetirement = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                        }
                        else
                            lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                    }
                    if ((lobjPAED.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueWaived)
                        || ((String.IsNullOrEmpty(lobjBenefitPlan.istrPlanNameNewDCRetirement))
                        && (String.IsNullOrEmpty(lobjBenefitPlan.istrPlanNameNewDCOptionalRetirement))))
                    {
                        if (lintPersonAccountId == 0)
                        {
                            lobjBenefitPlan.istrPlanNameNewDCRetirement = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                            lobjBenefitPlan.iblnEligibleDCRetirementEnrollment = true;
                        }
                        else
                            lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
                    }
                }
            }
        }

        private void SetPlanNameVisibilityIfRequestExists(busPersonAccountEmploymentDetail lobjPAED, busBenefitPlanWeb lobjBenefitPlan)
        {
            busWssPersonAccountEnrollmentRequest lobjPersonAccountEnrollmentrequest = new busWssPersonAccountEnrollmentRequest();
            lobjPersonAccountEnrollmentrequest.FindWssPersonAccountEnrollmentRequest(lobjBenefitPlan.iintEnrollmentRequestId);

            lobjBenefitPlan.istrPlanNameUpdate = lobjPAED.ibusPlan.icdoPlan.mss_plan_name;
            {
                if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                {
                    if (lobjPAED.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || lobjPAED.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB) //PIR 25920  New DC plan
                    {
                        if (lobjPersonAccountEnrollmentrequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial)
                            lobjBenefitPlan.iblnEligibleDBRetirementElectedOffcialEnrollment = true;

                        else if (lobjPersonAccountEnrollmentrequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional)
                            lobjBenefitPlan.iblnEligibleDBRetirementOptionalEnrollment = true;

                        else if (lobjPersonAccountEnrollmentrequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBRetirement)
                            lobjBenefitPlan.iblnEligibleDBRetirementEnrollment = true;
                    }
                    else if (lobjPAED.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC)
                    {
                        if (lobjPersonAccountEnrollmentrequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCOptional)
                            lobjBenefitPlan.iblnEligibleDCRetirementOptionalEnrollment = true;

                        else if (lobjPersonAccountEnrollmentrequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCRetirement)
                            lobjBenefitPlan.iblnEligibleDCRetirementEnrollment = true;
                    }
                }
                else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                {
                    if (lobjPersonAccountEnrollmentrequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeGHDV)
                        lobjBenefitPlan.iblnEligibleGHDVInsuranceEnrollment = true;
                    else if (lobjPersonAccountEnrollmentrequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeLife)
                        lobjBenefitPlan.iblnEligibleLifeInsuranceEnrollment = true;
                }
                else if (lobjPAED.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
                {
                    lobjBenefitPlan.iblnEligibleFlexCompInsuranceEnrollment = true;
                }
            }
        }

        public Collection<busPersonEmploymentDetail> iclbPersonEmploymentDetail { get; set; }

        public void LoadAllPersonEmploymentDetails()
        {
            if (iclbPersonEmploymentDetail == null)
                iclbPersonEmploymentDetail = new Collection<busPersonEmploymentDetail>();

            if (ibusPerson.icolPersonEmployment.IsNull())
                ibusPerson.LoadPersonEmployment();

            foreach (busPersonEmployment lobjPersonEmployment in ibusPerson.icolPersonEmployment)
            {
                if (lobjPersonEmployment.icolPersonEmploymentDetail == null)
                    lobjPersonEmployment.LoadPersonEmploymentDetail();
                foreach (busPersonEmploymentDetail lobjPersonEmpDtl in lobjPersonEmployment.icolPersonEmploymentDetail)
                {
                    iclbPersonEmploymentDetail.Add(lobjPersonEmpDtl);
                }
            }
        }

        public void LoadMessageBoard()
        {
            iclbWSSMessageDetails = new Collection<busWssMessageDetail>();
            DataTable ldtMessageDetail = Select<cdoWssMessageDetail>(new string[2] { enmWssMessageDetail.person_id.ToString(), enmWssMessageDetail.clear_message_flag.ToString() },
                                                                        new object[2] { ibusPerson.icdoPerson.person_id, busConstant.Flag_No }, null, null);
            iclbWSSMessageDetails = GetCollection<busWssMessageDetail>(ldtMessageDetail, "icdoWssMessageDetail");
            foreach (busWssMessageDetail lobjWSSMessageDetail in iclbWSSMessageDetails)
            {
                lobjWSSMessageDetail.LoadWSSMessageHeader();
                if (lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_id > 0)
                {

                    utlMessageInfo lobjutlMessageInfo = new utlMessageInfo();
                    lobjutlMessageInfo = utlPassInfo.iobjPassInfo.isrvDBCache.GetMessageInfo(lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_id);
                    if (lobjutlMessageInfo.IsNotNull())
                    {
                        lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.display_message = lobjutlMessageInfo.display_message;
                    }

                    //DataTable ldtbResult = iobjPassInfo.isrvDBCache.GetMessageInfo(lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_id);
                    //if (ldtbResult != null && ldtbResult.Rows.Count > 0)
                    //    lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.display_message = ldtbResult.Rows[0]["display_message"].ToString();
                }
                else
                {
                    lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.display_message = lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.message_text;
                }
				//PIR-19351
				if (lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.display_message.IsNotNullOrEmpty())
                {
                    if (lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.display_message.Contains("#"))
                    {
                        string lstrMsgText = lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.display_message;
                        int lintIndex = lstrMsgText.IndexOf('#');
                        if (lintIndex != -1)
                        {
                            string lstrEndText = lstrMsgText.Substring(lintIndex + 1);
                            string lstrContactTicketId = !string.IsNullOrEmpty(lstrMsgText.Substring(lintIndex + 1, lstrEndText.IndexOf(' ')))
                                                            ? lstrMsgText.Substring(lintIndex + 1, lstrEndText.IndexOf(' ')).Trim()
                                                            : lstrMsgText.Substring(lintIndex + 1, lstrEndText.IndexOf(' '));
                            int lintContactTicketId = 0;
                            if (int.TryParse(lstrContactTicketId, out lintContactTicketId))
                            {
                                lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.display_message = !string.IsNullOrEmpty(lstrMsgText.Substring(0, lintIndex + 1))
                                                                                                        ? lstrMsgText.Substring(0, lintIndex + 1).Trim()
                                                                                                        : lstrMsgText.Substring(0, lintIndex + 1);
                                lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.istrContactTicketId = Convert.ToString(lintContactTicketId);
                                if (!string.IsNullOrEmpty(lstrEndText) && lstrEndText.Length > Convert.ToString(lintContactTicketId).Length)
                                    lobjWSSMessageDetail.ibusWSSMessageHeader.icdoWssMessageHeader.istrEndText = "&nbsp;"+ lstrEndText.Substring(Convert.ToString(lintContactTicketId).Length);
                            }
                        }
                    }
                }
            }
        }

        public ArrayList btnClearMessage_Click(ArrayList aarrSelectedObjects)
        {
            ArrayList larrlist = new ArrayList();
            if (aarrSelectedObjects != null)
            {
                foreach (busWssMessageDetail lobjDetail in aarrSelectedObjects)
                {
                    iclbWSSMessageDetails.Remove(lobjDetail);
                    lobjDetail.icdoWssMessageDetail.correspondence_link = "Please contact NDPERS for correspondence";
                    lobjDetail.icdoWssMessageDetail.clear_message_flag = busConstant.Flag_Yes;
                    lobjDetail.icdoWssMessageDetail.Update();
                }
            }

            larrlist.Add(this);
            return larrlist;
        }

        //need to load the 1099R for the person for only last 3 years BR -24  06
        public Collection<busPayment1099r> iclb1099RForLast3Yrs { get; set; }
        public void Load1099RForLast3Yrs()
        {
            iclb1099RForLast3Yrs = new Collection<busPayment1099r>();

            if (ibusPerson.iclbPayment1099R.IsNull())
                ibusPerson.LoadPayment1099R();

            var lenumPayment1099RForLast3Yrs = ibusPerson.iclbPayment1099R.Where(lobj1099R => lobj1099R.icdoPayment1099r.tax_year >= DateTime.Now.AddYears(-3).Year);

            foreach (busPayment1099r lobj1099R in lenumPayment1099RForLast3Yrs)
            {
                //Start: Added for PIR 8890 to load Plan Name.
                lobj1099R.LoadPayeeAccount();
                if (lobj1099R.ibusPayeeAccount.icdoPayeeAccount.application_id.IsNotNull())
                {
                    lobj1099R.ibusPayeeAccount.LoadApplication();
                    lobj1099R.ibusPayeeAccount.ibusApplication.LoadPlan();
                    lobj1099R.istrPlanName = lobj1099R.ibusPayeeAccount.ibusApplication.ibusPlan.icdoPlan.plan_name;
                    //PIR 11131
                    if (lobj1099R.icdoPayment1099r.corrected_flag == busConstant.Flag_Yes)
                        lobj1099R.istrCorrected1099R = lobj1099R.icdoPayment1099r.tax_year + "  " + busConstant.Corrected1099R;
                    else
                        lobj1099R.istrCorrected1099R = (lobj1099R.icdoPayment1099r.tax_year).ToString();
                }
                else
                {
                    lobj1099R.ibusPayeeAccount.LoadDROApplication();
                    lobj1099R.ibusPayeeAccount.ibusDROApplication.LoadPlan();
                    lobj1099R.istrPlanName = lobj1099R.ibusPayeeAccount.ibusDROApplication.ibusPlan.icdoPlan.plan_name;
                    //PIR 11131
                    if (lobj1099R.icdoPayment1099r.corrected_flag == busConstant.Flag_Yes)
                        lobj1099R.istrCorrected1099R = lobj1099R.icdoPayment1099r.tax_year + "  " + busConstant.Corrected1099R;
                    else
                        lobj1099R.istrCorrected1099R = (lobj1099R.icdoPayment1099r.tax_year).ToString();
                }
                //End: PIR 8890
                iclb1099RForLast3Yrs.Add(lobj1099R);
            }
        }

        //need to load the 1099R for the person for only last 3 years BR -24  06
        public Collection<busMASStatementFile> iclbAnnualStatementsForLast3Yrs { get; set; }
        public void LoadAnnualStatementsForLast3Yrs()
        {
            iclbAnnualStatementsForLast3Yrs = new Collection<busMASStatementFile>();
            if (ibusPerson.iclbMASStatementFile.IsNull())
                ibusPerson.LoadMASStatementFile();

            //PIR 9790
            var lenumAnnualStatementForLast3Yrs = ibusPerson.iclbMASStatementFile.Where(lobjAS =>
                                            lobjAS.ibusMASSelection.ibusBatchRequest.icdoMasBatchRequest.statement_effective_date.Year >= DateTime.Now.AddYears(-3).Year);

            foreach (busMASStatementFile lobjAS in lenumAnnualStatementForLast3Yrs)
            {
                // Annual Statements - PIR 17506
                //PIR-17506
                //if ((lobjAS.ibusMASSelection.ibusBatchRequest.icdoMasBatchRequest.group_type_value == busConstant.GroupTypeRetired) ||
                //    (lobjAS.ibusMASSelection.ibusBatchRequest.icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired &&
                //    lobjAS.icdoMasStatementFile.statement_type_value == busConstant.OnlineStatementFile))
                //{
                iclbAnnualStatementsForLast3Yrs.Add(lobjAS);
                //}
            }
        }

        public Collection<busPayeeAccount> iclbMemberPlusAlternateActivePayeeAccounts { get; set; }
        public void LoadPayeeAccounts()
        {
            if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.IsNull())
                ibusPerson.LoadMemberPlusAlternatePayeeAccounts(true);

            iclbMemberPlusAlternateActivePayeeAccounts = new Collection<busPayeeAccount>();
            foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbMemberPlusAlternatePayeeAccounts)
            {
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsNull())
                    lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                if (!lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                {
                    if (lobjPayeeAccount.icdoPayeeAccount.dro_application_id > 0)
                    {
                        if (lobjPayeeAccount.ibusDROApplication.IsNull())
                            lobjPayeeAccount.LoadDROApplication();
                        if (lobjPayeeAccount.ibusMember.IsNull())
                            lobjPayeeAccount.LoadMember();
                    }
                    else
                    {
                        if (lobjPayeeAccount.ibusApplication.IsNull())
                            lobjPayeeAccount.LoadApplication();
                    }
                    if (lobjPayeeAccount.ibusPayee.IsNull())
                        lobjPayeeAccount.LoadPayee();
                    //load plans
                    if (lobjPayeeAccount.ibusPlan.IsNull())
                        lobjPayeeAccount.LoadPlan();
                    iclbMemberPlusAlternateActivePayeeAccounts.Add(lobjPayeeAccount);
                }
            }
        }

        //check does member is having peoplesoft id
        public bool IsPeopleSoftIdExistsForMember
        {
            get
            {
                if (!String.IsNullOrEmpty(ibusPerson.icdoPerson.peoplesoft_id))
                    return true;
                return false;
            }
        }
        //check when Job Class is Non-State Elected Official or State Elected Official 
        //within the first six months of the Employment Detail record Start Date
        public bool iblnIsSFN53405Visible { get; set; }
        public void SetSFN53405Visibility()
        {
            if (iclbEnrolledPlans.IsNull())
                LoadEnrolledAndEligiblePlans();

            foreach (busBenefitPlanWeb lobjBenefitPlan in iclbEnrolledPlans)
            {
                if (lobjBenefitPlan.iintPersonAccountId > 0)
                {
                    if ((lobjBenefitPlan.istrJobClassValue == busConstant.PersonJobClassNonStateElectedOfficial)
                        || (lobjBenefitPlan.istrJobClassValue == busConstant.PersonJobClassStateElectedOfficial))
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, lobjBenefitPlan.idtStartDate, lobjBenefitPlan.idtStartDate.AddMonths(6)))
                        {
                            iblnIsSFN53405Visible = true;
                            break;
                        }
                    }
                }
            }
        }
        //PIR-10022 Start
        public bool IsMemberhavingTwoOpenEmployments()
        {
            DataTable ldtPersonEmplooymentCount = DBFunction.DBSelect("cdoPersonEmployment.CountOfOpenEmployments",
                                                     new object[1] { this.ibusPerson.icdoPerson.person_id }
                                                     , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtPersonEmplooymentCount.Rows.Count > 1)
                return true;
            else
                return false;
        }
        //PIR- 10022 End

        //set visibility of the date of birth
        public bool IsDateOfBirthVisible()
        {
            if (ibusPerson.IsRetiree())
                return true;
            return false;
        }

        public string istrIsDCEligible { get; set; }
        public void LoadIsDCEligible()
        {
            istrIsDCEligible = busConstant.Flag_No;
            if (iclbEnrolledPlans.IsNull())
                LoadEnrolledAndEligiblePlans();

            var lDClist = iclbEnrolledPlans.Where(lobjPAED => (lobjPAED.iintPlanId == busConstant.PlanIdDC ||
                                                lobjPAED.iintPlanId == busConstant.PlanIdDC2020) //PIR 20232 ?code
                && lobjPAED.istrElectionDescription == busConstant.PlanParticipationStatusDescriptionEligible);

            if (lDClist.Count() > 0)
                istrIsDCEligible = busConstant.Flag_Yes;
        }

        //Load Service Purchase Contracts
        public Collection<busServicePurchaseHeader> iclbMSSServicePurchaseInPayment { get; set; }
        public Collection<busServicePurchaseHeader> iclbMSSServicePurchaseApprovedorPaidInFull { get; set; }
        public void LoadServicePurchaseContracts()
        {
            iclbMSSServicePurchaseInPayment = new Collection<busServicePurchaseHeader>();
            iclbMSSServicePurchaseApprovedorPaidInFull = new Collection<busServicePurchaseHeader>();
            if (ibusPerson.iclbServicePurchaseHeader.IsNull()) ibusPerson.LoadServicePurchase(true);

            var lenumServicePurchaseInPayment = ibusPerson.iclbServicePurchaseHeader
                .Where(lobJSP => lobJSP.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment);
            foreach (busServicePurchaseHeader lobjSerPur in lenumServicePurchaseInPayment)
            {
                iclbMSSServicePurchaseInPayment.Add(lobjSerPur);
            }

            var lenumServicePurchase = ibusPerson.iclbServicePurchaseHeader
                .Where(lobJSP => lobJSP.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Paid_In_Full
                || lobJSP.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Closed);
            foreach (busServicePurchaseHeader lobjSerPur in lenumServicePurchase)
            {
                iclbMSSServicePurchaseApprovedorPaidInFull.Add(lobjSerPur);
            }
        }

        //Load Benefit Estimates
        public Collection<busWssBenefitCalculator> iclbMSSBenefitEstimates { get; set; }
        public void LoadBenefitEstimates(int aintPersonID)
        {
            iclbMSSBenefitEstimates = new Collection<busWssBenefitCalculator>();
            DataTable ldtbBenefitEstimate = Select("cdoWssBenefitCalculator.LoadBenefitEstimates", new object[1] { aintPersonID });
            foreach (DataRow ldtrRow in ldtbBenefitEstimate.Rows)
            {
                busWssBenefitCalculator lobjBenefitEstimate = new busWssBenefitCalculator { icdoWssBenefitCalculator = new cdoWssBenefitCalculator() };
                lobjBenefitEstimate.icdoWssBenefitCalculator.LoadData(ldtrRow);
                lobjBenefitEstimate.LoadPlan();
                lobjBenefitEstimate.LoadRetirementBenefitCalculation();
                lobjBenefitEstimate.ibusRetirementBenefitCalculation.LoadMember();
                iclbMSSBenefitEstimates.Add(lobjBenefitEstimate);
            }
        }

        //load insurance retiree person accounts
        private void LoadInsuranceRetireesPlanAccounts()
        {
            ibusPerson.LoadInsuranceAccounts();

            foreach (busPersonAccount lobjPersonAccount in ibusPerson.iclbInsuranceAccounts)
            {
                if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    lobjPersonAccount.LoadPersonAccountEmploymentDetails();

                    if (lobjPersonAccount.iclbAccountEmploymentDetail.Count == 0)
                    {
                        busBenefitPlanWeb lobjBenefitPlan = new busBenefitPlanWeb();
                        lobjBenefitPlan.istrElectionValue = busConstant.PersonAccountElectionValueEnrolled;
                        lobjBenefitPlan.istrElectionDescription = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(340, busConstant.PersonAccountElectionValueEnrolled);
                        lobjBenefitPlan.istrPlanNameUpdate = lobjPersonAccount.ibusPlan.icdoPlan.mss_plan_name;
                        lobjBenefitPlan.iintPlanId = lobjPersonAccount.icdoPersonAccount.plan_id;
                        lobjBenefitPlan.iintPersonAccountId = lobjPersonAccount.icdoPersonAccount.person_account_id;
                        SetInsurancePlanNameEligibilityForRetiree(lobjBenefitPlan, lobjPersonAccount);
                        //PIR 16010
                        if (lobjBenefitPlan.iintPlanId != busConstant.PlanIdMedicarePartD && iblnIsFromMSSActivePlan)
                            iclbEnrolledPlans.Add(lobjBenefitPlan);
                        if (!iblnIsFromMSSActivePlan)
                            iclbEnrolledPlans.Add(lobjBenefitPlan);
                    }
                }
            }
        }

        private void SetInsurancePlanNameEligibilityForRetiree(busBenefitPlanWeb aobjBenefitPlan, busPersonAccount aobjPersonAccount)
        {
            if (aobjBenefitPlan.iintPlanId == busConstant.PlanIdGroupHealth
                || aobjBenefitPlan.iintPlanId == busConstant.PlanIdDental
                || aobjBenefitPlan.iintPlanId == busConstant.PlanIdMedicarePartD
                || aobjBenefitPlan.iintPlanId == busConstant.PlanIdVision)
            {
                aobjBenefitPlan.iblnEligibleGHDVInsuranceEnrollment = true;
                aobjBenefitPlan.istrPlanNameUpdate = aobjPersonAccount.ibusPlan.icdoPlan.mss_plan_name;
            }
            else if (aobjBenefitPlan.iintPlanId == busConstant.PlanIdGroupLife)
            {
                aobjBenefitPlan.iblnEligibleLifeInsuranceEnrollment = true;
                aobjBenefitPlan.istrPlanNameUpdate = aobjPersonAccount.ibusPlan.icdoPlan.mss_plan_name;
            }
            else if (aobjBenefitPlan.iintPlanId == busConstant.PlanIdEAP)
            {
                aobjBenefitPlan.iblnEligibleEAPInsuranceEnrollment = true;
                ibusPerson.LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeInsurance);
                var lenum = ibusPerson.icolPersonAccountByBenefitType.Where(lobjPA => lobjPA.icdoPersonAccount.plan_id == busConstant.PlanIdEAP
                    && lobjPA.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceCancelled);
                if (lenum.Count() > 0)
                {
                    aobjPersonAccount.icdoPersonAccount.person_account_id = lenum.FirstOrDefault().icdoPersonAccount.person_account_id;
                    aobjBenefitPlan.istrPlanNameUpdate = aobjPersonAccount.ibusPlan.icdoPlan.mss_plan_name;
                }
            }
            else if (aobjBenefitPlan.iintPlanId == busConstant.PlanIdLTC)
            {
                aobjBenefitPlan.iblnEligibleLTCInsuranceEnrollment = true;
                aobjBenefitPlan.istrPlanNameUpdate = aobjPersonAccount.ibusPlan.icdoPlan.mss_plan_name;
            }
        }

        private void SetDBDCEligibility(Collection<busPersonAccountEmploymentDetail> aclbPersonAccountEmploymentDetail, busPersonEmployment aobjPersonEmployment, string astrJobClassValue)
        {
            foreach (busPersonAccountEmploymentDetail lobjPersonAccountEmploymentDetail in aclbPersonAccountEmploymentDetail)
            {
                if (lobjPersonAccountEmploymentDetail.ibusPlan.IsNull())
                    lobjPersonAccountEmploymentDetail.LoadPlan();
            }

            if (aobjPersonEmployment.ibusPerson.IsNull()) aobjPersonEmployment.LoadPerson();
            if (aobjPersonEmployment.ibusPerson.icolPersonAccount.IsNull()) aobjPersonEmployment.ibusPerson.LoadPersonAccount();

            bool lblnIsEmployerOfferDB = aclbPersonAccountEmploymentDetail.Any(i => i.ibusPlan.IsDBRetirementPlan() || i.ibusPlan.IsHBRetirementPlan());
            bool lblnIsEmployerOfferDC = aclbPersonAccountEmploymentDetail.Any(i => i.ibusPlan.IsDCRetirementPlan());

            bool lblnIsDBAccountExists = aobjPersonEmployment.ibusPerson.IsDBPersonAccountExists();
            
            bool lblnIsDCAccountExists = false;
            if (aclbPersonAccountEmploymentDetail.Any(i => i.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC))
            {
                lblnIsDCAccountExists = aobjPersonEmployment.ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdDC); //PIR 20232 ?code
            }
            else if (aclbPersonAccountEmploymentDetail.Any(i => i.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC2020))
            {
                lblnIsDCAccountExists = aobjPersonEmployment.ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdDC2020); //PIR 20232 ?code
            }

            busPersonAccountEmploymentDetail lbusDCPAEmpDetail = aclbPersonAccountEmploymentDetail.FirstOrDefault(i => (i.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC ||
                                                                                                                    i.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2020)); //PIR 20232 ?code
            busPersonAccountEmploymentDetail lbusDBPAEmpDetail = aclbPersonAccountEmploymentDetail.FirstOrDefault(i => i.ibusPlan.IsDBRetirementPlan() || i.ibusPlan.IsHBRetirementPlan());

            //Scenario 1
            if ((!lblnIsDBAccountExists) && (!lblnIsDCAccountExists))
            {
                if (lblnIsEmployerOfferDB && lblnIsEmployerOfferDC)
                {
                    //Dont Show DC                    
                    if (lbusDCPAEmpDetail != null)
                        aclbPersonAccountEmploymentDetail.Remove(lbusDCPAEmpDetail);
                }
            }
            else if ((lblnIsDBAccountExists) && (!lblnIsDCAccountExists)) //Scenario 2
            {
                if (lblnIsEmployerOfferDB && lblnIsEmployerOfferDC)
                {
                    if (aobjPersonEmployment.ibusPerson.icolPersonAccount == null)
                        aobjPersonEmployment.ibusPerson.LoadPersonAccount();
                    foreach (busPersonAccount lbusPersonAccount in aobjPersonEmployment.ibusPerson.icolPersonAccount)
                    {
                        if (lbusPersonAccount.ibusPlan == null)
                            lbusPersonAccount.LoadPlan();

                        if ((lbusPersonAccount.ibusPlan.IsDBRetirementPlan() || lbusPersonAccount.ibusPlan.IsHBRetirementPlan()) && (!lbusPersonAccount.IsWithDrawn()))
                        {
                            if (lbusPersonAccount.ibusPersonAccountRetirement == null)
                                lbusPersonAccount.LoadPersonAccountRetirement();
                            //if (lbusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.dc_eligibility_date < DateTime.Now.Date)
                            //{
                            //Dont Show DC                    
                            if (lbusDCPAEmpDetail != null)
                                aclbPersonAccountEmploymentDetail.Remove(lbusDCPAEmpDetail);
                            //}
                        }
                    }
                }
            }
            else if ((!lblnIsDBAccountExists) && lblnIsDCAccountExists) //Scenario 3                    
            {
                if (lblnIsEmployerOfferDC)
                {
                    if (astrJobClassValue == busConstant.JobClassJudge || astrJobClassValue == busConstant.JobClassHighwayPatrolPerson) // PIR 9777
                    {
                        if (lbusDCPAEmpDetail != null)
                            aclbPersonAccountEmploymentDetail.Remove(lbusDCPAEmpDetail);
                    }
                    else
                    {
                        if (lbusDBPAEmpDetail != null)
                            aclbPersonAccountEmploymentDetail.Remove(lbusDBPAEmpDetail);
                    }
                }
            }
        }

        //check is person already enrolled in the DB plan
        //only one DB plan he can enroll at one time
        public void OnlyOneDBEnrolledPlanAllowed(Collection<busPersonAccountEmploymentDetail> aclbPersonAccountEmploymentDetail)
        {
            //Collection<busPersonAccountEmploymentDetail> lclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
            //lclbPersonAccountEmpDtl = aclbPersonAccountEmploymentDetail;

            foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in aclbPersonAccountEmploymentDetail)
            {
                if (lobjPAEmpDtl.ibusPlan == null)
                    lobjPAEmpDtl.LoadPlan();
                if (lobjPAEmpDtl.ibusPersonAccount == null)
                    lobjPAEmpDtl.LoadPersonAccount();
            }

            var lenum = aclbPersonAccountEmploymentDetail.Where(lobj => (lobj.ibusPlan.IsDBRetirementPlan() || lobj.ibusPlan.IsHBRetirementPlan())
                && lobj.icdoPersonAccountEmploymentDetail.person_account_id > 0);

            if (lenum.Count() > 0)
            {
                busPersonAccountEmploymentDetail lobjEnrolledDBPAED = lenum.FirstOrDefault();

                if (lobjEnrolledDBPAED.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                {
                    int lintCount = aclbPersonAccountEmploymentDetail.Count - 1;
                    for (int i = lintCount; i >= 0; i--)
                    {
                        busPersonAccountEmploymentDetail lobjPersonAccountEmpDtl = aclbPersonAccountEmploymentDetail[i];
                        if (lobjPersonAccountEmpDtl.ibusPlan.IsDBRetirementPlan() || lobjPersonAccountEmpDtl.ibusPlan.IsHBRetirementPlan())
                        {
                            if (lobjPersonAccountEmpDtl != lobjEnrolledDBPAED)
                                aclbPersonAccountEmploymentDetail.Remove(lobjPersonAccountEmpDtl);
                        }
                    }
                }
            }
        }

        public bool IsRetireeMember()
        {
            if (ibusPerson.IsNotNull())
            {
                busPayeeAccount ibusPayeeAccount = new busPayeeAccount();
                if (ibusPayeeAccount.IsRetiree(ibusPerson.icdoPerson.person_id, true) ||
                    IsInsurancePlanRetirees()) //IsMemberHaveRetiredOrWithdrawnAccount() PIR-9815
                    return true;
            }
            return false;
        }


        //PIR 12577 - Added below function for Name/Marital Status Change Link Display
        public bool IsRetireeMemberLinkUpdate()
        {
            if (ibusPerson.IsNotNull())
            {
                busPayeeAccount ibusPayeeAccount = new busPayeeAccount();
                if (ibusPayeeAccount.IsRetireeLinkUpdate(ibusPerson.icdoPerson.person_id, true) ||
                    IsInsurancePlanRetirees())
                    return true;
            }
            return false;
        }

        //pir 6887
        public bool IsMemberHaveRetiredOrWithdrawnAccount()
        {
            bool lblnResult = false;
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();

                if (lbusPersonAccount.ibusPlan.IsDBRetirementPlan() || lbusPersonAccount.ibusPlan.IsHBRetirementPlan())
                {
                    if (lbusPersonAccount.IsPlanParticipationStatusRetirementRetired() || lbusPersonAccount.IsWithDrawn())
                    {
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
        }

        //pir 6887
        public bool IsMemberHavePurchase()
        {
            bool lblnResult = false;
            if (ibusPerson.iclbServicePurchaseHeader == null)
                ibusPerson.LoadServicePurchase();

            if (ibusPerson.iclbServicePurchaseHeader.Count > 0)
                lblnResult = true;
            return lblnResult;
        }

        // PROD PIR 8861
        public bool IsInsurancePlanRetirees()
        {
            bool lblnResult = false;
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();

                // Check for GHDV Plans 
                if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental ||
                    lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision ||
                    lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth ||
                    lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    if (lbusPersonAccount.ibusPersonAccountGHDV.IsNull())
                        lbusPersonAccount.LoadPersonAccountGHDV();

                    if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree)
                    {
                        // PIR 11018 Need to add a condition that the in surance plan has to be Enrolled when logging in. 
                        busPersonAccountGhdvHistory lobjGHDVHistory = lbusPersonAccount.ibusPersonAccountGHDV.LoadHistoryByDate(DateTime.Now);
                        if (lobjGHDVHistory.IsNotNull() && lobjGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            lblnResult = true;
                            break;
                        }
                    }
                }

                // Check for Life Insurance Plan
                if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife)
                {
                    if (lbusPersonAccount.ibusPersonAccountLife.IsNull())
                        lbusPersonAccount.LoadPersonAccountLife();
                    if (lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeRetireeMember)
                    {
                        // PIR 11018 Need to add a condition that the in surance plan has to be Enrolled when logging in. 
                        lbusPersonAccount.ibusPersonAccountLife.LoadHistory();
                        if (lbusPersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where(o => busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                                            o.icdoPersonAccountLifeHistory.effective_start_date, o.icdoPersonAccountLifeHistory.effective_end_date) &&
                                            o.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Any())
                        {
                            lblnResult = true;
                            break;
                        }
                    }
                }
            }
            return lblnResult;
        }

        //PIR-13473
        public bool IsEnrolledInCOBRA()
        {
            bool lblnResult = false;
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount();
            //PIR 16933 - Added condition such that if a Retirement plan with Withdrawn/Cancelled status and enrolled in cobra should be redirected to Retiree page.

            bool IsWithdrawnRetrDBPlanExists = false;

            ibusPerson.LoadRetirementAccount();
            if (ibusPerson.iclbRetirementAccount.Count() == 0)
            {
                foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
                {

                    if (lbusPersonAccount.ibusPersonAccountGHDV.IsNull())
                        lbusPersonAccount.LoadPersonAccountGHDV();
                    if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty() && lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == "ENL2")
                    {
                        lblnResult = true;
                        return lblnResult;
                    }
                }
            }

            foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan.IsDBRetirementPlan() || lbusPersonAccount.ibusPlan.IsHBRetirementPlan())
                {
                    if (lbusPersonAccount.IsWithDrawn() || lbusPersonAccount.IsCanceled())
                    {
                        IsWithdrawnRetrDBPlanExists = true;
                    }
                }

            }

            foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
            {
                //if (lbusPersonAccount.ibusPlan == null)
                //    lbusPersonAccount.LoadPlan();
                if (IsWithdrawnRetrDBPlanExists)
                {
                    if (lbusPersonAccount.ibusPersonAccountGHDV.IsNull())
                        lbusPersonAccount.LoadPersonAccountGHDV();
                    if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty() && lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == "ENL2")
                    {
                        lblnResult = true;
                        break;
                    }
                }
            }


            return lblnResult;
        }

        public busPerson GetPerson(string astrLastName, string astrSSN, string astrDOB)
        {
            DateTime ldtTempDate;
            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            bool lblnDateConversionSucceed = DateTime.TryParse(astrDOB, out ldtTempDate);
            DataTable ldtbPerson = Select<cdoPerson>(new string[1] { "DATE_OF_BIRTH" },
                                                                    new object[1] { ldtTempDate },
                                                                    null, null);
            Collection<busPerson> lclbPerson = new Collection<busPerson>();

            if (ldtbPerson.Rows.Count > 0)
            {
                lclbPerson = GetCollection<busPerson>(ldtbPerson, "icdoPerson");
                lbusPerson = lclbPerson.Where(i => i.icdoPerson.LastFourDigitsOfSSN == astrSSN &&
                    i.icdoPerson.last_name.ToUpper().ReplaceWith("[^a-zA-Z0-9]", "") == astrLastName.ToUpper().ReplaceWith("[^a-zA-Z0-9]", "")).FirstOrDefault();
            }

            return lbusPerson;
        }

        public bool IsMASAvailable()
        {
            if (iclbAnnualStatementsForLast3Yrs == null)
                LoadAnnualStatementsForLast3Yrs();
            if (iclbAnnualStatementsForLast3Yrs.Count > 0)
                return true;
            else
                return false;
        }

        //For MSS Layout change
        public Collection<busPersonAccount> iclbBenefitPlanAccounts { get; set; }
        public void LoadActivePlans()
        {
            busOrgPlan lbusOrgPlan = new busOrgPlan();
            ibusPerson.LoadInsuranceAccounts();
            iclbBenefitPlanAccounts = new Collection<busPersonAccount>();
            iblnIsACHDetailExixts = false;
            foreach (busPersonAccount lbusPersonAccount in ibusPerson.iclbInsuranceAccounts)
            {
                if (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    if (lbusOrgPlan.FindOrgPlan(ibusPerson.LoadDefaultOrgPlanIdByPlanId(lbusPersonAccount.icdoPersonAccount.plan_id)))
                        lbusPersonAccount.istrProviderOrgName = busGlobalFunctions.GetOrgNameByOrgID(lbusOrgPlan.icdoOrgPlan.org_id);
                    if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental || lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdHMO
                         || lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                    {
                        lbusPersonAccount.ibusPersonAccountGHDV = new busPersonAccountGhdv() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                        lbusPersonAccount.ibusPersonAccountGHDV.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountGHDV.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountGHDV.LoadPlan();
                        lbusPersonAccount.ibusPersonAccountGHDV.ibusProviderOrgPlan = new busOrgPlan() { icdoOrgPlan = new cdoOrgPlan() };
                        lbusPersonAccount.ibusPersonAccountGHDV.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id = ibusPerson.LoadDefaultOrgPlanIdByPlanId(lbusPersonAccount.icdoPersonAccount.plan_id);
                        if (lbusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate == DateTime.MinValue)
                            lbusPersonAccount.ibusPersonAccountGHDV.LoadPlanEffectiveDate();
                        lbusPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmount(lbusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate);
                        lbusPersonAccount.idecMSSMonthlyPremiumAmount = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
                    }
                    else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                    {
                        lbusPersonAccount.LoadPremiumMonthlyAmount();
                        lbusPersonAccount.idecMSSMonthlyPremiumAmount = lbusPersonAccount.idecMonthlyPremiumAmountByPlan;
                    }
                    //PIR 15870
                    else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        lbusPersonAccount.ibusPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                        lbusPersonAccount.ibusPersonAccountMedicarePartDHistory.FindMedicareByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountMedicarePartDHistory.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountMedicarePartDHistory.LoadPlan();
                        lbusPersonAccount.ibusPersonAccountMedicarePartDHistory.GetMonthlyPremiumAmountForMedicarePartD();
                        lbusPersonAccount.idecMSSMonthlyPremiumAmount = lbusPersonAccount.ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount;
                    }
                    else
                    {
                        lbusPersonAccount.ibusPersonAccountLife = new busPersonAccountLife() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountLife = new cdoPersonAccountLife() };
                        lbusPersonAccount.ibusPersonAccountLife.FindPersonAccountLife(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountLife.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountLife.LoadPlan();
                        lbusPersonAccount.ibusPersonAccountLife.LoadLifeOption();
                        if (lbusPersonAccount.ibusPersonAccountLife.idtPlanEffectiveDate == DateTime.MinValue)
                            lbusPersonAccount.ibusPersonAccountLife.LoadPlanEffectiveDate();
                        lbusPersonAccount.ibusPersonAccountLife.GetMonthlyPremiumAmount(lbusPersonAccount.ibusPersonAccountLife.idtPlanEffectiveDate, ibusPerson.LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdGroupLife));
                        lbusPersonAccount.idecMSSMonthlyPremiumAmount = lbusPersonAccount.ibusPersonAccountLife.idecTotalMonthlyPremium;
                    }

                    //PIR 24989 - Display the latest bank information so the member can confirm the info was updated as per their request. 
                    if(lbusPersonAccount.iclbPersonAccountAchDetail == null)
                            lbusPersonAccount.LoadPersonAccountAchDetail();
                    busPersonAccountAchDetail lbusPersonAccountAchDetail = lbusPersonAccount.iclbPersonAccountAchDetail.Count > 0 ? lbusPersonAccount.iclbPersonAccountAchDetail.Where(i => i.icdoPersonAccountAchDetail.ach_end_date == DateTime.MinValue || i.icdoPersonAccountAchDetail.ach_end_date >= busGlobalFunctions.GetSysManagementBatchDate()).OrderByDescending(o => o.icdoPersonAccountAchDetail.person_account_ach_detail_id).FirstOrDefault(): null;
                    if(lbusPersonAccountAchDetail.IsNotNull())
                    {
                        iblnIsACHDetailExixts = true;
                        lbusPersonAccount.istrBankName = busGlobalFunctions.GetOrgNameByOrgID(lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.bank_org_id);
                        lbusPersonAccount.istrAccountType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1706, lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.bank_account_type_value);
                        lbusPersonAccount.istrAccountNumber = lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.scrambled_account_number;
                        lbusPersonAccount.idtACHStartDate = lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.ach_start_date;
                    }

                    // PIR 21636
                    if (lbusPersonAccount.ibusPlan.IsNull())
                        lbusPersonAccount.LoadPlan();
                    if ((!lbusPersonAccount.ibusPlan.IsGHDVPlan() && lbusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdEAP && lbusPersonAccount.ibusPlan.icdoPlan.plan_id != busConstant.PlanIdGroupLife) ||
                        (((lbusPersonAccount.ibusPlan.IsGHDVPlan()) || (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife))
                        && (lbusPersonAccount.IsRetireePlanEnrolledOrIsCobra())))
                            iclbBenefitPlanAccounts.Add(lbusPersonAccount);
                }
            }
        }

        //pir 8345
        public Collection<busPersonAccount> iclbMSSRetirementPersonAccount { get; set; }
        public bool iblnIsRetirementAccountAvailable { get; set; }
        public void LoadActiveRetirementPlan()
        {
            iclbMSSRetirementPersonAccount = new Collection<busPersonAccount>();
            ibusPerson.LoadRetirementAccount();
            foreach (busPersonAccount lobjPersonAccount in ibusPerson.iclbRetirementAccount)
            {
                if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled
                    || lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                {
                    lobjPersonAccount.LoadAllPersonEmploymentDetails();
                    if (lobjPersonAccount.iclbEmploymentDetail.Count > 0)
                    {
                        lobjPersonAccount.icdoPersonAccount.EmployerName =
                            lobjPersonAccount.iclbEmploymentDetail[0].ibusPersonEmployment.ibusOrganization.icdoOrganization.org_name;
                    }
                    iclbMSSRetirementPersonAccount.Add(lobjPersonAccount);

                }
            }
            if (iclbMSSRetirementPersonAccount.Count() > 0)
                iblnIsRetirementAccountAvailable = true;
        }

        public int iintCurrentEmploymentDetailID { get; set; }
        public int iintEmploymentDetailIDFromEligiblePlanList { get; set; }

        public busBenefitPlanWeb ibusRetirementPlan { get; set; }

        //public List<Tuple<string, DateTime>> ilstActivationCodes = new List<Tuple<string, DateTime>>();
        public Collection<busWssOtpActivation> iclbWssOtpActivation { get; set; }
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            foreach (utlError lobjError in iarrErrors)
                lobjError.istrErrorID = string.Empty;
            if(this.iarrErrors.Count == 0)
            {
                if (iblnExternalLogin)
                {
                    try
                    {
                        lcdoPerson = busGlobalFunctions.DeepCopy<cdoPerson>(ibusPerson.icdoPerson);  //PIR-20708 MSS ‘Certify’ logic/email validation – need to exclude PRE_EMAIL_ADDRESS	
                        lcdoPerson.Select();

                        if (ibusPerson.icdoPerson.ienuObjectState != ObjectState.Insert)
                        {
                            string lstrEmailFrom = NeoSpin.Common.ApplicationSettings.Instance.WSSRetrieveMailFrom;
                            string lstrActivationCodeEmailSub = NeoSpin.Common.ApplicationSettings.Instance.WSSActivationCodeEmailSubject;
                            string lstrActivationCodeEmailMsg = NeoSpin.Common.ApplicationSettings.Instance.WSSActivationCodeEmailMsg;
                            string lstrEmailChangeEmailSubject = NeoSpin.Common.ApplicationSettings.Instance.WSSEmailChangeEmailSubject;
                            string lstrEmailChangeEmailMsg = NeoSpin.Common.ApplicationSettings.Instance.WSSEmailChangeEmailMsg;
                            string lstrEmailChangeEmailMsgSignature = NeoSpin.Common.ApplicationSettings.Instance.WSSMailBodySignature;
                            string lstrMemberName = ibusPerson.icdoPerson.first_name + " " + ibusPerson.icdoPerson.last_name;
                            string lstrEmailMessage = string.Empty;
                            string lstrChangeEmailMessage = string.Empty;
                            ibusPerson.icdoPerson.email_waiver_flag = string.IsNullOrEmpty(ibusPerson.icdoPerson.email_waiver_flag) ? "N" : ibusPerson.icdoPerson.email_waiver_flag;
                            //FIrst Time User Logins And Email Address is exists.
                            if (ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_No && ibusPerson.icdoPerson.email_waiver_date == DateTime.MinValue && ibusPerson.icdoPerson.pre_email_address.IsNullOrEmpty()
                                && istrEmailAddressPre.IsNotNullOrEmpty() && (istrEmailAddressPre == ibusPerson.icdoPerson.email_address))
                            {
                                ibusPerson.icdoPerson.email_waiver_date = DateTime.Now;
                                ibusPerson.icdoPerson.certify_date = DateTime.Now;
                                ibusPerson.icdoPerson.activation_code_date = DateTime.Now;
                                ibusPerson.icdoPerson.activation_code = busGlobalFunctions.GenerateAnOTP();
                                ibusPerson.icdoPerson.activation_code_flag = busConstant.Flag_No;
                                lstrEmailMessage = string.Format(lstrActivationCodeEmailMsg + lstrEmailChangeEmailMsgSignature, lstrMemberName, Convert.ToString(ibusPerson.icdoPerson.activation_code));
                                busGlobalFunctions.SendMailRetryOnFail(lstrEmailFrom, ibusPerson.icdoPerson.email_address, lstrActivationCodeEmailSub, lstrEmailMessage, true, true);
                            }
                            //First Time User Logins And Email Addres is does not exists. New email address entering first time
                            else if (ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_No && ibusPerson.icdoPerson.pre_email_address.IsNullOrEmpty() && istrEmailAddressPre.IsNullOrEmpty() && ibusPerson.icdoPerson.email_address.IsNotNullOrEmpty())
                            {
                                ibusPerson.icdoPerson.certify_date = DateTime.Now;
                                ibusPerson.icdoPerson.activation_code_date = DateTime.Now;
                                ibusPerson.icdoPerson.activation_code = busGlobalFunctions.GenerateAnOTP();
                                ibusPerson.icdoPerson.activation_code_flag = busConstant.Flag_No;
                                lstrEmailMessage = string.Format(lstrActivationCodeEmailMsg + lstrEmailChangeEmailMsgSignature, lstrMemberName, Convert.ToString(ibusPerson.icdoPerson.activation_code));
                                busGlobalFunctions.SendMailRetryOnFail(lstrEmailFrom, ibusPerson.icdoPerson.email_address, lstrActivationCodeEmailSub, lstrEmailMessage, true, true);
                            }
                            //After login, if user is Updating an email address.
                            else if (ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_No && ibusPerson.icdoPerson.email_address.IsNotNullOrEmpty() && iblnEmailChanged)
                            {
                                ibusPerson.icdoPerson.certify_date = DateTime.Now;
                                ibusPerson.icdoPerson.activation_code = busGlobalFunctions.GenerateAnOTP();
                                ibusPerson.icdoPerson.activation_code_date = DateTime.Now;
                                ibusPerson.icdoPerson.activation_code_flag = busConstant.Flag_No;
                                lstrChangeEmailMessage = string.Format(lstrEmailChangeEmailMsg + lstrEmailChangeEmailMsgSignature, lstrMemberName);
                                lstrEmailMessage = string.Format(lstrActivationCodeEmailMsg + lstrEmailChangeEmailMsgSignature, lstrMemberName, Convert.ToString(ibusPerson.icdoPerson.activation_code));
                                busGlobalFunctions.SendMailRetryOnFail(lstrEmailFrom, ibusPerson.icdoPerson.pre_email_address, lstrEmailChangeEmailSubject, lstrChangeEmailMessage, true, true);
                                busGlobalFunctions.SendMailRetryOnFail(lstrEmailFrom, ibusPerson.icdoPerson.email_address, lstrActivationCodeEmailSub, lstrEmailMessage, true, true);
                            }
                            else if (ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_No && IsPersonCertify() && !iblnEmailChanged)
                            {
                                ibusPerson.icdoPerson.email_waiver_date = DateTime.Now;
                                ibusPerson.icdoPerson.certify_date = DateTime.Now;
                                ibusPerson.icdoPerson.activation_code_date = DateTime.Now;
                                ibusPerson.icdoPerson.activation_code = busGlobalFunctions.GenerateAnOTP();
                                ibusPerson.icdoPerson.activation_code_flag = busConstant.Flag_No;
                                lstrEmailMessage = string.Format(lstrActivationCodeEmailMsg + lstrEmailChangeEmailMsgSignature, lstrMemberName, Convert.ToString(ibusPerson.icdoPerson.activation_code));
                                busGlobalFunctions.SendMailRetryOnFail(lstrEmailFrom, ibusPerson.icdoPerson.email_address, lstrActivationCodeEmailSub, lstrEmailMessage, true, true);
                            }
                            AddActivationCodeInTable(ibusPerson.icdoPerson.activation_code, ibusPerson.icdoPerson.activation_code_date);
                            ibusPerson.icdoPerson.certify_date = DateTime.Now;
                            if (ibusPerson.icdoPerson.ienuObjectState != ObjectState.Update)
                                ibusPerson.icdoPerson.ienuObjectState = ObjectState.Update;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.Publish(ex);
                        utlError lutlError = new utlError();
                        lutlError.istrErrorMessage = NeoSpin.Common.ApplicationSettings.Instance.MSSEmailServerNotReachableMsg;
                        this.iarrErrors.Add(lutlError);
                    }
                }
            }
        }

        #region PIR 9966 MSS - Annual Enrollment

        // PIR 9966 MSS - Annual Enrollment
        public bool IsAnnualEnrollment
        {
            get
            {
                DateTime ldteStartDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));
                DateTime ldteEndDate = Convert.ToDateTime(busGlobalFunctions.GetData2ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));
                if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, ldteStartDate, ldteEndDate))
                    return true;
                return false;
            }
        }

        public string AnnualEnrollmentHomeText
        {
            get
            {
                DateTime ldteStartDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));
                DateTime ldteEndDate = Convert.ToDateTime(busGlobalFunctions.GetData2ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));
                return "Annual Enrollment season begins <b>" + ldteStartDate.ToString(busConstant.DateFormatLongDate) + "</b> and ends <b>" + ldteEndDate.ToString(busConstant.DateFormatLongDate + "</b>");
            }
        }

        public string istrAnnualEnrollmentTempHomeText
        {
            get
            {
                DateTime ldteACAAnnualStartDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.ACACertAnnualEnrollmentWindow, iobjPassInfo));
                DateTime ldteACAAnnualEndDate = Convert.ToDateTime(busGlobalFunctions.GetData2ByCodeValue(52, busConstant.ACACertAnnualEnrollmentWindow, iobjPassInfo));
                return "Annual Enrollment season For Temporary Employees begins <b>" + ldteACAAnnualStartDate.ToString(busConstant.DateFormatLongDate) + "</b> and ends <b>" + ldteACAAnnualEndDate.ToString(busConstant.DateFormatLongDate + "</b>");
            }
        }

        public bool IsAnnualEnrollmentForTemporary
        {
            get
            {
                DateTime ldteACAAnnualStartDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.ACACertAnnualEnrollmentWindow, iobjPassInfo));
                DateTime ldteACAAnnualEndDate = Convert.ToDateTime(busGlobalFunctions.GetData2ByCodeValue(52, busConstant.ACACertAnnualEnrollmentWindow, iobjPassInfo));
                if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now.Date, ldteACAAnnualStartDate.Date, ldteACAAnnualEndDate.Date))
                    return true;
                return false;
            }
        }
        public DateTime AnnualEnrollmentEffectiveDate
        {
            get
            {
                return Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));
            }
        }

        public bool IsAnnualEnrollmentPlan(int aintPlanID)
        {
            if (aintPlanID == busConstant.PlanIdGroupHealth || aintPlanID == busConstant.PlanIdDental || aintPlanID == busConstant.PlanIdVision ||
               aintPlanID == busConstant.PlanIdFlex || aintPlanID == busConstant.PlanIdGroupLife)
                return true;
            return false;
        }

        public Collection<busBenefitPlanWeb> iclbAnnualEnrollmentPlans { get; set; }

        public void LoadAnnualEnrollmentPlans()
        {
            string lstrStatus = string.Empty;
            if (iclbEligiblePlans.IsNull() || iclbEnrolledPlans.IsNull())
                LoadEnrolledAndEligiblePlans();

            iintPersonID = ibusPerson.icdoPerson.person_id;//PIR 19424

            iclbAnnualEnrollmentPlans = new Collection<busBenefitPlanWeb>();
            foreach (busBenefitPlanWeb lobjBenefitPlan in iclbEnrolledPlans)
            {
                if (IsAnnualEnrollmentPlan(lobjBenefitPlan.iintPlanId))
                {
                    lobjBenefitPlan.iintEnrollmentRequestId = GetCurrentANNERequestID(lobjBenefitPlan.iintPlanId, ref lstrStatus);
                    if (lobjBenefitPlan.iintEnrollmentRequestId == 0 || lstrStatus == busConstant.EnrollRequestStatusFinishLater)
                        lobjBenefitPlan.istrEnrollmentRequestStatus = busConstant.AnnualEnrollmentRequestStatus;
                    else
                    {
                        if (string.IsNullOrEmpty(lobjBenefitPlan.istrEnrollmentRequestStatus))
                            lobjBenefitPlan.istrEnrollmentRequestStatus = "Enrolled";
                    }
                    if ((lobjBenefitPlan.iintPlanId == busConstant.PlanIdLTC || lobjBenefitPlan.iintPlanId == busConstant.PlanIdFlex) //PIR 12532
                         && lobjBenefitPlan.istrElectionValue == busConstant.PlanOptionStatusValueWaived)
                        lobjBenefitPlan.istrEnrollmentRequestStatus = lobjBenefitPlan.istrElectionDescription;

                    //PIR 15544 - Added Validation if a member is enrolled as a Retiree in a plan, plan should not be added to Annual Enrollment.

                   // if (!IsInsurancePlanRetirees())
                        iclbAnnualEnrollmentPlans.Add(lobjBenefitPlan);
                }
				//PIR 19108
                if (lobjBenefitPlan.iintPlanId == busConstant.PlanIdDeferredCompensation)
                {
                    iintPersonAccountDeferredComp = lobjBenefitPlan.iintPersonAccountId;
                    iintPersonEmploymentDetailid = lobjBenefitPlan.iintPersonEmploymentDetailid;
                    iintPlanID = busConstant.PlanIdDeferredCompensation;
                }
            }
            foreach (busBenefitPlanWeb lobjBenefitPlan in iclbEligiblePlans)
            {
                if (IsAnnualEnrollmentPlan(lobjBenefitPlan.iintPlanId))
                {
                    lobjBenefitPlan.iintEnrollmentRequestId = GetCurrentANNERequestID(lobjBenefitPlan.iintPlanId, ref lstrStatus);
                    if (lobjBenefitPlan.iintEnrollmentRequestId == 0 || lstrStatus == busConstant.EnrollRequestStatusFinishLater)
                    {
                        lobjBenefitPlan.istrEnrollmentRequestStatus = busConstant.AnnualEnrollmentRequestStatus;
                    }
                    else
                    {
                        if (iclbEnrolledPlans.Where(o => o.iintPlanId == lobjBenefitPlan.iintPlanId).Count() > 0) //PIR-10379
                            continue;
                        if (string.IsNullOrEmpty(lobjBenefitPlan.istrEnrollmentRequestStatus))
                            lobjBenefitPlan.istrEnrollmentRequestStatus = "Enrolled";
                    }
                    if (lobjBenefitPlan.iintPlanId == busConstant.PlanIdLTC && lobjBenefitPlan.istrElectionValue == busConstant.PlanOptionStatusValueWaived)
                        lobjBenefitPlan.istrEnrollmentRequestStatus = lobjBenefitPlan.istrElectionDescription;
                    if (lobjBenefitPlan.iintPlanId == busConstant.PlanIdFlex && lobjBenefitPlan.istrElectionValue == busConstant.PlanOptionStatusValueWaived)
                    {
                        lobjBenefitPlan.istrEnrollmentRequestStatus = "Waived";
                        lobjBenefitPlan.istrElectionDescription = "Waived";
                    }
                    iclbAnnualEnrollmentPlans.Add(lobjBenefitPlan);
                }
				//PIR 19108
                if (lobjBenefitPlan.iintPlanId == busConstant.PlanIdDeferredCompensation)
                {
                    iintPersonAccountDeferredComp = lobjBenefitPlan.iintPersonAccountId;
                    iintPersonEmploymentDetailid = lobjBenefitPlan.iintPersonEmploymentDetailid;
                    iintPlanID = busConstant.PlanIdDeferredCompensation;
                }
            }
            //PIR 21636  
            iclbAnnualEnrollmentPlans.ForEach(BP => {
                if(BP.iintPlanId == busConstant.PlanIdGroupHealth && BP.iintPersonEmploymentDetailid > 0)
                {
                    busPersonEmploymentDetail lbusPersonEmploymentDetail = new busPersonEmploymentDetail() { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail()};
                    if(lbusPersonEmploymentDetail.FindPersonEmploymentDetail(BP.iintPersonEmploymentDetailid) && lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    {
                        ibusPerson.LoadACAEligibilityCertification(BP.iintPersonEmploymentDetailid); 
                        if(ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.wss_employment_aca_cert_id > 0 || 
						   lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary) // PIR 25321
                        {
                            BP.iblnDontShowInANNEGrid = true;
                        }
                    }                    
                }
                if ((IsAnnualEnrollmentPlan(BP.iintPlanId)) && BP.iintPlanId != busConstant.PlanIdFlex)
                    BP.istrProviderName = GetProvidersName(BP);
                int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(BP.iintPlanId, ibusPerson.icdoPerson.person_id);
                if (lintPersonAccountId > 0)
                {
                    BP.LoadPersonAccount(lintPersonAccountId);
                    if (BP.ibusPersonAccount.IsRetireePlanEnrolledOrIsCobra())
                        BP.iblnIsBenefitPlanCobraorRetiree = true;
                    else
                        BP.iblnIsBenefitPlanCobraorRetiree = false;
                }
                else
                    BP.iblnIsBenefitPlanCobraorRetiree = false;
            });

            iclbAnnualEnrollmentPlans = iclbAnnualEnrollmentPlans.Where(BP => !BP.iblnIsBenefitPlanCobraorRetiree && !BP.iblnDontShowInANNEGrid).OrderBy(o => o.iintPlanId).ToList().ToCollection();//PIR 10379

        }

        private int GetCurrentANNERequestID(int aintPlanID, ref string astrStatus)
        {
            if (ibusPerson.IsNotNull())
            {
                DataTable ldtbRequest;
                if (aintPlanID == busConstant.PlanIdFlex)
                {
                    ldtbRequest = Select<cdoWssPersonAccountEnrollmentRequest>(new string[4] { "PERSON_ID", "PLAN_ID", "DATE_OF_CHANGE", "STATUS_VALUE" },
                                                    new object[4] { ibusPerson.icdoPerson.person_id, aintPlanID, AnnualEnrollmentEffectiveDate, busConstant.EnrollRequestStatusFinishLater }, null, null);
                }
                else
                {
                    ldtbRequest = Select("cdoWssPersonAccountEnrollmentRequest.GetRequestIDForAnnualEnrollment", new object[3] { ibusPerson.icdoPerson.person_id, aintPlanID, AnnualEnrollmentEffectiveDate });
                }
                if (ldtbRequest.Rows.Count > 0)
                {
                    cdoWssPersonAccountEnrollmentRequest lcdoRequest = new cdoWssPersonAccountEnrollmentRequest();
                    lcdoRequest.LoadData(ldtbRequest.Rows[0]);
                    if (lcdoRequest.reason_value == busConstant.AnnualEnrollment ||
                        (lcdoRequest.reason_value == "CNLD" && lcdoRequest.is_changes_in_anne_flag == busConstant.Flag_Yes))
                    {
                        astrStatus = lcdoRequest.status_value;
                        return lcdoRequest.wss_person_account_enrollment_request_id;
                    }
                }
            }
            return 0;
        }


        #endregion
        public void AddActivationCodeInTable(string astrActivationCode,DateTime adtActivationCodeDate)
        {
            //Store Generated OTP into table 
            busWssOtpActivation lobjWssOTPActivation = new busWssOtpActivation { icdoWssOtpActivation = new cdoWssOtpActivation() };
            lobjWssOTPActivation.InsertActivationCodeInTable(ibusPerson.icdoPerson.person_id, busConstant.MSSMemberAuthenticationOTPSource, astrActivationCode, adtActivationCodeDate);
        }
        public void LoadWssOtpActivation()
        {
            busWssOtpActivation lobjWssOTPActivation = new busWssOtpActivation { icdoWssOtpActivation = new cdoWssOtpActivation() };
            iclbWssOtpActivation = lobjWssOTPActivation.LoadWssOtpActivation(ibusPerson.icdoPerson.person_id, busConstant.MSSMemberAuthenticationOTPSource);
        }
        public bool IsPeoplesoftEnabled()
        {
            string lstrResult = (string)DBFunction.DBExecuteScalar("cdoCodeValue.IsPeoplesoftEnabled", new object[] { },
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (ibusPerson.ibusCurrentEmployment == null)
                ibusPerson.LoadCurrentEmployer();
            if (ibusPerson.ibusCurrentEmployment.ibusOrganization == null)
                ibusPerson.ibusCurrentEmployment.LoadOrganization();
            if (ibusPerson.ibusCurrentEmployment.icolPersonEmploymentDetail == null)
                ibusPerson.ibusCurrentEmployment.LoadPersonEmploymentDetail();
            if (lstrResult == null || lstrResult.ToUpper() == "N")
                return false;
            else if (lstrResult.ToUpper() == "Y" && (ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueNonPSParoll
                //|| ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueHigherEd    //PIR 24075
                || ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == null))
                return false;
            else if (lstrResult.ToUpper() == "Y" && (ibusPerson.ibusCurrentEmployment.icolPersonEmploymentDetail != null && ibusPerson.ibusCurrentEmployment.icolPersonEmploymentDetail.Count > 0 &&
                ibusPerson.ibusCurrentEmployment.icolPersonEmploymentDetail[0].icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary))
                return false;
            else
                return true;
        }
        //PIR - 12652 Employer Name Should be shown only for those plans, which are showing up twice in MSS eligible plans grid
        public bool CanEmployerNameBeShown()
        {
            if (iclbEligiblePlansUI.IsNull())
                LoadEnrolledAndEligiblePlans();
            if (iclbEligiblePlansUI.IsNotNull() && iclbEligiblePlansUI.Count() > 1)
            {
                var temp = (from obj in iclbEligiblePlansUI
                            group obj by obj.iintPlanId into g
                            select new
                            {
                                planid = g.Key,
                                Count = g.Count()
                            });
                List<int> temp1 = (from obj in temp
                                   where obj.Count > 1
                                   select obj.planid).ToList();
                if (temp1 != null && temp1.Count > 0)
                {
                    Collection<busBenefitPlanWeb> lclbusBenefitPlanWeb = (from obj in iclbEligiblePlansUI
                                                                          where temp1.Contains(obj.iintPlanId)
                                                                          select obj).ToList().ToCollection<busBenefitPlanWeb>();

                    foreach (busBenefitPlanWeb lobjBenefitPlanWeb in iclbEligiblePlansUI)
                    {
                        if (!lclbusBenefitPlanWeb.Contains(lobjBenefitPlanWeb))
                            lobjBenefitPlanWeb.istrOrganizationName = string.Empty;
                    }
                    return true;
                }
            }
            return false;
        }

        //PIR 12533
        public string istrPlanYear
        {
            get
            {
                //PIR 13551 - Changed the year on Annual Enrollment landing page according to Annual Enrollment date.
                return Convert.ToString(Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo)).Year);
            }
        }
        //PIR 15126
        public void LoadCurrentANNERequestStatus()
        {
            foreach (busBenefitPlanWeb lobjAnnualEnrollmentPlan in iclbAnnualEnrollmentPlans)
            {
                DataTable ldtbRequest;
                ldtbRequest = Select("cdoWssPersonAccountEnrollmentRequest.GetEnrollmentRequestStatus", new object[3] { ibusPerson.icdoPerson.person_id, lobjAnnualEnrollmentPlan.iintPlanId, AnnualEnrollmentEffectiveDate });
                if (ldtbRequest.Rows.Count == 1)
                {
                    cdoWssPersonAccountEnrollmentRequest lcdoRequest = new cdoWssPersonAccountEnrollmentRequest();
                    lcdoRequest.LoadData(ldtbRequest.Rows[0]);
                    if (lcdoRequest.status_value == "PSTD" || lcdoRequest.status_value == "PROC")
                        lobjAnnualEnrollmentPlan.istrEnrollmentRequestStatus = busConstant.EnrollmentStatusPosted;
                    if (lcdoRequest.status_value == "PEND")
                        lobjAnnualEnrollmentPlan.istrEnrollmentRequestStatus = busConstant.EnrollmentStatusUnderReview;
                    if (lcdoRequest.status_value == "RJTD")
                        lobjAnnualEnrollmentPlan.istrEnrollmentRequestStatus = busConstant.EnrollmentStatusRejected;
                    //PIR 25341 When request is Ignored, donot show Ignored and keep it blank in the Request Status column
                    if (lcdoRequest.status_value == "FLTR" || lcdoRequest.status_value == "IGNR")
                        lobjAnnualEnrollmentPlan.istrEnrollmentRequestStatus = "";
                }
                else
                {
                    lobjAnnualEnrollmentPlan.istrEnrollmentRequestStatus = "";
                }
            }
        }
        public bool IsRetireePlanEnrolledOrIsCobra()
        {
            bool lblnResult = false;
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();

                // Check for GHDV Plans 
                if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental ||
                    lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision ||
                    lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (lbusPersonAccount.ibusPersonAccountGHDV.IsNull())
                        lbusPersonAccount.LoadPersonAccountGHDV();

                    if ((lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree) ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
                    {
                        busPersonAccountGhdvHistory lobjGHDVHistory = lbusPersonAccount.ibusPersonAccountGHDV.LoadHistoryByDate(DateTime.Now);
                        if (lobjGHDVHistory.IsNotNull() && lobjGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            lblnResult = true;
                            break;
                        }
                    }
                }
                //Medicare
                if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    lblnResult = true;
                    break;
                }

                // Check for Life Insurance Plan
                if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife)
                {
                    if (lbusPersonAccount.ibusPersonAccountLife.IsNull())
                        lbusPersonAccount.LoadPersonAccountLife();
                    if (lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeRetireeMember)
                    {
                        lbusPersonAccount.ibusPersonAccountLife.LoadHistory();
                        if (lbusPersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where(o => busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                                            o.icdoPersonAccountLifeHistory.effective_start_date, o.icdoPersonAccountLifeHistory.effective_end_date) &&
                                            o.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Any())
                        {
                            lblnResult = true;
                            break;
                        }
                    }
                }
            }
            return lblnResult;
        }

        #region PIR-18492      
        public bool IsExternalUser()
        {
            if(iblnExternalLogin)
            {
                return true;
            }
            return false;
        }

        public bool EmailWaiverFlagOrEMail()
        {
            if(iblnExternalLogin)
            {
                if (ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_Yes && ibusPerson.icdoPerson.email_address.IsNotNullOrEmpty())
                    return true;
            }
            return false;
        }

        public string istrEmailAddress { get; set; }
        public string istrActivationCode { get; set; }
        public bool iblnCertifyedPerson = false;
		public bool iblnIsCertifyDateNull { get; set; }
        public string istrPopUpMessage { get; set; }
        public string istrIsEmailWaiverFlagSelected { get; set; }
       
        //same email
        public bool IsSameEMail()
        {
            if (iblnExternalLogin)
            {
                DataTable ldtbPerson = Select<cdoPerson>(new string[1] { "EMAIL_ADDRESS" }, new object[1] { ibusPerson.icdoPerson.email_address }, null, null);
                if (ldtbPerson.IsNotNull())
                {
                    if ((ldtbPerson.Rows.Count > 0) && (ibusPerson.icdoPerson.person_id != Convert.ToInt32(ldtbPerson.Rows[0]["Person_id"])))
                        return true;
                }
                //PIR-20708 MSS ‘Certify’ logic/email validation – need to exclude PRE_EMAIL_ADDRESS	
                if (iblnEmailChanged && ibusPerson.icdoPerson.pre_email_address.IsNullOrEmpty() && istrEmailAddressPre.IsNotNullOrEmpty() && (istrEmailAddressPre == ibusPerson.icdoPerson.email_address))
                    return true;
                else
                    return false;
            }
            return false;
        }

        //visibilty rule for certify button
        public bool IsPersonCertify()
        {
            bool lblnResult = false;
            if (iblnExternalLogin)
            {
                int lintMonths = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(7010, "MNTH", iobjPassInfo));
                
                if (ibusPerson.icdoPerson.certify_date != DateTime.MinValue && ((ibusPerson.icdoPerson.certify_date).AddMonths(lintMonths) >= DateTime.Today))
                    lblnResult = false;
                //Certify button visible for first time and after 9 months updating demographic info
               else if (ibusPerson.icdoPerson.certify_date == DateTime.MinValue ||
                    (ibusPerson.icdoPerson.certify_date != DateTime.MinValue && ((ibusPerson.icdoPerson.certify_date).AddMonths(lintMonths) < DateTime.Today)))
                {
                    lblnResult = true;
                }
                else
                    lblnResult = true;
            }
            return lblnResult;
        }

        //i.	Address without End Date
        public bool IsPersonAddressIsValid() =>
         (iblnExternalLogin && ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.IsNotNull() && ((ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.end_date == DateTime.MinValue) ||
                 (busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                                                             ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.start_date,
                                                             ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.end_date))));
        //iii.	Email address (also need to add a second field on screen to �Verify Email�; error if two email addresses don�t match)
        public bool IsEmailEnteredMatches()
        {
            bool lblnResult = true;
            if (iblnExternalLogin)
            {
                if (ibusPerson.icdoPerson.email_waiver_flag.IsNullOrEmpty() || ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_No)
                {
                    //If person updating email for first time.(does not exists email)
                    if (istrEmailAddressPre.IsNullOrEmpty() && ibusPerson.icdoPerson.email_address.IsNotNullOrEmpty() && istrEmailAddress.IsNullOrEmpty())
                        lblnResult = false;
                    //If person updating email for first time, not entered Re-entered feild
                    else if (istrEmailAddressPre.IsNullOrEmpty() && ibusPerson.icdoPerson.email_address.IsNotNullOrEmpty() && istrEmailAddress.IsNotNullOrEmpty() && (istrEmailAddress != ibusPerson.icdoPerson.email_address))
                        lblnResult = false;
                    //If Email Address and Reenter Email Address are not Match //PIR-20708 MSS ‘Certify’ logic/email validation – need to exclude PRE_EMAIL_ADDRESS
                    else if (ibusPerson.icdoPerson.email_address.IsNotNullOrEmpty() && istrEmailAddress.IsNotNullOrEmpty() && istrEmailAddress != ibusPerson.icdoPerson.email_address)
                        lblnResult = false;
                    // updating email address, but not entered in Re-Entering
                    else if (istrEmailAddressPre.IsNotNullOrEmpty() &&  ibusPerson.icdoPerson.email_address.IsNotNullOrEmpty() && istrEmailAddress.IsNullOrEmpty() && istrEmailAddressPre != ibusPerson.icdoPerson.email_address )
                        lblnResult = false;
                    //Email address is exists but not updating.  
                    //else if (ibusPerson.icdoPerson.email_address.IsNotNullOrEmpty() && istrEmailAddress.IsNotNullOrEmpty() && istrEmailAddress != ibusPerson.icdoPerson.email_address)
                    //     lblnResult = false;  
                    else if (IsPersonCertify() && ibusPerson.icdoPerson.email_address.IsNotNullOrEmpty() && (istrEmailAddress != ibusPerson.icdoPerson.email_address))   //PIR-20708
                    {
                        lblnResult = false;
                    }
                }
                else
                    lblnResult = true;                            
            }
            return lblnResult;
        }
              

        //48hrs code email_waiver_date - getdate() < 48 hrs
        public bool IsEmailUpdationWithin48hrs()
        {
            bool lbnREsult = false;
            if (iblnExternalLogin)
            {
                if (iblnEmailChanged)
                {
                    TimeSpan diff = DateTime.Now - idtEmilWaiver;
                    double hours = diff.TotalHours;
                    if (hours < 48)
                        lbnREsult = true;
                }               
            }
            return lbnREsult;
        }
              

        public bool iblnEmailChanged = false;
        public DateTime idtEmilWaiver = DateTime.MinValue;
        public string istrEmailAddressPre { get; set; }
        
        public bool iblnIsActivationCodeNotVerified
        {
            get
            {
                return (iblnExternalLogin &&
                    !string.IsNullOrEmpty(ibusPerson.icdoPerson.activation_code) &&
                    (ibusPerson.icdoPerson.activation_code_date != DateTime.MinValue) &&
                    (String.IsNullOrEmpty(ibusPerson.icdoPerson.activation_code_flag) || (ibusPerson.icdoPerson.activation_code_flag == "N"))
                    ) ? true : false;
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (iblnExternalLogin)
            {
                //LoadPerson();
                if (ibusPerson.icdoPerson.ienuObjectState != ObjectState.Insert)
                {
                    //c.	If user checks �Waiver� and an email address exists store email address in �Previous Email Address� field (New field in SGT_PERSON)
                    if (ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_Yes && ibusPerson.icdoPerson.email_address.IsNullOrEmpty())
                    {
                        ibusPerson.icdoPerson.pre_email_address = istrEmailAddressPre;
                        ibusPerson.icdoPerson.email_address = string.Empty;
                        ibusPerson.icdoPerson.email_waiver_date = DateTime.Today;
                    }
                    if (ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_No)
                    {
                        //If email does not exists, entering new email Address
                        if (ibusPerson.icdoPerson.pre_email_address == null && istrEmailAddressPre.IsNullOrEmpty() && ibusPerson.icdoPerson.email_address.IsNotNullOrEmpty())
                        {                            
                            ibusPerson.icdoPerson.email_waiver_date = DateTime.Now;                            
                        }
                        if ((istrEmailAddressPre != ibusPerson.icdoPerson.email_address))  //PIR-20708 MSS ‘Certify’ logic/email validation – need to exclude PRE_EMAIL_ADDRESS
                        {
                            ibusPerson.icdoPerson.pre_email_address = istrEmailAddressPre;
                            ibusPerson.icdoPerson.email_waiver_date = DateTime.Now;
                            iblnEmailChanged = true;                            
                        }                     
                    }    
                }
            }
            base.BeforeValidate(aenmPageMode);
        }
        public bool IsActivationCodeValid(string astrActivationCode)
        {
            ibusPerson.icdoPerson.activation_code_flag = busConstant.Flag_No;
            if (ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_No && astrActivationCode.IsNotNullOrEmpty() && ibusPerson.icdoPerson.activation_code.IsNotNullOrEmpty())
            {
                if (iclbWssOtpActivation.Any(objWssOthActivation=> objWssOthActivation.icdoWssOtpActivation.activation_code == astrActivationCode))
                {
                    iblnCertifyedPerson = true;
                    ibusPerson.icdoPerson.activation_code_flag = busConstant.Flag_Yes;
                }
                if (!iclbWssOtpActivation.Any(objWssOthActivation => objWssOthActivation.icdoWssOtpActivation.activation_code == astrActivationCode))
                {
                    iblnCertifyedPerson = false;
                    ibusPerson.icdoPerson.activation_code_flag = busConstant.Flag_No;
                }
            }
            else if (ibusPerson.icdoPerson.email_waiver_flag == busConstant.Flag_No && astrActivationCode.IsNullOrEmpty() && ibusPerson.icdoPerson.activation_code.IsNotNullOrEmpty())
            {
                iblnCertifyedPerson = false;
                ibusPerson.icdoPerson.activation_code_flag = busConstant.Flag_No;
            }            
            return iblnCertifyedPerson;
        }
        //30 min code
		//To check if Activation code is valid for 30 minutes starting from the time it was generated.
        public bool IsActivationCodeValidFor30min(string astrActivationCode)
        {
            bool lbnResult = false;
            if (ibusPerson.icdoPerson.activation_code_date != DateTime.MinValue)
            {
                busWssOtpActivation lbusWssOtpActivation = iclbWssOtpActivation.FirstOrDefault(objWssOthActivation => objWssOthActivation.icdoWssOtpActivation.activation_code == astrActivationCode);

                if (lbusWssOtpActivation.IsNotNull() && lbusWssOtpActivation.icdoWssOtpActivation.activation_code_date != DateTime.MinValue)
                {
                    TimeSpan diff = DateTime.Now - lbusWssOtpActivation.icdoWssOtpActivation.activation_code_date;
                    double minutes = diff.TotalMinutes;
                    if (minutes <= 30)
                        lbnResult = true;
                }
            }
            return lbnResult;
        }
       public ArrayList btnVertify_Click(string astrActivationCode) 
        {
            ArrayList larrList = new ArrayList();
            LoadWssOtpActivation();
            if (!IsActivationCodeValidFor30min(astrActivationCode))
            {
                utlError lobjError = new utlError();
                lobjError = AddError(10328, "");
                larrList.Add(lobjError);
                return larrList;  
            } 
            if (!IsActivationCodeValid(astrActivationCode))
            {
                utlError lobjError = new utlError();
                lobjError = AddError(10318, "");
                larrList.Add(lobjError);
                return larrList;                
            }
           ibusPerson.icdoPerson.Update();
            if(ibusPerson.icdoPerson.certify_date != DateTime.MinValue)
            {
                iobjPassInfo.idictParams["PersonCertify"] = true;
            }
           larrList.Add(this);
           return larrList;
        }		
		
		//
	   public ArrayList btnReSendOtp()
       {
           ArrayList larrList = new ArrayList();
           string lstrEmailFrom = Convert.ToString(NeoSpin.Common.ApplicationSettings.Instance.WSSRetrieveMailFrom);
           string lstrActivationCodeEmailSub = Convert.ToString(NeoSpin.Common.ApplicationSettings.Instance.WSSActivationCodeEmailSubject);
           string lstrActivationCodeEmailMsg = Convert.ToString(NeoSpin.Common.ApplicationSettings.Instance.WSSActivationCodeEmailMsg);
           string lstrEmailChangeEmailMsgSignature = Convert.ToString(NeoSpin.Common.ApplicationSettings.Instance.WSSMailBodySignature);
           string lstrMemberName = ibusPerson.icdoPerson.first_name + " " + ibusPerson.icdoPerson.last_name;
           ibusPerson.icdoPerson.activation_code = busGlobalFunctions.GenerateAnOTP();
           ibusPerson.icdoPerson.activation_code_date = DateTime.Now;
           ibusPerson.icdoPerson.activation_code_flag = busConstant.Flag_No;
           string lstrEmailMessage = string.Format(lstrActivationCodeEmailMsg + lstrEmailChangeEmailMsgSignature, lstrMemberName, Convert.ToString(ibusPerson.icdoPerson.activation_code));
           ibusPerson.icdoPerson.Update();
            try
            {
                busGlobalFunctions.SendMailRetryOnFail(lstrEmailFrom, ibusPerson.icdoPerson.email_address, lstrActivationCodeEmailSub, lstrEmailMessage, true, true);
                //ilstActivationCodes.Add(new Tuple<string, DateTime>(ibusPerson.icdoPerson.activation_code, ibusPerson.icdoPerson.activation_code_date));
                AddActivationCodeInTable(ibusPerson.icdoPerson.activation_code, ibusPerson.icdoPerson.activation_code_date);
                larrList.Add(this);
            }
            catch(Exception ex)
            {
                ExceptionManager.Publish(ex);
                utlError lutlError = new utlError();
                lutlError.istrErrorMessage = NeoSpin.Common.ApplicationSettings.Instance.MSSEmailServerNotReachableMsg;
                larrList.Add(lutlError);
            }
           return larrList;
       }

        //PIR-20708 MSS ‘Certify’ logic/email validation – need to exclude PRE_EMAIL_ADDRESS	
        public ArrayList btnCancelOtp()
        {
            ArrayList larrList = new ArrayList();
            ibusPerson.icdoPerson.email_address = lcdoPerson.email_address;
            istrEmailAddress = lcdoPerson.email_address;
            ibusPerson.icdoPerson.pre_email_address = lcdoPerson.pre_email_address;
            ibusPerson.icdoPerson.certify_date = lcdoPerson.certify_date;
            ibusPerson.icdoPerson.email_waiver_flag = lcdoPerson.email_waiver_flag;
            ibusPerson.icdoPerson.activation_code_flag = lcdoPerson.activation_code_flag;
            ibusPerson.icdoPerson.Update();
            larrList.Add(this);
            return larrList;
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            iblnIsCertifyDateNull = IsPersonCertify();
        }
        #endregion PIR-18492

        public override int PersistChanges()
        {
            ibusPerson?.UpdateMedicarePartDFlags();
            return base.PersistChanges();  
        }

        //public bool IsRetireePlanEnrolledOrIsCobra()
        //{
        //    bool lblnResult = false;
        //    if (ibusPerson.icolPersonAccount == null)
        //        ibusPerson.LoadPersonAccount();

        //    foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
        //    {
        //        if (lbusPersonAccount.ibusPlan == null)
        //            lbusPersonAccount.LoadPlan();

        //        // Check for GHDV Plans 
        //        if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental ||
        //            lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision ||
        //            lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth)
        //        {
        //            if (lbusPersonAccount.ibusPersonAccountGHDV.IsNull())
        //                lbusPersonAccount.LoadPersonAccountGHDV();

        //            if ((lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree ||
        //                lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree ||
        //                lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree) ||
        //                lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
        //            {
        //                busPersonAccountGhdvHistory lobjGHDVHistory = lbusPersonAccount.ibusPersonAccountGHDV.LoadHistoryByDate(DateTime.Now);
        //                if (lobjGHDVHistory.IsNotNull() && lobjGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
        //                {
        //                    lblnResult = true;
        //                    break;
        //                }
        //            }
        //        }
        //        //Medicare
        //        if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMedicarePartD)
        //        {
        //            lblnResult = true;
        //            break;
        //        }

        //        // Check for Life Insurance Plan
        //        if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife)
        //        {
        //            if (lbusPersonAccount.ibusPersonAccountLife.IsNull())
        //                lbusPersonAccount.LoadPersonAccountLife();
        //            if (lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeRetireeMember)
        //            {
        //                lbusPersonAccount.ibusPersonAccountLife.LoadHistory();
        //                if (lbusPersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where(o => busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
        //                                    o.icdoPersonAccountLifeHistory.effective_start_date, o.icdoPersonAccountLifeHistory.effective_end_date) &&
        //                                    o.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Any())
        //                {
        //                    lblnResult = true;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return lblnResult;
        //}

	//PIR 19108
        public bool IsDeferredCompEnrolled()
        {
            if (iintPlanID == busConstant.PlanIdDeferredCompensation)
            {
                if (iintPersonAccountDeferredComp > 0)
                    return true;
            }
            return false;
        }

        public bool IsDefinedContributionEnrolled() //PIR 19433 - Annual Enrollment - Hide Bullet 1 
        {
            if (iclbEnrolledPlans != null)
            {
                foreach (busBenefitPlanWeb lobjBenefitPlan in iclbEnrolledPlans)
                {
                    if (lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC ||
                        lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2020 || //PIR 20232
                        lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025) //PIR 25920
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsHPOrJudgesOrDCEnrolled() //PIR 19433 - Annual Enrollment - Hide Bullet 1 & 2
        {
            if (iclbEnrolledPlans != null)
            {
                foreach (busBenefitPlanWeb lobjBenefitPlan in iclbEnrolledPlans)
                {
                    if (lobjBenefitPlan.iintPlanId == busConstant.PlanIdHP ||
                        lobjBenefitPlan.iintPlanId == busConstant.PlanIdJudges ||
                        lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC ||
                        lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2020) //PIR 20232
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsMemberHasHDHPPlan()
        {
            if (ibusPerson.icolPersonAccount == null || ibusPerson.icolPersonAccount.Any(pa => pa.ibusPlan.IsNull()))
                ibusPerson.LoadPersonAccount(true);
            busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.FirstOrDefault(peracc => peracc.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth);
            if (lbusPersonAccount.IsNotNull())
            {
                lbusPersonAccount.icdoPersonAccount.person_employment_dtl_id = iintHDHPPersonEmploymentDetailid;
                lbusPersonAccount.LoadPersonEmploymentDetail();
                lbusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lbusPersonAccount.LoadOrgPlan(DateTime.Now.GetFirstDayofNextMonth());
                if (lbusPersonAccount.ibusPersonAccountGHDV.IsNull())
                    lbusPersonAccount.LoadPersonAccountGHDV();

                busPersonAccountGhdvHistory lobjGHDVHistory = lbusPersonAccount?.ibusPersonAccountGHDV?.LoadHistoryByDate(DateTime.Now);

                if (lbusPersonAccount.ibusOrgPlan.icdoOrgPlan.hsa_pre_tax_agreement == "Y")
                {
                    if (lobjGHDVHistory?.icdoPersonAccountGhdvHistory?.alternate_structure_code_value == "HDHP" || lbusPersonAccount?.ibusPersonAccountGHDV?.icdoPersonAccountGhdv?.alternate_structure_code_value == "HDHP")
                        return true;
                }
            }
            return false;
        }

        //FW Upgrade :: wfmDefault.aspx.cs file code conversion for method btn_OpenPDF
        public string istrDownloadFileName{ get; set; }
        public string istrRegion_value { get; set; }

        public void PreDownload(int aintPlanId)
        {           
            //PIR PIR 26955 - Learn More Link Goes to the Wrong Document
            string lstrIsAnnual = (iobjPassInfo.istrFormName == "wfmAnnualEnrollmentBenefitPlansMaintenance") ? busConstant.Flag_Yes : busConstant.Flag_No;
            DataTable ldtMSSHelpFiles = busNeoSpinBase.Select("cdoCodeValue.LoadMSSHelpFiles",
                                new object[2] { lstrIsAnnual, aintPlanId });

            if (ldtMSSHelpFiles.Rows.Count > 0)
            {
                this.istrDownloadFileName = Convert.ToString(ldtMSSHelpFiles.Rows[0]["DATA1"]) + Convert.ToString(ldtMSSHelpFiles.Rows[0]["DATA2"]);
            }
        }
        //PIR PIR 26955
        public bool IsLearnMoreLinkVisible(int aintPlanId)
        {
            string lstrIsAnnual = (iobjPassInfo.istrFormName == "wfmAnnualEnrollmentBenefitPlansMaintenance") ? busConstant.Flag_Yes : busConstant.Flag_No;
            DataTable ldtMSSHelpFiles = busNeoSpinBase.Select("cdoCodeValue.LoadMSSHelpFiles",
                                new object[2] {  lstrIsAnnual ,aintPlanId});

            if (ldtMSSHelpFiles.Rows.Count > 0)
            {
                if (Convert.ToString(ldtMSSHelpFiles.Rows[0]["DATA2"]).IsNullOrEmpty())
                {
                    return false;
                }
            }           
            return true;
        }
        //PIR PIR 26955
        public bool IsVideoLinkVisible(int aintPlanId)
        {
            DataTable ldtMSSHelpFiles = busNeoSpinBase.Select("cdoCodeValue.LoadMSSVideoPath",
                                new object[1] {aintPlanId });

            if (ldtMSSHelpFiles.Rows.Count > 0)
            {
                if (Convert.ToString(ldtMSSHelpFiles.Rows[0]["DATA2"]).IsNullOrEmpty())
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsDBPlan(int aintPlanID)
        {
            busPlan lobjPlan = new busPlan { icdoPlan = new cdoPlan() };
            lobjPlan.FindPlan(aintPlanID);
            if (lobjPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB)   //PIR 25920  New DC plan
                return true;
            return false;
        }

        public ArrayList View1099RReport(int aint1099rID)
        {
            ArrayList larlstResult = new ArrayList();
            try
            {
                string lstrForm1099RName = string.Empty;
                busNeoSpinBase lobjNeospinBase = new busNeoSpinBase();
                busPayment1099r lobj1099R = new busPayment1099r();
                DataSet ldt = new DataSet();
                ldt = lobj1099R.GetDataSetToCreateReport(aint1099rID);
                //PIR-16715 created report file with respective year.
                if (ldt.Tables[0].Rows.Count > 0)
                    lstrForm1099RName = ldt.Tables[0].Rows[0]["RUN_YEAR"].ToString();
                larlstResult.Add("rptForm1099R_" + lstrForm1099RName + ".pdf");
                larlstResult.Add(lobjNeospinBase.CreateDynamicReport("rptForm1099R_" + lstrForm1099RName + ".rpt", ldt, string.Empty));
                larlstResult.Add("application/pdf");
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                utlError lutlError = AddError(0, ex.Message);
                larlstResult.Add(lutlError);
            }
            return larlstResult;
        }

        public ArrayList CreateMemberAnnualStatementDownload(int aintSelectionID, string astrReportName)
        {
            ArrayList larlstResult = new ArrayList();
            try
            {
                busMASSelection lobjSelection = new busMASSelection { icdoMasSelection = new cdoMasSelection() };
                lobjSelection.FindMASSelection(aintSelectionID);

                if (lobjSelection.ibusBatchRequest == null)
                    lobjSelection.LoadBatchRequest();
                larlstResult.Add(astrReportName + ".pdf");
                if (lobjSelection.ibusBatchRequest.icdoMasBatchRequest.group_type_value == busConstant.GroupTypeNonRetired)
                {
                    
                    larlstResult.Add(lobjSelection.CreateMASReport(astrReportName, busConstant.ReportMASPath));
                }
                else
                {
                    larlstResult.Add(lobjSelection.CreateAnnualRetireeStatement(astrReportName, busConstant.ReportMASPath));
                }
                larlstResult.Add("application/pdf");
            }
            catch(Exception ex)
            {
                ExceptionManager.Publish(ex);
                utlError lutlError = AddError(0, ex.Message);
                larlstResult.Add(lutlError);
            }
            return larlstResult;
        }
        //PIR 22914 New Hire Text Visibility
        public bool IsEmploymentDetailStartDateWithinMonth() =>
                (iclbPersonEmploymentDetail?.Count > 0) ?
                busGlobalFunctions.CheckDateOverlapping(iclbPersonEmploymentDetail[0].icdoPersonEmploymentDetail.start_date, DateTime.Now.Date.AddDays(-31), DateTime.Now.Date) : false;

        //PIR-23093 Provider_Name for Regular Plan and Annual Plan Enrollment	
        public string GetProvidersName(busBenefitPlanWeb lobjBenPlan, bool lblnIsNDPERSPlan = false)
        {
            if (lblnIsNDPERSPlan)
            {
                if (iclbEnrolledPlans.Any(i=>i.iintPlanId==lobjBenPlan.iintPlanId))
                {
                    int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(lobjBenPlan.iintPlanId, ibusPerson.icdoPerson.person_id);
                    
                    if (lintPersonAccountId > 0)
                    {
                        busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount=new cdoPersonAccount()};
                        if (lbusPersonAccount.FindPersonAccount(lintPersonAccountId))
                        {
                            int lintProviderOrgId = lbusPersonAccount.icdoPersonAccount.provider_org_id;
                            if (lintProviderOrgId >0)
                            {
                                busOrganization lbusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                                if (lbusOrganization.FindOrganization(lintProviderOrgId))
                                {
                                    return lbusOrganization.icdoOrganization.org_name;
                                }
                            }
                        }                    
                    }
                }
                else
                {
                    DataTable ldtblProviderName = Select("entMSSHome.GetActiveProviderByPlan", new object[2] { lobjBenPlan.iintPlanId, DateTime.Now });
                    if (ldtblProviderName.Rows.Count > 0 && ldtblProviderName.Rows[0]["ORG_NAME"] != DBNull.Value)
                        return Convert.ToString(ldtblProviderName.Rows[0]["ORG_NAME"]);
                }
            }
            else
            {
                string lstrAnnualEnrollmentEff = busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo);
                DataTable ldtblProviderName1 = Select("entMSSHome.GetActiveProviderByPlan", new object[2] { lobjBenPlan.iintPlanId, lstrAnnualEnrollmentEff });
                if (ldtblProviderName1.Rows.Count > 0 && ldtblProviderName1.Rows[0]["ORG_NAME"] != DBNull.Value)
                    return Convert.ToString(ldtblProviderName1.Rows[0]["ORG_NAME"]);
            }
            return string.Empty;
        }
        public bool IsTempAnnualEnrollmentForHealthLinkVisiblity()
        {
            if (ibusPerson.icdoPerson.person_id > 0)
            {
                DataTable ldtACAEligibilityCertification = Select("entMSSHome.IsACACertifyForTemporaryHealthMember", new object[2] { ibusPerson.icdoPerson.person_id, iintCurrentEmploymentDetailID });
                if (ldtACAEligibilityCertification.Rows.Count > 0)
                    return true;
            }
            return false;
        }
        public bool IsTempHealthIsHavingPENDRequest() => iintHealthEnrollmentRequestId > 0;
        //PIR 25920 DC 2025 change
        public bool lblnIsVisibleADECAmountLink
        {
            get
            {
                if (iclbEligiblePlansUI.IsNull() || iclbEnrolledPlansUI.IsNull()) LoadEnrolledAndEligiblePlans();
                if (iintEmploymentDetailIDFromEligiblePlanList == 0) iintEmploymentDetailIDFromEligiblePlanList = iintCurrentEmploymentDetailID;
                busMSSPersonAccountRetirementWeb lbusMSSPersonAccountRetirementWeb = new busMSSPersonAccountRetirementWeb();
                int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdDC2025, ibusPerson.icdoPerson.person_id);
                if (!iclbEligiblePlansUI.Any(lobjBenefitPlan => lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025) &&
                    !iclbEnrolledPlansUI.Any(lobjBenefitPlan => lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025))
                    return false;
                if (lintPersonAccountId > 0)
                {
                    busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    if (lbusPersonAccount.FindPersonAccount(lintPersonAccountId))
                    {
                        lbusMSSPersonAccountRetirementWeb.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                        lbusMSSPersonAccountRetirementWeb.ibusPersonAccount = lbusPersonAccount;
                        lbusMSSPersonAccountRetirementWeb.ibusPersonAccount.LoadPersonAccountRetirement();
                        lbusMSSPersonAccountRetirementWeb.icdoPersonAccountRetirement = lbusMSSPersonAccountRetirementWeb.ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement;
                        lbusMSSPersonAccountRetirementWeb.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = iintEmploymentDetailIDFromEligiblePlanList;
                        lbusMSSPersonAccountRetirementWeb.ibusPersonAccount.LoadPersonEmploymentDetail();
                        if (lbusMSSPersonAccountRetirementWeb.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing)
                            return lbusMSSPersonAccountRetirementWeb.lblnIsVisibleADECAmountLink;
                    }
                }
                else
                {
                    lbusMSSPersonAccountRetirementWeb.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount()};
                    lbusMSSPersonAccountRetirementWeb.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = iintEmploymentDetailIDFromEligiblePlanList;
                    lbusMSSPersonAccountRetirementWeb.icdoPersonAccount = new cdoPersonAccount();
                    lbusMSSPersonAccountRetirementWeb.icdoPersonAccount.person_employment_dtl_id = iintEmploymentDetailIDFromEligiblePlanList;
                    lbusMSSPersonAccountRetirementWeb.icdoPersonAccount.plan_id = busConstant.PlanIdDC2025;
                    return lbusMSSPersonAccountRetirementWeb.lblnIsVisibleADECAmountLink;
                }
                if (iclbEligiblePlans.IsNotNull())
                {
                    if (iclbEligiblePlans != null)
                    {
                        foreach (busBenefitPlanWeb lobjBenefitPlan in iclbEligiblePlans)
                        {
                            if (lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }
        //PIR 25920 DC 2025 change
        public void GetNoOfDaysRemainingBeforeElection()
        {
            
            int lintPersonAccountId = busPersonAccountHelper.GetPersonAccountID(busConstant.PlanIdDC2025, ibusPerson.icdoPerson.person_id);
            busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            if (iclbEligiblePlansUI.IsNull()) LoadEnrolledAndEligiblePlans();
            if (lintPersonAccountId > 0)
            {
                if (lbusPersonAccount.FindPersonAccount(lintPersonAccountId))
                {
                    iintDC25PersonAccountID = lbusPersonAccount.icdoPersonAccount.person_account_id;
                    
                    if (iclbEnrolledPlansUI.Any(lobjBenefitPlan => lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025))
                        iintEmploymentDetailIDFromEligiblePlanList = Convert.ToInt32(iclbEnrolledPlansUI.Where(objEligiblePlansUI => objEligiblePlansUI.iintPlanId == busConstant.PlanIdDC2025).Select(objEnrolledPlansUI => objEnrolledPlansUI.iintPersonEmploymentDetailid).First());
                    else if (iclbEligiblePlansUI.Any(lobjBenefitPlan => lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025))
                        iintEmploymentDetailIDFromEligiblePlanList = Convert.ToInt32(iclbEligiblePlansUI.Where(objEligiblePlansUI => objEligiblePlansUI.iintPlanId == busConstant.PlanIdDC2025).Select(objEligiblePlansUI => objEligiblePlansUI.iintPersonEmploymentDetailid).First());
                    if (iintEmploymentDetailIDFromEligiblePlanList == 0) iintEmploymentDetailIDFromEligiblePlanList = iintCurrentEmploymentDetailID;

                    lbusPersonAccount.icdoPersonAccount.person_employment_dtl_id = iintEmploymentDetailIDFromEligiblePlanList;
                    if (lbusPersonAccount.ibusPersonEmploymentDetail.IsNull())
                        lbusPersonAccount.LoadPersonEmploymentDetail();
                    iintNoOfDaysRemainingBeforeElection = GetNumberOfDaysBasedOnType(lbusPersonAccount.ibusPersonEmploymentDetail, lbusPersonAccount);
                    if (iclbEligiblePlansUI.Any(lobjBenefitPlan => lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025))
                    {
                        if (!iclbEnrolledPlansUI.Any(lobjBenefitPlan => lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025))
                            iintDC25PersonAccountID = 0;
                    }
                }
            }
            else
            {
                //if (iclbEligiblePlansUI.Any(lobjBenefitPlan => lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025))
                //    iintEmploymentDetailIDFromEligiblePlanList = Convert.ToInt32(iclbEligiblePlansUI.Where(objEligiblePlansUI => objEligiblePlansUI.iintPlanId == busConstant.PlanIdDC2025).Select(objEligiblePlansUI => objEligiblePlansUI.iintPersonEmploymentDetailid).First());
                //if (iintEmploymentDetailIDFromEligiblePlanList == 0) iintEmploymentDetailIDFromEligiblePlanList = iintCurrentEmploymentDetailID;
                GetPersonEmploymentDetailIDByDate();
                busPersonEmploymentDetail lbusPersonEmploymentDetail = new busPersonEmploymentDetail() { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lbusPersonEmploymentDetail.FindPersonEmploymentDetail(iintEmploymentDetailIDFromEligiblePlanList);
                iintNoOfDaysRemainingBeforeElection = GetNumberOfDaysBasedOnType(lbusPersonEmploymentDetail, lbusPersonAccount);
            }
            if (iintNoOfDaysRemainingBeforeElection <= 0) iintNoOfDaysRemainingBeforeElection = 1;
        }

        public int GetNumberOfDaysBasedOnType(busPersonEmploymentDetail abusPersonEmploymentDetail, busPersonAccount abusPersonAccount)
        {

            //busPersonEmploymentDetail lbusValidateDateRangePersonEmploymentDetail = new busPersonEmploymentDetail();
            if (abusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) abusPersonEmploymentDetail.LoadPersonEmployment();
            //lbusValidateDateRangePersonEmploymentDetail = abusPersonEmploymentDetail.LoadEarliestPermEmploymentDetail(abusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id, aintPlanID);
            DateTime ldtAddlEEContributionEndDate = abusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(30);
            DateTime ldtAddlEEContributionPercentEndDate = abusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ?
                        abusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp_end_date : abusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent_end_date;
            if (ldtAddlEEContributionPercentEndDate == DateTime.MinValue)
            {
                if (abusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    ldtAddlEEContributionEndDate = abusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(180);
                else if (abusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    ldtAddlEEContributionEndDate = abusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(30);
            }
            else if (ldtAddlEEContributionPercentEndDate != DateTime.MinValue)
                ldtAddlEEContributionEndDate = ldtAddlEEContributionPercentEndDate;
            if (abusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
            {
                iblnIsTemporaryMember = true;
                iintNoOfDaysRemainingBeforeElection = busGlobalFunctions.DateDiffInDays(busGlobalFunctions.GetSysManagementBatchDate(), ldtAddlEEContributionEndDate);
            }
            else if (abusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
            {
                iblnIsTemporaryMember = false;
                iintNoOfDaysRemainingBeforeElection = busGlobalFunctions.DateDiffInDays(busGlobalFunctions.GetSysManagementBatchDate(), ldtAddlEEContributionEndDate);
            }
            return iintNoOfDaysRemainingBeforeElection <= 0 ? 1 : iintNoOfDaysRemainingBeforeElection;
        }

        public void GetPersonEmploymentDetailIDByDate()
        {
            if (iclbEligiblePlansUI.Any(lobjBenefitPlan => lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025))
            {
                if (iclbEligiblePlansUI.Count(lobjBenefitPlan => lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025) > 1)
                {
                    busPersonEmploymentDetail lbusPersonEmploymentDetailGetDate = new busPersonEmploymentDetail() { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                    //int lintEarliestDays = 0;
                    iclbEligiblePlansUI.ForEach(lobjEligiblePlanUI =>
                    {
                        lbusPersonEmploymentDetailGetDate.FindPersonEmploymentDetail(lobjEligiblePlanUI.iintPersonEmploymentDetailid);
                        if (lbusPersonEmploymentDetailGetDate.IsNotNull() && busGlobalFunctions.CheckDateOverlapping(busGlobalFunctions.GetSysManagementBatchDate(),
                        lbusPersonEmploymentDetailGetDate.icdoPersonEmploymentDetail.start_date, lbusPersonEmploymentDetailGetDate.icdoPersonEmploymentDetail.end_date_no_null
                        ))
                        {
                            //lintEarliestDays = busGlobalFunctions.DateDiffInDays(busGlobalFunctions.GetSysManagementBatchDate(), lbusPersonEmploymentDetailGetDate.icdoPersonEmploymentDetail.start_date);
                            iintEmploymentDetailIDFromEligiblePlanList = lbusPersonEmploymentDetailGetDate.icdoPersonEmploymentDetail.person_employment_dtl_id;
                        }
                    }
                    );
                }
                else if (iclbEligiblePlansUI.Any(lobjBenefitPlan => lobjBenefitPlan.iintPlanId == busConstant.PlanIdDC2025))
                    iintEmploymentDetailIDFromEligiblePlanList = Convert.ToInt32(iclbEligiblePlansUI.Where(objEligiblePlansUI => objEligiblePlansUI.iintPlanId == busConstant.PlanIdDC2025).Select(objEligiblePlansUI => objEligiblePlansUI.iintPersonEmploymentDetailid).First());
            }
            if (iintEmploymentDetailIDFromEligiblePlanList == 0) iintEmploymentDetailIDFromEligiblePlanList = iintCurrentEmploymentDetailID;

        }
    }
}
