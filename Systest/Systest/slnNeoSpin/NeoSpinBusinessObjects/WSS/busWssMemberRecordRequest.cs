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
using NeoSpin.DataObjects;
using System.Collections.Generic;
using Sagitec.ExceptionPub;
using Sagitec.Bpm;
using NeoBase.BPM;
#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busWssMemberRecordRequest:
    /// Inherited from busWssMemberRecordRequestGen, the class is used to customize the business object busWssMemberRecordRequestGen.
    /// </summary>
    [Serializable]
    public class busWssMemberRecordRequest : busWssMemberRecordRequestGen
    {
        //public busWssPersonAddress ibusWSSPersonAddress { get; set; }

        //public busWssPersonContact ibusWSSPersonContact { get; set; }

        //public busWssPersonEmployment ibusWSSPersonEmployment { get; set; }

        //For PIR 7952
        public busContact ibusContact { get; set; }
        public int iintContact_id { get; set; }

        public busOrganization ibusContactOrg { get; set; }
        //PIR-11030 Start
        public Collection<busWssMemberRecordRequest> iclbPendingMemberRecordRequest { get; set; }
        public bool iblnIsFromPS { get; set; }
        //PIR-11030 End

        // PIR 11883
        //Update entity - Venkat - no reference found in entity
        public Collection<busSolBpmRequest> iclbWorkflowRequest { get; set; }
        public Collection<busSolBpmCaseInstance> iclbProcessInstance { get; set; }

        public string istrSuppressUSPSValidation { get; set; }

        public string istrRejectedByName { get; set; } //PIR 23900

        public ArrayList AutoEnrollPersonInMandatoryPlans(bool ablnIsFromPostClick = false, bool ablnIsFromBatch = true)
        {
            ArrayList larrMandatoryPlanEnrollmentResult = new ArrayList();
            iblnBPMInitiate = false;
            if (icdoWssPersonEmploymentDetail.person_employment_dtl_id > 0)
            {
                DataTable ldtbPersonAccountEmploymentDetail = busBase.Select<cdoPersonAccountEmploymentDetail>(new string[1] { enmPersonAccountEmploymentDetail.person_employment_dtl_id.ToString() }, new object[1] { icdoWssPersonEmploymentDetail.person_employment_dtl_id }, null, null);
                busBase lbusBase = new busBase();
                Collection<busPersonAccountEmploymentDetail> lclbPersonAccountEmploymentDetail = lbusBase.GetCollection<busPersonAccountEmploymentDetail>(ldtbPersonAccountEmploymentDetail, "icdoPersonAccountEmploymentDetail");
                if (lclbPersonAccountEmploymentDetail.Count > 0)
                {
                    foreach (busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail in lclbPersonAccountEmploymentDetail)
                    {
                        lbusPersonAccountEmploymentDetail.LoadPlan();
                    }
                    ibusPerson.LoadPersonAccount();
                    #region Start of Retirement Plan Enrollment logic
                    if (icdoWssPersonEmploymentDetail.employment_status_value == busConstant.EmploymentStatusContributing)
                    {
                        
                        int lintCountOfRetPlansMemberIsEligibleToEnroll = lclbPersonAccountEmploymentDetail.Where(pa => pa.ibusPlan.IsRetirementPlan() && !pa.ibusPlan.IsDCRetirementPlan()).Count();
                        if (lintCountOfRetPlansMemberIsEligibleToEnroll > 1)
                        {
                            iblnBPMInitiate = true;
                        }
                        else if (lintCountOfRetPlansMemberIsEligibleToEnroll == 1)
                        {
                            //PIR 25920 New Plan DC 2025 added Plan DC25 
                            if (ibusPerson.icolPersonAccount.Any(pa =>

                                                                ((pa.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC ||
                                                                pa.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC2020 || pa.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC2025) && //PIR 25920
                                                                (pa.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                                                                pa.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)) ||
                                                                ((pa.ibusPlan.IsRetirementPlan()) && (pa.icdoPersonAccount.plan_participation_status_value ==
                                                                busConstant.PlanParticipationStatusRetirementEnrolled)))
                                                                )
                            {
                                iblnBPMInitiate = true;
                            }
                            else
                            {
                                //Enroll in retirement plan
                                busPersonAccountEmploymentDetail lbusRetPlanMemberIsEligibleToEnroll = lclbPersonAccountEmploymentDetail
                                                                                                            .FirstOrDefault(pa => pa.ibusPlan.IsRetirementPlan() &&
                                                                                                            !pa.ibusPlan.IsDCRetirementPlan());
                                busPersonAccount lbusExistingPersonAccountForEligibleRetPlan = ibusPerson.LoadEnrOrSuspOrCancAccountByPlan(lbusRetPlanMemberIsEligibleToEnroll.icdoPersonAccountEmploymentDetail.plan_id);
                                if (lbusRetPlanMemberIsEligibleToEnroll.IsNotNull())
                                {
                                    if (lbusRetPlanMemberIsEligibleToEnroll.icdoPersonAccountEmploymentDetail.election_value != busConstant.PersonAccountElectionValueEnrolled)
                                    {
                                        lbusRetPlanMemberIsEligibleToEnroll.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                                        if (lbusExistingPersonAccountForEligibleRetPlan.icdoPersonAccount.person_account_id > 0)
                                            lbusRetPlanMemberIsEligibleToEnroll.icdoPersonAccountEmploymentDetail.person_account_id = lbusExistingPersonAccountForEligibleRetPlan.icdoPersonAccount.person_account_id;
                                        lbusRetPlanMemberIsEligibleToEnroll.icdoPersonAccountEmploymentDetail.Update();
                                    }
                                    busPersonAccountRetirement lobjPersonAccountRetirement = new busPersonAccountRetirement();
                                    lobjPersonAccountRetirement.icdoPersonAccount = new cdoPersonAccount();
                                    lobjPersonAccountRetirement.icdoPersonAccountRetirement = new cdoPersonAccountRetirement();
                                    busPersonEmploymentDetail lbusPersonEmploymentDetail = new busPersonEmploymentDetail() { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                                    lbusPersonEmploymentDetail.FindPersonEmploymentDetail(icdoWssPersonEmploymentDetail.person_employment_dtl_id);
                                    if (lbusExistingPersonAccountForEligibleRetPlan?.icdoPersonAccount?.person_account_id > 0)
                                    {
                                        if (lobjPersonAccountRetirement.FindPersonAccountRetirement(lbusExistingPersonAccountForEligibleRetPlan.icdoPersonAccount.person_account_id))
                                        {
                                            lobjPersonAccountRetirement.LoadPlan();
                                            lobjPersonAccountRetirement.LoadPlanEffectiveDate();
                                            lobjPersonAccountRetirement.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonEmploymentDetail.person_employment_dtl_id;
                                            lobjPersonAccountRetirement.icdoPersonAccount.history_change_date = lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date;
                                            lobjPersonAccountRetirement.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirementEnrolled;
                                            lobjPersonAccountRetirement.LoadPersonEmploymentDetail();
                                            lobjPersonAccountRetirement.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                            lobjPersonAccountRetirement.LoadPerson();
                                            lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                                            lobjPersonAccountRetirement.LoadOrgPlan(lobjPersonAccountRetirement.idtPlanEffectiveDate);
                                            lobjPersonAccountRetirement.LoadFYTDSummary();
                                            lobjPersonAccountRetirement.LoadLTDSummary();
                                            lobjPersonAccountRetirement.LoadMissedDeposits();
                                            lobjPersonAccountRetirement.LoadAllPersonEmploymentDetails();
                                            lobjPersonAccountRetirement.LoadDBDCTransfer();
                                            lobjPersonAccountRetirement.LoadPersonAccountRetirementHistory();
                                            lobjPersonAccountRetirement.LoadPersonAccountAdjustment();
                                            lobjPersonAccountRetirement.LoadPersonAccountDBDBTransfer(true);
                                            lobjPersonAccountRetirement.LoadErrors();
                                            lobjPersonAccountRetirement.LoadPreviousHistory();
                                            lobjPersonAccountRetirement.CalculateERVestedPercentage();
                                            lobjPersonAccountRetirement.LoadTotalPSC();
                                            lobjPersonAccountRetirement.LoadCapitalGainContribution();
                                        }
                                        lobjPersonAccountRetirement.LoadPersonAccountForRetirementPlan();
                                        lobjPersonAccountRetirement.idtInitialStartDate = lobjPersonAccountRetirement.icdoPersonAccount.start_date;
                                        lobjPersonAccountRetirement.SetQDRO();
                                        lobjPersonAccountRetirement.RefreshValues();
                                    }
                                    else
                                    {
                                        lobjPersonAccountRetirement.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonEmploymentDetail.person_employment_dtl_id;
                                        lobjPersonAccountRetirement.icdoPersonAccount.history_change_date = lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date;
                                        lobjPersonAccountRetirement.icdoPersonAccount.start_date = icdoWssPersonEmployment.start_date;
                                        lobjPersonAccountRetirement.LoadPersonEmploymentDetail();
                                        lobjPersonAccountRetirement.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                        lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                                        lobjPersonAccountRetirement.ibusPersonEmploymentDetail.LoadPlansOffered();
                                        lobjPersonAccountRetirement.icdoPersonAccount.person_id = lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id;
                                        lobjPersonAccountRetirement.LoadPerson();
                                        lobjPersonAccountRetirement.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirementEnrolled;
                                        lobjPersonAccountRetirement.icdoPersonAccount.status_value = busConstant.StatusValid;
                                        if (lbusRetPlanMemberIsEligibleToEnroll.ibusPlan.IsDBRetirementPlan() || lbusRetPlanMemberIsEligibleToEnroll.ibusPlan.IsHBRetirementPlan())
                                        {
                                            DataTable ldtbList = busPersonAccountHelper.DeterminePlan(lobjPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value,
                                                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value);
                                            if (ldtbList.Rows.Count > 1)
                                            {
                                                foreach (DataRow dr in ldtbList.Rows)
                                                {
                                                    var lintPlanId = from lobj in lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans
                                                                     where lobj.icdoOrgPlan.plan_id == (int)dr["plan_id"]
                                                                     select Convert.ToInt32(dr["plan_id"]);
                                                    if (!lintPlanId.IsNullOrEmpty())
                                                    {
                                                        lobjPersonAccountRetirement.icdoPersonAccount.plan_id = lintPlanId.FirstOrDefault();
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                lobjPersonAccountRetirement.icdoPersonAccount.plan_id = (int)ldtbList.Rows[0]["plan_id"];
                                            }

                                            if (lobjPersonAccountRetirement.IsDCPlanEligible() && lobjPersonAccountRetirement.IsDcEligibilityDateRequired() // PIR 11483
                                                && (lobjPersonAccountRetirement.IsPopulateDCEligibilityDate(lobjPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date))) // PIR 11891
                                            {
                                                lobjPersonAccountRetirement.icdoPersonAccountRetirement.dc_eligibility_date =
                                                                                            lobjPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(180);
                                            }
                                        }
                                        else if (lbusRetPlanMemberIsEligibleToEnroll.ibusPlan.IsDCRetirementPlan())
                                        {
                                            if (lobjPersonAccountRetirement.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl == null)
                                                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
                                            foreach (busPersonAccountEmploymentDetail lobjPAEmploymentDetail in lobjPersonAccountRetirement.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl)
                                            {
                                                if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC &&
                                                    lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled)
                                                {
                                                    lobjPersonAccountRetirement.icdoPersonAccount.plan_id = busConstant.PlanIdDC;
                                                }
                                                else if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2020 &&
                                                    lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled)
                                                {
                                                    lobjPersonAccountRetirement.icdoPersonAccount.plan_id = busConstant.PlanIdDC2020;
                                                }
                                            }
                                        }
                                        lobjPersonAccountRetirement.LoadPlan();
                                        lobjPersonAccountRetirement.LoadOrgPlan();
                                        lobjPersonAccountRetirement.SetQDRO();
                                        lobjPersonAccountRetirement.icolPersonAccountRetirementHistory = new Collection<busPersonAccountRetirementHistory>();
                                    }
                                    utlPageMode lutlPageMode = lbusExistingPersonAccountForEligibleRetPlan?.icdoPersonAccount?.person_account_id > 0 ? utlPageMode.Update : utlPageMode.New;
                                    lobjPersonAccountRetirement.icdoPersonAccount.ienuObjectState = lutlPageMode == utlPageMode.New ? ObjectState.Insert : ObjectState.Update;
                                    lobjPersonAccountRetirement.icdoPersonAccountRetirement.ienuObjectState = lutlPageMode == utlPageMode.New ? ObjectState.Insert : ObjectState.Update;
                                    lobjPersonAccountRetirement.BeforeValidate(lutlPageMode);
                                    lobjPersonAccountRetirement.ValidateHardErrors(lutlPageMode);
                                    if (lobjPersonAccountRetirement.iarrErrors.Count > 0)
                                    {
                                        iblnBPMInitiate = true;
                                    }
                                    else
                                    {
                                        lobjPersonAccountRetirement.BeforePersistChanges();
                                        if (!lobjPersonAccountRetirement.iarrChangeLog.Any(dataobject => dataobject is cdoPersonAccountRetirement))
                                            lobjPersonAccountRetirement.iarrChangeLog.Add(lobjPersonAccountRetirement.icdoPersonAccountRetirement);
                                        lobjPersonAccountRetirement.PersistChanges();
                                        lobjPersonAccountRetirement.ibusMemberRecordRequest = this;
                                        lobjPersonAccountRetirement.AfterPersistChanges();

                                        //PIR 25920 DC 2025 changes 
                                        if(lobjPersonAccountRetirement.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025 && icdoWssPersonEmployment.start_date < lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date)
                                        {
                                            lobjPersonAccountRetirement.IsHistoryEntryRequired = true;
                                            lobjPersonAccountRetirement.icdoPersonAccount.history_change_date = busGlobalFunctions.GetFirstDayofNextMonth(lobjPersonAccountRetirement.icdoPersonAccount.history_change_date);
                                            lobjPersonAccountRetirement?.ProcessHistory();
                                        }
                                    }
                                }
                                else
                                    iblnBPMInitiate = true;
                            }
                        }
                        else if (lintCountOfRetPlansMemberIsEligibleToEnroll != 0)
                        {
                            iblnBPMInitiate = true;
                        }
                    }
                    else
                    {
                        if (icdoWssPersonEmploymentDetail.retr_status != busConstant.StringLetterS)
                        {
                            icdoWssPersonEmploymentDetail.retr_status = busConstant.StringLetterS;
                            icdoWssPersonEmploymentDetail.Update();
                        }
                    }
                    if (!ablnIsFromBatch && iobjPassInfo.iblnInTransaction)
                    {
                        iobjPassInfo.Commit();
                        iobjPassInfo.BeginTransaction();
                    }
                    #endregion
                    #region Start of EAP Plan Enrollment logic
                    int lintCountOfEAPPlansMemberIsEligibleToEnroll = lclbPersonAccountEmploymentDetail.Where(pa => pa.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdEAP).Count();
                    busPersonAccount lbusEAPPersonAccount = ibusPerson.LoadActivePersonAccountByPlanForNewHirePostingBatch(busConstant.PlanIdEAP);
                    if (lintCountOfEAPPlansMemberIsEligibleToEnroll == 1)
                    {
                        busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail = lclbPersonAccountEmploymentDetail.FirstOrDefault(pa => pa.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdEAP);
                        if (lbusEAPPersonAccount?.icdoPersonAccount?.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            iblnBPMInitiate = true;
                        }
                        else
                        {
                            if (lbusPersonAccountEmploymentDetail != null && lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value != busConstant.PersonAccountElectionValueEnrolled)
                            {
                                lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                                if (lbusEAPPersonAccount.icdoPersonAccount.person_account_id > 0)
                                    lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = lbusEAPPersonAccount.icdoPersonAccount.person_account_id;
                                lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();
                            }
                            busPersonAccountEAP lobjEAP = new busPersonAccountEAP();
                            if (lbusEAPPersonAccount?.icdoPersonAccount?.person_account_id > 0)
                            {
                                if (lobjEAP.FindPersonAccount(lbusEAPPersonAccount.icdoPersonAccount.person_account_id))
                                {
                                    lobjEAP.LoadPlanEffectiveDate();
                                    lobjEAP.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonEmploymentDetail.person_employment_dtl_id;
                                    lobjEAP.icdoPersonAccount.history_change_date = icdoWssPersonEmployment.start_date.GetFirstDayofNextMonth();
                                    lobjEAP.idtPlanEffectiveDate = lobjEAP.icdoPersonAccount.history_change_date;
                                    lobjEAP.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
                                    lobjEAP.LoadPersonEmploymentDetail();
                                    lobjEAP.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                    lobjEAP.LoadPlan();
                                    lobjEAP.LoadPerson();
                                    lobjEAP.LoadOrgPlan(lobjEAP.idtPlanEffectiveDate);
                                    lobjEAP.LoadActiveProviderOrgPlan(lobjEAP.idtPlanEffectiveDate);
                                    lobjEAP.icdoPersonAccount.provider_org_id = lobjEAP.GetDefaultEAPProvider();
                                    lobjEAP.LoadProviderOrgPlanByProviderOrgID(lobjEAP.icdoPersonAccount.provider_org_id, lobjEAP.idtPlanEffectiveDate);
                                    lobjEAP.GetMonthlyPremium();
                                    lobjEAP.LoadInsuranceYTD();
                                    lobjEAP.LoadEAPHistory();
                                    lobjEAP.LoadErrors();
                                    lobjEAP.LoadPreviousHistory();
                                    lobjEAP.LoadAllPersonEmploymentDetails();
                                    lobjEAP.LoadPersonAccountInsuranceTransfer();
                                    if (!lobjEAP.IsProviderOrgIDValid())
                                        lobjEAP.icdoPersonAccount.provider_org_id = 0;
                                }
                                lobjEAP.RefreshValues();
                                lobjEAP.LoadPersonAccountForEAP();
                            }
                            else
                            {
                                lobjEAP.icdoPersonAccount = new cdoPersonAccount();
                                lobjEAP.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonEmploymentDetail.person_employment_dtl_id;
                                lobjEAP.LoadPersonEmploymentDetail();
                                lobjEAP.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                lobjEAP.icdoPersonAccount.person_id = lobjEAP.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id;
                                lobjEAP.icdoPersonAccount.start_date = new DateTime(lobjEAP.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date.AddMonths(1).Year,
                                                                        lobjEAP.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date.AddMonths(1).Month, 1);
                                lobjEAP.icdoPersonAccount.history_change_date = lobjEAP.icdoPersonAccount.start_date;
                                lobjEAP.icdoPersonAccount.plan_id = busConstant.PlanIdEAP;
                                lobjEAP.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
                                lobjEAP.icdoPersonAccount.status_value = busConstant.StatusValid;
                                lobjEAP.LoadPlan();
                                lobjEAP.LoadPerson();
                                lobjEAP.LoadOrgPlan();
                                lobjEAP.LoadEapProvidersNewMode();
                                lobjEAP.icdoPersonAccount.provider_org_id = lobjEAP.GetDefaultEAPProvider();
                            }
                            utlPageMode lutlPageMode = lbusEAPPersonAccount?.icdoPersonAccount?.person_account_id > 0 ? utlPageMode.Update : utlPageMode.New;
                            lobjEAP.icdoPersonAccount.ienuObjectState = lutlPageMode == utlPageMode.New ? ObjectState.Insert : ObjectState.Update;
                            lobjEAP.BeforeValidate(lutlPageMode);
                            lobjEAP.ValidateHardErrors(lutlPageMode);
                            if (lobjEAP.iarrErrors.Count > 0)
                            {
                                iblnBPMInitiate = true;
                            }
                            else
                            {
                                lobjEAP.BeforePersistChanges();
                                lobjEAP.PersistChanges();
                                lobjEAP.ibusMemberRecordRequest = this;
                                lobjEAP.AfterPersistChanges();
                            }
                        }
                    }
                    else if (lintCountOfEAPPlansMemberIsEligibleToEnroll != 0)
                    {
                        iblnBPMInitiate = true;
                    }
                    if (!ablnIsFromBatch && iobjPassInfo.iblnInTransaction)
                    {
                        iobjPassInfo.Commit();
                        iobjPassInfo.BeginTransaction();
                    }
                    #endregion
                    #region Start of Life Plan Enrollment logic
                    int lintCountOfLifePlansMemberIsEligibleToEnroll = lclbPersonAccountEmploymentDetail.Where(pa => pa.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupLife).Count();
                    if (lintCountOfLifePlansMemberIsEligibleToEnroll == 1)
                    {
                        busPersonAccount lbusExistingLifePersonAccount = ibusPerson.LoadActivePersonAccountByPlanForNewHirePostingBatch(busConstant.PlanIdGroupLife);
                        lbusExistingLifePersonAccount.LoadPersonAccountLife();
                        lbusExistingLifePersonAccount.ibusPersonAccountLife.LoadPersonAccountLifeOptions();


                        if (lbusExistingLifePersonAccount?.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                            lbusExistingLifePersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeOption.Any(lifeoption => lifeoption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled))
                        {
                            iblnBPMInitiate = true;
                        }
                        else
                        {
                            busPersonAccountEmploymentDetail lbusLifePersonAccountEmploymentDetail = lclbPersonAccountEmploymentDetail.FirstOrDefault(pa => pa.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupLife);
                            if (lbusLifePersonAccountEmploymentDetail != null && lbusLifePersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value != busConstant.PersonAccountElectionValueEnrolled)
                            {
                                lbusLifePersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                                if (lbusExistingLifePersonAccount.icdoPersonAccount.person_account_id > 0)
                                    lbusLifePersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = lbusExistingLifePersonAccount.icdoPersonAccount.person_account_id;
                                lbusLifePersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();
                            }
                            busPersonAccountLife lobjGroupLife = new busPersonAccountLife();
                            lobjGroupLife.icdoPersonAccount = new cdoPersonAccount();
                            lobjGroupLife.icdoPersonAccountLife = new cdoPersonAccountLife();
                            if (lbusExistingLifePersonAccount?.icdoPersonAccount?.person_account_id > 0)
                            {
                                if (lobjGroupLife.FindPersonAccount(lbusExistingLifePersonAccount.icdoPersonAccount.person_account_id))
                                {
                                    if (lobjGroupLife.FindPersonAccountLife(lbusExistingLifePersonAccount.icdoPersonAccount.person_account_id))
                                    {
                                        lobjGroupLife.icdoPersonAccount.history_change_date = icdoWssPersonEmployment.start_date.GetFirstDayofNextMonth();
                                        lobjGroupLife.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
                                        lobjGroupLife.LoadPerson();
                                        lobjGroupLife.LoadPlan();
                                        lobjGroupLife.LoadLifeOption();
                                        lobjGroupLife.LoadLifeOptionData();
                                        lobjGroupLife.LoadPlanEffectiveDate();
                                        lobjGroupLife.LoadMemberAge(lobjGroupLife.idtPlanEffectiveDate);
                                        lobjGroupLife.LoadHistory();
                                        lobjGroupLife.LoadProviderName();
                                        lobjGroupLife.LoadPaymentElection();
                                        lobjGroupLife.LoadBillingOrganization();
                                        lobjGroupLife.LoadInsuranceYTD();
                                        lobjGroupLife.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonEmploymentDetail.person_employment_dtl_id;
                                        if (lobjGroupLife.icdoPersonAccount.person_employment_dtl_id != 0)
                                        {
                                            lobjGroupLife.LoadPersonEmploymentDetail();
                                            lobjGroupLife.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                            lobjGroupLife.LoadOrgPlan(lobjGroupLife.idtPlanEffectiveDate);
                                            lobjGroupLife.LoadProviderOrgPlan(lobjGroupLife.idtPlanEffectiveDate);
                                        }
                                        else
                                        {
                                            lobjGroupLife.LoadActiveProviderOrgPlan(lobjGroupLife.idtPlanEffectiveDate);
                                        }
                                        if (lobjGroupLife.icdoPersonAccountLife.premium_waiver_flag != busConstant.Flag_Yes)
                                        {
                                            lobjGroupLife.GetMonthlyPremiumAmount();
                                        }
                                        lobjGroupLife.LoadErrors();
                                        lobjGroupLife.LoadPreviousHistory();
                                        lobjGroupLife.LoadAllPersonEmploymentDetails();
                                        lobjGroupLife.LoadPersonAccountAchDetail();
                                        lobjGroupLife.LoadPersonAccountInsuranceTransfer();
                                        lobjGroupLife.LoadPaymentElectionHistory();
                                    }
                                }
                                lobjGroupLife.RefreshValues();
                                lobjGroupLife.LoadPersonAccount();
                            }
                            else
                            {
                                lobjGroupLife.icdoPersonAccount.person_id = icdoWssMemberRecordRequest.person_id;
                                lobjGroupLife.icdoPersonAccount.plan_id = busConstant.PlanIdGroupLife;
                                lobjGroupLife.ibusPaymentElection = new busPersonAccountPaymentElection();
                                lobjGroupLife.ibusPaymentElection.icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection();
                                lobjGroupLife.icdoPersonAccount.history_change_date = icdoWssPersonEmployment.start_date.GetFirstDayofNextMonth();
                                lobjGroupLife.icdoPersonAccount.start_date = lobjGroupLife.icdoPersonAccount.history_change_date;
                                lobjGroupLife.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
                                lobjGroupLife.icdoPersonAccount.status_value = busConstant.StatusValid;
                                lobjGroupLife.icdoPersonAccountLife.life_insurance_type_value = busConstant.LifeInsuranceTypeActiveMember;
                                lobjGroupLife.icdoPersonAccountLife.premium_conversion_indicator_flag = busConstant.Flag_No;
                                lobjGroupLife.LoadPerson();
                                lobjGroupLife.LoadPlan();
                                lobjGroupLife.LoadLifeOption();
                                foreach (busPersonAccountLifeOption lobjPersonAccountLifeOption in lobjGroupLife.iclbLifeOption)
                                {
                                    if (lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                                    {
                                        lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.effective_start_date = lobjGroupLife.icdoPersonAccount.history_change_date;
                                        DataTable ldtbCovergeAmount = Select("cdoWssPersonAccountEnrollmentRequest.GetValidCoverageAmount", new object[3] { busConstant.LevelofCoverage_Basic, busConstant.LifeInsuranceTypeActiveMember, lobjGroupLife.icdoPersonAccount.history_change_date });
                                        if (ldtbCovergeAmount.Rows.Count > 0)
                                            lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.coverage_amount = Convert.ToDecimal(ldtbCovergeAmount.Rows[0]["FULL_COVERAGE_AMT"]);
                                    }
                                }
                                lobjGroupLife.LoadMemberAge();
                                lobjGroupLife.LoadHistory();
                                lobjGroupLife.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonEmploymentDetail.person_employment_dtl_id;
                                lobjGroupLife.LoadPersonEmploymentDetail();
                                lobjGroupLife.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                lobjGroupLife.LoadOrgPlan();
                                lobjGroupLife.LoadProviderOrgPlan();

                            }
                            utlPageMode lutlPageMode = lbusExistingLifePersonAccount?.icdoPersonAccount?.person_account_id > 0 ? utlPageMode.Update : utlPageMode.New;
                            lobjGroupLife.icdoPersonAccount.ienuObjectState = lutlPageMode == utlPageMode.New ? ObjectState.Insert : ObjectState.Update;
                            lobjGroupLife.BeforeValidate(lutlPageMode);
                            lobjGroupLife.ValidateHardErrors(lutlPageMode);
                            if (lobjGroupLife.iarrErrors.Count > 0)
                            {
                                iblnBPMInitiate = true;
                            }
                            else
                            {
                                lobjGroupLife.BeforePersistChanges();
                                lobjGroupLife.PersistChanges();
                                lobjGroupLife.ibusMemberRecordRequest = this;
                                lobjGroupLife.AfterPersistChanges();
                            }
                        }
                    }
                    else if (lintCountOfLifePlansMemberIsEligibleToEnroll != 0)
                    {
                        iblnBPMInitiate = true;
                    }
                    if (!ablnIsFromBatch && iobjPassInfo.iblnInTransaction)
                    {
                        iobjPassInfo.Commit();
                        iobjPassInfo.BeginTransaction();
                    }
                    #endregion
                }
                else
                {
                    iblnBPMInitiate = true;
                }
            }
            else
                iblnBPMInitiate = true;

            if (iblnBPMInitiate && icdoWssMemberRecordRequest.status_value != busConstant.EmploymentChangeRequestStatusProcessed)
            {
                utlError lutlError = AddError(10452, "");
                larrMandatoryPlanEnrollmentResult.Add(lutlError);
                //return larrMandatoryPlanEnrollmentResult;
            }
            if (icdoWssMemberRecordRequest.status_value != busConstant.EmploymentChangeRequestStatusProcessed && !ablnIsFromPostClick && iblnBPMInitiate && icdoWssMemberRecordRequest.bpm_initiated != busConstant.Flag_Yes) InitiateBPM();
            return larrMandatoryPlanEnrollmentResult;
        }

        private void CreateNonContributingDetail()
        {
            if (icdoWssMemberRecordRequest.first_month_for_retirement_contribution != DateTime.MinValue && 
                icdoWssMemberRecordRequest.first_month_for_retirement_contribution > icdoWssPersonEmployment.start_date)
            {
                busPersonAccountEmploymentDetail lobjPAEmpDetail = new busPersonAccountEmploymentDetail() { icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail() };
                lobjPAEmpDetail.icdoPersonAccountEmploymentDetail.person_employment_dtl_id = icdoWssPersonEmploymentDetail.person_employment_dtl_id;
                lobjPAEmpDetail.LoadPersonEmploymentDetail();
                lobjPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date = icdoWssMemberRecordRequest.first_month_for_retirement_contribution.AddDays(-1);
                lobjPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusNonContributing;
                lobjPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.Update();

                busPersonEmploymentDetail lobjEmpDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lobjEmpDetail.icdoPersonEmploymentDetail.person_employment_id = lobjPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id;
                lobjEmpDetail.icdoPersonEmploymentDetail.start_date = icdoWssMemberRecordRequest.first_month_for_retirement_contribution;
                lobjEmpDetail.icdoPersonEmploymentDetail.type_value = icdoWssPersonEmploymentDetail.type_value;
                lobjEmpDetail.icdoPersonEmploymentDetail.job_class_value = icdoWssPersonEmploymentDetail.job_class_value;
                lobjEmpDetail.icdoPersonEmploymentDetail.term_begin_date = icdoWssPersonEmploymentDetail.term_begin_date;
                lobjEmpDetail.icdoPersonEmploymentDetail.official_list_value = icdoWssPersonEmploymentDetail.official_list_value;
                lobjEmpDetail.icdoPersonEmploymentDetail.seasonal_value = icdoWssPersonEmploymentDetail.seasonal_value;
                lobjEmpDetail.icdoPersonEmploymentDetail.hourly_value = icdoWssPersonEmploymentDetail.hourly_value;
                lobjEmpDetail.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusContributing;
                lobjEmpDetail.icdoPersonEmploymentDetail.Insert();
                icdoWssPersonEmploymentDetail.person_employment_dtl_id = lobjEmpDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                icdoWssPersonEmploymentDetail.Update();
                if (lobjEmpDetail.ibusPersonEmployment.IsNull()) lobjEmpDetail.LoadPersonEmployment();
                if (lobjEmpDetail.ibusPersonEmployment.ibusOrganization.IsNull()) lobjEmpDetail.ibusPersonEmployment.LoadOrganization();
                lobjEmpDetail.InsertEmployerOfferedPlans();
            }
        }

        public DateTime idtmRejectedDate { get; set; } //PIR 24183
        public string istrRejectionReason { get; set; }//PIR 24183

        public bool iblnBPMInitiate = false;

        public void LoadContactOrg()
        {
            if (ibusContactOrg == null)
                ibusContactOrg = new busOrganization();
            ibusContactOrg.FindOrganization(icdoWssPersonContact.contact_org_id);
        }
        public busPerson ibusContactPerson { get; set; }

        public void LoadContactPerson()
        {
            if (ibusContactPerson == null)
                ibusContactPerson = new busPerson();
            ibusContactPerson.FindPerson(icdoWssPersonContact.contact_person_id);
        }
        public void SetContactAddress()
        {

            if (icdoWssPersonContact.contact_person_id > 0 && icdoWssPersonContact.same_as_member_address == busConstant.Flag_Yes)
            {
                icdoWssPersonContact.address_line_1 = icdoWssPersonAddress.addr_line_1;
                icdoWssPersonContact.address_line_2 = icdoWssPersonAddress.addr_line_2;
                icdoWssPersonContact.address_city = icdoWssPersonAddress.addr_city;
                icdoWssPersonContact.address_country_value = icdoWssPersonAddress.addr_country_value;
                icdoWssPersonContact.address_state_value = icdoWssPersonAddress.addr_state_value;
                icdoWssPersonContact.foreign_postal_code = icdoWssPersonAddress.foreign_postal_code;
                icdoWssPersonContact.foreign_province = icdoWssPersonAddress.foreign_province;
                icdoWssPersonContact.address_zip_code = icdoWssPersonAddress.addr_zip_code;
                icdoWssPersonContact.address_zip_4_code = icdoWssPersonContact.address_zip_4_code;
                icdoWssPersonContact.Insert();
            }
            else if (!string.IsNullOrEmpty(icdoWssPersonContact.contact_org_code))
            {
                if (ibusContactOrg == null)
                    LoadContactOrg();
                ibusContactOrg.LoadOrgPrimaryAddress();
                icdoWssPersonContact.address_line_1 = ibusContactOrg.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_1;
                icdoWssPersonContact.address_line_2 = ibusContactOrg.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2;
                icdoWssPersonContact.address_city = ibusContactOrg.ibusOrgPrimaryAddress.icdoOrgContactAddress.city;
                icdoWssPersonContact.address_country_value = ibusContactOrg.ibusOrgPrimaryAddress.icdoOrgContactAddress.country_value;
                icdoWssPersonContact.address_state_value = ibusContactOrg.ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value;
                icdoWssPersonContact.foreign_province = ibusContactOrg.ibusOrgPrimaryAddress.icdoOrgContactAddress.foreign_province;
                icdoWssPersonContact.foreign_postal_code = ibusContactOrg.ibusOrgPrimaryAddress.icdoOrgContactAddress.foreign_postal_code;
                icdoWssPersonContact.address_zip_code = ibusContactOrg.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code;
                icdoWssPersonContact.address_zip_4_code = ibusContactOrg.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code;//Internal finding
            }
        }
        public bool iblnIsSameEmployerExistsInActiveEmployment { get; set; }
        //PIR 24663
        //start
        public Collection<cdoCodeValue> LoadRetirementParticipationStatusValueDropdown()
        {
            if (ibusOrganization == null)
            {
                LoadOrganization();
            }
            return ibusOrganization.LoadEmployerStatusValueDropdown();
        }
        //end

        public override void BeforeWizardStepValidate(utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {
            if (aenmPageMode == utlPageMode.New)
                iblnIsNewMode = true;
            else
                icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusReview;
            istrStepName = astrWizardStepName;
            LoadOrganization();
            switch (astrWizardStepName)
            {
                case "wzsMemberEmploymentDetails":
                    icdoWssMemberRecordRequest.gender_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(300,
                                                                        icdoWssMemberRecordRequest.gender_value);
                    icdoWssMemberRecordRequest.marital_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(306,
                                                                    icdoWssMemberRecordRequest.marital_status_value);
                    //PIR 11127  The Spouse details will be displayed on the Summary screen only if the marital status is Married.
                    if (icdoWssMemberRecordRequest.marital_status_value != busConstant.PersonMaritalStatusMarried)
                    {
                        icdoWssPersonContact.contact_first_name = string.Empty;
                        icdoWssPersonContact.contact_last_name = string.Empty;
                        icdoWssPersonContact.contact_middle_name = string.Empty;
                        icdoWssPersonContact.contact_ssn = string.Empty;
                        icdoWssPersonContact.relationship_description = string.Empty;
                        icdoWssPersonContact.contact_gender_description = string.Empty;
                    }
                    else
                    {
                        icdoWssPersonContact.relationship_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(304,
                                                                        icdoWssPersonContact.relationship_value);
                        icdoWssPersonContact.contact_gender_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(300,
                                                                            icdoWssPersonContact.contact_gender_value);
                    }
                    icdoWssPersonEmploymentDetail.hourly_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(311,
                                                                        icdoWssPersonEmploymentDetail.hourly_value);
                    icdoWssPersonEmploymentDetail.job_class_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(314,
                                                                        icdoWssPersonEmploymentDetail.job_class_value);
                    icdoWssPersonEmploymentDetail.official_list_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(318,
                                                                        icdoWssPersonEmploymentDetail.official_list_value);
                    icdoWssPersonEmploymentDetail.seasonal_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(312,
                                                                        icdoWssPersonEmploymentDetail.seasonal_value);
                    icdoWssPersonEmploymentDetail.type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(313,
                                                                        icdoWssPersonEmploymentDetail.type_value);
                    icdoWssPersonEmploymentDetail.employment_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(310,
                                                                        icdoWssPersonEmploymentDetail.employment_status_value);

                    if (!string.IsNullOrEmpty(icdoWssPersonEmploymentDetail.istrMemberWorkLessThan12MonthsValue))
                    {
                        icdoWssPersonEmploymentDetail.seasonal_value = icdoWssPersonEmploymentDetail.istrMemberWorkLessThan12MonthsValue.Equals("NO") ? string.Empty : icdoWssPersonEmploymentDetail.seasonal_value;
                        icdoWssPersonEmploymentDetail.seasonal_description = icdoWssPersonEmploymentDetail.istrMemberWorkLessThan12MonthsValue.Equals("NO") ? string.Empty : icdoWssPersonEmploymentDetail.seasonal_description;
                    }

                    break;
                case "wzsMemberDetails":
                    VerifyAddressUsingUSPS();
                    break;
            }
            base.BeforeWizardStepValidate(aenmPageMode, astrWizardName, astrWizardStepName);
        }
        public override void BeforePersistChanges()
        {
            SetContactAddress();
            icdoWssPersonAddress.address_type_value = busConstant.AddressTypePermanent;
            if(icdoWssMemberRecordRequest.first_month_for_retirement_contribution != DateTime.MinValue && 
                icdoWssMemberRecordRequest.first_month_for_retirement_contribution.Month == icdoWssPersonEmployment.start_date.Month 
                && icdoWssMemberRecordRequest.first_month_for_retirement_contribution.Year == icdoWssPersonEmployment.start_date.Year)
            {
                icdoWssMemberRecordRequest.first_month_for_retirement_contribution = icdoWssPersonEmployment.start_date;
            }
            //Auto posting
            //icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusReview;
            icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusPendingAutoPosting;
            base.BeforePersistChanges();
        }
        public busOrganization ibusOrganization { get; set; }
        public void LoadOrganization()
        {
            if (ibusOrganization == null)
                ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(icdoWssMemberRecordRequest.org_id);
        }
        public bool CheckActiveEmploymentExists()
        {
            //PIR 13852
            iblnIsSameEmployerExistsInActiveEmployment = false;
            busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
            DataTable ldtPerson = LoadPersonByName();
            if (ldtPerson.Rows.Count > 0)
            {
                lobjPerson.icdoPerson.LoadData(ldtPerson.Rows[0]);
            }
            lobjPerson.LoadActivePersonEmployment();
            icdoWssMemberRecordRequest.person_id = lobjPerson.icdoPerson.person_id;
            //PIR-11030 Check In 25 September To skip the Validation in Case of Same Organization and Different Record Number
            if (icdoWssMemberRecordRequest.ps_initiated_flag == busConstant.Flag_Yes && lobjPerson.iclbActivePersonEmployment.Where(o => o.icdoPersonEmployment.org_id == icdoWssMemberRecordRequest.org_id).Count() > 0 &&
                icdoWssPersonEmployment.ps_empl_record_number != lobjPerson.iclbActivePersonEmployment.Where(o => o.icdoPersonEmployment.org_id == icdoWssMemberRecordRequest.org_id).FirstOrDefault().icdoPersonEmployment.ps_empl_record_number)
            {
                iblnIsSameEmployerExistsInActiveEmployment = false;
                return iblnIsSameEmployerExistsInActiveEmployment;
            }
            if (lobjPerson.iclbActivePersonEmployment.Where(o => o.icdoPersonEmployment.org_id == icdoWssMemberRecordRequest.org_id).Any())
                iblnIsSameEmployerExistsInActiveEmployment = true;
            return iblnIsSameEmployerExistsInActiveEmployment;
        }

        private DataTable LoadPersonByName()
        {
            DataTable ldtPerson = SelectWithOperator<cdoPerson>(new string[3] { "ssn", "first_name", "last_name" },
                                                                new string[3] { "=", "=", "=" },
                                                                new object[3]{icdoWssMemberRecordRequest.ssn,icdoWssMemberRecordRequest.first_name,
                                                                                        icdoWssMemberRecordRequest.last_name},
                                                                null);
            return ldtPerson;
        }
        private string istrStepName;
        private bool iblnIsNewMode = false;
        //   istrStepName = astrWizardStepName;

        public override void ValidateGroupRules(string astrGroupName, utlPageMode aenmPageMode)
        {
            if (!string.IsNullOrEmpty(icdoWssPersonContact.contact_org_code))
            {
                if (ibusContactOrg == null)
                    ibusContactOrg = new busOrganization();
                ibusContactOrg.FindOrganizationByOrgCode(icdoWssPersonContact.contact_org_code);
                icdoWssPersonContact.contact_org_id = ibusContactOrg.icdoOrganization.org_id;
            }
            else if (!string.IsNullOrEmpty(icdoWssPersonContact.contact_ssn))
            {
                LoadPersonContactPerson();
                icdoWssPersonContact.contact_person_id = ibusPersonContactPerson.icdoPerson.person_id;
                LoadContactPerson();
            }
            base.ValidateGroupRules(astrGroupName, aenmPageMode);

            if (iblnIsFromESS)
            {
                foreach (utlError lobjError in iarrErrors)
                {
                    lobjError.istrErrorID = string.Empty;
                }
            }
        }
        public Collection<cdoCodeValue> LoadJobClassByEmployerCategory()
        {
            //PROD Pir - 4555
            if (ibusOrganization.iclbOrgPlan == null)
                ibusOrganization.LoadOrgPlan();
            bool lblnLEExists = ibusOrganization.iclbOrgPlan.Where(o => o.icdoOrgPlan.plan_id == busConstant.PlanIdLE ||
                                                                    o.icdoOrgPlan.plan_id == busConstant.PlanIdLEWithoutPS ||
                                                                    o.icdoOrgPlan.plan_id == busConstant.PlanIdBCILawEnf || //pir 7943
                                                                    o.icdoOrgPlan.plan_id == busConstant.PlanIdNG || //PIR 25729
                                                                    o.icdoOrgPlan.plan_id == busConstant.PlanIdStatePublicSafety).Any(); //PIR 25729
            Collection<cdoCodeValue> lclbJobClasses = new Collection<cdoCodeValue>();
            DataTable ldtbResult = Select<cdoCodeValue>(new string[1] { "CODE_ID" }, new object[1] { 322 }, null, null);
            ldtbResult = ldtbResult.AsEnumerable().Where(o =>
                (o.Field<string>("DATA1") == ibusOrganization.icdoOrganization.emp_category_value) &&
                ((o.Field<string>("DATA3") == ibusOrganization.icdoOrganization.org_code) || (o.Field<string>("DATA3").IsEmpty()))).AsDataTable();
            DataTable ldtbTemp = Select<cdoCodeValue>(new string[1] { "CODE_ID" }, new object[1] { 314 }, null, null);
            foreach (DataRow ldtr in ldtbResult.Rows)
            {
                foreach (DataRow ldtrRow in ldtbTemp.Rows)
                {
                    if ((Convert.ToString(ldtrRow["CODE_VALUE"]) == (Convert.ToString(ldtr["DATA2"])))
                            && (Convert.ToString(ldtrRow["DATA1"]) == busConstant.Flag_Yes))
                    {
                        //PROD Pir 4555
                        if ((Convert.ToString(ldtrRow["CODE_VALUE"]) == busConstant.JobClassCorrectionalOfficer ||
                            Convert.ToString(ldtrRow["CODE_VALUE"]) == busConstant.JobClassPeaceOfficer) && lblnLEExists)
                        {
                            cdoCodeValue lcdoCV = new cdoCodeValue();
                            lcdoCV.LoadData(ldtrRow);
                            lclbJobClasses.Add(lcdoCV);
                        }
                        else if (Convert.ToString(ldtrRow["CODE_VALUE"]) != busConstant.JobClassCorrectionalOfficer &&
                            Convert.ToString(ldtrRow["CODE_VALUE"]) != busConstant.JobClassPeaceOfficer)
                        {
                            cdoCodeValue lcdoCV = new cdoCodeValue();
                            lcdoCV.LoadData(ldtrRow);
                            lclbJobClasses.Add(lcdoCV);
                        }
                    }
                }
            }
            return lclbJobClasses;
        }
        public string istrIsTermsAndConditionsAgreed { get; set; }

        private void InitializeWorkFlow()
        {
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Member_Record_Request, icdoWssMemberRecordRequest.person_id, 0, icdoWssMemberRecordRequest.member_record_request_id, iobjPassInfo, busConstant.BPMProcessSource_Batch);
        }
        public override int PersistChanges()
        {
            int lintRtn = 0;
            if (icdoWssMemberRecordRequest.member_record_request_id == 0)
            {
                if (LoadPersonByName().Rows.Count > 0)
                {
                    if (ibusPerson == null) ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    ibusPerson.icdoPerson.LoadData(LoadPersonByName().Rows[0]);
                }
                if (ibusPerson != null)
                    icdoWssMemberRecordRequest.person_id = ibusPerson.icdoPerson.person_id;
                icdoWssMemberRecordRequest.Insert();
                icdoWssPersonAddress.member_record_request_id = icdoWssMemberRecordRequest.member_record_request_id;
                icdoWssPersonAddress.Insert();
                if (icdoWssMemberRecordRequest.marital_status_value == busConstant.PersonMaritalStatusMarried) //PIR 11127 
                {
                    icdoWssPersonContact.member_record_request_id = icdoWssMemberRecordRequest.member_record_request_id;
                    icdoWssPersonContact.contact_name = icdoWssPersonContact.FullName; // PIR 10071
                    icdoWssPersonContact.Insert();
                }
                if (ibusOrganization == null)
                {
                    LoadOrganization();
                }
                icdoWssPersonEmployment.org_id = ibusOrganization.icdoOrganization.org_id;
                icdoWssPersonEmployment.member_record_request_id = icdoWssMemberRecordRequest.member_record_request_id;
                icdoWssPersonEmployment.Insert();
                icdoWssPersonEmploymentDetail.member_record_request_id = icdoWssMemberRecordRequest.member_record_request_id;
                icdoWssPersonEmploymentDetail.wss_person_employment_id = icdoWssPersonEmployment.wss_person_employment_id;
                icdoWssPersonEmploymentDetail.Insert();
                lintRtn = 1;
            }
            else
            {
                icdoWssMemberRecordRequest.Update();
                icdoWssPersonAddress.Update();
                if (icdoWssMemberRecordRequest.marital_status_value == busConstant.PersonMaritalStatusMarried)
                    icdoWssPersonContact.contact_name = icdoWssPersonContact.FullName;
                if (icdoWssPersonContact != null && icdoWssPersonContact.wss_person_contact_id > 0) //Update if there is entry in Contact PIR-11030
                    icdoWssPersonContact.Update();
                icdoWssPersonEmployment.Update();
                icdoWssPersonEmploymentDetail.Update();
                lintRtn = 1;


            }
            return lintRtn;
        }

        public void InitiateBPM()
        {
            busNeobaseBpmRequest lbusNeobaseBpmRequest = ClassMapper.GetObject<busNeobaseBpmRequest>();
            long referenceId = lbusNeobaseBpmRequest.icdoBpmRequest.reference_id;

            DataTable ldtRunningInstance =
                busBase.Select("entSolBpmActivityInstance.LoadRunningInstancesByPersonProcessRef",
                                new object[3] { busConstant.Map_Member_Record_Request, icdoWssMemberRecordRequest.member_record_request_id, referenceId });
            if (ldtRunningInstance.Rows.Count > 0)
            {
                busSolBpmCaseInstance lbusBpmCaseInstance = new busSolBpmCaseInstance();
                busBpmActivityInstance lbusActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst(Convert.ToInt32(ldtRunningInstance.Rows[0]["activity_instance_id"]));
                if (lbusActivityInstance.ibpmInitiator.ibusBpmActivity is busBpmStartEvent)
                    busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusResumed, lbusActivityInstance, iobjPassInfo);
                else
                    InitializeWorkFlow();
            }
            else
            {
                InitializeWorkFlow();
            }
            icdoWssMemberRecordRequest.bpm_initiated = busConstant.Flag_Yes;
            icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusReview;
            icdoWssMemberRecordRequest.Update();
            //if (icdoWssPersonEmploymentDetail.person_employment_dtl_id > 0)
            //    icdoWssPersonEmploymentDetail.Update();
        }
        /// <summary>
        /// method to check whether same person exists for the ssn, first name, last name combination
        /// </summary>
        /// <returns>person id</returns>
        public int IsSamePersonExists()
        {

            int lintPersonID = 0;
            DataTable ldtPerson = SelectWithOperator<cdoPerson>(new string[3] { "ssn", "first_name", "last_name" },
                                                                new string[3] { "=", "=", "=" },
                                                                new object[3]{icdoWssMemberRecordRequest.ssn,icdoWssMemberRecordRequest.first_name,
                                                                                        icdoWssMemberRecordRequest.last_name},
                                                                null);
            if (ldtPerson.Rows.Count > 0)
                lintPersonID = Convert.ToInt32(ldtPerson.Rows[0]["person_id"]);
            return lintPersonID;
        }

        public bool IsSamePersonExistsBySSN()
        {
            DataTable ldtPerson = SelectWithOperator<cdoPerson>(new string[1] { "ssn" },
                                                                new string[1] { "=" },
                                                                new object[1] { icdoWssMemberRecordRequest.ssn },
                                                                null);
            if (ldtPerson.Rows.Count > 0)
                return true;
            else
                return false;
        }

        //PIR 14039
        public bool IsSamePersonContactExistsBySSN()
        {
            DataTable ldtPersonContact = SelectWithOperator<cdoPerson>(new string[1] { enmPerson.ssn.ToString() },
                                                                       new string[1] { "=" },
                                                                       new object[1] { icdoWssPersonContact.contact_ssn },
                                                                       null);

            return (ldtPersonContact.Rows.Count > 0);
        }

        public bool IsOfficialListNullOREmpty()
        {
            if (icdoWssPersonEmploymentDetail.official_list_value == null || icdoWssPersonEmploymentDetail.official_list_value.Trim() == string.Empty)
            {
                return true;
            }
            return false;
        }
        //PIR 14656 - Add soft error when DB_ADDL_CONTRIB is Y 
        private bool IsPersonsDbAddlFlagYes()
        {
            int lintPersonID = 0;
            if (icdoWssMemberRecordRequest.person_id > 0)
                lintPersonID = icdoWssMemberRecordRequest.person_id;
            else
                lintPersonID = IsSamePersonExists();
            if (lintPersonID > 0)
            {
                busPerson lbusPerson = new busPerson();
                lbusPerson.FindPerson(lintPersonID);
                return (!string.IsNullOrEmpty(lbusPerson.icdoPerson.db_addl_contrib) && lbusPerson.icdoPerson.db_addl_contrib.ToUpper() == busConstant.Flag_Yes) ? true : false;
            }
            return false;
        }
        public ArrayList Post_click(bool ablnIsFromBatch = false)
        {
            ArrayList larrReturn = new ArrayList();
            utlError lobjError = new utlError();
            string lstrValidateError = string.Empty;
            //PROD PIR 4254
            //if (icdoWssMemberRecordRequest.person_id == 0)
            //{
            //    lobjError = new utlError();
            //    lobjError = AddError(4181, string.Empty);
            //    larrReturn.Add(lobjError);
            //}
            //else 

            //PIR 10237 --  User should not  allow to modify their own record 
            //FW Upgrade PIR ID : 17179 Address is invalid error is not getting display on View Member Record Request Maintenance after set up new employee from ESS.
            if (string.IsNullOrEmpty(istrSuppressUSPSValidation))
            {
                istrSuppressUSPSValidation = busConstant.Flag_No;
            }

            DataTable dtUserInfo = iobjPassInfo.isrvDBCache.GetUserInfo(iobjPassInfo.istrUserID);

            if (dtUserInfo?.Rows.Count > 0 && !String.IsNullOrEmpty(Convert.ToString(dtUserInfo.Rows[0]["Person_ID"])) && icdoWssMemberRecordRequest.person_id == Convert.ToInt32(dtUserInfo.Rows[0]["Person_ID"]))
            {
                utlError lerror = new utlError();
                lerror.istrErrorID = "10275";
                lerror.istrErrorMessage = "Request can not be posted by the same user who created the request.";
                larrReturn.Add(lerror);
                return larrReturn;

            }

            if (icdoWssMemberRecordRequest.person_id == 0 && IsSamePersonExistsBySSN())
            {
                lobjError = new utlError();
                lobjError = AddError(1076, string.Empty);
                larrReturn.Add(lobjError);
            } //PIR 14656 - Add soft error when DB_ADDL_CONTRIB is Y 
            if (IsPersonsDbAddlFlagYes() && istrSuppressUSPSValidation != busConstant.Flag_Yes) //PIR 26237 Suppress Warning
            {
                lobjError = new utlError();
                lobjError = AddError(10270, string.Empty);
                larrReturn.Add(lobjError);
            }//PIR 14656 - Add soft error when DB_ADDL_CONTRIB is Y 
            if ((string.IsNullOrEmpty(istrSuppressPersonDuplicateValidation) || istrSuppressPersonDuplicateValidation == busConstant.Flag_No) && (IsPersonDuplicated() /*|| IsSamePersonExistsBySSN()*/))
            {
                lobjError = new utlError();
                lobjError = AddError(1152, string.Empty);
                larrReturn.Add(lobjError);
            }
            //PIR 14039 - Check if Spouse's SSN is duplicate.
            if (icdoWssPersonContact.contact_person_id == 0 && icdoWssPersonContact.contact_ssn.IsNotNullOrEmpty() && IsSamePersonContactExistsBySSN())
            {
                lobjError = new utlError();
                lobjError = AddError(10295, string.Empty);
                larrReturn.Add(lobjError);
            }
            // PIR 14802
            if (icdoWssPersonEmploymentDetail.person_employment_dtl_id == 0 && IsMemberHavingOpenEmploymentForSameOrg())
            {
                lobjError = new utlError();
                lobjError = AddError(10293, string.Empty);
                larrReturn.Add(lobjError);
            }
            if (icdoWssPersonEmploymentDetail.employment_status_value == busConstant.EmploymentStatusContributing && (icdoWssMemberRecordRequest.first_month_for_retirement_contribution < icdoWssPersonEmployment.start_date || icdoWssMemberRecordRequest.first_month_for_retirement_contribution > icdoWssPersonEmployment.start_date.AddDays(45)))
            {
                utlError lutlError = AddError(10452, "");
                larrReturn.Add(lutlError);
            }
            if (larrReturn.Count > 0)
            {
                return larrReturn;
            }
            else
            {
                if (icdoWssPersonEmploymentDetail.person_employment_dtl_id == 0)
                {
                    CreatePersonDetails();
                    CreatePersonAddress();
                    //PIR 17294 - Create contact details only if there is data in sgt_wss_person_contact table
                    if ((icdoWssMemberRecordRequest.ps_initiated_flag).IsNotNullOrEmpty() && icdoWssMemberRecordRequest.ps_initiated_flag != busConstant.Flag_Yes)
                        CreatePersonContact();
                    ArrayList larrRequestError = CreatePersonEmployment();
                    if (larrRequestError.Count > 0)
                    {
                        foreach (utlError lobjErr in larrRequestError)
                        {
                            larrReturn.Add(lobjErr);
                        }
                        return larrReturn;
                    }
                    icdoWssMemberRecordRequest.person_id = ibusPerson.icdoPerson.person_id;
                    //icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusProcessed;
                    icdoWssMemberRecordRequest.posted_date = DateTime.Now;
                    icdoWssMemberRecordRequest.posted_in_perslink_by = iobjPassInfo.istrUserID;
                    icdoWssMemberRecordRequest.rejection_reason = null;
                    icdoWssMemberRecordRequest.Update();

                    // PIR 11883 - update person_id in SGW_WORKFLOW_REQUEST and SGW_PROCESS_INSTANCE
                    LoadWorkFlowRequest(icdoWssMemberRecordRequest.member_record_request_id);
                    foreach (busSolBpmRequest lobjWorkflowRequest in iclbWorkflowRequest)
                    {
                        if (lobjWorkflowRequest.icdoBpmRequest.person_id == 0)
                        {
                            LoadProcessInstance(lobjWorkflowRequest.icdoBpmRequest.process_id, lobjWorkflowRequest.icdoBpmRequest.request_id);
                            foreach (busSolBpmCaseInstance lobjProcessInstance in iclbProcessInstance)
                            {
                                if (lobjProcessInstance.icdoBpmCaseInstance.person_id == 0)
                                {
                                    lobjWorkflowRequest.icdoBpmRequest.person_id = icdoWssMemberRecordRequest.person_id;
                                    lobjWorkflowRequest.icdoBpmRequest.Update();

                                    lobjProcessInstance.icdoBpmCaseInstance.person_id = icdoWssMemberRecordRequest.person_id;
                                    lobjProcessInstance.icdoBpmCaseInstance.Update();
                                }
                            }
                        }
                    }

                    //PROD PIR 4971
                    //--start--//
                    DataTable ldtPersonEmployment = Select<cdoPersonEmployment>(new string[2] { "person_id", "start_date" },
                                                                           new object[2] { ibusPerson.icdoPerson.person_id, icdoWssPersonEmployment.start_date },
                                                                           null, null);
                    if (ldtPersonEmployment.Rows.Count > 0)
                    {
                        busPersonEmployment lobjPE = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                        lobjPE.icdoPersonEmployment.LoadData(ldtPersonEmployment.Rows[0]);
                        lobjPE.LoadOrganization();
                        int lintContactID = 0;
                        if (icdoWssMemberRecordRequest.contact_id > 0)
                            lintContactID = icdoWssMemberRecordRequest.contact_id;
                        else
                        {
                            lobjPE.ibusOrganization.LoadESSPrimaryAuthorizedContact();
                            lintContactID = lobjPE.ibusOrganization.ibusESSPrimaryOrgContact.icdoOrgContact.contact_id;
                        }
                        string lstrPrioityValue = string.Empty;
                        busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(5, iobjPassInfo, ref lstrPrioityValue), ibusPerson.icdoPerson.FullName, ibusPerson.icdoPerson.person_id),//pir 8148 //pir 13882
                            lstrPrioityValue, aintOrgID: lobjPE.ibusOrganization.icdoOrganization.org_id,
                            aintContactID: lintContactID);
                    }
                    CreateNonContributingDetail();
                }
                if (!ablnIsFromBatch && iobjPassInfo.iblnInTransaction)
                {
                    iobjPassInfo.Commit();
                    iobjPassInfo.BeginTransaction();
                }
                //--end--//
                if (!ablnIsFromBatch && icdoWssPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                {
                    ArrayList larrPlanEnrollmentResult = AutoEnrollPersonInMandatoryPlans(true, false);
                    if (larrPlanEnrollmentResult.Count > 0 && larrPlanEnrollmentResult[0] is utlError)
                    {
                        RefreshStatuses();
                        larrReturn = larrPlanEnrollmentResult;
                        return larrPlanEnrollmentResult;
                    }
                    else
                    {
                        UpdateRequestStatus();
                    }
                }
                RefreshStatuses();
                this.EvaluateInitialLoadRules();
                larrReturn.Add(this);
            }
            return larrReturn;
        }

        public void UpdateRequestStatus()
        {
            if (icdoWssPersonEmploymentDetail.retr_status == busConstant.StringLetterS && icdoWssMemberRecordRequest.status_value != busConstant.EmploymentChangeRequestStatusProcessed)
            {
                icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusProcessed;
                icdoWssMemberRecordRequest.Update();
            }
        }

        private void RefreshStatuses()
        {
            icdoWssMemberRecordRequest.SelectRow(new object[1] { icdoWssMemberRecordRequest.member_record_request_id });
            LoadWssPersonEmploymentDetail();
            DisplayWssMemberPlanEnrollmentStatus();
        }

        public void LoadWssPersonEmploymentDetail()
        {
            DataTable ldtrWSSPersonEmploymentDetail = Select<cdoWssPersonEmploymentDetail>(new string[1] { enmWssMemberRecordRequest.member_record_request_id.ToString() }, new object[1] { icdoWssMemberRecordRequest.member_record_request_id }, null, null);
            if (icdoWssPersonEmploymentDetail == null) icdoWssPersonEmploymentDetail = new cdoWssPersonEmploymentDetail();
            if (ldtrWSSPersonEmploymentDetail.Rows.Count > 0) icdoWssPersonEmploymentDetail.LoadData(ldtrWSSPersonEmploymentDetail.Rows[0]);
        }

        public void DisplayWssMemberPlanEnrollmentStatus()
        {
            icdoWssMemberRecordRequest.istrRetrStatus = icdoWssPersonEmploymentDetail.retr_status == "Y" ? "Done" : "Manually Enroll";
            icdoWssMemberRecordRequest.istrEapStatus = icdoWssPersonEmploymentDetail.eap_status == "Y" ? "Done" : "Manually Enroll";
            icdoWssMemberRecordRequest.istrLifeStatus = icdoWssPersonEmploymentDetail.life_status == "Y" ? "Done" : "Manually Enroll";
        }

        public string VerifyAddressUsingUSPS(ref string astrValidateError)
        {
            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = icdoWssPersonAddress.addr_line_1;
            _lobjcdoWebServiceAddress.addr_line_2 = icdoWssPersonAddress.addr_line_2;
            _lobjcdoWebServiceAddress.addr_city = icdoWssPersonAddress.addr_city;
            _lobjcdoWebServiceAddress.addr_state_value = icdoWssPersonAddress.addr_state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = icdoWssPersonAddress.addr_zip_code;
            _lobjcdoWebServiceAddress.addr_zip_4_code = icdoWssPersonAddress.addr_zip_4_code;
            cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
            astrValidateError = _lobjcdoWebServiceAddressResult.address_validate_error;
            icdoWssPersonAddress.addr_line_1 = _lobjcdoWebServiceAddressResult.addr_line_1;
            icdoWssPersonAddress.addr_line_2 = _lobjcdoWebServiceAddressResult.addr_line_2;
            icdoWssPersonAddress.addr_city = _lobjcdoWebServiceAddressResult.addr_city;
            icdoWssPersonAddress.addr_state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
            icdoWssPersonAddress.addr_zip_code = _lobjcdoWebServiceAddressResult.addr_zip_code;
            icdoWssPersonAddress.addr_zip_4_code = _lobjcdoWebServiceAddressResult.addr_zip_4_code;
            return _lobjcdoWebServiceAddressResult.address_validate_flag;
        }

        private void CreatePersonEmploymentDetails(int aintPersonEmploymentID)
        {
            busPersonEmploymentDetail lobjEmpDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            lobjEmpDetail.icdoPersonEmploymentDetail.person_employment_id = aintPersonEmploymentID;
            lobjEmpDetail.icdoPersonEmploymentDetail.start_date = icdoWssPersonEmployment.start_date;
            lobjEmpDetail.icdoPersonEmploymentDetail.type_value = icdoWssPersonEmploymentDetail.type_value;
            lobjEmpDetail.icdoPersonEmploymentDetail.job_class_value = icdoWssPersonEmploymentDetail.job_class_value;
            lobjEmpDetail.icdoPersonEmploymentDetail.term_begin_date = icdoWssPersonEmploymentDetail.term_begin_date;
            lobjEmpDetail.icdoPersonEmploymentDetail.official_list_value = icdoWssPersonEmploymentDetail.official_list_value;
            lobjEmpDetail.icdoPersonEmploymentDetail.seasonal_value = icdoWssPersonEmploymentDetail.seasonal_value;
            lobjEmpDetail.icdoPersonEmploymentDetail.hourly_value = icdoWssPersonEmploymentDetail.hourly_value;
            lobjEmpDetail.icdoPersonEmploymentDetail.status_value = icdoWssPersonEmploymentDetail.employment_status_value;//busConstant.EmploymentStatusContributing;
            lobjEmpDetail.icdoPersonEmploymentDetail.Insert();
            icdoWssPersonEmploymentDetail.person_employment_dtl_id = lobjEmpDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            icdoWssPersonEmploymentDetail.Update();

            // PIR 9802
            if (lobjEmpDetail.ibusPersonEmployment.IsNull()) lobjEmpDetail.LoadPersonEmployment();
            if (lobjEmpDetail.ibusPersonEmployment.ibusOrganization.IsNull()) lobjEmpDetail.ibusPersonEmployment.LoadOrganization();
            lobjEmpDetail.InsertEmployerOfferedPlans();
        }

        public ArrayList CreatePersonEmployment()
        {
            ArrayList larrMemberRecordRequest = new ArrayList();
            //before creating new employment need to check whether there is already an existing open employment
            DataTable ldtPersonEmployment = Select<cdoPersonEmployment>(new string[2] { "person_id", "start_date" },
                                                                        new object[2] { ibusPerson.icdoPerson.person_id, icdoWssPersonEmployment.start_date },
                                                                        null, null);

            DataTable ldtPersonEmploymentforPerson = Select<cdoPersonEmployment>(new string[2] { "person_id", "end_date" },
                                                                        new object[2] { ibusPerson.icdoPerson.person_id, DateTime.MinValue },
                                                                        null, null);
            if (ldtPersonEmployment.Rows.Count == 0 && (icdoWssMemberRecordRequest.ps_initiated_flag == busConstant.Flag_No || !icdoWssMemberRecordRequest.ps_initiated_flag.IsNotNullOrEmpty()))
            {
                busPersonEmployment lobjPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                lobjPersonEmployment.icdoPersonEmployment.person_id = ibusPerson.icdoPerson.person_id;
                lobjPersonEmployment.icdoPersonEmployment.org_id = icdoWssPersonEmployment.org_id;
                lobjPersonEmployment.icdoPersonEmployment.start_date = icdoWssPersonEmployment.start_date;
                if (lobjPersonEmployment.ibusPerson.IsNull()) lobjPersonEmployment.LoadPerson();
                lobjPersonEmployment.InitiateWorkFlowForPayeeAccount(); //PIR 12393 & 12148
                lobjPersonEmployment.icdoPersonEmployment.Insert();
                CreatePersonEmploymentDetails(lobjPersonEmployment.icdoPersonEmployment.person_employment_id);
            }
            //PIR-11030 Start
            //Need Confirmation from Karthik on this else block
            else
            {
                if (icdoWssMemberRecordRequest.ps_initiated_flag == busConstant.Flag_Yes)
                {
                    if (ldtPersonEmploymentforPerson.Rows.Count > 1) //More than one Open employment
                    {
                        Collection<busPersonEmployment> lclbPersonEmployment1 = GetCollection<busPersonEmployment>(ldtPersonEmploymentforPerson, "icdoPersonEmployment");
                        Collection<busPersonEmployment> lclbPersonEmploymentOrg = lclbPersonEmployment1.Where(o => o.icdoPersonEmployment.org_id == icdoWssPersonEmployment.org_id).ToList().ToCollection();
                        if (lclbPersonEmploymentOrg.Count > 0) //Same organization 
                        {
                            busPersonEmployment lbusEmp = lclbPersonEmploymentOrg.Where(o => o.icdoPersonEmployment.ps_empl_record_number != icdoWssPersonEmployment.ps_empl_record_number).FirstOrDefault();
                            if (lbusEmp != null) //Same Organization Different Record NUmber 
                            {
                                busPersonEmployment lbusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                                lbusPersonEmployment.icdoPersonEmployment.person_id = ibusPerson.icdoPerson.person_id;
                                lbusPersonEmployment.icdoPersonEmployment.org_id = icdoWssPersonEmployment.org_id;
                                lbusPersonEmployment.icdoPersonEmployment.start_date = icdoWssPersonEmployment.start_date;
                                lbusPersonEmployment.icdoPersonEmployment.ps_empl_record_number = icdoWssPersonEmployment.ps_empl_record_number;
                                if (lbusPersonEmployment.ibusPerson.IsNull()) lbusPersonEmployment.LoadPerson();
                                lbusPersonEmployment.InitiateWorkFlowForPayeeAccount(); //PIR 12393 & 12148
                                lbusPersonEmployment.icdoPersonEmployment.Insert();
                                //Need Confirmation from Karthik on this
                                CreatePersonEmploymentDetails(lbusPersonEmployment.icdoPersonEmployment.person_employment_id);
                            }

                        }
                        else
                        {
                            busPersonEmployment lbusPersonEmp = lclbPersonEmployment1.Where(o => o.icdoPersonEmployment.ps_empl_record_number == icdoWssPersonEmployment.ps_empl_record_number).FirstOrDefault();
                            if (lbusPersonEmp != null) //Different Organization  Same Record number
                            {
                                lbusPersonEmp.icdoPersonEmployment.end_date = icdoWssPersonEmployment.start_date.AddDays(-1); //lobjWssPersonEmployment.icdoWssPersonEmployment.start_date.AddDays(-1)
                                lbusPersonEmp.icdoPersonEmployment.ienuObjectState = ObjectState.Update;
                                lbusPersonEmp.LoadOtherEmployment();
                                lbusPersonEmp.LoadOrganization();
                                lbusPersonEmp.icdoPersonEmployment.istrOrgCodeID = lbusPersonEmp.ibusOrganization.icdoOrganization.org_code;
                                lbusPersonEmp.ValidateHardErrors(utlPageMode.Update);
                                if (lbusPersonEmp.iarrErrors.Count > 0)
                                {
                                    foreach (utlError lobjError in lbusPersonEmp.iarrErrors)
                                    {
                                        larrMemberRecordRequest.Add(lobjError);
                                    }
                                    return larrMemberRecordRequest;

                                }
                                else
                                {
                                    lbusPersonEmp.BeforePersistChanges();
                                    lbusPersonEmp.PersistChanges();
                                    lbusPersonEmp.AfterPersistChanges();
                                }
                                //lobjPersonEmployment.icdoPersonEmployment.Update();
                                busPersonEmployment lbusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                                lbusPersonEmployment.icdoPersonEmployment.person_id = ibusPerson.icdoPerson.person_id;
                                lbusPersonEmployment.icdoPersonEmployment.org_id = icdoWssPersonEmployment.org_id;
                                lbusPersonEmployment.icdoPersonEmployment.start_date = icdoWssPersonEmployment.start_date;
                                lbusPersonEmployment.icdoPersonEmployment.ps_empl_record_number = icdoWssPersonEmployment.ps_empl_record_number;
                                lbusPersonEmployment.icdoPersonEmployment.Insert();
                                CreatePersonEmploymentDetails(lbusPersonEmployment.icdoPersonEmployment.person_employment_id);
                            }
                            else
                            {
                                busPersonEmployment lbusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                                lbusPersonEmployment.icdoPersonEmployment.person_id = ibusPerson.icdoPerson.person_id;
                                lbusPersonEmployment.icdoPersonEmployment.org_id = icdoWssPersonEmployment.org_id;
                                lbusPersonEmployment.icdoPersonEmployment.start_date = icdoWssPersonEmployment.start_date;
                                lbusPersonEmployment.icdoPersonEmployment.ps_empl_record_number = icdoWssPersonEmployment.ps_empl_record_number;
                                if (lbusPersonEmployment.ibusPerson.IsNull()) lbusPersonEmployment.LoadPerson();
                                lbusPersonEmployment.InitiateWorkFlowForPayeeAccount(); //PIR 12393 & 12148
                                lbusPersonEmployment.icdoPersonEmployment.Insert();
                                //Need Confirmation from Karthik on this
                                CreatePersonEmploymentDetails(lbusPersonEmployment.icdoPersonEmployment.person_employment_id);
                            }

                        }
                    }
                    else if (ldtPersonEmploymentforPerson.Rows.Count == 1) //One Open employment
                    {
                        //Collection<busPsEmployment> lclbPsEmployment = this.LoadPsEmploymentBySSN(icdoWssMemberRecordRequest.ssn);
                        Collection<busPersonEmployment> lclbPersonEmployment = GetCollection<busPersonEmployment>(ldtPersonEmploymentforPerson, "icdoPersonEmployment");
                        //busWssPersonEmployment lobjWssPersonEmployment = LoadWssPersonEmployment(icdoWssMemberRecordRequest.member_record_request_id);
                        foreach (busPersonEmployment lobjPersonEmployment in lclbPersonEmployment)
                        {

                            //If existing employment is Org 019200 with EmplRecordNumber 1 and new employment is Org 011200 with EmplRecordNumber 1;
                            //then end date employment with 019200 and start 011200
                            if (icdoWssPersonEmployment.ps_empl_record_number == lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number)
                            {
                                //Same Record Number different organization 
                                if (lobjPersonEmployment.icdoPersonEmployment.org_id != icdoWssPersonEmployment.org_id)
                                {
                                    //Current employment has to be ended and the New Employment has to be started.
                                    lobjPersonEmployment.LoadPersonEmploymentDetail(false);
                                    foreach (busPersonEmploymentDetail lobjPersonEmploymentDtl in lobjPersonEmployment.icolPersonEmploymentDetail)
                                    {
                                        if (lobjPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date == DateTime.MinValue)
                                        {
                                            if (lobjPersonEmploymentDtl.icdoPersonEmploymentDetail.start_date == icdoWssPersonEmployment.start_date)
                                            {
                                                lobjPersonEmployment.icdoPersonEmployment.end_date = icdoWssPersonEmployment.start_date;
                                            }
                                            else
                                            {
                                                lobjPersonEmployment.icdoPersonEmployment.end_date = icdoWssPersonEmployment.start_date.AddDays(-1);
                                            }
                                            break;
                                        }
                                    }
                                    lobjPersonEmployment.icdoPersonEmployment.ienuObjectState = ObjectState.Update;
                                    lobjPersonEmployment.LoadOtherEmployment();
                                    lobjPersonEmployment.LoadOrganization();
                                    lobjPersonEmployment.icdoPersonEmployment.istrOrgCodeID = lobjPersonEmployment.ibusOrganization.icdoOrganization.org_code;
                                    lobjPersonEmployment.ValidateHardErrors(utlPageMode.Update);
                                    if (lobjPersonEmployment.iarrErrors.Count > 0)
                                    {
                                        foreach (utlError lobjError in lobjPersonEmployment.iarrErrors)
                                        {
                                            larrMemberRecordRequest.Add(lobjError);
                                        }
                                        return larrMemberRecordRequest;

                                    }
                                    else
                                    {
                                        lobjPersonEmployment.BeforePersistChanges();
                                        lobjPersonEmployment.PersistChanges();
                                        lobjPersonEmployment.AfterPersistChanges();
                                    }
                                    busPersonEmployment lbusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                                    lbusPersonEmployment.icdoPersonEmployment.person_id = ibusPerson.icdoPerson.person_id;
                                    lbusPersonEmployment.icdoPersonEmployment.org_id = icdoWssPersonEmployment.org_id;
                                    lbusPersonEmployment.icdoPersonEmployment.start_date = icdoWssPersonEmployment.start_date;
                                    lbusPersonEmployment.icdoPersonEmployment.ps_empl_record_number = icdoWssPersonEmployment.ps_empl_record_number;
                                    lbusPersonEmployment.icdoPersonEmployment.Insert();
                                    CreatePersonEmploymentDetails(lbusPersonEmployment.icdoPersonEmployment.person_employment_id);
                                }
                                else //Same Record Number Same Organization 
                                {
                                    //Update the
                                }
                            }
                            else
                            {
                                //Different Record Number Different Organization OR Different Record Number and Same Organization

                                //If existing employment is Org 019200 with EmplRecordNumber 1 and new employment is Org 011200 with EmplRecordNumber 2; then DO NOT end date employment with 019200 
                                //and create employment with 011200.  
                                //Do not end the current employment and start the new employment with new org id 
                                if (lobjPersonEmployment.icdoPersonEmployment.org_id != icdoWssPersonEmployment.org_id)
                                {
                                    //DoubtFull About this as I suppose the We just have create the Employment details for the reocrd where ps_empl_record_number is not equal to existing ps_empl_record_number
                                    // and organizations are different.
                                    busPersonEmployment lbusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                                    lbusPersonEmployment.icdoPersonEmployment.person_id = ibusPerson.icdoPerson.person_id;
                                    lbusPersonEmployment.icdoPersonEmployment.org_id = icdoWssPersonEmployment.org_id;
                                    lbusPersonEmployment.icdoPersonEmployment.start_date = icdoWssPersonEmployment.start_date;
                                    lbusPersonEmployment.icdoPersonEmployment.ps_empl_record_number = icdoWssPersonEmployment.ps_empl_record_number;
                                    if (lobjPersonEmployment.ibusPerson.IsNull()) lobjPersonEmployment.LoadPerson();
                                    lobjPersonEmployment.InitiateWorkFlowForPayeeAccount(); //PIR 12393 & 12148
                                    lbusPersonEmployment.icdoPersonEmployment.Insert();
                                    //Need Confirmation from Karthik on this
                                    CreatePersonEmploymentDetails(lbusPersonEmployment.icdoPersonEmployment.person_employment_id);
                                }
                                else  //Different Record Number Same Organization
                                {
                                    //Commented 19 September 
                                    //lobjPersonEmployment.icdoPersonEmployment.end_date = icdoWssPersonEmployment.start_date.AddDays(-1);
                                    //lobjPersonEmployment.icdoPersonEmployment.ienuObjectState = ObjectState.Update;
                                    //lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number = icdoWssPersonEmployment.ps_empl_record_number;
                                    //lobjPersonEmployment.LoadOtherEmployment();
                                    //lobjPersonEmployment.LoadOrganization();
                                    //lobjPersonEmployment.icdoPersonEmployment.istrOrgCodeID = lobjPersonEmployment.ibusOrganization.icdoOrganization.org_code;
                                    //lobjPersonEmployment.ValidateHardErrors(utlPageMode.Update);
                                    //if (lobjPersonEmployment.iarrErrors.Count > 0)
                                    //{
                                    //    foreach (object lobjError in lobjPersonEmployment.iarrErrors)
                                    //    {
                                    //        larrMemberRecordRequest.Add(lobjError);
                                    //    }
                                    //    return larrMemberRecordRequest;
                                    //}
                                    //else
                                    //{
                                    //    lobjPersonEmployment.BeforePersistChanges();
                                    //    lobjPersonEmployment.PersistChanges();
                                    //    lobjPersonEmployment.AfterPersistChanges();
                                    //}
                                    busPersonEmployment lbusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                                    lbusPersonEmployment.icdoPersonEmployment.person_id = ibusPerson.icdoPerson.person_id;
                                    lbusPersonEmployment.icdoPersonEmployment.org_id = icdoWssPersonEmployment.org_id;
                                    lbusPersonEmployment.icdoPersonEmployment.start_date = icdoWssPersonEmployment.start_date;
                                    lbusPersonEmployment.icdoPersonEmployment.ps_empl_record_number = icdoWssPersonEmployment.ps_empl_record_number;
                                    if (lobjPersonEmployment.ibusPerson.IsNull()) lobjPersonEmployment.LoadPerson();
                                    lobjPersonEmployment.InitiateWorkFlowForPayeeAccount();
                                    lbusPersonEmployment.icdoPersonEmployment.Insert();
                                    //Need Confirmation from Karthik on this
                                    CreatePersonEmploymentDetails(lbusPersonEmployment.icdoPersonEmployment.person_employment_id);
                                }
                            }
                        }
                    }
                    else //No open employments scenario
                    {
                        busPersonEmployment lobjPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                        lobjPersonEmployment.icdoPersonEmployment.person_id = ibusPerson.icdoPerson.person_id;
                        lobjPersonEmployment.icdoPersonEmployment.org_id = icdoWssPersonEmployment.org_id;
                        lobjPersonEmployment.icdoPersonEmployment.start_date = icdoWssPersonEmployment.start_date;
                        lobjPersonEmployment.icdoPersonEmployment.ps_empl_record_number = icdoWssPersonEmployment.ps_empl_record_number;
                        if (lobjPersonEmployment.ibusPerson.IsNull()) lobjPersonEmployment.LoadPerson();
                        lobjPersonEmployment.InitiateWorkFlowForPayeeAccount(); //PIR 12393 & 12148
                        lobjPersonEmployment.icdoPersonEmployment.Insert();
                        CreatePersonEmploymentDetails(lobjPersonEmployment.icdoPersonEmployment.person_employment_id);
                    }
                }

            }
            //PIR-11030 End
            return larrMemberRecordRequest;
        }
        public busWssPersonEmployment ibusWssPersonEmployment { get; set; }

        private busWssPersonEmployment LoadWssPersonEmployment(int requestId)
        {
            ibusWssPersonEmployment = new busWssPersonEmployment() { icdoWssPersonEmployment = new cdoWssPersonEmployment() };
            DataTable ldtWssPersonEmployment = Select<cdoWssPersonEmployment>(new string[1] { enmWssPersonEmployment.member_record_request_id.ToString() },
                                                                                new object[1] { icdoWssMemberRecordRequest.member_record_request_id }, null, null);
            Collection<busWssPersonEmployment> lclbWSSPersonEmployment = GetCollection<busWssPersonEmployment>(ldtWssPersonEmployment, "icdoWssPersonEmployment");
            return lclbWSSPersonEmployment[0];
        }



        private void CreatePersonContact()
        {
            //PIR 13985 if contact is married then only we create spouse contact.
            if (!string.IsNullOrEmpty(ibusPerson.icdoPerson.marital_status_value) && ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
            {
                busPersonContact lobjContact = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
                lobjContact.icdoPersonContact.person_id = ibusPerson.icdoPerson.person_id;

                //PIR 13985 if contact is married then set it as spouse contact.
                icdoWssPersonContact.relationship_value = busConstant.PersonContactTypeSpouse;

                if (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried && icdoWssPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                    lobjContact.icdoPersonContact.relationship_value = icdoWssPersonContact.relationship_value;
                else
                    lobjContact.icdoPersonContact.relationship_value = busConstant.PersonContactTypePrimaryContact;

                lobjContact.icdoPersonContact.same_as_member_address = icdoWssPersonContact.same_as_member_address;
                lobjContact.icdoPersonContact.address_line_1 = icdoWssPersonContact.address_line_1;
                lobjContact.icdoPersonContact.address_line_2 = icdoWssPersonContact.address_line_2;
                lobjContact.icdoPersonContact.address_city = icdoWssPersonContact.address_city;
                lobjContact.icdoPersonContact.address_state_value = icdoWssPersonContact.address_state_value;
                lobjContact.icdoPersonContact.address_country_value = icdoWssPersonContact.address_country_value;
                lobjContact.icdoPersonContact.address_zip_code = icdoWssPersonContact.address_zip_code;
                lobjContact.icdoPersonContact.address_zip_4_code = icdoWssPersonContact.address_zip_4_code;
                lobjContact.icdoPersonContact.contact_phone_no = icdoWssPersonContact.contact_phone_no;

                if (ibusPersonContactPerson == null)
                    LoadPersonContactPerson();
                if (ibusPersonContactPerson.icdoPerson.person_id > 0)
                    lobjContact.icdoPersonContact.contact_person_id = ibusPersonContactPerson.icdoPerson.person_id;
                else
                {
                    // PIR 10071 - create spouse record
                    ibusPersonSpouse = new busPerson { icdoPerson = new cdoPerson() };
                    SetSpouseProperties(ibusPersonSpouse);
                    ibusPersonSpouse.icdoPerson.Insert();
                    lobjContact.icdoPersonContact.contact_person_id = ibusPersonSpouse.icdoPerson.person_id;
                    icdoWssPersonContact.contact_person_id = ibusPersonSpouse.icdoPerson.person_id;
                    icdoWssPersonContact.Update();
                }
                lobjContact.icdoPersonContact.status_value = busConstant.PersonContactStatusActive;
                lobjContact.BeforeValidate(utlPageMode.All);
                lobjContact.ValidateHardErrors(utlPageMode.All);// PROD PIR 7994
                if (lobjContact.iarrErrors.Count == 0)
                    lobjContact.icdoPersonContact.Insert();
                CreatePersonContactSpouse(lobjContact.icdoPersonContact.contact_person_id); // PIR 10071
            }
        }

        // PIR 10071
        private void CreatePersonContactSpouse(int spousePersonId)
        {
            busPersonContact lobjContact = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
            lobjContact.icdoPersonContact.person_id = spousePersonId;
            if (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried && icdoWssPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                lobjContact.icdoPersonContact.relationship_value = icdoWssPersonContact.relationship_value;
            else
                lobjContact.icdoPersonContact.relationship_value = busConstant.PersonContactTypePrimaryContact;

            lobjContact.icdoPersonContact.contact_person_id = ibusPerson.icdoPerson.person_id;

            lobjContact.icdoPersonContact.status_value = busConstant.PersonContactStatusActive;
            lobjContact.BeforeValidate(utlPageMode.All);
            lobjContact.ValidateHardErrors(utlPageMode.All);
            if (lobjContact.iarrErrors.Count == 0)
                lobjContact.icdoPersonContact.Insert();
        }

        public busPerson ibusPersonContactPerson { get; set; }

        private void LoadPersonContactPerson()
        {
            ibusPersonContactPerson = new busPerson { icdoPerson = new cdoPerson() };
            //PIR 14039 - Comparing DOB, Gender along with SSN to load person contact.
            DataTable ldtPerson = SelectWithOperator<cdoPerson>(new string[3] { enmPerson.ssn.ToString(), enmPerson.date_of_birth.ToString(),
                                                                                enmPerson.gender_value.ToString() },
                                                                new string[3] { "=", "=", "=" },
                                                                new object[3] { icdoWssPersonContact.contact_ssn, icdoWssPersonContact.contact_date_of_birth,
                                                                                 icdoWssPersonContact.contact_gender_value },
                                                                null);
            if (ldtPerson.Rows.Count > 0)
                ibusPersonContactPerson.icdoPerson.LoadData(ldtPerson.Rows[0]);
        }

        private void CreatePersonAddress()
        {
            bool iblnSameAddress = false;

            if (ibusPerson.iclbPersonAddress == null || icdoWssMemberRecordRequest.ps_initiated_flag == busConstant.Flag_Yes)
                ibusPerson.LoadPersonAddress();
            busPersonAddress lobjAddress = null;
            if (icdoWssMemberRecordRequest.ps_initiated_flag == busConstant.Flag_Yes)
                lobjAddress = ibusPerson.iclbPersonAddress.Where(o => o.icdoPersonAddress.address_type_value == busConstant.AddressTypePermanent &&
                                  busGlobalFunctions.CheckDateOverlapping(icdoWssPersonAddress.addr_effective_date, o.icdoPersonAddress.start_date,
                                  o.icdoPersonAddress.end_date == DateTime.MinValue ? icdoWssPersonAddress.addr_effective_date : o.icdoPersonAddress.end_date)).FirstOrDefault();
            else
                lobjAddress = ibusPerson.iclbPersonAddress.Where(o => o.icdoPersonAddress.address_type_value == busConstant.AddressTypePermanent &&
                                       busGlobalFunctions.CheckDateOverlapping(DateTime.Today, o.icdoPersonAddress.start_date,
                                       o.icdoPersonAddress.end_date == DateTime.MinValue ? DateTime.Now : o.icdoPersonAddress.end_date)).FirstOrDefault(); //existing condition



            if ((icdoWssMemberRecordRequest.ps_initiated_flag == busConstant.Flag_Yes && (icdoWssPersonAddress.address_updated_in_ps_batch == busConstant.Flag_No || icdoWssPersonAddress.address_updated_in_ps_batch == null || icdoWssPersonAddress.address_updated_in_ps_batch.Trim() == string.Empty)) // 7 October
                || icdoWssMemberRecordRequest.ps_initiated_flag == busConstant.Flag_No)
            {
                // PIR 11030 start - as per mail from Maik dated 03/07/2014
                ibusPerson.LoadPersonCurrentAddress();

                if (ibusPerson.ibusPersonCurrentAddress != null && ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress != null && ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.person_address_id > 0)
                {
                    if (ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_1.ToUpper() != icdoWssPersonAddress.addr_line_1.ToUpper() ||
                        (icdoWssPersonAddress.addr_line_2.IsNotNullOrEmpty() && ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_2.IsNotNullOrEmpty() ? ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_2.Trim().ToUpper() != icdoWssPersonAddress.addr_line_2.Trim().ToUpper() : false) ||
                        ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_city.ToUpper() != icdoWssPersonAddress.addr_city.ToUpper() ||
                        //ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_4_code != icdoWssPersonAddress.addr_zip_4_code || PIR 20276 PeopleSoft Inbound File - Address update
                        ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code != icdoWssPersonAddress.addr_zip_code ||
                        ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_value != icdoWssPersonAddress.addr_country_value ||
                        ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_state_value.ToUpper() != icdoWssPersonAddress.addr_state_value.ToUpper())
                    { // do nothing
                    }
                    else
                        iblnSameAddress = true;
                }
                // PIR 11030 end

                if (!iblnSameAddress)
                {
                    if (lobjAddress != null)
                    {
                        if (lobjAddress.icdoPersonAddress.start_date == DateTime.Today)
                            lobjAddress.icdoPersonAddress.end_date = DateTime.Today;
                        else if (icdoWssPersonAddress.addr_effective_date != DateTime.MinValue && (icdoWssPersonAddress.address_updated_in_ps_batch == busConstant.Flag_No || icdoWssPersonAddress.address_updated_in_ps_batch.IsNullOrEmpty() || icdoWssPersonAddress.address_updated_in_ps_batch == null || icdoWssPersonAddress.address_updated_in_ps_batch.Trim() == string.Empty))
                        {
                            if (lobjAddress.icdoPersonAddress.start_date.Date == icdoWssPersonAddress.addr_effective_date.Date)
                                lobjAddress.icdoPersonAddress.end_date = icdoWssPersonAddress.addr_effective_date;
                            else
                                lobjAddress.icdoPersonAddress.end_date = icdoWssPersonAddress.addr_effective_date.AddDays(-1);
                        }
                        else
                            lobjAddress.icdoPersonAddress.end_date = DateTime.Today.AddDays(-1);
                        if (icdoWssMemberRecordRequest.ps_initiated_flag == busConstant.Flag_Yes)
                            lobjAddress.icdoPersonAddress.peoplesoft_flag = busConstant.Flag_Yes; //PIR-11030 
                        lobjAddress.icdoPersonAddress.Update();
                    }
                    busPersonAddress lobjNewAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                    lobjNewAddress.icdoPersonAddress.person_id = ibusPerson.icdoPerson.person_id;
                    lobjNewAddress.icdoPersonAddress.addr_line_1 = icdoWssPersonAddress.addr_line_1;
                    lobjNewAddress.icdoPersonAddress.addr_line_2 = icdoWssPersonAddress.addr_line_2;
                    lobjNewAddress.icdoPersonAddress.addr_city = icdoWssPersonAddress.addr_city;
                    lobjNewAddress.icdoPersonAddress.addr_state_value = icdoWssPersonAddress.addr_state_value;
                    lobjNewAddress.icdoPersonAddress.addr_country_value = icdoWssPersonAddress.addr_country_value;
                    lobjNewAddress.icdoPersonAddress.addr_zip_code = icdoWssPersonAddress.addr_zip_code;
                    lobjNewAddress.icdoPersonAddress.addr_zip_4_code = icdoWssPersonAddress.addr_zip_4_code;
                    if (busGlobalFunctions.GetData1ByCodeValue(151, lobjNewAddress.icdoPersonAddress.addr_country_value, iobjPassInfo) != "US")
                    {
                        lobjNewAddress.icdoPersonAddress.foreign_postal_code = icdoWssPersonAddress.foreign_postal_code;
                        lobjNewAddress.icdoPersonAddress.foreign_province = icdoWssPersonAddress.foreign_province;
                    }
                    lobjNewAddress.icdoPersonAddress.address_type_value = icdoWssPersonAddress.address_type_value;

                    if (icdoWssMemberRecordRequest.ps_initiated_flag == busConstant.Flag_Yes)
                    {
                        lobjNewAddress.icdoPersonAddress.start_date = icdoWssPersonAddress.addr_effective_date;
                        lobjNewAddress.icdoPersonAddress.peoplesoft_flag = busConstant.Flag_Yes;
                    }
                    else
                    {
                        lobjNewAddress.icdoPersonAddress.start_date = DateTime.Today;
                    }
                    lobjNewAddress.icdoPersonAddress.Insert();
                }
            }
        }

        public busPerson ibusPerson { get; set; }

        public busPerson ibusPersonSpouse { get; set; } // PIR 10071

        public bool iblnIsFromPortal { get; set; } // PIR 13380
        public bool iblnIsFromESS { get; set; }

        private void CreatePersonDetails()
        {
            ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            int lintPersonID = 0;
            if (icdoWssMemberRecordRequest.person_id > 0)
                lintPersonID = icdoWssMemberRecordRequest.person_id;
            else
                lintPersonID = IsSamePersonExists();
            if (lintPersonID > 0)
            {
                ibusPerson.FindPerson(lintPersonID);
                SetPersonProperties(ibusPerson);
                ibusPerson.icdoPerson.Update();
            }
            else
            {
                SetPersonProperties(ibusPerson);
                ibusPerson.icdoPerson.Insert();
            }
        }

        public void SetPersonProperties(busPerson abusPerson)
        {
            abusPerson.icdoPerson.first_name = icdoWssMemberRecordRequest.first_name;
            abusPerson.icdoPerson.middle_name = icdoWssMemberRecordRequest.middle_name;
            abusPerson.icdoPerson.last_name = icdoWssMemberRecordRequest.last_name;
            abusPerson.icdoPerson.name_suffix_value = icdoWssMemberRecordRequest.name_suffix_value;
            abusPerson.icdoPerson.name_prefix_value = icdoWssMemberRecordRequest.name_prefix_value;
            abusPerson.icdoPerson.ssn = icdoWssMemberRecordRequest.ssn;
            abusPerson.icdoPerson.date_of_birth = icdoWssMemberRecordRequest.date_of_birth;
            abusPerson.icdoPerson.gender_value = icdoWssMemberRecordRequest.gender_value;
            //PIR 21454 system-generated letter for marital status changes doesn't generate in all instances //ms_change_batch_flag should update from MSS as well
            if (!(string.IsNullOrEmpty(abusPerson.icdoPerson.marital_status_value)) && abusPerson.icdoPerson.marital_status_value != icdoWssMemberRecordRequest.marital_status_value &&
                abusPerson.icdoPerson.date_of_death == DateTime.MinValue && abusPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusDivorced)//PIR 24927 - Update ms_change_batch_flag to Y if there is change in martital status. Skip members with DOD.
            {
                if (abusPerson.IsMember() || abusPerson.IsRetiree() || abusPerson.IsPayee())
                {
                    abusPerson.UpdateMSChangeBatchFlag();
                }
            }
            abusPerson.icdoPerson.marital_status_value = icdoWssMemberRecordRequest.marital_status_value;
            abusPerson.icdoPerson.home_phone_no = icdoWssMemberRecordRequest.home_phone_no;
            abusPerson.icdoPerson.work_phone_no = icdoWssMemberRecordRequest.work_phone_no;
            abusPerson.icdoPerson.work_phone_ext = icdoWssMemberRecordRequest.work_phone_ext;
            abusPerson.icdoPerson.cell_phone_no = icdoWssMemberRecordRequest.cell_phone_no;
			//PIR 23690
            if(icdoWssMemberRecordRequest.peoplesoft_id > 0)
                abusPerson.icdoPerson.peoplesoft_id = Convert.ToString(icdoWssMemberRecordRequest.peoplesoft_id); //As per mail from Maik dated 03/20/2014, Issue#3
            abusPerson.icdoPerson.email_address = icdoWssPersonContact.email_address;
            abusPerson.icdoPerson.welcome_batch_letter_sent_flag = busConstant.Flag_No; //PIR 9803
        }


        // PIR 10071
        public void SetSpouseProperties(busPerson abusPerson)
        {
            string[] spouseName = icdoWssPersonContact.contact_name.Split(' ');
            if (spouseName.IsNotNull())
            {
                abusPerson.icdoPerson.first_name = spouseName[0];
                if (spouseName.Count() == 2) // if name does not contain middle name
                {
                    abusPerson.icdoPerson.last_name = spouseName[1];
                }
                else if (spouseName.Count() > 2) // if name contains middle name
                {
                    abusPerson.icdoPerson.middle_name = spouseName[1];
                    abusPerson.icdoPerson.last_name = spouseName[2];
                }
            }
            abusPerson.icdoPerson.ssn = icdoWssPersonContact.contact_ssn;
            abusPerson.icdoPerson.date_of_birth = icdoWssPersonContact.contact_date_of_birth;
            abusPerson.icdoPerson.gender_value = icdoWssPersonContact.contact_gender_value;
            abusPerson.icdoPerson.marital_status_value = busConstant.PersonMaritalStatusMarried;
            abusPerson.icdoPerson.email_address = icdoWssPersonContact.email_address;
        }

        public ArrayList Reject_click()
        {
            string lstrPrioityValue = string.Empty;
            ArrayList larrReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (string.IsNullOrEmpty(icdoWssMemberRecordRequest.rejection_reason))
            {
                lobjError = new utlError();
                lobjError = AddError(8521, string.Empty);
                larrReturn.Add(lobjError);
            }
            else if (icdoWssMemberRecordRequest.rejection_reason.Length > 2000)
            {
                lobjError = new utlError();
                lobjError = AddError(8522, string.Empty);
                larrReturn.Add(lobjError);
            }
            else
            {
                //PROD PIR 4973 : posting message to ESS
                if (icdoWssMemberRecordRequest.contact_id > 0)
                {
                    busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(9, iobjPassInfo, ref lstrPrioityValue), icdoWssMemberRecordRequest.first_name + ' ' + icdoWssMemberRecordRequest.last_name, "@" + icdoWssMemberRecordRequest.member_record_request_id),//pir 8148
                       lstrPrioityValue, aintOrgID: icdoWssPersonEmployment.org_id,
                       aintContactID: icdoWssMemberRecordRequest.contact_id);
                }
                else
                {
                    //PIR 24101
                    busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(9, iobjPassInfo, ref lstrPrioityValue), icdoWssMemberRecordRequest.first_name + ' ' + icdoWssMemberRecordRequest.last_name, "@" + icdoWssMemberRecordRequest.member_record_request_id),//pir 8148
                              lstrPrioityValue, aintOrgID: icdoWssPersonEmployment.org_id,
                              aclbOrgContacts: busGlobalFunctions.LoadPrimaryAutOrAAByOrgId(icdoWssPersonEmployment.org_id));

                }
                icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusRejected;
                icdoWssMemberRecordRequest.Update();
                larrReturn.Add(this);
            }
            return larrReturn;
        }

        /// <summary>
        /// Valaidate Email
        /// </summary>		
        /// <returns>bool</returns>   
        public bool ValidateEmail()
        {
            if (!string.IsNullOrEmpty(icdoWssPersonContact.email_address))
            {
                return busGlobalFunctions.IsEmailValid(icdoWssPersonContact.email_address);  //18492
            }
            return true;
        }

        public string istrSuppressPersonDuplicateValidation { get; set; }

        // PROD PIR 4254
        public bool IsPersonDuplicated()
        {
            if (!iblnIsFromPS)
            {
                if ((icdoWssMemberRecordRequest.last_name.IsNotNullOrEmpty()) &&
                    (icdoWssMemberRecordRequest.date_of_birth != DateTime.MinValue) &&
                    (icdoWssMemberRecordRequest.gender_value.IsNotNullOrEmpty()) &&
                    (icdoWssMemberRecordRequest.ssn.IsNotNullOrEmpty()))
                {
                    busDuplicatePersonScreen lobjDuplicatePerson = new busDuplicatePersonScreen();
                    lobjDuplicatePerson.LoadDuplicatePersons(icdoWssMemberRecordRequest.person_id, icdoWssMemberRecordRequest.last_name,
                                                                icdoWssMemberRecordRequest.date_of_birth, icdoWssMemberRecordRequest.gender_value, 1, 0,
                                                                icdoWssMemberRecordRequest.ssn);
                    if (lobjDuplicatePerson.iclbDuplicatePersons.Count > 0)
                        return true;
                }
            }
            return false;
        }

        public void SetPerson()
        {
            if (ibusPerson.IsNull())
                ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            DataTable ldtPerson = SelectWithOperator<cdoPerson>(new string[2] { "ssn", "date_of_birth" },
                                                                new string[2] { "=", "=" },
                                                                new object[2] { icdoWssMemberRecordRequest.ssn, icdoWssMemberRecordRequest.date_of_birth },
                                                                null);
            if (ldtPerson.Rows.Count > 0)
            {
                ibusPerson.icdoPerson.LoadData(ldtPerson.Rows[0]);
                icdoWssMemberRecordRequest.person_id = ibusPerson.icdoPerson.person_id;
            }
        }

        public string istrSuppressContactPersonDuplicateValidation { get; set; }

        // PROD PIR 4254
        public bool IsContactPersonDuplicated()
        {
            if (!iblnIsFromPS)
            {
                if ((ibusContactPerson.icdoPerson.last_name.IsNotNullOrEmpty()) &&
                    (icdoWssPersonContact.contact_date_of_birth != DateTime.MinValue) &&
                    (icdoWssPersonContact.contact_gender_value.IsNotNullOrEmpty()) &&
                    (icdoWssPersonContact.contact_ssn.IsNotNullOrEmpty()))
                {
                    busDuplicatePersonScreen lobjDuplicatePerson = new busDuplicatePersonScreen();
                    lobjDuplicatePerson.LoadDuplicatePersons(icdoWssPersonContact.contact_person_id, ibusContactPerson.icdoPerson.last_name,
                                                                icdoWssPersonContact.contact_date_of_birth, icdoWssPersonContact.contact_gender_value, 1, 0,
                                                                icdoWssPersonContact.contact_ssn);
                    if (lobjDuplicatePerson.iclbDuplicatePersons.Count > 0)
                        return true;
                }
            }
            return false;
        }

        public void SetContactPerson()
        {
            if (ibusContactPerson.IsNull())
                ibusContactPerson = new busPerson { icdoPerson = new cdoPerson() };
            DataTable ldtPerson = SelectWithOperator<cdoPerson>(new string[2] { "ssn", "date_of_birth" },
                                                                new string[2] { "=", "=" },
                                                                new object[2] { icdoWssPersonContact.contact_ssn, icdoWssPersonContact.contact_date_of_birth },
                                                                null);
            if (ldtPerson.Rows.Count > 0)
            {
                ibusContactPerson.icdoPerson.LoadData(ldtPerson.Rows[0]);
                icdoWssPersonContact.contact_person_id = ibusContactPerson.icdoPerson.person_id;
            }
        }

        /// <summary>
        /// prod pir 4846 : to ignore requests
        /// </summary>
        /// <returns></returns>
        public ArrayList Ignore_click()
        {
            ArrayList larrReturn = new ArrayList();
            utlError lobjError = new utlError();
            icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusIgnored;
            icdoWssMemberRecordRequest.Update();
            this.EvaluateInitialLoadRules();
            larrReturn.Add(this);
            return larrReturn;
        }


        /// <summary>
        /// pir 7332 : SSN starting with more than 2 zeros is invalid
        /// </summary>
        /// <returns></returns>
        public bool IsSSNValid()
        {
            return (icdoWssMemberRecordRequest.ssn.StartsWith("000") || icdoWssMemberRecordRequest.ssn.Length < 9) ? false : true;
        }

        //PIR 11782 - SSN validation for Spouse.
        public bool IsSSNValidForContact()
        {
            if (icdoWssPersonContact.contact_ssn.IsNotNullOrEmpty())
                if (icdoWssPersonContact.contact_ssn.StartsWith("000") || icdoWssPersonContact.contact_ssn.Length < 9)
                    return false;
                else
                    return true;
            return true;
        }


        /// <summary>
        /// Match SSN with Reenter SSN
        /// </summary>
        /// <returns></returns>
        public bool IsSSNMatch()
        {
            if (!string.IsNullOrWhiteSpace(icdoWssMemberRecordRequest.ssn))
                return (icdoWssMemberRecordRequest.ssn.Equals(icdoWssMemberRecordRequest.ReenterSSN)) ? true : false;
            else
                return true;
        }

        /// <summary>
        /// Match SSN with Contact Reenter SSN
        /// </summary>
        /// <returns></returns>
        public bool IsContactSSNMatch()
        {
            if (!string.IsNullOrWhiteSpace(icdoWssPersonContact.contact_ssn))
                return (icdoWssPersonContact.contact_ssn.Equals(icdoWssPersonContact.ReenterContactSSN)) ? true : false;
            else
                return true;
        }



        /// <summary>
        /// pir 5465 : Checks if hire date is valid
        /// </summary>
        /// <returns></returns>
        public bool IsEmploymentStartDateValid()
        {
            return busGlobalFunctions.CheckEmploymentStartDate(icdoWssPersonEmployment.start_date);
        }

        /// <summary>
        /// pir 7952 : Checks Created_by contains Underscore character i.e.'_' or Not
        /// </summary>
        /// <returns>True or False</returns>
        public bool IsCreatedByContainsUnderScore()
        {
            if (!icdoWssMemberRecordRequest.created_by.Contains('_'))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// pir 7952 : Set details of Panel Requested By
        /// </summary>
        /// <returns></returns>
        public void SetRequestedByDetails()
        {
            if (icdoWssMemberRecordRequest.contact_id > 0)
            {
                iintContact_id = icdoWssMemberRecordRequest.contact_id;
                ibusContact.FindContact(iintContact_id);
                icdoWssMemberRecordRequest.Requested_By_Contact_id = ibusContact.icdoContact.contact_id.ToString();
                icdoWssMemberRecordRequest.Requested_By_Contact_Name = ibusContact.icdoContact.ContactName;
                icdoWssMemberRecordRequest.Requested_By_Contact_Phone_No = ibusContact.icdoContact.phone_no;
                icdoWssMemberRecordRequest.Requested_By_Contact_Email = ibusContact.icdoContact.email_address;
            }
            //if (IsCreatedByContainsUnderScore())
            //{
            //    iintContact_id = Convert.ToInt32(icdoWssMemberRecordRequest.created_by.Substring(icdoWssMemberRecordRequest.created_by.IndexOf('_') + 1));
            //    ibusContact.FindContact(iintContact_id);
            //    icdoWssMemberRecordRequest.Requested_By_Contact_id = ibusContact.icdoContact.contact_id.ToString();
            //    icdoWssMemberRecordRequest.Requested_By_Contact_Name = ibusContact.icdoContact.ContactName;
            //    icdoWssMemberRecordRequest.Requested_By_Contact_Phone_No = ibusContact.icdoContact.phone_no;
            //    icdoWssMemberRecordRequest.Requested_By_Contact_Email = ibusContact.icdoContact.email_address;
            //}
            //else
            //{
            //    busUser lbusUser = new busUser() { icdoUser = new cdoUser() };
            //    lbusUser.icdoUser.user_id = this.icdoWssMemberRecordRequest.created_by;
            //    if (lbusUser.FindUserByUserName(lbusUser.icdoUser.user_id))
            //    {
            //        icdoWssMemberRecordRequest.Requested_By_Contact_id = lbusUser.icdoUser.user_id;
            //        icdoWssMemberRecordRequest.Requested_By_Contact_Name = lbusUser.icdoUser.User_Name;
            //        icdoWssMemberRecordRequest.Requested_By_Contact_Email = lbusUser.icdoUser.email_address;
            //    }
            //}
        }

        public bool IsMemberHavingContributingEmployment()
        {
            bool lblnResullt = false;
            if (icdoWssMemberRecordRequest.person_id > 0)
            {
                //if (icdoWssPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                //{
                if (ibusPerson == null)
                    ibusPerson = LoadPersonBySSN(icdoWssMemberRecordRequest.ssn);
                if (ibusPerson.ibusCurrentEmployment == null)
                    ibusPerson.LoadCurrentEmployer();
                if (ibusPerson.ibusCurrentEmployment.icdoPersonEmployment.person_employment_id > 0 && icdoWssPersonEmployment.ps_empl_record_number != ibusPerson.ibusCurrentEmployment.icdoPersonEmployment.ps_empl_record_number)
                {

                    if (ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail == null)
                        ibusPerson.ibusCurrentEmployment.LoadLatestPersonEmploymentDetail();
                    if (ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing) //As per mail from Maik dated 03/21/2014, Issue#2
                    {
                        iblnIsMemberContributing = true;
                        lblnResullt = true;
                    }
                    else
                        lblnResullt = false;
                }
                //}
                //else
                //{
                //    if (ibusPerson == null)
                //        ibusPerson = LoadPersonBySSN(icdoWssMemberRecordRequest.ssn);
                //    if (ibusPerson.ibusCurrentEmployment == null)
                //        ibusPerson.LoadCurrentEmployer();
                //    if (ibusPerson.ibusCurrentEmployment.icdoPersonEmployment.person_employment_id > 0 && icdoWssPersonEmployment.ps_empl_record_number != ibusPerson.ibusCurrentEmployment.icdoPersonEmployment.ps_empl_record_number)
                //    {

                //        if (ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail == null)
                //            ibusPerson.ibusCurrentEmployment.LoadLatestPersonEmploymentDetail();
                //        if (ibusPerson.ibusCurrentEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                //        {
                //            iblnIsMemberContributing = true;
                //            lblnResullt = true;
                //        }
                //        else
                //            lblnResullt = false;
                //    }
                //}
            }
            return lblnResullt;
        }

        //PIR 9867
        public bool IsAgeGreaterThanOrEqualToEighteen()
        {
            int lintYears, lintMonths;
            DateTime ldteFromDate = icdoWssMemberRecordRequest.date_of_birth;
            HelperFunction.GetMonthSpan(ldteFromDate, DateTime.Now, out lintYears, out lintMonths);
            if (lintYears >= 18)
                return true;
            else
                return false;
        }
        //PIR 9867
        public bool IsValidGender()
        {
            //if ((icdoWssPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
            //    || icdoWssPersonContact.relationship_value == busConstant.PersonContactTypeExSpouse)
            //    && icdoWssMemberRecordRequest.gender_value == icdoWssPersonContact.contact_gender_value)
            //    return false;
            //return true;

            //PIR  13211
            if (icdoWssMemberRecordRequest.gender_value == icdoWssPersonContact.contact_gender_value)
            {
                return false;
            }

            return true;
        }

        //PIR-11030 Start
        public busWssMemberRecordRequest LoadPendingMemberRecordRequestbySSN(string lstrssn)
        {
            DataTable ldtMemberRecordRequest = Select<cdoWssMemberRecordRequest>(new string[2] { enmWssMemberRecordRequest.ssn.ToString(), enmWssMemberRecordRequest.status_value.ToString() },
                                                                      new object[2] { lstrssn, busConstant.StatusReview }, null, null);
            if (ldtMemberRecordRequest.Rows.Count > 0)
            {
                iclbPendingMemberRecordRequest = GetCollection<busWssMemberRecordRequest>(ldtMemberRecordRequest, "icdoWssMemberRecordRequest");
                return iclbPendingMemberRecordRequest[0];
            }
            else
                return null;
        }
        public busWssMemberRecordRequest LoadProcessedMemberRecordRequestbySSN(string lstrssn)
        {
            DataTable ldtMemberRecordRequest = Select<cdoWssMemberRecordRequest>(new string[2] { enmWssMemberRecordRequest.ssn.ToString(), enmWssMemberRecordRequest.status_value.ToString() },
                                                                      new object[2] { lstrssn, busConstant.StatusProcessed }, null, null);
            if (ldtMemberRecordRequest.Rows.Count > 0)
            {
                iclbPendingMemberRecordRequest = GetCollection<busWssMemberRecordRequest>(ldtMemberRecordRequest, "icdoWssMemberRecordRequest");
                return iclbPendingMemberRecordRequest[0];
            }
            else
                return null;
        }

        public busPerson LoadPersonBySSN(string astrssn)
        {
            Collection<busPerson> lclbPerson = new Collection<busPerson>();
            DataTable ldtbPerson = DBFunction.DBSelect("cdoPerson.GetPersonBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPerson.Rows.Count > 0)
            {
                lclbPerson = GetCollection<busPerson>(ldtbPerson, "icdoPerson");
                return lclbPerson[0];
            }
            else
                return null;

        }

        private busPsPerson LoadPSPersonBySSN(string astrssn)
        {
            Collection<busPsPerson> lclbPsPerson = new Collection<busPsPerson>();
            DataTable ldtbPsPerson = DBFunction.DBSelect("cdoPsPerson.GetPsPersonBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPsPerson.Rows.Count > 0)
            {
                lclbPsPerson = GetCollection<busPsPerson>(ldtbPsPerson, "icdoPsPerson");
                return lclbPsPerson[0];
            }
            return null;

        }
        private busPsAddress LoadPSPersonAddressBySSN(string astrssn)
        {
            Collection<busPsAddress> lclbPsPerson = new Collection<busPsAddress>();
            DataTable ldtbPsAddress = DBFunction.DBSelect("cdoPsAddress.GetPsAddressBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPsAddress.Rows.Count > 0)
                lclbPsPerson = GetCollection<busPsAddress>(ldtbPsAddress, "icdoPsAddress");
            return lclbPsPerson[0];

        }
        public Collection<busPsEmployment> LoadPsEmploymentBySSN(string astrssn)
        {
            Collection<busPsEmployment> lclbPsEmployment = new Collection<busPsEmployment>();
            DataTable ldtbPsEmployment = DBFunction.DBSelect("cdoPsEmployment.GetPsEmploymentBySSN", new string[1] { astrssn },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPsEmployment.Rows.Count > 0)
                lclbPsEmployment = GetCollection<busPsEmployment>(ldtbPsEmployment, "icdoPsEmployment");
            return lclbPsEmployment;
        }
        //PIR-11030 End

        // PIR 11883
        public void LoadWorkFlowRequest(int aintRefId)
        {
            if (iclbWorkflowRequest.IsNull()) iclbWorkflowRequest = new Collection<busSolBpmRequest>();
            DataTable ldtWorkflowRequest = Select<doBpmRequest>(new string[1] { "REFERENCE_ID" },
                                                                      new object[1] { aintRefId }, null, null);
            if (ldtWorkflowRequest.Rows.Count > 0)
            {
                iclbWorkflowRequest = GetCollection<busSolBpmRequest>(ldtWorkflowRequest, "icdoBpmRequest");
            }
        }

        // PIR 11883
        //venkat look to load case instances properly
        public void LoadProcessInstance(int aintProcessId, int aintRequestId)
        {
            DataTable ldtCaseInstance =
                busBase.Select("entWssMemberRecordRequest.LoadProcessInstance",
                                new object[2] {aintRequestId, aintProcessId});

            if (iclbProcessInstance.IsNull()) iclbProcessInstance = new Collection<busSolBpmCaseInstance>();
            if (ldtCaseInstance.Rows.Count > 0)
            {
                iclbProcessInstance = GetCollection<busSolBpmCaseInstance>(ldtCaseInstance, "icdoBpmCaseInstance");
            }
        }


        public bool iblnIsMemberContributing { get; set; }
        public void VerifyAddressUsingUSPS()
        {
            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = icdoWssPersonAddress.addr_line_1;
            _lobjcdoWebServiceAddress.addr_line_2 = icdoWssPersonAddress.addr_line_2;
            _lobjcdoWebServiceAddress.addr_city = icdoWssPersonAddress.addr_city;
            _lobjcdoWebServiceAddress.addr_state_value = icdoWssPersonAddress.addr_state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = icdoWssPersonAddress.addr_zip_code;
            _lobjcdoWebServiceAddress.addr_zip_4_code = icdoWssPersonAddress.addr_zip_4_code;

            cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
            if (_lobjcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
            {
                //ASSSIGN 
                icdoWssPersonAddress.addr_line_1 = _lobjcdoWebServiceAddressResult.addr_line_1;
                icdoWssPersonAddress.addr_line_2 = _lobjcdoWebServiceAddressResult.addr_line_2;
                icdoWssPersonAddress.addr_city = _lobjcdoWebServiceAddressResult.addr_city;
                icdoWssPersonAddress.addr_state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                icdoWssPersonAddress.addr_zip_code = _lobjcdoWebServiceAddressResult.addr_zip_code;
                icdoWssPersonAddress.addr_zip_4_code = _lobjcdoWebServiceAddressResult.addr_zip_4_code;
            }
            icdoWssPersonAddress.address_validate_error = _lobjcdoWebServiceAddressResult.address_validate_error;
            icdoWssPersonAddress.address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;

            //PIR 13380
            if (istrSuppressWarning == busConstant.Flag_Yes)
            {
                //address_validate_flag = busConstant.Flag_Yes;
                //ibusPersonCurrentAddress.icdoPersonAddress.address_validate_flag = "Y";

                icdoWssPersonAddress.address_validate_flag = "Y";
                return;
            }
        }

        //PIR 13380
        private string _istrSuppressWarning;

        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (iblnIsFromESS)
            {
                VerifyAddressUsingUSPS();
            }
            base.BeforeValidate(aenmPageMode);
        }

        //PIR 13380
        /// <summary>
        /// Phone Number Validation
        /// </summary>
        /// <returns></returns>
        public Boolean VerifyContactPhoneNumber()
        {
            Boolean bIsPhoneValid = false;
            if (icdoWssMemberRecordRequest != null)
            {
                if (icdoWssMemberRecordRequest.home_phone_no != null)
                {
                    if (icdoWssMemberRecordRequest.home_phone_no.Length == 10)
                    {
                        bIsPhoneValid = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (icdoWssMemberRecordRequest.cell_phone_no != null)
                {
                    if (icdoWssMemberRecordRequest.cell_phone_no.Length == 10)
                    {
                        bIsPhoneValid = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (icdoWssMemberRecordRequest.work_phone_no != null)
                {
                    if (icdoWssMemberRecordRequest.work_phone_no.Length == 10)
                    {
                        bIsPhoneValid = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return bIsPhoneValid;
        }

        // PIR 14802
        public bool IsMemberHavingOpenEmploymentForSameOrg()
        {
            DataTable ldtOpenPersonEmployment = Select<cdoPersonEmployment>(new string[3] { "person_id", "end_date", "org_id" },
                                                                        new object[3] { icdoWssMemberRecordRequest.person_id, DateTime.MinValue, icdoWssPersonEmployment.org_id },
                                                                        null, null);
            return (ldtOpenPersonEmployment.Rows.Count > 0) ? true : false;
        }

        // PIR 17572
        public bool IsDuplicatePerson()
        {
            busPerson lbusDuplicatePerson = new busPerson();
            lbusDuplicatePerson = lbusDuplicatePerson.LoadPersonBySsn(icdoWssMemberRecordRequest.ssn);
            return (lbusDuplicatePerson.IsNotNull() &&
                (lbusDuplicatePerson.icdoPerson.date_of_birth != icdoWssMemberRecordRequest.date_of_birth
                || lbusDuplicatePerson.icdoPerson.gender_value != icdoWssMemberRecordRequest.gender_value));
        }

        public bool IsInvalidSSN()
        {
            busPerson lobjInvalidSSN = new busPerson();
            return (lobjInvalidSSN.LoadInvalidSSN(icdoWssMemberRecordRequest.ssn));
        }

        public bool IsDuplicateContact()
        {
            busPerson lbusDuplicatePerson = new busPerson();
            lbusDuplicatePerson = lbusDuplicatePerson.LoadPersonBySsn(icdoWssPersonContact.contact_ssn);
            return (lbusDuplicatePerson.IsNotNull() &&
                (lbusDuplicatePerson.icdoPerson.date_of_birth != icdoWssPersonContact.contact_date_of_birth
                || lbusDuplicatePerson.icdoPerson.gender_value != icdoWssPersonContact.contact_gender_value));
        }

        public bool IsInvalidContactSSN()
        {
            busPerson lobjInvalidSSN = new busPerson();
            return (lobjInvalidSSN.LoadInvalidSSN(icdoWssPersonContact.contact_ssn));
        }
        //PIR 14474 - Land the user up on summary page when wizard opened in update mode.
        public override void ProcessWizardData(utlWizardNavigationEventArgs we, string astrWizardName, string astrWizardStepName)
        {
            we.istrNextStepID = (icdoWssMemberRecordRequest.member_record_request_id > 0 && astrWizardName == "wizMain") ? "wzsMemberSummary" : we.istrNextStepID;
            base.ProcessWizardData(we, astrWizardName, astrWizardStepName);
        }

        public void LoadRejectionDetails()
        {
            if (icdoWssMemberRecordRequest.status_value == busConstant.EmploymentChangeRequestStatusRejected)
            {
                DataTable ldtbUserInfo = iobjPassInfo.isrvDBCache.GetUserInfo(icdoWssMemberRecordRequest.modified_by);
                if (ldtbUserInfo?.Rows?.Count > 0)
                {
                    istrRejectedByName = ldtbUserInfo.Rows[0]["FIRST_NAME"] + " " + ldtbUserInfo.Rows[0]["LAST_NAME"];
                    idtmRejectedDate = icdoWssMemberRecordRequest.modified_date;
                }
            }
        }

        public bool IsPlanOffered(int aintPlanType)
        {
            DataTable ldtbPersonAccountEmploymentDetail = busBase.Select<cdoPersonAccountEmploymentDetail>(new string[1] { enmPersonAccountEmploymentDetail.person_employment_dtl_id.ToString() }, new object[1] { icdoWssPersonEmploymentDetail.person_employment_dtl_id }, null, null);
            Collection<busPersonAccountEmploymentDetail> lclbPersonAccountEmploymentDetail = GetCollection<busPersonAccountEmploymentDetail>(ldtbPersonAccountEmploymentDetail, "icdoPersonAccountEmploymentDetail");
            if (lclbPersonAccountEmploymentDetail.Count > 0)
            {
                foreach (busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail in lclbPersonAccountEmploymentDetail)
                {
                    lbusPersonAccountEmploymentDetail.LoadPlan();
                }
                ibusPerson.LoadPersonAccount();
                if (aintPlanType == busConstant.RetirementCategory)
                {
                    return (lclbPersonAccountEmploymentDetail.Where(pa => pa.ibusPlan.IsRetirementPlan()).Count() > 0) && icdoWssPersonEmploymentDetail.employment_status_value == busConstant.EmploymentStatusContributing;
                }
                else if (aintPlanType == busConstant.EAPCategory)
                {
                    if (lclbPersonAccountEmploymentDetail.Where(pa => pa.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdEAP).Count() > 0) return true;
                }
                else
                {
                    if (lclbPersonAccountEmploymentDetail.Where(pa => pa.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdGroupLife).Count() > 0) return true;
                }
            }
            return false;
        }

        public ArrayList btnCallDuplicatePersonScreen_Click()
        {
            ArrayList larrList = new ArrayList();
            //ValidateHardErrors(utlPageMode.All);
            larrList.Add(this);
            return larrList;
        }
    }
}
