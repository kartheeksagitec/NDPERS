using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using System.Collections;
using Sagitec.DataObjects;
using Sagitec.DBUtility;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSDefCompWeb : busPersonAccountDeferredComp
    {
        public string istrAcknowledgementText { get; set; }   //PIR-2368
        public string istrAcknowledgementTextWaive { get; set; }
        public void AssignWSSEnrollmentRequest()
        {
            ibusWSSPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest { icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest() };
            ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id = icdoPersonAccount.person_id;
            ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_id = icdoPersonAccount.plan_id;
            ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.target_person_account_id = icdoPersonAccount.person_account_id;
            ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = icdoPersonAccount.person_employment_dtl_id;
            ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_id = busConstant.MemberPortalEnrollmentRequestStatus;
            ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_id = busConstant.MemberPortalEnrollmentType;
            ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.DeferredCompensation;
        }

        public Collection<cdoCodeValue> LoadPlanEnrollmentOption()
        {
            return ibusWSSPersonAccountEnrollmentRequest.LoadPlanEnrollmentOption();
        }

        public Collection<cdoCodeValue> LoadChangeReason(string astrPlanEnrollmentOption)
        {
            return ibusWSSPersonAccountEnrollmentRequest.LoadChangeReason(astrPlanEnrollmentOption);
        }

        public DateTime idtProviderStartDate { get; set; }

        public string istrExpeditedAcknowledgement { get; set; }

        public bool IsAllExpDeffCompAcknowledgementSelected() => istrExpeditedAcknowledgement == busConstant.Flag_Yes;

        public string istrAcknowledgement { get; set; }

        public bool IsAllDeffCompAcknowledgementSelected()
        {
            if (istrAcknowledgement == busConstant.Flag_No || string.IsNullOrEmpty(istrAcknowledgement))
                return false;
            return true;
        }

        public void LoadCalculatedStartDate()
        {
            string lstrFrquency = string.Empty;
            DateTime ldtUserModifiedDate = DateTime.Now;

            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();


            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan.IsNull())
                ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

            var lenumGetDefComp = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan
                .Where(lobjOrgp => lobjOrgp.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation);

            if (lenumGetDefComp.Count() > 0)
            {
                busOrgPlan lobjOrgPlan = lenumGetDefComp.FirstOrDefault();

                lstrFrquency = lobjOrgPlan.icdoOrgPlan.report_frequency_value;
                istrFrequency = lstrFrquency; // PIR 11967
                //Commented for PIR 11512
                //if (lstrFrquency == busConstant.DeffCompFrequencyMonthly)
                //{
                //    if (ldtUserModifiedDate.Day >= 1
                //        && ldtUserModifiedDate.Day <= 15)
                //    {
                //        idtProviderStartDate = ldtUserModifiedDate.GetFirstDayofNextMonth();
                //    }
                //    else
                //    {
                //        DateTime ldtTempDate = ldtUserModifiedDate.AddMonths(2);
                //        idtProviderStartDate = new DateTime(ldtTempDate.Year, ldtTempDate.Month, 1);
                //    }
                //}
                //else if ((lstrFrquency == busConstant.DeffCompFrequencySemiMonthly)
                //    || (lstrFrquency == busConstant.DeffCompFrequencyWeekly)
                //    || (lstrFrquency == busConstant.DeffCompFrequencyBiWeekly))
                //{
                //    if (ldtUserModifiedDate.Day >= 1
                //        && ldtUserModifiedDate.Day <= 15)
                //    {
                //        idtProviderStartDate = ldtUserModifiedDate.GetFirstDayofNextMonth();
                //    }
                //    else
                //    {
                //        DateTime ldtTempDate = ldtUserModifiedDate.AddMonths(1);
                //        idtProviderStartDate = new DateTime(ldtTempDate.Year, ldtTempDate.Month, 16);
                //    }
                //}

                //PIR 14377
                if (ibusDefCompProvider.IsNull())
                    ibusDefCompProvider = new busPersonAccountDeferredCompProvider() { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
                if (lstrFrquency == busConstant.DeffCompFrequencyBiWeekly || lstrFrquency == busConstant.DeffCompFrequencyWeekly || 
                    lstrFrquency == busConstant.DeffCompFrequencyMonthly || lstrFrquency == busConstant.DeffCompFrequencySemiMonthly)
                    idtProviderStartDate = ibusDefCompProvider.LoadEPHPayNextPeriodStartDate(lobjOrgPlan.icdoOrgPlan.org_id, lstrFrquency, lobjOrgPlan.icdoOrgPlan.day_of_month);

                //If no prior payroll reports for Def Comp exist (unless ‘Day of Month’ is set on Org Plan) default to 1st of month following election date
                if (idtProviderStartDate == DateTime.MinValue)
                    idtProviderStartDate = ldtUserModifiedDate.GetFirstDayofNextMonth().AddDays(lobjOrgPlan.icdoOrgPlan.day_of_month); // PIR 11512
            }
        }

        // PIR 11967
        public string istrFrequency { get; set; }

        public decimal idecExpeditedAmount
        {
            get
            {
                string lstrData1Value = busGlobalFunctions.GetData1ByCodeValue(52, "EDAM", iobjPassInfo);
                // PIR 11967 start
                lstrData1Value += ".00";
                if (istrFrequency == busConstant.DeffCompFrequencySemiMonthly || istrFrequency == busConstant.DeffCompFrequencyBiWeekly)
                    lstrData1Value = "12.50";
                // PIR 11967 end
                return Convert.ToDecimal(lstrData1Value);
            }
        }

        public int iintProviderContactAgentId
        {
            get
            {
                string lstrData1Value = busGlobalFunctions.GetData1ByCodeValue(52, "PRCO", iobjPassInfo);
                return Convert.ToInt32(lstrData1Value);
            }
        }

        public busOrganization ibusExpeditedProvider { get; set; }
        public void LoadExpeditedProvider()
        {
            ibusExpeditedProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
            ibusExpeditedProvider.FindOrganizationByOrgCode(busGlobalFunctions.GetData1ByCodeValue(52, "PROR", iobjPassInfo));
        }

        #region Acknowledgements PIR 6961

        public utlCollection<cdoCodeValue> iclbMSSDefCompAcknowledgements { get; set; }
        public utlCollection<cdoWssAcknowledgement> iclbMSSExpDefCompAcknowledgements { get; set; }
        public Collection<busWssAcknowledgement> iclbDefCompExpAck { get; set; }  //upgrade PIR-2513
        public Collection<busWssAcknowledgement> iclbDefCompAcknowledgementCheck { get; set; }
        public Collection<busWssAcknowledgement> iclbDefCompAcknowledgementGen { get; set; }
        public Collection<busWssAcknowledgement> iclbDefCompAcknowledgement { get; set; }
        public busPersonEmployment ibusPersonEmployment { get; set; }
        public cdoPersonAccountDeferredCompProvider icdoPersonAccountDeferredCompProvider { get; set; }

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

        public bool FindPersonAccountDeferredCompByRequestID(int aintid)
        {
            DataTable ltdtbWssPersonAccountDeferredComp = Select<cdoWssPersonAccountDeferredComp>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" }, new object[1] { aintid }, null, null);
            if (icdoWssPersonAccountEnrollmentRequest.IsNull())
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
            if (ltdtbWssPersonAccountDeferredComp.Rows.Count > 0)
            {
                icdoPersonAccountDeferredComp.person_account_id = Convert.ToInt32(ltdtbWssPersonAccountDeferredComp.Rows[0]["TARGET_PERSON_ACCOUNT_ID"]);
                icdoWssPersonAccountEnrollmentRequest.LoadData(ltdtbWssPersonAccountDeferredComp.Rows[0]);
                FindPersonAccount(icdoPersonAccountDeferredComp.person_account_id);
                FindPersonAccountDeferredComp(icdoPersonAccountDeferredComp.person_account_id);
                return true;
            }
            return false;
        }
        busBase ibusbase = new busBase();  //upgrade PIR-2513
        public Collection<busWssAcknowledgement> LoadExpAcknowledgement()
        {
            iclbDefCompExpAck = new Collection<busWssAcknowledgement>();
            DataTable ldtbListWSSAcknowledgement = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.DeferredCompCheck });
            iclbDefCompExpAck = ibusbase.GetCollection<busWssAcknowledgement>(ldtbListWSSAcknowledgement, "icdoWssAcknowledgement");
            return iclbDefCompExpAck;
        }

        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbPersonAccountEnrollmentRequestAck { get; set; }
        public void LoadDefCompAckDetailsForView(int aintrequestid)
        {
            busBase lbus = new busBase();
            iclbPersonAccountEnrollmentRequestAck = new Collection<busWssPersonAccountEnrollmentRequestAck>();

            DataTable ldtbAckDetailsView = Select("cdoWssPersonAccountEnrollmentRequestAck.SelectAckforView",
                new object[1] { aintrequestid });

            iclbPersonAccountEnrollmentRequestAck = lbus.GetCollection<busWssPersonAccountEnrollmentRequestAck>(ldtbAckDetailsView);
            StringBuilder lstrAcknowledgement_text = new StringBuilder();  //PIR-2368
            foreach (busWssPersonAccountEnrollmentRequestAck lobjPersonAccountEnrollmentRequestAck in iclbPersonAccountEnrollmentRequestAck)
            {
                if (lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text != null)
                {

                }
                else if (lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text == null)
                {
                    DataTable ldtbSpecificAckForView = Select("cdoWssAcknowledgement.SelectSpecificAckforView",
                        new object[1] { lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id });
                    busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                    lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                    lobjWssAcknowledgement.icdoWssAcknowledgement.LoadData(ldtbSpecificAckForView.Rows[0]);
                    lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;                    
                    lstrAcknowledgement_text.Append((lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text).ToString());
                    lstrAcknowledgement_text.Append("<br/>");                   
                }               
            }
            istrAcknowledgementText = lstrAcknowledgement_text.ToString();
            istrAcknowledgementTextWaive = lstrAcknowledgement_text.ToString();
        }

        public Collection<busWssAcknowledgement> LoadAcknowledgement()
        {
            DataTable ldtbListWSSAcknowledgement = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.DeferredCompCheck });
            iclbDefCompAcknowledgement = ibusbase.GetCollection<busWssAcknowledgement>(ldtbListWSSAcknowledgement, "icdoWssAcknowledgement");
            return iclbDefCompAcknowledgement;
        }

        public Collection<busWssAcknowledgement> LoadDefCompAcknowledgementGen()
        {
            iclbDefCompAcknowledgementGen = new Collection<busWssAcknowledgement>();
            DataTable ldtbListWSSAcknowledgement = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.DeferredCompWaiveGeneral });
            iclbDefCompAcknowledgementGen = ibusbase.GetCollection<busWssAcknowledgement>(ldtbListWSSAcknowledgement, "icdoWssAcknowledgement");
            return iclbDefCompAcknowledgementGen;
        }

        public Collection<busWssAcknowledgement> LoadDefCompAcknowledgementCheck()
        {
            iclbDefCompAcknowledgementCheck = new Collection<busWssAcknowledgement>();
            DataTable ldtbListWSSAcknowledgement = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.DeferredCompWaiveCheck });
            iclbDefCompAcknowledgementCheck = ibusbase.GetCollection<busWssAcknowledgement>(ldtbListWSSAcknowledgement, "icdoWssAcknowledgement");
            return iclbDefCompAcknowledgementCheck;
        }
        #endregion

        # region Expedited

        public override void ValidateGroupRules(string astrGroupName, utlPageMode aenmPageMode)
        {
            base.ValidateGroupRules(astrGroupName, aenmPageMode);
            if (ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.PlanEnrollmentQuickEnrollment &&
                astrGroupName == "WizardStep1")
            {
                LoadExpAcknowledgement();
            }
        }

        public override void BeforeWizardStepValidate(utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {
            if (astrWizardStepName == "wzsIntro")
            {
                icdoPersonAccount.reason_value = ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value;
                ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.PlanEnrollmentRegularEnrollment;
                if (ibusDefCompProvider.iclbDefCompProviders == null) ibusDefCompProvider.LoadDeferredCompProviderByProvider();
                if (IsEmployerMatchAvailable && ibusDefCompProvider.iclbDefCompProviders.IsNotNull() && ibusDefCompProvider.iclbDefCompProviders.Count == 0)
                    ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution = busConstant.Flag_Yes;
            }
			//PIR 25920 New Plan DC 2025 set default value to Yes on employer matching contribution flag
            if (astrWizardStepName == "wzsStep4")
            {
                if (ibusDefCompProvider.iclbDefCompProviders == null) ibusDefCompProvider.LoadDeferredCompProviderByProvider();
                if (IsEmployerMatchAvailable && ibusDefCompProvider.iclbDefCompProviders.IsNotNull() && ibusDefCompProvider.iclbDefCompProviders.Count == 0)
                    ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution = busConstant.Flag_Yes;
            }
            if (astrWizardStepName == "wzsStep2")
            {
                if (ibusDefCompProvider.iintMSSContactId != 0)
                {
                    ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.provider_agent_contact_id = ibusDefCompProvider.GetProviderOrgContactId();
                    ibusDefCompProvider.LoadProviderAgentOrgContact();
                    ibusDefCompProvider.ibusProviderAgentOrgContact.LoadContact();
                }
            }
            base.BeforeWizardStepValidate(aenmPageMode, astrWizardName, astrWizardStepName);
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {

            //F/W UPGRADE pir 11956 - The F/W's Wizard's next/finish button clicks' life cycle methods included calling BeforeValidate, which was not the case in earlier versions, causing issues.
            if (this.iobjPassInfo.istrFormName != "wfmMSSDeferredCompWizard")
                base.BeforeValidate(aenmPageMode);
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            ibusProvider.ValidateHardErrors(aenmPageMode);
            foreach (utlError lobjErr in ibusProvider.iarrErrors)
                iarrErrors.Add(lobjErr);
            foreach (utlError lobjErr in iarrErrors)
                lobjErr.istrErrorID = string.Empty;
        }

        public override void BeforePersistChanges()
        {
            //PIR 6961
            if (iclbDefCompExpAck.Count > 0)
            {
                for (int i = iarrChangeLog.Count - 1; i >= 0; i--)
                {
                    if (iarrChangeLog[i] is cdoWssAcknowledgement)
                    {
                        cdoWssAcknowledgement lcdoWssAcknowledgement = (cdoWssAcknowledgement)iarrChangeLog[i];
                        iarrChangeLog.Remove(lcdoWssAcknowledgement);
                    }
                }
            }
            //PIR 6961
            if (iclbDefCompAcknowledgement.Count > 0)
            {
                for (int i = iarrChangeLog.Count - 1; i >= 0; i--)
                {
                    if (iarrChangeLog[i] is cdoWssAcknowledgement)
                    {
                        cdoWssAcknowledgement lcdoCodeValue = (cdoWssAcknowledgement)iarrChangeLog[i];
                        iarrChangeLog.Remove(lcdoCodeValue);
                    }
                }
            }

            if (istrIsExpDefCompWaivedChecked != busConstant.Flag_Yes)
            {
                int lintCount = 0;
                if (ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.PlanEnrollmentRegularEnrollment)
                    lintCount = 5;
                else
                    lintCount = 6;

                for (int i = lintCount; i >= 1; i--) //PROD PIR ID 6366
                {
                    if (iarrChangeLog.Count >= i)
                    {
                        if (iarrChangeLog[i - 1] is cdoCodeValue)
                        {
                            cdoCodeValue lcdoCodeValue = (cdoCodeValue)iarrChangeLog[i - 1];
                            iarrChangeLog.Remove(lcdoCodeValue);
                        }
                    }
                }
            }
            if (istrIsExpDefCompWaivedChecked == busConstant.Flag_Yes)
            {
                if (ibusPersonAccountEmploymentDetail.IsNull())
                    LoadPersonAccountEmploymentDetail();

                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueWaived;
                // PIR 11684
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.is_waiver_report_generated = busConstant.Flag_No;
                busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                    busConstant.PlanIdDeferredCompensation, iobjPassInfo);
                // PIR 11684 end
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();
                //PIR 17081
                InsertIntoEnrollmentData();
            }
            if (istrIsExpDefCompWaivedChecked != busConstant.Flag_Yes)
            {
                base.BeforePersistChanges();
            }
        }

        //PIR 17081
        public void InsertIntoEnrollmentData()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();

            busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
            lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

            lobjEnrollmentData.icdoEnrollmentData.source_id = ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_employment_dtl_id;
            lobjEnrollmentData.icdoEnrollmentData.plan_id = ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id;
            lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
            lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
            lobjEnrollmentData.icdoEnrollmentData.employer_org_id = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
            lobjEnrollmentData.icdoEnrollmentData.employment_type_value = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
            lobjEnrollmentData.icdoEnrollmentData.start_date = ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.modified_date;
            lobjEnrollmentData.icdoEnrollmentData.monthly_premium = 0.0M;
            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
            lobjEnrollmentData.icdoEnrollmentData.Insert();
        }


        public override int PersistChanges()
        {
            if (istrIsExpDefCompWaivedChecked == busConstant.Flag_Yes)
                return 0;
            else
            {
                ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.target_person_account_id = icdoPersonAccount.person_account_id;
                ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = "NEWH";
                if (ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.PlanEnrollmentRegularEnrollment)
                {
                    if (ibusDefCompProvider.ibusProviderOrganization.IsNull()) ibusDefCompProvider.LoadProviderOrganization();
                    if (ibusDefCompProvider.ibusProviderOrgPlan.IsNull()) ibusDefCompProvider.LoadProviderOrgPlanByProvider();
                    ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.provider_org_plan_id = ibusDefCompProvider.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id;
                    ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.company_name = ibusDefCompProvider.ibusProviderOrganization.icdoOrganization.org_name;
                }
                if (ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.PlanEnrollmentQuickEnrollment)
                {
                    for (int i = iarrChangeLog.Count - 1; i >= 0; i--)
                    {
                        if (iarrChangeLog[i] is cdoPersonAccountDeferredCompProvider)
                        {
                            cdoPersonAccountDeferredCompProvider lcdoPersonAccountDeferredCompProvider = (cdoPersonAccountDeferredCompProvider)iarrChangeLog[i];
                            if (lcdoPersonAccountDeferredCompProvider.provider_org_plan_id == 0 &&
                                lcdoPersonAccountDeferredCompProvider.company_name.IsNull() &&
                                lcdoPersonAccountDeferredCompProvider.mutual_fund_window_flag.IsNull())
                            {
                                iarrChangeLog.Remove(lcdoPersonAccountDeferredCompProvider);
                                break;
                            }
                        }
                    }
                }
                return base.PersistChanges();
            }
        }

        public override void AfterPersistChanges()
        {
            if (istrIsExpDefCompWaivedChecked != busConstant.Flag_Yes)
            {
                int aintPersonAccountDeferredCompid = 0; // PIR 9692
                LoadPreviousHistory();
                iintRequestId = ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                base.AfterPersistChanges();
                PostMessageToEmployer();

                if (ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.PlanEnrollmentQuickEnrollment) // insert provider for Expedited
                {
                    CreateExpeditedDeferredCompProvider();
                }
                else
                { // PIR 9692
                    cdoWssPersonAccountDeferredCompProvider lcdoWssPersonAccountDeferredCompProvider = new cdoWssPersonAccountDeferredCompProvider();
                    LoadPersonAccount();
                    int aintpersonaccountid = icdoPersonAccount.person_account_id;
                    DataTable ldtbPersonAccDefCompProvider = Select("cdoWssAcknowledgement.PersonAccDefCompProvider", new object[1] { aintpersonaccountid });
                    DataTable dtbDeffComp = Select("cdoWssAcknowledgement.DeffComp", new object[1] { aintpersonaccountid });
                    if (dtbDeffComp.Rows.Count > 0)
                    {
                        aintPersonAccountDeferredCompid = Convert.ToInt32(dtbDeffComp.Rows[0]["WSS_PERSON_ACCOUNT_DEFERRED_COMP_ID"]);
                    }
                    if (ldtbPersonAccDefCompProvider.Rows.Count > 0)
                    {
                        lcdoWssPersonAccountDeferredCompProvider.wss_person_account_deferred_comp_id = aintPersonAccountDeferredCompid;
                        lcdoWssPersonAccountDeferredCompProvider.provider_org_plan_id = Convert.ToInt32(ldtbPersonAccDefCompProvider.Rows[0]["provider_org_plan_id"]);
                        lcdoWssPersonAccountDeferredCompProvider.company_name = ldtbPersonAccDefCompProvider.Rows[0]["company_name"].ToString();
                        lcdoWssPersonAccountDeferredCompProvider.provider_agent_contact_id = Convert.ToInt32(ldtbPersonAccDefCompProvider.Rows[0]["provider_agent_contact_id"]);
                        if (ldtbPersonAccDefCompProvider.Rows[0]["per_pay_period_contribution_amt"] != DBNull.Value) //PIR 9999
                            lcdoWssPersonAccountDeferredCompProvider.per_pay_period_contribution_amt = Convert.ToDecimal(ldtbPersonAccDefCompProvider.Rows[0]["per_pay_period_contribution_amt"]);
                        if (ldtbPersonAccDefCompProvider.Rows[0]["start_date"] != DBNull.Value)
                            lcdoWssPersonAccountDeferredCompProvider.start_date = Convert.ToDateTime(ldtbPersonAccDefCompProvider.Rows[0]["start_date"]);
                        if (ldtbPersonAccDefCompProvider.Rows[0]["end_date"] != DBNull.Value)
                            lcdoWssPersonAccountDeferredCompProvider.end_date = Convert.ToDateTime(ldtbPersonAccDefCompProvider.Rows[0]["end_date"]);
                        lcdoWssPersonAccountDeferredCompProvider.mutual_fund_window_flag = ldtbPersonAccDefCompProvider.Rows[0]["mutual_fund_window_flag"].ToString();
                        lcdoWssPersonAccountDeferredCompProvider.person_employment_id = Convert.ToInt32(ldtbPersonAccDefCompProvider.Rows[0]["person_employment_id"]);
                        lcdoWssPersonAccountDeferredCompProvider.assets_with_provider_id = Convert.ToInt32(ldtbPersonAccDefCompProvider.Rows[0]["assets_with_provider_id"]);
                        lcdoWssPersonAccountDeferredCompProvider.assets_with_provider_value = ldtbPersonAccDefCompProvider.Rows[0]["assets_with_provider_value"].ToString();
                        lcdoWssPersonAccountDeferredCompProvider.comments = ldtbPersonAccDefCompProvider.Rows[0]["comments"].ToString();
                        lcdoWssPersonAccountDeferredCompProvider.payment_status_id = Convert.ToInt32(ldtbPersonAccDefCompProvider.Rows[0]["payment_status_id"]);
                        lcdoWssPersonAccountDeferredCompProvider.payment_status_value = ldtbPersonAccDefCompProvider.Rows[0]["payment_status_value"].ToString();
                        lcdoWssPersonAccountDeferredCompProvider.Insert();
                    }
                }
            }

            //PIR6961
            LoadAcknowledgement();
            if (ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.PlanEnrollmentRegularEnrollment)
            {
                iintRequestId = ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                InsertString("DCAM"); // PIR 9692
                InsertCollection(iclbDefCompAcknowledgement);
            }
            if (ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.PlanEnrollmentQuickEnrollment)
            {
                iintRequestId = ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                LoadExpAcknowledgement();
                InsertString("DEG");
                InsertCollection(iclbDefCompExpAck);
            }
            if (istrIsExpDefCompWaivedChecked == busConstant.Flag_Yes)
            {
                base.InsertWSSEnrollmentWaiveRequest();
                InsertCollection(iclbDefCompAcknowledgementGen);
                InsertCollection(iclbDefCompAcknowledgementCheck);
            }
            InsertString("CONF");

            //Regular Enrollment
            if (ibusWSSPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.PlanEnrollmentRegularEnrollment) 
            {
                busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProvider = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new CustomDataObjects.cdoPersonAccountDeferredCompProvider() };
                if (lbusPersonAccountDeferredCompProvider.FindPersonAccountDeferredCompProvider(ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.person_account_provider_id))
                {
                    lbusPersonAccountDeferredCompProvider.LoadPersonAccountDeferredComp();
                    lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPerson();
                    lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPlan();
                    lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = icdoPersonAccount.person_employment_dtl_id;
                    lbusPersonAccountDeferredCompProvider.LoadPersonAccount();
                    lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPersonEmploymentDetail();
                    lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
                    lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                    lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.LoadOrgPlan();
                    lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.LoadPreviousHistory();
                    if (lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date != lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date)
                    {
                        //PIR 25920 - First time enrolling in Deferred comp. Check if there is an enrolled DC25 plan, if yes, send an extra entry for the Deferred comp.
                        DataTable ldtRetirement = DBFunction.DBSelect("entPersonAccount.IsEnrolledInDC25", new object[2] 
                        { lbusPersonAccountDeferredCompProvider.ibusPersonAccountDeferredComp.ibusPerson.icdoPerson.person_id, lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                        if (ldtRetirement.Rows.Count > 0)
                        {
                            lbusPersonAccountDeferredCompProvider.InsertIntoEnrollmentData(false, busConstant.Flag_No);//Entry for amount changes/new enrollment
                            lbusPersonAccountDeferredCompProvider.InsertIntoEnrollmentData(false, busConstant.Flag_No, true, Convert.IsDBNull(ldtRetirement.Rows[0]["addl_ee_contribution_percent"]) ? 0 :  Convert.ToInt32(ldtRetirement.Rows[0]["addl_ee_contribution_percent"]));//Extra line for DC25
                        }
                        else
                            lbusPersonAccountDeferredCompProvider.InsertIntoEnrollmentData(false, busConstant.Flag_No);
                    }
                }
            }
        }

        // PIR 9692
        public string istrDefCompAmountMessage
        {
            get
            {

                string lstrDefCompAmountMessage = "I authorize my employer to reduce my salary for the above indicated amounts.";
                return lstrDefCompAmountMessage;
            }
        }

        public busPersonAccountEmploymentDetail ibusPersonAccountEmploymentDetail { get; set; }
        public void LoadPersonAccountEmploymentDetail()
        {
            if (ibusPersonAccountEmploymentDetail.IsNull())
            {
                ibusPersonAccountEmploymentDetail = new busPersonAccountEmploymentDetail();
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail();
            }
            DataTable ldtbList = Select<cdoPersonAccountEmploymentDetail>(new string[2] { "plan_id", "PERSON_EMPLOYMENT_DTL_ID" },
                new object[2] { busConstant.PlanIdDeferredCompensation, icdoPersonAccount.person_employment_dtl_id }, null, null);

            if (ldtbList.Rows.Count > 0)
            {

                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.LoadData(ldtbList.Rows[0]);
            }
        }

        //PIR 6961
        public void InsertCollection(Collection<busWssAcknowledgement> aclbAcknowledgement)
        {
            busWssPersonAccountEnrollmentRequestAck lobjWssPersonAccountEnrollmentRequestAck = new busWssPersonAccountEnrollmentRequestAck();
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck = new cdoWssPersonAccountEnrollmentRequestAck();
            icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();

            foreach (busWssAcknowledgement lobjWssAcknowledgement in aclbAcknowledgement)
            {
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_id;
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.wss_person_account_enrollment_request_id = base.iintRequestId;
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.Insert();
            }

        }
        //PIR 6961
        public void InsertString(string astrQuersyString)
        {
            busWssPersonAccountEnrollmentRequestAck lobjWssPersonAccountEnrollmentRequestAck = new busWssPersonAccountEnrollmentRequestAck();
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck = new cdoWssPersonAccountEnrollmentRequestAck();

            DataTable ldtbListdtDTP = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='" + astrQuersyString + "'");
            int ack_id = 0;
            if (ldtbListdtDTP.Rows.Count > 0)
                ack_id = Convert.ToInt32(ldtbListdtDTP.Rows[0]["acknowledgement_id"]);
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id = ack_id;
            if (astrQuersyString == "CONF")
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrConfirmationText;
            else if (astrQuersyString == "DCAM")  // PIR 9692
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrDefCompAmountMessage;
            else if (astrQuersyString == "DEG")
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrExpMessage;
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.wss_person_account_enrollment_request_id = base.iintRequestId;
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.Insert();
        }

        private void CreateExpeditedDeferredCompProvider()
        {
            if (ibusExpeditedProvider.IsNull())
                LoadExpeditedProvider();

            if (ibusProviderOrgPlan.IsNull())
                LoadProviderOrgPlanByProviderOrgID(ibusExpeditedProvider.icdoOrganization.org_id, idtProviderStartDate);

            busPersonAccountDeferredCompProvider lobjPersonAccountDefComp = new busPersonAccountDeferredCompProvider
            {
                icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider()
            };

            lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.person_account_id = icdoPersonAccount.person_account_id;
            lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt = idecExpeditedAmount;
            lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.provider_org_plan_id = ibusProviderOrgPlan.icdoOrgPlan.org_plan_id;
            lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.person_employment_id = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_employment_id;
            lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.start_date = idtProviderStartDate;
            lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.provider_agent_contact_id = iintProviderContactAgentId;
            lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.istrProviderOrgCode = ibusExpeditedProvider.icdoOrganization.org_code;

            lobjPersonAccountDefComp.ibusPersonAccountDeferredComp = new busPersonAccountDeferredComp();
            lobjPersonAccountDefComp.ibusPersonAccountDeferredComp.icdoPersonAccountDeferredComp = icdoPersonAccountDeferredComp;
            lobjPersonAccountDefComp.ibusPersonAccountDeferredComp.icdoPersonAccount = icdoPersonAccount;
            lobjPersonAccountDefComp.icdoPersonAccountDeferredCompProvider.ienuObjectState = ObjectState.Insert;
            lobjPersonAccountDefComp.iblnIsFromPortal = true;
            lobjPersonAccountDefComp.LoadAllDeferredCompProviderByProvider();
            if (lobjPersonAccountDefComp.iclbPersonAccountDeferredCompProviderByProvider.Where(lobjPro =>
                busGlobalFunctions.CheckDateOverlapping(idtProviderStartDate, lobjPro.icdoPersonAccountDeferredCompProvider.start_date, lobjPro.icdoPersonAccountDeferredCompProvider.end_date)).Count() == 0)
            {
                if (lobjPersonAccountDefComp.ibusProviderOrganization.IsNull())
                    lobjPersonAccountDefComp.LoadProviderOrganization();

                lobjPersonAccountDefComp.BeforeValidate(utlPageMode.New);
                lobjPersonAccountDefComp.ValidateHardErrors(utlPageMode.New);
                lobjPersonAccountDefComp.BeforePersistChanges();
                lobjPersonAccountDefComp.PersistChanges();
                lobjPersonAccountDefComp.AfterPersistChanges();
            }
        }

        private void PostMessageToEmployer()
        {
            string lstrPrioityValue = string.Empty;
            if (ibusExpeditedProvider.IsNull())
                LoadExpeditedProvider();

            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();

            if (istrIsExpDefCompWaivedChecked != busConstant.Flag_Yes)
            {

                // PIR 9115 - Change message text
                busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                    busConstant.PlanIdDeferredCompensation, iobjPassInfo);

                if (ibusPlan.IsNull())
                    LoadPlan();
            }
            else
            {
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(2, iobjPassInfo, ref lstrPrioityValue), ibusPerson.icdoPerson.FullName, ibusPlan.icdoPlan.mss_plan_name),
                    lstrPrioityValue, aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                        astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
            }
            busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                    busConstant.WSS_MessageBoard_Priority_High, icdoPersonAccount.person_id);

        }

        public string istrIsExpDefCompWaivedChecked { get; set; }
        public bool IsAcknowledgementSelectedWithWaiverFlag()
        {
            if ((iclbMSSExpDefCompAcknowledgements.Any(i => i.ienuObjectState == ObjectState.CheckListInsert))
                && (istrIsExpDefCompWaivedChecked == busConstant.Flag_Yes))
                return true;
            return false;
        }

        #endregion

        //check if already def comp person account exists
        public bool IsMultipleDefCompAccountExists()
        {
            if (ibusPerson.icolPersonAccountByPlan.IsNull())
                ibusPerson.LoadPersonAccountByPlan(busConstant.PlanIdDeferredCompensation);

            if (ibusPerson.icolPersonAccountByPlan.Where(lobjPA => lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled
                && lobjPA.icdoPersonAccount.start_date == icdoPersonAccount.start_date).Count() >= 1)
                return true;
            return false;
        }

        public string istrExpMessage
        {
            get
            {
                DataTable ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='DEG'");
                string lstrExpMessage = string.Format(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_text"].ToString(), idecExpeditedAmount.ToString());
                return lstrExpMessage;
                //PIR 6961
                //return "I understand that by electing to begin participation in the 457 Deferred Compensation Plan, I will reduce my wages by $"
                //    + idecExpeditedAmount.ToString()
                //    + ".00 and vest in the employer’s contributions to the Defined Benefit Retirement Plan, to which I am entitled based on my service credit and level of contribution. My contributions will be invested with the NDPERS Companion Plan.";

            }
        }

        public bool IsDeffCompEnrollmentBeforeHiring()
        {
            if (idtProviderStartDate == DateTime.MinValue)
                LoadCalculatedStartDate();

            if (ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();

            if (idtProviderStartDate < ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date)
            {
                return true;
            }
            return false;
        }

        public decimal idec457Limit { get; set; }

        public busPersonAccountDeferredCompProvider ibusDefCompProvider { get; set; }

        public Collection<cdoOrganization> LoadDeferredCompProvidersNew()
        {
            return ibusDefCompProvider.LoadDeferredCompProviders();
        }

        public Collection<cdoPersonEmployment> LoadDeferredCompEmploymentsNew()
        {
            Collection<cdoPersonEmployment> lclbPersonEmployment = ibusDefCompProvider.LoadDeferredCompEmployments(false);
            if (lclbPersonEmployment.Count == 1)
                ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.person_employment_id = lclbPersonEmployment[0].person_employment_id;
            return lclbPersonEmployment;
        }

        public Collection<cdoCodeValue> LoadStartYear()
        {
            int lintMaxYear = 2100;
            Collection<cdoCodeValue> lclbRetirementYears = new Collection<cdoCodeValue>();
            for (int i = DateTime.Now.Year; i < lintMaxYear; i++)
            {
                cdoCodeValue lcdoCodeValue = new cdoCodeValue();
                lcdoCodeValue.code_value = i.ToString();
                lcdoCodeValue.description = i.ToString();
                lclbRetirementYears.Add(lcdoCodeValue);
            }
            return lclbRetirementYears;
        }

        public Collection<cdoContact> LoadActiveContacts(string astrProviderOrgCode)
        {
            Collection<cdoContact> lclbContacts = new Collection<cdoContact>();
            DataTable ldtbResults = Select("cdoWssPersonAccountDeferredComp.LoadActiveContacts", new object[1] { astrProviderOrgCode });
            foreach (DataRow ldtr in ldtbResults.Rows)
            {
                cdoContact lcdoContact = new cdoContact();
                lcdoContact.LoadData(ldtr);
                lclbContacts.Add(lcdoContact);
            }
            return lclbContacts;
        }
        //PIR 12158
        public bool IsSelectedAgentInActive()
        {
            if (ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.provider_agent_contact_id != 0 && ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.istrProviderOrgCode.IsNotNullOrEmpty())
            {
                int lintContactCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccountDeferredCompProvider.IsDeferredCompAgent", new object[2] { ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.provider_agent_contact_id, ibusDefCompProvider.icdoPersonAccountDeferredCompProvider.istrProviderOrgCode }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                return lintContactCount <= 0 ? true : false;
            }
            return false;
        }

        //PIR 19108
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
        
    }
}
