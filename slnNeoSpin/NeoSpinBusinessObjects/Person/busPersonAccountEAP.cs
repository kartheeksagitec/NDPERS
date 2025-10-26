#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;

#endregion


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountEAP : busPersonAccount
    {
        //this is used for posting message in member portal.
        public bool iblnIsFromPortal { get; set; }

        private Collection<busPersonAccountEAPHistory> _iclbEAPHistory;
        public Collection<busPersonAccountEAPHistory> iclbEAPHistory
        {
            get { return _iclbEAPHistory; }
            set { _iclbEAPHistory = value; }
        }
        private decimal _idecMonthlyPremium;

        public decimal idecMonthlyPremium
        {
            get { return _idecMonthlyPremium; }
            set { _idecMonthlyPremium = value; }
        }

        private busPersonAccountEAPHistory _ibusHistory;
        public busPersonAccountEAPHistory ibusHistory
        {
            get
            {
                return _ibusHistory;
            }
            set
            {
                _ibusHistory = value;
            }
        }

        public DataTable idtbCachedEapRate { get; set; }
        public string istrAllowOverlapHistory { get; set; }

        public string istrAcceptAcknowledgement { get; set; }

        public int iintProviderOrgId { get; set; } //PIR 10269

        // PIR 9115 functionality enable/disable property
        public string istrIsPIR9115Enabled
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            }
        }

        //PIR 6961
        public string istrConfirmationText
        {
            get
            {
                string luserName = ibusPerson.icdoPerson.FullName;
                DateTime Now = DateTime.Now;
                DataTable ldtbListdtDTP = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='CONF'");
                busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                if (ldtbListdtDTP.Rows.Count > 0)
                    lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbListdtDTP.Rows[0]["acknowledgement_text"].ToString();
                string lstrConfimation = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, luserName, Now);
                return lstrConfimation;
            }
        }

        public void LoadEAPHistory(bool ablnLoadOtherObjects)
        {
            DataTable ldtbEAPHistories = Select("entPersonAccountEAPHistory.LoadEAPHistory", new object[1] { icdoPersonAccount.person_account_id });
                _iclbEAPHistory = GetCollection<busPersonAccountEAPHistory>(ldtbEAPHistories, "icdoPersonAccountEapHistory");

            if (ablnLoadOtherObjects)
            {
                foreach (busPersonAccountEAPHistory lobjHistory in _iclbEAPHistory)
                {
                    lobjHistory.LoadPersonAccount();
                    lobjHistory.LoadPlan();
                    lobjHistory.LoadProvider();
                }
            }
        }

        public void LoadEAPHistory()
        {
            LoadEAPHistory(true);
        }
        //UAT PIR: 971. Loading Providers for New Mode added.
        public void LoadEapProvidersNewMode()
        {
            iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
            if (ibusPerson.icolEnrolledPersonAccountEmploymentDetail == null)
                ibusPerson.LoadPersonAccountEmploymentDetailEnrolled();

            var lenuList = ibusPerson.icolEnrolledPersonAccountEmploymentDetail.Where(i => i.icdoPersonAccountEmploymentDetail.plan_id == icdoPersonAccount.plan_id);
            if (lenuList != null && lenuList.Count() > 0)
            {
                foreach (var lbusPAEmpDetail in lenuList)
                {
                    if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                        lbusPAEmpDetail.LoadPersonEmploymentDetail();
                    iclbEmploymentDetail.Add(lbusPAEmpDetail.ibusEmploymentDetail);
                }
            }
        }

        public Collection<cdoOrganization> LoadEapProviders()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccountForEAP();
            if (ibusPersonAccount.ibusPersonEmploymentDetail.IsNull())
                ibusPersonAccount.LoadPersonEmploymentDetail();
            if (ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            //UAT: PIR:970 Method moved to base Person Account and invoked from there
            Collection<cdoOrganization> lclbActiveEmployers = new Collection<cdoOrganization>();
            lclbActiveEmployers = LoadActiveProviders();
            //PIR 26656 display Provider Org dropdown with org_name and org_code
            foreach (cdoOrganization lcdoOrganization in lclbActiveEmployers)
            {
                lcdoOrganization.org_name = lcdoOrganization.org_name + " - " + lcdoOrganization.istrProviderOrgName;
            }
            if (iblnIsFromPortal)
            {
                if (lclbActiveEmployers.Count == 1)
                {
                    icdoPersonAccount.provider_org_id = lclbActiveEmployers[0].org_id;
                    iintProviderOrgId = icdoPersonAccount.provider_org_id; //PIR 10269
                }

            }
            return lclbActiveEmployers;
        }

        public int GetDefaultEAPProvider()
        {
            int lintProviderOrgID = 0;
            DataTable ldtbList = Select("cdoPersonAccount.GetEAPDefaultProvider", new object[1] { ibusOrgPlan.icdoOrgPlan.org_plan_id });

            if (ldtbList.Rows.Count > 0)
            {
                lintProviderOrgID = Convert.ToInt32(ldtbList.Rows[0]["provider_org_id"]);
            }
            return lintProviderOrgID;
        }

        public bool IsPlanEnrollmentDateValidForEAP()
        {
            bool IsPlanEnrollmentDateValid = false;
            if ((ibusPersonEmploymentDetail != null) && (ibusPersonEmploymentDetail.ibusPersonEmployment != null))
            {
                DateTime ldtEmploymentStartDate = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date;
                DateTime ldtPlanEnrollmentDate = new DateTime(ldtEmploymentStartDate.AddMonths(1).Year, ldtEmploymentStartDate.AddMonths(1).Month, 1);
                if (icdoPersonAccount.start_date != DateTime.MinValue)
                {
                    if (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeEAP)
                    {
                        if (icdoPersonAccount.start_date != ldtPlanEnrollmentDate)
                        {
                            IsPlanEnrollmentDateValid = true;
                        }
                    }
                }
            }
            return IsPlanEnrollmentDateValid;
        }

        public void InsertHistory()
        {
            cdoPersonAccountEapHistory lobjEAPHistory = new cdoPersonAccountEapHistory();
            lobjEAPHistory.person_account_id = icdoPersonAccount.person_account_id;
            lobjEAPHistory.start_date = icdoPersonAccount.history_change_date;
            lobjEAPHistory.provider_org_id = icdoPersonAccount.provider_org_id;
            lobjEAPHistory.plan_participation_status_id = icdoPersonAccount.plan_participation_status_id;
            // PIR 9115
            //PIR 17081
            //if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended) ||
            //    (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled))
            //{
            //    DBFunction.DBNonQuery("entPersonAccountEAPHistory.UpdateReportGeneratedFlag", new object[1] { ibusHistory.icdoPersonAccountEapHistory.person_account_eap_history_id },
            //                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //}
            lobjEAPHistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
            lobjEAPHistory.status_id = icdoPersonAccount.status_id;
            lobjEAPHistory.status_value = icdoPersonAccount.status_value;
            lobjEAPHistory.from_person_account_id = icdoPersonAccount.from_person_account_id;
            lobjEAPHistory.to_person_account_id = icdoPersonAccount.to_person_account_id;
            lobjEAPHistory.suppress_warnings_flag = icdoPersonAccount.suppress_warnings_flag;
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes) // PROD PIR 7079
                lobjEAPHistory.suppress_warnings_by = icdoPersonAccount.suppress_warnings_by;
            lobjEAPHistory.suppress_warnings_date = icdoPersonAccount.suppress_warnings_date;
            lobjEAPHistory.Insert();
        }

        public void ProcessHistory()
        {
            Collection<busPersonAccountEAPHistory> lclbDeletedEAPHistory = new Collection<busPersonAccountEAPHistory>();
            if ((icdoPersonAccount.status_value == "VALD") && (IsHistoryEntryRequired))
            {
                //Remove the Overlapping History
                if (iclbOverlappingHistory != null && iclbOverlappingHistory.Count > 0)
                {
                    foreach (busPersonAccountEAPHistory lbusEAPHistory in iclbOverlappingHistory)
                    {
                        lclbDeletedEAPHistory.Add(lbusEAPHistory);
                        lbusEAPHistory.Delete();
                    }
                    //PIR 23340
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedEAP))
                    {
                        bool lblnIsPersonAccountModified = false;
                        if (icdoPersonAccount.start_date > icdoPersonAccount.history_change_date)
                        {
                            icdoPersonAccount.start_date = icdoPersonAccount.history_change_date;
                            lblnIsPersonAccountModified = true;

                        }
                        if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                            icdoPersonAccount.end_date != DateTime.MinValue)
                        {
                            icdoPersonAccount.end_date = DateTime.MinValue;
                            lblnIsPersonAccountModified = true;
                        }

                        if (lblnIsPersonAccountModified)
                            icdoPersonAccount.Update();
                    }
                }


                if (_ibusHistory == null)
                    LoadPreviousHistory();

                //If the Current Record is Getting End Dated, We should not create New History Entry. 
                //We Just need to Update the Previous History Entry

                //If the History is already End Dated and the New Record is now removing End Date, Then 
                //We should not update the Previous History End Date. We Just need to Create the New History Record Only.

                if (ibusHistory.icdoPersonAccountEapHistory.person_account_eap_history_id > 0)
                {
                    if (!lclbDeletedEAPHistory.Any(i => i.icdoPersonAccountEapHistory.person_account_eap_history_id == ibusHistory.icdoPersonAccountEapHistory.person_account_eap_history_id))
                    {
                        if (ibusHistory.icdoPersonAccountEapHistory.start_date == icdoPersonAccount.history_change_date)
                        {
                            ibusHistory.icdoPersonAccountEapHistory.end_date = icdoPersonAccount.history_change_date;
                            // Set flag to 'Y' so that ESS Benefit Enrollment report will ignore these records
                            //ibusHistory.icdoPersonAccountEapHistory.is_enrollment_report_generated = busConstant.Flag_Yes;//PIR 17081
                        }
                        else
                        {
                            ibusHistory.icdoPersonAccountEapHistory.end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                        }
                        ibusHistory.icdoPersonAccountEapHistory.Update();

                        //PIR 1729 : Update the End Date with Previous History End Date if the Plan Participation Status is other than Enrolled
                        //Reset the End Date with MinValue if the plan participation status is enrolled
                        if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            icdoPersonAccount.end_date = DateTime.MinValue;
                        }
                        else
                        {
                            icdoPersonAccount.end_date = ibusHistory.icdoPersonAccountEapHistory.end_date;
                        }
                        icdoPersonAccount.Update();
                    }
                }

                //Always Insert New History Whenever HistoryEntry Required Flag is Set
                InsertHistory();
            }
        }

        // EAP Enrollment start date must be within 31 days from the employment start date .Else throw an Error
        public bool ValidateStartDate()
        {
            if (icdoPersonAccount.start_date > ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date.AddDays(31))
            {
                return true;
            }
            return false;
        }

        public void GetMonthlyPremium()
        {
            if (ibusProviderOrgPlan == null)
                LoadProviderOrgPlanByProviderOrgID(icdoPersonAccount.provider_org_id, icdoPersonAccount.current_plan_start_date_no_null);

            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();

            GetMonthlyPremium(ibusProviderOrgPlan.icdoOrgPlan.org_plan_id, idtPlanEffectiveDate);
        }

        public void GetMonthlyPremium(int aintOrgPlanID, DateTime adtEffectiveDate)
        {
            idecMonthlyPremium = busRateHelper.GetEAPPremiumAmount(aintOrgPlanID, adtEffectiveDate, idtbCachedEapRate, iobjPassInfo);
        }

        //End Date must be last day of month follwing employment end date
        public bool ValidateEndate()
        {
            if ((ibusPersonEmploymentDetail != null) && (ibusPersonEmploymentDetail.ibusPersonEmployment != null))
            {
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue)
                {
                    if (icdoPersonAccount.end_date != DateTime.MinValue)
                    {
                        DateTime EmploymentEndMonth = new DateTime(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date.Year,
                            ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date.Month, 1);
                        DateTime PAEndate = EmploymentEndMonth.AddMonths(1).AddDays(-1);

                        if (icdoPersonAccount.end_date != PAEndate)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public Collection<busPersonAccountEAPHistory> iclbOverlappingHistory { get; set; }
        private Collection<busPersonAccountEAPHistory> LoadOverlappingHistory()
        {
            if (iclbEAPHistory == null)
                LoadEAPHistory();
            Collection<busPersonAccountEAPHistory> lclbEAPHistory = new Collection<busPersonAccountEAPHistory>();
            var lenuList = iclbEAPHistory.Where(i => busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                i.icdoPersonAccountEapHistory.start_date, i.icdoPersonAccountEapHistory.end_date)
                || i.icdoPersonAccountEapHistory.start_date > icdoPersonAccount.history_change_date);
            foreach (busPersonAccountEAPHistory lobjHistory in lenuList)
            {
                if (lobjHistory.icdoPersonAccountEapHistory.start_date >= icdoPersonAccount.history_change_date)
                {
                    lclbEAPHistory.Add(lobjHistory);
                }
                else if (lobjHistory.icdoPersonAccountEapHistory.start_date == lobjHistory.icdoPersonAccountEapHistory.end_date)
                {
                    lclbEAPHistory.Add(lobjHistory);
                }
                else if (lobjHistory.icdoPersonAccountEapHistory.start_date != lobjHistory.icdoPersonAccountEapHistory.end_date)
                {
                    break;
                }

            }
            return lclbEAPHistory;
        }

        public bool IsMoreThanOneEnrolledInOverlapHistory()
        {
            if (istrAllowOverlapHistory == busConstant.Flag_Yes)
            {
                if (iclbOverlappingHistory != null)
                {
                    var lenuList = iclbOverlappingHistory.Where(i => i.icdoPersonAccountEapHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                        i.icdoPersonAccountEapHistory.start_date != i.icdoPersonAccountEapHistory.end_date);
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedEAP))
                    {
                        if ((lenuList != null) && (lenuList.Count() > 2))
                            return true;
                    }
                    else
                    {
                        if ((lenuList != null) && (lenuList.Count() > 1))
                            return true;
                    }
                }
            }
            return false;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (icdoPersonAccount.provider_org_id == 0)
                icdoPersonAccount.provider_org_id = iintProviderOrgId; //PIR 10269

            //PROD Logic changes. - Delete the Previous open History lines if the Allow Overlap Sets true
            bool lblnReloadPreviousHistory = false;
            iclbOverlappingHistory = new Collection<busPersonAccountEAPHistory>();
            if ((istrAllowOverlapHistory == busConstant.Flag_Yes) && (icdoPersonAccount.history_change_date != DateTime.MinValue))
            {
                //Reload the History Always...
                LoadEAPHistory();

                Collection<busPersonAccountEAPHistory> lclbOpenHistory = LoadOverlappingHistory();
                if (lclbOpenHistory.Count > 0)
                {
                    foreach (busPersonAccountEAPHistory lbusEAPHistory in lclbOpenHistory)
                    {
                        iclbEAPHistory.Remove(lbusEAPHistory);
                        iclbOverlappingHistory.Add(lbusEAPHistory);
                        lblnReloadPreviousHistory = true;
                    }
                }
            }

            if (lblnReloadPreviousHistory)
            {
                LoadPreviousHistory();
            }

            if (ibusHistory == null)
                LoadPreviousHistory();

            SetHistoryEntryRequiredOrNot();
            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                iblnIsNewMode = true;
                if (ibusPersonEmploymentDetail != null)
                    if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id != 0)
                    {
                        LoadOrgPlan();
                    }
            }

            //Reload Provider Org Plan
            LoadProviderOrgPlanByProviderOrgID(icdoPersonAccount.provider_org_id, icdoPersonAccount.current_plan_start_date_no_null);
            LoadPlanEffectiveDate();
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            icolPosNegEmployerPayrollDtl = null;
            icolPosNegIbsDetail = null;
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
            {
                icdoPersonAccount.suppress_warnings_by = iobjPassInfo.istrUserID;
            }
            else
            {
                icdoPersonAccount.suppress_warnings_by = string.Empty;
            }

            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                icdoPersonAccount.history_change_date = icdoPersonAccount.start_date;
                //UAT PIR 2373 people soft file changes
                //--Start--//
                icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                //--End--//
            }
            else
            {
                //UAT PIR 2373 people soft file changes
                //--Start--//
                if (IsHistoryEntryRequired && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                {
                    SetPersonAcccountForTeminationChange();
                }
                else
                {
                    SetPersonAccountForEnrollmentChange();
                }
                //--End--//
            }
            GetMonthlyPremium();

            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            ProcessHistory();
            LoadEAPHistory();
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                SetPersonAccountIDInPersonAccountEmploymentDetail();
            }
            LoadPreviousHistory();

            if ((IsHistoryEntryRequired) && (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid))
            {
                busPersonAccountEAPHistory lbusClosedHistory = null;
                busPersonAccountEAPHistory lbusAddedHistory = null;
                foreach (busPersonAccountEAPHistory lbusEAPHistory in iclbEAPHistory)
                {
                    if (lbusEAPHistory.icdoPersonAccountEapHistory.end_date > DateTime.MinValue)
                    {
                        lbusClosedHistory = lbusEAPHistory;
                        break;
                    }
                    else
                        lbusAddedHistory = lbusEAPHistory;
                }
                if ((lbusClosedHistory != null) &&
                    (lbusClosedHistory.icdoPersonAccountEapHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)) //PIR 2029
                {
                    CreateAdjustmentPayrollForEnrollmentHistoryClose(lbusClosedHistory, lbusAddedHistory.icdoPersonAccountEapHistory.plan_participation_status_value);
                }
                if ((lbusAddedHistory != null) &&
                    (lbusAddedHistory.icdoPersonAccountEapHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)) //PIR 2029
                {
                    CreateAdjustmentPayrollForEnrollmentHistoryAdd(lbusAddedHistory);
                }
            }

            if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                istrPreviousPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceSuspended) ||
                (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled &&
                istrPreviousPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceCancelled))
            {
                // ignore if already in review IBS/EmployerPayrollDetail  entries
                if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid)
                {
                    DBFunction.DBNonQuery("cdoIbsDetail.UpdateAllIBSDetailStatusToIgnoreAfterPlanSusorCan", new object[2] { icdoPersonAccount.person_account_id, icdoPersonAccount.current_plan_start_date },
                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    DBFunction.DBNonQuery("entEmployerPayrollHeader.UpdateAllEPDetailStatusToIgnoreAfterPlanSusorCan", new object[3] {
                                    icdoPersonAccount.person_id, icdoPersonAccount.plan_id, icdoPersonAccount.current_plan_start_date },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            }
            // UAT PIR ID 1077 - To refresh the screen values only if the Suppress flag is On.
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
                RefreshValues();
            LoadAllPersonEmploymentDetails();

            // PIR 9115
            if (istrIsPIR9115Enabled == busConstant.Flag_Yes)
            {
                DataTable ldtPersonEmploymentCount = DBFunction.DBSelect("cdoPersonEmployment.CountOfOpenEmployments",
                        new object[1] { ibusPerson.icdoPerson.person_id }
                        , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                // PIR 11309 - dual employment scenario
                if (ldtPersonEmploymentCount.Rows.Count > 1)
                {
                    int lintOrgId = 0;
                    Collection<busPersonEmployment> lclbPersonEmployment = new Collection<busPersonEmployment>();

                    lclbPersonEmployment = GetCollection<busPersonEmployment>(ldtPersonEmploymentCount, "icdoPersonEmployment");
                    lclbPersonEmployment = busGlobalFunctions.Sort<busPersonEmployment>("icdoPersonEmployment.start_date desc", lclbPersonEmployment);
                    foreach (busPersonEmployment lobjPersonEmployment in lclbPersonEmployment)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date, lobjPersonEmployment.icdoPersonEmployment.start_date,
                            lobjPersonEmployment.icdoPersonEmployment.end_date))
                        {
                            lintOrgId = lobjPersonEmployment.icdoPersonEmployment.org_id;
                            break;
                        }
                    }
                    busGlobalFunctions.PostESSMessage(lintOrgId, ibusPlan.icdoPlan.plan_id, iobjPassInfo);
                }
                else
                {
                    busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                    ibusPlan.icdoPlan.plan_id, iobjPassInfo);
                }
            }
            
            if (iblnIsFromPortal)
            {
                if (ibusPlan.IsNull())
                    LoadPlan();

                busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.plan_name),
                                                                 busConstant.WSS_MessageBoard_Priority_High, icdoPersonAccount.person_id);
            }
            //PIR 6961
            if (iblnIsFromPortal)
            {

                cdoWssPersonAccountEnrollmentRequest lcdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
                LoadPersonAccountEAP();
                LoadPersonAccountEmploymentDetails();
                lcdoWssPersonAccountEnrollmentRequest.person_id = ibusPersonAccountEAP.icdoPersonAccount.person_id;
                lcdoWssPersonAccountEnrollmentRequest.plan_id = ibusPersonAccountEAP.icdoPersonAccount.plan_id;
                lcdoWssPersonAccountEnrollmentRequest.target_person_account_id = ibusPersonAccountEAP.icdoPersonAccount.person_account_id;
                lcdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = iclbAccountEmploymentDetail.FirstOrDefault().icdoPersonAccountEmploymentDetail.person_employment_dtl_id;
                lcdoWssPersonAccountEnrollmentRequest.status_id = busConstant.MemberPortalEnrollmentRequestStatus;
                lcdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                lcdoWssPersonAccountEnrollmentRequest.enrollment_type_id = busConstant.MemberPortalEnrollmentType;
                lcdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.PlanCodeEAP;
                lcdoWssPersonAccountEnrollmentRequest.provider_org_id = icdoPersonAccount.provider_org_id;
                lcdoWssPersonAccountEnrollmentRequest.Insert();

                busWssPersonAccountEnrollmentRequestAck lobjWssPersonAccountEnrollmentRequestAck = new busWssPersonAccountEnrollmentRequestAck();
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck = new cdoWssPersonAccountEnrollmentRequestAck();

                DataTable ldtbListdtDTP = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='CONF'");
                int ack_id = 0;
                if (ldtbListdtDTP.Rows.Count > 0)
                    ack_id = Convert.ToInt32(ldtbListdtDTP.Rows[0]["acknowledgement_id"]);
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id = ack_id;
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrConfirmationText;
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.wss_person_account_enrollment_request_id =lcdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.Insert();

            }
            UpdateNewHireMemberRecordReQuestDetailsManOrAutoPlanEnrollment(busConstant.EAPCategory);
            if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && IsHistoryEntryRequired
                && icdoPersonAccount.iblnIsEmploymentEnded) //PIR 20259
                icdoPersonAccount.iblnIsEmploymentEnded = false;
            if ((icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid) && (IsHistoryEntryRequired)
                && !icdoPersonAccount.iblnIsEmploymentEnded) //PIR 20259
            {
                InsertIntoEnrollmentData();
            }
        }
        
        //PIR 17081
        public void InsertIntoEnrollmentData()
        {
            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();
            icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
            LoadPersonEmploymentDetail();
            ibusPersonEmploymentDetail.LoadPersonEmployment();

            busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
            lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

            busDailyPersonAccountPeopleSoft lobjDailyPAPeopleSoft = new busDailyPersonAccountPeopleSoft();
            lobjDailyPAPeopleSoft.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjDailyPAPeopleSoft.ibusProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjDailyPAPeopleSoft.ibusPersonAccountEAP = new busPersonAccountEAP { icdoPersonAccount = new cdoPersonAccount() };

            lobjDailyPAPeopleSoft.ibusPersonAccount.icdoPersonAccount = icdoPersonAccount;
            lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
            lobjDailyPAPeopleSoft.LoadPersonEmploymentForPeopleSoft();
            lobjDailyPAPeopleSoft.LoadPeopleSoftOrgGroupValues();

            if (ibusPerson == null)
                LoadPerson();

            if (ibusProvider == null)
                LoadProvider();

            if (ibusHistory == null)
                LoadPreviousHistory();

            if (idecMonthlyPremium == 0)
                GetMonthlyPremium();

            if (iclbEAPHistory == null)
                LoadEAPHistory(false);

            lobjDailyPAPeopleSoft.ibusProvider = ibusProvider;

			//PIR 20135 Issue - 1
            DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftRecords", new object[5] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id ,
                        ibusHistory.icdoPersonAccountEapHistory.start_date, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,icdoPersonAccount.person_account_id },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //PIR 26238
            if (iclbOverlappingHistory.IsNotNull() && iclbOverlappingHistory.Count() > 0)
            {
                foreach (busPersonAccountEAPHistory lobjDeletedHistory in iclbOverlappingHistory)
                {
                    DBFunction.DBNonQuery("cdoPersonAccount.UpdatePrevPeopleSoftNBenefitEnrlFlag", new object[2] { lobjDeletedHistory.icdoPersonAccountEapHistory.person_account_eap_history_id, icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            }

            if ((lobjDailyPAPeopleSoft.iclbPeopleSoftOrgGroupValue.Where(i => i.code_value == lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value &&
                                                                                    i.data2 == busConstant.Flag_Yes).Count() > 0))
            {
                if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft == null)
                    lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft = new Collection<busDailyPersonAccountPeopleSoft>();

                lobjDailyPAPeopleSoft.GeneratePeoplesoftEntryForEAP();

                if (lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.IsNotNull() && lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft.Count > 0)
                {
                    foreach (busDailyPersonAccountPeopleSoft lobjDailyPeopleSoft in lobjDailyPAPeopleSoft.iclbDailyPersonAccountPeopleSoft)
                    {
                        lobjEnrollmentData.icdoEnrollmentData.source_id = ibusHistory.icdoPersonAccountEapHistory.person_account_eap_history_id;
                        lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                        lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                        lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                        lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                        lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                        lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                        lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusHistory.icdoPersonAccountEapHistory.plan_participation_status_value;
                        lobjEnrollmentData.icdoEnrollmentData.change_reason_value = icdoPersonAccount.reason_value;
                        lobjEnrollmentData.icdoEnrollmentData.start_date = ibusHistory.icdoPersonAccountEapHistory.start_date;
                        lobjEnrollmentData.icdoEnrollmentData.end_date = ibusHistory.icdoPersonAccountEapHistory.end_date;
                        lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusHistory.icdoPersonAccountEapHistory.provider_org_id;
                        lobjEnrollmentData.icdoEnrollmentData.monthly_premium = idecMonthlyPremium;

                        lobjEnrollmentData.icdoEnrollmentData.coverage_code = lobjDailyPeopleSoft.istrCoverageCode;
                        lobjEnrollmentData.icdoEnrollmentData.benefit_plan = lobjDailyPeopleSoft.istrBenefitPlan;
                        lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date = lobjDailyPeopleSoft.idtDeductionBeginDate;
                        lobjEnrollmentData.icdoEnrollmentData.coverage_begin_date = lobjDailyPeopleSoft.idtCoverageBeginDate;
                        lobjEnrollmentData.icdoEnrollmentData.plan_type = lobjDailyPeopleSoft.istrPlanType;
                        lobjEnrollmentData.icdoEnrollmentData.coverage_election_value = lobjDailyPeopleSoft.istrCoverageElection;
                        lobjEnrollmentData.icdoEnrollmentData.election_date = lobjDailyPeopleSoft.idtElectionDate;
                        lobjEnrollmentData.icdoEnrollmentData.pretax_amount = lobjDailyPeopleSoft.idecFlatAmount;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_change_reason_value = lobjDailyPAPeopleSoft.istrPSFileChangeEvent;

                        if ((lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue &&
                            busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date.AddMonths(-1), lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                            new DateTime(lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Year, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Month, 1).AddMonths(2)))))
                            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                        else
                            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_Yes;
                        lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_No;
                        lobjEnrollmentData.icdoEnrollmentData.Insert();

                        //PIR 23856 - Update previous PS sent flag in Enrollment Data to Y if Person Employment is changed (transfer).
                        DBFunction.DBNonQuery("cdoPersonAccount.UpdatePSSentFlagForTransfers", new object[5] { icdoPersonAccount.plan_id,icdoPersonAccount.person_id,
                                                                                                  lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                                                  lobjEnrollmentData.icdoEnrollmentData.deduction_begin_date,icdoPersonAccount.person_account_id },
                                                                                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    }
                }
            }
            else
            {
                if ((lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || (lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue &&
                busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date.AddMonths(-1), lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                new DateTime(lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Year, lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date.Month, 1).AddMonths(2)))))
                {
                    lobjEnrollmentData.icdoEnrollmentData.source_id = ibusHistory.icdoPersonAccountEapHistory.person_account_eap_history_id;
                    lobjEnrollmentData.icdoEnrollmentData.plan_id = icdoPersonAccount.plan_id;
                    lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
                    lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
                    lobjEnrollmentData.icdoEnrollmentData.person_account_id = icdoPersonAccount.person_account_id;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
                    lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                    lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                    lobjEnrollmentData.icdoEnrollmentData.plan_status_value = ibusHistory.icdoPersonAccountEapHistory.plan_participation_status_value;
                    lobjEnrollmentData.icdoEnrollmentData.change_reason_value = icdoPersonAccount.reason_value;
                    lobjEnrollmentData.icdoEnrollmentData.start_date = ibusHistory.icdoPersonAccountEapHistory.start_date;
                    lobjEnrollmentData.icdoEnrollmentData.end_date = ibusHistory.icdoPersonAccountEapHistory.end_date;
                    lobjEnrollmentData.icdoEnrollmentData.provider_org_id = ibusHistory.icdoPersonAccountEapHistory.provider_org_id;
                    lobjEnrollmentData.icdoEnrollmentData.monthly_premium = idecMonthlyPremium;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_org_group_value = lobjDailyPAPeopleSoft.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                    lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                    lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
                    lobjEnrollmentData.icdoEnrollmentData.Insert();
                }
            }
        }

        public void RefreshValues()
        {
            icdoPersonAccount.suppress_warnings_flag = string.Empty; // UAT PIR ID 1015
        }

        private void CreateAdjustmentPayrollForEnrollmentHistoryClose(busPersonAccountEAPHistory abusTerminatedHistory, string astrHistoryAddedStatus = null)
        {
            busEmployerPayrollHeader lbusPayrollHdr = null;
            int lintOrgID = 0;
            DateTime ldatEffectedDate = abusTerminatedHistory.icdoPersonAccountEapHistory.end_date.AddMonths(1);
            ldatEffectedDate = new DateTime(ldatEffectedDate.Year, ldatEffectedDate.Month, 1);
            if (iclbInsuranceContributionAll == null)
                LoadInsuranceContributionAll();
            var lenuContributionByMonth =
                iclbInsuranceContributionAll.GroupBy(i => i.icdoPersonAccountInsuranceContribution.effective_date).Select(o => o.First());

            if (lenuContributionByMonth != null)
            {
                foreach (busPersonAccountInsuranceContribution lbusInsuranceContribution in lenuContributionByMonth)
                {
                    if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date >= ldatEffectedDate)
                    {
                        if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting)
                        {
                            if (lbusPayrollHdr == null)
                            {
                                // As only most recent record can be ended then all these row should belong to same org id
                                busPersonEmploymentDetail lbusEmploymentDetail = new busPersonEmploymentDetail();
                                lbusEmploymentDetail.FindPersonEmploymentDetail(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.person_employment_dtl_id);
                                lbusEmploymentDetail.LoadPersonEmployment();
                                lintOrgID = lbusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;

                                //There are scenario, where Regular Payroll may not come.. at that time, we need to load the employment in different wat
                                if (lintOrgID == 0)
                                {
                                    if (ibusPersonEmploymentDetail == null)
                                    {
                                        LoadPersonEmploymentDetail();
                                    }
                                    if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                                    {
                                        ibusPersonEmploymentDetail.LoadPersonEmployment();
                                    }
                                    lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                                }

                                lbusPayrollHdr = new busEmployerPayrollHeader();
                                if (!lbusPayrollHdr.LoadCurrentAdjustmentPayrollHeader(lintOrgID, busConstant.PayrollHeaderBenefitTypeInsr))
                                {
                                    lbusPayrollHdr.CreateInsuranceAdjustmentPayrollHeader(lintOrgID);
                                    lbusPayrollHdr.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
                                }
                                else
                                {
                                    lbusPayrollHdr.icdoEmployerPayrollHeader.ienuObjectState = ObjectState.Update;
                                    lbusPayrollHdr.LoadEmployerPayrollDetail();
                                }
                            }

                            if (lintOrgID > 0)
                            {
                                //For Negative Adjustment Premoum Amount, we should not only consider Regular.. we also need to consider other adjustment too
                                //Example Scenario : Member was in Single First and Then changed Family and then suspended. This case Single - Family Change
                                //Could have created the adjustment record which also we need to consider while creating Neg Adjustment for suspended case.
                                decimal ldecTotalPremium = 0.00M;
                                var lclbFilterdContribution = _iclbInsuranceContributionAll.Where(
                                    i =>
                                    i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting &&
                                    i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);

                                if (lclbFilterdContribution != null)
                                {
                                    ldecTotalPremium = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.due_premium_amount);
                                }
                                // PIR 24918
                                if (icolPosNegEmployerPayrollDtl.IsNull())
                                    icolPosNegEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();
                                if (ldecTotalPremium > 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, 
								lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecTotalPremium, busConstant.PayrollDetailRecordTypeNegativeAdjustment))
                                {
                                    lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id,
                                                                                       ibusPerson.icdoPerson.first_name, ibusPerson.icdoPerson.last_name,
                                                                                       ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                                                                                       lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecTotalPremium,
                                                                                       lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, acolPosNegEmployerPayrollDtl : icolPosNegEmployerPayrollDtl);
                                }
                                else if (ldecTotalPremium < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecTotalPremium * -1, busConstant.PayrollDetailRecordTypePositiveAdjustment))
                                {
                                    lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);
                                    lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id,
                                                                                           ibusPerson.icdoPerson.first_name, ibusPerson.icdoPerson.last_name,
                                                                                           ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                                                                                           lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date,
                                                                                           ldecTotalPremium * -1, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                                }
                            }
                        }
                    }
                }
            }
            if ((lbusPayrollHdr != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail.Count > 0) && astrHistoryAddedStatus != busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                foreach (busEmployerPayrollDetail lbusPayrollDtl in lbusPayrollHdr.iclbEmployerPayrollDetail)
                {
                    lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                    lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                }
            }
        }

        private void CreateAdjustmentPayrollForEnrollmentHistoryAdd(busPersonAccountEAPHistory abusAddedHistory)
        {
            busEmployerPayrollHeader lbusPayrollHdr = null;
            int lintOrgID = 0;
            DateTime ldatEffectedDate = abusAddedHistory.icdoPersonAccountEapHistory.start_date;
            DateTime ldatCurrentPayPeriod = DateTime.MinValue;
            if (iclbInsuranceContributionAll == null)
                LoadInsuranceContributionAll();

            if (ibusPersonEmploymentDetail == null)
            {
                LoadPersonEmploymentDetail();
            }
            if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
            {
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            }
            lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;

            //If no contribution record, we must load the current pay period from payroll header / IBS header.
            if (ldatCurrentPayPeriod == DateTime.MinValue)
            {
                //Discussion with Satya : If the new Members enrolled, there wont be any contribution record. To handle such scenario also,
                //we are now taking the current pay pareiod from payroll header instead of last pay period from contrbution table.
                //Pure IBS Member
                if (lintOrgID > 0)
                {
                    DataTable ldtResult = Select<cdoEmployerPayrollHeader>(
                        new string[4] { "org_id", "HEADER_TYPE_VALUE", "REPORT_TYPE_VALUE", "STATUS_VALUE" },
                        new object[4]
                            {
                                lintOrgID,busConstant.PayrollHeaderBenefitTypeInsr, 
                                busConstant.PayrollHeaderReportTypeRegular,busConstant.PayrollHeaderStatusPosted
                            }, null, "EMPLOYER_PAYROLL_HEADER_ID desc");
                    if ((ldtResult != null) && (ldtResult.Rows.Count > 0))
                    {
                        if (!Convert.IsDBNull(ldtResult.Rows[0]["PAYROLL_PAID_DATE"]))
                        {
                            ldatCurrentPayPeriod = Convert.ToDateTime(ldtResult.Rows[0]["PAYROLL_PAID_DATE"]);
                        }
                    }
                }
            }

            if (ldatCurrentPayPeriod == DateTime.MinValue)
            {
                cdoCodeValue lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantsAndVariablesCodeID, busConstant.SystemConstantsLastEmployerPostingDate);
                ldatCurrentPayPeriod = new DateTime(Convert.ToDateTime(lcdoCodeValue.data1).Year, Convert.ToDateTime(lcdoCodeValue.data1).Month, 1);
            }

            while (ldatEffectedDate <= ldatCurrentPayPeriod)
            {
                //Recalculate the Premium based on the New Effective Date
                LoadProviderOrgPlanByProviderOrgID(abusAddedHistory.icdoPersonAccountEapHistory.provider_org_id, ldatEffectedDate);
                GetMonthlyPremium(ibusProviderOrgPlan.icdoOrgPlan.org_plan_id, ldatEffectedDate);

                if (lintOrgID > 0)
                {
                    // Health/D/V/Life
                    if (lbusPayrollHdr == null)
                    {
                        lbusPayrollHdr = new busEmployerPayrollHeader();
                        if (!lbusPayrollHdr.LoadCurrentAdjustmentPayrollHeader(lintOrgID, busConstant.PayrollHeaderBenefitTypeInsr))
                        {
                            lbusPayrollHdr.CreateInsuranceAdjustmentPayrollHeader(lintOrgID);
                            lbusPayrollHdr.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
                        }
                        else
                        {
                            lbusPayrollHdr.icdoEmployerPayrollHeader.ienuObjectState = ObjectState.Update;
                            lbusPayrollHdr.LoadEmployerPayrollDetail();
                        }
                    }
                    //PIR 24918
                    if (icolPosNegEmployerPayrollDtl.IsNull())
                        icolPosNegEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();
                    if (idecMonthlyPremium > 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, 
					ldatEffectedDate, idecMonthlyPremium, busConstant.PayrollDetailRecordTypePositiveAdjustment))
                    {
                        lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ldatEffectedDate);
                        lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                            ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                            ldatEffectedDate, idecMonthlyPremium, ibusProviderOrgPlan.icdoOrgPlan.org_id, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                    }
                    else if (idecMonthlyPremium < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, 
					ldatEffectedDate, idecMonthlyPremium * -1, busConstant.PayrollDetailRecordTypeNegativeAdjustment))
                    {
                        lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                            ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                            ldatEffectedDate, idecMonthlyPremium * -1, ibusProviderOrgPlan.icdoOrgPlan.org_id, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);
                    }
                }
                ldatEffectedDate = ldatEffectedDate.AddMonths(1);
            }
            Collection<busEmployerPayrollDetail> lcolFinal = ComparePositiveNegativeColEmployerPayrollDetail();
            if (lbusPayrollHdr != null && (lcolFinal != null) && (lcolFinal.Count > 0))
            {
                if (lbusPayrollHdr != null)
                    lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                foreach (busEmployerPayrollDetail lbusPayrollDtl in lcolFinal)
                {
                    if(lbusPayrollHdr != null && lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id == 0)
                        lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                    lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                }
            }

            //Reset the Premium with Screen Data
            LoadProviderOrgPlanByProviderOrgID(icdoPersonAccount.provider_org_id, icdoPersonAccount.current_plan_start_date_no_null);
            GetMonthlyPremium();
        }


        public void LoadPreviousHistory()
        {
            if (_ibusHistory == null)
            {
                _ibusHistory = new busPersonAccountEAPHistory();
                _ibusHistory.icdoPersonAccountEapHistory = new cdoPersonAccountEapHistory();
            }

            if (iclbEAPHistory == null)
                LoadEAPHistory();

            if (iclbEAPHistory.Count > 0)
                ibusHistory = iclbEAPHistory.First();
        }

        private void SetHistoryEntryRequiredOrNot()
        {
            if (_ibusHistory == null)
                LoadPreviousHistory();

            if ((icdoPersonAccount.plan_participation_status_value != ibusHistory.icdoPersonAccountEapHistory.plan_participation_status_value) ||
            (icdoPersonAccount.current_plan_start_date != ibusHistory.icdoPersonAccountEapHistory.start_date) ||
            (icdoPersonAccount.provider_org_id != ibusHistory.icdoPersonAccountEapHistory.provider_org_id))
            {
                IsHistoryEntryRequired = true;
            }
            else
            {
                IsHistoryEntryRequired = false;
            }
        }

        public busPersonAccountEAPHistory LoadHistoryByDate(DateTime adtGivenDate)
        {
            busPersonAccountEAPHistory lobjPersonAccountEAPHistory = new busPersonAccountEAPHistory();
            lobjPersonAccountEAPHistory.icdoPersonAccountEapHistory = new cdoPersonAccountEapHistory();

            if (iclbEAPHistory == null)
                LoadEAPHistory(false);

            foreach (busPersonAccountEAPHistory lobjPAEAPHistory in iclbEAPHistory)
            {
                //Ignore the Same Start Date and End Date Records
                if (lobjPAEAPHistory.icdoPersonAccountEapHistory.start_date != lobjPAEAPHistory.icdoPersonAccountEapHistory.end_date)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adtGivenDate, lobjPAEAPHistory.icdoPersonAccountEapHistory.start_date,
                        lobjPAEAPHistory.icdoPersonAccountEapHistory.end_date))
                    {
                        lobjPersonAccountEAPHistory = lobjPAEAPHistory;
                        break;
                    }
                }
            }
            return lobjPersonAccountEAPHistory;
        }

        //Logic of Date to Calculate Premium Amount has been changed and the details are mailed on 3/18/2009 after the discussion with RAJ.
        /*************************
         * 1) Member A started the Health Plan on Jan 1981 and the plan is still open.
         *      In this case, System will display the rates as of Today.
         * 2) Member A started the Health Plan on Jan 2000 and Suspended the Plan on May 2009. 
         *      In this case, system will display the rate as of End date of Latest Enrolled Status History Record. (i.e) Apr 2009. 
         * 3) Third Scenario (Future Date Scenario) might be little bit complicated. Let me know your feedback too. 
         *    If the Member starts the plan on Jan 2000 with the Single Coverage and May 2009 he wants to change to Family.
         *      Current Date is Mar 18 2009. But the latest enrolled history record is future date. 
         *      So System will display the rate as of Start Date of Latest Enrolled History Date. (i.e) of May 2009
         * *************************/

        public void LoadPlanEffectiveDate()
        {
            idtPlanEffectiveDate = DateTime.Now;

            //If the Current Participation status is enrolled, Set the Effective Date from History Change Date
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                if (icdoPersonAccount.current_plan_start_date_no_null > DateTime.Now)
                    idtPlanEffectiveDate = icdoPersonAccount.current_plan_start_date_no_null;
                else
                    idtPlanEffectiveDate = DateTime.Now;
            }
            else
            {
                if (iclbEAPHistory == null)
                    LoadEAPHistory(false);

                //By Default the Collection sorted by latest date
                foreach (busPersonAccountEAPHistory lbusPersonAccountEapHistory in iclbEAPHistory)
                {
                    if (lbusPersonAccountEapHistory.icdoPersonAccountEapHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        if (lbusPersonAccountEapHistory.icdoPersonAccountEapHistory.end_date == DateTime.MinValue)
                        {
                            //If the Start Date is Future Date, Set it otherwise Current Date will be Start Date of Premium Calc
                            if (lbusPersonAccountEapHistory.icdoPersonAccountEapHistory.start_date > DateTime.Now)
                            {
                                idtPlanEffectiveDate = lbusPersonAccountEapHistory.icdoPersonAccountEapHistory.start_date;
                            }
                            else
                            {
                                idtPlanEffectiveDate = DateTime.Now;
                            }
                        }
                        else
                        {
                            idtPlanEffectiveDate = lbusPersonAccountEapHistory.icdoPersonAccountEapHistory.end_date;
                        }
                        break;
                    }
                }
            }
        }

        public bool IsProviderOrgIDValid()
        {
            Collection<cdoOrganization> lclbActiveEmployers = new Collection<cdoOrganization>();
            lclbActiveEmployers = LoadActiveProviders();
            if (icdoPersonAccount.provider_org_id > 0)
            {
                if (lclbActiveEmployers.Where(o => o.org_id == icdoPersonAccount.provider_org_id).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccountForEAP();
        }
        /// <summary>
        /// uat pir 2373 : method to set the PS event value based on enrollment change
        /// </summary>
        private void SetPersonAccountForEnrollmentChange()
        {
            if (ibusHistory == null)
                LoadPreviousHistory();
            if (IsHistoryEntryRequired)
            {
                if ((icdoPersonAccount.plan_participation_status_value  == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                    ibusHistory.icdoPersonAccountEapHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
                    || 
                    ((iclbOverlappingHistory != null && iclbOverlappingHistory.Count > 0) && 
                    (iclbOverlappingHistory[0].icdoPersonAccountEapHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended 
                    && icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)))//PIR 22683
                {
                    icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                }
            }
        }

        #region UCS 22 correspondence
        //load person account
        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccountForEAP()
        {
            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

            ibusPersonAccount.icdoPersonAccount = this.icdoPersonAccount;
        }
        #endregion
    }
}
